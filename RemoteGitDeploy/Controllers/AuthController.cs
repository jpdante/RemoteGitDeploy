using System;
using System.Linq;
using System.Threading.Tasks;
using HtcSharp.Core.Logging.Abstractions;
using HtcSharp.HttpModule.Http.Abstractions;
using HtcSharp.HttpModule.Http.Abstractions.Extensions;
using HtcSharp.HttpModule.Routing;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using RemoteGitDeploy.Extensions;
using RemoteGitDeploy.Models.New;
using RemoteGitDeploy.Models.RequestData;
using RemoteGitDeploy.Models.Views;
using RemoteGitDeploy.Mvc;
using RemoteGitDeploy.Security;

namespace RemoteGitDeploy.Controllers {
    public class AuthController {

        [HttpPost("/api/auth/login", ContentType.JSON)]
        public static async Task Login(HttpContext httpContext, LoginData loginData) {
            await using var context = new RgdContext();

            var account = await (from a in context.Accounts where a.Email.Equals(loginData.Username) select a).FirstOrDefaultAsync();
            if (account == null) {
                account = await (from a in context.Accounts where a.Username.Equals(loginData.Username) select a).FirstOrDefaultAsync();
                if (account == null) {
                    await httpContext.Response.SendRequestErrorAsync(4, "There is no account registered with this email.");
                    return;
                }
            }

            if (await Password.CheckPassword(account.Password, loginData.Password)) {
                try {
                    account.LastAccess = DateTime.UtcNow;
                    context.Accounts.Update(account);
                    await context.SaveChangesAsync();
                    httpContext.Session.Set("account", account.Id);
                    await httpContext.Session.CommitAsync();
                    await httpContext.Response.WriteAsync(JsonConvert.SerializeObject(new { success = true, token = httpContext.Session.Id, account = new AccountView(account) }));

                    var accessHistory = new AccessHistory(account.Id, httpContext.Connection.RemoteIpAddress.ToString());
                    await context.AccessHistory.AddAsync(accessHistory);
                    account.LastAccess = DateTime.UtcNow;
                    context.Accounts.Update(account);
                    await context.SaveChangesAsync();
                } catch (Exception ex) {
                    HtcPlugin.Logger.LogError(ex);
                    await httpContext.Response.SendInternalErrorAsync(8, "An internal failure occurred while attempting to create the account. Please try again later.");
                }
            } else {
                await httpContext.Response.SendRequestErrorAsync(9, "The password is incorrect.");
            }
        }

        [HttpGet("/api/auth/checksession")]
        public static async Task CheckSession(HttpContext httpContext) {
            if (httpContext.Session.IsAvailable) {
                await httpContext.Response.WriteAsync("{\"isValid\":true}");
            } else {
                await httpContext.Response.WriteAsync("{\"isValid\":false}");
            }
        }

        [HttpGet("/api/auth/logout", true)]
        public static async Task Logout(HttpContext httpContext) {
            httpContext.Session.Clear();
            await httpContext.Session.CommitAsync();
            await httpContext.Response.WriteAsync("{\"ok\":true}");
        }
    }
}