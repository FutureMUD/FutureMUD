#nullable enable

using MudSharp.Body;
using MudSharp.GameItems.Inventory.Plans;
using MudSharp.RPG.Checks;

namespace DatabaseSeeder.Seeders;

public static partial class Recipes22Stock
{
	public static CatalogueRecipeDefinition MedicalAndMobilityMakeSomePlainLinenForcepsForDomesticCareCraftIndustrialisedMedicalMobility0007PlainForceps() => new()
	{
		Metadata = new("Medical and mobility/make some plain linen forceps for domestic care [craft_industrialised_medical_mobility_0007_plain_forceps]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_medical_mobility_0007_plain_forceps"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make some plain linen forceps for domestic care [craft_industrialised_medical_mobility_0007_plain_forceps]",
		Category = "Medical and mobility",
		Blurb = "Make some plain linen forceps for domestic care from prepared linen stock.",
		Action = "making some plain linen forceps for domestic care",
		ActiveCraftItemSdesc = "an in-progress some plain linen forceps for domestic care craft",
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
			new CatalogueCommodityRecipeInput(Grams: 1895.0, Material: "linen", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1895 grams of linen", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_medical_mobility_0007_plain_forceps", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x some plain linen forceps for domestic care (@industrialised_medical_mobility_0007_plain_forceps)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MedicalAndMobilityMakePlainMildSteelThermometerForDomesticCareCraftIndustrialisedMedicalMobility0008PlainThermometer() => new()
	{
		Metadata = new("Medical and mobility/make plain mild steel thermometer for domestic care [craft_industrialised_medical_mobility_0008_plain_thermometer]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_medical_mobility_0008_plain_thermometer"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make plain mild steel thermometer for domestic care [craft_industrialised_medical_mobility_0008_plain_thermometer]",
		Category = "Medical and mobility",
		Blurb = "Make a plain mild steel thermometer for domestic care from prepared mild steel stock.",
		Action = "making plain mild steel thermometer for domestic care",
		ActiveCraftItemSdesc = "an in-progress plain mild steel thermometer for domestic care craft",
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
			new CatalogueCommodityRecipeInput(Grams: 1936.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1936 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_medical_mobility_0008_plain_thermometer", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a plain mild steel thermometer for domestic care (@industrialised_medical_mobility_0008_plain_thermometer)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MedicalAndMobilityMakePlainGlassCaseForDomesticCareCraftIndustrialisedMedicalMobility0009PlainCase() => new()
	{
		Metadata = new("Medical and mobility/make plain glass case for domestic care [craft_industrialised_medical_mobility_0009_plain_case]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_medical_mobility_0009_plain_case"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make plain glass case for domestic care [craft_industrialised_medical_mobility_0009_plain_case]",
		Category = "Medical and mobility",
		Blurb = "Make a plain glass case for domestic care from prepared glass stock.",
		Action = "making plain glass case for domestic care",
		ActiveCraftItemSdesc = "an in-progress plain glass case for domestic care craft",
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
			new CatalogueCommodityRecipeInput(Grams: 1977.0, Material: "glass", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1977 grams of glass", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_medical_mobility_0009_plain_case", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a plain glass case for domestic care (@industrialised_medical_mobility_0009_plain_case)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MedicalAndMobilityMakePlainOakBraceForDomesticCareCraftIndustrialisedMedicalMobility0010PlainBrace() => new()
	{
		Metadata = new("Medical and mobility/make plain oak brace for domestic care [craft_industrialised_medical_mobility_0010_plain_brace]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_medical_mobility_0010_plain_brace"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make plain oak brace for domestic care [craft_industrialised_medical_mobility_0010_plain_brace]",
		Category = "Medical and mobility",
		Blurb = "Make a plain oak brace for domestic care from prepared oak stock.",
		Action = "making plain oak brace for domestic care",
		ActiveCraftItemSdesc = "an in-progress plain oak brace for domestic care craft",
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
			new CatalogueCommodityRecipeInput(Grams: 2017.0, Material: "oak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2017 grams of oak", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_medical_mobility_0010_plain_brace", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a plain oak brace for domestic care (@industrialised_medical_mobility_0010_plain_brace)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MedicalAndMobilityMakeReinforcedCottonWasteForDomesticCareCraftIndustrialisedMedicalMobility0011ReinforcedWaste() => new()
	{
		Metadata = new("Medical and mobility/make reinforced cotton waste for domestic care [craft_industrialised_medical_mobility_0011_reinforced_waste]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_medical_mobility_0011_reinforced_waste"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make reinforced cotton waste for domestic care [craft_industrialised_medical_mobility_0011_reinforced_waste]",
		Category = "Medical and mobility",
		Blurb = "Make a reinforced cotton waste for domestic care from prepared cotton stock.",
		Action = "making reinforced cotton waste for domestic care",
		ActiveCraftItemSdesc = "an in-progress reinforced cotton waste for domestic care craft",
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
			new CatalogueCommodityRecipeInput(Grams: 2058.0, Material: "cotton", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2058 grams of cotton", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_medical_mobility_0011_reinforced_waste", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a reinforced cotton waste for domestic care (@industrialised_medical_mobility_0011_reinforced_waste)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MedicalAndMobilityMakeReinforcedLinenBandageForDomesticCareCraftIndustrialisedMedicalMobility0012ReinforcedBandage() => new()
	{
		Metadata = new("Medical and mobility/make reinforced linen bandage for domestic care [craft_industrialised_medical_mobility_0012_reinforced_bandage]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_medical_mobility_0012_reinforced_bandage"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make reinforced linen bandage for domestic care [craft_industrialised_medical_mobility_0012_reinforced_bandage]",
		Category = "Medical and mobility",
		Blurb = "Make a reinforced linen bandage for domestic care from prepared linen stock.",
		Action = "making reinforced linen bandage for domestic care",
		ActiveCraftItemSdesc = "an in-progress reinforced linen bandage for domestic care craft",
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
			new CatalogueCommodityRecipeInput(Grams: 2099.0, Material: "linen", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2099 grams of linen", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_medical_mobility_0012_reinforced_bandage", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a reinforced linen bandage for domestic care (@industrialised_medical_mobility_0012_reinforced_bandage)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MedicalAndMobilityMakeReinforcedMildSteelSplintForDomesticCareCraftIndustrialisedMedicalMobility0013ReinforcedSplint() => new()
	{
		Metadata = new("Medical and mobility/make reinforced mild steel splint for domestic care [craft_industrialised_medical_mobility_0013_reinforced_splint]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_medical_mobility_0013_reinforced_splint"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make reinforced mild steel splint for domestic care [craft_industrialised_medical_mobility_0013_reinforced_splint]",
		Category = "Medical and mobility",
		Blurb = "Make a reinforced mild steel splint for domestic care from prepared mild steel stock.",
		Action = "making reinforced mild steel splint for domestic care",
		ActiveCraftItemSdesc = "an in-progress reinforced mild steel splint for domestic care craft",
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
			new CatalogueCommodityRecipeInput(Grams: 2140.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2140 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_medical_mobility_0013_reinforced_splint", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a reinforced mild steel splint for domestic care (@industrialised_medical_mobility_0013_reinforced_splint)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MedicalAndMobilityMakeReinforcedGlassCrutchForDomesticCareCraftIndustrialisedMedicalMobility0014ReinforcedCrutch() => new()
	{
		Metadata = new("Medical and mobility/make reinforced glass crutch for domestic care [craft_industrialised_medical_mobility_0014_reinforced_crutch]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_medical_mobility_0014_reinforced_crutch"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make reinforced glass crutch for domestic care [craft_industrialised_medical_mobility_0014_reinforced_crutch]",
		Category = "Medical and mobility",
		Blurb = "Make a reinforced glass crutch for domestic care from prepared glass stock.",
		Action = "making reinforced glass crutch for domestic care",
		ActiveCraftItemSdesc = "an in-progress reinforced glass crutch for domestic care craft",
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
			new CatalogueCommodityRecipeInput(Grams: 2180.0, Material: "glass", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2180 grams of glass", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_medical_mobility_0014_reinforced_crutch", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a reinforced glass crutch for domestic care (@industrialised_medical_mobility_0014_reinforced_crutch)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MedicalAndMobilityMakeReinforcedOakStretcherForDomesticCareCraftIndustrialisedMedicalMobility0015ReinforcedStretcher() => new()
	{
		Metadata = new("Medical and mobility/make reinforced oak stretcher for domestic care [craft_industrialised_medical_mobility_0015_reinforced_stretcher]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_medical_mobility_0015_reinforced_stretcher"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make reinforced oak stretcher for domestic care [craft_industrialised_medical_mobility_0015_reinforced_stretcher]",
		Category = "Medical and mobility",
		Blurb = "Make a reinforced oak stretcher for domestic care from prepared oak stock.",
		Action = "making reinforced oak stretcher for domestic care",
		ActiveCraftItemSdesc = "an in-progress reinforced oak stretcher for domestic care craft",
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
			new CatalogueCommodityRecipeInput(Grams: 2221.0, Material: "oak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2221 grams of oak", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_medical_mobility_0015_reinforced_stretcher", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a reinforced oak stretcher for domestic care (@industrialised_medical_mobility_0015_reinforced_stretcher)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MedicalAndMobilityMakeReinforcedCottonSyringeForDomesticCareCraftIndustrialisedMedicalMobility0016ReinforcedSyringe() => new()
	{
		Metadata = new("Medical and mobility/make reinforced cotton syringe for domestic care [craft_industrialised_medical_mobility_0016_reinforced_syringe]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_medical_mobility_0016_reinforced_syringe"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make reinforced cotton syringe for domestic care [craft_industrialised_medical_mobility_0016_reinforced_syringe]",
		Category = "Medical and mobility",
		Blurb = "Make a reinforced cotton syringe for domestic care from prepared cotton stock.",
		Action = "making reinforced cotton syringe for domestic care",
		ActiveCraftItemSdesc = "an in-progress reinforced cotton syringe for domestic care craft",
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
			new CatalogueCommodityRecipeInput(Grams: 2262.0, Material: "cotton", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2262 grams of cotton", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_medical_mobility_0016_reinforced_syringe", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a reinforced cotton syringe for domestic care (@industrialised_medical_mobility_0016_reinforced_syringe)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MedicalAndMobilityMakeSomeReinforcedLinenForcepsForDomesticCareCraftIndustrialisedMedicalMobility0017ReinforcedForceps() => new()
	{
		Metadata = new("Medical and mobility/make some reinforced linen forceps for domestic care [craft_industrialised_medical_mobility_0017_reinforced_forceps]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_medical_mobility_0017_reinforced_forceps"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make some reinforced linen forceps for domestic care [craft_industrialised_medical_mobility_0017_reinforced_forceps]",
		Category = "Medical and mobility",
		Blurb = "Make some reinforced linen forceps for domestic care from prepared linen stock.",
		Action = "making some reinforced linen forceps for domestic care",
		ActiveCraftItemSdesc = "an in-progress some reinforced linen forceps for domestic care craft",
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
			new CatalogueCommodityRecipeInput(Grams: 2302.0, Material: "linen", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2302 grams of linen", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_medical_mobility_0017_reinforced_forceps", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x some reinforced linen forceps for domestic care (@industrialised_medical_mobility_0017_reinforced_forceps)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MedicalAndMobilityMakeReinforcedMildSteelThermometerForDomesticCareCraftIndustrialisedMedicalMobility0018ReinforcedThermometer() => new()
	{
		Metadata = new("Medical and mobility/make reinforced mild steel thermometer for domestic care [craft_industrialised_medical_mobility_0018_reinforced_thermometer]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_medical_mobility_0018_reinforced_thermometer"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make reinforced mild steel thermometer for domestic care [craft_industrialised_medical_mobility_0018_reinforced_thermometer]",
		Category = "Medical and mobility",
		Blurb = "Make a reinforced mild steel thermometer for domestic care from prepared mild steel stock.",
		Action = "making reinforced mild steel thermometer for domestic care",
		ActiveCraftItemSdesc = "an in-progress reinforced mild steel thermometer for domestic care craft",
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
			new CatalogueCommodityRecipeInput(Grams: 2343.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2343 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_medical_mobility_0018_reinforced_thermometer", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a reinforced mild steel thermometer for domestic care (@industrialised_medical_mobility_0018_reinforced_thermometer)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MedicalAndMobilityMakeReinforcedGlassCaseForDomesticCareCraftIndustrialisedMedicalMobility0019ReinforcedCase() => new()
	{
		Metadata = new("Medical and mobility/make reinforced glass case for domestic care [craft_industrialised_medical_mobility_0019_reinforced_case]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_medical_mobility_0019_reinforced_case"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make reinforced glass case for domestic care [craft_industrialised_medical_mobility_0019_reinforced_case]",
		Category = "Medical and mobility",
		Blurb = "Make a reinforced glass case for domestic care from prepared glass stock.",
		Action = "making reinforced glass case for domestic care",
		ActiveCraftItemSdesc = "an in-progress reinforced glass case for domestic care craft",
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
			new CatalogueCommodityRecipeInput(Grams: 2384.0, Material: "glass", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2384 grams of glass", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_medical_mobility_0019_reinforced_case", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a reinforced glass case for domestic care (@industrialised_medical_mobility_0019_reinforced_case)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MedicalAndMobilityMakeReinforcedOakBraceForDomesticCareCraftIndustrialisedMedicalMobility0020ReinforcedBrace() => new()
	{
		Metadata = new("Medical and mobility/make reinforced oak brace for domestic care [craft_industrialised_medical_mobility_0020_reinforced_brace]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_medical_mobility_0020_reinforced_brace"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make reinforced oak brace for domestic care [craft_industrialised_medical_mobility_0020_reinforced_brace]",
		Category = "Medical and mobility",
		Blurb = "Make a reinforced oak brace for domestic care from prepared oak stock.",
		Action = "making reinforced oak brace for domestic care",
		ActiveCraftItemSdesc = "an in-progress reinforced oak brace for domestic care craft",
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
			new CatalogueCommodityRecipeInput(Grams: 2424.0, Material: "oak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2424 grams of oak", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_medical_mobility_0020_reinforced_brace", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a reinforced oak brace for domestic care (@industrialised_medical_mobility_0020_reinforced_brace)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MedicalAndMobilityMakeCompactCottonWasteForDomesticCareCraftIndustrialisedMedicalMobility0021CompactWaste() => new()
	{
		Metadata = new("Medical and mobility/make compact cotton waste for domestic care [craft_industrialised_medical_mobility_0021_compact_waste]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_medical_mobility_0021_compact_waste"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make compact cotton waste for domestic care [craft_industrialised_medical_mobility_0021_compact_waste]",
		Category = "Medical and mobility",
		Blurb = "Make a compact cotton waste for domestic care from prepared cotton stock.",
		Action = "making compact cotton waste for domestic care",
		ActiveCraftItemSdesc = "an in-progress compact cotton waste for domestic care craft",
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
			new CatalogueCommodityRecipeInput(Grams: 2465.0, Material: "cotton", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2465 grams of cotton", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_medical_mobility_0021_compact_waste", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a compact cotton waste for domestic care (@industrialised_medical_mobility_0021_compact_waste)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MedicalAndMobilityMakeCompactLinenBandageForDomesticCareCraftIndustrialisedMedicalMobility0022CompactBandage() => new()
	{
		Metadata = new("Medical and mobility/make compact linen bandage for domestic care [craft_industrialised_medical_mobility_0022_compact_bandage]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_medical_mobility_0022_compact_bandage"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make compact linen bandage for domestic care [craft_industrialised_medical_mobility_0022_compact_bandage]",
		Category = "Medical and mobility",
		Blurb = "Make a compact linen bandage for domestic care from prepared linen stock.",
		Action = "making compact linen bandage for domestic care",
		ActiveCraftItemSdesc = "an in-progress compact linen bandage for domestic care craft",
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
			new CatalogueCommodityRecipeInput(Grams: 2506.0, Material: "linen", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2506 grams of linen", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_medical_mobility_0022_compact_bandage", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a compact linen bandage for domestic care (@industrialised_medical_mobility_0022_compact_bandage)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MedicalAndMobilityMakeCompactMildSteelSplintForDomesticCareCraftIndustrialisedMedicalMobility0023CompactSplint() => new()
	{
		Metadata = new("Medical and mobility/make compact mild steel splint for domestic care [craft_industrialised_medical_mobility_0023_compact_splint]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_medical_mobility_0023_compact_splint"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make compact mild steel splint for domestic care [craft_industrialised_medical_mobility_0023_compact_splint]",
		Category = "Medical and mobility",
		Blurb = "Make a compact mild steel splint for domestic care from prepared mild steel stock.",
		Action = "making compact mild steel splint for domestic care",
		ActiveCraftItemSdesc = "an in-progress compact mild steel splint for domestic care craft",
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
			new CatalogueCommodityRecipeInput(Grams: 2546.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2546 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_medical_mobility_0023_compact_splint", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a compact mild steel splint for domestic care (@industrialised_medical_mobility_0023_compact_splint)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MedicalAndMobilityMakeCompactGlassCrutchForDomesticCareCraftIndustrialisedMedicalMobility0024CompactCrutch() => new()
	{
		Metadata = new("Medical and mobility/make compact glass crutch for domestic care [craft_industrialised_medical_mobility_0024_compact_crutch]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_medical_mobility_0024_compact_crutch"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make compact glass crutch for domestic care [craft_industrialised_medical_mobility_0024_compact_crutch]",
		Category = "Medical and mobility",
		Blurb = "Make a compact glass crutch for domestic care from prepared glass stock.",
		Action = "making compact glass crutch for domestic care",
		ActiveCraftItemSdesc = "an in-progress compact glass crutch for domestic care craft",
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
			new CatalogueCommodityRecipeInput(Grams: 2587.0, Material: "glass", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2587 grams of glass", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_medical_mobility_0024_compact_crutch", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a compact glass crutch for domestic care (@industrialised_medical_mobility_0024_compact_crutch)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MedicalAndMobilityMakeCompactOakStretcherForDomesticCareCraftIndustrialisedMedicalMobility0025CompactStretcher() => new()
	{
		Metadata = new("Medical and mobility/make compact oak stretcher for domestic care [craft_industrialised_medical_mobility_0025_compact_stretcher]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_medical_mobility_0025_compact_stretcher"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make compact oak stretcher for domestic care [craft_industrialised_medical_mobility_0025_compact_stretcher]",
		Category = "Medical and mobility",
		Blurb = "Make a compact oak stretcher for domestic care from prepared oak stock.",
		Action = "making compact oak stretcher for domestic care",
		ActiveCraftItemSdesc = "an in-progress compact oak stretcher for domestic care craft",
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
			new CatalogueCommodityRecipeInput(Grams: 2628.0, Material: "oak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2628 grams of oak", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_medical_mobility_0025_compact_stretcher", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a compact oak stretcher for domestic care (@industrialised_medical_mobility_0025_compact_stretcher)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MedicalAndMobilityMakeCompactCottonSyringeForDomesticCareCraftIndustrialisedMedicalMobility0026CompactSyringe() => new()
	{
		Metadata = new("Medical and mobility/make compact cotton syringe for domestic care [craft_industrialised_medical_mobility_0026_compact_syringe]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_medical_mobility_0026_compact_syringe"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make compact cotton syringe for domestic care [craft_industrialised_medical_mobility_0026_compact_syringe]",
		Category = "Medical and mobility",
		Blurb = "Make a compact cotton syringe for domestic care from prepared cotton stock.",
		Action = "making compact cotton syringe for domestic care",
		ActiveCraftItemSdesc = "an in-progress compact cotton syringe for domestic care craft",
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
			new CatalogueCommodityRecipeInput(Grams: 2669.0, Material: "cotton", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2669 grams of cotton", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_medical_mobility_0026_compact_syringe", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a compact cotton syringe for domestic care (@industrialised_medical_mobility_0026_compact_syringe)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MedicalAndMobilityMakeSomeCompactLinenForcepsForDomesticCareCraftIndustrialisedMedicalMobility0027CompactForceps() => new()
	{
		Metadata = new("Medical and mobility/make some compact linen forceps for domestic care [craft_industrialised_medical_mobility_0027_compact_forceps]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_medical_mobility_0027_compact_forceps"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make some compact linen forceps for domestic care [craft_industrialised_medical_mobility_0027_compact_forceps]",
		Category = "Medical and mobility",
		Blurb = "Make some compact linen forceps for domestic care from prepared linen stock.",
		Action = "making some compact linen forceps for domestic care",
		ActiveCraftItemSdesc = "an in-progress some compact linen forceps for domestic care craft",
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
			new CatalogueCommodityRecipeInput(Grams: 2709.0, Material: "linen", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2709 grams of linen", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_medical_mobility_0027_compact_forceps", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x some compact linen forceps for domestic care (@industrialised_medical_mobility_0027_compact_forceps)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MedicalAndMobilityMakeCompactMildSteelThermometerForDomesticCareCraftIndustrialisedMedicalMobility0028CompactThermometer() => new()
	{
		Metadata = new("Medical and mobility/make compact mild steel thermometer for domestic care [craft_industrialised_medical_mobility_0028_compact_thermometer]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_medical_mobility_0028_compact_thermometer"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make compact mild steel thermometer for domestic care [craft_industrialised_medical_mobility_0028_compact_thermometer]",
		Category = "Medical and mobility",
		Blurb = "Make a compact mild steel thermometer for domestic care from prepared mild steel stock.",
		Action = "making compact mild steel thermometer for domestic care",
		ActiveCraftItemSdesc = "an in-progress compact mild steel thermometer for domestic care craft",
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
			new CatalogueCommodityRecipeInput(Grams: 2750.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2750 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_medical_mobility_0028_compact_thermometer", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a compact mild steel thermometer for domestic care (@industrialised_medical_mobility_0028_compact_thermometer)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MedicalAndMobilityMakeCompactGlassCaseForDomesticCareCraftIndustrialisedMedicalMobility0029CompactCase() => new()
	{
		Metadata = new("Medical and mobility/make compact glass case for domestic care [craft_industrialised_medical_mobility_0029_compact_case]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_medical_mobility_0029_compact_case"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make compact glass case for domestic care [craft_industrialised_medical_mobility_0029_compact_case]",
		Category = "Medical and mobility",
		Blurb = "Make a compact glass case for domestic care from prepared glass stock.",
		Action = "making compact glass case for domestic care",
		ActiveCraftItemSdesc = "an in-progress compact glass case for domestic care craft",
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
			new CatalogueCommodityRecipeInput(Grams: 2791.0, Material: "glass", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2791 grams of glass", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_medical_mobility_0029_compact_case", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a compact glass case for domestic care (@industrialised_medical_mobility_0029_compact_case)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MedicalAndMobilityMakeCompactOakBraceForDomesticCareCraftIndustrialisedMedicalMobility0030CompactBrace() => new()
	{
		Metadata = new("Medical and mobility/make compact oak brace for domestic care [craft_industrialised_medical_mobility_0030_compact_brace]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_medical_mobility_0030_compact_brace"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make compact oak brace for domestic care [craft_industrialised_medical_mobility_0030_compact_brace]",
		Category = "Medical and mobility",
		Blurb = "Make a compact oak brace for domestic care from prepared oak stock.",
		Action = "making compact oak brace for domestic care",
		ActiveCraftItemSdesc = "an in-progress compact oak brace for domestic care craft",
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
			new CatalogueCommodityRecipeInput(Grams: 2831.0, Material: "oak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2831 grams of oak", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_medical_mobility_0030_compact_brace", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a compact oak brace for domestic care (@industrialised_medical_mobility_0030_compact_brace)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MedicalAndMobilityMakeHeavyDutyCottonWasteForDomesticCareCraftIndustrialisedMedicalMobility0031HeavyDutyWaste() => new()
	{
		Metadata = new("Medical and mobility/make heavy-duty cotton waste for domestic care [craft_industrialised_medical_mobility_0031_heavy_duty_waste]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_medical_mobility_0031_heavy_duty_waste"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make heavy-duty cotton waste for domestic care [craft_industrialised_medical_mobility_0031_heavy_duty_waste]",
		Category = "Medical and mobility",
		Blurb = "Make a heavy-duty cotton waste for domestic care from prepared cotton stock.",
		Action = "making heavy-duty cotton waste for domestic care",
		ActiveCraftItemSdesc = "an in-progress heavy-duty cotton waste for domestic care craft",
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
			new CatalogueCommodityRecipeInput(Grams: 2872.0, Material: "cotton", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2872 grams of cotton", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_medical_mobility_0031_heavy_duty_waste", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a heavy-duty cotton waste for domestic care (@industrialised_medical_mobility_0031_heavy_duty_waste)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MedicalAndMobilityMakeHeavyDutyLinenBandageForDomesticCareCraftIndustrialisedMedicalMobility0032HeavyDutyBandage() => new()
	{
		Metadata = new("Medical and mobility/make heavy-duty linen bandage for domestic care [craft_industrialised_medical_mobility_0032_heavy_duty_bandage]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_medical_mobility_0032_heavy_duty_bandage"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make heavy-duty linen bandage for domestic care [craft_industrialised_medical_mobility_0032_heavy_duty_bandage]",
		Category = "Medical and mobility",
		Blurb = "Make a heavy-duty linen bandage for domestic care from prepared linen stock.",
		Action = "making heavy-duty linen bandage for domestic care",
		ActiveCraftItemSdesc = "an in-progress heavy-duty linen bandage for domestic care craft",
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
			new CatalogueCommodityRecipeInput(Grams: 2913.0, Material: "linen", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2913 grams of linen", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_medical_mobility_0032_heavy_duty_bandage", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a heavy-duty linen bandage for domestic care (@industrialised_medical_mobility_0032_heavy_duty_bandage)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MedicalAndMobilityMakeHeavyDutyMildSteelSplintForDomesticCareCraftIndustrialisedMedicalMobility0033HeavyDutySplint() => new()
	{
		Metadata = new("Medical and mobility/make heavy-duty mild steel splint for domestic care [craft_industrialised_medical_mobility_0033_heavy_duty_splint]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_medical_mobility_0033_heavy_duty_splint"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make heavy-duty mild steel splint for domestic care [craft_industrialised_medical_mobility_0033_heavy_duty_splint]",
		Category = "Medical and mobility",
		Blurb = "Make a heavy-duty mild steel splint for domestic care from prepared mild steel stock.",
		Action = "making heavy-duty mild steel splint for domestic care",
		ActiveCraftItemSdesc = "an in-progress heavy-duty mild steel splint for domestic care craft",
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
			new CatalogueCommodityRecipeInput(Grams: 2954.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2954 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_medical_mobility_0033_heavy_duty_splint", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a heavy-duty mild steel splint for domestic care (@industrialised_medical_mobility_0033_heavy_duty_splint)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MedicalAndMobilityMakeHeavyDutyGlassCrutchForDomesticCareCraftIndustrialisedMedicalMobility0034HeavyDutyCrutch() => new()
	{
		Metadata = new("Medical and mobility/make heavy-duty glass crutch for domestic care [craft_industrialised_medical_mobility_0034_heavy_duty_crutch]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_medical_mobility_0034_heavy_duty_crutch"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make heavy-duty glass crutch for domestic care [craft_industrialised_medical_mobility_0034_heavy_duty_crutch]",
		Category = "Medical and mobility",
		Blurb = "Make a heavy-duty glass crutch for domestic care from prepared glass stock.",
		Action = "making heavy-duty glass crutch for domestic care",
		ActiveCraftItemSdesc = "an in-progress heavy-duty glass crutch for domestic care craft",
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
			new CatalogueCommodityRecipeInput(Grams: 2994.0, Material: "glass", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2994 grams of glass", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_medical_mobility_0034_heavy_duty_crutch", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a heavy-duty glass crutch for domestic care (@industrialised_medical_mobility_0034_heavy_duty_crutch)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MedicalAndMobilityMakeHeavyDutyOakStretcherForDomesticCareCraftIndustrialisedMedicalMobility0035HeavyDutyStretcher() => new()
	{
		Metadata = new("Medical and mobility/make heavy-duty oak stretcher for domestic care [craft_industrialised_medical_mobility_0035_heavy_duty_stretcher]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_medical_mobility_0035_heavy_duty_stretcher"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make heavy-duty oak stretcher for domestic care [craft_industrialised_medical_mobility_0035_heavy_duty_stretcher]",
		Category = "Medical and mobility",
		Blurb = "Make a heavy-duty oak stretcher for domestic care from prepared oak stock.",
		Action = "making heavy-duty oak stretcher for domestic care",
		ActiveCraftItemSdesc = "an in-progress heavy-duty oak stretcher for domestic care craft",
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
			new CatalogueCommodityRecipeInput(Grams: 3035.0, Material: "oak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3035 grams of oak", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_medical_mobility_0035_heavy_duty_stretcher", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a heavy-duty oak stretcher for domestic care (@industrialised_medical_mobility_0035_heavy_duty_stretcher)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MedicalAndMobilityMakeHeavyDutyCottonSyringeForDomesticCareCraftIndustrialisedMedicalMobility0036HeavyDutySyringe() => new()
	{
		Metadata = new("Medical and mobility/make heavy-duty cotton syringe for domestic care [craft_industrialised_medical_mobility_0036_heavy_duty_syringe]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_medical_mobility_0036_heavy_duty_syringe"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make heavy-duty cotton syringe for domestic care [craft_industrialised_medical_mobility_0036_heavy_duty_syringe]",
		Category = "Medical and mobility",
		Blurb = "Make a heavy-duty cotton syringe for domestic care from prepared cotton stock.",
		Action = "making heavy-duty cotton syringe for domestic care",
		ActiveCraftItemSdesc = "an in-progress heavy-duty cotton syringe for domestic care craft",
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
			new CatalogueCommodityRecipeInput(Grams: 3076.0, Material: "cotton", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3076 grams of cotton", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_medical_mobility_0036_heavy_duty_syringe", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a heavy-duty cotton syringe for domestic care (@industrialised_medical_mobility_0036_heavy_duty_syringe)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MedicalAndMobilityMakeSomeHeavyDutyLinenForcepsForDomesticCareCraftIndustrialisedMedicalMobility0037HeavyDutyForceps() => new()
	{
		Metadata = new("Medical and mobility/make some heavy-duty linen forceps for domestic care [craft_industrialised_medical_mobility_0037_heavy_duty_forceps]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_medical_mobility_0037_heavy_duty_forceps"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make some heavy-duty linen forceps for domestic care [craft_industrialised_medical_mobility_0037_heavy_duty_forceps]",
		Category = "Medical and mobility",
		Blurb = "Make some heavy-duty linen forceps for domestic care from prepared linen stock.",
		Action = "making some heavy-duty linen forceps for domestic care",
		ActiveCraftItemSdesc = "an in-progress some heavy-duty linen forceps for domestic care craft",
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
			new CatalogueCommodityRecipeInput(Grams: 3116.0, Material: "linen", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3116 grams of linen", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_medical_mobility_0037_heavy_duty_forceps", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x some heavy-duty linen forceps for domestic care (@industrialised_medical_mobility_0037_heavy_duty_forceps)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MedicalAndMobilityMakeHeavyDutyMildSteelThermometerForDomesticCareCraftIndustrialisedMedicalMobility0038HeavyDutyThermometer() => new()
	{
		Metadata = new("Medical and mobility/make heavy-duty mild steel thermometer for domestic care [craft_industrialised_medical_mobility_0038_heavy_duty_thermometer]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_medical_mobility_0038_heavy_duty_thermometer"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make heavy-duty mild steel thermometer for domestic care [craft_industrialised_medical_mobility_0038_heavy_duty_thermometer]",
		Category = "Medical and mobility",
		Blurb = "Make a heavy-duty mild steel thermometer for domestic care from prepared mild steel stock.",
		Action = "making heavy-duty mild steel thermometer for domestic care",
		ActiveCraftItemSdesc = "an in-progress heavy-duty mild steel thermometer for domestic care craft",
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
			new CatalogueCommodityRecipeInput(Grams: 3157.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3157 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_medical_mobility_0038_heavy_duty_thermometer", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a heavy-duty mild steel thermometer for domestic care (@industrialised_medical_mobility_0038_heavy_duty_thermometer)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MedicalAndMobilityMakeHeavyDutyGlassCaseForDomesticCareCraftIndustrialisedMedicalMobility0039HeavyDutyCase() => new()
	{
		Metadata = new("Medical and mobility/make heavy-duty glass case for domestic care [craft_industrialised_medical_mobility_0039_heavy_duty_case]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_medical_mobility_0039_heavy_duty_case"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make heavy-duty glass case for domestic care [craft_industrialised_medical_mobility_0039_heavy_duty_case]",
		Category = "Medical and mobility",
		Blurb = "Make a heavy-duty glass case for domestic care from prepared glass stock.",
		Action = "making heavy-duty glass case for domestic care",
		ActiveCraftItemSdesc = "an in-progress heavy-duty glass case for domestic care craft",
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
			new CatalogueCommodityRecipeInput(Grams: 3198.0, Material: "glass", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3198 grams of glass", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_medical_mobility_0039_heavy_duty_case", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a heavy-duty glass case for domestic care (@industrialised_medical_mobility_0039_heavy_duty_case)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MedicalAndMobilityMakeHeavyDutyOakBraceForDomesticCareCraftIndustrialisedMedicalMobility0040HeavyDutyBrace() => new()
	{
		Metadata = new("Medical and mobility/make heavy-duty oak brace for domestic care [craft_industrialised_medical_mobility_0040_heavy_duty_brace]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_medical_mobility_0040_heavy_duty_brace"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make heavy-duty oak brace for domestic care [craft_industrialised_medical_mobility_0040_heavy_duty_brace]",
		Category = "Medical and mobility",
		Blurb = "Make a heavy-duty oak brace for domestic care from prepared oak stock.",
		Action = "making heavy-duty oak brace for domestic care",
		ActiveCraftItemSdesc = "an in-progress heavy-duty oak brace for domestic care craft",
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
			new CatalogueCommodityRecipeInput(Grams: 3238.0, Material: "oak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3238 grams of oak", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_medical_mobility_0040_heavy_duty_brace", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a heavy-duty oak brace for domestic care (@industrialised_medical_mobility_0040_heavy_duty_brace)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MedicalAndMobilityMakeFoldingCottonWasteForDomesticCareCraftIndustrialisedMedicalMobility0041FoldingWaste() => new()
	{
		Metadata = new("Medical and mobility/make folding cotton waste for domestic care [craft_industrialised_medical_mobility_0041_folding_waste]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_medical_mobility_0041_folding_waste"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make folding cotton waste for domestic care [craft_industrialised_medical_mobility_0041_folding_waste]",
		Category = "Medical and mobility",
		Blurb = "Make a folding cotton waste for domestic care from prepared cotton stock.",
		Action = "making folding cotton waste for domestic care",
		ActiveCraftItemSdesc = "an in-progress folding cotton waste for domestic care craft",
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
			new CatalogueCommodityRecipeInput(Grams: 3279.0, Material: "cotton", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3279 grams of cotton", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_medical_mobility_0041_folding_waste", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a folding cotton waste for domestic care (@industrialised_medical_mobility_0041_folding_waste)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MedicalAndMobilityMakeFoldingLinenBandageForDomesticCareCraftIndustrialisedMedicalMobility0042FoldingBandage() => new()
	{
		Metadata = new("Medical and mobility/make folding linen bandage for domestic care [craft_industrialised_medical_mobility_0042_folding_bandage]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_medical_mobility_0042_folding_bandage"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make folding linen bandage for domestic care [craft_industrialised_medical_mobility_0042_folding_bandage]",
		Category = "Medical and mobility",
		Blurb = "Make a folding linen bandage for domestic care from prepared linen stock.",
		Action = "making folding linen bandage for domestic care",
		ActiveCraftItemSdesc = "an in-progress folding linen bandage for domestic care craft",
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
			new CatalogueCommodityRecipeInput(Grams: 3320.0, Material: "linen", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3320 grams of linen", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_medical_mobility_0042_folding_bandage", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a folding linen bandage for domestic care (@industrialised_medical_mobility_0042_folding_bandage)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MedicalAndMobilityMakeFoldingMildSteelSplintForDomesticCareCraftIndustrialisedMedicalMobility0043FoldingSplint() => new()
	{
		Metadata = new("Medical and mobility/make folding mild steel splint for domestic care [craft_industrialised_medical_mobility_0043_folding_splint]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_medical_mobility_0043_folding_splint"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make folding mild steel splint for domestic care [craft_industrialised_medical_mobility_0043_folding_splint]",
		Category = "Medical and mobility",
		Blurb = "Make a folding mild steel splint for domestic care from prepared mild steel stock.",
		Action = "making folding mild steel splint for domestic care",
		ActiveCraftItemSdesc = "an in-progress folding mild steel splint for domestic care craft",
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
			new CatalogueCommodityRecipeInput(Grams: 3361.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3361 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_medical_mobility_0043_folding_splint", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a folding mild steel splint for domestic care (@industrialised_medical_mobility_0043_folding_splint)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MedicalAndMobilityMakeFoldingGlassCrutchForDomesticCareCraftIndustrialisedMedicalMobility0044FoldingCrutch() => new()
	{
		Metadata = new("Medical and mobility/make folding glass crutch for domestic care [craft_industrialised_medical_mobility_0044_folding_crutch]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_medical_mobility_0044_folding_crutch"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make folding glass crutch for domestic care [craft_industrialised_medical_mobility_0044_folding_crutch]",
		Category = "Medical and mobility",
		Blurb = "Make a folding glass crutch for domestic care from prepared glass stock.",
		Action = "making folding glass crutch for domestic care",
		ActiveCraftItemSdesc = "an in-progress folding glass crutch for domestic care craft",
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
			new CatalogueCommodityRecipeInput(Grams: 3401.0, Material: "glass", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3401 grams of glass", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_medical_mobility_0044_folding_crutch", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a folding glass crutch for domestic care (@industrialised_medical_mobility_0044_folding_crutch)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MedicalAndMobilityMakeFoldingOakStretcherForDomesticCareCraftIndustrialisedMedicalMobility0045FoldingStretcher() => new()
	{
		Metadata = new("Medical and mobility/make folding oak stretcher for domestic care [craft_industrialised_medical_mobility_0045_folding_stretcher]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_medical_mobility_0045_folding_stretcher"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make folding oak stretcher for domestic care [craft_industrialised_medical_mobility_0045_folding_stretcher]",
		Category = "Medical and mobility",
		Blurb = "Make a folding oak stretcher for domestic care from prepared oak stock.",
		Action = "making folding oak stretcher for domestic care",
		ActiveCraftItemSdesc = "an in-progress folding oak stretcher for domestic care craft",
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
			new CatalogueCommodityRecipeInput(Grams: 3442.0, Material: "oak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3442 grams of oak", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_medical_mobility_0045_folding_stretcher", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a folding oak stretcher for domestic care (@industrialised_medical_mobility_0045_folding_stretcher)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MedicalAndMobilityMakeFoldingCottonSyringeForDomesticCareCraftIndustrialisedMedicalMobility0046FoldingSyringe() => new()
	{
		Metadata = new("Medical and mobility/make folding cotton syringe for domestic care [craft_industrialised_medical_mobility_0046_folding_syringe]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_medical_mobility_0046_folding_syringe"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make folding cotton syringe for domestic care [craft_industrialised_medical_mobility_0046_folding_syringe]",
		Category = "Medical and mobility",
		Blurb = "Make a folding cotton syringe for domestic care from prepared cotton stock.",
		Action = "making folding cotton syringe for domestic care",
		ActiveCraftItemSdesc = "an in-progress folding cotton syringe for domestic care craft",
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
			new CatalogueCommodityRecipeInput(Grams: 3483.0, Material: "cotton", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3483 grams of cotton", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_medical_mobility_0046_folding_syringe", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a folding cotton syringe for domestic care (@industrialised_medical_mobility_0046_folding_syringe)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MedicalAndMobilityMakeSomeFoldingLinenForcepsForDomesticCareCraftIndustrialisedMedicalMobility0047FoldingForceps() => new()
	{
		Metadata = new("Medical and mobility/make some folding linen forceps for domestic care [craft_industrialised_medical_mobility_0047_folding_forceps]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_medical_mobility_0047_folding_forceps"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make some folding linen forceps for domestic care [craft_industrialised_medical_mobility_0047_folding_forceps]",
		Category = "Medical and mobility",
		Blurb = "Make some folding linen forceps for domestic care from prepared linen stock.",
		Action = "making some folding linen forceps for domestic care",
		ActiveCraftItemSdesc = "an in-progress some folding linen forceps for domestic care craft",
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
			new CatalogueCommodityRecipeInput(Grams: 3523.0, Material: "linen", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3523 grams of linen", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_medical_mobility_0047_folding_forceps", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x some folding linen forceps for domestic care (@industrialised_medical_mobility_0047_folding_forceps)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MedicalAndMobilityMakeFoldingMildSteelThermometerForDomesticCareCraftIndustrialisedMedicalMobility0048FoldingThermometer() => new()
	{
		Metadata = new("Medical and mobility/make folding mild steel thermometer for domestic care [craft_industrialised_medical_mobility_0048_folding_thermometer]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_medical_mobility_0048_folding_thermometer"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make folding mild steel thermometer for domestic care [craft_industrialised_medical_mobility_0048_folding_thermometer]",
		Category = "Medical and mobility",
		Blurb = "Make a folding mild steel thermometer for domestic care from prepared mild steel stock.",
		Action = "making folding mild steel thermometer for domestic care",
		ActiveCraftItemSdesc = "an in-progress folding mild steel thermometer for domestic care craft",
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
			new CatalogueCommodityRecipeInput(Grams: 3564.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3564 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_medical_mobility_0048_folding_thermometer", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a folding mild steel thermometer for domestic care (@industrialised_medical_mobility_0048_folding_thermometer)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MedicalAndMobilityMakeFoldingGlassCaseForDomesticCareCraftIndustrialisedMedicalMobility0049FoldingCase() => new()
	{
		Metadata = new("Medical and mobility/make folding glass case for domestic care [craft_industrialised_medical_mobility_0049_folding_case]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_medical_mobility_0049_folding_case"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make folding glass case for domestic care [craft_industrialised_medical_mobility_0049_folding_case]",
		Category = "Medical and mobility",
		Blurb = "Make a folding glass case for domestic care from prepared glass stock.",
		Action = "making folding glass case for domestic care",
		ActiveCraftItemSdesc = "an in-progress folding glass case for domestic care craft",
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
			new CatalogueCommodityRecipeInput(Grams: 3605.0, Material: "glass", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3605 grams of glass", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_medical_mobility_0049_folding_case", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a folding glass case for domestic care (@industrialised_medical_mobility_0049_folding_case)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MedicalAndMobilityMakeFoldingOakBraceForDomesticCareCraftIndustrialisedMedicalMobility0050FoldingBrace() => new()
	{
		Metadata = new("Medical and mobility/make folding oak brace for domestic care [craft_industrialised_medical_mobility_0050_folding_brace]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_medical_mobility_0050_folding_brace"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make folding oak brace for domestic care [craft_industrialised_medical_mobility_0050_folding_brace]",
		Category = "Medical and mobility",
		Blurb = "Make a folding oak brace for domestic care from prepared oak stock.",
		Action = "making folding oak brace for domestic care",
		ActiveCraftItemSdesc = "an in-progress folding oak brace for domestic care craft",
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
			new CatalogueCommodityRecipeInput(Grams: 3645.0, Material: "oak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3645 grams of oak", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_medical_mobility_0050_folding_brace", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a folding oak brace for domestic care (@industrialised_medical_mobility_0050_folding_brace)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MedicalAndMobilityMakePortableCottonWasteForDomesticCareCraftIndustrialisedMedicalMobility0051PortableWaste() => new()
	{
		Metadata = new("Medical and mobility/make portable cotton waste for domestic care [craft_industrialised_medical_mobility_0051_portable_waste]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_medical_mobility_0051_portable_waste"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make portable cotton waste for domestic care [craft_industrialised_medical_mobility_0051_portable_waste]",
		Category = "Medical and mobility",
		Blurb = "Make a portable cotton waste for domestic care from prepared cotton stock.",
		Action = "making portable cotton waste for domestic care",
		ActiveCraftItemSdesc = "an in-progress portable cotton waste for domestic care craft",
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
			new CatalogueCommodityRecipeInput(Grams: 3686.0, Material: "cotton", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3686 grams of cotton", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_medical_mobility_0051_portable_waste", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a portable cotton waste for domestic care (@industrialised_medical_mobility_0051_portable_waste)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MedicalAndMobilityMakePortableLinenBandageForDomesticCareCraftIndustrialisedMedicalMobility0052PortableBandage() => new()
	{
		Metadata = new("Medical and mobility/make portable linen bandage for domestic care [craft_industrialised_medical_mobility_0052_portable_bandage]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_medical_mobility_0052_portable_bandage"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make portable linen bandage for domestic care [craft_industrialised_medical_mobility_0052_portable_bandage]",
		Category = "Medical and mobility",
		Blurb = "Make a portable linen bandage for domestic care from prepared linen stock.",
		Action = "making portable linen bandage for domestic care",
		ActiveCraftItemSdesc = "an in-progress portable linen bandage for domestic care craft",
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
			new CatalogueCommodityRecipeInput(Grams: 3727.0, Material: "linen", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3727 grams of linen", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_medical_mobility_0052_portable_bandage", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a portable linen bandage for domestic care (@industrialised_medical_mobility_0052_portable_bandage)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MedicalAndMobilityMakePortableMildSteelSplintForDomesticCareCraftIndustrialisedMedicalMobility0053PortableSplint() => new()
	{
		Metadata = new("Medical and mobility/make portable mild steel splint for domestic care [craft_industrialised_medical_mobility_0053_portable_splint]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_medical_mobility_0053_portable_splint"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make portable mild steel splint for domestic care [craft_industrialised_medical_mobility_0053_portable_splint]",
		Category = "Medical and mobility",
		Blurb = "Make a portable mild steel splint for domestic care from prepared mild steel stock.",
		Action = "making portable mild steel splint for domestic care",
		ActiveCraftItemSdesc = "an in-progress portable mild steel splint for domestic care craft",
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
			new CatalogueCommodityRecipeInput(Grams: 3768.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3768 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_medical_mobility_0053_portable_splint", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a portable mild steel splint for domestic care (@industrialised_medical_mobility_0053_portable_splint)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MedicalAndMobilityMakePortableGlassCrutchForDomesticCareCraftIndustrialisedMedicalMobility0054PortableCrutch() => new()
	{
		Metadata = new("Medical and mobility/make portable glass crutch for domestic care [craft_industrialised_medical_mobility_0054_portable_crutch]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_medical_mobility_0054_portable_crutch"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make portable glass crutch for domestic care [craft_industrialised_medical_mobility_0054_portable_crutch]",
		Category = "Medical and mobility",
		Blurb = "Make a portable glass crutch for domestic care from prepared glass stock.",
		Action = "making portable glass crutch for domestic care",
		ActiveCraftItemSdesc = "an in-progress portable glass crutch for domestic care craft",
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
			new CatalogueCommodityRecipeInput(Grams: 3808.0, Material: "glass", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3808 grams of glass", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_medical_mobility_0054_portable_crutch", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a portable glass crutch for domestic care (@industrialised_medical_mobility_0054_portable_crutch)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MedicalAndMobilityMakePortableOakStretcherForDomesticCareCraftIndustrialisedMedicalMobility0055PortableStretcher() => new()
	{
		Metadata = new("Medical and mobility/make portable oak stretcher for domestic care [craft_industrialised_medical_mobility_0055_portable_stretcher]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_medical_mobility_0055_portable_stretcher"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make portable oak stretcher for domestic care [craft_industrialised_medical_mobility_0055_portable_stretcher]",
		Category = "Medical and mobility",
		Blurb = "Make a portable oak stretcher for domestic care from prepared oak stock.",
		Action = "making portable oak stretcher for domestic care",
		ActiveCraftItemSdesc = "an in-progress portable oak stretcher for domestic care craft",
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
			new CatalogueCommodityRecipeInput(Grams: 3849.0, Material: "oak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3849 grams of oak", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_medical_mobility_0055_portable_stretcher", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a portable oak stretcher for domestic care (@industrialised_medical_mobility_0055_portable_stretcher)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MedicalAndMobilityMakePortableCottonSyringeForDomesticCareCraftIndustrialisedMedicalMobility0056PortableSyringe() => new()
	{
		Metadata = new("Medical and mobility/make portable cotton syringe for domestic care [craft_industrialised_medical_mobility_0056_portable_syringe]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_medical_mobility_0056_portable_syringe"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make portable cotton syringe for domestic care [craft_industrialised_medical_mobility_0056_portable_syringe]",
		Category = "Medical and mobility",
		Blurb = "Make a portable cotton syringe for domestic care from prepared cotton stock.",
		Action = "making portable cotton syringe for domestic care",
		ActiveCraftItemSdesc = "an in-progress portable cotton syringe for domestic care craft",
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
			new CatalogueCommodityRecipeInput(Grams: 3890.0, Material: "cotton", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3890 grams of cotton", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_medical_mobility_0056_portable_syringe", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a portable cotton syringe for domestic care (@industrialised_medical_mobility_0056_portable_syringe)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MedicalAndMobilityMakeSomePortableLinenForcepsForDomesticCareCraftIndustrialisedMedicalMobility0057PortableForceps() => new()
	{
		Metadata = new("Medical and mobility/make some portable linen forceps for domestic care [craft_industrialised_medical_mobility_0057_portable_forceps]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_medical_mobility_0057_portable_forceps"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make some portable linen forceps for domestic care [craft_industrialised_medical_mobility_0057_portable_forceps]",
		Category = "Medical and mobility",
		Blurb = "Make some portable linen forceps for domestic care from prepared linen stock.",
		Action = "making some portable linen forceps for domestic care",
		ActiveCraftItemSdesc = "an in-progress some portable linen forceps for domestic care craft",
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
			new CatalogueCommodityRecipeInput(Grams: 3930.0, Material: "linen", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3930 grams of linen", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_medical_mobility_0057_portable_forceps", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x some portable linen forceps for domestic care (@industrialised_medical_mobility_0057_portable_forceps)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MedicalAndMobilityMakePortableMildSteelThermometerForDomesticCareCraftIndustrialisedMedicalMobility0058PortableThermometer() => new()
	{
		Metadata = new("Medical and mobility/make portable mild steel thermometer for domestic care [craft_industrialised_medical_mobility_0058_portable_thermometer]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_medical_mobility_0058_portable_thermometer"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make portable mild steel thermometer for domestic care [craft_industrialised_medical_mobility_0058_portable_thermometer]",
		Category = "Medical and mobility",
		Blurb = "Make a portable mild steel thermometer for domestic care from prepared mild steel stock.",
		Action = "making portable mild steel thermometer for domestic care",
		ActiveCraftItemSdesc = "an in-progress portable mild steel thermometer for domestic care craft",
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
			new CatalogueCommodityRecipeInput(Grams: 3971.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3971 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_medical_mobility_0058_portable_thermometer", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a portable mild steel thermometer for domestic care (@industrialised_medical_mobility_0058_portable_thermometer)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MedicalAndMobilityMakePortableGlassCaseForDomesticCareCraftIndustrialisedMedicalMobility0059PortableCase() => new()
	{
		Metadata = new("Medical and mobility/make portable glass case for domestic care [craft_industrialised_medical_mobility_0059_portable_case]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_medical_mobility_0059_portable_case"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make portable glass case for domestic care [craft_industrialised_medical_mobility_0059_portable_case]",
		Category = "Medical and mobility",
		Blurb = "Make a portable glass case for domestic care from prepared glass stock.",
		Action = "making portable glass case for domestic care",
		ActiveCraftItemSdesc = "an in-progress portable glass case for domestic care craft",
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
			new CatalogueCommodityRecipeInput(Grams: 4012.0, Material: "glass", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("4012 grams of glass", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_medical_mobility_0059_portable_case", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a portable glass case for domestic care (@industrialised_medical_mobility_0059_portable_case)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MedicalAndMobilityMakePortableOakBraceForDomesticCareCraftIndustrialisedMedicalMobility0060PortableBrace() => new()
	{
		Metadata = new("Medical and mobility/make portable oak brace for domestic care [craft_industrialised_medical_mobility_0060_portable_brace]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_medical_mobility_0060_portable_brace"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make portable oak brace for domestic care [craft_industrialised_medical_mobility_0060_portable_brace]",
		Category = "Medical and mobility",
		Blurb = "Make a portable oak brace for domestic care from prepared oak stock.",
		Action = "making portable oak brace for domestic care",
		ActiveCraftItemSdesc = "an in-progress portable oak brace for domestic care craft",
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
			new CatalogueCommodityRecipeInput(Grams: 4052.0, Material: "oak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("4052 grams of oak", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_medical_mobility_0060_portable_brace", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a portable oak brace for domestic care (@industrialised_medical_mobility_0060_portable_brace)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MedicalAndMobilityMakeWorkshopCottonWasteForDomesticCareCraftIndustrialisedMedicalMobility0061WorkshopWaste() => new()
	{
		Metadata = new("Medical and mobility/make workshop cotton waste for domestic care [craft_industrialised_medical_mobility_0061_workshop_waste]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_medical_mobility_0061_workshop_waste"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make workshop cotton waste for domestic care [craft_industrialised_medical_mobility_0061_workshop_waste]",
		Category = "Medical and mobility",
		Blurb = "Make a workshop cotton waste for domestic care from prepared cotton stock.",
		Action = "making workshop cotton waste for domestic care",
		ActiveCraftItemSdesc = "an in-progress workshop cotton waste for domestic care craft",
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
			new CatalogueCommodityRecipeInput(Grams: 4093.0, Material: "cotton", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("4093 grams of cotton", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_medical_mobility_0061_workshop_waste", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a workshop cotton waste for domestic care (@industrialised_medical_mobility_0061_workshop_waste)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MedicalAndMobilityMakeWorkshopLinenBandageForDomesticCareCraftIndustrialisedMedicalMobility0062WorkshopBandage() => new()
	{
		Metadata = new("Medical and mobility/make workshop linen bandage for domestic care [craft_industrialised_medical_mobility_0062_workshop_bandage]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_medical_mobility_0062_workshop_bandage"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make workshop linen bandage for domestic care [craft_industrialised_medical_mobility_0062_workshop_bandage]",
		Category = "Medical and mobility",
		Blurb = "Make a workshop linen bandage for domestic care from prepared linen stock.",
		Action = "making workshop linen bandage for domestic care",
		ActiveCraftItemSdesc = "an in-progress workshop linen bandage for domestic care craft",
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
			new CatalogueCommodityRecipeInput(Grams: 4134.0, Material: "linen", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("4134 grams of linen", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_medical_mobility_0062_workshop_bandage", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a workshop linen bandage for domestic care (@industrialised_medical_mobility_0062_workshop_bandage)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MedicalAndMobilityMakeWorkshopMildSteelSplintForDomesticCareCraftIndustrialisedMedicalMobility0063WorkshopSplint() => new()
	{
		Metadata = new("Medical and mobility/make workshop mild steel splint for domestic care [craft_industrialised_medical_mobility_0063_workshop_splint]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_medical_mobility_0063_workshop_splint"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make workshop mild steel splint for domestic care [craft_industrialised_medical_mobility_0063_workshop_splint]",
		Category = "Medical and mobility",
		Blurb = "Make a workshop mild steel splint for domestic care from prepared mild steel stock.",
		Action = "making workshop mild steel splint for domestic care",
		ActiveCraftItemSdesc = "an in-progress workshop mild steel splint for domestic care craft",
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
			new CatalogueCommodityRecipeInput(Grams: 4174.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("4174 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_medical_mobility_0063_workshop_splint", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a workshop mild steel splint for domestic care (@industrialised_medical_mobility_0063_workshop_splint)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MedicalAndMobilityMakeWorkshopGlassCrutchForDomesticCareCraftIndustrialisedMedicalMobility0064WorkshopCrutch() => new()
	{
		Metadata = new("Medical and mobility/make workshop glass crutch for domestic care [craft_industrialised_medical_mobility_0064_workshop_crutch]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_medical_mobility_0064_workshop_crutch"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make workshop glass crutch for domestic care [craft_industrialised_medical_mobility_0064_workshop_crutch]",
		Category = "Medical and mobility",
		Blurb = "Make a workshop glass crutch for domestic care from prepared glass stock.",
		Action = "making workshop glass crutch for domestic care",
		ActiveCraftItemSdesc = "an in-progress workshop glass crutch for domestic care craft",
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
			new CatalogueCommodityRecipeInput(Grams: 4215.0, Material: "glass", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("4215 grams of glass", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_medical_mobility_0064_workshop_crutch", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a workshop glass crutch for domestic care (@industrialised_medical_mobility_0064_workshop_crutch)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MedicalAndMobilityMakeWorkshopOakStretcherForDomesticCareCraftIndustrialisedMedicalMobility0065WorkshopStretcher() => new()
	{
		Metadata = new("Medical and mobility/make workshop oak stretcher for domestic care [craft_industrialised_medical_mobility_0065_workshop_stretcher]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_medical_mobility_0065_workshop_stretcher"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make workshop oak stretcher for domestic care [craft_industrialised_medical_mobility_0065_workshop_stretcher]",
		Category = "Medical and mobility",
		Blurb = "Make a workshop oak stretcher for domestic care from prepared oak stock.",
		Action = "making workshop oak stretcher for domestic care",
		ActiveCraftItemSdesc = "an in-progress workshop oak stretcher for domestic care craft",
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
			new CatalogueCommodityRecipeInput(Grams: 4256.0, Material: "oak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("4256 grams of oak", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_medical_mobility_0065_workshop_stretcher", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a workshop oak stretcher for domestic care (@industrialised_medical_mobility_0065_workshop_stretcher)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MedicalAndMobilityMakeWorkshopCottonSyringeForDomesticCareCraftIndustrialisedMedicalMobility0066WorkshopSyringe() => new()
	{
		Metadata = new("Medical and mobility/make workshop cotton syringe for domestic care [craft_industrialised_medical_mobility_0066_workshop_syringe]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_medical_mobility_0066_workshop_syringe"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make workshop cotton syringe for domestic care [craft_industrialised_medical_mobility_0066_workshop_syringe]",
		Category = "Medical and mobility",
		Blurb = "Make a workshop cotton syringe for domestic care from prepared cotton stock.",
		Action = "making workshop cotton syringe for domestic care",
		ActiveCraftItemSdesc = "an in-progress workshop cotton syringe for domestic care craft",
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
			new CatalogueCommodityRecipeInput(Grams: 4297.0, Material: "cotton", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("4297 grams of cotton", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_medical_mobility_0066_workshop_syringe", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a workshop cotton syringe for domestic care (@industrialised_medical_mobility_0066_workshop_syringe)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};
}
