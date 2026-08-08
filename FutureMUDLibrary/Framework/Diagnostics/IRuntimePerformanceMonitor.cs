using System;
using System.Text;

namespace MudSharp.Framework.Diagnostics;

public enum RuntimeLoopPhase
{
	Network,
	PendingCommands,
	IdlerWarnings,
	Clocks,
	Scheduler,
	EffectScheduler,
	LogFlush,
	DeadConnections,
	Discord,
	SaveFlush,
	Pathfinding,
	LazyLoad
}

public readonly record struct RuntimeNetworkPerformanceSnapshot(
	long AcceptedConnections,
	long FloodRejectedConnections,
	long ActiveConnections,
	long BytesReceived,
	long BytesSent,
	long ReadOperations,
	long WriteOperations,
	long InputQueueHighWatermark,
	long OutputQueueHighWatermarkBytes,
	long SlowClientDisconnects,
	long AcceptErrors,
	long ReadErrors,
	long WriteErrors);

public interface IRuntimeNetworkPerformanceSource
{
	RuntimeNetworkPerformanceSnapshot GetNetworkPerformanceSnapshot();
	void ResetNetworkPerformanceCounters();
}

public enum RuntimeSchedulerKind
{
	Main,
	Effect
}

/// <summary>
/// Collects opt-in, bounded runtime performance statistics for a gameworld.
/// </summary>
public interface IRuntimePerformanceMonitor
{
	bool Enabled { get; }
	void Enable();
	void Disable();
	void Reset();
	void RecordLoopPhase(RuntimeLoopPhase phase, long elapsedStopwatchTicks, long allocatedBytes);
	void RecordLoopIteration(long elapsedStopwatchTicks, long allocatedBytes, bool overrun);
	void RecordSchedulerCheck(RuntimeSchedulerKind kind, int queueLength, int fired, int overdue,
		long elapsedStopwatchTicks);
	void RecordHeartbeatCallback(string cadence, Delegate callback, long elapsedStopwatchTicks);
	void AppendReport(StringBuilder sb, IFormatProvider formatProvider);
}

/// <summary>
/// Provides opt-in runtime performance diagnostics without adding a member to IFuturemud.
/// </summary>
public interface IRuntimePerformanceMonitorProvider
{
	IRuntimePerformanceMonitor RuntimePerformanceMonitor { get; }
}
