#nullable enable

using MudSharp.Body;
using MudSharp.GameItems.Inventory.Plans;
using MudSharp.RPG.Checks;

namespace DatabaseSeeder.Seeders;

public static partial class Recipes14Stock
{
	public static CatalogueRecipeDefinition JewelleryAndDevotionalWorkMakeVineGoldNecklace() => new()
	{
		Metadata = new("Jewellery and devotional work/make vine gold necklace", "crafts", ["earlymodern", "renaissance"], ["item:renaissance_jewellery_common_chain_necklace_95"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make vine gold necklace",
		Category = "Jewellery and devotional work",
		Blurb = "make a vine gold necklace",
		Action = "making jewellery or a devotional item",
		ActiveCraftItemSdesc = "an in-progress vine gold necklace craft",
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
			new CatalogueCommodityRecipeInput(Grams: 50.0, Material: "gold", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("50 grams of gold", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Pliers", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Pliers tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_jewellery_common_chain_necklace_95", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a vine gold necklace (@renaissance_jewellery_common_chain_necklace_95)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition JewelleryAndDevotionalWorkMakeRadiatingCoralCirclet() => new()
	{
		Metadata = new("Jewellery and devotional work/make radiating coral circlet", "crafts", ["earlymodern", "renaissance"], ["item:renaissance_jewellery_common_circlet_103"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make radiating coral circlet",
		Category = "Jewellery and devotional work",
		Blurb = "make a radiating coral circlet",
		Action = "making jewellery or a devotional item",
		ActiveCraftItemSdesc = "an in-progress radiating coral circlet craft",
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
			new CatalogueCommodityRecipeInput(Grams: 33.0, Material: "coral", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("33 grams of coral", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Pliers", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Pliers tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_jewellery_common_circlet_103", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a radiating coral circlet (@renaissance_jewellery_common_circlet_103)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition JewelleryAndDevotionalWorkMakeStarGlassCirclet() => new()
	{
		Metadata = new("Jewellery and devotional work/make star glass circlet", "crafts", ["earlymodern", "renaissance"], ["item:renaissance_jewellery_common_circlet_13"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make star glass circlet",
		Category = "Jewellery and devotional work",
		Blurb = "make a star glass circlet",
		Action = "making jewellery or a devotional item",
		ActiveCraftItemSdesc = "an in-progress star glass circlet craft",
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
			new CatalogueCommodityRecipeInput(Grams: 22.0, Material: "glass", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("22 grams of glass", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Pliers", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Pliers tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_jewellery_common_circlet_13", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a star glass circlet (@renaissance_jewellery_common_circlet_13)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition JewelleryAndDevotionalWorkMakeCheckerPewterCirclet() => new()
	{
		Metadata = new("Jewellery and devotional work/make checker pewter circlet", "crafts", ["earlymodern", "renaissance"], ["item:renaissance_jewellery_common_circlet_133"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make checker pewter circlet",
		Category = "Jewellery and devotional work",
		Blurb = "make a checker pewter circlet",
		Action = "making jewellery or a devotional item",
		ActiveCraftItemSdesc = "an in-progress checker pewter circlet craft",
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
			new CatalogueCommodityRecipeInput(Grams: 16.0, Material: "pewter", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("16 grams of pewter", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Pliers", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Pliers tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_jewellery_common_circlet_133", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a checker pewter circlet (@renaissance_jewellery_common_circlet_133)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition JewelleryAndDevotionalWorkMakeReedIvoryCirclet() => new()
	{
		Metadata = new("Jewellery and devotional work/make reed ivory circlet", "crafts", ["earlymodern", "renaissance"], ["item:renaissance_jewellery_common_circlet_163"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make reed ivory circlet",
		Category = "Jewellery and devotional work",
		Blurb = "make a reed ivory circlet",
		Action = "making jewellery or a devotional item",
		ActiveCraftItemSdesc = "an in-progress reed ivory circlet craft",
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
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_jewellery_common_circlet_163", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a reed ivory circlet (@renaissance_jewellery_common_circlet_163)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition JewelleryAndDevotionalWorkMakeCloudJadeCirclet() => new()
	{
		Metadata = new("Jewellery and devotional work/make cloud jade circlet", "crafts", ["earlymodern", "renaissance"], ["item:renaissance_jewellery_common_circlet_193"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make cloud jade circlet",
		Category = "Jewellery and devotional work",
		Blurb = "make a cloud jade circlet",
		Action = "making jewellery or a devotional item",
		ActiveCraftItemSdesc = "an in-progress cloud jade circlet craft",
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
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_jewellery_common_circlet_193", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a cloud jade circlet (@renaissance_jewellery_common_circlet_193)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition JewelleryAndDevotionalWorkMakeKnottedCarnelianCirclet() => new()
	{
		Metadata = new("Jewellery and devotional work/make knotted carnelian circlet", "crafts", ["earlymodern", "renaissance"], ["item:renaissance_jewellery_common_circlet_43"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make knotted carnelian circlet",
		Category = "Jewellery and devotional work",
		Blurb = "make a knotted carnelian circlet",
		Action = "making jewellery or a devotional item",
		ActiveCraftItemSdesc = "an in-progress knotted carnelian circlet craft",
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
			new CatalogueCommodityRecipeInput(Grams: 66.0, Material: "carnelian", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("66 grams of carnelian", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Pliers", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Pliers tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_jewellery_common_circlet_43", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a knotted carnelian circlet (@renaissance_jewellery_common_circlet_43)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition JewelleryAndDevotionalWorkMakeSunburstSilverCirclet() => new()
	{
		Metadata = new("Jewellery and devotional work/make sunburst silver circlet", "crafts", ["earlymodern", "renaissance"], ["item:renaissance_jewellery_common_circlet_73"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make sunburst silver circlet",
		Category = "Jewellery and devotional work",
		Blurb = "make a sunburst silver circlet",
		Action = "making jewellery or a devotional item",
		ActiveCraftItemSdesc = "an in-progress sunburst silver circlet craft",
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
			new CatalogueCommodityRecipeInput(Grams: 50.0, Material: "silver", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("50 grams of silver", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Pliers", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Pliers tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_jewellery_common_circlet_73", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a sunburst silver circlet (@renaissance_jewellery_common_circlet_73)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition JewelleryAndDevotionalWorkMakeCloudCopperDevotionalBadge() => new()
	{
		Metadata = new("Jewellery and devotional work/make cloud copper devotional badge", "crafts", ["earlymodern", "renaissance"], ["item:renaissance_jewellery_common_devotional_badge_113"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make cloud copper devotional badge",
		Category = "Jewellery and devotional work",
		Blurb = "make a cloud copper devotional badge",
		Action = "making jewellery or a devotional item",
		ActiveCraftItemSdesc = "an in-progress cloud copper devotional badge craft",
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
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_jewellery_common_devotional_badge_113", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a cloud copper devotional badge (@renaissance_jewellery_common_devotional_badge_113)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition JewelleryAndDevotionalWorkMakeVinePearlDevotionalBadge() => new()
	{
		Metadata = new("Jewellery and devotional work/make vine pearl devotional badge", "crafts", ["earlymodern", "renaissance"], ["item:renaissance_jewellery_common_devotional_badge_143"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make vine pearl devotional badge",
		Category = "Jewellery and devotional work",
		Blurb = "make a vine pearl devotional badge",
		Action = "making jewellery or a devotional item",
		ActiveCraftItemSdesc = "an in-progress vine pearl devotional badge craft",
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
			new CatalogueCommodityRecipeInput(Grams: 72.0, Material: "pearl", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("72 grams of pearl", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Pliers", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Pliers tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_jewellery_common_devotional_badge_143", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a vine pearl devotional badge (@renaissance_jewellery_common_devotional_badge_143)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition JewelleryAndDevotionalWorkMakeStarQuartzDevotionalBadge() => new()
	{
		Metadata = new("Jewellery and devotional work/make star quartz devotional badge", "crafts", ["earlymodern", "renaissance"], ["item:renaissance_jewellery_common_devotional_badge_173"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make star quartz devotional badge",
		Category = "Jewellery and devotional work",
		Blurb = "make a star quartz devotional badge",
		Action = "making jewellery or a devotional item",
		ActiveCraftItemSdesc = "an in-progress star quartz devotional badge craft",
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
			new CatalogueCommodityRecipeInput(Grams: 55.0, Material: "quartz", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("55 grams of quartz", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Pliers", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Pliers tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_jewellery_common_devotional_badge_173", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a star quartz devotional badge (@renaissance_jewellery_common_devotional_badge_173)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition JewelleryAndDevotionalWorkMakeKnottedBoneDevotionalBadge() => new()
	{
		Metadata = new("Jewellery and devotional work/make knotted bone devotional badge", "crafts", ["earlymodern", "renaissance"], ["item:renaissance_jewellery_common_devotional_badge_203"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make knotted bone devotional badge",
		Category = "Jewellery and devotional work",
		Blurb = "make a knotted bone devotional badge",
		Action = "making jewellery or a devotional item",
		ActiveCraftItemSdesc = "an in-progress knotted bone devotional badge craft",
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
			new CatalogueCommodityRecipeInput(Grams: 38.0, Material: "bone", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("38 grams of bone", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Pliers", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Pliers tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_jewellery_common_devotional_badge_203", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a knotted bone devotional badge (@renaissance_jewellery_common_devotional_badge_203)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition JewelleryAndDevotionalWorkMakeRadiatingLapisLazuliDevotionalBadge() => new()
	{
		Metadata = new("Jewellery and devotional work/make radiating lapis lazuli devotional badge", "crafts", ["earlymodern", "renaissance"], ["item:renaissance_jewellery_common_devotional_badge_23"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make radiating lapis lazuli devotional badge",
		Category = "Jewellery and devotional work",
		Blurb = "make a radiating lapis lazuli devotional badge",
		Action = "making jewellery or a devotional item",
		ActiveCraftItemSdesc = "an in-progress radiating lapis lazuli devotional badge craft",
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
			new CatalogueCommodityRecipeInput(Grams: 16.0, Material: "lapis lazuli", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("16 grams of lapis lazuli", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Pliers", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Pliers tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_jewellery_common_devotional_badge_23", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a radiating lapis lazuli devotional badge (@renaissance_jewellery_common_devotional_badge_23)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition JewelleryAndDevotionalWorkMakeCheckerGoldDevotionalBadge() => new()
	{
		Metadata = new("Jewellery and devotional work/make checker gold devotional badge", "crafts", ["earlymodern", "renaissance"], ["item:renaissance_jewellery_common_devotional_badge_53"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make checker gold devotional badge",
		Category = "Jewellery and devotional work",
		Blurb = "make a checker gold devotional badge",
		Action = "making jewellery or a devotional item",
		ActiveCraftItemSdesc = "an in-progress checker gold devotional badge craft",
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
			new CatalogueCommodityRecipeInput(Grams: 61.0, Material: "gold", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("61 grams of gold", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Pliers", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Pliers tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_jewellery_common_devotional_badge_53", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a checker gold devotional badge (@renaissance_jewellery_common_devotional_badge_53)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition JewelleryAndDevotionalWorkMakeReedGarnetDevotionalBadge() => new()
	{
		Metadata = new("Jewellery and devotional work/make reed garnet devotional badge", "crafts", ["earlymodern", "renaissance"], ["item:renaissance_jewellery_common_devotional_badge_83"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make reed garnet devotional badge",
		Category = "Jewellery and devotional work",
		Blurb = "make a reed garnet devotional badge",
		Action = "making jewellery or a devotional item",
		ActiveCraftItemSdesc = "an in-progress reed garnet devotional badge craft",
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
			new CatalogueCommodityRecipeInput(Grams: 44.0, Material: "garnet", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("44 grams of garnet", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Pliers", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Pliers tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_jewellery_common_devotional_badge_83", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a reed garnet devotional badge (@renaissance_jewellery_common_devotional_badge_83)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition JewelleryAndDevotionalWorkMakeGeometricPewterDevotionalMedal() => new()
	{
		Metadata = new("Jewellery and devotional work/make geometric pewter devotional medal", "crafts", ["earlymodern", "renaissance"], ["item:renaissance_jewellery_common_devotional_medal_112"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make geometric pewter devotional medal",
		Category = "Jewellery and devotional work",
		Blurb = "make a geometric pewter devotional medal",
		Action = "making jewellery or a devotional item",
		ActiveCraftItemSdesc = "an in-progress geometric pewter devotional medal craft",
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
			new CatalogueCommodityRecipeInput(Grams: 22.0, Material: "pewter", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("22 grams of pewter", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Pliers", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Pliers tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_jewellery_common_devotional_medal_112", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a geometric pewter devotional medal (@renaissance_jewellery_common_devotional_medal_112)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition JewelleryAndDevotionalWorkMakeBirdIvoryDevotionalMedal() => new()
	{
		Metadata = new("Jewellery and devotional work/make bird ivory devotional medal", "crafts", ["earlymodern", "renaissance"], ["item:renaissance_jewellery_common_devotional_medal_142"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make bird ivory devotional medal",
		Category = "Jewellery and devotional work",
		Blurb = "make a bird ivory devotional medal",
		Action = "making jewellery or a devotional item",
		ActiveCraftItemSdesc = "an in-progress bird ivory devotional medal craft",
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
			new CatalogueCommodityRecipeInput(Grams: 66.0, Material: "ivory", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("66 grams of ivory", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Pliers", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Pliers tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_jewellery_common_devotional_medal_142", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a bird ivory devotional medal (@renaissance_jewellery_common_devotional_medal_142)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition JewelleryAndDevotionalWorkMakeFlowerHeadJadeDevotionalMedal() => new()
	{
		Metadata = new("Jewellery and devotional work/make flower-head jade devotional medal", "crafts", ["earlymodern", "renaissance"], ["item:renaissance_jewellery_common_devotional_medal_172"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make flower-head jade devotional medal",
		Category = "Jewellery and devotional work",
		Blurb = "make a flower-head jade devotional medal",
		Action = "making jewellery or a devotional item",
		ActiveCraftItemSdesc = "an in-progress flower-head jade devotional medal craft",
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
			new CatalogueCommodityRecipeInput(Grams: 50.0, Material: "jade", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("50 grams of jade", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Pliers", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Pliers tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_jewellery_common_devotional_medal_172", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a flower-head jade devotional medal (@renaissance_jewellery_common_devotional_medal_172)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition JewelleryAndDevotionalWorkMakeWaterWaveGlassDevotionalMedal() => new()
	{
		Metadata = new("Jewellery and devotional work/make water-wave glass devotional medal", "crafts", ["earlymodern", "renaissance"], ["item:renaissance_jewellery_common_devotional_medal_202"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make water-wave glass devotional medal",
		Category = "Jewellery and devotional work",
		Blurb = "make a water-wave glass devotional medal",
		Action = "making jewellery or a devotional item",
		ActiveCraftItemSdesc = "an in-progress water-wave glass devotional medal craft",
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
			new CatalogueCommodityRecipeInput(Grams: 33.0, Material: "glass", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("33 grams of glass", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Pliers", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Pliers tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_jewellery_common_devotional_medal_202", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a water-wave glass devotional medal (@renaissance_jewellery_common_devotional_medal_202)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition JewelleryAndDevotionalWorkMakeBraidedCarnelianDevotionalMedal() => new()
	{
		Metadata = new("Jewellery and devotional work/make braided carnelian devotional medal", "crafts", ["earlymodern", "renaissance"], ["item:renaissance_jewellery_common_devotional_medal_22"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make braided carnelian devotional medal",
		Category = "Jewellery and devotional work",
		Blurb = "make a braided carnelian devotional medal",
		Action = "making jewellery or a devotional item",
		ActiveCraftItemSdesc = "an in-progress braided carnelian devotional medal craft",
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
			new CatalogueCommodityRecipeInput(Grams: 72.0, Material: "carnelian", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("72 grams of carnelian", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Pliers", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Pliers tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_jewellery_common_devotional_medal_22", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a braided carnelian devotional medal (@renaissance_jewellery_common_devotional_medal_22)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition JewelleryAndDevotionalWorkMakeRosetteSilverDevotionalMedal() => new()
	{
		Metadata = new("Jewellery and devotional work/make rosette silver devotional medal", "crafts", ["earlymodern", "renaissance"], ["item:renaissance_jewellery_common_devotional_medal_52"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make rosette silver devotional medal",
		Category = "Jewellery and devotional work",
		Blurb = "make a rosette silver devotional medal",
		Action = "making jewellery or a devotional item",
		ActiveCraftItemSdesc = "an in-progress rosette silver devotional medal craft",
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
			new CatalogueCommodityRecipeInput(Grams: 55.0, Material: "silver", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("55 grams of silver", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Pliers", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Pliers tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_jewellery_common_devotional_medal_52", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a rosette silver devotional medal (@renaissance_jewellery_common_devotional_medal_52)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition JewelleryAndDevotionalWorkMakeShellCoralDevotionalMedal() => new()
	{
		Metadata = new("Jewellery and devotional work/make shell coral devotional medal", "crafts", ["earlymodern", "renaissance"], ["item:renaissance_jewellery_common_devotional_medal_82"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make shell coral devotional medal",
		Category = "Jewellery and devotional work",
		Blurb = "make a shell coral devotional medal",
		Action = "making jewellery or a devotional item",
		ActiveCraftItemSdesc = "an in-progress shell coral devotional medal craft",
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
			new CatalogueCommodityRecipeInput(Grams: 38.0, Material: "coral", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("38 grams of coral", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Pliers", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Pliers tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_jewellery_common_devotional_medal_82", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a shell coral devotional medal (@renaissance_jewellery_common_devotional_medal_82)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition JewelleryAndDevotionalWorkMakeLeafScrollGarnetDiadem() => new()
	{
		Metadata = new("Jewellery and devotional work/make leaf-scroll garnet diadem", "crafts", ["earlymodern", "renaissance"], ["item:renaissance_jewellery_common_diadem_104"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make leaf-scroll garnet diadem",
		Category = "Jewellery and devotional work",
		Blurb = "make a leaf-scroll garnet diadem",
		Action = "making jewellery or a devotional item",
		ActiveCraftItemSdesc = "an in-progress leaf-scroll garnet diadem craft",
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
			new CatalogueCommodityRecipeInput(Grams: 38.0, Material: "garnet", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("38 grams of garnet", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Pliers", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Pliers tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_jewellery_common_diadem_104", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a leaf-scroll garnet diadem (@renaissance_jewellery_common_diadem_104)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition JewelleryAndDevotionalWorkMakeBraidedCopperDiadem() => new()
	{
		Metadata = new("Jewellery and devotional work/make braided copper diadem", "crafts", ["earlymodern", "renaissance"], ["item:renaissance_jewellery_common_diadem_134"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make braided copper diadem",
		Category = "Jewellery and devotional work",
		Blurb = "make a braided copper diadem",
		Action = "making jewellery or a devotional item",
		ActiveCraftItemSdesc = "an in-progress braided copper diadem craft",
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
			new CatalogueCommodityRecipeInput(Grams: 22.0, Material: "copper", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("22 grams of copper", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Pliers", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Pliers tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_jewellery_common_diadem_134", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a braided copper diadem (@renaissance_jewellery_common_diadem_134)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition JewelleryAndDevotionalWorkMakeBirdBoneDiadem() => new()
	{
		Metadata = new("Jewellery and devotional work/make bird bone diadem", "crafts", ["earlymodern", "renaissance"], ["item:renaissance_jewellery_common_diadem_14"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make bird bone diadem",
		Category = "Jewellery and devotional work",
		Blurb = "make a bird bone diadem",
		Action = "making jewellery or a devotional item",
		ActiveCraftItemSdesc = "an in-progress bird bone diadem craft",
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
			new CatalogueCommodityRecipeInput(Grams: 27.0, Material: "bone", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("27 grams of bone", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Pliers", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Pliers tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_jewellery_common_diadem_14", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a bird bone diadem (@renaissance_jewellery_common_diadem_14)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition JewelleryAndDevotionalWorkMakeRosettePearlDiadem() => new()
	{
		Metadata = new("Jewellery and devotional work/make rosette pearl diadem", "crafts", ["earlymodern", "renaissance"], ["item:renaissance_jewellery_common_diadem_164"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make rosette pearl diadem",
		Category = "Jewellery and devotional work",
		Blurb = "make a rosette pearl diadem",
		Action = "making jewellery or a devotional item",
		ActiveCraftItemSdesc = "an in-progress rosette pearl diadem craft",
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
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_jewellery_common_diadem_164", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a rosette pearl diadem (@renaissance_jewellery_common_diadem_164)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition JewelleryAndDevotionalWorkMakeShellQuartzDiadem() => new()
	{
		Metadata = new("Jewellery and devotional work/make shell quartz diadem", "crafts", ["earlymodern", "renaissance"], ["item:renaissance_jewellery_common_diadem_194"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make shell quartz diadem",
		Category = "Jewellery and devotional work",
		Blurb = "make a shell quartz diadem",
		Action = "making jewellery or a devotional item",
		ActiveCraftItemSdesc = "an in-progress shell quartz diadem craft",
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
			new CatalogueCommodityRecipeInput(Grams: 50.0, Material: "quartz", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("50 grams of quartz", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Pliers", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Pliers tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_jewellery_common_diadem_194", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a shell quartz diadem (@renaissance_jewellery_common_diadem_194)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition JewelleryAndDevotionalWorkMakeFlowerHeadLapisLazuliDiadem() => new()
	{
		Metadata = new("Jewellery and devotional work/make flower-head lapis lazuli diadem", "crafts", ["earlymodern", "renaissance"], ["item:renaissance_jewellery_common_diadem_44"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make flower-head lapis lazuli diadem",
		Category = "Jewellery and devotional work",
		Blurb = "make a flower-head lapis lazuli diadem",
		Action = "making jewellery or a devotional item",
		ActiveCraftItemSdesc = "an in-progress flower-head lapis lazuli diadem craft",
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
			new CatalogueCommodityRecipeInput(Grams: 72.0, Material: "lapis lazuli", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("72 grams of lapis lazuli", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Pliers", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Pliers tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_jewellery_common_diadem_44", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a flower-head lapis lazuli diadem (@renaissance_jewellery_common_diadem_44)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition JewelleryAndDevotionalWorkMakeWaterWaveGoldDiadem() => new()
	{
		Metadata = new("Jewellery and devotional work/make water-wave gold diadem", "crafts", ["earlymodern", "renaissance"], ["item:renaissance_jewellery_common_diadem_74"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make water-wave gold diadem",
		Category = "Jewellery and devotional work",
		Blurb = "make a water-wave gold diadem",
		Action = "making jewellery or a devotional item",
		ActiveCraftItemSdesc = "an in-progress water-wave gold diadem craft",
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
			new CatalogueCommodityRecipeInput(Grams: 55.0, Material: "gold", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("55 grams of gold", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Pliers", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Pliers tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_jewellery_common_diadem_74", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a water-wave gold diadem (@renaissance_jewellery_common_diadem_74)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition JewelleryAndDevotionalWorkMakeSunburstBronzeEarrings() => new()
	{
		Metadata = new("Jewellery and devotional work/make sunburst bronze earrings", "crafts", ["earlymodern", "renaissance"], ["item:renaissance_jewellery_common_earrings_09"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make sunburst bronze earrings",
		Category = "Jewellery and devotional work",
		Blurb = "make a sunburst bronze earrings",
		Action = "making jewellery or a devotional item",
		ActiveCraftItemSdesc = "an in-progress sunburst bronze earrings craft",
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
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_jewellery_common_earrings_09", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a sunburst bronze earrings (@renaissance_jewellery_common_earrings_09)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition JewelleryAndDevotionalWorkMakeCloudTurquoiseEarrings() => new()
	{
		Metadata = new("Jewellery and devotional work/make cloud turquoise earrings", "crafts", ["earlymodern", "renaissance"], ["item:renaissance_jewellery_common_earrings_129"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make cloud turquoise earrings",
		Category = "Jewellery and devotional work",
		Blurb = "make a cloud turquoise earrings",
		Action = "making jewellery or a devotional item",
		ActiveCraftItemSdesc = "an in-progress cloud turquoise earrings craft",
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
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_jewellery_common_earrings_129", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a cloud turquoise earrings (@renaissance_jewellery_common_earrings_129)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition JewelleryAndDevotionalWorkMakeVineBrassEarrings() => new()
	{
		Metadata = new("Jewellery and devotional work/make vine brass earrings", "crafts", ["earlymodern", "renaissance"], ["item:renaissance_jewellery_common_earrings_159"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make vine brass earrings",
		Category = "Jewellery and devotional work",
		Blurb = "make a vine brass earrings",
		Action = "making jewellery or a devotional item",
		ActiveCraftItemSdesc = "an in-progress vine brass earrings craft",
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
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_jewellery_common_earrings_159", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a vine brass earrings (@renaissance_jewellery_common_earrings_159)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition JewelleryAndDevotionalWorkMakeStarAgateEarrings() => new()
	{
		Metadata = new("Jewellery and devotional work/make star agate earrings", "crafts", ["earlymodern", "renaissance"], ["item:renaissance_jewellery_common_earrings_189"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make star agate earrings",
		Category = "Jewellery and devotional work",
		Blurb = "make a star agate earrings",
		Action = "making jewellery or a devotional item",
		ActiveCraftItemSdesc = "an in-progress star agate earrings craft",
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
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_jewellery_common_earrings_189", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a star agate earrings (@renaissance_jewellery_common_earrings_189)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition JewelleryAndDevotionalWorkMakeKnottedBronzeEarrings() => new()
	{
		Metadata = new("Jewellery and devotional work/make knotted bronze earrings", "crafts", ["earlymodern", "renaissance"], ["item:renaissance_jewellery_common_earrings_219"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make knotted bronze earrings",
		Category = "Jewellery and devotional work",
		Blurb = "make a knotted bronze earrings",
		Action = "making jewellery or a devotional item",
		ActiveCraftItemSdesc = "an in-progress knotted bronze earrings craft",
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
			new CatalogueCommodityRecipeInput(Grams: 66.0, Material: "bronze", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("66 grams of bronze", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Pliers", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Pliers tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_jewellery_common_earrings_219", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a knotted bronze earrings (@renaissance_jewellery_common_earrings_219)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition JewelleryAndDevotionalWorkMakeRadiatingAmberEarrings() => new()
	{
		Metadata = new("Jewellery and devotional work/make radiating amber earrings", "crafts", ["earlymodern", "renaissance"], ["item:renaissance_jewellery_common_earrings_39"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make radiating amber earrings",
		Category = "Jewellery and devotional work",
		Blurb = "make a radiating amber earrings",
		Action = "making jewellery or a devotional item",
		ActiveCraftItemSdesc = "an in-progress radiating amber earrings craft",
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
			new CatalogueCommodityRecipeInput(Grams: 44.0, Material: "amber", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("44 grams of amber", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Pliers", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Pliers tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_jewellery_common_earrings_39", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a radiating amber earrings (@renaissance_jewellery_common_earrings_39)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition JewelleryAndDevotionalWorkMakeCheckerFaienceEarrings() => new()
	{
		Metadata = new("Jewellery and devotional work/make checker faience earrings", "crafts", ["earlymodern", "renaissance"], ["item:renaissance_jewellery_common_earrings_69"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make checker faience earrings",
		Category = "Jewellery and devotional work",
		Blurb = "make a checker faience earrings",
		Action = "making jewellery or a devotional item",
		ActiveCraftItemSdesc = "an in-progress checker faience earrings craft",
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
			new CatalogueCommodityRecipeInput(Grams: 27.0, Material: "faience", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("27 grams of faience", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Pliers", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Pliers tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_jewellery_common_earrings_69", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a checker faience earrings (@renaissance_jewellery_common_earrings_69)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition JewelleryAndDevotionalWorkMakeReedShellEarrings() => new()
	{
		Metadata = new("Jewellery and devotional work/make reed shell earrings", "crafts", ["earlymodern", "renaissance"], ["item:renaissance_jewellery_common_earrings_99"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make reed shell earrings",
		Category = "Jewellery and devotional work",
		Blurb = "make a reed shell earrings",
		Action = "making jewellery or a devotional item",
		ActiveCraftItemSdesc = "an in-progress reed shell earrings craft",
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
			new CatalogueCommodityRecipeInput(Grams: 72.0, Material: "shell", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("72 grams of shell", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Pliers", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Pliers tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_jewellery_common_earrings_99", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a reed shell earrings (@renaissance_jewellery_common_earrings_99)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition JewelleryAndDevotionalWorkMakeBraidedAmberHairpin() => new()
	{
		Metadata = new("Jewellery and devotional work/make braided amber hairpin", "crafts", ["earlymodern", "renaissance"], ["item:renaissance_jewellery_common_hairpin_102"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make braided amber hairpin",
		Category = "Jewellery and devotional work",
		Blurb = "make a braided amber hairpin",
		Action = "making jewellery or a devotional item",
		ActiveCraftItemSdesc = "an in-progress braided amber hairpin craft",
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
			new CatalogueCommodityRecipeInput(Grams: 27.0, Material: "amber", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("27 grams of amber", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Pliers", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Pliers tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_jewellery_common_hairpin_102", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a braided amber hairpin (@renaissance_jewellery_common_hairpin_102)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition JewelleryAndDevotionalWorkMakeFlowerHeadBrassHairpin() => new()
	{
		Metadata = new("Jewellery and devotional work/make flower-head brass hairpin", "crafts", ["earlymodern", "renaissance"], ["item:renaissance_jewellery_common_hairpin_12"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make flower-head brass hairpin",
		Category = "Jewellery and devotional work",
		Blurb = "make a flower-head brass hairpin",
		Action = "making jewellery or a devotional item",
		ActiveCraftItemSdesc = "an in-progress flower-head brass hairpin craft",
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
			new CatalogueCommodityRecipeInput(Grams: 16.0, Material: "brass", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("16 grams of brass", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Pliers", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Pliers tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_jewellery_common_hairpin_12", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a flower-head brass hairpin (@renaissance_jewellery_common_hairpin_12)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition JewelleryAndDevotionalWorkMakeRosetteFaienceHairpin() => new()
	{
		Metadata = new("Jewellery and devotional work/make rosette faience hairpin", "crafts", ["earlymodern", "renaissance"], ["item:renaissance_jewellery_common_hairpin_132"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make rosette faience hairpin",
		Category = "Jewellery and devotional work",
		Blurb = "make a rosette faience hairpin",
		Action = "making jewellery or a devotional item",
		ActiveCraftItemSdesc = "an in-progress rosette faience hairpin craft",
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
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_jewellery_common_hairpin_132", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a rosette faience hairpin (@renaissance_jewellery_common_hairpin_132)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition JewelleryAndDevotionalWorkMakeShellShellHairpin() => new()
	{
		Metadata = new("Jewellery and devotional work/make shell shell hairpin", "crafts", ["earlymodern", "renaissance"], ["item:renaissance_jewellery_common_hairpin_162"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make shell shell hairpin",
		Category = "Jewellery and devotional work",
		Blurb = "make a shell shell hairpin",
		Action = "making jewellery or a devotional item",
		ActiveCraftItemSdesc = "an in-progress shell shell hairpin craft",
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
			new CatalogueCommodityRecipeInput(Grams: 55.0, Material: "shell", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("55 grams of shell", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Pliers", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Pliers tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_jewellery_common_hairpin_162", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a shell shell hairpin (@renaissance_jewellery_common_hairpin_162)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition JewelleryAndDevotionalWorkMakeGeometricTurquoiseHairpin() => new()
	{
		Metadata = new("Jewellery and devotional work/make geometric turquoise hairpin", "crafts", ["earlymodern", "renaissance"], ["item:renaissance_jewellery_common_hairpin_192"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make geometric turquoise hairpin",
		Category = "Jewellery and devotional work",
		Blurb = "make a geometric turquoise hairpin",
		Action = "making jewellery or a devotional item",
		ActiveCraftItemSdesc = "an in-progress geometric turquoise hairpin craft",
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
			new CatalogueCommodityRecipeInput(Grams: 38.0, Material: "turquoise", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("38 grams of turquoise", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Pliers", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Pliers tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_jewellery_common_hairpin_192", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a geometric turquoise hairpin (@renaissance_jewellery_common_hairpin_192)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition JewelleryAndDevotionalWorkMakeWaterWaveAgateHairpin() => new()
	{
		Metadata = new("Jewellery and devotional work/make water-wave agate hairpin", "crafts", ["earlymodern", "renaissance"], ["item:renaissance_jewellery_common_hairpin_42"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make water-wave agate hairpin",
		Category = "Jewellery and devotional work",
		Blurb = "make a water-wave agate hairpin",
		Action = "making jewellery or a devotional item",
		ActiveCraftItemSdesc = "an in-progress water-wave agate hairpin craft",
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
			new CatalogueCommodityRecipeInput(Grams: 61.0, Material: "agate", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("61 grams of agate", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Pliers", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Pliers tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_jewellery_common_hairpin_42", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a water-wave agate hairpin (@renaissance_jewellery_common_hairpin_42)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition JewelleryAndDevotionalWorkMakeLeafScrollBronzeHairpin() => new()
	{
		Metadata = new("Jewellery and devotional work/make leaf-scroll bronze hairpin", "crafts", ["earlymodern", "renaissance"], ["item:renaissance_jewellery_common_hairpin_72"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make leaf-scroll bronze hairpin",
		Category = "Jewellery and devotional work",
		Blurb = "make a leaf-scroll bronze hairpin",
		Action = "making jewellery or a devotional item",
		ActiveCraftItemSdesc = "an in-progress leaf-scroll bronze hairpin craft",
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
			new CatalogueCommodityRecipeInput(Grams: 44.0, Material: "bronze", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("44 grams of bronze", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Pliers", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Pliers tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_jewellery_common_hairpin_72", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a leaf-scroll bronze hairpin (@renaissance_jewellery_common_hairpin_72)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition JewelleryAndDevotionalWorkMakeBraidedBrassIncenseHolder() => new()
	{
		Metadata = new("Jewellery and devotional work/make braided brass incense holder", "crafts", ["earlymodern", "renaissance"], ["item:renaissance_jewellery_common_incense_holder_118"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make braided brass incense holder",
		Category = "Jewellery and devotional work",
		Blurb = "make a braided brass incense holder",
		Action = "making jewellery or a devotional item",
		ActiveCraftItemSdesc = "an in-progress braided brass incense holder craft",
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
			new CatalogueCommodityRecipeInput(Grams: 55.0, Material: "brass", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("55 grams of brass", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Pliers", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Pliers tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_jewellery_common_incense_holder_118", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a braided brass incense holder (@renaissance_jewellery_common_incense_holder_118)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition JewelleryAndDevotionalWorkMakeRosettePewterIncenseHolder() => new()
	{
		Metadata = new("Jewellery and devotional work/make rosette pewter incense holder", "crafts", ["earlymodern", "renaissance"], ["item:renaissance_jewellery_common_incense_holder_148"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make rosette pewter incense holder",
		Category = "Jewellery and devotional work",
		Blurb = "make a rosette pewter incense holder",
		Action = "making jewellery or a devotional item",
		ActiveCraftItemSdesc = "an in-progress rosette pewter incense holder craft",
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
			new CatalogueCommodityRecipeInput(Grams: 38.0, Material: "pewter", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("38 grams of pewter", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Pliers", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Pliers tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_jewellery_common_incense_holder_148", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a rosette pewter incense holder (@renaissance_jewellery_common_incense_holder_148)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition JewelleryAndDevotionalWorkMakeShellBrassIncenseHolder() => new()
	{
		Metadata = new("Jewellery and devotional work/make shell brass incense holder", "crafts", ["earlymodern", "renaissance"], ["item:renaissance_jewellery_common_incense_holder_178"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make shell brass incense holder",
		Category = "Jewellery and devotional work",
		Blurb = "make a shell brass incense holder",
		Action = "making jewellery or a devotional item",
		ActiveCraftItemSdesc = "an in-progress shell brass incense holder craft",
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
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_jewellery_common_incense_holder_178", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a shell brass incense holder (@renaissance_jewellery_common_incense_holder_178)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition JewelleryAndDevotionalWorkMakeGeometricPewterIncenseHolder() => new()
	{
		Metadata = new("Jewellery and devotional work/make geometric pewter incense holder", "crafts", ["earlymodern", "renaissance"], ["item:renaissance_jewellery_common_incense_holder_208"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make geometric pewter incense holder",
		Category = "Jewellery and devotional work",
		Blurb = "make a geometric pewter incense holder",
		Action = "making jewellery or a devotional item",
		ActiveCraftItemSdesc = "an in-progress geometric pewter incense holder craft",
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
			new CatalogueCommodityRecipeInput(Grams: 66.0, Material: "pewter", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("66 grams of pewter", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Pliers", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Pliers tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_jewellery_common_incense_holder_208", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a geometric pewter incense holder (@renaissance_jewellery_common_incense_holder_208)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition JewelleryAndDevotionalWorkMakeFlowerHeadPewterIncenseHolder() => new()
	{
		Metadata = new("Jewellery and devotional work/make flower-head pewter incense holder", "crafts", ["earlymodern", "renaissance"], ["item:renaissance_jewellery_common_incense_holder_28"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make flower-head pewter incense holder",
		Category = "Jewellery and devotional work",
		Blurb = "make a flower-head pewter incense holder",
		Action = "making jewellery or a devotional item",
		ActiveCraftItemSdesc = "an in-progress flower-head pewter incense holder craft",
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
			new CatalogueCommodityRecipeInput(Grams: 44.0, Material: "pewter", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("44 grams of pewter", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Pliers", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Pliers tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_jewellery_common_incense_holder_28", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a flower-head pewter incense holder (@renaissance_jewellery_common_incense_holder_28)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition JewelleryAndDevotionalWorkMakeWaterWaveBrassIncenseHolder() => new()
	{
		Metadata = new("Jewellery and devotional work/make water-wave brass incense holder", "crafts", ["earlymodern", "renaissance"], ["item:renaissance_jewellery_common_incense_holder_58"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make water-wave brass incense holder",
		Category = "Jewellery and devotional work",
		Blurb = "make a water-wave brass incense holder",
		Action = "making jewellery or a devotional item",
		ActiveCraftItemSdesc = "an in-progress water-wave brass incense holder craft",
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
			new CatalogueCommodityRecipeInput(Grams: 27.0, Material: "brass", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("27 grams of brass", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Pliers", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Pliers tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_jewellery_common_incense_holder_58", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a water-wave brass incense holder (@renaissance_jewellery_common_incense_holder_58)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition JewelleryAndDevotionalWorkMakeLeafScrollPewterIncenseHolder() => new()
	{
		Metadata = new("Jewellery and devotional work/make leaf-scroll pewter incense holder", "crafts", ["earlymodern", "renaissance"], ["item:renaissance_jewellery_common_incense_holder_88"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make leaf-scroll pewter incense holder",
		Category = "Jewellery and devotional work",
		Blurb = "make a leaf-scroll pewter incense holder",
		Action = "making jewellery or a devotional item",
		ActiveCraftItemSdesc = "an in-progress leaf-scroll pewter incense holder craft",
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
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_jewellery_common_incense_holder_88", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a leaf-scroll pewter incense holder (@renaissance_jewellery_common_incense_holder_88)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition JewelleryAndDevotionalWorkMakeCheckerBronzeOfferingBowl() => new()
	{
		Metadata = new("Jewellery and devotional work/make checker bronze offering bowl", "crafts", ["earlymodern", "renaissance"], ["item:renaissance_jewellery_common_offering_bowl_117"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make checker bronze offering bowl",
		Category = "Jewellery and devotional work",
		Blurb = "make a checker bronze offering bowl",
		Action = "making jewellery or a devotional item",
		ActiveCraftItemSdesc = "an in-progress checker bronze offering bowl craft",
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
			new CatalogueCommodityRecipeInput(Grams: 50.0, Material: "bronze", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("50 grams of bronze", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Pliers", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Pliers tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_jewellery_common_offering_bowl_117", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a checker bronze offering bowl (@renaissance_jewellery_common_offering_bowl_117)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition JewelleryAndDevotionalWorkMakeReedSilverOfferingBowl() => new()
	{
		Metadata = new("Jewellery and devotional work/make reed silver offering bowl", "crafts", ["earlymodern", "renaissance"], ["item:renaissance_jewellery_common_offering_bowl_147"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make reed silver offering bowl",
		Category = "Jewellery and devotional work",
		Blurb = "make a reed silver offering bowl",
		Action = "making jewellery or a devotional item",
		ActiveCraftItemSdesc = "an in-progress reed silver offering bowl craft",
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
			new CatalogueCommodityRecipeInput(Grams: 33.0, Material: "silver", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("33 grams of silver", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Pliers", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Pliers tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_jewellery_common_offering_bowl_147", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a reed silver offering bowl (@renaissance_jewellery_common_offering_bowl_147)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition JewelleryAndDevotionalWorkMakeCloudBronzeOfferingBowl() => new()
	{
		Metadata = new("Jewellery and devotional work/make cloud bronze offering bowl", "crafts", ["earlymodern", "renaissance"], ["item:renaissance_jewellery_common_offering_bowl_177"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make cloud bronze offering bowl",
		Category = "Jewellery and devotional work",
		Blurb = "make a cloud bronze offering bowl",
		Action = "making jewellery or a devotional item",
		ActiveCraftItemSdesc = "an in-progress cloud bronze offering bowl craft",
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
			new CatalogueCommodityRecipeInput(Grams: 16.0, Material: "bronze", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("16 grams of bronze", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Pliers", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Pliers tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_jewellery_common_offering_bowl_177", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a cloud bronze offering bowl (@renaissance_jewellery_common_offering_bowl_177)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition JewelleryAndDevotionalWorkMakeVineSilverOfferingBowl() => new()
	{
		Metadata = new("Jewellery and devotional work/make vine silver offering bowl", "crafts", ["earlymodern", "renaissance"], ["item:renaissance_jewellery_common_offering_bowl_207"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make vine silver offering bowl",
		Category = "Jewellery and devotional work",
		Blurb = "make a vine silver offering bowl",
		Action = "making jewellery or a devotional item",
		ActiveCraftItemSdesc = "an in-progress vine silver offering bowl craft",
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
			new CatalogueCommodityRecipeInput(Grams: 61.0, Material: "silver", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("61 grams of silver", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Pliers", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Pliers tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_jewellery_common_offering_bowl_207", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a vine silver offering bowl (@renaissance_jewellery_common_offering_bowl_207)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition JewelleryAndDevotionalWorkMakeKnottedSilverOfferingBowl() => new()
	{
		Metadata = new("Jewellery and devotional work/make knotted silver offering bowl", "crafts", ["earlymodern", "renaissance"], ["item:renaissance_jewellery_common_offering_bowl_27"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make knotted silver offering bowl",
		Category = "Jewellery and devotional work",
		Blurb = "make a knotted silver offering bowl",
		Action = "making jewellery or a devotional item",
		ActiveCraftItemSdesc = "an in-progress knotted silver offering bowl craft",
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
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_jewellery_common_offering_bowl_27", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a knotted silver offering bowl (@renaissance_jewellery_common_offering_bowl_27)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition JewelleryAndDevotionalWorkMakeSunburstBronzeOfferingBowl() => new()
	{
		Metadata = new("Jewellery and devotional work/make sunburst bronze offering bowl", "crafts", ["earlymodern", "renaissance"], ["item:renaissance_jewellery_common_offering_bowl_57"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make sunburst bronze offering bowl",
		Category = "Jewellery and devotional work",
		Blurb = "make a sunburst bronze offering bowl",
		Action = "making jewellery or a devotional item",
		ActiveCraftItemSdesc = "an in-progress sunburst bronze offering bowl craft",
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
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_jewellery_common_offering_bowl_57", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a sunburst bronze offering bowl (@renaissance_jewellery_common_offering_bowl_57)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition JewelleryAndDevotionalWorkMakeRadiatingSilverOfferingBowl() => new()
	{
		Metadata = new("Jewellery and devotional work/make radiating silver offering bowl", "crafts", ["earlymodern", "renaissance"], ["item:renaissance_jewellery_common_offering_bowl_87"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make radiating silver offering bowl",
		Category = "Jewellery and devotional work",
		Blurb = "make a radiating silver offering bowl",
		Action = "making jewellery or a devotional item",
		ActiveCraftItemSdesc = "an in-progress radiating silver offering bowl craft",
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
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_jewellery_common_offering_bowl_87", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a radiating silver offering bowl (@renaissance_jewellery_common_offering_bowl_87)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition JewelleryAndDevotionalWorkMakeReedTurquoisePendant() => new()
	{
		Metadata = new("Jewellery and devotional work/make reed turquoise pendant", "crafts", ["earlymodern", "renaissance"], ["item:renaissance_jewellery_common_pendant_03"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make reed turquoise pendant",
		Category = "Jewellery and devotional work",
		Blurb = "make a reed turquoise pendant",
		Action = "making jewellery or a devotional item",
		ActiveCraftItemSdesc = "an in-progress reed turquoise pendant craft",
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
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_jewellery_common_pendant_03", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a reed turquoise pendant (@renaissance_jewellery_common_pendant_03)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition JewelleryAndDevotionalWorkMakeKnottedAmberPendant() => new()
	{
		Metadata = new("Jewellery and devotional work/make knotted amber pendant", "crafts", ["earlymodern", "renaissance"], ["item:renaissance_jewellery_common_pendant_123"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make knotted amber pendant",
		Category = "Jewellery and devotional work",
		Blurb = "make a knotted amber pendant",
		Action = "making jewellery or a devotional item",
		ActiveCraftItemSdesc = "an in-progress knotted amber pendant craft",
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
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_jewellery_common_pendant_123", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a knotted amber pendant (@renaissance_jewellery_common_pendant_123)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};
}
