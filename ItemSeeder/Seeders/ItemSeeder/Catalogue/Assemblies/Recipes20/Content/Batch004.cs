#nullable enable

using MudSharp.Body;
using MudSharp.GameItems.Inventory.Plans;
using MudSharp.RPG.Checks;

namespace DatabaseSeeder.Seeders;

public static partial class Recipes20Stock
{
	public static CatalogueRecipeDefinition HouseholdFurnitureAndStorageMakeLinedMildSteelBedsteadForWorkshopFurnishingCraftIndustrialHouseholdFurnitureStorage0001LinedBedstead() => new()
	{
		Metadata = new("Household furniture and storage/make lined mild steel bedstead for workshop furnishing [craft_industrial_household_furniture_storage_0001_lined_bedstead]", "crafts", ["industrial"], ["item:industrial_household_furniture_storage_0001_lined_bedstead"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make lined mild steel bedstead for workshop furnishing [craft_industrial_household_furniture_storage_0001_lined_bedstead]",
		Category = "Household furniture and storage",
		Blurb = "Make a lined mild steel bedstead for workshop furnishing from prepared mild steel stock.",
		Action = "making lined mild steel bedstead for workshop furnishing",
		ActiveCraftItemSdesc = "an in-progress lined mild steel bedstead for workshop furnishing craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 429.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("429 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrial_household_furniture_storage_0001_lined_bedstead", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a lined mild steel bedstead for workshop furnishing (@industrial_household_furniture_storage_0001_lined_bedstead)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition HouseholdFurnitureAndStorageMakeLinedCastIronWashstandForWorkshopFurnishingCraftIndustrialHouseholdFurnitureStorage0002LinedWashstand() => new()
	{
		Metadata = new("Household furniture and storage/make lined cast iron washstand for workshop furnishing [craft_industrial_household_furniture_storage_0002_lined_washstand]", "crafts", ["industrial"], ["item:industrial_household_furniture_storage_0002_lined_washstand"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make lined cast iron washstand for workshop furnishing [craft_industrial_household_furniture_storage_0002_lined_washstand]",
		Category = "Household furniture and storage",
		Blurb = "Make a lined cast iron washstand for workshop furnishing from prepared cast iron stock.",
		Action = "making lined cast iron washstand for workshop furnishing",
		ActiveCraftItemSdesc = "an in-progress lined cast iron washstand for workshop furnishing craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 470.0, Material: "cast iron", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("470 grams of cast iron", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrial_household_furniture_storage_0002_lined_washstand", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a lined cast iron washstand for workshop furnishing (@industrial_household_furniture_storage_0002_lined_washstand)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition HouseholdFurnitureAndStorageMakeLinedOakScreenForWorkshopFurnishingCraftIndustrialHouseholdFurnitureStorage0003LinedScreen() => new()
	{
		Metadata = new("Household furniture and storage/make lined oak screen for workshop furnishing [craft_industrial_household_furniture_storage_0003_lined_screen]", "crafts", ["industrial"], ["item:industrial_household_furniture_storage_0003_lined_screen"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make lined oak screen for workshop furnishing [craft_industrial_household_furniture_storage_0003_lined_screen]",
		Category = "Household furniture and storage",
		Blurb = "Make a lined oak screen for workshop furnishing from prepared oak stock.",
		Action = "making lined oak screen for workshop furnishing",
		ActiveCraftItemSdesc = "an in-progress lined oak screen for workshop furnishing craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 510.0, Material: "oak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("510 grams of oak", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrial_household_furniture_storage_0003_lined_screen", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a lined oak screen for workshop furnishing (@industrial_household_furniture_storage_0003_lined_screen)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition HouseholdFurnitureAndStorageMakeLinedMildSteelRackForWorkshopFurnishingCraftIndustrialHouseholdFurnitureStorage0004LinedRack() => new()
	{
		Metadata = new("Household furniture and storage/make lined mild steel rack for workshop furnishing [craft_industrial_household_furniture_storage_0004_lined_rack]", "crafts", ["industrial"], ["item:industrial_household_furniture_storage_0004_lined_rack"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make lined mild steel rack for workshop furnishing [craft_industrial_household_furniture_storage_0004_lined_rack]",
		Category = "Household furniture and storage",
		Blurb = "Make a lined mild steel rack for workshop furnishing from prepared mild steel stock.",
		Action = "making lined mild steel rack for workshop furnishing",
		ActiveCraftItemSdesc = "an in-progress lined mild steel rack for workshop furnishing craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 551.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("551 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrial_household_furniture_storage_0004_lined_rack", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a lined mild steel rack for workshop furnishing (@industrial_household_furniture_storage_0004_lined_rack)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition HouseholdFurnitureAndStorageMakeLinedCastIronChestForWorkshopFurnishingCraftIndustrialHouseholdFurnitureStorage0005LinedChest() => new()
	{
		Metadata = new("Household furniture and storage/make lined cast iron chest for workshop furnishing [craft_industrial_household_furniture_storage_0005_lined_chest]", "crafts", ["industrial"], ["item:industrial_household_furniture_storage_0005_lined_chest"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make lined cast iron chest for workshop furnishing [craft_industrial_household_furniture_storage_0005_lined_chest]",
		Category = "Household furniture and storage",
		Blurb = "Make a lined cast iron chest for workshop furnishing from prepared cast iron stock.",
		Action = "making lined cast iron chest for workshop furnishing",
		ActiveCraftItemSdesc = "an in-progress lined cast iron chest for workshop furnishing craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 592.0, Material: "cast iron", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("592 grams of cast iron", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrial_household_furniture_storage_0005_lined_chest", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a lined cast iron chest for workshop furnishing (@industrial_household_furniture_storage_0005_lined_chest)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition HouseholdFurnitureAndStorageMakeLinedOakBenchForWorkshopFurnishingCraftIndustrialHouseholdFurnitureStorage0006LinedBench() => new()
	{
		Metadata = new("Household furniture and storage/make lined oak bench for workshop furnishing [craft_industrial_household_furniture_storage_0006_lined_bench]", "crafts", ["industrial"], ["item:industrial_household_furniture_storage_0006_lined_bench"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make lined oak bench for workshop furnishing [craft_industrial_household_furniture_storage_0006_lined_bench]",
		Category = "Household furniture and storage",
		Blurb = "Make a lined oak bench for workshop furnishing from prepared oak stock.",
		Action = "making lined oak bench for workshop furnishing",
		ActiveCraftItemSdesc = "an in-progress lined oak bench for workshop furnishing craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 632.0, Material: "oak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("632 grams of oak", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrial_household_furniture_storage_0006_lined_bench", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a lined oak bench for workshop furnishing (@industrial_household_furniture_storage_0006_lined_bench)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition HouseholdFurnitureAndStorageMakeVentilatedMildSteelScrapForWorkshopFurnishingCraftIndustrialHouseholdFurnitureStorage0007VentilatedScrap() => new()
	{
		Metadata = new("Household furniture and storage/make ventilated mild steel scrap for workshop furnishing [craft_industrial_household_furniture_storage_0007_ventilated_scrap]", "crafts", ["industrial"], ["item:industrial_household_furniture_storage_0007_ventilated_scrap"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make ventilated mild steel scrap for workshop furnishing [craft_industrial_household_furniture_storage_0007_ventilated_scrap]",
		Category = "Household furniture and storage",
		Blurb = "Make a ventilated mild steel scrap for workshop furnishing from prepared mild steel stock.",
		Action = "making ventilated mild steel scrap for workshop furnishing",
		ActiveCraftItemSdesc = "an in-progress ventilated mild steel scrap for workshop furnishing craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 673.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("673 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrial_household_furniture_storage_0007_ventilated_scrap", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a ventilated mild steel scrap for workshop furnishing (@industrial_household_furniture_storage_0007_ventilated_scrap)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition HouseholdFurnitureAndStorageMakeVentilatedCastIronChairForWorkshopFurnishingCraftIndustrialHouseholdFurnitureStorage0008VentilatedChair() => new()
	{
		Metadata = new("Household furniture and storage/make ventilated cast iron chair for workshop furnishing [craft_industrial_household_furniture_storage_0008_ventilated_chair]", "crafts", ["industrial"], ["item:industrial_household_furniture_storage_0008_ventilated_chair"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make ventilated cast iron chair for workshop furnishing [craft_industrial_household_furniture_storage_0008_ventilated_chair]",
		Category = "Household furniture and storage",
		Blurb = "Make a ventilated cast iron chair for workshop furnishing from prepared cast iron stock.",
		Action = "making ventilated cast iron chair for workshop furnishing",
		ActiveCraftItemSdesc = "an in-progress ventilated cast iron chair for workshop furnishing craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 714.0, Material: "cast iron", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("714 grams of cast iron", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrial_household_furniture_storage_0008_ventilated_chair", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a ventilated cast iron chair for workshop furnishing (@industrial_household_furniture_storage_0008_ventilated_chair)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition HouseholdFurnitureAndStorageMakeVentilatedOakStoolForWorkshopFurnishingCraftIndustrialHouseholdFurnitureStorage0009VentilatedStool() => new()
	{
		Metadata = new("Household furniture and storage/make ventilated oak stool for workshop furnishing [craft_industrial_household_furniture_storage_0009_ventilated_stool]", "crafts", ["industrial"], ["item:industrial_household_furniture_storage_0009_ventilated_stool"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make ventilated oak stool for workshop furnishing [craft_industrial_household_furniture_storage_0009_ventilated_stool]",
		Category = "Household furniture and storage",
		Blurb = "Make a ventilated oak stool for workshop furnishing from prepared oak stock.",
		Action = "making ventilated oak stool for workshop furnishing",
		ActiveCraftItemSdesc = "an in-progress ventilated oak stool for workshop furnishing craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 755.0, Material: "oak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("755 grams of oak", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrial_household_furniture_storage_0009_ventilated_stool", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a ventilated oak stool for workshop furnishing (@industrial_household_furniture_storage_0009_ventilated_stool)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition KitchenAndAppliancesMakePlainCastIronScrapForDomesticCookeryCraftIndustrialisedKitchenAppliances0001PlainScrap() => new()
	{
		Metadata = new("Kitchen and appliances/make plain cast iron scrap for domestic cookery [craft_industrialised_kitchen_appliances_0001_plain_scrap]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_kitchen_appliances_0001_plain_scrap"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make plain cast iron scrap for domestic cookery [craft_industrialised_kitchen_appliances_0001_plain_scrap]",
		Category = "Kitchen and appliances",
		Blurb = "Make a plain cast iron scrap for domestic cookery from prepared cast iron stock.",
		Action = "making plain cast iron scrap for domestic cookery",
		ActiveCraftItemSdesc = "an in-progress plain cast iron scrap for domestic cookery craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 540.0, Material: "cast iron", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("540 grams of cast iron", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_kitchen_appliances_0001_plain_scrap", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a plain cast iron scrap for domestic cookery (@industrialised_kitchen_appliances_0001_plain_scrap)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition KitchenAndAppliancesMakePlainCopperKettleForDomesticCookeryCraftIndustrialisedKitchenAppliances0002PlainKettle() => new()
	{
		Metadata = new("Kitchen and appliances/make plain copper kettle for domestic cookery [craft_industrialised_kitchen_appliances_0002_plain_kettle]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_kitchen_appliances_0002_plain_kettle"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make plain copper kettle for domestic cookery [craft_industrialised_kitchen_appliances_0002_plain_kettle]",
		Category = "Kitchen and appliances",
		Blurb = "Make a plain copper kettle for domestic cookery from prepared copper stock.",
		Action = "making plain copper kettle for domestic cookery",
		ActiveCraftItemSdesc = "an in-progress plain copper kettle for domestic cookery craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 581.0, Material: "copper", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("581 grams of copper", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_kitchen_appliances_0002_plain_kettle", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a plain copper kettle for domestic cookery (@industrialised_kitchen_appliances_0002_plain_kettle)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition KitchenAndAppliancesMakePlainCeramicPanForDomesticCookeryCraftIndustrialisedKitchenAppliances0003PlainPan() => new()
	{
		Metadata = new("Kitchen and appliances/make plain ceramic pan for domestic cookery [craft_industrialised_kitchen_appliances_0003_plain_pan]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_kitchen_appliances_0003_plain_pan"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make plain ceramic pan for domestic cookery [craft_industrialised_kitchen_appliances_0003_plain_pan]",
		Category = "Kitchen and appliances",
		Blurb = "Make a plain ceramic pan for domestic cookery from prepared ceramic stock.",
		Action = "making plain ceramic pan for domestic cookery",
		ActiveCraftItemSdesc = "an in-progress plain ceramic pan for domestic cookery craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 622.0, Material: "ceramic", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("622 grams of ceramic", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_kitchen_appliances_0003_plain_pan", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a plain ceramic pan for domestic cookery (@industrialised_kitchen_appliances_0003_plain_pan)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition KitchenAndAppliancesMakePlainMildSteelPotForDomesticCookeryCraftIndustrialisedKitchenAppliances0004PlainPot() => new()
	{
		Metadata = new("Kitchen and appliances/make plain mild steel pot for domestic cookery [craft_industrialised_kitchen_appliances_0004_plain_pot]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_kitchen_appliances_0004_plain_pot"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make plain mild steel pot for domestic cookery [craft_industrialised_kitchen_appliances_0004_plain_pot]",
		Category = "Kitchen and appliances",
		Blurb = "Make a plain mild steel pot for domestic cookery from prepared mild steel stock.",
		Action = "making plain mild steel pot for domestic cookery",
		ActiveCraftItemSdesc = "an in-progress plain mild steel pot for domestic cookery craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 662.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("662 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_kitchen_appliances_0004_plain_pot", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a plain mild steel pot for domestic cookery (@industrialised_kitchen_appliances_0004_plain_pot)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition KitchenAndAppliancesMakePlainCastIronGraterForDomesticCookeryCraftIndustrialisedKitchenAppliances0005PlainGrater() => new()
	{
		Metadata = new("Kitchen and appliances/make plain cast iron grater for domestic cookery [craft_industrialised_kitchen_appliances_0005_plain_grater]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_kitchen_appliances_0005_plain_grater"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make plain cast iron grater for domestic cookery [craft_industrialised_kitchen_appliances_0005_plain_grater]",
		Category = "Kitchen and appliances",
		Blurb = "Make a plain cast iron grater for domestic cookery from prepared cast iron stock.",
		Action = "making plain cast iron grater for domestic cookery",
		ActiveCraftItemSdesc = "an in-progress plain cast iron grater for domestic cookery craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 703.0, Material: "cast iron", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("703 grams of cast iron", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_kitchen_appliances_0005_plain_grater", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a plain cast iron grater for domestic cookery (@industrialised_kitchen_appliances_0005_plain_grater)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition KitchenAndAppliancesMakePlainCopperWhiskForDomesticCookeryCraftIndustrialisedKitchenAppliances0006PlainWhisk() => new()
	{
		Metadata = new("Kitchen and appliances/make plain copper whisk for domestic cookery [craft_industrialised_kitchen_appliances_0006_plain_whisk]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_kitchen_appliances_0006_plain_whisk"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make plain copper whisk for domestic cookery [craft_industrialised_kitchen_appliances_0006_plain_whisk]",
		Category = "Kitchen and appliances",
		Blurb = "Make a plain copper whisk for domestic cookery from prepared copper stock.",
		Action = "making plain copper whisk for domestic cookery",
		ActiveCraftItemSdesc = "an in-progress plain copper whisk for domestic cookery craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 744.0, Material: "copper", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("744 grams of copper", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_kitchen_appliances_0006_plain_whisk", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a plain copper whisk for domestic cookery (@industrialised_kitchen_appliances_0006_plain_whisk)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition KitchenAndAppliancesMakePlainCeramicLadleForDomesticCookeryCraftIndustrialisedKitchenAppliances0007PlainLadle() => new()
	{
		Metadata = new("Kitchen and appliances/make plain ceramic ladle for domestic cookery [craft_industrialised_kitchen_appliances_0007_plain_ladle]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_kitchen_appliances_0007_plain_ladle"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make plain ceramic ladle for domestic cookery [craft_industrialised_kitchen_appliances_0007_plain_ladle]",
		Category = "Kitchen and appliances",
		Blurb = "Make a plain ceramic ladle for domestic cookery from prepared ceramic stock.",
		Action = "making plain ceramic ladle for domestic cookery",
		ActiveCraftItemSdesc = "an in-progress plain ceramic ladle for domestic cookery craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 784.0, Material: "ceramic", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("784 grams of ceramic", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_kitchen_appliances_0007_plain_ladle", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a plain ceramic ladle for domestic cookery (@industrialised_kitchen_appliances_0007_plain_ladle)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition KitchenAndAppliancesMakePlainMildSteelStoveForDomesticCookeryCraftIndustrialisedKitchenAppliances0008PlainStove() => new()
	{
		Metadata = new("Kitchen and appliances/make plain mild steel stove for domestic cookery [craft_industrialised_kitchen_appliances_0008_plain_stove]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_kitchen_appliances_0008_plain_stove"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make plain mild steel stove for domestic cookery [craft_industrialised_kitchen_appliances_0008_plain_stove]",
		Category = "Kitchen and appliances",
		Blurb = "Make a plain mild steel stove for domestic cookery from prepared mild steel stock.",
		Action = "making plain mild steel stove for domestic cookery",
		ActiveCraftItemSdesc = "an in-progress plain mild steel stove for domestic cookery craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 825.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("825 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_kitchen_appliances_0008_plain_stove", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a plain mild steel stove for domestic cookery (@industrialised_kitchen_appliances_0008_plain_stove)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition KitchenAndAppliancesMakePlainCastIronToasterForDomesticCookeryCraftIndustrialisedKitchenAppliances0009PlainToaster() => new()
	{
		Metadata = new("Kitchen and appliances/make plain cast iron toaster for domestic cookery [craft_industrialised_kitchen_appliances_0009_plain_toaster]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_kitchen_appliances_0009_plain_toaster"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make plain cast iron toaster for domestic cookery [craft_industrialised_kitchen_appliances_0009_plain_toaster]",
		Category = "Kitchen and appliances",
		Blurb = "Make a plain cast iron toaster for domestic cookery from prepared cast iron stock.",
		Action = "making plain cast iron toaster for domestic cookery",
		ActiveCraftItemSdesc = "an in-progress plain cast iron toaster for domestic cookery craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 866.0, Material: "cast iron", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("866 grams of cast iron", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_kitchen_appliances_0009_plain_toaster", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a plain cast iron toaster for domestic cookery (@industrialised_kitchen_appliances_0009_plain_toaster)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition KitchenAndAppliancesMakePlainCopperMixerForDomesticCookeryCraftIndustrialisedKitchenAppliances0010PlainMixer() => new()
	{
		Metadata = new("Kitchen and appliances/make plain copper mixer for domestic cookery [craft_industrialised_kitchen_appliances_0010_plain_mixer]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_kitchen_appliances_0010_plain_mixer"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make plain copper mixer for domestic cookery [craft_industrialised_kitchen_appliances_0010_plain_mixer]",
		Category = "Kitchen and appliances",
		Blurb = "Make a plain copper mixer for domestic cookery from prepared copper stock.",
		Action = "making plain copper mixer for domestic cookery",
		ActiveCraftItemSdesc = "an in-progress plain copper mixer for domestic cookery craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 906.0, Material: "copper", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("906 grams of copper", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_kitchen_appliances_0010_plain_mixer", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a plain copper mixer for domestic cookery (@industrialised_kitchen_appliances_0010_plain_mixer)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition KitchenAndAppliancesMakePlainCeramicPressForDomesticCookeryCraftIndustrialisedKitchenAppliances0011PlainPress() => new()
	{
		Metadata = new("Kitchen and appliances/make plain ceramic press for domestic cookery [craft_industrialised_kitchen_appliances_0011_plain_press]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_kitchen_appliances_0011_plain_press"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make plain ceramic press for domestic cookery [craft_industrialised_kitchen_appliances_0011_plain_press]",
		Category = "Kitchen and appliances",
		Blurb = "Make a plain ceramic press for domestic cookery from prepared ceramic stock.",
		Action = "making plain ceramic press for domestic cookery",
		ActiveCraftItemSdesc = "an in-progress plain ceramic press for domestic cookery craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 947.0, Material: "ceramic", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("947 grams of ceramic", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_kitchen_appliances_0011_plain_press", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a plain ceramic press for domestic cookery (@industrialised_kitchen_appliances_0011_plain_press)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition KitchenAndAppliancesMakePlainMildSteelStrainerForDomesticCookeryCraftIndustrialisedKitchenAppliances0012PlainStrainer() => new()
	{
		Metadata = new("Kitchen and appliances/make plain mild steel strainer for domestic cookery [craft_industrialised_kitchen_appliances_0012_plain_strainer]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_kitchen_appliances_0012_plain_strainer"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make plain mild steel strainer for domestic cookery [craft_industrialised_kitchen_appliances_0012_plain_strainer]",
		Category = "Kitchen and appliances",
		Blurb = "Make a plain mild steel strainer for domestic cookery from prepared mild steel stock.",
		Action = "making plain mild steel strainer for domestic cookery",
		ActiveCraftItemSdesc = "an in-progress plain mild steel strainer for domestic cookery craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 988.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("988 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_kitchen_appliances_0012_plain_strainer", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a plain mild steel strainer for domestic cookery (@industrialised_kitchen_appliances_0012_plain_strainer)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition KitchenAndAppliancesMakeReinforcedCastIronScrapForDomesticCookeryCraftIndustrialisedKitchenAppliances0013ReinforcedScrap() => new()
	{
		Metadata = new("Kitchen and appliances/make reinforced cast iron scrap for domestic cookery [craft_industrialised_kitchen_appliances_0013_reinforced_scrap]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_kitchen_appliances_0013_reinforced_scrap"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make reinforced cast iron scrap for domestic cookery [craft_industrialised_kitchen_appliances_0013_reinforced_scrap]",
		Category = "Kitchen and appliances",
		Blurb = "Make a reinforced cast iron scrap for domestic cookery from prepared cast iron stock.",
		Action = "making reinforced cast iron scrap for domestic cookery",
		ActiveCraftItemSdesc = "an in-progress reinforced cast iron scrap for domestic cookery craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1028.0, Material: "cast iron", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1028 grams of cast iron", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_kitchen_appliances_0013_reinforced_scrap", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a reinforced cast iron scrap for domestic cookery (@industrialised_kitchen_appliances_0013_reinforced_scrap)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition KitchenAndAppliancesMakeReinforcedCopperKettleForDomesticCookeryCraftIndustrialisedKitchenAppliances0014ReinforcedKettle() => new()
	{
		Metadata = new("Kitchen and appliances/make reinforced copper kettle for domestic cookery [craft_industrialised_kitchen_appliances_0014_reinforced_kettle]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_kitchen_appliances_0014_reinforced_kettle"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make reinforced copper kettle for domestic cookery [craft_industrialised_kitchen_appliances_0014_reinforced_kettle]",
		Category = "Kitchen and appliances",
		Blurb = "Make a reinforced copper kettle for domestic cookery from prepared copper stock.",
		Action = "making reinforced copper kettle for domestic cookery",
		ActiveCraftItemSdesc = "an in-progress reinforced copper kettle for domestic cookery craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1069.0, Material: "copper", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1069 grams of copper", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_kitchen_appliances_0014_reinforced_kettle", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a reinforced copper kettle for domestic cookery (@industrialised_kitchen_appliances_0014_reinforced_kettle)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition KitchenAndAppliancesMakeReinforcedCeramicPanForDomesticCookeryCraftIndustrialisedKitchenAppliances0015ReinforcedPan() => new()
	{
		Metadata = new("Kitchen and appliances/make reinforced ceramic pan for domestic cookery [craft_industrialised_kitchen_appliances_0015_reinforced_pan]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_kitchen_appliances_0015_reinforced_pan"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make reinforced ceramic pan for domestic cookery [craft_industrialised_kitchen_appliances_0015_reinforced_pan]",
		Category = "Kitchen and appliances",
		Blurb = "Make a reinforced ceramic pan for domestic cookery from prepared ceramic stock.",
		Action = "making reinforced ceramic pan for domestic cookery",
		ActiveCraftItemSdesc = "an in-progress reinforced ceramic pan for domestic cookery craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1110.0, Material: "ceramic", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1110 grams of ceramic", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_kitchen_appliances_0015_reinforced_pan", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a reinforced ceramic pan for domestic cookery (@industrialised_kitchen_appliances_0015_reinforced_pan)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition KitchenAndAppliancesMakeReinforcedMildSteelPotForDomesticCookeryCraftIndustrialisedKitchenAppliances0016ReinforcedPot() => new()
	{
		Metadata = new("Kitchen and appliances/make reinforced mild steel pot for domestic cookery [craft_industrialised_kitchen_appliances_0016_reinforced_pot]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_kitchen_appliances_0016_reinforced_pot"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make reinforced mild steel pot for domestic cookery [craft_industrialised_kitchen_appliances_0016_reinforced_pot]",
		Category = "Kitchen and appliances",
		Blurb = "Make a reinforced mild steel pot for domestic cookery from prepared mild steel stock.",
		Action = "making reinforced mild steel pot for domestic cookery",
		ActiveCraftItemSdesc = "an in-progress reinforced mild steel pot for domestic cookery craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1151.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1151 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_kitchen_appliances_0016_reinforced_pot", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a reinforced mild steel pot for domestic cookery (@industrialised_kitchen_appliances_0016_reinforced_pot)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition KitchenAndAppliancesMakeReinforcedCastIronGraterForDomesticCookeryCraftIndustrialisedKitchenAppliances0017ReinforcedGrater() => new()
	{
		Metadata = new("Kitchen and appliances/make reinforced cast iron grater for domestic cookery [craft_industrialised_kitchen_appliances_0017_reinforced_grater]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_kitchen_appliances_0017_reinforced_grater"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make reinforced cast iron grater for domestic cookery [craft_industrialised_kitchen_appliances_0017_reinforced_grater]",
		Category = "Kitchen and appliances",
		Blurb = "Make a reinforced cast iron grater for domestic cookery from prepared cast iron stock.",
		Action = "making reinforced cast iron grater for domestic cookery",
		ActiveCraftItemSdesc = "an in-progress reinforced cast iron grater for domestic cookery craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1191.0, Material: "cast iron", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1191 grams of cast iron", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_kitchen_appliances_0017_reinforced_grater", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a reinforced cast iron grater for domestic cookery (@industrialised_kitchen_appliances_0017_reinforced_grater)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition KitchenAndAppliancesMakeReinforcedCopperWhiskForDomesticCookeryCraftIndustrialisedKitchenAppliances0018ReinforcedWhisk() => new()
	{
		Metadata = new("Kitchen and appliances/make reinforced copper whisk for domestic cookery [craft_industrialised_kitchen_appliances_0018_reinforced_whisk]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_kitchen_appliances_0018_reinforced_whisk"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make reinforced copper whisk for domestic cookery [craft_industrialised_kitchen_appliances_0018_reinforced_whisk]",
		Category = "Kitchen and appliances",
		Blurb = "Make a reinforced copper whisk for domestic cookery from prepared copper stock.",
		Action = "making reinforced copper whisk for domestic cookery",
		ActiveCraftItemSdesc = "an in-progress reinforced copper whisk for domestic cookery craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1232.0, Material: "copper", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1232 grams of copper", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_kitchen_appliances_0018_reinforced_whisk", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a reinforced copper whisk for domestic cookery (@industrialised_kitchen_appliances_0018_reinforced_whisk)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition KitchenAndAppliancesMakeReinforcedCeramicLadleForDomesticCookeryCraftIndustrialisedKitchenAppliances0019ReinforcedLadle() => new()
	{
		Metadata = new("Kitchen and appliances/make reinforced ceramic ladle for domestic cookery [craft_industrialised_kitchen_appliances_0019_reinforced_ladle]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_kitchen_appliances_0019_reinforced_ladle"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make reinforced ceramic ladle for domestic cookery [craft_industrialised_kitchen_appliances_0019_reinforced_ladle]",
		Category = "Kitchen and appliances",
		Blurb = "Make a reinforced ceramic ladle for domestic cookery from prepared ceramic stock.",
		Action = "making reinforced ceramic ladle for domestic cookery",
		ActiveCraftItemSdesc = "an in-progress reinforced ceramic ladle for domestic cookery craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1273.0, Material: "ceramic", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1273 grams of ceramic", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_kitchen_appliances_0019_reinforced_ladle", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a reinforced ceramic ladle for domestic cookery (@industrialised_kitchen_appliances_0019_reinforced_ladle)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition KitchenAndAppliancesMakeReinforcedMildSteelStoveForDomesticCookeryCraftIndustrialisedKitchenAppliances0020ReinforcedStove() => new()
	{
		Metadata = new("Kitchen and appliances/make reinforced mild steel stove for domestic cookery [craft_industrialised_kitchen_appliances_0020_reinforced_stove]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_kitchen_appliances_0020_reinforced_stove"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make reinforced mild steel stove for domestic cookery [craft_industrialised_kitchen_appliances_0020_reinforced_stove]",
		Category = "Kitchen and appliances",
		Blurb = "Make a reinforced mild steel stove for domestic cookery from prepared mild steel stock.",
		Action = "making reinforced mild steel stove for domestic cookery",
		ActiveCraftItemSdesc = "an in-progress reinforced mild steel stove for domestic cookery craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1313.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1313 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_kitchen_appliances_0020_reinforced_stove", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a reinforced mild steel stove for domestic cookery (@industrialised_kitchen_appliances_0020_reinforced_stove)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition KitchenAndAppliancesMakeReinforcedCastIronToasterForDomesticCookeryCraftIndustrialisedKitchenAppliances0021ReinforcedToaster() => new()
	{
		Metadata = new("Kitchen and appliances/make reinforced cast iron toaster for domestic cookery [craft_industrialised_kitchen_appliances_0021_reinforced_toaster]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_kitchen_appliances_0021_reinforced_toaster"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make reinforced cast iron toaster for domestic cookery [craft_industrialised_kitchen_appliances_0021_reinforced_toaster]",
		Category = "Kitchen and appliances",
		Blurb = "Make a reinforced cast iron toaster for domestic cookery from prepared cast iron stock.",
		Action = "making reinforced cast iron toaster for domestic cookery",
		ActiveCraftItemSdesc = "an in-progress reinforced cast iron toaster for domestic cookery craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1354.0, Material: "cast iron", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1354 grams of cast iron", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_kitchen_appliances_0021_reinforced_toaster", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a reinforced cast iron toaster for domestic cookery (@industrialised_kitchen_appliances_0021_reinforced_toaster)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition KitchenAndAppliancesMakeReinforcedCopperMixerForDomesticCookeryCraftIndustrialisedKitchenAppliances0022ReinforcedMixer() => new()
	{
		Metadata = new("Kitchen and appliances/make reinforced copper mixer for domestic cookery [craft_industrialised_kitchen_appliances_0022_reinforced_mixer]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_kitchen_appliances_0022_reinforced_mixer"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make reinforced copper mixer for domestic cookery [craft_industrialised_kitchen_appliances_0022_reinforced_mixer]",
		Category = "Kitchen and appliances",
		Blurb = "Make a reinforced copper mixer for domestic cookery from prepared copper stock.",
		Action = "making reinforced copper mixer for domestic cookery",
		ActiveCraftItemSdesc = "an in-progress reinforced copper mixer for domestic cookery craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1395.0, Material: "copper", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1395 grams of copper", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_kitchen_appliances_0022_reinforced_mixer", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a reinforced copper mixer for domestic cookery (@industrialised_kitchen_appliances_0022_reinforced_mixer)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition KitchenAndAppliancesMakeReinforcedCeramicPressForDomesticCookeryCraftIndustrialisedKitchenAppliances0023ReinforcedPress() => new()
	{
		Metadata = new("Kitchen and appliances/make reinforced ceramic press for domestic cookery [craft_industrialised_kitchen_appliances_0023_reinforced_press]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_kitchen_appliances_0023_reinforced_press"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make reinforced ceramic press for domestic cookery [craft_industrialised_kitchen_appliances_0023_reinforced_press]",
		Category = "Kitchen and appliances",
		Blurb = "Make a reinforced ceramic press for domestic cookery from prepared ceramic stock.",
		Action = "making reinforced ceramic press for domestic cookery",
		ActiveCraftItemSdesc = "an in-progress reinforced ceramic press for domestic cookery craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1436.0, Material: "ceramic", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1436 grams of ceramic", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_kitchen_appliances_0023_reinforced_press", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a reinforced ceramic press for domestic cookery (@industrialised_kitchen_appliances_0023_reinforced_press)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition KitchenAndAppliancesMakeReinforcedMildSteelStrainerForDomesticCookeryCraftIndustrialisedKitchenAppliances0024ReinforcedStrainer() => new()
	{
		Metadata = new("Kitchen and appliances/make reinforced mild steel strainer for domestic cookery [craft_industrialised_kitchen_appliances_0024_reinforced_strainer]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_kitchen_appliances_0024_reinforced_strainer"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make reinforced mild steel strainer for domestic cookery [craft_industrialised_kitchen_appliances_0024_reinforced_strainer]",
		Category = "Kitchen and appliances",
		Blurb = "Make a reinforced mild steel strainer for domestic cookery from prepared mild steel stock.",
		Action = "making reinforced mild steel strainer for domestic cookery",
		ActiveCraftItemSdesc = "an in-progress reinforced mild steel strainer for domestic cookery craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1476.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1476 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_kitchen_appliances_0024_reinforced_strainer", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a reinforced mild steel strainer for domestic cookery (@industrialised_kitchen_appliances_0024_reinforced_strainer)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition KitchenAndAppliancesMakeCompactCastIronScrapForDomesticCookeryCraftIndustrialisedKitchenAppliances0025CompactScrap() => new()
	{
		Metadata = new("Kitchen and appliances/make compact cast iron scrap for domestic cookery [craft_industrialised_kitchen_appliances_0025_compact_scrap]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_kitchen_appliances_0025_compact_scrap"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make compact cast iron scrap for domestic cookery [craft_industrialised_kitchen_appliances_0025_compact_scrap]",
		Category = "Kitchen and appliances",
		Blurb = "Make a compact cast iron scrap for domestic cookery from prepared cast iron stock.",
		Action = "making compact cast iron scrap for domestic cookery",
		ActiveCraftItemSdesc = "an in-progress compact cast iron scrap for domestic cookery craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1517.0, Material: "cast iron", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1517 grams of cast iron", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_kitchen_appliances_0025_compact_scrap", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a compact cast iron scrap for domestic cookery (@industrialised_kitchen_appliances_0025_compact_scrap)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition KitchenAndAppliancesMakeCompactCopperKettleForDomesticCookeryCraftIndustrialisedKitchenAppliances0026CompactKettle() => new()
	{
		Metadata = new("Kitchen and appliances/make compact copper kettle for domestic cookery [craft_industrialised_kitchen_appliances_0026_compact_kettle]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_kitchen_appliances_0026_compact_kettle"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make compact copper kettle for domestic cookery [craft_industrialised_kitchen_appliances_0026_compact_kettle]",
		Category = "Kitchen and appliances",
		Blurb = "Make a compact copper kettle for domestic cookery from prepared copper stock.",
		Action = "making compact copper kettle for domestic cookery",
		ActiveCraftItemSdesc = "an in-progress compact copper kettle for domestic cookery craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1558.0, Material: "copper", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1558 grams of copper", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_kitchen_appliances_0026_compact_kettle", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a compact copper kettle for domestic cookery (@industrialised_kitchen_appliances_0026_compact_kettle)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition KitchenAndAppliancesMakeCompactCeramicPanForDomesticCookeryCraftIndustrialisedKitchenAppliances0027CompactPan() => new()
	{
		Metadata = new("Kitchen and appliances/make compact ceramic pan for domestic cookery [craft_industrialised_kitchen_appliances_0027_compact_pan]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_kitchen_appliances_0027_compact_pan"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make compact ceramic pan for domestic cookery [craft_industrialised_kitchen_appliances_0027_compact_pan]",
		Category = "Kitchen and appliances",
		Blurb = "Make a compact ceramic pan for domestic cookery from prepared ceramic stock.",
		Action = "making compact ceramic pan for domestic cookery",
		ActiveCraftItemSdesc = "an in-progress compact ceramic pan for domestic cookery craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1598.0, Material: "ceramic", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1598 grams of ceramic", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_kitchen_appliances_0027_compact_pan", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a compact ceramic pan for domestic cookery (@industrialised_kitchen_appliances_0027_compact_pan)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition KitchenAndAppliancesMakeCompactMildSteelPotForDomesticCookeryCraftIndustrialisedKitchenAppliances0028CompactPot() => new()
	{
		Metadata = new("Kitchen and appliances/make compact mild steel pot for domestic cookery [craft_industrialised_kitchen_appliances_0028_compact_pot]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_kitchen_appliances_0028_compact_pot"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make compact mild steel pot for domestic cookery [craft_industrialised_kitchen_appliances_0028_compact_pot]",
		Category = "Kitchen and appliances",
		Blurb = "Make a compact mild steel pot for domestic cookery from prepared mild steel stock.",
		Action = "making compact mild steel pot for domestic cookery",
		ActiveCraftItemSdesc = "an in-progress compact mild steel pot for domestic cookery craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1639.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1639 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_kitchen_appliances_0028_compact_pot", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a compact mild steel pot for domestic cookery (@industrialised_kitchen_appliances_0028_compact_pot)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition KitchenAndAppliancesMakeCompactCastIronGraterForDomesticCookeryCraftIndustrialisedKitchenAppliances0029CompactGrater() => new()
	{
		Metadata = new("Kitchen and appliances/make compact cast iron grater for domestic cookery [craft_industrialised_kitchen_appliances_0029_compact_grater]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_kitchen_appliances_0029_compact_grater"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make compact cast iron grater for domestic cookery [craft_industrialised_kitchen_appliances_0029_compact_grater]",
		Category = "Kitchen and appliances",
		Blurb = "Make a compact cast iron grater for domestic cookery from prepared cast iron stock.",
		Action = "making compact cast iron grater for domestic cookery",
		ActiveCraftItemSdesc = "an in-progress compact cast iron grater for domestic cookery craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1680.0, Material: "cast iron", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1680 grams of cast iron", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_kitchen_appliances_0029_compact_grater", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a compact cast iron grater for domestic cookery (@industrialised_kitchen_appliances_0029_compact_grater)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition KitchenAndAppliancesMakeCompactCopperWhiskForDomesticCookeryCraftIndustrialisedKitchenAppliances0030CompactWhisk() => new()
	{
		Metadata = new("Kitchen and appliances/make compact copper whisk for domestic cookery [craft_industrialised_kitchen_appliances_0030_compact_whisk]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_kitchen_appliances_0030_compact_whisk"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make compact copper whisk for domestic cookery [craft_industrialised_kitchen_appliances_0030_compact_whisk]",
		Category = "Kitchen and appliances",
		Blurb = "Make a compact copper whisk for domestic cookery from prepared copper stock.",
		Action = "making compact copper whisk for domestic cookery",
		ActiveCraftItemSdesc = "an in-progress compact copper whisk for domestic cookery craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1720.0, Material: "copper", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1720 grams of copper", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_kitchen_appliances_0030_compact_whisk", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a compact copper whisk for domestic cookery (@industrialised_kitchen_appliances_0030_compact_whisk)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition KitchenAndAppliancesMakeCompactCeramicLadleForDomesticCookeryCraftIndustrialisedKitchenAppliances0031CompactLadle() => new()
	{
		Metadata = new("Kitchen and appliances/make compact ceramic ladle for domestic cookery [craft_industrialised_kitchen_appliances_0031_compact_ladle]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_kitchen_appliances_0031_compact_ladle"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make compact ceramic ladle for domestic cookery [craft_industrialised_kitchen_appliances_0031_compact_ladle]",
		Category = "Kitchen and appliances",
		Blurb = "Make a compact ceramic ladle for domestic cookery from prepared ceramic stock.",
		Action = "making compact ceramic ladle for domestic cookery",
		ActiveCraftItemSdesc = "an in-progress compact ceramic ladle for domestic cookery craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1761.0, Material: "ceramic", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1761 grams of ceramic", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_kitchen_appliances_0031_compact_ladle", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a compact ceramic ladle for domestic cookery (@industrialised_kitchen_appliances_0031_compact_ladle)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition KitchenAndAppliancesMakeCompactMildSteelStoveForDomesticCookeryCraftIndustrialisedKitchenAppliances0032CompactStove() => new()
	{
		Metadata = new("Kitchen and appliances/make compact mild steel stove for domestic cookery [craft_industrialised_kitchen_appliances_0032_compact_stove]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_kitchen_appliances_0032_compact_stove"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make compact mild steel stove for domestic cookery [craft_industrialised_kitchen_appliances_0032_compact_stove]",
		Category = "Kitchen and appliances",
		Blurb = "Make a compact mild steel stove for domestic cookery from prepared mild steel stock.",
		Action = "making compact mild steel stove for domestic cookery",
		ActiveCraftItemSdesc = "an in-progress compact mild steel stove for domestic cookery craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1802.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1802 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_kitchen_appliances_0032_compact_stove", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a compact mild steel stove for domestic cookery (@industrialised_kitchen_appliances_0032_compact_stove)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition KitchenAndAppliancesMakeCompactCastIronToasterForDomesticCookeryCraftIndustrialisedKitchenAppliances0033CompactToaster() => new()
	{
		Metadata = new("Kitchen and appliances/make compact cast iron toaster for domestic cookery [craft_industrialised_kitchen_appliances_0033_compact_toaster]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_kitchen_appliances_0033_compact_toaster"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make compact cast iron toaster for domestic cookery [craft_industrialised_kitchen_appliances_0033_compact_toaster]",
		Category = "Kitchen and appliances",
		Blurb = "Make a compact cast iron toaster for domestic cookery from prepared cast iron stock.",
		Action = "making compact cast iron toaster for domestic cookery",
		ActiveCraftItemSdesc = "an in-progress compact cast iron toaster for domestic cookery craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1843.0, Material: "cast iron", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1843 grams of cast iron", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_kitchen_appliances_0033_compact_toaster", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a compact cast iron toaster for domestic cookery (@industrialised_kitchen_appliances_0033_compact_toaster)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition KitchenAndAppliancesMakeCompactCopperMixerForDomesticCookeryCraftIndustrialisedKitchenAppliances0034CompactMixer() => new()
	{
		Metadata = new("Kitchen and appliances/make compact copper mixer for domestic cookery [craft_industrialised_kitchen_appliances_0034_compact_mixer]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_kitchen_appliances_0034_compact_mixer"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make compact copper mixer for domestic cookery [craft_industrialised_kitchen_appliances_0034_compact_mixer]",
		Category = "Kitchen and appliances",
		Blurb = "Make a compact copper mixer for domestic cookery from prepared copper stock.",
		Action = "making compact copper mixer for domestic cookery",
		ActiveCraftItemSdesc = "an in-progress compact copper mixer for domestic cookery craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1883.0, Material: "copper", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1883 grams of copper", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_kitchen_appliances_0034_compact_mixer", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a compact copper mixer for domestic cookery (@industrialised_kitchen_appliances_0034_compact_mixer)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition KitchenAndAppliancesMakeCompactCeramicPressForDomesticCookeryCraftIndustrialisedKitchenAppliances0035CompactPress() => new()
	{
		Metadata = new("Kitchen and appliances/make compact ceramic press for domestic cookery [craft_industrialised_kitchen_appliances_0035_compact_press]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_kitchen_appliances_0035_compact_press"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make compact ceramic press for domestic cookery [craft_industrialised_kitchen_appliances_0035_compact_press]",
		Category = "Kitchen and appliances",
		Blurb = "Make a compact ceramic press for domestic cookery from prepared ceramic stock.",
		Action = "making compact ceramic press for domestic cookery",
		ActiveCraftItemSdesc = "an in-progress compact ceramic press for domestic cookery craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1924.0, Material: "ceramic", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1924 grams of ceramic", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_kitchen_appliances_0035_compact_press", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a compact ceramic press for domestic cookery (@industrialised_kitchen_appliances_0035_compact_press)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition KitchenAndAppliancesMakeCompactMildSteelStrainerForDomesticCookeryCraftIndustrialisedKitchenAppliances0036CompactStrainer() => new()
	{
		Metadata = new("Kitchen and appliances/make compact mild steel strainer for domestic cookery [craft_industrialised_kitchen_appliances_0036_compact_strainer]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_kitchen_appliances_0036_compact_strainer"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make compact mild steel strainer for domestic cookery [craft_industrialised_kitchen_appliances_0036_compact_strainer]",
		Category = "Kitchen and appliances",
		Blurb = "Make a compact mild steel strainer for domestic cookery from prepared mild steel stock.",
		Action = "making compact mild steel strainer for domestic cookery",
		ActiveCraftItemSdesc = "an in-progress compact mild steel strainer for domestic cookery craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1965.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1965 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_kitchen_appliances_0036_compact_strainer", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a compact mild steel strainer for domestic cookery (@industrialised_kitchen_appliances_0036_compact_strainer)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition KitchenAndAppliancesMakeHeavyDutyCastIronScrapForDomesticCookeryCraftIndustrialisedKitchenAppliances0037HeavyDutyScrap() => new()
	{
		Metadata = new("Kitchen and appliances/make heavy-duty cast iron scrap for domestic cookery [craft_industrialised_kitchen_appliances_0037_heavy_duty_scrap]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_kitchen_appliances_0037_heavy_duty_scrap"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make heavy-duty cast iron scrap for domestic cookery [craft_industrialised_kitchen_appliances_0037_heavy_duty_scrap]",
		Category = "Kitchen and appliances",
		Blurb = "Make a heavy-duty cast iron scrap for domestic cookery from prepared cast iron stock.",
		Action = "making heavy-duty cast iron scrap for domestic cookery",
		ActiveCraftItemSdesc = "an in-progress heavy-duty cast iron scrap for domestic cookery craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2005.0, Material: "cast iron", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2005 grams of cast iron", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_kitchen_appliances_0037_heavy_duty_scrap", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a heavy-duty cast iron scrap for domestic cookery (@industrialised_kitchen_appliances_0037_heavy_duty_scrap)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition KitchenAndAppliancesMakeHeavyDutyCopperKettleForDomesticCookeryCraftIndustrialisedKitchenAppliances0038HeavyDutyKettle() => new()
	{
		Metadata = new("Kitchen and appliances/make heavy-duty copper kettle for domestic cookery [craft_industrialised_kitchen_appliances_0038_heavy_duty_kettle]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_kitchen_appliances_0038_heavy_duty_kettle"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make heavy-duty copper kettle for domestic cookery [craft_industrialised_kitchen_appliances_0038_heavy_duty_kettle]",
		Category = "Kitchen and appliances",
		Blurb = "Make a heavy-duty copper kettle for domestic cookery from prepared copper stock.",
		Action = "making heavy-duty copper kettle for domestic cookery",
		ActiveCraftItemSdesc = "an in-progress heavy-duty copper kettle for domestic cookery craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2046.0, Material: "copper", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2046 grams of copper", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_kitchen_appliances_0038_heavy_duty_kettle", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a heavy-duty copper kettle for domestic cookery (@industrialised_kitchen_appliances_0038_heavy_duty_kettle)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition KitchenAndAppliancesMakeHeavyDutyCeramicPanForDomesticCookeryCraftIndustrialisedKitchenAppliances0039HeavyDutyPan() => new()
	{
		Metadata = new("Kitchen and appliances/make heavy-duty ceramic pan for domestic cookery [craft_industrialised_kitchen_appliances_0039_heavy_duty_pan]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_kitchen_appliances_0039_heavy_duty_pan"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make heavy-duty ceramic pan for domestic cookery [craft_industrialised_kitchen_appliances_0039_heavy_duty_pan]",
		Category = "Kitchen and appliances",
		Blurb = "Make a heavy-duty ceramic pan for domestic cookery from prepared ceramic stock.",
		Action = "making heavy-duty ceramic pan for domestic cookery",
		ActiveCraftItemSdesc = "an in-progress heavy-duty ceramic pan for domestic cookery craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2087.0, Material: "ceramic", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2087 grams of ceramic", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_kitchen_appliances_0039_heavy_duty_pan", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a heavy-duty ceramic pan for domestic cookery (@industrialised_kitchen_appliances_0039_heavy_duty_pan)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition KitchenAndAppliancesMakeHeavyDutyMildSteelPotForDomesticCookeryCraftIndustrialisedKitchenAppliances0040HeavyDutyPot() => new()
	{
		Metadata = new("Kitchen and appliances/make heavy-duty mild steel pot for domestic cookery [craft_industrialised_kitchen_appliances_0040_heavy_duty_pot]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_kitchen_appliances_0040_heavy_duty_pot"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make heavy-duty mild steel pot for domestic cookery [craft_industrialised_kitchen_appliances_0040_heavy_duty_pot]",
		Category = "Kitchen and appliances",
		Blurb = "Make a heavy-duty mild steel pot for domestic cookery from prepared mild steel stock.",
		Action = "making heavy-duty mild steel pot for domestic cookery",
		ActiveCraftItemSdesc = "an in-progress heavy-duty mild steel pot for domestic cookery craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2127.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2127 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_kitchen_appliances_0040_heavy_duty_pot", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a heavy-duty mild steel pot for domestic cookery (@industrialised_kitchen_appliances_0040_heavy_duty_pot)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition KitchenAndAppliancesMakeHeavyDutyCastIronGraterForDomesticCookeryCraftIndustrialisedKitchenAppliances0041HeavyDutyGrater() => new()
	{
		Metadata = new("Kitchen and appliances/make heavy-duty cast iron grater for domestic cookery [craft_industrialised_kitchen_appliances_0041_heavy_duty_grater]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_kitchen_appliances_0041_heavy_duty_grater"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make heavy-duty cast iron grater for domestic cookery [craft_industrialised_kitchen_appliances_0041_heavy_duty_grater]",
		Category = "Kitchen and appliances",
		Blurb = "Make a heavy-duty cast iron grater for domestic cookery from prepared cast iron stock.",
		Action = "making heavy-duty cast iron grater for domestic cookery",
		ActiveCraftItemSdesc = "an in-progress heavy-duty cast iron grater for domestic cookery craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2168.0, Material: "cast iron", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2168 grams of cast iron", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_kitchen_appliances_0041_heavy_duty_grater", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a heavy-duty cast iron grater for domestic cookery (@industrialised_kitchen_appliances_0041_heavy_duty_grater)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition KitchenAndAppliancesMakeHeavyDutyCopperWhiskForDomesticCookeryCraftIndustrialisedKitchenAppliances0042HeavyDutyWhisk() => new()
	{
		Metadata = new("Kitchen and appliances/make heavy-duty copper whisk for domestic cookery [craft_industrialised_kitchen_appliances_0042_heavy_duty_whisk]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_kitchen_appliances_0042_heavy_duty_whisk"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make heavy-duty copper whisk for domestic cookery [craft_industrialised_kitchen_appliances_0042_heavy_duty_whisk]",
		Category = "Kitchen and appliances",
		Blurb = "Make a heavy-duty copper whisk for domestic cookery from prepared copper stock.",
		Action = "making heavy-duty copper whisk for domestic cookery",
		ActiveCraftItemSdesc = "an in-progress heavy-duty copper whisk for domestic cookery craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2209.0, Material: "copper", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2209 grams of copper", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_kitchen_appliances_0042_heavy_duty_whisk", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a heavy-duty copper whisk for domestic cookery (@industrialised_kitchen_appliances_0042_heavy_duty_whisk)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition KitchenAndAppliancesMakeHeavyDutyCeramicLadleForDomesticCookeryCraftIndustrialisedKitchenAppliances0043HeavyDutyLadle() => new()
	{
		Metadata = new("Kitchen and appliances/make heavy-duty ceramic ladle for domestic cookery [craft_industrialised_kitchen_appliances_0043_heavy_duty_ladle]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_kitchen_appliances_0043_heavy_duty_ladle"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make heavy-duty ceramic ladle for domestic cookery [craft_industrialised_kitchen_appliances_0043_heavy_duty_ladle]",
		Category = "Kitchen and appliances",
		Blurb = "Make a heavy-duty ceramic ladle for domestic cookery from prepared ceramic stock.",
		Action = "making heavy-duty ceramic ladle for domestic cookery",
		ActiveCraftItemSdesc = "an in-progress heavy-duty ceramic ladle for domestic cookery craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2250.0, Material: "ceramic", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2250 grams of ceramic", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_kitchen_appliances_0043_heavy_duty_ladle", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a heavy-duty ceramic ladle for domestic cookery (@industrialised_kitchen_appliances_0043_heavy_duty_ladle)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition KitchenAndAppliancesMakeHeavyDutyMildSteelStoveForDomesticCookeryCraftIndustrialisedKitchenAppliances0044HeavyDutyStove() => new()
	{
		Metadata = new("Kitchen and appliances/make heavy-duty mild steel stove for domestic cookery [craft_industrialised_kitchen_appliances_0044_heavy_duty_stove]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_kitchen_appliances_0044_heavy_duty_stove"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make heavy-duty mild steel stove for domestic cookery [craft_industrialised_kitchen_appliances_0044_heavy_duty_stove]",
		Category = "Kitchen and appliances",
		Blurb = "Make a heavy-duty mild steel stove for domestic cookery from prepared mild steel stock.",
		Action = "making heavy-duty mild steel stove for domestic cookery",
		ActiveCraftItemSdesc = "an in-progress heavy-duty mild steel stove for domestic cookery craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2290.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2290 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_kitchen_appliances_0044_heavy_duty_stove", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a heavy-duty mild steel stove for domestic cookery (@industrialised_kitchen_appliances_0044_heavy_duty_stove)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition KitchenAndAppliancesMakeHeavyDutyCastIronToasterForDomesticCookeryCraftIndustrialisedKitchenAppliances0045HeavyDutyToaster() => new()
	{
		Metadata = new("Kitchen and appliances/make heavy-duty cast iron toaster for domestic cookery [craft_industrialised_kitchen_appliances_0045_heavy_duty_toaster]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_kitchen_appliances_0045_heavy_duty_toaster"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make heavy-duty cast iron toaster for domestic cookery [craft_industrialised_kitchen_appliances_0045_heavy_duty_toaster]",
		Category = "Kitchen and appliances",
		Blurb = "Make a heavy-duty cast iron toaster for domestic cookery from prepared cast iron stock.",
		Action = "making heavy-duty cast iron toaster for domestic cookery",
		ActiveCraftItemSdesc = "an in-progress heavy-duty cast iron toaster for domestic cookery craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2331.0, Material: "cast iron", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2331 grams of cast iron", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_kitchen_appliances_0045_heavy_duty_toaster", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a heavy-duty cast iron toaster for domestic cookery (@industrialised_kitchen_appliances_0045_heavy_duty_toaster)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition KitchenAndAppliancesMakeHeavyDutyCopperMixerForDomesticCookeryCraftIndustrialisedKitchenAppliances0046HeavyDutyMixer() => new()
	{
		Metadata = new("Kitchen and appliances/make heavy-duty copper mixer for domestic cookery [craft_industrialised_kitchen_appliances_0046_heavy_duty_mixer]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_kitchen_appliances_0046_heavy_duty_mixer"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make heavy-duty copper mixer for domestic cookery [craft_industrialised_kitchen_appliances_0046_heavy_duty_mixer]",
		Category = "Kitchen and appliances",
		Blurb = "Make a heavy-duty copper mixer for domestic cookery from prepared copper stock.",
		Action = "making heavy-duty copper mixer for domestic cookery",
		ActiveCraftItemSdesc = "an in-progress heavy-duty copper mixer for domestic cookery craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2372.0, Material: "copper", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2372 grams of copper", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_kitchen_appliances_0046_heavy_duty_mixer", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a heavy-duty copper mixer for domestic cookery (@industrialised_kitchen_appliances_0046_heavy_duty_mixer)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition KitchenAndAppliancesMakeHeavyDutyCeramicPressForDomesticCookeryCraftIndustrialisedKitchenAppliances0047HeavyDutyPress() => new()
	{
		Metadata = new("Kitchen and appliances/make heavy-duty ceramic press for domestic cookery [craft_industrialised_kitchen_appliances_0047_heavy_duty_press]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_kitchen_appliances_0047_heavy_duty_press"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make heavy-duty ceramic press for domestic cookery [craft_industrialised_kitchen_appliances_0047_heavy_duty_press]",
		Category = "Kitchen and appliances",
		Blurb = "Make a heavy-duty ceramic press for domestic cookery from prepared ceramic stock.",
		Action = "making heavy-duty ceramic press for domestic cookery",
		ActiveCraftItemSdesc = "an in-progress heavy-duty ceramic press for domestic cookery craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2412.0, Material: "ceramic", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2412 grams of ceramic", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_kitchen_appliances_0047_heavy_duty_press", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a heavy-duty ceramic press for domestic cookery (@industrialised_kitchen_appliances_0047_heavy_duty_press)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition KitchenAndAppliancesMakeHeavyDutyMildSteelStrainerForDomesticCookeryCraftIndustrialisedKitchenAppliances0048HeavyDutyStrainer() => new()
	{
		Metadata = new("Kitchen and appliances/make heavy-duty mild steel strainer for domestic cookery [craft_industrialised_kitchen_appliances_0048_heavy_duty_strainer]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_kitchen_appliances_0048_heavy_duty_strainer"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make heavy-duty mild steel strainer for domestic cookery [craft_industrialised_kitchen_appliances_0048_heavy_duty_strainer]",
		Category = "Kitchen and appliances",
		Blurb = "Make a heavy-duty mild steel strainer for domestic cookery from prepared mild steel stock.",
		Action = "making heavy-duty mild steel strainer for domestic cookery",
		ActiveCraftItemSdesc = "an in-progress heavy-duty mild steel strainer for domestic cookery craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2453.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2453 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_kitchen_appliances_0048_heavy_duty_strainer", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a heavy-duty mild steel strainer for domestic cookery (@industrialised_kitchen_appliances_0048_heavy_duty_strainer)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition KitchenAndAppliancesMakeFoldingCastIronScrapForDomesticCookeryCraftIndustrialisedKitchenAppliances0049FoldingScrap() => new()
	{
		Metadata = new("Kitchen and appliances/make folding cast iron scrap for domestic cookery [craft_industrialised_kitchen_appliances_0049_folding_scrap]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_kitchen_appliances_0049_folding_scrap"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make folding cast iron scrap for domestic cookery [craft_industrialised_kitchen_appliances_0049_folding_scrap]",
		Category = "Kitchen and appliances",
		Blurb = "Make a folding cast iron scrap for domestic cookery from prepared cast iron stock.",
		Action = "making folding cast iron scrap for domestic cookery",
		ActiveCraftItemSdesc = "an in-progress folding cast iron scrap for domestic cookery craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2494.0, Material: "cast iron", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2494 grams of cast iron", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_kitchen_appliances_0049_folding_scrap", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a folding cast iron scrap for domestic cookery (@industrialised_kitchen_appliances_0049_folding_scrap)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition KitchenAndAppliancesMakeFoldingCopperKettleForDomesticCookeryCraftIndustrialisedKitchenAppliances0050FoldingKettle() => new()
	{
		Metadata = new("Kitchen and appliances/make folding copper kettle for domestic cookery [craft_industrialised_kitchen_appliances_0050_folding_kettle]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_kitchen_appliances_0050_folding_kettle"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make folding copper kettle for domestic cookery [craft_industrialised_kitchen_appliances_0050_folding_kettle]",
		Category = "Kitchen and appliances",
		Blurb = "Make a folding copper kettle for domestic cookery from prepared copper stock.",
		Action = "making folding copper kettle for domestic cookery",
		ActiveCraftItemSdesc = "an in-progress folding copper kettle for domestic cookery craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2534.0, Material: "copper", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2534 grams of copper", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_kitchen_appliances_0050_folding_kettle", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a folding copper kettle for domestic cookery (@industrialised_kitchen_appliances_0050_folding_kettle)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition KitchenAndAppliancesMakeFoldingCeramicPanForDomesticCookeryCraftIndustrialisedKitchenAppliances0051FoldingPan() => new()
	{
		Metadata = new("Kitchen and appliances/make folding ceramic pan for domestic cookery [craft_industrialised_kitchen_appliances_0051_folding_pan]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_kitchen_appliances_0051_folding_pan"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make folding ceramic pan for domestic cookery [craft_industrialised_kitchen_appliances_0051_folding_pan]",
		Category = "Kitchen and appliances",
		Blurb = "Make a folding ceramic pan for domestic cookery from prepared ceramic stock.",
		Action = "making folding ceramic pan for domestic cookery",
		ActiveCraftItemSdesc = "an in-progress folding ceramic pan for domestic cookery craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2575.0, Material: "ceramic", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2575 grams of ceramic", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_kitchen_appliances_0051_folding_pan", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a folding ceramic pan for domestic cookery (@industrialised_kitchen_appliances_0051_folding_pan)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};
}
