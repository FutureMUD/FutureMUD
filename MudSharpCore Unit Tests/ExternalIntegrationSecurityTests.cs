#nullable enable
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Accounts;
using MudSharp.Discord;
using MudSharp.Framework;
using System;

namespace MudSharp_Unit_Tests;

[TestClass]
public class ExternalIntegrationSecurityTests
{
	[TestMethod]
	public void SanitiseDiscordAdminAlert_NeutralisesMentionsAndCodeFences()
	{
		string result = ExternalIntegrationAlertHelper.SanitiseDiscordAdminAlert(
			"hello @everyone @here <@123>\n```secret```", 200);

		StringAssert.Contains(result, "@\u200beveryone");
		StringAssert.Contains(result, "@\u200bhere");
		StringAssert.Contains(result, "<@\u200b123>");
		Assert.IsFalse(result.Contains("```", StringComparison.Ordinal));
	}

	[TestMethod]
	public void BuildSafeGptErrorAlert_DoesNotIncludeRawExceptionMessage()
	{
		InvalidOperationException exception = new("api-key=secret-token\nstack details");

		string result = ExternalIntegrationAlertHelper.BuildSafeGptErrorAlert(exception, "GPT test context");

		StringAssert.Contains(result, "GPT test context");
		StringAssert.Contains(result, nameof(InvalidOperationException));
		Assert.IsFalse(result.Contains("secret-token", StringComparison.Ordinal));
		Assert.IsFalse(result.Contains("stack details", StringComparison.Ordinal));
	}

	[TestMethod]
	public void DiscordRequesterCanUseShowCommands_RequiresJuniorAdminAuthority()
	{
		Assert.IsFalse(DiscordConnection.DiscordRequesterCanUseShowCommands(AccountWithAuthority(PermissionLevel.Player)));
		Assert.IsTrue(DiscordConnection.DiscordRequesterCanUseShowCommands(AccountWithAuthority(PermissionLevel.JuniorAdmin)));
	}

	private static IAccount AccountWithAuthority(PermissionLevel level)
	{
		Mock<IAuthority> authority = new();
		authority.SetupGet(x => x.Level).Returns(level);
		Mock<IAccount> account = new();
		account.SetupGet(x => x.Authority).Returns(authority.Object);
		return account.Object;
	}
}
