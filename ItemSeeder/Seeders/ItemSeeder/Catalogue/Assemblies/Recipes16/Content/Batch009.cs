#nullable enable

using MudSharp.Body;
using MudSharp.GameItems.Inventory.Plans;
using MudSharp.RPG.Checks;

namespace DatabaseSeeder.Seeders;

public static partial class Recipes16Stock
{
	public static CatalogueRecipeDefinition FoodAndDrinkPrepareSweetFriedRavioli() => new()
	{
		Metadata = new("Food and drink/prepare sweet fried ravioli", "crafts", ["renaissance"], ["item:renaissance_food_italian_fried_ravioli_sweet"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "prepare sweet fried ravioli",
		Category = "Food and drink",
		Blurb = "prepare sweet fried ravioli",
		Action = "preparing a food or drink item",
		ActiveCraftItemSdesc = "an in-progress sweet fried ravioli craft",
		Gate = CatalogueRecipeGates.RenaissanceFinishedItemCraftingCooking10,
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
			new(25, "$0 sort|sorts and prepare|prepares $i1 beside $t1.", "$0 sort|sorts and prepare|prepares $i1 beside $t1.", ExertionLevel.Stasis, 0),
			new(40, "$0 tend|tends the heat and work|works the prepared material into its final form. $0 turn|turns the work over to judge its balance.", "$0 tend|tends the heat and work|works the prepared material into its final form. $0 turn|turns the work over to judge its balance.", ExertionLevel.Stasis, 0),
			new(25, "$0 portion|portions and set|sets aside $p1 for serving or storage.", "$0 find|finds the batch spoiled and set|sets the material aside.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 188.0, Material: "cheese", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("188 grams of cheese", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hot Fire", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Hot Fire tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_food_italian_fried_ravioli_sweet", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x sweet fried ravioli (@renaissance_food_italian_fried_ravioli_sweet)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodAndDrinkPrepareFruitMustMostarda() => new()
	{
		Metadata = new("Food and drink/prepare fruit-must mostarda", "crafts", ["renaissance"], ["item:renaissance_food_italian_fruit_must_mostarda"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "prepare fruit-must mostarda",
		Category = "Food and drink",
		Blurb = "prepare a fruit-must mostarda",
		Action = "preparing a food or drink item",
		ActiveCraftItemSdesc = "an in-progress fruit-must mostarda craft",
		Gate = CatalogueRecipeGates.RenaissanceFinishedItemCraftingCooking10,
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
			new(25, "$0 sort|sorts and prepare|prepares $i1 beside $t1.", "$0 sort|sorts and prepare|prepares $i1 beside $t1.", ExertionLevel.Stasis, 0),
			new(40, "$0 tend|tends the heat and work|works the prepared material into its final form. $0 turn|turns the work over to judge its balance.", "$0 tend|tends the heat and work|works the prepared material into its final form. $0 turn|turns the work over to judge its balance.", ExertionLevel.Stasis, 0),
			new(25, "$0 portion|portions and set|sets aside $p1 for serving or storage.", "$0 find|finds the batch spoiled and set|sets the material aside.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 165.0, Material: "food", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("165 grams of food", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hot Fire", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Hot Fire tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_food_italian_fruit_must_mostarda", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a fruit-must mostarda (@renaissance_food_italian_fruit_must_mostarda)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodAndDrinkPrepareGreenHerbTortelli() => new()
	{
		Metadata = new("Food and drink/prepare green herb tortelli", "crafts", ["renaissance"], ["item:renaissance_food_italian_greens_tortelli"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "prepare green herb tortelli",
		Category = "Food and drink",
		Blurb = "prepare green herb tortelli",
		Action = "preparing a food or drink item",
		ActiveCraftItemSdesc = "an in-progress green herb tortelli craft",
		Gate = CatalogueRecipeGates.RenaissanceFinishedItemCraftingCooking10,
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
			new(25, "$0 sort|sorts and prepare|prepares $i1 beside $t1.", "$0 sort|sorts and prepare|prepares $i1 beside $t1.", ExertionLevel.Stasis, 0),
			new(40, "$0 tend|tends the heat and work|works the prepared material into its final form. $0 turn|turns the work over to judge its balance.", "$0 tend|tends the heat and work|works the prepared material into its final form. $0 turn|turns the work over to judge its balance.", ExertionLevel.Stasis, 0),
			new(25, "$0 portion|portions and set|sets aside $p1 for serving or storage.", "$0 find|finds the batch spoiled and set|sets the material aside.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 323.0, Material: "cheese", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("323 grams of cheese", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hot Fire", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Hot Fire tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_food_italian_greens_tortelli", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x green herb tortelli (@renaissance_food_italian_greens_tortelli)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodAndDrinkPrepareHerbAndCheeseRavioli() => new()
	{
		Metadata = new("Food and drink/prepare herb-and-cheese ravioli", "crafts", ["renaissance"], ["item:renaissance_food_italian_herb_cheese_ravioli"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "prepare herb-and-cheese ravioli",
		Category = "Food and drink",
		Blurb = "prepare herb-and-cheese ravioli",
		Action = "preparing a food or drink item",
		ActiveCraftItemSdesc = "an in-progress herb-and-cheese ravioli craft",
		Gate = CatalogueRecipeGates.RenaissanceFinishedItemCraftingCooking10,
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
			new(25, "$0 sort|sorts and prepare|prepares $i1 beside $t1.", "$0 sort|sorts and prepare|prepares $i1 beside $t1.", ExertionLevel.Stasis, 0),
			new(40, "$0 tend|tends the heat and work|works the prepared material into its final form. $0 turn|turns the work over to judge its balance.", "$0 tend|tends the heat and work|works the prepared material into its final form. $0 turn|turns the work over to judge its balance.", ExertionLevel.Stasis, 0),
			new(25, "$0 portion|portions and set|sets aside $p1 for serving or storage.", "$0 find|finds the batch spoiled and set|sets the material aside.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 338.0, Material: "cheese", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("338 grams of cheese", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hot Fire", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Hot Fire tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_food_italian_herb_cheese_ravioli", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x herb-and-cheese ravioli (@renaissance_food_italian_herb_cheese_ravioli)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodAndDrinkPrepareIcedFruitSnow() => new()
	{
		Metadata = new("Food and drink/prepare iced fruit snow", "crafts", ["renaissance"], ["item:renaissance_food_italian_iced_fruit_snow"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "prepare iced fruit snow",
		Category = "Food and drink",
		Blurb = "prepare an iced fruit snow",
		Action = "preparing a food or drink item",
		ActiveCraftItemSdesc = "an in-progress iced fruit snow craft",
		Gate = CatalogueRecipeGates.RenaissanceFinishedItemCraftingCooking10,
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
			new(25, "$0 sort|sorts and prepare|prepares $i1 beside $t1.", "$0 sort|sorts and prepare|prepares $i1 beside $t1.", ExertionLevel.Stasis, 0),
			new(40, "$0 tend|tends the heat and work|works the prepared material into its final form. $0 turn|turns the work over to judge its balance.", "$0 tend|tends the heat and work|works the prepared material into its final form. $0 turn|turns the work over to judge its balance.", ExertionLevel.Stasis, 0),
			new(25, "$0 portion|portions and set|sets aside $p1 for serving or storage.", "$0 find|finds the batch spoiled and set|sets the material aside.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 225.0, Material: "food", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("225 grams of food", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hot Fire", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Hot Fire tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_food_italian_iced_fruit_snow", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x an iced fruit snow (@renaissance_food_italian_iced_fruit_snow)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodAndDrinkPrepareLayeredEggplantTorta() => new()
	{
		Metadata = new("Food and drink/prepare layered eggplant torta", "crafts", ["renaissance"], ["item:renaissance_food_italian_layered_eggplant_torta"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "prepare layered eggplant torta",
		Category = "Food and drink",
		Blurb = "prepare a layered eggplant torta",
		Action = "preparing a food or drink item",
		ActiveCraftItemSdesc = "an in-progress layered eggplant torta craft",
		Gate = CatalogueRecipeGates.RenaissanceFinishedItemCraftingCooking10,
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
			new(25, "$0 sort|sorts and prepare|prepares $i1 beside $t1.", "$0 sort|sorts and prepare|prepares $i1 beside $t1.", ExertionLevel.Stasis, 0),
			new(40, "$0 tend|tends the heat and work|works the prepared material into its final form. $0 turn|turns the work over to judge its balance.", "$0 tend|tends the heat and work|works the prepared material into its final form. $0 turn|turns the work over to judge its balance.", ExertionLevel.Stasis, 0),
			new(25, "$0 portion|portions and set|sets aside $p1 for serving or storage.", "$0 find|finds the batch spoiled and set|sets the material aside.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 525.0, Material: "vegetable", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("525 grams of vegetable", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hot Fire", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Hot Fire tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_food_italian_layered_eggplant_torta", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a layered eggplant torta (@renaissance_food_italian_layered_eggplant_torta)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodAndDrinkPrepareLayeredRenaissanceLasagne() => new()
	{
		Metadata = new("Food and drink/prepare layered Renaissance lasagne", "crafts", ["renaissance"], ["item:renaissance_food_italian_layered_lasagne"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "prepare layered Renaissance lasagne",
		Category = "Food and drink",
		Blurb = "prepare layered Renaissance lasagne",
		Action = "preparing a food or drink item",
		ActiveCraftItemSdesc = "an in-progress layered Renaissance lasagne craft",
		Gate = CatalogueRecipeGates.RenaissanceFinishedItemCraftingCooking10,
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
			new(25, "$0 sort|sorts and prepare|prepares $i1 beside $t1.", "$0 sort|sorts and prepare|prepares $i1 beside $t1.", ExertionLevel.Stasis, 0),
			new(40, "$0 tend|tends the heat and work|works the prepared material into its final form. $0 turn|turns the work over to judge its balance.", "$0 tend|tends the heat and work|works the prepared material into its final form. $0 turn|turns the work over to judge its balance.", ExertionLevel.Stasis, 0),
			new(25, "$0 portion|portions and set|sets aside $p1 for serving or storage.", "$0 find|finds the batch spoiled and set|sets the material aside.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 488.0, Material: "wheat", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("488 grams of wheat", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hot Fire", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Hot Fire tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_food_italian_layered_lasagne", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x layered Renaissance lasagne (@renaissance_food_italian_layered_lasagne)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodAndDrinkPrepareTheatricalBirdShowPie() => new()
	{
		Metadata = new("Food and drink/prepare theatrical bird show pie", "crafts", ["renaissance"], ["item:renaissance_food_italian_live_bird_show_pie"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "prepare theatrical bird show pie",
		Category = "Food and drink",
		Blurb = "prepare a theatrical bird show pie",
		Action = "preparing a food or drink item",
		ActiveCraftItemSdesc = "an in-progress theatrical bird show pie craft",
		Gate = CatalogueRecipeGates.RenaissanceFinishedItemCraftingCooking10,
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
			new(25, "$0 sort|sorts and prepare|prepares $i1 beside $t1.", "$0 sort|sorts and prepare|prepares $i1 beside $t1.", ExertionLevel.Stasis, 0),
			new(40, "$0 tend|tends the heat and work|works the prepared material into its final form. $0 pause|pauses to check the proportions before carrying on.", "$0 tend|tends the heat and work|works the prepared material into its final form. $0 pause|pauses to check the proportions before carrying on.", ExertionLevel.Stasis, 0),
			new(25, "$0 portion|portions and set|sets aside $p1 for serving or storage.", "$0 find|finds the batch spoiled and set|sets the material aside.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1350.0, Material: "meat", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1350 grams of meat", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hot Fire", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Hot Fire tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_food_italian_live_bird_show_pie", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a theatrical bird show pie (@renaissance_food_italian_live_bird_show_pie)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodAndDrinkPreparePaintedSugarSculpture() => new()
	{
		Metadata = new("Food and drink/prepare painted sugar sculpture", "crafts", ["renaissance"], ["item:renaissance_food_italian_marquets_sugar_sculpture"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "prepare painted sugar sculpture",
		Category = "Food and drink",
		Blurb = "prepare a painted sugar sculpture",
		Action = "preparing a food or drink item",
		ActiveCraftItemSdesc = "an in-progress painted sugar sculpture craft",
		Gate = CatalogueRecipeGates.RenaissanceFinishedItemCraftingCooking10,
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
			new(25, "$0 sort|sorts and prepare|prepares $i1 beside $t1.", "$0 sort|sorts and prepare|prepares $i1 beside $t1.", ExertionLevel.Stasis, 0),
			new(40, "$0 tend|tends the heat and work|works the prepared material into its final form. $0 measure|measures the developing form against the intended finish.", "$0 tend|tends the heat and work|works the prepared material into its final form. $0 measure|measures the developing form against the intended finish.", ExertionLevel.Stasis, 0),
			new(25, "$0 portion|portions and set|sets aside $p1 for serving or storage.", "$0 find|finds the batch spoiled and set|sets the material aside.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 525.0, Material: "honey", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("525 grams of honey", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hot Fire", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Hot Fire tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_food_italian_marquets_sugar_sculpture", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a painted sugar sculpture (@renaissance_food_italian_marquets_sugar_sculpture)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodAndDrinkPrepareMeatRavioliInClearBroth() => new()
	{
		Metadata = new("Food and drink/prepare meat ravioli in clear broth", "crafts", ["renaissance"], ["item:renaissance_food_italian_meat_ravioli_broth"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "prepare meat ravioli in clear broth",
		Category = "Food and drink",
		Blurb = "prepare meat ravioli in clear broth",
		Action = "preparing a food or drink item",
		ActiveCraftItemSdesc = "an in-progress meat ravioli in clear broth craft",
		Gate = CatalogueRecipeGates.RenaissanceFinishedItemCraftingCooking10,
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
			new(25, "$0 sort|sorts and prepare|prepares $i1 beside $t1.", "$0 sort|sorts and prepare|prepares $i1 beside $t1.", ExertionLevel.Stasis, 0),
			new(40, "$0 tend|tends the heat and work|works the prepared material into its final form. $0 turn|turns the work over to judge its balance.", "$0 tend|tends the heat and work|works the prepared material into its final form. $0 turn|turns the work over to judge its balance.", ExertionLevel.Stasis, 0),
			new(25, "$0 portion|portions and set|sets aside $p1 for serving or storage.", "$0 find|finds the batch spoiled and set|sets the material aside.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 375.0, Material: "meat", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("375 grams of meat", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hot Fire", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Hot Fire tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_food_italian_meat_ravioli_broth", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x meat ravioli in clear broth (@renaissance_food_italian_meat_ravioli_broth)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodAndDrinkPreparePeacockWithSugarPlumage() => new()
	{
		Metadata = new("Food and drink/prepare peacock with sugar plumage", "crafts", ["renaissance"], ["item:renaissance_food_italian_peacock_sugar_plumage"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "prepare peacock with sugar plumage",
		Category = "Food and drink",
		Blurb = "prepare a peacock with sugar plumage",
		Action = "preparing a food or drink item",
		ActiveCraftItemSdesc = "an in-progress peacock with sugar plumage craft",
		Gate = CatalogueRecipeGates.RenaissanceFinishedItemCraftingCooking10,
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
			new(25, "$0 sort|sorts and prepare|prepares $i1 beside $t1.", "$0 sort|sorts and prepare|prepares $i1 beside $t1.", ExertionLevel.Stasis, 0),
			new(40, "$0 tend|tends the heat and work|works the prepared material into its final form. $0 pause|pauses to check the proportions before carrying on.", "$0 tend|tends the heat and work|works the prepared material into its final form. $0 pause|pauses to check the proportions before carrying on.", ExertionLevel.Stasis, 0),
			new(25, "$0 portion|portions and set|sets aside $p1 for serving or storage.", "$0 find|finds the batch spoiled and set|sets the material aside.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1350.0, Material: "meat", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1350 grams of meat", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hot Fire", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Hot Fire tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_food_italian_peacock_sugar_plumage", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a peacock with sugar plumage (@renaissance_food_italian_peacock_sugar_plumage)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodAndDrinkPrepareSweetSpicedPumpkinTortelli() => new()
	{
		Metadata = new("Food and drink/prepare sweet-spiced pumpkin tortelli", "crafts", ["renaissance"], ["item:renaissance_food_italian_pumpkin_tortelli"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "prepare sweet-spiced pumpkin tortelli",
		Category = "Food and drink",
		Blurb = "prepare sweet-spiced pumpkin tortelli",
		Action = "preparing a food or drink item",
		ActiveCraftItemSdesc = "an in-progress sweet-spiced pumpkin tortelli craft",
		Gate = CatalogueRecipeGates.RenaissanceFinishedItemCraftingCooking10,
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
			new(25, "$0 sort|sorts and prepare|prepares $i1 beside $t1.", "$0 sort|sorts and prepare|prepares $i1 beside $t1.", ExertionLevel.Stasis, 0),
			new(40, "$0 tend|tends the heat and work|works the prepared material into its final form. $0 clear|clears the work area before the final steps.", "$0 tend|tends the heat and work|works the prepared material into its final form. $0 clear|clears the work area before the final steps.", ExertionLevel.Stasis, 0),
			new(25, "$0 portion|portions and set|sets aside $p1 for serving or storage.", "$0 find|finds the batch spoiled and set|sets the material aside.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 338.0, Material: "vegetable", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("338 grams of vegetable", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hot Fire", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Hot Fire tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_food_italian_pumpkin_tortelli", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x sweet-spiced pumpkin tortelli (@renaissance_food_italian_pumpkin_tortelli)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodAndDrinkPrepareQuinceMostardaPreserve() => new()
	{
		Metadata = new("Food and drink/prepare quince mostarda preserve", "crafts", ["renaissance"], ["item:renaissance_food_italian_quince_mostarda"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "prepare quince mostarda preserve",
		Category = "Food and drink",
		Blurb = "prepare a quince mostarda preserve",
		Action = "preparing a food or drink item",
		ActiveCraftItemSdesc = "an in-progress quince mostarda preserve craft",
		Gate = CatalogueRecipeGates.RenaissanceFinishedItemCraftingCooking10,
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
			new(25, "$0 sort|sorts and prepare|prepares $i1 beside $t1.", "$0 sort|sorts and prepare|prepares $i1 beside $t1.", ExertionLevel.Stasis, 0),
			new(40, "$0 tend|tends the heat and work|works the prepared material into its final form. $0 measure|measures the developing form against the intended finish.", "$0 tend|tends the heat and work|works the prepared material into its final form. $0 measure|measures the developing form against the intended finish.", ExertionLevel.Stasis, 0),
			new(25, "$0 portion|portions and set|sets aside $p1 for serving or storage.", "$0 find|finds the batch spoiled and set|sets the material aside.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 135.0, Material: "food", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("135 grams of food", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hot Fire", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Hot Fire tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_food_italian_quince_mostarda", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a quince mostarda preserve (@renaissance_food_italian_quince_mostarda)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodAndDrinkPrepareRevestedCaponShowpiece() => new()
	{
		Metadata = new("Food and drink/prepare revested capon showpiece", "crafts", ["renaissance"], ["item:renaissance_food_italian_revested_capon_showpiece"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "prepare revested capon showpiece",
		Category = "Food and drink",
		Blurb = "prepare a revested capon showpiece",
		Action = "preparing a food or drink item",
		ActiveCraftItemSdesc = "an in-progress revested capon showpiece craft",
		Gate = CatalogueRecipeGates.RenaissanceFinishedItemCraftingCooking10,
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
			new(25, "$0 sort|sorts and prepare|prepares $i1 beside $t1.", "$0 sort|sorts and prepare|prepares $i1 beside $t1.", ExertionLevel.Stasis, 0),
			new(40, "$0 tend|tends the heat and work|works the prepared material into its final form. $0 turn|turns the work over to judge its balance.", "$0 tend|tends the heat and work|works the prepared material into its final form. $0 turn|turns the work over to judge its balance.", ExertionLevel.Stasis, 0),
			new(25, "$0 portion|portions and set|sets aside $p1 for serving or storage.", "$0 find|finds the batch spoiled and set|sets the material aside.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1050.0, Material: "meat", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1050 grams of meat", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hot Fire", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Hot Fire tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_food_italian_revested_capon_showpiece", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a revested capon showpiece (@renaissance_food_italian_revested_capon_showpiece)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodAndDrinkPrepareSweetRiceCustardTorta() => new()
	{
		Metadata = new("Food and drink/prepare sweet rice custard torta", "crafts", ["renaissance"], ["item:renaissance_food_italian_rice_custard_torta"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "prepare sweet rice custard torta",
		Category = "Food and drink",
		Blurb = "prepare a sweet rice custard torta",
		Action = "preparing a food or drink item",
		ActiveCraftItemSdesc = "an in-progress sweet rice custard torta craft",
		Gate = CatalogueRecipeGates.RenaissanceFinishedItemCraftingCooking10,
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
			new(25, "$0 sort|sorts and prepare|prepares $i1 beside $t1.", "$0 sort|sorts and prepare|prepares $i1 beside $t1.", ExertionLevel.Stasis, 0),
			new(40, "$0 tend|tends the heat and work|works the prepared material into its final form. $0 measure|measures the developing form against the intended finish.", "$0 tend|tends the heat and work|works the prepared material into its final form. $0 measure|measures the developing form against the intended finish.", ExertionLevel.Stasis, 0),
			new(25, "$0 portion|portions and set|sets aside $p1 for serving or storage.", "$0 find|finds the batch spoiled and set|sets the material aside.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 450.0, Material: "rice", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("450 grams of rice", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hot Fire", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Hot Fire tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_food_italian_rice_custard_torta", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a sweet rice custard torta (@renaissance_food_italian_rice_custard_torta)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodAndDrinkPrepareRenaissanceRiceFlourGnocchi() => new()
	{
		Metadata = new("Food and drink/prepare Renaissance rice-flour gnocchi", "crafts", ["renaissance"], ["item:renaissance_food_italian_rice_flour_gnocchi"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "prepare Renaissance rice-flour gnocchi",
		Category = "Food and drink",
		Blurb = "prepare Renaissance rice-flour gnocchi",
		Action = "preparing a food or drink item",
		ActiveCraftItemSdesc = "an in-progress Renaissance rice-flour gnocchi craft",
		Gate = CatalogueRecipeGates.RenaissanceFinishedItemCraftingCooking10,
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
			new(25, "$0 sort|sorts and prepare|prepares $i1 beside $t1.", "$0 sort|sorts and prepare|prepares $i1 beside $t1.", ExertionLevel.Stasis, 0),
			new(40, "$0 tend|tends the heat and work|works the prepared material into its final form. $0 turn|turns the work over to judge its balance.", "$0 tend|tends the heat and work|works the prepared material into its final form. $0 turn|turns the work over to judge its balance.", ExertionLevel.Stasis, 0),
			new(25, "$0 portion|portions and set|sets aside $p1 for serving or storage.", "$0 find|finds the batch spoiled and set|sets the material aside.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 338.0, Material: "rice", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("338 grams of rice", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hot Fire", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Hot Fire tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_food_italian_rice_flour_gnocchi", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x Renaissance rice-flour gnocchi (@renaissance_food_italian_rice_flour_gnocchi)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodAndDrinkPrepareFineSavoyardBiscuit() => new()
	{
		Metadata = new("Food and drink/prepare fine Savoyard biscuit", "crafts", ["renaissance"], ["item:renaissance_food_italian_savoiardi_biscuit"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "prepare fine Savoyard biscuit",
		Category = "Food and drink",
		Blurb = "prepare a fine Savoyard biscuit",
		Action = "preparing a food or drink item",
		ActiveCraftItemSdesc = "an in-progress fine Savoyard biscuit craft",
		Gate = CatalogueRecipeGates.RenaissanceFinishedItemCraftingCooking10,
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
			new(25, "$0 sort|sorts and prepare|prepares $i1 beside $t1.", "$0 sort|sorts and prepare|prepares $i1 beside $t1.", ExertionLevel.Stasis, 0),
			new(40, "$0 tend|tends the heat and work|works the prepared material into its final form. $0 clear|clears the work area before the final steps.", "$0 tend|tends the heat and work|works the prepared material into its final form. $0 clear|clears the work area before the final steps.", ExertionLevel.Stasis, 0),
			new(25, "$0 portion|portions and set|sets aside $p1 for serving or storage.", "$0 find|finds the batch spoiled and set|sets the material aside.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 75.0, Material: "wheat", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("75 grams of wheat", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hot Fire", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Hot Fire tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_food_italian_savoiardi_biscuit", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a fine Savoyard biscuit (@renaissance_food_italian_savoiardi_biscuit)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodAndDrinkPrepareSculptedBanquetPie() => new()
	{
		Metadata = new("Food and drink/prepare sculpted banquet pie", "crafts", ["renaissance"], ["item:renaissance_food_italian_sculpted_pastry_pie"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "prepare sculpted banquet pie",
		Category = "Food and drink",
		Blurb = "prepare a sculpted banquet pie",
		Action = "preparing a food or drink item",
		ActiveCraftItemSdesc = "an in-progress sculpted banquet pie craft",
		Gate = CatalogueRecipeGates.RenaissanceFinishedItemCraftingCooking10,
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
			new(25, "$0 sort|sorts and prepare|prepares $i1 beside $t1.", "$0 sort|sorts and prepare|prepares $i1 beside $t1.", ExertionLevel.Stasis, 0),
			new(40, "$0 tend|tends the heat and work|works the prepared material into its final form. $0 pause|pauses to check the proportions before carrying on.", "$0 tend|tends the heat and work|works the prepared material into its final form. $0 pause|pauses to check the proportions before carrying on.", ExertionLevel.Stasis, 0),
			new(25, "$0 portion|portions and set|sets aside $p1 for serving or storage.", "$0 find|finds the batch spoiled and set|sets the material aside.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 900.0, Material: "meat", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("900 grams of meat", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hot Fire", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Hot Fire tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_food_italian_sculpted_pastry_pie", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a sculpted banquet pie (@renaissance_food_italian_sculpted_pastry_pie)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodAndDrinkPrepareSpinachAndCurdTorta() => new()
	{
		Metadata = new("Food and drink/prepare spinach-and-curd torta", "crafts", ["renaissance"], ["item:renaissance_food_italian_spinach_ricotta_torta"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "prepare spinach-and-curd torta",
		Category = "Food and drink",
		Blurb = "prepare a spinach-and-curd torta",
		Action = "preparing a food or drink item",
		ActiveCraftItemSdesc = "an in-progress spinach-and-curd torta craft",
		Gate = CatalogueRecipeGates.RenaissanceFinishedItemCraftingCooking10,
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
			new(25, "$0 sort|sorts and prepare|prepares $i1 beside $t1.", "$0 sort|sorts and prepare|prepares $i1 beside $t1.", ExertionLevel.Stasis, 0),
			new(40, "$0 tend|tends the heat and work|works the prepared material into its final form. $0 turn|turns the work over to judge its balance.", "$0 tend|tends the heat and work|works the prepared material into its final form. $0 turn|turns the work over to judge its balance.", ExertionLevel.Stasis, 0),
			new(25, "$0 portion|portions and set|sets aside $p1 for serving or storage.", "$0 find|finds the batch spoiled and set|sets the material aside.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 488.0, Material: "cheese", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("488 grams of cheese", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hot Fire", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Hot Fire tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_food_italian_spinach_ricotta_torta", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a spinach-and-curd torta (@renaissance_food_italian_spinach_ricotta_torta)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodAndDrinkPrepareStuffedArtichokeHeads() => new()
	{
		Metadata = new("Food and drink/prepare stuffed artichoke heads", "crafts", ["renaissance"], ["item:renaissance_food_italian_stuffed_artichoke_heads"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "prepare stuffed artichoke heads",
		Category = "Food and drink",
		Blurb = "prepare stuffed artichoke heads",
		Action = "preparing a food or drink item",
		ActiveCraftItemSdesc = "an in-progress stuffed artichoke heads craft",
		Gate = CatalogueRecipeGates.RenaissanceFinishedItemCraftingCooking10,
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
			new(25, "$0 sort|sorts and prepare|prepares $i1 beside $t1.", "$0 sort|sorts and prepare|prepares $i1 beside $t1.", ExertionLevel.Stasis, 0),
			new(40, "$0 tend|tends the heat and work|works the prepared material into its final form. $0 clear|clears the work area before the final steps.", "$0 tend|tends the heat and work|works the prepared material into its final form. $0 clear|clears the work area before the final steps.", ExertionLevel.Stasis, 0),
			new(25, "$0 portion|portions and set|sets aside $p1 for serving or storage.", "$0 find|finds the batch spoiled and set|sets the material aside.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 300.0, Material: "vegetable", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("300 grams of vegetable", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hot Fire", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Hot Fire tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_food_italian_stuffed_artichoke_heads", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x stuffed artichoke heads (@renaissance_food_italian_stuffed_artichoke_heads)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodAndDrinkPrepareSugarPlateCastle() => new()
	{
		Metadata = new("Food and drink/prepare sugar-plate castle", "crafts", ["renaissance"], ["item:renaissance_food_italian_sugar_plate_castle"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "prepare sugar-plate castle",
		Category = "Food and drink",
		Blurb = "prepare a sugar-plate castle",
		Action = "preparing a food or drink item",
		ActiveCraftItemSdesc = "an in-progress sugar-plate castle craft",
		Gate = CatalogueRecipeGates.RenaissanceFinishedItemCraftingCooking10,
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
			new(25, "$0 sort|sorts and prepare|prepares $i1 beside $t1.", "$0 sort|sorts and prepare|prepares $i1 beside $t1.", ExertionLevel.Stasis, 0),
			new(40, "$0 tend|tends the heat and work|works the prepared material into its final form. $0 turn|turns the work over to judge its balance.", "$0 tend|tends the heat and work|works the prepared material into its final form. $0 turn|turns the work over to judge its balance.", ExertionLevel.Stasis, 0),
			new(25, "$0 portion|portions and set|sets aside $p1 for serving or storage.", "$0 find|finds the batch spoiled and set|sets the material aside.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 675.0, Material: "honey", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("675 grams of honey", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hot Fire", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Hot Fire tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_food_italian_sugar_plate_castle", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a sugar-plate castle (@renaissance_food_italian_sugar_plate_castle)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodAndDrinkPrepareSugarSpicedItalianMacaroni() => new()
	{
		Metadata = new("Food and drink/prepare sugar-spiced Italian macaroni", "crafts", ["renaissance"], ["item:renaissance_food_italian_sugar_spiced_macaroni"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "prepare sugar-spiced Italian macaroni",
		Category = "Food and drink",
		Blurb = "prepare sugar-spiced Italian macaroni",
		Action = "preparing a food or drink item",
		ActiveCraftItemSdesc = "an in-progress sugar-spiced Italian macaroni craft",
		Gate = CatalogueRecipeGates.RenaissanceFinishedItemCraftingCooking10,
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
			new(25, "$0 sort|sorts and prepare|prepares $i1 beside $t1.", "$0 sort|sorts and prepare|prepares $i1 beside $t1.", ExertionLevel.Stasis, 0),
			new(40, "$0 tend|tends the heat and work|works the prepared material into its final form. $0 clear|clears the work area before the final steps.", "$0 tend|tends the heat and work|works the prepared material into its final form. $0 clear|clears the work area before the final steps.", ExertionLevel.Stasis, 0),
			new(25, "$0 portion|portions and set|sets aside $p1 for serving or storage.", "$0 find|finds the batch spoiled and set|sets the material aside.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 413.0, Material: "wheat", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("413 grams of wheat", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hot Fire", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Hot Fire tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_food_italian_sugar_spiced_macaroni", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x sugar-spiced Italian macaroni (@renaissance_food_italian_sugar_spiced_macaroni)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodAndDrinkPrepareTurkeyPreparedInCaponStyle() => new()
	{
		Metadata = new("Food and drink/prepare turkey prepared in capon style", "crafts", ["renaissance"], ["item:renaissance_food_italian_turkey_capon_style"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "prepare turkey prepared in capon style",
		Category = "Food and drink",
		Blurb = "prepare turkey prepared in capon style",
		Action = "preparing a food or drink item",
		ActiveCraftItemSdesc = "an in-progress turkey prepared in capon style craft",
		Gate = CatalogueRecipeGates.RenaissanceFinishedItemCraftingCooking10,
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
			new(25, "$0 sort|sorts and prepare|prepares $i1 beside $t1.", "$0 sort|sorts and prepare|prepares $i1 beside $t1.", ExertionLevel.Stasis, 0),
			new(40, "$0 tend|tends the heat and work|works the prepared material into its final form. $0 turn|turns the work over to judge its balance.", "$0 tend|tends the heat and work|works the prepared material into its final form. $0 turn|turns the work over to judge its balance.", ExertionLevel.Stasis, 0),
			new(25, "$0 portion|portions and set|sets aside $p1 for serving or storage.", "$0 find|finds the batch spoiled and set|sets the material aside.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1200.0, Material: "meat", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1200 grams of meat", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hot Fire", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Hot Fire tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_food_italian_turkey_capon_style", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x turkey prepared in capon style (@renaissance_food_italian_turkey_capon_style)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodAndDrinkPrepareAlmondMilkFastingVermicelli() => new()
	{
		Metadata = new("Food and drink/prepare almond-milk fasting vermicelli", "crafts", ["renaissance"], ["item:renaissance_food_italian_vermicelli_almond_fast"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "prepare almond-milk fasting vermicelli",
		Category = "Food and drink",
		Blurb = "prepare almond-milk fasting vermicelli",
		Action = "preparing a food or drink item",
		ActiveCraftItemSdesc = "an in-progress almond-milk fasting vermicelli craft",
		Gate = CatalogueRecipeGates.RenaissanceFinishedItemCraftingCooking10,
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
			new(25, "$0 sort|sorts and prepare|prepares $i1 beside $t1.", "$0 sort|sorts and prepare|prepares $i1 beside $t1.", ExertionLevel.Stasis, 0),
			new(40, "$0 tend|tends the heat and work|works the prepared material into its final form. $0 pause|pauses to check the proportions before carrying on.", "$0 tend|tends the heat and work|works the prepared material into its final form. $0 pause|pauses to check the proportions before carrying on.", ExertionLevel.Stasis, 0),
			new(25, "$0 portion|portions and set|sets aside $p1 for serving or storage.", "$0 find|finds the batch spoiled and set|sets the material aside.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 338.0, Material: "wheat", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("338 grams of wheat", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hot Fire", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Hot Fire tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_food_italian_vermicelli_almond_fast", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x almond-milk fasting vermicelli (@renaissance_food_italian_vermicelli_almond_fast)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodAndDrinkPrepareEarlyJapaneseCastellaCake() => new()
	{
		Metadata = new("Food and drink/prepare early Japanese castella cake", "crafts", ["renaissance"], ["item:renaissance_food_japanese_castella_cake"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "prepare early Japanese castella cake",
		Category = "Food and drink",
		Blurb = "prepare an early Japanese castella cake",
		Action = "preparing a food or drink item",
		ActiveCraftItemSdesc = "an in-progress early Japanese castella cake craft",
		Gate = CatalogueRecipeGates.RenaissanceFinishedItemCraftingCooking10,
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
			new(25, "$0 sort|sorts and prepare|prepares $i1 beside $t1.", "$0 sort|sorts and prepare|prepares $i1 beside $t1.", ExertionLevel.Stasis, 0),
			new(40, "$0 tend|tends the heat and work|works the prepared material into its final form. $0 measure|measures the developing form against the intended finish.", "$0 tend|tends the heat and work|works the prepared material into its final form. $0 measure|measures the developing form against the intended finish.", ExertionLevel.Stasis, 0),
			new(25, "$0 portion|portions and set|sets aside $p1 for serving or storage.", "$0 find|finds the batch spoiled and set|sets the material aside.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 338.0, Material: "wheat", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("338 grams of wheat", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hot Fire", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Hot Fire tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_food_japanese_castella_cake", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x an early Japanese castella cake (@renaissance_food_japanese_castella_cake)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodAndDrinkPrepareMisoGlazedGrilledRiceCake() => new()
	{
		Metadata = new("Food and drink/prepare miso-glazed grilled rice cake", "crafts", ["renaissance"], ["item:renaissance_food_japanese_grilled_miso_rice_cake"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "prepare miso-glazed grilled rice cake",
		Category = "Food and drink",
		Blurb = "prepare a miso-glazed grilled rice cake",
		Action = "preparing a food or drink item",
		ActiveCraftItemSdesc = "an in-progress miso-glazed grilled rice cake craft",
		Gate = CatalogueRecipeGates.RenaissanceFinishedItemCraftingCooking10,
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
			new(25, "$0 sort|sorts and prepare|prepares $i1 beside $t1.", "$0 sort|sorts and prepare|prepares $i1 beside $t1.", ExertionLevel.Stasis, 0),
			new(40, "$0 tend|tends the heat and work|works the prepared material into its final form. $0 clear|clears the work area before the final steps.", "$0 tend|tends the heat and work|works the prepared material into its final form. $0 clear|clears the work area before the final steps.", ExertionLevel.Stasis, 0),
			new(25, "$0 portion|portions and set|sets aside $p1 for serving or storage.", "$0 find|finds the batch spoiled and set|sets the material aside.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 165.0, Material: "rice", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("165 grams of rice", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hot Fire", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Hot Fire tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_food_japanese_grilled_miso_rice_cake", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a miso-glazed grilled rice cake (@renaissance_food_japanese_grilled_miso_rice_cake)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodAndDrinkPrepareMisoPickledTofu() => new()
	{
		Metadata = new("Food and drink/prepare miso-pickled tofu", "crafts", ["renaissance"], ["item:renaissance_food_japanese_miso_pickled_tofu"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "prepare miso-pickled tofu",
		Category = "Food and drink",
		Blurb = "prepare miso-pickled tofu",
		Action = "preparing a food or drink item",
		ActiveCraftItemSdesc = "an in-progress miso-pickled tofu craft",
		Gate = CatalogueRecipeGates.RenaissanceFinishedItemCraftingCooking10,
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
			new(25, "$0 sort|sorts and prepare|prepares $i1 beside $t1.", "$0 sort|sorts and prepare|prepares $i1 beside $t1.", ExertionLevel.Stasis, 0),
			new(40, "$0 tend|tends the heat and work|works the prepared material into its final form. $0 pause|pauses to check the proportions before carrying on.", "$0 tend|tends the heat and work|works the prepared material into its final form. $0 pause|pauses to check the proportions before carrying on.", ExertionLevel.Stasis, 0),
			new(25, "$0 portion|portions and set|sets aside $p1 for serving or storage.", "$0 find|finds the batch spoiled and set|sets the material aside.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 165.0, Material: "vegetable", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("165 grams of vegetable", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hot Fire", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Hot Fire tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_food_japanese_miso_pickled_tofu", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x miso-pickled tofu (@renaissance_food_japanese_miso_pickled_tofu)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodAndDrinkPrepareFilledMochiTeaSweet() => new()
	{
		Metadata = new("Food and drink/prepare filled mochi tea sweet", "crafts", ["renaissance"], ["item:renaissance_food_japanese_mochi_tea_sweet"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "prepare filled mochi tea sweet",
		Category = "Food and drink",
		Blurb = "prepare a filled mochi tea sweet",
		Action = "preparing a food or drink item",
		ActiveCraftItemSdesc = "an in-progress filled mochi tea sweet craft",
		Gate = CatalogueRecipeGates.RenaissanceFinishedItemCraftingCooking10,
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
			new(25, "$0 sort|sorts and prepare|prepares $i1 beside $t1.", "$0 sort|sorts and prepare|prepares $i1 beside $t1.", ExertionLevel.Stasis, 0),
			new(40, "$0 tend|tends the heat and work|works the prepared material into its final form. $0 measure|measures the developing form against the intended finish.", "$0 tend|tends the heat and work|works the prepared material into its final form. $0 measure|measures the developing form against the intended finish.", ExertionLevel.Stasis, 0),
			new(25, "$0 portion|portions and set|sets aside $p1 for serving or storage.", "$0 find|finds the batch spoiled and set|sets the material aside.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 105.0, Material: "rice", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("105 grams of rice", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hot Fire", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Hot Fire tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_food_japanese_mochi_tea_sweet", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a filled mochi tea sweet (@renaissance_food_japanese_mochi_tea_sweet)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodAndDrinkPrepareNanbanStyleFriedVegetables() => new()
	{
		Metadata = new("Food and drink/prepare Nanban-style fried vegetables", "crafts", ["renaissance"], ["item:renaissance_food_japanese_nanban_fried_vegetables"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "prepare Nanban-style fried vegetables",
		Category = "Food and drink",
		Blurb = "prepare Nanban-style fried vegetables",
		Action = "preparing a food or drink item",
		ActiveCraftItemSdesc = "an in-progress Nanban-style fried vegetables craft",
		Gate = CatalogueRecipeGates.RenaissanceFinishedItemCraftingCooking10,
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
			new(25, "$0 sort|sorts and prepare|prepares $i1 beside $t1.", "$0 sort|sorts and prepare|prepares $i1 beside $t1.", ExertionLevel.Stasis, 0),
			new(40, "$0 tend|tends the heat and work|works the prepared material into its final form. $0 measure|measures the developing form against the intended finish.", "$0 tend|tends the heat and work|works the prepared material into its final form. $0 measure|measures the developing form against the intended finish.", ExertionLevel.Stasis, 0),
			new(25, "$0 portion|portions and set|sets aside $p1 for serving or storage.", "$0 find|finds the batch spoiled and set|sets the material aside.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 210.0, Material: "vegetable", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("210 grams of vegetable", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hot Fire", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Hot Fire tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_food_japanese_nanban_fried_vegetables", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x Nanban-style fried vegetables (@renaissance_food_japanese_nanban_fried_vegetables)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodAndDrinkPrepareNanbanSugarComfits() => new()
	{
		Metadata = new("Food and drink/prepare Nanban sugar comfits", "crafts", ["renaissance"], ["item:renaissance_food_japanese_nanban_sugar_comfits"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "prepare Nanban sugar comfits",
		Category = "Food and drink",
		Blurb = "prepare Nanban sugar comfits",
		Action = "preparing a food or drink item",
		ActiveCraftItemSdesc = "an in-progress Nanban sugar comfits craft",
		Gate = CatalogueRecipeGates.RenaissanceFinishedItemCraftingCooking10,
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
			new(25, "$0 sort|sorts and prepare|prepares $i1 beside $t1.", "$0 sort|sorts and prepare|prepares $i1 beside $t1.", ExertionLevel.Stasis, 0),
			new(40, "$0 tend|tends the heat and work|works the prepared material into its final form. $0 turn|turns the work over to judge its balance.", "$0 tend|tends the heat and work|works the prepared material into its final form. $0 turn|turns the work over to judge its balance.", ExertionLevel.Stasis, 0),
			new(25, "$0 portion|portions and set|sets aside $p1 for serving or storage.", "$0 find|finds the batch spoiled and set|sets the material aside.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 75.0, Material: "honey", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("75 grams of honey", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hot Fire", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Hot Fire tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_food_japanese_nanban_sugar_comfits", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x Nanban sugar comfits (@renaissance_food_japanese_nanban_sugar_comfits)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodAndDrinkPrepareEarlyNanbanStyleFriedFish() => new()
	{
		Metadata = new("Food and drink/prepare early Nanban-style fried fish", "crafts", ["renaissance"], ["item:renaissance_food_japanese_nanban_tempura"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "prepare early Nanban-style fried fish",
		Category = "Food and drink",
		Blurb = "prepare early Nanban-style fried fish",
		Action = "preparing a food or drink item",
		ActiveCraftItemSdesc = "an in-progress early Nanban-style fried fish craft",
		Gate = CatalogueRecipeGates.RenaissanceFinishedItemCraftingCooking10,
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
			new(25, "$0 sort|sorts and prepare|prepares $i1 beside $t1.", "$0 sort|sorts and prepare|prepares $i1 beside $t1.", ExertionLevel.Stasis, 0),
			new(40, "$0 tend|tends the heat and work|works the prepared material into its final form. $0 clear|clears the work area before the final steps.", "$0 tend|tends the heat and work|works the prepared material into its final form. $0 clear|clears the work area before the final steps.", ExertionLevel.Stasis, 0),
			new(25, "$0 portion|portions and set|sets aside $p1 for serving or storage.", "$0 find|finds the batch spoiled and set|sets the material aside.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 225.0, Material: "fish", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("225 grams of fish", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hot Fire", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Hot Fire tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_food_japanese_nanban_tempura", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x early Nanban-style fried fish (@renaissance_food_japanese_nanban_tempura)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodAndDrinkPrepareShojinGreensWithSesameDressing() => new()
	{
		Metadata = new("Food and drink/prepare shojin greens with sesame dressing", "crafts", ["renaissance"], ["item:renaissance_food_japanese_shojin_sesame_dressing"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "prepare shojin greens with sesame dressing",
		Category = "Food and drink",
		Blurb = "prepare shojin greens with sesame dressing",
		Action = "preparing a food or drink item",
		ActiveCraftItemSdesc = "an in-progress shojin greens with sesame dressing craft",
		Gate = CatalogueRecipeGates.RenaissanceFinishedItemCraftingCooking10,
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
			new(25, "$0 sort|sorts and prepare|prepares $i1 beside $t1.", "$0 sort|sorts and prepare|prepares $i1 beside $t1.", ExertionLevel.Stasis, 0),
			new(40, "$0 tend|tends the heat and work|works the prepared material into its final form. $0 measure|measures the developing form against the intended finish.", "$0 tend|tends the heat and work|works the prepared material into its final form. $0 measure|measures the developing form against the intended finish.", ExertionLevel.Stasis, 0),
			new(25, "$0 portion|portions and set|sets aside $p1 for serving or storage.", "$0 find|finds the batch spoiled and set|sets the material aside.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 225.0, Material: "vegetable", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("225 grams of vegetable", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hot Fire", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Hot Fire tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_food_japanese_shojin_sesame_dressing", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x shojin greens with sesame dressing (@renaissance_food_japanese_shojin_sesame_dressing)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodAndDrinkPrepareSeasonalTeaCeremonySweet() => new()
	{
		Metadata = new("Food and drink/prepare seasonal tea-ceremony sweet", "crafts", ["renaissance"], ["item:renaissance_food_japanese_tea_ceremony_wagashi"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "prepare seasonal tea-ceremony sweet",
		Category = "Food and drink",
		Blurb = "prepare a seasonal tea-ceremony sweet",
		Action = "preparing a food or drink item",
		ActiveCraftItemSdesc = "an in-progress seasonal tea-ceremony sweet craft",
		Gate = CatalogueRecipeGates.RenaissanceFinishedItemCraftingCooking10,
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
			new(25, "$0 sort|sorts and prepare|prepares $i1 beside $t1.", "$0 sort|sorts and prepare|prepares $i1 beside $t1.", ExertionLevel.Stasis, 0),
			new(40, "$0 tend|tends the heat and work|works the prepared material into its final form. $0 pause|pauses to check the proportions before carrying on.", "$0 tend|tends the heat and work|works the prepared material into its final form. $0 pause|pauses to check the proportions before carrying on.", ExertionLevel.Stasis, 0),
			new(25, "$0 portion|portions and set|sets aside $p1 for serving or storage.", "$0 find|finds the batch spoiled and set|sets the material aside.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 75.0, Material: "food", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("75 grams of food", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hot Fire", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Hot Fire tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_food_japanese_tea_ceremony_wagashi", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a seasonal tea-ceremony sweet (@renaissance_food_japanese_tea_ceremony_wagashi)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodAndDrinkPrepareWarriorSCompartmentedRiceRation() => new()
	{
		Metadata = new("Food and drink/prepare warrior's compartmented rice ration", "crafts", ["renaissance"], ["item:renaissance_food_japanese_warrior_rice_ration_box"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "prepare warrior's compartmented rice ration",
		Category = "Food and drink",
		Blurb = "prepare a warrior's compartmented rice ration",
		Action = "preparing a food or drink item",
		ActiveCraftItemSdesc = "an in-progress warrior's compartmented rice ration craft",
		Gate = CatalogueRecipeGates.RenaissanceFinishedItemCraftingCooking10,
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
			new(25, "$0 sort|sorts and prepare|prepares $i1 beside $t1.", "$0 sort|sorts and prepare|prepares $i1 beside $t1.", ExertionLevel.Stasis, 0),
			new(40, "$0 tend|tends the heat and work|works the prepared material into its final form. $0 measure|measures the developing form against the intended finish.", "$0 tend|tends the heat and work|works the prepared material into its final form. $0 measure|measures the developing form against the intended finish.", ExertionLevel.Stasis, 0),
			new(25, "$0 portion|portions and set|sets aside $p1 for serving or storage.", "$0 find|finds the batch spoiled and set|sets the material aside.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 488.0, Material: "rice", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("488 grams of rice", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hot Fire", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Hot Fire tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_food_japanese_warrior_rice_ration_box", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a warrior's compartmented rice ration (@renaissance_food_japanese_warrior_rice_ration_box)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodAndDrinkPrepareJoseonManduCourtHotpot() => new()
	{
		Metadata = new("Food and drink/prepare Joseon mandu court hotpot", "crafts", ["renaissance"], ["item:renaissance_food_joseon_mandu_court_hotpot"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "prepare Joseon mandu court hotpot",
		Category = "Food and drink",
		Blurb = "prepare a Joseon mandu court hotpot",
		Action = "preparing a food or drink item",
		ActiveCraftItemSdesc = "an in-progress Joseon mandu court hotpot craft",
		Gate = CatalogueRecipeGates.RenaissanceFinishedItemCraftingCooking10,
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
			new(25, "$0 sort|sorts and prepare|prepares $i1 beside $t1.", "$0 sort|sorts and prepare|prepares $i1 beside $t1.", ExertionLevel.Stasis, 0),
			new(40, "$0 tend|tends the heat and work|works the prepared material into its final form. $0 turn|turns the work over to judge its balance.", "$0 tend|tends the heat and work|works the prepared material into its final form. $0 turn|turns the work over to judge its balance.", ExertionLevel.Stasis, 0),
			new(25, "$0 portion|portions and set|sets aside $p1 for serving or storage.", "$0 find|finds the batch spoiled and set|sets the material aside.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 600.0, Material: "meat", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("600 grams of meat", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hot Fire", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Hot Fire tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_food_joseon_mandu_court_hotpot", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a Joseon mandu court hotpot (@renaissance_food_joseon_mandu_court_hotpot)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodAndDrinkPrepareExperimentalJoseonChilePreserve() => new()
	{
		Metadata = new("Food and drink/prepare experimental Joseon chile preserve", "crafts", ["renaissance"], ["item:renaissance_food_joseon_new_chile_preserve"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "prepare experimental Joseon chile preserve",
		Category = "Food and drink",
		Blurb = "prepare an experimental Joseon chile preserve",
		Action = "preparing a food or drink item",
		ActiveCraftItemSdesc = "an in-progress experimental Joseon chile preserve craft",
		Gate = CatalogueRecipeGates.RenaissanceFinishedItemCraftingCooking10,
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
			new(25, "$0 sort|sorts and prepare|prepares $i1 beside $t1.", "$0 sort|sorts and prepare|prepares $i1 beside $t1.", ExertionLevel.Stasis, 0),
			new(40, "$0 tend|tends the heat and work|works the prepared material into its final form. $0 measure|measures the developing form against the intended finish.", "$0 tend|tends the heat and work|works the prepared material into its final form. $0 measure|measures the developing form against the intended finish.", ExertionLevel.Stasis, 0),
			new(25, "$0 portion|portions and set|sets aside $p1 for serving or storage.", "$0 find|finds the batch spoiled and set|sets the material aside.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 135.0, Material: "vegetable", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("135 grams of vegetable", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hot Fire", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Hot Fire tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_food_joseon_new_chile_preserve", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x an experimental Joseon chile preserve (@renaissance_food_joseon_new_chile_preserve)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodAndDrinkPrepareJoseonNutAndFruitGangjeong() => new()
	{
		Metadata = new("Food and drink/prepare Joseon nut-and-fruit gangjeong", "crafts", ["renaissance"], ["item:renaissance_food_joseon_nut_fruit_gangjeong"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "prepare Joseon nut-and-fruit gangjeong",
		Category = "Food and drink",
		Blurb = "prepare Joseon nut-and-fruit gangjeong",
		Action = "preparing a food or drink item",
		ActiveCraftItemSdesc = "an in-progress Joseon nut-and-fruit gangjeong craft",
		Gate = CatalogueRecipeGates.RenaissanceFinishedItemCraftingCooking10,
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
			new(25, "$0 sort|sorts and prepare|prepares $i1 beside $t1.", "$0 sort|sorts and prepare|prepares $i1 beside $t1.", ExertionLevel.Stasis, 0),
			new(40, "$0 tend|tends the heat and work|works the prepared material into its final form. $0 pause|pauses to check the proportions before carrying on.", "$0 tend|tends the heat and work|works the prepared material into its final form. $0 pause|pauses to check the proportions before carrying on.", ExertionLevel.Stasis, 0),
			new(25, "$0 portion|portions and set|sets aside $p1 for serving or storage.", "$0 find|finds the batch spoiled and set|sets the material aside.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 180.0, Material: "honey", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("180 grams of honey", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hot Fire", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Hot Fire tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_food_joseon_nut_fruit_gangjeong", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x Joseon nut-and-fruit gangjeong (@renaissance_food_joseon_nut_fruit_gangjeong)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodAndDrinkPrepareJoseonRoyalRiceCakeMosaic() => new()
	{
		Metadata = new("Food and drink/prepare Joseon royal rice-cake mosaic", "crafts", ["renaissance"], ["item:renaissance_food_joseon_royal_rice_cake_mosaic"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "prepare Joseon royal rice-cake mosaic",
		Category = "Food and drink",
		Blurb = "prepare a Joseon royal rice-cake mosaic",
		Action = "preparing a food or drink item",
		ActiveCraftItemSdesc = "an in-progress Joseon royal rice-cake mosaic craft",
		Gate = CatalogueRecipeGates.RenaissanceFinishedItemCraftingCooking10,
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
			new(25, "$0 sort|sorts and prepare|prepares $i1 beside $t1.", "$0 sort|sorts and prepare|prepares $i1 beside $t1.", ExertionLevel.Stasis, 0),
			new(40, "$0 tend|tends the heat and work|works the prepared material into its final form. $0 pause|pauses to check the proportions before carrying on.", "$0 tend|tends the heat and work|works the prepared material into its final form. $0 pause|pauses to check the proportions before carrying on.", ExertionLevel.Stasis, 0),
			new(25, "$0 portion|portions and set|sets aside $p1 for serving or storage.", "$0 find|finds the batch spoiled and set|sets the material aside.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 525.0, Material: "rice", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("525 grams of rice", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hot Fire", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Hot Fire tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_food_joseon_royal_rice_cake_mosaic", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a Joseon royal rice-cake mosaic (@renaissance_food_joseon_royal_rice_cake_mosaic)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodAndDrinkPrepareJoseonRoyalSinseollo() => new()
	{
		Metadata = new("Food and drink/prepare Joseon royal sinseollo", "crafts", ["renaissance"], ["item:renaissance_food_joseon_royal_sinseollo"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "prepare Joseon royal sinseollo",
		Category = "Food and drink",
		Blurb = "prepare a Joseon royal sinseollo",
		Action = "preparing a food or drink item",
		ActiveCraftItemSdesc = "an in-progress Joseon royal sinseollo craft",
		Gate = CatalogueRecipeGates.RenaissanceFinishedItemCraftingCooking10,
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
			new(25, "$0 sort|sorts and prepare|prepares $i1 beside $t1.", "$0 sort|sorts and prepare|prepares $i1 beside $t1.", ExertionLevel.Stasis, 0),
			new(40, "$0 tend|tends the heat and work|works the prepared material into its final form. $0 turn|turns the work over to judge its balance.", "$0 tend|tends the heat and work|works the prepared material into its final form. $0 turn|turns the work over to judge its balance.", ExertionLevel.Stasis, 0),
			new(25, "$0 portion|portions and set|sets aside $p1 for serving or storage.", "$0 find|finds the batch spoiled and set|sets the material aside.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 750.0, Material: "food", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("750 grams of food", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hot Fire", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Hot Fire tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_food_joseon_royal_sinseollo", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a Joseon royal sinseollo (@renaissance_food_joseon_royal_sinseollo)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodAndDrinkPrepareFlowerShapedJoseonYakgwa() => new()
	{
		Metadata = new("Food and drink/prepare flower-shaped Joseon yakgwa", "crafts", ["renaissance"], ["item:renaissance_food_joseon_yakgwa_flower_cakes"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "prepare flower-shaped Joseon yakgwa",
		Category = "Food and drink",
		Blurb = "prepare flower-shaped Joseon yakgwa",
		Action = "preparing a food or drink item",
		ActiveCraftItemSdesc = "an in-progress flower-shaped Joseon yakgwa craft",
		Gate = CatalogueRecipeGates.RenaissanceFinishedItemCraftingCooking10,
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
			new(25, "$0 sort|sorts and prepare|prepares $i1 beside $t1.", "$0 sort|sorts and prepare|prepares $i1 beside $t1.", ExertionLevel.Stasis, 0),
			new(40, "$0 tend|tends the heat and work|works the prepared material into its final form. $0 clear|clears the work area before the final steps.", "$0 tend|tends the heat and work|works the prepared material into its final form. $0 clear|clears the work area before the final steps.", ExertionLevel.Stasis, 0),
			new(25, "$0 portion|portions and set|sets aside $p1 for serving or storage.", "$0 find|finds the batch spoiled and set|sets the material aside.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 188.0, Material: "wheat", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("188 grams of wheat", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hot Fire", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Hot Fire tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_food_joseon_yakgwa_flower_cakes", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x flower-shaped Joseon yakgwa (@renaissance_food_joseon_yakgwa_flower_cakes)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodAndDrinkPrepareKongoEliteLeafWrappedMeat() => new()
	{
		Metadata = new("Food and drink/prepare Kongo elite leaf-wrapped meat", "crafts", ["renaissance"], ["item:renaissance_food_kongo_elite_leaf_wrapped_meat"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "prepare Kongo elite leaf-wrapped meat",
		Category = "Food and drink",
		Blurb = "prepare Kongo elite leaf-wrapped meat",
		Action = "preparing a food or drink item",
		ActiveCraftItemSdesc = "an in-progress Kongo elite leaf-wrapped meat craft",
		Gate = CatalogueRecipeGates.RenaissanceFinishedItemCraftingCooking10,
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
			new(25, "$0 sort|sorts and prepare|prepares $i1 beside $t1.", "$0 sort|sorts and prepare|prepares $i1 beside $t1.", ExertionLevel.Stasis, 0),
			new(40, "$0 tend|tends the heat and work|works the prepared material into its final form. $0 turn|turns the work over to judge its balance.", "$0 tend|tends the heat and work|works the prepared material into its final form. $0 turn|turns the work over to judge its balance.", ExertionLevel.Stasis, 0),
			new(25, "$0 portion|portions and set|sets aside $p1 for serving or storage.", "$0 find|finds the batch spoiled and set|sets the material aside.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 525.0, Material: "meat", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("525 grams of meat", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hot Fire", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Hot Fire tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_food_kongo_elite_leaf_wrapped_meat", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x Kongo elite leaf-wrapped meat (@renaissance_food_kongo_elite_leaf_wrapped_meat)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodAndDrinkPrepareKongoHoneySorghumFeastCake() => new()
	{
		Metadata = new("Food and drink/prepare Kongo honey-sorghum feast cake", "crafts", ["renaissance"], ["item:renaissance_food_kongo_honey_sorghum_cake"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "prepare Kongo honey-sorghum feast cake",
		Category = "Food and drink",
		Blurb = "prepare a Kongo honey-sorghum feast cake",
		Action = "preparing a food or drink item",
		ActiveCraftItemSdesc = "an in-progress Kongo honey-sorghum feast cake craft",
		Gate = CatalogueRecipeGates.RenaissanceFinishedItemCraftingCooking10,
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
			new(25, "$0 sort|sorts and prepare|prepares $i1 beside $t1.", "$0 sort|sorts and prepare|prepares $i1 beside $t1.", ExertionLevel.Stasis, 0),
			new(40, "$0 tend|tends the heat and work|works the prepared material into its final form. $0 turn|turns the work over to judge its balance.", "$0 tend|tends the heat and work|works the prepared material into its final form. $0 turn|turns the work over to judge its balance.", ExertionLevel.Stasis, 0),
			new(25, "$0 portion|portions and set|sets aside $p1 for serving or storage.", "$0 find|finds the batch spoiled and set|sets the material aside.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 375.0, Material: "sorghum", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("375 grams of sorghum", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hot Fire", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Hot Fire tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_food_kongo_honey_sorghum_cake", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a Kongo honey-sorghum feast cake (@renaissance_food_kongo_honey_sorghum_cake)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodAndDrinkPrepareGroundSeedLeafSauce() => new()
	{
		Metadata = new("Food and drink/prepare ground-seed leaf sauce", "crafts", ["renaissance"], ["item:renaissance_food_mande_ground_seed_leaf_sauce"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "prepare ground-seed leaf sauce",
		Category = "Food and drink",
		Blurb = "prepare a ground-seed leaf sauce",
		Action = "preparing a food or drink item",
		ActiveCraftItemSdesc = "an in-progress ground-seed leaf sauce craft",
		Gate = CatalogueRecipeGates.RenaissanceFinishedItemCraftingCooking10,
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
			new(25, "$0 sort|sorts and prepare|prepares $i1 beside $t1.", "$0 sort|sorts and prepare|prepares $i1 beside $t1.", ExertionLevel.Stasis, 0),
			new(40, "$0 tend|tends the heat and work|works the prepared material into its final form. $0 measure|measures the developing form against the intended finish.", "$0 tend|tends the heat and work|works the prepared material into its final form. $0 measure|measures the developing form against the intended finish.", ExertionLevel.Stasis, 0),
			new(25, "$0 portion|portions and set|sets aside $p1 for serving or storage.", "$0 find|finds the batch spoiled and set|sets the material aside.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 375.0, Material: "vegetable", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("375 grams of vegetable", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hot Fire", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Hot Fire tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_food_mande_ground_seed_leaf_sauce", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a ground-seed leaf sauce (@renaissance_food_mande_ground_seed_leaf_sauce)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodAndDrinkPrepareMayaPitCookedTurkeyPacket() => new()
	{
		Metadata = new("Food and drink/prepare Maya pit-cooked turkey packet", "crafts", ["renaissance"], ["item:renaissance_food_maya_achiote_turkey_pit_packet"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "prepare Maya pit-cooked turkey packet",
		Category = "Food and drink",
		Blurb = "prepare a Maya pit-cooked turkey packet",
		Action = "preparing a food or drink item",
		ActiveCraftItemSdesc = "an in-progress Maya pit-cooked turkey packet craft",
		Gate = CatalogueRecipeGates.RenaissanceFinishedItemCraftingCooking10,
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
			new(25, "$0 sort|sorts and prepare|prepares $i1 beside $t1.", "$0 sort|sorts and prepare|prepares $i1 beside $t1.", ExertionLevel.Stasis, 0),
			new(40, "$0 tend|tends the heat and work|works the prepared material into its final form. $0 pause|pauses to check the proportions before carrying on.", "$0 tend|tends the heat and work|works the prepared material into its final form. $0 pause|pauses to check the proportions before carrying on.", ExertionLevel.Stasis, 0),
			new(25, "$0 portion|portions and set|sets aside $p1 for serving or storage.", "$0 find|finds the batch spoiled and set|sets the material aside.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 900.0, Material: "meat", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("900 grams of meat", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hot Fire", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Hot Fire tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_food_maya_achiote_turkey_pit_packet", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a Maya pit-cooked turkey packet (@renaissance_food_maya_achiote_turkey_pit_packet)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodAndDrinkPrepareCacaoHoneyMayaTamales() => new()
	{
		Metadata = new("Food and drink/prepare cacao-honey Maya tamales", "crafts", ["renaissance"], ["item:renaissance_food_maya_cacao_honey_tamales"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "prepare cacao-honey Maya tamales",
		Category = "Food and drink",
		Blurb = "prepare cacao-honey Maya tamales",
		Action = "preparing a food or drink item",
		ActiveCraftItemSdesc = "an in-progress cacao-honey Maya tamales craft",
		Gate = CatalogueRecipeGates.RenaissanceFinishedItemCraftingCooking10,
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
			new(25, "$0 sort|sorts and prepare|prepares $i1 beside $t1.", "$0 sort|sorts and prepare|prepares $i1 beside $t1.", ExertionLevel.Stasis, 0),
			new(40, "$0 tend|tends the heat and work|works the prepared material into its final form. $0 measure|measures the developing form against the intended finish.", "$0 tend|tends the heat and work|works the prepared material into its final form. $0 measure|measures the developing form against the intended finish.", ExertionLevel.Stasis, 0),
			new(25, "$0 portion|portions and set|sets aside $p1 for serving or storage.", "$0 find|finds the batch spoiled and set|sets the material aside.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 300.0, Material: "corn", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("300 grams of corn", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hot Fire", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Hot Fire tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_food_maya_cacao_honey_tamales", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x cacao-honey Maya tamales (@renaissance_food_maya_cacao_honey_tamales)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodAndDrinkPrepareFishInMayaSeedSauce() => new()
	{
		Metadata = new("Food and drink/prepare fish in Maya seed sauce", "crafts", ["renaissance"], ["item:renaissance_food_maya_pumpkin_seed_sauce_fish"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "prepare fish in Maya seed sauce",
		Category = "Food and drink",
		Blurb = "prepare fish in Maya seed sauce",
		Action = "preparing a food or drink item",
		ActiveCraftItemSdesc = "an in-progress fish in Maya seed sauce craft",
		Gate = CatalogueRecipeGates.RenaissanceFinishedItemCraftingCooking10,
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
			new(25, "$0 sort|sorts and prepare|prepares $i1 beside $t1.", "$0 sort|sorts and prepare|prepares $i1 beside $t1.", ExertionLevel.Stasis, 0),
			new(40, "$0 tend|tends the heat and work|works the prepared material into its final form. $0 measure|measures the developing form against the intended finish.", "$0 tend|tends the heat and work|works the prepared material into its final form. $0 measure|measures the developing form against the intended finish.", ExertionLevel.Stasis, 0),
			new(25, "$0 portion|portions and set|sets aside $p1 for serving or storage.", "$0 find|finds the batch spoiled and set|sets the material aside.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 413.0, Material: "fish", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("413 grams of fish", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hot Fire", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Hot Fire tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_food_maya_pumpkin_seed_sauce_fish", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x fish in Maya seed sauce (@renaissance_food_maya_pumpkin_seed_sauce_fish)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodAndDrinkPrepareMingBuddhistMockGoose() => new()
	{
		Metadata = new("Food and drink/prepare Ming Buddhist mock goose", "crafts", ["renaissance"], ["item:renaissance_food_ming_buddhist_mock_goose"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "prepare Ming Buddhist mock goose",
		Category = "Food and drink",
		Blurb = "prepare a Ming Buddhist mock goose",
		Action = "preparing a food or drink item",
		ActiveCraftItemSdesc = "an in-progress Ming Buddhist mock goose craft",
		Gate = CatalogueRecipeGates.RenaissanceFinishedItemCraftingCooking10,
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
			new(25, "$0 sort|sorts and prepare|prepares $i1 beside $t1.", "$0 sort|sorts and prepare|prepares $i1 beside $t1.", ExertionLevel.Stasis, 0),
			new(40, "$0 tend|tends the heat and work|works the prepared material into its final form. $0 pause|pauses to check the proportions before carrying on.", "$0 tend|tends the heat and work|works the prepared material into its final form. $0 pause|pauses to check the proportions before carrying on.", ExertionLevel.Stasis, 0),
			new(25, "$0 portion|portions and set|sets aside $p1 for serving or storage.", "$0 find|finds the batch spoiled and set|sets the material aside.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 413.0, Material: "vegetable", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("413 grams of vegetable", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hot Fire", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Hot Fire tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_food_ming_buddhist_mock_goose", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a Ming Buddhist mock goose (@renaissance_food_ming_buddhist_mock_goose)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodAndDrinkPrepareMingCandiedWinterFruit() => new()
	{
		Metadata = new("Food and drink/prepare Ming candied winter fruit", "crafts", ["renaissance"], ["item:renaissance_food_ming_candied_winter_fruit"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "prepare Ming candied winter fruit",
		Category = "Food and drink",
		Blurb = "prepare Ming candied winter fruit",
		Action = "preparing a food or drink item",
		ActiveCraftItemSdesc = "an in-progress Ming candied winter fruit craft",
		Gate = CatalogueRecipeGates.RenaissanceFinishedItemCraftingCooking10,
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
			new(25, "$0 sort|sorts and prepare|prepares $i1 beside $t1.", "$0 sort|sorts and prepare|prepares $i1 beside $t1.", ExertionLevel.Stasis, 0),
			new(40, "$0 tend|tends the heat and work|works the prepared material into its final form. $0 clear|clears the work area before the final steps.", "$0 tend|tends the heat and work|works the prepared material into its final form. $0 clear|clears the work area before the final steps.", ExertionLevel.Stasis, 0),
			new(25, "$0 portion|portions and set|sets aside $p1 for serving or storage.", "$0 find|finds the batch spoiled and set|sets the material aside.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 150.0, Material: "food", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("150 grams of food", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hot Fire", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Hot Fire tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_food_ming_candied_winter_fruit", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x Ming candied winter fruit (@renaissance_food_ming_candied_winter_fruit)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodAndDrinkPrepareDragonWhiskerSugarSweet() => new()
	{
		Metadata = new("Food and drink/prepare dragon-whisker sugar sweet", "crafts", ["renaissance"], ["item:renaissance_food_ming_dragon_whisker_sweet"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "prepare dragon-whisker sugar sweet",
		Category = "Food and drink",
		Blurb = "prepare a dragon-whisker sugar sweet",
		Action = "preparing a food or drink item",
		ActiveCraftItemSdesc = "an in-progress dragon-whisker sugar sweet craft",
		Gate = CatalogueRecipeGates.RenaissanceFinishedItemCraftingCooking10,
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
			new(25, "$0 sort|sorts and prepare|prepares $i1 beside $t1.", "$0 sort|sorts and prepare|prepares $i1 beside $t1.", ExertionLevel.Stasis, 0),
			new(40, "$0 tend|tends the heat and work|works the prepared material into its final form. $0 pause|pauses to check the proportions before carrying on.", "$0 tend|tends the heat and work|works the prepared material into its final form. $0 pause|pauses to check the proportions before carrying on.", ExertionLevel.Stasis, 0),
			new(25, "$0 portion|portions and set|sets aside $p1 for serving or storage.", "$0 find|finds the batch spoiled and set|sets the material aside.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 90.0, Material: "honey", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("90 grams of honey", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hot Fire", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Hot Fire tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_food_ming_dragon_whisker_sweet", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a dragon-whisker sugar sweet (@renaissance_food_ming_dragon_whisker_sweet)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodAndDrinkPrepareDuckSkinPancakeService() => new()
	{
		Metadata = new("Food and drink/prepare duck-skin pancake service", "crafts", ["renaissance"], ["item:renaissance_food_ming_duck_skin_pancake_service"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "prepare duck-skin pancake service",
		Category = "Food and drink",
		Blurb = "prepare a duck-skin pancake service",
		Action = "preparing a food or drink item",
		ActiveCraftItemSdesc = "an in-progress duck-skin pancake service craft",
		Gate = CatalogueRecipeGates.RenaissanceFinishedItemCraftingCooking10,
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
			new(25, "$0 sort|sorts and prepare|prepares $i1 beside $t1.", "$0 sort|sorts and prepare|prepares $i1 beside $t1.", ExertionLevel.Stasis, 0),
			new(40, "$0 tend|tends the heat and work|works the prepared material into its final form. $0 turn|turns the work over to judge its balance.", "$0 tend|tends the heat and work|works the prepared material into its final form. $0 turn|turns the work over to judge its balance.", ExertionLevel.Stasis, 0),
			new(25, "$0 portion|portions and set|sets aside $p1 for serving or storage.", "$0 find|finds the batch spoiled and set|sets the material aside.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 600.0, Material: "meat", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("600 grams of meat", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hot Fire", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Hot Fire tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_food_ming_duck_skin_pancake_service", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a duck-skin pancake service (@renaissance_food_ming_duck_skin_pancake_service)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodAndDrinkPrepareMingEightTreasureRice() => new()
	{
		Metadata = new("Food and drink/prepare Ming eight-treasure rice", "crafts", ["renaissance"], ["item:renaissance_food_ming_eight_treasure_rice"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "prepare Ming eight-treasure rice",
		Category = "Food and drink",
		Blurb = "prepare Ming eight-treasure rice",
		Action = "preparing a food or drink item",
		ActiveCraftItemSdesc = "an in-progress Ming eight-treasure rice craft",
		Gate = CatalogueRecipeGates.RenaissanceFinishedItemCraftingCooking10,
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
			new(25, "$0 sort|sorts and prepare|prepares $i1 beside $t1.", "$0 sort|sorts and prepare|prepares $i1 beside $t1.", ExertionLevel.Stasis, 0),
			new(40, "$0 tend|tends the heat and work|works the prepared material into its final form. $0 clear|clears the work area before the final steps.", "$0 tend|tends the heat and work|works the prepared material into its final form. $0 clear|clears the work area before the final steps.", ExertionLevel.Stasis, 0),
			new(25, "$0 portion|portions and set|sets aside $p1 for serving or storage.", "$0 find|finds the batch spoiled and set|sets the material aside.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 488.0, Material: "rice", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("488 grams of rice", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hot Fire", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Hot Fire tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_food_ming_eight_treasure_rice", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x Ming eight-treasure rice (@renaissance_food_ming_eight_treasure_rice)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodAndDrinkPrepareGlutinousRicePearlBalls() => new()
	{
		Metadata = new("Food and drink/prepare glutinous rice pearl balls", "crafts", ["renaissance"], ["item:renaissance_food_ming_glutinous_rice_pearls"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "prepare glutinous rice pearl balls",
		Category = "Food and drink",
		Blurb = "prepare glutinous rice pearl balls",
		Action = "preparing a food or drink item",
		ActiveCraftItemSdesc = "an in-progress glutinous rice pearl balls craft",
		Gate = CatalogueRecipeGates.RenaissanceFinishedItemCraftingCooking10,
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
			new(25, "$0 sort|sorts and prepare|prepares $i1 beside $t1.", "$0 sort|sorts and prepare|prepares $i1 beside $t1.", ExertionLevel.Stasis, 0),
			new(40, "$0 tend|tends the heat and work|works the prepared material into its final form. $0 measure|measures the developing form against the intended finish.", "$0 tend|tends the heat and work|works the prepared material into its final form. $0 measure|measures the developing form against the intended finish.", ExertionLevel.Stasis, 0),
			new(25, "$0 portion|portions and set|sets aside $p1 for serving or storage.", "$0 find|finds the batch spoiled and set|sets the material aside.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 225.0, Material: "rice", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("225 grams of rice", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hot Fire", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Hot Fire tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_food_ming_glutinous_rice_pearls", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x glutinous rice pearl balls (@renaissance_food_ming_glutinous_rice_pearls)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodAndDrinkPrepareMingKnifeShavedNoodles() => new()
	{
		Metadata = new("Food and drink/prepare Ming knife-shaved noodles", "crafts", ["renaissance"], ["item:renaissance_food_ming_knife_shaved_noodles"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "prepare Ming knife-shaved noodles",
		Category = "Food and drink",
		Blurb = "prepare Ming knife-shaved noodles",
		Action = "preparing a food or drink item",
		ActiveCraftItemSdesc = "an in-progress Ming knife-shaved noodles craft",
		Gate = CatalogueRecipeGates.RenaissanceFinishedItemCraftingCooking10,
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
			new(25, "$0 sort|sorts and prepare|prepares $i1 beside $t1.", "$0 sort|sorts and prepare|prepares $i1 beside $t1.", ExertionLevel.Stasis, 0),
			new(40, "$0 tend|tends the heat and work|works the prepared material into its final form. $0 measure|measures the developing form against the intended finish.", "$0 tend|tends the heat and work|works the prepared material into its final form. $0 measure|measures the developing form against the intended finish.", ExertionLevel.Stasis, 0),
			new(25, "$0 portion|portions and set|sets aside $p1 for serving or storage.", "$0 find|finds the batch spoiled and set|sets the material aside.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 413.0, Material: "wheat", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("413 grams of wheat", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hot Fire", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Hot Fire tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_food_ming_knife_shaved_noodles", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x Ming knife-shaved noodles (@renaissance_food_ming_knife_shaved_noodles)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodAndDrinkPrepareStuffedLotusLikeRootSlices() => new()
	{
		Metadata = new("Food and drink/prepare stuffed lotus-like root slices", "crafts", ["renaissance"], ["item:renaissance_food_ming_lotus_root_stuffed_rice"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "prepare stuffed lotus-like root slices",
		Category = "Food and drink",
		Blurb = "prepare stuffed lotus-like root slices",
		Action = "preparing a food or drink item",
		ActiveCraftItemSdesc = "an in-progress stuffed lotus-like root slices craft",
		Gate = CatalogueRecipeGates.RenaissanceFinishedItemCraftingCooking10,
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
			new(25, "$0 sort|sorts and prepare|prepares $i1 beside $t1.", "$0 sort|sorts and prepare|prepares $i1 beside $t1.", ExertionLevel.Stasis, 0),
			new(40, "$0 tend|tends the heat and work|works the prepared material into its final form. $0 pause|pauses to check the proportions before carrying on.", "$0 tend|tends the heat and work|works the prepared material into its final form. $0 pause|pauses to check the proportions before carrying on.", ExertionLevel.Stasis, 0),
			new(25, "$0 portion|portions and set|sets aside $p1 for serving or storage.", "$0 find|finds the batch spoiled and set|sets the material aside.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 300.0, Material: "vegetable", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("300 grams of vegetable", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hot Fire", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Hot Fire tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_food_ming_lotus_root_stuffed_rice", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x stuffed lotus-like root slices (@renaissance_food_ming_lotus_root_stuffed_rice)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodAndDrinkPrepareUncookedMockGooseRoll() => new()
	{
		Metadata = new("Food and drink/prepare uncooked mock-goose roll", "crafts", ["renaissance"], ["item:renaissance_food_ming_mock_goose_uncooked_roll"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "prepare uncooked mock-goose roll",
		Category = "Food and drink",
		Blurb = "prepare an uncooked mock-goose roll",
		Action = "preparing a food or drink item",
		ActiveCraftItemSdesc = "an in-progress uncooked mock-goose roll craft",
		Gate = CatalogueRecipeGates.RenaissanceFinishedItemCraftingCooking10,
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
			new(25, "$0 sort|sorts and prepare|prepares $i1 beside $t1.", "$0 sort|sorts and prepare|prepares $i1 beside $t1.", ExertionLevel.Stasis, 0),
			new(40, "$0 tend|tends the heat and work|works the prepared material into its final form. $0 clear|clears the work area before the final steps.", "$0 tend|tends the heat and work|works the prepared material into its final form. $0 clear|clears the work area before the final steps.", ExertionLevel.Stasis, 0),
			new(25, "$0 portion|portions and set|sets aside $p1 for serving or storage.", "$0 find|finds the batch spoiled and set|sets the material aside.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 450.0, Material: "vegetable", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("450 grams of vegetable", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hot Fire", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Hot Fire tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_food_ming_mock_goose_uncooked_roll", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x an uncooked mock-goose roll (@renaissance_food_ming_mock_goose_uncooked_roll)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodAndDrinkPrepareLacqueredMingRoastDuck() => new()
	{
		Metadata = new("Food and drink/prepare lacquered Ming roast duck", "crafts", ["renaissance"], ["item:renaissance_food_ming_roast_duck_lacquer"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "prepare lacquered Ming roast duck",
		Category = "Food and drink",
		Blurb = "prepare a lacquered Ming roast duck",
		Action = "preparing a food or drink item",
		ActiveCraftItemSdesc = "an in-progress lacquered Ming roast duck craft",
		Gate = CatalogueRecipeGates.RenaissanceFinishedItemCraftingCooking10,
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
			new(25, "$0 sort|sorts and prepare|prepares $i1 beside $t1.", "$0 sort|sorts and prepare|prepares $i1 beside $t1.", ExertionLevel.Stasis, 0),
			new(40, "$0 tend|tends the heat and work|works the prepared material into its final form. $0 turn|turns the work over to judge its balance.", "$0 tend|tends the heat and work|works the prepared material into its final form. $0 turn|turns the work over to judge its balance.", ExertionLevel.Stasis, 0),
			new(25, "$0 portion|portions and set|sets aside $p1 for serving or storage.", "$0 find|finds the batch spoiled and set|sets the material aside.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 825.0, Material: "meat", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("825 grams of meat", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hot Fire", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Hot Fire tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_food_ming_roast_duck_lacquer", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a lacquered Ming roast duck (@renaissance_food_ming_roast_duck_lacquer)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodAndDrinkPrepareMingSesamePasteNoodles() => new()
	{
		Metadata = new("Food and drink/prepare Ming sesame-paste noodles", "crafts", ["renaissance"], ["item:renaissance_food_ming_sesame_paste_noodles"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "prepare Ming sesame-paste noodles",
		Category = "Food and drink",
		Blurb = "prepare Ming sesame-paste noodles",
		Action = "preparing a food or drink item",
		ActiveCraftItemSdesc = "an in-progress Ming sesame-paste noodles craft",
		Gate = CatalogueRecipeGates.RenaissanceFinishedItemCraftingCooking10,
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
			new(25, "$0 sort|sorts and prepare|prepares $i1 beside $t1.", "$0 sort|sorts and prepare|prepares $i1 beside $t1.", ExertionLevel.Stasis, 0),
			new(40, "$0 tend|tends the heat and work|works the prepared material into its final form. $0 clear|clears the work area before the final steps.", "$0 tend|tends the heat and work|works the prepared material into its final form. $0 clear|clears the work area before the final steps.", ExertionLevel.Stasis, 0),
			new(25, "$0 portion|portions and set|sets aside $p1 for serving or storage.", "$0 find|finds the batch spoiled and set|sets the material aside.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 375.0, Material: "wheat", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("375 grams of wheat", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hot Fire", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Hot Fire tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_food_ming_sesame_paste_noodles", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x Ming sesame-paste noodles (@renaissance_food_ming_sesame_paste_noodles)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodAndDrinkPrepareStampedMingBeanPastry() => new()
	{
		Metadata = new("Food and drink/prepare stamped Ming bean pastry", "crafts", ["renaissance"], ["item:renaissance_food_ming_sweet_bean_pastry"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "prepare stamped Ming bean pastry",
		Category = "Food and drink",
		Blurb = "prepare a stamped Ming bean pastry",
		Action = "preparing a food or drink item",
		ActiveCraftItemSdesc = "an in-progress stamped Ming bean pastry craft",
		Gate = CatalogueRecipeGates.RenaissanceFinishedItemCraftingCooking10,
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
			new(25, "$0 sort|sorts and prepare|prepares $i1 beside $t1.", "$0 sort|sorts and prepare|prepares $i1 beside $t1.", ExertionLevel.Stasis, 0),
			new(40, "$0 tend|tends the heat and work|works the prepared material into its final form. $0 clear|clears the work area before the final steps.", "$0 tend|tends the heat and work|works the prepared material into its final form. $0 clear|clears the work area before the final steps.", ExertionLevel.Stasis, 0),
			new(25, "$0 portion|portions and set|sets aside $p1 for serving or storage.", "$0 find|finds the batch spoiled and set|sets the material aside.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 120.0, Material: "wheat", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("120 grams of wheat", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hot Fire", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Hot Fire tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_food_ming_sweet_bean_pastry", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a stamped Ming bean pastry (@renaissance_food_ming_sweet_bean_pastry)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodAndDrinkPrepareEarlyMingSweetPotatoRation() => new()
	{
		Metadata = new("Food and drink/prepare early Ming sweet-potato ration", "crafts", ["renaissance"], ["item:renaissance_food_ming_sweet_potato_ration"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "prepare early Ming sweet-potato ration",
		Category = "Food and drink",
		Blurb = "prepare an early Ming sweet-potato ration",
		Action = "preparing a food or drink item",
		ActiveCraftItemSdesc = "an in-progress early Ming sweet-potato ration craft",
		Gate = CatalogueRecipeGates.RenaissanceFinishedItemCraftingCooking10,
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
			new(25, "$0 sort|sorts and prepare|prepares $i1 beside $t1.", "$0 sort|sorts and prepare|prepares $i1 beside $t1.", ExertionLevel.Stasis, 0),
			new(40, "$0 tend|tends the heat and work|works the prepared material into its final form. $0 measure|measures the developing form against the intended finish.", "$0 tend|tends the heat and work|works the prepared material into its final form. $0 measure|measures the developing form against the intended finish.", ExertionLevel.Stasis, 0),
			new(25, "$0 portion|portions and set|sets aside $p1 for serving or storage.", "$0 find|finds the batch spoiled and set|sets the material aside.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 263.0, Material: "vegetable", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("263 grams of vegetable", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hot Fire", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Hot Fire tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_food_ming_sweet_potato_ration", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x an early Ming sweet-potato ration (@renaissance_food_ming_sweet_potato_ration)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodAndDrinkPrepareTeaHouseBrothFilledDumplings() => new()
	{
		Metadata = new("Food and drink/prepare tea-house broth-filled dumplings", "crafts", ["renaissance"], ["item:renaissance_food_ming_tea_house_soup_dumplings"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "prepare tea-house broth-filled dumplings",
		Category = "Food and drink",
		Blurb = "prepare tea-house broth-filled dumplings",
		Action = "preparing a food or drink item",
		ActiveCraftItemSdesc = "an in-progress tea-house broth-filled dumplings craft",
		Gate = CatalogueRecipeGates.RenaissanceFinishedItemCraftingCooking10,
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
			new(25, "$0 sort|sorts and prepare|prepares $i1 beside $t1.", "$0 sort|sorts and prepare|prepares $i1 beside $t1.", ExertionLevel.Stasis, 0),
			new(40, "$0 tend|tends the heat and work|works the prepared material into its final form. $0 turn|turns the work over to judge its balance.", "$0 tend|tends the heat and work|works the prepared material into its final form. $0 turn|turns the work over to judge its balance.", ExertionLevel.Stasis, 0),
			new(25, "$0 portion|portions and set|sets aside $p1 for serving or storage.", "$0 find|finds the batch spoiled and set|sets the material aside.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 263.0, Material: "meat", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("263 grams of meat", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hot Fire", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Hot Fire tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_food_ming_tea_house_soup_dumplings", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x tea-house broth-filled dumplings (@renaissance_food_ming_tea_house_soup_dumplings)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};
}
