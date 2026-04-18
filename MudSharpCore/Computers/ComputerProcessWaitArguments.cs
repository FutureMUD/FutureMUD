#nullable enable

using System.Text.Json;

namespace MudSharp.Computers;

internal static class ComputerProcessWaitArguments
{
	private sealed class UserInputWaitPayload
	{
		public long CharacterId { get; set; }
		public long TerminalItemId { get; set; }
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
}
