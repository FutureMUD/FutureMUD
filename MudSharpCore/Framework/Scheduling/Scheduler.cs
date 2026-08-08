using System.Diagnostics;
using MudSharp.Framework.Diagnostics;

namespace MudSharp.Framework.Scheduling;

public class Scheduler : IScheduler
{
	private readonly StableScheduleHeap<ISchedule> _schedules = new();
	private readonly TimeProvider _timeProvider;
	private readonly IRuntimePerformanceMonitor _performanceMonitor;

	public Scheduler(TimeProvider timeProvider = null, IRuntimePerformanceMonitor performanceMonitor = null)
	{
		_timeProvider = timeProvider ?? TimeProvider.System;
		_performanceMonitor = performanceMonitor;
	}

	public void AddSchedule(ISchedule schedule)
	{
		_schedules.Add(schedule.TriggerETA, schedule);
	}

	public void AddOrDelaySchedule(ISchedule schedule, IFrameworkItem item)
	{
		var existing = _schedules.Find(x => x.PertainsTo(item, schedule.Type));
		if (existing is not null)
		{
			DelaySchedule(existing, schedule.Duration);
			return;
		}

		AddSchedule(schedule);
	}

	public void DelaySchedule(ISchedule schedule, TimeSpan delay)
	{
		schedule.TriggerETA += delay;
		_schedules.RemoveAll(x => ReferenceEquals(x, schedule));
		AddSchedule(schedule);
	}

	public void DelayScheduleType(IFrameworkItem item, ScheduleType type, TimeSpan delay)
	{
		if (delay.Ticks <= 0)
		{
			return;
		}

		_schedules.UpdateAll(
			x => x.PertainsTo(item, type),
			x => x.TriggerETA += delay,
			x => x.TriggerETA);
	}

	public void CheckSchedules()
	{
		var started = Stopwatch.GetTimestamp();
		var fired = 0;
		var overdue = 0;
		while (_schedules.TryPeek(out var next) && UtcNow >= next.TriggerUtc)
		{
			_schedules.TryDequeue(out var entry);
			var schedule = entry.Schedule;
			var now = UtcNow;
			if (now - entry.TriggerUtc > TimeSpan.FromSeconds(10))
			{
				overdue++;
				Console.ForegroundColor = ConsoleColor.Yellow;
				Console.WriteLine(
					$"[PERF] Schedule was {(now - entry.TriggerUtc).TotalSeconds:N2}s overdue: {schedule.DebugInfoString.RawText()}");
				Console.ResetColor();
			}

			var fireStarted = Stopwatch.GetTimestamp();
			schedule.Fire();
			var fireElapsed = Stopwatch.GetElapsedTime(fireStarted);
			if (fireElapsed.TotalMilliseconds > 100)
			{
				Console.ForegroundColor = ConsoleColor.DarkYellow;
				Console.WriteLine(
					$"[PERF] Schedule took {fireElapsed.TotalSeconds:N2}s to fire: {schedule.DebugInfoString.RawText()}");
				Console.ResetColor();
			}

			fired++;
		}

		_performanceMonitor?.RecordSchedulerCheck(RuntimeSchedulerKind.Main, _schedules.Count, fired, overdue,
			Stopwatch.GetTimestamp() - started);
	}

	public void Destroy(IFrameworkItem item)
	{
		_schedules.RemoveAll(x => x.PertainsTo(item));
	}

	public void Destroy(IFrameworkItem item, ScheduleType type)
	{
		_schedules.RemoveAll(x => x.PertainsTo(item, type));
	}

	public TimeSpan RemainingDuration(IFrameworkItem item, ScheduleType type)
	{
		var schedule = _schedules.Find(x => x.PertainsTo(item, type));
		return schedule is null ? TimeSpan.MinValue : schedule.TriggerETA - UtcNow;
	}

	public TimeSpan OriginalDuration(IFrameworkItem item, ScheduleType type)
	{
		var schedule = _schedules.Find(x => x.PertainsTo(item, type));
		return schedule?.Duration ?? TimeSpan.MinValue;
	}

	public void DebugOutputForScheduler(StringBuilder sb)
	{
		sb.AppendLine("Schedules:");
		foreach (var entry in _schedules.SnapshotOrdered())
		{
			sb.AppendLine(
				$"\t{(entry.TriggerUtc - UtcNow).TotalSeconds:N3}s - {entry.Schedule.Type.DescribeEnum()} - {entry.Schedule.DebugInfoString}");
		}
	}

	private DateTime UtcNow => _timeProvider.GetUtcNow().UtcDateTime;
}
