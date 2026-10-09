#nullable enable

using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Character;
using MudSharp.Combat;
using MudSharp.Construction;
using MudSharp.Effects;
using MudSharp.Effects.Concrete;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework;
using MudSharp.Magic;

namespace MudSharp_Unit_Tests;

[TestClass]
public class SpellNpcGuardianTests
{
	[TestMethod]
	public void Guardian_SaveLoadAndLiveReferences_RetainOnlyExplicitCreatorLinksWithoutResolution()
	{
		var world = new Mock<IFuturemud>(MockBehavior.Strict);
		var creator = new Mock<ICharacter>(); creator.SetupGet(x => x.Id).Returns(17); creator.SetupGet(x => x.InstanceId).Returns(23);
		var owner = new Mock<ICharacter>(); owner.SetupGet(x => x.Gameworld).Returns(world.Object);
		var effect = new SpellNpcGuardian(owner.Object, creator.Object);
		var saved = effect.SaveToXml(new Dictionary<IEffect, System.TimeSpan>());
		SpellNpcGuardian.InitialiseEffectType();
		// Effect's base loader reads this catalogue, but must not load the creator or enumerate actors.
		world.SetupGet(x => x.FutureProgs).Returns(Mock.Of<IUneditableAll<MudSharp.FutureProg.IFutureProg>>());
		var loaded = (SpellNpcGuardian)Effect.LoadEffect(saved, owner.Object);
		Assert.IsTrue(loaded.SavingEffect);
		Assert.IsTrue(XNode.DeepEquals(saved, loaded.SaveToXml(new())));
		var live = ((IPhysicalEntityReferenceProvider)loaded).PhysicalReferences.ToArray();
		var stored = PhysicalReferenceCodecs.Effects(new XElement("Effects", saved).ToString()).ToArray();
		foreach (var refs in new[] { live, stored })
		{
			Assert.AreEqual(2, refs.Length);
			Assert.IsTrue(refs.Any(x => x.Kind == PhysicalEntityKind.Character && x.Id == 17));
			Assert.IsTrue(refs.Any(x => x.Kind == PhysicalEntityKind.CharacterInstance && x.Id == 23));
		}
	}

	[TestMethod]
	public void Guardian_Targets_OnlyExactLoadedLivingCreatorInstance()
	{
		var world = new Mock<IFuturemud>();
		ICharacter Actor(long id, long instance, CharacterState state = CharacterState.Awake) =>
			Mock.Of<ICharacter>(x => x.Id == id && x.InstanceId == instance && x.State == state);
		var creator = Actor(17, 23); var otherInstance = Actor(17, 24); var otherActor = Actor(23, 17);
		world.SetupGet(x => x.Actors).Returns(MagicCastingFixture.Collection(() => new[] { creator, otherInstance, otherActor }));
		var guardian = Mock.Of<ICharacter>(x => x.Gameworld == world.Object);
		var bond = new SpellNpcGuardian(guardian, creator);
		CollectionAssert.AreEqual(new[] { creator }, bond.Targets.ToArray());
		Mock.Get(creator).SetupGet(x => x.State).Returns(CharacterState.Dead);
		Assert.IsFalse(bond.Targets.Any());
	}

	[TestMethod]
	public void Stock_Definition_GradeCountCreatorBondAndDefaultDissipationAreExplicit()
	{
		var xml = ArmageddonAirGuardianStock.Definition(1, 2, 3, "120*grade");
		var lifecycle = xml.Element("Effects")!.Element("Effect")!.Element("Lifecycle")!;
		Assert.AreEqual("grade", lifecycle.Element("Count")!.Value);
		Assert.AreEqual("true", lifecycle.Element("GuardCaster")!.Value);
		Assert.AreEqual("TemporaryCleanup", lifecycle.Attribute("mode")!.Value);
		Assert.AreEqual("120*grade", lifecycle.Element("Seconds")!.Value);
		Assert.AreEqual("room", xml.Element("Trigger")!.Attribute("type")!.Value);
	}

	[DataTestMethod]
	[DataRow(true)]
	[DataRow(false)]
	public void Restart_SavedSuspensionOrDurableRetirementIntent_PreventsBondReactivation(bool savedSuspension)
	{
		var world = new Mock<IFuturemud>();
		world.SetupGet(x => x.FutureProgs).Returns(Mock.Of<IUneditableAll<MudSharp.FutureProg.IFutureProg>>());
		var creator = Mock.Of<ICharacter>(x => x.Id == 17 && x.InstanceId == 23);
		var guardian = new Mock<ICharacter>(); guardian.SetupGet(x => x.Gameworld).Returns(world.Object);
		guardian.SetupGet(x => x.State).Returns(CharacterState.Awake);
		ICharacter? following = creator;
		guardian.SetupGet(x => x.Following).Returns(() => following);
		guardian.Setup(x => x.CeaseFollowing()).Callback(() => following = null);
		var lifecycle = new Mock<ISpellOwnedNpcService>();
		lifecycle.Setup(x => x.HasPendingRetirement(guardian.Object, 17)).Returns(true);
		world.SetupGet(x => x.SpellOwnedNpcs).Returns(lifecycle.Object);
		var bond = new SpellNpcGuardian(guardian.Object, creator);
		if (savedSuspension) Assert.IsTrue(bond.PrepareRetirement(17));
		var saved = bond.SaveToXml(new());
		SpellNpcGuardian.InitialiseEffectType();
		var restored = (SpellNpcGuardian)Effect.LoadEffect(saved, guardian.Object);
		restored.InitialEffect(); restored.Login();
		Assert.IsFalse(restored.Targets.Any());
		Assert.IsNull(following);
		guardian.Verify(x => x.Follow(It.IsAny<ICharacter>()), Times.Never);
		guardian.Verify(x => x.CeaseFollowing(), Times.Once);
		guardian.VerifySet(x => x.EffectsChanged = true, Times.Once);
		lifecycle.Verify(x => x.HasPendingRetirement(guardian.Object, 17), savedSuspension ? Times.Never() : Times.Once());
	}

	[DataTestMethod]
	[DataRow(true)]
	[DataRow(false)]
	public void Retirement_ReleasesOnlyExactOwnedFollowAndStopsEngagement(bool ownFollow)
	{
		var world = new Mock<IFuturemud>(); var room = new Mock<IRoom>();
		var creator = Mock.Of<ICharacter>(x => x.Id == 17 && x.InstanceId == 23 && x.State == CharacterState.Awake);
		var guardian = new Mock<ICharacter>(); guardian.SetupGet(x => x.Gameworld).Returns(world.Object);
		guardian.SetupGet(x => x.State).Returns(CharacterState.Awake);
		ICharacter? following = ownFollow ? creator : Mock.Of<ICharacter>(x => x.Id == 17 && x.InstanceId == 24);
		guardian.SetupGet(x => x.Following).Returns(() => following);
		guardian.Setup(x => x.CeaseFollowing()).Callback(() => following = null);
		var bond = new SpellNpcGuardian(guardian.Object, creator);
		Assert.IsFalse(bond.PrepareRetirement(99));
		guardian.Verify(x => x.CeaseFollowing(), Times.Never);
		Assert.IsTrue(bond.PrepareRetirement(17));
		Assert.IsFalse(bond.Targets.Any());
		guardian.Verify(x => x.CeaseFollowing(), ownFollow ? Times.Once() : Times.Never());
		Assert.AreEqual(!ownFollow, following is not null);
		world.SetupGet(x => x.Actors).Returns(MagicCastingFixture.Collection(() => new[] { creator }));
		Mock.Get(creator).SetupGet(x => x.Location).Returns(room.Object);
		guardian.Setup(x => x.EffectsOfType<SpellNpcGuardian>(It.IsAny<System.Predicate<SpellNpcGuardian>>())).Returns(new[] { bond });
		room.Setup(x => x.LayerCharacters(It.IsAny<RoomLayer>())).Returns(new[] { creator, guardian.Object });
		var attacker = Mock.Of<ICharacter>(x => x.CombatTarget == creator);
		SpellNpcGuardian.NotifyEngagement(attacker, creator);
		guardian.Verify(x => x.Engage(It.IsAny<ICharacter>(), It.IsAny<bool>(), It.IsAny<bool>()), Times.Never);
	}

	[DataTestMethod]
	[DataRow(false, false)]
	[DataRow(true, false)]
	[DataRow(false, true)]
	[DataRow(true, true)]
	public void Engagement_RespondsImmediatelyEvenWhileGuardianFightsElsewhere(bool occupied, bool ranged)
	{
		var world = new Mock<IFuturemud>(); var room = new Mock<IRoom>();
		var creator = new Mock<ICharacter>(); creator.SetupGet(x => x.Id).Returns(17); creator.SetupGet(x => x.InstanceId).Returns(23);
		creator.SetupGet(x => x.Location).Returns(room.Object);
		world.SetupGet(x => x.Actors).Returns(MagicCastingFixture.Collection(() => new[] { creator.Object }));
		var guardian = new Mock<ICharacter>(); guardian.SetupGet(x => x.Gameworld).Returns(world.Object);
		guardian.SetupGet(x => x.State).Returns(CharacterState.Awake);
		var attacker = new Mock<ICharacter>(); attacker.SetupGet(x => x.CombatTarget).Returns(creator.Object);
		guardian.Setup(x => x.ColocatedWith(creator.Object)).Returns(true);
		guardian.Setup(x => x.ColocatedWith(attacker.Object)).Returns(!ranged);
		guardian.Setup(x => x.CanSee(attacker.Object, It.IsAny<PerceiveIgnoreFlags>())).Returns(true);
		guardian.Setup(x => x.CanEngage(attacker.Object)).Returns(true);
		if (occupied) guardian.SetupGet(x => x.Combat).Returns(Mock.Of<ICombat>());
		var bond = new SpellNpcGuardian(guardian.Object, creator.Object);
		guardian.Setup(x => x.EffectsOfType<SpellNpcGuardian>(It.IsAny<System.Predicate<SpellNpcGuardian>>())).Returns(new[] { bond });
		room.Setup(x => x.LayerCharacters(It.IsAny<RoomLayer>())).Returns(new[] { creator.Object, guardian.Object });
		SpellNpcGuardian.NotifyEngagement(attacker.Object, creator.Object);
		guardian.Verify(x => x.Engage(attacker.Object, ranged, false), Times.Once);
	}
}
