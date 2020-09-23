using System;
using System.Linq;
using System.Threading.Tasks;
using HtcSharp.HttpModule.Http.Abstractions;
using HtcSharp.HttpModule.Http.Abstractions.Extensions;
using HtcSharp.HttpModule.Routing;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using RemoteGitDeploy.Core;
using RemoteGitDeploy.Extensions;
using RemoteGitDeploy.Models.New;
using RemoteGitDeploy.Models.RequestData;
using RemoteGitDeploy.Mvc;

namespace RemoteGitDeploy.Controllers {
    public class SnippetController {
        [HttpPost("/api/snippet/new", ContentType.JSON, true)]
        public static async Task NewSnippet(HttpContext httpContext, NewSnippetData newSnippetData) {
            await using var context = new RgdContext();

            if (!httpContext.Session.GetAccountId(out long accountId)) throw new Exception("Failed to get accountId");
            var creatorPermissions = await (from a in context.Accounts where a.Id.Equals(accountId) select a.Permissions).FirstOrDefaultAsync();
            if ((creatorPermissions & Permission.WriteSnippet) != Permission.WriteSnippet) throw new HttpException(403, "No WriteSnippet permission.");

            var snippet = new Snippet(accountId, newSnippetData.Description);

            await context.Snippets.AddAsync(snippet);

            foreach (var snippetFileData in newSnippetData.Files) { 
                var snippetFile = new SnippetFile(snippet.Id, snippetFileData.Filename, snippetFileData.Code, snippetFileData.Language);
                await context.SnippetFiles.AddAsync(snippetFile);
            }

            await context.SaveChangesAsync();

            await httpContext.Response.WriteAsync(JsonConvert.SerializeObject(new {
                success = true,
                guid = snippet.Guid
            }));
        }
    }
}
