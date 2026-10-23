#nullable enable

using MudSharp.Body;
using MudSharp.GameItems.Inventory.Plans;
using MudSharp.RPG.Checks;

namespace DatabaseSeeder.Seeders;

public static partial class Recipes06Stock
{
	public static CatalogueRecipeDefinition PreIndustrialFoodmakingMakeRenaissancePreparedFruitFoodCatalogueStock() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/make Renaissance Prepared fruit food catalogue stock", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make Renaissance Prepared fruit food catalogue stock",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for renaissance_food_safavid_fruit_leather_rollups.",
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
			new CatalogueProgCookedRecipeProduct(Prog: "ItemSeederPreIndustrialFood_Renaissance_Prepared_Fruit_renaissance_food_andean_fruit_quinoa_like_bar", RemoveDrugsAndFoodEffects: false, Ingredients: []) { Compatibility = new("ItemSeederPreIndustrialFood_Renaissance_Prepared_Fruit_renaissance_food_andean_fruit_quinoa_like_bar", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingMakeRenaissancePreparedSweetBreadCatalogueStock() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/make Renaissance Prepared sweet bread catalogue stock", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make Renaissance Prepared sweet bread catalogue stock",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for renaissance_food_german_moulded_lebkuchen.",
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
			new CatalogueProgCookedRecipeProduct(Prog: "ItemSeederPreIndustrialFood_Renaissance_Prepared_Sweet_renaissance_food_german_fruit_stollen_loaf", RemoveDrugsAndFoodEffects: false, Ingredients: []) { Compatibility = new("ItemSeederPreIndustrialFood_Renaissance_Prepared_Sweet_renaissance_food_german_fruit_stollen_loaf", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingMakeRenaissancePreparedSweetEggCatalogueStock() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/make Renaissance Prepared sweet egg catalogue stock", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make Renaissance Prepared sweet egg catalogue stock",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for renaissance_food_iberian_egg_yolk_sweet.",
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
			new CatalogueProgCookedRecipeProduct(Prog: "ItemSeederPreIndustrialFood_Renaissance_Prepared_Sweet_renaissance_food_iberian_egg_yolk_sweet", RemoveDrugsAndFoodEffects: false, Ingredients: []) { Compatibility = new("ItemSeederPreIndustrialFood_Renaissance_Prepared_Sweet_renaissance_food_iberian_egg_yolk_sweet", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingMakeRenaissancePreparedSweetFoodCatalogueStock() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/make Renaissance Prepared sweet food catalogue stock", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make Renaissance Prepared sweet food catalogue stock",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for renaissance_food_italian_candied_citrus_peel.",
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
			new CatalogueProgCookedRecipeProduct(Prog: "ItemSeederPreIndustrialFood_Renaissance_Prepared_Sweet_renaissance_food_aztec_amaranth_deity_figure", RemoveDrugsAndFoodEffects: false, Ingredients: []) { Compatibility = new("ItemSeederPreIndustrialFood_Renaissance_Prepared_Sweet_renaissance_food_aztec_amaranth_deity_figure", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingMakeRenaissancePreparedSweetCheeseCatalogueStock() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/make Renaissance Prepared sweet cheese catalogue stock", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make Renaissance Prepared sweet cheese catalogue stock",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for renaissance_food_italian_fried_ravioli_sweet.",
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
			new CatalogueProgCookedRecipeProduct(Prog: "ItemSeederPreIndustrialFood_Renaissance_Prepared_Sweet_renaissance_food_english_cream_trifle", RemoveDrugsAndFoodEffects: false, Ingredients: []) { Compatibility = new("ItemSeederPreIndustrialFood_Renaissance_Prepared_Sweet_renaissance_food_english_cream_trifle", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingMakeRenaissancePreparedSweetRiceCatalogueStock() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/make Renaissance Prepared sweet rice catalogue stock", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make Renaissance Prepared sweet rice catalogue stock",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for renaissance_food_italian_rice_custard_torta.",
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
			new CatalogueProgCookedRecipeProduct(Prog: "ItemSeederPreIndustrialFood_Renaissance_Prepared_Sweet_renaissance_food_iberian_lenten_manjar_blanco", RemoveDrugsAndFoodEffects: false, Ingredients: []) { Compatibility = new("ItemSeederPreIndustrialFood_Renaissance_Prepared_Sweet_renaissance_food_iberian_lenten_manjar_blanco", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingMakeRenaissancePreparedSweetWheatCatalogueStock() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/make Renaissance Prepared sweet wheat catalogue stock", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make Renaissance Prepared sweet wheat catalogue stock",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for renaissance_food_italian_savoiardi_biscuit.",
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
			new CatalogueProgCookedRecipeProduct(Prog: "ItemSeederPreIndustrialFood_Renaissance_Prepared_Sweet_renaissance_food_english_banquet_knot_biscuit", RemoveDrugsAndFoodEffects: false, Ingredients: []) { Compatibility = new("ItemSeederPreIndustrialFood_Renaissance_Prepared_Sweet_renaissance_food_english_banquet_knot_biscuit", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingMakeRenaissancePreparedSweetHoneyCatalogueStock() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/make Renaissance Prepared sweet honey catalogue stock", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make Renaissance Prepared sweet honey catalogue stock",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for renaissance_food_italian_sugar_plate_castle.",
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
			new CatalogueProgCookedRecipeProduct(Prog: "ItemSeederPreIndustrialFood_Renaissance_Prepared_Sweet_renaissance_food_french_candied_flower_comfits", RemoveDrugsAndFoodEffects: false, Ingredients: []) { Compatibility = new("ItemSeederPreIndustrialFood_Renaissance_Prepared_Sweet_renaissance_food_french_candied_flower_comfits", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingMakeRenaissancePreparedSweetSorghumCatalogueStock() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/make Renaissance Prepared sweet sorghum catalogue stock", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make Renaissance Prepared sweet sorghum catalogue stock",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for renaissance_food_kongo_honey_sorghum_cake.",
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
			new CatalogueProgCookedRecipeProduct(Prog: "ItemSeederPreIndustrialFood_Renaissance_Prepared_Sweet_renaissance_food_kongo_honey_sorghum_cake", RemoveDrugsAndFoodEffects: false, Ingredients: []) { Compatibility = new("ItemSeederPreIndustrialFood_Renaissance_Prepared_Sweet_renaissance_food_kongo_honey_sorghum_cake", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingMakeRenaissancePreparedSweetCornCatalogueStock() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/make Renaissance Prepared sweet corn catalogue stock", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make Renaissance Prepared sweet corn catalogue stock",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for renaissance_food_maya_cacao_honey_tamales.",
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
			new CatalogueProgCookedRecipeProduct(Prog: "ItemSeederPreIndustrialFood_Renaissance_Prepared_Sweet_renaissance_food_andean_honeyed_maize_offering", RemoveDrugsAndFoodEffects: false, Ingredients: []) { Compatibility = new("ItemSeederPreIndustrialFood_Renaissance_Prepared_Sweet_renaissance_food_andean_honeyed_maize_offering", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingMakeRenaissancePreparedSweetDoughCatalogueStock() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/make Renaissance Prepared sweet dough catalogue stock", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make Renaissance Prepared sweet dough catalogue stock",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for renaissance_food_ottoman_baklava_palace_tray.",
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
			new CatalogueProgCookedRecipeProduct(Prog: "ItemSeederPreIndustrialFood_Renaissance_Prepared_Sweet_renaissance_food_ottoman_baklava_palace_tray", RemoveDrugsAndFoodEffects: false, Ingredients: []) { Compatibility = new("ItemSeederPreIndustrialFood_Renaissance_Prepared_Sweet_renaissance_food_ottoman_baklava_palace_tray", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingMakeRenaissancePreparedSweetVegetableCatalogueStock() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/make Renaissance Prepared sweet vegetable catalogue stock", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make Renaissance Prepared sweet vegetable catalogue stock",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for renaissance_food_spanish_candied_sweet_potato.",
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
			new CatalogueProgCookedRecipeProduct(Prog: "ItemSeederPreIndustrialFood_Renaissance_Prepared_Sweet_renaissance_food_spanish_candied_sweet_potato", RemoveDrugsAndFoodEffects: false, Ingredients: []) { Compatibility = new("ItemSeederPreIndustrialFood_Renaissance_Prepared_Sweet_renaissance_food_spanish_candied_sweet_potato", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingMakeRenaissancePreparedCondimentFoodCatalogueStock() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/make Renaissance Prepared condiment food catalogue stock", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make Renaissance Prepared condiment food catalogue stock",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for renaissance_food_italian_quince_mostarda.",
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
			new CatalogueProgCookedRecipeProduct(Prog: "ItemSeederPreIndustrialFood_Renaissance_Prepared_Condiment_renaissance_food_italian_fruit_must_mostarda", RemoveDrugsAndFoodEffects: false, Ingredients: []) { Compatibility = new("ItemSeederPreIndustrialFood_Renaissance_Prepared_Condiment_renaissance_food_italian_fruit_must_mostarda", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingMakeRenaissancePreparedCondimentVegetableCatalogueStock() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/make Renaissance Prepared condiment vegetable catalogue stock", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make Renaissance Prepared condiment vegetable catalogue stock",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for renaissance_food_mughal_new_chile_pickle.",
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
			new CatalogueCommodityTagRecipeInput(Grams: 1000.0, MaterialTag: "Vegetable", PileTag: "Seeded Yield", CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1 kilogram of a material tagged as Vegetable", ["piletag Seeded Yield"]), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Cooking", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Cooking tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueProgCookedRecipeProduct(Prog: "ItemSeederPreIndustrialFood_Renaissance_Prepared_Condiment_renaissance_food_joseon_new_chile_preserve", RemoveDrugsAndFoodEffects: false, Ingredients: []) { Compatibility = new("ItemSeederPreIndustrialFood_Renaissance_Prepared_Condiment_renaissance_food_joseon_new_chile_preserve", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingFillCatalogueLiquidRenaissanceFoodLiquidMughalSaffronAlmondSharbat() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/fill catalogue liquid renaissance_food_liquid_mughal_saffron_almond_sharbat", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:preindustrial_food_catalogue_liquid_amphora"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "fill catalogue liquid renaissance_food_liquid_mughal_saffron_almond_sharbat",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for renaissance_food_liquid_mughal_saffron_almond_sharbat.",
		Action = "preparing dairy drink food",
		ActiveCraftItemSdesc = "a dairy drink preparation",
		Gate = CatalogueRecipeGates.PreIndustrialFoodCatalogueProductionCooking1512,
		Trait = null,
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
			new(15, "$0 warm|warms $i1 gently, watching for the first skin to form.", "$0 warm|warms $i1 gently, watching for the first skin to form.", ExertionLevel.Stasis, 0),
			new(25, "$0 stir|stirs the dairy stock until its body becomes smooth and drinkable, using $t1.", "$0 stir|stirs the dairy stock until its body becomes smooth and drinkable, using $t1.", ExertionLevel.Stasis, 0),
			new(20, "$0 pour|pours $p1 into a clean vessel and let|lets it cool.", "$0 smell|smells the failed drink and consign|consigns it to the sour-stock jar.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityTagRecipeInput(Grams: 1000.0, MaterialTag: "Raw Milk", PileTag: "Raw Milk", CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1 kilogram of a material tagged as Raw Milk", ["piletag Raw Milk"]), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Cooking", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Cooking tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueLiquidRecipeProduct(ItemStableReference: "preindustrial_food_catalogue_liquid_amphora", Quantity: 1, SkinStableReference: null, Liquid: "Mughal saffron-almond sharbat", Litres: 10.0) { Compatibility = new("1x a shared catalogue liquid amphora (@preindustrial_food_catalogue_liquid_amphora) filled with 10 litres of Mughal saffron-almond sharbat", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingFillCatalogueLiquidRenaissanceFoodLiquidJoseonFiveFlavourPunch() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/fill catalogue liquid renaissance_food_liquid_joseon_five_flavour_punch", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:preindustrial_food_catalogue_liquid_amphora"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "fill catalogue liquid renaissance_food_liquid_joseon_five_flavour_punch",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for renaissance_food_liquid_joseon_five_flavour_punch.",
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
			new CatalogueLiquidRecipeProduct(ItemStableReference: "preindustrial_food_catalogue_liquid_amphora", Quantity: 1, SkinStableReference: null, Liquid: "Joseon five-flavour berry punch", Litres: 10.0) { Compatibility = new("1x a shared catalogue liquid amphora (@preindustrial_food_catalogue_liquid_amphora) filled with 10 litres of Joseon five-flavour berry punch", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingFillCatalogueLiquidRenaissanceFoodLiquidMughalSandalRoseSharbat() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/fill catalogue liquid renaissance_food_liquid_mughal_sandal_rose_sharbat", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:preindustrial_food_catalogue_liquid_amphora"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "fill catalogue liquid renaissance_food_liquid_mughal_sandal_rose_sharbat",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for renaissance_food_liquid_mughal_sandal_rose_sharbat.",
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
			new CatalogueLiquidRecipeProduct(ItemStableReference: "preindustrial_food_catalogue_liquid_amphora", Quantity: 1, SkinStableReference: null, Liquid: "Mughal sandalwood-rose sharbat", Litres: 10.0) { Compatibility = new("1x a shared catalogue liquid amphora (@preindustrial_food_catalogue_liquid_amphora) filled with 10 litres of Mughal sandalwood-rose sharbat", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingFillCatalogueLiquidRenaissanceFoodLiquidOttomanRoseSherbet() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/fill catalogue liquid renaissance_food_liquid_ottoman_rose_sherbet", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:preindustrial_food_catalogue_liquid_amphora"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "fill catalogue liquid renaissance_food_liquid_ottoman_rose_sherbet",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for renaissance_food_liquid_ottoman_rose_sherbet.",
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
			new CatalogueLiquidRecipeProduct(ItemStableReference: "preindustrial_food_catalogue_liquid_amphora", Quantity: 1, SkinStableReference: null, Liquid: "Ottoman rose-petal sherbet", Litres: 10.0) { Compatibility = new("1x a shared catalogue liquid amphora (@preindustrial_food_catalogue_liquid_amphora) filled with 10 litres of Ottoman rose-petal sherbet", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingFillCatalogueLiquidRenaissanceFoodLiquidOttomanSourCherrySherbet() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/fill catalogue liquid renaissance_food_liquid_ottoman_sour_cherry_sherbet", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:preindustrial_food_catalogue_liquid_amphora"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "fill catalogue liquid renaissance_food_liquid_ottoman_sour_cherry_sherbet",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for renaissance_food_liquid_ottoman_sour_cherry_sherbet.",
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
			new CatalogueLiquidRecipeProduct(ItemStableReference: "preindustrial_food_catalogue_liquid_amphora", Quantity: 1, SkinStableReference: null, Liquid: "Ottoman sour-cherry sherbet", Litres: 10.0) { Compatibility = new("1x a shared catalogue liquid amphora (@preindustrial_food_catalogue_liquid_amphora) filled with 10 litres of Ottoman sour-cherry sherbet", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingFillCatalogueLiquidRenaissanceFoodLiquidAtlanticMaizeMilletBeer() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/fill catalogue liquid renaissance_food_liquid_atlantic_maize_millet_beer", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:preindustrial_food_catalogue_liquid_amphora"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "fill catalogue liquid renaissance_food_liquid_atlantic_maize_millet_beer",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for renaissance_food_liquid_atlantic_maize_millet_beer.",
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
			new CatalogueLiquidRecipeProduct(ItemStableReference: "preindustrial_food_catalogue_liquid_amphora", Quantity: 1, SkinStableReference: null, Liquid: "Atlantic maize-millet beer", Litres: 10.0) { Compatibility = new("1x a shared catalogue liquid amphora (@preindustrial_food_catalogue_liquid_amphora) filled with 10 litres of Atlantic maize-millet beer", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingFillCatalogueLiquidRenaissanceFoodLiquidColonialAndeanAniseQuinoaChicha() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/fill catalogue liquid renaissance_food_liquid_colonial_andean_anise_quinoa_chicha", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:preindustrial_food_catalogue_liquid_amphora"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "fill catalogue liquid renaissance_food_liquid_colonial_andean_anise_quinoa_chicha",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for renaissance_food_liquid_colonial_andean_anise_quinoa_chicha.",
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
			new CatalogueLiquidRecipeProduct(ItemStableReference: "preindustrial_food_catalogue_liquid_amphora", Quantity: 1, SkinStableReference: null, Liquid: "colonial anise-quinoa chicha", Litres: 10.0) { Compatibility = new("1x a shared catalogue liquid amphora (@preindustrial_food_catalogue_liquid_amphora) filled with 10 litres of colonial anise-quinoa chicha", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingFillCatalogueLiquidRenaissanceFoodLiquidCanarySackCookingWine() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/fill catalogue liquid renaissance_food_liquid_canary_sack_cooking_wine", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:preindustrial_food_catalogue_liquid_amphora"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "fill catalogue liquid renaissance_food_liquid_canary_sack_cooking_wine",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for renaissance_food_liquid_canary_sack_cooking_wine.",
		Action = "preparing wine food",
		ActiveCraftItemSdesc = "a wine preparation",
		Gate = CatalogueRecipeGates.PreIndustrialFoodCatalogueProductionBrewing202,
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
			new(25, "$0 mash|mashes the wine stock in hot water, then draw|draws off the sweet wort, using $t1.", "$0 mash|mashes the wine stock in hot water, then draw|draws off the sweet wort, using $t1.", ExertionLevel.Stasis, 0),
			new(20, "$0 strain|strains $p1 and set|sets it aside to ferment and settle.", "$0 taste|tastes the failed brew and set|sets it aside as sour stock.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityTagRecipeInput(Grams: 1000.0, MaterialTag: "Fruit", PileTag: "Seeded Yield", CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1 kilogram of a material tagged as Fruit", ["piletag Seeded Yield"]), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Brew Copper", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Brew Copper tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueLiquidRecipeProduct(ItemStableReference: "preindustrial_food_catalogue_liquid_amphora", Quantity: 1, SkinStableReference: null, Liquid: "Canary sack cooking wine", Litres: 10.0) { Compatibility = new("1x a shared catalogue liquid amphora (@preindustrial_food_catalogue_liquid_amphora) filled with 10 litres of Canary sack cooking wine", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingFillCatalogueLiquidRenaissanceFoodLiquidCitrusPeelAquavitae() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/fill catalogue liquid renaissance_food_liquid_citrus_peel_aquavitae", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:preindustrial_food_catalogue_liquid_amphora"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "fill catalogue liquid renaissance_food_liquid_citrus_peel_aquavitae",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for renaissance_food_liquid_citrus_peel_aquavitae.",
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
			new CatalogueLiquidRecipeProduct(ItemStableReference: "preindustrial_food_catalogue_liquid_amphora", Quantity: 1, SkinStableReference: null, Liquid: "citrus-peel aqua-vitae cordial", Litres: 10.0) { Compatibility = new("1x a shared catalogue liquid amphora (@preindustrial_food_catalogue_liquid_amphora) filled with 10 litres of citrus-peel aqua-vitae cordial", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingFillCatalogueLiquidRenaissanceFoodLiquidRosemaryAquavitae() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/fill catalogue liquid renaissance_food_liquid_rosemary_aquavitae", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:preindustrial_food_catalogue_liquid_amphora"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "fill catalogue liquid renaissance_food_liquid_rosemary_aquavitae",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for renaissance_food_liquid_rosemary_aquavitae.",
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
			new CatalogueLiquidRecipeProduct(ItemStableReference: "preindustrial_food_catalogue_liquid_amphora", Quantity: 1, SkinStableReference: null, Liquid: "rosemary aqua-vitae cordial", Litres: 10.0) { Compatibility = new("1x a shared catalogue liquid amphora (@preindustrial_food_catalogue_liquid_amphora) filled with 10 litres of rosemary aqua-vitae cordial", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingFillCatalogueLiquidRenaissanceFoodLiquidSageHoneyCordial() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/fill catalogue liquid renaissance_food_liquid_sage_honey_cordial", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:preindustrial_food_catalogue_liquid_amphora"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "fill catalogue liquid renaissance_food_liquid_sage_honey_cordial",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for renaissance_food_liquid_sage_honey_cordial.",
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
			new CatalogueLiquidRecipeProduct(ItemStableReference: "preindustrial_food_catalogue_liquid_amphora", Quantity: 1, SkinStableReference: null, Liquid: "sage-and-honey cordial spirit", Litres: 10.0) { Compatibility = new("1x a shared catalogue liquid amphora (@preindustrial_food_catalogue_liquid_amphora) filled with 10 litres of sage-and-honey cordial spirit", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingFillCatalogueLiquidRenaissanceFoodLiquidJapaneseWhiskedTea() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/fill catalogue liquid renaissance_food_liquid_japanese_whisked_tea", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:preindustrial_food_catalogue_liquid_amphora"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "fill catalogue liquid renaissance_food_liquid_japanese_whisked_tea",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for renaissance_food_liquid_japanese_whisked_tea.",
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
			new CatalogueLiquidRecipeProduct(ItemStableReference: "preindustrial_food_catalogue_liquid_amphora", Quantity: 1, SkinStableReference: null, Liquid: "Japanese whisked tea liquor", Litres: 10.0) { Compatibility = new("1x a shared catalogue liquid amphora (@preindustrial_food_catalogue_liquid_amphora) filled with 10 litres of Japanese whisked tea liquor", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingFillCatalogueLiquidRenaissanceFoodLiquidMingChrysanthemumTea() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/fill catalogue liquid renaissance_food_liquid_ming_chrysanthemum_tea", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:preindustrial_food_catalogue_liquid_amphora"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "fill catalogue liquid renaissance_food_liquid_ming_chrysanthemum_tea",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for renaissance_food_liquid_ming_chrysanthemum_tea.",
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
			new CatalogueLiquidRecipeProduct(ItemStableReference: "preindustrial_food_catalogue_liquid_amphora", Quantity: 1, SkinStableReference: null, Liquid: "Ming chrysanthemum tea liquor", Litres: 10.0) { Compatibility = new("1x a shared catalogue liquid amphora (@preindustrial_food_catalogue_liquid_amphora) filled with 10 litres of Ming chrysanthemum tea liquor", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingFillCatalogueLiquidRenaissanceFoodLiquidMingSmokedPlumTea() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/fill catalogue liquid renaissance_food_liquid_ming_smoked_plum_tea", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:preindustrial_food_catalogue_liquid_amphora"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "fill catalogue liquid renaissance_food_liquid_ming_smoked_plum_tea",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for renaissance_food_liquid_ming_smoked_plum_tea.",
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
			new CatalogueLiquidRecipeProduct(ItemStableReference: "preindustrial_food_catalogue_liquid_amphora", Quantity: 1, SkinStableReference: null, Liquid: "Ming smoked-plum tea liquor", Litres: 10.0) { Compatibility = new("1x a shared catalogue liquid amphora (@preindustrial_food_catalogue_liquid_amphora) filled with 10 litres of Ming smoked-plum tea liquor", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingFillCatalogueLiquidRenaissanceFoodLiquidOttomanCardamomCoffee() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/fill catalogue liquid renaissance_food_liquid_ottoman_cardamom_coffee", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:preindustrial_food_catalogue_liquid_amphora"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "fill catalogue liquid renaissance_food_liquid_ottoman_cardamom_coffee",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for renaissance_food_liquid_ottoman_cardamom_coffee.",
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
			new CatalogueLiquidRecipeProduct(ItemStableReference: "preindustrial_food_catalogue_liquid_amphora", Quantity: 1, SkinStableReference: null, Liquid: "Ottoman cardamom coffee liquor", Litres: 10.0) { Compatibility = new("1x a shared catalogue liquid amphora (@preindustrial_food_catalogue_liquid_amphora) filled with 10 litres of Ottoman cardamom coffee liquor", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingFillCatalogueLiquidRenaissanceFoodLiquidAchioteCacao() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/fill catalogue liquid renaissance_food_liquid_achiote_cacao", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:preindustrial_food_catalogue_liquid_amphora"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "fill catalogue liquid renaissance_food_liquid_achiote_cacao",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for renaissance_food_liquid_achiote_cacao.",
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
			new CatalogueLiquidRecipeProduct(ItemStableReference: "preindustrial_food_catalogue_liquid_amphora", Quantity: 1, SkinStableReference: null, Liquid: "achiote-red cacao drink", Litres: 10.0) { Compatibility = new("1x a shared catalogue liquid amphora (@preindustrial_food_catalogue_liquid_amphora) filled with 10 litres of achiote-red cacao drink", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingFillCatalogueLiquidRenaissanceFoodLiquidChileMaizeCacao() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/fill catalogue liquid renaissance_food_liquid_chile_maize_cacao", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:preindustrial_food_catalogue_liquid_amphora"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "fill catalogue liquid renaissance_food_liquid_chile_maize_cacao",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for renaissance_food_liquid_chile_maize_cacao.",
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
			new CatalogueLiquidRecipeProduct(ItemStableReference: "preindustrial_food_catalogue_liquid_amphora", Quantity: 1, SkinStableReference: null, Liquid: "chile-maize cacao froth", Litres: 10.0) { Compatibility = new("1x a shared catalogue liquid amphora (@preindustrial_food_catalogue_liquid_amphora) filled with 10 litres of chile-maize cacao froth", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingFillCatalogueLiquidRenaissanceFoodLiquidVanillaFlowerCacao() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/fill catalogue liquid renaissance_food_liquid_vanilla_flower_cacao", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:preindustrial_food_catalogue_liquid_amphora"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "fill catalogue liquid renaissance_food_liquid_vanilla_flower_cacao",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for renaissance_food_liquid_vanilla_flower_cacao.",
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
			new CatalogueLiquidRecipeProduct(ItemStableReference: "preindustrial_food_catalogue_liquid_amphora", Quantity: 1, SkinStableReference: null, Liquid: "vanilla-flower cacao liquor", Litres: 10.0) { Compatibility = new("1x a shared catalogue liquid amphora (@preindustrial_food_catalogue_liquid_amphora) filled with 10 litres of vanilla-flower cacao liquor", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingFillCatalogueLiquidRenaissanceFoodLiquidBitterOrangeAgrodolce() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/fill catalogue liquid renaissance_food_liquid_bitter_orange_agrodolce", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:preindustrial_food_catalogue_liquid_amphora"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "fill catalogue liquid renaissance_food_liquid_bitter_orange_agrodolce",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for renaissance_food_liquid_bitter_orange_agrodolce.",
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
			new CatalogueLiquidRecipeProduct(ItemStableReference: "preindustrial_food_catalogue_liquid_amphora", Quantity: 1, SkinStableReference: null, Liquid: "bitter-orange agrodolce reduction", Litres: 10.0) { Compatibility = new("1x a shared catalogue liquid amphora (@preindustrial_food_catalogue_liquid_amphora) filled with 10 litres of bitter-orange agrodolce reduction", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingFillCatalogueLiquidRenaissanceFoodLiquidCaperCinnamonCourtSauce() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/fill catalogue liquid renaissance_food_liquid_caper_cinnamon_court_sauce", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:preindustrial_food_catalogue_liquid_amphora"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "fill catalogue liquid renaissance_food_liquid_caper_cinnamon_court_sauce",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for renaissance_food_liquid_caper_cinnamon_court_sauce.",
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
			new CatalogueLiquidRecipeProduct(ItemStableReference: "preindustrial_food_catalogue_liquid_amphora", Quantity: 1, SkinStableReference: null, Liquid: "caper-cinnamon court sauce", Litres: 10.0) { Compatibility = new("1x a shared catalogue liquid amphora (@preindustrial_food_catalogue_liquid_amphora) filled with 10 litres of caper-cinnamon court sauce", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingFillCatalogueLiquidRenaissanceFoodLiquidJapaneseIrizake() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/fill catalogue liquid renaissance_food_liquid_japanese_irizake", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:preindustrial_food_catalogue_liquid_amphora"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "fill catalogue liquid renaissance_food_liquid_japanese_irizake",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for renaissance_food_liquid_japanese_irizake.",
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
			new CatalogueLiquidRecipeProduct(ItemStableReference: "preindustrial_food_catalogue_liquid_amphora", Quantity: 1, SkinStableReference: null, Liquid: "Japanese iri-zake cooking liquor", Litres: 10.0) { Compatibility = new("1x a shared catalogue liquid amphora (@preindustrial_food_catalogue_liquid_amphora) filled with 10 litres of Japanese iri-zake cooking liquor", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingFillCatalogueLiquidRenaissanceFoodLiquidMingBlackBeanLiquor() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/fill catalogue liquid renaissance_food_liquid_ming_black_bean_liquor", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:preindustrial_food_catalogue_liquid_amphora"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "fill catalogue liquid renaissance_food_liquid_ming_black_bean_liquor",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for renaissance_food_liquid_ming_black_bean_liquor.",
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
			new CatalogueLiquidRecipeProduct(ItemStableReference: "preindustrial_food_catalogue_liquid_amphora", Quantity: 1, SkinStableReference: null, Liquid: "Ming fermented-black-bean cooking liquor", Litres: 10.0) { Compatibility = new("1x a shared catalogue liquid amphora (@preindustrial_food_catalogue_liquid_amphora) filled with 10 litres of Ming fermented-black-bean cooking liquor", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingFillCatalogueLiquidRenaissanceFoodLiquidRaisinPinenutSauce() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/fill catalogue liquid renaissance_food_liquid_raisin_pinenut_sauce", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:preindustrial_food_catalogue_liquid_amphora"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "fill catalogue liquid renaissance_food_liquid_raisin_pinenut_sauce",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for renaissance_food_liquid_raisin_pinenut_sauce.",
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
			new CatalogueLiquidRecipeProduct(ItemStableReference: "preindustrial_food_catalogue_liquid_amphora", Quantity: 1, SkinStableReference: null, Liquid: "raisin-pine-nut court sauce", Litres: 10.0) { Compatibility = new("1x a shared catalogue liquid amphora (@preindustrial_food_catalogue_liquid_amphora) filled with 10 litres of raisin-pine-nut court sauce", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingFillCatalogueLiquidRenaissanceFoodLiquidRosewaterVerjuiceEmulsion() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/fill catalogue liquid renaissance_food_liquid_rosewater_verjuice_emulsion", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:preindustrial_food_catalogue_liquid_amphora"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "fill catalogue liquid renaissance_food_liquid_rosewater_verjuice_emulsion",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for renaissance_food_liquid_rosewater_verjuice_emulsion.",
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
			new CatalogueLiquidRecipeProduct(ItemStableReference: "preindustrial_food_catalogue_liquid_amphora", Quantity: 1, SkinStableReference: null, Liquid: "rosewater verjuice emulsion", Litres: 10.0) { Compatibility = new("1x a shared catalogue liquid amphora (@preindustrial_food_catalogue_liquid_amphora) filled with 10 litres of rosewater verjuice emulsion", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingFillCatalogueLiquidRenaissanceFoodLiquidSafavidMintSekanjabin() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/fill catalogue liquid renaissance_food_liquid_safavid_mint_sekanjabin", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:preindustrial_food_catalogue_liquid_amphora"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "fill catalogue liquid renaissance_food_liquid_safavid_mint_sekanjabin",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for renaissance_food_liquid_safavid_mint_sekanjabin.",
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
			new CatalogueLiquidRecipeProduct(ItemStableReference: "preindustrial_food_catalogue_liquid_amphora", Quantity: 1, SkinStableReference: null, Liquid: "Safavid mint sekanjabin concentrate", Litres: 10.0) { Compatibility = new("1x a shared catalogue liquid amphora (@preindustrial_food_catalogue_liquid_amphora) filled with 10 litres of Safavid mint sekanjabin concentrate", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingMakeRenaissanceIntermediateIntermediateMeatCatalogueStock() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/make Renaissance Intermediate intermediate meat catalogue stock", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make Renaissance Intermediate intermediate meat catalogue stock",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for renaissance_food_capon_forcemeat_bowl.",
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
			new CatalogueProgRecipeProduct(Prog: "ItemSeederPreIndustrialFood_Renaissance_Intermediate_Intermediate_renaissance_food_capon_forcemeat_bowl") { Compatibility = new("ItemSeederPreIndustrialFood_Renaissance_Intermediate_Intermediate_renaissance_food_capon_forcemeat_bowl", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingMakeRenaissanceIntermediateIntermediateFoodCatalogueStock() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/make Renaissance Intermediate intermediate food catalogue stock", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make Renaissance Intermediate intermediate food catalogue stock",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for renaissance_food_citrus_peel_first_syrup_stage.",
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
			new CatalogueProgRecipeProduct(Prog: "ItemSeederPreIndustrialFood_Renaissance_Intermediate_Intermediate_renaissance_food_citrus_peel_first_syrup_stage") { Compatibility = new("ItemSeederPreIndustrialFood_Renaissance_Intermediate_Intermediate_renaissance_food_citrus_peel_first_syrup_stage", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingMakeRenaissanceIntermediateIntermediateFishCatalogueStock() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/make Renaissance Intermediate intermediate fish catalogue stock", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make Renaissance Intermediate intermediate fish catalogue stock",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for renaissance_food_escabeche_uncooked_marinade_fish.",
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
			new CatalogueProgRecipeProduct(Prog: "ItemSeederPreIndustrialFood_Renaissance_Intermediate_Intermediate_renaissance_food_empanada_filled_raw_round") { Compatibility = new("ItemSeederPreIndustrialFood_Renaissance_Intermediate_Intermediate_renaissance_food_empanada_filled_raw_round", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingMakeRenaissanceIntermediateIntermediateVegetableCatalogueStock() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/make Renaissance Intermediate intermediate vegetable catalogue stock", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make Renaissance Intermediate intermediate vegetable catalogue stock",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for renaissance_food_ming_mock_goose_uncooked_roll.",
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
			new CatalogueProgRecipeProduct(Prog: "ItemSeederPreIndustrialFood_Renaissance_Intermediate_Intermediate_renaissance_food_chuno_thawing_press_bundle") { Compatibility = new("ItemSeederPreIndustrialFood_Renaissance_Intermediate_Intermediate_renaissance_food_chuno_thawing_press_bundle", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingMakeRenaissanceIntermediateIntermediateCornCatalogueStock() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/make Renaissance Intermediate intermediate corn catalogue stock", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make Renaissance Intermediate intermediate corn catalogue stock",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for renaissance_food_nixtamal_wet_maize_dough.",
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
			new CatalogueProgRecipeProduct(Prog: "ItemSeederPreIndustrialFood_Renaissance_Intermediate_Intermediate_renaissance_food_nixtamal_wet_maize_dough") { Compatibility = new("ItemSeederPreIndustrialFood_Renaissance_Intermediate_Intermediate_renaissance_food_nixtamal_wet_maize_dough", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingMakeRenaissanceIntermediateIntermediateWheatCatalogueStock() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/make Renaissance Intermediate intermediate wheat catalogue stock", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make Renaissance Intermediate intermediate wheat catalogue stock",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for renaissance_food_pasta_sheet_drying_frame.",
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
			new CatalogueProgRecipeProduct(Prog: "ItemSeederPreIndustrialFood_Renaissance_Intermediate_Intermediate_renaissance_food_pasta_sheet_drying_frame") { Compatibility = new("ItemSeederPreIndustrialFood_Renaissance_Intermediate_Intermediate_renaissance_food_pasta_sheet_drying_frame", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingMakeRenaissanceIntermediateIntermediateHoneyCatalogueStock() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/make Renaissance Intermediate intermediate honey catalogue stock", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make Renaissance Intermediate intermediate honey catalogue stock",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for renaissance_food_rolled_sugar_plate_sheet.",
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
			new CatalogueCommodityTagRecipeInput(Grams: 1000.0, MaterialTag: "Food", PileTag: "Pressed Honey", CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1 kilogram of a material tagged as Food", ["piletag Pressed Honey"]), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Cooking", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Cooking tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueProgRecipeProduct(Prog: "ItemSeederPreIndustrialFood_Renaissance_Intermediate_Intermediate_renaissance_food_pulled_sugar_working_lump") { Compatibility = new("ItemSeederPreIndustrialFood_Renaissance_Intermediate_Intermediate_renaissance_food_pulled_sugar_working_lump", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingMakeRenaissanceIntermediateIntermediateDoughCatalogueStock() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/make Renaissance Intermediate intermediate dough catalogue stock", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make Renaissance Intermediate intermediate dough catalogue stock",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for renaissance_food_unbaked_torta_shell.",
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
			new CatalogueProgRecipeProduct(Prog: "ItemSeederPreIndustrialFood_Renaissance_Intermediate_Intermediate_renaissance_food_baklava_unbaked_layered_tray") { Compatibility = new("ItemSeederPreIndustrialFood_Renaissance_Intermediate_Intermediate_renaissance_food_baklava_unbaked_layered_tray", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingMakeRenaissanceIntermediateIntermediateCheeseCatalogueStock() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/make Renaissance Intermediate intermediate cheese catalogue stock", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make Renaissance Intermediate intermediate cheese catalogue stock",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for renaissance_food_uncooked_ravioli_tray.",
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
			new CatalogueProgRecipeProduct(Prog: "ItemSeederPreIndustrialFood_Renaissance_Intermediate_Intermediate_renaissance_food_uncooked_ravioli_tray") { Compatibility = new("ItemSeederPreIndustrialFood_Renaissance_Intermediate_Intermediate_renaissance_food_uncooked_ravioli_tray", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingMakeEarlyModernPreparedGrainWheatCatalogueStock() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/make EarlyModern Prepared grain wheat catalogue stock", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make EarlyModern Prepared grain wheat catalogue stock",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for earlymodern_food_british_yorkshire_batter_pudding.",
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
			new CatalogueProgCookedRecipeProduct(Prog: "ItemSeederPreIndustrialFood_EarlyModern_Prepared_Grain_earlymodern_food_british_yorkshire_batter_pudding", RemoveDrugsAndFoodEffects: false, Ingredients: []) { Compatibility = new("ItemSeederPreIndustrialFood_EarlyModern_Prepared_Grain_earlymodern_food_british_yorkshire_batter_pudding", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingMakeEarlyModernPreparedGrainCornCatalogueStock() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/make EarlyModern Prepared grain corn catalogue stock", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make EarlyModern Prepared grain corn catalogue stock",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for earlymodern_food_mediterranean_maize_cheese_porridge.",
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
			new CatalogueCommodityTagRecipeInput(Grams: 1000.0, MaterialTag: "Vegetable", PileTag: "Seeded Yield", CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1 kilogram of a material tagged as Vegetable", ["piletag Seeded Yield"]), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Cooking", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Cooking tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueProgCookedRecipeProduct(Prog: "ItemSeederPreIndustrialFood_EarlyModern_Prepared_Grain_earlymodern_food_caribbean_enslaved_cornmeal_ration", RemoveDrugsAndFoodEffects: false, Ingredients: []) { Compatibility = new("ItemSeederPreIndustrialFood_EarlyModern_Prepared_Grain_earlymodern_food_caribbean_enslaved_cornmeal_ration", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingMakeEarlyModernPreparedGrainRiceCatalogueStock() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/make EarlyModern Prepared grain rice catalogue stock", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make EarlyModern Prepared grain rice catalogue stock",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for earlymodern_food_safavid_crisp_rice_crust_cake.",
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
			new CatalogueProgCookedRecipeProduct(Prog: "ItemSeederPreIndustrialFood_EarlyModern_Prepared_Grain_earlymodern_food_louisiana_african_rice_fritter", RemoveDrugsAndFoodEffects: false, Ingredients: []) { Compatibility = new("ItemSeederPreIndustrialFood_EarlyModern_Prepared_Grain_earlymodern_food_louisiana_african_rice_fritter", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingMakeEarlyModernPreparedGrainFoodCatalogueStock() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/make EarlyModern Prepared grain food catalogue stock", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make EarlyModern Prepared grain food catalogue stock",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for earlymodern_food_tokugawa_bento_theatre_box.",
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
			new CatalogueProgCookedRecipeProduct(Prog: "ItemSeederPreIndustrialFood_EarlyModern_Prepared_Grain_earlymodern_food_joseon_royal_gujeolpan", RemoveDrugsAndFoodEffects: false, Ingredients: []) { Compatibility = new("ItemSeederPreIndustrialFood_EarlyModern_Prepared_Grain_earlymodern_food_joseon_royal_gujeolpan", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingMakeEarlyModernPreparedBreadVegetableCatalogueStock() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/make EarlyModern Prepared bread vegetable catalogue stock", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make EarlyModern Prepared bread vegetable catalogue stock",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for earlymodern_food_caribbean_cassava_bammy.",
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
			new CatalogueProgCookedRecipeProduct(Prog: "ItemSeederPreIndustrialFood_EarlyModern_Prepared_Bread_earlymodern_food_caribbean_cassava_bammy", RemoveDrugsAndFoodEffects: false, Ingredients: []) { Compatibility = new("ItemSeederPreIndustrialFood_EarlyModern_Prepared_Bread_earlymodern_food_caribbean_cassava_bammy", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingMakeEarlyModernPreparedBreadBreadCatalogueStock() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/make EarlyModern Prepared bread bread catalogue stock", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make EarlyModern Prepared bread bread catalogue stock",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for earlymodern_food_french_coffee_house_brioche.",
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
			new CatalogueProgCookedRecipeProduct(Prog: "ItemSeederPreIndustrialFood_EarlyModern_Prepared_Bread_earlymodern_food_british_tea_rusk", RemoveDrugsAndFoodEffects: false, Ingredients: []) { Compatibility = new("ItemSeederPreIndustrialFood_EarlyModern_Prepared_Bread_earlymodern_food_british_tea_rusk", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingMakeEarlyModernPreparedPorridgeCornCatalogueStock() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/make EarlyModern Prepared porridge corn catalogue stock", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make EarlyModern Prepared porridge corn catalogue stock",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for earlymodern_food_qing_maize_millet_relief_porridge.",
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
			new CatalogueProgCookedRecipeProduct(Prog: "ItemSeederPreIndustrialFood_EarlyModern_Prepared_Porridge_earlymodern_food_qing_maize_millet_relief_porridge", RemoveDrugsAndFoodEffects: false, Ingredients: []) { Compatibility = new("ItemSeederPreIndustrialFood_EarlyModern_Prepared_Porridge_earlymodern_food_qing_maize_millet_relief_porridge", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingMakeEarlyModernPreparedNoodleWheatCatalogueStock() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/make EarlyModern Prepared noodle wheat catalogue stock", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make EarlyModern Prepared noodle wheat catalogue stock",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for earlymodern_food_qing_chile_oil_noodles.",
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
			new CatalogueProgCookedRecipeProduct(Prog: "ItemSeederPreIndustrialFood_EarlyModern_Prepared_Noodle_earlymodern_food_qing_chile_oil_noodles", RemoveDrugsAndFoodEffects: false, Ingredients: []) { Compatibility = new("ItemSeederPreIndustrialFood_EarlyModern_Prepared_Noodle_earlymodern_food_qing_chile_oil_noodles", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingMakeEarlyModernPreparedDumplingDoughCatalogueStock() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/make EarlyModern Prepared dumpling dough catalogue stock", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make EarlyModern Prepared dumpling dough catalogue stock",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for earlymodern_food_british_steamed_suet_dumplings.",
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
			new CatalogueProgCookedRecipeProduct(Prog: "ItemSeederPreIndustrialFood_EarlyModern_Prepared_Dumpling_earlymodern_food_british_steamed_suet_dumplings", RemoveDrugsAndFoodEffects: false, Ingredients: []) { Compatibility = new("ItemSeederPreIndustrialFood_EarlyModern_Prepared_Dumpling_earlymodern_food_british_steamed_suet_dumplings", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingMakeEarlyModernPreparedDumplingVegetableCatalogueStock() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/make EarlyModern Prepared dumpling vegetable catalogue stock", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make EarlyModern Prepared dumpling vegetable catalogue stock",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for earlymodern_food_german_potato_dumplings.",
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
			new CatalogueProgCookedRecipeProduct(Prog: "ItemSeederPreIndustrialFood_EarlyModern_Prepared_Dumpling_earlymodern_food_german_potato_dumplings", RemoveDrugsAndFoodEffects: false, Ingredients: []) { Compatibility = new("ItemSeederPreIndustrialFood_EarlyModern_Prepared_Dumpling_earlymodern_food_german_potato_dumplings", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingMakeEarlyModernPreparedPulseFoodCatalogueStock() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/make EarlyModern Prepared pulse food catalogue stock", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make EarlyModern Prepared pulse food catalogue stock",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for earlymodern_food_british_naval_pease_pudding.",
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
			new CatalogueProgCookedRecipeProduct(Prog: "ItemSeederPreIndustrialFood_EarlyModern_Prepared_Pulse_earlymodern_food_brazilian_enslaved_bean_cake", RemoveDrugsAndFoodEffects: false, Ingredients: []) { Compatibility = new("ItemSeederPreIndustrialFood_EarlyModern_Prepared_Pulse_earlymodern_food_brazilian_enslaved_bean_cake", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingMakeEarlyModernPreparedVegetableEggCatalogueStock() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/make EarlyModern Prepared vegetable egg catalogue stock", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make EarlyModern Prepared vegetable egg catalogue stock",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for earlymodern_food_french_entremet_spinach_custard.",
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
			new CatalogueCommodityTagRecipeInput(Grams: 1000.0, MaterialTag: "Egg Product", PileTag: "Egg Product", CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1 kilogram of a material tagged as Egg Product", ["piletag Egg Product"]), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Cooking", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Cooking tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueProgCookedRecipeProduct(Prog: "ItemSeederPreIndustrialFood_EarlyModern_Prepared_Vegetable_earlymodern_food_french_entremet_spinach_custard", RemoveDrugsAndFoodEffects: false, Ingredients: []) { Compatibility = new("ItemSeederPreIndustrialFood_EarlyModern_Prepared_Vegetable_earlymodern_food_french_entremet_spinach_custard", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};
}
