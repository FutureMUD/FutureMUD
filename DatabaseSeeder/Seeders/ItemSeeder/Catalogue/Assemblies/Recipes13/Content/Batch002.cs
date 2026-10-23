#nullable enable

using MudSharp.Body;
using MudSharp.GameItems.Inventory.Plans;
using MudSharp.RPG.Checks;

namespace DatabaseSeeder.Seeders;

public static partial class Recipes13Stock
{
	public static CatalogueRecipeDefinition LocksmithingForgeWaterWaveBronzeGateDropBar() => new()
	{
		Metadata = new("Locksmithing/forge water-wave bronze gate drop bar", "crafts", ["earlymodern", "renaissance"], ["item:renaissance_door_common_gate_dropbar_157"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "forge water-wave bronze gate drop bar",
		Category = "Locksmithing",
		Blurb = "forge a water-wave bronze gate drop bar",
		Action = "forging a loose lock, key, latch, or fitting",
		ActiveCraftItemSdesc = "an in-progress water-wave bronze gate drop bar craft",
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
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_door_common_gate_dropbar_157", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a water-wave bronze gate drop bar (@renaissance_door_common_gate_dropbar_157)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition LocksmithingForgeSunburstBronzeGateDropBarPattern2() => new()
	{
		Metadata = new("Locksmithing/forge sunburst bronze gate drop bar pattern 2", "crafts", ["earlymodern", "renaissance"], ["item:renaissance_door_common_gate_dropbar_172"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "forge sunburst bronze gate drop bar pattern 2",
		Category = "Locksmithing",
		Blurb = "forge a sunburst bronze gate drop bar",
		Action = "forging a loose lock, key, latch, or fitting",
		ActiveCraftItemSdesc = "an in-progress sunburst bronze gate drop bar craft",
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
			new CatalogueCommodityRecipeInput(Grams: 201.0, Material: "bronze", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("201 grams of bronze", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Anvil", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Anvil tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_door_common_gate_dropbar_172", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a sunburst bronze gate drop bar (@renaissance_door_common_gate_dropbar_172)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition LocksmithingForgeLeafScrollBronzeGateDropBar() => new()
	{
		Metadata = new("Locksmithing/forge leaf-scroll bronze gate drop bar", "crafts", ["earlymodern", "renaissance"], ["item:renaissance_door_common_gate_dropbar_187"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "forge leaf-scroll bronze gate drop bar",
		Category = "Locksmithing",
		Blurb = "forge a leaf-scroll bronze gate drop bar",
		Action = "forging a loose lock, key, latch, or fitting",
		ActiveCraftItemSdesc = "an in-progress leaf-scroll bronze gate drop bar craft",
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
			new CatalogueCommodityRecipeInput(Grams: 1246.0, Material: "bronze", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1246 grams of bronze", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Anvil", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Anvil tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_door_common_gate_dropbar_187", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a leaf-scroll bronze gate drop bar (@renaissance_door_common_gate_dropbar_187)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition LocksmithingForgeRadiatingBronzeGateDropBar() => new()
	{
		Metadata = new("Locksmithing/forge radiating bronze gate drop bar", "crafts", ["earlymodern", "renaissance"], ["item:renaissance_door_common_gate_dropbar_202"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "forge radiating bronze gate drop bar",
		Category = "Locksmithing",
		Blurb = "forge a radiating bronze gate drop bar",
		Action = "forging a loose lock, key, latch, or fitting",
		ActiveCraftItemSdesc = "an in-progress radiating bronze gate drop bar craft",
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
			new CatalogueCommodityRecipeInput(Grams: 1107.0, Material: "bronze", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1107 grams of bronze", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Anvil", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Anvil tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_door_common_gate_dropbar_202", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a radiating bronze gate drop bar (@renaissance_door_common_gate_dropbar_202)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition LocksmithingForgeBraidedBronzeGateDropBarPattern2() => new()
	{
		Metadata = new("Locksmithing/forge braided bronze gate drop bar pattern 2", "crafts", ["earlymodern", "renaissance"], ["item:renaissance_door_common_gate_dropbar_217"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "forge braided bronze gate drop bar pattern 2",
		Category = "Locksmithing",
		Blurb = "forge a braided bronze gate drop bar",
		Action = "forging a loose lock, key, latch, or fitting",
		ActiveCraftItemSdesc = "an in-progress braided bronze gate drop bar craft",
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
			new CatalogueCommodityRecipeInput(Grams: 968.0, Material: "bronze", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("968 grams of bronze", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Anvil", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Anvil tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_door_common_gate_dropbar_217", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a braided bronze gate drop bar (@renaissance_door_common_gate_dropbar_217)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition LocksmithingForgeCheckerBronzeGateDropBarPattern2() => new()
	{
		Metadata = new("Locksmithing/forge checker bronze gate drop bar pattern 2", "crafts", ["earlymodern", "renaissance"], ["item:renaissance_door_common_gate_dropbar_232"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "forge checker bronze gate drop bar pattern 2",
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
			new CatalogueCommodityRecipeInput(Grams: 828.0, Material: "bronze", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("828 grams of bronze", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Anvil", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Anvil tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_door_common_gate_dropbar_232", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a checker bronze gate drop bar (@renaissance_door_common_gate_dropbar_232)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition LocksmithingForgeCheckerBronzeFineWardedLock() => new()
	{
		Metadata = new("Locksmithing/forge checker bronze fine warded lock", "crafts", ["earlymodern", "renaissance"], ["item:renaissance_door_common_good_warded_lock_152"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "forge checker bronze fine warded lock",
		Category = "Locksmithing",
		Blurb = "forge a checker bronze fine warded lock",
		Action = "forging a loose lock, key, latch, or fitting",
		ActiveCraftItemSdesc = "an in-progress checker bronze fine warded lock craft",
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
			new CatalogueCommodityRecipeInput(Grams: 1177.0, Material: "bronze", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1177 grams of bronze", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Anvil", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Anvil tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_door_common_good_warded_lock_152", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a checker bronze fine warded lock (@renaissance_door_common_good_warded_lock_152)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition LocksmithingForgeRosetteBronzeFineWardedLockPattern3() => new()
	{
		Metadata = new("Locksmithing/forge rosette bronze fine warded lock pattern 3", "crafts", ["earlymodern", "renaissance"], ["item:renaissance_door_common_good_warded_lock_167"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "forge rosette bronze fine warded lock pattern 3",
		Category = "Locksmithing",
		Blurb = "forge a rosette bronze fine warded lock",
		Action = "forging a loose lock, key, latch, or fitting",
		ActiveCraftItemSdesc = "an in-progress rosette bronze fine warded lock craft",
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
			new CatalogueCommodityRecipeInput(Grams: 1037.0, Material: "bronze", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1037 grams of bronze", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Anvil", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Anvil tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_door_common_good_warded_lock_167", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a rosette bronze fine warded lock (@renaissance_door_common_good_warded_lock_167)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition LocksmithingForgeReedBronzeFineWardedLockPattern2() => new()
	{
		Metadata = new("Locksmithing/forge reed bronze fine warded lock pattern 2", "crafts", ["earlymodern", "renaissance"], ["item:renaissance_door_common_good_warded_lock_182"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "forge reed bronze fine warded lock pattern 2",
		Category = "Locksmithing",
		Blurb = "forge a reed bronze fine warded lock",
		Action = "forging a loose lock, key, latch, or fitting",
		ActiveCraftItemSdesc = "an in-progress reed bronze fine warded lock craft",
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
			new CatalogueCommodityRecipeInput(Grams: 898.0, Material: "bronze", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("898 grams of bronze", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Anvil", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Anvil tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_door_common_good_warded_lock_182", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a reed bronze fine warded lock (@renaissance_door_common_good_warded_lock_182)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition LocksmithingForgeShellBronzeFineWardedLockPattern2() => new()
	{
		Metadata = new("Locksmithing/forge shell bronze fine warded lock pattern 2", "crafts", ["earlymodern", "renaissance"], ["item:renaissance_door_common_good_warded_lock_197"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "forge shell bronze fine warded lock pattern 2",
		Category = "Locksmithing",
		Blurb = "forge a shell bronze fine warded lock",
		Action = "forging a loose lock, key, latch, or fitting",
		ActiveCraftItemSdesc = "an in-progress shell bronze fine warded lock craft",
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
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_door_common_good_warded_lock_197", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a shell bronze fine warded lock (@renaissance_door_common_good_warded_lock_197)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition LocksmithingForgeCloudBronzeFineWardedLockPattern2() => new()
	{
		Metadata = new("Locksmithing/forge cloud bronze fine warded lock pattern 2", "crafts", ["earlymodern", "renaissance"], ["item:renaissance_door_common_good_warded_lock_212"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "forge cloud bronze fine warded lock pattern 2",
		Category = "Locksmithing",
		Blurb = "forge a cloud bronze fine warded lock",
		Action = "forging a loose lock, key, latch, or fitting",
		ActiveCraftItemSdesc = "an in-progress cloud bronze fine warded lock craft",
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
			new CatalogueCommodityRecipeInput(Grams: 619.0, Material: "bronze", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("619 grams of bronze", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Anvil", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Anvil tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_door_common_good_warded_lock_212", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a cloud bronze fine warded lock (@renaissance_door_common_good_warded_lock_212)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition LocksmithingForgeGeometricBronzeFineWardedLockPattern2() => new()
	{
		Metadata = new("Locksmithing/forge geometric bronze fine warded lock pattern 2", "crafts", ["earlymodern", "renaissance"], ["item:renaissance_door_common_good_warded_lock_227"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "forge geometric bronze fine warded lock pattern 2",
		Category = "Locksmithing",
		Blurb = "forge a geometric bronze fine warded lock",
		Action = "forging a loose lock, key, latch, or fitting",
		ActiveCraftItemSdesc = "an in-progress geometric bronze fine warded lock craft",
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
			new CatalogueCommodityRecipeInput(Grams: 480.0, Material: "bronze", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("480 grams of bronze", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Anvil", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Anvil tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_door_common_good_warded_lock_227", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a geometric bronze fine warded lock (@renaissance_door_common_good_warded_lock_227)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition CarpentryAndJoineryBuildReedBrassMetalGrillePattern2() => new()
	{
		Metadata = new("Carpentry and joinery/build reed brass metal grille pattern 2", "crafts", ["earlymodern", "renaissance"], ["item:renaissance_door_common_grille_109"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "build reed brass metal grille pattern 2",
		Category = "Carpentry and joinery",
		Blurb = "build a reed brass metal grille",
		Action = "building a door, gate, shutter, screen, grille, or barrier",
		ActiveCraftItemSdesc = "an in-progress reed brass metal grille craft",
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
			new CatalogueCommodityRecipeInput(Grams: 41712.0, Material: "brass", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("41712 grams of brass", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Saw", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Saw tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_door_common_grille_109", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a reed brass metal grille (@renaissance_door_common_grille_109)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition CarpentryAndJoineryBuildVineWroughtIronMetalGrille() => new()
	{
		Metadata = new("Carpentry and joinery/build vine wrought iron metal grille", "crafts", ["earlymodern", "renaissance"], ["item:renaissance_door_common_grille_11"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "build vine wrought iron metal grille",
		Category = "Carpentry and joinery",
		Blurb = "build a vine wrought iron metal grille",
		Action = "building a door, gate, shutter, screen, grille, or barrier",
		ActiveCraftItemSdesc = "an in-progress vine wrought iron metal grille craft",
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
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_door_common_grille_11", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a vine wrought iron metal grille (@renaissance_door_common_grille_11)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition CarpentryAndJoineryBuildVineBronzeMetalGrillePattern3() => new()
	{
		Metadata = new("Carpentry and joinery/build vine bronze metal grille pattern 3", "crafts", ["earlymodern", "renaissance"], ["item:renaissance_door_common_grille_123"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "build vine bronze metal grille pattern 3",
		Category = "Carpentry and joinery",
		Blurb = "build a vine bronze metal grille",
		Action = "building a door, gate, shutter, screen, grille, or barrier",
		ActiveCraftItemSdesc = "an in-progress vine bronze metal grille craft",
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
			new CatalogueCommodityRecipeInput(Grams: 33792.0, Material: "bronze", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("33792 grams of bronze", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Saw", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Saw tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_door_common_grille_123", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a vine bronze metal grille (@renaissance_door_common_grille_123)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition CarpentryAndJoineryBuildKnottedWroughtIronMetalGrille() => new()
	{
		Metadata = new("Carpentry and joinery/build knotted wrought iron metal grille", "crafts", ["earlymodern", "renaissance"], ["item:renaissance_door_common_grille_137"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "build knotted wrought iron metal grille",
		Category = "Carpentry and joinery",
		Blurb = "build a knotted wrought iron metal grille",
		Action = "building a door, gate, shutter, screen, grille, or barrier",
		ActiveCraftItemSdesc = "an in-progress knotted wrought iron metal grille craft",
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
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_door_common_grille_137", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a knotted wrought iron metal grille (@renaissance_door_common_grille_137)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition CarpentryAndJoineryBuildKnottedBrassMetalGrillePattern3() => new()
	{
		Metadata = new("Carpentry and joinery/build knotted brass metal grille pattern 3", "crafts", ["earlymodern", "renaissance"], ["item:renaissance_door_common_grille_25"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "build knotted brass metal grille pattern 3",
		Category = "Carpentry and joinery",
		Blurb = "build a knotted brass metal grille",
		Action = "building a door, gate, shutter, screen, grille, or barrier",
		ActiveCraftItemSdesc = "an in-progress knotted brass metal grille craft",
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
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_door_common_grille_25", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a knotted brass metal grille (@renaissance_door_common_grille_25)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition CarpentryAndJoineryBuildRadiatingBronzeMetalGrille() => new()
	{
		Metadata = new("Carpentry and joinery/build radiating bronze metal grille", "crafts", ["earlymodern", "renaissance"], ["item:renaissance_door_common_grille_39"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "build radiating bronze metal grille",
		Category = "Carpentry and joinery",
		Blurb = "build a radiating bronze metal grille",
		Action = "building a door, gate, shutter, screen, grille, or barrier",
		ActiveCraftItemSdesc = "an in-progress radiating bronze metal grille craft",
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
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_door_common_grille_39", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a radiating bronze metal grille (@renaissance_door_common_grille_39)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition CarpentryAndJoineryBuildReedWroughtIronMetalGrille() => new()
	{
		Metadata = new("Carpentry and joinery/build reed wrought iron metal grille", "crafts", ["earlymodern", "renaissance"], ["item:renaissance_door_common_grille_53"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "build reed wrought iron metal grille",
		Category = "Carpentry and joinery",
		Blurb = "build a reed wrought iron metal grille",
		Action = "building a door, gate, shutter, screen, grille, or barrier",
		ActiveCraftItemSdesc = "an in-progress reed wrought iron metal grille craft",
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
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_door_common_grille_53", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a reed wrought iron metal grille (@renaissance_door_common_grille_53)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition CarpentryAndJoineryBuildVineBrassMetalGrillePattern3() => new()
	{
		Metadata = new("Carpentry and joinery/build vine brass metal grille pattern 3", "crafts", ["earlymodern", "renaissance"], ["item:renaissance_door_common_grille_67"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "build vine brass metal grille pattern 3",
		Category = "Carpentry and joinery",
		Blurb = "build a vine brass metal grille",
		Action = "building a door, gate, shutter, screen, grille, or barrier",
		ActiveCraftItemSdesc = "an in-progress vine brass metal grille craft",
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
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_door_common_grille_67", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a vine brass metal grille (@renaissance_door_common_grille_67)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition CarpentryAndJoineryBuildKnottedBronzeMetalGrille() => new()
	{
		Metadata = new("Carpentry and joinery/build knotted bronze metal grille", "crafts", ["earlymodern", "renaissance"], ["item:renaissance_door_common_grille_81"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "build knotted bronze metal grille",
		Category = "Carpentry and joinery",
		Blurb = "build a knotted bronze metal grille",
		Action = "building a door, gate, shutter, screen, grille, or barrier",
		ActiveCraftItemSdesc = "an in-progress knotted bronze metal grille craft",
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
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_door_common_grille_81", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a knotted bronze metal grille (@renaissance_door_common_grille_81)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition CarpentryAndJoineryBuildRadiatingWroughtIronMetalGrillePattern4() => new()
	{
		Metadata = new("Carpentry and joinery/build radiating wrought iron metal grille pattern 4", "crafts", ["earlymodern", "renaissance"], ["item:renaissance_door_common_grille_95"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "build radiating wrought iron metal grille pattern 4",
		Category = "Carpentry and joinery",
		Blurb = "build a radiating wrought iron metal grille",
		Action = "building a door, gate, shutter, screen, grille, or barrier",
		ActiveCraftItemSdesc = "an in-progress radiating wrought iron metal grille craft",
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
			new CatalogueCommodityRecipeInput(Grams: 49632.0, Material: "wrought iron", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("49632 grams of wrought iron", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Saw", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Saw tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_door_common_grille_95", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a radiating wrought iron metal grille (@renaissance_door_common_grille_95)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition LocksmithingForgeBirdPewterIronHasp() => new()
	{
		Metadata = new("Locksmithing/forge bird pewter iron hasp", "crafts", ["earlymodern", "renaissance"], ["item:renaissance_door_common_hasp_145"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "forge bird pewter iron hasp",
		Category = "Locksmithing",
		Blurb = "forge a bird pewter iron hasp",
		Action = "forging a loose lock, key, latch, or fitting",
		ActiveCraftItemSdesc = "an in-progress bird pewter iron hasp craft",
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
			new CatalogueCommodityRecipeInput(Grams: 689.0, Material: "pewter", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("689 grams of pewter", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Anvil", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Anvil tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_door_common_hasp_145", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a bird pewter iron hasp (@renaissance_door_common_hasp_145)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition LocksmithingForgeStarPewterIronHasp() => new()
	{
		Metadata = new("Locksmithing/forge star pewter iron hasp", "crafts", ["earlymodern", "renaissance"], ["item:renaissance_door_common_hasp_160"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "forge star pewter iron hasp",
		Category = "Locksmithing",
		Blurb = "forge a star pewter iron hasp",
		Action = "forging a loose lock, key, latch, or fitting",
		ActiveCraftItemSdesc = "an in-progress star pewter iron hasp craft",
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
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_door_common_hasp_160", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a star pewter iron hasp (@renaissance_door_common_hasp_160)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition LocksmithingForgeFlowerHeadPewterIronHaspPattern2() => new()
	{
		Metadata = new("Locksmithing/forge flower-head pewter iron hasp pattern 2", "crafts", ["earlymodern", "renaissance"], ["item:renaissance_door_common_hasp_175"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "forge flower-head pewter iron hasp pattern 2",
		Category = "Locksmithing",
		Blurb = "forge a flower-head pewter iron hasp",
		Action = "forging a loose lock, key, latch, or fitting",
		ActiveCraftItemSdesc = "an in-progress flower-head pewter iron hasp craft",
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
			new CatalogueCommodityRecipeInput(Grams: 410.0, Material: "pewter", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("410 grams of pewter", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Anvil", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Anvil tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_door_common_hasp_175", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a flower-head pewter iron hasp (@renaissance_door_common_hasp_175)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition LocksmithingForgeKnottedPewterIronHasp() => new()
	{
		Metadata = new("Locksmithing/forge knotted pewter iron hasp", "crafts", ["earlymodern", "renaissance"], ["item:renaissance_door_common_hasp_190"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "forge knotted pewter iron hasp",
		Category = "Locksmithing",
		Blurb = "forge a knotted pewter iron hasp",
		Action = "forging a loose lock, key, latch, or fitting",
		ActiveCraftItemSdesc = "an in-progress knotted pewter iron hasp craft",
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
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_door_common_hasp_190", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a knotted pewter iron hasp (@renaissance_door_common_hasp_190)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition LocksmithingForgeWaterWavePewterIronHaspPattern2() => new()
	{
		Metadata = new("Locksmithing/forge water-wave pewter iron hasp pattern 2", "crafts", ["earlymodern", "renaissance"], ["item:renaissance_door_common_hasp_205"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "forge water-wave pewter iron hasp pattern 2",
		Category = "Locksmithing",
		Blurb = "forge a water-wave pewter iron hasp",
		Action = "forging a loose lock, key, latch, or fitting",
		ActiveCraftItemSdesc = "an in-progress water-wave pewter iron hasp craft",
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
			new CatalogueCommodityRecipeInput(Grams: 131.0, Material: "pewter", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("131 grams of pewter", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Anvil", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Anvil tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_door_common_hasp_205", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a water-wave pewter iron hasp (@renaissance_door_common_hasp_205)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition LocksmithingForgeSunburstPewterIronHaspPattern2() => new()
	{
		Metadata = new("Locksmithing/forge sunburst pewter iron hasp pattern 2", "crafts", ["earlymodern", "renaissance"], ["item:renaissance_door_common_hasp_220"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "forge sunburst pewter iron hasp pattern 2",
		Category = "Locksmithing",
		Blurb = "forge a sunburst pewter iron hasp",
		Action = "forging a loose lock, key, latch, or fitting",
		ActiveCraftItemSdesc = "an in-progress sunburst pewter iron hasp craft",
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
			new CatalogueCommodityRecipeInput(Grams: 1177.0, Material: "pewter", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1177 grams of pewter", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Anvil", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Anvil tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_door_common_hasp_220", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a sunburst pewter iron hasp (@renaissance_door_common_hasp_220)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition LocksmithingForgeLeafScrollPewterIronHaspPattern2() => new()
	{
		Metadata = new("Locksmithing/forge leaf-scroll pewter iron hasp pattern 2", "crafts", ["earlymodern", "renaissance"], ["item:renaissance_door_common_hasp_235"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "forge leaf-scroll pewter iron hasp pattern 2",
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
			new CatalogueCommodityRecipeInput(Grams: 1037.0, Material: "pewter", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1037 grams of pewter", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Anvil", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Anvil tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_door_common_hasp_235", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a leaf-scroll pewter iron hasp (@renaissance_door_common_hasp_235)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition CarpentryAndJoineryBuildRadiatingOakServiceHatch() => new()
	{
		Metadata = new("Carpentry and joinery/build radiating oak service hatch", "crafts", ["earlymodern", "renaissance"], ["item:renaissance_door_common_hatch_111"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "build radiating oak service hatch",
		Category = "Carpentry and joinery",
		Blurb = "build a radiating oak service hatch",
		Action = "building a door, gate, shutter, screen, grille, or barrier",
		ActiveCraftItemSdesc = "an in-progress radiating oak service hatch craft",
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
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_door_common_hatch_111", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a radiating oak service hatch (@renaissance_door_common_hatch_111)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition CarpentryAndJoineryBuildReedWoodServiceHatch() => new()
	{
		Metadata = new("Carpentry and joinery/build reed wood service hatch", "crafts", ["earlymodern", "renaissance"], ["item:renaissance_door_common_hatch_125"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "build reed wood service hatch",
		Category = "Carpentry and joinery",
		Blurb = "build a reed wood service hatch",
		Action = "building a door, gate, shutter, screen, grille, or barrier",
		ActiveCraftItemSdesc = "an in-progress reed wood service hatch craft",
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
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_door_common_hatch_125", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a reed wood service hatch (@renaissance_door_common_hatch_125)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition CarpentryAndJoineryBuildReedCedarServiceHatch() => new()
	{
		Metadata = new("Carpentry and joinery/build reed cedar service hatch", "crafts", ["earlymodern", "renaissance"], ["item:renaissance_door_common_hatch_13"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "build reed cedar service hatch",
		Category = "Carpentry and joinery",
		Blurb = "build a reed cedar service hatch",
		Action = "building a door, gate, shutter, screen, grille, or barrier",
		ActiveCraftItemSdesc = "an in-progress reed cedar service hatch craft",
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
			new CatalogueCommodityRecipeInput(Grams: 40128.0, Material: "cedar", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("40128 grams of cedar", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Saw", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Saw tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_door_common_hatch_13", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a reed cedar service hatch (@renaissance_door_common_hatch_13)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition CarpentryAndJoineryBuildVineTeakServiceHatch() => new()
	{
		Metadata = new("Carpentry and joinery/build vine teak service hatch", "crafts", ["earlymodern", "renaissance"], ["item:renaissance_door_common_hatch_139"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "build vine teak service hatch",
		Category = "Carpentry and joinery",
		Blurb = "build a vine teak service hatch",
		Action = "building a door, gate, shutter, screen, grille, or barrier",
		ActiveCraftItemSdesc = "an in-progress vine teak service hatch craft",
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
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_door_common_hatch_139", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a vine teak service hatch (@renaissance_door_common_hatch_139)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition CarpentryAndJoineryBuildVinePineServiceHatch() => new()
	{
		Metadata = new("Carpentry and joinery/build vine pine service hatch", "crafts", ["earlymodern", "renaissance"], ["item:renaissance_door_common_hatch_27"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "build vine pine service hatch",
		Category = "Carpentry and joinery",
		Blurb = "build a vine pine service hatch",
		Action = "building a door, gate, shutter, screen, grille, or barrier",
		ActiveCraftItemSdesc = "an in-progress vine pine service hatch craft",
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
			new CatalogueCommodityRecipeInput(Grams: 32208.0, Material: "pine", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("32208 grams of pine", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Saw", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Saw tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_door_common_hatch_27", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a vine pine service hatch (@renaissance_door_common_hatch_27)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition CarpentryAndJoineryBuildKnottedOakServiceHatch() => new()
	{
		Metadata = new("Carpentry and joinery/build knotted oak service hatch", "crafts", ["earlymodern", "renaissance"], ["item:renaissance_door_common_hatch_41"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "build knotted oak service hatch",
		Category = "Carpentry and joinery",
		Blurb = "build a knotted oak service hatch",
		Action = "building a door, gate, shutter, screen, grille, or barrier",
		ActiveCraftItemSdesc = "an in-progress knotted oak service hatch craft",
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
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_door_common_hatch_41", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a knotted oak service hatch (@renaissance_door_common_hatch_41)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition CarpentryAndJoineryBuildRadiatingWoodServiceHatch() => new()
	{
		Metadata = new("Carpentry and joinery/build radiating wood service hatch", "crafts", ["earlymodern", "renaissance"], ["item:renaissance_door_common_hatch_55"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "build radiating wood service hatch",
		Category = "Carpentry and joinery",
		Blurb = "build a radiating wood service hatch",
		Action = "building a door, gate, shutter, screen, grille, or barrier",
		ActiveCraftItemSdesc = "an in-progress radiating wood service hatch craft",
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
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_door_common_hatch_55", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a radiating wood service hatch (@renaissance_door_common_hatch_55)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition CarpentryAndJoineryBuildReedTeakServiceHatch() => new()
	{
		Metadata = new("Carpentry and joinery/build reed teak service hatch", "crafts", ["earlymodern", "renaissance"], ["item:renaissance_door_common_hatch_69"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "build reed teak service hatch",
		Category = "Carpentry and joinery",
		Blurb = "build a reed teak service hatch",
		Action = "building a door, gate, shutter, screen, grille, or barrier",
		ActiveCraftItemSdesc = "an in-progress reed teak service hatch craft",
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
			new CatalogueCommodityRecipeInput(Grams: 38544.0, Material: "teak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("38544 grams of teak", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Saw", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Saw tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_door_common_hatch_69", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a reed teak service hatch (@renaissance_door_common_hatch_69)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition CarpentryAndJoineryBuildVineCedarServiceHatch() => new()
	{
		Metadata = new("Carpentry and joinery/build vine cedar service hatch", "crafts", ["earlymodern", "renaissance"], ["item:renaissance_door_common_hatch_83"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "build vine cedar service hatch",
		Category = "Carpentry and joinery",
		Blurb = "build a vine cedar service hatch",
		Action = "building a door, gate, shutter, screen, grille, or barrier",
		ActiveCraftItemSdesc = "an in-progress vine cedar service hatch craft",
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
			new CatalogueCommodityRecipeInput(Grams: 30624.0, Material: "cedar", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("30624 grams of cedar", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Saw", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Saw tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_door_common_hatch_83", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a vine cedar service hatch (@renaissance_door_common_hatch_83)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition CarpentryAndJoineryBuildKnottedPineServiceHatch() => new()
	{
		Metadata = new("Carpentry and joinery/build knotted pine service hatch", "crafts", ["earlymodern", "renaissance"], ["item:renaissance_door_common_hatch_97"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "build knotted pine service hatch",
		Category = "Carpentry and joinery",
		Blurb = "build a knotted pine service hatch",
		Action = "building a door, gate, shutter, screen, grille, or barrier",
		ActiveCraftItemSdesc = "an in-progress knotted pine service hatch craft",
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
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_door_common_hatch_97", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a knotted pine service hatch (@renaissance_door_common_hatch_97)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition LocksmithingForgeCloudBrassHingePair() => new()
	{
		Metadata = new("Locksmithing/forge cloud brass hinge pair", "crafts", ["earlymodern", "renaissance"], ["item:renaissance_door_common_hinge_pair_148"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "forge cloud brass hinge pair",
		Category = "Locksmithing",
		Blurb = "forge a cloud brass hinge pair",
		Action = "forging a loose lock, key, latch, or fitting",
		ActiveCraftItemSdesc = "an in-progress cloud brass hinge pair craft",
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
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_door_common_hinge_pair_148", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a cloud brass hinge pair (@renaissance_door_common_hinge_pair_148)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition LocksmithingForgeGeometricBrassHingePairPattern3() => new()
	{
		Metadata = new("Locksmithing/forge geometric brass hinge pair pattern 3", "crafts", ["earlymodern", "renaissance"], ["item:renaissance_door_common_hinge_pair_163"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "forge geometric brass hinge pair pattern 3",
		Category = "Locksmithing",
		Blurb = "forge a geometric brass hinge pair",
		Action = "forging a loose lock, key, latch, or fitting",
		ActiveCraftItemSdesc = "an in-progress geometric brass hinge pair craft",
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
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_door_common_hinge_pair_163", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a geometric brass hinge pair (@renaissance_door_common_hinge_pair_163)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition LocksmithingForgeVineBrassHingePairPattern3() => new()
	{
		Metadata = new("Locksmithing/forge vine brass hinge pair pattern 3", "crafts", ["earlymodern", "renaissance"], ["item:renaissance_door_common_hinge_pair_178"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "forge vine brass hinge pair pattern 3",
		Category = "Locksmithing",
		Blurb = "forge a vine brass hinge pair",
		Action = "forging a loose lock, key, latch, or fitting",
		ActiveCraftItemSdesc = "an in-progress vine brass hinge pair craft",
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
			new CatalogueCommodityRecipeInput(Grams: 619.0, Material: "brass", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("619 grams of brass", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Anvil", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Anvil tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_door_common_hinge_pair_178", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a vine brass hinge pair (@renaissance_door_common_hinge_pair_178)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition LocksmithingForgeBirdBrassHingePairPattern2() => new()
	{
		Metadata = new("Locksmithing/forge bird brass hinge pair pattern 2", "crafts", ["earlymodern", "renaissance"], ["item:renaissance_door_common_hinge_pair_193"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "forge bird brass hinge pair pattern 2",
		Category = "Locksmithing",
		Blurb = "forge a bird brass hinge pair",
		Action = "forging a loose lock, key, latch, or fitting",
		ActiveCraftItemSdesc = "an in-progress bird brass hinge pair craft",
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
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_door_common_hinge_pair_193", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a bird brass hinge pair (@renaissance_door_common_hinge_pair_193)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition LocksmithingForgeStarBrassHingePairPattern2() => new()
	{
		Metadata = new("Locksmithing/forge star brass hinge pair pattern 2", "crafts", ["earlymodern", "renaissance"], ["item:renaissance_door_common_hinge_pair_208"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "forge star brass hinge pair pattern 2",
		Category = "Locksmithing",
		Blurb = "forge a star brass hinge pair",
		Action = "forging a loose lock, key, latch, or fitting",
		ActiveCraftItemSdesc = "an in-progress star brass hinge pair craft",
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
			new CatalogueCommodityRecipeInput(Grams: 340.0, Material: "brass", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("340 grams of brass", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Anvil", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Anvil tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_door_common_hinge_pair_208", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a star brass hinge pair (@renaissance_door_common_hinge_pair_208)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition LocksmithingForgeFlowerHeadBrassHingePairPattern2() => new()
	{
		Metadata = new("Locksmithing/forge flower-head brass hinge pair pattern 2", "crafts", ["earlymodern", "renaissance"], ["item:renaissance_door_common_hinge_pair_223"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "forge flower-head brass hinge pair pattern 2",
		Category = "Locksmithing",
		Blurb = "forge a flower-head brass hinge pair",
		Action = "forging a loose lock, key, latch, or fitting",
		ActiveCraftItemSdesc = "an in-progress flower-head brass hinge pair craft",
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
			new CatalogueCommodityRecipeInput(Grams: 201.0, Material: "brass", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("201 grams of brass", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Anvil", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Anvil tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_door_common_hinge_pair_223", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a flower-head brass hinge pair (@renaissance_door_common_hinge_pair_223)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition LocksmithingForgeKnottedBrassHingePairPattern3() => new()
	{
		Metadata = new("Locksmithing/forge knotted brass hinge pair pattern 3", "crafts", ["earlymodern", "renaissance"], ["item:renaissance_door_common_hinge_pair_238"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "forge knotted brass hinge pair pattern 3",
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
			new CatalogueCommodityRecipeInput(Grams: 1246.0, Material: "brass", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1246 grams of brass", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Anvil", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Anvil tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_door_common_hinge_pair_238", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a knotted brass hinge pair (@renaissance_door_common_hinge_pair_238)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition LocksmithingForgeVineWroughtIronHookLatch() => new()
	{
		Metadata = new("Locksmithing/forge vine wrought iron hook latch", "crafts", ["earlymodern", "renaissance"], ["item:renaissance_door_common_hook_latch_146"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "forge vine wrought iron hook latch",
		Category = "Locksmithing",
		Blurb = "forge a vine wrought iron hook latch",
		Action = "forging a loose lock, key, latch, or fitting",
		ActiveCraftItemSdesc = "an in-progress vine wrought iron hook latch craft",
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
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_door_common_hook_latch_146", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a vine wrought iron hook latch (@renaissance_door_common_hook_latch_146)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition LocksmithingForgeBirdWroughtIronHookLatch() => new()
	{
		Metadata = new("Locksmithing/forge bird wrought iron hook latch", "crafts", ["earlymodern", "renaissance"], ["item:renaissance_door_common_hook_latch_161"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "forge bird wrought iron hook latch",
		Category = "Locksmithing",
		Blurb = "forge a bird wrought iron hook latch",
		Action = "forging a loose lock, key, latch, or fitting",
		ActiveCraftItemSdesc = "an in-progress bird wrought iron hook latch craft",
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
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_door_common_hook_latch_161", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a bird wrought iron hook latch (@renaissance_door_common_hook_latch_161)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition LocksmithingForgeStarWroughtIronHookLatchPattern2() => new()
	{
		Metadata = new("Locksmithing/forge star wrought iron hook latch pattern 2", "crafts", ["earlymodern", "renaissance"], ["item:renaissance_door_common_hook_latch_176"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "forge star wrought iron hook latch pattern 2",
		Category = "Locksmithing",
		Blurb = "forge a star wrought iron hook latch",
		Action = "forging a loose lock, key, latch, or fitting",
		ActiveCraftItemSdesc = "an in-progress star wrought iron hook latch craft",
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
			new CatalogueCommodityRecipeInput(Grams: 480.0, Material: "wrought iron", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("480 grams of wrought iron", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Anvil", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Anvil tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_door_common_hook_latch_176", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a star wrought iron hook latch (@renaissance_door_common_hook_latch_176)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition LocksmithingForgeFlowerHeadWroughtIronHookLatch() => new()
	{
		Metadata = new("Locksmithing/forge flower-head wrought iron hook latch", "crafts", ["earlymodern", "renaissance"], ["item:renaissance_door_common_hook_latch_191"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "forge flower-head wrought iron hook latch",
		Category = "Locksmithing",
		Blurb = "forge a flower-head wrought iron hook latch",
		Action = "forging a loose lock, key, latch, or fitting",
		ActiveCraftItemSdesc = "an in-progress flower-head wrought iron hook latch craft",
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
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_door_common_hook_latch_191", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a flower-head wrought iron hook latch (@renaissance_door_common_hook_latch_191)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition LocksmithingForgeKnottedWroughtIronHookLatchPattern2() => new()
	{
		Metadata = new("Locksmithing/forge knotted wrought iron hook latch pattern 2", "crafts", ["earlymodern", "renaissance"], ["item:renaissance_door_common_hook_latch_206"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "forge knotted wrought iron hook latch pattern 2",
		Category = "Locksmithing",
		Blurb = "forge a knotted wrought iron hook latch",
		Action = "forging a loose lock, key, latch, or fitting",
		ActiveCraftItemSdesc = "an in-progress knotted wrought iron hook latch craft",
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
			new CatalogueCommodityRecipeInput(Grams: 201.0, Material: "wrought iron", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("201 grams of wrought iron", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Anvil", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Anvil tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_door_common_hook_latch_206", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a knotted wrought iron hook latch (@renaissance_door_common_hook_latch_206)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition LocksmithingForgeWaterWaveWroughtIronHookLatchPattern2() => new()
	{
		Metadata = new("Locksmithing/forge water-wave wrought iron hook latch pattern 2", "crafts", ["earlymodern", "renaissance"], ["item:renaissance_door_common_hook_latch_221"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "forge water-wave wrought iron hook latch pattern 2",
		Category = "Locksmithing",
		Blurb = "forge a water-wave wrought iron hook latch",
		Action = "forging a loose lock, key, latch, or fitting",
		ActiveCraftItemSdesc = "an in-progress water-wave wrought iron hook latch craft",
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
			new CatalogueCommodityRecipeInput(Grams: 1246.0, Material: "wrought iron", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1246 grams of wrought iron", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Anvil", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Anvil tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_door_common_hook_latch_221", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a water-wave wrought iron hook latch (@renaissance_door_common_hook_latch_221)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition LocksmithingForgeSunburstWroughtIronHookLatchPattern2() => new()
	{
		Metadata = new("Locksmithing/forge sunburst wrought iron hook latch pattern 2", "crafts", ["earlymodern", "renaissance"], ["item:renaissance_door_common_hook_latch_236"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "forge sunburst wrought iron hook latch pattern 2",
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
			new CatalogueCommodityRecipeInput(Grams: 1107.0, Material: "wrought iron", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1107 grams of wrought iron", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Anvil", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Anvil tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_door_common_hook_latch_236", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a sunburst wrought iron hook latch (@renaissance_door_common_hook_latch_236)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition LocksmithingForgeShellCopperKeyring() => new()
	{
		Metadata = new("Locksmithing/forge shell copper keyring", "crafts", ["earlymodern", "renaissance"], ["item:renaissance_door_common_keyring_149"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "forge shell copper keyring",
		Category = "Locksmithing",
		Blurb = "forge a shell copper keyring",
		Action = "forging a loose lock, key, latch, or fitting",
		ActiveCraftItemSdesc = "an in-progress shell copper keyring craft",
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
			new CatalogueCommodityRecipeInput(Grams: 968.0, Material: "copper", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("968 grams of copper", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Anvil", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Anvil tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_door_common_keyring_149", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a shell copper keyring (@renaissance_door_common_keyring_149)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition LocksmithingForgeCloudCopperKeyringPattern3() => new()
	{
		Metadata = new("Locksmithing/forge cloud copper keyring pattern 3", "crafts", ["earlymodern", "renaissance"], ["item:renaissance_door_common_keyring_164"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "forge cloud copper keyring pattern 3",
		Category = "Locksmithing",
		Blurb = "forge a cloud copper keyring",
		Action = "forging a loose lock, key, latch, or fitting",
		ActiveCraftItemSdesc = "an in-progress cloud copper keyring craft",
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
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_door_common_keyring_164", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a cloud copper keyring (@renaissance_door_common_keyring_164)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition LocksmithingForgeGeometricCopperKeyringPattern2() => new()
	{
		Metadata = new("Locksmithing/forge geometric copper keyring pattern 2", "crafts", ["earlymodern", "renaissance"], ["item:renaissance_door_common_keyring_179"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "forge geometric copper keyring pattern 2",
		Category = "Locksmithing",
		Blurb = "forge a geometric copper keyring",
		Action = "forging a loose lock, key, latch, or fitting",
		ActiveCraftItemSdesc = "an in-progress geometric copper keyring craft",
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
			new CatalogueCommodityRecipeInput(Grams: 689.0, Material: "copper", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("689 grams of copper", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Anvil", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Anvil tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_door_common_keyring_179", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a geometric copper keyring (@renaissance_door_common_keyring_179)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition LocksmithingForgeVineCopperKeyringPattern2() => new()
	{
		Metadata = new("Locksmithing/forge vine copper keyring pattern 2", "crafts", ["earlymodern", "renaissance"], ["item:renaissance_door_common_keyring_194"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "forge vine copper keyring pattern 2",
		Category = "Locksmithing",
		Blurb = "forge a vine copper keyring",
		Action = "forging a loose lock, key, latch, or fitting",
		ActiveCraftItemSdesc = "an in-progress vine copper keyring craft",
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
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_door_common_keyring_194", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a vine copper keyring (@renaissance_door_common_keyring_194)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition LocksmithingForgeBirdCopperKeyringPattern2() => new()
	{
		Metadata = new("Locksmithing/forge bird copper keyring pattern 2", "crafts", ["earlymodern", "renaissance"], ["item:renaissance_door_common_keyring_209"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "forge bird copper keyring pattern 2",
		Category = "Locksmithing",
		Blurb = "forge a bird copper keyring",
		Action = "forging a loose lock, key, latch, or fitting",
		ActiveCraftItemSdesc = "an in-progress bird copper keyring craft",
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
			new CatalogueCommodityRecipeInput(Grams: 410.0, Material: "copper", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("410 grams of copper", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Anvil", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Anvil tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_door_common_keyring_209", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a bird copper keyring (@renaissance_door_common_keyring_209)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition LocksmithingForgeStarCopperKeyringPattern2() => new()
	{
		Metadata = new("Locksmithing/forge star copper keyring pattern 2", "crafts", ["earlymodern", "renaissance"], ["item:renaissance_door_common_keyring_224"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "forge star copper keyring pattern 2",
		Category = "Locksmithing",
		Blurb = "forge a star copper keyring",
		Action = "forging a loose lock, key, latch, or fitting",
		ActiveCraftItemSdesc = "an in-progress star copper keyring craft",
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
			new CatalogueCommodityRecipeInput(Grams: 271.0, Material: "copper", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("271 grams of copper", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Anvil", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Anvil tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_door_common_keyring_224", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a star copper keyring (@renaissance_door_common_keyring_224)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition LocksmithingForgeFlowerHeadCopperKeyringPattern3() => new()
	{
		Metadata = new("Locksmithing/forge flower-head copper keyring pattern 3", "crafts", ["earlymodern", "renaissance"], ["item:renaissance_door_common_keyring_239"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["renaissance", "earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "forge flower-head copper keyring pattern 3",
		Category = "Locksmithing",
		Blurb = "forge a flower-head copper keyring",
		Action = "forging a loose lock, key, latch, or fitting",
		ActiveCraftItemSdesc = "an in-progress flower-head copper keyring craft",
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
			new CatalogueCommodityRecipeInput(Grams: 131.0, Material: "copper", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("131 grams of copper", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Anvil", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Anvil tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_door_common_keyring_239", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a flower-head copper keyring (@renaissance_door_common_keyring_239)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};
}
