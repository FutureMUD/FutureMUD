using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Effects;
using MudSharp.Framework;
using MudSharp.Framework.Scheduling;
using System;
using System.Collections.Generic;

namespace MudSharp_Unit_Tests;

[TestClass]
public class RuntimeSchedulerTests
{
	[TestMethod]
	public void CheckSchedules_EqualTriggerTimes_FiresInInsertionOrder()
	{
		var time = new ManualTimeProvider(DateTimeOffset.UtcNow);
		var scheduler = new Scheduler(time);
		var fired = new List<string>();
		var trigger = time.GetUtcNow().UtcDateTime;

		scheduler.AddSchedule(CreateSchedule("first", trigger, fired));
		scheduler.AddSchedule(CreateSchedule("second", trigger, fired));
		scheduler.AddSchedule(CreateSchedule("third", trigger, fired));

		scheduler.CheckSchedules();

		CollectionAssert.AreEqual(new[] { "first", "second", "third" }, fired);
	}

	[TestMethod]
	public void SchedulerDiagnostics_QueuedAndFiredSchedules_ReportNextTriggerAndCount()
	{
		var now = DateTimeOffset.UtcNow;
		var time = new ManualTimeProvider(now);
		var scheduler = new Scheduler(time);
		var fired = new List<string>();
		var trigger = now.UtcDateTime.AddSeconds(2);
		scheduler.AddSchedule(CreateSchedule("first", trigger, fired));
		scheduler.AddSchedule(CreateSchedule("second", trigger, fired));

		Assert.AreEqual(trigger, scheduler.NextTriggerUtc);
		time.Advance(TimeSpan.FromSeconds(2));
		scheduler.CheckSchedules();

		Assert.AreEqual(2, scheduler.LastCheckFiredCount);
		Assert.IsNull(scheduler.NextTriggerUtc);
	}

	[TestMethod]
	public void ScheduleBase_AmbientRuntimeClock_UsesVirtualCreationTime()
	{
		var virtualNow = new DateTimeOffset(2042, 3, 4, 5, 6, 7, TimeSpan.Zero);
		var time = new ManualTimeProvider(virtualNow);
		using var scope = RuntimeClock.Push(time);

		var schedule = new Schedule(() => { }, ScheduleType.System, TimeSpan.FromSeconds(3), "virtual");

		Assert.AreEqual(virtualNow.UtcDateTime, schedule.CreatedAt);
		Assert.AreEqual(virtualNow.UtcDateTime.AddSeconds(3), schedule.TriggerETA);
	}

	[TestMethod]
	public void CheckSchedules_CallbackAddsDueSchedule_FiresAddedScheduleInSameCheck()
	{
		var time = new ManualTimeProvider(DateTimeOffset.UtcNow);
		var scheduler = new Scheduler(time);
		var fired = new List<string>();
		var trigger = time.GetUtcNow().UtcDateTime;
		var added = CreateSchedule("added", trigger, fired);
		var initial = new Schedule(() =>
		{
			fired.Add("initial");
			scheduler.AddSchedule(added);
		}, ScheduleType.System, TimeSpan.Zero, "initial")
		{
			TriggerETA = trigger
		};

		scheduler.AddSchedule(initial);
		scheduler.CheckSchedules();

		CollectionAssert.AreEqual(new[] { "initial", "added" }, fired);
	}

	[TestMethod]
	public void CheckSchedules_RepeatingScheduleAfterStall_FiresEveryMissedTick()
	{
		var time = new ManualTimeProvider(DateTimeOffset.UtcNow);
		var scheduler = new Scheduler(time);
		var gameworld = new Mock<IFuturemud>();
		gameworld.SetupGet(x => x.Scheduler).Returns(scheduler);
		var count = 0;
		var schedule = new RepeatingSchedule(gameworld.Object, () => count++, ScheduleType.System,
			TimeSpan.FromSeconds(1), "repeat")
		{
			TriggerETA = time.GetUtcNow().UtcDateTime.AddSeconds(1)
		};
		scheduler.AddSchedule(schedule);

		time.Advance(TimeSpan.FromSeconds(3));
		scheduler.CheckSchedules();

		Assert.AreEqual(3, count);
	}

	[TestMethod]
	public void DelayScheduleType_RebuildsHeapAndPreservesRemainingDuration()
	{
		var time = new ManualTimeProvider(DateTimeOffset.UtcNow);
		var scheduler = new Scheduler(time);
		var item = new Mock<IFrameworkItem>().Object;
		var count = 0;
		var schedule = new Schedule<IFrameworkItem>(item, _ => count++, ScheduleType.System,
			TimeSpan.FromSeconds(1), "delay")
		{
			TriggerETA = time.GetUtcNow().UtcDateTime.AddSeconds(1)
		};
		scheduler.AddSchedule(schedule);

		scheduler.DelayScheduleType(item, ScheduleType.System, TimeSpan.FromSeconds(2));
		Assert.AreEqual(TimeSpan.FromSeconds(3), scheduler.RemainingDuration(item, ScheduleType.System));

		time.Advance(TimeSpan.FromSeconds(2));
		scheduler.CheckSchedules();
		Assert.AreEqual(0, count);

		time.Advance(TimeSpan.FromSeconds(1));
		scheduler.CheckSchedules();
		Assert.AreEqual(1, count);
	}

	[TestMethod]
	public void Destroy_RemovesEveryScheduleForTheItem()
	{
		var time = new ManualTimeProvider(DateTimeOffset.UtcNow);
		var scheduler = new Scheduler(time);
		var item = new Mock<IFrameworkItem>().Object;
		var count = 0;
		var first = new Schedule<IFrameworkItem>(item, _ => count++, ScheduleType.System,
			TimeSpan.FromSeconds(1), "first") { TriggerETA = time.GetUtcNow().UtcDateTime };
		var second = new Schedule<IFrameworkItem>(item, _ => count++, ScheduleType.System,
			TimeSpan.FromSeconds(2), "second") { TriggerETA = time.GetUtcNow().UtcDateTime };
		scheduler.AddSchedule(first);
		scheduler.AddSchedule(second);

		scheduler.Destroy(item);
		scheduler.CheckSchedules();

		Assert.AreEqual(0, count);
		Assert.AreEqual(TimeSpan.MinValue, scheduler.RemainingDuration(item, ScheduleType.System));
	}

	[TestMethod]
	public void AddOrDelaySchedule_DelaysTheEarliestMatchingSchedule()
	{
		var time = new ManualTimeProvider(DateTimeOffset.UtcNow);
		var scheduler = new Scheduler(time);
		var item = new Mock<IFrameworkItem>().Object;
		var fired = new List<string>();
		var first = new Schedule<IFrameworkItem>(item, _ => fired.Add("first"), ScheduleType.System,
			TimeSpan.FromSeconds(1), "first") { TriggerETA = time.GetUtcNow().UtcDateTime.AddSeconds(1) };
		var second = new Schedule<IFrameworkItem>(item, _ => fired.Add("second"), ScheduleType.System,
			TimeSpan.FromSeconds(2), "second") { TriggerETA = time.GetUtcNow().UtcDateTime.AddSeconds(2) };
		scheduler.AddSchedule(first);
		scheduler.AddSchedule(second);

		scheduler.AddOrDelaySchedule(
			new Schedule<IFrameworkItem>(item, _ => fired.Add("new"), ScheduleType.System,
				TimeSpan.FromSeconds(4), "new"), item);

		time.Advance(TimeSpan.FromSeconds(2));
		scheduler.CheckSchedules();
		CollectionAssert.AreEqual(new[] { "second" }, fired);

		time.Advance(TimeSpan.FromSeconds(3));
		scheduler.CheckSchedules();
		CollectionAssert.AreEqual(new[] { "second", "first" }, fired);
	}

	[TestMethod]
	public void EffectScheduler_DueEffect_RemovesScheduleMapBeforeFiring()
	{
		var time = new ManualTimeProvider(DateTimeOffset.UtcNow);
		var effect = new Mock<IEffect>();
		var scheduler = new EffectScheduler(new Mock<IFuturemud>().Object, time);
		var schedule = new EffectSchedule(effect.Object, TimeSpan.Zero)
		{
			TriggerETA = time.GetUtcNow().UtcDateTime
		};
		scheduler.AddSchedule(schedule);

		scheduler.CheckSchedules();

		effect.Verify(x => x.ExpireEffect(), Times.Once);
		Assert.IsFalse(scheduler.IsScheduled(effect.Object));
	}

	[TestMethod]
	public void EffectScheduler_RescheduleAndUnschedule_KeepMapConsistent()
	{
		var effect = new Mock<IEffect>();
		var scheduler = new EffectScheduler(new Mock<IFuturemud>().Object);

		scheduler.Reschedule(effect.Object, TimeSpan.FromMinutes(1));
		Assert.IsTrue(scheduler.IsScheduled(effect.Object));
		scheduler.Reschedule(effect.Object, TimeSpan.FromMinutes(2));
		Assert.IsTrue(scheduler.IsScheduled(effect.Object));

		scheduler.Unschedule(effect.Object);
		Assert.IsFalse(scheduler.IsScheduled(effect.Object));
	}

	private static Schedule CreateSchedule(string name, DateTime trigger, ICollection<string> fired)
	{
		return new Schedule(() => fired.Add(name), ScheduleType.System, TimeSpan.Zero, name)
		{
			TriggerETA = trigger
		};
	}

	private sealed class ManualTimeProvider(DateTimeOffset now) : TimeProvider
	{
		private DateTimeOffset _now = now;

		public override DateTimeOffset GetUtcNow() => _now;

		public void Advance(TimeSpan duration)
		{
			_now += duration;
		}
	}
}
