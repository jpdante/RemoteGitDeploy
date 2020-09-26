using System.Globalization;
using RemoteGitDeploy.Core;
using RemoteGitDeploy.Models.New;

namespace RemoteGitDeploy.Models.Views {
    public class RepositoryView {

        public long Id { get; set; }

        public string Guid { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string Git { get; set; }

        public string Branch { get; set; }

        public TeamView Team { get; set; }

        public string CreationDate { get; set; }

        public RepositoryView(Repository repository, TeamView team) {
            Id = repository.Id;
            Guid = repository.Guid;
            Name = repository.Name;
            Description = repository.Description;
            Git = repository.Git;
            Branch = repository.Branch;
            CreationDate = repository.CreationDate.ToString(CultureInfo.InvariantCulture);
            Team = team;
        }

    }
}
