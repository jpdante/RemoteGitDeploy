using HtcSharp.HttpModule.Http.Features;
using StackExchange.Redis;

namespace RemoteGitDeploy.Extensions {
    public static class SessionExtension {

        public static bool GetAccountId(this ISession session, out long accountId) {
            if (session.TryGetValue("account", out var rawAccountId) && long.TryParse(((RedisValue) rawAccountId).ToString(), out accountId)) return true;
            accountId = -1;
            return false;
        }

    }
}