#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Xml.Linq;
using DatabaseSeeder.Seeders;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Database;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.FutureProg;
using MudSharp.Health;
using MudSharp.Models;

namespace MudSharp_Unit_Tests;

[TestClass]
[TestCategory("Integration")]
public partial class ArmageddonStockSupportInstallerTests
{
	[TestMethod]
	[DataRow(ArmageddonProgressionMode.Simple, false)]
	[DataRow(ArmageddonProgressionMode.Classic, true)]
	public void Supports_ExactNativeGraphsStableRerunAndUnchangedLockedInventory(ArmageddonProgressionMode mode, bool classes)
	{
		using var db = Fixture(mode, classes); var factory = ArmageddonInstallationSettingsTests.Factory(db);
		var beforeItems = db.GameItemProtos.Count(); var beforeAttacks = db.WeaponAttacks.Count();
		var first = ArmageddonStockSupportInstaller.Install(factory);
		Assert.AreEqual(ArmageddonInstallStatus.Completed, first.Status, string.Join("\n", first.Messages)); db.ChangeTracker.Clear();
		Assert.AreEqual(68, first.Identities.Count); Assert.AreEqual(beforeItems + 19, db.GameItemProtos.Count());
		Assert.AreEqual(beforeAttacks + 16, db.WeaponAttacks.Count()); Assert.AreEqual(1, db.Planes.Count(x => x.IsDefault));
		var itemIds = first.Identities.Values.ToArray();
		foreach (var item in db.GameItemProtos.Where(x => itemIds.Contains(x.Id)).AsEnumerable())
			Assert.IsTrue(!item.Name.Any(char.IsWhiteSpace) && !string.IsNullOrEmpty(item.UniqueName), item.ShortDescription);
		var passage = db.AutobuilderRoomTemplates.Single(x => x.Id == first.Identities[ArmageddonStockSupportInstaller.NilazPassageTemplateKey]);
		Assert.AreEqual("room by terrain", passage.TemplateType);
		var passageXml = XElement.Parse(passage.Definition);
		Assert.AreEqual("A Grey, Bleak Hallway", passageXml.Element("Default")?.Element("Terrain")?.Element("RoomName")?.Value);
		Assert.AreEqual(0, db.GameItems.Count()); Assert.AreEqual(0, db.Characters.Count()); Assert.AreEqual(0, db.MagicSpells.Count());
		Assert.AreEqual(210, new ArmageddonStockCatalogue().Blueprints.Count);
		Assert.AreEqual(49, new ArmageddonStockCatalogue().Blueprints.Count(x => x.Kind == ArmageddonBlueprintKind.MeleeItem));
		foreach (var sphere in new[] { "alteration", "teleportation" })
		{
			var key = ArmageddonStockSupportInstaller.TravelRankFiveKey(sphere);
			var tag = db.Tags.Single(x => x.Id == first.Identities[key + ".tag"]);
			Assert.AreEqual(db.SeederManagedRecords.Single(x => x.StableKey == "arm.material." + sphere + ".rank4.tag").LogicalId, tag.ParentId);
			var recipe = db.Crafts.Include(x => x.CraftProducts).Single(x => x.Id == first.Identities[key + ".recipe"]);
			Assert.AreEqual(first.Identities[key], (long)XElement.Parse(recipe.CraftProducts.Single().Definition).Element("ProductProducedId")!);
		}
		var corpse = db.FutureProgs.Include(x => x.FutureProgsParameters)
			.Single(x => x.Id == first.Identities[ArmageddonStockSupportInstaller.CorpseEligibilityKey]);
		CollectionAssert.AreEqual(new[] { ProgVariableTypes.Character, ProgVariableTypes.Item }, corpse.FutureProgsParameters
			.OrderBy(x => x.ParameterIndex).Select(x => ProgVariableTypes.FromStorageString(x.ParameterTypeDefinition)).ToArray());
		for (var variant = 1; variant <= 8; variant++)
		{
			var flame = ArmageddonStockSupportInstaller.FlameStaffKey(variant);
			Assert.AreEqual(db.SeederManagedRecords.Single(x => x.StableKey == flame).LogicalId, first.Identities[flame]);
			foreach (var key in new[] { ArmageddonStockSupportInstaller.ShadowStaffKey(variant), ArmageddonStockSupportInstaller.SparkStaffKey(variant) })
			{
				var item = db.GameItemProtos.Include(x => x.GameItemProtosGameItemComponentProtos).Single(x => x.Id == first.Identities[key]);
				Assert.AreEqual(800.0, item.Weight); Assert.IsTrue(item.ShortDescription.EndsWith("staff"));
				Assert.AreEqual(2, item.GameItemProtosGameItemComponentProtos.Count);
				var attack = db.WeaponAttacks.Single(x => x.Id == db.SeederManagedRecords.Single(r => r.StableKey == key + ".attack").LogicalId);
				Assert.AreEqual(5.0, attack.StaminaCost); Assert.AreEqual((int)(key.Contains("shadow") ? DamageType.Crushing : DamageType.Electrical), attack.DamageType);
				Assert.AreEqual("19", db.TraitExpressions.Single(x => x.Id == attack.DamageExpressionId).Expression);
				Assert.AreEqual("14", db.TraitExpressions.Single(x => x.Id == attack.PainExpressionId).Expression);
				Assert.AreEqual("0", db.TraitExpressions.Single(x => x.Id == attack.StunExpressionId).Expression);
				var weapon = db.WeaponTypes.Single(x => x.Id == attack.WeaponTypeId); var club = db.WeaponTypes.Single(x => x.Name == "Club");
				Assert.AreEqual(club.AttackTraitId, weapon.AttackTraitId); Assert.AreEqual(club.ParryTraitId, weapon.ParryTraitId);
			}
		}
		var claims = db.SeederManagedRecords.Where(x => x.Module == "stock-catalogue").ToArray();
		var snapshot = claims.ToDictionary(x => x.StableKey, x => (x.LogicalId, x.RevisionNumber, x.SeedBaseline, x.AppliedAt));
		var second = ArmageddonStockSupportInstaller.Install(factory); Assert.AreEqual(ArmageddonInstallStatus.Completed, second.Status, string.Join("\n", second.Messages));
		db.ChangeTracker.Clear(); foreach (var row in db.SeederManagedRecords.Where(x => x.Module == "stock-catalogue"))
			Assert.AreEqual(snapshot[row.StableKey], (row.LogicalId, row.RevisionNumber, row.SeedBaseline, row.AppliedAt));
		ArmageddonInstallation.Prepare(factory, mode);
	}

	[TestMethod]
	public void Supports_BuilderEditedPlaneProgStaffAndAttackAreRetainedByExactIdentity()
	{
		using var db = Fixture(); var factory = ArmageddonInstallationSettingsTests.Factory(db); var first = ArmageddonStockSupportInstaller.Install(factory);
		Assert.AreEqual(ArmageddonInstallStatus.Completed, first.Status, string.Join("\n", first.Messages)); db.ChangeTracker.Clear();
		var plane = db.Planes.Single(x => x.Id == first.Identities[ArmageddonStockSupportInstaller.ShadowPlaneKey]); plane.Name = "builder's shadow";
		var prog = db.FutureProgs.Single(x => x.Id == first.Identities[ArmageddonStockSupportInstaller.FlameMultiplierKey]); prog.FunctionText = "return 0.25";
		var item = db.GameItemProtos.Single(x => x.Id == first.Identities[ArmageddonStockSupportInstaller.ShadowStaffKey(1)]); item.ShortDescription = "a custom staff";
		item.Name = "cane"; item.UniqueName = "Builder's shadow staff";
		var legacy = db.GameItemProtos.Single(x => x.Id == first.Identities[ArmageddonStockSupportInstaller.SparkStaffKey(1)]);
		ArmageddonItemNamingTests.MakeLegacy(db, legacy);
		var attackId = db.SeederManagedRecords.Single(x => x.StableKey == ArmageddonStockSupportInstaller.ShadowStaffKey(1) + ".attack").LogicalId;
		db.WeaponAttacks.Single(x => x.Id == attackId).StaminaCost = 9; db.SaveChanges();
		var rerun = ArmageddonStockSupportInstaller.Install(factory); Assert.AreEqual(ArmageddonInstallStatus.Completed, rerun.Status, string.Join("\n", rerun.Messages));
		db.ChangeTracker.Clear(); Assert.AreEqual("builder's shadow", db.Planes.Single(x => x.Id == plane.Id).Name);
		Assert.AreEqual("return 0.25", db.FutureProgs.Single(x => x.Id == prog.Id).FunctionText);
		Assert.AreEqual("a custom staff", db.GameItemProtos.Single(x => x.Id == item.Id).ShortDescription); Assert.AreEqual(9.0, db.WeaponAttacks.Single(x => x.Id == attackId).StaminaCost);
		Assert.AreEqual("cane", db.GameItemProtos.Single(x => x.Id == item.Id).Name);
		Assert.AreEqual("Builder's shadow staff", db.GameItemProtos.Single(x => x.Id == item.Id).UniqueName);
		Assert.AreEqual("staff", db.GameItemProtos.Single(x => x.Id == legacy.Id).Name);
		Assert.AreEqual("Armageddon Spark Staff 1", db.GameItemProtos.Single(x => x.Id == legacy.Id).UniqueName);
	}

	[TestMethod]
	[DataRow("affinity")][DataRow("staff_revision")][DataRow("defiler_claim")]
	public void Supports_MissingOrIncompatibleExactDependenciesBlockBeforeNewRows(string failure)
	{
		using var db = Fixture();
		if (failure == "affinity") db.SeederManagedRecords.Remove(db.SeederManagedRecords.Single(x => x.StableKey == ArmageddonStockSupportInstaller.VoidAffinityMeritKey));
		if (failure == "defiler_claim") db.SeederManagedRecords.Single(x => x.StableKey == ArmageddonStockSupportInstaller.DefilerMeritKey).Module = "unrecognized";
		if (failure == "staff_revision")
		{
			var id = db.SeederManagedRecords.Single(x => x.StableKey == ArmageddonStockSupportInstaller.FlameStaffKey(1)).LogicalId;
			db.GameItemProtos.Include(x => x.EditableItem).Single(x => x.Id == id).EditableItem.RevisionStatus = (int)RevisionStatus.UnderDesign;
		}
		db.SaveChanges(); var before = db.SeederManagedRecords.Count();
		var result = ArmageddonStockSupportInstaller.Install(ArmageddonInstallationSettingsTests.Factory(db));
		Assert.AreEqual(ArmageddonInstallStatus.Blocked, result.Status, string.Join("\n", result.Messages)); Assert.AreEqual(0, result.Identities.Count);
		Assert.AreEqual(before, db.SeederManagedRecords.Count()); Assert.AreEqual(1, db.Planes.Count());
	}

	[TestMethod]
	public void Supports_AllDeclaredOwnedProgBodiesCompileWithExactNativeSignatures()
	{
		var definitions = ArmageddonStockSupportInstaller.ProgDefinitions(new(11, 12, 21, 22, 31, 47, 32, 23, 33, new[] { 41L, 42, 43, 44, 45, 46, 47 }));
		Assert.AreEqual(12, definitions.Count); var id = 100L; foreach (var row in definitions.Values) row.Id = id++;
		CollectionAssert.AreEqual(new[] { ProgVariableTypes.Character, ProgVariableTypes.Item },
			definitions[ArmageddonStockSupportInstaller.CorpseEligibilityKey].FutureProgsParameters
				.OrderBy(x => x.ParameterIndex).Select(x => ProgVariableTypes.FromStorageString(x.ParameterTypeDefinition)).ToArray());
		CollectionAssert.AreEqual(new[] { ProgVariableTypes.Character, ProgVariableTypes.Item },
			definitions[ArmageddonStockSupportInstaller.DeadSpeakLinkKey].FutureProgsParameters
				.OrderBy(x => x.ParameterIndex).Select(x => ProgVariableTypes.FromStorageString(x.ParameterTypeDefinition)).ToArray());
		using var compiler = new OfflineProgCompilation(definitions.Values);
		foreach (var definition in definitions.Values)
		{
			var prog = compiler.Compile(definition.Id); Assert.AreEqual(ProgVariableTypes.FromStorageString(definition.ReturnTypeDefinition), prog.ReturnType);
			Assert.IsTrue(prog.MatchesParameters(definition.FutureProgsParameters.OrderBy(x => x.ParameterIndex).Select(x => ProgVariableTypes.FromStorageString(x.ParameterTypeDefinition)).ToArray()));
		}
		StringAssert.Contains(definitions[ArmageddonStockSupportInstaller.MagicalEligibilityKey].FunctionText, "hasmagictag(@target, \"armageddon.magical_creature\")");
		Assert.IsFalse(definitions[ArmageddonStockSupportInstaller.TargetEligibilityKey].FunctionText.Contains("species"));
		Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => ArmageddonStockSupportInstaller.ProgDefinitions(new(0, 12, 21, 22, 31, 47, 32, 23, 33, new[] { 41L, 42, 43, 44, 45, 46, 47 })));
	}

	[TestMethod]
	public void Supports_AfterCommitConfirmationFailureRetainsIdentitiesAndFreshRerunResumes()
	{
		using var db = Fixture(); var factory = ArmageddonInstallationSettingsTests.Factory(db);
		var uncertain = ArmageddonStockSupportInstaller.Install(factory, point =>
		{ if (point == ArmageddonInstallCheckpoint.AfterCommit) throw new InvalidOperationException("confirmation fixture"); });
		Assert.AreEqual(ArmageddonInstallStatus.CommittedConfirmationFailed, uncertain.Status); Assert.AreEqual(68, uncertain.Identities.Count);
		var rerun = ArmageddonStockSupportInstaller.Install(factory); Assert.AreEqual(ArmageddonInstallStatus.Completed, rerun.Status, string.Join("\n", rerun.Messages));
		foreach (var entry in uncertain.Identities) Assert.AreEqual(entry.Value, rerun.Identities[entry.Key]);
		db.ChangeTracker.Clear(); Assert.AreEqual(132, db.GameItemProtos.Count()); Assert.AreEqual(0, db.GameItems.Count());
	}

	private static FuturemudDatabaseContext Fixture(ArmageddonProgressionMode mode = ArmageddonProgressionMode.Simple, bool classes = false)
		=> FixtureWithCombat(mode, classes, null);

	private static FuturemudDatabaseContext FixtureWithCombat(ArmageddonProgressionMode mode, bool classes,
		Action<FuturemudDatabaseContext>? configureCombat)
	{
		var db = ArmageddonCatalogueWeaponTests.Fixture(mode, classes); var factory = ArmageddonInstallationSettingsTests.Factory(db);
		configureCombat?.Invoke(db);
		var weapons = ArmageddonCatalogueWeapons.Install(factory); Assert.AreEqual(ArmageddonInstallStatus.Completed, weapons.Status, string.Join("\n", weapons.Messages)); db.ChangeTracker.Clear();
		// Minimal genuine native S3 affinity dependency rows; avoids unrelated anatomy/creature graph generation.
		var settings = ArmageddonInstallation.Read(db)!; var owned = new ArmageddonCatalogueOwnership(db, settings.NativeBindings[ArmageddonNativeBinding.BuilderAccount], []);
		var catalogue = new ArmageddonStockCatalogue();
		foreach (var key in ArmageddonStockSupportInstaller.AffinityMeritKeys)
		{
			var raceKey = key[..^".affinity".Length]; var affinities = catalogue.ByKey[raceKey].Configuration.GetProperty("affinity").EnumerateArray().Select(x => x.GetString()!);
			owned.Put(key, new Merit { Name = "Fixture " + key, Type = "Elemental Affinity", MeritScope = 2, MeritType = 0,
				Definition = new XElement("Merit", new XElement("ChargenAvailableProg", settings.NativeBindings[ArmageddonNativeBinding.AlwaysFalseProg]),
					new XElement("ApplicabilityProg", settings.NativeBindings[ArmageddonNativeBinding.AlwaysTrueProg]), new XElement("ChargenBlurb", "Explicit fixture"),
					new XElement("DescriptionText", "@ have|has a declared affinity"), new XElement("ElementalAffinities", affinities.Select(x => new XElement("Affinity", x)))).ToString() }, []);
		}
		db.Planes.Add(new Plane { Id = 1, Name = "Material", Alias = "material", IsDefault = true, Description = "Default", RoomDescriptionAddendum = "", RoomNameFormat = "{0}", RemoteObservationTag = "" });
		db.SaveChanges(); db.ChangeTracker.Clear(); return db;
	}
	[TestMethod]
	public void DeadSpeakLink_UneditedLegacySignatureUpgradesByOwnedIdentityAndThenRerunsUnchanged()
	{
		using var db = Fixture(); var factory = ArmageddonInstallationSettingsTests.Factory(db);
		var first = ArmageddonStockSupportInstaller.Install(factory);
		Assert.AreEqual(ArmageddonInstallStatus.Completed, first.Status, string.Join("\n", first.Messages));
		db.ChangeTracker.Clear();
		var record = db.SeederManagedRecords.Single(x => x.StableKey == ArmageddonStockSupportInstaller.DeadSpeakLinkKey);
		var prog = db.FutureProgs.Include(x => x.FutureProgsParameters).Single(x => x.Id == record.LogicalId);
		var old = prog.FutureProgsParameters.Single(x => x.ParameterIndex == 1);
		old.ParameterName = "proxy"; old.ParameterTypeDefinition = ProgVariableTypes.Character.ToStorageString();
		var baseline = JsonSerializer.Deserialize<Dictionary<string, string>>(record.SeedBaseline!)!;
		baseline["Parameters"] = JsonSerializer.Serialize(prog.FutureProgsParameters.OrderBy(x => x.ParameterIndex)
			.Select(x => new { Index = x.ParameterIndex, Name = x.ParameterName, Type = x.ParameterTypeDefinition }).ToArray());
		record.SeedBaseline = JsonSerializer.Serialize(baseline); db.SaveChanges();
		var id = prog.Id;

		var upgrade = ArmageddonStockSupportInstaller.Install(factory);

		Assert.AreEqual(ArmageddonInstallStatus.Completed, upgrade.Status, string.Join("\n", upgrade.Messages));
		db.ChangeTracker.Clear();
		var upgraded = db.FutureProgs.Include(x => x.FutureProgsParameters).Single(x => x.Id == id);
		var corpse = upgraded.FutureProgsParameters.Single(x => x.ParameterIndex == 1);
		Assert.AreEqual("corpse", corpse.ParameterName);
		Assert.AreEqual(ProgVariableTypes.Item, ProgVariableTypes.FromStorageString(corpse.ParameterTypeDefinition));
		var after = db.SeederManagedRecords.Single(x => x.StableKey == record.StableKey);
		var snapshot = (after.LogicalId, after.SeedBaseline, after.AppliedAt);
		var rerun = ArmageddonStockSupportInstaller.Install(factory);
		Assert.AreEqual(ArmageddonInstallStatus.Completed, rerun.Status, string.Join("\n", rerun.Messages));
		db.ChangeTracker.Clear(); after = db.SeederManagedRecords.Single(x => x.StableKey == record.StableKey);
		Assert.AreEqual(snapshot, (after.LogicalId, after.SeedBaseline, after.AppliedAt));
	}

}
