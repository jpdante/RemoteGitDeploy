using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using HtcSharp.Core.Logging.Abstractions;
using HtcSharp.Core.Utils;
using HtcSharp.HttpModule.Http.Abstractions;
using HtcSharp.HttpModule.Routing;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RemoteGitDeploy.Model.Database;
using RemoteGitDeploy.Utils;

namespace RemoteGitDeploy.API.New {
    public class Snippet : IAPI {
        public string FileName => "/api/new/snippet";
        public string RequestMethod => HttpMethods.Post;
        public string RequestContentType => ContentType.JSON.ToValue();
        public bool NeedAuthentication => true;

        public async Task OnRequest(HttpContext httpContext) {
            using var reader = new StreamReader(httpContext.Request.Body, Encoding.UTF8, true, 2048, true);
            var data = JObject.Parse(await reader.ReadToEndAsync());
            if (data.TryGetValue("description", out var descriptionToken) &&
                data.TryGetValue("files", out var filesToken)) {

                if (descriptionToken == null) {
                    await DefaultResponse.FailedParsingData(httpContext);
                    return;
                }
                if (filesToken == null || filesToken.Type != JTokenType.Array) {
                    await DefaultResponse.FailedParsingData(httpContext);
                    return;
                }
                var description = descriptionToken.ToObject<string>();
                if (!(filesToken is JArray filesTokens)) {
                    await DefaultResponse.FailedParsingData(httpContext);
                    return;
                }
                await using var conn = await HtcPlugin.DatabaseManager.GetConnectionAsync();
                var transaction = await conn.BeginTransactionAsync();
                var guid = Guid.NewGuid().ToString("N");
                long snippetId = StaticData.IdGenerator.CreateId();
                long account = await HtcPlugin.CacheManager.GetUserIdFromSessionAsync(httpContext.Session.Id);
                if (account == -1) {
                    await DefaultResponse.InvalidSession(httpContext);
                    return;
                }
                try {
                    if (await HtcPlugin.DatabaseManager.NewSnippetAsync(snippetId, guid, account, description, conn, transaction)) {
                        var files = new List<SnippetFile>();
                        foreach (var jToken in filesTokens) {
                            var fileObject = (JObject)jToken;
                            if (!fileObject.TryGetValue("filename", out var filenameToken) || !fileObject.TryGetValue("code", out var codeToken)) continue;
                            var filename = filenameToken?.ToObject<string>();
                            var code = codeToken?.ToObject<string>();
                            if (filename == null || filename.Replace(" ", "").Equals("")) continue;
                            if (code == null || filename.Replace(" ", "").Equals("")) continue;
                            files.Add(new SnippetFile(-1, snippetId, filename, code));
                        }
                        foreach (var file in files) {
                            await HtcPlugin.DatabaseManager.NewSnippetFileAsync(snippetId, file.Filename, file.Code, conn, transaction);
                        }
                        await transaction.CommitAsync();
                        httpContext.Response.StatusCode = StatusCodes.Status200OK;
                        await httpContext.Response.WriteAsync(JsonUtils.SerializeObject(new { success = true, guid }));
                    }
                } catch (Exception ex) {
                    HtcPlugin.Logger.LogError(ex);
                    await transaction.RollbackAsync();
                    await DefaultResponse.Failed(httpContext);
                }
            } else await DefaultResponse.FieldsMissing(httpContext);
        }
    }
}