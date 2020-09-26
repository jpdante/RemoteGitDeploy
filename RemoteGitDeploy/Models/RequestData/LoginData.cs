using System.Threading.Tasks;
using HtcSharp.HttpModule.Http.Abstractions;
using RemoteGitDeploy.Extensions;
using RemoteGitDeploy.Mvc;

namespace RemoteGitDeploy.Models.RequestData {
    public class LoginData : IHttpJsonObject {

        public string Username { get; set; }

        public string Password { get; set; }

        public bool RememberMe { get; set; }

        public async Task<bool> ValidateData(HttpContext httpContext) {
            if (string.IsNullOrEmpty(Username)) {
                await httpContext.Response.SendRequestErrorAsync(-1, "Missing field 'username'.");
                return false;
            }

            if (string.IsNullOrEmpty(Password)) {
                await httpContext.Response.SendRequestErrorAsync(-1, "Missing field 'password'.");
                return false;
            }

            return true;
        }

    }
}