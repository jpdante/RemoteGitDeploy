using System.Net.Mail;
using System.Threading.Tasks;
using HtcSharp.HttpModule.Http.Abstractions;
using RemoteGitDeploy.Extensions;
using RemoteGitDeploy.Mvc;

namespace RemoteGitDeploy.Models.RequestData {
    public class NewAccountData : IHttpJsonObject {

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

        public string Username { get; set; }

        public PermissionsData Permissions { get; set; }

        public async Task<bool> ValidateData(HttpContext httpContext) {
            if (FirstName == null) {
                await httpContext.Response.SendRequestErrorAsync(-1, "Missing field 'firstName'.");
                return false;
            }

            if (LastName == null) {
                await httpContext.Response.SendRequestErrorAsync(-1, "Missing field 'lastName'.");
                return false;
            }

            if (Email == null) {
                await httpContext.Response.SendRequestErrorAsync(-1, "Missing field 'email'.");
                return false;
            }

            if (Username == null) {
                await httpContext.Response.SendRequestErrorAsync(-1, "Missing field 'email'.");
                return false;
            }

            if (Permissions == null) {
                await httpContext.Response.SendRequestErrorAsync(-1, "Missing field 'permissions'.");
                return false;
            }

            if (FirstName.Length < 3) {
                await httpContext.Response.SendRequestErrorAsync(1, "First name must be at least 3 characters long.");
                return false;
            }

            if (LastName.Length < 3) {
                await httpContext.Response.SendRequestErrorAsync(1, "Last name must be at least 3 characters long.");
                return false;
            }

            if (Username.Length < 3) {
                await httpContext.Response.SendRequestErrorAsync(1, "Username must be at least 3 characters long.");
                return false;
            }

            if (FirstName.Length > 255) {
                await httpContext.Response.SendRequestErrorAsync(1, "First name can have a maximum of 255 characters.");
                return false;
            }

            if (LastName.Length > 255) {
                await httpContext.Response.SendRequestErrorAsync(1, "Last name can have a maximum of 255 characters.");
                return false;
            }

            if (Username.Length > 255) {
                await httpContext.Response.SendRequestErrorAsync(1, "Username can have a maximum of 255 characters.");
                return false;
            }

            try {
                _ = new MailAddress(Email);
            } catch {
                await httpContext.Response.SendRequestErrorAsync(-1, "Invalid email.");
                return false;
            }

            return true;
        }

        public class PermissionsData {

            public bool ReadRepository { get; set; }
            public bool WriteRepository { get; set; }
            public bool ManageRepository { get; set; }

            public bool ReadSnippet { get; set; }
            public bool WriteSnippet { get; set; }
            public bool ManageSnippet { get; set; }

            public bool ReadAccount { get; set; }
            public bool WriteAccount { get; set; }
            public bool ManageAccount { get; set; }

            public bool ReadTeam { get; set; }
            public bool WriteTeam { get; set; }
            public bool ManageTeam { get; set; }

        }
    }
}