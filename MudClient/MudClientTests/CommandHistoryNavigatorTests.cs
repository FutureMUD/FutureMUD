using MudClientBlazor.Services;

namespace MudClientTests;

public class CommandHistoryNavigatorTests
{
	private static readonly IReadOnlyList<string> History = ["look", "score", "inventory"];

	[Fact]
	public void Navigate_UpFromNewestEntry_ReturnsMostRecentCommand()
	{
		var result = CommandHistoryNavigator.Navigate(History, History.Count, CommandHistoryDirection.Up, false);

		Assert.Equal(History.Count - 1, result.Index);
		Assert.Equal("inventory", result.Input);
	}

	[Fact]
	public void Navigate_UpAtOldestEntry_PreservesCurrentInput()
	{
		var result = CommandHistoryNavigator.Navigate(History, 0, CommandHistoryDirection.Up, false);

		Assert.Equal(0, result.Index);
		Assert.False(result.HasChanged);
		Assert.Null(result.Input);
	}

	[Fact]
	public void Navigate_DownFromNewestEntry_ReturnsBlankInput()
	{
		var result = CommandHistoryNavigator.Navigate(History, History.Count - 1, CommandHistoryDirection.Down, false);

		Assert.Equal(History.Count, result.Index);
		Assert.True(result.HasChanged);
		Assert.Equal(string.Empty, result.Input);
	}

	[Fact]
	public void Navigate_DownPastNewestEntry_RestoresDraftInput()
	{
		var result = CommandHistoryNavigator.Navigate(
			History,
			History.Count - 1,
			CommandHistoryDirection.Down,
			false,
			"say unfinished thought");

		Assert.Equal(History.Count, result.Index);
		Assert.Equal("say unfinished thought", result.Input);
	}

	[Fact]
	public void Navigate_WithControlJump_MovesToRequestedHistoryBoundary()
	{
		var oldest = CommandHistoryNavigator.Navigate(History, History.Count, CommandHistoryDirection.Up, true);
		var newest = CommandHistoryNavigator.Navigate(History, 0, CommandHistoryDirection.Down, true);

		Assert.Equal((0, "look"), (oldest.Index, oldest.Input));
		Assert.Equal((History.Count - 1, "inventory"), (newest.Index, newest.Input));
	}

	[Fact]
	public void Navigate_WithoutHistory_LeavesInputUntouched()
	{
		var result = CommandHistoryNavigator.Navigate([], 0, CommandHistoryDirection.Down, false);

		Assert.False(result.HasChanged);
		Assert.Null(result.Input);
	}
}
