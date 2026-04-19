#nullable enable

using System.Text.Json;
using MudSharp.GameItems.Components;
using MudSharp.GameItems.Interfaces;

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
}
