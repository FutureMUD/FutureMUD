#nullable enable

using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Effects;
using MudSharp.Effects.Concrete;
using MudSharp.Effects.Concrete.SpellEffects;
using MudSharp.Events;

namespace MudSharp_Unit_Tests;

[TestClass]
public class ThresholdAlarmTests
{
	[TestMethod]
	public void ThresholdAlarm_NativeEntryWitnessNotifiesOnlyLoadedOwnerWithoutEntrantIdentity()
	{
		var fixture = new MagicCastingFixture();
		var room = new Mock<IRoom>();
		var effects = new List<IEffect>();
		room.SetupGet(x => x.Gameworld).Returns(fixture.World.Object);
		room.SetupGet(x => x.Effects).Returns(() => effects);
		fixture.Actor.SetupGet(x => x.Location).Returns(room.Object);
		fixture.World.SetupGet(x => x.Characters).Returns(MagicCastingFixture.Collection<ICharacter>(() => [fixture.Actor.Object]));
		var entrant = new Mock<ICharacter>();
		entrant.SetupGet(x => x.Id).Returns(987);
		entrant.SetupGet(x => x.Location).Returns(room.Object);
		var parent = new MagicSpellParent(room.Object, fixture.Spell, fixture.Actor.Object);
		var alarm = new SpellThresholdAlarmEffect(room.Object, parent, fixture.Actor.Object);
		parent.AddSpellEffect(alarm);
		effects.Add(parent);
		effects.Add(alarm);
		fixture.Messages.Clear();

		Assert.IsTrue(alarm.IsActive);
		Assert.IsTrue(alarm.HandlesEvent(EventType.CharacterEnterRoomWitness));
		Assert.IsFalse(alarm.HandlesEvent(EventType.CharacterEnterRoomFinish));
		alarm.HandleEvent(EventType.CharacterEnterRoomFinish, entrant.Object, room.Object);
		alarm.HandleEvent(EventType.CharacterEnterRoomWitness, fixture.Actor.Object, room.Object);
		Assert.AreEqual(0, fixture.Messages.Count);
		alarm.HandleEvent(EventType.CharacterEnterRoomWitness, entrant.Object, room.Object);
		Assert.AreEqual(1, fixture.Messages.Count);
		StringAssert.Contains(fixture.Messages[0], "threshold alarm");
		Assert.IsFalse(fixture.Messages[0].Contains("987"));
		effects.Remove(parent);
		alarm.HandleEvent(EventType.CharacterEnterRoomWitness, entrant.Object, room.Object);
		Assert.AreEqual(1, fixture.Messages.Count);
		effects.Add(parent);
		fixture.World.SetupGet(x => x.Characters).Returns(MagicCastingFixture.Collection<ICharacter>(() => []));
		fixture.World.SetupGet(x => x.Actors).Returns(MagicCastingFixture.Collection<ICharacter>(() => []));
		alarm.HandleEvent(EventType.CharacterEnterRoomWitness, entrant.Object, room.Object);
		Assert.AreEqual(1, fixture.Messages.Count, "Offline owners do not receive a queued or leaked notice.");
	}
}
