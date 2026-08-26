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
}
