using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using HtcSharp.HttpModule.Http.Abstractions;
using HtcSharp.HttpModule.Routing;
using RemoteGitDeploy.Security;
using RemoteGitDeploy.Utils;

namespace RemoteGitDeploy.API.Dev {
    public class PwdGen : IAPI {
        public string FileName => "/dev/pwdgen";
        public string RequestMethod => HttpMethods.Get;
        public string RequestContentType => null;
        public bool NeedAuthentication => false;

        public async Task OnRequest(HttpContext httpContext) {
            if (httpContext.Request.Query.TryGetValue("key", out var key) && httpContext.Request.Query.TryGetValue("pwd", out var pwd)) {
                if (key[0].Equals(HtcPlugin.DevKey)) {
                    httpContext.Response.StatusCode = 200;
                    httpContext.Response.ContentType = ContentType.HTML.ToValue();
                    byte[] salt = PasswordSecurity.CreateSalt();
                    string encodedSalt = Convert.ToBase64String(salt);
                    byte[] hashedPassword = PasswordSecurity.HashPasswordAsync(pwd[0], salt);
                    string encodedHashedPassword = Convert.ToBase64String(hashedPassword);
                    string data = $"&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<strong>Salt:</strong> {encodedSalt}<br/>" +
                                  $"<strong>Hashed Password:</strong> {encodedHashedPassword}";
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