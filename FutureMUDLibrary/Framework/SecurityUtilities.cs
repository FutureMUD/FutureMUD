using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace MudSharp.Framework
{
    public static class SecurityUtilities
    {
        public static long GetSalt64()
        {
            byte[] saltBytes = new byte[8];
            Constants.CryptoRandom.GetBytes(saltBytes);
            long salt = BitConverter.ToInt64(saltBytes, 0);
            return salt;
        }

        public static string GetPasswordHash(string password, long salt)
        {
			ArgumentNullException.ThrowIfNull(password);
			return Encoding.UTF8.GetString(SHA384.HashData(Encoding.UTF8.GetBytes(password + salt)));
        }

        public static bool VerifyPassword(string password, string hash, long salt)
        {
			if (password is null || hash is null)
			{
				return false;
			}

			var actualBytes = Encoding.UTF8.GetBytes(GetPasswordHash(password, salt));
			var expectedBytes = Encoding.UTF8.GetBytes(hash);
			return actualBytes.Length == expectedBytes.Length &&
			       CryptographicOperations.FixedTimeEquals(actualBytes, expectedBytes);
        }

        private static IEnumerable<char> YieldRandomCharacters(int length, char[] characterSet)
        {
            while (length-- > 0)
            {
                yield return characterSet.PickRandom(1).First();
            }
        }

        public static string GetRandomString(int length, char[] characterSet)
        {
            return new string(YieldRandomCharacters(length, characterSet).ToArray());
        }
    }
}
