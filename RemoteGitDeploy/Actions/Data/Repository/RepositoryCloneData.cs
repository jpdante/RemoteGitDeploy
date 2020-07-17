﻿using System;

namespace RemoteGitDeploy.Actions.Data.Repository {
    public class RepositoryCloneData : IActionData {
        public string Id { get; }
        public ActionDataType Type => ActionDataType.NewRepository;

        public readonly long Account;
        public readonly string Git;
        public readonly string Name;
        public readonly string Description;
        public readonly long Team;

        public RepositoryCloneData(long account, string git, string name, string description, long team) {
            Id = Guid.NewGuid().ToString("N");
            Account = account;
            Git = git;
            Name = name;
            Description = description;
            Team = team;
        }
    }
}