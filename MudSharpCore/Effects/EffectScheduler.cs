using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using MudSharp.Framework;
using MudSharp.Framework.Scheduling;
using MudSharp.ThirdPartyCode;

namespace MudSharp.Effects;

public class EffectScheduler : IScheduler, IHaveFuturemud, IEffectScheduler
{
	protected Dictionary<IEffect, IEffectSchedule> _scheduleMap = new();
	private List<(DateTime DateTime, IEffectSchedule Schedule)> _schedules = new();

	protected class ScheduleComparer : IComparer<(DateTime DateTime, IEffectSchedule Schedule)>
	{
		public int Compare((DateTime DateTime, IEffectSchedule Schedule) x,
			(DateTime DateTime, IEffectSchedule Schedule) y)
		{
			return DateTime.Compare(x.DateTime, y.DateTime);
		}
	}

	public IFuturemud Gameworld { get; protected set; }

	public EffectScheduler(IFuturemud gameworld)
	{
		Gameworld = gameworld;
	}

	public void Destroy(IFrameworkItem item)
	{
		if (item is IPerceivable itemAsPerceivable)
		{
			Destroy(itemAsPerceivable);
		}
	}

	public void Destroy(IFrameworkItem item, ScheduleType type)
	{
		if (type != ScheduleType.Effect)
		{
			return;
		}

		if (item is IPerceivable itemAsPerceivable)
		{
			Destroy(itemAsPerceivable);
		}
	}

	public TimeSpan RemainingDuration(IFrameworkItem item, ScheduleType type)
	{
		var schedule = _schedules.FirstOrDefault(x => x.Schedule.PertainsTo(item, type));
		if (schedule.Equals(default))
		{
			return TimeSpan.MinValue;
		}

		return schedule.Schedule.TriggerETA - DateTime.UtcNow;
	}

	public TimeSpan OriginalDuration(IFrameworkItem item, ScheduleType type)
	{
		var schedule = _schedules.FirstOrDefault(x => x.Schedule.PertainsTo(item, type));
		return schedule.Equals(default) ? TimeSpan.MinValue : schedule.Schedule.Duration;
	}

	public void AddSchedule(ISchedule schedule)
	{
		// Definitely want this to throw a cast exception if someone adds a non effect schedule
		AddSchedule((EffectSchedule)schedule);
	}

	public void AddOrDelaySchedule(ISchedule schedule, IFrameworkItem item)
	{
		if (_schedules.Any(x => x.Schedule.PertainsTo(item, schedule.Type)))
		{
			DelaySchedule(_schedules.FirstOrDefault(x => x.Schedule.PertainsTo(item, schedule.Type)).Schedule,
				schedule.Duration);
			return;
		}

		AddSchedule(schedule);
	}

	public void DelaySchedule(ISchedule schedule, TimeSpan delay)
	{
		schedule.TriggerETA += delay;
		_schedules.RemoveAll(x => x.Schedule == schedule);
		AddSchedule(schedule);
	}

       public void DelayScheduleType(IFrameworkItem item, ScheduleType type, TimeSpan delay)
       {
               if (delay.Ticks <= 0)
               {
                       return;
               }

               foreach (var sched in _schedules.Where(x => x.Schedule.PertainsTo(item, type)).ToArray())
               {
                       DelaySchedule(sched.Schedule, delay);
               }
       }

       public void CheckSchedules()
       {
               var sw = new Stopwatch();
               while (_schedules.Any())
               {
                       var now = DateTime.UtcNow;
                       if (now < _schedules.First().DateTime)
                       {
                               break;
                       }

                       var (trigger, schedule) = _schedules.First();
                       _schedules.RemoveAt(0);
			if (DateTime.UtcNow - trigger > TimeSpan.FromSeconds(10))
			{
				Console.ForegroundColor = ConsoleColor.Yellow;
				Console.WriteLine(
					$"[PERF] Effect Schedule was {(DateTime.UtcNow - trigger).TotalSeconds:N2}s overdue: {schedule.DebugInfoString.RawText()}");
				Console.ResetColor();
			}

			_scheduleMap.Remove(schedule.Effect);
			sw.Restart();
			schedule.Fire();
			sw.Stop();
			if (sw.ElapsedMilliseconds > 100)
			{
				Console.ForegroundColor = ConsoleColor.DarkYellow;
				Console.WriteLine(
					$"[PERF] Effect Schedule took {sw.ElapsedMilliseconds / 1000.0:N2}s to fire: {schedule.DebugInfoString.RawText()}");
				Console.ResetColor();
			}
		}
	}

	/// <summary>
	///     Requests the EffectScheduler to remove all effects pertaining to the specified IPerceivable.
	///     Should be called both when players leave the game world (e.g. quit) and when something is destroyed (e.g. junked
	///     item)
	/// </summary>
	/// <param name="target">The IPerceivable to remove effects for</param>
	/// <param name="save">Whether or not to invoke the EffectSchedule's Save routine</param>
	public void Destroy(IPerceivable target, bool save = false, bool fireRemovalAction = false)
	{
		foreach (var schedule in _schedules.Where(x => x.Schedule.Effect.Owner == target).ToArray())
		{
			_schedules.Remove(schedule);
			_scheduleMap.Remove(schedule.Schedule.Effect);

			if (fireRemovalAction)
			{
				schedule.Schedule.Effect.ExpireEffect();
			}

			if (save)
			{
				schedule.Schedule.Save();
			}
		}
	}

	public TimeSpan RemainingDuration(IEffect effect)
	{
		return IsScheduled(effect) ? _scheduleMap[effect].TriggerETA - DateTime.UtcNow : TimeSpan.Zero;
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

		_schedules.RemoveAll(x => x.Schedule.Effect == effect);
		_scheduleMap.Remove(effect);

		if (fireExpireAction)
		{
			effect.ExpireEffect();
		}
	}

       public void AddSchedule(IEffectSchedule schedule)
       {
               var entry = (schedule.TriggerETA, schedule);
               var comparer = new ScheduleComparer();
               var index = _schedules.BinarySearch(entry, comparer);
               if (index < 0)
               {
                       index = ~index;
               }
               _schedules.Insert(index, entry);
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

		if (_scheduleMap[effect].TriggerETA - DateTime.UtcNow < newTimespan)
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
		foreach (var (trigger, schedule) in _schedules)
		{
			sb.AppendLine(
				$"\t{(trigger - DateTime.UtcNow).TotalSeconds:N3}s - {schedule.Type.DescribeEnum()} - {schedule.DebugInfoString}");
		}
	}

	public void SetupEffectSaver()
	{
		Gameworld.Scheduler.AddSchedule(new RepeatingSchedule(Gameworld, () => SaveScheduledEffectDurations(),
			ScheduleType.EffectSaving, TimeSpan.FromSeconds(60), "SaveScheduledEffectDurations"));
	}

	public void SaveScheduledEffectDurations()
	{
		foreach (var owner in _scheduleMap.Keys.Where(x => x.SavingEffect).Select(x => x.Owner).Distinct())
		{
			owner.EffectsChanged = true;
		}
	}
}