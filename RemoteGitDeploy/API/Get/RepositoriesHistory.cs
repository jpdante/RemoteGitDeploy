using System.Threading.Tasks;
using HtcSharp.Core.Utils;
using HtcSharp.HttpModule.Http.Abstractions;
using HtcSharp.HttpModule.Routing;
using RemoteGitDeploy.Utils;

namespace RemoteGitDeploy.API.Get {
    public class RepositoriesHistory : IAPI {
        public string FileName => "/api/get/repositories/history";
        public string RequestMethod => HttpMethods.Get;
        public string RequestContentType => null;
        public bool NeedAuthentication => true;

        public async Task OnRequest(HttpContext httpContext) {
            await using var conn = await HtcPlugin.DatabaseManager.GetConnectionAsync();
            Model.Database.RepositoryHistory[] history = await HtcPlugin.DatabaseManager.GetRepositoryHistoryAsync(conn);
            if (history == null) {
                await DefaultResponse.Failed(httpContext);
                return;
            }
            await httpContext.Response.WriteAsync(JsonUtils.SerializeObject(new { success = true, history }));
        }
    }
}