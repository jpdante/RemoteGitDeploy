using System.Collections.Generic;
using System.Threading.Tasks;
using HtcSharp.Core.Utils;
using HtcSharp.HttpModule.Http.Abstractions;
using HtcSharp.HttpModule.Routing;
using RemoteGitDeploy.Model.Database;
using RemoteGitDeploy.Utils;

namespace RemoteGitDeploy.API.Delete {
    public class Repository : IAPI {
        public string FileName => "/api/delete/repository";
        public string RequestMethod => HttpMethods.Post;
        public string RequestContentType => ContentType.JSON.ToValue();
        public bool NeedAuthentication => true;

        public async Task OnRequest(HttpContext httpContext) {
            var data = await new JsonData().Load(httpContext);
            if (data.TryGetValue("guid", out string guid)) {
                var repository = HtcPlugin.RepositoryManager.GetRepository(guid);
                if (repository == null) {
                    await DefaultResponse.Failed(httpContext, "This repository does not exist.");
                }
                HtcPlugin.RepositoryManager.DeleteRepository(repository);
                await DefaultResponse.Success(httpContext);
            } else await DefaultResponse.FieldsMissing(httpContext);
        }
    }
}