#nullable enable

using System.Text.Json;
using MudSharp.GameItems.Components;

namespace MudSharp.Computers;

internal static class ComputerProcessWaitArguments
{
	private sealed class UserInputWaitPayload
	{
		public long CharacterId { get; set; }
		public long TerminalItemId { get; set; }
	}

	private sealed class SignalWaitPayload
	{
		public long SourceItemId { get; set; }
		public string SourceItemName { get; set; } = string.Empty;
		public long SourceComponentId { get; set; }
		public string SourceComponentName { get; set; } = string.Empty;
		public string SourceEndpointKey { get; set; } = string.Empty;
	}

	private sealed class MediaWaitPayload
	{
		public long ItemId { get; set; }
		public long ComponentId { get; set; }
		public string EndpointKey { get; set; } = string.Empty;
	}

	public static string CreateUserInput(long characterId, long terminalItemId)
	{
		return JsonSerializer.Serialize(new UserInputWaitPayload
		{
			CharacterId = characterId,
			TerminalItemId = terminalItemId
		});
	}

	public static bool TryParseUserInput(string? waitArgument, out long characterId, out long terminalItemId)
	{
		characterId = 0L;
		terminalItemId = 0L;
		if (string.IsNullOrWhiteSpace(waitArgument))
		{
			return false;
		}

		try
		{
			var payload = JsonSerializer.Deserialize<UserInputWaitPayload>(waitArgument);
			if (payload is null || payload.CharacterId <= 0L || payload.TerminalItemId <= 0L)
			{
				return false;
			}

			characterId = payload.CharacterId;
			terminalItemId = payload.TerminalItemId;
			return true;
		}
		catch (JsonException)
		{
			return false;
		}
	}

	public static string CreateSignal(LocalSignalBinding binding)
	{
		return JsonSerializer.Serialize(new SignalWaitPayload
		{
			SourceItemId = binding.SourceItemId,
			SourceItemName = binding.SourceItemName,
			SourceComponentId = binding.SourceComponentId,
			SourceComponentName = binding.SourceComponentName,
			SourceEndpointKey = binding.SourceEndpointKey
		});
	}

	public static bool TryParseSignal(string? waitArgument, out LocalSignalBinding binding)
	{
		binding = new LocalSignalBinding(0L, string.Empty, 0L, string.Empty,
			SignalComponentUtilities.DefaultLocalSignalEndpointKey);
		if (string.IsNullOrWhiteSpace(waitArgument))
		{
			return false;
		}

		try
		{
			var payload = JsonSerializer.Deserialize<SignalWaitPayload>(waitArgument);
			if (payload is null || payload.SourceComponentId <= 0L)
			{
				return false;
			}

			binding = new LocalSignalBinding(
				payload.SourceItemId,
				payload.SourceItemName ?? string.Empty,
				payload.SourceComponentId,
				payload.SourceComponentName ?? string.Empty,
				SignalComponentUtilities.NormaliseSignalEndpointKey(payload.SourceEndpointKey));
			return true;
		}
		catch (JsonException)
		{
			return false;
		}
	}

	public static string CreateMedia(MediaEndpointAddress endpoint)
	{
		return JsonSerializer.Serialize(new MediaWaitPayload
		{
			ItemId = endpoint.ItemId,
			ComponentId = endpoint.ComponentId,
			EndpointKey = endpoint.EndpointKey
		});
	}

	public static bool TryParseMedia(string? waitArgument, out MediaEndpointAddress endpoint)
	{
		endpoint = MediaEndpointAddress.Empty;
		if (string.IsNullOrWhiteSpace(waitArgument))
		{
			return false;
		}

		try
		{
			var payload = JsonSerializer.Deserialize<MediaWaitPayload>(waitArgument);
			if (payload is null || payload.ItemId <= 0L || payload.ComponentId <= 0L ||
			    string.IsNullOrWhiteSpace(payload.EndpointKey))
			{
				return false;
			}

			endpoint = new MediaEndpointAddress(payload.ItemId, payload.ComponentId, payload.EndpointKey,
				MediaEndpointDirection.Input);
			return true;
		}
		catch (JsonException)
		{
			return false;
		}
	}
}
