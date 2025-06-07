using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using MudSharp.ThirdPartyCode;

namespace MudSharp.Framework.Scheduling;

public class Scheduler : IScheduler
{
       protected List<(DateTime DateTime, ISchedule Schedule)> _schedules = new();

	protected class ScheduleComparer : IComparer<(DateTime DateTime, ISchedule Schedule)>
	{
		public int Compare((DateTime DateTime, ISchedule Schedule) x, (DateTime DateTime, ISchedule Schedule) y)
		{
			return DateTime.Compare(x.DateTime, y.DateTime);
		}
	}

       public void AddSchedule(ISchedule schedule)
       {
               var entry = (schedule.TriggerETA, schedule);
               var comparer = new ScheduleComparer();
               var index = _schedules.BinarySearch(entry, comparer);
               if (index < 0)
               {
                       index = ~index;
               }
               _schedules.Insert(index, entry);
       }

	public void AddOrDelaySchedule(ISchedule schedule, IFrameworkItem item)
	{
		if (_schedules.Any(x => x.Schedule.PertainsTo(item, schedule.Type)))
		{
			DelaySchedule(_schedules.First(x => x.Schedule.PertainsTo(item, schedule.Type)).Schedule,
				schedule.Duration);
			return;
		}

		AddSchedule(schedule);
	}

	public void DelaySchedule(ISchedule schedule, TimeSpan delay)
	{
		schedule.TriggerETA = schedule.TriggerETA + delay;
		_schedules.RemoveAll(x => x.Schedule == schedule);
		AddSchedule(schedule);
	}

       public void DelayScheduleType(IFrameworkItem item, ScheduleType type, TimeSpan delay)
       {
               if (delay.Ticks <= 0)
               {
                       return;
               }

               foreach (var schedule in _schedules.Where(x => x.Schedule.PertainsTo(item, type)).ToArray())
               {
                       _schedules.Remove(schedule);
                       schedule.Schedule.TriggerETA += delay;
                       AddSchedule(schedule.Schedule);
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
                                       $"[PERF] Schedule was {(DateTime.UtcNow - trigger).TotalSeconds:N2}s overdue: {schedule.DebugInfoString.RawText()}");
				Console.ResetColor();
			}

                       sw.Restart();
                       schedule.Fire();
                       sw.Stop();
			if (sw.ElapsedMilliseconds > 100)
			{
				Console.ForegroundColor = ConsoleColor.DarkYellow;
				Console.WriteLine(
					$"[PERF] Schedule took {sw.ElapsedMilliseconds / 1000.0:N2}s to fire: {schedule.DebugInfoString.RawText()}");
				Console.ResetColor();
			}
		}
	}

	public void Destroy(IFrameworkItem item)
	{
		_schedules.RemoveAll(x => x.Schedule.PertainsTo(item));
	}

	public void Destroy(IFrameworkItem item, ScheduleType type)
	{
		_schedules.RemoveAll(x => x.Schedule.PertainsTo(item, type));
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

	public void DebugOutputForScheduler(StringBuilder sb)
	{
		sb.AppendLine("Schedules:");
		foreach (var (trigger, schedule) in _schedules)
		{
			sb.AppendLine(
				$"\t{(trigger - DateTime.UtcNow).TotalSeconds.ToString("N3")}s - {schedule.Type.DescribeEnum()} - {schedule.DebugInfoString}");
		}
	}
}