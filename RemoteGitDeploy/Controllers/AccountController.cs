using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HtcSharp.HttpModule.Http.Abstractions;
using HtcSharp.HttpModule.Http.Abstractions.Extensions;
using HtcSharp.HttpModule.Routing;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using RemoteGitDeploy.Core;
using RemoteGitDeploy.Extensions;
using RemoteGitDeploy.Models.New;
using RemoteGitDeploy.Models.RequestData;
using RemoteGitDeploy.Models.Views;
using RemoteGitDeploy.Mvc;
using RemoteGitDeploy.Security;

namespace RemoteGitDeploy.Controllers {
    public class AccountController {

        [HttpPost("/api/account/new", ContentType.JSON, true)]
        public static async Task NewAccount(HttpContext httpContext, NewAccountData newAccountData) {
            await using var context = new RgdContext();

            if (!httpContext.Session.GetAccountId(out long accountId)) throw new Exception("Failed to get accountId");
            var creatorPermissions = await (from a in context.Accounts where a.Id.Equals(accountId) select a.Permissions).FirstOrDefaultAsync();
            if ((creatorPermissions & Permission.WriteAccount) != Permission.WriteAccount) throw new HttpException(403, "No WriteAccount permission.");

            bool hasAccountWithEmail = await (from a in context.Accounts where a.Email.Equals(newAccountData.Email) select a).AnyAsync();
            if (hasAccountWithEmail) {
                await httpContext.Response.SendRequestErrorAsync(8, "An account with that email address already exists.");
                return;
            }

            bool hasAccountWithUsername = await (from a in context.Accounts where a.Username.Equals(newAccountData.Username) select a).AnyAsync();
            if (hasAccountWithUsername) {
                await httpContext.Response.SendRequestErrorAsync(9, "An account with that username already exists.");
            }

            var permissions = Permission.None;
            if(newAccountData.Permissions.ReadRepository) permissions |= Permission.ReadRepository;
            if (newAccountData.Permissions.WriteRepository) permissions |= Permission.WriteRepository;
            if (newAccountData.Permissions.ManageRepository) permissions |= Permission.ManageRepository;

            if (newAccountData.Permissions.ReadSnippet) permissions |= Permission.ReadSnippet;
            if (newAccountData.Permissions.WriteSnippet) permissions |= Permission.WriteSnippet;
            if (newAccountData.Permissions.ManageSnippet) permissions |= Permission.ManageSnippet;

            if (newAccountData.Permissions.ReadAccount) permissions |= Permission.ReadAccount;
            if (newAccountData.Permissions.WriteAccount) permissions |= Permission.WriteAccount;
            if (newAccountData.Permissions.ManageAccount) permissions |= Permission.ManageAccount;

            if (newAccountData.Permissions.ReadTeam) permissions |= Permission.ReadTeam;
            if (newAccountData.Permissions.WriteTeam) permissions |= Permission.WriteTeam;
            if (newAccountData.Permissions.ManageTeam) permissions |= Permission.ManageTeam;

            string rawPassword = Password.RandomPassword(16);
            string encryptedPassword = await Password.GeneratePassword(rawPassword);

            var account = new Account(newAccountData.FirstName, newAccountData.LastName, newAccountData.Email, newAccountData.Username, encryptedPassword, permissions);

            await context.Accounts.AddAsync(account);
            await context.SaveChangesAsync();

            await httpContext.Response.WriteAsync(JsonConvert.SerializeObject(new {
                success = true,
                password = rawPassword
            }));
        }

        [HttpGet("/api/accounts/get", true)]
        public static async Task GetAccounts(HttpContext httpContext) {
            await using var context = new RgdContext();

            if (!httpContext.Session.GetAccountId(out long accountId)) throw new Exception("Failed to get accountId");
            var creatorPermissions = await (from a in context.Accounts where a.Id.Equals(accountId) select a.Permissions).FirstOrDefaultAsync();
            if ((creatorPermissions & Permission.ManageAccount) != Permission.ManageAccount) throw new HttpException(403, "No ManageAccount permission.");

            Account[] accountsRaw = await (from a in context.Accounts select a).ToArrayAsync();
            List <AccountView> accounts = accountsRaw.Select(account => new AccountView(account)).ToList();

            await httpContext.Response.WriteAsync(JsonConvert.SerializeObject(new { success = true, accounts }));
        }
    }
}
