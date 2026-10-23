#nullable enable

using MudSharp.Body;
using MudSharp.GameItems.Inventory.Plans;
using MudSharp.RPG.Checks;

namespace DatabaseSeeder.Seeders;

public static partial class Recipes06Stock
{
	public static CatalogueRecipeDefinition PreIndustrialFoodmakingMakeSharedIntermediateNoodleRiceCatalogueStock() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/make Shared Intermediate noodle rice catalogue stock", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["medieval", "renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make Shared Intermediate noodle rice catalogue stock",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for preindustrial_food_rice_noodle_sheet.",
		Action = "preparing noodle stock",
		ActiveCraftItemSdesc = "a noodle preparation",
		Gate = CatalogueRecipeGates.PreIndustrialFoodCatalogueProductionBaking10,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Easy,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 3,
		Interruptable = false,
		Phases = [
			new(15, "$0 sort|sorts $i1, removing husks and damaged pieces from the noodle stock.", "$0 sort|sorts $i1, removing husks and damaged pieces from the noodle stock.", ExertionLevel.Stasis, 0),
			new(25, "$0 process|processes the noodle stock into a clean, useful household supply, using $t1.", "$0 process|processes the noodle stock into a clean, useful household supply, using $t1.", ExertionLevel.Stasis, 0),
			new(20, "$0 weigh|weighs $p1 and store|stores the finished noodle stock for later work.", "$0 inspect|inspects the poor stock and reserve|reserves it for a lesser use.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityTagRecipeInput(Grams: 1000.0, MaterialTag: "Food Crop", PileTag: "Seeded Yield", CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1 kilogram of a material tagged as Food Crop", ["piletag Seeded Yield"]), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Cooking", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Cooking tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueProgRecipeProduct(Prog: "ItemSeederPreIndustrialFood_Shared_Intermediate_Noodle_preindustrial_food_rice_noodle_sheet") { Compatibility = new("ItemSeederPreIndustrialFood_Shared_Intermediate_Noodle_preindustrial_food_rice_noodle_sheet", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingMakeSharedIntermediateIntermediateBreadCatalogueStock() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/make Shared Intermediate intermediate bread catalogue stock", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["medieval", "renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make Shared Intermediate intermediate bread catalogue stock",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for preindustrial_food_bread_stuffing_mix.",
		Action = "preparing intermediate stock",
		ActiveCraftItemSdesc = "a intermediate preparation",
		Gate = CatalogueRecipeGates.PreIndustrialFoodCatalogueProductionCooking10,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Easy,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 3,
		Interruptable = false,
		Phases = [
			new(15, "$0 sort|sorts $i1, removing husks and damaged pieces from the intermediate stock.", "$0 sort|sorts $i1, removing husks and damaged pieces from the intermediate stock.", ExertionLevel.Stasis, 0),
			new(25, "$0 process|processes the intermediate stock into a clean, useful household supply, using $t1.", "$0 process|processes the intermediate stock into a clean, useful household supply, using $t1.", ExertionLevel.Stasis, 0),
			new(20, "$0 weigh|weighs $p1 and store|stores the finished intermediate stock for later work.", "$0 inspect|inspects the poor stock and reserve|reserves it for a lesser use.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityTagRecipeInput(Grams: 1000.0, MaterialTag: "Food Crop", PileTag: "Seeded Yield", CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1 kilogram of a material tagged as Food Crop", ["piletag Seeded Yield"]), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Cooking", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Cooking tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueProgRecipeProduct(Prog: "ItemSeederPreIndustrialFood_Shared_Intermediate_Intermediate_preindustrial_food_bread_stuffing_mix") { Compatibility = new("ItemSeederPreIndustrialFood_Shared_Intermediate_Intermediate_preindustrial_food_bread_stuffing_mix", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingMakeSharedIntermediateIntermediateCheeseCatalogueStock() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/make Shared Intermediate intermediate cheese catalogue stock", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["medieval", "renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make Shared Intermediate intermediate cheese catalogue stock",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for preindustrial_food_cheese_curd_portion.",
		Action = "preparing intermediate stock",
		ActiveCraftItemSdesc = "a intermediate preparation",
		Gate = CatalogueRecipeGates.PreIndustrialFoodCatalogueProductionCooking10,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Easy,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 3,
		Interruptable = false,
		Phases = [
			new(15, "$0 sort|sorts $i1, removing husks and damaged pieces from the intermediate stock.", "$0 sort|sorts $i1, removing husks and damaged pieces from the intermediate stock.", ExertionLevel.Stasis, 0),
			new(25, "$0 process|processes the intermediate stock into a clean, useful household supply, using $t1.", "$0 process|processes the intermediate stock into a clean, useful household supply, using $t1.", ExertionLevel.Stasis, 0),
			new(20, "$0 weigh|weighs $p1 and store|stores the finished intermediate stock for later work.", "$0 inspect|inspects the poor stock and reserve|reserves it for a lesser use.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityTagRecipeInput(Grams: 1000.0, MaterialTag: "Raw Milk", PileTag: "Raw Milk", CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1 kilogram of a material tagged as Raw Milk", ["piletag Raw Milk"]), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Cooking", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Cooking tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueProgRecipeProduct(Prog: "ItemSeederPreIndustrialFood_Shared_Intermediate_Intermediate_preindustrial_food_brining_cheese_wheel") { Compatibility = new("ItemSeederPreIndustrialFood_Shared_Intermediate_Intermediate_preindustrial_food_brining_cheese_wheel", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingMakeSharedIntermediateIntermediateYoghurtCatalogueStock() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/make Shared Intermediate intermediate yoghurt catalogue stock", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["medieval", "renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make Shared Intermediate intermediate yoghurt catalogue stock",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for preindustrial_food_cultured_milk_clabber.",
		Action = "preparing intermediate stock",
		ActiveCraftItemSdesc = "a intermediate preparation",
		Gate = CatalogueRecipeGates.PreIndustrialFoodCatalogueProductionCooking10,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Easy,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 3,
		Interruptable = false,
		Phases = [
			new(15, "$0 sort|sorts $i1, removing husks and damaged pieces from the intermediate stock.", "$0 sort|sorts $i1, removing husks and damaged pieces from the intermediate stock.", ExertionLevel.Stasis, 0),
			new(25, "$0 process|processes the intermediate stock into a clean, useful household supply, using $t1.", "$0 process|processes the intermediate stock into a clean, useful household supply, using $t1.", ExertionLevel.Stasis, 0),
			new(20, "$0 weigh|weighs $p1 and store|stores the finished intermediate stock for later work.", "$0 inspect|inspects the poor stock and reserve|reserves it for a lesser use.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityTagRecipeInput(Grams: 1000.0, MaterialTag: "Raw Milk", PileTag: "Raw Milk", CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1 kilogram of a material tagged as Raw Milk", ["piletag Raw Milk"]), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Cooking", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Cooking tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueProgRecipeProduct(Prog: "ItemSeederPreIndustrialFood_Shared_Intermediate_Intermediate_preindustrial_food_cultured_milk_clabber") { Compatibility = new("ItemSeederPreIndustrialFood_Shared_Intermediate_Intermediate_preindustrial_food_cultured_milk_clabber", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingMakeSharedIntermediateIntermediateWheatCatalogueStock() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/make Shared Intermediate intermediate wheat catalogue stock", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["medieval", "renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make Shared Intermediate intermediate wheat catalogue stock",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for preindustrial_food_dried_broad_noodle_bundle.",
		Action = "preparing intermediate stock",
		ActiveCraftItemSdesc = "a intermediate preparation",
		Gate = CatalogueRecipeGates.PreIndustrialFoodCatalogueProductionMilling10,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Easy,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 3,
		Interruptable = false,
		Phases = [
			new(15, "$0 sort|sorts $i1, removing husks and damaged pieces from the intermediate stock.", "$0 sort|sorts $i1, removing husks and damaged pieces from the intermediate stock.", ExertionLevel.Stasis, 0),
			new(25, "$0 process|processes the intermediate stock into a clean, useful household supply, using $t1.", "$0 process|processes the intermediate stock into a clean, useful household supply, using $t1.", ExertionLevel.Stasis, 0),
			new(20, "$0 weigh|weighs $p1 and store|stores the finished intermediate stock for later work.", "$0 inspect|inspects the poor stock and reserve|reserves it for a lesser use.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityTagRecipeInput(Grams: 1000.0, MaterialTag: "Food Crop", PileTag: "Seeded Yield", CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1 kilogram of a material tagged as Food Crop", ["piletag Seeded Yield"]), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Cooking", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Cooking tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueProgRecipeProduct(Prog: "ItemSeederPreIndustrialFood_Shared_Intermediate_Intermediate_preindustrial_food_dried_broad_noodle_bundle") { Compatibility = new("ItemSeederPreIndustrialFood_Shared_Intermediate_Intermediate_preindustrial_food_dried_broad_noodle_bundle", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingMakeSharedIntermediateIntermediateMilletCatalogueStock() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/make Shared Intermediate intermediate millet catalogue stock", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["medieval", "renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make Shared Intermediate intermediate millet catalogue stock",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for preindustrial_food_dried_millet_noodle_bundle.",
		Action = "preparing intermediate stock",
		ActiveCraftItemSdesc = "a intermediate preparation",
		Gate = CatalogueRecipeGates.PreIndustrialFoodCatalogueProductionMilling10,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Easy,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 3,
		Interruptable = false,
		Phases = [
			new(15, "$0 sort|sorts $i1, removing husks and damaged pieces from the intermediate stock.", "$0 sort|sorts $i1, removing husks and damaged pieces from the intermediate stock.", ExertionLevel.Stasis, 0),
			new(25, "$0 process|processes the intermediate stock into a clean, useful household supply, using $t1.", "$0 process|processes the intermediate stock into a clean, useful household supply, using $t1.", ExertionLevel.Stasis, 0),
			new(20, "$0 weigh|weighs $p1 and store|stores the finished intermediate stock for later work.", "$0 inspect|inspects the poor stock and reserve|reserves it for a lesser use.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityTagRecipeInput(Grams: 1000.0, MaterialTag: "Food Crop", PileTag: "Seeded Yield", CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1 kilogram of a material tagged as Food Crop", ["piletag Seeded Yield"]), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Cooking", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Cooking tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueProgRecipeProduct(Prog: "ItemSeederPreIndustrialFood_Shared_Intermediate_Intermediate_preindustrial_food_dried_millet_noodle_bundle") { Compatibility = new("ItemSeederPreIndustrialFood_Shared_Intermediate_Intermediate_preindustrial_food_dried_millet_noodle_bundle", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingMakeSharedIntermediateIntermediateRiceCatalogueStock() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/make Shared Intermediate intermediate rice catalogue stock", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["medieval", "renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make Shared Intermediate intermediate rice catalogue stock",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for preindustrial_food_dried_rice_noodle_bundle.",
		Action = "preparing intermediate stock",
		ActiveCraftItemSdesc = "a intermediate preparation",
		Gate = CatalogueRecipeGates.PreIndustrialFoodCatalogueProductionMilling10,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Easy,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 3,
		Interruptable = false,
		Phases = [
			new(15, "$0 sort|sorts $i1, removing husks and damaged pieces from the intermediate stock.", "$0 sort|sorts $i1, removing husks and damaged pieces from the intermediate stock.", ExertionLevel.Stasis, 0),
			new(25, "$0 process|processes the intermediate stock into a clean, useful household supply, using $t1.", "$0 process|processes the intermediate stock into a clean, useful household supply, using $t1.", ExertionLevel.Stasis, 0),
			new(20, "$0 weigh|weighs $p1 and store|stores the finished intermediate stock for later work.", "$0 inspect|inspects the poor stock and reserve|reserves it for a lesser use.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityTagRecipeInput(Grams: 1000.0, MaterialTag: "Food Crop", PileTag: "Seeded Yield", CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1 kilogram of a material tagged as Food Crop", ["piletag Seeded Yield"]), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Cooking", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Cooking tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueProgRecipeProduct(Prog: "ItemSeederPreIndustrialFood_Shared_Intermediate_Intermediate_preindustrial_food_dried_rice_noodle_bundle") { Compatibility = new("ItemSeederPreIndustrialFood_Shared_Intermediate_Intermediate_preindustrial_food_dried_rice_noodle_bundle", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingMakeSharedIntermediateIntermediateEggCatalogueStock() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/make Shared Intermediate intermediate egg catalogue stock", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["medieval", "renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make Shared Intermediate intermediate egg catalogue stock",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for preindustrial_food_egg_cheese_filling_portion.",
		Action = "preparing intermediate stock",
		ActiveCraftItemSdesc = "a intermediate preparation",
		Gate = CatalogueRecipeGates.PreIndustrialFoodCatalogueProductionCooking10,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Easy,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 3,
		Interruptable = false,
		Phases = [
			new(15, "$0 sort|sorts $i1, removing husks and damaged pieces from the intermediate stock.", "$0 sort|sorts $i1, removing husks and damaged pieces from the intermediate stock.", ExertionLevel.Stasis, 0),
			new(25, "$0 process|processes the intermediate stock into a clean, useful household supply, using $t1.", "$0 process|processes the intermediate stock into a clean, useful household supply, using $t1.", ExertionLevel.Stasis, 0),
			new(20, "$0 weigh|weighs $p1 and store|stores the finished intermediate stock for later work.", "$0 inspect|inspects the poor stock and reserve|reserves it for a lesser use.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityTagRecipeInput(Grams: 1000.0, MaterialTag: "Egg Product", PileTag: "Egg Product", CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1 kilogram of a material tagged as Egg Product", ["piletag Egg Product"]), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Cooking", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Cooking tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueProgRecipeProduct(Prog: "ItemSeederPreIndustrialFood_Shared_Intermediate_Intermediate_preindustrial_food_egg_cheese_filling_portion") { Compatibility = new("ItemSeederPreIndustrialFood_Shared_Intermediate_Intermediate_preindustrial_food_egg_cheese_filling_portion", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingMakeSharedIntermediateIntermediateFishCatalogueStock() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/make Shared Intermediate intermediate fish catalogue stock", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["medieval", "renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make Shared Intermediate intermediate fish catalogue stock",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for preindustrial_food_fish_stuffing_mix.",
		Action = "preparing intermediate stock",
		ActiveCraftItemSdesc = "a intermediate preparation",
		Gate = CatalogueRecipeGates.PreIndustrialFoodCatalogueProductionButchering10,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Easy,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 3,
		Interruptable = false,
		Phases = [
			new(15, "$0 sort|sorts $i1, removing husks and damaged pieces from the intermediate stock.", "$0 sort|sorts $i1, removing husks and damaged pieces from the intermediate stock.", ExertionLevel.Stasis, 0),
			new(25, "$0 process|processes the intermediate stock into a clean, useful household supply, using $t1.", "$0 process|processes the intermediate stock into a clean, useful household supply, using $t1.", ExertionLevel.Stasis, 0),
			new(20, "$0 weigh|weighs $p1 and store|stores the finished intermediate stock for later work.", "$0 inspect|inspects the poor stock and reserve|reserves it for a lesser use.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueTagRecipeInput(Tag: "Raw Fish Cut", Quantity: 1) { Compatibility = new("1x an item with the Raw Fish Cut tag", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Cooking", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Cooking tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueProgRecipeProduct(Prog: "ItemSeederPreIndustrialFood_Shared_Intermediate_Intermediate_preindustrial_food_boned_fish_fillet_sheet") { Compatibility = new("ItemSeederPreIndustrialFood_Shared_Intermediate_Intermediate_preindustrial_food_boned_fish_fillet_sheet", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingMakeSharedIntermediateIntermediateFruitCatalogueStock() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/make Shared Intermediate intermediate fruit catalogue stock", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["medieval", "renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make Shared Intermediate intermediate fruit catalogue stock",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for preindustrial_food_fruit_stuffing_mix.",
		Action = "preparing intermediate stock",
		ActiveCraftItemSdesc = "a intermediate preparation",
		Gate = CatalogueRecipeGates.PreIndustrialFoodCatalogueProductionCooking10,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Easy,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 3,
		Interruptable = false,
		Phases = [
			new(15, "$0 sort|sorts $i1, removing husks and damaged pieces from the intermediate stock.", "$0 sort|sorts $i1, removing husks and damaged pieces from the intermediate stock.", ExertionLevel.Stasis, 0),
			new(25, "$0 process|processes the intermediate stock into a clean, useful household supply, using $t1.", "$0 process|processes the intermediate stock into a clean, useful household supply, using $t1.", ExertionLevel.Stasis, 0),
			new(20, "$0 weigh|weighs $p1 and store|stores the finished intermediate stock for later work.", "$0 inspect|inspects the poor stock and reserve|reserves it for a lesser use.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityTagRecipeInput(Grams: 1000.0, MaterialTag: "Fruit", PileTag: "Seeded Yield", CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1 kilogram of a material tagged as Fruit", ["piletag Seeded Yield"]), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Cooking", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Cooking tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueProgRecipeProduct(Prog: "ItemSeederPreIndustrialFood_Shared_Intermediate_Intermediate_preindustrial_food_candying_fruit_jar") { Compatibility = new("ItemSeederPreIndustrialFood_Shared_Intermediate_Intermediate_preindustrial_food_candying_fruit_jar", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingMakeSharedIntermediateIntermediateBarleyCatalogueStock() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/make Shared Intermediate intermediate barley catalogue stock", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["medieval", "renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make Shared Intermediate intermediate barley catalogue stock",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for preindustrial_food_grain_fermentation_starter_cake.",
		Action = "preparing intermediate stock",
		ActiveCraftItemSdesc = "a intermediate preparation",
		Gate = CatalogueRecipeGates.PreIndustrialFoodCatalogueProductionMilling10,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Easy,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 3,
		Interruptable = false,
		Phases = [
			new(15, "$0 sort|sorts $i1, removing husks and damaged pieces from the intermediate stock.", "$0 sort|sorts $i1, removing husks and damaged pieces from the intermediate stock.", ExertionLevel.Stasis, 0),
			new(25, "$0 process|processes the intermediate stock into a clean, useful household supply, using $t1.", "$0 process|processes the intermediate stock into a clean, useful household supply, using $t1.", ExertionLevel.Stasis, 0),
			new(20, "$0 weigh|weighs $p1 and store|stores the finished intermediate stock for later work.", "$0 inspect|inspects the poor stock and reserve|reserves it for a lesser use.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityTagRecipeInput(Grams: 1000.0, MaterialTag: "Food Crop", PileTag: "Seeded Yield", CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1 kilogram of a material tagged as Food Crop", ["piletag Seeded Yield"]), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Cooking", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Cooking tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueProgRecipeProduct(Prog: "ItemSeederPreIndustrialFood_Shared_Intermediate_Intermediate_preindustrial_food_grain_fermentation_starter_cake") { Compatibility = new("ItemSeederPreIndustrialFood_Shared_Intermediate_Intermediate_preindustrial_food_grain_fermentation_starter_cake", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingMakeSharedIntermediateIntermediateCornCatalogueStock() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/make Shared Intermediate intermediate corn catalogue stock", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["medieval", "renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make Shared Intermediate intermediate corn catalogue stock",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for preindustrial_food_maize_wrapper_rounds.",
		Action = "preparing intermediate stock",
		ActiveCraftItemSdesc = "a intermediate preparation",
		Gate = CatalogueRecipeGates.PreIndustrialFoodCatalogueProductionCooking10,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Easy,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 3,
		Interruptable = false,
		Phases = [
			new(15, "$0 sort|sorts $i1, removing husks and damaged pieces from the intermediate stock.", "$0 sort|sorts $i1, removing husks and damaged pieces from the intermediate stock.", ExertionLevel.Stasis, 0),
			new(25, "$0 process|processes the intermediate stock into a clean, useful household supply, using $t1.", "$0 process|processes the intermediate stock into a clean, useful household supply, using $t1.", ExertionLevel.Stasis, 0),
			new(20, "$0 weigh|weighs $p1 and store|stores the finished intermediate stock for later work.", "$0 inspect|inspects the poor stock and reserve|reserves it for a lesser use.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityTagRecipeInput(Grams: 1000.0, MaterialTag: "Vegetable", PileTag: "Seeded Yield", CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1 kilogram of a material tagged as Vegetable", ["piletag Seeded Yield"]), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Cooking", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Cooking tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueProgRecipeProduct(Prog: "ItemSeederPreIndustrialFood_Shared_Intermediate_Intermediate_preindustrial_food_maize_dumpling_dough") { Compatibility = new("ItemSeederPreIndustrialFood_Shared_Intermediate_Intermediate_preindustrial_food_maize_dumpling_dough", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingMakeSharedIntermediateIntermediateTreeNutCatalogueStock() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/make Shared Intermediate intermediate tree nut catalogue stock", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["medieval", "renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make Shared Intermediate intermediate tree nut catalogue stock",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for preindustrial_food_nut_stuffing_mix.",
		Action = "preparing intermediate stock",
		ActiveCraftItemSdesc = "a intermediate preparation",
		Gate = CatalogueRecipeGates.PreIndustrialFoodCatalogueProductionCooking10,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Easy,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 3,
		Interruptable = false,
		Phases = [
			new(15, "$0 sort|sorts $i1, removing husks and damaged pieces from the intermediate stock.", "$0 sort|sorts $i1, removing husks and damaged pieces from the intermediate stock.", ExertionLevel.Stasis, 0),
			new(25, "$0 process|processes the intermediate stock into a clean, useful household supply, using $t1.", "$0 process|processes the intermediate stock into a clean, useful household supply, using $t1.", ExertionLevel.Stasis, 0),
			new(20, "$0 weigh|weighs $p1 and store|stores the finished intermediate stock for later work.", "$0 inspect|inspects the poor stock and reserve|reserves it for a lesser use.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityTagRecipeInput(Grams: 1000.0, MaterialTag: "Food Crop", PileTag: "Seeded Yield", CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1 kilogram of a material tagged as Food Crop", ["piletag Seeded Yield"]), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Cooking", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Cooking tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueProgRecipeProduct(Prog: "ItemSeederPreIndustrialFood_Shared_Intermediate_Intermediate_preindustrial_food_honey_nut_filling_portion") { Compatibility = new("ItemSeederPreIndustrialFood_Shared_Intermediate_Intermediate_preindustrial_food_honey_nut_filling_portion", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingMakeSharedIntermediateIntermediateVegetableCatalogueStock() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/make Shared Intermediate intermediate vegetable catalogue stock", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["medieval", "renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make Shared Intermediate intermediate vegetable catalogue stock",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for preindustrial_food_oiled_leaf_food_wrappers.",
		Action = "preparing intermediate stock",
		ActiveCraftItemSdesc = "a intermediate preparation",
		Gate = CatalogueRecipeGates.PreIndustrialFoodCatalogueProductionCooking10,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Easy,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 3,
		Interruptable = false,
		Phases = [
			new(15, "$0 sort|sorts $i1, removing husks and damaged pieces from the intermediate stock.", "$0 sort|sorts $i1, removing husks and damaged pieces from the intermediate stock.", ExertionLevel.Stasis, 0),
			new(25, "$0 process|processes the intermediate stock into a clean, useful household supply, using $t1.", "$0 process|processes the intermediate stock into a clean, useful household supply, using $t1.", ExertionLevel.Stasis, 0),
			new(20, "$0 weigh|weighs $p1 and store|stores the finished intermediate stock for later work.", "$0 inspect|inspects the poor stock and reserve|reserves it for a lesser use.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityTagRecipeInput(Grams: 1000.0, MaterialTag: "Vegetable", PileTag: "Seeded Yield", CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1 kilogram of a material tagged as Vegetable", ["piletag Seeded Yield"]), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Cooking", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Cooking tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueProgRecipeProduct(Prog: "ItemSeederPreIndustrialFood_Shared_Intermediate_Intermediate_preindustrial_food_mushroom_filling_portion") { Compatibility = new("ItemSeederPreIndustrialFood_Shared_Intermediate_Intermediate_preindustrial_food_mushroom_filling_portion", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingMakeSharedIntermediateIntermediateMeatCatalogueStock() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/make Shared Intermediate intermediate meat catalogue stock", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["medieval", "renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make Shared Intermediate intermediate meat catalogue stock",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for preindustrial_food_raw_sausage_links.",
		Action = "preparing intermediate stock",
		ActiveCraftItemSdesc = "a intermediate preparation",
		Gate = CatalogueRecipeGates.PreIndustrialFoodCatalogueProductionButchering10,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Easy,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 3,
		Interruptable = false,
		Phases = [
			new(15, "$0 sort|sorts $i1, removing husks and damaged pieces from the intermediate stock.", "$0 sort|sorts $i1, removing husks and damaged pieces from the intermediate stock.", ExertionLevel.Stasis, 0),
			new(25, "$0 process|processes the intermediate stock into a clean, useful household supply, using $t1.", "$0 process|processes the intermediate stock into a clean, useful household supply, using $t1.", ExertionLevel.Stasis, 0),
			new(20, "$0 weigh|weighs $p1 and store|stores the finished intermediate stock for later work.", "$0 inspect|inspects the poor stock and reserve|reserves it for a lesser use.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueTagRecipeInput(Tag: "Raw Non-Fish Meat Cut", Quantity: 1) { Compatibility = new("1x an item with the Raw Non-Fish Meat Cut tag", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Cooking", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Cooking tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueProgRecipeProduct(Prog: "ItemSeederPreIndustrialFood_Shared_Intermediate_Intermediate_preindustrial_food_brined_meat_joint") { Compatibility = new("ItemSeederPreIndustrialFood_Shared_Intermediate_Intermediate_preindustrial_food_brined_meat_joint", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingMakeSharedIntermediateIntermediateFoodCatalogueStock() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/make Shared Intermediate intermediate food catalogue stock", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["medieval", "renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make Shared Intermediate intermediate food catalogue stock",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for preindustrial_food_rennet_curdling_tablet.",
		Action = "preparing intermediate stock",
		ActiveCraftItemSdesc = "a intermediate preparation",
		Gate = CatalogueRecipeGates.PreIndustrialFoodCatalogueProductionCooking10,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Easy,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 3,
		Interruptable = false,
		Phases = [
			new(15, "$0 sort|sorts $i1, removing husks and damaged pieces from the intermediate stock.", "$0 sort|sorts $i1, removing husks and damaged pieces from the intermediate stock.", ExertionLevel.Stasis, 0),
			new(25, "$0 process|processes the intermediate stock into a clean, useful household supply, using $t1.", "$0 process|processes the intermediate stock into a clean, useful household supply, using $t1.", ExertionLevel.Stasis, 0),
			new(20, "$0 weigh|weighs $p1 and store|stores the finished intermediate stock for later work.", "$0 inspect|inspects the poor stock and reserve|reserves it for a lesser use.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityTagRecipeInput(Grams: 1000.0, MaterialTag: "Food Crop", PileTag: "Seeded Yield", CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1 kilogram of a material tagged as Food Crop", ["piletag Seeded Yield"]), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Cooking", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Cooking tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueProgRecipeProduct(Prog: "ItemSeederPreIndustrialFood_Shared_Intermediate_Intermediate_preindustrial_food_boiling_sugar_mass") { Compatibility = new("ItemSeederPreIndustrialFood_Shared_Intermediate_Intermediate_preindustrial_food_boiling_sugar_mass", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingMakeSharedIntermediateIntermediateDoughCatalogueStock() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/make Shared Intermediate intermediate dough catalogue stock", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["medieval", "renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make Shared Intermediate intermediate dough catalogue stock",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for preindustrial_food_resting_bread_dough_round.",
		Action = "preparing intermediate stock",
		ActiveCraftItemSdesc = "a intermediate preparation",
		Gate = CatalogueRecipeGates.PreIndustrialFoodCatalogueProductionCooking10,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Easy,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 3,
		Interruptable = false,
		Phases = [
			new(15, "$0 sort|sorts $i1, removing husks and damaged pieces from the intermediate stock.", "$0 sort|sorts $i1, removing husks and damaged pieces from the intermediate stock.", ExertionLevel.Stasis, 0),
			new(25, "$0 process|processes the intermediate stock into a clean, useful household supply, using $t1.", "$0 process|processes the intermediate stock into a clean, useful household supply, using $t1.", ExertionLevel.Stasis, 0),
			new(20, "$0 weigh|weighs $p1 and store|stores the finished intermediate stock for later work.", "$0 inspect|inspects the poor stock and reserve|reserves it for a lesser use.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityTagRecipeInput(Grams: 1000.0, MaterialTag: "Food Crop", PileTag: "Seeded Yield", CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1 kilogram of a material tagged as Food Crop", ["piletag Seeded Yield"]), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Cooking", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Cooking tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueProgRecipeProduct(Prog: "ItemSeederPreIndustrialFood_Shared_Intermediate_Intermediate_preindustrial_food_braided_bread_dough") { Compatibility = new("ItemSeederPreIndustrialFood_Shared_Intermediate_Intermediate_preindustrial_food_braided_bread_dough", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingMakeMedievalPreparedGrainSorghumCatalogueStock() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/make Medieval Prepared grain sorghum catalogue stock", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make Medieval Prepared grain sorghum catalogue stock",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for medieval_food_ethiopian_royal_layered_injera.",
		Action = "preparing grain food",
		ActiveCraftItemSdesc = "a grain preparation",
		Gate = CatalogueRecipeGates.PreIndustrialFoodCatalogueProductionMilling15,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 3,
		Interruptable = false,
		Phases = [
			new(15, "$0 sort|sorts $i1 and rinse|rinses the grain before the pot is heated.", "$0 sort|sorts $i1 and rinse|rinses the grain before the pot is heated.", ExertionLevel.Stasis, 0),
			new(25, "$0 simmer|simmers the grain stock until it softens and thickens, using $t1.", "$0 simmer|simmers the grain stock until it softens and thickens, using $t1.", ExertionLevel.Stasis, 0),
			new(20, "$0 taste|tastes $p1, season|seasons it, and set|sets it aside to serve.", "$0 taste|tastes the thin result and reserve|reserves it as animal feed.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityTagRecipeInput(Grams: 1000.0, MaterialTag: "Food Crop", PileTag: "Seeded Yield", CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1 kilogram of a material tagged as Food Crop", ["piletag Seeded Yield"]), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Cooking", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Cooking tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueProgCookedRecipeProduct(Prog: "ItemSeederPreIndustrialFood_Medieval_Prepared_Grain_medieval_food_ethiopian_royal_layered_injera", RemoveDrugsAndFoodEffects: false, Ingredients: []) { Compatibility = new("ItemSeederPreIndustrialFood_Medieval_Prepared_Grain_medieval_food_ethiopian_royal_layered_injera", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingMakeMedievalPreparedGrainRiceCatalogueStock() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/make Medieval Prepared grain rice catalogue stock", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make Medieval Prepared grain rice catalogue stock",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for medieval_food_infirmary_almond_rice.",
		Action = "preparing grain food",
		ActiveCraftItemSdesc = "a grain preparation",
		Gate = CatalogueRecipeGates.PreIndustrialFoodCatalogueProductionMilling15,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 3,
		Interruptable = false,
		Phases = [
			new(15, "$0 sort|sorts $i1 and rinse|rinses the grain before the pot is heated.", "$0 sort|sorts $i1 and rinse|rinses the grain before the pot is heated.", ExertionLevel.Stasis, 0),
			new(25, "$0 simmer|simmers the grain stock until it softens and thickens, using $t1.", "$0 simmer|simmers the grain stock until it softens and thickens, using $t1.", ExertionLevel.Stasis, 0),
			new(20, "$0 taste|tastes $p1, season|seasons it, and set|sets it aside to serve.", "$0 taste|tastes the thin result and reserve|reserves it as animal feed.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityTagRecipeInput(Grams: 1000.0, MaterialTag: "Food Crop", PileTag: "Seeded Yield", CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1 kilogram of a material tagged as Food Crop", ["piletag Seeded Yield"]), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Cooking", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Cooking tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueProgCookedRecipeProduct(Prog: "ItemSeederPreIndustrialFood_Medieval_Prepared_Grain_medieval_food_appam_fermented_bowl_cake", RemoveDrugsAndFoodEffects: false, Ingredients: []) { Compatibility = new("ItemSeederPreIndustrialFood_Medieval_Prepared_Grain_medieval_food_appam_fermented_bowl_cake", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingMakeMedievalPreparedGrainWheatCatalogueStock() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/make Medieval Prepared grain wheat catalogue stock", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make Medieval Prepared grain wheat catalogue stock",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for medieval_food_saffroned_frumenty_with_meat.",
		Action = "preparing grain food",
		ActiveCraftItemSdesc = "a grain preparation",
		Gate = CatalogueRecipeGates.PreIndustrialFoodCatalogueProductionMilling15,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 3,
		Interruptable = false,
		Phases = [
			new(15, "$0 sort|sorts $i1 and rinse|rinses the grain before the pot is heated.", "$0 sort|sorts $i1 and rinse|rinses the grain before the pot is heated.", ExertionLevel.Stasis, 0),
			new(25, "$0 simmer|simmers the grain stock until it softens and thickens, using $t1.", "$0 simmer|simmers the grain stock until it softens and thickens, using $t1.", ExertionLevel.Stasis, 0),
			new(20, "$0 taste|tastes $p1, season|seasons it, and set|sets it aside to serve.", "$0 taste|tastes the thin result and reserve|reserves it as animal feed.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityTagRecipeInput(Grams: 1000.0, MaterialTag: "Food Crop", PileTag: "Seeded Yield", CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1 kilogram of a material tagged as Food Crop", ["piletag Seeded Yield"]), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Cooking", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Cooking tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueProgCookedRecipeProduct(Prog: "ItemSeederPreIndustrialFood_Medieval_Prepared_Grain_medieval_food_almond_frumenty_for_lent", RemoveDrugsAndFoodEffects: false, Ingredients: []) { Compatibility = new("ItemSeederPreIndustrialFood_Medieval_Prepared_Grain_medieval_food_almond_frumenty_for_lent", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingMakeMedievalPreparedGrainMilletCatalogueStock() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/make Medieval Prepared grain millet catalogue stock", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make Medieval Prepared grain millet catalogue stock",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for medieval_food_sahel_court_millet_couscous.",
		Action = "preparing grain food",
		ActiveCraftItemSdesc = "a grain preparation",
		Gate = CatalogueRecipeGates.PreIndustrialFoodCatalogueProductionMilling15,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 3,
		Interruptable = false,
		Phases = [
			new(15, "$0 sort|sorts $i1 and rinse|rinses the grain before the pot is heated.", "$0 sort|sorts $i1 and rinse|rinses the grain before the pot is heated.", ExertionLevel.Stasis, 0),
			new(25, "$0 simmer|simmers the grain stock until it softens and thickens, using $t1.", "$0 simmer|simmers the grain stock until it softens and thickens, using $t1.", ExertionLevel.Stasis, 0),
			new(20, "$0 taste|tastes $p1, season|seasons it, and set|sets it aside to serve.", "$0 taste|tastes the thin result and reserve|reserves it as animal feed.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityTagRecipeInput(Grams: 1000.0, MaterialTag: "Food Crop", PileTag: "Seeded Yield", CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1 kilogram of a material tagged as Food Crop", ["piletag Seeded Yield"]), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Cooking", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Cooking tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueProgCookedRecipeProduct(Prog: "ItemSeederPreIndustrialFood_Medieval_Prepared_Grain_medieval_food_sahel_court_millet_couscous", RemoveDrugsAndFoodEffects: false, Ingredients: []) { Compatibility = new("ItemSeederPreIndustrialFood_Medieval_Prepared_Grain_medieval_food_sahel_court_millet_couscous", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingMakeMedievalPreparedBreadBreadCatalogueStock() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/make Medieval Prepared bread bread catalogue stock", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make Medieval Prepared bread bread catalogue stock",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for medieval_food_spiced_wine_trencher_sops.",
		Action = "preparing bread food",
		ActiveCraftItemSdesc = "a bread preparation",
		Gate = CatalogueRecipeGates.PreIndustrialFoodCatalogueProductionBaking15,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 3,
		Interruptable = false,
		Phases = [
			new(15, "$0 sift|sifts $i1 and work|works the grain into a pliable bread dough.", "$0 sift|sifts $i1 and work|works the grain into a pliable bread dough.", ExertionLevel.Stasis, 0),
			new(25, "$0 knead|kneads the dough, shape|shapes it, and set|sets it near the heat, using $t1.", "$0 knead|kneads the dough, shape|shapes it, and set|sets it near the heat, using $t1.", ExertionLevel.Stasis, 0),
			new(20, "$0 bake|bakes the loaf and set|sets $p1 aside to cool before serving.", "$0 open|opens the oven to a collapsed loaf and salvage|salvages the crust.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityTagRecipeInput(Grams: 1000.0, MaterialTag: "Food Crop", PileTag: "Seeded Yield", CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1 kilogram of a material tagged as Food Crop", ["piletag Seeded Yield"]), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Cooking", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Cooking tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueProgCookedRecipeProduct(Prog: "ItemSeederPreIndustrialFood_Medieval_Prepared_Bread_medieval_food_castle_shaped_bread_subtlety", RemoveDrugsAndFoodEffects: false, Ingredients: []) { Compatibility = new("ItemSeederPreIndustrialFood_Medieval_Prepared_Bread_medieval_food_castle_shaped_bread_subtlety", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingMakeMedievalPreparedNoodleWheatCatalogueStock() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/make Medieval Prepared noodle wheat catalogue stock", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make Medieval Prepared noodle wheat catalogue stock",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for medieval_food_itriyya_meat_broth_noodles.",
		Action = "preparing noodle food",
		ActiveCraftItemSdesc = "a noodle preparation",
		Gate = CatalogueRecipeGates.PreIndustrialFoodCatalogueProductionBaking152,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 3,
		Interruptable = false,
		Phases = [
			new(15, "$0 mix|mixes $i1 with water and work|works it into a firm dough.", "$0 mix|mixes $i1 with water and work|works it into a firm dough.", ExertionLevel.Stasis, 0),
			new(25, "$0 roll|rolls and cut|cuts the dough into the noodle pieces, using $t1.", "$0 roll|rolls and cut|cuts the dough into the noodle pieces, using $t1.", ExertionLevel.Stasis, 0),
			new(20, "$0 boil|boils $p1 until tender and drain|drains it for the table.", "$0 overcook|overcooks the pieces and save|saves the broken bits for soup.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityTagRecipeInput(Grams: 1000.0, MaterialTag: "Food Crop", PileTag: "Seeded Yield", CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1 kilogram of a material tagged as Food Crop", ["piletag Seeded Yield"]), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Cooking", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Cooking tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueProgCookedRecipeProduct(Prog: "ItemSeederPreIndustrialFood_Medieval_Prepared_Noodle_medieval_food_buddhist_mushroom_noodle_bowl", RemoveDrugsAndFoodEffects: false, Ingredients: []) { Compatibility = new("ItemSeederPreIndustrialFood_Medieval_Prepared_Noodle_medieval_food_buddhist_mushroom_noodle_bowl", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingMakeMedievalPreparedNoodleFoodCatalogueStock() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/make Medieval Prepared noodle food catalogue stock", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make Medieval Prepared noodle food catalogue stock",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for medieval_food_medieval_pulse_flour_noodle_bowl.",
		Action = "preparing noodle food",
		ActiveCraftItemSdesc = "a noodle preparation",
		Gate = CatalogueRecipeGates.PreIndustrialFoodCatalogueProductionBaking152,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 3,
		Interruptable = false,
		Phases = [
			new(15, "$0 mix|mixes $i1 with water and work|works it into a firm dough.", "$0 mix|mixes $i1 with water and work|works it into a firm dough.", ExertionLevel.Stasis, 0),
			new(25, "$0 roll|rolls and cut|cuts the dough into the noodle pieces, using $t1.", "$0 roll|rolls and cut|cuts the dough into the noodle pieces, using $t1.", ExertionLevel.Stasis, 0),
			new(20, "$0 boil|boils $p1 until tender and drain|drains it for the table.", "$0 overcook|overcooks the pieces and save|saves the broken bits for soup.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityTagRecipeInput(Grams: 1000.0, MaterialTag: "Food Crop", PileTag: "Seeded Yield", CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1 kilogram of a material tagged as Food Crop", ["piletag Seeded Yield"]), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Cooking", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Cooking tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueProgCookedRecipeProduct(Prog: "ItemSeederPreIndustrialFood_Medieval_Prepared_Noodle_medieval_food_medieval_pulse_flour_noodle_bowl", RemoveDrugsAndFoodEffects: false, Ingredients: []) { Compatibility = new("ItemSeederPreIndustrialFood_Medieval_Prepared_Noodle_medieval_food_medieval_pulse_flour_noodle_bowl", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingMakeMedievalPreparedDumplingVegetableCatalogueStock() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/make Medieval Prepared dumpling vegetable catalogue stock", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make Medieval Prepared dumpling vegetable catalogue stock",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for medieval_food_buddhist_vegetable_baozi.",
		Action = "preparing dumpling food",
		ActiveCraftItemSdesc = "a dumpling preparation",
		Gate = CatalogueRecipeGates.PreIndustrialFoodCatalogueProductionBaking153,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 3,
		Interruptable = false,
		Phases = [
			new(15, "$0 mix|mixes $i1 with water and work|works it into a firm dough.", "$0 mix|mixes $i1 with water and work|works it into a firm dough.", ExertionLevel.Stasis, 0),
			new(25, "$0 roll|rolls and cut|cuts the dough into the dumpling pieces, using $t1.", "$0 roll|rolls and cut|cuts the dough into the dumpling pieces, using $t1.", ExertionLevel.Stasis, 0),
			new(20, "$0 boil|boils $p1 until tender and drain|drains it for the table.", "$0 overcook|overcooks the pieces and save|saves the broken bits for soup.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityTagRecipeInput(Grams: 1000.0, MaterialTag: "Vegetable", PileTag: "Seeded Yield", CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1 kilogram of a material tagged as Vegetable", ["piletag Seeded Yield"]), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Cooking", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Cooking tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueProgCookedRecipeProduct(Prog: "ItemSeederPreIndustrialFood_Medieval_Prepared_Dumpling_medieval_food_buddhist_vegetable_baozi", RemoveDrugsAndFoodEffects: false, Ingredients: []) { Compatibility = new("ItemSeederPreIndustrialFood_Medieval_Prepared_Dumpling_medieval_food_buddhist_vegetable_baozi", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingMakeMedievalPreparedDumplingMeatCatalogueStock() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/make Medieval Prepared dumpling meat catalogue stock", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make Medieval Prepared dumpling meat catalogue stock",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for medieval_food_shishbarak_meat_dumplings.",
		Action = "preparing dumpling food",
		ActiveCraftItemSdesc = "a dumpling preparation",
		Gate = CatalogueRecipeGates.PreIndustrialFoodCatalogueProductionButchering1510,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 3,
		Interruptable = false,
		Phases = [
			new(15, "$0 mix|mixes $i1 with water and work|works it into a firm dough.", "$0 mix|mixes $i1 with water and work|works it into a firm dough.", ExertionLevel.Stasis, 0),
			new(25, "$0 roll|rolls and cut|cuts the dough into the dumpling pieces, using $t1.", "$0 roll|rolls and cut|cuts the dough into the dumpling pieces, using $t1.", ExertionLevel.Stasis, 0),
			new(20, "$0 boil|boils $p1 until tender and drain|drains it for the table.", "$0 overcook|overcooks the pieces and save|saves the broken bits for soup.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueTagRecipeInput(Tag: "Raw Non-Fish Meat Cut", Quantity: 1) { Compatibility = new("1x an item with the Raw Non-Fish Meat Cut tag", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Cooking", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Cooking tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueProgCookedRecipeProduct(Prog: "ItemSeederPreIndustrialFood_Medieval_Prepared_Dumpling_medieval_food_manti_yogurt_dumpling_platter", RemoveDrugsAndFoodEffects: false, Ingredients: []) { Compatibility = new("ItemSeederPreIndustrialFood_Medieval_Prepared_Dumpling_medieval_food_manti_yogurt_dumpling_platter", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingMakeMedievalPreparedPulseFoodCatalogueStock() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/make Medieval Prepared pulse food catalogue stock", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make Medieval Prepared pulse food catalogue stock",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for medieval_food_monastic_fasting_pease_pudding.",
		Action = "preparing pulse food",
		ActiveCraftItemSdesc = "a pulse preparation",
		Gate = CatalogueRecipeGates.PreIndustrialFoodCatalogueProductionMilling153,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 3,
		Interruptable = false,
		Phases = [
			new(15, "$0 sort|sorts $i1 and rinse|rinses the grain before the pot is heated.", "$0 sort|sorts $i1 and rinse|rinses the grain before the pot is heated.", ExertionLevel.Stasis, 0),
			new(25, "$0 simmer|simmers the grain stock until it softens and thickens, using $t1.", "$0 simmer|simmers the grain stock until it softens and thickens, using $t1.", ExertionLevel.Stasis, 0),
			new(20, "$0 taste|tastes $p1, season|seasons it, and set|sets it aside to serve.", "$0 taste|tastes the thin result and reserve|reserves it as animal feed.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityTagRecipeInput(Grams: 1000.0, MaterialTag: "Food Crop", PileTag: "Seeded Yield", CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1 kilogram of a material tagged as Food Crop", ["piletag Seeded Yield"]), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Cooking", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Cooking tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueProgCookedRecipeProduct(Prog: "ItemSeederPreIndustrialFood_Medieval_Prepared_Pulse_medieval_food_byzantine_monastic_lentil_paste", RemoveDrugsAndFoodEffects: false, Ingredients: []) { Compatibility = new("ItemSeederPreIndustrialFood_Medieval_Prepared_Pulse_medieval_food_byzantine_monastic_lentil_paste", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingMakeMedievalPreparedVegetableVegetableCatalogueStock() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/make Medieval Prepared vegetable vegetable catalogue stock", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make Medieval Prepared vegetable vegetable catalogue stock",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for medieval_food_buddhist_mock_duck_roll.",
		Action = "preparing vegetable food",
		ActiveCraftItemSdesc = "a vegetable preparation",
		Gate = CatalogueRecipeGates.PreIndustrialFoodCatalogueProductionCooking15,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 3,
		Interruptable = false,
		Phases = [
			new(15, "$0 trim|trims $i1 and cut|cuts away the tough ends.", "$0 trim|trims $i1 and cut|cuts away the tough ends.", ExertionLevel.Stasis, 0),
			new(25, "$0 chop|chops the vegetables and cook|cooks them until their colour softens, using $t1.", "$0 chop|chops the vegetables and cook|cooks them until their colour softens, using $t1.", ExertionLevel.Stasis, 0),
			new(20, "$0 fold|folds $p1 into the serving dish and season|seasons it to taste.", "$0 taste|tastes the failed vegetables and reserve|reserves them for broth.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityTagRecipeInput(Grams: 1000.0, MaterialTag: "Vegetable", PileTag: "Seeded Yield", CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1 kilogram of a material tagged as Vegetable", ["piletag Seeded Yield"]), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Cooking", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Cooking tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueProgCookedRecipeProduct(Prog: "ItemSeederPreIndustrialFood_Medieval_Prepared_Vegetable_medieval_food_buddhist_mock_duck_roll", RemoveDrugsAndFoodEffects: false, Ingredients: []) { Compatibility = new("ItemSeederPreIndustrialFood_Medieval_Prepared_Vegetable_medieval_food_buddhist_mock_duck_roll", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingMakeMedievalPreparedVegetableRiceCatalogueStock() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/make Medieval Prepared vegetable rice catalogue stock", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make Medieval Prepared vegetable rice catalogue stock",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for medieval_food_byzantine_fasting_stuffed_leaves.",
		Action = "preparing vegetable food",
		ActiveCraftItemSdesc = "a vegetable preparation",
		Gate = CatalogueRecipeGates.PreIndustrialFoodCatalogueProductionMilling1511,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 3,
		Interruptable = false,
		Phases = [
			new(15, "$0 trim|trims $i1 and cut|cuts away the tough ends.", "$0 trim|trims $i1 and cut|cuts away the tough ends.", ExertionLevel.Stasis, 0),
			new(25, "$0 chop|chops the vegetables and cook|cooks them until their colour softens, using $t1.", "$0 chop|chops the vegetables and cook|cooks them until their colour softens, using $t1.", ExertionLevel.Stasis, 0),
			new(20, "$0 fold|folds $p1 into the serving dish and season|seasons it to taste.", "$0 taste|tastes the failed vegetables and reserve|reserves them for broth.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityTagRecipeInput(Grams: 1000.0, MaterialTag: "Food Crop", PileTag: "Seeded Yield", CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1 kilogram of a material tagged as Food Crop", ["piletag Seeded Yield"]), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Cooking", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Cooking tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueProgCookedRecipeProduct(Prog: "ItemSeederPreIndustrialFood_Medieval_Prepared_Vegetable_medieval_food_byzantine_fasting_stuffed_leaves", RemoveDrugsAndFoodEffects: false, Ingredients: []) { Compatibility = new("ItemSeederPreIndustrialFood_Medieval_Prepared_Vegetable_medieval_food_byzantine_fasting_stuffed_leaves", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingMakeMedievalPreparedVegetableSorghumCatalogueStock() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/make Medieval Prepared vegetable sorghum catalogue stock", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make Medieval Prepared vegetable sorghum catalogue stock",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for medieval_food_ethiopian_christian_fast_platter.",
		Action = "preparing vegetable food",
		ActiveCraftItemSdesc = "a vegetable preparation",
		Gate = CatalogueRecipeGates.PreIndustrialFoodCatalogueProductionMilling1511,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 3,
		Interruptable = false,
		Phases = [
			new(15, "$0 trim|trims $i1 and cut|cuts away the tough ends.", "$0 trim|trims $i1 and cut|cuts away the tough ends.", ExertionLevel.Stasis, 0),
			new(25, "$0 chop|chops the vegetables and cook|cooks them until their colour softens, using $t1.", "$0 chop|chops the vegetables and cook|cooks them until their colour softens, using $t1.", ExertionLevel.Stasis, 0),
			new(20, "$0 fold|folds $p1 into the serving dish and season|seasons it to taste.", "$0 taste|tastes the failed vegetables and reserve|reserves them for broth.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityTagRecipeInput(Grams: 1000.0, MaterialTag: "Food Crop", PileTag: "Seeded Yield", CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1 kilogram of a material tagged as Food Crop", ["piletag Seeded Yield"]), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Cooking", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Cooking tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueProgCookedRecipeProduct(Prog: "ItemSeederPreIndustrialFood_Medieval_Prepared_Vegetable_medieval_food_ethiopian_christian_fast_platter", RemoveDrugsAndFoodEffects: false, Ingredients: []) { Compatibility = new("ItemSeederPreIndustrialFood_Medieval_Prepared_Vegetable_medieval_food_ethiopian_christian_fast_platter", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingMakeMedievalPreparedVegetableBreadCatalogueStock() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/make Medieval Prepared vegetable bread catalogue stock", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make Medieval Prepared vegetable bread catalogue stock",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for medieval_food_fasting_vegetable_tharid.",
		Action = "preparing vegetable food",
		ActiveCraftItemSdesc = "a vegetable preparation",
		Gate = CatalogueRecipeGates.PreIndustrialFoodCatalogueProductionCooking15,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 3,
		Interruptable = false,
		Phases = [
			new(15, "$0 trim|trims $i1 and cut|cuts away the tough ends.", "$0 trim|trims $i1 and cut|cuts away the tough ends.", ExertionLevel.Stasis, 0),
			new(25, "$0 chop|chops the vegetables and cook|cooks them until their colour softens, using $t1.", "$0 chop|chops the vegetables and cook|cooks them until their colour softens, using $t1.", ExertionLevel.Stasis, 0),
			new(20, "$0 fold|folds $p1 into the serving dish and season|seasons it to taste.", "$0 taste|tastes the failed vegetables and reserve|reserves them for broth.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityTagRecipeInput(Grams: 1000.0, MaterialTag: "Food Crop", PileTag: "Seeded Yield", CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1 kilogram of a material tagged as Food Crop", ["piletag Seeded Yield"]), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Cooking", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Cooking tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueProgCookedRecipeProduct(Prog: "ItemSeederPreIndustrialFood_Medieval_Prepared_Vegetable_medieval_food_bazmaward_fasting_roll", RemoveDrugsAndFoodEffects: false, Ingredients: []) { Compatibility = new("ItemSeederPreIndustrialFood_Medieval_Prepared_Vegetable_medieval_food_bazmaward_fasting_roll", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingMakeMedievalPreparedVegetableFoodCatalogueStock() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/make Medieval Prepared vegetable food catalogue stock", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make Medieval Prepared vegetable food catalogue stock",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for medieval_food_japanese_zen_sesame_tofu.",
		Action = "preparing vegetable food",
		ActiveCraftItemSdesc = "a vegetable preparation",
		Gate = CatalogueRecipeGates.PreIndustrialFoodCatalogueProductionCooking15,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 3,
		Interruptable = false,
		Phases = [
			new(15, "$0 trim|trims $i1 and cut|cuts away the tough ends.", "$0 trim|trims $i1 and cut|cuts away the tough ends.", ExertionLevel.Stasis, 0),
			new(25, "$0 chop|chops the vegetables and cook|cooks them until their colour softens, using $t1.", "$0 chop|chops the vegetables and cook|cooks them until their colour softens, using $t1.", ExertionLevel.Stasis, 0),
			new(20, "$0 fold|folds $p1 into the serving dish and season|seasons it to taste.", "$0 taste|tastes the failed vegetables and reserve|reserves them for broth.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityTagRecipeInput(Grams: 1000.0, MaterialTag: "Food Crop", PileTag: "Seeded Yield", CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1 kilogram of a material tagged as Food Crop", ["piletag Seeded Yield"]), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Cooking", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Cooking tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueProgCookedRecipeProduct(Prog: "ItemSeederPreIndustrialFood_Medieval_Prepared_Vegetable_medieval_food_japanese_zen_sesame_tofu", RemoveDrugsAndFoodEffects: false, Ingredients: []) { Compatibility = new("ItemSeederPreIndustrialFood_Medieval_Prepared_Vegetable_medieval_food_japanese_zen_sesame_tofu", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingMakeMedievalPreparedVegetableWheatCatalogueStock() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/make Medieval Prepared vegetable wheat catalogue stock", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make Medieval Prepared vegetable wheat catalogue stock",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for medieval_food_song_braised_gluten_mock_meat.",
		Action = "preparing vegetable food",
		ActiveCraftItemSdesc = "a vegetable preparation",
		Gate = CatalogueRecipeGates.PreIndustrialFoodCatalogueProductionMilling1511,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 3,
		Interruptable = false,
		Phases = [
			new(15, "$0 trim|trims $i1 and cut|cuts away the tough ends.", "$0 trim|trims $i1 and cut|cuts away the tough ends.", ExertionLevel.Stasis, 0),
			new(25, "$0 chop|chops the vegetables and cook|cooks them until their colour softens, using $t1.", "$0 chop|chops the vegetables and cook|cooks them until their colour softens, using $t1.", ExertionLevel.Stasis, 0),
			new(20, "$0 fold|folds $p1 into the serving dish and season|seasons it to taste.", "$0 taste|tastes the failed vegetables and reserve|reserves them for broth.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityTagRecipeInput(Grams: 1000.0, MaterialTag: "Food Crop", PileTag: "Seeded Yield", CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1 kilogram of a material tagged as Food Crop", ["piletag Seeded Yield"]), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Cooking", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Cooking tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueProgCookedRecipeProduct(Prog: "ItemSeederPreIndustrialFood_Medieval_Prepared_Vegetable_medieval_food_song_braised_gluten_mock_meat", RemoveDrugsAndFoodEffects: false, Ingredients: []) { Compatibility = new("ItemSeederPreIndustrialFood_Medieval_Prepared_Vegetable_medieval_food_song_braised_gluten_mock_meat", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingMakeMedievalPreparedSoupVegetableCatalogueStock() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/make Medieval Prepared soup vegetable catalogue stock", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make Medieval Prepared soup vegetable catalogue stock",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for medieval_food_monastic_almond_pottage.",
		Action = "preparing soup food",
		ActiveCraftItemSdesc = "a soup preparation",
		Gate = CatalogueRecipeGates.PreIndustrialFoodCatalogueProductionCooking152,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 3,
		Interruptable = false,
		Phases = [
			new(15, "$0 chop|chops $i1 and lay|lays the pieces beside the cooking pot.", "$0 chop|chops $i1 and lay|lays the pieces beside the cooking pot.", ExertionLevel.Stasis, 0),
			new(25, "$0 simmer|simmers the soup slowly, stirring so the bottom does not catch, using $t1.", "$0 simmer|simmers the soup slowly, stirring so the bottom does not catch, using $t1.", ExertionLevel.Stasis, 0),
			new(20, "$0 taste|tastes $p1, season|seasons it, and send|sends it to the table.", "$0 taste|tastes the weak soup and reserve|reserves it for another pot.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityTagRecipeInput(Grams: 1000.0, MaterialTag: "Vegetable", PileTag: "Seeded Yield", CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1 kilogram of a material tagged as Vegetable", ["piletag Seeded Yield"]), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Cooking", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Cooking tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueProgCookedRecipeProduct(Prog: "ItemSeederPreIndustrialFood_Medieval_Prepared_Soup_medieval_food_monastic_almond_pottage", RemoveDrugsAndFoodEffects: false, Ingredients: []) { Compatibility = new("ItemSeederPreIndustrialFood_Medieval_Prepared_Soup_medieval_food_monastic_almond_pottage", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingMakeMedievalPreparedSoupMeatCatalogueStock() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/make Medieval Prepared soup meat catalogue stock", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make Medieval Prepared soup meat catalogue stock",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for medieval_food_song_clear_wonton_soup.",
		Action = "preparing soup food",
		ActiveCraftItemSdesc = "a soup preparation",
		Gate = CatalogueRecipeGates.PreIndustrialFoodCatalogueProductionButchering15,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 3,
		Interruptable = false,
		Phases = [
			new(15, "$0 chop|chops $i1 and lay|lays the pieces beside the cooking pot.", "$0 chop|chops $i1 and lay|lays the pieces beside the cooking pot.", ExertionLevel.Stasis, 0),
			new(25, "$0 simmer|simmers the soup slowly, stirring so the bottom does not catch, using $t1.", "$0 simmer|simmers the soup slowly, stirring so the bottom does not catch, using $t1.", ExertionLevel.Stasis, 0),
			new(20, "$0 taste|tastes $p1, season|seasons it, and send|sends it to the table.", "$0 taste|tastes the weak soup and reserve|reserves it for another pot.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueTagRecipeInput(Tag: "Raw Non-Fish Meat Cut", Quantity: 1) { Compatibility = new("1x an item with the Raw Non-Fish Meat Cut tag", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Cooking", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Cooking tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueProgCookedRecipeProduct(Prog: "ItemSeederPreIndustrialFood_Medieval_Prepared_Soup_medieval_food_korean_mandu_broth", RemoveDrugsAndFoodEffects: false, Ingredients: []) { Compatibility = new("ItemSeederPreIndustrialFood_Medieval_Prepared_Soup_medieval_food_korean_mandu_broth", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingMakeMedievalPreparedStewMeatCatalogueStock() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/make Medieval Prepared stew meat catalogue stock", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make Medieval Prepared stew meat catalogue stock",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for medieval_food_hospital_pilgrim_stew.",
		Action = "preparing stew food",
		ActiveCraftItemSdesc = "a stew preparation",
		Gate = CatalogueRecipeGates.PreIndustrialFoodCatalogueProductionButchering152,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 3,
		Interruptable = false,
		Phases = [
			new(15, "$0 chop|chops $i1 and lay|lays the pieces beside the cooking pot.", "$0 chop|chops $i1 and lay|lays the pieces beside the cooking pot.", ExertionLevel.Stasis, 0),
			new(25, "$0 simmer|simmers the stew slowly, stirring so the bottom does not catch, using $t1.", "$0 simmer|simmers the stew slowly, stirring so the bottom does not catch, using $t1.", ExertionLevel.Stasis, 0),
			new(20, "$0 taste|tastes $p1, season|seasons it, and send|sends it to the table.", "$0 taste|tastes the weak stew and reserve|reserves it for another pot.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueTagRecipeInput(Tag: "Raw Non-Fish Meat Cut", Quantity: 1) { Compatibility = new("1x an item with the Raw Non-Fish Meat Cut tag", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Cooking", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Cooking tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueProgCookedRecipeProduct(Prog: "ItemSeederPreIndustrialFood_Medieval_Prepared_Stew_medieval_food_guild_feast_pottage", RemoveDrugsAndFoodEffects: false, Ingredients: []) { Compatibility = new("ItemSeederPreIndustrialFood_Medieval_Prepared_Stew_medieval_food_guild_feast_pottage", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingMakeMedievalPreparedStewBreadCatalogueStock() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/make Medieval Prepared stew bread catalogue stock", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make Medieval Prepared stew bread catalogue stock",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for medieval_food_pottage_filled_trencher.",
		Action = "preparing stew food",
		ActiveCraftItemSdesc = "a stew preparation",
		Gate = CatalogueRecipeGates.PreIndustrialFoodCatalogueProductionCooking153,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 3,
		Interruptable = false,
		Phases = [
			new(15, "$0 chop|chops $i1 and lay|lays the pieces beside the cooking pot.", "$0 chop|chops $i1 and lay|lays the pieces beside the cooking pot.", ExertionLevel.Stasis, 0),
			new(25, "$0 simmer|simmers the stew slowly, stirring so the bottom does not catch, using $t1.", "$0 simmer|simmers the stew slowly, stirring so the bottom does not catch, using $t1.", ExertionLevel.Stasis, 0),
			new(20, "$0 taste|tastes $p1, season|seasons it, and send|sends it to the table.", "$0 taste|tastes the weak stew and reserve|reserves it for another pot.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityTagRecipeInput(Grams: 1000.0, MaterialTag: "Food Crop", PileTag: "Seeded Yield", CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1 kilogram of a material tagged as Food Crop", ["piletag Seeded Yield"]), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Cooking", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Cooking tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueProgCookedRecipeProduct(Prog: "ItemSeederPreIndustrialFood_Medieval_Prepared_Stew_medieval_food_alms_house_bread_pottage", RemoveDrugsAndFoodEffects: false, Ingredients: []) { Compatibility = new("ItemSeederPreIndustrialFood_Medieval_Prepared_Stew_medieval_food_alms_house_bread_pottage", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingMakeMedievalPreparedMeatMeatCatalogueStock() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/make Medieval Prepared meat meat catalogue stock", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make Medieval Prepared meat meat catalogue stock",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for medieval_food_almond_milk_mortrews.",
		Action = "preparing meat food",
		ActiveCraftItemSdesc = "a meat preparation",
		Gate = CatalogueRecipeGates.PreIndustrialFoodCatalogueProductionButchering153,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 3,
		Interruptable = false,
		Phases = [
			new(15, "$0 trim|trims $i1 and portion|portions the meat for the hearth.", "$0 trim|trims $i1 and portion|portions the meat for the hearth.", ExertionLevel.Stasis, 0),
			new(25, "$0 sear|sears the meat, then cook|cooks it through over a steady heat, using $t1.", "$0 sear|sears the meat, then cook|cooks it through over a steady heat, using $t1.", ExertionLevel.Stasis, 0),
			new(20, "$0 rest|rests $p1, then slice|slices it for the meal.", "$0 pierce|pierces the tough result and reserve|reserves it for a stew.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueTagRecipeInput(Tag: "Raw Non-Fish Meat Cut", Quantity: 1) { Compatibility = new("1x an item with the Raw Non-Fish Meat Cut tag", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Cooking", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Cooking tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueProgCookedRecipeProduct(Prog: "ItemSeederPreIndustrialFood_Medieval_Prepared_Meat_medieval_food_almond_lamb_harisa", RemoveDrugsAndFoodEffects: false, Ingredients: []) { Compatibility = new("ItemSeederPreIndustrialFood_Medieval_Prepared_Meat_medieval_food_almond_lamb_harisa", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingMakeMedievalPreparedMeatBreadCatalogueStock() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/make Medieval Prepared meat bread catalogue stock", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make Medieval Prepared meat bread catalogue stock",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for medieval_food_bread_trencher_meat_sops.",
		Action = "preparing meat food",
		ActiveCraftItemSdesc = "a meat preparation",
		Gate = CatalogueRecipeGates.PreIndustrialFoodCatalogueProductionButchering153,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 3,
		Interruptable = false,
		Phases = [
			new(15, "$0 trim|trims $i1 and portion|portions the meat for the hearth.", "$0 trim|trims $i1 and portion|portions the meat for the hearth.", ExertionLevel.Stasis, 0),
			new(25, "$0 sear|sears the meat, then cook|cooks it through over a steady heat, using $t1.", "$0 sear|sears the meat, then cook|cooks it through over a steady heat, using $t1.", ExertionLevel.Stasis, 0),
			new(20, "$0 rest|rests $p1, then slice|slices it for the meal.", "$0 pierce|pierces the tough result and reserve|reserves it for a stew.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueTagRecipeInput(Tag: "Raw Non-Fish Meat Cut", Quantity: 1) { Compatibility = new("1x an item with the Raw Non-Fish Meat Cut tag", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Cooking", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Cooking tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueProgCookedRecipeProduct(Prog: "ItemSeederPreIndustrialFood_Medieval_Prepared_Meat_medieval_food_abbasid_bread_tharid", RemoveDrugsAndFoodEffects: false, Ingredients: []) { Compatibility = new("ItemSeederPreIndustrialFood_Medieval_Prepared_Meat_medieval_food_abbasid_bread_tharid", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingMakeMedievalPreparedPoultryMeatCatalogueStock() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/make Medieval Prepared poultry meat catalogue stock", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make Medieval Prepared poultry meat catalogue stock",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for medieval_food_chicken_rice_blancmanger.",
		Action = "preparing poultry food",
		ActiveCraftItemSdesc = "a poultry preparation",
		Gate = CatalogueRecipeGates.PreIndustrialFoodCatalogueProductionButchering154,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 3,
		Interruptable = false,
		Phases = [
			new(15, "$0 trim|trims $i1 and portion|portions the meat for the hearth.", "$0 trim|trims $i1 and portion|portions the meat for the hearth.", ExertionLevel.Stasis, 0),
			new(25, "$0 sear|sears the meat, then cook|cooks it through over a steady heat, using $t1.", "$0 sear|sears the meat, then cook|cooks it through over a steady heat, using $t1.", ExertionLevel.Stasis, 0),
			new(20, "$0 rest|rests $p1, then slice|slices it for the meal.", "$0 pierce|pierces the tough result and reserve|reserves it for a stew.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueTagRecipeInput(Tag: "Raw Non-Fish Meat Cut", Quantity: 1) { Compatibility = new("1x an item with the Raw Non-Fish Meat Cut tag", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Cooking", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Cooking tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueProgCookedRecipeProduct(Prog: "ItemSeederPreIndustrialFood_Medieval_Prepared_Poultry_medieval_food_bird_filled_battalia_pie", RemoveDrugsAndFoodEffects: false, Ingredients: []) { Compatibility = new("ItemSeederPreIndustrialFood_Medieval_Prepared_Poultry_medieval_food_bird_filled_battalia_pie", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingMakeMedievalPreparedPoultryBreadCatalogueStock() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/make Medieval Prepared poultry bread catalogue stock", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make Medieval Prepared poultry bread catalogue stock",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for medieval_food_judhab_layered_chicken_bread.",
		Action = "preparing poultry food",
		ActiveCraftItemSdesc = "a poultry preparation",
		Gate = CatalogueRecipeGates.PreIndustrialFoodCatalogueProductionButchering154,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 3,
		Interruptable = false,
		Phases = [
			new(15, "$0 trim|trims $i1 and portion|portions the meat for the hearth.", "$0 trim|trims $i1 and portion|portions the meat for the hearth.", ExertionLevel.Stasis, 0),
			new(25, "$0 sear|sears the meat, then cook|cooks it through over a steady heat, using $t1.", "$0 sear|sears the meat, then cook|cooks it through over a steady heat, using $t1.", ExertionLevel.Stasis, 0),
			new(20, "$0 rest|rests $p1, then slice|slices it for the meal.", "$0 pierce|pierces the tough result and reserve|reserves it for a stew.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueTagRecipeInput(Tag: "Raw Non-Fish Meat Cut", Quantity: 1) { Compatibility = new("1x an item with the Raw Non-Fish Meat Cut tag", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Cooking", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Cooking tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueProgCookedRecipeProduct(Prog: "ItemSeederPreIndustrialFood_Medieval_Prepared_Poultry_medieval_food_judhab_layered_chicken_bread", RemoveDrugsAndFoodEffects: false, Ingredients: []) { Compatibility = new("ItemSeederPreIndustrialFood_Medieval_Prepared_Poultry_medieval_food_judhab_layered_chicken_bread", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingMakeMedievalPreparedFishBreadCatalogueStock() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/make Medieval Prepared fish bread catalogue stock", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make Medieval Prepared fish bread catalogue stock",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for medieval_food_fasting_trencher_fish_sops.",
		Action = "preparing fish food",
		ActiveCraftItemSdesc = "a fish preparation",
		Gate = CatalogueRecipeGates.PreIndustrialFoodCatalogueProductionButchering156,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 3,
		Interruptable = false,
		Phases = [
			new(15, "$0 clean|cleans $i1 and remove|removes the scales, shells, and offal.", "$0 clean|cleans $i1 and remove|removes the scales, shells, and offal.", ExertionLevel.Stasis, 0),
			new(25, "$0 season|seasons the catch and cook|cooks it over the heat until firm, using $t1.", "$0 season|seasons the catch and cook|cooks it over the heat until firm, using $t1.", ExertionLevel.Stasis, 0),
			new(20, "$0 inspect|inspects $p1 and serve|serves the finished catch while it is fresh.", "$0 smell|smells the failed catch and set|sets it aside for stock.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueTagRecipeInput(Tag: "Raw Fish Cut", Quantity: 1) { Compatibility = new("1x an item with the Raw Fish Cut tag", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Cooking", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Cooking tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueProgCookedRecipeProduct(Prog: "ItemSeederPreIndustrialFood_Medieval_Prepared_Fish_medieval_food_fasting_trencher_fish_sops", RemoveDrugsAndFoodEffects: false, Ingredients: []) { Compatibility = new("ItemSeederPreIndustrialFood_Medieval_Prepared_Fish_medieval_food_fasting_trencher_fish_sops", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingMakeMedievalPreparedFishFishCatalogueStock() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/make Medieval Prepared fish fish catalogue stock", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make Medieval Prepared fish fish catalogue stock",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for medieval_food_lenten_almond_fish_pottage.",
		Action = "preparing fish food",
		ActiveCraftItemSdesc = "a fish preparation",
		Gate = CatalogueRecipeGates.PreIndustrialFoodCatalogueProductionButchering156,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 3,
		Interruptable = false,
		Phases = [
			new(15, "$0 clean|cleans $i1 and remove|removes the scales, shells, and offal.", "$0 clean|cleans $i1 and remove|removes the scales, shells, and offal.", ExertionLevel.Stasis, 0),
			new(25, "$0 season|seasons the catch and cook|cooks it over the heat until firm, using $t1.", "$0 season|seasons the catch and cook|cooks it over the heat until firm, using $t1.", ExertionLevel.Stasis, 0),
			new(20, "$0 inspect|inspects $p1 and serve|serves the finished catch while it is fresh.", "$0 smell|smells the failed catch and set|sets it aside for stock.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueTagRecipeInput(Tag: "Raw Fish Cut", Quantity: 1) { Compatibility = new("1x an item with the Raw Fish Cut tag", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Cooking", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Cooking tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueProgCookedRecipeProduct(Prog: "ItemSeederPreIndustrialFood_Medieval_Prepared_Fish_medieval_food_almond_thickened_fasting_sauce_fish", RemoveDrugsAndFoodEffects: false, Ingredients: []) { Compatibility = new("ItemSeederPreIndustrialFood_Medieval_Prepared_Fish_medieval_food_almond_thickened_fasting_sauce_fish", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingMakeMedievalPreparedFishRiceCatalogueStock() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/make Medieval Prepared fish rice catalogue stock", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make Medieval Prepared fish rice catalogue stock",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for medieval_food_white_fasting_blaunche.",
		Action = "preparing fish food",
		ActiveCraftItemSdesc = "a fish preparation",
		Gate = CatalogueRecipeGates.PreIndustrialFoodCatalogueProductionButchering156,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 3,
		Interruptable = false,
		Phases = [
			new(15, "$0 clean|cleans $i1 and remove|removes the scales, shells, and offal.", "$0 clean|cleans $i1 and remove|removes the scales, shells, and offal.", ExertionLevel.Stasis, 0),
			new(25, "$0 season|seasons the catch and cook|cooks it over the heat until firm, using $t1.", "$0 season|seasons the catch and cook|cooks it over the heat until firm, using $t1.", ExertionLevel.Stasis, 0),
			new(20, "$0 inspect|inspects $p1 and serve|serves the finished catch while it is fresh.", "$0 smell|smells the failed catch and set|sets it aside for stock.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueTagRecipeInput(Tag: "Raw Fish Cut", Quantity: 1) { Compatibility = new("1x an item with the Raw Fish Cut tag", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Cooking", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Cooking tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueProgCookedRecipeProduct(Prog: "ItemSeederPreIndustrialFood_Medieval_Prepared_Fish_medieval_food_white_fasting_blaunche", RemoveDrugsAndFoodEffects: false, Ingredients: []) { Compatibility = new("ItemSeederPreIndustrialFood_Medieval_Prepared_Fish_medieval_food_white_fasting_blaunche", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingMakeMedievalPreparedDairyCheeseCatalogueStock() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/make Medieval Prepared dairy cheese catalogue stock", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make Medieval Prepared dairy cheese catalogue stock",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for medieval_food_cheese_filled_rissoles.",
		Action = "preparing dairy food",
		ActiveCraftItemSdesc = "a dairy preparation",
		Gate = CatalogueRecipeGates.PreIndustrialFoodCatalogueProductionCooking154,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 3,
		Interruptable = false,
		Phases = [
			new(15, "$0 break|breaks and combine|combines $i1 in the cooking bowl.", "$0 break|breaks and combine|combines $i1 in the cooking bowl.", ExertionLevel.Stasis, 0),
			new(25, "$0 warm|warms the mixture slowly until it thickens and holds together, using $t1.", "$0 warm|warms the mixture slowly until it thickens and holds together, using $t1.", ExertionLevel.Stasis, 0),
			new(20, "$0 set|sets $p1 aside to cool before serving.", "$0 taste|tastes the poor result and reserve|reserves it for a sauce.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityTagRecipeInput(Grams: 1000.0, MaterialTag: "Raw Milk", PileTag: "Raw Milk", CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1 kilogram of a material tagged as Raw Milk", ["piletag Raw Milk"]), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Cooking", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Cooking tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueProgCookedRecipeProduct(Prog: "ItemSeederPreIndustrialFood_Medieval_Prepared_Dairy_medieval_food_byzantine_cheese_honey_cake", RemoveDrugsAndFoodEffects: false, Ingredients: []) { Compatibility = new("ItemSeederPreIndustrialFood_Medieval_Prepared_Dairy_medieval_food_byzantine_cheese_honey_cake", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingMakeMedievalPreparedEggEggCatalogueStock() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/make Medieval Prepared egg egg catalogue stock", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make Medieval Prepared egg egg catalogue stock",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for medieval_food_herb_flower_omelette.",
		Action = "preparing egg food",
		ActiveCraftItemSdesc = "a egg preparation",
		Gate = CatalogueRecipeGates.PreIndustrialFoodCatalogueProductionCooking155,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 3,
		Interruptable = false,
		Phases = [
			new(15, "$0 break|breaks and combine|combines $i1 in the cooking bowl.", "$0 break|breaks and combine|combines $i1 in the cooking bowl.", ExertionLevel.Stasis, 0),
			new(25, "$0 warm|warms the mixture slowly until it thickens and holds together, using $t1.", "$0 warm|warms the mixture slowly until it thickens and holds together, using $t1.", ExertionLevel.Stasis, 0),
			new(20, "$0 set|sets $p1 aside to cool before serving.", "$0 taste|tastes the poor result and reserve|reserves it for a sauce.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityTagRecipeInput(Grams: 1000.0, MaterialTag: "Egg Product", PileTag: "Egg Product", CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1 kilogram of a material tagged as Egg Product", ["piletag Egg Product"]), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Cooking", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Cooking tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueProgCookedRecipeProduct(Prog: "ItemSeederPreIndustrialFood_Medieval_Prepared_Egg_medieval_food_egg_and_fruit_fast_pie", RemoveDrugsAndFoodEffects: false, Ingredients: []) { Compatibility = new("ItemSeederPreIndustrialFood_Medieval_Prepared_Egg_medieval_food_egg_and_fruit_fast_pie", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingMakeMedievalPreparedPreservedMeatCatalogueStock() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/make Medieval Prepared preserved meat catalogue stock", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make Medieval Prepared preserved meat catalogue stock",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for medieval_food_sahel_royal_roasted_meat_powder.",
		Action = "preparing preserved food",
		ActiveCraftItemSdesc = "a preserved preparation",
		Gate = CatalogueRecipeGates.PreIndustrialFoodCatalogueProductionButchering158,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 3,
		Interruptable = false,
		Phases = [
			new(15, "$0 portion|portions $i1 and rub|rubs the pieces with preserving salt.", "$0 portion|portions $i1 and rub|rubs the pieces with preserving salt.", ExertionLevel.Stasis, 0),
			new(25, "$0 dry|dries or smoke|smokes the provision until its surface tightens, using $t1.", "$0 dry|dries or smoke|smokes the provision until its surface tightens, using $t1.", ExertionLevel.Stasis, 0),
			new(20, "$0 wrap|wraps $p1 and store|stores it away from damp and vermin.", "$0 inspect|inspects the poor ration and salvage|salvages the sound portion.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueTagRecipeInput(Tag: "Raw Non-Fish Meat Cut", Quantity: 1) { Compatibility = new("1x an item with the Raw Non-Fish Meat Cut tag", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Cooking", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Cooking tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueProgCookedRecipeProduct(Prog: "ItemSeederPreIndustrialFood_Medieval_Prepared_Preserved_medieval_food_sahel_royal_roasted_meat_powder", RemoveDrugsAndFoodEffects: false, Ingredients: []) { Compatibility = new("ItemSeederPreIndustrialFood_Medieval_Prepared_Preserved_medieval_food_sahel_royal_roasted_meat_powder", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingMakeMedievalPreparedSweetDoughCatalogueStock() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/make Medieval Prepared sweet dough catalogue stock", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make Medieval Prepared sweet dough catalogue stock",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for medieval_food_byzantine_layered_sesame_pastry.",
		Action = "preparing sweet food",
		ActiveCraftItemSdesc = "a sweet preparation",
		Gate = CatalogueRecipeGates.PreIndustrialFoodCatalogueProductionCooking159,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 3,
		Interruptable = false,
		Phases = [
			new(15, "$0 crush|crushes $i1 and pick|picks out stones and skins.", "$0 crush|crushes $i1 and pick|picks out stones and skins.", ExertionLevel.Stasis, 0),
			new(25, "$0 mix|mixes the sweet stock into an even preparation, using $t1.", "$0 mix|mixes the sweet stock into an even preparation, using $t1.", ExertionLevel.Stasis, 0),
			new(20, "$0 shape|shapes $p1 and set|sets it aside to cool and firm.", "$0 break|breaks the finished sweet and save|saves the crumbs for porridge.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityTagRecipeInput(Grams: 1000.0, MaterialTag: "Food Crop", PileTag: "Seeded Yield", CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1 kilogram of a material tagged as Food Crop", ["piletag Seeded Yield"]), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Cooking", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Cooking tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueProgCookedRecipeProduct(Prog: "ItemSeederPreIndustrialFood_Medieval_Prepared_Sweet_medieval_food_byzantine_layered_sesame_pastry", RemoveDrugsAndFoodEffects: false, Ingredients: []) { Compatibility = new("ItemSeederPreIndustrialFood_Medieval_Prepared_Sweet_medieval_food_byzantine_layered_sesame_pastry", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingMakeMedievalPreparedSweetBreadCatalogueStock() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/make Medieval Prepared sweet bread catalogue stock", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make Medieval Prepared sweet bread catalogue stock",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for medieval_food_honeyed_ginger_bread_mass.",
		Action = "preparing sweet food",
		ActiveCraftItemSdesc = "a sweet preparation",
		Gate = CatalogueRecipeGates.PreIndustrialFoodCatalogueProductionCooking159,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 3,
		Interruptable = false,
		Phases = [
			new(15, "$0 crush|crushes $i1 and pick|picks out stones and skins.", "$0 crush|crushes $i1 and pick|picks out stones and skins.", ExertionLevel.Stasis, 0),
			new(25, "$0 mix|mixes the sweet stock into an even preparation, using $t1.", "$0 mix|mixes the sweet stock into an even preparation, using $t1.", ExertionLevel.Stasis, 0),
			new(20, "$0 shape|shapes $p1 and set|sets it aside to cool and firm.", "$0 break|breaks the finished sweet and save|saves the crumbs for porridge.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityTagRecipeInput(Grams: 1000.0, MaterialTag: "Food Crop", PileTag: "Seeded Yield", CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1 kilogram of a material tagged as Food Crop", ["piletag Seeded Yield"]), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Cooking", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Cooking tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueProgCookedRecipeProduct(Prog: "ItemSeederPreIndustrialFood_Medieval_Prepared_Sweet_medieval_food_apricot_judhab_pudding", RemoveDrugsAndFoodEffects: false, Ingredients: []) { Compatibility = new("ItemSeederPreIndustrialFood_Medieval_Prepared_Sweet_medieval_food_apricot_judhab_pudding", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingMakeMedievalPreparedSweetMilletCatalogueStock() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/make Medieval Prepared sweet millet catalogue stock", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make Medieval Prepared sweet millet catalogue stock",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for medieval_food_jaggery_like_millet_payasa.",
		Action = "preparing sweet food",
		ActiveCraftItemSdesc = "a sweet preparation",
		Gate = CatalogueRecipeGates.PreIndustrialFoodCatalogueProductionMilling1510,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 3,
		Interruptable = false,
		Phases = [
			new(15, "$0 crush|crushes $i1 and pick|picks out stones and skins.", "$0 crush|crushes $i1 and pick|picks out stones and skins.", ExertionLevel.Stasis, 0),
			new(25, "$0 mix|mixes the sweet stock into an even preparation, using $t1.", "$0 mix|mixes the sweet stock into an even preparation, using $t1.", ExertionLevel.Stasis, 0),
			new(20, "$0 shape|shapes $p1 and set|sets it aside to cool and firm.", "$0 break|breaks the finished sweet and save|saves the crumbs for porridge.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityTagRecipeInput(Grams: 1000.0, MaterialTag: "Food Crop", PileTag: "Seeded Yield", CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1 kilogram of a material tagged as Food Crop", ["piletag Seeded Yield"]), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Cooking", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Cooking tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueProgCookedRecipeProduct(Prog: "ItemSeederPreIndustrialFood_Medieval_Prepared_Sweet_medieval_food_jaggery_like_millet_payasa", RemoveDrugsAndFoodEffects: false, Ingredients: []) { Compatibility = new("ItemSeederPreIndustrialFood_Medieval_Prepared_Sweet_medieval_food_jaggery_like_millet_payasa", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingMakeMedievalPreparedSweetWheatCatalogueStock() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/make Medieval Prepared sweet wheat catalogue stock", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make Medieval Prepared sweet wheat catalogue stock",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for medieval_food_monastery_honey_wafer.",
		Action = "preparing sweet food",
		ActiveCraftItemSdesc = "a sweet preparation",
		Gate = CatalogueRecipeGates.PreIndustrialFoodCatalogueProductionMilling1510,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 3,
		Interruptable = false,
		Phases = [
			new(15, "$0 crush|crushes $i1 and pick|picks out stones and skins.", "$0 crush|crushes $i1 and pick|picks out stones and skins.", ExertionLevel.Stasis, 0),
			new(25, "$0 mix|mixes the sweet stock into an even preparation, using $t1.", "$0 mix|mixes the sweet stock into an even preparation, using $t1.", ExertionLevel.Stasis, 0),
			new(20, "$0 shape|shapes $p1 and set|sets it aside to cool and firm.", "$0 break|breaks the finished sweet and save|saves the crumbs for porridge.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityTagRecipeInput(Grams: 1000.0, MaterialTag: "Food Crop", PileTag: "Seeded Yield", CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1 kilogram of a material tagged as Food Crop", ["piletag Seeded Yield"]), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Cooking", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Cooking tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueProgCookedRecipeProduct(Prog: "ItemSeederPreIndustrialFood_Medieval_Prepared_Sweet_medieval_food_court_spice_wafer_roll", RemoveDrugsAndFoodEffects: false, Ingredients: []) { Compatibility = new("ItemSeederPreIndustrialFood_Medieval_Prepared_Sweet_medieval_food_court_spice_wafer_roll", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingMakeMedievalPreparedSweetCheeseCatalogueStock() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/make Medieval Prepared sweet cheese catalogue stock", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make Medieval Prepared sweet cheese catalogue stock",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for medieval_food_qatayif_cheese_pockets.",
		Action = "preparing sweet food",
		ActiveCraftItemSdesc = "a sweet preparation",
		Gate = CatalogueRecipeGates.PreIndustrialFoodCatalogueProductionCooking159,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 3,
		Interruptable = false,
		Phases = [
			new(15, "$0 crush|crushes $i1 and pick|picks out stones and skins.", "$0 crush|crushes $i1 and pick|picks out stones and skins.", ExertionLevel.Stasis, 0),
			new(25, "$0 mix|mixes the sweet stock into an even preparation, using $t1.", "$0 mix|mixes the sweet stock into an even preparation, using $t1.", ExertionLevel.Stasis, 0),
			new(20, "$0 shape|shapes $p1 and set|sets it aside to cool and firm.", "$0 break|breaks the finished sweet and save|saves the crumbs for porridge.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityTagRecipeInput(Grams: 1000.0, MaterialTag: "Raw Milk", PileTag: "Raw Milk", CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1 kilogram of a material tagged as Raw Milk", ["piletag Raw Milk"]), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Cooking", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Cooking tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueProgCookedRecipeProduct(Prog: "ItemSeederPreIndustrialFood_Medieval_Prepared_Sweet_medieval_food_milk_reduced_peda_tablets", RemoveDrugsAndFoodEffects: false, Ingredients: []) { Compatibility = new("ItemSeederPreIndustrialFood_Medieval_Prepared_Sweet_medieval_food_milk_reduced_peda_tablets", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingMakeMedievalPreparedSweetVegetableCatalogueStock() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/make Medieval Prepared sweet vegetable catalogue stock", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make Medieval Prepared sweet vegetable catalogue stock",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for medieval_food_royal_betel_spice_sweet.",
		Action = "preparing sweet food",
		ActiveCraftItemSdesc = "a sweet preparation",
		Gate = CatalogueRecipeGates.PreIndustrialFoodCatalogueProductionCooking159,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 3,
		Interruptable = false,
		Phases = [
			new(15, "$0 crush|crushes $i1 and pick|picks out stones and skins.", "$0 crush|crushes $i1 and pick|picks out stones and skins.", ExertionLevel.Stasis, 0),
			new(25, "$0 mix|mixes the sweet stock into an even preparation, using $t1.", "$0 mix|mixes the sweet stock into an even preparation, using $t1.", ExertionLevel.Stasis, 0),
			new(20, "$0 shape|shapes $p1 and set|sets it aside to cool and firm.", "$0 break|breaks the finished sweet and save|saves the crumbs for porridge.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityTagRecipeInput(Grams: 1000.0, MaterialTag: "Vegetable", PileTag: "Seeded Yield", CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1 kilogram of a material tagged as Vegetable", ["piletag Seeded Yield"]), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Cooking", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Cooking tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueProgCookedRecipeProduct(Prog: "ItemSeederPreIndustrialFood_Medieval_Prepared_Sweet_medieval_food_royal_betel_spice_sweet", RemoveDrugsAndFoodEffects: false, Ingredients: []) { Compatibility = new("ItemSeederPreIndustrialFood_Medieval_Prepared_Sweet_medieval_food_royal_betel_spice_sweet", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingMakeMedievalPreparedSweetHoneyCatalogueStock() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/make Medieval Prepared sweet honey catalogue stock", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make Medieval Prepared sweet honey catalogue stock",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for medieval_food_royal_heraldic_subtlety.",
		Action = "preparing sweet food",
		ActiveCraftItemSdesc = "a sweet preparation",
		Gate = CatalogueRecipeGates.PreIndustrialFoodCatalogueProductionCooking159,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 3,
		Interruptable = false,
		Phases = [
			new(15, "$0 crush|crushes $i1 and pick|picks out stones and skins.", "$0 crush|crushes $i1 and pick|picks out stones and skins.", ExertionLevel.Stasis, 0),
			new(25, "$0 mix|mixes the sweet stock into an even preparation, using $t1.", "$0 mix|mixes the sweet stock into an even preparation, using $t1.", ExertionLevel.Stasis, 0),
			new(20, "$0 shape|shapes $p1 and set|sets it aside to cool and firm.", "$0 break|breaks the finished sweet and save|saves the crumbs for porridge.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityTagRecipeInput(Grams: 1000.0, MaterialTag: "Food", PileTag: "Pressed Honey", CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1 kilogram of a material tagged as Food", ["piletag Pressed Honey"]), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Cooking", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Cooking tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueProgCookedRecipeProduct(Prog: "ItemSeederPreIndustrialFood_Medieval_Prepared_Sweet_medieval_food_malt_sugar_animal_figure", RemoveDrugsAndFoodEffects: false, Ingredients: []) { Compatibility = new("ItemSeederPreIndustrialFood_Medieval_Prepared_Sweet_medieval_food_malt_sugar_animal_figure", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingMakeMedievalPreparedSweetRiceCatalogueStock() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/make Medieval Prepared sweet rice catalogue stock", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make Medieval Prepared sweet rice catalogue stock",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for medieval_food_royal_snow_cooling_sweet.",
		Action = "preparing sweet food",
		ActiveCraftItemSdesc = "a sweet preparation",
		Gate = CatalogueRecipeGates.PreIndustrialFoodCatalogueProductionMilling1510,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 3,
		Interruptable = false,
		Phases = [
			new(15, "$0 crush|crushes $i1 and pick|picks out stones and skins.", "$0 crush|crushes $i1 and pick|picks out stones and skins.", ExertionLevel.Stasis, 0),
			new(25, "$0 mix|mixes the sweet stock into an even preparation, using $t1.", "$0 mix|mixes the sweet stock into an even preparation, using $t1.", ExertionLevel.Stasis, 0),
			new(20, "$0 shape|shapes $p1 and set|sets it aside to cool and firm.", "$0 break|breaks the finished sweet and save|saves the crumbs for porridge.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityTagRecipeInput(Grams: 1000.0, MaterialTag: "Food Crop", PileTag: "Seeded Yield", CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1 kilogram of a material tagged as Food Crop", ["piletag Seeded Yield"]), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Cooking", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Cooking tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueProgCookedRecipeProduct(Prog: "ItemSeederPreIndustrialFood_Medieval_Prepared_Sweet_medieval_food_almond_rice_muhallabiya", RemoveDrugsAndFoodEffects: false, Ingredients: []) { Compatibility = new("ItemSeederPreIndustrialFood_Medieval_Prepared_Sweet_medieval_food_almond_rice_muhallabiya", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingMakeMedievalPreparedSweetFoodCatalogueStock() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/make Medieval Prepared sweet food catalogue stock", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make Medieval Prepared sweet food catalogue stock",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for medieval_food_sainted_figure_marchpane.",
		Action = "preparing sweet food",
		ActiveCraftItemSdesc = "a sweet preparation",
		Gate = CatalogueRecipeGates.PreIndustrialFoodCatalogueProductionCooking159,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 3,
		Interruptable = false,
		Phases = [
			new(15, "$0 crush|crushes $i1 and pick|picks out stones and skins.", "$0 crush|crushes $i1 and pick|picks out stones and skins.", ExertionLevel.Stasis, 0),
			new(25, "$0 mix|mixes the sweet stock into an even preparation, using $t1.", "$0 mix|mixes the sweet stock into an even preparation, using $t1.", ExertionLevel.Stasis, 0),
			new(20, "$0 shape|shapes $p1 and set|sets it aside to cool and firm.", "$0 break|breaks the finished sweet and save|saves the crumbs for porridge.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityTagRecipeInput(Grams: 1000.0, MaterialTag: "Food Crop", PileTag: "Seeded Yield", CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1 kilogram of a material tagged as Food Crop", ["piletag Seeded Yield"]), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Cooking", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Cooking tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueProgCookedRecipeProduct(Prog: "ItemSeederPreIndustrialFood_Medieval_Prepared_Sweet_medieval_food_faludhaj_starch_confection", RemoveDrugsAndFoodEffects: false, Ingredients: []) { Compatibility = new("ItemSeederPreIndustrialFood_Medieval_Prepared_Sweet_medieval_food_faludhaj_starch_confection", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingMakeMedievalPreparedSweetMeatCatalogueStock() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/make Medieval Prepared sweet meat catalogue stock", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make Medieval Prepared sweet meat catalogue stock",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for medieval_food_sultanate_almond_meat_halwa.",
		Action = "preparing sweet food",
		ActiveCraftItemSdesc = "a sweet preparation",
		Gate = CatalogueRecipeGates.PreIndustrialFoodCatalogueProductionButchering1511,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 3,
		Interruptable = false,
		Phases = [
			new(15, "$0 crush|crushes $i1 and pick|picks out stones and skins.", "$0 crush|crushes $i1 and pick|picks out stones and skins.", ExertionLevel.Stasis, 0),
			new(25, "$0 mix|mixes the sweet stock into an even preparation, using $t1.", "$0 mix|mixes the sweet stock into an even preparation, using $t1.", ExertionLevel.Stasis, 0),
			new(20, "$0 shape|shapes $p1 and set|sets it aside to cool and firm.", "$0 break|breaks the finished sweet and save|saves the crumbs for porridge.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueTagRecipeInput(Tag: "Raw Non-Fish Meat Cut", Quantity: 1) { Compatibility = new("1x an item with the Raw Non-Fish Meat Cut tag", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Cooking", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Cooking tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueProgCookedRecipeProduct(Prog: "ItemSeederPreIndustrialFood_Medieval_Prepared_Sweet_medieval_food_sultanate_almond_meat_halwa", RemoveDrugsAndFoodEffects: false, Ingredients: []) { Compatibility = new("ItemSeederPreIndustrialFood_Medieval_Prepared_Sweet_medieval_food_sultanate_almond_meat_halwa", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingFillCatalogueLiquidMedievalFoodLiquidAleOnionPotLiquor() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/fill catalogue liquid medieval_food_liquid_ale_onion_pot_liquor", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:preindustrial_food_catalogue_liquid_amphora"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "fill catalogue liquid medieval_food_liquid_ale_onion_pot_liquor",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for medieval_food_liquid_ale_onion_pot_liquor.",
		Action = "preparing broth food",
		ActiveCraftItemSdesc = "a broth preparation",
		Gate = CatalogueRecipeGates.PreIndustrialFoodCatalogueProductionCooking1511,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 3,
		Interruptable = false,
		Phases = [
			new(15, "$0 crush|crushes $i1 and lay|lays out the broth stock.", "$0 crush|crushes $i1 and lay|lays out the broth stock.", ExertionLevel.Stasis, 0),
			new(25, "$0 work|works the broth stock into a smooth liquid, following a household method, using $t1.", "$0 work|works the broth stock into a smooth liquid, following a household method, using $t1.", ExertionLevel.Stasis, 0),
			new(20, "$0 inspect|inspects $p1 and set|sets the finished drink aside to settle.", "$0 inspect|inspects the poor drink and salvage|salvages the usable stock.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityTagRecipeInput(Grams: 1000.0, MaterialTag: "Food Crop", PileTag: "Seeded Yield", CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1 kilogram of a material tagged as Food Crop", ["piletag Seeded Yield"]), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Cooking", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Cooking tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueLiquidRecipeProduct(ItemStableReference: "preindustrial_food_catalogue_liquid_amphora", Quantity: 1, SkinStableReference: null, Liquid: "ale-onion pottage liquor", Litres: 10.0) { Compatibility = new("1x a shared catalogue liquid amphora (@preindustrial_food_catalogue_liquid_amphora) filled with 10 litres of ale-onion pottage liquor", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingFillCatalogueLiquidMedievalFoodLiquidCaponRestorationBroth() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/fill catalogue liquid medieval_food_liquid_capon_restoration_broth", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:preindustrial_food_catalogue_liquid_amphora"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "fill catalogue liquid medieval_food_liquid_capon_restoration_broth",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for medieval_food_liquid_capon_restoration_broth.",
		Action = "preparing broth food",
		ActiveCraftItemSdesc = "a broth preparation",
		Gate = CatalogueRecipeGates.PreIndustrialFoodCatalogueProductionCooking1511,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 3,
		Interruptable = false,
		Phases = [
			new(15, "$0 crush|crushes $i1 and lay|lays out the broth stock.", "$0 crush|crushes $i1 and lay|lays out the broth stock.", ExertionLevel.Stasis, 0),
			new(25, "$0 work|works the broth stock into a smooth liquid, following a household method, using $t1.", "$0 work|works the broth stock into a smooth liquid, following a household method, using $t1.", ExertionLevel.Stasis, 0),
			new(20, "$0 inspect|inspects $p1 and set|sets the finished drink aside to settle.", "$0 inspect|inspects the poor drink and salvage|salvages the usable stock.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityTagRecipeInput(Grams: 1000.0, MaterialTag: "Food Crop", PileTag: "Seeded Yield", CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1 kilogram of a material tagged as Food Crop", ["piletag Seeded Yield"]), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Cooking", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Cooking tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueLiquidRecipeProduct(ItemStableReference: "preindustrial_food_catalogue_liquid_amphora", Quantity: 1, SkinStableReference: null, Liquid: "capon restoration broth", Litres: 10.0) { Compatibility = new("1x a shared catalogue liquid amphora (@preindustrial_food_catalogue_liquid_amphora) filled with 10 litres of capon restoration broth", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingFillCatalogueLiquidMedievalFoodLiquidFishDaySaffronBroth() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/fill catalogue liquid medieval_food_liquid_fish_day_saffron_broth", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:preindustrial_food_catalogue_liquid_amphora"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "fill catalogue liquid medieval_food_liquid_fish_day_saffron_broth",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for medieval_food_liquid_fish_day_saffron_broth.",
		Action = "preparing broth food",
		ActiveCraftItemSdesc = "a broth preparation",
		Gate = CatalogueRecipeGates.PreIndustrialFoodCatalogueProductionCooking1511,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 3,
		Interruptable = false,
		Phases = [
			new(15, "$0 crush|crushes $i1 and lay|lays out the broth stock.", "$0 crush|crushes $i1 and lay|lays out the broth stock.", ExertionLevel.Stasis, 0),
			new(25, "$0 work|works the broth stock into a smooth liquid, following a household method, using $t1.", "$0 work|works the broth stock into a smooth liquid, following a household method, using $t1.", ExertionLevel.Stasis, 0),
			new(20, "$0 inspect|inspects $p1 and set|sets the finished drink aside to settle.", "$0 inspect|inspects the poor drink and salvage|salvages the usable stock.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityTagRecipeInput(Grams: 1000.0, MaterialTag: "Food Crop", PileTag: "Seeded Yield", CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1 kilogram of a material tagged as Food Crop", ["piletag Seeded Yield"]), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Cooking", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Cooking tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueLiquidRecipeProduct(ItemStableReference: "preindustrial_food_catalogue_liquid_amphora", Quantity: 1, SkinStableReference: null, Liquid: "fish-day saffron broth", Litres: 10.0) { Compatibility = new("1x a shared catalogue liquid amphora (@preindustrial_food_catalogue_liquid_amphora) filled with 10 litres of fish-day saffron broth", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};
}
