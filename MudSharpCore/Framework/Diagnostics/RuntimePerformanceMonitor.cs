using System.Diagnostics;
using System.Numerics;
using System.Reflection;
using System.Text;
using MudSharp.Framework.Diagnostics;

namespace MudSharp.Framework.Diagnostics;

public sealed class RuntimePerformanceMonitor : IRuntimePerformanceMonitor
{
	private const int HistogramBucketCount = 32;
	private const int MaximumHeartbeatCallbacks = 32;
	private readonly Dictionary<RuntimeLoopPhase, TimingStatistics> _loopPhases = [];
	private readonly Dictionary<RuntimeSchedulerKind, SchedulerStatistics> _schedulers = [];
	private readonly Dictionary<HeartbeatCallbackKey, TimingStatistics> _heartbeatCallbacks = [];
	private readonly TimingStatistics _otherHeartbeatCallbacks = new();
	private readonly Process _process = Process.GetCurrentProcess();
	private readonly IRuntimeNetworkPerformanceSource _networkPerformanceSource;
	private TimeSpan _startProcessorTime;
	private long _startAllocatedBytes;
	private readonly int[] _startCollections = new int[GC.MaxGeneration + 1];
	private long _sessionStartTimestamp = Stopwatch.GetTimestamp();
	private long _loopCount;
	private long _loopOverruns;
	private long _totalLoopTicks;
	private long _maximumLoopTicks;
	private long _totalLoopAllocatedBytes;

	public RuntimePerformanceMonitor(IRuntimeNetworkPerformanceSource networkPerformanceSource = null)
	{
		_networkPerformanceSource = networkPerformanceSource;
	}

	public bool Enabled { get; private set; }

	public void Enable()
	{
		Reset();
		Enabled = true;
	}

	public void Disable()
	{
		Enabled = false;
	}

	public void Reset()
	{
		_loopPhases.Clear();
		_schedulers.Clear();
		_heartbeatCallbacks.Clear();
		_otherHeartbeatCallbacks.Clear();
		_loopCount = 0;
		_loopOverruns = 0;
		_totalLoopTicks = 0;
		_maximumLoopTicks = 0;
		_totalLoopAllocatedBytes = 0;
		_networkPerformanceSource?.ResetNetworkPerformanceCounters();
		_sessionStartTimestamp = Stopwatch.GetTimestamp();
		_startProcessorTime = _process.TotalProcessorTime;
		_startAllocatedBytes = GC.GetTotalAllocatedBytes(false);
		for (var i = 0; i <= GC.MaxGeneration; i++)
		{
			_startCollections[i] = GC.CollectionCount(i);
		}
	}

	public void RecordLoopPhase(RuntimeLoopPhase phase, long elapsedStopwatchTicks, long allocatedBytes)
	{
		if (!Enabled)
		{
			return;
		}

		if (!_loopPhases.TryGetValue(phase, out var statistics))
		{
			_loopPhases[phase] = statistics = new TimingStatistics();
		}

		statistics.Record(elapsedStopwatchTicks, allocatedBytes);
	}

	public void RecordLoopIteration(long elapsedStopwatchTicks, long allocatedBytes, bool overrun)
	{
		if (!Enabled)
		{
			return;
		}

		_loopCount++;
		_totalLoopTicks += elapsedStopwatchTicks;
		_maximumLoopTicks = Math.Max(_maximumLoopTicks, elapsedStopwatchTicks);
		_totalLoopAllocatedBytes += Math.Max(0, allocatedBytes);
		if (overrun)
		{
			_loopOverruns++;
		}
	}

	public void RecordSchedulerCheck(RuntimeSchedulerKind kind, int queueLength, int fired, int overdue,
		long elapsedStopwatchTicks)
	{
		if (!Enabled)
		{
			return;
		}

		if (!_schedulers.TryGetValue(kind, out var statistics))
		{
			_schedulers[kind] = statistics = new SchedulerStatistics();
		}

		statistics.QueueLength = queueLength;
		statistics.Fired += fired;
		statistics.Overdue += overdue;
		statistics.Timing.Record(elapsedStopwatchTicks, 0);
	}

	public void RecordHeartbeatCallback(string cadence, Delegate callback, long elapsedStopwatchTicks)
	{
		if (!Enabled)
		{
			return;
		}

		var key = new HeartbeatCallbackKey(cadence, callback.Method);
		if (!_heartbeatCallbacks.TryGetValue(key, out var statistics))
		{
			if (_heartbeatCallbacks.Count >= MaximumHeartbeatCallbacks)
			{
				_otherHeartbeatCallbacks.Record(elapsedStopwatchTicks, 0);
				return;
			}

			_heartbeatCallbacks[key] = statistics = new TimingStatistics();
		}

		statistics.Record(elapsedStopwatchTicks, 0);
	}

	public void AppendReport(StringBuilder sb, IFormatProvider formatProvider)
	{
		var elapsed = ToTimeSpan(Stopwatch.GetTimestamp() - _sessionStartTimestamp);
		var allocated = GC.GetTotalAllocatedBytes(false) - _startAllocatedBytes;
		var processor = _process.TotalProcessorTime - _startProcessorTime;

		sb.AppendLine($"Runtime performance monitoring is {(Enabled ? "enabled" : "disabled")}.");
		sb.AppendLine($"Session: {elapsed:hh\\:mm\\:ss} | Loops: {_loopCount:N0} | Overruns: {_loopOverruns:N0}");
		sb.AppendLine($"Process CPU: {processor.TotalSeconds.ToString("N2", formatProvider)}s | Working set: {_process.WorkingSet64.ToString("N0", formatProvider)} bytes | Managed heap: {GC.GetTotalMemory(false).ToString("N0", formatProvider)} bytes");
		sb.AppendLine($"Allocated: {allocated.ToString("N0", formatProvider)} bytes | GC: {string.Join(", ", Enumerable.Range(0, GC.MaxGeneration + 1).Select(x => $"Gen{x} +{GC.CollectionCount(x) - _startCollections[x]:N0}"))}");

		if (_loopCount > 0)
		{
			sb.AppendLine($"Game loop: avg {FormatTicks(_totalLoopTicks / _loopCount, formatProvider)}, max {FormatTicks(_maximumLoopTicks, formatProvider)}, main-thread alloc {_totalLoopAllocatedBytes:N0} bytes");
		}

		if (_loopPhases.Count > 0)
		{
			sb.AppendLine("Loop phases:");
			foreach (var (phase, statistics) in _loopPhases.OrderBy(x => x.Key))
			{
				sb.AppendLine($"\t{phase}: {statistics.Describe(formatProvider)}");
			}
		}

		if (_schedulers.Count > 0)
		{
			sb.AppendLine("Schedulers:");
			foreach (var (kind, statistics) in _schedulers.OrderBy(x => x.Key))
			{
				sb.AppendLine($"\t{kind}: queued {statistics.QueueLength:N0}, fired {statistics.Fired:N0}, overdue {statistics.Overdue:N0}, {statistics.Timing.Describe(formatProvider)}");
			}
		}

		if (_heartbeatCallbacks.Count > 0)
		{
			sb.AppendLine("Slowest heartbeat callbacks:");
			var heartbeatCallbacks = _heartbeatCallbacks
				.Select(x => (Name: x.Key.Describe(), Statistics: x.Value))
				.Append((Name: "Other heartbeat callbacks", Statistics: _otherHeartbeatCallbacks))
				.Where(x => x.Statistics.Count > 0);
			foreach (var (name, statistics) in heartbeatCallbacks
			         .OrderByDescending(x => x.Statistics.MaximumTicks)
			         .ThenByDescending(x => x.Statistics.TotalTicks)
			         .Take(10))
			{
				sb.AppendLine($"\t{name}: {statistics.Describe(formatProvider)}");
			}
		}

		if (_networkPerformanceSource is not null)
		{
			var network = _networkPerformanceSource.GetNetworkPerformanceSnapshot();
			sb.AppendLine("Network transport:");
			sb.AppendLine($"\tConnections: accepted {network.AcceptedConnections:N0}, flood-rejected {network.FloodRejectedConnections:N0}, active {network.ActiveConnections:N0}, slow-client disconnects {network.SlowClientDisconnects:N0}");
			sb.AppendLine($"\tInput: {network.BytesReceived:N0} bytes in {network.ReadOperations:N0} reads, queue high-water {network.InputQueueHighWatermark:N0} commands");
			sb.AppendLine($"\tOutput: {network.BytesSent:N0} bytes in {network.WriteOperations:N0} writes, queue high-water {network.OutputQueueHighWatermarkBytes:N0} bytes");
			sb.AppendLine($"\tErrors: accept {network.AcceptErrors:N0}, read {network.ReadErrors:N0}, write {network.WriteErrors:N0}");
		}
	}

	private static TimeSpan ToTimeSpan(long ticks)
	{
		return TimeSpan.FromSeconds((double)ticks / Stopwatch.Frequency);
	}

	private static string FormatTicks(long ticks, IFormatProvider formatProvider)
	{
		return ToTimeSpan(ticks).TotalMilliseconds.ToString("N3", formatProvider) + "ms";
	}

	private sealed class SchedulerStatistics
	{
		public int QueueLength { get; set; }
		public long Fired { get; set; }
		public long Overdue { get; set; }
		public TimingStatistics Timing { get; } = new();
	}

	private sealed class TimingStatistics
	{
		private readonly long[] _histogram = new long[HistogramBucketCount];
		public long Count { get; private set; }
		public long TotalTicks { get; private set; }
		public long MaximumTicks { get; private set; }
		public long AllocatedBytes { get; private set; }

		public void Clear()
		{
			Array.Clear(_histogram);
			Count = 0;
			TotalTicks = 0;
			MaximumTicks = 0;
			AllocatedBytes = 0;
		}

		public void Record(long ticks, long allocatedBytes)
		{
			Count++;
			TotalTicks += ticks;
			MaximumTicks = Math.Max(MaximumTicks, ticks);
			AllocatedBytes += Math.Max(0, allocatedBytes);
			_histogram[BucketFor(ticks)]++;
		}

		public string Describe(IFormatProvider formatProvider)
		{
			if (Count == 0)
			{
				return "no samples";
			}

			return $"count {Count:N0}, avg {FormatTicks(TotalTicks / Count, formatProvider)}, p95 {FormatTicks(ApproximatePercentile(0.95), formatProvider)}, max {FormatTicks(MaximumTicks, formatProvider)}, alloc {AllocatedBytes:N0} bytes";
		}

		private long ApproximatePercentile(double percentile)
		{
			var target = (long)Math.Ceiling(Count * percentile);
			var total = 0L;
			for (var i = 0; i < _histogram.Length; i++)
			{
				total += _histogram[i];
				if (total >= target)
				{
					return i == 0 ? 1L : 1L << Math.Min(62, i + 1);
				}
			}

			return MaximumTicks;
		}

		private static int BucketFor(long ticks)
		{
			if (ticks <= 1)
			{
				return 0;
			}

			return Math.Min(HistogramBucketCount - 1, BitOperations.Log2((ulong)ticks));
		}
	}

	private readonly record struct HeartbeatCallbackKey(string Cadence, MethodInfo Method)
	{
		public string Describe()
		{
			return $"{Cadence}: {Method.DeclaringType?.FullName ?? "<unknown>"}.{Method.Name}";
		}
	}
}
