using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using HtcSharp.Core.Logging.Abstractions;
using Microsoft.EntityFrameworkCore;
using RemoteGitDeploy.Actions;
using RemoteGitDeploy.Models.Internal;
using RemoteGitDeploy.Models.New;

namespace RemoteGitDeploy.Manager {
    public class RepositoryManager {

        public bool Started { get; private set; }
        public static string RepositoriesPath { get; private set; }
        public static string TempRepositoryPath { get; private set; }

        private readonly List<Repository> _repositoriesInfos;
        private readonly Dictionary<string, InternalRepository> _repositories;

        public RepositoryManager(string repositoriesDirectory) {
            RepositoriesPath = repositoriesDirectory;
            TempRepositoryPath = Path.Combine(RepositoryManager.RepositoriesPath, "temp");
            if (!Directory.Exists(RepositoriesPath)) Directory.CreateDirectory(RepositoriesPath);
            if (!Directory.Exists(TempRepositoryPath)) Directory.CreateDirectory(TempRepositoryPath);
            _repositories = new Dictionary<string, InternalRepository>();
            _repositoriesInfos = new List<Repository>();
        }

        public async Task Start() {
            if (Started) return;
            Started = true;
            _repositories.Clear();
            _repositoriesInfos.Clear();
            await using var context = new RgdContext();
            Repository[] repositories = await (from t in context.Repositories select t).ToArrayAsync();
            foreach (var repository in repositories) {
                HtcPlugin.Logger.LogInfo($"Loading repository: {repository.Guid}");
                var internalRepository = new InternalRepository(repository);
                try {
                    internalRepository.LastCommit = await GetLocalRepositoryLastCommit(internalRepository);
                    internalRepository.LastUpdate = DateTime.UtcNow;
                } catch {
                    // ignored
                }
                _repositories.Add(repository.Guid, internalRepository);
                _repositoriesInfos.Add(repository);
            }
            HtcPlugin.Logger.LogInfo("Repositories loaded.");
        }

        public Task Stop() {
            if (!Started) return Task.CompletedTask;
            return Task.CompletedTask;
        }

        public Repository[] GetRepositories() => _repositoriesInfos.ToArray();
        public InternalRepository[] GetInternalRepositories() => _repositories.Values.ToArray();

        public bool TryGetRepository(string guid, out Repository repository) {
            bool result = _repositories.TryGetValue(guid, out var internalRepository);
            repository = internalRepository?.RepositoryInfo;
           return result;
        }

        public bool TryGetInternalRepository(string guid, out InternalRepository internalRepository) => _repositories.TryGetValue(guid, out internalRepository);

        public async Task<bool> CreateRepository(Repository repository) {
            try {
                await using var context = new RgdContext();
                await context.Repositories.AddAsync(repository);
                await context.SaveChangesAsync();

                _repositories.Add(repository.Guid, new InternalRepository(repository));
                _repositoriesInfos.Add(repository);

                await _repositories[repository.Guid].AddAction(new CloneRepositoryAction());
                return true;
            } catch {
                return false;
            }
        }

        public async Task<string> GetLocalRepositoryLastCommit(InternalRepository internalRepository) {
            try {
                var processStartInfo = new ProcessStartInfo {
                    FileName = "git",
                    Arguments = $"rev-parse {internalRepository.RepositoryInfo.Branch}",
                    WorkingDirectory = Path.Combine(RepositoriesPath, internalRepository.RepositoryInfo.Guid),
                    RedirectStandardError = true,
                    RedirectStandardOutput = true,
                };
                var process = new Process {
                    StartInfo = processStartInfo,
                    EnableRaisingEvents = true,
                };
                process.Start();
                string data = await process.StandardOutput.ReadToEndAsync();
                return data.Length >= 40 ? data.Substring(0, 40) : data;
            } catch (Exception ex) {
                HtcPlugin.Logger.LogError(ex);
                return null;
            }
        }

        /*public async Task<string> GetRemoteRepositoryLastCommit(InternalRepository internalRepository) {
            try {
                var processStartInfo = new ProcessStartInfo {
                    FileName = "git",
                    Arguments = $"ls-remote {internalRepository.RepositoryInfo.Git} {internalRepository.RepositoryInfo.Branch}",
                    WorkingDirectory = Path.Combine(RepositoriesPath, internalRepository.RepositoryInfo.Guid),
                    RedirectStandardError = true,
                    RedirectStandardOutput = true,
                };
                var process = new Process {
                    StartInfo = processStartInfo,
                    EnableRaisingEvents = true,
                };
                process.Start();
                string data = await process.StandardOutput.ReadToEndAsync();
                return data.Length >= 40 ? data.Substring(0, 40) : data;
            } catch (Exception ex) {
                HtcPlugin.Logger.LogError(ex);
                return null;
            }
        }*/

        public static bool BreakGitLink(string link, out string scheme, out string domain, out string path) {
            try {
                if (!Uri.TryCreate(link, UriKind.RelativeOrAbsolute, out var uri)) {
                    scheme = null;
                    domain = null;
                    path = null;
                    return false;
                }
                scheme = uri.Scheme;
                domain = uri.Host;
                path = uri.AbsolutePath;
                return true;
            } catch {
                scheme = null;
                domain = null;
                path = null;
                return false;
            }
        }

        public async Task PullApi(InternalRepository internalRepository) {
            await internalRepository.AddAction(new PullRepositoryAction());
            await internalRepository.AddAction(new ExecActionRepositoryAction());
        }
    }
}