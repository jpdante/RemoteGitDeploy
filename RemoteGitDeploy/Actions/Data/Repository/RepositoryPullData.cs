using System;

namespace RemoteGitDeploy.Actions.Data.Repository {
    public class RepositoryPullData : IActionData {
        public string Id { get; }
        public ActionDataType Type => ActionDataType.PullRepository;

        public readonly string RepositoryGuid;

        public RepositoryPullData(string repositoryGuid) {
            Id = Guid.NewGuid().ToString("N");
            RepositoryGuid = repositoryGuid;
        }
    }
}