namespace MudClientBlazor.Services;

public enum CommandHistoryDirection
{
	Up,
	Down
}

public readonly record struct CommandHistoryNavigation(int Index, string? Input)
{
	public bool HasChanged => Input is not null;
}

public static class CommandHistoryNavigator
{
	public static CommandHistoryNavigation Navigate(
		IReadOnlyList<string> commandHistory,
		int currentIndex,
		CommandHistoryDirection direction,
		bool jumpToBoundary,
		string draftInput = "")
	{
		ArgumentNullException.ThrowIfNull(commandHistory);

		if (commandHistory.Count == 0)
		{
			return new CommandHistoryNavigation(currentIndex, null);
		}

		var index = currentIndex < 0
			? commandHistory.Count
			: Math.Clamp(currentIndex, 0, commandHistory.Count);
		return direction switch
		{
			CommandHistoryDirection.Up => NavigateUp(commandHistory, index, jumpToBoundary),
			CommandHistoryDirection.Down => NavigateDown(commandHistory, index, jumpToBoundary, draftInput),
			_ => throw new ArgumentOutOfRangeException(nameof(direction), direction, null)
		};
	}

	private static CommandHistoryNavigation NavigateUp(
		IReadOnlyList<string> commandHistory,
		int currentIndex,
		bool jumpToBoundary)
	{
		if (currentIndex == 0)
		{
			return new CommandHistoryNavigation(currentIndex, null);
		}

		var nextIndex = jumpToBoundary ? 0 : currentIndex - 1;
		return new CommandHistoryNavigation(nextIndex, commandHistory[nextIndex]);
	}

	private static CommandHistoryNavigation NavigateDown(
		IReadOnlyList<string> commandHistory,
		int currentIndex,
		bool jumpToBoundary,
		string draftInput)
	{
		if (currentIndex >= commandHistory.Count)
		{
			return new CommandHistoryNavigation(currentIndex, null);
		}

		var nextIndex = jumpToBoundary ? commandHistory.Count - 1 : currentIndex + 1;
		return nextIndex == commandHistory.Count
			? new CommandHistoryNavigation(nextIndex, draftInput)
			: new CommandHistoryNavigation(nextIndex, commandHistory[nextIndex]);
	}
}
