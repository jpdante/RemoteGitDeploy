using System;
using System.Threading.Tasks;
using HtcSharp.Core.Utils;
using HtcSharp.HttpModule.Http.Abstractions;
using HtcSharp.HttpModule.Routing;
using RemoteGitDeploy.Security;
using RemoteGitDeploy.Utils;

namespace RemoteGitDeploy.API.Auth {
    public class NewTeam : IAPI {
        public string FileName => "/auth/logout";
        public string RequestMethod => HttpMethods.Get;
        public string RequestContentType => null;
        public bool NeedAuthentication => true;

        public async Task OnRequest(HttpContext httpContext) {
            if (httpContext.Session.IsAvailable) {
                if (await HtcPlugin.CacheManager.DeleteSessionAsync(httpContext.Session.Id)) {
                    await DefaultResponse.Success(httpContext);
                } else {
                    await DefaultResponse.Failed(httpContext);
                }
            } else {
                await DefaultResponse.InvalidSession(httpContext);
            }
        }
    }
}