#nullable enable

using System;
using System.Text.Json;
using System.Text.Encodings.Web;
using System.Text.Json.Serialization;

namespace MudSharp.Discord;

/// <summary>A character-independent request; account identity is resolved by the engine from a verified link.</summary>
public sealed record DiscordRemoteRequest
{
	[JsonRequired]
	public Guid RequestId { get; init; } = Guid.NewGuid();
	public ulong DiscordUserId { get; init; }
	public ulong? GuildId { get; init; }
	public ulong ChannelId { get; init; }
	public string Operation { get; init; } = "";
	public string Arguments { get; init; } = "";
	public Guid? ConfirmationId { get; init; }
}

/// <summary>An attributed outcome, including a confirmation challenge or durable invocation identifier.</summary>
public sealed record DiscordRemoteResponse
{
	public Guid RequestId { get; init; }
	public string Status { get; init; } = "";
	public string Text { get; init; } = "";
	public Guid? InvocationId { get; init; }
	public Guid? ConfirmationId { get; init; }
}

/// <summary>Encodes the versioned remote payload without quote/newline ambiguity in the compatibility command router.</summary>
public static class DiscordRemoteWire
{
	private static readonly JsonSerializerOptions Options = new()
	{
		Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
	};
	public static string SerializeRequest(DiscordRemoteRequest request) => Encode(request);
	public static string SerializeResponse(DiscordRemoteResponse response)
	{
		var encoded = Encode(response);
		const int limit = DiscordBridgeProtocol.MaximumPayloadBytes - 7; // "remote "
		if (encoded.Length <= limit) { return encoded; }
		var low = 0;
		var high = response.Text.Length;
		while (low < high)
		{
			var middle = (low + high + 1) / 2;
			var candidate = Encode(response with { Text = response.Text[..middle] + "\n[Response truncated; narrow the query.]" });
			if (candidate.Length <= limit) { low = middle; }
			else { high = middle - 1; }
		}
		if (low > 0 && char.IsHighSurrogate(response.Text[low - 1])) { low--; }
		return Encode(response with { Text = response.Text[..low] + "\n[Response truncated; narrow the query.]" });
	}
	public static DiscordRemoteRequest DeserializeRequest(string payload) => Decode<DiscordRemoteRequest>(payload);
	public static DiscordRemoteResponse DeserializeResponse(string payload) => Decode<DiscordRemoteResponse>(payload);

	private static string Encode<T>(T value) => Convert.ToBase64String(JsonSerializer.SerializeToUtf8Bytes(value, Options));

	private static T Decode<T>(string payload)
	{
		if (payload.Length > DiscordBridgeProtocol.MaximumPayloadBytes)
		{
			throw new FormatException("Remote payload is too large.");
		}

		return JsonSerializer.Deserialize<T>(Convert.FromBase64String(payload)) ??
		       throw new FormatException("Remote payload is empty.");
	}
}
