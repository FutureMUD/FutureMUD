#nullable enable

using MudSharp.Body;
using MudSharp.GameItems.Inventory.Plans;
using MudSharp.RPG.Checks;

namespace DatabaseSeeder.Seeders;

public static partial class Recipes11Stock
{
	public static CatalogueRecipeDefinition JewelleryAndDevotionalWorkMakeReedBronzeDiadem() => new()
	{
		Metadata = new("Jewellery and devotional work/make reed bronze diadem", "crafts", ["earlymodern", "renaissance"], ["item:earlymodern_jewellery_it_diadem_114"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make reed bronze diadem",
		Category = "Jewellery and devotional work",
		Blurb = "make a reed bronze diadem",
		Action = "making jewellery or a devotional item",
		ActiveCraftItemSdesc = "an in-progress reed bronze diadem craft",
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
			new CatalogueItemRecipeProduct(ItemStableReference: "earlymodern_jewellery_it_diadem_114", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a reed bronze diadem (@earlymodern_jewellery_it_diadem_114)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition JewelleryAndDevotionalWorkMakeBirdJadeEarrings() => new()
	{
		Metadata = new("Jewellery and devotional work/make bird jade earrings", "crafts", ["earlymodern", "renaissance"], ["item:earlymodern_jewellery_it_earrings_109"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make bird jade earrings",
		Category = "Jewellery and devotional work",
		Blurb = "make a bird jade earrings",
		Action = "making jewellery or a devotional item",
		ActiveCraftItemSdesc = "an in-progress bird jade earrings craft",
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
			new CatalogueCommodityRecipeInput(Grams: 66.0, Material: "jade", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("66 grams of jade", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Pliers", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Pliers tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "earlymodern_jewellery_it_earrings_109", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a bird jade earrings (@earlymodern_jewellery_it_earrings_109)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition JewelleryAndDevotionalWorkMakeCloudPewterHairpin() => new()
	{
		Metadata = new("Jewellery and devotional work/make cloud pewter hairpin", "crafts", ["earlymodern", "renaissance"], ["item:earlymodern_jewellery_it_hairpin_112"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make cloud pewter hairpin",
		Category = "Jewellery and devotional work",
		Blurb = "make a cloud pewter hairpin",
		Action = "making jewellery or a devotional item",
		ActiveCraftItemSdesc = "an in-progress cloud pewter hairpin craft",
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
			new CatalogueItemRecipeProduct(ItemStableReference: "earlymodern_jewellery_it_hairpin_112", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a cloud pewter hairpin (@earlymodern_jewellery_it_hairpin_112)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition JewelleryAndDevotionalWorkMakeLeafScrollCoralPendant() => new()
	{
		Metadata = new("Jewellery and devotional work/make leaf-scroll coral pendant", "crafts", ["earlymodern", "renaissance"], ["item:earlymodern_jewellery_it_pendant_103"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make leaf-scroll coral pendant",
		Category = "Jewellery and devotional work",
		Blurb = "make a leaf-scroll coral pendant",
		Action = "making jewellery or a devotional item",
		ActiveCraftItemSdesc = "an in-progress leaf-scroll coral pendant craft",
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
			new CatalogueItemRecipeProduct(ItemStableReference: "earlymodern_jewellery_it_pendant_103", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a leaf-scroll coral pendant (@earlymodern_jewellery_it_pendant_103)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition JewelleryAndDevotionalWorkMakeGeometricFaiencePin() => new()
	{
		Metadata = new("Jewellery and devotional work/make geometric faience pin", "crafts", ["earlymodern", "renaissance"], ["item:earlymodern_jewellery_it_pin_111"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make geometric faience pin",
		Category = "Jewellery and devotional work",
		Blurb = "make a geometric faience pin",
		Action = "making jewellery or a devotional item",
		ActiveCraftItemSdesc = "an in-progress geometric faience pin craft",
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
			new CatalogueCommodityRecipeInput(Grams: 16.0, Material: "faience", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("16 grams of faience", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Pliers", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Pliers tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "earlymodern_jewellery_it_pin_111", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a geometric faience pin (@earlymodern_jewellery_it_pin_111)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition JewelleryAndDevotionalWorkMakeBraidedPearlRing() => new()
	{
		Metadata = new("Jewellery and devotional work/make braided pearl ring", "crafts", ["earlymodern", "renaissance"], ["item:earlymodern_jewellery_it_ring_101"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make braided pearl ring",
		Category = "Jewellery and devotional work",
		Blurb = "make a braided pearl ring",
		Action = "making jewellery or a devotional item",
		ActiveCraftItemSdesc = "an in-progress braided pearl ring craft",
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
			new CatalogueCommodityRecipeInput(Grams: 22.0, Material: "pearl", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("22 grams of pearl", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Pliers", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Pliers tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "earlymodern_jewellery_it_ring_101", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a braided pearl ring (@earlymodern_jewellery_it_ring_101)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition JewelleryAndDevotionalWorkMakeRadiatingAmberRing() => new()
	{
		Metadata = new("Jewellery and devotional work/make radiating amber ring", "crafts", ["earlymodern", "renaissance"], ["item:earlymodern_jewellery_it_signet_ring_102"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make radiating amber ring",
		Category = "Jewellery and devotional work",
		Blurb = "make a radiating amber ring",
		Action = "making jewellery or a devotional item",
		ActiveCraftItemSdesc = "an in-progress radiating amber ring craft",
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
			new CatalogueItemRecipeProduct(ItemStableReference: "earlymodern_jewellery_it_signet_ring_102", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a radiating amber ring (@earlymodern_jewellery_it_signet_ring_102)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition JewelleryAndDevotionalWorkMakeRadiatingGlassTempleRing() => new()
	{
		Metadata = new("Jewellery and devotional work/make radiating glass temple ring", "crafts", ["earlymodern", "renaissance"], ["item:earlymodern_jewellery_it_temple_ring_118"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make radiating glass temple ring",
		Category = "Jewellery and devotional work",
		Blurb = "make a radiating glass temple ring",
		Action = "making jewellery or a devotional item",
		ActiveCraftItemSdesc = "an in-progress radiating glass temple ring craft",
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
			new CatalogueItemRecipeProduct(ItemStableReference: "earlymodern_jewellery_it_temple_ring_118", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a radiating glass temple ring (@earlymodern_jewellery_it_temple_ring_118)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition JewelleryAndDevotionalWorkMakeLeafScrollBoneTorc() => new()
	{
		Metadata = new("Jewellery and devotional work/make leaf-scroll bone torc", "crafts", ["earlymodern", "renaissance"], ["item:earlymodern_jewellery_it_torc_119"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make leaf-scroll bone torc",
		Category = "Jewellery and devotional work",
		Blurb = "make a leaf-scroll bone torc",
		Action = "making jewellery or a devotional item",
		ActiveCraftItemSdesc = "an in-progress leaf-scroll bone torc craft",
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
			new CatalogueItemRecipeProduct(ItemStableReference: "earlymodern_jewellery_it_torc_119", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a leaf-scroll bone torc (@earlymodern_jewellery_it_torc_119)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition JewelleryAndDevotionalWorkMakeRosetteSilverWaistChain() => new()
	{
		Metadata = new("Jewellery and devotional work/make rosette silver waist chain", "crafts", ["earlymodern", "renaissance"], ["item:earlymodern_jewellery_it_waist_chain_115"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make rosette silver waist chain",
		Category = "Jewellery and devotional work",
		Blurb = "make a rosette silver waist chain",
		Action = "making jewellery or a devotional item",
		ActiveCraftItemSdesc = "an in-progress rosette silver waist chain craft",
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
			new CatalogueItemRecipeProduct(ItemStableReference: "earlymodern_jewellery_it_waist_chain_115", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a rosette silver waist chain (@earlymodern_jewellery_it_waist_chain_115)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition JewelleryAndDevotionalWorkMakeSunburstShellWreath() => new()
	{
		Metadata = new("Jewellery and devotional work/make sunburst shell wreath", "crafts", ["earlymodern", "renaissance"], ["item:earlymodern_jewellery_it_wreath_120"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make sunburst shell wreath",
		Category = "Jewellery and devotional work",
		Blurb = "make a sunburst shell wreath",
		Action = "making jewellery or a devotional item",
		ActiveCraftItemSdesc = "an in-progress sunburst shell wreath craft",
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
			new CatalogueItemRecipeProduct(ItemStableReference: "earlymodern_jewellery_it_wreath_120", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a sunburst shell wreath (@earlymodern_jewellery_it_wreath_120)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition JewelleryAndDevotionalWorkMakeRadiatingSilverAnklet() => new()
	{
		Metadata = new("Jewellery and devotional work/make radiating silver anklet", "crafts", ["earlymodern", "renaissance"], ["item:earlymodern_jewellery_jos_anklet_348"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make radiating silver anklet",
		Category = "Jewellery and devotional work",
		Blurb = "make a radiating silver anklet",
		Action = "making jewellery or a devotional item",
		ActiveCraftItemSdesc = "an in-progress radiating silver anklet craft",
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
			new CatalogueItemRecipeProduct(ItemStableReference: "earlymodern_jewellery_jos_anklet_348", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a radiating silver anklet (@earlymodern_jewellery_jos_anklet_348)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition JewelleryAndDevotionalWorkMakeBraidedBronzeArmlet() => new()
	{
		Metadata = new("Jewellery and devotional work/make braided bronze armlet", "crafts", ["earlymodern", "renaissance"], ["item:earlymodern_jewellery_jos_armlet_347"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make braided bronze armlet",
		Category = "Jewellery and devotional work",
		Blurb = "make a braided bronze armlet",
		Action = "making jewellery or a devotional item",
		ActiveCraftItemSdesc = "an in-progress braided bronze armlet craft",
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
			new CatalogueItemRecipeProduct(ItemStableReference: "earlymodern_jewellery_jos_armlet_347", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a braided bronze armlet (@earlymodern_jewellery_jos_armlet_347)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition JewelleryAndDevotionalWorkMakeGeometricCoralBadge() => new()
	{
		Metadata = new("Jewellery and devotional work/make geometric coral badge", "crafts", ["earlymodern", "renaissance"], ["item:earlymodern_jewellery_jos_badge_357"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make geometric coral badge",
		Category = "Jewellery and devotional work",
		Blurb = "make a geometric coral badge",
		Action = "making jewellery or a devotional item",
		ActiveCraftItemSdesc = "an in-progress geometric coral badge craft",
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
			new CatalogueItemRecipeProduct(ItemStableReference: "earlymodern_jewellery_jos_badge_357", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a geometric coral badge (@earlymodern_jewellery_jos_badge_357)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition JewelleryAndDevotionalWorkMakeReedFaienceNecklace() => new()
	{
		Metadata = new("Jewellery and devotional work/make reed faience necklace", "crafts", ["earlymodern", "renaissance"], ["item:earlymodern_jewellery_jos_bead_necklace_344"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make reed faience necklace",
		Category = "Jewellery and devotional work",
		Blurb = "make a reed faience necklace",
		Action = "making jewellery or a devotional item",
		ActiveCraftItemSdesc = "an in-progress reed faience necklace craft",
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
			new CatalogueItemRecipeProduct(ItemStableReference: "earlymodern_jewellery_jos_bead_necklace_344", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a reed faience necklace (@earlymodern_jewellery_jos_bead_necklace_344)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition JewelleryAndDevotionalWorkMakeVineAmberBeltOrnament() => new()
	{
		Metadata = new("Jewellery and devotional work/make vine amber belt ornament", "crafts", ["earlymodern", "renaissance"], ["item:earlymodern_jewellery_jos_belt_ornament_356"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make vine amber belt ornament",
		Category = "Jewellery and devotional work",
		Blurb = "make a vine amber belt ornament",
		Action = "making jewellery or a devotional item",
		ActiveCraftItemSdesc = "an in-progress vine amber belt ornament craft",
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
			new CatalogueCommodityRecipeInput(Grams: 33.0, Material: "amber", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("33 grams of amber", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Pliers", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Pliers tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "earlymodern_jewellery_jos_belt_ornament_356", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a vine amber belt ornament (@earlymodern_jewellery_jos_belt_ornament_356)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition JewelleryAndDevotionalWorkMakeCheckerCopperBracelet() => new()
	{
		Metadata = new("Jewellery and devotional work/make checker copper bracelet", "crafts", ["earlymodern", "renaissance"], ["item:earlymodern_jewellery_jos_bracelet_346"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make checker copper bracelet",
		Category = "Jewellery and devotional work",
		Blurb = "make a checker copper bracelet",
		Action = "making jewellery or a devotional item",
		ActiveCraftItemSdesc = "an in-progress checker copper bracelet craft",
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
			new CatalogueCommodityRecipeInput(Grams: 38.0, Material: "copper", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("38 grams of copper", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Pliers", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Pliers tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "earlymodern_jewellery_jos_bracelet_346", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a checker copper bracelet (@earlymodern_jewellery_jos_bracelet_346)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition JewelleryAndDevotionalWorkMakeSunburstBrassBrooch() => new()
	{
		Metadata = new("Jewellery and devotional work/make sunburst brass brooch", "crafts", ["earlymodern", "renaissance"], ["item:earlymodern_jewellery_jos_brooch_350"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make sunburst brass brooch",
		Category = "Jewellery and devotional work",
		Blurb = "make a sunburst brass brooch",
		Action = "making jewellery or a devotional item",
		ActiveCraftItemSdesc = "an in-progress sunburst brass brooch craft",
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
			new CatalogueItemRecipeProduct(ItemStableReference: "earlymodern_jewellery_jos_brooch_350", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a sunburst brass brooch (@earlymodern_jewellery_jos_brooch_350)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition JewelleryAndDevotionalWorkMakeRosettePewterNecklace() => new()
	{
		Metadata = new("Jewellery and devotional work/make rosette pewter necklace", "crafts", ["earlymodern", "renaissance"], ["item:earlymodern_jewellery_jos_chain_necklace_345"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make rosette pewter necklace",
		Category = "Jewellery and devotional work",
		Blurb = "make a rosette pewter necklace",
		Action = "making jewellery or a devotional item",
		ActiveCraftItemSdesc = "an in-progress rosette pewter necklace craft",
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
			new CatalogueCommodityRecipeInput(Grams: 33.0, Material: "pewter", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("33 grams of pewter", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Pliers", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Pliers tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "earlymodern_jewellery_jos_chain_necklace_345", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a rosette pewter necklace (@earlymodern_jewellery_jos_chain_necklace_345)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition JewelleryAndDevotionalWorkMakeFlowerHeadShellCirclet() => new()
	{
		Metadata = new("Jewellery and devotional work/make flower-head shell circlet", "crafts", ["earlymodern", "renaissance"], ["item:earlymodern_jewellery_jos_circlet_353"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make flower-head shell circlet",
		Category = "Jewellery and devotional work",
		Blurb = "make a flower-head shell circlet",
		Action = "making jewellery or a devotional item",
		ActiveCraftItemSdesc = "an in-progress flower-head shell circlet craft",
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
			new CatalogueCommodityRecipeInput(Grams: 16.0, Material: "shell", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("16 grams of shell", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Pliers", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Pliers tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "earlymodern_jewellery_jos_circlet_353", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a flower-head shell circlet (@earlymodern_jewellery_jos_circlet_353)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition JewelleryAndDevotionalWorkMakeStarIvoryDiadem() => new()
	{
		Metadata = new("Jewellery and devotional work/make star ivory diadem", "crafts", ["earlymodern", "renaissance"], ["item:earlymodern_jewellery_jos_diadem_354"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make star ivory diadem",
		Category = "Jewellery and devotional work",
		Blurb = "make a star ivory diadem",
		Action = "making jewellery or a devotional item",
		ActiveCraftItemSdesc = "an in-progress star ivory diadem craft",
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
			new CatalogueCommodityRecipeInput(Grams: 22.0, Material: "ivory", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("22 grams of ivory", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Pliers", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Pliers tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "earlymodern_jewellery_jos_diadem_354", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a star ivory diadem (@earlymodern_jewellery_jos_diadem_354)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition JewelleryAndDevotionalWorkMakeLeafScrollGoldEarrings() => new()
	{
		Metadata = new("Jewellery and devotional work/make leaf-scroll gold earrings", "crafts", ["earlymodern", "renaissance"], ["item:earlymodern_jewellery_jos_earrings_349"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make leaf-scroll gold earrings",
		Category = "Jewellery and devotional work",
		Blurb = "make a leaf-scroll gold earrings",
		Action = "making jewellery or a devotional item",
		ActiveCraftItemSdesc = "an in-progress leaf-scroll gold earrings craft",
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
			new CatalogueItemRecipeProduct(ItemStableReference: "earlymodern_jewellery_jos_earrings_349", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a leaf-scroll gold earrings (@earlymodern_jewellery_jos_earrings_349)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition JewelleryAndDevotionalWorkMakeKnottedBoneHairpin() => new()
	{
		Metadata = new("Jewellery and devotional work/make knotted bone hairpin", "crafts", ["earlymodern", "renaissance"], ["item:earlymodern_jewellery_jos_hairpin_352"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make knotted bone hairpin",
		Category = "Jewellery and devotional work",
		Blurb = "make a knotted bone hairpin",
		Action = "making jewellery or a devotional item",
		ActiveCraftItemSdesc = "an in-progress knotted bone hairpin craft",
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
			new CatalogueCommodityRecipeInput(Grams: 72.0, Material: "bone", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("72 grams of bone", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Pliers", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Pliers tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "earlymodern_jewellery_jos_hairpin_352", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a knotted bone hairpin (@earlymodern_jewellery_jos_hairpin_352)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition JewelleryAndDevotionalWorkMakeShellQuartzPendant() => new()
	{
		Metadata = new("Jewellery and devotional work/make shell quartz pendant", "crafts", ["earlymodern", "renaissance"], ["item:earlymodern_jewellery_jos_pendant_343"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make shell quartz pendant",
		Category = "Jewellery and devotional work",
		Blurb = "make a shell quartz pendant",
		Action = "making jewellery or a devotional item",
		ActiveCraftItemSdesc = "an in-progress shell quartz pendant craft",
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
			new CatalogueCommodityRecipeInput(Grams: 22.0, Material: "quartz", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("22 grams of quartz", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Pliers", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Pliers tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "earlymodern_jewellery_jos_pendant_343", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a shell quartz pendant (@earlymodern_jewellery_jos_pendant_343)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition JewelleryAndDevotionalWorkMakeWaterWaveGlassPin() => new()
	{
		Metadata = new("Jewellery and devotional work/make water-wave glass pin", "crafts", ["earlymodern", "renaissance"], ["item:earlymodern_jewellery_jos_pin_351"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make water-wave glass pin",
		Category = "Jewellery and devotional work",
		Blurb = "make a water-wave glass pin",
		Action = "making jewellery or a devotional item",
		ActiveCraftItemSdesc = "an in-progress water-wave glass pin craft",
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
			new CatalogueCommodityRecipeInput(Grams: 66.0, Material: "glass", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("66 grams of glass", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Pliers", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Pliers tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "earlymodern_jewellery_jos_pin_351", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a water-wave glass pin (@earlymodern_jewellery_jos_pin_351)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition JewelleryAndDevotionalWorkMakeGeometricTurquoiseRing() => new()
	{
		Metadata = new("Jewellery and devotional work/make geometric turquoise ring", "crafts", ["earlymodern", "renaissance"], ["item:earlymodern_jewellery_jos_ring_341"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make geometric turquoise ring",
		Category = "Jewellery and devotional work",
		Blurb = "make a geometric turquoise ring",
		Action = "making jewellery or a devotional item",
		ActiveCraftItemSdesc = "an in-progress geometric turquoise ring craft",
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
			new CatalogueCommodityRecipeInput(Grams: 72.0, Material: "turquoise", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("72 grams of turquoise", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Pliers", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Pliers tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "earlymodern_jewellery_jos_ring_341", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a geometric turquoise ring (@earlymodern_jewellery_jos_ring_341)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition JewelleryAndDevotionalWorkMakeCloudJadeRing() => new()
	{
		Metadata = new("Jewellery and devotional work/make cloud jade ring", "crafts", ["earlymodern", "renaissance"], ["item:earlymodern_jewellery_jos_signet_ring_342"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make cloud jade ring",
		Category = "Jewellery and devotional work",
		Blurb = "make a cloud jade ring",
		Action = "making jewellery or a devotional item",
		ActiveCraftItemSdesc = "an in-progress cloud jade ring craft",
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
			new CatalogueCommodityRecipeInput(Grams: 16.0, Material: "jade", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("16 grams of jade", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Pliers", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Pliers tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "earlymodern_jewellery_jos_signet_ring_342", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a cloud jade ring (@earlymodern_jewellery_jos_signet_ring_342)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition JewelleryAndDevotionalWorkMakeCloudGarnetTempleRing() => new()
	{
		Metadata = new("Jewellery and devotional work/make cloud garnet temple ring", "crafts", ["earlymodern", "renaissance"], ["item:earlymodern_jewellery_jos_temple_ring_358"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make cloud garnet temple ring",
		Category = "Jewellery and devotional work",
		Blurb = "make a cloud garnet temple ring",
		Action = "making jewellery or a devotional item",
		ActiveCraftItemSdesc = "an in-progress cloud garnet temple ring craft",
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
			new CatalogueItemRecipeProduct(ItemStableReference: "earlymodern_jewellery_jos_temple_ring_358", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a cloud garnet temple ring (@earlymodern_jewellery_jos_temple_ring_358)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition JewelleryAndDevotionalWorkMakeShellAgateTorc() => new()
	{
		Metadata = new("Jewellery and devotional work/make shell agate torc", "crafts", ["earlymodern", "renaissance"], ["item:earlymodern_jewellery_jos_torc_359"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make shell agate torc",
		Category = "Jewellery and devotional work",
		Blurb = "make a shell agate torc",
		Action = "making jewellery or a devotional item",
		ActiveCraftItemSdesc = "an in-progress shell agate torc craft",
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
			new CatalogueCommodityRecipeInput(Grams: 50.0, Material: "agate", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("50 grams of agate", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Pliers", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Pliers tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "earlymodern_jewellery_jos_torc_359", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a shell agate torc (@earlymodern_jewellery_jos_torc_359)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition JewelleryAndDevotionalWorkMakeBirdPearlWaistChain() => new()
	{
		Metadata = new("Jewellery and devotional work/make bird pearl waist chain", "crafts", ["earlymodern", "renaissance"], ["item:earlymodern_jewellery_jos_waist_chain_355"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make bird pearl waist chain",
		Category = "Jewellery and devotional work",
		Blurb = "make a bird pearl waist chain",
		Action = "making jewellery or a devotional item",
		ActiveCraftItemSdesc = "an in-progress bird pearl waist chain craft",
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
			new CatalogueItemRecipeProduct(ItemStableReference: "earlymodern_jewellery_jos_waist_chain_355", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a bird pearl waist chain (@earlymodern_jewellery_jos_waist_chain_355)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition JewelleryAndDevotionalWorkMakeReedCarnelianWreath() => new()
	{
		Metadata = new("Jewellery and devotional work/make reed carnelian wreath", "crafts", ["earlymodern", "renaissance"], ["item:earlymodern_jewellery_jos_wreath_360"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make reed carnelian wreath",
		Category = "Jewellery and devotional work",
		Blurb = "make a reed carnelian wreath",
		Action = "making jewellery or a devotional item",
		ActiveCraftItemSdesc = "an in-progress reed carnelian wreath craft",
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
			new CatalogueItemRecipeProduct(ItemStableReference: "earlymodern_jewellery_jos_wreath_360", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a reed carnelian wreath (@earlymodern_jewellery_jos_wreath_360)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition JewelleryAndDevotionalWorkMakeVineLapisLazuliAnklet() => new()
	{
		Metadata = new("Jewellery and devotional work/make vine lapis lazuli anklet", "crafts", ["earlymodern", "renaissance"], ["item:earlymodern_jewellery_kongo_anklet_488"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make vine lapis lazuli anklet",
		Category = "Jewellery and devotional work",
		Blurb = "make a vine lapis lazuli anklet",
		Action = "making jewellery or a devotional item",
		ActiveCraftItemSdesc = "an in-progress vine lapis lazuli anklet craft",
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
			new CatalogueItemRecipeProduct(ItemStableReference: "earlymodern_jewellery_kongo_anklet_488", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a vine lapis lazuli anklet (@earlymodern_jewellery_kongo_anklet_488)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition JewelleryAndDevotionalWorkMakeBirdCarnelianArmlet() => new()
	{
		Metadata = new("Jewellery and devotional work/make bird carnelian armlet", "crafts", ["earlymodern", "renaissance"], ["item:earlymodern_jewellery_kongo_armlet_487"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make bird carnelian armlet",
		Category = "Jewellery and devotional work",
		Blurb = "make a bird carnelian armlet",
		Action = "making jewellery or a devotional item",
		ActiveCraftItemSdesc = "an in-progress bird carnelian armlet craft",
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
			new CatalogueItemRecipeProduct(ItemStableReference: "earlymodern_jewellery_kongo_armlet_487", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a bird carnelian armlet (@earlymodern_jewellery_kongo_armlet_487)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition JewelleryAndDevotionalWorkMakeLeafScrollGoldBadge() => new()
	{
		Metadata = new("Jewellery and devotional work/make leaf-scroll gold badge", "crafts", ["earlymodern", "renaissance"], ["item:earlymodern_jewellery_kongo_badge_497"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make leaf-scroll gold badge",
		Category = "Jewellery and devotional work",
		Blurb = "make a leaf-scroll gold badge",
		Action = "making jewellery or a devotional item",
		ActiveCraftItemSdesc = "an in-progress leaf-scroll gold badge craft",
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
			new CatalogueItemRecipeProduct(ItemStableReference: "earlymodern_jewellery_kongo_badge_497", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a leaf-scroll gold badge (@earlymodern_jewellery_kongo_badge_497)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition JewelleryAndDevotionalWorkMakeKnottedCoralNecklace() => new()
	{
		Metadata = new("Jewellery and devotional work/make knotted coral necklace", "crafts", ["earlymodern", "renaissance"], ["item:earlymodern_jewellery_kongo_bead_necklace_484"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make knotted coral necklace",
		Category = "Jewellery and devotional work",
		Blurb = "make a knotted coral necklace",
		Action = "making jewellery or a devotional item",
		ActiveCraftItemSdesc = "an in-progress knotted coral necklace craft",
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
			new CatalogueItemRecipeProduct(ItemStableReference: "earlymodern_jewellery_kongo_bead_necklace_484", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a knotted coral necklace (@earlymodern_jewellery_kongo_bead_necklace_484)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition JewelleryAndDevotionalWorkMakeRadiatingSilverBeltOrnament() => new()
	{
		Metadata = new("Jewellery and devotional work/make radiating silver belt ornament", "crafts", ["earlymodern", "renaissance"], ["item:earlymodern_jewellery_kongo_belt_ornament_496"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make radiating silver belt ornament",
		Category = "Jewellery and devotional work",
		Blurb = "make a radiating silver belt ornament",
		Action = "making jewellery or a devotional item",
		ActiveCraftItemSdesc = "an in-progress radiating silver belt ornament craft",
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
			new CatalogueItemRecipeProduct(ItemStableReference: "earlymodern_jewellery_kongo_belt_ornament_496", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a radiating silver belt ornament (@earlymodern_jewellery_kongo_belt_ornament_496)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition JewelleryAndDevotionalWorkMakeStarAgateBracelet() => new()
	{
		Metadata = new("Jewellery and devotional work/make star agate bracelet", "crafts", ["earlymodern", "renaissance"], ["item:earlymodern_jewellery_kongo_bracelet_486"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make star agate bracelet",
		Category = "Jewellery and devotional work",
		Blurb = "make a star agate bracelet",
		Action = "making jewellery or a devotional item",
		ActiveCraftItemSdesc = "an in-progress star agate bracelet craft",
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
			new CatalogueItemRecipeProduct(ItemStableReference: "earlymodern_jewellery_kongo_bracelet_486", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a star agate bracelet (@earlymodern_jewellery_kongo_bracelet_486)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition JewelleryAndDevotionalWorkMakeCloudJadeBrooch() => new()
	{
		Metadata = new("Jewellery and devotional work/make cloud jade brooch", "crafts", ["earlymodern", "renaissance"], ["item:earlymodern_jewellery_kongo_brooch_490"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make cloud jade brooch",
		Category = "Jewellery and devotional work",
		Blurb = "make a cloud jade brooch",
		Action = "making jewellery or a devotional item",
		ActiveCraftItemSdesc = "an in-progress cloud jade brooch craft",
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
			new CatalogueItemRecipeProduct(ItemStableReference: "earlymodern_jewellery_kongo_brooch_490", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a cloud jade brooch (@earlymodern_jewellery_kongo_brooch_490)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition JewelleryAndDevotionalWorkMakeFlowerHeadGarnetNecklace() => new()
	{
		Metadata = new("Jewellery and devotional work/make flower-head garnet necklace", "crafts", ["earlymodern", "renaissance"], ["item:earlymodern_jewellery_kongo_chain_necklace_485"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make flower-head garnet necklace",
		Category = "Jewellery and devotional work",
		Blurb = "make a flower-head garnet necklace",
		Action = "making jewellery or a devotional item",
		ActiveCraftItemSdesc = "an in-progress flower-head garnet necklace craft",
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
			new CatalogueItemRecipeProduct(ItemStableReference: "earlymodern_jewellery_kongo_chain_necklace_485", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a flower-head garnet necklace (@earlymodern_jewellery_kongo_chain_necklace_485)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition JewelleryAndDevotionalWorkMakeRosettePewterCirclet() => new()
	{
		Metadata = new("Jewellery and devotional work/make rosette pewter circlet", "crafts", ["earlymodern", "renaissance"], ["item:earlymodern_jewellery_kongo_circlet_493"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make rosette pewter circlet",
		Category = "Jewellery and devotional work",
		Blurb = "make a rosette pewter circlet",
		Action = "making jewellery or a devotional item",
		ActiveCraftItemSdesc = "an in-progress rosette pewter circlet craft",
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
			new CatalogueItemRecipeProduct(ItemStableReference: "earlymodern_jewellery_kongo_circlet_493", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a rosette pewter circlet (@earlymodern_jewellery_kongo_circlet_493)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition JewelleryAndDevotionalWorkMakeCheckerCopperDiadem() => new()
	{
		Metadata = new("Jewellery and devotional work/make checker copper diadem", "crafts", ["earlymodern", "renaissance"], ["item:earlymodern_jewellery_kongo_diadem_494"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make checker copper diadem",
		Category = "Jewellery and devotional work",
		Blurb = "make a checker copper diadem",
		Action = "making jewellery or a devotional item",
		ActiveCraftItemSdesc = "an in-progress checker copper diadem craft",
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
			new CatalogueCommodityRecipeInput(Grams: 66.0, Material: "copper", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("66 grams of copper", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Pliers", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Pliers tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "earlymodern_jewellery_kongo_diadem_494", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a checker copper diadem (@earlymodern_jewellery_kongo_diadem_494)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition JewelleryAndDevotionalWorkMakeGeometricTurquoiseEarrings() => new()
	{
		Metadata = new("Jewellery and devotional work/make geometric turquoise earrings", "crafts", ["earlymodern", "renaissance"], ["item:earlymodern_jewellery_kongo_earrings_489"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make geometric turquoise earrings",
		Category = "Jewellery and devotional work",
		Blurb = "make a geometric turquoise earrings",
		Action = "making jewellery or a devotional item",
		ActiveCraftItemSdesc = "an in-progress geometric turquoise earrings craft",
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
			new CatalogueItemRecipeProduct(ItemStableReference: "earlymodern_jewellery_kongo_earrings_489", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a geometric turquoise earrings (@earlymodern_jewellery_kongo_earrings_489)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition JewelleryAndDevotionalWorkMakeReedFaienceHairpin() => new()
	{
		Metadata = new("Jewellery and devotional work/make reed faience hairpin", "crafts", ["earlymodern", "renaissance"], ["item:earlymodern_jewellery_kongo_hairpin_492"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make reed faience hairpin",
		Category = "Jewellery and devotional work",
		Blurb = "make a reed faience hairpin",
		Action = "making jewellery or a devotional item",
		ActiveCraftItemSdesc = "an in-progress reed faience hairpin craft",
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
			new CatalogueItemRecipeProduct(ItemStableReference: "earlymodern_jewellery_kongo_hairpin_492", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a reed faience hairpin (@earlymodern_jewellery_kongo_hairpin_492)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition JewelleryAndDevotionalWorkMakeWaterWaveAmberPendant() => new()
	{
		Metadata = new("Jewellery and devotional work/make water-wave amber pendant", "crafts", ["earlymodern", "renaissance"], ["item:earlymodern_jewellery_kongo_pendant_483"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make water-wave amber pendant",
		Category = "Jewellery and devotional work",
		Blurb = "make a water-wave amber pendant",
		Action = "making jewellery or a devotional item",
		ActiveCraftItemSdesc = "an in-progress water-wave amber pendant craft",
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
			new CatalogueCommodityRecipeInput(Grams: 66.0, Material: "amber", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("66 grams of amber", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Pliers", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Pliers tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "earlymodern_jewellery_kongo_pendant_483", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a water-wave amber pendant (@earlymodern_jewellery_kongo_pendant_483)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition JewelleryAndDevotionalWorkMakeShellQuartzPin() => new()
	{
		Metadata = new("Jewellery and devotional work/make shell quartz pin", "crafts", ["earlymodern", "renaissance"], ["item:earlymodern_jewellery_kongo_pin_491"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make shell quartz pin",
		Category = "Jewellery and devotional work",
		Blurb = "make a shell quartz pin",
		Action = "making jewellery or a devotional item",
		ActiveCraftItemSdesc = "an in-progress shell quartz pin craft",
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
			new CatalogueItemRecipeProduct(ItemStableReference: "earlymodern_jewellery_kongo_pin_491", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a shell quartz pin (@earlymodern_jewellery_kongo_pin_491)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition JewelleryAndDevotionalWorkMakeLeafScrollIvoryRing() => new()
	{
		Metadata = new("Jewellery and devotional work/make leaf-scroll ivory ring", "crafts", ["earlymodern", "renaissance"], ["item:earlymodern_jewellery_kongo_ring_481"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make leaf-scroll ivory ring",
		Category = "Jewellery and devotional work",
		Blurb = "make a leaf-scroll ivory ring",
		Action = "making jewellery or a devotional item",
		ActiveCraftItemSdesc = "an in-progress leaf-scroll ivory ring craft",
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
			new CatalogueItemRecipeProduct(ItemStableReference: "earlymodern_jewellery_kongo_ring_481", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a leaf-scroll ivory ring (@earlymodern_jewellery_kongo_ring_481)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition JewelleryAndDevotionalWorkMakeSunburstPearlRing() => new()
	{
		Metadata = new("Jewellery and devotional work/make sunburst pearl ring", "crafts", ["earlymodern", "renaissance"], ["item:earlymodern_jewellery_kongo_signet_ring_482"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make sunburst pearl ring",
		Category = "Jewellery and devotional work",
		Blurb = "make a sunburst pearl ring",
		Action = "making jewellery or a devotional item",
		ActiveCraftItemSdesc = "an in-progress sunburst pearl ring craft",
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
			new CatalogueCommodityRecipeInput(Grams: 61.0, Material: "pearl", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("61 grams of pearl", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Pliers", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Pliers tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "earlymodern_jewellery_kongo_signet_ring_482", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a sunburst pearl ring (@earlymodern_jewellery_kongo_signet_ring_482)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition JewelleryAndDevotionalWorkMakeSunburstBrassTempleRing() => new()
	{
		Metadata = new("Jewellery and devotional work/make sunburst brass temple ring", "crafts", ["earlymodern", "renaissance"], ["item:earlymodern_jewellery_kongo_temple_ring_498"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make sunburst brass temple ring",
		Category = "Jewellery and devotional work",
		Blurb = "make a sunburst brass temple ring",
		Action = "making jewellery or a devotional item",
		ActiveCraftItemSdesc = "an in-progress sunburst brass temple ring craft",
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
			new CatalogueItemRecipeProduct(ItemStableReference: "earlymodern_jewellery_kongo_temple_ring_498", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a sunburst brass temple ring (@earlymodern_jewellery_kongo_temple_ring_498)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition JewelleryAndDevotionalWorkMakeWaterWaveGlassTorc() => new()
	{
		Metadata = new("Jewellery and devotional work/make water-wave glass torc", "crafts", ["earlymodern", "renaissance"], ["item:earlymodern_jewellery_kongo_torc_499"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make water-wave glass torc",
		Category = "Jewellery and devotional work",
		Blurb = "make a water-wave glass torc",
		Action = "making jewellery or a devotional item",
		ActiveCraftItemSdesc = "an in-progress water-wave glass torc craft",
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
			new CatalogueItemRecipeProduct(ItemStableReference: "earlymodern_jewellery_kongo_torc_499", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a water-wave glass torc (@earlymodern_jewellery_kongo_torc_499)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition JewelleryAndDevotionalWorkMakeBraidedBronzeWaistChain() => new()
	{
		Metadata = new("Jewellery and devotional work/make braided bronze waist chain", "crafts", ["earlymodern", "renaissance"], ["item:earlymodern_jewellery_kongo_waist_chain_495"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make braided bronze waist chain",
		Category = "Jewellery and devotional work",
		Blurb = "make a braided bronze waist chain",
		Action = "making jewellery or a devotional item",
		ActiveCraftItemSdesc = "an in-progress braided bronze waist chain craft",
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
			new CatalogueItemRecipeProduct(ItemStableReference: "earlymodern_jewellery_kongo_waist_chain_495", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a braided bronze waist chain (@earlymodern_jewellery_kongo_waist_chain_495)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition JewelleryAndDevotionalWorkMakeKnottedBoneWreath() => new()
	{
		Metadata = new("Jewellery and devotional work/make knotted bone wreath", "crafts", ["earlymodern", "renaissance"], ["item:earlymodern_jewellery_kongo_wreath_500"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make knotted bone wreath",
		Category = "Jewellery and devotional work",
		Blurb = "make a knotted bone wreath",
		Action = "making jewellery or a devotional item",
		ActiveCraftItemSdesc = "an in-progress knotted bone wreath craft",
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
			new CatalogueItemRecipeProduct(ItemStableReference: "earlymodern_jewellery_kongo_wreath_500", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a knotted bone wreath (@earlymodern_jewellery_kongo_wreath_500)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition JewelleryAndDevotionalWorkMakeSunburstFaienceAnklet() => new()
	{
		Metadata = new("Jewellery and devotional work/make sunburst faience anklet", "crafts", ["earlymodern", "renaissance"], ["item:earlymodern_jewellery_mag_anklet_208"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make sunburst faience anklet",
		Category = "Jewellery and devotional work",
		Blurb = "make a sunburst faience anklet",
		Action = "making jewellery or a devotional item",
		ActiveCraftItemSdesc = "an in-progress sunburst faience anklet craft",
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
			new CatalogueItemRecipeProduct(ItemStableReference: "earlymodern_jewellery_mag_anklet_208", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a sunburst faience anklet (@earlymodern_jewellery_mag_anklet_208)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition JewelleryAndDevotionalWorkMakeLeafScrollQuartzArmlet() => new()
	{
		Metadata = new("Jewellery and devotional work/make leaf-scroll quartz armlet", "crafts", ["earlymodern", "renaissance"], ["item:earlymodern_jewellery_mag_armlet_207"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make leaf-scroll quartz armlet",
		Category = "Jewellery and devotional work",
		Blurb = "make a leaf-scroll quartz armlet",
		Action = "making jewellery or a devotional item",
		ActiveCraftItemSdesc = "an in-progress leaf-scroll quartz armlet craft",
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
			new CatalogueItemRecipeProduct(ItemStableReference: "earlymodern_jewellery_mag_armlet_207", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a leaf-scroll quartz armlet (@earlymodern_jewellery_mag_armlet_207)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition JewelleryAndDevotionalWorkMakeShellShellBadge() => new()
	{
		Metadata = new("Jewellery and devotional work/make shell shell badge", "crafts", ["earlymodern", "renaissance"], ["item:earlymodern_jewellery_mag_badge_217"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make shell shell badge",
		Category = "Jewellery and devotional work",
		Blurb = "make a shell shell badge",
		Action = "making jewellery or a devotional item",
		ActiveCraftItemSdesc = "an in-progress shell shell badge craft",
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
			new CatalogueItemRecipeProduct(ItemStableReference: "earlymodern_jewellery_mag_badge_217", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a shell shell badge (@earlymodern_jewellery_mag_badge_217)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition JewelleryAndDevotionalWorkMakeCheckerLapisLazuliNecklace() => new()
	{
		Metadata = new("Jewellery and devotional work/make checker lapis lazuli necklace", "crafts", ["earlymodern", "renaissance"], ["item:earlymodern_jewellery_mag_bead_necklace_204"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make checker lapis lazuli necklace",
		Category = "Jewellery and devotional work",
		Blurb = "make a checker lapis lazuli necklace",
		Action = "making jewellery or a devotional item",
		ActiveCraftItemSdesc = "an in-progress checker lapis lazuli necklace craft",
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
			new CatalogueCommodityRecipeInput(Grams: 44.0, Material: "lapis lazuli", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("44 grams of lapis lazuli", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Pliers", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Pliers tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "earlymodern_jewellery_mag_bead_necklace_204", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a checker lapis lazuli necklace (@earlymodern_jewellery_mag_bead_necklace_204)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition JewelleryAndDevotionalWorkMakeCloudBoneBeltOrnament() => new()
	{
		Metadata = new("Jewellery and devotional work/make cloud bone belt ornament", "crafts", ["earlymodern", "renaissance"], ["item:earlymodern_jewellery_mag_belt_ornament_216"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make cloud bone belt ornament",
		Category = "Jewellery and devotional work",
		Blurb = "make a cloud bone belt ornament",
		Action = "making jewellery or a devotional item",
		ActiveCraftItemSdesc = "an in-progress cloud bone belt ornament craft",
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
			new CatalogueItemRecipeProduct(ItemStableReference: "earlymodern_jewellery_mag_belt_ornament_216", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a cloud bone belt ornament (@earlymodern_jewellery_mag_belt_ornament_216)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition JewelleryAndDevotionalWorkMakeRadiatingJadeBracelet() => new()
	{
		Metadata = new("Jewellery and devotional work/make radiating jade bracelet", "crafts", ["earlymodern", "renaissance"], ["item:earlymodern_jewellery_mag_bracelet_206"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make radiating jade bracelet",
		Category = "Jewellery and devotional work",
		Blurb = "make a radiating jade bracelet",
		Action = "making jewellery or a devotional item",
		ActiveCraftItemSdesc = "an in-progress radiating jade bracelet craft",
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
			new CatalogueItemRecipeProduct(ItemStableReference: "earlymodern_jewellery_mag_bracelet_206", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a radiating jade bracelet (@earlymodern_jewellery_mag_bracelet_206)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition JewelleryAndDevotionalWorkMakeKnottedCopperBrooch() => new()
	{
		Metadata = new("Jewellery and devotional work/make knotted copper brooch", "crafts", ["earlymodern", "renaissance"], ["item:earlymodern_jewellery_mag_brooch_210"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make knotted copper brooch",
		Category = "Jewellery and devotional work",
		Blurb = "make a knotted copper brooch",
		Action = "making jewellery or a devotional item",
		ActiveCraftItemSdesc = "an in-progress knotted copper brooch craft",
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
			new CatalogueItemRecipeProduct(ItemStableReference: "earlymodern_jewellery_mag_brooch_210", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a knotted copper brooch (@earlymodern_jewellery_mag_brooch_210)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition JewelleryAndDevotionalWorkMakeBraidedTurquoiseNecklace() => new()
	{
		Metadata = new("Jewellery and devotional work/make braided turquoise necklace", "crafts", ["earlymodern", "renaissance"], ["item:earlymodern_jewellery_mag_chain_necklace_205"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make braided turquoise necklace",
		Category = "Jewellery and devotional work",
		Blurb = "make a braided turquoise necklace",
		Action = "making jewellery or a devotional item",
		ActiveCraftItemSdesc = "an in-progress braided turquoise necklace craft",
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
			new CatalogueItemRecipeProduct(ItemStableReference: "earlymodern_jewellery_mag_chain_necklace_205", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a braided turquoise necklace (@earlymodern_jewellery_mag_chain_necklace_205)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition JewelleryAndDevotionalWorkMakeBirdGoldCirclet() => new()
	{
		Metadata = new("Jewellery and devotional work/make bird gold circlet", "crafts", ["earlymodern", "renaissance"], ["item:earlymodern_jewellery_mag_circlet_213"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make bird gold circlet",
		Category = "Jewellery and devotional work",
		Blurb = "make a bird gold circlet",
		Action = "making jewellery or a devotional item",
		ActiveCraftItemSdesc = "an in-progress bird gold circlet craft",
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
			new CatalogueItemRecipeProduct(ItemStableReference: "earlymodern_jewellery_mag_circlet_213", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a bird gold circlet (@earlymodern_jewellery_mag_circlet_213)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};
}
