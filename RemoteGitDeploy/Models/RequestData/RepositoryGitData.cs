using System;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using HtcSharp.HttpModule.Http.Abstractions;
using RemoteGitDeploy.Extensions;
using RemoteGitDeploy.Mvc;

namespace RemoteGitDeploy.Models.RequestData {
    public class RepositoryGitData : IHttpJsonObject {

        public string Username { get; set; }

        public string Token { get; set; }

        public string Git { get; set; }

        public async Task<bool> ValidateData(HttpContext httpContext) {
            if (string.IsNullOrEmpty(Git)) {
                await httpContext.Response.SendRequestErrorAsync(-1, "Missing field 'git'.");
                return false;
            }
            if (string.IsNullOrEmpty(Username)) {
                await httpContext.Response.SendRequestErrorAsync(-1, "Missing field 'username'.");
                return false;
            }
            if (string.IsNullOrEmpty(Token)) {
                await httpContext.Response.SendRequestErrorAsync(-1, "Missing field 'token'.");
                return false;
            }
            if (Username.Length > 64) {
                await httpContext.Response.SendRequestErrorAsync(-1, "Username can only be a maximum of 64 characters.");
                return false;
            }
            if (Token.Length != 40) {
                await httpContext.Response.SendRequestErrorAsync(-1, "Private access token needs to be 40 characters long.");
                return false;
            }
            if (Username.Any(char.IsWhiteSpace)) {
                await httpContext.Response.SendRequestErrorAsync(-1, "Username can not have spaces.");
                return false;
            }
            if (Token.Any(char.IsWhiteSpace)) {
                await httpContext.Response.SendRequestErrorAsync(-1, "Token can not have spaces.");
                return false;
            }
            if (Git.Any(char.IsWhiteSpace)) {
                await httpContext.Response.SendRequestErrorAsync(-1, "Git link name can not have spaces.");
                return false;
            }
            try {
                if (!Uri.TryCreate(Git, UriKind.RelativeOrAbsolute, out var gitUri)) {
                    await httpContext.Response.SendRequestErrorAsync(-1, "Git link must be a valid URL.");
                    return false;
                }
                if (!gitUri.Scheme.Equals("http") && !gitUri.Scheme.Equals("https")) {
                    await httpContext.Response.SendRequestErrorAsync(-1, "Git link must use the http or https protocol.");
                    return false;
                }
                if (!gitUri.AbsolutePath.EndsWith(".git")) {
                    await httpContext.Response.SendRequestErrorAsync(-1, "Git link must be a valid git URL.");
                    return false;
                }
            } catch {
                await httpContext.Response.SendRequestErrorAsync(-1, "Git link must be a valid URL.");
                return false;
            }
            return true;
        }
    }
}