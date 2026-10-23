#nullable enable

using MudSharp.Body;
using MudSharp.GameItems.Inventory.Plans;
using MudSharp.RPG.Checks;

namespace DatabaseSeeder.Seeders;

public static partial class Recipes06Stock
{
	public static CatalogueRecipeDefinition PreIndustrialFoodmakingFillCatalogueLiquidMedievalFoodLiquidMonasticParsleyBroth() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/fill catalogue liquid medieval_food_liquid_monastic_parsley_broth", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:preindustrial_food_catalogue_liquid_amphora"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "fill catalogue liquid medieval_food_liquid_monastic_parsley_broth",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for medieval_food_liquid_monastic_parsley_broth.",
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
			new CatalogueLiquidRecipeProduct(ItemStableReference: "preindustrial_food_catalogue_liquid_amphora", Quantity: 1, SkinStableReference: null, Liquid: "monastic parsley broth", Litres: 10.0) { Compatibility = new("1x a shared catalogue liquid amphora (@preindustrial_food_catalogue_liquid_amphora) filled with 10 litres of monastic parsley broth", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingFillCatalogueLiquidMedievalFoodLiquidAlmondSaffronCaudle() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/fill catalogue liquid medieval_food_liquid_almond_saffron_caudle", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:preindustrial_food_catalogue_liquid_amphora"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "fill catalogue liquid medieval_food_liquid_almond_saffron_caudle",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for medieval_food_liquid_almond_saffron_caudle.",
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
			new CatalogueLiquidRecipeProduct(ItemStableReference: "preindustrial_food_catalogue_liquid_amphora", Quantity: 1, SkinStableReference: null, Liquid: "almond-saffron caudle", Litres: 10.0) { Compatibility = new("1x a shared catalogue liquid amphora (@preindustrial_food_catalogue_liquid_amphora) filled with 10 litres of almond-saffron caudle", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingFillCatalogueLiquidMedievalFoodLiquidConventWalnutMilk() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/fill catalogue liquid medieval_food_liquid_convent_walnut_milk", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:preindustrial_food_catalogue_liquid_amphora"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "fill catalogue liquid medieval_food_liquid_convent_walnut_milk",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for medieval_food_liquid_convent_walnut_milk.",
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
			new CatalogueLiquidRecipeProduct(ItemStableReference: "preindustrial_food_catalogue_liquid_amphora", Quantity: 1, SkinStableReference: null, Liquid: "convent walnut milk", Litres: 10.0) { Compatibility = new("1x a shared catalogue liquid amphora (@preindustrial_food_catalogue_liquid_amphora) filled with 10 litres of convent walnut milk", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingFillCatalogueLiquidMedievalFoodLiquidLentenAlmondDraught() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/fill catalogue liquid medieval_food_liquid_lenten_almond_draught", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:preindustrial_food_catalogue_liquid_amphora"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "fill catalogue liquid medieval_food_liquid_lenten_almond_draught",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for medieval_food_liquid_lenten_almond_draught.",
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
			new CatalogueLiquidRecipeProduct(ItemStableReference: "preindustrial_food_catalogue_liquid_amphora", Quantity: 1, SkinStableReference: null, Liquid: "Lenten almond draught", Litres: 10.0) { Compatibility = new("1x a shared catalogue liquid amphora (@preindustrial_food_catalogue_liquid_amphora) filled with 10 litres of Lenten almond draught", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingFillCatalogueLiquidMedievalFoodLiquidPoppySeedFastEmulsion() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/fill catalogue liquid medieval_food_liquid_poppy_seed_fast_emulsion", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:preindustrial_food_catalogue_liquid_amphora"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "fill catalogue liquid medieval_food_liquid_poppy_seed_fast_emulsion",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for medieval_food_liquid_poppy_seed_fast_emulsion.",
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
			new CatalogueLiquidRecipeProduct(ItemStableReference: "preindustrial_food_catalogue_liquid_amphora", Quantity: 1, SkinStableReference: null, Liquid: "poppy-seed fast emulsion", Litres: 10.0) { Compatibility = new("1x a shared catalogue liquid amphora (@preindustrial_food_catalogue_liquid_amphora) filled with 10 litres of poppy-seed fast emulsion", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingFillCatalogueLiquidMedievalFoodLiquidHoneyedPomegranateDraught() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/fill catalogue liquid medieval_food_liquid_honeyed_pomegranate_draught", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:preindustrial_food_catalogue_liquid_amphora"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "fill catalogue liquid medieval_food_liquid_honeyed_pomegranate_draught",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for medieval_food_liquid_honeyed_pomegranate_draught.",
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
			new CatalogueLiquidRecipeProduct(ItemStableReference: "preindustrial_food_catalogue_liquid_amphora", Quantity: 1, SkinStableReference: null, Liquid: "honeyed pomegranate table draught", Litres: 10.0) { Compatibility = new("1x a shared catalogue liquid amphora (@preindustrial_food_catalogue_liquid_amphora) filled with 10 litres of honeyed pomegranate table draught", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingFillCatalogueLiquidMedievalFoodLiquidBreadCaudle() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/fill catalogue liquid medieval_food_liquid_bread_caudle", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:preindustrial_food_catalogue_liquid_amphora"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "fill catalogue liquid medieval_food_liquid_bread_caudle",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for medieval_food_liquid_bread_caudle.",
		Action = "preparing grain drink food",
		ActiveCraftItemSdesc = "a grain drink preparation",
		Gate = CatalogueRecipeGates.PreIndustrialFoodCatalogueProductionCooking1514,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 3,
		Interruptable = false,
		Phases = [
			new(15, "$0 crush|crushes $i1 and stir|stirs it into the brewing vessel.", "$0 crush|crushes $i1 and stir|stirs it into the brewing vessel.", ExertionLevel.Stasis, 0),
			new(25, "$0 mash|mashes the grain drink stock in hot water, then draw|draws off the sweet wort, using $t1.", "$0 mash|mashes the grain drink stock in hot water, then draw|draws off the sweet wort, using $t1.", ExertionLevel.Stasis, 0),
			new(20, "$0 strain|strains $p1 and set|sets it aside to ferment and settle.", "$0 taste|tastes the failed brew and set|sets it aside as sour stock.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityTagRecipeInput(Grams: 1000.0, MaterialTag: "Food Crop", PileTag: "Seeded Yield", CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1 kilogram of a material tagged as Food Crop", ["piletag Seeded Yield"]), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Brew Copper", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Brew Copper tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueLiquidRecipeProduct(ItemStableReference: "preindustrial_food_catalogue_liquid_amphora", Quantity: 1, SkinStableReference: null, Liquid: "bread-thickened sickroom caudle", Litres: 10.0) { Compatibility = new("1x a shared catalogue liquid amphora (@preindustrial_food_catalogue_liquid_amphora) filled with 10 litres of bread-thickened sickroom caudle", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingFillCatalogueLiquidMedievalFoodLiquidInfirmaryBarleyWater() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/fill catalogue liquid medieval_food_liquid_infirmary_barley_water", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:preindustrial_food_catalogue_liquid_amphora"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "fill catalogue liquid medieval_food_liquid_infirmary_barley_water",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for medieval_food_liquid_infirmary_barley_water.",
		Action = "preparing grain drink food",
		ActiveCraftItemSdesc = "a grain drink preparation",
		Gate = CatalogueRecipeGates.PreIndustrialFoodCatalogueProductionCooking1514,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 3,
		Interruptable = false,
		Phases = [
			new(15, "$0 crush|crushes $i1 and stir|stirs it into the brewing vessel.", "$0 crush|crushes $i1 and stir|stirs it into the brewing vessel.", ExertionLevel.Stasis, 0),
			new(25, "$0 mash|mashes the grain drink stock in hot water, then draw|draws off the sweet wort, using $t1.", "$0 mash|mashes the grain drink stock in hot water, then draw|draws off the sweet wort, using $t1.", ExertionLevel.Stasis, 0),
			new(20, "$0 strain|strains $p1 and set|sets it aside to ferment and settle.", "$0 taste|tastes the failed brew and set|sets it aside as sour stock.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityTagRecipeInput(Grams: 1000.0, MaterialTag: "Food Crop", PileTag: "Seeded Yield", CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1 kilogram of a material tagged as Food Crop", ["piletag Seeded Yield"]), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Brew Copper", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Brew Copper tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueLiquidRecipeProduct(ItemStableReference: "preindustrial_food_catalogue_liquid_amphora", Quantity: 1, SkinStableReference: null, Liquid: "infirmary barley water", Litres: 10.0) { Compatibility = new("1x a shared catalogue liquid amphora (@preindustrial_food_catalogue_liquid_amphora) filled with 10 litres of infirmary barley water", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingFillCatalogueLiquidMedievalFoodLiquidOatSouringLiquor() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/fill catalogue liquid medieval_food_liquid_oat_souring_liquor", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:preindustrial_food_catalogue_liquid_amphora"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "fill catalogue liquid medieval_food_liquid_oat_souring_liquor",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for medieval_food_liquid_oat_souring_liquor.",
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
			new CatalogueLiquidRecipeProduct(ItemStableReference: "preindustrial_food_catalogue_liquid_amphora", Quantity: 1, SkinStableReference: null, Liquid: "oat-pottage souring liquor", Litres: 10.0) { Compatibility = new("1x a shared catalogue liquid amphora (@preindustrial_food_catalogue_liquid_amphora) filled with 10 litres of oat-pottage souring liquor", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingFillCatalogueLiquidMedievalFoodLiquidSpicedOxymel() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/fill catalogue liquid medieval_food_liquid_spiced_oxymel", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:preindustrial_food_catalogue_liquid_amphora"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "fill catalogue liquid medieval_food_liquid_spiced_oxymel",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for medieval_food_liquid_spiced_oxymel.",
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
			new CatalogueLiquidRecipeProduct(ItemStableReference: "preindustrial_food_catalogue_liquid_amphora", Quantity: 1, SkinStableReference: null, Liquid: "spiced monastic oxymel", Litres: 10.0) { Compatibility = new("1x a shared catalogue liquid amphora (@preindustrial_food_catalogue_liquid_amphora) filled with 10 litres of spiced monastic oxymel", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingFillCatalogueLiquidMedievalFoodLiquidClarifiedHippocras() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/fill catalogue liquid medieval_food_liquid_clarified_hippocras", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:preindustrial_food_catalogue_liquid_amphora"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "fill catalogue liquid medieval_food_liquid_clarified_hippocras",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for medieval_food_liquid_clarified_hippocras.",
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
			new CatalogueLiquidRecipeProduct(ItemStableReference: "preindustrial_food_catalogue_liquid_amphora", Quantity: 1, SkinStableReference: null, Liquid: "clarified court hippocras", Litres: 10.0) { Compatibility = new("1x a shared catalogue liquid amphora (@preindustrial_food_catalogue_liquid_amphora) filled with 10 litres of clarified court hippocras", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingFillCatalogueLiquidMedievalFoodLiquidMonasticHerbWine() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/fill catalogue liquid medieval_food_liquid_monastic_herb_wine", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:preindustrial_food_catalogue_liquid_amphora"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "fill catalogue liquid medieval_food_liquid_monastic_herb_wine",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for medieval_food_liquid_monastic_herb_wine.",
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
			new CatalogueLiquidRecipeProduct(ItemStableReference: "preindustrial_food_catalogue_liquid_amphora", Quantity: 1, SkinStableReference: null, Liquid: "monastic bitter-herb wine", Litres: 10.0) { Compatibility = new("1x a shared catalogue liquid amphora (@preindustrial_food_catalogue_liquid_amphora) filled with 10 litres of monastic bitter-herb wine", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingFillCatalogueLiquidMedievalFoodLiquidWarmedPepperedWine() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/fill catalogue liquid medieval_food_liquid_warmed_peppered_wine", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:preindustrial_food_catalogue_liquid_amphora"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "fill catalogue liquid medieval_food_liquid_warmed_peppered_wine",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for medieval_food_liquid_warmed_peppered_wine.",
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
			new CatalogueLiquidRecipeProduct(ItemStableReference: "preindustrial_food_catalogue_liquid_amphora", Quantity: 1, SkinStableReference: null, Liquid: "warmed peppered wine", Litres: 10.0) { Compatibility = new("1x a shared catalogue liquid amphora (@preindustrial_food_catalogue_liquid_amphora) filled with 10 litres of warmed peppered wine", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingFillCatalogueLiquidMedievalFoodLiquidAleMustardSauce() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/fill catalogue liquid medieval_food_liquid_ale_mustard_sauce", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:preindustrial_food_catalogue_liquid_amphora"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "fill catalogue liquid medieval_food_liquid_ale_mustard_sauce",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for medieval_food_liquid_ale_mustard_sauce.",
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
			new CatalogueLiquidRecipeProduct(ItemStableReference: "preindustrial_food_catalogue_liquid_amphora", Quantity: 1, SkinStableReference: null, Liquid: "ale-mustard trencher sauce", Litres: 10.0) { Compatibility = new("1x a shared catalogue liquid amphora (@preindustrial_food_catalogue_liquid_amphora) filled with 10 litres of ale-mustard trencher sauce", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingFillCatalogueLiquidMedievalFoodLiquidBreadGarlicJance() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/fill catalogue liquid medieval_food_liquid_bread_garlic_jance", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:preindustrial_food_catalogue_liquid_amphora"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "fill catalogue liquid medieval_food_liquid_bread_garlic_jance",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for medieval_food_liquid_bread_garlic_jance.",
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
			new CatalogueLiquidRecipeProduct(ItemStableReference: "preindustrial_food_catalogue_liquid_amphora", Quantity: 1, SkinStableReference: null, Liquid: "breaded garlic jance", Litres: 10.0) { Compatibility = new("1x a shared catalogue liquid amphora (@preindustrial_food_catalogue_liquid_amphora) filled with 10 litres of breaded garlic jance", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingFillCatalogueLiquidMedievalFoodLiquidGreenGrapeVerjuice() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/fill catalogue liquid medieval_food_liquid_green_grape_verjuice", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:preindustrial_food_catalogue_liquid_amphora"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "fill catalogue liquid medieval_food_liquid_green_grape_verjuice",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for medieval_food_liquid_green_grape_verjuice.",
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
			new CatalogueLiquidRecipeProduct(ItemStableReference: "preindustrial_food_catalogue_liquid_amphora", Quantity: 1, SkinStableReference: null, Liquid: "green-grape cook's verjuice", Litres: 10.0) { Compatibility = new("1x a shared catalogue liquid amphora (@preindustrial_food_catalogue_liquid_amphora) filled with 10 litres of green-grape cook's verjuice", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingFillCatalogueLiquidMedievalFoodLiquidGreenSawse() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/fill catalogue liquid medieval_food_liquid_green_sawse", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:preindustrial_food_catalogue_liquid_amphora"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "fill catalogue liquid medieval_food_liquid_green_sawse",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for medieval_food_liquid_green_sawse.",
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
			new CatalogueLiquidRecipeProduct(ItemStableReference: "preindustrial_food_catalogue_liquid_amphora", Quantity: 1, SkinStableReference: null, Liquid: "medieval green herb sawse", Litres: 10.0) { Compatibility = new("1x a shared catalogue liquid amphora (@preindustrial_food_catalogue_liquid_amphora) filled with 10 litres of medieval green herb sawse", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingFillCatalogueLiquidMedievalFoodLiquidSorrelVerjuiceSauce() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/fill catalogue liquid medieval_food_liquid_sorrel_verjuice_sauce", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:preindustrial_food_catalogue_liquid_amphora"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "fill catalogue liquid medieval_food_liquid_sorrel_verjuice_sauce",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for medieval_food_liquid_sorrel_verjuice_sauce.",
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
			new CatalogueLiquidRecipeProduct(ItemStableReference: "preindustrial_food_catalogue_liquid_amphora", Quantity: 1, SkinStableReference: null, Liquid: "sorrel verjuice sauce", Litres: 10.0) { Compatibility = new("1x a shared catalogue liquid amphora (@preindustrial_food_catalogue_liquid_amphora) filled with 10 litres of sorrel verjuice sauce", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingFillCatalogueLiquidMedievalFoodLiquidSpicedGalantineSauce() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/fill catalogue liquid medieval_food_liquid_spiced_galantine_sauce", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:preindustrial_food_catalogue_liquid_amphora"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "fill catalogue liquid medieval_food_liquid_spiced_galantine_sauce",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for medieval_food_liquid_spiced_galantine_sauce.",
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
			new CatalogueLiquidRecipeProduct(ItemStableReference: "preindustrial_food_catalogue_liquid_amphora", Quantity: 1, SkinStableReference: null, Liquid: "spiced galantine sauce", Litres: 10.0) { Compatibility = new("1x a shared catalogue liquid amphora (@preindustrial_food_catalogue_liquid_amphora) filled with 10 litres of spiced galantine sauce", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingFillCatalogueLiquidMedievalFoodLiquidQuinceHoneySyrup() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/fill catalogue liquid medieval_food_liquid_quince_honey_syrup", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:preindustrial_food_catalogue_liquid_amphora"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "fill catalogue liquid medieval_food_liquid_quince_honey_syrup",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for medieval_food_liquid_quince_honey_syrup.",
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
			new CatalogueLiquidRecipeProduct(ItemStableReference: "preindustrial_food_catalogue_liquid_amphora", Quantity: 1, SkinStableReference: null, Liquid: "quince-and-honey electuary syrup", Litres: 10.0) { Compatibility = new("1x a shared catalogue liquid amphora (@preindustrial_food_catalogue_liquid_amphora) filled with 10 litres of quince-and-honey electuary syrup", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingFillCatalogueLiquidMedievalFoodLiquidSpicedMustReduction() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/fill catalogue liquid medieval_food_liquid_spiced_must_reduction", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:preindustrial_food_catalogue_liquid_amphora"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "fill catalogue liquid medieval_food_liquid_spiced_must_reduction",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for medieval_food_liquid_spiced_must_reduction.",
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
			new CatalogueLiquidRecipeProduct(ItemStableReference: "preindustrial_food_catalogue_liquid_amphora", Quantity: 1, SkinStableReference: null, Liquid: "spiced must reduction", Litres: 10.0) { Compatibility = new("1x a shared catalogue liquid amphora (@preindustrial_food_catalogue_liquid_amphora) filled with 10 litres of spiced must reduction", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingFillCatalogueLiquidMedievalFoodLiquidVioletSugarSyrup() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/fill catalogue liquid medieval_food_liquid_violet_sugar_syrup", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:preindustrial_food_catalogue_liquid_amphora"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "fill catalogue liquid medieval_food_liquid_violet_sugar_syrup",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for medieval_food_liquid_violet_sugar_syrup.",
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
			new CatalogueLiquidRecipeProduct(ItemStableReference: "preindustrial_food_catalogue_liquid_amphora", Quantity: 1, SkinStableReference: null, Liquid: "late-medieval violet syrup", Litres: 10.0) { Compatibility = new("1x a shared catalogue liquid amphora (@preindustrial_food_catalogue_liquid_amphora) filled with 10 litres of late-medieval violet syrup", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingMakeMedievalIntermediateIntermediateFoodCatalogueStock() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/make Medieval Intermediate intermediate food catalogue stock", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make Medieval Intermediate intermediate food catalogue stock",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for medieval_food_blanching_almond_paste.",
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
			new CatalogueProgRecipeProduct(Prog: "ItemSeederPreIndustrialFood_Medieval_Intermediate_Intermediate_medieval_food_almond_milk_press_cake") { Compatibility = new("ItemSeederPreIndustrialFood_Medieval_Intermediate_Intermediate_medieval_food_almond_milk_press_cake", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingMakeMedievalIntermediateIntermediateHoneyCatalogueStock() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/make Medieval Intermediate intermediate honey catalogue stock", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make Medieval Intermediate intermediate honey catalogue stock",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for medieval_food_coloured_sugar_paste_sheet.",
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
			new CatalogueProgRecipeProduct(Prog: "ItemSeederPreIndustrialFood_Medieval_Intermediate_Intermediate_medieval_food_coloured_sugar_paste_sheet") { Compatibility = new("ItemSeederPreIndustrialFood_Medieval_Intermediate_Intermediate_medieval_food_coloured_sugar_paste_sheet", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingMakeMedievalIntermediateIntermediateFishCatalogueStock() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/make Medieval Intermediate intermediate fish catalogue stock", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make Medieval Intermediate intermediate fish catalogue stock",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for medieval_food_filled_unbaked_fish_coffin.",
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
			new CatalogueProgRecipeProduct(Prog: "ItemSeederPreIndustrialFood_Medieval_Intermediate_Intermediate_medieval_food_filled_unbaked_fish_coffin") { Compatibility = new("ItemSeederPreIndustrialFood_Medieval_Intermediate_Intermediate_medieval_food_filled_unbaked_fish_coffin", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingMakeMedievalIntermediateIntermediateRiceCatalogueStock() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/make Medieval Intermediate intermediate rice catalogue stock", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make Medieval Intermediate intermediate rice catalogue stock",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for medieval_food_idli_fermenting_batter_cake.",
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
			new CatalogueProgRecipeProduct(Prog: "ItemSeederPreIndustrialFood_Medieval_Intermediate_Intermediate_medieval_food_dosa_fermenting_batter_crock") { Compatibility = new("ItemSeederPreIndustrialFood_Medieval_Intermediate_Intermediate_medieval_food_dosa_fermenting_batter_crock", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingMakeMedievalIntermediateIntermediateSorghumCatalogueStock() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/make Medieval Intermediate intermediate sorghum catalogue stock", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make Medieval Intermediate intermediate sorghum catalogue stock",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for medieval_food_injera_fermenting_batter.",
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
			new CatalogueProgRecipeProduct(Prog: "ItemSeederPreIndustrialFood_Medieval_Intermediate_Intermediate_medieval_food_injera_fermenting_batter") { Compatibility = new("ItemSeederPreIndustrialFood_Medieval_Intermediate_Intermediate_medieval_food_injera_fermenting_batter", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingMakeMedievalIntermediateIntermediateMeatCatalogueStock() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/make Medieval Intermediate intermediate meat catalogue stock", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make Medieval Intermediate intermediate meat catalogue stock",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for medieval_food_pounded_mortrews_base.",
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
			new CatalogueProgRecipeProduct(Prog: "ItemSeederPreIndustrialFood_Medieval_Intermediate_Intermediate_medieval_food_baozi_uncooked_pleated_buns") { Compatibility = new("ItemSeederPreIndustrialFood_Medieval_Intermediate_Intermediate_medieval_food_baozi_uncooked_pleated_buns", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingMakeMedievalIntermediateIntermediateDoughCatalogueStock() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/make Medieval Intermediate intermediate dough catalogue stock", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make Medieval Intermediate intermediate dough catalogue stock",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for medieval_food_raw_raised_pie_coffin.",
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
			new CatalogueProgRecipeProduct(Prog: "ItemSeederPreIndustrialFood_Medieval_Intermediate_Intermediate_medieval_food_raw_raised_pie_coffin") { Compatibility = new("ItemSeederPreIndustrialFood_Medieval_Intermediate_Intermediate_medieval_food_raw_raised_pie_coffin", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingMakeMedievalIntermediateIntermediateBreadCatalogueStock() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/make Medieval Intermediate intermediate bread catalogue stock", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make Medieval Intermediate intermediate bread catalogue stock",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for medieval_food_subtlety_bread_armature.",
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
			new CatalogueProgRecipeProduct(Prog: "ItemSeederPreIndustrialFood_Medieval_Intermediate_Intermediate_medieval_food_cameline_sauce_spice_cake") { Compatibility = new("ItemSeederPreIndustrialFood_Medieval_Intermediate_Intermediate_medieval_food_cameline_sauce_spice_cake", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingMakeMedievalIntermediateIntermediateWheatCatalogueStock() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/make Medieval Intermediate intermediate wheat catalogue stock", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make Medieval Intermediate intermediate wheat catalogue stock",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for medieval_food_wafer_iron_batter_bowl.",
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
			new CatalogueProgRecipeProduct(Prog: "ItemSeederPreIndustrialFood_Medieval_Intermediate_Intermediate_medieval_food_buddhist_gluten_washed_mass") { Compatibility = new("ItemSeederPreIndustrialFood_Medieval_Intermediate_Intermediate_medieval_food_buddhist_gluten_washed_mass", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingMakeRenaissancePreparedGrainFoodCatalogueStock() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/make Renaissance Prepared grain food catalogue stock", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make Renaissance Prepared grain food catalogue stock",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for renaissance_food_andean_quinoa_like_grain_cake.",
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
			new CatalogueProgCookedRecipeProduct(Prog: "ItemSeederPreIndustrialFood_Renaissance_Prepared_Grain_renaissance_food_andean_quinoa_like_grain_cake", RemoveDrugsAndFoodEffects: false, Ingredients: []) { Compatibility = new("ItemSeederPreIndustrialFood_Renaissance_Prepared_Grain_renaissance_food_andean_quinoa_like_grain_cake", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingMakeRenaissancePreparedGrainRiceCatalogueStock() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/make Renaissance Prepared grain rice catalogue stock", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make Renaissance Prepared grain rice catalogue stock",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for renaissance_food_ottoman_palace_rice_pilav.",
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
			new CatalogueProgCookedRecipeProduct(Prog: "ItemSeederPreIndustrialFood_Renaissance_Prepared_Grain_renaissance_food_japanese_grilled_miso_rice_cake", RemoveDrugsAndFoodEffects: false, Ingredients: []) { Compatibility = new("ItemSeederPreIndustrialFood_Renaissance_Prepared_Grain_renaissance_food_japanese_grilled_miso_rice_cake", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingMakeRenaissancePreparedGrainMilletCatalogueStock() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/make Renaissance Prepared grain millet catalogue stock", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make Renaissance Prepared grain millet catalogue stock",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for renaissance_food_songhai_millet_couscous_feast.",
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
			new CatalogueProgCookedRecipeProduct(Prog: "ItemSeederPreIndustrialFood_Renaissance_Prepared_Grain_renaissance_food_songhai_millet_couscous_feast", RemoveDrugsAndFoodEffects: false, Ingredients: []) { Compatibility = new("ItemSeederPreIndustrialFood_Renaissance_Prepared_Grain_renaissance_food_songhai_millet_couscous_feast", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingMakeRenaissancePreparedGrainCornCatalogueStock() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/make Renaissance Prepared grain corn catalogue stock", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make Renaissance Prepared grain corn catalogue stock",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for renaissance_food_west_african_new_maize_cake.",
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
			new CatalogueProgCookedRecipeProduct(Prog: "ItemSeederPreIndustrialFood_Renaissance_Prepared_Grain_renaissance_food_andean_royal_maize_tamale", RemoveDrugsAndFoodEffects: false, Ingredients: []) { Compatibility = new("ItemSeederPreIndustrialFood_Renaissance_Prepared_Grain_renaissance_food_andean_royal_maize_tamale", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingMakeRenaissancePreparedBreadCornCatalogueStock() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/make Renaissance Prepared bread corn catalogue stock", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make Renaissance Prepared bread corn catalogue stock",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for renaissance_food_aztec_nixtamal_tortilla_stack.",
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
			new CatalogueProgCookedRecipeProduct(Prog: "ItemSeederPreIndustrialFood_Renaissance_Prepared_Bread_renaissance_food_andean_chicha_residue_bread", RemoveDrugsAndFoodEffects: false, Ingredients: []) { Compatibility = new("ItemSeederPreIndustrialFood_Renaissance_Prepared_Bread_renaissance_food_andean_chicha_residue_bread", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingMakeRenaissancePreparedBreadBreadCatalogueStock() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/make Renaissance Prepared bread bread catalogue stock", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make Renaissance Prepared bread bread catalogue stock",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for renaissance_food_ottoman_coffee_house_sesame_ring.",
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
			new CatalogueProgCookedRecipeProduct(Prog: "ItemSeederPreIndustrialFood_Renaissance_Prepared_Bread_renaissance_food_ottoman_coffee_house_sesame_ring", RemoveDrugsAndFoodEffects: false, Ingredients: []) { Compatibility = new("ItemSeederPreIndustrialFood_Renaissance_Prepared_Bread_renaissance_food_ottoman_coffee_house_sesame_ring", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingMakeRenaissancePreparedNoodleWheatCatalogueStock() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/make Renaissance Prepared noodle wheat catalogue stock", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make Renaissance Prepared noodle wheat catalogue stock",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for renaissance_food_italian_buttered_macaroni.",
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
			new CatalogueProgCookedRecipeProduct(Prog: "ItemSeederPreIndustrialFood_Renaissance_Prepared_Noodle_renaissance_food_german_egg_noodle_tangle", RemoveDrugsAndFoodEffects: false, Ingredients: []) { Compatibility = new("ItemSeederPreIndustrialFood_Renaissance_Prepared_Noodle_renaissance_food_german_egg_noodle_tangle", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingMakeRenaissancePreparedDumplingBreadCatalogueStock() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/make Renaissance Prepared dumpling bread catalogue stock", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make Renaissance Prepared dumpling bread catalogue stock",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for renaissance_food_italian_bread_crumb_gnocchi.",
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
			new CatalogueProgCookedRecipeProduct(Prog: "ItemSeederPreIndustrialFood_Renaissance_Prepared_Dumpling_renaissance_food_italian_bread_crumb_gnocchi", RemoveDrugsAndFoodEffects: false, Ingredients: []) { Compatibility = new("ItemSeederPreIndustrialFood_Renaissance_Prepared_Dumpling_renaissance_food_italian_bread_crumb_gnocchi", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingMakeRenaissancePreparedDumplingCheeseCatalogueStock() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/make Renaissance Prepared dumpling cheese catalogue stock", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make Renaissance Prepared dumpling cheese catalogue stock",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for renaissance_food_italian_herb_cheese_ravioli.",
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
			new CatalogueCommodityTagRecipeInput(Grams: 1000.0, MaterialTag: "Raw Milk", PileTag: "Raw Milk", CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1 kilogram of a material tagged as Raw Milk", ["piletag Raw Milk"]), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Cooking", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Cooking tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueProgCookedRecipeProduct(Prog: "ItemSeederPreIndustrialFood_Renaissance_Prepared_Dumpling_renaissance_food_italian_greens_tortelli", RemoveDrugsAndFoodEffects: false, Ingredients: []) { Compatibility = new("ItemSeederPreIndustrialFood_Renaissance_Prepared_Dumpling_renaissance_food_italian_greens_tortelli", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingMakeRenaissancePreparedDumplingMeatCatalogueStock() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/make Renaissance Prepared dumpling meat catalogue stock", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make Renaissance Prepared dumpling meat catalogue stock",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for renaissance_food_italian_meat_ravioli_broth.",
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
			new CatalogueProgCookedRecipeProduct(Prog: "ItemSeederPreIndustrialFood_Renaissance_Prepared_Dumpling_renaissance_food_italian_capon_tortellini", RemoveDrugsAndFoodEffects: false, Ingredients: []) { Compatibility = new("ItemSeederPreIndustrialFood_Renaissance_Prepared_Dumpling_renaissance_food_italian_capon_tortellini", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingMakeRenaissancePreparedDumplingVegetableCatalogueStock() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/make Renaissance Prepared dumpling vegetable catalogue stock", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make Renaissance Prepared dumpling vegetable catalogue stock",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for renaissance_food_italian_pumpkin_tortelli.",
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
			new CatalogueProgCookedRecipeProduct(Prog: "ItemSeederPreIndustrialFood_Renaissance_Prepared_Dumpling_renaissance_food_italian_pumpkin_tortelli", RemoveDrugsAndFoodEffects: false, Ingredients: []) { Compatibility = new("ItemSeederPreIndustrialFood_Renaissance_Prepared_Dumpling_renaissance_food_italian_pumpkin_tortelli", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingMakeRenaissancePreparedDumplingRiceCatalogueStock() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/make Renaissance Prepared dumpling rice catalogue stock", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make Renaissance Prepared dumpling rice catalogue stock",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for renaissance_food_italian_rice_flour_gnocchi.",
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
			new CatalogueProgCookedRecipeProduct(Prog: "ItemSeederPreIndustrialFood_Renaissance_Prepared_Dumpling_renaissance_food_italian_rice_flour_gnocchi", RemoveDrugsAndFoodEffects: false, Ingredients: []) { Compatibility = new("ItemSeederPreIndustrialFood_Renaissance_Prepared_Dumpling_renaissance_food_italian_rice_flour_gnocchi", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingMakeRenaissancePreparedPulseFoodCatalogueStock() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/make Renaissance Prepared pulse food catalogue stock", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make Renaissance Prepared pulse food catalogue stock",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for renaissance_food_benin_bean_leaf_pudding.",
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
			new CatalogueProgCookedRecipeProduct(Prog: "ItemSeederPreIndustrialFood_Renaissance_Prepared_Pulse_renaissance_food_benin_bean_leaf_pudding", RemoveDrugsAndFoodEffects: false, Ingredients: []) { Compatibility = new("ItemSeederPreIndustrialFood_Renaissance_Prepared_Pulse_renaissance_food_benin_bean_leaf_pudding", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingMakeRenaissancePreparedVegetableCornCatalogueStock() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/make Renaissance Prepared vegetable corn catalogue stock", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make Renaissance Prepared vegetable corn catalogue stock",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for renaissance_food_aztec_avocado_maize_roll.",
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
			new CatalogueProgCookedRecipeProduct(Prog: "ItemSeederPreIndustrialFood_Renaissance_Prepared_Vegetable_renaissance_food_aztec_avocado_maize_roll", RemoveDrugsAndFoodEffects: false, Ingredients: []) { Compatibility = new("ItemSeederPreIndustrialFood_Renaissance_Prepared_Vegetable_renaissance_food_aztec_avocado_maize_roll", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingMakeRenaissancePreparedVegetableVegetableCatalogueStock() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/make Renaissance Prepared vegetable vegetable catalogue stock", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make Renaissance Prepared vegetable vegetable catalogue stock",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for renaissance_food_italian_artichoke_torta.",
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
			new CatalogueProgCookedRecipeProduct(Prog: "ItemSeederPreIndustrialFood_Renaissance_Prepared_Vegetable_renaissance_food_andean_freeze_dried_tuber_pudding", RemoveDrugsAndFoodEffects: false, Ingredients: []) { Compatibility = new("ItemSeederPreIndustrialFood_Renaissance_Prepared_Vegetable_renaissance_food_andean_freeze_dried_tuber_pudding", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingMakeRenaissancePreparedVegetableRiceCatalogueStock() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/make Renaissance Prepared vegetable rice catalogue stock", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make Renaissance Prepared vegetable rice catalogue stock",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for renaissance_food_ottoman_vine_leaf_sarma.",
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
			new CatalogueProgCookedRecipeProduct(Prog: "ItemSeederPreIndustrialFood_Renaissance_Prepared_Vegetable_renaissance_food_ottoman_vine_leaf_sarma", RemoveDrugsAndFoodEffects: false, Ingredients: []) { Compatibility = new("ItemSeederPreIndustrialFood_Renaissance_Prepared_Vegetable_renaissance_food_ottoman_vine_leaf_sarma", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingMakeRenaissancePreparedSoupMeatCatalogueStock() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/make Renaissance Prepared soup meat catalogue stock", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make Renaissance Prepared soup meat catalogue stock",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for renaissance_food_benin_palace_pepper_soup.",
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
			new CatalogueProgCookedRecipeProduct(Prog: "ItemSeederPreIndustrialFood_Renaissance_Prepared_Soup_renaissance_food_benin_palace_pepper_soup", RemoveDrugsAndFoodEffects: false, Ingredients: []) { Compatibility = new("ItemSeederPreIndustrialFood_Renaissance_Prepared_Soup_renaissance_food_benin_palace_pepper_soup", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingMakeRenaissancePreparedStewCornCatalogueStock() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/make Renaissance Prepared stew corn catalogue stock", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make Renaissance Prepared stew corn catalogue stock",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for renaissance_food_aztec_hominy_pozole_feast.",
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
			new CatalogueProgCookedRecipeProduct(Prog: "ItemSeederPreIndustrialFood_Renaissance_Prepared_Stew_renaissance_food_aztec_hominy_pozole_feast", RemoveDrugsAndFoodEffects: false, Ingredients: []) { Compatibility = new("ItemSeederPreIndustrialFood_Renaissance_Prepared_Stew_renaissance_food_aztec_hominy_pozole_feast", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingMakeRenaissancePreparedStewMeatCatalogueStock() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/make Renaissance Prepared stew meat catalogue stock", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make Renaissance Prepared stew meat catalogue stock",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for renaissance_food_iberian_olla_podrida_feast.",
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
			new CatalogueProgCookedRecipeProduct(Prog: "ItemSeederPreIndustrialFood_Renaissance_Prepared_Stew_renaissance_food_andean_chuno_meat_stew", RemoveDrugsAndFoodEffects: false, Ingredients: []) { Compatibility = new("ItemSeederPreIndustrialFood_Renaissance_Prepared_Stew_renaissance_food_andean_chuno_meat_stew", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingMakeRenaissancePreparedStewFoodCatalogueStock() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/make Renaissance Prepared stew food catalogue stock", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make Renaissance Prepared stew food catalogue stock",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for renaissance_food_joseon_royal_sinseollo.",
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
			new CatalogueProgCookedRecipeProduct(Prog: "ItemSeederPreIndustrialFood_Renaissance_Prepared_Stew_renaissance_food_joseon_royal_sinseollo", RemoveDrugsAndFoodEffects: false, Ingredients: []) { Compatibility = new("ItemSeederPreIndustrialFood_Renaissance_Prepared_Stew_renaissance_food_joseon_royal_sinseollo", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingMakeRenaissancePreparedMeatFoodCatalogueStock() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/make Renaissance Prepared meat food catalogue stock", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make Renaissance Prepared meat food catalogue stock",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for renaissance_food_aztec_market_insect_seed_cake.",
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
			new CatalogueProgCookedRecipeProduct(Prog: "ItemSeederPreIndustrialFood_Renaissance_Prepared_Meat_renaissance_food_aztec_market_insect_seed_cake", RemoveDrugsAndFoodEffects: false, Ingredients: []) { Compatibility = new("ItemSeederPreIndustrialFood_Renaissance_Prepared_Meat_renaissance_food_aztec_market_insect_seed_cake", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingMakeRenaissancePreparedMeatMeatCatalogueStock() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/make Renaissance Prepared meat meat catalogue stock", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make Renaissance Prepared meat meat catalogue stock",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for renaissance_food_italian_sculpted_pastry_pie.",
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
			new CatalogueProgCookedRecipeProduct(Prog: "ItemSeederPreIndustrialFood_Renaissance_Prepared_Meat_renaissance_food_andean_guinea_pig_pachamanca", RemoveDrugsAndFoodEffects: false, Ingredients: []) { Compatibility = new("ItemSeederPreIndustrialFood_Renaissance_Prepared_Meat_renaissance_food_andean_guinea_pig_pachamanca", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingMakeRenaissancePreparedPoultryMeatCatalogueStock() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/make Renaissance Prepared poultry meat catalogue stock", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make Renaissance Prepared poultry meat catalogue stock",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for renaissance_food_italian_capon_pomegranate_torta.",
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
			new CatalogueProgCookedRecipeProduct(Prog: "ItemSeederPreIndustrialFood_Renaissance_Prepared_Poultry_renaissance_food_atlantic_ship_turkey_pastie", RemoveDrugsAndFoodEffects: false, Ingredients: []) { Compatibility = new("ItemSeederPreIndustrialFood_Renaissance_Prepared_Poultry_renaissance_food_atlantic_ship_turkey_pastie", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingMakeRenaissancePreparedOffalMeatCatalogueStock() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/make Renaissance Prepared offal meat catalogue stock", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make Renaissance Prepared offal meat catalogue stock",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for renaissance_food_italian_fried_liver_croquettes.",
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
			new CatalogueProgCookedRecipeProduct(Prog: "ItemSeederPreIndustrialFood_Renaissance_Prepared_Offal_renaissance_food_italian_fried_liver_croquettes", RemoveDrugsAndFoodEffects: false, Ingredients: []) { Compatibility = new("ItemSeederPreIndustrialFood_Renaissance_Prepared_Offal_renaissance_food_italian_fried_liver_croquettes", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingMakeRenaissancePreparedFishFishCatalogueStock() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/make Renaissance Prepared fish fish catalogue stock", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make Renaissance Prepared fish fish catalogue stock",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for renaissance_food_italian_fish_jelly_medallions.",
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
			new CatalogueProgCookedRecipeProduct(Prog: "ItemSeederPreIndustrialFood_Renaissance_Prepared_Fish_renaissance_food_andean_sun_dried_fish_maize_cake", RemoveDrugsAndFoodEffects: false, Ingredients: []) { Compatibility = new("ItemSeederPreIndustrialFood_Renaissance_Prepared_Fish_renaissance_food_andean_sun_dried_fish_maize_cake", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingMakeRenaissancePreparedShellfishFishCatalogueStock() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/make Renaissance Prepared shellfish fish catalogue stock", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make Renaissance Prepared shellfish fish catalogue stock",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for renaissance_food_iberian_escabeche_shellfish.",
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
			new CatalogueProgCookedRecipeProduct(Prog: "ItemSeederPreIndustrialFood_Renaissance_Prepared_Shellfish_renaissance_food_iberian_escabeche_shellfish", RemoveDrugsAndFoodEffects: false, Ingredients: []) { Compatibility = new("ItemSeederPreIndustrialFood_Renaissance_Prepared_Shellfish_renaissance_food_iberian_escabeche_shellfish", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingMakeRenaissancePreparedDairyCheeseCatalogueStock() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/make Renaissance Prepared dairy cheese catalogue stock", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make Renaissance Prepared dairy cheese catalogue stock",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for renaissance_food_italian_spinach_ricotta_torta.",
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
			new CatalogueProgCookedRecipeProduct(Prog: "ItemSeederPreIndustrialFood_Renaissance_Prepared_Dairy_renaissance_food_italian_spinach_ricotta_torta", RemoveDrugsAndFoodEffects: false, Ingredients: []) { Compatibility = new("ItemSeederPreIndustrialFood_Renaissance_Prepared_Dairy_renaissance_food_italian_spinach_ricotta_torta", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingMakeRenaissancePreparedPreservedVegetableCatalogueStock() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/make Renaissance Prepared preserved vegetable catalogue stock", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make Renaissance Prepared preserved vegetable catalogue stock",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for renaissance_food_japanese_miso_pickled_tofu.",
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
			new CatalogueProgCookedRecipeProduct(Prog: "ItemSeederPreIndustrialFood_Renaissance_Prepared_Preserved_renaissance_food_andean_freeze_dried_potato_cake", RemoveDrugsAndFoodEffects: false, Ingredients: []) { Compatibility = new("ItemSeederPreIndustrialFood_Renaissance_Prepared_Preserved_renaissance_food_andean_freeze_dried_potato_cake", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingMakeRenaissancePreparedPreservedMeatCatalogueStock() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/make Renaissance Prepared preserved meat catalogue stock", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make Renaissance Prepared preserved meat catalogue stock",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for renaissance_food_songhai_cavalry_meat_grain_cake.",
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
			new CatalogueProgCookedRecipeProduct(Prog: "ItemSeederPreIndustrialFood_Renaissance_Prepared_Preserved_renaissance_food_andean_army_charqui_ration", RemoveDrugsAndFoodEffects: false, Ingredients: []) { Compatibility = new("ItemSeederPreIndustrialFood_Renaissance_Prepared_Preserved_renaissance_food_andean_army_charqui_ration", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};
}
