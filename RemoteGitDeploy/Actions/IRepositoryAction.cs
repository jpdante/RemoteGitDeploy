using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using RemoteGitDeploy.Models;
using RemoteGitDeploy.Models.Internal;
using RemoteGitDeploy.Models.Views;

namespace RemoteGitDeploy.Actions {
    public interface IRepositoryAction {
        public string ActionGuid { get; }
        public bool Running { get; }
        public bool Success { get; set; }
        public bool InQueue { get; set; }
        public bool Finished { get; set; }
        public DateTime StartTime { get; }
        public DateTime ExitTime { get; }
        public List<OutputLine> Output { get; }

        public Task<bool> Start(InternalRepository internalRepository);
        public Task<bool> Cancel();
        public ActionHistoryCompactView GetFutureActionHistoryCompactView();

        public delegate void OnFinishDelegate(IRepositoryAction action);
        public event OnFinishDelegate OnFinish;
    }
}
