using IdGen;

namespace RemoteGitDeploy.Security {
    public static class IdGen {
        public static readonly IdGenerator IdGenerator = new IdGenerator(0);

        public static long GetId() => IdGenerator.CreateId();
        public static Id FromId(long id) => IdGenerator.FromId(id);
    }
}
