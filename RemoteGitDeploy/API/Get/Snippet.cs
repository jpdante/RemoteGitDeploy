using System.Collections.Generic;
using System.Threading.Tasks;
using HtcSharp.Core.Utils;
using HtcSharp.HttpModule.Http.Abstractions;
using HtcSharp.HttpModule.Routing;
using RemoteGitDeploy.Model.Database;
using RemoteGitDeploy.Utils;

namespace RemoteGitDeploy.API.Get {
    public class Snippet : IAPI {
        public string FileName => "/api/get/snippet";
        public string RequestMethod => HttpMethods.Post;
        public string RequestContentType => ContentType.JSON.ToValue();
        public bool NeedAuthentication => true;

        public async Task OnRequest(HttpContext httpContext) {
            var data = await new JsonData().Load(httpContext);
            if (data.TryGetValue("guid", out string guid)) {
                await using var conn = await HtcPlugin.DatabaseManager.GetConnectionAsync();
                var snippet = await HtcPlugin.DatabaseManager.GetSnippetAsync(guid, conn);
                if (snippet == null) {
                    await DefaultResponse.Failed(httpContext);
                    return;
                }
                snippet.SnippetFiles = new List<SnippetFile>();
                foreach (var snippetFile in await HtcPlugin.DatabaseManager.GetSnippetFilesAsync(snippet.Id, conn)) {
                    snippet.SnippetFiles.Add(snippetFile);
                }
                await httpContext.Response.WriteAsync(JsonUtils.SerializeObject(new { success = true, snippet }));
            } else await DefaultResponse.FieldsMissing(httpContext);
        }
    }
}