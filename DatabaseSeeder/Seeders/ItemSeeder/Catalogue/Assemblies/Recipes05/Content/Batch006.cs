#nullable enable

using MudSharp.Body;
using MudSharp.GameItems.Inventory.Plans;
using MudSharp.RPG.Checks;

namespace DatabaseSeeder.Seeders;

public static partial class Recipes05Stock
{
	public static CatalogueRecipeDefinition PreIndustrialFoodmakingMakeSharedPreparedGrainHoneyCatalogueStock() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/make Shared Prepared grain honey catalogue stock", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["medieval", "renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make Shared Prepared grain honey catalogue stock",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for preindustrial_food_honey_bound_parched_grain_bar.",
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
			new CatalogueCommodityTagRecipeInput(Grams: 1000.0, MaterialTag: "Food", PileTag: "Pressed Honey", CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1 kilogram of a material tagged as Food", ["piletag Pressed Honey"]), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Cooking", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Cooking tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueProgCookedRecipeProduct(Prog: "ItemSeederPreIndustrialFood_Shared_Prepared_Grain_preindustrial_food_caravan_sesame_grain_bar", RemoveDrugsAndFoodEffects: false, Ingredients: []) { Compatibility = new("ItemSeederPreIndustrialFood_Shared_Prepared_Grain_preindustrial_food_caravan_sesame_grain_bar", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingMakeSharedPreparedGrainDoughCatalogueStock() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/make Shared Prepared grain dough catalogue stock", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["medieval", "renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make Shared Prepared grain dough catalogue stock",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for preindustrial_food_honeyed_fried_dough.",
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
			new CatalogueProgCookedRecipeProduct(Prog: "ItemSeederPreIndustrialFood_Shared_Prepared_Grain_preindustrial_food_honeyed_fried_dough", RemoveDrugsAndFoodEffects: false, Ingredients: []) { Compatibility = new("ItemSeederPreIndustrialFood_Shared_Prepared_Grain_preindustrial_food_honeyed_fried_dough", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingMakeSharedPreparedGrainRiceCatalogueStock() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/make Shared Prepared grain rice catalogue stock", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["medieval", "renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make Shared Prepared grain rice catalogue stock",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for preindustrial_food_lacy_rice_crepe.",
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
			new CatalogueProgCookedRecipeProduct(Prog: "ItemSeederPreIndustrialFood_Shared_Prepared_Grain_preindustrial_food_broth_boiled_rice", RemoveDrugsAndFoodEffects: false, Ingredients: []) { Compatibility = new("ItemSeederPreIndustrialFood_Shared_Prepared_Grain_preindustrial_food_broth_boiled_rice", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingMakeSharedPreparedGrainBreadCatalogueStock() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/make Shared Prepared grain bread catalogue stock", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["medieval", "renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make Shared Prepared grain bread catalogue stock",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for preindustrial_food_mixed_grain_pancake.",
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
			new CatalogueProgCookedRecipeProduct(Prog: "ItemSeederPreIndustrialFood_Shared_Prepared_Grain_preindustrial_food_army_grain_ration_disc", RemoveDrugsAndFoodEffects: false, Ingredients: []) { Compatibility = new("ItemSeederPreIndustrialFood_Shared_Prepared_Grain_preindustrial_food_army_grain_ration_disc", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingMakeSharedPreparedGrainOatCatalogueStock() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/make Shared Prepared grain oat catalogue stock", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["medieval", "renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make Shared Prepared grain oat catalogue stock",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for preindustrial_food_oatmeal_pancake.",
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
			new CatalogueProgCookedRecipeProduct(Prog: "ItemSeederPreIndustrialFood_Shared_Prepared_Grain_preindustrial_food_boiled_oat_groats", RemoveDrugsAndFoodEffects: false, Ingredients: []) { Compatibility = new("ItemSeederPreIndustrialFood_Shared_Prepared_Grain_preindustrial_food_boiled_oat_groats", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingMakeSharedPreparedGrainWheatCatalogueStock() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/make Shared Prepared grain wheat catalogue stock", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["medieval", "renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make Shared Prepared grain wheat catalogue stock",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for preindustrial_food_plain_wheat_pancake.",
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
			new CatalogueProgCookedRecipeProduct(Prog: "ItemSeederPreIndustrialFood_Shared_Prepared_Grain_preindustrial_food_apple_batter_fritter", RemoveDrugsAndFoodEffects: false, Ingredients: []) { Compatibility = new("ItemSeederPreIndustrialFood_Shared_Prepared_Grain_preindustrial_food_apple_batter_fritter", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingMakeSharedPreparedGrainFoodCatalogueStock() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/make Shared Prepared grain food catalogue stock", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["medieval", "renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make Shared Prepared grain food catalogue stock",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for preindustrial_food_pounded_grain_ball.",
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
			new CatalogueProgCookedRecipeProduct(Prog: "ItemSeederPreIndustrialFood_Shared_Prepared_Grain_preindustrial_food_acrid_burnt_grain_ration", RemoveDrugsAndFoodEffects: false, Ingredients: []) { Compatibility = new("ItemSeederPreIndustrialFood_Shared_Prepared_Grain_preindustrial_food_acrid_burnt_grain_ration", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingMakeSharedPreparedGrainRyeCatalogueStock() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/make Shared Prepared grain rye catalogue stock", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["medieval", "renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make Shared Prepared grain rye catalogue stock",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for preindustrial_food_rye_batter_pancake.",
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
			new CatalogueProgCookedRecipeProduct(Prog: "ItemSeederPreIndustrialFood_Shared_Prepared_Grain_preindustrial_food_boiled_rye_berries", RemoveDrugsAndFoodEffects: false, Ingredients: []) { Compatibility = new("ItemSeederPreIndustrialFood_Shared_Prepared_Grain_preindustrial_food_boiled_rye_berries", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingMakeSharedPreparedGrainSorghumCatalogueStock() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/make Shared Prepared grain sorghum catalogue stock", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["medieval", "renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make Shared Prepared grain sorghum catalogue stock",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for preindustrial_food_sorghum_pancake.",
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
			new CatalogueProgCookedRecipeProduct(Prog: "ItemSeederPreIndustrialFood_Shared_Prepared_Grain_preindustrial_food_boiled_sorghum", RemoveDrugsAndFoodEffects: false, Ingredients: []) { Compatibility = new("ItemSeederPreIndustrialFood_Shared_Prepared_Grain_preindustrial_food_boiled_sorghum", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingMakeSharedPreparedGrainMilletCatalogueStock() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/make Shared Prepared grain millet catalogue stock", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["medieval", "renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make Shared Prepared grain millet catalogue stock",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for preindustrial_food_steamed_millet_round.",
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
			new CatalogueProgCookedRecipeProduct(Prog: "ItemSeederPreIndustrialFood_Shared_Prepared_Grain_preindustrial_food_boiled_millet", RemoveDrugsAndFoodEffects: false, Ingredients: []) { Compatibility = new("ItemSeederPreIndustrialFood_Shared_Prepared_Grain_preindustrial_food_boiled_millet", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingMakeSharedPreparedBreadDoughCatalogueStock() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/make Shared Prepared bread dough catalogue stock", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["medieval", "renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make Shared Prepared bread dough catalogue stock",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for preindustrial_food_boiled_wheat_bread.",
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
			new CatalogueProgCookedRecipeProduct(Prog: "ItemSeederPreIndustrialFood_Shared_Prepared_Bread_preindustrial_food_boiled_wheat_bread", RemoveDrugsAndFoodEffects: false, Ingredients: []) { Compatibility = new("ItemSeederPreIndustrialFood_Shared_Prepared_Bread_preindustrial_food_boiled_wheat_bread", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingMakeSharedPreparedBreadBarleyCatalogueStock() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/make Shared Prepared bread barley catalogue stock", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["medieval", "renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make Shared Prepared bread barley catalogue stock",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for preindustrial_food_brown_barley_loaf.",
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
			new CatalogueProgCookedRecipeProduct(Prog: "ItemSeederPreIndustrialFood_Shared_Prepared_Bread_preindustrial_food_barley_campaign_biscuit", RemoveDrugsAndFoodEffects: false, Ingredients: []) { Compatibility = new("ItemSeederPreIndustrialFood_Shared_Prepared_Bread_preindustrial_food_barley_campaign_biscuit", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingMakeSharedPreparedBreadCornCatalogueStock() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/make Shared Prepared bread corn catalogue stock", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["medieval", "renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make Shared Prepared bread corn catalogue stock",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for preindustrial_food_corn_hearth_flatbread.",
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
			new CatalogueCommodityTagRecipeInput(Grams: 1000.0, MaterialTag: "Vegetable", PileTag: "Seeded Yield", CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1 kilogram of a material tagged as Vegetable", ["piletag Seeded Yield"]), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Cooking", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Cooking tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueProgCookedRecipeProduct(Prog: "ItemSeederPreIndustrialFood_Shared_Prepared_Bread_preindustrial_food_corn_hearth_flatbread", RemoveDrugsAndFoodEffects: false, Ingredients: []) { Compatibility = new("ItemSeederPreIndustrialFood_Shared_Prepared_Bread_preindustrial_food_corn_hearth_flatbread", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingMakeSharedPreparedBreadBreadCatalogueStock() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/make Shared Prepared bread bread catalogue stock", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["medieval", "renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make Shared Prepared bread bread catalogue stock",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for preindustrial_food_fruit_filled_bread_roll.",
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
			new CatalogueProgCookedRecipeProduct(Prog: "ItemSeederPreIndustrialFood_Shared_Prepared_Bread_preindustrial_food_ale_soaked_bread", RemoveDrugsAndFoodEffects: false, Ingredients: []) { Compatibility = new("ItemSeederPreIndustrialFood_Shared_Prepared_Bread_preindustrial_food_ale_soaked_bread", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingMakeSharedPreparedBreadMilletCatalogueStock() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/make Shared Prepared bread millet catalogue stock", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["medieval", "renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make Shared Prepared bread millet catalogue stock",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for preindustrial_food_millet_hearth_loaf.",
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
			new CatalogueProgCookedRecipeProduct(Prog: "ItemSeederPreIndustrialFood_Shared_Prepared_Bread_preindustrial_food_fermented_millet_flatbread", RemoveDrugsAndFoodEffects: false, Ingredients: []) { Compatibility = new("ItemSeederPreIndustrialFood_Shared_Prepared_Bread_preindustrial_food_fermented_millet_flatbread", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingMakeSharedPreparedBreadOatCatalogueStock() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/make Shared Prepared bread oat catalogue stock", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["medieval", "renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make Shared Prepared bread oat catalogue stock",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for preindustrial_food_oaten_pan_loaf.",
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
			new CatalogueProgCookedRecipeProduct(Prog: "ItemSeederPreIndustrialFood_Shared_Prepared_Bread_preindustrial_food_oat_griddle_bread", RemoveDrugsAndFoodEffects: false, Ingredients: []) { Compatibility = new("ItemSeederPreIndustrialFood_Shared_Prepared_Bread_preindustrial_food_oat_griddle_bread", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingMakeSharedPreparedBreadRiceCatalogueStock() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/make Shared Prepared bread rice catalogue stock", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["medieval", "renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make Shared Prepared bread rice catalogue stock",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for preindustrial_food_rice_flour_flatbread.",
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
			new CatalogueProgCookedRecipeProduct(Prog: "ItemSeederPreIndustrialFood_Shared_Prepared_Bread_preindustrial_food_fermented_rice_flatbread", RemoveDrugsAndFoodEffects: false, Ingredients: []) { Compatibility = new("ItemSeederPreIndustrialFood_Shared_Prepared_Bread_preindustrial_food_fermented_rice_flatbread", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingMakeSharedPreparedBreadRyeCatalogueStock() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/make Shared Prepared bread rye catalogue stock", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["medieval", "renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make Shared Prepared bread rye catalogue stock",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for preindustrial_food_rustic_rye_loaf.",
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
			new CatalogueProgCookedRecipeProduct(Prog: "ItemSeederPreIndustrialFood_Shared_Prepared_Bread_preindustrial_food_caraway_rye_biscuit", RemoveDrugsAndFoodEffects: false, Ingredients: []) { Compatibility = new("ItemSeederPreIndustrialFood_Shared_Prepared_Bread_preindustrial_food_caraway_rye_biscuit", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingMakeSharedPreparedBreadSorghumCatalogueStock() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/make Shared Prepared bread sorghum catalogue stock", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["medieval", "renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make Shared Prepared bread sorghum catalogue stock",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for preindustrial_food_sorghum_round_loaf.",
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
			new CatalogueProgCookedRecipeProduct(Prog: "ItemSeederPreIndustrialFood_Shared_Prepared_Bread_preindustrial_food_fermented_sorghum_flatbread", RemoveDrugsAndFoodEffects: false, Ingredients: []) { Compatibility = new("ItemSeederPreIndustrialFood_Shared_Prepared_Bread_preindustrial_food_fermented_sorghum_flatbread", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingMakeSharedPreparedBreadWheatCatalogueStock() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/make Shared Prepared bread wheat catalogue stock", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["medieval", "renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make Shared Prepared bread wheat catalogue stock",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for preindustrial_food_white_sieved_loaf.",
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
			new CatalogueProgCookedRecipeProduct(Prog: "ItemSeederPreIndustrialFood_Shared_Prepared_Bread_preindustrial_food_bean_filled_steamed_bun", RemoveDrugsAndFoodEffects: false, Ingredients: []) { Compatibility = new("ItemSeederPreIndustrialFood_Shared_Prepared_Bread_preindustrial_food_bean_filled_steamed_bun", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingMakeSharedPreparedPorridgeBarleyCatalogueStock() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/make Shared Prepared porridge barley catalogue stock", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["medieval", "renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make Shared Prepared porridge barley catalogue stock",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for preindustrial_food_barley_porridge.",
		Action = "preparing porridge food",
		ActiveCraftItemSdesc = "a porridge preparation",
		Gate = CatalogueRecipeGates.PreIndustrialFoodCatalogueProductionMilling152,
		Trait = null,
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
			new CatalogueProgCookedRecipeProduct(Prog: "ItemSeederPreIndustrialFood_Shared_Prepared_Porridge_preindustrial_food_barley_porridge", RemoveDrugsAndFoodEffects: false, Ingredients: []) { Compatibility = new("ItemSeederPreIndustrialFood_Shared_Prepared_Porridge_preindustrial_food_barley_porridge", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingMakeSharedPreparedPorridgeCornCatalogueStock() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/make Shared Prepared porridge corn catalogue stock", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["medieval", "renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make Shared Prepared porridge corn catalogue stock",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for preindustrial_food_maize_porridge.",
		Action = "preparing porridge food",
		ActiveCraftItemSdesc = "a porridge preparation",
		Gate = CatalogueRecipeGates.PreIndustrialFoodCatalogueProductionMilling152,
		Trait = null,
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
			new CatalogueCommodityTagRecipeInput(Grams: 1000.0, MaterialTag: "Vegetable", PileTag: "Seeded Yield", CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1 kilogram of a material tagged as Vegetable", ["piletag Seeded Yield"]), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Cooking", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Cooking tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueProgCookedRecipeProduct(Prog: "ItemSeederPreIndustrialFood_Shared_Prepared_Porridge_preindustrial_food_bean_maize_porridge", RemoveDrugsAndFoodEffects: false, Ingredients: []) { Compatibility = new("ItemSeederPreIndustrialFood_Shared_Prepared_Porridge_preindustrial_food_bean_maize_porridge", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingMakeSharedPreparedPorridgeMilletCatalogueStock() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/make Shared Prepared porridge millet catalogue stock", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["medieval", "renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make Shared Prepared porridge millet catalogue stock",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for preindustrial_food_millet_porridge.",
		Action = "preparing porridge food",
		ActiveCraftItemSdesc = "a porridge preparation",
		Gate = CatalogueRecipeGates.PreIndustrialFoodCatalogueProductionMilling152,
		Trait = null,
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
			new CatalogueProgCookedRecipeProduct(Prog: "ItemSeederPreIndustrialFood_Shared_Prepared_Porridge_preindustrial_food_millet_porridge", RemoveDrugsAndFoodEffects: false, Ingredients: []) { Compatibility = new("ItemSeederPreIndustrialFood_Shared_Prepared_Porridge_preindustrial_food_millet_porridge", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingMakeSharedPreparedPorridgeOatCatalogueStock() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/make Shared Prepared porridge oat catalogue stock", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["medieval", "renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make Shared Prepared porridge oat catalogue stock",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for preindustrial_food_oat_porridge.",
		Action = "preparing porridge food",
		ActiveCraftItemSdesc = "a porridge preparation",
		Gate = CatalogueRecipeGates.PreIndustrialFoodCatalogueProductionMilling152,
		Trait = null,
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
			new CatalogueProgCookedRecipeProduct(Prog: "ItemSeederPreIndustrialFood_Shared_Prepared_Porridge_preindustrial_food_oat_porridge", RemoveDrugsAndFoodEffects: false, Ingredients: []) { Compatibility = new("ItemSeederPreIndustrialFood_Shared_Prepared_Porridge_preindustrial_food_oat_porridge", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingMakeSharedPreparedPorridgeFoodCatalogueStock() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/make Shared Prepared porridge food catalogue stock", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["medieval", "renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make Shared Prepared porridge food catalogue stock",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for preindustrial_food_plain_gruel.",
		Action = "preparing porridge food",
		ActiveCraftItemSdesc = "a porridge preparation",
		Gate = CatalogueRecipeGates.PreIndustrialFoodCatalogueProductionMilling152,
		Trait = null,
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
			new CatalogueProgCookedRecipeProduct(Prog: "ItemSeederPreIndustrialFood_Shared_Prepared_Porridge_preindustrial_food_acorn_gruel", RemoveDrugsAndFoodEffects: false, Ingredients: []) { Compatibility = new("ItemSeederPreIndustrialFood_Shared_Prepared_Porridge_preindustrial_food_acorn_gruel", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingMakeSharedPreparedPorridgeRiceCatalogueStock() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/make Shared Prepared porridge rice catalogue stock", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["medieval", "renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make Shared Prepared porridge rice catalogue stock",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for preindustrial_food_rice_porridge.",
		Action = "preparing porridge food",
		ActiveCraftItemSdesc = "a porridge preparation",
		Gate = CatalogueRecipeGates.PreIndustrialFoodCatalogueProductionMilling152,
		Trait = null,
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
			new CatalogueProgCookedRecipeProduct(Prog: "ItemSeederPreIndustrialFood_Shared_Prepared_Porridge_preindustrial_food_black_rice_porridge", RemoveDrugsAndFoodEffects: false, Ingredients: []) { Compatibility = new("ItemSeederPreIndustrialFood_Shared_Prepared_Porridge_preindustrial_food_black_rice_porridge", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingMakeSharedPreparedPorridgeRyeCatalogueStock() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/make Shared Prepared porridge rye catalogue stock", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["medieval", "renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make Shared Prepared porridge rye catalogue stock",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for preindustrial_food_rye_porridge.",
		Action = "preparing porridge food",
		ActiveCraftItemSdesc = "a porridge preparation",
		Gate = CatalogueRecipeGates.PreIndustrialFoodCatalogueProductionMilling152,
		Trait = null,
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
			new CatalogueProgCookedRecipeProduct(Prog: "ItemSeederPreIndustrialFood_Shared_Prepared_Porridge_preindustrial_food_rye_porridge", RemoveDrugsAndFoodEffects: false, Ingredients: []) { Compatibility = new("ItemSeederPreIndustrialFood_Shared_Prepared_Porridge_preindustrial_food_rye_porridge", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingMakeSharedPreparedPorridgeSorghumCatalogueStock() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/make Shared Prepared porridge sorghum catalogue stock", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["medieval", "renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make Shared Prepared porridge sorghum catalogue stock",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for preindustrial_food_sorghum_porridge.",
		Action = "preparing porridge food",
		ActiveCraftItemSdesc = "a porridge preparation",
		Gate = CatalogueRecipeGates.PreIndustrialFoodCatalogueProductionMilling152,
		Trait = null,
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
			new CatalogueProgCookedRecipeProduct(Prog: "ItemSeederPreIndustrialFood_Shared_Prepared_Porridge_preindustrial_food_palm_oil_porridge", RemoveDrugsAndFoodEffects: false, Ingredients: []) { Compatibility = new("ItemSeederPreIndustrialFood_Shared_Prepared_Porridge_preindustrial_food_palm_oil_porridge", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingMakeSharedPreparedPorridgeWheatCatalogueStock() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/make Shared Prepared porridge wheat catalogue stock", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["medieval", "renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make Shared Prepared porridge wheat catalogue stock",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for preindustrial_food_wheat_porridge.",
		Action = "preparing porridge food",
		ActiveCraftItemSdesc = "a porridge preparation",
		Gate = CatalogueRecipeGates.PreIndustrialFoodCatalogueProductionMilling152,
		Trait = null,
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
			new CatalogueProgCookedRecipeProduct(Prog: "ItemSeederPreIndustrialFood_Shared_Prepared_Porridge_preindustrial_food_green_grain_porridge", RemoveDrugsAndFoodEffects: false, Ingredients: []) { Compatibility = new("ItemSeederPreIndustrialFood_Shared_Prepared_Porridge_preindustrial_food_green_grain_porridge", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingMakeSharedPreparedNoodleFoodCatalogueStock() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/make Shared Prepared noodle food catalogue stock", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["medieval", "renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make Shared Prepared noodle food catalogue stock",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for preindustrial_food_buckwheat_noodles.",
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
			new CatalogueProgCookedRecipeProduct(Prog: "ItemSeederPreIndustrialFood_Shared_Prepared_Noodle_preindustrial_food_buckwheat_noodles", RemoveDrugsAndFoodEffects: false, Ingredients: []) { Compatibility = new("ItemSeederPreIndustrialFood_Shared_Prepared_Noodle_preindustrial_food_buckwheat_noodles", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingMakeSharedPreparedNoodleMilletCatalogueStock() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/make Shared Prepared noodle millet catalogue stock", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["medieval", "renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make Shared Prepared noodle millet catalogue stock",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for preindustrial_food_millet_noodles.",
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
			new CatalogueProgCookedRecipeProduct(Prog: "ItemSeederPreIndustrialFood_Shared_Prepared_Noodle_preindustrial_food_millet_noodles", RemoveDrugsAndFoodEffects: false, Ingredients: []) { Compatibility = new("ItemSeederPreIndustrialFood_Shared_Prepared_Noodle_preindustrial_food_millet_noodles", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingMakeSharedPreparedNoodleWheatCatalogueStock() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/make Shared Prepared noodle wheat catalogue stock", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["medieval", "renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make Shared Prepared noodle wheat catalogue stock",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for preindustrial_food_plain_wheat_noodles.",
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
			new CatalogueProgCookedRecipeProduct(Prog: "ItemSeederPreIndustrialFood_Shared_Prepared_Noodle_preindustrial_food_baked_noodle_dish", RemoveDrugsAndFoodEffects: false, Ingredients: []) { Compatibility = new("ItemSeederPreIndustrialFood_Shared_Prepared_Noodle_preindustrial_food_baked_noodle_dish", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingMakeSharedPreparedNoodleRiceCatalogueStock() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/make Shared Prepared noodle rice catalogue stock", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["medieval", "renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make Shared Prepared noodle rice catalogue stock",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for preindustrial_food_rice_noodles.",
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
			new CatalogueProgCookedRecipeProduct(Prog: "ItemSeederPreIndustrialFood_Shared_Prepared_Noodle_preindustrial_food_broad_rice_noodles", RemoveDrugsAndFoodEffects: false, Ingredients: []) { Compatibility = new("ItemSeederPreIndustrialFood_Shared_Prepared_Noodle_preindustrial_food_broad_rice_noodles", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingMakeSharedPreparedNoodleDoughCatalogueStock() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/make Shared Prepared noodle dough catalogue stock", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["medieval", "renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make Shared Prepared noodle dough catalogue stock",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for preindustrial_food_torn_dough_noodles.",
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
			new CatalogueProgCookedRecipeProduct(Prog: "ItemSeederPreIndustrialFood_Shared_Prepared_Noodle_preindustrial_food_egg_noodles", RemoveDrugsAndFoodEffects: false, Ingredients: []) { Compatibility = new("ItemSeederPreIndustrialFood_Shared_Prepared_Noodle_preindustrial_food_egg_noodles", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingMakeSharedPreparedDumplingMilletCatalogueStock() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/make Shared Prepared dumpling millet catalogue stock", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["medieval", "renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make Shared Prepared dumpling millet catalogue stock",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for preindustrial_food_boiled_millet_bread_ball.",
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
			new CatalogueCommodityTagRecipeInput(Grams: 1000.0, MaterialTag: "Food Crop", PileTag: "Seeded Yield", CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1 kilogram of a material tagged as Food Crop", ["piletag Seeded Yield"]), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Cooking", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Cooking tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueProgCookedRecipeProduct(Prog: "ItemSeederPreIndustrialFood_Shared_Prepared_Dumpling_preindustrial_food_boiled_millet_bread_ball", RemoveDrugsAndFoodEffects: false, Ingredients: []) { Compatibility = new("ItemSeederPreIndustrialFood_Shared_Prepared_Dumpling_preindustrial_food_boiled_millet_bread_ball", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingMakeSharedPreparedDumplingBreadCatalogueStock() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/make Shared Prepared dumpling bread catalogue stock", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["medieval", "renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make Shared Prepared dumpling bread catalogue stock",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for preindustrial_food_bread_crumb_dumpling.",
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
			new CatalogueCommodityTagRecipeInput(Grams: 1000.0, MaterialTag: "Food Crop", PileTag: "Seeded Yield", CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1 kilogram of a material tagged as Food Crop", ["piletag Seeded Yield"]), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Cooking", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Cooking tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueProgCookedRecipeProduct(Prog: "ItemSeederPreIndustrialFood_Shared_Prepared_Dumpling_preindustrial_food_bread_crumb_dumpling", RemoveDrugsAndFoodEffects: false, Ingredients: []) { Compatibility = new("ItemSeederPreIndustrialFood_Shared_Prepared_Dumpling_preindustrial_food_bread_crumb_dumpling", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingMakeSharedPreparedDumplingFoodCatalogueStock() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/make Shared Prepared dumpling food catalogue stock", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["medieval", "renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make Shared Prepared dumpling food catalogue stock",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for preindustrial_food_grain_dumplings.",
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
			new CatalogueCommodityTagRecipeInput(Grams: 1000.0, MaterialTag: "Food Crop", PileTag: "Seeded Yield", CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1 kilogram of a material tagged as Food Crop", ["piletag Seeded Yield"]), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Cooking", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Cooking tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueProgCookedRecipeProduct(Prog: "ItemSeederPreIndustrialFood_Shared_Prepared_Dumpling_preindustrial_food_cassava_dumplings", RemoveDrugsAndFoodEffects: false, Ingredients: []) { Compatibility = new("ItemSeederPreIndustrialFood_Shared_Prepared_Dumpling_preindustrial_food_cassava_dumplings", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingMakeSharedPreparedDumplingCornCatalogueStock() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/make Shared Prepared dumpling corn catalogue stock", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["medieval", "renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make Shared Prepared dumpling corn catalogue stock",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for preindustrial_food_maize_dumplings.",
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
			new CatalogueProgCookedRecipeProduct(Prog: "ItemSeederPreIndustrialFood_Shared_Prepared_Dumpling_preindustrial_food_bean_filled_maize_dumplings", RemoveDrugsAndFoodEffects: false, Ingredients: []) { Compatibility = new("ItemSeederPreIndustrialFood_Shared_Prepared_Dumpling_preindustrial_food_bean_filled_maize_dumplings", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingMakeSharedPreparedDumplingDoughCatalogueStock() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/make Shared Prepared dumpling dough catalogue stock", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["medieval", "renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make Shared Prepared dumpling dough catalogue stock",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for preindustrial_food_plain_boiled_dumplings.",
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
			new CatalogueCommodityTagRecipeInput(Grams: 1000.0, MaterialTag: "Food Crop", PileTag: "Seeded Yield", CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1 kilogram of a material tagged as Food Crop", ["piletag Seeded Yield"]), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Cooking", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Cooking tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueProgCookedRecipeProduct(Prog: "ItemSeederPreIndustrialFood_Shared_Prepared_Dumpling_preindustrial_food_apple_filled_dumplings", RemoveDrugsAndFoodEffects: false, Ingredients: []) { Compatibility = new("ItemSeederPreIndustrialFood_Shared_Prepared_Dumpling_preindustrial_food_apple_filled_dumplings", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingMakeSharedPreparedDumplingRiceCatalogueStock() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/make Shared Prepared dumpling rice catalogue stock", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["medieval", "renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make Shared Prepared dumpling rice catalogue stock",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for preindustrial_food_rice_dumplings.",
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
			new CatalogueCommodityTagRecipeInput(Grams: 1000.0, MaterialTag: "Food Crop", PileTag: "Seeded Yield", CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1 kilogram of a material tagged as Food Crop", ["piletag Seeded Yield"]), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Cooking", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Cooking tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueProgCookedRecipeProduct(Prog: "ItemSeederPreIndustrialFood_Shared_Prepared_Dumpling_preindustrial_food_rice_dumplings", RemoveDrugsAndFoodEffects: false, Ingredients: []) { Compatibility = new("ItemSeederPreIndustrialFood_Shared_Prepared_Dumpling_preindustrial_food_rice_dumplings", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingMakeSharedPreparedPulseLentilCatalogueStock() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/make Shared Prepared pulse lentil catalogue stock", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["medieval", "renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make Shared Prepared pulse lentil catalogue stock",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for preindustrial_food_brown_lentil_pottage.",
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
			new CatalogueCommodityTagRecipeInput(Grams: 1000.0, MaterialTag: "Vegetable", PileTag: "Seeded Yield", CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1 kilogram of a material tagged as Vegetable", ["piletag Seeded Yield"]), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Cooking", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Cooking tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueProgCookedRecipeProduct(Prog: "ItemSeederPreIndustrialFood_Shared_Prepared_Pulse_preindustrial_food_aubergine_and_lentil_mash", RemoveDrugsAndFoodEffects: false, Ingredients: []) { Compatibility = new("ItemSeederPreIndustrialFood_Shared_Prepared_Pulse_preindustrial_food_aubergine_and_lentil_mash", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingMakeSharedPreparedPulseFoodCatalogueStock() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/make Shared Prepared pulse food catalogue stock", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["medieval", "renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make Shared Prepared pulse food catalogue stock",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for preindustrial_food_mixed_pulse_pottage.",
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
			new CatalogueProgCookedRecipeProduct(Prog: "ItemSeederPreIndustrialFood_Shared_Prepared_Pulse_preindustrial_food_mixed_pulse_pottage", RemoveDrugsAndFoodEffects: false, Ingredients: []) { Compatibility = new("ItemSeederPreIndustrialFood_Shared_Prepared_Pulse_preindustrial_food_mixed_pulse_pottage", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingMakeSharedPreparedPulseBeanCatalogueStock() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/make Shared Prepared pulse bean catalogue stock", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["medieval", "renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make Shared Prepared pulse bean catalogue stock",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for preindustrial_food_plain_boiled_broad_beans.",
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
			new CatalogueCommodityTagRecipeInput(Grams: 1000.0, MaterialTag: "Vegetable", PileTag: "Seeded Yield", CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1 kilogram of a material tagged as Vegetable", ["piletag Seeded Yield"]), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Cooking", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Cooking tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueProgCookedRecipeProduct(Prog: "ItemSeederPreIndustrialFood_Shared_Prepared_Pulse_preindustrial_food_bean_and_beet_mash", RemoveDrugsAndFoodEffects: false, Ingredients: []) { Compatibility = new("ItemSeederPreIndustrialFood_Shared_Prepared_Pulse_preindustrial_food_bean_and_beet_mash", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingMakeSharedPreparedPulsePeaCatalogueStock() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/make Shared Prepared pulse pea catalogue stock", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["medieval", "renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make Shared Prepared pulse pea catalogue stock",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for preindustrial_food_plain_boiled_field_peas.",
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
			new CatalogueCommodityTagRecipeInput(Grams: 1000.0, MaterialTag: "Vegetable", PileTag: "Seeded Yield", CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1 kilogram of a material tagged as Vegetable", ["piletag Seeded Yield"]), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Cooking", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Cooking tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueProgCookedRecipeProduct(Prog: "ItemSeederPreIndustrialFood_Shared_Prepared_Pulse_preindustrial_food_asparagus_with_peas", RemoveDrugsAndFoodEffects: false, Ingredients: []) { Compatibility = new("ItemSeederPreIndustrialFood_Shared_Prepared_Pulse_preindustrial_food_asparagus_with_peas", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingMakeSharedPreparedVegetableVegetableCatalogueStock() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/make Shared Prepared vegetable vegetable catalogue stock", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["medieval", "renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make Shared Prepared vegetable vegetable catalogue stock",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for preindustrial_food_dried_leafy_green_cake.",
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
			new CatalogueProgCookedRecipeProduct(Prog: "ItemSeederPreIndustrialFood_Shared_Prepared_Vegetable_preindustrial_food_asparagus_and_herb_mash", RemoveDrugsAndFoodEffects: false, Ingredients: []) { Compatibility = new("ItemSeederPreIndustrialFood_Shared_Prepared_Vegetable_preindustrial_food_asparagus_and_herb_mash", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingMakeSharedPreparedSoupMeatCatalogueStock() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/make Shared Prepared soup meat catalogue stock", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["medieval", "renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make Shared Prepared soup meat catalogue stock",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for preindustrial_food_beef_and_onion_soup.",
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
			new CatalogueProgCookedRecipeProduct(Prog: "ItemSeederPreIndustrialFood_Shared_Prepared_Soup_preindustrial_food_beef_and_onion_soup", RemoveDrugsAndFoodEffects: false, Ingredients: []) { Compatibility = new("ItemSeederPreIndustrialFood_Shared_Prepared_Soup_preindustrial_food_beef_and_onion_soup", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingMakeSharedPreparedSoupBarleyCatalogueStock() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/make Shared Prepared soup barley catalogue stock", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["medieval", "renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make Shared Prepared soup barley catalogue stock",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for preindustrial_food_bitter_fisher_gruel.",
		Action = "preparing soup food",
		ActiveCraftItemSdesc = "a soup preparation",
		Gate = CatalogueRecipeGates.PreIndustrialFoodCatalogueProductionMilling154,
		Trait = null,
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
			new CatalogueCommodityTagRecipeInput(Grams: 1000.0, MaterialTag: "Food Crop", PileTag: "Seeded Yield", CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1 kilogram of a material tagged as Food Crop", ["piletag Seeded Yield"]), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Cooking", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Cooking tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueProgCookedRecipeProduct(Prog: "ItemSeederPreIndustrialFood_Shared_Prepared_Soup_preindustrial_food_barley_and_leek_soup", RemoveDrugsAndFoodEffects: false, Ingredients: []) { Compatibility = new("ItemSeederPreIndustrialFood_Shared_Prepared_Soup_preindustrial_food_barley_and_leek_soup", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingMakeSharedPreparedSoupVegetableCatalogueStock() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/make Shared Prepared soup vegetable catalogue stock", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["medieval", "renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make Shared Prepared soup vegetable catalogue stock",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for preindustrial_food_cabbage_leaf_soup.",
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
			new CatalogueProgCookedRecipeProduct(Prog: "ItemSeederPreIndustrialFood_Shared_Prepared_Soup_preindustrial_food_beetroot_garlic_soup", RemoveDrugsAndFoodEffects: false, Ingredients: []) { Compatibility = new("ItemSeederPreIndustrialFood_Shared_Prepared_Soup_preindustrial_food_beetroot_garlic_soup", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingMakeSharedPreparedSoupCheeseCatalogueStock() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/make Shared Prepared soup cheese catalogue stock", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["medieval", "renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make Shared Prepared soup cheese catalogue stock",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for preindustrial_food_cheese_and_onion_soup.",
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
			new CatalogueCommodityTagRecipeInput(Grams: 1000.0, MaterialTag: "Raw Milk", PileTag: "Raw Milk", CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1 kilogram of a material tagged as Raw Milk", ["piletag Raw Milk"]), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Cooking", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Cooking tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueProgCookedRecipeProduct(Prog: "ItemSeederPreIndustrialFood_Shared_Prepared_Soup_preindustrial_food_cheese_and_onion_soup", RemoveDrugsAndFoodEffects: false, Ingredients: []) { Compatibility = new("ItemSeederPreIndustrialFood_Shared_Prepared_Soup_preindustrial_food_cheese_and_onion_soup", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingMakeSharedPreparedSoupFoodCatalogueStock() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/make Shared Prepared soup food catalogue stock", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["medieval", "renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make Shared Prepared soup food catalogue stock",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for preindustrial_food_famine_husk_gruel.",
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
			new CatalogueCommodityTagRecipeInput(Grams: 1000.0, MaterialTag: "Food Crop", PileTag: "Seeded Yield", CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1 kilogram of a material tagged as Food Crop", ["piletag Seeded Yield"]), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Cooking", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Cooking tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueProgCookedRecipeProduct(Prog: "ItemSeederPreIndustrialFood_Shared_Prepared_Soup_preindustrial_food_famine_husk_gruel", RemoveDrugsAndFoodEffects: false, Ingredients: []) { Compatibility = new("ItemSeederPreIndustrialFood_Shared_Prepared_Soup_preindustrial_food_famine_husk_gruel", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingMakeSharedPreparedSoupBreadCatalogueStock() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/make Shared Prepared soup bread catalogue stock", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["medieval", "renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make Shared Prepared soup bread catalogue stock",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for preindustrial_food_fish_bread_soup.",
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
			new CatalogueCommodityTagRecipeInput(Grams: 1000.0, MaterialTag: "Food Crop", PileTag: "Seeded Yield", CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1 kilogram of a material tagged as Food Crop", ["piletag Seeded Yield"]), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Cooking", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Cooking tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueProgCookedRecipeProduct(Prog: "ItemSeederPreIndustrialFood_Shared_Prepared_Soup_preindustrial_food_fish_bread_soup", RemoveDrugsAndFoodEffects: false, Ingredients: []) { Compatibility = new("ItemSeederPreIndustrialFood_Shared_Prepared_Soup_preindustrial_food_fish_bread_soup", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingMakeSharedPreparedSoupRiceCatalogueStock() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/make Shared Prepared soup rice catalogue stock", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["medieval", "renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make Shared Prepared soup rice catalogue stock",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for preindustrial_food_fish_rice_congee.",
		Action = "preparing soup food",
		ActiveCraftItemSdesc = "a soup preparation",
		Gate = CatalogueRecipeGates.PreIndustrialFoodCatalogueProductionMilling154,
		Trait = null,
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
			new CatalogueCommodityTagRecipeInput(Grams: 1000.0, MaterialTag: "Food Crop", PileTag: "Seeded Yield", CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1 kilogram of a material tagged as Food Crop", ["piletag Seeded Yield"]), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Cooking", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Cooking tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueProgCookedRecipeProduct(Prog: "ItemSeederPreIndustrialFood_Shared_Prepared_Soup_preindustrial_food_fish_rice_congee", RemoveDrugsAndFoodEffects: false, Ingredients: []) { Compatibility = new("ItemSeederPreIndustrialFood_Shared_Prepared_Soup_preindustrial_food_fish_rice_congee", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingMakeSharedPreparedSoupBeanCatalogueStock() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/make Shared Prepared soup bean catalogue stock", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["medieval", "renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make Shared Prepared soup bean catalogue stock",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for preindustrial_food_plain_bean_soup.",
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
			new CatalogueProgCookedRecipeProduct(Prog: "ItemSeederPreIndustrialFood_Shared_Prepared_Soup_preindustrial_food_bean_and_leek_soup", RemoveDrugsAndFoodEffects: false, Ingredients: []) { Compatibility = new("ItemSeederPreIndustrialFood_Shared_Prepared_Soup_preindustrial_food_bean_and_leek_soup", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingMakeSharedPreparedSoupChickpeaCatalogueStock() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/make Shared Prepared soup chickpea catalogue stock", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["medieval", "renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make Shared Prepared soup chickpea catalogue stock",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for preindustrial_food_plain_chickpea_soup.",
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
			new CatalogueCommodityTagRecipeInput(Grams: 1000.0, MaterialTag: "Food Crop", PileTag: "Seeded Yield", CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1 kilogram of a material tagged as Food Crop", ["piletag Seeded Yield"]), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Cooking", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Cooking tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueProgCookedRecipeProduct(Prog: "ItemSeederPreIndustrialFood_Shared_Prepared_Soup_preindustrial_food_chickpea_and_rice_broth", RemoveDrugsAndFoodEffects: false, Ingredients: []) { Compatibility = new("ItemSeederPreIndustrialFood_Shared_Prepared_Soup_preindustrial_food_chickpea_and_rice_broth", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingMakeSharedPreparedSoupLentilCatalogueStock() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/make Shared Prepared soup lentil catalogue stock", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["medieval", "renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make Shared Prepared soup lentil catalogue stock",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for preindustrial_food_plain_lentil_soup.",
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
			new CatalogueProgCookedRecipeProduct(Prog: "ItemSeederPreIndustrialFood_Shared_Prepared_Soup_preindustrial_food_lentil_and_garlic_broth", RemoveDrugsAndFoodEffects: false, Ingredients: []) { Compatibility = new("ItemSeederPreIndustrialFood_Shared_Prepared_Soup_preindustrial_food_lentil_and_garlic_broth", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingMakeSharedPreparedSoupPeaCatalogueStock() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/make Shared Prepared soup pea catalogue stock", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["medieval", "renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make Shared Prepared soup pea catalogue stock",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for preindustrial_food_plain_pea_soup.",
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
			new CatalogueProgCookedRecipeProduct(Prog: "ItemSeederPreIndustrialFood_Shared_Prepared_Soup_preindustrial_food_cabbage_and_pea_soup", RemoveDrugsAndFoodEffects: false, Ingredients: []) { Compatibility = new("ItemSeederPreIndustrialFood_Shared_Prepared_Soup_preindustrial_food_cabbage_and_pea_soup", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingMakeSharedPreparedSoupRyeCatalogueStock() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/make Shared Prepared soup rye catalogue stock", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["medieval", "renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make Shared Prepared soup rye catalogue stock",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for preindustrial_food_rye_and_cabbage_soup.",
		Action = "preparing soup food",
		ActiveCraftItemSdesc = "a soup preparation",
		Gate = CatalogueRecipeGates.PreIndustrialFoodCatalogueProductionMilling154,
		Trait = null,
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
			new CatalogueCommodityTagRecipeInput(Grams: 1000.0, MaterialTag: "Food Crop", PileTag: "Seeded Yield", CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1 kilogram of a material tagged as Food Crop", ["piletag Seeded Yield"]), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Cooking", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Cooking tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueProgCookedRecipeProduct(Prog: "ItemSeederPreIndustrialFood_Shared_Prepared_Soup_preindustrial_food_rye_and_cabbage_soup", RemoveDrugsAndFoodEffects: false, Ingredients: []) { Compatibility = new("ItemSeederPreIndustrialFood_Shared_Prepared_Soup_preindustrial_food_rye_and_cabbage_soup", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingMakeSharedPreparedSoupSorghumCatalogueStock() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/make Shared Prepared soup sorghum catalogue stock", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["medieval", "renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make Shared Prepared soup sorghum catalogue stock",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for preindustrial_food_sorghum_thin_gruel.",
		Action = "preparing soup food",
		ActiveCraftItemSdesc = "a soup preparation",
		Gate = CatalogueRecipeGates.PreIndustrialFoodCatalogueProductionMilling154,
		Trait = null,
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
			new CatalogueCommodityTagRecipeInput(Grams: 1000.0, MaterialTag: "Food Crop", PileTag: "Seeded Yield", CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1 kilogram of a material tagged as Food Crop", ["piletag Seeded Yield"]), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Cooking", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Cooking tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueProgCookedRecipeProduct(Prog: "ItemSeederPreIndustrialFood_Shared_Prepared_Soup_preindustrial_food_sorghum_and_onion_broth", RemoveDrugsAndFoodEffects: false, Ingredients: []) { Compatibility = new("ItemSeederPreIndustrialFood_Shared_Prepared_Soup_preindustrial_food_sorghum_and_onion_broth", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingMakeSharedPreparedSoupFishCatalogueStock() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/make Shared Prepared soup fish catalogue stock", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["medieval", "renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make Shared Prepared soup fish catalogue stock",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for preindustrial_food_thin_fish_broth_bowl.",
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
			new CatalogueTagRecipeInput(Tag: "Raw Fish Cut", Quantity: 1) { Compatibility = new("1x an item with the Raw Fish Cut tag", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Cooking", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Cooking tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueProgCookedRecipeProduct(Prog: "ItemSeederPreIndustrialFood_Shared_Prepared_Soup_preindustrial_food_clear_clam_broth", RemoveDrugsAndFoodEffects: false, Ingredients: []) { Compatibility = new("ItemSeederPreIndustrialFood_Shared_Prepared_Soup_preindustrial_food_clear_clam_broth", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingMakeSharedPreparedSoupWheatCatalogueStock() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/make Shared Prepared soup wheat catalogue stock", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["medieval", "renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make Shared Prepared soup wheat catalogue stock",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for preindustrial_food_wheat_berry_soup.",
		Action = "preparing soup food",
		ActiveCraftItemSdesc = "a soup preparation",
		Gate = CatalogueRecipeGates.PreIndustrialFoodCatalogueProductionMilling154,
		Trait = null,
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
			new CatalogueCommodityTagRecipeInput(Grams: 1000.0, MaterialTag: "Food Crop", PileTag: "Seeded Yield", CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1 kilogram of a material tagged as Food Crop", ["piletag Seeded Yield"]), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Cooking", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Cooking tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueProgCookedRecipeProduct(Prog: "ItemSeederPreIndustrialFood_Shared_Prepared_Soup_preindustrial_food_wheat_and_onion_soup", RemoveDrugsAndFoodEffects: false, Ingredients: []) { Compatibility = new("ItemSeederPreIndustrialFood_Shared_Prepared_Soup_preindustrial_food_wheat_and_onion_soup", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};
}
