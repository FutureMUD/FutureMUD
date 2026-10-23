#nullable enable

using MudSharp.Body;
using MudSharp.GameItems.Inventory.Plans;
using MudSharp.RPG.Checks;

namespace DatabaseSeeder.Seeders;

public static partial class Recipes01Stock
{
	public static CatalogueRecipeDefinition GemcraftCutJasperJewellerySettingStock() => new()
	{
		Metadata = new("Gemcraft/cut jasper jewellery setting stock", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Antiquity.cs:140", []),
		RequiredAnyEra = ["antiquity"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "cut jasper jewellery setting stock",
		Category = "Gemcraft",
		Blurb = "cut and polish jasper into jewellery settings",
		Action = "cut jasper jewellery setting stock",
		ActiveCraftItemSdesc = "an in-progress and polish jasper jewellery settings craft",
		Gate = CatalogueRecipeGates.AncientJewelleryCraftingGemcraft15,
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
			new(25, "$0 inspect|inspects $i1 and select|selects pieces fit for jewellery work.", "$0 inspect|inspects $i1 and select|selects pieces fit for jewellery work.", ExertionLevel.Stasis, 0),
			new(35, "$0 work|works the material into small, regular stock, using $t1 and $t2.", "$0 work|works the material into small, regular stock, using $t1 and $t2.", ExertionLevel.Stasis, 0),
			new(25, "$0 finish|finishes the jewellery stock and set|sets aside $p1.", "$0 finish|finishes the stock, but the result is not worth keeping.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 240.0, Material: "jasper", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Specific, Characteristics: [new("Colour", null), new("Fine Colour", null)]) { Compatibility = new("240 grams of jasper", ["characteristic Colour any", "characteristic Fine Colour any"]), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Bow Drill", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Bow Drill tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Polishing Stone", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Polishing Stone tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueCommodityRecipeProduct(Grams: 170.0, Material: "jasper", Tag: "Jewellery Setting Stock", Characteristics: [new("Colour", null, 1), new("Fine Colour", null, 1)]) { Compatibility = new("170 grams of jasper commodity", ["tag Jewellery Setting Stock", "characteristic Colour from $i1", "characteristic Fine Colour from $i1"]), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition GemcraftDrillLapisLazuliJewelleryBeadStock() => new()
	{
		Metadata = new("Gemcraft/drill lapis lazuli jewellery bead stock", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Antiquity.cs:140", []),
		RequiredAnyEra = ["antiquity"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "drill lapis lazuli jewellery bead stock",
		Category = "Gemcraft",
		Blurb = "drill and polish lapis lazuli into jewellery beads",
		Action = "drill lapis lazuli jewellery bead stock",
		ActiveCraftItemSdesc = "an in-progress drill and polish lapis lazuli jewellery beads craft",
		Gate = CatalogueRecipeGates.AncientJewelleryCraftingGemcraft15,
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
			new(25, "$0 inspect|inspects $i1 and select|selects pieces fit for jewellery work.", "$0 inspect|inspects $i1 and select|selects pieces fit for jewellery work.", ExertionLevel.Stasis, 0),
			new(35, "$0 work|works the material into small, regular stock, using $t1 and $t2.", "$0 work|works the material into small, regular stock, using $t1 and $t2.", ExertionLevel.Stasis, 0),
			new(25, "$0 finish|finishes the jewellery stock and set|sets aside $p1.", "$0 finish|finishes the stock, but the result is not worth keeping.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 260.0, Material: "lapis lazuli", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Specific, Characteristics: [new("Colour", null), new("Fine Colour", null)]) { Compatibility = new("260 grams of lapis lazuli", ["characteristic Colour any", "characteristic Fine Colour any"]), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Bow Drill", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Bow Drill tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Polishing Stone", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Polishing Stone tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueCommodityRecipeProduct(Grams: 190.0, Material: "lapis lazuli", Tag: "Jewellery Bead Stock", Characteristics: [new("Colour", null, 1), new("Fine Colour", null, 1)]) { Compatibility = new("190 grams of lapis lazuli commodity", ["tag Jewellery Bead Stock", "characteristic Colour from $i1", "characteristic Fine Colour from $i1"]), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition GemcraftCutLapisLazuliJewellerySettingStock() => new()
	{
		Metadata = new("Gemcraft/cut lapis lazuli jewellery setting stock", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Antiquity.cs:140", []),
		RequiredAnyEra = ["antiquity"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "cut lapis lazuli jewellery setting stock",
		Category = "Gemcraft",
		Blurb = "cut and polish lapis lazuli into jewellery settings",
		Action = "cut lapis lazuli jewellery setting stock",
		ActiveCraftItemSdesc = "an in-progress and polish lapis lazuli jewellery settings craft",
		Gate = CatalogueRecipeGates.AncientJewelleryCraftingGemcraft15,
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
			new(25, "$0 inspect|inspects $i1 and select|selects pieces fit for jewellery work.", "$0 inspect|inspects $i1 and select|selects pieces fit for jewellery work.", ExertionLevel.Stasis, 0),
			new(35, "$0 work|works the material into small, regular stock, using $t1 and $t2.", "$0 work|works the material into small, regular stock, using $t1 and $t2.", ExertionLevel.Stasis, 0),
			new(25, "$0 finish|finishes the jewellery stock and set|sets aside $p1.", "$0 finish|finishes the stock, but the result is not worth keeping.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 240.0, Material: "lapis lazuli", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Specific, Characteristics: [new("Colour", null), new("Fine Colour", null)]) { Compatibility = new("240 grams of lapis lazuli", ["characteristic Colour any", "characteristic Fine Colour any"]), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Bow Drill", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Bow Drill tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Polishing Stone", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Polishing Stone tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueCommodityRecipeProduct(Grams: 170.0, Material: "lapis lazuli", Tag: "Jewellery Setting Stock", Characteristics: [new("Colour", null, 1), new("Fine Colour", null, 1)]) { Compatibility = new("170 grams of lapis lazuli commodity", ["tag Jewellery Setting Stock", "characteristic Colour from $i1", "characteristic Fine Colour from $i1"]), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition GemcraftDrillPearlJewelleryBeadStock() => new()
	{
		Metadata = new("Gemcraft/drill pearl jewellery bead stock", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Antiquity.cs:140", []),
		RequiredAnyEra = ["antiquity"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "drill pearl jewellery bead stock",
		Category = "Gemcraft",
		Blurb = "drill and polish pearl into jewellery beads",
		Action = "drill pearl jewellery bead stock",
		ActiveCraftItemSdesc = "an in-progress drill and polish pearl jewellery beads craft",
		Gate = CatalogueRecipeGates.AncientJewelleryCraftingGemcraft15,
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
			new(25, "$0 inspect|inspects $i1 and select|selects pieces fit for jewellery work.", "$0 inspect|inspects $i1 and select|selects pieces fit for jewellery work.", ExertionLevel.Stasis, 0),
			new(35, "$0 work|works the material into small, regular stock, using $t1 and $t2.", "$0 work|works the material into small, regular stock, using $t1 and $t2.", ExertionLevel.Stasis, 0),
			new(25, "$0 finish|finishes the jewellery stock and set|sets aside $p1.", "$0 finish|finishes the stock, but the result is not worth keeping.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 260.0, Material: "pearl", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Specific, Characteristics: [new("Colour", null), new("Fine Colour", null)]) { Compatibility = new("260 grams of pearl", ["characteristic Colour any", "characteristic Fine Colour any"]), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Bow Drill", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Bow Drill tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Polishing Stone", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Polishing Stone tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueCommodityRecipeProduct(Grams: 190.0, Material: "pearl", Tag: "Jewellery Bead Stock", Characteristics: [new("Colour", null, 1), new("Fine Colour", null, 1)]) { Compatibility = new("190 grams of pearl commodity", ["tag Jewellery Bead Stock", "characteristic Colour from $i1", "characteristic Fine Colour from $i1"]), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition GemcraftCutPearlJewellerySettingStock() => new()
	{
		Metadata = new("Gemcraft/cut pearl jewellery setting stock", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Antiquity.cs:140", []),
		RequiredAnyEra = ["antiquity"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "cut pearl jewellery setting stock",
		Category = "Gemcraft",
		Blurb = "cut and polish pearl into jewellery settings",
		Action = "cut pearl jewellery setting stock",
		ActiveCraftItemSdesc = "an in-progress and polish pearl jewellery settings craft",
		Gate = CatalogueRecipeGates.AncientJewelleryCraftingGemcraft15,
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
			new(25, "$0 inspect|inspects $i1 and select|selects pieces fit for jewellery work.", "$0 inspect|inspects $i1 and select|selects pieces fit for jewellery work.", ExertionLevel.Stasis, 0),
			new(35, "$0 work|works the material into small, regular stock, using $t1 and $t2.", "$0 work|works the material into small, regular stock, using $t1 and $t2.", ExertionLevel.Stasis, 0),
			new(25, "$0 finish|finishes the jewellery stock and set|sets aside $p1.", "$0 finish|finishes the stock, but the result is not worth keeping.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 240.0, Material: "pearl", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Specific, Characteristics: [new("Colour", null), new("Fine Colour", null)]) { Compatibility = new("240 grams of pearl", ["characteristic Colour any", "characteristic Fine Colour any"]), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Bow Drill", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Bow Drill tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Polishing Stone", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Polishing Stone tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueCommodityRecipeProduct(Grams: 170.0, Material: "pearl", Tag: "Jewellery Setting Stock", Characteristics: [new("Colour", null, 1), new("Fine Colour", null, 1)]) { Compatibility = new("170 grams of pearl commodity", ["tag Jewellery Setting Stock", "characteristic Colour from $i1", "characteristic Fine Colour from $i1"]), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition GemcraftDrillQuartzJewelleryBeadStock() => new()
	{
		Metadata = new("Gemcraft/drill quartz jewellery bead stock", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Antiquity.cs:140", []),
		RequiredAnyEra = ["antiquity"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "drill quartz jewellery bead stock",
		Category = "Gemcraft",
		Blurb = "drill and polish quartz into jewellery beads",
		Action = "drill quartz jewellery bead stock",
		ActiveCraftItemSdesc = "an in-progress drill and polish quartz jewellery beads craft",
		Gate = CatalogueRecipeGates.AncientJewelleryCraftingGemcraft15,
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
			new(25, "$0 inspect|inspects $i1 and select|selects pieces fit for jewellery work.", "$0 inspect|inspects $i1 and select|selects pieces fit for jewellery work.", ExertionLevel.Stasis, 0),
			new(35, "$0 work|works the material into small, regular stock, using $t1 and $t2.", "$0 work|works the material into small, regular stock, using $t1 and $t2.", ExertionLevel.Stasis, 0),
			new(25, "$0 finish|finishes the jewellery stock and set|sets aside $p1.", "$0 finish|finishes the stock, but the result is not worth keeping.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 260.0, Material: "quartz", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Specific, Characteristics: [new("Colour", null), new("Fine Colour", null)]) { Compatibility = new("260 grams of quartz", ["characteristic Colour any", "characteristic Fine Colour any"]), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Bow Drill", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Bow Drill tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Polishing Stone", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Polishing Stone tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueCommodityRecipeProduct(Grams: 190.0, Material: "quartz", Tag: "Jewellery Bead Stock", Characteristics: [new("Colour", null, 1), new("Fine Colour", null, 1)]) { Compatibility = new("190 grams of quartz commodity", ["tag Jewellery Bead Stock", "characteristic Colour from $i1", "characteristic Fine Colour from $i1"]), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition GemcraftCutQuartzJewellerySettingStock() => new()
	{
		Metadata = new("Gemcraft/cut quartz jewellery setting stock", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Antiquity.cs:140", []),
		RequiredAnyEra = ["antiquity"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "cut quartz jewellery setting stock",
		Category = "Gemcraft",
		Blurb = "cut and polish quartz into jewellery settings",
		Action = "cut quartz jewellery setting stock",
		ActiveCraftItemSdesc = "an in-progress and polish quartz jewellery settings craft",
		Gate = CatalogueRecipeGates.AncientJewelleryCraftingGemcraft15,
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
			new(25, "$0 inspect|inspects $i1 and select|selects pieces fit for jewellery work.", "$0 inspect|inspects $i1 and select|selects pieces fit for jewellery work.", ExertionLevel.Stasis, 0),
			new(35, "$0 work|works the material into small, regular stock, using $t1 and $t2.", "$0 work|works the material into small, regular stock, using $t1 and $t2.", ExertionLevel.Stasis, 0),
			new(25, "$0 finish|finishes the jewellery stock and set|sets aside $p1.", "$0 finish|finishes the stock, but the result is not worth keeping.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 240.0, Material: "quartz", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Specific, Characteristics: [new("Colour", null), new("Fine Colour", null)]) { Compatibility = new("240 grams of quartz", ["characteristic Colour any", "characteristic Fine Colour any"]), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Bow Drill", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Bow Drill tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Polishing Stone", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Polishing Stone tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueCommodityRecipeProduct(Grams: 170.0, Material: "quartz", Tag: "Jewellery Setting Stock", Characteristics: [new("Colour", null, 1), new("Fine Colour", null, 1)]) { Compatibility = new("170 grams of quartz commodity", ["tag Jewellery Setting Stock", "characteristic Colour from $i1", "characteristic Fine Colour from $i1"]), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition GemcraftDrillShellJewelleryBeadStock() => new()
	{
		Metadata = new("Gemcraft/drill shell jewellery bead stock", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Antiquity.cs:140", []),
		RequiredAnyEra = ["antiquity"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "drill shell jewellery bead stock",
		Category = "Gemcraft",
		Blurb = "drill and polish shell into jewellery beads",
		Action = "drill shell jewellery bead stock",
		ActiveCraftItemSdesc = "an in-progress drill and polish shell jewellery beads craft",
		Gate = CatalogueRecipeGates.AncientJewelleryCraftingGemcraft15,
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
			new(25, "$0 inspect|inspects $i1 and select|selects pieces fit for jewellery work.", "$0 inspect|inspects $i1 and select|selects pieces fit for jewellery work.", ExertionLevel.Stasis, 0),
			new(35, "$0 work|works the material into small, regular stock, using $t1 and $t2.", "$0 work|works the material into small, regular stock, using $t1 and $t2.", ExertionLevel.Stasis, 0),
			new(25, "$0 finish|finishes the jewellery stock and set|sets aside $p1.", "$0 finish|finishes the stock, but the result is not worth keeping.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 260.0, Material: "shell", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Specific, Characteristics: [new("Colour", null), new("Fine Colour", null)]) { Compatibility = new("260 grams of shell", ["characteristic Colour any", "characteristic Fine Colour any"]), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Bow Drill", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Bow Drill tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Polishing Stone", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Polishing Stone tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueCommodityRecipeProduct(Grams: 190.0, Material: "shell", Tag: "Jewellery Bead Stock", Characteristics: [new("Colour", null, 1), new("Fine Colour", null, 1)]) { Compatibility = new("190 grams of shell commodity", ["tag Jewellery Bead Stock", "characteristic Colour from $i1", "characteristic Fine Colour from $i1"]), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition GemcraftCutShellJewellerySettingStock() => new()
	{
		Metadata = new("Gemcraft/cut shell jewellery setting stock", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Antiquity.cs:140", []),
		RequiredAnyEra = ["antiquity"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "cut shell jewellery setting stock",
		Category = "Gemcraft",
		Blurb = "cut and polish shell into jewellery settings",
		Action = "cut shell jewellery setting stock",
		ActiveCraftItemSdesc = "an in-progress and polish shell jewellery settings craft",
		Gate = CatalogueRecipeGates.AncientJewelleryCraftingGemcraft15,
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
			new(25, "$0 inspect|inspects $i1 and select|selects pieces fit for jewellery work.", "$0 inspect|inspects $i1 and select|selects pieces fit for jewellery work.", ExertionLevel.Stasis, 0),
			new(35, "$0 work|works the material into small, regular stock, using $t1 and $t2.", "$0 work|works the material into small, regular stock, using $t1 and $t2.", ExertionLevel.Stasis, 0),
			new(25, "$0 finish|finishes the jewellery stock and set|sets aside $p1.", "$0 finish|finishes the stock, but the result is not worth keeping.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 240.0, Material: "shell", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Specific, Characteristics: [new("Colour", null), new("Fine Colour", null)]) { Compatibility = new("240 grams of shell", ["characteristic Colour any", "characteristic Fine Colour any"]), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Bow Drill", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Bow Drill tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Polishing Stone", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Polishing Stone tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueCommodityRecipeProduct(Grams: 170.0, Material: "shell", Tag: "Jewellery Setting Stock", Characteristics: [new("Colour", null, 1), new("Fine Colour", null, 1)]) { Compatibility = new("170 grams of shell commodity", ["tag Jewellery Setting Stock", "characteristic Colour from $i1", "characteristic Fine Colour from $i1"]), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition GemcraftDrillTurquoiseJewelleryBeadStock() => new()
	{
		Metadata = new("Gemcraft/drill turquoise jewellery bead stock", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Antiquity.cs:140", []),
		RequiredAnyEra = ["antiquity"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "drill turquoise jewellery bead stock",
		Category = "Gemcraft",
		Blurb = "drill and polish turquoise into jewellery beads",
		Action = "drill turquoise jewellery bead stock",
		ActiveCraftItemSdesc = "an in-progress drill and polish turquoise jewellery beads craft",
		Gate = CatalogueRecipeGates.AncientJewelleryCraftingGemcraft15,
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
			new(25, "$0 inspect|inspects $i1 and select|selects pieces fit for jewellery work.", "$0 inspect|inspects $i1 and select|selects pieces fit for jewellery work.", ExertionLevel.Stasis, 0),
			new(35, "$0 work|works the material into small, regular stock, using $t1 and $t2.", "$0 work|works the material into small, regular stock, using $t1 and $t2.", ExertionLevel.Stasis, 0),
			new(25, "$0 finish|finishes the jewellery stock and set|sets aside $p1.", "$0 finish|finishes the stock, but the result is not worth keeping.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 260.0, Material: "turquoise", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Specific, Characteristics: [new("Colour", null), new("Fine Colour", null)]) { Compatibility = new("260 grams of turquoise", ["characteristic Colour any", "characteristic Fine Colour any"]), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Bow Drill", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Bow Drill tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Polishing Stone", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Polishing Stone tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueCommodityRecipeProduct(Grams: 190.0, Material: "turquoise", Tag: "Jewellery Bead Stock", Characteristics: [new("Colour", null, 1), new("Fine Colour", null, 1)]) { Compatibility = new("190 grams of turquoise commodity", ["tag Jewellery Bead Stock", "characteristic Colour from $i1", "characteristic Fine Colour from $i1"]), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};
}
