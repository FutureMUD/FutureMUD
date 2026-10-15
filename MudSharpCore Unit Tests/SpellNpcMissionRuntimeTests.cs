#nullable enable

using System;
using System.Linq;
using System.Collections.Generic;
using System.Xml.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Body;
using MudSharp.Body.Traits;
using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Construction.Boundary;
using MudSharp.Effects.Concrete;
using MudSharp.Framework;
using MudSharp.Magic;
using MudSharp.Magic.Lifecycle;
using MudSharp.Framework.Scheduling;
using MudSharp.Effects;
using MudSharp.Movement;
using MudSharp.PerceptionEngine;
using MudSharp.RPG.Checks;

namespace MudSharp_Unit_Tests;

[TestClass]
public class SpellNpcMissionRuntimeTests
{
	[DataTestMethod]
	[DataRow("wrong-character", false)]
	[DataRow("wrong-instance", false)]
	[DataRow("ambiguous", false)]
	[DataRow("single", true)]
	[DataRow("duplicate-exit", true)]
	public void SeekingProcess_FollowsOnlyOneUnambiguousExactInstanceExit(string sample, bool moves)
	{
		var time = CelestialTestFactory.CreateEarthSystem();
		time.SetDateTime("1/jan/2026", 12, 0, 0);
		var world = new Mock<IFuturemud> { DefaultValue = DefaultValue.Mock };
		var creator = Mock.Of<ICharacter>(x => x.Id == 17 && x.InstanceId == 71);
		var target = Mock.Of<ICharacter>(x => x.Id == 18 && x.InstanceId == 72 && x.Body == Mock.Of<IBody>(b => b.Id == 8));
		var actors = new All<ICharacter>(); actors.Add(creator); actors.Add(target);
		world.SetupGet(x => x.Actors).Returns(actors);
		var service = new Mock<ISpellOwnedNpcService>();
		service.Setup(x => x.HasCommandAuthority(It.IsAny<ICharacter>(), creator, It.IsAny<Guid>(), 17, 71, 2, 1)).Returns(true);
		world.SetupGet(x => x.SpellOwnedNpcs).Returns(service.Object);
		var creature = new Mock<ICharacter>(); creature.SetupGet(x => x.Gameworld).Returns(world.Object);
		var room = new Mock<IRoom>(); creature.SetupGet(x => x.Location).Returns(room.Object);
		var first = Mock.Of<IRoomExit>(); var second = Mock.Of<IRoomExit>();
		ITrack Track(IRoomExit exit)
		{
			var track = new Mock<ITrack>();
			track.SetupGet(x => x.CharacterId).Returns(sample == "wrong-character" ? 99 : 18);
			track.SetupGet(x => x.CharacterInstanceId).Returns(sample == "wrong-instance" ? 99 : 72);
			track.SetupGet(x => x.ToRoomExit).Returns(exit);
			track.SetupGet(x => x.MudDateTime).Returns(time.Calendar.CurrentDateTime);
			track.Setup(x => x.VisualTrackDifficulty(creature.Object)).Returns(Difficulty.Normal);
			return track.Object;
		}
		room.SetupGet(x => x.Tracks).Returns(sample switch
		{
			"ambiguous" => [Track(first), Track(second)],
			"duplicate-exit" => [Track(first), Track(first)],
			_ => [Track(first)]
		});
		var check = new Mock<ICheck>();
		check.Setup(x => x.CheckAgainstAllDifficulties(creature.Object, Difficulty.Normal, null!, null!, 0.0,
			TraitUseType.Practical, It.IsAny<(string Parameter, object value)[]>()))
			.Returns(Enum.GetValues<Difficulty>().Distinct().ToDictionary(x => x,
				_ => CheckOutcome.SimpleOutcome(CheckType.SearchForTracksCheck, Outcome.MajorPass)));
		world.Setup(x => x.GetCheck(It.IsAny<CheckType>())).Returns(check.Object);
		creature.Setup(x => x.CanMove(It.IsAny<IRoomExit>(), CanMoveFlags.None)).Returns(CanMoveResponse.True);
		var control = new SpellNpcControlEffect(creature.Object, new SpellLifecycleOrigin(Guid.NewGuid(), 2, 1,
			creator.Id, "fixture", SpellLifecycleMode.TemporaryCleanup, RuntimeClock.UtcNow, RuntimeClock.UtcNow.AddMinutes(2)), creator);
		var mission = new SpellNpcMissionEffect(creature.Object, "seek", target);

		mission.Process(control);

		creature.Verify(x => x.Move(first, null, false), moves ? Times.Once() : Times.Never());
		creature.Verify(x => x.Move(second, It.IsAny<IEmote>(), It.IsAny<bool>()), Times.Never());
	}
	[TestMethod]
	public void HauntCleanup_RemovesOnlyTheExactSavedFearParent()
	{
		var world = new Mock<IFuturemud> { DefaultValue = DefaultValue.Mock };
		var owner = new Mock<ICharacter>(); owner.SetupGet(x => x.Gameworld).Returns(world.Object);
		var target = new Mock<ICharacter>(); target.SetupGet(x => x.Id).Returns(18);
		target.SetupGet(x => x.InstanceId).Returns(72); target.SetupGet(x => x.Body).Returns(Mock.Of<IBody>(x => x.Id == 8));
		var actors = new All<ICharacter>(); actors.Add(target.Object); world.SetupGet(x => x.Actors).Returns(actors);
		var parent = new MagicSpellParent(target.Object, Mock.Of<IMagicSpell>(), target.Object);
		var foreign = new MagicSpellParent(target.Object, Mock.Of<IMagicSpell>(), target.Object);
		target.Setup(x => x.EffectsOfType<MagicSpellParent>(It.IsAny<Predicate<MagicSpellParent>>())).Returns([foreign, parent]);
		var mission = new SpellNpcMissionEffect(owner.Object, "haunt", target.Object, expiryUtc: RuntimeClock.UtcNow.AddMinutes(2));
		var xml = mission.SaveToXml(new Dictionary<IEffect, TimeSpan>());
		xml.Element("Effect")!.Element("Phase")!.Value = "haunting";
		xml.Element("Effect")!.Add(new XElement("FearParentIdentity", parent.Identity));
		SpellNpcMissionEffect.InitialiseEffectType();
		var loaded = (SpellNpcMissionEffect)Effect.LoadEffect(xml, owner.Object);

		Assert.AreSame(target.Object, loaded.RemoveOwnedFearParent());

		target.Verify(x => x.RemoveEffect(parent, true), Times.Once());
		target.Verify(x => x.RemoveEffect(foreign, It.IsAny<bool>()), Times.Never());
		target.Verify(x => x.Save(), Times.Never());
	}

}
