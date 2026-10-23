#nullable enable

using MudSharp.Body;
using MudSharp.GameItems.Inventory.Plans;
using MudSharp.RPG.Checks;

namespace DatabaseSeeder.Seeders;

public static partial class Recipes22Stock
{
	public static CatalogueRecipeDefinition ConstructionAndUtilitiesMakeHeavyDutyCeramicPipeForDomesticBuildingWorkCraftIndustrialisedConstructionUtilities0034HeavyDutyPipe() => new()
	{
		Metadata = new("Construction and utilities/make heavy-duty ceramic pipe for domestic building work [craft_industrialised_construction_utilities_0034_heavy_duty_pipe]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_construction_utilities_0034_heavy_duty_pipe"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make heavy-duty ceramic pipe for domestic building work [craft_industrialised_construction_utilities_0034_heavy_duty_pipe]",
		Category = "Construction and utilities",
		Blurb = "Make a heavy-duty ceramic pipe for domestic building work from prepared ceramic stock.",
		Action = "making heavy-duty ceramic pipe for domestic building work",
		ActiveCraftItemSdesc = "an in-progress heavy-duty ceramic pipe for domestic building work craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2883.0, Material: "ceramic", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2883 grams of ceramic", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_construction_utilities_0034_heavy_duty_pipe", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a heavy-duty ceramic pipe for domestic building work (@industrialised_construction_utilities_0034_heavy_duty_pipe)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ConstructionAndUtilitiesMakeHeavyDutyMildSteelValveForDomesticBuildingWorkCraftIndustrialisedConstructionUtilities0035HeavyDutyValve() => new()
	{
		Metadata = new("Construction and utilities/make heavy-duty mild steel valve for domestic building work [craft_industrialised_construction_utilities_0035_heavy_duty_valve]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_construction_utilities_0035_heavy_duty_valve"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make heavy-duty mild steel valve for domestic building work [craft_industrialised_construction_utilities_0035_heavy_duty_valve]",
		Category = "Construction and utilities",
		Blurb = "Make a heavy-duty mild steel valve for domestic building work from prepared mild steel stock.",
		Action = "making heavy-duty mild steel valve for domestic building work",
		ActiveCraftItemSdesc = "an in-progress heavy-duty mild steel valve for domestic building work craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2924.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2924 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_construction_utilities_0035_heavy_duty_valve", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a heavy-duty mild steel valve for domestic building work (@industrialised_construction_utilities_0035_heavy_duty_valve)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ConstructionAndUtilitiesMakeHeavyDutyCastIronFittingForDomesticBuildingWorkCraftIndustrialisedConstructionUtilities0036HeavyDutyFitting() => new()
	{
		Metadata = new("Construction and utilities/make heavy-duty cast iron fitting for domestic building work [craft_industrialised_construction_utilities_0036_heavy_duty_fitting]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_construction_utilities_0036_heavy_duty_fitting"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make heavy-duty cast iron fitting for domestic building work [craft_industrialised_construction_utilities_0036_heavy_duty_fitting]",
		Category = "Construction and utilities",
		Blurb = "Make a heavy-duty cast iron fitting for domestic building work from prepared cast iron stock.",
		Action = "making heavy-duty cast iron fitting for domestic building work",
		ActiveCraftItemSdesc = "an in-progress heavy-duty cast iron fitting for domestic building work craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2965.0, Material: "cast iron", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2965 grams of cast iron", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_construction_utilities_0036_heavy_duty_fitting", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a heavy-duty cast iron fitting for domestic building work (@industrialised_construction_utilities_0036_heavy_duty_fitting)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ConstructionAndUtilitiesMakeHeavyDutyOakBeamForDomesticBuildingWorkCraftIndustrialisedConstructionUtilities0037HeavyDutyBeam() => new()
	{
		Metadata = new("Construction and utilities/make heavy-duty oak beam for domestic building work [craft_industrialised_construction_utilities_0037_heavy_duty_beam]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_construction_utilities_0037_heavy_duty_beam"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make heavy-duty oak beam for domestic building work [craft_industrialised_construction_utilities_0037_heavy_duty_beam]",
		Category = "Construction and utilities",
		Blurb = "Make a heavy-duty oak beam for domestic building work from prepared oak stock.",
		Action = "making heavy-duty oak beam for domestic building work",
		ActiveCraftItemSdesc = "an in-progress heavy-duty oak beam for domestic building work craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3005.0, Material: "oak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3005 grams of oak", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_construction_utilities_0037_heavy_duty_beam", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a heavy-duty oak beam for domestic building work (@industrialised_construction_utilities_0037_heavy_duty_beam)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ConstructionAndUtilitiesMakeHeavyDutyCeramicPanelForDomesticBuildingWorkCraftIndustrialisedConstructionUtilities0038HeavyDutyPanel() => new()
	{
		Metadata = new("Construction and utilities/make heavy-duty ceramic panel for domestic building work [craft_industrialised_construction_utilities_0038_heavy_duty_panel]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_construction_utilities_0038_heavy_duty_panel"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make heavy-duty ceramic panel for domestic building work [craft_industrialised_construction_utilities_0038_heavy_duty_panel]",
		Category = "Construction and utilities",
		Blurb = "Make a heavy-duty ceramic panel for domestic building work from prepared ceramic stock.",
		Action = "making heavy-duty ceramic panel for domestic building work",
		ActiveCraftItemSdesc = "an in-progress heavy-duty ceramic panel for domestic building work craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3046.0, Material: "ceramic", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3046 grams of ceramic", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_construction_utilities_0038_heavy_duty_panel", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a heavy-duty ceramic panel for domestic building work (@industrialised_construction_utilities_0038_heavy_duty_panel)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ConstructionAndUtilitiesMakeHeavyDutyMildSteelGrateForDomesticBuildingWorkCraftIndustrialisedConstructionUtilities0039HeavyDutyGrate() => new()
	{
		Metadata = new("Construction and utilities/make heavy-duty mild steel grate for domestic building work [craft_industrialised_construction_utilities_0039_heavy_duty_grate]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_construction_utilities_0039_heavy_duty_grate"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make heavy-duty mild steel grate for domestic building work [craft_industrialised_construction_utilities_0039_heavy_duty_grate]",
		Category = "Construction and utilities",
		Blurb = "Make a heavy-duty mild steel grate for domestic building work from prepared mild steel stock.",
		Action = "making heavy-duty mild steel grate for domestic building work",
		ActiveCraftItemSdesc = "an in-progress heavy-duty mild steel grate for domestic building work craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3087.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3087 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_construction_utilities_0039_heavy_duty_grate", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a heavy-duty mild steel grate for domestic building work (@industrialised_construction_utilities_0039_heavy_duty_grate)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ConstructionAndUtilitiesMakeHeavyDutyCastIronConduitForDomesticBuildingWorkCraftIndustrialisedConstructionUtilities0040HeavyDutyConduit() => new()
	{
		Metadata = new("Construction and utilities/make heavy-duty cast iron conduit for domestic building work [craft_industrialised_construction_utilities_0040_heavy_duty_conduit]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_construction_utilities_0040_heavy_duty_conduit"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make heavy-duty cast iron conduit for domestic building work [craft_industrialised_construction_utilities_0040_heavy_duty_conduit]",
		Category = "Construction and utilities",
		Blurb = "Make a heavy-duty cast iron conduit for domestic building work from prepared cast iron stock.",
		Action = "making heavy-duty cast iron conduit for domestic building work",
		ActiveCraftItemSdesc = "an in-progress heavy-duty cast iron conduit for domestic building work craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3127.0, Material: "cast iron", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3127 grams of cast iron", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_construction_utilities_0040_heavy_duty_conduit", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a heavy-duty cast iron conduit for domestic building work (@industrialised_construction_utilities_0040_heavy_duty_conduit)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ConstructionAndUtilitiesMakeFoldingOakRubbleForDomesticBuildingWorkCraftIndustrialisedConstructionUtilities0041FoldingRubble() => new()
	{
		Metadata = new("Construction and utilities/make folding oak rubble for domestic building work [craft_industrialised_construction_utilities_0041_folding_rubble]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_construction_utilities_0041_folding_rubble"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make folding oak rubble for domestic building work [craft_industrialised_construction_utilities_0041_folding_rubble]",
		Category = "Construction and utilities",
		Blurb = "Make a folding oak rubble for domestic building work from prepared oak stock.",
		Action = "making folding oak rubble for domestic building work",
		ActiveCraftItemSdesc = "an in-progress folding oak rubble for domestic building work craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3168.0, Material: "oak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3168 grams of oak", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_construction_utilities_0041_folding_rubble", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a folding oak rubble for domestic building work (@industrialised_construction_utilities_0041_folding_rubble)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ConstructionAndUtilitiesMakeFoldingCeramicBrickForDomesticBuildingWorkCraftIndustrialisedConstructionUtilities0042FoldingBrick() => new()
	{
		Metadata = new("Construction and utilities/make folding ceramic brick for domestic building work [craft_industrialised_construction_utilities_0042_folding_brick]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_construction_utilities_0042_folding_brick"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make folding ceramic brick for domestic building work [craft_industrialised_construction_utilities_0042_folding_brick]",
		Category = "Construction and utilities",
		Blurb = "Make a folding ceramic brick for domestic building work from prepared ceramic stock.",
		Action = "making folding ceramic brick for domestic building work",
		ActiveCraftItemSdesc = "an in-progress folding ceramic brick for domestic building work craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3209.0, Material: "ceramic", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3209 grams of ceramic", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_construction_utilities_0042_folding_brick", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a folding ceramic brick for domestic building work (@industrialised_construction_utilities_0042_folding_brick)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ConstructionAndUtilitiesMakeFoldingMildSteelTileForDomesticBuildingWorkCraftIndustrialisedConstructionUtilities0043FoldingTile() => new()
	{
		Metadata = new("Construction and utilities/make folding mild steel tile for domestic building work [craft_industrialised_construction_utilities_0043_folding_tile]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_construction_utilities_0043_folding_tile"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make folding mild steel tile for domestic building work [craft_industrialised_construction_utilities_0043_folding_tile]",
		Category = "Construction and utilities",
		Blurb = "Make a folding mild steel tile for domestic building work from prepared mild steel stock.",
		Action = "making folding mild steel tile for domestic building work",
		ActiveCraftItemSdesc = "an in-progress folding mild steel tile for domestic building work craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3249.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3249 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_construction_utilities_0043_folding_tile", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a folding mild steel tile for domestic building work (@industrialised_construction_utilities_0043_folding_tile)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ConstructionAndUtilitiesMakeFoldingCastIronPipeForDomesticBuildingWorkCraftIndustrialisedConstructionUtilities0044FoldingPipe() => new()
	{
		Metadata = new("Construction and utilities/make folding cast iron pipe for domestic building work [craft_industrialised_construction_utilities_0044_folding_pipe]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_construction_utilities_0044_folding_pipe"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make folding cast iron pipe for domestic building work [craft_industrialised_construction_utilities_0044_folding_pipe]",
		Category = "Construction and utilities",
		Blurb = "Make a folding cast iron pipe for domestic building work from prepared cast iron stock.",
		Action = "making folding cast iron pipe for domestic building work",
		ActiveCraftItemSdesc = "an in-progress folding cast iron pipe for domestic building work craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3290.0, Material: "cast iron", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3290 grams of cast iron", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_construction_utilities_0044_folding_pipe", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a folding cast iron pipe for domestic building work (@industrialised_construction_utilities_0044_folding_pipe)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ConstructionAndUtilitiesMakeFoldingOakValveForDomesticBuildingWorkCraftIndustrialisedConstructionUtilities0045FoldingValve() => new()
	{
		Metadata = new("Construction and utilities/make folding oak valve for domestic building work [craft_industrialised_construction_utilities_0045_folding_valve]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_construction_utilities_0045_folding_valve"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make folding oak valve for domestic building work [craft_industrialised_construction_utilities_0045_folding_valve]",
		Category = "Construction and utilities",
		Blurb = "Make a folding oak valve for domestic building work from prepared oak stock.",
		Action = "making folding oak valve for domestic building work",
		ActiveCraftItemSdesc = "an in-progress folding oak valve for domestic building work craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3331.0, Material: "oak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3331 grams of oak", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_construction_utilities_0045_folding_valve", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a folding oak valve for domestic building work (@industrialised_construction_utilities_0045_folding_valve)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ConstructionAndUtilitiesMakeFoldingCeramicFittingForDomesticBuildingWorkCraftIndustrialisedConstructionUtilities0046FoldingFitting() => new()
	{
		Metadata = new("Construction and utilities/make folding ceramic fitting for domestic building work [craft_industrialised_construction_utilities_0046_folding_fitting]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_construction_utilities_0046_folding_fitting"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make folding ceramic fitting for domestic building work [craft_industrialised_construction_utilities_0046_folding_fitting]",
		Category = "Construction and utilities",
		Blurb = "Make a folding ceramic fitting for domestic building work from prepared ceramic stock.",
		Action = "making folding ceramic fitting for domestic building work",
		ActiveCraftItemSdesc = "an in-progress folding ceramic fitting for domestic building work craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3372.0, Material: "ceramic", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3372 grams of ceramic", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_construction_utilities_0046_folding_fitting", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a folding ceramic fitting for domestic building work (@industrialised_construction_utilities_0046_folding_fitting)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ConstructionAndUtilitiesMakeFoldingMildSteelBeamForDomesticBuildingWorkCraftIndustrialisedConstructionUtilities0047FoldingBeam() => new()
	{
		Metadata = new("Construction and utilities/make folding mild steel beam for domestic building work [craft_industrialised_construction_utilities_0047_folding_beam]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_construction_utilities_0047_folding_beam"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make folding mild steel beam for domestic building work [craft_industrialised_construction_utilities_0047_folding_beam]",
		Category = "Construction and utilities",
		Blurb = "Make a folding mild steel beam for domestic building work from prepared mild steel stock.",
		Action = "making folding mild steel beam for domestic building work",
		ActiveCraftItemSdesc = "an in-progress folding mild steel beam for domestic building work craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3412.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3412 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_construction_utilities_0047_folding_beam", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a folding mild steel beam for domestic building work (@industrialised_construction_utilities_0047_folding_beam)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ConstructionAndUtilitiesMakeFoldingCastIronPanelForDomesticBuildingWorkCraftIndustrialisedConstructionUtilities0048FoldingPanel() => new()
	{
		Metadata = new("Construction and utilities/make folding cast iron panel for domestic building work [craft_industrialised_construction_utilities_0048_folding_panel]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_construction_utilities_0048_folding_panel"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make folding cast iron panel for domestic building work [craft_industrialised_construction_utilities_0048_folding_panel]",
		Category = "Construction and utilities",
		Blurb = "Make a folding cast iron panel for domestic building work from prepared cast iron stock.",
		Action = "making folding cast iron panel for domestic building work",
		ActiveCraftItemSdesc = "an in-progress folding cast iron panel for domestic building work craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3453.0, Material: "cast iron", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3453 grams of cast iron", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_construction_utilities_0048_folding_panel", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a folding cast iron panel for domestic building work (@industrialised_construction_utilities_0048_folding_panel)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ConstructionAndUtilitiesMakeFoldingOakGrateForDomesticBuildingWorkCraftIndustrialisedConstructionUtilities0049FoldingGrate() => new()
	{
		Metadata = new("Construction and utilities/make folding oak grate for domestic building work [craft_industrialised_construction_utilities_0049_folding_grate]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_construction_utilities_0049_folding_grate"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make folding oak grate for domestic building work [craft_industrialised_construction_utilities_0049_folding_grate]",
		Category = "Construction and utilities",
		Blurb = "Make a folding oak grate for domestic building work from prepared oak stock.",
		Action = "making folding oak grate for domestic building work",
		ActiveCraftItemSdesc = "an in-progress folding oak grate for domestic building work craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3494.0, Material: "oak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3494 grams of oak", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_construction_utilities_0049_folding_grate", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a folding oak grate for domestic building work (@industrialised_construction_utilities_0049_folding_grate)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ConstructionAndUtilitiesMakeFoldingCeramicConduitForDomesticBuildingWorkCraftIndustrialisedConstructionUtilities0050FoldingConduit() => new()
	{
		Metadata = new("Construction and utilities/make folding ceramic conduit for domestic building work [craft_industrialised_construction_utilities_0050_folding_conduit]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_construction_utilities_0050_folding_conduit"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make folding ceramic conduit for domestic building work [craft_industrialised_construction_utilities_0050_folding_conduit]",
		Category = "Construction and utilities",
		Blurb = "Make a folding ceramic conduit for domestic building work from prepared ceramic stock.",
		Action = "making folding ceramic conduit for domestic building work",
		ActiveCraftItemSdesc = "an in-progress folding ceramic conduit for domestic building work craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3534.0, Material: "ceramic", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3534 grams of ceramic", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_construction_utilities_0050_folding_conduit", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a folding ceramic conduit for domestic building work (@industrialised_construction_utilities_0050_folding_conduit)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ConstructionAndUtilitiesMakePortableMildSteelRubbleForDomesticBuildingWorkCraftIndustrialisedConstructionUtilities0051PortableRubble() => new()
	{
		Metadata = new("Construction and utilities/make portable mild steel rubble for domestic building work [craft_industrialised_construction_utilities_0051_portable_rubble]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_construction_utilities_0051_portable_rubble"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make portable mild steel rubble for domestic building work [craft_industrialised_construction_utilities_0051_portable_rubble]",
		Category = "Construction and utilities",
		Blurb = "Make a portable mild steel rubble for domestic building work from prepared mild steel stock.",
		Action = "making portable mild steel rubble for domestic building work",
		ActiveCraftItemSdesc = "an in-progress portable mild steel rubble for domestic building work craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3575.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3575 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_construction_utilities_0051_portable_rubble", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a portable mild steel rubble for domestic building work (@industrialised_construction_utilities_0051_portable_rubble)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ConstructionAndUtilitiesMakePortableCastIronBrickForDomesticBuildingWorkCraftIndustrialisedConstructionUtilities0052PortableBrick() => new()
	{
		Metadata = new("Construction and utilities/make portable cast iron brick for domestic building work [craft_industrialised_construction_utilities_0052_portable_brick]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_construction_utilities_0052_portable_brick"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make portable cast iron brick for domestic building work [craft_industrialised_construction_utilities_0052_portable_brick]",
		Category = "Construction and utilities",
		Blurb = "Make a portable cast iron brick for domestic building work from prepared cast iron stock.",
		Action = "making portable cast iron brick for domestic building work",
		ActiveCraftItemSdesc = "an in-progress portable cast iron brick for domestic building work craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3616.0, Material: "cast iron", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3616 grams of cast iron", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_construction_utilities_0052_portable_brick", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a portable cast iron brick for domestic building work (@industrialised_construction_utilities_0052_portable_brick)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ConstructionAndUtilitiesMakePortableOakTileForDomesticBuildingWorkCraftIndustrialisedConstructionUtilities0053PortableTile() => new()
	{
		Metadata = new("Construction and utilities/make portable oak tile for domestic building work [craft_industrialised_construction_utilities_0053_portable_tile]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_construction_utilities_0053_portable_tile"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make portable oak tile for domestic building work [craft_industrialised_construction_utilities_0053_portable_tile]",
		Category = "Construction and utilities",
		Blurb = "Make a portable oak tile for domestic building work from prepared oak stock.",
		Action = "making portable oak tile for domestic building work",
		ActiveCraftItemSdesc = "an in-progress portable oak tile for domestic building work craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3656.0, Material: "oak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3656 grams of oak", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_construction_utilities_0053_portable_tile", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a portable oak tile for domestic building work (@industrialised_construction_utilities_0053_portable_tile)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ConstructionAndUtilitiesMakePortableCeramicPipeForDomesticBuildingWorkCraftIndustrialisedConstructionUtilities0054PortablePipe() => new()
	{
		Metadata = new("Construction and utilities/make portable ceramic pipe for domestic building work [craft_industrialised_construction_utilities_0054_portable_pipe]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_construction_utilities_0054_portable_pipe"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make portable ceramic pipe for domestic building work [craft_industrialised_construction_utilities_0054_portable_pipe]",
		Category = "Construction and utilities",
		Blurb = "Make a portable ceramic pipe for domestic building work from prepared ceramic stock.",
		Action = "making portable ceramic pipe for domestic building work",
		ActiveCraftItemSdesc = "an in-progress portable ceramic pipe for domestic building work craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3697.0, Material: "ceramic", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3697 grams of ceramic", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_construction_utilities_0054_portable_pipe", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a portable ceramic pipe for domestic building work (@industrialised_construction_utilities_0054_portable_pipe)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ConstructionAndUtilitiesMakePortableMildSteelValveForDomesticBuildingWorkCraftIndustrialisedConstructionUtilities0055PortableValve() => new()
	{
		Metadata = new("Construction and utilities/make portable mild steel valve for domestic building work [craft_industrialised_construction_utilities_0055_portable_valve]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_construction_utilities_0055_portable_valve"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make portable mild steel valve for domestic building work [craft_industrialised_construction_utilities_0055_portable_valve]",
		Category = "Construction and utilities",
		Blurb = "Make a portable mild steel valve for domestic building work from prepared mild steel stock.",
		Action = "making portable mild steel valve for domestic building work",
		ActiveCraftItemSdesc = "an in-progress portable mild steel valve for domestic building work craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3738.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3738 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_construction_utilities_0055_portable_valve", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a portable mild steel valve for domestic building work (@industrialised_construction_utilities_0055_portable_valve)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ConstructionAndUtilitiesMakePortableCastIronFittingForDomesticBuildingWorkCraftIndustrialisedConstructionUtilities0056PortableFitting() => new()
	{
		Metadata = new("Construction and utilities/make portable cast iron fitting for domestic building work [craft_industrialised_construction_utilities_0056_portable_fitting]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_construction_utilities_0056_portable_fitting"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make portable cast iron fitting for domestic building work [craft_industrialised_construction_utilities_0056_portable_fitting]",
		Category = "Construction and utilities",
		Blurb = "Make a portable cast iron fitting for domestic building work from prepared cast iron stock.",
		Action = "making portable cast iron fitting for domestic building work",
		ActiveCraftItemSdesc = "an in-progress portable cast iron fitting for domestic building work craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3779.0, Material: "cast iron", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3779 grams of cast iron", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_construction_utilities_0056_portable_fitting", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a portable cast iron fitting for domestic building work (@industrialised_construction_utilities_0056_portable_fitting)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ConstructionAndUtilitiesMakePortableOakBeamForDomesticBuildingWorkCraftIndustrialisedConstructionUtilities0057PortableBeam() => new()
	{
		Metadata = new("Construction and utilities/make portable oak beam for domestic building work [craft_industrialised_construction_utilities_0057_portable_beam]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_construction_utilities_0057_portable_beam"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make portable oak beam for domestic building work [craft_industrialised_construction_utilities_0057_portable_beam]",
		Category = "Construction and utilities",
		Blurb = "Make a portable oak beam for domestic building work from prepared oak stock.",
		Action = "making portable oak beam for domestic building work",
		ActiveCraftItemSdesc = "an in-progress portable oak beam for domestic building work craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3819.0, Material: "oak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3819 grams of oak", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_construction_utilities_0057_portable_beam", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a portable oak beam for domestic building work (@industrialised_construction_utilities_0057_portable_beam)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ConstructionAndUtilitiesMakePortableCeramicPanelForDomesticBuildingWorkCraftIndustrialisedConstructionUtilities0058PortablePanel() => new()
	{
		Metadata = new("Construction and utilities/make portable ceramic panel for domestic building work [craft_industrialised_construction_utilities_0058_portable_panel]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_construction_utilities_0058_portable_panel"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make portable ceramic panel for domestic building work [craft_industrialised_construction_utilities_0058_portable_panel]",
		Category = "Construction and utilities",
		Blurb = "Make a portable ceramic panel for domestic building work from prepared ceramic stock.",
		Action = "making portable ceramic panel for domestic building work",
		ActiveCraftItemSdesc = "an in-progress portable ceramic panel for domestic building work craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3860.0, Material: "ceramic", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3860 grams of ceramic", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_construction_utilities_0058_portable_panel", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a portable ceramic panel for domestic building work (@industrialised_construction_utilities_0058_portable_panel)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ConstructionAndUtilitiesMakePortableMildSteelGrateForDomesticBuildingWorkCraftIndustrialisedConstructionUtilities0059PortableGrate() => new()
	{
		Metadata = new("Construction and utilities/make portable mild steel grate for domestic building work [craft_industrialised_construction_utilities_0059_portable_grate]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_construction_utilities_0059_portable_grate"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make portable mild steel grate for domestic building work [craft_industrialised_construction_utilities_0059_portable_grate]",
		Category = "Construction and utilities",
		Blurb = "Make a portable mild steel grate for domestic building work from prepared mild steel stock.",
		Action = "making portable mild steel grate for domestic building work",
		ActiveCraftItemSdesc = "an in-progress portable mild steel grate for domestic building work craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3901.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3901 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_construction_utilities_0059_portable_grate", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a portable mild steel grate for domestic building work (@industrialised_construction_utilities_0059_portable_grate)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ConstructionAndUtilitiesMakePortableCastIronConduitForDomesticBuildingWorkCraftIndustrialisedConstructionUtilities0060PortableConduit() => new()
	{
		Metadata = new("Construction and utilities/make portable cast iron conduit for domestic building work [craft_industrialised_construction_utilities_0060_portable_conduit]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_construction_utilities_0060_portable_conduit"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make portable cast iron conduit for domestic building work [craft_industrialised_construction_utilities_0060_portable_conduit]",
		Category = "Construction and utilities",
		Blurb = "Make a portable cast iron conduit for domestic building work from prepared cast iron stock.",
		Action = "making portable cast iron conduit for domestic building work",
		ActiveCraftItemSdesc = "an in-progress portable cast iron conduit for domestic building work craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3941.0, Material: "cast iron", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3941 grams of cast iron", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_construction_utilities_0060_portable_conduit", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a portable cast iron conduit for domestic building work (@industrialised_construction_utilities_0060_portable_conduit)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ConstructionAndUtilitiesMakeWorkshopOakRubbleForDomesticBuildingWorkCraftIndustrialisedConstructionUtilities0061WorkshopRubble() => new()
	{
		Metadata = new("Construction and utilities/make workshop oak rubble for domestic building work [craft_industrialised_construction_utilities_0061_workshop_rubble]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_construction_utilities_0061_workshop_rubble"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make workshop oak rubble for domestic building work [craft_industrialised_construction_utilities_0061_workshop_rubble]",
		Category = "Construction and utilities",
		Blurb = "Make a workshop oak rubble for domestic building work from prepared oak stock.",
		Action = "making workshop oak rubble for domestic building work",
		ActiveCraftItemSdesc = "an in-progress workshop oak rubble for domestic building work craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3982.0, Material: "oak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3982 grams of oak", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_construction_utilities_0061_workshop_rubble", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a workshop oak rubble for domestic building work (@industrialised_construction_utilities_0061_workshop_rubble)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ConstructionAndUtilitiesMakeWorkshopCeramicBrickForDomesticBuildingWorkCraftIndustrialisedConstructionUtilities0062WorkshopBrick() => new()
	{
		Metadata = new("Construction and utilities/make workshop ceramic brick for domestic building work [craft_industrialised_construction_utilities_0062_workshop_brick]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_construction_utilities_0062_workshop_brick"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make workshop ceramic brick for domestic building work [craft_industrialised_construction_utilities_0062_workshop_brick]",
		Category = "Construction and utilities",
		Blurb = "Make a workshop ceramic brick for domestic building work from prepared ceramic stock.",
		Action = "making workshop ceramic brick for domestic building work",
		ActiveCraftItemSdesc = "an in-progress workshop ceramic brick for domestic building work craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 4023.0, Material: "ceramic", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("4023 grams of ceramic", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_construction_utilities_0062_workshop_brick", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a workshop ceramic brick for domestic building work (@industrialised_construction_utilities_0062_workshop_brick)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ConstructionAndUtilitiesMakeWorkshopMildSteelTileForDomesticBuildingWorkCraftIndustrialisedConstructionUtilities0063WorkshopTile() => new()
	{
		Metadata = new("Construction and utilities/make workshop mild steel tile for domestic building work [craft_industrialised_construction_utilities_0063_workshop_tile]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_construction_utilities_0063_workshop_tile"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make workshop mild steel tile for domestic building work [craft_industrialised_construction_utilities_0063_workshop_tile]",
		Category = "Construction and utilities",
		Blurb = "Make a workshop mild steel tile for domestic building work from prepared mild steel stock.",
		Action = "making workshop mild steel tile for domestic building work",
		ActiveCraftItemSdesc = "an in-progress workshop mild steel tile for domestic building work craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 4063.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("4063 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_construction_utilities_0063_workshop_tile", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a workshop mild steel tile for domestic building work (@industrialised_construction_utilities_0063_workshop_tile)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ConstructionAndUtilitiesMakeWorkshopCastIronPipeForDomesticBuildingWorkCraftIndustrialisedConstructionUtilities0064WorkshopPipe() => new()
	{
		Metadata = new("Construction and utilities/make workshop cast iron pipe for domestic building work [craft_industrialised_construction_utilities_0064_workshop_pipe]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_construction_utilities_0064_workshop_pipe"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make workshop cast iron pipe for domestic building work [craft_industrialised_construction_utilities_0064_workshop_pipe]",
		Category = "Construction and utilities",
		Blurb = "Make a workshop cast iron pipe for domestic building work from prepared cast iron stock.",
		Action = "making workshop cast iron pipe for domestic building work",
		ActiveCraftItemSdesc = "an in-progress workshop cast iron pipe for domestic building work craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 4104.0, Material: "cast iron", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("4104 grams of cast iron", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_construction_utilities_0064_workshop_pipe", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a workshop cast iron pipe for domestic building work (@industrialised_construction_utilities_0064_workshop_pipe)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ConstructionAndUtilitiesMakeWorkshopOakValveForDomesticBuildingWorkCraftIndustrialisedConstructionUtilities0065WorkshopValve() => new()
	{
		Metadata = new("Construction and utilities/make workshop oak valve for domestic building work [craft_industrialised_construction_utilities_0065_workshop_valve]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_construction_utilities_0065_workshop_valve"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make workshop oak valve for domestic building work [craft_industrialised_construction_utilities_0065_workshop_valve]",
		Category = "Construction and utilities",
		Blurb = "Make a workshop oak valve for domestic building work from prepared oak stock.",
		Action = "making workshop oak valve for domestic building work",
		ActiveCraftItemSdesc = "an in-progress workshop oak valve for domestic building work craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 4145.0, Material: "oak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("4145 grams of oak", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_construction_utilities_0065_workshop_valve", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a workshop oak valve for domestic building work (@industrialised_construction_utilities_0065_workshop_valve)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ConstructionAndUtilitiesMakeWorkshopCeramicFittingForDomesticBuildingWorkCraftIndustrialisedConstructionUtilities0066WorkshopFitting() => new()
	{
		Metadata = new("Construction and utilities/make workshop ceramic fitting for domestic building work [craft_industrialised_construction_utilities_0066_workshop_fitting]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_construction_utilities_0066_workshop_fitting"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make workshop ceramic fitting for domestic building work [craft_industrialised_construction_utilities_0066_workshop_fitting]",
		Category = "Construction and utilities",
		Blurb = "Make a workshop ceramic fitting for domestic building work from prepared ceramic stock.",
		Action = "making workshop ceramic fitting for domestic building work",
		ActiveCraftItemSdesc = "an in-progress workshop ceramic fitting for domestic building work craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 4186.0, Material: "ceramic", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("4186 grams of ceramic", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_construction_utilities_0066_workshop_fitting", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a workshop ceramic fitting for domestic building work (@industrialised_construction_utilities_0066_workshop_fitting)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ConstructionAndUtilitiesMakeWorkshopMildSteelBeamForDomesticBuildingWorkCraftIndustrialisedConstructionUtilities0067WorkshopBeam() => new()
	{
		Metadata = new("Construction and utilities/make workshop mild steel beam for domestic building work [craft_industrialised_construction_utilities_0067_workshop_beam]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_construction_utilities_0067_workshop_beam"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make workshop mild steel beam for domestic building work [craft_industrialised_construction_utilities_0067_workshop_beam]",
		Category = "Construction and utilities",
		Blurb = "Make a workshop mild steel beam for domestic building work from prepared mild steel stock.",
		Action = "making workshop mild steel beam for domestic building work",
		ActiveCraftItemSdesc = "an in-progress workshop mild steel beam for domestic building work craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 4226.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("4226 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_construction_utilities_0067_workshop_beam", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a workshop mild steel beam for domestic building work (@industrialised_construction_utilities_0067_workshop_beam)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ConstructionAndUtilitiesMakeWorkshopCastIronPanelForDomesticBuildingWorkCraftIndustrialisedConstructionUtilities0068WorkshopPanel() => new()
	{
		Metadata = new("Construction and utilities/make workshop cast iron panel for domestic building work [craft_industrialised_construction_utilities_0068_workshop_panel]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_construction_utilities_0068_workshop_panel"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make workshop cast iron panel for domestic building work [craft_industrialised_construction_utilities_0068_workshop_panel]",
		Category = "Construction and utilities",
		Blurb = "Make a workshop cast iron panel for domestic building work from prepared cast iron stock.",
		Action = "making workshop cast iron panel for domestic building work",
		ActiveCraftItemSdesc = "an in-progress workshop cast iron panel for domestic building work craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 4267.0, Material: "cast iron", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("4267 grams of cast iron", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_construction_utilities_0068_workshop_panel", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a workshop cast iron panel for domestic building work (@industrialised_construction_utilities_0068_workshop_panel)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ConstructionAndUtilitiesMakeWorkshopOakGrateForDomesticBuildingWorkCraftIndustrialisedConstructionUtilities0069WorkshopGrate() => new()
	{
		Metadata = new("Construction and utilities/make workshop oak grate for domestic building work [craft_industrialised_construction_utilities_0069_workshop_grate]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_construction_utilities_0069_workshop_grate"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make workshop oak grate for domestic building work [craft_industrialised_construction_utilities_0069_workshop_grate]",
		Category = "Construction and utilities",
		Blurb = "Make a workshop oak grate for domestic building work from prepared oak stock.",
		Action = "making workshop oak grate for domestic building work",
		ActiveCraftItemSdesc = "an in-progress workshop oak grate for domestic building work craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 4308.0, Material: "oak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("4308 grams of oak", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_construction_utilities_0069_workshop_grate", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a workshop oak grate for domestic building work (@industrialised_construction_utilities_0069_workshop_grate)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ConstructionAndUtilitiesMakeWorkshopCeramicConduitForDomesticBuildingWorkCraftIndustrialisedConstructionUtilities0070WorkshopConduit() => new()
	{
		Metadata = new("Construction and utilities/make workshop ceramic conduit for domestic building work [craft_industrialised_construction_utilities_0070_workshop_conduit]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_construction_utilities_0070_workshop_conduit"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make workshop ceramic conduit for domestic building work [craft_industrialised_construction_utilities_0070_workshop_conduit]",
		Category = "Construction and utilities",
		Blurb = "Make a workshop ceramic conduit for domestic building work from prepared ceramic stock.",
		Action = "making workshop ceramic conduit for domestic building work",
		ActiveCraftItemSdesc = "an in-progress workshop ceramic conduit for domestic building work craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 4348.0, Material: "ceramic", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("4348 grams of ceramic", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_construction_utilities_0070_workshop_conduit", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a workshop ceramic conduit for domestic building work (@industrialised_construction_utilities_0070_workshop_conduit)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ConstructionAndUtilitiesMakeInstitutionalMildSteelRubbleForDomesticBuildingWorkCraftIndustrialisedConstructionUtilities0071InstitutionalRubble() => new()
	{
		Metadata = new("Construction and utilities/make institutional mild steel rubble for domestic building work [craft_industrialised_construction_utilities_0071_institutional_rubble]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_construction_utilities_0071_institutional_rubble"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make institutional mild steel rubble for domestic building work [craft_industrialised_construction_utilities_0071_institutional_rubble]",
		Category = "Construction and utilities",
		Blurb = "Make an institutional mild steel rubble for domestic building work from prepared mild steel stock.",
		Action = "making institutional mild steel rubble for domestic building work",
		ActiveCraftItemSdesc = "an in-progress institutional mild steel rubble for domestic building work craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 4389.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("4389 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_construction_utilities_0071_institutional_rubble", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x an institutional mild steel rubble for domestic building work (@industrialised_construction_utilities_0071_institutional_rubble)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ConstructionAndUtilitiesMakeInstitutionalCastIronBrickForDomesticBuildingWorkCraftIndustrialisedConstructionUtilities0072InstitutionalBrick() => new()
	{
		Metadata = new("Construction and utilities/make institutional cast iron brick for domestic building work [craft_industrialised_construction_utilities_0072_institutional_brick]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_construction_utilities_0072_institutional_brick"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make institutional cast iron brick for domestic building work [craft_industrialised_construction_utilities_0072_institutional_brick]",
		Category = "Construction and utilities",
		Blurb = "Make an institutional cast iron brick for domestic building work from prepared cast iron stock.",
		Action = "making institutional cast iron brick for domestic building work",
		ActiveCraftItemSdesc = "an in-progress institutional cast iron brick for domestic building work craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 4430.0, Material: "cast iron", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("4430 grams of cast iron", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_construction_utilities_0072_institutional_brick", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x an institutional cast iron brick for domestic building work (@industrialised_construction_utilities_0072_institutional_brick)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ConstructionAndUtilitiesMakeInstitutionalOakTileForDomesticBuildingWorkCraftIndustrialisedConstructionUtilities0073InstitutionalTile() => new()
	{
		Metadata = new("Construction and utilities/make institutional oak tile for domestic building work [craft_industrialised_construction_utilities_0073_institutional_tile]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_construction_utilities_0073_institutional_tile"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make institutional oak tile for domestic building work [craft_industrialised_construction_utilities_0073_institutional_tile]",
		Category = "Construction and utilities",
		Blurb = "Make an institutional oak tile for domestic building work from prepared oak stock.",
		Action = "making institutional oak tile for domestic building work",
		ActiveCraftItemSdesc = "an in-progress institutional oak tile for domestic building work craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 4470.0, Material: "oak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("4470 grams of oak", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_construction_utilities_0073_institutional_tile", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x an institutional oak tile for domestic building work (@industrialised_construction_utilities_0073_institutional_tile)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ConstructionAndUtilitiesMakeInstitutionalCeramicPipeForDomesticBuildingWorkCraftIndustrialisedConstructionUtilities0074InstitutionalPipe() => new()
	{
		Metadata = new("Construction and utilities/make institutional ceramic pipe for domestic building work [craft_industrialised_construction_utilities_0074_institutional_pipe]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_construction_utilities_0074_institutional_pipe"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make institutional ceramic pipe for domestic building work [craft_industrialised_construction_utilities_0074_institutional_pipe]",
		Category = "Construction and utilities",
		Blurb = "Make an institutional ceramic pipe for domestic building work from prepared ceramic stock.",
		Action = "making institutional ceramic pipe for domestic building work",
		ActiveCraftItemSdesc = "an in-progress institutional ceramic pipe for domestic building work craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 4511.0, Material: "ceramic", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("4511 grams of ceramic", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_construction_utilities_0074_institutional_pipe", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x an institutional ceramic pipe for domestic building work (@industrialised_construction_utilities_0074_institutional_pipe)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ConstructionAndUtilitiesMakeInstitutionalMildSteelValveForDomesticBuildingWorkCraftIndustrialisedConstructionUtilities0075InstitutionalValve() => new()
	{
		Metadata = new("Construction and utilities/make institutional mild steel valve for domestic building work [craft_industrialised_construction_utilities_0075_institutional_valve]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_construction_utilities_0075_institutional_valve"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make institutional mild steel valve for domestic building work [craft_industrialised_construction_utilities_0075_institutional_valve]",
		Category = "Construction and utilities",
		Blurb = "Make an institutional mild steel valve for domestic building work from prepared mild steel stock.",
		Action = "making institutional mild steel valve for domestic building work",
		ActiveCraftItemSdesc = "an in-progress institutional mild steel valve for domestic building work craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 4552.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("4552 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_construction_utilities_0075_institutional_valve", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x an institutional mild steel valve for domestic building work (@industrialised_construction_utilities_0075_institutional_valve)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ConstructionAndUtilitiesMakeInstitutionalCastIronFittingForDomesticBuildingWorkCraftIndustrialisedConstructionUtilities0076InstitutionalFitting() => new()
	{
		Metadata = new("Construction and utilities/make institutional cast iron fitting for domestic building work [craft_industrialised_construction_utilities_0076_institutional_fitting]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_construction_utilities_0076_institutional_fitting"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make institutional cast iron fitting for domestic building work [craft_industrialised_construction_utilities_0076_institutional_fitting]",
		Category = "Construction and utilities",
		Blurb = "Make an institutional cast iron fitting for domestic building work from prepared cast iron stock.",
		Action = "making institutional cast iron fitting for domestic building work",
		ActiveCraftItemSdesc = "an in-progress institutional cast iron fitting for domestic building work craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 4592.0, Material: "cast iron", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("4592 grams of cast iron", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_construction_utilities_0076_institutional_fitting", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x an institutional cast iron fitting for domestic building work (@industrialised_construction_utilities_0076_institutional_fitting)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ConstructionAndUtilitiesMakeReinforcedMildSteelRubbleForWorkshopBuildingWorkCraftIndustrialConstructionUtilities0001ReinforcedRubble() => new()
	{
		Metadata = new("Construction and utilities/make reinforced mild steel rubble for workshop building work [craft_industrial_construction_utilities_0001_reinforced_rubble]", "crafts", ["industrial"], ["item:industrial_construction_utilities_0001_reinforced_rubble"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make reinforced mild steel rubble for workshop building work [craft_industrial_construction_utilities_0001_reinforced_rubble]",
		Category = "Construction and utilities",
		Blurb = "Make a reinforced mild steel rubble for workshop building work from prepared mild steel stock.",
		Action = "making reinforced mild steel rubble for workshop building work",
		ActiveCraftItemSdesc = "an in-progress reinforced mild steel rubble for workshop building work craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1540.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1540 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrial_construction_utilities_0001_reinforced_rubble", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a reinforced mild steel rubble for workshop building work (@industrial_construction_utilities_0001_reinforced_rubble)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ConstructionAndUtilitiesMakeReinforcedCastIronBrickForWorkshopBuildingWorkCraftIndustrialConstructionUtilities0002ReinforcedBrick() => new()
	{
		Metadata = new("Construction and utilities/make reinforced cast iron brick for workshop building work [craft_industrial_construction_utilities_0002_reinforced_brick]", "crafts", ["industrial"], ["item:industrial_construction_utilities_0002_reinforced_brick"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make reinforced cast iron brick for workshop building work [craft_industrial_construction_utilities_0002_reinforced_brick]",
		Category = "Construction and utilities",
		Blurb = "Make a reinforced cast iron brick for workshop building work from prepared cast iron stock.",
		Action = "making reinforced cast iron brick for workshop building work",
		ActiveCraftItemSdesc = "an in-progress reinforced cast iron brick for workshop building work craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1581.0, Material: "cast iron", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1581 grams of cast iron", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrial_construction_utilities_0002_reinforced_brick", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a reinforced cast iron brick for workshop building work (@industrial_construction_utilities_0002_reinforced_brick)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ConstructionAndUtilitiesMakeReinforcedOakTileForWorkshopBuildingWorkCraftIndustrialConstructionUtilities0003ReinforcedTile() => new()
	{
		Metadata = new("Construction and utilities/make reinforced oak tile for workshop building work [craft_industrial_construction_utilities_0003_reinforced_tile]", "crafts", ["industrial"], ["item:industrial_construction_utilities_0003_reinforced_tile"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make reinforced oak tile for workshop building work [craft_industrial_construction_utilities_0003_reinforced_tile]",
		Category = "Construction and utilities",
		Blurb = "Make a reinforced oak tile for workshop building work from prepared oak stock.",
		Action = "making reinforced oak tile for workshop building work",
		ActiveCraftItemSdesc = "an in-progress reinforced oak tile for workshop building work craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1621.0, Material: "oak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1621 grams of oak", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrial_construction_utilities_0003_reinforced_tile", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a reinforced oak tile for workshop building work (@industrial_construction_utilities_0003_reinforced_tile)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ConstructionAndUtilitiesMakeReinforcedCeramicPipeForWorkshopBuildingWorkCraftIndustrialConstructionUtilities0004ReinforcedPipe() => new()
	{
		Metadata = new("Construction and utilities/make reinforced ceramic pipe for workshop building work [craft_industrial_construction_utilities_0004_reinforced_pipe]", "crafts", ["industrial"], ["item:industrial_construction_utilities_0004_reinforced_pipe"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make reinforced ceramic pipe for workshop building work [craft_industrial_construction_utilities_0004_reinforced_pipe]",
		Category = "Construction and utilities",
		Blurb = "Make a reinforced ceramic pipe for workshop building work from prepared ceramic stock.",
		Action = "making reinforced ceramic pipe for workshop building work",
		ActiveCraftItemSdesc = "an in-progress reinforced ceramic pipe for workshop building work craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1662.0, Material: "ceramic", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1662 grams of ceramic", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrial_construction_utilities_0004_reinforced_pipe", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a reinforced ceramic pipe for workshop building work (@industrial_construction_utilities_0004_reinforced_pipe)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ConstructionAndUtilitiesMakeReinforcedMildSteelValveForWorkshopBuildingWorkCraftIndustrialConstructionUtilities0005ReinforcedValve() => new()
	{
		Metadata = new("Construction and utilities/make reinforced mild steel valve for workshop building work [craft_industrial_construction_utilities_0005_reinforced_valve]", "crafts", ["industrial"], ["item:industrial_construction_utilities_0005_reinforced_valve"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make reinforced mild steel valve for workshop building work [craft_industrial_construction_utilities_0005_reinforced_valve]",
		Category = "Construction and utilities",
		Blurb = "Make a reinforced mild steel valve for workshop building work from prepared mild steel stock.",
		Action = "making reinforced mild steel valve for workshop building work",
		ActiveCraftItemSdesc = "an in-progress reinforced mild steel valve for workshop building work craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1703.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1703 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrial_construction_utilities_0005_reinforced_valve", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a reinforced mild steel valve for workshop building work (@industrial_construction_utilities_0005_reinforced_valve)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ConstructionAndUtilitiesMakeReinforcedCastIronFittingForWorkshopBuildingWorkCraftIndustrialConstructionUtilities0006ReinforcedFitting() => new()
	{
		Metadata = new("Construction and utilities/make reinforced cast iron fitting for workshop building work [craft_industrial_construction_utilities_0006_reinforced_fitting]", "crafts", ["industrial"], ["item:industrial_construction_utilities_0006_reinforced_fitting"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make reinforced cast iron fitting for workshop building work [craft_industrial_construction_utilities_0006_reinforced_fitting]",
		Category = "Construction and utilities",
		Blurb = "Make a reinforced cast iron fitting for workshop building work from prepared cast iron stock.",
		Action = "making reinforced cast iron fitting for workshop building work",
		ActiveCraftItemSdesc = "an in-progress reinforced cast iron fitting for workshop building work craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1744.0, Material: "cast iron", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1744 grams of cast iron", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrial_construction_utilities_0006_reinforced_fitting", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a reinforced cast iron fitting for workshop building work (@industrial_construction_utilities_0006_reinforced_fitting)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ConstructionAndUtilitiesMakeReinforcedOakBeamForWorkshopBuildingWorkCraftIndustrialConstructionUtilities0007ReinforcedBeam() => new()
	{
		Metadata = new("Construction and utilities/make reinforced oak beam for workshop building work [craft_industrial_construction_utilities_0007_reinforced_beam]", "crafts", ["industrial"], ["item:industrial_construction_utilities_0007_reinforced_beam"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make reinforced oak beam for workshop building work [craft_industrial_construction_utilities_0007_reinforced_beam]",
		Category = "Construction and utilities",
		Blurb = "Make a reinforced oak beam for workshop building work from prepared oak stock.",
		Action = "making reinforced oak beam for workshop building work",
		ActiveCraftItemSdesc = "an in-progress reinforced oak beam for workshop building work craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1784.0, Material: "oak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1784 grams of oak", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrial_construction_utilities_0007_reinforced_beam", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a reinforced oak beam for workshop building work (@industrial_construction_utilities_0007_reinforced_beam)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ConstructionAndUtilitiesMakeReinforcedCeramicPanelForWorkshopBuildingWorkCraftIndustrialConstructionUtilities0008ReinforcedPanel() => new()
	{
		Metadata = new("Construction and utilities/make reinforced ceramic panel for workshop building work [craft_industrial_construction_utilities_0008_reinforced_panel]", "crafts", ["industrial"], ["item:industrial_construction_utilities_0008_reinforced_panel"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make reinforced ceramic panel for workshop building work [craft_industrial_construction_utilities_0008_reinforced_panel]",
		Category = "Construction and utilities",
		Blurb = "Make a reinforced ceramic panel for workshop building work from prepared ceramic stock.",
		Action = "making reinforced ceramic panel for workshop building work",
		ActiveCraftItemSdesc = "an in-progress reinforced ceramic panel for workshop building work craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1825.0, Material: "ceramic", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1825 grams of ceramic", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrial_construction_utilities_0008_reinforced_panel", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a reinforced ceramic panel for workshop building work (@industrial_construction_utilities_0008_reinforced_panel)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ConstructionAndUtilitiesMakeReinforcedMildSteelGrateForWorkshopBuildingWorkCraftIndustrialConstructionUtilities0009ReinforcedGrate() => new()
	{
		Metadata = new("Construction and utilities/make reinforced mild steel grate for workshop building work [craft_industrial_construction_utilities_0009_reinforced_grate]", "crafts", ["industrial"], ["item:industrial_construction_utilities_0009_reinforced_grate"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make reinforced mild steel grate for workshop building work [craft_industrial_construction_utilities_0009_reinforced_grate]",
		Category = "Construction and utilities",
		Blurb = "Make a reinforced mild steel grate for workshop building work from prepared mild steel stock.",
		Action = "making reinforced mild steel grate for workshop building work",
		ActiveCraftItemSdesc = "an in-progress reinforced mild steel grate for workshop building work craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1866.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1866 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrial_construction_utilities_0009_reinforced_grate", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a reinforced mild steel grate for workshop building work (@industrial_construction_utilities_0009_reinforced_grate)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ConstructionAndUtilitiesMakeReinforcedCastIronConduitForWorkshopBuildingWorkCraftIndustrialConstructionUtilities0010ReinforcedConduit() => new()
	{
		Metadata = new("Construction and utilities/make reinforced cast iron conduit for workshop building work [craft_industrial_construction_utilities_0010_reinforced_conduit]", "crafts", ["industrial"], ["item:industrial_construction_utilities_0010_reinforced_conduit"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make reinforced cast iron conduit for workshop building work [craft_industrial_construction_utilities_0010_reinforced_conduit]",
		Category = "Construction and utilities",
		Blurb = "Make a reinforced cast iron conduit for workshop building work from prepared cast iron stock.",
		Action = "making reinforced cast iron conduit for workshop building work",
		ActiveCraftItemSdesc = "an in-progress reinforced cast iron conduit for workshop building work craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1906.0, Material: "cast iron", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1906 grams of cast iron", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrial_construction_utilities_0010_reinforced_conduit", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a reinforced cast iron conduit for workshop building work (@industrial_construction_utilities_0010_reinforced_conduit)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ConstructionAndUtilitiesMakeCompactOakRubbleForWorkshopBuildingWorkCraftIndustrialConstructionUtilities0011CompactRubble() => new()
	{
		Metadata = new("Construction and utilities/make compact oak rubble for workshop building work [craft_industrial_construction_utilities_0011_compact_rubble]", "crafts", ["industrial"], ["item:industrial_construction_utilities_0011_compact_rubble"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make compact oak rubble for workshop building work [craft_industrial_construction_utilities_0011_compact_rubble]",
		Category = "Construction and utilities",
		Blurb = "Make a compact oak rubble for workshop building work from prepared oak stock.",
		Action = "making compact oak rubble for workshop building work",
		ActiveCraftItemSdesc = "an in-progress compact oak rubble for workshop building work craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1947.0, Material: "oak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1947 grams of oak", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrial_construction_utilities_0011_compact_rubble", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a compact oak rubble for workshop building work (@industrial_construction_utilities_0011_compact_rubble)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MedicalAndMobilityMakePlainCottonWasteForDomesticCareCraftIndustrialisedMedicalMobility0001PlainWaste() => new()
	{
		Metadata = new("Medical and mobility/make plain cotton waste for domestic care [craft_industrialised_medical_mobility_0001_plain_waste]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_medical_mobility_0001_plain_waste"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make plain cotton waste for domestic care [craft_industrialised_medical_mobility_0001_plain_waste]",
		Category = "Medical and mobility",
		Blurb = "Make a plain cotton waste for domestic care from prepared cotton stock.",
		Action = "making plain cotton waste for domestic care",
		ActiveCraftItemSdesc = "an in-progress plain cotton waste for domestic care craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
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
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_medical_mobility_0001_plain_waste", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a plain cotton waste for domestic care (@industrialised_medical_mobility_0001_plain_waste)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MedicalAndMobilityMakePlainLinenBandageForDomesticCareCraftIndustrialisedMedicalMobility0002PlainBandage() => new()
	{
		Metadata = new("Medical and mobility/make plain linen bandage for domestic care [craft_industrialised_medical_mobility_0002_plain_bandage]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_medical_mobility_0002_plain_bandage"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make plain linen bandage for domestic care [craft_industrialised_medical_mobility_0002_plain_bandage]",
		Category = "Medical and mobility",
		Blurb = "Make a plain linen bandage for domestic care from prepared linen stock.",
		Action = "making plain linen bandage for domestic care",
		ActiveCraftItemSdesc = "an in-progress plain linen bandage for domestic care craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
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
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_medical_mobility_0002_plain_bandage", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a plain linen bandage for domestic care (@industrialised_medical_mobility_0002_plain_bandage)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MedicalAndMobilityMakePlainMildSteelSplintForDomesticCareCraftIndustrialisedMedicalMobility0003PlainSplint() => new()
	{
		Metadata = new("Medical and mobility/make plain mild steel splint for domestic care [craft_industrialised_medical_mobility_0003_plain_splint]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_medical_mobility_0003_plain_splint"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make plain mild steel splint for domestic care [craft_industrialised_medical_mobility_0003_plain_splint]",
		Category = "Medical and mobility",
		Blurb = "Make a plain mild steel splint for domestic care from prepared mild steel stock.",
		Action = "making plain mild steel splint for domestic care",
		ActiveCraftItemSdesc = "an in-progress plain mild steel splint for domestic care craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
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
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_medical_mobility_0003_plain_splint", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a plain mild steel splint for domestic care (@industrialised_medical_mobility_0003_plain_splint)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MedicalAndMobilityMakePlainGlassCrutchForDomesticCareCraftIndustrialisedMedicalMobility0004PlainCrutch() => new()
	{
		Metadata = new("Medical and mobility/make plain glass crutch for domestic care [craft_industrialised_medical_mobility_0004_plain_crutch]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_medical_mobility_0004_plain_crutch"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make plain glass crutch for domestic care [craft_industrialised_medical_mobility_0004_plain_crutch]",
		Category = "Medical and mobility",
		Blurb = "Make a plain glass crutch for domestic care from prepared glass stock.",
		Action = "making plain glass crutch for domestic care",
		ActiveCraftItemSdesc = "an in-progress plain glass crutch for domestic care craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
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
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_medical_mobility_0004_plain_crutch", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a plain glass crutch for domestic care (@industrialised_medical_mobility_0004_plain_crutch)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MedicalAndMobilityMakePlainOakStretcherForDomesticCareCraftIndustrialisedMedicalMobility0005PlainStretcher() => new()
	{
		Metadata = new("Medical and mobility/make plain oak stretcher for domestic care [craft_industrialised_medical_mobility_0005_plain_stretcher]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_medical_mobility_0005_plain_stretcher"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make plain oak stretcher for domestic care [craft_industrialised_medical_mobility_0005_plain_stretcher]",
		Category = "Medical and mobility",
		Blurb = "Make a plain oak stretcher for domestic care from prepared oak stock.",
		Action = "making plain oak stretcher for domestic care",
		ActiveCraftItemSdesc = "an in-progress plain oak stretcher for domestic care craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
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
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_medical_mobility_0005_plain_stretcher", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a plain oak stretcher for domestic care (@industrialised_medical_mobility_0005_plain_stretcher)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MedicalAndMobilityMakePlainCottonSyringeForDomesticCareCraftIndustrialisedMedicalMobility0006PlainSyringe() => new()
	{
		Metadata = new("Medical and mobility/make plain cotton syringe for domestic care [craft_industrialised_medical_mobility_0006_plain_syringe]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_medical_mobility_0006_plain_syringe"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make plain cotton syringe for domestic care [craft_industrialised_medical_mobility_0006_plain_syringe]",
		Category = "Medical and mobility",
		Blurb = "Make a plain cotton syringe for domestic care from prepared cotton stock.",
		Action = "making plain cotton syringe for domestic care",
		ActiveCraftItemSdesc = "an in-progress plain cotton syringe for domestic care craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
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
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_medical_mobility_0006_plain_syringe", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a plain cotton syringe for domestic care (@industrialised_medical_mobility_0006_plain_syringe)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};
}
