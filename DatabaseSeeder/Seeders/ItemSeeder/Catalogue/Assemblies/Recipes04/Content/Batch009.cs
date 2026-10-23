#nullable enable

using MudSharp.Body;
using MudSharp.GameItems.Inventory.Plans;
using MudSharp.RPG.Checks;

namespace DatabaseSeeder.Seeders;

public static partial class Recipes04Stock
{
	public static CatalogueRecipeDefinition JewelleryAndDevotionalGoodsMakeElectrumFingerRing() => new()
	{
		Metadata = new("Jewellery and Devotional Goods/make electrum finger ring", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:medieval_jewellery_ring_electrum_finger_ring"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Medieval.cs:893", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make electrum finger ring",
		Category = "Jewellery and Devotional Goods",
		Blurb = "make an electrum finger ring",
		Action = "makeing an electrum finger ring",
		ActiveCraftItemSdesc = "an in-progress electrum finger ring craft",
		Gate = CatalogueRecipeGates.MedievalJewelleryAndDevotionalCraftingGoldsmithing10,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
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
			new CatalogueCommodityRecipeInput(Grams: 8.0, Material: "electrum", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("8 grams of electrum", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "medieval_jewellery_ring_electrum_finger_ring", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x an electrum finger ring (@medieval_jewellery_ring_electrum_finger_ring)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition JewelleryAndDevotionalGoodsMakeEmeraldCourtRing() => new()
	{
		Metadata = new("Jewellery and Devotional Goods/make emerald court ring", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:medieval_jewellery_ring_emerald_court_ring"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Medieval.cs:893", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make emerald court ring",
		Category = "Jewellery and Devotional Goods",
		Blurb = "make an emerald court ring",
		Action = "makeing an emerald court ring",
		ActiveCraftItemSdesc = "an in-progress emerald court ring craft",
		Gate = CatalogueRecipeGates.MedievalJewelleryAndDevotionalCraftingGoldsmithing10,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
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
			new CatalogueCommodityRecipeInput(Grams: 11.0, Material: "gold", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("11 grams of gold", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "medieval_jewellery_ring_emerald_court_ring", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x an emerald court ring (@medieval_jewellery_ring_emerald_court_ring)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition JewelleryAndDevotionalGoodsMakeFinishSilverRing() => new()
	{
		Metadata = new("Jewellery and Devotional Goods/make $finish silver ring", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:medieval_jewellery_ring_finish_silver_ring"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Medieval.cs:893", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make $finish silver ring",
		Category = "Jewellery and Devotional Goods",
		Blurb = "make a $finish silver ring",
		Action = "makeing a $finish silver ring",
		ActiveCraftItemSdesc = "an in-progress $finish silver ring craft",
		Gate = CatalogueRecipeGates.MedievalJewelleryAndDevotionalCraftingGoldsmithing10,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
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
			new CatalogueCommodityRecipeInput(Grams: 9.0, Material: "silver", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("9 grams of silver", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "medieval_jewellery_ring_finish_silver_ring", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a $finish silver ring (@medieval_jewellery_ring_finish_silver_ring)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition JewelleryAndDevotionalGoodsMakeGarnetSetGoldRing() => new()
	{
		Metadata = new("Jewellery and Devotional Goods/make garnet-set gold ring", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:medieval_jewellery_ring_garnet_set_gold_ring"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Medieval.cs:893", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make garnet-set gold ring",
		Category = "Jewellery and Devotional Goods",
		Blurb = "make a garnet-set gold ring",
		Action = "makeing a garnet-set gold ring",
		ActiveCraftItemSdesc = "an in-progress garnet-set gold ring craft",
		Gate = CatalogueRecipeGates.MedievalJewelleryAndDevotionalCraftingGoldsmithing10,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
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
			new CatalogueCommodityRecipeInput(Grams: 10.0, Material: "gold", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("10 grams of gold", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "medieval_jewellery_ring_garnet_set_gold_ring", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a garnet-set gold ring (@medieval_jewellery_ring_garnet_set_gold_ring)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition JewelleryAndDevotionalGoodsMakeGildedBronzeRing() => new()
	{
		Metadata = new("Jewellery and Devotional Goods/make gilded bronze ring", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:medieval_jewellery_ring_gilded_bronze_ring"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Medieval.cs:893", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make gilded bronze ring",
		Category = "Jewellery and Devotional Goods",
		Blurb = "make a gilded bronze ring",
		Action = "makeing a gilded bronze ring",
		ActiveCraftItemSdesc = "an in-progress gilded bronze ring craft",
		Gate = CatalogueRecipeGates.MedievalJewelleryAndDevotionalCraftingGoldsmithing10,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
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
			new CatalogueCommodityRecipeInput(Grams: 10.0, Material: "gilded bronze", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("10 grams of gilded bronze", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "medieval_jewellery_ring_gilded_bronze_ring", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a gilded bronze ring (@medieval_jewellery_ring_gilded_bronze_ring)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition JewelleryAndDevotionalGoodsMakeGildedCopperRing() => new()
	{
		Metadata = new("Jewellery and Devotional Goods/make gilded copper ring", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:medieval_jewellery_ring_gilded_copper_ring"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Medieval.cs:893", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make gilded copper ring",
		Category = "Jewellery and Devotional Goods",
		Blurb = "make a gilded copper ring",
		Action = "makeing a gilded copper ring",
		ActiveCraftItemSdesc = "an in-progress gilded copper ring craft",
		Gate = CatalogueRecipeGates.MedievalJewelleryAndDevotionalCraftingGoldsmithing10,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
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
			new CatalogueCommodityRecipeInput(Grams: 9.0, Material: "gilded copper", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("9 grams of gilded copper", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "medieval_jewellery_ring_gilded_copper_ring", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a gilded copper ring (@medieval_jewellery_ring_gilded_copper_ring)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition JewelleryAndDevotionalGoodsMakeHammeredBronzeRing() => new()
	{
		Metadata = new("Jewellery and Devotional Goods/make hammered bronze ring", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:medieval_jewellery_ring_hammered_bronze_ring"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Medieval.cs:893", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make hammered bronze ring",
		Category = "Jewellery and Devotional Goods",
		Blurb = "make a hammered bronze ring",
		Action = "makeing a hammered bronze ring",
		ActiveCraftItemSdesc = "an in-progress hammered bronze ring craft",
		Gate = CatalogueRecipeGates.MedievalJewelleryAndDevotionalCraftingGoldsmithing10,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
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
			new CatalogueCommodityRecipeInput(Grams: 9.0, Material: "bronze", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("9 grams of bronze", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "medieval_jewellery_ring_hammered_bronze_ring", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a hammered bronze ring (@medieval_jewellery_ring_hammered_bronze_ring)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition JewelleryAndDevotionalGoodsMakeHeavyGoldThumbRing() => new()
	{
		Metadata = new("Jewellery and Devotional Goods/make heavy gold thumb ring", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:medieval_jewellery_ring_heavy_gold_thumb_ring"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Medieval.cs:893", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make heavy gold thumb ring",
		Category = "Jewellery and Devotional Goods",
		Blurb = "make a heavy gold thumb ring",
		Action = "makeing a heavy gold thumb ring",
		ActiveCraftItemSdesc = "an in-progress heavy gold thumb ring craft",
		Gate = CatalogueRecipeGates.MedievalJewelleryAndDevotionalCraftingGoldsmithing10,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
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
			new CatalogueCommodityRecipeInput(Grams: 18.0, Material: "gold", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("18 grams of gold", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "medieval_jewellery_ring_heavy_gold_thumb_ring", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a heavy gold thumb ring (@medieval_jewellery_ring_heavy_gold_thumb_ring)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition JewelleryAndDevotionalGoodsMakeJadeThumbRing() => new()
	{
		Metadata = new("Jewellery and Devotional Goods/make jade thumb ring", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:medieval_jewellery_ring_jade_thumb_ring"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Medieval.cs:893", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make jade thumb ring",
		Category = "Jewellery and Devotional Goods",
		Blurb = "make a jade thumb ring",
		Action = "makeing a jade thumb ring",
		ActiveCraftItemSdesc = "an in-progress jade thumb ring craft",
		Gate = CatalogueRecipeGates.MedievalJewelleryAndDevotionalCraftingGoldsmithing10,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
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
			new CatalogueCommodityRecipeInput(Grams: 17.0, Material: "jade", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("17 grams of jade", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "medieval_jewellery_ring_jade_thumb_ring", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a jade thumb ring (@medieval_jewellery_ring_jade_thumb_ring)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition JewelleryAndDevotionalGoodsMakeKnotworkLoveRing() => new()
	{
		Metadata = new("Jewellery and Devotional Goods/make knotwork love ring", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:medieval_jewellery_ring_knotwork_love_ring"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Medieval.cs:893", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make knotwork love ring",
		Category = "Jewellery and Devotional Goods",
		Blurb = "make a knotwork love ring",
		Action = "makeing a knotwork love ring",
		ActiveCraftItemSdesc = "an in-progress knotwork love ring craft",
		Gate = CatalogueRecipeGates.MedievalJewelleryAndDevotionalCraftingGoldsmithing10,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
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
			new CatalogueCommodityRecipeInput(Grams: 8.0, Material: "silver", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("8 grams of silver", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "medieval_jewellery_ring_knotwork_love_ring", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a knotwork love ring (@medieval_jewellery_ring_knotwork_love_ring)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition JewelleryAndDevotionalGoodsMakeLapisSignetLikeRing() => new()
	{
		Metadata = new("Jewellery and Devotional Goods/make lapis signet-like ring", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:medieval_jewellery_ring_lapis_signet_like_ring"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Medieval.cs:893", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make lapis signet-like ring",
		Category = "Jewellery and Devotional Goods",
		Blurb = "make a lapis signet-like ring",
		Action = "makeing a lapis signet-like ring",
		ActiveCraftItemSdesc = "an in-progress lapis signet-like ring craft",
		Gate = CatalogueRecipeGates.MedievalJewelleryAndDevotionalCraftingGoldsmithing10,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
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
			new CatalogueCommodityRecipeInput(Grams: 14.0, Material: "silver", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("14 grams of silver", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "medieval_jewellery_ring_lapis_signet_like_ring", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a lapis signet-like ring (@medieval_jewellery_ring_lapis_signet_like_ring)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition JewelleryAndDevotionalGoodsMakeMoonstoneGoldRing() => new()
	{
		Metadata = new("Jewellery and Devotional Goods/make moonstone gold ring", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:medieval_jewellery_ring_moonstone_gold_ring"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Medieval.cs:893", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make moonstone gold ring",
		Category = "Jewellery and Devotional Goods",
		Blurb = "make a moonstone gold ring",
		Action = "makeing a moonstone gold ring",
		ActiveCraftItemSdesc = "an in-progress moonstone gold ring craft",
		Gate = CatalogueRecipeGates.MedievalJewelleryAndDevotionalCraftingGoldsmithing10,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
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
			new CatalogueCommodityRecipeInput(Grams: 10.0, Material: "gold", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("10 grams of gold", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "medieval_jewellery_ring_moonstone_gold_ring", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a moonstone gold ring (@medieval_jewellery_ring_moonstone_gold_ring)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition JewelleryAndDevotionalGoodsMakeMotherOfPearlRing() => new()
	{
		Metadata = new("Jewellery and Devotional Goods/make mother-of-pearl ring", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:medieval_jewellery_ring_mother_of_pearl_ring"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Medieval.cs:893", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make mother-of-pearl ring",
		Category = "Jewellery and Devotional Goods",
		Blurb = "make a mother-of-pearl ring",
		Action = "makeing a mother-of-pearl ring",
		ActiveCraftItemSdesc = "an in-progress mother-of-pearl ring craft",
		Gate = CatalogueRecipeGates.MedievalJewelleryAndDevotionalCraftingGoldsmithing10,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
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
			new CatalogueCommodityRecipeInput(Grams: 6.0, Material: "mother-of-pearl", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("6 grams of mother-of-pearl", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "medieval_jewellery_ring_mother_of_pearl_ring", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a mother-of-pearl ring (@medieval_jewellery_ring_mother_of_pearl_ring)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition JewelleryAndDevotionalGoodsMakeMotifBrassRing() => new()
	{
		Metadata = new("Jewellery and Devotional Goods/make $motif brass ring", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:medieval_jewellery_ring_motif_brass_ring"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Medieval.cs:893", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make $motif brass ring",
		Category = "Jewellery and Devotional Goods",
		Blurb = "make a $motif brass ring",
		Action = "makeing a $motif brass ring",
		ActiveCraftItemSdesc = "an in-progress $motif brass ring craft",
		Gate = CatalogueRecipeGates.MedievalJewelleryAndDevotionalCraftingGoldsmithing10,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
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
			new CatalogueCommodityRecipeInput(Grams: 9.0, Material: "brass", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("9 grams of brass", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "medieval_jewellery_ring_motif_brass_ring", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a $motif brass ring (@medieval_jewellery_ring_motif_brass_ring)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition JewelleryAndDevotionalGoodsMakeMotifBronzeRing() => new()
	{
		Metadata = new("Jewellery and Devotional Goods/make $motif bronze ring", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:medieval_jewellery_ring_motif_bronze_ring"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Medieval.cs:893", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make $motif bronze ring",
		Category = "Jewellery and Devotional Goods",
		Blurb = "make a $motif bronze ring",
		Action = "makeing a $motif bronze ring",
		ActiveCraftItemSdesc = "an in-progress $motif bronze ring craft",
		Gate = CatalogueRecipeGates.MedievalJewelleryAndDevotionalCraftingGoldsmithing10,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
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
			new CatalogueCommodityRecipeInput(Grams: 9.0, Material: "bronze", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("9 grams of bronze", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "medieval_jewellery_ring_motif_bronze_ring", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a $motif bronze ring (@medieval_jewellery_ring_motif_bronze_ring)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition JewelleryAndDevotionalGoodsMakeMotifCopperRing() => new()
	{
		Metadata = new("Jewellery and Devotional Goods/make $motif copper ring", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:medieval_jewellery_ring_motif_copper_ring"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Medieval.cs:893", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make $motif copper ring",
		Category = "Jewellery and Devotional Goods",
		Blurb = "make a $motif copper ring",
		Action = "makeing a $motif copper ring",
		ActiveCraftItemSdesc = "an in-progress $motif copper ring craft",
		Gate = CatalogueRecipeGates.MedievalJewelleryAndDevotionalCraftingGoldsmithing10,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
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
			new CatalogueCommodityRecipeInput(Grams: 9.0, Material: "copper", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("9 grams of copper", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "medieval_jewellery_ring_motif_copper_ring", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a $motif copper ring (@medieval_jewellery_ring_motif_copper_ring)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition JewelleryAndDevotionalGoodsMakeMotifGoldRing() => new()
	{
		Metadata = new("Jewellery and Devotional Goods/make $motif gold ring", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:medieval_jewellery_ring_motif_gold_ring"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Medieval.cs:893", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make $motif gold ring",
		Category = "Jewellery and Devotional Goods",
		Blurb = "make a $motif gold ring",
		Action = "makeing a $motif gold ring",
		ActiveCraftItemSdesc = "an in-progress $motif gold ring craft",
		Gate = CatalogueRecipeGates.MedievalJewelleryAndDevotionalCraftingGoldsmithing10,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
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
			new CatalogueCommodityRecipeInput(Grams: 9.0, Material: "gold", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("9 grams of gold", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "medieval_jewellery_ring_motif_gold_ring", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a $motif gold ring (@medieval_jewellery_ring_motif_gold_ring)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition JewelleryAndDevotionalGoodsMakeMotifSilverGiltRing() => new()
	{
		Metadata = new("Jewellery and Devotional Goods/make $motif silver-gilt ring", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:medieval_jewellery_ring_motif_silver_gilt_ring"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Medieval.cs:893", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make $motif silver-gilt ring",
		Category = "Jewellery and Devotional Goods",
		Blurb = "make a $motif silver-gilt ring",
		Action = "makeing a $motif silver-gilt ring",
		ActiveCraftItemSdesc = "an in-progress $motif silver-gilt ring craft",
		Gate = CatalogueRecipeGates.MedievalJewelleryAndDevotionalCraftingGoldsmithing10,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
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
			new CatalogueCommodityRecipeInput(Grams: 9.0, Material: "silver-gilt", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("9 grams of silver-gilt", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "medieval_jewellery_ring_motif_silver_gilt_ring", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a $motif silver-gilt ring (@medieval_jewellery_ring_motif_silver_gilt_ring)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition JewelleryAndDevotionalGoodsMakeMotifSilverRing() => new()
	{
		Metadata = new("Jewellery and Devotional Goods/make $motif silver ring", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:medieval_jewellery_ring_motif_silver_ring"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Medieval.cs:893", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make $motif silver ring",
		Category = "Jewellery and Devotional Goods",
		Blurb = "make a $motif silver ring",
		Action = "makeing a $motif silver ring",
		ActiveCraftItemSdesc = "an in-progress $motif silver ring craft",
		Gate = CatalogueRecipeGates.MedievalJewelleryAndDevotionalCraftingGoldsmithing10,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
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
			new CatalogueCommodityRecipeInput(Grams: 9.0, Material: "silver", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("9 grams of silver", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "medieval_jewellery_ring_motif_silver_ring", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a $motif silver ring (@medieval_jewellery_ring_motif_silver_ring)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition JewelleryAndDevotionalGoodsMakeNarrowIronRing() => new()
	{
		Metadata = new("Jewellery and Devotional Goods/make narrow iron ring", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:medieval_jewellery_ring_narrow_iron_ring"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Medieval.cs:893", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make narrow iron ring",
		Category = "Jewellery and Devotional Goods",
		Blurb = "make a narrow iron ring",
		Action = "makeing a narrow iron ring",
		ActiveCraftItemSdesc = "an in-progress narrow iron ring craft",
		Gate = CatalogueRecipeGates.MedievalJewelleryAndDevotionalCraftingGoldsmithing10,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
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
			new CatalogueCommodityRecipeInput(Grams: 8.0, Material: "wrought iron", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("8 grams of wrought iron", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "medieval_jewellery_ring_narrow_iron_ring", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a narrow iron ring (@medieval_jewellery_ring_narrow_iron_ring)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition JewelleryAndDevotionalGoodsMakeNobleSignetRing() => new()
	{
		Metadata = new("Jewellery and Devotional Goods/make noble signet ring", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:medieval_jewellery_ring_noble_signet_ring"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Medieval.cs:893", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make noble signet ring",
		Category = "Jewellery and Devotional Goods",
		Blurb = "make a noble signet ring",
		Action = "makeing a noble signet ring",
		ActiveCraftItemSdesc = "an in-progress noble signet ring craft",
		Gate = CatalogueRecipeGates.MedievalJewelleryAndDevotionalCraftingGoldsmithing10,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
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
			new CatalogueCommodityRecipeInput(Grams: 14.0, Material: "gold", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("14 grams of gold", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "medieval_jewellery_ring_noble_signet_ring", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a noble signet ring (@medieval_jewellery_ring_noble_signet_ring)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition JewelleryAndDevotionalGoodsMakeOnyxSilverRing() => new()
	{
		Metadata = new("Jewellery and Devotional Goods/make onyx silver ring", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:medieval_jewellery_ring_onyx_silver_ring"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Medieval.cs:893", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make onyx silver ring",
		Category = "Jewellery and Devotional Goods",
		Blurb = "make an onyx silver ring",
		Action = "makeing an onyx silver ring",
		ActiveCraftItemSdesc = "an in-progress onyx silver ring craft",
		Gate = CatalogueRecipeGates.MedievalJewelleryAndDevotionalCraftingGoldsmithing10,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
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
			new CatalogueCommodityRecipeInput(Grams: 11.0, Material: "silver", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("11 grams of silver", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "medieval_jewellery_ring_onyx_silver_ring", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x an onyx silver ring (@medieval_jewellery_ring_onyx_silver_ring)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition JewelleryAndDevotionalGoodsMakePairedCourtshipRing() => new()
	{
		Metadata = new("Jewellery and Devotional Goods/make paired courtship ring", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:medieval_jewellery_ring_paired_courtship_ring"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Medieval.cs:893", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make paired courtship ring",
		Category = "Jewellery and Devotional Goods",
		Blurb = "make a paired courtship ring",
		Action = "makeing a paired courtship ring",
		ActiveCraftItemSdesc = "an in-progress paired courtship ring craft",
		Gate = CatalogueRecipeGates.MedievalJewelleryAndDevotionalCraftingGoldsmithing10,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
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
			new CatalogueCommodityRecipeInput(Grams: 8.0, Material: "silver", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("8 grams of silver", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "medieval_jewellery_ring_paired_courtship_ring", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a paired courtship ring (@medieval_jewellery_ring_paired_courtship_ring)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition JewelleryAndDevotionalGoodsMakePersonalSealRing() => new()
	{
		Metadata = new("Jewellery and Devotional Goods/make personal seal ring", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:medieval_jewellery_ring_personal_seal_ring"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Medieval.cs:893", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make personal seal ring",
		Category = "Jewellery and Devotional Goods",
		Blurb = "make a personal seal ring",
		Action = "makeing a personal seal ring",
		ActiveCraftItemSdesc = "an in-progress personal seal ring craft",
		Gate = CatalogueRecipeGates.MedievalJewelleryAndDevotionalCraftingGoldsmithing10,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
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
			new CatalogueCommodityRecipeInput(Grams: 14.0, Material: "bronze", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("14 grams of bronze", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "medieval_jewellery_ring_personal_seal_ring", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a personal seal ring (@medieval_jewellery_ring_personal_seal_ring)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition JewelleryAndDevotionalGoodsMakePlainMerchantSignetRing() => new()
	{
		Metadata = new("Jewellery and Devotional Goods/make plain merchant signet ring", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:medieval_jewellery_ring_plain_merchant_signet_ring"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Medieval.cs:893", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make plain merchant signet ring",
		Category = "Jewellery and Devotional Goods",
		Blurb = "make a plain merchant signet ring",
		Action = "makeing a plain merchant signet ring",
		ActiveCraftItemSdesc = "an in-progress plain merchant signet ring craft",
		Gate = CatalogueRecipeGates.MedievalJewelleryAndDevotionalCraftingGoldsmithing10,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
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
			new CatalogueCommodityRecipeInput(Grams: 14.0, Material: "silver", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("14 grams of silver", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "medieval_jewellery_ring_plain_merchant_signet_ring", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a plain merchant signet ring (@medieval_jewellery_ring_plain_merchant_signet_ring)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition JewelleryAndDevotionalGoodsMakePlainPewterRing() => new()
	{
		Metadata = new("Jewellery and Devotional Goods/make plain pewter ring", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:medieval_jewellery_ring_plain_pewter_ring"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Medieval.cs:893", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make plain pewter ring",
		Category = "Jewellery and Devotional Goods",
		Blurb = "make a plain pewter ring",
		Action = "makeing a plain pewter ring",
		ActiveCraftItemSdesc = "an in-progress plain pewter ring craft",
		Gate = CatalogueRecipeGates.MedievalJewelleryAndDevotionalCraftingGoldsmithing10,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
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
			new CatalogueCommodityRecipeInput(Grams: 8.0, Material: "pewter", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("8 grams of pewter", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "medieval_jewellery_ring_plain_pewter_ring", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a plain pewter ring (@medieval_jewellery_ring_plain_pewter_ring)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition JewelleryAndDevotionalGoodsMakeRockCrystalRing() => new()
	{
		Metadata = new("Jewellery and Devotional Goods/make rock crystal ring", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:medieval_jewellery_ring_rock_crystal_ring"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Medieval.cs:893", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make rock crystal ring",
		Category = "Jewellery and Devotional Goods",
		Blurb = "make a rock crystal ring",
		Action = "makeing a rock crystal ring",
		ActiveCraftItemSdesc = "an in-progress rock crystal ring craft",
		Gate = CatalogueRecipeGates.MedievalJewelleryAndDevotionalCraftingGoldsmithing10,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
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
			new CatalogueCommodityRecipeInput(Grams: 11.0, Material: "silver", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("11 grams of silver", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "medieval_jewellery_ring_rock_crystal_ring", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a rock crystal ring (@medieval_jewellery_ring_rock_crystal_ring)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition JewelleryAndDevotionalGoodsMakeRoseQuartzLoveRing() => new()
	{
		Metadata = new("Jewellery and Devotional Goods/make rose quartz love ring", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:medieval_jewellery_ring_rose_quartz_love_ring"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Medieval.cs:893", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make rose quartz love ring",
		Category = "Jewellery and Devotional Goods",
		Blurb = "make a rose quartz love ring",
		Action = "makeing a rose quartz love ring",
		ActiveCraftItemSdesc = "an in-progress rose quartz love ring craft",
		Gate = CatalogueRecipeGates.MedievalJewelleryAndDevotionalCraftingGoldsmithing10,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
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
			new CatalogueCommodityRecipeInput(Grams: 10.0, Material: "silver", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("10 grams of silver", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "medieval_jewellery_ring_rose_quartz_love_ring", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a rose quartz love ring (@medieval_jewellery_ring_rose_quartz_love_ring)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition JewelleryAndDevotionalGoodsMakeRubyCourtRing() => new()
	{
		Metadata = new("Jewellery and Devotional Goods/make ruby court ring", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:medieval_jewellery_ring_ruby_court_ring"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Medieval.cs:893", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make ruby court ring",
		Category = "Jewellery and Devotional Goods",
		Blurb = "make a ruby court ring",
		Action = "makeing a ruby court ring",
		ActiveCraftItemSdesc = "an in-progress ruby court ring craft",
		Gate = CatalogueRecipeGates.MedievalJewelleryAndDevotionalCraftingGoldsmithing10,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
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
			new CatalogueCommodityRecipeInput(Grams: 11.0, Material: "gold", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("11 grams of gold", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "medieval_jewellery_ring_ruby_court_ring", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a ruby court ring (@medieval_jewellery_ring_ruby_court_ring)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition JewelleryAndDevotionalGoodsMakeSapphireCourtRing() => new()
	{
		Metadata = new("Jewellery and Devotional Goods/make sapphire court ring", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:medieval_jewellery_ring_sapphire_court_ring"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Medieval.cs:893", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make sapphire court ring",
		Category = "Jewellery and Devotional Goods",
		Blurb = "make a sapphire court ring",
		Action = "makeing a sapphire court ring",
		ActiveCraftItemSdesc = "an in-progress sapphire court ring craft",
		Gate = CatalogueRecipeGates.MedievalJewelleryAndDevotionalCraftingGoldsmithing10,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
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
			new CatalogueCommodityRecipeInput(Grams: 11.0, Material: "gold", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("11 grams of gold", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "medieval_jewellery_ring_sapphire_court_ring", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a sapphire court ring (@medieval_jewellery_ring_sapphire_court_ring)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition JewelleryAndDevotionalGoodsMakeShapeGemRing() => new()
	{
		Metadata = new("Jewellery and Devotional Goods/make $shape gem ring", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:medieval_jewellery_ring_shape_gem_ring"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Medieval.cs:893", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make $shape gem ring",
		Category = "Jewellery and Devotional Goods",
		Blurb = "make a $shape gem ring",
		Action = "makeing a $shape gem ring",
		ActiveCraftItemSdesc = "an in-progress $shape gem ring craft",
		Gate = CatalogueRecipeGates.MedievalJewelleryAndDevotionalCraftingGoldsmithing10,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
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
			new CatalogueCommodityRecipeInput(Grams: 11.0, Material: "silver", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("11 grams of silver", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "medieval_jewellery_ring_shape_gem_ring", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a $shape gem ring (@medieval_jewellery_ring_shape_gem_ring)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition JewelleryAndDevotionalGoodsMakeRingSignetOfSilverGilt() => new()
	{
		Metadata = new("Jewellery and Devotional Goods/make ring signet of silver-gilt", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:medieval_jewellery_ring_signet_of_silver_gilt"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Medieval.cs:893", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make ring signet of silver-gilt",
		Category = "Jewellery and Devotional Goods",
		Blurb = "make a ring signet of silver-gilt",
		Action = "makeing a ring signet of silver-gilt",
		ActiveCraftItemSdesc = "an in-progress ring signet of silver-gilt craft",
		Gate = CatalogueRecipeGates.MedievalJewelleryAndDevotionalCraftingGoldsmithing10,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
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
			new CatalogueCommodityRecipeInput(Grams: 14.0, Material: "silver-gilt", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("14 grams of silver-gilt", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "medieval_jewellery_ring_signet_of_silver_gilt", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a ring signet of silver-gilt (@medieval_jewellery_ring_signet_of_silver_gilt)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition JewelleryAndDevotionalGoodsMakeSilverGiltFingerRing() => new()
	{
		Metadata = new("Jewellery and Devotional Goods/make silver-gilt finger ring", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:medieval_jewellery_ring_silver_gilt_finger_ring"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Medieval.cs:893", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make silver-gilt finger ring",
		Category = "Jewellery and Devotional Goods",
		Blurb = "make a silver-gilt finger ring",
		Action = "makeing a silver-gilt finger ring",
		ActiveCraftItemSdesc = "an in-progress silver-gilt finger ring craft",
		Gate = CatalogueRecipeGates.MedievalJewelleryAndDevotionalCraftingGoldsmithing10,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
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
			new CatalogueCommodityRecipeInput(Grams: 9.0, Material: "silver-gilt", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("9 grams of silver-gilt", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "medieval_jewellery_ring_silver_gilt_finger_ring", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a silver-gilt finger ring (@medieval_jewellery_ring_silver_gilt_finger_ring)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition JewelleryAndDevotionalGoodsMakeSilverWireRing() => new()
	{
		Metadata = new("Jewellery and Devotional Goods/make silver wire ring", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:medieval_jewellery_ring_silver_wire_ring"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Medieval.cs:893", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make silver wire ring",
		Category = "Jewellery and Devotional Goods",
		Blurb = "make a silver wire ring",
		Action = "makeing a silver wire ring",
		ActiveCraftItemSdesc = "an in-progress silver wire ring craft",
		Gate = CatalogueRecipeGates.MedievalJewelleryAndDevotionalCraftingGoldsmithing10,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
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
			new CatalogueCommodityRecipeInput(Grams: 7.0, Material: "silver", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("7 grams of silver", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "medieval_jewellery_ring_silver_wire_ring", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a silver wire ring (@medieval_jewellery_ring_silver_wire_ring)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition JewelleryAndDevotionalGoodsMakeSmallGoldRing() => new()
	{
		Metadata = new("Jewellery and Devotional Goods/make small gold ring", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:medieval_jewellery_ring_small_gold_ring"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Medieval.cs:893", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make small gold ring",
		Category = "Jewellery and Devotional Goods",
		Blurb = "make a small gold ring",
		Action = "makeing a small gold ring",
		ActiveCraftItemSdesc = "an in-progress small gold ring craft",
		Gate = CatalogueRecipeGates.MedievalJewelleryAndDevotionalCraftingGoldsmithing10,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
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
			new CatalogueCommodityRecipeInput(Grams: 7.0, Material: "gold", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("7 grams of gold", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "medieval_jewellery_ring_small_gold_ring", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a small gold ring (@medieval_jewellery_ring_small_gold_ring)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition JewelleryAndDevotionalGoodsMakeStampedBrassRing() => new()
	{
		Metadata = new("Jewellery and Devotional Goods/make stamped brass ring", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:medieval_jewellery_ring_stamped_brass_ring"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Medieval.cs:893", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make stamped brass ring",
		Category = "Jewellery and Devotional Goods",
		Blurb = "make a stamped brass ring",
		Action = "makeing a stamped brass ring",
		ActiveCraftItemSdesc = "an in-progress stamped brass ring craft",
		Gate = CatalogueRecipeGates.MedievalJewelleryAndDevotionalCraftingGoldsmithing10,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
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
			new CatalogueCommodityRecipeInput(Grams: 8.0, Material: "brass", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("8 grams of brass", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "medieval_jewellery_ring_stamped_brass_ring", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a stamped brass ring (@medieval_jewellery_ring_stamped_brass_ring)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition JewelleryAndDevotionalGoodsMakeSterlingSilverBand() => new()
	{
		Metadata = new("Jewellery and Devotional Goods/make sterling silver band", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:medieval_jewellery_ring_sterling_silver_band"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Medieval.cs:893", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make sterling silver band",
		Category = "Jewellery and Devotional Goods",
		Blurb = "make a sterling silver band",
		Action = "makeing a sterling silver band",
		ActiveCraftItemSdesc = "an in-progress sterling silver band craft",
		Gate = CatalogueRecipeGates.MedievalJewelleryAndDevotionalCraftingGoldsmithing10,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
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
			new CatalogueCommodityRecipeInput(Grams: 8.0, Material: "sterling silver", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("8 grams of sterling silver", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "medieval_jewellery_ring_sterling_silver_band", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a sterling silver band (@medieval_jewellery_ring_sterling_silver_band)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition JewelleryAndDevotionalGoodsMakeThinCopperRing() => new()
	{
		Metadata = new("Jewellery and Devotional Goods/make thin copper ring", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:medieval_jewellery_ring_thin_copper_ring"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Medieval.cs:893", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make thin copper ring",
		Category = "Jewellery and Devotional Goods",
		Blurb = "make a thin copper ring",
		Action = "makeing a thin copper ring",
		ActiveCraftItemSdesc = "an in-progress thin copper ring craft",
		Gate = CatalogueRecipeGates.MedievalJewelleryAndDevotionalCraftingGoldsmithing10,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
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
			new CatalogueCommodityRecipeInput(Grams: 5.0, Material: "copper", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("5 grams of copper", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "medieval_jewellery_ring_thin_copper_ring", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a thin copper ring (@medieval_jewellery_ring_thin_copper_ring)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition JewelleryAndDevotionalGoodsMakeTurquoiseBrassRing() => new()
	{
		Metadata = new("Jewellery and Devotional Goods/make turquoise brass ring", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:medieval_jewellery_ring_turquoise_brass_ring"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Medieval.cs:893", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make turquoise brass ring",
		Category = "Jewellery and Devotional Goods",
		Blurb = "make a turquoise brass ring",
		Action = "makeing a turquoise brass ring",
		ActiveCraftItemSdesc = "an in-progress turquoise brass ring craft",
		Gate = CatalogueRecipeGates.MedievalJewelleryAndDevotionalCraftingGoldsmithing10,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
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
			new CatalogueCommodityRecipeInput(Grams: 11.0, Material: "brass", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("11 grams of brass", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "medieval_jewellery_ring_turquoise_brass_ring", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a turquoise brass ring (@medieval_jewellery_ring_turquoise_brass_ring)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition JewelleryAndDevotionalGoodsMakeBellLessSilverGirdleChain() => new()
	{
		Metadata = new("Jewellery and Devotional Goods/make bell-less silver girdle chain", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:medieval_jewellery_waist_bell_less_silver_girdle_chain"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Medieval.cs:893", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make bell-less silver girdle chain",
		Category = "Jewellery and Devotional Goods",
		Blurb = "make a bell-less silver girdle chain",
		Action = "makeing a bell-less silver girdle chain",
		ActiveCraftItemSdesc = "an in-progress bell-less silver girdle chain craft",
		Gate = CatalogueRecipeGates.MedievalJewelleryAndDevotionalCraftingGoldsmithing10,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
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
			new CatalogueCommodityRecipeInput(Grams: 75.0, Material: "silver", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("75 grams of silver", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "medieval_jewellery_waist_bell_less_silver_girdle_chain", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a bell-less silver girdle chain (@medieval_jewellery_waist_bell_less_silver_girdle_chain)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition JewelleryAndDevotionalGoodsMakeBrassBeltJewel() => new()
	{
		Metadata = new("Jewellery and Devotional Goods/make brass belt jewel", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:medieval_jewellery_waist_brass_belt_jewel"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Medieval.cs:893", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make brass belt jewel",
		Category = "Jewellery and Devotional Goods",
		Blurb = "make a brass belt jewel",
		Action = "makeing a brass belt jewel",
		ActiveCraftItemSdesc = "an in-progress brass belt jewel craft",
		Gate = CatalogueRecipeGates.MedievalJewelleryAndDevotionalCraftingGoldsmithing10,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
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
			new CatalogueCommodityRecipeInput(Grams: 23.0, Material: "brass", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("23 grams of brass", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "medieval_jewellery_waist_brass_belt_jewel", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a brass belt jewel (@medieval_jewellery_waist_brass_belt_jewel)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition JewelleryAndDevotionalGoodsMakeBrassGirdleOrnament() => new()
	{
		Metadata = new("Jewellery and Devotional Goods/make brass girdle ornament", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:medieval_jewellery_waist_brass_girdle_ornament"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Medieval.cs:893", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make brass girdle ornament",
		Category = "Jewellery and Devotional Goods",
		Blurb = "make a brass girdle ornament",
		Action = "makeing a brass girdle ornament",
		ActiveCraftItemSdesc = "an in-progress brass girdle ornament craft",
		Gate = CatalogueRecipeGates.MedievalJewelleryAndDevotionalCraftingGoldsmithing10,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
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
			new CatalogueCommodityRecipeInput(Grams: 38.0, Material: "brass", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("38 grams of brass", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "medieval_jewellery_waist_brass_girdle_ornament", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a brass girdle ornament (@medieval_jewellery_waist_brass_girdle_ornament)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition JewelleryAndDevotionalGoodsMakeBrassMerchantWaistChain() => new()
	{
		Metadata = new("Jewellery and Devotional Goods/make brass merchant waist chain", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:medieval_jewellery_waist_brass_merchant_waist_chain"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Medieval.cs:893", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make brass merchant waist chain",
		Category = "Jewellery and Devotional Goods",
		Blurb = "make a brass merchant waist chain",
		Action = "makeing a brass merchant waist chain",
		ActiveCraftItemSdesc = "an in-progress brass merchant waist chain craft",
		Gate = CatalogueRecipeGates.MedievalJewelleryAndDevotionalCraftingGoldsmithing10,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
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
			new CatalogueCommodityRecipeInput(Grams: 90.0, Material: "brass", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("90 grams of brass", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "medieval_jewellery_waist_brass_merchant_waist_chain", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a brass merchant waist chain (@medieval_jewellery_waist_brass_merchant_waist_chain)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition JewelleryAndDevotionalGoodsMakeBronzeBeltOrnament() => new()
	{
		Metadata = new("Jewellery and Devotional Goods/make bronze belt ornament", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:medieval_jewellery_waist_bronze_belt_ornament"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Medieval.cs:893", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make bronze belt ornament",
		Category = "Jewellery and Devotional Goods",
		Blurb = "make a bronze belt ornament",
		Action = "makeing a bronze belt ornament",
		ActiveCraftItemSdesc = "an in-progress bronze belt ornament craft",
		Gate = CatalogueRecipeGates.MedievalJewelleryAndDevotionalCraftingGoldsmithing10,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
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
			new CatalogueCommodityRecipeInput(Grams: 24.0, Material: "bronze", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("24 grams of bronze", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "medieval_jewellery_waist_bronze_belt_ornament", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a bronze belt ornament (@medieval_jewellery_waist_bronze_belt_ornament)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition JewelleryAndDevotionalGoodsMakeBronzeWaistChain() => new()
	{
		Metadata = new("Jewellery and Devotional Goods/make bronze waist chain", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:medieval_jewellery_waist_bronze_waist_chain"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Medieval.cs:893", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make bronze waist chain",
		Category = "Jewellery and Devotional Goods",
		Blurb = "make a bronze waist chain",
		Action = "makeing a bronze waist chain",
		ActiveCraftItemSdesc = "an in-progress bronze waist chain craft",
		Gate = CatalogueRecipeGates.MedievalJewelleryAndDevotionalCraftingGoldsmithing10,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
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
			new CatalogueCommodityRecipeInput(Grams: 83.0, Material: "bronze", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("83 grams of bronze", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "medieval_jewellery_waist_bronze_waist_chain", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a bronze waist chain (@medieval_jewellery_waist_bronze_waist_chain)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition JewelleryAndDevotionalGoodsMakeCoralGirdleOrnament() => new()
	{
		Metadata = new("Jewellery and Devotional Goods/make coral girdle ornament", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:medieval_jewellery_waist_coral_girdle_ornament"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Medieval.cs:893", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make coral girdle ornament",
		Category = "Jewellery and Devotional Goods",
		Blurb = "make a coral girdle ornament",
		Action = "makeing a coral girdle ornament",
		ActiveCraftItemSdesc = "an in-progress coral girdle ornament craft",
		Gate = CatalogueRecipeGates.MedievalJewelleryAndDevotionalCraftingGoldsmithing10,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
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
			new CatalogueCommodityRecipeInput(Grams: 27.0, Material: "coral", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("27 grams of coral", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "medieval_jewellery_waist_coral_girdle_ornament", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a coral girdle ornament (@medieval_jewellery_waist_coral_girdle_ornament)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition JewelleryAndDevotionalGoodsMakeCoralHungWaistChain() => new()
	{
		Metadata = new("Jewellery and Devotional Goods/make coral-hung waist chain", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:medieval_jewellery_waist_coral_hung_waist_chain"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Medieval.cs:893", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make coral-hung waist chain",
		Category = "Jewellery and Devotional Goods",
		Blurb = "make a coral-hung waist chain",
		Action = "makeing a coral-hung waist chain",
		ActiveCraftItemSdesc = "an in-progress coral-hung waist chain craft",
		Gate = CatalogueRecipeGates.MedievalJewelleryAndDevotionalCraftingGoldsmithing10,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
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
			new CatalogueCommodityRecipeInput(Grams: 79.0, Material: "silver", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("79 grams of silver", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "medieval_jewellery_waist_coral_hung_waist_chain", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a coral-hung waist chain (@medieval_jewellery_waist_coral_hung_waist_chain)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition JewelleryAndDevotionalGoodsMakeEnamelGirdleMount() => new()
	{
		Metadata = new("Jewellery and Devotional Goods/make enamel girdle mount", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:medieval_jewellery_waist_enamel_girdle_mount"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Medieval.cs:893", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make enamel girdle mount",
		Category = "Jewellery and Devotional Goods",
		Blurb = "make an enamel girdle mount",
		Action = "makeing an enamel girdle mount",
		ActiveCraftItemSdesc = "an in-progress enamel girdle mount craft",
		Gate = CatalogueRecipeGates.MedievalJewelleryAndDevotionalCraftingGoldsmithing10,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
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
			new CatalogueCommodityRecipeInput(Grams: 32.0, Material: "enamel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("32 grams of enamel", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "medieval_jewellery_waist_enamel_girdle_mount", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x an enamel girdle mount (@medieval_jewellery_waist_enamel_girdle_mount)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition JewelleryAndDevotionalGoodsMakeFinishBronzeWaistChain() => new()
	{
		Metadata = new("Jewellery and Devotional Goods/make $finish bronze waist chain", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:medieval_jewellery_waist_finish_bronze_waist_chain"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Medieval.cs:893", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make $finish bronze waist chain",
		Category = "Jewellery and Devotional Goods",
		Blurb = "make a $finish bronze waist chain",
		Action = "makeing a $finish bronze waist chain",
		ActiveCraftItemSdesc = "an in-progress $finish bronze waist chain craft",
		Gate = CatalogueRecipeGates.MedievalJewelleryAndDevotionalCraftingGoldsmithing10,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
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
			new CatalogueCommodityRecipeInput(Grams: 83.0, Material: "bronze", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("83 grams of bronze", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "medieval_jewellery_waist_finish_bronze_waist_chain", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a $finish bronze waist chain (@medieval_jewellery_waist_finish_bronze_waist_chain)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition JewelleryAndDevotionalGoodsMakeGildedCopperBeltMount() => new()
	{
		Metadata = new("Jewellery and Devotional Goods/make gilded copper belt mount", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:medieval_jewellery_waist_gilded_copper_belt_mount"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Medieval.cs:893", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make gilded copper belt mount",
		Category = "Jewellery and Devotional Goods",
		Blurb = "make a gilded copper belt mount",
		Action = "makeing a gilded copper belt mount",
		ActiveCraftItemSdesc = "an in-progress gilded copper belt mount craft",
		Gate = CatalogueRecipeGates.MedievalJewelleryAndDevotionalCraftingGoldsmithing10,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
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
			new CatalogueCommodityRecipeInput(Grams: 26.0, Material: "gilded copper", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("26 grams of gilded copper", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "medieval_jewellery_waist_gilded_copper_belt_mount", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a gilded copper belt mount (@medieval_jewellery_waist_gilded_copper_belt_mount)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition JewelleryAndDevotionalGoodsMakeGoldGirdlePendant() => new()
	{
		Metadata = new("Jewellery and Devotional Goods/make gold girdle pendant", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:medieval_jewellery_waist_gold_girdle_pendant"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Medieval.cs:893", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make gold girdle pendant",
		Category = "Jewellery and Devotional Goods",
		Blurb = "make a gold girdle pendant",
		Action = "makeing a gold girdle pendant",
		ActiveCraftItemSdesc = "an in-progress gold girdle pendant craft",
		Gate = CatalogueRecipeGates.MedievalJewelleryAndDevotionalCraftingGoldsmithing10,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
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
			new CatalogueCommodityRecipeInput(Grams: 39.0, Material: "gold", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("39 grams of gold", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "medieval_jewellery_waist_gold_girdle_pendant", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a gold girdle pendant (@medieval_jewellery_waist_gold_girdle_pendant)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition JewelleryAndDevotionalGoodsMakeGoldWaistChain() => new()
	{
		Metadata = new("Jewellery and Devotional Goods/make gold waist chain", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:medieval_jewellery_waist_gold_waist_chain"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Medieval.cs:893", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make gold waist chain",
		Category = "Jewellery and Devotional Goods",
		Blurb = "make a gold waist chain",
		Action = "makeing a gold waist chain",
		ActiveCraftItemSdesc = "an in-progress gold waist chain craft",
		Gate = CatalogueRecipeGates.MedievalJewelleryAndDevotionalCraftingGoldsmithing10,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
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
			new CatalogueCommodityRecipeInput(Grams: 83.0, Material: "gold", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("83 grams of gold", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "medieval_jewellery_waist_gold_waist_chain", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a gold waist chain (@medieval_jewellery_waist_gold_waist_chain)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition JewelleryAndDevotionalGoodsMakeLeatherWaistOrnament() => new()
	{
		Metadata = new("Jewellery and Devotional Goods/make leather waist ornament", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:medieval_jewellery_waist_leather_waist_ornament"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Medieval.cs:893", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make leather waist ornament",
		Category = "Jewellery and Devotional Goods",
		Blurb = "make a leather waist ornament",
		Action = "makeing a leather waist ornament",
		ActiveCraftItemSdesc = "an in-progress leather waist ornament craft",
		Gate = CatalogueRecipeGates.MedievalJewelleryAndDevotionalCraftingGoldsmithing10,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
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
			new CatalogueCommodityRecipeInput(Grams: 30.0, Material: "leather", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("30 grams of leather", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "medieval_jewellery_waist_leather_waist_ornament", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a leather waist ornament (@medieval_jewellery_waist_leather_waist_ornament)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition JewelleryAndDevotionalGoodsMakeMotifGirdleOrnament() => new()
	{
		Metadata = new("Jewellery and Devotional Goods/make $motif girdle ornament", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:medieval_jewellery_waist_motif_girdle_ornament"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Medieval.cs:893", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make $motif girdle ornament",
		Category = "Jewellery and Devotional Goods",
		Blurb = "make a $motif girdle ornament",
		Action = "makeing a $motif girdle ornament",
		ActiveCraftItemSdesc = "an in-progress $motif girdle ornament craft",
		Gate = CatalogueRecipeGates.MedievalJewelleryAndDevotionalCraftingGoldsmithing10,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
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
			new CatalogueCommodityRecipeInput(Grams: 33.0, Material: "silver", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("33 grams of silver", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "medieval_jewellery_waist_motif_girdle_ornament", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a $motif girdle ornament (@medieval_jewellery_waist_motif_girdle_ornament)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition JewelleryAndDevotionalGoodsMakePewterGirdleTag() => new()
	{
		Metadata = new("Jewellery and Devotional Goods/make pewter girdle tag", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:medieval_jewellery_waist_pewter_girdle_tag"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Medieval.cs:893", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make pewter girdle tag",
		Category = "Jewellery and Devotional Goods",
		Blurb = "make a pewter girdle tag",
		Action = "makeing a pewter girdle tag",
		ActiveCraftItemSdesc = "an in-progress pewter girdle tag craft",
		Gate = CatalogueRecipeGates.MedievalJewelleryAndDevotionalCraftingGoldsmithing10,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
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
			new CatalogueCommodityRecipeInput(Grams: 27.0, Material: "pewter", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("27 grams of pewter", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "medieval_jewellery_waist_pewter_girdle_tag", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a pewter girdle tag (@medieval_jewellery_waist_pewter_girdle_tag)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition JewelleryAndDevotionalGoodsMakeSetOfBronzeBeltPlaques() => new()
	{
		Metadata = new("Jewellery and Devotional Goods/make set of bronze belt plaques", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:medieval_jewellery_waist_set_of_bronze_belt_plaques"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Medieval.cs:893", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make set of bronze belt plaques",
		Category = "Jewellery and Devotional Goods",
		Blurb = "make a set of bronze belt plaques",
		Action = "makeing a set of bronze belt plaques",
		ActiveCraftItemSdesc = "an in-progress set of bronze belt plaques craft",
		Gate = CatalogueRecipeGates.MedievalJewelleryAndDevotionalCraftingGoldsmithing10,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
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
			new CatalogueCommodityRecipeInput(Grams: 98.0, Material: "bronze", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("98 grams of bronze", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "medieval_jewellery_waist_set_of_bronze_belt_plaques", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a set of bronze belt plaques (@medieval_jewellery_waist_set_of_bronze_belt_plaques)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition JewelleryAndDevotionalGoodsMakeSetOfGildedBeltPlaques() => new()
	{
		Metadata = new("Jewellery and Devotional Goods/make set of gilded belt plaques", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:medieval_jewellery_waist_set_of_gilded_belt_plaques"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Medieval.cs:893", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make set of gilded belt plaques",
		Category = "Jewellery and Devotional Goods",
		Blurb = "make a set of gilded belt plaques",
		Action = "makeing a set of gilded belt plaques",
		ActiveCraftItemSdesc = "an in-progress set of gilded belt plaques craft",
		Gate = CatalogueRecipeGates.MedievalJewelleryAndDevotionalCraftingGoldsmithing10,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
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
			new CatalogueCommodityRecipeInput(Grams: 94.0, Material: "gilded bronze", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("94 grams of gilded bronze", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "medieval_jewellery_waist_set_of_gilded_belt_plaques", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a set of gilded belt plaques (@medieval_jewellery_waist_set_of_gilded_belt_plaques)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition JewelleryAndDevotionalGoodsMakeSetOfSilverBeltPlaques() => new()
	{
		Metadata = new("Jewellery and Devotional Goods/make set of silver belt plaques", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:medieval_jewellery_waist_set_of_silver_belt_plaques"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Medieval.cs:893", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make set of silver belt plaques",
		Category = "Jewellery and Devotional Goods",
		Blurb = "make a set of silver belt plaques",
		Action = "makeing a set of silver belt plaques",
		ActiveCraftItemSdesc = "an in-progress set of silver belt plaques craft",
		Gate = CatalogueRecipeGates.MedievalJewelleryAndDevotionalCraftingGoldsmithing10,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
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
			new CatalogueCommodityRecipeInput(Grams: 90.0, Material: "silver", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("90 grams of silver", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "medieval_jewellery_waist_set_of_silver_belt_plaques", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a set of silver belt plaques (@medieval_jewellery_waist_set_of_silver_belt_plaques)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition JewelleryAndDevotionalGoodsMakeShapeBeltOrnament() => new()
	{
		Metadata = new("Jewellery and Devotional Goods/make $shape belt ornament", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:medieval_jewellery_waist_shape_belt_ornament"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Medieval.cs:893", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make $shape belt ornament",
		Category = "Jewellery and Devotional Goods",
		Blurb = "make a $shape belt ornament",
		Action = "makeing a $shape belt ornament",
		ActiveCraftItemSdesc = "an in-progress $shape belt ornament craft",
		Gate = CatalogueRecipeGates.MedievalJewelleryAndDevotionalCraftingGoldsmithing10,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
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
			new CatalogueCommodityRecipeInput(Grams: 27.0, Material: "silver-gilt", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("27 grams of silver-gilt", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "medieval_jewellery_waist_shape_belt_ornament", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a $shape belt ornament (@medieval_jewellery_waist_shape_belt_ornament)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition JewelleryAndDevotionalGoodsMakeSilkTiedWaistChain() => new()
	{
		Metadata = new("Jewellery and Devotional Goods/make silk-tied waist chain", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:medieval_jewellery_waist_silk_tied_waist_chain"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Medieval.cs:893", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make silk-tied waist chain",
		Category = "Jewellery and Devotional Goods",
		Blurb = "make a silk-tied waist chain",
		Action = "makeing a silk-tied waist chain",
		ActiveCraftItemSdesc = "an in-progress silk-tied waist chain craft",
		Gate = CatalogueRecipeGates.MedievalJewelleryAndDevotionalCraftingGoldsmithing10,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
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
			new CatalogueCommodityRecipeInput(Grams: 26.0, Material: "silk", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("26 grams of silk", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "medieval_jewellery_waist_silk_tied_waist_chain", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a silk-tied waist chain (@medieval_jewellery_waist_silk_tied_waist_chain)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};
}
