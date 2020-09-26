using System;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using HtcSharp.HttpModule.Http.Abstractions;
using RemoteGitDeploy.Extensions;
using RemoteGitDeploy.Mvc;

namespace RemoteGitDeploy.Models.RequestData {
    public class NewRepositoryData : IHttpJsonObject {

        public string Username { get; set; }

        public string Token { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string Team { get; set; }

        public string Branch { get; set; }

        public string Git { get; set; }

        public async Task<bool> ValidateData(HttpContext httpContext) {
            if (string.IsNullOrEmpty(Username)) {
                await httpContext.Response.SendRequestErrorAsync(-1, "Missing field 'username'.");
                return false;
            }
            if (string.IsNullOrEmpty(Token)) {
                await httpContext.Response.SendRequestErrorAsync(-1, "Missing field 'token'.");
                return false;
            }
            if (string.IsNullOrEmpty(Name)) {
                await httpContext.Response.SendRequestErrorAsync(-1, "Missing field 'name'.");
                return false;
            }
            if (string.IsNullOrEmpty(Team)) {
                await httpContext.Response.SendRequestErrorAsync(-1, "Missing field 'team'.");
                return false;
            }
            if (string.IsNullOrEmpty(Branch)) {
                await httpContext.Response.SendRequestErrorAsync(-1, "Missing field 'branch'.");
                return false;
            }
            if (string.IsNullOrEmpty(Git)) {
                await httpContext.Response.SendRequestErrorAsync(-1, "Missing field 'git'.");
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
            if (Name.Length > 64) {
                await httpContext.Response.SendRequestErrorAsync(-1, "Name can only be a maximum of 64 characters.");
                return false;
            }
            if (Team.Length > 64) {
                await httpContext.Response.SendRequestErrorAsync(-1, "Name can only be a maximum of 64 characters.");
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
            if (Name.Any(char.IsWhiteSpace)) {
                await httpContext.Response.SendRequestErrorAsync(-1, "Repository name can not have spaces.");
                return false;
            }
            if (Team.Any(char.IsWhiteSpace)) {
                await httpContext.Response.SendRequestErrorAsync(-1, "Team name can not have spaces.");
                return false;
            }
            if (Git.Any(char.IsWhiteSpace)) {
                await httpContext.Response.SendRequestErrorAsync(-1, "Git link name can not have spaces.");
                return false;
            }
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
            return true;
        }
    }
}