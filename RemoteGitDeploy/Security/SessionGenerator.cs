using System;
using System.Collections.Generic;
using System.Security.Cryptography;

namespace RemoteGitDeploy.Security {
    public static class SessionGenerator {
        internal const int EncodingBitsPerChar = 5;
        internal const int IdLengthBits = 120;
        internal const int IdLengthBytes = (IdLengthBits / 8);                        // 15
        internal const int IdLengthChars = (IdLengthBits / EncodingBitsPerChar);    // 24

        private static readonly char[] StringEncoding = {
            'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm',
            'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z',
            '0', '1', '2', '3', '4', '5'
        };

        private static readonly bool[] StringLegalChars;

        static SessionGenerator() {
            int i;

            StringLegalChars = new bool[128];
            for (i = StringEncoding.Length - 1; i >= 0; i--) {
                char ch = StringEncoding[i];
                StringLegalChars[ch] = true;
            }
        }

        public static bool IsLegit(string s) {
            if (s == null || s.Length != IdLengthChars) return false;
            try {
                int i = IdLengthChars;
                while (--i >= 0) {
                    char ch = s[i];
                    if (!StringLegalChars[ch])
                        return false;
                }
                return true;
            } catch (IndexOutOfRangeException) {
                return false;
            }
        }

        private static string Encode(IReadOnlyList<byte> buffer) {
            int i;
            var chars = new char[IdLengthChars];

            var j = 0;
            for (i = 0; i < IdLengthBytes; i += 5) {
                int n = (int)buffer[i] |
                        ((int)buffer[i + 1] << 8) |
                        ((int)buffer[i + 2] << 16) |
                        ((int)buffer[i + 3] << 24);

                int k = (n & 0x0000001F);
                chars[j++] = StringEncoding[k];

                k = ((n >> 5) & 0x0000001F);
                chars[j++] = StringEncoding[k];

                k = ((n >> 10) & 0x0000001F);
                chars[j++] = StringEncoding[k];

                k = ((n >> 15) & 0x0000001F);
                chars[j++] = StringEncoding[k];

                k = ((n >> 20) & 0x0000001F);
                chars[j++] = StringEncoding[k];

                k = ((n >> 25) & 0x0000001F);
                chars[j++] = StringEncoding[k];

                n = ((n >> 30) & 0x00000003) | ((int)buffer[i + 4] << 2);

                k = (n & 0x0000001F);
                chars[j++] = StringEncoding[k];

                k = ((n >> 5) & 0x0000001F);
                chars[j++] = StringEncoding[k];
            }

            return new string(chars);
        }

        public static string Create(RandomNumberGenerator randomNumberGenerator = null) {
            randomNumberGenerator ??= new RNGCryptoServiceProvider();
            var buffer = new byte[15];
            randomNumberGenerator.GetBytes(buffer);
            string encoding = Encode(buffer);
            return encoding;
        }
    }
}
