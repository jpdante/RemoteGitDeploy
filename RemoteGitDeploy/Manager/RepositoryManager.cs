using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using System.Transactions;
using RemoteGitDeploy.Actions.Data;
using RemoteGitDeploy.Actions.Data.Repository;
using RemoteGitDeploy.Actions.Repository;
using RemoteGitDeploy.Model;
using RemoteGitDeploy.Utils;

namespace RemoteGitDeploy.Manager {
    public class RepositoryManager {

        internal readonly string GitUsername;
        internal readonly string GitPersonalAccessToken;
        internal readonly string GitRepositoriesDirectory;

        private readonly List<IRepositoryAction> _activeActions;
        private readonly Timer _timer;

        public RepositoryManager(string gitUsername, string gitPersonalAccessToken, string gitRepositoriesDirectory) {
            GitUsername = gitUsername;
            GitPersonalAccessToken = gitPersonalAccessToken;
            GitRepositoriesDirectory = gitRepositoriesDirectory;
            _activeActions = new List<IRepositoryAction>();
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

        public void Start() {
            _timer.Start();
        }

        public void Stop() {
            _timer.Stop();
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
            await using var conn = await HtcPlugin.DatabaseManager.GetConnectionAsync();
            long id = StaticData.IdGenerator.CreateId();
            if (await HtcPlugin.DatabaseManager.NewRepositoryAsync(id, data.Id, data.Account, data.Name, data.Git, data.Team, data.Description, conn)) {
                action.Success = true;
            } else {
                action.Success = false;
            }
            action.Finished = true;
        }

        private void OnActionFinish(IRepositoryAction action, IActionData data) {
            switch (data.Type) {
                case ActionDataType.NewRepository:
                    var repositoryCloneData = data as RepositoryCloneData;
                    if (!action.Running) {
                        if (action.Success) {
                            Task.Run(async () => await CreateRepository(action, repositoryCloneData));
                        } else {
                            action.Finished = true;
                        }
                    }
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