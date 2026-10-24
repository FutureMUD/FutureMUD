#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Body;
using MudSharp.Body.Traits;
using MudSharp.Character;
using MudSharp.Construction.Boundary;
using MudSharp.Effects;
using MudSharp.Effects.Concrete;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework;
using MudSharp.Magic;
using MudSharp.Magic.Powers;
using MudSharp.Models;
using MudSharp.RPG.Checks;
using MudSharp.Planes;

namespace MudSharp_Unit_Tests;

public partial class MagicPsionicTraceTests
{
	[TestMethod]
	[DataRow(true)]
	[DataRow(false)]
	public void MentalSpeech_ChargesConfiguredInvocationCostForSuccessAndFailure(bool success)
	{
		var world = CreateGameworld();
		var resource = Mock.Of<IMagicResource>(x => x.Id == 1 && x.Name == "Focus");
		world.SetupGet(x => x.MagicResources).Returns(CreateCollectionMock(resource).Object);
		world.SetupGet(x => x.MagicCapabilities).Returns(new All<IMagicCapability>());
		world.Setup(x => x.GetStaticInt("MaximumSayLength")).Returns(1000);
		var room = CreateRoom(1, world.Object);
		var actor = CreateCharacter(1, "actor", world.Object, room.Object);
		actor.SetupGet(x => x.State).Returns(CharacterState.Awake);
		actor.SetupGet(x => x.IsEngagedInMelee).Returns(true);
		var target = CreateCharacter(2, "target", world.Object, room.Object);
		actor.SetupGet(x => x.MagicResourceAmounts).Returns(new Dictionary<IMagicResource, double> { [resource] = 10 });
		actor.Setup(x => x.CanUseResource(resource, 1)).Returns(true);
		actor.Setup(x => x.UseResource(resource, 1)).Returns(true);
		var connection = new ConnectMindEffect(actor.Object, target.Object, (ConnectMindPower)MagicPowerFactory.LoadPower(CreateConnectMindPowerModel(), world.Object));
		actor.Setup(x => x.EffectsOfType<ConnectMindEffect>(It.IsAny<Predicate<ConnectMindEffect>>())).Returns([connection]);
		world.Setup(x => x.GetCheck(CheckType.MindSayPower)).Returns(CreateCheck(CheckType.MindSayPower, _ => success).Object);
		var definition = PsionicStockContent.Definition(PsionicStockContent.Powers.Single(x => x.Verb == "say"), 1, 1, 0, 1, 0, 0);
		definition.Element("PsionicTrace")?.Remove();
		var power = MagicPowerFactory.LoadPower(new MagicPower { Id = 300, Name = "Speech", Blurb = "Speech", ShowHelp = "Speech",
			MagicSchoolId = 1, PowerModel = "mindsay", Definition = definition.ToString() }, world.Object);
		power.UseCommand(actor.Object, "say", new StringStack("hello"));
		actor.Verify(x => x.UseResource(resource, 1), Times.Once);
		connection.ReleaseEvents(); connection.ChildEffect.ReleaseEvents();
	}

	private static T LoadArmageddonSustained<T>(IFuturemud world, string model, string verb, long id) where T : class, IMagicPower =>
		(T)MagicPowerFactory.LoadPower(new MagicPower { Id = id, Name = verb, Blurb = verb, ShowHelp = verb,
			MagicSchoolId = 1, PowerModel = model, Definition = SustainedDefinition(verb, "end" + verb, false).ToString() }, world);

	[TestMethod]
	public void SensePresence_MovementUsesBonusUnlessSpottingIsSuppressed()
	{
		var world = CreateGameworld();
		var room = CreateRoom(1, world.Object);
		var mover = CreateCharacter(1, "mover", world.Object, room.Object);
		mover.Setup(x => x.Equals(It.IsAny<ICharacter>())).Returns<ICharacter>(x => ReferenceEquals(x, mover.Object));
		var observer = CreateCharacter(2, "observer", world.Object, room.Object);
		var power = LoadArmageddonSustained<PsionicSensePresencePower>(world.Object, "psionicsensepresence", "sensepresence", 200);
		observer.Setup(x => x.TraitValue(power.SkillCheckTrait, It.IsAny<TraitBonusContext>())).Returns(45);
		var presence = new PsionicSensePresenceEffect(observer.Object, power);
		observer.Setup(x => x.CombinedEffectsOfType<PsionicSensePresenceEffect>()).Returns([presence]);
		mover.SetupGet(x => x.Capabilities).Returns([Mock.Of<IMagicCapability>(x => x.AllPowers == new[] { power })]);
		mover.Setup(x => x.AffectedBy<Immwalk>()).Returns(true);
		var movement = new MudSharp.Movement.Movement(mover.Object, null!, [], [], [mover.Object], [], [], [], Mock.Of<IRoomExit>());
		var sneak = new Mock<ISneakMoveEffect>();
		sneak.SetupGet(x => x.StealthOutcome).Returns(CheckOutcome.SimpleOutcome(CheckType.SneakCheck, Outcome.Pass));
		movement.SneakMoveEffects[mover.Object] = sneak.Object;
		Assert.AreEqual(1, movement.CharacterMovers.Count());
		Assert.AreSame(sneak.Object, movement.SneakMoveEffects[mover.Object]);
		Assert.AreNotSame(mover.Object, observer.Object);
		var check = new Mock<ICheck>();
		check.Setup(x => x.Check(observer.Object, It.IsAny<Difficulty>(), mover.Object, It.IsAny<IUseTrait>(), 45,
			It.IsAny<TraitUseType>(), It.IsAny<(string Parameter, object value)[]>())).Returns(CheckOutcome.SimpleOutcome(CheckType.SpotSneakCheck, Outcome.MajorPass));
		world.Setup(x => x.GetCheck(CheckType.SpotSneakCheck)).Returns(check.Object);
		movement.SeenBy(observer.Object);
		check.Verify(x => x.Check(observer.Object, It.IsAny<Difficulty>(), mover.Object, It.IsAny<IUseTrait>(), 45,
			It.IsAny<TraitUseType>(), It.IsAny<(string Parameter, object value)[]>()), Times.Once);
		movement.SeenBy(observer.Object, PerceiveIgnoreFlags.IgnoreSpotting);
		check.Verify(x => x.Check(observer.Object, It.IsAny<Difficulty>(), mover.Object, It.IsAny<IUseTrait>(), It.IsAny<double>(),
			It.IsAny<TraitUseType>(), It.IsAny<(string Parameter, object value)[]>()), Times.Once);
		mover.SetupGet(x => x.Capabilities).Returns(Array.Empty<IMagicCapability>());
		Assert.AreEqual(0, PsionicSensePresenceEffect.SneakSpottingBonus(observer.Object, mover.Object));
		presence.RemovalEffect();
	}

	[TestMethod]
	public void Vanish_RechecksRangeAndPresenceBeforeCachedObserverContest()
	{
		var world = CreateGameworld();
		var room = CreateRoom(1, world.Object);
		var otherRoom = CreateRoom(2, world.Object);
		var owner = CreateCharacter(1, "owner", world.Object, room.Object);
		var observer = CreateCharacter(2, "observer", world.Object, room.Object);
		owner.SetupGet(x => x.Body).Returns(Mock.Of<IBody>(x => x.BasePlanarPresence == PlanarPresenceDefinition.DefaultMaterial(1)));
		observer.SetupGet(x => x.Body).Returns(Mock.Of<IBody>(x => x.BasePlanarPresence == PlanarPresenceDefinition.DefaultMaterial(1)));
		var power = LoadArmageddonSustained<PsionicVanishPower>(world.Object, "psionicvanish", "vanish", 200);
		var presencePower = LoadArmageddonSustained<PsionicSensePresencePower>(world.Object, "psionicsensepresence", "sensepresence", 201);
		var check = CreateCheck(CheckType.MagicTelepathyCheck, _ => true);
		check.Setup(x => x.Check(observer.Object, It.IsAny<Difficulty>(), owner.Object, It.IsAny<IUseTrait>(), It.IsAny<double>(),
			It.IsAny<TraitUseType>(), It.IsAny<(string Parameter, object value)[]>())).Returns(CheckOutcome.SimpleOutcome(CheckType.MagicTelepathyCheck, Outcome.Fail));
		world.Setup(x => x.GetCheck(CheckType.MagicTelepathyCheck)).Returns(check.Object);
		var effect = new PsionicVanishEffect(owner.Object, power, MagicPowerDistance.SameLocationOnly);
		Assert.IsTrue(effect.Applies(observer.Object, PerceiveIgnoreFlags.None));
		Assert.IsTrue(effect.Applies(observer.Object, PerceiveIgnoreFlags.None));
		check.Verify(x => x.Check(observer.Object, It.IsAny<Difficulty>(), owner.Object, It.IsAny<IUseTrait>(), It.IsAny<double>(),
			It.IsAny<TraitUseType>(), It.IsAny<(string Parameter, object value)[]>()), Times.Once);
		Assert.IsFalse(effect.Applies(owner.Object, PerceiveIgnoreFlags.None));
		Assert.IsFalse(effect.Applies(observer.Object, PerceiveIgnoreFlags.IgnoreHiding));
		observer.SetupGet(x => x.Location).Returns(otherRoom.Object);
		Assert.IsFalse(effect.Applies(observer.Object, PerceiveIgnoreFlags.None));
		observer.SetupGet(x => x.Location).Returns(room.Object);
		var presence = new PsionicSensePresenceEffect(observer.Object, presencePower);
		observer.Setup(x => x.CombinedEffectsOfType<PsionicSensePresenceEffect>()).Returns([presence]);
		Assert.IsFalse(effect.Applies(observer.Object, PerceiveIgnoreFlags.None));
		var all = new All<IMagicPower>(); all.Add(power); all.Add(presencePower);
		world.SetupGet(x => x.MagicPowers).Returns(all);
		PsionicVanishEffect.InitialiseEffectType();
		var saved = effect.SaveToXml(new Dictionary<IEffect, TimeSpan> { [effect] = TimeSpan.FromMinutes(3) });
		var loaded = (PsionicVanishEffect)Effect.LoadEffect(saved, owner.Object);
		Assert.AreEqual(MagicPowerDistance.SameLocationOnly, loaded.Distance);
		Assert.AreSame(power, loaded.Power);
		effect.RemovalEffect(); presence.RemovalEffect();
	}

	[TestMethod]
	public void Vanish_InvalidOrUnlearnedScopeDoesNotCreateAnEffect()
	{
		var world = CreateGameworld();
		var actor = CreateCharacter(1, "actor", world.Object, CreateRoom(1, world.Object).Object);
		var power = LoadArmageddonSustained<PsionicVanishPower>(world.Object, "psionicvanish", "vanish", 200);
		actor.Setup(x => x.TraitValue(power.SkillCheckTrait, It.IsAny<TraitBonusContext>())).Returns(20);
		power.UseCommand(actor.Object, "vanish", new StringStack("far"));
		power.UseCommand(actor.Object, "vanish", new StringStack("everywhere"));
		actor.Verify(x => x.AddEffect(It.IsAny<IEffect>(), It.IsAny<TimeSpan>()), Times.Never);
	}
}
