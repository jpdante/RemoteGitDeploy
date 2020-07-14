using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using HtcSharp.HttpModule.Http.Abstractions;
using HtcSharp.HttpModule.Routing;
using RemoteGitDeploy.Utils;

namespace RemoteGitDeploy.API.Dev {
    public class IdGen : IAPI {
        public string FileName => "/dev/idgen";
        public string RequestMethod => HttpMethods.Get;
        public string RequestContentType => null;
        public bool NeedAuthentication => false;

        public async Task OnRequest(HttpContext httpContext) {
            if (httpContext.Request.Query.TryGetValue("key", out var values)) {
                if (values[0].Equals(HtcPlugin.DevKey)) {
                    httpContext.Response.StatusCode = 200;
                    httpContext.Response.ContentType = ContentType.HTML.ToValue();
                    string data = $"<strong>ID Gen:</strong> {StaticData.IdGenerator.CreateId()}<br/>" +
                                  $"&nbsp;&nbsp;&nbsp;&nbsp;<strong>Guid:</strong> {Guid.NewGuid():N}";
                    await httpContext.Response.WriteAsync(data);
                } else {
                    httpContext.Response.StatusCode = 403;
                    await httpContext.Response.WriteAsync("Invalid devkey");
                }
            } else {
                httpContext.Response.StatusCode = 403;
                await httpContext.Response.WriteAsync("No devkey");
            }
        }
    }
}