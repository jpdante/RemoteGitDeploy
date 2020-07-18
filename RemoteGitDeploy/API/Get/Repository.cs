using System.Collections.Generic;
using System.Threading.Tasks;
using HtcSharp.Core.Utils;
using HtcSharp.HttpModule.Http.Abstractions;
using HtcSharp.HttpModule.Routing;
using RemoteGitDeploy.Model.Database;
using RemoteGitDeploy.Utils;

namespace RemoteGitDeploy.API.Get {
    public class Repository : IAPI {
        public string FileName => "/api/get/repository";
        public string RequestMethod => HttpMethods.Post;
        public string RequestContentType => ContentType.JSON.ToValue();
        public bool NeedAuthentication => true;

        public async Task OnRequest(HttpContext httpContext) {
            var data = await new JsonData().Load(httpContext);
            if (data.TryGetValue("guid", out string guid)) {
                await using var conn = await HtcPlugin.DatabaseManager.GetConnectionAsync();
                var repository = await HtcPlugin.DatabaseManager.GetFullRepositoryAsync(guid, conn);
                if (repository == null) {
                    await DefaultResponse.Failed(httpContext);
                    return;
                }
                await httpContext.Response.WriteAsync(JsonUtils.SerializeObject(new { success = true, repository }));
            } else await DefaultResponse.FieldsMissing(httpContext);
        }
    }
}