#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;

using System.Reflection;
using System.Xml.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Character;
using MudSharp.Body;
using MudSharp.Effects;
using MudSharp.Effects.Concrete;
using MudSharp.Effects.Concrete.SpellEffects;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework;
using MudSharp.Magic;
using MudSharp.NPC.AI;
using MudSharp.Magic.Lifecycle;
using MudSharp.GameItems;
using MudSharp.Movement;
using MudSharp.Framework.Scheduling;
using MudSharp.RPG.Checks;
using MudSharp.Body.Traits;
using MudSharp.FutureProg;

namespace MudSharp_Unit_Tests;

[TestClass]
public class SpellNpcControlTests
{
	[TestMethod]
	[DataRow(false)]
	[DataRow(true)]
	public void Haunt_RefusesFearWhenResistanceCallbackMakesExactTargetInvisible(bool configuredResistance)
	{
		var world = new Mock<IFuturemud> { DefaultValue = DefaultValue.Mock };
		var schools = new All<IMagicSchool>(); schools.Add(Mock.Of<IMagicSchool>(x => x.Id == 1));
		world.SetupGet(x => x.MagicSchools).Returns(schools);
		world.SetupGet(x => x.FutureProgs).Returns(new All<IFutureProg>());
		var trait = configuredResistance ? Mock.Of<ITraitDefinition>(x => x.Id == 4) : null;
		var traits = new All<ITraitDefinition>();
		if (trait is not null) traits.Add(trait);
		world.SetupGet(x => x.Traits).Returns(traits);
		world.SetupGet(x => x.TraitExpressions).Returns(new All<ITraitExpression>());
		world.SetupGet(x => x.MagicResources).Returns(new All<IMagicResource>());
		var spell = new MagicSpell(new MudSharp.Models.MagicSpell
		{
			Id = 2, Name = "Isolated Haunt resistance fixture", MagicSchoolId = 1,
			ResistingTraitDefinitionId = configuredResistance ? 4 : null,
			ResistingDifficulty = configuredResistance ? (int)Difficulty.VeryHard : null,
			Definition = "<Definition><Trigger type='substancecharacter'/><Costs/><Effects/><CasterEffects/><Plan/></Definition>"
		}, world.Object);
		Assert.IsTrue(spell.ReadyForGame);
		var spells = new All<IMagicSpell>(); spells.Add(spell); world.SetupGet(x => x.MagicSpells).Returns(spells);
		var service = new Mock<ISpellOwnedNpcService>();
		service.Setup(x => x.HasCommandAuthority(It.IsAny<ICharacter>(), It.IsAny<ICharacter>(), It.IsAny<Guid>(),
			It.IsAny<long>(), It.IsAny<long>(), It.IsAny<long>(), It.IsAny<int>())).Returns(true);
		world.SetupGet(x => x.SpellOwnedNpcs).Returns(service.Object);
		var creature = new Mock<ICharacter>(); creature.SetupGet(x => x.Gameworld).Returns(world.Object);
		var creator = Mock.Of<ICharacter>(x => x.Id == 17 && x.InstanceId == 71);
		var target = new Mock<ICharacter>(); target.SetupGet(x => x.Id).Returns(18); target.SetupGet(x => x.InstanceId).Returns(72);
		target.SetupGet(x => x.Body).Returns(Mock.Of<IBody>(x => x.Id == 8));
		target.SetupGet(x => x.State).Returns(CharacterState.Mobile);
		var actors = new All<ICharacter>(); actors.Add(creator); actors.Add(target.Object); world.SetupGet(x => x.Actors).Returns(actors);
		var visible = true;
		creature.Setup(x => x.CanSee(target.Object)).Returns(() => visible);
		creature.Setup(x => x.ColocatedWith(target.Object)).Returns(true);
		var check = new Mock<ICheck>();
		var opposedDifficulty = configuredResistance ? Difficulty.VeryHard : Difficulty.Normal;
		check.Setup(x => x.Check(target.Object, opposedDifficulty, trait!, creator, 0.0,
			TraitUseType.Practical, It.IsAny<(string Parameter, object value)[]>()))
			.Callback(() => visible = false)
			.Returns(CheckOutcome.SimpleOutcome(CheckType.ResistMagicSpellCheck, Outcome.Fail));
		world.Setup(x => x.GetCheck(CheckType.ResistMagicSpellCheck)).Returns(check.Object);
		var deadline = RuntimeClock.UtcNow.AddMinutes(2);
		var control = new SpellNpcControlEffect(creature.Object, new SpellLifecycleOrigin(Guid.NewGuid(), spell.Id, 2,
			creator.Id, "fixture", SpellLifecycleMode.TemporaryCleanup, RuntimeClock.UtcNow, deadline), creator);
		var mission = new SpellNpcMissionEffect(creature.Object, "haunt", target.Object, expiryUtc: deadline);
		mission.Process(control);
		check.Verify(x => x.Check(target.Object, opposedDifficulty, trait!, creator, 0.0,
			TraitUseType.Practical, It.IsAny<(string Parameter, object value)[]>()), Times.Once);
		Assert.AreEqual("active", mission.Phase);
		Assert.IsNull(mission.FearParentIdentity);
		target.Verify(x => x.AddEffect(It.IsAny<IEffect>()), Times.Never);
		target.Verify(x => x.AddEffect(It.IsAny<IEffect>(), It.IsAny<TimeSpan>()), Times.Never);
	}

	[TestMethod]
	[DataRow(false)]
	[DataRow(true)]
	public void CachedFearRemoval_DoesNotIntroduceABroadSaveButPreservesPriorQueuedChanges(bool priorQueue)
	{
		var saveManager = new Mock<MudSharp.Framework.Save.ISaveManager>();
		var world = new Mock<IFuturemud>(); world.SetupGet(x => x.SaveManager).Returns(saveManager.Object);
		var target = new Mock<ICharacter>(); target.SetupGet(x => x.Gameworld).Returns(world.Object);
		target.SetupProperty(x => x.Changed, false); target.SetupProperty(x => x.EffectsChanged, false);
		saveManager.Setup(x => x.IsQueued(target.Object)).Returns(priorQueue);
		var parent = new MagicSpellParent(target.Object, Mock.Of<IMagicSpell>(), target.Object);
		target.Setup(x => x.RemoveEffect(parent, true)).Callback(() => { target.Object.Changed = true; target.Object.EffectsChanged = true; });
		SpellNpcControlEffect.RemoveCachedOwnedFearParent(target.Object, parent);
		saveManager.Verify(x => x.Abort(target.Object), priorQueue ? Times.Never() : Times.Once());
		Assert.AreEqual(priorQueue, target.Object.Changed);
		Assert.AreEqual(priorQueue, target.Object.EffectsChanged);
		target.Verify(x => x.Save(), Times.Never);
	}

	private sealed class FixedClock(DateTime now) : TimeProvider
	{
		public override DateTimeOffset GetUtcNow() => new(now);
	}

	[TestMethod]
	public void FearExpiry_UsesTheSameRuntimeClockAsTheOwningLifecycle()
	{
		var now = new DateTime(2000, 1, 1, 12, 0, 0, DateTimeKind.Utc);
		using var time = RuntimeClock.Push(new FixedClock(now));
		var scheduler = new Mock<IEffectScheduler>();
		var world = new Mock<IFuturemud>(); world.SetupGet(x => x.EffectScheduler).Returns(scheduler.Object);
		var owner = new Mock<ICharacter>(); owner.SetupGet(x => x.Gameworld).Returns(world.Object);
		var future = new SpellFearEffect(owner.Object, Mock.Of<IMagicSpellEffectParent>(), expiryUtc: now.AddMinutes(2));
		future.InitialEffect();
		scheduler.Verify(x => x.AddSchedule(It.Is<IEffectSchedule>(schedule => schedule.TriggerETA == now.AddMinutes(2))), Times.Once);
		owner.Verify(x => x.RemoveEffect(future, true), Times.Never);
		var expired = new SpellFearEffect(owner.Object, Mock.Of<IMagicSpellEffectParent>(), expiryUtc: now.AddSeconds(-1));
		expired.InitialEffect();
		owner.Verify(x => x.RemoveEffect(expired, true), Times.Once);
	}

	[TestMethod]
	public void MissionTarget_RefusesAChangedBodyOrLoggedOutPhysicalTarget()
	{
		var world = new Mock<IFuturemud>();
		var owner = Mock.Of<ICharacter>(x => x.Gameworld == world.Object);
		IBody body = Mock.Of<IBody>(x => x.Id == 8);
		var state = CharacterState.Mobile;
		var target = new Mock<ICharacter>(); target.SetupGet(x => x.Id).Returns(17); target.SetupGet(x => x.InstanceId).Returns(71);
		target.SetupGet(x => x.Body).Returns(() => body); target.SetupGet(x => x.State).Returns(() => state);
		var actors = new All<ICharacter>(); actors.Add(target.Object); world.SetupGet(x => x.Actors).Returns(actors);
		var mission = new SpellNpcMissionEffect(owner, "seek", target.Object);
		Assert.AreSame(target.Object, mission.ResolveTarget());
		body = Mock.Of<IBody>(x => x.Id == 9);
		Assert.IsNull(mission.ResolveTarget());
		body = Mock.Of<IBody>(x => x.Id == 8); state = CharacterState.Stasis;
		Assert.IsNull(mission.ResolveTarget());
	}

	[TestMethod]
	public void AttackOrder_SelfTargetRefusesBeforeSavingOrEngagementCallbacks()
	{
		var world = new Mock<IFuturemud>();
		var service = new Mock<ISpellOwnedNpcService>();
		service.Setup(x => x.HasCommandAuthority(It.IsAny<ICharacter>(), It.IsAny<ICharacter>(), It.IsAny<Guid>(),
			It.IsAny<long>(), It.IsAny<long>(), It.IsAny<long>(), It.IsAny<int>())).Returns(true);
		world.SetupGet(x => x.SpellOwnedNpcs).Returns(service.Object);
		var creature = new Mock<ICharacter>(); creature.SetupGet(x => x.Gameworld).Returns(world.Object);
		var creator = Mock.Of<ICharacter>(x => x.InstanceId == 23);
		var origin = new SpellLifecycleOrigin(Guid.NewGuid(), 7, 2, 17, "fixture", SpellLifecycleMode.TemporaryCleanup,
			DateTime.UtcNow, DateTime.UtcNow.AddMinutes(5));
		var effect = new SpellNpcControlEffect(creature.Object, origin, creator);
		Assert.IsFalse(effect.SetOrder("attack", creature.Object, creator));
		Assert.AreEqual("none", effect.Intent);
		creature.Verify(x => x.Save(), Times.Never);
		creature.Verify(x => x.CanEngage(It.IsAny<ICharacter>()), Times.Never);
	}

	[TestMethod]
	public void CurrentNpcPrimaryInstance_RejectsSecondaryOrAnotherLoadedInstanceOfSameBody()
	{
		Assert.IsTrue(SpellOwnedNpcService.IsExactCurrentNpcPrimary(31, 12, 44, true, 31, 12, 44));
		Assert.IsFalse(SpellOwnedNpcService.IsExactCurrentNpcPrimary(30, 12, 44, false, 31, 12, 44));
		Assert.IsFalse(SpellOwnedNpcService.IsExactCurrentNpcPrimary(30, 12, 44, true, 31, 12, 44));
		Assert.IsFalse(SpellOwnedNpcService.IsExactCurrentNpcPrimary(31, 12, 44, true, 32, 12, 44));
	}

	[TestMethod]
	public void LifecycleDiagnostic_RejectsHeldAndActivationPendingRowsButAllowsNativeOrigin()
	{
		Assert.IsTrue(SpellOwnedNpcService.HasUsableNpcDiagnostic("native-createnpc; template=7/1"));
		Assert.IsFalse(SpellOwnedNpcService.HasUsableNpcDiagnostic("Native NPC activation pending: activation failed"));
		Assert.IsFalse(SpellOwnedNpcService.HasUsableNpcDiagnostic("Persisted native death reconciliation held"));
		Assert.IsTrue(SpellOwnedNpcService.HasUsableNpcDiagnostic(""));
	}

	[TestMethod]
	public void ControlBond_SavesLifecycleCreatorAndExactPhysicalInstanceReferences()
	{
		var world = new Mock<IFuturemud>();
		world.SetupGet(x => x.FutureProgs).Returns(Mock.Of<IUneditableAll<MudSharp.FutureProg.IFutureProg>>());
		var owner = new Mock<ICharacter>(); owner.SetupGet(x => x.Gameworld).Returns(world.Object);
		var creator = Mock.Of<ICharacter>(x => x.InstanceId == 23);
		var origin = new SpellLifecycleOrigin(Guid.NewGuid(), 7, 2, 17, "fixture", SpellLifecycleMode.TemporaryCleanup,
			DateTime.UtcNow, DateTime.UtcNow.AddMinutes(5));
		var effect = new SpellNpcControlEffect(owner.Object, origin, creator);
		var saved = effect.SaveToXml(new Dictionary<IEffect, TimeSpan>());
		SpellNpcControlEffect.InitialiseEffectType();
		var loaded = (SpellNpcControlEffect)Effect.LoadEffect(saved, owner.Object);
		Assert.IsTrue(loaded.SavingEffect);
		Assert.AreEqual(origin.Id, loaded.LifecycleId);
		Assert.AreEqual(17L, loaded.CreatorId);
		Assert.AreEqual(23L, loaded.CreatorInstanceId);
		Assert.AreEqual("none", loaded.Intent);
		Assert.IsFalse(loaded.ControlLost);
		Assert.AreEqual(0, loaded.Targets.Count());
		Assert.IsFalse(loaded.SetOrder("attack", creator, creator));
		Assert.IsFalse(loaded.LoseControl(creator));
		Assert.AreEqual("none", loaded.Intent);
		Assert.IsFalse(loaded.ControlLost);
		owner.Verify(x => x.Save(), Times.Never);
		Assert.IsTrue(XNode.DeepEquals(saved, loaded.SaveToXml(new())));
		var stored = PhysicalReferenceCodecs.Effects(new XElement("Effects", saved).ToString()).ToArray();
		Assert.AreEqual(2, stored.Length);
		CollectionAssert.AreEquivalent(new[] { (PhysicalEntityKind.Character, 17L), (PhysicalEntityKind.CharacterInstance, 23L) },
			stored.Select(x => (x.Kind, x.Id)).ToArray());
	}

	[TestMethod]
	public void SpellCompanionAi_LoadAndSave_PreservesPurposeModeSubsetAndLossPolicy()
	{
		var world = new Mock<IFuturemud>();
		var model = new MudSharp.Models.ArtificialIntelligence
		{
			Id = 11,
			Name = "outsider",
			Type = "SpellCompanion",
			Definition = new XElement("Definition", new XElement("Mode", "outsider"), new XElement("Purpose", "escort"),
				new XElement("Permitted", new XElement("Order", "follow"), new XElement("Order", "attack")),
				new XElement("LossChancePerGrade", 0.05), new XElement("LossChanceCap", 0.5)).ToString()
		};
		var constructor = typeof(SpellCompanionAI).GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance,
			null, [typeof(MudSharp.Models.ArtificialIntelligence), typeof(IFuturemud)], null)!;
		var ai = (SpellCompanionAI)constructor.Invoke([model, world.Object]);
		var save = typeof(ArtificialIntelligenceBase).GetMethod("DefinitionForSave", BindingFlags.NonPublic | BindingFlags.Instance)!;
		var xml = XElement.Parse((string)save.Invoke(ai, null)!);
		Assert.AreEqual("outsider", xml.Element("Mode")!.Value);
		Assert.AreEqual("escort", xml.Element("Purpose")!.Value);
		CollectionAssert.AreEquivalent(new[] { "follow", "attack" }, xml.Element("Permitted")!.Elements("Order").Select(x => x.Value).ToArray());
		Assert.AreEqual(0.05, (double)xml.Element("LossChancePerGrade")!);
		Assert.AreEqual(0.5, (double)xml.Element("LossChanceCap")!);
	}

	[TestMethod]
	public void CourierMission_SaveAndLoad_PreservesExactActorsAndParcelReferences()
	{
		var world = new Mock<IFuturemud>();
		world.SetupGet(x => x.FutureProgs).Returns(Mock.Of<IUneditableAll<MudSharp.FutureProg.IFutureProg>>());
		var owner = new Mock<ICharacter>(); owner.SetupGet(x => x.Gameworld).Returns(world.Object);
		var target = new Mock<ICharacter>(); target.SetupGet(x => x.Id).Returns(17); target.SetupGet(x => x.InstanceId).Returns(71);
		var recipient = new Mock<ICharacter>(); recipient.SetupGet(x => x.Id).Returns(19); recipient.SetupGet(x => x.InstanceId).Returns(91);
		var parcel = new Mock<IGameItem>(); parcel.SetupGet(x => x.Id).Returns(23);
		var mission = new SpellNpcMissionEffect(owner.Object, "courier", target.Object, recipient.Object, parcel.Object);
		var xml = mission.SaveToXml(new Dictionary<IEffect, TimeSpan>());
		SpellNpcMissionEffect.InitialiseEffectType();
		var loaded = (SpellNpcMissionEffect)Effect.LoadEffect(xml, owner.Object);
		Assert.AreEqual("courier", loaded.Mission);
		Assert.AreEqual(0L, loaded.TargetId);
		Assert.AreEqual(0L, loaded.TargetInstanceId);
		Assert.AreEqual(19L, loaded.RecipientId);
		Assert.AreEqual(91L, loaded.RecipientInstanceId);
		Assert.AreEqual(23L, loaded.ParcelId);
		var refs = PhysicalReferenceCodecs.Effects(new XElement("Effects", xml).ToString()).ToArray();
		CollectionAssert.AreEquivalent(new[]
		{
			(PhysicalEntityKind.Character, 19L), (PhysicalEntityKind.CharacterInstance, 91L),
			(PhysicalEntityKind.GameItem, 23L)
		}, refs.Select(x => (x.Kind, x.Id)).ToArray());
	}

	[TestMethod]
	public void SeekingTrackProof_RequiresExactCharacterAndPhysicalInstance()
	{
		var track = new Mock<ITrack>();
		track.SetupGet(x => x.CharacterId).Returns(17);
		track.SetupGet(x => x.CharacterInstanceId).Returns(71);
		Assert.IsTrue(SpellNpcMissionEffect.TrackMatchesExactTarget(track.Object, 17, 71));
		Assert.IsFalse(SpellNpcMissionEffect.TrackMatchesExactTarget(track.Object, 17, 72));
		track.SetupGet(x => x.CharacterInstanceId).Returns((long?)null);
		Assert.IsFalse(SpellNpcMissionEffect.TrackMatchesExactTarget(track.Object, 17, 71));
	}

	[TestMethod]
	public void CourierTransferProof_RequiresPostCallbackExactExclusiveCustody()
	{
		var source = new Mock<IBody>();
		var destination = new Mock<IBody>();
		var parcel = new Mock<IGameItem>();
		var sourceItems = new List<IGameItem>();
		var destinationItems = new List<IGameItem> { parcel.Object };
		source.SetupGet(x => x.HeldOrWieldedItems).Returns(() => sourceItems);
		destination.SetupGet(x => x.HeldOrWieldedItems).Returns(() => destinationItems);
		parcel.SetupGet(x => x.Quantity).Returns(1);
		parcel.SetupGet(x => x.InInventoryOf).Returns(destination.Object);
		Assert.IsTrue(SpellNpcMissionEffect.IsProvedCustodyTransfer(source.Object, destination.Object, parcel.Object, 1, parcel.Object));
		Assert.IsFalse(SpellNpcMissionEffect.IsProvedCustodyTransfer(source.Object, destination.Object, parcel.Object, 1, null));
		sourceItems.Add(parcel.Object);
		Assert.IsFalse(SpellNpcMissionEffect.IsProvedCustodyTransfer(source.Object, destination.Object, parcel.Object, 1, parcel.Object));
	}

	[TestMethod]
	public void CompletedMission_SaveHasNoTargetRecipientOrParcelPhysicalReferences()
	{
		var world = new Mock<IFuturemud>();
		world.SetupGet(x => x.FutureProgs).Returns(Mock.Of<IUneditableAll<MudSharp.FutureProg.IFutureProg>>());
		var owner = new Mock<ICharacter>(); owner.SetupGet(x => x.Gameworld).Returns(world.Object);
		var target = Mock.Of<ICharacter>(x => x.Id == 17 && x.InstanceId == 71);
		var recipient = Mock.Of<ICharacter>(x => x.Id == 19 && x.InstanceId == 91);
		var parcel = Mock.Of<IGameItem>(x => x.Id == 23);
		var mission = new SpellNpcMissionEffect(owner.Object, "courier", target, recipient, parcel);
		var xml = mission.SaveToXml(new Dictionary<IEffect, TimeSpan>());
		var data = xml.Element("Effect")!;
		data.Element("Phase")!.Value = "complete";
		data.Element("TargetId")?.Remove();
		data.Element("TargetInstanceId")?.Remove();
		data.Element("RecipientId")!.Remove(); data.Element("RecipientInstanceId")!.Remove();
		data.Element("ParcelId")!.Value = "0";
		SpellNpcMissionEffect.InitialiseEffectType();
		var loaded = (SpellNpcMissionEffect)Effect.LoadEffect(xml, owner.Object);
		Assert.AreEqual(0, loaded.PhysicalReferences.Count());
		Assert.AreEqual(0, PhysicalReferenceCodecs.Effects(new XElement("Effects", xml).ToString()).Count());
	}

	[TestMethod]
	public void SpellFearChild_RoundTripsAbsoluteExpiryForSavedParent()
	{
		var world = new Mock<IFuturemud>();
		world.SetupGet(x => x.FutureProgs).Returns(Mock.Of<IUneditableAll<MudSharp.FutureProg.IFutureProg>>());
		var owner = new Mock<ICharacter>(); owner.SetupGet(x => x.Gameworld).Returns(world.Object);
		var parent = Mock.Of<IMagicSpellEffectParent>();
		var deadline = DateTime.UtcNow.AddMinutes(4);
		var fear = new SpellFearEffect(owner.Object, parent, expiryUtc: deadline);
		var xml = fear.SaveToXml(new Dictionary<IEffect, TimeSpan>());
		SpellFearEffect.InitialiseEffectType();
		var loaded = (SpellFearEffect)Effect.LoadEffect(xml, owner.Object);
		Assert.AreEqual(deadline, loaded.ExpiryUtc);
		Assert.AreEqual(deadline, (DateTime?)xml.Element("Effect")!.Element("ExpiryUtc"));
	}

	[TestMethod]
	public void MissionPersistence_SaveMissionActors_SavesEachExactCharacterBodyAndParcel()
	{
		var creature = new Mock<ICharacter>(); var target = new Mock<ICharacter>();
		var creatureBody = new Mock<IBody>(); var targetBody = new Mock<IBody>();
		creature.SetupGet(x => x.Body).Returns(creatureBody.Object);
		target.SetupGet(x => x.Body).Returns(targetBody.Object);
		var parcel = new Mock<IGameItem>();
		SpellNpcControlEffect.SaveMissionActors([creature.Object, target.Object, creature.Object], parcel.Object);
		creature.Verify(x => x.Save(), Times.Once); target.Verify(x => x.Save(), Times.Once);
		creatureBody.Verify(x => x.Save(), Times.Once); targetBody.Verify(x => x.Save(), Times.Once);
		parcel.Verify(x => x.Save(), Times.Once);
	}

	[TestMethod]
	public void MagicSpellParent_OptInCleanupRemovesEmptyParentAfterExpiredChildLoad()
	{
		var world = new Mock<IFuturemud>();
		var owner = new Mock<ICharacter>(); owner.SetupGet(x => x.Gameworld).Returns(world.Object);
		var parent = new MagicSpellParent(owner.Object, Mock.Of<IMagicSpell>(), Mock.Of<ICharacter>())
		{
			RemoveWhenLoadedChildrenExpire = true
		};
		parent.InitialEffect();
		owner.Verify(x => x.RemoveEffect(parent, true), Times.Once);
		var ordinaryParent = new MagicSpellParent(owner.Object, Mock.Of<IMagicSpell>(), Mock.Of<ICharacter>());
		ordinaryParent.InitialEffect();
		owner.Verify(x => x.RemoveEffect(ordinaryParent, true), Times.Never);
	}

	[TestMethod]
	public void MissionLoad_RejectsUnknownPhaseAndIncompleteHauntSource()
	{
		var world = new Mock<IFuturemud>();
		world.SetupGet(x => x.FutureProgs).Returns(Mock.Of<IUneditableAll<MudSharp.FutureProg.IFutureProg>>());
		var owner = new Mock<ICharacter>(); owner.SetupGet(x => x.Gameworld).Returns(world.Object);
		var target = new Mock<ICharacter>(); target.SetupGet(x => x.Id).Returns(17); target.SetupGet(x => x.InstanceId).Returns(71);
		target.SetupGet(x => x.Body).Returns(Mock.Of<IBody>(x => x.Id == 72));
		var unknown = new SpellNpcMissionEffect(owner.Object, "seek", target.Object).SaveToXml(new());
		unknown.Element("Effect")!.Element("Phase")!.Value = "attack-tree";
		SpellNpcMissionEffect.InitialiseEffectType();
		Assert.ThrowsExactly<FormatException>(() => Effect.LoadEffect(unknown, owner.Object));

		var incompleteHaunt = new SpellNpcMissionEffect(owner.Object, "haunt", target.Object, expiryUtc: DateTime.UtcNow.AddMinutes(2)).SaveToXml(new());
		incompleteHaunt.Element("Effect")!.Element("Phase")!.Value = "haunting";
		Assert.ThrowsExactly<FormatException>(() => Effect.LoadEffect(incompleteHaunt, owner.Object));
	}

	[TestMethod]
	public void OfflineFearCleanup_RemovesOnlyOneExactPrimaryParentAndPreservesSiblings()
	{
		var id = Guid.NewGuid();
		var descriptor = new SpellNpcControlEffect.FearParentCleanup(17, 71, 72, id, 8, 19, 91);
		var exact = new XElement("Effect", new XElement("ApplicabilityProg", 0), new XElement("Type", "MagicSpellParent"),
			new XElement("Original", 0), new XElement("Remaining", 0), new XElement("Effect", new XElement("Identity", id),
			new XElement("RemoveWhenLoadedChildrenExpire", true), new XElement("Spell", 8), new XElement("Caster", 19),
			new XElement("CasterInstance", 91), new XElement("Children", new XElement("Effect",
				new XElement("Type", "SpellFear"), new XElement("Effect", new XElement("ExpiryUtc", DateTime.UtcNow.AddMinutes(2)))))));
		var sibling = new XElement("Effect", new XElement("Type", "Unrelated"), new XElement("Effect", new XElement("Marker", "keep")));
		var xml = new XElement("Effects", exact, sibling).ToString(SaveOptions.DisableFormatting);
		Assert.IsTrue(SpellNpcControlEffect.TryRemoveExactFearParentXml(xml, descriptor, out var updated, out var changed));
		Assert.IsTrue(changed);
		var result = XElement.Parse(updated!);
		Assert.AreEqual(1, result.Elements("Effect").Count());
		Assert.IsTrue(XNode.DeepEquals(sibling, result.Element("Effect")));
	}

	[TestMethod]
	public void OfflineFearCleanup_RejectsMismatchedAndDuplicateParentAndSecondaryIdentity()
	{
		var id = Guid.NewGuid();
		var descriptor = new SpellNpcControlEffect.FearParentCleanup(17, 71, 72, id, 8, 19, 91);
		var exact = new XElement("Effect", new XElement("Type", "MagicSpellParent"), new XElement("Effect", new XElement("Identity", id),
			new XElement("Spell", 8), new XElement("Caster", 19), new XElement("CasterInstance", 91),
			new XElement("Children", new XElement("Effect", new XElement("Type", "SpellFear")))));
		var wrongSource = new XElement(exact); wrongSource.Element("Effect")!.Element("Caster")!.Value = "20";
		Assert.IsFalse(SpellNpcControlEffect.TryRemoveExactFearParentXml(new XElement("Effects", wrongSource).ToString(), descriptor, out _, out _));
		Assert.IsFalse(SpellNpcControlEffect.TryRemoveExactFearParentXml(new XElement("Effects", exact, new XElement(exact)).ToString(), descriptor, out _, out _));
		Assert.IsFalse(SpellNpcControlEffect.IsExactPrimaryFearTarget(70, 17, 72, false, 71, 17, 72));
		Assert.IsFalse(SpellNpcControlEffect.IsExactPrimaryFearTarget(70, 17, 72, true, 71, 17, 72));
		Assert.IsTrue(SpellNpcControlEffect.IsExactPrimaryFearTarget(71, 17, 72, true, 71, 17, 72));
		Assert.IsTrue(SpellNpcControlEffect.TryRemoveExactFearParentXml(new XElement("Effects", new XElement("Effect",
			new XElement("Type", "Unrelated"), new XElement("Effect", new XElement("Identity", id)))).ToString(), descriptor, out _, out var changed));
		Assert.IsFalse(changed);
	}

	[TestMethod]
	public void MissionTargetBody_IsPersistedAsExplicitPhysicalReference()
	{
		var world = new Mock<IFuturemud>();
		world.SetupGet(x => x.FutureProgs).Returns(Mock.Of<IUneditableAll<MudSharp.FutureProg.IFutureProg>>());
		var owner = new Mock<ICharacter>(); owner.SetupGet(x => x.Gameworld).Returns(world.Object);
		var body = new Mock<IBody>(); body.SetupGet(x => x.Id).Returns(72);
		var target = new Mock<ICharacter>(); target.SetupGet(x => x.Id).Returns(17); target.SetupGet(x => x.InstanceId).Returns(71);
		target.SetupGet(x => x.Body).Returns(body.Object);
		var mission = new SpellNpcMissionEffect(owner.Object, "seek", target.Object);
		var xml = mission.SaveToXml(new Dictionary<IEffect, TimeSpan>());
		var refs = PhysicalReferenceCodecs.Effects(new XElement("Effects", xml).ToString()).ToArray();
		CollectionAssert.Contains(refs.Select(x => (x.Kind, x.Id)).ToArray(), (PhysicalEntityKind.Body, 72L));
	}

	[TestMethod]
	public void UncertainMissionPersistence_LatchesOrdersAndMissionWorkUntilReload()
	{
		var world = new Mock<IFuturemud>();
		var owner = new Mock<ICharacter>(); owner.SetupGet(x => x.Gameworld).Returns(world.Object);
		var creator = Mock.Of<ICharacter>(x => x.InstanceId == 23);
		var origin = new SpellLifecycleOrigin(Guid.NewGuid(), 7, 2, 17, "fixture", SpellLifecycleMode.TemporaryCleanup,
			DateTime.UtcNow, DateTime.UtcNow.AddMinutes(5));
		var effect = new SpellNpcControlEffect(owner.Object, origin, creator);
		effect.HoldAfterUncertainMissionMutation();
		Assert.IsFalse(effect.CanPerformMission());
		Assert.IsFalse(effect.CanAcceptOrder(creator));
		Assert.IsFalse(effect.SetOrder("follow", creator, creator));
	}
}
