namespace MudClientBlazor.Services;

public sealed record TranscriptEntry(long Id, string Html);

public sealed class ClientTranscript
{
	public const int DefaultRenderedEntryLimit = 750;
	public const int DefaultLogEntryLimit = 10_000;
	public const int DefaultRenderedCharacterLimit = 1_000_000;
	public const int DefaultLogCharacterLimit = 5_000_000;

	private readonly int _renderedEntryLimit;
	private readonly int _logEntryLimit;
	private readonly int _renderedCharacterLimit;
	private readonly int _logCharacterLimit;
	private readonly List<TranscriptEntry> _renderedEntries = [];
	private readonly List<TranscriptEntry> _logEntries = [];
	private long _nextId;
	private int _renderedCharacterCount;
	private int _logCharacterCount;

	public ClientTranscript(
		int renderedEntryLimit = DefaultRenderedEntryLimit,
		int logEntryLimit = DefaultLogEntryLimit,
		int renderedCharacterLimit = DefaultRenderedCharacterLimit,
		int logCharacterLimit = DefaultLogCharacterLimit)
	{
		if (renderedEntryLimit < 1)
		{
			throw new ArgumentOutOfRangeException(nameof(renderedEntryLimit));
		}

		if (logEntryLimit < renderedEntryLimit)
		{
			throw new ArgumentOutOfRangeException(nameof(logEntryLimit));
		}

		if (renderedCharacterLimit < 1)
		{
			throw new ArgumentOutOfRangeException(nameof(renderedCharacterLimit));
		}

		if (logCharacterLimit < renderedCharacterLimit)
		{
			throw new ArgumentOutOfRangeException(nameof(logCharacterLimit));
		}

		_renderedEntryLimit = renderedEntryLimit;
		_logEntryLimit = logEntryLimit;
		_renderedCharacterLimit = renderedCharacterLimit;
		_logCharacterLimit = logCharacterLimit;
	}

	public IReadOnlyList<TranscriptEntry> RenderedEntries => _renderedEntries;
	public IReadOnlyList<TranscriptEntry> LogEntries => _logEntries;

	public void Add(string html)
	{
		ArgumentNullException.ThrowIfNull(html);

		var entry = new TranscriptEntry(_nextId++, html);
		_renderedEntries.Add(entry);
		_logEntries.Add(entry);
		_renderedCharacterCount += html.Length;
		_logCharacterCount += html.Length;
		TrimToLimit(_renderedEntries, _renderedEntryLimit, _renderedCharacterLimit, ref _renderedCharacterCount);
		TrimToLimit(_logEntries, _logEntryLimit, _logCharacterLimit, ref _logCharacterCount);
	}

	private static void TrimToLimit(
		List<TranscriptEntry> entries,
		int entryLimit,
		int characterLimit,
		ref int characterCount)
	{
		var removeCount = 0;
		while (removeCount < entries.Count &&
		       (entries.Count - removeCount > entryLimit || characterCount > characterLimit))
		{
			characterCount -= entries[removeCount].Html.Length;
			removeCount++;
		}

		if (removeCount > 0)
		{
			entries.RemoveRange(0, removeCount);
		}
	}
}
