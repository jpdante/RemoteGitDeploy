using System.Threading.Tasks;
using HtcSharp.Core.Utils;
using HtcSharp.HttpModule.Http.Abstractions;
using HtcSharp.HttpModule.Routing;
using RemoteGitDeploy.Actions.Data.Repository;
using RemoteGitDeploy.Utils;

namespace RemoteGitDeploy.API.Status {
    public class NewRepository : IAPI {
        public string FileName => "/api/status/newrepository";
        public string RequestMethod => HttpMethods.Post;
        public string RequestContentType => ContentType.JSON.ToValue();
        public bool NeedAuthentication => true;

        public async Task OnRequest(HttpContext httpContext) {
            var data = await new JsonData().Load(httpContext);
            if (data.TryGetValue("guid", out string guid)) {
                var action = HtcPlugin.RepositoryManager.GetRepositoryAction(guid);
                if (action != null) {
                    httpContext.Response.StatusCode = StatusCodes.Status200OK;
                    await httpContext.Response.WriteAsync(JsonUtils.SerializeObject(new {
                        success = true,
                        status = new {
                            success = action.Success,
                            inQueue = action.InQueue,
                            finished = action.Finished,
                            log = action.Output,
                            guid = action.Data.Id
                        }
                    }));
                } else {
                    await DefaultResponse.Failed(httpContext);
                }
            } else await DefaultResponse.FieldsMissing(httpContext);
        }
    }
}