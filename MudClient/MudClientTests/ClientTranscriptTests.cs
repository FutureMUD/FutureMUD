using MudClientBlazor.Services;

namespace MudClientTests;

public class ClientTranscriptTests
{
	[Fact]
	public void Add_BoundsRenderedAndSavedEntriesIndependently()
	{
		var transcript = new ClientTranscript(renderedEntryLimit: 2, logEntryLimit: 3);

		transcript.Add("one");
		transcript.Add("two");
		transcript.Add("three");
		transcript.Add("four");

		Assert.Equal(["three", "four"], transcript.RenderedEntries.Select(entry => entry.Html));
		Assert.Equal(["two", "three", "four"], transcript.LogEntries.Select(entry => entry.Html));
		Assert.Equal([2, 3], transcript.RenderedEntries.Select(entry => entry.Id));
	}

	[Fact]
	public void Add_BoundsTranscriptByCharacterCount()
	{
		var transcript = new ClientTranscript(
			renderedEntryLimit: 10,
			logEntryLimit: 10,
			renderedCharacterLimit: 5,
			logCharacterLimit: 8);

		transcript.Add("1234");
		transcript.Add("5678");

		Assert.Equal(["5678"], transcript.RenderedEntries.Select(entry => entry.Html));
		Assert.Equal(["1234", "5678"], transcript.LogEntries.Select(entry => entry.Html));
	}
}
