#nullable enable

using MudSharp.Body;
using MudSharp.GameItems.Inventory.Plans;
using MudSharp.RPG.Checks;

namespace DatabaseSeeder.Seeders;

public static partial class Recipes06Stock
{
	public static CatalogueRecipeDefinition PreIndustrialFoodmakingMakeEarlyModernPreparedVegetableVegetableCatalogueStock() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/make EarlyModern Prepared vegetable vegetable catalogue stock", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make EarlyModern Prepared vegetable vegetable catalogue stock",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for earlymodern_food_french_mushroom_ragout.",
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
			new CatalogueProgCookedRecipeProduct(Prog: "ItemSeederPreIndustrialFood_EarlyModern_Prepared_Vegetable_earlymodern_food_andean_chile_cheese_potato", RemoveDrugsAndFoodEffects: false, Ingredients: []) { Compatibility = new("ItemSeederPreIndustrialFood_EarlyModern_Prepared_Vegetable_earlymodern_food_andean_chile_cheese_potato", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingMakeEarlyModernPreparedVegetableRiceCatalogueStock() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/make EarlyModern Prepared vegetable rice catalogue stock", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make EarlyModern Prepared vegetable rice catalogue stock",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for earlymodern_food_ottoman_green_pepper_dolma.",
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
			new CatalogueProgCookedRecipeProduct(Prog: "ItemSeederPreIndustrialFood_EarlyModern_Prepared_Vegetable_earlymodern_food_lowcountry_okra_rice_pot", RemoveDrugsAndFoodEffects: false, Ingredients: []) { Compatibility = new("ItemSeederPreIndustrialFood_EarlyModern_Prepared_Vegetable_earlymodern_food_lowcountry_okra_rice_pot", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingMakeEarlyModernPreparedVegetableBreadCatalogueStock() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/make EarlyModern Prepared vegetable bread catalogue stock", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make EarlyModern Prepared vegetable bread catalogue stock",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for earlymodern_food_tea_table_cucumber_like_pickle_toast.",
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
			new CatalogueProgCookedRecipeProduct(Prog: "ItemSeederPreIndustrialFood_EarlyModern_Prepared_Vegetable_earlymodern_food_tea_table_cucumber_like_pickle_toast", RemoveDrugsAndFoodEffects: false, Ingredients: []) { Compatibility = new("ItemSeederPreIndustrialFood_EarlyModern_Prepared_Vegetable_earlymodern_food_tea_table_cucumber_like_pickle_toast", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingMakeEarlyModernPreparedSoupRiceCatalogueStock() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/make EarlyModern Prepared soup rice catalogue stock", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make EarlyModern Prepared soup rice catalogue stock",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for earlymodern_food_joseon_tteokguk_ritual_soup.",
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
			new CatalogueProgCookedRecipeProduct(Prog: "ItemSeederPreIndustrialFood_EarlyModern_Prepared_Soup_earlymodern_food_joseon_tteokguk_ritual_soup", RemoveDrugsAndFoodEffects: false, Ingredients: []) { Compatibility = new("ItemSeederPreIndustrialFood_EarlyModern_Prepared_Soup_earlymodern_food_joseon_tteokguk_ritual_soup", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingMakeEarlyModernPreparedSoupFoodCatalogueStock() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/make EarlyModern Prepared soup food catalogue stock", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make EarlyModern Prepared soup food catalogue stock",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for earlymodern_food_qing_bird_nest_like_soup.",
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
			new CatalogueProgCookedRecipeProduct(Prog: "ItemSeederPreIndustrialFood_EarlyModern_Prepared_Soup_earlymodern_food_qing_bird_nest_like_soup", RemoveDrugsAndFoodEffects: false, Ingredients: []) { Compatibility = new("ItemSeederPreIndustrialFood_EarlyModern_Prepared_Soup_earlymodern_food_qing_bird_nest_like_soup", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingMakeEarlyModernPreparedSoupMeatCatalogueStock() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/make EarlyModern Prepared soup meat catalogue stock", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make EarlyModern Prepared soup meat catalogue stock",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for earlymodern_food_safavid_sour_fruit_leather_soup.",
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
			new CatalogueProgCookedRecipeProduct(Prog: "ItemSeederPreIndustrialFood_EarlyModern_Prepared_Soup_earlymodern_food_benin_chile_pepper_soup", RemoveDrugsAndFoodEffects: false, Ingredients: []) { Compatibility = new("ItemSeederPreIndustrialFood_EarlyModern_Prepared_Soup_earlymodern_food_benin_chile_pepper_soup", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingMakeEarlyModernPreparedStewFoodCatalogueStock() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/make EarlyModern Prepared stew food catalogue stock", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make EarlyModern Prepared stew food catalogue stock",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for earlymodern_food_joseon_sinseollo_brazier.",
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
			new CatalogueProgCookedRecipeProduct(Prog: "ItemSeederPreIndustrialFood_EarlyModern_Prepared_Stew_earlymodern_food_joseon_sinseollo_brazier", RemoveDrugsAndFoodEffects: false, Ingredients: []) { Compatibility = new("ItemSeederPreIndustrialFood_EarlyModern_Prepared_Stew_earlymodern_food_joseon_sinseollo_brazier", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingMakeEarlyModernPreparedStewMeatCatalogueStock() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/make EarlyModern Prepared stew meat catalogue stock", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make EarlyModern Prepared stew meat catalogue stock",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for earlymodern_food_safavid_herb_plum_meat_stew.",
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
			new CatalogueProgCookedRecipeProduct(Prog: "ItemSeederPreIndustrialFood_EarlyModern_Prepared_Stew_earlymodern_food_benin_plantain_like_meat_pottage", RemoveDrugsAndFoodEffects: false, Ingredients: []) { Compatibility = new("ItemSeederPreIndustrialFood_EarlyModern_Prepared_Stew_earlymodern_food_benin_plantain_like_meat_pottage", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingMakeEarlyModernPreparedStewCornCatalogueStock() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/make EarlyModern Prepared stew corn catalogue stock", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make EarlyModern Prepared stew corn catalogue stock",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for earlymodern_food_woodlands_succotash_bowl.",
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
			new CatalogueCommodityTagRecipeInput(Grams: 1000.0, MaterialTag: "Vegetable", PileTag: "Seeded Yield", CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1 kilogram of a material tagged as Vegetable", ["piletag Seeded Yield"]), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Cooking", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Cooking tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueProgCookedRecipeProduct(Prog: "ItemSeederPreIndustrialFood_EarlyModern_Prepared_Stew_earlymodern_food_woodlands_succotash_bowl", RemoveDrugsAndFoodEffects: false, Ingredients: []) { Compatibility = new("ItemSeederPreIndustrialFood_EarlyModern_Prepared_Stew_earlymodern_food_woodlands_succotash_bowl", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingMakeEarlyModernPreparedMeatMeatCatalogueStock() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/make EarlyModern Prepared meat meat catalogue stock", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make EarlyModern Prepared meat meat catalogue stock",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for earlymodern_food_french_brown_ragout_meat.",
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
			new CatalogueProgCookedRecipeProduct(Prog: "ItemSeederPreIndustrialFood_EarlyModern_Prepared_Meat_earlymodern_food_british_minced_tongue_sandwich", RemoveDrugsAndFoodEffects: false, Ingredients: []) { Compatibility = new("ItemSeederPreIndustrialFood_EarlyModern_Prepared_Meat_earlymodern_food_british_minced_tongue_sandwich", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingMakeEarlyModernPreparedMeatFoodCatalogueStock() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/make EarlyModern Prepared meat food catalogue stock", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make EarlyModern Prepared meat food catalogue stock",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for earlymodern_food_qing_imperial_manhan_platter.",
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
			new CatalogueProgCookedRecipeProduct(Prog: "ItemSeederPreIndustrialFood_EarlyModern_Prepared_Meat_earlymodern_food_qing_imperial_manhan_platter", RemoveDrugsAndFoodEffects: false, Ingredients: []) { Compatibility = new("ItemSeederPreIndustrialFood_EarlyModern_Prepared_Meat_earlymodern_food_qing_imperial_manhan_platter", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingMakeEarlyModernPreparedPoultryMeatCatalogueStock() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/make EarlyModern Prepared poultry meat catalogue stock", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make EarlyModern Prepared poultry meat catalogue stock",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for earlymodern_food_french_roux_veloute_fowl.",
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
			new CatalogueProgCookedRecipeProduct(Prog: "ItemSeederPreIndustrialFood_EarlyModern_Prepared_Poultry_earlymodern_food_french_roux_veloute_fowl", RemoveDrugsAndFoodEffects: false, Ingredients: []) { Compatibility = new("ItemSeederPreIndustrialFood_EarlyModern_Prepared_Poultry_earlymodern_food_french_roux_veloute_fowl", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingMakeEarlyModernPreparedFishFishCatalogueStock() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/make EarlyModern Prepared fish fish catalogue stock", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make EarlyModern Prepared fish fish catalogue stock",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for earlymodern_food_french_bechamel_baked_fish.",
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
			new CatalogueProgCookedRecipeProduct(Prog: "ItemSeederPreIndustrialFood_EarlyModern_Prepared_Fish_earlymodern_food_brazilian_cassava_fish_pirao", RemoveDrugsAndFoodEffects: false, Ingredients: []) { Compatibility = new("ItemSeederPreIndustrialFood_EarlyModern_Prepared_Fish_earlymodern_food_brazilian_cassava_fish_pirao", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingMakeEarlyModernPreparedFishBreadCatalogueStock() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/make EarlyModern Prepared fish bread catalogue stock", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make EarlyModern Prepared fish bread catalogue stock",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for earlymodern_food_french_hors_doeuvre_anchovy_toasts.",
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
			new CatalogueProgCookedRecipeProduct(Prog: "ItemSeederPreIndustrialFood_EarlyModern_Prepared_Fish_earlymodern_food_french_hors_doeuvre_anchovy_toasts", RemoveDrugsAndFoodEffects: false, Ingredients: []) { Compatibility = new("ItemSeederPreIndustrialFood_EarlyModern_Prepared_Fish_earlymodern_food_french_hors_doeuvre_anchovy_toasts", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingMakeEarlyModernPreparedFishRiceCatalogueStock() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/make EarlyModern Prepared fish rice catalogue stock", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make EarlyModern Prepared fish rice catalogue stock",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for earlymodern_food_tokugawa_sushi_box.",
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
			new CatalogueProgCookedRecipeProduct(Prog: "ItemSeederPreIndustrialFood_EarlyModern_Prepared_Fish_earlymodern_food_company_kedgeree_predecessor", RemoveDrugsAndFoodEffects: false, Ingredients: []) { Compatibility = new("ItemSeederPreIndustrialFood_EarlyModern_Prepared_Fish_earlymodern_food_company_kedgeree_predecessor", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingMakeEarlyModernPreparedFishFoodCatalogueStock() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/make EarlyModern Prepared fish food catalogue stock", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make EarlyModern Prepared fish food catalogue stock",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for earlymodern_food_tokugawa_tempura_assortment.",
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
			new CatalogueProgCookedRecipeProduct(Prog: "ItemSeederPreIndustrialFood_EarlyModern_Prepared_Fish_earlymodern_food_tokugawa_tempura_assortment", RemoveDrugsAndFoodEffects: false, Ingredients: []) { Compatibility = new("ItemSeederPreIndustrialFood_EarlyModern_Prepared_Fish_earlymodern_food_tokugawa_tempura_assortment", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingMakeEarlyModernPreparedShellfishFishCatalogueStock() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/make EarlyModern Prepared shellfish fish catalogue stock", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make EarlyModern Prepared shellfish fish catalogue stock",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for earlymodern_food_british_potted_shrimp_toast.",
		Action = "preparing shellfish food",
		ActiveCraftItemSdesc = "a shellfish preparation",
		Gate = CatalogueRecipeGates.PreIndustrialFoodCatalogueProductionButchering157,
		Trait = null,
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
			new CatalogueProgCookedRecipeProduct(Prog: "ItemSeederPreIndustrialFood_EarlyModern_Prepared_Shellfish_earlymodern_food_british_potted_shrimp_toast", RemoveDrugsAndFoodEffects: false, Ingredients: []) { Compatibility = new("ItemSeederPreIndustrialFood_EarlyModern_Prepared_Shellfish_earlymodern_food_british_potted_shrimp_toast", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingMakeEarlyModernPreparedDairyCheeseCatalogueStock() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/make EarlyModern Prepared dairy cheese catalogue stock", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make EarlyModern Prepared dairy cheese catalogue stock",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for earlymodern_food_french_cheese_gougere_puffs.",
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
			new CatalogueProgCookedRecipeProduct(Prog: "ItemSeederPreIndustrialFood_EarlyModern_Prepared_Dairy_earlymodern_food_french_cheese_gougere_puffs", RemoveDrugsAndFoodEffects: false, Ingredients: []) { Compatibility = new("ItemSeederPreIndustrialFood_EarlyModern_Prepared_Dairy_earlymodern_food_french_cheese_gougere_puffs", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingMakeEarlyModernPreparedEggEggCatalogueStock() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/make EarlyModern Prepared egg egg catalogue stock", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make EarlyModern Prepared egg egg catalogue stock",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for earlymodern_food_qing_tea_egg_plate.",
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
			new CatalogueProgCookedRecipeProduct(Prog: "ItemSeederPreIndustrialFood_EarlyModern_Prepared_Egg_earlymodern_food_andean_chuno_egg_cake", RemoveDrugsAndFoodEffects: false, Ingredients: []) { Compatibility = new("ItemSeederPreIndustrialFood_EarlyModern_Prepared_Egg_earlymodern_food_andean_chuno_egg_cake", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingMakeEarlyModernPreparedPreservedMeatCatalogueStock() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/make EarlyModern Prepared preserved meat catalogue stock", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make EarlyModern Prepared preserved meat catalogue stock",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for earlymodern_food_british_portable_soup_cake.",
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
			new CatalogueProgCookedRecipeProduct(Prog: "ItemSeederPreIndustrialFood_EarlyModern_Prepared_Preserved_earlymodern_food_british_portable_soup_cake", RemoveDrugsAndFoodEffects: false, Ingredients: []) { Compatibility = new("ItemSeederPreIndustrialFood_EarlyModern_Prepared_Preserved_earlymodern_food_british_portable_soup_cake", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingMakeEarlyModernPreparedPreservedCheeseCatalogueStock() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/make EarlyModern Prepared preserved cheese catalogue stock", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make EarlyModern Prepared preserved cheese catalogue stock",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for earlymodern_food_dutch_shipboard_cheese_ration.",
		Action = "preparing preserved food",
		ActiveCraftItemSdesc = "a preserved preparation",
		Gate = CatalogueRecipeGates.PreIndustrialFoodCatalogueProductionCooking156,
		Trait = null,
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
			new CatalogueCommodityTagRecipeInput(Grams: 1000.0, MaterialTag: "Raw Milk", PileTag: "Raw Milk", CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1 kilogram of a material tagged as Raw Milk", ["piletag Raw Milk"]), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Cooking", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Cooking tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueProgCookedRecipeProduct(Prog: "ItemSeederPreIndustrialFood_EarlyModern_Prepared_Preserved_earlymodern_food_dutch_shipboard_cheese_ration", RemoveDrugsAndFoodEffects: false, Ingredients: []) { Compatibility = new("ItemSeederPreIndustrialFood_EarlyModern_Prepared_Preserved_earlymodern_food_dutch_shipboard_cheese_ration", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingMakeEarlyModernPreparedPreservedVegetableCatalogueStock() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/make EarlyModern Prepared preserved vegetable catalogue stock", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make EarlyModern Prepared preserved vegetable catalogue stock",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for earlymodern_food_joseon_chile_cabbage_kimchi.",
		Action = "preparing preserved food",
		ActiveCraftItemSdesc = "a preserved preparation",
		Gate = CatalogueRecipeGates.PreIndustrialFoodCatalogueProductionCooking156,
		Trait = null,
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
			new CatalogueCommodityTagRecipeInput(Grams: 1000.0, MaterialTag: "Vegetable", PileTag: "Seeded Yield", CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1 kilogram of a material tagged as Vegetable", ["piletag Seeded Yield"]), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Cooking", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Cooking tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueProgCookedRecipeProduct(Prog: "ItemSeederPreIndustrialFood_EarlyModern_Prepared_Preserved_earlymodern_food_joseon_chile_cabbage_kimchi", RemoveDrugsAndFoodEffects: false, Ingredients: []) { Compatibility = new("ItemSeederPreIndustrialFood_EarlyModern_Prepared_Preserved_earlymodern_food_joseon_chile_cabbage_kimchi", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingMakeEarlyModernPreparedPreservedFoodCatalogueStock() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/make EarlyModern Prepared preserved food catalogue stock", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make EarlyModern Prepared preserved food catalogue stock",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for earlymodern_food_naval_salmagundi_platter.",
		Action = "preparing preserved food",
		ActiveCraftItemSdesc = "a preserved preparation",
		Gate = CatalogueRecipeGates.PreIndustrialFoodCatalogueProductionCooking156,
		Trait = null,
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
			new CatalogueCommodityTagRecipeInput(Grams: 1000.0, MaterialTag: "Food Crop", PileTag: "Seeded Yield", CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1 kilogram of a material tagged as Food Crop", ["piletag Seeded Yield"]), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Cooking", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Cooking tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueProgCookedRecipeProduct(Prog: "ItemSeederPreIndustrialFood_EarlyModern_Prepared_Preserved_earlymodern_food_naval_salmagundi_platter", RemoveDrugsAndFoodEffects: false, Ingredients: []) { Compatibility = new("ItemSeederPreIndustrialFood_EarlyModern_Prepared_Preserved_earlymodern_food_naval_salmagundi_platter", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingMakeEarlyModernPreparedPreservedFishCatalogueStock() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/make EarlyModern Prepared preserved fish catalogue stock", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make EarlyModern Prepared preserved fish catalogue stock",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for earlymodern_food_tokugawa_miso_pickled_fish.",
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
			new CatalogueTagRecipeInput(Tag: "Raw Fish Cut", Quantity: 1) { Compatibility = new("1x an item with the Raw Fish Cut tag", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Cooking", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Cooking tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueProgCookedRecipeProduct(Prog: "ItemSeederPreIndustrialFood_EarlyModern_Prepared_Preserved_earlymodern_food_tokugawa_miso_pickled_fish", RemoveDrugsAndFoodEffects: false, Ingredients: []) { Compatibility = new("ItemSeederPreIndustrialFood_EarlyModern_Prepared_Preserved_earlymodern_food_tokugawa_miso_pickled_fish", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingMakeEarlyModernPreparedFruitFoodCatalogueStock() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/make EarlyModern Prepared fruit food catalogue stock", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make EarlyModern Prepared fruit food catalogue stock",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for earlymodern_food_ottoman_quince_jam_cheese_plate.",
		Action = "preparing fruit food",
		ActiveCraftItemSdesc = "a fruit preparation",
		Gate = CatalogueRecipeGates.PreIndustrialFoodCatalogueProductionCooking157,
		Trait = null,
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
			new(25, "$0 mix|mixes the fruit stock into an even preparation, using $t1.", "$0 mix|mixes the fruit stock into an even preparation, using $t1.", ExertionLevel.Stasis, 0),
			new(20, "$0 shape|shapes $p1 and set|sets it aside to cool and firm.", "$0 break|breaks the finished sweet and save|saves the crumbs for porridge.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityTagRecipeInput(Grams: 1000.0, MaterialTag: "Fruit", PileTag: "Seeded Yield", CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1 kilogram of a material tagged as Fruit", ["piletag Seeded Yield"]), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Cooking", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Cooking tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueProgCookedRecipeProduct(Prog: "ItemSeederPreIndustrialFood_EarlyModern_Prepared_Fruit_earlymodern_food_ottoman_quince_jam_cheese_plate", RemoveDrugsAndFoodEffects: false, Ingredients: []) { Compatibility = new("ItemSeederPreIndustrialFood_EarlyModern_Prepared_Fruit_earlymodern_food_ottoman_quince_jam_cheese_plate", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingMakeEarlyModernPreparedSweetBreadCatalogueStock() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/make EarlyModern Prepared sweet bread catalogue stock", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make EarlyModern Prepared sweet bread catalogue stock",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for earlymodern_food_british_cloth_boiled_plum_pudding.",
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
			new CatalogueProgCookedRecipeProduct(Prog: "ItemSeederPreIndustrialFood_EarlyModern_Prepared_Sweet_earlymodern_food_british_bath_sugar_bun", RemoveDrugsAndFoodEffects: false, Ingredients: []) { Compatibility = new("ItemSeederPreIndustrialFood_EarlyModern_Prepared_Sweet_earlymodern_food_british_bath_sugar_bun", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingMakeEarlyModernPreparedSweetDoughCatalogueStock() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/make EarlyModern Prepared sweet dough catalogue stock", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make EarlyModern Prepared sweet dough catalogue stock",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for earlymodern_food_british_sussex_like_pond_pudding.",
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
			new CatalogueProgCookedRecipeProduct(Prog: "ItemSeederPreIndustrialFood_EarlyModern_Prepared_Sweet_earlymodern_food_british_sussex_like_pond_pudding", RemoveDrugsAndFoodEffects: false, Ingredients: []) { Compatibility = new("ItemSeederPreIndustrialFood_EarlyModern_Prepared_Sweet_earlymodern_food_british_sussex_like_pond_pudding", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingMakeEarlyModernPreparedSweetRiceCatalogueStock() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/make EarlyModern Prepared sweet rice catalogue stock", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make EarlyModern Prepared sweet rice catalogue stock",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for earlymodern_food_dutch_company_rice_tart.",
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
			new CatalogueProgCookedRecipeProduct(Prog: "ItemSeederPreIndustrialFood_EarlyModern_Prepared_Sweet_earlymodern_food_dutch_company_rice_tart", RemoveDrugsAndFoodEffects: false, Ingredients: []) { Compatibility = new("ItemSeederPreIndustrialFood_EarlyModern_Prepared_Sweet_earlymodern_food_dutch_company_rice_tart", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingMakeEarlyModernPreparedSweetFoodCatalogueStock() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/make EarlyModern Prepared sweet food catalogue stock", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make EarlyModern Prepared sweet food catalogue stock",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for earlymodern_food_french_almond_macaron_pair.",
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
			new CatalogueProgCookedRecipeProduct(Prog: "ItemSeederPreIndustrialFood_EarlyModern_Prepared_Sweet_earlymodern_food_british_chocolate_puff", RemoveDrugsAndFoodEffects: false, Ingredients: []) { Compatibility = new("ItemSeederPreIndustrialFood_EarlyModern_Prepared_Sweet_earlymodern_food_british_chocolate_puff", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingMakeEarlyModernPreparedSweetEggCatalogueStock() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/make EarlyModern Prepared sweet egg catalogue stock", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make EarlyModern Prepared sweet egg catalogue stock",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for earlymodern_food_french_meringue_kisses.",
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
			new CatalogueCommodityTagRecipeInput(Grams: 1000.0, MaterialTag: "Egg Product", PileTag: "Egg Product", CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1 kilogram of a material tagged as Egg Product", ["piletag Egg Product"]), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Cooking", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Cooking tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueProgCookedRecipeProduct(Prog: "ItemSeederPreIndustrialFood_EarlyModern_Prepared_Sweet_earlymodern_food_brazilian_sugar_egg_convent_sweet", RemoveDrugsAndFoodEffects: false, Ingredients: []) { Compatibility = new("ItemSeederPreIndustrialFood_EarlyModern_Prepared_Sweet_earlymodern_food_brazilian_sugar_egg_convent_sweet", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingMakeEarlyModernPreparedSweetCheeseCatalogueStock() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/make EarlyModern Prepared sweet cheese catalogue stock", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make EarlyModern Prepared sweet cheese catalogue stock",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for earlymodern_food_french_pistachio_like_ice_cream.",
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
			new CatalogueProgCookedRecipeProduct(Prog: "ItemSeederPreIndustrialFood_EarlyModern_Prepared_Sweet_earlymodern_food_british_syllabub_cream", RemoveDrugsAndFoodEffects: false, Ingredients: []) { Compatibility = new("ItemSeederPreIndustrialFood_EarlyModern_Prepared_Sweet_earlymodern_food_british_syllabub_cream", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingMakeEarlyModernPreparedSweetWheatCatalogueStock() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/make EarlyModern Prepared sweet wheat catalogue stock", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make EarlyModern Prepared sweet wheat catalogue stock",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for earlymodern_food_french_puff_pastry_cream_slice.",
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
			new CatalogueProgCookedRecipeProduct(Prog: "ItemSeederPreIndustrialFood_EarlyModern_Prepared_Sweet_earlymodern_food_british_caraway_seed_cake", RemoveDrugsAndFoodEffects: false, Ingredients: []) { Compatibility = new("ItemSeederPreIndustrialFood_EarlyModern_Prepared_Sweet_earlymodern_food_british_caraway_seed_cake", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingMakeEarlyModernPreparedSweetHoneyCatalogueStock() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/make EarlyModern Prepared sweet honey catalogue stock", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make EarlyModern Prepared sweet honey catalogue stock",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for earlymodern_food_joseon_gangjeong_confection.",
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
			new CatalogueProgCookedRecipeProduct(Prog: "ItemSeederPreIndustrialFood_EarlyModern_Prepared_Sweet_earlymodern_food_caribbean_plantation_sugar_cake", RemoveDrugsAndFoodEffects: false, Ingredients: []) { Compatibility = new("ItemSeederPreIndustrialFood_EarlyModern_Prepared_Sweet_earlymodern_food_caribbean_plantation_sugar_cake", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingMakeEarlyModernPreparedSweetVegetableCatalogueStock() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/make EarlyModern Prepared sweet vegetable catalogue stock", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make EarlyModern Prepared sweet vegetable catalogue stock",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for earlymodern_food_qing_candied_sweet_potato.",
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
			new CatalogueProgCookedRecipeProduct(Prog: "ItemSeederPreIndustrialFood_EarlyModern_Prepared_Sweet_earlymodern_food_mexican_candied_sweet_potato", RemoveDrugsAndFoodEffects: false, Ingredients: []) { Compatibility = new("ItemSeederPreIndustrialFood_EarlyModern_Prepared_Sweet_earlymodern_food_mexican_candied_sweet_potato", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingMakeEarlyModernPreparedCondimentBreadCatalogueStock() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/make EarlyModern Prepared condiment bread catalogue stock", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make EarlyModern Prepared condiment bread catalogue stock",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for earlymodern_food_company_mango_like_chutney_toast.",
		Action = "preparing condiment food",
		ActiveCraftItemSdesc = "a condiment preparation",
		Gate = CatalogueRecipeGates.PreIndustrialFoodCatalogueProductionCooking1510,
		Trait = null,
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
			new(15, "$0 grind|grinds $i1 and measure|measures the sharp stock into the bowl.", "$0 grind|grinds $i1 and measure|measures the sharp stock into the bowl.", ExertionLevel.Stasis, 0),
			new(25, "$0 stir|stirs the condiment until its flavour and body come together, using $t1.", "$0 stir|stirs the condiment until its flavour and body come together, using $t1.", ExertionLevel.Stasis, 0),
			new(20, "$0 jar|jars $p1 and label|labels it for the next meal.", "$0 taste|tastes the harsh condiment and reserve|reserves it for a stronger batch.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityTagRecipeInput(Grams: 1000.0, MaterialTag: "Food Crop", PileTag: "Seeded Yield", CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1 kilogram of a material tagged as Food Crop", ["piletag Seeded Yield"]), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Cooking", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Cooking tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueProgCookedRecipeProduct(Prog: "ItemSeederPreIndustrialFood_EarlyModern_Prepared_Condiment_earlymodern_food_company_mango_like_chutney_toast", RemoveDrugsAndFoodEffects: false, Ingredients: []) { Compatibility = new("ItemSeederPreIndustrialFood_EarlyModern_Prepared_Condiment_earlymodern_food_company_mango_like_chutney_toast", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingFillCatalogueLiquidEarlymodernFoodLiquidCompanyTamarindPunch() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/fill catalogue liquid earlymodern_food_liquid_company_tamarind_punch", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:preindustrial_food_catalogue_liquid_amphora"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "fill catalogue liquid earlymodern_food_liquid_company_tamarind_punch",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for earlymodern_food_liquid_company_tamarind_punch.",
		Action = "preparing fruit drink food",
		ActiveCraftItemSdesc = "a fruit drink preparation",
		Gate = CatalogueRecipeGates.PreIndustrialFoodCatalogueProductionCooking1513,
		Trait = null,
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
			new(15, "$0 crush|crushes $i1 and load|loads the fruit into the press.", "$0 crush|crushes $i1 and load|loads the fruit into the press.", ExertionLevel.Stasis, 0),
			new(25, "$0 press|presses the fruit down and draw|draws off its bright juice, using $t1.", "$0 press|presses the fruit down and draw|draws off its bright juice, using $t1.", ExertionLevel.Stasis, 0),
			new(20, "$0 strain|strains $p1 and set|sets the drink aside to settle.", "$0 strain|strains the poor drink and save|saves what can be used in a sauce.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityTagRecipeInput(Grams: 1000.0, MaterialTag: "Fruit", PileTag: "Seeded Yield", CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1 kilogram of a material tagged as Fruit", ["piletag Seeded Yield"]), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Cooking", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Cooking tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueLiquidRecipeProduct(ItemStableReference: "preindustrial_food_catalogue_liquid_amphora", Quantity: 1, SkinStableReference: null, Liquid: "company-settlement tamarind punch", Litres: 10.0) { Compatibility = new("1x a shared catalogue liquid amphora (@preindustrial_food_catalogue_liquid_amphora) filled with 10 litres of company-settlement tamarind punch", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingFillCatalogueLiquidEarlymodernFoodLiquidJoseonOmijaPunch() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/fill catalogue liquid earlymodern_food_liquid_joseon_omija_punch", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:preindustrial_food_catalogue_liquid_amphora"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "fill catalogue liquid earlymodern_food_liquid_joseon_omija_punch",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for earlymodern_food_liquid_joseon_omija_punch.",
		Action = "preparing fruit drink food",
		ActiveCraftItemSdesc = "a fruit drink preparation",
		Gate = CatalogueRecipeGates.PreIndustrialFoodCatalogueProductionCooking1513,
		Trait = null,
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
			new(15, "$0 crush|crushes $i1 and load|loads the fruit into the press.", "$0 crush|crushes $i1 and load|loads the fruit into the press.", ExertionLevel.Stasis, 0),
			new(25, "$0 press|presses the fruit down and draw|draws off its bright juice, using $t1.", "$0 press|presses the fruit down and draw|draws off its bright juice, using $t1.", ExertionLevel.Stasis, 0),
			new(20, "$0 strain|strains $p1 and set|sets the drink aside to settle.", "$0 strain|strains the poor drink and save|saves what can be used in a sauce.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityTagRecipeInput(Grams: 1000.0, MaterialTag: "Fruit", PileTag: "Seeded Yield", CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1 kilogram of a material tagged as Fruit", ["piletag Seeded Yield"]), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Cooking", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Cooking tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueLiquidRecipeProduct(ItemStableReference: "preindustrial_food_catalogue_liquid_amphora", Quantity: 1, SkinStableReference: null, Liquid: "Joseon omija-cinnamon punch", Litres: 10.0) { Compatibility = new("1x a shared catalogue liquid amphora (@preindustrial_food_catalogue_liquid_amphora) filled with 10 litres of Joseon omija-cinnamon punch", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingFillCatalogueLiquidEarlymodernFoodLiquidShipboardSpruceBeer() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/fill catalogue liquid earlymodern_food_liquid_shipboard_spruce_beer", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:preindustrial_food_catalogue_liquid_amphora"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "fill catalogue liquid earlymodern_food_liquid_shipboard_spruce_beer",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for earlymodern_food_liquid_shipboard_spruce_beer.",
		Action = "preparing fermented drink food",
		ActiveCraftItemSdesc = "a fermented drink preparation",
		Gate = CatalogueRecipeGates.PreIndustrialFoodCatalogueProductionBrewing20,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Hard,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 3,
		Interruptable = false,
		Phases = [
			new(15, "$0 crush|crushes $i1 and stir|stirs it into the brewing vessel.", "$0 crush|crushes $i1 and stir|stirs it into the brewing vessel.", ExertionLevel.Stasis, 0),
			new(25, "$0 mash|mashes the fermented drink stock in hot water, then draw|draws off the sweet wort, using $t1.", "$0 mash|mashes the fermented drink stock in hot water, then draw|draws off the sweet wort, using $t1.", ExertionLevel.Stasis, 0),
			new(20, "$0 strain|strains $p1 and set|sets it aside to ferment and settle.", "$0 taste|tastes the failed brew and set|sets it aside as sour stock.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityTagRecipeInput(Grams: 1000.0, MaterialTag: "Food Crop", PileTag: "Seeded Yield", CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1 kilogram of a material tagged as Food Crop", ["piletag Seeded Yield"]), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Brew Copper", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Brew Copper tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueLiquidRecipeProduct(ItemStableReference: "preindustrial_food_catalogue_liquid_amphora", Quantity: 1, SkinStableReference: null, Liquid: "shipboard spruce-molasses beer", Litres: 10.0) { Compatibility = new("1x a shared catalogue liquid amphora (@preindustrial_food_catalogue_liquid_amphora) filled with 10 litres of shipboard spruce-molasses beer", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingFillCatalogueLiquidEarlymodernFoodLiquidBataviaArrackPunch() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/fill catalogue liquid earlymodern_food_liquid_batavia_arrack_punch", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:preindustrial_food_catalogue_liquid_amphora"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "fill catalogue liquid earlymodern_food_liquid_batavia_arrack_punch",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for earlymodern_food_liquid_batavia_arrack_punch.",
		Action = "preparing spirit food",
		ActiveCraftItemSdesc = "a spirit preparation",
		Gate = CatalogueRecipeGates.PreIndustrialFoodCatalogueProductionBrewing203,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Hard,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 3,
		Interruptable = false,
		Phases = [
			new(15, "$0 crush|crushes $i1 and stir|stirs it into the brewing vessel.", "$0 crush|crushes $i1 and stir|stirs it into the brewing vessel.", ExertionLevel.Stasis, 0),
			new(25, "$0 mash|mashes the spirit stock in hot water, then draw|draws off the sweet wort, using $t1.", "$0 mash|mashes the spirit stock in hot water, then draw|draws off the sweet wort, using $t1.", ExertionLevel.Stasis, 0),
			new(20, "$0 strain|strains $p1 and set|sets it aside to ferment and settle.", "$0 taste|tastes the failed brew and set|sets it aside as sour stock.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityTagRecipeInput(Grams: 1000.0, MaterialTag: "Food Crop", PileTag: "Seeded Yield", CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1 kilogram of a material tagged as Food Crop", ["piletag Seeded Yield"]), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Brew Copper", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Brew Copper tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueLiquidRecipeProduct(ItemStableReference: "preindustrial_food_catalogue_liquid_amphora", Quantity: 1, SkinStableReference: null, Liquid: "Batavia arrack punch", Litres: 10.0) { Compatibility = new("1x a shared catalogue liquid amphora (@preindustrial_food_catalogue_liquid_amphora) filled with 10 litres of Batavia arrack punch", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingFillCatalogueLiquidEarlymodernFoodLiquidCherryRatafia() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/fill catalogue liquid earlymodern_food_liquid_cherry_ratafia", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:preindustrial_food_catalogue_liquid_amphora"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "fill catalogue liquid earlymodern_food_liquid_cherry_ratafia",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for earlymodern_food_liquid_cherry_ratafia.",
		Action = "preparing spirit food",
		ActiveCraftItemSdesc = "a spirit preparation",
		Gate = CatalogueRecipeGates.PreIndustrialFoodCatalogueProductionBrewing203,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Hard,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 3,
		Interruptable = false,
		Phases = [
			new(15, "$0 crush|crushes $i1 and stir|stirs it into the brewing vessel.", "$0 crush|crushes $i1 and stir|stirs it into the brewing vessel.", ExertionLevel.Stasis, 0),
			new(25, "$0 mash|mashes the spirit stock in hot water, then draw|draws off the sweet wort, using $t1.", "$0 mash|mashes the spirit stock in hot water, then draw|draws off the sweet wort, using $t1.", ExertionLevel.Stasis, 0),
			new(20, "$0 strain|strains $p1 and set|sets it aside to ferment and settle.", "$0 taste|tastes the failed brew and set|sets it aside as sour stock.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityTagRecipeInput(Grams: 1000.0, MaterialTag: "Food Crop", PileTag: "Seeded Yield", CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1 kilogram of a material tagged as Food Crop", ["piletag Seeded Yield"]), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Brew Copper", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Brew Copper tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueLiquidRecipeProduct(ItemStableReference: "preindustrial_food_catalogue_liquid_amphora", Quantity: 1, SkinStableReference: null, Liquid: "cherry-kernel ratafia cordial", Litres: 10.0) { Compatibility = new("1x a shared catalogue liquid amphora (@preindustrial_food_catalogue_liquid_amphora) filled with 10 litres of cherry-kernel ratafia cordial", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingFillCatalogueLiquidEarlymodernFoodLiquidJuniperCorianderCordial() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/fill catalogue liquid earlymodern_food_liquid_juniper_coriander_cordial", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:preindustrial_food_catalogue_liquid_amphora"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "fill catalogue liquid earlymodern_food_liquid_juniper_coriander_cordial",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for earlymodern_food_liquid_juniper_coriander_cordial.",
		Action = "preparing spirit food",
		ActiveCraftItemSdesc = "a spirit preparation",
		Gate = CatalogueRecipeGates.PreIndustrialFoodCatalogueProductionBrewing203,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Hard,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 3,
		Interruptable = false,
		Phases = [
			new(15, "$0 crush|crushes $i1 and stir|stirs it into the brewing vessel.", "$0 crush|crushes $i1 and stir|stirs it into the brewing vessel.", ExertionLevel.Stasis, 0),
			new(25, "$0 mash|mashes the spirit stock in hot water, then draw|draws off the sweet wort, using $t1.", "$0 mash|mashes the spirit stock in hot water, then draw|draws off the sweet wort, using $t1.", ExertionLevel.Stasis, 0),
			new(20, "$0 strain|strains $p1 and set|sets it aside to ferment and settle.", "$0 taste|tastes the failed brew and set|sets it aside as sour stock.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityTagRecipeInput(Grams: 1000.0, MaterialTag: "Food Crop", PileTag: "Seeded Yield", CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1 kilogram of a material tagged as Food Crop", ["piletag Seeded Yield"]), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Brew Copper", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Brew Copper tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueLiquidRecipeProduct(ItemStableReference: "preindustrial_food_catalogue_liquid_amphora", Quantity: 1, SkinStableReference: null, Liquid: "juniper-coriander cordial spirit", Litres: 10.0) { Compatibility = new("1x a shared catalogue liquid amphora (@preindustrial_food_catalogue_liquid_amphora) filled with 10 litres of juniper-coriander cordial spirit", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingFillCatalogueLiquidEarlymodernFoodLiquidOrangeflowerBrandyCordial() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/fill catalogue liquid earlymodern_food_liquid_orangeflower_brandy_cordial", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:preindustrial_food_catalogue_liquid_amphora"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "fill catalogue liquid earlymodern_food_liquid_orangeflower_brandy_cordial",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for earlymodern_food_liquid_orangeflower_brandy_cordial.",
		Action = "preparing spirit food",
		ActiveCraftItemSdesc = "a spirit preparation",
		Gate = CatalogueRecipeGates.PreIndustrialFoodCatalogueProductionBrewing203,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Hard,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 3,
		Interruptable = false,
		Phases = [
			new(15, "$0 crush|crushes $i1 and stir|stirs it into the brewing vessel.", "$0 crush|crushes $i1 and stir|stirs it into the brewing vessel.", ExertionLevel.Stasis, 0),
			new(25, "$0 mash|mashes the spirit stock in hot water, then draw|draws off the sweet wort, using $t1.", "$0 mash|mashes the spirit stock in hot water, then draw|draws off the sweet wort, using $t1.", ExertionLevel.Stasis, 0),
			new(20, "$0 strain|strains $p1 and set|sets it aside to ferment and settle.", "$0 taste|tastes the failed brew and set|sets it aside as sour stock.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityTagRecipeInput(Grams: 1000.0, MaterialTag: "Food Crop", PileTag: "Seeded Yield", CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1 kilogram of a material tagged as Food Crop", ["piletag Seeded Yield"]), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Brew Copper", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Brew Copper tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueLiquidRecipeProduct(ItemStableReference: "preindustrial_food_catalogue_liquid_amphora", Quantity: 1, SkinStableReference: null, Liquid: "orange-flower brandy cordial", Litres: 10.0) { Compatibility = new("1x a shared catalogue liquid amphora (@preindustrial_food_catalogue_liquid_amphora) filled with 10 litres of orange-flower brandy cordial", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingFillCatalogueLiquidEarlymodernFoodLiquidVernonNavalGrog() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/fill catalogue liquid earlymodern_food_liquid_vernon_naval_grog", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:preindustrial_food_catalogue_liquid_amphora"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "fill catalogue liquid earlymodern_food_liquid_vernon_naval_grog",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for earlymodern_food_liquid_vernon_naval_grog.",
		Action = "preparing spirit food",
		ActiveCraftItemSdesc = "a spirit preparation",
		Gate = CatalogueRecipeGates.PreIndustrialFoodCatalogueProductionBrewing203,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Hard,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 3,
		Interruptable = false,
		Phases = [
			new(15, "$0 crush|crushes $i1 and stir|stirs it into the brewing vessel.", "$0 crush|crushes $i1 and stir|stirs it into the brewing vessel.", ExertionLevel.Stasis, 0),
			new(25, "$0 mash|mashes the spirit stock in hot water, then draw|draws off the sweet wort, using $t1.", "$0 mash|mashes the spirit stock in hot water, then draw|draws off the sweet wort, using $t1.", ExertionLevel.Stasis, 0),
			new(20, "$0 strain|strains $p1 and set|sets it aside to ferment and settle.", "$0 taste|tastes the failed brew and set|sets it aside as sour stock.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityTagRecipeInput(Grams: 1000.0, MaterialTag: "Food Crop", PileTag: "Seeded Yield", CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1 kilogram of a material tagged as Food Crop", ["piletag Seeded Yield"]), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Brew Copper", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Brew Copper tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueLiquidRecipeProduct(ItemStableReference: "preindustrial_food_catalogue_liquid_amphora", Quantity: 1, SkinStableReference: null, Liquid: "Vernon naval rum-water", Litres: 10.0) { Compatibility = new("1x a shared catalogue liquid amphora (@preindustrial_food_catalogue_liquid_amphora) filled with 10 litres of Vernon naval rum-water", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingFillCatalogueLiquidEarlymodernFoodLiquidCompanyBoheaTea() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/fill catalogue liquid earlymodern_food_liquid_company_bohea_tea", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:preindustrial_food_catalogue_liquid_amphora"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "fill catalogue liquid earlymodern_food_liquid_company_bohea_tea",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for earlymodern_food_liquid_company_bohea_tea.",
		Action = "preparing tea food",
		ActiveCraftItemSdesc = "a tea preparation",
		Gate = CatalogueRecipeGates.PreIndustrialFoodCatalogueProductionCooking1515,
		Trait = null,
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
			new(15, "$0 roast|roasts and crack|cracks $i1 until its fragrance rises.", "$0 roast|roasts and crack|cracks $i1 until its fragrance rises.", ExertionLevel.Stasis, 0),
			new(25, "$0 steep|steeps the tea stock in clean water, drawing out its colour, using $t1.", "$0 steep|steeps the tea stock in clean water, drawing out its colour, using $t1.", ExertionLevel.Stasis, 0),
			new(20, "$0 strain|strains $p1 into a serving vessel and let|lets it cool.", "$0 pour|pours off the weak drink and reserve|reserves it for cooking.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityTagRecipeInput(Grams: 1000.0, MaterialTag: "Food Crop", PileTag: "Seeded Yield", CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1 kilogram of a material tagged as Food Crop", ["piletag Seeded Yield"]), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Cooking", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Cooking tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueLiquidRecipeProduct(ItemStableReference: "preindustrial_food_catalogue_liquid_amphora", Quantity: 1, SkinStableReference: null, Liquid: "company-house bohea tea liquor", Litres: 10.0) { Compatibility = new("1x a shared catalogue liquid amphora (@preindustrial_food_catalogue_liquid_amphora) filled with 10 litres of company-house bohea tea liquor", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingFillCatalogueLiquidEarlymodernFoodLiquidQingJasmineTea() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/fill catalogue liquid earlymodern_food_liquid_qing_jasmine_tea", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:preindustrial_food_catalogue_liquid_amphora"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "fill catalogue liquid earlymodern_food_liquid_qing_jasmine_tea",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for earlymodern_food_liquid_qing_jasmine_tea.",
		Action = "preparing tea food",
		ActiveCraftItemSdesc = "a tea preparation",
		Gate = CatalogueRecipeGates.PreIndustrialFoodCatalogueProductionCooking1515,
		Trait = null,
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
			new(15, "$0 roast|roasts and crack|cracks $i1 until its fragrance rises.", "$0 roast|roasts and crack|cracks $i1 until its fragrance rises.", ExertionLevel.Stasis, 0),
			new(25, "$0 steep|steeps the tea stock in clean water, drawing out its colour, using $t1.", "$0 steep|steeps the tea stock in clean water, drawing out its colour, using $t1.", ExertionLevel.Stasis, 0),
			new(20, "$0 strain|strains $p1 into a serving vessel and let|lets it cool.", "$0 pour|pours off the weak drink and reserve|reserves it for cooking.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityTagRecipeInput(Grams: 1000.0, MaterialTag: "Food Crop", PileTag: "Seeded Yield", CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1 kilogram of a material tagged as Food Crop", ["piletag Seeded Yield"]), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Cooking", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Cooking tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueLiquidRecipeProduct(ItemStableReference: "preindustrial_food_catalogue_liquid_amphora", Quantity: 1, SkinStableReference: null, Liquid: "Qing jasmine-scented tea liquor", Litres: 10.0) { Compatibility = new("1x a shared catalogue liquid amphora (@preindustrial_food_catalogue_liquid_amphora) filled with 10 litres of Qing jasmine-scented tea liquor", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingFillCatalogueLiquidEarlymodernFoodLiquidTokugawaRoastedTwigTea() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/fill catalogue liquid earlymodern_food_liquid_tokugawa_roasted_twig_tea", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:preindustrial_food_catalogue_liquid_amphora"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "fill catalogue liquid earlymodern_food_liquid_tokugawa_roasted_twig_tea",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for earlymodern_food_liquid_tokugawa_roasted_twig_tea.",
		Action = "preparing tea food",
		ActiveCraftItemSdesc = "a tea preparation",
		Gate = CatalogueRecipeGates.PreIndustrialFoodCatalogueProductionCooking1515,
		Trait = null,
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
			new(15, "$0 roast|roasts and crack|cracks $i1 until its fragrance rises.", "$0 roast|roasts and crack|cracks $i1 until its fragrance rises.", ExertionLevel.Stasis, 0),
			new(25, "$0 steep|steeps the tea stock in clean water, drawing out its colour, using $t1.", "$0 steep|steeps the tea stock in clean water, drawing out its colour, using $t1.", ExertionLevel.Stasis, 0),
			new(20, "$0 strain|strains $p1 into a serving vessel and let|lets it cool.", "$0 pour|pours off the weak drink and reserve|reserves it for cooking.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityTagRecipeInput(Grams: 1000.0, MaterialTag: "Food Crop", PileTag: "Seeded Yield", CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1 kilogram of a material tagged as Food Crop", ["piletag Seeded Yield"]), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Cooking", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Cooking tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueLiquidRecipeProduct(ItemStableReference: "preindustrial_food_catalogue_liquid_amphora", Quantity: 1, SkinStableReference: null, Liquid: "Tokugawa roasted-twig tea liquor", Litres: 10.0) { Compatibility = new("1x a shared catalogue liquid amphora (@preindustrial_food_catalogue_liquid_amphora) filled with 10 litres of Tokugawa roasted-twig tea liquor", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingFillCatalogueLiquidEarlymodernFoodLiquidMughalAlmondCoffee() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/fill catalogue liquid earlymodern_food_liquid_mughal_almond_coffee", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:preindustrial_food_catalogue_liquid_amphora"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "fill catalogue liquid earlymodern_food_liquid_mughal_almond_coffee",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for earlymodern_food_liquid_mughal_almond_coffee.",
		Action = "preparing coffee food",
		ActiveCraftItemSdesc = "a coffee preparation",
		Gate = CatalogueRecipeGates.PreIndustrialFoodCatalogueProductionCooking1516,
		Trait = null,
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
			new(15, "$0 roast|roasts and crack|cracks $i1 until its fragrance rises.", "$0 roast|roasts and crack|cracks $i1 until its fragrance rises.", ExertionLevel.Stasis, 0),
			new(25, "$0 steep|steeps the coffee stock in clean water, drawing out its colour, using $t1.", "$0 steep|steeps the coffee stock in clean water, drawing out its colour, using $t1.", ExertionLevel.Stasis, 0),
			new(20, "$0 strain|strains $p1 into a serving vessel and let|lets it cool.", "$0 pour|pours off the weak drink and reserve|reserves it for cooking.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityTagRecipeInput(Grams: 1000.0, MaterialTag: "Food Crop", PileTag: "Seeded Yield", CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1 kilogram of a material tagged as Food Crop", ["piletag Seeded Yield"]), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Cooking", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Cooking tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueLiquidRecipeProduct(ItemStableReference: "preindustrial_food_catalogue_liquid_amphora", Quantity: 1, SkinStableReference: null, Liquid: "Mughal almond-cardamom coffee", Litres: 10.0) { Compatibility = new("1x a shared catalogue liquid amphora (@preindustrial_food_catalogue_liquid_amphora) filled with 10 litres of Mughal almond-cardamom coffee", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingFillCatalogueLiquidEarlymodernFoodLiquidOttomanCloveCoffee() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/fill catalogue liquid earlymodern_food_liquid_ottoman_clove_coffee", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:preindustrial_food_catalogue_liquid_amphora"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "fill catalogue liquid earlymodern_food_liquid_ottoman_clove_coffee",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for earlymodern_food_liquid_ottoman_clove_coffee.",
		Action = "preparing coffee food",
		ActiveCraftItemSdesc = "a coffee preparation",
		Gate = CatalogueRecipeGates.PreIndustrialFoodCatalogueProductionCooking1516,
		Trait = null,
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
			new(15, "$0 roast|roasts and crack|cracks $i1 until its fragrance rises.", "$0 roast|roasts and crack|cracks $i1 until its fragrance rises.", ExertionLevel.Stasis, 0),
			new(25, "$0 steep|steeps the coffee stock in clean water, drawing out its colour, using $t1.", "$0 steep|steeps the coffee stock in clean water, drawing out its colour, using $t1.", ExertionLevel.Stasis, 0),
			new(20, "$0 strain|strains $p1 into a serving vessel and let|lets it cool.", "$0 pour|pours off the weak drink and reserve|reserves it for cooking.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityTagRecipeInput(Grams: 1000.0, MaterialTag: "Food Crop", PileTag: "Seeded Yield", CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1 kilogram of a material tagged as Food Crop", ["piletag Seeded Yield"]), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Cooking", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Cooking tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueLiquidRecipeProduct(ItemStableReference: "preindustrial_food_catalogue_liquid_amphora", Quantity: 1, SkinStableReference: null, Liquid: "Ottoman clove coffeehouse liquor", Litres: 10.0) { Compatibility = new("1x a shared catalogue liquid amphora (@preindustrial_food_catalogue_liquid_amphora) filled with 10 litres of Ottoman clove coffeehouse liquor", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingFillCatalogueLiquidEarlymodernFoodLiquidSafavidRoseCoffee() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/fill catalogue liquid earlymodern_food_liquid_safavid_rose_coffee", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:preindustrial_food_catalogue_liquid_amphora"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "fill catalogue liquid earlymodern_food_liquid_safavid_rose_coffee",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for earlymodern_food_liquid_safavid_rose_coffee.",
		Action = "preparing coffee food",
		ActiveCraftItemSdesc = "a coffee preparation",
		Gate = CatalogueRecipeGates.PreIndustrialFoodCatalogueProductionCooking1516,
		Trait = null,
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
			new(15, "$0 roast|roasts and crack|cracks $i1 until its fragrance rises.", "$0 roast|roasts and crack|cracks $i1 until its fragrance rises.", ExertionLevel.Stasis, 0),
			new(25, "$0 steep|steeps the coffee stock in clean water, drawing out its colour, using $t1.", "$0 steep|steeps the coffee stock in clean water, drawing out its colour, using $t1.", ExertionLevel.Stasis, 0),
			new(20, "$0 strain|strains $p1 into a serving vessel and let|lets it cool.", "$0 pour|pours off the weak drink and reserve|reserves it for cooking.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityTagRecipeInput(Grams: 1000.0, MaterialTag: "Food Crop", PileTag: "Seeded Yield", CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1 kilogram of a material tagged as Food Crop", ["piletag Seeded Yield"]), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Cooking", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Cooking tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueLiquidRecipeProduct(ItemStableReference: "preindustrial_food_catalogue_liquid_amphora", Quantity: 1, SkinStableReference: null, Liquid: "Safavid rosewater coffee", Litres: 10.0) { Compatibility = new("1x a shared catalogue liquid amphora (@preindustrial_food_catalogue_liquid_amphora) filled with 10 litres of Safavid rosewater coffee", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingFillCatalogueLiquidEarlymodernFoodLiquidCaribbeanCacaoShellInfusion() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/fill catalogue liquid earlymodern_food_liquid_caribbean_cacao_shell_infusion", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:preindustrial_food_catalogue_liquid_amphora"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "fill catalogue liquid earlymodern_food_liquid_caribbean_cacao_shell_infusion",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for earlymodern_food_liquid_caribbean_cacao_shell_infusion.",
		Action = "preparing chocolate food",
		ActiveCraftItemSdesc = "a chocolate preparation",
		Gate = CatalogueRecipeGates.PreIndustrialFoodCatalogueProductionCooking1517,
		Trait = null,
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
			new(15, "$0 roast|roasts and crack|cracks $i1 until its fragrance rises.", "$0 roast|roasts and crack|cracks $i1 until its fragrance rises.", ExertionLevel.Stasis, 0),
			new(25, "$0 steep|steeps the chocolate stock in clean water, drawing out its colour, using $t1.", "$0 steep|steeps the chocolate stock in clean water, drawing out its colour, using $t1.", ExertionLevel.Stasis, 0),
			new(20, "$0 strain|strains $p1 into a serving vessel and let|lets it cool.", "$0 pour|pours off the weak drink and reserve|reserves it for cooking.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityTagRecipeInput(Grams: 1000.0, MaterialTag: "Food Crop", PileTag: "Seeded Yield", CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1 kilogram of a material tagged as Food Crop", ["piletag Seeded Yield"]), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Cooking", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Cooking tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueLiquidRecipeProduct(ItemStableReference: "preindustrial_food_catalogue_liquid_amphora", Quantity: 1, SkinStableReference: null, Liquid: "Caribbean cacao-shell plantation infusion", Litres: 10.0) { Compatibility = new("1x a shared catalogue liquid amphora (@preindustrial_food_catalogue_liquid_amphora) filled with 10 litres of Caribbean cacao-shell plantation infusion", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingFillCatalogueLiquidEarlymodernFoodLiquidFrenchVanillaMilkChocolate() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/fill catalogue liquid earlymodern_food_liquid_french_vanilla_milk_chocolate", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:preindustrial_food_catalogue_liquid_amphora"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "fill catalogue liquid earlymodern_food_liquid_french_vanilla_milk_chocolate",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for earlymodern_food_liquid_french_vanilla_milk_chocolate.",
		Action = "preparing chocolate food",
		ActiveCraftItemSdesc = "a chocolate preparation",
		Gate = CatalogueRecipeGates.PreIndustrialFoodCatalogueProductionCooking1517,
		Trait = null,
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
			new(15, "$0 roast|roasts and crack|cracks $i1 until its fragrance rises.", "$0 roast|roasts and crack|cracks $i1 until its fragrance rises.", ExertionLevel.Stasis, 0),
			new(25, "$0 steep|steeps the chocolate stock in clean water, drawing out its colour, using $t1.", "$0 steep|steeps the chocolate stock in clean water, drawing out its colour, using $t1.", ExertionLevel.Stasis, 0),
			new(20, "$0 strain|strains $p1 into a serving vessel and let|lets it cool.", "$0 pour|pours off the weak drink and reserve|reserves it for cooking.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityTagRecipeInput(Grams: 1000.0, MaterialTag: "Food Crop", PileTag: "Seeded Yield", CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1 kilogram of a material tagged as Food Crop", ["piletag Seeded Yield"]), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Cooking", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Cooking tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueLiquidRecipeProduct(ItemStableReference: "preindustrial_food_catalogue_liquid_amphora", Quantity: 1, SkinStableReference: null, Liquid: "French court vanilla milk chocolate", Litres: 10.0) { Compatibility = new("1x a shared catalogue liquid amphora (@preindustrial_food_catalogue_liquid_amphora) filled with 10 litres of French court vanilla milk chocolate", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingFillCatalogueLiquidEarlymodernFoodLiquidSpanishCinnamonChocolate() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/fill catalogue liquid earlymodern_food_liquid_spanish_cinnamon_chocolate", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:preindustrial_food_catalogue_liquid_amphora"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "fill catalogue liquid earlymodern_food_liquid_spanish_cinnamon_chocolate",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for earlymodern_food_liquid_spanish_cinnamon_chocolate.",
		Action = "preparing chocolate food",
		ActiveCraftItemSdesc = "a chocolate preparation",
		Gate = CatalogueRecipeGates.PreIndustrialFoodCatalogueProductionCooking1517,
		Trait = null,
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
			new(15, "$0 roast|roasts and crack|cracks $i1 until its fragrance rises.", "$0 roast|roasts and crack|cracks $i1 until its fragrance rises.", ExertionLevel.Stasis, 0),
			new(25, "$0 steep|steeps the chocolate stock in clean water, drawing out its colour, using $t1.", "$0 steep|steeps the chocolate stock in clean water, drawing out its colour, using $t1.", ExertionLevel.Stasis, 0),
			new(20, "$0 strain|strains $p1 into a serving vessel and let|lets it cool.", "$0 pour|pours off the weak drink and reserve|reserves it for cooking.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityTagRecipeInput(Grams: 1000.0, MaterialTag: "Food Crop", PileTag: "Seeded Yield", CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1 kilogram of a material tagged as Food Crop", ["piletag Seeded Yield"]), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Cooking", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Cooking tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueLiquidRecipeProduct(ItemStableReference: "preindustrial_food_catalogue_liquid_amphora", Quantity: 1, SkinStableReference: null, Liquid: "Spanish Atlantic cinnamon chocolate", Litres: 10.0) { Compatibility = new("1x a shared catalogue liquid amphora (@preindustrial_food_catalogue_liquid_amphora) filled with 10 litres of Spanish Atlantic cinnamon chocolate", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingFillCatalogueLiquidEarlymodernFoodLiquidAnchovyLemonEssence() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/fill catalogue liquid earlymodern_food_liquid_anchovy_lemon_essence", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:preindustrial_food_catalogue_liquid_amphora"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "fill catalogue liquid earlymodern_food_liquid_anchovy_lemon_essence",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for earlymodern_food_liquid_anchovy_lemon_essence.",
		Action = "preparing sauce food",
		ActiveCraftItemSdesc = "a sauce preparation",
		Gate = CatalogueRecipeGates.PreIndustrialFoodCatalogueProductionCooking1518,
		Trait = null,
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
			new(15, "$0 crush|crushes $i1 and lay|lays out the sauce stock.", "$0 crush|crushes $i1 and lay|lays out the sauce stock.", ExertionLevel.Stasis, 0),
			new(25, "$0 work|works the sauce stock into a smooth liquid, following a household method, using $t1.", "$0 work|works the sauce stock into a smooth liquid, following a household method, using $t1.", ExertionLevel.Stasis, 0),
			new(20, "$0 inspect|inspects $p1 and set|sets the finished drink aside to settle.", "$0 inspect|inspects the poor drink and salvage|salvages the usable stock.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityTagRecipeInput(Grams: 1000.0, MaterialTag: "Food Crop", PileTag: "Seeded Yield", CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1 kilogram of a material tagged as Food Crop", ["piletag Seeded Yield"]), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Cooking", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Cooking tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueLiquidRecipeProduct(ItemStableReference: "preindustrial_food_catalogue_liquid_amphora", Quantity: 1, SkinStableReference: null, Liquid: "anchovy-lemon table essence", Litres: 10.0) { Compatibility = new("1x a shared catalogue liquid amphora (@preindustrial_food_catalogue_liquid_amphora) filled with 10 litres of anchovy-lemon table essence", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingFillCatalogueLiquidEarlymodernFoodLiquidButterCaperSauce() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/fill catalogue liquid earlymodern_food_liquid_butter_caper_sauce", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:preindustrial_food_catalogue_liquid_amphora"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "fill catalogue liquid earlymodern_food_liquid_butter_caper_sauce",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for earlymodern_food_liquid_butter_caper_sauce.",
		Action = "preparing sauce food",
		ActiveCraftItemSdesc = "a sauce preparation",
		Gate = CatalogueRecipeGates.PreIndustrialFoodCatalogueProductionCooking1518,
		Trait = null,
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
			new(15, "$0 crush|crushes $i1 and lay|lays out the sauce stock.", "$0 crush|crushes $i1 and lay|lays out the sauce stock.", ExertionLevel.Stasis, 0),
			new(25, "$0 work|works the sauce stock into a smooth liquid, following a household method, using $t1.", "$0 work|works the sauce stock into a smooth liquid, following a household method, using $t1.", ExertionLevel.Stasis, 0),
			new(20, "$0 inspect|inspects $p1 and set|sets the finished drink aside to settle.", "$0 inspect|inspects the poor drink and salvage|salvages the usable stock.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityTagRecipeInput(Grams: 1000.0, MaterialTag: "Food Crop", PileTag: "Seeded Yield", CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1 kilogram of a material tagged as Food Crop", ["piletag Seeded Yield"]), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Cooking", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Cooking tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueLiquidRecipeProduct(ItemStableReference: "preindustrial_food_catalogue_liquid_amphora", Quantity: 1, SkinStableReference: null, Liquid: "butter-mounted caper sauce", Litres: 10.0) { Compatibility = new("1x a shared catalogue liquid amphora (@preindustrial_food_catalogue_liquid_amphora) filled with 10 litres of butter-mounted caper sauce", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingFillCatalogueLiquidEarlymodernFoodLiquidCreamMushroomSauce() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/fill catalogue liquid earlymodern_food_liquid_cream_mushroom_sauce", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:preindustrial_food_catalogue_liquid_amphora"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "fill catalogue liquid earlymodern_food_liquid_cream_mushroom_sauce",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for earlymodern_food_liquid_cream_mushroom_sauce.",
		Action = "preparing sauce food",
		ActiveCraftItemSdesc = "a sauce preparation",
		Gate = CatalogueRecipeGates.PreIndustrialFoodCatalogueProductionCooking1518,
		Trait = null,
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
			new(15, "$0 crush|crushes $i1 and lay|lays out the sauce stock.", "$0 crush|crushes $i1 and lay|lays out the sauce stock.", ExertionLevel.Stasis, 0),
			new(25, "$0 work|works the sauce stock into a smooth liquid, following a household method, using $t1.", "$0 work|works the sauce stock into a smooth liquid, following a household method, using $t1.", ExertionLevel.Stasis, 0),
			new(20, "$0 inspect|inspects $p1 and set|sets the finished drink aside to settle.", "$0 inspect|inspects the poor drink and salvage|salvages the usable stock.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityTagRecipeInput(Grams: 1000.0, MaterialTag: "Food Crop", PileTag: "Seeded Yield", CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1 kilogram of a material tagged as Food Crop", ["piletag Seeded Yield"]), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Cooking", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Cooking tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueLiquidRecipeProduct(ItemStableReference: "preindustrial_food_catalogue_liquid_amphora", Quantity: 1, SkinStableReference: null, Liquid: "cream-bound mushroom sauce", Litres: 10.0) { Compatibility = new("1x a shared catalogue liquid amphora (@preindustrial_food_catalogue_liquid_amphora) filled with 10 litres of cream-bound mushroom sauce", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingFillCatalogueLiquidEarlymodernFoodLiquidRouxBrownGravy() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/fill catalogue liquid earlymodern_food_liquid_roux_brown_gravy", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:preindustrial_food_catalogue_liquid_amphora"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "fill catalogue liquid earlymodern_food_liquid_roux_brown_gravy",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for earlymodern_food_liquid_roux_brown_gravy.",
		Action = "preparing sauce food",
		ActiveCraftItemSdesc = "a sauce preparation",
		Gate = CatalogueRecipeGates.PreIndustrialFoodCatalogueProductionCooking1518,
		Trait = null,
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
			new(15, "$0 crush|crushes $i1 and lay|lays out the sauce stock.", "$0 crush|crushes $i1 and lay|lays out the sauce stock.", ExertionLevel.Stasis, 0),
			new(25, "$0 work|works the sauce stock into a smooth liquid, following a household method, using $t1.", "$0 work|works the sauce stock into a smooth liquid, following a household method, using $t1.", ExertionLevel.Stasis, 0),
			new(20, "$0 inspect|inspects $p1 and set|sets the finished drink aside to settle.", "$0 inspect|inspects the poor drink and salvage|salvages the usable stock.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityTagRecipeInput(Grams: 1000.0, MaterialTag: "Food Crop", PileTag: "Seeded Yield", CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1 kilogram of a material tagged as Food Crop", ["piletag Seeded Yield"]), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Cooking", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Cooking tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueLiquidRecipeProduct(ItemStableReference: "preindustrial_food_catalogue_liquid_amphora", Quantity: 1, SkinStableReference: null, Liquid: "roux-bound brown gravy", Litres: 10.0) { Compatibility = new("1x a shared catalogue liquid amphora (@preindustrial_food_catalogue_liquid_amphora) filled with 10 litres of roux-bound brown gravy", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingFillCatalogueLiquidEarlymodernFoodLiquidTokugawaPonzu() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/fill catalogue liquid earlymodern_food_liquid_tokugawa_ponzu", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:preindustrial_food_catalogue_liquid_amphora"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "fill catalogue liquid earlymodern_food_liquid_tokugawa_ponzu",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for earlymodern_food_liquid_tokugawa_ponzu.",
		Action = "preparing sauce food",
		ActiveCraftItemSdesc = "a sauce preparation",
		Gate = CatalogueRecipeGates.PreIndustrialFoodCatalogueProductionCooking1518,
		Trait = null,
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
			new(15, "$0 crush|crushes $i1 and lay|lays out the sauce stock.", "$0 crush|crushes $i1 and lay|lays out the sauce stock.", ExertionLevel.Stasis, 0),
			new(25, "$0 work|works the sauce stock into a smooth liquid, following a household method, using $t1.", "$0 work|works the sauce stock into a smooth liquid, following a household method, using $t1.", ExertionLevel.Stasis, 0),
			new(20, "$0 inspect|inspects $p1 and set|sets the finished drink aside to settle.", "$0 inspect|inspects the poor drink and salvage|salvages the usable stock.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityTagRecipeInput(Grams: 1000.0, MaterialTag: "Food Crop", PileTag: "Seeded Yield", CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1 kilogram of a material tagged as Food Crop", ["piletag Seeded Yield"]), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Cooking", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Cooking tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueLiquidRecipeProduct(ItemStableReference: "preindustrial_food_catalogue_liquid_amphora", Quantity: 1, SkinStableReference: null, Liquid: "Tokugawa citrus-soy ponzu", Litres: 10.0) { Compatibility = new("1x a shared catalogue liquid amphora (@preindustrial_food_catalogue_liquid_amphora) filled with 10 litres of Tokugawa citrus-soy ponzu", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingFillCatalogueLiquidEarlymodernFoodLiquidTomatoHerbSauce() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/fill catalogue liquid earlymodern_food_liquid_tomato_herb_sauce", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:preindustrial_food_catalogue_liquid_amphora"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "fill catalogue liquid earlymodern_food_liquid_tomato_herb_sauce",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for earlymodern_food_liquid_tomato_herb_sauce.",
		Action = "preparing sauce food",
		ActiveCraftItemSdesc = "a sauce preparation",
		Gate = CatalogueRecipeGates.PreIndustrialFoodCatalogueProductionCooking1518,
		Trait = null,
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
			new(15, "$0 crush|crushes $i1 and lay|lays out the sauce stock.", "$0 crush|crushes $i1 and lay|lays out the sauce stock.", ExertionLevel.Stasis, 0),
			new(25, "$0 work|works the sauce stock into a smooth liquid, following a household method, using $t1.", "$0 work|works the sauce stock into a smooth liquid, following a household method, using $t1.", ExertionLevel.Stasis, 0),
			new(20, "$0 inspect|inspects $p1 and set|sets the finished drink aside to settle.", "$0 inspect|inspects the poor drink and salvage|salvages the usable stock.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityTagRecipeInput(Grams: 1000.0, MaterialTag: "Food Crop", PileTag: "Seeded Yield", CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1 kilogram of a material tagged as Food Crop", ["piletag Seeded Yield"]), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Cooking", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Cooking tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueLiquidRecipeProduct(ItemStableReference: "preindustrial_food_catalogue_liquid_amphora", Quantity: 1, SkinStableReference: null, Liquid: "early tomato-herb cooking sauce", Litres: 10.0) { Compatibility = new("1x a shared catalogue liquid amphora (@preindustrial_food_catalogue_liquid_amphora) filled with 10 litres of early tomato-herb cooking sauce", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingFillCatalogueLiquidEarlymodernFoodLiquidCoffeehouseOrgeat() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/fill catalogue liquid earlymodern_food_liquid_coffeehouse_orgeat", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:preindustrial_food_catalogue_liquid_amphora"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "fill catalogue liquid earlymodern_food_liquid_coffeehouse_orgeat",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for earlymodern_food_liquid_coffeehouse_orgeat.",
		Action = "preparing syrup food",
		ActiveCraftItemSdesc = "a syrup preparation",
		Gate = CatalogueRecipeGates.PreIndustrialFoodCatalogueProductionCooking1521,
		Trait = null,
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
			new(15, "$0 crush|crushes $i1 and lay|lays out the syrup stock.", "$0 crush|crushes $i1 and lay|lays out the syrup stock.", ExertionLevel.Stasis, 0),
			new(25, "$0 work|works the syrup stock into a smooth liquid, following a household method, using $t1.", "$0 work|works the syrup stock into a smooth liquid, following a household method, using $t1.", ExertionLevel.Stasis, 0),
			new(20, "$0 inspect|inspects $p1 and set|sets the finished drink aside to settle.", "$0 inspect|inspects the poor drink and salvage|salvages the usable stock.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityTagRecipeInput(Grams: 1000.0, MaterialTag: "Food Crop", PileTag: "Seeded Yield", CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1 kilogram of a material tagged as Food Crop", ["piletag Seeded Yield"]), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Cooking", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Cooking tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueLiquidRecipeProduct(ItemStableReference: "preindustrial_food_catalogue_liquid_amphora", Quantity: 1, SkinStableReference: null, Liquid: "coffeehouse orgeat concentrate", Litres: 10.0) { Compatibility = new("1x a shared catalogue liquid amphora (@preindustrial_food_catalogue_liquid_amphora) filled with 10 litres of coffeehouse orgeat concentrate", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingFillCatalogueLiquidEarlymodernFoodLiquidPlantationMolassesSyrup() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/fill catalogue liquid earlymodern_food_liquid_plantation_molasses_syrup", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:preindustrial_food_catalogue_liquid_amphora"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "fill catalogue liquid earlymodern_food_liquid_plantation_molasses_syrup",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for earlymodern_food_liquid_plantation_molasses_syrup.",
		Action = "preparing syrup food",
		ActiveCraftItemSdesc = "a syrup preparation",
		Gate = CatalogueRecipeGates.PreIndustrialFoodCatalogueProductionCooking1521,
		Trait = null,
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
			new(15, "$0 crush|crushes $i1 and lay|lays out the syrup stock.", "$0 crush|crushes $i1 and lay|lays out the syrup stock.", ExertionLevel.Stasis, 0),
			new(25, "$0 work|works the syrup stock into a smooth liquid, following a household method, using $t1.", "$0 work|works the syrup stock into a smooth liquid, following a household method, using $t1.", ExertionLevel.Stasis, 0),
			new(20, "$0 inspect|inspects $p1 and set|sets the finished drink aside to settle.", "$0 inspect|inspects the poor drink and salvage|salvages the usable stock.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityTagRecipeInput(Grams: 1000.0, MaterialTag: "Food Crop", PileTag: "Seeded Yield", CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1 kilogram of a material tagged as Food Crop", ["piletag Seeded Yield"]), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Cooking", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Cooking tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueLiquidRecipeProduct(ItemStableReference: "preindustrial_food_catalogue_liquid_amphora", Quantity: 1, SkinStableReference: null, Liquid: "Atlantic plantation molasses syrup", Litres: 10.0) { Compatibility = new("1x a shared catalogue liquid amphora (@preindustrial_food_catalogue_liquid_amphora) filled with 10 litres of Atlantic plantation molasses syrup", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};
}
