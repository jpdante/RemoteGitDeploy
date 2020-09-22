using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using HtcSharp.HttpModule.Http.Abstractions;

namespace RemoteGitDeploy.Mvc {
    public interface IHttpJsonObject {

        public Task<bool> ValidateData(HttpContext httpContext);

    }
}
