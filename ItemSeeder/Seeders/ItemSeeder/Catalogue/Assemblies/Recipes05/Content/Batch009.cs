#nullable enable

using MudSharp.Body;
using MudSharp.GameItems.Inventory.Plans;
using MudSharp.RPG.Checks;

namespace DatabaseSeeder.Seeders;

public static partial class Recipes05Stock
{
	public static CatalogueRecipeDefinition PreIndustrialFoodmakingFillCatalogueLiquidPreindustrialFoodPressedAppleMust() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/fill catalogue liquid preindustrial_food_pressed_apple_must", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:preindustrial_food_catalogue_liquid_amphora"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["medieval", "renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "fill catalogue liquid preindustrial_food_pressed_apple_must",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for preindustrial_food_pressed_apple_must.",
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
			new CatalogueLiquidRecipeProduct(ItemStableReference: "preindustrial_food_catalogue_liquid_amphora", Quantity: 1, SkinStableReference: null, Liquid: "pressed apple must", Litres: 10.0) { Compatibility = new("1x a shared catalogue liquid amphora (@preindustrial_food_catalogue_liquid_amphora) filled with 10 litres of pressed apple must", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingFillCatalogueLiquidPreindustrialFoodPressedGrapeMust() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/fill catalogue liquid preindustrial_food_pressed_grape_must", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:preindustrial_food_catalogue_liquid_amphora"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["medieval", "renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "fill catalogue liquid preindustrial_food_pressed_grape_must",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for preindustrial_food_pressed_grape_must.",
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
			new CatalogueLiquidRecipeProduct(ItemStableReference: "preindustrial_food_catalogue_liquid_amphora", Quantity: 1, SkinStableReference: null, Liquid: "pressed grape must", Litres: 10.0) { Compatibility = new("1x a shared catalogue liquid amphora (@preindustrial_food_catalogue_liquid_amphora) filled with 10 litres of pressed grape must", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingFillCatalogueLiquidPreindustrialFoodPressedPearMust() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/fill catalogue liquid preindustrial_food_pressed_pear_must", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:preindustrial_food_catalogue_liquid_amphora"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["medieval", "renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "fill catalogue liquid preindustrial_food_pressed_pear_must",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for preindustrial_food_pressed_pear_must.",
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
			new CatalogueLiquidRecipeProduct(ItemStableReference: "preindustrial_food_catalogue_liquid_amphora", Quantity: 1, SkinStableReference: null, Liquid: "pressed pear must", Litres: 10.0) { Compatibility = new("1x a shared catalogue liquid amphora (@preindustrial_food_catalogue_liquid_amphora) filled with 10 litres of pressed pear must", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingFillCatalogueLiquidPreindustrialFoodTamarindWaterDrink() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/fill catalogue liquid preindustrial_food_tamarind_water_drink", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:preindustrial_food_catalogue_liquid_amphora"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["medieval", "renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "fill catalogue liquid preindustrial_food_tamarind_water_drink",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for preindustrial_food_tamarind_water_drink.",
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
			new CatalogueLiquidRecipeProduct(ItemStableReference: "preindustrial_food_catalogue_liquid_amphora", Quantity: 1, SkinStableReference: null, Liquid: "tamarind water drink", Litres: 10.0) { Compatibility = new("1x a shared catalogue liquid amphora (@preindustrial_food_catalogue_liquid_amphora) filled with 10 litres of tamarind water drink", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingFillCatalogueLiquidPreindustrialFoodTartCitrusPressing() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/fill catalogue liquid preindustrial_food_tart_citrus_pressing", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:preindustrial_food_catalogue_liquid_amphora"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["medieval", "renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "fill catalogue liquid preindustrial_food_tart_citrus_pressing",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for preindustrial_food_tart_citrus_pressing.",
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
			new CatalogueLiquidRecipeProduct(ItemStableReference: "preindustrial_food_catalogue_liquid_amphora", Quantity: 1, SkinStableReference: null, Liquid: "tart citrus pressing", Litres: 10.0) { Compatibility = new("1x a shared catalogue liquid amphora (@preindustrial_food_catalogue_liquid_amphora) filled with 10 litres of tart citrus pressing", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingFillCatalogueLiquidPreindustrialFoodBarleyWaterDrink() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/fill catalogue liquid preindustrial_food_barley_water_drink", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:preindustrial_food_catalogue_liquid_amphora"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["medieval", "renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "fill catalogue liquid preindustrial_food_barley_water_drink",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for preindustrial_food_barley_water_drink.",
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
			new CatalogueLiquidRecipeProduct(ItemStableReference: "preindustrial_food_catalogue_liquid_amphora", Quantity: 1, SkinStableReference: null, Liquid: "barley water drink", Litres: 10.0) { Compatibility = new("1x a shared catalogue liquid amphora (@preindustrial_food_catalogue_liquid_amphora) filled with 10 litres of barley water drink", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingFillCatalogueLiquidPreindustrialFoodHoneyedGrainWater() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/fill catalogue liquid preindustrial_food_honeyed_grain_water", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:preindustrial_food_catalogue_liquid_amphora"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["medieval", "renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "fill catalogue liquid preindustrial_food_honeyed_grain_water",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for preindustrial_food_honeyed_grain_water.",
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
			new CatalogueLiquidRecipeProduct(ItemStableReference: "preindustrial_food_catalogue_liquid_amphora", Quantity: 1, SkinStableReference: null, Liquid: "honeyed grain water", Litres: 10.0) { Compatibility = new("1x a shared catalogue liquid amphora (@preindustrial_food_catalogue_liquid_amphora) filled with 10 litres of honeyed grain water", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingFillCatalogueLiquidPreindustrialFoodMaizeWaterDrink() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/fill catalogue liquid preindustrial_food_maize_water_drink", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:preindustrial_food_catalogue_liquid_amphora"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["medieval", "renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "fill catalogue liquid preindustrial_food_maize_water_drink",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for preindustrial_food_maize_water_drink.",
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
			new CatalogueLiquidRecipeProduct(ItemStableReference: "preindustrial_food_catalogue_liquid_amphora", Quantity: 1, SkinStableReference: null, Liquid: "maize water drink", Litres: 10.0) { Compatibility = new("1x a shared catalogue liquid amphora (@preindustrial_food_catalogue_liquid_amphora) filled with 10 litres of maize water drink", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingFillCatalogueLiquidPreindustrialFoodMilletWaterDrink() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/fill catalogue liquid preindustrial_food_millet_water_drink", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:preindustrial_food_catalogue_liquid_amphora"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["medieval", "renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "fill catalogue liquid preindustrial_food_millet_water_drink",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for preindustrial_food_millet_water_drink.",
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
			new CatalogueLiquidRecipeProduct(ItemStableReference: "preindustrial_food_catalogue_liquid_amphora", Quantity: 1, SkinStableReference: null, Liquid: "millet water drink", Litres: 10.0) { Compatibility = new("1x a shared catalogue liquid amphora (@preindustrial_food_catalogue_liquid_amphora) filled with 10 litres of millet water drink", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingFillCatalogueLiquidPreindustrialFoodRiceWaterDrink() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/fill catalogue liquid preindustrial_food_rice_water_drink", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:preindustrial_food_catalogue_liquid_amphora"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["medieval", "renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "fill catalogue liquid preindustrial_food_rice_water_drink",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for preindustrial_food_rice_water_drink.",
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
			new CatalogueLiquidRecipeProduct(ItemStableReference: "preindustrial_food_catalogue_liquid_amphora", Quantity: 1, SkinStableReference: null, Liquid: "rice water drink", Litres: 10.0) { Compatibility = new("1x a shared catalogue liquid amphora (@preindustrial_food_catalogue_liquid_amphora) filled with 10 litres of rice water drink", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingFillCatalogueLiquidPreindustrialFoodRyeWaterDrink() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/fill catalogue liquid preindustrial_food_rye_water_drink", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:preindustrial_food_catalogue_liquid_amphora"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["medieval", "renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "fill catalogue liquid preindustrial_food_rye_water_drink",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for preindustrial_food_rye_water_drink.",
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
			new CatalogueLiquidRecipeProduct(ItemStableReference: "preindustrial_food_catalogue_liquid_amphora", Quantity: 1, SkinStableReference: null, Liquid: "rye water drink", Litres: 10.0) { Compatibility = new("1x a shared catalogue liquid amphora (@preindustrial_food_catalogue_liquid_amphora) filled with 10 litres of rye water drink", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingFillCatalogueLiquidPreindustrialFoodSorghumWaterDrink() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/fill catalogue liquid preindustrial_food_sorghum_water_drink", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:preindustrial_food_catalogue_liquid_amphora"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["medieval", "renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "fill catalogue liquid preindustrial_food_sorghum_water_drink",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for preindustrial_food_sorghum_water_drink.",
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
			new CatalogueLiquidRecipeProduct(ItemStableReference: "preindustrial_food_catalogue_liquid_amphora", Quantity: 1, SkinStableReference: null, Liquid: "sorghum water drink", Litres: 10.0) { Compatibility = new("1x a shared catalogue liquid amphora (@preindustrial_food_catalogue_liquid_amphora) filled with 10 litres of sorghum water drink", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingFillCatalogueLiquidPreindustrialFoodSpicedGrainWater() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/fill catalogue liquid preindustrial_food_spiced_grain_water", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:preindustrial_food_catalogue_liquid_amphora"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["medieval", "renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "fill catalogue liquid preindustrial_food_spiced_grain_water",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for preindustrial_food_spiced_grain_water.",
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
			new CatalogueLiquidRecipeProduct(ItemStableReference: "preindustrial_food_catalogue_liquid_amphora", Quantity: 1, SkinStableReference: null, Liquid: "spiced grain water", Litres: 10.0) { Compatibility = new("1x a shared catalogue liquid amphora (@preindustrial_food_catalogue_liquid_amphora) filled with 10 litres of spiced grain water", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingFillCatalogueLiquidPreindustrialFoodSproutedGrainDrink() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/fill catalogue liquid preindustrial_food_sprouted_grain_drink", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:preindustrial_food_catalogue_liquid_amphora"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["medieval", "renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "fill catalogue liquid preindustrial_food_sprouted_grain_drink",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for preindustrial_food_sprouted_grain_drink.",
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
			new CatalogueLiquidRecipeProduct(ItemStableReference: "preindustrial_food_catalogue_liquid_amphora", Quantity: 1, SkinStableReference: null, Liquid: "sprouted grain drink", Litres: 10.0) { Compatibility = new("1x a shared catalogue liquid amphora (@preindustrial_food_catalogue_liquid_amphora) filled with 10 litres of sprouted grain drink", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingFillCatalogueLiquidPreindustrialFoodToastedBarleyDrink() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/fill catalogue liquid preindustrial_food_toasted_barley_drink", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:preindustrial_food_catalogue_liquid_amphora"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["medieval", "renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "fill catalogue liquid preindustrial_food_toasted_barley_drink",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for preindustrial_food_toasted_barley_drink.",
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
			new CatalogueLiquidRecipeProduct(ItemStableReference: "preindustrial_food_catalogue_liquid_amphora", Quantity: 1, SkinStableReference: null, Liquid: "toasted barley drink", Litres: 10.0) { Compatibility = new("1x a shared catalogue liquid amphora (@preindustrial_food_catalogue_liquid_amphora) filled with 10 litres of toasted barley drink", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingFillCatalogueLiquidPreindustrialFoodToastedRiceDrink() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/fill catalogue liquid preindustrial_food_toasted_rice_drink", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:preindustrial_food_catalogue_liquid_amphora"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["medieval", "renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "fill catalogue liquid preindustrial_food_toasted_rice_drink",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for preindustrial_food_toasted_rice_drink.",
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
			new CatalogueLiquidRecipeProduct(ItemStableReference: "preindustrial_food_catalogue_liquid_amphora", Quantity: 1, SkinStableReference: null, Liquid: "toasted rice drink", Litres: 10.0) { Compatibility = new("1x a shared catalogue liquid amphora (@preindustrial_food_catalogue_liquid_amphora) filled with 10 litres of toasted rice drink", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingFillCatalogueLiquidPreindustrialFoodWheatWaterDrink() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/fill catalogue liquid preindustrial_food_wheat_water_drink", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:preindustrial_food_catalogue_liquid_amphora"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["medieval", "renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "fill catalogue liquid preindustrial_food_wheat_water_drink",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for preindustrial_food_wheat_water_drink.",
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
			new CatalogueLiquidRecipeProduct(ItemStableReference: "preindustrial_food_catalogue_liquid_amphora", Quantity: 1, SkinStableReference: null, Liquid: "wheat water drink", Litres: 10.0) { Compatibility = new("1x a shared catalogue liquid amphora (@preindustrial_food_catalogue_liquid_amphora) filled with 10 litres of wheat water drink", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingFillCatalogueLiquidPreindustrialFoodAppleFerment() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/fill catalogue liquid preindustrial_food_apple_ferment", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:preindustrial_food_catalogue_liquid_amphora"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["medieval", "renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "fill catalogue liquid preindustrial_food_apple_ferment",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for preindustrial_food_apple_ferment.",
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
			new CatalogueLiquidRecipeProduct(ItemStableReference: "preindustrial_food_catalogue_liquid_amphora", Quantity: 1, SkinStableReference: null, Liquid: "fermented apple drink", Litres: 10.0) { Compatibility = new("1x a shared catalogue liquid amphora (@preindustrial_food_catalogue_liquid_amphora) filled with 10 litres of fermented apple drink", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingFillCatalogueLiquidPreindustrialFoodBerryFerment() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/fill catalogue liquid preindustrial_food_berry_ferment", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:preindustrial_food_catalogue_liquid_amphora"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["medieval", "renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "fill catalogue liquid preindustrial_food_berry_ferment",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for preindustrial_food_berry_ferment.",
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
			new CatalogueLiquidRecipeProduct(ItemStableReference: "preindustrial_food_catalogue_liquid_amphora", Quantity: 1, SkinStableReference: null, Liquid: "fermented berry drink", Litres: 10.0) { Compatibility = new("1x a shared catalogue liquid amphora (@preindustrial_food_catalogue_liquid_amphora) filled with 10 litres of fermented berry drink", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingFillCatalogueLiquidPreindustrialFoodBreadFerment() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/fill catalogue liquid preindustrial_food_bread_ferment", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:preindustrial_food_catalogue_liquid_amphora"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["medieval", "renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "fill catalogue liquid preindustrial_food_bread_ferment",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for preindustrial_food_bread_ferment.",
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
			new CatalogueLiquidRecipeProduct(ItemStableReference: "preindustrial_food_catalogue_liquid_amphora", Quantity: 1, SkinStableReference: null, Liquid: "fermented bread drink", Litres: 10.0) { Compatibility = new("1x a shared catalogue liquid amphora (@preindustrial_food_catalogue_liquid_amphora) filled with 10 litres of fermented bread drink", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingFillCatalogueLiquidPreindustrialFoodFruitHoneyFerment() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/fill catalogue liquid preindustrial_food_fruit_honey_ferment", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:preindustrial_food_catalogue_liquid_amphora"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["medieval", "renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "fill catalogue liquid preindustrial_food_fruit_honey_ferment",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for preindustrial_food_fruit_honey_ferment.",
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
			new CatalogueLiquidRecipeProduct(ItemStableReference: "preindustrial_food_catalogue_liquid_amphora", Quantity: 1, SkinStableReference: null, Liquid: "fruit-honey ferment", Litres: 10.0) { Compatibility = new("1x a shared catalogue liquid amphora (@preindustrial_food_catalogue_liquid_amphora) filled with 10 litres of fruit-honey ferment", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingFillCatalogueLiquidPreindustrialFoodHerbFerment() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/fill catalogue liquid preindustrial_food_herb_ferment", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:preindustrial_food_catalogue_liquid_amphora"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["medieval", "renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "fill catalogue liquid preindustrial_food_herb_ferment",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for preindustrial_food_herb_ferment.",
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
			new CatalogueLiquidRecipeProduct(ItemStableReference: "preindustrial_food_catalogue_liquid_amphora", Quantity: 1, SkinStableReference: null, Liquid: "fermented herb drink", Litres: 10.0) { Compatibility = new("1x a shared catalogue liquid amphora (@preindustrial_food_catalogue_liquid_amphora) filled with 10 litres of fermented herb drink", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingFillCatalogueLiquidPreindustrialFoodHoneyFerment() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/fill catalogue liquid preindustrial_food_honey_ferment", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:preindustrial_food_catalogue_liquid_amphora"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["medieval", "renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "fill catalogue liquid preindustrial_food_honey_ferment",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for preindustrial_food_honey_ferment.",
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
			new CatalogueLiquidRecipeProduct(ItemStableReference: "preindustrial_food_catalogue_liquid_amphora", Quantity: 1, SkinStableReference: null, Liquid: "honey ferment", Litres: 10.0) { Compatibility = new("1x a shared catalogue liquid amphora (@preindustrial_food_catalogue_liquid_amphora) filled with 10 litres of honey ferment", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingFillCatalogueLiquidPreindustrialFoodJuniperSmokedSmallBeer() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/fill catalogue liquid preindustrial_food_juniper_smoked_small_beer", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:preindustrial_food_catalogue_liquid_amphora"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["medieval", "renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "fill catalogue liquid preindustrial_food_juniper_smoked_small_beer",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for preindustrial_food_juniper_smoked_small_beer.",
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
			new CatalogueLiquidRecipeProduct(ItemStableReference: "preindustrial_food_catalogue_liquid_amphora", Quantity: 1, SkinStableReference: null, Liquid: "juniper-smoked small beer", Litres: 10.0) { Compatibility = new("1x a shared catalogue liquid amphora (@preindustrial_food_catalogue_liquid_amphora) filled with 10 litres of juniper-smoked small beer", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingFillCatalogueLiquidPreindustrialFoodLightHoneyFerment() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/fill catalogue liquid preindustrial_food_light_honey_ferment", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:preindustrial_food_catalogue_liquid_amphora"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["medieval", "renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "fill catalogue liquid preindustrial_food_light_honey_ferment",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for preindustrial_food_light_honey_ferment.",
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
			new CatalogueLiquidRecipeProduct(ItemStableReference: "preindustrial_food_catalogue_liquid_amphora", Quantity: 1, SkinStableReference: null, Liquid: "light honey ferment", Litres: 10.0) { Compatibility = new("1x a shared catalogue liquid amphora (@preindustrial_food_catalogue_liquid_amphora) filled with 10 litres of light honey ferment", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingFillCatalogueLiquidPreindustrialFoodMaizeBeer() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/fill catalogue liquid preindustrial_food_maize_beer", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:preindustrial_food_catalogue_liquid_amphora"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["medieval", "renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "fill catalogue liquid preindustrial_food_maize_beer",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for preindustrial_food_maize_beer.",
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
			new CatalogueLiquidRecipeProduct(ItemStableReference: "preindustrial_food_catalogue_liquid_amphora", Quantity: 1, SkinStableReference: null, Liquid: "maize beer", Litres: 10.0) { Compatibility = new("1x a shared catalogue liquid amphora (@preindustrial_food_catalogue_liquid_amphora) filled with 10 litres of maize beer", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingFillCatalogueLiquidPreindustrialFoodMilletBeer() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/fill catalogue liquid preindustrial_food_millet_beer", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:preindustrial_food_catalogue_liquid_amphora"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["medieval", "renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "fill catalogue liquid preindustrial_food_millet_beer",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for preindustrial_food_millet_beer.",
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
			new CatalogueLiquidRecipeProduct(ItemStableReference: "preindustrial_food_catalogue_liquid_amphora", Quantity: 1, SkinStableReference: null, Liquid: "millet beer", Litres: 10.0) { Compatibility = new("1x a shared catalogue liquid amphora (@preindustrial_food_catalogue_liquid_amphora) filled with 10 litres of millet beer", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingFillCatalogueLiquidPreindustrialFoodPearFerment() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/fill catalogue liquid preindustrial_food_pear_ferment", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:preindustrial_food_catalogue_liquid_amphora"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["medieval", "renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "fill catalogue liquid preindustrial_food_pear_ferment",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for preindustrial_food_pear_ferment.",
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
			new CatalogueLiquidRecipeProduct(ItemStableReference: "preindustrial_food_catalogue_liquid_amphora", Quantity: 1, SkinStableReference: null, Liquid: "fermented pear drink", Litres: 10.0) { Compatibility = new("1x a shared catalogue liquid amphora (@preindustrial_food_catalogue_liquid_amphora) filled with 10 litres of fermented pear drink", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingFillCatalogueLiquidPreindustrialFoodPlumFerment() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/fill catalogue liquid preindustrial_food_plum_ferment", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:preindustrial_food_catalogue_liquid_amphora"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["medieval", "renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "fill catalogue liquid preindustrial_food_plum_ferment",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for preindustrial_food_plum_ferment.",
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
			new CatalogueLiquidRecipeProduct(ItemStableReference: "preindustrial_food_catalogue_liquid_amphora", Quantity: 1, SkinStableReference: null, Liquid: "fermented plum drink", Litres: 10.0) { Compatibility = new("1x a shared catalogue liquid amphora (@preindustrial_food_catalogue_liquid_amphora) filled with 10 litres of fermented plum drink", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingFillCatalogueLiquidPreindustrialFoodRiceBeer() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/fill catalogue liquid preindustrial_food_rice_beer", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:preindustrial_food_catalogue_liquid_amphora"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["medieval", "renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "fill catalogue liquid preindustrial_food_rice_beer",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for preindustrial_food_rice_beer.",
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
			new CatalogueLiquidRecipeProduct(ItemStableReference: "preindustrial_food_catalogue_liquid_amphora", Quantity: 1, SkinStableReference: null, Liquid: "rice beer", Litres: 10.0) { Compatibility = new("1x a shared catalogue liquid amphora (@preindustrial_food_catalogue_liquid_amphora) filled with 10 litres of rice beer", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingFillCatalogueLiquidPreindustrialFoodRyeBeer() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/fill catalogue liquid preindustrial_food_rye_beer", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:preindustrial_food_catalogue_liquid_amphora"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["medieval", "renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "fill catalogue liquid preindustrial_food_rye_beer",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for preindustrial_food_rye_beer.",
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
			new CatalogueLiquidRecipeProduct(ItemStableReference: "preindustrial_food_catalogue_liquid_amphora", Quantity: 1, SkinStableReference: null, Liquid: "rye beer", Litres: 10.0) { Compatibility = new("1x a shared catalogue liquid amphora (@preindustrial_food_catalogue_liquid_amphora) filled with 10 litres of rye beer", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingFillCatalogueLiquidPreindustrialFoodSmallGrainBeer() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/fill catalogue liquid preindustrial_food_small_grain_beer", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:preindustrial_food_catalogue_liquid_amphora"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["medieval", "renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "fill catalogue liquid preindustrial_food_small_grain_beer",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for preindustrial_food_small_grain_beer.",
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
			new CatalogueLiquidRecipeProduct(ItemStableReference: "preindustrial_food_catalogue_liquid_amphora", Quantity: 1, SkinStableReference: null, Liquid: "small grain beer", Litres: 10.0) { Compatibility = new("1x a shared catalogue liquid amphora (@preindustrial_food_catalogue_liquid_amphora) filled with 10 litres of small grain beer", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingFillCatalogueLiquidPreindustrialFoodSmokedMaltStrongBeer() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/fill catalogue liquid preindustrial_food_smoked_malt_strong_beer", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:preindustrial_food_catalogue_liquid_amphora"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["medieval", "renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "fill catalogue liquid preindustrial_food_smoked_malt_strong_beer",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for preindustrial_food_smoked_malt_strong_beer.",
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
			new CatalogueLiquidRecipeProduct(ItemStableReference: "preindustrial_food_catalogue_liquid_amphora", Quantity: 1, SkinStableReference: null, Liquid: "smoked-malt strong beer", Litres: 10.0) { Compatibility = new("1x a shared catalogue liquid amphora (@preindustrial_food_catalogue_liquid_amphora) filled with 10 litres of smoked-malt strong beer", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingFillCatalogueLiquidPreindustrialFoodSorghumBeer() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/fill catalogue liquid preindustrial_food_sorghum_beer", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:preindustrial_food_catalogue_liquid_amphora"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["medieval", "renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "fill catalogue liquid preindustrial_food_sorghum_beer",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for preindustrial_food_sorghum_beer.",
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
			new CatalogueLiquidRecipeProduct(ItemStableReference: "preindustrial_food_catalogue_liquid_amphora", Quantity: 1, SkinStableReference: null, Liquid: "sorghum beer", Litres: 10.0) { Compatibility = new("1x a shared catalogue liquid amphora (@preindustrial_food_catalogue_liquid_amphora) filled with 10 litres of sorghum beer", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingFillCatalogueLiquidPreindustrialFoodSpicedHoneyFerment() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/fill catalogue liquid preindustrial_food_spiced_honey_ferment", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:preindustrial_food_catalogue_liquid_amphora"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["medieval", "renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "fill catalogue liquid preindustrial_food_spiced_honey_ferment",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for preindustrial_food_spiced_honey_ferment.",
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
			new CatalogueLiquidRecipeProduct(ItemStableReference: "preindustrial_food_catalogue_liquid_amphora", Quantity: 1, SkinStableReference: null, Liquid: "spiced honey ferment", Litres: 10.0) { Compatibility = new("1x a shared catalogue liquid amphora (@preindustrial_food_catalogue_liquid_amphora) filled with 10 litres of spiced honey ferment", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingFillCatalogueLiquidPreindustrialFoodSproutedGrainAle() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/fill catalogue liquid preindustrial_food_sprouted_grain_ale", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:preindustrial_food_catalogue_liquid_amphora"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["medieval", "renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "fill catalogue liquid preindustrial_food_sprouted_grain_ale",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for preindustrial_food_sprouted_grain_ale.",
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
			new CatalogueLiquidRecipeProduct(ItemStableReference: "preindustrial_food_catalogue_liquid_amphora", Quantity: 1, SkinStableReference: null, Liquid: "sprouted grain ale", Litres: 10.0) { Compatibility = new("1x a shared catalogue liquid amphora (@preindustrial_food_catalogue_liquid_amphora) filled with 10 litres of sprouted grain ale", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingFillCatalogueLiquidPreindustrialFoodWheatBeer() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/fill catalogue liquid preindustrial_food_wheat_beer", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:preindustrial_food_catalogue_liquid_amphora"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["medieval", "renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "fill catalogue liquid preindustrial_food_wheat_beer",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for preindustrial_food_wheat_beer.",
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
			new CatalogueLiquidRecipeProduct(ItemStableReference: "preindustrial_food_catalogue_liquid_amphora", Quantity: 1, SkinStableReference: null, Liquid: "wheat beer", Litres: 10.0) { Compatibility = new("1x a shared catalogue liquid amphora (@preindustrial_food_catalogue_liquid_amphora) filled with 10 litres of wheat beer", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingFillCatalogueLiquidPreindustrialFoodAmberGrapeWine() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/fill catalogue liquid preindustrial_food_amber_grape_wine", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:preindustrial_food_catalogue_liquid_amphora"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["medieval", "renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "fill catalogue liquid preindustrial_food_amber_grape_wine",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for preindustrial_food_amber_grape_wine.",
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
			new CatalogueLiquidRecipeProduct(ItemStableReference: "preindustrial_food_catalogue_liquid_amphora", Quantity: 1, SkinStableReference: null, Liquid: "amber grape wine", Litres: 10.0) { Compatibility = new("1x a shared catalogue liquid amphora (@preindustrial_food_catalogue_liquid_amphora) filled with 10 litres of amber grape wine", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingFillCatalogueLiquidPreindustrialFoodAppleWine() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/fill catalogue liquid preindustrial_food_apple_wine", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:preindustrial_food_catalogue_liquid_amphora"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["medieval", "renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "fill catalogue liquid preindustrial_food_apple_wine",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for preindustrial_food_apple_wine.",
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
			new CatalogueLiquidRecipeProduct(ItemStableReference: "preindustrial_food_catalogue_liquid_amphora", Quantity: 1, SkinStableReference: null, Liquid: "apple wine", Litres: 10.0) { Compatibility = new("1x a shared catalogue liquid amphora (@preindustrial_food_catalogue_liquid_amphora) filled with 10 litres of apple wine", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingFillCatalogueLiquidPreindustrialFoodDriedGrapeWine() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/fill catalogue liquid preindustrial_food_dried_grape_wine", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:preindustrial_food_catalogue_liquid_amphora"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["medieval", "renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "fill catalogue liquid preindustrial_food_dried_grape_wine",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for preindustrial_food_dried_grape_wine.",
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
			new CatalogueLiquidRecipeProduct(ItemStableReference: "preindustrial_food_catalogue_liquid_amphora", Quantity: 1, SkinStableReference: null, Liquid: "dried-grape wine", Litres: 10.0) { Compatibility = new("1x a shared catalogue liquid amphora (@preindustrial_food_catalogue_liquid_amphora) filled with 10 litres of dried-grape wine", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingFillCatalogueLiquidPreindustrialFoodHoneyedGrapeWine() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/fill catalogue liquid preindustrial_food_honeyed_grape_wine", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:preindustrial_food_catalogue_liquid_amphora"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["medieval", "renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "fill catalogue liquid preindustrial_food_honeyed_grape_wine",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for preindustrial_food_honeyed_grape_wine.",
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
			new CatalogueLiquidRecipeProduct(ItemStableReference: "preindustrial_food_catalogue_liquid_amphora", Quantity: 1, SkinStableReference: null, Liquid: "honeyed grape wine", Litres: 10.0) { Compatibility = new("1x a shared catalogue liquid amphora (@preindustrial_food_catalogue_liquid_amphora) filled with 10 litres of honeyed grape wine", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingFillCatalogueLiquidPreindustrialFoodPearWine() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/fill catalogue liquid preindustrial_food_pear_wine", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:preindustrial_food_catalogue_liquid_amphora"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["medieval", "renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "fill catalogue liquid preindustrial_food_pear_wine",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for preindustrial_food_pear_wine.",
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
			new CatalogueLiquidRecipeProduct(ItemStableReference: "preindustrial_food_catalogue_liquid_amphora", Quantity: 1, SkinStableReference: null, Liquid: "pear wine", Litres: 10.0) { Compatibility = new("1x a shared catalogue liquid amphora (@preindustrial_food_catalogue_liquid_amphora) filled with 10 litres of pear wine", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingFillCatalogueLiquidPreindustrialFoodPineResinGrapeWine() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/fill catalogue liquid preindustrial_food_pine_resin_grape_wine", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:preindustrial_food_catalogue_liquid_amphora"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["medieval", "renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "fill catalogue liquid preindustrial_food_pine_resin_grape_wine",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for preindustrial_food_pine_resin_grape_wine.",
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
			new CatalogueLiquidRecipeProduct(ItemStableReference: "preindustrial_food_catalogue_liquid_amphora", Quantity: 1, SkinStableReference: null, Liquid: "pine-resin grape wine", Litres: 10.0) { Compatibility = new("1x a shared catalogue liquid amphora (@preindustrial_food_catalogue_liquid_amphora) filled with 10 litres of pine-resin grape wine", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingFillCatalogueLiquidPreindustrialFoodPlumWine() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/fill catalogue liquid preindustrial_food_plum_wine", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:preindustrial_food_catalogue_liquid_amphora"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["medieval", "renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "fill catalogue liquid preindustrial_food_plum_wine",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for preindustrial_food_plum_wine.",
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
			new CatalogueLiquidRecipeProduct(ItemStableReference: "preindustrial_food_catalogue_liquid_amphora", Quantity: 1, SkinStableReference: null, Liquid: "plum wine", Litres: 10.0) { Compatibility = new("1x a shared catalogue liquid amphora (@preindustrial_food_catalogue_liquid_amphora) filled with 10 litres of plum wine", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingFillCatalogueLiquidPreindustrialFoodPomegranateWine() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/fill catalogue liquid preindustrial_food_pomegranate_wine", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:preindustrial_food_catalogue_liquid_amphora"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["medieval", "renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "fill catalogue liquid preindustrial_food_pomegranate_wine",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for preindustrial_food_pomegranate_wine.",
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
			new CatalogueLiquidRecipeProduct(ItemStableReference: "preindustrial_food_catalogue_liquid_amphora", Quantity: 1, SkinStableReference: null, Liquid: "pomegranate wine", Litres: 10.0) { Compatibility = new("1x a shared catalogue liquid amphora (@preindustrial_food_catalogue_liquid_amphora) filled with 10 litres of pomegranate wine", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingFillCatalogueLiquidPreindustrialFoodRaisinRestoredGrapeWine() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/fill catalogue liquid preindustrial_food_raisin_restored_grape_wine", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:preindustrial_food_catalogue_liquid_amphora"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["medieval", "renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "fill catalogue liquid preindustrial_food_raisin_restored_grape_wine",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for preindustrial_food_raisin_restored_grape_wine.",
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
			new CatalogueLiquidRecipeProduct(ItemStableReference: "preindustrial_food_catalogue_liquid_amphora", Quantity: 1, SkinStableReference: null, Liquid: "raisin-restored grape wine", Litres: 10.0) { Compatibility = new("1x a shared catalogue liquid amphora (@preindustrial_food_catalogue_liquid_amphora) filled with 10 litres of raisin-restored grape wine", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingFillCatalogueLiquidPreindustrialFoodRoseGrapeWine() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/fill catalogue liquid preindustrial_food_rose_grape_wine", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:preindustrial_food_catalogue_liquid_amphora"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["medieval", "renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "fill catalogue liquid preindustrial_food_rose_grape_wine",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for preindustrial_food_rose_grape_wine.",
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
			new CatalogueLiquidRecipeProduct(ItemStableReference: "preindustrial_food_catalogue_liquid_amphora", Quantity: 1, SkinStableReference: null, Liquid: "rose grape wine", Litres: 10.0) { Compatibility = new("1x a shared catalogue liquid amphora (@preindustrial_food_catalogue_liquid_amphora) filled with 10 litres of rose grape wine", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingFillCatalogueLiquidPreindustrialFoodSpicedGrapeWine() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/fill catalogue liquid preindustrial_food_spiced_grape_wine", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:preindustrial_food_catalogue_liquid_amphora"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["medieval", "renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "fill catalogue liquid preindustrial_food_spiced_grape_wine",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for preindustrial_food_spiced_grape_wine.",
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
			new CatalogueLiquidRecipeProduct(ItemStableReference: "preindustrial_food_catalogue_liquid_amphora", Quantity: 1, SkinStableReference: null, Liquid: "spiced grape wine", Litres: 10.0) { Compatibility = new("1x a shared catalogue liquid amphora (@preindustrial_food_catalogue_liquid_amphora) filled with 10 litres of spiced grape wine", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingFillCatalogueLiquidPreindustrialFoodSweetGrapeWine() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/fill catalogue liquid preindustrial_food_sweet_grape_wine", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:preindustrial_food_catalogue_liquid_amphora"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["medieval", "renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "fill catalogue liquid preindustrial_food_sweet_grape_wine",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for preindustrial_food_sweet_grape_wine.",
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
			new CatalogueLiquidRecipeProduct(ItemStableReference: "preindustrial_food_catalogue_liquid_amphora", Quantity: 1, SkinStableReference: null, Liquid: "sweet grape wine", Litres: 10.0) { Compatibility = new("1x a shared catalogue liquid amphora (@preindustrial_food_catalogue_liquid_amphora) filled with 10 litres of sweet grape wine", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingFillCatalogueLiquidPreindustrialFoodAgedGrainSpirit() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/fill catalogue liquid preindustrial_food_aged_grain_spirit", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:preindustrial_food_catalogue_liquid_amphora"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["medieval", "renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "fill catalogue liquid preindustrial_food_aged_grain_spirit",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for preindustrial_food_aged_grain_spirit.",
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
			new CatalogueLiquidRecipeProduct(ItemStableReference: "preindustrial_food_catalogue_liquid_amphora", Quantity: 1, SkinStableReference: null, Liquid: "aged grain spirit", Litres: 10.0) { Compatibility = new("1x a shared catalogue liquid amphora (@preindustrial_food_catalogue_liquid_amphora) filled with 10 litres of aged grain spirit", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingFillCatalogueLiquidPreindustrialFoodClearGrainSpirit() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/fill catalogue liquid preindustrial_food_clear_grain_spirit", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:preindustrial_food_catalogue_liquid_amphora"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["medieval", "renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "fill catalogue liquid preindustrial_food_clear_grain_spirit",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for preindustrial_food_clear_grain_spirit.",
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
			new CatalogueLiquidRecipeProduct(ItemStableReference: "preindustrial_food_catalogue_liquid_amphora", Quantity: 1, SkinStableReference: null, Liquid: "clear grain spirit", Litres: 10.0) { Compatibility = new("1x a shared catalogue liquid amphora (@preindustrial_food_catalogue_liquid_amphora) filled with 10 litres of clear grain spirit", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingFillCatalogueLiquidPreindustrialFoodFruitSpirit() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/fill catalogue liquid preindustrial_food_fruit_spirit", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:preindustrial_food_catalogue_liquid_amphora"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["medieval", "renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "fill catalogue liquid preindustrial_food_fruit_spirit",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for preindustrial_food_fruit_spirit.",
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
			new CatalogueLiquidRecipeProduct(ItemStableReference: "preindustrial_food_catalogue_liquid_amphora", Quantity: 1, SkinStableReference: null, Liquid: "clear fruit spirit", Litres: 10.0) { Compatibility = new("1x a shared catalogue liquid amphora (@preindustrial_food_catalogue_liquid_amphora) filled with 10 litres of clear fruit spirit", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingFillCatalogueLiquidPreindustrialFoodGrapeSpirit() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/fill catalogue liquid preindustrial_food_grape_spirit", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:preindustrial_food_catalogue_liquid_amphora"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["medieval", "renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "fill catalogue liquid preindustrial_food_grape_spirit",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for preindustrial_food_grape_spirit.",
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
			new CatalogueLiquidRecipeProduct(ItemStableReference: "preindustrial_food_catalogue_liquid_amphora", Quantity: 1, SkinStableReference: null, Liquid: "grape spirit", Litres: 10.0) { Compatibility = new("1x a shared catalogue liquid amphora (@preindustrial_food_catalogue_liquid_amphora) filled with 10 litres of grape spirit", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingFillCatalogueLiquidPreindustrialFoodHoneySpirit() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/fill catalogue liquid preindustrial_food_honey_spirit", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:preindustrial_food_catalogue_liquid_amphora"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["medieval", "renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "fill catalogue liquid preindustrial_food_honey_spirit",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for preindustrial_food_honey_spirit.",
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
			new CatalogueLiquidRecipeProduct(ItemStableReference: "preindustrial_food_catalogue_liquid_amphora", Quantity: 1, SkinStableReference: null, Liquid: "honey spirit", Litres: 10.0) { Compatibility = new("1x a shared catalogue liquid amphora (@preindustrial_food_catalogue_liquid_amphora) filled with 10 litres of honey spirit", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingFillCatalogueLiquidPreindustrialFoodSpicedSpirit() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/fill catalogue liquid preindustrial_food_spiced_spirit", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:preindustrial_food_catalogue_liquid_amphora"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["medieval", "renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "fill catalogue liquid preindustrial_food_spiced_spirit",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for preindustrial_food_spiced_spirit.",
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
			new CatalogueLiquidRecipeProduct(ItemStableReference: "preindustrial_food_catalogue_liquid_amphora", Quantity: 1, SkinStableReference: null, Liquid: "spiced distilled spirit", Litres: 10.0) { Compatibility = new("1x a shared catalogue liquid amphora (@preindustrial_food_catalogue_liquid_amphora) filled with 10 litres of spiced distilled spirit", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingFillCatalogueLiquidPreindustrialFoodDriedLeafInfusion() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/fill catalogue liquid preindustrial_food_dried_leaf_infusion", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:preindustrial_food_catalogue_liquid_amphora"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["medieval", "renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "fill catalogue liquid preindustrial_food_dried_leaf_infusion",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for preindustrial_food_dried_leaf_infusion.",
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
			new CatalogueLiquidRecipeProduct(ItemStableReference: "preindustrial_food_catalogue_liquid_amphora", Quantity: 1, SkinStableReference: null, Liquid: "dried leaf infusion", Litres: 10.0) { Compatibility = new("1x a shared catalogue liquid amphora (@preindustrial_food_catalogue_liquid_amphora) filled with 10 litres of dried leaf infusion", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingFillCatalogueLiquidPreindustrialFoodFlowerInfusion() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/fill catalogue liquid preindustrial_food_flower_infusion", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:preindustrial_food_catalogue_liquid_amphora"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["medieval", "renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "fill catalogue liquid preindustrial_food_flower_infusion",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for preindustrial_food_flower_infusion.",
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
			new CatalogueLiquidRecipeProduct(ItemStableReference: "preindustrial_food_catalogue_liquid_amphora", Quantity: 1, SkinStableReference: null, Liquid: "flower infusion", Litres: 10.0) { Compatibility = new("1x a shared catalogue liquid amphora (@preindustrial_food_catalogue_liquid_amphora) filled with 10 litres of flower infusion", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingFillCatalogueLiquidPreindustrialFoodFreshLeafInfusion() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/fill catalogue liquid preindustrial_food_fresh_leaf_infusion", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:preindustrial_food_catalogue_liquid_amphora"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["medieval", "renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "fill catalogue liquid preindustrial_food_fresh_leaf_infusion",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for preindustrial_food_fresh_leaf_infusion.",
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
			new CatalogueLiquidRecipeProduct(ItemStableReference: "preindustrial_food_catalogue_liquid_amphora", Quantity: 1, SkinStableReference: null, Liquid: "fresh leaf infusion", Litres: 10.0) { Compatibility = new("1x a shared catalogue liquid amphora (@preindustrial_food_catalogue_liquid_amphora) filled with 10 litres of fresh leaf infusion", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingFillCatalogueLiquidPreindustrialFoodFruitPeelInfusion() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/fill catalogue liquid preindustrial_food_fruit_peel_infusion", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:preindustrial_food_catalogue_liquid_amphora"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["medieval", "renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "fill catalogue liquid preindustrial_food_fruit_peel_infusion",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for preindustrial_food_fruit_peel_infusion.",
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
			new CatalogueLiquidRecipeProduct(ItemStableReference: "preindustrial_food_catalogue_liquid_amphora", Quantity: 1, SkinStableReference: null, Liquid: "fruit peel infusion", Litres: 10.0) { Compatibility = new("1x a shared catalogue liquid amphora (@preindustrial_food_catalogue_liquid_amphora) filled with 10 litres of fruit peel infusion", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingFillCatalogueLiquidPreindustrialFoodHoneyedHerbInfusion() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/fill catalogue liquid preindustrial_food_honeyed_herb_infusion", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:preindustrial_food_catalogue_liquid_amphora"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["medieval", "renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "fill catalogue liquid preindustrial_food_honeyed_herb_infusion",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for preindustrial_food_honeyed_herb_infusion.",
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
			new CatalogueLiquidRecipeProduct(ItemStableReference: "preindustrial_food_catalogue_liquid_amphora", Quantity: 1, SkinStableReference: null, Liquid: "honeyed herb infusion", Litres: 10.0) { Compatibility = new("1x a shared catalogue liquid amphora (@preindustrial_food_catalogue_liquid_amphora) filled with 10 litres of honeyed herb infusion", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};
}
