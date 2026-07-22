#nullable enable

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Body;
using MudSharp.Character;
using MudSharp.Commands.Modules;
using MudSharp.Construction;
using MudSharp.Effects.Interfaces;
using MudSharp.Form.Shape;
using MudSharp.Framework;
using MudSharp.NPC;
using MudSharp.NPC.AI;
using System.IO;
using System.Linq;

namespace MudSharp_Unit_Tests;

[TestClass]
public class CharacterInstanceServiceTests
{
	[TestMethod]
	public void ValidateSecondarySpawnMode_Passive_AllowsPlayerCharacters()
	{
		var pc = BuildPlayerCharacter();

		var result = CharacterInstanceService.ValidateSecondarySpawnMode(pc.Object,
			SecondaryCharacterInstanceSpawnMode.Passive);

		Assert.IsTrue(result.Success);
	}

	[TestMethod]
	public void CreateSpawnOptionsForMode_Passive_PreservesPhaseFourDefaults()
	{
		var pc = BuildPlayerCharacter();
		var form = BuildForm();
		var location = new Mock<ICell>();

		var options = CharacterInstanceService.CreateSpawnOptionsForMode(pc.Object, form.Object, location.Object,
			RoomLayer.GroundLevel, CharacterInstancePersistencePolicy.DespawnOnReboot,
			SecondaryCharacterInstanceSpawnMode.Passive);

		Assert.AreEqual(CharacterInstanceKind.Other, options.InstanceKind);
		Assert.AreEqual(CharacterInstanceControlPolicy.NotControllable, options.ControlPolicy);
		Assert.AreEqual(CharacterInstanceDeathPolicy.DestroyInstanceOnly, options.DeathPolicy);
		Assert.AreEqual(CharacterInstancePerceptionPolicy.OrdinaryEmbodied, options.PerceptionPolicy);
		Assert.AreEqual(CharacterInstancePersistencePolicy.DespawnOnReboot, options.PersistencePolicy);
		Assert.AreEqual("astral form", options.InstanceName);
	}

	[TestMethod]
	public void CreateAstralProjectionSpawnOptions_SetsProjectionPoliciesAndMetadata()
	{
		var pc = BuildPlayerCharacter();
		pc.SetupGet(x => x.Id).Returns(10);
		pc.SetupGet(x => x.InstanceId).Returns(101);
		var form = BuildForm(202, "silver projection");
		var location = new Mock<ICell>();

		var options = CharacterInstanceService.CreateAstralProjectionSpawnOptions(pc.Object, form.Object,
			location.Object, RoomLayer.GroundLevel, 303, AstralProjectionAnchorPolicy.Sleep, 404, "astral");

		Assert.AreEqual(CharacterInstanceKind.AstralProjection, options.InstanceKind);
		Assert.AreEqual(CharacterInstanceControlPolicy.PlayerFocusable, options.ControlPolicy);
		Assert.AreEqual(CharacterInstanceDeathPolicy.CollapseToAnchor, options.DeathPolicy);
		Assert.AreEqual(CharacterInstancePerceptionPolicy.PlanarProjection, options.PerceptionPolicy);
		Assert.AreEqual(CharacterInstancePersistencePolicy.DespawnOnReboot, options.PersistencePolicy);
		Assert.AreEqual("silver projection", options.InstanceName);
		Assert.IsTrue(CharacterInstanceMetadata.TryGetAstralProjectionMetadata(options.EffectData,
			out var metadata));
		Assert.AreEqual(10, metadata.AnchorCharacterId);
		Assert.AreEqual(101, metadata.AnchorInstanceId);
		Assert.AreEqual(202, metadata.ProjectionBodyId);
		Assert.AreEqual(303, metadata.PlaneId);
		Assert.AreEqual(AstralProjectionAnchorPolicy.Sleep, metadata.AnchorPolicy);
		Assert.AreEqual(404, metadata.SourceSpellId);
		Assert.AreEqual("astral", metadata.FormKey);
	}

	[TestMethod]
	public void CreateMagicalCopySpawnOptions_SetsCopyPoliciesAndMetadata()
	{
		var pc = BuildPlayerCharacter();
		pc.SetupGet(x => x.Id).Returns(11);
		pc.SetupGet(x => x.InstanceId).Returns(111);
		var form = BuildForm(222, "mirror copy");
		var location = new Mock<ICell>();

		var options = CharacterInstanceService.CreateMagicalCopySpawnOptions(pc.Object, form.Object,
			location.Object, RoomLayer.GroundLevel, 333, 444, "copy", true, true,
			CharacterInstancePersistencePolicy.DespawnOnLogout);

		Assert.AreEqual(CharacterInstanceKind.MagicalCopy, options.InstanceKind);
		Assert.AreEqual(CharacterInstanceControlPolicy.PlayerFocusable, options.ControlPolicy);
		Assert.AreEqual(CharacterInstanceDeathPolicy.CollapseToAnchor, options.DeathPolicy);
		Assert.AreEqual(CharacterInstancePerceptionPolicy.PlanarProjection, options.PerceptionPolicy);
		Assert.AreEqual(CharacterInstancePersistencePolicy.DespawnOnLogout, options.PersistencePolicy);
		Assert.AreEqual("mirror copy", options.InstanceName);
		Assert.IsTrue(CharacterInstanceMetadata.TryGetMagicalCopyMetadata(options.EffectData, out var metadata));
		Assert.AreEqual(11, metadata.AnchorCharacterId);
		Assert.AreEqual(111, metadata.AnchorInstanceId);
		Assert.AreEqual(222, metadata.CopyBodyId);
		Assert.AreEqual(333, metadata.PlaneId);
		Assert.AreEqual(444, metadata.SourceSpellId);
		Assert.AreEqual("copy", metadata.FormKey);
		Assert.IsTrue(metadata.PlayerFocusable);
		Assert.IsTrue(metadata.Intangible);
		Assert.AreEqual(CharacterInstancePersistencePolicy.DespawnOnLogout, metadata.PersistencePolicy);
	}

	[TestMethod]
	public void CreatePhysicalCloneSpawnOptions_SetsClonePoliciesAndMetadata()
	{
		var pc = BuildPlayerCharacter();
		pc.SetupGet(x => x.Id).Returns(12);
		pc.SetupGet(x => x.InstanceId).Returns(121);
		var form = BuildForm(232, "spare clone");
		var location = new Mock<ICell>();

		var options = CharacterInstanceService.CreatePhysicalCloneSpawnOptions(pc.Object, form.Object,
			location.Object, RoomLayer.GroundLevel, 454, "clone", false,
			CharacterInstancePersistencePolicy.Persistent);

		Assert.AreEqual(CharacterInstanceKind.PhysicalClone, options.InstanceKind);
		Assert.AreEqual(CharacterInstanceControlPolicy.NotControllable, options.ControlPolicy);
		Assert.AreEqual(CharacterInstanceDeathPolicy.DestroyInstanceOnly, options.DeathPolicy);
		Assert.AreEqual(CharacterInstancePerceptionPolicy.OrdinaryEmbodied, options.PerceptionPolicy);
		Assert.AreEqual(CharacterInstancePersistencePolicy.Persistent, options.PersistencePolicy);
		Assert.AreEqual("spare clone", options.InstanceName);
		Assert.IsTrue(CharacterInstanceMetadata.TryGetPhysicalCloneMetadata(options.EffectData, out var metadata));
		Assert.AreEqual(12, metadata.AnchorCharacterId);
		Assert.AreEqual(121, metadata.AnchorInstanceId);
		Assert.AreEqual(232, metadata.CloneBodyId);
		Assert.AreEqual(454, metadata.SourceSpellId);
		Assert.AreEqual("clone", metadata.FormKey);
		Assert.IsFalse(metadata.PlayerFocusable);
		Assert.AreEqual(CharacterInstancePersistencePolicy.Persistent, metadata.PersistencePolicy);
	}

	[TestMethod]
	public void CreatePhysicalCloneSpawnOptions_PersistentSpawn_PreservesExactRouteCoordinate()
	{
		var pc = BuildPlayerCharacter();
		var form = BuildForm(232, "persistent clone");
		var routeCell = BuildRouteCell(42, 10_000.0, 125.0);
		var location = new SpatialLocation(routeCell.Cell.Object, RoomLayer.GroundLevel, 7_150.125);

		var options = CharacterInstanceService.CreatePhysicalCloneSpawnOptions(
			pc.Object,
			form.Object,
			location,
			454,
			"persistent-clone",
			false,
			CharacterInstancePersistencePolicy.Persistent);

		Assert.AreEqual(CharacterInstancePersistencePolicy.Persistent, options.PersistencePolicy);
		Assert.AreEqual(location, options.SpawnLocation);
		Assert.IsTrue(CharacterInstanceService.TryGetPersistedSpatialLocation(
			options.SpawnLocation,
			out var persisted,
			out var error), error);
		Assert.AreEqual(42L, persisted.LocationId);
		Assert.AreEqual((int)RoomLayer.GroundLevel, persisted.RoomLayerValue);
		Assert.IsTrue(persisted.RoutePosition.HasValue);
		Assert.AreEqual(7_150.125m, persisted.RoutePosition.Value);
	}

	[TestMethod]
	public void CreateScriptedAiSpawnOptions_TemporarySpawn_PreservesAndRoundsExactRouteCoordinate()
	{
		var pc = BuildPlayerCharacter();
		var form = BuildForm(242, "temporary double");
		var routeCell = BuildRouteCell(43, 10_000.0, 125.0);
		var location = new SpatialLocation(routeCell.Cell.Object, RoomLayer.InTrees, 7_150.1236);

		var options = CharacterInstanceService.CreateScriptedAiSpawnOptions(
			pc.Object,
			form.Object,
			location,
			persistencePolicy: CharacterInstancePersistencePolicy.DespawnOnReboot);

		Assert.AreEqual(CharacterInstancePersistencePolicy.DespawnOnReboot, options.PersistencePolicy);
		Assert.AreEqual(location, options.SpawnLocation);
		Assert.IsTrue(CharacterInstanceService.TryGetPersistedSpatialLocation(
			options.SpawnLocation,
			out var persisted,
			out var error), error);
		Assert.IsTrue(persisted.RoutePosition.HasValue);
		Assert.AreEqual(7_150.124m, persisted.RoutePosition.Value);
	}

	[TestMethod]
	public void CreateDefaultSpawnLocation_RouteCell_UsesAuthoredDefault()
	{
		var routeCell = BuildRouteCell(44, 10_000.0, 3_750.5);

		var location = CharacterInstanceService.CreateDefaultSpawnLocation(
			routeCell.Cell.Object,
			RoomLayer.GroundLevel);

		Assert.AreSame(routeCell.Cell.Object, location.Cell);
		Assert.AreEqual(RoomLayer.GroundLevel, location.Layer);
		Assert.IsTrue(location.RoutePositionMetres.HasValue);
		Assert.AreEqual(3_750.5, location.RoutePositionMetres.Value);
	}

	[TestMethod]
	public void CreateDefaultSpawnLocation_OrdinaryCell_PreservesLegacyNullCoordinate()
	{
		var cell = new Mock<ICell>();

		var location = CharacterInstanceService.CreateDefaultSpawnLocation(cell.Object, RoomLayer.OnRooftops);

		Assert.AreSame(cell.Object, location.Cell);
		Assert.AreEqual(RoomLayer.OnRooftops, location.Layer);
		Assert.IsNull(location.RoutePositionMetres);
	}

	[TestMethod]
	public void ValidateSecondarySpawnOptions_InvalidRouteCoordinates_FailBeforePersistence()
	{
		var pc = BuildPlayerCharacter();
		var form = BuildForm();
		var routeCell = BuildRouteCell(45, 10_000.0, 100.0);
		var missingCoordinate = new SecondaryCharacterInstanceSpawnOptions
		{
			Owner = pc.Object,
			Form = form.Object,
			SpawnLocation = new SpatialLocation(routeCell.Cell.Object, RoomLayer.GroundLevel),
			ControlPolicy = CharacterInstanceControlPolicy.NotControllable
		};
		var outOfBounds = missingCoordinate with
		{
			SpawnLocation = new SpatialLocation(routeCell.Cell.Object, RoomLayer.GroundLevel, 10_000.001)
		};

		var missingResult = CharacterInstanceService.ValidateSecondarySpawnOptions(missingCoordinate);
		var outOfBoundsResult = CharacterInstanceService.ValidateSecondarySpawnOptions(outOfBounds);

		Assert.IsFalse(missingResult.Success);
		StringAssert.Contains(missingResult.Message, "requires a route coordinate");
		Assert.IsFalse(outOfBoundsResult.Success);
		StringAssert.Contains(outOfBoundsResult.Message, "outside RouteCell");
	}

	[TestMethod]
	public void TryGetPersistedSpatialLocation_CoordinateOutsideDecimalSchema_FailsClosed()
	{
		var routeCell = BuildRouteCell(49, 2_000_000_000_000_000.0, 0.0);
		var location = new SpatialLocation(
			routeCell.Cell.Object,
			RoomLayer.GroundLevel,
			2_000_000_000_000_000.0);

		var success = CharacterInstanceService.TryGetPersistedSpatialLocation(
			location,
			out _,
			out var error);

		Assert.IsFalse(success);
		StringAssert.Contains(error, "decimal(18,3)");
	}

	[TestMethod]
	public void ResolvePersistedSpatialLocation_RebootLoad_RestoresExactCoordinate()
	{
		var routeCell = BuildRouteCell(46, 10_000.0, 100.0);
		var instance = new MudSharp.Models.CharacterInstance
		{
			Id = 601,
			LocationId = 46,
			RoomLayer = (int)RoomLayer.GroundLevel,
			RoutePosition = 7_150.125m
		};

		var location = CharacterInstanceService.ResolvePersistedSpatialLocation(instance, routeCell.Cell.Object);

		Assert.AreSame(routeCell.Cell.Object, location.Cell);
		Assert.AreEqual(RoomLayer.GroundLevel, location.Layer);
		Assert.IsTrue(location.RoutePositionMetres.HasValue);
		Assert.AreEqual(7_150.125, location.RoutePositionMetres.Value);
	}

	[TestMethod]
	public void ResolvePersistedSpatialLocation_InvalidRebootRow_ReportsInstanceAndReason()
	{
		var routeCell = BuildRouteCell(47, 10_000.0, 100.0);
		var instance = new MudSharp.Models.CharacterInstance
		{
			Id = 602,
			LocationId = 47,
			RoomLayer = (int)RoomLayer.GroundLevel,
			RoutePosition = null
		};

		var exception = Assert.ThrowsException<InvalidDataException>(() =>
			CharacterInstanceService.ResolvePersistedSpatialLocation(instance, routeCell.Cell.Object));

		StringAssert.Contains(exception.Message, "Character instance #602");
		StringAssert.Contains(exception.Message, "requires a route coordinate");
	}

	[TestMethod]
	public void NpcLoadHere_ResolvesActorsExactEffectiveRouteCoordinate()
	{
		var routeCell = BuildRouteCell(48, 10_000.0, 100.0);
		var actorLocation = new SpatialLocation(routeCell.Cell.Object, RoomLayer.GroundLevel, 7_150.75);
		var actor = BuildPlayerCharacter();
		actor.SetupGet(x => x.SpatialLocation).Returns(actorLocation);

		var success = NPCBuilderModule.TryResolveNpcLoadSpatialLocation(
			actor.Object,
			out var resolved,
			out var error);

		Assert.IsTrue(success, error);
		Assert.AreEqual(actorLocation, resolved);
	}

	[TestMethod]
	public void NpcLoadHere_OrdinaryCell_PreservesLegacyCellAndLayer()
	{
		var cell = new Mock<ICell>();
		var actorLocation = new SpatialLocation(cell.Object, RoomLayer.InTrees);
		var actor = BuildPlayerCharacter();
		actor.SetupGet(x => x.SpatialLocation).Returns(actorLocation);

		var success = NPCBuilderModule.TryResolveNpcLoadSpatialLocation(
			actor.Object,
			out var resolved,
			out var error);

		Assert.IsTrue(success, error);
		Assert.AreEqual(actorLocation, resolved);
		Assert.IsNull(resolved.RoutePositionMetres);
	}

	[TestMethod]
	public void CreatePossessedBodySpawnOptions_SetsPossessionPoliciesAndMetadata()
	{
		var pc = BuildPlayerCharacter();
		pc.SetupGet(x => x.Id).Returns(14);
		pc.SetupGet(x => x.InstanceId).Returns(141);
		var form = BuildForm(252, "borrowed shell");
		var location = new Mock<ICell>();

		var options = CharacterInstanceService.CreatePossessedBodySpawnOptions(pc.Object, form.Object,
			location.Object, RoomLayer.GroundLevel, 515, 616, 717, "possessed-body:515");

		Assert.AreEqual(CharacterInstanceKind.PossessedBody, options.InstanceKind);
		Assert.AreEqual(CharacterInstanceControlPolicy.PlayerFocusable, options.ControlPolicy);
		Assert.AreEqual(CharacterInstanceDeathPolicy.CollapseToAnchor, options.DeathPolicy);
		Assert.AreEqual(CharacterInstancePerceptionPolicy.OrdinaryEmbodied, options.PerceptionPolicy);
		Assert.AreEqual(CharacterInstancePersistencePolicy.DespawnOnReboot, options.PersistencePolicy);
		Assert.AreEqual("borrowed shell", options.InstanceName);
		Assert.IsTrue(CharacterInstanceMetadata.TryGetPossessedBodyMetadata(options.EffectData, out var metadata));
		Assert.AreEqual(14, metadata.AnchorCharacterId);
		Assert.AreEqual(141, metadata.AnchorInstanceId);
		Assert.AreEqual(252, metadata.ShellBodyId);
		Assert.AreEqual(515, metadata.SourceTargetCharacterId);
		Assert.AreEqual(616, metadata.SourceTargetInstanceId);
		Assert.AreEqual(717, metadata.SourceSpellId);
		Assert.AreEqual("possessed-body:515", metadata.FormKey);
		Assert.AreEqual(CharacterInstancePersistencePolicy.DespawnOnReboot, metadata.PersistencePolicy);
	}

	[TestMethod]
	public void PossessedCorpseMetadata_RoundTripsCorpseProvenance()
	{
		var data = CharacterInstanceMetadata.CreatePossessedCorpseEffectData(
			14,
			141,
			515,
			616,
			717,
			818,
			CharacterInstancePersistencePolicy.TemporaryEffectBound);

		Assert.IsTrue(CharacterInstanceMetadata.TryGetPossessedCorpseMetadata(data, out var metadata));
		Assert.AreEqual(14, metadata.AnchorCharacterId);
		Assert.AreEqual(141, metadata.AnchorInstanceId);
		Assert.AreEqual(515, metadata.CorpseItemId);
		Assert.AreEqual(616, metadata.OriginalCharacterId);
		Assert.AreEqual(717, metadata.OriginalBodyId);
		Assert.AreEqual(818, metadata.SourceSpellId);
		Assert.AreEqual(CharacterInstancePersistencePolicy.TemporaryEffectBound, metadata.PersistencePolicy);
	}

	[TestMethod]
	public void AnimatedCorpseMetadata_RoundTripsCorpseProvenanceAndAIs()
	{
		var data = CharacterInstanceMetadata.CreateAnimatedCorpseEffectData(
			14,
			141,
			515,
			616,
			717,
			818,
			new long[] { 1, 2, 1 },
			CharacterInstancePersistencePolicy.TemporaryEffectBound);

		Assert.IsTrue(CharacterInstanceMetadata.TryGetAnimatedCorpseMetadata(data, out var metadata));
		Assert.AreEqual(14, metadata.AnchorCharacterId);
		Assert.AreEqual(141, metadata.AnchorInstanceId);
		Assert.AreEqual(515, metadata.CorpseItemId);
		Assert.AreEqual(616, metadata.OriginalCharacterId);
		Assert.AreEqual(717, metadata.OriginalBodyId);
		Assert.AreEqual(818, metadata.SourceSpellId);
		CollectionAssert.AreEqual(new long[] { 1, 2 }, metadata.ArtificialIntelligenceIds.ToArray());
		Assert.AreEqual(CharacterInstancePersistencePolicy.TemporaryEffectBound, metadata.PersistencePolicy);
	}

	[TestMethod]
	public void CreateScriptedAiSpawnOptions_SetsAiPoliciesAndMetadata()
	{
		var pc = BuildPlayerCharacter();
		pc.SetupGet(x => x.Id).Returns(13);
		pc.SetupGet(x => x.InstanceId).Returns(131);
		var form = BuildForm(242, "evil twin");
		var location = new Mock<ICell>();
		var aiOne = BuildArtificialIntelligence(1);
		var aiTwo = BuildArtificialIntelligence(2);

		var options = CharacterInstanceService.CreateScriptedAiSpawnOptions(pc.Object, form.Object, location.Object,
			RoomLayer.GroundLevel, new[] { aiOne.Object, aiTwo.Object, aiOne.Object }, true);

		Assert.AreEqual(CharacterInstanceKind.ScriptedAi, options.InstanceKind);
		Assert.AreEqual(CharacterInstanceControlPolicy.ScriptOnly, options.ControlPolicy);
		Assert.AreEqual(CharacterInstanceDeathPolicy.DestroyInstanceOnly, options.DeathPolicy);
		Assert.AreEqual(CharacterInstancePerceptionPolicy.OrdinaryEmbodied, options.PerceptionPolicy);
		Assert.AreEqual(CharacterInstancePersistencePolicy.DespawnOnReboot, options.PersistencePolicy);
		Assert.AreEqual("evil twin", options.InstanceName);
		Assert.IsTrue(options.CloneInventoryFromPrimary);
		CollectionAssert.AreEquivalent(new[] { aiOne.Object, aiTwo.Object }, options.ArtificialIntelligences.ToList());
		Assert.IsTrue(CharacterInstanceMetadata.TryGetScriptedAiMetadata(options.EffectData, out var metadata));
		Assert.AreEqual(13, metadata.AnchorCharacterId);
		Assert.AreEqual(131, metadata.AnchorInstanceId);
		Assert.AreEqual(242, metadata.BodyId);
		Assert.AreEqual("evil twin", metadata.FormKey);
		Assert.IsTrue(metadata.CloneInventory);
		CollectionAssert.AreEqual(new long[] { 1, 2 }, metadata.ArtificialIntelligenceIds.ToArray());
	}

	[TestMethod]
	public void CanStaffPossessNpcTarget_RejectsNonPlayerSecondaryForPlayerIdentity()
	{
		var primary = new Mock<ICharacterInstance>();
		primary.SetupGet(x => x.IsPlayerCharacter).Returns(true);
		var identity = new Mock<ICharacterIdentity>();
		identity.SetupGet(x => x.PrimaryInstance).Returns(primary.Object);
		var scriptedAiSecondary = new Mock<ICharacter>();
		scriptedAiSecondary.SetupGet(x => x.Identity).Returns(identity.Object);
		scriptedAiSecondary.SetupGet(x => x.IsPlayerCharacter).Returns(false);

		var result = CharacterInstanceService.CanStaffPossessNpcTarget(scriptedAiSecondary.Object);

		Assert.IsFalse(result);
	}

	[TestMethod]
	public void CanStaffPossessNpcTarget_AllowsNpcIdentity()
	{
		var npc = BuildNpc();
		var primary = new Mock<ICharacterInstance>();
		primary.SetupGet(x => x.IsPlayerCharacter).Returns(false);
		var identity = new Mock<ICharacterIdentity>();
		identity.SetupGet(x => x.PrimaryInstance).Returns(primary.Object);
		npc.SetupGet(x => x.Identity).Returns(identity.Object);

		var result = CharacterInstanceService.CanStaffPossessNpcTarget(npc.Object);

		Assert.IsTrue(result);
	}

	[TestMethod]
	public void ValidateSecondarySpawnOptions_PlayerFocusable_RejectsNpcs()
	{
		var npc = BuildNpc();
		var form = BuildForm();
		var location = new Mock<ICell>();
		var options = new SecondaryCharacterInstanceSpawnOptions
		{
			Owner = npc.Object,
			Form = form.Object,
			SpawnLocation = new SpatialLocation(location.Object, RoomLayer.GroundLevel, null),
			ControlPolicy = CharacterInstanceControlPolicy.PlayerFocusable
		};

		var result = CharacterInstanceService.ValidateSecondarySpawnOptions(options);

		Assert.IsFalse(result.Success);
		StringAssert.Contains(result.Message, "player characters");
	}

	[TestMethod]
	public void ValidateSecondarySpawnMode_PlayerFocusable_RejectsNpcs()
	{
		var npc = BuildNpc();

		var result = CharacterInstanceService.ValidateSecondarySpawnMode(npc.Object,
			SecondaryCharacterInstanceSpawnMode.PlayerFocusable);

		Assert.IsFalse(result.Success);
		StringAssert.Contains(result.Message, "player characters");
	}

	[TestMethod]
	public void ValidateSecondarySpawnMode_NpcAiControlled_AllowsNpcs()
	{
		var npc = BuildNpc();

		var result = CharacterInstanceService.ValidateSecondarySpawnMode(npc.Object,
			SecondaryCharacterInstanceSpawnMode.NpcAiControlled);

		Assert.IsTrue(result.Success);
	}

	[TestMethod]
	public void ValidateSecondarySpawnMode_NpcAiControlled_RejectsPlayerCharacters()
	{
		var pc = BuildPlayerCharacter();

		var result = CharacterInstanceService.ValidateSecondarySpawnMode(pc.Object,
			SecondaryCharacterInstanceSpawnMode.NpcAiControlled);

		Assert.IsFalse(result.Success);
		StringAssert.Contains(result.Message, "NPC identities");
	}

	[TestMethod]
	public void ValidateSecondarySpawnMode_ScriptAiControlled_AllowsPlayerCharacters()
	{
		var pc = BuildPlayerCharacter();

		var result = CharacterInstanceService.ValidateSecondarySpawnMode(pc.Object,
			SecondaryCharacterInstanceSpawnMode.ScriptAiControlled);

		Assert.IsTrue(result.Success);
	}

	[TestMethod]
	public void ValidateSecondarySpawnOptions_ScriptAiControlled_AllowsPlayerCharacters()
	{
		var pc = BuildPlayerCharacter();
		var form = BuildForm();
		var location = new Mock<ICell>();
		var options = CharacterInstanceService.CreateSpawnOptionsForMode(pc.Object, form.Object, location.Object,
			RoomLayer.GroundLevel, CharacterInstancePersistencePolicy.DespawnOnReboot,
			SecondaryCharacterInstanceSpawnMode.ScriptAiControlled);

		var result = CharacterInstanceService.ValidateSecondarySpawnOptions(options);

		Assert.IsTrue(result.Success);
		Assert.AreEqual(CharacterInstanceKind.ScriptedAi, options.InstanceKind);
		Assert.AreEqual(CharacterInstanceControlPolicy.ScriptOnly, options.ControlPolicy);
	}

	[TestMethod]
	public void ValidateSecondarySpawnOptions_AnimatedCorpseScriptOnly_AllowsPlayerCharacters()
	{
		var pc = BuildPlayerCharacter();
		var form = BuildForm();
		var location = new Mock<ICell>();
		var options = new SecondaryCharacterInstanceSpawnOptions
		{
			Owner = pc.Object,
			Form = form.Object,
			SpawnLocation = new SpatialLocation(location.Object, RoomLayer.GroundLevel, null),
			InstanceKind = CharacterInstanceKind.AnimatedCorpse,
			ControlPolicy = CharacterInstanceControlPolicy.ScriptOnly
		};

		var result = CharacterInstanceService.ValidateSecondarySpawnOptions(options);

		Assert.IsTrue(result.Success);
	}

	private static Mock<ICharacter> BuildPlayerCharacter()
	{
		var identity = new Mock<ICharacterIdentity>();
		var pc = new Mock<ICharacter>();
		pc.SetupGet(x => x.Identity).Returns(identity.Object);
		pc.SetupGet(x => x.IsPlayerCharacter).Returns(true);
		pc.SetupGet(x => x.IsGuest).Returns(false);
		return pc;
	}

	private static Mock<ICharacterForm> BuildForm(long bodyId = 100, string alias = "astral form")
	{
		var body = new Mock<IBody>();
		body.SetupGet(x => x.Id).Returns(bodyId);
		var form = new Mock<ICharacterForm>();
		form.SetupGet(x => x.Body).Returns(body.Object);
		form.SetupGet(x => x.Alias).Returns(alias);
		return form;
	}

	private static Mock<INPC> BuildNpc()
	{
		var identity = new Mock<ICharacterIdentity>();
		var npc = new Mock<INPC>();
		npc.SetupGet(x => x.Identity).Returns(identity.Object);
		npc.SetupGet(x => x.IsPlayerCharacter).Returns(false);
		npc.SetupGet(x => x.IsGuest).Returns(false);
		return npc;
	}

	private static Mock<IArtificialIntelligence> BuildArtificialIntelligence(long id)
	{
		var ai = new Mock<IArtificialIntelligence>();
		ai.SetupGet(x => x.Id).Returns(id);
		ai.SetupGet(x => x.Name).Returns($"AI {id}");
		ai.SetupGet(x => x.IsReadyToBeUsed).Returns(true);
		return ai;
	}

	private static (Mock<ICell> Cell, Mock<IRouteCellDefinition> Definition) BuildRouteCell(
		long id,
		double lengthMetres,
		double defaultPositionMetres)
	{
		var cell = new Mock<ICell>();
		var definition = new Mock<IRouteCellDefinition>();
		cell.SetupGet(x => x.Id).Returns(id);
		cell.SetupGet(x => x.RouteDefinition).Returns(definition.Object);
		definition.SetupGet(x => x.Cell).Returns(cell.Object);
		definition.SetupGet(x => x.LengthMetres).Returns(lengthMetres);
		definition.SetupGet(x => x.DefaultPositionMetres).Returns(defaultPositionMetres);
		return (cell, definition);
	}
}
