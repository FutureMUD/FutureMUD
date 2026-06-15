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

	private static void EnsureComponentMarkers(FuturemudDatabaseContext context, params string[] names)
	{
		var nextId = context.GameItemComponentProtos.Any() ? context.GameItemComponentProtos.Max(x => x.Id) + 1 : 1;
		foreach (var name in names)
		{
			if (context.GameItemComponentProtos.Any(x => x.Name == name))
			{
				continue;
			}

			context.GameItemComponentProtos.Add(CreateComponentMarker(nextId++, name));
		}

		context.SaveChanges();
	}

	private static GameItemProto LoadItem(FuturemudDatabaseContext context, string uniqueName)
	{
		return context.GameItemProtos
		              .Include(x => x.GameItemProtosGameItemComponentProtos)
		              .ThenInclude(x => x.GameItemComponent)
		              .Single(x => x.UniqueName == uniqueName);
	}

	private static string[] ComponentNames(GameItemProto item)
	{
		return item.GameItemProtosGameItemComponentProtos
		           .Select(x => x.GameItemComponent.Name)
		           .ToArray();
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
		Assert.IsTrue(UsefulSeeder.StockItemMarkersForTesting.Contains("Medieval_Parchment_Sheet_Surface"));
		Assert.IsTrue(UsefulSeeder.StockItemMarkersForTesting.Contains("Medieval_Wax_Tablet_Surface"));
		Assert.IsTrue(UsefulSeeder.StockItemMarkersForTesting.Contains("Medieval_Quill_Pen"));
		Assert.IsTrue(UsefulSeeder.StockItemMarkersForTesting.Contains("SealStamp_Medieval_BronzeSignet"));
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
	public void SeedLockAndWaterSourceCoverageForTesting_RerunDoesNotDuplicateAndCreatesPublicVariants()
	{
		using FuturemudDatabaseContext context = BuildContext();
		SeedGeneralPrerequisites(context);
		UsefulSeeder seeder = new();

		seeder.SeedLockAndWaterSourceCoverageForTesting(context);
		seeder.SeedLockAndWaterSourceCoverageForTesting(context);

		string[] expectedLatchNames =
		[
			"Latch_Container_Hook",
			"Latch_Container_Hasp",
			"Latch_Door_Bar",
			"Latch_Gate_DropBar",
			"Latch_Portcullis_Pawl"
		];

		string[] expectedWaterSourceNames =
		[
			"Infinite_PublicTapWaterSource",
			"Infinite_DrinkingFountainWaterSource",
			"Infinite_PublicPumpWaterSource",
			"Infinite_StandpipeWaterSource",
			"Infinite_PublicTroughWaterSource",
			"Infinite_PublicCisternWaterSource"
		];

		foreach (string name in expectedLatchNames.Concat(expectedWaterSourceNames))
		{
			Assert.AreEqual(1, context.GameItemComponentProtos.Count(x => x.Name == name), $"Expected a single component named {name}.");
		}

		XElement Definition(string name) => XElement.Parse(context.GameItemComponentProtos.Single(x => x.Name == name).Definition);

		GameItemComponentProto doorBar = context.GameItemComponentProtos.Single(x => x.Name == "Latch_Door_Bar");
		Assert.AreEqual("Latch", doorBar.Type);
		Assert.AreEqual((int)Difficulty.VeryHard, (int)Definition("Latch_Door_Bar").Element("ForceDifficulty")!);
		Assert.AreEqual((int)Difficulty.Hard, (int)Definition("Latch_Door_Bar").Element("PickDifficulty")!);
		StringAssert.Contains(Definition("Latch_Portcullis_Pawl").Element("LockEmote")!.Value, "portcullis");

		GameItemComponentProto publicTap = context.GameItemComponentProtos.Single(x => x.Name == "Infinite_PublicTapWaterSource");
		Assert.AreEqual("WaterSource", publicTap.Type);
		Assert.AreEqual(1000000.0, (double)Definition("Infinite_PublicTapWaterSource").Attribute("LiquidCapacity")!);
		Assert.AreEqual(0.8333333333333334, (double)Definition("Infinite_PublicTapWaterSource").Attribute("RefillRate")!);
		Assert.IsTrue(bool.Parse((string)Definition("Infinite_PublicTapWaterSource").Attribute("UseOnOffForRefill")!));
		Assert.AreEqual(500.0, (double)Definition("Infinite_PublicTroughWaterSource").Attribute("LiquidCapacity")!);
		Assert.IsFalse(bool.Parse((string)Definition("Infinite_PublicTroughWaterSource").Attribute("UseOnOffForRefill")!));
		Assert.AreEqual(10000.0, (double)Definition("Infinite_PublicCisternWaterSource").Attribute("LiquidCapacity")!);
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
			"RidingGear_Saddle",
			"RidingGear_SaddlePad",
			"RidingGear_Bridle",
			"RidingGear_Reins",
			"RidingGear_Bit",
			"RidingGear_Stirrups",
			"RidingGear_PackSaddle",
			"RidingGear_BitlessBridle",
			"RidingGear_RidingHarness",
			"HitchGear_LeadRope",
			"HitchGear_Yoke",
			"HitchGear_Harness",
			"HitchGear_Chain",
			"HitchGear_Rope",
			"HitchGear_Traces",
			"HitchGear_TowBar",
			"Treatment_AntiInflammatory_Single",
			"Treatment_AntiInflammatory_Kit",
			"Destroyable_Shield",
			"TimePiece_PocketWatch",
			"TimePiece_WallClock"
		];

		foreach (string name in expectedNames)
		{
			Assert.AreEqual(1, context.GameItemComponentProtos.Count(x => x.Name == name), $"Expected a single general component named {name}.");
		}

		Assert.AreEqual(1, context.GameItemComponentProtos.Count(x => x.Name == "Cigarette"));
		GameItemComponentProto shieldDestroyable = context.GameItemComponentProtos.Single(x => x.Name == "Destroyable_Shield");
		Assert.AreEqual("Destroyable", shieldDestroyable.Type);
		StringAssert.Contains(shieldDestroyable.Definition, "12 * quality");
		Assert.AreEqual(1, context.FutureProgs.Count(x => x.FunctionName == "OnSmokeCigarette"));
		Assert.AreEqual(1, context.VariableDefinitions.Count(x => x.Property == "nicotineuntil"));
		Assert.AreEqual(1, context.VariableDefaults.Count(x => x.Property == "nicotineuntil"));
		Assert.AreEqual(0, context.GameItemComponentProtos.Count(x => x.Name.StartsWith("Food_")));

		XElement Definition(string name) => XElement.Parse(context.GameItemComponentProtos.Single(x => x.Name == name).Definition);
		Assert.AreEqual("RidingGear", context.GameItemComponentProtos.Single(x => x.Name == "RidingGear_Saddle").Type);
		Assert.AreEqual(RidingGearRole.Saddle,
			Enum.Parse<RidingGearRole>(Definition("RidingGear_Saddle").Element("Roles")!.Value));
		var bitlessRoles = Enum.Parse<RidingGearRole>(Definition("RidingGear_BitlessBridle").Element("Roles")!.Value);
		Assert.IsTrue(bitlessRoles.HasFlag(RidingGearRole.Bridle));
		Assert.IsTrue(bitlessRoles.HasFlag(RidingGearRole.Reins));
		Assert.IsTrue(bitlessRoles.HasFlag(RidingGearRole.BitlessControl));
		Assert.AreEqual(7.0, (double)Definition("RidingGear_BitlessBridle").Element("ControlBonus")!);

		Assert.AreEqual("HitchGear", context.GameItemComponentProtos.Single(x => x.Name == "HitchGear_Yoke").Type);
		Assert.AreEqual(HitchGearRole.Yoke,
			Enum.Parse<HitchGearRole>(Definition("HitchGear_Yoke").Element("Roles")!.Value));
		Assert.AreEqual(2, (int)Definition("HitchGear_Yoke").Element("MaximumUsers")!);
		Assert.AreEqual(2.5, (double)Definition("HitchGear_Yoke").Element("EffortMultiplier")!);
		var leadRopeRoles = Enum.Parse<HitchGearRole>(Definition("HitchGear_LeadRope").Element("Roles")!.Value);
		Assert.IsTrue(leadRopeRoles.HasFlag(HitchGearRole.LeadRope));
		Assert.IsTrue(leadRopeRoles.HasFlag(HitchGearRole.Rope));
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
			"SealStamp_Medieval_BronzeSignet",
			"SealStamp_Medieval_IronSealMatrix",
			"SealStamp_Medieval_BrassOfficeSeal",
			"SealStamp_Medieval_LeadSealMatrix",
			"Sealable_Document_Wax",
			"Sealable_Document_Clay",
			"Sealable_Envelope",
			"Sealable_Scroll",
			"Sealable_Container_Wax",
			"Sealable_Antiquity_Clay_Tablet_Edge",
			"Sealable_Antiquity_Clay_Bulla",
			"Sealable_Antiquity_Papyrus_Letter",
			"Sealable_Antiquity_Papyrus_Scroll",
			"Sealable_Antiquity_Papyrus_Packet",
			"Sealable_Antiquity_Wax_Tablet_Diptych",
			"Sealable_Antiquity_Linen_Document_Bundle",
			"Sealable_Antiquity_Archive_Jar_Cap",
			"Sealable_Medieval_Parchment_Charter",
			"Sealable_Medieval_Parchment_Roll",
			"Sealable_Medieval_Rag_Paper_Letter",
			"Sealable_Medieval_Official_Writ",
			"Sealable_Medieval_East_Asian_Scroll",
			"Sealable_Medieval_Palm_Leaf_Bundle",
			"Sealable_Medieval_Document_Pouch",
			"Sealable_Medieval_Archive_Box",
			"Sealable_Modern_Business_Envelope",
			"Sealable_Modern_Padded_Envelope",
			"Sealable_Modern_File_Folder",
			"Sealable_Modern_Security_Envelope",
			"Sealable_Modern_Evidence_Bag",
			"Sealable_Modern_Registered_Mail_Pouch",
			"Sealable_Modern_Courier_Tube",
			"Sealable_Modern_Diplomatic_Pouch",
			"Sealable_Modern_Archive_Box",
			"MeasuringInstrument_Antiquity_BalanceScale",
			"MeasuringInstrument_Antiquity_StandardWeights",
			"MeasuringInstrument_Antiquity_FalseWeights",
			"MeasuringInstrument_Antiquity_GrainMeasure",
			"MeasuringInstrument_Antiquity_OilCup",
			"MeasuringInstrument_Antiquity_WineCup",
			"MeasuringInstrument_Antiquity_TaxAssessorKit",
			"Medieval_Parchment_Sheet_Surface",
			"Medieval_Parchment_Bifolium_Surface",
			"Medieval_Parchment_Roll_Surface",
			"Medieval_Rag_Paper_Sheet_Surface",
			"Medieval_Rag_Paper_Letter_Surface",
			"Medieval_Rag_Paper_Scroll_Surface",
			"Medieval_Papyrus_Sheet_Surface",
			"Medieval_Papyrus_Scroll_Surface",
			"Medieval_East_Asian_Paper_Scroll_Surface",
			"Medieval_East_Asian_Paper_Sheet_Surface",
			"Medieval_Palm_Leaf_Manuscript_Surface",
			"Medieval_Wax_Tablet_Surface",
			"Medieval_Wax_Diptych_Surface",
			"Medieval_Wax_Triptych_Surface",
			"Medieval_Wooden_Tablet_Surface",
			"Medieval_Slate_Tablet_Surface",
			"Medieval_Birch_Bark_Surface",
			"Medieval_Bamboo_Slip_Surface",
			"Medieval_Ostracon_Surface",
			"Medieval_Practice_Board_Surface",
			"Medieval_Quill_Pen",
			"Medieval_Fine_Quill_Pen",
			"Medieval_Reed_Pen",
			"Medieval_Qalam",
			"Medieval_Calligraphy_Brush",
			"Medieval_East_Asian_Writing_Brush",
			"Medieval_Charcoal_Stick",
			"Medieval_Bone_Stylus",
			"Medieval_Bronze_Stylus",
			"Medieval_Iron_Stylus",
			"Medieval_Reed_Stylus",
			"Medieval_Scribing_Chisel",
			"Medieval_Parchment_Codex_20_Page",
			"Medieval_Parchment_Codex_40_Page",
			"Medieval_Parchment_Codex_90_Page",
			"Medieval_Rag_Paper_Codex_40_Page",
			"Medieval_Account_Ledger_90_Page",
			"Medieval_East_Asian_Stitched_Book",
			"Medieval_Palm_Leaf_Manuscript_Bundle",
			"Container_Document_Pouch",
			"Container_Scroll_Tube",
			"Container_Seal_Box",
			"Container_Archive_Box",
			"Container_Document_Satchel",
			"Container_Document_Bookcase_Shelves",
			"Container_Writing_Desk_Surface",
			"Container_Writing_Desk_Drawers",
			"Container_Archive_Chest",
			"Container_Envelope",
			"PaperSheet_Envelope",
			"PaperSheet_Scroll",
			"IncenseBurner_Antiquity_BronzeCenser",
			"OfferingReceiver_Antiquity_HouseholdAltar",
			"OfferingReceiver_Antiquity_VotiveBasin",
			"OfferingReceiver_Antiquity_FuneralTray"
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
		Assert.AreEqual("a brass office seal bearing an institutional device",
			(string)Definition("SealStamp_Medieval_BrassOfficeSeal").Element("SealDesign")!);
		Assert.IsTrue(Definition("Sealable_Envelope").Element("AllowedMedia")!.Elements("Medium")
		                                      .Any(x => (string)x == "wax"));
		Assert.IsTrue(context.GameItemComponentProtos.Count(x => x.Type == "Sealable") >= 30);
		Assert.AreEqual((int)Difficulty.VeryHard,
			(int)Definition("Sealable_Antiquity_Clay_Bulla").Element("InspectionDifficulty")!);
		Assert.IsTrue(Definition("Sealable_Modern_Security_Envelope").Element("AllowedMedia")!.Elements("Medium")
		                                      .Any(x => (string)x == "security tape"));
		Assert.IsTrue(Definition("Sealable_Modern_Diplomatic_Pouch").Element("AllowedMedia")!.Elements("Medium")
		                                      .Any(x => (string)x == "numbered seal"));
		Assert.IsFalse(bool.Parse((string)Definition("Sealable_Modern_File_Folder").Element("BrokenSealLeavesResidue")!));
		Assert.AreEqual("Weight", (string)Definition("MeasuringInstrument_Antiquity_BalanceScale").Element("Mode")!);
		Assert.AreEqual("FluidVolume", (string)Definition("MeasuringInstrument_Antiquity_OilCup").Element("Mode")!);
		Assert.AreEqual(3600,
			(int)Definition("Medieval_Parchment_Sheet_Surface").Element("MaximumCharacterLengthOfText")!);
		CollectionAssert.AreEquivalent(new[] { "Stylus" },
			Definition("Medieval_Wax_Tablet_Surface").Element("AllowedImplementTypes")!.Elements("Type")
			                                      .Select(x => x.Value)
			                                      .ToArray());
		CollectionAssert.Contains(
			Definition("Medieval_Practice_Board_Surface").Element("AllowedImplementTypes")!.Elements("Type")
			                                            .Select(x => x.Value)
			                                            .ToArray(),
			"Chisel");
		Assert.AreEqual("Quill", (string)Definition("Medieval_Quill_Pen").Element("ImplementType")!);
		Assert.IsTrue((int)Definition("Medieval_Quill_Pen").Element("TotalUses")! > 0);
		Assert.AreEqual(0, (int)Definition("Medieval_Bone_Stylus").Element("TotalUses")!);
		Assert.AreEqual("in", (string)Definition("Container_Document_Pouch").Attribute("Preposition")!);
		Assert.IsTrue(bool.Parse((string)Definition("Container_Document_Pouch").Attribute("Closable")!));
		Assert.AreEqual((int)SizeCategory.Large,
			(int)Definition("Container_Archive_Chest").Attribute("MaxSize")!);
		var parchmentBookPageId = (long)Definition("Medieval_Parchment_Codex_90_Page").Element("PaperProto")!;
		var parchmentBookPage = context.GameItemProtos.Single(x => x.Id == parchmentBookPageId);
		Assert.AreEqual("a parchment codex leaf", parchmentBookPage.ShortDescription);
		Assert.AreNotEqual("a sheet of paper", parchmentBookPage.ShortDescription,
			"Medieval codex components should not borrow the modern A-size paper prototype.");
		Assert.AreEqual(1, (int)Definition("IncenseBurner_Antiquity_BronzeCenser").Element("ScentRange")!);
		Assert.AreEqual(4, (int)Definition("IncenseBurner_Antiquity_BronzeCenser").Element("ScentDifficulty")!);
		Assert.IsTrue(Definition("IncenseBurner_Antiquity_BronzeCenser").Element("SourceScentDescription")!.Value
			.Contains("resinous", StringComparison.OrdinalIgnoreCase));
		Assert.AreEqual("ManualBurn",
			(string)Definition("OfferingReceiver_Antiquity_HouseholdAltar").Element("ConsumptionMode")!);
		Assert.AreEqual("BurnOnOffer",
			(string)Definition("OfferingReceiver_Antiquity_VotiveBasin").Element("ConsumptionMode")!);

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
	public void ItemSeeder_AntiquityComponentGapItems_RerunDoesNotDuplicateAndUsesReportComponents()
	{
		using FuturemudDatabaseContext context = BuildContext();
		SeedGeneralPrerequisites(context);
		context.Tags.AddRange(
			new Tag { Id = 1, Name = "Functions" },
			new Tag { Id = 2, Name = "Tools", ParentId = 1 },
			new Tag { Id = 3, Name = "Scientific Tools", ParentId = 2 },
			new Tag { Id = 4, Name = "Measurement Tools", ParentId = 3 });
		context.SaveChanges();
		SeedMarketCategories(context);
		UsefulSeeder usefulSeeder = new();
		usefulSeeder.SeedAntiquityComponentGapCoverageForTesting(context);
		EnsureComponentMarkers(context,
			"Destroyable_Misc",
			"Destroyable_Furniture",
			"Destroyable_WoodenHeavy",
			"Destroyable_HeavyMetal",
			"Destroyable_Shield",
			"Container_Tray",
			"Container_Pouch",
			"Container_Small_Drum",
			"LContainer_DrinkingGlass",
			"LContainer_WineGlass",
			"LContainer_Amphora_Urna",
			"Wear_Ring",
			"Wear_Waist",
			"Keyring_Large",
			"LockingContainer_Lockbox",
			"Dice_d6");
		ItemSeeder itemSeeder = new();

		itemSeeder.SeedAntiquityComponentGapItemsForTesting(context);
		itemSeeder.SeedAntiquityComponentGapItemsForTesting(context);

		var expectedStableReferences = ItemSeeder.AntiquityComponentGapItemStableReferencesForTesting;
		foreach (var stableReference in expectedStableReferences)
		{
			Assert.AreEqual(1, context.GameItemProtos.Count(x => x.UniqueName == stableReference),
				$"Expected one item prototype named {stableReference}.");
		}

		GameItemProto bronzeSignet = LoadItem(context, "antiquity_bronze_signet_ring");
		CollectionAssert.Contains(ComponentNames(bronzeSignet), "SealStamp_Antiquity_BronzeSignet");
		CollectionAssert.Contains(ComponentNames(bronzeSignet), "Wear_Ring");

		GameItemProto papyrusScroll = LoadItem(context, "antiquity_sealed_papyrus_scroll");
		CollectionAssert.Contains(ComponentNames(papyrusScroll), "Antiquity_Papyrus_Scroll_Surface");
		CollectionAssert.Contains(ComponentNames(papyrusScroll), "Sealable_Scroll");

		GameItemProto sealBox = LoadItem(context, "antiquity_tax_office_seal_box");
		CollectionAssert.Contains(ComponentNames(sealBox), "LockingContainer_Lockbox");
		CollectionAssert.Contains(ComponentNames(sealBox), "Sealable_Container_Wax");

		GameItemProto oilCup = LoadItem(context, "antiquity_oil_measure_cup");
		CollectionAssert.Contains(ComponentNames(oilCup), "LContainer_DrinkingGlass");
		CollectionAssert.Contains(ComponentNames(oilCup), "MeasuringInstrument_Antiquity_OilCup");
		Assert.AreEqual(1, context.Tags.Count(x => x.Name == "Measurement Tools"),
			"The item seeder should reuse the existing measurement tag instead of creating a shorter duplicate path.");
		var measurementTag = context.Tags.Single(x => x.Name == "Measurement Tools");
		CollectionAssert.Contains(
			context.GameItemProtosTags
			       .Where(x => x.GameItemProtoId == oilCup.Id)
			       .Select(x => x.TagId)
			       .ToArray(),
			measurementTag.Id);
		Assert.IsFalse(context.Tags
		                      .AsEnumerable()
		                      .GroupBy(x => x.Name, StringComparer.OrdinalIgnoreCase)
		                      .Any(x => x.Count() > 1),
			"Seeded item tag creation should not create duplicate tag names.");

		GameItemProto publicWell = LoadItem(context, "antiquity_stone_public_well");
		CollectionAssert.Contains(ComponentNames(publicWell), "WaterSource_Antiquity_PublicWell");

		GameItemProto measuringRod = LoadItem(context, "antiquity_wooden_measuring_rod");
		CollectionAssert.DoesNotContain(ComponentNames(measuringRod), "MeasuringInstrument_Antiquity_BalanceScale",
			"Length measurement is deferred, so the rod should remain a non-measuring prop.");

		GameItemProto censer = LoadItem(context, "antiquity_bronze_incense_censer");
		CollectionAssert.Contains(ComponentNames(censer), "IncenseBurner_Antiquity_BronzeCenser");

		GameItemProto incense = LoadItem(context, "antiquity_resin_incense_pellets");
		CollectionAssert.Contains(ComponentNames(incense), "Holdable");
		Assert.IsTrue(incense.GameItemProtosTags
		                     .Select(x => context.Tags.Single(tag => tag.Id == x.TagId).Name)
		                     .Contains("Incense Fuel"));

		GameItemProto altar = LoadItem(context, "antiquity_household_altar");
		CollectionAssert.Contains(ComponentNames(altar), "OfferingReceiver_Antiquity_HouseholdAltar");

		GameItemProto votiveBasin = LoadItem(context, "antiquity_votive_offering_basin");
		CollectionAssert.Contains(ComponentNames(votiveBasin), "OfferingReceiver_Antiquity_VotiveBasin");

		GameItemProto funeralTray = LoadItem(context, "antiquity_funeral_offering_tray");
		CollectionAssert.Contains(ComponentNames(funeralTray), "OfferingReceiver_Antiquity_FuneralTray");
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
