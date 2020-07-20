using System.Threading.Tasks;
using HtcSharp.Core.Utils;
using HtcSharp.HttpModule.Http.Abstractions;
using HtcSharp.HttpModule.Routing;
using RemoteGitDeploy.Utils;

namespace RemoteGitDeploy.API.Get {
    public class RepositorySettings : IAPI {
        public string FileName => "/api/get/repository/settings";
        public string RequestMethod => HttpMethods.Post;
        public string RequestContentType => ContentType.JSON.ToValue();
        public bool NeedAuthentication => true;

        public async Task OnRequest(HttpContext httpContext) {
            var data = await new JsonData().Load(httpContext);
            if (data.TryGetValue("guid", out string guid)) {
                await httpContext.Response.WriteAsync(JsonUtils.SerializeObject(new {
                    success = true,
                    webhook = new {
                        payloadUrl = $"http://{HtcPlugin.Domain}/api/repository/pull?repository={guid}",
                        contentType = "application/json",
                        secrete = HtcPlugin.RepositorySecreteKey,
                    }
                }));
            } else await DefaultResponse.FieldsMissing(httpContext);
        }
    }
}