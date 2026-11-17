using System;
using System.Security.Cryptography;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Framework;

namespace MudSharp_Unit_Tests;

[TestClass]
public class SecurityUtilitiesTests
{
	[TestMethod]
	public void PasswordHashRemainsCompatibleWithLegacySha384Representation()
	{
		const string password = "known legacy password";
		const long salt = 123456789;
		const string expectedUtf8Base64 =
			"77+9XVrvv73vv73jl7Dvv73vv709H++/ve+/vTnPlO+/vU1M77+977+977+977+9NSfvv73vv73vv73vv73vv73vv71OAyJa77+977+977+977+9EVfvv70N77+977+9Mw==";
		var legacyBytes = SHA384.HashData(Encoding.UTF8.GetBytes(password + salt));
		var legacyHash = Encoding.UTF8.GetString(legacyBytes);

		Assert.AreEqual(expectedUtf8Base64, Convert.ToBase64String(Encoding.UTF8.GetBytes(legacyHash)));
		Assert.AreEqual(legacyHash, SecurityUtilities.GetPasswordHash(password, salt));
		Assert.IsTrue(SecurityUtilities.VerifyPassword(password, legacyHash, salt));
		Assert.IsFalse(SecurityUtilities.VerifyPassword("incorrect", legacyHash, salt));
	}

	[TestMethod]
	[DataRow(null, false)]
	[DataRow("", false)]
	[DataRow("1234567", false)]
	[DataRow("12345678", false)]
	[DataRow("a sufficiently long password", true)]
	[DataRow("a longer password", true)]
	public void NewPasswordPolicy_UsesTheSharedSelectionRules(string password, bool expected)
	{
		Assert.AreEqual(expected, SecurityUtilities.IsValidNewPassword(password));
	}

	[TestMethod]
	public void ExistingShortPassword_StillVerifiesAfterPolicyChange()
	{
		var hash = SecurityUtilities.GetPasswordHash("old", 123);
		Assert.IsFalse(SecurityUtilities.IsValidNewPassword("old"));
		Assert.IsTrue(SecurityUtilities.VerifyPassword("old", hash, 123));
	}

	[TestMethod]
	[DataRow(8, "abcdefghijklmnopqrstuvwxyz")]
	[DataRow(12, "ABCabc123")]
	[DataRow(20, "abcdefghijklmnopqrstuvwxyz")]
	public void GeneratedCredentials_KeepRequestedLengthAndAlphabetWithoutUsingWorldRandom(int length, string alphabet)
	{
		using var randomScope = Constants.PushRandom(new ForbiddenWorldRandom());
		var value = SecurityUtilities.GetRandomString(length, alphabet.ToCharArray());
		Assert.AreEqual(length, value.Length);
		foreach (var character in value)
		{
			Assert.IsTrue(alphabet.Contains(character));
		}
	}

	[TestMethod]
	public void GeneratedCredentials_ValidateArgumentsAndSupportEmptyOrSingleCharacterResults()
	{
		Assert.AreEqual(string.Empty, SecurityUtilities.GetRandomString(0, Array.Empty<char>()));
		Assert.AreEqual("xxx", SecurityUtilities.GetRandomString(3, ['x']));
		Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => SecurityUtilities.GetRandomString(-1, ['x']));
		Assert.ThrowsExactly<ArgumentNullException>(() => SecurityUtilities.GetRandomString(1, null));
		Assert.ThrowsExactly<ArgumentException>(() => SecurityUtilities.GetRandomString(1, Array.Empty<char>()));
	}

	private sealed class ForbiddenWorldRandom : Random
	{
		public override int Next(int maxValue) =>
			throw new InvalidOperationException("Authentication credentials must not use the simulation PRNG.");
	}
}
