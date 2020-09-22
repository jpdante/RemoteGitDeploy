using HtcSharp.HttpModule.Routing;

namespace RemoteGitDeploy.Mvc {
    public class HttpGetAttribute : HttpMethodAttribute {

        public HttpGetAttribute(string path, bool requireSession = false) : base("GET", path, ContentType.DEFAULT, requireSession) { }

    }
}
