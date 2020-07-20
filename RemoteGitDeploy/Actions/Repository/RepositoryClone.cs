using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using HtcSharp.Core.Logging.Abstractions;
using RemoteGitDeploy.Actions.Data;
using RemoteGitDeploy.Manager;
using RemoteGitDeploy.Model;

namespace RemoteGitDeploy.Actions.Repository {
    public class RepositoryClone : IRepositoryAction {

        private readonly string _gitLink;
        private readonly string _directory;
        private Process _process;

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

        public RepositoryClone(string repositoryGuid, IActionData data, string gitLink, string directory) {
            RepositoryGuid = repositoryGuid;
            Data = data;
            _gitLink = gitLink;
            _directory = directory;
            Running = false;
            Success = false;
            Output = new List<OutputLine>();
            DeleteDelay = 60;
            KillDelay = 300;
            InQueue = true;
        }

        public bool Start() {
            if (!RepositoryManager.BreakGitLink(_gitLink, out string scheme, out string domain, out string path)) return false;
            try {
                if (!Directory.Exists(_directory)) Directory.CreateDirectory(_directory);
                var processStartInfo = new ProcessStartInfo {
                    FileName = "git",
                    Arguments = $"clone {scheme}://{HtcPlugin.RepositoryManager.GitUsername}:{HtcPlugin.RepositoryManager.GitPersonalAccessToken}@{domain}{path} {_directory}",
                    WorkingDirectory = _directory,
                    RedirectStandardError = true,
                    RedirectStandardOutput = true,
                };
                _process = new Process {
                    StartInfo = processStartInfo,
                    EnableRaisingEvents = true,
                };

                _process.OutputDataReceived += ProcessOnOutputDataReceived;
                _process.ErrorDataReceived += ProcessOnErrorDataReceived;
                _process.Exited += ProcessOnExited;

                _process.Start();
                StartTime = DateTime.Now;

                _process.BeginOutputReadLine();
                _process.BeginErrorReadLine();
                Running = true;
                return true;
            } catch (Exception ex) {
                Running = false;
                Success = false;
                try {
                    Directory.Delete(_directory, true);
                } catch {
                    // ignored
                }
                ExitTime = DateTime.Now;
                HtcPlugin.Logger.LogError(ex);
                OnFinish?.Invoke(this, Data);
                return false;
            }
        }

        public void ForceKill() {
            if (_process != null && !_process.HasExited) _process.Kill(true);
            Running = false;
            Success = false;
            ExitTime = DateTime.Now;
            Output.Add(new OutputLine($"The process was killed by the manager!", (DateTime.Now - StartTime).Milliseconds));
            OnFinish?.Invoke(this, Data);
        }

        public event IRepositoryAction.OnFinishDelegate OnFinish;

        private void ProcessOnExited(object sender, EventArgs e) {
            Running = false;
            Success = _process.ExitCode == 0;
            if (!Success) {
                Directory.Delete(_directory, true);
            }
            ExitTime = DateTime.Now;
            Output.Add(new OutputLine($"The process ended with code: {_process.ExitCode}", (DateTime.Now - StartTime).Milliseconds));
            OnFinish?.Invoke(this, Data);
        }

        private void ProcessOnErrorDataReceived(object sender, DataReceivedEventArgs e) {
            Output.Add(new OutputLine(e.Data, (DateTime.Now - StartTime).Milliseconds));
        }

        private void ProcessOnOutputDataReceived(object sender, DataReceivedEventArgs e) {
            Output.Add(new OutputLine(e.Data, (DateTime.Now - StartTime).Milliseconds));
        }
    }
}