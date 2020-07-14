using System;
using System.Threading.Tasks;
using HtcSharp.Core.Utils;
using HtcSharp.HttpModule.Http.Abstractions;
using HtcSharp.HttpModule.Routing;
using RemoteGitDeploy.Security;
using RemoteGitDeploy.Utils;

namespace RemoteGitDeploy.API.New {
    public class User : IAPI {
        public string FileName => "/api/new/user";
        public string RequestMethod => HttpMethods.Post;
        public string RequestContentType => ContentType.JSON.ToValue();
        public bool NeedAuthentication => true;

        public async Task OnRequest(HttpContext httpContext) {
            var data = await new JsonData().Load(httpContext);
            if (data.TryGetValue("firstName", out string firstName) &&
                data.TryGetValue("lastName", out string lastName) &&
                data.TryGetValue("username", out string username) &&
                data.TryGetValue("email", out string email)) {
                if (!DataValidation.ValidateUsername(firstName, out string error)) {
                    await DefaultResponse.InvalidField(httpContext, "firstname", error);
                    return;
                }
                if (!DataValidation.ValidateUsername(lastName, out string error2)) {
                    await DefaultResponse.InvalidField(httpContext, "lastname", error2);
                    return;
                }
                if (!DataValidation.ValidateUsername(username, out string error3)) {
                    await DefaultResponse.InvalidField(httpContext, "username", error3);
                    return;
                }
                if (!DataValidation.ValidateEmail(email)) {
                    await DefaultResponse.InvalidField(httpContext, "email", "Invalid email.");
                    return;
                }

                await using var conn = await HtcPlugin.DatabaseManager.GetConnectionAsync();

                if (await HtcPlugin.DatabaseManager.HasUsernameAsync(username, conn)) {
                    await DefaultResponse.InvalidField(httpContext, "username", "There is already an account with that username.");
                    return;
                }

                if (await HtcPlugin.DatabaseManager.HasUsernameAsync(email, conn)) {
                    await DefaultResponse.InvalidField(httpContext, "email", "There is already an account with that email.");
                    return;
                }

                string password = PasswordSecurity.GeneratePassword();
                byte[] salt = PasswordSecurity.CreateSalt();
                string encodedSalt = Convert.ToBase64String(salt);
                byte[] hashedPassword = PasswordSecurity.HashPasswordAsync(password, salt);
                string encodedHashedPassword = Convert.ToBase64String(hashedPassword);

                if (await HtcPlugin.DatabaseManager.NewAccountAsync(firstName, lastName, email, username, encodedHashedPassword, encodedSalt, conn)) {
                    httpContext.Response.StatusCode = StatusCodes.Status200OK;
                    await httpContext.Response.WriteAsync(JsonUtils.SerializeObject(new { success = true, password }));
                } else {
                    httpContext.Response.StatusCode = StatusCodes.Status200OK;
                    await httpContext.Response.WriteAsync(JsonUtils.SerializeObject(new { success = false, message = "Failed to create account." }));
                }
            } else await DefaultResponse.FieldsMissing(httpContext);
        }
    }
}