#nullable enable

using MudSharp.Body;
using MudSharp.GameItems.Inventory.Plans;
using MudSharp.RPG.Checks;

namespace DatabaseSeeder.Seeders;

public static partial class Recipes20Stock
{
	public static CatalogueRecipeDefinition HouseholdFurnitureAndStorageMakeWorkshopMildSteelRackForDomesticFurnishingCraftIndustrialisedHouseholdFurnitureStorage0082WorkshopRack() => new()
	{
		Metadata = new("Household furniture and storage/make workshop mild steel rack for domestic furnishing [craft_industrialised_household_furniture_storage_0082_workshop_rack]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_household_furniture_storage_0082_workshop_rack"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make workshop mild steel rack for domestic furnishing [craft_industrialised_household_furniture_storage_0082_workshop_rack]",
		Category = "Household furniture and storage",
		Blurb = "Make a workshop mild steel rack for domestic furnishing from prepared mild steel stock.",
		Action = "making workshop mild steel rack for domestic furnishing",
		ActiveCraftItemSdesc = "an in-progress workshop mild steel rack for domestic furnishing craft",
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
			new CatalogueCommodityRecipeInput(Grams: 3726.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3726 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_household_furniture_storage_0082_workshop_rack", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a workshop mild steel rack for domestic furnishing (@industrialised_household_furniture_storage_0082_workshop_rack)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition HouseholdFurnitureAndStorageMakeWorkshopCastIronChestForDomesticFurnishingCraftIndustrialisedHouseholdFurnitureStorage0083WorkshopChest() => new()
	{
		Metadata = new("Household furniture and storage/make workshop cast iron chest for domestic furnishing [craft_industrialised_household_furniture_storage_0083_workshop_chest]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_household_furniture_storage_0083_workshop_chest"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make workshop cast iron chest for domestic furnishing [craft_industrialised_household_furniture_storage_0083_workshop_chest]",
		Category = "Household furniture and storage",
		Blurb = "Make a workshop cast iron chest for domestic furnishing from prepared cast iron stock.",
		Action = "making workshop cast iron chest for domestic furnishing",
		ActiveCraftItemSdesc = "an in-progress workshop cast iron chest for domestic furnishing craft",
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
			new CatalogueCommodityRecipeInput(Grams: 3766.0, Material: "cast iron", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3766 grams of cast iron", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_household_furniture_storage_0083_workshop_chest", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a workshop cast iron chest for domestic furnishing (@industrialised_household_furniture_storage_0083_workshop_chest)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition HouseholdFurnitureAndStorageMakeWorkshopOakBenchForDomesticFurnishingCraftIndustrialisedHouseholdFurnitureStorage0084WorkshopBench() => new()
	{
		Metadata = new("Household furniture and storage/make workshop oak bench for domestic furnishing [craft_industrialised_household_furniture_storage_0084_workshop_bench]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_household_furniture_storage_0084_workshop_bench"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make workshop oak bench for domestic furnishing [craft_industrialised_household_furniture_storage_0084_workshop_bench]",
		Category = "Household furniture and storage",
		Blurb = "Make a workshop oak bench for domestic furnishing from prepared oak stock.",
		Action = "making workshop oak bench for domestic furnishing",
		ActiveCraftItemSdesc = "an in-progress workshop oak bench for domestic furnishing craft",
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
			new CatalogueCommodityRecipeInput(Grams: 3807.0, Material: "oak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3807 grams of oak", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_household_furniture_storage_0084_workshop_bench", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a workshop oak bench for domestic furnishing (@industrialised_household_furniture_storage_0084_workshop_bench)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition HouseholdFurnitureAndStorageMakeInstitutionalMildSteelScrapForDomesticFurnishingCraftIndustrialisedHouseholdFurnitureStorage0085InstitutionalScrap() => new()
	{
		Metadata = new("Household furniture and storage/make institutional mild steel scrap for domestic furnishing [craft_industrialised_household_furniture_storage_0085_institutional_scrap]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_household_furniture_storage_0085_institutional_scrap"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make institutional mild steel scrap for domestic furnishing [craft_industrialised_household_furniture_storage_0085_institutional_scrap]",
		Category = "Household furniture and storage",
		Blurb = "Make an institutional mild steel scrap for domestic furnishing from prepared mild steel stock.",
		Action = "making institutional mild steel scrap for domestic furnishing",
		ActiveCraftItemSdesc = "an in-progress institutional mild steel scrap for domestic furnishing craft",
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
			new CatalogueCommodityRecipeInput(Grams: 3848.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3848 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_household_furniture_storage_0085_institutional_scrap", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x an institutional mild steel scrap for domestic furnishing (@industrialised_household_furniture_storage_0085_institutional_scrap)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition HouseholdFurnitureAndStorageMakeInstitutionalCastIronChairForDomesticFurnishingCraftIndustrialisedHouseholdFurnitureStorage0086InstitutionalChair() => new()
	{
		Metadata = new("Household furniture and storage/make institutional cast iron chair for domestic furnishing [craft_industrialised_household_furniture_storage_0086_institutional_chair]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_household_furniture_storage_0086_institutional_chair"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make institutional cast iron chair for domestic furnishing [craft_industrialised_household_furniture_storage_0086_institutional_chair]",
		Category = "Household furniture and storage",
		Blurb = "Make an institutional cast iron chair for domestic furnishing from prepared cast iron stock.",
		Action = "making institutional cast iron chair for domestic furnishing",
		ActiveCraftItemSdesc = "an in-progress institutional cast iron chair for domestic furnishing craft",
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
			new CatalogueCommodityRecipeInput(Grams: 3889.0, Material: "cast iron", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3889 grams of cast iron", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_household_furniture_storage_0086_institutional_chair", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x an institutional cast iron chair for domestic furnishing (@industrialised_household_furniture_storage_0086_institutional_chair)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition HouseholdFurnitureAndStorageMakeInstitutionalOakStoolForDomesticFurnishingCraftIndustrialisedHouseholdFurnitureStorage0087InstitutionalStool() => new()
	{
		Metadata = new("Household furniture and storage/make institutional oak stool for domestic furnishing [craft_industrialised_household_furniture_storage_0087_institutional_stool]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_household_furniture_storage_0087_institutional_stool"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make institutional oak stool for domestic furnishing [craft_industrialised_household_furniture_storage_0087_institutional_stool]",
		Category = "Household furniture and storage",
		Blurb = "Make an institutional oak stool for domestic furnishing from prepared oak stock.",
		Action = "making institutional oak stool for domestic furnishing",
		ActiveCraftItemSdesc = "an in-progress institutional oak stool for domestic furnishing craft",
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
			new CatalogueCommodityRecipeInput(Grams: 3929.0, Material: "oak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3929 grams of oak", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_household_furniture_storage_0087_institutional_stool", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x an institutional oak stool for domestic furnishing (@industrialised_household_furniture_storage_0087_institutional_stool)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition HouseholdFurnitureAndStorageMakeInstitutionalMildSteelTableForDomesticFurnishingCraftIndustrialisedHouseholdFurnitureStorage0088InstitutionalTable() => new()
	{
		Metadata = new("Household furniture and storage/make institutional mild steel table for domestic furnishing [craft_industrialised_household_furniture_storage_0088_institutional_table]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_household_furniture_storage_0088_institutional_table"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make institutional mild steel table for domestic furnishing [craft_industrialised_household_furniture_storage_0088_institutional_table]",
		Category = "Household furniture and storage",
		Blurb = "Make an institutional mild steel table for domestic furnishing from prepared mild steel stock.",
		Action = "making institutional mild steel table for domestic furnishing",
		ActiveCraftItemSdesc = "an in-progress institutional mild steel table for domestic furnishing craft",
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
			new CatalogueCommodityRecipeInput(Grams: 3970.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3970 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_household_furniture_storage_0088_institutional_table", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x an institutional mild steel table for domestic furnishing (@industrialised_household_furniture_storage_0088_institutional_table)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition HouseholdFurnitureAndStorageMakeInstitutionalCastIronCabinetForDomesticFurnishingCraftIndustrialisedHouseholdFurnitureStorage0089InstitutionalCabinet() => new()
	{
		Metadata = new("Household furniture and storage/make institutional cast iron cabinet for domestic furnishing [craft_industrialised_household_furniture_storage_0089_institutional_cabinet]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_household_furniture_storage_0089_institutional_cabinet"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make institutional cast iron cabinet for domestic furnishing [craft_industrialised_household_furniture_storage_0089_institutional_cabinet]",
		Category = "Household furniture and storage",
		Blurb = "Make an institutional cast iron cabinet for domestic furnishing from prepared cast iron stock.",
		Action = "making institutional cast iron cabinet for domestic furnishing",
		ActiveCraftItemSdesc = "an in-progress institutional cast iron cabinet for domestic furnishing craft",
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
			new CatalogueCommodityRecipeInput(Grams: 4011.0, Material: "cast iron", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("4011 grams of cast iron", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_household_furniture_storage_0089_institutional_cabinet", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x an institutional cast iron cabinet for domestic furnishing (@industrialised_household_furniture_storage_0089_institutional_cabinet)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition HouseholdFurnitureAndStorageMakeInstitutionalOakShelfForDomesticFurnishingCraftIndustrialisedHouseholdFurnitureStorage0090InstitutionalShelf() => new()
	{
		Metadata = new("Household furniture and storage/make institutional oak shelf for domestic furnishing [craft_industrialised_household_furniture_storage_0090_institutional_shelf]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_household_furniture_storage_0090_institutional_shelf"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make institutional oak shelf for domestic furnishing [craft_industrialised_household_furniture_storage_0090_institutional_shelf]",
		Category = "Household furniture and storage",
		Blurb = "Make an institutional oak shelf for domestic furnishing from prepared oak stock.",
		Action = "making institutional oak shelf for domestic furnishing",
		ActiveCraftItemSdesc = "an in-progress institutional oak shelf for domestic furnishing craft",
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
			new CatalogueCommodityRecipeInput(Grams: 4051.0, Material: "oak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("4051 grams of oak", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_household_furniture_storage_0090_institutional_shelf", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x an institutional oak shelf for domestic furnishing (@industrialised_household_furniture_storage_0090_institutional_shelf)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition HouseholdFurnitureAndStorageMakeInstitutionalMildSteelBedsteadForDomesticFurnishingCraftIndustrialisedHouseholdFurnitureStorage0091InstitutionalBedstead() => new()
	{
		Metadata = new("Household furniture and storage/make institutional mild steel bedstead for domestic furnishing [craft_industrialised_household_furniture_storage_0091_institutional_bedstead]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_household_furniture_storage_0091_institutional_bedstead"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make institutional mild steel bedstead for domestic furnishing [craft_industrialised_household_furniture_storage_0091_institutional_bedstead]",
		Category = "Household furniture and storage",
		Blurb = "Make an institutional mild steel bedstead for domestic furnishing from prepared mild steel stock.",
		Action = "making institutional mild steel bedstead for domestic furnishing",
		ActiveCraftItemSdesc = "an in-progress institutional mild steel bedstead for domestic furnishing craft",
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
			new CatalogueCommodityRecipeInput(Grams: 4092.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("4092 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_household_furniture_storage_0091_institutional_bedstead", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x an institutional mild steel bedstead for domestic furnishing (@industrialised_household_furniture_storage_0091_institutional_bedstead)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition HouseholdFurnitureAndStorageMakeInstitutionalCastIronWashstandForDomesticFurnishingCraftIndustrialisedHouseholdFurnitureStorage0092InstitutionalWashstand() => new()
	{
		Metadata = new("Household furniture and storage/make institutional cast iron washstand for domestic furnishing [craft_industrialised_household_furniture_storage_0092_institutional_washstand]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_household_furniture_storage_0092_institutional_washstand"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make institutional cast iron washstand for domestic furnishing [craft_industrialised_household_furniture_storage_0092_institutional_washstand]",
		Category = "Household furniture and storage",
		Blurb = "Make an institutional cast iron washstand for domestic furnishing from prepared cast iron stock.",
		Action = "making institutional cast iron washstand for domestic furnishing",
		ActiveCraftItemSdesc = "an in-progress institutional cast iron washstand for domestic furnishing craft",
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
			new CatalogueCommodityRecipeInput(Grams: 4133.0, Material: "cast iron", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("4133 grams of cast iron", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_household_furniture_storage_0092_institutional_washstand", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x an institutional cast iron washstand for domestic furnishing (@industrialised_household_furniture_storage_0092_institutional_washstand)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition HouseholdFurnitureAndStorageMakeInstitutionalOakScreenForDomesticFurnishingCraftIndustrialisedHouseholdFurnitureStorage0093InstitutionalScreen() => new()
	{
		Metadata = new("Household furniture and storage/make institutional oak screen for domestic furnishing [craft_industrialised_household_furniture_storage_0093_institutional_screen]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_household_furniture_storage_0093_institutional_screen"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make institutional oak screen for domestic furnishing [craft_industrialised_household_furniture_storage_0093_institutional_screen]",
		Category = "Household furniture and storage",
		Blurb = "Make an institutional oak screen for domestic furnishing from prepared oak stock.",
		Action = "making institutional oak screen for domestic furnishing",
		ActiveCraftItemSdesc = "an in-progress institutional oak screen for domestic furnishing craft",
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
			new CatalogueCommodityRecipeInput(Grams: 4173.0, Material: "oak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("4173 grams of oak", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_household_furniture_storage_0093_institutional_screen", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x an institutional oak screen for domestic furnishing (@industrialised_household_furniture_storage_0093_institutional_screen)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition HouseholdFurnitureAndStorageMakeInstitutionalMildSteelRackForDomesticFurnishingCraftIndustrialisedHouseholdFurnitureStorage0094InstitutionalRack() => new()
	{
		Metadata = new("Household furniture and storage/make institutional mild steel rack for domestic furnishing [craft_industrialised_household_furniture_storage_0094_institutional_rack]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_household_furniture_storage_0094_institutional_rack"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make institutional mild steel rack for domestic furnishing [craft_industrialised_household_furniture_storage_0094_institutional_rack]",
		Category = "Household furniture and storage",
		Blurb = "Make an institutional mild steel rack for domestic furnishing from prepared mild steel stock.",
		Action = "making institutional mild steel rack for domestic furnishing",
		ActiveCraftItemSdesc = "an in-progress institutional mild steel rack for domestic furnishing craft",
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
			new CatalogueCommodityRecipeInput(Grams: 4214.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("4214 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_household_furniture_storage_0094_institutional_rack", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x an institutional mild steel rack for domestic furnishing (@industrialised_household_furniture_storage_0094_institutional_rack)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition HouseholdFurnitureAndStorageMakeInstitutionalCastIronChestForDomesticFurnishingCraftIndustrialisedHouseholdFurnitureStorage0095InstitutionalChest() => new()
	{
		Metadata = new("Household furniture and storage/make institutional cast iron chest for domestic furnishing [craft_industrialised_household_furniture_storage_0095_institutional_chest]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_household_furniture_storage_0095_institutional_chest"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make institutional cast iron chest for domestic furnishing [craft_industrialised_household_furniture_storage_0095_institutional_chest]",
		Category = "Household furniture and storage",
		Blurb = "Make an institutional cast iron chest for domestic furnishing from prepared cast iron stock.",
		Action = "making institutional cast iron chest for domestic furnishing",
		ActiveCraftItemSdesc = "an in-progress institutional cast iron chest for domestic furnishing craft",
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
			new CatalogueCommodityRecipeInput(Grams: 4255.0, Material: "cast iron", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("4255 grams of cast iron", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_household_furniture_storage_0095_institutional_chest", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x an institutional cast iron chest for domestic furnishing (@industrialised_household_furniture_storage_0095_institutional_chest)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition HouseholdFurnitureAndStorageMakeInstitutionalOakBenchForDomesticFurnishingCraftIndustrialisedHouseholdFurnitureStorage0096InstitutionalBench() => new()
	{
		Metadata = new("Household furniture and storage/make institutional oak bench for domestic furnishing [craft_industrialised_household_furniture_storage_0096_institutional_bench]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_household_furniture_storage_0096_institutional_bench"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make institutional oak bench for domestic furnishing [craft_industrialised_household_furniture_storage_0096_institutional_bench]",
		Category = "Household furniture and storage",
		Blurb = "Make an institutional oak bench for domestic furnishing from prepared oak stock.",
		Action = "making institutional oak bench for domestic furnishing",
		ActiveCraftItemSdesc = "an in-progress institutional oak bench for domestic furnishing craft",
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
			new CatalogueCommodityRecipeInput(Grams: 4296.0, Material: "oak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("4296 grams of oak", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_household_furniture_storage_0096_institutional_bench", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x an institutional oak bench for domestic furnishing (@industrialised_household_furniture_storage_0096_institutional_bench)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition HouseholdFurnitureAndStorageMakeWeatherproofMildSteelScrapForDomesticFurnishingCraftIndustrialisedHouseholdFurnitureStorage0097WeatherproofScrap() => new()
	{
		Metadata = new("Household furniture and storage/make weatherproof mild steel scrap for domestic furnishing [craft_industrialised_household_furniture_storage_0097_weatherproof_scrap]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_household_furniture_storage_0097_weatherproof_scrap"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make weatherproof mild steel scrap for domestic furnishing [craft_industrialised_household_furniture_storage_0097_weatherproof_scrap]",
		Category = "Household furniture and storage",
		Blurb = "Make a weatherproof mild steel scrap for domestic furnishing from prepared mild steel stock.",
		Action = "making weatherproof mild steel scrap for domestic furnishing",
		ActiveCraftItemSdesc = "an in-progress weatherproof mild steel scrap for domestic furnishing craft",
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
			new CatalogueCommodityRecipeInput(Grams: 4336.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("4336 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_household_furniture_storage_0097_weatherproof_scrap", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a weatherproof mild steel scrap for domestic furnishing (@industrialised_household_furniture_storage_0097_weatherproof_scrap)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition HouseholdFurnitureAndStorageMakeWeatherproofCastIronChairForDomesticFurnishingCraftIndustrialisedHouseholdFurnitureStorage0098WeatherproofChair() => new()
	{
		Metadata = new("Household furniture and storage/make weatherproof cast iron chair for domestic furnishing [craft_industrialised_household_furniture_storage_0098_weatherproof_chair]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_household_furniture_storage_0098_weatherproof_chair"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make weatherproof cast iron chair for domestic furnishing [craft_industrialised_household_furniture_storage_0098_weatherproof_chair]",
		Category = "Household furniture and storage",
		Blurb = "Make a weatherproof cast iron chair for domestic furnishing from prepared cast iron stock.",
		Action = "making weatherproof cast iron chair for domestic furnishing",
		ActiveCraftItemSdesc = "an in-progress weatherproof cast iron chair for domestic furnishing craft",
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
			new CatalogueCommodityRecipeInput(Grams: 4377.0, Material: "cast iron", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("4377 grams of cast iron", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_household_furniture_storage_0098_weatherproof_chair", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a weatherproof cast iron chair for domestic furnishing (@industrialised_household_furniture_storage_0098_weatherproof_chair)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition HouseholdFurnitureAndStorageMakeWeatherproofOakStoolForDomesticFurnishingCraftIndustrialisedHouseholdFurnitureStorage0099WeatherproofStool() => new()
	{
		Metadata = new("Household furniture and storage/make weatherproof oak stool for domestic furnishing [craft_industrialised_household_furniture_storage_0099_weatherproof_stool]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_household_furniture_storage_0099_weatherproof_stool"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make weatherproof oak stool for domestic furnishing [craft_industrialised_household_furniture_storage_0099_weatherproof_stool]",
		Category = "Household furniture and storage",
		Blurb = "Make a weatherproof oak stool for domestic furnishing from prepared oak stock.",
		Action = "making weatherproof oak stool for domestic furnishing",
		ActiveCraftItemSdesc = "an in-progress weatherproof oak stool for domestic furnishing craft",
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
			new CatalogueCommodityRecipeInput(Grams: 4418.0, Material: "oak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("4418 grams of oak", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_household_furniture_storage_0099_weatherproof_stool", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a weatherproof oak stool for domestic furnishing (@industrialised_household_furniture_storage_0099_weatherproof_stool)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition HouseholdFurnitureAndStorageMakeWeatherproofMildSteelTableForDomesticFurnishingCraftIndustrialisedHouseholdFurnitureStorage0100WeatherproofTable() => new()
	{
		Metadata = new("Household furniture and storage/make weatherproof mild steel table for domestic furnishing [craft_industrialised_household_furniture_storage_0100_weatherproof_table]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_household_furniture_storage_0100_weatherproof_table"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make weatherproof mild steel table for domestic furnishing [craft_industrialised_household_furniture_storage_0100_weatherproof_table]",
		Category = "Household furniture and storage",
		Blurb = "Make a weatherproof mild steel table for domestic furnishing from prepared mild steel stock.",
		Action = "making weatherproof mild steel table for domestic furnishing",
		ActiveCraftItemSdesc = "an in-progress weatherproof mild steel table for domestic furnishing craft",
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
			new CatalogueCommodityRecipeInput(Grams: 4458.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("4458 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_household_furniture_storage_0100_weatherproof_table", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a weatherproof mild steel table for domestic furnishing (@industrialised_household_furniture_storage_0100_weatherproof_table)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition HouseholdFurnitureAndStorageMakeWeatherproofCastIronCabinetForDomesticFurnishingCraftIndustrialisedHouseholdFurnitureStorage0101WeatherproofCabinet() => new()
	{
		Metadata = new("Household furniture and storage/make weatherproof cast iron cabinet for domestic furnishing [craft_industrialised_household_furniture_storage_0101_weatherproof_cabinet]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_household_furniture_storage_0101_weatherproof_cabinet"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make weatherproof cast iron cabinet for domestic furnishing [craft_industrialised_household_furniture_storage_0101_weatherproof_cabinet]",
		Category = "Household furniture and storage",
		Blurb = "Make a weatherproof cast iron cabinet for domestic furnishing from prepared cast iron stock.",
		Action = "making weatherproof cast iron cabinet for domestic furnishing",
		ActiveCraftItemSdesc = "an in-progress weatherproof cast iron cabinet for domestic furnishing craft",
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
			new CatalogueCommodityRecipeInput(Grams: 4499.0, Material: "cast iron", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("4499 grams of cast iron", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_household_furniture_storage_0101_weatherproof_cabinet", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a weatherproof cast iron cabinet for domestic furnishing (@industrialised_household_furniture_storage_0101_weatherproof_cabinet)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition HouseholdFurnitureAndStorageMakeWeatherproofOakShelfForDomesticFurnishingCraftIndustrialisedHouseholdFurnitureStorage0102WeatherproofShelf() => new()
	{
		Metadata = new("Household furniture and storage/make weatherproof oak shelf for domestic furnishing [craft_industrialised_household_furniture_storage_0102_weatherproof_shelf]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_household_furniture_storage_0102_weatherproof_shelf"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make weatherproof oak shelf for domestic furnishing [craft_industrialised_household_furniture_storage_0102_weatherproof_shelf]",
		Category = "Household furniture and storage",
		Blurb = "Make a weatherproof oak shelf for domestic furnishing from prepared oak stock.",
		Action = "making weatherproof oak shelf for domestic furnishing",
		ActiveCraftItemSdesc = "an in-progress weatherproof oak shelf for domestic furnishing craft",
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
			new CatalogueCommodityRecipeInput(Grams: 4540.0, Material: "oak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("4540 grams of oak", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_household_furniture_storage_0102_weatherproof_shelf", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a weatherproof oak shelf for domestic furnishing (@industrialised_household_furniture_storage_0102_weatherproof_shelf)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition HouseholdFurnitureAndStorageMakeWeatherproofMildSteelBedsteadForDomesticFurnishingCraftIndustrialisedHouseholdFurnitureStorage0103WeatherproofBedstead() => new()
	{
		Metadata = new("Household furniture and storage/make weatherproof mild steel bedstead for domestic furnishing [craft_industrialised_household_furniture_storage_0103_weatherproof_bedstead]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_household_furniture_storage_0103_weatherproof_bedstead"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make weatherproof mild steel bedstead for domestic furnishing [craft_industrialised_household_furniture_storage_0103_weatherproof_bedstead]",
		Category = "Household furniture and storage",
		Blurb = "Make a weatherproof mild steel bedstead for domestic furnishing from prepared mild steel stock.",
		Action = "making weatherproof mild steel bedstead for domestic furnishing",
		ActiveCraftItemSdesc = "an in-progress weatherproof mild steel bedstead for domestic furnishing craft",
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
			new CatalogueCommodityRecipeInput(Grams: 4580.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("4580 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_household_furniture_storage_0103_weatherproof_bedstead", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a weatherproof mild steel bedstead for domestic furnishing (@industrialised_household_furniture_storage_0103_weatherproof_bedstead)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition HouseholdFurnitureAndStorageMakeWeatherproofCastIronWashstandForDomesticFurnishingCraftIndustrialisedHouseholdFurnitureStorage0104WeatherproofWashstand() => new()
	{
		Metadata = new("Household furniture and storage/make weatherproof cast iron washstand for domestic furnishing [craft_industrialised_household_furniture_storage_0104_weatherproof_washstand]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_household_furniture_storage_0104_weatherproof_washstand"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make weatherproof cast iron washstand for domestic furnishing [craft_industrialised_household_furniture_storage_0104_weatherproof_washstand]",
		Category = "Household furniture and storage",
		Blurb = "Make a weatherproof cast iron washstand for domestic furnishing from prepared cast iron stock.",
		Action = "making weatherproof cast iron washstand for domestic furnishing",
		ActiveCraftItemSdesc = "an in-progress weatherproof cast iron washstand for domestic furnishing craft",
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
			new CatalogueCommodityRecipeInput(Grams: 4621.0, Material: "cast iron", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("4621 grams of cast iron", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_household_furniture_storage_0104_weatherproof_washstand", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a weatherproof cast iron washstand for domestic furnishing (@industrialised_household_furniture_storage_0104_weatherproof_washstand)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition HouseholdFurnitureAndStorageMakeWeatherproofOakScreenForDomesticFurnishingCraftIndustrialisedHouseholdFurnitureStorage0105WeatherproofScreen() => new()
	{
		Metadata = new("Household furniture and storage/make weatherproof oak screen for domestic furnishing [craft_industrialised_household_furniture_storage_0105_weatherproof_screen]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_household_furniture_storage_0105_weatherproof_screen"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make weatherproof oak screen for domestic furnishing [craft_industrialised_household_furniture_storage_0105_weatherproof_screen]",
		Category = "Household furniture and storage",
		Blurb = "Make a weatherproof oak screen for domestic furnishing from prepared oak stock.",
		Action = "making weatherproof oak screen for domestic furnishing",
		ActiveCraftItemSdesc = "an in-progress weatherproof oak screen for domestic furnishing craft",
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
			new CatalogueCommodityRecipeInput(Grams: 4662.0, Material: "oak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("4662 grams of oak", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_household_furniture_storage_0105_weatherproof_screen", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a weatherproof oak screen for domestic furnishing (@industrialised_household_furniture_storage_0105_weatherproof_screen)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition HouseholdFurnitureAndStorageMakeWeatherproofMildSteelRackForDomesticFurnishingCraftIndustrialisedHouseholdFurnitureStorage0106WeatherproofRack() => new()
	{
		Metadata = new("Household furniture and storage/make weatherproof mild steel rack for domestic furnishing [craft_industrialised_household_furniture_storage_0106_weatherproof_rack]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_household_furniture_storage_0106_weatherproof_rack"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make weatherproof mild steel rack for domestic furnishing [craft_industrialised_household_furniture_storage_0106_weatherproof_rack]",
		Category = "Household furniture and storage",
		Blurb = "Make a weatherproof mild steel rack for domestic furnishing from prepared mild steel stock.",
		Action = "making weatherproof mild steel rack for domestic furnishing",
		ActiveCraftItemSdesc = "an in-progress weatherproof mild steel rack for domestic furnishing craft",
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
			new CatalogueCommodityRecipeInput(Grams: 4702.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("4702 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_household_furniture_storage_0106_weatherproof_rack", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a weatherproof mild steel rack for domestic furnishing (@industrialised_household_furniture_storage_0106_weatherproof_rack)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition HouseholdFurnitureAndStorageMakeWeatherproofCastIronChestForDomesticFurnishingCraftIndustrialisedHouseholdFurnitureStorage0107WeatherproofChest() => new()
	{
		Metadata = new("Household furniture and storage/make weatherproof cast iron chest for domestic furnishing [craft_industrialised_household_furniture_storage_0107_weatherproof_chest]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_household_furniture_storage_0107_weatherproof_chest"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make weatherproof cast iron chest for domestic furnishing [craft_industrialised_household_furniture_storage_0107_weatherproof_chest]",
		Category = "Household furniture and storage",
		Blurb = "Make a weatherproof cast iron chest for domestic furnishing from prepared cast iron stock.",
		Action = "making weatherproof cast iron chest for domestic furnishing",
		ActiveCraftItemSdesc = "an in-progress weatherproof cast iron chest for domestic furnishing craft",
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
			new CatalogueCommodityRecipeInput(Grams: 4743.0, Material: "cast iron", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("4743 grams of cast iron", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_household_furniture_storage_0107_weatherproof_chest", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a weatherproof cast iron chest for domestic furnishing (@industrialised_household_furniture_storage_0107_weatherproof_chest)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition HouseholdFurnitureAndStorageMakeWeatherproofOakBenchForDomesticFurnishingCraftIndustrialisedHouseholdFurnitureStorage0108WeatherproofBench() => new()
	{
		Metadata = new("Household furniture and storage/make weatherproof oak bench for domestic furnishing [craft_industrialised_household_furniture_storage_0108_weatherproof_bench]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_household_furniture_storage_0108_weatherproof_bench"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make weatherproof oak bench for domestic furnishing [craft_industrialised_household_furniture_storage_0108_weatherproof_bench]",
		Category = "Household furniture and storage",
		Blurb = "Make a weatherproof oak bench for domestic furnishing from prepared oak stock.",
		Action = "making weatherproof oak bench for domestic furnishing",
		ActiveCraftItemSdesc = "an in-progress weatherproof oak bench for domestic furnishing craft",
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
			new CatalogueCommodityRecipeInput(Grams: 4784.0, Material: "oak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("4784 grams of oak", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_household_furniture_storage_0108_weatherproof_bench", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a weatherproof oak bench for domestic furnishing (@industrialised_household_furniture_storage_0108_weatherproof_bench)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition HouseholdFurnitureAndStorageMakeServiceMildSteelScrapForDomesticFurnishingCraftIndustrialisedHouseholdFurnitureStorage0109ServiceScrap() => new()
	{
		Metadata = new("Household furniture and storage/make service mild steel scrap for domestic furnishing [craft_industrialised_household_furniture_storage_0109_service_scrap]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_household_furniture_storage_0109_service_scrap"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make service mild steel scrap for domestic furnishing [craft_industrialised_household_furniture_storage_0109_service_scrap]",
		Category = "Household furniture and storage",
		Blurb = "Make a service mild steel scrap for domestic furnishing from prepared mild steel stock.",
		Action = "making service mild steel scrap for domestic furnishing",
		ActiveCraftItemSdesc = "an in-progress service mild steel scrap for domestic furnishing craft",
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
			new CatalogueCommodityRecipeInput(Grams: 4825.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("4825 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_household_furniture_storage_0109_service_scrap", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a service mild steel scrap for domestic furnishing (@industrialised_household_furniture_storage_0109_service_scrap)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition HouseholdFurnitureAndStorageMakeServiceCastIronChairForDomesticFurnishingCraftIndustrialisedHouseholdFurnitureStorage0110ServiceChair() => new()
	{
		Metadata = new("Household furniture and storage/make service cast iron chair for domestic furnishing [craft_industrialised_household_furniture_storage_0110_service_chair]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_household_furniture_storage_0110_service_chair"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make service cast iron chair for domestic furnishing [craft_industrialised_household_furniture_storage_0110_service_chair]",
		Category = "Household furniture and storage",
		Blurb = "Make a service cast iron chair for domestic furnishing from prepared cast iron stock.",
		Action = "making service cast iron chair for domestic furnishing",
		ActiveCraftItemSdesc = "an in-progress service cast iron chair for domestic furnishing craft",
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
			new CatalogueCommodityRecipeInput(Grams: 4865.0, Material: "cast iron", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("4865 grams of cast iron", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_household_furniture_storage_0110_service_chair", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a service cast iron chair for domestic furnishing (@industrialised_household_furniture_storage_0110_service_chair)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition HouseholdFurnitureAndStorageMakeServiceOakStoolForDomesticFurnishingCraftIndustrialisedHouseholdFurnitureStorage0111ServiceStool() => new()
	{
		Metadata = new("Household furniture and storage/make service oak stool for domestic furnishing [craft_industrialised_household_furniture_storage_0111_service_stool]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_household_furniture_storage_0111_service_stool"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make service oak stool for domestic furnishing [craft_industrialised_household_furniture_storage_0111_service_stool]",
		Category = "Household furniture and storage",
		Blurb = "Make a service oak stool for domestic furnishing from prepared oak stock.",
		Action = "making service oak stool for domestic furnishing",
		ActiveCraftItemSdesc = "an in-progress service oak stool for domestic furnishing craft",
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
			new CatalogueCommodityRecipeInput(Grams: 4906.0, Material: "oak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("4906 grams of oak", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_household_furniture_storage_0111_service_stool", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a service oak stool for domestic furnishing (@industrialised_household_furniture_storage_0111_service_stool)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition HouseholdFurnitureAndStorageMakeServiceMildSteelTableForDomesticFurnishingCraftIndustrialisedHouseholdFurnitureStorage0112ServiceTable() => new()
	{
		Metadata = new("Household furniture and storage/make service mild steel table for domestic furnishing [craft_industrialised_household_furniture_storage_0112_service_table]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_household_furniture_storage_0112_service_table"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make service mild steel table for domestic furnishing [craft_industrialised_household_furniture_storage_0112_service_table]",
		Category = "Household furniture and storage",
		Blurb = "Make a service mild steel table for domestic furnishing from prepared mild steel stock.",
		Action = "making service mild steel table for domestic furnishing",
		ActiveCraftItemSdesc = "an in-progress service mild steel table for domestic furnishing craft",
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
			new CatalogueCommodityRecipeInput(Grams: 4947.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("4947 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_household_furniture_storage_0112_service_table", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a service mild steel table for domestic furnishing (@industrialised_household_furniture_storage_0112_service_table)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition HouseholdFurnitureAndStorageMakeServiceCastIronCabinetForDomesticFurnishingCraftIndustrialisedHouseholdFurnitureStorage0113ServiceCabinet() => new()
	{
		Metadata = new("Household furniture and storage/make service cast iron cabinet for domestic furnishing [craft_industrialised_household_furniture_storage_0113_service_cabinet]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_household_furniture_storage_0113_service_cabinet"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make service cast iron cabinet for domestic furnishing [craft_industrialised_household_furniture_storage_0113_service_cabinet]",
		Category = "Household furniture and storage",
		Blurb = "Make a service cast iron cabinet for domestic furnishing from prepared cast iron stock.",
		Action = "making service cast iron cabinet for domestic furnishing",
		ActiveCraftItemSdesc = "an in-progress service cast iron cabinet for domestic furnishing craft",
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
			new CatalogueCommodityRecipeInput(Grams: 4987.0, Material: "cast iron", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("4987 grams of cast iron", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_household_furniture_storage_0113_service_cabinet", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a service cast iron cabinet for domestic furnishing (@industrialised_household_furniture_storage_0113_service_cabinet)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition HouseholdFurnitureAndStorageMakeServiceOakShelfForDomesticFurnishingCraftIndustrialisedHouseholdFurnitureStorage0114ServiceShelf() => new()
	{
		Metadata = new("Household furniture and storage/make service oak shelf for domestic furnishing [craft_industrialised_household_furniture_storage_0114_service_shelf]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_household_furniture_storage_0114_service_shelf"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make service oak shelf for domestic furnishing [craft_industrialised_household_furniture_storage_0114_service_shelf]",
		Category = "Household furniture and storage",
		Blurb = "Make a service oak shelf for domestic furnishing from prepared oak stock.",
		Action = "making service oak shelf for domestic furnishing",
		ActiveCraftItemSdesc = "an in-progress service oak shelf for domestic furnishing craft",
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
			new CatalogueCommodityRecipeInput(Grams: 5028.0, Material: "oak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("5028 grams of oak", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_household_furniture_storage_0114_service_shelf", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a service oak shelf for domestic furnishing (@industrialised_household_furniture_storage_0114_service_shelf)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition HouseholdFurnitureAndStorageMakeServiceMildSteelBedsteadForDomesticFurnishingCraftIndustrialisedHouseholdFurnitureStorage0115ServiceBedstead() => new()
	{
		Metadata = new("Household furniture and storage/make service mild steel bedstead for domestic furnishing [craft_industrialised_household_furniture_storage_0115_service_bedstead]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_household_furniture_storage_0115_service_bedstead"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make service mild steel bedstead for domestic furnishing [craft_industrialised_household_furniture_storage_0115_service_bedstead]",
		Category = "Household furniture and storage",
		Blurb = "Make a service mild steel bedstead for domestic furnishing from prepared mild steel stock.",
		Action = "making service mild steel bedstead for domestic furnishing",
		ActiveCraftItemSdesc = "an in-progress service mild steel bedstead for domestic furnishing craft",
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
			new CatalogueCommodityRecipeInput(Grams: 5069.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("5069 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_household_furniture_storage_0115_service_bedstead", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a service mild steel bedstead for domestic furnishing (@industrialised_household_furniture_storage_0115_service_bedstead)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition HouseholdFurnitureAndStorageMakeServiceCastIronWashstandForDomesticFurnishingCraftIndustrialisedHouseholdFurnitureStorage0116ServiceWashstand() => new()
	{
		Metadata = new("Household furniture and storage/make service cast iron washstand for domestic furnishing [craft_industrialised_household_furniture_storage_0116_service_washstand]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_household_furniture_storage_0116_service_washstand"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make service cast iron washstand for domestic furnishing [craft_industrialised_household_furniture_storage_0116_service_washstand]",
		Category = "Household furniture and storage",
		Blurb = "Make a service cast iron washstand for domestic furnishing from prepared cast iron stock.",
		Action = "making service cast iron washstand for domestic furnishing",
		ActiveCraftItemSdesc = "an in-progress service cast iron washstand for domestic furnishing craft",
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
			new CatalogueCommodityRecipeInput(Grams: 5110.0, Material: "cast iron", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("5110 grams of cast iron", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_household_furniture_storage_0116_service_washstand", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a service cast iron washstand for domestic furnishing (@industrialised_household_furniture_storage_0116_service_washstand)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition HouseholdFurnitureAndStorageMakeServiceOakScreenForDomesticFurnishingCraftIndustrialisedHouseholdFurnitureStorage0117ServiceScreen() => new()
	{
		Metadata = new("Household furniture and storage/make service oak screen for domestic furnishing [craft_industrialised_household_furniture_storage_0117_service_screen]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_household_furniture_storage_0117_service_screen"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make service oak screen for domestic furnishing [craft_industrialised_household_furniture_storage_0117_service_screen]",
		Category = "Household furniture and storage",
		Blurb = "Make a service oak screen for domestic furnishing from prepared oak stock.",
		Action = "making service oak screen for domestic furnishing",
		ActiveCraftItemSdesc = "an in-progress service oak screen for domestic furnishing craft",
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
			new CatalogueCommodityRecipeInput(Grams: 5150.0, Material: "oak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("5150 grams of oak", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_household_furniture_storage_0117_service_screen", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a service oak screen for domestic furnishing (@industrialised_household_furniture_storage_0117_service_screen)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition HouseholdFurnitureAndStorageMakeServiceMildSteelRackForDomesticFurnishingCraftIndustrialisedHouseholdFurnitureStorage0118ServiceRack() => new()
	{
		Metadata = new("Household furniture and storage/make service mild steel rack for domestic furnishing [craft_industrialised_household_furniture_storage_0118_service_rack]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_household_furniture_storage_0118_service_rack"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make service mild steel rack for domestic furnishing [craft_industrialised_household_furniture_storage_0118_service_rack]",
		Category = "Household furniture and storage",
		Blurb = "Make a service mild steel rack for domestic furnishing from prepared mild steel stock.",
		Action = "making service mild steel rack for domestic furnishing",
		ActiveCraftItemSdesc = "an in-progress service mild steel rack for domestic furnishing craft",
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
			new CatalogueCommodityRecipeInput(Grams: 5191.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("5191 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_household_furniture_storage_0118_service_rack", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a service mild steel rack for domestic furnishing (@industrialised_household_furniture_storage_0118_service_rack)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition HouseholdFurnitureAndStorageMakeServiceCastIronChestForDomesticFurnishingCraftIndustrialisedHouseholdFurnitureStorage0119ServiceChest() => new()
	{
		Metadata = new("Household furniture and storage/make service cast iron chest for domestic furnishing [craft_industrialised_household_furniture_storage_0119_service_chest]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_household_furniture_storage_0119_service_chest"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make service cast iron chest for domestic furnishing [craft_industrialised_household_furniture_storage_0119_service_chest]",
		Category = "Household furniture and storage",
		Blurb = "Make a service cast iron chest for domestic furnishing from prepared cast iron stock.",
		Action = "making service cast iron chest for domestic furnishing",
		ActiveCraftItemSdesc = "an in-progress service cast iron chest for domestic furnishing craft",
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
			new CatalogueCommodityRecipeInput(Grams: 5232.0, Material: "cast iron", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("5232 grams of cast iron", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_household_furniture_storage_0119_service_chest", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a service cast iron chest for domestic furnishing (@industrialised_household_furniture_storage_0119_service_chest)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition HouseholdFurnitureAndStorageMakeServiceOakBenchForDomesticFurnishingCraftIndustrialisedHouseholdFurnitureStorage0120ServiceBench() => new()
	{
		Metadata = new("Household furniture and storage/make service oak bench for domestic furnishing [craft_industrialised_household_furniture_storage_0120_service_bench]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_household_furniture_storage_0120_service_bench"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make service oak bench for domestic furnishing [craft_industrialised_household_furniture_storage_0120_service_bench]",
		Category = "Household furniture and storage",
		Blurb = "Make a service oak bench for domestic furnishing from prepared oak stock.",
		Action = "making service oak bench for domestic furnishing",
		ActiveCraftItemSdesc = "an in-progress service oak bench for domestic furnishing craft",
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
			new CatalogueCommodityRecipeInput(Grams: 5272.0, Material: "oak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("5272 grams of oak", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_household_furniture_storage_0120_service_bench", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a service oak bench for domestic furnishing (@industrialised_household_furniture_storage_0120_service_bench)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition HouseholdFurnitureAndStorageMakePrecisionMildSteelScrapForDomesticFurnishingCraftIndustrialisedHouseholdFurnitureStorage0121PrecisionScrap() => new()
	{
		Metadata = new("Household furniture and storage/make precision mild steel scrap for domestic furnishing [craft_industrialised_household_furniture_storage_0121_precision_scrap]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_household_furniture_storage_0121_precision_scrap"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make precision mild steel scrap for domestic furnishing [craft_industrialised_household_furniture_storage_0121_precision_scrap]",
		Category = "Household furniture and storage",
		Blurb = "Make a precision mild steel scrap for domestic furnishing from prepared mild steel stock.",
		Action = "making precision mild steel scrap for domestic furnishing",
		ActiveCraftItemSdesc = "an in-progress precision mild steel scrap for domestic furnishing craft",
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
			new CatalogueCommodityRecipeInput(Grams: 5313.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("5313 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_household_furniture_storage_0121_precision_scrap", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a precision mild steel scrap for domestic furnishing (@industrialised_household_furniture_storage_0121_precision_scrap)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition HouseholdFurnitureAndStorageMakePrecisionCastIronChairForDomesticFurnishingCraftIndustrialisedHouseholdFurnitureStorage0122PrecisionChair() => new()
	{
		Metadata = new("Household furniture and storage/make precision cast iron chair for domestic furnishing [craft_industrialised_household_furniture_storage_0122_precision_chair]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_household_furniture_storage_0122_precision_chair"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make precision cast iron chair for domestic furnishing [craft_industrialised_household_furniture_storage_0122_precision_chair]",
		Category = "Household furniture and storage",
		Blurb = "Make a precision cast iron chair for domestic furnishing from prepared cast iron stock.",
		Action = "making precision cast iron chair for domestic furnishing",
		ActiveCraftItemSdesc = "an in-progress precision cast iron chair for domestic furnishing craft",
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
			new CatalogueCommodityRecipeInput(Grams: 5354.0, Material: "cast iron", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("5354 grams of cast iron", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_household_furniture_storage_0122_precision_chair", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a precision cast iron chair for domestic furnishing (@industrialised_household_furniture_storage_0122_precision_chair)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition HouseholdFurnitureAndStorageMakePrecisionOakStoolForDomesticFurnishingCraftIndustrialisedHouseholdFurnitureStorage0123PrecisionStool() => new()
	{
		Metadata = new("Household furniture and storage/make precision oak stool for domestic furnishing [craft_industrialised_household_furniture_storage_0123_precision_stool]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_household_furniture_storage_0123_precision_stool"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make precision oak stool for domestic furnishing [craft_industrialised_household_furniture_storage_0123_precision_stool]",
		Category = "Household furniture and storage",
		Blurb = "Make a precision oak stool for domestic furnishing from prepared oak stock.",
		Action = "making precision oak stool for domestic furnishing",
		ActiveCraftItemSdesc = "an in-progress precision oak stool for domestic furnishing craft",
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
			new CatalogueCommodityRecipeInput(Grams: 5394.0, Material: "oak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("5394 grams of oak", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_household_furniture_storage_0123_precision_stool", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a precision oak stool for domestic furnishing (@industrialised_household_furniture_storage_0123_precision_stool)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition HouseholdFurnitureAndStorageMakePrecisionMildSteelTableForDomesticFurnishingCraftIndustrialisedHouseholdFurnitureStorage0124PrecisionTable() => new()
	{
		Metadata = new("Household furniture and storage/make precision mild steel table for domestic furnishing [craft_industrialised_household_furniture_storage_0124_precision_table]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_household_furniture_storage_0124_precision_table"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make precision mild steel table for domestic furnishing [craft_industrialised_household_furniture_storage_0124_precision_table]",
		Category = "Household furniture and storage",
		Blurb = "Make a precision mild steel table for domestic furnishing from prepared mild steel stock.",
		Action = "making precision mild steel table for domestic furnishing",
		ActiveCraftItemSdesc = "an in-progress precision mild steel table for domestic furnishing craft",
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
			new CatalogueCommodityRecipeInput(Grams: 5435.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("5435 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_household_furniture_storage_0124_precision_table", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a precision mild steel table for domestic furnishing (@industrialised_household_furniture_storage_0124_precision_table)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition HouseholdFurnitureAndStorageMakePrecisionCastIronCabinetForDomesticFurnishingCraftIndustrialisedHouseholdFurnitureStorage0125PrecisionCabinet() => new()
	{
		Metadata = new("Household furniture and storage/make precision cast iron cabinet for domestic furnishing [craft_industrialised_household_furniture_storage_0125_precision_cabinet]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_household_furniture_storage_0125_precision_cabinet"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make precision cast iron cabinet for domestic furnishing [craft_industrialised_household_furniture_storage_0125_precision_cabinet]",
		Category = "Household furniture and storage",
		Blurb = "Make a precision cast iron cabinet for domestic furnishing from prepared cast iron stock.",
		Action = "making precision cast iron cabinet for domestic furnishing",
		ActiveCraftItemSdesc = "an in-progress precision cast iron cabinet for domestic furnishing craft",
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
			new CatalogueCommodityRecipeInput(Grams: 5476.0, Material: "cast iron", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("5476 grams of cast iron", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_household_furniture_storage_0125_precision_cabinet", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a precision cast iron cabinet for domestic furnishing (@industrialised_household_furniture_storage_0125_precision_cabinet)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition HouseholdFurnitureAndStorageMakePrecisionOakShelfForDomesticFurnishingCraftIndustrialisedHouseholdFurnitureStorage0126PrecisionShelf() => new()
	{
		Metadata = new("Household furniture and storage/make precision oak shelf for domestic furnishing [craft_industrialised_household_furniture_storage_0126_precision_shelf]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_household_furniture_storage_0126_precision_shelf"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make precision oak shelf for domestic furnishing [craft_industrialised_household_furniture_storage_0126_precision_shelf]",
		Category = "Household furniture and storage",
		Blurb = "Make a precision oak shelf for domestic furnishing from prepared oak stock.",
		Action = "making precision oak shelf for domestic furnishing",
		ActiveCraftItemSdesc = "an in-progress precision oak shelf for domestic furnishing craft",
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
			new CatalogueCommodityRecipeInput(Grams: 72.0, Material: "oak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("72 grams of oak", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_household_furniture_storage_0126_precision_shelf", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a precision oak shelf for domestic furnishing (@industrialised_household_furniture_storage_0126_precision_shelf)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition HouseholdFurnitureAndStorageMakePrecisionMildSteelBedsteadForDomesticFurnishingCraftIndustrialisedHouseholdFurnitureStorage0127PrecisionBedstead() => new()
	{
		Metadata = new("Household furniture and storage/make precision mild steel bedstead for domestic furnishing [craft_industrialised_household_furniture_storage_0127_precision_bedstead]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_household_furniture_storage_0127_precision_bedstead"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make precision mild steel bedstead for domestic furnishing [craft_industrialised_household_furniture_storage_0127_precision_bedstead]",
		Category = "Household furniture and storage",
		Blurb = "Make a precision mild steel bedstead for domestic furnishing from prepared mild steel stock.",
		Action = "making precision mild steel bedstead for domestic furnishing",
		ActiveCraftItemSdesc = "an in-progress precision mild steel bedstead for domestic furnishing craft",
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
			new CatalogueCommodityRecipeInput(Grams: 112.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("112 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_household_furniture_storage_0127_precision_bedstead", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a precision mild steel bedstead for domestic furnishing (@industrialised_household_furniture_storage_0127_precision_bedstead)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition HouseholdFurnitureAndStorageMakePrecisionCastIronWashstandForDomesticFurnishingCraftIndustrialisedHouseholdFurnitureStorage0128PrecisionWashstand() => new()
	{
		Metadata = new("Household furniture and storage/make precision cast iron washstand for domestic furnishing [craft_industrialised_household_furniture_storage_0128_precision_washstand]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_household_furniture_storage_0128_precision_washstand"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make precision cast iron washstand for domestic furnishing [craft_industrialised_household_furniture_storage_0128_precision_washstand]",
		Category = "Household furniture and storage",
		Blurb = "Make a precision cast iron washstand for domestic furnishing from prepared cast iron stock.",
		Action = "making precision cast iron washstand for domestic furnishing",
		ActiveCraftItemSdesc = "an in-progress precision cast iron washstand for domestic furnishing craft",
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
			new CatalogueCommodityRecipeInput(Grams: 153.0, Material: "cast iron", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("153 grams of cast iron", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_household_furniture_storage_0128_precision_washstand", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a precision cast iron washstand for domestic furnishing (@industrialised_household_furniture_storage_0128_precision_washstand)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition HouseholdFurnitureAndStorageMakePrecisionOakScreenForDomesticFurnishingCraftIndustrialisedHouseholdFurnitureStorage0129PrecisionScreen() => new()
	{
		Metadata = new("Household furniture and storage/make precision oak screen for domestic furnishing [craft_industrialised_household_furniture_storage_0129_precision_screen]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_household_furniture_storage_0129_precision_screen"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make precision oak screen for domestic furnishing [craft_industrialised_household_furniture_storage_0129_precision_screen]",
		Category = "Household furniture and storage",
		Blurb = "Make a precision oak screen for domestic furnishing from prepared oak stock.",
		Action = "making precision oak screen for domestic furnishing",
		ActiveCraftItemSdesc = "an in-progress precision oak screen for domestic furnishing craft",
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
			new CatalogueCommodityRecipeInput(Grams: 194.0, Material: "oak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("194 grams of oak", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_household_furniture_storage_0129_precision_screen", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a precision oak screen for domestic furnishing (@industrialised_household_furniture_storage_0129_precision_screen)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition HouseholdFurnitureAndStorageMakePrecisionMildSteelRackForDomesticFurnishingCraftIndustrialisedHouseholdFurnitureStorage0130PrecisionRack() => new()
	{
		Metadata = new("Household furniture and storage/make precision mild steel rack for domestic furnishing [craft_industrialised_household_furniture_storage_0130_precision_rack]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_household_furniture_storage_0130_precision_rack"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make precision mild steel rack for domestic furnishing [craft_industrialised_household_furniture_storage_0130_precision_rack]",
		Category = "Household furniture and storage",
		Blurb = "Make a precision mild steel rack for domestic furnishing from prepared mild steel stock.",
		Action = "making precision mild steel rack for domestic furnishing",
		ActiveCraftItemSdesc = "an in-progress precision mild steel rack for domestic furnishing craft",
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
			new CatalogueCommodityRecipeInput(Grams: 234.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("234 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_household_furniture_storage_0130_precision_rack", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a precision mild steel rack for domestic furnishing (@industrialised_household_furniture_storage_0130_precision_rack)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition HouseholdFurnitureAndStorageMakePrecisionCastIronChestForDomesticFurnishingCraftIndustrialisedHouseholdFurnitureStorage0131PrecisionChest() => new()
	{
		Metadata = new("Household furniture and storage/make precision cast iron chest for domestic furnishing [craft_industrialised_household_furniture_storage_0131_precision_chest]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_household_furniture_storage_0131_precision_chest"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make precision cast iron chest for domestic furnishing [craft_industrialised_household_furniture_storage_0131_precision_chest]",
		Category = "Household furniture and storage",
		Blurb = "Make a precision cast iron chest for domestic furnishing from prepared cast iron stock.",
		Action = "making precision cast iron chest for domestic furnishing",
		ActiveCraftItemSdesc = "an in-progress precision cast iron chest for domestic furnishing craft",
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
			new CatalogueCommodityRecipeInput(Grams: 275.0, Material: "cast iron", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("275 grams of cast iron", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_household_furniture_storage_0131_precision_chest", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a precision cast iron chest for domestic furnishing (@industrialised_household_furniture_storage_0131_precision_chest)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition HouseholdFurnitureAndStorageMakePrecisionOakBenchForDomesticFurnishingCraftIndustrialisedHouseholdFurnitureStorage0132PrecisionBench() => new()
	{
		Metadata = new("Household furniture and storage/make precision oak bench for domestic furnishing [craft_industrialised_household_furniture_storage_0132_precision_bench]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_household_furniture_storage_0132_precision_bench"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make precision oak bench for domestic furnishing [craft_industrialised_household_furniture_storage_0132_precision_bench]",
		Category = "Household furniture and storage",
		Blurb = "Make a precision oak bench for domestic furnishing from prepared oak stock.",
		Action = "making precision oak bench for domestic furnishing",
		ActiveCraftItemSdesc = "an in-progress precision oak bench for domestic furnishing craft",
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
			new CatalogueCommodityRecipeInput(Grams: 316.0, Material: "oak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("316 grams of oak", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_household_furniture_storage_0132_precision_bench", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a precision oak bench for domestic furnishing (@industrialised_household_furniture_storage_0132_precision_bench)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition HouseholdFurnitureAndStorageMakeEconomyMildSteelScrapForDomesticFurnishingCraftIndustrialisedHouseholdFurnitureStorage0133EconomyScrap() => new()
	{
		Metadata = new("Household furniture and storage/make economy mild steel scrap for domestic furnishing [craft_industrialised_household_furniture_storage_0133_economy_scrap]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_household_furniture_storage_0133_economy_scrap"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make economy mild steel scrap for domestic furnishing [craft_industrialised_household_furniture_storage_0133_economy_scrap]",
		Category = "Household furniture and storage",
		Blurb = "Make an economy mild steel scrap for domestic furnishing from prepared mild steel stock.",
		Action = "making economy mild steel scrap for domestic furnishing",
		ActiveCraftItemSdesc = "an in-progress economy mild steel scrap for domestic furnishing craft",
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
			new CatalogueCommodityRecipeInput(Grams: 356.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("356 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_household_furniture_storage_0133_economy_scrap", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x an economy mild steel scrap for domestic furnishing (@industrialised_household_furniture_storage_0133_economy_scrap)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition HouseholdFurnitureAndStorageMakeEconomyCastIronChairForDomesticFurnishingCraftIndustrialisedHouseholdFurnitureStorage0134EconomyChair() => new()
	{
		Metadata = new("Household furniture and storage/make economy cast iron chair for domestic furnishing [craft_industrialised_household_furniture_storage_0134_economy_chair]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_household_furniture_storage_0134_economy_chair"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make economy cast iron chair for domestic furnishing [craft_industrialised_household_furniture_storage_0134_economy_chair]",
		Category = "Household furniture and storage",
		Blurb = "Make an economy cast iron chair for domestic furnishing from prepared cast iron stock.",
		Action = "making economy cast iron chair for domestic furnishing",
		ActiveCraftItemSdesc = "an in-progress economy cast iron chair for domestic furnishing craft",
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
			new CatalogueCommodityRecipeInput(Grams: 397.0, Material: "cast iron", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("397 grams of cast iron", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_household_furniture_storage_0134_economy_chair", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x an economy cast iron chair for domestic furnishing (@industrialised_household_furniture_storage_0134_economy_chair)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition HouseholdFurnitureAndStorageMakeEconomyOakStoolForDomesticFurnishingCraftIndustrialisedHouseholdFurnitureStorage0135EconomyStool() => new()
	{
		Metadata = new("Household furniture and storage/make economy oak stool for domestic furnishing [craft_industrialised_household_furniture_storage_0135_economy_stool]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_household_furniture_storage_0135_economy_stool"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make economy oak stool for domestic furnishing [craft_industrialised_household_furniture_storage_0135_economy_stool]",
		Category = "Household furniture and storage",
		Blurb = "Make an economy oak stool for domestic furnishing from prepared oak stock.",
		Action = "making economy oak stool for domestic furnishing",
		ActiveCraftItemSdesc = "an in-progress economy oak stool for domestic furnishing craft",
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
			new CatalogueCommodityRecipeInput(Grams: 438.0, Material: "oak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("438 grams of oak", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_household_furniture_storage_0135_economy_stool", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x an economy oak stool for domestic furnishing (@industrialised_household_furniture_storage_0135_economy_stool)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition HouseholdFurnitureAndStorageMakeEconomyMildSteelTableForDomesticFurnishingCraftIndustrialisedHouseholdFurnitureStorage0136EconomyTable() => new()
	{
		Metadata = new("Household furniture and storage/make economy mild steel table for domestic furnishing [craft_industrialised_household_furniture_storage_0136_economy_table]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_household_furniture_storage_0136_economy_table"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make economy mild steel table for domestic furnishing [craft_industrialised_household_furniture_storage_0136_economy_table]",
		Category = "Household furniture and storage",
		Blurb = "Make an economy mild steel table for domestic furnishing from prepared mild steel stock.",
		Action = "making economy mild steel table for domestic furnishing",
		ActiveCraftItemSdesc = "an in-progress economy mild steel table for domestic furnishing craft",
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
			new CatalogueCommodityRecipeInput(Grams: 479.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("479 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_household_furniture_storage_0136_economy_table", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x an economy mild steel table for domestic furnishing (@industrialised_household_furniture_storage_0136_economy_table)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition HouseholdFurnitureAndStorageMakeEconomyCastIronCabinetForDomesticFurnishingCraftIndustrialisedHouseholdFurnitureStorage0137EconomyCabinet() => new()
	{
		Metadata = new("Household furniture and storage/make economy cast iron cabinet for domestic furnishing [craft_industrialised_household_furniture_storage_0137_economy_cabinet]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_household_furniture_storage_0137_economy_cabinet"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make economy cast iron cabinet for domestic furnishing [craft_industrialised_household_furniture_storage_0137_economy_cabinet]",
		Category = "Household furniture and storage",
		Blurb = "Make an economy cast iron cabinet for domestic furnishing from prepared cast iron stock.",
		Action = "making economy cast iron cabinet for domestic furnishing",
		ActiveCraftItemSdesc = "an in-progress economy cast iron cabinet for domestic furnishing craft",
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
			new CatalogueCommodityRecipeInput(Grams: 519.0, Material: "cast iron", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("519 grams of cast iron", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_household_furniture_storage_0137_economy_cabinet", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x an economy cast iron cabinet for domestic furnishing (@industrialised_household_furniture_storage_0137_economy_cabinet)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition HouseholdFurnitureAndStorageMakeEconomyOakShelfForDomesticFurnishingCraftIndustrialisedHouseholdFurnitureStorage0138EconomyShelf() => new()
	{
		Metadata = new("Household furniture and storage/make economy oak shelf for domestic furnishing [craft_industrialised_household_furniture_storage_0138_economy_shelf]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_household_furniture_storage_0138_economy_shelf"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make economy oak shelf for domestic furnishing [craft_industrialised_household_furniture_storage_0138_economy_shelf]",
		Category = "Household furniture and storage",
		Blurb = "Make an economy oak shelf for domestic furnishing from prepared oak stock.",
		Action = "making economy oak shelf for domestic furnishing",
		ActiveCraftItemSdesc = "an in-progress economy oak shelf for domestic furnishing craft",
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
			new CatalogueCommodityRecipeInput(Grams: 560.0, Material: "oak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("560 grams of oak", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_household_furniture_storage_0138_economy_shelf", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x an economy oak shelf for domestic furnishing (@industrialised_household_furniture_storage_0138_economy_shelf)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition HouseholdFurnitureAndStorageMakeEconomyMildSteelBedsteadForDomesticFurnishingCraftIndustrialisedHouseholdFurnitureStorage0139EconomyBedstead() => new()
	{
		Metadata = new("Household furniture and storage/make economy mild steel bedstead for domestic furnishing [craft_industrialised_household_furniture_storage_0139_economy_bedstead]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_household_furniture_storage_0139_economy_bedstead"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make economy mild steel bedstead for domestic furnishing [craft_industrialised_household_furniture_storage_0139_economy_bedstead]",
		Category = "Household furniture and storage",
		Blurb = "Make an economy mild steel bedstead for domestic furnishing from prepared mild steel stock.",
		Action = "making economy mild steel bedstead for domestic furnishing",
		ActiveCraftItemSdesc = "an in-progress economy mild steel bedstead for domestic furnishing craft",
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
			new CatalogueCommodityRecipeInput(Grams: 601.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("601 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_household_furniture_storage_0139_economy_bedstead", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x an economy mild steel bedstead for domestic furnishing (@industrialised_household_furniture_storage_0139_economy_bedstead)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition HouseholdFurnitureAndStorageMakeEconomyCastIronWashstandForDomesticFurnishingCraftIndustrialisedHouseholdFurnitureStorage0140EconomyWashstand() => new()
	{
		Metadata = new("Household furniture and storage/make economy cast iron washstand for domestic furnishing [craft_industrialised_household_furniture_storage_0140_economy_washstand]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_household_furniture_storage_0140_economy_washstand"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make economy cast iron washstand for domestic furnishing [craft_industrialised_household_furniture_storage_0140_economy_washstand]",
		Category = "Household furniture and storage",
		Blurb = "Make an economy cast iron washstand for domestic furnishing from prepared cast iron stock.",
		Action = "making economy cast iron washstand for domestic furnishing",
		ActiveCraftItemSdesc = "an in-progress economy cast iron washstand for domestic furnishing craft",
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
			new CatalogueCommodityRecipeInput(Grams: 641.0, Material: "cast iron", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("641 grams of cast iron", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_household_furniture_storage_0140_economy_washstand", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x an economy cast iron washstand for domestic furnishing (@industrialised_household_furniture_storage_0140_economy_washstand)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition HouseholdFurnitureAndStorageMakeEconomyOakScreenForDomesticFurnishingCraftIndustrialisedHouseholdFurnitureStorage0141EconomyScreen() => new()
	{
		Metadata = new("Household furniture and storage/make economy oak screen for domestic furnishing [craft_industrialised_household_furniture_storage_0141_economy_screen]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_household_furniture_storage_0141_economy_screen"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make economy oak screen for domestic furnishing [craft_industrialised_household_furniture_storage_0141_economy_screen]",
		Category = "Household furniture and storage",
		Blurb = "Make an economy oak screen for domestic furnishing from prepared oak stock.",
		Action = "making economy oak screen for domestic furnishing",
		ActiveCraftItemSdesc = "an in-progress economy oak screen for domestic furnishing craft",
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
			new CatalogueCommodityRecipeInput(Grams: 682.0, Material: "oak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("682 grams of oak", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_household_furniture_storage_0141_economy_screen", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x an economy oak screen for domestic furnishing (@industrialised_household_furniture_storage_0141_economy_screen)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};
}
