#nullable enable

using MudSharp.Body;
using MudSharp.GameItems.Inventory.Plans;
using MudSharp.RPG.Checks;

namespace DatabaseSeeder.Seeders;

public static partial class Recipes16Stock
{
	public static CatalogueRecipeDefinition FoodAndDrinkPrepareBagOfUnstrainedMeatJellyStock() => new()
	{
		Metadata = new("Food and drink/prepare bag of unstrained meat-jelly stock", "crafts", ["renaissance"], ["item:renaissance_food_clarifying_meat_jelly_bag"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "prepare bag of unstrained meat-jelly stock",
		Category = "Food and drink",
		Blurb = "prepare a bag of unstrained meat-jelly stock",
		Action = "preparing a food or drink item",
		ActiveCraftItemSdesc = "an in-progress bag of unstrained meat-jelly stock craft",
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
			new CatalogueCommodityRecipeInput(Grams: 525.0, Material: "meat", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("525 grams of meat", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hot Fire", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Hot Fire tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_food_clarifying_meat_jelly_bag", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a bag of unstrained meat-jelly stock (@renaissance_food_clarifying_meat_jelly_bag)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodAndDrinkPrepareSmokedFishMilletBalls() => new()
	{
		Metadata = new("Food and drink/prepare smoked-fish millet balls", "crafts", ["renaissance"], ["item:renaissance_food_coastal_smoked_fish_millet_balls"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "prepare smoked-fish millet balls",
		Category = "Food and drink",
		Blurb = "prepare smoked-fish millet balls",
		Action = "preparing a food or drink item",
		ActiveCraftItemSdesc = "an in-progress smoked-fish millet balls craft",
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
			new CatalogueCommodityRecipeInput(Grams: 263.0, Material: "fish", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("263 grams of fish", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hot Fire", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Hot Fire tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_food_coastal_smoked_fish_millet_balls", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x smoked-fish millet balls (@renaissance_food_coastal_smoked_fish_millet_balls)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodAndDrinkPrepareTurkeyJointInContactEraBrine() => new()
	{
		Metadata = new("Food and drink/prepare turkey joint in contact-era brine", "crafts", ["renaissance"], ["item:renaissance_food_contact_turkey_brining_joint"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "prepare turkey joint in contact-era brine",
		Category = "Food and drink",
		Blurb = "prepare a turkey joint in contact-era brine",
		Action = "preparing a food or drink item",
		ActiveCraftItemSdesc = "an in-progress turkey joint in contact-era brine craft",
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
			new CatalogueCommodityRecipeInput(Grams: 675.0, Material: "meat", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("675 grams of meat", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hot Fire", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Hot Fire tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_food_contact_turkey_brining_joint", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a turkey joint in contact-era brine (@renaissance_food_contact_turkey_brining_joint)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodAndDrinkPrepareFilledUnbakedEmpanadaRound() => new()
	{
		Metadata = new("Food and drink/prepare filled unbaked empanada round", "crafts", ["renaissance"], ["item:renaissance_food_empanada_filled_raw_round"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "prepare filled unbaked empanada round",
		Category = "Food and drink",
		Blurb = "prepare a filled unbaked empanada round",
		Action = "preparing a food or drink item",
		ActiveCraftItemSdesc = "an in-progress filled unbaked empanada round craft",
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
			new CatalogueCommodityRecipeInput(Grams: 675.0, Material: "fish", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("675 grams of fish", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hot Fire", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Hot Fire tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_food_empanada_filled_raw_round", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a filled unbaked empanada round (@renaissance_food_empanada_filled_raw_round)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodAndDrinkPrepareKnottedBanquetBiscuits() => new()
	{
		Metadata = new("Food and drink/prepare knotted banquet biscuits", "crafts", ["renaissance"], ["item:renaissance_food_english_banquet_knot_biscuit"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "prepare knotted banquet biscuits",
		Category = "Food and drink",
		Blurb = "prepare knotted banquet biscuits",
		Action = "preparing a food or drink item",
		ActiveCraftItemSdesc = "an in-progress knotted banquet biscuits craft",
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
			new CatalogueCommodityRecipeInput(Grams: 90.0, Material: "wheat", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("90 grams of wheat", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hot Fire", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Hot Fire tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_food_english_banquet_knot_biscuit", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x knotted banquet biscuits (@renaissance_food_english_banquet_knot_biscuit)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodAndDrinkPrepareEnglishBanquetMarchpane() => new()
	{
		Metadata = new("Food and drink/prepare English banquet marchpane", "crafts", ["renaissance"], ["item:renaissance_food_english_banquet_marchpane"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "prepare English banquet marchpane",
		Category = "Food and drink",
		Blurb = "prepare an English banquet marchpane",
		Action = "preparing a food or drink item",
		ActiveCraftItemSdesc = "an in-progress English banquet marchpane craft",
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
			new CatalogueCommodityRecipeInput(Grams: 450.0, Material: "food", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("450 grams of food", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hot Fire", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Hot Fire tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_food_english_banquet_marchpane", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x an English banquet marchpane (@renaissance_food_english_banquet_marchpane)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodAndDrinkPrepareEarlyEnglishCreamTrifle() => new()
	{
		Metadata = new("Food and drink/prepare early English cream trifle", "crafts", ["renaissance"], ["item:renaissance_food_english_cream_trifle"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "prepare early English cream trifle",
		Category = "Food and drink",
		Blurb = "prepare an early English cream trifle",
		Action = "preparing a food or drink item",
		ActiveCraftItemSdesc = "an in-progress early English cream trifle craft",
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
			new CatalogueCommodityRecipeInput(Grams: 225.0, Material: "cheese", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("225 grams of cheese", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hot Fire", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Hot Fire tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_food_english_cream_trifle", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x an early English cream trifle (@renaissance_food_english_cream_trifle)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodAndDrinkPrepareEnglishSpicedMincePie() => new()
	{
		Metadata = new("Food and drink/prepare English spiced mince pie", "crafts", ["renaissance"], ["item:renaissance_food_english_spiced_mince_pie"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "prepare English spiced mince pie",
		Category = "Food and drink",
		Blurb = "prepare an English spiced mince pie",
		Action = "preparing a food or drink item",
		ActiveCraftItemSdesc = "an in-progress English spiced mince pie craft",
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
			new CatalogueCommodityRecipeInput(Grams: 488.0, Material: "meat", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("488 grams of meat", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hot Fire", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Hot Fire tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_food_english_spiced_mince_pie", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x an English spiced mince pie (@renaissance_food_english_spiced_mince_pie)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodAndDrinkPrepareBoxOfWetAndDrySuckets() => new()
	{
		Metadata = new("Food and drink/prepare box of wet and dry suckets", "crafts", ["renaissance"], ["item:renaissance_food_english_sucket_citrus_box"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "prepare box of wet and dry suckets",
		Category = "Food and drink",
		Blurb = "prepare a box of wet and dry suckets",
		Action = "preparing a food or drink item",
		ActiveCraftItemSdesc = "an in-progress box of wet and dry suckets craft",
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
			new CatalogueCommodityRecipeInput(Grams: 263.0, Material: "food", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("263 grams of food", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hot Fire", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Hot Fire tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_food_english_sucket_citrus_box", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a box of wet and dry suckets (@renaissance_food_english_sucket_citrus_box)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodAndDrinkPrepareRawFishInEscabecheMarinade() => new()
	{
		Metadata = new("Food and drink/prepare raw fish in escabeche marinade", "crafts", ["renaissance"], ["item:renaissance_food_escabeche_uncooked_marinade_fish"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "prepare raw fish in escabeche marinade",
		Category = "Food and drink",
		Blurb = "prepare raw fish in escabeche marinade",
		Action = "preparing a food or drink item",
		ActiveCraftItemSdesc = "an in-progress raw fish in escabeche marinade craft",
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
			new CatalogueCommodityRecipeInput(Grams: 488.0, Material: "fish", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("488 grams of fish", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hot Fire", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Hot Fire tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_food_escabeche_uncooked_marinade_fish", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x raw fish in escabeche marinade (@renaissance_food_escabeche_uncooked_marinade_fish)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodAndDrinkPrepareCandiedFlowerComfits() => new()
	{
		Metadata = new("Food and drink/prepare candied flower comfits", "crafts", ["renaissance"], ["item:renaissance_food_french_candied_flower_comfits"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "prepare candied flower comfits",
		Category = "Food and drink",
		Blurb = "prepare candied flower comfits",
		Action = "preparing a food or drink item",
		ActiveCraftItemSdesc = "an in-progress candied flower comfits craft",
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
			new CatalogueCommodityRecipeInput(Grams: 60.0, Material: "honey", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("60 grams of honey", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hot Fire", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Hot Fire tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_food_french_candied_flower_comfits", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x candied flower comfits (@renaissance_food_french_candied_flower_comfits)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodAndDrinkPrepareBoxOfCotignacQuincePaste() => new()
	{
		Metadata = new("Food and drink/prepare box of cotignac quince paste", "crafts", ["renaissance"], ["item:renaissance_food_french_cotignac_quince_box"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "prepare box of cotignac quince paste",
		Category = "Food and drink",
		Blurb = "prepare a box of cotignac quince paste",
		Action = "preparing a food or drink item",
		ActiveCraftItemSdesc = "an in-progress box of cotignac quince paste craft",
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
			new CatalogueCommodityRecipeInput(Grams: 225.0, Material: "food", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("225 grams of food", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hot Fire", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Hot Fire tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_food_french_cotignac_quince_box", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a box of cotignac quince paste (@renaissance_food_french_cotignac_quince_box)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodAndDrinkPrepareRenaissanceCreamTart() => new()
	{
		Metadata = new("Food and drink/prepare Renaissance cream tart", "crafts", ["renaissance"], ["item:renaissance_food_french_cream_tart"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "prepare Renaissance cream tart",
		Category = "Food and drink",
		Blurb = "prepare a Renaissance cream tart",
		Action = "preparing a food or drink item",
		ActiveCraftItemSdesc = "an in-progress Renaissance cream tart craft",
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
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_food_french_cream_tart", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a Renaissance cream tart (@renaissance_food_french_cream_tart)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodAndDrinkPrepareAssortedDryFruitPastes() => new()
	{
		Metadata = new("Food and drink/prepare assorted dry fruit pastes", "crafts", ["renaissance"], ["item:renaissance_food_french_dry_fruit_pastes"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "prepare assorted dry fruit pastes",
		Category = "Food and drink",
		Blurb = "prepare assorted dry fruit pastes",
		Action = "preparing a food or drink item",
		ActiveCraftItemSdesc = "an in-progress assorted dry fruit pastes craft",
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
			new CatalogueCommodityRecipeInput(Grams: 225.0, Material: "food", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("225 grams of food", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hot Fire", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Hot Fire tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_food_french_dry_fruit_pastes", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x assorted dry fruit pastes (@renaissance_food_french_dry_fruit_pastes)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodAndDrinkPrepareFastDayFishPT() => new()
	{
		Metadata = new("Food and drink/prepare fast-day fish p\u00e2t\u00e9", "crafts", ["renaissance"], ["item:renaissance_food_french_fish_pate_fast_day"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "prepare fast-day fish p\u00e2t\u00e9",
		Category = "Food and drink",
		Blurb = "prepare a fast-day fish p\u00e2t\u00e9",
		Action = "preparing a food or drink item",
		ActiveCraftItemSdesc = "an in-progress fast-day fish p\u00e2t\u00e9 craft",
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
			new CatalogueCommodityRecipeInput(Grams: 450.0, Material: "fish", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("450 grams of fish", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hot Fire", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Hot Fire tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_food_french_fish_pate_fast_day", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a fast-day fish p\u00e2t\u00e9 (@renaissance_food_french_fish_pate_fast_day)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodAndDrinkPrepareJewelLikeHerbSalad() => new()
	{
		Metadata = new("Food and drink/prepare jewel-like herb salad", "crafts", ["renaissance"], ["item:renaissance_food_french_herb_salad_illusion"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "prepare jewel-like herb salad",
		Category = "Food and drink",
		Blurb = "prepare a jewel-like herb salad",
		Action = "preparing a food or drink item",
		ActiveCraftItemSdesc = "an in-progress jewel-like herb salad craft",
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
			new CatalogueCommodityRecipeInput(Grams: 263.0, Material: "vegetable", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("263 grams of vegetable", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hot Fire", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Hot Fire tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_food_french_herb_salad_illusion", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a jewel-like herb salad (@renaissance_food_french_herb_salad_illusion)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodAndDrinkPrepareRaisedPTEnCroTe() => new()
	{
		Metadata = new("Food and drink/prepare raised p\u00e2t\u00e9 en cro\u00fbte", "crafts", ["renaissance"], ["item:renaissance_food_french_raised_pate_en_croute"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "prepare raised p\u00e2t\u00e9 en cro\u00fbte",
		Category = "Food and drink",
		Blurb = "prepare a raised p\u00e2t\u00e9 en cro\u00fbte",
		Action = "preparing a food or drink item",
		ActiveCraftItemSdesc = "an in-progress raised p\u00e2t\u00e9 en cro\u00fbte craft",
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
			new CatalogueCommodityRecipeInput(Grams: 675.0, Material: "meat", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("675 grams of meat", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hot Fire", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Hot Fire tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_food_french_raised_pate_en_croute", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a raised p\u00e2t\u00e9 en cro\u00fbte (@renaissance_food_french_raised_pate_en_croute)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodAndDrinkPrepareSugaredAniseComfits() => new()
	{
		Metadata = new("Food and drink/prepare sugared anise comfits", "crafts", ["renaissance"], ["item:renaissance_food_french_sugared_anise_comfits"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "prepare sugared anise comfits",
		Category = "Food and drink",
		Blurb = "prepare sugared anise comfits",
		Action = "preparing a food or drink item",
		ActiveCraftItemSdesc = "an in-progress sugared anise comfits craft",
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
			new CatalogueCommodityRecipeInput(Grams: 75.0, Material: "honey", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("75 grams of honey", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hot Fire", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Hot Fire tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_food_french_sugared_anise_comfits", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x sugared anise comfits (@renaissance_food_french_sugared_anise_comfits)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodAndDrinkPrepareFragrantGamePT() => new()
	{
		Metadata = new("Food and drink/prepare fragrant game p\u00e2t\u00e9", "crafts", ["renaissance"], ["item:renaissance_food_french_truffled_like_game_pate"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "prepare fragrant game p\u00e2t\u00e9",
		Category = "Food and drink",
		Blurb = "prepare a fragrant game p\u00e2t\u00e9",
		Action = "preparing a food or drink item",
		ActiveCraftItemSdesc = "an in-progress fragrant game p\u00e2t\u00e9 craft",
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
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_food_french_truffled_like_game_pate", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a fragrant game p\u00e2t\u00e9 (@renaissance_food_french_truffled_like_game_pate)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodAndDrinkPrepareEarlyTurkeyGalantine() => new()
	{
		Metadata = new("Food and drink/prepare early turkey galantine", "crafts", ["renaissance"], ["item:renaissance_food_french_turkey_galantine"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "prepare early turkey galantine",
		Category = "Food and drink",
		Blurb = "prepare an early turkey galantine",
		Action = "preparing a food or drink item",
		ActiveCraftItemSdesc = "an in-progress early turkey galantine craft",
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
			new CatalogueCommodityRecipeInput(Grams: 675.0, Material: "meat", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("675 grams of meat", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hot Fire", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Hot Fire tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_food_french_turkey_galantine", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x an early turkey galantine (@renaissance_food_french_turkey_galantine)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodAndDrinkPrepareButteredEggNoodleTangle() => new()
	{
		Metadata = new("Food and drink/prepare buttered egg-noodle tangle", "crafts", ["renaissance"], ["item:renaissance_food_german_egg_noodle_tangle"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "prepare buttered egg-noodle tangle",
		Category = "Food and drink",
		Blurb = "prepare a buttered egg-noodle tangle",
		Action = "preparing a food or drink item",
		ActiveCraftItemSdesc = "an in-progress buttered egg-noodle tangle craft",
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
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_food_german_egg_noodle_tangle", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a buttered egg-noodle tangle (@renaissance_food_german_egg_noodle_tangle)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodAndDrinkPrepareRenaissanceFruitStollen() => new()
	{
		Metadata = new("Food and drink/prepare Renaissance fruit Stollen", "crafts", ["renaissance"], ["item:renaissance_food_german_fruit_stollen_loaf"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "prepare Renaissance fruit Stollen",
		Category = "Food and drink",
		Blurb = "prepare a Renaissance fruit Stollen",
		Action = "preparing a food or drink item",
		ActiveCraftItemSdesc = "an in-progress Renaissance fruit Stollen craft",
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
			new CatalogueCommodityRecipeInput(Grams: 600.0, Material: "bread", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("600 grams of bread", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hot Fire", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Hot Fire tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_food_german_fruit_stollen_loaf", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a Renaissance fruit Stollen (@renaissance_food_german_fruit_stollen_loaf)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodAndDrinkPrepareGermanMaizeGriddleExperiment() => new()
	{
		Metadata = new("Food and drink/prepare German maize griddle experiment", "crafts", ["renaissance"], ["item:renaissance_food_german_maize_griddle_experiment"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "prepare German maize griddle experiment",
		Category = "Food and drink",
		Blurb = "prepare a German maize griddle experiment",
		Action = "preparing a food or drink item",
		ActiveCraftItemSdesc = "an in-progress German maize griddle experiment craft",
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
			new CatalogueCommodityRecipeInput(Grams: 263.0, Material: "corn", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("263 grams of corn", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hot Fire", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Hot Fire tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_food_german_maize_griddle_experiment", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a German maize griddle experiment (@renaissance_food_german_maize_griddle_experiment)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodAndDrinkPrepareGuildMarzipanFigure() => new()
	{
		Metadata = new("Food and drink/prepare guild marzipan figure", "crafts", ["renaissance"], ["item:renaissance_food_german_marzipan_guild_figure"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "prepare guild marzipan figure",
		Category = "Food and drink",
		Blurb = "prepare a guild marzipan figure",
		Action = "preparing a food or drink item",
		ActiveCraftItemSdesc = "an in-progress guild marzipan figure craft",
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
			new CatalogueCommodityRecipeInput(Grams: 263.0, Material: "food", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("263 grams of food", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hot Fire", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Hot Fire tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_food_german_marzipan_guild_figure", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a guild marzipan figure (@renaissance_food_german_marzipan_guild_figure)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodAndDrinkPrepareMouldedGermanLebkuchen() => new()
	{
		Metadata = new("Food and drink/prepare moulded German Lebkuchen", "crafts", ["renaissance"], ["item:renaissance_food_german_moulded_lebkuchen"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "prepare moulded German Lebkuchen",
		Category = "Food and drink",
		Blurb = "prepare moulded German Lebkuchen",
		Action = "preparing a food or drink item",
		ActiveCraftItemSdesc = "an in-progress moulded German Lebkuchen craft",
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
			new CatalogueCommodityRecipeInput(Grams: 165.0, Material: "bread", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("165 grams of bread", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hot Fire", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Hot Fire tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_food_german_moulded_lebkuchen", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x moulded German Lebkuchen (@renaissance_food_german_moulded_lebkuchen)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodAndDrinkPrepareCakeOfUnfinishedCacaoSpicePaste() => new()
	{
		Metadata = new("Food and drink/prepare cake of unfinished cacao spice paste", "crafts", ["renaissance"], ["item:renaissance_food_ground_cacao_spice_paste"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "prepare cake of unfinished cacao spice paste",
		Category = "Food and drink",
		Blurb = "prepare a cake of unfinished cacao spice paste",
		Action = "preparing a food or drink item",
		ActiveCraftItemSdesc = "an in-progress cake of unfinished cacao spice paste craft",
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
			new CatalogueCommodityRecipeInput(Grams: 225.0, Material: "food", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("225 grams of food", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hot Fire", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Hot Fire tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_food_ground_cacao_spice_paste", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a cake of unfinished cacao spice paste (@renaissance_food_ground_cacao_spice_paste)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodAndDrinkPrepareIberianChickenManjarBlanco() => new()
	{
		Metadata = new("Food and drink/prepare Iberian chicken manjar blanco", "crafts", ["renaissance"], ["item:renaissance_food_iberian_chicken_manjar_blanco"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "prepare Iberian chicken manjar blanco",
		Category = "Food and drink",
		Blurb = "prepare Iberian chicken manjar blanco",
		Action = "preparing a food or drink item",
		ActiveCraftItemSdesc = "an in-progress Iberian chicken manjar blanco craft",
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
			new CatalogueCommodityRecipeInput(Grams: 338.0, Material: "meat", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("338 grams of meat", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hot Fire", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Hot Fire tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_food_iberian_chicken_manjar_blanco", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x Iberian chicken manjar blanco (@renaissance_food_iberian_chicken_manjar_blanco)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodAndDrinkPrepareConventAlmondCakes() => new()
	{
		Metadata = new("Food and drink/prepare convent almond cakes", "crafts", ["renaissance"], ["item:renaissance_food_iberian_convent_almond_cakes"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "prepare convent almond cakes",
		Category = "Food and drink",
		Blurb = "prepare convent almond cakes",
		Action = "preparing a food or drink item",
		ActiveCraftItemSdesc = "an in-progress convent almond cakes craft",
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
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_food_iberian_convent_almond_cakes", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x convent almond cakes (@renaissance_food_iberian_convent_almond_cakes)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodAndDrinkPrepareCreamFilledIberianFritters() => new()
	{
		Metadata = new("Food and drink/prepare cream-filled Iberian fritters", "crafts", ["renaissance"], ["item:renaissance_food_iberian_cream_filled_fritters"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "prepare cream-filled Iberian fritters",
		Category = "Food and drink",
		Blurb = "prepare cream-filled Iberian fritters",
		Action = "preparing a food or drink item",
		ActiveCraftItemSdesc = "an in-progress cream-filled Iberian fritters craft",
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
			new CatalogueCommodityRecipeInput(Grams: 188.0, Material: "cheese", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("188 grams of cheese", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hot Fire", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Hot Fire tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_food_iberian_cream_filled_fritters", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x cream-filled Iberian fritters (@renaissance_food_iberian_cream_filled_fritters)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodAndDrinkPrepareEggYolkConventSweet() => new()
	{
		Metadata = new("Food and drink/prepare egg-yolk convent sweet", "crafts", ["renaissance"], ["item:renaissance_food_iberian_egg_yolk_sweet"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "prepare egg-yolk convent sweet",
		Category = "Food and drink",
		Blurb = "prepare an egg-yolk convent sweet",
		Action = "preparing a food or drink item",
		ActiveCraftItemSdesc = "an in-progress egg-yolk convent sweet craft",
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
			new CatalogueCommodityRecipeInput(Grams: 120.0, Material: "egg", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("120 grams of egg", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hot Fire", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Hot Fire tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_food_iberian_egg_yolk_sweet", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x an egg-yolk convent sweet (@renaissance_food_iberian_egg_yolk_sweet)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodAndDrinkPrepareEscabecheGameBird() => new()
	{
		Metadata = new("Food and drink/prepare escabeche game bird", "crafts", ["renaissance"], ["item:renaissance_food_iberian_escabeche_game_bird"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "prepare escabeche game bird",
		Category = "Food and drink",
		Blurb = "prepare an escabeche game bird",
		Action = "preparing a food or drink item",
		ActiveCraftItemSdesc = "an in-progress escabeche game bird craft",
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
			new CatalogueCommodityRecipeInput(Grams: 413.0, Material: "meat", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("413 grams of meat", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hot Fire", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Hot Fire tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_food_iberian_escabeche_game_bird", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x an escabeche game bird (@renaissance_food_iberian_escabeche_game_bird)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodAndDrinkPrepareEscabechePreservedShellfish() => new()
	{
		Metadata = new("Food and drink/prepare escabeche-preserved shellfish", "crafts", ["renaissance"], ["item:renaissance_food_iberian_escabeche_shellfish"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "prepare escabeche-preserved shellfish",
		Category = "Food and drink",
		Blurb = "prepare escabeche-preserved shellfish",
		Action = "preparing a food or drink item",
		ActiveCraftItemSdesc = "an in-progress escabeche-preserved shellfish craft",
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
			new CatalogueCommodityRecipeInput(Grams: 300.0, Material: "fish", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("300 grams of fish", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hot Fire", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Hot Fire tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_food_iberian_escabeche_shellfish", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x escabeche-preserved shellfish (@renaissance_food_iberian_escabeche_shellfish)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodAndDrinkPrepareEarlyIberianTurkeyPie() => new()
	{
		Metadata = new("Food and drink/prepare early Iberian turkey pie", "crafts", ["renaissance"], ["item:renaissance_food_iberian_first_turkey_pie"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "prepare early Iberian turkey pie",
		Category = "Food and drink",
		Blurb = "prepare an early Iberian turkey pie",
		Action = "preparing a food or drink item",
		ActiveCraftItemSdesc = "an in-progress early Iberian turkey pie craft",
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
			new CatalogueCommodityRecipeInput(Grams: 675.0, Material: "meat", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("675 grams of meat", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hot Fire", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Hot Fire tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_food_iberian_first_turkey_pie", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x an early Iberian turkey pie (@renaissance_food_iberian_first_turkey_pie)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodAndDrinkPrepareHardHoneyAlmondTurron() => new()
	{
		Metadata = new("Food and drink/prepare hard honey-almond turron", "crafts", ["renaissance"], ["item:renaissance_food_iberian_honey_almond_turron"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "prepare hard honey-almond turron",
		Category = "Food and drink",
		Blurb = "prepare hard honey-almond turron",
		Action = "preparing a food or drink item",
		ActiveCraftItemSdesc = "an in-progress hard honey-almond turron craft",
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
			new CatalogueCommodityRecipeInput(Grams: 188.0, Material: "honey", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("188 grams of honey", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hot Fire", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Hot Fire tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_food_iberian_honey_almond_turron", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x hard honey-almond turron (@renaissance_food_iberian_honey_almond_turron)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodAndDrinkPrepareLentenManjarBlanco() => new()
	{
		Metadata = new("Food and drink/prepare Lenten manjar blanco", "crafts", ["renaissance"], ["item:renaissance_food_iberian_lenten_manjar_blanco"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "prepare Lenten manjar blanco",
		Category = "Food and drink",
		Blurb = "prepare a Lenten manjar blanco",
		Action = "preparing a food or drink item",
		ActiveCraftItemSdesc = "an in-progress Lenten manjar blanco craft",
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
			new CatalogueCommodityRecipeInput(Grams: 300.0, Material: "rice", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("300 grams of rice", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hot Fire", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Hot Fire tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_food_iberian_lenten_manjar_blanco", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a Lenten manjar blanco (@renaissance_food_iberian_lenten_manjar_blanco)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodAndDrinkPrepareEarlyIberianMaizeShipCake() => new()
	{
		Metadata = new("Food and drink/prepare early Iberian maize ship cake", "crafts", ["renaissance"], ["item:renaissance_food_iberian_maize_ship_cake"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "prepare early Iberian maize ship cake",
		Category = "Food and drink",
		Blurb = "prepare an early Iberian maize ship cake",
		Action = "preparing a food or drink item",
		ActiveCraftItemSdesc = "an in-progress early Iberian maize ship cake craft",
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
			new CatalogueCommodityRecipeInput(Grams: 263.0, Material: "corn", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("263 grams of corn", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hot Fire", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Hot Fire tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_food_iberian_maize_ship_cake", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x an early Iberian maize ship cake (@renaissance_food_iberian_maize_ship_cake)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodAndDrinkPrepareMeatFilledEmpanadillas() => new()
	{
		Metadata = new("Food and drink/prepare meat-filled empanadillas", "crafts", ["renaissance"], ["item:renaissance_food_iberian_meat_empanadillas"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "prepare meat-filled empanadillas",
		Category = "Food and drink",
		Blurb = "prepare meat-filled empanadillas",
		Action = "preparing a food or drink item",
		ActiveCraftItemSdesc = "an in-progress meat-filled empanadillas craft",
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
			new CatalogueCommodityRecipeInput(Grams: 210.0, Material: "meat", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("210 grams of meat", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hot Fire", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Hot Fire tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_food_iberian_meat_empanadillas", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x meat-filled empanadillas (@renaissance_food_iberian_meat_empanadillas)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodAndDrinkPrepareIberianOllaPodridaFeast() => new()
	{
		Metadata = new("Food and drink/prepare Iberian olla podrida feast", "crafts", ["renaissance"], ["item:renaissance_food_iberian_olla_podrida_feast"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "prepare Iberian olla podrida feast",
		Category = "Food and drink",
		Blurb = "prepare an Iberian olla podrida feast",
		Action = "preparing a food or drink item",
		ActiveCraftItemSdesc = "an in-progress Iberian olla podrida feast craft",
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
			new CatalogueCommodityRecipeInput(Grams: 900.0, Material: "meat", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("900 grams of meat", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hot Fire", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Hot Fire tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_food_iberian_olla_podrida_feast", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x an Iberian olla podrida feast (@renaissance_food_iberian_olla_podrida_feast)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodAndDrinkPrepareFirmQuinceMembrillo() => new()
	{
		Metadata = new("Food and drink/prepare firm quince membrillo", "crafts", ["renaissance"], ["item:renaissance_food_iberian_quince_membrillo"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "prepare firm quince membrillo",
		Category = "Food and drink",
		Blurb = "prepare a firm quince membrillo",
		Action = "preparing a food or drink item",
		ActiveCraftItemSdesc = "an in-progress firm quince membrillo craft",
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
			new CatalogueCommodityRecipeInput(Grams: 188.0, Material: "food", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("188 grams of food", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hot Fire", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Hot Fire tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_food_iberian_quince_membrillo", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a firm quince membrillo (@renaissance_food_iberian_quince_membrillo)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodAndDrinkPrepareEscabecheShipPie() => new()
	{
		Metadata = new("Food and drink/prepare escabeche ship pie", "crafts", ["renaissance"], ["item:renaissance_food_iberian_shipboard_escabeche_pie"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "prepare escabeche ship pie",
		Category = "Food and drink",
		Blurb = "prepare an escabeche ship pie",
		Action = "preparing a food or drink item",
		ActiveCraftItemSdesc = "an in-progress escabeche ship pie craft",
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
			new CatalogueCommodityRecipeInput(Grams: 375.0, Material: "fish", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("375 grams of fish", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hot Fire", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Hot Fire tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_food_iberian_shipboard_escabeche_pie", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x an escabeche ship pie (@renaissance_food_iberian_shipboard_escabeche_pie)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodAndDrinkPrepareSoftGroundAlmondTurron() => new()
	{
		Metadata = new("Food and drink/prepare soft ground-almond turron", "crafts", ["renaissance"], ["item:renaissance_food_iberian_soft_almond_turron"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "prepare soft ground-almond turron",
		Category = "Food and drink",
		Blurb = "prepare soft ground-almond turron",
		Action = "preparing a food or drink item",
		ActiveCraftItemSdesc = "an in-progress soft ground-almond turron craft",
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
			new CatalogueCommodityRecipeInput(Grams: 188.0, Material: "food", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("188 grams of food", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hot Fire", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Hot Fire tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_food_iberian_soft_almond_turron", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x soft ground-almond turron (@renaissance_food_iberian_soft_almond_turron)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodAndDrinkPrepareLargeTunaEmpanada() => new()
	{
		Metadata = new("Food and drink/prepare large tuna empanada", "crafts", ["renaissance"], ["item:renaissance_food_iberian_tuna_empanada"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "prepare large tuna empanada",
		Category = "Food and drink",
		Blurb = "prepare a large tuna empanada",
		Action = "preparing a food or drink item",
		ActiveCraftItemSdesc = "an in-progress large tuna empanada craft",
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
			new CatalogueCommodityRecipeInput(Grams: 600.0, Material: "fish", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("600 grams of fish", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hot Fire", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Hot Fire tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_food_iberian_tuna_empanada", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a large tuna empanada (@renaissance_food_iberian_tuna_empanada)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodAndDrinkPreparePlateOfWindFritters() => new()
	{
		Metadata = new("Food and drink/prepare plate of wind fritters", "crafts", ["renaissance"], ["item:renaissance_food_iberian_wind_fritters"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "prepare plate of wind fritters",
		Category = "Food and drink",
		Blurb = "prepare a plate of wind fritters",
		Action = "preparing a food or drink item",
		ActiveCraftItemSdesc = "an in-progress plate of wind fritters craft",
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
			new CatalogueCommodityRecipeInput(Grams: 150.0, Material: "wheat", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("150 grams of wheat", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hot Fire", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Hot Fire tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_food_iberian_wind_fritters", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a plate of wind fritters (@renaissance_food_iberian_wind_fritters)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodAndDrinkPrepareAlmondStuddedBiscotti() => new()
	{
		Metadata = new("Food and drink/prepare almond-studded biscotti", "crafts", ["renaissance"], ["item:renaissance_food_italian_almond_biscotti"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "prepare almond-studded biscotti",
		Category = "Food and drink",
		Blurb = "prepare almond-studded biscotti",
		Action = "preparing a food or drink item",
		ActiveCraftItemSdesc = "an in-progress almond-studded biscotti craft",
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
			new CatalogueCommodityRecipeInput(Grams: 135.0, Material: "wheat", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("135 grams of wheat", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hot Fire", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Hot Fire tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_food_italian_almond_biscotti", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x almond-studded biscotti (@renaissance_food_italian_almond_biscotti)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodAndDrinkPrepareAlmondFishPolpette() => new()
	{
		Metadata = new("Food and drink/prepare almond fish polpette", "crafts", ["renaissance"], ["item:renaissance_food_italian_almond_fish_polpette"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "prepare almond fish polpette",
		Category = "Food and drink",
		Blurb = "prepare almond fish polpette",
		Action = "preparing a food or drink item",
		ActiveCraftItemSdesc = "an in-progress almond fish polpette craft",
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
			new CatalogueCommodityRecipeInput(Grams: 338.0, Material: "fish", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("338 grams of fish", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hot Fire", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Hot Fire tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_food_italian_almond_fish_polpette", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x almond fish polpette (@renaissance_food_italian_almond_fish_polpette)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodAndDrinkPrepareAniseScentedBiscotti() => new()
	{
		Metadata = new("Food and drink/prepare anise-scented biscotti", "crafts", ["renaissance"], ["item:renaissance_food_italian_anise_biscotti"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "prepare anise-scented biscotti",
		Category = "Food and drink",
		Blurb = "prepare anise-scented biscotti",
		Action = "preparing a food or drink item",
		ActiveCraftItemSdesc = "an in-progress anise-scented biscotti craft",
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
			new CatalogueCommodityRecipeInput(Grams: 113.0, Material: "wheat", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("113 grams of wheat", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hot Fire", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Hot Fire tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_food_italian_anise_biscotti", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x anise-scented biscotti (@renaissance_food_italian_anise_biscotti)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodAndDrinkPrepareArtichokeCourtTorta() => new()
	{
		Metadata = new("Food and drink/prepare artichoke court torta", "crafts", ["renaissance"], ["item:renaissance_food_italian_artichoke_torta"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "prepare artichoke court torta",
		Category = "Food and drink",
		Blurb = "prepare an artichoke court torta",
		Action = "preparing a food or drink item",
		ActiveCraftItemSdesc = "an in-progress artichoke court torta craft",
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
			new CatalogueCommodityRecipeInput(Grams: 450.0, Material: "vegetable", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("450 grams of vegetable", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hot Fire", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Hot Fire tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_food_italian_artichoke_torta", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x an artichoke court torta (@renaissance_food_italian_artichoke_torta)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodAndDrinkPrepareBakedVermicelliTorte() => new()
	{
		Metadata = new("Food and drink/prepare baked vermicelli torte", "crafts", ["renaissance"], ["item:renaissance_food_italian_baked_vermicelli_torte"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "prepare baked vermicelli torte",
		Category = "Food and drink",
		Blurb = "prepare a baked vermicelli torte",
		Action = "preparing a food or drink item",
		ActiveCraftItemSdesc = "an in-progress baked vermicelli torte craft",
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
			new CatalogueCommodityRecipeInput(Grams: 488.0, Material: "wheat", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("488 grams of wheat", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hot Fire", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Hot Fire tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_food_italian_baked_vermicelli_torte", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a baked vermicelli torte (@renaissance_food_italian_baked_vermicelli_torte)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodAndDrinkPrepareBreadCrumbCourtGnocchi() => new()
	{
		Metadata = new("Food and drink/prepare bread-crumb court gnocchi", "crafts", ["renaissance"], ["item:renaissance_food_italian_bread_crumb_gnocchi"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "prepare bread-crumb court gnocchi",
		Category = "Food and drink",
		Blurb = "prepare bread-crumb court gnocchi",
		Action = "preparing a food or drink item",
		ActiveCraftItemSdesc = "an in-progress bread-crumb court gnocchi craft",
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
			new CatalogueCommodityRecipeInput(Grams: 323.0, Material: "bread", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("323 grams of bread", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hot Fire", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Hot Fire tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_food_italian_bread_crumb_gnocchi", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x bread-crumb court gnocchi (@renaissance_food_italian_bread_crumb_gnocchi)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodAndDrinkPrepareItalianButteredMacaroni() => new()
	{
		Metadata = new("Food and drink/prepare Italian buttered macaroni", "crafts", ["renaissance"], ["item:renaissance_food_italian_buttered_macaroni"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "prepare Italian buttered macaroni",
		Category = "Food and drink",
		Blurb = "prepare Italian buttered macaroni",
		Action = "preparing a food or drink item",
		ActiveCraftItemSdesc = "an in-progress Italian buttered macaroni craft",
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
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_food_italian_buttered_macaroni", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x Italian buttered macaroni (@renaissance_food_italian_buttered_macaroni)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodAndDrinkPrepareCandiedCitrusPeel() => new()
	{
		Metadata = new("Food and drink/prepare candied citrus peel", "crafts", ["renaissance"], ["item:renaissance_food_italian_candied_citrus_peel"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "prepare candied citrus peel",
		Category = "Food and drink",
		Blurb = "prepare candied citrus peel",
		Action = "preparing a food or drink item",
		ActiveCraftItemSdesc = "an in-progress candied citrus peel craft",
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
			new CatalogueCommodityRecipeInput(Grams: 135.0, Material: "food", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("135 grams of food", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hot Fire", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Hot Fire tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_food_italian_candied_citrus_peel", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x candied citrus peel (@renaissance_food_italian_candied_citrus_peel)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodAndDrinkPrepareCandiedGreenWalnuts() => new()
	{
		Metadata = new("Food and drink/prepare candied green walnuts", "crafts", ["renaissance"], ["item:renaissance_food_italian_candied_green_walnuts"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "prepare candied green walnuts",
		Category = "Food and drink",
		Blurb = "prepare candied green walnuts",
		Action = "preparing a food or drink item",
		ActiveCraftItemSdesc = "an in-progress candied green walnuts craft",
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
			new CatalogueCommodityRecipeInput(Grams: 188.0, Material: "food", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("188 grams of food", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hot Fire", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Hot Fire tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_food_italian_candied_green_walnuts", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x candied green walnuts (@renaissance_food_italian_candied_green_walnuts)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodAndDrinkPrepareCaponPolpetteWithFruit() => new()
	{
		Metadata = new("Food and drink/prepare capon polpette with fruit", "crafts", ["renaissance"], ["item:renaissance_food_italian_capon_polpette"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "prepare capon polpette with fruit",
		Category = "Food and drink",
		Blurb = "prepare capon polpette with fruit",
		Action = "preparing a food or drink item",
		ActiveCraftItemSdesc = "an in-progress capon polpette with fruit craft",
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
			new CatalogueCommodityRecipeInput(Grams: 375.0, Material: "meat", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("375 grams of meat", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hot Fire", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Hot Fire tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_food_italian_capon_polpette", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x capon polpette with fruit (@renaissance_food_italian_capon_polpette)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodAndDrinkPrepareCaponAndFruitTorta() => new()
	{
		Metadata = new("Food and drink/prepare capon-and-fruit torta", "crafts", ["renaissance"], ["item:renaissance_food_italian_capon_pomegranate_torta"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "prepare capon-and-fruit torta",
		Category = "Food and drink",
		Blurb = "prepare a capon-and-fruit torta",
		Action = "preparing a food or drink item",
		ActiveCraftItemSdesc = "an in-progress capon-and-fruit torta craft",
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
			new CatalogueCommodityRecipeInput(Grams: 675.0, Material: "meat", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("675 grams of meat", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hot Fire", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Hot Fire tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_food_italian_capon_pomegranate_torta", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a capon-and-fruit torta (@renaissance_food_italian_capon_pomegranate_torta)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodAndDrinkPrepareCaponFilledTortellini() => new()
	{
		Metadata = new("Food and drink/prepare capon-filled tortellini", "crafts", ["renaissance"], ["item:renaissance_food_italian_capon_tortellini"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "prepare capon-filled tortellini",
		Category = "Food and drink",
		Blurb = "prepare capon-filled tortellini",
		Action = "preparing a food or drink item",
		ActiveCraftItemSdesc = "an in-progress capon-filled tortellini craft",
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
			new CatalogueCommodityRecipeInput(Grams: 338.0, Material: "meat", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("338 grams of meat", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hot Fire", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Hot Fire tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_food_italian_capon_tortellini", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x capon-filled tortellini (@renaissance_food_italian_capon_tortellini)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodAndDrinkPrepareCaponJellyMosaic() => new()
	{
		Metadata = new("Food and drink/prepare capon jelly mosaic", "crafts", ["renaissance"], ["item:renaissance_food_italian_cold_capon_jelly_mosaic"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "prepare capon jelly mosaic",
		Category = "Food and drink",
		Blurb = "prepare a capon jelly mosaic",
		Action = "preparing a food or drink item",
		ActiveCraftItemSdesc = "an in-progress capon jelly mosaic craft",
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
			new CatalogueCommodityRecipeInput(Grams: 563.0, Material: "meat", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("563 grams of meat", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hot Fire", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Hot Fire tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_food_italian_cold_capon_jelly_mosaic", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a capon jelly mosaic (@renaissance_food_italian_cold_capon_jelly_mosaic)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodAndDrinkPrepareWhippedCreamSnow() => new()
	{
		Metadata = new("Food and drink/prepare whipped cream snow", "crafts", ["renaissance"], ["item:renaissance_food_italian_cream_snow"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "prepare whipped cream snow",
		Category = "Food and drink",
		Blurb = "prepare a whipped cream snow",
		Action = "preparing a food or drink item",
		ActiveCraftItemSdesc = "an in-progress whipped cream snow craft",
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
			new CatalogueCommodityRecipeInput(Grams: 225.0, Material: "cheese", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("225 grams of cheese", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hot Fire", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Hot Fire tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_food_italian_cream_snow", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a whipped cream snow (@renaissance_food_italian_cream_snow)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodAndDrinkPrepareEarlyItalianMaizePolenta() => new()
	{
		Metadata = new("Food and drink/prepare early Italian maize polenta", "crafts", ["renaissance"], ["item:renaissance_food_italian_early_maize_polenta"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "prepare early Italian maize polenta",
		Category = "Food and drink",
		Blurb = "prepare early Italian maize polenta",
		Action = "preparing a food or drink item",
		ActiveCraftItemSdesc = "an in-progress early Italian maize polenta craft",
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
			new CatalogueCommodityRecipeInput(Grams: 413.0, Material: "corn", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("413 grams of corn", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hot Fire", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Hot Fire tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_food_italian_early_maize_polenta", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x early Italian maize polenta (@renaissance_food_italian_early_maize_polenta)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodAndDrinkPrepareFishJellyMedallions() => new()
	{
		Metadata = new("Food and drink/prepare fish jelly medallions", "crafts", ["renaissance"], ["item:renaissance_food_italian_fish_jelly_medallions"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "prepare fish jelly medallions",
		Category = "Food and drink",
		Blurb = "prepare fish jelly medallions",
		Action = "preparing a food or drink item",
		ActiveCraftItemSdesc = "an in-progress fish jelly medallions craft",
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
			new CatalogueCommodityRecipeInput(Grams: 375.0, Material: "fish", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("375 grams of fish", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hot Fire", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Hot Fire tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_food_italian_fish_jelly_medallions", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x fish jelly medallions (@renaissance_food_italian_fish_jelly_medallions)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodAndDrinkPrepareFriedLiverCroquettes() => new()
	{
		Metadata = new("Food and drink/prepare fried liver croquettes", "crafts", ["renaissance"], ["item:renaissance_food_italian_fried_liver_croquettes"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "prepare fried liver croquettes",
		Category = "Food and drink",
		Blurb = "prepare fried liver croquettes",
		Action = "preparing a food or drink item",
		ActiveCraftItemSdesc = "an in-progress fried liver croquettes craft",
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
			new CatalogueCommodityRecipeInput(Grams: 225.0, Material: "meat", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("225 grams of meat", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hot Fire", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Hot Fire tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_food_italian_fried_liver_croquettes", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x fried liver croquettes (@renaissance_food_italian_fried_liver_croquettes)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};
}
