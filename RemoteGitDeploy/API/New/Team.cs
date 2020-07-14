using System.Threading.Tasks;
using HtcSharp.Core.Utils;
using HtcSharp.HttpModule.Http.Abstractions;
using HtcSharp.HttpModule.Routing;
using RemoteGitDeploy.Utils;

namespace RemoteGitDeploy.API.New {
    public class Team : IAPI {
        public string FileName => "/api/new/team";
        public string RequestMethod => HttpMethods.Post;
        public string RequestContentType => ContentType.JSON.ToValue();
        public bool NeedAuthentication => true;

        public async Task OnRequest(HttpContext httpContext) {
            var data = await new JsonData().Load(httpContext);
            if (data.TryGetValue("name", out string name) &&
                data.TryGetValue("description", out string description)) {
                if (!DataValidation.ValidateTeamName(name, out string error)) {
                    await DefaultResponse.InvalidField(httpContext, "name", error);
                    return;
                }
                description ??= "";
                await using var conn = await HtcPlugin.DatabaseManager.GetConnectionAsync();
                if (await HtcPlugin.DatabaseManager.HasTeamAsync(name, conn)) {
                    await DefaultResponse.InvalidField(httpContext, "name", "There is already a team with that name.");
                    return;
                }
                if (await HtcPlugin.DatabaseManager.NewTeamAsync(name, description, conn)) {
                    await DefaultResponse.Success(httpContext);
                } else {
                    httpContext.Response.StatusCode = StatusCodes.Status200OK;
                    await httpContext.Response.WriteAsync(JsonUtils.SerializeObject(new { success = false, message = "Failed to create team." }));
                }
            } else await DefaultResponse.FieldsMissing(httpContext);
        }
    }
}