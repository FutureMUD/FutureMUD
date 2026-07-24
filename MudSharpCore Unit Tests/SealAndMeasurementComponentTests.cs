#nullable enable

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Accounts;
using MudSharp.Body;
using MudSharp.Body.Position;
using MudSharp.Body.Traits;
using MudSharp.Character;
using MudSharp.Commands.Modules;
using MudSharp.Community;
using MudSharp.Combat;
using MudSharp.Construction;
using MudSharp.Effects;
using MudSharp.Effects.Concrete;
using MudSharp.Effects.Interfaces;
using MudSharp.Events;
using MudSharp.Form.Material;
using MudSharp.Form.Shape;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.Framework.Save;
using MudSharp.Framework.Units;
using MudSharp.FutureProg;
using MudSharp.GameItems;
using MudSharp.GameItems.Components;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Prototypes;
using MudSharp.Health;
using MudSharp.Movement;
using MudSharp.PerceptionEngine;
using MudSharp.RPG.Checks;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Xml.Linq;
using DbEditableItem = MudSharp.Models.EditableItem;
using DbGameItemComponent = MudSharp.Models.GameItemComponent;
using DbGameItemComponentProto = MudSharp.Models.GameItemComponentProto;

namespace MudSharp_Unit_Tests;

[TestClass]
public class SealAndMeasurementComponentTests
{
	[TestMethod]
	public void GameItemComponentManager_RegistersSealAndMeasuringTypes()
	{
		var manager = new GameItemComponentManager();
		var primaryTypes = manager.PrimaryTypes.ToList();
		var helpTypes = manager.TypeHelpInfo.Select(x => x.Name).ToList();

		CollectionAssert.Contains(primaryTypes, "sealstamp");
		CollectionAssert.Contains(primaryTypes, "sealable");
		CollectionAssert.Contains(primaryTypes, "measuringinstrument");
		CollectionAssert.Contains(primaryTypes, "incenseburner");
		CollectionAssert.Contains(primaryTypes, "offeringreceiver");
		CollectionAssert.Contains(primaryTypes, "bayonetattachment");
		CollectionAssert.Contains(primaryTypes, "lockingcashregister");
		CollectionAssert.Contains(primaryTypes, "instrument");
		CollectionAssert.Contains(primaryTypes, "signalinstrument");
		CollectionAssert.Contains(primaryTypes, "militarystandard");
		CollectionAssert.Contains(helpTypes, "SealStamp");
		CollectionAssert.Contains(helpTypes, "Sealable");
		CollectionAssert.Contains(helpTypes, "MeasuringInstrument");
		CollectionAssert.Contains(helpTypes, "IncenseBurner");
		CollectionAssert.Contains(helpTypes, "OfferingReceiver");
		CollectionAssert.Contains(helpTypes, "BayonetAttachment");
		CollectionAssert.Contains(helpTypes, "LockingCashRegister");
		CollectionAssert.Contains(helpTypes, "Instrument");
		CollectionAssert.Contains(helpTypes, "SignalInstrument");
		CollectionAssert.Contains(helpTypes, "MilitaryStandard");
	}

	[TestMethod]
	public void InstrumentAndSignalInstrument_DefinitionsRoundTripPerformanceAndSignalSettings()
	{
		var gameworld = CreateGameworld();
		var instrument = CreateProto<InstrumentGameItemComponentProto>(gameworld.Object,
			ComponentProtoModel(601, "Instrument", new XElement("Definition",
				new XElement("Family", "Lyre"),
				new XElement("PerformanceTrait", 0),
				new XElement("Difficulty", Difficulty.Hard),
				new XElement("Volume", "Decent"),
				new XElement("RequiredHands", 2),
				new XElement("UseModes", "Handheld, Worn"),
				new XElement("InitialStamina", 2.0),
				new XElement("TickStamina", 1.0),
				new XElement("TickSeconds", 10),
				new XElement("Positions", new XElement("Position", "standing")),
				new XElement("Styles", new XElement("Style", "hymn")),
				new XElement("LocalPlayEmote", "@ play|plays $1."),
				new XElement("LocalTickEmote", "@ continue|continues $1."),
				new XElement("DistantPlayEmote", "You hear a lyre {0}."),
				new XElement("FailureEmote", "@ fail|fails to play $1."),
				new XElement("StopEmote", "@ stop|stops playing $1.")).ToString()));

		Assert.AreEqual("Lyre", instrument.InstrumentFamily);
		Assert.AreEqual(2, instrument.RequiredHands);
		Assert.IsTrue(instrument.UseModes.HasFlag(InstrumentUseMode.Handheld));
		Assert.IsTrue(instrument.UseModes.HasFlag(InstrumentUseMode.Worn));
		CollectionAssert.Contains(instrument.Styles, "hymn");
		StringAssert.Contains(InvokeSaveToXml(instrument), "<TickSeconds>10</TickSeconds>");

		var signal = CreateProto<SignalInstrumentGameItemComponentProto>(gameworld.Object,
			ComponentProtoModel(602, "SignalInstrument", new XElement("Definition",
				new XElement("Family", "Field Drum"),
				new XElement("PerformanceTrait", 0),
				new XElement("Difficulty", Difficulty.Normal),
				new XElement("Volume", "Loud"),
				new XElement("RequiredHands", 2),
				new XElement("UseModes", "Handheld, Worn"),
				new XElement("InitialStamina", 2.0),
				new XElement("TickStamina", 1.0),
				new XElement("TickSeconds", 10),
				new XElement("Positions"),
				new XElement("Styles", new XElement("Style", "march")),
				new XElement("SignalStamina", 5.0),
				new XElement("SignalCooldownSeconds", 10),
				new XElement("Signals",
					new XElement("Signal", new XAttribute("name", "rally"),
						new XElement("Local", "@ sound|sounds rally on $1."),
						new XElement("Distant", "You hear rally {0}."),
						new XElement("Failure", "@ garble|garbles $1.")))).ToString()));

		Assert.AreEqual(1, signal.SignalPatterns.Count);
		Assert.AreEqual("rally", signal.SignalPatterns.Single().Name);
		Assert.AreEqual(TimeSpan.FromSeconds(10), signal.SignalCooldown);
		StringAssert.Contains(InvokeSaveToXml(signal), "name=\"rally\"");
	}

	[TestMethod]
	public void MilitaryStandard_CustodyTransitionsPersistWithoutDuplicateHostileCapturesAndCopiesResetHistory()
	{
		var gameworld = CreateGameworld();
		var proto = CreateProto<MilitaryStandardGameItemComponentProto>(gameworld.Object,
			ComponentProtoModel(610, "MilitaryStandard", new XElement("Definition",
				new XElement("Family", "InfantryColour"),
				new XElement("IdentityKey", "first-colour"),
				new XElement("IdentityName", "the First Regiment's colour"),
				new XElement("Design", "A red field bears a white tower."),
				new XElement("AssociationType", "Unit"),
				new XElement("AssociationKey", "first-regiment"),
				new XElement("AssociationName", "First Regiment"),
				new XElement("RecognitionTrait", 0),
				new XElement("RecognitionDifficulty", Difficulty.Normal),
				new XElement("Signals"),
				new XElement("PlantEmote", "@ plant|plants $1."),
				new XElement("TakeUpEmote", "@ take|takes up $1."),
				new XElement("RecogniseEmote", "@ recognise|recognises $1.")).ToString()));
		var parent = CreateParent(gameworld.Object, 611L, "colour");
		parent.SetupGet(x => x.HasOwner).Returns(true);
		var friendly = CreateActor(gameworld.Object, 1L);
		var hostile = CreateActor(gameworld.Object, 2L);
		parent.Setup(x => x.IsOwnedBy(friendly.Object)).Returns(true);
		parent.Setup(x => x.IsOwnedBy(hostile.Object)).Returns(false);
		var standard = (MilitaryStandardGameItemComponent)proto.CreateNew(parent.Object, temporary: true);

		standard.ReevaluateCustody(friendly.Object);
		Assert.AreEqual(MilitaryStandardCustodyState.Friendly, standard.CustodyState);
		standard.ReevaluateCustody(hostile.Object);
		Assert.AreEqual(MilitaryStandardCustodyState.Captured, standard.CustodyState);
		Assert.AreEqual(1, standard.CaptureCount);
		standard.ReevaluateCustody(hostile.Object);
		Assert.AreEqual(1, standard.CaptureCount,
			"Transfers among hostile custodians must not count as fresh captures.");
		standard.ReevaluateCustody(friendly.Object);
		Assert.AreEqual(MilitaryStandardCustodyState.Friendly, standard.CustodyState);
		Assert.AreEqual(1, standard.CaptureCount);

		standard.SetIdentityOverride("campaign-colour", "the campaign colour");
		standard.SetAssociationOverride(MilitaryStandardAssociationType.Unit, "field-army", "Field Army");
		var persisted = InvokeSaveToXml(standard);
		StringAssert.Contains(persisted, "<CustodyState>Friendly</CustodyState>");
		StringAssert.Contains(persisted, "<CaptureCount>1</CaptureCount>");
		var loaded = (MilitaryStandardGameItemComponent)proto.LoadComponent(
			new DbGameItemComponent { Definition = persisted },
			CreateParent(gameworld.Object, 612L, "loaded colour").Object);
		Assert.AreEqual("campaign-colour", loaded.IdentityKey);
		Assert.AreEqual("field-army", loaded.AssociationKey);
		Assert.AreEqual(1, loaded.CaptureCount);

		var copy = (MilitaryStandardGameItemComponent)standard.Copy(
			CreateParent(gameworld.Object, 613L, "copy colour").Object, true);
		Assert.AreEqual("campaign-colour", copy.IdentityKey);
		Assert.AreEqual("field-army", copy.AssociationKey);
		Assert.AreEqual(MilitaryStandardCustodyState.Unclaimed, copy.CustodyState);
		Assert.AreEqual(0, copy.CaptureCount);
		Assert.IsFalse(copy.IsPlanted);
	}

	[TestMethod]
	public void Instrument_RuntimeConsumesStaminaPropagatesAudioTicksStopsAndQuits()
	{
		var gameworld = CreateGameworld();
		var proto = CreateProto<InstrumentGameItemComponentProto>(gameworld.Object,
			ComponentProtoModel(620, "Instrument", new XElement("Definition",
				new XElement("Family", "Lyre"),
				new XElement("PerformanceTrait", 0),
				new XElement("Difficulty", Difficulty.Automatic),
				new XElement("Volume", "Decent"),
				new XElement("RequiredHands", 0),
				new XElement("UseModes", "Room"),
				new XElement("InitialStamina", 2.0),
				new XElement("TickStamina", 1.0),
				new XElement("TickSeconds", 10),
				new XElement("Positions"),
				new XElement("Styles", new XElement("Style", "hymn")),
				new XElement("LocalPlayEmote", "@ play|plays $1."),
				new XElement("LocalTickEmote", "@ continue|continues playing $1."),
				new XElement("DistantPlayEmote", "You hear music {0}."),
				new XElement("FailureEmote", "@ fail|fails to play $1."),
				new XElement("StopEmote", "@ stop|stops playing $1.")).ToString()));
		var parent = CreateParent(gameworld.Object, 621L, "lyre");
		var (actor, _, cell) = CreateInstrumentActor(gameworld.Object, parent);
		var component = (InstrumentGameItemComponent)proto.CreateNew(parent.Object, temporary: true);
		PlayingInstrument? playingEffect = null;
		actor.Setup(x => x.AddEffect(It.IsAny<IEffect>(), It.IsAny<TimeSpan>()))
		     .Callback<IEffect, TimeSpan>((effect, _) => playingEffect = effect as PlayingInstrument);
		actor.Setup(x => x.RemoveAllEffects<PlayingInstrument>(
				It.IsAny<Predicate<PlayingInstrument>>(), It.IsAny<bool>()))
		     .Returns(true);

		Assert.IsTrue(component.UseModes.HasFlag(InstrumentUseMode.Room));
		Assert.AreSame(parent.Object, component.Parent);
		Assert.AreEqual(string.Empty, component.WhyCannotPlay(actor.Object, "hymn"));
		Assert.IsTrue(component.Play(actor.Object, "hymn"));
		Assert.IsNotNull(playingEffect);
		Assert.AreEqual(Outcome.Pass, component.CurrentOutcome);
		actor.Verify(x => x.SpendStamina(2.0), Times.Once);
		cell.Verify(x => x.HandleAudioEcho("You hear music {0}.",
			MudSharp.Form.Audio.AudioVolume.Decent, parent.Object, It.IsAny<RoomLayer>(), true), Times.Once);

		component.PerformTick();
		actor.Verify(x => x.SpendStamina(1.0), Times.Once);
		cell.Verify(x => x.HandleAudioEcho("You hear music {0}.",
			MudSharp.Form.Audio.AudioVolume.Decent, parent.Object, It.IsAny<RoomLayer>(), true), Times.Exactly(2));

		component.StopPlaying(actor.Object);
		Assert.IsFalse(component.IsBeingPlayed);
		actor.Verify(x => x.RemoveAllEffects<PlayingInstrument>(
			It.IsAny<Predicate<PlayingInstrument>>(), true), Times.Once);

		Assert.IsTrue(component.Play(actor.Object, "hymn"));
		component.Quit();
		Assert.IsFalse(component.IsBeingPlayed, "Logout must terminate a sustained performance.");
	}

	[TestMethod]
	public void SignalInstrument_FailedSignalIsNeutralSuppressesHookAndAppliesCooldown()
	{
		var trait = new Mock<ITraitDefinition>();
		trait.SetupGet(x => x.Id).Returns(30L);
		trait.SetupGet(x => x.Name).Returns("Performance");
		var onSignal = new Mock<IFutureProg>();
		onSignal.SetupGet(x => x.Id).Returns(31L);
		onSignal.SetupGet(x => x.Name).Returns("On Signal");
		var check = new Mock<ICheck>();
		check.Setup(x => x.Check(It.IsAny<ICharacter>(), Difficulty.Normal, trait.Object,
				It.IsAny<IPerceivable>(), It.IsAny<double>(), It.IsAny<TraitUseType>(),
				It.IsAny<(string Parameter, object value)[]>()))
		     .Returns(CheckOutcome.SimpleOutcome(CheckType.GenericSkillCheck, Outcome.Fail));
		var gameworld = CreateGameworld();
		gameworld.SetupGet(x => x.Traits).Returns(Repository(trait.Object));
		gameworld.SetupGet(x => x.FutureProgs).Returns(Repository(onSignal.Object));
		gameworld.Setup(x => x.GetCheck(CheckType.GenericSkillCheck)).Returns(check.Object);
		var proto = CreateProto<SignalInstrumentGameItemComponentProto>(gameworld.Object,
			ComponentProtoModel(630, "SignalInstrument", new XElement("Definition",
				new XElement("Family", "Field Drum"),
				new XElement("PerformanceTrait", trait.Object.Id),
				new XElement("Difficulty", Difficulty.Normal),
				new XElement("Volume", "Loud"),
				new XElement("RequiredHands", 0),
				new XElement("UseModes", "Room"),
				new XElement("InitialStamina", 2.0),
				new XElement("TickStamina", 1.0),
				new XElement("TickSeconds", 10),
				new XElement("Positions"),
				new XElement("Styles", new XElement("Style", "march")),
				new XElement("SignalStamina", 5.0),
				new XElement("SignalCooldownSeconds", 10),
				new XElement("OnSignalProg", onSignal.Object.Id),
				new XElement("Signals",
					new XElement("Signal", new XAttribute("name", "rally"),
						new XElement("Local", "@ sound|sounds a rally on $1."),
						new XElement("Distant", "You hear a rally {0}."),
						new XElement("Failure", "@ sound|sounds a garbled call on $1.")))).ToString()));
		var parent = CreateParent(gameworld.Object, 631L, "drum");
		var (actor, _, cell) = CreateInstrumentActor(gameworld.Object, parent);
		var component = (SignalInstrumentGameItemComponent)proto.CreateNew(parent.Object, temporary: true);
		SignalInstrumentCooldown? cooldown = null;
		actor.Setup(x => x.AddEffect(It.IsAny<IEffect>(), It.IsAny<TimeSpan>()))
		     .Callback<IEffect, TimeSpan>((effect, _) => cooldown = effect as SignalInstrumentCooldown);
		actor.Setup(x => x.EffectsOfType<SignalInstrumentCooldown>(
				It.IsAny<Predicate<SignalInstrumentCooldown>>()))
		     .Returns(() => cooldown is null ? [] : [cooldown]);

		Assert.IsTrue(component.UseModes.HasFlag(InstrumentUseMode.Room));
		Assert.AreEqual(string.Empty, component.WhyCannotSignal(actor.Object, "rally"));
		Assert.IsFalse(component.Signal(actor.Object, "rally"));
		Assert.IsNotNull(cooldown);
		actor.Verify(x => x.SpendStamina(5.0), Times.Once);
		cell.Verify(x => x.HandleAudioEcho(
			It.Is<string>(text => text.Contains("unrecognisable", StringComparison.OrdinalIgnoreCase)),
			MudSharp.Form.Audio.AudioVolume.Loud, parent.Object, It.IsAny<RoomLayer>(), true), Times.Once);
		onSignal.Verify(x => x.Execute(It.IsAny<object[]>()), Times.Never);
		StringAssert.Contains(component.WhyCannotSignal(actor.Object, "rally"), "quite yet");
	}

	[TestMethod]
	public void MilitaryStandard_AuthorisationProgIsAuthoritativeAndHooksFireOncePerTransition()
	{
		var canBear = new Mock<IFutureProg>();
		canBear.SetupGet(x => x.Id).Returns(40L);
		canBear.SetupGet(x => x.Name).Returns("Can Bear");
		canBear.Setup(x => x.ExecuteBool(false, It.IsAny<object[]>()))
		       .Returns(false);
		var onCaptured = new Mock<IFutureProg>();
		onCaptured.SetupGet(x => x.Id).Returns(41L);
		onCaptured.SetupGet(x => x.Name).Returns("On Captured");
		var onRecovered = new Mock<IFutureProg>();
		onRecovered.SetupGet(x => x.Id).Returns(42L);
		onRecovered.SetupGet(x => x.Name).Returns("On Recovered");
		var onCustody = new Mock<IFutureProg>();
		onCustody.SetupGet(x => x.Id).Returns(43L);
		onCustody.SetupGet(x => x.Name).Returns("On Custody");
		var gameworld = CreateGameworld();
		gameworld.SetupGet(x => x.FutureProgs)
		         .Returns(Repository(canBear.Object, onCaptured.Object, onRecovered.Object, onCustody.Object));
		var proto = CreateProto<MilitaryStandardGameItemComponentProto>(gameworld.Object,
			ComponentProtoModel(640, "MilitaryStandard", new XElement("Definition",
				new XElement("Family", "Guidon"),
				new XElement("IdentityKey", "guidon"),
				new XElement("IdentityName", "the guidon"),
				new XElement("Design", "A fork-tailed field."),
				new XElement("AssociationType", "None"),
				new XElement("AssociationKey", string.Empty),
				new XElement("AssociationName", string.Empty),
				new XElement("RecognitionTrait", 0),
				new XElement("RecognitionDifficulty", Difficulty.Automatic),
				new XElement("CanBearProg", canBear.Object.Id),
				new XElement("OnCapturedProg", onCaptured.Object.Id),
				new XElement("OnRecoveredProg", onRecovered.Object.Id),
				new XElement("OnCustodyChangedProg", onCustody.Object.Id),
				new XElement("Signals"),
				new XElement("PlantEmote", "@ plant|plants $1."),
				new XElement("TakeUpEmote", "@ take|takes up $1."),
				new XElement("RecogniseEmote", "@ recognise|recognises $1.")).ToString()));
		var parent = CreateParent(gameworld.Object, 641L, "guidon");
		parent.SetupGet(x => x.HasOwner).Returns(true);
		var actor = CreateActor(gameworld.Object, 642L);
		parent.Setup(x => x.IsOwnedBy(actor.Object)).Returns(true);
		var standard = (MilitaryStandardGameItemComponent)proto.CreateNew(parent.Object, temporary: true);

		standard.ReevaluateCustody(actor.Object);
		Assert.AreEqual(MilitaryStandardCustodyState.Captured, standard.CustodyState,
			"The configured CanBearProg must override durable character ownership.");
		Assert.AreEqual(1, standard.CaptureCount);
		standard.ReevaluateCustody(actor.Object);
		onCaptured.Verify(x => x.Execute(It.IsAny<object[]>()), Times.Once);
		onCustody.Verify(x => x.Execute(It.IsAny<object[]>()), Times.Once);

		canBear.Setup(x => x.ExecuteBool(false, It.IsAny<object[]>()))
		       .Returns(true);
		standard.HandleEvent(EventType.ItemOwnershipChanged, parent.Object, actor.Object);
		Assert.AreEqual(MilitaryStandardCustodyState.Friendly, standard.CustodyState);
		onRecovered.Verify(x => x.Execute(It.IsAny<object[]>()), Times.Once);
		onCustody.Verify(x => x.Execute(It.IsAny<object[]>()), Times.Exactly(2));
	}

	[TestMethod]
	public void MilitaryStandard_UnownedIsUnclaimedAndClanMembershipIsFriendly()
	{
		var gameworld = CreateGameworld();
		var proto = CreateProto<MilitaryStandardGameItemComponentProto>(gameworld.Object,
			ComponentProtoModel(650, "MilitaryStandard", new XElement("Definition",
				new XElement("Family", "InfantryColour"),
				new XElement("IdentityKey", "clan-colour"),
				new XElement("IdentityName", "the clan colour"),
				new XElement("Design", "A clan device."),
				new XElement("AssociationType", "None"),
				new XElement("AssociationKey", string.Empty),
				new XElement("AssociationName", string.Empty),
				new XElement("RecognitionTrait", 0),
				new XElement("RecognitionDifficulty", Difficulty.Automatic),
				new XElement("Signals"),
				new XElement("PlantEmote", "@ plant|plants $1."),
				new XElement("TakeUpEmote", "@ take|takes up $1."),
				new XElement("RecogniseEmote", "@ recognise|recognises $1.")).ToString()));
		var parent = CreateParent(gameworld.Object, 651L, "colour");
		var hasOwner = false;
		parent.SetupGet(x => x.HasOwner).Returns(() => hasOwner);
		var actor = CreateActor(gameworld.Object, 652L);
		actor.SetupGet(x => x.ClanMemberships).Returns([]);
		var standard = (MilitaryStandardGameItemComponent)proto.CreateNew(parent.Object, temporary: true);

		standard.ReevaluateCustody(actor.Object);
		Assert.AreEqual(MilitaryStandardCustodyState.Unclaimed, standard.CustodyState);

		var clan = new Mock<IClan>();
		clan.SetupGet(x => x.Id).Returns(653L);
		clan.SetupGet(x => x.Name).Returns("First Clan");
		var membership = new Mock<IClanMembership>();
		membership.SetupGet(x => x.Clan).Returns(clan.Object);
		hasOwner = true;
		parent.SetupGet(x => x.Owner).Returns(clan.Object);
		actor.SetupGet(x => x.ClanMemberships).Returns([membership.Object]);
		standard.ReevaluateCustody(actor.Object);
		Assert.AreEqual(MilitaryStandardCustodyState.Friendly, standard.CustodyState);
		Assert.AreEqual(0, standard.CaptureCount);
	}

	[TestMethod]
	public void DependencyClosurePrototypes_LoadLegacyAndRestrictedDefinitions()
	{
		var allowed = CreateTag(201L, "Paper Cartridges");
		var blocked = CreateTag(202L, "Training Loads");
		var ammunition = new Mock<IAmmunitionType>();
		ammunition.SetupGet(x => x.Id).Returns(301L);
		ammunition.SetupGet(x => x.Name).Returns("0.65 Bore Musket Shot");
		var bullet = new Mock<IGameItemProto>();
		bullet.SetupGet(x => x.Id).Returns(302L);
		bullet.SetupGet(x => x.RevisionNumber).Returns(0);
		bullet.SetupGet(x => x.Name).Returns("round ball");
		var gameworld = CreateGameworld(allowed.Object, blocked.Object);
		gameworld.SetupGet(x => x.AmmunitionTypes).Returns(Repository(ammunition.Object));
		gameworld.SetupGet(x => x.ItemProtos).Returns(RevisableRepository(bullet.Object));

		var legacyCartridge = CreateProto<MusketCartridgeGameItemComponentProto>(gameworld.Object,
			ComponentProtoModel(201, "MusketCartridge", new XElement("Definition",
				new XElement("AmmoType", ammunition.Object.Id),
				new XElement("BulletProto", bullet.Object.Id),
				new XElement("BulletBore", 0.65)).ToString()));
		Assert.IsNull(legacyCartridge.PowderMass);
		Assert.IsTrue(legacyCartridge.IncludesWad);
		StringAssert.Contains(InvokeSaveToXml(legacyCartridge), "<IncludesWad>true</IncludesWad>");

		var container = CreateProto<ContainerGameItemComponentProto>(gameworld.Object,
			ComponentProtoModel(202, "Container", new XElement("Definition",
				new XAttribute("Weight", 12000),
				new XAttribute("MaxSize", (int)SizeCategory.Small),
				new XAttribute("Preposition", "in"),
				new XAttribute("Closable", true),
				new XAttribute("Transparent", false),
				new XElement("AllowedTags", new XElement("Tag", allowed.Object.Id)),
				new XElement("BlockedTags", new XElement("Tag", blocked.Object.Id))).ToString()));
		CollectionAssert.AreEqual(new[] { allowed.Object }, container.AllowedTags);
		CollectionAssert.AreEqual(new[] { blocked.Object }, container.BlockedTags);
		StringAssert.Contains(InvokeSaveToXml(container), "<Tag>201</Tag>");
		var admitted = CreateContainerCandidate(allowed.Object);
		var rejected = CreateContainerCandidate();
		var blockedCandidate = CreateContainerCandidate(allowed.Object, blocked.Object);
		var runtimeContainer = (ContainerGameItemComponent)container.CreateNew(
			CreateParent(gameworld.Object, 205L, "bandolier").Object, temporary: true);
		Assert.IsTrue(runtimeContainer.CanPut(admitted.Object));
		Assert.IsFalse(runtimeContainer.CanPut(rejected.Object));
		Assert.IsFalse(runtimeContainer.CanPut(blockedCandidate.Object),
			"Blocked tags must win when an item also matches an allowed tag.");

		var bayonet = CreateProto<BayonetAttachmentGameItemComponentProto>(gameworld.Object,
			ComponentProtoModel(203, "BayonetAttachment", new XElement("Definition",
				new XElement("Style", "Plug"),
				new XElement("MinimumBore", 0.45),
				new XElement("MaximumBore", 0.8)).ToString()));
		Assert.AreEqual(BayonetAttachmentStyle.Plug, bayonet.Style);
		Assert.IsTrue(bayonet.BlocksFiring);
		Assert.IsTrue(0.75 >= bayonet.MinimumBore && 0.75 <= bayonet.MaximumBore);
	}

	[TestMethod]
	public void LockingCashRegister_ClosedTillCannotOpenWhileLockedAndPersistsState()
	{
		var gameworld = CreateGameworld();
		var proto = CreateProto<LockingCashRegisterGameItemComponentProto>(gameworld.Object,
			ComponentProtoModel(210, "LockingCashRegister", new XElement("Definition",
				new XAttribute("Weight", 50000),
				new XAttribute("MaxSize", (int)SizeCategory.Small),
				new XElement("ForceDifficulty", (int)Difficulty.VeryHard),
				new XElement("PickDifficulty", (int)Difficulty.Hard),
				new XElement("LockType", "Ward Lock")).ToString()));
		var parent = CreateParent(gameworld.Object, 211L, "till chest");
		var component = (LockingCashRegisterGameItemComponent)proto.CreateNew(parent.Object, temporary: true);
		parent.Setup(x => x.GetItemType<ILockable>()).Returns(component);

		component.Close();
		Assert.IsTrue(component.SetLocked(true, false));
		Assert.IsTrue(component.IsLocked);
		Assert.AreEqual(WhyCannotOpenReason.Locked, component.WhyCannotOpen(null!));
		StringAssert.Contains(InvokeSaveToXml(component), "<IsLocked>true</IsLocked>");
		Assert.IsTrue(component.SetLocked(false, false));
		Assert.IsFalse(component.IsLocked);
	}

	[TestMethod]
	public void CrossbowReadyState_LegacyDefaultsFalseAndExplicitStatePersists()
	{
		var spanningTool = CreateTag(450L, "Windlass");
		var ranged = new Mock<IRangedWeaponType>();
		ranged.SetupGet(x => x.Id).Returns(401L);
		ranged.SetupGet(x => x.Name).Returns("Windlass Crossbow");
		var gameworld = CreateGameworld(spanningTool.Object);
		gameworld.SetupGet(x => x.RangedWeaponTypes).Returns(Repository(ranged.Object));
		var proto = CreateProto<CrossbowGameItemComponentProto>(gameworld.Object,
			ComponentProtoModel(401, "Crossbow", new XElement("Definition",
				new XElement("RangedWeaponType", ranged.Object.Id),
				new XElement("MeleeWeaponType", 0),
				new XElement("RequiredSpanningToolTag", spanningTool.Object.Id)).ToString()));
		Assert.AreSame(spanningTool.Object, proto.RequiredSpanningToolTag);
		Assert.IsNotNull(proto.ReadyTemplate);
		StringAssert.Contains(InvokeSaveToXml(proto), "<RequiredSpanningToolTag>450</RequiredSpanningToolTag>");
		var parent = CreateParent(gameworld.Object, 402L, "crossbow");
		var legacy = (CrossbowGameItemComponent)proto.LoadComponent(
			new DbGameItemComponent { Definition = "<Definition><Wielded>0</Wielded><Loaded>0</Loaded></Definition>" },
			parent.Object);
		Assert.IsFalse(legacy.IsReadied);

		legacy.IsReadied = true;
		StringAssert.Contains(InvokeSaveToXml(legacy), "<IsReadied>true</IsReadied>");
		var copy = (CrossbowGameItemComponent)legacy.Copy(
			CreateParent(gameworld.Object, 403L, "copy").Object, true);
		Assert.IsTrue(copy.IsReadied);
	}

	[TestMethod]
	public void MusketBayonet_AttachmentRoutesMeleeAndPlugBlocksFiring()
	{
		var normalMelee = new Mock<IWeaponType>();
		normalMelee.SetupGet(x => x.Id).Returns(502L);
		normalMelee.SetupGet(x => x.Name).Returns("Musket Butt");
		var bayonetMelee = new Mock<IWeaponType>();
		bayonetMelee.SetupGet(x => x.Id).Returns(503L);
		bayonetMelee.SetupGet(x => x.Name).Returns("Bayonet");
		var gameworld = CreateGameworld();
		var proto = (MusketGameItemComponentProto)RuntimeHelpers.GetUninitializedObject(
			typeof(MusketGameItemComponentProto));
		proto.MeleeWeaponType = normalMelee.Object;
		typeof(MusketGameItemComponentProto)
			.GetField("_barrelBore", BindingFlags.Instance | BindingFlags.NonPublic)!
			.SetValue(proto, 0.75);
		var musketParent = CreateParent(gameworld.Object, 504L, "musket");
		musketParent.SetupGet(x => x.Size).Returns(SizeCategory.Large);
		var musket = (MusketGameItemComponent)proto.CreateNew(musketParent.Object, temporary: true);
		musket.LoadStage = 4;
		musket.IsReadied = true;

		var attachment = new Mock<IBayonetAttachment>();
		attachment.SetupGet(x => x.Style).Returns(BayonetAttachmentStyle.Plug);
		attachment.SetupGet(x => x.BlocksFiring).Returns(true);
		attachment.Setup(x => x.FitsBore(It.IsAny<double>())).Returns(true);
		var melee = new Mock<IMeleeWeapon>();
		melee.SetupGet(x => x.WeaponType).Returns(bayonetMelee.Object);
		var bayonetParent = CreateParent(gameworld.Object, 505L, "plug bayonet");
		bayonetParent.SetupGet(x => x.Size).Returns(SizeCategory.Small);
		bayonetParent.Setup(x => x.IsItemType<IBayonetAttachment>()).Returns(true);
		bayonetParent.Setup(x => x.GetItemType<IBayonetAttachment>()).Returns(attachment.Object);
		bayonetParent.Setup(x => x.GetItemType<IMeleeWeapon>()).Returns(melee.Object);
		var beltable = new Mock<IBeltable>();
		beltable.SetupGet(x => x.Parent).Returns(bayonetParent.Object);
		beltable.SetupProperty(x => x.ConnectedTo);

		Assert.AreEqual(IBeltCanAttachBeltableResult.Success, musket.CanAttachBeltable(beltable.Object));
		musket.AddConnectedItem(beltable.Object);
		Assert.IsFalse(musket.CanFire(null!, null!));
		StringAssert.Contains(musket.WhyCannotFire(null!, null!), "plug bayonet");
		Assert.AreSame(bayonetMelee.Object, ((IMeleeWeapon)musket).WeaponType);

		musket.RemoveConnectedItem(beltable.Object);
		Assert.IsTrue(musket.CanFire(null!, null!));
		Assert.AreSame(normalMelee.Object, ((IMeleeWeapon)musket).WeaponType);
	}

	[TestMethod]
	public void MusketCartridgeCompatibility_ValidatesTypeGradeBoreAndExplicitCharge()
	{
		var ranged = new Mock<IRangedWeaponType>();
		ranged.SetupGet(x => x.SpecificAmmunitionGrade).Returns("Musket Ball");
		var proto = (MusketGameItemComponentProto)RuntimeHelpers.GetUninitializedObject(
			typeof(MusketGameItemComponentProto));
		typeof(MusketGameItemComponentProto)
			.GetField("_rangedWeaponType", BindingFlags.Instance | BindingFlags.NonPublic)!
			.SetValue(proto, ranged.Object);
		typeof(MusketGameItemComponentProto)
			.GetField("_barrelBore", BindingFlags.Instance | BindingFlags.NonPublic)!
			.SetValue(proto, 0.75);
		typeof(MusketGameItemComponentProto)
			.GetField("_powderVolumePerShot", BindingFlags.Instance | BindingFlags.NonPublic)!
			.SetValue(proto, 7.0);
		var ammunition = new Mock<IAmmunitionType>();
		ammunition.SetupGet(x => x.SpecificType).Returns("Musket Ball");
		ammunition.SetupGet(x => x.RangedWeaponTypes).Returns([RangedWeaponType.Musket]);
		var cartridge = new Mock<IMusketCartridge>();
		cartridge.SetupGet(x => x.AmmoType).Returns(ammunition.Object);
		cartridge.SetupGet(x => x.BulletProto).Returns(Mock.Of<IGameItemProto>());
		cartridge.SetupGet(x => x.BulletBore).Returns(0.7);
		cartridge.SetupGet(x => x.PowderMass).Returns(7.0);

		Assert.IsTrue(proto.IsCompatibleCartridgeForLoading(cartridge.Object));
		cartridge.SetupGet(x => x.PowderMass).Returns(6.5);
		Assert.IsFalse(proto.IsCompatibleCartridgeForLoading(cartridge.Object));
		cartridge.SetupGet(x => x.PowderMass).Returns((double?)null);
		Assert.IsTrue(proto.IsCompatibleCartridgeForLoading(cartridge.Object),
			"Missing charge data must preserve legacy weapon-defined charge behavior.");
		cartridge.SetupGet(x => x.BulletBore).Returns(0.8);
		Assert.IsFalse(proto.IsCompatibleCartridgeForLoading(cartridge.Object));
		ammunition.SetupGet(x => x.SpecificType).Returns("Shotgun Shell");
		Assert.IsFalse(proto.IsCompatibleCartridgeForLoading(cartridge.Object));
		ammunition.SetupGet(x => x.SpecificType).Returns("Musket Ball");
		cartridge.SetupGet(x => x.BulletBore).Returns(0.7);
		cartridge.SetupGet(x => x.BulletProto).Returns((IGameItemProto)null!);
		Assert.IsFalse(proto.IsCompatibleCartridgeForLoading(cartridge.Object));
	}

	private static T CreateProto<T>(IFuturemud gameworld, DbGameItemComponentProto model)
		where T : GameItemComponentProto
	{
		return (T)Activator.CreateInstance(typeof(T), BindingFlags.Instance | BindingFlags.NonPublic, null,
			new object[] { model, gameworld }, CultureInfo.InvariantCulture)!;
	}

	private static Mock<IGameItem> CreateContainerCandidate(params ITag[] tags)
	{
		var item = new Mock<IGameItem>();
		item.SetupGet(x => x.Size).Returns(SizeCategory.Tiny);
		item.SetupGet(x => x.Weight).Returns(1.0);
		item.SetupGet(x => x.Quantity).Returns(1);
		item.Setup(x => x.IsA(It.IsAny<ITag>()))
			.Returns<ITag>(tag => tags.Contains(tag));
		return item;
	}

	[TestMethod]
	public void ManipulationModule_ExposesSealAndMeasurementCommands()
	{
		var commands = typeof(ManipulationModule)
		               .GetMethods(BindingFlags.Static | BindingFlags.NonPublic)
		               .Select(x => x.GetCustomAttribute<PlayerCommand>())
		               .OfType<PlayerCommand>()
		               .ToDictionary(x => x.Name, StringComparer.OrdinalIgnoreCase);

		foreach (var name in new[] { "Seal", "Break", "Inspect", "Compare", "Weigh", "Measure", "Calibrate", "Offer", "Burn" })
		{
			Assert.IsTrue(commands.ContainsKey(name), $"Expected {name} command to be registered.");
		}
	}

	[TestMethod]
	public void IncenseAndOfferingPrototypes_LoadSaveAndExposeInterfaces()
	{
		var fuelTag = CreateTag(55L, "Incense Fuel");
		var gameworld = CreateGameworld(fuelTag.Object);
		var incenseProto = CreateIncenseProto(gameworld.Object, fuelTag.Object.Id);
		var offeringProto = CreateOfferingProto(gameworld.Object);
		var incense = incenseProto.CreateNew(CreateParent(gameworld.Object, 60L, "censer").Object, temporary: true);
		var offering = offeringProto.CreateNew(CreateParent(gameworld.Object, 61L, "altar").Object, temporary: true);

		Assert.IsInstanceOfType(incenseProto, typeof(IIncenseBurnerPrototype));
		Assert.IsInstanceOfType(offeringProto, typeof(IOfferingReceiverPrototype));
		Assert.AreSame(fuelTag.Object, incenseProto.FuelTag);
		Assert.AreEqual(1, incenseProto.ScentRange);
		Assert.AreEqual(OfferingConsumptionMode.BurnOnOffer, offeringProto.ConsumptionMode);
		Assert.AreEqual(SizeCategory.Normal, offeringProto.MaximumItemSize);

		Assert.IsInstanceOfType(incense, typeof(IIncenseBurner));
		Assert.IsInstanceOfType(incense, typeof(ILightable));
		Assert.IsInstanceOfType(incense, typeof(IContainer));
		Assert.IsInstanceOfType(offering, typeof(IOfferingReceiver));
		Assert.IsInstanceOfType(offering, typeof(IContainer));

		StringAssert.Contains(InvokeSaveToXml(incenseProto), "<FuelTag>55</FuelTag>");
		StringAssert.Contains(InvokeSaveToXml(offeringProto), "<ConsumptionMode>BurnOnOffer</ConsumptionMode>");
		StringAssert.Contains(InvokeSaveToXml((GameItemComponent)incense), "<Lit>false</Lit>");
		StringAssert.Contains(InvokeSaveToXml((GameItemComponent)offering), "<Definition");
	}

	[TestMethod]
	public void AmbientScent_IsTrackableScentMetadataNotMovementTrack()
	{
		var gameworld = CreateGameworld();
		var owner = new Mock<IPerceivable>();
		owner.SetupGet(x => x.Gameworld).Returns(gameworld.Object);
		var actor = CreateActor(gameworld.Object, 70L);
		var scent = new AmbientScent(owner.Object, 99L, "a bronze censer",
			"Sweet resinous smoke lingers here.", RoomLayer.GroundLevel, 1, Difficulty.Easy);

		Assert.IsInstanceOfType(scent, typeof(IScentTrailEffect));
		Assert.IsFalse(scent is ITrack, "Ambient scents should augment tracks output without becoming movement tracks.");
		Assert.AreEqual(99L, scent.SourceItemId);
		Assert.AreEqual(RoomLayer.GroundLevel, scent.RoomLayer);
		Assert.AreEqual(Difficulty.Easy, scent.ScentDifficulty(actor.Object));
		StringAssert.Contains(scent.DescribeForTracksCommand(actor.Object), "bronze censer");
		StringAssert.Contains(scent.SaveToXml(new Dictionary<IEffect, TimeSpan>()).ToString(), "AmbientScent");
	}

	[TestMethod]
	public void Sealable_SealsBreaksSavesCopiesAndCompares()
	{
		var gameworld = CreateGameworld();
		var targetParent = CreateParent(gameworld.Object, 10L, "sealed tablet");
		var stampParent = CreateStampParent(gameworld.Object, 11L, "bronze");
		var actor = CreateActor(gameworld.Object, 20L);
		var stampProto = CreateSealStampProto(gameworld.Object, "a lion seal", "Temple", "bronze");
		var sealableProto = CreateSealableProto(gameworld.Object);
		var stamp = (SealStampGameItemComponent)stampProto.CreateNew(stampParent.Object, temporary: true);
		var sealable = (SealableGameItemComponent)sealableProto.CreateNew(targetParent.Object, temporary: true);

		Assert.IsTrue(sealable.CanSeal(actor.Object, stamp, null, out var error), error);

		sealable.Seal(actor.Object, stamp, null);

		Assert.IsTrue(sealable.IsSealed);
		Assert.AreEqual("a lion seal", sealable.CurrentSeal?.SealDesign);
		Assert.IsTrue(sealable.SealMatches(stamp));
		StringAssert.Contains(sealable.InspectSeal(actor.Object), "a lion seal");

		sealable.HandleEvent(MudSharp.Events.EventType.ItemOpened);

		Assert.IsFalse(sealable.IsSealed);
		Assert.IsTrue(sealable.SealBroken);
		Assert.IsTrue(sealable.HasSealResidue);
		StringAssert.Contains(InvokeSaveToXml(sealable), "<SealBroken>true</SealBroken>");

		var copy = (SealableGameItemComponent)sealable.Copy(CreateParent(gameworld.Object, 12L, "copy").Object, true);
		Assert.IsTrue(copy.SealBroken);
		Assert.AreEqual("a lion seal", copy.CurrentSeal?.SealDesign);
	}

	[TestMethod]
	public void MeasuringInstrument_DriftCalibrationAndWrongCalibrationPersist()
	{
		var gameworld = CreateGameworld();
		var actor = CreateActor(gameworld.Object, 30L);
		var proto = CreateMeasuringProto(gameworld.Object, "Weight", 0.001, 10000.0, 0.001);
		var poor = (MeasuringInstrumentGameItemComponent)proto.CreateNew(
			CreateParent(gameworld.Object, 20L, "poor scale", quality: ItemQuality.Terrible).Object, temporary: true);
		var excellent = (MeasuringInstrumentGameItemComponent)proto.CreateNew(
			CreateParent(gameworld.Object, 22L, "excellent scale", quality: ItemQuality.Legendary).Object, temporary: true);
		var target = CreateParent(gameworld.Object, 40L, "weight", weight: 1000.0);

		var poorResult = poor.Measure(actor.Object, target.Object);
		var excellentResult = excellent.Measure(actor.Object, target.Object);

		Assert.AreEqual(1, poor.UsesSinceCalibration);
		Assert.IsTrue(Math.Abs(poorResult.Drift) > Math.Abs(excellentResult.Drift));

		Assert.IsTrue(poor.CalibrateWrong(actor.Object, 0.10, true, out var error), error);
		Assert.IsTrue(poor.HasDeliberateBias);
		Assert.IsTrue(poor.CalibrationBiasIsPercentage);
		Assert.AreEqual(0.10, poor.CalibrationBias, 0.000001);

		var biasedResult = poor.Measure(actor.Object, target.Object);
		Assert.IsTrue(Math.Abs(biasedResult.CalibrationBias) > 0.0);
		StringAssert.Contains(poor.InspectCalibration(actor.Object), "Deliberate Bias");

		poor.Calibrate(actor.Object);

		Assert.IsFalse(poor.HasDeliberateBias);
		Assert.AreEqual(0, poor.UsesSinceCalibration);
		StringAssert.Contains(InvokeSaveToXml(poor), "<HasDeliberateBias>false</HasDeliberateBias>");
	}

	[TestMethod]
	public void MeasuringInstrument_LoadsSavesAndCopiesCalibrationState()
	{
		var gameworld = CreateGameworld();
		var proto = CreateMeasuringProto(gameworld.Object, "FluidVolume", 0.01, 2.0, 0.0005);
		var component = (MeasuringInstrumentGameItemComponent)proto.LoadComponent(
			new DbGameItemComponent
			{
				Definition = new XElement("Definition",
					new XElement("CalibrationBias", -0.05),
					new XElement("CalibrationBiasIsPercentage", true),
					new XElement("HasDeliberateBias", true),
					new XElement("UsesSinceCalibration", 7),
					new XElement("DriftDirection", -1)).ToString()
			},
			CreateParent(gameworld.Object, 50L, "cup").Object);

		Assert.AreEqual(MeasuringInstrumentMode.FluidVolume, component.Mode);
		Assert.AreEqual(-0.05, component.CalibrationBias, 0.000001);
		Assert.IsTrue(component.CalibrationBiasIsPercentage);
		Assert.IsTrue(component.HasDeliberateBias);
		Assert.AreEqual(7, component.UsesSinceCalibration);

		var copy = (MeasuringInstrumentGameItemComponent)component.Copy(
			CreateParent(gameworld.Object, 51L, "copy").Object, true);

		Assert.AreEqual(component.CalibrationBias, copy.CalibrationBias, 0.000001);
		Assert.AreEqual(component.UsesSinceCalibration, copy.UsesSinceCalibration);
		StringAssert.Contains(InvokeSaveToXml(component), "<UsesSinceCalibration>7</UsesSinceCalibration>");
	}

	private static SealStampGameItemComponentProto CreateSealStampProto(IFuturemud gameworld, string design,
		string issuer, string material)
	{
		return (SealStampGameItemComponentProto)Activator.CreateInstance(
			typeof(SealStampGameItemComponentProto),
			BindingFlags.Instance | BindingFlags.NonPublic,
			null,
			new object[]
			{
				ComponentProtoModel(100, "SealStamp", new XElement("Definition",
					new XElement("SealDesign", new XCData(design)),
					new XElement("IssuerText", new XCData(issuer)),
					new XElement("OwnerText", new XCData(string.Empty)),
					new XElement("ClanText", new XCData(string.Empty)),
					new XElement("OfficeText", new XCData("scribe")),
					new XElement("StampMaterial", new XCData(material)),
					new XElement("ForgeryDifficulty", (int)Difficulty.Hard),
					new XElement("AuthorityProg", 0)).ToString()),
				gameworld
			},
			CultureInfo.InvariantCulture)!;
	}

	private static SealableGameItemComponentProto CreateSealableProto(IFuturemud gameworld)
	{
		return (SealableGameItemComponentProto)Activator.CreateInstance(
			typeof(SealableGameItemComponentProto),
			BindingFlags.Instance | BindingFlags.NonPublic,
			null,
			new object[]
			{
				ComponentProtoModel(101, "Sealable", new XElement("Definition",
					new XElement("AllowedMedia", new XElement("Medium", new XCData("wax"))),
					new XElement("InspectionDifficulty", (int)Difficulty.Normal),
					new XElement("BrokenSealLeavesResidue", true)).ToString()),
				gameworld
			},
			CultureInfo.InvariantCulture)!;
	}

	private static MeasuringInstrumentGameItemComponentProto CreateMeasuringProto(IFuturemud gameworld, string mode,
		double precision, double capacity, double baseDrift)
	{
		return (MeasuringInstrumentGameItemComponentProto)Activator.CreateInstance(
			typeof(MeasuringInstrumentGameItemComponentProto),
			BindingFlags.Instance | BindingFlags.NonPublic,
			null,
			new object[]
			{
				ComponentProtoModel(102, "MeasuringInstrument", new XElement("Definition",
					new XElement("Mode", mode),
					new XElement("Precision", precision),
					new XElement("Capacity", capacity),
					new XElement("BaseDriftPerUse", baseDrift),
					new XElement("MaximumDrift", 0.10),
					new XElement("MaximumWrongCalibration", 0.50),
					new XElement("CalibrationInspectionDifficulty", (int)Difficulty.Normal)).ToString()),
				gameworld
			},
			CultureInfo.InvariantCulture)!;
	}

	private static IncenseBurnerGameItemComponentProto CreateIncenseProto(IFuturemud gameworld, long fuelTagId)
	{
		return (IncenseBurnerGameItemComponentProto)Activator.CreateInstance(
			typeof(IncenseBurnerGameItemComponentProto),
			BindingFlags.Instance | BindingFlags.NonPublic,
			null,
			new object[]
			{
				ComponentProtoModel(103, "IncenseBurner", new XElement("Definition",
					new XElement("FuelTag", fuelTagId),
					new XElement("MaximumFuelWeight", 750.0),
					new XElement("SecondsPerUnitWeight", 45.0),
					new XElement("ScentRange", 1),
					new XElement("DrugRange", 0),
					new XElement("DrugPulseSeconds", 10),
					new XElement("LingeringMultiplier", 5.0),
					new XElement("SourceScentDescription", new XCData("Sweet resinous smoke curls from $0.")),
					new XElement("DistantScentDescription", new XCData("A faint sweet smoke drifts in from nearby.")),
					new XElement("ScentDifficulty", (int)Difficulty.Easy),
					new XElement("Drug", 0),
					new XElement("GramsPerPulse", 0.0)).ToString()),
				gameworld
			},
			CultureInfo.InvariantCulture)!;
	}

	private static OfferingReceiverGameItemComponentProto CreateOfferingProto(IFuturemud gameworld)
	{
		return (OfferingReceiverGameItemComponentProto)Activator.CreateInstance(
			typeof(OfferingReceiverGameItemComponentProto),
			BindingFlags.Instance | BindingFlags.NonPublic,
			null,
			new object[]
			{
				ComponentProtoModel(104, "OfferingReceiver", new XElement("Definition",
					new XElement("AllowedTags"),
					new XElement("BlockedTags"),
					new XElement("MaximumContentsWeight", 15000.0),
					new XElement("MaximumItemSize", (int)SizeCategory.Normal),
					new XElement("ConsumptionMode", "BurnOnOffer"),
					new XElement("ResidueItemProto", 0),
					new XElement("CanOfferProg", 0),
					new XElement("OnOfferProg", 0),
					new XElement("OnBurnProg", 0),
					new XElement("AcceptEcho", new XCData("@ lay|lays $1 in $2.")),
					new XElement("BurnEcho", new XCData("@ burn|burns $1 in $2.")),
					new XElement("RejectEcho", new XCData("$2 rejects $1."))).ToString()),
				gameworld
			},
			CultureInfo.InvariantCulture)!;
	}

	private static DbGameItemComponentProto ComponentProtoModel(long id, string type, string definition)
	{
		return new DbGameItemComponentProto
		{
			Id = id,
			Type = type,
			Name = type,
			Description = $"{type} component",
			Definition = definition,
			RevisionNumber = 1,
			EditableItem = new DbEditableItem
			{
				Id = id,
				BuilderAccountId = 1,
				BuilderDate = DateTime.UtcNow,
				RevisionNumber = 1,
				RevisionStatus = (int)RevisionStatus.Current
			}
		};
	}

	private static string InvokeSaveToXml(GameItemComponent component)
	{
		return (string)component.GetType()
		                .GetMethod("SaveToXml", BindingFlags.Instance | BindingFlags.NonPublic)!
		                .Invoke(component, Array.Empty<object>())!;
	}

	private static string InvokeSaveToXml(GameItemComponentProto proto)
	{
		return (string)proto.GetType()
		                    .GetMethod("SaveToXml", BindingFlags.Instance | BindingFlags.NonPublic)!
		                    .Invoke(proto, Array.Empty<object>())!;
	}

	private static Mock<IGameItem> CreateParent(IFuturemud gameworld, long id, string seen, double weight = 0.0,
		ItemQuality quality = ItemQuality.Standard)
	{
		var proto = new Mock<IGameItemProto>();
		proto.SetupGet(x => x.Id).Returns(id + 1000);
		var parent = new Mock<IGameItem>();
		parent.SetupGet(x => x.Id).Returns(id);
		parent.SetupGet(x => x.Gameworld).Returns(gameworld);
		parent.SetupGet(x => x.Prototype).Returns(proto.Object);
		parent.SetupGet(x => x.Quality).Returns(quality);
		parent.SetupGet(x => x.Weight).Returns(weight);
		parent.Setup(x => x.GetItemType<ILiquidContainer>()).Returns((ILiquidContainer)null!);
		parent.Setup(x => x.HowSeen(It.IsAny<IPerceiver>(), It.IsAny<bool>(), It.IsAny<DescriptionType>(),
				It.IsAny<bool>(), It.IsAny<PerceiveIgnoreFlags>()))
		      .Returns(seen);
		return parent;
	}

	private static Mock<IGameItem> CreateStampParent(IFuturemud gameworld, long id, string materialName)
	{
		var solid = new Mock<ISolid>();
		solid.SetupGet(x => x.Name).Returns(materialName);
		var parent = CreateParent(gameworld, id, "stamp");
		parent.SetupGet(x => x.Material).Returns(solid.Object);
		return parent;
	}

	private static Mock<ICharacter> CreateActor(IFuturemud gameworld, long id)
	{
		var actor = new Mock<ICharacter>();
		actor.SetupGet(x => x.Id).Returns(id);
		actor.SetupGet(x => x.Gameworld).Returns(gameworld);
		actor.Setup(x => x.IsAdministrator(It.IsAny<PermissionLevel>())).Returns(true);
		actor.Setup(x => x.HowSeen(It.IsAny<IPerceiver>(), It.IsAny<bool>(), It.IsAny<DescriptionType>(),
				It.IsAny<bool>(), It.IsAny<PerceiveIgnoreFlags>()))
		     .Returns("Tester");
		return actor;
	}

	private static (Mock<ICharacter> Actor, Mock<IBody> Body, Mock<ICell> Cell) CreateInstrumentActor(
		IFuturemud gameworld, Mock<IGameItem> instrument)
	{
		var actor = CreateActor(gameworld, instrument.Object.Id + 100);
		var body = new Mock<IBody>();
		var cell = new Mock<ICell>();
		var position = new Mock<IPositionState>();
		var output = new Mock<IOutputHandler>();
		var firstHand = new Mock<IGrab>();
		var secondHand = new Mock<IGrab>();
		body.SetupGet(x => x.HeldOrWieldedItems).Returns([instrument.Object]);
		body.SetupGet(x => x.WornItems).Returns([]);
		body.SetupGet(x => x.FunctioningFreeHands).Returns([firstHand.Object, secondHand.Object]);
		instrument.SetupGet(x => x.Location).Returns(cell.Object);
		instrument.SetupGet(x => x.InInventoryOf).Returns((IBody)null!);
		instrument.SetupGet(x => x.ContainedIn).Returns((IGameItem)null!);
		actor.SetupGet(x => x.Body).Returns(body.Object);
		actor.SetupGet(x => x.State).Returns(CharacterState.Able);
		actor.SetupGet(x => x.Combat).Returns((ICombat)null!);
		actor.SetupGet(x => x.PositionState).Returns(position.Object);
		actor.SetupGet(x => x.Location).Returns(cell.Object);
		actor.SetupGet(x => x.OutputHandler).Returns(output.Object);
		actor.Setup(x => x.CanSpendStamina(It.IsAny<double>())).Returns(true);
		actor.SetupGet(x => x.Effects).Returns([]);
		actor.Setup(x => x.EffectsOfType<PlayingInstrument>(It.IsAny<Predicate<PlayingInstrument>>()))
		     .Returns([]);
		position.SetupGet(x => x.Name).Returns("standing");
		return (actor, body, cell);
	}

	private static Mock<ITag> CreateTag(long id, string name)
	{
		var tag = new Mock<ITag>();
		tag.SetupGet(x => x.Id).Returns(id);
		tag.SetupGet(x => x.Name).Returns(name);
		tag.SetupGet(x => x.FullName).Returns(name);
		tag.Setup(x => x.IsA(It.IsAny<ITag>())).Returns<ITag>(other => ReferenceEquals(other, tag.Object));
		return tag;
	}

	private static Mock<IFuturemud> CreateGameworld(params ITag[] tags)
	{
		var saveManager = new Mock<ISaveManager>();
		var unitManager = new Mock<IUnitManager>();
		unitManager.Setup(x => x.DescribeExact(It.IsAny<double>(), It.IsAny<UnitType>(), It.IsAny<IPerceiver>()))
		           .Returns<double, UnitType, IPerceiver>((value, type, _) => $"{value:N3} {type}");
		unitManager.Setup(x => x.DescribeExact(It.IsAny<double>(), It.IsAny<UnitType>(), It.IsAny<string>(),
				It.IsAny<IFormatProvider>()))
		           .Returns<double, UnitType, string, IFormatProvider>((value, type, _, _) => $"{value:N3} {type}");
		var gameworld = new Mock<IFuturemud>();
		gameworld.SetupGet(x => x.SaveManager).Returns(saveManager.Object);
		gameworld.SetupGet(x => x.UnitManager).Returns(unitManager.Object);
		gameworld.SetupGet(x => x.FutureProgs).Returns(Repository<IFutureProg>());
		gameworld.SetupGet(x => x.Tags).Returns(Repository(tags));
		gameworld.SetupGet(x => x.Traits).Returns(Repository<ITraitDefinition>());
		gameworld.SetupGet(x => x.Drugs).Returns(Repository<IDrug>());
		gameworld.SetupGet(x => x.ItemProtos).Returns(RevisableRepository<IGameItemProto>());
		gameworld.SetupGet(x => x.AmmunitionTypes).Returns(Repository<IAmmunitionType>());
		gameworld.SetupGet(x => x.RangedWeaponTypes).Returns(Repository<IRangedWeaponType>());
		gameworld.SetupGet(x => x.WeaponTypes).Returns(Repository<IWeaponType>());
		gameworld.SetupGet(x => x.BodypartPrototypes).Returns(Repository<IBodypart>());
		return gameworld;
	}

	private static IUneditableAll<T> Repository<T>(params T[] items) where T : class, IFrameworkItem
	{
		var repository = new Mock<IUneditableAll<T>>();
		repository.Setup(x => x.Get(It.IsAny<long>()))
		          .Returns<long>(id => items.FirstOrDefault(x => x.Id == id));
		repository.Setup(x => x.GetByName(It.IsAny<string>()))
		          .Returns<string>(name => items.FirstOrDefault(x => x.Name.EqualTo(name)));
		repository.Setup(x => x.Get(It.IsAny<string>()))
		          .Returns<string>(name => items.Where(x => x.Name.EqualTo(name)).ToList());
		repository.Setup(x => x.GetEnumerator())
		          .Returns(() => ((IEnumerable<T>)items).GetEnumerator());
		repository.SetupGet(x => x.Count).Returns(items.Length);
		return repository.Object;
	}

	private static IUneditableRevisableAll<T> RevisableRepository<T>(params T[] items) where T : class, IRevisableItem
	{
		var repository = new Mock<IUneditableRevisableAll<T>>();
		repository.Setup(x => x.Get(It.IsAny<long>()))
		          .Returns<long>(id => items.FirstOrDefault(x => x.Id == id)!);
		repository.Setup(x => x.Get(It.IsAny<long>(), It.IsAny<int>()))
		          .Returns<long, int>((id, revision) => items.FirstOrDefault(x => x.Id == id && x.RevisionNumber == revision)!);
		repository.Setup(x => x.GetByName(It.IsAny<string>(), It.IsAny<bool>()))
		          .Returns<string, bool>((name, ignoreCase) => items.FirstOrDefault(x =>
			          ignoreCase ? x.Name.EqualTo(name) : x.Name == name)!);
		repository.Setup(x => x.Get(It.IsAny<string>()))
		          .Returns<string>(name => items.Where(x => x.Name.EqualTo(name)).ToList());
		repository.Setup(x => x.GetEnumerator())
		          .Returns(() => ((IEnumerable<T>)items).GetEnumerator());
		return repository.Object;
	}
}
