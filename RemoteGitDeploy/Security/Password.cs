using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Konscious.Security.Cryptography;

namespace RemoteGitDeploy.Security {
    public class Password {

        public const int SaltSize = 24;
        public const int DegreeOfParallelism = 1;
        public const int Iterations = 1;
        public const int MemorySize = 4 * 1024;

        public static async Task<string> GeneratePassword(string password) {
            byte[] salt = GenerateSalt();
            byte[] hashedPassword = await Argon2IdHashPassword(password, salt);
            return $"argon2d;{Convert.ToBase64String(salt)};{Convert.ToBase64String(hashedPassword)}";
        }

        public static async Task<bool> CheckPassword(string rawHashedPassword, string password) {
            string[] data = rawHashedPassword.Split(";", 3);
            if (!data[0].Equals("argon2d")) return false;
            byte[] salt = Convert.FromBase64String(data[1]);
            byte[] hashedPassword = Convert.FromBase64String(data[2]);
            byte[] newHash = await Argon2IdHashPassword(password, salt);
            return hashedPassword.SequenceEqual(newHash);
        }

        public static byte[] GenerateSalt() {
            var buffer = new byte[SaltSize];
            var rng = new RNGCryptoServiceProvider();
            rng.GetBytes(buffer);
            return buffer;
        }

        public static async Task<byte[]> Argon2IdHashPassword(string password, byte[] salt) {
            var argon2 = new Argon2d(Encoding.UTF8.GetBytes(password)) {
                Salt = salt,
                DegreeOfParallelism = DegreeOfParallelism,
                Iterations = Iterations,
                MemorySize = MemorySize
            };
            return await argon2.GetBytesAsync(16);
        }

        private static readonly Random Random = new Random();

        public static string RandomPassword(int length) {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789@!#$%&*/\\-+";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[Random.Next(s.Length)]).ToArray());
        }
    }
}