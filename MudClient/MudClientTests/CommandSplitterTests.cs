using MudClientBlazor.Services;

namespace MudClientTests;

public class CommandSplitterTests
{
	[Fact]
	public void Split_SeparatesSemicolonsAndCrLfNewlines()
	{
		var commands = CommandSplitter.Split("look;score\r\nnorth", ClientSettings.CreateDefault());

		Assert.Equal(["look", "score", "north"], commands);
	}

	[Fact]
	public void Split_UnescapesConfiguredDelimiter()
	{
		var commands = CommandSplitter.Split("say hello\\;world;look", ClientSettings.CreateDefault());

		Assert.Equal(["say hello;world", "look"], commands);
		Assert.DoesNotContain("say hello;world ", commands);
	}

	[Fact]
	public void Split_PreservesBackslashWhenItDoesNotEscapeDelimiter()
	{
		var commands = CommandSplitter.Split("use C:\\temp;look", ClientSettings.CreateDefault());

		Assert.Equal(["use C:\\temp", "look"], commands);
	}

	[Fact]
	public void Split_CanDisableSemicolonAndNewlineStacking()
	{
		var settings = ClientSettings.CreateDefault();
		settings.SemicolonCommandStackingEnabled = false;
		settings.NewlineCommandStackingEnabled = false;

		var commands = CommandSplitter.Split("look;score\r\nnorth", settings);

		Assert.Equal(["look;score\r\nnorth"], commands);
	}

	[Fact]
	public void Split_CanUseAConfiguredDelimiter()
	{
		var settings = ClientSettings.CreateDefault();
		settings.CommandStackingDelimiter = "|";

		var commands = CommandSplitter.Split("look|score\\|me", settings);

		Assert.Equal(["look", "score|me"], commands);
	}
}
