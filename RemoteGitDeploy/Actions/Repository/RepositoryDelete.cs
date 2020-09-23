using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using HtcSharp.Core.Logging.Abstractions;
using RemoteGitDeploy.Actions.Data;
using RemoteGitDeploy.Models;

namespace RemoteGitDeploy.Actions.Repository {
    public class RepositoryDelete : IRepositoryAction {

        private readonly string _directory;
        public string RepositoryGuid { get; }
        public bool Running { get; private set; }
        public bool Success { get; set; }
        public bool InQueue { get; set; }
        public bool Finished { get; set; }
        public int DeleteDelay { get; set; }
        public int KillDelay { get; set; }
        public DateTime StartTime { get; private set; }
        public DateTime ExitTime { get; private set; }
        public List<OutputLine> Output { get; }
        public IActionData Data { get; }

        public RepositoryDelete(string repositoryGuid, IActionData data, string directory) {
            RepositoryGuid = repositoryGuid;
            Data = data;
            _directory = directory;
            Running = false;
            Success = false;
            Output = new List<OutputLine>();
            DeleteDelay = 60;
            KillDelay = 300;
            InQueue = true;
        }

        public bool Start() {
            try {
                StartTime = DateTime.UtcNow;
                if (Directory.Exists(_directory)) Directory.Delete(_directory, true);
                Running = false;
                Success = true;
                ExitTime = DateTime.Now;
                OnFinish?.Invoke(this, Data);
                return true;
            } catch (Exception ex) {
                Running = false;
                Success = false;
                ExitTime = DateTime.Now;
                HtcPlugin.Logger.LogError(ex);
                OnFinish?.Invoke(this, Data);
                return false;
            }
        }

        public void ForceKill() {
            try {
                Running = false;
                Success = false;
                ExitTime = DateTime.Now;
                Output.Add(new OutputLine($"The process was killed by the manager!", (DateTime.Now - StartTime).Milliseconds));
                OnFinish?.Invoke(this, Data);
            } catch (Exception ex) {
                HtcPlugin.Logger.LogError(ex);
            }
        }

        public event IRepositoryAction.OnFinishDelegate OnFinish;
    }
}