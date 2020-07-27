using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using HtcSharp.Core.Utils;
using HtcSharp.HttpModule.Http.Abstractions;
using HtcSharp.HttpModule.Routing;
using RemoteGitDeploy.Manager;
using RemoteGitDeploy.Utils;

namespace RemoteGitDeploy.API.Get {
    public class RepositoryBranchList : IAPI {
        public string FileName => "/api/get/repositorybranchlist";
        public string RequestMethod => HttpMethods.Post;
        public string RequestContentType => ContentType.JSON.ToValue();
        public bool NeedAuthentication => true;

        public async Task OnRequest(HttpContext httpContext) {
            var data = await new JsonData().Load(httpContext);
            if (data.TryGetValue("git", out string git)) {
                if (!RepositoryManager.BreakGitLink(git, out string scheme, out string domain, out string path)) {
                    await DefaultResponse.FailedParsingData(httpContext);
                    return;
                }
                var processStartInfo = new ProcessStartInfo {
                    FileName = "git",
                    Arguments = $"ls-remote --heads {scheme}://{HtcPlugin.RepositoryManager.GitUsername}:{HtcPlugin.RepositoryManager.GitPersonalAccessToken}@{domain}{path}",
                    WorkingDirectory = HtcPlugin.RepositoryManager.GitRepositoriesDirectory,
                    RedirectStandardOutput = true,
                };
                var process = new Process {
                    StartInfo = processStartInfo,
                    EnableRaisingEvents = true,
                };
                process.Start();
                process.WaitForExit();
                if (process.ExitCode == 0) {
                    string text = await process.StandardOutput.ReadToEndAsync();
                    string[] lines = text.Split("\n");
                    List<string> branchList = lines.Select(line => line.Split("/")).Select(lineSplit => lineSplit.Last()).Where(name => !string.IsNullOrWhiteSpace(name)).ToList();
                    httpContext.Response.StatusCode = 200;
                    await httpContext.Response.WriteAsync(JsonUtils.SerializeObject(new { success = true, branchs = branchList }));
                } else {
                    await DefaultResponse.Failed(httpContext, $"Status code: {process.ExitCode}");
                }
            } else await DefaultResponse.FieldsMissing(httpContext);
        }
    }
}
