using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using HtcSharp.HttpModule.Http.Abstractions;

namespace RemoteGitDeploy.Utils {
    public static class DefaultResponse {

        public static async Task InternalError(HttpContext httpContext) {
            httpContext.Response.StatusCode = StatusCodes.Status200OK;
            await httpContext.Response.WriteAsync("{\"success\":false,\"message\":\"Internal server error.\"}");
        }

        public static async Task InvalidContentType(HttpContext httpContext, string expectedContentType) {
            httpContext.Response.StatusCode = StatusCodes.Status200OK;
            await httpContext.Response.WriteAsync($"{{\"success\":false,\"message\":\"Invalid content type, expected '{expectedContentType}'.\"}}");
        }

        public static async Task UnknownApi(HttpContext httpContext, string apiPath) {
            httpContext.Response.StatusCode = StatusCodes.Status200OK;
            await httpContext.Response.WriteAsync($"{{\"success\":false,\"message\":\"Unknown API '{apiPath}'.\"}}");
        }

        public static async Task InvalidSession(HttpContext httpContext) {
            httpContext.Response.StatusCode = StatusCodes.Status200OK;
            await httpContext.Response.WriteAsync("{\"success\":false,\"invalidSession\":true,\"message\":\"Invalid or non-existent session.\"}");
        }

        public static async Task FieldsMissing(HttpContext httpContext) {
            httpContext.Response.StatusCode = StatusCodes.Status200OK;
            await httpContext.Response.WriteAsync("{\"success\":false,\"message\":\"There are fields missing.\"}");
        }

        public static async Task RequestSizeTooBig(HttpContext httpContext) {
            httpContext.Response.StatusCode = StatusCodes.Status200OK;
            await httpContext.Response.WriteAsync("{\"success\":false,\"message\":\"The request size is too large. Maximum of 1000000 bytes / 1 MB\"}");
        }

        public static async Task InvalidField(HttpContext httpContext, string field, string error = null) {
            httpContext.Response.StatusCode = StatusCodes.Status200OK;
            if(error == null)  await httpContext.Response.WriteAsync($"{{\"success\":false,\"message\":\"Invalid '{field}' field.\",\"field\":\"{field}\"}}");
            else await httpContext.Response.WriteAsync($"{{\"success\":false,\"message\":\"{error}\",\"field\":\"{field}\"}}");
        }

        public static async Task Success(HttpContext httpContext) {
            httpContext.Response.StatusCode = StatusCodes.Status200OK;
            await httpContext.Response.WriteAsync("{\"success\":true}");
        }

        public static async Task Failed(HttpContext httpContext) {
            httpContext.Response.StatusCode = StatusCodes.Status200OK;
            await httpContext.Response.WriteAsync("{\"success\":false}");
        }

        public static async Task Failed(HttpContext httpContext, string message) {
            httpContext.Response.StatusCode = StatusCodes.Status200OK;
            await httpContext.Response.WriteAsync($"{{\"success\":false,\"message\":\"{message}\"}}");
        }

        public static async Task FailedParsingData(HttpContext httpContext) {
            httpContext.Response.StatusCode = StatusCodes.Status200OK;
            await httpContext.Response.WriteAsync("{\"success\":false,\"message\":\"A failure occurred while attempting to decode the data.\"}");
        }
    }
}
