using System;
using System.Collections.Generic;
using System.Text;
using RemoteGitDeploy.Models.New;

namespace RemoteGitDeploy.Models.Views {
    public class TeamView {

        public long Id { get; set; }

        public string Guid { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public TeamView(Team team) {
            Id = team.Id;
            Guid = team.Guid;
            Name = team.Name;
            Description = team.Description;
        }
    }
}
