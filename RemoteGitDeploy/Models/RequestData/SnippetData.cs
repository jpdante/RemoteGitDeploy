using System;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using HtcSharp.HttpModule.Http.Abstractions;
using RemoteGitDeploy.Extensions;
using RemoteGitDeploy.Mvc;

namespace RemoteGitDeploy.Models.RequestData {
    public class SnippetData : IHttpJsonObject {

        public string Guid { get; set; }

        public async Task<bool> ValidateData(HttpContext httpContext) {
            if (string.IsNullOrEmpty(Guid)) {
                await httpContext.Response.SendRequestErrorAsync(-1, "Missing field 'guid'.");
                return false;
            }
            if (Guid.Length != 36) {
                await httpContext.Response.SendRequestErrorAsync(-1, "Guid should have 36 characters.");
                return false;
            }
            return true;
        }
    }
}