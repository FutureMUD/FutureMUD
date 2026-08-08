using System.Diagnostics;
using MudSharp.Framework.Diagnostics;
using MudSharp.Framework.Scheduling;

namespace MudSharp.Effects;

public class EffectScheduler : IScheduler, IHaveFuturemud, IEffectScheduler
{
	protected readonly Dictionary<IEffect, IEffectSchedule> _scheduleMap = new();
	private readonly StableScheduleHeap<IEffectSchedule> _schedules = new();
	private readonly TimeProvider _timeProvider;

	public IFuturemud Gameworld { get; protected set; }

	public EffectScheduler(IFuturemud gameworld, TimeProvider timeProvider = null)
	{
		Gameworld = gameworld;
		_timeProvider = timeProvider ?? TimeProvider.System;
	}

	public void Destroy(IFrameworkItem item)
	{
		if (item is IPerceivable perceivable)
		{
			Destroy(perceivable);
		}
	}

	public void Destroy(IFrameworkItem item, ScheduleType type)
	{
		if (type == ScheduleType.Effect && item is IPerceivable perceivable)
		{
			Destroy(perceivable);
		}
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

	public void AddSchedule(ISchedule schedule)
	{
		AddSchedule((EffectSchedule)schedule);
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
					$"[PERF] Effect Schedule was {(now - entry.TriggerUtc).TotalSeconds:N2}s overdue: {schedule.DebugInfoString.RawText()}");
				Console.ResetColor();
			}

			_scheduleMap.Remove(schedule.Effect);
			var fireStarted = Stopwatch.GetTimestamp();
			schedule.Fire();
			var fireElapsed = Stopwatch.GetElapsedTime(fireStarted);
			if (fireElapsed.TotalMilliseconds > 100)
			{
				Console.ForegroundColor = ConsoleColor.DarkYellow;
				Console.WriteLine(
					$"[PERF] Effect Schedule took {fireElapsed.TotalSeconds:N2}s to fire: {schedule.DebugInfoString.RawText()}");
				Console.ResetColor();
			}

			fired++;
		}

		(Gameworld as IRuntimePerformanceMonitorProvider)?.RuntimePerformanceMonitor.RecordSchedulerCheck(
			RuntimeSchedulerKind.Effect,
			_schedules.Count,
			fired,
			overdue,
			Stopwatch.GetTimestamp() - started);
	}

	/// <summary>
	/// Requests removal of all effects pertaining to the specified perceivable.
	/// </summary>
	public void Destroy(IPerceivable target, bool save = false, bool fireRemovalAction = false)
	{
		var schedules = _schedules
			.SnapshotOrdered()
			.Where(x => x.Schedule.Effect.Owner == target)
			.Select(x => x.Schedule)
			.ToList();
		if (schedules.Count == 0)
		{
			return;
		}

		_schedules.RemoveAll(x => x.Effect.Owner == target);
		foreach (var schedule in schedules)
		{
			_scheduleMap.Remove(schedule.Effect);
			if (fireRemovalAction)
			{
				schedule.Effect.ExpireEffect();
			}

			if (save)
			{
				schedule.Save();
			}
		}
	}

	public TimeSpan RemainingDuration(IEffect effect)
	{
		return IsScheduled(effect) ? _scheduleMap[effect].TriggerETA - UtcNow : TimeSpan.Zero;
	}

	public TimeSpan OriginalDuration(IEffect effect)
	{
		return IsScheduled(effect) ? _scheduleMap[effect].Duration : TimeSpan.Zero;
	}

	public void Unschedule(IEffect effect, bool fireExpireAction = false, bool fireRemovalAction = false)
	{
		if (!IsScheduled(effect))
		{
			return;
		}

		_schedules.RemoveAll(x => x.Effect == effect);
		_scheduleMap.Remove(effect);
		if (fireExpireAction)
		{
			effect.ExpireEffect();
		}
	}

	public void AddSchedule(IEffectSchedule schedule)
	{
		_schedules.Add(schedule.TriggerETA, schedule);
		_scheduleMap[schedule.Effect] = schedule;
	}

	public bool IsScheduled(IEffect effect)
	{
		return _scheduleMap.ContainsKey(effect);
	}

	public void ExtendSchedule(IEffect effect, TimeSpan extension)
	{
		var schedule = _scheduleMap[effect];
		Unschedule(effect);
		schedule.ExtendDuration(extension);
		AddSchedule(schedule);
	}

	public void Reschedule(IEffect effect, TimeSpan newTimespan)
	{
		Unschedule(effect);
		AddSchedule(new EffectSchedule(effect, newTimespan));
	}

	public void RescheduleIfLonger(IEffect effect, TimeSpan newTimespan)
	{
		if (!IsScheduled(effect))
		{
			AddSchedule(new EffectSchedule(effect, newTimespan));
			return;
		}

		if (_scheduleMap[effect].TriggerETA - UtcNow < newTimespan)
		{
			Unschedule(effect);
			AddSchedule(new EffectSchedule(effect, newTimespan));
		}
	}

	public string Describe(IEffect effect, IPerceiver voyeur)
	{
		return IsScheduled(effect) ? _scheduleMap[effect].Describe(voyeur) : effect.Describe(voyeur);
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

	public void SetupEffectSaver()
	{
		Gameworld.Scheduler.AddSchedule(new RepeatingSchedule(Gameworld, SaveScheduledEffectDurations,
			ScheduleType.EffectSaving, TimeSpan.FromSeconds(60), "SaveScheduledEffectDurations"));
	}

	public void SaveScheduledEffectDurations()
	{
		foreach (var owner in _scheduleMap.Keys
		         .Where(x => x.SavingEffect)
		         .Select(x => x.Owner)
		         .Distinct())
		{
			owner.EffectsChanged = true;
		}
	}

	private DateTime UtcNow => _timeProvider.GetUtcNow().UtcDateTime;
}
