using System;
using System.Collections.Generic;
using System.Text;

namespace RemoteGitDeploy.Core {
    [Flags]
    public enum Permission : int {

        None = 0,

        ReadRepository = 1,
        WriteRepository = 2,
        ManageRepository = 4,

        ReadSnippet = 8,
        WriteSnippet = 16,
        ManageSnippet = 32,

        ReadAccount = 64,
        WriteAccount = 128,
        ManageAccount = 256,

        ReadTeam = 512,
        WriteTeam = 1024,
        ManageTeam = 2048,

        All = ReadRepository + WriteRepository + ManageRepository + ReadSnippet + WriteSnippet + ManageSnippet + ReadAccount + WriteAccount + ManageAccount + ReadTeam + WriteTeam + ManageTeam

    }
}
