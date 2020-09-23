using System.Net.Mail;
using System.Threading.Tasks;
using HtcSharp.HttpModule.Http.Abstractions;
using RemoteGitDeploy.Extensions;
using RemoteGitDeploy.Mvc;

namespace RemoteGitDeploy.Models.RequestData {
    public class NewSnippetData : IHttpJsonObject {

        public string Description { get; set; }

        public SnippetFile[] Files { get; set; }

        public async Task<bool> ValidateData(HttpContext httpContext) {
            if (Files.Length == 0) {
                await httpContext.Response.SendRequestErrorAsync(-1, "Missing field 'files'.");
                return false;
            }

            foreach (var file in Files) {
                if (!await file.ValidateData(httpContext)) return false;
            }

            return true;
        }

        public class SnippetFile : IHttpJsonObject {

            public string Filename { get; set; }
            public string Code { get; set; }
            public string Language { get; set; }

            public async Task<bool> ValidateData(HttpContext httpContext) {

                if (string.IsNullOrEmpty(Filename)) {
                    await httpContext.Response.SendRequestErrorAsync(-1, "Missing field 'filename' in snippet file.");
                    return false;
                }

                if (string.IsNullOrEmpty(Code)) {
                    await httpContext.Response.SendRequestErrorAsync(-1, "Missing field 'code' in snippet file.");
                    return false;
                }

                if (string.IsNullOrEmpty(Language)) {
                    await httpContext.Response.SendRequestErrorAsync(-1, "Missing field 'language' in snippet file.");
                    return false;
                }

                return true;
            }
        }
    }
}