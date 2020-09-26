using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
using RemoteGitDeploy.Mvc;

namespace RemoteGitDeploy.Controllers {
    public class TeamController {

        [HttpPost("/api/team/new", ContentType.JSON, true)]
        public static async Task NewTeam(HttpContext httpContext, NewTeamData newTeamData) {
            await using var context = new RgdContext();

            if (!httpContext.Session.GetAccountId(out long accountId)) throw new Exception("Failed to get accountId");
            var creatorPermissions = await (from a in context.Accounts where a.Id.Equals(accountId) select a.Permissions).FirstOrDefaultAsync();
            if ((creatorPermissions & Permission.WriteTeam) != Permission.WriteTeam) throw new HttpException(403, "No WriteTeam permission.");

            bool hasTeamWithName = await (from t in context.Teams where t.Name.Equals(newTeamData.Name) select t).AnyAsync();
            if (hasTeamWithName) {
                await httpContext.Response.SendRequestErrorAsync(9, "An account with that username already exists.");
            }

            var team = new Team(accountId, newTeamData.Name, newTeamData.Description);

            await context.Teams.AddAsync(team);
            await context.SaveChangesAsync();

            await httpContext.Response.WriteAsync(JsonConvert.SerializeObject(new {
                success = true,
            }));
        }

        [HttpGet("/api/teams/get", true)]
        public static async Task GetTeams(HttpContext httpContext) {
            await using var context = new RgdContext();

            if (!httpContext.Session.GetAccountId(out long accountId)) throw new Exception("Failed to get accountId");
            var creatorPermissions = await (from a in context.Accounts where a.Id.Equals(accountId) select a.Permissions).FirstOrDefaultAsync();
            if ((creatorPermissions & Permission.ManageTeam) != Permission.ManageTeam) throw new HttpException(403, "No ManageTeam permission.");

            Team[] teams = await (from t in context.Teams select t).ToArrayAsync();

            await httpContext.Response.WriteAsync(JsonConvert.SerializeObject(new {
                success = true,
                teams
            }));
        }
    }
}
