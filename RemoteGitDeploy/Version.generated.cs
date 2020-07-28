using System;
using System.Collections.Generic;
using System.Text;

namespace RemoteGitDeploy {
    public static class Version {

        public static readonly int Major = 1;
        public static readonly int Minor = 0;
        public static readonly int Patch = 0;
        public static readonly int Build = 195;

        public static string GetVersion() { return $"{Major}.{Minor}.{Patch} Build {Build}"; }

    }
}
