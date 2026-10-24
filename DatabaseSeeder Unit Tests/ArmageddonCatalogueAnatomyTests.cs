#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using DatabaseSeeder.Seeders;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Body;
using MudSharp.Body.Position.PositionStates;
using MudSharp.Database;
using MudSharp.Models;

namespace MudSharp_Unit_Tests;

[TestClass]
public class ArmageddonCatalogueAnatomyTests
{
	[TestMethod]
	[DataRow(ArmageddonProgressionMode.Classic, false)]
	[DataRow(ArmageddonProgressionMode.Classic, true)]
	[DataRow(ArmageddonProgressionMode.Simple, false)]
	[DataRow(ArmageddonProgressionMode.Simple, true)]
	public void Anatomy_IndependentTypedGraphsAndStableRerun(ArmageddonProgressionMode mode, bool classes)
	{
		using var db = Fixture(mode, classes);
		var factory = ArmageddonInstallationSettingsTests.Factory(db);
		var result = ArmageddonCatalogueAnatomy.Install(factory);
		Assert.AreEqual(ArmageddonInstallStatus.Completed, result.Status, string.Join("\n", result.Messages));
		db.ChangeTracker.Clear();
		Assert.AreEqual(7, result.Identities.Count);
		foreach (var blueprint in new ArmageddonStockCatalogue().Blueprints.Where(x => x.Kind == ArmageddonBlueprintKind.RaceBundle))
		{
			var body = db.BodyProtos.Single(x => x.Id == result.Identities[blueprint.Key + ".body"]);
			Assert.IsNull(body.CountsAsId, "Shared anatomy must not be inherited after flattening.");
			var parts = db.BodypartProtos.Where(x => x.BodyId == body.Id).ToArray();
			var partIds = parts.Select(x => x.Id).ToArray();
			Assert.IsTrue(parts.Any(x => x.BodypartType == (int)BodypartTypeEnum.Heart));
			Assert.IsTrue(parts.Any(x => x.BodypartType == (int)BodypartTypeEnum.Lung));
			Assert.IsTrue(parts.Any(x => x.BodypartType == (int)BodypartTypeEnum.Brain));
			Assert.AreEqual(1, db.BodypartInternalInfos.Count(x => partIds.Contains(x.BodypartProtoId)));
			Assert.AreEqual(1, db.BoneOrganCoverages.Count(x => partIds.Contains(x.BoneId)));
			Assert.AreEqual(1, db.BodypartProtoAlignmentHits.Count(x => partIds.Contains(x.BodypartProtoId)));
			Assert.AreEqual(1, db.BodypartProtoOrientationHits.Count(x => partIds.Contains(x.BodypartProtoId)));
			foreach (var link in db.BodypartProtoBodypartProtoUpstream.Where(x => partIds.Contains(x.Child))) Assert.IsTrue(partIds.Contains(link.Parent));
			foreach (var link in db.BodypartInternalInfos.Where(x => partIds.Contains(x.BodypartProtoId))) Assert.IsTrue(partIds.Contains(link.InternalPartId));
			foreach (var link in db.BoneOrganCoverages.Where(x => partIds.Contains(x.BoneId))) Assert.IsTrue(partIds.Contains(link.OrganId));
			Assert.AreEqual(2, parts.Count(x => x.BodypartType == (int)BodypartTypeEnum.GrabbingWielding));
			var plan = blueprint.Configuration.GetProperty("body_plan").GetString()!;
			var legs = plan.Contains("quadruped", StringComparison.Ordinal) ? 4 : 2;
			Assert.AreEqual(legs, body.MinimumLegsToStand);
			Assert.AreEqual(legs, db.Limbs.Count(x => x.RootBodyId == body.Id && x.LimbType == (int)LimbType.Leg));
			Assert.AreEqual(legs, parts.Count(x => x.BodypartType == (int)BodypartTypeEnum.Standing));
			var wings = plan.StartsWith("winged ", StringComparison.Ordinal) ? 2 : 0;
			Assert.AreEqual(wings, parts.Count(x => x.BodypartType == (int)BodypartTypeEnum.Wing));
			Assert.AreEqual(wings, db.Limbs.Count(x => x.RootBodyId == body.Id && x.LimbType == (int)LimbType.Wing));
			Assert.AreEqual(wings / 2, db.MoveSpeeds.Count(x => x.BodyProtoId == body.Id && x.PositionId == PositionFlying.Instance.Id));
			Assert.IsTrue(db.MoveSpeeds.Any(x => x.BodyProtoId == body.Id && x.PositionId == PositionStanding.Instance.Id));
			Assert.AreEqual("run", db.MoveSpeeds.Single(x => x.BodyProtoId == body.Id && x.Alias == "run").FirstPersonVerb);
		}
		var before = db.SeederManagedRecords.ToDictionary(x => x.StableKey, x => (x.LogicalId, x.RevisionNumber, x.SeedBaseline, x.AppliedAt));
		Assert.AreEqual(ArmageddonInstallStatus.Completed, ArmageddonCatalogueAnatomy.Install(factory).Status);
		db.ChangeTracker.Clear();
		foreach (var record in db.SeederManagedRecords) Assert.AreEqual(before[record.StableKey], (record.LogicalId, record.RevisionNumber, record.SeedBaseline, record.AppliedAt), record.StableKey);
		ArmageddonInstallation.Prepare(factory, mode);
		Assert.AreEqual(0, db.Characters.Count()); Assert.AreEqual(0, db.GameItems.Count()); Assert.AreEqual(0, db.NpcTemplates.Count());
	}

	[TestMethod]
	public void Anatomy_NewSourceFlightIdentityCannotDuplicateRetainedNativeAlias()
	{
		using var db = Fixture(); var factory = ArmageddonInstallationSettingsTests.Factory(db);
		var first = ArmageddonCatalogueAnatomy.Install(factory); Assert.AreEqual(ArmageddonInstallStatus.Completed, first.Status);
		db.ChangeTracker.Clear();
		var speeds = db.MoveSpeeds.Where(x => x.BodyProtoId == first.Identities["arm.race.air_guardian.body"]).Select(x => x.Id).ToArray();
		db.MoveSpeeds.Add(new() { Id = db.MoveSpeeds.Max(x => x.Id) + 1, BodyProtoId = 1, Alias = "fly", PositionId = PositionFlying.Instance.Id,
			FirstPersonVerb = "fly", ThirdPersonVerb = "flies", PresentParticiple = "flying", Multiplier = 1, StaminaMultiplier = 1 });
		db.SaveChanges(); db.ChangeTracker.Clear();
		var result = ArmageddonCatalogueAnatomy.Install(factory);
		Assert.AreEqual(ArmageddonInstallStatus.Failed, result.Status); StringAssert.Contains(string.Join("\n", result.Messages), "movement alias collision");
		CollectionAssert.AreEquivalent(speeds, db.MoveSpeeds.Where(x => x.BodyProtoId == first.Identities["arm.race.air_guardian.body"]).Select(x => x.Id).ToArray());
	}

	[TestMethod]
	[DataRow(ArmageddonProgressionMode.Classic, false)]
	[DataRow(ArmageddonProgressionMode.Classic, true)]
	[DataRow(ArmageddonProgressionMode.Simple, false)]
	[DataRow(ArmageddonProgressionMode.Simple, true)]
	public void Anatomy_NativeBootAddsSourceFloat_RetainsOwnedIdentityAndBuilderFields(ArmageddonProgressionMode mode, bool customised)
	{
		using var db = Fixture(mode); var factory = ArmageddonInstallationSettingsTests.Factory(db);
		var first = ArmageddonCatalogueAnatomy.Install(factory);
		Assert.AreEqual(ArmageddonInstallStatus.Completed, first.Status);
		db.ChangeTracker.Clear();
		var before = db.SeederManagedRecords.ToDictionary(x => x.StableKey, x => x.LogicalId);
		var floating = db.MoveSpeeds.Where(x => first.Identities.Values.Contains(x.BodyProtoId) && x.PositionId == PositionFloatingInZeroGravity.Instance.Id).ToArray();
		Assert.AreEqual(7, floating.Length);
		if (customised) foreach (var speed in floating) { speed.Alias = "builder float"; speed.Multiplier = 7; }
		// This is the exact compatibility row BodyPrototype creates when it loads a
		// shared source whose first install did not yet have zero-gravity movement.
		var standing = db.MoveSpeeds.First(x => x.BodyProtoId == 1 && x.PositionId == PositionStanding.Instance.Id);
		db.MoveSpeeds.Add(new() { Id = db.MoveSpeeds.Max(x => x.Id) + 1, BodyProtoId = 1,
			Alias = "float", PositionId = PositionFloatingInZeroGravity.Instance.Id,
			FirstPersonVerb = "float", ThirdPersonVerb = "floats", PresentParticiple = "floating",
			Multiplier = standing.Multiplier, StaminaMultiplier = standing.StaminaMultiplier });
		db.SaveChanges(); db.ChangeTracker.Clear();
		var rerun = ArmageddonCatalogueAnatomy.Install(factory);
		Assert.AreEqual(ArmageddonInstallStatus.Completed, rerun.Status, string.Join("\n", rerun.Messages));
		db.ChangeTracker.Clear();
		Assert.AreEqual(before.Count, db.SeederManagedRecords.Count());
		foreach (var record in db.SeederManagedRecords) Assert.AreEqual(before[record.StableKey], record.LogicalId);
		var after = db.MoveSpeeds.Where(x => first.Identities.Values.Contains(x.BodyProtoId) && x.PositionId == PositionFloatingInZeroGravity.Instance.Id).ToArray();
		CollectionAssert.AreEquivalent(floating.Select(x => x.Id).ToArray(), after.Select(x => x.Id).ToArray());
		if (customised) Assert.IsTrue(after.All(x => x.Alias == "builder float" && x.Multiplier == 7));
	}

	[TestMethod]
	public void Anatomy_InheritedFlightSpeedIsOwnedOnceAndOnlyForWingedFamilies()
	{
		using var db = Fixture(); var factory = ArmageddonInstallationSettingsTests.Factory(db);
		db.BodyProtosPositions.Add(new() { BodyProtoId = 1, Position = (int)PositionFlying.Instance.Id });
		db.MoveSpeeds.Add(new() { Id = 4, BodyProtoId = 1, Alias = "fly", PositionId = PositionFlying.Instance.Id,
			FirstPersonVerb = "fly", ThirdPersonVerb = "flies", PresentParticiple = "flying", Multiplier = 1, StaminaMultiplier = 1 });
		db.SaveChanges(); db.ChangeTracker.Clear();
		var result = ArmageddonCatalogueAnatomy.Install(factory);
		Assert.AreEqual(ArmageddonInstallStatus.Completed, result.Status, string.Join("\n", result.Messages));
		foreach (var blueprint in new ArmageddonStockCatalogue().Blueprints.Where(x => x.Kind == ArmageddonBlueprintKind.RaceBundle))
		{
			var bodyId = result.Identities[blueprint.Key + ".body"];
			var winged = blueprint.Configuration.GetProperty("body_plan").GetString()!.StartsWith("winged ", StringComparison.Ordinal);
			Assert.AreEqual(winged ? 1 : 0, db.MoveSpeeds.Count(x => x.BodyProtoId == bodyId && x.Alias == "fly"));
			Assert.AreEqual(winged ? 1 : 0, db.BodyProtosPositions.Count(x => x.BodyProtoId == bodyId && x.Position == PositionFlying.Instance.Id));
		}
		Assert.AreEqual(ArmageddonInstallStatus.Completed, ArmageddonCatalogueAnatomy.Install(factory).Status);
	}

	[TestMethod]
	public void Anatomy_SharedInheritedLinksExcludeUnrelatedDerivedBodyParts()
	{
		using var db = Fixture(); var factory = ArmageddonInstallationSettingsTests.Factory(db);
		db.BodyProtos.Add(new() { Id = 3, Name = "Unrelated derived animal", CountsAsId = 1 });
		db.BodypartProtos.Add(new() { Id = 999, BodyId = 3, Name = "foreignlung", BodypartType = (int)BodypartTypeEnum.Lung, IsOrgan = 1 });
		db.LimbsBodypartProto.Add(new() { LimbId = 2, BodypartProtoId = 999 });
		db.LimbsSpinalParts.Add(new() { LimbId = 1, BodypartProtoId = 999 });
		db.BodypartInternalInfos.Add(new() { BodypartProtoId = 1, InternalPartId = 999 });
		db.BoneOrganCoverages.Add(new() { BoneId = 7, OrganId = 999 });
		db.SaveChanges(); db.ChangeTracker.Clear();
		var result = ArmageddonCatalogueAnatomy.Install(factory);
		Assert.AreEqual(ArmageddonInstallStatus.Completed, result.Status, string.Join("\n", result.Messages));
		var bodyIds = result.Identities.Values.ToArray();
		var limbIds = db.Limbs.Where(x => bodyIds.Contains(x.RootBodyId)).Select(x => x.Id).ToArray();
		Assert.IsFalse(db.LimbsBodypartProto.Any(x => limbIds.Contains(x.LimbId) && x.BodypartProtoId == 999));
		Assert.IsFalse(db.LimbsSpinalParts.Any(x => limbIds.Contains(x.LimbId) && x.BodypartProtoId == 999));
		Assert.AreEqual(1, db.BodypartProtos.Count(x => x.Name == "foreignlung"));
	}

	[TestMethod]
	public void Anatomy_DerivedStockRelationshipChangesUpdateInPlace()
	{
		using var db = Fixture(); var factory = ArmageddonInstallationSettingsTests.Factory(db);
		var first = ArmageddonCatalogueAnatomy.Install(factory); Assert.AreEqual(ArmageddonInstallStatus.Completed, first.Status);
		db.ChangeTracker.Clear();
		var parts = db.BodypartProtos.Where(x => x.BodyId == first.Identities["arm.race.air_guardian.body"]).ToDictionary(x => x.Name);
		var linkKey = (parts["torso"].Id, parts["heart"].Id);
		db.BodypartInternalInfos.Single(x => x.BodypartProtoId == 1 && x.InternalPartId == 6).HitChance = 0.75;
		db.BoneOrganCoverages.Single(x => x.BoneId == 7 && x.OrganId == 6).CoverageChance = 0.5;
		db.SaveChanges(); db.ChangeTracker.Clear();
		var rerun = ArmageddonCatalogueAnatomy.Install(factory);
		Assert.AreEqual(ArmageddonInstallStatus.Completed, rerun.Status, string.Join("\n", rerun.Messages));
		db.ChangeTracker.Clear();
		Assert.AreEqual(0.75, db.BodypartInternalInfos.Single(x => x.BodypartProtoId == linkKey.Item1 && x.InternalPartId == linkKey.Item2).HitChance);
		Assert.AreEqual(0.5, db.BoneOrganCoverages.Single(x => x.BoneId == parts["bone"].Id && x.OrganId == parts["heart"].Id).CoverageChance);
		CollectionAssert.AreEquivalent(first.Identities.Values.ToArray(), rerun.Identities.Values.ToArray());
	}

	[TestMethod]
	public void Anatomy_BuilderPartsLimbsSpeedAndRelationshipsPreserved()
	{
		using var db = Fixture(); var factory = ArmageddonInstallationSettingsTests.Factory(db);
		var first = ArmageddonCatalogueAnatomy.Install(factory); Assert.AreEqual(ArmageddonInstallStatus.Completed, first.Status);
		db.ChangeTracker.Clear();
		var bodyId = first.Identities["arm.race.air_guardian.body"];
		var body = db.BodyProtos.Single(x => x.Id == bodyId); body.Name = "Builder guardian anatomy";
		var part = db.BodypartProtos.Single(x => x.BodyId == bodyId && x.Name == "heart"); part.MaxLife = 999;
		var limb = db.Limbs.Single(x => x.RootBodyId == bodyId && x.Name == "right wing"); limb.LimbDamageThresholdMultiplier = 0.75;
		var speed = db.MoveSpeeds.Single(x => x.BodyProtoId == bodyId && x.Alias == "fly"); speed.Multiplier = 2;
		var link = db.BodypartInternalInfos.Single(x => x.InternalPartId == part.Id); link.HitChance = 0.5;
		db.BodyProtosPositions.Remove(db.BodyProtosPositions.Single(x => x.BodyProtoId == bodyId && x.Position == PositionFlying.Instance.Id));
		db.SaveChanges(); db.ChangeTracker.Clear();
		var rerun = ArmageddonCatalogueAnatomy.Install(factory);
		Assert.AreEqual(ArmageddonInstallStatus.Completed, rerun.Status, string.Join("\n", rerun.Messages));
		db.ChangeTracker.Clear();
		Assert.AreEqual("Builder guardian anatomy", db.BodyProtos.Single(x => x.Id == bodyId).Name);
		Assert.AreEqual(999, db.BodypartProtos.Single(x => x.Id == part.Id).MaxLife);
		Assert.AreEqual(0.75, db.Limbs.Single(x => x.Id == limb.Id).LimbDamageThresholdMultiplier);
		Assert.AreEqual(2.0, db.MoveSpeeds.Single(x => x.Id == speed.Id).Multiplier);
		Assert.AreEqual(0.5, db.BodypartInternalInfos.Single(x => x.InternalPartId == part.Id).HitChance);
		Assert.IsFalse(db.BodyProtosPositions.Any(x => x.BodyProtoId == bodyId && x.Position == PositionFlying.Instance.Id));
		Assert.IsTrue(rerun.Messages.Any(x => x.Contains("preserved builder edit")));
	}

	[TestMethod]
	[DataRow("missing")][DataRow("retired")][DataRow("competition")]
	public void Anatomy_InvalidOwnedPartFailsWithoutRecreation(string damage)
	{
		using var db = Fixture(); var factory = ArmageddonInstallationSettingsTests.Factory(db);
		var first = ArmageddonCatalogueAnatomy.Install(factory); Assert.AreEqual(ArmageddonInstallStatus.Completed, first.Status);
		db.ChangeTracker.Clear();
		var record = db.SeederManagedRecords.First(x => x.EntityType == nameof(BodypartProto));
		if (damage == "missing") db.BodypartProtos.Remove(db.BodypartProtos.Single(x => x.Id == record.LogicalId));
		if (damage == "retired") record.Retired = true;
		if (damage == "competition") db.SeederManagedRecords.Add(new() { Seeder = "Other", StableKey = "other.part", EntityType = record.EntityType, LogicalId = record.LogicalId });
		db.SaveChanges(); db.ChangeTracker.Clear();
		var before = db.BodypartProtos.Count();
		var rerun = ArmageddonCatalogueAnatomy.Install(factory);
		Assert.IsTrue(rerun.Status is ArmageddonInstallStatus.Blocked or ArmageddonInstallStatus.Failed);
		Assert.AreEqual(before, db.BodypartProtos.Count());
	}

	[TestMethod]
	public void Anatomy_UnownedNameCollisionDoesNotAdoptNativeBody()
	{
		using var db = Fixture(); var factory = ArmageddonInstallationSettingsTests.Factory(db);
		db.BodyProtos.Add(new() { Id = 99, Name = "Armageddon Earth Mount stock anatomy" }); db.SaveChanges();
		var result = ArmageddonCatalogueAnatomy.Install(factory);
		Assert.AreEqual(ArmageddonInstallStatus.Failed, result.Status);
		Assert.IsFalse(db.SeederManagedRecords.Any(x => x.EntityType == nameof(BodyProto)));
	}

	internal static FuturemudDatabaseContext Fixture(ArmageddonProgressionMode mode = ArmageddonProgressionMode.Simple, bool classes = false, bool enableStockForNewInstall = false)
	{
		var db = ArmageddonInstallationSettingsTests.Fixture(classes);
		db.BodyProtos.Single(x => x.Id == 1).Name = "Humanoid";
		db.BodyProtos.Add(new() { Id = 2, Name = "Organic Humanoid", CountsAsId = 1, MinimumLegsToStand = 2, MinimumWingsToFly = 2 });
		db.Races.Single(x => x.Id == 1).BaseBodyId = 2;
		var torso = db.BodypartProtos.Single(x => x.Id == 1); torso.Name = "torso"; torso.BodypartType = (int)BodypartTypeEnum.BonyDrapeable;
		var definitions = new[] { (2L,"rhand",BodypartTypeEnum.GrabbingWielding,1L), (3L,"lhand",BodypartTypeEnum.GrabbingWielding,1L),
			(4L,"rfoot",BodypartTypeEnum.Standing,1L), (5L,"lfoot",BodypartTypeEnum.Standing,1L), (6L,"heart",BodypartTypeEnum.Heart,2L),
			(7L,"bone",BodypartTypeEnum.Bone,2L), (8L,"spine",BodypartTypeEnum.Spine,2L), (9L,"lung",BodypartTypeEnum.Lung,2L), (10L,"brain",BodypartTypeEnum.Brain,2L) };
		foreach (var (id, name, type, body) in definitions)
		{
			db.BodypartProtos.Add(new() { Id = id, BodyId = body, Name = name, Description = name, BodypartType = (int)type,
				IsOrgan = id >= 6 ? 1 : 0, MaxLife = 100, Size = 6, DefaultMaterialId = 1 });
			db.BodypartProtoBodypartProtoUpstream.Add(new() { Child = id, Parent = 1 });
		}
		db.BodypartInternalInfos.Add(new() { BodypartProtoId = 1, InternalPartId = 6, HitChance = 1, IsPrimaryOrganLocation = true, ProximityGroup = "thorax" });
		db.BoneOrganCoverages.Add(new() { BoneId = 7, OrganId = 6, CoverageChance = 1 });
		db.BodypartProtoAlignmentHits.Add(new() { Id = 1, BodypartProtoId = 1, Alignment = 1, HitChance = 20 });
		db.BodypartProtoOrientationHits.Add(new() { Id = 1, BodypartProtoId = 1, Orientation = 1, HitChance = 20 });
		var limbDefinitions = new[] { (1L,"Torso",LimbType.Torso,1L), (2L,"Right Arm",LimbType.Arm,2L), (3L,"Left Arm",LimbType.Arm,3L), (4L,"Right Leg",LimbType.Leg,4L), (5L,"Left Leg",LimbType.Leg,5L) };
		foreach (var (id, name, type, root) in limbDefinitions)
		{
			db.Limbs.Add(new() { Id = id, Name = name, LimbType = (int)type, RootBodyId = 1, RootBodypartId = root });
			db.LimbsBodypartProto.Add(new() { LimbId = id, BodypartProtoId = root });
			db.LimbsSpinalParts.Add(new() { LimbId = id, BodypartProtoId = 8 });
		}
		db.BodyProtosPositions.Add(new() { BodyProtoId = 1, Position = (int)PositionStanding.Instance.Id });
		db.MoveSpeeds.AddRange(new MoveSpeed { Id = 1, BodyProtoId = 1, Alias = "walk", PositionId = PositionStanding.Instance.Id, FirstPersonVerb = "walk", Multiplier = 1, StaminaMultiplier = 1 },
			new MoveSpeed { Id = 2, BodyProtoId = 1, Alias = "run", PositionId = PositionStanding.Instance.Id, FirstPersonVerb = "oldrun", Multiplier = 0.5, StaminaMultiplier = 2 },
			new MoveSpeed { Id = 3, BodyProtoId = 2, Alias = "run", PositionId = PositionStanding.Instance.Id, FirstPersonVerb = "run", Multiplier = 0.25, StaminaMultiplier = 2 });
		db.SaveChanges();
		ArmageddonInstallation.Prepare(ArmageddonInstallationSettingsTests.Factory(db), mode, enableStockForNewInstall: enableStockForNewInstall);
		return db;
	}
}
