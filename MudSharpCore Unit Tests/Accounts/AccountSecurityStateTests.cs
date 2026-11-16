#nullable enable
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Accounts;
using MudSharp.Framework;
using DbAccount = MudSharp.Models.Account;
using MudSharp.Network;

namespace MudSharp_Unit_Tests.Accounts;

/// <summary>Pure transitions and injected verifiers: no EF models, database, native KDF work or sleeps.</summary>
[TestClass]
public class AccountSecurityStateTests
{
	private static readonly DateTime Now = new(2026, 11, 16, 4, 0, 0, DateTimeKind.Utc);
	private static DbAccount Account() => new() { Id = 71, Name = "owner", Password = "legacy", Salt = 3 };
	[TestMethod]
	[DataRow(true, "synthetic password")]
	[DataRow(false, "setpassword owner accidentally-inline-secret")]
	[DataRow(false, "resetpassword owner accidentally-inline-secret")]
	[DataRow(false, "setpassword\towner\taccidentally-inline-secret")]
	public void SecurityInput_IsRedactedFromFailureContext(bool sensitive, string input)
	{
		var recorded = PlayerConnection.AccountSafeFailureInput(sensitive, input);
		Assert.AreEqual("[REDACTED ACCOUNT SECURITY INPUT]", recorded);
		Assert.AreEqual("look", PlayerConnection.AccountSafeFailureInput(false, "look"));
	}

	[TestMethod]
	public void Recovery_PreUpgradeHashWithoutExpiry_IsNeverAccepted()
	{
		var account = Account();
		account.RecoveryCode = SecurityUtilities.GetPasswordHash("old code", account.Salt);
		Assert.IsNull(AccountSecurityState.CheckRecovery(account, "old code", Now));
	}

	[TestMethod]
	public void Recovery_ResetIsSingleUse_AndPreservesManualRegistrationState()
	{
		var account = Account();
		account.IsRegistered = true;
		var code = AccountSecurityState.IssueRecovery(account, Now)!;
		Assert.AreNotEqual(code, account.RecoveryCode);
		var permission = AccountSecurityState.CheckRecovery(account, code, Now.AddMinutes(1));
		Assert.IsNotNull(permission);
		Assert.AreEqual("legacy", account.Password, "Validating the code must not change a credential or authenticate an account.");
		Assert.IsTrue(AccountSecurityState.ReplacePassword(account, permission, Argon2AccountCredentialHasher.DummyHash, Now.AddMinutes(2)));
		Assert.IsFalse(AccountSecurityState.ReplacePassword(account, permission, Argon2AccountCredentialHasher.DummyHash, Now.AddMinutes(2)));
		Assert.AreEqual(1L, account.CredentialVersion);
		Assert.IsNull(account.RecoveryCode);
		Assert.IsTrue(account.IsRegistered);
	}

	[TestMethod]
	public void Recovery_ExpiryIsRecheckedAtCommit_NotOnlyAtCodeEntry()
	{
		var account = Account();
		var code = AccountSecurityState.IssueRecovery(account, Now)!;
		var permission = AccountSecurityState.CheckRecovery(account, code, Now.AddMinutes(29))!;
		Assert.IsFalse(AccountSecurityState.ReplacePassword(account, permission, Argon2AccountCredentialHasher.DummyHash, Now.AddMinutes(30)));
		Assert.AreEqual("legacy", account.Password);
	}

	[TestMethod]
	public void Recovery_NewerCodeInvalidatesAnAlreadyVerifiedPendingReset()
	{
		var account = Account();
		var oldCode = AccountSecurityState.IssueRecovery(account, Now)!;
		var oldPermission = AccountSecurityState.CheckRecovery(account, oldCode, Now)!;
		Assert.IsNull(AccountSecurityState.IssueRecovery(account, Now.AddSeconds(59)));
		var newCode = AccountSecurityState.IssueRecovery(account, Now.AddMinutes(1))!;
		Assert.IsFalse(AccountSecurityState.ReplacePassword(account, oldPermission, Argon2AccountCredentialHasher.DummyHash, Now.AddMinutes(2)));
		Assert.IsNotNull(AccountSecurityState.CheckRecovery(account, newCode, Now.AddMinutes(2)));
	}

	[TestMethod]
	public void Recovery_AttemptBudgetCannotBeResetByASecondSession()
	{
		var account = Account();
		var code = AccountSecurityState.IssueRecovery(account, Now)!;
		for (var i = 0; i < AccountSecurityState.MaximumRecoveryAttempts; i++)
			Assert.IsNull(AccountSecurityState.CheckRecovery(account, "wrong", Now));
		Assert.IsNull(AccountSecurityState.CheckRecovery(account, code, Now));
		Assert.IsNull(account.RecoveryCode);
	}

	[TestMethod]
	public void Password_ImplementorReplacementNeedsNoEmail_AndInvalidatesOldPermissions()
	{
		var account = Account();
		account.Email = string.Empty;
		var permission = new AccountPasswordPermission(account.Id, 0, Now.AddMinutes(5));
		Assert.IsTrue(AccountSecurityState.ReplacePassword(account, permission, Argon2AccountCredentialHasher.DummyHash, Now));
		Assert.IsFalse(AccountSecurityState.ReplacePassword(account, permission, Argon2AccountCredentialHasher.DummyHash, Now));
		Assert.AreEqual(string.Empty, account.Email);
		Assert.IsFalse(account.IsRegistered, "Changing a password must not secretly register an account.");
	}

	[TestMethod]
	public void Login_AttemptsPersistOnTheAccount_AndBackoffExpires()
	{
		var account = Account();
		for (var i = 0; i < 10; i++) Assert.IsTrue(AccountSecurityState.ReserveLogin(account, Now));
		Assert.IsFalse(AccountSecurityState.ReserveLogin(account, Now));
		Assert.IsFalse(AccountSecurityState.ReserveLogin(account, Now.AddSeconds(59)));
		Assert.IsTrue(AccountSecurityState.ReserveLogin(account, Now.AddMinutes(1)));
	}

	[TestMethod]
	public void LegacyVerification_UpgradesOnlyTheRealPassword_WithoutApplyingNewSelectionRules()
	{
		var hasher = new FakeHasher();
		var snapshot = new AccountCredentialSnapshot(71, "owner", SecurityUtilities.GetPasswordHash("short", 3), 3, 0);
		var valid = AccountSecurityService.VerifyCredential(snapshot, "short", hasher);
		Assert.IsTrue(valid.Valid);
		Assert.AreEqual("modern verifier", valid.UpgradedHash);
		Assert.AreEqual(1, hasher.Created);
		Assert.IsFalse(AccountSecurityService.VerifyCredential(snapshot, "support impersonation", hasher).Valid);
		Assert.AreEqual(1, hasher.Created);
		Assert.AreEqual(1, hasher.Verified);
	}

	[TestMethod]
	public void LegacyVerification_DoesNotApplyTheNewPasswordLengthCap()
	{
		var password = new string('x', 5000);
		var snapshot = new AccountCredentialSnapshot(71, "owner", SecurityUtilities.GetPasswordHash(password, 3), 3, 0);
		Assert.IsFalse(AccountPasswordPolicy.TryValidate(password, out _));
		Assert.IsTrue(AccountSecurityService.VerifyCredential(snapshot, password, new FakeHasher()).Valid);
	}

	[TestMethod]
	public void ModernVerification_CannotFallBackToTheLegacyHash()
	{
		var hasher = new FakeHasher();
		var snapshot = new AccountCredentialSnapshot(71, "owner", "$argon2id$malformed", 0, 0);
		Assert.IsFalse(AccountSecurityService.VerifyCredential(snapshot, "anything", hasher).Valid);
		Assert.AreEqual(0, hasher.Created);
		Assert.AreEqual(1, hasher.Verified);
	}

	[TestMethod]
	[DataRow("$argon2id$v=19$m=2147483647,t=2,p=1$AAAA$AAAA")]
	[DataRow("$argon2id$v=99$m=19456,t=2,p=1$AAAA$AAAA")]
	[DataRow("$argon2id$v=19$m=19456,t=99,p=1$AAAA$AAAA")]
	[DataRow("$argon2id$v=19$m=19456,t=2,p=99$AAAA$AAAA")]
	[DataRow("$argon2id$v=19$m=19456,t=2,p=1$invalid$invalid")]
	public void CorruptVerifier_IsRefusedBeforeAnyKdfAllocation(string encoded)
	{
		Assert.IsFalse(Argon2AccountCredentialHasher.TryParse(encoded, out _, out _, out _, out _));
	}

	private sealed class FakeHasher : IAccountCredentialHasher
	{
		internal int Created;
		internal int Verified;
		public string Hash(string password) { Created++; return "modern verifier"; }
		public bool Verify(string password, string encodedHash) { Verified++; return false; }
	}
}
