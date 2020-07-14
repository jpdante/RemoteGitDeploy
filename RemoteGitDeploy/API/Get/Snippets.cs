using System.Collections.Generic;
using System.Threading.Tasks;
using HtcSharp.Core.Utils;
using HtcSharp.HttpModule.Http.Abstractions;
using HtcSharp.HttpModule.Routing;
using RemoteGitDeploy.Model.Database;

namespace RemoteGitDeploy.API.Get {
    public class Snippets : IAPI {
        public string FileName => "/api/get/snippets";
        public string RequestMethod => HttpMethods.Get;
        public string RequestContentType => null;
        public bool NeedAuthentication => true;

        public async Task OnRequest(HttpContext httpContext) {
            await using var conn = await HtcPlugin.DatabaseManager.GetConnectionAsync();
            Model.Database.Snippet[] snippets = await HtcPlugin.DatabaseManager.GetSnippetsAsync(conn);
            foreach (var snippet in snippets) {
                snippet.SnippetFiles = new List<SnippetFile>();
                foreach (string fileName in await HtcPlugin.DatabaseManager.GetSnippetsFileNamesAsync(snippet.Id, conn)) {
                    snippet.SnippetFiles.Add(new SnippetFile(-1, snippet.Id, fileName, null, null));
                }
            }
            await httpContext.Response.WriteAsync(JsonUtils.SerializeObject(new { success = true, snippets }));
        }
    }
}