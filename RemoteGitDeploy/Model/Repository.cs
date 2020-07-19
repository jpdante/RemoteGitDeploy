using System;
using System.Collections.Generic;

namespace RemoteGitDeploy.Model {
    public class Repository {

        public long Id { get; }
        public string Guid { get; }
        public string Name { get; }
        public string Git { get; }
        public List<Action> Actions { get; }

        public string LastCommit { get; set; }
        public DateTime LastUpdate { get; set; }

        public Repository(long id, string guid, string name, string git) {
            Id = id;
            Guid = guid;
            Name = name;
            Git = git;
            Actions = new List<Action>();
        }

    }
}
