using MudSharp.Framework;
using MudSharp.PerceptionEngine;

#nullable enable

namespace MudSharp.Combat.Simulation;

internal sealed class CombatSimulationTranscript(TimeProvider timeProvider, DateTimeOffset startedAt, int maximumEntries,
	CombatSimulationExecutionFingerprint fingerprint)
{
	private readonly List<string> _entries = [];

	public IReadOnlyList<string> Entries => _entries;
	public bool Truncated { get; private set; }

	public void Add(string participant, string traceParticipant, string text)
	{
		if (string.IsNullOrWhiteSpace(text))
		{
			return;
		}

		foreach (var line in text.RawText()
		         .Split(['\r', '\n'], StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
		{
			var elapsed = timeProvider.GetUtcNow() - startedAt;
			fingerprint.RecordTranscript(traceParticipant, elapsed, line);
			if (_entries.Count >= maximumEntries)
			{
				Truncated = true;
				return;
			}

			_entries.Add($"[{elapsed:hh\\:mm\\:ss\\.fff}] {participant}: {line}");
		}
	}
}

internal sealed class CombatSimulationOutputHandler(
	CombatSimulationTranscript transcript,
	string participant,
	string traceParticipant) : IOutputHandler
{
	public IPerceiver? Perceiver { get; private set; }
	public bool HasBufferedOutput => false;
	public string BufferedOutput => string.Empty;
	public bool QuietMode { get; set; }

	public bool Register(IPerceiver perceiver)
	{
		Perceiver = perceiver;
		return true;
	}

	public bool Send(string text, bool newline = true, bool nopage = false)
	{
		if (QuietMode || string.IsNullOrEmpty(text))
		{
			return false;
		}

		transcript.Add(participant, traceParticipant, text);
		return true;
	}

	public bool Send(IOutput output, bool newline = true, bool nopage = false)
	{
		if (output is null || Perceiver is null || !output.ShouldSee(Perceiver))
		{
			return false;
		}

		return Send(output.ParseFor(Perceiver), newline, nopage);
	}

	public void More()
	{
	}

	public void Flush()
	{
	}

	public bool SendPrompt()
	{
		return true;
	}
}
