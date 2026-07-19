#nullable enable

using System.Security.Cryptography;
using System.Text;

namespace Discord_Bot;

internal static class DiscordAuthenticationPolicy
{
	public static bool IsValid(string? presentedSecret, string? configuredSecret)
	{
		if (string.IsNullOrEmpty(presentedSecret) || string.IsNullOrEmpty(configuredSecret))
		{
			return false;
		}

		return CryptographicOperations.FixedTimeEquals(
			Encoding.UTF8.GetBytes(presentedSecret),
			Encoding.UTF8.GetBytes(configuredSecret));
	}
}
