#nullable enable

using MudSharp.Body;
using MudSharp.GameItems.Inventory.Plans;
using MudSharp.RPG.Checks;

namespace DatabaseSeeder.Seeders;

public static partial class Recipes22Stock
{
	public static CatalogueRecipeDefinition MedicalAndMobilityMakeSomeWorkshopLinenForcepsForDomesticCareCraftIndustrialisedMedicalMobility0067WorkshopForceps() => new()
	{
		Metadata = new("Medical and mobility/make some workshop linen forceps for domestic care [craft_industrialised_medical_mobility_0067_workshop_forceps]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_medical_mobility_0067_workshop_forceps"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make some workshop linen forceps for domestic care [craft_industrialised_medical_mobility_0067_workshop_forceps]",
		Category = "Medical and mobility",
		Blurb = "Make some workshop linen forceps for domestic care from prepared linen stock.",
		Action = "making some workshop linen forceps for domestic care",
		ActiveCraftItemSdesc = "an in-progress some workshop linen forceps for domestic care craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 4337.0, Material: "linen", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("4337 grams of linen", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_medical_mobility_0067_workshop_forceps", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x some workshop linen forceps for domestic care (@industrialised_medical_mobility_0067_workshop_forceps)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MedicalAndMobilityMakeWorkshopMildSteelThermometerForDomesticCareCraftIndustrialisedMedicalMobility0068WorkshopThermometer() => new()
	{
		Metadata = new("Medical and mobility/make workshop mild steel thermometer for domestic care [craft_industrialised_medical_mobility_0068_workshop_thermometer]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_medical_mobility_0068_workshop_thermometer"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make workshop mild steel thermometer for domestic care [craft_industrialised_medical_mobility_0068_workshop_thermometer]",
		Category = "Medical and mobility",
		Blurb = "Make a workshop mild steel thermometer for domestic care from prepared mild steel stock.",
		Action = "making workshop mild steel thermometer for domestic care",
		ActiveCraftItemSdesc = "an in-progress workshop mild steel thermometer for domestic care craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 4378.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("4378 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_medical_mobility_0068_workshop_thermometer", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a workshop mild steel thermometer for domestic care (@industrialised_medical_mobility_0068_workshop_thermometer)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MedicalAndMobilityMakeWorkshopGlassCaseForDomesticCareCraftIndustrialisedMedicalMobility0069WorkshopCase() => new()
	{
		Metadata = new("Medical and mobility/make workshop glass case for domestic care [craft_industrialised_medical_mobility_0069_workshop_case]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_medical_mobility_0069_workshop_case"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make workshop glass case for domestic care [craft_industrialised_medical_mobility_0069_workshop_case]",
		Category = "Medical and mobility",
		Blurb = "Make a workshop glass case for domestic care from prepared glass stock.",
		Action = "making workshop glass case for domestic care",
		ActiveCraftItemSdesc = "an in-progress workshop glass case for domestic care craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 4419.0, Material: "glass", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("4419 grams of glass", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_medical_mobility_0069_workshop_case", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a workshop glass case for domestic care (@industrialised_medical_mobility_0069_workshop_case)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MedicalAndMobilityMakeWorkshopOakBraceForDomesticCareCraftIndustrialisedMedicalMobility0070WorkshopBrace() => new()
	{
		Metadata = new("Medical and mobility/make workshop oak brace for domestic care [craft_industrialised_medical_mobility_0070_workshop_brace]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_medical_mobility_0070_workshop_brace"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make workshop oak brace for domestic care [craft_industrialised_medical_mobility_0070_workshop_brace]",
		Category = "Medical and mobility",
		Blurb = "Make a workshop oak brace for domestic care from prepared oak stock.",
		Action = "making workshop oak brace for domestic care",
		ActiveCraftItemSdesc = "an in-progress workshop oak brace for domestic care craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 4459.0, Material: "oak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("4459 grams of oak", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_medical_mobility_0070_workshop_brace", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a workshop oak brace for domestic care (@industrialised_medical_mobility_0070_workshop_brace)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MedicalAndMobilityMakeInstitutionalCottonWasteForDomesticCareCraftIndustrialisedMedicalMobility0071InstitutionalWaste() => new()
	{
		Metadata = new("Medical and mobility/make institutional cotton waste for domestic care [craft_industrialised_medical_mobility_0071_institutional_waste]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_medical_mobility_0071_institutional_waste"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make institutional cotton waste for domestic care [craft_industrialised_medical_mobility_0071_institutional_waste]",
		Category = "Medical and mobility",
		Blurb = "Make an institutional cotton waste for domestic care from prepared cotton stock.",
		Action = "making institutional cotton waste for domestic care",
		ActiveCraftItemSdesc = "an in-progress institutional cotton waste for domestic care craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 4500.0, Material: "cotton", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("4500 grams of cotton", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_medical_mobility_0071_institutional_waste", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x an institutional cotton waste for domestic care (@industrialised_medical_mobility_0071_institutional_waste)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MedicalAndMobilityMakeInstitutionalLinenBandageForDomesticCareCraftIndustrialisedMedicalMobility0072InstitutionalBandage() => new()
	{
		Metadata = new("Medical and mobility/make institutional linen bandage for domestic care [craft_industrialised_medical_mobility_0072_institutional_bandage]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_medical_mobility_0072_institutional_bandage"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make institutional linen bandage for domestic care [craft_industrialised_medical_mobility_0072_institutional_bandage]",
		Category = "Medical and mobility",
		Blurb = "Make an institutional linen bandage for domestic care from prepared linen stock.",
		Action = "making institutional linen bandage for domestic care",
		ActiveCraftItemSdesc = "an in-progress institutional linen bandage for domestic care craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 4541.0, Material: "linen", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("4541 grams of linen", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_medical_mobility_0072_institutional_bandage", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x an institutional linen bandage for domestic care (@industrialised_medical_mobility_0072_institutional_bandage)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MedicalAndMobilityMakeInstitutionalMildSteelSplintForDomesticCareCraftIndustrialisedMedicalMobility0073InstitutionalSplint() => new()
	{
		Metadata = new("Medical and mobility/make institutional mild steel splint for domestic care [craft_industrialised_medical_mobility_0073_institutional_splint]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_medical_mobility_0073_institutional_splint"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make institutional mild steel splint for domestic care [craft_industrialised_medical_mobility_0073_institutional_splint]",
		Category = "Medical and mobility",
		Blurb = "Make an institutional mild steel splint for domestic care from prepared mild steel stock.",
		Action = "making institutional mild steel splint for domestic care",
		ActiveCraftItemSdesc = "an in-progress institutional mild steel splint for domestic care craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 4582.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("4582 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_medical_mobility_0073_institutional_splint", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x an institutional mild steel splint for domestic care (@industrialised_medical_mobility_0073_institutional_splint)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MedicalAndMobilityMakeInstitutionalGlassCrutchForDomesticCareCraftIndustrialisedMedicalMobility0074InstitutionalCrutch() => new()
	{
		Metadata = new("Medical and mobility/make institutional glass crutch for domestic care [craft_industrialised_medical_mobility_0074_institutional_crutch]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_medical_mobility_0074_institutional_crutch"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make institutional glass crutch for domestic care [craft_industrialised_medical_mobility_0074_institutional_crutch]",
		Category = "Medical and mobility",
		Blurb = "Make an institutional glass crutch for domestic care from prepared glass stock.",
		Action = "making institutional glass crutch for domestic care",
		ActiveCraftItemSdesc = "an in-progress institutional glass crutch for domestic care craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 4622.0, Material: "glass", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("4622 grams of glass", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_medical_mobility_0074_institutional_crutch", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x an institutional glass crutch for domestic care (@industrialised_medical_mobility_0074_institutional_crutch)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MedicalAndMobilityMakeInstitutionalOakStretcherForDomesticCareCraftIndustrialisedMedicalMobility0075InstitutionalStretcher() => new()
	{
		Metadata = new("Medical and mobility/make institutional oak stretcher for domestic care [craft_industrialised_medical_mobility_0075_institutional_stretcher]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_medical_mobility_0075_institutional_stretcher"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make institutional oak stretcher for domestic care [craft_industrialised_medical_mobility_0075_institutional_stretcher]",
		Category = "Medical and mobility",
		Blurb = "Make an institutional oak stretcher for domestic care from prepared oak stock.",
		Action = "making institutional oak stretcher for domestic care",
		ActiveCraftItemSdesc = "an in-progress institutional oak stretcher for domestic care craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 4663.0, Material: "oak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("4663 grams of oak", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_medical_mobility_0075_institutional_stretcher", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x an institutional oak stretcher for domestic care (@industrialised_medical_mobility_0075_institutional_stretcher)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MedicalAndMobilityMakeInstitutionalCottonSyringeForDomesticCareCraftIndustrialisedMedicalMobility0076InstitutionalSyringe() => new()
	{
		Metadata = new("Medical and mobility/make institutional cotton syringe for domestic care [craft_industrialised_medical_mobility_0076_institutional_syringe]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_medical_mobility_0076_institutional_syringe"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make institutional cotton syringe for domestic care [craft_industrialised_medical_mobility_0076_institutional_syringe]",
		Category = "Medical and mobility",
		Blurb = "Make an institutional cotton syringe for domestic care from prepared cotton stock.",
		Action = "making institutional cotton syringe for domestic care",
		ActiveCraftItemSdesc = "an in-progress institutional cotton syringe for domestic care craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 4704.0, Material: "cotton", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("4704 grams of cotton", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_medical_mobility_0076_institutional_syringe", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x an institutional cotton syringe for domestic care (@industrialised_medical_mobility_0076_institutional_syringe)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MedicalAndMobilityMakeSomeInstitutionalLinenForcepsForDomesticCareCraftIndustrialisedMedicalMobility0077InstitutionalForceps() => new()
	{
		Metadata = new("Medical and mobility/make some institutional linen forceps for domestic care [craft_industrialised_medical_mobility_0077_institutional_forceps]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_medical_mobility_0077_institutional_forceps"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make some institutional linen forceps for domestic care [craft_industrialised_medical_mobility_0077_institutional_forceps]",
		Category = "Medical and mobility",
		Blurb = "Make some institutional linen forceps for domestic care from prepared linen stock.",
		Action = "making some institutional linen forceps for domestic care",
		ActiveCraftItemSdesc = "an in-progress some institutional linen forceps for domestic care craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 4744.0, Material: "linen", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("4744 grams of linen", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_medical_mobility_0077_institutional_forceps", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x some institutional linen forceps for domestic care (@industrialised_medical_mobility_0077_institutional_forceps)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MedicalAndMobilityMakeInstitutionalMildSteelThermometerForDomesticCareCraftIndustrialisedMedicalMobility0078InstitutionalThermometer() => new()
	{
		Metadata = new("Medical and mobility/make institutional mild steel thermometer for domestic care [craft_industrialised_medical_mobility_0078_institutional_thermometer]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_medical_mobility_0078_institutional_thermometer"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make institutional mild steel thermometer for domestic care [craft_industrialised_medical_mobility_0078_institutional_thermometer]",
		Category = "Medical and mobility",
		Blurb = "Make an institutional mild steel thermometer for domestic care from prepared mild steel stock.",
		Action = "making institutional mild steel thermometer for domestic care",
		ActiveCraftItemSdesc = "an in-progress institutional mild steel thermometer for domestic care craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 4785.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("4785 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_medical_mobility_0078_institutional_thermometer", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x an institutional mild steel thermometer for domestic care (@industrialised_medical_mobility_0078_institutional_thermometer)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MedicalAndMobilityMakeInstitutionalGlassCaseForDomesticCareCraftIndustrialisedMedicalMobility0079InstitutionalCase() => new()
	{
		Metadata = new("Medical and mobility/make institutional glass case for domestic care [craft_industrialised_medical_mobility_0079_institutional_case]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_medical_mobility_0079_institutional_case"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make institutional glass case for domestic care [craft_industrialised_medical_mobility_0079_institutional_case]",
		Category = "Medical and mobility",
		Blurb = "Make an institutional glass case for domestic care from prepared glass stock.",
		Action = "making institutional glass case for domestic care",
		ActiveCraftItemSdesc = "an in-progress institutional glass case for domestic care craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 4826.0, Material: "glass", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("4826 grams of glass", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_medical_mobility_0079_institutional_case", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x an institutional glass case for domestic care (@industrialised_medical_mobility_0079_institutional_case)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MedicalAndMobilityMakeInstitutionalOakBraceForDomesticCareCraftIndustrialisedMedicalMobility0080InstitutionalBrace() => new()
	{
		Metadata = new("Medical and mobility/make institutional oak brace for domestic care [craft_industrialised_medical_mobility_0080_institutional_brace]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_medical_mobility_0080_institutional_brace"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make institutional oak brace for domestic care [craft_industrialised_medical_mobility_0080_institutional_brace]",
		Category = "Medical and mobility",
		Blurb = "Make an institutional oak brace for domestic care from prepared oak stock.",
		Action = "making institutional oak brace for domestic care",
		ActiveCraftItemSdesc = "an in-progress institutional oak brace for domestic care craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 4866.0, Material: "oak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("4866 grams of oak", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_medical_mobility_0080_institutional_brace", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x an institutional oak brace for domestic care (@industrialised_medical_mobility_0080_institutional_brace)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MedicalAndMobilityMakeWeatherproofCottonWasteForDomesticCareCraftIndustrialisedMedicalMobility0081WeatherproofWaste() => new()
	{
		Metadata = new("Medical and mobility/make weatherproof cotton waste for domestic care [craft_industrialised_medical_mobility_0081_weatherproof_waste]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_medical_mobility_0081_weatherproof_waste"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make weatherproof cotton waste for domestic care [craft_industrialised_medical_mobility_0081_weatherproof_waste]",
		Category = "Medical and mobility",
		Blurb = "Make a weatherproof cotton waste for domestic care from prepared cotton stock.",
		Action = "making weatherproof cotton waste for domestic care",
		ActiveCraftItemSdesc = "an in-progress weatherproof cotton waste for domestic care craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 4907.0, Material: "cotton", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("4907 grams of cotton", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_medical_mobility_0081_weatherproof_waste", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a weatherproof cotton waste for domestic care (@industrialised_medical_mobility_0081_weatherproof_waste)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MedicalAndMobilityMakeWeatherproofLinenBandageForDomesticCareCraftIndustrialisedMedicalMobility0082WeatherproofBandage() => new()
	{
		Metadata = new("Medical and mobility/make weatherproof linen bandage for domestic care [craft_industrialised_medical_mobility_0082_weatherproof_bandage]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_medical_mobility_0082_weatherproof_bandage"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make weatherproof linen bandage for domestic care [craft_industrialised_medical_mobility_0082_weatherproof_bandage]",
		Category = "Medical and mobility",
		Blurb = "Make a weatherproof linen bandage for domestic care from prepared linen stock.",
		Action = "making weatherproof linen bandage for domestic care",
		ActiveCraftItemSdesc = "an in-progress weatherproof linen bandage for domestic care craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 4948.0, Material: "linen", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("4948 grams of linen", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_medical_mobility_0082_weatherproof_bandage", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a weatherproof linen bandage for domestic care (@industrialised_medical_mobility_0082_weatherproof_bandage)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MedicalAndMobilityMakeWeatherproofMildSteelSplintForDomesticCareCraftIndustrialisedMedicalMobility0083WeatherproofSplint() => new()
	{
		Metadata = new("Medical and mobility/make weatherproof mild steel splint for domestic care [craft_industrialised_medical_mobility_0083_weatherproof_splint]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_medical_mobility_0083_weatherproof_splint"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make weatherproof mild steel splint for domestic care [craft_industrialised_medical_mobility_0083_weatherproof_splint]",
		Category = "Medical and mobility",
		Blurb = "Make a weatherproof mild steel splint for domestic care from prepared mild steel stock.",
		Action = "making weatherproof mild steel splint for domestic care",
		ActiveCraftItemSdesc = "an in-progress weatherproof mild steel splint for domestic care craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 4988.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("4988 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_medical_mobility_0083_weatherproof_splint", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a weatherproof mild steel splint for domestic care (@industrialised_medical_mobility_0083_weatherproof_splint)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MedicalAndMobilityMakeWeatherproofGlassCrutchForDomesticCareCraftIndustrialisedMedicalMobility0084WeatherproofCrutch() => new()
	{
		Metadata = new("Medical and mobility/make weatherproof glass crutch for domestic care [craft_industrialised_medical_mobility_0084_weatherproof_crutch]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_medical_mobility_0084_weatherproof_crutch"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make weatherproof glass crutch for domestic care [craft_industrialised_medical_mobility_0084_weatherproof_crutch]",
		Category = "Medical and mobility",
		Blurb = "Make a weatherproof glass crutch for domestic care from prepared glass stock.",
		Action = "making weatherproof glass crutch for domestic care",
		ActiveCraftItemSdesc = "an in-progress weatherproof glass crutch for domestic care craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 5029.0, Material: "glass", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("5029 grams of glass", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_medical_mobility_0084_weatherproof_crutch", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a weatherproof glass crutch for domestic care (@industrialised_medical_mobility_0084_weatherproof_crutch)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MedicalAndMobilityMakeWeatherproofOakStretcherForDomesticCareCraftIndustrialisedMedicalMobility0085WeatherproofStretcher() => new()
	{
		Metadata = new("Medical and mobility/make weatherproof oak stretcher for domestic care [craft_industrialised_medical_mobility_0085_weatherproof_stretcher]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_medical_mobility_0085_weatherproof_stretcher"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make weatherproof oak stretcher for domestic care [craft_industrialised_medical_mobility_0085_weatherproof_stretcher]",
		Category = "Medical and mobility",
		Blurb = "Make a weatherproof oak stretcher for domestic care from prepared oak stock.",
		Action = "making weatherproof oak stretcher for domestic care",
		ActiveCraftItemSdesc = "an in-progress weatherproof oak stretcher for domestic care craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 5070.0, Material: "oak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("5070 grams of oak", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_medical_mobility_0085_weatherproof_stretcher", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a weatherproof oak stretcher for domestic care (@industrialised_medical_mobility_0085_weatherproof_stretcher)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MedicalAndMobilityMakeWeatherproofCottonSyringeForDomesticCareCraftIndustrialisedMedicalMobility0086WeatherproofSyringe() => new()
	{
		Metadata = new("Medical and mobility/make weatherproof cotton syringe for domestic care [craft_industrialised_medical_mobility_0086_weatherproof_syringe]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_medical_mobility_0086_weatherproof_syringe"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make weatherproof cotton syringe for domestic care [craft_industrialised_medical_mobility_0086_weatherproof_syringe]",
		Category = "Medical and mobility",
		Blurb = "Make a weatherproof cotton syringe for domestic care from prepared cotton stock.",
		Action = "making weatherproof cotton syringe for domestic care",
		ActiveCraftItemSdesc = "an in-progress weatherproof cotton syringe for domestic care craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 5111.0, Material: "cotton", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("5111 grams of cotton", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_medical_mobility_0086_weatherproof_syringe", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a weatherproof cotton syringe for domestic care (@industrialised_medical_mobility_0086_weatherproof_syringe)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MedicalAndMobilityMakeSomeWeatherproofLinenForcepsForDomesticCareCraftIndustrialisedMedicalMobility0087WeatherproofForceps() => new()
	{
		Metadata = new("Medical and mobility/make some weatherproof linen forceps for domestic care [craft_industrialised_medical_mobility_0087_weatherproof_forceps]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_medical_mobility_0087_weatherproof_forceps"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make some weatherproof linen forceps for domestic care [craft_industrialised_medical_mobility_0087_weatherproof_forceps]",
		Category = "Medical and mobility",
		Blurb = "Make some weatherproof linen forceps for domestic care from prepared linen stock.",
		Action = "making some weatherproof linen forceps for domestic care",
		ActiveCraftItemSdesc = "an in-progress some weatherproof linen forceps for domestic care craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 5151.0, Material: "linen", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("5151 grams of linen", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_medical_mobility_0087_weatherproof_forceps", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x some weatherproof linen forceps for domestic care (@industrialised_medical_mobility_0087_weatherproof_forceps)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MedicalAndMobilityMakeWeatherproofMildSteelThermometerForDomesticCareCraftIndustrialisedMedicalMobility0088WeatherproofThermometer() => new()
	{
		Metadata = new("Medical and mobility/make weatherproof mild steel thermometer for domestic care [craft_industrialised_medical_mobility_0088_weatherproof_thermometer]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_medical_mobility_0088_weatherproof_thermometer"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make weatherproof mild steel thermometer for domestic care [craft_industrialised_medical_mobility_0088_weatherproof_thermometer]",
		Category = "Medical and mobility",
		Blurb = "Make a weatherproof mild steel thermometer for domestic care from prepared mild steel stock.",
		Action = "making weatherproof mild steel thermometer for domestic care",
		ActiveCraftItemSdesc = "an in-progress weatherproof mild steel thermometer for domestic care craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 5192.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("5192 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_medical_mobility_0088_weatherproof_thermometer", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a weatherproof mild steel thermometer for domestic care (@industrialised_medical_mobility_0088_weatherproof_thermometer)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MedicalAndMobilityMakeWeatherproofGlassCaseForDomesticCareCraftIndustrialisedMedicalMobility0089WeatherproofCase() => new()
	{
		Metadata = new("Medical and mobility/make weatherproof glass case for domestic care [craft_industrialised_medical_mobility_0089_weatherproof_case]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_medical_mobility_0089_weatherproof_case"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make weatherproof glass case for domestic care [craft_industrialised_medical_mobility_0089_weatherproof_case]",
		Category = "Medical and mobility",
		Blurb = "Make a weatherproof glass case for domestic care from prepared glass stock.",
		Action = "making weatherproof glass case for domestic care",
		ActiveCraftItemSdesc = "an in-progress weatherproof glass case for domestic care craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 5233.0, Material: "glass", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("5233 grams of glass", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_medical_mobility_0089_weatherproof_case", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a weatherproof glass case for domestic care (@industrialised_medical_mobility_0089_weatherproof_case)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MedicalAndMobilityMakeWeatherproofOakBraceForDomesticCareCraftIndustrialisedMedicalMobility0090WeatherproofBrace() => new()
	{
		Metadata = new("Medical and mobility/make weatherproof oak brace for domestic care [craft_industrialised_medical_mobility_0090_weatherproof_brace]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_medical_mobility_0090_weatherproof_brace"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make weatherproof oak brace for domestic care [craft_industrialised_medical_mobility_0090_weatherproof_brace]",
		Category = "Medical and mobility",
		Blurb = "Make a weatherproof oak brace for domestic care from prepared oak stock.",
		Action = "making weatherproof oak brace for domestic care",
		ActiveCraftItemSdesc = "an in-progress weatherproof oak brace for domestic care craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 5273.0, Material: "oak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("5273 grams of oak", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_medical_mobility_0090_weatherproof_brace", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a weatherproof oak brace for domestic care (@industrialised_medical_mobility_0090_weatherproof_brace)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MedicalAndMobilityMakeServiceCottonWasteForDomesticCareCraftIndustrialisedMedicalMobility0091ServiceWaste() => new()
	{
		Metadata = new("Medical and mobility/make service cotton waste for domestic care [craft_industrialised_medical_mobility_0091_service_waste]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_medical_mobility_0091_service_waste"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make service cotton waste for domestic care [craft_industrialised_medical_mobility_0091_service_waste]",
		Category = "Medical and mobility",
		Blurb = "Make a service cotton waste for domestic care from prepared cotton stock.",
		Action = "making service cotton waste for domestic care",
		ActiveCraftItemSdesc = "an in-progress service cotton waste for domestic care craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 5314.0, Material: "cotton", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("5314 grams of cotton", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_medical_mobility_0091_service_waste", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a service cotton waste for domestic care (@industrialised_medical_mobility_0091_service_waste)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MedicalAndMobilityMakeServiceLinenBandageForDomesticCareCraftIndustrialisedMedicalMobility0092ServiceBandage() => new()
	{
		Metadata = new("Medical and mobility/make service linen bandage for domestic care [craft_industrialised_medical_mobility_0092_service_bandage]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_medical_mobility_0092_service_bandage"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make service linen bandage for domestic care [craft_industrialised_medical_mobility_0092_service_bandage]",
		Category = "Medical and mobility",
		Blurb = "Make a service linen bandage for domestic care from prepared linen stock.",
		Action = "making service linen bandage for domestic care",
		ActiveCraftItemSdesc = "an in-progress service linen bandage for domestic care craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 5355.0, Material: "linen", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("5355 grams of linen", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_medical_mobility_0092_service_bandage", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a service linen bandage for domestic care (@industrialised_medical_mobility_0092_service_bandage)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MedicalAndMobilityMakeServiceMildSteelSplintForDomesticCareCraftIndustrialisedMedicalMobility0093ServiceSplint() => new()
	{
		Metadata = new("Medical and mobility/make service mild steel splint for domestic care [craft_industrialised_medical_mobility_0093_service_splint]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_medical_mobility_0093_service_splint"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make service mild steel splint for domestic care [craft_industrialised_medical_mobility_0093_service_splint]",
		Category = "Medical and mobility",
		Blurb = "Make a service mild steel splint for domestic care from prepared mild steel stock.",
		Action = "making service mild steel splint for domestic care",
		ActiveCraftItemSdesc = "an in-progress service mild steel splint for domestic care craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 5396.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("5396 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_medical_mobility_0093_service_splint", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a service mild steel splint for domestic care (@industrialised_medical_mobility_0093_service_splint)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MedicalAndMobilityMakeServiceGlassCrutchForDomesticCareCraftIndustrialisedMedicalMobility0094ServiceCrutch() => new()
	{
		Metadata = new("Medical and mobility/make service glass crutch for domestic care [craft_industrialised_medical_mobility_0094_service_crutch]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_medical_mobility_0094_service_crutch"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make service glass crutch for domestic care [craft_industrialised_medical_mobility_0094_service_crutch]",
		Category = "Medical and mobility",
		Blurb = "Make a service glass crutch for domestic care from prepared glass stock.",
		Action = "making service glass crutch for domestic care",
		ActiveCraftItemSdesc = "an in-progress service glass crutch for domestic care craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 5436.0, Material: "glass", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("5436 grams of glass", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_medical_mobility_0094_service_crutch", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a service glass crutch for domestic care (@industrialised_medical_mobility_0094_service_crutch)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MedicalAndMobilityMakeServiceOakStretcherForDomesticCareCraftIndustrialisedMedicalMobility0095ServiceStretcher() => new()
	{
		Metadata = new("Medical and mobility/make service oak stretcher for domestic care [craft_industrialised_medical_mobility_0095_service_stretcher]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_medical_mobility_0095_service_stretcher"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make service oak stretcher for domestic care [craft_industrialised_medical_mobility_0095_service_stretcher]",
		Category = "Medical and mobility",
		Blurb = "Make a service oak stretcher for domestic care from prepared oak stock.",
		Action = "making service oak stretcher for domestic care",
		ActiveCraftItemSdesc = "an in-progress service oak stretcher for domestic care craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 5477.0, Material: "oak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("5477 grams of oak", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_medical_mobility_0095_service_stretcher", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a service oak stretcher for domestic care (@industrialised_medical_mobility_0095_service_stretcher)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MedicalAndMobilityMakeServiceCottonSyringeForDomesticCareCraftIndustrialisedMedicalMobility0096ServiceSyringe() => new()
	{
		Metadata = new("Medical and mobility/make service cotton syringe for domestic care [craft_industrialised_medical_mobility_0096_service_syringe]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_medical_mobility_0096_service_syringe"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make service cotton syringe for domestic care [craft_industrialised_medical_mobility_0096_service_syringe]",
		Category = "Medical and mobility",
		Blurb = "Make a service cotton syringe for domestic care from prepared cotton stock.",
		Action = "making service cotton syringe for domestic care",
		ActiveCraftItemSdesc = "an in-progress service cotton syringe for domestic care craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 73.0, Material: "cotton", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("73 grams of cotton", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_medical_mobility_0096_service_syringe", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a service cotton syringe for domestic care (@industrialised_medical_mobility_0096_service_syringe)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MedicalAndMobilityMakeSomeServiceLinenForcepsForDomesticCareCraftIndustrialisedMedicalMobility0097ServiceForceps() => new()
	{
		Metadata = new("Medical and mobility/make some service linen forceps for domestic care [craft_industrialised_medical_mobility_0097_service_forceps]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_medical_mobility_0097_service_forceps"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make some service linen forceps for domestic care [craft_industrialised_medical_mobility_0097_service_forceps]",
		Category = "Medical and mobility",
		Blurb = "Make some service linen forceps for domestic care from prepared linen stock.",
		Action = "making some service linen forceps for domestic care",
		ActiveCraftItemSdesc = "an in-progress some service linen forceps for domestic care craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 113.0, Material: "linen", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("113 grams of linen", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_medical_mobility_0097_service_forceps", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x some service linen forceps for domestic care (@industrialised_medical_mobility_0097_service_forceps)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MedicalAndMobilityMakeServiceMildSteelThermometerForDomesticCareCraftIndustrialisedMedicalMobility0098ServiceThermometer() => new()
	{
		Metadata = new("Medical and mobility/make service mild steel thermometer for domestic care [craft_industrialised_medical_mobility_0098_service_thermometer]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_medical_mobility_0098_service_thermometer"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make service mild steel thermometer for domestic care [craft_industrialised_medical_mobility_0098_service_thermometer]",
		Category = "Medical and mobility",
		Blurb = "Make a service mild steel thermometer for domestic care from prepared mild steel stock.",
		Action = "making service mild steel thermometer for domestic care",
		ActiveCraftItemSdesc = "an in-progress service mild steel thermometer for domestic care craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 154.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("154 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_medical_mobility_0098_service_thermometer", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a service mild steel thermometer for domestic care (@industrialised_medical_mobility_0098_service_thermometer)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MedicalAndMobilityMakeServiceGlassCaseForDomesticCareCraftIndustrialisedMedicalMobility0099ServiceCase() => new()
	{
		Metadata = new("Medical and mobility/make service glass case for domestic care [craft_industrialised_medical_mobility_0099_service_case]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_medical_mobility_0099_service_case"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make service glass case for domestic care [craft_industrialised_medical_mobility_0099_service_case]",
		Category = "Medical and mobility",
		Blurb = "Make a service glass case for domestic care from prepared glass stock.",
		Action = "making service glass case for domestic care",
		ActiveCraftItemSdesc = "an in-progress service glass case for domestic care craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 195.0, Material: "glass", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("195 grams of glass", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_medical_mobility_0099_service_case", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a service glass case for domestic care (@industrialised_medical_mobility_0099_service_case)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MedicalAndMobilityMakeServiceOakBraceForDomesticCareCraftIndustrialisedMedicalMobility0100ServiceBrace() => new()
	{
		Metadata = new("Medical and mobility/make service oak brace for domestic care [craft_industrialised_medical_mobility_0100_service_brace]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_medical_mobility_0100_service_brace"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make service oak brace for domestic care [craft_industrialised_medical_mobility_0100_service_brace]",
		Category = "Medical and mobility",
		Blurb = "Make a service oak brace for domestic care from prepared oak stock.",
		Action = "making service oak brace for domestic care",
		ActiveCraftItemSdesc = "an in-progress service oak brace for domestic care craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 235.0, Material: "oak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("235 grams of oak", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_medical_mobility_0100_service_brace", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a service oak brace for domestic care (@industrialised_medical_mobility_0100_service_brace)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MedicalAndMobilityMakePrecisionCottonWasteForDomesticCareCraftIndustrialisedMedicalMobility0101PrecisionWaste() => new()
	{
		Metadata = new("Medical and mobility/make precision cotton waste for domestic care [craft_industrialised_medical_mobility_0101_precision_waste]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_medical_mobility_0101_precision_waste"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make precision cotton waste for domestic care [craft_industrialised_medical_mobility_0101_precision_waste]",
		Category = "Medical and mobility",
		Blurb = "Make a precision cotton waste for domestic care from prepared cotton stock.",
		Action = "making precision cotton waste for domestic care",
		ActiveCraftItemSdesc = "an in-progress precision cotton waste for domestic care craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 276.0, Material: "cotton", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("276 grams of cotton", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_medical_mobility_0101_precision_waste", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a precision cotton waste for domestic care (@industrialised_medical_mobility_0101_precision_waste)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MedicalAndMobilityMakePrecisionLinenBandageForDomesticCareCraftIndustrialisedMedicalMobility0102PrecisionBandage() => new()
	{
		Metadata = new("Medical and mobility/make precision linen bandage for domestic care [craft_industrialised_medical_mobility_0102_precision_bandage]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_medical_mobility_0102_precision_bandage"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make precision linen bandage for domestic care [craft_industrialised_medical_mobility_0102_precision_bandage]",
		Category = "Medical and mobility",
		Blurb = "Make a precision linen bandage for domestic care from prepared linen stock.",
		Action = "making precision linen bandage for domestic care",
		ActiveCraftItemSdesc = "an in-progress precision linen bandage for domestic care craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 317.0, Material: "linen", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("317 grams of linen", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_medical_mobility_0102_precision_bandage", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a precision linen bandage for domestic care (@industrialised_medical_mobility_0102_precision_bandage)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MedicalAndMobilityMakePrecisionMildSteelSplintForDomesticCareCraftIndustrialisedMedicalMobility0103PrecisionSplint() => new()
	{
		Metadata = new("Medical and mobility/make precision mild steel splint for domestic care [craft_industrialised_medical_mobility_0103_precision_splint]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_medical_mobility_0103_precision_splint"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make precision mild steel splint for domestic care [craft_industrialised_medical_mobility_0103_precision_splint]",
		Category = "Medical and mobility",
		Blurb = "Make a precision mild steel splint for domestic care from prepared mild steel stock.",
		Action = "making precision mild steel splint for domestic care",
		ActiveCraftItemSdesc = "an in-progress precision mild steel splint for domestic care craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 358.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("358 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_medical_mobility_0103_precision_splint", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a precision mild steel splint for domestic care (@industrialised_medical_mobility_0103_precision_splint)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MedicalAndMobilityMakePrecisionGlassCrutchForDomesticCareCraftIndustrialisedMedicalMobility0104PrecisionCrutch() => new()
	{
		Metadata = new("Medical and mobility/make precision glass crutch for domestic care [craft_industrialised_medical_mobility_0104_precision_crutch]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_medical_mobility_0104_precision_crutch"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make precision glass crutch for domestic care [craft_industrialised_medical_mobility_0104_precision_crutch]",
		Category = "Medical and mobility",
		Blurb = "Make a precision glass crutch for domestic care from prepared glass stock.",
		Action = "making precision glass crutch for domestic care",
		ActiveCraftItemSdesc = "an in-progress precision glass crutch for domestic care craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 398.0, Material: "glass", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("398 grams of glass", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_medical_mobility_0104_precision_crutch", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a precision glass crutch for domestic care (@industrialised_medical_mobility_0104_precision_crutch)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MedicalAndMobilityMakePrecisionOakStretcherForDomesticCareCraftIndustrialisedMedicalMobility0105PrecisionStretcher() => new()
	{
		Metadata = new("Medical and mobility/make precision oak stretcher for domestic care [craft_industrialised_medical_mobility_0105_precision_stretcher]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_medical_mobility_0105_precision_stretcher"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make precision oak stretcher for domestic care [craft_industrialised_medical_mobility_0105_precision_stretcher]",
		Category = "Medical and mobility",
		Blurb = "Make a precision oak stretcher for domestic care from prepared oak stock.",
		Action = "making precision oak stretcher for domestic care",
		ActiveCraftItemSdesc = "an in-progress precision oak stretcher for domestic care craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 439.0, Material: "oak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("439 grams of oak", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_medical_mobility_0105_precision_stretcher", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a precision oak stretcher for domestic care (@industrialised_medical_mobility_0105_precision_stretcher)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MedicalAndMobilityMakeServiceCottonWasteForWorkshopCareCraftIndustrialMedicalMobility0001ServiceWaste() => new()
	{
		Metadata = new("Medical and mobility/make service cotton waste for workshop care [craft_industrial_medical_mobility_0001_service_waste]", "crafts", ["industrial"], ["item:industrial_medical_mobility_0001_service_waste"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make service cotton waste for workshop care [craft_industrial_medical_mobility_0001_service_waste]",
		Category = "Medical and mobility",
		Blurb = "Make a service cotton waste for workshop care from prepared cotton stock.",
		Action = "making service cotton waste for workshop care",
		ActiveCraftItemSdesc = "an in-progress service cotton waste for workshop care craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1651.0, Material: "cotton", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1651 grams of cotton", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrial_medical_mobility_0001_service_waste", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a service cotton waste for workshop care (@industrial_medical_mobility_0001_service_waste)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MedicalAndMobilityMakeServiceLinenBandageForWorkshopCareCraftIndustrialMedicalMobility0002ServiceBandage() => new()
	{
		Metadata = new("Medical and mobility/make service linen bandage for workshop care [craft_industrial_medical_mobility_0002_service_bandage]", "crafts", ["industrial"], ["item:industrial_medical_mobility_0002_service_bandage"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make service linen bandage for workshop care [craft_industrial_medical_mobility_0002_service_bandage]",
		Category = "Medical and mobility",
		Blurb = "Make a service linen bandage for workshop care from prepared linen stock.",
		Action = "making service linen bandage for workshop care",
		ActiveCraftItemSdesc = "an in-progress service linen bandage for workshop care craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1692.0, Material: "linen", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1692 grams of linen", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrial_medical_mobility_0002_service_bandage", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a service linen bandage for workshop care (@industrial_medical_mobility_0002_service_bandage)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MedicalAndMobilityMakeServiceMildSteelSplintForWorkshopCareCraftIndustrialMedicalMobility0003ServiceSplint() => new()
	{
		Metadata = new("Medical and mobility/make service mild steel splint for workshop care [craft_industrial_medical_mobility_0003_service_splint]", "crafts", ["industrial"], ["item:industrial_medical_mobility_0003_service_splint"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make service mild steel splint for workshop care [craft_industrial_medical_mobility_0003_service_splint]",
		Category = "Medical and mobility",
		Blurb = "Make a service mild steel splint for workshop care from prepared mild steel stock.",
		Action = "making service mild steel splint for workshop care",
		ActiveCraftItemSdesc = "an in-progress service mild steel splint for workshop care craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1733.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1733 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrial_medical_mobility_0003_service_splint", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a service mild steel splint for workshop care (@industrial_medical_mobility_0003_service_splint)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MedicalAndMobilityMakeServiceGlassCrutchForWorkshopCareCraftIndustrialMedicalMobility0004ServiceCrutch() => new()
	{
		Metadata = new("Medical and mobility/make service glass crutch for workshop care [craft_industrial_medical_mobility_0004_service_crutch]", "crafts", ["industrial"], ["item:industrial_medical_mobility_0004_service_crutch"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make service glass crutch for workshop care [craft_industrial_medical_mobility_0004_service_crutch]",
		Category = "Medical and mobility",
		Blurb = "Make a service glass crutch for workshop care from prepared glass stock.",
		Action = "making service glass crutch for workshop care",
		ActiveCraftItemSdesc = "an in-progress service glass crutch for workshop care craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1773.0, Material: "glass", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1773 grams of glass", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrial_medical_mobility_0004_service_crutch", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a service glass crutch for workshop care (@industrial_medical_mobility_0004_service_crutch)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MedicalAndMobilityMakeServiceOakStretcherForWorkshopCareCraftIndustrialMedicalMobility0005ServiceStretcher() => new()
	{
		Metadata = new("Medical and mobility/make service oak stretcher for workshop care [craft_industrial_medical_mobility_0005_service_stretcher]", "crafts", ["industrial"], ["item:industrial_medical_mobility_0005_service_stretcher"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make service oak stretcher for workshop care [craft_industrial_medical_mobility_0005_service_stretcher]",
		Category = "Medical and mobility",
		Blurb = "Make a service oak stretcher for workshop care from prepared oak stock.",
		Action = "making service oak stretcher for workshop care",
		ActiveCraftItemSdesc = "an in-progress service oak stretcher for workshop care craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1814.0, Material: "oak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1814 grams of oak", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrial_medical_mobility_0005_service_stretcher", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a service oak stretcher for workshop care (@industrial_medical_mobility_0005_service_stretcher)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MedicalAndMobilityMakeServiceCottonSyringeForWorkshopCareCraftIndustrialMedicalMobility0006ServiceSyringe() => new()
	{
		Metadata = new("Medical and mobility/make service cotton syringe for workshop care [craft_industrial_medical_mobility_0006_service_syringe]", "crafts", ["industrial"], ["item:industrial_medical_mobility_0006_service_syringe"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make service cotton syringe for workshop care [craft_industrial_medical_mobility_0006_service_syringe]",
		Category = "Medical and mobility",
		Blurb = "Make a service cotton syringe for workshop care from prepared cotton stock.",
		Action = "making service cotton syringe for workshop care",
		ActiveCraftItemSdesc = "an in-progress service cotton syringe for workshop care craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1855.0, Material: "cotton", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1855 grams of cotton", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrial_medical_mobility_0006_service_syringe", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a service cotton syringe for workshop care (@industrial_medical_mobility_0006_service_syringe)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MedicalAndMobilityMakeSomeServiceLinenForcepsForWorkshopCareCraftIndustrialMedicalMobility0007ServiceForceps() => new()
	{
		Metadata = new("Medical and mobility/make some service linen forceps for workshop care [craft_industrial_medical_mobility_0007_service_forceps]", "crafts", ["industrial"], ["item:industrial_medical_mobility_0007_service_forceps"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make some service linen forceps for workshop care [craft_industrial_medical_mobility_0007_service_forceps]",
		Category = "Medical and mobility",
		Blurb = "Make some service linen forceps for workshop care from prepared linen stock.",
		Action = "making some service linen forceps for workshop care",
		ActiveCraftItemSdesc = "an in-progress some service linen forceps for workshop care craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
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
			new CatalogueItemRecipeProduct(ItemStableReference: "industrial_medical_mobility_0007_service_forceps", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x some service linen forceps for workshop care (@industrial_medical_mobility_0007_service_forceps)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MedicalAndMobilityMakeServiceMildSteelThermometerForWorkshopCareCraftIndustrialMedicalMobility0008ServiceThermometer() => new()
	{
		Metadata = new("Medical and mobility/make service mild steel thermometer for workshop care [craft_industrial_medical_mobility_0008_service_thermometer]", "crafts", ["industrial"], ["item:industrial_medical_mobility_0008_service_thermometer"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make service mild steel thermometer for workshop care [craft_industrial_medical_mobility_0008_service_thermometer]",
		Category = "Medical and mobility",
		Blurb = "Make a service mild steel thermometer for workshop care from prepared mild steel stock.",
		Action = "making service mild steel thermometer for workshop care",
		ActiveCraftItemSdesc = "an in-progress service mild steel thermometer for workshop care craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
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
			new CatalogueItemRecipeProduct(ItemStableReference: "industrial_medical_mobility_0008_service_thermometer", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a service mild steel thermometer for workshop care (@industrial_medical_mobility_0008_service_thermometer)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MedicalAndMobilityMakeServiceGlassCaseForWorkshopCareCraftIndustrialMedicalMobility0009ServiceCase() => new()
	{
		Metadata = new("Medical and mobility/make service glass case for workshop care [craft_industrial_medical_mobility_0009_service_case]", "crafts", ["industrial"], ["item:industrial_medical_mobility_0009_service_case"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make service glass case for workshop care [craft_industrial_medical_mobility_0009_service_case]",
		Category = "Medical and mobility",
		Blurb = "Make a service glass case for workshop care from prepared glass stock.",
		Action = "making service glass case for workshop care",
		ActiveCraftItemSdesc = "an in-progress service glass case for workshop care craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
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
			new CatalogueItemRecipeProduct(ItemStableReference: "industrial_medical_mobility_0009_service_case", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a service glass case for workshop care (@industrial_medical_mobility_0009_service_case)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition WeaponsPoliceAndMilitaryMakePlainMildSteelScrapForDomesticSecurityCraftIndustrialisedWeaponsPoliceMilitary0001PlainScrap() => new()
	{
		Metadata = new("Weapons, police and military/make plain mild steel scrap for domestic security [craft_industrialised_weapons_police_military_0001_plain_scrap]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_weapons_police_military_0001_plain_scrap"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make plain mild steel scrap for domestic security [craft_industrialised_weapons_police_military_0001_plain_scrap]",
		Category = "Weapons, police and military",
		Blurb = "Make a plain mild steel scrap for domestic security from prepared mild steel stock.",
		Action = "making plain mild steel scrap for domestic security",
		ActiveCraftItemSdesc = "an in-progress plain mild steel scrap for domestic security craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1762.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1762 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_weapons_police_military_0001_plain_scrap", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a plain mild steel scrap for domestic security (@industrialised_weapons_police_military_0001_plain_scrap)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition WeaponsPoliceAndMilitaryMakePlainLeatherHolsterForDomesticSecurityCraftIndustrialisedWeaponsPoliceMilitary0002PlainHolster() => new()
	{
		Metadata = new("Weapons, police and military/make plain leather holster for domestic security [craft_industrialised_weapons_police_military_0002_plain_holster]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_weapons_police_military_0002_plain_holster"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make plain leather holster for domestic security [craft_industrialised_weapons_police_military_0002_plain_holster]",
		Category = "Weapons, police and military",
		Blurb = "Make a plain leather holster for domestic security from prepared leather stock.",
		Action = "making plain leather holster for domestic security",
		ActiveCraftItemSdesc = "an in-progress plain leather holster for domestic security craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1803.0, Material: "leather", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1803 grams of leather", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_weapons_police_military_0002_plain_holster", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a plain leather holster for domestic security (@industrialised_weapons_police_military_0002_plain_holster)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition WeaponsPoliceAndMilitaryMakePlainCottonBatonForDomesticSecurityCraftIndustrialisedWeaponsPoliceMilitary0003PlainBaton() => new()
	{
		Metadata = new("Weapons, police and military/make plain cotton baton for domestic security [craft_industrialised_weapons_police_military_0003_plain_baton]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_weapons_police_military_0003_plain_baton"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make plain cotton baton for domestic security [craft_industrialised_weapons_police_military_0003_plain_baton]",
		Category = "Weapons, police and military",
		Blurb = "Make a plain cotton baton for domestic security from prepared cotton stock.",
		Action = "making plain cotton baton for domestic security",
		ActiveCraftItemSdesc = "an in-progress plain cotton baton for domestic security craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1844.0, Material: "cotton", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1844 grams of cotton", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_weapons_police_military_0003_plain_baton", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a plain cotton baton for domestic security (@industrialised_weapons_police_military_0003_plain_baton)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition WeaponsPoliceAndMilitaryMakePlainOakScabbardForDomesticSecurityCraftIndustrialisedWeaponsPoliceMilitary0004PlainScabbard() => new()
	{
		Metadata = new("Weapons, police and military/make plain oak scabbard for domestic security [craft_industrialised_weapons_police_military_0004_plain_scabbard]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_weapons_police_military_0004_plain_scabbard"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make plain oak scabbard for domestic security [craft_industrialised_weapons_police_military_0004_plain_scabbard]",
		Category = "Weapons, police and military",
		Blurb = "Make a plain oak scabbard for domestic security from prepared oak stock.",
		Action = "making plain oak scabbard for domestic security",
		ActiveCraftItemSdesc = "an in-progress plain oak scabbard for domestic security craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1884.0, Material: "oak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1884 grams of oak", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_weapons_police_military_0004_plain_scabbard", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a plain oak scabbard for domestic security (@industrialised_weapons_police_military_0004_plain_scabbard)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition WeaponsPoliceAndMilitaryMakePlainMildSteelCartridgeForDomesticSecurityCraftIndustrialisedWeaponsPoliceMilitary0005PlainCartridge() => new()
	{
		Metadata = new("Weapons, police and military/make plain mild steel cartridge for domestic security [craft_industrialised_weapons_police_military_0005_plain_cartridge]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_weapons_police_military_0005_plain_cartridge"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make plain mild steel cartridge for domestic security [craft_industrialised_weapons_police_military_0005_plain_cartridge]",
		Category = "Weapons, police and military",
		Blurb = "Make a plain mild steel cartridge for domestic security from prepared mild steel stock.",
		Action = "making plain mild steel cartridge for domestic security",
		ActiveCraftItemSdesc = "an in-progress plain mild steel cartridge for domestic security craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1925.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1925 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_weapons_police_military_0005_plain_cartridge", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a plain mild steel cartridge for domestic security (@industrialised_weapons_police_military_0005_plain_cartridge)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition WeaponsPoliceAndMilitaryMakePlainLeatherMagazineForDomesticSecurityCraftIndustrialisedWeaponsPoliceMilitary0006PlainMagazine() => new()
	{
		Metadata = new("Weapons, police and military/make plain leather magazine for domestic security [craft_industrialised_weapons_police_military_0006_plain_magazine]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_weapons_police_military_0006_plain_magazine"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make plain leather magazine for domestic security [craft_industrialised_weapons_police_military_0006_plain_magazine]",
		Category = "Weapons, police and military",
		Blurb = "Make a plain leather magazine for domestic security from prepared leather stock.",
		Action = "making plain leather magazine for domestic security",
		ActiveCraftItemSdesc = "an in-progress plain leather magazine for domestic security craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1966.0, Material: "leather", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1966 grams of leather", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_weapons_police_military_0006_plain_magazine", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a plain leather magazine for domestic security (@industrialised_weapons_police_military_0006_plain_magazine)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition WeaponsPoliceAndMilitaryMakePlainCottonShieldForDomesticSecurityCraftIndustrialisedWeaponsPoliceMilitary0007PlainShield() => new()
	{
		Metadata = new("Weapons, police and military/make plain cotton shield for domestic security [craft_industrialised_weapons_police_military_0007_plain_shield]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_weapons_police_military_0007_plain_shield"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make plain cotton shield for domestic security [craft_industrialised_weapons_police_military_0007_plain_shield]",
		Category = "Weapons, police and military",
		Blurb = "Make a plain cotton shield for domestic security from prepared cotton stock.",
		Action = "making plain cotton shield for domestic security",
		ActiveCraftItemSdesc = "an in-progress plain cotton shield for domestic security craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2006.0, Material: "cotton", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2006 grams of cotton", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_weapons_police_military_0007_plain_shield", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a plain cotton shield for domestic security (@industrialised_weapons_police_military_0007_plain_shield)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition WeaponsPoliceAndMilitaryMakePlainOakHelmetForDomesticSecurityCraftIndustrialisedWeaponsPoliceMilitary0008PlainHelmet() => new()
	{
		Metadata = new("Weapons, police and military/make plain oak helmet for domestic security [craft_industrialised_weapons_police_military_0008_plain_helmet]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_weapons_police_military_0008_plain_helmet"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make plain oak helmet for domestic security [craft_industrialised_weapons_police_military_0008_plain_helmet]",
		Category = "Weapons, police and military",
		Blurb = "Make a plain oak helmet for domestic security from prepared oak stock.",
		Action = "making plain oak helmet for domestic security",
		ActiveCraftItemSdesc = "an in-progress plain oak helmet for domestic security craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2047.0, Material: "oak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2047 grams of oak", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_weapons_police_military_0008_plain_helmet", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a plain oak helmet for domestic security (@industrialised_weapons_police_military_0008_plain_helmet)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition WeaponsPoliceAndMilitaryMakePlainMildSteelPouchForDomesticSecurityCraftIndustrialisedWeaponsPoliceMilitary0009PlainPouch() => new()
	{
		Metadata = new("Weapons, police and military/make plain mild steel pouch for domestic security [craft_industrialised_weapons_police_military_0009_plain_pouch]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_weapons_police_military_0009_plain_pouch"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make plain mild steel pouch for domestic security [craft_industrialised_weapons_police_military_0009_plain_pouch]",
		Category = "Weapons, police and military",
		Blurb = "Make a plain mild steel pouch for domestic security from prepared mild steel stock.",
		Action = "making plain mild steel pouch for domestic security",
		ActiveCraftItemSdesc = "an in-progress plain mild steel pouch for domestic security craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2088.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2088 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_weapons_police_military_0009_plain_pouch", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a plain mild steel pouch for domestic security (@industrialised_weapons_police_military_0009_plain_pouch)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition WeaponsPoliceAndMilitaryMakePlainLeatherSignalForDomesticSecurityCraftIndustrialisedWeaponsPoliceMilitary0010PlainSignal() => new()
	{
		Metadata = new("Weapons, police and military/make plain leather signal for domestic security [craft_industrialised_weapons_police_military_0010_plain_signal]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_weapons_police_military_0010_plain_signal"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make plain leather signal for domestic security [craft_industrialised_weapons_police_military_0010_plain_signal]",
		Category = "Weapons, police and military",
		Blurb = "Make a plain leather signal for domestic security from prepared leather stock.",
		Action = "making plain leather signal for domestic security",
		ActiveCraftItemSdesc = "an in-progress plain leather signal for domestic security craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2128.0, Material: "leather", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2128 grams of leather", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_weapons_police_military_0010_plain_signal", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a plain leather signal for domestic security (@industrialised_weapons_police_military_0010_plain_signal)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition WeaponsPoliceAndMilitaryMakeReinforcedCottonScrapForDomesticSecurityCraftIndustrialisedWeaponsPoliceMilitary0011ReinforcedScrap() => new()
	{
		Metadata = new("Weapons, police and military/make reinforced cotton scrap for domestic security [craft_industrialised_weapons_police_military_0011_reinforced_scrap]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_weapons_police_military_0011_reinforced_scrap"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make reinforced cotton scrap for domestic security [craft_industrialised_weapons_police_military_0011_reinforced_scrap]",
		Category = "Weapons, police and military",
		Blurb = "Make a reinforced cotton scrap for domestic security from prepared cotton stock.",
		Action = "making reinforced cotton scrap for domestic security",
		ActiveCraftItemSdesc = "an in-progress reinforced cotton scrap for domestic security craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2169.0, Material: "cotton", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2169 grams of cotton", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_weapons_police_military_0011_reinforced_scrap", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a reinforced cotton scrap for domestic security (@industrialised_weapons_police_military_0011_reinforced_scrap)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition WeaponsPoliceAndMilitaryMakeReinforcedOakHolsterForDomesticSecurityCraftIndustrialisedWeaponsPoliceMilitary0012ReinforcedHolster() => new()
	{
		Metadata = new("Weapons, police and military/make reinforced oak holster for domestic security [craft_industrialised_weapons_police_military_0012_reinforced_holster]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_weapons_police_military_0012_reinforced_holster"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make reinforced oak holster for domestic security [craft_industrialised_weapons_police_military_0012_reinforced_holster]",
		Category = "Weapons, police and military",
		Blurb = "Make a reinforced oak holster for domestic security from prepared oak stock.",
		Action = "making reinforced oak holster for domestic security",
		ActiveCraftItemSdesc = "an in-progress reinforced oak holster for domestic security craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2210.0, Material: "oak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2210 grams of oak", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_weapons_police_military_0012_reinforced_holster", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a reinforced oak holster for domestic security (@industrialised_weapons_police_military_0012_reinforced_holster)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};
}
