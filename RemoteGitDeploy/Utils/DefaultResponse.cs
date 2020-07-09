using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using HtcSharp.HttpModule.Http.Abstractions;

namespace RemoteGitDeploy.Utils {
    public static class DefaultResponse {

        public static async Task InternalError(HttpContext httpContext) {
            httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
            await httpContext.Response.WriteAsync("{\"success\":false,\"message\":\"Internal server error.\"}");
        }

        public static async Task InvalidContentType(HttpContext httpContext, string expectedContentType) {
            httpContext.Response.StatusCode = StatusCodes.Status415UnsupportedMediaType;
            await httpContext.Response.WriteAsync($"{{\"success\":false,\"message\":\"Invalid content type, expected '{expectedContentType}'.\"}}");
        }

        public static async Task UnknownApi(HttpContext httpContext, string apiPath) {
            httpContext.Response.StatusCode = StatusCodes.Status404NotFound;
            await httpContext.Response.WriteAsync($"{{\"success\":false,\"message\":\"Unknown API '{apiPath}'.\"}}");
        }

        public static async Task InvalidSession(HttpContext httpContext) {
            httpContext.Response.StatusCode = StatusCodes.Status403Forbidden;
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

        public static async Task InvalidField(HttpContext httpContext, string field) {
            httpContext.Response.StatusCode = StatusCodes.Status403Forbidden;
            await httpContext.Response.WriteAsync($"{{\"success\":false,\"message\":\"Invalid '{field}' field.\",\"field\":\"{field}\"}}");
        }

        /*

        public static async Task FailedParsingData(HttpContext httpContext) {
            httpContext.Response.StatusCode = StatusCodes.Status200OK;
            await httpContext.Response.WriteAsync("{\"success\":false,\"message\":\"errors.failedParsingData\"}");
        }

        public static async Task SuccessUpdate(HttpContext httpContext) {
            httpContext.Response.StatusCode = StatusCodes.Status200OK;
            await httpContext.Response.WriteAsync("{\"success\":true}");
        }

        public static async Task FailedUpdate(HttpContext httpContext) => await InternalError(httpContext);
        */
    }
}
