using System.Net.Mail;
using System.Threading.Tasks;
using HtcSharp.HttpModule.Http.Abstractions;
using RemoteGitDeploy.Extensions;
using RemoteGitDeploy.Mvc;

namespace RemoteGitDeploy.Model.Request {
    public class LoginData : IHttpJsonObject {

        public string Email { get; set; }

        public string Password { get; set; }

        public async Task<bool> ValidateData(HttpContext httpContext) {
            if (Email == null) {
                await httpContext.Response.SendRequestErrorAsync(-1, "Missing field 'email'.");
                return false;
            }

            if (Password == null) {
                await httpContext.Response.SendRequestErrorAsync(-1, "Missing field 'password'.");
                return false;
            }

            try {
                _ = new MailAddress(Email);
            } catch {
                await httpContext.Response.SendRequestErrorAsync(-1, "Invalid email.");
                return false;
            }

            if (Password.Length < 8) {
                await httpContext.Response.SendRequestErrorAsync(-1, "Password must be at least 8 characters long.");
                return false;
            }

            return true;
        }

    }
}