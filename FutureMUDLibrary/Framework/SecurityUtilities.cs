#nullable enable

using System;
using System.Security.Cryptography;
using System.Text;

namespace MudSharp.Framework
{
    public static class SecurityUtilities
    {
		/// <summary>The minimum character count for a newly selected account password.</summary>
		public const int MinimumPasswordLength = 8;

		/// <summary>Checks the shared password-selection policy without affecting existing password verification.</summary>
		public static bool IsValidNewPassword(string? password)
		{
			return password is { Length: >= MinimumPasswordLength };
		}

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

        public static bool VerifyPassword(string? password, string? hash, long salt)
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

		/// <summary>Creates a cryptographically random string of the requested length from the supplied alphabet.</summary>
		/// <exception cref="ArgumentOutOfRangeException">The length is negative.</exception>
		/// <exception cref="ArgumentException">A nonempty result is requested with an empty alphabet.</exception>
		public static string GetRandomString(int length, char[] characterSet)
		{
			ArgumentOutOfRangeException.ThrowIfNegative(length);
			ArgumentNullException.ThrowIfNull(characterSet);
			if (length > 0 && characterSet.Length == 0)
			{
				throw new ArgumentException("A nonempty random string requires at least one character.", nameof(characterSet));
			}

			return string.Create(length, characterSet, static (result, alphabet) =>
			{
				for (var i = 0; i < result.Length; i++)
				{
					result[i] = alphabet[RandomNumberGenerator.GetInt32(alphabet.Length)];
				}
			});
		}
    }
}
