using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HtcSharp.Core.Logging.Abstractions;
using HtcSharp.HttpModule.Http.Abstractions;
using HtcSharp.HttpModule.Http.Abstractions.Extensions;
using HtcSharp.HttpModule.Logging;
using HtcSharp.HttpModule.Routing;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using RemoteGitDeploy.Core;
using RemoteGitDeploy.Extensions;
using RemoteGitDeploy.Models.New;
using RemoteGitDeploy.Models.RequestData;
using RemoteGitDeploy.Models.Views;
using RemoteGitDeploy.Mvc;

namespace RemoteGitDeploy.Controllers {
    public class ActionController {

        [HttpGet("/api/actions/get", true)]
        public static async Task GetActions(HttpContext httpContext) {
            await using var context = new RgdContext();

            if (!httpContext.Session.GetAccountId(out long accountId)) throw new Exception("Failed to get accountId");
            var creatorPermissions = await (from a in context.Accounts where a.Id.Equals(accountId) select a.Permissions).FirstOrDefaultAsync();
            if ((creatorPermissions & Permission.ManageRepository) != Permission.ManageRepository) throw new HttpException(403, "No ManageRepository permission.");

            ActionHistory[] actionHistoryRaw = await (from ah in context.ActionHistory orderby ah.Id descending select ah).Take(10).ToArrayAsync();

            List<ActionHistoryCompactView> history = actionHistoryRaw.Select(actionHistory => new ActionHistoryCompactView(actionHistory)).ToList();

            foreach (var repository in HtcPlugin.RepositoryManager.GetInternalRepositories()) {
                if(repository.CurrentAction != null) history.Insert(0, repository.CurrentAction.GetFutureActionHistoryCompactView());
                foreach (var action in repository.QueuedActions) {
                    history.Insert(0, action.GetFutureActionHistoryCompactView());
                }
            }

            await httpContext.Response.WriteAsync(JsonConvert.SerializeObject(new { success = true, history }));
        }

        [HttpPost("/api/actions/repository/get", ContentType.JSON, true)]
        public static async Task GetRepositoryActions(HttpContext httpContext, RepositoryData repositoryData) {
            await using var context = new RgdContext();

            if (!httpContext.Session.GetAccountId(out long accountId)) throw new Exception("Failed to get accountId");
            var creatorPermissions = await (from a in context.Accounts where a.Id.Equals(accountId) select a.Permissions).FirstOrDefaultAsync();
            if ((creatorPermissions & Permission.ManageRepository) != Permission.ManageRepository) throw new HttpException(403, "No ManageRepository permission.");

            if (HtcPlugin.RepositoryManager.TryGetRepository(repositoryData.Guid, out var repository)) {
                ActionHistory[] actionHistoryRaw = await (from ah in context.ActionHistory orderby ah.Id descending where ah.RepositoryId == repository.Id select ah).Take(10).ToArrayAsync();

                List<ActionHistoryView> history = actionHistoryRaw.Select(actionHistory => new ActionHistoryView(actionHistory)).ToList();

                if (HtcPlugin.RepositoryManager.TryGetInternalRepository(repositoryData.Guid, out var internalRepository)) {
                    if (internalRepository.CurrentAction != null) history.Insert(0, new ActionHistoryView(internalRepository.CurrentAction.GetFutureActionHistoryCompactView()));
                    foreach (var action in internalRepository.QueuedActions) {
                        history.Insert(0, new ActionHistoryView(action.GetFutureActionHistoryCompactView()));
                    }
                }

                await httpContext.Response.WriteAsync(JsonConvert.SerializeObject(new { success = true, history }));
            } else {
                await httpContext.Response.SendErrorAsync(404, "Unknown repository.");
            }
        }
    }
}
