using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Timers;
using HtcSharp.Core.Logging.Abstractions;
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

        private readonly List<IRepositoryAction> _activeActions;
        private readonly Timer _timer;
        private readonly Dictionary<string, Repository> _repositories;

        public RepositoryManager(string gitUsername, string gitPersonalAccessToken, string gitRepositoriesDirectory) {
            GitUsername = gitUsername;
            GitPersonalAccessToken = gitPersonalAccessToken;
            GitRepositoriesDirectory = gitRepositoriesDirectory;
            _activeActions = new List<IRepositoryAction>();
            _repositories = new Dictionary<string, Repository>();
            _timer = new Timer {
                Interval = 5000
            };
            _timer.Elapsed += TimerOnElapsed;
        }

        private void TimerOnElapsed(object sender, ElapsedEventArgs e) {
            for (var i = 0; i < _activeActions.Count; i++) {
                _activeActions[i].DeleteDelay--;
                if (_activeActions[i].DeleteDelay <= 0) {
                    _activeActions.RemoveAt(i);
                }
            }
            foreach (var action in _activeActions) {
                action.KillDelay--;
                if (action.KillDelay > 0) continue;
                action.ForceKill();
                action.KillDelay = 300;
            }
        }

        public Repository GetRepository(string guid) => _repositories.TryGetValue(guid, out var repository) ? repository : null;

        public async Task Start() {
            _timer.Start();
            _repositories.Clear();
            await using var conn = await HtcPlugin.DatabaseManager.GetConnectionAsync();
            FullRepository[] repositories = await HtcPlugin.DatabaseManager.GetFullRepositoriesAsync(conn);
            foreach (var repository in repositories) {
                _repositories.Add(repository.Guid, new Repository(repository.Id, repository.Guid, repository.Name, repository.Git));
            }
            foreach (KeyValuePair<string, Repository> repository in _repositories) {
                HtcPlugin.Logger.LogInfo($"Initializing repository {repository.Value.Name} [{repository.Key}]...");
                repository.Value.LastCommit = await GetLocalRepositoryLastCommit(repository.Value.Guid);
                string lastRemoteCommit = await GetRemoteRepositoryLastCommit(repository.Value.Guid, repository.Value.Git);
                if (!repository.Value.LastCommit.Equals(lastRemoteCommit)) {
                    HtcPlugin.Logger.LogInfo($"Pulling repository {repository.Value.Name} [{repository.Key}]...");
                    var action = new RepositoryPull(new RepositoryPullData(repository.Key), Path.Combine(GitRepositoriesDirectory, repository.Value.Guid));
                    action.OnFinish += OnActionFinish;
                    _activeActions.Add(action);
                    action.Start();
                }
                repository.Value.LastUpdate = DateTime.UtcNow;
            }
        }

        public void Stop() {
            _timer.Stop();
            _repositories.Clear();
        }

        public IRepositoryAction GetRepositoryAction(string guid) => _activeActions.FirstOrDefault(action => action.Data.Id.Equals(guid));

        public void CreateRepository(RepositoryCloneData repositoryCloneData) {
            var repositoryClone = new RepositoryClone(repositoryCloneData, repositoryCloneData.Git, Path.Combine(GitRepositoriesDirectory, repositoryCloneData.Id));
            repositoryClone.OnFinish += OnActionFinish;
            _activeActions.Add(repositoryClone);
            repositoryClone.Start();
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
                action.Success = await HtcPlugin.DatabaseManager.NewRepositoryAsync(id, data.Id, data.Account, data.Name, data.Git, data.Team, data.Description, conn);
                if (action.Success) {
                    var repository = new Repository(id, data.Id, data.Name, data.Git) {
                        LastCommit = await GetLocalRepositoryLastCommit(data.Id),
                        LastUpdate = DateTime.UtcNow
                    };
                    _repositories.Add(data.Id, repository);
                    var parameters = new List<Model.Database.RepositoryHistory.Parameter> { new RepositoryHistory.Parameter("Clone status", "Success", "success") };
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
                    repository.LastCommit = await GetLocalRepositoryLastCommit(repository.Guid);
                    repository.LastUpdate = DateTime.UtcNow;
                    await using var conn = await HtcPlugin.DatabaseManager.GetConnectionAsync();
                    var parameters = new List<Model.Database.RepositoryHistory.Parameter>();
                    if (action.Success) {
                        parameters.Add(new RepositoryHistory.Parameter("Pull status", "Success", "success"));
                    } else {
                        parameters.Add(new RepositoryHistory.Parameter("Pull status", "Failed", "danger"));
                        string log = action.Output.Aggregate("", (current, output) => current + (output.Data + Environment.NewLine));
                        parameters.Add(new RepositoryHistory.Parameter("Log", log, "dark"));
                    }
                    await HtcPlugin.DatabaseManager.NewRepositoryHistoryAsync(repository.Id, 2, $"Pull repository '{repository.Name}'", parameters, conn);
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

        public async Task<string> GetLocalRepositoryLastCommit(string guid) {
            try {
                var processStartInfo = new ProcessStartInfo {
                    FileName = "git",
                    Arguments = "rev-parse HEAD",
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

        public async Task<string> GetRemoteRepositoryLastCommit(string guid, string gitLink) {
            if (!BreakGitLink(gitLink, out string scheme, out string domain, out string path)) return null;
            try {
                var processStartInfo = new ProcessStartInfo {
                    FileName = "git",
                    Arguments = $"ls-remote {scheme}://{GitUsername}:{GitPersonalAccessToken}@{domain}{path} HEAD",
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
                case ActionDataType.Unknown:
                default:
                    break;
            }
        }

        public static bool BreakGitLink(string link, out string scheme, out string domain, out string path) {
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
        }

    }
}