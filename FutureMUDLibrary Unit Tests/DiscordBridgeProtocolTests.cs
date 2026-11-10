#nullable enable

using System;
using System.Buffers.Binary;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Discord;

namespace MudSharp_Unit_Tests;

[TestClass]
public class DiscordBridgeProtocolTests
{
	[TestMethod]
	public void RemoteWire_MissingInvocationIdentity_RejectsInsteadOfInventingAnotherMutation()
	{
		var payload = Convert.ToBase64String(Encoding.UTF8.GetBytes("{\"DiscordUserId\":42,\"ChannelId\":7,\"Operation\":\"hooks.run\"}"));
		Assert.ThrowsExactly<JsonException>(() => DiscordRemoteWire.DeserializeRequest(payload));
	}

	[TestMethod]
	public void VersionTwo_FragmentedCoalescedUnicode_RoundTripsWithoutDelimiterCollision()
	{
		const string message = "request ā Ċ \n \"quoted\" 😀";
		var frames = DiscordBridgeProtocol.EncodeFrame(message).Concat(DiscordBridgeProtocol.EncodeFrame("second")).ToArray();
		var decoder = new DiscordBridgeFrameDecoder(1);
		for (var i = 0; i < frames.Length - 1; i++)
		{
			decoder.Append(frames.AsSpan(i, 1));
			if (i < DiscordBridgeProtocol.EncodeFrame(message).Length - 1)
			{
				Assert.IsFalse(decoder.TryRead(out _));
			}
		}
		decoder.Append(frames.AsSpan(frames.Length - 1));
		Assert.IsTrue(decoder.TryRead(out var first));
		Assert.AreEqual(message, first);
		Assert.IsTrue(decoder.TryRead(out var second));
		Assert.AreEqual("second", second);
		Assert.IsFalse(decoder.TryRead(out _));
		Assert.IsTrue(decoder.IsVersion2);
	}

	[TestMethod]
	public void VersionTwo_InvalidLength_RejectsBeforeWaitingForPayload()
	{
		var frame = DiscordBridgeProtocol.EncodeFrame("");
		BinaryPrimitives.WriteInt32BigEndian(frame.AsSpan(4), DiscordBridgeProtocol.MaximumPayloadBytes + 1);
		var decoder = new DiscordBridgeFrameDecoder(1);
		decoder.Append(frame);
		Assert.ThrowsExactly<InvalidDataException>(() => decoder.TryRead(out _));
	}

	[TestMethod]
	public void VersionTwo_InvalidUtf8_RejectsInvalidText()
	{
		var frame = DiscordBridgeProtocol.EncodeFrame("x");
		frame[^1] = 0xff;
		var decoder = new DiscordBridgeFrameDecoder(1);
		decoder.Append(frame);
		Assert.ThrowsExactly<DecoderFallbackException>(() => decoder.TryRead(out _));
	}

	[TestMethod]
	public void VersionTwo_ProtocolSwitch_RejectsMixedFrames()
	{
		var decoder = new DiscordBridgeFrameDecoder(1);
		decoder.Append(DiscordBridgeProtocol.EncodeFrame("first"));
		Assert.IsTrue(decoder.TryRead(out _));
		decoder.Append(Encoding.Unicode.GetBytes("legacy message").Append((byte)1).ToArray());
		Assert.ThrowsExactly<InvalidDataException>(() => decoder.TryRead(out _));
	}

	[TestMethod]
	public void Legacy_ExistingAsciiNotification_RemainsReadable()
	{
		var decoder = new DiscordBridgeFrameDecoder(1);
		decoder.Append(Encoding.Unicode.GetBytes("broadcast hello").Append((byte)1).ToArray());
		Assert.IsTrue(decoder.TryRead(out var text));
		Assert.AreEqual("broadcast hello", text);
		Assert.IsFalse(decoder.IsVersion2);
	}

	[TestMethod]
	public void RemoteWire_EncodedLargeUnicodeResponse_FitsActualFrame()
	{
		var response = new DiscordRemoteResponse { RequestId = Guid.NewGuid(), Status = "completed",
			Text = new string('漢', 12000) + new string('\0', 12000) };
		var payload = DiscordRemoteWire.SerializeResponse(response);
		var frame = DiscordBridgeProtocol.EncodeFrame("remote " + payload);
		Assert.IsLessThanOrEqualTo(DiscordBridgeProtocol.MaximumPayloadBytes + DiscordBridgeProtocol.HeaderBytes, frame.Length);
		Assert.IsTrue(DiscordRemoteWire.DeserializeResponse(payload).Text.EndsWith("[Response truncated; narrow the query.]"));
	}

	[TestMethod]
	public void RemoteWire_IdentityQuotesNewlinesAndConfirmation_RoundTrip()
	{
		var request = new DiscordRemoteRequest
		{
			DiscordUserId = ulong.MaxValue, ChannelId = ulong.MaxValue, GuildId = 123,
			Operation = "hooks.run", Arguments = "{\"hook\":\"test\",\"arguments\":\"ā Ċ \\\"quoted\\\"\\n\"}",
			ConfirmationId = Guid.NewGuid()
		};
		Assert.AreEqual(request, DiscordRemoteWire.DeserializeRequest(DiscordRemoteWire.SerializeRequest(request)));
	}
}
