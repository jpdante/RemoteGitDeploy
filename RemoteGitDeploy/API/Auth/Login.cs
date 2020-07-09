using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using HtcSharp.HttpModule.Http.Abstractions;
using HtcSharp.HttpModule.Routing;
using RemoteGitDeploy.Utils;

namespace RemoteGitDeploy.API.Auth {
    public class Login : IAPI {
        public string FileName => "/login";
        public string RequestMethod => HttpMethods.Get;
        public string RequestContentType => ContentType.JSON.ToValue();
        public bool NeedAuthentication => false;

        public async Task OnRequest(HttpContext httpContext) {
            var data = await new JsonData().Load(httpContext);
            if (data.TryGetValue("email", out string email) && data.TryGetValue("password", out string password)) {
                if (!DataValidation.ValidateEmail(email)) {
                    await DefaultResponse.InvalidField(httpContext, "email");
                    return;
                }
                if (!DataValidation.ValidatePassword(password)) {
                    await DefaultResponse.InvalidField(httpContext, "password");
                    return;
                }
                await using var connection = await HtcPlugin.DatabaseManager.GetConnectionAsync();
            } else await DefaultResponse.FieldsMissing(httpContext);
        }
    }
}
