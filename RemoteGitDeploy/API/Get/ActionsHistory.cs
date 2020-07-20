using System.Threading.Tasks;
using HtcSharp.Core.Utils;
using HtcSharp.HttpModule.Http.Abstractions;
using RemoteGitDeploy.Utils;

namespace RemoteGitDeploy.API.Get {
    public class ActionsHistory : IAPI {
        public string FileName => "/api/get/actions/history";
        public string RequestMethod => HttpMethods.Get;
        public string RequestContentType => null;
        public bool NeedAuthentication => true;

        public async Task OnRequest(HttpContext httpContext) {
            await using var conn = await HtcPlugin.DatabaseManager.GetConnectionAsync();
            Model.Database.ActionHistory[] history = await HtcPlugin.DatabaseManager.GetActionsHistoryAsync(conn);
            if (history == null) {
                await DefaultResponse.Failed(httpContext);
                return;
            }
            await httpContext.Response.WriteAsync(JsonUtils.SerializeObject(new { success = true, history }));
        }
    }
}