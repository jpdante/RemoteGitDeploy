using System.Net.Mail;
using System.Threading.Tasks;
using HtcSharp.HttpModule.Http.Abstractions;
using RemoteGitDeploy.Extensions;
using RemoteGitDeploy.Mvc;

namespace RemoteGitDeploy.Models.RequestData {
    public class NewTeamData : IHttpJsonObject {

        public string Name { get; set; }

        public string Description { get; set; }

        public async Task<bool> ValidateData(HttpContext httpContext) {
            if (string.IsNullOrEmpty(Name)) {
                await httpContext.Response.SendRequestErrorAsync(-1, "Missing field 'name'.");
                return false;
            }

            return true;
        }
    }
}