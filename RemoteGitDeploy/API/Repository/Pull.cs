using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using HtcSharp.HttpModule.Http.Abstractions;
using HtcSharp.HttpModule.Routing;
using RemoteGitDeploy.Utils;

namespace RemoteGitDeploy.API.Repository {
    public class Pull : IAPI {
        public string FileName => "/api/repository/pull";
        public string RequestMethod => HttpMethods.Post;
        public string RequestContentType => ContentType.JSON.ToValue();
        public bool NeedAuthentication => false;

        private const string Sha1Prefix = "sha1=";

        public async Task OnRequest(HttpContext httpContext) {
            if (httpContext.Request.Query.TryGetValue("repository", out var repository) &&
                httpContext.Request.Headers.TryGetValue("X-GitHub-Event", out var eventName) &&
                httpContext.Request.Headers.TryGetValue("X-Hub-Signature", out var signature) &&
                httpContext.Request.Headers.TryGetValue("X-GitHub-Delivery", out var delivery)) {
                using var reader = new StreamReader(httpContext.Request.Body);
                string payload = await reader.ReadToEndAsync();
                if (await IsGithubPushAllowed(payload, eventName, signature, httpContext)) {
                    var repositoryModel = HtcPlugin.RepositoryManager.GetRepository(repository);
                    if (repositoryModel == null) {
                        httpContext.Response.StatusCode = 404;
                        await httpContext.Response.WriteAsync("Repository not found");
                        return;
                    }
                    await HtcPlugin.RepositoryManager.PullRepository(repositoryModel);
                    httpContext.Response.StatusCode = 200;
                    await httpContext.Response.WriteAsync("Ok");
                }
            } else await DefaultResponse.FieldsMissing(httpContext);
        }

        private static async Task<bool> IsGithubPushAllowed(string payload, string eventName, string signatureWithPrefix, HttpContext httpContext) {
            if (string.IsNullOrWhiteSpace(payload)) {
                httpContext.Response.StatusCode = 500;
                await httpContext.Response.WriteAsync("Invalid payload/body");
                return false;
            }
            if (string.IsNullOrWhiteSpace(eventName)) {
                httpContext.Response.StatusCode = 500;
                await httpContext.Response.WriteAsync("Invalid event");
                return false;
            }
            if (string.IsNullOrWhiteSpace(signatureWithPrefix)) {
                httpContext.Response.StatusCode = 403;
                await httpContext.Response.WriteAsync("Invalid signature");
                return false;
            }
            if (!signatureWithPrefix.StartsWith(Sha1Prefix, StringComparison.OrdinalIgnoreCase)) {
                httpContext.Response.StatusCode = 500;
                await httpContext.Response.WriteAsync("Invalid SHA-1");
                return false;
            }
            string signature = signatureWithPrefix.Substring(Sha1Prefix.Length);
            byte[] secret = Encoding.UTF8.GetBytes(HtcPlugin.RepositorySecreteKey);
            byte[] payloadBytes = Encoding.UTF8.GetBytes(payload);
            using var hmSha1 = new HMACSHA1(secret);
            byte[] hash = hmSha1.ComputeHash(payloadBytes);
            string hashString = ToHexString(hash);
            if (hashString.Equals(signature)) return true;
            httpContext.Response.StatusCode = 403;
            await httpContext.Response.WriteAsync("Forbidden");
            return false;
        }

        private static string ToHexString(IReadOnlyCollection<byte> bytes) {
            var builder = new StringBuilder(bytes.Count * 2);
            foreach (byte b in bytes) {
                builder.AppendFormat("{0:x2}", b);
            }
            return builder.ToString();
        }
    }
}