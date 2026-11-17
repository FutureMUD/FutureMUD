#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using MudSharp.Database;
using MudSharp.Email;
using MudSharp.Framework;
using DbAccount = MudSharp.Models.Account;

namespace MudSharp.Accounts;

internal sealed record AccountCredentialCheck(bool Valid, string? UpgradedHash = null);

internal static class AccountSecurityService
{
	internal static readonly IAccountCredentialHasher Hasher = new Argon2AccountCredentialHasher();
	private sealed record SourceWindow(DateTime Started, int Attempts);
	private static readonly Dictionary<string, SourceWindow> Sources = new(StringComparer.Ordinal);

	// Called on the game thread. Bounded and independent of account existence; shared NATs get a wider budget.
	internal static bool AllowSource(string source, string operation, DateTime utcNow)
	{
		var key = operation + ":" + source;
		if (Sources.Count >= 4096)
		{
			foreach (var old in Sources.Where(x => utcNow - x.Value.Started >= TimeSpan.FromMinutes(5)).Select(x => x.Key).ToList())
				Sources.Remove(old);
			if (Sources.Count >= 4096 && !Sources.ContainsKey(key)) return false;
		}
		if (!Sources.TryGetValue(key, out var window) || utcNow - window.Started >= TimeSpan.FromMinutes(5))
			window = new(utcNow, 0);
		Sources[key] = window with { Attempts = window.Attempts + 1 };
		return window.Attempts < (operation == "login" ? 100 : 20);
	}

	internal static AccountCredentialSnapshot? ReadCredentials(string name, bool reserveLogin = false)
	{
		using (new FMDB())
		{
			var row = FMDB.Context.Accounts.FirstOrDefault(x => x.Name == name.ToLowerInvariant());
			if (row is null) return null;
			if (reserveLogin)
			{
				var allowed = AccountSecurityState.ReserveLogin(row, DateTime.UtcNow);
				FMDB.Context.SaveChanges();
				if (!allowed) return null;
			}
			return AccountCredentialSnapshot.From(row);
		}
	}

	internal static AccountCredentialSnapshot? ReadCredentials(long id, bool reserveLogin = false)
	{
		using (new FMDB())
		{
			var row = FMDB.Context.Accounts.Find(id);
			if (row is not null && reserveLogin)
			{
				var allowed = AccountSecurityState.ReserveLogin(row, DateTime.UtcNow);
				FMDB.Context.SaveChanges();
				if (!allowed) return null;
			}
			return row is null ? null : AccountCredentialSnapshot.From(row);
		}
	}

	// CPU only: may execute on bounded workers. No FMDB, world, controller or email access.
	internal static AccountCredentialCheck VerifyCredential(AccountCredentialSnapshot? snapshot, string password,
		IAccountCredentialHasher? hasher = null)
	{
		hasher ??= Hasher;
		if (password.Length > Constants.PlayerConnectionBufferSize) return new(false);
		if (snapshot is not null && snapshot.Hash.StartsWith(Argon2AccountCredentialHasher.Prefix, StringComparison.Ordinal))
			return new(hasher.Verify(password, snapshot.Hash));
		if (snapshot is not null && SecurityUtilities.VerifyPassword(password, snapshot.Hash, snapshot.Salt))
			return new(true, hasher.Hash(password));
		hasher.Verify(password, Argon2AccountCredentialHasher.DummyHash);
		return new(false);
	}

	internal static AccountPasswordPermission? ValidateRecovery(string name, string code)
	{
		using (new FMDB())
		{
			var row = FMDB.Context.Accounts.FirstOrDefault(x => x.Name == name.ToLowerInvariant());
			if (row is null) return null;
			var permission = AccountSecurityState.CheckRecovery(row, code, DateTime.UtcNow);
			FMDB.Context.SaveChanges();
			return permission;
		}
	}

	internal static bool SendRecovery(string nameOrEmail, string source)
	{
		if (!AllowSource(source, "recovery", DateTime.UtcNow) || !EmailHelper.Instance.CanDeliverAccountEmail) return false;
		using (new FMDB())
		{
			var row = FMDB.Context.Accounts.FirstOrDefault(x => x.Name == nameOrEmail.ToLowerInvariant() || x.Email == nameOrEmail);
			if (row is null || string.IsNullOrWhiteSpace(row.Email)) return false;
			var code = AccountSecurityState.IssueRecovery(row, DateTime.UtcNow);
			if (code is null) return false;
			FMDB.Context.SaveChanges();
			EmailHelper.Instance.SendEmail(EmailTemplateTypes.AccountRecoveryCode, row.Email, row.Name.Proper(), source, code);
			return true;
		}
	}

	internal static string? IssueStaffRecovery(long accountId)
	{
		using (new FMDB())
		{
			var row = FMDB.Context.Accounts.Find(accountId);
			if (row is null) return null;
			var code = AccountSecurityState.IssueRecovery(row, DateTime.UtcNow);
			if (code is not null) FMDB.Context.SaveChanges();
			return code;
		}
	}

	internal static bool ReplacePassword(IFuturemud world, AccountPasswordPermission permission, string hash,
		IAccountController? keepSession, string source, string reason)
	{
		string name;
		try
		{
			using (new FMDB())
			{
				var row = FMDB.Context.Accounts.Find(permission.AccountId);
				if (row is null || !AccountSecurityState.ReplacePassword(row, permission, hash, DateTime.UtcNow)) return false;
				FMDB.Context.SaveChanges();
				name = row.Name;
			}
		}
		catch (DbUpdateConcurrencyException) { return false; }
		foreach (var connection in world.Connections.ToList())
		{
			var controller = connection.ControlPuppet;
			if (controller?.Account?.Id != permission.AccountId || ReferenceEquals(controller, keepSession)) continue;
			controller.OutputHandler.Send("Your account credentials were changed. Please reconnect and log in again.");
			controller.Close();
		}
		world.SystemMessage($"Account {name.Proper()} credentials changed ({reason}) from {source}.", true);
		using (new FMDB())
		{
			var email = FMDB.Context.Accounts.Where(x => x.Id == permission.AccountId).Select(x => x.Email).FirstOrDefault();
			if (!string.IsNullOrWhiteSpace(email)) EmailHelper.Instance.SendPasswordChangedEmail(email, name.Proper(), source);
		}
		return true;
	}
}
