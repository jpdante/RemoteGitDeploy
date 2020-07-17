using System;
using System.Collections.Generic;
using System.Text;
using RemoteGitDeploy.Actions.Data;
using RemoteGitDeploy.Model;

namespace RemoteGitDeploy.Actions.Repository {
    public interface IRepositoryAction {
        public bool Running { get; }
        public bool Success { get; set; }
        public bool Finished { get; set; }
        public int DeleteDelay { get; set; }
        public int KillDelay { get; set; }
        public IActionData Data { get; }
        public DateTime StartTime { get; }
        public DateTime ExitTime { get; }
        public List<OutputLine> Output { get; }
        public bool Start();
        public void ForceKill();
        public delegate void OnFinishDelegate(IRepositoryAction action, IActionData data);
        public event OnFinishDelegate OnFinish;
    }
}
