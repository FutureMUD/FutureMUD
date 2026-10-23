#nullable enable

using MudSharp.Body;
using MudSharp.GameItems.Inventory.Plans;
using MudSharp.RPG.Checks;

namespace DatabaseSeeder.Seeders;

public static partial class Recipes15Stock
{
	public static CatalogueRecipeDefinition JewelleryAndDevotionalWorkMakeCloudCoralVotiveToken() => new()
	{
		Metadata = new("Jewellery and devotional work/make cloud coral votive token", "crafts", ["earlymodern", "renaissance"], ["item:renaissance_jewellery_common_votive_token_145"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make cloud coral votive token",
		Category = "Jewellery and devotional work",
		Blurb = "make a cloud coral votive token",
		Action = "making jewellery or a devotional item",
		ActiveCraftItemSdesc = "an in-progress cloud coral votive token craft",
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
			new CatalogueCommodityRecipeInput(Grams: 22.0, Material: "coral", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("22 grams of coral", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Pliers", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Pliers tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_jewellery_common_votive_token_145", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a cloud coral votive token (@renaissance_jewellery_common_votive_token_145)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition JewelleryAndDevotionalWorkMakeVinePewterVotiveToken() => new()
	{
		Metadata = new("Jewellery and devotional work/make vine pewter votive token", "crafts", ["earlymodern", "renaissance"], ["item:renaissance_jewellery_common_votive_token_175"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make vine pewter votive token",
		Category = "Jewellery and devotional work",
		Blurb = "make a vine pewter votive token",
		Action = "making jewellery or a devotional item",
		ActiveCraftItemSdesc = "an in-progress vine pewter votive token craft",
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
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_jewellery_common_votive_token_175", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a vine pewter votive token (@renaissance_jewellery_common_votive_token_175)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition JewelleryAndDevotionalWorkMakeStarIvoryVotiveToken() => new()
	{
		Metadata = new("Jewellery and devotional work/make star ivory votive token", "crafts", ["earlymodern", "renaissance"], ["item:renaissance_jewellery_common_votive_token_205"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make star ivory votive token",
		Category = "Jewellery and devotional work",
		Blurb = "make a star ivory votive token",
		Action = "making jewellery or a devotional item",
		ActiveCraftItemSdesc = "an in-progress star ivory votive token craft",
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
			new CatalogueCommodityRecipeInput(Grams: 50.0, Material: "ivory", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("50 grams of ivory", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Pliers", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Pliers tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_jewellery_common_votive_token_205", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a star ivory votive token (@renaissance_jewellery_common_votive_token_205)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition JewelleryAndDevotionalWorkMakeSunburstJadeVotiveToken() => new()
	{
		Metadata = new("Jewellery and devotional work/make sunburst jade votive token", "crafts", ["earlymodern", "renaissance"], ["item:renaissance_jewellery_common_votive_token_25"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make sunburst jade votive token",
		Category = "Jewellery and devotional work",
		Blurb = "make a sunburst jade votive token",
		Action = "making jewellery or a devotional item",
		ActiveCraftItemSdesc = "an in-progress sunburst jade votive token craft",
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
			new CatalogueCommodityRecipeInput(Grams: 27.0, Material: "jade", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("27 grams of jade", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Pliers", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Pliers tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_jewellery_common_votive_token_25", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a sunburst jade votive token (@renaissance_jewellery_common_votive_token_25)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition JewelleryAndDevotionalWorkMakeRadiatingGlassVotiveToken() => new()
	{
		Metadata = new("Jewellery and devotional work/make radiating glass votive token", "crafts", ["earlymodern", "renaissance"], ["item:renaissance_jewellery_common_votive_token_55"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make radiating glass votive token",
		Category = "Jewellery and devotional work",
		Blurb = "make a radiating glass votive token",
		Action = "making jewellery or a devotional item",
		ActiveCraftItemSdesc = "an in-progress radiating glass votive token craft",
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
			new CatalogueCommodityRecipeInput(Grams: 72.0, Material: "glass", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("72 grams of glass", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Pliers", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Pliers tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_jewellery_common_votive_token_55", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a radiating glass votive token (@renaissance_jewellery_common_votive_token_55)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition JewelleryAndDevotionalWorkMakeCheckerCarnelianVotiveToken() => new()
	{
		Metadata = new("Jewellery and devotional work/make checker carnelian votive token", "crafts", ["earlymodern", "renaissance"], ["item:renaissance_jewellery_common_votive_token_85"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make checker carnelian votive token",
		Category = "Jewellery and devotional work",
		Blurb = "make a checker carnelian votive token",
		Action = "making jewellery or a devotional item",
		ActiveCraftItemSdesc = "an in-progress checker carnelian votive token craft",
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
			new CatalogueCommodityRecipeInput(Grams: 55.0, Material: "carnelian", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("55 grams of carnelian", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Pliers", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Pliers tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_jewellery_common_votive_token_85", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a checker carnelian votive token (@renaissance_jewellery_common_votive_token_85)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition JewelleryAndDevotionalWorkMakeSunburstAgateWaistChain() => new()
	{
		Metadata = new("Jewellery and devotional work/make sunburst agate waist chain", "crafts", ["earlymodern", "renaissance"], ["item:renaissance_jewellery_common_waist_chain_105"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make sunburst agate waist chain",
		Category = "Jewellery and devotional work",
		Blurb = "make a sunburst agate waist chain",
		Action = "making jewellery or a devotional item",
		ActiveCraftItemSdesc = "an in-progress sunburst agate waist chain craft",
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
			new CatalogueCommodityRecipeInput(Grams: 44.0, Material: "agate", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("44 grams of agate", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Pliers", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Pliers tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_jewellery_common_waist_chain_105", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a sunburst agate waist chain (@renaissance_jewellery_common_waist_chain_105)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition JewelleryAndDevotionalWorkMakeRadiatingBronzeWaistChain() => new()
	{
		Metadata = new("Jewellery and devotional work/make radiating bronze waist chain", "crafts", ["earlymodern", "renaissance"], ["item:renaissance_jewellery_common_waist_chain_135"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make radiating bronze waist chain",
		Category = "Jewellery and devotional work",
		Blurb = "make a radiating bronze waist chain",
		Action = "making jewellery or a devotional item",
		ActiveCraftItemSdesc = "an in-progress radiating bronze waist chain craft",
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
			new CatalogueCommodityRecipeInput(Grams: 27.0, Material: "bronze", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("27 grams of bronze", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Pliers", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Pliers tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_jewellery_common_waist_chain_135", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a radiating bronze waist chain (@renaissance_jewellery_common_waist_chain_135)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition JewelleryAndDevotionalWorkMakeVineShellWaistChain() => new()
	{
		Metadata = new("Jewellery and devotional work/make vine shell waist chain", "crafts", ["earlymodern", "renaissance"], ["item:renaissance_jewellery_common_waist_chain_15"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make vine shell waist chain",
		Category = "Jewellery and devotional work",
		Blurb = "make a vine shell waist chain",
		Action = "making jewellery or a devotional item",
		ActiveCraftItemSdesc = "an in-progress vine shell waist chain craft",
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
			new CatalogueCommodityRecipeInput(Grams: 33.0, Material: "shell", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("33 grams of shell", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Pliers", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Pliers tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_jewellery_common_waist_chain_15", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a vine shell waist chain (@renaissance_jewellery_common_waist_chain_15)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition JewelleryAndDevotionalWorkMakeCheckerAmberWaistChain() => new()
	{
		Metadata = new("Jewellery and devotional work/make checker amber waist chain", "crafts", ["earlymodern", "renaissance"], ["item:renaissance_jewellery_common_waist_chain_165"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make checker amber waist chain",
		Category = "Jewellery and devotional work",
		Blurb = "make a checker amber waist chain",
		Action = "making jewellery or a devotional item",
		ActiveCraftItemSdesc = "an in-progress checker amber waist chain craft",
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
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_jewellery_common_waist_chain_165", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a checker amber waist chain (@renaissance_jewellery_common_waist_chain_165)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition JewelleryAndDevotionalWorkMakeReedFaienceWaistChain() => new()
	{
		Metadata = new("Jewellery and devotional work/make reed faience waist chain", "crafts", ["earlymodern", "renaissance"], ["item:renaissance_jewellery_common_waist_chain_195"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make reed faience waist chain",
		Category = "Jewellery and devotional work",
		Blurb = "make a reed faience waist chain",
		Action = "making jewellery or a devotional item",
		ActiveCraftItemSdesc = "an in-progress reed faience waist chain craft",
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
			new CatalogueCommodityRecipeInput(Grams: 55.0, Material: "faience", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("55 grams of faience", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Pliers", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Pliers tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_jewellery_common_waist_chain_195", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a reed faience waist chain (@renaissance_jewellery_common_waist_chain_195)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition JewelleryAndDevotionalWorkMakeStarTurquoiseWaistChain() => new()
	{
		Metadata = new("Jewellery and devotional work/make star turquoise waist chain", "crafts", ["earlymodern", "renaissance"], ["item:renaissance_jewellery_common_waist_chain_45"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make star turquoise waist chain",
		Category = "Jewellery and devotional work",
		Blurb = "make a star turquoise waist chain",
		Action = "making jewellery or a devotional item",
		ActiveCraftItemSdesc = "an in-progress star turquoise waist chain craft",
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
			new CatalogueCommodityRecipeInput(Grams: 16.0, Material: "turquoise", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("16 grams of turquoise", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Pliers", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Pliers tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_jewellery_common_waist_chain_45", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a star turquoise waist chain (@renaissance_jewellery_common_waist_chain_45)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition JewelleryAndDevotionalWorkMakeKnottedBrassWaistChain() => new()
	{
		Metadata = new("Jewellery and devotional work/make knotted brass waist chain", "crafts", ["earlymodern", "renaissance"], ["item:renaissance_jewellery_common_waist_chain_75"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make knotted brass waist chain",
		Category = "Jewellery and devotional work",
		Blurb = "make a knotted brass waist chain",
		Action = "making jewellery or a devotional item",
		ActiveCraftItemSdesc = "an in-progress knotted brass waist chain craft",
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
			new CatalogueCommodityRecipeInput(Grams: 61.0, Material: "brass", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("61 grams of brass", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Pliers", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Pliers tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_jewellery_common_waist_chain_75", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a knotted brass waist chain (@renaissance_jewellery_common_waist_chain_75)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition JewelleryAndDevotionalWorkMakeBirdQuartzWreath() => new()
	{
		Metadata = new("Jewellery and devotional work/make bird quartz wreath", "crafts", ["earlymodern", "renaissance"], ["item:renaissance_jewellery_common_wreath_110"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make bird quartz wreath",
		Category = "Jewellery and devotional work",
		Blurb = "make a bird quartz wreath",
		Action = "making jewellery or a devotional item",
		ActiveCraftItemSdesc = "an in-progress bird quartz wreath craft",
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
			new CatalogueCommodityRecipeInput(Grams: 72.0, Material: "quartz", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("72 grams of quartz", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Pliers", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Pliers tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_jewellery_common_wreath_110", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a bird quartz wreath (@renaissance_jewellery_common_wreath_110)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition JewelleryAndDevotionalWorkMakeFlowerHeadBoneWreath() => new()
	{
		Metadata = new("Jewellery and devotional work/make flower-head bone wreath", "crafts", ["earlymodern", "renaissance"], ["item:renaissance_jewellery_common_wreath_140"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make flower-head bone wreath",
		Category = "Jewellery and devotional work",
		Blurb = "make a flower-head bone wreath",
		Action = "making jewellery or a devotional item",
		ActiveCraftItemSdesc = "an in-progress flower-head bone wreath craft",
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
			new CatalogueCommodityRecipeInput(Grams: 55.0, Material: "bone", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("55 grams of bone", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Pliers", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Pliers tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_jewellery_common_wreath_140", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a flower-head bone wreath (@renaissance_jewellery_common_wreath_140)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition JewelleryAndDevotionalWorkMakeWaterWaveLapisLazuliWreath() => new()
	{
		Metadata = new("Jewellery and devotional work/make water-wave lapis lazuli wreath", "crafts", ["earlymodern", "renaissance"], ["item:renaissance_jewellery_common_wreath_170"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make water-wave lapis lazuli wreath",
		Category = "Jewellery and devotional work",
		Blurb = "make a water-wave lapis lazuli wreath",
		Action = "making jewellery or a devotional item",
		ActiveCraftItemSdesc = "an in-progress water-wave lapis lazuli wreath craft",
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
			new CatalogueCommodityRecipeInput(Grams: 38.0, Material: "lapis lazuli", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("38 grams of lapis lazuli", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Pliers", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Pliers tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_jewellery_common_wreath_170", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a water-wave lapis lazuli wreath (@renaissance_jewellery_common_wreath_170)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition JewelleryAndDevotionalWorkMakeRosetteGarnetWreath() => new()
	{
		Metadata = new("Jewellery and devotional work/make rosette garnet wreath", "crafts", ["earlymodern", "renaissance"], ["item:renaissance_jewellery_common_wreath_20"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make rosette garnet wreath",
		Category = "Jewellery and devotional work",
		Blurb = "make a rosette garnet wreath",
		Action = "making jewellery or a devotional item",
		ActiveCraftItemSdesc = "an in-progress rosette garnet wreath craft",
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
			new CatalogueCommodityRecipeInput(Grams: 61.0, Material: "garnet", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("61 grams of garnet", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Pliers", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Pliers tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_jewellery_common_wreath_20", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a rosette garnet wreath (@renaissance_jewellery_common_wreath_20)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition JewelleryAndDevotionalWorkMakeLeafScrollGoldWreath() => new()
	{
		Metadata = new("Jewellery and devotional work/make leaf-scroll gold wreath", "crafts", ["earlymodern", "renaissance"], ["item:renaissance_jewellery_common_wreath_200"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make leaf-scroll gold wreath",
		Category = "Jewellery and devotional work",
		Blurb = "make a leaf-scroll gold wreath",
		Action = "making jewellery or a devotional item",
		ActiveCraftItemSdesc = "an in-progress leaf-scroll gold wreath craft",
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
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_jewellery_common_wreath_200", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a leaf-scroll gold wreath (@renaissance_jewellery_common_wreath_200)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition JewelleryAndDevotionalWorkMakeShellCopperWreath() => new()
	{
		Metadata = new("Jewellery and devotional work/make shell copper wreath", "crafts", ["earlymodern", "renaissance"], ["item:renaissance_jewellery_common_wreath_50"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make shell copper wreath",
		Category = "Jewellery and devotional work",
		Blurb = "make a shell copper wreath",
		Action = "making jewellery or a devotional item",
		ActiveCraftItemSdesc = "an in-progress shell copper wreath craft",
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
			new CatalogueCommodityRecipeInput(Grams: 44.0, Material: "copper", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("44 grams of copper", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Pliers", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Pliers tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_jewellery_common_wreath_50", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a shell copper wreath (@renaissance_jewellery_common_wreath_50)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition JewelleryAndDevotionalWorkMakeGeometricPearlWreath() => new()
	{
		Metadata = new("Jewellery and devotional work/make geometric pearl wreath", "crafts", ["earlymodern", "renaissance"], ["item:renaissance_jewellery_common_wreath_80"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make geometric pearl wreath",
		Category = "Jewellery and devotional work",
		Blurb = "make a geometric pearl wreath",
		Action = "making jewellery or a devotional item",
		ActiveCraftItemSdesc = "an in-progress geometric pearl wreath craft",
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
			new CatalogueCommodityRecipeInput(Grams: 27.0, Material: "pearl", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("27 grams of pearl", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Pliers", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Pliers tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_jewellery_common_wreath_80", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a geometric pearl wreath (@renaissance_jewellery_common_wreath_80)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition JewelleryAndDevotionalWorkMakeBraidedSilverAnklet() => new()
	{
		Metadata = new("Jewellery and devotional work/make braided silver anklet", "crafts", ["earlymodern", "renaissance"], ["item:renaissance_jewellery_eal_anklet_278"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make braided silver anklet",
		Category = "Jewellery and devotional work",
		Blurb = "make a braided silver anklet",
		Action = "making jewellery or a devotional item",
		ActiveCraftItemSdesc = "an in-progress braided silver anklet craft",
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
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_jewellery_eal_anklet_278", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a braided silver anklet (@renaissance_jewellery_eal_anklet_278)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition JewelleryAndDevotionalWorkMakeCheckerBronzeArmlet() => new()
	{
		Metadata = new("Jewellery and devotional work/make checker bronze armlet", "crafts", ["earlymodern", "renaissance"], ["item:renaissance_jewellery_eal_armlet_277"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make checker bronze armlet",
		Category = "Jewellery and devotional work",
		Blurb = "make a checker bronze armlet",
		Action = "making jewellery or a devotional item",
		ActiveCraftItemSdesc = "an in-progress checker bronze armlet craft",
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
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_jewellery_eal_armlet_277", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a checker bronze armlet (@renaissance_jewellery_eal_armlet_277)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition JewelleryAndDevotionalWorkMakeVineCoralBadge() => new()
	{
		Metadata = new("Jewellery and devotional work/make vine coral badge", "crafts", ["earlymodern", "renaissance"], ["item:renaissance_jewellery_eal_badge_287"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make vine coral badge",
		Category = "Jewellery and devotional work",
		Blurb = "make a vine coral badge",
		Action = "making jewellery or a devotional item",
		ActiveCraftItemSdesc = "an in-progress vine coral badge craft",
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
			new CatalogueCommodityRecipeInput(Grams: 16.0, Material: "coral", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("16 grams of coral", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Pliers", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Pliers tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_jewellery_eal_badge_287", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a vine coral badge (@renaissance_jewellery_eal_badge_287)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition JewelleryAndDevotionalWorkMakeShellFaienceNecklace() => new()
	{
		Metadata = new("Jewellery and devotional work/make shell faience necklace", "crafts", ["earlymodern", "renaissance"], ["item:renaissance_jewellery_eal_bead_necklace_274"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make shell faience necklace",
		Category = "Jewellery and devotional work",
		Blurb = "make a shell faience necklace",
		Action = "making jewellery or a devotional item",
		ActiveCraftItemSdesc = "an in-progress shell faience necklace craft",
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
			new CatalogueCommodityRecipeInput(Grams: 66.0, Material: "faience", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("66 grams of faience", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Pliers", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Pliers tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_jewellery_eal_bead_necklace_274", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a shell faience necklace (@renaissance_jewellery_eal_bead_necklace_274)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition JewelleryAndDevotionalWorkMakeBirdAmberBeltOrnament() => new()
	{
		Metadata = new("Jewellery and devotional work/make bird amber belt ornament", "crafts", ["earlymodern", "renaissance"], ["item:renaissance_jewellery_eal_belt_ornament_286"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make bird amber belt ornament",
		Category = "Jewellery and devotional work",
		Blurb = "make a bird amber belt ornament",
		Action = "making jewellery or a devotional item",
		ActiveCraftItemSdesc = "an in-progress bird amber belt ornament craft",
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
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_jewellery_eal_belt_ornament_286", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a bird amber belt ornament (@renaissance_jewellery_eal_belt_ornament_286)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition JewelleryAndDevotionalWorkMakeRosetteCopperBracelet() => new()
	{
		Metadata = new("Jewellery and devotional work/make rosette copper bracelet", "crafts", ["earlymodern", "renaissance"], ["item:renaissance_jewellery_eal_bracelet_276"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make rosette copper bracelet",
		Category = "Jewellery and devotional work",
		Blurb = "make a rosette copper bracelet",
		Action = "making jewellery or a devotional item",
		ActiveCraftItemSdesc = "an in-progress rosette copper bracelet craft",
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
			new CatalogueCommodityRecipeInput(Grams: 16.0, Material: "copper", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("16 grams of copper", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Pliers", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Pliers tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_jewellery_eal_bracelet_276", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a rosette copper bracelet (@renaissance_jewellery_eal_bracelet_276)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition JewelleryAndDevotionalWorkMakeLeafScrollBrassBrooch() => new()
	{
		Metadata = new("Jewellery and devotional work/make leaf-scroll brass brooch", "crafts", ["earlymodern", "renaissance"], ["item:renaissance_jewellery_eal_brooch_280"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make leaf-scroll brass brooch",
		Category = "Jewellery and devotional work",
		Blurb = "make a leaf-scroll brass brooch",
		Action = "making jewellery or a devotional item",
		ActiveCraftItemSdesc = "an in-progress leaf-scroll brass brooch craft",
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
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_jewellery_eal_brooch_280", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a leaf-scroll brass brooch (@renaissance_jewellery_eal_brooch_280)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition JewelleryAndDevotionalWorkMakeReedPewterNecklace() => new()
	{
		Metadata = new("Jewellery and devotional work/make reed pewter necklace", "crafts", ["earlymodern", "renaissance"], ["item:renaissance_jewellery_eal_chain_necklace_275"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make reed pewter necklace",
		Category = "Jewellery and devotional work",
		Blurb = "make a reed pewter necklace",
		Action = "making jewellery or a devotional item",
		ActiveCraftItemSdesc = "an in-progress reed pewter necklace craft",
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
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_jewellery_eal_chain_necklace_275", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a reed pewter necklace (@renaissance_jewellery_eal_chain_necklace_275)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition JewelleryAndDevotionalWorkMakeKnottedShellCirclet() => new()
	{
		Metadata = new("Jewellery and devotional work/make knotted shell circlet", "crafts", ["earlymodern", "renaissance"], ["item:renaissance_jewellery_eal_circlet_283"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make knotted shell circlet",
		Category = "Jewellery and devotional work",
		Blurb = "make a knotted shell circlet",
		Action = "making jewellery or a devotional item",
		ActiveCraftItemSdesc = "an in-progress knotted shell circlet craft",
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
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_jewellery_eal_circlet_283", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a knotted shell circlet (@renaissance_jewellery_eal_circlet_283)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition JewelleryAndDevotionalWorkMakeCheckerJadeDevotionalBadge() => new()
	{
		Metadata = new("Jewellery and devotional work/make checker jade devotional badge", "crafts", ["earlymodern", "renaissance"], ["item:renaissance_jewellery_eal_devotional_badge_293"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make checker jade devotional badge",
		Category = "Jewellery and devotional work",
		Blurb = "make a checker jade devotional badge",
		Action = "making jewellery or a devotional item",
		ActiveCraftItemSdesc = "an in-progress checker jade devotional badge craft",
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
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_jewellery_eal_devotional_badge_293", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a checker jade devotional badge (@renaissance_jewellery_eal_devotional_badge_293)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition JewelleryAndDevotionalWorkMakeRosetteTurquoiseDevotionalMedal() => new()
	{
		Metadata = new("Jewellery and devotional work/make rosette turquoise devotional medal", "crafts", ["earlymodern", "renaissance"], ["item:renaissance_jewellery_eal_devotional_medal_292"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make rosette turquoise devotional medal",
		Category = "Jewellery and devotional work",
		Blurb = "make a rosette turquoise devotional medal",
		Action = "making jewellery or a devotional item",
		ActiveCraftItemSdesc = "an in-progress rosette turquoise devotional medal craft",
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
			new CatalogueCommodityRecipeInput(Grams: 44.0, Material: "turquoise", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("44 grams of turquoise", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Pliers", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Pliers tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_jewellery_eal_devotional_medal_292", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a rosette turquoise devotional medal (@renaissance_jewellery_eal_devotional_medal_292)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition JewelleryAndDevotionalWorkMakeFlowerHeadIvoryDiadem() => new()
	{
		Metadata = new("Jewellery and devotional work/make flower-head ivory diadem", "crafts", ["earlymodern", "renaissance"], ["item:renaissance_jewellery_eal_diadem_284"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make flower-head ivory diadem",
		Category = "Jewellery and devotional work",
		Blurb = "make a flower-head ivory diadem",
		Action = "making jewellery or a devotional item",
		ActiveCraftItemSdesc = "an in-progress flower-head ivory diadem craft",
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
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_jewellery_eal_diadem_284", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a flower-head ivory diadem (@renaissance_jewellery_eal_diadem_284)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition JewelleryAndDevotionalWorkMakeRadiatingGoldEarrings() => new()
	{
		Metadata = new("Jewellery and devotional work/make radiating gold earrings", "crafts", ["earlymodern", "renaissance"], ["item:renaissance_jewellery_eal_earrings_279"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make radiating gold earrings",
		Category = "Jewellery and devotional work",
		Blurb = "make a radiating gold earrings",
		Action = "making jewellery or a devotional item",
		ActiveCraftItemSdesc = "an in-progress radiating gold earrings craft",
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
			new CatalogueCommodityRecipeInput(Grams: 33.0, Material: "gold", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("33 grams of gold", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Pliers", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Pliers tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_jewellery_eal_earrings_279", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a radiating gold earrings (@renaissance_jewellery_eal_earrings_279)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition JewelleryAndDevotionalWorkMakeWaterWaveBoneHairpin() => new()
	{
		Metadata = new("Jewellery and devotional work/make water-wave bone hairpin", "crafts", ["earlymodern", "renaissance"], ["item:renaissance_jewellery_eal_hairpin_282"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make water-wave bone hairpin",
		Category = "Jewellery and devotional work",
		Blurb = "make a water-wave bone hairpin",
		Action = "making jewellery or a devotional item",
		ActiveCraftItemSdesc = "an in-progress water-wave bone hairpin craft",
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
			new CatalogueCommodityRecipeInput(Grams: 50.0, Material: "bone", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("50 grams of bone", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Pliers", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Pliers tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_jewellery_eal_hairpin_282", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a water-wave bone hairpin (@renaissance_jewellery_eal_hairpin_282)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition JewelleryAndDevotionalWorkMakeWaterWaveBrassIncenseHolderPattern2() => new()
	{
		Metadata = new("Jewellery and devotional work/make water-wave brass incense holder pattern 2", "crafts", ["earlymodern", "renaissance"], ["item:renaissance_jewellery_eal_incense_holder_298"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make water-wave brass incense holder pattern 2",
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
			new CatalogueCommodityRecipeInput(Grams: 16.0, Material: "brass", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("16 grams of brass", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Pliers", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Pliers tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_jewellery_eal_incense_holder_298", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a water-wave brass incense holder (@renaissance_jewellery_eal_incense_holder_298)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition JewelleryAndDevotionalWorkMakeSunburstBronzeOfferingBowlPattern2() => new()
	{
		Metadata = new("Jewellery and devotional work/make sunburst bronze offering bowl pattern 2", "crafts", ["earlymodern", "renaissance"], ["item:renaissance_jewellery_eal_offering_bowl_297"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make sunburst bronze offering bowl pattern 2",
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
			new CatalogueCommodityRecipeInput(Grams: 72.0, Material: "bronze", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("72 grams of bronze", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Pliers", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Pliers tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_jewellery_eal_offering_bowl_297", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a sunburst bronze offering bowl (@renaissance_jewellery_eal_offering_bowl_297)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition JewelleryAndDevotionalWorkMakeCloudQuartzPendant() => new()
	{
		Metadata = new("Jewellery and devotional work/make cloud quartz pendant", "crafts", ["earlymodern", "renaissance"], ["item:renaissance_jewellery_eal_pendant_273"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make cloud quartz pendant",
		Category = "Jewellery and devotional work",
		Blurb = "make a cloud quartz pendant",
		Action = "making jewellery or a devotional item",
		ActiveCraftItemSdesc = "an in-progress cloud quartz pendant craft",
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
			new CatalogueCommodityRecipeInput(Grams: 61.0, Material: "quartz", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("61 grams of quartz", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Pliers", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Pliers tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_jewellery_eal_pendant_273", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a cloud quartz pendant (@renaissance_jewellery_eal_pendant_273)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition JewelleryAndDevotionalWorkMakeSunburstGlassPin() => new()
	{
		Metadata = new("Jewellery and devotional work/make sunburst glass pin", "crafts", ["earlymodern", "renaissance"], ["item:renaissance_jewellery_eal_pin_281"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make sunburst glass pin",
		Category = "Jewellery and devotional work",
		Blurb = "make a sunburst glass pin",
		Action = "making jewellery or a devotional item",
		ActiveCraftItemSdesc = "an in-progress sunburst glass pin craft",
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
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_jewellery_eal_pin_281", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a sunburst glass pin (@renaissance_jewellery_eal_pin_281)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition JewelleryAndDevotionalWorkMakeKnottedWoodSmallDevotionalPanelPattern2() => new()
	{
		Metadata = new("Jewellery and devotional work/make knotted wood small devotional panel pattern 2", "crafts", ["earlymodern", "renaissance"], ["item:renaissance_jewellery_eal_portable_panel_299"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make knotted wood small devotional panel pattern 2",
		Category = "Jewellery and devotional work",
		Blurb = "make a knotted wood small devotional panel",
		Action = "making jewellery or a devotional item",
		ActiveCraftItemSdesc = "an in-progress knotted wood small devotional panel craft",
		Gate = CatalogueRecipeGates.RenaissanceAndEarlyModernJewelleryCarpentry15,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
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
			new CatalogueCommodityRecipeInput(Grams: 22.0, Material: "wood", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("22 grams of wood", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Pliers", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Pliers tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_jewellery_eal_portable_panel_299", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a knotted wood small devotional panel (@renaissance_jewellery_eal_portable_panel_299)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition JewelleryAndDevotionalWorkMakeReedLapisLazuliPrayerBeadStrand() => new()
	{
		Metadata = new("Jewellery and devotional work/make reed lapis lazuli prayer bead strand", "crafts", ["earlymodern", "renaissance"], ["item:renaissance_jewellery_eal_prayer_beads_291"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make reed lapis lazuli prayer bead strand",
		Category = "Jewellery and devotional work",
		Blurb = "make a reed lapis lazuli prayer bead strand",
		Action = "making jewellery or a devotional item",
		ActiveCraftItemSdesc = "an in-progress reed lapis lazuli prayer bead strand craft",
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
			new CatalogueCommodityRecipeInput(Grams: 38.0, Material: "lapis lazuli", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("38 grams of lapis lazuli", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Pliers", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Pliers tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_jewellery_eal_prayer_beads_291", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a reed lapis lazuli prayer bead strand (@renaissance_jewellery_eal_prayer_beads_291)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition JewelleryAndDevotionalWorkMakeBraidedLinenPrayerCordPattern2() => new()
	{
		Metadata = new("Jewellery and devotional work/make braided linen prayer cord pattern 2", "crafts", ["earlymodern", "renaissance"], ["item:renaissance_jewellery_eal_prayer_cord_294"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make braided linen prayer cord pattern 2",
		Category = "Jewellery and devotional work",
		Blurb = "make a braided linen prayer cord",
		Action = "making jewellery or a devotional item",
		ActiveCraftItemSdesc = "an in-progress braided linen prayer cord craft",
		Gate = CatalogueRecipeGates.RenaissanceAndEarlyModernJewelleryWeaving15,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
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
			new CatalogueCommodityRecipeInput(Grams: 55.0, Material: "linen", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("55 grams of linen", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Pliers", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Pliers tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_jewellery_eal_prayer_cord_294", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a braided linen prayer cord (@renaissance_jewellery_eal_prayer_cord_294)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition JewelleryAndDevotionalWorkMakeFlowerHeadPewterProcessionalOrnamentPattern2() => new()
	{
		Metadata = new("Jewellery and devotional work/make flower-head pewter processional ornament pattern 2", "crafts", ["earlymodern", "renaissance"], ["item:renaissance_jewellery_eal_processional_ornament_300"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make flower-head pewter processional ornament pattern 2",
		Category = "Jewellery and devotional work",
		Blurb = "make a flower-head pewter processional ornament",
		Action = "making jewellery or a devotional item",
		ActiveCraftItemSdesc = "an in-progress flower-head pewter processional ornament craft",
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
			new CatalogueCommodityRecipeInput(Grams: 27.0, Material: "pewter", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("27 grams of pewter", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Pliers", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Pliers tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_jewellery_eal_processional_ornament_300", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a flower-head pewter processional ornament (@renaissance_jewellery_eal_processional_ornament_300)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition JewelleryAndDevotionalWorkMakeVineTurquoiseRing() => new()
	{
		Metadata = new("Jewellery and devotional work/make vine turquoise ring", "crafts", ["earlymodern", "renaissance"], ["item:renaissance_jewellery_eal_ring_271"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make vine turquoise ring",
		Category = "Jewellery and devotional work",
		Blurb = "make a vine turquoise ring",
		Action = "making jewellery or a devotional item",
		ActiveCraftItemSdesc = "an in-progress vine turquoise ring craft",
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
			new CatalogueCommodityRecipeInput(Grams: 50.0, Material: "turquoise", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("50 grams of turquoise", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Pliers", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Pliers tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_jewellery_eal_ring_271", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a vine turquoise ring (@renaissance_jewellery_eal_ring_271)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition JewelleryAndDevotionalWorkMakeLeafScrollPewterSmallScriptureBox() => new()
	{
		Metadata = new("Jewellery and devotional work/make leaf-scroll pewter small scripture box", "crafts", ["earlymodern", "renaissance"], ["item:renaissance_jewellery_eal_scripture_box_296"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make leaf-scroll pewter small scripture box",
		Category = "Jewellery and devotional work",
		Blurb = "make a leaf-scroll pewter small scripture box",
		Action = "making jewellery or a devotional item",
		ActiveCraftItemSdesc = "an in-progress leaf-scroll pewter small scripture box craft",
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
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_jewellery_eal_scripture_box_296", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a leaf-scroll pewter small scripture box (@renaissance_jewellery_eal_scripture_box_296)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition JewelleryAndDevotionalWorkMakeGeometricJadeRing() => new()
	{
		Metadata = new("Jewellery and devotional work/make geometric jade ring", "crafts", ["earlymodern", "renaissance"], ["item:renaissance_jewellery_eal_signet_ring_272"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make geometric jade ring",
		Category = "Jewellery and devotional work",
		Blurb = "make a geometric jade ring",
		Action = "making jewellery or a devotional item",
		ActiveCraftItemSdesc = "an in-progress geometric jade ring craft",
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
			new CatalogueCommodityRecipeInput(Grams: 55.0, Material: "jade", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("55 grams of jade", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Pliers", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Pliers tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_jewellery_eal_signet_ring_272", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a geometric jade ring (@renaissance_jewellery_eal_signet_ring_272)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition JewelleryAndDevotionalWorkMakeGeometricGarnetTempleRing() => new()
	{
		Metadata = new("Jewellery and devotional work/make geometric garnet temple ring", "crafts", ["earlymodern", "renaissance"], ["item:renaissance_jewellery_eal_temple_ring_288"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make geometric garnet temple ring",
		Category = "Jewellery and devotional work",
		Blurb = "make a geometric garnet temple ring",
		Action = "making jewellery or a devotional item",
		ActiveCraftItemSdesc = "an in-progress geometric garnet temple ring craft",
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
			new CatalogueCommodityRecipeInput(Grams: 22.0, Material: "garnet", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("22 grams of garnet", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Pliers", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Pliers tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_jewellery_eal_temple_ring_288", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a geometric garnet temple ring (@renaissance_jewellery_eal_temple_ring_288)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition JewelleryAndDevotionalWorkMakeCloudAgateTorc() => new()
	{
		Metadata = new("Jewellery and devotional work/make cloud agate torc", "crafts", ["earlymodern", "renaissance"], ["item:renaissance_jewellery_eal_torc_289"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make cloud agate torc",
		Category = "Jewellery and devotional work",
		Blurb = "make a cloud agate torc",
		Action = "making jewellery or a devotional item",
		ActiveCraftItemSdesc = "an in-progress cloud agate torc craft",
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
			new CatalogueCommodityRecipeInput(Grams: 27.0, Material: "agate", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("27 grams of agate", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Pliers", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Pliers tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_jewellery_eal_torc_289", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a cloud agate torc (@renaissance_jewellery_eal_torc_289)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition JewelleryAndDevotionalWorkMakeRadiatingFaienceVotiveToken() => new()
	{
		Metadata = new("Jewellery and devotional work/make radiating faience votive token", "crafts", ["earlymodern", "renaissance"], ["item:renaissance_jewellery_eal_votive_token_295"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make radiating faience votive token",
		Category = "Jewellery and devotional work",
		Blurb = "make a radiating faience votive token",
		Action = "making jewellery or a devotional item",
		ActiveCraftItemSdesc = "an in-progress radiating faience votive token craft",
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
			new CatalogueCommodityRecipeInput(Grams: 61.0, Material: "faience", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("61 grams of faience", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Pliers", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Pliers tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_jewellery_eal_votive_token_295", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a radiating faience votive token (@renaissance_jewellery_eal_votive_token_295)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition JewelleryAndDevotionalWorkMakeStarPearlWaistChain() => new()
	{
		Metadata = new("Jewellery and devotional work/make star pearl waist chain", "crafts", ["earlymodern", "renaissance"], ["item:renaissance_jewellery_eal_waist_chain_285"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make star pearl waist chain",
		Category = "Jewellery and devotional work",
		Blurb = "make a star pearl waist chain",
		Action = "making jewellery or a devotional item",
		ActiveCraftItemSdesc = "an in-progress star pearl waist chain craft",
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
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_jewellery_eal_waist_chain_285", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a star pearl waist chain (@renaissance_jewellery_eal_waist_chain_285)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition JewelleryAndDevotionalWorkMakeShellCarnelianWreath() => new()
	{
		Metadata = new("Jewellery and devotional work/make shell carnelian wreath", "crafts", ["earlymodern", "renaissance"], ["item:renaissance_jewellery_eal_wreath_290"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make shell carnelian wreath",
		Category = "Jewellery and devotional work",
		Blurb = "make a shell carnelian wreath",
		Action = "making jewellery or a devotional item",
		ActiveCraftItemSdesc = "an in-progress shell carnelian wreath craft",
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
			new CatalogueCommodityRecipeInput(Grams: 33.0, Material: "carnelian", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("33 grams of carnelian", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Pliers", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Pliers tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_jewellery_eal_wreath_290", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a shell carnelian wreath (@renaissance_jewellery_eal_wreath_290)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition JewelleryAndDevotionalWorkMakeGeometricCarnelianAnklet() => new()
	{
		Metadata = new("Jewellery and devotional work/make geometric carnelian anklet", "crafts", ["earlymodern", "renaissance"], ["item:renaissance_jewellery_eon_anklet_158"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make geometric carnelian anklet",
		Category = "Jewellery and devotional work",
		Blurb = "make a geometric carnelian anklet",
		Action = "making jewellery or a devotional item",
		ActiveCraftItemSdesc = "an in-progress geometric carnelian anklet craft",
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
			new CatalogueCommodityRecipeInput(Grams: 33.0, Material: "carnelian", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("33 grams of carnelian", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Pliers", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Pliers tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_jewellery_eon_anklet_158", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a geometric carnelian anklet (@renaissance_jewellery_eon_anklet_158)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition JewelleryAndDevotionalWorkMakeVineAgateArmlet() => new()
	{
		Metadata = new("Jewellery and devotional work/make vine agate armlet", "crafts", ["earlymodern", "renaissance"], ["item:renaissance_jewellery_eon_armlet_157"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make vine agate armlet",
		Category = "Jewellery and devotional work",
		Blurb = "make a vine agate armlet",
		Action = "making jewellery or a devotional item",
		ActiveCraftItemSdesc = "an in-progress vine agate armlet craft",
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
			new CatalogueCommodityRecipeInput(Grams: 27.0, Material: "agate", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("27 grams of agate", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Pliers", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Pliers tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_jewellery_eon_armlet_157", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a vine agate armlet (@renaissance_jewellery_eon_armlet_157)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition JewelleryAndDevotionalWorkMakeSunburstSilverBadge() => new()
	{
		Metadata = new("Jewellery and devotional work/make sunburst silver badge", "crafts", ["earlymodern", "renaissance"], ["item:renaissance_jewellery_eon_badge_167"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make sunburst silver badge",
		Category = "Jewellery and devotional work",
		Blurb = "make a sunburst silver badge",
		Action = "making jewellery or a devotional item",
		ActiveCraftItemSdesc = "an in-progress sunburst silver badge craft",
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
			new CatalogueCommodityRecipeInput(Grams: 22.0, Material: "silver", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("22 grams of silver", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Pliers", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Pliers tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_jewellery_eon_badge_167", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a sunburst silver badge (@renaissance_jewellery_eon_badge_167)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition JewelleryAndDevotionalWorkMakeFlowerHeadAmberNecklace() => new()
	{
		Metadata = new("Jewellery and devotional work/make flower-head amber necklace", "crafts", ["earlymodern", "renaissance"], ["item:renaissance_jewellery_eon_bead_necklace_154"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make flower-head amber necklace",
		Category = "Jewellery and devotional work",
		Blurb = "make a flower-head amber necklace",
		Action = "making jewellery or a devotional item",
		ActiveCraftItemSdesc = "an in-progress flower-head amber necklace craft",
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
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_jewellery_eon_bead_necklace_154", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a flower-head amber necklace (@renaissance_jewellery_eon_bead_necklace_154)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition JewelleryAndDevotionalWorkMakeLeafScrollBronzeBeltOrnament() => new()
	{
		Metadata = new("Jewellery and devotional work/make leaf-scroll bronze belt ornament", "crafts", ["earlymodern", "renaissance"], ["item:renaissance_jewellery_eon_belt_ornament_166"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make leaf-scroll bronze belt ornament",
		Category = "Jewellery and devotional work",
		Blurb = "make a leaf-scroll bronze belt ornament",
		Action = "making jewellery or a devotional item",
		ActiveCraftItemSdesc = "an in-progress leaf-scroll bronze belt ornament craft",
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
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_jewellery_eon_belt_ornament_166", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a leaf-scroll bronze belt ornament (@renaissance_jewellery_eon_belt_ornament_166)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition JewelleryAndDevotionalWorkMakeBirdGarnetBracelet() => new()
	{
		Metadata = new("Jewellery and devotional work/make bird garnet bracelet", "crafts", ["earlymodern", "renaissance"], ["item:renaissance_jewellery_eon_bracelet_156"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make bird garnet bracelet",
		Category = "Jewellery and devotional work",
		Blurb = "make a bird garnet bracelet",
		Action = "making jewellery or a devotional item",
		ActiveCraftItemSdesc = "an in-progress bird garnet bracelet craft",
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
			new CatalogueCommodityRecipeInput(Grams: 22.0, Material: "garnet", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("22 grams of garnet", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Pliers", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Pliers tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_jewellery_eon_bracelet_156", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a bird garnet bracelet (@renaissance_jewellery_eon_bracelet_156)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition JewelleryAndDevotionalWorkMakeShellTurquoiseBrooch() => new()
	{
		Metadata = new("Jewellery and devotional work/make shell turquoise brooch", "crafts", ["earlymodern", "renaissance"], ["item:renaissance_jewellery_eon_brooch_160"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make shell turquoise brooch",
		Category = "Jewellery and devotional work",
		Blurb = "make a shell turquoise brooch",
		Action = "making jewellery or a devotional item",
		ActiveCraftItemSdesc = "an in-progress shell turquoise brooch craft",
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
			new CatalogueCommodityRecipeInput(Grams: 44.0, Material: "turquoise", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("44 grams of turquoise", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Pliers", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Pliers tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_jewellery_eon_brooch_160", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a shell turquoise brooch (@renaissance_jewellery_eon_brooch_160)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition JewelleryAndDevotionalWorkMakeStarCoralNecklace() => new()
	{
		Metadata = new("Jewellery and devotional work/make star coral necklace", "crafts", ["earlymodern", "renaissance"], ["item:renaissance_jewellery_eon_chain_necklace_155"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make star coral necklace",
		Category = "Jewellery and devotional work",
		Blurb = "make a star coral necklace",
		Action = "making jewellery or a devotional item",
		ActiveCraftItemSdesc = "an in-progress star coral necklace craft",
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
			new CatalogueCommodityRecipeInput(Grams: 16.0, Material: "coral", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("16 grams of coral", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Pliers", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Pliers tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_jewellery_eon_chain_necklace_155", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a star coral necklace (@renaissance_jewellery_eon_chain_necklace_155)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition JewelleryAndDevotionalWorkMakeCheckerFaienceCirclet() => new()
	{
		Metadata = new("Jewellery and devotional work/make checker faience circlet", "crafts", ["earlymodern", "renaissance"], ["item:renaissance_jewellery_eon_circlet_163"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make checker faience circlet",
		Category = "Jewellery and devotional work",
		Blurb = "make a checker faience circlet",
		Action = "making jewellery or a devotional item",
		ActiveCraftItemSdesc = "an in-progress checker faience circlet craft",
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
			new CatalogueCommodityRecipeInput(Grams: 61.0, Material: "faience", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("61 grams of faience", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Pliers", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Pliers tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_jewellery_eon_circlet_163", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a checker faience circlet (@renaissance_jewellery_eon_circlet_163)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition JewelleryAndDevotionalWorkMakeVineIvoryDevotionalBadge() => new()
	{
		Metadata = new("Jewellery and devotional work/make vine ivory devotional badge", "crafts", ["earlymodern", "renaissance"], ["item:renaissance_jewellery_eon_devotional_badge_173"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make vine ivory devotional badge",
		Category = "Jewellery and devotional work",
		Blurb = "make a vine ivory devotional badge",
		Action = "making jewellery or a devotional item",
		ActiveCraftItemSdesc = "an in-progress vine ivory devotional badge craft",
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
			new CatalogueCommodityRecipeInput(Grams: 55.0, Material: "ivory", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("55 grams of ivory", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Pliers", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Pliers tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_jewellery_eon_devotional_badge_173", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a vine ivory devotional badge (@renaissance_jewellery_eon_devotional_badge_173)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};
}
