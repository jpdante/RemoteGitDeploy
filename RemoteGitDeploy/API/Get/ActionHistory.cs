using System.Threading.Tasks;
using HtcSharp.Core.Utils;
using HtcSharp.HttpModule.Http.Abstractions;
using HtcSharp.HttpModule.Routing;
using RemoteGitDeploy.Utils;

namespace RemoteGitDeploy.API.Get {
    public class ActionHistory : IAPI {
        public string FileName => "/api/get/action/history";
        public string RequestMethod => HttpMethods.Post;
        public string RequestContentType => ContentType.JSON.ToValue();
        public bool NeedAuthentication => true;

        public async Task OnRequest(HttpContext httpContext) {
            var data = await new JsonData().Load(httpContext);
            if (data.TryGetValue("id", out string id)) {
                if (!long.TryParse(id, out long repository)) {
                    await DefaultResponse.FailedParsingData(httpContext);
                }
                await using var conn = await HtcPlugin.DatabaseManager.GetConnectionAsync();
                Model.Database.ActionHistory[] history = await HtcPlugin.DatabaseManager.GetActionsHistoryAsync(repository, conn);
                if (history == null) {
                    await DefaultResponse.Failed(httpContext);
                    return;
                }
                await httpContext.Response.WriteAsync(JsonUtils.SerializeObject(new { success = true, history }));
            } else await DefaultResponse.FieldsMissing(httpContext);
        }
    }
}