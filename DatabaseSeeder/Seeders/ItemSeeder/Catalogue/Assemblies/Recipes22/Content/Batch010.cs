#nullable enable

using MudSharp.Body;
using MudSharp.GameItems.Inventory.Plans;
using MudSharp.RPG.Checks;

namespace DatabaseSeeder.Seeders;

public static partial class Recipes22Stock
{
	public static CatalogueRecipeDefinition ScienceAndEducationMakeReinforcedOakBeakerForDomesticInstructionCraftIndustrialisedScienceEducation0015ReinforcedBeaker() => new()
	{
		Metadata = new("Science and education/make reinforced oak beaker for domestic instruction [craft_industrialised_science_education_0015_reinforced_beaker]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_science_education_0015_reinforced_beaker"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make reinforced oak beaker for domestic instruction [craft_industrialised_science_education_0015_reinforced_beaker]",
		Category = "Science and education",
		Blurb = "Make a reinforced oak beaker for domestic instruction from prepared oak stock.",
		Action = "making reinforced oak beaker for domestic instruction",
		ActiveCraftItemSdesc = "an in-progress reinforced oak beaker for domestic instruction craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2776.0, Material: "oak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2776 grams of oak", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_science_education_0015_reinforced_beaker", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a reinforced oak beaker for domestic instruction (@industrialised_science_education_0015_reinforced_beaker)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ScienceAndEducationMakeReinforcedMildSteelModelForDomesticInstructionCraftIndustrialisedScienceEducation0016ReinforcedModel() => new()
	{
		Metadata = new("Science and education/make reinforced mild steel model for domestic instruction [craft_industrialised_science_education_0016_reinforced_model]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_science_education_0016_reinforced_model"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make reinforced mild steel model for domestic instruction [craft_industrialised_science_education_0016_reinforced_model]",
		Category = "Science and education",
		Blurb = "Make a reinforced mild steel model for domestic instruction from prepared mild steel stock.",
		Action = "making reinforced mild steel model for domestic instruction",
		ActiveCraftItemSdesc = "an in-progress reinforced mild steel model for domestic instruction craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2817.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2817 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_science_education_0016_reinforced_model", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a reinforced mild steel model for domestic instruction (@industrialised_science_education_0016_reinforced_model)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ScienceAndEducationMakeReinforcedGlassChartForDomesticInstructionCraftIndustrialisedScienceEducation0017ReinforcedChart() => new()
	{
		Metadata = new("Science and education/make reinforced glass chart for domestic instruction [craft_industrialised_science_education_0017_reinforced_chart]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_science_education_0017_reinforced_chart"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make reinforced glass chart for domestic instruction [craft_industrialised_science_education_0017_reinforced_chart]",
		Category = "Science and education",
		Blurb = "Make a reinforced glass chart for domestic instruction from prepared glass stock.",
		Action = "making reinforced glass chart for domestic instruction",
		ActiveCraftItemSdesc = "an in-progress reinforced glass chart for domestic instruction craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2858.0, Material: "glass", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2858 grams of glass", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_science_education_0017_reinforced_chart", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a reinforced glass chart for domestic instruction (@industrialised_science_education_0017_reinforced_chart)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ScienceAndEducationMakeReinforcedPaperPointerForDomesticInstructionCraftIndustrialisedScienceEducation0018ReinforcedPointer() => new()
	{
		Metadata = new("Science and education/make reinforced paper pointer for domestic instruction [craft_industrialised_science_education_0018_reinforced_pointer]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_science_education_0018_reinforced_pointer"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make reinforced paper pointer for domestic instruction [craft_industrialised_science_education_0018_reinforced_pointer]",
		Category = "Science and education",
		Blurb = "Make a reinforced paper pointer for domestic instruction from prepared paper stock.",
		Action = "making reinforced paper pointer for domestic instruction",
		ActiveCraftItemSdesc = "an in-progress reinforced paper pointer for domestic instruction craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2899.0, Material: "paper", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2899 grams of paper", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_science_education_0018_reinforced_pointer", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a reinforced paper pointer for domestic instruction (@industrialised_science_education_0018_reinforced_pointer)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ScienceAndEducationMakeReinforcedOakBalanceForDomesticInstructionCraftIndustrialisedScienceEducation0019ReinforcedBalance() => new()
	{
		Metadata = new("Science and education/make reinforced oak balance for domestic instruction [craft_industrialised_science_education_0019_reinforced_balance]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_science_education_0019_reinforced_balance"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make reinforced oak balance for domestic instruction [craft_industrialised_science_education_0019_reinforced_balance]",
		Category = "Science and education",
		Blurb = "Make a reinforced oak balance for domestic instruction from prepared oak stock.",
		Action = "making reinforced oak balance for domestic instruction",
		ActiveCraftItemSdesc = "an in-progress reinforced oak balance for domestic instruction craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2939.0, Material: "oak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2939 grams of oak", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_science_education_0019_reinforced_balance", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a reinforced oak balance for domestic instruction (@industrialised_science_education_0019_reinforced_balance)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ScienceAndEducationMakeReinforcedMildSteelCaseForDomesticInstructionCraftIndustrialisedScienceEducation0020ReinforcedCase() => new()
	{
		Metadata = new("Science and education/make reinforced mild steel case for domestic instruction [craft_industrialised_science_education_0020_reinforced_case]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_science_education_0020_reinforced_case"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make reinforced mild steel case for domestic instruction [craft_industrialised_science_education_0020_reinforced_case]",
		Category = "Science and education",
		Blurb = "Make a reinforced mild steel case for domestic instruction from prepared mild steel stock.",
		Action = "making reinforced mild steel case for domestic instruction",
		ActiveCraftItemSdesc = "an in-progress reinforced mild steel case for domestic instruction craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2980.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2980 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_science_education_0020_reinforced_case", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a reinforced mild steel case for domestic instruction (@industrialised_science_education_0020_reinforced_case)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ScienceAndEducationMakeCompactGlassWasteForDomesticInstructionCraftIndustrialisedScienceEducation0021CompactWaste() => new()
	{
		Metadata = new("Science and education/make compact glass waste for domestic instruction [craft_industrialised_science_education_0021_compact_waste]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_science_education_0021_compact_waste"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make compact glass waste for domestic instruction [craft_industrialised_science_education_0021_compact_waste]",
		Category = "Science and education",
		Blurb = "Make a compact glass waste for domestic instruction from prepared glass stock.",
		Action = "making compact glass waste for domestic instruction",
		ActiveCraftItemSdesc = "an in-progress compact glass waste for domestic instruction craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3021.0, Material: "glass", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3021 grams of glass", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_science_education_0021_compact_waste", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a compact glass waste for domestic instruction (@industrialised_science_education_0021_compact_waste)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ScienceAndEducationMakeCompactPaperSpecimenForDomesticInstructionCraftIndustrialisedScienceEducation0022CompactSpecimen() => new()
	{
		Metadata = new("Science and education/make compact paper specimen for domestic instruction [craft_industrialised_science_education_0022_compact_specimen]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_science_education_0022_compact_specimen"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make compact paper specimen for domestic instruction [craft_industrialised_science_education_0022_compact_specimen]",
		Category = "Science and education",
		Blurb = "Make a compact paper specimen for domestic instruction from prepared paper stock.",
		Action = "making compact paper specimen for domestic instruction",
		ActiveCraftItemSdesc = "an in-progress compact paper specimen for domestic instruction craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3061.0, Material: "paper", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3061 grams of paper", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_science_education_0022_compact_specimen", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a compact paper specimen for domestic instruction (@industrialised_science_education_0022_compact_specimen)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ScienceAndEducationMakeCompactOakSlideForDomesticInstructionCraftIndustrialisedScienceEducation0023CompactSlide() => new()
	{
		Metadata = new("Science and education/make compact oak slide for domestic instruction [craft_industrialised_science_education_0023_compact_slide]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_science_education_0023_compact_slide"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make compact oak slide for domestic instruction [craft_industrialised_science_education_0023_compact_slide]",
		Category = "Science and education",
		Blurb = "Make a compact oak slide for domestic instruction from prepared oak stock.",
		Action = "making compact oak slide for domestic instruction",
		ActiveCraftItemSdesc = "an in-progress compact oak slide for domestic instruction craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3102.0, Material: "oak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3102 grams of oak", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_science_education_0023_compact_slide", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a compact oak slide for domestic instruction (@industrialised_science_education_0023_compact_slide)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ScienceAndEducationMakeCompactMildSteelFlaskForDomesticInstructionCraftIndustrialisedScienceEducation0024CompactFlask() => new()
	{
		Metadata = new("Science and education/make compact mild steel flask for domestic instruction [craft_industrialised_science_education_0024_compact_flask]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_science_education_0024_compact_flask"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make compact mild steel flask for domestic instruction [craft_industrialised_science_education_0024_compact_flask]",
		Category = "Science and education",
		Blurb = "Make a compact mild steel flask for domestic instruction from prepared mild steel stock.",
		Action = "making compact mild steel flask for domestic instruction",
		ActiveCraftItemSdesc = "an in-progress compact mild steel flask for domestic instruction craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3143.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3143 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_science_education_0024_compact_flask", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a compact mild steel flask for domestic instruction (@industrialised_science_education_0024_compact_flask)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ScienceAndEducationMakeCompactGlassBeakerForDomesticInstructionCraftIndustrialisedScienceEducation0025CompactBeaker() => new()
	{
		Metadata = new("Science and education/make compact glass beaker for domestic instruction [craft_industrialised_science_education_0025_compact_beaker]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_science_education_0025_compact_beaker"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make compact glass beaker for domestic instruction [craft_industrialised_science_education_0025_compact_beaker]",
		Category = "Science and education",
		Blurb = "Make a compact glass beaker for domestic instruction from prepared glass stock.",
		Action = "making compact glass beaker for domestic instruction",
		ActiveCraftItemSdesc = "an in-progress compact glass beaker for domestic instruction craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3183.0, Material: "glass", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3183 grams of glass", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_science_education_0025_compact_beaker", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a compact glass beaker for domestic instruction (@industrialised_science_education_0025_compact_beaker)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ScienceAndEducationMakeCompactPaperModelForDomesticInstructionCraftIndustrialisedScienceEducation0026CompactModel() => new()
	{
		Metadata = new("Science and education/make compact paper model for domestic instruction [craft_industrialised_science_education_0026_compact_model]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_science_education_0026_compact_model"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make compact paper model for domestic instruction [craft_industrialised_science_education_0026_compact_model]",
		Category = "Science and education",
		Blurb = "Make a compact paper model for domestic instruction from prepared paper stock.",
		Action = "making compact paper model for domestic instruction",
		ActiveCraftItemSdesc = "an in-progress compact paper model for domestic instruction craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3224.0, Material: "paper", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3224 grams of paper", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_science_education_0026_compact_model", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a compact paper model for domestic instruction (@industrialised_science_education_0026_compact_model)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ScienceAndEducationMakeCompactOakChartForDomesticInstructionCraftIndustrialisedScienceEducation0027CompactChart() => new()
	{
		Metadata = new("Science and education/make compact oak chart for domestic instruction [craft_industrialised_science_education_0027_compact_chart]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_science_education_0027_compact_chart"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make compact oak chart for domestic instruction [craft_industrialised_science_education_0027_compact_chart]",
		Category = "Science and education",
		Blurb = "Make a compact oak chart for domestic instruction from prepared oak stock.",
		Action = "making compact oak chart for domestic instruction",
		ActiveCraftItemSdesc = "an in-progress compact oak chart for domestic instruction craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3265.0, Material: "oak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3265 grams of oak", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_science_education_0027_compact_chart", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a compact oak chart for domestic instruction (@industrialised_science_education_0027_compact_chart)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ScienceAndEducationMakeCompactMildSteelPointerForDomesticInstructionCraftIndustrialisedScienceEducation0028CompactPointer() => new()
	{
		Metadata = new("Science and education/make compact mild steel pointer for domestic instruction [craft_industrialised_science_education_0028_compact_pointer]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_science_education_0028_compact_pointer"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make compact mild steel pointer for domestic instruction [craft_industrialised_science_education_0028_compact_pointer]",
		Category = "Science and education",
		Blurb = "Make a compact mild steel pointer for domestic instruction from prepared mild steel stock.",
		Action = "making compact mild steel pointer for domestic instruction",
		ActiveCraftItemSdesc = "an in-progress compact mild steel pointer for domestic instruction craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3306.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3306 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_science_education_0028_compact_pointer", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a compact mild steel pointer for domestic instruction (@industrialised_science_education_0028_compact_pointer)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ScienceAndEducationMakeCompactGlassBalanceForDomesticInstructionCraftIndustrialisedScienceEducation0029CompactBalance() => new()
	{
		Metadata = new("Science and education/make compact glass balance for domestic instruction [craft_industrialised_science_education_0029_compact_balance]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_science_education_0029_compact_balance"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make compact glass balance for domestic instruction [craft_industrialised_science_education_0029_compact_balance]",
		Category = "Science and education",
		Blurb = "Make a compact glass balance for domestic instruction from prepared glass stock.",
		Action = "making compact glass balance for domestic instruction",
		ActiveCraftItemSdesc = "an in-progress compact glass balance for domestic instruction craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3346.0, Material: "glass", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3346 grams of glass", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_science_education_0029_compact_balance", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a compact glass balance for domestic instruction (@industrialised_science_education_0029_compact_balance)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ScienceAndEducationMakeCompactPaperCaseForDomesticInstructionCraftIndustrialisedScienceEducation0030CompactCase() => new()
	{
		Metadata = new("Science and education/make compact paper case for domestic instruction [craft_industrialised_science_education_0030_compact_case]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_science_education_0030_compact_case"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make compact paper case for domestic instruction [craft_industrialised_science_education_0030_compact_case]",
		Category = "Science and education",
		Blurb = "Make a compact paper case for domestic instruction from prepared paper stock.",
		Action = "making compact paper case for domestic instruction",
		ActiveCraftItemSdesc = "an in-progress compact paper case for domestic instruction craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3387.0, Material: "paper", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3387 grams of paper", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_science_education_0030_compact_case", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a compact paper case for domestic instruction (@industrialised_science_education_0030_compact_case)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ScienceAndEducationMakeHeavyDutyOakWasteForDomesticInstructionCraftIndustrialisedScienceEducation0031HeavyDutyWaste() => new()
	{
		Metadata = new("Science and education/make heavy-duty oak waste for domestic instruction [craft_industrialised_science_education_0031_heavy_duty_waste]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_science_education_0031_heavy_duty_waste"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make heavy-duty oak waste for domestic instruction [craft_industrialised_science_education_0031_heavy_duty_waste]",
		Category = "Science and education",
		Blurb = "Make a heavy-duty oak waste for domestic instruction from prepared oak stock.",
		Action = "making heavy-duty oak waste for domestic instruction",
		ActiveCraftItemSdesc = "an in-progress heavy-duty oak waste for domestic instruction craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3428.0, Material: "oak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3428 grams of oak", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_science_education_0031_heavy_duty_waste", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a heavy-duty oak waste for domestic instruction (@industrialised_science_education_0031_heavy_duty_waste)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ScienceAndEducationMakeHeavyDutyMildSteelSpecimenForDomesticInstructionCraftIndustrialisedScienceEducation0032HeavyDutySpecimen() => new()
	{
		Metadata = new("Science and education/make heavy-duty mild steel specimen for domestic instruction [craft_industrialised_science_education_0032_heavy_duty_specimen]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_science_education_0032_heavy_duty_specimen"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make heavy-duty mild steel specimen for domestic instruction [craft_industrialised_science_education_0032_heavy_duty_specimen]",
		Category = "Science and education",
		Blurb = "Make a heavy-duty mild steel specimen for domestic instruction from prepared mild steel stock.",
		Action = "making heavy-duty mild steel specimen for domestic instruction",
		ActiveCraftItemSdesc = "an in-progress heavy-duty mild steel specimen for domestic instruction craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3468.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3468 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_science_education_0032_heavy_duty_specimen", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a heavy-duty mild steel specimen for domestic instruction (@industrialised_science_education_0032_heavy_duty_specimen)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ScienceAndEducationMakeHeavyDutyGlassSlideForDomesticInstructionCraftIndustrialisedScienceEducation0033HeavyDutySlide() => new()
	{
		Metadata = new("Science and education/make heavy-duty glass slide for domestic instruction [craft_industrialised_science_education_0033_heavy_duty_slide]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_science_education_0033_heavy_duty_slide"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make heavy-duty glass slide for domestic instruction [craft_industrialised_science_education_0033_heavy_duty_slide]",
		Category = "Science and education",
		Blurb = "Make a heavy-duty glass slide for domestic instruction from prepared glass stock.",
		Action = "making heavy-duty glass slide for domestic instruction",
		ActiveCraftItemSdesc = "an in-progress heavy-duty glass slide for domestic instruction craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3509.0, Material: "glass", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3509 grams of glass", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_science_education_0033_heavy_duty_slide", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a heavy-duty glass slide for domestic instruction (@industrialised_science_education_0033_heavy_duty_slide)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ScienceAndEducationMakeHeavyDutyPaperFlaskForDomesticInstructionCraftIndustrialisedScienceEducation0034HeavyDutyFlask() => new()
	{
		Metadata = new("Science and education/make heavy-duty paper flask for domestic instruction [craft_industrialised_science_education_0034_heavy_duty_flask]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_science_education_0034_heavy_duty_flask"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make heavy-duty paper flask for domestic instruction [craft_industrialised_science_education_0034_heavy_duty_flask]",
		Category = "Science and education",
		Blurb = "Make a heavy-duty paper flask for domestic instruction from prepared paper stock.",
		Action = "making heavy-duty paper flask for domestic instruction",
		ActiveCraftItemSdesc = "an in-progress heavy-duty paper flask for domestic instruction craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3550.0, Material: "paper", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3550 grams of paper", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_science_education_0034_heavy_duty_flask", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a heavy-duty paper flask for domestic instruction (@industrialised_science_education_0034_heavy_duty_flask)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ScienceAndEducationMakeHeavyDutyOakBeakerForDomesticInstructionCraftIndustrialisedScienceEducation0035HeavyDutyBeaker() => new()
	{
		Metadata = new("Science and education/make heavy-duty oak beaker for domestic instruction [craft_industrialised_science_education_0035_heavy_duty_beaker]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_science_education_0035_heavy_duty_beaker"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make heavy-duty oak beaker for domestic instruction [craft_industrialised_science_education_0035_heavy_duty_beaker]",
		Category = "Science and education",
		Blurb = "Make a heavy-duty oak beaker for domestic instruction from prepared oak stock.",
		Action = "making heavy-duty oak beaker for domestic instruction",
		ActiveCraftItemSdesc = "an in-progress heavy-duty oak beaker for domestic instruction craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3590.0, Material: "oak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3590 grams of oak", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_science_education_0035_heavy_duty_beaker", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a heavy-duty oak beaker for domestic instruction (@industrialised_science_education_0035_heavy_duty_beaker)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ScienceAndEducationMakeHeavyDutyMildSteelModelForDomesticInstructionCraftIndustrialisedScienceEducation0036HeavyDutyModel() => new()
	{
		Metadata = new("Science and education/make heavy-duty mild steel model for domestic instruction [craft_industrialised_science_education_0036_heavy_duty_model]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_science_education_0036_heavy_duty_model"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make heavy-duty mild steel model for domestic instruction [craft_industrialised_science_education_0036_heavy_duty_model]",
		Category = "Science and education",
		Blurb = "Make a heavy-duty mild steel model for domestic instruction from prepared mild steel stock.",
		Action = "making heavy-duty mild steel model for domestic instruction",
		ActiveCraftItemSdesc = "an in-progress heavy-duty mild steel model for domestic instruction craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3631.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3631 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_science_education_0036_heavy_duty_model", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a heavy-duty mild steel model for domestic instruction (@industrialised_science_education_0036_heavy_duty_model)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ScienceAndEducationMakeHeavyDutyGlassChartForDomesticInstructionCraftIndustrialisedScienceEducation0037HeavyDutyChart() => new()
	{
		Metadata = new("Science and education/make heavy-duty glass chart for domestic instruction [craft_industrialised_science_education_0037_heavy_duty_chart]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_science_education_0037_heavy_duty_chart"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make heavy-duty glass chart for domestic instruction [craft_industrialised_science_education_0037_heavy_duty_chart]",
		Category = "Science and education",
		Blurb = "Make a heavy-duty glass chart for domestic instruction from prepared glass stock.",
		Action = "making heavy-duty glass chart for domestic instruction",
		ActiveCraftItemSdesc = "an in-progress heavy-duty glass chart for domestic instruction craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3672.0, Material: "glass", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3672 grams of glass", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_science_education_0037_heavy_duty_chart", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a heavy-duty glass chart for domestic instruction (@industrialised_science_education_0037_heavy_duty_chart)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ScienceAndEducationMakeHeavyDutyPaperPointerForDomesticInstructionCraftIndustrialisedScienceEducation0038HeavyDutyPointer() => new()
	{
		Metadata = new("Science and education/make heavy-duty paper pointer for domestic instruction [craft_industrialised_science_education_0038_heavy_duty_pointer]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_science_education_0038_heavy_duty_pointer"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make heavy-duty paper pointer for domestic instruction [craft_industrialised_science_education_0038_heavy_duty_pointer]",
		Category = "Science and education",
		Blurb = "Make a heavy-duty paper pointer for domestic instruction from prepared paper stock.",
		Action = "making heavy-duty paper pointer for domestic instruction",
		ActiveCraftItemSdesc = "an in-progress heavy-duty paper pointer for domestic instruction craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3713.0, Material: "paper", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3713 grams of paper", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_science_education_0038_heavy_duty_pointer", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a heavy-duty paper pointer for domestic instruction (@industrialised_science_education_0038_heavy_duty_pointer)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ScienceAndEducationMakeHeavyDutyOakBalanceForDomesticInstructionCraftIndustrialisedScienceEducation0039HeavyDutyBalance() => new()
	{
		Metadata = new("Science and education/make heavy-duty oak balance for domestic instruction [craft_industrialised_science_education_0039_heavy_duty_balance]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_science_education_0039_heavy_duty_balance"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make heavy-duty oak balance for domestic instruction [craft_industrialised_science_education_0039_heavy_duty_balance]",
		Category = "Science and education",
		Blurb = "Make a heavy-duty oak balance for domestic instruction from prepared oak stock.",
		Action = "making heavy-duty oak balance for domestic instruction",
		ActiveCraftItemSdesc = "an in-progress heavy-duty oak balance for domestic instruction craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3753.0, Material: "oak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3753 grams of oak", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_science_education_0039_heavy_duty_balance", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a heavy-duty oak balance for domestic instruction (@industrialised_science_education_0039_heavy_duty_balance)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ScienceAndEducationMakeHeavyDutyMildSteelCaseForDomesticInstructionCraftIndustrialisedScienceEducation0040HeavyDutyCase() => new()
	{
		Metadata = new("Science and education/make heavy-duty mild steel case for domestic instruction [craft_industrialised_science_education_0040_heavy_duty_case]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_science_education_0040_heavy_duty_case"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make heavy-duty mild steel case for domestic instruction [craft_industrialised_science_education_0040_heavy_duty_case]",
		Category = "Science and education",
		Blurb = "Make a heavy-duty mild steel case for domestic instruction from prepared mild steel stock.",
		Action = "making heavy-duty mild steel case for domestic instruction",
		ActiveCraftItemSdesc = "an in-progress heavy-duty mild steel case for domestic instruction craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3794.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3794 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_science_education_0040_heavy_duty_case", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a heavy-duty mild steel case for domestic instruction (@industrialised_science_education_0040_heavy_duty_case)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ScienceAndEducationMakeFoldingGlassWasteForDomesticInstructionCraftIndustrialisedScienceEducation0041FoldingWaste() => new()
	{
		Metadata = new("Science and education/make folding glass waste for domestic instruction [craft_industrialised_science_education_0041_folding_waste]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_science_education_0041_folding_waste"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make folding glass waste for domestic instruction [craft_industrialised_science_education_0041_folding_waste]",
		Category = "Science and education",
		Blurb = "Make a folding glass waste for domestic instruction from prepared glass stock.",
		Action = "making folding glass waste for domestic instruction",
		ActiveCraftItemSdesc = "an in-progress folding glass waste for domestic instruction craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3835.0, Material: "glass", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3835 grams of glass", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_science_education_0041_folding_waste", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a folding glass waste for domestic instruction (@industrialised_science_education_0041_folding_waste)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ScienceAndEducationMakeFoldingPaperSpecimenForDomesticInstructionCraftIndustrialisedScienceEducation0042FoldingSpecimen() => new()
	{
		Metadata = new("Science and education/make folding paper specimen for domestic instruction [craft_industrialised_science_education_0042_folding_specimen]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_science_education_0042_folding_specimen"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make folding paper specimen for domestic instruction [craft_industrialised_science_education_0042_folding_specimen]",
		Category = "Science and education",
		Blurb = "Make a folding paper specimen for domestic instruction from prepared paper stock.",
		Action = "making folding paper specimen for domestic instruction",
		ActiveCraftItemSdesc = "an in-progress folding paper specimen for domestic instruction craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3875.0, Material: "paper", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3875 grams of paper", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_science_education_0042_folding_specimen", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a folding paper specimen for domestic instruction (@industrialised_science_education_0042_folding_specimen)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ScienceAndEducationMakeFoldingOakSlideForDomesticInstructionCraftIndustrialisedScienceEducation0043FoldingSlide() => new()
	{
		Metadata = new("Science and education/make folding oak slide for domestic instruction [craft_industrialised_science_education_0043_folding_slide]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_science_education_0043_folding_slide"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make folding oak slide for domestic instruction [craft_industrialised_science_education_0043_folding_slide]",
		Category = "Science and education",
		Blurb = "Make a folding oak slide for domestic instruction from prepared oak stock.",
		Action = "making folding oak slide for domestic instruction",
		ActiveCraftItemSdesc = "an in-progress folding oak slide for domestic instruction craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3916.0, Material: "oak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3916 grams of oak", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_science_education_0043_folding_slide", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a folding oak slide for domestic instruction (@industrialised_science_education_0043_folding_slide)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ScienceAndEducationMakeFoldingMildSteelFlaskForDomesticInstructionCraftIndustrialisedScienceEducation0044FoldingFlask() => new()
	{
		Metadata = new("Science and education/make folding mild steel flask for domestic instruction [craft_industrialised_science_education_0044_folding_flask]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_science_education_0044_folding_flask"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make folding mild steel flask for domestic instruction [craft_industrialised_science_education_0044_folding_flask]",
		Category = "Science and education",
		Blurb = "Make a folding mild steel flask for domestic instruction from prepared mild steel stock.",
		Action = "making folding mild steel flask for domestic instruction",
		ActiveCraftItemSdesc = "an in-progress folding mild steel flask for domestic instruction craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3957.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3957 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_science_education_0044_folding_flask", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a folding mild steel flask for domestic instruction (@industrialised_science_education_0044_folding_flask)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ScienceAndEducationMakeFoldingGlassBeakerForDomesticInstructionCraftIndustrialisedScienceEducation0045FoldingBeaker() => new()
	{
		Metadata = new("Science and education/make folding glass beaker for domestic instruction [craft_industrialised_science_education_0045_folding_beaker]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_science_education_0045_folding_beaker"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make folding glass beaker for domestic instruction [craft_industrialised_science_education_0045_folding_beaker]",
		Category = "Science and education",
		Blurb = "Make a folding glass beaker for domestic instruction from prepared glass stock.",
		Action = "making folding glass beaker for domestic instruction",
		ActiveCraftItemSdesc = "an in-progress folding glass beaker for domestic instruction craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3997.0, Material: "glass", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3997 grams of glass", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_science_education_0045_folding_beaker", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a folding glass beaker for domestic instruction (@industrialised_science_education_0045_folding_beaker)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ScienceAndEducationMakeFoldingPaperModelForDomesticInstructionCraftIndustrialisedScienceEducation0046FoldingModel() => new()
	{
		Metadata = new("Science and education/make folding paper model for domestic instruction [craft_industrialised_science_education_0046_folding_model]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_science_education_0046_folding_model"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make folding paper model for domestic instruction [craft_industrialised_science_education_0046_folding_model]",
		Category = "Science and education",
		Blurb = "Make a folding paper model for domestic instruction from prepared paper stock.",
		Action = "making folding paper model for domestic instruction",
		ActiveCraftItemSdesc = "an in-progress folding paper model for domestic instruction craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 4038.0, Material: "paper", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("4038 grams of paper", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_science_education_0046_folding_model", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a folding paper model for domestic instruction (@industrialised_science_education_0046_folding_model)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ScienceAndEducationMakeFoldingOakChartForDomesticInstructionCraftIndustrialisedScienceEducation0047FoldingChart() => new()
	{
		Metadata = new("Science and education/make folding oak chart for domestic instruction [craft_industrialised_science_education_0047_folding_chart]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_science_education_0047_folding_chart"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make folding oak chart for domestic instruction [craft_industrialised_science_education_0047_folding_chart]",
		Category = "Science and education",
		Blurb = "Make a folding oak chart for domestic instruction from prepared oak stock.",
		Action = "making folding oak chart for domestic instruction",
		ActiveCraftItemSdesc = "an in-progress folding oak chart for domestic instruction craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 4079.0, Material: "oak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("4079 grams of oak", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_science_education_0047_folding_chart", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a folding oak chart for domestic instruction (@industrialised_science_education_0047_folding_chart)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ScienceAndEducationMakeFoldingMildSteelPointerForDomesticInstructionCraftIndustrialisedScienceEducation0048FoldingPointer() => new()
	{
		Metadata = new("Science and education/make folding mild steel pointer for domestic instruction [craft_industrialised_science_education_0048_folding_pointer]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_science_education_0048_folding_pointer"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make folding mild steel pointer for domestic instruction [craft_industrialised_science_education_0048_folding_pointer]",
		Category = "Science and education",
		Blurb = "Make a folding mild steel pointer for domestic instruction from prepared mild steel stock.",
		Action = "making folding mild steel pointer for domestic instruction",
		ActiveCraftItemSdesc = "an in-progress folding mild steel pointer for domestic instruction craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 4120.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("4120 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_science_education_0048_folding_pointer", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a folding mild steel pointer for domestic instruction (@industrialised_science_education_0048_folding_pointer)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ScienceAndEducationMakeFoldingGlassBalanceForDomesticInstructionCraftIndustrialisedScienceEducation0049FoldingBalance() => new()
	{
		Metadata = new("Science and education/make folding glass balance for domestic instruction [craft_industrialised_science_education_0049_folding_balance]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_science_education_0049_folding_balance"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make folding glass balance for domestic instruction [craft_industrialised_science_education_0049_folding_balance]",
		Category = "Science and education",
		Blurb = "Make a folding glass balance for domestic instruction from prepared glass stock.",
		Action = "making folding glass balance for domestic instruction",
		ActiveCraftItemSdesc = "an in-progress folding glass balance for domestic instruction craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 4160.0, Material: "glass", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("4160 grams of glass", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_science_education_0049_folding_balance", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a folding glass balance for domestic instruction (@industrialised_science_education_0049_folding_balance)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ScienceAndEducationMakeFoldingPaperCaseForDomesticInstructionCraftIndustrialisedScienceEducation0050FoldingCase() => new()
	{
		Metadata = new("Science and education/make folding paper case for domestic instruction [craft_industrialised_science_education_0050_folding_case]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_science_education_0050_folding_case"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make folding paper case for domestic instruction [craft_industrialised_science_education_0050_folding_case]",
		Category = "Science and education",
		Blurb = "Make a folding paper case for domestic instruction from prepared paper stock.",
		Action = "making folding paper case for domestic instruction",
		ActiveCraftItemSdesc = "an in-progress folding paper case for domestic instruction craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 4201.0, Material: "paper", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("4201 grams of paper", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_science_education_0050_folding_case", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a folding paper case for domestic instruction (@industrialised_science_education_0050_folding_case)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ScienceAndEducationMakePortableOakWasteForDomesticInstructionCraftIndustrialisedScienceEducation0051PortableWaste() => new()
	{
		Metadata = new("Science and education/make portable oak waste for domestic instruction [craft_industrialised_science_education_0051_portable_waste]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_science_education_0051_portable_waste"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make portable oak waste for domestic instruction [craft_industrialised_science_education_0051_portable_waste]",
		Category = "Science and education",
		Blurb = "Make a portable oak waste for domestic instruction from prepared oak stock.",
		Action = "making portable oak waste for domestic instruction",
		ActiveCraftItemSdesc = "an in-progress portable oak waste for domestic instruction craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 4242.0, Material: "oak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("4242 grams of oak", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_science_education_0051_portable_waste", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a portable oak waste for domestic instruction (@industrialised_science_education_0051_portable_waste)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ScienceAndEducationMakeStackableGlassWasteForDomesticInstructionCraftIndustrialScienceEducation0001StackableWaste() => new()
	{
		Metadata = new("Science and education/make stackable glass waste for domestic instruction [craft_industrial_science_education_0001_stackable_waste]", "crafts", ["industrial"], ["item:industrial_science_education_0001_stackable_waste"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make stackable glass waste for domestic instruction [craft_industrial_science_education_0001_stackable_waste]",
		Category = "Science and education",
		Blurb = "Make a stackable glass waste for domestic instruction from prepared glass stock.",
		Action = "making stackable glass waste for domestic instruction",
		ActiveCraftItemSdesc = "an in-progress stackable glass waste for domestic instruction craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2207.0, Material: "glass", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2207 grams of glass", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrial_science_education_0001_stackable_waste", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a stackable glass waste for domestic instruction (@industrial_science_education_0001_stackable_waste)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ScienceAndEducationMakeStackablePaperSpecimenForDomesticInstructionCraftIndustrialScienceEducation0002StackableSpecimen() => new()
	{
		Metadata = new("Science and education/make stackable paper specimen for domestic instruction [craft_industrial_science_education_0002_stackable_specimen]", "crafts", ["industrial"], ["item:industrial_science_education_0002_stackable_specimen"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make stackable paper specimen for domestic instruction [craft_industrial_science_education_0002_stackable_specimen]",
		Category = "Science and education",
		Blurb = "Make a stackable paper specimen for domestic instruction from prepared paper stock.",
		Action = "making stackable paper specimen for domestic instruction",
		ActiveCraftItemSdesc = "an in-progress stackable paper specimen for domestic instruction craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2247.0, Material: "paper", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2247 grams of paper", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrial_science_education_0002_stackable_specimen", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a stackable paper specimen for domestic instruction (@industrial_science_education_0002_stackable_specimen)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ScienceAndEducationMakeStackableOakSlideForDomesticInstructionCraftIndustrialScienceEducation0003StackableSlide() => new()
	{
		Metadata = new("Science and education/make stackable oak slide for domestic instruction [craft_industrial_science_education_0003_stackable_slide]", "crafts", ["industrial"], ["item:industrial_science_education_0003_stackable_slide"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make stackable oak slide for domestic instruction [craft_industrial_science_education_0003_stackable_slide]",
		Category = "Science and education",
		Blurb = "Make a stackable oak slide for domestic instruction from prepared oak stock.",
		Action = "making stackable oak slide for domestic instruction",
		ActiveCraftItemSdesc = "an in-progress stackable oak slide for domestic instruction craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2288.0, Material: "oak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2288 grams of oak", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrial_science_education_0003_stackable_slide", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a stackable oak slide for domestic instruction (@industrial_science_education_0003_stackable_slide)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ScienceAndEducationMakeStackableMildSteelFlaskForDomesticInstructionCraftIndustrialScienceEducation0004StackableFlask() => new()
	{
		Metadata = new("Science and education/make stackable mild steel flask for domestic instruction [craft_industrial_science_education_0004_stackable_flask]", "crafts", ["industrial"], ["item:industrial_science_education_0004_stackable_flask"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make stackable mild steel flask for domestic instruction [craft_industrial_science_education_0004_stackable_flask]",
		Category = "Science and education",
		Blurb = "Make a stackable mild steel flask for domestic instruction from prepared mild steel stock.",
		Action = "making stackable mild steel flask for domestic instruction",
		ActiveCraftItemSdesc = "an in-progress stackable mild steel flask for domestic instruction craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2329.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2329 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrial_science_education_0004_stackable_flask", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a stackable mild steel flask for domestic instruction (@industrial_science_education_0004_stackable_flask)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ScienceAndEducationMakeStackableGlassBeakerForDomesticInstructionCraftIndustrialScienceEducation0005StackableBeaker() => new()
	{
		Metadata = new("Science and education/make stackable glass beaker for domestic instruction [craft_industrial_science_education_0005_stackable_beaker]", "crafts", ["industrial"], ["item:industrial_science_education_0005_stackable_beaker"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make stackable glass beaker for domestic instruction [craft_industrial_science_education_0005_stackable_beaker]",
		Category = "Science and education",
		Blurb = "Make a stackable glass beaker for domestic instruction from prepared glass stock.",
		Action = "making stackable glass beaker for domestic instruction",
		ActiveCraftItemSdesc = "an in-progress stackable glass beaker for domestic instruction craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2369.0, Material: "glass", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2369 grams of glass", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrial_science_education_0005_stackable_beaker", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a stackable glass beaker for domestic instruction (@industrial_science_education_0005_stackable_beaker)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ScienceAndEducationMakeStackablePaperModelForDomesticInstructionCraftIndustrialScienceEducation0006StackableModel() => new()
	{
		Metadata = new("Science and education/make stackable paper model for domestic instruction [craft_industrial_science_education_0006_stackable_model]", "crafts", ["industrial"], ["item:industrial_science_education_0006_stackable_model"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make stackable paper model for domestic instruction [craft_industrial_science_education_0006_stackable_model]",
		Category = "Science and education",
		Blurb = "Make a stackable paper model for domestic instruction from prepared paper stock.",
		Action = "making stackable paper model for domestic instruction",
		ActiveCraftItemSdesc = "an in-progress stackable paper model for domestic instruction craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2410.0, Material: "paper", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2410 grams of paper", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrial_science_education_0006_stackable_model", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a stackable paper model for domestic instruction (@industrial_science_education_0006_stackable_model)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ScienceAndEducationMakeStackableOakChartForDomesticInstructionCraftIndustrialScienceEducation0007StackableChart() => new()
	{
		Metadata = new("Science and education/make stackable oak chart for domestic instruction [craft_industrial_science_education_0007_stackable_chart]", "crafts", ["industrial"], ["item:industrial_science_education_0007_stackable_chart"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make stackable oak chart for domestic instruction [craft_industrial_science_education_0007_stackable_chart]",
		Category = "Science and education",
		Blurb = "Make a stackable oak chart for domestic instruction from prepared oak stock.",
		Action = "making stackable oak chart for domestic instruction",
		ActiveCraftItemSdesc = "an in-progress stackable oak chart for domestic instruction craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2451.0, Material: "oak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2451 grams of oak", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrial_science_education_0007_stackable_chart", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a stackable oak chart for domestic instruction (@industrial_science_education_0007_stackable_chart)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ScienceAndEducationMakeStackableMildSteelPointerForDomesticInstructionCraftIndustrialScienceEducation0008StackablePointer() => new()
	{
		Metadata = new("Science and education/make stackable mild steel pointer for domestic instruction [craft_industrial_science_education_0008_stackable_pointer]", "crafts", ["industrial"], ["item:industrial_science_education_0008_stackable_pointer"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make stackable mild steel pointer for domestic instruction [craft_industrial_science_education_0008_stackable_pointer]",
		Category = "Science and education",
		Blurb = "Make a stackable mild steel pointer for domestic instruction from prepared mild steel stock.",
		Action = "making stackable mild steel pointer for domestic instruction",
		ActiveCraftItemSdesc = "an in-progress stackable mild steel pointer for domestic instruction craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2492.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2492 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrial_science_education_0008_stackable_pointer", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a stackable mild steel pointer for domestic instruction (@industrial_science_education_0008_stackable_pointer)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ScienceAndEducationMakeStackableGlassBalanceForDomesticInstructionCraftIndustrialScienceEducation0009StackableBalance() => new()
	{
		Metadata = new("Science and education/make stackable glass balance for domestic instruction [craft_industrial_science_education_0009_stackable_balance]", "crafts", ["industrial"], ["item:industrial_science_education_0009_stackable_balance"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make stackable glass balance for domestic instruction [craft_industrial_science_education_0009_stackable_balance]",
		Category = "Science and education",
		Blurb = "Make a stackable glass balance for domestic instruction from prepared glass stock.",
		Action = "making stackable glass balance for domestic instruction",
		ActiveCraftItemSdesc = "an in-progress stackable glass balance for domestic instruction craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2532.0, Material: "glass", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2532 grams of glass", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrial_science_education_0009_stackable_balance", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a stackable glass balance for domestic instruction (@industrial_science_education_0009_stackable_balance)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition SportsRecreationAndMusicMakePlainLeatherScrapForDomesticRecreationCraftIndustrialisedSportsRecreationMusic0001PlainScrap() => new()
	{
		Metadata = new("Sports, recreation and music/make plain leather scrap for domestic recreation [craft_industrialised_sports_recreation_music_0001_plain_scrap]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_sports_recreation_music_0001_plain_scrap"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make plain leather scrap for domestic recreation [craft_industrialised_sports_recreation_music_0001_plain_scrap]",
		Category = "Sports, recreation and music",
		Blurb = "Make a plain leather scrap for domestic recreation from prepared leather stock.",
		Action = "making plain leather scrap for domestic recreation",
		ActiveCraftItemSdesc = "an in-progress plain leather scrap for domestic recreation craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2318.0, Material: "leather", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2318 grams of leather", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_sports_recreation_music_0001_plain_scrap", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a plain leather scrap for domestic recreation (@industrialised_sports_recreation_music_0001_plain_scrap)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition SportsRecreationAndMusicMakePlainCottonBallForDomesticRecreationCraftIndustrialisedSportsRecreationMusic0002PlainBall() => new()
	{
		Metadata = new("Sports, recreation and music/make plain cotton ball for domestic recreation [craft_industrialised_sports_recreation_music_0002_plain_ball]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_sports_recreation_music_0002_plain_ball"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make plain cotton ball for domestic recreation [craft_industrialised_sports_recreation_music_0002_plain_ball]",
		Category = "Sports, recreation and music",
		Blurb = "Make a plain cotton ball for domestic recreation from prepared cotton stock.",
		Action = "making plain cotton ball for domestic recreation",
		ActiveCraftItemSdesc = "an in-progress plain cotton ball for domestic recreation craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2358.0, Material: "cotton", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2358 grams of cotton", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_sports_recreation_music_0002_plain_ball", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a plain cotton ball for domestic recreation (@industrialised_sports_recreation_music_0002_plain_ball)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition SportsRecreationAndMusicMakePlainMildSteelBatForDomesticRecreationCraftIndustrialisedSportsRecreationMusic0003PlainBat() => new()
	{
		Metadata = new("Sports, recreation and music/make plain mild steel bat for domestic recreation [craft_industrialised_sports_recreation_music_0003_plain_bat]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_sports_recreation_music_0003_plain_bat"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make plain mild steel bat for domestic recreation [craft_industrialised_sports_recreation_music_0003_plain_bat]",
		Category = "Sports, recreation and music",
		Blurb = "Make a plain mild steel bat for domestic recreation from prepared mild steel stock.",
		Action = "making plain mild steel bat for domestic recreation",
		ActiveCraftItemSdesc = "an in-progress plain mild steel bat for domestic recreation craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2399.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2399 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_sports_recreation_music_0003_plain_bat", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a plain mild steel bat for domestic recreation (@industrialised_sports_recreation_music_0003_plain_bat)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition SportsRecreationAndMusicMakePlainOakRacketForDomesticRecreationCraftIndustrialisedSportsRecreationMusic0004PlainRacket() => new()
	{
		Metadata = new("Sports, recreation and music/make plain oak racket for domestic recreation [craft_industrialised_sports_recreation_music_0004_plain_racket]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_sports_recreation_music_0004_plain_racket"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make plain oak racket for domestic recreation [craft_industrialised_sports_recreation_music_0004_plain_racket]",
		Category = "Sports, recreation and music",
		Blurb = "Make a plain oak racket for domestic recreation from prepared oak stock.",
		Action = "making plain oak racket for domestic recreation",
		ActiveCraftItemSdesc = "an in-progress plain oak racket for domestic recreation craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2440.0, Material: "oak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2440 grams of oak", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_sports_recreation_music_0004_plain_racket", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a plain oak racket for domestic recreation (@industrialised_sports_recreation_music_0004_plain_racket)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition SportsRecreationAndMusicMakePlainLeatherClubForDomesticRecreationCraftIndustrialisedSportsRecreationMusic0005PlainClub() => new()
	{
		Metadata = new("Sports, recreation and music/make plain leather club for domestic recreation [craft_industrialised_sports_recreation_music_0005_plain_club]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_sports_recreation_music_0005_plain_club"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make plain leather club for domestic recreation [craft_industrialised_sports_recreation_music_0005_plain_club]",
		Category = "Sports, recreation and music",
		Blurb = "Make a plain leather club for domestic recreation from prepared leather stock.",
		Action = "making plain leather club for domestic recreation",
		ActiveCraftItemSdesc = "an in-progress plain leather club for domestic recreation craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2480.0, Material: "leather", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2480 grams of leather", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_sports_recreation_music_0005_plain_club", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a plain leather club for domestic recreation (@industrialised_sports_recreation_music_0005_plain_club)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition SportsRecreationAndMusicMakePlainCottonDrumForDomesticRecreationCraftIndustrialisedSportsRecreationMusic0006PlainDrum() => new()
	{
		Metadata = new("Sports, recreation and music/make plain cotton drum for domestic recreation [craft_industrialised_sports_recreation_music_0006_plain_drum]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_sports_recreation_music_0006_plain_drum"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make plain cotton drum for domestic recreation [craft_industrialised_sports_recreation_music_0006_plain_drum]",
		Category = "Sports, recreation and music",
		Blurb = "Make a plain cotton drum for domestic recreation from prepared cotton stock.",
		Action = "making plain cotton drum for domestic recreation",
		ActiveCraftItemSdesc = "an in-progress plain cotton drum for domestic recreation craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2521.0, Material: "cotton", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2521 grams of cotton", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_sports_recreation_music_0006_plain_drum", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a plain cotton drum for domestic recreation (@industrialised_sports_recreation_music_0006_plain_drum)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition SportsRecreationAndMusicMakePlainMildSteelFluteForDomesticRecreationCraftIndustrialisedSportsRecreationMusic0007PlainFlute() => new()
	{
		Metadata = new("Sports, recreation and music/make plain mild steel flute for domestic recreation [craft_industrialised_sports_recreation_music_0007_plain_flute]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_sports_recreation_music_0007_plain_flute"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make plain mild steel flute for domestic recreation [craft_industrialised_sports_recreation_music_0007_plain_flute]",
		Category = "Sports, recreation and music",
		Blurb = "Make a plain mild steel flute for domestic recreation from prepared mild steel stock.",
		Action = "making plain mild steel flute for domestic recreation",
		ActiveCraftItemSdesc = "an in-progress plain mild steel flute for domestic recreation craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2562.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2562 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_sports_recreation_music_0007_plain_flute", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a plain mild steel flute for domestic recreation (@industrialised_sports_recreation_music_0007_plain_flute)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition SportsRecreationAndMusicMakePlainOakBoardForDomesticRecreationCraftIndustrialisedSportsRecreationMusic0008PlainBoard() => new()
	{
		Metadata = new("Sports, recreation and music/make plain oak board for domestic recreation [craft_industrialised_sports_recreation_music_0008_plain_board]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_sports_recreation_music_0008_plain_board"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make plain oak board for domestic recreation [craft_industrialised_sports_recreation_music_0008_plain_board]",
		Category = "Sports, recreation and music",
		Blurb = "Make a plain oak board for domestic recreation from prepared oak stock.",
		Action = "making plain oak board for domestic recreation",
		ActiveCraftItemSdesc = "an in-progress plain oak board for domestic recreation craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2603.0, Material: "oak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2603 grams of oak", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_sports_recreation_music_0008_plain_board", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a plain oak board for domestic recreation (@industrialised_sports_recreation_music_0008_plain_board)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition SportsRecreationAndMusicMakePlainLeatherCounterForDomesticRecreationCraftIndustrialisedSportsRecreationMusic0009PlainCounter() => new()
	{
		Metadata = new("Sports, recreation and music/make plain leather counter for domestic recreation [craft_industrialised_sports_recreation_music_0009_plain_counter]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_sports_recreation_music_0009_plain_counter"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make plain leather counter for domestic recreation [craft_industrialised_sports_recreation_music_0009_plain_counter]",
		Category = "Sports, recreation and music",
		Blurb = "Make a plain leather counter for domestic recreation from prepared leather stock.",
		Action = "making plain leather counter for domestic recreation",
		ActiveCraftItemSdesc = "an in-progress plain leather counter for domestic recreation craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2643.0, Material: "leather", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2643 grams of leather", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_sports_recreation_music_0009_plain_counter", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a plain leather counter for domestic recreation (@industrialised_sports_recreation_music_0009_plain_counter)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition SportsRecreationAndMusicMakePlainCottonSkateForDomesticRecreationCraftIndustrialisedSportsRecreationMusic0010PlainSkate() => new()
	{
		Metadata = new("Sports, recreation and music/make plain cotton skate for domestic recreation [craft_industrialised_sports_recreation_music_0010_plain_skate]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_sports_recreation_music_0010_plain_skate"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make plain cotton skate for domestic recreation [craft_industrialised_sports_recreation_music_0010_plain_skate]",
		Category = "Sports, recreation and music",
		Blurb = "Make a plain cotton skate for domestic recreation from prepared cotton stock.",
		Action = "making plain cotton skate for domestic recreation",
		ActiveCraftItemSdesc = "an in-progress plain cotton skate for domestic recreation craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2684.0, Material: "cotton", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2684 grams of cotton", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_sports_recreation_music_0010_plain_skate", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a plain cotton skate for domestic recreation (@industrialised_sports_recreation_music_0010_plain_skate)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition SportsRecreationAndMusicMakeReinforcedMildSteelScrapForDomesticRecreationCraftIndustrialisedSportsRecreationMusic0011ReinforcedScrap() => new()
	{
		Metadata = new("Sports, recreation and music/make reinforced mild steel scrap for domestic recreation [craft_industrialised_sports_recreation_music_0011_reinforced_scrap]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_sports_recreation_music_0011_reinforced_scrap"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make reinforced mild steel scrap for domestic recreation [craft_industrialised_sports_recreation_music_0011_reinforced_scrap]",
		Category = "Sports, recreation and music",
		Blurb = "Make a reinforced mild steel scrap for domestic recreation from prepared mild steel stock.",
		Action = "making reinforced mild steel scrap for domestic recreation",
		ActiveCraftItemSdesc = "an in-progress reinforced mild steel scrap for domestic recreation craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2725.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2725 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_sports_recreation_music_0011_reinforced_scrap", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a reinforced mild steel scrap for domestic recreation (@industrialised_sports_recreation_music_0011_reinforced_scrap)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition SportsRecreationAndMusicMakeReinforcedOakBallForDomesticRecreationCraftIndustrialisedSportsRecreationMusic0012ReinforcedBall() => new()
	{
		Metadata = new("Sports, recreation and music/make reinforced oak ball for domestic recreation [craft_industrialised_sports_recreation_music_0012_reinforced_ball]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_sports_recreation_music_0012_reinforced_ball"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make reinforced oak ball for domestic recreation [craft_industrialised_sports_recreation_music_0012_reinforced_ball]",
		Category = "Sports, recreation and music",
		Blurb = "Make a reinforced oak ball for domestic recreation from prepared oak stock.",
		Action = "making reinforced oak ball for domestic recreation",
		ActiveCraftItemSdesc = "an in-progress reinforced oak ball for domestic recreation craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2765.0, Material: "oak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2765 grams of oak", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_sports_recreation_music_0012_reinforced_ball", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a reinforced oak ball for domestic recreation (@industrialised_sports_recreation_music_0012_reinforced_ball)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition SportsRecreationAndMusicMakeReinforcedLeatherBatForDomesticRecreationCraftIndustrialisedSportsRecreationMusic0013ReinforcedBat() => new()
	{
		Metadata = new("Sports, recreation and music/make reinforced leather bat for domestic recreation [craft_industrialised_sports_recreation_music_0013_reinforced_bat]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_sports_recreation_music_0013_reinforced_bat"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make reinforced leather bat for domestic recreation [craft_industrialised_sports_recreation_music_0013_reinforced_bat]",
		Category = "Sports, recreation and music",
		Blurb = "Make a reinforced leather bat for domestic recreation from prepared leather stock.",
		Action = "making reinforced leather bat for domestic recreation",
		ActiveCraftItemSdesc = "an in-progress reinforced leather bat for domestic recreation craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2806.0, Material: "leather", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2806 grams of leather", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_sports_recreation_music_0013_reinforced_bat", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a reinforced leather bat for domestic recreation (@industrialised_sports_recreation_music_0013_reinforced_bat)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition SportsRecreationAndMusicMakeReinforcedCottonRacketForDomesticRecreationCraftIndustrialisedSportsRecreationMusic0014ReinforcedRacket() => new()
	{
		Metadata = new("Sports, recreation and music/make reinforced cotton racket for domestic recreation [craft_industrialised_sports_recreation_music_0014_reinforced_racket]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_sports_recreation_music_0014_reinforced_racket"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make reinforced cotton racket for domestic recreation [craft_industrialised_sports_recreation_music_0014_reinforced_racket]",
		Category = "Sports, recreation and music",
		Blurb = "Make a reinforced cotton racket for domestic recreation from prepared cotton stock.",
		Action = "making reinforced cotton racket for domestic recreation",
		ActiveCraftItemSdesc = "an in-progress reinforced cotton racket for domestic recreation craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2847.0, Material: "cotton", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2847 grams of cotton", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_sports_recreation_music_0014_reinforced_racket", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a reinforced cotton racket for domestic recreation (@industrialised_sports_recreation_music_0014_reinforced_racket)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};
}
