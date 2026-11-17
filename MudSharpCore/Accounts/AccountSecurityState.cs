#nullable enable
using System;
using System.Security.Cryptography;
using System.Text;
using MudSharp.Framework;
using DbAccount = MudSharp.Models.Account;

namespace MudSharp.Accounts;

internal sealed record AccountCredentialSnapshot(long Id, string Name, string Hash, long Salt, long Version)
{
	internal static AccountCredentialSnapshot From(DbAccount account) =>
		new(account.Id, account.Name, account.Password, account.Salt, account.CredentialVersion);
	internal bool Matches(DbAccount account) => Id == account.Id && Version == account.CredentialVersion &&
		Salt == account.Salt && string.Equals(Hash, account.Password, StringComparison.Ordinal);
}

internal sealed record AccountPasswordPermission(long AccountId, long Version, DateTime ExpiresUtc, string? RecoveryDigest = null);

/// <summary>Pure credential transitions. Persistence uses concurrency tokens to commit each transition once.</summary>
internal static class AccountSecurityState
{
	internal static readonly TimeSpan RecoveryLifetime = TimeSpan.FromMinutes(30);
	internal static readonly TimeSpan RecoveryCooldown = TimeSpan.FromMinutes(1);
	internal const int MaximumRecoveryAttempts = 5;

	internal static bool ReserveLogin(DbAccount account, DateTime utcNow)
	{
		if (account.LoginBlockedUntilUtc > utcNow) return false;
		if (account.LoginAttemptWindowUtc is null || utcNow - account.LoginAttemptWindowUtc >= TimeSpan.FromMinutes(5))
		{
			account.LoginAttempts = 0;
			account.LoginAttemptWindowUtc = utcNow;
		}
		if (++account.LoginAttempts <= 10) return true;
		account.LoginBlockedUntilUtc = utcNow.AddMinutes(1);
		account.LoginAttempts = 0;
		account.LoginAttemptWindowUtc = utcNow;
		return false;
	}

	internal static string? IssueRecovery(DbAccount account, DateTime utcNow)
	{
		if (account.RecoveryIssuedUtc > utcNow - RecoveryCooldown) return null;
		var code = SecurityUtilities.GetRandomString(24, Constants.ValidRandomCharacters.ToCharArray());
		account.RecoveryCode = RecoveryDigest(code);
		account.RecoveryIssuedUtc = utcNow;
		account.RecoveryExpiresUtc = utcNow + RecoveryLifetime;
		account.RecoveryCredentialVersion = account.CredentialVersion;
		account.RecoveryAttempts = 0;
		return code;
	}

	internal static string RecoveryDigest(string code) => "v1$" + Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(code)));

	internal static bool RecoveryMatches(DbAccount account, string digest, DateTime utcNow) =>
		account.RecoveryExpiresUtc > utcNow && account.RecoveryCredentialVersion == account.CredentialVersion &&
		account.RecoveryAttempts <= MaximumRecoveryAttempts && account.RecoveryCode is { } expected &&
		expected.StartsWith("v1$", StringComparison.Ordinal) && digest.Length == expected.Length &&
		CryptographicOperations.FixedTimeEquals(Encoding.UTF8.GetBytes(digest), Encoding.UTF8.GetBytes(expected));

	internal static AccountPasswordPermission? CheckRecovery(DbAccount account, string code, DateTime utcNow)
	{
		if (account.RecoveryAttempts >= MaximumRecoveryAttempts) return null;
		account.RecoveryAttempts++;
		var digest = RecoveryDigest(code);
		if (RecoveryMatches(account, digest, utcNow))
			return new(account.Id, account.CredentialVersion, account.RecoveryExpiresUtc!.Value, digest);
		if (account.RecoveryAttempts >= MaximumRecoveryAttempts) account.RecoveryCode = null;
		return null;
	}

	internal static bool ReplacePassword(DbAccount account, AccountPasswordPermission permission, string hash, DateTime utcNow)
	{
		if (account.Id != permission.AccountId || account.CredentialVersion != permission.Version || permission.ExpiresUtc <= utcNow ||
		    (permission.RecoveryDigest is not null && !RecoveryMatches(account, permission.RecoveryDigest, utcNow)) ||
		    !Argon2AccountCredentialHasher.TryParse(hash, out _, out _, out _, out _)) return false;
		account.Password = hash;
		account.Salt = 0;
		account.CredentialVersion = checked(account.CredentialVersion + 1);
		account.RecoveryCode = null;
		account.RecoveryExpiresUtc = null;
		account.RecoveryCredentialVersion = null;
		account.RecoveryAttempts = 0;
		account.LoginAttempts = 0;
		account.LoginBlockedUntilUtc = null;
		return true;
	}
}
