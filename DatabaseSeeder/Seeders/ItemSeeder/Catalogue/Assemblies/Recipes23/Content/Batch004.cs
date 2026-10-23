#nullable enable

using MudSharp.Body;
using MudSharp.GameItems.Inventory.Plans;
using MudSharp.RPG.Checks;

namespace DatabaseSeeder.Seeders;

public static partial class Recipes23Stock
{
	public static CatalogueRecipeDefinition ReligiousAndInstitutionalMakePlainCottonPlaqueForDomesticObservanceCraftIndustrialisedReligiousInstitutional0007PlainPlaque() => new()
	{
		Metadata = new("Religious and institutional/make plain cotton plaque for domestic observance [craft_industrialised_religious_institutional_0007_plain_plaque]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_religious_institutional_0007_plain_plaque"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make plain cotton plaque for domestic observance [craft_industrialised_religious_institutional_0007_plain_plaque]",
		Category = "Religious and institutional",
		Blurb = "Make a plain cotton plaque for domestic observance from prepared cotton stock.",
		Action = "making plain cotton plaque for domestic observance",
		ActiveCraftItemSdesc = "an in-progress plain cotton plaque for domestic observance craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2895.0, Material: "cotton", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2895 grams of cotton", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_religious_institutional_0007_plain_plaque", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a plain cotton plaque for domestic observance (@industrialised_religious_institutional_0007_plain_plaque)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ReligiousAndInstitutionalMakePlainCeramicBoxForDomesticObservanceCraftIndustrialisedReligiousInstitutional0008PlainBox() => new()
	{
		Metadata = new("Religious and institutional/make plain ceramic box for domestic observance [craft_industrialised_religious_institutional_0008_plain_box]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_religious_institutional_0008_plain_box"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make plain ceramic box for domestic observance [craft_industrialised_religious_institutional_0008_plain_box]",
		Category = "Religious and institutional",
		Blurb = "Make a plain ceramic box for domestic observance from prepared ceramic stock.",
		Action = "making plain ceramic box for domestic observance",
		ActiveCraftItemSdesc = "an in-progress plain ceramic box for domestic observance craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2936.0, Material: "ceramic", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2936 grams of ceramic", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_religious_institutional_0008_plain_box", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a plain ceramic box for domestic observance (@industrialised_religious_institutional_0008_plain_box)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ReligiousAndInstitutionalMakePlainOakBellForDomesticObservanceCraftIndustrialisedReligiousInstitutional0009PlainBell() => new()
	{
		Metadata = new("Religious and institutional/make plain oak bell for domestic observance [craft_industrialised_religious_institutional_0009_plain_bell]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_religious_institutional_0009_plain_bell"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make plain oak bell for domestic observance [craft_industrialised_religious_institutional_0009_plain_bell]",
		Category = "Religious and institutional",
		Blurb = "Make a plain oak bell for domestic observance from prepared oak stock.",
		Action = "making plain oak bell for domestic observance",
		ActiveCraftItemSdesc = "an in-progress plain oak bell for domestic observance craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2977.0, Material: "oak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2977 grams of oak", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_religious_institutional_0009_plain_bell", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a plain oak bell for domestic observance (@industrialised_religious_institutional_0009_plain_bell)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ReligiousAndInstitutionalMakePlainMildSteelStandForDomesticObservanceCraftIndustrialisedReligiousInstitutional0010PlainStand() => new()
	{
		Metadata = new("Religious and institutional/make plain mild steel stand for domestic observance [craft_industrialised_religious_institutional_0010_plain_stand]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_religious_institutional_0010_plain_stand"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make plain mild steel stand for domestic observance [craft_industrialised_religious_institutional_0010_plain_stand]",
		Category = "Religious and institutional",
		Blurb = "Make a plain mild steel stand for domestic observance from prepared mild steel stock.",
		Action = "making plain mild steel stand for domestic observance",
		ActiveCraftItemSdesc = "an in-progress plain mild steel stand for domestic observance craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3017.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3017 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_religious_institutional_0010_plain_stand", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a plain mild steel stand for domestic observance (@industrialised_religious_institutional_0010_plain_stand)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ReligiousAndInstitutionalMakeReinforcedCottonWasteForDomesticObservanceCraftIndustrialisedReligiousInstitutional0011ReinforcedWaste() => new()
	{
		Metadata = new("Religious and institutional/make reinforced cotton waste for domestic observance [craft_industrialised_religious_institutional_0011_reinforced_waste]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_religious_institutional_0011_reinforced_waste"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make reinforced cotton waste for domestic observance [craft_industrialised_religious_institutional_0011_reinforced_waste]",
		Category = "Religious and institutional",
		Blurb = "Make a reinforced cotton waste for domestic observance from prepared cotton stock.",
		Action = "making reinforced cotton waste for domestic observance",
		ActiveCraftItemSdesc = "an in-progress reinforced cotton waste for domestic observance craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3058.0, Material: "cotton", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3058 grams of cotton", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_religious_institutional_0011_reinforced_waste", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a reinforced cotton waste for domestic observance (@industrialised_religious_institutional_0011_reinforced_waste)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ReligiousAndInstitutionalMakeReinforcedCeramicLecternForDomesticObservanceCraftIndustrialisedReligiousInstitutional0012ReinforcedLectern() => new()
	{
		Metadata = new("Religious and institutional/make reinforced ceramic lectern for domestic observance [craft_industrialised_religious_institutional_0012_reinforced_lectern]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_religious_institutional_0012_reinforced_lectern"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make reinforced ceramic lectern for domestic observance [craft_industrialised_religious_institutional_0012_reinforced_lectern]",
		Category = "Religious and institutional",
		Blurb = "Make a reinforced ceramic lectern for domestic observance from prepared ceramic stock.",
		Action = "making reinforced ceramic lectern for domestic observance",
		ActiveCraftItemSdesc = "an in-progress reinforced ceramic lectern for domestic observance craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3099.0, Material: "ceramic", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3099 grams of ceramic", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_religious_institutional_0012_reinforced_lectern", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a reinforced ceramic lectern for domestic observance (@industrialised_religious_institutional_0012_reinforced_lectern)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ReligiousAndInstitutionalMakeReinforcedOakKneelerForDomesticObservanceCraftIndustrialisedReligiousInstitutional0013ReinforcedKneeler() => new()
	{
		Metadata = new("Religious and institutional/make reinforced oak kneeler for domestic observance [craft_industrialised_religious_institutional_0013_reinforced_kneeler]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_religious_institutional_0013_reinforced_kneeler"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make reinforced oak kneeler for domestic observance [craft_industrialised_religious_institutional_0013_reinforced_kneeler]",
		Category = "Religious and institutional",
		Blurb = "Make a reinforced oak kneeler for domestic observance from prepared oak stock.",
		Action = "making reinforced oak kneeler for domestic observance",
		ActiveCraftItemSdesc = "an in-progress reinforced oak kneeler for domestic observance craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3139.0, Material: "oak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3139 grams of oak", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_religious_institutional_0013_reinforced_kneeler", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a reinforced oak kneeler for domestic observance (@industrialised_religious_institutional_0013_reinforced_kneeler)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ReligiousAndInstitutionalMakeReinforcedMildSteelVesselForDomesticObservanceCraftIndustrialisedReligiousInstitutional0014ReinforcedVessel() => new()
	{
		Metadata = new("Religious and institutional/make reinforced mild steel vessel for domestic observance [craft_industrialised_religious_institutional_0014_reinforced_vessel]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_religious_institutional_0014_reinforced_vessel"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make reinforced mild steel vessel for domestic observance [craft_industrialised_religious_institutional_0014_reinforced_vessel]",
		Category = "Religious and institutional",
		Blurb = "Make a reinforced mild steel vessel for domestic observance from prepared mild steel stock.",
		Action = "making reinforced mild steel vessel for domestic observance",
		ActiveCraftItemSdesc = "an in-progress reinforced mild steel vessel for domestic observance craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3180.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3180 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_religious_institutional_0014_reinforced_vessel", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a reinforced mild steel vessel for domestic observance (@industrialised_religious_institutional_0014_reinforced_vessel)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ReligiousAndInstitutionalMakeReinforcedCottonScreenForDomesticObservanceCraftIndustrialisedReligiousInstitutional0015ReinforcedScreen() => new()
	{
		Metadata = new("Religious and institutional/make reinforced cotton screen for domestic observance [craft_industrialised_religious_institutional_0015_reinforced_screen]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_religious_institutional_0015_reinforced_screen"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make reinforced cotton screen for domestic observance [craft_industrialised_religious_institutional_0015_reinforced_screen]",
		Category = "Religious and institutional",
		Blurb = "Make a reinforced cotton screen for domestic observance from prepared cotton stock.",
		Action = "making reinforced cotton screen for domestic observance",
		ActiveCraftItemSdesc = "an in-progress reinforced cotton screen for domestic observance craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3221.0, Material: "cotton", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3221 grams of cotton", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_religious_institutional_0015_reinforced_screen", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a reinforced cotton screen for domestic observance (@industrialised_religious_institutional_0015_reinforced_screen)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ReligiousAndInstitutionalMakeReinforcedCeramicCandlestickForDomesticObservanceCraftIndustrialisedReligiousInstitutional0016ReinforcedCandlestick() => new()
	{
		Metadata = new("Religious and institutional/make reinforced ceramic candlestick for domestic observance [craft_industrialised_religious_institutional_0016_reinforced_candlestick]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_religious_institutional_0016_reinforced_candlestick"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make reinforced ceramic candlestick for domestic observance [craft_industrialised_religious_institutional_0016_reinforced_candlestick]",
		Category = "Religious and institutional",
		Blurb = "Make a reinforced ceramic candlestick for domestic observance from prepared ceramic stock.",
		Action = "making reinforced ceramic candlestick for domestic observance",
		ActiveCraftItemSdesc = "an in-progress reinforced ceramic candlestick for domestic observance craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3262.0, Material: "ceramic", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3262 grams of ceramic", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_religious_institutional_0016_reinforced_candlestick", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a reinforced ceramic candlestick for domestic observance (@industrialised_religious_institutional_0016_reinforced_candlestick)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ReligiousAndInstitutionalMakeReinforcedOakPlaqueForDomesticObservanceCraftIndustrialisedReligiousInstitutional0017ReinforcedPlaque() => new()
	{
		Metadata = new("Religious and institutional/make reinforced oak plaque for domestic observance [craft_industrialised_religious_institutional_0017_reinforced_plaque]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_religious_institutional_0017_reinforced_plaque"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make reinforced oak plaque for domestic observance [craft_industrialised_religious_institutional_0017_reinforced_plaque]",
		Category = "Religious and institutional",
		Blurb = "Make a reinforced oak plaque for domestic observance from prepared oak stock.",
		Action = "making reinforced oak plaque for domestic observance",
		ActiveCraftItemSdesc = "an in-progress reinforced oak plaque for domestic observance craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3302.0, Material: "oak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3302 grams of oak", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_religious_institutional_0017_reinforced_plaque", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a reinforced oak plaque for domestic observance (@industrialised_religious_institutional_0017_reinforced_plaque)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ReligiousAndInstitutionalMakeReinforcedMildSteelBoxForDomesticObservanceCraftIndustrialisedReligiousInstitutional0018ReinforcedBox() => new()
	{
		Metadata = new("Religious and institutional/make reinforced mild steel box for domestic observance [craft_industrialised_religious_institutional_0018_reinforced_box]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_religious_institutional_0018_reinforced_box"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make reinforced mild steel box for domestic observance [craft_industrialised_religious_institutional_0018_reinforced_box]",
		Category = "Religious and institutional",
		Blurb = "Make a reinforced mild steel box for domestic observance from prepared mild steel stock.",
		Action = "making reinforced mild steel box for domestic observance",
		ActiveCraftItemSdesc = "an in-progress reinforced mild steel box for domestic observance craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3343.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3343 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_religious_institutional_0018_reinforced_box", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a reinforced mild steel box for domestic observance (@industrialised_religious_institutional_0018_reinforced_box)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ReligiousAndInstitutionalMakeReinforcedCottonBellForDomesticObservanceCraftIndustrialisedReligiousInstitutional0019ReinforcedBell() => new()
	{
		Metadata = new("Religious and institutional/make reinforced cotton bell for domestic observance [craft_industrialised_religious_institutional_0019_reinforced_bell]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_religious_institutional_0019_reinforced_bell"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make reinforced cotton bell for domestic observance [craft_industrialised_religious_institutional_0019_reinforced_bell]",
		Category = "Religious and institutional",
		Blurb = "Make a reinforced cotton bell for domestic observance from prepared cotton stock.",
		Action = "making reinforced cotton bell for domestic observance",
		ActiveCraftItemSdesc = "an in-progress reinforced cotton bell for domestic observance craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3384.0, Material: "cotton", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3384 grams of cotton", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_religious_institutional_0019_reinforced_bell", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a reinforced cotton bell for domestic observance (@industrialised_religious_institutional_0019_reinforced_bell)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ReligiousAndInstitutionalMakeReinforcedCeramicStandForDomesticObservanceCraftIndustrialisedReligiousInstitutional0020ReinforcedStand() => new()
	{
		Metadata = new("Religious and institutional/make reinforced ceramic stand for domestic observance [craft_industrialised_religious_institutional_0020_reinforced_stand]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_religious_institutional_0020_reinforced_stand"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make reinforced ceramic stand for domestic observance [craft_industrialised_religious_institutional_0020_reinforced_stand]",
		Category = "Religious and institutional",
		Blurb = "Make a reinforced ceramic stand for domestic observance from prepared ceramic stock.",
		Action = "making reinforced ceramic stand for domestic observance",
		ActiveCraftItemSdesc = "an in-progress reinforced ceramic stand for domestic observance craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3424.0, Material: "ceramic", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3424 grams of ceramic", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_religious_institutional_0020_reinforced_stand", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a reinforced ceramic stand for domestic observance (@industrialised_religious_institutional_0020_reinforced_stand)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ReligiousAndInstitutionalMakeCompactOakWasteForDomesticObservanceCraftIndustrialisedReligiousInstitutional0021CompactWaste() => new()
	{
		Metadata = new("Religious and institutional/make compact oak waste for domestic observance [craft_industrialised_religious_institutional_0021_compact_waste]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_religious_institutional_0021_compact_waste"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make compact oak waste for domestic observance [craft_industrialised_religious_institutional_0021_compact_waste]",
		Category = "Religious and institutional",
		Blurb = "Make a compact oak waste for domestic observance from prepared oak stock.",
		Action = "making compact oak waste for domestic observance",
		ActiveCraftItemSdesc = "an in-progress compact oak waste for domestic observance craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3465.0, Material: "oak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3465 grams of oak", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_religious_institutional_0021_compact_waste", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a compact oak waste for domestic observance (@industrialised_religious_institutional_0021_compact_waste)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ReligiousAndInstitutionalMakeCompactMildSteelLecternForDomesticObservanceCraftIndustrialisedReligiousInstitutional0022CompactLectern() => new()
	{
		Metadata = new("Religious and institutional/make compact mild steel lectern for domestic observance [craft_industrialised_religious_institutional_0022_compact_lectern]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_religious_institutional_0022_compact_lectern"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make compact mild steel lectern for domestic observance [craft_industrialised_religious_institutional_0022_compact_lectern]",
		Category = "Religious and institutional",
		Blurb = "Make a compact mild steel lectern for domestic observance from prepared mild steel stock.",
		Action = "making compact mild steel lectern for domestic observance",
		ActiveCraftItemSdesc = "an in-progress compact mild steel lectern for domestic observance craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3506.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3506 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_religious_institutional_0022_compact_lectern", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a compact mild steel lectern for domestic observance (@industrialised_religious_institutional_0022_compact_lectern)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ReligiousAndInstitutionalMakeCompactCottonKneelerForDomesticObservanceCraftIndustrialisedReligiousInstitutional0023CompactKneeler() => new()
	{
		Metadata = new("Religious and institutional/make compact cotton kneeler for domestic observance [craft_industrialised_religious_institutional_0023_compact_kneeler]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_religious_institutional_0023_compact_kneeler"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make compact cotton kneeler for domestic observance [craft_industrialised_religious_institutional_0023_compact_kneeler]",
		Category = "Religious and institutional",
		Blurb = "Make a compact cotton kneeler for domestic observance from prepared cotton stock.",
		Action = "making compact cotton kneeler for domestic observance",
		ActiveCraftItemSdesc = "an in-progress compact cotton kneeler for domestic observance craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3546.0, Material: "cotton", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3546 grams of cotton", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_religious_institutional_0023_compact_kneeler", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a compact cotton kneeler for domestic observance (@industrialised_religious_institutional_0023_compact_kneeler)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ReligiousAndInstitutionalMakeCompactCeramicVesselForDomesticObservanceCraftIndustrialisedReligiousInstitutional0024CompactVessel() => new()
	{
		Metadata = new("Religious and institutional/make compact ceramic vessel for domestic observance [craft_industrialised_religious_institutional_0024_compact_vessel]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_religious_institutional_0024_compact_vessel"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make compact ceramic vessel for domestic observance [craft_industrialised_religious_institutional_0024_compact_vessel]",
		Category = "Religious and institutional",
		Blurb = "Make a compact ceramic vessel for domestic observance from prepared ceramic stock.",
		Action = "making compact ceramic vessel for domestic observance",
		ActiveCraftItemSdesc = "an in-progress compact ceramic vessel for domestic observance craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3587.0, Material: "ceramic", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3587 grams of ceramic", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_religious_institutional_0024_compact_vessel", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a compact ceramic vessel for domestic observance (@industrialised_religious_institutional_0024_compact_vessel)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ReligiousAndInstitutionalMakeCompactOakScreenForDomesticObservanceCraftIndustrialisedReligiousInstitutional0025CompactScreen() => new()
	{
		Metadata = new("Religious and institutional/make compact oak screen for domestic observance [craft_industrialised_religious_institutional_0025_compact_screen]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_religious_institutional_0025_compact_screen"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make compact oak screen for domestic observance [craft_industrialised_religious_institutional_0025_compact_screen]",
		Category = "Religious and institutional",
		Blurb = "Make a compact oak screen for domestic observance from prepared oak stock.",
		Action = "making compact oak screen for domestic observance",
		ActiveCraftItemSdesc = "an in-progress compact oak screen for domestic observance craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3628.0, Material: "oak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3628 grams of oak", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_religious_institutional_0025_compact_screen", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a compact oak screen for domestic observance (@industrialised_religious_institutional_0025_compact_screen)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ReligiousAndInstitutionalMakeCompactMildSteelCandlestickForDomesticObservanceCraftIndustrialisedReligiousInstitutional0026CompactCandlestick() => new()
	{
		Metadata = new("Religious and institutional/make compact mild steel candlestick for domestic observance [craft_industrialised_religious_institutional_0026_compact_candlestick]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_religious_institutional_0026_compact_candlestick"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make compact mild steel candlestick for domestic observance [craft_industrialised_religious_institutional_0026_compact_candlestick]",
		Category = "Religious and institutional",
		Blurb = "Make a compact mild steel candlestick for domestic observance from prepared mild steel stock.",
		Action = "making compact mild steel candlestick for domestic observance",
		ActiveCraftItemSdesc = "an in-progress compact mild steel candlestick for domestic observance craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3669.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3669 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_religious_institutional_0026_compact_candlestick", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a compact mild steel candlestick for domestic observance (@industrialised_religious_institutional_0026_compact_candlestick)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ReligiousAndInstitutionalMakeCompactCottonPlaqueForDomesticObservanceCraftIndustrialisedReligiousInstitutional0027CompactPlaque() => new()
	{
		Metadata = new("Religious and institutional/make compact cotton plaque for domestic observance [craft_industrialised_religious_institutional_0027_compact_plaque]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_religious_institutional_0027_compact_plaque"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make compact cotton plaque for domestic observance [craft_industrialised_religious_institutional_0027_compact_plaque]",
		Category = "Religious and institutional",
		Blurb = "Make a compact cotton plaque for domestic observance from prepared cotton stock.",
		Action = "making compact cotton plaque for domestic observance",
		ActiveCraftItemSdesc = "an in-progress compact cotton plaque for domestic observance craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3709.0, Material: "cotton", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3709 grams of cotton", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_religious_institutional_0027_compact_plaque", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a compact cotton plaque for domestic observance (@industrialised_religious_institutional_0027_compact_plaque)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ReligiousAndInstitutionalMakeCompactCeramicBoxForDomesticObservanceCraftIndustrialisedReligiousInstitutional0028CompactBox() => new()
	{
		Metadata = new("Religious and institutional/make compact ceramic box for domestic observance [craft_industrialised_religious_institutional_0028_compact_box]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_religious_institutional_0028_compact_box"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make compact ceramic box for domestic observance [craft_industrialised_religious_institutional_0028_compact_box]",
		Category = "Religious and institutional",
		Blurb = "Make a compact ceramic box for domestic observance from prepared ceramic stock.",
		Action = "making compact ceramic box for domestic observance",
		ActiveCraftItemSdesc = "an in-progress compact ceramic box for domestic observance craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3750.0, Material: "ceramic", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3750 grams of ceramic", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_religious_institutional_0028_compact_box", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a compact ceramic box for domestic observance (@industrialised_religious_institutional_0028_compact_box)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ReligiousAndInstitutionalMakeCompactOakBellForDomesticObservanceCraftIndustrialisedReligiousInstitutional0029CompactBell() => new()
	{
		Metadata = new("Religious and institutional/make compact oak bell for domestic observance [craft_industrialised_religious_institutional_0029_compact_bell]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_religious_institutional_0029_compact_bell"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make compact oak bell for domestic observance [craft_industrialised_religious_institutional_0029_compact_bell]",
		Category = "Religious and institutional",
		Blurb = "Make a compact oak bell for domestic observance from prepared oak stock.",
		Action = "making compact oak bell for domestic observance",
		ActiveCraftItemSdesc = "an in-progress compact oak bell for domestic observance craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3791.0, Material: "oak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3791 grams of oak", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_religious_institutional_0029_compact_bell", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a compact oak bell for domestic observance (@industrialised_religious_institutional_0029_compact_bell)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ReligiousAndInstitutionalMakeWeatherproofOakWasteForDomesticObservanceCraftIndustrialReligiousInstitutional0001WeatherproofWaste() => new()
	{
		Metadata = new("Religious and institutional/make weatherproof oak waste for domestic observance [craft_industrial_religious_institutional_0001_weatherproof_waste]", "crafts", ["industrial"], ["item:industrial_religious_institutional_0001_weatherproof_waste"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make weatherproof oak waste for domestic observance [craft_industrial_religious_institutional_0001_weatherproof_waste]",
		Category = "Religious and institutional",
		Blurb = "Make a weatherproof oak waste for domestic observance from prepared oak stock.",
		Action = "making weatherproof oak waste for domestic observance",
		ActiveCraftItemSdesc = "an in-progress weatherproof oak waste for domestic observance craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2651.0, Material: "oak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2651 grams of oak", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrial_religious_institutional_0001_weatherproof_waste", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a weatherproof oak waste for domestic observance (@industrial_religious_institutional_0001_weatherproof_waste)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ReligiousAndInstitutionalMakeWeatherproofMildSteelLecternForDomesticObservanceCraftIndustrialReligiousInstitutional0002WeatherproofLectern() => new()
	{
		Metadata = new("Religious and institutional/make weatherproof mild steel lectern for domestic observance [craft_industrial_religious_institutional_0002_weatherproof_lectern]", "crafts", ["industrial"], ["item:industrial_religious_institutional_0002_weatherproof_lectern"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make weatherproof mild steel lectern for domestic observance [craft_industrial_religious_institutional_0002_weatherproof_lectern]",
		Category = "Religious and institutional",
		Blurb = "Make a weatherproof mild steel lectern for domestic observance from prepared mild steel stock.",
		Action = "making weatherproof mild steel lectern for domestic observance",
		ActiveCraftItemSdesc = "an in-progress weatherproof mild steel lectern for domestic observance craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2692.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2692 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrial_religious_institutional_0002_weatherproof_lectern", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a weatherproof mild steel lectern for domestic observance (@industrial_religious_institutional_0002_weatherproof_lectern)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PrintedMediaAndSignageMakePlainCottonWastepaperForDomesticPublicDisplayCraftIndustrialisedPrintedMediaSignage0001PlainWastepaper() => new()
	{
		Metadata = new("Printed media and signage/make plain cotton wastepaper for domestic public display [craft_industrialised_printed_media_signage_0001_plain_wastepaper]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_printed_media_signage_0001_plain_wastepaper"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make plain cotton wastepaper for domestic public display [craft_industrialised_printed_media_signage_0001_plain_wastepaper]",
		Category = "Printed media and signage",
		Blurb = "Make a plain cotton wastepaper for domestic public display from prepared cotton stock.",
		Action = "making plain cotton wastepaper for domestic public display",
		ActiveCraftItemSdesc = "an in-progress plain cotton wastepaper for domestic public display craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2762.0, Material: "cotton", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2762 grams of cotton", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_printed_media_signage_0001_plain_wastepaper", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a plain cotton wastepaper for domestic public display (@industrialised_printed_media_signage_0001_plain_wastepaper)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PrintedMediaAndSignageMakePlainOakPosterForDomesticPublicDisplayCraftIndustrialisedPrintedMediaSignage0002PlainPoster() => new()
	{
		Metadata = new("Printed media and signage/make plain oak poster for domestic public display [craft_industrialised_printed_media_signage_0002_plain_poster]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_printed_media_signage_0002_plain_poster"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make plain oak poster for domestic public display [craft_industrialised_printed_media_signage_0002_plain_poster]",
		Category = "Printed media and signage",
		Blurb = "Make a plain oak poster for domestic public display from prepared oak stock.",
		Action = "making plain oak poster for domestic public display",
		ActiveCraftItemSdesc = "an in-progress plain oak poster for domestic public display craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2803.0, Material: "oak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2803 grams of oak", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_printed_media_signage_0002_plain_poster", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a plain oak poster for domestic public display (@industrialised_printed_media_signage_0002_plain_poster)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PrintedMediaAndSignageMakePlainPaperNoticeForDomesticPublicDisplayCraftIndustrialisedPrintedMediaSignage0003PlainNotice() => new()
	{
		Metadata = new("Printed media and signage/make plain paper notice for domestic public display [craft_industrialised_printed_media_signage_0003_plain_notice]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_printed_media_signage_0003_plain_notice"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make plain paper notice for domestic public display [craft_industrialised_printed_media_signage_0003_plain_notice]",
		Category = "Printed media and signage",
		Blurb = "Make a plain paper notice for domestic public display from prepared paper stock.",
		Action = "making plain paper notice for domestic public display",
		ActiveCraftItemSdesc = "an in-progress plain paper notice for domestic public display craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2844.0, Material: "paper", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2844 grams of paper", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_printed_media_signage_0003_plain_notice", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a plain paper notice for domestic public display (@industrialised_printed_media_signage_0003_plain_notice)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PrintedMediaAndSignageMakePlainCottonPlacardForDomesticPublicDisplayCraftIndustrialisedPrintedMediaSignage0004PlainPlacard() => new()
	{
		Metadata = new("Printed media and signage/make plain cotton placard for domestic public display [craft_industrialised_printed_media_signage_0004_plain_placard]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_printed_media_signage_0004_plain_placard"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make plain cotton placard for domestic public display [craft_industrialised_printed_media_signage_0004_plain_placard]",
		Category = "Printed media and signage",
		Blurb = "Make a plain cotton placard for domestic public display from prepared cotton stock.",
		Action = "making plain cotton placard for domestic public display",
		ActiveCraftItemSdesc = "an in-progress plain cotton placard for domestic public display craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2884.0, Material: "cotton", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2884 grams of cotton", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_printed_media_signage_0004_plain_placard", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a plain cotton placard for domestic public display (@industrialised_printed_media_signage_0004_plain_placard)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PrintedMediaAndSignageMakePlainOakMapForDomesticPublicDisplayCraftIndustrialisedPrintedMediaSignage0005PlainMap() => new()
	{
		Metadata = new("Printed media and signage/make plain oak map for domestic public display [craft_industrialised_printed_media_signage_0005_plain_map]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_printed_media_signage_0005_plain_map"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make plain oak map for domestic public display [craft_industrialised_printed_media_signage_0005_plain_map]",
		Category = "Printed media and signage",
		Blurb = "Make a plain oak map for domestic public display from prepared oak stock.",
		Action = "making plain oak map for domestic public display",
		ActiveCraftItemSdesc = "an in-progress plain oak map for domestic public display craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2925.0, Material: "oak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2925 grams of oak", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_printed_media_signage_0005_plain_map", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a plain oak map for domestic public display (@industrialised_printed_media_signage_0005_plain_map)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PrintedMediaAndSignageMakePlainPaperManualForDomesticPublicDisplayCraftIndustrialisedPrintedMediaSignage0006PlainManual() => new()
	{
		Metadata = new("Printed media and signage/make plain paper manual for domestic public display [craft_industrialised_printed_media_signage_0006_plain_manual]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_printed_media_signage_0006_plain_manual"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make plain paper manual for domestic public display [craft_industrialised_printed_media_signage_0006_plain_manual]",
		Category = "Printed media and signage",
		Blurb = "Make a plain paper manual for domestic public display from prepared paper stock.",
		Action = "making plain paper manual for domestic public display",
		ActiveCraftItemSdesc = "an in-progress plain paper manual for domestic public display craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2966.0, Material: "paper", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2966 grams of paper", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_printed_media_signage_0006_plain_manual", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a plain paper manual for domestic public display (@industrialised_printed_media_signage_0006_plain_manual)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PrintedMediaAndSignageMakePlainCottonPamphletForDomesticPublicDisplayCraftIndustrialisedPrintedMediaSignage0007PlainPamphlet() => new()
	{
		Metadata = new("Printed media and signage/make plain cotton pamphlet for domestic public display [craft_industrialised_printed_media_signage_0007_plain_pamphlet]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_printed_media_signage_0007_plain_pamphlet"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make plain cotton pamphlet for domestic public display [craft_industrialised_printed_media_signage_0007_plain_pamphlet]",
		Category = "Printed media and signage",
		Blurb = "Make a plain cotton pamphlet for domestic public display from prepared cotton stock.",
		Action = "making plain cotton pamphlet for domestic public display",
		ActiveCraftItemSdesc = "an in-progress plain cotton pamphlet for domestic public display craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3006.0, Material: "cotton", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3006 grams of cotton", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_printed_media_signage_0007_plain_pamphlet", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a plain cotton pamphlet for domestic public display (@industrialised_printed_media_signage_0007_plain_pamphlet)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PrintedMediaAndSignageMakePlainOakTicketForDomesticPublicDisplayCraftIndustrialisedPrintedMediaSignage0008PlainTicket() => new()
	{
		Metadata = new("Printed media and signage/make plain oak ticket for domestic public display [craft_industrialised_printed_media_signage_0008_plain_ticket]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_printed_media_signage_0008_plain_ticket"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make plain oak ticket for domestic public display [craft_industrialised_printed_media_signage_0008_plain_ticket]",
		Category = "Printed media and signage",
		Blurb = "Make a plain oak ticket for domestic public display from prepared oak stock.",
		Action = "making plain oak ticket for domestic public display",
		ActiveCraftItemSdesc = "an in-progress plain oak ticket for domestic public display craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3047.0, Material: "oak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3047 grams of oak", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_printed_media_signage_0008_plain_ticket", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a plain oak ticket for domestic public display (@industrialised_printed_media_signage_0008_plain_ticket)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PrintedMediaAndSignageMakePlainPaperLabelForDomesticPublicDisplayCraftIndustrialisedPrintedMediaSignage0009PlainLabel() => new()
	{
		Metadata = new("Printed media and signage/make plain paper label for domestic public display [craft_industrialised_printed_media_signage_0009_plain_label]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_printed_media_signage_0009_plain_label"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make plain paper label for domestic public display [craft_industrialised_printed_media_signage_0009_plain_label]",
		Category = "Printed media and signage",
		Blurb = "Make a plain paper label for domestic public display from prepared paper stock.",
		Action = "making plain paper label for domestic public display",
		ActiveCraftItemSdesc = "an in-progress plain paper label for domestic public display craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3088.0, Material: "paper", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3088 grams of paper", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_printed_media_signage_0009_plain_label", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a plain paper label for domestic public display (@industrialised_printed_media_signage_0009_plain_label)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PrintedMediaAndSignageMakePlainCottonChartForDomesticPublicDisplayCraftIndustrialisedPrintedMediaSignage0010PlainChart() => new()
	{
		Metadata = new("Printed media and signage/make plain cotton chart for domestic public display [craft_industrialised_printed_media_signage_0010_plain_chart]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_printed_media_signage_0010_plain_chart"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make plain cotton chart for domestic public display [craft_industrialised_printed_media_signage_0010_plain_chart]",
		Category = "Printed media and signage",
		Blurb = "Make a plain cotton chart for domestic public display from prepared cotton stock.",
		Action = "making plain cotton chart for domestic public display",
		ActiveCraftItemSdesc = "an in-progress plain cotton chart for domestic public display craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3128.0, Material: "cotton", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3128 grams of cotton", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_printed_media_signage_0010_plain_chart", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a plain cotton chart for domestic public display (@industrialised_printed_media_signage_0010_plain_chart)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PrintedMediaAndSignageMakeReinforcedOakWastepaperForDomesticPublicDisplayCraftIndustrialisedPrintedMediaSignage0011ReinforcedWastepaper() => new()
	{
		Metadata = new("Printed media and signage/make reinforced oak wastepaper for domestic public display [craft_industrialised_printed_media_signage_0011_reinforced_wastepaper]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_printed_media_signage_0011_reinforced_wastepaper"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make reinforced oak wastepaper for domestic public display [craft_industrialised_printed_media_signage_0011_reinforced_wastepaper]",
		Category = "Printed media and signage",
		Blurb = "Make a reinforced oak wastepaper for domestic public display from prepared oak stock.",
		Action = "making reinforced oak wastepaper for domestic public display",
		ActiveCraftItemSdesc = "an in-progress reinforced oak wastepaper for domestic public display craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3169.0, Material: "oak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3169 grams of oak", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_printed_media_signage_0011_reinforced_wastepaper", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a reinforced oak wastepaper for domestic public display (@industrialised_printed_media_signage_0011_reinforced_wastepaper)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PrintedMediaAndSignageMakeReinforcedPaperPosterForDomesticPublicDisplayCraftIndustrialisedPrintedMediaSignage0012ReinforcedPoster() => new()
	{
		Metadata = new("Printed media and signage/make reinforced paper poster for domestic public display [craft_industrialised_printed_media_signage_0012_reinforced_poster]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_printed_media_signage_0012_reinforced_poster"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make reinforced paper poster for domestic public display [craft_industrialised_printed_media_signage_0012_reinforced_poster]",
		Category = "Printed media and signage",
		Blurb = "Make a reinforced paper poster for domestic public display from prepared paper stock.",
		Action = "making reinforced paper poster for domestic public display",
		ActiveCraftItemSdesc = "an in-progress reinforced paper poster for domestic public display craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3210.0, Material: "paper", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3210 grams of paper", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_printed_media_signage_0012_reinforced_poster", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a reinforced paper poster for domestic public display (@industrialised_printed_media_signage_0012_reinforced_poster)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PrintedMediaAndSignageMakeReinforcedCottonNoticeForDomesticPublicDisplayCraftIndustrialisedPrintedMediaSignage0013ReinforcedNotice() => new()
	{
		Metadata = new("Printed media and signage/make reinforced cotton notice for domestic public display [craft_industrialised_printed_media_signage_0013_reinforced_notice]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_printed_media_signage_0013_reinforced_notice"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make reinforced cotton notice for domestic public display [craft_industrialised_printed_media_signage_0013_reinforced_notice]",
		Category = "Printed media and signage",
		Blurb = "Make a reinforced cotton notice for domestic public display from prepared cotton stock.",
		Action = "making reinforced cotton notice for domestic public display",
		ActiveCraftItemSdesc = "an in-progress reinforced cotton notice for domestic public display craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3251.0, Material: "cotton", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3251 grams of cotton", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_printed_media_signage_0013_reinforced_notice", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a reinforced cotton notice for domestic public display (@industrialised_printed_media_signage_0013_reinforced_notice)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PrintedMediaAndSignageMakeReinforcedOakPlacardForDomesticPublicDisplayCraftIndustrialisedPrintedMediaSignage0014ReinforcedPlacard() => new()
	{
		Metadata = new("Printed media and signage/make reinforced oak placard for domestic public display [craft_industrialised_printed_media_signage_0014_reinforced_placard]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_printed_media_signage_0014_reinforced_placard"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make reinforced oak placard for domestic public display [craft_industrialised_printed_media_signage_0014_reinforced_placard]",
		Category = "Printed media and signage",
		Blurb = "Make a reinforced oak placard for domestic public display from prepared oak stock.",
		Action = "making reinforced oak placard for domestic public display",
		ActiveCraftItemSdesc = "an in-progress reinforced oak placard for domestic public display craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3291.0, Material: "oak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3291 grams of oak", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_printed_media_signage_0014_reinforced_placard", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a reinforced oak placard for domestic public display (@industrialised_printed_media_signage_0014_reinforced_placard)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PrintedMediaAndSignageMakeReinforcedPaperMapForDomesticPublicDisplayCraftIndustrialisedPrintedMediaSignage0015ReinforcedMap() => new()
	{
		Metadata = new("Printed media and signage/make reinforced paper map for domestic public display [craft_industrialised_printed_media_signage_0015_reinforced_map]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_printed_media_signage_0015_reinforced_map"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make reinforced paper map for domestic public display [craft_industrialised_printed_media_signage_0015_reinforced_map]",
		Category = "Printed media and signage",
		Blurb = "Make a reinforced paper map for domestic public display from prepared paper stock.",
		Action = "making reinforced paper map for domestic public display",
		ActiveCraftItemSdesc = "an in-progress reinforced paper map for domestic public display craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3332.0, Material: "paper", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3332 grams of paper", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_printed_media_signage_0015_reinforced_map", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a reinforced paper map for domestic public display (@industrialised_printed_media_signage_0015_reinforced_map)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PrintedMediaAndSignageMakeReinforcedCottonManualForDomesticPublicDisplayCraftIndustrialisedPrintedMediaSignage0016ReinforcedManual() => new()
	{
		Metadata = new("Printed media and signage/make reinforced cotton manual for domestic public display [craft_industrialised_printed_media_signage_0016_reinforced_manual]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_printed_media_signage_0016_reinforced_manual"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make reinforced cotton manual for domestic public display [craft_industrialised_printed_media_signage_0016_reinforced_manual]",
		Category = "Printed media and signage",
		Blurb = "Make a reinforced cotton manual for domestic public display from prepared cotton stock.",
		Action = "making reinforced cotton manual for domestic public display",
		ActiveCraftItemSdesc = "an in-progress reinforced cotton manual for domestic public display craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3373.0, Material: "cotton", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3373 grams of cotton", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_printed_media_signage_0016_reinforced_manual", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a reinforced cotton manual for domestic public display (@industrialised_printed_media_signage_0016_reinforced_manual)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PrintedMediaAndSignageMakeReinforcedOakPamphletForDomesticPublicDisplayCraftIndustrialisedPrintedMediaSignage0017ReinforcedPamphlet() => new()
	{
		Metadata = new("Printed media and signage/make reinforced oak pamphlet for domestic public display [craft_industrialised_printed_media_signage_0017_reinforced_pamphlet]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_printed_media_signage_0017_reinforced_pamphlet"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make reinforced oak pamphlet for domestic public display [craft_industrialised_printed_media_signage_0017_reinforced_pamphlet]",
		Category = "Printed media and signage",
		Blurb = "Make a reinforced oak pamphlet for domestic public display from prepared oak stock.",
		Action = "making reinforced oak pamphlet for domestic public display",
		ActiveCraftItemSdesc = "an in-progress reinforced oak pamphlet for domestic public display craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3413.0, Material: "oak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3413 grams of oak", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_printed_media_signage_0017_reinforced_pamphlet", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a reinforced oak pamphlet for domestic public display (@industrialised_printed_media_signage_0017_reinforced_pamphlet)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PrintedMediaAndSignageMakeReinforcedPaperTicketForDomesticPublicDisplayCraftIndustrialisedPrintedMediaSignage0018ReinforcedTicket() => new()
	{
		Metadata = new("Printed media and signage/make reinforced paper ticket for domestic public display [craft_industrialised_printed_media_signage_0018_reinforced_ticket]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_printed_media_signage_0018_reinforced_ticket"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make reinforced paper ticket for domestic public display [craft_industrialised_printed_media_signage_0018_reinforced_ticket]",
		Category = "Printed media and signage",
		Blurb = "Make a reinforced paper ticket for domestic public display from prepared paper stock.",
		Action = "making reinforced paper ticket for domestic public display",
		ActiveCraftItemSdesc = "an in-progress reinforced paper ticket for domestic public display craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3454.0, Material: "paper", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3454 grams of paper", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_printed_media_signage_0018_reinforced_ticket", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a reinforced paper ticket for domestic public display (@industrialised_printed_media_signage_0018_reinforced_ticket)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PrintedMediaAndSignageMakeReinforcedCottonLabelForDomesticPublicDisplayCraftIndustrialisedPrintedMediaSignage0019ReinforcedLabel() => new()
	{
		Metadata = new("Printed media and signage/make reinforced cotton label for domestic public display [craft_industrialised_printed_media_signage_0019_reinforced_label]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_printed_media_signage_0019_reinforced_label"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make reinforced cotton label for domestic public display [craft_industrialised_printed_media_signage_0019_reinforced_label]",
		Category = "Printed media and signage",
		Blurb = "Make a reinforced cotton label for domestic public display from prepared cotton stock.",
		Action = "making reinforced cotton label for domestic public display",
		ActiveCraftItemSdesc = "an in-progress reinforced cotton label for domestic public display craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3495.0, Material: "cotton", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3495 grams of cotton", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_printed_media_signage_0019_reinforced_label", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a reinforced cotton label for domestic public display (@industrialised_printed_media_signage_0019_reinforced_label)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PrintedMediaAndSignageMakeReinforcedOakChartForDomesticPublicDisplayCraftIndustrialisedPrintedMediaSignage0020ReinforcedChart() => new()
	{
		Metadata = new("Printed media and signage/make reinforced oak chart for domestic public display [craft_industrialised_printed_media_signage_0020_reinforced_chart]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_printed_media_signage_0020_reinforced_chart"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make reinforced oak chart for domestic public display [craft_industrialised_printed_media_signage_0020_reinforced_chart]",
		Category = "Printed media and signage",
		Blurb = "Make a reinforced oak chart for domestic public display from prepared oak stock.",
		Action = "making reinforced oak chart for domestic public display",
		ActiveCraftItemSdesc = "an in-progress reinforced oak chart for domestic public display craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3535.0, Material: "oak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3535 grams of oak", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_printed_media_signage_0020_reinforced_chart", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a reinforced oak chart for domestic public display (@industrialised_printed_media_signage_0020_reinforced_chart)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PrintedMediaAndSignageMakeCompactPaperWastepaperForDomesticPublicDisplayCraftIndustrialisedPrintedMediaSignage0021CompactWastepaper() => new()
	{
		Metadata = new("Printed media and signage/make compact paper wastepaper for domestic public display [craft_industrialised_printed_media_signage_0021_compact_wastepaper]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_printed_media_signage_0021_compact_wastepaper"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make compact paper wastepaper for domestic public display [craft_industrialised_printed_media_signage_0021_compact_wastepaper]",
		Category = "Printed media and signage",
		Blurb = "Make a compact paper wastepaper for domestic public display from prepared paper stock.",
		Action = "making compact paper wastepaper for domestic public display",
		ActiveCraftItemSdesc = "an in-progress compact paper wastepaper for domestic public display craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3576.0, Material: "paper", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3576 grams of paper", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_printed_media_signage_0021_compact_wastepaper", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a compact paper wastepaper for domestic public display (@industrialised_printed_media_signage_0021_compact_wastepaper)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PrintedMediaAndSignageMakeCompactCottonPosterForDomesticPublicDisplayCraftIndustrialisedPrintedMediaSignage0022CompactPoster() => new()
	{
		Metadata = new("Printed media and signage/make compact cotton poster for domestic public display [craft_industrialised_printed_media_signage_0022_compact_poster]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_printed_media_signage_0022_compact_poster"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make compact cotton poster for domestic public display [craft_industrialised_printed_media_signage_0022_compact_poster]",
		Category = "Printed media and signage",
		Blurb = "Make a compact cotton poster for domestic public display from prepared cotton stock.",
		Action = "making compact cotton poster for domestic public display",
		ActiveCraftItemSdesc = "an in-progress compact cotton poster for domestic public display craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3617.0, Material: "cotton", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3617 grams of cotton", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_printed_media_signage_0022_compact_poster", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a compact cotton poster for domestic public display (@industrialised_printed_media_signage_0022_compact_poster)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PrintedMediaAndSignageMakeCompactOakNoticeForDomesticPublicDisplayCraftIndustrialisedPrintedMediaSignage0023CompactNotice() => new()
	{
		Metadata = new("Printed media and signage/make compact oak notice for domestic public display [craft_industrialised_printed_media_signage_0023_compact_notice]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_printed_media_signage_0023_compact_notice"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make compact oak notice for domestic public display [craft_industrialised_printed_media_signage_0023_compact_notice]",
		Category = "Printed media and signage",
		Blurb = "Make a compact oak notice for domestic public display from prepared oak stock.",
		Action = "making compact oak notice for domestic public display",
		ActiveCraftItemSdesc = "an in-progress compact oak notice for domestic public display craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3658.0, Material: "oak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3658 grams of oak", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_printed_media_signage_0023_compact_notice", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a compact oak notice for domestic public display (@industrialised_printed_media_signage_0023_compact_notice)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PrintedMediaAndSignageMakeCompactPaperPlacardForDomesticPublicDisplayCraftIndustrialisedPrintedMediaSignage0024CompactPlacard() => new()
	{
		Metadata = new("Printed media and signage/make compact paper placard for domestic public display [craft_industrialised_printed_media_signage_0024_compact_placard]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_printed_media_signage_0024_compact_placard"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make compact paper placard for domestic public display [craft_industrialised_printed_media_signage_0024_compact_placard]",
		Category = "Printed media and signage",
		Blurb = "Make a compact paper placard for domestic public display from prepared paper stock.",
		Action = "making compact paper placard for domestic public display",
		ActiveCraftItemSdesc = "an in-progress compact paper placard for domestic public display craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3698.0, Material: "paper", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3698 grams of paper", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_printed_media_signage_0024_compact_placard", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a compact paper placard for domestic public display (@industrialised_printed_media_signage_0024_compact_placard)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PrintedMediaAndSignageMakeCompactCottonMapForDomesticPublicDisplayCraftIndustrialisedPrintedMediaSignage0025CompactMap() => new()
	{
		Metadata = new("Printed media and signage/make compact cotton map for domestic public display [craft_industrialised_printed_media_signage_0025_compact_map]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_printed_media_signage_0025_compact_map"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make compact cotton map for domestic public display [craft_industrialised_printed_media_signage_0025_compact_map]",
		Category = "Printed media and signage",
		Blurb = "Make a compact cotton map for domestic public display from prepared cotton stock.",
		Action = "making compact cotton map for domestic public display",
		ActiveCraftItemSdesc = "an in-progress compact cotton map for domestic public display craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3739.0, Material: "cotton", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3739 grams of cotton", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_printed_media_signage_0025_compact_map", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a compact cotton map for domestic public display (@industrialised_printed_media_signage_0025_compact_map)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PrintedMediaAndSignageMakeCompactOakManualForDomesticPublicDisplayCraftIndustrialisedPrintedMediaSignage0026CompactManual() => new()
	{
		Metadata = new("Printed media and signage/make compact oak manual for domestic public display [craft_industrialised_printed_media_signage_0026_compact_manual]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_printed_media_signage_0026_compact_manual"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make compact oak manual for domestic public display [craft_industrialised_printed_media_signage_0026_compact_manual]",
		Category = "Printed media and signage",
		Blurb = "Make a compact oak manual for domestic public display from prepared oak stock.",
		Action = "making compact oak manual for domestic public display",
		ActiveCraftItemSdesc = "an in-progress compact oak manual for domestic public display craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3780.0, Material: "oak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3780 grams of oak", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_printed_media_signage_0026_compact_manual", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a compact oak manual for domestic public display (@industrialised_printed_media_signage_0026_compact_manual)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PrintedMediaAndSignageMakeCompactPaperPamphletForDomesticPublicDisplayCraftIndustrialisedPrintedMediaSignage0027CompactPamphlet() => new()
	{
		Metadata = new("Printed media and signage/make compact paper pamphlet for domestic public display [craft_industrialised_printed_media_signage_0027_compact_pamphlet]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_printed_media_signage_0027_compact_pamphlet"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make compact paper pamphlet for domestic public display [craft_industrialised_printed_media_signage_0027_compact_pamphlet]",
		Category = "Printed media and signage",
		Blurb = "Make a compact paper pamphlet for domestic public display from prepared paper stock.",
		Action = "making compact paper pamphlet for domestic public display",
		ActiveCraftItemSdesc = "an in-progress compact paper pamphlet for domestic public display craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3820.0, Material: "paper", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3820 grams of paper", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_printed_media_signage_0027_compact_pamphlet", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a compact paper pamphlet for domestic public display (@industrialised_printed_media_signage_0027_compact_pamphlet)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PrintedMediaAndSignageMakeCompactCottonTicketForDomesticPublicDisplayCraftIndustrialisedPrintedMediaSignage0028CompactTicket() => new()
	{
		Metadata = new("Printed media and signage/make compact cotton ticket for domestic public display [craft_industrialised_printed_media_signage_0028_compact_ticket]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_printed_media_signage_0028_compact_ticket"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make compact cotton ticket for domestic public display [craft_industrialised_printed_media_signage_0028_compact_ticket]",
		Category = "Printed media and signage",
		Blurb = "Make a compact cotton ticket for domestic public display from prepared cotton stock.",
		Action = "making compact cotton ticket for domestic public display",
		ActiveCraftItemSdesc = "an in-progress compact cotton ticket for domestic public display craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3861.0, Material: "cotton", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3861 grams of cotton", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_printed_media_signage_0028_compact_ticket", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a compact cotton ticket for domestic public display (@industrialised_printed_media_signage_0028_compact_ticket)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PrintedMediaAndSignageMakeCompactOakLabelForDomesticPublicDisplayCraftIndustrialisedPrintedMediaSignage0029CompactLabel() => new()
	{
		Metadata = new("Printed media and signage/make compact oak label for domestic public display [craft_industrialised_printed_media_signage_0029_compact_label]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_printed_media_signage_0029_compact_label"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make compact oak label for domestic public display [craft_industrialised_printed_media_signage_0029_compact_label]",
		Category = "Printed media and signage",
		Blurb = "Make a compact oak label for domestic public display from prepared oak stock.",
		Action = "making compact oak label for domestic public display",
		ActiveCraftItemSdesc = "an in-progress compact oak label for domestic public display craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3902.0, Material: "oak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3902 grams of oak", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_printed_media_signage_0029_compact_label", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a compact oak label for domestic public display (@industrialised_printed_media_signage_0029_compact_label)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PrintedMediaAndSignageMakeCompactPaperChartForDomesticPublicDisplayCraftIndustrialisedPrintedMediaSignage0030CompactChart() => new()
	{
		Metadata = new("Printed media and signage/make compact paper chart for domestic public display [craft_industrialised_printed_media_signage_0030_compact_chart]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_printed_media_signage_0030_compact_chart"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make compact paper chart for domestic public display [craft_industrialised_printed_media_signage_0030_compact_chart]",
		Category = "Printed media and signage",
		Blurb = "Make a compact paper chart for domestic public display from prepared paper stock.",
		Action = "making compact paper chart for domestic public display",
		ActiveCraftItemSdesc = "an in-progress compact paper chart for domestic public display craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3942.0, Material: "paper", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3942 grams of paper", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_printed_media_signage_0030_compact_chart", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a compact paper chart for domestic public display (@industrialised_printed_media_signage_0030_compact_chart)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PrintedMediaAndSignageMakeHeavyDutyCottonWastepaperForDomesticPublicDisplayCraftIndustrialisedPrintedMediaSignage0031HeavyDutyWastepaper() => new()
	{
		Metadata = new("Printed media and signage/make heavy-duty cotton wastepaper for domestic public display [craft_industrialised_printed_media_signage_0031_heavy_duty_wastepaper]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_printed_media_signage_0031_heavy_duty_wastepaper"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make heavy-duty cotton wastepaper for domestic public display [craft_industrialised_printed_media_signage_0031_heavy_duty_wastepaper]",
		Category = "Printed media and signage",
		Blurb = "Make a heavy-duty cotton wastepaper for domestic public display from prepared cotton stock.",
		Action = "making heavy-duty cotton wastepaper for domestic public display",
		ActiveCraftItemSdesc = "an in-progress heavy-duty cotton wastepaper for domestic public display craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3983.0, Material: "cotton", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3983 grams of cotton", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_printed_media_signage_0031_heavy_duty_wastepaper", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a heavy-duty cotton wastepaper for domestic public display (@industrialised_printed_media_signage_0031_heavy_duty_wastepaper)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PrintedMediaAndSignageMakeHeavyDutyOakPosterForDomesticPublicDisplayCraftIndustrialisedPrintedMediaSignage0032HeavyDutyPoster() => new()
	{
		Metadata = new("Printed media and signage/make heavy-duty oak poster for domestic public display [craft_industrialised_printed_media_signage_0032_heavy_duty_poster]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_printed_media_signage_0032_heavy_duty_poster"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make heavy-duty oak poster for domestic public display [craft_industrialised_printed_media_signage_0032_heavy_duty_poster]",
		Category = "Printed media and signage",
		Blurb = "Make a heavy-duty oak poster for domestic public display from prepared oak stock.",
		Action = "making heavy-duty oak poster for domestic public display",
		ActiveCraftItemSdesc = "an in-progress heavy-duty oak poster for domestic public display craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 4024.0, Material: "oak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("4024 grams of oak", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_printed_media_signage_0032_heavy_duty_poster", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a heavy-duty oak poster for domestic public display (@industrialised_printed_media_signage_0032_heavy_duty_poster)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PrintedMediaAndSignageMakeHeavyDutyPaperNoticeForDomesticPublicDisplayCraftIndustrialisedPrintedMediaSignage0033HeavyDutyNotice() => new()
	{
		Metadata = new("Printed media and signage/make heavy-duty paper notice for domestic public display [craft_industrialised_printed_media_signage_0033_heavy_duty_notice]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_printed_media_signage_0033_heavy_duty_notice"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make heavy-duty paper notice for domestic public display [craft_industrialised_printed_media_signage_0033_heavy_duty_notice]",
		Category = "Printed media and signage",
		Blurb = "Make a heavy-duty paper notice for domestic public display from prepared paper stock.",
		Action = "making heavy-duty paper notice for domestic public display",
		ActiveCraftItemSdesc = "an in-progress heavy-duty paper notice for domestic public display craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 4065.0, Material: "paper", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("4065 grams of paper", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_printed_media_signage_0033_heavy_duty_notice", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a heavy-duty paper notice for domestic public display (@industrialised_printed_media_signage_0033_heavy_duty_notice)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PrintedMediaAndSignageMakeHeavyDutyCottonPlacardForDomesticPublicDisplayCraftIndustrialisedPrintedMediaSignage0034HeavyDutyPlacard() => new()
	{
		Metadata = new("Printed media and signage/make heavy-duty cotton placard for domestic public display [craft_industrialised_printed_media_signage_0034_heavy_duty_placard]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_printed_media_signage_0034_heavy_duty_placard"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make heavy-duty cotton placard for domestic public display [craft_industrialised_printed_media_signage_0034_heavy_duty_placard]",
		Category = "Printed media and signage",
		Blurb = "Make a heavy-duty cotton placard for domestic public display from prepared cotton stock.",
		Action = "making heavy-duty cotton placard for domestic public display",
		ActiveCraftItemSdesc = "an in-progress heavy-duty cotton placard for domestic public display craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 4105.0, Material: "cotton", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("4105 grams of cotton", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_printed_media_signage_0034_heavy_duty_placard", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a heavy-duty cotton placard for domestic public display (@industrialised_printed_media_signage_0034_heavy_duty_placard)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PrintedMediaAndSignageMakeHeavyDutyOakMapForDomesticPublicDisplayCraftIndustrialisedPrintedMediaSignage0035HeavyDutyMap() => new()
	{
		Metadata = new("Printed media and signage/make heavy-duty oak map for domestic public display [craft_industrialised_printed_media_signage_0035_heavy_duty_map]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_printed_media_signage_0035_heavy_duty_map"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make heavy-duty oak map for domestic public display [craft_industrialised_printed_media_signage_0035_heavy_duty_map]",
		Category = "Printed media and signage",
		Blurb = "Make a heavy-duty oak map for domestic public display from prepared oak stock.",
		Action = "making heavy-duty oak map for domestic public display",
		ActiveCraftItemSdesc = "an in-progress heavy-duty oak map for domestic public display craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 4146.0, Material: "oak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("4146 grams of oak", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_printed_media_signage_0035_heavy_duty_map", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a heavy-duty oak map for domestic public display (@industrialised_printed_media_signage_0035_heavy_duty_map)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};
}
