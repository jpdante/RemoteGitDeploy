using System;
using System.Collections.Generic;

namespace RemoteGitDeploy.Models {
    public class Repository {

        public long Id { get; }
        public string Guid { get; }
        public string Name { get; }
        public string Git { get; }
        public string Branch { get; }
        public List<Action> Actions { get; }

        public string LastCommit { get; set; }
        public DateTime LastUpdate { get; set; }

        public Repository(long id, string guid, string name, string git, string branch) {
            Id = id;
            Guid = guid;
            Name = name;
            Git = git;
            Branch = branch;
            Actions = new List<Action>();
        }

    }
}
