#nullable enable

using System;
using System.Reflection;
using DatabaseSeeder;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MudSharp_Unit_Tests;

[TestClass]
public class ItemManifestCredentialSecurityTests
{
	[DataTestMethod]
	[DataRow("--capture-item-manifest", "Server=example;uid=admin;password=example-secret;")]
	[DataRow("--CAPTURE-ITEM-MANIFEST", "secret-without-connection-syntax")]
	[DataRow("--capture-item-manifest", "")]
	public void Capture_PositionalCredential_RejectsBeforeConnectingWithoutEchoingValue(string flag, string supplied)
	{
		var method = typeof(Program).GetMethod("TryCaptureItemManifest", BindingFlags.Static | BindingFlags.NonPublic)!;
		var exception = Assert.ThrowsException<TargetInvocationException>(() => method.Invoke(null,
			new object[] { new[] { flag, supplied } }));

		Assert.IsInstanceOfType(exception.InnerException, typeof(InvalidOperationException));
		StringAssert.Contains(exception.InnerException!.Message, "FUTUREMUD_ITEM_MANIFEST_CONNECTION_STRING");
		if (supplied.Length > 0)
		{
			Assert.IsFalse(exception.InnerException.Message.Contains(supplied, StringComparison.Ordinal));
		}
	}
}
