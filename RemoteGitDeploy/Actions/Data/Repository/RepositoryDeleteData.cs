using System;

namespace RemoteGitDeploy.Actions.Data.Repository {
    public class RepositoryDeleteData : IActionData {
        public string Id { get; }
        public ActionDataType Type => ActionDataType.DeleteRepository;

        public readonly string RepositoryGuid;

        public RepositoryDeleteData(string repositoryGuid) {
            Id = Guid.NewGuid().ToString("N");
            RepositoryGuid = repositoryGuid;
        }
    }
}