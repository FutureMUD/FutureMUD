#nullable enable

using Discord_Bot;
using Discord_Bot.Modules;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using MudSharp.Discord;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Concurrent;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MudSharp_Unit_Tests;

[TestClass]
public class DiscordBotProtocolTests
{
	[TestMethod]
	public void BotInboundDecoder_LegacyEngineDelimiter_PreservesNotificationCompatibility()
	{
		var decoder = TcpConnection.CreateInboundDecoder();
		decoder.Append(Encoding.Unicode.GetBytes("broadcast hello").Append((byte)1).ToArray());
		Assert.IsTrue(decoder.TryRead(out var message));
		Assert.AreEqual("broadcast hello", message);
		Assert.IsFalse(decoder.IsVersion2);
	}

	[TestMethod]
	public void RegistrationRequest_OwnershipProofWindow_DoesNotExpireAfterOrdinaryRequestTimeout()
	{
		var now = DateTime.UtcNow;
		var request = new CachedDiscordRequest { CreatedUtc = now, Timeout = TimeSpan.FromMinutes(3) };
		Assert.IsFalse(request.IsExpired(now.AddMinutes(2)));
		Assert.IsTrue(request.IsExpired(now.AddMinutes(3)));
	}

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

	[TestMethod]
	[DataRow("request 42 who")]
	[DataRow("remote AQID")]
	[DataRow("accountnotification 42 AQID")]
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

	[TestMethod]
	[DataRow("secret-value", "secret-value", true)]
	[DataRow("Secret-Value", "secret-value", false)]
	public void AuthenticationPolicy_RequiresAnExactConfiguredSecret(string presented, string configured,
		bool expected)
	{
		Assert.AreEqual(expected, DiscordAuthenticationPolicy.IsValid(presented, configured));
	}

	[TestMethod]
	[DataRow("{\"Port\":4000,\"Token\":\"token\",\"Prefixes\":[\"!\"],\"ServerAuth\":\"secret\",\"AnnounceChannelId\":1,\"AdminAnnounceChannelId\":2,\"DebugAnnounceChannelId\":3,\"GameName\":\"Game\",\"AdminUsers\":[],\"CustomGlobalReactions\":[]}", true)]
	[DataRow("{not json", false)]
	public void SettingsStore_ValidatesConfigurationWithoutThrowing(string json, bool expected)
	{
		var result = DiscordBotSettingsStore.TryLoadSettings(json, out var settings);

		Assert.AreEqual(expected, result);
		if (expected)
		{
			Assert.AreEqual(4000, settings.Port);
			Assert.AreEqual("127.0.0.1", settings.ListenAddress);
		}
	}

	[TestMethod]
	public void SettingsStore_AllInterfacesCanBeExplicitlyConfigured()
	{
		const string json = "{\"Port\":4000,\"ListenAddress\":\"0.0.0.0\",\"Token\":\"token\",\"Prefixes\":[\"!\"],\"ServerAuth\":\"secret\",\"AnnounceChannelId\":1,\"AdminAnnounceChannelId\":2,\"DebugAnnounceChannelId\":3,\"GameName\":\"Game\",\"AdminUsers\":[],\"CustomGlobalReactions\":[]}";
		Assert.IsTrue(DiscordBotSettingsStore.TryLoadSettings(json, out var settings));
		Assert.AreEqual("0.0.0.0", settings.ListenAddress);
	}

	[TestMethod]
	public void RemoteCommandPolicy_DeauthoriseRequiresOwnerAndReadsCanonicalUserField()
	{
		Assert.IsTrue(DiscordRemoteCommandPolicy.RequiresOwner("admin.deauthorise"));
		Assert.IsFalse(DiscordRemoteCommandPolicy.RequiresOwner("admin.applications"));
		Assert.IsTrue(DiscordRemoteCommandPolicy.IsOwner(7, [7], [7, 8]));
		Assert.IsFalse(DiscordRemoteCommandPolicy.IsOwner(8, [7], [7, 8]));
		Assert.IsTrue(DiscordRemoteCommandPolicy.IsOwner(7, [], [7, 8]));
		Assert.IsFalse(DiscordRemoteCommandPolicy.IsOwner(8, [], [7, 8]));
		Assert.IsTrue(DiscordRemoteCommandPolicy.TryGetDeauthorisedDiscordUserId("{\"user\":\"123456789012345678\"}", out var userId));
		Assert.AreEqual(123456789012345678UL, userId);
		Assert.IsTrue(DiscordRemoteCommandPolicy.TryGetDeauthorisedDiscordUserId("{\"discordUserId\":\"42\"}", out userId));
		Assert.AreEqual(42UL, userId);
		Assert.IsFalse(DiscordRemoteCommandPolicy.TryGetDeauthorisedDiscordUserId("{\"user\":\"not-a-snowflake\"}", out _));
	}

	[TestMethod]
	public void LegacyRequestCache_ExpiresEntriesAfterBoundedLifetime()
	{
		var request = new CachedDiscordRequest { CreatedUtc = DateTime.UtcNow.AddSeconds(-46) };

		Assert.IsTrue(request.IsExpired(DateTime.UtcNow));
		Assert.IsFalse(request.IsExpired(request.CreatedUtc.AddSeconds(1)));
	}

	[TestMethod]
	public async Task LegacyRequestCache_RejectsRequestsAboveCapacity()
	{
		var cache = new CachedDiscordRequestCache();
		for (var i = 0; i < 256; i++)
		{
			Assert.IsTrue(await cache.TryAddRequestAsync(new CachedDiscordRequest
			{
				Context = null!,
				OnResponseAction = null!
			}));
		}

		Assert.IsFalse(await cache.TryAddRequestAsync(new CachedDiscordRequest
		{
			Context = null!,
			OnResponseAction = null!
		}));
		Assert.AreEqual(256, cache.Count);
	}

	[TestMethod]
	public void ConfirmationPrompt_TruncatesAtLimitAndNeutralisesMentions()
	{
		var prompt = DiscordResponseText.Truncate("@everyone " + new string('x', 2100), 1800);

		Assert.IsTrue(prompt.Length <= 1800);
		Assert.IsFalse(prompt.Contains("@everyone", StringComparison.OrdinalIgnoreCase));
		Assert.IsTrue(prompt.EndsWith("…", StringComparison.Ordinal));
	}

	[TestMethod]
	public void SlashCommandOptions_UseTypesSupportedByDSharpPlus453AndValidDefaults()
	{
		var slashMethods = typeof(GameCommands).Assembly.GetTypes()
			.Where(type => typeof(ApplicationCommandModule).IsAssignableFrom(type))
			.SelectMany(type => type.GetMethods())
			.Where(method => method.GetCustomAttributes(typeof(SlashCommandAttribute), false).Length > 0)
			.ToArray();
		Assert.IsTrue(slashMethods.Length > 0, "At least one slash command should be discovered.");

		foreach (var method in slashMethods)
		{
			Assert.IsTrue(typeof(Task).IsAssignableFrom(method.ReturnType), $"{method.Name} must return Task.");
			var parameters = method.GetParameters();
			Assert.IsTrue(parameters.Length > 0 && parameters[0].ParameterType == typeof(InteractionContext),
				$"{method.Name} must start with InteractionContext.");
			foreach (var parameter in parameters.Skip(1))
			{
				Assert.IsNotNull(parameter.GetCustomAttributes(typeof(OptionAttribute), false).SingleOrDefault(),
					$"{method.Name}.{parameter.Name} must declare OptionAttribute.");
				Assert.IsTrue(IsSupportedDSharpPlus453OptionType(parameter.ParameterType),
					$"DSharpPlus 4.5.3 cannot register {method.Name}.{parameter.Name} with type {parameter.ParameterType}.");
				Assert.AreEqual(parameter.IsOptional, parameter.HasDefaultValue,
					$"{method.Name}.{parameter.Name} must have a default exactly when it is optional.");
			}
		}
	}

	private static bool IsSupportedDSharpPlus453OptionType(Type type)
	{
		if (type == typeof(string) || type == typeof(long) || type == typeof(bool) || type == typeof(double))
		{
			return true;
		}

		if (Nullable.GetUnderlyingType(type) is { } nullableType)
		{
			return nullableType == typeof(long) || nullableType == typeof(bool) || nullableType == typeof(double) ||
			       nullableType == typeof(TimeSpan) || nullableType.IsEnum;
		}

		return type == typeof(DiscordChannel) || type == typeof(DiscordUser) || type == typeof(DiscordRole) ||
		       type == typeof(DiscordEmoji) || type == typeof(DiscordAttachment) || type == typeof(SnowflakeObject) ||
		       type.IsEnum;
	}

	[TestMethod]
	public async Task AtomicFileWriter_SerializesConcurrentWholeFileReplacements()
	{
		var directory = Path.Combine(Path.GetTempPath(), $"discord-bot-{Guid.NewGuid():N}");
		Directory.CreateDirectory(directory);
		try
		{
			var path = Path.Combine(directory, "settings.json");
			var writer = new DiscordBotAtomicFileWriter();
			var payloads = Enumerable.Range(0, 32).Select(i => $"settings version {i:D2} {new string('x', 256)}").ToArray();
			await Task.WhenAll(payloads.Select(payload => writer.WriteAsync(path, payload)));

			var persisted = await File.ReadAllTextAsync(path);
			Assert.IsTrue(payloads.Contains(persisted));
			Assert.AreEqual(1, Directory.GetFiles(directory).Length);
		}
		finally
		{
			Directory.Delete(directory, recursive: true);
		}
	}

	[TestMethod]
	public async Task AtomicFileWriter_FailedWriteLeavesExistingFileUntouchedAndReleasesGate()
	{
		var directory = Path.Combine(Path.GetTempPath(), $"discord-bot-{Guid.NewGuid():N}");
		Directory.CreateDirectory(directory);
		try
		{
			var blockingFile = Path.Combine(directory, "not-a-directory");
			await File.WriteAllTextAsync(blockingFile, "preserve me");
			var writer = new DiscordBotAtomicFileWriter();
			await Assert.ThrowsExactlyAsync<DirectoryNotFoundException>(() =>
				writer.WriteAsync(Path.Combine(blockingFile, "settings.json"), "replacement"));
			Assert.AreEqual("preserve me", await File.ReadAllTextAsync(blockingFile));

			var validPath = Path.Combine(directory, "settings.json");
			await writer.WriteAsync(validPath, "valid after failure");
			Assert.AreEqual("valid after failure", await File.ReadAllTextAsync(validPath));
		}
		finally
		{
			Directory.Delete(directory, recursive: true);
		}
	}
	private static byte[] Frame(string payload)
	{
		return Encoding.Unicode.GetBytes(payload)
			.Append(DiscordTcpFrameDecoder.FrameDelimiter)
			.ToArray();
	}

	[TestMethod]
	public void VersionTwoFrameDecoder_HandlesFragmentationCoalescingAndUnicode()
	{
		var decoder = new DiscordBridgeFrameDecoder(DiscordBotProtocol.EngineCommandDelimiter);
		var first = DiscordBridgeProtocol.EncodeFrame("remote ΩāĊ \"quoted\"");
		var second = DiscordBridgeProtocol.EncodeFrame("remote second");
		decoder.Append(first.AsSpan(0, 5));
		Assert.IsFalse(decoder.TryRead(out _));
		decoder.Append(first.AsSpan(5).ToArray().Concat(second).ToArray());

		Assert.IsTrue(decoder.TryRead(out var firstMessage));
		Assert.AreEqual("remote ΩāĊ \"quoted\"", firstMessage);
		Assert.IsTrue(decoder.IsVersion2);
		Assert.IsTrue(decoder.TryRead(out var secondMessage));
		Assert.AreEqual("remote second", secondMessage);
	}

	[TestMethod]
	public void RemoteWire_RoundTripsRequestAndResponseWithoutDelimiterAmbiguity()
	{
		var request = new DiscordRemoteRequest
		{
			DiscordUserId = 123456789,
			GuildId = 987654321,
			ChannelId = 987654322,
			Operation = "hooks.run",
			Arguments = "{\"hook\":\"safe\",\"arguments\":\"line one\\nline two\"}"
		};
		var requestPayload = DiscordRemoteWire.SerializeRequest(request);
		var requestRoundTrip = DiscordRemoteWire.DeserializeRequest(requestPayload);
		Assert.AreEqual(request.RequestId, requestRoundTrip.RequestId);
		Assert.AreEqual(request.Arguments, requestRoundTrip.Arguments);
		Assert.AreEqual(request.DiscordUserId, requestRoundTrip.DiscordUserId);

		var response = new DiscordRemoteResponse
		{
			RequestId = request.RequestId,
			Status = "confirmation",
			ConfirmationId = Guid.NewGuid(),
			Text = "Quoted \"text\" with @everyone and Ω"
		};
		var responseRoundTrip = DiscordRemoteWire.DeserializeResponse(DiscordRemoteWire.SerializeResponse(response));
		Assert.AreEqual(response.RequestId, responseRoundTrip.RequestId);
		Assert.AreEqual(response.ConfirmationId, responseRoundTrip.ConfirmationId);
		Assert.AreEqual(response.Text, responseRoundTrip.Text);
	}

	[TestMethod]
	public void DiscordResponseText_SplitsLongContentAndNeutralisesMentions()
	{
		var neutral = DiscordResponseText.NeutraliseMentions("@everyone <@123> @here");
		Assert.IsFalse(neutral.Contains("@everyone", StringComparison.OrdinalIgnoreCase));
		Assert.IsFalse(neutral.Contains("<@123>", StringComparison.Ordinal));
		Assert.IsFalse(neutral.Contains("@here", StringComparison.OrdinalIgnoreCase));

		var chunks = DiscordResponseText.Split(new string('x', 4000));
		Assert.AreEqual(3, chunks.Count);
		Assert.IsTrue(chunks.All(x => x.Length <= 1850));
	}
}
