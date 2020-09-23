using System;
using System.Collections.Generic;
using System.Text;

namespace RemoteGitDeploy.Mvc {
    public class HttpException : Exception {

        public readonly int Status;

        public HttpException(int status, string message) : base(message) {
            Status = status;
        }
    }
}
