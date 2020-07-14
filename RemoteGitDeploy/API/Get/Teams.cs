using System.Threading.Tasks;
using HtcSharp.Core.Utils;
using HtcSharp.HttpModule.Http.Abstractions;

namespace RemoteGitDeploy.API.Get {
    public class Teams : IAPI {
        public string FileName => "/api/get/teams";
        public string RequestMethod => HttpMethods.Get;
        public string RequestContentType => null;
        public bool NeedAuthentication => true;

        public async Task OnRequest(HttpContext httpContext) {
            await using var conn = await HtcPlugin.DatabaseManager.GetConnectionAsync();
            Model.Database.Team[] teams = await HtcPlugin.DatabaseManager.GetTeamsAsync(conn);
            await httpContext.Response.WriteAsync(JsonUtils.SerializeObject(new { success = true, teams }));
        }
    }
}