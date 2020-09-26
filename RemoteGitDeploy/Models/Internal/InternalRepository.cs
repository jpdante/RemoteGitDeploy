using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using RemoteGitDeploy.Actions;
using RemoteGitDeploy.Models.New;

namespace RemoteGitDeploy.Models.Internal {
    public class InternalRepository {

        public Repository RepositoryInfo { get; }
        public Queue<IRepositoryAction> QueuedActions { get; }
        public IRepositoryAction CurrentAction { get; private set; }
        public string LastCommit { get; internal set; }
        public DateTime LastUpdate { get; internal set; }

        public InternalRepository(Repository repositoryInfo) {
            RepositoryInfo = repositoryInfo;
            QueuedActions = new Queue<IRepositoryAction>();
        }

        public async Task AddAction(IRepositoryAction repositoryAction) {
            QueuedActions.Enqueue(repositoryAction);
            await ProcessRepositoryAction();
        }

        public async Task ProcessRepositoryAction() {
            if (CurrentAction != null) return;
            if (QueuedActions.TryDequeue(out var action)) {
                CurrentAction = action;
                action.OnFinish += ActionOnOnFinish;
                await action.Start(this);
            }
        }

        private async void ActionOnOnFinish(IRepositoryAction action) {
            CurrentAction = null;
            await ProcessRepositoryAction();
        }
    }
}
