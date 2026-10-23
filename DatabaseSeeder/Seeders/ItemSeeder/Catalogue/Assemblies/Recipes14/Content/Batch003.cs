#nullable enable

using MudSharp.Body;
using MudSharp.GameItems.Inventory.Plans;
using MudSharp.RPG.Checks;

namespace DatabaseSeeder.Seeders;

public static partial class Recipes14Stock
{
	public static CatalogueRecipeDefinition LocksmithingForgeFlowerHeadPewterWardedKeyPattern3() => new()
	{
		Metadata = new("Locksmithing/forge flower-head pewter warded key pattern 3", "crafts", ["earlymodern", "renaissance"], ["item:renaissance_door_sem_warded_key_355"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "forge flower-head pewter warded key pattern 3",
		Category = "Locksmithing",
		Blurb = "forge a flower-head pewter warded key",
		Action = "forging a loose lock, key, latch, or fitting",
		ActiveCraftItemSdesc = "an in-progress flower-head pewter warded key craft",
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
			new CatalogueCommodityRecipeInput(Grams: 1107.0, Material: "pewter", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1107 grams of pewter", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Anvil", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Anvil tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_door_sem_warded_key_355", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a flower-head pewter warded key (@renaissance_door_sem_warded_key_355)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition CarpentryAndJoineryBuildCloudBronzeWarehouseGatePattern2() => new()
	{
		Metadata = new("Carpentry and joinery/build cloud bronze warehouse gate pattern 2", "crafts", ["earlymodern", "renaissance"], ["item:renaissance_door_sem_warehouse_gate_345"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "build cloud bronze warehouse gate pattern 2",
		Category = "Carpentry and joinery",
		Blurb = "build a cloud bronze warehouse gate",
		Action = "building a door, gate, shutter, screen, grille, or barrier",
		ActiveCraftItemSdesc = "an in-progress cloud bronze warehouse gate craft",
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
			new CatalogueCommodityRecipeInput(Grams: 24288.0, Material: "bronze", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("24288 grams of bronze", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Saw", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Saw tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_door_sem_warehouse_gate_345", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a cloud bronze warehouse gate (@renaissance_door_sem_warehouse_gate_345)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition CarpentryAndJoineryBuildKnottedBronzeWicketGatePattern2() => new()
	{
		Metadata = new("Carpentry and joinery/build knotted bronze wicket gate pattern 2", "crafts", ["earlymodern", "renaissance"], ["item:renaissance_door_sem_wicket_350"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "build knotted bronze wicket gate pattern 2",
		Category = "Carpentry and joinery",
		Blurb = "build a knotted bronze wicket gate",
		Action = "building a door, gate, shutter, screen, grille, or barrier",
		ActiveCraftItemSdesc = "an in-progress knotted bronze wicket gate craft",
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
			new CatalogueCommodityRecipeInput(Grams: 32208.0, Material: "bronze", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("32208 grams of bronze", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Saw", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Saw tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_door_sem_wicket_350", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a knotted bronze wicket gate (@renaissance_door_sem_wicket_350)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition CarpentryAndJoineryBuildGeometricBronzeBarredBarrierPattern2() => new()
	{
		Metadata = new("Carpentry and joinery/build geometric bronze barred barrier pattern 2", "crafts", ["earlymodern", "renaissance"], ["item:renaissance_door_stp_barrier_404"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "build geometric bronze barred barrier pattern 2",
		Category = "Carpentry and joinery",
		Blurb = "build a geometric bronze barred barrier",
		Action = "building a door, gate, shutter, screen, grille, or barrier",
		ActiveCraftItemSdesc = "an in-progress geometric bronze barred barrier craft",
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
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_door_stp_barrier_404", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a geometric bronze barred barrier (@renaissance_door_stp_barrier_404)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition CarpentryAndJoineryBuildFlowerHeadPineBracedDoorPattern3() => new()
	{
		Metadata = new("Carpentry and joinery/build flower-head pine braced door pattern 3", "crafts", ["earlymodern", "renaissance"], ["item:renaissance_door_stp_braced_door_394"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "build flower-head pine braced door pattern 3",
		Category = "Carpentry and joinery",
		Blurb = "build a flower-head pine braced door",
		Action = "building a door, gate, shutter, screen, grille, or barrier",
		ActiveCraftItemSdesc = "an in-progress flower-head pine braced door craft",
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
			new CatalogueCommodityRecipeInput(Grams: 41712.0, Material: "pine", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("41712 grams of pine", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Saw", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Saw tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_door_stp_braced_door_394", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a flower-head pine braced door (@renaissance_door_stp_braced_door_394)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition CarpentryAndJoineryBuildLeafScrollOakBracedDoorPattern3() => new()
	{
		Metadata = new("Carpentry and joinery/build leaf-scroll oak braced door pattern 3", "crafts", ["earlymodern", "renaissance"], ["item:renaissance_door_stp_braced_door_408"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "build leaf-scroll oak braced door pattern 3",
		Category = "Carpentry and joinery",
		Blurb = "build a leaf-scroll oak braced door",
		Action = "building a door, gate, shutter, screen, grille, or barrier",
		ActiveCraftItemSdesc = "an in-progress leaf-scroll oak braced door craft",
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
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_door_stp_braced_door_408", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a leaf-scroll oak braced door (@renaissance_door_stp_braced_door_408)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition CarpentryAndJoineryBuildLeafScrollTeakCourtyardGatePattern2() => new()
	{
		Metadata = new("Carpentry and joinery/build leaf-scroll teak courtyard gate pattern 2", "crafts", ["earlymodern", "renaissance"], ["item:renaissance_door_stp_courtyard_gate_400"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "build leaf-scroll teak courtyard gate pattern 2",
		Category = "Carpentry and joinery",
		Blurb = "build a leaf-scroll teak courtyard gate",
		Action = "building a door, gate, shutter, screen, grille, or barrier",
		ActiveCraftItemSdesc = "an in-progress leaf-scroll teak courtyard gate craft",
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
			new CatalogueCommodityRecipeInput(Grams: 21120.0, Material: "teak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("21120 grams of teak", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Saw", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Saw tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_door_stp_courtyard_gate_400", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a leaf-scroll teak courtyard gate (@renaissance_door_stp_courtyard_gate_400)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition LocksmithingForgeWaterWavePewterDoorBarPattern2() => new()
	{
		Metadata = new("Locksmithing/forge water-wave pewter door bar pattern 2", "crafts", ["earlymodern", "renaissance"], ["item:renaissance_door_stp_door_bar_415"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "forge water-wave pewter door bar pattern 2",
		Category = "Locksmithing",
		Blurb = "forge a water-wave pewter door bar",
		Action = "forging a loose lock, key, latch, or fitting",
		ActiveCraftItemSdesc = "an in-progress water-wave pewter door bar craft",
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
			new CatalogueCommodityRecipeInput(Grams: 549.0, Material: "pewter", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("549 grams of pewter", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Anvil", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Anvil tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_door_stp_door_bar_415", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a water-wave pewter door bar (@renaissance_door_stp_door_bar_415)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition LocksmithingForgeLeafScrollBrassDoorLatchPattern2() => new()
	{
		Metadata = new("Locksmithing/forge leaf-scroll brass door latch pattern 2", "crafts", ["earlymodern", "renaissance"], ["item:renaissance_door_stp_door_latch_413"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "forge leaf-scroll brass door latch pattern 2",
		Category = "Locksmithing",
		Blurb = "forge a leaf-scroll brass door latch",
		Action = "forging a loose lock, key, latch, or fitting",
		ActiveCraftItemSdesc = "an in-progress leaf-scroll brass door latch craft",
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
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_door_stp_door_latch_413", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a leaf-scroll brass door latch (@renaissance_door_stp_door_latch_413)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition CarpentryAndJoineryBuildFlowerHeadBronzeGardenGatePattern2() => new()
	{
		Metadata = new("Carpentry and joinery/build flower-head bronze garden gate pattern 2", "crafts", ["earlymodern", "renaissance"], ["item:renaissance_door_stp_garden_gate_402"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "build flower-head bronze garden gate pattern 2",
		Category = "Carpentry and joinery",
		Blurb = "build a flower-head bronze garden gate",
		Action = "building a door, gate, shutter, screen, grille, or barrier",
		ActiveCraftItemSdesc = "an in-progress flower-head bronze garden gate craft",
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
			new CatalogueCommodityRecipeInput(Grams: 24288.0, Material: "bronze", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("24288 grams of bronze", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Saw", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Saw tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_door_stp_garden_gate_402", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a flower-head bronze garden gate (@renaissance_door_stp_garden_gate_402)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition LocksmithingForgeSunburstCopperGateDropBarPattern2() => new()
	{
		Metadata = new("Locksmithing/forge sunburst copper gate drop bar pattern 2", "crafts", ["earlymodern", "renaissance"], ["item:renaissance_door_stp_gate_dropbar_414"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "forge sunburst copper gate drop bar pattern 2",
		Category = "Locksmithing",
		Blurb = "forge a sunburst copper gate drop bar",
		Action = "forging a loose lock, key, latch, or fitting",
		ActiveCraftItemSdesc = "an in-progress sunburst copper gate drop bar craft",
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
			new CatalogueCommodityRecipeInput(Grams: 480.0, Material: "copper", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("480 grams of copper", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Anvil", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Anvil tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_door_stp_gate_dropbar_414", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a sunburst copper gate drop bar (@renaissance_door_stp_gate_dropbar_414)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition CarpentryAndJoineryBuildBirdWroughtIronMetalGrillePattern4() => new()
	{
		Metadata = new("Carpentry and joinery/build bird wrought iron metal grille pattern 4", "crafts", ["earlymodern", "renaissance"], ["item:renaissance_door_stp_grille_403"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "build bird wrought iron metal grille pattern 4",
		Category = "Carpentry and joinery",
		Blurb = "build a bird wrought iron metal grille",
		Action = "building a door, gate, shutter, screen, grille, or barrier",
		ActiveCraftItemSdesc = "an in-progress bird wrought iron metal grille craft",
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
			new CatalogueCommodityRecipeInput(Grams: 25872.0, Material: "wrought iron", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("25872 grams of wrought iron", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Saw", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Saw tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_door_stp_grille_403", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a bird wrought iron metal grille (@renaissance_door_stp_grille_403)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition LocksmithingForgeFlowerHeadBronzeIronHaspPattern2() => new()
	{
		Metadata = new("Locksmithing/forge flower-head bronze iron hasp pattern 2", "crafts", ["earlymodern", "renaissance"], ["item:renaissance_door_stp_hasp_417"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "forge flower-head bronze iron hasp pattern 2",
		Category = "Locksmithing",
		Blurb = "forge a flower-head bronze iron hasp",
		Action = "forging a loose lock, key, latch, or fitting",
		ActiveCraftItemSdesc = "an in-progress flower-head bronze iron hasp craft",
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
			new CatalogueCommodityRecipeInput(Grams: 689.0, Material: "bronze", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("689 grams of bronze", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Anvil", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Anvil tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_door_stp_hasp_417", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a flower-head bronze iron hasp (@renaissance_door_stp_hasp_417)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition CarpentryAndJoineryBuildShellCedarServiceHatch() => new()
	{
		Metadata = new("Carpentry and joinery/build shell cedar service hatch", "crafts", ["earlymodern", "renaissance"], ["item:renaissance_door_stp_hatch_405"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "build shell cedar service hatch",
		Category = "Carpentry and joinery",
		Blurb = "build a shell cedar service hatch",
		Action = "building a door, gate, shutter, screen, grille, or barrier",
		ActiveCraftItemSdesc = "an in-progress shell cedar service hatch craft",
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
			new CatalogueCommodityRecipeInput(Grams: 29040.0, Material: "cedar", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("29040 grams of cedar", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Saw", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Saw tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_door_stp_hatch_405", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a shell cedar service hatch (@renaissance_door_stp_hatch_405)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition LocksmithingForgeVinePewterHingePairPattern2() => new()
	{
		Metadata = new("Locksmithing/forge vine pewter hinge pair pattern 2", "crafts", ["earlymodern", "renaissance"], ["item:renaissance_door_stp_hinge_pair_420"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "forge vine pewter hinge pair pattern 2",
		Category = "Locksmithing",
		Blurb = "forge a vine pewter hinge pair",
		Action = "forging a loose lock, key, latch, or fitting",
		ActiveCraftItemSdesc = "an in-progress vine pewter hinge pair craft",
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
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_door_stp_hinge_pair_420", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a vine pewter hinge pair (@renaissance_door_stp_hinge_pair_420)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition LocksmithingForgeStarBrassHookLatchPattern2() => new()
	{
		Metadata = new("Locksmithing/forge star brass hook latch pattern 2", "crafts", ["earlymodern", "renaissance"], ["item:renaissance_door_stp_hook_latch_418"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "forge star brass hook latch pattern 2",
		Category = "Locksmithing",
		Blurb = "forge a star brass hook latch",
		Action = "forging a loose lock, key, latch, or fitting",
		ActiveCraftItemSdesc = "an in-progress star brass hook latch craft",
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
			new CatalogueCommodityRecipeInput(Grams: 759.0, Material: "brass", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("759 grams of brass", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Anvil", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Anvil tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_door_stp_hook_latch_418", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a star brass hook latch (@renaissance_door_stp_hook_latch_418)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition CarpentryAndJoineryBuildBraidedCedarLatticeGatePattern2() => new()
	{
		Metadata = new("Carpentry and joinery/build braided cedar lattice gate pattern 2", "crafts", ["earlymodern", "renaissance"], ["item:renaissance_door_stp_lattice_gate_399"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "build braided cedar lattice gate pattern 2",
		Category = "Carpentry and joinery",
		Blurb = "build a braided cedar lattice gate",
		Action = "building a door, gate, shutter, screen, grille, or barrier",
		ActiveCraftItemSdesc = "an in-progress braided cedar lattice gate craft",
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
			new CatalogueCommodityRecipeInput(Grams: 49632.0, Material: "cedar", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("49632 grams of cedar", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Saw", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Saw tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_door_stp_lattice_gate_399", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a braided cedar lattice gate (@renaissance_door_stp_lattice_gate_399)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition CarpentryAndJoineryBuildBirdCedarLockableDoorPattern3() => new()
	{
		Metadata = new("Carpentry and joinery/build bird cedar lockable door pattern 3", "crafts", ["earlymodern", "renaissance"], ["item:renaissance_door_stp_lockable_door_395"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "build bird cedar lockable door pattern 3",
		Category = "Carpentry and joinery",
		Blurb = "build a bird cedar lockable door",
		Action = "building a door, gate, shutter, screen, grille, or barrier",
		ActiveCraftItemSdesc = "an in-progress bird cedar lockable door craft",
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
			new CatalogueCommodityRecipeInput(Grams: 43296.0, Material: "cedar", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("43296 grams of cedar", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Saw", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Saw tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_door_stp_lockable_door_395", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a bird cedar lockable door (@renaissance_door_stp_lockable_door_395)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition CarpentryAndJoineryBuildWaterWavePineLockableDoorPattern3() => new()
	{
		Metadata = new("Carpentry and joinery/build water-wave pine lockable door pattern 3", "crafts", ["earlymodern", "renaissance"], ["item:renaissance_door_stp_lockable_door_409"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "build water-wave pine lockable door pattern 3",
		Category = "Carpentry and joinery",
		Blurb = "build a water-wave pine lockable door",
		Action = "building a door, gate, shutter, screen, grille, or barrier",
		ActiveCraftItemSdesc = "an in-progress water-wave pine lockable door craft",
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
			new CatalogueCommodityRecipeInput(Grams: 35376.0, Material: "pine", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("35376 grams of pine", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Saw", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Saw tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_door_stp_lockable_door_409", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a water-wave pine lockable door (@renaissance_door_stp_lockable_door_409)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition LocksmithingForgeCheckerPewterMasterWardedLockPattern2() => new()
	{
		Metadata = new("Locksmithing/forge checker pewter master warded lock pattern 2", "crafts", ["earlymodern", "renaissance"], ["item:renaissance_door_stp_master_warded_lock_410"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "forge checker pewter master warded lock pattern 2",
		Category = "Locksmithing",
		Blurb = "forge a checker pewter master warded lock",
		Action = "forging a loose lock, key, latch, or fitting",
		ActiveCraftItemSdesc = "an in-progress checker pewter master warded lock craft",
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
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_door_stp_master_warded_lock_410", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a checker pewter master warded lock (@renaissance_door_stp_master_warded_lock_410)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition CarpentryAndJoineryBuildWaterWaveOakDoorLeafPattern3() => new()
	{
		Metadata = new("Carpentry and joinery/build water-wave oak door leaf pattern 3", "crafts", ["earlymodern", "renaissance"], ["item:renaissance_door_stp_panel_door_393"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "build water-wave oak door leaf pattern 3",
		Category = "Carpentry and joinery",
		Blurb = "build a water-wave oak door leaf",
		Action = "building a door, gate, shutter, screen, grille, or barrier",
		ActiveCraftItemSdesc = "an in-progress water-wave oak door leaf craft",
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
			new CatalogueCommodityRecipeInput(Grams: 40128.0, Material: "oak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("40128 grams of oak", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Saw", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Saw tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_door_stp_panel_door_393", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a water-wave oak door leaf (@renaissance_door_stp_panel_door_393)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition CarpentryAndJoineryBuildBraidedWoodDoorLeafPattern3() => new()
	{
		Metadata = new("Carpentry and joinery/build braided wood door leaf pattern 3", "crafts", ["earlymodern", "renaissance"], ["item:renaissance_door_stp_panel_door_407"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "build braided wood door leaf pattern 3",
		Category = "Carpentry and joinery",
		Blurb = "build a braided wood door leaf",
		Action = "building a door, gate, shutter, screen, grille, or barrier",
		ActiveCraftItemSdesc = "an in-progress braided wood door leaf craft",
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
			new CatalogueCommodityRecipeInput(Grams: 32208.0, Material: "wood", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("32208 grams of wood", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Saw", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Saw tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_door_stp_panel_door_407", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a braided wood door leaf (@renaissance_door_stp_panel_door_407)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition LocksmithingForgeKnottedWroughtIronWinchPawlPattern2() => new()
	{
		Metadata = new("Locksmithing/forge knotted wrought iron winch pawl pattern 2", "crafts", ["earlymodern", "renaissance"], ["item:renaissance_door_stp_portcullis_pawl_416"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "forge knotted wrought iron winch pawl pattern 2",
		Category = "Locksmithing",
		Blurb = "forge a knotted wrought iron winch pawl",
		Action = "forging a loose lock, key, latch, or fitting",
		ActiveCraftItemSdesc = "an in-progress knotted wrought iron winch pawl craft",
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
			new CatalogueCommodityRecipeInput(Grams: 619.0, Material: "wrought iron", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("619 grams of wrought iron", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Anvil", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Anvil tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_door_stp_portcullis_pawl_416", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a knotted wrought iron winch pawl (@renaissance_door_stp_portcullis_pawl_416)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition LocksmithingForgeRadiatingBronzeRingBowKeyPattern2() => new()
	{
		Metadata = new("Locksmithing/forge radiating bronze ring bow key pattern 2", "crafts", ["earlymodern", "renaissance"], ["item:renaissance_door_stp_ring_key_412"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "forge radiating bronze ring bow key pattern 2",
		Category = "Locksmithing",
		Blurb = "forge a radiating bronze ring bow key",
		Action = "forging a loose lock, key, latch, or fitting",
		ActiveCraftItemSdesc = "an in-progress radiating bronze ring bow key craft",
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
			new CatalogueCommodityRecipeInput(Grams: 340.0, Material: "bronze", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("340 grams of bronze", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Anvil", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Anvil tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_door_stp_ring_key_412", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a radiating bronze ring bow key (@renaissance_door_stp_ring_key_412)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition CarpentryAndJoineryBuildRosetteWoodScreenDoorPattern2() => new()
	{
		Metadata = new("Carpentry and joinery/build rosette wood screen door pattern 2", "crafts", ["earlymodern", "renaissance"], ["item:renaissance_door_stp_screen_398"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "build rosette wood screen door pattern 2",
		Category = "Carpentry and joinery",
		Blurb = "build a rosette wood screen door",
		Action = "building a door, gate, shutter, screen, grille, or barrier",
		ActiveCraftItemSdesc = "an in-progress rosette wood screen door craft",
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
			new CatalogueCommodityRecipeInput(Grams: 48048.0, Material: "wood", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("48048 grams of wood", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Saw", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Saw tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_door_stp_screen_398", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a rosette wood screen door (@renaissance_door_stp_screen_398)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition CarpentryAndJoineryBuildGeometricTeakSecureDoorPattern2() => new()
	{
		Metadata = new("Carpentry and joinery/build geometric teak secure door pattern 2", "crafts", ["earlymodern", "renaissance"], ["item:renaissance_door_stp_secure_door_396"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "build geometric teak secure door pattern 2",
		Category = "Carpentry and joinery",
		Blurb = "build a geometric teak secure door",
		Action = "building a door, gate, shutter, screen, grille, or barrier",
		ActiveCraftItemSdesc = "an in-progress geometric teak secure door craft",
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
			new CatalogueCommodityRecipeInput(Grams: 44880.0, Material: "teak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("44880 grams of teak", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Saw", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Saw tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_door_stp_secure_door_396", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a geometric teak secure door (@renaissance_door_stp_secure_door_396)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition CarpentryAndJoineryBuildShellWoodShutteredOpeningPattern2() => new()
	{
		Metadata = new("Carpentry and joinery/build shell wood shuttered opening pattern 2", "crafts", ["earlymodern", "renaissance"], ["item:renaissance_door_stp_shutter_397"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "build shell wood shuttered opening pattern 2",
		Category = "Carpentry and joinery",
		Blurb = "build a shell wood shuttered opening",
		Action = "building a door, gate, shutter, screen, grille, or barrier",
		ActiveCraftItemSdesc = "an in-progress shell wood shuttered opening craft",
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
			new CatalogueCommodityRecipeInput(Grams: 46464.0, Material: "wood", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("46464 grams of wood", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Saw", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Saw tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_door_stp_shutter_397", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a shell wood shuttered opening (@renaissance_door_stp_shutter_397)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition LocksmithingForgeBirdCopperStrikePlatePattern2() => new()
	{
		Metadata = new("Locksmithing/forge bird copper strike plate pattern 2", "crafts", ["earlymodern", "renaissance"], ["item:renaissance_door_stp_strike_plate_419"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "forge bird copper strike plate pattern 2",
		Category = "Locksmithing",
		Blurb = "forge a bird copper strike plate",
		Action = "forging a loose lock, key, latch, or fitting",
		ActiveCraftItemSdesc = "an in-progress bird copper strike plate craft",
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
			new CatalogueCommodityRecipeInput(Grams: 828.0, Material: "copper", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("828 grams of copper", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Anvil", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Anvil tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_door_stp_strike_plate_419", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a bird copper strike plate (@renaissance_door_stp_strike_plate_419)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition LocksmithingForgeBraidedWroughtIronWardedKeyPattern2() => new()
	{
		Metadata = new("Locksmithing/forge braided wrought iron warded key pattern 2", "crafts", ["earlymodern", "renaissance"], ["item:renaissance_door_stp_warded_key_411"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "forge braided wrought iron warded key pattern 2",
		Category = "Locksmithing",
		Blurb = "forge a braided wrought iron warded key",
		Action = "forging a loose lock, key, latch, or fitting",
		ActiveCraftItemSdesc = "an in-progress braided wrought iron warded key craft",
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
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_door_stp_warded_key_411", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a braided wrought iron warded key (@renaissance_door_stp_warded_key_411)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition CarpentryAndJoineryBuildWaterWaveWroughtIronWarehouseGatePattern2() => new()
	{
		Metadata = new("Carpentry and joinery/build water-wave wrought iron warehouse gate pattern 2", "crafts", ["earlymodern", "renaissance"], ["item:renaissance_door_stp_warehouse_gate_401"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "build water-wave wrought iron warehouse gate pattern 2",
		Category = "Carpentry and joinery",
		Blurb = "build a water-wave wrought iron warehouse gate",
		Action = "building a door, gate, shutter, screen, grille, or barrier",
		ActiveCraftItemSdesc = "an in-progress water-wave wrought iron warehouse gate craft",
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
			new CatalogueCommodityRecipeInput(Grams: 22704.0, Material: "wrought iron", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("22704 grams of wrought iron", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Saw", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Saw tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_door_stp_warehouse_gate_401", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a water-wave wrought iron warehouse gate (@renaissance_door_stp_warehouse_gate_401)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition CarpentryAndJoineryBuildRosetteWroughtIronWicketGate() => new()
	{
		Metadata = new("Carpentry and joinery/build rosette wrought iron wicket gate", "crafts", ["earlymodern", "renaissance"], ["item:renaissance_door_stp_wicket_406"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "build rosette wrought iron wicket gate",
		Category = "Carpentry and joinery",
		Blurb = "build a rosette wrought iron wicket gate",
		Action = "building a door, gate, shutter, screen, grille, or barrier",
		ActiveCraftItemSdesc = "an in-progress rosette wrought iron wicket gate craft",
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
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_door_stp_wicket_406", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a rosette wrought iron wicket gate (@renaissance_door_stp_wicket_406)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition CarpentryAndJoineryBuildWaterWaveWroughtIronBarredBarrier() => new()
	{
		Metadata = new("Carpentry and joinery/build water-wave wrought iron barred barrier", "crafts", ["earlymodern", "renaissance"], ["item:renaissance_door_wer_barrier_12"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "build water-wave wrought iron barred barrier",
		Category = "Carpentry and joinery",
		Blurb = "build a water-wave wrought iron barred barrier",
		Action = "building a door, gate, shutter, screen, grille, or barrier",
		ActiveCraftItemSdesc = "an in-progress water-wave wrought iron barred barrier craft",
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
			new CatalogueCommodityRecipeInput(Grams: 38544.0, Material: "wrought iron", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("38544 grams of wrought iron", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Saw", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Saw tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_door_wer_barrier_12", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a water-wave wrought iron barred barrier (@renaissance_door_wer_barrier_12)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition CarpentryAndJoineryBuildBraidedWoodBracedDoorPattern2() => new()
	{
		Metadata = new("Carpentry and joinery/build braided wood braced door pattern 2", "crafts", ["earlymodern", "renaissance"], ["item:renaissance_door_wer_braced_door_02"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "build braided wood braced door pattern 2",
		Category = "Carpentry and joinery",
		Blurb = "build a braided wood braced door",
		Action = "building a door, gate, shutter, screen, grille, or barrier",
		ActiveCraftItemSdesc = "an in-progress braided wood braced door craft",
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
			new CatalogueCommodityRecipeInput(Grams: 22704.0, Material: "wood", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("22704 grams of wood", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Saw", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Saw tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_door_wer_braced_door_02", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a braided wood braced door (@renaissance_door_wer_braced_door_02)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition CarpentryAndJoineryBuildShellTeakBracedDoorPattern2() => new()
	{
		Metadata = new("Carpentry and joinery/build shell teak braced door pattern 2", "crafts", ["earlymodern", "renaissance"], ["item:renaissance_door_wer_braced_door_16"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "build shell teak braced door pattern 2",
		Category = "Carpentry and joinery",
		Blurb = "build a shell teak braced door",
		Action = "building a door, gate, shutter, screen, grille, or barrier",
		ActiveCraftItemSdesc = "an in-progress shell teak braced door craft",
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
			new CatalogueCommodityRecipeInput(Grams: 44880.0, Material: "teak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("44880 grams of teak", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Saw", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Saw tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_door_wer_braced_door_16", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a shell teak braced door (@renaissance_door_wer_braced_door_16)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition CarpentryAndJoineryBuildShellOakCourtyardGate() => new()
	{
		Metadata = new("Carpentry and joinery/build shell oak courtyard gate", "crafts", ["earlymodern", "renaissance"], ["item:renaissance_door_wer_courtyard_gate_08"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "build shell oak courtyard gate",
		Category = "Carpentry and joinery",
		Blurb = "build a shell oak courtyard gate",
		Action = "building a door, gate, shutter, screen, grille, or barrier",
		ActiveCraftItemSdesc = "an in-progress shell oak courtyard gate craft",
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
			new CatalogueCommodityRecipeInput(Grams: 32208.0, Material: "oak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("32208 grams of oak", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Saw", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Saw tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_door_wer_courtyard_gate_08", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a shell oak courtyard gate (@renaissance_door_wer_courtyard_gate_08)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition LocksmithingForgeBraidedBrassDoorBarPattern4() => new()
	{
		Metadata = new("Locksmithing/forge braided brass door bar pattern 4", "crafts", ["earlymodern", "renaissance"], ["item:renaissance_door_wer_door_bar_23"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "forge braided brass door bar pattern 4",
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
			new CatalogueCommodityRecipeInput(Grams: 480.0, Material: "brass", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("480 grams of brass", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Anvil", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Anvil tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_door_wer_door_bar_23", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a braided brass door bar (@renaissance_door_wer_door_bar_23)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition LocksmithingForgeRosetteWroughtIronDoorLatchPattern4() => new()
	{
		Metadata = new("Locksmithing/forge rosette wrought iron door latch pattern 4", "crafts", ["earlymodern", "renaissance"], ["item:renaissance_door_wer_door_latch_21"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "forge rosette wrought iron door latch pattern 4",
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
			new CatalogueCommodityRecipeInput(Grams: 340.0, Material: "wrought iron", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("340 grams of wrought iron", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Anvil", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Anvil tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_door_wer_door_latch_21", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a rosette wrought iron door latch (@renaissance_door_wer_door_latch_21)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition CarpentryAndJoineryBuildBraidedTeakGardenGate() => new()
	{
		Metadata = new("Carpentry and joinery/build braided teak garden gate", "crafts", ["earlymodern", "renaissance"], ["item:renaissance_door_wer_garden_gate_10"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "build braided teak garden gate",
		Category = "Carpentry and joinery",
		Blurb = "build a braided teak garden gate",
		Action = "building a door, gate, shutter, screen, grille, or barrier",
		ActiveCraftItemSdesc = "an in-progress braided teak garden gate craft",
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
			new CatalogueCommodityRecipeInput(Grams: 35376.0, Material: "teak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("35376 grams of teak", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Saw", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Saw tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_door_wer_garden_gate_10", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a braided teak garden gate (@renaissance_door_wer_garden_gate_10)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition LocksmithingForgeCheckerBronzeGateDropBarPattern4() => new()
	{
		Metadata = new("Locksmithing/forge checker bronze gate drop bar pattern 4", "crafts", ["earlymodern", "renaissance"], ["item:renaissance_door_wer_gate_dropbar_22"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "forge checker bronze gate drop bar pattern 4",
		Category = "Locksmithing",
		Blurb = "forge a checker bronze gate drop bar",
		Action = "forging a loose lock, key, latch, or fitting",
		ActiveCraftItemSdesc = "an in-progress checker bronze gate drop bar craft",
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
			new CatalogueCommodityRecipeInput(Grams: 410.0, Material: "bronze", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("410 grams of bronze", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Anvil", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Anvil tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_door_wer_gate_dropbar_22", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a checker bronze gate drop bar (@renaissance_door_wer_gate_dropbar_22)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition CarpentryAndJoineryBuildLeafScrollBrassMetalGrillePattern2() => new()
	{
		Metadata = new("Carpentry and joinery/build leaf-scroll brass metal grille pattern 2", "crafts", ["earlymodern", "renaissance"], ["item:renaissance_door_wer_grille_11"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "build leaf-scroll brass metal grille pattern 2",
		Category = "Carpentry and joinery",
		Blurb = "build a leaf-scroll brass metal grille",
		Action = "building a door, gate, shutter, screen, grille, or barrier",
		ActiveCraftItemSdesc = "an in-progress leaf-scroll brass metal grille craft",
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
			new CatalogueCommodityRecipeInput(Grams: 36960.0, Material: "brass", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("36960 grams of brass", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Saw", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Saw tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_door_wer_grille_11", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a leaf-scroll brass metal grille (@renaissance_door_wer_grille_11)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition LocksmithingForgeLeafScrollPewterIronHaspPattern4() => new()
	{
		Metadata = new("Locksmithing/forge leaf-scroll pewter iron hasp pattern 4", "crafts", ["earlymodern", "renaissance"], ["item:renaissance_door_wer_hasp_25"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "forge leaf-scroll pewter iron hasp pattern 4",
		Category = "Locksmithing",
		Blurb = "forge a leaf-scroll pewter iron hasp",
		Action = "forging a loose lock, key, latch, or fitting",
		ActiveCraftItemSdesc = "an in-progress leaf-scroll pewter iron hasp craft",
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
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_door_wer_hasp_25", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a leaf-scroll pewter iron hasp (@renaissance_door_wer_hasp_25)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition CarpentryAndJoineryBuildFlowerHeadOakServiceHatch() => new()
	{
		Metadata = new("Carpentry and joinery/build flower-head oak service hatch", "crafts", ["earlymodern", "renaissance"], ["item:renaissance_door_wer_hatch_13"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "build flower-head oak service hatch",
		Category = "Carpentry and joinery",
		Blurb = "build a flower-head oak service hatch",
		Action = "building a door, gate, shutter, screen, grille, or barrier",
		ActiveCraftItemSdesc = "an in-progress flower-head oak service hatch craft",
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
			new CatalogueCommodityRecipeInput(Grams: 40128.0, Material: "oak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("40128 grams of oak", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Saw", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Saw tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_door_wer_hatch_13", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a flower-head oak service hatch (@renaissance_door_wer_hatch_13)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition LocksmithingForgeKnottedBrassHingePairPattern5() => new()
	{
		Metadata = new("Locksmithing/forge knotted brass hinge pair pattern 5", "crafts", ["earlymodern", "renaissance"], ["item:renaissance_door_wer_hinge_pair_28"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "forge knotted brass hinge pair pattern 5",
		Category = "Locksmithing",
		Blurb = "forge a knotted brass hinge pair",
		Action = "forging a loose lock, key, latch, or fitting",
		ActiveCraftItemSdesc = "an in-progress knotted brass hinge pair craft",
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
			new CatalogueCommodityRecipeInput(Grams: 828.0, Material: "brass", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("828 grams of brass", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Anvil", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Anvil tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_door_wer_hinge_pair_28", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a knotted brass hinge pair (@renaissance_door_wer_hinge_pair_28)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition LocksmithingForgeSunburstWroughtIronHookLatchPattern4() => new()
	{
		Metadata = new("Locksmithing/forge sunburst wrought iron hook latch pattern 4", "crafts", ["earlymodern", "renaissance"], ["item:renaissance_door_wer_hook_latch_26"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "forge sunburst wrought iron hook latch pattern 4",
		Category = "Locksmithing",
		Blurb = "forge a sunburst wrought iron hook latch",
		Action = "forging a loose lock, key, latch, or fitting",
		ActiveCraftItemSdesc = "an in-progress sunburst wrought iron hook latch craft",
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
			new CatalogueCommodityRecipeInput(Grams: 689.0, Material: "wrought iron", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("689 grams of wrought iron", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Anvil", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Anvil tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_door_wer_hook_latch_26", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a sunburst wrought iron hook latch (@renaissance_door_wer_hook_latch_26)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition CarpentryAndJoineryBuildGeometricBronzeLatticeGate() => new()
	{
		Metadata = new("Carpentry and joinery/build geometric bronze lattice gate", "crafts", ["earlymodern", "renaissance"], ["item:renaissance_door_wer_lattice_gate_07"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "build geometric bronze lattice gate",
		Category = "Carpentry and joinery",
		Blurb = "build a geometric bronze lattice gate",
		Action = "building a door, gate, shutter, screen, grille, or barrier",
		ActiveCraftItemSdesc = "an in-progress geometric bronze lattice gate craft",
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
			new CatalogueCommodityRecipeInput(Grams: 30624.0, Material: "bronze", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("30624 grams of bronze", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Saw", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Saw tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_door_wer_lattice_gate_07", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a geometric bronze lattice gate (@renaissance_door_wer_lattice_gate_07)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition CarpentryAndJoineryBuildLeafScrollOakLockableDoorPattern2() => new()
	{
		Metadata = new("Carpentry and joinery/build leaf-scroll oak lockable door pattern 2", "crafts", ["earlymodern", "renaissance"], ["item:renaissance_door_wer_lockable_door_03"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "build leaf-scroll oak lockable door pattern 2",
		Category = "Carpentry and joinery",
		Blurb = "build a leaf-scroll oak lockable door",
		Action = "building a door, gate, shutter, screen, grille, or barrier",
		ActiveCraftItemSdesc = "an in-progress leaf-scroll oak lockable door craft",
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
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_door_wer_lockable_door_03", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a leaf-scroll oak lockable door (@renaissance_door_wer_lockable_door_03)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition CarpentryAndJoineryBuildRosetteWoodLockableDoorPattern2() => new()
	{
		Metadata = new("Carpentry and joinery/build rosette wood lockable door pattern 2", "crafts", ["earlymodern", "renaissance"], ["item:renaissance_door_wer_lockable_door_17"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "build rosette wood lockable door pattern 2",
		Category = "Carpentry and joinery",
		Blurb = "build a rosette wood lockable door",
		Action = "building a door, gate, shutter, screen, grille, or barrier",
		ActiveCraftItemSdesc = "an in-progress rosette wood lockable door craft",
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
			new CatalogueCommodityRecipeInput(Grams: 46464.0, Material: "wood", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("46464 grams of wood", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Saw", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Saw tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_door_wer_lockable_door_17", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a rosette wood lockable door (@renaissance_door_wer_lockable_door_17)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition LocksmithingForgeCloudBrassMasterWardedLockPattern4() => new()
	{
		Metadata = new("Locksmithing/forge cloud brass master warded lock pattern 4", "crafts", ["earlymodern", "renaissance"], ["item:renaissance_door_wer_master_warded_lock_18"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "forge cloud brass master warded lock pattern 4",
		Category = "Locksmithing",
		Blurb = "forge a cloud brass master warded lock",
		Action = "forging a loose lock, key, latch, or fitting",
		ActiveCraftItemSdesc = "an in-progress cloud brass master warded lock craft",
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
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_door_wer_master_warded_lock_18", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a cloud brass master warded lock (@renaissance_door_wer_master_warded_lock_18)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition CarpentryAndJoineryBuildRosetteTeakDoorLeafPattern2() => new()
	{
		Metadata = new("Carpentry and joinery/build rosette teak door leaf pattern 2", "crafts", ["earlymodern", "renaissance"], ["item:renaissance_door_wer_panel_door_01"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "build rosette teak door leaf pattern 2",
		Category = "Carpentry and joinery",
		Blurb = "build a rosette teak door leaf",
		Action = "building a door, gate, shutter, screen, grille, or barrier",
		ActiveCraftItemSdesc = "an in-progress rosette teak door leaf craft",
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
			new CatalogueCommodityRecipeInput(Grams: 21120.0, Material: "teak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("21120 grams of teak", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Saw", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Saw tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_door_wer_panel_door_01", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a rosette teak door leaf (@renaissance_door_wer_panel_door_01)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition CarpentryAndJoineryBuildGeometricCedarDoorLeafPattern2() => new()
	{
		Metadata = new("Carpentry and joinery/build geometric cedar door leaf pattern 2", "crafts", ["earlymodern", "renaissance"], ["item:renaissance_door_wer_panel_door_15"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "build geometric cedar door leaf pattern 2",
		Category = "Carpentry and joinery",
		Blurb = "build a geometric cedar door leaf",
		Action = "building a door, gate, shutter, screen, grille, or barrier",
		ActiveCraftItemSdesc = "an in-progress geometric cedar door leaf craft",
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
			new CatalogueCommodityRecipeInput(Grams: 43296.0, Material: "cedar", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("43296 grams of cedar", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Saw", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Saw tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_door_wer_panel_door_15", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a geometric cedar door leaf (@renaissance_door_wer_panel_door_15)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition LocksmithingForgeRadiatingCopperWinchPawlPattern4() => new()
	{
		Metadata = new("Locksmithing/forge radiating copper winch pawl pattern 4", "crafts", ["earlymodern", "renaissance"], ["item:renaissance_door_wer_portcullis_pawl_24"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "forge radiating copper winch pawl pattern 4",
		Category = "Locksmithing",
		Blurb = "forge a radiating copper winch pawl",
		Action = "forging a loose lock, key, latch, or fitting",
		ActiveCraftItemSdesc = "an in-progress radiating copper winch pawl craft",
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
			new CatalogueCommodityRecipeInput(Grams: 549.0, Material: "copper", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("549 grams of copper", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Anvil", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Anvil tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_door_wer_portcullis_pawl_24", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a radiating copper winch pawl (@renaissance_door_wer_portcullis_pawl_24)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition LocksmithingForgeReedPewterRingBowKeyPattern4() => new()
	{
		Metadata = new("Locksmithing/forge reed pewter ring bow key pattern 4", "crafts", ["earlymodern", "renaissance"], ["item:renaissance_door_wer_ring_key_20"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "forge reed pewter ring bow key pattern 4",
		Category = "Locksmithing",
		Blurb = "forge a reed pewter ring bow key",
		Action = "forging a loose lock, key, latch, or fitting",
		ActiveCraftItemSdesc = "an in-progress reed pewter ring bow key craft",
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
			new CatalogueCommodityRecipeInput(Grams: 271.0, Material: "pewter", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("271 grams of pewter", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Anvil", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Anvil tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_door_wer_ring_key_20", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a reed pewter ring bow key (@renaissance_door_wer_ring_key_20)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition CarpentryAndJoineryBuildBirdTeakScreenDoorPattern2() => new()
	{
		Metadata = new("Carpentry and joinery/build bird teak screen door pattern 2", "crafts", ["earlymodern", "renaissance"], ["item:renaissance_door_wer_screen_06"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "build bird teak screen door pattern 2",
		Category = "Carpentry and joinery",
		Blurb = "build a bird teak screen door",
		Action = "building a door, gate, shutter, screen, grille, or barrier",
		ActiveCraftItemSdesc = "an in-progress bird teak screen door craft",
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
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_door_wer_screen_06", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a bird teak screen door (@renaissance_door_wer_screen_06)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition CarpentryAndJoineryBuildWaterWavePineSecureDoor() => new()
	{
		Metadata = new("Carpentry and joinery/build water-wave pine secure door", "crafts", ["earlymodern", "renaissance"], ["item:renaissance_door_wer_secure_door_04"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "build water-wave pine secure door",
		Category = "Carpentry and joinery",
		Blurb = "build a water-wave pine secure door",
		Action = "building a door, gate, shutter, screen, grille, or barrier",
		ActiveCraftItemSdesc = "an in-progress water-wave pine secure door craft",
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
			new CatalogueCommodityRecipeInput(Grams: 25872.0, Material: "pine", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("25872 grams of pine", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Saw", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Saw tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_door_wer_secure_door_04", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a water-wave pine secure door (@renaissance_door_wer_secure_door_04)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition CarpentryAndJoineryBuildFlowerHeadCedarShutteredOpening() => new()
	{
		Metadata = new("Carpentry and joinery/build flower-head cedar shuttered opening", "crafts", ["earlymodern", "renaissance"], ["item:renaissance_door_wer_shutter_05"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "build flower-head cedar shuttered opening",
		Category = "Carpentry and joinery",
		Blurb = "build a flower-head cedar shuttered opening",
		Action = "building a door, gate, shutter, screen, grille, or barrier",
		ActiveCraftItemSdesc = "an in-progress flower-head cedar shuttered opening craft",
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
			new CatalogueCommodityRecipeInput(Grams: 27456.0, Material: "cedar", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("27456 grams of cedar", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Saw", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Saw tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_door_wer_shutter_05", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a flower-head cedar shuttered opening (@renaissance_door_wer_shutter_05)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition LocksmithingForgeWaterWaveBronzeStrikePlatePattern5() => new()
	{
		Metadata = new("Locksmithing/forge water-wave bronze strike plate pattern 5", "crafts", ["earlymodern", "renaissance"], ["item:renaissance_door_wer_strike_plate_27"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "forge water-wave bronze strike plate pattern 5",
		Category = "Locksmithing",
		Blurb = "forge a water-wave bronze strike plate",
		Action = "forging a loose lock, key, latch, or fitting",
		ActiveCraftItemSdesc = "an in-progress water-wave bronze strike plate craft",
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
			new CatalogueCommodityRecipeInput(Grams: 759.0, Material: "bronze", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("759 grams of bronze", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Anvil", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Anvil tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_door_wer_strike_plate_27", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a water-wave bronze strike plate (@renaissance_door_wer_strike_plate_27)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition LocksmithingForgeShellCopperWardedKeyPattern4() => new()
	{
		Metadata = new("Locksmithing/forge shell copper warded key pattern 4", "crafts", ["earlymodern", "renaissance"], ["item:renaissance_door_wer_warded_key_19"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "forge shell copper warded key pattern 4",
		Category = "Locksmithing",
		Blurb = "forge a shell copper warded key",
		Action = "forging a loose lock, key, latch, or fitting",
		ActiveCraftItemSdesc = "an in-progress shell copper warded key craft",
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
			new CatalogueCommodityRecipeInput(Grams: 201.0, Material: "copper", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("201 grams of copper", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Anvil", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Anvil tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_door_wer_warded_key_19", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a shell copper warded key (@renaissance_door_wer_warded_key_19)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition CarpentryAndJoineryBuildRosetteCedarWarehouseGate() => new()
	{
		Metadata = new("Carpentry and joinery/build rosette cedar warehouse gate", "crafts", ["earlymodern", "renaissance"], ["item:renaissance_door_wer_warehouse_gate_09"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "build rosette cedar warehouse gate",
		Category = "Carpentry and joinery",
		Blurb = "build a rosette cedar warehouse gate",
		Action = "building a door, gate, shutter, screen, grille, or barrier",
		ActiveCraftItemSdesc = "an in-progress rosette cedar warehouse gate craft",
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
			new CatalogueCommodityRecipeInput(Grams: 33792.0, Material: "cedar", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("33792 grams of cedar", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Saw", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Saw tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_door_wer_warehouse_gate_09", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a rosette cedar warehouse gate (@renaissance_door_wer_warehouse_gate_09)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition CarpentryAndJoineryBuildBirdCedarWicketGate() => new()
	{
		Metadata = new("Carpentry and joinery/build bird cedar wicket gate", "crafts", ["earlymodern", "renaissance"], ["item:renaissance_door_wer_wicket_14"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "build bird cedar wicket gate",
		Category = "Carpentry and joinery",
		Blurb = "build a bird cedar wicket gate",
		Action = "building a door, gate, shutter, screen, grille, or barrier",
		ActiveCraftItemSdesc = "an in-progress bird cedar wicket gate craft",
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
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_door_wer_wicket_14", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a bird cedar wicket gate (@renaissance_door_wer_wicket_14)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition JewelleryAndDevotionalWorkMakeReedJadeAnkletPattern2() => new()
	{
		Metadata = new("Jewellery and devotional work/make reed jade anklet pattern 2", "crafts", ["earlymodern", "renaissance"], ["item:renaissance_jewellery_aca_anklet_458"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make reed jade anklet pattern 2",
		Category = "Jewellery and devotional work",
		Blurb = "make a reed jade anklet",
		Action = "making jewellery or a devotional item",
		ActiveCraftItemSdesc = "an in-progress reed jade anklet craft",
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
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_jewellery_aca_anklet_458", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a reed jade anklet (@renaissance_jewellery_aca_anklet_458)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};
}
