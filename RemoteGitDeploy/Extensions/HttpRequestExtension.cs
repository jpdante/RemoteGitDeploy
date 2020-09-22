using System.IO;
using System.Text;
using System.Threading.Tasks;
using HtcSharp.HttpModule.Http.Abstractions;

namespace RemoteGitDeploy.Extensions {
    public static class HttpRequestExtension {
        public static async Task<string> BodyToString(this HttpRequest httpRequest) {
            using var streamReader = new StreamReader(httpRequest.Body, Encoding.UTF8);
            return await streamReader.ReadToEndAsync();
        }
    }
}