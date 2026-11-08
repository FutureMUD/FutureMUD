#nullable enable

using MudSharp.Body;
using MudSharp.GameItems.Inventory.Plans;
using MudSharp.RPG.Checks;

namespace DatabaseSeeder.Seeders;

public static partial class Recipes20Stock
{
	public static CatalogueRecipeDefinition HouseholdFurnitureAndStorageMakeReinforcedMildSteelRackForDomesticFurnishingCraftIndustrialisedHouseholdFurnitureStorage0022ReinforcedRack() => new()
	{
		Metadata = new("Household furniture and storage/make reinforced mild steel rack for domestic furnishing [craft_industrialised_household_furniture_storage_0022_reinforced_rack]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_household_furniture_storage_0022_reinforced_rack"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make reinforced mild steel rack for domestic furnishing [craft_industrialised_household_furniture_storage_0022_reinforced_rack]",
		Category = "Household furniture and storage",
		Blurb = "Make a reinforced mild steel rack for domestic furnishing from prepared mild steel stock.",
		Action = "making reinforced mild steel rack for domestic furnishing",
		ActiveCraftItemSdesc = "an in-progress reinforced mild steel rack for domestic furnishing craft",
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
			new CatalogueCommodityRecipeInput(Grams: 1284.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1284 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_household_furniture_storage_0022_reinforced_rack", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a reinforced mild steel rack for domestic furnishing (@industrialised_household_furniture_storage_0022_reinforced_rack)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition HouseholdFurnitureAndStorageMakeReinforcedCastIronChestForDomesticFurnishingCraftIndustrialisedHouseholdFurnitureStorage0023ReinforcedChest() => new()
	{
		Metadata = new("Household furniture and storage/make reinforced cast iron chest for domestic furnishing [craft_industrialised_household_furniture_storage_0023_reinforced_chest]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_household_furniture_storage_0023_reinforced_chest"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make reinforced cast iron chest for domestic furnishing [craft_industrialised_household_furniture_storage_0023_reinforced_chest]",
		Category = "Household furniture and storage",
		Blurb = "Make a reinforced cast iron chest for domestic furnishing from prepared cast iron stock.",
		Action = "making reinforced cast iron chest for domestic furnishing",
		ActiveCraftItemSdesc = "an in-progress reinforced cast iron chest for domestic furnishing craft",
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
			new CatalogueCommodityRecipeInput(Grams: 1324.0, Material: "cast iron", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1324 grams of cast iron", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_household_furniture_storage_0023_reinforced_chest", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a reinforced cast iron chest for domestic furnishing (@industrialised_household_furniture_storage_0023_reinforced_chest)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition HouseholdFurnitureAndStorageMakeReinforcedOakBenchForDomesticFurnishingCraftIndustrialisedHouseholdFurnitureStorage0024ReinforcedBench() => new()
	{
		Metadata = new("Household furniture and storage/make reinforced oak bench for domestic furnishing [craft_industrialised_household_furniture_storage_0024_reinforced_bench]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_household_furniture_storage_0024_reinforced_bench"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make reinforced oak bench for domestic furnishing [craft_industrialised_household_furniture_storage_0024_reinforced_bench]",
		Category = "Household furniture and storage",
		Blurb = "Make a reinforced oak bench for domestic furnishing from prepared oak stock.",
		Action = "making reinforced oak bench for domestic furnishing",
		ActiveCraftItemSdesc = "an in-progress reinforced oak bench for domestic furnishing craft",
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
			new CatalogueCommodityRecipeInput(Grams: 1365.0, Material: "oak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1365 grams of oak", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_household_furniture_storage_0024_reinforced_bench", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a reinforced oak bench for domestic furnishing (@industrialised_household_furniture_storage_0024_reinforced_bench)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition HouseholdFurnitureAndStorageMakeCompactMildSteelScrapForDomesticFurnishingCraftIndustrialisedHouseholdFurnitureStorage0025CompactScrap() => new()
	{
		Metadata = new("Household furniture and storage/make compact mild steel scrap for domestic furnishing [craft_industrialised_household_furniture_storage_0025_compact_scrap]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_household_furniture_storage_0025_compact_scrap"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make compact mild steel scrap for domestic furnishing [craft_industrialised_household_furniture_storage_0025_compact_scrap]",
		Category = "Household furniture and storage",
		Blurb = "Make a compact mild steel scrap for domestic furnishing from prepared mild steel stock.",
		Action = "making compact mild steel scrap for domestic furnishing",
		ActiveCraftItemSdesc = "an in-progress compact mild steel scrap for domestic furnishing craft",
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
			new CatalogueCommodityRecipeInput(Grams: 1406.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1406 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_household_furniture_storage_0025_compact_scrap", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a compact mild steel scrap for domestic furnishing (@industrialised_household_furniture_storage_0025_compact_scrap)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition HouseholdFurnitureAndStorageMakeCompactCastIronChairForDomesticFurnishingCraftIndustrialisedHouseholdFurnitureStorage0026CompactChair() => new()
	{
		Metadata = new("Household furniture and storage/make compact cast iron chair for domestic furnishing [craft_industrialised_household_furniture_storage_0026_compact_chair]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_household_furniture_storage_0026_compact_chair"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make compact cast iron chair for domestic furnishing [craft_industrialised_household_furniture_storage_0026_compact_chair]",
		Category = "Household furniture and storage",
		Blurb = "Make a compact cast iron chair for domestic furnishing from prepared cast iron stock.",
		Action = "making compact cast iron chair for domestic furnishing",
		ActiveCraftItemSdesc = "an in-progress compact cast iron chair for domestic furnishing craft",
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
			new CatalogueCommodityRecipeInput(Grams: 1447.0, Material: "cast iron", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1447 grams of cast iron", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_household_furniture_storage_0026_compact_chair", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a compact cast iron chair for domestic furnishing (@industrialised_household_furniture_storage_0026_compact_chair)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition HouseholdFurnitureAndStorageMakeCompactOakStoolForDomesticFurnishingCraftIndustrialisedHouseholdFurnitureStorage0027CompactStool() => new()
	{
		Metadata = new("Household furniture and storage/make compact oak stool for domestic furnishing [craft_industrialised_household_furniture_storage_0027_compact_stool]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_household_furniture_storage_0027_compact_stool"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make compact oak stool for domestic furnishing [craft_industrialised_household_furniture_storage_0027_compact_stool]",
		Category = "Household furniture and storage",
		Blurb = "Make a compact oak stool for domestic furnishing from prepared oak stock.",
		Action = "making compact oak stool for domestic furnishing",
		ActiveCraftItemSdesc = "an in-progress compact oak stool for domestic furnishing craft",
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
			new CatalogueCommodityRecipeInput(Grams: 1487.0, Material: "oak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1487 grams of oak", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_household_furniture_storage_0027_compact_stool", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a compact oak stool for domestic furnishing (@industrialised_household_furniture_storage_0027_compact_stool)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition HouseholdFurnitureAndStorageMakeCompactMildSteelTableForDomesticFurnishingCraftIndustrialisedHouseholdFurnitureStorage0028CompactTable() => new()
	{
		Metadata = new("Household furniture and storage/make compact mild steel table for domestic furnishing [craft_industrialised_household_furniture_storage_0028_compact_table]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_household_furniture_storage_0028_compact_table"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make compact mild steel table for domestic furnishing [craft_industrialised_household_furniture_storage_0028_compact_table]",
		Category = "Household furniture and storage",
		Blurb = "Make a compact mild steel table for domestic furnishing from prepared mild steel stock.",
		Action = "making compact mild steel table for domestic furnishing",
		ActiveCraftItemSdesc = "an in-progress compact mild steel table for domestic furnishing craft",
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
			new CatalogueCommodityRecipeInput(Grams: 1528.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1528 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_household_furniture_storage_0028_compact_table", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a compact mild steel table for domestic furnishing (@industrialised_household_furniture_storage_0028_compact_table)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition HouseholdFurnitureAndStorageMakeCompactCastIronCabinetForDomesticFurnishingCraftIndustrialisedHouseholdFurnitureStorage0029CompactCabinet() => new()
	{
		Metadata = new("Household furniture and storage/make compact cast iron cabinet for domestic furnishing [craft_industrialised_household_furniture_storage_0029_compact_cabinet]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_household_furniture_storage_0029_compact_cabinet"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make compact cast iron cabinet for domestic furnishing [craft_industrialised_household_furniture_storage_0029_compact_cabinet]",
		Category = "Household furniture and storage",
		Blurb = "Make a compact cast iron cabinet for domestic furnishing from prepared cast iron stock.",
		Action = "making compact cast iron cabinet for domestic furnishing",
		ActiveCraftItemSdesc = "an in-progress compact cast iron cabinet for domestic furnishing craft",
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
			new CatalogueCommodityRecipeInput(Grams: 1569.0, Material: "cast iron", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1569 grams of cast iron", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_household_furniture_storage_0029_compact_cabinet", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a compact cast iron cabinet for domestic furnishing (@industrialised_household_furniture_storage_0029_compact_cabinet)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition HouseholdFurnitureAndStorageMakeCompactOakShelfForDomesticFurnishingCraftIndustrialisedHouseholdFurnitureStorage0030CompactShelf() => new()
	{
		Metadata = new("Household furniture and storage/make compact oak shelf for domestic furnishing [craft_industrialised_household_furniture_storage_0030_compact_shelf]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_household_furniture_storage_0030_compact_shelf"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make compact oak shelf for domestic furnishing [craft_industrialised_household_furniture_storage_0030_compact_shelf]",
		Category = "Household furniture and storage",
		Blurb = "Make a compact oak shelf for domestic furnishing from prepared oak stock.",
		Action = "making compact oak shelf for domestic furnishing",
		ActiveCraftItemSdesc = "an in-progress compact oak shelf for domestic furnishing craft",
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
			new CatalogueCommodityRecipeInput(Grams: 1609.0, Material: "oak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1609 grams of oak", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_household_furniture_storage_0030_compact_shelf", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a compact oak shelf for domestic furnishing (@industrialised_household_furniture_storage_0030_compact_shelf)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition HouseholdFurnitureAndStorageMakeCompactMildSteelBedsteadForDomesticFurnishingCraftIndustrialisedHouseholdFurnitureStorage0031CompactBedstead() => new()
	{
		Metadata = new("Household furniture and storage/make compact mild steel bedstead for domestic furnishing [craft_industrialised_household_furniture_storage_0031_compact_bedstead]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_household_furniture_storage_0031_compact_bedstead"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make compact mild steel bedstead for domestic furnishing [craft_industrialised_household_furniture_storage_0031_compact_bedstead]",
		Category = "Household furniture and storage",
		Blurb = "Make a compact mild steel bedstead for domestic furnishing from prepared mild steel stock.",
		Action = "making compact mild steel bedstead for domestic furnishing",
		ActiveCraftItemSdesc = "an in-progress compact mild steel bedstead for domestic furnishing craft",
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
			new CatalogueCommodityRecipeInput(Grams: 1650.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1650 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_household_furniture_storage_0031_compact_bedstead", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a compact mild steel bedstead for domestic furnishing (@industrialised_household_furniture_storage_0031_compact_bedstead)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition HouseholdFurnitureAndStorageMakeCompactCastIronWashstandForDomesticFurnishingCraftIndustrialisedHouseholdFurnitureStorage0032CompactWashstand() => new()
	{
		Metadata = new("Household furniture and storage/make compact cast iron washstand for domestic furnishing [craft_industrialised_household_furniture_storage_0032_compact_washstand]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_household_furniture_storage_0032_compact_washstand"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make compact cast iron washstand for domestic furnishing [craft_industrialised_household_furniture_storage_0032_compact_washstand]",
		Category = "Household furniture and storage",
		Blurb = "Make a compact cast iron washstand for domestic furnishing from prepared cast iron stock.",
		Action = "making compact cast iron washstand for domestic furnishing",
		ActiveCraftItemSdesc = "an in-progress compact cast iron washstand for domestic furnishing craft",
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
			new CatalogueCommodityRecipeInput(Grams: 1691.0, Material: "cast iron", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1691 grams of cast iron", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_household_furniture_storage_0032_compact_washstand", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a compact cast iron washstand for domestic furnishing (@industrialised_household_furniture_storage_0032_compact_washstand)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition HouseholdFurnitureAndStorageMakeCompactOakScreenForDomesticFurnishingCraftIndustrialisedHouseholdFurnitureStorage0033CompactScreen() => new()
	{
		Metadata = new("Household furniture and storage/make compact oak screen for domestic furnishing [craft_industrialised_household_furniture_storage_0033_compact_screen]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_household_furniture_storage_0033_compact_screen"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make compact oak screen for domestic furnishing [craft_industrialised_household_furniture_storage_0033_compact_screen]",
		Category = "Household furniture and storage",
		Blurb = "Make a compact oak screen for domestic furnishing from prepared oak stock.",
		Action = "making compact oak screen for domestic furnishing",
		ActiveCraftItemSdesc = "an in-progress compact oak screen for domestic furnishing craft",
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
			new CatalogueCommodityRecipeInput(Grams: 1731.0, Material: "oak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1731 grams of oak", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_household_furniture_storage_0033_compact_screen", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a compact oak screen for domestic furnishing (@industrialised_household_furniture_storage_0033_compact_screen)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition HouseholdFurnitureAndStorageMakeCompactMildSteelRackForDomesticFurnishingCraftIndustrialisedHouseholdFurnitureStorage0034CompactRack() => new()
	{
		Metadata = new("Household furniture and storage/make compact mild steel rack for domestic furnishing [craft_industrialised_household_furniture_storage_0034_compact_rack]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_household_furniture_storage_0034_compact_rack"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make compact mild steel rack for domestic furnishing [craft_industrialised_household_furniture_storage_0034_compact_rack]",
		Category = "Household furniture and storage",
		Blurb = "Make a compact mild steel rack for domestic furnishing from prepared mild steel stock.",
		Action = "making compact mild steel rack for domestic furnishing",
		ActiveCraftItemSdesc = "an in-progress compact mild steel rack for domestic furnishing craft",
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
			new CatalogueCommodityRecipeInput(Grams: 1772.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1772 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_household_furniture_storage_0034_compact_rack", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a compact mild steel rack for domestic furnishing (@industrialised_household_furniture_storage_0034_compact_rack)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition HouseholdFurnitureAndStorageMakeCompactCastIronChestForDomesticFurnishingCraftIndustrialisedHouseholdFurnitureStorage0035CompactChest() => new()
	{
		Metadata = new("Household furniture and storage/make compact cast iron chest for domestic furnishing [craft_industrialised_household_furniture_storage_0035_compact_chest]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_household_furniture_storage_0035_compact_chest"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make compact cast iron chest for domestic furnishing [craft_industrialised_household_furniture_storage_0035_compact_chest]",
		Category = "Household furniture and storage",
		Blurb = "Make a compact cast iron chest for domestic furnishing from prepared cast iron stock.",
		Action = "making compact cast iron chest for domestic furnishing",
		ActiveCraftItemSdesc = "an in-progress compact cast iron chest for domestic furnishing craft",
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
			new CatalogueCommodityRecipeInput(Grams: 1813.0, Material: "cast iron", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1813 grams of cast iron", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_household_furniture_storage_0035_compact_chest", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a compact cast iron chest for domestic furnishing (@industrialised_household_furniture_storage_0035_compact_chest)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition HouseholdFurnitureAndStorageMakeCompactOakBenchForDomesticFurnishingCraftIndustrialisedHouseholdFurnitureStorage0036CompactBench() => new()
	{
		Metadata = new("Household furniture and storage/make compact oak bench for domestic furnishing [craft_industrialised_household_furniture_storage_0036_compact_bench]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_household_furniture_storage_0036_compact_bench"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make compact oak bench for domestic furnishing [craft_industrialised_household_furniture_storage_0036_compact_bench]",
		Category = "Household furniture and storage",
		Blurb = "Make a compact oak bench for domestic furnishing from prepared oak stock.",
		Action = "making compact oak bench for domestic furnishing",
		ActiveCraftItemSdesc = "an in-progress compact oak bench for domestic furnishing craft",
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
			new CatalogueCommodityRecipeInput(Grams: 1854.0, Material: "oak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1854 grams of oak", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_household_furniture_storage_0036_compact_bench", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a compact oak bench for domestic furnishing (@industrialised_household_furniture_storage_0036_compact_bench)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition HouseholdFurnitureAndStorageMakeHeavyDutyMildSteelScrapForDomesticFurnishingCraftIndustrialisedHouseholdFurnitureStorage0037HeavyDutyScrap() => new()
	{
		Metadata = new("Household furniture and storage/make heavy-duty mild steel scrap for domestic furnishing [craft_industrialised_household_furniture_storage_0037_heavy_duty_scrap]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_household_furniture_storage_0037_heavy_duty_scrap"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make heavy-duty mild steel scrap for domestic furnishing [craft_industrialised_household_furniture_storage_0037_heavy_duty_scrap]",
		Category = "Household furniture and storage",
		Blurb = "Make a heavy-duty mild steel scrap for domestic furnishing from prepared mild steel stock.",
		Action = "making heavy-duty mild steel scrap for domestic furnishing",
		ActiveCraftItemSdesc = "an in-progress heavy-duty mild steel scrap for domestic furnishing craft",
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
			new CatalogueCommodityRecipeInput(Grams: 1894.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1894 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_household_furniture_storage_0037_heavy_duty_scrap", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a heavy-duty mild steel scrap for domestic furnishing (@industrialised_household_furniture_storage_0037_heavy_duty_scrap)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition HouseholdFurnitureAndStorageMakeHeavyDutyCastIronChairForDomesticFurnishingCraftIndustrialisedHouseholdFurnitureStorage0038HeavyDutyChair() => new()
	{
		Metadata = new("Household furniture and storage/make heavy-duty cast iron chair for domestic furnishing [craft_industrialised_household_furniture_storage_0038_heavy_duty_chair]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_household_furniture_storage_0038_heavy_duty_chair"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make heavy-duty cast iron chair for domestic furnishing [craft_industrialised_household_furniture_storage_0038_heavy_duty_chair]",
		Category = "Household furniture and storage",
		Blurb = "Make a heavy-duty cast iron chair for domestic furnishing from prepared cast iron stock.",
		Action = "making heavy-duty cast iron chair for domestic furnishing",
		ActiveCraftItemSdesc = "an in-progress heavy-duty cast iron chair for domestic furnishing craft",
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
			new CatalogueCommodityRecipeInput(Grams: 1935.0, Material: "cast iron", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1935 grams of cast iron", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_household_furniture_storage_0038_heavy_duty_chair", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a heavy-duty cast iron chair for domestic furnishing (@industrialised_household_furniture_storage_0038_heavy_duty_chair)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition HouseholdFurnitureAndStorageMakeHeavyDutyOakStoolForDomesticFurnishingCraftIndustrialisedHouseholdFurnitureStorage0039HeavyDutyStool() => new()
	{
		Metadata = new("Household furniture and storage/make heavy-duty oak stool for domestic furnishing [craft_industrialised_household_furniture_storage_0039_heavy_duty_stool]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_household_furniture_storage_0039_heavy_duty_stool"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make heavy-duty oak stool for domestic furnishing [craft_industrialised_household_furniture_storage_0039_heavy_duty_stool]",
		Category = "Household furniture and storage",
		Blurb = "Make a heavy-duty oak stool for domestic furnishing from prepared oak stock.",
		Action = "making heavy-duty oak stool for domestic furnishing",
		ActiveCraftItemSdesc = "an in-progress heavy-duty oak stool for domestic furnishing craft",
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
			new CatalogueCommodityRecipeInput(Grams: 1976.0, Material: "oak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1976 grams of oak", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_household_furniture_storage_0039_heavy_duty_stool", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a heavy-duty oak stool for domestic furnishing (@industrialised_household_furniture_storage_0039_heavy_duty_stool)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition HouseholdFurnitureAndStorageMakeHeavyDutyMildSteelTableForDomesticFurnishingCraftIndustrialisedHouseholdFurnitureStorage0040HeavyDutyTable() => new()
	{
		Metadata = new("Household furniture and storage/make heavy-duty mild steel table for domestic furnishing [craft_industrialised_household_furniture_storage_0040_heavy_duty_table]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_household_furniture_storage_0040_heavy_duty_table"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make heavy-duty mild steel table for domestic furnishing [craft_industrialised_household_furniture_storage_0040_heavy_duty_table]",
		Category = "Household furniture and storage",
		Blurb = "Make a heavy-duty mild steel table for domestic furnishing from prepared mild steel stock.",
		Action = "making heavy-duty mild steel table for domestic furnishing",
		ActiveCraftItemSdesc = "an in-progress heavy-duty mild steel table for domestic furnishing craft",
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
			new CatalogueCommodityRecipeInput(Grams: 2016.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2016 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_household_furniture_storage_0040_heavy_duty_table", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a heavy-duty mild steel table for domestic furnishing (@industrialised_household_furniture_storage_0040_heavy_duty_table)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition HouseholdFurnitureAndStorageMakeHeavyDutyCastIronCabinetForDomesticFurnishingCraftIndustrialisedHouseholdFurnitureStorage0041HeavyDutyCabinet() => new()
	{
		Metadata = new("Household furniture and storage/make heavy-duty cast iron cabinet for domestic furnishing [craft_industrialised_household_furniture_storage_0041_heavy_duty_cabinet]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_household_furniture_storage_0041_heavy_duty_cabinet"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make heavy-duty cast iron cabinet for domestic furnishing [craft_industrialised_household_furniture_storage_0041_heavy_duty_cabinet]",
		Category = "Household furniture and storage",
		Blurb = "Make a heavy-duty cast iron cabinet for domestic furnishing from prepared cast iron stock.",
		Action = "making heavy-duty cast iron cabinet for domestic furnishing",
		ActiveCraftItemSdesc = "an in-progress heavy-duty cast iron cabinet for domestic furnishing craft",
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
			new CatalogueCommodityRecipeInput(Grams: 2057.0, Material: "cast iron", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2057 grams of cast iron", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_household_furniture_storage_0041_heavy_duty_cabinet", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a heavy-duty cast iron cabinet for domestic furnishing (@industrialised_household_furniture_storage_0041_heavy_duty_cabinet)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition HouseholdFurnitureAndStorageMakeHeavyDutyOakShelfForDomesticFurnishingCraftIndustrialisedHouseholdFurnitureStorage0042HeavyDutyShelf() => new()
	{
		Metadata = new("Household furniture and storage/make heavy-duty oak shelf for domestic furnishing [craft_industrialised_household_furniture_storage_0042_heavy_duty_shelf]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_household_furniture_storage_0042_heavy_duty_shelf"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make heavy-duty oak shelf for domestic furnishing [craft_industrialised_household_furniture_storage_0042_heavy_duty_shelf]",
		Category = "Household furniture and storage",
		Blurb = "Make a heavy-duty oak shelf for domestic furnishing from prepared oak stock.",
		Action = "making heavy-duty oak shelf for domestic furnishing",
		ActiveCraftItemSdesc = "an in-progress heavy-duty oak shelf for domestic furnishing craft",
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
			new CatalogueCommodityRecipeInput(Grams: 2098.0, Material: "oak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2098 grams of oak", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_household_furniture_storage_0042_heavy_duty_shelf", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a heavy-duty oak shelf for domestic furnishing (@industrialised_household_furniture_storage_0042_heavy_duty_shelf)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition HouseholdFurnitureAndStorageMakeHeavyDutyMildSteelBedsteadForDomesticFurnishingCraftIndustrialisedHouseholdFurnitureStorage0043HeavyDutyBedstead() => new()
	{
		Metadata = new("Household furniture and storage/make heavy-duty mild steel bedstead for domestic furnishing [craft_industrialised_household_furniture_storage_0043_heavy_duty_bedstead]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_household_furniture_storage_0043_heavy_duty_bedstead"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make heavy-duty mild steel bedstead for domestic furnishing [craft_industrialised_household_furniture_storage_0043_heavy_duty_bedstead]",
		Category = "Household furniture and storage",
		Blurb = "Make a heavy-duty mild steel bedstead for domestic furnishing from prepared mild steel stock.",
		Action = "making heavy-duty mild steel bedstead for domestic furnishing",
		ActiveCraftItemSdesc = "an in-progress heavy-duty mild steel bedstead for domestic furnishing craft",
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
			new CatalogueCommodityRecipeInput(Grams: 2138.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2138 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_household_furniture_storage_0043_heavy_duty_bedstead", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a heavy-duty mild steel bedstead for domestic furnishing (@industrialised_household_furniture_storage_0043_heavy_duty_bedstead)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition HouseholdFurnitureAndStorageMakeHeavyDutyCastIronWashstandForDomesticFurnishingCraftIndustrialisedHouseholdFurnitureStorage0044HeavyDutyWashstand() => new()
	{
		Metadata = new("Household furniture and storage/make heavy-duty cast iron washstand for domestic furnishing [craft_industrialised_household_furniture_storage_0044_heavy_duty_washstand]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_household_furniture_storage_0044_heavy_duty_washstand"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make heavy-duty cast iron washstand for domestic furnishing [craft_industrialised_household_furniture_storage_0044_heavy_duty_washstand]",
		Category = "Household furniture and storage",
		Blurb = "Make a heavy-duty cast iron washstand for domestic furnishing from prepared cast iron stock.",
		Action = "making heavy-duty cast iron washstand for domestic furnishing",
		ActiveCraftItemSdesc = "an in-progress heavy-duty cast iron washstand for domestic furnishing craft",
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
			new CatalogueCommodityRecipeInput(Grams: 2179.0, Material: "cast iron", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2179 grams of cast iron", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_household_furniture_storage_0044_heavy_duty_washstand", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a heavy-duty cast iron washstand for domestic furnishing (@industrialised_household_furniture_storage_0044_heavy_duty_washstand)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition HouseholdFurnitureAndStorageMakeHeavyDutyOakScreenForDomesticFurnishingCraftIndustrialisedHouseholdFurnitureStorage0045HeavyDutyScreen() => new()
	{
		Metadata = new("Household furniture and storage/make heavy-duty oak screen for domestic furnishing [craft_industrialised_household_furniture_storage_0045_heavy_duty_screen]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_household_furniture_storage_0045_heavy_duty_screen"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make heavy-duty oak screen for domestic furnishing [craft_industrialised_household_furniture_storage_0045_heavy_duty_screen]",
		Category = "Household furniture and storage",
		Blurb = "Make a heavy-duty oak screen for domestic furnishing from prepared oak stock.",
		Action = "making heavy-duty oak screen for domestic furnishing",
		ActiveCraftItemSdesc = "an in-progress heavy-duty oak screen for domestic furnishing craft",
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
			new CatalogueCommodityRecipeInput(Grams: 2220.0, Material: "oak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2220 grams of oak", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_household_furniture_storage_0045_heavy_duty_screen", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a heavy-duty oak screen for domestic furnishing (@industrialised_household_furniture_storage_0045_heavy_duty_screen)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition HouseholdFurnitureAndStorageMakeHeavyDutyMildSteelRackForDomesticFurnishingCraftIndustrialisedHouseholdFurnitureStorage0046HeavyDutyRack() => new()
	{
		Metadata = new("Household furniture and storage/make heavy-duty mild steel rack for domestic furnishing [craft_industrialised_household_furniture_storage_0046_heavy_duty_rack]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_household_furniture_storage_0046_heavy_duty_rack"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make heavy-duty mild steel rack for domestic furnishing [craft_industrialised_household_furniture_storage_0046_heavy_duty_rack]",
		Category = "Household furniture and storage",
		Blurb = "Make a heavy-duty mild steel rack for domestic furnishing from prepared mild steel stock.",
		Action = "making heavy-duty mild steel rack for domestic furnishing",
		ActiveCraftItemSdesc = "an in-progress heavy-duty mild steel rack for domestic furnishing craft",
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
			new CatalogueCommodityRecipeInput(Grams: 2260.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2260 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_household_furniture_storage_0046_heavy_duty_rack", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a heavy-duty mild steel rack for domestic furnishing (@industrialised_household_furniture_storage_0046_heavy_duty_rack)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition HouseholdFurnitureAndStorageMakeHeavyDutyCastIronChestForDomesticFurnishingCraftIndustrialisedHouseholdFurnitureStorage0047HeavyDutyChest() => new()
	{
		Metadata = new("Household furniture and storage/make heavy-duty cast iron chest for domestic furnishing [craft_industrialised_household_furniture_storage_0047_heavy_duty_chest]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_household_furniture_storage_0047_heavy_duty_chest"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make heavy-duty cast iron chest for domestic furnishing [craft_industrialised_household_furniture_storage_0047_heavy_duty_chest]",
		Category = "Household furniture and storage",
		Blurb = "Make a heavy-duty cast iron chest for domestic furnishing from prepared cast iron stock.",
		Action = "making heavy-duty cast iron chest for domestic furnishing",
		ActiveCraftItemSdesc = "an in-progress heavy-duty cast iron chest for domestic furnishing craft",
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
			new CatalogueCommodityRecipeInput(Grams: 2301.0, Material: "cast iron", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2301 grams of cast iron", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_household_furniture_storage_0047_heavy_duty_chest", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a heavy-duty cast iron chest for domestic furnishing (@industrialised_household_furniture_storage_0047_heavy_duty_chest)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition HouseholdFurnitureAndStorageMakeHeavyDutyOakBenchForDomesticFurnishingCraftIndustrialisedHouseholdFurnitureStorage0048HeavyDutyBench() => new()
	{
		Metadata = new("Household furniture and storage/make heavy-duty oak bench for domestic furnishing [craft_industrialised_household_furniture_storage_0048_heavy_duty_bench]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_household_furniture_storage_0048_heavy_duty_bench"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make heavy-duty oak bench for domestic furnishing [craft_industrialised_household_furniture_storage_0048_heavy_duty_bench]",
		Category = "Household furniture and storage",
		Blurb = "Make a heavy-duty oak bench for domestic furnishing from prepared oak stock.",
		Action = "making heavy-duty oak bench for domestic furnishing",
		ActiveCraftItemSdesc = "an in-progress heavy-duty oak bench for domestic furnishing craft",
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
			new CatalogueCommodityRecipeInput(Grams: 2342.0, Material: "oak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2342 grams of oak", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_household_furniture_storage_0048_heavy_duty_bench", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a heavy-duty oak bench for domestic furnishing (@industrialised_household_furniture_storage_0048_heavy_duty_bench)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition HouseholdFurnitureAndStorageMakeFoldingMildSteelScrapForDomesticFurnishingCraftIndustrialisedHouseholdFurnitureStorage0049FoldingScrap() => new()
	{
		Metadata = new("Household furniture and storage/make folding mild steel scrap for domestic furnishing [craft_industrialised_household_furniture_storage_0049_folding_scrap]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_household_furniture_storage_0049_folding_scrap"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make folding mild steel scrap for domestic furnishing [craft_industrialised_household_furniture_storage_0049_folding_scrap]",
		Category = "Household furniture and storage",
		Blurb = "Make a folding mild steel scrap for domestic furnishing from prepared mild steel stock.",
		Action = "making folding mild steel scrap for domestic furnishing",
		ActiveCraftItemSdesc = "an in-progress folding mild steel scrap for domestic furnishing craft",
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
			new CatalogueCommodityRecipeInput(Grams: 2383.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2383 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_household_furniture_storage_0049_folding_scrap", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a folding mild steel scrap for domestic furnishing (@industrialised_household_furniture_storage_0049_folding_scrap)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition HouseholdFurnitureAndStorageMakeFoldingCastIronChairForDomesticFurnishingCraftIndustrialisedHouseholdFurnitureStorage0050FoldingChair() => new()
	{
		Metadata = new("Household furniture and storage/make folding cast iron chair for domestic furnishing [craft_industrialised_household_furniture_storage_0050_folding_chair]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_household_furniture_storage_0050_folding_chair"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make folding cast iron chair for domestic furnishing [craft_industrialised_household_furniture_storage_0050_folding_chair]",
		Category = "Household furniture and storage",
		Blurb = "Make a folding cast iron chair for domestic furnishing from prepared cast iron stock.",
		Action = "making folding cast iron chair for domestic furnishing",
		ActiveCraftItemSdesc = "an in-progress folding cast iron chair for domestic furnishing craft",
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
			new CatalogueCommodityRecipeInput(Grams: 2423.0, Material: "cast iron", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2423 grams of cast iron", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_household_furniture_storage_0050_folding_chair", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a folding cast iron chair for domestic furnishing (@industrialised_household_furniture_storage_0050_folding_chair)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition HouseholdFurnitureAndStorageMakeFoldingOakStoolForDomesticFurnishingCraftIndustrialisedHouseholdFurnitureStorage0051FoldingStool() => new()
	{
		Metadata = new("Household furniture and storage/make folding oak stool for domestic furnishing [craft_industrialised_household_furniture_storage_0051_folding_stool]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_household_furniture_storage_0051_folding_stool"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make folding oak stool for domestic furnishing [craft_industrialised_household_furniture_storage_0051_folding_stool]",
		Category = "Household furniture and storage",
		Blurb = "Make a folding oak stool for domestic furnishing from prepared oak stock.",
		Action = "making folding oak stool for domestic furnishing",
		ActiveCraftItemSdesc = "an in-progress folding oak stool for domestic furnishing craft",
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
			new CatalogueCommodityRecipeInput(Grams: 2464.0, Material: "oak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2464 grams of oak", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_household_furniture_storage_0051_folding_stool", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a folding oak stool for domestic furnishing (@industrialised_household_furniture_storage_0051_folding_stool)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition HouseholdFurnitureAndStorageMakeFoldingMildSteelTableForDomesticFurnishingCraftIndustrialisedHouseholdFurnitureStorage0052FoldingTable() => new()
	{
		Metadata = new("Household furniture and storage/make folding mild steel table for domestic furnishing [craft_industrialised_household_furniture_storage_0052_folding_table]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_household_furniture_storage_0052_folding_table"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make folding mild steel table for domestic furnishing [craft_industrialised_household_furniture_storage_0052_folding_table]",
		Category = "Household furniture and storage",
		Blurb = "Make a folding mild steel table for domestic furnishing from prepared mild steel stock.",
		Action = "making folding mild steel table for domestic furnishing",
		ActiveCraftItemSdesc = "an in-progress folding mild steel table for domestic furnishing craft",
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
			new CatalogueCommodityRecipeInput(Grams: 2505.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2505 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_household_furniture_storage_0052_folding_table", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a folding mild steel table for domestic furnishing (@industrialised_household_furniture_storage_0052_folding_table)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition HouseholdFurnitureAndStorageMakeFoldingCastIronCabinetForDomesticFurnishingCraftIndustrialisedHouseholdFurnitureStorage0053FoldingCabinet() => new()
	{
		Metadata = new("Household furniture and storage/make folding cast iron cabinet for domestic furnishing [craft_industrialised_household_furniture_storage_0053_folding_cabinet]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_household_furniture_storage_0053_folding_cabinet"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make folding cast iron cabinet for domestic furnishing [craft_industrialised_household_furniture_storage_0053_folding_cabinet]",
		Category = "Household furniture and storage",
		Blurb = "Make a folding cast iron cabinet for domestic furnishing from prepared cast iron stock.",
		Action = "making folding cast iron cabinet for domestic furnishing",
		ActiveCraftItemSdesc = "an in-progress folding cast iron cabinet for domestic furnishing craft",
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
			new CatalogueCommodityRecipeInput(Grams: 2545.0, Material: "cast iron", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2545 grams of cast iron", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_household_furniture_storage_0053_folding_cabinet", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a folding cast iron cabinet for domestic furnishing (@industrialised_household_furniture_storage_0053_folding_cabinet)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition HouseholdFurnitureAndStorageMakeFoldingOakShelfForDomesticFurnishingCraftIndustrialisedHouseholdFurnitureStorage0054FoldingShelf() => new()
	{
		Metadata = new("Household furniture and storage/make folding oak shelf for domestic furnishing [craft_industrialised_household_furniture_storage_0054_folding_shelf]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_household_furniture_storage_0054_folding_shelf"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make folding oak shelf for domestic furnishing [craft_industrialised_household_furniture_storage_0054_folding_shelf]",
		Category = "Household furniture and storage",
		Blurb = "Make a folding oak shelf for domestic furnishing from prepared oak stock.",
		Action = "making folding oak shelf for domestic furnishing",
		ActiveCraftItemSdesc = "an in-progress folding oak shelf for domestic furnishing craft",
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
			new CatalogueCommodityRecipeInput(Grams: 2586.0, Material: "oak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2586 grams of oak", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_household_furniture_storage_0054_folding_shelf", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a folding oak shelf for domestic furnishing (@industrialised_household_furniture_storage_0054_folding_shelf)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition HouseholdFurnitureAndStorageMakeFoldingMildSteelBedsteadForDomesticFurnishingCraftIndustrialisedHouseholdFurnitureStorage0055FoldingBedstead() => new()
	{
		Metadata = new("Household furniture and storage/make folding mild steel bedstead for domestic furnishing [craft_industrialised_household_furniture_storage_0055_folding_bedstead]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_household_furniture_storage_0055_folding_bedstead"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make folding mild steel bedstead for domestic furnishing [craft_industrialised_household_furniture_storage_0055_folding_bedstead]",
		Category = "Household furniture and storage",
		Blurb = "Make a folding mild steel bedstead for domestic furnishing from prepared mild steel stock.",
		Action = "making folding mild steel bedstead for domestic furnishing",
		ActiveCraftItemSdesc = "an in-progress folding mild steel bedstead for domestic furnishing craft",
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
			new CatalogueCommodityRecipeInput(Grams: 2627.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2627 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_household_furniture_storage_0055_folding_bedstead", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a folding mild steel bedstead for domestic furnishing (@industrialised_household_furniture_storage_0055_folding_bedstead)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition HouseholdFurnitureAndStorageMakeFoldingCastIronWashstandForDomesticFurnishingCraftIndustrialisedHouseholdFurnitureStorage0056FoldingWashstand() => new()
	{
		Metadata = new("Household furniture and storage/make folding cast iron washstand for domestic furnishing [craft_industrialised_household_furniture_storage_0056_folding_washstand]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_household_furniture_storage_0056_folding_washstand"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make folding cast iron washstand for domestic furnishing [craft_industrialised_household_furniture_storage_0056_folding_washstand]",
		Category = "Household furniture and storage",
		Blurb = "Make a folding cast iron washstand for domestic furnishing from prepared cast iron stock.",
		Action = "making folding cast iron washstand for domestic furnishing",
		ActiveCraftItemSdesc = "an in-progress folding cast iron washstand for domestic furnishing craft",
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
			new CatalogueCommodityRecipeInput(Grams: 2668.0, Material: "cast iron", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2668 grams of cast iron", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_household_furniture_storage_0056_folding_washstand", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a folding cast iron washstand for domestic furnishing (@industrialised_household_furniture_storage_0056_folding_washstand)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition HouseholdFurnitureAndStorageMakeFoldingOakScreenForDomesticFurnishingCraftIndustrialisedHouseholdFurnitureStorage0057FoldingScreen() => new()
	{
		Metadata = new("Household furniture and storage/make folding oak screen for domestic furnishing [craft_industrialised_household_furniture_storage_0057_folding_screen]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_household_furniture_storage_0057_folding_screen"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make folding oak screen for domestic furnishing [craft_industrialised_household_furniture_storage_0057_folding_screen]",
		Category = "Household furniture and storage",
		Blurb = "Make a folding oak screen for domestic furnishing from prepared oak stock.",
		Action = "making folding oak screen for domestic furnishing",
		ActiveCraftItemSdesc = "an in-progress folding oak screen for domestic furnishing craft",
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
			new CatalogueCommodityRecipeInput(Grams: 2708.0, Material: "oak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2708 grams of oak", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_household_furniture_storage_0057_folding_screen", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a folding oak screen for domestic furnishing (@industrialised_household_furniture_storage_0057_folding_screen)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition HouseholdFurnitureAndStorageMakeFoldingMildSteelRackForDomesticFurnishingCraftIndustrialisedHouseholdFurnitureStorage0058FoldingRack() => new()
	{
		Metadata = new("Household furniture and storage/make folding mild steel rack for domestic furnishing [craft_industrialised_household_furniture_storage_0058_folding_rack]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_household_furniture_storage_0058_folding_rack"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make folding mild steel rack for domestic furnishing [craft_industrialised_household_furniture_storage_0058_folding_rack]",
		Category = "Household furniture and storage",
		Blurb = "Make a folding mild steel rack for domestic furnishing from prepared mild steel stock.",
		Action = "making folding mild steel rack for domestic furnishing",
		ActiveCraftItemSdesc = "an in-progress folding mild steel rack for domestic furnishing craft",
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
			new CatalogueCommodityRecipeInput(Grams: 2749.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2749 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_household_furniture_storage_0058_folding_rack", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a folding mild steel rack for domestic furnishing (@industrialised_household_furniture_storage_0058_folding_rack)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition HouseholdFurnitureAndStorageMakeFoldingCastIronChestForDomesticFurnishingCraftIndustrialisedHouseholdFurnitureStorage0059FoldingChest() => new()
	{
		Metadata = new("Household furniture and storage/make folding cast iron chest for domestic furnishing [craft_industrialised_household_furniture_storage_0059_folding_chest]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_household_furniture_storage_0059_folding_chest"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make folding cast iron chest for domestic furnishing [craft_industrialised_household_furniture_storage_0059_folding_chest]",
		Category = "Household furniture and storage",
		Blurb = "Make a folding cast iron chest for domestic furnishing from prepared cast iron stock.",
		Action = "making folding cast iron chest for domestic furnishing",
		ActiveCraftItemSdesc = "an in-progress folding cast iron chest for domestic furnishing craft",
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
			new CatalogueCommodityRecipeInput(Grams: 2790.0, Material: "cast iron", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2790 grams of cast iron", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_household_furniture_storage_0059_folding_chest", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a folding cast iron chest for domestic furnishing (@industrialised_household_furniture_storage_0059_folding_chest)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition HouseholdFurnitureAndStorageMakeFoldingOakBenchForDomesticFurnishingCraftIndustrialisedHouseholdFurnitureStorage0060FoldingBench() => new()
	{
		Metadata = new("Household furniture and storage/make folding oak bench for domestic furnishing [craft_industrialised_household_furniture_storage_0060_folding_bench]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_household_furniture_storage_0060_folding_bench"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make folding oak bench for domestic furnishing [craft_industrialised_household_furniture_storage_0060_folding_bench]",
		Category = "Household furniture and storage",
		Blurb = "Make a folding oak bench for domestic furnishing from prepared oak stock.",
		Action = "making folding oak bench for domestic furnishing",
		ActiveCraftItemSdesc = "an in-progress folding oak bench for domestic furnishing craft",
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
			new CatalogueCommodityRecipeInput(Grams: 2830.0, Material: "oak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2830 grams of oak", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_household_furniture_storage_0060_folding_bench", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a folding oak bench for domestic furnishing (@industrialised_household_furniture_storage_0060_folding_bench)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition HouseholdFurnitureAndStorageMakePortableMildSteelScrapForDomesticFurnishingCraftIndustrialisedHouseholdFurnitureStorage0061PortableScrap() => new()
	{
		Metadata = new("Household furniture and storage/make portable mild steel scrap for domestic furnishing [craft_industrialised_household_furniture_storage_0061_portable_scrap]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_household_furniture_storage_0061_portable_scrap"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make portable mild steel scrap for domestic furnishing [craft_industrialised_household_furniture_storage_0061_portable_scrap]",
		Category = "Household furniture and storage",
		Blurb = "Make a portable mild steel scrap for domestic furnishing from prepared mild steel stock.",
		Action = "making portable mild steel scrap for domestic furnishing",
		ActiveCraftItemSdesc = "an in-progress portable mild steel scrap for domestic furnishing craft",
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
			new CatalogueCommodityRecipeInput(Grams: 2871.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2871 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_household_furniture_storage_0061_portable_scrap", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a portable mild steel scrap for domestic furnishing (@industrialised_household_furniture_storage_0061_portable_scrap)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition HouseholdFurnitureAndStorageMakePortableCastIronChairForDomesticFurnishingCraftIndustrialisedHouseholdFurnitureStorage0062PortableChair() => new()
	{
		Metadata = new("Household furniture and storage/make portable cast iron chair for domestic furnishing [craft_industrialised_household_furniture_storage_0062_portable_chair]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_household_furniture_storage_0062_portable_chair"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make portable cast iron chair for domestic furnishing [craft_industrialised_household_furniture_storage_0062_portable_chair]",
		Category = "Household furniture and storage",
		Blurb = "Make a portable cast iron chair for domestic furnishing from prepared cast iron stock.",
		Action = "making portable cast iron chair for domestic furnishing",
		ActiveCraftItemSdesc = "an in-progress portable cast iron chair for domestic furnishing craft",
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
			new CatalogueCommodityRecipeInput(Grams: 2912.0, Material: "cast iron", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2912 grams of cast iron", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_household_furniture_storage_0062_portable_chair", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a portable cast iron chair for domestic furnishing (@industrialised_household_furniture_storage_0062_portable_chair)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition HouseholdFurnitureAndStorageMakePortableOakStoolForDomesticFurnishingCraftIndustrialisedHouseholdFurnitureStorage0063PortableStool() => new()
	{
		Metadata = new("Household furniture and storage/make portable oak stool for domestic furnishing [craft_industrialised_household_furniture_storage_0063_portable_stool]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_household_furniture_storage_0063_portable_stool"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make portable oak stool for domestic furnishing [craft_industrialised_household_furniture_storage_0063_portable_stool]",
		Category = "Household furniture and storage",
		Blurb = "Make a portable oak stool for domestic furnishing from prepared oak stock.",
		Action = "making portable oak stool for domestic furnishing",
		ActiveCraftItemSdesc = "an in-progress portable oak stool for domestic furnishing craft",
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
			new CatalogueCommodityRecipeInput(Grams: 2952.0, Material: "oak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2952 grams of oak", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_household_furniture_storage_0063_portable_stool", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a portable oak stool for domestic furnishing (@industrialised_household_furniture_storage_0063_portable_stool)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition HouseholdFurnitureAndStorageMakePortableMildSteelTableForDomesticFurnishingCraftIndustrialisedHouseholdFurnitureStorage0064PortableTable() => new()
	{
		Metadata = new("Household furniture and storage/make portable mild steel table for domestic furnishing [craft_industrialised_household_furniture_storage_0064_portable_table]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_household_furniture_storage_0064_portable_table"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make portable mild steel table for domestic furnishing [craft_industrialised_household_furniture_storage_0064_portable_table]",
		Category = "Household furniture and storage",
		Blurb = "Make a portable mild steel table for domestic furnishing from prepared mild steel stock.",
		Action = "making portable mild steel table for domestic furnishing",
		ActiveCraftItemSdesc = "an in-progress portable mild steel table for domestic furnishing craft",
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
			new CatalogueCommodityRecipeInput(Grams: 2993.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2993 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_household_furniture_storage_0064_portable_table", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a portable mild steel table for domestic furnishing (@industrialised_household_furniture_storage_0064_portable_table)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition HouseholdFurnitureAndStorageMakePortableCastIronCabinetForDomesticFurnishingCraftIndustrialisedHouseholdFurnitureStorage0065PortableCabinet() => new()
	{
		Metadata = new("Household furniture and storage/make portable cast iron cabinet for domestic furnishing [craft_industrialised_household_furniture_storage_0065_portable_cabinet]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_household_furniture_storage_0065_portable_cabinet"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make portable cast iron cabinet for domestic furnishing [craft_industrialised_household_furniture_storage_0065_portable_cabinet]",
		Category = "Household furniture and storage",
		Blurb = "Make a portable cast iron cabinet for domestic furnishing from prepared cast iron stock.",
		Action = "making portable cast iron cabinet for domestic furnishing",
		ActiveCraftItemSdesc = "an in-progress portable cast iron cabinet for domestic furnishing craft",
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
			new CatalogueCommodityRecipeInput(Grams: 3034.0, Material: "cast iron", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3034 grams of cast iron", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_household_furniture_storage_0065_portable_cabinet", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a portable cast iron cabinet for domestic furnishing (@industrialised_household_furniture_storage_0065_portable_cabinet)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition HouseholdFurnitureAndStorageMakePortableOakShelfForDomesticFurnishingCraftIndustrialisedHouseholdFurnitureStorage0066PortableShelf() => new()
	{
		Metadata = new("Household furniture and storage/make portable oak shelf for domestic furnishing [craft_industrialised_household_furniture_storage_0066_portable_shelf]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_household_furniture_storage_0066_portable_shelf"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make portable oak shelf for domestic furnishing [craft_industrialised_household_furniture_storage_0066_portable_shelf]",
		Category = "Household furniture and storage",
		Blurb = "Make a portable oak shelf for domestic furnishing from prepared oak stock.",
		Action = "making portable oak shelf for domestic furnishing",
		ActiveCraftItemSdesc = "an in-progress portable oak shelf for domestic furnishing craft",
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
			new CatalogueCommodityRecipeInput(Grams: 3075.0, Material: "oak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3075 grams of oak", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_household_furniture_storage_0066_portable_shelf", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a portable oak shelf for domestic furnishing (@industrialised_household_furniture_storage_0066_portable_shelf)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition HouseholdFurnitureAndStorageMakePortableMildSteelBedsteadForDomesticFurnishingCraftIndustrialisedHouseholdFurnitureStorage0067PortableBedstead() => new()
	{
		Metadata = new("Household furniture and storage/make portable mild steel bedstead for domestic furnishing [craft_industrialised_household_furniture_storage_0067_portable_bedstead]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_household_furniture_storage_0067_portable_bedstead"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make portable mild steel bedstead for domestic furnishing [craft_industrialised_household_furniture_storage_0067_portable_bedstead]",
		Category = "Household furniture and storage",
		Blurb = "Make a portable mild steel bedstead for domestic furnishing from prepared mild steel stock.",
		Action = "making portable mild steel bedstead for domestic furnishing",
		ActiveCraftItemSdesc = "an in-progress portable mild steel bedstead for domestic furnishing craft",
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
			new CatalogueCommodityRecipeInput(Grams: 3115.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3115 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_household_furniture_storage_0067_portable_bedstead", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a portable mild steel bedstead for domestic furnishing (@industrialised_household_furniture_storage_0067_portable_bedstead)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition HouseholdFurnitureAndStorageMakePortableCastIronWashstandForDomesticFurnishingCraftIndustrialisedHouseholdFurnitureStorage0068PortableWashstand() => new()
	{
		Metadata = new("Household furniture and storage/make portable cast iron washstand for domestic furnishing [craft_industrialised_household_furniture_storage_0068_portable_washstand]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_household_furniture_storage_0068_portable_washstand"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make portable cast iron washstand for domestic furnishing [craft_industrialised_household_furniture_storage_0068_portable_washstand]",
		Category = "Household furniture and storage",
		Blurb = "Make a portable cast iron washstand for domestic furnishing from prepared cast iron stock.",
		Action = "making portable cast iron washstand for domestic furnishing",
		ActiveCraftItemSdesc = "an in-progress portable cast iron washstand for domestic furnishing craft",
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
			new CatalogueCommodityRecipeInput(Grams: 3156.0, Material: "cast iron", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3156 grams of cast iron", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_household_furniture_storage_0068_portable_washstand", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a portable cast iron washstand for domestic furnishing (@industrialised_household_furniture_storage_0068_portable_washstand)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition HouseholdFurnitureAndStorageMakePortableOakScreenForDomesticFurnishingCraftIndustrialisedHouseholdFurnitureStorage0069PortableScreen() => new()
	{
		Metadata = new("Household furniture and storage/make portable oak screen for domestic furnishing [craft_industrialised_household_furniture_storage_0069_portable_screen]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_household_furniture_storage_0069_portable_screen"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make portable oak screen for domestic furnishing [craft_industrialised_household_furniture_storage_0069_portable_screen]",
		Category = "Household furniture and storage",
		Blurb = "Make a portable oak screen for domestic furnishing from prepared oak stock.",
		Action = "making portable oak screen for domestic furnishing",
		ActiveCraftItemSdesc = "an in-progress portable oak screen for domestic furnishing craft",
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
			new CatalogueCommodityRecipeInput(Grams: 3197.0, Material: "oak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3197 grams of oak", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_household_furniture_storage_0069_portable_screen", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a portable oak screen for domestic furnishing (@industrialised_household_furniture_storage_0069_portable_screen)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition HouseholdFurnitureAndStorageMakePortableMildSteelRackForDomesticFurnishingCraftIndustrialisedHouseholdFurnitureStorage0070PortableRack() => new()
	{
		Metadata = new("Household furniture and storage/make portable mild steel rack for domestic furnishing [craft_industrialised_household_furniture_storage_0070_portable_rack]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_household_furniture_storage_0070_portable_rack"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make portable mild steel rack for domestic furnishing [craft_industrialised_household_furniture_storage_0070_portable_rack]",
		Category = "Household furniture and storage",
		Blurb = "Make a portable mild steel rack for domestic furnishing from prepared mild steel stock.",
		Action = "making portable mild steel rack for domestic furnishing",
		ActiveCraftItemSdesc = "an in-progress portable mild steel rack for domestic furnishing craft",
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
			new CatalogueCommodityRecipeInput(Grams: 3237.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3237 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_household_furniture_storage_0070_portable_rack", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a portable mild steel rack for domestic furnishing (@industrialised_household_furniture_storage_0070_portable_rack)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition HouseholdFurnitureAndStorageMakePortableCastIronChestForDomesticFurnishingCraftIndustrialisedHouseholdFurnitureStorage0071PortableChest() => new()
	{
		Metadata = new("Household furniture and storage/make portable cast iron chest for domestic furnishing [craft_industrialised_household_furniture_storage_0071_portable_chest]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_household_furniture_storage_0071_portable_chest"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make portable cast iron chest for domestic furnishing [craft_industrialised_household_furniture_storage_0071_portable_chest]",
		Category = "Household furniture and storage",
		Blurb = "Make a portable cast iron chest for domestic furnishing from prepared cast iron stock.",
		Action = "making portable cast iron chest for domestic furnishing",
		ActiveCraftItemSdesc = "an in-progress portable cast iron chest for domestic furnishing craft",
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
			new CatalogueCommodityRecipeInput(Grams: 3278.0, Material: "cast iron", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3278 grams of cast iron", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_household_furniture_storage_0071_portable_chest", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a portable cast iron chest for domestic furnishing (@industrialised_household_furniture_storage_0071_portable_chest)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition HouseholdFurnitureAndStorageMakePortableOakBenchForDomesticFurnishingCraftIndustrialisedHouseholdFurnitureStorage0072PortableBench() => new()
	{
		Metadata = new("Household furniture and storage/make portable oak bench for domestic furnishing [craft_industrialised_household_furniture_storage_0072_portable_bench]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_household_furniture_storage_0072_portable_bench"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make portable oak bench for domestic furnishing [craft_industrialised_household_furniture_storage_0072_portable_bench]",
		Category = "Household furniture and storage",
		Blurb = "Make a portable oak bench for domestic furnishing from prepared oak stock.",
		Action = "making portable oak bench for domestic furnishing",
		ActiveCraftItemSdesc = "an in-progress portable oak bench for domestic furnishing craft",
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
			new CatalogueCommodityRecipeInput(Grams: 3319.0, Material: "oak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3319 grams of oak", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_household_furniture_storage_0072_portable_bench", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a portable oak bench for domestic furnishing (@industrialised_household_furniture_storage_0072_portable_bench)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition HouseholdFurnitureAndStorageMakeWorkshopMildSteelScrapForDomesticFurnishingCraftIndustrialisedHouseholdFurnitureStorage0073WorkshopScrap() => new()
	{
		Metadata = new("Household furniture and storage/make workshop mild steel scrap for domestic furnishing [craft_industrialised_household_furniture_storage_0073_workshop_scrap]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_household_furniture_storage_0073_workshop_scrap"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make workshop mild steel scrap for domestic furnishing [craft_industrialised_household_furniture_storage_0073_workshop_scrap]",
		Category = "Household furniture and storage",
		Blurb = "Make a workshop mild steel scrap for domestic furnishing from prepared mild steel stock.",
		Action = "making workshop mild steel scrap for domestic furnishing",
		ActiveCraftItemSdesc = "an in-progress workshop mild steel scrap for domestic furnishing craft",
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
			new CatalogueCommodityRecipeInput(Grams: 3359.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3359 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_household_furniture_storage_0073_workshop_scrap", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a workshop mild steel scrap for domestic furnishing (@industrialised_household_furniture_storage_0073_workshop_scrap)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition HouseholdFurnitureAndStorageMakeWorkshopCastIronChairForDomesticFurnishingCraftIndustrialisedHouseholdFurnitureStorage0074WorkshopChair() => new()
	{
		Metadata = new("Household furniture and storage/make workshop cast iron chair for domestic furnishing [craft_industrialised_household_furniture_storage_0074_workshop_chair]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_household_furniture_storage_0074_workshop_chair"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make workshop cast iron chair for domestic furnishing [craft_industrialised_household_furniture_storage_0074_workshop_chair]",
		Category = "Household furniture and storage",
		Blurb = "Make a workshop cast iron chair for domestic furnishing from prepared cast iron stock.",
		Action = "making workshop cast iron chair for domestic furnishing",
		ActiveCraftItemSdesc = "an in-progress workshop cast iron chair for domestic furnishing craft",
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
			new CatalogueCommodityRecipeInput(Grams: 3400.0, Material: "cast iron", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3400 grams of cast iron", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_household_furniture_storage_0074_workshop_chair", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a workshop cast iron chair for domestic furnishing (@industrialised_household_furniture_storage_0074_workshop_chair)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition HouseholdFurnitureAndStorageMakeWorkshopOakStoolForDomesticFurnishingCraftIndustrialisedHouseholdFurnitureStorage0075WorkshopStool() => new()
	{
		Metadata = new("Household furniture and storage/make workshop oak stool for domestic furnishing [craft_industrialised_household_furniture_storage_0075_workshop_stool]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_household_furniture_storage_0075_workshop_stool"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make workshop oak stool for domestic furnishing [craft_industrialised_household_furniture_storage_0075_workshop_stool]",
		Category = "Household furniture and storage",
		Blurb = "Make a workshop oak stool for domestic furnishing from prepared oak stock.",
		Action = "making workshop oak stool for domestic furnishing",
		ActiveCraftItemSdesc = "an in-progress workshop oak stool for domestic furnishing craft",
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
			new CatalogueCommodityRecipeInput(Grams: 3441.0, Material: "oak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3441 grams of oak", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_household_furniture_storage_0075_workshop_stool", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a workshop oak stool for domestic furnishing (@industrialised_household_furniture_storage_0075_workshop_stool)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition HouseholdFurnitureAndStorageMakeWorkshopMildSteelTableForDomesticFurnishingCraftIndustrialisedHouseholdFurnitureStorage0076WorkshopTable() => new()
	{
		Metadata = new("Household furniture and storage/make workshop mild steel table for domestic furnishing [craft_industrialised_household_furniture_storage_0076_workshop_table]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_household_furniture_storage_0076_workshop_table"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make workshop mild steel table for domestic furnishing [craft_industrialised_household_furniture_storage_0076_workshop_table]",
		Category = "Household furniture and storage",
		Blurb = "Make a workshop mild steel table for domestic furnishing from prepared mild steel stock.",
		Action = "making workshop mild steel table for domestic furnishing",
		ActiveCraftItemSdesc = "an in-progress workshop mild steel table for domestic furnishing craft",
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
			new CatalogueCommodityRecipeInput(Grams: 3482.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3482 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_household_furniture_storage_0076_workshop_table", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a workshop mild steel table for domestic furnishing (@industrialised_household_furniture_storage_0076_workshop_table)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition HouseholdFurnitureAndStorageMakeWorkshopCastIronCabinetForDomesticFurnishingCraftIndustrialisedHouseholdFurnitureStorage0077WorkshopCabinet() => new()
	{
		Metadata = new("Household furniture and storage/make workshop cast iron cabinet for domestic furnishing [craft_industrialised_household_furniture_storage_0077_workshop_cabinet]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_household_furniture_storage_0077_workshop_cabinet"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make workshop cast iron cabinet for domestic furnishing [craft_industrialised_household_furniture_storage_0077_workshop_cabinet]",
		Category = "Household furniture and storage",
		Blurb = "Make a workshop cast iron cabinet for domestic furnishing from prepared cast iron stock.",
		Action = "making workshop cast iron cabinet for domestic furnishing",
		ActiveCraftItemSdesc = "an in-progress workshop cast iron cabinet for domestic furnishing craft",
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
			new CatalogueCommodityRecipeInput(Grams: 3522.0, Material: "cast iron", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3522 grams of cast iron", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_household_furniture_storage_0077_workshop_cabinet", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a workshop cast iron cabinet for domestic furnishing (@industrialised_household_furniture_storage_0077_workshop_cabinet)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition HouseholdFurnitureAndStorageMakeWorkshopOakShelfForDomesticFurnishingCraftIndustrialisedHouseholdFurnitureStorage0078WorkshopShelf() => new()
	{
		Metadata = new("Household furniture and storage/make workshop oak shelf for domestic furnishing [craft_industrialised_household_furniture_storage_0078_workshop_shelf]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_household_furniture_storage_0078_workshop_shelf"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make workshop oak shelf for domestic furnishing [craft_industrialised_household_furniture_storage_0078_workshop_shelf]",
		Category = "Household furniture and storage",
		Blurb = "Make a workshop oak shelf for domestic furnishing from prepared oak stock.",
		Action = "making workshop oak shelf for domestic furnishing",
		ActiveCraftItemSdesc = "an in-progress workshop oak shelf for domestic furnishing craft",
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
			new CatalogueCommodityRecipeInput(Grams: 3563.0, Material: "oak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3563 grams of oak", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_household_furniture_storage_0078_workshop_shelf", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a workshop oak shelf for domestic furnishing (@industrialised_household_furniture_storage_0078_workshop_shelf)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition HouseholdFurnitureAndStorageMakeWorkshopMildSteelBedsteadForDomesticFurnishingCraftIndustrialisedHouseholdFurnitureStorage0079WorkshopBedstead() => new()
	{
		Metadata = new("Household furniture and storage/make workshop mild steel bedstead for domestic furnishing [craft_industrialised_household_furniture_storage_0079_workshop_bedstead]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_household_furniture_storage_0079_workshop_bedstead"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make workshop mild steel bedstead for domestic furnishing [craft_industrialised_household_furniture_storage_0079_workshop_bedstead]",
		Category = "Household furniture and storage",
		Blurb = "Make a workshop mild steel bedstead for domestic furnishing from prepared mild steel stock.",
		Action = "making workshop mild steel bedstead for domestic furnishing",
		ActiveCraftItemSdesc = "an in-progress workshop mild steel bedstead for domestic furnishing craft",
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
			new CatalogueCommodityRecipeInput(Grams: 3604.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3604 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_household_furniture_storage_0079_workshop_bedstead", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a workshop mild steel bedstead for domestic furnishing (@industrialised_household_furniture_storage_0079_workshop_bedstead)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition HouseholdFurnitureAndStorageMakeWorkshopCastIronWashstandForDomesticFurnishingCraftIndustrialisedHouseholdFurnitureStorage0080WorkshopWashstand() => new()
	{
		Metadata = new("Household furniture and storage/make workshop cast iron washstand for domestic furnishing [craft_industrialised_household_furniture_storage_0080_workshop_washstand]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_household_furniture_storage_0080_workshop_washstand"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make workshop cast iron washstand for domestic furnishing [craft_industrialised_household_furniture_storage_0080_workshop_washstand]",
		Category = "Household furniture and storage",
		Blurb = "Make a workshop cast iron washstand for domestic furnishing from prepared cast iron stock.",
		Action = "making workshop cast iron washstand for domestic furnishing",
		ActiveCraftItemSdesc = "an in-progress workshop cast iron washstand for domestic furnishing craft",
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
			new CatalogueCommodityRecipeInput(Grams: 3644.0, Material: "cast iron", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3644 grams of cast iron", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_household_furniture_storage_0080_workshop_washstand", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a workshop cast iron washstand for domestic furnishing (@industrialised_household_furniture_storage_0080_workshop_washstand)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition HouseholdFurnitureAndStorageMakeWorkshopOakScreenForDomesticFurnishingCraftIndustrialisedHouseholdFurnitureStorage0081WorkshopScreen() => new()
	{
		Metadata = new("Household furniture and storage/make workshop oak screen for domestic furnishing [craft_industrialised_household_furniture_storage_0081_workshop_screen]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_household_furniture_storage_0081_workshop_screen"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make workshop oak screen for domestic furnishing [craft_industrialised_household_furniture_storage_0081_workshop_screen]",
		Category = "Household furniture and storage",
		Blurb = "Make a workshop oak screen for domestic furnishing from prepared oak stock.",
		Action = "making workshop oak screen for domestic furnishing",
		ActiveCraftItemSdesc = "an in-progress workshop oak screen for domestic furnishing craft",
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
			new CatalogueCommodityRecipeInput(Grams: 3685.0, Material: "oak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3685 grams of oak", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_household_furniture_storage_0081_workshop_screen", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a workshop oak screen for domestic furnishing (@industrialised_household_furniture_storage_0081_workshop_screen)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};
}
