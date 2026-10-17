#nullable enable

using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Effects;
using MudSharp.Effects.Concrete;
using MudSharp.Framework;
using MudSharp.Framework.Scheduling;
using MudSharp.GameItems;
using MudSharp.PerceptionEngine;

namespace MudSharp_Unit_Tests;

public partial class GatheringAuthoringSupportTests
{
	[TestMethod]
	public void DescriptionExpiry_ItemMorphDurationPath_PreservesDeadlineWithOneCallback()
	{
		var time = new ExpiryClock(new DateTimeOffset(2042, 3, 4, 5, 6, 7, TimeSpan.Zero));
		using var clock = RuntimeClock.Push(time);
		var world = new Mock<IFuturemud>();
		var scheduler = new EffectScheduler(world.Object, time);
		world.SetupGet(x => x.EffectScheduler).Returns(scheduler);
		var oldItem = new Mock<IGameItem>();
		var newItem = new Mock<IGameItem>();
		var oldHandler = new EffectHandler(oldItem.Object);
		var newHandler = new EffectHandler(newItem.Object);
		oldItem.SetupGet(x => x.Gameworld).Returns(world.Object);
		newItem.SetupGet(x => x.Gameworld).Returns(world.Object);
		oldItem.SetupGet(x => x.Effects).Returns(() => oldHandler.Effects);
		newItem.SetupGet(x => x.Effects).Returns(() => newHandler.Effects);
		newItem.Setup(x => x.RemoveEffect(It.IsAny<IEffect>(), It.IsAny<bool>()))
			.Callback<IEffect, bool>((effect, fire) => newHandler.RemoveEffect(effect, fire));
		var output = new Mock<IOutputHandler>();
		newItem.SetupGet(x => x.OutputHandler).Returns(output.Object);
		var original = new DescriptionAddition(oldItem.Object, AshText, false, null!);
		var deadline = time.Now.UtcDateTime.AddSeconds(270);
		original.SetExpiration(deadline, ExpiryText);
		oldHandler.AddEffect(original);
		time.Advance(TimeSpan.FromSeconds(90));

		// GameItem.Morph transfers a scheduled effect through this exact native duration overload.
		var transferred = (DescriptionAddition)original.NewEffectOnItemMorph(oldItem.Object, newItem.Object);
		newHandler.AddEffect(transferred, scheduler.RemainingDuration(original));
		oldHandler.RemoveEffect(original);
		Assert.AreEqual(deadline, transferred.ExpiresUtc);
		Assert.AreEqual(1, scheduler.ScheduleCount, "The copied effect must have one callback, including its InitialEffect schedule.");
		time.Advance(TimeSpan.FromSeconds(180));
		scheduler.CheckSchedules();
		Assert.AreEqual(1, scheduler.LastCheckFiredCount);
		Assert.AreEqual(0, scheduler.ScheduleCount);
		Assert.IsFalse(newHandler.Effects.Any());
		output.Verify(x => x.Send(ExpiryText, true, false), Times.Once);
	}
}
