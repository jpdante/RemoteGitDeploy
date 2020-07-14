using System;
using System.Threading.Tasks;
using HtcSharp.Core.Utils;
using HtcSharp.HttpModule.Http.Abstractions;
using HtcSharp.HttpModule.Routing;
using RemoteGitDeploy.Security;
using RemoteGitDeploy.Utils;

namespace RemoteGitDeploy.API.Auth {
    public class Login : IAPI {
        public string FileName => "/auth/login";
        public string RequestMethod => HttpMethods.Post;
        public string RequestContentType => ContentType.JSON.ToValue();
        public bool NeedAuthentication => false;

        public async Task OnRequest(HttpContext httpContext) {
            var data = await new JsonData().Load(httpContext);
            if (data.TryGetValue("email", out string email) &&
                data.TryGetValue("password", out string password) &&
                data.TryGetValue("rememberMe", out string rememberMeRaw)) {
                if (!DataValidation.ValidateEmail(email)) {
                    await DefaultResponse.InvalidField(httpContext, "email");
                    return;
                }
                if (!DataValidation.ValidatePassword(password, out string error)) {
                    await DefaultResponse.InvalidField(httpContext, "password", error);
                    return;
                }
                bool rememberMe = rememberMeRaw.Equals("true", StringComparison.CurrentCultureIgnoreCase);
                await using var conn = await HtcPlugin.DatabaseManager.GetConnectionAsync();
                var account = await HtcPlugin.DatabaseManager.GetAccountAsync(email, conn);
                if (account == null) {
                    httpContext.Response.StatusCode = StatusCodes.Status200OK;
                    await httpContext.Response.WriteAsync(JsonUtils.SerializeObject(new { success = false, message = "Email not registed." }));
                    return;
                }
                if (PasswordSecurity.CompareHashAsync(password, Convert.FromBase64String(account.Password), Convert.FromBase64String(account.Salt))) {
                    string sessionToken = SessionGenerator.Create();
                    if(rememberMe) await HtcPlugin.CacheManager.CreateSessionAsync(sessionToken, account.Id, TimeSpan.FromDays(30));
                    else await HtcPlugin.CacheManager.CreateSessionAsync(sessionToken, account.Id, TimeSpan.FromDays(1));
                    httpContext.Response.StatusCode = StatusCodes.Status200OK;
                    await httpContext.Response.WriteAsync(JsonUtils.SerializeObject(new { success = true, token = sessionToken, account }));
                } else {
                    httpContext.Response.StatusCode = StatusCodes.Status200OK;
                    await httpContext.Response.WriteAsync(JsonUtils.SerializeObject(new { success = false, message = "Incorrect password." }));
                }
            } else await DefaultResponse.FieldsMissing(httpContext);
        }
    }
}