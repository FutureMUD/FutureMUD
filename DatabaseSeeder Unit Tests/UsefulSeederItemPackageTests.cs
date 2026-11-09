#nullable enable

using DatabaseSeeder;
using DatabaseSeeder.Seeders;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Database;
using MudSharp.GameItems.Interfaces;
using MudSharp.Health;
using MudSharp.Models;
using MudSharp.RPG.Checks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using MaterialBehaviourType = MudSharp.Form.Material.MaterialBehaviourType;
using TraitOwnerScope = MudSharp.Body.Traits.TraitOwnerScope;
using TraitType = MudSharp.Body.Traits.TraitType;
using SizeCategory = MudSharp.GameItems.SizeCategory;

using static MudSharp_Unit_Tests.UsefulItemTestFixture;

namespace MudSharp_Unit_Tests;

[TestClass]
public class UsefulSeederItemPackageTests
{

	private static Dictionary<string, string> BuildUsefulAnswers(string covers = "no")
	{
		return new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
		{
			["ai"] = "no",
			["covers"] = covers,
			["items"] = "no",
			["modernitems"] = "no",

			["autobuilder"] = "no",
			["hints"] = "no",
			["dreams"] = "no"
		};
	}

	private static Terrain CreateTerrain(long id, string name, string tagInformation)
	{
		return new Terrain
		{
			Id = id,
			Name = name,
			HideDifficulty = 0,
			SpotDifficulty = 0,
			InfectionType = 0,
			InfectionVirulence = 0,
			InfectionMultiplier = 0,
			StaminaCost = 0,
			TerrainANSIColour = "7",
			TerrainEditorColour = "#FFFFFFFF",
			TerrainBehaviourMode = "outdoors",
			DefaultTerrain = id == 1,
			MovementRate = 1,
			ForagableProfileId = 0,
			AtmosphereType = "Gas",
			DefaultRoomOutdoorsType = 0,
			GravityModel = 0,
			TerrainEditorText = name[..Math.Min(2, name.Length)],
			CanHaveTracks = false,
			TrackIntensityMultiplierVisual = 1.0,
			TrackIntensityMultiplierOlfactory = 1.0,
			TagInformation = tagInformation
		};
	}

	private static void SeedRangedCoverTerrainPrerequisites(FuturemudDatabaseContext context)
	{
		Tag urban = CreateTag(101, "Urban");
		Tag rural = CreateTag(102, "Rural");
		Tag terrestrial = CreateTag(103, "Terrestrial");
		context.Tags.AddRange(urban, rural, terrestrial);
		context.Terrains.AddRange(
			CreateTerrain(1, "City Street", urban.Id.ToString()),
			CreateTerrain(2, "Country Field", $"{rural.Id},{terrestrial.Id}"));
		context.SaveChanges();
	}

	private static void SeedItemPackageCoverOverlap(FuturemudDatabaseContext context)
	{
		string[] names =
		[
			"Uneven Ground",
			"Upright Table",
			"Overturned Table",
			"Smoke",
			"Sandbag",
			"Tree"
		];

		long id = context.RangedCovers.Any() ? context.RangedCovers.Max(x => x.Id) + 1 : 1;
		foreach (string name in names)
		{
			context.RangedCovers.Add(new RangedCover
			{
				Id = id++,
				Name = name,
				CoverType = 0,
				CoverExtent = 0,
				HighestPositionState = 1,
				DescriptionString = name,
				ActionDescriptionString = name,
				MaximumSimultaneousCovers = 0,
				CoverStaysWhileMoving = false
			});
		}

		context.SaveChanges();
	}

	private static TraitDefinition CreateSkill(long id, string name)
	{
		return new TraitDefinition
		{
			Id = id,
			Name = name,
			Alias = name.Replace(" ", string.Empty).ToLowerInvariant(),
			Type = (int)TraitType.Skill,
			OwnerScope = (int)TraitOwnerScope.Character,
			TraitGroup = "Test",
			ChargenBlurb = string.Empty,
			ValueExpression = string.Empty
		};
	}

	private static Tag CreateTag(long id, string name)
	{
		return new Tag
		{
			Id = id,
			Name = name
		};
	}

	private static Material CreateMaterial(long id, string name, MaterialBehaviourType behaviourType)
	{
		return new Material
		{
			Id = id,
			Name = name,
			MaterialDescription = name,
			BehaviourType = (int)behaviourType,
			ResidueSdesc = string.Empty,
			ResidueDesc = string.Empty,
			ResidueColour = string.Empty
		};
	}

	private static void SeedRepairKitPrerequisites(FuturemudDatabaseContext context)
	{
		new CoreDataSeeder().SeedTagsForTesting(context);

		context.TraitDefinitions.AddRange(
			CreateSkill(101, "Tailoring"),
			CreateSkill(102, "Armourcrafting"),
			CreateSkill(103, "Weaponcrafting"),
			CreateSkill(104, "Blacksmithing"),
			CreateSkill(105, "Carpentry"),
			CreateSkill(106, "Masonry"),
			CreateSkill(107, "Pottery"),
			CreateSkill(108, "Scrimshawing"),
			CreateSkill(109, "Salvaging"),
			CreateSkill(110, "Glassworking"),
			CreateSkill(111, "Papermaking"),
			CreateSkill(112, "Lacquering"),
			CreateSkill(113, "Ropemaking"),
			CreateSkill(114, "Bowmaking"));

		context.Materials.AddRange(
			CreateMaterial(201, "linen", MaterialBehaviourType.Fabric),
			CreateMaterial(202, "hair", MaterialBehaviourType.Hair),
			CreateMaterial(203, "feather", MaterialBehaviourType.Feather),
			CreateMaterial(204, "leather", MaterialBehaviourType.Leather),
			CreateMaterial(205, "skin", MaterialBehaviourType.Skin),
			CreateMaterial(206, "flesh", MaterialBehaviourType.Flesh),
			CreateMaterial(207, "bronze", MaterialBehaviourType.Metal),
			CreateMaterial(208, "oak", MaterialBehaviourType.Wood),
			CreateMaterial(209, "limestone", MaterialBehaviourType.Stone),
			CreateMaterial(210, "earthenware", MaterialBehaviourType.Ceramic),
			CreateMaterial(211, "bone", MaterialBehaviourType.Bone),
			CreateMaterial(212, "shell", MaterialBehaviourType.Shell),
			CreateMaterial(213, "horn", MaterialBehaviourType.Horn),
			CreateMaterial(214, "tooth", MaterialBehaviourType.Tooth),
			CreateMaterial(215, "scale", MaterialBehaviourType.Scale),
			CreateMaterial(216, "claw", MaterialBehaviourType.Claw),
			CreateMaterial(217, "beak", MaterialBehaviourType.Beak),
			CreateMaterial(218, "glass", MaterialBehaviourType.Paste),
			CreateMaterial(219, "silicate glass", MaterialBehaviourType.Paste),
			CreateMaterial(220, "soda-lime glass", MaterialBehaviourType.Paste),
			CreateMaterial(221, "lead glass", MaterialBehaviourType.Paste),
			CreateMaterial(222, "paper", MaterialBehaviourType.Plant),
			CreateMaterial(223, "parchment", MaterialBehaviourType.Skin),
			CreateMaterial(224, "papyrus", MaterialBehaviourType.Plant),
			CreateMaterial(225, "lacquer", MaterialBehaviourType.Grease));
		context.SaveChanges();
	}

    [TestMethod]
    public void RepairKitRerun_PreservesCustomTagReferencesInsteadOfRebindingByLeaf()
    {
        using var context = BuildContext();
        SeedGeneralPrerequisites(context);
        SeedRepairKitPrerequisites(context);
        var seeder = new UsefulSeeder();
        seeder.SeedRepairKitsForTesting(context);
        var kit = context.GameItemComponentProtos.Single(x => x.Name == "Repair_Cordage");
        var custom = new Tag { Name = "Custom cordage repair" };
        context.Tags.Add(custom); context.SaveChanges();
        var xml = XElement.Parse(kit.Definition);
        xml.Element("Tags")!.Element("Tag")!.Value = custom.Id.ToString();
        kit.Definition = xml.ToString(); context.SaveChanges();
        seeder.SeedRepairKitsForTesting(context);
        Assert.AreEqual(xml.ToString(), kit.Definition);
    }

    [TestMethod]
    public void IncenseRerun_PreservesForeignPathFuelReference()
    {
        using var context = BuildContext();
        SeedGeneralPrerequisites(context);
        SeedMarketCategories(context);
        new CoreDataSeeder().SeedTagsForTesting(context);
        var seeder = new UsefulSeeder();
        seeder.SeedAntiquityComponentGapCoverageForTesting(context);
        var burner = context.GameItemComponentProtos.Single(x => x.Name == "IncenseBurner_Antiquity_BronzeCenser");
        var custom = new Tag { Name = "Incense Fuel", Parent = new Tag { Name = "Builder ritual supplies" } };
        context.Tags.Add(custom); context.SaveChanges();
        var xml = XElement.Parse(burner.Definition);
        xml.Element("FuelTag")!.Value = custom.Id.ToString();
        burner.Definition = xml.ToString(); context.SaveChanges();
        seeder.SeedAntiquityComponentGapCoverageForTesting(context);
        Assert.AreEqual(xml.ToString(), burner.Definition);
        Assert.AreEqual("Builder ritual supplies", custom.Parent.Name);
    }

	[TestMethod]
	public void ClassifyItemPackagePresence_NonePartialAndFull_ReturnExpectedStates()
	{
		using FuturemudDatabaseContext context = BuildContext();
		SeedGeneralPrerequisites(context);

		Assert.AreEqual(ShouldSeedResult.ReadyToInstall, UsefulSeeder.ClassifyItemPackagePresence(context));

		context.GameItemComponentProtos.Add(CreateComponentMarker(10, UsefulSeeder.StockItemMarkersForTesting.First()));
		context.SaveChanges();
		Assert.AreEqual(ShouldSeedResult.ExtraPackagesAvailable, UsefulSeeder.ClassifyItemPackagePresence(context));

		context.GameItemComponentProtos.RemoveRange(context.GameItemComponentProtos.ToList());
		long id = 20L;
		foreach (string name in UsefulSeeder.StockItemMarkersForTesting.Where(x => x != "Container_Bookcase_Shelves"))
		{
			context.GameItemComponentProtos.Add(CreateComponentMarker(id++, name));
		}

		context.SaveChanges();

		Assert.AreEqual(ShouldSeedResult.ExtraPackagesAvailable, UsefulSeeder.ClassifyItemPackagePresence(context));
		Assert.IsTrue(UsefulSeeder.StockItemMarkersForTesting.Contains("Container_Bookcase_Shelves"));
		Assert.IsTrue(UsefulSeeder.StockItemMarkersForTesting.Contains("TimePiece_Antiquity_Sundial"));
		Assert.IsTrue(UsefulSeeder.StockItemMarkersForTesting.Contains("SealStamp_Antiquity_BronzeSignet"));
		Assert.IsTrue(UsefulSeeder.StockItemMarkersForTesting.Contains("Tool_Blacksmithing_General"));
		Assert.IsTrue(UsefulSeeder.StockItemMarkersForTesting.Contains("Tool_Jewellery_General"));
		Assert.IsTrue(UsefulSeeder.StockItemMarkersForTesting.Contains("Tool_Printing_Woodblock_General"));
		Assert.IsTrue(UsefulSeeder.StockItemMarkersForTesting.Contains("Medieval_Parchment_Sheet_Surface"));
		Assert.IsTrue(UsefulSeeder.StockItemMarkersForTesting.Contains("Medieval_Wax_Tablet_Surface"));
		Assert.IsTrue(UsefulSeeder.StockItemMarkersForTesting.Contains("Medieval_Quill_Pen"));
		Assert.IsTrue(UsefulSeeder.StockItemMarkersForTesting.Contains("Variable_JewelleryMotif"));
		Assert.IsTrue(UsefulSeeder.StockItemMarkersForTesting.Contains("SealStamp_Medieval_BronzeSignet"));
		Assert.IsTrue(UsefulSeeder.StockItemMarkersForTesting.Contains("SealStamp_Medieval_RingSignet"));
		Assert.IsTrue(UsefulSeeder.StockItemMarkersForTesting.Contains("Medieval_Parchment_Codex_40_Page"));
		Assert.IsTrue(UsefulSeeder.StockItemMarkersForTesting.Contains("Container_Document_Pouch"));
		Assert.IsTrue(UsefulSeeder.StockItemMarkersForTesting.Contains("Container_Archive_Chest"));
		Assert.IsTrue(UsefulSeeder.StockItemMarkersForTesting.Contains("Sealable_Envelope"));
		Assert.IsTrue(UsefulSeeder.StockItemMarkersForTesting.Contains("Sealable_Antiquity_Clay_Bulla"));
		Assert.IsTrue(UsefulSeeder.StockItemMarkersForTesting.Contains("Sealable_Modern_Security_Envelope"));
		Assert.IsTrue(UsefulSeeder.StockItemMarkersForTesting.Contains("MeasuringInstrument_Antiquity_BalanceScale"));
		Assert.IsTrue(UsefulSeeder.StockItemMarkersForTesting.Contains("IncenseBurner_Antiquity_BronzeCenser"));
		Assert.IsTrue(UsefulSeeder.StockItemMarkersForTesting.Contains("OfferingReceiver_Antiquity_HouseholdAltar"));
		Assert.IsTrue(UsefulSeeder.StockItemMarkersForTesting.Contains("Destroyable_Shield"));
		Assert.IsTrue(UsefulSeeder.StockItemMarkersForTesting.Contains("Latch_Portcullis_Pawl"));
		Assert.IsTrue(UsefulSeeder.StockItemMarkersForTesting.Contains("Infinite_PublicTapWaterSource"));

		context.GameItemComponentProtos.Add(CreateComponentMarker(id++, "Container_Bookcase_Shelves"));
		context.SaveChanges();

		Assert.AreEqual(ShouldSeedResult.MayAlreadyBeInstalled, UsefulSeeder.ClassifyItemPackagePresence(context));
	}

	[TestMethod]
	[DataRow("all")]
	[DataRow("Variable_LeatherColour")]
	[DataRow("Variable_WoodColour")]
	[DataRow("Variable_LacquerColour")]
	public void ClassifyItemPackagePresence_MissingClothingColours_AdvertisesExistingPackage(string missing)
	{
		using var context = BuildContext();
		SeedGeneralPrerequisites(context);
		string[] clothingComponents = ["Variable_LeatherColour", "Variable_WoodColour", "Variable_LacquerColour"];
		var installed = UsefulSeeder.StockItemMarkersForTesting
			.Concat(clothingComponents)
			.Distinct(StringComparer.Ordinal)
			.Where(name => missing == "all" ? !clothingComponents.Contains(name) : name != missing);
		long id = 100;
		foreach (var name in installed)
		{
			context.GameItemComponentProtos.Add(CreateComponentMarker(id++, name));
		}

		context.SaveChanges();
		Assert.AreEqual(ShouldSeedResult.ExtraPackagesAvailable, UsefulSeeder.ClassifyItemPackagePresence(context),
			"A previously complete standard-component installation must advertise missing clothing prerequisites.");
		var question = new UsefulSeeder().SeederQuestions.Single(x => x.Id == "items");
		Assert.IsTrue(question.Filter(context, new Dictionary<string, string>()));
		Assert.IsTrue(question.Validator("yes", context).Success);

		foreach (var name in clothingComponents.Where(name => missing == "all" || name == missing))
		{
			context.GameItemComponentProtos.Add(CreateComponentMarker(id++, name));
		}

		context.SaveChanges();
		Assert.AreEqual(ShouldSeedResult.MayAlreadyBeInstalled, UsefulSeeder.ClassifyItemPackagePresence(context));
		Assert.IsFalse(context.ChangeTracker.HasChanges());
	}

	[TestMethod]
	public void SeedEraDependencyComponentsForTesting_RerunPreservesInstalledComponentIds()
	{
		using var context = BuildContext();
		SeedGeneralPrerequisites(context);
		new CoreDataSeeder().SeedTagsForTesting(context);
		var seeder = new UsefulSeeder();
		var prior0 = context.GameItemComponentProtos.Select(x => x.Id).ToArray();
		seeder.SeedEraDependencyComponentsForTesting(context);
		var installed0 = context.GameItemComponentProtos.Select(x => x.Id).OrderBy(x => x).ToArray();
		Assert.IsTrue(installed0.Except(prior0).Any(), "The first installation must create records beyond its prerequisites.");
		seeder.SeedEraDependencyComponentsForTesting(context);
		CollectionAssert.AreEqual(installed0, context.GameItemComponentProtos.Select(x => x.Id).OrderBy(x => x).ToArray());
	}

    [TestMethod]
    public void CoreTags_DoNotCountAsInstalledUsefulPackages()
    {
        using var context = BuildContext();
        SeedGeneralPrerequisites(context);
        new CoreDataSeeder().SeedTagsForTesting(context);
        Assert.AreEqual(ShouldSeedResult.ReadyToInstall, UsefulSeeder.ClassifyItemPackagePresence(context));
        Assert.AreEqual(ShouldSeedResult.ReadyToInstall, UsefulSeeder.ClassifyAutobuilderPackagePresence(context));
        Assert.IsFalse(new UsefulSeeder().SeederQuestions.Any(x => x.Id == "tags"));
    }

	[TestMethod]
	public void ClassifyRangedCoverPackagePresence_ItemPackageCoverOverlapOnly_ReturnsReadyToInstall()
	{
		using FuturemudDatabaseContext context = BuildContext();
		SeedGeneralPrerequisites(context);
		SeedRangedCoverTerrainPrerequisites(context);
		SeedItemPackageCoverOverlap(context);

		Assert.AreEqual(ShouldSeedResult.ReadyToInstall, UsefulSeeder.ClassifyRangedCoverPackagePresence(context));
	}

	[TestMethod]
	public void SeedData_CoversWithExistingItemPackageCoverRows_AddsMissingCoversAndLinks()
	{
		using FuturemudDatabaseContext context = BuildContext();
		SeedGeneralPrerequisites(context);
		SeedRangedCoverTerrainPrerequisites(context);
		SeedItemPackageCoverOverlap(context);
		UsefulSeeder seeder = new();

		var coversQuestion = seeder.SeederQuestions.Single(x => x.Id == "covers");
		Assert.IsTrue(coversQuestion.Filter(context, BuildUsefulAnswers()));

		string result = seeder.SeedData(context, BuildUsefulAnswers(covers: "yes"));

		Assert.AreEqual("The operation completed successfully.", result);
		Assert.AreEqual(1, context.RangedCovers.Count(x => x.Name == "Smoke"));
		Assert.AreEqual(1, context.RangedCovers.Count(x => x.Name == "Corridor Doorway"));

		Terrain cityStreet = context.Terrains.Single(x => x.Name == "City Street");
		RangedCover doorway = context.RangedCovers.Single(x => x.Name == "Corridor Doorway");
		Assert.IsTrue(context.TerrainsRangedCovers.Any(x =>
			x.TerrainId == cityStreet.Id &&
			x.RangedCoverId == doorway.Id));
		Assert.AreEqual(ShouldSeedResult.MayAlreadyBeInstalled, UsefulSeeder.ClassifyRangedCoverPackagePresence(context));
	}

	[TestMethod]
	public void SeedLockAndWaterSourceCoverageForTesting_RerunPreservesInstalledComponentIds()
	{
		using var context = BuildContext();
		SeedGeneralPrerequisites(context);
		var seeder = new UsefulSeeder();
		var prior0 = context.GameItemComponentProtos.Select(x => x.Id).ToArray();
		seeder.SeedLockAndWaterSourceCoverageForTesting(context);
		var installed0 = context.GameItemComponentProtos.Select(x => x.Id).OrderBy(x => x).ToArray();
		Assert.IsTrue(installed0.Except(prior0).Any(), "The first installation must create records beyond its prerequisites.");
		seeder.SeedLockAndWaterSourceCoverageForTesting(context);
		CollectionAssert.AreEqual(installed0, context.GameItemComponentProtos.Select(x => x.Id).OrderBy(x => x).ToArray());
	}

	[TestMethod]
	public void SeedContainersForTesting_RerunPreservesInstalledComponentIds()
	{
		using var context = BuildContext();
		SeedGeneralPrerequisites(context);
		var seeder = new UsefulSeeder();
		var prior0 = context.GameItemComponentProtos.Select(x => x.Id).ToArray();
		seeder.SeedContainersForTesting(context);
		var installed0 = context.GameItemComponentProtos.Select(x => x.Id).OrderBy(x => x).ToArray();
		Assert.IsTrue(installed0.Except(prior0).Any(), "The first installation must create records beyond its prerequisites.");
		seeder.SeedContainersForTesting(context);
		CollectionAssert.AreEqual(installed0, context.GameItemComponentProtos.Select(x => x.Id).OrderBy(x => x).ToArray());
	}

	[TestMethod]
	public void SeedGeneralCoverageForTesting_RerunPreservesInstalledComponentIds()
	{
		using var context = BuildContext();
		SeedGeneralPrerequisites(context);
		var seeder = new UsefulSeeder();
		var prior0 = context.GameItemComponentProtos.Select(x => x.Id).ToArray();
		seeder.SeedGeneralCoverageForTesting(context);
		var installed0 = context.GameItemComponentProtos.Select(x => x.Id).OrderBy(x => x).ToArray();
		Assert.IsTrue(installed0.Except(prior0).Any(), "The first installation must create records beyond its prerequisites.");
		seeder.SeedGeneralCoverageForTesting(context);
		CollectionAssert.AreEqual(installed0, context.GameItemComponentProtos.Select(x => x.Id).OrderBy(x => x).ToArray());
	}

	[TestMethod]
	public void SeedAntiquityComponentGapCoverageForTesting_RerunPreservesInstalledComponentIds()
	{
		using var context = BuildContext();
		SeedGeneralPrerequisites(context);
		SeedMarketCategories(context);
		new CoreDataSeeder().SeedTagsForTesting(context);
		var seeder = new UsefulSeeder();
		var prior0 = context.GameItemComponentProtos.Select(x => x.Id).ToArray();
		seeder.SeedAntiquityComponentGapCoverageForTesting(context);
		var installed0 = context.GameItemComponentProtos.Select(x => x.Id).OrderBy(x => x).ToArray();
		Assert.IsTrue(installed0.Except(prior0).Any(), "The first installation must create records beyond its prerequisites.");
		seeder.SeedAntiquityComponentGapCoverageForTesting(context);
		CollectionAssert.AreEqual(installed0, context.GameItemComponentProtos.Select(x => x.Id).OrderBy(x => x).ToArray());
	}

	[TestMethod]
	public void SeedRepairKitsForTesting_RerunPreservesInstalledComponentIds()
	{
		using var context = BuildContext();
		SeedGeneralPrerequisites(context);
		SeedRepairKitPrerequisites(context);
		var seeder = new UsefulSeeder();
		var prior0 = context.GameItemComponentProtos.Select(x => x.Id).ToArray();
		seeder.SeedRepairKitsForTesting(context);
		var installed0 = context.GameItemComponentProtos.Select(x => x.Id).OrderBy(x => x).ToArray();
		Assert.IsTrue(installed0.Except(prior0).Any(), "The first installation must create records beyond its prerequisites.");
		seeder.SeedRepairKitsForTesting(context);
		CollectionAssert.AreEqual(installed0, context.GameItemComponentProtos.Select(x => x.Id).OrderBy(x => x).ToArray());
	}

	[TestMethod]
	public void SeedWornTraitChangersForTesting_NoExpectedSkillPackageTraits_SkipsFamily()
	{
		using FuturemudDatabaseContext context = BuildContext();
		SeedGeneralPrerequisites(context);
		UsefulSeeder seeder = new();

		seeder.SeedWornTraitChangersForTesting(context);

		Assert.AreEqual(0,
			context.GameItemComponentProtos.Count(x => x.Name.StartsWith("WornTraitChanger_") && x.Type == "WornTraitChanger"));
	}

	[TestMethod]
	public void SeedWornTraitChangersForTesting_WithSkillPackageTraits_UpsertsGradedFamilies()
	{
		using FuturemudDatabaseContext context = BuildContext();
		SeedGeneralPrerequisites(context);
		context.TraitDefinitions.AddRange(
			CreateSkill(101, "Hiding"),
			CreateSkill(102, "Sneaking"),
			CreateSkill(103, "Swimming"),
			CreateSkill(104, "Climbing"),
			CreateSkill(105, "Flying"),
			CreateSkill(106, "Running"),
			CreateSkill(107, "Palming"),
			CreateSkill(108, "Lockpicking"),
			CreateSkill(109, "Stealing"),
			CreateSkill(110, "Surgery"),
			CreateSkill(111, "First Aid"),
			CreateSkill(112, "Spotting"),
			CreateSkill(113, "Searching"),
			CreateSkill(114, "Tracking"),
			CreateSkill(115, "Listening"),
			CreateSkill(116, "Handwriting"));
		context.GameItemComponentProtos.Add(CreateComponentMarker(200,
			"WornTraitChanger_StealthPenalty_Moderate", "OldType"));
		context.SaveChanges();
		UsefulSeeder seeder = new();

		seeder.SeedWornTraitChangersForTesting(context);
		seeder.SeedWornTraitChangersForTesting(context);

		GameItemComponentProto stealthPenalty = context.GameItemComponentProtos.Single(x =>
			x.Name == "WornTraitChanger_StealthPenalty_Moderate");
		Assert.AreEqual("WornTraitChanger", stealthPenalty.Type);
		Assert.AreEqual(1, context.GameItemComponentProtos.Count(x => x.Name == "WornTraitChanger_StealthPenalty_Moderate"));

	}
}
