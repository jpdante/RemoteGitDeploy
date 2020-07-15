using System;
using System.Collections.Generic;
using System.Diagnostics;
using HtcSharp.Core.Logging.Abstractions;
using RemoteGitDeploy.Manager;
using RemoteGitDeploy.Model;

namespace RemoteGitDeploy.Actions.Repository {
    public class RepositoryClone : IRepositoryAction {

        private readonly string _gitLink;
        private readonly string _directory;
        private Process _process;

        public bool Running { get; private set; }
        public bool Success { get; private set; }
        public DateTime StartTime { get; private set; }
        public DateTime ExitTime { get; private set; }
        public List<OutputLine> Output { get; }

        public RepositoryClone(string gitLink, string directory) {
            _gitLink = gitLink;
            _directory = directory;
            Running = false;
            Success = false;
            Output = new List<OutputLine>();
        }

        public bool Start() {
            if (!RepositoryManager.BreakGitLink(_gitLink, out string scheme, out string domain, out string path)) return false;
            try {
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
                HtcPlugin.Logger.LogError(ex);
                return false;
            }
        }

        public void ForceKill() {
            _process.Kill(true);
            Running = false;
            Success = false;
            ExitTime = DateTime.Now;
            Output.Add(new OutputLine($"The process was killed by the manager!", (DateTime.Now - StartTime).Milliseconds));
        }

        private void ProcessOnExited(object sender, EventArgs e) {
            Running = false;
            Success = _process.ExitCode == 0;
            ExitTime = DateTime.Now;
            Output.Add(new OutputLine($"The process ended with code: {_process.ExitCode}", (DateTime.Now - StartTime).Milliseconds));
        }

        private void ProcessOnErrorDataReceived(object sender, DataReceivedEventArgs e) {
            Output.Add(new OutputLine(e.Data, (DateTime.Now - StartTime).Milliseconds));
        }

        private void ProcessOnOutputDataReceived(object sender, DataReceivedEventArgs e) {
            Output.Add(new OutputLine(e.Data, (DateTime.Now - StartTime).Milliseconds));
        }
    }
}