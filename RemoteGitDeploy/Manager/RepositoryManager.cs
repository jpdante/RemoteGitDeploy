using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using System.Timers;
using HtcSharp.Core.Logging.Abstractions;
using Newtonsoft.Json;
using RemoteGitDeploy.Actions.Data;
using RemoteGitDeploy.Actions.Data.Repository;
using RemoteGitDeploy.Actions.Repository;
using RemoteGitDeploy.Model.Database;
using RemoteGitDeploy.Utils;
using Repository = RemoteGitDeploy.Model.Repository;

namespace RemoteGitDeploy.Manager {
    public class RepositoryManager {

        internal readonly string GitUsername;
        internal readonly string GitPersonalAccessToken;
        internal readonly string GitRepositoriesDirectory;

        private readonly List<IRepositoryAction> _finishedActions;
        private readonly List<IRepositoryAction> _activeActions;
        private readonly List<IRepositoryAction> _queuedActions;
        private readonly Timer _timer;
        private readonly Dictionary<string, Repository> _repositories;

        public RepositoryManager(string gitUsername, string gitPersonalAccessToken, string gitRepositoriesDirectory) {
            GitUsername = gitUsername;
            GitPersonalAccessToken = gitPersonalAccessToken;
            GitRepositoriesDirectory = gitRepositoriesDirectory;
            _finishedActions = new List<IRepositoryAction>();
            _activeActions = new List<IRepositoryAction>();
            _queuedActions = new List<IRepositoryAction>();
            _repositories = new Dictionary<string, Repository>();
            _timer = new Timer {
                Interval = 1000
            };
            _timer.Elapsed += TimerOnElapsed;
        }

        private void TimerOnElapsed(object sender, ElapsedEventArgs e) {
            for (var i = 0; i < _finishedActions.Count; i++) {
                _finishedActions[i].DeleteDelay--;
                if (_finishedActions[i].DeleteDelay <= 0) {
                    _finishedActions.RemoveAt(i);
                }
            }
            foreach (var action in _activeActions) {
                action.KillDelay--;
                if (action.KillDelay > 0) continue;
                action.ForceKill();
                action.KillDelay = 300;
            }
            for (var i = 0; i < _queuedActions.Count; i++) {
                bool hasActive = _activeActions.Any(action => action.RepositoryGuid.Equals(_queuedActions[i].RepositoryGuid));
                if (hasActive) continue;
                _activeActions.Add(_queuedActions[i]);
                _queuedActions[i].InQueue = false;
                _queuedActions[i].Start();
                _queuedActions.RemoveAt(i);
            }
        }

        public Repository GetRepository(string guid) => _repositories.TryGetValue(guid, out var repository) ? repository : null;

        public async Task Start() {
            _timer.Start();
            _repositories.Clear();
            await using var conn = await HtcPlugin.DatabaseManager.GetConnectionAsync();
            FullRepository[] repositories = await HtcPlugin.DatabaseManager.GetFullRepositoriesAsync(conn);
            foreach (var repository in repositories) {
                _repositories.Add(repository.Guid, new Repository(repository.Id, repository.Guid, repository.Name, repository.Git, repository.Branch));
            }
            foreach (KeyValuePair<string, Repository> repository in _repositories) {
                HtcPlugin.Logger.LogInfo($"Initializing repository {repository.Value.Name} [{repository.Key}]...");
                repository.Value.LastCommit = await GetLocalRepositoryLastCommit(repository.Value.Guid, repository.Value.Branch);
                string lastRemoteCommit = await GetRemoteRepositoryLastCommit(repository.Value.Guid, repository.Value.Git, repository.Value.Branch);
                if (!repository.Value.LastCommit.Equals(lastRemoteCommit)) {
                    HtcPlugin.Logger.LogInfo($"Pulling repository {repository.Value.Name} [{repository.Key}]...");
                    var action = new RepositoryPull(repository.Value.Guid, new RepositoryPullData(repository.Value.Guid), Path.Combine(GitRepositoriesDirectory, repository.Value.Guid));
                    action.OnFinish += OnActionFinish;
                    _queuedActions.Add(action);
                    action.Start();
                }
                repository.Value.LastUpdate = DateTime.UtcNow;
            }
        }

        public void Stop() {
            _timer.Stop();
            _repositories.Clear();
        }

        public IRepositoryAction GetRepositoryAction(string guid) {
            var result = _activeActions.FirstOrDefault(action => action.Data.Id.Equals(guid));
            if (result != null) return result;
            result = _finishedActions.FirstOrDefault(action => action.Data.Id.Equals(guid));
            return result ?? _queuedActions.FirstOrDefault(action => action.Data.Id.Equals(guid));
        }

        public void CreateRepository(RepositoryCloneData repositoryCloneData) {
            var repositoryClone = new RepositoryClone(repositoryCloneData.Id, repositoryCloneData, repositoryCloneData.Git, repositoryCloneData.Branch, Path.Combine(GitRepositoriesDirectory, repositoryCloneData.Id));
            repositoryClone.OnFinish += OnActionFinish;
            _queuedActions.Add(repositoryClone);
        }

        public async Task PullRepository(Repository repository) {
            repository.LastCommit = await GetLocalRepositoryLastCommit(repository.Guid, repository.Branch);
            string lastRemoteCommit = await GetRemoteRepositoryLastCommit(repository.Guid, repository.Git, repository.Branch);
            if (!repository.LastCommit.Equals(lastRemoteCommit)) {
                var action = new RepositoryPull(repository.Guid, new RepositoryPullData(repository.Guid), Path.Combine(GitRepositoriesDirectory, repository.Guid));
                action.OnFinish += OnActionFinish;
                _queuedActions.Add(action);
            }
        }

        public void DeleteRepository(Repository repository) {
            var action = new RepositoryDelete(repository.Guid, new RepositoryDeleteData(repository.Guid), Path.Combine(GitRepositoriesDirectory, repository.Guid));
            action.OnFinish += OnActionFinish;
            _queuedActions.Add(action);
        }

        public bool HasProcessingRepository(string name) {
            foreach (var action in _activeActions.Where(action => !action.Finished)) {
                switch (action.Data.Type) {
                    case ActionDataType.NewRepository:
                        if (action.Data is RepositoryCloneData repositoryCloneData && repositoryCloneData.Name.Equals(name)) return true;
                        break;
                    case ActionDataType.Unknown:
                    default:
                        break;
                }
            }
            return false;
        }

        private async Task CreateRepository(IRepositoryAction action, RepositoryCloneData data) {
            try {
                await using var conn = await HtcPlugin.DatabaseManager.GetConnectionAsync();
                long id = StaticData.IdGenerator.CreateId();
                action.Success = await HtcPlugin.DatabaseManager.NewRepositoryAsync(id, data.Id, data.Account, data.Name, data.Git, data.Branch, data.Team, data.Description, conn);
                if (action.Success) {
                    var repository = new Repository(id, data.Id, data.Name, data.Git, data.Branch) {
                        LastCommit = await GetLocalRepositoryLastCommit(data.Id, data.Branch),
                        LastUpdate = DateTime.UtcNow
                    };
                    _repositories.Add(data.Id, repository);
                    var parameters = new List<RepositoryHistory.Parameter> { new RepositoryHistory.Parameter("Clone status", "Success", "success") };
                    await HtcPlugin.DatabaseManager.NewRepositoryHistoryAsync(id, 1, $"Clone repository '{data.Name}'", parameters, conn);
                }
                action.Finished = true;
            } catch (Exception ex) {
                HtcPlugin.Logger.LogError(ex);
                action.Success = false;
                action.Finished = true;
            }
        }

        private async Task PullRepository(IRepositoryAction action, RepositoryPullData data) {
            try {
                if (_repositories.TryGetValue(data.RepositoryGuid, out var repository)) {
                    repository.LastCommit = await GetLocalRepositoryLastCommit(repository.Guid, repository.Branch);
                    repository.LastUpdate = DateTime.UtcNow;
                    await using var conn = await HtcPlugin.DatabaseManager.GetConnectionAsync();
                    var parameters = new List<RepositoryHistory.Parameter>();
                    if (action.Success) {
                        parameters.Add(new RepositoryHistory.Parameter("Pull status", "Success", "success"));
                    } else {
                        parameters.Add(new RepositoryHistory.Parameter("Pull status", "Failed", "danger"));
                        string log = action.Output.Aggregate("", (current, output) => current + (output.Data + Environment.NewLine));
                        parameters.Add(new RepositoryHistory.Parameter("Log", log, "dark"));
                    }
                    await HtcPlugin.DatabaseManager.NewRepositoryHistoryAsync(repository.Id, 2, $"Pull repository '{repository.Name}'", parameters, conn);
                    repository.LastUpdate = DateTime.UtcNow;
                    string actionFile = Path.Combine(GitRepositoriesDirectory, action.RepositoryGuid, ".rgd/run.lua");
                    string repositoryDirectory = Path.Combine(GitRepositoriesDirectory, action.RepositoryGuid);
                    if (File.Exists(Path.Combine(GitRepositoriesDirectory, action.RepositoryGuid, ".rgd/run.lua"))) {
                        var repositoryAction = new RepositoryAction(repository.Guid, repository.Branch, new RepositoryActionData(repository.Guid), actionFile, repositoryDirectory);
                        repositoryAction.OnFinish += OnActionFinish;
                        _queuedActions.Add(repositoryAction);
                    }
                } else {
                    action.Success = false;
                    action.Finished = true;
                }
            } catch (Exception ex) {
                HtcPlugin.Logger.LogError(ex);
                action.Success = false;
                action.Finished = true;
            }
        }

        public async Task RunRepositoryAction(IRepositoryAction action, RepositoryActionData data) {
            try {
                if (_repositories.TryGetValue(data.RepositoryGuid, out var repository)) {
                    repository.LastUpdate = DateTime.UtcNow;
                    await using var conn = await HtcPlugin.DatabaseManager.GetConnectionAsync();
                    var parameters = new List<RepositoryHistory.Parameter> {
                        action.Success ?
                            new RepositoryHistory.Parameter("Execution status", "Success", "success") :
                            new RepositoryHistory.Parameter("Execution status", "Failed", "danger")
                    };
                    await HtcPlugin.DatabaseManager.NewRepositoryHistoryAsync(repository.Id, 3, $"Execute action '{repository.Name}'", parameters, conn);
                    parameters = new List<RepositoryHistory.Parameter> {
                        action.Success ?
                            new RepositoryHistory.Parameter("Status", "Success", "success") :
                            new RepositoryHistory.Parameter("Status", "Failed", "danger")
                    };
                    string content = JsonConvert.SerializeObject(action.Output, Formatting.None);
                    await HtcPlugin.DatabaseManager.NewActionHistoryAsync(data.Id, repository.Id, $"Execute action '{data.Id}'", parameters, content, action.Success, conn);
                }
            } catch (Exception ex) {
                HtcPlugin.Logger.LogError(ex);
                action.Success = false;
                action.Finished = true;
            }
        }

        private async Task DeleteRepository(IRepositoryAction action, RepositoryDeleteData data) {
            try {
                if (_repositories.TryGetValue(data.RepositoryGuid, out var repository)) {
                    await using var conn = await HtcPlugin.DatabaseManager.GetConnectionAsync();
                    if (await HtcPlugin.DatabaseManager.DeleteRepositoryAsync(repository.Id, conn)) {
                        _repositories.Remove(data.RepositoryGuid);
                    }
                }
            } catch (Exception ex) {
                HtcPlugin.Logger.LogError(ex);
                action.Success = false;
                action.Finished = true;
            }
        }

        public async Task<string> GetLocalRepositoryLastCommit(string guid, string branch) {
            try {
                var processStartInfo = new ProcessStartInfo {
                    FileName = "git",
                    Arguments = $"rev-parse {branch}",
                    WorkingDirectory = Path.Combine(GitRepositoriesDirectory, guid),
                    RedirectStandardError = true,
                    RedirectStandardOutput = true,
                };
                var process = new Process {
                    StartInfo = processStartInfo,
                    EnableRaisingEvents = true,
                };
                process.Start();
                string data = await process.StandardOutput.ReadToEndAsync();
                return data.Length >= 40 ? data.Substring(0, 40) : data;
            } catch (Exception ex) {
                HtcPlugin.Logger.LogError(ex);
                return null;
            }
        }

        public async Task<string> GetRemoteRepositoryLastCommit(string guid, string gitLink, string branch) {
            if (!BreakGitLink(gitLink, out string scheme, out string domain, out string path)) return null;
            try {
                var processStartInfo = new ProcessStartInfo {
                    FileName = "git",
                    Arguments = $"ls-remote {scheme}://{GitUsername}:{GitPersonalAccessToken}@{domain}{path} {branch}",
                    WorkingDirectory = Path.Combine(GitRepositoriesDirectory, guid),
                    RedirectStandardError = true,
                    RedirectStandardOutput = true,
                };
                var process = new Process {
                    StartInfo = processStartInfo,
                    EnableRaisingEvents = true,
                };
                process.Start();
                string data = await process.StandardOutput.ReadToEndAsync();
                return data.Length >= 40 ? data.Substring(0, 40) : data;
            } catch (Exception ex) {
                HtcPlugin.Logger.LogError(ex);
                return null;
            }
        }

        private void OnActionFinish(IRepositoryAction action, IActionData data) {
            _activeActions.Remove(action);
            _finishedActions.Add(action);
            switch (data.Type) {
                case ActionDataType.NewRepository:
                    if (!(data is RepositoryCloneData repositoryCloneData)) return;
                    if (!action.Running) {
                        if (action.Success) {
                            Task.Run(async () => await CreateRepository(action, repositoryCloneData));
                        } else {
                            action.Finished = true;
                        }
                    }
                    break;
                case ActionDataType.PullRepository:
                    if (!(data is RepositoryPullData repositoryPullData)) return;
                    Task.Run(async () => await PullRepository(action, repositoryPullData));
                    break;
                case ActionDataType.RunAction:
                    if (!(data is RepositoryActionData repositoryActionData)) return;
                    Task.Run(async () => await RunRepositoryAction(action, repositoryActionData));
                    break;
                case ActionDataType.DeleteRepository:
                    if (!(data is RepositoryDeleteData repositoryDeleteData)) return;
                    Task.Run(async () => await DeleteRepository(action, repositoryDeleteData));
                    break;
                case ActionDataType.Unknown:
                default:
                    break;
            }
        }

        public static bool BreakGitLink(string link, out string scheme, out string domain, out string path) {
            try {
                if (!Uri.TryCreate(link, UriKind.RelativeOrAbsolute, out var uri)) {
                    scheme = null;
                    domain = null;
                    path = null;
                    return false;
                }
                scheme = uri.Scheme;
                domain = uri.Host;
                path = uri.AbsolutePath;
                return true;
            } catch {
                scheme = null;
                domain = null;
                path = null;
                return false;
            }
        }

    }
}