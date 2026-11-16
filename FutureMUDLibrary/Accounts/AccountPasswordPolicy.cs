#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MudSharp.Accounts;

/// <summary>Offline password-selection rules. Existing passwords are verified without applying these rules.</summary>
public static class AccountPasswordPolicy
{
	public const int MinimumLength = 15;
	public const int MaximumLength = 128;
	private static readonly HashSet<string> CommonPasswords = new(StringComparer.OrdinalIgnoreCase)
	{
		"passwordpassword", "password123456789", "password12345678", "123456789012345",
		"1234567890123456", "12345678901234567890", "qwertyuiopasdfgh", "qwerty1234567890",
		"letmeinletmeinletmein", "iloveyouiloveyou", "correct horse battery staple",
		"aaaaaaaaaaaaaaa", "aaaaaaaaaaaaaaaa", "111111111111111", "1111111111111111"
	};

	public static bool TryValidate(string? password, out string reason)
	{
		var length = password?.EnumerateRunes().Count() ?? 0;
		if (length < MinimumLength || length > MaximumLength)
		{
			reason = $"Passwords must be at least {MinimumLength} characters and no more than {MaximumLength} characters long.";
			return false;
		}
		if (CommonPasswords.Contains(password!))
		{
			reason = "That password is too common. Choose a different password or a longer, uncommon passphrase.";
			return false;
		}
		reason = string.Empty;
		return true;
	}
}
