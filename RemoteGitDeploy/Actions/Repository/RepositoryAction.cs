using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using HtcSharp.Core.Logging.Abstractions;
using MoonSharp.Interpreter;
using MoonSharp.Interpreter.Loaders;
using RemoteGitDeploy.Actions.Data;
using RemoteGitDeploy.Models;

namespace RemoteGitDeploy.Actions.Repository {
    public class RepositoryAction : IRepositoryAction {

        private readonly Script _luaScript;
        private readonly string _filename;
        private readonly string _directory;
        private bool _statusSet;
        private Process _process;

        public string RepositoryGuid { get; }
        public string RepositoryBranch { get; }
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

        public RepositoryAction(string repositoryGuid, string repositoryBranch, IActionData data, string fileName, string directory) {
            RepositoryGuid = repositoryGuid;
            RepositoryBranch = repositoryBranch;
            Data = data;
            _filename = fileName;
            _directory = directory;
            _luaScript = new Script();
            string luaIncludePath = Path.GetDirectoryName(fileName)?.Replace(@"\", "/");
            ((ScriptLoaderBase)_luaScript.Options.ScriptLoader).ModulePaths = new[] { $"?.lua", $"{luaIncludePath}/?", $"{luaIncludePath}/?.lua", };
            Running = false;
            Success = false;
            Output = new List<OutputLine>();
            DeleteDelay = 60;
            KillDelay = 300;
            InQueue = true;
        }

        public bool Start() {
            try {
                _luaScript.Options.DebugPrint = data => { Output.Add(new OutputLine(data, (DateTime.Now - StartTime).Milliseconds)); };
                Func<string, string, int> runProcess = (fileName, arguments) => {
                    var id = Guid.NewGuid().ToString();
                    var processStartInfo = new ProcessStartInfo {
                        FileName = fileName,
                        Arguments = arguments,
                        WorkingDirectory = _directory,
                        RedirectStandardError = true,
                        RedirectStandardOutput = true,
                    };
                    Output.Add(new OutputLine($"[{id}] Starting process: \"{fileName} {arguments}\"", (DateTime.Now - StartTime).Milliseconds));
                    _process = new Process { StartInfo = processStartInfo, };
                    _process.OutputDataReceived += ProcessOnOutputDataReceived;
                    _process.ErrorDataReceived += ProcessOnErrorDataReceived;
                    _process.Start();
                    _process.BeginOutputReadLine();
                    _process.BeginErrorReadLine();
                    _process.WaitForExit();
                    Output.Add(new OutputLine($"[{id}] Process ended with code: {_process.ExitCode}", (DateTime.Now - StartTime).Milliseconds));
                    return _process.ExitCode;
                };
                Func<string, string, string, int> runProcessWorkingDirectory = (fileName, arguments, workingDirectory) => {
                    var id = Guid.NewGuid().ToString();
                    var processStartInfo = new ProcessStartInfo {
                        FileName = fileName,
                        Arguments = arguments,
                        WorkingDirectory = workingDirectory,
                        RedirectStandardError = true,
                        RedirectStandardOutput = true,
                    };
                    Output.Add(new OutputLine($"[{id}] Starting process: \"{fileName} {arguments}\", Working on \"{workingDirectory}\"", (DateTime.Now - StartTime).Milliseconds));
                    _process = new Process { StartInfo = processStartInfo, };
                    _process.OutputDataReceived += ProcessOnOutputDataReceived;
                    _process.ErrorDataReceived += ProcessOnErrorDataReceived;
                    _process.Start();
                    _process.BeginOutputReadLine();
                    _process.BeginErrorReadLine();
                    _process.WaitForExit();
                    Output.Add(new OutputLine($"[{id}] Process ended with code: {_process.ExitCode}", (DateTime.Now - StartTime).Milliseconds));
                    return _process.ExitCode;
                };
                Action<int> setStatus = (code) => {
                    _statusSet = true;
                    Success = code == 0;
                    Output.Add(new OutputLine($"Script status set with code: {code}", (DateTime.Now - StartTime).Milliseconds));
                };
                _luaScript.Globals["run"] = runProcess;
                _luaScript.Globals["runWD"] = runProcessWorkingDirectory;
                _luaScript.Globals["setStatus"] = setStatus;
                _luaScript.Globals["branch"] = RepositoryBranch;
                Task.Run(() => {
                    _luaScript.DoFile(_filename);
                    Running = false;
                    if (!_statusSet) Success = true;
                    ExitTime = DateTime.Now;
                    OnFinish?.Invoke(this, Data);
                });
                StartTime = DateTime.Now;
                Running = true;
                return true;
            } catch (ScriptRuntimeException ex) {
                Running = false;
                Success = false;
                ExitTime = DateTime.Now;
                HtcPlugin.Logger.LogError(ex.DecoratedMessage, ex);
                OnFinish?.Invoke(this, Data);
                return false;
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
                if (_process != null && !_process.HasExited) _process.Kill(true);
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

        private void ProcessOnErrorDataReceived(object sender, DataReceivedEventArgs e) {
            try {
                Output.Add(new OutputLine(e.Data, (DateTime.Now - StartTime).Milliseconds));
            } catch (Exception ex) {
                HtcPlugin.Logger.LogError(ex);
            }
        }

        private void ProcessOnOutputDataReceived(object sender, DataReceivedEventArgs e) {
            try {
                Output.Add(new OutputLine(e.Data, (DateTime.Now - StartTime).Milliseconds));
            } catch (Exception ex) {
                HtcPlugin.Logger.LogError(ex);
            }
        }
    }
}