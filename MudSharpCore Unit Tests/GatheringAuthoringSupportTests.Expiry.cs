#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Effects;
using MudSharp.Effects.Concrete;
using MudSharp.Framework.Scheduling;
using MudSharp.FutureProg;
using MudSharp.FutureProg.Functions.Effects;
using MudSharp.PerceptionEngine;

namespace MudSharp_Unit_Tests;

public partial class GatheringAuthoringSupportTests
{
	private const string AshText = "Grey, lifeless ash collects on the ground.";
	private const string ExpiryText = "The grey ash on the ground dissipates.";

	[TestMethod]
	public void DescriptionExpiry_NativeSchedulerRemovesAtDeadlineAndAnnouncesOnce()
	{
		var f = ExpiryFixture();
		using var clock = RuntimeClock.Push(f.Clock);
		Assert.AreEqual(StatementResult.Normal, TimedFunction(f, f.Clock.Now.UtcDateTime.AddSeconds(270)).Execute(new VariableSpace()));
		var effect = f.Data.Effects.OfType<DescriptionAddition>().Single();
		f.Clock.Advance(TimeSpan.FromSeconds(269));
		f.Scheduler.CheckSchedules();
		Assert.AreSame(effect, f.Data.Effects.Single());
		f.Output.Verify(x => x.Send(It.IsAny<string>(), true, false), Times.Never);
		f.Clock.Advance(TimeSpan.FromSeconds(1));
		f.Scheduler.CheckSchedules();
		f.Scheduler.CheckSchedules();
		effect.ExpireEffect();
		Assert.AreEqual(0, f.Data.Effects.Count);
		Assert.AreEqual(0, f.Scheduler.ScheduleCount);
		f.Output.Verify(x => x.Send(ExpiryText, true, false), Times.Once);
	}

	[TestMethod]
	public void DescriptionExpiry_ExtensionRetainsOneEffectAndOldDeadlineCannotRemoveIt()
	{
		var f = ExpiryFixture();
		using var clock = RuntimeClock.Push(f.Clock);
		var firstDeadline = f.Clock.Now.UtcDateTime.AddSeconds(270);
		Assert.AreEqual(StatementResult.Normal, TimedFunction(f, firstDeadline).Execute(new VariableSpace()));
		var effect = f.Data.Effects.OfType<DescriptionAddition>().Single();
		f.Clock.Advance(TimeSpan.FromSeconds(20));
		var extended = firstDeadline.AddSeconds(540);
		Assert.AreEqual(StatementResult.Normal, TimedFunction(f, extended).Execute(new VariableSpace()));
		Assert.AreSame(effect, f.Data.Effects.Single());
		Assert.AreEqual(extended, effect.ExpiresUtc);
		Assert.AreEqual(1, f.Scheduler.ScheduleCount);
		f.Clock.Advance(TimeSpan.FromSeconds(250));
		effect.ExpireEffect(); // A stale queued callback must obey the current deadline.
		Assert.AreSame(effect, f.Data.Effects.Single());
		f.Output.Verify(x => x.Send(It.IsAny<string>(), true, false), Times.Never);
		f.Clock.Advance(TimeSpan.FromSeconds(540));
		f.Scheduler.CheckSchedules();
		Assert.AreEqual(0, f.Data.Effects.Count);
		f.Output.Verify(x => x.Send(ExpiryText, true, false), Times.Once);
		f.Data.Owner.Verify(x => x.AddEffect(It.IsAny<IEffect>()), Times.Once);
	}

	[TestMethod]
	public void DescriptionExpiry_ColdRoundTripCountsOfflineTimeAndOverridesCachedDuration()
	{
		var f = ExpiryFixture();
		using var clock = RuntimeClock.Push(f.Clock);
		var deadline = f.Clock.Now.UtcDateTime.AddSeconds(270);
		Assert.AreEqual(StatementResult.Normal, TimedFunction(f, deadline).Execute(new VariableSpace()));
		var effect = f.Data.Effects.OfType<DescriptionAddition>().Single();
		var xml = effect.SaveToXml(new Dictionary<IEffect, TimeSpan> { [effect] = TimeSpan.FromSeconds(270) });
		f.Data.Owner.Object.RemoveEffect(effect);
		f.Clock.Advance(TimeSpan.FromSeconds(90));
		var loaded = new DescriptionAddition(xml, f.Data.Owner.Object);
		Assert.IsFalse(loaded.LoadErrors);
		Assert.AreEqual(deadline, loaded.ExpiresUtc);
		f.Data.Owner.Object.AddEffect(loaded);
		f.Scheduler.Reschedule(loaded, TimeSpan.FromMinutes(20)); // Native cache restores before Login.
		loaded.Login();
		Assert.AreEqual(TimeSpan.FromSeconds(180), f.Scheduler.RemainingDuration(loaded));
		f.Clock.Advance(TimeSpan.FromSeconds(180));
		f.Scheduler.CheckSchedules();
		Assert.AreEqual(0, f.Data.Effects.Count);
		f.Output.Verify(x => x.Send(ExpiryText, true, false), Times.Once);
	}

	[TestMethod]
	public void DescriptionExpiry_OverdueColdStorageRetiresWithoutReplayingAnnouncement()
	{
		var f = ExpiryFixture();
		using var clock = RuntimeClock.Push(f.Clock);
		Assert.AreEqual(StatementResult.Normal, TimedFunction(f, f.Clock.Now.UtcDateTime.AddSeconds(270)).Execute(new VariableSpace()));
		var effect = f.Data.Effects.OfType<DescriptionAddition>().Single();
		var xml = effect.SaveToXml(new Dictionary<IEffect, TimeSpan> { [effect] = TimeSpan.FromSeconds(270) });
		f.Data.Owner.Object.RemoveEffect(effect);
		f.Clock.Advance(TimeSpan.FromSeconds(271));
		var loaded = new DescriptionAddition(xml, f.Data.Owner.Object);
		Assert.IsTrue(loaded.LoadErrors, "The native loader must omit overdue cold storage before restoring cached schedules.");
		Assert.AreEqual(0, f.Data.Effects.Count);
		Assert.AreEqual(0, f.Scheduler.ScheduleCount);
		f.Output.Verify(x => x.Send(It.IsAny<string>(), true, false), Times.Never);
	}

	[TestMethod]
	public void DescriptionExpiry_ConflictingMessageOrPastDeadlinePreservesRetainedEffect()
	{
		var f = ExpiryFixture();
		using var clock = RuntimeClock.Push(f.Clock);
		var deadline = f.Clock.Now.UtcDateTime.AddSeconds(270);
		Assert.AreEqual(StatementResult.Normal, TimedFunction(f, deadline).Execute(new VariableSpace()));
		var effect = f.Data.Effects.OfType<DescriptionAddition>().Single();
		Assert.AreEqual(StatementResult.Error, TimedFunction(f, deadline.AddMinutes(10), "Builder's different message.").Execute(new VariableSpace()));
		Assert.AreEqual(StatementResult.Error, TimedFunction(f, f.Clock.Now.UtcDateTime.AddSeconds(-1)).Execute(new VariableSpace()));
		Assert.AreSame(effect, f.Data.Effects.Single());
		Assert.AreEqual(deadline, effect.ExpiresUtc);
		Assert.AreEqual(ExpiryText, effect.ExpirationMessage);
		Assert.AreEqual(1, f.Scheduler.ScheduleCount);
	}

	[TestMethod]
	public void DescriptionExpiry_CompilesTimedAndLegacyPublicContracts()
	{
		FutureProgTestBootstrap.EnsureInitialised();
		var prog = new FutureProg(FutureProgTestBootstrap.Gameworld, "timed_description_contract", ProgVariableTypes.Boolean,
			new[] { Tuple.Create(ProgVariableTypes.Location, "room") },
			"return adddescriptionaddition(@room, \"Ash covers the ground.\", 71, now() + timespanfromseconds(270), \"The ash dissipates.\")");
		Assert.IsTrue(prog.Compile(), prog.CompileError);
	}

	private static AddDescriptionAdditionFunction TimedFunction(DescriptionExpiryFixture fixture, DateTime deadline, string message = ExpiryText) =>
		new([Constant(fixture.Data.Owner.Object, ProgVariableTypes.Location), Constant(AshText, ProgVariableTypes.Text),
			Constant(71.0M, ProgVariableTypes.Number), Constant(deadline, ProgVariableTypes.DateTime),
			Constant(message, ProgVariableTypes.Text)], fixture.Data.World.Object);

	private static DescriptionExpiryFixture ExpiryFixture()
	{
		var data = DescriptionFixture();
		var time = new ExpiryClock(new DateTimeOffset(2042, 3, 4, 5, 6, 7, TimeSpan.Zero));
		var scheduler = new EffectScheduler(data.World.Object, time);
		var output = new Mock<IOutputHandler>();
		data.World.SetupGet(x => x.EffectScheduler).Returns(scheduler);
		data.Owner.SetupGet(x => x.OutputHandler).Returns(output.Object);
		data.Owner.Setup(x => x.AddEffect(It.IsAny<IEffect>())).Callback<IEffect>(effect =>
		{
			data.Effects.Add(effect);
			effect.InitialEffect();
		});
		data.Owner.Setup(x => x.RemoveEffect(It.IsAny<IEffect>(), It.IsAny<bool>())).Callback<IEffect, bool>((effect, _) =>
		{
			data.Effects.Remove(effect);
			scheduler.Unschedule(effect);
		});
		return new(data, time, scheduler, output);
	}

	private sealed record DescriptionExpiryFixture(
		(Moq.Mock<MudSharp.Framework.IPerceivable> Owner, Moq.Mock<MudSharp.Framework.IFuturemud> World,
			Moq.Mock<IFutureProg> Prog, List<IEffect> Effects) Data,
		ExpiryClock Clock, EffectScheduler Scheduler, Mock<IOutputHandler> Output);

	private sealed class ExpiryClock(DateTimeOffset now) : TimeProvider
	{
		public DateTimeOffset Now { get; private set; } = now;
		public override DateTimeOffset GetUtcNow() => Now;
		public void Advance(TimeSpan duration) => Now += duration;
	}
}
