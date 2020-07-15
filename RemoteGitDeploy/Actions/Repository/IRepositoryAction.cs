using System;
using System.Collections.Generic;
using System.Text;
using RemoteGitDeploy.Model;

namespace RemoteGitDeploy.Actions.Repository {
    public interface IRepositoryAction {
        bool Running { get; }
        bool Success { get; }
        public DateTime StartTime { get; }
        public DateTime ExitTime { get; }
        public List<OutputLine> Output { get; }
        bool Start();
        void ForceKill();
    }
}
