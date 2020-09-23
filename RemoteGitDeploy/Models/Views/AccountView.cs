using System.Globalization;
using RemoteGitDeploy.Core;
using RemoteGitDeploy.Models.New;

namespace RemoteGitDeploy.Models.Views {
    public class AccountView {

        public long Id { get; set; }

        public string Guid { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

        public string Username { get; set; }

        public Permission Permissions { get; set; }

        public string LastAccess { get; set; }

        public AccountView(Account account) {
            Id = account.Id;
            Guid = account.Guid;
            FirstName = account.FirstName;
            LastName = account.LastName;
            Email = account.Email;
            Username = account.Username;
            Permissions = account.Permissions;
            LastAccess = account.LastAccess.ToString(CultureInfo.InvariantCulture); ;
        }

    }
}
