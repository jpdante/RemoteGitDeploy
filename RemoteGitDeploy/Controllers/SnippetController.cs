using System;
using System.Collections.Generic;
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
using RemoteGitDeploy.Models.Views;
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

        [HttpPost("/api/snippet/get", ContentType.JSON, true)]
        public static async Task GetSnippet(HttpContext httpContext, SnippetData snippetData) {
            await using var context = new RgdContext();

            if (!httpContext.Session.GetAccountId(out long accountId)) throw new Exception("Failed to get accountId");
            var creatorPermissions = await (from a in context.Accounts where a.Id.Equals(accountId) select a.Permissions).FirstOrDefaultAsync();
            if ((creatorPermissions & Permission.ReadSnippet) != Permission.ReadSnippet) throw new HttpException(403, "No ReadSnippet permission.");

            var snippetRaw = await (from s in context.Snippets where s.Guid.Equals(snippetData.Guid) select s).FirstOrDefaultAsync();
            if (snippetRaw == null) {
                await httpContext.Response.SendRequestErrorAsync(4, "There is no snippet with this guid.");
                return;
            }

            SnippetFile[] snippetFilesRaw = await (from sf in context.SnippetFiles where sf.SnippetId == snippetRaw.Id select sf).ToArrayAsync();
            SnippetFileView[] snippetFiles = snippetFilesRaw.Select(snippetFile => new SnippetFileView(snippetFile)).ToArray();

            var snippet = new SnippetView(snippetRaw, snippetFiles);

            await httpContext.Response.WriteAsync(JsonConvert.SerializeObject(new { success = true, snippet }));
        }


        [HttpGet("/api/snippets/get", true)]
        public static async Task GetSnippets(HttpContext httpContext) {
            await using var context = new RgdContext();

            if (!httpContext.Session.GetAccountId(out long accountId)) throw new Exception("Failed to get accountId");
            var creatorPermissions = await (from a in context.Accounts where a.Id.Equals(accountId) select a.Permissions).FirstOrDefaultAsync();
            if ((creatorPermissions & Permission.ManageSnippet) != Permission.ManageSnippet) throw new HttpException(403, "No ManageSnippet permission.");

            Snippet[] snippetsRaw = await (from s in context.Snippets select s).ToArrayAsync();
            var snippets = new List<SnippetView>();
            foreach (var snippet in snippetsRaw) {
                SnippetFile[] snippetFilesRaw = await (from sf in context.SnippetFiles where sf.SnippetId == snippet.Id select sf).ToArrayAsync();
                SnippetFileView[] snippetFiles = snippetFilesRaw.Select(snippetFile => new SnippetFileView(snippetFile)).ToArray();
                snippets.Add(new SnippetView(snippet, snippetFiles));
            }

            await httpContext.Response.WriteAsync(JsonConvert.SerializeObject(new { success = true, snippets }));
        }
    }
}
