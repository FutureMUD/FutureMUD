#nullable enable

using MudSharp.Body;
using MudSharp.GameItems.Inventory.Plans;
using MudSharp.RPG.Checks;

namespace DatabaseSeeder.Seeders;

public static partial class Recipes05Stock
{
	public static CatalogueRecipeDefinition FurnitureAndContainersBuildLowTruckleBed() => new()
	{
		Metadata = new("Furniture and Containers/build low truckle bed", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:medieval_furniture_western_truckle_bed"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Medieval.cs:893", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "build low truckle bed",
		Category = "Furniture and Containers",
		Blurb = "build a low truckle bed",
		Action = "building a low truckle bed",
		ActiveCraftItemSdesc = "an in-progress low truckle bed craft",
		Gate = CatalogueRecipeGates.MedievalFurnitureAndContainerCraftingCarpentry10,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 3,
		Interruptable = false,
		Phases = [
			new(30, "$0 inspect|inspects the stock and lay|lays out the work.", "$0 inspect|inspects the stock and lay|lays out the work.", ExertionLevel.Stasis, 0),
			new(45, "$0 shape|shapes and fit|fits the prepared pieces with $t1.", "$0 shape|shapes and fit|fits the prepared pieces with $t1.", ExertionLevel.Stasis, 0),
			new(45, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 botch|botches the finishing work and spoil|spoils the assembly.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 16500.0, Material: "pine", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("16500 grams of pine", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "medieval_furniture_western_truckle_bed", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a low truckle bed (@medieval_furniture_western_truckle_bed)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FurnitureAndContainersBuildPanelBackedChair() => new()
	{
		Metadata = new("Furniture and Containers/build panel-backed chair", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:medieval_furniture_western_wainscot_chair"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Medieval.cs:893", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "build panel-backed chair",
		Category = "Furniture and Containers",
		Blurb = "build a panel-backed chair",
		Action = "building a panel-backed chair",
		ActiveCraftItemSdesc = "an in-progress panel-backed chair craft",
		Gate = CatalogueRecipeGates.MedievalFurnitureAndContainerCraftingCarpentry10,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 3,
		Interruptable = false,
		Phases = [
			new(30, "$0 inspect|inspects the stock and lay|lays out the work.", "$0 inspect|inspects the stock and lay|lays out the work.", ExertionLevel.Stasis, 0),
			new(45, "$0 shape|shapes and fit|fits the prepared pieces with $t1.", "$0 shape|shapes and fit|fits the prepared pieces with $t1.", ExertionLevel.Stasis, 0),
			new(45, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 botch|botches the finishing work and spoil|spoils the assembly.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 9750.0, Material: "oak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("9750 grams of oak", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "medieval_furniture_western_wainscot_chair", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a panel-backed chair (@medieval_furniture_western_wainscot_chair)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FurnitureAndContainersBuildWovenWillowCradle() => new()
	{
		Metadata = new("Furniture and Containers/build woven willow cradle", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:medieval_furniture_wicker_cradle"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Medieval.cs:893", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "build woven willow cradle",
		Category = "Furniture and Containers",
		Blurb = "build a woven willow cradle",
		Action = "building a woven willow cradle",
		ActiveCraftItemSdesc = "an in-progress woven willow cradle craft",
		Gate = CatalogueRecipeGates.MedievalFurnitureAndContainerCraftingCarpentry10,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 3,
		Interruptable = false,
		Phases = [
			new(30, "$0 inspect|inspects the stock and lay|lays out the work.", "$0 inspect|inspects the stock and lay|lays out the work.", ExertionLevel.Stasis, 0),
			new(45, "$0 shape|shapes and fit|fits the prepared pieces with $t1.", "$0 shape|shapes and fit|fits the prepared pieces with $t1.", ExertionLevel.Stasis, 0),
			new(45, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 botch|botches the finishing work and spoil|spoils the assembly.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3375.0, Material: "willow", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3375 grams of willow", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "medieval_furniture_wicker_cradle", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a woven willow cradle (@medieval_furniture_wicker_cradle)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FurnitureAndContainersBuildWideWoodenBed() => new()
	{
		Metadata = new("Furniture and Containers/build wide wooden bed", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:medieval_furniture_wide_bed_frame"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Medieval.cs:893", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "build wide wooden bed",
		Category = "Furniture and Containers",
		Blurb = "build a wide wooden bed",
		Action = "building a wide wooden bed",
		ActiveCraftItemSdesc = "an in-progress wide wooden bed craft",
		Gate = CatalogueRecipeGates.MedievalFurnitureAndContainerCraftingCarpentry10,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 3,
		Interruptable = false,
		Phases = [
			new(30, "$0 inspect|inspects the stock and lay|lays out the work.", "$0 inspect|inspects the stock and lay|lays out the work.", ExertionLevel.Stasis, 0),
			new(45, "$0 shape|shapes and fit|fits the prepared pieces with $t1.", "$0 shape|shapes and fit|fits the prepared pieces with $t1.", ExertionLevel.Stasis, 0),
			new(45, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 botch|botches the finishing work and spoil|spoils the assembly.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 48000.0, Material: "oak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("48000 grams of oak", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "medieval_furniture_wide_bed_frame", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a wide wooden bed (@medieval_furniture_wide_bed_frame)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FurnitureAndContainersBuildNarrowWindowBench() => new()
	{
		Metadata = new("Furniture and Containers/build narrow window bench", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:medieval_furniture_window_bench"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Medieval.cs:893", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "build narrow window bench",
		Category = "Furniture and Containers",
		Blurb = "build a narrow window bench",
		Action = "building a narrow window bench",
		ActiveCraftItemSdesc = "an in-progress narrow window bench craft",
		Gate = CatalogueRecipeGates.MedievalFurnitureAndContainerCraftingCarpentry10,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 3,
		Interruptable = false,
		Phases = [
			new(30, "$0 inspect|inspects the stock and lay|lays out the work.", "$0 inspect|inspects the stock and lay|lays out the work.", ExertionLevel.Stasis, 0),
			new(45, "$0 shape|shapes and fit|fits the prepared pieces with $t1.", "$0 shape|shapes and fit|fits the prepared pieces with $t1.", ExertionLevel.Stasis, 0),
			new(45, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 botch|botches the finishing work and spoil|spoils the assembly.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 9000.0, Material: "ash", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("9000 grams of ash", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "medieval_furniture_window_bench", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a narrow window bench (@medieval_furniture_window_bench)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FurnitureAndContainersBuildWoodenDaybedSeat() => new()
	{
		Metadata = new("Furniture and Containers/build wooden daybed seat", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:medieval_furniture_wooden_daybed_seat"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Medieval.cs:893", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "build wooden daybed seat",
		Category = "Furniture and Containers",
		Blurb = "build a wooden daybed seat",
		Action = "building a wooden daybed seat",
		ActiveCraftItemSdesc = "an in-progress wooden daybed seat craft",
		Gate = CatalogueRecipeGates.MedievalFurnitureAndContainerCraftingCarpentry10,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 3,
		Interruptable = false,
		Phases = [
			new(30, "$0 inspect|inspects the stock and lay|lays out the work.", "$0 inspect|inspects the stock and lay|lays out the work.", ExertionLevel.Stasis, 0),
			new(45, "$0 shape|shapes and fit|fits the prepared pieces with $t1.", "$0 shape|shapes and fit|fits the prepared pieces with $t1.", ExertionLevel.Stasis, 0),
			new(45, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 botch|botches the finishing work and spoil|spoils the assembly.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 18000.0, Material: "oak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("18000 grams of oak", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "medieval_furniture_wooden_daybed_seat", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a wooden daybed seat (@medieval_furniture_wooden_daybed_seat)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FurnitureAndContainersBuildRolledWoolBedroll() => new()
	{
		Metadata = new("Furniture and Containers/build rolled wool bedroll", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:medieval_furniture_wool_bedding_roll"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Medieval.cs:893", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "build rolled wool bedroll",
		Category = "Furniture and Containers",
		Blurb = "build a rolled wool bedroll",
		Action = "building a rolled wool bedroll",
		ActiveCraftItemSdesc = "an in-progress rolled wool bedroll craft",
		Gate = CatalogueRecipeGates.MedievalFurnitureAndContainerCraftingCarpentry10,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 3,
		Interruptable = false,
		Phases = [
			new(30, "$0 inspect|inspects the stock and lay|lays out the work.", "$0 inspect|inspects the stock and lay|lays out the work.", ExertionLevel.Stasis, 0),
			new(45, "$0 shape|shapes and fit|fits the prepared pieces with $t1.", "$0 shape|shapes and fit|fits the prepared pieces with $t1.", ExertionLevel.Stasis, 0),
			new(45, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 botch|botches the finishing work and spoil|spoils the assembly.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3150.0, Material: "wool", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3150 grams of wool", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "medieval_furniture_wool_bedding_roll", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a rolled wool bedroll (@medieval_furniture_wool_bedding_roll)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FurnitureAndContainersBuildWoolSittingMat() => new()
	{
		Metadata = new("Furniture and Containers/build wool sitting mat", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:medieval_furniture_wool_sitting_mat"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Medieval.cs:893", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "build wool sitting mat",
		Category = "Furniture and Containers",
		Blurb = "build a wool sitting mat",
		Action = "building a wool sitting mat",
		ActiveCraftItemSdesc = "an in-progress wool sitting mat craft",
		Gate = CatalogueRecipeGates.MedievalFurnitureAndContainerCraftingCarpentry10,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 3,
		Interruptable = false,
		Phases = [
			new(30, "$0 inspect|inspects the stock and lay|lays out the work.", "$0 inspect|inspects the stock and lay|lays out the work.", ExertionLevel.Stasis, 0),
			new(45, "$0 shape|shapes and fit|fits the prepared pieces with $t1.", "$0 shape|shapes and fit|fits the prepared pieces with $t1.", ExertionLevel.Stasis, 0),
			new(45, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 botch|botches the finishing work and spoil|spoils the assembly.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1875.0, Material: "wool", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1875 grams of wool", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "medieval_furniture_wool_sitting_mat", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a wool sitting mat (@medieval_furniture_wool_sitting_mat)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FurnitureAndContainersBuildWoolSleepingPallet() => new()
	{
		Metadata = new("Furniture and Containers/build wool sleeping pallet", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:medieval_furniture_wool_sleeping_pallet"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Medieval.cs:893", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "build wool sleeping pallet",
		Category = "Furniture and Containers",
		Blurb = "build a wool sleeping pallet",
		Action = "building a wool sleeping pallet",
		ActiveCraftItemSdesc = "an in-progress wool sleeping pallet craft",
		Gate = CatalogueRecipeGates.MedievalFurnitureAndContainerCraftingCarpentry10,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 3,
		Interruptable = false,
		Phases = [
			new(30, "$0 inspect|inspects the stock and lay|lays out the work.", "$0 inspect|inspects the stock and lay|lays out the work.", ExertionLevel.Stasis, 0),
			new(45, "$0 shape|shapes and fit|fits the prepared pieces with $t1.", "$0 shape|shapes and fit|fits the prepared pieces with $t1.", ExertionLevel.Stasis, 0),
			new(45, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 botch|botches the finishing work and spoil|spoils the assembly.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 5700.0, Material: "wool", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("5700 grams of wool", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "medieval_furniture_wool_sleeping_pallet", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a wool sleeping pallet (@medieval_furniture_wool_sleeping_pallet)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FurnitureAndContainersBuildSturdyWorkStool() => new()
	{
		Metadata = new("Furniture and Containers/build sturdy work stool", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:medieval_furniture_work_stool"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Medieval.cs:893", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "build sturdy work stool",
		Category = "Furniture and Containers",
		Blurb = "build a sturdy work stool",
		Action = "building a sturdy work stool",
		ActiveCraftItemSdesc = "an in-progress sturdy work stool craft",
		Gate = CatalogueRecipeGates.MedievalFurnitureAndContainerCraftingCarpentry10,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 3,
		Interruptable = false,
		Phases = [
			new(30, "$0 inspect|inspects the stock and lay|lays out the work.", "$0 inspect|inspects the stock and lay|lays out the work.", ExertionLevel.Stasis, 0),
			new(45, "$0 shape|shapes and fit|fits the prepared pieces with $t1.", "$0 shape|shapes and fit|fits the prepared pieces with $t1.", ExertionLevel.Stasis, 0),
			new(45, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 botch|botches the finishing work and spoil|spoils the assembly.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2700.0, Material: "oak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2700 grams of oak", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "medieval_furniture_work_stool", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a sturdy work stool (@medieval_furniture_work_stool)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MedievalFoodToolmakingMedievalFoodForgeBroadButcherSKnife() => new()
	{
		Metadata = new("Medieval Food / Toolmaking/medieval food - forge broad butcher's knife", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:medieval_tool_butchers_knife"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.MedievalFood.cs:138", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "medieval food - forge broad butcher's knife",
		Category = "Medieval Food / Toolmaking",
		Blurb = "medieval food - forge broad butcher's knife",
		Action = "medieval food - forge broad butcher's knife",
		ActiveCraftItemSdesc = "an in-progress medieval food - forge broad butcher's knife craft",
		Gate = CatalogueRecipeGates.MedievalFoodProductionBlacksmithing20,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 3,
		Interruptable = false,
		Phases = [
			new(30, "$0 inspect|inspects the stock and lay|lays out the foodmaking equipment pieces.", "$0 inspect|inspects the stock and lay|lays out the foodmaking equipment pieces.", ExertionLevel.Stasis, 0),
			new(45, "$0 shape|shapes and fit|fits the pieces together with $t1, using $t2.", "$0 shape|shapes and fit|fits the pieces together with $t1, using $t2.", ExertionLevel.Stasis, 0),
			new(45, "$0 finish|finishes the equipment and set|sets aside $p1.", "$0 botch|botches the finishing work and spoil|spoils the assembly.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueItemRecipeInput(ItemStableReference: "medieval_industry_stock_iron_bar", Quantity: 1) { Compatibility = new("1x a wrought-iron bar (@medieval_industry_stock_iron_bar)", []), QualityWeight = 1 },
			new CatalogueItemRecipeInput(ItemStableReference: "medieval_industry_stock_handle_blanks", Quantity: 1) { Compatibility = new("1x a bundle of handle blanks (@medieval_industry_stock_handle_blanks)", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Anvil", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Anvil tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "medieval_tool_butchers_knife", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a broad iron butcher's knife (@medieval_tool_butchers_knife)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MedievalFoodToolmakingMedievalFoodForgeKitchenCookingKnife() => new()
	{
		Metadata = new("Medieval Food / Toolmaking/medieval food - forge kitchen cooking knife", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:medieval_tool_cooking_knife"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.MedievalFood.cs:138", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "medieval food - forge kitchen cooking knife",
		Category = "Medieval Food / Toolmaking",
		Blurb = "medieval food - forge kitchen cooking knife",
		Action = "medieval food - forge kitchen cooking knife",
		ActiveCraftItemSdesc = "an in-progress medieval food - forge kitchen cooking knife craft",
		Gate = CatalogueRecipeGates.MedievalFoodProductionBlacksmithing20,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 3,
		Interruptable = false,
		Phases = [
			new(30, "$0 inspect|inspects the stock and lay|lays out the foodmaking equipment pieces.", "$0 inspect|inspects the stock and lay|lays out the foodmaking equipment pieces.", ExertionLevel.Stasis, 0),
			new(45, "$0 shape|shapes and fit|fits the pieces together with $t1, using $t2.", "$0 shape|shapes and fit|fits the pieces together with $t1, using $t2.", ExertionLevel.Stasis, 0),
			new(45, "$0 finish|finishes the equipment and set|sets aside $p1.", "$0 botch|botches the finishing work and spoil|spoils the assembly.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueItemRecipeInput(ItemStableReference: "medieval_industry_stock_iron_bar", Quantity: 1) { Compatibility = new("1x a wrought-iron bar (@medieval_industry_stock_iron_bar)", []), QualityWeight = 1 },
			new CatalogueItemRecipeInput(ItemStableReference: "medieval_industry_stock_handle_blanks", Quantity: 1) { Compatibility = new("1x a bundle of handle blanks (@medieval_industry_stock_handle_blanks)", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Anvil", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Anvil tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "medieval_tool_cooking_knife", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x an iron cooking knife (@medieval_tool_cooking_knife)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MedievalFoodToolmakingMedievalFoodBuildJointedThreshingFlail() => new()
	{
		Metadata = new("Medieval Food / Toolmaking/medieval food - build jointed threshing flail", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:medieval_tool_threshing_flail"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.MedievalFood.cs:138", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "medieval food - build jointed threshing flail",
		Category = "Medieval Food / Toolmaking",
		Blurb = "medieval food - build jointed threshing flail",
		Action = "medieval food - build jointed threshing flail",
		ActiveCraftItemSdesc = "an in-progress medieval food - build jointed threshing flail craft",
		Gate = CatalogueRecipeGates.MedievalFoodProductionCarpentry10,
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
			new(30, "$0 inspect|inspects the stock and lay|lays out the foodmaking equipment pieces.", "$0 inspect|inspects the stock and lay|lays out the foodmaking equipment pieces.", ExertionLevel.Stasis, 0),
			new(45, "$0 shape|shapes and fit|fits the pieces together with $t1.", "$0 shape|shapes and fit|fits the pieces together with $t1.", ExertionLevel.Stasis, 0),
			new(45, "$0 finish|finishes the equipment and set|sets aside $p1.", "$0 botch|botches the finishing work and spoil|spoils the assembly.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueItemRecipeInput(ItemStableReference: "medieval_industry_stock_handle_blanks", Quantity: 1) { Compatibility = new("1x a bundle of handle blanks (@medieval_industry_stock_handle_blanks)", []), QualityWeight = 1 },
			new CatalogueItemRecipeInput(ItemStableReference: "medieval_industry_stock_leather_panel", Quantity: 1) { Compatibility = new("1x a tanned leather panel (@medieval_industry_stock_leather_panel)", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Awl Punch", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Awl Punch tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "medieval_tool_threshing_flail", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a jointed wooden threshing flail (@medieval_tool_threshing_flail)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MedievalFoodToolmakingMedievalFoodWeaveBroadWinnowingBasket() => new()
	{
		Metadata = new("Medieval Food / Toolmaking/medieval food - weave broad winnowing basket", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:medieval_tool_winnowing_basket"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.MedievalFood.cs:138", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "medieval food - weave broad winnowing basket",
		Category = "Medieval Food / Toolmaking",
		Blurb = "medieval food - weave broad winnowing basket",
		Action = "medieval food - weave broad winnowing basket",
		ActiveCraftItemSdesc = "an in-progress medieval food - weave broad winnowing basket craft",
		Gate = CatalogueRecipeGates.MedievalFoodProductionBasketry10,
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
			new(30, "$0 inspect|inspects the stock and lay|lays out the foodmaking equipment pieces.", "$0 inspect|inspects the stock and lay|lays out the foodmaking equipment pieces.", ExertionLevel.Stasis, 0),
			new(45, "$0 shape|shapes and fit|fits the pieces together with $t1.", "$0 shape|shapes and fit|fits the pieces together with $t1.", ExertionLevel.Stasis, 0),
			new(45, "$0 finish|finishes the equipment and set|sets aside $p1.", "$0 botch|botches the finishing work and spoil|spoils the assembly.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1200.0, Material: "willow", PileTag: "Basketry Splint", CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1 kilogram 200 grams of willow", ["piletag Basketry Splint"]), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Knife", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Knife tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "medieval_tool_winnowing_basket", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a broad woven winnowing basket (@medieval_tool_winnowing_basket)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MedievalFoodToolmakingMedievalFoodRaiseBronzeCookingPot() => new()
	{
		Metadata = new("Medieval Food / Toolmaking/medieval food - raise bronze cooking pot", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:medieval_tool_cooking_pot"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.MedievalFood.cs:138", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "medieval food - raise bronze cooking pot",
		Category = "Medieval Food / Toolmaking",
		Blurb = "medieval food - raise bronze cooking pot",
		Action = "medieval food - raise bronze cooking pot",
		ActiveCraftItemSdesc = "an in-progress medieval food - raise bronze cooking pot craft",
		Gate = CatalogueRecipeGates.MedievalFoodProductionBlacksmithing20,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 3,
		Interruptable = false,
		Phases = [
			new(30, "$0 inspect|inspects the stock and lay|lays out the foodmaking equipment pieces.", "$0 inspect|inspects the stock and lay|lays out the foodmaking equipment pieces.", ExertionLevel.Stasis, 0),
			new(45, "$0 shape|shapes and fit|fits the pieces together with $t1, using $t2.", "$0 shape|shapes and fit|fits the pieces together with $t1, using $t2.", ExertionLevel.Stasis, 0),
			new(45, "$0 finish|finishes the equipment and set|sets aside $p1.", "$0 botch|botches the finishing work and spoil|spoils the assembly.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueItemRecipeInput(ItemStableReference: "medieval_industry_stock_bronze_bar", Quantity: 1) { Compatibility = new("1x a bronze casting bar (@medieval_industry_stock_bronze_bar)", []), QualityWeight = 1 },
			new CatalogueItemRecipeInput(ItemStableReference: "medieval_industry_stock_wire_coil", Quantity: 1) { Compatibility = new("1x a coil of drawn wire (@medieval_industry_stock_wire_coil)", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Anvil", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Anvil tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "medieval_tool_cooking_pot", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a soot-darkened bronze cooking pot (@medieval_tool_cooking_pot)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MedievalFoodToolmakingMedievalFoodBuildSlattedLauterTun() => new()
	{
		Metadata = new("Medieval Food / Toolmaking/medieval food - build slatted lauter tun", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:medieval_workshop_lauter_tun"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.MedievalFood.cs:138", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "medieval food - build slatted lauter tun",
		Category = "Medieval Food / Toolmaking",
		Blurb = "medieval food - build slatted lauter tun",
		Action = "medieval food - build slatted lauter tun",
		ActiveCraftItemSdesc = "an in-progress medieval food - build slatted lauter tun craft",
		Gate = CatalogueRecipeGates.MedievalFoodProductionCoopering25,
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
			new(30, "$0 inspect|inspects the stock and lay|lays out the foodmaking equipment pieces.", "$0 inspect|inspects the stock and lay|lays out the foodmaking equipment pieces.", ExertionLevel.Stasis, 0),
			new(45, "$0 shape|shapes and fit|fits the pieces together with $t1, using $t2.", "$0 shape|shapes and fit|fits the pieces together with $t1, using $t2.", ExertionLevel.Stasis, 0),
			new(45, "$0 finish|finishes the equipment and set|sets aside $p1.", "$0 botch|botches the finishing work and spoil|spoils the assembly.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueItemRecipeInput(ItemStableReference: "medieval_industry_stock_plank_bundle", Quantity: 1) { Compatibility = new("1x a bundle of worked planks (@medieval_industry_stock_plank_bundle)", []), QualityWeight = 1 },
			new CatalogueItemRecipeInput(ItemStableReference: "medieval_industry_stock_rivet_packet", Quantity: 1) { Compatibility = new("1x a packet of iron rivets (@medieval_industry_stock_rivet_packet)", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Wood Auger", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Wood Auger tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "medieval_workshop_lauter_tun", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a slatted wooden lauter tun (@medieval_workshop_lauter_tun)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MedievalFoodToolmakingMedievalFoodBuildClayBakeOven() => new()
	{
		Metadata = new("Medieval Food / Toolmaking/medieval food - build clay bake oven", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:medieval_workshop_bake_oven"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.MedievalFood.cs:138", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "medieval food - build clay bake oven",
		Category = "Medieval Food / Toolmaking",
		Blurb = "medieval food - build clay bake oven",
		Action = "medieval food - build clay bake oven",
		ActiveCraftItemSdesc = "an in-progress medieval food - build clay bake oven craft",
		Gate = CatalogueRecipeGates.MedievalFoodProductionPottery25,
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
			new(30, "$0 inspect|inspects the stock and lay|lays out the foodmaking equipment pieces.", "$0 inspect|inspects the stock and lay|lays out the foodmaking equipment pieces.", ExertionLevel.Stasis, 0),
			new(45, "$0 shape|shapes and fit|fits the pieces together with $t1.", "$0 shape|shapes and fit|fits the pieces together with $t1.", ExertionLevel.Stasis, 0),
			new(45, "$0 finish|finishes the equipment and set|sets aside $p1.", "$0 botch|botches the finishing work and spoil|spoils the assembly.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueItemRecipeInput(ItemStableReference: "medieval_industry_stock_clay_body_lump", Quantity: 1) { Compatibility = new("1x a lump of prepared clay body (@medieval_industry_stock_clay_body_lump)", []), QualityWeight = 1 },
			new CatalogueItemRecipeInput(ItemStableReference: "medieval_industry_stock_fired_brick_stack", Quantity: 1) { Compatibility = new("1x a stack of fired bricks (@medieval_industry_stock_fired_brick_stack)", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Kiln Tool", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Kiln Tool tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "medieval_workshop_bake_oven", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a clay bake oven (@medieval_workshop_bake_oven)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MedievalFoodToolmakingMedievalFoodRaiseBronzeBrewCopper() => new()
	{
		Metadata = new("Medieval Food / Toolmaking/medieval food - raise bronze brew copper", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:medieval_workshop_brew_copper"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.MedievalFood.cs:138", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "medieval food - raise bronze brew copper",
		Category = "Medieval Food / Toolmaking",
		Blurb = "medieval food - raise bronze brew copper",
		Action = "medieval food - raise bronze brew copper",
		ActiveCraftItemSdesc = "an in-progress medieval food - raise bronze brew copper craft",
		Gate = CatalogueRecipeGates.MedievalFoodProductionBlacksmithing25,
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
			new(30, "$0 inspect|inspects the stock and lay|lays out the foodmaking equipment pieces.", "$0 inspect|inspects the stock and lay|lays out the foodmaking equipment pieces.", ExertionLevel.Stasis, 0),
			new(45, "$0 shape|shapes and fit|fits the pieces together with $t1, using $t2.", "$0 shape|shapes and fit|fits the pieces together with $t1, using $t2.", ExertionLevel.Stasis, 0),
			new(45, "$0 finish|finishes the equipment and set|sets aside $p1.", "$0 botch|botches the finishing work and spoil|spoils the assembly.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueItemRecipeInput(ItemStableReference: "medieval_industry_stock_bronze_bar", Quantity: 1) { Compatibility = new("1x a bronze casting bar (@medieval_industry_stock_bronze_bar)", []), QualityWeight = 1 },
			new CatalogueItemRecipeInput(ItemStableReference: "medieval_industry_stock_rivet_packet", Quantity: 1) { Compatibility = new("1x a packet of iron rivets (@medieval_industry_stock_rivet_packet)", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Anvil", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Anvil tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "medieval_workshop_brew_copper", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a bronze brew copper (@medieval_workshop_brew_copper)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MedievalFoodToolmakingMedievalFoodCooperWoodenMashTun() => new()
	{
		Metadata = new("Medieval Food / Toolmaking/medieval food - cooper wooden mash tun", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:medieval_workshop_mash_tun"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.MedievalFood.cs:138", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "medieval food - cooper wooden mash tun",
		Category = "Medieval Food / Toolmaking",
		Blurb = "medieval food - cooper wooden mash tun",
		Action = "medieval food - cooper wooden mash tun",
		ActiveCraftItemSdesc = "an in-progress medieval food - cooper wooden mash tun craft",
		Gate = CatalogueRecipeGates.MedievalFoodProductionCoopering25,
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
			new(30, "$0 inspect|inspects the stock and lay|lays out the foodmaking equipment pieces.", "$0 inspect|inspects the stock and lay|lays out the foodmaking equipment pieces.", ExertionLevel.Stasis, 0),
			new(45, "$0 shape|shapes and fit|fits the pieces together with $t1, using $t2.", "$0 shape|shapes and fit|fits the pieces together with $t1, using $t2.", ExertionLevel.Stasis, 0),
			new(45, "$0 finish|finishes the equipment and set|sets aside $p1.", "$0 botch|botches the finishing work and spoil|spoils the assembly.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueItemRecipeInput(ItemStableReference: "medieval_industry_stock_plank_bundle", Quantity: 1) { Compatibility = new("1x a bundle of worked planks (@medieval_industry_stock_plank_bundle)", []), QualityWeight = 1 },
			new CatalogueItemRecipeInput(ItemStableReference: "medieval_industry_stock_rivet_packet", Quantity: 1) { Compatibility = new("1x a packet of iron rivets (@medieval_industry_stock_rivet_packet)", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Wood Auger", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Wood Auger tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "medieval_workshop_mash_tun", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a wooden mash tun (@medieval_workshop_mash_tun)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MedievalFoodToolmakingMedievalFoodCooperFermentingGyleTun() => new()
	{
		Metadata = new("Medieval Food / Toolmaking/medieval food - cooper fermenting gyle tun", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:medieval_workshop_fermenting_gyle_tun"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.MedievalFood.cs:138", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "medieval food - cooper fermenting gyle tun",
		Category = "Medieval Food / Toolmaking",
		Blurb = "medieval food - cooper fermenting gyle tun",
		Action = "medieval food - cooper fermenting gyle tun",
		ActiveCraftItemSdesc = "an in-progress medieval food - cooper fermenting gyle tun craft",
		Gate = CatalogueRecipeGates.MedievalFoodProductionCoopering25,
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
			new(30, "$0 inspect|inspects the stock and lay|lays out the foodmaking equipment pieces.", "$0 inspect|inspects the stock and lay|lays out the foodmaking equipment pieces.", ExertionLevel.Stasis, 0),
			new(45, "$0 shape|shapes and fit|fits the pieces together with $t1, using $t2.", "$0 shape|shapes and fit|fits the pieces together with $t1, using $t2.", ExertionLevel.Stasis, 0),
			new(45, "$0 finish|finishes the equipment and set|sets aside $p1.", "$0 botch|botches the finishing work and spoil|spoils the assembly.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueItemRecipeInput(ItemStableReference: "medieval_industry_stock_plank_bundle", Quantity: 1) { Compatibility = new("1x a bundle of worked planks (@medieval_industry_stock_plank_bundle)", []), QualityWeight = 1 },
			new CatalogueItemRecipeInput(ItemStableReference: "medieval_industry_stock_rivet_packet", Quantity: 1) { Compatibility = new("1x a packet of iron rivets (@medieval_industry_stock_rivet_packet)", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Wood Auger", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Wood Auger tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "medieval_workshop_fermenting_gyle_tun", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a fermenting gyle tun (@medieval_workshop_fermenting_gyle_tun)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MedievalFoodToolmakingMedievalFoodFrameFineFlourSieve() => new()
	{
		Metadata = new("Medieval Food / Toolmaking/medieval food - frame fine flour sieve", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:medieval_tool_flour_sieve"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.MedievalFood.cs:138", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "medieval food - frame fine flour sieve",
		Category = "Medieval Food / Toolmaking",
		Blurb = "medieval food - frame fine flour sieve",
		Action = "medieval food - frame fine flour sieve",
		ActiveCraftItemSdesc = "an in-progress medieval food - frame fine flour sieve craft",
		Gate = CatalogueRecipeGates.MedievalFoodProductionCarpentry20,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 3,
		Interruptable = false,
		Phases = [
			new(30, "$0 inspect|inspects the stock and lay|lays out the foodmaking equipment pieces.", "$0 inspect|inspects the stock and lay|lays out the foodmaking equipment pieces.", ExertionLevel.Stasis, 0),
			new(45, "$0 shape|shapes and fit|fits the pieces together with $t1.", "$0 shape|shapes and fit|fits the pieces together with $t1.", ExertionLevel.Stasis, 0),
			new(45, "$0 finish|finishes the equipment and set|sets aside $p1.", "$0 botch|botches the finishing work and spoil|spoils the assembly.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueItemRecipeInput(ItemStableReference: "medieval_industry_stock_handle_blanks", Quantity: 1) { Compatibility = new("1x a bundle of handle blanks (@medieval_industry_stock_handle_blanks)", []), QualityWeight = 1 },
			new CatalogueItemRecipeInput(ItemStableReference: "medieval_industry_stock_plain_cloth_bolt", Quantity: 1) { Compatibility = new("1x a bolt of plain wool cloth (@medieval_industry_stock_plain_cloth_bolt)", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Awl Punch", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Awl Punch tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "medieval_tool_flour_sieve", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a framed flour sieve (@medieval_tool_flour_sieve)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MedievalFoodToolmakingMedievalFoodHollowWoodenKneadingTrough() => new()
	{
		Metadata = new("Medieval Food / Toolmaking/medieval food - hollow wooden kneading trough", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:medieval_tool_kneading_trough"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.MedievalFood.cs:138", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "medieval food - hollow wooden kneading trough",
		Category = "Medieval Food / Toolmaking",
		Blurb = "medieval food - hollow wooden kneading trough",
		Action = "medieval food - hollow wooden kneading trough",
		ActiveCraftItemSdesc = "an in-progress medieval food - hollow wooden kneading trough craft",
		Gate = CatalogueRecipeGates.MedievalFoodProductionCarpentry20,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 3,
		Interruptable = false,
		Phases = [
			new(30, "$0 inspect|inspects the stock and lay|lays out the foodmaking equipment pieces.", "$0 inspect|inspects the stock and lay|lays out the foodmaking equipment pieces.", ExertionLevel.Stasis, 0),
			new(45, "$0 shape|shapes and fit|fits the pieces together with $t1, using $t2.", "$0 shape|shapes and fit|fits the pieces together with $t1, using $t2.", ExertionLevel.Stasis, 0),
			new(45, "$0 finish|finishes the equipment and set|sets aside $p1.", "$0 botch|botches the finishing work and spoil|spoils the assembly.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueItemRecipeInput(ItemStableReference: "medieval_industry_stock_plank_bundle", Quantity: 1) { Compatibility = new("1x a bundle of worked planks (@medieval_industry_stock_plank_bundle)", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Wood Chisel", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Wood Chisel tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Drawknife", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Drawknife tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "medieval_tool_kneading_trough", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a wooden kneading trough (@medieval_tool_kneading_trough)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MedievalFoodToolmakingMedievalFoodBuildOakSaltingTrough() => new()
	{
		Metadata = new("Medieval Food / Toolmaking/medieval food - build oak salting trough", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:medieval_tool_salting_trough"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.MedievalFood.cs:138", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "medieval food - build oak salting trough",
		Category = "Medieval Food / Toolmaking",
		Blurb = "medieval food - build oak salting trough",
		Action = "medieval food - build oak salting trough",
		ActiveCraftItemSdesc = "an in-progress medieval food - build oak salting trough craft",
		Gate = CatalogueRecipeGates.MedievalFoodProductionCarpentry20,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 3,
		Interruptable = false,
		Phases = [
			new(30, "$0 inspect|inspects the stock and lay|lays out the foodmaking equipment pieces.", "$0 inspect|inspects the stock and lay|lays out the foodmaking equipment pieces.", ExertionLevel.Stasis, 0),
			new(45, "$0 shape|shapes and fit|fits the pieces together with $t1, using $t2.", "$0 shape|shapes and fit|fits the pieces together with $t1, using $t2.", ExertionLevel.Stasis, 0),
			new(45, "$0 finish|finishes the equipment and set|sets aside $p1.", "$0 botch|botches the finishing work and spoil|spoils the assembly.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueItemRecipeInput(ItemStableReference: "medieval_industry_stock_plank_bundle", Quantity: 1) { Compatibility = new("1x a bundle of worked planks (@medieval_industry_stock_plank_bundle)", []), QualityWeight = 1 },
			new CatalogueItemRecipeInput(ItemStableReference: "medieval_industry_stock_rivet_packet", Quantity: 1) { Compatibility = new("1x a packet of iron rivets (@medieval_industry_stock_rivet_packet)", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Wood Chisel", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Wood Chisel tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "medieval_tool_salting_trough", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x an oak salting trough (@medieval_tool_salting_trough)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MedievalFoodToolmakingMedievalFoodBuildSmokingAndDryingRack() => new()
	{
		Metadata = new("Medieval Food / Toolmaking/medieval food - build smoking and drying rack", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:medieval_tool_smoking_rack"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.MedievalFood.cs:138", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "medieval food - build smoking and drying rack",
		Category = "Medieval Food / Toolmaking",
		Blurb = "medieval food - build smoking and drying rack",
		Action = "medieval food - build smoking and drying rack",
		ActiveCraftItemSdesc = "an in-progress medieval food - build smoking and drying rack craft",
		Gate = CatalogueRecipeGates.MedievalFoodProductionCarpentry20,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 3,
		Interruptable = false,
		Phases = [
			new(30, "$0 inspect|inspects the stock and lay|lays out the foodmaking equipment pieces.", "$0 inspect|inspects the stock and lay|lays out the foodmaking equipment pieces.", ExertionLevel.Stasis, 0),
			new(45, "$0 shape|shapes and fit|fits the pieces together with $t1, using $t2.", "$0 shape|shapes and fit|fits the pieces together with $t1, using $t2.", ExertionLevel.Stasis, 0),
			new(45, "$0 finish|finishes the equipment and set|sets aside $p1.", "$0 botch|botches the finishing work and spoil|spoils the assembly.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueItemRecipeInput(ItemStableReference: "medieval_industry_stock_plank_bundle", Quantity: 1) { Compatibility = new("1x a bundle of worked planks (@medieval_industry_stock_plank_bundle)", []), QualityWeight = 1 },
			new CatalogueItemRecipeInput(ItemStableReference: "medieval_industry_stock_iron_bar", Quantity: 1) { Compatibility = new("1x a wrought-iron bar (@medieval_industry_stock_iron_bar)", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hand Saw", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hand Saw tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "medieval_tool_smoking_rack", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a blackened smoking rack (@medieval_tool_smoking_rack)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MedievalFoodToolmakingMedievalFoodBuildWoodenOilPress() => new()
	{
		Metadata = new("Medieval Food / Toolmaking/medieval food - build wooden oil press", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:medieval_tool_oil_press"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.MedievalFood.cs:138", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "medieval food - build wooden oil press",
		Category = "Medieval Food / Toolmaking",
		Blurb = "medieval food - build wooden oil press",
		Action = "medieval food - build wooden oil press",
		ActiveCraftItemSdesc = "an in-progress medieval food - build wooden oil press craft",
		Gate = CatalogueRecipeGates.MedievalFoodProductionCarpentry25,
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
			new(30, "$0 inspect|inspects the stock and lay|lays out the foodmaking equipment pieces.", "$0 inspect|inspects the stock and lay|lays out the foodmaking equipment pieces.", ExertionLevel.Stasis, 0),
			new(45, "$0 shape|shapes and fit|fits the pieces together with $t1, using $t2 and $t3.", "$0 shape|shapes and fit|fits the pieces together with $t1, using $t2 and $t3.", ExertionLevel.Stasis, 0),
			new(45, "$0 finish|finishes the equipment and set|sets aside $p1.", "$0 botch|botches the finishing work and spoil|spoils the assembly.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueItemRecipeInput(ItemStableReference: "medieval_industry_stock_plank_bundle", Quantity: 1) { Compatibility = new("1x a bundle of worked planks (@medieval_industry_stock_plank_bundle)", []), QualityWeight = 1 },
			new CatalogueItemRecipeInput(ItemStableReference: "medieval_industry_stock_iron_bar", Quantity: 1) { Compatibility = new("1x a wrought-iron bar (@medieval_industry_stock_iron_bar)", []), QualityWeight = 1 },
			new CatalogueItemRecipeInput(ItemStableReference: "medieval_industry_stock_rivet_packet", Quantity: 1) { Compatibility = new("1x a packet of iron rivets (@medieval_industry_stock_rivet_packet)", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Wood Chisel", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Wood Chisel tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Wood Auger", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Wood Auger tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "medieval_tool_oil_press", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a wooden oil press (@medieval_tool_oil_press)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MedievalFoodToolmakingMedievalFoodBuildWoodenFruitPress() => new()
	{
		Metadata = new("Medieval Food / Toolmaking/medieval food - build wooden fruit press", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:medieval_tool_fruit_press"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.MedievalFood.cs:138", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "medieval food - build wooden fruit press",
		Category = "Medieval Food / Toolmaking",
		Blurb = "medieval food - build wooden fruit press",
		Action = "medieval food - build wooden fruit press",
		ActiveCraftItemSdesc = "an in-progress medieval food - build wooden fruit press craft",
		Gate = CatalogueRecipeGates.MedievalFoodProductionCarpentry25,
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
			new(30, "$0 inspect|inspects the stock and lay|lays out the foodmaking equipment pieces.", "$0 inspect|inspects the stock and lay|lays out the foodmaking equipment pieces.", ExertionLevel.Stasis, 0),
			new(45, "$0 shape|shapes and fit|fits the pieces together with $t1, using $t2 and $t3.", "$0 shape|shapes and fit|fits the pieces together with $t1, using $t2 and $t3.", ExertionLevel.Stasis, 0),
			new(45, "$0 finish|finishes the equipment and set|sets aside $p1.", "$0 botch|botches the finishing work and spoil|spoils the assembly.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueItemRecipeInput(ItemStableReference: "medieval_industry_stock_plank_bundle", Quantity: 1) { Compatibility = new("1x a bundle of worked planks (@medieval_industry_stock_plank_bundle)", []), QualityWeight = 1 },
			new CatalogueItemRecipeInput(ItemStableReference: "medieval_industry_stock_iron_bar", Quantity: 1) { Compatibility = new("1x a wrought-iron bar (@medieval_industry_stock_iron_bar)", []), QualityWeight = 1 },
			new CatalogueItemRecipeInput(ItemStableReference: "medieval_industry_stock_rivet_packet", Quantity: 1) { Compatibility = new("1x a packet of iron rivets (@medieval_industry_stock_rivet_packet)", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Wood Chisel", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Wood Chisel tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Wood Auger", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Wood Auger tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "medieval_tool_fruit_press", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a wooden fruit press (@medieval_tool_fruit_press)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MedievalFoodToolmakingMedievalFoodShapeBroadMashingPaddle() => new()
	{
		Metadata = new("Medieval Food / Toolmaking/medieval food - shape broad mashing paddle", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:medieval_tool_mashing_paddle"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.MedievalFood.cs:138", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "medieval food - shape broad mashing paddle",
		Category = "Medieval Food / Toolmaking",
		Blurb = "medieval food - shape broad mashing paddle",
		Action = "medieval food - shape broad mashing paddle",
		ActiveCraftItemSdesc = "an in-progress medieval food - shape broad mashing paddle craft",
		Gate = CatalogueRecipeGates.MedievalFoodProductionCarpentry20,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 3,
		Interruptable = false,
		Phases = [
			new(30, "$0 inspect|inspects the stock and lay|lays out the foodmaking equipment pieces.", "$0 inspect|inspects the stock and lay|lays out the foodmaking equipment pieces.", ExertionLevel.Stasis, 0),
			new(45, "$0 shape|shapes and fit|fits the pieces together with $t1, using $t2.", "$0 shape|shapes and fit|fits the pieces together with $t1, using $t2.", ExertionLevel.Stasis, 0),
			new(45, "$0 finish|finishes the equipment and set|sets aside $p1.", "$0 botch|botches the finishing work and spoil|spoils the assembly.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueItemRecipeInput(ItemStableReference: "medieval_industry_stock_handle_blanks", Quantity: 1) { Compatibility = new("1x a bundle of handle blanks (@medieval_industry_stock_handle_blanks)", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Drawknife", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Drawknife tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Wood Auger", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Wood Auger tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "medieval_tool_mashing_paddle", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a broad mashing paddle (@medieval_tool_mashing_paddle)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MedievalFoodProcessingMedievalFoodThreshGrainIntoCleaningStock() => new()
	{
		Metadata = new("Medieval Food / Processing/medieval food - thresh grain into cleaning stock", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.MedievalFood.cs:138", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "medieval food - thresh grain into cleaning stock",
		Category = "Medieval Food / Processing",
		Blurb = "medieval food - thresh grain into cleaning stock",
		Action = "medieval food - thresh grain into cleaning stock",
		ActiveCraftItemSdesc = "an in-progress medieval food - thresh grain cleaning stock craft",
		Gate = CatalogueRecipeGates.MedievalFoodProductionThreshing10,
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
			new(30, "$0 inspect|inspects $i1 and prepare|prepares the work with $t1.", "$0 inspect|inspects $i1 and prepare|prepares the work with $t1.", ExertionLevel.Stasis, 0),
			new(45, "$0 work|works the ingredients steadily through the main preparation.", "$0 work|works the ingredients steadily through the main preparation.", ExertionLevel.Stasis, 0),
			new(45, "$0 finish|finishes the preparation and set|sets aside $p1.", "$0 spoil|spoils the final preparation and have|has nothing useful to show for it.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityTagRecipeInput(Grams: 2000.0, MaterialTag: "Grain Crop", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2 kilograms of a material tagged as Grain Crop", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Threshing Flail", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Threshing Flail tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueCommodityRecipeProduct(Grams: 1700.0, Material: "wheat", Tag: "Grain Cleaning Stock", Characteristics: []) { Compatibility = new("1 kilogram 700 grams of wheat commodity", ["tag Grain Cleaning Stock"]), IsFailProduct = false, MaterialDefiningInputIndex = null },
			new CatalogueCommodityRecipeProduct(Grams: 250.0, Material: "wheat", Tag: "Bran Commodity", Characteristics: []) { Compatibility = new("250 grams of wheat commodity", ["tag Bran Commodity"]), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MedievalFoodProcessingMedievalFoodWinnowThreshedGrain() => new()
	{
		Metadata = new("Medieval Food / Processing/medieval food - winnow threshed grain", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.MedievalFood.cs:138", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "medieval food - winnow threshed grain",
		Category = "Medieval Food / Processing",
		Blurb = "medieval food - winnow threshed grain",
		Action = "medieval food - winnow threshed grain",
		ActiveCraftItemSdesc = "an in-progress medieval food - winnow threshed grain craft",
		Gate = CatalogueRecipeGates.MedievalFoodProductionThreshing10,
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
			new(30, "$0 inspect|inspects $i1 and prepare|prepares the work with $t1.", "$0 inspect|inspects $i1 and prepare|prepares the work with $t1.", ExertionLevel.Stasis, 0),
			new(45, "$0 work|works the ingredients steadily through the main preparation.", "$0 work|works the ingredients steadily through the main preparation.", ExertionLevel.Stasis, 0),
			new(45, "$0 finish|finishes the preparation and set|sets aside $p1.", "$0 spoil|spoils the final preparation and have|has nothing useful to show for it.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityTagRecipeInput(Grams: 1700.0, MaterialTag: "Grain Crop", PileTag: "Grain Cleaning Stock", CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1 kilogram 700 grams of a material tagged as Grain Crop", ["piletag Grain Cleaning Stock"]), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Winnowing Basket", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Winnowing Basket tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueCommodityRecipeProduct(Grams: 1500.0, Material: "wheat", Tag: "Cleaned Grain Commodity", Characteristics: []) { Compatibility = new("1 kilogram 500 grams of wheat commodity", ["tag Cleaned Grain Commodity"]), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MedievalFoodProcessingMedievalFoodMillCleanedGrainIntoFlour() => new()
	{
		Metadata = new("Medieval Food / Processing/medieval food - mill cleaned grain into flour", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.MedievalFood.cs:138", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "medieval food - mill cleaned grain into flour",
		Category = "Medieval Food / Processing",
		Blurb = "medieval food - mill cleaned grain into flour",
		Action = "medieval food - mill cleaned grain into flour",
		ActiveCraftItemSdesc = "an in-progress medieval food - mill cleaned grain flour craft",
		Gate = CatalogueRecipeGates.MedievalFoodProductionMilling15,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 3,
		Interruptable = false,
		Phases = [
			new(30, "$0 inspect|inspects $i1 and prepare|prepares the work with $t1.", "$0 inspect|inspects $i1 and prepare|prepares the work with $t1.", ExertionLevel.Stasis, 0),
			new(45, "$0 work|works the ingredients steadily through the main preparation, using $t2.", "$0 work|works the ingredients steadily through the main preparation, using $t2.", ExertionLevel.Stasis, 0),
			new(45, "$0 finish|finishes the preparation and set|sets aside $p1.", "$0 spoil|spoils the final preparation and have|has nothing useful to show for it.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityTagRecipeInput(Grams: 1350.0, MaterialTag: "Grain Crop", PileTag: "Cleaned Grain Commodity", CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1 kilogram 350 grams of a material tagged as Grain Crop", ["piletag Cleaned Grain Commodity"]), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hand Quern", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Hand Quern tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Grain Sieve", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Grain Sieve tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueCommodityRecipeProduct(Grams: 1100.0, Material: "wheat", Tag: "Flour Commodity", Characteristics: []) { Compatibility = new("1 kilogram 100 grams of wheat commodity", ["tag Flour Commodity"]), IsFailProduct = false, MaterialDefiningInputIndex = null },
			new CatalogueCommodityRecipeProduct(Grams: 150.0, Material: "wheat", Tag: "Bran Commodity", Characteristics: []) { Compatibility = new("150 grams of wheat commodity", ["tag Bran Commodity"]), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MedievalFoodProcessingMedievalFoodGrindCleanedGrainIntoMeal() => new()
	{
		Metadata = new("Medieval Food / Processing/medieval food - grind cleaned grain into meal", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.MedievalFood.cs:138", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "medieval food - grind cleaned grain into meal",
		Category = "Medieval Food / Processing",
		Blurb = "medieval food - grind cleaned grain into meal",
		Action = "medieval food - grind cleaned grain into meal",
		ActiveCraftItemSdesc = "an in-progress medieval food - grind cleaned grain meal craft",
		Gate = CatalogueRecipeGates.MedievalFoodProductionMilling10,
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
			new(30, "$0 inspect|inspects $i1 and prepare|prepares the work with $t1.", "$0 inspect|inspects $i1 and prepare|prepares the work with $t1.", ExertionLevel.Stasis, 0),
			new(45, "$0 work|works the ingredients steadily through the main preparation.", "$0 work|works the ingredients steadily through the main preparation.", ExertionLevel.Stasis, 0),
			new(45, "$0 finish|finishes the preparation and set|sets aside $p1.", "$0 spoil|spoils the final preparation and have|has nothing useful to show for it.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityTagRecipeInput(Grams: 1000.0, MaterialTag: "Grain Crop", PileTag: "Cleaned Grain Commodity", CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1 kilogram of a material tagged as Grain Crop", ["piletag Cleaned Grain Commodity"]), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hand Quern", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Hand Quern tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueCommodityRecipeProduct(Grams: 900.0, Material: "wheat", Tag: "Meal Commodity", Characteristics: []) { Compatibility = new("900 grams of wheat commodity", ["tag Meal Commodity"]), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MedievalFoodProcessingMedievalFoodMaltCleanedGrain() => new()
	{
		Metadata = new("Medieval Food / Processing/medieval food - malt cleaned grain", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.MedievalFood.cs:138", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "medieval food - malt cleaned grain",
		Category = "Medieval Food / Processing",
		Blurb = "medieval food - malt cleaned grain",
		Action = "medieval food - malt cleaned grain",
		ActiveCraftItemSdesc = "an in-progress medieval food - malt cleaned grain craft",
		Gate = CatalogueRecipeGates.MedievalFoodProductionBrewing15,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 3,
		Interruptable = false,
		Phases = [
			new(30, "$0 inspect|inspects $i1 and prepare|prepares the work with $t1.", "$0 inspect|inspects $i1 and prepare|prepares the work with $t1.", ExertionLevel.Stasis, 0),
			new(45, "$0 work|works the ingredients steadily through the main preparation.", "$0 work|works the ingredients steadily through the main preparation.", ExertionLevel.Stasis, 0),
			new(45, "$0 finish|finishes the preparation and set|sets aside $p1.", "$0 spoil|spoils the final preparation and have|has nothing useful to show for it.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityTagRecipeInput(Grams: 1200.0, MaterialTag: "Grain Crop", PileTag: "Cleaned Grain Commodity", CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1 kilogram 200 grams of a material tagged as Grain Crop", ["piletag Cleaned Grain Commodity"]), QualityWeight = 1 },
			new CatalogueLiquidRecipeInput(Liquid: "Water", Litres: 1.0) { Compatibility = new("1 litre of Water", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Drying Rack", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Drying Rack tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueCommodityRecipeProduct(Grams: 1050.0, Material: "wheat", Tag: "Malted Grain Commodity", Characteristics: []) { Compatibility = new("1 kilogram 50 grams of wheat commodity", ["tag Malted Grain Commodity"]), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MedievalFoodProcessingMedievalFoodMixBreadDough() => new()
	{
		Metadata = new("Medieval Food / Processing/medieval food - mix bread dough", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.MedievalFood.cs:138", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "medieval food - mix bread dough",
		Category = "Medieval Food / Processing",
		Blurb = "medieval food - mix bread dough",
		Action = "medieval food - mix bread dough",
		ActiveCraftItemSdesc = "an in-progress medieval food - mix bread dough craft",
		Gate = CatalogueRecipeGates.MedievalFoodProductionCooking10,
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
			new(30, "$0 inspect|inspects $i1 and prepare|prepares the work with $t1.", "$0 inspect|inspects $i1 and prepare|prepares the work with $t1.", ExertionLevel.Stasis, 0),
			new(45, "$0 work|works the ingredients steadily through the main preparation.", "$0 work|works the ingredients steadily through the main preparation.", ExertionLevel.Stasis, 0),
			new(45, "$0 finish|finishes the preparation and set|sets aside $p1.", "$0 spoil|spoils the final preparation and have|has nothing useful to show for it.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityTagRecipeInput(Grams: 800.0, MaterialTag: "Grain Crop", PileTag: "Flour Commodity", CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("800 grams of a material tagged as Grain Crop", ["piletag Flour Commodity"]), QualityWeight = 1 },
			new CatalogueLiquidRecipeInput(Liquid: "Water", Litres: 0.6) { Compatibility = new("600 millilitres of Water", []), QualityWeight = 1 },
			new CatalogueCommodityRecipeInput(Grams: 15.0, Material: "salt", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("15 grams of salt", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Kneading Trough", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Kneading Trough tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueCommodityRecipeProduct(Grams: 1300.0, Material: "wheat", Tag: "Dough Commodity", Characteristics: []) { Compatibility = new("1 kilogram 300 grams of wheat commodity", ["tag Dough Commodity"]), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MedievalFoodProcessingMedievalFoodCrushOilseedIntoMash() => new()
	{
		Metadata = new("Medieval Food / Processing/medieval food - crush oilseed into mash", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.MedievalFood.cs:138", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "medieval food - crush oilseed into mash",
		Category = "Medieval Food / Processing",
		Blurb = "medieval food - crush oilseed into mash",
		Action = "medieval food - crush oilseed into mash",
		ActiveCraftItemSdesc = "an in-progress medieval food - crush oilseed mash craft",
		Gate = CatalogueRecipeGates.MedievalFoodProductionMilling102,
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
			new(30, "$0 inspect|inspects $i1 and prepare|prepares the work with $t1.", "$0 inspect|inspects $i1 and prepare|prepares the work with $t1.", ExertionLevel.Stasis, 0),
			new(45, "$0 work|works the ingredients steadily through the main preparation.", "$0 work|works the ingredients steadily through the main preparation.", ExertionLevel.Stasis, 0),
			new(45, "$0 finish|finishes the preparation and set|sets aside $p1.", "$0 spoil|spoils the final preparation and have|has nothing useful to show for it.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityTagRecipeInput(Grams: 2000.0, MaterialTag: "Oilseed Crop", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2 kilograms of a material tagged as Oilseed Crop", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Mortar and Pestle", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Mortar and Pestle tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueCommodityRecipeProduct(Grams: 1800.0, Material: "olive crop", Tag: "Oilseed Mash Commodity", Characteristics: []) { Compatibility = new("1 kilogram 800 grams of olive crop commodity", ["tag Oilseed Mash Commodity"]), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MedievalFoodProcessingMedievalFoodPressCookingOil() => new()
	{
		Metadata = new("Medieval Food / Processing/medieval food - press cooking oil", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:medieval_tableware_oil_amphora"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.MedievalFood.cs:138", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "medieval food - press cooking oil",
		Category = "Medieval Food / Processing",
		Blurb = "medieval food - press cooking oil",
		Action = "medieval food - press cooking oil",
		ActiveCraftItemSdesc = "an in-progress medieval food - press cooking oil craft",
		Gate = CatalogueRecipeGates.MedievalFoodProductionMilling152,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 3,
		Interruptable = false,
		Phases = [
			new(30, "$0 inspect|inspects $i1 and prepare|prepares the work with $t1.", "$0 inspect|inspects $i1 and prepare|prepares the work with $t1.", ExertionLevel.Stasis, 0),
			new(45, "$0 work|works the ingredients steadily through the main preparation.", "$0 work|works the ingredients steadily through the main preparation.", ExertionLevel.Stasis, 0),
			new(45, "$0 finish|finishes the preparation and set|sets aside $p1.", "$0 spoil|spoils the final preparation and have|has nothing useful to show for it.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityTagRecipeInput(Grams: 1800.0, MaterialTag: "Oilseed Crop", PileTag: "Oilseed Mash Commodity", CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1 kilogram 800 grams of a material tagged as Oilseed Crop", ["piletag Oilseed Mash Commodity"]), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Oil Press", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Oil Press tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueLiquidRecipeProduct(ItemStableReference: "medieval_tableware_oil_amphora", Quantity: 1, SkinStableReference: null, Liquid: "vegetable oil", Litres: 1.0) { Compatibility = new("1x an earthenware oil amphora (@medieval_tableware_oil_amphora) filled with 1 litre of vegetable oil", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
			new CatalogueCommodityRecipeProduct(Grams: 800.0, Material: "olive crop", Tag: "Oilseed Cake Commodity", Characteristics: []) { Compatibility = new("800 grams of olive crop commodity", ["tag Oilseed Cake Commodity"]), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MedievalFoodProcessingMedievalFoodPressFruitMust() => new()
	{
		Metadata = new("Medieval Food / Processing/medieval food - press fruit must", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.MedievalFood.cs:138", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "medieval food - press fruit must",
		Category = "Medieval Food / Processing",
		Blurb = "medieval food - press fruit must",
		Action = "medieval food - press fruit must",
		ActiveCraftItemSdesc = "an in-progress medieval food - press fruit must craft",
		Gate = CatalogueRecipeGates.MedievalFoodProductionBrewing152,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 3,
		Interruptable = false,
		Phases = [
			new(30, "$0 inspect|inspects $i1 and prepare|prepares the work with $t1.", "$0 inspect|inspects $i1 and prepare|prepares the work with $t1.", ExertionLevel.Stasis, 0),
			new(45, "$0 work|works the ingredients steadily through the main preparation.", "$0 work|works the ingredients steadily through the main preparation.", ExertionLevel.Stasis, 0),
			new(45, "$0 finish|finishes the preparation and set|sets aside $p1.", "$0 spoil|spoils the final preparation and have|has nothing useful to show for it.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityTagRecipeInput(Grams: 2000.0, MaterialTag: "Fruit Must Crop", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2 kilograms of a material tagged as Fruit Must Crop", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Fruit Press", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Fruit Press tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueCommodityRecipeProduct(Grams: 1500.0, Material: "grape", Tag: "Fruit Must Commodity", Characteristics: []) { Compatibility = new("1 kilogram 500 grams of grape commodity", ["tag Fruit Must Commodity"]), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MedievalFoodProcessingMedievalFoodMashAndLauterGrainWort() => new()
	{
		Metadata = new("Medieval Food / Processing/medieval food - mash and lauter grain wort", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.MedievalFood.cs:138", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "medieval food - mash and lauter grain wort",
		Category = "Medieval Food / Processing",
		Blurb = "medieval food - mash and lauter grain wort",
		Action = "medieval food - mash and lauter grain wort",
		ActiveCraftItemSdesc = "an in-progress medieval food - mash and lauter grain wort craft",
		Gate = CatalogueRecipeGates.MedievalFoodProductionBrewing20,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 3,
		Interruptable = false,
		Phases = [
			new(30, "$0 inspect|inspects $i1 and prepare|prepares the work with $t1.", "$0 inspect|inspects $i1 and prepare|prepares the work with $t1.", ExertionLevel.Stasis, 0),
			new(45, "$0 work|works the ingredients steadily through the main preparation, using $t2, $t3 and $t4.", "$0 work|works the ingredients steadily through the main preparation, using $t2, $t3 and $t4.", ExertionLevel.Stasis, 0),
			new(45, "$0 finish|finishes the preparation and set|sets aside $p1.", "$0 spoil|spoils the final preparation and have|has nothing useful to show for it.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityTagRecipeInput(Grams: 1000.0, MaterialTag: "Grain Crop", PileTag: "Malted Grain Commodity", CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1 kilogram of a material tagged as Grain Crop", ["piletag Malted Grain Commodity"]), QualityWeight = 1 },
			new CatalogueLiquidRecipeInput(Liquid: "Water", Litres: 3.0) { Compatibility = new("3 litres of Water", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Mash Tun", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Mash Tun tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Brew Copper", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Brew Copper tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Mashing Paddle", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Mashing Paddle tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Lauter Tun", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Lauter Tun tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueCommodityRecipeProduct(Grams: 3500.0, Material: "wheat", Tag: "Wort Commodity", Characteristics: []) { Compatibility = new("3 kilograms 500 grams of wheat commodity", ["tag Wort Commodity"]), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MedievalFoodProcessingMedievalFoodBreakDownRawMeatCuts() => new()
	{
		Metadata = new("Medieval Food / Processing/medieval food - break down raw meat cuts", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.MedievalFood.cs:138", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "medieval food - break down raw meat cuts",
		Category = "Medieval Food / Processing",
		Blurb = "medieval food - break down raw meat cuts",
		Action = "medieval food - break down raw meat cuts",
		ActiveCraftItemSdesc = "an in-progress medieval food - break down raw meat cuts craft",
		Gate = CatalogueRecipeGates.MedievalFoodProductionButchering10,
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
			new(30, "$0 inspect|inspects $i1 and prepare|prepares the work with $t1.", "$0 inspect|inspects $i1 and prepare|prepares the work with $t1.", ExertionLevel.Stasis, 0),
			new(45, "$0 work|works the ingredients steadily through the main preparation.", "$0 work|works the ingredients steadily through the main preparation.", ExertionLevel.Stasis, 0),
			new(45, "$0 finish|finishes the preparation and set|sets aside $p1.", "$0 spoil|spoils the final preparation and have|has nothing useful to show for it.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueTagRecipeInput(Tag: "Raw Non-Fish Meat Cut", Quantity: 1) { Compatibility = new("1x an item with the Raw Non-Fish Meat Cut tag", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Butcher's Knife", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Butcher's Knife tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueCommodityRecipeProduct(Grams: 2000.0, Material: "meat", Tag: "Raw Meat Commodity", Characteristics: []) { Compatibility = new("2 kilograms of meat commodity", ["tag Raw Meat Commodity"]), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MedievalFoodProcessingMedievalFoodBreakDownRawFishCuts() => new()
	{
		Metadata = new("Medieval Food / Processing/medieval food - break down raw fish cuts", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.MedievalFood.cs:138", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "medieval food - break down raw fish cuts",
		Category = "Medieval Food / Processing",
		Blurb = "medieval food - break down raw fish cuts",
		Action = "medieval food - break down raw fish cuts",
		ActiveCraftItemSdesc = "an in-progress medieval food - break down raw fish cuts craft",
		Gate = CatalogueRecipeGates.MedievalFoodProductionButchering102,
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
			new(30, "$0 inspect|inspects $i1 and prepare|prepares the work with $t1.", "$0 inspect|inspects $i1 and prepare|prepares the work with $t1.", ExertionLevel.Stasis, 0),
			new(45, "$0 work|works the ingredients steadily through the main preparation.", "$0 work|works the ingredients steadily through the main preparation.", ExertionLevel.Stasis, 0),
			new(45, "$0 finish|finishes the preparation and set|sets aside $p1.", "$0 spoil|spoils the final preparation and have|has nothing useful to show for it.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueTagRecipeInput(Tag: "Raw Fish Cut", Quantity: 1) { Compatibility = new("1x an item with the Raw Fish Cut tag", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Butcher's Knife", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Butcher's Knife tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueCommodityRecipeProduct(Grams: 1000.0, Material: "fish", Tag: "Raw Fish Commodity", Characteristics: []) { Compatibility = new("1 kilogram of fish commodity", ["tag Raw Fish Commodity"]), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MedievalFoodProcessingMedievalFoodSaltRawMeat() => new()
	{
		Metadata = new("Medieval Food / Processing/medieval food - salt raw meat", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.MedievalFood.cs:138", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "medieval food - salt raw meat",
		Category = "Medieval Food / Processing",
		Blurb = "medieval food - salt raw meat",
		Action = "medieval food - salt raw meat",
		ActiveCraftItemSdesc = "an in-progress medieval food - salt raw meat craft",
		Gate = CatalogueRecipeGates.MedievalFoodProductionCooking15,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 3,
		Interruptable = false,
		Phases = [
			new(30, "$0 inspect|inspects $i1 and prepare|prepares the work with $t1.", "$0 inspect|inspects $i1 and prepare|prepares the work with $t1.", ExertionLevel.Stasis, 0),
			new(45, "$0 work|works the ingredients steadily through the main preparation.", "$0 work|works the ingredients steadily through the main preparation.", ExertionLevel.Stasis, 0),
			new(45, "$0 finish|finishes the preparation and set|sets aside $p1.", "$0 spoil|spoils the final preparation and have|has nothing useful to show for it.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1000.0, Material: "meat", PileTag: "Raw Meat Commodity", CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1 kilogram of meat", ["piletag Raw Meat Commodity"]), QualityWeight = 1 },
			new CatalogueCommodityRecipeInput(Grams: 200.0, Material: "salt", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("200 grams of salt", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Salting Trough", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Salting Trough tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueCommodityRecipeProduct(Grams: 1000.0, Material: "meat", Tag: "Salted Meat Commodity", Characteristics: []) { Compatibility = new("1 kilogram of meat commodity", ["tag Salted Meat Commodity"]), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MedievalFoodProcessingMedievalFoodDrySaltedMeat() => new()
	{
		Metadata = new("Medieval Food / Processing/medieval food - dry salted meat", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.MedievalFood.cs:138", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "medieval food - dry salted meat",
		Category = "Medieval Food / Processing",
		Blurb = "medieval food - dry salted meat",
		Action = "medieval food - dry salted meat",
		ActiveCraftItemSdesc = "an in-progress medieval food - dry salted meat craft",
		Gate = CatalogueRecipeGates.MedievalFoodProductionCooking15,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 3,
		Interruptable = false,
		Phases = [
			new(30, "$0 inspect|inspects $i1 and prepare|prepares the work with $t1.", "$0 inspect|inspects $i1 and prepare|prepares the work with $t1.", ExertionLevel.Stasis, 0),
			new(45, "$0 work|works the ingredients steadily through the main preparation.", "$0 work|works the ingredients steadily through the main preparation.", ExertionLevel.Stasis, 0),
			new(45, "$0 finish|finishes the preparation and set|sets aside $p1.", "$0 spoil|spoils the final preparation and have|has nothing useful to show for it.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1000.0, Material: "meat", PileTag: "Salted Meat Commodity", CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1 kilogram of meat", ["piletag Salted Meat Commodity"]), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Drying Rack", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Drying Rack tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueCommodityRecipeProduct(Grams: 700.0, Material: "meat", Tag: "Dried Meat Commodity", Characteristics: []) { Compatibility = new("700 grams of meat commodity", ["tag Dried Meat Commodity"]), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MedievalFoodProcessingMedievalFoodSmokeSaltedMeat() => new()
	{
		Metadata = new("Medieval Food / Processing/medieval food - smoke salted meat", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.MedievalFood.cs:138", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "medieval food - smoke salted meat",
		Category = "Medieval Food / Processing",
		Blurb = "medieval food - smoke salted meat",
		Action = "medieval food - smoke salted meat",
		ActiveCraftItemSdesc = "an in-progress medieval food - smoke salted meat craft",
		Gate = CatalogueRecipeGates.MedievalFoodProductionCooking15,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 3,
		Interruptable = false,
		Phases = [
			new(30, "$0 inspect|inspects $i1 and prepare|prepares the work with $t1.", "$0 inspect|inspects $i1 and prepare|prepares the work with $t1.", ExertionLevel.Stasis, 0),
			new(45, "$0 work|works the ingredients steadily through the main preparation, using $t2.", "$0 work|works the ingredients steadily through the main preparation, using $t2.", ExertionLevel.Stasis, 0),
			new(45, "$0 finish|finishes the preparation and set|sets aside $p1.", "$0 spoil|spoils the final preparation and have|has nothing useful to show for it.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1000.0, Material: "meat", PileTag: "Salted Meat Commodity", CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1 kilogram of meat", ["piletag Salted Meat Commodity"]), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Smoking Rack", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Smoking Rack tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Fire", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Fire tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueCommodityRecipeProduct(Grams: 800.0, Material: "meat", Tag: "Smoked Meat Commodity", Characteristics: []) { Compatibility = new("800 grams of meat commodity", ["tag Smoked Meat Commodity"]), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MedievalFoodProcessingMedievalFoodSaltRawFish() => new()
	{
		Metadata = new("Medieval Food / Processing/medieval food - salt raw fish", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.MedievalFood.cs:138", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "medieval food - salt raw fish",
		Category = "Medieval Food / Processing",
		Blurb = "medieval food - salt raw fish",
		Action = "medieval food - salt raw fish",
		ActiveCraftItemSdesc = "an in-progress medieval food - salt raw fish craft",
		Gate = CatalogueRecipeGates.MedievalFoodProductionCooking152,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 3,
		Interruptable = false,
		Phases = [
			new(30, "$0 inspect|inspects $i1 and prepare|prepares the work with $t1.", "$0 inspect|inspects $i1 and prepare|prepares the work with $t1.", ExertionLevel.Stasis, 0),
			new(45, "$0 work|works the ingredients steadily through the main preparation.", "$0 work|works the ingredients steadily through the main preparation.", ExertionLevel.Stasis, 0),
			new(45, "$0 finish|finishes the preparation and set|sets aside $p1.", "$0 spoil|spoils the final preparation and have|has nothing useful to show for it.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1000.0, Material: "fish", PileTag: "Raw Fish Commodity", CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1 kilogram of fish", ["piletag Raw Fish Commodity"]), QualityWeight = 1 },
			new CatalogueCommodityRecipeInput(Grams: 200.0, Material: "salt", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("200 grams of salt", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Salting Trough", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Salting Trough tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueCommodityRecipeProduct(Grams: 1000.0, Material: "fish", Tag: "Salted Fish Commodity", Characteristics: []) { Compatibility = new("1 kilogram of fish commodity", ["tag Salted Fish Commodity"]), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MedievalFoodProcessingMedievalFoodDrySaltedFish() => new()
	{
		Metadata = new("Medieval Food / Processing/medieval food - dry salted fish", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.MedievalFood.cs:138", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "medieval food - dry salted fish",
		Category = "Medieval Food / Processing",
		Blurb = "medieval food - dry salted fish",
		Action = "medieval food - dry salted fish",
		ActiveCraftItemSdesc = "an in-progress medieval food - dry salted fish craft",
		Gate = CatalogueRecipeGates.MedievalFoodProductionCooking152,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 3,
		Interruptable = false,
		Phases = [
			new(30, "$0 inspect|inspects $i1 and prepare|prepares the work with $t1.", "$0 inspect|inspects $i1 and prepare|prepares the work with $t1.", ExertionLevel.Stasis, 0),
			new(45, "$0 work|works the ingredients steadily through the main preparation.", "$0 work|works the ingredients steadily through the main preparation.", ExertionLevel.Stasis, 0),
			new(45, "$0 finish|finishes the preparation and set|sets aside $p1.", "$0 spoil|spoils the final preparation and have|has nothing useful to show for it.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1000.0, Material: "fish", PileTag: "Salted Fish Commodity", CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1 kilogram of fish", ["piletag Salted Fish Commodity"]), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Drying Rack", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Drying Rack tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueCommodityRecipeProduct(Grams: 700.0, Material: "fish", Tag: "Dried Fish Commodity", Characteristics: []) { Compatibility = new("700 grams of fish commodity", ["tag Dried Fish Commodity"]), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MedievalFoodProcessingMedievalFoodSmokeSaltedFish() => new()
	{
		Metadata = new("Medieval Food / Processing/medieval food - smoke salted fish", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.MedievalFood.cs:138", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "medieval food - smoke salted fish",
		Category = "Medieval Food / Processing",
		Blurb = "medieval food - smoke salted fish",
		Action = "medieval food - smoke salted fish",
		ActiveCraftItemSdesc = "an in-progress medieval food - smoke salted fish craft",
		Gate = CatalogueRecipeGates.MedievalFoodProductionCooking152,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 3,
		Interruptable = false,
		Phases = [
			new(30, "$0 inspect|inspects $i1 and prepare|prepares the work with $t1.", "$0 inspect|inspects $i1 and prepare|prepares the work with $t1.", ExertionLevel.Stasis, 0),
			new(45, "$0 work|works the ingredients steadily through the main preparation, using $t2.", "$0 work|works the ingredients steadily through the main preparation, using $t2.", ExertionLevel.Stasis, 0),
			new(45, "$0 finish|finishes the preparation and set|sets aside $p1.", "$0 spoil|spoils the final preparation and have|has nothing useful to show for it.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1000.0, Material: "fish", PileTag: "Salted Fish Commodity", CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1 kilogram of fish", ["piletag Salted Fish Commodity"]), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Smoking Rack", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Smoking Rack tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Fire", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Fire tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueCommodityRecipeProduct(Grams: 800.0, Material: "fish", Tag: "Smoked Fish Commodity", Characteristics: []) { Compatibility = new("800 grams of fish commodity", ["tag Smoked Fish Commodity"]), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MedievalFoodFinishedProvisionsMedievalFoodBakeCoarseBreadLoaf() => new()
	{
		Metadata = new("Medieval Food / Finished Provisions/medieval food - bake coarse bread loaf", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:medieval_food_coarse_bread_loaf"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.MedievalFood.cs:138", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "medieval food - bake coarse bread loaf",
		Category = "Medieval Food / Finished Provisions",
		Blurb = "medieval food - bake coarse bread loaf",
		Action = "medieval food - bake coarse bread loaf",
		ActiveCraftItemSdesc = "an in-progress medieval food - bake coarse bread loaf craft",
		Gate = CatalogueRecipeGates.MedievalFoodProductionCooking153,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 3,
		Interruptable = false,
		Phases = [
			new(30, "$0 inspect|inspects $i1 and prepare|prepares the work with $t1.", "$0 inspect|inspects $i1 and prepare|prepares the work with $t1.", ExertionLevel.Stasis, 0),
			new(45, "$0 work|works the ingredients steadily through the main preparation, using $t2.", "$0 work|works the ingredients steadily through the main preparation, using $t2.", ExertionLevel.Stasis, 0),
			new(45, "$0 finish|finishes the preparation and set|sets aside $p1.", "$0 spoil|spoils the final preparation and have|has nothing useful to show for it.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 700.0, Material: "wheat", PileTag: "Dough Commodity", CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("700 grams of wheat", ["piletag Dough Commodity"]), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Bake Oven", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Bake Oven tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Fire", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Fire tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueCookedRecipeProduct(ItemStableReference: "medieval_food_coarse_bread_loaf", Quantity: 1, SkinStableReference: null, RemoveDrugsAndFoodEffects: false, Ingredients: [new(1, "grain")]) { Compatibility = new("1x a coarse brown bread loaf (@medieval_food_coarse_bread_loaf)", ["ingredient $i1 = grain"]), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MedievalFoodFinishedProvisionsMedievalFoodBakeBroadFlatbread() => new()
	{
		Metadata = new("Medieval Food / Finished Provisions/medieval food - bake broad flatbread", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:medieval_food_flatbread"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.MedievalFood.cs:138", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "medieval food - bake broad flatbread",
		Category = "Medieval Food / Finished Provisions",
		Blurb = "medieval food - bake broad flatbread",
		Action = "medieval food - bake broad flatbread",
		ActiveCraftItemSdesc = "an in-progress medieval food - bake broad flatbread craft",
		Gate = CatalogueRecipeGates.MedievalFoodProductionCooking153,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 3,
		Interruptable = false,
		Phases = [
			new(30, "$0 inspect|inspects $i1 and prepare|prepares the work with $t1.", "$0 inspect|inspects $i1 and prepare|prepares the work with $t1.", ExertionLevel.Stasis, 0),
			new(45, "$0 work|works the ingredients steadily through the main preparation, using $t2.", "$0 work|works the ingredients steadily through the main preparation, using $t2.", ExertionLevel.Stasis, 0),
			new(45, "$0 finish|finishes the preparation and set|sets aside $p1.", "$0 spoil|spoils the final preparation and have|has nothing useful to show for it.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 320.0, Material: "wheat", PileTag: "Dough Commodity", CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("320 grams of wheat", ["piletag Dough Commodity"]), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Bake Oven", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Bake Oven tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Fire", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Fire tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueCookedRecipeProduct(ItemStableReference: "medieval_food_flatbread", Quantity: 1, SkinStableReference: null, RemoveDrugsAndFoodEffects: false, Ingredients: [new(1, "grain")]) { Compatibility = new("1x a broad griddle-baked flatbread (@medieval_food_flatbread)", ["ingredient $i1 = grain"]), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MedievalFoodFinishedProvisionsMedievalFoodBakeHardTravelBread() => new()
	{
		Metadata = new("Medieval Food / Finished Provisions/medieval food - bake hard travel bread", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:medieval_food_hard_bread"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.MedievalFood.cs:138", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "medieval food - bake hard travel bread",
		Category = "Medieval Food / Finished Provisions",
		Blurb = "medieval food - bake hard travel bread",
		Action = "medieval food - bake hard travel bread",
		ActiveCraftItemSdesc = "an in-progress medieval food - bake hard travel bread craft",
		Gate = CatalogueRecipeGates.MedievalFoodProductionCooking153,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 3,
		Interruptable = false,
		Phases = [
			new(30, "$0 inspect|inspects $i1 and prepare|prepares the work with $t1.", "$0 inspect|inspects $i1 and prepare|prepares the work with $t1.", ExertionLevel.Stasis, 0),
			new(45, "$0 work|works the ingredients steadily through the main preparation, using $t2.", "$0 work|works the ingredients steadily through the main preparation, using $t2.", ExertionLevel.Stasis, 0),
			new(45, "$0 finish|finishes the preparation and set|sets aside $p1.", "$0 spoil|spoils the final preparation and have|has nothing useful to show for it.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 450.0, Material: "wheat", PileTag: "Dough Commodity", CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("450 grams of wheat", ["piletag Dough Commodity"]), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Bake Oven", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Bake Oven tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Fire", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Fire tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueCookedRecipeProduct(ItemStableReference: "medieval_food_hard_bread", Quantity: 1, SkinStableReference: null, RemoveDrugsAndFoodEffects: false, Ingredients: [new(1, "grain")]) { Compatibility = new("1x a hard travel bread (@medieval_food_hard_bread)", ["ingredient $i1 = grain"]), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MedievalFoodFinishedProvisionsMedievalFoodCookThickGrainPottage() => new()
	{
		Metadata = new("Medieval Food / Finished Provisions/medieval food - cook thick grain pottage", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:medieval_food_grain_pottage"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.MedievalFood.cs:138", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "medieval food - cook thick grain pottage",
		Category = "Medieval Food / Finished Provisions",
		Blurb = "medieval food - cook thick grain pottage",
		Action = "medieval food - cook thick grain pottage",
		ActiveCraftItemSdesc = "an in-progress medieval food - cook thick grain pottage craft",
		Gate = CatalogueRecipeGates.MedievalFoodProductionCooking153,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 3,
		Interruptable = false,
		Phases = [
			new(30, "$0 inspect|inspects $i1 and prepare|prepares the work with $t1.", "$0 inspect|inspects $i1 and prepare|prepares the work with $t1.", ExertionLevel.Stasis, 0),
			new(45, "$0 work|works the ingredients steadily through the main preparation, using $t2.", "$0 work|works the ingredients steadily through the main preparation, using $t2.", ExertionLevel.Stasis, 0),
			new(45, "$0 finish|finishes the preparation and set|sets aside $p1.", "$0 spoil|spoils the final preparation and have|has nothing useful to show for it.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 400.0, Material: "wheat", PileTag: "Meal Commodity", CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("400 grams of wheat", ["piletag Meal Commodity"]), QualityWeight = 1 },
			new CatalogueLiquidRecipeInput(Liquid: "Water", Litres: 2.0) { Compatibility = new("2 litres of Water", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Cooking Pot", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Cooking Pot tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Fire", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Fire tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueCookedRecipeProduct(ItemStableReference: "medieval_food_grain_pottage", Quantity: 1, SkinStableReference: null, RemoveDrugsAndFoodEffects: false, Ingredients: [new(1, "grain")]) { Compatibility = new("1x a bowlful of thick grain pottage (@medieval_food_grain_pottage)", ["ingredient $i1 = grain"]), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MedievalFoodFinishedProvisionsMedievalFoodCookMeatAndGrainPottage() => new()
	{
		Metadata = new("Medieval Food / Finished Provisions/medieval food - cook meat and grain pottage", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:medieval_food_meat_pottage"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.MedievalFood.cs:138", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "medieval food - cook meat and grain pottage",
		Category = "Medieval Food / Finished Provisions",
		Blurb = "medieval food - cook meat and grain pottage",
		Action = "medieval food - cook meat and grain pottage",
		ActiveCraftItemSdesc = "an in-progress medieval food - cook meat and grain pottage craft",
		Gate = CatalogueRecipeGates.MedievalFoodProductionCooking153,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 3,
		Interruptable = false,
		Phases = [
			new(30, "$0 inspect|inspects $i1 and prepare|prepares the work with $t1.", "$0 inspect|inspects $i1 and prepare|prepares the work with $t1.", ExertionLevel.Stasis, 0),
			new(45, "$0 work|works the ingredients steadily through the main preparation, using $t2.", "$0 work|works the ingredients steadily through the main preparation, using $t2.", ExertionLevel.Stasis, 0),
			new(45, "$0 finish|finishes the preparation and set|sets aside $p1.", "$0 spoil|spoils the final preparation and have|has nothing useful to show for it.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 250.0, Material: "wheat", PileTag: "Meal Commodity", CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("250 grams of wheat", ["piletag Meal Commodity"]), QualityWeight = 1 },
			new CatalogueCommodityRecipeInput(Grams: 400.0, Material: "meat", PileTag: "Raw Meat Commodity", CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("400 grams of meat", ["piletag Raw Meat Commodity"]), QualityWeight = 1 },
			new CatalogueLiquidRecipeInput(Liquid: "Water", Litres: 2.0) { Compatibility = new("2 litres of Water", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Cooking Pot", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Cooking Pot tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Fire", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Fire tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueCookedRecipeProduct(ItemStableReference: "medieval_food_meat_pottage", Quantity: 1, SkinStableReference: null, RemoveDrugsAndFoodEffects: false, Ingredients: [new(1, "grain"), new(2, "meat")]) { Compatibility = new("1x a bowlful of meat and grain pottage (@medieval_food_meat_pottage)", ["ingredient $i1 = grain", "ingredient $i2 = meat"]), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MedievalFoodFinishedProvisionsMedievalFoodPortionSaltedMeatRation() => new()
	{
		Metadata = new("Medieval Food / Finished Provisions/medieval food - portion salted meat ration", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:medieval_food_salted_meat_ration"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.MedievalFood.cs:138", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "medieval food - portion salted meat ration",
		Category = "Medieval Food / Finished Provisions",
		Blurb = "medieval food - portion salted meat ration",
		Action = "medieval food - portion salted meat ration",
		ActiveCraftItemSdesc = "an in-progress medieval food - portion salted meat ration craft",
		Gate = CatalogueRecipeGates.MedievalFoodProductionCooking102,
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
			new(30, "$0 inspect|inspects $i1 and prepare|prepares the work with $t1.", "$0 inspect|inspects $i1 and prepare|prepares the work with $t1.", ExertionLevel.Stasis, 0),
			new(45, "$0 work|works the ingredients steadily through the main preparation.", "$0 work|works the ingredients steadily through the main preparation.", ExertionLevel.Stasis, 0),
			new(45, "$0 finish|finishes the preparation and set|sets aside $p1.", "$0 spoil|spoils the final preparation and have|has nothing useful to show for it.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 520.0, Material: "meat", PileTag: "Salted Meat Commodity", CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("520 grams of meat", ["piletag Salted Meat Commodity"]), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Cooking Knife", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Cooking Knife tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueCookedRecipeProduct(ItemStableReference: "medieval_food_salted_meat_ration", Quantity: 1, SkinStableReference: null, RemoveDrugsAndFoodEffects: false, Ingredients: [new(1, "provision")]) { Compatibility = new("1x a ration of salted meat (@medieval_food_salted_meat_ration)", ["ingredient $i1 = provision"]), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MedievalFoodFinishedProvisionsMedievalFoodPortionDriedMeatRation() => new()
	{
		Metadata = new("Medieval Food / Finished Provisions/medieval food - portion dried meat ration", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:medieval_food_dried_meat_ration"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.MedievalFood.cs:138", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "medieval food - portion dried meat ration",
		Category = "Medieval Food / Finished Provisions",
		Blurb = "medieval food - portion dried meat ration",
		Action = "medieval food - portion dried meat ration",
		ActiveCraftItemSdesc = "an in-progress medieval food - portion dried meat ration craft",
		Gate = CatalogueRecipeGates.MedievalFoodProductionCooking102,
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
			new(30, "$0 inspect|inspects $i1 and prepare|prepares the work with $t1.", "$0 inspect|inspects $i1 and prepare|prepares the work with $t1.", ExertionLevel.Stasis, 0),
			new(45, "$0 work|works the ingredients steadily through the main preparation.", "$0 work|works the ingredients steadily through the main preparation.", ExertionLevel.Stasis, 0),
			new(45, "$0 finish|finishes the preparation and set|sets aside $p1.", "$0 spoil|spoils the final preparation and have|has nothing useful to show for it.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 380.0, Material: "meat", PileTag: "Dried Meat Commodity", CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("380 grams of meat", ["piletag Dried Meat Commodity"]), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Cooking Knife", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Cooking Knife tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueCookedRecipeProduct(ItemStableReference: "medieval_food_dried_meat_ration", Quantity: 1, SkinStableReference: null, RemoveDrugsAndFoodEffects: false, Ingredients: [new(1, "provision")]) { Compatibility = new("1x a ration of dried meat (@medieval_food_dried_meat_ration)", ["ingredient $i1 = provision"]), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MedievalFoodFinishedProvisionsMedievalFoodPortionSmokedMeatRation() => new()
	{
		Metadata = new("Medieval Food / Finished Provisions/medieval food - portion smoked meat ration", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:medieval_food_smoked_meat_ration"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.MedievalFood.cs:138", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "medieval food - portion smoked meat ration",
		Category = "Medieval Food / Finished Provisions",
		Blurb = "medieval food - portion smoked meat ration",
		Action = "medieval food - portion smoked meat ration",
		ActiveCraftItemSdesc = "an in-progress medieval food - portion smoked meat ration craft",
		Gate = CatalogueRecipeGates.MedievalFoodProductionCooking102,
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
			new(30, "$0 inspect|inspects $i1 and prepare|prepares the work with $t1.", "$0 inspect|inspects $i1 and prepare|prepares the work with $t1.", ExertionLevel.Stasis, 0),
			new(45, "$0 work|works the ingredients steadily through the main preparation.", "$0 work|works the ingredients steadily through the main preparation.", ExertionLevel.Stasis, 0),
			new(45, "$0 finish|finishes the preparation and set|sets aside $p1.", "$0 spoil|spoils the final preparation and have|has nothing useful to show for it.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 430.0, Material: "meat", PileTag: "Smoked Meat Commodity", CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("430 grams of meat", ["piletag Smoked Meat Commodity"]), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Cooking Knife", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Cooking Knife tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueCookedRecipeProduct(ItemStableReference: "medieval_food_smoked_meat_ration", Quantity: 1, SkinStableReference: null, RemoveDrugsAndFoodEffects: false, Ingredients: [new(1, "provision")]) { Compatibility = new("1x a ration of smoked meat (@medieval_food_smoked_meat_ration)", ["ingredient $i1 = provision"]), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MedievalFoodFinishedProvisionsMedievalFoodPortionSaltedFishRation() => new()
	{
		Metadata = new("Medieval Food / Finished Provisions/medieval food - portion salted fish ration", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:medieval_food_salted_fish_ration"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.MedievalFood.cs:138", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "medieval food - portion salted fish ration",
		Category = "Medieval Food / Finished Provisions",
		Blurb = "medieval food - portion salted fish ration",
		Action = "medieval food - portion salted fish ration",
		ActiveCraftItemSdesc = "an in-progress medieval food - portion salted fish ration craft",
		Gate = CatalogueRecipeGates.MedievalFoodProductionCooking103,
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
			new(30, "$0 inspect|inspects $i1 and prepare|prepares the work with $t1.", "$0 inspect|inspects $i1 and prepare|prepares the work with $t1.", ExertionLevel.Stasis, 0),
			new(45, "$0 work|works the ingredients steadily through the main preparation.", "$0 work|works the ingredients steadily through the main preparation.", ExertionLevel.Stasis, 0),
			new(45, "$0 finish|finishes the preparation and set|sets aside $p1.", "$0 spoil|spoils the final preparation and have|has nothing useful to show for it.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 500.0, Material: "fish", PileTag: "Salted Fish Commodity", CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("500 grams of fish", ["piletag Salted Fish Commodity"]), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Cooking Knife", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Cooking Knife tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueCookedRecipeProduct(ItemStableReference: "medieval_food_salted_fish_ration", Quantity: 1, SkinStableReference: null, RemoveDrugsAndFoodEffects: false, Ingredients: [new(1, "provision")]) { Compatibility = new("1x a ration of salted fish (@medieval_food_salted_fish_ration)", ["ingredient $i1 = provision"]), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MedievalFoodFinishedProvisionsMedievalFoodPortionDriedFishRation() => new()
	{
		Metadata = new("Medieval Food / Finished Provisions/medieval food - portion dried fish ration", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:medieval_food_dried_fish_ration"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.MedievalFood.cs:138", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "medieval food - portion dried fish ration",
		Category = "Medieval Food / Finished Provisions",
		Blurb = "medieval food - portion dried fish ration",
		Action = "medieval food - portion dried fish ration",
		ActiveCraftItemSdesc = "an in-progress medieval food - portion dried fish ration craft",
		Gate = CatalogueRecipeGates.MedievalFoodProductionCooking103,
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
			new(30, "$0 inspect|inspects $i1 and prepare|prepares the work with $t1.", "$0 inspect|inspects $i1 and prepare|prepares the work with $t1.", ExertionLevel.Stasis, 0),
			new(45, "$0 work|works the ingredients steadily through the main preparation.", "$0 work|works the ingredients steadily through the main preparation.", ExertionLevel.Stasis, 0),
			new(45, "$0 finish|finishes the preparation and set|sets aside $p1.", "$0 spoil|spoils the final preparation and have|has nothing useful to show for it.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 340.0, Material: "fish", PileTag: "Dried Fish Commodity", CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("340 grams of fish", ["piletag Dried Fish Commodity"]), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Cooking Knife", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Cooking Knife tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueCookedRecipeProduct(ItemStableReference: "medieval_food_dried_fish_ration", Quantity: 1, SkinStableReference: null, RemoveDrugsAndFoodEffects: false, Ingredients: [new(1, "provision")]) { Compatibility = new("1x a ration of dried fish (@medieval_food_dried_fish_ration)", ["ingredient $i1 = provision"]), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MedievalFoodFinishedProvisionsMedievalFoodPortionSmokedFishRation() => new()
	{
		Metadata = new("Medieval Food / Finished Provisions/medieval food - portion smoked fish ration", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:medieval_food_smoked_fish_ration"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.MedievalFood.cs:138", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "medieval food - portion smoked fish ration",
		Category = "Medieval Food / Finished Provisions",
		Blurb = "medieval food - portion smoked fish ration",
		Action = "medieval food - portion smoked fish ration",
		ActiveCraftItemSdesc = "an in-progress medieval food - portion smoked fish ration craft",
		Gate = CatalogueRecipeGates.MedievalFoodProductionCooking103,
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
			new(30, "$0 inspect|inspects $i1 and prepare|prepares the work with $t1.", "$0 inspect|inspects $i1 and prepare|prepares the work with $t1.", ExertionLevel.Stasis, 0),
			new(45, "$0 work|works the ingredients steadily through the main preparation.", "$0 work|works the ingredients steadily through the main preparation.", ExertionLevel.Stasis, 0),
			new(45, "$0 finish|finishes the preparation and set|sets aside $p1.", "$0 spoil|spoils the final preparation and have|has nothing useful to show for it.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 390.0, Material: "fish", PileTag: "Smoked Fish Commodity", CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("390 grams of fish", ["piletag Smoked Fish Commodity"]), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Cooking Knife", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Cooking Knife tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueCookedRecipeProduct(ItemStableReference: "medieval_food_smoked_fish_ration", Quantity: 1, SkinStableReference: null, RemoveDrugsAndFoodEffects: false, Ingredients: [new(1, "provision")]) { Compatibility = new("1x a ration of smoked fish (@medieval_food_smoked_fish_ration)", ["ingredient $i1 = provision"]), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MedievalFoodBrewingMedievalFoodFermentCaskOfAmberAle() => new()
	{
		Metadata = new("Medieval Food / Brewing/medieval food - ferment cask of amber ale", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:medieval_tableware_table_beer_cask"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.MedievalFood.cs:138", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "medieval food - ferment cask of amber ale",
		Category = "Medieval Food / Brewing",
		Blurb = "medieval food - ferment cask of amber ale",
		Action = "medieval food - ferment cask of amber ale",
		ActiveCraftItemSdesc = "an in-progress medieval food - ferment cask of amber ale craft",
		Gate = CatalogueRecipeGates.MedievalFoodProductionBrewing20,
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
			new(30, "$0 inspect|inspects $i1 and prepare|prepares the work with $t1.", "$0 inspect|inspects $i1 and prepare|prepares the work with $t1.", ExertionLevel.Stasis, 0),
			new(45, "$0 work|works the ingredients steadily through the main preparation.", "$0 work|works the ingredients steadily through the main preparation.", ExertionLevel.Stasis, 0),
			new(45, "$0 finish|finishes the preparation and set|sets aside $p1.", "$0 spoil|spoils the final preparation and have|has nothing useful to show for it.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3500.0, Material: "wheat", PileTag: "Wort Commodity", CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3 kilograms 500 grams of wheat", ["piletag Wort Commodity"]), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Fermenting Gyle Tun", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Fermenting Gyle Tun tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueLiquidRecipeProduct(ItemStableReference: "medieval_tableware_table_beer_cask", Quantity: 1, SkinStableReference: null, Liquid: "amber ale", Litres: 3.5) { Compatibility = new("1x a table beer cask (@medieval_tableware_table_beer_cask) filled with 3.5 litres of amber ale", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MedievalFoodBrewingMedievalFoodFermentCaskOfRedWine() => new()
	{
		Metadata = new("Medieval Food / Brewing/medieval food - ferment cask of red wine", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:medieval_tableware_small_wine_cask"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.MedievalFood.cs:138", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "medieval food - ferment cask of red wine",
		Category = "Medieval Food / Brewing",
		Blurb = "medieval food - ferment cask of red wine",
		Action = "medieval food - ferment cask of red wine",
		ActiveCraftItemSdesc = "an in-progress medieval food - ferment cask of red wine craft",
		Gate = CatalogueRecipeGates.MedievalFoodProductionBrewing20,
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
			new(30, "$0 inspect|inspects $i1 and prepare|prepares the work with $t1.", "$0 inspect|inspects $i1 and prepare|prepares the work with $t1.", ExertionLevel.Stasis, 0),
			new(45, "$0 work|works the ingredients steadily through the main preparation.", "$0 work|works the ingredients steadily through the main preparation.", ExertionLevel.Stasis, 0),
			new(45, "$0 finish|finishes the preparation and set|sets aside $p1.", "$0 spoil|spoils the final preparation and have|has nothing useful to show for it.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 4000.0, Material: "grape", PileTag: "Fruit Must Commodity", CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("4 kilograms of grape", ["piletag Fruit Must Commodity"]), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Fermenting Gyle Tun", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Fermenting Gyle Tun tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueLiquidRecipeProduct(ItemStableReference: "medieval_tableware_small_wine_cask", Quantity: 1, SkinStableReference: null, Liquid: "red wine", Litres: 3.5) { Compatibility = new("1x a small wine cask (@medieval_tableware_small_wine_cask) filled with 3.5 litres of red wine", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingMakeSharedPreparedGrainBarleyCatalogueStock() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/make Shared Prepared grain barley catalogue stock", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["medieval", "renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make Shared Prepared grain barley catalogue stock",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for preindustrial_food_barley_batter_pancake.",
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
			new CatalogueProgCookedRecipeProduct(Prog: "ItemSeederPreIndustrialFood_Shared_Prepared_Grain_preindustrial_food_barley_batter_fritter", RemoveDrugsAndFoodEffects: false, Ingredients: []) { Compatibility = new("ItemSeederPreIndustrialFood_Shared_Prepared_Grain_preindustrial_food_barley_batter_fritter", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PreIndustrialFoodmakingMakeSharedPreparedGrainCornCatalogueStock() => new()
	{
		Metadata = new("Pre-Industrial Foodmaking/make Shared Prepared grain corn catalogue stock", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs:219", []),
		RequiredAnyEra = ["medieval", "renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make Shared Prepared grain corn catalogue stock",
		Category = "Pre-Industrial Foodmaking",
		Blurb = "A catalogue recipe for preindustrial_food_corn_griddle_cake.",
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
			new CatalogueProgCookedRecipeProduct(Prog: "ItemSeederPreIndustrialFood_Shared_Prepared_Grain_preindustrial_food_corn_ash_cake", RemoveDrugsAndFoodEffects: false, Ingredients: []) { Compatibility = new("ItemSeederPreIndustrialFood_Shared_Prepared_Grain_preindustrial_food_corn_ash_cake", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};
}
