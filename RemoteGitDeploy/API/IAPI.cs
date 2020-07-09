using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using HtcSharp.HttpModule.Http.Abstractions;

namespace RemoteGitDeploy.API {
    interface IAPI {

        public string FileName { get; }
        public string RequestMethod { get; }
        public string RequestContentType { get; }
        public bool NeedAuthentication { get; }

        public Task OnRequest(HttpContext httpContext);

    }
}
