using System.Threading.Tasks;
using HtcSharp.Core.Utils;
using HtcSharp.HttpModule.Http.Abstractions;
using HtcSharp.HttpModule.Routing;
using RemoteGitDeploy.Actions.Data.Repository;
using RemoteGitDeploy.Utils;

namespace RemoteGitDeploy.API.New {
    public class Repository : IAPI {
        public string FileName => "/api/new/repository";
        public string RequestMethod => HttpMethods.Post;
        public string RequestContentType => ContentType.JSON.ToValue();
        public bool NeedAuthentication => true;

        public async Task OnRequest(HttpContext httpContext) {
            var data = await new JsonData().Load(httpContext);
            if (data.TryGetValue("git", out string git) &&
                data.TryGetValue("name", out string name) &&
                data.TryGetValue("description", out string description) &&
                data.TryGetValue("team", out string team)) {
                if (!DataValidation.ValidateGitLink(git, out string error)) {
                    await DefaultResponse.InvalidField(httpContext, "git", error);
                    return;
                }
                if (!DataValidation.ValidateRepositoryName(name, out error)) {
                    await DefaultResponse.InvalidField(httpContext, "name", error);
                    return;
                }
                if (!DataValidation.ValidateRepositoryName(team, out error)) {
                    await DefaultResponse.InvalidField(httpContext, "team", error);
                    return;
                }
                description ??= "";
                await using var conn = await HtcPlugin.DatabaseManager.GetConnectionAsync();
                long teamId = await HtcPlugin.DatabaseManager.HasTeamWithResultAsync(team, conn);
                if (teamId == -1) {
                    await DefaultResponse.InvalidField(httpContext, "team", "This team does not exist or is not valid.");
                    return;
                }
                if (await HtcPlugin.DatabaseManager.HasRepositoryAsync(name, conn)) {
                    await DefaultResponse.InvalidField(httpContext, "name", "There is already a repository with that name.");
                    return;
                }
                if (HtcPlugin.RepositoryManager.HasProcessingRepository(name)) {
                    await DefaultResponse.InvalidField(httpContext, "name", "There is already a repository with that name currently being processed, please wait or use another name.");
                    return;
                }
                long account = await HtcPlugin.CacheManager.GetUserIdFromSessionAsync(httpContext.Session.Id);
                if (account == -1) {
                    await DefaultResponse.InvalidSession(httpContext);
                    return;
                }
                var repositoryCloneData = new RepositoryCloneData(account, git, name, description, teamId);
                HtcPlugin.RepositoryManager.CreateRepository(repositoryCloneData);
                httpContext.Response.StatusCode = StatusCodes.Status200OK;
                await httpContext.Response.WriteAsync(JsonUtils.SerializeObject(new { success = true, status = repositoryCloneData.Id }));
            } else await DefaultResponse.FieldsMissing(httpContext);
        }
    }
}