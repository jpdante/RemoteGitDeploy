using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using HtcSharp.Core.Logging.Abstractions;
using Newtonsoft.Json;
using RemoteGitDeploy.Extensions;
using RemoteGitDeploy.Manager;
using RemoteGitDeploy.Models;
using RemoteGitDeploy.Models.Internal;
using RemoteGitDeploy.Models.New;
using RemoteGitDeploy.Models.Views;

namespace RemoteGitDeploy.Actions {
    public class PullRepositoryAction : IRepositoryAction {

        public string ActionGuid { get; }
        public bool Running { get; private set; }
        public bool Success { get; set; }
        public bool InQueue { get; set; }
        public bool Finished { get; set; }

        public DateTime StartTime { get; private set; }
        public DateTime ExitTime { get; private set; }
        public List<OutputLine> Output { get; }

        private Process _process;
        private string _directory;
        private InternalRepository _internalRepository;

        public PullRepositoryAction() {
            ActionGuid = Guid.NewGuid().ToString();
            Output = new List<OutputLine>();
        }

        public async Task<bool> Start(InternalRepository internalRepository) {
            if (Finished) return false;
            _internalRepository = internalRepository;
            try {
                _directory = Path.Combine(RepositoryManager.RepositoriesPath, internalRepository.RepositoryInfo.Guid);
                if (!Directory.Exists(_directory)) Directory.CreateDirectory(_directory);
                var processStartInfo = new ProcessStartInfo {
                    FileName = "git",
                    Arguments = "pull",
                    WorkingDirectory = _directory,
                    RedirectStandardError = true,
                    RedirectStandardOutput = true,
                };
                _process = new Process { StartInfo = processStartInfo, EnableRaisingEvents = true, };

                _process.OutputDataReceived += ProcessOnOutputDataReceived;
                _process.ErrorDataReceived += ProcessOnErrorDataReceived;
                _process.Exited += ProcessOnExited;

                _process.Start();
                StartTime = DateTime.UtcNow;

                _process.BeginOutputReadLine();
                _process.BeginErrorReadLine();
                Running = true;
                return true;
            } catch (Exception ex) {
                Running = false;
                Success = false;
                Finished = true;
                try {
                    if (Directory.Exists(_directory)) new DirectoryInfo(_directory).DeleteReadOnly();
                } catch (Exception ex2) {
                    HtcPlugin.Logger.LogError(ex2);
                    // ignored
                }
                ExitTime = DateTime.UtcNow;
                HtcPlugin.Logger.LogError(ex);
                await SaveActionToHistory();
                OnFinish?.Invoke(this);
                return false;
            }
        }

        public async Task<bool> Cancel() {
            try {
                if (_process != null && !_process.HasExited) _process.Kill(true);
                Running = false;
                Success = false;
                Finished = true;
                ExitTime = DateTime.UtcNow;
                Output.Add(new OutputLine($"The process was killed by the manager!", (DateTime.UtcNow - StartTime).Milliseconds));
                _internalRepository.LastUpdate = DateTime.UtcNow;
                await SaveActionToHistory();
                OnFinish?.Invoke(this);
                return true;
            } catch (Exception ex) {
                HtcPlugin.Logger.LogError(ex);
                return false;
            }
        }

        public ActionHistoryCompactView GetFutureActionHistoryCompactView() {
            return new ActionHistoryCompactView {
                Id = 0,
                Guid = ActionGuid,
                CreationDate = "N/A",
                FinishTime = "N/A",
                StartTime = "N/A",
                Icon = 4,
                Name = "Pull repository",
                Status = -1
            };
        }

        public event IRepositoryAction.OnFinishDelegate OnFinish;

        private async void ProcessOnExited(object sender, EventArgs e) {
            try {
                Running = false;
                Finished = true;
                Success = _process.ExitCode == 0;
                if (!Success) {
                    new DirectoryInfo(_directory).DeleteReadOnly();
                }
                ExitTime = DateTime.UtcNow;
                Output.Add(new OutputLine($"The process ended with code: {_process.ExitCode}", (DateTime.UtcNow - StartTime).Milliseconds));
            } catch (Exception ex) {
                HtcPlugin.Logger.LogError(ex);
            }
            _internalRepository.LastUpdate = DateTime.UtcNow;
            await SaveActionToHistory();
            OnFinish?.Invoke(this);
        }

        private async Task SaveActionToHistory() {
            await using var context = new RgdContext();
            int exitCode = 1;
            if (_process != null) exitCode = _process.ExitCode;
            await context.ActionHistory.AddAsync(
                new ActionHistory(
                    ActionGuid,
                    _internalRepository.RepositoryInfo.Id,
                    2,
                    "Pull repository",
                    null,
                    JsonConvert.SerializeObject(Output),
                    exitCode,
                    StartTime,
                    ExitTime)
            );
            await context.SaveChangesAsync();
        }

        private void ProcessOnErrorDataReceived(object sender, DataReceivedEventArgs e) {
            try {
                if (!string.IsNullOrEmpty(e.Data)) Output.Add(new OutputLine(e.Data, (DateTime.UtcNow - StartTime).Milliseconds));
            } catch (Exception ex) {
                HtcPlugin.Logger.LogError(ex);
            }
        }

        private void ProcessOnOutputDataReceived(object sender, DataReceivedEventArgs e) {
            try {
                if (!string.IsNullOrEmpty(e.Data)) Output.Add(new OutputLine(e.Data, (DateTime.UtcNow - StartTime).Milliseconds));
            } catch (Exception ex) {
                HtcPlugin.Logger.LogError(ex);
            }
        }
    }
}
