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
    public class CloneRepositoryAction : IRepositoryAction {

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

        public CloneRepositoryAction() {
            ActionGuid = Guid.NewGuid().ToString();
            Output = new List<OutputLine>();
        }

        public async Task<bool> Start(InternalRepository internalRepository) {
            if (Finished) return false;
            if (!RepositoryManager.BreakGitLink(internalRepository.RepositoryInfo.Git, out string scheme, out string domain, out string path)) return false;
            _internalRepository = internalRepository;
            try {
                _directory = Path.Combine(RepositoryManager.RepositoriesPath, internalRepository.RepositoryInfo.Guid);
                if (!Directory.Exists(_directory)) Directory.CreateDirectory(_directory);
                var processStartInfo = new ProcessStartInfo {
                    FileName = "git",
                    Arguments = $"clone -b {internalRepository.RepositoryInfo.Branch} {scheme}://{internalRepository.RepositoryInfo.Username}:{internalRepository.RepositoryInfo.PersonalAccessToken}@{domain}{path} {_directory}",
                    WorkingDirectory = _directory,
                    RedirectStandardError = true,
                    RedirectStandardOutput = true,
                    RedirectStandardInput = true,
                };
                _process = new Process { StartInfo = processStartInfo, EnableRaisingEvents = true, };

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
                Finished = true;
                try {
                    if (Directory.Exists(_directory)) new DirectoryInfo(_directory).DeleteReadOnly();
                } catch (Exception ex2) {
                    HtcPlugin.Logger.LogError(ex2);
                    // ignored
                }
                ExitTime = DateTime.Now;
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
                ExitTime = DateTime.Now;
                Output.Add(new OutputLine($"The process was killed by the manager!", (DateTime.Now - StartTime).Milliseconds));
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
                Name = "Clone repository",
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
                ExitTime = DateTime.Now;
                Output.Add(new OutputLine($"The process ended with code: {_process.ExitCode}", (DateTime.Now - StartTime).Milliseconds));
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
                    1,
                    "Clone repository",
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
                if (!string.IsNullOrEmpty(e.Data)) Output.Add(new OutputLine(e.Data, (DateTime.Now - StartTime).Milliseconds));
            } catch (Exception ex) {
                HtcPlugin.Logger.LogError(ex);
            }
        }

        private void ProcessOnOutputDataReceived(object sender, DataReceivedEventArgs e) {
            try {
                if (!string.IsNullOrEmpty(e.Data)) Output.Add(new OutputLine(e.Data, (DateTime.Now - StartTime).Milliseconds));
            } catch (Exception ex) {
                HtcPlugin.Logger.LogError(ex);
            }
        }
    }
}
