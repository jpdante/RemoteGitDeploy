using System;

namespace RemoteGitDeploy.Actions.Data.Repository {
    public class RepositoryActionData : IActionData {
        public string Id { get; }
        public ActionDataType Type => ActionDataType.RunAction;

        public readonly string RepositoryGuid;

        public RepositoryActionData(string repositoryGuid) {
            Id = Guid.NewGuid().ToString("N");
            RepositoryGuid = repositoryGuid;
        }
    }
}