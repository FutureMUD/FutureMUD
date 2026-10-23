#nullable enable

using MudSharp.Body;
using MudSharp.GameItems.Inventory.Plans;
using MudSharp.RPG.Checks;

namespace DatabaseSeeder.Seeders;

public static partial class Recipes22Stock
{
	public static CatalogueRecipeDefinition AgricultureForestryAndFishingMakePortableMildSteelScrapForDomesticHusbandryCraftIndustrialisedAgricultureForestryFishing0051PortableScrap() => new()
	{
		Metadata = new("Agriculture, forestry and fishing/make portable mild steel scrap for domestic husbandry [craft_industrialised_agriculture_forestry_fishing_0051_portable_scrap]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_agriculture_forestry_fishing_0051_portable_scrap"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make portable mild steel scrap for domestic husbandry [craft_industrialised_agriculture_forestry_fishing_0051_portable_scrap]",
		Category = "Agriculture, forestry and fishing",
		Blurb = "Make a portable mild steel scrap for domestic husbandry from prepared mild steel stock.",
		Action = "making portable mild steel scrap for domestic husbandry",
		ActiveCraftItemSdesc = "an in-progress portable mild steel scrap for domestic husbandry craft",
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
			new CatalogueCommodityRecipeInput(Grams: 4019.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("4019 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_agriculture_forestry_fishing_0051_portable_scrap", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a portable mild steel scrap for domestic husbandry (@industrialised_agriculture_forestry_fishing_0051_portable_scrap)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition AgricultureForestryAndFishingMakePortableOakHoeForDomesticHusbandryCraftIndustrialisedAgricultureForestryFishing0052PortableHoe() => new()
	{
		Metadata = new("Agriculture, forestry and fishing/make portable oak hoe for domestic husbandry [craft_industrialised_agriculture_forestry_fishing_0052_portable_hoe]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_agriculture_forestry_fishing_0052_portable_hoe"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make portable oak hoe for domestic husbandry [craft_industrialised_agriculture_forestry_fishing_0052_portable_hoe]",
		Category = "Agriculture, forestry and fishing",
		Blurb = "Make a portable oak hoe for domestic husbandry from prepared oak stock.",
		Action = "making portable oak hoe for domestic husbandry",
		ActiveCraftItemSdesc = "an in-progress portable oak hoe for domestic husbandry craft",
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
			new CatalogueCommodityRecipeInput(Grams: 4060.0, Material: "oak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("4060 grams of oak", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_agriculture_forestry_fishing_0052_portable_hoe", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a portable oak hoe for domestic husbandry (@industrialised_agriculture_forestry_fishing_0052_portable_hoe)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition AgricultureForestryAndFishingMakePortableCottonRakeForDomesticHusbandryCraftIndustrialisedAgricultureForestryFishing0053PortableRake() => new()
	{
		Metadata = new("Agriculture, forestry and fishing/make portable cotton rake for domestic husbandry [craft_industrialised_agriculture_forestry_fishing_0053_portable_rake]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_agriculture_forestry_fishing_0053_portable_rake"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make portable cotton rake for domestic husbandry [craft_industrialised_agriculture_forestry_fishing_0053_portable_rake]",
		Category = "Agriculture, forestry and fishing",
		Blurb = "Make a portable cotton rake for domestic husbandry from prepared cotton stock.",
		Action = "making portable cotton rake for domestic husbandry",
		ActiveCraftItemSdesc = "an in-progress portable cotton rake for domestic husbandry craft",
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
			new CatalogueCommodityRecipeInput(Grams: 4101.0, Material: "cotton", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("4101 grams of cotton", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_agriculture_forestry_fishing_0053_portable_rake", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a portable cotton rake for domestic husbandry (@industrialised_agriculture_forestry_fishing_0053_portable_rake)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition AgricultureForestryAndFishingMakePortableLeatherForkForDomesticHusbandryCraftIndustrialisedAgricultureForestryFishing0054PortableFork() => new()
	{
		Metadata = new("Agriculture, forestry and fishing/make portable leather fork for domestic husbandry [craft_industrialised_agriculture_forestry_fishing_0054_portable_fork]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_agriculture_forestry_fishing_0054_portable_fork"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make portable leather fork for domestic husbandry [craft_industrialised_agriculture_forestry_fishing_0054_portable_fork]",
		Category = "Agriculture, forestry and fishing",
		Blurb = "Make a portable leather fork for domestic husbandry from prepared leather stock.",
		Action = "making portable leather fork for domestic husbandry",
		ActiveCraftItemSdesc = "an in-progress portable leather fork for domestic husbandry craft",
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
			new CatalogueCommodityRecipeInput(Grams: 4142.0, Material: "leather", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("4142 grams of leather", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_agriculture_forestry_fishing_0054_portable_fork", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a portable leather fork for domestic husbandry (@industrialised_agriculture_forestry_fishing_0054_portable_fork)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition AgricultureForestryAndFishingMakeFieldMildSteelScrapForDomesticHusbandryCraftIndustrialAgricultureForestryFishing0001FieldScrap() => new()
	{
		Metadata = new("Agriculture, forestry and fishing/make field mild steel scrap for domestic husbandry [craft_industrial_agriculture_forestry_fishing_0001_field_scrap]", "crafts", ["industrial"], ["item:industrial_agriculture_forestry_fishing_0001_field_scrap"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make field mild steel scrap for domestic husbandry [craft_industrial_agriculture_forestry_fishing_0001_field_scrap]",
		Category = "Agriculture, forestry and fishing",
		Blurb = "Make a field mild steel scrap for domestic husbandry from prepared mild steel stock.",
		Action = "making field mild steel scrap for domestic husbandry",
		ActiveCraftItemSdesc = "an in-progress field mild steel scrap for domestic husbandry craft",
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
			new CatalogueCommodityRecipeInput(Grams: 1984.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1984 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrial_agriculture_forestry_fishing_0001_field_scrap", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a field mild steel scrap for domestic husbandry (@industrial_agriculture_forestry_fishing_0001_field_scrap)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition AgricultureForestryAndFishingMakeFieldOakHoeForDomesticHusbandryCraftIndustrialAgricultureForestryFishing0002FieldHoe() => new()
	{
		Metadata = new("Agriculture, forestry and fishing/make field oak hoe for domestic husbandry [craft_industrial_agriculture_forestry_fishing_0002_field_hoe]", "crafts", ["industrial"], ["item:industrial_agriculture_forestry_fishing_0002_field_hoe"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make field oak hoe for domestic husbandry [craft_industrial_agriculture_forestry_fishing_0002_field_hoe]",
		Category = "Agriculture, forestry and fishing",
		Blurb = "Make a field oak hoe for domestic husbandry from prepared oak stock.",
		Action = "making field oak hoe for domestic husbandry",
		ActiveCraftItemSdesc = "an in-progress field oak hoe for domestic husbandry craft",
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
			new CatalogueCommodityRecipeInput(Grams: 2025.0, Material: "oak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2025 grams of oak", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrial_agriculture_forestry_fishing_0002_field_hoe", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a field oak hoe for domestic husbandry (@industrial_agriculture_forestry_fishing_0002_field_hoe)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition AgricultureForestryAndFishingMakeFieldCottonRakeForDomesticHusbandryCraftIndustrialAgricultureForestryFishing0003FieldRake() => new()
	{
		Metadata = new("Agriculture, forestry and fishing/make field cotton rake for domestic husbandry [craft_industrial_agriculture_forestry_fishing_0003_field_rake]", "crafts", ["industrial"], ["item:industrial_agriculture_forestry_fishing_0003_field_rake"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make field cotton rake for domestic husbandry [craft_industrial_agriculture_forestry_fishing_0003_field_rake]",
		Category = "Agriculture, forestry and fishing",
		Blurb = "Make a field cotton rake for domestic husbandry from prepared cotton stock.",
		Action = "making field cotton rake for domestic husbandry",
		ActiveCraftItemSdesc = "an in-progress field cotton rake for domestic husbandry craft",
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
			new CatalogueCommodityRecipeInput(Grams: 2066.0, Material: "cotton", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2066 grams of cotton", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrial_agriculture_forestry_fishing_0003_field_rake", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a field cotton rake for domestic husbandry (@industrial_agriculture_forestry_fishing_0003_field_rake)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition AgricultureForestryAndFishingMakeFieldLeatherForkForDomesticHusbandryCraftIndustrialAgricultureForestryFishing0004FieldFork() => new()
	{
		Metadata = new("Agriculture, forestry and fishing/make field leather fork for domestic husbandry [craft_industrial_agriculture_forestry_fishing_0004_field_fork]", "crafts", ["industrial"], ["item:industrial_agriculture_forestry_fishing_0004_field_fork"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make field leather fork for domestic husbandry [craft_industrial_agriculture_forestry_fishing_0004_field_fork]",
		Category = "Agriculture, forestry and fishing",
		Blurb = "Make a field leather fork for domestic husbandry from prepared leather stock.",
		Action = "making field leather fork for domestic husbandry",
		ActiveCraftItemSdesc = "an in-progress field leather fork for domestic husbandry craft",
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
			new CatalogueCommodityRecipeInput(Grams: 2106.0, Material: "leather", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2106 grams of leather", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrial_agriculture_forestry_fishing_0004_field_fork", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a field leather fork for domestic husbandry (@industrial_agriculture_forestry_fishing_0004_field_fork)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition AgricultureForestryAndFishingMakeFieldMildSteelSpadeForDomesticHusbandryCraftIndustrialAgricultureForestryFishing0005FieldSpade() => new()
	{
		Metadata = new("Agriculture, forestry and fishing/make field mild steel spade for domestic husbandry [craft_industrial_agriculture_forestry_fishing_0005_field_spade]", "crafts", ["industrial"], ["item:industrial_agriculture_forestry_fishing_0005_field_spade"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make field mild steel spade for domestic husbandry [craft_industrial_agriculture_forestry_fishing_0005_field_spade]",
		Category = "Agriculture, forestry and fishing",
		Blurb = "Make a field mild steel spade for domestic husbandry from prepared mild steel stock.",
		Action = "making field mild steel spade for domestic husbandry",
		ActiveCraftItemSdesc = "an in-progress field mild steel spade for domestic husbandry craft",
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
			new CatalogueCommodityRecipeInput(Grams: 2147.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2147 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrial_agriculture_forestry_fishing_0005_field_spade", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a field mild steel spade for domestic husbandry (@industrial_agriculture_forestry_fishing_0005_field_spade)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition AgricultureForestryAndFishingMakeSomeFieldOakShearsForDomesticHusbandryCraftIndustrialAgricultureForestryFishing0006FieldShears() => new()
	{
		Metadata = new("Agriculture, forestry and fishing/make some field oak shears for domestic husbandry [craft_industrial_agriculture_forestry_fishing_0006_field_shears]", "crafts", ["industrial"], ["item:industrial_agriculture_forestry_fishing_0006_field_shears"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make some field oak shears for domestic husbandry [craft_industrial_agriculture_forestry_fishing_0006_field_shears]",
		Category = "Agriculture, forestry and fishing",
		Blurb = "Make some field oak shears for domestic husbandry from prepared oak stock.",
		Action = "making some field oak shears for domestic husbandry",
		ActiveCraftItemSdesc = "an in-progress some field oak shears for domestic husbandry craft",
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
			new CatalogueCommodityRecipeInput(Grams: 2188.0, Material: "oak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2188 grams of oak", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrial_agriculture_forestry_fishing_0006_field_shears", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x some field oak shears for domestic husbandry (@industrial_agriculture_forestry_fishing_0006_field_shears)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition AgricultureForestryAndFishingMakeFieldCottonNetForDomesticHusbandryCraftIndustrialAgricultureForestryFishing0007FieldNet() => new()
	{
		Metadata = new("Agriculture, forestry and fishing/make field cotton net for domestic husbandry [craft_industrial_agriculture_forestry_fishing_0007_field_net]", "crafts", ["industrial"], ["item:industrial_agriculture_forestry_fishing_0007_field_net"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make field cotton net for domestic husbandry [craft_industrial_agriculture_forestry_fishing_0007_field_net]",
		Category = "Agriculture, forestry and fishing",
		Blurb = "Make a field cotton net for domestic husbandry from prepared cotton stock.",
		Action = "making field cotton net for domestic husbandry",
		ActiveCraftItemSdesc = "an in-progress field cotton net for domestic husbandry craft",
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
			new CatalogueCommodityRecipeInput(Grams: 2229.0, Material: "cotton", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2229 grams of cotton", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrial_agriculture_forestry_fishing_0007_field_net", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a field cotton net for domestic husbandry (@industrial_agriculture_forestry_fishing_0007_field_net)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition AgricultureForestryAndFishingMakeFieldLeatherTrapForDomesticHusbandryCraftIndustrialAgricultureForestryFishing0008FieldTrap() => new()
	{
		Metadata = new("Agriculture, forestry and fishing/make field leather trap for domestic husbandry [craft_industrial_agriculture_forestry_fishing_0008_field_trap]", "crafts", ["industrial"], ["item:industrial_agriculture_forestry_fishing_0008_field_trap"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make field leather trap for domestic husbandry [craft_industrial_agriculture_forestry_fishing_0008_field_trap]",
		Category = "Agriculture, forestry and fishing",
		Blurb = "Make a field leather trap for domestic husbandry from prepared leather stock.",
		Action = "making field leather trap for domestic husbandry",
		ActiveCraftItemSdesc = "an in-progress field leather trap for domestic husbandry craft",
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
			new CatalogueCommodityRecipeInput(Grams: 2269.0, Material: "leather", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2269 grams of leather", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrial_agriculture_forestry_fishing_0008_field_trap", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a field leather trap for domestic husbandry (@industrial_agriculture_forestry_fishing_0008_field_trap)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition AgricultureForestryAndFishingMakeFieldMildSteelBasketForDomesticHusbandryCraftIndustrialAgricultureForestryFishing0009FieldBasket() => new()
	{
		Metadata = new("Agriculture, forestry and fishing/make field mild steel basket for domestic husbandry [craft_industrial_agriculture_forestry_fishing_0009_field_basket]", "crafts", ["industrial"], ["item:industrial_agriculture_forestry_fishing_0009_field_basket"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make field mild steel basket for domestic husbandry [craft_industrial_agriculture_forestry_fishing_0009_field_basket]",
		Category = "Agriculture, forestry and fishing",
		Blurb = "Make a field mild steel basket for domestic husbandry from prepared mild steel stock.",
		Action = "making field mild steel basket for domestic husbandry",
		ActiveCraftItemSdesc = "an in-progress field mild steel basket for domestic husbandry craft",
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
			new CatalogueCommodityRecipeInput(Grams: 2310.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2310 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrial_agriculture_forestry_fishing_0009_field_basket", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a field mild steel basket for domestic husbandry (@industrial_agriculture_forestry_fishing_0009_field_basket)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition LogisticsRetailAndHospitalityMakePlainCottonWasteForDomesticGoodsHandlingCraftIndustrialisedLogisticsRetailHospitality0001PlainWaste() => new()
	{
		Metadata = new("Logistics, retail and hospitality/make plain cotton waste for domestic goods handling [craft_industrialised_logistics_retail_hospitality_0001_plain_waste]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_logistics_retail_hospitality_0001_plain_waste"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make plain cotton waste for domestic goods handling [craft_industrialised_logistics_retail_hospitality_0001_plain_waste]",
		Category = "Logistics, retail and hospitality",
		Blurb = "Make a plain cotton waste for domestic goods handling from prepared cotton stock.",
		Action = "making plain cotton waste for domestic goods handling",
		ActiveCraftItemSdesc = "an in-progress plain cotton waste for domestic goods handling craft",
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
			new CatalogueCommodityRecipeInput(Grams: 2096.0, Material: "cotton", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2096 grams of cotton", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_logistics_retail_hospitality_0001_plain_waste", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a plain cotton waste for domestic goods handling (@industrialised_logistics_retail_hospitality_0001_plain_waste)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition LogisticsRetailAndHospitalityMakePlainOakCrateForDomesticGoodsHandlingCraftIndustrialisedLogisticsRetailHospitality0002PlainCrate() => new()
	{
		Metadata = new("Logistics, retail and hospitality/make plain oak crate for domestic goods handling [craft_industrialised_logistics_retail_hospitality_0002_plain_crate]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_logistics_retail_hospitality_0002_plain_crate"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make plain oak crate for domestic goods handling [craft_industrialised_logistics_retail_hospitality_0002_plain_crate]",
		Category = "Logistics, retail and hospitality",
		Blurb = "Make a plain oak crate for domestic goods handling from prepared oak stock.",
		Action = "making plain oak crate for domestic goods handling",
		ActiveCraftItemSdesc = "an in-progress plain oak crate for domestic goods handling craft",
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
			new CatalogueCommodityRecipeInput(Grams: 2136.0, Material: "oak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2136 grams of oak", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_logistics_retail_hospitality_0002_plain_crate", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a plain oak crate for domestic goods handling (@industrialised_logistics_retail_hospitality_0002_plain_crate)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition LogisticsRetailAndHospitalityMakePlainMildSteelTrolleyForDomesticGoodsHandlingCraftIndustrialisedLogisticsRetailHospitality0003PlainTrolley() => new()
	{
		Metadata = new("Logistics, retail and hospitality/make plain mild steel trolley for domestic goods handling [craft_industrialised_logistics_retail_hospitality_0003_plain_trolley]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_logistics_retail_hospitality_0003_plain_trolley"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make plain mild steel trolley for domestic goods handling [craft_industrialised_logistics_retail_hospitality_0003_plain_trolley]",
		Category = "Logistics, retail and hospitality",
		Blurb = "Make a plain mild steel trolley for domestic goods handling from prepared mild steel stock.",
		Action = "making plain mild steel trolley for domestic goods handling",
		ActiveCraftItemSdesc = "an in-progress plain mild steel trolley for domestic goods handling craft",
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
			new CatalogueCommodityRecipeInput(Grams: 2177.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2177 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_logistics_retail_hospitality_0003_plain_trolley", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a plain mild steel trolley for domestic goods handling (@industrialised_logistics_retail_hospitality_0003_plain_trolley)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition LogisticsRetailAndHospitalityMakePlainPaperTrayForDomesticGoodsHandlingCraftIndustrialisedLogisticsRetailHospitality0004PlainTray() => new()
	{
		Metadata = new("Logistics, retail and hospitality/make plain paper tray for domestic goods handling [craft_industrialised_logistics_retail_hospitality_0004_plain_tray]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_logistics_retail_hospitality_0004_plain_tray"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make plain paper tray for domestic goods handling [craft_industrialised_logistics_retail_hospitality_0004_plain_tray]",
		Category = "Logistics, retail and hospitality",
		Blurb = "Make a plain paper tray for domestic goods handling from prepared paper stock.",
		Action = "making plain paper tray for domestic goods handling",
		ActiveCraftItemSdesc = "an in-progress plain paper tray for domestic goods handling craft",
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
			new CatalogueCommodityRecipeInput(Grams: 2218.0, Material: "paper", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2218 grams of paper", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_logistics_retail_hospitality_0004_plain_tray", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a plain paper tray for domestic goods handling (@industrialised_logistics_retail_hospitality_0004_plain_tray)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition LogisticsRetailAndHospitalityMakePlainCottonCounterForDomesticGoodsHandlingCraftIndustrialisedLogisticsRetailHospitality0005PlainCounter() => new()
	{
		Metadata = new("Logistics, retail and hospitality/make plain cotton counter for domestic goods handling [craft_industrialised_logistics_retail_hospitality_0005_plain_counter]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_logistics_retail_hospitality_0005_plain_counter"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make plain cotton counter for domestic goods handling [craft_industrialised_logistics_retail_hospitality_0005_plain_counter]",
		Category = "Logistics, retail and hospitality",
		Blurb = "Make a plain cotton counter for domestic goods handling from prepared cotton stock.",
		Action = "making plain cotton counter for domestic goods handling",
		ActiveCraftItemSdesc = "an in-progress plain cotton counter for domestic goods handling craft",
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
			new CatalogueCommodityRecipeInput(Grams: 2258.0, Material: "cotton", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2258 grams of cotton", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_logistics_retail_hospitality_0005_plain_counter", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a plain cotton counter for domestic goods handling (@industrialised_logistics_retail_hospitality_0005_plain_counter)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition LogisticsRetailAndHospitalityMakePlainOakScaleForDomesticGoodsHandlingCraftIndustrialisedLogisticsRetailHospitality0006PlainScale() => new()
	{
		Metadata = new("Logistics, retail and hospitality/make plain oak scale for domestic goods handling [craft_industrialised_logistics_retail_hospitality_0006_plain_scale]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_logistics_retail_hospitality_0006_plain_scale"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make plain oak scale for domestic goods handling [craft_industrialised_logistics_retail_hospitality_0006_plain_scale]",
		Category = "Logistics, retail and hospitality",
		Blurb = "Make a plain oak scale for domestic goods handling from prepared oak stock.",
		Action = "making plain oak scale for domestic goods handling",
		ActiveCraftItemSdesc = "an in-progress plain oak scale for domestic goods handling craft",
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
			new CatalogueCommodityRecipeInput(Grams: 2299.0, Material: "oak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2299 grams of oak", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_logistics_retail_hospitality_0006_plain_scale", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a plain oak scale for domestic goods handling (@industrialised_logistics_retail_hospitality_0006_plain_scale)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition LogisticsRetailAndHospitalityMakePlainMildSteelTicketForDomesticGoodsHandlingCraftIndustrialisedLogisticsRetailHospitality0007PlainTicket() => new()
	{
		Metadata = new("Logistics, retail and hospitality/make plain mild steel ticket for domestic goods handling [craft_industrialised_logistics_retail_hospitality_0007_plain_ticket]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_logistics_retail_hospitality_0007_plain_ticket"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make plain mild steel ticket for domestic goods handling [craft_industrialised_logistics_retail_hospitality_0007_plain_ticket]",
		Category = "Logistics, retail and hospitality",
		Blurb = "Make a plain mild steel ticket for domestic goods handling from prepared mild steel stock.",
		Action = "making plain mild steel ticket for domestic goods handling",
		ActiveCraftItemSdesc = "an in-progress plain mild steel ticket for domestic goods handling craft",
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
			new CatalogueCommodityRecipeInput(Grams: 2340.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2340 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_logistics_retail_hospitality_0007_plain_ticket", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a plain mild steel ticket for domestic goods handling (@industrialised_logistics_retail_hospitality_0007_plain_ticket)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition LogisticsRetailAndHospitalityMakePlainPaperBasketForDomesticGoodsHandlingCraftIndustrialisedLogisticsRetailHospitality0008PlainBasket() => new()
	{
		Metadata = new("Logistics, retail and hospitality/make plain paper basket for domestic goods handling [craft_industrialised_logistics_retail_hospitality_0008_plain_basket]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_logistics_retail_hospitality_0008_plain_basket"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make plain paper basket for domestic goods handling [craft_industrialised_logistics_retail_hospitality_0008_plain_basket]",
		Category = "Logistics, retail and hospitality",
		Blurb = "Make a plain paper basket for domestic goods handling from prepared paper stock.",
		Action = "making plain paper basket for domestic goods handling",
		ActiveCraftItemSdesc = "an in-progress plain paper basket for domestic goods handling craft",
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
			new CatalogueCommodityRecipeInput(Grams: 2380.0, Material: "paper", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2380 grams of paper", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_logistics_retail_hospitality_0008_plain_basket", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a plain paper basket for domestic goods handling (@industrialised_logistics_retail_hospitality_0008_plain_basket)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition LogisticsRetailAndHospitalityMakePlainCottonCaseForDomesticGoodsHandlingCraftIndustrialisedLogisticsRetailHospitality0009PlainCase() => new()
	{
		Metadata = new("Logistics, retail and hospitality/make plain cotton case for domestic goods handling [craft_industrialised_logistics_retail_hospitality_0009_plain_case]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_logistics_retail_hospitality_0009_plain_case"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make plain cotton case for domestic goods handling [craft_industrialised_logistics_retail_hospitality_0009_plain_case]",
		Category = "Logistics, retail and hospitality",
		Blurb = "Make a plain cotton case for domestic goods handling from prepared cotton stock.",
		Action = "making plain cotton case for domestic goods handling",
		ActiveCraftItemSdesc = "an in-progress plain cotton case for domestic goods handling craft",
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
			new CatalogueCommodityRecipeInput(Grams: 2421.0, Material: "cotton", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2421 grams of cotton", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_logistics_retail_hospitality_0009_plain_case", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a plain cotton case for domestic goods handling (@industrialised_logistics_retail_hospitality_0009_plain_case)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition LogisticsRetailAndHospitalityMakePlainOakSignForDomesticGoodsHandlingCraftIndustrialisedLogisticsRetailHospitality0010PlainSign() => new()
	{
		Metadata = new("Logistics, retail and hospitality/make plain oak sign for domestic goods handling [craft_industrialised_logistics_retail_hospitality_0010_plain_sign]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_logistics_retail_hospitality_0010_plain_sign"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make plain oak sign for domestic goods handling [craft_industrialised_logistics_retail_hospitality_0010_plain_sign]",
		Category = "Logistics, retail and hospitality",
		Blurb = "Make a plain oak sign for domestic goods handling from prepared oak stock.",
		Action = "making plain oak sign for domestic goods handling",
		ActiveCraftItemSdesc = "an in-progress plain oak sign for domestic goods handling craft",
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
			new CatalogueCommodityRecipeInput(Grams: 2462.0, Material: "oak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2462 grams of oak", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_logistics_retail_hospitality_0010_plain_sign", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a plain oak sign for domestic goods handling (@industrialised_logistics_retail_hospitality_0010_plain_sign)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition LogisticsRetailAndHospitalityMakeReinforcedMildSteelWasteForDomesticGoodsHandlingCraftIndustrialisedLogisticsRetailHospitality0011ReinforcedWaste() => new()
	{
		Metadata = new("Logistics, retail and hospitality/make reinforced mild steel waste for domestic goods handling [craft_industrialised_logistics_retail_hospitality_0011_reinforced_waste]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_logistics_retail_hospitality_0011_reinforced_waste"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make reinforced mild steel waste for domestic goods handling [craft_industrialised_logistics_retail_hospitality_0011_reinforced_waste]",
		Category = "Logistics, retail and hospitality",
		Blurb = "Make a reinforced mild steel waste for domestic goods handling from prepared mild steel stock.",
		Action = "making reinforced mild steel waste for domestic goods handling",
		ActiveCraftItemSdesc = "an in-progress reinforced mild steel waste for domestic goods handling craft",
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
			new CatalogueCommodityRecipeInput(Grams: 2502.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2502 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_logistics_retail_hospitality_0011_reinforced_waste", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a reinforced mild steel waste for domestic goods handling (@industrialised_logistics_retail_hospitality_0011_reinforced_waste)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition LogisticsRetailAndHospitalityMakeReinforcedPaperCrateForDomesticGoodsHandlingCraftIndustrialisedLogisticsRetailHospitality0012ReinforcedCrate() => new()
	{
		Metadata = new("Logistics, retail and hospitality/make reinforced paper crate for domestic goods handling [craft_industrialised_logistics_retail_hospitality_0012_reinforced_crate]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_logistics_retail_hospitality_0012_reinforced_crate"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make reinforced paper crate for domestic goods handling [craft_industrialised_logistics_retail_hospitality_0012_reinforced_crate]",
		Category = "Logistics, retail and hospitality",
		Blurb = "Make a reinforced paper crate for domestic goods handling from prepared paper stock.",
		Action = "making reinforced paper crate for domestic goods handling",
		ActiveCraftItemSdesc = "an in-progress reinforced paper crate for domestic goods handling craft",
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
			new CatalogueCommodityRecipeInput(Grams: 2543.0, Material: "paper", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2543 grams of paper", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_logistics_retail_hospitality_0012_reinforced_crate", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a reinforced paper crate for domestic goods handling (@industrialised_logistics_retail_hospitality_0012_reinforced_crate)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition LogisticsRetailAndHospitalityMakeReinforcedCottonTrolleyForDomesticGoodsHandlingCraftIndustrialisedLogisticsRetailHospitality0013ReinforcedTrolley() => new()
	{
		Metadata = new("Logistics, retail and hospitality/make reinforced cotton trolley for domestic goods handling [craft_industrialised_logistics_retail_hospitality_0013_reinforced_trolley]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_logistics_retail_hospitality_0013_reinforced_trolley"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make reinforced cotton trolley for domestic goods handling [craft_industrialised_logistics_retail_hospitality_0013_reinforced_trolley]",
		Category = "Logistics, retail and hospitality",
		Blurb = "Make a reinforced cotton trolley for domestic goods handling from prepared cotton stock.",
		Action = "making reinforced cotton trolley for domestic goods handling",
		ActiveCraftItemSdesc = "an in-progress reinforced cotton trolley for domestic goods handling craft",
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
			new CatalogueCommodityRecipeInput(Grams: 2584.0, Material: "cotton", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2584 grams of cotton", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_logistics_retail_hospitality_0013_reinforced_trolley", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a reinforced cotton trolley for domestic goods handling (@industrialised_logistics_retail_hospitality_0013_reinforced_trolley)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition LogisticsRetailAndHospitalityMakeReinforcedOakTrayForDomesticGoodsHandlingCraftIndustrialisedLogisticsRetailHospitality0014ReinforcedTray() => new()
	{
		Metadata = new("Logistics, retail and hospitality/make reinforced oak tray for domestic goods handling [craft_industrialised_logistics_retail_hospitality_0014_reinforced_tray]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_logistics_retail_hospitality_0014_reinforced_tray"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make reinforced oak tray for domestic goods handling [craft_industrialised_logistics_retail_hospitality_0014_reinforced_tray]",
		Category = "Logistics, retail and hospitality",
		Blurb = "Make a reinforced oak tray for domestic goods handling from prepared oak stock.",
		Action = "making reinforced oak tray for domestic goods handling",
		ActiveCraftItemSdesc = "an in-progress reinforced oak tray for domestic goods handling craft",
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
			new CatalogueCommodityRecipeInput(Grams: 2625.0, Material: "oak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2625 grams of oak", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_logistics_retail_hospitality_0014_reinforced_tray", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a reinforced oak tray for domestic goods handling (@industrialised_logistics_retail_hospitality_0014_reinforced_tray)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition LogisticsRetailAndHospitalityMakeReinforcedMildSteelCounterForDomesticGoodsHandlingCraftIndustrialisedLogisticsRetailHospitality0015ReinforcedCounter() => new()
	{
		Metadata = new("Logistics, retail and hospitality/make reinforced mild steel counter for domestic goods handling [craft_industrialised_logistics_retail_hospitality_0015_reinforced_counter]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_logistics_retail_hospitality_0015_reinforced_counter"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make reinforced mild steel counter for domestic goods handling [craft_industrialised_logistics_retail_hospitality_0015_reinforced_counter]",
		Category = "Logistics, retail and hospitality",
		Blurb = "Make a reinforced mild steel counter for domestic goods handling from prepared mild steel stock.",
		Action = "making reinforced mild steel counter for domestic goods handling",
		ActiveCraftItemSdesc = "an in-progress reinforced mild steel counter for domestic goods handling craft",
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
			new CatalogueCommodityRecipeInput(Grams: 2665.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2665 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_logistics_retail_hospitality_0015_reinforced_counter", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a reinforced mild steel counter for domestic goods handling (@industrialised_logistics_retail_hospitality_0015_reinforced_counter)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition LogisticsRetailAndHospitalityMakeReinforcedPaperScaleForDomesticGoodsHandlingCraftIndustrialisedLogisticsRetailHospitality0016ReinforcedScale() => new()
	{
		Metadata = new("Logistics, retail and hospitality/make reinforced paper scale for domestic goods handling [craft_industrialised_logistics_retail_hospitality_0016_reinforced_scale]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_logistics_retail_hospitality_0016_reinforced_scale"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make reinforced paper scale for domestic goods handling [craft_industrialised_logistics_retail_hospitality_0016_reinforced_scale]",
		Category = "Logistics, retail and hospitality",
		Blurb = "Make a reinforced paper scale for domestic goods handling from prepared paper stock.",
		Action = "making reinforced paper scale for domestic goods handling",
		ActiveCraftItemSdesc = "an in-progress reinforced paper scale for domestic goods handling craft",
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
			new CatalogueCommodityRecipeInput(Grams: 2706.0, Material: "paper", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2706 grams of paper", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_logistics_retail_hospitality_0016_reinforced_scale", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a reinforced paper scale for domestic goods handling (@industrialised_logistics_retail_hospitality_0016_reinforced_scale)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition LogisticsRetailAndHospitalityMakeReinforcedCottonTicketForDomesticGoodsHandlingCraftIndustrialisedLogisticsRetailHospitality0017ReinforcedTicket() => new()
	{
		Metadata = new("Logistics, retail and hospitality/make reinforced cotton ticket for domestic goods handling [craft_industrialised_logistics_retail_hospitality_0017_reinforced_ticket]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_logistics_retail_hospitality_0017_reinforced_ticket"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make reinforced cotton ticket for domestic goods handling [craft_industrialised_logistics_retail_hospitality_0017_reinforced_ticket]",
		Category = "Logistics, retail and hospitality",
		Blurb = "Make a reinforced cotton ticket for domestic goods handling from prepared cotton stock.",
		Action = "making reinforced cotton ticket for domestic goods handling",
		ActiveCraftItemSdesc = "an in-progress reinforced cotton ticket for domestic goods handling craft",
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
			new CatalogueCommodityRecipeInput(Grams: 2747.0, Material: "cotton", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2747 grams of cotton", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_logistics_retail_hospitality_0017_reinforced_ticket", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a reinforced cotton ticket for domestic goods handling (@industrialised_logistics_retail_hospitality_0017_reinforced_ticket)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition LogisticsRetailAndHospitalityMakeReinforcedOakBasketForDomesticGoodsHandlingCraftIndustrialisedLogisticsRetailHospitality0018ReinforcedBasket() => new()
	{
		Metadata = new("Logistics, retail and hospitality/make reinforced oak basket for domestic goods handling [craft_industrialised_logistics_retail_hospitality_0018_reinforced_basket]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_logistics_retail_hospitality_0018_reinforced_basket"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make reinforced oak basket for domestic goods handling [craft_industrialised_logistics_retail_hospitality_0018_reinforced_basket]",
		Category = "Logistics, retail and hospitality",
		Blurb = "Make a reinforced oak basket for domestic goods handling from prepared oak stock.",
		Action = "making reinforced oak basket for domestic goods handling",
		ActiveCraftItemSdesc = "an in-progress reinforced oak basket for domestic goods handling craft",
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
			new CatalogueCommodityRecipeInput(Grams: 2787.0, Material: "oak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2787 grams of oak", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_logistics_retail_hospitality_0018_reinforced_basket", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a reinforced oak basket for domestic goods handling (@industrialised_logistics_retail_hospitality_0018_reinforced_basket)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition LogisticsRetailAndHospitalityMakeReinforcedMildSteelCaseForDomesticGoodsHandlingCraftIndustrialisedLogisticsRetailHospitality0019ReinforcedCase() => new()
	{
		Metadata = new("Logistics, retail and hospitality/make reinforced mild steel case for domestic goods handling [craft_industrialised_logistics_retail_hospitality_0019_reinforced_case]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_logistics_retail_hospitality_0019_reinforced_case"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make reinforced mild steel case for domestic goods handling [craft_industrialised_logistics_retail_hospitality_0019_reinforced_case]",
		Category = "Logistics, retail and hospitality",
		Blurb = "Make a reinforced mild steel case for domestic goods handling from prepared mild steel stock.",
		Action = "making reinforced mild steel case for domestic goods handling",
		ActiveCraftItemSdesc = "an in-progress reinforced mild steel case for domestic goods handling craft",
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
			new CatalogueCommodityRecipeInput(Grams: 2828.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2828 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_logistics_retail_hospitality_0019_reinforced_case", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a reinforced mild steel case for domestic goods handling (@industrialised_logistics_retail_hospitality_0019_reinforced_case)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition LogisticsRetailAndHospitalityMakeReinforcedPaperSignForDomesticGoodsHandlingCraftIndustrialisedLogisticsRetailHospitality0020ReinforcedSign() => new()
	{
		Metadata = new("Logistics, retail and hospitality/make reinforced paper sign for domestic goods handling [craft_industrialised_logistics_retail_hospitality_0020_reinforced_sign]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_logistics_retail_hospitality_0020_reinforced_sign"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make reinforced paper sign for domestic goods handling [craft_industrialised_logistics_retail_hospitality_0020_reinforced_sign]",
		Category = "Logistics, retail and hospitality",
		Blurb = "Make a reinforced paper sign for domestic goods handling from prepared paper stock.",
		Action = "making reinforced paper sign for domestic goods handling",
		ActiveCraftItemSdesc = "an in-progress reinforced paper sign for domestic goods handling craft",
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
			new CatalogueCommodityRecipeInput(Grams: 2869.0, Material: "paper", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2869 grams of paper", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_logistics_retail_hospitality_0020_reinforced_sign", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a reinforced paper sign for domestic goods handling (@industrialised_logistics_retail_hospitality_0020_reinforced_sign)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition LogisticsRetailAndHospitalityMakeCompactCottonWasteForDomesticGoodsHandlingCraftIndustrialisedLogisticsRetailHospitality0021CompactWaste() => new()
	{
		Metadata = new("Logistics, retail and hospitality/make compact cotton waste for domestic goods handling [craft_industrialised_logistics_retail_hospitality_0021_compact_waste]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_logistics_retail_hospitality_0021_compact_waste"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make compact cotton waste for domestic goods handling [craft_industrialised_logistics_retail_hospitality_0021_compact_waste]",
		Category = "Logistics, retail and hospitality",
		Blurb = "Make a compact cotton waste for domestic goods handling from prepared cotton stock.",
		Action = "making compact cotton waste for domestic goods handling",
		ActiveCraftItemSdesc = "an in-progress compact cotton waste for domestic goods handling craft",
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
			new CatalogueCommodityRecipeInput(Grams: 2910.0, Material: "cotton", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2910 grams of cotton", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_logistics_retail_hospitality_0021_compact_waste", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a compact cotton waste for domestic goods handling (@industrialised_logistics_retail_hospitality_0021_compact_waste)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition LogisticsRetailAndHospitalityMakeCompactOakCrateForDomesticGoodsHandlingCraftIndustrialisedLogisticsRetailHospitality0022CompactCrate() => new()
	{
		Metadata = new("Logistics, retail and hospitality/make compact oak crate for domestic goods handling [craft_industrialised_logistics_retail_hospitality_0022_compact_crate]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_logistics_retail_hospitality_0022_compact_crate"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make compact oak crate for domestic goods handling [craft_industrialised_logistics_retail_hospitality_0022_compact_crate]",
		Category = "Logistics, retail and hospitality",
		Blurb = "Make a compact oak crate for domestic goods handling from prepared oak stock.",
		Action = "making compact oak crate for domestic goods handling",
		ActiveCraftItemSdesc = "an in-progress compact oak crate for domestic goods handling craft",
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
			new CatalogueCommodityRecipeInput(Grams: 2950.0, Material: "oak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2950 grams of oak", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_logistics_retail_hospitality_0022_compact_crate", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a compact oak crate for domestic goods handling (@industrialised_logistics_retail_hospitality_0022_compact_crate)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition LogisticsRetailAndHospitalityMakeCompactMildSteelTrolleyForDomesticGoodsHandlingCraftIndustrialisedLogisticsRetailHospitality0023CompactTrolley() => new()
	{
		Metadata = new("Logistics, retail and hospitality/make compact mild steel trolley for domestic goods handling [craft_industrialised_logistics_retail_hospitality_0023_compact_trolley]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_logistics_retail_hospitality_0023_compact_trolley"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make compact mild steel trolley for domestic goods handling [craft_industrialised_logistics_retail_hospitality_0023_compact_trolley]",
		Category = "Logistics, retail and hospitality",
		Blurb = "Make a compact mild steel trolley for domestic goods handling from prepared mild steel stock.",
		Action = "making compact mild steel trolley for domestic goods handling",
		ActiveCraftItemSdesc = "an in-progress compact mild steel trolley for domestic goods handling craft",
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
			new CatalogueCommodityRecipeInput(Grams: 2991.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2991 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_logistics_retail_hospitality_0023_compact_trolley", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a compact mild steel trolley for domestic goods handling (@industrialised_logistics_retail_hospitality_0023_compact_trolley)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition LogisticsRetailAndHospitalityMakeCompactPaperTrayForDomesticGoodsHandlingCraftIndustrialisedLogisticsRetailHospitality0024CompactTray() => new()
	{
		Metadata = new("Logistics, retail and hospitality/make compact paper tray for domestic goods handling [craft_industrialised_logistics_retail_hospitality_0024_compact_tray]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_logistics_retail_hospitality_0024_compact_tray"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make compact paper tray for domestic goods handling [craft_industrialised_logistics_retail_hospitality_0024_compact_tray]",
		Category = "Logistics, retail and hospitality",
		Blurb = "Make a compact paper tray for domestic goods handling from prepared paper stock.",
		Action = "making compact paper tray for domestic goods handling",
		ActiveCraftItemSdesc = "an in-progress compact paper tray for domestic goods handling craft",
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
			new CatalogueCommodityRecipeInput(Grams: 3032.0, Material: "paper", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3032 grams of paper", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_logistics_retail_hospitality_0024_compact_tray", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a compact paper tray for domestic goods handling (@industrialised_logistics_retail_hospitality_0024_compact_tray)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition LogisticsRetailAndHospitalityMakeCompactCottonCounterForDomesticGoodsHandlingCraftIndustrialisedLogisticsRetailHospitality0025CompactCounter() => new()
	{
		Metadata = new("Logistics, retail and hospitality/make compact cotton counter for domestic goods handling [craft_industrialised_logistics_retail_hospitality_0025_compact_counter]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_logistics_retail_hospitality_0025_compact_counter"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make compact cotton counter for domestic goods handling [craft_industrialised_logistics_retail_hospitality_0025_compact_counter]",
		Category = "Logistics, retail and hospitality",
		Blurb = "Make a compact cotton counter for domestic goods handling from prepared cotton stock.",
		Action = "making compact cotton counter for domestic goods handling",
		ActiveCraftItemSdesc = "an in-progress compact cotton counter for domestic goods handling craft",
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
			new CatalogueCommodityRecipeInput(Grams: 3072.0, Material: "cotton", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3072 grams of cotton", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_logistics_retail_hospitality_0025_compact_counter", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a compact cotton counter for domestic goods handling (@industrialised_logistics_retail_hospitality_0025_compact_counter)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition LogisticsRetailAndHospitalityMakeCompactOakScaleForDomesticGoodsHandlingCraftIndustrialisedLogisticsRetailHospitality0026CompactScale() => new()
	{
		Metadata = new("Logistics, retail and hospitality/make compact oak scale for domestic goods handling [craft_industrialised_logistics_retail_hospitality_0026_compact_scale]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_logistics_retail_hospitality_0026_compact_scale"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make compact oak scale for domestic goods handling [craft_industrialised_logistics_retail_hospitality_0026_compact_scale]",
		Category = "Logistics, retail and hospitality",
		Blurb = "Make a compact oak scale for domestic goods handling from prepared oak stock.",
		Action = "making compact oak scale for domestic goods handling",
		ActiveCraftItemSdesc = "an in-progress compact oak scale for domestic goods handling craft",
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
			new CatalogueCommodityRecipeInput(Grams: 3113.0, Material: "oak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3113 grams of oak", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_logistics_retail_hospitality_0026_compact_scale", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a compact oak scale for domestic goods handling (@industrialised_logistics_retail_hospitality_0026_compact_scale)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition LogisticsRetailAndHospitalityMakeCompactMildSteelTicketForDomesticGoodsHandlingCraftIndustrialisedLogisticsRetailHospitality0027CompactTicket() => new()
	{
		Metadata = new("Logistics, retail and hospitality/make compact mild steel ticket for domestic goods handling [craft_industrialised_logistics_retail_hospitality_0027_compact_ticket]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_logistics_retail_hospitality_0027_compact_ticket"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make compact mild steel ticket for domestic goods handling [craft_industrialised_logistics_retail_hospitality_0027_compact_ticket]",
		Category = "Logistics, retail and hospitality",
		Blurb = "Make a compact mild steel ticket for domestic goods handling from prepared mild steel stock.",
		Action = "making compact mild steel ticket for domestic goods handling",
		ActiveCraftItemSdesc = "an in-progress compact mild steel ticket for domestic goods handling craft",
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
			new CatalogueCommodityRecipeInput(Grams: 3154.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3154 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_logistics_retail_hospitality_0027_compact_ticket", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a compact mild steel ticket for domestic goods handling (@industrialised_logistics_retail_hospitality_0027_compact_ticket)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition LogisticsRetailAndHospitalityMakeCompactPaperBasketForDomesticGoodsHandlingCraftIndustrialisedLogisticsRetailHospitality0028CompactBasket() => new()
	{
		Metadata = new("Logistics, retail and hospitality/make compact paper basket for domestic goods handling [craft_industrialised_logistics_retail_hospitality_0028_compact_basket]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_logistics_retail_hospitality_0028_compact_basket"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make compact paper basket for domestic goods handling [craft_industrialised_logistics_retail_hospitality_0028_compact_basket]",
		Category = "Logistics, retail and hospitality",
		Blurb = "Make a compact paper basket for domestic goods handling from prepared paper stock.",
		Action = "making compact paper basket for domestic goods handling",
		ActiveCraftItemSdesc = "an in-progress compact paper basket for domestic goods handling craft",
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
			new CatalogueCommodityRecipeInput(Grams: 3194.0, Material: "paper", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3194 grams of paper", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_logistics_retail_hospitality_0028_compact_basket", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a compact paper basket for domestic goods handling (@industrialised_logistics_retail_hospitality_0028_compact_basket)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition LogisticsRetailAndHospitalityMakeCompactCottonCaseForDomesticGoodsHandlingCraftIndustrialisedLogisticsRetailHospitality0029CompactCase() => new()
	{
		Metadata = new("Logistics, retail and hospitality/make compact cotton case for domestic goods handling [craft_industrialised_logistics_retail_hospitality_0029_compact_case]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_logistics_retail_hospitality_0029_compact_case"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make compact cotton case for domestic goods handling [craft_industrialised_logistics_retail_hospitality_0029_compact_case]",
		Category = "Logistics, retail and hospitality",
		Blurb = "Make a compact cotton case for domestic goods handling from prepared cotton stock.",
		Action = "making compact cotton case for domestic goods handling",
		ActiveCraftItemSdesc = "an in-progress compact cotton case for domestic goods handling craft",
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
			new CatalogueCommodityRecipeInput(Grams: 3235.0, Material: "cotton", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3235 grams of cotton", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_logistics_retail_hospitality_0029_compact_case", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a compact cotton case for domestic goods handling (@industrialised_logistics_retail_hospitality_0029_compact_case)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition LogisticsRetailAndHospitalityMakeCompactOakSignForDomesticGoodsHandlingCraftIndustrialisedLogisticsRetailHospitality0030CompactSign() => new()
	{
		Metadata = new("Logistics, retail and hospitality/make compact oak sign for domestic goods handling [craft_industrialised_logistics_retail_hospitality_0030_compact_sign]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_logistics_retail_hospitality_0030_compact_sign"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make compact oak sign for domestic goods handling [craft_industrialised_logistics_retail_hospitality_0030_compact_sign]",
		Category = "Logistics, retail and hospitality",
		Blurb = "Make a compact oak sign for domestic goods handling from prepared oak stock.",
		Action = "making compact oak sign for domestic goods handling",
		ActiveCraftItemSdesc = "an in-progress compact oak sign for domestic goods handling craft",
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
			new CatalogueCommodityRecipeInput(Grams: 3276.0, Material: "oak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3276 grams of oak", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_logistics_retail_hospitality_0030_compact_sign", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a compact oak sign for domestic goods handling (@industrialised_logistics_retail_hospitality_0030_compact_sign)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition LogisticsRetailAndHospitalityMakeHeavyDutyMildSteelWasteForDomesticGoodsHandlingCraftIndustrialisedLogisticsRetailHospitality0031HeavyDutyWaste() => new()
	{
		Metadata = new("Logistics, retail and hospitality/make heavy-duty mild steel waste for domestic goods handling [craft_industrialised_logistics_retail_hospitality_0031_heavy_duty_waste]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_logistics_retail_hospitality_0031_heavy_duty_waste"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make heavy-duty mild steel waste for domestic goods handling [craft_industrialised_logistics_retail_hospitality_0031_heavy_duty_waste]",
		Category = "Logistics, retail and hospitality",
		Blurb = "Make a heavy-duty mild steel waste for domestic goods handling from prepared mild steel stock.",
		Action = "making heavy-duty mild steel waste for domestic goods handling",
		ActiveCraftItemSdesc = "an in-progress heavy-duty mild steel waste for domestic goods handling craft",
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
			new CatalogueCommodityRecipeInput(Grams: 3317.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3317 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_logistics_retail_hospitality_0031_heavy_duty_waste", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a heavy-duty mild steel waste for domestic goods handling (@industrialised_logistics_retail_hospitality_0031_heavy_duty_waste)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition LogisticsRetailAndHospitalityMakeHeavyDutyPaperCrateForDomesticGoodsHandlingCraftIndustrialisedLogisticsRetailHospitality0032HeavyDutyCrate() => new()
	{
		Metadata = new("Logistics, retail and hospitality/make heavy-duty paper crate for domestic goods handling [craft_industrialised_logistics_retail_hospitality_0032_heavy_duty_crate]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_logistics_retail_hospitality_0032_heavy_duty_crate"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make heavy-duty paper crate for domestic goods handling [craft_industrialised_logistics_retail_hospitality_0032_heavy_duty_crate]",
		Category = "Logistics, retail and hospitality",
		Blurb = "Make a heavy-duty paper crate for domestic goods handling from prepared paper stock.",
		Action = "making heavy-duty paper crate for domestic goods handling",
		ActiveCraftItemSdesc = "an in-progress heavy-duty paper crate for domestic goods handling craft",
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
			new CatalogueCommodityRecipeInput(Grams: 3357.0, Material: "paper", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3357 grams of paper", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_logistics_retail_hospitality_0032_heavy_duty_crate", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a heavy-duty paper crate for domestic goods handling (@industrialised_logistics_retail_hospitality_0032_heavy_duty_crate)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition LogisticsRetailAndHospitalityMakeHeavyDutyCottonTrolleyForDomesticGoodsHandlingCraftIndustrialisedLogisticsRetailHospitality0033HeavyDutyTrolley() => new()
	{
		Metadata = new("Logistics, retail and hospitality/make heavy-duty cotton trolley for domestic goods handling [craft_industrialised_logistics_retail_hospitality_0033_heavy_duty_trolley]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_logistics_retail_hospitality_0033_heavy_duty_trolley"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make heavy-duty cotton trolley for domestic goods handling [craft_industrialised_logistics_retail_hospitality_0033_heavy_duty_trolley]",
		Category = "Logistics, retail and hospitality",
		Blurb = "Make a heavy-duty cotton trolley for domestic goods handling from prepared cotton stock.",
		Action = "making heavy-duty cotton trolley for domestic goods handling",
		ActiveCraftItemSdesc = "an in-progress heavy-duty cotton trolley for domestic goods handling craft",
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
			new CatalogueCommodityRecipeInput(Grams: 3398.0, Material: "cotton", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3398 grams of cotton", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_logistics_retail_hospitality_0033_heavy_duty_trolley", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a heavy-duty cotton trolley for domestic goods handling (@industrialised_logistics_retail_hospitality_0033_heavy_duty_trolley)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition LogisticsRetailAndHospitalityMakeHeavyDutyOakTrayForDomesticGoodsHandlingCraftIndustrialisedLogisticsRetailHospitality0034HeavyDutyTray() => new()
	{
		Metadata = new("Logistics, retail and hospitality/make heavy-duty oak tray for domestic goods handling [craft_industrialised_logistics_retail_hospitality_0034_heavy_duty_tray]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_logistics_retail_hospitality_0034_heavy_duty_tray"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make heavy-duty oak tray for domestic goods handling [craft_industrialised_logistics_retail_hospitality_0034_heavy_duty_tray]",
		Category = "Logistics, retail and hospitality",
		Blurb = "Make a heavy-duty oak tray for domestic goods handling from prepared oak stock.",
		Action = "making heavy-duty oak tray for domestic goods handling",
		ActiveCraftItemSdesc = "an in-progress heavy-duty oak tray for domestic goods handling craft",
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
			new CatalogueCommodityRecipeInput(Grams: 3439.0, Material: "oak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3439 grams of oak", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_logistics_retail_hospitality_0034_heavy_duty_tray", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a heavy-duty oak tray for domestic goods handling (@industrialised_logistics_retail_hospitality_0034_heavy_duty_tray)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition LogisticsRetailAndHospitalityMakeHeavyDutyMildSteelCounterForDomesticGoodsHandlingCraftIndustrialisedLogisticsRetailHospitality0035HeavyDutyCounter() => new()
	{
		Metadata = new("Logistics, retail and hospitality/make heavy-duty mild steel counter for domestic goods handling [craft_industrialised_logistics_retail_hospitality_0035_heavy_duty_counter]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_logistics_retail_hospitality_0035_heavy_duty_counter"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make heavy-duty mild steel counter for domestic goods handling [craft_industrialised_logistics_retail_hospitality_0035_heavy_duty_counter]",
		Category = "Logistics, retail and hospitality",
		Blurb = "Make a heavy-duty mild steel counter for domestic goods handling from prepared mild steel stock.",
		Action = "making heavy-duty mild steel counter for domestic goods handling",
		ActiveCraftItemSdesc = "an in-progress heavy-duty mild steel counter for domestic goods handling craft",
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
			new CatalogueCommodityRecipeInput(Grams: 3479.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3479 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_logistics_retail_hospitality_0035_heavy_duty_counter", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a heavy-duty mild steel counter for domestic goods handling (@industrialised_logistics_retail_hospitality_0035_heavy_duty_counter)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition LogisticsRetailAndHospitalityMakeHeavyDutyPaperScaleForDomesticGoodsHandlingCraftIndustrialisedLogisticsRetailHospitality0036HeavyDutyScale() => new()
	{
		Metadata = new("Logistics, retail and hospitality/make heavy-duty paper scale for domestic goods handling [craft_industrialised_logistics_retail_hospitality_0036_heavy_duty_scale]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_logistics_retail_hospitality_0036_heavy_duty_scale"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make heavy-duty paper scale for domestic goods handling [craft_industrialised_logistics_retail_hospitality_0036_heavy_duty_scale]",
		Category = "Logistics, retail and hospitality",
		Blurb = "Make a heavy-duty paper scale for domestic goods handling from prepared paper stock.",
		Action = "making heavy-duty paper scale for domestic goods handling",
		ActiveCraftItemSdesc = "an in-progress heavy-duty paper scale for domestic goods handling craft",
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
			new CatalogueCommodityRecipeInput(Grams: 3520.0, Material: "paper", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3520 grams of paper", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_logistics_retail_hospitality_0036_heavy_duty_scale", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a heavy-duty paper scale for domestic goods handling (@industrialised_logistics_retail_hospitality_0036_heavy_duty_scale)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition LogisticsRetailAndHospitalityMakeHeavyDutyCottonTicketForDomesticGoodsHandlingCraftIndustrialisedLogisticsRetailHospitality0037HeavyDutyTicket() => new()
	{
		Metadata = new("Logistics, retail and hospitality/make heavy-duty cotton ticket for domestic goods handling [craft_industrialised_logistics_retail_hospitality_0037_heavy_duty_ticket]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_logistics_retail_hospitality_0037_heavy_duty_ticket"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make heavy-duty cotton ticket for domestic goods handling [craft_industrialised_logistics_retail_hospitality_0037_heavy_duty_ticket]",
		Category = "Logistics, retail and hospitality",
		Blurb = "Make a heavy-duty cotton ticket for domestic goods handling from prepared cotton stock.",
		Action = "making heavy-duty cotton ticket for domestic goods handling",
		ActiveCraftItemSdesc = "an in-progress heavy-duty cotton ticket for domestic goods handling craft",
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
			new CatalogueCommodityRecipeInput(Grams: 3561.0, Material: "cotton", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3561 grams of cotton", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_logistics_retail_hospitality_0037_heavy_duty_ticket", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a heavy-duty cotton ticket for domestic goods handling (@industrialised_logistics_retail_hospitality_0037_heavy_duty_ticket)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition LogisticsRetailAndHospitalityMakeHeavyDutyOakBasketForDomesticGoodsHandlingCraftIndustrialisedLogisticsRetailHospitality0038HeavyDutyBasket() => new()
	{
		Metadata = new("Logistics, retail and hospitality/make heavy-duty oak basket for domestic goods handling [craft_industrialised_logistics_retail_hospitality_0038_heavy_duty_basket]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_logistics_retail_hospitality_0038_heavy_duty_basket"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make heavy-duty oak basket for domestic goods handling [craft_industrialised_logistics_retail_hospitality_0038_heavy_duty_basket]",
		Category = "Logistics, retail and hospitality",
		Blurb = "Make a heavy-duty oak basket for domestic goods handling from prepared oak stock.",
		Action = "making heavy-duty oak basket for domestic goods handling",
		ActiveCraftItemSdesc = "an in-progress heavy-duty oak basket for domestic goods handling craft",
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
			new CatalogueCommodityRecipeInput(Grams: 3601.0, Material: "oak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3601 grams of oak", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_logistics_retail_hospitality_0038_heavy_duty_basket", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a heavy-duty oak basket for domestic goods handling (@industrialised_logistics_retail_hospitality_0038_heavy_duty_basket)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition LogisticsRetailAndHospitalityMakeHeavyDutyMildSteelCaseForDomesticGoodsHandlingCraftIndustrialisedLogisticsRetailHospitality0039HeavyDutyCase() => new()
	{
		Metadata = new("Logistics, retail and hospitality/make heavy-duty mild steel case for domestic goods handling [craft_industrialised_logistics_retail_hospitality_0039_heavy_duty_case]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_logistics_retail_hospitality_0039_heavy_duty_case"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make heavy-duty mild steel case for domestic goods handling [craft_industrialised_logistics_retail_hospitality_0039_heavy_duty_case]",
		Category = "Logistics, retail and hospitality",
		Blurb = "Make a heavy-duty mild steel case for domestic goods handling from prepared mild steel stock.",
		Action = "making heavy-duty mild steel case for domestic goods handling",
		ActiveCraftItemSdesc = "an in-progress heavy-duty mild steel case for domestic goods handling craft",
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
			new CatalogueCommodityRecipeInput(Grams: 3642.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3642 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_logistics_retail_hospitality_0039_heavy_duty_case", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a heavy-duty mild steel case for domestic goods handling (@industrialised_logistics_retail_hospitality_0039_heavy_duty_case)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition LogisticsRetailAndHospitalityMakeHeavyDutyPaperSignForDomesticGoodsHandlingCraftIndustrialisedLogisticsRetailHospitality0040HeavyDutySign() => new()
	{
		Metadata = new("Logistics, retail and hospitality/make heavy-duty paper sign for domestic goods handling [craft_industrialised_logistics_retail_hospitality_0040_heavy_duty_sign]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_logistics_retail_hospitality_0040_heavy_duty_sign"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make heavy-duty paper sign for domestic goods handling [craft_industrialised_logistics_retail_hospitality_0040_heavy_duty_sign]",
		Category = "Logistics, retail and hospitality",
		Blurb = "Make a heavy-duty paper sign for domestic goods handling from prepared paper stock.",
		Action = "making heavy-duty paper sign for domestic goods handling",
		ActiveCraftItemSdesc = "an in-progress heavy-duty paper sign for domestic goods handling craft",
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
			new CatalogueCommodityRecipeInput(Grams: 3683.0, Material: "paper", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3683 grams of paper", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_logistics_retail_hospitality_0040_heavy_duty_sign", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a heavy-duty paper sign for domestic goods handling (@industrialised_logistics_retail_hospitality_0040_heavy_duty_sign)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition LogisticsRetailAndHospitalityMakeFoldingCottonWasteForDomesticGoodsHandlingCraftIndustrialisedLogisticsRetailHospitality0041FoldingWaste() => new()
	{
		Metadata = new("Logistics, retail and hospitality/make folding cotton waste for domestic goods handling [craft_industrialised_logistics_retail_hospitality_0041_folding_waste]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_logistics_retail_hospitality_0041_folding_waste"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make folding cotton waste for domestic goods handling [craft_industrialised_logistics_retail_hospitality_0041_folding_waste]",
		Category = "Logistics, retail and hospitality",
		Blurb = "Make a folding cotton waste for domestic goods handling from prepared cotton stock.",
		Action = "making folding cotton waste for domestic goods handling",
		ActiveCraftItemSdesc = "an in-progress folding cotton waste for domestic goods handling craft",
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
			new CatalogueCommodityRecipeInput(Grams: 3724.0, Material: "cotton", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3724 grams of cotton", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_logistics_retail_hospitality_0041_folding_waste", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a folding cotton waste for domestic goods handling (@industrialised_logistics_retail_hospitality_0041_folding_waste)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition LogisticsRetailAndHospitalityMakeFoldingOakCrateForDomesticGoodsHandlingCraftIndustrialisedLogisticsRetailHospitality0042FoldingCrate() => new()
	{
		Metadata = new("Logistics, retail and hospitality/make folding oak crate for domestic goods handling [craft_industrialised_logistics_retail_hospitality_0042_folding_crate]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_logistics_retail_hospitality_0042_folding_crate"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make folding oak crate for domestic goods handling [craft_industrialised_logistics_retail_hospitality_0042_folding_crate]",
		Category = "Logistics, retail and hospitality",
		Blurb = "Make a folding oak crate for domestic goods handling from prepared oak stock.",
		Action = "making folding oak crate for domestic goods handling",
		ActiveCraftItemSdesc = "an in-progress folding oak crate for domestic goods handling craft",
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
			new CatalogueCommodityRecipeInput(Grams: 3764.0, Material: "oak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3764 grams of oak", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_logistics_retail_hospitality_0042_folding_crate", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a folding oak crate for domestic goods handling (@industrialised_logistics_retail_hospitality_0042_folding_crate)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition LogisticsRetailAndHospitalityMakeFoldingMildSteelTrolleyForDomesticGoodsHandlingCraftIndustrialisedLogisticsRetailHospitality0043FoldingTrolley() => new()
	{
		Metadata = new("Logistics, retail and hospitality/make folding mild steel trolley for domestic goods handling [craft_industrialised_logistics_retail_hospitality_0043_folding_trolley]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_logistics_retail_hospitality_0043_folding_trolley"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make folding mild steel trolley for domestic goods handling [craft_industrialised_logistics_retail_hospitality_0043_folding_trolley]",
		Category = "Logistics, retail and hospitality",
		Blurb = "Make a folding mild steel trolley for domestic goods handling from prepared mild steel stock.",
		Action = "making folding mild steel trolley for domestic goods handling",
		ActiveCraftItemSdesc = "an in-progress folding mild steel trolley for domestic goods handling craft",
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
			new CatalogueCommodityRecipeInput(Grams: 3805.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3805 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_logistics_retail_hospitality_0043_folding_trolley", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a folding mild steel trolley for domestic goods handling (@industrialised_logistics_retail_hospitality_0043_folding_trolley)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition LogisticsRetailAndHospitalityMakeFoldingPaperTrayForDomesticGoodsHandlingCraftIndustrialisedLogisticsRetailHospitality0044FoldingTray() => new()
	{
		Metadata = new("Logistics, retail and hospitality/make folding paper tray for domestic goods handling [craft_industrialised_logistics_retail_hospitality_0044_folding_tray]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_logistics_retail_hospitality_0044_folding_tray"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make folding paper tray for domestic goods handling [craft_industrialised_logistics_retail_hospitality_0044_folding_tray]",
		Category = "Logistics, retail and hospitality",
		Blurb = "Make a folding paper tray for domestic goods handling from prepared paper stock.",
		Action = "making folding paper tray for domestic goods handling",
		ActiveCraftItemSdesc = "an in-progress folding paper tray for domestic goods handling craft",
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
			new CatalogueCommodityRecipeInput(Grams: 3846.0, Material: "paper", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3846 grams of paper", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_logistics_retail_hospitality_0044_folding_tray", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a folding paper tray for domestic goods handling (@industrialised_logistics_retail_hospitality_0044_folding_tray)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition LogisticsRetailAndHospitalityMakeFoldingCottonCounterForDomesticGoodsHandlingCraftIndustrialisedLogisticsRetailHospitality0045FoldingCounter() => new()
	{
		Metadata = new("Logistics, retail and hospitality/make folding cotton counter for domestic goods handling [craft_industrialised_logistics_retail_hospitality_0045_folding_counter]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_logistics_retail_hospitality_0045_folding_counter"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make folding cotton counter for domestic goods handling [craft_industrialised_logistics_retail_hospitality_0045_folding_counter]",
		Category = "Logistics, retail and hospitality",
		Blurb = "Make a folding cotton counter for domestic goods handling from prepared cotton stock.",
		Action = "making folding cotton counter for domestic goods handling",
		ActiveCraftItemSdesc = "an in-progress folding cotton counter for domestic goods handling craft",
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
			new CatalogueCommodityRecipeInput(Grams: 3886.0, Material: "cotton", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3886 grams of cotton", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_logistics_retail_hospitality_0045_folding_counter", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a folding cotton counter for domestic goods handling (@industrialised_logistics_retail_hospitality_0045_folding_counter)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition LogisticsRetailAndHospitalityMakeFoldingOakScaleForDomesticGoodsHandlingCraftIndustrialisedLogisticsRetailHospitality0046FoldingScale() => new()
	{
		Metadata = new("Logistics, retail and hospitality/make folding oak scale for domestic goods handling [craft_industrialised_logistics_retail_hospitality_0046_folding_scale]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_logistics_retail_hospitality_0046_folding_scale"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make folding oak scale for domestic goods handling [craft_industrialised_logistics_retail_hospitality_0046_folding_scale]",
		Category = "Logistics, retail and hospitality",
		Blurb = "Make a folding oak scale for domestic goods handling from prepared oak stock.",
		Action = "making folding oak scale for domestic goods handling",
		ActiveCraftItemSdesc = "an in-progress folding oak scale for domestic goods handling craft",
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
			new CatalogueCommodityRecipeInput(Grams: 3927.0, Material: "oak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3927 grams of oak", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_logistics_retail_hospitality_0046_folding_scale", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a folding oak scale for domestic goods handling (@industrialised_logistics_retail_hospitality_0046_folding_scale)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition LogisticsRetailAndHospitalityMakeFoldingMildSteelTicketForDomesticGoodsHandlingCraftIndustrialisedLogisticsRetailHospitality0047FoldingTicket() => new()
	{
		Metadata = new("Logistics, retail and hospitality/make folding mild steel ticket for domestic goods handling [craft_industrialised_logistics_retail_hospitality_0047_folding_ticket]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_logistics_retail_hospitality_0047_folding_ticket"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make folding mild steel ticket for domestic goods handling [craft_industrialised_logistics_retail_hospitality_0047_folding_ticket]",
		Category = "Logistics, retail and hospitality",
		Blurb = "Make a folding mild steel ticket for domestic goods handling from prepared mild steel stock.",
		Action = "making folding mild steel ticket for domestic goods handling",
		ActiveCraftItemSdesc = "an in-progress folding mild steel ticket for domestic goods handling craft",
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
			new CatalogueCommodityRecipeInput(Grams: 3968.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3968 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_logistics_retail_hospitality_0047_folding_ticket", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a folding mild steel ticket for domestic goods handling (@industrialised_logistics_retail_hospitality_0047_folding_ticket)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};
}
