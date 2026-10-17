#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Effects;
using MudSharp.Effects.Concrete;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework;
using MudSharp.Framework.Scheduling;
using MudSharp.PerceptionEngine;

namespace MudSharp_Unit_Tests;

public partial class GatheringAuthoringSupportTests
{
	[TestMethod]
	[DataRow(0, false)]
	[DataRow(2, false)]
	[DataRow(0, true)]
	[DataRow(2, true)]
	public void DescriptionExpiry_RoomHasNoOutputHandler_BroadcastsOnceToEachOccupant(int occupants, bool routed)
	{
		var clock = new ExpiryClock(new DateTimeOffset(2042, 3, 4, 5, 6, 7, TimeSpan.Zero));
		using var scope = RuntimeClock.Push(clock);
		var world = new Mock<IFuturemud>();
		var room = new Mock<IRoom>();
		if (routed) room.SetupGet(x => x.RouteDefinition).Returns(new Mock<IRouteRoomDefinition>().Object);
		var effects = new List<IEffect>();
		var scheduler = new EffectScheduler(world.Object, clock);
		world.SetupGet(x => x.EffectScheduler).Returns(scheduler);
		room.SetupGet(x => x.Gameworld).Returns(world.Object);
		room.SetupGet(x => x.Effects).Returns(effects);
		room.SetupGet(x => x.Rooms).Returns([room.Object]);
		room.Setup(x => x.EffectsOfType<IRemoteObservationEffect>()).Returns([]);
		var outputs = Enumerable.Range(0, occupants).Select(_ => new Mock<IOutputHandler>()).ToArray();
		var characters = outputs.Select(output =>
		{
			var character = new Mock<ICharacter>();
			character.SetupGet(x => x.OutputHandler).Returns(output.Object);
			character.Setup(x => x.EffectsOfType<IRemoteObservationEffect>()).Returns([]);
			return character.Object;
		}).ToArray();
		room.SetupGet(x => x.Characters).Returns(characters);
		room.Setup(x => x.RemoveEffect(It.IsAny<IEffect>(), It.IsAny<bool>())).Callback<IEffect, bool>((effect, _) =>
		{
			effects.Remove(effect);
			scheduler.Unschedule(effect);
		});
		var effect = new DescriptionAddition(room.Object, AshText, false, null!);
		effect.SetExpiration(clock.Now.UtcDateTime.AddSeconds(270), ExpiryText);
		effects.Add(effect);
		effect.InitialEffect();
		clock.Advance(TimeSpan.FromSeconds(270));
		scheduler.CheckSchedules();
		effect.ExpireEffect();
		Assert.AreEqual(0, effects.Count);
		Assert.AreEqual(0, scheduler.ScheduleCount);
		foreach (var output in outputs) output.Verify(x => x.Send(ExpiryText, true, false), Times.Once);
		room.Verify(x => x.HandleRoomEcho(ExpiryText, null), Times.Once);
		room.VerifyGet(x => x.OutputHandler, Times.Never);
	}
}
