using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;

#nullable enable

namespace MudSharp.Combat.Simulation;

/// <summary>
/// Produces a versioned, deterministic digest of an accelerated combat run. It deliberately excludes
/// wall-clock timing and transient framework identifiers so that repeated seeded runs can be compared.
/// </summary>
internal sealed class CombatSimulationExecutionFingerprint
{
	public const string Version = "v1";
	private const int TraceCheckpointInterval = 50;
	private const int MaximumTraceCheckpoints = 200;
	private const int MaximumRecentRandomOperations = 128;
	private const int MaximumDetailedRandomOperations = 10_000;
	private const int MaximumMaterialisationEntries = 256;
	private const int MaximumDetailedStateOperations = 10_000;
	private readonly IncrementalHash _hash = IncrementalHash.CreateHash(HashAlgorithmName.SHA256);
	private readonly IncrementalHash _materialisationHash = IncrementalHash.CreateHash(HashAlgorithmName.SHA256);
	private readonly IncrementalHash _randomHash = IncrementalHash.CreateHash(HashAlgorithmName.SHA256);
	private readonly IncrementalHash _schedulerHash = IncrementalHash.CreateHash(HashAlgorithmName.SHA256);
	private readonly IncrementalHash _transcriptHash = IncrementalHash.CreateHash(HashAlgorithmName.SHA256);
	private readonly IncrementalHash _terminalHash = IncrementalHash.CreateHash(HashAlgorithmName.SHA256);
	private readonly IncrementalHash _randomCheckpointHash = IncrementalHash.CreateHash(HashAlgorithmName.SHA256);
	private readonly IncrementalHash _schedulerCheckpointHash = IncrementalHash.CreateHash(HashAlgorithmName.SHA256);
	private readonly IncrementalHash _transcriptCheckpointHash = IncrementalHash.CreateHash(HashAlgorithmName.SHA256);
	private readonly List<CombatSimulationExecutionTraceCheckpoint> _checkpoints = [];
	private readonly Queue<CombatSimulationRandomTraceEntry> _recentRandomOperations = [];
	private readonly List<CombatSimulationMaterialisationTraceEntry> _materialisationEntries = [];
	private readonly Queue<CombatSimulationStateTraceEntry> _recentStateOperations = [];
	private string? _completedFingerprint;
	private CombatSimulationExecutionTraceSummary? _traceSummary;
	private int _materialisationOperations;
	private int _randomOperations;
	private int _schedulerTicks;
	private int _transcriptEntries;
	private int _lastCheckpointEventCount;
	private int _nextCheckpointEvent = TraceCheckpointInterval;
	private bool _checkpointsTruncated;

	public CombatSimulationExecutionFingerprint(int seed, bool captureRandomCallSites = false)
	{
		CaptureRandomCallSites = captureRandomCallSites;
		Record("futuremud-combat-simulation");
		Record(Version);
		Record("seed");
		Record(seed.ToString(System.Globalization.CultureInfo.InvariantCulture));
	}

	public CombatSimulationExecutionTraceSummary TraceSummary => _traceSummary ??
		throw new InvalidOperationException("The combat simulation fingerprint has not been completed.");
	public bool CaptureRandomCallSites { get; }
	public CombatSimulationRandomTraceEntry? LastRandomOperation => _recentRandomOperations.LastOrDefault();

	public void RecordMaterialisation(CombatSimulationParticipantRequest participant)
	{
		var slot = participant.Slot.ToString(System.Globalization.CultureInfo.InvariantCulture);
		var sourceType = ((int)participant.SourceType).ToString(System.Globalization.CultureInfo.InvariantCulture);
		var source = participant.SourceType == CombatSimulationSourceType.Character
			? participant.Character?.Id.ToString(System.Globalization.CultureInfo.InvariantCulture) ?? string.Empty
			: participant.NpcTemplate?.Id.ToString(System.Globalization.CultureInfo.InvariantCulture) ?? string.Empty;
		var ordinal = participant.Ordinal.ToString(System.Globalization.CultureInfo.InvariantCulture);
		Record("materialise");
		Record(slot);
		Record(participant.Team);
		Record(sourceType);
		Record(source);
		Record(ordinal);
		RecordTrace(_materialisationHash, "materialise", slot, participant.Team, sourceType, source, ordinal);
		_materialisationOperations++;
	}

	public void RecordMaterialisationRuntimeState(int slot, string category, IEnumerable<string> values)
	{
		var slotText = slot.ToString(System.Globalization.CultureInfo.InvariantCulture);
		var value = string.Join("|", values);
		Record("materialise-runtime");
		Record(slotText);
		Record(category);
		Record(value);
		RecordTrace(_materialisationHash, "materialise-runtime", slotText, category, value);
		if (CaptureRandomCallSites && _materialisationEntries.Count < MaximumMaterialisationEntries)
		{
			_materialisationEntries.Add(new CombatSimulationMaterialisationTraceEntry(
				_materialisationOperations + 1,
				$"slot:{slotText} {category}:{ShortDigest(value)}"));
		}

		_materialisationOperations++;
	}

	public void RecordRandom(
		string operation,
		long value,
		long firstArgument = 0,
		long secondArgument = 0,
		string? callSite = null)
	{
		var first = firstArgument.ToString(System.Globalization.CultureInfo.InvariantCulture);
		var second = secondArgument.ToString(System.Globalization.CultureInfo.InvariantCulture);
		var result = value.ToString(System.Globalization.CultureInfo.InvariantCulture);
		Record("random");
		Record(operation);
		Record(first);
		Record(second);
		Record(result);
		RecordTrace(_randomHash, "random", operation, first, second, result);
		RecordTrace(_randomCheckpointHash, "random", operation, first, second, result);
		RecordRecentRandomOperation($"{operation}({first}, {second}) = {result}", callSite);
		_randomOperations++;
	}

	public void RecordRandom(string operation, double value, string? callSite = null)
	{
		var result = BitConverter.DoubleToInt64Bits(value).ToString(System.Globalization.CultureInfo.InvariantCulture);
		Record("random");
		Record(operation);
		Record(result);
		RecordTrace(_randomHash, "random", operation, result);
		RecordTrace(_randomCheckpointHash, "random", operation, result);
		RecordRecentRandomOperation($"{operation} = {result}", callSite);
		_randomOperations++;
	}

	public void RecordRandomBytes(ReadOnlySpan<byte> values, string? callSite = null)
	{
		var result = Convert.ToHexString(values);
		Record("random-bytes");
		Record(result);
		RecordTrace(_randomHash, "random-bytes", result);
		RecordTrace(_randomCheckpointHash, "random-bytes", result);
		RecordRecentRandomOperation($"bytes = {result}", callSite);
		_randomOperations++;
	}

	public void RecordState(string description)
	{
		if (!CaptureRandomCallSites)
		{
			return;
		}

		if (_recentStateOperations.Count >= MaximumDetailedStateOperations)
		{
			_recentStateOperations.Dequeue();
		}

		_recentStateOperations.Enqueue(new CombatSimulationStateTraceEntry(
			_recentStateOperations.Count == 0
				? 1
				: _recentStateOperations.Last().OperationIndex + 1,
			description));
	}

	public void RecordTranscript(string participant, TimeSpan elapsed, string line)
	{
		var ticks = elapsed.Ticks.ToString(System.Globalization.CultureInfo.InvariantCulture);
		Record("output");
		Record(participant);
		Record(ticks);
		Record(line);
		RecordTrace(_transcriptHash, "output", participant, ticks, line);
		RecordTrace(_transcriptCheckpointHash, "output", participant, ticks, line);
		_transcriptEntries++;
	}

	public void RecordSchedulerTick(DateTimeOffset utc, int schedulerFired, int effectSchedulerFired, int totalEvents)
	{
		var ticks = utc.Ticks.ToString(System.Globalization.CultureInfo.InvariantCulture);
		var scheduler = schedulerFired.ToString(System.Globalization.CultureInfo.InvariantCulture);
		var effects = effectSchedulerFired.ToString(System.Globalization.CultureInfo.InvariantCulture);
		var total = totalEvents.ToString(System.Globalization.CultureInfo.InvariantCulture);
		Record("scheduler");
		Record(ticks);
		Record(scheduler);
		Record(effects);
		Record(total);
		RecordTrace(_schedulerHash, "scheduler", ticks, scheduler, effects, total);
		RecordTrace(_schedulerCheckpointHash, "scheduler", ticks, scheduler, effects, total);
		_schedulerTicks++;
		if (totalEvents >= _nextCheckpointEvent)
		{
			CaptureCheckpoint(totalEvents);
			_nextCheckpointEvent = ((totalEvents / TraceCheckpointInterval) + 1) * TraceCheckpointInterval;
		}
	}

	public string Complete(CombatSimulationRunStatus status, string? winningTeam, TimeSpan virtualDuration,
		int eventCount, IEnumerable<CombatSimulationParticipantResult> participants)
	{
		if (_completedFingerprint is not null)
		{
			return _completedFingerprint;
		}

		var terminalTrace = new List<string>
		{
			"terminal",
			((int)status).ToString(System.Globalization.CultureInfo.InvariantCulture),
			winningTeam ?? string.Empty,
			virtualDuration.Ticks.ToString(System.Globalization.CultureInfo.InvariantCulture),
			eventCount.ToString(System.Globalization.CultureInfo.InvariantCulture)
		};
		foreach (var value in terminalTrace)
		{
			Record(value);
		}

		foreach (var participant in participants.OrderBy(x => x.Slot))
		{
			var participantTrace = new[]
			{
				"participant",
				participant.Slot.ToString(System.Globalization.CultureInfo.InvariantCulture),
				participant.Team,
				((int)participant.Outcome).ToString(System.Globalization.CultureInfo.InvariantCulture),
				((long)participant.FinalState).ToString(System.Globalization.CultureInfo.InvariantCulture),
				BitConverter.DoubleToInt64Bits(participant.BloodRatio)
					.ToString(System.Globalization.CultureInfo.InvariantCulture),
				BitConverter.DoubleToInt64Bits(participant.StaminaRatio)
					.ToString(System.Globalization.CultureInfo.InvariantCulture),
				participant.WoundCount.ToString(System.Globalization.CultureInfo.InvariantCulture),
				participant.UnderFullGrappleControl ? "1" : "0"
			};
			foreach (var value in participantTrace)
			{
				Record(value);
			}

			terminalTrace.AddRange(participantTrace);
		}

		if (eventCount > 0 && eventCount != _lastCheckpointEventCount)
		{
			CaptureCheckpoint(eventCount);
		}

		_completedFingerprint = $"{Version}:{Convert.ToHexString(_hash.GetHashAndReset()).ToLowerInvariant()}";
		_traceSummary = new CombatSimulationExecutionTraceSummary(
			_materialisationOperations,
			Digest(_materialisationHash),
			_materialisationEntries.AsReadOnly(),
			_randomOperations,
			Digest(_randomHash),
			_schedulerTicks,
			Digest(_schedulerHash),
			_transcriptEntries,
			Digest(_transcriptHash),
			Digest(_terminalHash, terminalTrace),
			_recentRandomOperations.ToArray(),
			_recentStateOperations.ToArray(),
			_checkpoints.AsReadOnly(),
			_checkpointsTruncated);
		return _completedFingerprint;
	}

	private void Record(string value)
	{
		Append(_hash, value);
	}

	private void CaptureCheckpoint(int eventCount)
	{
		if (_checkpoints.Count >= MaximumTraceCheckpoints)
		{
			_checkpointsTruncated = true;
			return;
		}

		_checkpoints.Add(new CombatSimulationExecutionTraceCheckpoint(
			eventCount,
			_randomOperations,
			Digest(_randomCheckpointHash),
			_schedulerTicks,
			Digest(_schedulerCheckpointHash),
			_transcriptEntries,
			Digest(_transcriptCheckpointHash)));
		_lastCheckpointEventCount = eventCount;
	}

	private void RecordRecentRandomOperation(string description, string? callSite)
	{
		var maximumOperations = CaptureRandomCallSites
			? MaximumDetailedRandomOperations
			: MaximumRecentRandomOperations;
		if (_recentRandomOperations.Count >= maximumOperations)
		{
			_recentRandomOperations.Dequeue();
		}

		_recentRandomOperations.Enqueue(new CombatSimulationRandomTraceEntry(
			_randomOperations + 1,
			string.IsNullOrEmpty(callSite) ? description : $"{description} from {callSite}"));
	}

	private static void RecordTrace(IncrementalHash hash, params string[] values)
	{
		foreach (var value in values)
		{
			Append(hash, value);
		}
	}

	private static string Digest(IncrementalHash hash, IEnumerable<string>? trailingValues = null)
	{
		if (trailingValues is not null)
		{
			foreach (var value in trailingValues)
			{
				Append(hash, value);
			}
		}

		return Convert.ToHexString(hash.GetHashAndReset()).ToLowerInvariant();
	}

	private static void Append(IncrementalHash hash, string value)
	{
		var bytes = Encoding.UTF8.GetBytes(value);
		hash.AppendData(bytes);
		hash.AppendData([0]);
	}

	private static string ShortDigest(string value)
	{
		return Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(value)))[..12].ToLowerInvariant();
	}
}

internal sealed class CombatSimulationRecordingRandom(Random inner, CombatSimulationExecutionFingerprint fingerprint)
	: Random
{
	public override int Next()
	{
		var value = inner.Next();
		fingerprint.RecordRandom("next", value, callSite: GetCallSite());
		return value;
	}

	public override int Next(int maxValue)
	{
		var value = inner.Next(maxValue);
		fingerprint.RecordRandom("next-max", value, maxValue, callSite: GetCallSite());
		return value;
	}

	public override int Next(int minValue, int maxValue)
	{
		var value = inner.Next(minValue, maxValue);
		fingerprint.RecordRandom("next-range", value, minValue, maxValue, GetCallSite());
		return value;
	}

	public override double NextDouble()
	{
		var value = inner.NextDouble();
		fingerprint.RecordRandom("next-double", value, GetCallSite());
		return value;
	}

	public override float NextSingle()
	{
		var value = inner.NextSingle();
		fingerprint.RecordRandom("next-single", value, GetCallSite());
		return value;
	}

	public override long NextInt64()
	{
		var value = inner.NextInt64();
		fingerprint.RecordRandom("next-int64", value, GetCallSite());
		return value;
	}

	public override long NextInt64(long maxValue)
	{
		var value = inner.NextInt64(maxValue);
		fingerprint.RecordRandom("next-int64-max", value, maxValue, callSite: GetCallSite());
		return value;
	}

	public override long NextInt64(long minValue, long maxValue)
	{
		var value = inner.NextInt64(minValue, maxValue);
		fingerprint.RecordRandom("next-int64-range", value, minValue, maxValue, GetCallSite());
		return value;
	}

	public override void NextBytes(byte[] buffer)
	{
		inner.NextBytes(buffer);
		fingerprint.RecordRandomBytes(buffer, GetCallSite());
	}

	public override void NextBytes(Span<byte> buffer)
	{
		inner.NextBytes(buffer);
		fingerprint.RecordRandomBytes(buffer, GetCallSite());
	}

	private string? GetCallSite()
	{
		if (!fingerprint.CaptureRandomCallSites)
		{
			return null;
		}

		var callSites = new StackTrace(skipFrames: 1, fNeedFileInfo: false)
			.GetFrames()?
			.Select(x => x.GetMethod())
			.Where(x => x?.DeclaringType != typeof(CombatSimulationRecordingRandom))
			.Take(4)
			.Select(x => $"{x!.DeclaringType?.FullName}.{x.Name}")
			.ToList();
		return callSites is { Count: > 0 }
			? string.Join(" <- ", callSites)
			: null;
	}
}
