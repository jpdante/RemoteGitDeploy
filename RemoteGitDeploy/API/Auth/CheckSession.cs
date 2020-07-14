using System;
using System.Threading.Tasks;
using HtcSharp.Core.Utils;
using HtcSharp.HttpModule.Http.Abstractions;
using HtcSharp.HttpModule.Routing;
using RemoteGitDeploy.Security;
using RemoteGitDeploy.Utils;

namespace RemoteGitDeploy.API.Auth {
    public class CheckSession : IAPI {
        public string FileName => "/auth/checksession";
        public string RequestMethod => HttpMethods.Get;
        public string RequestContentType => null;
        public bool NeedAuthentication => true;

        public async Task OnRequest(HttpContext httpContext) {
            if (httpContext.Session.IsAvailable) {
                await httpContext.Response.WriteAsync("{\"invalidSession\":false}");
            } else {
                await DefaultResponse.InvalidSession(httpContext);
            }
        }
    }
}