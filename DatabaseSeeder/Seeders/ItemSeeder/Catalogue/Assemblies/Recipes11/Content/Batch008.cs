#nullable enable

using MudSharp.Body;
using MudSharp.GameItems.Inventory.Plans;
using MudSharp.RPG.Checks;

namespace DatabaseSeeder.Seeders;

public static partial class Recipes11Stock
{
	public static CatalogueRecipeDefinition JewelleryAndDevotionalWorkMakeVineBrassDiadem() => new()
	{
		Metadata = new("Jewellery and devotional work/make vine brass diadem", "crafts", ["earlymodern", "renaissance"], ["item:earlymodern_jewellery_mag_diadem_214"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make vine brass diadem",
		Category = "Jewellery and devotional work",
		Blurb = "make a vine brass diadem",
		Action = "making jewellery or a devotional item",
		ActiveCraftItemSdesc = "an in-progress vine brass diadem craft",
		Gate = CatalogueRecipeGates.RenaissanceAndEarlyModernJewellerySilversmithing15,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 3,
		Interruptable = false,
		Phases = [
			new(25, "$0 select|selects $i1 and begin|begins to lay out the work with $t1.", "$0 select|selects $i1 and begin|begins to lay out the work with $t1.", ExertionLevel.Stasis, 0),
			new(35, "$0 shape|shapes the material with $t1 and $t2, checking the joins and fit as the work develops.", "$0 shape|shapes the material with $t1 and $t2, checking the joins and fit as the work develops.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the visible form and set|sets aside $p1 for inspection.", "$0 end|ends the unsuccessful work and set|sets the spoiled material aside.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 38.0, Material: "brass", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("38 grams of brass", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Pliers", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Pliers tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "earlymodern_jewellery_mag_diadem_214", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a vine brass diadem (@earlymodern_jewellery_mag_diadem_214)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition JewelleryAndDevotionalWorkMakeWaterWavePewterEarrings() => new()
	{
		Metadata = new("Jewellery and devotional work/make water-wave pewter earrings", "crafts", ["earlymodern", "renaissance"], ["item:earlymodern_jewellery_mag_earrings_209"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make water-wave pewter earrings",
		Category = "Jewellery and devotional work",
		Blurb = "make a water-wave pewter earrings",
		Action = "making jewellery or a devotional item",
		ActiveCraftItemSdesc = "an in-progress water-wave pewter earrings craft",
		Gate = CatalogueRecipeGates.RenaissanceAndEarlyModernJewellerySilversmithing15,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 3,
		Interruptable = false,
		Phases = [
			new(25, "$0 select|selects $i1 and begin|begins to lay out the work with $t1.", "$0 select|selects $i1 and begin|begins to lay out the work with $t1.", ExertionLevel.Stasis, 0),
			new(35, "$0 shape|shapes the material with $t1 and $t2, checking the joins and fit as the work develops.", "$0 shape|shapes the material with $t1 and $t2, checking the joins and fit as the work develops.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the visible form and set|sets aside $p1 for inspection.", "$0 end|ends the unsuccessful work and set|sets the spoiled material aside.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 72.0, Material: "pewter", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("72 grams of pewter", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Pliers", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Pliers tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "earlymodern_jewellery_mag_earrings_209", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a water-wave pewter earrings (@earlymodern_jewellery_mag_earrings_209)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition JewelleryAndDevotionalWorkMakeStarSilverHairpin() => new()
	{
		Metadata = new("Jewellery and devotional work/make star silver hairpin", "crafts", ["earlymodern", "renaissance"], ["item:earlymodern_jewellery_mag_hairpin_212"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make star silver hairpin",
		Category = "Jewellery and devotional work",
		Blurb = "make a star silver hairpin",
		Action = "making jewellery or a devotional item",
		ActiveCraftItemSdesc = "an in-progress star silver hairpin craft",
		Gate = CatalogueRecipeGates.RenaissanceAndEarlyModernJewellerySilversmithing15,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 3,
		Interruptable = false,
		Phases = [
			new(25, "$0 select|selects $i1 and begin|begins to lay out the work with $t1.", "$0 select|selects $i1 and begin|begins to lay out the work with $t1.", ExertionLevel.Stasis, 0),
			new(35, "$0 shape|shapes the material with $t1 and $t2, checking the joins and fit as the work develops.", "$0 shape|shapes the material with $t1 and $t2, checking the joins and fit as the work develops.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the visible form and set|sets aside $p1 for inspection.", "$0 end|ends the unsuccessful work and set|sets the spoiled material aside.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 27.0, Material: "silver", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("27 grams of silver", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Pliers", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Pliers tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "earlymodern_jewellery_mag_hairpin_212", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a star silver hairpin (@earlymodern_jewellery_mag_hairpin_212)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition JewelleryAndDevotionalWorkMakeRosetteCarnelianPendant() => new()
	{
		Metadata = new("Jewellery and devotional work/make rosette carnelian pendant", "crafts", ["earlymodern", "renaissance"], ["item:earlymodern_jewellery_mag_pendant_203"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make rosette carnelian pendant",
		Category = "Jewellery and devotional work",
		Blurb = "make a rosette carnelian pendant",
		Action = "making jewellery or a devotional item",
		ActiveCraftItemSdesc = "an in-progress rosette carnelian pendant craft",
		Gate = CatalogueRecipeGates.RenaissanceAndEarlyModernJewelleryGemcraft15,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 3,
		Interruptable = false,
		Phases = [
			new(25, "$0 select|selects $i1 and begin|begins to lay out the work with $t1.", "$0 select|selects $i1 and begin|begins to lay out the work with $t1.", ExertionLevel.Stasis, 0),
			new(35, "$0 shape|shapes the material with $t1 and $t2, checking the joins and fit as the work develops.", "$0 shape|shapes the material with $t1 and $t2, checking the joins and fit as the work develops.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the visible form and set|sets aside $p1 for inspection.", "$0 end|ends the unsuccessful work and set|sets the spoiled material aside.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 38.0, Material: "carnelian", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("38 grams of carnelian", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Pliers", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Pliers tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "earlymodern_jewellery_mag_pendant_203", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a rosette carnelian pendant (@earlymodern_jewellery_mag_pendant_203)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition JewelleryAndDevotionalWorkMakeFlowerHeadBronzePin() => new()
	{
		Metadata = new("Jewellery and devotional work/make flower-head bronze pin", "crafts", ["earlymodern", "renaissance"], ["item:earlymodern_jewellery_mag_pin_211"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make flower-head bronze pin",
		Category = "Jewellery and devotional work",
		Blurb = "make a flower-head bronze pin",
		Action = "making jewellery or a devotional item",
		ActiveCraftItemSdesc = "an in-progress flower-head bronze pin craft",
		Gate = CatalogueRecipeGates.RenaissanceAndEarlyModernJewellerySilversmithing15,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 3,
		Interruptable = false,
		Phases = [
			new(25, "$0 select|selects $i1 and begin|begins to lay out the work with $t1.", "$0 select|selects $i1 and begin|begins to lay out the work with $t1.", ExertionLevel.Stasis, 0),
			new(35, "$0 shape|shapes the material with $t1 and $t2, checking the joins and fit as the work develops.", "$0 shape|shapes the material with $t1 and $t2, checking the joins and fit as the work develops.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the visible form and set|sets aside $p1 for inspection.", "$0 end|ends the unsuccessful work and set|sets the spoiled material aside.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 22.0, Material: "bronze", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("22 grams of bronze", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Pliers", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Pliers tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "earlymodern_jewellery_mag_pin_211", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a flower-head bronze pin (@earlymodern_jewellery_mag_pin_211)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition JewelleryAndDevotionalWorkMakeShellGarnetRing() => new()
	{
		Metadata = new("Jewellery and devotional work/make shell garnet ring", "crafts", ["earlymodern", "renaissance"], ["item:earlymodern_jewellery_mag_ring_201"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make shell garnet ring",
		Category = "Jewellery and devotional work",
		Blurb = "make a shell garnet ring",
		Action = "making jewellery or a devotional item",
		ActiveCraftItemSdesc = "an in-progress shell garnet ring craft",
		Gate = CatalogueRecipeGates.RenaissanceAndEarlyModernJewelleryGemcraft15,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 3,
		Interruptable = false,
		Phases = [
			new(25, "$0 select|selects $i1 and begin|begins to lay out the work with $t1.", "$0 select|selects $i1 and begin|begins to lay out the work with $t1.", ExertionLevel.Stasis, 0),
			new(35, "$0 shape|shapes the material with $t1 and $t2, checking the joins and fit as the work develops.", "$0 shape|shapes the material with $t1 and $t2, checking the joins and fit as the work develops.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the visible form and set|sets aside $p1 for inspection.", "$0 end|ends the unsuccessful work and set|sets the spoiled material aside.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 27.0, Material: "garnet", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("27 grams of garnet", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Pliers", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Pliers tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "earlymodern_jewellery_mag_ring_201", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a shell garnet ring (@earlymodern_jewellery_mag_ring_201)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition JewelleryAndDevotionalWorkMakeReedAgateRing() => new()
	{
		Metadata = new("Jewellery and devotional work/make reed agate ring", "crafts", ["earlymodern", "renaissance"], ["item:earlymodern_jewellery_mag_signet_ring_202"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make reed agate ring",
		Category = "Jewellery and devotional work",
		Blurb = "make a reed agate ring",
		Action = "making jewellery or a devotional item",
		ActiveCraftItemSdesc = "an in-progress reed agate ring craft",
		Gate = CatalogueRecipeGates.RenaissanceAndEarlyModernJewelleryGemcraft15,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 3,
		Interruptable = false,
		Phases = [
			new(25, "$0 select|selects $i1 and begin|begins to lay out the work with $t1.", "$0 select|selects $i1 and begin|begins to lay out the work with $t1.", ExertionLevel.Stasis, 0),
			new(35, "$0 shape|shapes the material with $t1 and $t2, checking the joins and fit as the work develops.", "$0 shape|shapes the material with $t1 and $t2, checking the joins and fit as the work develops.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the visible form and set|sets aside $p1 for inspection.", "$0 end|ends the unsuccessful work and set|sets the spoiled material aside.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 33.0, Material: "agate", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("33 grams of agate", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Pliers", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Pliers tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "earlymodern_jewellery_mag_signet_ring_202", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a reed agate ring (@earlymodern_jewellery_mag_signet_ring_202)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition JewelleryAndDevotionalWorkMakeReedIvoryTempleRing() => new()
	{
		Metadata = new("Jewellery and devotional work/make reed ivory temple ring", "crafts", ["earlymodern", "renaissance"], ["item:earlymodern_jewellery_mag_temple_ring_218"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make reed ivory temple ring",
		Category = "Jewellery and devotional work",
		Blurb = "make a reed ivory temple ring",
		Action = "making jewellery or a devotional item",
		ActiveCraftItemSdesc = "an in-progress reed ivory temple ring craft",
		Gate = CatalogueRecipeGates.RenaissanceAndEarlyModernJewelleryScrimshawing15,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 3,
		Interruptable = false,
		Phases = [
			new(25, "$0 select|selects $i1 and begin|begins to lay out the work with $t1.", "$0 select|selects $i1 and begin|begins to lay out the work with $t1.", ExertionLevel.Stasis, 0),
			new(35, "$0 shape|shapes the material with $t1 and $t2, checking the joins and fit as the work develops.", "$0 shape|shapes the material with $t1 and $t2, checking the joins and fit as the work develops.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the visible form and set|sets aside $p1 for inspection.", "$0 end|ends the unsuccessful work and set|sets the spoiled material aside.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 61.0, Material: "ivory", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("61 grams of ivory", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Pliers", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Pliers tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "earlymodern_jewellery_mag_temple_ring_218", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a reed ivory temple ring (@earlymodern_jewellery_mag_temple_ring_218)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition JewelleryAndDevotionalWorkMakeRosettePearlTorc() => new()
	{
		Metadata = new("Jewellery and devotional work/make rosette pearl torc", "crafts", ["earlymodern", "renaissance"], ["item:earlymodern_jewellery_mag_torc_219"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make rosette pearl torc",
		Category = "Jewellery and devotional work",
		Blurb = "make a rosette pearl torc",
		Action = "making jewellery or a devotional item",
		ActiveCraftItemSdesc = "an in-progress rosette pearl torc craft",
		Gate = CatalogueRecipeGates.RenaissanceAndEarlyModernJewelleryGemcraft15,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 3,
		Interruptable = false,
		Phases = [
			new(25, "$0 select|selects $i1 and begin|begins to lay out the work with $t1.", "$0 select|selects $i1 and begin|begins to lay out the work with $t1.", ExertionLevel.Stasis, 0),
			new(35, "$0 shape|shapes the material with $t1 and $t2, checking the joins and fit as the work develops.", "$0 shape|shapes the material with $t1 and $t2, checking the joins and fit as the work develops.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the visible form and set|sets aside $p1 for inspection.", "$0 end|ends the unsuccessful work and set|sets the spoiled material aside.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 66.0, Material: "pearl", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("66 grams of pearl", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Pliers", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Pliers tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "earlymodern_jewellery_mag_torc_219", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a rosette pearl torc (@earlymodern_jewellery_mag_torc_219)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition JewelleryAndDevotionalWorkMakeGeometricGlassWaistChain() => new()
	{
		Metadata = new("Jewellery and devotional work/make geometric glass waist chain", "crafts", ["earlymodern", "renaissance"], ["item:earlymodern_jewellery_mag_waist_chain_215"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make geometric glass waist chain",
		Category = "Jewellery and devotional work",
		Blurb = "make a geometric glass waist chain",
		Action = "making jewellery or a devotional item",
		ActiveCraftItemSdesc = "an in-progress geometric glass waist chain craft",
		Gate = CatalogueRecipeGates.RenaissanceAndEarlyModernJewelleryGlassworking15,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 3,
		Interruptable = false,
		Phases = [
			new(25, "$0 select|selects $i1 and begin|begins to lay out the work with $t1.", "$0 select|selects $i1 and begin|begins to lay out the work with $t1.", ExertionLevel.Stasis, 0),
			new(35, "$0 shape|shapes the material with $t1 and $t2, checking the joins and fit as the work develops.", "$0 shape|shapes the material with $t1 and $t2, checking the joins and fit as the work develops.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the visible form and set|sets aside $p1 for inspection.", "$0 end|ends the unsuccessful work and set|sets the spoiled material aside.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 44.0, Material: "glass", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("44 grams of glass", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Pliers", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Pliers tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "earlymodern_jewellery_mag_waist_chain_215", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a geometric glass waist chain (@earlymodern_jewellery_mag_waist_chain_215)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition JewelleryAndDevotionalWorkMakeCheckerAmberWreath() => new()
	{
		Metadata = new("Jewellery and devotional work/make checker amber wreath", "crafts", ["earlymodern", "renaissance"], ["item:earlymodern_jewellery_mag_wreath_220"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make checker amber wreath",
		Category = "Jewellery and devotional work",
		Blurb = "make a checker amber wreath",
		Action = "making jewellery or a devotional item",
		ActiveCraftItemSdesc = "an in-progress checker amber wreath craft",
		Gate = CatalogueRecipeGates.RenaissanceAndEarlyModernJewelleryGemcraft15,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 3,
		Interruptable = false,
		Phases = [
			new(25, "$0 select|selects $i1 and begin|begins to lay out the work with $t1.", "$0 select|selects $i1 and begin|begins to lay out the work with $t1.", ExertionLevel.Stasis, 0),
			new(35, "$0 shape|shapes the material with $t1 and $t2, checking the joins and fit as the work develops.", "$0 shape|shapes the material with $t1 and $t2, checking the joins and fit as the work develops.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the visible form and set|sets aside $p1 for inspection.", "$0 end|ends the unsuccessful work and set|sets the spoiled material aside.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 72.0, Material: "amber", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("72 grams of amber", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Pliers", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Pliers tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "earlymodern_jewellery_mag_wreath_220", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a checker amber wreath (@earlymodern_jewellery_mag_wreath_220)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition JewelleryAndDevotionalWorkMakeVineShellAnklet() => new()
	{
		Metadata = new("Jewellery and devotional work/make vine shell anklet", "crafts", ["earlymodern", "renaissance"], ["item:earlymodern_jewellery_mca_anklet_648"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make vine shell anklet",
		Category = "Jewellery and devotional work",
		Blurb = "make a vine shell anklet",
		Action = "making jewellery or a devotional item",
		ActiveCraftItemSdesc = "an in-progress vine shell anklet craft",
		Gate = CatalogueRecipeGates.RenaissanceAndEarlyModernJewelleryScrimshawing15,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 3,
		Interruptable = false,
		Phases = [
			new(25, "$0 select|selects $i1 and begin|begins to lay out the work with $t1.", "$0 select|selects $i1 and begin|begins to lay out the work with $t1.", ExertionLevel.Stasis, 0),
			new(35, "$0 shape|shapes the material with $t1 and $t2, checking the joins and fit as the work develops.", "$0 shape|shapes the material with $t1 and $t2, checking the joins and fit as the work develops.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the visible form and set|sets aside $p1 for inspection.", "$0 end|ends the unsuccessful work and set|sets the spoiled material aside.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 66.0, Material: "shell", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("66 grams of shell", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Pliers", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Pliers tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "earlymodern_jewellery_mca_anklet_648", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a vine shell anklet (@earlymodern_jewellery_mca_anklet_648)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition JewelleryAndDevotionalWorkMakeBirdBoneArmlet() => new()
	{
		Metadata = new("Jewellery and devotional work/make bird bone armlet", "crafts", ["earlymodern", "renaissance"], ["item:earlymodern_jewellery_mca_armlet_647"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make bird bone armlet",
		Category = "Jewellery and devotional work",
		Blurb = "make a bird bone armlet",
		Action = "making jewellery or a devotional item",
		ActiveCraftItemSdesc = "an in-progress bird bone armlet craft",
		Gate = CatalogueRecipeGates.RenaissanceAndEarlyModernJewelleryScrimshawing15,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 3,
		Interruptable = false,
		Phases = [
			new(25, "$0 select|selects $i1 and begin|begins to lay out the work with $t1.", "$0 select|selects $i1 and begin|begins to lay out the work with $t1.", ExertionLevel.Stasis, 0),
			new(35, "$0 shape|shapes the material with $t1 and $t2, checking the joins and fit as the work develops.", "$0 shape|shapes the material with $t1 and $t2, checking the joins and fit as the work develops.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the visible form and set|sets aside $p1 for inspection.", "$0 end|ends the unsuccessful work and set|sets the spoiled material aside.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 61.0, Material: "bone", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("61 grams of bone", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Pliers", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Pliers tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "earlymodern_jewellery_mca_armlet_647", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a bird bone armlet (@earlymodern_jewellery_mca_armlet_647)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition JewelleryAndDevotionalWorkMakeLeafScrollTurquoiseBadge() => new()
	{
		Metadata = new("Jewellery and devotional work/make leaf-scroll turquoise badge", "crafts", ["earlymodern", "renaissance"], ["item:earlymodern_jewellery_mca_badge_657"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make leaf-scroll turquoise badge",
		Category = "Jewellery and devotional work",
		Blurb = "make a leaf-scroll turquoise badge",
		Action = "making jewellery or a devotional item",
		ActiveCraftItemSdesc = "an in-progress leaf-scroll turquoise badge craft",
		Gate = CatalogueRecipeGates.RenaissanceAndEarlyModernJewelleryGemcraft15,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 3,
		Interruptable = false,
		Phases = [
			new(25, "$0 select|selects $i1 and begin|begins to lay out the work with $t1.", "$0 select|selects $i1 and begin|begins to lay out the work with $t1.", ExertionLevel.Stasis, 0),
			new(35, "$0 shape|shapes the material with $t1 and $t2, checking the joins and fit as the work develops.", "$0 shape|shapes the material with $t1 and $t2, checking the joins and fit as the work develops.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the visible form and set|sets aside $p1 for inspection.", "$0 end|ends the unsuccessful work and set|sets the spoiled material aside.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 55.0, Material: "turquoise", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("55 grams of turquoise", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Pliers", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Pliers tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "earlymodern_jewellery_mca_badge_657", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a leaf-scroll turquoise badge (@earlymodern_jewellery_mca_badge_657)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition JewelleryAndDevotionalWorkMakeKnottedGoldNecklace() => new()
	{
		Metadata = new("Jewellery and devotional work/make knotted gold necklace", "crafts", ["earlymodern", "renaissance"], ["item:earlymodern_jewellery_mca_bead_necklace_644"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make knotted gold necklace",
		Category = "Jewellery and devotional work",
		Blurb = "make a knotted gold necklace",
		Action = "making jewellery or a devotional item",
		ActiveCraftItemSdesc = "an in-progress knotted gold necklace craft",
		Gate = CatalogueRecipeGates.RenaissanceAndEarlyModernJewellerySilversmithing15,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 3,
		Interruptable = false,
		Phases = [
			new(25, "$0 select|selects $i1 and begin|begins to lay out the work with $t1.", "$0 select|selects $i1 and begin|begins to lay out the work with $t1.", ExertionLevel.Stasis, 0),
			new(35, "$0 shape|shapes the material with $t1 and $t2, checking the joins and fit as the work develops.", "$0 shape|shapes the material with $t1 and $t2, checking the joins and fit as the work develops.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the visible form and set|sets aside $p1 for inspection.", "$0 end|ends the unsuccessful work and set|sets the spoiled material aside.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 44.0, Material: "gold", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("44 grams of gold", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Pliers", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Pliers tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "earlymodern_jewellery_mca_bead_necklace_644", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a knotted gold necklace (@earlymodern_jewellery_mca_bead_necklace_644)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition JewelleryAndDevotionalWorkMakeRadiatingLapisLazuliBeltOrnament() => new()
	{
		Metadata = new("Jewellery and devotional work/make radiating lapis lazuli belt ornament", "crafts", ["earlymodern", "renaissance"], ["item:earlymodern_jewellery_mca_belt_ornament_656"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make radiating lapis lazuli belt ornament",
		Category = "Jewellery and devotional work",
		Blurb = "make a radiating lapis lazuli belt ornament",
		Action = "making jewellery or a devotional item",
		ActiveCraftItemSdesc = "an in-progress radiating lapis lazuli belt ornament craft",
		Gate = CatalogueRecipeGates.RenaissanceAndEarlyModernJewelleryGemcraft15,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 3,
		Interruptable = false,
		Phases = [
			new(25, "$0 select|selects $i1 and begin|begins to lay out the work with $t1.", "$0 select|selects $i1 and begin|begins to lay out the work with $t1.", ExertionLevel.Stasis, 0),
			new(35, "$0 shape|shapes the material with $t1 and $t2, checking the joins and fit as the work develops.", "$0 shape|shapes the material with $t1 and $t2, checking the joins and fit as the work develops.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the visible form and set|sets aside $p1 for inspection.", "$0 end|ends the unsuccessful work and set|sets the spoiled material aside.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 50.0, Material: "lapis lazuli", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("50 grams of lapis lazuli", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Pliers", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Pliers tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "earlymodern_jewellery_mca_belt_ornament_656", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a radiating lapis lazuli belt ornament (@earlymodern_jewellery_mca_belt_ornament_656)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition JewelleryAndDevotionalWorkMakeStarGlassBracelet() => new()
	{
		Metadata = new("Jewellery and devotional work/make star glass bracelet", "crafts", ["earlymodern", "renaissance"], ["item:earlymodern_jewellery_mca_bracelet_646"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make star glass bracelet",
		Category = "Jewellery and devotional work",
		Blurb = "make a star glass bracelet",
		Action = "making jewellery or a devotional item",
		ActiveCraftItemSdesc = "an in-progress star glass bracelet craft",
		Gate = CatalogueRecipeGates.RenaissanceAndEarlyModernJewelleryGlassworking15,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 3,
		Interruptable = false,
		Phases = [
			new(25, "$0 select|selects $i1 and begin|begins to lay out the work with $t1.", "$0 select|selects $i1 and begin|begins to lay out the work with $t1.", ExertionLevel.Stasis, 0),
			new(35, "$0 shape|shapes the material with $t1 and $t2, checking the joins and fit as the work develops.", "$0 shape|shapes the material with $t1 and $t2, checking the joins and fit as the work develops.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the visible form and set|sets aside $p1 for inspection.", "$0 end|ends the unsuccessful work and set|sets the spoiled material aside.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 55.0, Material: "glass", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("55 grams of glass", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Pliers", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Pliers tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "earlymodern_jewellery_mca_bracelet_646", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a star glass bracelet (@earlymodern_jewellery_mca_bracelet_646)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition JewelleryAndDevotionalWorkMakeCloudPearlBrooch() => new()
	{
		Metadata = new("Jewellery and devotional work/make cloud pearl brooch", "crafts", ["earlymodern", "renaissance"], ["item:earlymodern_jewellery_mca_brooch_650"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make cloud pearl brooch",
		Category = "Jewellery and devotional work",
		Blurb = "make a cloud pearl brooch",
		Action = "making jewellery or a devotional item",
		ActiveCraftItemSdesc = "an in-progress cloud pearl brooch craft",
		Gate = CatalogueRecipeGates.RenaissanceAndEarlyModernJewelleryGemcraft15,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 3,
		Interruptable = false,
		Phases = [
			new(25, "$0 select|selects $i1 and begin|begins to lay out the work with $t1.", "$0 select|selects $i1 and begin|begins to lay out the work with $t1.", ExertionLevel.Stasis, 0),
			new(35, "$0 shape|shapes the material with $t1 and $t2, checking the joins and fit as the work develops.", "$0 shape|shapes the material with $t1 and $t2, checking the joins and fit as the work develops.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the visible form and set|sets aside $p1 for inspection.", "$0 end|ends the unsuccessful work and set|sets the spoiled material aside.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 16.0, Material: "pearl", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("16 grams of pearl", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Pliers", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Pliers tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "earlymodern_jewellery_mca_brooch_650", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a cloud pearl brooch (@earlymodern_jewellery_mca_brooch_650)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition JewelleryAndDevotionalWorkMakeFlowerHeadBrassNecklace() => new()
	{
		Metadata = new("Jewellery and devotional work/make flower-head brass necklace", "crafts", ["earlymodern", "renaissance"], ["item:earlymodern_jewellery_mca_chain_necklace_645"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make flower-head brass necklace",
		Category = "Jewellery and devotional work",
		Blurb = "make a flower-head brass necklace",
		Action = "making jewellery or a devotional item",
		ActiveCraftItemSdesc = "an in-progress flower-head brass necklace craft",
		Gate = CatalogueRecipeGates.RenaissanceAndEarlyModernJewellerySilversmithing15,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 3,
		Interruptable = false,
		Phases = [
			new(25, "$0 select|selects $i1 and begin|begins to lay out the work with $t1.", "$0 select|selects $i1 and begin|begins to lay out the work with $t1.", ExertionLevel.Stasis, 0),
			new(35, "$0 shape|shapes the material with $t1 and $t2, checking the joins and fit as the work develops.", "$0 shape|shapes the material with $t1 and $t2, checking the joins and fit as the work develops.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the visible form and set|sets aside $p1 for inspection.", "$0 end|ends the unsuccessful work and set|sets the spoiled material aside.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 50.0, Material: "brass", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("50 grams of brass", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Pliers", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Pliers tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "earlymodern_jewellery_mca_chain_necklace_645", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a flower-head brass necklace (@earlymodern_jewellery_mca_chain_necklace_645)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition JewelleryAndDevotionalWorkMakeRosetteGarnetCirclet() => new()
	{
		Metadata = new("Jewellery and devotional work/make rosette garnet circlet", "crafts", ["earlymodern", "renaissance"], ["item:earlymodern_jewellery_mca_circlet_653"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make rosette garnet circlet",
		Category = "Jewellery and devotional work",
		Blurb = "make a rosette garnet circlet",
		Action = "making jewellery or a devotional item",
		ActiveCraftItemSdesc = "an in-progress rosette garnet circlet craft",
		Gate = CatalogueRecipeGates.RenaissanceAndEarlyModernJewelleryGemcraft15,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 3,
		Interruptable = false,
		Phases = [
			new(25, "$0 select|selects $i1 and begin|begins to lay out the work with $t1.", "$0 select|selects $i1 and begin|begins to lay out the work with $t1.", ExertionLevel.Stasis, 0),
			new(35, "$0 shape|shapes the material with $t1 and $t2, checking the joins and fit as the work develops.", "$0 shape|shapes the material with $t1 and $t2, checking the joins and fit as the work develops.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the visible form and set|sets aside $p1 for inspection.", "$0 end|ends the unsuccessful work and set|sets the spoiled material aside.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 33.0, Material: "garnet", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("33 grams of garnet", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Pliers", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Pliers tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "earlymodern_jewellery_mca_circlet_653", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a rosette garnet circlet (@earlymodern_jewellery_mca_circlet_653)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition JewelleryAndDevotionalWorkMakeCheckerAgateDiadem() => new()
	{
		Metadata = new("Jewellery and devotional work/make checker agate diadem", "crafts", ["earlymodern", "renaissance"], ["item:earlymodern_jewellery_mca_diadem_654"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make checker agate diadem",
		Category = "Jewellery and devotional work",
		Blurb = "make a checker agate diadem",
		Action = "making jewellery or a devotional item",
		ActiveCraftItemSdesc = "an in-progress checker agate diadem craft",
		Gate = CatalogueRecipeGates.RenaissanceAndEarlyModernJewelleryGemcraft15,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 3,
		Interruptable = false,
		Phases = [
			new(25, "$0 select|selects $i1 and begin|begins to lay out the work with $t1.", "$0 select|selects $i1 and begin|begins to lay out the work with $t1.", ExertionLevel.Stasis, 0),
			new(35, "$0 shape|shapes the material with $t1 and $t2, checking the joins and fit as the work develops.", "$0 shape|shapes the material with $t1 and $t2, checking the joins and fit as the work develops.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the visible form and set|sets aside $p1 for inspection.", "$0 end|ends the unsuccessful work and set|sets the spoiled material aside.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 38.0, Material: "agate", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("38 grams of agate", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Pliers", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Pliers tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "earlymodern_jewellery_mca_diadem_654", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a checker agate diadem (@earlymodern_jewellery_mca_diadem_654)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition JewelleryAndDevotionalWorkMakeGeometricIvoryEarrings() => new()
	{
		Metadata = new("Jewellery and devotional work/make geometric ivory earrings", "crafts", ["earlymodern", "renaissance"], ["item:earlymodern_jewellery_mca_earrings_649"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make geometric ivory earrings",
		Category = "Jewellery and devotional work",
		Blurb = "make a geometric ivory earrings",
		Action = "making jewellery or a devotional item",
		ActiveCraftItemSdesc = "an in-progress geometric ivory earrings craft",
		Gate = CatalogueRecipeGates.RenaissanceAndEarlyModernJewelleryScrimshawing15,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 3,
		Interruptable = false,
		Phases = [
			new(25, "$0 select|selects $i1 and begin|begins to lay out the work with $t1.", "$0 select|selects $i1 and begin|begins to lay out the work with $t1.", ExertionLevel.Stasis, 0),
			new(35, "$0 shape|shapes the material with $t1 and $t2, checking the joins and fit as the work develops.", "$0 shape|shapes the material with $t1 and $t2, checking the joins and fit as the work develops.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the visible form and set|sets aside $p1 for inspection.", "$0 end|ends the unsuccessful work and set|sets the spoiled material aside.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 72.0, Material: "ivory", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("72 grams of ivory", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Pliers", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Pliers tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "earlymodern_jewellery_mca_earrings_649", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a geometric ivory earrings (@earlymodern_jewellery_mca_earrings_649)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition JewelleryAndDevotionalWorkMakeReedCoralHairpin() => new()
	{
		Metadata = new("Jewellery and devotional work/make reed coral hairpin", "crafts", ["earlymodern", "renaissance"], ["item:earlymodern_jewellery_mca_hairpin_652"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make reed coral hairpin",
		Category = "Jewellery and devotional work",
		Blurb = "make a reed coral hairpin",
		Action = "making jewellery or a devotional item",
		ActiveCraftItemSdesc = "an in-progress reed coral hairpin craft",
		Gate = CatalogueRecipeGates.RenaissanceAndEarlyModernJewelleryScrimshawing15,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 3,
		Interruptable = false,
		Phases = [
			new(25, "$0 select|selects $i1 and begin|begins to lay out the work with $t1.", "$0 select|selects $i1 and begin|begins to lay out the work with $t1.", ExertionLevel.Stasis, 0),
			new(35, "$0 shape|shapes the material with $t1 and $t2, checking the joins and fit as the work develops.", "$0 shape|shapes the material with $t1 and $t2, checking the joins and fit as the work develops.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the visible form and set|sets aside $p1 for inspection.", "$0 end|ends the unsuccessful work and set|sets the spoiled material aside.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 27.0, Material: "coral", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("27 grams of coral", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Pliers", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Pliers tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "earlymodern_jewellery_mca_hairpin_652", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a reed coral hairpin (@earlymodern_jewellery_mca_hairpin_652)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition JewelleryAndDevotionalWorkMakeWaterWaveSilverPendant() => new()
	{
		Metadata = new("Jewellery and devotional work/make water-wave silver pendant", "crafts", ["earlymodern", "renaissance"], ["item:earlymodern_jewellery_mca_pendant_643"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make water-wave silver pendant",
		Category = "Jewellery and devotional work",
		Blurb = "make a water-wave silver pendant",
		Action = "making jewellery or a devotional item",
		ActiveCraftItemSdesc = "an in-progress water-wave silver pendant craft",
		Gate = CatalogueRecipeGates.RenaissanceAndEarlyModernJewellerySilversmithing15,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 3,
		Interruptable = false,
		Phases = [
			new(25, "$0 select|selects $i1 and begin|begins to lay out the work with $t1.", "$0 select|selects $i1 and begin|begins to lay out the work with $t1.", ExertionLevel.Stasis, 0),
			new(35, "$0 shape|shapes the material with $t1 and $t2, checking the joins and fit as the work develops.", "$0 shape|shapes the material with $t1 and $t2, checking the joins and fit as the work develops.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the visible form and set|sets aside $p1 for inspection.", "$0 end|ends the unsuccessful work and set|sets the spoiled material aside.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 38.0, Material: "silver", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("38 grams of silver", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Pliers", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Pliers tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "earlymodern_jewellery_mca_pendant_643", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a water-wave silver pendant (@earlymodern_jewellery_mca_pendant_643)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition JewelleryAndDevotionalWorkMakeShellAmberPin() => new()
	{
		Metadata = new("Jewellery and devotional work/make shell amber pin", "crafts", ["earlymodern", "renaissance"], ["item:earlymodern_jewellery_mca_pin_651"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make shell amber pin",
		Category = "Jewellery and devotional work",
		Blurb = "make a shell amber pin",
		Action = "making jewellery or a devotional item",
		ActiveCraftItemSdesc = "an in-progress shell amber pin craft",
		Gate = CatalogueRecipeGates.RenaissanceAndEarlyModernJewelleryGemcraft15,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 3,
		Interruptable = false,
		Phases = [
			new(25, "$0 select|selects $i1 and begin|begins to lay out the work with $t1.", "$0 select|selects $i1 and begin|begins to lay out the work with $t1.", ExertionLevel.Stasis, 0),
			new(35, "$0 shape|shapes the material with $t1 and $t2, checking the joins and fit as the work develops.", "$0 shape|shapes the material with $t1 and $t2, checking the joins and fit as the work develops.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the visible form and set|sets aside $p1 for inspection.", "$0 end|ends the unsuccessful work and set|sets the spoiled material aside.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 22.0, Material: "amber", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("22 grams of amber", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Pliers", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Pliers tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "earlymodern_jewellery_mca_pin_651", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a shell amber pin (@earlymodern_jewellery_mca_pin_651)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition JewelleryAndDevotionalWorkMakeLeafScrollCopperRing() => new()
	{
		Metadata = new("Jewellery and devotional work/make leaf-scroll copper ring", "crafts", ["earlymodern", "renaissance"], ["item:earlymodern_jewellery_mca_ring_641"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make leaf-scroll copper ring",
		Category = "Jewellery and devotional work",
		Blurb = "make a leaf-scroll copper ring",
		Action = "making jewellery or a devotional item",
		ActiveCraftItemSdesc = "an in-progress leaf-scroll copper ring craft",
		Gate = CatalogueRecipeGates.RenaissanceAndEarlyModernJewellerySilversmithing15,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 3,
		Interruptable = false,
		Phases = [
			new(25, "$0 select|selects $i1 and begin|begins to lay out the work with $t1.", "$0 select|selects $i1 and begin|begins to lay out the work with $t1.", ExertionLevel.Stasis, 0),
			new(35, "$0 shape|shapes the material with $t1 and $t2, checking the joins and fit as the work develops.", "$0 shape|shapes the material with $t1 and $t2, checking the joins and fit as the work develops.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the visible form and set|sets aside $p1 for inspection.", "$0 end|ends the unsuccessful work and set|sets the spoiled material aside.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 27.0, Material: "copper", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("27 grams of copper", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Pliers", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Pliers tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "earlymodern_jewellery_mca_ring_641", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a leaf-scroll copper ring (@earlymodern_jewellery_mca_ring_641)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition JewelleryAndDevotionalWorkMakeSunburstBronzeRing() => new()
	{
		Metadata = new("Jewellery and devotional work/make sunburst bronze ring", "crafts", ["earlymodern", "renaissance"], ["item:earlymodern_jewellery_mca_signet_ring_642"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make sunburst bronze ring",
		Category = "Jewellery and devotional work",
		Blurb = "make a sunburst bronze ring",
		Action = "making jewellery or a devotional item",
		ActiveCraftItemSdesc = "an in-progress sunburst bronze ring craft",
		Gate = CatalogueRecipeGates.RenaissanceAndEarlyModernJewellerySilversmithing15,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 3,
		Interruptable = false,
		Phases = [
			new(25, "$0 select|selects $i1 and begin|begins to lay out the work with $t1.", "$0 select|selects $i1 and begin|begins to lay out the work with $t1.", ExertionLevel.Stasis, 0),
			new(35, "$0 shape|shapes the material with $t1 and $t2, checking the joins and fit as the work develops.", "$0 shape|shapes the material with $t1 and $t2, checking the joins and fit as the work develops.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the visible form and set|sets aside $p1 for inspection.", "$0 end|ends the unsuccessful work and set|sets the spoiled material aside.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 33.0, Material: "bronze", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("33 grams of bronze", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Pliers", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Pliers tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "earlymodern_jewellery_mca_signet_ring_642", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a sunburst bronze ring (@earlymodern_jewellery_mca_signet_ring_642)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition JewelleryAndDevotionalWorkMakeSunburstJadeTempleRing() => new()
	{
		Metadata = new("Jewellery and devotional work/make sunburst jade temple ring", "crafts", ["earlymodern", "renaissance"], ["item:earlymodern_jewellery_mca_temple_ring_658"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make sunburst jade temple ring",
		Category = "Jewellery and devotional work",
		Blurb = "make a sunburst jade temple ring",
		Action = "making jewellery or a devotional item",
		ActiveCraftItemSdesc = "an in-progress sunburst jade temple ring craft",
		Gate = CatalogueRecipeGates.RenaissanceAndEarlyModernJewelleryGemcraft15,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 3,
		Interruptable = false,
		Phases = [
			new(25, "$0 select|selects $i1 and begin|begins to lay out the work with $t1.", "$0 select|selects $i1 and begin|begins to lay out the work with $t1.", ExertionLevel.Stasis, 0),
			new(35, "$0 shape|shapes the material with $t1 and $t2, checking the joins and fit as the work develops.", "$0 shape|shapes the material with $t1 and $t2, checking the joins and fit as the work develops.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the visible form and set|sets aside $p1 for inspection.", "$0 end|ends the unsuccessful work and set|sets the spoiled material aside.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 61.0, Material: "jade", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("61 grams of jade", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Pliers", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Pliers tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "earlymodern_jewellery_mca_temple_ring_658", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a sunburst jade temple ring (@earlymodern_jewellery_mca_temple_ring_658)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition JewelleryAndDevotionalWorkMakeWaterWaveQuartzTorc() => new()
	{
		Metadata = new("Jewellery and devotional work/make water-wave quartz torc", "crafts", ["earlymodern", "renaissance"], ["item:earlymodern_jewellery_mca_torc_659"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make water-wave quartz torc",
		Category = "Jewellery and devotional work",
		Blurb = "make a water-wave quartz torc",
		Action = "making jewellery or a devotional item",
		ActiveCraftItemSdesc = "an in-progress water-wave quartz torc craft",
		Gate = CatalogueRecipeGates.RenaissanceAndEarlyModernJewelleryGemcraft15,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 3,
		Interruptable = false,
		Phases = [
			new(25, "$0 select|selects $i1 and begin|begins to lay out the work with $t1.", "$0 select|selects $i1 and begin|begins to lay out the work with $t1.", ExertionLevel.Stasis, 0),
			new(35, "$0 shape|shapes the material with $t1 and $t2, checking the joins and fit as the work develops.", "$0 shape|shapes the material with $t1 and $t2, checking the joins and fit as the work develops.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the visible form and set|sets aside $p1 for inspection.", "$0 end|ends the unsuccessful work and set|sets the spoiled material aside.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 66.0, Material: "quartz", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("66 grams of quartz", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Pliers", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Pliers tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "earlymodern_jewellery_mca_torc_659", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a water-wave quartz torc (@earlymodern_jewellery_mca_torc_659)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition JewelleryAndDevotionalWorkMakeBraidedCarnelianWaistChain() => new()
	{
		Metadata = new("Jewellery and devotional work/make braided carnelian waist chain", "crafts", ["earlymodern", "renaissance"], ["item:earlymodern_jewellery_mca_waist_chain_655"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make braided carnelian waist chain",
		Category = "Jewellery and devotional work",
		Blurb = "make a braided carnelian waist chain",
		Action = "making jewellery or a devotional item",
		ActiveCraftItemSdesc = "an in-progress braided carnelian waist chain craft",
		Gate = CatalogueRecipeGates.RenaissanceAndEarlyModernJewelleryGemcraft15,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 3,
		Interruptable = false,
		Phases = [
			new(25, "$0 select|selects $i1 and begin|begins to lay out the work with $t1.", "$0 select|selects $i1 and begin|begins to lay out the work with $t1.", ExertionLevel.Stasis, 0),
			new(35, "$0 shape|shapes the material with $t1 and $t2, checking the joins and fit as the work develops.", "$0 shape|shapes the material with $t1 and $t2, checking the joins and fit as the work develops.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the visible form and set|sets aside $p1 for inspection.", "$0 end|ends the unsuccessful work and set|sets the spoiled material aside.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 44.0, Material: "carnelian", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("44 grams of carnelian", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Pliers", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Pliers tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "earlymodern_jewellery_mca_waist_chain_655", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a braided carnelian waist chain (@earlymodern_jewellery_mca_waist_chain_655)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition JewelleryAndDevotionalWorkMakeKnottedFaienceWreath() => new()
	{
		Metadata = new("Jewellery and devotional work/make knotted faience wreath", "crafts", ["earlymodern", "renaissance"], ["item:earlymodern_jewellery_mca_wreath_660"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make knotted faience wreath",
		Category = "Jewellery and devotional work",
		Blurb = "make a knotted faience wreath",
		Action = "making jewellery or a devotional item",
		ActiveCraftItemSdesc = "an in-progress knotted faience wreath craft",
		Gate = CatalogueRecipeGates.RenaissanceAndEarlyModernJewelleryGlassworking15,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 3,
		Interruptable = false,
		Phases = [
			new(25, "$0 select|selects $i1 and begin|begins to lay out the work with $t1.", "$0 select|selects $i1 and begin|begins to lay out the work with $t1.", ExertionLevel.Stasis, 0),
			new(35, "$0 shape|shapes the material with $t1 and $t2, checking the joins and fit as the work develops.", "$0 shape|shapes the material with $t1 and $t2, checking the joins and fit as the work develops.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the visible form and set|sets aside $p1 for inspection.", "$0 end|ends the unsuccessful work and set|sets the spoiled material aside.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 72.0, Material: "faience", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("72 grams of faience", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Pliers", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Pliers tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "earlymodern_jewellery_mca_wreath_660", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a knotted faience wreath (@earlymodern_jewellery_mca_wreath_660)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition JewelleryAndDevotionalWorkMakeKnottedCoralAnklet() => new()
	{
		Metadata = new("Jewellery and devotional work/make knotted coral anklet", "crafts", ["earlymodern", "renaissance"], ["item:earlymodern_jewellery_ming_anklet_328"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make knotted coral anklet",
		Category = "Jewellery and devotional work",
		Blurb = "make a knotted coral anklet",
		Action = "making jewellery or a devotional item",
		ActiveCraftItemSdesc = "an in-progress knotted coral anklet craft",
		Gate = CatalogueRecipeGates.RenaissanceAndEarlyModernJewelleryScrimshawing15,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 3,
		Interruptable = false,
		Phases = [
			new(25, "$0 select|selects $i1 and begin|begins to lay out the work with $t1.", "$0 select|selects $i1 and begin|begins to lay out the work with $t1.", ExertionLevel.Stasis, 0),
			new(35, "$0 shape|shapes the material with $t1 and $t2, checking the joins and fit as the work develops.", "$0 shape|shapes the material with $t1 and $t2, checking the joins and fit as the work develops.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the visible form and set|sets aside $p1 for inspection.", "$0 end|ends the unsuccessful work and set|sets the spoiled material aside.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 61.0, Material: "coral", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("61 grams of coral", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Pliers", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Pliers tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "earlymodern_jewellery_ming_anklet_328", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a knotted coral anklet (@earlymodern_jewellery_ming_anklet_328)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition JewelleryAndDevotionalWorkMakeWaterWaveAmberArmlet() => new()
	{
		Metadata = new("Jewellery and devotional work/make water-wave amber armlet", "crafts", ["earlymodern", "renaissance"], ["item:earlymodern_jewellery_ming_armlet_327"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make water-wave amber armlet",
		Category = "Jewellery and devotional work",
		Blurb = "make a water-wave amber armlet",
		Action = "making jewellery or a devotional item",
		ActiveCraftItemSdesc = "an in-progress water-wave amber armlet craft",
		Gate = CatalogueRecipeGates.RenaissanceAndEarlyModernJewelleryGemcraft15,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 3,
		Interruptable = false,
		Phases = [
			new(25, "$0 select|selects $i1 and begin|begins to lay out the work with $t1.", "$0 select|selects $i1 and begin|begins to lay out the work with $t1.", ExertionLevel.Stasis, 0),
			new(35, "$0 shape|shapes the material with $t1 and $t2, checking the joins and fit as the work develops.", "$0 shape|shapes the material with $t1 and $t2, checking the joins and fit as the work develops.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the visible form and set|sets aside $p1 for inspection.", "$0 end|ends the unsuccessful work and set|sets the spoiled material aside.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 55.0, Material: "amber", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("55 grams of amber", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Pliers", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Pliers tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "earlymodern_jewellery_ming_armlet_327", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a water-wave amber armlet (@earlymodern_jewellery_ming_armlet_327)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition JewelleryAndDevotionalWorkMakeRosettePewterBadge() => new()
	{
		Metadata = new("Jewellery and devotional work/make rosette pewter badge", "crafts", ["earlymodern", "renaissance"], ["item:earlymodern_jewellery_ming_badge_337"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make rosette pewter badge",
		Category = "Jewellery and devotional work",
		Blurb = "make a rosette pewter badge",
		Action = "making jewellery or a devotional item",
		ActiveCraftItemSdesc = "an in-progress rosette pewter badge craft",
		Gate = CatalogueRecipeGates.RenaissanceAndEarlyModernJewellerySilversmithing15,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 3,
		Interruptable = false,
		Phases = [
			new(25, "$0 select|selects $i1 and begin|begins to lay out the work with $t1.", "$0 select|selects $i1 and begin|begins to lay out the work with $t1.", ExertionLevel.Stasis, 0),
			new(35, "$0 shape|shapes the material with $t1 and $t2, checking the joins and fit as the work develops.", "$0 shape|shapes the material with $t1 and $t2, checking the joins and fit as the work develops.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the visible form and set|sets aside $p1 for inspection.", "$0 end|ends the unsuccessful work and set|sets the spoiled material aside.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 50.0, Material: "pewter", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("50 grams of pewter", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Pliers", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Pliers tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "earlymodern_jewellery_ming_badge_337", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a rosette pewter badge (@earlymodern_jewellery_ming_badge_337)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition JewelleryAndDevotionalWorkMakeRadiatingShellNecklace() => new()
	{
		Metadata = new("Jewellery and devotional work/make radiating shell necklace", "crafts", ["earlymodern", "renaissance"], ["item:earlymodern_jewellery_ming_bead_necklace_324"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make radiating shell necklace",
		Category = "Jewellery and devotional work",
		Blurb = "make a radiating shell necklace",
		Action = "making jewellery or a devotional item",
		ActiveCraftItemSdesc = "an in-progress radiating shell necklace craft",
		Gate = CatalogueRecipeGates.RenaissanceAndEarlyModernJewelleryScrimshawing15,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 3,
		Interruptable = false,
		Phases = [
			new(25, "$0 select|selects $i1 and begin|begins to lay out the work with $t1.", "$0 select|selects $i1 and begin|begins to lay out the work with $t1.", ExertionLevel.Stasis, 0),
			new(35, "$0 shape|shapes the material with $t1 and $t2, checking the joins and fit as the work develops.", "$0 shape|shapes the material with $t1 and $t2, checking the joins and fit as the work develops.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the visible form and set|sets aside $p1 for inspection.", "$0 end|ends the unsuccessful work and set|sets the spoiled material aside.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 38.0, Material: "shell", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("38 grams of shell", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Pliers", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Pliers tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "earlymodern_jewellery_ming_bead_necklace_324", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a radiating shell necklace (@earlymodern_jewellery_ming_bead_necklace_324)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition JewelleryAndDevotionalWorkMakeReedFaienceBeltOrnament() => new()
	{
		Metadata = new("Jewellery and devotional work/make reed faience belt ornament", "crafts", ["earlymodern", "renaissance"], ["item:earlymodern_jewellery_ming_belt_ornament_336"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make reed faience belt ornament",
		Category = "Jewellery and devotional work",
		Blurb = "make a reed faience belt ornament",
		Action = "making jewellery or a devotional item",
		ActiveCraftItemSdesc = "an in-progress reed faience belt ornament craft",
		Gate = CatalogueRecipeGates.RenaissanceAndEarlyModernJewelleryGlassworking15,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 3,
		Interruptable = false,
		Phases = [
			new(25, "$0 select|selects $i1 and begin|begins to lay out the work with $t1.", "$0 select|selects $i1 and begin|begins to lay out the work with $t1.", ExertionLevel.Stasis, 0),
			new(35, "$0 shape|shapes the material with $t1 and $t2, checking the joins and fit as the work develops.", "$0 shape|shapes the material with $t1 and $t2, checking the joins and fit as the work develops.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the visible form and set|sets aside $p1 for inspection.", "$0 end|ends the unsuccessful work and set|sets the spoiled material aside.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 44.0, Material: "faience", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("44 grams of faience", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Pliers", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Pliers tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "earlymodern_jewellery_ming_belt_ornament_336", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a reed faience belt ornament (@earlymodern_jewellery_ming_belt_ornament_336)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition JewelleryAndDevotionalWorkMakeSunburstPearlBracelet() => new()
	{
		Metadata = new("Jewellery and devotional work/make sunburst pearl bracelet", "crafts", ["earlymodern", "renaissance"], ["item:earlymodern_jewellery_ming_bracelet_326"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make sunburst pearl bracelet",
		Category = "Jewellery and devotional work",
		Blurb = "make a sunburst pearl bracelet",
		Action = "making jewellery or a devotional item",
		ActiveCraftItemSdesc = "an in-progress sunburst pearl bracelet craft",
		Gate = CatalogueRecipeGates.RenaissanceAndEarlyModernJewelleryGemcraft15,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 3,
		Interruptable = false,
		Phases = [
			new(25, "$0 select|selects $i1 and begin|begins to lay out the work with $t1.", "$0 select|selects $i1 and begin|begins to lay out the work with $t1.", ExertionLevel.Stasis, 0),
			new(35, "$0 shape|shapes the material with $t1 and $t2, checking the joins and fit as the work develops.", "$0 shape|shapes the material with $t1 and $t2, checking the joins and fit as the work develops.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the visible form and set|sets aside $p1 for inspection.", "$0 end|ends the unsuccessful work and set|sets the spoiled material aside.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 50.0, Material: "pearl", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("50 grams of pearl", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Pliers", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Pliers tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "earlymodern_jewellery_ming_bracelet_326", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a sunburst pearl bracelet (@earlymodern_jewellery_ming_bracelet_326)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition JewelleryAndDevotionalWorkMakeStarAgateBrooch() => new()
	{
		Metadata = new("Jewellery and devotional work/make star agate brooch", "crafts", ["earlymodern", "renaissance"], ["item:earlymodern_jewellery_ming_brooch_330"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make star agate brooch",
		Category = "Jewellery and devotional work",
		Blurb = "make a star agate brooch",
		Action = "making jewellery or a devotional item",
		ActiveCraftItemSdesc = "an in-progress star agate brooch craft",
		Gate = CatalogueRecipeGates.RenaissanceAndEarlyModernJewelleryGemcraft15,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 3,
		Interruptable = false,
		Phases = [
			new(25, "$0 select|selects $i1 and begin|begins to lay out the work with $t1.", "$0 select|selects $i1 and begin|begins to lay out the work with $t1.", ExertionLevel.Stasis, 0),
			new(35, "$0 shape|shapes the material with $t1 and $t2, checking the joins and fit as the work develops.", "$0 shape|shapes the material with $t1 and $t2, checking the joins and fit as the work develops.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the visible form and set|sets aside $p1 for inspection.", "$0 end|ends the unsuccessful work and set|sets the spoiled material aside.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 72.0, Material: "agate", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("72 grams of agate", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Pliers", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Pliers tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "earlymodern_jewellery_ming_brooch_330", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a star agate brooch (@earlymodern_jewellery_ming_brooch_330)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition JewelleryAndDevotionalWorkMakeLeafScrollIvoryNecklace() => new()
	{
		Metadata = new("Jewellery and devotional work/make leaf-scroll ivory necklace", "crafts", ["earlymodern", "renaissance"], ["item:earlymodern_jewellery_ming_chain_necklace_325"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make leaf-scroll ivory necklace",
		Category = "Jewellery and devotional work",
		Blurb = "make a leaf-scroll ivory necklace",
		Action = "making jewellery or a devotional item",
		ActiveCraftItemSdesc = "an in-progress leaf-scroll ivory necklace craft",
		Gate = CatalogueRecipeGates.RenaissanceAndEarlyModernJewelleryScrimshawing15,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 3,
		Interruptable = false,
		Phases = [
			new(25, "$0 select|selects $i1 and begin|begins to lay out the work with $t1.", "$0 select|selects $i1 and begin|begins to lay out the work with $t1.", ExertionLevel.Stasis, 0),
			new(35, "$0 shape|shapes the material with $t1 and $t2, checking the joins and fit as the work develops.", "$0 shape|shapes the material with $t1 and $t2, checking the joins and fit as the work develops.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the visible form and set|sets aside $p1 for inspection.", "$0 end|ends the unsuccessful work and set|sets the spoiled material aside.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 44.0, Material: "ivory", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("44 grams of ivory", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Pliers", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Pliers tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "earlymodern_jewellery_ming_chain_necklace_325", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a leaf-scroll ivory necklace (@earlymodern_jewellery_ming_chain_necklace_325)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition JewelleryAndDevotionalWorkMakeGeometricTurquoiseCirclet() => new()
	{
		Metadata = new("Jewellery and devotional work/make geometric turquoise circlet", "crafts", ["earlymodern", "renaissance"], ["item:earlymodern_jewellery_ming_circlet_333"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make geometric turquoise circlet",
		Category = "Jewellery and devotional work",
		Blurb = "make a geometric turquoise circlet",
		Action = "making jewellery or a devotional item",
		ActiveCraftItemSdesc = "an in-progress geometric turquoise circlet craft",
		Gate = CatalogueRecipeGates.RenaissanceAndEarlyModernJewelleryGemcraft15,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 3,
		Interruptable = false,
		Phases = [
			new(25, "$0 select|selects $i1 and begin|begins to lay out the work with $t1.", "$0 select|selects $i1 and begin|begins to lay out the work with $t1.", ExertionLevel.Stasis, 0),
			new(35, "$0 shape|shapes the material with $t1 and $t2, checking the joins and fit as the work develops.", "$0 shape|shapes the material with $t1 and $t2, checking the joins and fit as the work develops.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the visible form and set|sets aside $p1 for inspection.", "$0 end|ends the unsuccessful work and set|sets the spoiled material aside.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 27.0, Material: "turquoise", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("27 grams of turquoise", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Pliers", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Pliers tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "earlymodern_jewellery_ming_circlet_333", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a geometric turquoise circlet (@earlymodern_jewellery_ming_circlet_333)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition JewelleryAndDevotionalWorkMakeCloudJadeDiadem() => new()
	{
		Metadata = new("Jewellery and devotional work/make cloud jade diadem", "crafts", ["earlymodern", "renaissance"], ["item:earlymodern_jewellery_ming_diadem_334"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make cloud jade diadem",
		Category = "Jewellery and devotional work",
		Blurb = "make a cloud jade diadem",
		Action = "making jewellery or a devotional item",
		ActiveCraftItemSdesc = "an in-progress cloud jade diadem craft",
		Gate = CatalogueRecipeGates.RenaissanceAndEarlyModernJewelleryGemcraft15,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 3,
		Interruptable = false,
		Phases = [
			new(25, "$0 select|selects $i1 and begin|begins to lay out the work with $t1.", "$0 select|selects $i1 and begin|begins to lay out the work with $t1.", ExertionLevel.Stasis, 0),
			new(35, "$0 shape|shapes the material with $t1 and $t2, checking the joins and fit as the work develops.", "$0 shape|shapes the material with $t1 and $t2, checking the joins and fit as the work develops.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the visible form and set|sets aside $p1 for inspection.", "$0 end|ends the unsuccessful work and set|sets the spoiled material aside.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 33.0, Material: "jade", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("33 grams of jade", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Pliers", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Pliers tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "earlymodern_jewellery_ming_diadem_334", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a cloud jade diadem (@earlymodern_jewellery_ming_diadem_334)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition JewelleryAndDevotionalWorkMakeFlowerHeadGarnetEarrings() => new()
	{
		Metadata = new("Jewellery and devotional work/make flower-head garnet earrings", "crafts", ["earlymodern", "renaissance"], ["item:earlymodern_jewellery_ming_earrings_329"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make flower-head garnet earrings",
		Category = "Jewellery and devotional work",
		Blurb = "make a flower-head garnet earrings",
		Action = "making jewellery or a devotional item",
		ActiveCraftItemSdesc = "an in-progress flower-head garnet earrings craft",
		Gate = CatalogueRecipeGates.RenaissanceAndEarlyModernJewelleryGemcraft15,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 3,
		Interruptable = false,
		Phases = [
			new(25, "$0 select|selects $i1 and begin|begins to lay out the work with $t1.", "$0 select|selects $i1 and begin|begins to lay out the work with $t1.", ExertionLevel.Stasis, 0),
			new(35, "$0 shape|shapes the material with $t1 and $t2, checking the joins and fit as the work develops.", "$0 shape|shapes the material with $t1 and $t2, checking the joins and fit as the work develops.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the visible form and set|sets aside $p1 for inspection.", "$0 end|ends the unsuccessful work and set|sets the spoiled material aside.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 66.0, Material: "garnet", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("66 grams of garnet", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Pliers", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Pliers tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "earlymodern_jewellery_ming_earrings_329", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a flower-head garnet earrings (@earlymodern_jewellery_ming_earrings_329)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition JewelleryAndDevotionalWorkMakeVineLapisLazuliHairpin() => new()
	{
		Metadata = new("Jewellery and devotional work/make vine lapis lazuli hairpin", "crafts", ["earlymodern", "renaissance"], ["item:earlymodern_jewellery_ming_hairpin_332"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make vine lapis lazuli hairpin",
		Category = "Jewellery and devotional work",
		Blurb = "make a vine lapis lazuli hairpin",
		Action = "making jewellery or a devotional item",
		ActiveCraftItemSdesc = "an in-progress vine lapis lazuli hairpin craft",
		Gate = CatalogueRecipeGates.RenaissanceAndEarlyModernJewelleryGemcraft15,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 3,
		Interruptable = false,
		Phases = [
			new(25, "$0 select|selects $i1 and begin|begins to lay out the work with $t1.", "$0 select|selects $i1 and begin|begins to lay out the work with $t1.", ExertionLevel.Stasis, 0),
			new(35, "$0 shape|shapes the material with $t1 and $t2, checking the joins and fit as the work develops.", "$0 shape|shapes the material with $t1 and $t2, checking the joins and fit as the work develops.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the visible form and set|sets aside $p1 for inspection.", "$0 end|ends the unsuccessful work and set|sets the spoiled material aside.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 22.0, Material: "lapis lazuli", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("22 grams of lapis lazuli", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Pliers", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Pliers tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "earlymodern_jewellery_ming_hairpin_332", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a vine lapis lazuli hairpin (@earlymodern_jewellery_ming_hairpin_332)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition JewelleryAndDevotionalWorkMakeBraidedBonePendant() => new()
	{
		Metadata = new("Jewellery and devotional work/make braided bone pendant", "crafts", ["earlymodern", "renaissance"], ["item:earlymodern_jewellery_ming_pendant_323"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make braided bone pendant",
		Category = "Jewellery and devotional work",
		Blurb = "make a braided bone pendant",
		Action = "making jewellery or a devotional item",
		ActiveCraftItemSdesc = "an in-progress braided bone pendant craft",
		Gate = CatalogueRecipeGates.RenaissanceAndEarlyModernJewelleryScrimshawing15,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 3,
		Interruptable = false,
		Phases = [
			new(25, "$0 select|selects $i1 and begin|begins to lay out the work with $t1.", "$0 select|selects $i1 and begin|begins to lay out the work with $t1.", ExertionLevel.Stasis, 0),
			new(35, "$0 shape|shapes the material with $t1 and $t2, checking the joins and fit as the work develops.", "$0 shape|shapes the material with $t1 and $t2, checking the joins and fit as the work develops.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the visible form and set|sets aside $p1 for inspection.", "$0 end|ends the unsuccessful work and set|sets the spoiled material aside.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 33.0, Material: "bone", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("33 grams of bone", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Pliers", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Pliers tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "earlymodern_jewellery_ming_pendant_323", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a braided bone pendant (@earlymodern_jewellery_ming_pendant_323)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition JewelleryAndDevotionalWorkMakeBirdCarnelianPin() => new()
	{
		Metadata = new("Jewellery and devotional work/make bird carnelian pin", "crafts", ["earlymodern", "renaissance"], ["item:earlymodern_jewellery_ming_pin_331"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make bird carnelian pin",
		Category = "Jewellery and devotional work",
		Blurb = "make a bird carnelian pin",
		Action = "making jewellery or a devotional item",
		ActiveCraftItemSdesc = "an in-progress bird carnelian pin craft",
		Gate = CatalogueRecipeGates.RenaissanceAndEarlyModernJewelleryGemcraft15,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 3,
		Interruptable = false,
		Phases = [
			new(25, "$0 select|selects $i1 and begin|begins to lay out the work with $t1.", "$0 select|selects $i1 and begin|begins to lay out the work with $t1.", ExertionLevel.Stasis, 0),
			new(35, "$0 shape|shapes the material with $t1 and $t2, checking the joins and fit as the work develops.", "$0 shape|shapes the material with $t1 and $t2, checking the joins and fit as the work develops.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the visible form and set|sets aside $p1 for inspection.", "$0 end|ends the unsuccessful work and set|sets the spoiled material aside.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 16.0, Material: "carnelian", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("16 grams of carnelian", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Pliers", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Pliers tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "earlymodern_jewellery_ming_pin_331", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a bird carnelian pin (@earlymodern_jewellery_ming_pin_331)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition JewelleryAndDevotionalWorkMakeRosetteBrassRing() => new()
	{
		Metadata = new("Jewellery and devotional work/make rosette brass ring", "crafts", ["earlymodern", "renaissance"], ["item:earlymodern_jewellery_ming_ring_321"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make rosette brass ring",
		Category = "Jewellery and devotional work",
		Blurb = "make a rosette brass ring",
		Action = "making jewellery or a devotional item",
		ActiveCraftItemSdesc = "an in-progress rosette brass ring craft",
		Gate = CatalogueRecipeGates.RenaissanceAndEarlyModernJewellerySilversmithing15,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 3,
		Interruptable = false,
		Phases = [
			new(25, "$0 select|selects $i1 and begin|begins to lay out the work with $t1.", "$0 select|selects $i1 and begin|begins to lay out the work with $t1.", ExertionLevel.Stasis, 0),
			new(35, "$0 shape|shapes the material with $t1 and $t2, checking the joins and fit as the work develops.", "$0 shape|shapes the material with $t1 and $t2, checking the joins and fit as the work develops.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the visible form and set|sets aside $p1 for inspection.", "$0 end|ends the unsuccessful work and set|sets the spoiled material aside.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 22.0, Material: "brass", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("22 grams of brass", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Pliers", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Pliers tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "earlymodern_jewellery_ming_ring_321", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a rosette brass ring (@earlymodern_jewellery_ming_ring_321)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition JewelleryAndDevotionalWorkMakeCheckerGlassRing() => new()
	{
		Metadata = new("Jewellery and devotional work/make checker glass ring", "crafts", ["earlymodern", "renaissance"], ["item:earlymodern_jewellery_ming_signet_ring_322"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make checker glass ring",
		Category = "Jewellery and devotional work",
		Blurb = "make a checker glass ring",
		Action = "making jewellery or a devotional item",
		ActiveCraftItemSdesc = "an in-progress checker glass ring craft",
		Gate = CatalogueRecipeGates.RenaissanceAndEarlyModernJewelleryGlassworking15,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 3,
		Interruptable = false,
		Phases = [
			new(25, "$0 select|selects $i1 and begin|begins to lay out the work with $t1.", "$0 select|selects $i1 and begin|begins to lay out the work with $t1.", ExertionLevel.Stasis, 0),
			new(35, "$0 shape|shapes the material with $t1 and $t2, checking the joins and fit as the work develops.", "$0 shape|shapes the material with $t1 and $t2, checking the joins and fit as the work develops.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the visible form and set|sets aside $p1 for inspection.", "$0 end|ends the unsuccessful work and set|sets the spoiled material aside.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 27.0, Material: "glass", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("27 grams of glass", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Pliers", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Pliers tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "earlymodern_jewellery_ming_signet_ring_322", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a checker glass ring (@earlymodern_jewellery_ming_signet_ring_322)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition JewelleryAndDevotionalWorkMakeCheckerCopperTempleRing() => new()
	{
		Metadata = new("Jewellery and devotional work/make checker copper temple ring", "crafts", ["earlymodern", "renaissance"], ["item:earlymodern_jewellery_ming_temple_ring_338"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make checker copper temple ring",
		Category = "Jewellery and devotional work",
		Blurb = "make a checker copper temple ring",
		Action = "making jewellery or a devotional item",
		ActiveCraftItemSdesc = "an in-progress checker copper temple ring craft",
		Gate = CatalogueRecipeGates.RenaissanceAndEarlyModernJewellerySilversmithing15,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 3,
		Interruptable = false,
		Phases = [
			new(25, "$0 select|selects $i1 and begin|begins to lay out the work with $t1.", "$0 select|selects $i1 and begin|begins to lay out the work with $t1.", ExertionLevel.Stasis, 0),
			new(35, "$0 shape|shapes the material with $t1 and $t2, checking the joins and fit as the work develops.", "$0 shape|shapes the material with $t1 and $t2, checking the joins and fit as the work develops.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the visible form and set|sets aside $p1 for inspection.", "$0 end|ends the unsuccessful work and set|sets the spoiled material aside.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 55.0, Material: "copper", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("55 grams of copper", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Pliers", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Pliers tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "earlymodern_jewellery_ming_temple_ring_338", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a checker copper temple ring (@earlymodern_jewellery_ming_temple_ring_338)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition JewelleryAndDevotionalWorkMakeBraidedBronzeTorc() => new()
	{
		Metadata = new("Jewellery and devotional work/make braided bronze torc", "crafts", ["earlymodern", "renaissance"], ["item:earlymodern_jewellery_ming_torc_339"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make braided bronze torc",
		Category = "Jewellery and devotional work",
		Blurb = "make a braided bronze torc",
		Action = "making jewellery or a devotional item",
		ActiveCraftItemSdesc = "an in-progress braided bronze torc craft",
		Gate = CatalogueRecipeGates.RenaissanceAndEarlyModernJewellerySilversmithing15,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 3,
		Interruptable = false,
		Phases = [
			new(25, "$0 select|selects $i1 and begin|begins to lay out the work with $t1.", "$0 select|selects $i1 and begin|begins to lay out the work with $t1.", ExertionLevel.Stasis, 0),
			new(35, "$0 shape|shapes the material with $t1 and $t2, checking the joins and fit as the work develops.", "$0 shape|shapes the material with $t1 and $t2, checking the joins and fit as the work develops.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the visible form and set|sets aside $p1 for inspection.", "$0 end|ends the unsuccessful work and set|sets the spoiled material aside.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 61.0, Material: "bronze", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("61 grams of bronze", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Pliers", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Pliers tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "earlymodern_jewellery_ming_torc_339", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a braided bronze torc (@earlymodern_jewellery_ming_torc_339)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition JewelleryAndDevotionalWorkMakeShellQuartzWaistChain() => new()
	{
		Metadata = new("Jewellery and devotional work/make shell quartz waist chain", "crafts", ["earlymodern", "renaissance"], ["item:earlymodern_jewellery_ming_waist_chain_335"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make shell quartz waist chain",
		Category = "Jewellery and devotional work",
		Blurb = "make a shell quartz waist chain",
		Action = "making jewellery or a devotional item",
		ActiveCraftItemSdesc = "an in-progress shell quartz waist chain craft",
		Gate = CatalogueRecipeGates.RenaissanceAndEarlyModernJewelleryGemcraft15,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 3,
		Interruptable = false,
		Phases = [
			new(25, "$0 select|selects $i1 and begin|begins to lay out the work with $t1.", "$0 select|selects $i1 and begin|begins to lay out the work with $t1.", ExertionLevel.Stasis, 0),
			new(35, "$0 shape|shapes the material with $t1 and $t2, checking the joins and fit as the work develops.", "$0 shape|shapes the material with $t1 and $t2, checking the joins and fit as the work develops.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the visible form and set|sets aside $p1 for inspection.", "$0 end|ends the unsuccessful work and set|sets the spoiled material aside.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 38.0, Material: "quartz", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("38 grams of quartz", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Pliers", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Pliers tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "earlymodern_jewellery_ming_waist_chain_335", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a shell quartz waist chain (@earlymodern_jewellery_ming_waist_chain_335)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition JewelleryAndDevotionalWorkMakeRadiatingSilverWreath() => new()
	{
		Metadata = new("Jewellery and devotional work/make radiating silver wreath", "crafts", ["earlymodern", "renaissance"], ["item:earlymodern_jewellery_ming_wreath_340"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make radiating silver wreath",
		Category = "Jewellery and devotional work",
		Blurb = "make a radiating silver wreath",
		Action = "making jewellery or a devotional item",
		ActiveCraftItemSdesc = "an in-progress radiating silver wreath craft",
		Gate = CatalogueRecipeGates.RenaissanceAndEarlyModernJewellerySilversmithing15,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 3,
		Interruptable = false,
		Phases = [
			new(25, "$0 select|selects $i1 and begin|begins to lay out the work with $t1.", "$0 select|selects $i1 and begin|begins to lay out the work with $t1.", ExertionLevel.Stasis, 0),
			new(35, "$0 shape|shapes the material with $t1 and $t2, checking the joins and fit as the work develops.", "$0 shape|shapes the material with $t1 and $t2, checking the joins and fit as the work develops.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the visible form and set|sets aside $p1 for inspection.", "$0 end|ends the unsuccessful work and set|sets the spoiled material aside.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 66.0, Material: "silver", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("66 grams of silver", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Pliers", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Pliers tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "earlymodern_jewellery_ming_wreath_340", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a radiating silver wreath (@earlymodern_jewellery_ming_wreath_340)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition JewelleryAndDevotionalWorkMakeShellLapisLazuliAnkletPattern2() => new()
	{
		Metadata = new("Jewellery and devotional work/make shell lapis lazuli anklet pattern 2", "crafts", ["earlymodern", "renaissance"], ["item:earlymodern_jewellery_mrd_anklet_268"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make shell lapis lazuli anklet pattern 2",
		Category = "Jewellery and devotional work",
		Blurb = "make a shell lapis lazuli anklet",
		Action = "making jewellery or a devotional item",
		ActiveCraftItemSdesc = "an in-progress shell lapis lazuli anklet craft",
		Gate = CatalogueRecipeGates.RenaissanceAndEarlyModernJewelleryGemcraft15,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 3,
		Interruptable = false,
		Phases = [
			new(25, "$0 select|selects $i1 and begin|begins to lay out the work with $t1.", "$0 select|selects $i1 and begin|begins to lay out the work with $t1.", ExertionLevel.Stasis, 0),
			new(35, "$0 shape|shapes the material with $t1 and $t2, checking the joins and fit as the work develops.", "$0 shape|shapes the material with $t1 and $t2, checking the joins and fit as the work develops.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the visible form and set|sets aside $p1 for inspection.", "$0 end|ends the unsuccessful work and set|sets the spoiled material aside.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 33.0, Material: "lapis lazuli", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("33 grams of lapis lazuli", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Pliers", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Pliers tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "earlymodern_jewellery_mrd_anklet_268", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a shell lapis lazuli anklet (@earlymodern_jewellery_mrd_anklet_268)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition JewelleryAndDevotionalWorkMakeCloudCarnelianArmletPattern2() => new()
	{
		Metadata = new("Jewellery and devotional work/make cloud carnelian armlet pattern 2", "crafts", ["earlymodern", "renaissance"], ["item:earlymodern_jewellery_mrd_armlet_267"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make cloud carnelian armlet pattern 2",
		Category = "Jewellery and devotional work",
		Blurb = "make a cloud carnelian armlet",
		Action = "making jewellery or a devotional item",
		ActiveCraftItemSdesc = "an in-progress cloud carnelian armlet craft",
		Gate = CatalogueRecipeGates.RenaissanceAndEarlyModernJewelleryGemcraft15,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 3,
		Interruptable = false,
		Phases = [
			new(25, "$0 select|selects $i1 and begin|begins to lay out the work with $t1.", "$0 select|selects $i1 and begin|begins to lay out the work with $t1.", ExertionLevel.Stasis, 0),
			new(35, "$0 shape|shapes the material with $t1 and $t2, checking the joins and fit as the work develops.", "$0 shape|shapes the material with $t1 and $t2, checking the joins and fit as the work develops.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the visible form and set|sets aside $p1 for inspection.", "$0 end|ends the unsuccessful work and set|sets the spoiled material aside.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 27.0, Material: "carnelian", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("27 grams of carnelian", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Pliers", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Pliers tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "earlymodern_jewellery_mrd_armlet_267", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a cloud carnelian armlet (@earlymodern_jewellery_mrd_armlet_267)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition JewelleryAndDevotionalWorkMakeKnottedGoldBadgePattern2() => new()
	{
		Metadata = new("Jewellery and devotional work/make knotted gold badge pattern 2", "crafts", ["earlymodern", "renaissance"], ["item:earlymodern_jewellery_mrd_badge_277"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make knotted gold badge pattern 2",
		Category = "Jewellery and devotional work",
		Blurb = "make a knotted gold badge",
		Action = "making jewellery or a devotional item",
		ActiveCraftItemSdesc = "an in-progress knotted gold badge craft",
		Gate = CatalogueRecipeGates.RenaissanceAndEarlyModernJewellerySilversmithing15,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 3,
		Interruptable = false,
		Phases = [
			new(25, "$0 select|selects $i1 and begin|begins to lay out the work with $t1.", "$0 select|selects $i1 and begin|begins to lay out the work with $t1.", ExertionLevel.Stasis, 0),
			new(35, "$0 shape|shapes the material with $t1 and $t2, checking the joins and fit as the work develops.", "$0 shape|shapes the material with $t1 and $t2, checking the joins and fit as the work develops.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the visible form and set|sets aside $p1 for inspection.", "$0 end|ends the unsuccessful work and set|sets the spoiled material aside.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 22.0, Material: "gold", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("22 grams of gold", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Pliers", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Pliers tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "earlymodern_jewellery_mrd_badge_277", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a knotted gold badge (@earlymodern_jewellery_mrd_badge_277)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition JewelleryAndDevotionalWorkMakeBirdCoralNecklacePattern2() => new()
	{
		Metadata = new("Jewellery and devotional work/make bird coral necklace pattern 2", "crafts", ["earlymodern", "renaissance"], ["item:earlymodern_jewellery_mrd_bead_necklace_264"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make bird coral necklace pattern 2",
		Category = "Jewellery and devotional work",
		Blurb = "make a bird coral necklace",
		Action = "making jewellery or a devotional item",
		ActiveCraftItemSdesc = "an in-progress bird coral necklace craft",
		Gate = CatalogueRecipeGates.RenaissanceAndEarlyModernJewelleryScrimshawing15,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 3,
		Interruptable = false,
		Phases = [
			new(25, "$0 select|selects $i1 and begin|begins to lay out the work with $t1.", "$0 select|selects $i1 and begin|begins to lay out the work with $t1.", ExertionLevel.Stasis, 0),
			new(35, "$0 shape|shapes the material with $t1 and $t2, checking the joins and fit as the work develops.", "$0 shape|shapes the material with $t1 and $t2, checking the joins and fit as the work develops.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the visible form and set|sets aside $p1 for inspection.", "$0 end|ends the unsuccessful work and set|sets the spoiled material aside.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 72.0, Material: "coral", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("72 grams of coral", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Pliers", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Pliers tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "earlymodern_jewellery_mrd_bead_necklace_264", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a bird coral necklace (@earlymodern_jewellery_mrd_bead_necklace_264)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition JewelleryAndDevotionalWorkMakeWaterWaveSilverBeltOrnamentPattern2() => new()
	{
		Metadata = new("Jewellery and devotional work/make water-wave silver belt ornament pattern 2", "crafts", ["earlymodern", "renaissance"], ["item:earlymodern_jewellery_mrd_belt_ornament_276"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make water-wave silver belt ornament pattern 2",
		Category = "Jewellery and devotional work",
		Blurb = "make a water-wave silver belt ornament",
		Action = "making jewellery or a devotional item",
		ActiveCraftItemSdesc = "an in-progress water-wave silver belt ornament craft",
		Gate = CatalogueRecipeGates.RenaissanceAndEarlyModernJewellerySilversmithing15,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 3,
		Interruptable = false,
		Phases = [
			new(25, "$0 select|selects $i1 and begin|begins to lay out the work with $t1.", "$0 select|selects $i1 and begin|begins to lay out the work with $t1.", ExertionLevel.Stasis, 0),
			new(35, "$0 shape|shapes the material with $t1 and $t2, checking the joins and fit as the work develops.", "$0 shape|shapes the material with $t1 and $t2, checking the joins and fit as the work develops.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the visible form and set|sets aside $p1 for inspection.", "$0 end|ends the unsuccessful work and set|sets the spoiled material aside.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 16.0, Material: "silver", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("16 grams of silver", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Pliers", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Pliers tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "earlymodern_jewellery_mrd_belt_ornament_276", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a water-wave silver belt ornament (@earlymodern_jewellery_mrd_belt_ornament_276)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition JewelleryAndDevotionalWorkMakeGeometricAgateBraceletPattern2() => new()
	{
		Metadata = new("Jewellery and devotional work/make geometric agate bracelet pattern 2", "crafts", ["earlymodern", "renaissance"], ["item:earlymodern_jewellery_mrd_bracelet_266"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make geometric agate bracelet pattern 2",
		Category = "Jewellery and devotional work",
		Blurb = "make a geometric agate bracelet",
		Action = "making jewellery or a devotional item",
		ActiveCraftItemSdesc = "an in-progress geometric agate bracelet craft",
		Gate = CatalogueRecipeGates.RenaissanceAndEarlyModernJewelleryGemcraft15,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 3,
		Interruptable = false,
		Phases = [
			new(25, "$0 select|selects $i1 and begin|begins to lay out the work with $t1.", "$0 select|selects $i1 and begin|begins to lay out the work with $t1.", ExertionLevel.Stasis, 0),
			new(35, "$0 shape|shapes the material with $t1 and $t2, checking the joins and fit as the work develops.", "$0 shape|shapes the material with $t1 and $t2, checking the joins and fit as the work develops.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the visible form and set|sets aside $p1 for inspection.", "$0 end|ends the unsuccessful work and set|sets the spoiled material aside.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 22.0, Material: "agate", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("22 grams of agate", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Pliers", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Pliers tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "earlymodern_jewellery_mrd_bracelet_266", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a geometric agate bracelet (@earlymodern_jewellery_mrd_bracelet_266)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition JewelleryAndDevotionalWorkMakeRosetteJadeBroochPattern2() => new()
	{
		Metadata = new("Jewellery and devotional work/make rosette jade brooch pattern 2", "crafts", ["earlymodern", "renaissance"], ["item:earlymodern_jewellery_mrd_brooch_270"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make rosette jade brooch pattern 2",
		Category = "Jewellery and devotional work",
		Blurb = "make a rosette jade brooch",
		Action = "making jewellery or a devotional item",
		ActiveCraftItemSdesc = "an in-progress rosette jade brooch craft",
		Gate = CatalogueRecipeGates.RenaissanceAndEarlyModernJewelleryGemcraft15,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 3,
		Interruptable = false,
		Phases = [
			new(25, "$0 select|selects $i1 and begin|begins to lay out the work with $t1.", "$0 select|selects $i1 and begin|begins to lay out the work with $t1.", ExertionLevel.Stasis, 0),
			new(35, "$0 shape|shapes the material with $t1 and $t2, checking the joins and fit as the work develops.", "$0 shape|shapes the material with $t1 and $t2, checking the joins and fit as the work develops.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the visible form and set|sets aside $p1 for inspection.", "$0 end|ends the unsuccessful work and set|sets the spoiled material aside.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 44.0, Material: "jade", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("44 grams of jade", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Pliers", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Pliers tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "earlymodern_jewellery_mrd_brooch_270", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a rosette jade brooch (@earlymodern_jewellery_mrd_brooch_270)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition JewelleryAndDevotionalWorkMakeVineGarnetNecklacePattern2() => new()
	{
		Metadata = new("Jewellery and devotional work/make vine garnet necklace pattern 2", "crafts", ["earlymodern", "renaissance"], ["item:earlymodern_jewellery_mrd_chain_necklace_265"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make vine garnet necklace pattern 2",
		Category = "Jewellery and devotional work",
		Blurb = "make a vine garnet necklace",
		Action = "making jewellery or a devotional item",
		ActiveCraftItemSdesc = "an in-progress vine garnet necklace craft",
		Gate = CatalogueRecipeGates.RenaissanceAndEarlyModernJewelleryGemcraft15,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 3,
		Interruptable = false,
		Phases = [
			new(25, "$0 select|selects $i1 and begin|begins to lay out the work with $t1.", "$0 select|selects $i1 and begin|begins to lay out the work with $t1.", ExertionLevel.Stasis, 0),
			new(35, "$0 shape|shapes the material with $t1 and $t2, checking the joins and fit as the work develops.", "$0 shape|shapes the material with $t1 and $t2, checking the joins and fit as the work develops.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the visible form and set|sets aside $p1 for inspection.", "$0 end|ends the unsuccessful work and set|sets the spoiled material aside.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 16.0, Material: "garnet", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("16 grams of garnet", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Pliers", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Pliers tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "earlymodern_jewellery_mrd_chain_necklace_265", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a vine garnet necklace (@earlymodern_jewellery_mrd_chain_necklace_265)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition JewelleryAndDevotionalWorkMakeRadiatingPewterCircletPattern2() => new()
	{
		Metadata = new("Jewellery and devotional work/make radiating pewter circlet pattern 2", "crafts", ["earlymodern", "renaissance"], ["item:earlymodern_jewellery_mrd_circlet_273"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make radiating pewter circlet pattern 2",
		Category = "Jewellery and devotional work",
		Blurb = "make a radiating pewter circlet",
		Action = "making jewellery or a devotional item",
		ActiveCraftItemSdesc = "an in-progress radiating pewter circlet craft",
		Gate = CatalogueRecipeGates.RenaissanceAndEarlyModernJewellerySilversmithing15,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 3,
		Interruptable = false,
		Phases = [
			new(25, "$0 select|selects $i1 and begin|begins to lay out the work with $t1.", "$0 select|selects $i1 and begin|begins to lay out the work with $t1.", ExertionLevel.Stasis, 0),
			new(35, "$0 shape|shapes the material with $t1 and $t2, checking the joins and fit as the work develops.", "$0 shape|shapes the material with $t1 and $t2, checking the joins and fit as the work develops.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the visible form and set|sets aside $p1 for inspection.", "$0 end|ends the unsuccessful work and set|sets the spoiled material aside.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 61.0, Material: "pewter", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("61 grams of pewter", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Pliers", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Pliers tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "earlymodern_jewellery_mrd_circlet_273", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a radiating pewter circlet (@earlymodern_jewellery_mrd_circlet_273)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};
}
