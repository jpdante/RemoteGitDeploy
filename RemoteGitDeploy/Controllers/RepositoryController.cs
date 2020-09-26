using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using HtcSharp.Core.Logging.Abstractions;
using HtcSharp.HttpModule.Http.Abstractions;
using HtcSharp.HttpModule.Http.Abstractions.Extensions;
using HtcSharp.HttpModule.Routing;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using RemoteGitDeploy.Actions;
using RemoteGitDeploy.Core;
using RemoteGitDeploy.Extensions;
using RemoteGitDeploy.Manager;
using RemoteGitDeploy.Models.New;
using RemoteGitDeploy.Models.RequestData;
using RemoteGitDeploy.Models.Views;
using RemoteGitDeploy.Mvc;
using RemoteGitDeploy.Utils;

namespace RemoteGitDeploy.Controllers {
    public class RepositoryController {

        [HttpPost("/api/repository/branchs/get", ContentType.JSON, true)]
        public static async Task GetBranch(HttpContext httpContext, RepositoryGitData repositoryGitData) {
            await using var context = new RgdContext();

            if (!httpContext.Session.GetAccountId(out long accountId)) throw new Exception("Failed to get accountId");
            var creatorPermissions = await (from a in context.Accounts where a.Id.Equals(accountId) select a.Permissions).FirstOrDefaultAsync();
            if ((creatorPermissions & Permission.ManageRepository) != Permission.ManageRepository) throw new HttpException(403, "No ManageRepository permission.");

            if (!RepositoryManager.BreakGitLink(repositoryGitData.Git, out string scheme, out string domain, out string path)) {
                await httpContext.Response.SendDecodeErrorAsync();
                return;
            }

            var processStartInfo = new ProcessStartInfo {
                FileName = "git",
                Arguments = $"ls-remote --heads {scheme}://{repositoryGitData.Username}:{repositoryGitData.Token}@{domain}{path}",
                WorkingDirectory = Path.Combine(RepositoryManager.RepositoriesPath, "temp"),
                RedirectStandardOutput = true,
                RedirectStandardInput = true,
            };
            var process = new Process {
                StartInfo = processStartInfo,
                EnableRaisingEvents = true,
            };
            var branchList = new List<string>();
            process.Start();
            process.BeginOutputReadLine();
            process.OutputDataReceived += (sender, args) => {
                if (string.IsNullOrEmpty(args.Data)) return;
                branchList.Add(args.Data.Split("/").Last());
            };
            process.WaitForExit(2000);
            try {
                process.Kill(true);
            } catch {
                // ignored
            }
            if (process.ExitCode == 0) {
                httpContext.Response.StatusCode = 200;
                await httpContext.Response.WriteAsync(JsonConvert.SerializeObject(new { success = true, branchs = branchList }));
            } else {
                await httpContext.Response.SendErrorAsync(process.ExitCode, $"Status code: {process.ExitCode}");
            }
        }

        [HttpPost("/api/repository/new", ContentType.JSON, true)]
        public static async Task NewRepository(HttpContext httpContext, NewRepositoryData newRepositoryData) {
            await using var context = new RgdContext();

            if (!httpContext.Session.GetAccountId(out long accountId)) throw new Exception("Failed to get accountId");
            var creatorPermissions = await (from a in context.Accounts where a.Id.Equals(accountId) select a.Permissions).FirstOrDefaultAsync();
            if ((creatorPermissions & Permission.WriteRepository) != Permission.WriteRepository) throw new HttpException(403, "No WriteRepository permission.");

            var team = await (from t in context.Teams where t.Name.Equals(newRepositoryData.Team) select t).FirstOrDefaultAsync();

            var repository = new Repository(accountId, newRepositoryData.Username, newRepositoryData.Token, newRepositoryData.Name, newRepositoryData.Git, newRepositoryData.Branch, team.Id, newRepositoryData.Description);

            if (await HtcPlugin.RepositoryManager.CreateRepository(repository)) {
                await httpContext.Response.WriteAsync(JsonConvert.SerializeObject(new { success = true, guid = repository.Guid }));
            } else {
                await httpContext.Response.SendErrorAsync(1, "An unknown failure occurred while creating the repository, make sure that all data is correct.");
            }
        }

        [HttpPost("/api/repository/get", ContentType.JSON, true)]
        public static async Task GetRepository(HttpContext httpContext, RepositoryData repositoryData) {
            await using var context = new RgdContext();

            if (!httpContext.Session.GetAccountId(out long accountId)) throw new Exception("Failed to get accountId");
            var creatorPermissions = await (from a in context.Accounts where a.Id.Equals(accountId) select a.Permissions).FirstOrDefaultAsync();
            if ((creatorPermissions & Permission.ReadRepository) != Permission.ReadRepository) throw new HttpException(403, "No ReadRepository permission.");

            if (HtcPlugin.RepositoryManager.TryGetRepository(repositoryData.Guid, out var repository)) {
                var team = await (from t in context.Teams where t.Id.Equals(repository.TeamId) select t).FirstOrDefaultAsync();

                await httpContext.Response.WriteAsync(JsonConvert.SerializeObject(new { success = true, repository = new RepositoryView(repository, team != null ? new TeamView(team) : null) }));
            } else {
                await httpContext.Response.SendErrorAsync(404, "Unknown repository.");
            }
        }

        [HttpGet("/api/repositories/get", true)]
        public static async Task GetRepositories(HttpContext httpContext) {
            await using var context = new RgdContext();

            if (!httpContext.Session.GetAccountId(out long accountId)) throw new Exception("Failed to get accountId");
            var creatorPermissions = await (from a in context.Accounts where a.Id.Equals(accountId) select a.Permissions).FirstOrDefaultAsync();
            if ((creatorPermissions & Permission.ManageRepository) != Permission.ManageRepository) throw new HttpException(403, "No ManageRepository permission.");

            Repository[] repositoriesRaw = HtcPlugin.RepositoryManager.GetRepositories();
            var repositories = new List<RepositoryView>();
            foreach (var repository in repositoriesRaw) {
                var team = await (from t in context.Teams where t.Id.Equals(repository.TeamId) select t).FirstOrDefaultAsync();
                repositories.Add(new RepositoryView(repository, team != null ? new TeamView(team) : null));
            }

            await httpContext.Response.WriteAsync(JsonConvert.SerializeObject(new { success = true, repositories }));
        }

        [HttpPost("/api/repository/settings/get", ContentType.JSON, true)]
        public static async Task GetRepositorySettings(HttpContext httpContext, RepositoryData repositoryData) {
            await using var context = new RgdContext();

            if (!httpContext.Session.GetAccountId(out long accountId)) throw new Exception("Failed to get accountId");
            var creatorPermissions = await (from a in context.Accounts where a.Id.Equals(accountId) select a.Permissions).FirstOrDefaultAsync();
            if ((creatorPermissions & Permission.ManageRepository) != Permission.ManageRepository) throw new HttpException(403, "No ManageRepository permission.");

            if (HtcPlugin.RepositoryManager.TryGetRepository(repositoryData.Guid, out var repository)) {
                await httpContext.Response.WriteAsync(JsonConvert.SerializeObject(new {
                    success = true,
                    webhook = new {
                        payloadUrl = $"{httpContext.Request.Scheme}://{HtcPlugin.Config.Other.Domain}/api/repository/pull?repository={repository.Guid}",
                        contentType = "application/json",
                        secrete = HtcPlugin.Config.Other.SecretKey,
                    }
                }));
            } else {
                await httpContext.Response.SendErrorAsync(404, "Unknown repository.");
            }
        }

        [HttpPost("/api/repository/status/get", ContentType.JSON, true)]
        public static async Task GetRepositoryStatus(HttpContext httpContext, RepositoryData repositoryData) {
            await using var context = new RgdContext();

            if (!httpContext.Session.GetAccountId(out long accountId)) throw new Exception("Failed to get accountId");
            var creatorPermissions = await (from a in context.Accounts where a.Id.Equals(accountId) select a.Permissions).FirstOrDefaultAsync();
            if ((creatorPermissions & Permission.ManageRepository) != Permission.ManageRepository) throw new HttpException(403, "No ManageRepository permission.");

            if (HtcPlugin.RepositoryManager.TryGetInternalRepository(repositoryData.Guid, out var internalRepository)) {
                await httpContext.Response.WriteAsync(JsonConvert.SerializeObject(new {
                    success = true,
                    status = new {
                        lastCommit = internalRepository.LastCommit,
                        lastUpdate = internalRepository.LastUpdate.ToString(CultureInfo.InvariantCulture),
                    }
                }));
            } else {
                await httpContext.Response.SendErrorAsync(404, "Unknown repository.");
            }
        }

        [HttpPost("/api/repository/force/pull", ContentType.JSON, true)]
        public static async Task GetRepositoryForcePull(HttpContext httpContext, RepositoryData repositoryData) {
            await using var context = new RgdContext();

            if (!httpContext.Session.GetAccountId(out long accountId)) throw new Exception("Failed to get accountId");
            var creatorPermissions = await (from a in context.Accounts where a.Id.Equals(accountId) select a.Permissions).FirstOrDefaultAsync();
            if ((creatorPermissions & Permission.ManageRepository) != Permission.ManageRepository) throw new HttpException(403, "No ManageRepository permission.");

            if (HtcPlugin.RepositoryManager.TryGetInternalRepository(repositoryData.Guid, out var internalRepository)) {
                await internalRepository.AddAction(new PullRepositoryAction());
                await httpContext.Response.WriteAsync(JsonConvert.SerializeObject(new {
                    success = true
                }));
            } else {
                await httpContext.Response.SendErrorAsync(404, "Unknown repository.");
            }
        }


        [HttpPost("/api/repository/force/reclone", ContentType.JSON, true)]
        public static async Task GetRepositoryForceReClone(HttpContext httpContext, RepositoryData repositoryData) {
            await using var context = new RgdContext();

            if (!httpContext.Session.GetAccountId(out long accountId)) throw new Exception("Failed to get accountId");
            var creatorPermissions = await (from a in context.Accounts where a.Id.Equals(accountId) select a.Permissions).FirstOrDefaultAsync();
            if ((creatorPermissions & Permission.ManageRepository) != Permission.ManageRepository) throw new HttpException(403, "No ManageRepository permission.");

            if (HtcPlugin.RepositoryManager.TryGetInternalRepository(repositoryData.Guid, out var internalRepository)) {
                await internalRepository.AddAction(new ReCloneRepositoryAction());
                await httpContext.Response.WriteAsync(JsonConvert.SerializeObject(new {
                    success = true
                }));
            } else {
                await httpContext.Response.SendErrorAsync(404, "Unknown repository.");
            }
        }

        [HttpPost("/api/repository/force/action", ContentType.JSON, true)]
        public static async Task GetRepositoryForceAction(HttpContext httpContext, RepositoryData repositoryData) {
            await using var context = new RgdContext();

            if (!httpContext.Session.GetAccountId(out long accountId)) throw new Exception("Failed to get accountId");
            var creatorPermissions = await (from a in context.Accounts where a.Id.Equals(accountId) select a.Permissions).FirstOrDefaultAsync();
            if ((creatorPermissions & Permission.ManageRepository) != Permission.ManageRepository) throw new HttpException(403, "No ManageRepository permission.");

            if (HtcPlugin.RepositoryManager.TryGetInternalRepository(repositoryData.Guid, out var internalRepository)) {
                await internalRepository.AddAction(new ExecActionRepositoryAction());
                await httpContext.Response.WriteAsync(JsonConvert.SerializeObject(new {
                    success = true
                }));
            } else {
                await httpContext.Response.SendErrorAsync(404, "Unknown repository.");
            }
        }

        [HttpPost("/api/repository/force/cancel", ContentType.JSON, true)]
        public static async Task GetRepositoryForceCancel(HttpContext httpContext, RepositoryData repositoryData) {
            await using var context = new RgdContext();

            if (!httpContext.Session.GetAccountId(out long accountId)) throw new Exception("Failed to get accountId");
            var creatorPermissions = await (from a in context.Accounts where a.Id.Equals(accountId) select a.Permissions).FirstOrDefaultAsync();
            if ((creatorPermissions & Permission.ManageRepository) != Permission.ManageRepository) throw new HttpException(403, "No ManageRepository permission.");

            if (HtcPlugin.RepositoryManager.TryGetInternalRepository(repositoryData.Guid, out var internalRepository)) {
                if(internalRepository.CurrentAction != null) {
                    await internalRepository.CurrentAction.Cancel();
                }
                await httpContext.Response.WriteAsync(JsonConvert.SerializeObject(new {
                    success = true
                }));
            } else {
                await httpContext.Response.SendErrorAsync(404, "Unknown repository.");
            }
        }

        [HttpPost("/api/repository/pull", ContentType.JSON, false)]
        public static async Task GetRepositoryPull(HttpContext httpContext) {
            if (httpContext.Request.Query.TryGetValue("repository", out var repository) &&
                httpContext.Request.Headers.TryGetValue("X-GitHub-Event", out var eventName) &&
                httpContext.Request.Headers.TryGetValue("X-Hub-Signature", out var signature) &&
                httpContext.Request.Headers.TryGetValue("X-GitHub-Delivery", out var delivery)) {
                using var reader = new StreamReader(httpContext.Request.Body);
                string payload = await reader.ReadToEndAsync();
                if (await IsGithubPushAllowed(payload, eventName, signature, httpContext)) {
                    var response = "";
                    foreach (string repositoryGuid in repository.ToString().Split(";")) {
                        if (HtcPlugin.RepositoryManager.TryGetInternalRepository(repositoryGuid, out var internalRepository)) {
                            await HtcPlugin.RepositoryManager.PullApi(internalRepository);
                            response += $"[{repositoryGuid}] Ok{Environment.NewLine}";
                        } else {
                            response += $"[{repositoryGuid}] Repository not found{Environment.NewLine}";
                        }
                    }
                    await httpContext.Response.WriteAsync(response);
                }
            } else await httpContext.Response.SendErrorAsync(400, "Fields missing.");
        }

        private const string Sha1Prefix = "sha1=";

        private static async Task<bool> IsGithubPushAllowed(string payload, string eventName, string signatureWithPrefix, HttpContext httpContext) {
            if (string.IsNullOrWhiteSpace(payload)) {
                httpContext.Response.StatusCode = 500;
                await httpContext.Response.WriteAsync("Invalid payload/body");
                return false;
            }
            if (string.IsNullOrWhiteSpace(eventName)) {
                httpContext.Response.StatusCode = 500;
                await httpContext.Response.WriteAsync("Invalid event");
                return false;
            }
            if (string.IsNullOrWhiteSpace(signatureWithPrefix)) {
                httpContext.Response.StatusCode = 403;
                await httpContext.Response.WriteAsync("Invalid signature");
                return false;
            }
            if (!signatureWithPrefix.StartsWith(Sha1Prefix, StringComparison.OrdinalIgnoreCase)) {
                httpContext.Response.StatusCode = 500;
                await httpContext.Response.WriteAsync("Invalid SHA-1");
                return false;
            }
            string signature = signatureWithPrefix.Substring(Sha1Prefix.Length);
            byte[] secret = Encoding.UTF8.GetBytes(HtcPlugin.Config.Other.SecretKey);
            byte[] payloadBytes = Encoding.UTF8.GetBytes(payload);
            using var hmSha1 = new HMACSHA1(secret);
            byte[] hash = hmSha1.ComputeHash(payloadBytes);
            string hashString = ToHexString(hash);
            if (hashString.Equals(signature)) return true;
            httpContext.Response.StatusCode = 403;
            await httpContext.Response.WriteAsync("Forbidden");
            return false;
        }

        private static string ToHexString(IReadOnlyCollection<byte> bytes) {
            var builder = new StringBuilder(bytes.Count * 2);
            foreach (byte b in bytes) {
                builder.AppendFormat("{0:x2}", b);
            }
            return builder.ToString();
        }
    }
}
