#nullable enable

using Discord_Bot;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Concurrent;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MudSharp_Unit_Tests;

[TestClass]
public class DiscordBotProtocolTests
{
	[TestMethod]
	public void PrepareCommandForEngine_LineBreaks_EscapesEverySupportedForm()
	{
		Assert.AreEqual(
			@"broadcast first\nsecond\nthird\nfourth",
			DiscordBotProtocol.PrepareCommandForEngine("broadcast first\r\nsecond\nthird\rfourth"));
	}

	[TestMethod]
	public void EncodeCommandForEngine_UnicodePayload_AppendsRawDelimiter()
	{
		var encoded = DiscordBotProtocol.EncodeCommandForEngine("broadcast Καλημέρα");
		var expectedPayload = Encoding.Unicode.GetBytes("broadcast Καλημέρα");

		Assert.AreEqual(DiscordBotProtocol.EngineCommandDelimiter, encoded[^1]);
		CollectionAssert.AreEqual(expectedPayload, encoded[..^1]);
	}

	[TestMethod]
	public void ParseShutdownNotification_LegacyPayload_DefaultsToReboot()
	{
		var result = DiscordBotProtocol.ParseShutdownNotification("AdminUser");

		Assert.AreEqual("AdminUser", result.ShutdownAccount);
		Assert.IsTrue(result.Reboot);
	}

	[TestMethod]
	public void ParseShutdownNotification_QuotedAccountAndStopFlag_ReadsBothValues()
	{
		var result = DiscordBotProtocol.ParseShutdownNotification("\"Admin User\" false");

		Assert.AreEqual("Admin User", result.ShutdownAccount);
		Assert.IsFalse(result.Reboot);
	}

	[TestMethod]
	public void ParseShutdownNotification_InvalidFlag_PreservesLegacyRebootDefault()
	{
		Assert.IsTrue(DiscordBotProtocol.ParseShutdownNotification("Admin maybe").Reboot);
	}

	[TestMethod]
	public void FrameDecoder_FragmentedUnicodeFrame_WaitsForDelimiterAndReassemblesPayload()
	{
		var decoder = new DiscordTcpFrameDecoder();
		var payload = Encoding.Unicode.GetBytes("broadcast Привет");

		decoder.Append(payload.AsSpan(0, 7));
		Assert.AreEqual(DiscordTcpFrameReadResult.NoFrame, decoder.TryRead(out _));

		decoder.Append(payload.AsSpan(7));
		decoder.Append([DiscordTcpFrameDecoder.FrameDelimiter]);
		Assert.AreEqual(DiscordTcpFrameReadResult.Frame, decoder.TryRead(out var frame));
		Assert.AreEqual("broadcast Привет", Encoding.Unicode.GetString(frame));
	}

	[TestMethod]
	public void FrameDecoder_MultipleFramesInOneRead_ReturnsThemInOrder()
	{
		var decoder = new DiscordTcpFrameDecoder();
		var first = Frame("login secret");
		var second = Frame("shutdown Admin false");
		decoder.Append(first.Concat(second).ToArray());

		Assert.AreEqual(DiscordTcpFrameReadResult.Frame, decoder.TryRead(out var firstFrame));
		Assert.AreEqual("login secret", Encoding.Unicode.GetString(firstFrame));
		Assert.AreEqual(DiscordTcpFrameReadResult.Frame, decoder.TryRead(out var secondFrame));
		Assert.AreEqual("shutdown Admin false", Encoding.Unicode.GetString(secondFrame));
		Assert.AreEqual(DiscordTcpFrameReadResult.NoFrame, decoder.TryRead(out _));
	}

	[TestMethod]
	public void FrameDecoder_IncompleteOversizedFrame_ReportsOverflowAndClearsBuffer()
	{
		var decoder = new DiscordTcpFrameDecoder();
		decoder.Append(Enumerable.Repeat((byte)2, DiscordTcpFrameDecoder.MaximumFrameBytes + 1).ToArray());

		Assert.AreEqual(DiscordTcpFrameReadResult.Overflow, decoder.TryRead(out var frame));
		Assert.AreEqual(0, frame.Length);
		Assert.AreEqual(DiscordTcpFrameReadResult.NoFrame, decoder.TryRead(out _));
	}

	[TestMethod]
	public void FrameDecoder_DelimitedOversizedFrame_ReportsOverflowAndClearsBuffer()
	{
		var decoder = new DiscordTcpFrameDecoder();
		decoder.Append(Enumerable
			.Repeat((byte)2, DiscordTcpFrameDecoder.MaximumFrameBytes + 1)
			.Append(DiscordTcpFrameDecoder.FrameDelimiter)
			.ToArray());

		Assert.AreEqual(DiscordTcpFrameReadResult.Overflow, decoder.TryRead(out _));
		Assert.AreEqual(DiscordTcpFrameReadResult.NoFrame, decoder.TryRead(out _));
	}

	[TestMethod]
	public void CommandRouter_Login_DoesNotRequirePriorAuthentication()
	{
		Assert.IsTrue(DiscordTcpCommandRouter.TryParse("login secret 1.2.3", out var command));
		Assert.AreEqual("login", command.Name);
		Assert.AreEqual("secret 1.2.3", command.Payload);
		Assert.IsFalse(command.RequiresAuthentication);
	}

	[DataTestMethod]
	[DataRow("request 42 who")]
	[DataRow("shutdown Admin false")]
	[DataRow("broadcast hello")]
	[DataRow("progerror 10 test error")]
	public void CommandRouter_ServerCommands_RequireAuthentication(string input)
	{
		Assert.IsTrue(DiscordTcpCommandRouter.TryParse(input, out var command));
		Assert.IsTrue(command.RequiresAuthentication);
	}

	[TestMethod]
	public void CommandRouter_UnknownOrEmptyCommand_IsRejected()
	{
		Assert.IsFalse(DiscordTcpCommandRouter.TryParse("unknown payload", out _));
		Assert.IsFalse(DiscordTcpCommandRouter.TryParse(string.Empty, out _));
	}

	[TestMethod]
	public void DetailedUserSetting_SaveAndParse_IsCultureInvariant()
	{
		var originalCulture = CultureInfo.CurrentCulture;
		try
		{
			CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("fr-FR");
			var saved = new DetailedUserSetting(12345678901234567890UL, "Example Account", -42).SaveToConfig();
			var parsed = new DetailedUserSetting(saved);

			Assert.AreEqual("12345678901234567890,Example Account,-42", saved);
			Assert.AreEqual(12345678901234567890UL, parsed.DiscordUserId);
			Assert.AreEqual("Example Account", parsed.MudAccountName);
			Assert.AreEqual(-42L, parsed.MudAccountId);
		}
		finally
		{
			CultureInfo.CurrentCulture = originalCulture;
		}
	}

	[TestMethod]
	public void SettingsStore_MalformedLines_AreSkippedWithoutDiscardingValidLinks()
	{
		var result = DiscordBotSettingsStore.LoadAccountLinks([
			"1,First,10",
			"not-an-id,Bad,20",
			"2,Second,30",
			"missing-fields",
			""
		]);

		Assert.AreEqual(2, result.Settings.Count);
		Assert.AreEqual(2, result.InvalidLineCount);
		CollectionAssert.AreEqual(new[] { "First", "Second" }, result.Settings.Select(x => x.MudAccountName).ToArray());
	}

	[TestMethod]
	public void CachedRequestIds_ConcurrentCreation_ProducesUniqueIds()
	{
		var ids = new ConcurrentBag<ulong>();
		Parallel.For(0, 1_000, _ => ids.Add(new CachedDiscordRequest().RequestId));

		Assert.AreEqual(1_000, ids.Distinct().Count());
	}

	[DataTestMethod]
	[DataRow("secret-value", "secret-value", true)]
	[DataRow("Secret-Value", "secret-value", false)]
	public void AuthenticationPolicy_RequiresAnExactConfiguredSecret(string presented, string configured,
		bool expected)
	{
		Assert.AreEqual(expected, DiscordAuthenticationPolicy.IsValid(presented, configured));
	}

	[DataTestMethod]
	[DataRow("{\"Port\":4000,\"Token\":\"token\",\"Prefixes\":[\"!\"],\"ServerAuth\":\"secret\",\"AnnounceChannelId\":1,\"AdminAnnounceChannelId\":2,\"DebugAnnounceChannelId\":3,\"GameName\":\"Game\",\"AdminUsers\":[],\"CustomGlobalReactions\":[]}", true)]
	[DataRow("{not json", false)]
	public void SettingsStore_ValidatesConfigurationWithoutThrowing(string json, bool expected)
	{
		var result = DiscordBotSettingsStore.TryLoadSettings(json, out var settings);

		Assert.AreEqual(expected, result);
		if (expected)
		{
			Assert.AreEqual(4000, settings.Port);
		}
	}
	private static byte[] Frame(string payload)
	{
		return Encoding.Unicode.GetBytes(payload)
			.Append(DiscordTcpFrameDecoder.FrameDelimiter)
			.ToArray();
	}
}
