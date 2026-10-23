#nullable enable

using MudSharp.Body;
using MudSharp.GameItems.Inventory.Plans;
using MudSharp.RPG.Checks;

namespace DatabaseSeeder.Seeders;

public static partial class Recipes05Stock
{
	public static CatalogueRecipeDefinition PreIndustrialFoodmakingMakeSharedPreparedSoupYoghurtCatalogueStock() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/make Shared Prepared soup yoghurt catalogue stock", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["medieval", "renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make Shared Prepared soup yoghurt catalogue stock",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for preindustrial_food_yoghurt_herb_soup.",
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
			new CatalogueProgCookedRecipeProduct(Prog: "ItemSeederPreIndustrialFood_Shared_Prepared_Soup_preindustrial_food_yoghurt_and_greens_broth", RemoveDrugsAndFoodEffects: false, Ingredients: []) { Compatibility = new("ItemSeederPreIndustrialFood_Shared_Prepared_Soup_preindustrial_food_yoghurt_and_greens_broth", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingMakeSharedPreparedStewBarleyCatalogueStock() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/make Shared Prepared stew barley catalogue stock", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["medieval", "renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make Shared Prepared stew barley catalogue stock",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for preindustrial_food_barley_herb_potage.",
		Action = "preparing stew food",
		ActiveCraftItemSdesc = "a stew preparation",
		Gate = CatalogueRecipeGates.PreIndustrialFoodCatalogueProductionMilling155,
		Trait = null,
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
			new CatalogueProgCookedRecipeProduct(Prog: "ItemSeederPreIndustrialFood_Shared_Prepared_Stew_preindustrial_food_barley_garden_stew", RemoveDrugsAndFoodEffects: false, Ingredients: []) { Compatibility = new("ItemSeederPreIndustrialFood_Shared_Prepared_Stew_preindustrial_food_barley_garden_stew", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingMakeSharedPreparedStewVegetableCatalogueStock() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/make Shared Prepared stew vegetable catalogue stock", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["medieval", "renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make Shared Prepared stew vegetable catalogue stock",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for preindustrial_food_cabbage_and_onion_stew.",
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
			new CatalogueProgCookedRecipeProduct(Prog: "ItemSeederPreIndustrialFood_Shared_Prepared_Stew_preindustrial_food_beetroot_and_cabbage_stew", RemoveDrugsAndFoodEffects: false, Ingredients: []) { Compatibility = new("ItemSeederPreIndustrialFood_Shared_Prepared_Stew_preindustrial_food_beetroot_and_cabbage_stew", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingMakeSharedPreparedStewCheeseCatalogueStock() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/make Shared Prepared stew cheese catalogue stock", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["medieval", "renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make Shared Prepared stew cheese catalogue stock",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for preindustrial_food_cheese_and_leek_potage.",
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
			new CatalogueCommodityTagRecipeInput(Grams: 1000.0, MaterialTag: "Raw Milk", PileTag: "Raw Milk", CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1 kilogram of a material tagged as Raw Milk", ["piletag Raw Milk"]), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Cooking", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Cooking tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueProgCookedRecipeProduct(Prog: "ItemSeederPreIndustrialFood_Shared_Prepared_Stew_preindustrial_food_cheese_and_barley_pot", RemoveDrugsAndFoodEffects: false, Ingredients: []) { Compatibility = new("ItemSeederPreIndustrialFood_Shared_Prepared_Stew_preindustrial_food_cheese_and_barley_pot", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingMakeSharedPreparedStewFishCatalogueStock() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/make Shared Prepared stew fish catalogue stock", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["medieval", "renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make Shared Prepared stew fish catalogue stock",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for preindustrial_food_fish_meatball_stew.",
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
			new CatalogueTagRecipeInput(Tag: "Raw Fish Cut", Quantity: 1) { Compatibility = new("1x an item with the Raw Fish Cut tag", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Cooking", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Cooking tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueProgCookedRecipeProduct(Prog: "ItemSeederPreIndustrialFood_Shared_Prepared_Stew_preindustrial_food_crustacean_root_stew", RemoveDrugsAndFoodEffects: false, Ingredients: []) { Compatibility = new("ItemSeederPreIndustrialFood_Shared_Prepared_Stew_preindustrial_food_crustacean_root_stew", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingMakeSharedPreparedStewChickpeaCatalogueStock() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/make Shared Prepared stew chickpea catalogue stock", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["medieval", "renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make Shared Prepared stew chickpea catalogue stock",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for preindustrial_food_rice_and_chickpea_pot.",
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
			new CatalogueProgCookedRecipeProduct(Prog: "ItemSeederPreIndustrialFood_Shared_Prepared_Stew_preindustrial_food_chickpea_and_garlic_stew", RemoveDrugsAndFoodEffects: false, Ingredients: []) { Compatibility = new("ItemSeederPreIndustrialFood_Shared_Prepared_Stew_preindustrial_food_chickpea_and_garlic_stew", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingMakeSharedPreparedStewLentilCatalogueStock() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/make Shared Prepared stew lentil catalogue stock", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["medieval", "renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make Shared Prepared stew lentil catalogue stock",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for preindustrial_food_rice_and_lentil_stew.",
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
			new CatalogueProgCookedRecipeProduct(Prog: "ItemSeederPreIndustrialFood_Shared_Prepared_Stew_preindustrial_food_beef_and_lentil_pot", RemoveDrugsAndFoodEffects: false, Ingredients: []) { Compatibility = new("ItemSeederPreIndustrialFood_Shared_Prepared_Stew_preindustrial_food_beef_and_lentil_pot", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingMakeSharedPreparedStewRiceCatalogueStock() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/make Shared Prepared stew rice catalogue stock", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["medieval", "renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make Shared Prepared stew rice catalogue stock",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for preindustrial_food_rice_garden_stew.",
		Action = "preparing stew food",
		ActiveCraftItemSdesc = "a stew preparation",
		Gate = CatalogueRecipeGates.PreIndustrialFoodCatalogueProductionMilling155,
		Trait = null,
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
			new CatalogueProgCookedRecipeProduct(Prog: "ItemSeederPreIndustrialFood_Shared_Prepared_Stew_preindustrial_food_milk_and_rice_pottage", RemoveDrugsAndFoodEffects: false, Ingredients: []) { Compatibility = new("ItemSeederPreIndustrialFood_Shared_Prepared_Stew_preindustrial_food_milk_and_rice_pottage", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingMakeSharedPreparedStewRyeCatalogueStock() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/make Shared Prepared stew rye catalogue stock", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["medieval", "renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make Shared Prepared stew rye catalogue stock",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for preindustrial_food_rye_berry_pottage.",
		Action = "preparing stew food",
		ActiveCraftItemSdesc = "a stew preparation",
		Gate = CatalogueRecipeGates.PreIndustrialFoodCatalogueProductionMilling155,
		Trait = null,
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
			new CatalogueProgCookedRecipeProduct(Prog: "ItemSeederPreIndustrialFood_Shared_Prepared_Stew_preindustrial_food_rye_and_turnip_stew", RemoveDrugsAndFoodEffects: false, Ingredients: []) { Compatibility = new("ItemSeederPreIndustrialFood_Shared_Prepared_Stew_preindustrial_food_rye_and_turnip_stew", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingMakeSharedPreparedStewBeanCatalogueStock() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/make Shared Prepared stew bean catalogue stock", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["medieval", "renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make Shared Prepared stew bean catalogue stock",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for preindustrial_food_sorghum_and_bean_stew.",
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
			new CatalogueProgCookedRecipeProduct(Prog: "ItemSeederPreIndustrialFood_Shared_Prepared_Stew_preindustrial_food_bean_and_cabbage_pot", RemoveDrugsAndFoodEffects: false, Ingredients: []) { Compatibility = new("ItemSeederPreIndustrialFood_Shared_Prepared_Stew_preindustrial_food_bean_and_cabbage_pot", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingMakeSharedPreparedStewMeatCatalogueStock() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/make Shared Prepared stew meat catalogue stock", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["medieval", "renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make Shared Prepared stew meat catalogue stock",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for preindustrial_food_sorghum_and_meat_pot.",
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
			new CatalogueProgCookedRecipeProduct(Prog: "ItemSeederPreIndustrialFood_Shared_Prepared_Stew_preindustrial_food_beef_and_barley_stew", RemoveDrugsAndFoodEffects: false, Ingredients: []) { Compatibility = new("ItemSeederPreIndustrialFood_Shared_Prepared_Stew_preindustrial_food_beef_and_barley_stew", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingMakeSharedPreparedStewSorghumCatalogueStock() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/make Shared Prepared stew sorghum catalogue stock", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["medieval", "renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make Shared Prepared stew sorghum catalogue stock",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for preindustrial_food_sorghum_root_potage.",
		Action = "preparing stew food",
		ActiveCraftItemSdesc = "a stew preparation",
		Gate = CatalogueRecipeGates.PreIndustrialFoodCatalogueProductionMilling155,
		Trait = null,
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
			new CatalogueProgCookedRecipeProduct(Prog: "ItemSeederPreIndustrialFood_Shared_Prepared_Stew_preindustrial_food_sorghum_root_potage", RemoveDrugsAndFoodEffects: false, Ingredients: []) { Compatibility = new("ItemSeederPreIndustrialFood_Shared_Prepared_Stew_preindustrial_food_sorghum_root_potage", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingMakeSharedPreparedStewYoghurtCatalogueStock() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/make Shared Prepared stew yoghurt catalogue stock", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["medieval", "renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make Shared Prepared stew yoghurt catalogue stock",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for preindustrial_food_soured_milk_stew.",
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
			new CatalogueCommodityTagRecipeInput(Grams: 1000.0, MaterialTag: "Raw Milk", PileTag: "Raw Milk", CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1 kilogram of a material tagged as Raw Milk", ["piletag Raw Milk"]), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Cooking", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Cooking tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueProgCookedRecipeProduct(Prog: "ItemSeederPreIndustrialFood_Shared_Prepared_Stew_preindustrial_food_soured_milk_stew", RemoveDrugsAndFoodEffects: false, Ingredients: []) { Compatibility = new("ItemSeederPreIndustrialFood_Shared_Prepared_Stew_preindustrial_food_soured_milk_stew", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingMakeSharedPreparedStewPeaCatalogueStock() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/make Shared Prepared stew pea catalogue stock", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["medieval", "renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make Shared Prepared stew pea catalogue stock",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for preindustrial_food_split_pea_pottage.",
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
			new CatalogueProgCookedRecipeProduct(Prog: "ItemSeederPreIndustrialFood_Shared_Prepared_Stew_preindustrial_food_carrot_and_pea_pot", RemoveDrugsAndFoodEffects: false, Ingredients: []) { Compatibility = new("ItemSeederPreIndustrialFood_Shared_Prepared_Stew_preindustrial_food_carrot_and_pea_pot", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingMakeSharedPreparedStewWheatCatalogueStock() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/make Shared Prepared stew wheat catalogue stock", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["medieval", "renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make Shared Prepared stew wheat catalogue stock",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for preindustrial_food_wheat_and_pulse_stew.",
		Action = "preparing stew food",
		ActiveCraftItemSdesc = "a stew preparation",
		Gate = CatalogueRecipeGates.PreIndustrialFoodCatalogueProductionMilling155,
		Trait = null,
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
			new CatalogueProgCookedRecipeProduct(Prog: "ItemSeederPreIndustrialFood_Shared_Prepared_Stew_preindustrial_food_toasted_wheat_stew", RemoveDrugsAndFoodEffects: false, Ingredients: []) { Compatibility = new("ItemSeederPreIndustrialFood_Shared_Prepared_Stew_preindustrial_food_toasted_wheat_stew", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingMakeSharedPreparedMeatBreadCatalogueStock() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/make Shared Prepared meat bread catalogue stock", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["medieval", "renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make Shared Prepared meat bread catalogue stock",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for preindustrial_food_bread_meat_sops.",
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
			new CatalogueProgCookedRecipeProduct(Prog: "ItemSeederPreIndustrialFood_Shared_Prepared_Meat_preindustrial_food_bread_meat_sops", RemoveDrugsAndFoodEffects: false, Ingredients: []) { Compatibility = new("ItemSeederPreIndustrialFood_Shared_Prepared_Meat_preindustrial_food_bread_meat_sops", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingMakeSharedPreparedMeatMeatCatalogueStock() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/make Shared Prepared meat meat catalogue stock", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["medieval", "renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make Shared Prepared meat meat catalogue stock",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for preindustrial_food_plain_boiled_beef.",
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
			new CatalogueProgCookedRecipeProduct(Prog: "ItemSeederPreIndustrialFood_Shared_Prepared_Meat_preindustrial_food_ale_braised_beef", RemoveDrugsAndFoodEffects: false, Ingredients: []) { Compatibility = new("ItemSeederPreIndustrialFood_Shared_Prepared_Meat_preindustrial_food_ale_braised_beef", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingMakeSharedPreparedPoultryMeatCatalogueStock() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/make Shared Prepared poultry meat catalogue stock", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["medieval", "renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make Shared Prepared poultry meat catalogue stock",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for preindustrial_food_plain_boiled_hen.",
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
			new CatalogueProgCookedRecipeProduct(Prog: "ItemSeederPreIndustrialFood_Shared_Prepared_Poultry_preindustrial_food_boiled_game_bird", RemoveDrugsAndFoodEffects: false, Ingredients: []) { Compatibility = new("ItemSeederPreIndustrialFood_Shared_Prepared_Poultry_preindustrial_food_boiled_game_bird", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingMakeSharedPreparedOffalBreadCatalogueStock() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/make Shared Prepared offal bread catalogue stock", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["medieval", "renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make Shared Prepared offal bread catalogue stock",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for preindustrial_food_marrow_on_bread.",
		Action = "preparing offal food",
		ActiveCraftItemSdesc = "a offal preparation",
		Gate = CatalogueRecipeGates.PreIndustrialFoodCatalogueProductionButchering155,
		Trait = null,
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
			new CatalogueTagRecipeInput(Tag: "Offal", Quantity: 1) { Compatibility = new("1x an item with the Offal tag", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Cooking", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Cooking tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueProgCookedRecipeProduct(Prog: "ItemSeederPreIndustrialFood_Shared_Prepared_Offal_preindustrial_food_marrow_on_bread", RemoveDrugsAndFoodEffects: false, Ingredients: []) { Compatibility = new("ItemSeederPreIndustrialFood_Shared_Prepared_Offal_preindustrial_food_marrow_on_bread", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingMakeSharedPreparedOffalMeatCatalogueStock() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/make Shared Prepared offal meat catalogue stock", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["medieval", "renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make Shared Prepared offal meat catalogue stock",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for preindustrial_food_roast_heart.",
		Action = "preparing offal food",
		ActiveCraftItemSdesc = "a offal preparation",
		Gate = CatalogueRecipeGates.PreIndustrialFoodCatalogueProductionButchering155,
		Trait = null,
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
			new CatalogueTagRecipeInput(Tag: "Offal", Quantity: 1) { Compatibility = new("1x an item with the Offal tag", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Cooking", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Cooking tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueProgCookedRecipeProduct(Prog: "ItemSeederPreIndustrialFood_Shared_Prepared_Offal_preindustrial_food_blood_and_grain_pot", RemoveDrugsAndFoodEffects: false, Ingredients: []) { Compatibility = new("ItemSeederPreIndustrialFood_Shared_Prepared_Offal_preindustrial_food_blood_and_grain_pot", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingMakeSharedPreparedOffalEggCatalogueStock() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/make Shared Prepared offal egg catalogue stock", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["medieval", "renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make Shared Prepared offal egg catalogue stock",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for preindustrial_food_scrambled_brains.",
		Action = "preparing offal food",
		ActiveCraftItemSdesc = "a offal preparation",
		Gate = CatalogueRecipeGates.PreIndustrialFoodCatalogueProductionButchering155,
		Trait = null,
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
			new CatalogueTagRecipeInput(Tag: "Offal", Quantity: 1) { Compatibility = new("1x an item with the Offal tag", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Cooking", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Cooking tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueProgCookedRecipeProduct(Prog: "ItemSeederPreIndustrialFood_Shared_Prepared_Offal_preindustrial_food_scrambled_brains", RemoveDrugsAndFoodEffects: false, Ingredients: []) { Compatibility = new("ItemSeederPreIndustrialFood_Shared_Prepared_Offal_preindustrial_food_scrambled_brains", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingMakeSharedPreparedFishBreadCatalogueStock() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/make Shared Prepared fish bread catalogue stock", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["medieval", "renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make Shared Prepared fish bread catalogue stock",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for preindustrial_food_fresh_roe_on_toast.",
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
			new CatalogueProgCookedRecipeProduct(Prog: "ItemSeederPreIndustrialFood_Shared_Prepared_Fish_preindustrial_food_fresh_roe_on_toast", RemoveDrugsAndFoodEffects: false, Ingredients: []) { Compatibility = new("ItemSeederPreIndustrialFood_Shared_Prepared_Fish_preindustrial_food_fresh_roe_on_toast", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingMakeSharedPreparedFishEggCatalogueStock() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/make Shared Prepared fish egg catalogue stock", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["medieval", "renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make Shared Prepared fish egg catalogue stock",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for preindustrial_food_scrambled_egg_with_fish.",
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
			new CatalogueProgCookedRecipeProduct(Prog: "ItemSeederPreIndustrialFood_Shared_Prepared_Fish_preindustrial_food_fish_stuffed_egg_cakes", RemoveDrugsAndFoodEffects: false, Ingredients: []) { Compatibility = new("ItemSeederPreIndustrialFood_Shared_Prepared_Fish_preindustrial_food_fish_stuffed_egg_cakes", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingMakeSharedPreparedFishFishCatalogueStock() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/make Shared Prepared fish fish catalogue stock", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["medieval", "renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make Shared Prepared fish fish catalogue stock",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for preindustrial_food_whole_ember_roasted_fish.",
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
			new CatalogueProgCookedRecipeProduct(Prog: "ItemSeederPreIndustrialFood_Shared_Prepared_Fish_preindustrial_food_barley_crusted_fried_fish", RemoveDrugsAndFoodEffects: false, Ingredients: []) { Compatibility = new("ItemSeederPreIndustrialFood_Shared_Prepared_Fish_preindustrial_food_barley_crusted_fried_fish", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingMakeSharedPreparedShellfishFishCatalogueStock() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/make Shared Prepared shellfish fish catalogue stock", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["medieval", "renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make Shared Prepared shellfish fish catalogue stock",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for preindustrial_food_boiled_shellfish_platter.",
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
			new CatalogueProgCookedRecipeProduct(Prog: "ItemSeederPreIndustrialFood_Shared_Prepared_Shellfish_preindustrial_food_baked_stuffed_clams", RemoveDrugsAndFoodEffects: false, Ingredients: []) { Compatibility = new("ItemSeederPreIndustrialFood_Shared_Prepared_Shellfish_preindustrial_food_baked_stuffed_clams", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingMakeSharedPreparedShellfishBarleyCatalogueStock() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/make Shared Prepared shellfish barley catalogue stock", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["medieval", "renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make Shared Prepared shellfish barley catalogue stock",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for preindustrial_food_shellfish_barley_pot.",
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
			new CatalogueProgCookedRecipeProduct(Prog: "ItemSeederPreIndustrialFood_Shared_Prepared_Shellfish_preindustrial_food_crustacean_barley_bake", RemoveDrugsAndFoodEffects: false, Ingredients: []) { Compatibility = new("ItemSeederPreIndustrialFood_Shared_Prepared_Shellfish_preindustrial_food_crustacean_barley_bake", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingMakeSharedPreparedShellfishEggCatalogueStock() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/make Shared Prepared shellfish egg catalogue stock", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["medieval", "renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make Shared Prepared shellfish egg catalogue stock",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for preindustrial_food_shellfish_egg_custard.",
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
			new CatalogueProgCookedRecipeProduct(Prog: "ItemSeederPreIndustrialFood_Shared_Prepared_Shellfish_preindustrial_food_shellfish_egg_custard", RemoveDrugsAndFoodEffects: false, Ingredients: []) { Compatibility = new("ItemSeederPreIndustrialFood_Shared_Prepared_Shellfish_preindustrial_food_shellfish_egg_custard", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingMakeSharedPreparedShellfishRiceCatalogueStock() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/make Shared Prepared shellfish rice catalogue stock", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["medieval", "renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make Shared Prepared shellfish rice catalogue stock",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for preindustrial_food_shellfish_rice_bowl.",
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
			new CatalogueProgCookedRecipeProduct(Prog: "ItemSeederPreIndustrialFood_Shared_Prepared_Shellfish_preindustrial_food_crustacean_rice_pilaf", RemoveDrugsAndFoodEffects: false, Ingredients: []) { Compatibility = new("ItemSeederPreIndustrialFood_Shared_Prepared_Shellfish_preindustrial_food_crustacean_rice_pilaf", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingMakeSharedPreparedDairyBarleyCatalogueStock() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/make Shared Prepared dairy barley catalogue stock", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["medieval", "renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make Shared Prepared dairy barley catalogue stock",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for preindustrial_food_buttered_grain.",
		Action = "preparing dairy food",
		ActiveCraftItemSdesc = "a dairy preparation",
		Gate = CatalogueRecipeGates.PreIndustrialFoodCatalogueProductionMilling156,
		Trait = null,
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
			new CatalogueProgCookedRecipeProduct(Prog: "ItemSeederPreIndustrialFood_Shared_Prepared_Dairy_preindustrial_food_buttered_grain", RemoveDrugsAndFoodEffects: false, Ingredients: []) { Compatibility = new("ItemSeederPreIndustrialFood_Shared_Prepared_Dairy_preindustrial_food_buttered_grain", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingMakeSharedPreparedDairyVegetableCatalogueStock() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/make Shared Prepared dairy vegetable catalogue stock", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["medieval", "renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make Shared Prepared dairy vegetable catalogue stock",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for preindustrial_food_buttered_roots.",
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
			new CatalogueProgCookedRecipeProduct(Prog: "ItemSeederPreIndustrialFood_Shared_Prepared_Dairy_preindustrial_food_buttered_roots", RemoveDrugsAndFoodEffects: false, Ingredients: []) { Compatibility = new("ItemSeederPreIndustrialFood_Shared_Prepared_Dairy_preindustrial_food_buttered_roots", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingMakeSharedPreparedDairyCheeseCatalogueStock() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/make Shared Prepared dairy cheese catalogue stock", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["medieval", "renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make Shared Prepared dairy cheese catalogue stock",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for preindustrial_food_fresh_milk_curds.",
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
			new CatalogueProgCookedRecipeProduct(Prog: "ItemSeederPreIndustrialFood_Shared_Prepared_Dairy_preindustrial_food_aged_hard_cheese", RemoveDrugsAndFoodEffects: false, Ingredients: []) { Compatibility = new("ItemSeederPreIndustrialFood_Shared_Prepared_Dairy_preindustrial_food_aged_hard_cheese", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingMakeSharedPreparedDairyFoodCatalogueStock() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/make Shared Prepared dairy food catalogue stock", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["medieval", "renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make Shared Prepared dairy food catalogue stock",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for preindustrial_food_plain_clotted_cream.",
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
			new CatalogueProgCookedRecipeProduct(Prog: "ItemSeederPreIndustrialFood_Shared_Prepared_Dairy_preindustrial_food_browned_butter_spread", RemoveDrugsAndFoodEffects: false, Ingredients: []) { Compatibility = new("ItemSeederPreIndustrialFood_Shared_Prepared_Dairy_preindustrial_food_browned_butter_spread", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingMakeSharedPreparedDairyYoghurtCatalogueStock() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/make Shared Prepared dairy yoghurt catalogue stock", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["medieval", "renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make Shared Prepared dairy yoghurt catalogue stock",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for preindustrial_food_plain_strained_yoghurt.",
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
			new CatalogueProgCookedRecipeProduct(Prog: "ItemSeederPreIndustrialFood_Shared_Prepared_Dairy_preindustrial_food_baked_yoghurt_dish", RemoveDrugsAndFoodEffects: false, Ingredients: []) { Compatibility = new("ItemSeederPreIndustrialFood_Shared_Prepared_Dairy_preindustrial_food_baked_yoghurt_dish", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingMakeSharedPreparedDairyBreadCatalogueStock() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/make Shared Prepared dairy bread catalogue stock", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["medieval", "renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make Shared Prepared dairy bread catalogue stock",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for preindustrial_food_toasted_cheese_bread.",
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
			new CatalogueProgCookedRecipeProduct(Prog: "ItemSeederPreIndustrialFood_Shared_Prepared_Dairy_preindustrial_food_buttered_bread", RemoveDrugsAndFoodEffects: false, Ingredients: []) { Compatibility = new("ItemSeederPreIndustrialFood_Shared_Prepared_Dairy_preindustrial_food_buttered_bread", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingMakeSharedPreparedEggBreadCatalogueStock() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/make Shared Prepared egg bread catalogue stock", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["medieval", "renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make Shared Prepared egg bread catalogue stock",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for preindustrial_food_bread_egg_pudding.",
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
			new CatalogueProgCookedRecipeProduct(Prog: "ItemSeederPreIndustrialFood_Shared_Prepared_Egg_preindustrial_food_bread_egg_pudding", RemoveDrugsAndFoodEffects: false, Ingredients: []) { Compatibility = new("ItemSeederPreIndustrialFood_Shared_Prepared_Egg_preindustrial_food_bread_egg_pudding", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingMakeSharedPreparedEggEggCatalogueStock() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/make Shared Prepared egg egg catalogue stock", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["medieval", "renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make Shared Prepared egg egg catalogue stock",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for preindustrial_food_plain_boiled_egg.",
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
			new CatalogueProgCookedRecipeProduct(Prog: "ItemSeederPreIndustrialFood_Shared_Prepared_Egg_preindustrial_food_baked_egg_dish", RemoveDrugsAndFoodEffects: false, Ingredients: []) { Compatibility = new("ItemSeederPreIndustrialFood_Shared_Prepared_Egg_preindustrial_food_baked_egg_dish", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingMakeSharedPreparedPreservedBreadCatalogueStock() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/make Shared Prepared preserved bread catalogue stock", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["medieval", "renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make Shared Prepared preserved bread catalogue stock",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for preindustrial_food_cured_meat_sops.",
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
			new CatalogueProgCookedRecipeProduct(Prog: "ItemSeederPreIndustrialFood_Shared_Prepared_Preserved_preindustrial_food_cured_meat_sops", RemoveDrugsAndFoodEffects: false, Ingredients: []) { Compatibility = new("ItemSeederPreIndustrialFood_Shared_Prepared_Preserved_preindustrial_food_cured_meat_sops", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingMakeSharedPreparedPreservedCheeseCatalogueStock() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/make Shared Prepared preserved cheese catalogue stock", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["medieval", "renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make Shared Prepared preserved cheese catalogue stock",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for preindustrial_food_dried_cheese_cakes.",
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
			new CatalogueProgCookedRecipeProduct(Prog: "ItemSeederPreIndustrialFood_Shared_Prepared_Preserved_preindustrial_food_cheese_travel_ration", RemoveDrugsAndFoodEffects: false, Ingredients: []) { Compatibility = new("ItemSeederPreIndustrialFood_Shared_Prepared_Preserved_preindustrial_food_cheese_travel_ration", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingMakeSharedPreparedPreservedRiceCatalogueStock() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/make Shared Prepared preserved rice catalogue stock", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["medieval", "renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make Shared Prepared preserved rice catalogue stock",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for preindustrial_food_fermented_fish_and_rice.",
		Action = "preparing preserved food",
		ActiveCraftItemSdesc = "a preserved preparation",
		Gate = CatalogueRecipeGates.PreIndustrialFoodCatalogueProductionMilling157,
		Trait = null,
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
			new CatalogueProgCookedRecipeProduct(Prog: "ItemSeederPreIndustrialFood_Shared_Prepared_Preserved_preindustrial_food_fermented_fish_and_rice", RemoveDrugsAndFoodEffects: false, Ingredients: []) { Compatibility = new("ItemSeederPreIndustrialFood_Shared_Prepared_Preserved_preindustrial_food_fermented_fish_and_rice", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingMakeSharedPreparedPreservedHoneyCatalogueStock() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/make Shared Prepared preserved honey catalogue stock", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["medieval", "renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make Shared Prepared preserved honey catalogue stock",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for preindustrial_food_honey_preserved_fruit.",
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
			new CatalogueCommodityTagRecipeInput(Grams: 1000.0, MaterialTag: "Food", PileTag: "Pressed Honey", CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1 kilogram of a material tagged as Food", ["piletag Pressed Honey"]), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Cooking", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Cooking tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueProgCookedRecipeProduct(Prog: "ItemSeederPreIndustrialFood_Shared_Prepared_Preserved_preindustrial_food_honey_preserved_fruit", RemoveDrugsAndFoodEffects: false, Ingredients: []) { Compatibility = new("ItemSeederPreIndustrialFood_Shared_Prepared_Preserved_preindustrial_food_honey_preserved_fruit", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingMakeSharedPreparedPreservedYoghurtCatalogueStock() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/make Shared Prepared preserved yoghurt catalogue stock", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["medieval", "renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make Shared Prepared preserved yoghurt catalogue stock",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for preindustrial_food_oil_preserved_yoghurt_balls.",
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
			new CatalogueProgCookedRecipeProduct(Prog: "ItemSeederPreIndustrialFood_Shared_Prepared_Preserved_preindustrial_food_dried_yoghurt_cakes", RemoveDrugsAndFoodEffects: false, Ingredients: []) { Compatibility = new("ItemSeederPreIndustrialFood_Shared_Prepared_Preserved_preindustrial_food_dried_yoghurt_cakes", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingMakeSharedPreparedPreservedEggCatalogueStock() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/make Shared Prepared preserved egg catalogue stock", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["medieval", "renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make Shared Prepared preserved egg catalogue stock",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for preindustrial_food_pickled_eggs.",
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
			new CatalogueCommodityTagRecipeInput(Grams: 1000.0, MaterialTag: "Egg Product", PileTag: "Egg Product", CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1 kilogram of a material tagged as Egg Product", ["piletag Egg Product"]), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Cooking", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Cooking tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueProgCookedRecipeProduct(Prog: "ItemSeederPreIndustrialFood_Shared_Prepared_Preserved_preindustrial_food_ash_preserved_eggs", RemoveDrugsAndFoodEffects: false, Ingredients: []) { Compatibility = new("ItemSeederPreIndustrialFood_Shared_Prepared_Preserved_preindustrial_food_ash_preserved_eggs", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingMakeSharedPreparedPreservedFishCatalogueStock() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/make Shared Prepared preserved fish catalogue stock", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["medieval", "renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make Shared Prepared preserved fish catalogue stock",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for preindustrial_food_preserved_fish_seasoning_cake.",
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
			new CatalogueProgCookedRecipeProduct(Prog: "ItemSeederPreIndustrialFood_Shared_Prepared_Preserved_preindustrial_food_cold_smoked_fish_slice", RemoveDrugsAndFoodEffects: false, Ingredients: []) { Compatibility = new("ItemSeederPreIndustrialFood_Shared_Prepared_Preserved_preindustrial_food_cold_smoked_fish_slice", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingMakeSharedPreparedPreservedVegetableCatalogueStock() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/make Shared Prepared preserved vegetable catalogue stock", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["medieval", "renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make Shared Prepared preserved vegetable catalogue stock",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for preindustrial_food_preserved_herb_cake.",
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
			new CatalogueProgCookedRecipeProduct(Prog: "ItemSeederPreIndustrialFood_Shared_Prepared_Preserved_preindustrial_food_preserved_herb_cake", RemoveDrugsAndFoodEffects: false, Ingredients: []) { Compatibility = new("ItemSeederPreIndustrialFood_Shared_Prepared_Preserved_preindustrial_food_preserved_herb_cake", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingMakeSharedPreparedPreservedMeatCatalogueStock() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/make Shared Prepared preserved meat catalogue stock", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["medieval", "renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make Shared Prepared preserved meat catalogue stock",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for preindustrial_food_preserved_meat_seasoning_cake.",
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
			new CatalogueProgCookedRecipeProduct(Prog: "ItemSeederPreIndustrialFood_Shared_Prepared_Preserved_preindustrial_food_berry_meat_travel_cake", RemoveDrugsAndFoodEffects: false, Ingredients: []) { Compatibility = new("ItemSeederPreIndustrialFood_Shared_Prepared_Preserved_preindustrial_food_berry_meat_travel_cake", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingMakeSharedPreparedPreservedFoodCatalogueStock() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/make Shared Prepared preserved food catalogue stock", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["medieval", "renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make Shared Prepared preserved food catalogue stock",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for preindustrial_food_preserved_spice_cake.",
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
			new CatalogueProgCookedRecipeProduct(Prog: "ItemSeederPreIndustrialFood_Shared_Prepared_Preserved_preindustrial_food_clarified_butter_cake", RemoveDrugsAndFoodEffects: false, Ingredients: []) { Compatibility = new("ItemSeederPreIndustrialFood_Shared_Prepared_Preserved_preindustrial_food_clarified_butter_cake", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingMakeSharedPreparedPreservedBarleyCatalogueStock() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/make Shared Prepared preserved barley catalogue stock", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["medieval", "renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make Shared Prepared preserved barley catalogue stock",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for preindustrial_food_smoked_fish_and_barley_cake.",
		Action = "preparing preserved food",
		ActiveCraftItemSdesc = "a preserved preparation",
		Gate = CatalogueRecipeGates.PreIndustrialFoodCatalogueProductionMilling157,
		Trait = null,
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
			new CatalogueProgCookedRecipeProduct(Prog: "ItemSeederPreIndustrialFood_Shared_Prepared_Preserved_preindustrial_food_smoked_fish_and_barley_cake", RemoveDrugsAndFoodEffects: false, Ingredients: []) { Compatibility = new("ItemSeederPreIndustrialFood_Shared_Prepared_Preserved_preindustrial_food_smoked_fish_and_barley_cake", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingMakeSharedPreparedPreservedFruitCatalogueStock() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/make Shared Prepared preserved fruit catalogue stock", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["medieval", "renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make Shared Prepared preserved fruit catalogue stock",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for preindustrial_food_sour_fruit_seasoning_cake.",
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
			new CatalogueCommodityTagRecipeInput(Grams: 1000.0, MaterialTag: "Fruit", PileTag: "Seeded Yield", CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1 kilogram of a material tagged as Fruit", ["piletag Seeded Yield"]), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Cooking", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Cooking tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueProgCookedRecipeProduct(Prog: "ItemSeederPreIndustrialFood_Shared_Prepared_Preserved_preindustrial_food_dried_apple_rings", RemoveDrugsAndFoodEffects: false, Ingredients: []) { Compatibility = new("ItemSeederPreIndustrialFood_Shared_Prepared_Preserved_preindustrial_food_dried_apple_rings", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingMakeSharedPreparedFruitWheatCatalogueStock() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/make Shared Prepared fruit wheat catalogue stock", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["medieval", "renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make Shared Prepared fruit wheat catalogue stock",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for preindustrial_food_baked_berry_pudding.",
		Action = "preparing fruit food",
		ActiveCraftItemSdesc = "a fruit preparation",
		Gate = CatalogueRecipeGates.PreIndustrialFoodCatalogueProductionMilling158,
		Trait = null,
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
			new CatalogueProgCookedRecipeProduct(Prog: "ItemSeederPreIndustrialFood_Shared_Prepared_Fruit_preindustrial_food_baked_berry_pudding", RemoveDrugsAndFoodEffects: false, Ingredients: []) { Compatibility = new("ItemSeederPreIndustrialFood_Shared_Prepared_Fruit_preindustrial_food_baked_berry_pudding", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingMakeSharedPreparedFruitBarleyCatalogueStock() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/make Shared Prepared fruit barley catalogue stock", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["medieval", "renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make Shared Prepared fruit barley catalogue stock",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for preindustrial_food_date_and_barley_ration.",
		Action = "preparing fruit food",
		ActiveCraftItemSdesc = "a fruit preparation",
		Gate = CatalogueRecipeGates.PreIndustrialFoodCatalogueProductionMilling158,
		Trait = null,
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
			new CatalogueProgCookedRecipeProduct(Prog: "ItemSeederPreIndustrialFood_Shared_Prepared_Fruit_preindustrial_food_date_and_barley_ration", RemoveDrugsAndFoodEffects: false, Ingredients: []) { Compatibility = new("ItemSeederPreIndustrialFood_Shared_Prepared_Fruit_preindustrial_food_date_and_barley_ration", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingMakeSharedPreparedFruitRiceCatalogueStock() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/make Shared Prepared fruit rice catalogue stock", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["medieval", "renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make Shared Prepared fruit rice catalogue stock",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for preindustrial_food_date_rice_pudding.",
		Action = "preparing fruit food",
		ActiveCraftItemSdesc = "a fruit preparation",
		Gate = CatalogueRecipeGates.PreIndustrialFoodCatalogueProductionMilling158,
		Trait = null,
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
			new CatalogueProgCookedRecipeProduct(Prog: "ItemSeederPreIndustrialFood_Shared_Prepared_Fruit_preindustrial_food_date_rice_pudding", RemoveDrugsAndFoodEffects: false, Ingredients: []) { Compatibility = new("ItemSeederPreIndustrialFood_Shared_Prepared_Fruit_preindustrial_food_date_rice_pudding", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingMakeSharedPreparedFruitFoodCatalogueStock() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/make Shared Prepared fruit food catalogue stock", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["medieval", "renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make Shared Prepared fruit food catalogue stock",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for preindustrial_food_dried_quince_wedges.",
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
			new CatalogueProgCookedRecipeProduct(Prog: "ItemSeederPreIndustrialFood_Shared_Prepared_Fruit_preindustrial_food_berry_pulp_travel_sheet", RemoveDrugsAndFoodEffects: false, Ingredients: []) { Compatibility = new("ItemSeederPreIndustrialFood_Shared_Prepared_Fruit_preindustrial_food_berry_pulp_travel_sheet", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingMakeSharedPreparedFruitCheeseCatalogueStock() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/make Shared Prepared fruit cheese catalogue stock", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["medieval", "renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make Shared Prepared fruit cheese catalogue stock",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for preindustrial_food_fruit_and_fresh_cheese.",
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
			new CatalogueProgCookedRecipeProduct(Prog: "ItemSeederPreIndustrialFood_Shared_Prepared_Fruit_preindustrial_food_fruit_and_fresh_cheese", RemoveDrugsAndFoodEffects: false, Ingredients: []) { Compatibility = new("ItemSeederPreIndustrialFood_Shared_Prepared_Fruit_preindustrial_food_fruit_and_fresh_cheese", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingMakeSharedPreparedFruitBreadCatalogueStock() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/make Shared Prepared fruit bread catalogue stock", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["medieval", "renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make Shared Prepared fruit bread catalogue stock",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for preindustrial_food_fruit_bread_pudding.",
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
			new CatalogueProgCookedRecipeProduct(Prog: "ItemSeederPreIndustrialFood_Shared_Prepared_Fruit_preindustrial_food_fruit_bread_pudding", RemoveDrugsAndFoodEffects: false, Ingredients: []) { Compatibility = new("ItemSeederPreIndustrialFood_Shared_Prepared_Fruit_preindustrial_food_fruit_bread_pudding", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingMakeSharedPreparedFruitHoneyCatalogueStock() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/make Shared Prepared fruit honey catalogue stock", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["medieval", "renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make Shared Prepared fruit honey catalogue stock",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for preindustrial_food_honey_glazed_dried_fruit.",
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
			new CatalogueCommodityTagRecipeInput(Grams: 1000.0, MaterialTag: "Food", PileTag: "Pressed Honey", CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1 kilogram of a material tagged as Food", ["piletag Pressed Honey"]), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Cooking", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Cooking tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueProgCookedRecipeProduct(Prog: "ItemSeederPreIndustrialFood_Shared_Prepared_Fruit_preindustrial_food_berry_honey_ration_bar", RemoveDrugsAndFoodEffects: false, Ingredients: []) { Compatibility = new("ItemSeederPreIndustrialFood_Shared_Prepared_Fruit_preindustrial_food_berry_honey_ration_bar", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingMakeSharedPreparedFruitFruitCatalogueStock() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/make Shared Prepared fruit fruit catalogue stock", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["medieval", "renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make Shared Prepared fruit fruit catalogue stock",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for preindustrial_food_sliced_fresh_apples.",
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
			new CatalogueProgCookedRecipeProduct(Prog: "ItemSeederPreIndustrialFood_Shared_Prepared_Fruit_preindustrial_food_baked_quince", RemoveDrugsAndFoodEffects: false, Ingredients: []) { Compatibility = new("ItemSeederPreIndustrialFood_Shared_Prepared_Fruit_preindustrial_food_baked_quince", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingMakeSharedPreparedNutBreadCatalogueStock() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/make Shared Prepared nut bread catalogue stock", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["medieval", "renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make Shared Prepared nut bread catalogue stock",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for preindustrial_food_nut_bread_pudding.",
		Action = "preparing nut food",
		ActiveCraftItemSdesc = "a nut preparation",
		Gate = CatalogueRecipeGates.PreIndustrialFoodCatalogueProductionCooking158,
		Trait = null,
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
			new(25, "$0 mix|mixes the nut stock into an even preparation, using $t1.", "$0 mix|mixes the nut stock into an even preparation, using $t1.", ExertionLevel.Stasis, 0),
			new(20, "$0 shape|shapes $p1 and set|sets it aside to cool and firm.", "$0 break|breaks the finished sweet and save|saves the crumbs for porridge.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityTagRecipeInput(Grams: 1000.0, MaterialTag: "Food Crop", PileTag: "Seeded Yield", CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1 kilogram of a material tagged as Food Crop", ["piletag Seeded Yield"]), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Cooking", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Cooking tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueProgCookedRecipeProduct(Prog: "ItemSeederPreIndustrialFood_Shared_Prepared_Nut_preindustrial_food_nut_bread_pudding", RemoveDrugsAndFoodEffects: false, Ingredients: []) { Compatibility = new("ItemSeederPreIndustrialFood_Shared_Prepared_Nut_preindustrial_food_nut_bread_pudding", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingMakeSharedPreparedNutWheatCatalogueStock() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/make Shared Prepared nut wheat catalogue stock", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["medieval", "renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make Shared Prepared nut wheat catalogue stock",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for preindustrial_food_nut_filled_pastry.",
		Action = "preparing nut food",
		ActiveCraftItemSdesc = "a nut preparation",
		Gate = CatalogueRecipeGates.PreIndustrialFoodCatalogueProductionMilling159,
		Trait = null,
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
			new(25, "$0 mix|mixes the nut stock into an even preparation, using $t1.", "$0 mix|mixes the nut stock into an even preparation, using $t1.", ExertionLevel.Stasis, 0),
			new(20, "$0 shape|shapes $p1 and set|sets it aside to cool and firm.", "$0 break|breaks the finished sweet and save|saves the crumbs for porridge.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityTagRecipeInput(Grams: 1000.0, MaterialTag: "Food Crop", PileTag: "Seeded Yield", CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1 kilogram of a material tagged as Food Crop", ["piletag Seeded Yield"]), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Cooking", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Cooking tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueProgCookedRecipeProduct(Prog: "ItemSeederPreIndustrialFood_Shared_Prepared_Nut_preindustrial_food_fruit_nut_pastry", RemoveDrugsAndFoodEffects: false, Ingredients: []) { Compatibility = new("ItemSeederPreIndustrialFood_Shared_Prepared_Nut_preindustrial_food_fruit_nut_pastry", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingMakeSharedPreparedNutTreeNutCatalogueStock() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/make Shared Prepared nut tree nut catalogue stock", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["medieval", "renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make Shared Prepared nut tree nut catalogue stock",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for preindustrial_food_plain_roasted_nuts.",
		Action = "preparing nut food",
		ActiveCraftItemSdesc = "a nut preparation",
		Gate = CatalogueRecipeGates.PreIndustrialFoodCatalogueProductionCooking158,
		Trait = null,
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
			new(25, "$0 mix|mixes the nut stock into an even preparation, using $t1.", "$0 mix|mixes the nut stock into an even preparation, using $t1.", ExertionLevel.Stasis, 0),
			new(20, "$0 shape|shapes $p1 and set|sets it aside to cool and firm.", "$0 break|breaks the finished sweet and save|saves the crumbs for porridge.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityTagRecipeInput(Grams: 1000.0, MaterialTag: "Food Crop", PileTag: "Seeded Yield", CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1 kilogram of a material tagged as Food Crop", ["piletag Seeded Yield"]), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Cooking", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Cooking tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueProgCookedRecipeProduct(Prog: "ItemSeederPreIndustrialFood_Shared_Prepared_Nut_preindustrial_food_boiled_chestnut_bowl", RemoveDrugsAndFoodEffects: false, Ingredients: []) { Compatibility = new("ItemSeederPreIndustrialFood_Shared_Prepared_Nut_preindustrial_food_boiled_chestnut_bowl", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingMakeSharedPreparedSweetOatCatalogueStock() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/make Shared Prepared sweet oat catalogue stock", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["medieval", "renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make Shared Prepared sweet oat catalogue stock",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for preindustrial_food_apple_oat_cake.",
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
			new CatalogueProgCookedRecipeProduct(Prog: "ItemSeederPreIndustrialFood_Shared_Prepared_Sweet_preindustrial_food_apple_oat_cake", RemoveDrugsAndFoodEffects: false, Ingredients: []) { Compatibility = new("ItemSeederPreIndustrialFood_Shared_Prepared_Sweet_preindustrial_food_apple_oat_cake", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};
}
