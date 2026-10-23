#nullable enable

using MudSharp.Body;
using MudSharp.GameItems.Inventory.Plans;
using MudSharp.RPG.Checks;

namespace DatabaseSeeder.Seeders;

public static partial class Recipes02Stock
{
	public static CatalogueRecipeDefinition CarpentryCarveBirchHouseholdVesselStock() => new()
	{
		Metadata = new("Carpentry/carve birch household vessel stock", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.AntiquityHousehold.cs:969", []),
		RequiredAnyEra = ["antiquity"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "carve birch household vessel stock",
		Category = "Carpentry",
		Blurb = "carve birch into household vessel blanks",
		Action = "carve birch household vessel stock",
		ActiveCraftItemSdesc = "an in-progress carve birch household vessel blanks craft",
		Gate = CatalogueRecipeGates.AncientWoodworkingAndJoineryCarpentry15,
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
			new(25, "$0 inspect|inspects $i1 and sort|sorts out the usable material.", "$0 inspect|inspects $i1 and sort|sorts out the usable material.", ExertionLevel.Stasis, 0),
			new(35, "$0 work|works the material into a consistent prepared stock, using $t1 and $t2.", "$0 work|works the material into a consistent prepared stock, using $t1 and $t2.", ExertionLevel.Stasis, 0),
			new(25, "$0 finish|finishes the preparation and set|sets aside $p1.", "$0 finish|finishes the preparation, but the stock is not worth keeping.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1000.0, Material: "birch", PileTag: "Furniture Timber Stock", CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1 kilogram of birch", ["piletag Furniture Timber Stock"]), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Wood Chisel", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Wood Chisel tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Rasp", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Rasp tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueCommodityRecipeProduct(Grams: 800.0, Material: "birch", Tag: "Carved Wood Stock", Characteristics: []) { Compatibility = new("800 grams of birch commodity", ["tag Carved Wood Stock"]), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition CarpentryPrepareBoxwoodFurnitureTimberStock() => new()
	{
		Metadata = new("Carpentry/prepare boxwood furniture timber stock", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.AntiquityHousehold.cs:969", []),
		RequiredAnyEra = ["antiquity"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "prepare boxwood furniture timber stock",
		Category = "Carpentry",
		Blurb = "dress boxwood into furniture timber stock",
		Action = "prepare boxwood furniture timber stock",
		ActiveCraftItemSdesc = "an in-progress dress boxwood furniture timber stock craft",
		Gate = CatalogueRecipeGates.AncientWoodworkingAndJoineryCarpentry15,
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
			new(25, "$0 inspect|inspects $i1 and sort|sorts out the usable material.", "$0 inspect|inspects $i1 and sort|sorts out the usable material.", ExertionLevel.Stasis, 0),
			new(35, "$0 work|works the material into a consistent prepared stock, using $t1 and $t2.", "$0 work|works the material into a consistent prepared stock, using $t1 and $t2.", ExertionLevel.Stasis, 0),
			new(25, "$0 finish|finishes the preparation and set|sets aside $p1.", "$0 finish|finishes the preparation, but the stock is not worth keeping.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1200.0, Material: "boxwood", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1 kilogram 200 grams of boxwood", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hand Saw", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hand Saw tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Adze", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Adze tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueCommodityRecipeProduct(Grams: 1000.0, Material: "boxwood", Tag: "Furniture Timber Stock", Characteristics: []) { Compatibility = new("1 kilogram of boxwood commodity", ["tag Furniture Timber Stock"]), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition CarpentrySawBoxwoodFurniturePanelStock() => new()
	{
		Metadata = new("Carpentry/saw boxwood furniture panel stock", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.AntiquityHousehold.cs:969", []),
		RequiredAnyEra = ["antiquity"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "saw boxwood furniture panel stock",
		Category = "Carpentry",
		Blurb = "saw boxwood into furniture panel stock",
		Action = "saw boxwood furniture panel stock",
		ActiveCraftItemSdesc = "an in-progress saw boxwood furniture panel stock craft",
		Gate = CatalogueRecipeGates.AncientWoodworkingAndJoineryCarpentry15,
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
			new(25, "$0 inspect|inspects $i1 and sort|sorts out the usable material.", "$0 inspect|inspects $i1 and sort|sorts out the usable material.", ExertionLevel.Stasis, 0),
			new(35, "$0 work|works the material into a consistent prepared stock, using $t1 and $t2.", "$0 work|works the material into a consistent prepared stock, using $t1 and $t2.", ExertionLevel.Stasis, 0),
			new(25, "$0 finish|finishes the preparation and set|sets aside $p1.", "$0 finish|finishes the preparation, but the stock is not worth keeping.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1200.0, Material: "boxwood", PileTag: "Furniture Timber Stock", CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1 kilogram 200 grams of boxwood", ["piletag Furniture Timber Stock"]), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hand Saw", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hand Saw tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Wood Clamp", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Wood Clamp tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueCommodityRecipeProduct(Grams: 900.0, Material: "boxwood", Tag: "Furniture Panel Stock", Characteristics: []) { Compatibility = new("900 grams of boxwood commodity", ["tag Furniture Panel Stock"]), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition CarpentryCarveBoxwoodHouseholdVesselStock() => new()
	{
		Metadata = new("Carpentry/carve boxwood household vessel stock", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.AntiquityHousehold.cs:969", []),
		RequiredAnyEra = ["antiquity"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "carve boxwood household vessel stock",
		Category = "Carpentry",
		Blurb = "carve boxwood into household vessel blanks",
		Action = "carve boxwood household vessel stock",
		ActiveCraftItemSdesc = "an in-progress carve boxwood household vessel blanks craft",
		Gate = CatalogueRecipeGates.AncientWoodworkingAndJoineryCarpentry15,
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
			new(25, "$0 inspect|inspects $i1 and sort|sorts out the usable material.", "$0 inspect|inspects $i1 and sort|sorts out the usable material.", ExertionLevel.Stasis, 0),
			new(35, "$0 work|works the material into a consistent prepared stock, using $t1 and $t2.", "$0 work|works the material into a consistent prepared stock, using $t1 and $t2.", ExertionLevel.Stasis, 0),
			new(25, "$0 finish|finishes the preparation and set|sets aside $p1.", "$0 finish|finishes the preparation, but the stock is not worth keeping.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1000.0, Material: "boxwood", PileTag: "Furniture Timber Stock", CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1 kilogram of boxwood", ["piletag Furniture Timber Stock"]), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Wood Chisel", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Wood Chisel tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Rasp", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Rasp tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueCommodityRecipeProduct(Grams: 800.0, Material: "boxwood", Tag: "Carved Wood Stock", Characteristics: []) { Compatibility = new("800 grams of boxwood commodity", ["tag Carved Wood Stock"]), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition CarpentryPrepareCedarFurnitureTimberStock() => new()
	{
		Metadata = new("Carpentry/prepare cedar furniture timber stock", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.AntiquityHousehold.cs:969", []),
		RequiredAnyEra = ["antiquity"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "prepare cedar furniture timber stock",
		Category = "Carpentry",
		Blurb = "dress cedar into furniture timber stock",
		Action = "prepare cedar furniture timber stock",
		ActiveCraftItemSdesc = "an in-progress dress cedar furniture timber stock craft",
		Gate = CatalogueRecipeGates.AncientWoodworkingAndJoineryCarpentry15,
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
			new(25, "$0 inspect|inspects $i1 and sort|sorts out the usable material.", "$0 inspect|inspects $i1 and sort|sorts out the usable material.", ExertionLevel.Stasis, 0),
			new(35, "$0 work|works the material into a consistent prepared stock, using $t1 and $t2.", "$0 work|works the material into a consistent prepared stock, using $t1 and $t2.", ExertionLevel.Stasis, 0),
			new(25, "$0 finish|finishes the preparation and set|sets aside $p1.", "$0 finish|finishes the preparation, but the stock is not worth keeping.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1200.0, Material: "cedar", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1 kilogram 200 grams of cedar", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hand Saw", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hand Saw tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Adze", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Adze tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueCommodityRecipeProduct(Grams: 1000.0, Material: "cedar", Tag: "Furniture Timber Stock", Characteristics: []) { Compatibility = new("1 kilogram of cedar commodity", ["tag Furniture Timber Stock"]), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition CarpentrySawCedarFurniturePanelStock() => new()
	{
		Metadata = new("Carpentry/saw cedar furniture panel stock", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.AntiquityHousehold.cs:969", []),
		RequiredAnyEra = ["antiquity"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "saw cedar furniture panel stock",
		Category = "Carpentry",
		Blurb = "saw cedar into furniture panel stock",
		Action = "saw cedar furniture panel stock",
		ActiveCraftItemSdesc = "an in-progress saw cedar furniture panel stock craft",
		Gate = CatalogueRecipeGates.AncientWoodworkingAndJoineryCarpentry15,
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
			new(25, "$0 inspect|inspects $i1 and sort|sorts out the usable material.", "$0 inspect|inspects $i1 and sort|sorts out the usable material.", ExertionLevel.Stasis, 0),
			new(35, "$0 work|works the material into a consistent prepared stock, using $t1 and $t2.", "$0 work|works the material into a consistent prepared stock, using $t1 and $t2.", ExertionLevel.Stasis, 0),
			new(25, "$0 finish|finishes the preparation and set|sets aside $p1.", "$0 finish|finishes the preparation, but the stock is not worth keeping.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1200.0, Material: "cedar", PileTag: "Furniture Timber Stock", CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1 kilogram 200 grams of cedar", ["piletag Furniture Timber Stock"]), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hand Saw", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hand Saw tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Wood Clamp", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Wood Clamp tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueCommodityRecipeProduct(Grams: 900.0, Material: "cedar", Tag: "Furniture Panel Stock", Characteristics: []) { Compatibility = new("900 grams of cedar commodity", ["tag Furniture Panel Stock"]), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition CarpentryCarveCedarHouseholdVesselStock() => new()
	{
		Metadata = new("Carpentry/carve cedar household vessel stock", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.AntiquityHousehold.cs:969", []),
		RequiredAnyEra = ["antiquity"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "carve cedar household vessel stock",
		Category = "Carpentry",
		Blurb = "carve cedar into household vessel blanks",
		Action = "carve cedar household vessel stock",
		ActiveCraftItemSdesc = "an in-progress carve cedar household vessel blanks craft",
		Gate = CatalogueRecipeGates.AncientWoodworkingAndJoineryCarpentry15,
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
			new(25, "$0 inspect|inspects $i1 and sort|sorts out the usable material.", "$0 inspect|inspects $i1 and sort|sorts out the usable material.", ExertionLevel.Stasis, 0),
			new(35, "$0 work|works the material into a consistent prepared stock, using $t1 and $t2.", "$0 work|works the material into a consistent prepared stock, using $t1 and $t2.", ExertionLevel.Stasis, 0),
			new(25, "$0 finish|finishes the preparation and set|sets aside $p1.", "$0 finish|finishes the preparation, but the stock is not worth keeping.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1000.0, Material: "cedar", PileTag: "Furniture Timber Stock", CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1 kilogram of cedar", ["piletag Furniture Timber Stock"]), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Wood Chisel", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Wood Chisel tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Rasp", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Rasp tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueCommodityRecipeProduct(Grams: 800.0, Material: "cedar", Tag: "Carved Wood Stock", Characteristics: []) { Compatibility = new("800 grams of cedar commodity", ["tag Carved Wood Stock"]), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition CarpentryPrepareCypressFurnitureTimberStock() => new()
	{
		Metadata = new("Carpentry/prepare cypress furniture timber stock", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.AntiquityHousehold.cs:969", []),
		RequiredAnyEra = ["antiquity"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "prepare cypress furniture timber stock",
		Category = "Carpentry",
		Blurb = "dress cypress into furniture timber stock",
		Action = "prepare cypress furniture timber stock",
		ActiveCraftItemSdesc = "an in-progress dress cypress furniture timber stock craft",
		Gate = CatalogueRecipeGates.AncientWoodworkingAndJoineryCarpentry15,
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
			new(25, "$0 inspect|inspects $i1 and sort|sorts out the usable material.", "$0 inspect|inspects $i1 and sort|sorts out the usable material.", ExertionLevel.Stasis, 0),
			new(35, "$0 work|works the material into a consistent prepared stock, using $t1 and $t2.", "$0 work|works the material into a consistent prepared stock, using $t1 and $t2.", ExertionLevel.Stasis, 0),
			new(25, "$0 finish|finishes the preparation and set|sets aside $p1.", "$0 finish|finishes the preparation, but the stock is not worth keeping.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1200.0, Material: "cypress", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1 kilogram 200 grams of cypress", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hand Saw", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hand Saw tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Adze", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Adze tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueCommodityRecipeProduct(Grams: 1000.0, Material: "cypress", Tag: "Furniture Timber Stock", Characteristics: []) { Compatibility = new("1 kilogram of cypress commodity", ["tag Furniture Timber Stock"]), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition CarpentrySawCypressFurniturePanelStock() => new()
	{
		Metadata = new("Carpentry/saw cypress furniture panel stock", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.AntiquityHousehold.cs:969", []),
		RequiredAnyEra = ["antiquity"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "saw cypress furniture panel stock",
		Category = "Carpentry",
		Blurb = "saw cypress into furniture panel stock",
		Action = "saw cypress furniture panel stock",
		ActiveCraftItemSdesc = "an in-progress saw cypress furniture panel stock craft",
		Gate = CatalogueRecipeGates.AncientWoodworkingAndJoineryCarpentry15,
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
			new(25, "$0 inspect|inspects $i1 and sort|sorts out the usable material.", "$0 inspect|inspects $i1 and sort|sorts out the usable material.", ExertionLevel.Stasis, 0),
			new(35, "$0 work|works the material into a consistent prepared stock, using $t1 and $t2.", "$0 work|works the material into a consistent prepared stock, using $t1 and $t2.", ExertionLevel.Stasis, 0),
			new(25, "$0 finish|finishes the preparation and set|sets aside $p1.", "$0 finish|finishes the preparation, but the stock is not worth keeping.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1200.0, Material: "cypress", PileTag: "Furniture Timber Stock", CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1 kilogram 200 grams of cypress", ["piletag Furniture Timber Stock"]), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hand Saw", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hand Saw tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Wood Clamp", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Wood Clamp tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueCommodityRecipeProduct(Grams: 900.0, Material: "cypress", Tag: "Furniture Panel Stock", Characteristics: []) { Compatibility = new("900 grams of cypress commodity", ["tag Furniture Panel Stock"]), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition CarpentryCarveCypressHouseholdVesselStock() => new()
	{
		Metadata = new("Carpentry/carve cypress household vessel stock", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.AntiquityHousehold.cs:969", []),
		RequiredAnyEra = ["antiquity"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "carve cypress household vessel stock",
		Category = "Carpentry",
		Blurb = "carve cypress into household vessel blanks",
		Action = "carve cypress household vessel stock",
		ActiveCraftItemSdesc = "an in-progress carve cypress household vessel blanks craft",
		Gate = CatalogueRecipeGates.AncientWoodworkingAndJoineryCarpentry15,
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
			new(25, "$0 inspect|inspects $i1 and sort|sorts out the usable material.", "$0 inspect|inspects $i1 and sort|sorts out the usable material.", ExertionLevel.Stasis, 0),
			new(35, "$0 work|works the material into a consistent prepared stock, using $t1 and $t2.", "$0 work|works the material into a consistent prepared stock, using $t1 and $t2.", ExertionLevel.Stasis, 0),
			new(25, "$0 finish|finishes the preparation and set|sets aside $p1.", "$0 finish|finishes the preparation, but the stock is not worth keeping.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1000.0, Material: "cypress", PileTag: "Furniture Timber Stock", CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1 kilogram of cypress", ["piletag Furniture Timber Stock"]), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Wood Chisel", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Wood Chisel tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Rasp", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Rasp tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueCommodityRecipeProduct(Grams: 800.0, Material: "cypress", Tag: "Carved Wood Stock", Characteristics: []) { Compatibility = new("800 grams of cypress commodity", ["tag Carved Wood Stock"]), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition CarpentryPrepareEbonyFurnitureTimberStock() => new()
	{
		Metadata = new("Carpentry/prepare ebony furniture timber stock", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.AntiquityHousehold.cs:969", []),
		RequiredAnyEra = ["antiquity"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "prepare ebony furniture timber stock",
		Category = "Carpentry",
		Blurb = "dress ebony into furniture timber stock",
		Action = "prepare ebony furniture timber stock",
		ActiveCraftItemSdesc = "an in-progress dress ebony furniture timber stock craft",
		Gate = CatalogueRecipeGates.AncientWoodworkingAndJoineryCarpentry15,
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
			new(25, "$0 inspect|inspects $i1 and sort|sorts out the usable material.", "$0 inspect|inspects $i1 and sort|sorts out the usable material.", ExertionLevel.Stasis, 0),
			new(35, "$0 work|works the material into a consistent prepared stock, using $t1 and $t2.", "$0 work|works the material into a consistent prepared stock, using $t1 and $t2.", ExertionLevel.Stasis, 0),
			new(25, "$0 finish|finishes the preparation and set|sets aside $p1.", "$0 finish|finishes the preparation, but the stock is not worth keeping.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1200.0, Material: "ebony", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1 kilogram 200 grams of ebony", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hand Saw", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hand Saw tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Adze", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Adze tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueCommodityRecipeProduct(Grams: 1000.0, Material: "ebony", Tag: "Furniture Timber Stock", Characteristics: []) { Compatibility = new("1 kilogram of ebony commodity", ["tag Furniture Timber Stock"]), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition CarpentrySawEbonyFurniturePanelStock() => new()
	{
		Metadata = new("Carpentry/saw ebony furniture panel stock", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.AntiquityHousehold.cs:969", []),
		RequiredAnyEra = ["antiquity"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "saw ebony furniture panel stock",
		Category = "Carpentry",
		Blurb = "saw ebony into furniture panel stock",
		Action = "saw ebony furniture panel stock",
		ActiveCraftItemSdesc = "an in-progress saw ebony furniture panel stock craft",
		Gate = CatalogueRecipeGates.AncientWoodworkingAndJoineryCarpentry15,
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
			new(25, "$0 inspect|inspects $i1 and sort|sorts out the usable material.", "$0 inspect|inspects $i1 and sort|sorts out the usable material.", ExertionLevel.Stasis, 0),
			new(35, "$0 work|works the material into a consistent prepared stock, using $t1 and $t2.", "$0 work|works the material into a consistent prepared stock, using $t1 and $t2.", ExertionLevel.Stasis, 0),
			new(25, "$0 finish|finishes the preparation and set|sets aside $p1.", "$0 finish|finishes the preparation, but the stock is not worth keeping.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1200.0, Material: "ebony", PileTag: "Furniture Timber Stock", CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1 kilogram 200 grams of ebony", ["piletag Furniture Timber Stock"]), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hand Saw", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hand Saw tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Wood Clamp", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Wood Clamp tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueCommodityRecipeProduct(Grams: 900.0, Material: "ebony", Tag: "Furniture Panel Stock", Characteristics: []) { Compatibility = new("900 grams of ebony commodity", ["tag Furniture Panel Stock"]), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition CarpentryCarveEbonyHouseholdVesselStock() => new()
	{
		Metadata = new("Carpentry/carve ebony household vessel stock", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.AntiquityHousehold.cs:969", []),
		RequiredAnyEra = ["antiquity"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "carve ebony household vessel stock",
		Category = "Carpentry",
		Blurb = "carve ebony into household vessel blanks",
		Action = "carve ebony household vessel stock",
		ActiveCraftItemSdesc = "an in-progress carve ebony household vessel blanks craft",
		Gate = CatalogueRecipeGates.AncientWoodworkingAndJoineryCarpentry15,
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
			new(25, "$0 inspect|inspects $i1 and sort|sorts out the usable material.", "$0 inspect|inspects $i1 and sort|sorts out the usable material.", ExertionLevel.Stasis, 0),
			new(35, "$0 work|works the material into a consistent prepared stock, using $t1 and $t2.", "$0 work|works the material into a consistent prepared stock, using $t1 and $t2.", ExertionLevel.Stasis, 0),
			new(25, "$0 finish|finishes the preparation and set|sets aside $p1.", "$0 finish|finishes the preparation, but the stock is not worth keeping.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1000.0, Material: "ebony", PileTag: "Furniture Timber Stock", CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1 kilogram of ebony", ["piletag Furniture Timber Stock"]), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Wood Chisel", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Wood Chisel tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Rasp", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Rasp tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueCommodityRecipeProduct(Grams: 800.0, Material: "ebony", Tag: "Carved Wood Stock", Characteristics: []) { Compatibility = new("800 grams of ebony commodity", ["tag Carved Wood Stock"]), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition CarpentryPrepareOakFurnitureTimberStock() => new()
	{
		Metadata = new("Carpentry/prepare oak furniture timber stock", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.AntiquityHousehold.cs:969", []),
		RequiredAnyEra = ["antiquity"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "prepare oak furniture timber stock",
		Category = "Carpentry",
		Blurb = "dress oak into furniture timber stock",
		Action = "prepare oak furniture timber stock",
		ActiveCraftItemSdesc = "an in-progress dress oak furniture timber stock craft",
		Gate = CatalogueRecipeGates.AncientWoodworkingAndJoineryCarpentry15,
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
			new(25, "$0 inspect|inspects $i1 and sort|sorts out the usable material.", "$0 inspect|inspects $i1 and sort|sorts out the usable material.", ExertionLevel.Stasis, 0),
			new(35, "$0 work|works the material into a consistent prepared stock, using $t1 and $t2.", "$0 work|works the material into a consistent prepared stock, using $t1 and $t2.", ExertionLevel.Stasis, 0),
			new(25, "$0 finish|finishes the preparation and set|sets aside $p1.", "$0 finish|finishes the preparation, but the stock is not worth keeping.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1200.0, Material: "oak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1 kilogram 200 grams of oak", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hand Saw", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hand Saw tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Adze", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Adze tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueCommodityRecipeProduct(Grams: 1000.0, Material: "oak", Tag: "Furniture Timber Stock", Characteristics: []) { Compatibility = new("1 kilogram of oak commodity", ["tag Furniture Timber Stock"]), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition CarpentrySawOakFurniturePanelStock() => new()
	{
		Metadata = new("Carpentry/saw oak furniture panel stock", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.AntiquityHousehold.cs:969", []),
		RequiredAnyEra = ["antiquity"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "saw oak furniture panel stock",
		Category = "Carpentry",
		Blurb = "saw oak into furniture panel stock",
		Action = "saw oak furniture panel stock",
		ActiveCraftItemSdesc = "an in-progress saw oak furniture panel stock craft",
		Gate = CatalogueRecipeGates.AncientWoodworkingAndJoineryCarpentry15,
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
			new(25, "$0 inspect|inspects $i1 and sort|sorts out the usable material.", "$0 inspect|inspects $i1 and sort|sorts out the usable material.", ExertionLevel.Stasis, 0),
			new(35, "$0 work|works the material into a consistent prepared stock, using $t1 and $t2.", "$0 work|works the material into a consistent prepared stock, using $t1 and $t2.", ExertionLevel.Stasis, 0),
			new(25, "$0 finish|finishes the preparation and set|sets aside $p1.", "$0 finish|finishes the preparation, but the stock is not worth keeping.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1200.0, Material: "oak", PileTag: "Furniture Timber Stock", CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1 kilogram 200 grams of oak", ["piletag Furniture Timber Stock"]), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hand Saw", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hand Saw tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Wood Clamp", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Wood Clamp tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueCommodityRecipeProduct(Grams: 900.0, Material: "oak", Tag: "Furniture Panel Stock", Characteristics: []) { Compatibility = new("900 grams of oak commodity", ["tag Furniture Panel Stock"]), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition CarpentryCarveOakHouseholdVesselStock() => new()
	{
		Metadata = new("Carpentry/carve oak household vessel stock", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.AntiquityHousehold.cs:969", []),
		RequiredAnyEra = ["antiquity"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "carve oak household vessel stock",
		Category = "Carpentry",
		Blurb = "carve oak into household vessel blanks",
		Action = "carve oak household vessel stock",
		ActiveCraftItemSdesc = "an in-progress carve oak household vessel blanks craft",
		Gate = CatalogueRecipeGates.AncientWoodworkingAndJoineryCarpentry15,
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
			new(25, "$0 inspect|inspects $i1 and sort|sorts out the usable material.", "$0 inspect|inspects $i1 and sort|sorts out the usable material.", ExertionLevel.Stasis, 0),
			new(35, "$0 work|works the material into a consistent prepared stock, using $t1 and $t2.", "$0 work|works the material into a consistent prepared stock, using $t1 and $t2.", ExertionLevel.Stasis, 0),
			new(25, "$0 finish|finishes the preparation and set|sets aside $p1.", "$0 finish|finishes the preparation, but the stock is not worth keeping.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1000.0, Material: "oak", PileTag: "Furniture Timber Stock", CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1 kilogram of oak", ["piletag Furniture Timber Stock"]), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Wood Chisel", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Wood Chisel tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Rasp", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Rasp tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueCommodityRecipeProduct(Grams: 800.0, Material: "oak", Tag: "Carved Wood Stock", Characteristics: []) { Compatibility = new("800 grams of oak commodity", ["tag Carved Wood Stock"]), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition CarpentryPreparePineFurnitureTimberStock() => new()
	{
		Metadata = new("Carpentry/prepare pine furniture timber stock", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.AntiquityHousehold.cs:969", []),
		RequiredAnyEra = ["antiquity"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "prepare pine furniture timber stock",
		Category = "Carpentry",
		Blurb = "dress pine into furniture timber stock",
		Action = "prepare pine furniture timber stock",
		ActiveCraftItemSdesc = "an in-progress dress pine furniture timber stock craft",
		Gate = CatalogueRecipeGates.AncientWoodworkingAndJoineryCarpentry15,
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
			new(25, "$0 inspect|inspects $i1 and sort|sorts out the usable material.", "$0 inspect|inspects $i1 and sort|sorts out the usable material.", ExertionLevel.Stasis, 0),
			new(35, "$0 work|works the material into a consistent prepared stock, using $t1 and $t2.", "$0 work|works the material into a consistent prepared stock, using $t1 and $t2.", ExertionLevel.Stasis, 0),
			new(25, "$0 finish|finishes the preparation and set|sets aside $p1.", "$0 finish|finishes the preparation, but the stock is not worth keeping.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1200.0, Material: "pine", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1 kilogram 200 grams of pine", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hand Saw", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hand Saw tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Adze", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Adze tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueCommodityRecipeProduct(Grams: 1000.0, Material: "pine", Tag: "Furniture Timber Stock", Characteristics: []) { Compatibility = new("1 kilogram of pine commodity", ["tag Furniture Timber Stock"]), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition CarpentrySawPineFurniturePanelStock() => new()
	{
		Metadata = new("Carpentry/saw pine furniture panel stock", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.AntiquityHousehold.cs:969", []),
		RequiredAnyEra = ["antiquity"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "saw pine furniture panel stock",
		Category = "Carpentry",
		Blurb = "saw pine into furniture panel stock",
		Action = "saw pine furniture panel stock",
		ActiveCraftItemSdesc = "an in-progress saw pine furniture panel stock craft",
		Gate = CatalogueRecipeGates.AncientWoodworkingAndJoineryCarpentry15,
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
			new(25, "$0 inspect|inspects $i1 and sort|sorts out the usable material.", "$0 inspect|inspects $i1 and sort|sorts out the usable material.", ExertionLevel.Stasis, 0),
			new(35, "$0 work|works the material into a consistent prepared stock, using $t1 and $t2.", "$0 work|works the material into a consistent prepared stock, using $t1 and $t2.", ExertionLevel.Stasis, 0),
			new(25, "$0 finish|finishes the preparation and set|sets aside $p1.", "$0 finish|finishes the preparation, but the stock is not worth keeping.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1200.0, Material: "pine", PileTag: "Furniture Timber Stock", CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1 kilogram 200 grams of pine", ["piletag Furniture Timber Stock"]), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hand Saw", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hand Saw tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Wood Clamp", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Wood Clamp tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueCommodityRecipeProduct(Grams: 900.0, Material: "pine", Tag: "Furniture Panel Stock", Characteristics: []) { Compatibility = new("900 grams of pine commodity", ["tag Furniture Panel Stock"]), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition CarpentryCarvePineHouseholdVesselStock() => new()
	{
		Metadata = new("Carpentry/carve pine household vessel stock", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.AntiquityHousehold.cs:969", []),
		RequiredAnyEra = ["antiquity"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "carve pine household vessel stock",
		Category = "Carpentry",
		Blurb = "carve pine into household vessel blanks",
		Action = "carve pine household vessel stock",
		ActiveCraftItemSdesc = "an in-progress carve pine household vessel blanks craft",
		Gate = CatalogueRecipeGates.AncientWoodworkingAndJoineryCarpentry15,
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
			new(25, "$0 inspect|inspects $i1 and sort|sorts out the usable material.", "$0 inspect|inspects $i1 and sort|sorts out the usable material.", ExertionLevel.Stasis, 0),
			new(35, "$0 work|works the material into a consistent prepared stock, using $t1 and $t2.", "$0 work|works the material into a consistent prepared stock, using $t1 and $t2.", ExertionLevel.Stasis, 0),
			new(25, "$0 finish|finishes the preparation and set|sets aside $p1.", "$0 finish|finishes the preparation, but the stock is not worth keeping.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1000.0, Material: "pine", PileTag: "Furniture Timber Stock", CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1 kilogram of pine", ["piletag Furniture Timber Stock"]), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Wood Chisel", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Wood Chisel tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Rasp", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Rasp tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueCommodityRecipeProduct(Grams: 800.0, Material: "pine", Tag: "Carved Wood Stock", Characteristics: []) { Compatibility = new("800 grams of pine commodity", ["tag Carved Wood Stock"]), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition CarpentryPrepareSycamoreFurnitureTimberStock() => new()
	{
		Metadata = new("Carpentry/prepare sycamore furniture timber stock", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.AntiquityHousehold.cs:969", []),
		RequiredAnyEra = ["antiquity"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "prepare sycamore furniture timber stock",
		Category = "Carpentry",
		Blurb = "dress sycamore into furniture timber stock",
		Action = "prepare sycamore furniture timber stock",
		ActiveCraftItemSdesc = "an in-progress dress sycamore furniture timber stock craft",
		Gate = CatalogueRecipeGates.AncientWoodworkingAndJoineryCarpentry15,
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
			new(25, "$0 inspect|inspects $i1 and sort|sorts out the usable material.", "$0 inspect|inspects $i1 and sort|sorts out the usable material.", ExertionLevel.Stasis, 0),
			new(35, "$0 work|works the material into a consistent prepared stock, using $t1 and $t2.", "$0 work|works the material into a consistent prepared stock, using $t1 and $t2.", ExertionLevel.Stasis, 0),
			new(25, "$0 finish|finishes the preparation and set|sets aside $p1.", "$0 finish|finishes the preparation, but the stock is not worth keeping.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1200.0, Material: "sycamore", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1 kilogram 200 grams of sycamore", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hand Saw", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hand Saw tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Adze", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Adze tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueCommodityRecipeProduct(Grams: 1000.0, Material: "sycamore", Tag: "Furniture Timber Stock", Characteristics: []) { Compatibility = new("1 kilogram of sycamore commodity", ["tag Furniture Timber Stock"]), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition CarpentrySawSycamoreFurniturePanelStock() => new()
	{
		Metadata = new("Carpentry/saw sycamore furniture panel stock", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.AntiquityHousehold.cs:969", []),
		RequiredAnyEra = ["antiquity"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "saw sycamore furniture panel stock",
		Category = "Carpentry",
		Blurb = "saw sycamore into furniture panel stock",
		Action = "saw sycamore furniture panel stock",
		ActiveCraftItemSdesc = "an in-progress saw sycamore furniture panel stock craft",
		Gate = CatalogueRecipeGates.AncientWoodworkingAndJoineryCarpentry15,
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
			new(25, "$0 inspect|inspects $i1 and sort|sorts out the usable material.", "$0 inspect|inspects $i1 and sort|sorts out the usable material.", ExertionLevel.Stasis, 0),
			new(35, "$0 work|works the material into a consistent prepared stock, using $t1 and $t2.", "$0 work|works the material into a consistent prepared stock, using $t1 and $t2.", ExertionLevel.Stasis, 0),
			new(25, "$0 finish|finishes the preparation and set|sets aside $p1.", "$0 finish|finishes the preparation, but the stock is not worth keeping.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1200.0, Material: "sycamore", PileTag: "Furniture Timber Stock", CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1 kilogram 200 grams of sycamore", ["piletag Furniture Timber Stock"]), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hand Saw", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hand Saw tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Wood Clamp", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Wood Clamp tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueCommodityRecipeProduct(Grams: 900.0, Material: "sycamore", Tag: "Furniture Panel Stock", Characteristics: []) { Compatibility = new("900 grams of sycamore commodity", ["tag Furniture Panel Stock"]), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition CarpentryCarveSycamoreHouseholdVesselStock() => new()
	{
		Metadata = new("Carpentry/carve sycamore household vessel stock", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.AntiquityHousehold.cs:969", []),
		RequiredAnyEra = ["antiquity"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "carve sycamore household vessel stock",
		Category = "Carpentry",
		Blurb = "carve sycamore into household vessel blanks",
		Action = "carve sycamore household vessel stock",
		ActiveCraftItemSdesc = "an in-progress carve sycamore household vessel blanks craft",
		Gate = CatalogueRecipeGates.AncientWoodworkingAndJoineryCarpentry15,
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
			new(25, "$0 inspect|inspects $i1 and sort|sorts out the usable material.", "$0 inspect|inspects $i1 and sort|sorts out the usable material.", ExertionLevel.Stasis, 0),
			new(35, "$0 work|works the material into a consistent prepared stock, using $t1 and $t2.", "$0 work|works the material into a consistent prepared stock, using $t1 and $t2.", ExertionLevel.Stasis, 0),
			new(25, "$0 finish|finishes the preparation and set|sets aside $p1.", "$0 finish|finishes the preparation, but the stock is not worth keeping.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1000.0, Material: "sycamore", PileTag: "Furniture Timber Stock", CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1 kilogram of sycamore", ["piletag Furniture Timber Stock"]), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Wood Chisel", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Wood Chisel tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Rasp", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Rasp tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueCommodityRecipeProduct(Grams: 800.0, Material: "sycamore", Tag: "Carved Wood Stock", Characteristics: []) { Compatibility = new("800 grams of sycamore commodity", ["tag Carved Wood Stock"]), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition CarpentryPrepareWalnutFurnitureTimberStock() => new()
	{
		Metadata = new("Carpentry/prepare walnut furniture timber stock", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.AntiquityHousehold.cs:969", []),
		RequiredAnyEra = ["antiquity"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "prepare walnut furniture timber stock",
		Category = "Carpentry",
		Blurb = "dress walnut into furniture timber stock",
		Action = "prepare walnut furniture timber stock",
		ActiveCraftItemSdesc = "an in-progress dress walnut furniture timber stock craft",
		Gate = CatalogueRecipeGates.AncientWoodworkingAndJoineryCarpentry15,
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
			new(25, "$0 inspect|inspects $i1 and sort|sorts out the usable material.", "$0 inspect|inspects $i1 and sort|sorts out the usable material.", ExertionLevel.Stasis, 0),
			new(35, "$0 work|works the material into a consistent prepared stock, using $t1 and $t2.", "$0 work|works the material into a consistent prepared stock, using $t1 and $t2.", ExertionLevel.Stasis, 0),
			new(25, "$0 finish|finishes the preparation and set|sets aside $p1.", "$0 finish|finishes the preparation, but the stock is not worth keeping.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1200.0, Material: "walnut", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1 kilogram 200 grams of walnut", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hand Saw", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hand Saw tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Adze", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Adze tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueCommodityRecipeProduct(Grams: 1000.0, Material: "walnut", Tag: "Furniture Timber Stock", Characteristics: []) { Compatibility = new("1 kilogram of walnut commodity", ["tag Furniture Timber Stock"]), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition CarpentrySawWalnutFurniturePanelStock() => new()
	{
		Metadata = new("Carpentry/saw walnut furniture panel stock", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.AntiquityHousehold.cs:969", []),
		RequiredAnyEra = ["antiquity"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "saw walnut furniture panel stock",
		Category = "Carpentry",
		Blurb = "saw walnut into furniture panel stock",
		Action = "saw walnut furniture panel stock",
		ActiveCraftItemSdesc = "an in-progress saw walnut furniture panel stock craft",
		Gate = CatalogueRecipeGates.AncientWoodworkingAndJoineryCarpentry15,
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
			new(25, "$0 inspect|inspects $i1 and sort|sorts out the usable material.", "$0 inspect|inspects $i1 and sort|sorts out the usable material.", ExertionLevel.Stasis, 0),
			new(35, "$0 work|works the material into a consistent prepared stock, using $t1 and $t2.", "$0 work|works the material into a consistent prepared stock, using $t1 and $t2.", ExertionLevel.Stasis, 0),
			new(25, "$0 finish|finishes the preparation and set|sets aside $p1.", "$0 finish|finishes the preparation, but the stock is not worth keeping.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1200.0, Material: "walnut", PileTag: "Furniture Timber Stock", CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1 kilogram 200 grams of walnut", ["piletag Furniture Timber Stock"]), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hand Saw", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hand Saw tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Wood Clamp", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Wood Clamp tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueCommodityRecipeProduct(Grams: 900.0, Material: "walnut", Tag: "Furniture Panel Stock", Characteristics: []) { Compatibility = new("900 grams of walnut commodity", ["tag Furniture Panel Stock"]), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition CarpentryCarveWalnutHouseholdVesselStock() => new()
	{
		Metadata = new("Carpentry/carve walnut household vessel stock", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.AntiquityHousehold.cs:969", []),
		RequiredAnyEra = ["antiquity"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "carve walnut household vessel stock",
		Category = "Carpentry",
		Blurb = "carve walnut into household vessel blanks",
		Action = "carve walnut household vessel stock",
		ActiveCraftItemSdesc = "an in-progress carve walnut household vessel blanks craft",
		Gate = CatalogueRecipeGates.AncientWoodworkingAndJoineryCarpentry15,
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
			new(25, "$0 inspect|inspects $i1 and sort|sorts out the usable material.", "$0 inspect|inspects $i1 and sort|sorts out the usable material.", ExertionLevel.Stasis, 0),
			new(35, "$0 work|works the material into a consistent prepared stock, using $t1 and $t2.", "$0 work|works the material into a consistent prepared stock, using $t1 and $t2.", ExertionLevel.Stasis, 0),
			new(25, "$0 finish|finishes the preparation and set|sets aside $p1.", "$0 finish|finishes the preparation, but the stock is not worth keeping.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1000.0, Material: "walnut", PileTag: "Furniture Timber Stock", CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1 kilogram of walnut", ["piletag Furniture Timber Stock"]), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Wood Chisel", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Wood Chisel tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Rasp", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Rasp tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueCommodityRecipeProduct(Grams: 800.0, Material: "walnut", Tag: "Carved Wood Stock", Characteristics: []) { Compatibility = new("800 grams of walnut commodity", ["tag Carved Wood Stock"]), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition CarpentryPrepareWillowFurnitureTimberStock() => new()
	{
		Metadata = new("Carpentry/prepare willow furniture timber stock", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.AntiquityHousehold.cs:969", []),
		RequiredAnyEra = ["antiquity"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "prepare willow furniture timber stock",
		Category = "Carpentry",
		Blurb = "dress willow into furniture timber stock",
		Action = "prepare willow furniture timber stock",
		ActiveCraftItemSdesc = "an in-progress dress willow furniture timber stock craft",
		Gate = CatalogueRecipeGates.AncientWoodworkingAndJoineryCarpentry15,
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
			new(25, "$0 inspect|inspects $i1 and sort|sorts out the usable material.", "$0 inspect|inspects $i1 and sort|sorts out the usable material.", ExertionLevel.Stasis, 0),
			new(35, "$0 work|works the material into a consistent prepared stock, using $t1 and $t2.", "$0 work|works the material into a consistent prepared stock, using $t1 and $t2.", ExertionLevel.Stasis, 0),
			new(25, "$0 finish|finishes the preparation and set|sets aside $p1.", "$0 finish|finishes the preparation, but the stock is not worth keeping.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1200.0, Material: "willow", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1 kilogram 200 grams of willow", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hand Saw", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hand Saw tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Adze", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Adze tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueCommodityRecipeProduct(Grams: 1000.0, Material: "willow", Tag: "Furniture Timber Stock", Characteristics: []) { Compatibility = new("1 kilogram of willow commodity", ["tag Furniture Timber Stock"]), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition CarpentrySawWillowFurniturePanelStock() => new()
	{
		Metadata = new("Carpentry/saw willow furniture panel stock", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.AntiquityHousehold.cs:969", []),
		RequiredAnyEra = ["antiquity"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "saw willow furniture panel stock",
		Category = "Carpentry",
		Blurb = "saw willow into furniture panel stock",
		Action = "saw willow furniture panel stock",
		ActiveCraftItemSdesc = "an in-progress saw willow furniture panel stock craft",
		Gate = CatalogueRecipeGates.AncientWoodworkingAndJoineryCarpentry15,
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
			new(25, "$0 inspect|inspects $i1 and sort|sorts out the usable material.", "$0 inspect|inspects $i1 and sort|sorts out the usable material.", ExertionLevel.Stasis, 0),
			new(35, "$0 work|works the material into a consistent prepared stock, using $t1 and $t2.", "$0 work|works the material into a consistent prepared stock, using $t1 and $t2.", ExertionLevel.Stasis, 0),
			new(25, "$0 finish|finishes the preparation and set|sets aside $p1.", "$0 finish|finishes the preparation, but the stock is not worth keeping.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1200.0, Material: "willow", PileTag: "Furniture Timber Stock", CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1 kilogram 200 grams of willow", ["piletag Furniture Timber Stock"]), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hand Saw", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hand Saw tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Wood Clamp", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Wood Clamp tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueCommodityRecipeProduct(Grams: 900.0, Material: "willow", Tag: "Furniture Panel Stock", Characteristics: []) { Compatibility = new("900 grams of willow commodity", ["tag Furniture Panel Stock"]), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition CarpentryCarveWillowHouseholdVesselStock() => new()
	{
		Metadata = new("Carpentry/carve willow household vessel stock", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.AntiquityHousehold.cs:969", []),
		RequiredAnyEra = ["antiquity"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "carve willow household vessel stock",
		Category = "Carpentry",
		Blurb = "carve willow into household vessel blanks",
		Action = "carve willow household vessel stock",
		ActiveCraftItemSdesc = "an in-progress carve willow household vessel blanks craft",
		Gate = CatalogueRecipeGates.AncientWoodworkingAndJoineryCarpentry15,
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
			new(25, "$0 inspect|inspects $i1 and sort|sorts out the usable material.", "$0 inspect|inspects $i1 and sort|sorts out the usable material.", ExertionLevel.Stasis, 0),
			new(35, "$0 work|works the material into a consistent prepared stock, using $t1 and $t2.", "$0 work|works the material into a consistent prepared stock, using $t1 and $t2.", ExertionLevel.Stasis, 0),
			new(25, "$0 finish|finishes the preparation and set|sets aside $p1.", "$0 finish|finishes the preparation, but the stock is not worth keeping.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1000.0, Material: "willow", PileTag: "Furniture Timber Stock", CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1 kilogram of willow", ["piletag Furniture Timber Stock"]), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Wood Chisel", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Wood Chisel tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Rasp", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Rasp tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueCommodityRecipeProduct(Grams: 800.0, Material: "willow", Tag: "Carved Wood Stock", Characteristics: []) { Compatibility = new("800 grams of willow commodity", ["tag Carved Wood Stock"]), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition CarpentryPrepareWoodFurnitureTimberStock() => new()
	{
		Metadata = new("Carpentry/prepare wood furniture timber stock", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.AntiquityHousehold.cs:969", []),
		RequiredAnyEra = ["antiquity"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "prepare wood furniture timber stock",
		Category = "Carpentry",
		Blurb = "dress wood into furniture timber stock",
		Action = "prepare wood furniture timber stock",
		ActiveCraftItemSdesc = "an in-progress dress wood furniture timber stock craft",
		Gate = CatalogueRecipeGates.AncientWoodworkingAndJoineryCarpentry15,
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
			new(25, "$0 inspect|inspects $i1 and sort|sorts out the usable material.", "$0 inspect|inspects $i1 and sort|sorts out the usable material.", ExertionLevel.Stasis, 0),
			new(35, "$0 work|works the material into a consistent prepared stock, using $t1 and $t2.", "$0 work|works the material into a consistent prepared stock, using $t1 and $t2.", ExertionLevel.Stasis, 0),
			new(25, "$0 finish|finishes the preparation and set|sets aside $p1.", "$0 finish|finishes the preparation, but the stock is not worth keeping.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1200.0, Material: "wood", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1 kilogram 200 grams of wood", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hand Saw", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hand Saw tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Adze", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Adze tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueCommodityRecipeProduct(Grams: 1000.0, Material: "wood", Tag: "Furniture Timber Stock", Characteristics: []) { Compatibility = new("1 kilogram of wood commodity", ["tag Furniture Timber Stock"]), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition CarpentrySawWoodFurniturePanelStock() => new()
	{
		Metadata = new("Carpentry/saw wood furniture panel stock", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.AntiquityHousehold.cs:969", []),
		RequiredAnyEra = ["antiquity"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "saw wood furniture panel stock",
		Category = "Carpentry",
		Blurb = "saw wood into furniture panel stock",
		Action = "saw wood furniture panel stock",
		ActiveCraftItemSdesc = "an in-progress saw wood furniture panel stock craft",
		Gate = CatalogueRecipeGates.AncientWoodworkingAndJoineryCarpentry15,
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
			new(25, "$0 inspect|inspects $i1 and sort|sorts out the usable material.", "$0 inspect|inspects $i1 and sort|sorts out the usable material.", ExertionLevel.Stasis, 0),
			new(35, "$0 work|works the material into a consistent prepared stock, using $t1 and $t2.", "$0 work|works the material into a consistent prepared stock, using $t1 and $t2.", ExertionLevel.Stasis, 0),
			new(25, "$0 finish|finishes the preparation and set|sets aside $p1.", "$0 finish|finishes the preparation, but the stock is not worth keeping.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1200.0, Material: "wood", PileTag: "Furniture Timber Stock", CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1 kilogram 200 grams of wood", ["piletag Furniture Timber Stock"]), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hand Saw", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hand Saw tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Wood Clamp", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Wood Clamp tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueCommodityRecipeProduct(Grams: 900.0, Material: "wood", Tag: "Furniture Panel Stock", Characteristics: []) { Compatibility = new("900 grams of wood commodity", ["tag Furniture Panel Stock"]), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition CarpentryCarveWoodHouseholdVesselStock() => new()
	{
		Metadata = new("Carpentry/carve wood household vessel stock", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.AntiquityHousehold.cs:969", []),
		RequiredAnyEra = ["antiquity"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "carve wood household vessel stock",
		Category = "Carpentry",
		Blurb = "carve wood into household vessel blanks",
		Action = "carve wood household vessel stock",
		ActiveCraftItemSdesc = "an in-progress carve wood household vessel blanks craft",
		Gate = CatalogueRecipeGates.AncientWoodworkingAndJoineryCarpentry15,
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
			new(25, "$0 inspect|inspects $i1 and sort|sorts out the usable material.", "$0 inspect|inspects $i1 and sort|sorts out the usable material.", ExertionLevel.Stasis, 0),
			new(35, "$0 work|works the material into a consistent prepared stock, using $t1 and $t2.", "$0 work|works the material into a consistent prepared stock, using $t1 and $t2.", ExertionLevel.Stasis, 0),
			new(25, "$0 finish|finishes the preparation and set|sets aside $p1.", "$0 finish|finishes the preparation, but the stock is not worth keeping.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1000.0, Material: "wood", PileTag: "Furniture Timber Stock", CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1 kilogram of wood", ["piletag Furniture Timber Stock"]), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Wood Chisel", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Wood Chisel tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Rasp", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Rasp tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueCommodityRecipeProduct(Grams: 800.0, Material: "wood", Tag: "Carved Wood Stock", Characteristics: []) { Compatibility = new("800 grams of wood commodity", ["tag Carved Wood Stock"]), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition CooperingShapeOakCooperedStaves() => new()
	{
		Metadata = new("Coopering/shape oak coopered staves", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.AntiquityHousehold.cs:969", []),
		RequiredAnyEra = ["antiquity"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "shape oak coopered staves",
		Category = "Coopering",
		Blurb = "shape oak into coopered staves",
		Action = "shape oak coopered staves",
		ActiveCraftItemSdesc = "an in-progress oak coopered staves craft",
		Gate = CatalogueRecipeGates.AncientCooperingCoopering15,
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
			new(25, "$0 inspect|inspects $i1 and sort|sorts out the usable material.", "$0 inspect|inspects $i1 and sort|sorts out the usable material.", ExertionLevel.Stasis, 0),
			new(35, "$0 work|works the material into a consistent prepared stock, using $t1 and $t2.", "$0 work|works the material into a consistent prepared stock, using $t1 and $t2.", ExertionLevel.Stasis, 0),
			new(25, "$0 finish|finishes the preparation and set|sets aside $p1.", "$0 finish|finishes the preparation, but the stock is not worth keeping.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1500.0, Material: "oak", PileTag: "Furniture Timber Stock", CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1 kilogram 500 grams of oak", ["piletag Furniture Timber Stock"]), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Cooper's Adze", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Cooper's Adze tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Croze", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Croze tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueCommodityRecipeProduct(Grams: 1150.0, Material: "oak", Tag: "Coopered Staves", Characteristics: []) { Compatibility = new("1 kilogram 150 grams of oak commodity", ["tag Coopered Staves"]), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition CooperingShapeCedarCooperedStaves() => new()
	{
		Metadata = new("Coopering/shape cedar coopered staves", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.AntiquityHousehold.cs:969", []),
		RequiredAnyEra = ["antiquity"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "shape cedar coopered staves",
		Category = "Coopering",
		Blurb = "shape cedar into coopered staves",
		Action = "shape cedar coopered staves",
		ActiveCraftItemSdesc = "an in-progress cedar coopered staves craft",
		Gate = CatalogueRecipeGates.AncientCooperingCoopering15,
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
			new(25, "$0 inspect|inspects $i1 and sort|sorts out the usable material.", "$0 inspect|inspects $i1 and sort|sorts out the usable material.", ExertionLevel.Stasis, 0),
			new(35, "$0 work|works the material into a consistent prepared stock, using $t1 and $t2.", "$0 work|works the material into a consistent prepared stock, using $t1 and $t2.", ExertionLevel.Stasis, 0),
			new(25, "$0 finish|finishes the preparation and set|sets aside $p1.", "$0 finish|finishes the preparation, but the stock is not worth keeping.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1500.0, Material: "cedar", PileTag: "Furniture Timber Stock", CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1 kilogram 500 grams of cedar", ["piletag Furniture Timber Stock"]), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Cooper's Adze", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Cooper's Adze tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Croze", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Croze tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueCommodityRecipeProduct(Grams: 1150.0, Material: "cedar", Tag: "Coopered Staves", Characteristics: []) { Compatibility = new("1 kilogram 150 grams of cedar commodity", ["tag Coopered Staves"]), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition CooperingShapeCypressCooperedStaves() => new()
	{
		Metadata = new("Coopering/shape cypress coopered staves", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.AntiquityHousehold.cs:969", []),
		RequiredAnyEra = ["antiquity"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "shape cypress coopered staves",
		Category = "Coopering",
		Blurb = "shape cypress into coopered staves",
		Action = "shape cypress coopered staves",
		ActiveCraftItemSdesc = "an in-progress cypress coopered staves craft",
		Gate = CatalogueRecipeGates.AncientCooperingCoopering15,
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
			new(25, "$0 inspect|inspects $i1 and sort|sorts out the usable material.", "$0 inspect|inspects $i1 and sort|sorts out the usable material.", ExertionLevel.Stasis, 0),
			new(35, "$0 work|works the material into a consistent prepared stock, using $t1 and $t2.", "$0 work|works the material into a consistent prepared stock, using $t1 and $t2.", ExertionLevel.Stasis, 0),
			new(25, "$0 finish|finishes the preparation and set|sets aside $p1.", "$0 finish|finishes the preparation, but the stock is not worth keeping.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1500.0, Material: "cypress", PileTag: "Furniture Timber Stock", CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1 kilogram 500 grams of cypress", ["piletag Furniture Timber Stock"]), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Cooper's Adze", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Cooper's Adze tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Croze", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Croze tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueCommodityRecipeProduct(Grams: 1150.0, Material: "cypress", Tag: "Coopered Staves", Characteristics: []) { Compatibility = new("1 kilogram 150 grams of cypress commodity", ["tag Coopered Staves"]), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition CooperingShapePineCooperedStaves() => new()
	{
		Metadata = new("Coopering/shape pine coopered staves", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.AntiquityHousehold.cs:969", []),
		RequiredAnyEra = ["antiquity"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "shape pine coopered staves",
		Category = "Coopering",
		Blurb = "shape pine into coopered staves",
		Action = "shape pine coopered staves",
		ActiveCraftItemSdesc = "an in-progress pine coopered staves craft",
		Gate = CatalogueRecipeGates.AncientCooperingCoopering15,
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
			new(25, "$0 inspect|inspects $i1 and sort|sorts out the usable material.", "$0 inspect|inspects $i1 and sort|sorts out the usable material.", ExertionLevel.Stasis, 0),
			new(35, "$0 work|works the material into a consistent prepared stock, using $t1 and $t2.", "$0 work|works the material into a consistent prepared stock, using $t1 and $t2.", ExertionLevel.Stasis, 0),
			new(25, "$0 finish|finishes the preparation and set|sets aside $p1.", "$0 finish|finishes the preparation, but the stock is not worth keeping.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1500.0, Material: "pine", PileTag: "Furniture Timber Stock", CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1 kilogram 500 grams of pine", ["piletag Furniture Timber Stock"]), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Cooper's Adze", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Cooper's Adze tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Croze", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Croze tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueCommodityRecipeProduct(Grams: 1150.0, Material: "pine", Tag: "Coopered Staves", Characteristics: []) { Compatibility = new("1 kilogram 150 grams of pine commodity", ["tag Coopered Staves"]), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition CooperingShapeWillowCooperedStaves() => new()
	{
		Metadata = new("Coopering/shape willow coopered staves", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.AntiquityHousehold.cs:969", []),
		RequiredAnyEra = ["antiquity"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "shape willow coopered staves",
		Category = "Coopering",
		Blurb = "shape willow into coopered staves",
		Action = "shape willow coopered staves",
		ActiveCraftItemSdesc = "an in-progress willow coopered staves craft",
		Gate = CatalogueRecipeGates.AncientCooperingCoopering15,
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
			new(25, "$0 inspect|inspects $i1 and sort|sorts out the usable material.", "$0 inspect|inspects $i1 and sort|sorts out the usable material.", ExertionLevel.Stasis, 0),
			new(35, "$0 work|works the material into a consistent prepared stock, using $t1 and $t2.", "$0 work|works the material into a consistent prepared stock, using $t1 and $t2.", ExertionLevel.Stasis, 0),
			new(25, "$0 finish|finishes the preparation and set|sets aside $p1.", "$0 finish|finishes the preparation, but the stock is not worth keeping.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1500.0, Material: "willow", PileTag: "Furniture Timber Stock", CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1 kilogram 500 grams of willow", ["piletag Furniture Timber Stock"]), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Cooper's Adze", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Cooper's Adze tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Croze", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Croze tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueCommodityRecipeProduct(Grams: 1150.0, Material: "willow", Tag: "Coopered Staves", Characteristics: []) { Compatibility = new("1 kilogram 150 grams of willow commodity", ["tag Coopered Staves"]), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition CooperingShapeWoodCooperedStaves() => new()
	{
		Metadata = new("Coopering/shape wood coopered staves", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.AntiquityHousehold.cs:969", []),
		RequiredAnyEra = ["antiquity"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "shape wood coopered staves",
		Category = "Coopering",
		Blurb = "shape wood into coopered staves",
		Action = "shape wood coopered staves",
		ActiveCraftItemSdesc = "an in-progress wood coopered staves craft",
		Gate = CatalogueRecipeGates.AncientCooperingCoopering15,
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
			new(25, "$0 inspect|inspects $i1 and sort|sorts out the usable material.", "$0 inspect|inspects $i1 and sort|sorts out the usable material.", ExertionLevel.Stasis, 0),
			new(35, "$0 work|works the material into a consistent prepared stock, using $t1 and $t2.", "$0 work|works the material into a consistent prepared stock, using $t1 and $t2.", ExertionLevel.Stasis, 0),
			new(25, "$0 finish|finishes the preparation and set|sets aside $p1.", "$0 finish|finishes the preparation, but the stock is not worth keeping.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1500.0, Material: "wood", PileTag: "Furniture Timber Stock", CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1 kilogram 500 grams of wood", ["piletag Furniture Timber Stock"]), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Cooper's Adze", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Cooper's Adze tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Croze", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Croze tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueCommodityRecipeProduct(Grams: 1150.0, Material: "wood", Tag: "Coopered Staves", Characteristics: []) { Compatibility = new("1 kilogram 150 grams of wood commodity", ["tag Coopered Staves"]), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition BlacksmithingMakeBronzeHoopStock() => new()
	{
		Metadata = new("Blacksmithing/make bronze hoop stock", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.AntiquityHousehold.cs:969", []),
		RequiredAnyEra = ["antiquity"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make bronze hoop stock",
		Category = "Blacksmithing",
		Blurb = "make bronze hoop stock",
		Action = "make bronze hoop stock",
		ActiveCraftItemSdesc = "an in-progress bronze hoop stock craft",
		Gate = CatalogueRecipeGates.AncientCooperingBlacksmithing15,
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
			new(25, "$0 inspect|inspects $i1 and sort|sorts out the usable material.", "$0 inspect|inspects $i1 and sort|sorts out the usable material.", ExertionLevel.Stasis, 0),
			new(35, "$0 work|works the material into a consistent prepared stock, using $t1 and $t2.", "$0 work|works the material into a consistent prepared stock, using $t1 and $t2.", ExertionLevel.Stasis, 0),
			new(25, "$0 finish|finishes the preparation and set|sets aside $p1.", "$0 finish|finishes the preparation, but the stock is not worth keeping.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 800.0, Material: "bronze", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("800 grams of bronze", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Anvil", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Anvil tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueCommodityRecipeProduct(Grams: 650.0, Material: "bronze", Tag: "Hoop Stock", Characteristics: []) { Compatibility = new("650 grams of bronze commodity", ["tag Hoop Stock"]), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition BlacksmithingMakeWroughtIronHoopStock() => new()
	{
		Metadata = new("Blacksmithing/make wrought iron hoop stock", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.AntiquityHousehold.cs:969", []),
		RequiredAnyEra = ["antiquity"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make wrought iron hoop stock",
		Category = "Blacksmithing",
		Blurb = "make wrought iron hoop stock",
		Action = "make wrought iron hoop stock",
		ActiveCraftItemSdesc = "an in-progress wrought iron hoop stock craft",
		Gate = CatalogueRecipeGates.AncientCooperingBlacksmithing15,
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
			new(25, "$0 inspect|inspects $i1 and sort|sorts out the usable material.", "$0 inspect|inspects $i1 and sort|sorts out the usable material.", ExertionLevel.Stasis, 0),
			new(35, "$0 work|works the material into a consistent prepared stock, using $t1 and $t2.", "$0 work|works the material into a consistent prepared stock, using $t1 and $t2.", ExertionLevel.Stasis, 0),
			new(25, "$0 finish|finishes the preparation and set|sets aside $p1.", "$0 finish|finishes the preparation, but the stock is not worth keeping.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 800.0, Material: "wrought iron", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("800 grams of wrought iron", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Anvil", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Anvil tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueCommodityRecipeProduct(Grams: 650.0, Material: "wrought iron", Tag: "Hoop Stock", Characteristics: []) { Compatibility = new("650 grams of wrought iron commodity", ["tag Hoop Stock"]), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition CooperingMakeWillowHoopStock() => new()
	{
		Metadata = new("Coopering/make willow hoop stock", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.AntiquityHousehold.cs:969", []),
		RequiredAnyEra = ["antiquity"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make willow hoop stock",
		Category = "Coopering",
		Blurb = "make willow hoop stock",
		Action = "make willow hoop stock",
		ActiveCraftItemSdesc = "an in-progress willow hoop stock craft",
		Gate = CatalogueRecipeGates.AncientCooperingCoopering15,
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
			new(25, "$0 inspect|inspects $i1 and sort|sorts out the usable material.", "$0 inspect|inspects $i1 and sort|sorts out the usable material.", ExertionLevel.Stasis, 0),
			new(35, "$0 work|works the material into a consistent prepared stock, using $t1 and $t2.", "$0 work|works the material into a consistent prepared stock, using $t1 and $t2.", ExertionLevel.Stasis, 0),
			new(25, "$0 finish|finishes the preparation and set|sets aside $p1.", "$0 finish|finishes the preparation, but the stock is not worth keeping.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 800.0, Material: "willow", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("800 grams of willow", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Basket Knife", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Basket Knife tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Packing Bone", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Packing Bone tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueCommodityRecipeProduct(Grams: 650.0, Material: "willow", Tag: "Hoop Stock", Characteristics: []) { Compatibility = new("650 grams of willow commodity", ["tag Hoop Stock"]), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition CooperingMakeWoodHoopStock() => new()
	{
		Metadata = new("Coopering/make wood hoop stock", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.AntiquityHousehold.cs:969", []),
		RequiredAnyEra = ["antiquity"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make wood hoop stock",
		Category = "Coopering",
		Blurb = "make wood hoop stock",
		Action = "make wood hoop stock",
		ActiveCraftItemSdesc = "an in-progress wood hoop stock craft",
		Gate = CatalogueRecipeGates.AncientCooperingCoopering15,
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
			new(25, "$0 inspect|inspects $i1 and sort|sorts out the usable material.", "$0 inspect|inspects $i1 and sort|sorts out the usable material.", ExertionLevel.Stasis, 0),
			new(35, "$0 work|works the material into a consistent prepared stock, using $t1 and $t2.", "$0 work|works the material into a consistent prepared stock, using $t1 and $t2.", ExertionLevel.Stasis, 0),
			new(25, "$0 finish|finishes the preparation and set|sets aside $p1.", "$0 finish|finishes the preparation, but the stock is not worth keeping.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 800.0, Material: "wood", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("800 grams of wood", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Basket Knife", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Basket Knife tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Packing Bone", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Packing Bone tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueCommodityRecipeProduct(Grams: 650.0, Material: "wood", Tag: "Hoop Stock", Characteristics: []) { Compatibility = new("650 grams of wood commodity", ["tag Hoop Stock"]), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition BasketrySplitWillowBasketrySplints() => new()
	{
		Metadata = new("Basketry/split willow basketry splints", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.AntiquityHousehold.cs:969", []),
		RequiredAnyEra = ["antiquity"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "split willow basketry splints",
		Category = "Basketry",
		Blurb = "split willow into basketry splints",
		Action = "split willow basketry splints",
		ActiveCraftItemSdesc = "an in-progress split willow basketry splints craft",
		Gate = CatalogueRecipeGates.AncientBasketryBasketry15,
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
			new(25, "$0 inspect|inspects $i1 and sort|sorts out the usable material.", "$0 inspect|inspects $i1 and sort|sorts out the usable material.", ExertionLevel.Stasis, 0),
			new(35, "$0 work|works the material into a consistent prepared stock, using $t1 and $t2.", "$0 work|works the material into a consistent prepared stock, using $t1 and $t2.", ExertionLevel.Stasis, 0),
			new(25, "$0 finish|finishes the preparation and set|sets aside $p1.", "$0 finish|finishes the preparation, but the stock is not worth keeping.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1000.0, Material: "willow", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1 kilogram of willow", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Basket Knife", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Basket Knife tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Reed Splitter", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Reed Splitter tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueCommodityRecipeProduct(Grams: 850.0, Material: "willow", Tag: "Basketry Splint", Characteristics: []) { Compatibility = new("850 grams of willow commodity", ["tag Basketry Splint"]), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition BasketryWeaveWillowReedMatting() => new()
	{
		Metadata = new("Basketry/weave willow reed matting", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.AntiquityHousehold.cs:969", []),
		RequiredAnyEra = ["antiquity"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "weave willow reed matting",
		Category = "Basketry",
		Blurb = "weave willow into reed matting",
		Action = "weave willow reed matting",
		ActiveCraftItemSdesc = "an in-progress weave willow reed matting craft",
		Gate = CatalogueRecipeGates.AncientBasketryBasketry15,
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
			new(25, "$0 inspect|inspects $i1 and sort|sorts out the usable material.", "$0 inspect|inspects $i1 and sort|sorts out the usable material.", ExertionLevel.Stasis, 0),
			new(35, "$0 work|works the material into a consistent prepared stock, using $t1 and $t2.", "$0 work|works the material into a consistent prepared stock, using $t1 and $t2.", ExertionLevel.Stasis, 0),
			new(25, "$0 finish|finishes the preparation and set|sets aside $p1.", "$0 finish|finishes the preparation, but the stock is not worth keeping.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1000.0, Material: "willow", PileTag: "Basketry Splint", CharacteristicMode: CatalogueCharacteristicMode.Specific, Characteristics: [new("Colour", null), new("Fine Colour", null)]) { Compatibility = new("1 kilogram of willow", ["piletag Basketry Splint", "characteristic Colour any", "characteristic Fine Colour any"]), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Weaving Bodkin", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Weaving Bodkin tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Packing Bone", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Packing Bone tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueCommodityRecipeProduct(Grams: 800.0, Material: "willow", Tag: "Reed Matting", Characteristics: [new("Colour", null, 1), new("Fine Colour", null, 1)]) { Compatibility = new("800 grams of willow commodity", ["tag Reed Matting", "characteristic Colour from $i1", "characteristic Fine Colour from $i1"]), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition BasketrySplitPapyrusBasketrySplints() => new()
	{
		Metadata = new("Basketry/split papyrus basketry splints", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.AntiquityHousehold.cs:969", []),
		RequiredAnyEra = ["antiquity"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "split papyrus basketry splints",
		Category = "Basketry",
		Blurb = "split papyrus into basketry splints",
		Action = "split papyrus basketry splints",
		ActiveCraftItemSdesc = "an in-progress split papyrus basketry splints craft",
		Gate = CatalogueRecipeGates.AncientBasketryBasketry15,
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
			new(25, "$0 inspect|inspects $i1 and sort|sorts out the usable material.", "$0 inspect|inspects $i1 and sort|sorts out the usable material.", ExertionLevel.Stasis, 0),
			new(35, "$0 work|works the material into a consistent prepared stock, using $t1 and $t2.", "$0 work|works the material into a consistent prepared stock, using $t1 and $t2.", ExertionLevel.Stasis, 0),
			new(25, "$0 finish|finishes the preparation and set|sets aside $p1.", "$0 finish|finishes the preparation, but the stock is not worth keeping.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1000.0, Material: "papyrus", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1 kilogram of papyrus", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Basket Knife", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Basket Knife tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Reed Splitter", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Reed Splitter tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueCommodityRecipeProduct(Grams: 850.0, Material: "papyrus", Tag: "Basketry Splint", Characteristics: []) { Compatibility = new("850 grams of papyrus commodity", ["tag Basketry Splint"]), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition BasketryWeavePapyrusReedMatting() => new()
	{
		Metadata = new("Basketry/weave papyrus reed matting", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.AntiquityHousehold.cs:969", []),
		RequiredAnyEra = ["antiquity"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "weave papyrus reed matting",
		Category = "Basketry",
		Blurb = "weave papyrus into reed matting",
		Action = "weave papyrus reed matting",
		ActiveCraftItemSdesc = "an in-progress weave papyrus reed matting craft",
		Gate = CatalogueRecipeGates.AncientBasketryBasketry15,
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
			new(25, "$0 inspect|inspects $i1 and sort|sorts out the usable material.", "$0 inspect|inspects $i1 and sort|sorts out the usable material.", ExertionLevel.Stasis, 0),
			new(35, "$0 work|works the material into a consistent prepared stock, using $t1 and $t2.", "$0 work|works the material into a consistent prepared stock, using $t1 and $t2.", ExertionLevel.Stasis, 0),
			new(25, "$0 finish|finishes the preparation and set|sets aside $p1.", "$0 finish|finishes the preparation, but the stock is not worth keeping.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1000.0, Material: "papyrus", PileTag: "Basketry Splint", CharacteristicMode: CatalogueCharacteristicMode.Specific, Characteristics: [new("Colour", null), new("Fine Colour", null)]) { Compatibility = new("1 kilogram of papyrus", ["piletag Basketry Splint", "characteristic Colour any", "characteristic Fine Colour any"]), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Weaving Bodkin", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Weaving Bodkin tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Packing Bone", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Packing Bone tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueCommodityRecipeProduct(Grams: 800.0, Material: "papyrus", Tag: "Reed Matting", Characteristics: [new("Colour", null, 1), new("Fine Colour", null, 1)]) { Compatibility = new("800 grams of papyrus commodity", ["tag Reed Matting", "characteristic Colour from $i1", "characteristic Fine Colour from $i1"]), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition BasketrySplitHempBasketrySplints() => new()
	{
		Metadata = new("Basketry/split hemp basketry splints", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.AntiquityHousehold.cs:969", []),
		RequiredAnyEra = ["antiquity"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "split hemp basketry splints",
		Category = "Basketry",
		Blurb = "split hemp into basketry splints",
		Action = "split hemp basketry splints",
		ActiveCraftItemSdesc = "an in-progress split hemp basketry splints craft",
		Gate = CatalogueRecipeGates.AncientBasketryBasketry15,
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
			new(25, "$0 inspect|inspects $i1 and sort|sorts out the usable material.", "$0 inspect|inspects $i1 and sort|sorts out the usable material.", ExertionLevel.Stasis, 0),
			new(35, "$0 work|works the material into a consistent prepared stock, using $t1 and $t2.", "$0 work|works the material into a consistent prepared stock, using $t1 and $t2.", ExertionLevel.Stasis, 0),
			new(25, "$0 finish|finishes the preparation and set|sets aside $p1.", "$0 finish|finishes the preparation, but the stock is not worth keeping.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1000.0, Material: "hemp", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1 kilogram of hemp", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Basket Knife", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Basket Knife tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Reed Splitter", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Reed Splitter tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueCommodityRecipeProduct(Grams: 850.0, Material: "hemp", Tag: "Basketry Splint", Characteristics: []) { Compatibility = new("850 grams of hemp commodity", ["tag Basketry Splint"]), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition BasketryWeaveHempReedMatting() => new()
	{
		Metadata = new("Basketry/weave hemp reed matting", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.AntiquityHousehold.cs:969", []),
		RequiredAnyEra = ["antiquity"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "weave hemp reed matting",
		Category = "Basketry",
		Blurb = "weave hemp into reed matting",
		Action = "weave hemp reed matting",
		ActiveCraftItemSdesc = "an in-progress weave hemp reed matting craft",
		Gate = CatalogueRecipeGates.AncientBasketryBasketry15,
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
			new(25, "$0 inspect|inspects $i1 and sort|sorts out the usable material.", "$0 inspect|inspects $i1 and sort|sorts out the usable material.", ExertionLevel.Stasis, 0),
			new(35, "$0 work|works the material into a consistent prepared stock, using $t1 and $t2.", "$0 work|works the material into a consistent prepared stock, using $t1 and $t2.", ExertionLevel.Stasis, 0),
			new(25, "$0 finish|finishes the preparation and set|sets aside $p1.", "$0 finish|finishes the preparation, but the stock is not worth keeping.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1000.0, Material: "hemp", PileTag: "Basketry Splint", CharacteristicMode: CatalogueCharacteristicMode.Specific, Characteristics: [new("Colour", null), new("Fine Colour", null)]) { Compatibility = new("1 kilogram of hemp", ["piletag Basketry Splint", "characteristic Colour any", "characteristic Fine Colour any"]), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Weaving Bodkin", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Weaving Bodkin tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Packing Bone", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Packing Bone tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueCommodityRecipeProduct(Grams: 800.0, Material: "hemp", Tag: "Reed Matting", Characteristics: [new("Colour", null, 1), new("Fine Colour", null, 1)]) { Compatibility = new("800 grams of hemp commodity", ["tag Reed Matting", "characteristic Colour from $i1", "characteristic Fine Colour from $i1"]), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition LeathermakingPrepareDeerLeatherLeatherPanelStock() => new()
	{
		Metadata = new("Leathermaking/prepare deer leather leather panel stock", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.AntiquityHousehold.cs:969", []),
		RequiredAnyEra = ["antiquity"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "prepare deer leather leather panel stock",
		Category = "Leathermaking",
		Blurb = "prepare deer leather into container panels",
		Action = "prepare deer leather leather panel stock",
		ActiveCraftItemSdesc = "an in-progress deer leather container panels craft",
		Gate = CatalogueRecipeGates.AncientLeatherContainersLeathermaking15,
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
			new(25, "$0 inspect|inspects $i1 and sort|sorts out the usable material.", "$0 inspect|inspects $i1 and sort|sorts out the usable material.", ExertionLevel.Stasis, 0),
			new(35, "$0 work|works the material into a consistent prepared stock, using $t1 and $t2.", "$0 work|works the material into a consistent prepared stock, using $t1 and $t2.", ExertionLevel.Stasis, 0),
			new(25, "$0 finish|finishes the preparation and set|sets aside $p1.", "$0 finish|finishes the preparation, but the stock is not worth keeping.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1000.0, Material: "deer leather", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Specific, Characteristics: [new("Colour", null), new("Fine Colour", null)]) { Compatibility = new("1 kilogram of deer leather", ["characteristic Colour any", "characteristic Fine Colour any"]), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Awl Punch", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Awl Punch tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Leather Stitching Pony", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Leather Stitching Pony tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueCommodityRecipeProduct(Grams: 850.0, Material: "deer leather", Tag: "Prepared Leather Panel", Characteristics: [new("Colour", null, 1), new("Fine Colour", null, 1)]) { Compatibility = new("850 grams of deer leather commodity", ["tag Prepared Leather Panel", "characteristic Colour from $i1", "characteristic Fine Colour from $i1"]), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition LeathermakingPrepareFurLeatherPanelStock() => new()
	{
		Metadata = new("Leathermaking/prepare fur leather panel stock", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.AntiquityHousehold.cs:969", []),
		RequiredAnyEra = ["antiquity"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "prepare fur leather panel stock",
		Category = "Leathermaking",
		Blurb = "prepare fur into container panels",
		Action = "prepare fur leather panel stock",
		ActiveCraftItemSdesc = "an in-progress fur container panels craft",
		Gate = CatalogueRecipeGates.AncientLeatherContainersLeathermaking15,
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
			new(25, "$0 inspect|inspects $i1 and sort|sorts out the usable material.", "$0 inspect|inspects $i1 and sort|sorts out the usable material.", ExertionLevel.Stasis, 0),
			new(35, "$0 work|works the material into a consistent prepared stock, using $t1 and $t2.", "$0 work|works the material into a consistent prepared stock, using $t1 and $t2.", ExertionLevel.Stasis, 0),
			new(25, "$0 finish|finishes the preparation and set|sets aside $p1.", "$0 finish|finishes the preparation, but the stock is not worth keeping.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1000.0, Material: "fur", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Specific, Characteristics: [new("Colour", null), new("Fine Colour", null)]) { Compatibility = new("1 kilogram of fur", ["characteristic Colour any", "characteristic Fine Colour any"]), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Awl Punch", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Awl Punch tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Leather Stitching Pony", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Leather Stitching Pony tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueCommodityRecipeProduct(Grams: 850.0, Material: "fur", Tag: "Prepared Leather Panel", Characteristics: [new("Colour", null, 1), new("Fine Colour", null, 1)]) { Compatibility = new("850 grams of fur commodity", ["tag Prepared Leather Panel", "characteristic Colour from $i1", "characteristic Fine Colour from $i1"]), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition LeathermakingPrepareLeatherLeatherPanelStock() => new()
	{
		Metadata = new("Leathermaking/prepare leather leather panel stock", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.AntiquityHousehold.cs:969", []),
		RequiredAnyEra = ["antiquity"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "prepare leather leather panel stock",
		Category = "Leathermaking",
		Blurb = "prepare leather into container panels",
		Action = "prepare leather leather panel stock",
		ActiveCraftItemSdesc = "an in-progress leather container panels craft",
		Gate = CatalogueRecipeGates.AncientLeatherContainersLeathermaking15,
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
			new(25, "$0 inspect|inspects $i1 and sort|sorts out the usable material.", "$0 inspect|inspects $i1 and sort|sorts out the usable material.", ExertionLevel.Stasis, 0),
			new(35, "$0 work|works the material into a consistent prepared stock, using $t1 and $t2.", "$0 work|works the material into a consistent prepared stock, using $t1 and $t2.", ExertionLevel.Stasis, 0),
			new(25, "$0 finish|finishes the preparation and set|sets aside $p1.", "$0 finish|finishes the preparation, but the stock is not worth keeping.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1000.0, Material: "leather", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Specific, Characteristics: [new("Colour", null), new("Fine Colour", null)]) { Compatibility = new("1 kilogram of leather", ["characteristic Colour any", "characteristic Fine Colour any"]), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Awl Punch", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Awl Punch tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Leather Stitching Pony", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Leather Stitching Pony tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueCommodityRecipeProduct(Grams: 850.0, Material: "leather", Tag: "Prepared Leather Panel", Characteristics: [new("Colour", null, 1), new("Fine Colour", null, 1)]) { Compatibility = new("850 grams of leather commodity", ["tag Prepared Leather Panel", "characteristic Colour from $i1", "characteristic Fine Colour from $i1"]), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition TailoringPrepareCanvasHouseholdClothPanels() => new()
	{
		Metadata = new("Tailoring/prepare canvas household cloth panels", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.AntiquityHousehold.cs:969", []),
		RequiredAnyEra = ["antiquity"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "prepare canvas household cloth panels",
		Category = "Tailoring",
		Blurb = "prepare canvas into household cloth panels",
		Action = "prepare canvas household cloth panels",
		ActiveCraftItemSdesc = "an in-progress canvas household cloth panels craft",
		Gate = CatalogueRecipeGates.AncientHouseholdCraftingTailoring15,
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
			new(25, "$0 inspect|inspects $i1 and sort|sorts out the usable material.", "$0 inspect|inspects $i1 and sort|sorts out the usable material.", ExertionLevel.Stasis, 0),
			new(35, "$0 work|works the material into a consistent prepared stock, using $t1 and $t2.", "$0 work|works the material into a consistent prepared stock, using $t1 and $t2.", ExertionLevel.Stasis, 0),
			new(25, "$0 finish|finishes the preparation and set|sets aside $p1.", "$0 finish|finishes the preparation, but the stock is not worth keeping.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1000.0, Material: "canvas", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Specific, Characteristics: [new("Colour", null), new("Fine Colour", null)]) { Compatibility = new("1 kilogram of canvas", ["characteristic Colour any", "characteristic Fine Colour any"]), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Sewing Needle", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Sewing Needle tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Shears", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Shears tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueCommodityRecipeProduct(Grams: 850.0, Material: "canvas", Tag: "Garment Cloth", Characteristics: [new("Colour", null, 1), new("Fine Colour", null, 1)]) { Compatibility = new("850 grams of canvas commodity", ["tag Garment Cloth", "characteristic Colour from $i1", "characteristic Fine Colour from $i1"]), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition TailoringPrepareFeltHouseholdClothPanels() => new()
	{
		Metadata = new("Tailoring/prepare felt household cloth panels", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.AntiquityHousehold.cs:969", []),
		RequiredAnyEra = ["antiquity"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "prepare felt household cloth panels",
		Category = "Tailoring",
		Blurb = "prepare felt into household cloth panels",
		Action = "prepare felt household cloth panels",
		ActiveCraftItemSdesc = "an in-progress felt household cloth panels craft",
		Gate = CatalogueRecipeGates.AncientHouseholdCraftingTailoring15,
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
			new(25, "$0 inspect|inspects $i1 and sort|sorts out the usable material.", "$0 inspect|inspects $i1 and sort|sorts out the usable material.", ExertionLevel.Stasis, 0),
			new(35, "$0 work|works the material into a consistent prepared stock, using $t1 and $t2.", "$0 work|works the material into a consistent prepared stock, using $t1 and $t2.", ExertionLevel.Stasis, 0),
			new(25, "$0 finish|finishes the preparation and set|sets aside $p1.", "$0 finish|finishes the preparation, but the stock is not worth keeping.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1000.0, Material: "felt", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Specific, Characteristics: [new("Colour", null), new("Fine Colour", null)]) { Compatibility = new("1 kilogram of felt", ["characteristic Colour any", "characteristic Fine Colour any"]), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Sewing Needle", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Sewing Needle tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Shears", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Shears tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueCommodityRecipeProduct(Grams: 850.0, Material: "felt", Tag: "Garment Cloth", Characteristics: [new("Colour", null, 1), new("Fine Colour", null, 1)]) { Compatibility = new("850 grams of felt commodity", ["tag Garment Cloth", "characteristic Colour from $i1", "characteristic Fine Colour from $i1"]), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition TailoringPrepareHempHouseholdClothPanels() => new()
	{
		Metadata = new("Tailoring/prepare hemp household cloth panels", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.AntiquityHousehold.cs:969", []),
		RequiredAnyEra = ["antiquity"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "prepare hemp household cloth panels",
		Category = "Tailoring",
		Blurb = "prepare hemp into household cloth panels",
		Action = "prepare hemp household cloth panels",
		ActiveCraftItemSdesc = "an in-progress hemp household cloth panels craft",
		Gate = CatalogueRecipeGates.AncientHouseholdCraftingTailoring15,
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
			new(25, "$0 inspect|inspects $i1 and sort|sorts out the usable material.", "$0 inspect|inspects $i1 and sort|sorts out the usable material.", ExertionLevel.Stasis, 0),
			new(35, "$0 work|works the material into a consistent prepared stock, using $t1 and $t2.", "$0 work|works the material into a consistent prepared stock, using $t1 and $t2.", ExertionLevel.Stasis, 0),
			new(25, "$0 finish|finishes the preparation and set|sets aside $p1.", "$0 finish|finishes the preparation, but the stock is not worth keeping.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1000.0, Material: "hemp", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Specific, Characteristics: [new("Colour", null), new("Fine Colour", null)]) { Compatibility = new("1 kilogram of hemp", ["characteristic Colour any", "characteristic Fine Colour any"]), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Sewing Needle", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Sewing Needle tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Shears", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Shears tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueCommodityRecipeProduct(Grams: 850.0, Material: "hemp", Tag: "Garment Cloth", Characteristics: [new("Colour", null, 1), new("Fine Colour", null, 1)]) { Compatibility = new("850 grams of hemp commodity", ["tag Garment Cloth", "characteristic Colour from $i1", "characteristic Fine Colour from $i1"]), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition TailoringPrepareLinenHouseholdClothPanels() => new()
	{
		Metadata = new("Tailoring/prepare linen household cloth panels", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.AntiquityHousehold.cs:969", []),
		RequiredAnyEra = ["antiquity"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "prepare linen household cloth panels",
		Category = "Tailoring",
		Blurb = "prepare linen into household cloth panels",
		Action = "prepare linen household cloth panels",
		ActiveCraftItemSdesc = "an in-progress linen household cloth panels craft",
		Gate = CatalogueRecipeGates.AncientHouseholdCraftingTailoring15,
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
			new(25, "$0 inspect|inspects $i1 and sort|sorts out the usable material.", "$0 inspect|inspects $i1 and sort|sorts out the usable material.", ExertionLevel.Stasis, 0),
			new(35, "$0 work|works the material into a consistent prepared stock, using $t1 and $t2.", "$0 work|works the material into a consistent prepared stock, using $t1 and $t2.", ExertionLevel.Stasis, 0),
			new(25, "$0 finish|finishes the preparation and set|sets aside $p1.", "$0 finish|finishes the preparation, but the stock is not worth keeping.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1000.0, Material: "linen", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Specific, Characteristics: [new("Colour", null), new("Fine Colour", null)]) { Compatibility = new("1 kilogram of linen", ["characteristic Colour any", "characteristic Fine Colour any"]), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Sewing Needle", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Sewing Needle tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Shears", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Shears tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueCommodityRecipeProduct(Grams: 850.0, Material: "linen", Tag: "Garment Cloth", Characteristics: [new("Colour", null, 1), new("Fine Colour", null, 1)]) { Compatibility = new("850 grams of linen commodity", ["tag Garment Cloth", "characteristic Colour from $i1", "characteristic Fine Colour from $i1"]), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition TailoringPrepareSilkHouseholdClothPanels() => new()
	{
		Metadata = new("Tailoring/prepare silk household cloth panels", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.AntiquityHousehold.cs:969", []),
		RequiredAnyEra = ["antiquity"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "prepare silk household cloth panels",
		Category = "Tailoring",
		Blurb = "prepare silk into household cloth panels",
		Action = "prepare silk household cloth panels",
		ActiveCraftItemSdesc = "an in-progress silk household cloth panels craft",
		Gate = CatalogueRecipeGates.AncientHouseholdCraftingTailoring15,
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
			new(25, "$0 inspect|inspects $i1 and sort|sorts out the usable material.", "$0 inspect|inspects $i1 and sort|sorts out the usable material.", ExertionLevel.Stasis, 0),
			new(35, "$0 work|works the material into a consistent prepared stock, using $t1 and $t2.", "$0 work|works the material into a consistent prepared stock, using $t1 and $t2.", ExertionLevel.Stasis, 0),
			new(25, "$0 finish|finishes the preparation and set|sets aside $p1.", "$0 finish|finishes the preparation, but the stock is not worth keeping.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1000.0, Material: "silk", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Specific, Characteristics: [new("Colour", null), new("Fine Colour", null)]) { Compatibility = new("1 kilogram of silk", ["characteristic Colour any", "characteristic Fine Colour any"]), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Sewing Needle", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Sewing Needle tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Shears", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Shears tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueCommodityRecipeProduct(Grams: 850.0, Material: "silk", Tag: "Garment Cloth", Characteristics: [new("Colour", null, 1), new("Fine Colour", null, 1)]) { Compatibility = new("850 grams of silk commodity", ["tag Garment Cloth", "characteristic Colour from $i1", "characteristic Fine Colour from $i1"]), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition TailoringPrepareWoolHouseholdClothPanels() => new()
	{
		Metadata = new("Tailoring/prepare wool household cloth panels", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.AntiquityHousehold.cs:969", []),
		RequiredAnyEra = ["antiquity"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "prepare wool household cloth panels",
		Category = "Tailoring",
		Blurb = "prepare wool into household cloth panels",
		Action = "prepare wool household cloth panels",
		ActiveCraftItemSdesc = "an in-progress wool household cloth panels craft",
		Gate = CatalogueRecipeGates.AncientHouseholdCraftingTailoring15,
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
			new(25, "$0 inspect|inspects $i1 and sort|sorts out the usable material.", "$0 inspect|inspects $i1 and sort|sorts out the usable material.", ExertionLevel.Stasis, 0),
			new(35, "$0 work|works the material into a consistent prepared stock, using $t1 and $t2.", "$0 work|works the material into a consistent prepared stock, using $t1 and $t2.", ExertionLevel.Stasis, 0),
			new(25, "$0 finish|finishes the preparation and set|sets aside $p1.", "$0 finish|finishes the preparation, but the stock is not worth keeping.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1000.0, Material: "wool", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Specific, Characteristics: [new("Colour", null), new("Fine Colour", null)]) { Compatibility = new("1 kilogram of wool", ["characteristic Colour any", "characteristic Fine Colour any"]), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Sewing Needle", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Sewing Needle tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Shears", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Shears tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueCommodityRecipeProduct(Grams: 850.0, Material: "wool", Tag: "Garment Cloth", Characteristics: [new("Colour", null, 1), new("Fine Colour", null, 1)]) { Compatibility = new("850 grams of wool commodity", ["tag Garment Cloth", "characteristic Colour from $i1", "characteristic Fine Colour from $i1"]), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition RopemakingTwistLinenLampWick() => new()
	{
		Metadata = new("Ropemaking/twist linen lamp wick", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.AntiquityHousehold.cs:969", []),
		RequiredAnyEra = ["antiquity"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "twist linen lamp wick",
		Category = "Ropemaking",
		Blurb = "twist linen into lamp wick",
		Action = "twist linen lamp wick",
		ActiveCraftItemSdesc = "an in-progress twist linen lamp wick craft",
		Gate = CatalogueRecipeGates.AncientLightingAndHeatingRopemaking15,
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
			new(25, "$0 inspect|inspects $i1 and sort|sorts out the usable material.", "$0 inspect|inspects $i1 and sort|sorts out the usable material.", ExertionLevel.Stasis, 0),
			new(35, "$0 work|works the material into a consistent prepared stock, using $t1 and $t2.", "$0 work|works the material into a consistent prepared stock, using $t1 and $t2.", ExertionLevel.Stasis, 0),
			new(25, "$0 finish|finishes the preparation and set|sets aside $p1.", "$0 finish|finishes the preparation, but the stock is not worth keeping.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 300.0, Material: "linen", PileTag: "Spun Yarn", CharacteristicMode: CatalogueCharacteristicMode.Specific, Characteristics: [new("Colour", null)]) { Compatibility = new("300 grams of linen", ["piletag Spun Yarn", "characteristic Colour any"]), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Rope Hook", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Rope Hook tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Twine Shuttle", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Twine Shuttle tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueCommodityRecipeProduct(Grams: 240.0, Material: "linen", Tag: "Lamp Wick", Characteristics: [new("Colour", null, 1)]) { Compatibility = new("240 grams of linen commodity", ["tag Lamp Wick", "characteristic Colour from $i1"]), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition CandlemakingPurifyBeeswaxCandlemakingStock() => new()
	{
		Metadata = new("Candlemaking/purify beeswax candlemaking stock", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.AntiquityHousehold.cs:969", []),
		RequiredAnyEra = ["antiquity"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "purify beeswax candlemaking stock",
		Category = "Candlemaking",
		Blurb = "purify beeswax for candle moulding",
		Action = "purify beeswax candlemaking stock",
		ActiveCraftItemSdesc = "an in-progress purify beeswax for candle moulding craft",
		Gate = CatalogueRecipeGates.AncientLightingAndHeatingCandlemaking15,
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
			new(25, "$0 inspect|inspects $i1 and sort|sorts out the usable material.", "$0 inspect|inspects $i1 and sort|sorts out the usable material.", ExertionLevel.Stasis, 0),
			new(35, "$0 work|works the material into a consistent prepared stock, using $t1.", "$0 work|works the material into a consistent prepared stock, using $t1.", ExertionLevel.Stasis, 0),
			new(25, "$0 finish|finishes the preparation and set|sets aside $p1.", "$0 finish|finishes the preparation, but the stock is not worth keeping.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1000.0, Material: "beeswax", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1 kilogram of beeswax", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Fire", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Fire tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueCommodityRecipeProduct(Grams: 850.0, Material: "beeswax", Tag: "Candlemaking Wax", Characteristics: []) { Compatibility = new("850 grams of beeswax commodity", ["tag Candlemaking Wax"]), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition CandlemakingPreparePitchForTorches() => new()
	{
		Metadata = new("Candlemaking/prepare pitch for torches", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.AntiquityHousehold.cs:969", []),
		RequiredAnyEra = ["antiquity"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "prepare pitch for torches",
		Category = "Candlemaking",
		Blurb = "prepare pitch for household lighting",
		Action = "prepare pitch for torches",
		ActiveCraftItemSdesc = "an in-progress pitch for household lighting craft",
		Gate = CatalogueRecipeGates.AncientLightingAndHeatingCandlemaking15,
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
			new(25, "$0 inspect|inspects $i1 and sort|sorts out the usable material.", "$0 inspect|inspects $i1 and sort|sorts out the usable material.", ExertionLevel.Stasis, 0),
			new(35, "$0 work|works the material into a consistent prepared stock, using $t1.", "$0 work|works the material into a consistent prepared stock, using $t1.", ExertionLevel.Stasis, 0),
			new(25, "$0 finish|finishes the preparation and set|sets aside $p1.", "$0 finish|finishes the preparation, but the stock is not worth keeping.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1000.0, Material: "pitch", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1 kilogram of pitch", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Fire", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Fire tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueCommodityRecipeProduct(Grams: 850.0, Material: "pitch", Tag: "Prepared Pitch", Characteristics: []) { Compatibility = new("850 grams of pitch commodity", ["tag Prepared Pitch"]), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PotteryLevigateCeramicPotteryClayBody() => new()
	{
		Metadata = new("Pottery/levigate ceramic pottery clay body", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.AntiquityHousehold.cs:969", []),
		RequiredAnyEra = ["antiquity"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "levigate ceramic pottery clay body",
		Category = "Pottery",
		Blurb = "levigate clay into ceramic pottery body",
		Action = "levigate ceramic pottery clay body",
		ActiveCraftItemSdesc = "an in-progress levigate clay ceramic pottery body craft",
		Gate = CatalogueRecipeGates.AncientCeramicVesselmakingPottery152,
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
			new(25, "$0 inspect|inspects $i1 and sort|sorts out the usable material.", "$0 inspect|inspects $i1 and sort|sorts out the usable material.", ExertionLevel.Stasis, 0),
			new(35, "$0 work|works the material into a consistent prepared stock, using $t1.", "$0 work|works the material into a consistent prepared stock, using $t1.", ExertionLevel.Stasis, 0),
			new(25, "$0 finish|finishes the preparation and set|sets aside $p1.", "$0 finish|finishes the preparation, but the stock is not worth keeping.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1400.0, Material: "clay", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1 kilogram 400 grams of clay", []), QualityWeight = 1 },
			new CatalogueLiquidRecipeInput(Liquid: "Water", Litres: 2.0) { Compatibility = new("2 litres of Water", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Clay Knife", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Clay Knife tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueCommodityRecipeProduct(Grams: 1200.0, Material: "ceramic", Tag: "Pottery Clay Body", Characteristics: []) { Compatibility = new("1 kilogram 200 grams of ceramic commodity", ["tag Pottery Clay Body"]), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};
}
