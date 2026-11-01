#nullable enable

using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using System.Text.Json;
using System.Xml.Linq;
using DatabaseSeeder;
using DatabaseSeeder.Seeders;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Models;

namespace MudSharp_Unit_Tests;

[TestClass]
public class ArmageddonWorldfileSeederTests
{
	[TestMethod]
	[DataRow("blade knife steel", "blade")]
	[DataRow("  The\tkey brass ", "key")]
	[DataRow("cloak", "cloak")]
	[DataRow("a an the", "item")]
	[DataRow("", "item")]
	public void ImportedItemNamesUseOnePrimaryKeyword(string keywords, string expected) =>
		Assert.AreEqual(expected, ArmageddonWorldfileWriter.SourcePrimaryNoun(keywords));

	[TestMethod]
	public void ResetFitting_RefitsOnlyWornClothingAndArmour_PreservingCarriedStock()
	{
		foreach (var type in new[] { 9, 11 })
		{
			Assert.AreEqual("wearsize=\"for self\"", ArmageddonWorldfileWriter.NpcResetFitArguments(MudSharp.GameItems.OutfitTemplateItemPlacement.Worn, type));
			Assert.AreEqual("", ArmageddonWorldfileWriter.NpcResetFitArguments(MudSharp.GameItems.OutfitTemplateItemPlacement.Inventory, type));
		}
		Assert.AreEqual("", ArmageddonWorldfileWriter.NpcResetFitArguments(MudSharp.GameItems.OutfitTemplateItemPlacement.Worn, 5));
	}

	[TestMethod]
	public void SafeRoomsEnableQuittingWithoutInventingCombatImmunity()
	{
		using var db = ArmageddonInstallationSettingsTests.Fixture();
		ArmageddonInstallation.Prepare(ArmageddonInstallationSettingsTests.Factory(db));
		db.Zones.Add(new Zone { Id = 1, Name = "Baseline", ShardId = 1 });
		db.Terrains.AddRange(new Terrain { Id = 1, Name = "Hallway" },
			new Terrain { Id = 2, Name = "Gas Giant Atmosphere" },
			new Terrain { Id = 3, Name = "Sandy Desert" });
		db.SaveChanges();
		var location = new ArmWorldSource("fixture.wld", 1);
		var source = new ArmageddonWorldfilePackage
		{
			Zones = [new ArmWorldZone(location, 0, 0, "Imported zone", [], [])],
			Rooms =
			[
				new ArmWorldRoom(location, 0, "Quit-safe camp", 10, 0, 1 << 4, 0,
					[new ArmWorldExit(0, 11, "archway", 0, -1, "A narrow archway leads north.")],
					[new ArmWorldExtra("wall", "A weathered wall.")], [], 0, [], "A camp."),
				new ArmWorldRoom(location, 0, "Ordinary room", 11, 0, 0, 0, [], [], [], 0, [], "An ordinary room.")
			]
		};
		var writer = new ArmageddonWorldfileWriter(db, source);
		typeof(ArmageddonWorldfileWriter).GetMethod("InstallRooms", BindingFlags.Instance | BindingFlags.NonPublic)!
			.Invoke(writer, null);
		db.ChangeTracker.Clear();
		var safe = db.RoomOverlays.Single(x => x.Name == "Vnum10");
		var ordinary = db.RoomOverlays.Single(x => x.Name == "Vnum11");
		Assert.IsTrue(safe.SafeQuit);
		Assert.IsFalse(ordinary.SafeQuit);
		var effects = XElement.Parse(db.Rooms.Single(x => x.UniqueName == "Vnum10").EffectData);
		CollectionAssert.AreEqual(new[] { "KeywordDescriptions" },
			effects.Elements("Effect").Select(x => (string)x.Element("Type")!).ToArray());
		var world = new Moq.Mock<MudSharp.Framework.IFuturemud>();
		world.SetupGet(x => x.FutureProgs).Returns(new MudSharp.Framework.All<MudSharp.FutureProg.IFutureProg>());
		var runtimeRoom = new Moq.Mock<MudSharp.Construction.IRoom>();
		runtimeRoom.SetupGet(x => x.Gameworld).Returns(world.Object);
		var descriptions = new MudSharp.Effects.Concrete.KeywordDescriptionsEffect(effects.Element("Effect")!, runtimeRoom.Object);
		Assert.AreEqual("A weathered wall.", MudSharp.GameItems.KeywordDescriptionResolver.Find("wall", false,
			[(true, new[] { descriptions })])!.Description);
		Assert.AreEqual("A narrow archway leads north.", MudSharp.GameItems.KeywordDescriptionResolver.Find("northward", false,
			[(true, new[] { descriptions })])!.Description);
		Assert.IsNull(MudSharp.GameItems.KeywordDescriptionResolver.Find("northward", false,
			[(false, new[] { descriptions })]));
		Assert.AreEqual("<Effects/>", db.Rooms.Single(x => x.UniqueName == "Vnum11").EffectData);
		Assert.IsFalse(db.Rooms.Any(x => x.EffectData.Contains("Peaceful")));
		Assert.AreEqual(633, ArmageddonWorldfileTestData.Load().Rooms.Count(x => (x.Flags & (1 << 4)) != 0));
	}

	[TestMethod]
	public void PackageRetainsQualifiedInventoryAndAllReferencedPhysicalGenerators()
	{
		var package = ArmageddonWorldfileTestData.Load();
		Assert.AreEqual(26137, package.Rooms.Length);
		Assert.AreEqual(7415, package.Items.Length);
		Assert.AreEqual(2460, package.Npcs.Length);
		Assert.AreEqual(27682, package.Resets.Length);
		Assert.AreEqual(173, package.Shops.Length);
		Assert.AreEqual(165, package.Shops.Select(x => x.Keeper).Distinct().Count());
		Assert.IsTrue(package.Rooms.Any(x => x.Vnum == 0));
		foreach (var race in package.Npcs.Select(x => x.Identity[6]).Distinct())
		{
			var dimensions = package.PhysicalGenerators.Single(x => x.Race == race);
			Assert.IsTrue(dimensions.Height.Mean > 0);
			Assert.IsTrue(dimensions.Weight.Mean > 0);
		}
		Assert.AreEqual(package.Fingerprint, package.Manifest.GetProperty("PackageSha256").GetString());
	}

	[TestMethod]
	[DataRow("G", 3)]
	[DataRow("E", 4)]
	[DataRow("O", 4)]
	[DataRow("P", 4)]
	public void ResetValuesUseOpcodeSpecificOffsetAndRetainAllSixValues(string opcode, int offset)
	{
		var arguments = Enumerable.Range(0, offset).Concat(new[] { -1, 22, 33, 44, 55, 66 }).Append(999).ToArray();
		var reset = new ArmWorldReset(new("cmds/z0.cmd", 1), 0, opcode, arguments, [], []);
		CollectionAssert.AreEqual(new[] { -1, 22, 33, 44, 55, 66 }, ArmageddonWorldfileWriter.ResetValues(reset));
	}

	[TestMethod]
	public void LightResetPreservesSourceTypeAndFuelFlagsButNotItsLitBit()
	{
		var light = Item(1, [7, 80, 3, 4, 5, 255]);
		var reset = new[] { 2, 0, 0, 0, 0, 0 };
		CollectionAssert.AreEqual(new[] { 2, 80, 3, 4, 5, 254 }, ArmageddonWorldfileWriter.EffectiveResetValues(light, reset));
		CollectionAssert.AreEqual(new[] { 2, 80, 3, 4, 5, 255 }, ArmageddonWorldfileWriter.EffectiveResetValues(light, reset, true));
		CollectionAssert.AreEqual(new[] { 2, 0, 0, 0, 0, 0 }, reset);
	}

	[TestMethod]
	public void ArmourResetPreservesOriginalMaximumOnlyWhenResetMaximumIsZero()
	{
		var armour = Item(9, [100, 90, 5, 200, 0, 0]);
		CollectionAssert.AreEqual(new[] { 40, 30, 3, 200, 4, 5 },
			ArmageddonWorldfileWriter.EffectiveResetValues(armour, [40, 30, 3, 0, 4, 5]));
		CollectionAssert.AreEqual(new[] { 40, 30, 3, 150, 4, 5 },
			ArmageddonWorldfileWriter.EffectiveResetValues(armour, [40, 30, 3, 150, 4, 5]));
	}

	[TestMethod]
	public void SourceWearableSizes_UseEffectiveWornAndArmourValuesAndPreserveZero()
	{
		var worn = Item(11, [0, 0, 0, 0, 0, 0]);
		var wornOverride = ArmageddonWorldfileWriter.EffectiveResetValues(worn, [15, 0, 0, 0, 0, 0]);
		var armour = Item(9, [100, 90, 5, 200, 0, 0]);
		var armourOverride = ArmageddonWorldfileWriter.EffectiveResetValues(armour, [40, 30, 0, 200, 4, 5]);

		Assert.AreEqual(0, ArmageddonWorldfileWriter.SourceWearableSize(11, worn.Values));
		Assert.AreEqual(15, ArmageddonWorldfileWriter.SourceWearableSize(11, wornOverride));
		Assert.AreEqual(0, ArmageddonWorldfileWriter.SourceWearableSize(9, armourOverride));
		Assert.IsNull(ArmageddonWorldfileWriter.SourceWearableSize(5, [1, 2, 3, 4, 5, 6]));
	}

	[TestMethod]
	public void SourceWearableFit_UsesFixedScalarAndNativeUnitBindings()
	{
		var sizing = ArmageddonWorldfileWriter.SourceWearableSizing(0, 123);
		Assert.IsTrue((bool)sizing.Attribute("enabled")!);
		Assert.IsFalse((bool)sizing.Attribute("firstwear")!);
		Assert.AreEqual(123L, (long)sizing.Attribute("scalarProg")!);
		Assert.AreEqual("Scalar", (string)sizing.Element("Fit")!.Attribute("kind")!);
		Assert.AreEqual(0, (int)sizing.Element("Fit")!.Attribute("scalar")!);

		// 70.866 inches plus 7 ten-kilogram units, divided by 7.4 and floored gives source size 10.
		Assert.AreEqual(10, ArmageddonWorldfileWriter.SourceWearableSizeFromNativeMeasurements(1.8, 70, 1, 1));
		Assert.AreEqual(10, ArmageddonWorldfileWriter.SourceWearableSizeFromNativeMeasurements(3.6, 350, 2, 5));
	}

	[TestMethod]
	public void ContractHasNoQuestionsAndNoUpdatePath()
	{
		IDatabaseSeeder seeder = new ArmageddonWorldfileSeeder();
		Assert.AreEqual(0, seeder.Questions.Count());
		Assert.AreEqual(SeederRepeatabilityMode.OneShot, seeder.Metadata.RepeatabilityMode);
		Assert.AreEqual(SeederUpdateCapability.None, seeder.Metadata.UpdateCapability);
		Assert.IsFalse(seeder.SafeToRunMoreThanOnce);
	}

	[TestMethod]
	public void AuthoredMobileEquipmentNamesRemainUniqueAcrossZoneFiles()
	{
		var mobiles = ArmageddonWorldfileTestData.Load().Resets.Where(x => x.Opcode == "M" && x.ReferenceIssues.Length == 0).ToArray();
		Assert.AreEqual(mobiles.Length, mobiles.Select(ArmageddonWorldfileWriter.ResetOutfitName).Distinct(StringComparer.OrdinalIgnoreCase).Count());
	}

	[TestMethod]
	public void DirectCallRefusesAnExistingMarkerBeforeLookingForFoundationsOrWritingData()
	{
		using var db = ArmageddonInstallationSettingsTests.Fixture();
		db.SeederManagedRecords.Add(new SeederManagedRecord
		{
			Seeder = "ArmageddonWorldfile", Module = "worldfile-provenance", StableKey = "installation",
			EntityType = "ArmageddonWorldfileInstallation", ManifestVersion = "fixture", AppliedAt = DateTime.UtcNow
		});
		db.SaveChanges();
		var before = db.SeederManagedRecords.Count();
		var exception = Assert.ThrowsExactly<InvalidOperationException>(() =>
			new ArmageddonWorldfileSeeder().SeedData(db, new System.Collections.Generic.Dictionary<string, string>()));
		StringAssert.Contains(exception.Message, "already installed");
		Assert.AreEqual(before, db.SeederManagedRecords.Count());
		Assert.AreEqual(0, db.Rooms.Count());
		Assert.IsFalse(db.ChangeTracker.HasChanges());
	}

	private static ArmWorldItem Item(int type, int[] values) => new(new("objects/z0.obj", 1), 1, -1,
		"fixture", "a fixture", "A fixture is here.", "A fixture.", [type, 0, 1, 0, 0], values, [100, 10, 0], [0, 0, 0, 0], [], [], []);

	[TestMethod]
	[DataRow(-1, 68, 34)]
	[DataRow(-1, 101, 50)]
	[DataRow(-1, -1, 0)]
	[DataRow(0, 68, 0)]
	[DataRow(7, 68, 7)]
	public void NpcAgeUsesSourceHalfMaximumOnlyForTheUnspecifiedSentinel(int authored, int maximum, int expected) =>
		Assert.AreEqual(expected, ArmageddonWorldfileWriter.SourceNpcAge(authored, maximum, 18));

	[TestMethod]
	public void ShopPricesRoundToWholeBitsAndRepeatingBuybackRatiosDoNotLoseACoin()
	{
		Assert.AreEqual(2m, ArmageddonWorldfileWriter.SourceShopPrice(10, 0.15));
		Assert.AreEqual(0m, ArmageddonWorldfileWriter.SourceShopPrice(1, 0.25));
		Assert.AreEqual(3m, ArmageddonWorldfileWriter.SourceShopPrice(10, 0.5, true));
		foreach (var price in new[] { 3m, 7m, 13m, 999999m })
		foreach (var payout in new[] { 1m, 2m, 5m })
		{
			var ratio = ArmageddonWorldfileWriter.SourceBuybackModifier(price, payout);
			Assert.AreEqual(payout, decimal.Truncate(price * ratio));
			Assert.AreEqual(payout * int.MaxValue, decimal.Truncate(price * ratio * int.MaxValue));
		}
	}

	[TestMethod]
	public void PrimaryLanguageUsesRacialInitialisationOnlyWithoutAnExistingSpokenSkill()
	{
		var original = ArmageddonWorldfileTestData.Load().Npcs[0];
		var elf = original with { Identity = [0, 0, 0, 0, 0, 0, 2], Flags = [0, 0, 0, 0], Skills = [] };
		Assert.AreEqual(2, ArmageddonWorldfileWriter.SourcePrimaryLanguage(elf));
		Assert.AreEqual(4, ArmageddonWorldfileWriter.SourcePrimaryLanguage(elf with { Flags = [0, 0, 0, 4], Skills = [[101, 100]] }));
		Assert.AreEqual(0, ArmageddonWorldfileWriter.SourcePrimaryLanguage(elf with { Identity = [0, 0, 0, 0, 0, 0, 32] }));
	}

	[TestMethod]
	[DataRow(10, 0, MudSharp.Combat.RangedWeaponType.Bow)]
	[DataRow(14311, 1, MudSharp.Combat.RangedWeaponType.Crossbow)]
	[DataRow(14400, 1, MudSharp.Combat.RangedWeaponType.Sling)]
	[DataRow(39033, 1, MudSharp.Combat.RangedWeaponType.Blowgun)]
	public void RangedFamiliesUseTheReviewedSourceLoadingAndCatalogueBindings(int vnum, int loading, MudSharp.Combat.RangedWeaponType expected) =>
		Assert.AreEqual(expected, ArmageddonWorldfileWriter.SourceRangedFamily(vnum, loading));

	[TestMethod]
	public void ProvenanceFitsTheNativeManifestColumnAndBodyLookupIncludesInheritedAnatomy()
	{
		using var db = ArmageddonInstallationSettingsTests.Fixture();
		ArmageddonInstallation.Prepare(ArmageddonInstallationSettingsTests.Factory(db));
		db.BodyProtos.Add(new BodyProto { Id = 2, CountsAsId = 1, Name = "Derived body" });
		db.SaveChanges();
		var writer = new ArmageddonWorldfileWriter(db, ArmageddonWorldfileTestData.Load());
		var record = (SeederManagedRecord)typeof(ArmageddonWorldfileWriter).GetMethod("Record", BindingFlags.Instance | BindingFlags.NonPublic)!
			.Invoke(writer, ["fixture", "Fixture", null, null, new { Fixture = true }])!;
		var limit = db.Model.FindEntityType(typeof(SeederManagedRecord))!.FindProperty(nameof(SeederManagedRecord.ManifestVersion))!.GetMaxLength();
		Assert.IsTrue(record.ManifestVersion.Length <= limit);
		var ancestry = (long[])typeof(ArmageddonWorldfileWriter).GetMethod("BodyAncestry", BindingFlags.Instance | BindingFlags.NonPublic)!.Invoke(writer, [2L])!;
		Assert.IsTrue(ancestry.ToHashSet().SetEquals([1L, 2L]));
	}

	[TestMethod]
	public void NpcRaceWithoutBaselineEthnicityStillGetsAPersistableNativePopulation()
	{
		using var db = ArmageddonInstallationSettingsTests.Fixture();
		ArmageddonInstallation.Prepare(ArmageddonInstallationSettingsTests.Factory(db));
		var writer = new ArmageddonWorldfileWriter(db, ArmageddonWorldfileTestData.Load());
		var race = new Race { Id = 222, Name = "Fixture (Worldfile NPC)" };
		var ethnicity = writer.CreateNpcEthnicity(race, null);
		Assert.IsTrue(ethnicity.Id > 0);
		Assert.AreEqual(race.Id, ethnicity.ParentRaceId);
		Assert.AreEqual(race.Name, ethnicity.Name);
		Assert.IsNotNull(ethnicity.AvailabilityProgId);
		Assert.IsNull(ethnicity.PopulationBloodModelId);
		Assert.AreEqual(0, ethnicity.EthnicitiesCharacteristics.Count);
		Assert.IsTrue(db.Ethnicities.Local.Contains(ethnicity));
	}

	[TestMethod]
	public void ExplicitResetEquipmentSlotsAddOnlyMissingWearLocationsWithoutChangingTheBaseItem()
	{
		var pike = ArmageddonWorldfileTestData.Load().Items.Single(x => x.Vnum == 20032);
		Assert.AreEqual(1 << 9, ArmageddonWorldfileWriter.ResetForcedWearMask(pike, 4));
		Assert.AreEqual(0, pike.Flags[2] & (1 << 9));
		var wristKey = ArmageddonWorldfileTestData.Load().Items.Single(x => x.Vnum == 4631);
		Assert.AreEqual(0, ArmageddonWorldfileWriter.ResetForcedWearMask(wristKey, 14));
		Assert.AreEqual((12, 0), ArmageddonWorldfileWriter.SourceEquipmentLocation(14));
		Assert.AreEqual(0, ArmageddonWorldfileWriter.ResetForcedWearMask(pike, 16));
	}

	[TestMethod]
	public void DoorPickingPreservesNoLockAndImpossibleCasesAndMapsTheSourceDifficultyBands()
	{
		var native = new[] { 0, 2, 4, 5, 6, 7, 9, 10 };
		for (var level = 0; level < 8; level++)
		{
			Assert.AreEqual(native[level], ArmageddonWorldfileWriter.SourceDoorPickDifficulty(0, (level << 12) | 7));
			Assert.AreEqual(10, ArmageddonWorldfileWriter.SourceDoorPickDifficulty(-1, level << 12));
		}
	}

	[TestMethod]
	public void ValidSyntheticCraftRetainsMultiplicityAndEligibilityInOneNativePhase()
	{
		using var db = ArmageddonInstallationSettingsTests.Fixture();
		ArmageddonInstallation.Prepare(ArmageddonInstallationSettingsTests.Factory(db));
		var skillId = (db.TraitDefinitions.Max(x => (long?)x.Id) ?? 0) + 1;
		db.TraitDefinitions.Add(new() { Id = skillId, Name = "Fixture Crafting", Type = 0 });
		db.SaveChanges();
		using var json = JsonDocument.Parse("""
			{"NAME":"fixture craft","DESC":"fixture blurb","FROM":"10 10","INTO_SUCCESS":"20 20","INTO_FAIL":"20","SKILL":"Fixture Crafting","PERC_MIN":"73","PERC_MOD":"10","DELAY_MIN":"2","DELAY_MAX":"4","TO_CHAR_SUCCESS":"source success","TO_ROOM_SUCCESS":"source room success","TO_CHAR_FAIL":"source fail","TO_ROOM_FAIL":"source room fail","ONLY_TRIBE":"0","REQUIRE_FIRE":"0","TOOL_TYPE":"none"}
			""");
		var writer = new ArmageddonWorldfileWriter(db, new ArmageddonWorldfilePackage());
		var items = (Dictionary<int, GameItemProto>)typeof(ArmageddonWorldfileWriter)
			.GetField("_items", BindingFlags.Instance | BindingFlags.NonPublic)!.GetValue(writer)!;
		items.Add(10, new() { Id = 100 });
		items.Add(20, new() { Id = 200 });
		typeof(ArmageddonWorldfileWriter).GetMethod("InstallCraft", BindingFlags.Instance | BindingFlags.NonPublic)!
			.Invoke(writer, [json.RootElement]);
		db.SaveChanges();
		var craft = db.Crafts.Local.Single();
		var phase = craft.CraftPhases.Single();
		Assert.AreEqual(1, phase.PhaseNumber);
		Assert.AreEqual(3.0, phase.PhaseLengthInSeconds);
		Assert.AreEqual("$0 craft|crafts $p1.", phase.Echo);
		Assert.AreEqual(skillId, craft.CheckTraitId);
		Assert.AreEqual(craft.CanUseProgId, craft.AppearInCraftsListProgId);
		StringAssert.Contains(db.FutureProgs.Local.Single(x => x.Id == craft.CanUseProgId).FunctionText, ">= 73");
		var input = XElement.Parse(craft.CraftInputs.Single().Definition);
		Assert.AreEqual(100L, (long)input.Element("TargetItemId")!);
		Assert.AreEqual(2, (int)input.Element("Quantity")!);
		Assert.AreEqual(2, craft.CraftProducts.Count);
		Assert.AreEqual(2, (int)XElement.Parse(craft.CraftProducts.Single(x => !x.IsFailProduct).Definition).Element("Quantity")!);
		Assert.AreEqual(1, (int)XElement.Parse(craft.CraftProducts.Single(x => x.IsFailProduct).Definition).Element("Quantity")!);
		StringAssert.Contains(db.SeederManagedRecords.Local.Single(x => x.EntityType == nameof(Craft)).SeedBaseline, "source room success");
	}

	[TestMethod]
	public void NpcInventoryCapacityUsesTheLargestFiniteWeightThroughTheInstalledExpressionEngine()
	{
		var capacity = new ExpressionEngine.Expression(ArmageddonWorldfileWriter.NpcInventoryCapacityExpression).EvaluateDouble();
		Assert.IsTrue(double.IsFinite(capacity));
		Assert.AreEqual(double.MaxValue, capacity);
	}

	[TestMethod]
	public void NpcRaceDefaultRestorationPreservesFalseAndZeroWithoutChangingItsBaseline()
	{
		using var db = ArmageddonInstallationSettingsTests.Fixture();
		ArmageddonInstallation.Prepare(ArmageddonInstallationSettingsTests.Factory(db));
		var baseline = db.Races.First();
		var originalBreathing = baseline.NeedsToBreathe;
		var race = (Race)db.Entry(baseline).CurrentValues.ToObject();
		race.Id = db.Races.Max(x => x.Id) + 1;
		race.Name = "Fixture non-breather (Worldfile NPC)";
		race.NeedsToBreathe = race.CanSwim = race.CanUseWeapons = race.RaceUsesStamina = false;
		race.BiteWeight = race.TrackIntensityVisual = race.SweatRateInLitresPerMinute = 0;
		db.Races.Add(race);
		var writer = new ArmageddonWorldfileWriter(db, new ArmageddonWorldfilePackage());
		writer.CaptureNpcRaceDefaults(race);
		db.SaveChanges();
		// Model the store-generated values returned by the relational INSERT.
		race.NeedsToBreathe = race.CanSwim = race.CanUseWeapons = race.RaceUsesStamina = true;
		race.BiteWeight = 1000;
		race.TrackIntensityVisual = 1;
		race.SweatRateInLitresPerMinute = 0.8;
		db.SaveChanges();
		writer.RestoreNpcRaceDefaults();
		db.SaveChanges();
		var raceId = race.Id;
		var baselineId = baseline.Id;
		db.ChangeTracker.Clear();
		var restored = db.Races.Single(x => x.Id == raceId);
		Assert.IsFalse(restored.NeedsToBreathe);
		Assert.IsFalse(restored.CanSwim);
		Assert.IsFalse(restored.CanUseWeapons);
		Assert.IsFalse(restored.RaceUsesStamina);
		Assert.AreEqual(0.0, restored.BiteWeight);
		Assert.AreEqual(0.0, restored.TrackIntensityVisual);
		Assert.AreEqual(0.0, restored.SweatRateInLitresPerMinute);
		Assert.AreEqual(originalBreathing, db.Races.Single(x => x.Id == baselineId).NeedsToBreathe);
	}
}
