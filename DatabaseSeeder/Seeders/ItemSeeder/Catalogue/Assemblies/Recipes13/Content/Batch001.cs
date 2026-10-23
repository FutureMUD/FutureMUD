#nullable enable

using MudSharp.Body;
using MudSharp.GameItems.Inventory.Plans;
using MudSharp.RPG.Checks;

namespace DatabaseSeeder.Seeders;

public static partial class Recipes13Stock
{
	public static CatalogueRecipeDefinition CarpentryAndJoineryBuildCheckerWroughtIronBarredBarrier() => new()
	{
		Metadata = new("Carpentry and joinery/build checker wrought iron barred barrier", "crafts", ["earlymodern", "renaissance"], ["item:renaissance_door_common_barrier_110"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "build checker wrought iron barred barrier",
		Category = "Carpentry and joinery",
		Blurb = "build a checker wrought iron barred barrier",
		Action = "building a door, gate, shutter, screen, grille, or barrier",
		ActiveCraftItemSdesc = "an in-progress checker wrought iron barred barrier craft",
		Gate = CatalogueRecipeGates.RenaissanceAndEarlyModernJoineryCarpentry15,
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
			new(35, "$0 mark|marks and prepare|prepares $i1 with $t2, laying out the fitted edges.", "$0 mark|marks and prepare|prepares $i1 with $t2, laying out the fitted edges.", ExertionLevel.Stasis, 0),
			new(45, "$0 shape|shapes the material and secure|secures its fitted parts with $t1.", "$0 shape|shapes the material and secure|secures its fitted parts with $t1.", ExertionLevel.Stasis, 0),
			new(35, "$0 inspect|inspects the finished work and set|sets aside $p1.", "$0 find|finds the work has failed to hold together and set|sets the material aside.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 43296.0, Material: "wrought iron", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("43296 grams of wrought iron", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Saw", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Saw tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_door_common_barrier_110", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a checker wrought iron barred barrier (@renaissance_door_common_barrier_110)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition CarpentryAndJoineryBuildCloudBronzeBarredBarrier() => new()
	{
		Metadata = new("Carpentry and joinery/build cloud bronze barred barrier", "crafts", ["earlymodern", "renaissance"], ["item:renaissance_door_common_barrier_12"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "build cloud bronze barred barrier",
		Category = "Carpentry and joinery",
		Blurb = "build a cloud bronze barred barrier",
		Action = "building a door, gate, shutter, screen, grille, or barrier",
		ActiveCraftItemSdesc = "an in-progress cloud bronze barred barrier craft",
		Gate = CatalogueRecipeGates.RenaissanceAndEarlyModernJoineryCarpentry15,
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
			new(35, "$0 mark|marks and prepare|prepares $i1 with $t2, laying out the fitted edges.", "$0 mark|marks and prepare|prepares $i1 with $t2, laying out the fitted edges.", ExertionLevel.Stasis, 0),
			new(45, "$0 shape|shapes the material and secure|secures its fitted parts with $t1.", "$0 shape|shapes the material and secure|secures its fitted parts with $t1.", ExertionLevel.Stasis, 0),
			new(35, "$0 inspect|inspects the finished work and set|sets aside $p1.", "$0 find|finds the work has failed to hold together and set|sets the material aside.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 38544.0, Material: "bronze", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("38544 grams of bronze", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Saw", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Saw tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_door_common_barrier_12", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a cloud bronze barred barrier (@renaissance_door_common_barrier_12)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition CarpentryAndJoineryBuildCloudBrassBarredBarrier() => new()
	{
		Metadata = new("Carpentry and joinery/build cloud brass barred barrier", "crafts", ["earlymodern", "renaissance"], ["item:renaissance_door_common_barrier_124"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "build cloud brass barred barrier",
		Category = "Carpentry and joinery",
		Blurb = "build a cloud brass barred barrier",
		Action = "building a door, gate, shutter, screen, grille, or barrier",
		ActiveCraftItemSdesc = "an in-progress cloud brass barred barrier craft",
		Gate = CatalogueRecipeGates.RenaissanceAndEarlyModernJoineryCarpentry15,
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
			new(35, "$0 mark|marks and prepare|prepares $i1 with $t2, laying out the fitted edges.", "$0 mark|marks and prepare|prepares $i1 with $t2, laying out the fitted edges.", ExertionLevel.Stasis, 0),
			new(45, "$0 shape|shapes the material and secure|secures its fitted parts with $t1.", "$0 shape|shapes the material and secure|secures its fitted parts with $t1.", ExertionLevel.Stasis, 0),
			new(35, "$0 inspect|inspects the finished work and set|sets aside $p1.", "$0 find|finds the work has failed to hold together and set|sets the material aside.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 35376.0, Material: "brass", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("35376 grams of brass", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Saw", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Saw tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_door_common_barrier_124", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a cloud brass barred barrier (@renaissance_door_common_barrier_124)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition CarpentryAndJoineryBuildStarBronzeBarredBarrier() => new()
	{
		Metadata = new("Carpentry and joinery/build star bronze barred barrier", "crafts", ["earlymodern", "renaissance"], ["item:renaissance_door_common_barrier_138"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "build star bronze barred barrier",
		Category = "Carpentry and joinery",
		Blurb = "build a star bronze barred barrier",
		Action = "building a door, gate, shutter, screen, grille, or barrier",
		ActiveCraftItemSdesc = "an in-progress star bronze barred barrier craft",
		Gate = CatalogueRecipeGates.RenaissanceAndEarlyModernJoineryCarpentry15,
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
			new(35, "$0 mark|marks and prepare|prepares $i1 with $t2, laying out the fitted edges.", "$0 mark|marks and prepare|prepares $i1 with $t2, laying out the fitted edges.", ExertionLevel.Stasis, 0),
			new(45, "$0 shape|shapes the material and secure|secures its fitted parts with $t1.", "$0 shape|shapes the material and secure|secures its fitted parts with $t1.", ExertionLevel.Stasis, 0),
			new(35, "$0 inspect|inspects the finished work and set|sets aside $p1.", "$0 find|finds the work has failed to hold together and set|sets the material aside.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 27456.0, Material: "bronze", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("27456 grams of bronze", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Saw", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Saw tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_door_common_barrier_138", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a star bronze barred barrier (@renaissance_door_common_barrier_138)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition CarpentryAndJoineryBuildStarWroughtIronBarredBarrier() => new()
	{
		Metadata = new("Carpentry and joinery/build star wrought iron barred barrier", "crafts", ["earlymodern", "renaissance"], ["item:renaissance_door_common_barrier_26"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "build star wrought iron barred barrier",
		Category = "Carpentry and joinery",
		Blurb = "build a star wrought iron barred barrier",
		Action = "building a door, gate, shutter, screen, grille, or barrier",
		ActiveCraftItemSdesc = "an in-progress star wrought iron barred barrier craft",
		Gate = CatalogueRecipeGates.RenaissanceAndEarlyModernJoineryCarpentry15,
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
			new(35, "$0 mark|marks and prepare|prepares $i1 with $t2, laying out the fitted edges.", "$0 mark|marks and prepare|prepares $i1 with $t2, laying out the fitted edges.", ExertionLevel.Stasis, 0),
			new(45, "$0 shape|shapes the material and secure|secures its fitted parts with $t1.", "$0 shape|shapes the material and secure|secures its fitted parts with $t1.", ExertionLevel.Stasis, 0),
			new(35, "$0 inspect|inspects the finished work and set|sets aside $p1.", "$0 find|finds the work has failed to hold together and set|sets the material aside.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 30624.0, Material: "wrought iron", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("30624 grams of wrought iron", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Saw", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Saw tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_door_common_barrier_26", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a star wrought iron barred barrier (@renaissance_door_common_barrier_26)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition CarpentryAndJoineryBuildSunburstBrassBarredBarrier() => new()
	{
		Metadata = new("Carpentry and joinery/build sunburst brass barred barrier", "crafts", ["earlymodern", "renaissance"], ["item:renaissance_door_common_barrier_40"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "build sunburst brass barred barrier",
		Category = "Carpentry and joinery",
		Blurb = "build a sunburst brass barred barrier",
		Action = "building a door, gate, shutter, screen, grille, or barrier",
		ActiveCraftItemSdesc = "an in-progress sunburst brass barred barrier craft",
		Gate = CatalogueRecipeGates.RenaissanceAndEarlyModernJoineryCarpentry15,
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
			new(35, "$0 mark|marks and prepare|prepares $i1 with $t2, laying out the fitted edges.", "$0 mark|marks and prepare|prepares $i1 with $t2, laying out the fitted edges.", ExertionLevel.Stasis, 0),
			new(45, "$0 shape|shapes the material and secure|secures its fitted parts with $t1.", "$0 shape|shapes the material and secure|secures its fitted parts with $t1.", ExertionLevel.Stasis, 0),
			new(35, "$0 inspect|inspects the finished work and set|sets aside $p1.", "$0 find|finds the work has failed to hold together and set|sets the material aside.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 22704.0, Material: "brass", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("22704 grams of brass", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Saw", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Saw tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_door_common_barrier_40", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a sunburst brass barred barrier (@renaissance_door_common_barrier_40)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition CarpentryAndJoineryBuildCheckerBronzeBarredBarrier() => new()
	{
		Metadata = new("Carpentry and joinery/build checker bronze barred barrier", "crafts", ["earlymodern", "renaissance"], ["item:renaissance_door_common_barrier_54"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "build checker bronze barred barrier",
		Category = "Carpentry and joinery",
		Blurb = "build a checker bronze barred barrier",
		Action = "building a door, gate, shutter, screen, grille, or barrier",
		ActiveCraftItemSdesc = "an in-progress checker bronze barred barrier craft",
		Gate = CatalogueRecipeGates.RenaissanceAndEarlyModernJoineryCarpentry15,
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
			new(35, "$0 mark|marks and prepare|prepares $i1 with $t2, laying out the fitted edges.", "$0 mark|marks and prepare|prepares $i1 with $t2, laying out the fitted edges.", ExertionLevel.Stasis, 0),
			new(45, "$0 shape|shapes the material and secure|secures its fitted parts with $t1.", "$0 shape|shapes the material and secure|secures its fitted parts with $t1.", ExertionLevel.Stasis, 0),
			new(35, "$0 inspect|inspects the finished work and set|sets aside $p1.", "$0 find|finds the work has failed to hold together and set|sets the material aside.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 44880.0, Material: "bronze", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("44880 grams of bronze", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Saw", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Saw tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_door_common_barrier_54", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a checker bronze barred barrier (@renaissance_door_common_barrier_54)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition CarpentryAndJoineryBuildCloudWroughtIronBarredBarrier() => new()
	{
		Metadata = new("Carpentry and joinery/build cloud wrought iron barred barrier", "crafts", ["earlymodern", "renaissance"], ["item:renaissance_door_common_barrier_68"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "build cloud wrought iron barred barrier",
		Category = "Carpentry and joinery",
		Blurb = "build a cloud wrought iron barred barrier",
		Action = "building a door, gate, shutter, screen, grille, or barrier",
		ActiveCraftItemSdesc = "an in-progress cloud wrought iron barred barrier craft",
		Gate = CatalogueRecipeGates.RenaissanceAndEarlyModernJoineryCarpentry15,
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
			new(35, "$0 mark|marks and prepare|prepares $i1 with $t2, laying out the fitted edges.", "$0 mark|marks and prepare|prepares $i1 with $t2, laying out the fitted edges.", ExertionLevel.Stasis, 0),
			new(45, "$0 shape|shapes the material and secure|secures its fitted parts with $t1.", "$0 shape|shapes the material and secure|secures its fitted parts with $t1.", ExertionLevel.Stasis, 0),
			new(35, "$0 inspect|inspects the finished work and set|sets aside $p1.", "$0 find|finds the work has failed to hold together and set|sets the material aside.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 36960.0, Material: "wrought iron", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("36960 grams of wrought iron", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Saw", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Saw tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_door_common_barrier_68", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a cloud wrought iron barred barrier (@renaissance_door_common_barrier_68)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition CarpentryAndJoineryBuildStarBrassBarredBarrier() => new()
	{
		Metadata = new("Carpentry and joinery/build star brass barred barrier", "crafts", ["earlymodern", "renaissance"], ["item:renaissance_door_common_barrier_82"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "build star brass barred barrier",
		Category = "Carpentry and joinery",
		Blurb = "build a star brass barred barrier",
		Action = "building a door, gate, shutter, screen, grille, or barrier",
		ActiveCraftItemSdesc = "an in-progress star brass barred barrier craft",
		Gate = CatalogueRecipeGates.RenaissanceAndEarlyModernJoineryCarpentry15,
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
			new(35, "$0 mark|marks and prepare|prepares $i1 with $t2, laying out the fitted edges.", "$0 mark|marks and prepare|prepares $i1 with $t2, laying out the fitted edges.", ExertionLevel.Stasis, 0),
			new(45, "$0 shape|shapes the material and secure|secures its fitted parts with $t1.", "$0 shape|shapes the material and secure|secures its fitted parts with $t1.", ExertionLevel.Stasis, 0),
			new(35, "$0 inspect|inspects the finished work and set|sets aside $p1.", "$0 find|finds the work has failed to hold together and set|sets the material aside.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 29040.0, Material: "brass", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("29040 grams of brass", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Saw", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Saw tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_door_common_barrier_82", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a star brass barred barrier (@renaissance_door_common_barrier_82)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition CarpentryAndJoineryBuildSunburstBronzeBarredBarrier() => new()
	{
		Metadata = new("Carpentry and joinery/build sunburst bronze barred barrier", "crafts", ["earlymodern", "renaissance"], ["item:renaissance_door_common_barrier_96"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "build sunburst bronze barred barrier",
		Category = "Carpentry and joinery",
		Blurb = "build a sunburst bronze barred barrier",
		Action = "building a door, gate, shutter, screen, grille, or barrier",
		ActiveCraftItemSdesc = "an in-progress sunburst bronze barred barrier craft",
		Gate = CatalogueRecipeGates.RenaissanceAndEarlyModernJoineryCarpentry15,
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
			new(35, "$0 mark|marks and prepare|prepares $i1 with $t2, laying out the fitted edges.", "$0 mark|marks and prepare|prepares $i1 with $t2, laying out the fitted edges.", ExertionLevel.Stasis, 0),
			new(45, "$0 shape|shapes the material and secure|secures its fitted parts with $t1.", "$0 shape|shapes the material and secure|secures its fitted parts with $t1.", ExertionLevel.Stasis, 0),
			new(35, "$0 inspect|inspects the finished work and set|sets aside $p1.", "$0 find|finds the work has failed to hold together and set|sets the material aside.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 21120.0, Material: "bronze", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("21120 grams of bronze", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Saw", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Saw tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_door_common_barrier_96", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a sunburst bronze barred barrier (@renaissance_door_common_barrier_96)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition CarpentryAndJoineryBuildStarPineBracedDoor() => new()
	{
		Metadata = new("Carpentry and joinery/build star pine braced door", "crafts", ["earlymodern", "renaissance"], ["item:renaissance_door_common_braced_door_02"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "build star pine braced door",
		Category = "Carpentry and joinery",
		Blurb = "build a star pine braced door",
		Action = "building a door, gate, shutter, screen, grille, or barrier",
		ActiveCraftItemSdesc = "an in-progress star pine braced door craft",
		Gate = CatalogueRecipeGates.RenaissanceAndEarlyModernJoineryCarpentry15,
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
			new(35, "$0 mark|marks and prepare|prepares $i1 with $t2, laying out the fitted edges.", "$0 mark|marks and prepare|prepares $i1 with $t2, laying out the fitted edges.", ExertionLevel.Stasis, 0),
			new(45, "$0 shape|shapes the material and secure|secures its fitted parts with $t1.", "$0 shape|shapes the material and secure|secures its fitted parts with $t1.", ExertionLevel.Stasis, 0),
			new(35, "$0 inspect|inspects the finished work and set|sets aside $p1.", "$0 find|finds the work has failed to hold together and set|sets the material aside.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 22704.0, Material: "pine", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("22704 grams of pine", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Saw", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Saw tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_door_common_braced_door_02", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a star pine braced door (@renaissance_door_common_braced_door_02)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition CarpentryAndJoineryBuildCloudWoodBracedDoorPattern2() => new()
	{
		Metadata = new("Carpentry and joinery/build cloud wood braced door pattern 2", "crafts", ["earlymodern", "renaissance"], ["item:renaissance_door_common_braced_door_100"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "build cloud wood braced door pattern 2",
		Category = "Carpentry and joinery",
		Blurb = "build a cloud wood braced door",
		Action = "building a door, gate, shutter, screen, grille, or barrier",
		ActiveCraftItemSdesc = "an in-progress cloud wood braced door craft",
		Gate = CatalogueRecipeGates.RenaissanceAndEarlyModernJoineryCarpentry15,
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
			new(35, "$0 mark|marks and prepare|prepares $i1 with $t2, laying out the fitted edges.", "$0 mark|marks and prepare|prepares $i1 with $t2, laying out the fitted edges.", ExertionLevel.Stasis, 0),
			new(45, "$0 shape|shapes the material and secure|secures its fitted parts with $t1.", "$0 shape|shapes the material and secure|secures its fitted parts with $t1.", ExertionLevel.Stasis, 0),
			new(35, "$0 inspect|inspects the finished work and set|sets aside $p1.", "$0 find|finds the work has failed to hold together and set|sets the material aside.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 27456.0, Material: "wood", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("27456 grams of wood", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Saw", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Saw tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_door_common_braced_door_100", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a cloud wood braced door (@renaissance_door_common_braced_door_100)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition CarpentryAndJoineryBuildStarTeakBracedDoor() => new()
	{
		Metadata = new("Carpentry and joinery/build star teak braced door", "crafts", ["earlymodern", "renaissance"], ["item:renaissance_door_common_braced_door_114"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "build star teak braced door",
		Category = "Carpentry and joinery",
		Blurb = "build a star teak braced door",
		Action = "building a door, gate, shutter, screen, grille, or barrier",
		ActiveCraftItemSdesc = "an in-progress star teak braced door craft",
		Gate = CatalogueRecipeGates.RenaissanceAndEarlyModernJoineryCarpentry15,
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
			new(35, "$0 mark|marks and prepare|prepares $i1 with $t2, laying out the fitted edges.", "$0 mark|marks and prepare|prepares $i1 with $t2, laying out the fitted edges.", ExertionLevel.Stasis, 0),
			new(45, "$0 shape|shapes the material and secure|secures its fitted parts with $t1.", "$0 shape|shapes the material and secure|secures its fitted parts with $t1.", ExertionLevel.Stasis, 0),
			new(35, "$0 inspect|inspects the finished work and set|sets aside $p1.", "$0 find|finds the work has failed to hold together and set|sets the material aside.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 49632.0, Material: "teak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("49632 grams of teak", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Saw", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Saw tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_door_common_braced_door_114", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a star teak braced door (@renaissance_door_common_braced_door_114)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition CarpentryAndJoineryBuildSunburstCedarBracedDoor() => new()
	{
		Metadata = new("Carpentry and joinery/build sunburst cedar braced door", "crafts", ["earlymodern", "renaissance"], ["item:renaissance_door_common_braced_door_128"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "build sunburst cedar braced door",
		Category = "Carpentry and joinery",
		Blurb = "build a sunburst cedar braced door",
		Action = "building a door, gate, shutter, screen, grille, or barrier",
		ActiveCraftItemSdesc = "an in-progress sunburst cedar braced door craft",
		Gate = CatalogueRecipeGates.RenaissanceAndEarlyModernJoineryCarpentry15,
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
			new(35, "$0 mark|marks and prepare|prepares $i1 with $t2, laying out the fitted edges.", "$0 mark|marks and prepare|prepares $i1 with $t2, laying out the fitted edges.", ExertionLevel.Stasis, 0),
			new(45, "$0 shape|shapes the material and secure|secures its fitted parts with $t1.", "$0 shape|shapes the material and secure|secures its fitted parts with $t1.", ExertionLevel.Stasis, 0),
			new(35, "$0 inspect|inspects the finished work and set|sets aside $p1.", "$0 find|finds the work has failed to hold together and set|sets the material aside.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 41712.0, Material: "cedar", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("41712 grams of cedar", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Saw", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Saw tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_door_common_braced_door_128", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a sunburst cedar braced door (@renaissance_door_common_braced_door_128)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition CarpentryAndJoineryBuildCheckerPineBracedDoorPattern3() => new()
	{
		Metadata = new("Carpentry and joinery/build checker pine braced door pattern 3", "crafts", ["earlymodern", "renaissance"], ["item:renaissance_door_common_braced_door_142"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "build checker pine braced door pattern 3",
		Category = "Carpentry and joinery",
		Blurb = "build a checker pine braced door",
		Action = "building a door, gate, shutter, screen, grille, or barrier",
		ActiveCraftItemSdesc = "an in-progress checker pine braced door craft",
		Gate = CatalogueRecipeGates.RenaissanceAndEarlyModernJoineryCarpentry15,
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
			new(35, "$0 mark|marks and prepare|prepares $i1 with $t2, laying out the fitted edges.", "$0 mark|marks and prepare|prepares $i1 with $t2, laying out the fitted edges.", ExertionLevel.Stasis, 0),
			new(45, "$0 shape|shapes the material and secure|secures its fitted parts with $t1.", "$0 shape|shapes the material and secure|secures its fitted parts with $t1.", ExertionLevel.Stasis, 0),
			new(35, "$0 inspect|inspects the finished work and set|sets aside $p1.", "$0 find|finds the work has failed to hold together and set|sets the material aside.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 33792.0, Material: "pine", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("33792 grams of pine", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Saw", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Saw tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_door_common_braced_door_142", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a checker pine braced door (@renaissance_door_common_braced_door_142)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition CarpentryAndJoineryBuildSunburstOakBracedDoor() => new()
	{
		Metadata = new("Carpentry and joinery/build sunburst oak braced door", "crafts", ["earlymodern", "renaissance"], ["item:renaissance_door_common_braced_door_16"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "build sunburst oak braced door",
		Category = "Carpentry and joinery",
		Blurb = "build a sunburst oak braced door",
		Action = "building a door, gate, shutter, screen, grille, or barrier",
		ActiveCraftItemSdesc = "an in-progress sunburst oak braced door craft",
		Gate = CatalogueRecipeGates.RenaissanceAndEarlyModernJoineryCarpentry15,
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
			new(35, "$0 mark|marks and prepare|prepares $i1 with $t2, laying out the fitted edges.", "$0 mark|marks and prepare|prepares $i1 with $t2, laying out the fitted edges.", ExertionLevel.Stasis, 0),
			new(45, "$0 shape|shapes the material and secure|secures its fitted parts with $t1.", "$0 shape|shapes the material and secure|secures its fitted parts with $t1.", ExertionLevel.Stasis, 0),
			new(35, "$0 inspect|inspects the finished work and set|sets aside $p1.", "$0 find|finds the work has failed to hold together and set|sets the material aside.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 44880.0, Material: "oak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("44880 grams of oak", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Saw", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Saw tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_door_common_braced_door_16", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a sunburst oak braced door (@renaissance_door_common_braced_door_16)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition CarpentryAndJoineryBuildCheckerWoodBracedDoor() => new()
	{
		Metadata = new("Carpentry and joinery/build checker wood braced door", "crafts", ["earlymodern", "renaissance"], ["item:renaissance_door_common_braced_door_30"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "build checker wood braced door",
		Category = "Carpentry and joinery",
		Blurb = "build a checker wood braced door",
		Action = "building a door, gate, shutter, screen, grille, or barrier",
		ActiveCraftItemSdesc = "an in-progress checker wood braced door craft",
		Gate = CatalogueRecipeGates.RenaissanceAndEarlyModernJoineryCarpentry15,
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
			new(35, "$0 mark|marks and prepare|prepares $i1 with $t2, laying out the fitted edges.", "$0 mark|marks and prepare|prepares $i1 with $t2, laying out the fitted edges.", ExertionLevel.Stasis, 0),
			new(45, "$0 shape|shapes the material and secure|secures its fitted parts with $t1.", "$0 shape|shapes the material and secure|secures its fitted parts with $t1.", ExertionLevel.Stasis, 0),
			new(35, "$0 inspect|inspects the finished work and set|sets aside $p1.", "$0 find|finds the work has failed to hold together and set|sets the material aside.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 36960.0, Material: "wood", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("36960 grams of wood", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Saw", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Saw tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_door_common_braced_door_30", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a checker wood braced door (@renaissance_door_common_braced_door_30)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition CarpentryAndJoineryBuildCloudTeakBracedDoor() => new()
	{
		Metadata = new("Carpentry and joinery/build cloud teak braced door", "crafts", ["earlymodern", "renaissance"], ["item:renaissance_door_common_braced_door_44"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "build cloud teak braced door",
		Category = "Carpentry and joinery",
		Blurb = "build a cloud teak braced door",
		Action = "building a door, gate, shutter, screen, grille, or barrier",
		ActiveCraftItemSdesc = "an in-progress cloud teak braced door craft",
		Gate = CatalogueRecipeGates.RenaissanceAndEarlyModernJoineryCarpentry15,
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
			new(35, "$0 mark|marks and prepare|prepares $i1 with $t2, laying out the fitted edges.", "$0 mark|marks and prepare|prepares $i1 with $t2, laying out the fitted edges.", ExertionLevel.Stasis, 0),
			new(45, "$0 shape|shapes the material and secure|secures its fitted parts with $t1.", "$0 shape|shapes the material and secure|secures its fitted parts with $t1.", ExertionLevel.Stasis, 0),
			new(35, "$0 inspect|inspects the finished work and set|sets aside $p1.", "$0 find|finds the work has failed to hold together and set|sets the material aside.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 29040.0, Material: "teak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("29040 grams of teak", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Saw", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Saw tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_door_common_braced_door_44", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a cloud teak braced door (@renaissance_door_common_braced_door_44)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition CarpentryAndJoineryBuildStarCedarBracedDoorPattern3() => new()
	{
		Metadata = new("Carpentry and joinery/build star cedar braced door pattern 3", "crafts", ["earlymodern", "renaissance"], ["item:renaissance_door_common_braced_door_58"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "build star cedar braced door pattern 3",
		Category = "Carpentry and joinery",
		Blurb = "build a star cedar braced door",
		Action = "building a door, gate, shutter, screen, grille, or barrier",
		ActiveCraftItemSdesc = "an in-progress star cedar braced door craft",
		Gate = CatalogueRecipeGates.RenaissanceAndEarlyModernJoineryCarpentry15,
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
			new(35, "$0 mark|marks and prepare|prepares $i1 with $t2, laying out the fitted edges.", "$0 mark|marks and prepare|prepares $i1 with $t2, laying out the fitted edges.", ExertionLevel.Stasis, 0),
			new(45, "$0 shape|shapes the material and secure|secures its fitted parts with $t1.", "$0 shape|shapes the material and secure|secures its fitted parts with $t1.", ExertionLevel.Stasis, 0),
			new(35, "$0 inspect|inspects the finished work and set|sets aside $p1.", "$0 find|finds the work has failed to hold together and set|sets the material aside.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 21120.0, Material: "cedar", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("21120 grams of cedar", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Saw", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Saw tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_door_common_braced_door_58", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a star cedar braced door (@renaissance_door_common_braced_door_58)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition CarpentryAndJoineryBuildSunburstPineBracedDoorPattern2() => new()
	{
		Metadata = new("Carpentry and joinery/build sunburst pine braced door pattern 2", "crafts", ["earlymodern", "renaissance"], ["item:renaissance_door_common_braced_door_72"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "build sunburst pine braced door pattern 2",
		Category = "Carpentry and joinery",
		Blurb = "build a sunburst pine braced door",
		Action = "building a door, gate, shutter, screen, grille, or barrier",
		ActiveCraftItemSdesc = "an in-progress sunburst pine braced door craft",
		Gate = CatalogueRecipeGates.RenaissanceAndEarlyModernJoineryCarpentry15,
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
			new(35, "$0 mark|marks and prepare|prepares $i1 with $t2, laying out the fitted edges.", "$0 mark|marks and prepare|prepares $i1 with $t2, laying out the fitted edges.", ExertionLevel.Stasis, 0),
			new(45, "$0 shape|shapes the material and secure|secures its fitted parts with $t1.", "$0 shape|shapes the material and secure|secures its fitted parts with $t1.", ExertionLevel.Stasis, 0),
			new(35, "$0 inspect|inspects the finished work and set|sets aside $p1.", "$0 find|finds the work has failed to hold together and set|sets the material aside.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 43296.0, Material: "pine", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("43296 grams of pine", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Saw", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Saw tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_door_common_braced_door_72", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a sunburst pine braced door (@renaissance_door_common_braced_door_72)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition CarpentryAndJoineryBuildCheckerOakBracedDoor() => new()
	{
		Metadata = new("Carpentry and joinery/build checker oak braced door", "crafts", ["earlymodern", "renaissance"], ["item:renaissance_door_common_braced_door_86"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "build checker oak braced door",
		Category = "Carpentry and joinery",
		Blurb = "build a checker oak braced door",
		Action = "building a door, gate, shutter, screen, grille, or barrier",
		ActiveCraftItemSdesc = "an in-progress checker oak braced door craft",
		Gate = CatalogueRecipeGates.RenaissanceAndEarlyModernJoineryCarpentry15,
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
			new(35, "$0 mark|marks and prepare|prepares $i1 with $t2, laying out the fitted edges.", "$0 mark|marks and prepare|prepares $i1 with $t2, laying out the fitted edges.", ExertionLevel.Stasis, 0),
			new(45, "$0 shape|shapes the material and secure|secures its fitted parts with $t1.", "$0 shape|shapes the material and secure|secures its fitted parts with $t1.", ExertionLevel.Stasis, 0),
			new(35, "$0 inspect|inspects the finished work and set|sets aside $p1.", "$0 find|finds the work has failed to hold together and set|sets the material aside.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 35376.0, Material: "oak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("35376 grams of oak", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Saw", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Saw tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_door_common_braced_door_86", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a checker oak braced door (@renaissance_door_common_braced_door_86)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition CarpentryAndJoineryBuildSunburstTeakCourtyardGate() => new()
	{
		Metadata = new("Carpentry and joinery/build sunburst teak courtyard gate", "crafts", ["earlymodern", "renaissance"], ["item:renaissance_door_common_courtyard_gate_08"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "build sunburst teak courtyard gate",
		Category = "Carpentry and joinery",
		Blurb = "build a sunburst teak courtyard gate",
		Action = "building a door, gate, shutter, screen, grille, or barrier",
		ActiveCraftItemSdesc = "an in-progress sunburst teak courtyard gate craft",
		Gate = CatalogueRecipeGates.RenaissanceAndEarlyModernJoineryCarpentry15,
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
			new(35, "$0 mark|marks and prepare|prepares $i1 with $t2, laying out the fitted edges.", "$0 mark|marks and prepare|prepares $i1 with $t2, laying out the fitted edges.", ExertionLevel.Stasis, 0),
			new(45, "$0 shape|shapes the material and secure|secures its fitted parts with $t1.", "$0 shape|shapes the material and secure|secures its fitted parts with $t1.", ExertionLevel.Stasis, 0),
			new(35, "$0 inspect|inspects the finished work and set|sets aside $p1.", "$0 find|finds the work has failed to hold together and set|sets the material aside.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 32208.0, Material: "teak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("32208 grams of teak", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Saw", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Saw tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_door_common_courtyard_gate_08", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a sunburst teak courtyard gate (@renaissance_door_common_courtyard_gate_08)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition CarpentryAndJoineryBuildStarOakCourtyardGatePattern2() => new()
	{
		Metadata = new("Carpentry and joinery/build star oak courtyard gate pattern 2", "crafts", ["earlymodern", "renaissance"], ["item:renaissance_door_common_courtyard_gate_106"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "build star oak courtyard gate pattern 2",
		Category = "Carpentry and joinery",
		Blurb = "build a star oak courtyard gate",
		Action = "building a door, gate, shutter, screen, grille, or barrier",
		ActiveCraftItemSdesc = "an in-progress star oak courtyard gate craft",
		Gate = CatalogueRecipeGates.RenaissanceAndEarlyModernJoineryCarpentry15,
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
			new(35, "$0 mark|marks and prepare|prepares $i1 with $t2, laying out the fitted edges.", "$0 mark|marks and prepare|prepares $i1 with $t2, laying out the fitted edges.", ExertionLevel.Stasis, 0),
			new(45, "$0 shape|shapes the material and secure|secures its fitted parts with $t1.", "$0 shape|shapes the material and secure|secures its fitted parts with $t1.", ExertionLevel.Stasis, 0),
			new(35, "$0 inspect|inspects the finished work and set|sets aside $p1.", "$0 find|finds the work has failed to hold together and set|sets the material aside.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 36960.0, Material: "oak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("36960 grams of oak", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Saw", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Saw tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_door_common_courtyard_gate_106", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a star oak courtyard gate (@renaissance_door_common_courtyard_gate_106)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition CarpentryAndJoineryBuildSunburstBronzeCourtyardGate() => new()
	{
		Metadata = new("Carpentry and joinery/build sunburst bronze courtyard gate", "crafts", ["earlymodern", "renaissance"], ["item:renaissance_door_common_courtyard_gate_120"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "build sunburst bronze courtyard gate",
		Category = "Carpentry and joinery",
		Blurb = "build a sunburst bronze courtyard gate",
		Action = "building a door, gate, shutter, screen, grille, or barrier",
		ActiveCraftItemSdesc = "an in-progress sunburst bronze courtyard gate craft",
		Gate = CatalogueRecipeGates.RenaissanceAndEarlyModernJoineryCarpentry15,
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
			new(35, "$0 mark|marks and prepare|prepares $i1 with $t2, laying out the fitted edges.", "$0 mark|marks and prepare|prepares $i1 with $t2, laying out the fitted edges.", ExertionLevel.Stasis, 0),
			new(45, "$0 shape|shapes the material and secure|secures its fitted parts with $t1.", "$0 shape|shapes the material and secure|secures its fitted parts with $t1.", ExertionLevel.Stasis, 0),
			new(35, "$0 inspect|inspects the finished work and set|sets aside $p1.", "$0 find|finds the work has failed to hold together and set|sets the material aside.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 29040.0, Material: "bronze", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("29040 grams of bronze", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Saw", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Saw tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_door_common_courtyard_gate_120", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a sunburst bronze courtyard gate (@renaissance_door_common_courtyard_gate_120)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition CarpentryAndJoineryBuildCheckerWroughtIronCourtyardGate() => new()
	{
		Metadata = new("Carpentry and joinery/build checker wrought iron courtyard gate", "crafts", ["earlymodern", "renaissance"], ["item:renaissance_door_common_courtyard_gate_134"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "build checker wrought iron courtyard gate",
		Category = "Carpentry and joinery",
		Blurb = "build a checker wrought iron courtyard gate",
		Action = "building a door, gate, shutter, screen, grille, or barrier",
		ActiveCraftItemSdesc = "an in-progress checker wrought iron courtyard gate craft",
		Gate = CatalogueRecipeGates.RenaissanceAndEarlyModernJoineryCarpentry15,
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
			new(35, "$0 mark|marks and prepare|prepares $i1 with $t2, laying out the fitted edges.", "$0 mark|marks and prepare|prepares $i1 with $t2, laying out the fitted edges.", ExertionLevel.Stasis, 0),
			new(45, "$0 shape|shapes the material and secure|secures its fitted parts with $t1.", "$0 shape|shapes the material and secure|secures its fitted parts with $t1.", ExertionLevel.Stasis, 0),
			new(35, "$0 inspect|inspects the finished work and set|sets aside $p1.", "$0 find|finds the work has failed to hold together and set|sets the material aside.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 21120.0, Material: "wrought iron", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("21120 grams of wrought iron", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Saw", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Saw tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_door_common_courtyard_gate_134", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a checker wrought iron courtyard gate (@renaissance_door_common_courtyard_gate_134)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition CarpentryAndJoineryBuildCheckerCedarCourtyardGate() => new()
	{
		Metadata = new("Carpentry and joinery/build checker cedar courtyard gate", "crafts", ["earlymodern", "renaissance"], ["item:renaissance_door_common_courtyard_gate_22"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "build checker cedar courtyard gate",
		Category = "Carpentry and joinery",
		Blurb = "build a checker cedar courtyard gate",
		Action = "building a door, gate, shutter, screen, grille, or barrier",
		ActiveCraftItemSdesc = "an in-progress checker cedar courtyard gate craft",
		Gate = CatalogueRecipeGates.RenaissanceAndEarlyModernJoineryCarpentry15,
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
			new(35, "$0 mark|marks and prepare|prepares $i1 with $t2, laying out the fitted edges.", "$0 mark|marks and prepare|prepares $i1 with $t2, laying out the fitted edges.", ExertionLevel.Stasis, 0),
			new(45, "$0 shape|shapes the material and secure|secures its fitted parts with $t1.", "$0 shape|shapes the material and secure|secures its fitted parts with $t1.", ExertionLevel.Stasis, 0),
			new(35, "$0 inspect|inspects the finished work and set|sets aside $p1.", "$0 find|finds the work has failed to hold together and set|sets the material aside.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 24288.0, Material: "cedar", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("24288 grams of cedar", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Saw", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Saw tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_door_common_courtyard_gate_22", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a checker cedar courtyard gate (@renaissance_door_common_courtyard_gate_22)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition CarpentryAndJoineryBuildCloudOakCourtyardGate() => new()
	{
		Metadata = new("Carpentry and joinery/build cloud oak courtyard gate", "crafts", ["earlymodern", "renaissance"], ["item:renaissance_door_common_courtyard_gate_36"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "build cloud oak courtyard gate",
		Category = "Carpentry and joinery",
		Blurb = "build a cloud oak courtyard gate",
		Action = "building a door, gate, shutter, screen, grille, or barrier",
		ActiveCraftItemSdesc = "an in-progress cloud oak courtyard gate craft",
		Gate = CatalogueRecipeGates.RenaissanceAndEarlyModernJoineryCarpentry15,
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
			new(35, "$0 mark|marks and prepare|prepares $i1 with $t2, laying out the fitted edges.", "$0 mark|marks and prepare|prepares $i1 with $t2, laying out the fitted edges.", ExertionLevel.Stasis, 0),
			new(45, "$0 shape|shapes the material and secure|secures its fitted parts with $t1.", "$0 shape|shapes the material and secure|secures its fitted parts with $t1.", ExertionLevel.Stasis, 0),
			new(35, "$0 inspect|inspects the finished work and set|sets aside $p1.", "$0 find|finds the work has failed to hold together and set|sets the material aside.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 46464.0, Material: "oak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("46464 grams of oak", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Saw", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Saw tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_door_common_courtyard_gate_36", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a cloud oak courtyard gate (@renaissance_door_common_courtyard_gate_36)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition CarpentryAndJoineryBuildStarBronzeCourtyardGate() => new()
	{
		Metadata = new("Carpentry and joinery/build star bronze courtyard gate", "crafts", ["earlymodern", "renaissance"], ["item:renaissance_door_common_courtyard_gate_50"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "build star bronze courtyard gate",
		Category = "Carpentry and joinery",
		Blurb = "build a star bronze courtyard gate",
		Action = "building a door, gate, shutter, screen, grille, or barrier",
		ActiveCraftItemSdesc = "an in-progress star bronze courtyard gate craft",
		Gate = CatalogueRecipeGates.RenaissanceAndEarlyModernJoineryCarpentry15,
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
			new(35, "$0 mark|marks and prepare|prepares $i1 with $t2, laying out the fitted edges.", "$0 mark|marks and prepare|prepares $i1 with $t2, laying out the fitted edges.", ExertionLevel.Stasis, 0),
			new(45, "$0 shape|shapes the material and secure|secures its fitted parts with $t1.", "$0 shape|shapes the material and secure|secures its fitted parts with $t1.", ExertionLevel.Stasis, 0),
			new(35, "$0 inspect|inspects the finished work and set|sets aside $p1.", "$0 find|finds the work has failed to hold together and set|sets the material aside.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 38544.0, Material: "bronze", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("38544 grams of bronze", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Saw", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Saw tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_door_common_courtyard_gate_50", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a star bronze courtyard gate (@renaissance_door_common_courtyard_gate_50)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition CarpentryAndJoineryBuildSunburstWroughtIronCourtyardGatePattern3() => new()
	{
		Metadata = new("Carpentry and joinery/build sunburst wrought iron courtyard gate pattern 3", "crafts", ["earlymodern", "renaissance"], ["item:renaissance_door_common_courtyard_gate_64"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "build sunburst wrought iron courtyard gate pattern 3",
		Category = "Carpentry and joinery",
		Blurb = "build a sunburst wrought iron courtyard gate",
		Action = "building a door, gate, shutter, screen, grille, or barrier",
		ActiveCraftItemSdesc = "an in-progress sunburst wrought iron courtyard gate craft",
		Gate = CatalogueRecipeGates.RenaissanceAndEarlyModernJoineryCarpentry15,
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
			new(35, "$0 mark|marks and prepare|prepares $i1 with $t2, laying out the fitted edges.", "$0 mark|marks and prepare|prepares $i1 with $t2, laying out the fitted edges.", ExertionLevel.Stasis, 0),
			new(45, "$0 shape|shapes the material and secure|secures its fitted parts with $t1.", "$0 shape|shapes the material and secure|secures its fitted parts with $t1.", ExertionLevel.Stasis, 0),
			new(35, "$0 inspect|inspects the finished work and set|sets aside $p1.", "$0 find|finds the work has failed to hold together and set|sets the material aside.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 30624.0, Material: "wrought iron", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("30624 grams of wrought iron", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Saw", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Saw tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_door_common_courtyard_gate_64", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a sunburst wrought iron courtyard gate (@renaissance_door_common_courtyard_gate_64)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition CarpentryAndJoineryBuildCheckerTeakCourtyardGatePattern2() => new()
	{
		Metadata = new("Carpentry and joinery/build checker teak courtyard gate pattern 2", "crafts", ["earlymodern", "renaissance"], ["item:renaissance_door_common_courtyard_gate_78"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "build checker teak courtyard gate pattern 2",
		Category = "Carpentry and joinery",
		Blurb = "build a checker teak courtyard gate",
		Action = "building a door, gate, shutter, screen, grille, or barrier",
		ActiveCraftItemSdesc = "an in-progress checker teak courtyard gate craft",
		Gate = CatalogueRecipeGates.RenaissanceAndEarlyModernJoineryCarpentry15,
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
			new(35, "$0 mark|marks and prepare|prepares $i1 with $t2, laying out the fitted edges.", "$0 mark|marks and prepare|prepares $i1 with $t2, laying out the fitted edges.", ExertionLevel.Stasis, 0),
			new(45, "$0 shape|shapes the material and secure|secures its fitted parts with $t1.", "$0 shape|shapes the material and secure|secures its fitted parts with $t1.", ExertionLevel.Stasis, 0),
			new(35, "$0 inspect|inspects the finished work and set|sets aside $p1.", "$0 find|finds the work has failed to hold together and set|sets the material aside.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 22704.0, Material: "teak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("22704 grams of teak", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Saw", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Saw tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_door_common_courtyard_gate_78", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a checker teak courtyard gate (@renaissance_door_common_courtyard_gate_78)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition CarpentryAndJoineryBuildCloudCedarCourtyardGate() => new()
	{
		Metadata = new("Carpentry and joinery/build cloud cedar courtyard gate", "crafts", ["earlymodern", "renaissance"], ["item:renaissance_door_common_courtyard_gate_92"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "build cloud cedar courtyard gate",
		Category = "Carpentry and joinery",
		Blurb = "build a cloud cedar courtyard gate",
		Action = "building a door, gate, shutter, screen, grille, or barrier",
		ActiveCraftItemSdesc = "an in-progress cloud cedar courtyard gate craft",
		Gate = CatalogueRecipeGates.RenaissanceAndEarlyModernJoineryCarpentry15,
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
			new(35, "$0 mark|marks and prepare|prepares $i1 with $t2, laying out the fitted edges.", "$0 mark|marks and prepare|prepares $i1 with $t2, laying out the fitted edges.", ExertionLevel.Stasis, 0),
			new(45, "$0 shape|shapes the material and secure|secures its fitted parts with $t1.", "$0 shape|shapes the material and secure|secures its fitted parts with $t1.", ExertionLevel.Stasis, 0),
			new(35, "$0 inspect|inspects the finished work and set|sets aside $p1.", "$0 find|finds the work has failed to hold together and set|sets the material aside.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 44880.0, Material: "cedar", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("44880 grams of cedar", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Saw", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Saw tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_door_common_courtyard_gate_92", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a cloud cedar courtyard gate (@renaissance_door_common_courtyard_gate_92)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition LocksmithingForgeKnottedBrassDoorBar() => new()
	{
		Metadata = new("Locksmithing/forge knotted brass door bar", "crafts", ["earlymodern", "renaissance"], ["item:renaissance_door_common_door_bar_158"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "forge knotted brass door bar",
		Category = "Locksmithing",
		Blurb = "forge a knotted brass door bar",
		Action = "forging a loose lock, key, latch, or fitting",
		ActiveCraftItemSdesc = "an in-progress knotted brass door bar craft",
		Gate = CatalogueRecipeGates.RenaissanceAndEarlyModernLocksmithingBlacksmithing15,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 3,
		Interruptable = false,
		Phases = [
			new(25, "$0 heat|heats and prepare|prepares $i1 at the work area with $t1.", "$0 heat|heats and prepare|prepares $i1 at the work area with $t1.", ExertionLevel.Stasis, 0),
			new(35, "$0 work|works the fitting with $t1, checking its accessible moving surfaces, using $t2.", "$0 work|works the fitting with $t1, checking its accessible moving surfaces, using $t2.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the fitting and set|sets aside $p1.", "$0 end|ends the unsuccessful fitting and set|sets the material aside.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 410.0, Material: "brass", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("410 grams of brass", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Anvil", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Anvil tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_door_common_door_bar_158", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a knotted brass door bar (@renaissance_door_common_door_bar_158)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition LocksmithingForgeWaterWaveBrassDoorBarPattern2() => new()
	{
		Metadata = new("Locksmithing/forge water-wave brass door bar pattern 2", "crafts", ["earlymodern", "renaissance"], ["item:renaissance_door_common_door_bar_173"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "forge water-wave brass door bar pattern 2",
		Category = "Locksmithing",
		Blurb = "forge a water-wave brass door bar",
		Action = "forging a loose lock, key, latch, or fitting",
		ActiveCraftItemSdesc = "an in-progress water-wave brass door bar craft",
		Gate = CatalogueRecipeGates.RenaissanceAndEarlyModernLocksmithingBlacksmithing15,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 3,
		Interruptable = false,
		Phases = [
			new(25, "$0 heat|heats and prepare|prepares $i1 at the work area with $t1.", "$0 heat|heats and prepare|prepares $i1 at the work area with $t1.", ExertionLevel.Stasis, 0),
			new(35, "$0 work|works the fitting with $t1, checking its accessible moving surfaces, using $t2.", "$0 work|works the fitting with $t1, checking its accessible moving surfaces, using $t2.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the fitting and set|sets aside $p1.", "$0 end|ends the unsuccessful fitting and set|sets the material aside.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 271.0, Material: "brass", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("271 grams of brass", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Anvil", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Anvil tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_door_common_door_bar_173", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a water-wave brass door bar (@renaissance_door_common_door_bar_173)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition LocksmithingForgeSunburstBrassDoorBar() => new()
	{
		Metadata = new("Locksmithing/forge sunburst brass door bar", "crafts", ["earlymodern", "renaissance"], ["item:renaissance_door_common_door_bar_188"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "forge sunburst brass door bar",
		Category = "Locksmithing",
		Blurb = "forge a sunburst brass door bar",
		Action = "forging a loose lock, key, latch, or fitting",
		ActiveCraftItemSdesc = "an in-progress sunburst brass door bar craft",
		Gate = CatalogueRecipeGates.RenaissanceAndEarlyModernLocksmithingBlacksmithing15,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 3,
		Interruptable = false,
		Phases = [
			new(25, "$0 heat|heats and prepare|prepares $i1 at the work area with $t1.", "$0 heat|heats and prepare|prepares $i1 at the work area with $t1.", ExertionLevel.Stasis, 0),
			new(35, "$0 work|works the fitting with $t1, checking its accessible moving surfaces, using $t2.", "$0 work|works the fitting with $t1, checking its accessible moving surfaces, using $t2.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the fitting and set|sets aside $p1.", "$0 end|ends the unsuccessful fitting and set|sets the material aside.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 131.0, Material: "brass", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("131 grams of brass", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Anvil", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Anvil tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_door_common_door_bar_188", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a sunburst brass door bar (@renaissance_door_common_door_bar_188)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition LocksmithingForgeLeafScrollBrassDoorBar() => new()
	{
		Metadata = new("Locksmithing/forge leaf-scroll brass door bar", "crafts", ["earlymodern", "renaissance"], ["item:renaissance_door_common_door_bar_203"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "forge leaf-scroll brass door bar",
		Category = "Locksmithing",
		Blurb = "forge a leaf-scroll brass door bar",
		Action = "forging a loose lock, key, latch, or fitting",
		ActiveCraftItemSdesc = "an in-progress leaf-scroll brass door bar craft",
		Gate = CatalogueRecipeGates.RenaissanceAndEarlyModernLocksmithingBlacksmithing15,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 3,
		Interruptable = false,
		Phases = [
			new(25, "$0 heat|heats and prepare|prepares $i1 at the work area with $t1.", "$0 heat|heats and prepare|prepares $i1 at the work area with $t1.", ExertionLevel.Stasis, 0),
			new(35, "$0 work|works the fitting with $t1, checking its accessible moving surfaces, using $t2.", "$0 work|works the fitting with $t1, checking its accessible moving surfaces, using $t2.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the fitting and set|sets aside $p1.", "$0 end|ends the unsuccessful fitting and set|sets the material aside.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1177.0, Material: "brass", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1177 grams of brass", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Anvil", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Anvil tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_door_common_door_bar_203", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a leaf-scroll brass door bar (@renaissance_door_common_door_bar_203)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition LocksmithingForgeRadiatingBrassDoorBarPattern2() => new()
	{
		Metadata = new("Locksmithing/forge radiating brass door bar pattern 2", "crafts", ["earlymodern", "renaissance"], ["item:renaissance_door_common_door_bar_218"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "forge radiating brass door bar pattern 2",
		Category = "Locksmithing",
		Blurb = "forge a radiating brass door bar",
		Action = "forging a loose lock, key, latch, or fitting",
		ActiveCraftItemSdesc = "an in-progress radiating brass door bar craft",
		Gate = CatalogueRecipeGates.RenaissanceAndEarlyModernLocksmithingBlacksmithing15,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 3,
		Interruptable = false,
		Phases = [
			new(25, "$0 heat|heats and prepare|prepares $i1 at the work area with $t1.", "$0 heat|heats and prepare|prepares $i1 at the work area with $t1.", ExertionLevel.Stasis, 0),
			new(35, "$0 work|works the fitting with $t1, checking its accessible moving surfaces, using $t2.", "$0 work|works the fitting with $t1, checking its accessible moving surfaces, using $t2.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the fitting and set|sets aside $p1.", "$0 end|ends the unsuccessful fitting and set|sets the material aside.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1037.0, Material: "brass", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1037 grams of brass", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Anvil", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Anvil tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_door_common_door_bar_218", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a radiating brass door bar (@renaissance_door_common_door_bar_218)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition LocksmithingForgeBraidedBrassDoorBarPattern2() => new()
	{
		Metadata = new("Locksmithing/forge braided brass door bar pattern 2", "crafts", ["earlymodern", "renaissance"], ["item:renaissance_door_common_door_bar_233"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "forge braided brass door bar pattern 2",
		Category = "Locksmithing",
		Blurb = "forge a braided brass door bar",
		Action = "forging a loose lock, key, latch, or fitting",
		ActiveCraftItemSdesc = "an in-progress braided brass door bar craft",
		Gate = CatalogueRecipeGates.RenaissanceAndEarlyModernLocksmithingBlacksmithing15,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 3,
		Interruptable = false,
		Phases = [
			new(25, "$0 heat|heats and prepare|prepares $i1 at the work area with $t1.", "$0 heat|heats and prepare|prepares $i1 at the work area with $t1.", ExertionLevel.Stasis, 0),
			new(35, "$0 work|works the fitting with $t1, checking its accessible moving surfaces, using $t2.", "$0 work|works the fitting with $t1, checking its accessible moving surfaces, using $t2.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the fitting and set|sets aside $p1.", "$0 end|ends the unsuccessful fitting and set|sets the material aside.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 898.0, Material: "brass", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("898 grams of brass", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Anvil", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Anvil tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_door_common_door_bar_233", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a braided brass door bar (@renaissance_door_common_door_bar_233)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition LocksmithingForgeSunburstWroughtIronDoorLatch() => new()
	{
		Metadata = new("Locksmithing/forge sunburst wrought iron door latch", "crafts", ["earlymodern", "renaissance"], ["item:renaissance_door_common_door_latch_156"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "forge sunburst wrought iron door latch",
		Category = "Locksmithing",
		Blurb = "forge a sunburst wrought iron door latch",
		Action = "forging a loose lock, key, latch, or fitting",
		ActiveCraftItemSdesc = "an in-progress sunburst wrought iron door latch craft",
		Gate = CatalogueRecipeGates.RenaissanceAndEarlyModernLocksmithingBlacksmithing15,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 3,
		Interruptable = false,
		Phases = [
			new(25, "$0 heat|heats and prepare|prepares $i1 at the work area with $t1.", "$0 heat|heats and prepare|prepares $i1 at the work area with $t1.", ExertionLevel.Stasis, 0),
			new(35, "$0 work|works the fitting with $t1, checking its accessible moving surfaces, using $t2.", "$0 work|works the fitting with $t1, checking its accessible moving surfaces, using $t2.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the fitting and set|sets aside $p1.", "$0 end|ends the unsuccessful fitting and set|sets the material aside.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 271.0, Material: "wrought iron", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("271 grams of wrought iron", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Anvil", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Anvil tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_door_common_door_latch_156", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a sunburst wrought iron door latch (@renaissance_door_common_door_latch_156)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition LocksmithingForgeLeafScrollWroughtIronDoorLatchPattern2() => new()
	{
		Metadata = new("Locksmithing/forge leaf-scroll wrought iron door latch pattern 2", "crafts", ["earlymodern", "renaissance"], ["item:renaissance_door_common_door_latch_171"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "forge leaf-scroll wrought iron door latch pattern 2",
		Category = "Locksmithing",
		Blurb = "forge a leaf-scroll wrought iron door latch",
		Action = "forging a loose lock, key, latch, or fitting",
		ActiveCraftItemSdesc = "an in-progress leaf-scroll wrought iron door latch craft",
		Gate = CatalogueRecipeGates.RenaissanceAndEarlyModernLocksmithingBlacksmithing15,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 3,
		Interruptable = false,
		Phases = [
			new(25, "$0 heat|heats and prepare|prepares $i1 at the work area with $t1.", "$0 heat|heats and prepare|prepares $i1 at the work area with $t1.", ExertionLevel.Stasis, 0),
			new(35, "$0 work|works the fitting with $t1, checking its accessible moving surfaces, using $t2.", "$0 work|works the fitting with $t1, checking its accessible moving surfaces, using $t2.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the fitting and set|sets aside $p1.", "$0 end|ends the unsuccessful fitting and set|sets the material aside.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 131.0, Material: "wrought iron", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("131 grams of wrought iron", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Anvil", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Anvil tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_door_common_door_latch_171", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a leaf-scroll wrought iron door latch (@renaissance_door_common_door_latch_171)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition LocksmithingForgeRadiatingWroughtIronDoorLatch() => new()
	{
		Metadata = new("Locksmithing/forge radiating wrought iron door latch", "crafts", ["earlymodern", "renaissance"], ["item:renaissance_door_common_door_latch_186"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "forge radiating wrought iron door latch",
		Category = "Locksmithing",
		Blurb = "forge a radiating wrought iron door latch",
		Action = "forging a loose lock, key, latch, or fitting",
		ActiveCraftItemSdesc = "an in-progress radiating wrought iron door latch craft",
		Gate = CatalogueRecipeGates.RenaissanceAndEarlyModernLocksmithingBlacksmithing15,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 3,
		Interruptable = false,
		Phases = [
			new(25, "$0 heat|heats and prepare|prepares $i1 at the work area with $t1.", "$0 heat|heats and prepare|prepares $i1 at the work area with $t1.", ExertionLevel.Stasis, 0),
			new(35, "$0 work|works the fitting with $t1, checking its accessible moving surfaces, using $t2.", "$0 work|works the fitting with $t1, checking its accessible moving surfaces, using $t2.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the fitting and set|sets aside $p1.", "$0 end|ends the unsuccessful fitting and set|sets the material aside.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1177.0, Material: "wrought iron", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1177 grams of wrought iron", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Anvil", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Anvil tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_door_common_door_latch_186", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a radiating wrought iron door latch (@renaissance_door_common_door_latch_186)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition LocksmithingForgeBraidedWroughtIronDoorLatch() => new()
	{
		Metadata = new("Locksmithing/forge braided wrought iron door latch", "crafts", ["earlymodern", "renaissance"], ["item:renaissance_door_common_door_latch_201"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "forge braided wrought iron door latch",
		Category = "Locksmithing",
		Blurb = "forge a braided wrought iron door latch",
		Action = "forging a loose lock, key, latch, or fitting",
		ActiveCraftItemSdesc = "an in-progress braided wrought iron door latch craft",
		Gate = CatalogueRecipeGates.RenaissanceAndEarlyModernLocksmithingBlacksmithing15,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 3,
		Interruptable = false,
		Phases = [
			new(25, "$0 heat|heats and prepare|prepares $i1 at the work area with $t1.", "$0 heat|heats and prepare|prepares $i1 at the work area with $t1.", ExertionLevel.Stasis, 0),
			new(35, "$0 work|works the fitting with $t1, checking its accessible moving surfaces, using $t2.", "$0 work|works the fitting with $t1, checking its accessible moving surfaces, using $t2.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the fitting and set|sets aside $p1.", "$0 end|ends the unsuccessful fitting and set|sets the material aside.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1037.0, Material: "wrought iron", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1037 grams of wrought iron", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Anvil", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Anvil tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_door_common_door_latch_201", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a braided wrought iron door latch (@renaissance_door_common_door_latch_201)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition LocksmithingForgeCheckerWroughtIronDoorLatchPattern2() => new()
	{
		Metadata = new("Locksmithing/forge checker wrought iron door latch pattern 2", "crafts", ["earlymodern", "renaissance"], ["item:renaissance_door_common_door_latch_216"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "forge checker wrought iron door latch pattern 2",
		Category = "Locksmithing",
		Blurb = "forge a checker wrought iron door latch",
		Action = "forging a loose lock, key, latch, or fitting",
		ActiveCraftItemSdesc = "an in-progress checker wrought iron door latch craft",
		Gate = CatalogueRecipeGates.RenaissanceAndEarlyModernLocksmithingBlacksmithing15,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 3,
		Interruptable = false,
		Phases = [
			new(25, "$0 heat|heats and prepare|prepares $i1 at the work area with $t1.", "$0 heat|heats and prepare|prepares $i1 at the work area with $t1.", ExertionLevel.Stasis, 0),
			new(35, "$0 work|works the fitting with $t1, checking its accessible moving surfaces, using $t2.", "$0 work|works the fitting with $t1, checking its accessible moving surfaces, using $t2.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the fitting and set|sets aside $p1.", "$0 end|ends the unsuccessful fitting and set|sets the material aside.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 898.0, Material: "wrought iron", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("898 grams of wrought iron", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Anvil", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Anvil tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_door_common_door_latch_216", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a checker wrought iron door latch (@renaissance_door_common_door_latch_216)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition LocksmithingForgeRosetteWroughtIronDoorLatchPattern2() => new()
	{
		Metadata = new("Locksmithing/forge rosette wrought iron door latch pattern 2", "crafts", ["earlymodern", "renaissance"], ["item:renaissance_door_common_door_latch_231"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "forge rosette wrought iron door latch pattern 2",
		Category = "Locksmithing",
		Blurb = "forge a rosette wrought iron door latch",
		Action = "forging a loose lock, key, latch, or fitting",
		ActiveCraftItemSdesc = "an in-progress rosette wrought iron door latch craft",
		Gate = CatalogueRecipeGates.RenaissanceAndEarlyModernLocksmithingBlacksmithing15,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 3,
		Interruptable = false,
		Phases = [
			new(25, "$0 heat|heats and prepare|prepares $i1 at the work area with $t1.", "$0 heat|heats and prepare|prepares $i1 at the work area with $t1.", ExertionLevel.Stasis, 0),
			new(35, "$0 work|works the fitting with $t1, checking its accessible moving surfaces, using $t2.", "$0 work|works the fitting with $t1, checking its accessible moving surfaces, using $t2.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the fitting and set|sets aside $p1.", "$0 end|ends the unsuccessful fitting and set|sets the material aside.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 759.0, Material: "wrought iron", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("759 grams of wrought iron", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Anvil", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Anvil tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_door_common_door_latch_231", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a rosette wrought iron door latch (@renaissance_door_common_door_latch_231)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition LocksmithingForgeReedPewterKeyholeEscutcheon() => new()
	{
		Metadata = new("Locksmithing/forge reed pewter keyhole escutcheon", "crafts", ["earlymodern", "renaissance"], ["item:renaissance_door_common_escutcheon_150"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "forge reed pewter keyhole escutcheon",
		Category = "Locksmithing",
		Blurb = "forge a reed pewter keyhole escutcheon",
		Action = "forging a loose lock, key, latch, or fitting",
		ActiveCraftItemSdesc = "an in-progress reed pewter keyhole escutcheon craft",
		Gate = CatalogueRecipeGates.RenaissanceAndEarlyModernLocksmithingBlacksmithing15,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 3,
		Interruptable = false,
		Phases = [
			new(25, "$0 heat|heats and prepare|prepares $i1 at the work area with $t1.", "$0 heat|heats and prepare|prepares $i1 at the work area with $t1.", ExertionLevel.Stasis, 0),
			new(35, "$0 work|works the fitting with $t1, checking its accessible moving surfaces, using $t2.", "$0 work|works the fitting with $t1, checking its accessible moving surfaces, using $t2.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the fitting and set|sets aside $p1.", "$0 end|ends the unsuccessful fitting and set|sets the material aside.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1037.0, Material: "pewter", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1037 grams of pewter", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Anvil", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Anvil tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_door_common_escutcheon_150", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a reed pewter keyhole escutcheon (@renaissance_door_common_escutcheon_150)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition LocksmithingForgeShellPewterKeyholeEscutcheonPattern3() => new()
	{
		Metadata = new("Locksmithing/forge shell pewter keyhole escutcheon pattern 3", "crafts", ["earlymodern", "renaissance"], ["item:renaissance_door_common_escutcheon_165"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "forge shell pewter keyhole escutcheon pattern 3",
		Category = "Locksmithing",
		Blurb = "forge a shell pewter keyhole escutcheon",
		Action = "forging a loose lock, key, latch, or fitting",
		ActiveCraftItemSdesc = "an in-progress shell pewter keyhole escutcheon craft",
		Gate = CatalogueRecipeGates.RenaissanceAndEarlyModernLocksmithingBlacksmithing15,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 3,
		Interruptable = false,
		Phases = [
			new(25, "$0 heat|heats and prepare|prepares $i1 at the work area with $t1.", "$0 heat|heats and prepare|prepares $i1 at the work area with $t1.", ExertionLevel.Stasis, 0),
			new(35, "$0 work|works the fitting with $t1, checking its accessible moving surfaces, using $t2.", "$0 work|works the fitting with $t1, checking its accessible moving surfaces, using $t2.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the fitting and set|sets aside $p1.", "$0 end|ends the unsuccessful fitting and set|sets the material aside.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 898.0, Material: "pewter", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("898 grams of pewter", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Anvil", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Anvil tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_door_common_escutcheon_165", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a shell pewter keyhole escutcheon (@renaissance_door_common_escutcheon_165)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition LocksmithingForgeCloudPewterKeyholeEscutcheonPattern2() => new()
	{
		Metadata = new("Locksmithing/forge cloud pewter keyhole escutcheon pattern 2", "crafts", ["earlymodern", "renaissance"], ["item:renaissance_door_common_escutcheon_180"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "forge cloud pewter keyhole escutcheon pattern 2",
		Category = "Locksmithing",
		Blurb = "forge a cloud pewter keyhole escutcheon",
		Action = "forging a loose lock, key, latch, or fitting",
		ActiveCraftItemSdesc = "an in-progress cloud pewter keyhole escutcheon craft",
		Gate = CatalogueRecipeGates.RenaissanceAndEarlyModernLocksmithingBlacksmithing15,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 3,
		Interruptable = false,
		Phases = [
			new(25, "$0 heat|heats and prepare|prepares $i1 at the work area with $t1.", "$0 heat|heats and prepare|prepares $i1 at the work area with $t1.", ExertionLevel.Stasis, 0),
			new(35, "$0 work|works the fitting with $t1, checking its accessible moving surfaces, using $t2.", "$0 work|works the fitting with $t1, checking its accessible moving surfaces, using $t2.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the fitting and set|sets aside $p1.", "$0 end|ends the unsuccessful fitting and set|sets the material aside.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 759.0, Material: "pewter", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("759 grams of pewter", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Anvil", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Anvil tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_door_common_escutcheon_180", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a cloud pewter keyhole escutcheon (@renaissance_door_common_escutcheon_180)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition LocksmithingForgeGeometricPewterKeyholeEscutcheonPattern2() => new()
	{
		Metadata = new("Locksmithing/forge geometric pewter keyhole escutcheon pattern 2", "crafts", ["earlymodern", "renaissance"], ["item:renaissance_door_common_escutcheon_195"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "forge geometric pewter keyhole escutcheon pattern 2",
		Category = "Locksmithing",
		Blurb = "forge a geometric pewter keyhole escutcheon",
		Action = "forging a loose lock, key, latch, or fitting",
		ActiveCraftItemSdesc = "an in-progress geometric pewter keyhole escutcheon craft",
		Gate = CatalogueRecipeGates.RenaissanceAndEarlyModernLocksmithingBlacksmithing15,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 3,
		Interruptable = false,
		Phases = [
			new(25, "$0 heat|heats and prepare|prepares $i1 at the work area with $t1.", "$0 heat|heats and prepare|prepares $i1 at the work area with $t1.", ExertionLevel.Stasis, 0),
			new(35, "$0 work|works the fitting with $t1, checking its accessible moving surfaces, using $t2.", "$0 work|works the fitting with $t1, checking its accessible moving surfaces, using $t2.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the fitting and set|sets aside $p1.", "$0 end|ends the unsuccessful fitting and set|sets the material aside.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 619.0, Material: "pewter", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("619 grams of pewter", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Anvil", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Anvil tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_door_common_escutcheon_195", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a geometric pewter keyhole escutcheon (@renaissance_door_common_escutcheon_195)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition LocksmithingForgeVinePewterKeyholeEscutcheonPattern2() => new()
	{
		Metadata = new("Locksmithing/forge vine pewter keyhole escutcheon pattern 2", "crafts", ["earlymodern", "renaissance"], ["item:renaissance_door_common_escutcheon_210"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "forge vine pewter keyhole escutcheon pattern 2",
		Category = "Locksmithing",
		Blurb = "forge a vine pewter keyhole escutcheon",
		Action = "forging a loose lock, key, latch, or fitting",
		ActiveCraftItemSdesc = "an in-progress vine pewter keyhole escutcheon craft",
		Gate = CatalogueRecipeGates.RenaissanceAndEarlyModernLocksmithingBlacksmithing15,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 3,
		Interruptable = false,
		Phases = [
			new(25, "$0 heat|heats and prepare|prepares $i1 at the work area with $t1.", "$0 heat|heats and prepare|prepares $i1 at the work area with $t1.", ExertionLevel.Stasis, 0),
			new(35, "$0 work|works the fitting with $t1, checking its accessible moving surfaces, using $t2.", "$0 work|works the fitting with $t1, checking its accessible moving surfaces, using $t2.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the fitting and set|sets aside $p1.", "$0 end|ends the unsuccessful fitting and set|sets the material aside.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 480.0, Material: "pewter", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("480 grams of pewter", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Anvil", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Anvil tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_door_common_escutcheon_210", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a vine pewter keyhole escutcheon (@renaissance_door_common_escutcheon_210)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition LocksmithingForgeBirdPewterKeyholeEscutcheonPattern2() => new()
	{
		Metadata = new("Locksmithing/forge bird pewter keyhole escutcheon pattern 2", "crafts", ["earlymodern", "renaissance"], ["item:renaissance_door_common_escutcheon_225"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "forge bird pewter keyhole escutcheon pattern 2",
		Category = "Locksmithing",
		Blurb = "forge a bird pewter keyhole escutcheon",
		Action = "forging a loose lock, key, latch, or fitting",
		ActiveCraftItemSdesc = "an in-progress bird pewter keyhole escutcheon craft",
		Gate = CatalogueRecipeGates.RenaissanceAndEarlyModernLocksmithingBlacksmithing15,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 3,
		Interruptable = false,
		Phases = [
			new(25, "$0 heat|heats and prepare|prepares $i1 at the work area with $t1.", "$0 heat|heats and prepare|prepares $i1 at the work area with $t1.", ExertionLevel.Stasis, 0),
			new(35, "$0 work|works the fitting with $t1, checking its accessible moving surfaces, using $t2.", "$0 work|works the fitting with $t1, checking its accessible moving surfaces, using $t2.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the fitting and set|sets aside $p1.", "$0 end|ends the unsuccessful fitting and set|sets the material aside.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 340.0, Material: "pewter", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("340 grams of pewter", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Anvil", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Anvil tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_door_common_escutcheon_225", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a bird pewter keyhole escutcheon (@renaissance_door_common_escutcheon_225)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition LocksmithingForgeStarPewterKeyholeEscutcheonPattern3() => new()
	{
		Metadata = new("Locksmithing/forge star pewter keyhole escutcheon pattern 3", "crafts", ["earlymodern", "renaissance"], ["item:renaissance_door_common_escutcheon_240"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "forge star pewter keyhole escutcheon pattern 3",
		Category = "Locksmithing",
		Blurb = "forge a star pewter keyhole escutcheon",
		Action = "forging a loose lock, key, latch, or fitting",
		ActiveCraftItemSdesc = "an in-progress star pewter keyhole escutcheon craft",
		Gate = CatalogueRecipeGates.RenaissanceAndEarlyModernLocksmithingBlacksmithing15,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 3,
		Interruptable = false,
		Phases = [
			new(25, "$0 heat|heats and prepare|prepares $i1 at the work area with $t1.", "$0 heat|heats and prepare|prepares $i1 at the work area with $t1.", ExertionLevel.Stasis, 0),
			new(35, "$0 work|works the fitting with $t1, checking its accessible moving surfaces, using $t2.", "$0 work|works the fitting with $t1, checking its accessible moving surfaces, using $t2.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the fitting and set|sets aside $p1.", "$0 end|ends the unsuccessful fitting and set|sets the material aside.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 201.0, Material: "pewter", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("201 grams of pewter", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Anvil", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Anvil tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_door_common_escutcheon_240", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a star pewter keyhole escutcheon (@renaissance_door_common_escutcheon_240)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition CarpentryAndJoineryBuildStarBronzeGardenGate() => new()
	{
		Metadata = new("Carpentry and joinery/build star bronze garden gate", "crafts", ["earlymodern", "renaissance"], ["item:renaissance_door_common_garden_gate_10"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "build star bronze garden gate",
		Category = "Carpentry and joinery",
		Blurb = "build a star bronze garden gate",
		Action = "building a door, gate, shutter, screen, grille, or barrier",
		ActiveCraftItemSdesc = "an in-progress star bronze garden gate craft",
		Gate = CatalogueRecipeGates.RenaissanceAndEarlyModernJoineryCarpentry15,
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
			new(35, "$0 mark|marks and prepare|prepares $i1 with $t2, laying out the fitted edges.", "$0 mark|marks and prepare|prepares $i1 with $t2, laying out the fitted edges.", ExertionLevel.Stasis, 0),
			new(45, "$0 shape|shapes the material and secure|secures its fitted parts with $t1.", "$0 shape|shapes the material and secure|secures its fitted parts with $t1.", ExertionLevel.Stasis, 0),
			new(35, "$0 inspect|inspects the finished work and set|sets aside $p1.", "$0 find|finds the work has failed to hold together and set|sets the material aside.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 35376.0, Material: "bronze", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("35376 grams of bronze", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Saw", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Saw tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_door_common_garden_gate_10", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a star bronze garden gate (@renaissance_door_common_garden_gate_10)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition CarpentryAndJoineryBuildCloudTeakGardenGatePattern2() => new()
	{
		Metadata = new("Carpentry and joinery/build cloud teak garden gate pattern 2", "crafts", ["earlymodern", "renaissance"], ["item:renaissance_door_common_garden_gate_108"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "build cloud teak garden gate pattern 2",
		Category = "Carpentry and joinery",
		Blurb = "build a cloud teak garden gate",
		Action = "building a door, gate, shutter, screen, grille, or barrier",
		ActiveCraftItemSdesc = "an in-progress cloud teak garden gate craft",
		Gate = CatalogueRecipeGates.RenaissanceAndEarlyModernJoineryCarpentry15,
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
			new(35, "$0 mark|marks and prepare|prepares $i1 with $t2, laying out the fitted edges.", "$0 mark|marks and prepare|prepares $i1 with $t2, laying out the fitted edges.", ExertionLevel.Stasis, 0),
			new(45, "$0 shape|shapes the material and secure|secures its fitted parts with $t1.", "$0 shape|shapes the material and secure|secures its fitted parts with $t1.", ExertionLevel.Stasis, 0),
			new(35, "$0 inspect|inspects the finished work and set|sets aside $p1.", "$0 find|finds the work has failed to hold together and set|sets the material aside.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 40128.0, Material: "teak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("40128 grams of teak", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Saw", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Saw tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_door_common_garden_gate_108", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a cloud teak garden gate (@renaissance_door_common_garden_gate_108)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition CarpentryAndJoineryBuildStarCedarGardenGate() => new()
	{
		Metadata = new("Carpentry and joinery/build star cedar garden gate", "crafts", ["earlymodern", "renaissance"], ["item:renaissance_door_common_garden_gate_122"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "build star cedar garden gate",
		Category = "Carpentry and joinery",
		Blurb = "build a star cedar garden gate",
		Action = "building a door, gate, shutter, screen, grille, or barrier",
		ActiveCraftItemSdesc = "an in-progress star cedar garden gate craft",
		Gate = CatalogueRecipeGates.RenaissanceAndEarlyModernJoineryCarpentry15,
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
			new(35, "$0 mark|marks and prepare|prepares $i1 with $t2, laying out the fitted edges.", "$0 mark|marks and prepare|prepares $i1 with $t2, laying out the fitted edges.", ExertionLevel.Stasis, 0),
			new(45, "$0 shape|shapes the material and secure|secures its fitted parts with $t1.", "$0 shape|shapes the material and secure|secures its fitted parts with $t1.", ExertionLevel.Stasis, 0),
			new(35, "$0 inspect|inspects the finished work and set|sets aside $p1.", "$0 find|finds the work has failed to hold together and set|sets the material aside.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 32208.0, Material: "cedar", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("32208 grams of cedar", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Saw", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Saw tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_door_common_garden_gate_122", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a star cedar garden gate (@renaissance_door_common_garden_gate_122)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition CarpentryAndJoineryBuildSunburstOakGardenGate() => new()
	{
		Metadata = new("Carpentry and joinery/build sunburst oak garden gate", "crafts", ["earlymodern", "renaissance"], ["item:renaissance_door_common_garden_gate_136"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "build sunburst oak garden gate",
		Category = "Carpentry and joinery",
		Blurb = "build a sunburst oak garden gate",
		Action = "building a door, gate, shutter, screen, grille, or barrier",
		ActiveCraftItemSdesc = "an in-progress sunburst oak garden gate craft",
		Gate = CatalogueRecipeGates.RenaissanceAndEarlyModernJoineryCarpentry15,
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
			new(35, "$0 mark|marks and prepare|prepares $i1 with $t2, laying out the fitted edges.", "$0 mark|marks and prepare|prepares $i1 with $t2, laying out the fitted edges.", ExertionLevel.Stasis, 0),
			new(45, "$0 shape|shapes the material and secure|secures its fitted parts with $t1.", "$0 shape|shapes the material and secure|secures its fitted parts with $t1.", ExertionLevel.Stasis, 0),
			new(35, "$0 inspect|inspects the finished work and set|sets aside $p1.", "$0 find|finds the work has failed to hold together and set|sets the material aside.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 24288.0, Material: "oak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("24288 grams of oak", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Saw", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Saw tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_door_common_garden_gate_136", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a sunburst oak garden gate (@renaissance_door_common_garden_gate_136)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition CarpentryAndJoineryBuildSunburstWroughtIronGardenGate() => new()
	{
		Metadata = new("Carpentry and joinery/build sunburst wrought iron garden gate", "crafts", ["earlymodern", "renaissance"], ["item:renaissance_door_common_garden_gate_24"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "build sunburst wrought iron garden gate",
		Category = "Carpentry and joinery",
		Blurb = "build a sunburst wrought iron garden gate",
		Action = "building a door, gate, shutter, screen, grille, or barrier",
		ActiveCraftItemSdesc = "an in-progress sunburst wrought iron garden gate craft",
		Gate = CatalogueRecipeGates.RenaissanceAndEarlyModernJoineryCarpentry15,
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
			new(35, "$0 mark|marks and prepare|prepares $i1 with $t2, laying out the fitted edges.", "$0 mark|marks and prepare|prepares $i1 with $t2, laying out the fitted edges.", ExertionLevel.Stasis, 0),
			new(45, "$0 shape|shapes the material and secure|secures its fitted parts with $t1.", "$0 shape|shapes the material and secure|secures its fitted parts with $t1.", ExertionLevel.Stasis, 0),
			new(35, "$0 inspect|inspects the finished work and set|sets aside $p1.", "$0 find|finds the work has failed to hold together and set|sets the material aside.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 27456.0, Material: "wrought iron", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("27456 grams of wrought iron", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Saw", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Saw tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_door_common_garden_gate_24", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a sunburst wrought iron garden gate (@renaissance_door_common_garden_gate_24)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition CarpentryAndJoineryBuildCheckerTeakGardenGate() => new()
	{
		Metadata = new("Carpentry and joinery/build checker teak garden gate", "crafts", ["earlymodern", "renaissance"], ["item:renaissance_door_common_garden_gate_38"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "build checker teak garden gate",
		Category = "Carpentry and joinery",
		Blurb = "build a checker teak garden gate",
		Action = "building a door, gate, shutter, screen, grille, or barrier",
		ActiveCraftItemSdesc = "an in-progress checker teak garden gate craft",
		Gate = CatalogueRecipeGates.RenaissanceAndEarlyModernJoineryCarpentry15,
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
			new(35, "$0 mark|marks and prepare|prepares $i1 with $t2, laying out the fitted edges.", "$0 mark|marks and prepare|prepares $i1 with $t2, laying out the fitted edges.", ExertionLevel.Stasis, 0),
			new(45, "$0 shape|shapes the material and secure|secures its fitted parts with $t1.", "$0 shape|shapes the material and secure|secures its fitted parts with $t1.", ExertionLevel.Stasis, 0),
			new(35, "$0 inspect|inspects the finished work and set|sets aside $p1.", "$0 find|finds the work has failed to hold together and set|sets the material aside.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 49632.0, Material: "teak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("49632 grams of teak", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Saw", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Saw tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_door_common_garden_gate_38", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a checker teak garden gate (@renaissance_door_common_garden_gate_38)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition CarpentryAndJoineryBuildCloudCedarGardenGate() => new()
	{
		Metadata = new("Carpentry and joinery/build cloud cedar garden gate", "crafts", ["earlymodern", "renaissance"], ["item:renaissance_door_common_garden_gate_52"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "build cloud cedar garden gate",
		Category = "Carpentry and joinery",
		Blurb = "build a cloud cedar garden gate",
		Action = "building a door, gate, shutter, screen, grille, or barrier",
		ActiveCraftItemSdesc = "an in-progress cloud cedar garden gate craft",
		Gate = CatalogueRecipeGates.RenaissanceAndEarlyModernJoineryCarpentry15,
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
			new(35, "$0 mark|marks and prepare|prepares $i1 with $t2, laying out the fitted edges.", "$0 mark|marks and prepare|prepares $i1 with $t2, laying out the fitted edges.", ExertionLevel.Stasis, 0),
			new(45, "$0 shape|shapes the material and secure|secures its fitted parts with $t1.", "$0 shape|shapes the material and secure|secures its fitted parts with $t1.", ExertionLevel.Stasis, 0),
			new(35, "$0 inspect|inspects the finished work and set|sets aside $p1.", "$0 find|finds the work has failed to hold together and set|sets the material aside.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 41712.0, Material: "cedar", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("41712 grams of cedar", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Saw", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Saw tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_door_common_garden_gate_52", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a cloud cedar garden gate (@renaissance_door_common_garden_gate_52)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition CarpentryAndJoineryBuildStarOakGardenGatePattern3() => new()
	{
		Metadata = new("Carpentry and joinery/build star oak garden gate pattern 3", "crafts", ["earlymodern", "renaissance"], ["item:renaissance_door_common_garden_gate_66"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "build star oak garden gate pattern 3",
		Category = "Carpentry and joinery",
		Blurb = "build a star oak garden gate",
		Action = "building a door, gate, shutter, screen, grille, or barrier",
		ActiveCraftItemSdesc = "an in-progress star oak garden gate craft",
		Gate = CatalogueRecipeGates.RenaissanceAndEarlyModernJoineryCarpentry15,
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
			new(35, "$0 mark|marks and prepare|prepares $i1 with $t2, laying out the fitted edges.", "$0 mark|marks and prepare|prepares $i1 with $t2, laying out the fitted edges.", ExertionLevel.Stasis, 0),
			new(45, "$0 shape|shapes the material and secure|secures its fitted parts with $t1.", "$0 shape|shapes the material and secure|secures its fitted parts with $t1.", ExertionLevel.Stasis, 0),
			new(35, "$0 inspect|inspects the finished work and set|sets aside $p1.", "$0 find|finds the work has failed to hold together and set|sets the material aside.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 33792.0, Material: "oak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("33792 grams of oak", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Saw", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Saw tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_door_common_garden_gate_66", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a star oak garden gate (@renaissance_door_common_garden_gate_66)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition CarpentryAndJoineryBuildSunburstBronzeGardenGatePattern2() => new()
	{
		Metadata = new("Carpentry and joinery/build sunburst bronze garden gate pattern 2", "crafts", ["earlymodern", "renaissance"], ["item:renaissance_door_common_garden_gate_80"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "build sunburst bronze garden gate pattern 2",
		Category = "Carpentry and joinery",
		Blurb = "build a sunburst bronze garden gate",
		Action = "building a door, gate, shutter, screen, grille, or barrier",
		ActiveCraftItemSdesc = "an in-progress sunburst bronze garden gate craft",
		Gate = CatalogueRecipeGates.RenaissanceAndEarlyModernJoineryCarpentry15,
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
			new(35, "$0 mark|marks and prepare|prepares $i1 with $t2, laying out the fitted edges.", "$0 mark|marks and prepare|prepares $i1 with $t2, laying out the fitted edges.", ExertionLevel.Stasis, 0),
			new(45, "$0 shape|shapes the material and secure|secures its fitted parts with $t1.", "$0 shape|shapes the material and secure|secures its fitted parts with $t1.", ExertionLevel.Stasis, 0),
			new(35, "$0 inspect|inspects the finished work and set|sets aside $p1.", "$0 find|finds the work has failed to hold together and set|sets the material aside.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 25872.0, Material: "bronze", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("25872 grams of bronze", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Saw", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Saw tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_door_common_garden_gate_80", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a sunburst bronze garden gate (@renaissance_door_common_garden_gate_80)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition CarpentryAndJoineryBuildCheckerWroughtIronGardenGate() => new()
	{
		Metadata = new("Carpentry and joinery/build checker wrought iron garden gate", "crafts", ["earlymodern", "renaissance"], ["item:renaissance_door_common_garden_gate_94"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "build checker wrought iron garden gate",
		Category = "Carpentry and joinery",
		Blurb = "build a checker wrought iron garden gate",
		Action = "building a door, gate, shutter, screen, grille, or barrier",
		ActiveCraftItemSdesc = "an in-progress checker wrought iron garden gate craft",
		Gate = CatalogueRecipeGates.RenaissanceAndEarlyModernJoineryCarpentry15,
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
			new(35, "$0 mark|marks and prepare|prepares $i1 with $t2, laying out the fitted edges.", "$0 mark|marks and prepare|prepares $i1 with $t2, laying out the fitted edges.", ExertionLevel.Stasis, 0),
			new(45, "$0 shape|shapes the material and secure|secures its fitted parts with $t1.", "$0 shape|shapes the material and secure|secures its fitted parts with $t1.", ExertionLevel.Stasis, 0),
			new(35, "$0 inspect|inspects the finished work and set|sets aside $p1.", "$0 find|finds the work has failed to hold together and set|sets the material aside.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 48048.0, Material: "wrought iron", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("48048 grams of wrought iron", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Saw", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Saw tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_door_common_garden_gate_94", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a checker wrought iron garden gate (@renaissance_door_common_garden_gate_94)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};
}
