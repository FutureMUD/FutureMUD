#nullable enable

using MudSharp.Body;
using MudSharp.GameItems.Inventory.Plans;
using MudSharp.RPG.Checks;

namespace DatabaseSeeder.Seeders;

public static partial class Recipes08Stock
{
	public static CatalogueRecipeDefinition ResidualStockPrepareCarvedWoodenMinbar() => new()
	{
		Metadata = new("Residual Stock/prepare carved wooden minbar", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:medieval_islamic_wooden_minbar_steps"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Medieval.cs:893", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "prepare carved wooden minbar",
		Category = "Residual Stock",
		Blurb = "prepare a carved wooden minbar",
		Action = "prepareing a carved wooden minbar",
		ActiveCraftItemSdesc = "an in-progress carved wooden minbar craft",
		Gate = CatalogueRecipeGates.MedievalResidualStockCraftingCarpentry10,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
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
			new CatalogueCommodityRecipeInput(Grams: 27000.0, Material: "walnut", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("27000 grams of walnut", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "medieval_islamic_wooden_minbar_steps", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a carved wooden minbar (@medieval_islamic_wooden_minbar_steps)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ResidualStockPreparePlainColourCottonRobe() => new()
	{
		Metadata = new("Residual Stock/prepare plain $colour cotton robe", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:medieval_jain_white_ascetic_robe"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Medieval.cs:893", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "prepare plain $colour cotton robe",
		Category = "Residual Stock",
		Blurb = "prepare a plain $colour cotton robe",
		Action = "prepareing a plain $colour cotton robe",
		ActiveCraftItemSdesc = "an in-progress plain $colour cotton robe craft",
		Gate = CatalogueRecipeGates.MedievalResidualStockCraftingCarpentry10,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
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
			new CatalogueCommodityRecipeInput(Grams: 315.0, Material: "cotton", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("315 grams of cotton", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "medieval_jain_white_ascetic_robe", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a plain $colour cotton robe (@medieval_jain_white_ascetic_robe)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ResidualStockPrepareColourCottonShoulderWrap() => new()
	{
		Metadata = new("Residual Stock/prepare $colour cotton shoulder wrap", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:medieval_jain_white_shoulder_wrap"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Medieval.cs:893", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "prepare $colour cotton shoulder wrap",
		Category = "Residual Stock",
		Blurb = "prepare a $colour cotton shoulder wrap",
		Action = "prepareing a $colour cotton shoulder wrap",
		ActiveCraftItemSdesc = "an in-progress $colour cotton shoulder wrap craft",
		Gate = CatalogueRecipeGates.MedievalResidualStockCraftingCarpentry10,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
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
			new CatalogueCommodityRecipeInput(Grams: 120.0, Material: "cotton", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("120 grams of cotton", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "medieval_jain_white_shoulder_wrap", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a $colour cotton shoulder wrap (@medieval_jain_white_shoulder_wrap)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ResidualStockPrepareBlackEboshiCap() => new()
	{
		Metadata = new("Residual Stock/prepare black eboshi cap", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:medieval_japanese_eboshi"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Medieval.cs:893", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "prepare black eboshi cap",
		Category = "Residual Stock",
		Blurb = "prepare a black eboshi cap",
		Action = "prepareing a black eboshi cap",
		ActiveCraftItemSdesc = "an in-progress black eboshi cap craft",
		Gate = CatalogueRecipeGates.MedievalResidualStockCraftingCarpentry10,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
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
			new CatalogueCommodityRecipeInput(Grams: 60.0, Material: "silk", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("60 grams of silk", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "medieval_japanese_eboshi", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a black eboshi cap (@medieval_japanese_eboshi)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ResidualStockPrepareColourHakama() => new()
	{
		Metadata = new("Residual Stock/prepare $colour hakama", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:medieval_japanese_hakama"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Medieval.cs:893", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "prepare $colour hakama",
		Category = "Residual Stock",
		Blurb = "prepare a pair of $colour hakama",
		Action = "prepareing a pair of $colour hakama",
		ActiveCraftItemSdesc = "an in-progress pair of $colour hakama craft",
		Gate = CatalogueRecipeGates.MedievalResidualStockCraftingCarpentry10,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
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
			new CatalogueCommodityRecipeInput(Grams: 315.0, Material: "silk", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("315 grams of silk", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "medieval_japanese_hakama", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a pair of $colour hakama (@medieval_japanese_hakama)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ResidualStockPrepareColourLinenHeadcloth() => new()
	{
		Metadata = new("Residual Stock/prepare $colour linen headcloth", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:medieval_japanese_headcloth"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Medieval.cs:893", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "prepare $colour linen headcloth",
		Category = "Residual Stock",
		Blurb = "prepare a $colour linen headcloth",
		Action = "prepareing a $colour linen headcloth",
		ActiveCraftItemSdesc = "an in-progress $colour linen headcloth craft",
		Gate = CatalogueRecipeGates.MedievalResidualStockCraftingCarpentry10,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
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
			new CatalogueCommodityRecipeInput(Grams: 68.0, Material: "linen", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("68 grams of linen", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "medieval_japanese_headcloth", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a $colour linen headcloth (@medieval_japanese_headcloth)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ResidualStockPrepareHempSandals() => new()
	{
		Metadata = new("Residual Stock/prepare hemp sandals", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:medieval_japanese_hemp_sandals"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Medieval.cs:893", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "prepare hemp sandals",
		Category = "Residual Stock",
		Blurb = "prepare a pair of hemp sandals",
		Action = "prepareing a pair of hemp sandals",
		ActiveCraftItemSdesc = "an in-progress pair of hemp sandals craft",
		Gate = CatalogueRecipeGates.MedievalResidualStockCraftingCarpentry10,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
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
			new CatalogueCommodityRecipeInput(Grams: 135.0, Material: "hemp", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("135 grams of hemp", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "medieval_japanese_hemp_sandals", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a pair of hemp sandals (@medieval_japanese_hemp_sandals)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ResidualStockPrepareHempWorkApron() => new()
	{
		Metadata = new("Residual Stock/prepare hemp work apron", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:medieval_japanese_hemp_work_apron"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Medieval.cs:893", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "prepare hemp work apron",
		Category = "Residual Stock",
		Blurb = "prepare a hemp work apron",
		Action = "prepareing a hemp work apron",
		ActiveCraftItemSdesc = "an in-progress hemp work apron craft",
		Gate = CatalogueRecipeGates.MedievalResidualStockCraftingCarpentry10,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
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
			new CatalogueCommodityRecipeInput(Grams: 128.0, Material: "hemp", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("128 grams of hemp", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "medieval_japanese_hemp_work_apron", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a hemp work apron (@medieval_japanese_hemp_work_apron)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ResidualStockPrepareColourHitatareRobe() => new()
	{
		Metadata = new("Residual Stock/prepare $colour hitatare robe", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:medieval_japanese_hitatare"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Medieval.cs:893", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "prepare $colour hitatare robe",
		Category = "Residual Stock",
		Blurb = "prepare a $colour hitatare robe",
		Action = "prepareing a $colour hitatare robe",
		ActiveCraftItemSdesc = "an in-progress $colour hitatare robe craft",
		Gate = CatalogueRecipeGates.MedievalResidualStockCraftingCarpentry10,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
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
			new CatalogueCommodityRecipeInput(Grams: 465.0, Material: "silk", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("465 grams of silk", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "medieval_japanese_hitatare", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a $colour hitatare robe (@medieval_japanese_hitatare)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ResidualStockPrepareFineColourKariginuRobe() => new()
	{
		Metadata = new("Residual Stock/prepare fine $colour kariginu robe", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:medieval_japanese_kariginu"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Medieval.cs:893", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "prepare fine $colour kariginu robe",
		Category = "Residual Stock",
		Blurb = "prepare a fine $colour kariginu robe",
		Action = "prepareing a fine $colour kariginu robe",
		ActiveCraftItemSdesc = "an in-progress fine $colour kariginu robe craft",
		Gate = CatalogueRecipeGates.MedievalResidualStockCraftingCarpentry10,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
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
			new CatalogueCommodityRecipeInput(Grams: 465.0, Material: "silk", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("465 grams of silk", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "medieval_japanese_kariginu", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a fine $colour kariginu robe (@medieval_japanese_kariginu)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ResidualStockPrepareColourKosodeRobe() => new()
	{
		Metadata = new("Residual Stock/prepare $colour kosode robe", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:medieval_japanese_kosode"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Medieval.cs:893", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "prepare $colour kosode robe",
		Category = "Residual Stock",
		Blurb = "prepare a $colour kosode robe",
		Action = "prepareing a $colour kosode robe",
		ActiveCraftItemSdesc = "an in-progress $colour kosode robe craft",
		Gate = CatalogueRecipeGates.MedievalResidualStockCraftingCarpentry10,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
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
			new CatalogueCommodityRecipeInput(Grams: 420.0, Material: "silk", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("420 grams of silk", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "medieval_japanese_kosode", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a $colour kosode robe (@medieval_japanese_kosode)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ResidualStockPrepareColourSuikanRobe() => new()
	{
		Metadata = new("Residual Stock/prepare $colour suikan robe", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:medieval_japanese_suikan"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Medieval.cs:893", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "prepare $colour suikan robe",
		Category = "Residual Stock",
		Blurb = "prepare a $colour suikan robe",
		Action = "prepareing a $colour suikan robe",
		ActiveCraftItemSdesc = "an in-progress $colour suikan robe craft",
		Gate = CatalogueRecipeGates.MedievalResidualStockCraftingCarpentry10,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
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
			new CatalogueCommodityRecipeInput(Grams: 450.0, Material: "silk", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("450 grams of silk", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "medieval_japanese_suikan", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a $colour suikan robe (@medieval_japanese_suikan)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ResidualStockPrepareFineColourUchigiRobe() => new()
	{
		Metadata = new("Residual Stock/prepare fine $colour uchigi robe", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:medieval_japanese_uchigi"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Medieval.cs:893", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "prepare fine $colour uchigi robe",
		Category = "Residual Stock",
		Blurb = "prepare a fine $colour uchigi robe",
		Action = "prepareing a fine $colour uchigi robe",
		ActiveCraftItemSdesc = "an in-progress fine $colour uchigi robe craft",
		Gate = CatalogueRecipeGates.MedievalResidualStockCraftingCarpentry10,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
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
			new CatalogueCommodityRecipeInput(Grams: 480.0, Material: "silk", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("480 grams of silk", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "medieval_japanese_uchigi", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a fine $colour uchigi robe (@medieval_japanese_uchigi)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ResidualStockPrepareColourWorkHakama() => new()
	{
		Metadata = new("Residual Stock/prepare $colour work hakama", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:medieval_japanese_work_hakama"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Medieval.cs:893", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "prepare $colour work hakama",
		Category = "Residual Stock",
		Blurb = "prepare a pair of $colour work hakama",
		Action = "prepareing a pair of $colour work hakama",
		ActiveCraftItemSdesc = "an in-progress pair of $colour work hakama craft",
		Gate = CatalogueRecipeGates.MedievalResidualStockCraftingCarpentry10,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
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
			new CatalogueCommodityRecipeInput(Grams: 285.0, Material: "hemp", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("285 grams of hemp", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "medieval_japanese_work_hakama", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a pair of $colour work hakama (@medieval_japanese_work_hakama)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ResidualStockPreparePlainColourWorkKosode() => new()
	{
		Metadata = new("Residual Stock/prepare plain $colour work kosode", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:medieval_japanese_work_kosode"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Medieval.cs:893", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "prepare plain $colour work kosode",
		Category = "Residual Stock",
		Blurb = "prepare a plain $colour work kosode",
		Action = "prepareing a plain $colour work kosode",
		ActiveCraftItemSdesc = "an in-progress plain $colour work kosode craft",
		Gate = CatalogueRecipeGates.MedievalResidualStockCraftingCarpentry10,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
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
			new CatalogueCommodityRecipeInput(Grams: 360.0, Material: "ramie", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("360 grams of ramie", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "medieval_japanese_work_kosode", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a plain $colour work kosode (@medieval_japanese_work_kosode)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ResidualStockPrepareColourYumakiWaistWrap() => new()
	{
		Metadata = new("Residual Stock/prepare $colour yumaki waist wrap", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:medieval_japanese_yumaki_wrap"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Medieval.cs:893", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "prepare $colour yumaki waist wrap",
		Category = "Residual Stock",
		Blurb = "prepare a $colour yumaki waist wrap",
		Action = "prepareing a $colour yumaki waist wrap",
		ActiveCraftItemSdesc = "an in-progress $colour yumaki waist wrap craft",
		Gate = CatalogueRecipeGates.MedievalResidualStockCraftingCarpentry10,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
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
			new CatalogueCommodityRecipeInput(Grams: 143.0, Material: "hemp", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("143 grams of hemp", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "medieval_japanese_yumaki_wrap", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a $colour yumaki waist wrap (@medieval_japanese_yumaki_wrap)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ResidualStockPrepareBimahReadingTable() => new()
	{
		Metadata = new("Residual Stock/prepare bimah reading table", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:medieval_jewish_bimah_table"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Medieval.cs:893", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "prepare bimah reading table",
		Category = "Residual Stock",
		Blurb = "prepare a bimah reading table",
		Action = "prepareing a bimah reading table",
		ActiveCraftItemSdesc = "an in-progress bimah reading table craft",
		Gate = CatalogueRecipeGates.MedievalResidualStockCraftingCarpentry10,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
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
			new CatalogueCommodityRecipeInput(Grams: 19500.0, Material: "oak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("19500 grams of oak", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "medieval_jewish_bimah_table", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a bimah reading table (@medieval_jewish_bimah_table)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ResidualStockPrepareBronzeSevenBranchedLampstand() => new()
	{
		Metadata = new("Residual Stock/prepare bronze seven-branched lampstand", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:medieval_jewish_bronze_lampstand"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Medieval.cs:893", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "prepare bronze seven-branched lampstand",
		Category = "Residual Stock",
		Blurb = "prepare a bronze seven-branched lampstand",
		Action = "prepareing a bronze seven-branched lampstand",
		ActiveCraftItemSdesc = "an in-progress bronze seven-branched lampstand craft",
		Gate = CatalogueRecipeGates.MedievalResidualStockCraftingCarpentry10,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
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
			new CatalogueCommodityRecipeInput(Grams: 3900.0, Material: "bronze", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3900 grams of bronze", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "medieval_jewish_bronze_lampstand", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a bronze seven-branched lampstand (@medieval_jewish_bronze_lampstand)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ResidualStockPrepareGenizahChest() => new()
	{
		Metadata = new("Residual Stock/prepare genizah chest", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:medieval_jewish_genizah_chest"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Medieval.cs:893", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "prepare genizah chest",
		Category = "Residual Stock",
		Blurb = "prepare a genizah chest",
		Action = "prepareing a genizah chest",
		ActiveCraftItemSdesc = "an in-progress genizah chest craft",
		Gate = CatalogueRecipeGates.MedievalResidualStockCraftingCarpentry10,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
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
			new CatalogueCommodityRecipeInput(Grams: 13875.0, Material: "cedar", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("13875 grams of cedar", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "medieval_jewish_genizah_chest", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a genizah chest (@medieval_jewish_genizah_chest)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ResidualStockPrepareMemorialBeeswaxCandle() => new()
	{
		Metadata = new("Residual Stock/prepare memorial beeswax candle", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:medieval_jewish_memorial_candle"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Medieval.cs:893", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "prepare memorial beeswax candle",
		Category = "Residual Stock",
		Blurb = "prepare a memorial beeswax candle",
		Action = "prepareing a memorial beeswax candle",
		ActiveCraftItemSdesc = "an in-progress memorial beeswax candle craft",
		Gate = CatalogueRecipeGates.MedievalResidualStockCraftingCarpentry10,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
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
			new CatalogueCommodityRecipeInput(Grams: 135.0, Material: "beeswax", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("135 grams of beeswax", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "medieval_jewish_memorial_candle", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a memorial beeswax candle (@medieval_jewish_memorial_candle)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ResidualStockPrepareNerTamidLamp() => new()
	{
		Metadata = new("Residual Stock/prepare ner tamid lamp", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:medieval_jewish_ner_tamid_lamp"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Medieval.cs:893", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "prepare ner tamid lamp",
		Category = "Residual Stock",
		Blurb = "prepare a ner tamid lamp",
		Action = "prepareing a ner tamid lamp",
		ActiveCraftItemSdesc = "an in-progress ner tamid lamp craft",
		Gate = CatalogueRecipeGates.MedievalResidualStockCraftingCarpentry10,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
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
			new CatalogueCommodityRecipeInput(Grams: 900.0, Material: "brass", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("900 grams of brass", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "medieval_jewish_ner_tamid_lamp", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a ner tamid lamp (@medieval_jewish_ner_tamid_lamp)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ResidualStockPrepareSynagogueReadingLectern() => new()
	{
		Metadata = new("Residual Stock/prepare synagogue reading lectern", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:medieval_jewish_reading_lectern"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Medieval.cs:893", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "prepare synagogue reading lectern",
		Category = "Residual Stock",
		Blurb = "prepare a synagogue reading lectern",
		Action = "prepareing a synagogue reading lectern",
		ActiveCraftItemSdesc = "an in-progress synagogue reading lectern craft",
		Gate = CatalogueRecipeGates.MedievalResidualStockCraftingCarpentry10,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
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
			new CatalogueCommodityRecipeInput(Grams: 5400.0, Material: "walnut", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("5400 grams of walnut", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "medieval_jewish_reading_lectern", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a synagogue reading lectern (@medieval_jewish_reading_lectern)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ResidualStockPrepareDarkWoolScholarSRobe() => new()
	{
		Metadata = new("Residual Stock/prepare dark wool scholar's robe", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:medieval_jewish_scholars_robe"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Medieval.cs:893", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "prepare dark wool scholar's robe",
		Category = "Residual Stock",
		Blurb = "prepare a dark wool scholar's robe",
		Action = "prepareing a dark wool scholar's robe",
		ActiveCraftItemSdesc = "an in-progress dark wool scholar's robe craft",
		Gate = CatalogueRecipeGates.MedievalResidualStockCraftingCarpentry10,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
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
			new CatalogueCommodityRecipeInput(Grams: 615.0, Material: "wool", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("615 grams of wool", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "medieval_jewish_scholars_robe", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a dark wool scholar's robe (@medieval_jewish_scholars_robe)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ResidualStockPrepareTorahScrollCabinet() => new()
	{
		Metadata = new("Residual Stock/prepare Torah scroll cabinet", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:medieval_jewish_scroll_cabinet"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Medieval.cs:893", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "prepare Torah scroll cabinet",
		Category = "Residual Stock",
		Blurb = "prepare a Torah scroll cabinet",
		Action = "prepareing a Torah scroll cabinet",
		ActiveCraftItemSdesc = "an in-progress Torah scroll cabinet craft",
		Gate = CatalogueRecipeGates.MedievalResidualStockCraftingCarpentry10,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
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
			new CatalogueCommodityRecipeInput(Grams: 18000.0, Material: "cedar", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("18000 grams of cedar", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "medieval_jewish_scroll_cabinet", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a Torah scroll cabinet (@medieval_jewish_scroll_cabinet)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ResidualStockPrepareWoodenScrollRest() => new()
	{
		Metadata = new("Residual Stock/prepare wooden scroll rest", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:medieval_jewish_scroll_rest"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Medieval.cs:893", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "prepare wooden scroll rest",
		Category = "Residual Stock",
		Blurb = "prepare a wooden scroll rest",
		Action = "prepareing a wooden scroll rest",
		ActiveCraftItemSdesc = "an in-progress wooden scroll rest craft",
		Gate = CatalogueRecipeGates.MedievalResidualStockCraftingCarpentry10,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
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
			new CatalogueCommodityRecipeInput(Grams: 675.0, Material: "boxwood", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("675 grams of boxwood", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "medieval_jewish_scroll_rest", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a wooden scroll rest (@medieval_jewish_scroll_rest)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ResidualStockPrepareColourWoolSkullcap() => new()
	{
		Metadata = new("Residual Stock/prepare $colour wool skullcap", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:medieval_jewish_skullcap"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Medieval.cs:893", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "prepare $colour wool skullcap",
		Category = "Residual Stock",
		Blurb = "prepare a $colour wool skullcap",
		Action = "prepareing a $colour wool skullcap",
		ActiveCraftItemSdesc = "an in-progress $colour wool skullcap craft",
		Gate = CatalogueRecipeGates.MedievalResidualStockCraftingCarpentry10,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
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
			new CatalogueCommodityRecipeInput(Grams: 45.0, Material: "wool", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("45 grams of wool", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "medieval_jewish_skullcap", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a $colour wool skullcap (@medieval_jewish_skullcap)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ResidualStockPrepareSynagogueBench() => new()
	{
		Metadata = new("Residual Stock/prepare synagogue bench", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:medieval_jewish_synagogue_bench"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Medieval.cs:893", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "prepare synagogue bench",
		Category = "Residual Stock",
		Blurb = "prepare a synagogue bench",
		Action = "prepareing a synagogue bench",
		ActiveCraftItemSdesc = "an in-progress synagogue bench craft",
		Gate = CatalogueRecipeGates.MedievalResidualStockCraftingCarpentry10,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
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
			new CatalogueCommodityRecipeInput(Grams: 16500.0, Material: "oak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("16500 grams of oak", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "medieval_jewish_synagogue_bench", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a synagogue bench (@medieval_jewish_synagogue_bench)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ResidualStockPrepareColour1WoolTallitGadol() => new()
	{
		Metadata = new("Residual Stock/prepare $colour1 wool tallit gadol", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:medieval_jewish_tallit_gadol"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Medieval.cs:893", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "prepare $colour1 wool tallit gadol",
		Category = "Residual Stock",
		Blurb = "prepare a $colour1 wool tallit gadol",
		Action = "prepareing a $colour1 wool tallit gadol",
		ActiveCraftItemSdesc = "an in-progress $colour1 wool tallit gadol craft",
		Gate = CatalogueRecipeGates.MedievalResidualStockCraftingCarpentry10,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
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
			new CatalogueCommodityRecipeInput(Grams: 315.0, Material: "wool", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("315 grams of wool", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "medieval_jewish_tallit_gadol", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a $colour1 wool tallit gadol (@medieval_jewish_tallit_gadol)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ResidualStockPrepareColourWoolTallitKatan() => new()
	{
		Metadata = new("Residual Stock/prepare $colour wool tallit katan", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:medieval_jewish_tallit_katan"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Medieval.cs:893", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "prepare $colour wool tallit katan",
		Category = "Residual Stock",
		Blurb = "prepare a $colour wool tallit katan",
		Action = "prepareing a $colour wool tallit katan",
		ActiveCraftItemSdesc = "an in-progress $colour wool tallit katan craft",
		Gate = CatalogueRecipeGates.MedievalResidualStockCraftingCarpentry10,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
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
			new CatalogueCommodityRecipeInput(Grams: 173.0, Material: "wool", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("173 grams of wool", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "medieval_jewish_tallit_katan", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a $colour wool tallit katan (@medieval_jewish_tallit_katan)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ResidualStockPrepareWallTabletPanel() => new()
	{
		Metadata = new("Residual Stock/prepare wall tablet panel", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:medieval_jewish_wall_tablet_panel"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Medieval.cs:893", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "prepare wall tablet panel",
		Category = "Residual Stock",
		Blurb = "prepare a wall tablet panel",
		Action = "prepareing a wall tablet panel",
		ActiveCraftItemSdesc = "an in-progress wall tablet panel craft",
		Gate = CatalogueRecipeGates.MedievalResidualStockCraftingCarpentry10,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
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
			new CatalogueCommodityRecipeInput(Grams: 2550.0, Material: "stoneware", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2550 grams of stoneware", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "medieval_jewish_wall_tablet_panel", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a wall tablet panel (@medieval_jewish_wall_tablet_panel)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ResidualStockPrepareCeramicWashingBasin() => new()
	{
		Metadata = new("Residual Stock/prepare ceramic washing basin", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:medieval_jewish_washing_basin"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Medieval.cs:893", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "prepare ceramic washing basin",
		Category = "Residual Stock",
		Blurb = "prepare a ceramic washing basin",
		Action = "prepareing a ceramic washing basin",
		ActiveCraftItemSdesc = "an in-progress ceramic washing basin craft",
		Gate = CatalogueRecipeGates.MedievalResidualStockCraftingCarpentry10,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
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
			new CatalogueCommodityRecipeInput(Grams: 4200.0, Material: "ceramic", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("4200 grams of ceramic", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "medieval_jewish_washing_basin", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a ceramic washing basin (@medieval_jewish_washing_basin)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ResidualStockPrepareBrassWashingEwer() => new()
	{
		Metadata = new("Residual Stock/prepare brass washing ewer", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:medieval_jewish_washing_ewer"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Medieval.cs:893", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "prepare brass washing ewer",
		Category = "Residual Stock",
		Blurb = "prepare a brass washing ewer",
		Action = "prepareing a brass washing ewer",
		ActiveCraftItemSdesc = "an in-progress brass washing ewer craft",
		Gate = CatalogueRecipeGates.MedievalResidualStockCraftingCarpentry10,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
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
			new CatalogueCommodityRecipeInput(Grams: 1050.0, Material: "brass", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1050 grams of brass", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "medieval_jewish_washing_ewer", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a brass washing ewer (@medieval_jewish_washing_ewer)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ResidualStockPrepareColourKitchenSleeveTies() => new()
	{
		Metadata = new("Residual Stock/prepare $colour kitchen sleeve ties", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:medieval_kitchen_sleeve_ties"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Medieval.cs:893", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "prepare $colour kitchen sleeve ties",
		Category = "Residual Stock",
		Blurb = "prepare a pair of $colour kitchen sleeve ties",
		Action = "prepareing a pair of $colour kitchen sleeve ties",
		ActiveCraftItemSdesc = "an in-progress pair of $colour kitchen sleeve ties craft",
		Gate = CatalogueRecipeGates.MedievalResidualStockCraftingCarpentry10,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
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
			new CatalogueCommodityRecipeInput(Grams: 34.0, Material: "linen", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("34 grams of linen", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "medieval_kitchen_sleeve_ties", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a pair of $colour kitchen sleeve ties (@medieval_kitchen_sleeve_ties)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ResidualStockPrepareLargeColourLinenWorkApron() => new()
	{
		Metadata = new("Residual Stock/prepare large $colour linen work apron", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:medieval_large_linen_work_apron"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Medieval.cs:893", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "prepare large $colour linen work apron",
		Category = "Residual Stock",
		Blurb = "prepare a large $colour linen work apron",
		Action = "prepareing a large $colour linen work apron",
		ActiveCraftItemSdesc = "an in-progress large $colour linen work apron craft",
		Gate = CatalogueRecipeGates.MedievalResidualStockCraftingCarpentry10,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
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
			new CatalogueCommodityRecipeInput(Grams: 135.0, Material: "linen", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("135 grams of linen", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "medieval_large_linen_work_apron", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a large $colour linen work apron (@medieval_large_linen_work_apron)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ResidualStockPrepareColourLinenAmice() => new()
	{
		Metadata = new("Residual Stock/prepare $colour linen amice", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:medieval_latin_amice"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Medieval.cs:893", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "prepare $colour linen amice",
		Category = "Residual Stock",
		Blurb = "prepare a $colour linen amice",
		Action = "prepareing a $colour linen amice",
		ActiveCraftItemSdesc = "an in-progress $colour linen amice craft",
		Gate = CatalogueRecipeGates.MedievalResidualStockCraftingCarpentry10,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
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
			new CatalogueCommodityRecipeInput(Grams: 90.0, Material: "linen", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("90 grams of linen", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "medieval_latin_amice", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a $colour linen amice (@medieval_latin_amice)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ResidualStockPrepareWhiteWoolPalliumBand() => new()
	{
		Metadata = new("Residual Stock/prepare white wool pallium band", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:medieval_latin_archbishop_pallium"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Medieval.cs:893", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "prepare white wool pallium band",
		Category = "Residual Stock",
		Blurb = "prepare a white wool pallium band",
		Action = "prepareing a white wool pallium band",
		ActiveCraftItemSdesc = "an in-progress white wool pallium band craft",
		Gate = CatalogueRecipeGates.MedievalResidualStockCraftingCarpentry10,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
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
			new CatalogueCommodityRecipeInput(Grams: 105.0, Material: "wool", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("105 grams of wool", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "medieval_latin_archbishop_pallium", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a white wool pallium band (@medieval_latin_archbishop_pallium)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ResidualStockPrepareFineWhiteLiturgicalGloves() => new()
	{
		Metadata = new("Residual Stock/prepare fine white liturgical gloves", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:medieval_latin_bishop_gloves"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Medieval.cs:893", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "prepare fine white liturgical gloves",
		Category = "Residual Stock",
		Blurb = "prepare a pair of fine white liturgical gloves",
		Action = "prepareing a pair of fine white liturgical gloves",
		ActiveCraftItemSdesc = "an in-progress pair of fine white liturgical gloves craft",
		Gate = CatalogueRecipeGates.MedievalResidualStockCraftingCarpentry10,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
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
			new CatalogueCommodityRecipeInput(Grams: 68.0, Material: "silk", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("68 grams of silk", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "medieval_latin_bishop_gloves", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a pair of fine white liturgical gloves (@medieval_latin_bishop_gloves)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ResidualStockPrepareFineWhiteSilkMitre() => new()
	{
		Metadata = new("Residual Stock/prepare fine white silk mitre", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:medieval_latin_bishop_mitre"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Medieval.cs:893", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "prepare fine white silk mitre",
		Category = "Residual Stock",
		Blurb = "prepare a fine white silk mitre",
		Action = "prepareing a fine white silk mitre",
		ActiveCraftItemSdesc = "an in-progress fine white silk mitre craft",
		Gate = CatalogueRecipeGates.MedievalResidualStockCraftingCarpentry10,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
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
			new CatalogueCommodityRecipeInput(Grams: 120.0, Material: "silk", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("120 grams of silk", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "medieval_latin_bishop_mitre", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a fine white silk mitre (@medieval_latin_bishop_mitre)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ResidualStockPrepareBlackWoolClericalGown() => new()
	{
		Metadata = new("Residual Stock/prepare black wool clerical gown", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:medieval_latin_black_clerical_gown"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Medieval.cs:893", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "prepare black wool clerical gown",
		Category = "Residual Stock",
		Blurb = "prepare a black wool clerical gown",
		Action = "prepareing a black wool clerical gown",
		ActiveCraftItemSdesc = "an in-progress black wool clerical gown craft",
		Gate = CatalogueRecipeGates.MedievalResidualStockCraftingCarpentry10,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
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
			new CatalogueCommodityRecipeInput(Grams: 585.0, Material: "wool", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("585 grams of wool", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "medieval_latin_black_clerical_gown", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a black wool clerical gown (@medieval_latin_black_clerical_gown)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ResidualStockPrepareBlackWoolCowl() => new()
	{
		Metadata = new("Residual Stock/prepare black wool cowl", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:medieval_latin_black_cowl"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Medieval.cs:893", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "prepare black wool cowl",
		Category = "Residual Stock",
		Blurb = "prepare a black wool cowl",
		Action = "prepareing a black wool cowl",
		ActiveCraftItemSdesc = "an in-progress black wool cowl craft",
		Gate = CatalogueRecipeGates.MedievalResidualStockCraftingCarpentry10,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
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
			new CatalogueCommodityRecipeInput(Grams: 488.0, Material: "wool", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("488 grams of wool", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "medieval_latin_black_cowl", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a black wool cowl (@medieval_latin_black_cowl)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ResidualStockPrepareBlackWoolFriarSCappa() => new()
	{
		Metadata = new("Residual Stock/prepare black wool friar's cappa", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:medieval_latin_black_friar_cappa"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Medieval.cs:893", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "prepare black wool friar's cappa",
		Category = "Residual Stock",
		Blurb = "prepare a black wool friar's cappa",
		Action = "prepareing a black wool friar's cappa",
		ActiveCraftItemSdesc = "an in-progress black wool friar's cappa craft",
		Gate = CatalogueRecipeGates.MedievalResidualStockCraftingCarpentry10,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
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
			new CatalogueCommodityRecipeInput(Grams: 540.0, Material: "wool", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("540 grams of wool", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "medieval_latin_black_friar_cappa", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a black wool friar's cappa (@medieval_latin_black_friar_cappa)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ResidualStockPrepareBlackWoolMonasticHabit() => new()
	{
		Metadata = new("Residual Stock/prepare black wool monastic habit", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:medieval_latin_black_monastic_habit"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Medieval.cs:893", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "prepare black wool monastic habit",
		Category = "Residual Stock",
		Blurb = "prepare a black wool monastic habit",
		Action = "prepareing a black wool monastic habit",
		ActiveCraftItemSdesc = "an in-progress black wool monastic habit craft",
		Gate = CatalogueRecipeGates.MedievalResidualStockCraftingCarpentry10,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
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
			new CatalogueCommodityRecipeInput(Grams: 675.0, Material: "wool", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("675 grams of wool", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "medieval_latin_black_monastic_habit", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a black wool monastic habit (@medieval_latin_black_monastic_habit)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ResidualStockPrepareBlackWoolOverScapular() => new()
	{
		Metadata = new("Residual Stock/prepare black wool over-scapular", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:medieval_latin_black_over_scapular"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Medieval.cs:893", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "prepare black wool over-scapular",
		Category = "Residual Stock",
		Blurb = "prepare a black wool over-scapular",
		Action = "prepareing a black wool over-scapular",
		ActiveCraftItemSdesc = "an in-progress black wool over-scapular craft",
		Gate = CatalogueRecipeGates.MedievalResidualStockCraftingCarpentry10,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
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
			new CatalogueCommodityRecipeInput(Grams: 210.0, Material: "wool", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("210 grams of wool", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "medieval_latin_black_over_scapular", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a black wool over-scapular (@medieval_latin_black_over_scapular)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ResidualStockPrepareBlackWoolScapular() => new()
	{
		Metadata = new("Residual Stock/prepare black wool scapular", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:medieval_latin_black_scapular"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Medieval.cs:893", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "prepare black wool scapular",
		Category = "Residual Stock",
		Blurb = "prepare a black wool scapular",
		Action = "prepareing a black wool scapular",
		ActiveCraftItemSdesc = "an in-progress black wool scapular craft",
		Gate = CatalogueRecipeGates.MedievalResidualStockCraftingCarpentry10,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
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
			new CatalogueCommodityRecipeInput(Grams: 195.0, Material: "wool", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("195 grams of wool", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "medieval_latin_black_scapular", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a black wool scapular (@medieval_latin_black_scapular)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ResidualStockPrepareBrownWoolFriarSHabit() => new()
	{
		Metadata = new("Residual Stock/prepare brown wool friar's habit", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:medieval_latin_brown_friar_habit"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Medieval.cs:893", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "prepare brown wool friar's habit",
		Category = "Residual Stock",
		Blurb = "prepare a brown wool friar's habit",
		Action = "prepareing a brown wool friar's habit",
		ActiveCraftItemSdesc = "an in-progress brown wool friar's habit craft",
		Gate = CatalogueRecipeGates.MedievalResidualStockCraftingCarpentry10,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
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
			new CatalogueCommodityRecipeInput(Grams: 600.0, Material: "wool", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("600 grams of wool", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "medieval_latin_brown_friar_habit", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a brown wool friar's habit (@medieval_latin_brown_friar_habit)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ResidualStockPrepareBlackWoolCanonSHabit() => new()
	{
		Metadata = new("Residual Stock/prepare black wool canon's habit", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:medieval_latin_canon_black_habit"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Medieval.cs:893", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "prepare black wool canon's habit",
		Category = "Residual Stock",
		Blurb = "prepare a black wool canon's habit",
		Action = "prepareing a black wool canon's habit",
		ActiveCraftItemSdesc = "an in-progress black wool canon's habit craft",
		Gate = CatalogueRecipeGates.MedievalResidualStockCraftingCarpentry10,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
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
			new CatalogueCommodityRecipeInput(Grams: 638.0, Material: "wool", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("638 grams of wool", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "medieval_latin_canon_black_habit", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a black wool canon's habit (@medieval_latin_canon_black_habit)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ResidualStockPrepareFineColourSilkChasuble() => new()
	{
		Metadata = new("Residual Stock/prepare fine $colour silk chasuble", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:medieval_latin_chasuble"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Medieval.cs:893", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "prepare fine $colour silk chasuble",
		Category = "Residual Stock",
		Blurb = "prepare a fine $colour silk chasuble",
		Action = "prepareing a fine $colour silk chasuble",
		ActiveCraftItemSdesc = "an in-progress fine $colour silk chasuble craft",
		Gate = CatalogueRecipeGates.MedievalResidualStockCraftingCarpentry10,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
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
			new CatalogueCommodityRecipeInput(Grams: 540.0, Material: "silk", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("540 grams of silk", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "medieval_latin_chasuble", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a fine $colour silk chasuble (@medieval_latin_chasuble)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ResidualStockPrepareFineColourSilkDalmatic() => new()
	{
		Metadata = new("Residual Stock/prepare fine $colour silk dalmatic", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:medieval_latin_dalmatic_vestment"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Medieval.cs:893", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "prepare fine $colour silk dalmatic",
		Category = "Residual Stock",
		Blurb = "prepare a fine $colour silk dalmatic",
		Action = "prepareing a fine $colour silk dalmatic",
		ActiveCraftItemSdesc = "an in-progress fine $colour silk dalmatic craft",
		Gate = CatalogueRecipeGates.MedievalResidualStockCraftingCarpentry10,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
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
			new CatalogueCommodityRecipeInput(Grams: 488.0, Material: "silk", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("488 grams of silk", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "medieval_latin_dalmatic_vestment", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a fine $colour silk dalmatic (@medieval_latin_dalmatic_vestment)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ResidualStockPrepareGreyWoolFriarSHabit() => new()
	{
		Metadata = new("Residual Stock/prepare grey wool friar's habit", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:medieval_latin_friar_grey_habit"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Medieval.cs:893", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "prepare grey wool friar's habit",
		Category = "Residual Stock",
		Blurb = "prepare a grey wool friar's habit",
		Action = "prepareing a grey wool friar's habit",
		ActiveCraftItemSdesc = "an in-progress grey wool friar's habit craft",
		Gate = CatalogueRecipeGates.MedievalResidualStockCraftingCarpentry10,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
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
			new CatalogueCommodityRecipeInput(Grams: 638.0, Material: "wool", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("638 grams of wool", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "medieval_latin_friar_grey_habit", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a grey wool friar's habit (@medieval_latin_friar_grey_habit)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ResidualStockPrepareColourLinenCincture() => new()
	{
		Metadata = new("Residual Stock/prepare $colour linen cincture", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:medieval_latin_linen_cincture"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Medieval.cs:893", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "prepare $colour linen cincture",
		Category = "Residual Stock",
		Blurb = "prepare a $colour linen cincture",
		Action = "prepareing a $colour linen cincture",
		ActiveCraftItemSdesc = "an in-progress $colour linen cincture craft",
		Gate = CatalogueRecipeGates.MedievalResidualStockCraftingCarpentry10,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
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
			new CatalogueCommodityRecipeInput(Grams: 60.0, Material: "linen", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("60 grams of linen", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "medieval_latin_linen_cincture", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a $colour linen cincture (@medieval_latin_linen_cincture)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ResidualStockPrepareColourSilkManiple() => new()
	{
		Metadata = new("Residual Stock/prepare $colour silk maniple", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:medieval_latin_maniple"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Medieval.cs:893", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "prepare $colour silk maniple",
		Category = "Residual Stock",
		Blurb = "prepare a $colour silk maniple",
		Action = "prepareing a $colour silk maniple",
		ActiveCraftItemSdesc = "an in-progress $colour silk maniple craft",
		Gate = CatalogueRecipeGates.MedievalResidualStockCraftingCarpentry10,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
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
			new CatalogueCommodityRecipeInput(Grams: 45.0, Material: "silk", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("45 grams of silk", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "medieval_latin_maniple", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a $colour silk maniple (@medieval_latin_maniple)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ResidualStockPrepareWhiteLinenWimpleAndVeil() => new()
	{
		Metadata = new("Residual Stock/prepare white linen wimple and veil", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:medieval_latin_nun_wimple_veil"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Medieval.cs:893", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "prepare white linen wimple and veil",
		Category = "Residual Stock",
		Blurb = "prepare a white linen wimple and veil",
		Action = "prepareing a white linen wimple and veil",
		ActiveCraftItemSdesc = "an in-progress white linen wimple and veil craft",
		Gate = CatalogueRecipeGates.MedievalResidualStockCraftingCarpentry10,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
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
			new CatalogueCommodityRecipeInput(Grams: 165.0, Material: "linen", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("165 grams of linen", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "medieval_latin_nun_wimple_veil", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a white linen wimple and veil (@medieval_latin_nun_wimple_veil)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ResidualStockPrepareGreyWoolSisterSHabit() => new()
	{
		Metadata = new("Residual Stock/prepare grey wool sister's habit", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:medieval_latin_poor_clare_grey_habit"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Medieval.cs:893", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "prepare grey wool sister's habit",
		Category = "Residual Stock",
		Blurb = "prepare a grey wool sister's habit",
		Action = "prepareing a grey wool sister's habit",
		ActiveCraftItemSdesc = "an in-progress grey wool sister's habit craft",
		Gate = CatalogueRecipeGates.MedievalResidualStockCraftingCarpentry10,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
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
			new CatalogueItemRecipeProduct(ItemStableReference: "medieval_latin_poor_clare_grey_habit", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a grey wool sister's habit (@medieval_latin_poor_clare_grey_habit)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ResidualStockPreparePlainLinenSisterSVeil() => new()
	{
		Metadata = new("Residual Stock/prepare plain linen sister's veil", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:medieval_latin_poor_clare_veil"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Medieval.cs:893", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "prepare plain linen sister's veil",
		Category = "Residual Stock",
		Blurb = "prepare a plain linen sister's veil",
		Action = "prepareing a plain linen sister's veil",
		ActiveCraftItemSdesc = "an in-progress plain linen sister's veil craft",
		Gate = CatalogueRecipeGates.MedievalResidualStockCraftingCarpentry10,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
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
			new CatalogueCommodityRecipeInput(Grams: 135.0, Material: "linen", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("135 grams of linen", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "medieval_latin_poor_clare_veil", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a plain linen sister's veil (@medieval_latin_poor_clare_veil)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ResidualStockPrepareFineColourSilkCope() => new()
	{
		Metadata = new("Residual Stock/prepare fine $colour silk cope", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:medieval_latin_processional_cope"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Medieval.cs:893", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "prepare fine $colour silk cope",
		Category = "Residual Stock",
		Blurb = "prepare a fine $colour silk cope",
		Action = "prepareing a fine $colour silk cope",
		ActiveCraftItemSdesc = "an in-progress fine $colour silk cope craft",
		Gate = CatalogueRecipeGates.MedievalResidualStockCraftingCarpentry10,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
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
			new CatalogueCommodityRecipeInput(Grams: 675.0, Material: "silk", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("675 grams of silk", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "medieval_latin_processional_cope", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a fine $colour silk cope (@medieval_latin_processional_cope)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ResidualStockPrepareKnottedRopeCincture() => new()
	{
		Metadata = new("Residual Stock/prepare knotted rope cincture", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:medieval_latin_rope_cincture"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Medieval.cs:893", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "prepare knotted rope cincture",
		Category = "Residual Stock",
		Blurb = "prepare a knotted rope cincture",
		Action = "prepareing a knotted rope cincture",
		ActiveCraftItemSdesc = "an in-progress knotted rope cincture craft",
		Gate = CatalogueRecipeGates.MedievalResidualStockCraftingCarpentry10,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
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
			new CatalogueCommodityRecipeInput(Grams: 120.0, Material: "hemp", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("120 grams of hemp", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "medieval_latin_rope_cincture", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a knotted rope cincture (@medieval_latin_rope_cincture)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ResidualStockPrepareDarkWoolNunSHabit() => new()
	{
		Metadata = new("Residual Stock/prepare dark wool nun's habit", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:medieval_latin_simple_nun_habit"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Medieval.cs:893", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "prepare dark wool nun's habit",
		Category = "Residual Stock",
		Blurb = "prepare a dark wool nun's habit",
		Action = "prepareing a dark wool nun's habit",
		ActiveCraftItemSdesc = "an in-progress dark wool nun's habit craft",
		Gate = CatalogueRecipeGates.MedievalResidualStockCraftingCarpentry10,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
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
			new CatalogueCommodityRecipeInput(Grams: 615.0, Material: "wool", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("615 grams of wool", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "medieval_latin_simple_nun_habit", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a dark wool nun's habit (@medieval_latin_simple_nun_habit)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ResidualStockPrepareColourSilkStole() => new()
	{
		Metadata = new("Residual Stock/prepare $colour silk stole", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:medieval_latin_stole"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Medieval.cs:893", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "prepare $colour silk stole",
		Category = "Residual Stock",
		Blurb = "prepare a $colour silk stole",
		Action = "prepareing a $colour silk stole",
		ActiveCraftItemSdesc = "an in-progress $colour silk stole craft",
		Gate = CatalogueRecipeGates.MedievalResidualStockCraftingCarpentry10,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
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
			new CatalogueCommodityRecipeInput(Grams: 83.0, Material: "silk", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("83 grams of silk", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "medieval_latin_stole", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a $colour silk stole (@medieval_latin_stole)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ResidualStockPrepareWhiteLinenAlb() => new()
	{
		Metadata = new("Residual Stock/prepare white linen alb", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:medieval_latin_white_alb"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Medieval.cs:893", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "prepare white linen alb",
		Category = "Residual Stock",
		Blurb = "prepare a white linen alb",
		Action = "prepareing a white linen alb",
		ActiveCraftItemSdesc = "an in-progress white linen alb craft",
		Gate = CatalogueRecipeGates.MedievalResidualStockCraftingCarpentry10,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
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
			new CatalogueCommodityRecipeInput(Grams: 345.0, Material: "linen", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("345 grams of linen", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "medieval_latin_white_alb", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a white linen alb (@medieval_latin_white_alb)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ResidualStockPrepareWhiteWoolHermitSHabit() => new()
	{
		Metadata = new("Residual Stock/prepare white wool hermit's habit", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:medieval_latin_white_carthusian_habit"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Medieval.cs:893", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "prepare white wool hermit's habit",
		Category = "Residual Stock",
		Blurb = "prepare a white wool hermit's habit",
		Action = "prepareing a white wool hermit's habit",
		ActiveCraftItemSdesc = "an in-progress white wool hermit's habit craft",
		Gate = CatalogueRecipeGates.MedievalResidualStockCraftingCarpentry10,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
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
			new CatalogueCommodityRecipeInput(Grams: 713.0, Material: "wool", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("713 grams of wool", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "medieval_latin_white_carthusian_habit", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a white wool hermit's habit (@medieval_latin_white_carthusian_habit)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};
}
