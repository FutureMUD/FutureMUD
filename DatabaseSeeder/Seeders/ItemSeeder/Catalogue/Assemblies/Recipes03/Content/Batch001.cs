#nullable enable

using MudSharp.Body;
using MudSharp.GameItems.Inventory.Plans;
using MudSharp.RPG.Checks;

namespace DatabaseSeeder.Seeders;

public static partial class Recipes03Stock
{
	public static CatalogueRecipeDefinition MedievalIndustryStockStackFiredBricks() => new()
	{
		Metadata = new("Medieval Industry / Stock/stack fired bricks", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:medieval_industry_stock_fired_brick_stack"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Medieval.cs:153", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "stack fired bricks",
		Category = "Medieval Industry / Stock",
		Blurb = "stack fired bricks",
		Action = "stack fired bricks",
		ActiveCraftItemSdesc = "an in-progress stack fired bricks craft",
		Gate = CatalogueRecipeGates.MedievalIndustryFoundationsPottery10,
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
			new(30, "$0 inspect|inspects the stock and lay|lays out the work.", "$0 inspect|inspects the stock and lay|lays out the work.", ExertionLevel.Stasis, 0),
			new(45, "$0 shape|shapes and fit|fits the prepared pieces with $t1.", "$0 shape|shapes and fit|fits the prepared pieces with $t1.", ExertionLevel.Stasis, 0),
			new(45, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 botch|botches the finishing work and spoil|spoils the assembly.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 28000.0, Material: "fired brick", PileTag: "Fired Brick Commodity", CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("28 kilograms of fired brick", ["piletag Fired Brick Commodity"]), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hauling Tool", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hauling Tool tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "medieval_industry_stock_fired_brick_stack", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a stack of fired bricks (@medieval_industry_stock_fired_brick_stack)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MedievalIndustryStockPrepareLeatherPanel() => new()
	{
		Metadata = new("Medieval Industry / Stock/prepare leather panel", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:medieval_industry_stock_leather_panel"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Medieval.cs:153", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "prepare leather panel",
		Category = "Medieval Industry / Stock",
		Blurb = "prepare leather panel",
		Action = "prepare leather panel",
		ActiveCraftItemSdesc = "an in-progress leather panel craft",
		Gate = CatalogueRecipeGates.MedievalIndustryFoundationsLeathermaking15,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
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
			new(45, "$0 shape|shapes and fit|fits the prepared pieces with $t1, using $t2.", "$0 shape|shapes and fit|fits the prepared pieces with $t1, using $t2.", ExertionLevel.Stasis, 0),
			new(45, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 botch|botches the finishing work and spoil|spoils the assembly.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1200.0, Material: "leather", PileTag: "Prepared Leather Panel", CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1 kilogram 200 grams of leather", ["piletag Prepared Leather Panel"]), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Awl Punch", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Awl Punch tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Shears", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Shears tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "medieval_industry_stock_leather_panel", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a tanned leather panel (@medieval_industry_stock_leather_panel)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MedievalIndustryStockPrepareParchmentSheet() => new()
	{
		Metadata = new("Medieval Industry / Stock/prepare parchment sheet", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:medieval_industry_stock_parchment_sheet"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Medieval.cs:153", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "prepare parchment sheet",
		Category = "Medieval Industry / Stock",
		Blurb = "prepare parchment sheet",
		Action = "prepare parchment sheet",
		ActiveCraftItemSdesc = "an in-progress parchment sheet craft",
		Gate = CatalogueRecipeGates.MedievalIndustryFoundationsParchmentmaking15,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
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
			new(45, "$0 shape|shapes and fit|fits the prepared pieces with $t1, using $t2.", "$0 shape|shapes and fit|fits the prepared pieces with $t1, using $t2.", ExertionLevel.Stasis, 0),
			new(45, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 botch|botches the finishing work and spoil|spoils the assembly.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 30.0, Material: "parchment", PileTag: "Parchment Sheet Stock", CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("30 grams of parchment", ["piletag Parchment Sheet Stock"]), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Tanning Rack", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Tanning Rack tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Awl Punch", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Awl Punch tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "medieval_industry_stock_parchment_sheet", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a prepared parchment sheet (@medieval_industry_stock_parchment_sheet)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MedievalIndustryStockSpinWoolYarnSkein() => new()
	{
		Metadata = new("Medieval Industry / Stock/spin wool yarn skein", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:medieval_industry_stock_yarn_skein"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Medieval.cs:153", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "spin wool yarn skein",
		Category = "Medieval Industry / Stock",
		Blurb = "spin wool yarn skein",
		Action = "spin wool yarn skein",
		ActiveCraftItemSdesc = "an in-progress spin wool yarn skein craft",
		Gate = CatalogueRecipeGates.MedievalIndustryFoundationsSpinning10,
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
			new(30, "$0 inspect|inspects the stock and lay|lays out the work.", "$0 inspect|inspects the stock and lay|lays out the work.", ExertionLevel.Stasis, 0),
			new(45, "$0 shape|shapes and fit|fits the prepared pieces with $t1.", "$0 shape|shapes and fit|fits the prepared pieces with $t1.", ExertionLevel.Stasis, 0),
			new(45, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 botch|botches the finishing work and spoil|spoils the assembly.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 280.0, Material: "wool", PileTag: "Raw Textile Fibre", CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("280 grams of wool", ["piletag Raw Textile Fibre"]), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Drop Spindle", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Drop Spindle tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "medieval_industry_stock_yarn_skein", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a skein of spun yarn (@medieval_industry_stock_yarn_skein)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MedievalIndustryStockSpinLinenSewingThread() => new()
	{
		Metadata = new("Medieval Industry / Stock/spin linen sewing thread", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:medieval_industry_stock_sewing_thread"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Medieval.cs:153", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "spin linen sewing thread",
		Category = "Medieval Industry / Stock",
		Blurb = "spin linen sewing thread",
		Action = "spin linen sewing thread",
		ActiveCraftItemSdesc = "an in-progress spin linen sewing thread craft",
		Gate = CatalogueRecipeGates.MedievalIndustryFoundationsSpinning10,
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
			new(30, "$0 inspect|inspects the stock and lay|lays out the work.", "$0 inspect|inspects the stock and lay|lays out the work.", ExertionLevel.Stasis, 0),
			new(45, "$0 shape|shapes and fit|fits the prepared pieces with $t1.", "$0 shape|shapes and fit|fits the prepared pieces with $t1.", ExertionLevel.Stasis, 0),
			new(45, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 botch|botches the finishing work and spoil|spoils the assembly.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 70.0, Material: "linen", PileTag: "Prepared Textile Fibre", CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("70 grams of linen", ["piletag Prepared Textile Fibre"]), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Drop Spindle", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Drop Spindle tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "medieval_industry_stock_sewing_thread", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a spool of sewing thread (@medieval_industry_stock_sewing_thread)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MedievalIndustryStockWeavePlainWoolClothBolt() => new()
	{
		Metadata = new("Medieval Industry / Stock/weave plain wool cloth bolt", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:medieval_industry_stock_plain_cloth_bolt"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Medieval.cs:153", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "weave plain wool cloth bolt",
		Category = "Medieval Industry / Stock",
		Blurb = "weave plain wool cloth bolt",
		Action = "weave plain wool cloth bolt",
		ActiveCraftItemSdesc = "an in-progress weave plain wool cloth bolt craft",
		Gate = CatalogueRecipeGates.MedievalIndustryFoundationsWeaving15,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
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
			new CatalogueCommodityRecipeInput(Grams: 4500.0, Material: "wool", PileTag: "Spun Yarn", CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("4 kilograms 500 grams of wool", ["piletag Spun Yarn"]), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Loom", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Loom tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "medieval_industry_stock_plain_cloth_bolt", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a bolt of plain wool cloth (@medieval_industry_stock_plain_cloth_bolt)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MedievalIndustryStockRenderHideGlueCake() => new()
	{
		Metadata = new("Medieval Industry / Stock/render hide glue cake", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:medieval_industry_stock_glue_cake"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Medieval.cs:153", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "render hide glue cake",
		Category = "Medieval Industry / Stock",
		Blurb = "render hide glue cake",
		Action = "render hide glue cake",
		ActiveCraftItemSdesc = "an in-progress render hide glue cake craft",
		Gate = CatalogueRecipeGates.MedievalIndustryFoundationsLeathermaking152,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
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
			new CatalogueCommodityRecipeInput(Grams: 500.0, Material: "bone", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("500 grams of bone", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hot Fire", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Hot Fire tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "medieval_industry_stock_glue_cake", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a hide-glue cake (@medieval_industry_stock_glue_cake)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MedievalIndustryStockFormSealingWaxStick() => new()
	{
		Metadata = new("Medieval Industry / Stock/form sealing wax stick", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:medieval_industry_stock_seal_wax_stick"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Medieval.cs:153", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "form sealing wax stick",
		Category = "Medieval Industry / Stock",
		Blurb = "form sealing wax stick",
		Action = "form sealing wax stick",
		ActiveCraftItemSdesc = "an in-progress sealing wax stick craft",
		Gate = CatalogueRecipeGates.MedievalIndustryFoundationsCandlemaking10,
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
			new(30, "$0 inspect|inspects the stock and lay|lays out the work.", "$0 inspect|inspects the stock and lay|lays out the work.", ExertionLevel.Stasis, 0),
			new(45, "$0 shape|shapes and fit|fits the prepared pieces with $t1.", "$0 shape|shapes and fit|fits the prepared pieces with $t1.", ExertionLevel.Stasis, 0),
			new(45, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 botch|botches the finishing work and spoil|spoils the assembly.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 45.0, Material: "beeswax", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("45 grams of beeswax", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hot Fire", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Hot Fire tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "medieval_industry_stock_seal_wax_stick", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a red seal-wax stick (@medieval_industry_stock_seal_wax_stick)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MedievalIndustryStockCutCleanBandageRoll() => new()
	{
		Metadata = new("Medieval Industry / Stock/cut clean bandage roll", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:medieval_industry_stock_bandage_roll"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Medieval.cs:153", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "cut clean bandage roll",
		Category = "Medieval Industry / Stock",
		Blurb = "cut clean bandage roll",
		Action = "cut clean bandage roll",
		ActiveCraftItemSdesc = "an in-progress clean bandage roll craft",
		Gate = CatalogueRecipeGates.MedievalIndustryFoundationsTailoring10,
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
			new(30, "$0 inspect|inspects the stock and lay|lays out the work.", "$0 inspect|inspects the stock and lay|lays out the work.", ExertionLevel.Stasis, 0),
			new(45, "$0 shape|shapes and fit|fits the prepared pieces with $t1.", "$0 shape|shapes and fit|fits the prepared pieces with $t1.", ExertionLevel.Stasis, 0),
			new(45, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 botch|botches the finishing work and spoil|spoils the assembly.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 200.0, Material: "linen", PileTag: "Woven Cloth", CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("200 grams of linen", ["piletag Woven Cloth"]), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Shears", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Shears tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "medieval_industry_stock_bandage_roll", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a roll of clean bandage linen (@medieval_industry_stock_bandage_roll)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MedievalIndustryToolmakingForgeIronFellingAxe() => new()
	{
		Metadata = new("Medieval Industry / Toolmaking/forge iron felling axe", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:medieval_tool_felling_axe"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Medieval.cs:153", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "forge iron felling axe",
		Category = "Medieval Industry / Toolmaking",
		Blurb = "forge iron felling axe",
		Action = "forge iron felling axe",
		ActiveCraftItemSdesc = "an in-progress iron felling axe craft",
		Gate = CatalogueRecipeGates.MedievalIndustryFoundationsBlacksmithing20,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
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
			new(45, "$0 shape|shapes and fit|fits the prepared pieces with $t1, using $t2 and $t3.", "$0 shape|shapes and fit|fits the prepared pieces with $t1, using $t2 and $t3.", ExertionLevel.Stasis, 0),
			new(45, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 botch|botches the finishing work and spoil|spoils the assembly.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueItemRecipeInput(ItemStableReference: "medieval_industry_stock_iron_bar", Quantity: 1) { Compatibility = new("1x a wrought-iron bar (@medieval_industry_stock_iron_bar)", []), QualityWeight = 1 },
			new CatalogueItemRecipeInput(ItemStableReference: "medieval_industry_stock_handle_blanks", Quantity: 1) { Compatibility = new("1x a bundle of handle blanks (@medieval_industry_stock_handle_blanks)", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Anvil", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Anvil tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Forge Tongs", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Forge Tongs tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "medieval_tool_felling_axe", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x an iron felling axe (@medieval_tool_felling_axe)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MedievalIndustryToolmakingForgeIronHandSaw() => new()
	{
		Metadata = new("Medieval Industry / Toolmaking/forge iron hand saw", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:medieval_tool_hand_saw"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Medieval.cs:153", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "forge iron hand saw",
		Category = "Medieval Industry / Toolmaking",
		Blurb = "forge iron hand saw",
		Action = "forge iron hand saw",
		ActiveCraftItemSdesc = "an in-progress iron hand saw craft",
		Gate = CatalogueRecipeGates.MedievalIndustryFoundationsBlacksmithing20,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
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
			new(45, "$0 shape|shapes and fit|fits the prepared pieces with $t1, using $t2 and $t3.", "$0 shape|shapes and fit|fits the prepared pieces with $t1, using $t2 and $t3.", ExertionLevel.Stasis, 0),
			new(45, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 botch|botches the finishing work and spoil|spoils the assembly.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueItemRecipeInput(ItemStableReference: "medieval_industry_stock_iron_bar", Quantity: 1) { Compatibility = new("1x a wrought-iron bar (@medieval_industry_stock_iron_bar)", []), QualityWeight = 1 },
			new CatalogueItemRecipeInput(ItemStableReference: "medieval_industry_stock_handle_blanks", Quantity: 1) { Compatibility = new("1x a bundle of handle blanks (@medieval_industry_stock_handle_blanks)", []), QualityWeight = 1 },
			new CatalogueItemRecipeInput(ItemStableReference: "medieval_industry_stock_rivet_packet", Quantity: 1) { Compatibility = new("1x a packet of iron rivets (@medieval_industry_stock_rivet_packet)", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Anvil", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Anvil tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Forge Tongs", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Forge Tongs tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "medieval_tool_hand_saw", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x an iron hand saw (@medieval_tool_hand_saw)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MedievalIndustryToolmakingForgeIronWoodChisel() => new()
	{
		Metadata = new("Medieval Industry / Toolmaking/forge iron wood chisel", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:medieval_tool_wood_chisel"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Medieval.cs:153", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "forge iron wood chisel",
		Category = "Medieval Industry / Toolmaking",
		Blurb = "forge iron wood chisel",
		Action = "forge iron wood chisel",
		ActiveCraftItemSdesc = "an in-progress iron wood chisel craft",
		Gate = CatalogueRecipeGates.MedievalIndustryFoundationsBlacksmithing20,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
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
			new(45, "$0 shape|shapes and fit|fits the prepared pieces with $t1, using $t2 and $t3.", "$0 shape|shapes and fit|fits the prepared pieces with $t1, using $t2 and $t3.", ExertionLevel.Stasis, 0),
			new(45, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 botch|botches the finishing work and spoil|spoils the assembly.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueItemRecipeInput(ItemStableReference: "medieval_industry_stock_iron_bar", Quantity: 1) { Compatibility = new("1x a wrought-iron bar (@medieval_industry_stock_iron_bar)", []), QualityWeight = 1 },
			new CatalogueItemRecipeInput(ItemStableReference: "medieval_industry_stock_handle_blanks", Quantity: 1) { Compatibility = new("1x a bundle of handle blanks (@medieval_industry_stock_handle_blanks)", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Anvil", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Anvil tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Forge Tongs", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Forge Tongs tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "medieval_tool_wood_chisel", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x an iron wood chisel (@medieval_tool_wood_chisel)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MedievalIndustryToolmakingForgeIronWoodAuger() => new()
	{
		Metadata = new("Medieval Industry / Toolmaking/forge iron wood auger", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:medieval_tool_wood_auger"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Medieval.cs:153", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "forge iron wood auger",
		Category = "Medieval Industry / Toolmaking",
		Blurb = "forge iron wood auger",
		Action = "forge iron wood auger",
		ActiveCraftItemSdesc = "an in-progress iron wood auger craft",
		Gate = CatalogueRecipeGates.MedievalIndustryFoundationsBlacksmithing20,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
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
			new(45, "$0 shape|shapes and fit|fits the prepared pieces with $t1, using $t2 and $t3.", "$0 shape|shapes and fit|fits the prepared pieces with $t1, using $t2 and $t3.", ExertionLevel.Stasis, 0),
			new(45, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 botch|botches the finishing work and spoil|spoils the assembly.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueItemRecipeInput(ItemStableReference: "medieval_industry_stock_iron_bar", Quantity: 1) { Compatibility = new("1x a wrought-iron bar (@medieval_industry_stock_iron_bar)", []), QualityWeight = 1 },
			new CatalogueItemRecipeInput(ItemStableReference: "medieval_industry_stock_handle_blanks", Quantity: 1) { Compatibility = new("1x a bundle of handle blanks (@medieval_industry_stock_handle_blanks)", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Anvil", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Anvil tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Forge Tongs", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Forge Tongs tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "medieval_tool_wood_auger", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x an iron wood auger (@medieval_tool_wood_auger)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MedievalIndustryToolmakingForgeIronDrawknife() => new()
	{
		Metadata = new("Medieval Industry / Toolmaking/forge iron drawknife", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:medieval_tool_drawknife"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Medieval.cs:153", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "forge iron drawknife",
		Category = "Medieval Industry / Toolmaking",
		Blurb = "forge iron drawknife",
		Action = "forge iron drawknife",
		ActiveCraftItemSdesc = "an in-progress iron drawknife craft",
		Gate = CatalogueRecipeGates.MedievalIndustryFoundationsBlacksmithing20,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
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
			new(45, "$0 shape|shapes and fit|fits the prepared pieces with $t1, using $t2 and $t3.", "$0 shape|shapes and fit|fits the prepared pieces with $t1, using $t2 and $t3.", ExertionLevel.Stasis, 0),
			new(45, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 botch|botches the finishing work and spoil|spoils the assembly.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueItemRecipeInput(ItemStableReference: "medieval_industry_stock_iron_bar", Quantity: 1) { Compatibility = new("1x a wrought-iron bar (@medieval_industry_stock_iron_bar)", []), QualityWeight = 1 },
			new CatalogueItemRecipeInput(ItemStableReference: "medieval_industry_stock_handle_blanks", Quantity: 1) { Compatibility = new("1x a bundle of handle blanks (@medieval_industry_stock_handle_blanks)", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Anvil", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Anvil tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Forge Tongs", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Forge Tongs tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "medieval_tool_drawknife", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x an iron drawknife (@medieval_tool_drawknife)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MedievalIndustryToolmakingForgeIronHideScraper() => new()
	{
		Metadata = new("Medieval Industry / Toolmaking/forge iron hide scraper", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:medieval_tool_hide_scraper"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Medieval.cs:153", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "forge iron hide scraper",
		Category = "Medieval Industry / Toolmaking",
		Blurb = "forge iron hide scraper",
		Action = "forge iron hide scraper",
		ActiveCraftItemSdesc = "an in-progress iron hide scraper craft",
		Gate = CatalogueRecipeGates.MedievalIndustryFoundationsBlacksmithing20,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
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
			new(45, "$0 shape|shapes and fit|fits the prepared pieces with $t1, using $t2 and $t3.", "$0 shape|shapes and fit|fits the prepared pieces with $t1, using $t2 and $t3.", ExertionLevel.Stasis, 0),
			new(45, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 botch|botches the finishing work and spoil|spoils the assembly.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueItemRecipeInput(ItemStableReference: "medieval_industry_stock_iron_bar", Quantity: 1) { Compatibility = new("1x a wrought-iron bar (@medieval_industry_stock_iron_bar)", []), QualityWeight = 1 },
			new CatalogueItemRecipeInput(ItemStableReference: "medieval_industry_stock_handle_blanks", Quantity: 1) { Compatibility = new("1x a bundle of handle blanks (@medieval_industry_stock_handle_blanks)", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Anvil", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Anvil tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Forge Tongs", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Forge Tongs tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "medieval_tool_hide_scraper", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x an iron hide scraper (@medieval_tool_hide_scraper)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MedievalIndustryToolmakingBuildSlopedTanningBeam() => new()
	{
		Metadata = new("Medieval Industry / Toolmaking/build sloped tanning beam", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:medieval_tool_tanning_beam"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Medieval.cs:153", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "build sloped tanning beam",
		Category = "Medieval Industry / Toolmaking",
		Blurb = "build sloped tanning beam",
		Action = "build sloped tanning beam",
		ActiveCraftItemSdesc = "an in-progress sloped tanning beam craft",
		Gate = CatalogueRecipeGates.MedievalIndustryFoundationsCarpentry20,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
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
			new(45, "$0 shape|shapes and fit|fits the prepared pieces with $t1, using $t2.", "$0 shape|shapes and fit|fits the prepared pieces with $t1, using $t2.", ExertionLevel.Stasis, 0),
			new(45, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 botch|botches the finishing work and spoil|spoils the assembly.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueItemRecipeInput(ItemStableReference: "medieval_industry_stock_plank_bundle", Quantity: 1) { Compatibility = new("1x a bundle of worked planks (@medieval_industry_stock_plank_bundle)", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Awl Punch", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Awl Punch tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "medieval_tool_tanning_beam", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a sloped tanning beam (@medieval_tool_tanning_beam)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MedievalIndustryWorkshopApparatusBuildSmithingForge() => new()
	{
		Metadata = new("Medieval Industry / Workshop Apparatus/build smithing forge", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:medieval_workshop_forge"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Medieval.cs:153", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "build smithing forge",
		Category = "Medieval Industry / Workshop Apparatus",
		Blurb = "build smithing forge",
		Action = "build smithing forge",
		ActiveCraftItemSdesc = "an in-progress smithing forge craft",
		Gate = CatalogueRecipeGates.MedievalIndustryFoundationsMasonry25,
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
			new(30, "$0 inspect|inspects the stock and lay|lays out the work.", "$0 inspect|inspects the stock and lay|lays out the work.", ExertionLevel.Stasis, 0),
			new(45, "$0 shape|shapes and fit|fits the prepared pieces with $t1, using $t2.", "$0 shape|shapes and fit|fits the prepared pieces with $t1, using $t2.", ExertionLevel.Stasis, 0),
			new(45, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 botch|botches the finishing work and spoil|spoils the assembly.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueItemRecipeInput(ItemStableReference: "medieval_industry_stock_clay_body_lump", Quantity: 1) { Compatibility = new("1x a lump of prepared clay body (@medieval_industry_stock_clay_body_lump)", []), QualityWeight = 1 },
			new CatalogueItemRecipeInput(ItemStableReference: "medieval_industry_stock_fired_brick_stack", Quantity: 1) { Compatibility = new("1x a stack of fired bricks (@medieval_industry_stock_fired_brick_stack)", []), QualityWeight = 1 },
			new CatalogueItemRecipeInput(ItemStableReference: "medieval_industry_stock_iron_bar", Quantity: 1) { Compatibility = new("1x a wrought-iron bar (@medieval_industry_stock_iron_bar)", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Masonry Tool", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Masonry Tool tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Kiln Tool", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Kiln Tool tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "medieval_workshop_forge", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x an unlit smithing forge (@medieval_workshop_forge)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MedievalIndustryWorkshopApparatusBuildBloomerySmeltingFurnace() => new()
	{
		Metadata = new("Medieval Industry / Workshop Apparatus/build bloomery smelting furnace", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:medieval_workshop_smelting_furnace"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Medieval.cs:153", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "build bloomery smelting furnace",
		Category = "Medieval Industry / Workshop Apparatus",
		Blurb = "build bloomery smelting furnace",
		Action = "build bloomery smelting furnace",
		ActiveCraftItemSdesc = "an in-progress bloomery smelting furnace craft",
		Gate = CatalogueRecipeGates.MedievalIndustryFoundationsMasonry25,
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
			new(30, "$0 inspect|inspects the stock and lay|lays out the work.", "$0 inspect|inspects the stock and lay|lays out the work.", ExertionLevel.Stasis, 0),
			new(45, "$0 shape|shapes and fit|fits the prepared pieces with $t1, using $t2.", "$0 shape|shapes and fit|fits the prepared pieces with $t1, using $t2.", ExertionLevel.Stasis, 0),
			new(45, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 botch|botches the finishing work and spoil|spoils the assembly.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueItemRecipeInput(ItemStableReference: "medieval_industry_stock_clay_body_lump", Quantity: 1) { Compatibility = new("1x a lump of prepared clay body (@medieval_industry_stock_clay_body_lump)", []), QualityWeight = 1 },
			new CatalogueItemRecipeInput(ItemStableReference: "medieval_industry_stock_fired_brick_stack", Quantity: 1) { Compatibility = new("1x a stack of fired bricks (@medieval_industry_stock_fired_brick_stack)", []), QualityWeight = 1 },
			new CatalogueItemRecipeInput(ItemStableReference: "medieval_industry_stock_iron_bar", Quantity: 1) { Compatibility = new("1x a wrought-iron bar (@medieval_industry_stock_iron_bar)", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Masonry Tool", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Masonry Tool tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Kiln Tool", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Kiln Tool tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "medieval_workshop_smelting_furnace", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x an unlit bloomery furnace (@medieval_workshop_smelting_furnace)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MedievalIndustryWorkshopApparatusFireCeramicCrucible() => new()
	{
		Metadata = new("Medieval Industry / Workshop Apparatus/fire ceramic crucible", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:medieval_tool_crucible"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Medieval.cs:153", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "fire ceramic crucible",
		Category = "Medieval Industry / Workshop Apparatus",
		Blurb = "fire ceramic crucible",
		Action = "fire ceramic crucible",
		ActiveCraftItemSdesc = "an in-progress fire ceramic crucible craft",
		Gate = CatalogueRecipeGates.MedievalIndustryFoundationsPottery25,
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
			new(30, "$0 inspect|inspects the stock and lay|lays out the work.", "$0 inspect|inspects the stock and lay|lays out the work.", ExertionLevel.Stasis, 0),
			new(45, "$0 shape|shapes and fit|fits the prepared pieces with $t1.", "$0 shape|shapes and fit|fits the prepared pieces with $t1.", ExertionLevel.Stasis, 0),
			new(45, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 botch|botches the finishing work and spoil|spoils the assembly.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueItemRecipeInput(ItemStableReference: "medieval_industry_stock_clay_body_lump", Quantity: 1) { Compatibility = new("1x a lump of prepared clay body (@medieval_industry_stock_clay_body_lump)", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Kiln Tool", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Kiln Tool tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "medieval_tool_crucible", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a ceramic crucible (@medieval_tool_crucible)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MedievalIndustryWorkshopApparatusBuildTreadledGrindstone() => new()
	{
		Metadata = new("Medieval Industry / Workshop Apparatus/build treadled grindstone", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:medieval_tool_grindstone"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Medieval.cs:153", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "build treadled grindstone",
		Category = "Medieval Industry / Workshop Apparatus",
		Blurb = "build treadled grindstone",
		Action = "build treadled grindstone",
		ActiveCraftItemSdesc = "an in-progress treadled grindstone craft",
		Gate = CatalogueRecipeGates.MedievalIndustryFoundationsMasonry25,
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
			new(30, "$0 inspect|inspects the stock and lay|lays out the work.", "$0 inspect|inspects the stock and lay|lays out the work.", ExertionLevel.Stasis, 0),
			new(45, "$0 shape|shapes and fit|fits the prepared pieces with $t1, using $t2.", "$0 shape|shapes and fit|fits the prepared pieces with $t1, using $t2.", ExertionLevel.Stasis, 0),
			new(45, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 botch|botches the finishing work and spoil|spoils the assembly.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueItemRecipeInput(ItemStableReference: "medieval_industry_stock_plank_bundle", Quantity: 1) { Compatibility = new("1x a bundle of worked planks (@medieval_industry_stock_plank_bundle)", []), QualityWeight = 1 },
			new CatalogueItemRecipeInput(ItemStableReference: "medieval_industry_stock_iron_bar", Quantity: 1) { Compatibility = new("1x a wrought-iron bar (@medieval_industry_stock_iron_bar)", []), QualityWeight = 1 },
			new CatalogueCommodityTagRecipeInput(Grams: 28000.0, MaterialTag: "Materials", PileTag: "Dressed Stone Block Commodity", CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("28 kilograms of a material tagged as Materials", ["piletag Dressed Stone Block Commodity"]), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Masonry Tool", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Masonry Tool tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "medieval_tool_grindstone", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a treadled grindstone (@medieval_tool_grindstone)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MedievalIndustryToolmakingBuildParchmentStretchingFrame() => new()
	{
		Metadata = new("Medieval Industry / Toolmaking/build parchment stretching frame", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:medieval_tool_parchment_stretching_frame"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Medieval.cs:153", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "build parchment stretching frame",
		Category = "Medieval Industry / Toolmaking",
		Blurb = "build parchment stretching frame",
		Action = "build parchment stretching frame",
		ActiveCraftItemSdesc = "an in-progress parchment stretching frame craft",
		Gate = CatalogueRecipeGates.MedievalIndustryFoundationsParchmentmaking25,
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
			new(30, "$0 inspect|inspects the stock and lay|lays out the work.", "$0 inspect|inspects the stock and lay|lays out the work.", ExertionLevel.Stasis, 0),
			new(45, "$0 shape|shapes and fit|fits the prepared pieces with $t1, using $t2.", "$0 shape|shapes and fit|fits the prepared pieces with $t1, using $t2.", ExertionLevel.Stasis, 0),
			new(45, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 botch|botches the finishing work and spoil|spoils the assembly.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueItemRecipeInput(ItemStableReference: "medieval_industry_stock_plank_bundle", Quantity: 1) { Compatibility = new("1x a bundle of worked planks (@medieval_industry_stock_plank_bundle)", []), QualityWeight = 1 },
			new CatalogueItemRecipeInput(ItemStableReference: "medieval_industry_stock_sewing_thread", Quantity: 1) { Compatibility = new("1x a spool of sewing thread (@medieval_industry_stock_sewing_thread)", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Wood Chisel", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Wood Chisel tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Wood Auger", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Wood Auger tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "medieval_tool_parchment_stretching_frame", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a parchment stretching frame (@medieval_tool_parchment_stretching_frame)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MedievalIndustryToolmakingBuildPapermakingMouldAndDeckle() => new()
	{
		Metadata = new("Medieval Industry / Toolmaking/build papermaking mould and deckle", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:medieval_tool_mould_and_deckle"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Medieval.cs:153", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "build papermaking mould and deckle",
		Category = "Medieval Industry / Toolmaking",
		Blurb = "build papermaking mould and deckle",
		Action = "build papermaking mould and deckle",
		ActiveCraftItemSdesc = "an in-progress papermaking mould and deckle craft",
		Gate = CatalogueRecipeGates.MedievalIndustryFoundationsPapermaking25,
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
			new(30, "$0 inspect|inspects the stock and lay|lays out the work.", "$0 inspect|inspects the stock and lay|lays out the work.", ExertionLevel.Stasis, 0),
			new(45, "$0 shape|shapes and fit|fits the prepared pieces with $t1, using $t2.", "$0 shape|shapes and fit|fits the prepared pieces with $t1, using $t2.", ExertionLevel.Stasis, 0),
			new(45, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 botch|botches the finishing work and spoil|spoils the assembly.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueItemRecipeInput(ItemStableReference: "medieval_industry_stock_handle_blanks", Quantity: 1) { Compatibility = new("1x a bundle of handle blanks (@medieval_industry_stock_handle_blanks)", []), QualityWeight = 1 },
			new CatalogueItemRecipeInput(ItemStableReference: "medieval_industry_stock_wire_coil", Quantity: 1) { Compatibility = new("1x a coil of drawn wire (@medieval_industry_stock_wire_coil)", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Wood Chisel", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Wood Chisel tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Wood Auger", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Wood Auger tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "medieval_tool_mould_and_deckle", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a mould and deckle (@medieval_tool_mould_and_deckle)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MedievalIndustryToolmakingBuildWoodenBookPress() => new()
	{
		Metadata = new("Medieval Industry / Toolmaking/build wooden book press", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:medieval_workshop_book_press"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Medieval.cs:153", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "build wooden book press",
		Category = "Medieval Industry / Toolmaking",
		Blurb = "build wooden book press",
		Action = "build wooden book press",
		ActiveCraftItemSdesc = "an in-progress wooden book press craft",
		Gate = CatalogueRecipeGates.MedievalIndustryFoundationsBookbinding25,
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
			new(30, "$0 inspect|inspects the stock and lay|lays out the work.", "$0 inspect|inspects the stock and lay|lays out the work.", ExertionLevel.Stasis, 0),
			new(45, "$0 shape|shapes and fit|fits the prepared pieces with $t1, using $t2 and $t3.", "$0 shape|shapes and fit|fits the prepared pieces with $t1, using $t2 and $t3.", ExertionLevel.Stasis, 0),
			new(45, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 botch|botches the finishing work and spoil|spoils the assembly.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueItemRecipeInput(ItemStableReference: "medieval_industry_stock_plank_bundle", Quantity: 1) { Compatibility = new("1x a bundle of worked planks (@medieval_industry_stock_plank_bundle)", []), QualityWeight = 1 },
			new CatalogueItemRecipeInput(ItemStableReference: "medieval_industry_stock_iron_bar", Quantity: 1) { Compatibility = new("1x a wrought-iron bar (@medieval_industry_stock_iron_bar)", []), QualityWeight = 1 },
			new CatalogueItemRecipeInput(ItemStableReference: "medieval_industry_stock_glue_cake", Quantity: 1) { Compatibility = new("1x a hide-glue cake (@medieval_industry_stock_glue_cake)", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Wood Chisel", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Wood Chisel tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Wood Auger", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Wood Auger tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "medieval_workshop_book_press", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a wooden book press (@medieval_workshop_book_press)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MedievalIndustryToolmakingCarveStoneMortarAndPestle() => new()
	{
		Metadata = new("Medieval Industry / Toolmaking/carve stone mortar and pestle", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:medieval_tool_mortar_and_pestle"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Medieval.cs:153", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "carve stone mortar and pestle",
		Category = "Medieval Industry / Toolmaking",
		Blurb = "carve stone mortar and pestle",
		Action = "carve stone mortar and pestle",
		ActiveCraftItemSdesc = "an in-progress carve stone mortar and pestle craft",
		Gate = CatalogueRecipeGates.MedievalIndustryFoundationsMasonry252,
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
			new(30, "$0 inspect|inspects the stock and lay|lays out the work.", "$0 inspect|inspects the stock and lay|lays out the work.", ExertionLevel.Stasis, 0),
			new(45, "$0 shape|shapes and fit|fits the prepared pieces with $t1.", "$0 shape|shapes and fit|fits the prepared pieces with $t1.", ExertionLevel.Stasis, 0),
			new(45, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 botch|botches the finishing work and spoil|spoils the assembly.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityTagRecipeInput(Grams: 4000.0, MaterialTag: "Materials", PileTag: "Dressed Stone Block Commodity", CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("4 kilograms of a material tagged as Materials", ["piletag Dressed Stone Block Commodity"]), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Masonry Tool", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Masonry Tool tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "medieval_tool_mortar_and_pestle", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a stone mortar and pestle (@medieval_tool_mortar_and_pestle)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MedievalIndustryToolmakingForgeCurvedSutureNeedle() => new()
	{
		Metadata = new("Medieval Industry / Toolmaking/forge curved suture needle", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:medieval_tool_suture_needle"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Medieval.cs:153", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "forge curved suture needle",
		Category = "Medieval Industry / Toolmaking",
		Blurb = "forge curved suture needle",
		Action = "forge curved suture needle",
		ActiveCraftItemSdesc = "an in-progress curved suture needle craft",
		Gate = CatalogueRecipeGates.MedievalIndustryFoundationsBlacksmithing25,
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
			new(30, "$0 inspect|inspects the stock and lay|lays out the work.", "$0 inspect|inspects the stock and lay|lays out the work.", ExertionLevel.Stasis, 0),
			new(45, "$0 shape|shapes and fit|fits the prepared pieces with $t1, using $t2 and $t3.", "$0 shape|shapes and fit|fits the prepared pieces with $t1, using $t2 and $t3.", ExertionLevel.Stasis, 0),
			new(45, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 botch|botches the finishing work and spoil|spoils the assembly.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueItemRecipeInput(ItemStableReference: "medieval_industry_stock_iron_bar", Quantity: 1) { Compatibility = new("1x a wrought-iron bar (@medieval_industry_stock_iron_bar)", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Anvil", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Anvil tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Forge Tongs", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Forge Tongs tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "medieval_tool_suture_needle", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a curved suture needle (@medieval_tool_suture_needle)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MedievalIndustryToolmakingForgeBronzeSurgicalProbe() => new()
	{
		Metadata = new("Medieval Industry / Toolmaking/forge bronze surgical probe", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:medieval_tool_surgical_probe"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Medieval.cs:153", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "forge bronze surgical probe",
		Category = "Medieval Industry / Toolmaking",
		Blurb = "forge bronze surgical probe",
		Action = "forge bronze surgical probe",
		ActiveCraftItemSdesc = "an in-progress bronze surgical probe craft",
		Gate = CatalogueRecipeGates.MedievalIndustryFoundationsGoldsmithing25,
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
			new(30, "$0 inspect|inspects the stock and lay|lays out the work.", "$0 inspect|inspects the stock and lay|lays out the work.", ExertionLevel.Stasis, 0),
			new(45, "$0 shape|shapes and fit|fits the prepared pieces with $t1, using $t2 and $t3.", "$0 shape|shapes and fit|fits the prepared pieces with $t1, using $t2 and $t3.", ExertionLevel.Stasis, 0),
			new(45, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 botch|botches the finishing work and spoil|spoils the assembly.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueItemRecipeInput(ItemStableReference: "medieval_industry_stock_bronze_bar", Quantity: 1) { Compatibility = new("1x a bronze casting bar (@medieval_industry_stock_bronze_bar)", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Anvil", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Anvil tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Forge Tongs", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Forge Tongs tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "medieval_tool_surgical_probe", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a slender surgical probe (@medieval_tool_surgical_probe)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MedievalIndustryLightingLightSmithingForge() => new()
	{
		Metadata = new("Medieval Industry / Lighting/light smithing forge", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:medieval_workshop_lit_forge"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Medieval.cs:153", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "light smithing forge",
		Category = "Medieval Industry / Lighting",
		Blurb = "light smithing forge",
		Action = "light smithing forge",
		ActiveCraftItemSdesc = "an in-progress light smithing forge craft",
		Gate = CatalogueRecipeGates.MedievalIndustryFoundationsBlacksmithing10,
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
			new(20, "$0 prepare|prepares $i1 with a measured charcoal charge.", "$0 prepare|prepares $i1 with a measured charcoal charge.", ExertionLevel.Stasis, 0),
			new(30, "$0 coax|coaxes the fuel into a steady high heat, using $t1.", "$0 coax|coaxes the fuel into a steady high heat, using $t1.", ExertionLevel.Stasis, 0),
			new(20, "$0 set|sets $p1 into its working state.", "$0 leave|leaves only $f1 after the fire fails to take.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueItemRecipeInput(ItemStableReference: "medieval_workshop_forge", Quantity: 1) { Compatibility = new("1x an unlit smithing forge (@medieval_workshop_forge)", []), QualityWeight = 1 },
			new CatalogueCommodityRecipeInput(Grams: 2500.0, Material: "charcoal", PileTag: "Charcoal Fuel Commodity", CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2 kilograms 500 grams of charcoal", ["piletag Charcoal Fuel Commodity"]), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Charcoal Burning Tool", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Charcoal Burning Tool tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "medieval_workshop_lit_forge", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a lit smithing forge (@medieval_workshop_lit_forge)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
			new CatalogueUnusedRecipeProduct(InputIndex: 1, FractionRecovered: 1.0) { Compatibility = new("100.00% of 1x an unlit smithing forge (@medieval_workshop_forge) ($i1)", []), IsFailProduct = true, MaterialDefiningInputIndex = null },
		],
	};

	public static CatalogueRecipeDefinition MedievalIndustryLightingLightBloomerySmeltingFurnace() => new()
	{
		Metadata = new("Medieval Industry / Lighting/light bloomery smelting furnace", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:medieval_workshop_lit_smelting_furnace"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Medieval.cs:153", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "light bloomery smelting furnace",
		Category = "Medieval Industry / Lighting",
		Blurb = "light bloomery smelting furnace",
		Action = "light bloomery smelting furnace",
		ActiveCraftItemSdesc = "an in-progress light bloomery smelting furnace craft",
		Gate = CatalogueRecipeGates.MedievalIndustryFoundationsSmelting10,
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
			new(20, "$0 prepare|prepares $i1 with a measured charcoal charge.", "$0 prepare|prepares $i1 with a measured charcoal charge.", ExertionLevel.Stasis, 0),
			new(30, "$0 coax|coaxes the fuel into a steady high heat, using $t1 and $t2.", "$0 coax|coaxes the fuel into a steady high heat, using $t1 and $t2.", ExertionLevel.Stasis, 0),
			new(20, "$0 set|sets $p1 into its working state.", "$0 leave|leaves only $f1 after the fire fails to take.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueItemRecipeInput(ItemStableReference: "medieval_workshop_smelting_furnace", Quantity: 1) { Compatibility = new("1x an unlit bloomery furnace (@medieval_workshop_smelting_furnace)", []), QualityWeight = 1 },
			new CatalogueCommodityRecipeInput(Grams: 2500.0, Material: "charcoal", PileTag: "Charcoal Fuel Commodity", CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2 kilograms 500 grams of charcoal", ["piletag Charcoal Fuel Commodity"]), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Kiln Tool", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Kiln Tool tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Forge Tongs", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Forge Tongs tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "medieval_workshop_lit_smelting_furnace", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a lit bloomery furnace (@medieval_workshop_lit_smelting_furnace)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
			new CatalogueUnusedRecipeProduct(InputIndex: 1, FractionRecovered: 1.0) { Compatibility = new("100.00% of 1x an unlit bloomery furnace (@medieval_workshop_smelting_furnace) ($i1)", []), IsFailProduct = true, MaterialDefiningInputIndex = null },
		],
	};

	public static CatalogueRecipeDefinition MilitaryEquipmentMakeArcherSUtilityBelt() => new()
	{
		Metadata = new("Military Equipment/make archer's utility belt", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:medieval_military_archers_utility_belt"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Medieval.cs:893", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make archer's utility belt",
		Category = "Military Equipment",
		Blurb = "make an archer's utility belt",
		Action = "makeing an archer's utility belt",
		ActiveCraftItemSdesc = "an in-progress archer's utility belt craft",
		Gate = CatalogueRecipeGates.MedievalEquipmentCraftingWeaponcrafting10,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
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
			new CatalogueCommodityRecipeInput(Grams: 345.0, Material: "leather", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("345 grams of leather", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "medieval_military_archers_utility_belt", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x an archer's utility belt (@medieval_military_archers_utility_belt)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MilitaryEquipmentMakeArmingSwordWaster() => new()
	{
		Metadata = new("Military Equipment/make arming sword waster", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:medieval_military_arming_sword_waster"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Medieval.cs:893", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make arming sword waster",
		Category = "Military Equipment",
		Blurb = "make an arming sword waster",
		Action = "makeing an arming sword waster",
		ActiveCraftItemSdesc = "an in-progress arming sword waster craft",
		Gate = CatalogueRecipeGates.MedievalEquipmentCraftingWeaponcrafting10,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
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
			new CatalogueCommodityRecipeInput(Grams: 570.0, Material: "beech", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("570 grams of beech", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "medieval_military_arming_sword_waster", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x an arming sword waster (@medieval_military_arming_sword_waster)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MilitaryEquipmentMakeAshQuarterstaff() => new()
	{
		Metadata = new("Military Equipment/make ash quarterstaff", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:medieval_military_ash_quarterstaff"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Medieval.cs:893", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make ash quarterstaff",
		Category = "Military Equipment",
		Blurb = "make an ash quarterstaff",
		Action = "makeing an ash quarterstaff",
		ActiveCraftItemSdesc = "an in-progress ash quarterstaff craft",
		Gate = CatalogueRecipeGates.MedievalEquipmentCraftingWeaponcrafting10,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
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
			new CatalogueCommodityRecipeInput(Grams: 1088.0, Material: "ash", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1088 grams of ash", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "medieval_military_ash_quarterstaff", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x an ash quarterstaff (@medieval_military_ash_quarterstaff)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MilitaryEquipmentMakeSpangenhelmWithAnAventail() => new()
	{
		Metadata = new("Military Equipment/make spangenhelm with an aventail", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:medieval_military_aventail_spangenhelm"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Medieval.cs:893", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make spangenhelm with an aventail",
		Category = "Military Equipment",
		Blurb = "make a spangenhelm with an aventail",
		Action = "makeing a spangenhelm with an aventail",
		ActiveCraftItemSdesc = "an in-progress spangenhelm with an aventail craft",
		Gate = CatalogueRecipeGates.MedievalEquipmentCraftingWeaponcrafting10,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
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
			new CatalogueCommodityRecipeInput(Grams: 1950.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1950 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "medieval_military_aventail_spangenhelm", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a spangenhelm with an aventail (@medieval_military_aventail_spangenhelm)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MilitaryEquipmentMakeBackWornArrowQuiver() => new()
	{
		Metadata = new("Military Equipment/make back-worn arrow quiver", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:medieval_military_back_arrow_quiver"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Medieval.cs:893", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make back-worn arrow quiver",
		Category = "Military Equipment",
		Blurb = "make a back-worn arrow quiver",
		Action = "makeing a back-worn arrow quiver",
		ActiveCraftItemSdesc = "an in-progress back-worn arrow quiver craft",
		Gate = CatalogueRecipeGates.MedievalEquipmentCraftingWeaponcrafting10,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
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
			new CatalogueCommodityRecipeInput(Grams: 570.0, Material: "leather", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("570 grams of leather", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "medieval_military_back_arrow_quiver", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a back-worn arrow quiver (@medieval_military_back_arrow_quiver)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MilitaryEquipmentMakeBalancedArmingSword() => new()
	{
		Metadata = new("Military Equipment/make balanced arming sword", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:medieval_military_balanced_arming_sword"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Medieval.cs:893", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make balanced arming sword",
		Category = "Military Equipment",
		Blurb = "make a balanced arming sword",
		Action = "makeing a balanced arming sword",
		ActiveCraftItemSdesc = "an in-progress balanced arming sword craft",
		Gate = CatalogueRecipeGates.MedievalEquipmentCraftingWeaponcrafting10,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
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
			new CatalogueCommodityRecipeInput(Grams: 840.0, Material: "carbon steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("840 grams of carbon steel", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "medieval_military_balanced_arming_sword", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a balanced arming sword (@medieval_military_balanced_arming_sword)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MilitaryEquipmentMakeBalancedMilitaryPick() => new()
	{
		Metadata = new("Military Equipment/make balanced military pick", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:medieval_military_balanced_military_pick"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Medieval.cs:893", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make balanced military pick",
		Category = "Military Equipment",
		Blurb = "make a balanced military pick",
		Action = "makeing a balanced military pick",
		ActiveCraftItemSdesc = "an in-progress balanced military pick craft",
		Gate = CatalogueRecipeGates.MedievalEquipmentCraftingWeaponcrafting10,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
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
			new CatalogueCommodityRecipeInput(Grams: 675.0, Material: "carbon steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("675 grams of carbon steel", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "medieval_military_balanced_military_pick", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a balanced military pick (@medieval_military_balanced_military_pick)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MilitaryEquipmentMakeBalancedSteelDagger() => new()
	{
		Metadata = new("Military Equipment/make balanced steel dagger", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:medieval_military_balanced_steel_dagger"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Medieval.cs:893", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make balanced steel dagger",
		Category = "Military Equipment",
		Blurb = "make a balanced steel dagger",
		Action = "makeing a balanced steel dagger",
		ActiveCraftItemSdesc = "an in-progress balanced steel dagger craft",
		Gate = CatalogueRecipeGates.MedievalEquipmentCraftingWeaponcrafting10,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
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
			new CatalogueCommodityRecipeInput(Grams: 240.0, Material: "carbon steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("240 grams of carbon steel", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "medieval_military_balanced_steel_dagger", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a balanced steel dagger (@medieval_military_balanced_steel_dagger)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MilitaryEquipmentMakeBalancedThrowingAxe() => new()
	{
		Metadata = new("Military Equipment/make balanced throwing axe", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:medieval_military_balanced_throwing_axe"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Medieval.cs:893", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make balanced throwing axe",
		Category = "Military Equipment",
		Blurb = "make a balanced throwing axe",
		Action = "makeing a balanced throwing axe",
		ActiveCraftItemSdesc = "an in-progress balanced throwing axe craft",
		Gate = CatalogueRecipeGates.MedievalEquipmentCraftingWeaponcrafting10,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
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
			new CatalogueCommodityRecipeInput(Grams: 570.0, Material: "carbon steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("570 grams of carbon steel", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "medieval_military_balanced_throwing_axe", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a balanced throwing axe (@medieval_military_balanced_throwing_axe)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MilitaryEquipmentMakeBalancedThrowingKnife() => new()
	{
		Metadata = new("Military Equipment/make balanced throwing knife", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:medieval_military_balanced_throwing_knife"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Medieval.cs:893", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make balanced throwing knife",
		Category = "Military Equipment",
		Blurb = "make a balanced throwing knife",
		Action = "makeing a balanced throwing knife",
		ActiveCraftItemSdesc = "an in-progress balanced throwing knife craft",
		Gate = CatalogueRecipeGates.MedievalEquipmentCraftingWeaponcrafting10,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
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
			new CatalogueCommodityRecipeInput(Grams: 158.0, Material: "carbon steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("158 grams of carbon steel", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "medieval_military_balanced_throwing_knife", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a balanced throwing knife (@medieval_military_balanced_throwing_knife)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MilitaryEquipmentMakeBambooShortbow() => new()
	{
		Metadata = new("Military Equipment/make bamboo shortbow", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:medieval_military_bamboo_shortbow"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Medieval.cs:893", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make bamboo shortbow",
		Category = "Military Equipment",
		Blurb = "make a bamboo shortbow",
		Action = "makeing a bamboo shortbow",
		ActiveCraftItemSdesc = "an in-progress bamboo shortbow craft",
		Gate = CatalogueRecipeGates.MedievalEquipmentCraftingWeaponcrafting10,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
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
			new CatalogueCommodityRecipeInput(Grams: 390.0, Material: "bamboo", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("390 grams of bamboo", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "medieval_military_bamboo_shortbow", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a bamboo shortbow (@medieval_military_bamboo_shortbow)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MilitaryEquipmentMakeBambooStaffSling() => new()
	{
		Metadata = new("Military Equipment/make bamboo staff sling", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:medieval_military_bamboo_staff_sling"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Medieval.cs:893", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make bamboo staff sling",
		Category = "Military Equipment",
		Blurb = "make a bamboo staff sling",
		Action = "makeing a bamboo staff sling",
		ActiveCraftItemSdesc = "an in-progress bamboo staff sling craft",
		Gate = CatalogueRecipeGates.MedievalEquipmentCraftingWeaponcrafting10,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
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
			new CatalogueCommodityRecipeInput(Grams: 713.0, Material: "bamboo", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("713 grams of bamboo", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "medieval_military_bamboo_staff_sling", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a bamboo staff sling (@medieval_military_bamboo_staff_sling)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MilitaryEquipmentMakeBambooWarDart() => new()
	{
		Metadata = new("Military Equipment/make bamboo war dart", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:medieval_military_bamboo_war_dart"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Medieval.cs:893", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make bamboo war dart",
		Category = "Military Equipment",
		Blurb = "make a bamboo war dart",
		Action = "makeing a bamboo war dart",
		ActiveCraftItemSdesc = "an in-progress bamboo war dart craft",
		Gate = CatalogueRecipeGates.MedievalEquipmentCraftingWeaponcrafting10,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
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
			new CatalogueCommodityRecipeInput(Grams: 390.0, Material: "bamboo", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("390 grams of bamboo", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "medieval_military_bamboo_war_dart", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a bamboo war dart (@medieval_military_bamboo_war_dart)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MilitaryEquipmentMakeBarbedThrowingSpear() => new()
	{
		Metadata = new("Military Equipment/make barbed throwing spear", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:medieval_military_barbed_throwing_spear"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Medieval.cs:893", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make barbed throwing spear",
		Category = "Military Equipment",
		Blurb = "make a barbed throwing spear",
		Action = "makeing a barbed throwing spear",
		ActiveCraftItemSdesc = "an in-progress barbed throwing spear craft",
		Gate = CatalogueRecipeGates.MedievalEquipmentCraftingWeaponcrafting10,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
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
			new CatalogueCommodityRecipeInput(Grams: 735.0, Material: "ash", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("735 grams of ash", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "medieval_military_barbed_throwing_spear", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a barbed throwing spear (@medieval_military_barbed_throwing_spear)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MilitaryEquipmentMakeBardingTrestle() => new()
	{
		Metadata = new("Military Equipment/make barding trestle", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:medieval_military_barding_trestle"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Medieval.cs:893", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make barding trestle",
		Category = "Military Equipment",
		Blurb = "make a barding trestle",
		Action = "makeing a barding trestle",
		ActiveCraftItemSdesc = "an in-progress barding trestle craft",
		Gate = CatalogueRecipeGates.MedievalEquipmentCraftingWeaponcrafting10,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
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
			new CatalogueCommodityRecipeInput(Grams: 24000.0, Material: "oak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("24000 grams of oak", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "medieval_military_barding_trestle", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a barding trestle (@medieval_military_barding_trestle)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MilitaryEquipmentMakeBarrelLidShield() => new()
	{
		Metadata = new("Military Equipment/make barrel-lid shield", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:medieval_military_barrel_lid_shield"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Medieval.cs:893", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make barrel-lid shield",
		Category = "Military Equipment",
		Blurb = "make a barrel-lid shield",
		Action = "makeing a barrel-lid shield",
		ActiveCraftItemSdesc = "an in-progress barrel-lid shield craft",
		Gate = CatalogueRecipeGates.MedievalEquipmentCraftingWeaponcrafting10,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
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
			new CatalogueCommodityRecipeInput(Grams: 1650.0, Material: "oak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1650 grams of oak", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "medieval_military_barrel_lid_shield", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a barrel-lid shield (@medieval_military_barrel_lid_shield)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MilitaryEquipmentMakeBeardedThrowingAxe() => new()
	{
		Metadata = new("Military Equipment/make bearded throwing axe", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:medieval_military_bearded_throwing_axe"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Medieval.cs:893", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make bearded throwing axe",
		Category = "Military Equipment",
		Blurb = "make a bearded throwing axe",
		Action = "makeing a bearded throwing axe",
		ActiveCraftItemSdesc = "an in-progress bearded throwing axe craft",
		Gate = CatalogueRecipeGates.MedievalEquipmentCraftingWeaponcrafting10,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
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
			new CatalogueCommodityRecipeInput(Grams: 615.0, Material: "carbon steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("615 grams of carbon steel", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "medieval_military_bearded_throwing_axe", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a bearded throwing axe (@medieval_military_bearded_throwing_axe)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MilitaryEquipmentMakeStrappedBellyPlate() => new()
	{
		Metadata = new("Military Equipment/make strapped belly plate", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:medieval_military_belly_plate"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Medieval.cs:893", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make strapped belly plate",
		Category = "Military Equipment",
		Blurb = "make a strapped belly plate",
		Action = "makeing a strapped belly plate",
		ActiveCraftItemSdesc = "an in-progress strapped belly plate craft",
		Gate = CatalogueRecipeGates.MedievalEquipmentCraftingWeaponcrafting10,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
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
			new CatalogueCommodityRecipeInput(Grams: 1275.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1275 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "medieval_military_belly_plate", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a strapped belly plate (@medieval_military_belly_plate)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MilitaryEquipmentMakeBeltHungBoltCase() => new()
	{
		Metadata = new("Military Equipment/make belt-hung bolt case", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:medieval_military_belt_bolt_case"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Medieval.cs:893", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make belt-hung bolt case",
		Category = "Military Equipment",
		Blurb = "make a belt-hung bolt case",
		Action = "makeing a belt-hung bolt case",
		ActiveCraftItemSdesc = "an in-progress belt-hung bolt case craft",
		Gate = CatalogueRecipeGates.MedievalEquipmentCraftingWeaponcrafting10,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
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
			new CatalogueCommodityRecipeInput(Grams: 330.0, Material: "cow leather", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("330 grams of cow leather", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "medieval_military_belt_bolt_case", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a belt-hung bolt case (@medieval_military_belt_bolt_case)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MilitaryEquipmentMakeLeatherCoveredBlackjack() => new()
	{
		Metadata = new("Military Equipment/make leather-covered blackjack", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:medieval_military_blackjack_leather_wrapped"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Medieval.cs:893", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make leather-covered blackjack",
		Category = "Military Equipment",
		Blurb = "make a leather-covered blackjack",
		Action = "makeing a leather-covered blackjack",
		ActiveCraftItemSdesc = "an in-progress leather-covered blackjack craft",
		Gate = CatalogueRecipeGates.MedievalEquipmentCraftingWeaponcrafting10,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
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
			new CatalogueCommodityRecipeInput(Grams: 338.0, Material: "lead", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("338 grams of lead", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "medieval_military_blackjack_leather_wrapped", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a leather-covered blackjack (@medieval_military_blackjack_leather_wrapped)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MilitaryEquipmentMakeLuggedBoarSpear() => new()
	{
		Metadata = new("Military Equipment/make lugged boar spear", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:medieval_military_boar_spear"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Medieval.cs:893", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make lugged boar spear",
		Category = "Military Equipment",
		Blurb = "make a lugged boar spear",
		Action = "makeing a lugged boar spear",
		ActiveCraftItemSdesc = "an in-progress lugged boar spear craft",
		Gate = CatalogueRecipeGates.MedievalEquipmentCraftingWeaponcrafting10,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
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
			new CatalogueCommodityRecipeInput(Grams: 1425.0, Material: "carbon steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1425 grams of carbon steel", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "medieval_military_boar_spear", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a lugged boar spear (@medieval_military_boar_spear)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MilitaryEquipmentMakeBodkinArrow() => new()
	{
		Metadata = new("Military Equipment/make bodkin arrow", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:medieval_military_bodkin_arrow"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Medieval.cs:893", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make bodkin arrow",
		Category = "Military Equipment",
		Blurb = "make a bodkin arrow",
		Action = "makeing a bodkin arrow",
		ActiveCraftItemSdesc = "an in-progress bodkin arrow craft",
		Gate = CatalogueRecipeGates.MedievalEquipmentCraftingWeaponcrafting10,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
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
			new CatalogueCommodityRecipeInput(Grams: 29.0, Material: "ash", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("29 grams of ash", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "medieval_military_bodkin_arrow", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a bodkin arrow (@medieval_military_bodkin_arrow)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MilitaryEquipmentMakeBodkinBolt() => new()
	{
		Metadata = new("Military Equipment/make bodkin bolt", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:medieval_military_bodkin_bolt"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Medieval.cs:893", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make bodkin bolt",
		Category = "Military Equipment",
		Blurb = "make a bodkin bolt",
		Action = "makeing a bodkin bolt",
		ActiveCraftItemSdesc = "an in-progress bodkin bolt craft",
		Gate = CatalogueRecipeGates.MedievalEquipmentCraftingWeaponcrafting10,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
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
			new CatalogueCommodityRecipeInput(Grams: 42.0, Material: "ash", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("42 grams of ash", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "medieval_military_bodkin_bolt", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a bodkin bolt (@medieval_military_bodkin_bolt)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MilitaryEquipmentMakeBoiledLeatherBracers() => new()
	{
		Metadata = new("Military Equipment/make boiled leather bracers", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:medieval_military_boiled_leather_bracers"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Medieval.cs:893", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make boiled leather bracers",
		Category = "Military Equipment",
		Blurb = "make a pair of boiled leather bracers",
		Action = "makeing a pair of boiled leather bracers",
		ActiveCraftItemSdesc = "an in-progress pair of boiled leather bracers craft",
		Gate = CatalogueRecipeGates.MedievalEquipmentCraftingWeaponcrafting10,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
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
			new CatalogueCommodityRecipeInput(Grams: 420.0, Material: "leather", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("420 grams of leather", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "medieval_military_boiled_leather_bracers", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a pair of boiled leather bracers (@medieval_military_boiled_leather_bracers)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MilitaryEquipmentMakeBoiledLeatherCuirass() => new()
	{
		Metadata = new("Military Equipment/make boiled leather cuirass", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:medieval_military_boiled_leather_cuirass"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Medieval.cs:893", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make boiled leather cuirass",
		Category = "Military Equipment",
		Blurb = "make a boiled leather cuirass",
		Action = "makeing a boiled leather cuirass",
		ActiveCraftItemSdesc = "an in-progress boiled leather cuirass craft",
		Gate = CatalogueRecipeGates.MedievalEquipmentCraftingWeaponcrafting10,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
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
			new CatalogueCommodityRecipeInput(Grams: 2100.0, Material: "leather", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2100 grams of leather", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "medieval_military_boiled_leather_cuirass", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a boiled leather cuirass (@medieval_military_boiled_leather_cuirass)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MilitaryEquipmentMakeColourBorderedSurcoat() => new()
	{
		Metadata = new("Military Equipment/make $colour bordered surcoat", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:medieval_military_bordered_surcoat"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Medieval.cs:893", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make $colour bordered surcoat",
		Category = "Military Equipment",
		Blurb = "make a $colour bordered surcoat",
		Action = "makeing a $colour bordered surcoat",
		ActiveCraftItemSdesc = "an in-progress $colour bordered surcoat craft",
		Gate = CatalogueRecipeGates.MedievalEquipmentCraftingWeaponcrafting10,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
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
			new CatalogueCommodityRecipeInput(Grams: 570.0, Material: "wool", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("570 grams of wool", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "medieval_military_bordered_surcoat", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a $colour bordered surcoat (@medieval_military_bordered_surcoat)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MilitaryEquipmentMakeBowRack() => new()
	{
		Metadata = new("Military Equipment/make bow rack", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:medieval_military_bow_rack"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Medieval.cs:893", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make bow rack",
		Category = "Military Equipment",
		Blurb = "make a bow rack",
		Action = "makeing a bow rack",
		ActiveCraftItemSdesc = "an in-progress bow rack craft",
		Gate = CatalogueRecipeGates.MedievalEquipmentCraftingWeaponcrafting10,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
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
			new CatalogueCommodityRecipeInput(Grams: 12000.0, Material: "ash", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("12000 grams of ash", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "medieval_military_bow_rack", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a bow rack (@medieval_military_bow_rack)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MilitaryEquipmentMakeBraidedLinenSling() => new()
	{
		Metadata = new("Military Equipment/make braided linen sling", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:medieval_military_braided_linen_sling"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Medieval.cs:893", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make braided linen sling",
		Category = "Military Equipment",
		Blurb = "make a braided linen sling",
		Action = "makeing a braided linen sling",
		ActiveCraftItemSdesc = "an in-progress braided linen sling craft",
		Gate = CatalogueRecipeGates.MedievalEquipmentCraftingWeaponcrafting10,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
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
			new CatalogueCommodityRecipeInput(Grams: 53.0, Material: "linen", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("53 grams of linen", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "medieval_military_braided_linen_sling", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a braided linen sling (@medieval_military_braided_linen_sling)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MilitaryEquipmentMakeBroadheadArrow() => new()
	{
		Metadata = new("Military Equipment/make broadhead arrow", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:medieval_military_broadhead_arrow"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Medieval.cs:893", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make broadhead arrow",
		Category = "Military Equipment",
		Blurb = "make a broadhead arrow",
		Action = "makeing a broadhead arrow",
		ActiveCraftItemSdesc = "an in-progress broadhead arrow craft",
		Gate = CatalogueRecipeGates.MedievalEquipmentCraftingWeaponcrafting10,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
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
			new CatalogueCommodityRecipeInput(Grams: 32.0, Material: "ash", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("32 grams of ash", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "medieval_military_broadhead_arrow", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a broadhead arrow (@medieval_military_broadhead_arrow)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MilitaryEquipmentMakeBroadheadBolt() => new()
	{
		Metadata = new("Military Equipment/make broadhead bolt", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:medieval_military_broadhead_bolt"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Medieval.cs:893", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make broadhead bolt",
		Category = "Military Equipment",
		Blurb = "make a broadhead bolt",
		Action = "makeing a broadhead bolt",
		ActiveCraftItemSdesc = "an in-progress broadhead bolt craft",
		Gate = CatalogueRecipeGates.MedievalEquipmentCraftingWeaponcrafting10,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
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
			new CatalogueCommodityRecipeInput(Grams: 44.0, Material: "ash", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("44 grams of ash", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "medieval_military_broadhead_bolt", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a broadhead bolt (@medieval_military_broadhead_bolt)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MilitaryEquipmentMakeBroadBeardedAxe() => new()
	{
		Metadata = new("Military Equipment/make broad bearded axe", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:medieval_military_broad_bearded_axe"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Medieval.cs:893", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make broad bearded axe",
		Category = "Military Equipment",
		Blurb = "make a broad bearded axe",
		Action = "makeing a broad bearded axe",
		ActiveCraftItemSdesc = "an in-progress broad bearded axe craft",
		Gate = CatalogueRecipeGates.MedievalEquipmentCraftingWeaponcrafting10,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
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
			new CatalogueCommodityRecipeInput(Grams: 788.0, Material: "carbon steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("788 grams of carbon steel", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "medieval_military_broad_bearded_axe", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a broad bearded axe (@medieval_military_broad_bearded_axe)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MilitaryEquipmentMakeBroadSteelPoleblade() => new()
	{
		Metadata = new("Military Equipment/make broad steel poleblade", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:medieval_military_broad_poleblade"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Medieval.cs:893", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make broad steel poleblade",
		Category = "Military Equipment",
		Blurb = "make a broad steel poleblade",
		Action = "makeing a broad steel poleblade",
		ActiveCraftItemSdesc = "an in-progress broad steel poleblade craft",
		Gate = CatalogueRecipeGates.MedievalEquipmentCraftingWeaponcrafting10,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
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
			new CatalogueCommodityRecipeInput(Grams: 1538.0, Material: "carbon steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1538 grams of carbon steel", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "medieval_military_broad_poleblade", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a broad steel poleblade (@medieval_military_broad_poleblade)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};
}
