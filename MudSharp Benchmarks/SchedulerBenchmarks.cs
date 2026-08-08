using BenchmarkDotNet.Attributes;
using MudSharp.Framework.Scheduling;

namespace MudSharp_Benchmarks;

[MemoryDiagnoser]
public class SchedulerBenchmarks
{
	private readonly List<Schedule> _schedules = [];

	[Params(100, 10_000, 100_000)]
	public int ScheduleCount { get; set; }

	[GlobalSetup]
	public void Setup()
	{
		_schedules.Clear();
		var trigger = DateTime.UtcNow.AddHours(1);
		for (var i = 0; i < ScheduleCount; i++)
		{
			_schedules.Add(new Schedule(() => { }, ScheduleType.System, TimeSpan.Zero, "benchmark")
			{
				TriggerETA = trigger.AddTicks(i % 1000)
			});
		}
	}

	[Benchmark]
	public int HeapEnqueue()
	{
		var scheduler = new Scheduler();
		foreach (var schedule in _schedules)
		{
			scheduler.AddSchedule(schedule);
		}

		return _schedules.Count;
	}

	[Benchmark(Baseline = true)]
	public int LegacySortedListEnqueue()
	{
		var schedules = new List<(DateTime Trigger, Schedule Schedule)>();
		foreach (var schedule in _schedules)
		{
			var index = schedules.BinarySearch((schedule.TriggerETA, schedule), ScheduleComparer.Instance);
			schedules.Insert(index < 0 ? ~index : index, (schedule.TriggerETA, schedule));
		}

		return schedules.Count;
	}

	private sealed class ScheduleComparer : IComparer<(DateTime Trigger, Schedule Schedule)>
	{
		public static ScheduleComparer Instance { get; } = new();

		public int Compare((DateTime Trigger, Schedule Schedule) x, (DateTime Trigger, Schedule Schedule) y)
		{
			return DateTime.Compare(x.Trigger, y.Trigger);
		}
	}
}
