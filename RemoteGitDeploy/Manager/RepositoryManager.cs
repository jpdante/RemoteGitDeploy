using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Transactions;

namespace RemoteGitDeploy.Manager {
    public class RepositoryManager {

        internal readonly string GitUsername;
        internal readonly string GitPersonalAccessToken;
        internal readonly string GitRepositoriesDirectory;

        public RepositoryManager(string gitUsername, string gitPersonalAccessToken, string gitRepositoriesDirectory) {
            GitUsername = gitUsername;
            GitPersonalAccessToken = gitPersonalAccessToken;
            GitRepositoriesDirectory = gitRepositoriesDirectory;
        }

        public static bool BreakGitLink(string link, out string scheme, out string domain, out string path) {
            if (!Uri.TryCreate(link, UriKind.RelativeOrAbsolute, out var uri)) {
                scheme = null;
                domain = null;
                path = null;
                return false;
            }
            scheme = uri.Scheme;
            domain = uri.Host;
            path = uri.PathAndQuery;
            return true;
        }

    }
}