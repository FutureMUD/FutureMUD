#nullable enable

using DatabaseSeeder;
using DatabaseSeeder.Seeders;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Database;
using MudSharp.Health;
using MudSharp.Models;
using System;
using System.Linq;
using System.Xml.Linq;
using MaterialBehaviourType = MudSharp.Form.Material.MaterialBehaviourType;
using TraitOwnerScope = MudSharp.Body.Traits.TraitOwnerScope;
using TraitType = MudSharp.Body.Traits.TraitType;
using SizeCategory = MudSharp.GameItems.SizeCategory;

namespace MudSharp_Unit_Tests;

[TestClass]
public class UsefulSeederItemPackageTests
{
	private static FuturemudDatabaseContext BuildContext()
	{
		DbContextOptions<FuturemudDatabaseContext> options = new DbContextOptionsBuilder<FuturemudDatabaseContext>()
			.UseInMemoryDatabase(Guid.NewGuid().ToString())
			.ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning))
			.Options;
		return new FuturemudDatabaseContext(options);
	}

	private static void SeedAccount(FuturemudDatabaseContext context)
	{
		context.Accounts.Add(new Account
		{
			Id = 1,
			Name = "SeederTest",
			Password = "password",
			Salt = 1,
			AccessStatus = 0,
			Email = "seeder@example.com",
			LastLoginIp = "127.0.0.1",
			FormatLength = 80,
			InnerFormatLength = 78,
			UseMxp = false,
			UseMsp = false,
			UseMccp = false,
			ActiveCharactersAllowed = 1,
			UseUnicode = true,
			TimeZoneId = "UTC",
			CultureName = "en-AU",
			RegistrationCode = string.Empty,
			IsRegistered = true,
			RecoveryCode = string.Empty,
			UnitPreference = "metric",
			CreationDate = DateTime.UtcNow,
			PageLength = 22,
			PromptType = 0,
			TabRoomDescriptions = false,
			CodedRoomDescriptionAdditionsOnNewLine = false,
			CharacterNameOverlaySetting = 0,
			AppendNewlinesBetweenMultipleEchoesPerPrompt = false,
			ActLawfully = false,
			HasBeenActiveInWeek = true,
			HintsEnabled = true,
			AutoReacquireTargets = false
		});
	}

	private static void SeedGeneralPrerequisites(FuturemudDatabaseContext context)
	{
		SeedAccount(context);

		context.Liquids.Add(new Liquid
		{
			Id = 1,
			Name = "water",
			Description = "water",
			LongDescription = "water",
			TasteText = "water",
			VagueTasteText = "water",
			SmellText = "water",
			VagueSmellText = "water",
			DisplayColour = "blue",
			DampDescription = "water-damp",
			WetDescription = "water-wet",
			DrenchedDescription = "water-drenched",
			DampShortDescription = "water-damp",
			WetShortDescription = "water-wet",
			DrenchedShortDescription = "water-drenched",
			SurfaceReactionInfo = "water"
		});

		context.Clocks.Add(new Clock
		{
			Id = 1,
			Definition = "<Clock />",
			Seconds = 0,
			Minutes = 0,
			Hours = 12,
			PrimaryTimezoneId = 1
		});
		context.Timezones.Add(new Timezone
		{
			Id = 1,
			Name = "UTC",
			Description = "Test timezone",
			OffsetHours = 0,
			OffsetMinutes = 0,
			ClockId = 1
		});

		context.SaveChanges();
	}

	private static GameItemComponentProto CreateComponentMarker(long id, string name, string type = "Test")
	{
		return new GameItemComponentProto
		{
			Id = id,
			Name = name,
			Type = type,
			Description = $"{name} marker",
			Definition = "<Definition />",
			RevisionNumber = 0,
			EditableItem = new EditableItem
			{
				RevisionNumber = 0,
				RevisionStatus = 4,
				BuilderAccountId = 1,
				BuilderDate = DateTime.UtcNow,
				BuilderComment = "test",
				ReviewerAccountId = 1,
				ReviewerComment = "test",
				ReviewerDate = DateTime.UtcNow
			}
		};
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

	private static MarketCategory CreateMarketCategory(long id, string name)
	{
		return new MarketCategory
		{
			Id = id,
			Name = name,
			Description = $"{name} test category",
			ElasticityFactorAbove = 0.5,
			ElasticityFactorBelow = 0.5,
			MarketCategoryType = 0,
			Tags = "<Tags />",
			CombinationCategories = "<Components />"
		};
	}

	private static void SeedMarketCategories(FuturemudDatabaseContext context)
	{
		string[] names =
		[
			"Staple Food",
			"Standard Food",
			"Beer",
			"Wine",
			"Luxury Wares",
			"Luxury Clothing",
			"Luxury Furniture",
			"Luxury Decorations",
			"Military Goods",
			"Weapons",
			"Armour",
			"Ammunition",
			"Shields"
		];

		context.MarketCategories.AddRange(names.Select((name, index) => CreateMarketCategory(index + 1, name)));
		context.SaveChanges();
	}

	private static void SeedRepairKitPrerequisites(FuturemudDatabaseContext context)
	{
		context.Tags.AddRange(
			CreateTag(1, "Armour"),
			CreateTag(2, "Weapons"),
			CreateTag(3, "Tools"));

		context.TraitDefinitions.AddRange(
			CreateSkill(101, "Tailoring"),
			CreateSkill(102, "Armourcrafting"),
			CreateSkill(103, "Weaponcrafting"),
			CreateSkill(104, "Blacksmithing"),
			CreateSkill(105, "Carpentry"),
			CreateSkill(106, "Masonry"),
			CreateSkill(107, "Pottery"),
			CreateSkill(108, "Scrimshawing"),
			CreateSkill(109, "Salvaging"));

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
			CreateMaterial(217, "beak", MaterialBehaviourType.Beak));
		context.SaveChanges();
	}

	private static void AssertContainerDefinition(GameItemComponentProto component, double weight, SizeCategory maxSize, string preposition, bool closable, bool transparent)
	{
		XElement definition = XElement.Parse(component.Definition);
		Assert.AreEqual(weight, (double)definition.Attribute("Weight")!);
		Assert.AreEqual((int)maxSize, (int)definition.Attribute("MaxSize")!);
		Assert.AreEqual(preposition, (string)definition.Attribute("Preposition")!);
		Assert.AreEqual(closable, bool.Parse((string)definition.Attribute("Closable")!));
		Assert.AreEqual(transparent, bool.Parse((string)definition.Attribute("Transparent")!));
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
		Assert.IsTrue(UsefulSeeder.StockItemMarkersForTesting.Contains("Sealable_Envelope"));
		Assert.IsTrue(UsefulSeeder.StockItemMarkersForTesting.Contains("MeasuringInstrument_Antiquity_BalanceScale"));

		context.GameItemComponentProtos.Add(CreateComponentMarker(id++, "Container_Bookcase_Shelves"));
		context.SaveChanges();

		Assert.AreEqual(ShouldSeedResult.MayAlreadyBeInstalled, UsefulSeeder.ClassifyItemPackagePresence(context));
	}

	[TestMethod]
	public void SeedContainersForTesting_RerunDoesNotDuplicateAndCreatesFurnitureCoverage()
	{
		using FuturemudDatabaseContext context = BuildContext();
		SeedGeneralPrerequisites(context);
		UsefulSeeder seeder = new();

		seeder.SeedContainersForTesting(context);
		seeder.SeedContainersForTesting(context);

		string[] expectedNames =
		[
			"Container_Side_Table",
			"Container_Desk_Surface",
			"Container_Counter",
			"Container_Cot_Surface",
			"Container_Bed_Surface",
			"Container_Couch_Surface",
			"Container_Bench_Surface",
			"Container_Wall_Shelf",
			"Container_Narrow_Shelves",
			"Container_Wide_Shelves",
			"Container_Bookcase_Shelves",
			"Container_Display_Shelves",
			"Container_Weapon_Rack",
			"Container_Armor_Stand",
			"Container_Open_Bin",
			"Container_Small_Cabinet",
			"Container_Large_Cabinet",
			"Container_Glass_Cabinet",
			"Container_Cupboard",
			"Container_Wardrobe",
			"Container_Armoire",
			"Container_Dresser",
			"Container_Desk_Drawers",
			"Container_Nightstand",
			"Container_Sideboard",
			"Container_Hutch",
			"Container_Trunk",
			"Container_Footlocker",
			"Container_Blanket_Box",
			"Container_Display_Case"
		];

		foreach (string name in expectedNames)
		{
			Assert.AreEqual(1, context.GameItemComponentProtos.Count(x => x.Name == name), $"Expected a single container component named {name}.");
		}

		Assert.AreEqual(60, context.GameItemComponentProtos.Count(x => x.Type == "Container"));

		AssertContainerDefinition(context.GameItemComponentProtos.Single(x => x.Name == "Container_Bookcase_Shelves"),
			175000, SizeCategory.Large, "on", false, true);
		AssertContainerDefinition(context.GameItemComponentProtos.Single(x => x.Name == "Container_Glass_Cabinet"),
			150000, SizeCategory.Normal, "in", true, true);
		AssertContainerDefinition(context.GameItemComponentProtos.Single(x => x.Name == "Container_Trunk"),
			200000, SizeCategory.Large, "in", true, false);
	}

	[TestMethod]
	public void SeedGeneralCoverageForTesting_RerunDoesNotDuplicateAndCreatesExpandedCoverage()
	{
		using FuturemudDatabaseContext context = BuildContext();
		SeedGeneralPrerequisites(context);
		UsefulSeeder seeder = new();

		seeder.SeedGeneralCoverageForTesting(context);
		seeder.SeedGeneralCoverageForTesting(context);

		string[] expectedNames =
		[
			"Smokeable_Cigar",
			"Smokeable_Cigarillo",
			"Smokeable_PipeBowl",
			"DragAid_Stretcher",
			"DragAid_Sled",
			"DragAid_Travois",
			"Treatment_AntiInflammatory_Single",
			"Treatment_AntiInflammatory_Kit",
			"TimePiece_PocketWatch",
			"TimePiece_WallClock"
		];

		foreach (string name in expectedNames)
		{
			Assert.AreEqual(1, context.GameItemComponentProtos.Count(x => x.Name == name), $"Expected a single general component named {name}.");
		}

		Assert.AreEqual(1, context.GameItemComponentProtos.Count(x => x.Name == "Cigarette"));
		Assert.AreEqual(1, context.FutureProgs.Count(x => x.FunctionName == "OnSmokeCigarette"));
		Assert.AreEqual(1, context.VariableDefinitions.Count(x => x.Property == "nicotineuntil"));
		Assert.AreEqual(1, context.VariableDefaults.Count(x => x.Property == "nicotineuntil"));
		Assert.AreEqual(0, context.GameItemComponentProtos.Count(x => x.Name.StartsWith("Food_")));
	}

	[TestMethod]
	public void SeedAntiquityComponentGapCoverageForTesting_RerunDoesNotDuplicateAndSeedsReportComponents()
	{
		using FuturemudDatabaseContext context = BuildContext();
		SeedGeneralPrerequisites(context);
		SeedMarketCategories(context);
		UsefulSeeder seeder = new();

		seeder.SeedAntiquityComponentGapCoverageForTesting(context);
		seeder.SeedAntiquityComponentGapCoverageForTesting(context);

		string[] expectedNames =
		[
			"TimePiece_Antiquity_Sundial",
			"TimePiece_Antiquity_WaterClock",
			"TimePiece_Antiquity_MarkedCandle",
			"TimePiece_Antiquity_WatchBoard",
			"WaterSource_Antiquity_PublicWell",
			"WaterSource_Antiquity_Cistern",
			"WaterSource_Antiquity_Fountain",
			"WaterSource_Antiquity_BathPool",
			"WaterSource_Antiquity_RitualBasin",
			"WaterSource_Antiquity_IrrigationOutlet",
			"ShopStall_Antiquity_OpenCounter",
			"ShopStall_Antiquity_LockableCounter",
			"ShopStall_Antiquity_PortableBooth",
			"MarketGoodWeight_Antiquity_StapleFood",
			"MarketGoodWeight_Antiquity_LuxuryCraft",
			"MarketGoodWeight_Antiquity_MilitarySupply",
			"Dice_Antiquity_Knucklebones",
			"Dice_Antiquity_CastingSticks",
			"Dice_Antiquity_LoadedD6",
			"Dice_Antiquity_DivinationLots",
			"DragAid_Antiquity_FieldStretcher",
			"DragAid_Antiquity_CorpseBier",
			"DragAid_Antiquity_CargoSled",
			"DragAid_Antiquity_PackTravois",
			"DragAid_Antiquity_CarryingSling",
			"Locksmithing_Antiquity_BronzePoor",
			"Locksmithing_Antiquity_BronzeStandard",
			"Locksmithing_Antiquity_FineSteel",
			"Locksmithing_Antiquity_Installation",
			"Locksmithing_Antiquity_Fabrication",
			"SealStamp_Antiquity_BronzeSignet",
			"SealStamp_Antiquity_CylinderSeal",
			"Sealable_Document_Wax",
			"Sealable_Document_Clay",
			"Sealable_Envelope",
			"Sealable_Scroll",
			"Sealable_Container_Wax",
			"MeasuringInstrument_Antiquity_BalanceScale",
			"MeasuringInstrument_Antiquity_StandardWeights",
			"MeasuringInstrument_Antiquity_FalseWeights",
			"MeasuringInstrument_Antiquity_GrainMeasure",
			"MeasuringInstrument_Antiquity_OilCup",
			"MeasuringInstrument_Antiquity_WineCup",
			"MeasuringInstrument_Antiquity_TaxAssessorKit",
			"Container_Envelope",
			"PaperSheet_Envelope",
			"PaperSheet_Scroll"
		];

		foreach (string name in expectedNames)
		{
			Assert.AreEqual(1, context.GameItemComponentProtos.Count(x => x.Name == name), $"Expected one component named {name}.");
		}

		XElement Definition(string name) => XElement.Parse(context.GameItemComponentProtos.Single(x => x.Name == name).Definition);

		Assert.AreEqual("$c", (string)Definition("TimePiece_Antiquity_Sundial").Element("TimeDisplayString")!);
		Assert.AreEqual(100.0, (double)Definition("WaterSource_Antiquity_RitualBasin").Attribute("LiquidCapacity")!);
		Assert.AreEqual(0.0, (double)Definition("WaterSource_Antiquity_RitualBasin").Attribute("RefillRate")!);
		Assert.AreEqual(6, (int)Definition("DragAid_Antiquity_CorpseBier").Element("MaximumUsers")!);
		Assert.AreEqual(-2, (int)Definition("Locksmithing_Antiquity_BronzePoor").Element("DifficultyAdjustment")!);
		Assert.AreEqual("on", (string)Definition("ShopStall_Antiquity_OpenCounter").Attribute("Preposition")!);
		Assert.AreEqual("a bronze signet showing a lion beneath a civic star",
			(string)Definition("SealStamp_Antiquity_BronzeSignet").Element("SealDesign")!);
		Assert.IsTrue(Definition("Sealable_Envelope").Element("AllowedMedia")!.Elements("Medium")
		                                      .Any(x => (string)x == "wax"));
		Assert.AreEqual("Weight", (string)Definition("MeasuringInstrument_Antiquity_BalanceScale").Element("Mode")!);
		Assert.AreEqual("FluidVolume", (string)Definition("MeasuringInstrument_Antiquity_OilCup").Element("Mode")!);

		XElement loadedDie = Definition("Dice_Antiquity_LoadedD6");
		Assert.AreEqual(6, loadedDie.Element("Faces")!.Elements("Face").Count());
		Assert.AreEqual(2.5, (double)loadedDie.Element("Weights")!.Elements("Weight").Last().Element("Probability")!);

		XElement militaryWeight = Definition("MarketGoodWeight_Antiquity_MilitarySupply");
		long weaponsCategoryId = context.MarketCategories.Single(x => x.Name == "Weapons").Id;
		Assert.IsTrue(militaryWeight.Element("Multipliers")!.Elements("Multiplier")
			.Any(x => (long)x.Attribute("category")! == weaponsCategoryId && (decimal)x.Attribute("value")! == 1.25m));

		GameItemProto envelope = context.GameItemProtos
		                                .Include(x => x.GameItemProtosGameItemComponentProtos)
		                                .ThenInclude(x => x.GameItemComponent)
		                                .Single(x => x.ShortDescription == "a sealable envelope");
		CollectionAssert.IsSubsetOf(new[]
			{
				"Holdable",
				"Container_Envelope",
				"PaperSheet_Envelope",
				"Sealable_Envelope"
			},
			envelope.GameItemProtosGameItemComponentProtos.Select(x => x.GameItemComponent.Name).ToArray());

		GameItemProto scroll = context.GameItemProtos
		                            .Include(x => x.GameItemProtosGameItemComponentProtos)
		                            .ThenInclude(x => x.GameItemComponent)
		                            .Single(x => x.ShortDescription == "a sealable scroll");
		CollectionAssert.IsSubsetOf(new[]
			{
				"Holdable",
				"PaperSheet_Scroll",
				"Sealable_Scroll"
			},
			scroll.GameItemProtosGameItemComponentProtos.Select(x => x.GameItemComponent.Name).ToArray());
	}

	[TestMethod]
	public void SeedRepairKitsForTesting_RerunDoesNotDuplicateAndAddsGeneralMaterialFamilies()
	{
		using FuturemudDatabaseContext context = BuildContext();
		SeedGeneralPrerequisites(context);
		SeedRepairKitPrerequisites(context);
		UsefulSeeder seeder = new();

		seeder.SeedRepairKitsForTesting(context);
		seeder.SeedRepairKitsForTesting(context);

		foreach (var name in new[]
		{
			"Repair_Wood",
			"Repair_Wood_Good",
			"Repair_Wood_Poor",
			"Repair_Metal",
			"Repair_Metal_Good",
			"Repair_Metal_Poor",
			"Repair_Stone",
			"Repair_Stone_Good",
			"Repair_Stone_Poor",
			"Repair_Ceramic",
			"Repair_Ceramic_Good",
			"Repair_Ceramic_Poor",
			"Repair_Hard_Organic",
			"Repair_Hard_Organic_Good",
			"Repair_Hard_Organic_Poor"
		})
		{
			Assert.AreEqual(1, context.GameItemComponentProtos.Count(x => x.Name == name), $"Expected one component named {name}.");
			Assert.AreEqual("RepairKit", context.GameItemComponentProtos.Single(x => x.Name == name).Type);
		}

		AssertRepairKitDefinition(context, "Repair_Wood", "Carpentry", WoundSeverity.Grievous, 1000.0, 0.0, "oak");
		AssertRepairKitDefinition(context, "Repair_Wood_Good", "Carpentry", WoundSeverity.Horrifying, 1500.0, 1.0, "oak");
		AssertRepairKitDefinition(context, "Repair_Wood_Poor", "Carpentry", WoundSeverity.Severe, 600.0, -1.0, "oak");

		AssertRepairKitDefinition(context, "Repair_Metal", "Blacksmithing", WoundSeverity.Grievous, 1000.0, 0.0, "bronze");
		AssertRepairKitDefinition(context, "Repair_Metal_Good", "Blacksmithing", WoundSeverity.Horrifying, 1500.0, 1.0, "bronze");
		AssertRepairKitDefinition(context, "Repair_Metal_Poor", "Blacksmithing", WoundSeverity.Severe, 600.0, -1.0, "bronze");

		AssertRepairKitDefinition(context, "Repair_Stone", "Masonry", WoundSeverity.Grievous, 1000.0, 0.0, "limestone");
		AssertRepairKitDefinition(context, "Repair_Stone_Good", "Masonry", WoundSeverity.Horrifying, 1500.0, 1.0, "limestone");
		AssertRepairKitDefinition(context, "Repair_Stone_Poor", "Masonry", WoundSeverity.Severe, 600.0, -1.0, "limestone");

		AssertRepairKitDefinition(context, "Repair_Ceramic", "Pottery", WoundSeverity.Grievous, 1000.0, 0.0, "earthenware");
		AssertRepairKitDefinition(context, "Repair_Ceramic_Good", "Pottery", WoundSeverity.Horrifying, 1500.0, 1.0, "earthenware");
		AssertRepairKitDefinition(context, "Repair_Ceramic_Poor", "Pottery", WoundSeverity.Severe, 600.0, -1.0, "earthenware");

		AssertRepairKitDefinition(context, "Repair_Hard_Organic", "Scrimshawing", WoundSeverity.Grievous, 1000.0, 0.0,
			"beak", "bone", "claw", "horn", "scale", "shell", "tooth");
		AssertRepairKitDefinition(context, "Repair_Hard_Organic_Good", "Scrimshawing", WoundSeverity.Horrifying, 1500.0, 1.0,
			"beak", "bone", "claw", "horn", "scale", "shell", "tooth");
		AssertRepairKitDefinition(context, "Repair_Hard_Organic_Poor", "Scrimshawing", WoundSeverity.Severe, 600.0, -1.0,
			"beak", "bone", "claw", "horn", "scale", "shell", "tooth");
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
		Assert.AreEqual(-5.0, ModifierFor(stealthPenalty, 101));
		Assert.AreEqual(-5.0, ModifierFor(stealthPenalty, 102));

		Assert.AreEqual(-2.5, ModifierFor(Component("WornTraitChanger_SwimPenalty_Minor"), 103));
		Assert.AreEqual(-10.0, ModifierFor(Component("WornTraitChanger_ClimbPenalty_Severe"), 104));
		Assert.AreEqual(-7.5, ModifierFor(Component("WornTraitChanger_FlyPenalty_Major"), 105));
		Assert.AreEqual(7.5, ModifierFor(Component("WornTraitChanger_RunningBonus_Major"), 106));
		Assert.AreEqual(-5.0, ModifierFor(Component("WornTraitChanger_ManualDexterityPenalty_Moderate"), 107));
		Assert.AreEqual(-5.0, ModifierFor(Component("WornTraitChanger_ManualDexterityPenalty_Moderate"), 108));
		Assert.AreEqual(-10.0, ModifierFor(Component("WornTraitChanger_VisualPerceptionPenalty_Severe"), 112));
		Assert.AreEqual(-2.5, ModifierFor(Component("WornTraitChanger_ListeningPenalty_Minor"), 115));
		Assert.AreEqual(-7.5, ModifierFor(Component("WornTraitChanger_AwarenessPenalty_Major"), 115));
		Assert.AreEqual(5.0, ModifierFor(Component("WornTraitChanger_StealthMobilityBonus_Moderate"), 106));

		GameItemComponentProto Component(string name) => context.GameItemComponentProtos.Single(x => x.Name == name);
	}

	private static void AssertRepairKitDefinition(FuturemudDatabaseContext context, string componentName, string traitName,
		WoundSeverity maximumSeverity, double repairPoints, double checkBonus, params string[] expectedMaterials)
	{
		var component = context.GameItemComponentProtos.Single(x => x.Name == componentName);
		XElement definition = XElement.Parse(component.Definition);
		Assert.AreEqual((int)maximumSeverity, (int)definition.Element("MaximumSeverity")!);
		Assert.AreEqual(repairPoints, (double)definition.Element("RepairPoints")!);
		Assert.AreEqual(context.TraitDefinitions.Single(x => x.Name == traitName).Id, (long)definition.Element("CheckTrait")!);
		Assert.AreEqual(checkBonus, (double)definition.Element("CheckBonus")!);
		Assert.AreEqual(0, definition.Element("Tags")!.Elements("Tag").Count(), $"{componentName} should not require category tags.");

		var materialIds = definition.Element("Materials")!
		                            .Elements("Material")
		                            .Select(x => (long)x)
		                            .ToHashSet();
		var materialNames = context.Materials
		                           .AsEnumerable()
		                           .Where(x => materialIds.Contains(x.Id))
		                           .Select(x => x.Name)
		                           .OrderBy(x => x, StringComparer.OrdinalIgnoreCase)
		                           .ToArray();
		CollectionAssert.AreEqual(expectedMaterials.OrderBy(x => x, StringComparer.OrdinalIgnoreCase).ToArray(), materialNames);
	}

	private static double ModifierFor(GameItemComponentProto component, long traitId)
	{
		XElement modifier = XElement.Parse(component.Definition)
		                            .Elements("Modifier")
		                            .Single(x => (long)x.Attribute("trait")! == traitId);
		return (double)modifier.Attribute("bonus")!;
	}
}
