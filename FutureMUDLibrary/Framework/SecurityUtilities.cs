using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace MudSharp.Framework {
    public static class SecurityUtilities {
        public static readonly HashAlgorithm HashAlgorithm = SHA384.Create();

        public static long GetSalt64() {
            var saltBytes = new byte[8];
            Constants.CryptoRandom.GetBytes(saltBytes);
            var salt = BitConverter.ToInt64(saltBytes, 0);
            return salt;
        }

        public static string GetPasswordHash(string password, long salt) {
            return Encoding.UTF8.GetString(HashAlgorithm.ComputeHash(Encoding.UTF8.GetBytes(password + salt)));
        }

        public static bool VerifyPassword(string password, string hash, long salt) {
            return GetPasswordHash(password, salt).Equals(hash);
        }

        private static IEnumerable<char> YieldRandomCharacters(int length, char[] characterSet) {
            while (length-- > 0) {
                yield return characterSet.PickRandom(1).First();
            }
        }

        public static string GetRandomString(int length, char[] characterSet) {
            return new string(YieldRandomCharacters(length, characterSet).ToArray());
        }
    }
}