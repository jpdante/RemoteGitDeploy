using System.Linq;
using System.Security.Cryptography;

namespace RemoteGitDeploy.Security {
    public static class PasswordSecurity {

        public const int SaltSize = 24;
        public const int HashSize = 24;
        public const int Iterations = 100000;

        public static byte[] CreateSalt() {
            var buffer = new byte[SaltSize];
            var rng = new RNGCryptoServiceProvider();
            rng.GetBytes(buffer);
            return buffer;
        }

        public static byte[] HashPasswordAsync(string password, byte[] salt) {
            /*var argon2 = new Argon2id(Encoding.UTF8.GetBytes(password)) {
                Salt = salt,
                DegreeOfParallelism = 8,
                Iterations = 4,
                MemorySize = 1024 * 1024
            };
            return await argon2.GetBytesAsync(16);*/
            var pbkdf2 = new Rfc2898DeriveBytes(password, salt, Iterations);
            return pbkdf2.GetBytes(HashSize);
        }

        public static bool CompareHashAsync(string password, byte[] hash, byte[] salt) {
            byte[] newHash = HashPasswordAsync(password, salt);
            return hash.SequenceEqual(newHash);
        }

    }
}