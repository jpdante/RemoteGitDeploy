using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using HtcSharp.Core.Logging.Abstractions;
using MoonSharp.Interpreter;
using MoonSharp.Interpreter.Loaders;
using Newtonsoft.Json;
using RemoteGitDeploy.Manager;
using RemoteGitDeploy.Models;
using RemoteGitDeploy.Models.Internal;
using RemoteGitDeploy.Models.New;
using RemoteGitDeploy.Models.Views;

namespace RemoteGitDeploy.Actions {
    public class ExecActionRepositoryAction : IRepositoryAction {

        public string ActionGuid { get; }
        public bool Running { get; private set; }
        public bool Success { get; set; }
        public bool InQueue { get; set; }
        public bool Finished { get; set; }

        public DateTime StartTime { get; private set; }
        public DateTime ExitTime { get; private set; }
        public List<OutputLine> Output { get; }

        private Script _luaScript;
        private int _exitCode;
        private Process _process;
        private string _directory;
        private string _filename;
        private InternalRepository _internalRepository;

        public ExecActionRepositoryAction() {
            ActionGuid = Guid.NewGuid().ToString();
            Output = new List<OutputLine>();
        }

        public async Task<bool> Start(InternalRepository internalRepository) {
            if (Finished) return false;
            _internalRepository = internalRepository;
            try {
                _directory = Path.Combine(RepositoryManager.RepositoriesPath, internalRepository.RepositoryInfo.Guid);
                _filename = Path.Combine(_directory, ".rgd", "run.lua");
                if (!File.Exists(_filename)) {
                    Running = false;
                    Success = false;
                    ExitTime = DateTime.UtcNow;
                    OnFinish?.Invoke(this);
                    return true;
                }
                _luaScript = new Script();
                string luaIncludePath = Path.GetDirectoryName(_filename)?.Replace(@"\", "/");
                ((ScriptLoaderBase)_luaScript.Options.ScriptLoader).ModulePaths = new[] { $"?.lua", $"{luaIncludePath}/?", $"{luaIncludePath}/?.lua", };
                _luaScript.Options.DebugPrint = data => { Output.Add(new OutputLine(data, (DateTime.UtcNow - StartTime).Milliseconds)); };
                Func<string, string, string, string, int> run = (fileName, arguments, workingDirectory, environmentVariables) => {
                    if (Finished) return -1;
                    var id = Guid.NewGuid().ToString();
                    var processStartInfo = new ProcessStartInfo {
                        FileName = fileName,
                        Arguments = string.IsNullOrEmpty(arguments) ? "" : arguments,
                        WorkingDirectory = string.IsNullOrEmpty(workingDirectory) ? _directory : workingDirectory,
                        RedirectStandardError = true,
                        RedirectStandardOutput = true,
                        RedirectStandardInput = true,
                    };
                    if (!string.IsNullOrEmpty(environmentVariables)) {
                        foreach (string variable in environmentVariables.Split(";")) {
                            string[] varData = variable.Split("=", 2);
                            if (varData.Length == 2) processStartInfo.EnvironmentVariables.Add(varData[0], varData[1]);
                        }
                    }
                    Output.Add(new OutputLine($"[{id}] Starting process: \"{fileName}{(string.IsNullOrEmpty(arguments) ? "" : " " + arguments)}\"", (DateTime.UtcNow - StartTime).Milliseconds));
                    _process = new Process { StartInfo = processStartInfo, };
                    _process.OutputDataReceived += ProcessOnOutputDataReceived;
                    _process.ErrorDataReceived += ProcessOnErrorDataReceived;
                    _process.Start();
                    _process.BeginOutputReadLine();
                    _process.BeginErrorReadLine();
                    _process.WaitForExit();
                    Output.Add(new OutputLine($"[{id}] Process ended with code: {_process.ExitCode}", (DateTime.UtcNow - StartTime).Milliseconds));
                    return _process.ExitCode;
                };
                Action<int> setStatus = (code) => {
                    _exitCode = code;
                    Output.Add(new OutputLine($"Script status set with code: {code}", (DateTime.UtcNow - StartTime).Milliseconds));
                };
                _luaScript.Globals["run"] = run;
                _luaScript.Globals["setStatus"] = setStatus;
                _luaScript.Globals["repoBranch"] = internalRepository.RepositoryInfo.Branch;
                _luaScript.Globals["repoDirectory"] = _directory;
                _luaScript.Globals["repoScriptFilename"] = _filename;
                _luaScript.Globals["repoGit"] = internalRepository.RepositoryInfo.Git;
                _luaScript.Globals["repoPersonalAccessToken"] = internalRepository.RepositoryInfo.PersonalAccessToken;
                _luaScript.Globals["repoUsername"] = internalRepository.RepositoryInfo.Username;
                _luaScript.Globals["repoGuid"] = internalRepository.RepositoryInfo.Guid;
                _ = Task.Run(async () => {
                    try {
                        StartTime = DateTime.UtcNow;
                        Running = true;
                        _luaScript.DoFile(_filename);
                        Running = false;
                        if (_exitCode == 0) Success = true;
                        ExitTime = DateTime.UtcNow;
                        await SaveActionToHistory();
                        OnFinish?.Invoke(this);
                    } catch (ScriptRuntimeException ex) {
                        Running = false;
                        Success = false;
                        ExitTime = DateTime.UtcNow;
                        Output.Add(new OutputLine(ex.DecoratedMessage, (DateTime.UtcNow - StartTime).Milliseconds));
                        HtcPlugin.Logger.LogError(ex);
                        await SaveActionToHistory();
                        OnFinish?.Invoke(this);
                    } catch (Exception ex) {
                        Running = false;
                        Success = false;
                        ExitTime = DateTime.UtcNow;
                        Output.Add(new OutputLine(ex.Message, (DateTime.UtcNow - StartTime).Milliseconds));
                        HtcPlugin.Logger.LogError(ex);
                        await SaveActionToHistory();
                        OnFinish?.Invoke(this);
                    }
                });
                return true;
            } catch (Exception ex) {
                Running = false;
                Success = false;
                ExitTime = DateTime.UtcNow;
                Output.Add(new OutputLine(ex.Message, (DateTime.UtcNow - StartTime).Milliseconds));
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
                Name = "Execute repository action",
                Status = -1
            };
        }

        public event IRepositoryAction.OnFinishDelegate OnFinish;

        private async Task SaveActionToHistory() {
            await using var context = new RgdContext();
            await context.ActionHistory.AddAsync(
                new ActionHistory(
                    ActionGuid,
                    _internalRepository.RepositoryInfo.Id,
                    3,
                    "Execute repository lua script",
                    null,
                    JsonConvert.SerializeObject(Output),
                    _exitCode,
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
