#nullable enable

using MudSharp.Body;
using MudSharp.GameItems.Inventory.Plans;
using MudSharp.RPG.Checks;

namespace DatabaseSeeder.Seeders;

public static partial class Recipes10Stock
{
	public static CatalogueRecipeDefinition CarpentryAndJoineryBuildSunburstBrassMetalGrillePattern2() => new()
	{
		Metadata = new("Carpentry and joinery/build sunburst brass metal grille pattern 2", "crafts", ["earlymodern", "renaissance"], ["item:earlymodern_door_ott_grille_182"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "build sunburst brass metal grille pattern 2",
		Category = "Carpentry and joinery",
		Blurb = "build a sunburst brass metal grille",
		Action = "building a door, gate, shutter, screen, grille, or barrier",
		ActiveCraftItemSdesc = "an in-progress sunburst brass metal grille craft",
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
			new CatalogueItemRecipeProduct(ItemStableReference: "earlymodern_door_ott_grille_182", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a sunburst brass metal grille (@earlymodern_door_ott_grille_182)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition LocksmithingForgeStarCopperHingePair() => new()
	{
		Metadata = new("Locksmithing/forge star copper hinge pair", "crafts", ["earlymodern", "renaissance"], ["item:earlymodern_door_ott_hinge_pair_184"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "forge star copper hinge pair",
		Category = "Locksmithing",
		Blurb = "forge a star copper hinge pair",
		Action = "forging a loose lock, key, latch, or fitting",
		ActiveCraftItemSdesc = "an in-progress star copper hinge pair craft",
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
			new CatalogueCommodityRecipeInput(Grams: 1037.0, Material: "copper", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1037 grams of copper", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Anvil", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Anvil tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "earlymodern_door_ott_hinge_pair_184", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a star copper hinge pair (@earlymodern_door_ott_hinge_pair_184)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition LocksmithingForgeBirdPewterKeyring() => new()
	{
		Metadata = new("Locksmithing/forge bird pewter keyring", "crafts", ["earlymodern", "renaissance"], ["item:earlymodern_door_ott_keyring_185"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "forge bird pewter keyring",
		Category = "Locksmithing",
		Blurb = "forge a bird pewter keyring",
		Action = "forging a loose lock, key, latch, or fitting",
		ActiveCraftItemSdesc = "an in-progress bird pewter keyring craft",
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
			new CatalogueItemRecipeProduct(ItemStableReference: "earlymodern_door_ott_keyring_185", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a bird pewter keyring (@earlymodern_door_ott_keyring_185)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition CarpentryAndJoineryBuildCloudBronzeLatticeGate() => new()
	{
		Metadata = new("Carpentry and joinery/build cloud bronze lattice gate", "crafts", ["earlymodern", "renaissance"], ["item:earlymodern_door_ott_lattice_gate_178"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "build cloud bronze lattice gate",
		Category = "Carpentry and joinery",
		Blurb = "build a cloud bronze lattice gate",
		Action = "building a door, gate, shutter, screen, grille, or barrier",
		ActiveCraftItemSdesc = "an in-progress cloud bronze lattice gate craft",
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
			new CatalogueItemRecipeProduct(ItemStableReference: "earlymodern_door_ott_lattice_gate_178", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a cloud bronze lattice gate (@earlymodern_door_ott_lattice_gate_178)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition CarpentryAndJoineryBuildSunburstOakLockableDoor() => new()
	{
		Metadata = new("Carpentry and joinery/build sunburst oak lockable door", "crafts", ["earlymodern", "renaissance"], ["item:earlymodern_door_ott_lockable_door_174"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "build sunburst oak lockable door",
		Category = "Carpentry and joinery",
		Blurb = "build a sunburst oak lockable door",
		Action = "building a door, gate, shutter, screen, grille, or barrier",
		ActiveCraftItemSdesc = "an in-progress sunburst oak lockable door craft",
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
			new CatalogueItemRecipeProduct(ItemStableReference: "earlymodern_door_ott_lockable_door_174", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a sunburst oak lockable door (@earlymodern_door_ott_lockable_door_174)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition LocksmithingForgeShellCopperMasterWardedLock() => new()
	{
		Metadata = new("Locksmithing/forge shell copper master warded lock", "crafts", ["earlymodern", "renaissance"], ["item:earlymodern_door_ott_master_warded_lock_189"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "forge shell copper master warded lock",
		Category = "Locksmithing",
		Blurb = "forge a shell copper master warded lock",
		Action = "forging a loose lock, key, latch, or fitting",
		ActiveCraftItemSdesc = "an in-progress shell copper master warded lock craft",
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
			new CatalogueItemRecipeProduct(ItemStableReference: "earlymodern_door_ott_master_warded_lock_189", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a shell copper master warded lock (@earlymodern_door_ott_master_warded_lock_189)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition CarpentryAndJoineryBuildCheckerTeakDoorLeaf() => new()
	{
		Metadata = new("Carpentry and joinery/build checker teak door leaf", "crafts", ["earlymodern", "renaissance"], ["item:earlymodern_door_ott_panel_door_172"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "build checker teak door leaf",
		Category = "Carpentry and joinery",
		Blurb = "build a checker teak door leaf",
		Action = "building a door, gate, shutter, screen, grille, or barrier",
		ActiveCraftItemSdesc = "an in-progress checker teak door leaf craft",
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
			new CatalogueItemRecipeProduct(ItemStableReference: "earlymodern_door_ott_panel_door_172", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a checker teak door leaf (@earlymodern_door_ott_panel_door_172)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition CarpentryAndJoineryBuildVineWoodScreenDoor() => new()
	{
		Metadata = new("Carpentry and joinery/build vine wood screen door", "crafts", ["earlymodern", "renaissance"], ["item:earlymodern_door_ott_screen_177"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "build vine wood screen door",
		Category = "Carpentry and joinery",
		Blurb = "build a vine wood screen door",
		Action = "building a door, gate, shutter, screen, grille, or barrier",
		ActiveCraftItemSdesc = "an in-progress vine wood screen door craft",
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
			new CatalogueCommodityRecipeInput(Grams: 29040.0, Material: "wood", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("29040 grams of wood", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Saw", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Saw tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "earlymodern_door_ott_screen_177", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a vine wood screen door (@earlymodern_door_ott_screen_177)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition CarpentryAndJoineryBuildKnottedPineSecureDoor() => new()
	{
		Metadata = new("Carpentry and joinery/build knotted pine secure door", "crafts", ["earlymodern", "renaissance"], ["item:earlymodern_door_ott_secure_door_175"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "build knotted pine secure door",
		Category = "Carpentry and joinery",
		Blurb = "build a knotted pine secure door",
		Action = "building a door, gate, shutter, screen, grille, or barrier",
		ActiveCraftItemSdesc = "an in-progress knotted pine secure door craft",
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
			new CatalogueItemRecipeProduct(ItemStableReference: "earlymodern_door_ott_secure_door_175", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a knotted pine secure door (@earlymodern_door_ott_secure_door_175)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition CarpentryAndJoineryBuildStarCedarShutteredOpening() => new()
	{
		Metadata = new("Carpentry and joinery/build star cedar shuttered opening", "crafts", ["earlymodern", "renaissance"], ["item:earlymodern_door_ott_shutter_176"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "build star cedar shuttered opening",
		Category = "Carpentry and joinery",
		Blurb = "build a star cedar shuttered opening",
		Action = "building a door, gate, shutter, screen, grille, or barrier",
		ActiveCraftItemSdesc = "an in-progress star cedar shuttered opening craft",
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
			new CatalogueItemRecipeProduct(ItemStableReference: "earlymodern_door_ott_shutter_176", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a star cedar shuttered opening (@earlymodern_door_ott_shutter_176)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition LocksmithingForgeFlowerHeadBrassStrikePlate() => new()
	{
		Metadata = new("Locksmithing/forge flower-head brass strike plate", "crafts", ["earlymodern", "renaissance"], ["item:earlymodern_door_ott_strike_plate_183"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "forge flower-head brass strike plate",
		Category = "Locksmithing",
		Blurb = "forge a flower-head brass strike plate",
		Action = "forging a loose lock, key, latch, or fitting",
		ActiveCraftItemSdesc = "an in-progress flower-head brass strike plate craft",
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
			new CatalogueCommodityRecipeInput(Grams: 968.0, Material: "brass", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("968 grams of brass", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Anvil", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Anvil tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "earlymodern_door_ott_strike_plate_183", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a flower-head brass strike plate (@earlymodern_door_ott_strike_plate_183)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition LocksmithingForgeReedPewterWardedKey() => new()
	{
		Metadata = new("Locksmithing/forge reed pewter warded key", "crafts", ["earlymodern", "renaissance"], ["item:earlymodern_door_ott_warded_key_190"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "forge reed pewter warded key",
		Category = "Locksmithing",
		Blurb = "forge a reed pewter warded key",
		Action = "forging a loose lock, key, latch, or fitting",
		ActiveCraftItemSdesc = "an in-progress reed pewter warded key craft",
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
			new CatalogueItemRecipeProduct(ItemStableReference: "earlymodern_door_ott_warded_key_190", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a reed pewter warded key (@earlymodern_door_ott_warded_key_190)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition LocksmithingForgeGeometricBronzeWardedLock() => new()
	{
		Metadata = new("Locksmithing/forge geometric bronze warded lock", "crafts", ["earlymodern", "renaissance"], ["item:earlymodern_door_ott_warded_lock_187"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "forge geometric bronze warded lock",
		Category = "Locksmithing",
		Blurb = "forge a geometric bronze warded lock",
		Action = "forging a loose lock, key, latch, or fitting",
		ActiveCraftItemSdesc = "an in-progress geometric bronze warded lock craft",
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
			new CatalogueItemRecipeProduct(ItemStableReference: "earlymodern_door_ott_warded_lock_187", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a geometric bronze warded lock (@earlymodern_door_ott_warded_lock_187)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition CarpentryAndJoineryBuildCheckerCedarWarehouseGate() => new()
	{
		Metadata = new("Carpentry and joinery/build checker cedar warehouse gate", "crafts", ["earlymodern", "renaissance"], ["item:earlymodern_door_ott_warehouse_gate_180"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "build checker cedar warehouse gate",
		Category = "Carpentry and joinery",
		Blurb = "build a checker cedar warehouse gate",
		Action = "building a door, gate, shutter, screen, grille, or barrier",
		ActiveCraftItemSdesc = "an in-progress checker cedar warehouse gate craft",
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
			new CatalogueItemRecipeProduct(ItemStableReference: "earlymodern_door_ott_warehouse_gate_180", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a checker cedar warehouse gate (@earlymodern_door_ott_warehouse_gate_180)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition CarpentryAndJoineryBuildLeafScrollOakBracedDoor() => new()
	{
		Metadata = new("Carpentry and joinery/build leaf-scroll oak braced door", "crafts", ["earlymodern", "renaissance"], ["item:earlymodern_door_plh_braced_door_135"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "build leaf-scroll oak braced door",
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
			new CatalogueCommodityRecipeInput(Grams: 22704.0, Material: "oak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("22704 grams of oak", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Saw", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Saw tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "earlymodern_door_plh_braced_door_135", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a leaf-scroll oak braced door (@earlymodern_door_plh_braced_door_135)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition CarpentryAndJoineryBuildRosetteCedarCourtyardGate() => new()
	{
		Metadata = new("Carpentry and joinery/build rosette cedar courtyard gate", "crafts", ["earlymodern", "renaissance"], ["item:earlymodern_door_plh_courtyard_gate_141"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "build rosette cedar courtyard gate",
		Category = "Carpentry and joinery",
		Blurb = "build a rosette cedar courtyard gate",
		Action = "building a door, gate, shutter, screen, grille, or barrier",
		ActiveCraftItemSdesc = "an in-progress rosette cedar courtyard gate craft",
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
			new CatalogueItemRecipeProduct(ItemStableReference: "earlymodern_door_plh_courtyard_gate_141", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a rosette cedar courtyard gate (@earlymodern_door_plh_courtyard_gate_141)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition LocksmithingForgeReedBrassKeyholeEscutcheon() => new()
	{
		Metadata = new("Locksmithing/forge reed brass keyhole escutcheon", "crafts", ["earlymodern", "renaissance"], ["item:earlymodern_door_plh_escutcheon_148"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "forge reed brass keyhole escutcheon",
		Category = "Locksmithing",
		Blurb = "forge a reed brass keyhole escutcheon",
		Action = "forging a loose lock, key, latch, or fitting",
		ActiveCraftItemSdesc = "an in-progress reed brass keyhole escutcheon craft",
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
			new CatalogueItemRecipeProduct(ItemStableReference: "earlymodern_door_plh_escutcheon_148", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a reed brass keyhole escutcheon (@earlymodern_door_plh_escutcheon_148)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition CarpentryAndJoineryBuildLeafScrollWroughtIronGardenGate() => new()
	{
		Metadata = new("Carpentry and joinery/build leaf-scroll wrought iron garden gate", "crafts", ["earlymodern", "renaissance"], ["item:earlymodern_door_plh_garden_gate_143"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "build leaf-scroll wrought iron garden gate",
		Category = "Carpentry and joinery",
		Blurb = "build a leaf-scroll wrought iron garden gate",
		Action = "building a door, gate, shutter, screen, grille, or barrier",
		ActiveCraftItemSdesc = "an in-progress leaf-scroll wrought iron garden gate craft",
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
			new CatalogueCommodityRecipeInput(Grams: 35376.0, Material: "wrought iron", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("35376 grams of wrought iron", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Saw", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Saw tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "earlymodern_door_plh_garden_gate_143", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a leaf-scroll wrought iron garden gate (@earlymodern_door_plh_garden_gate_143)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition LocksmithingForgeCheckerPewterFineWardedLock() => new()
	{
		Metadata = new("Locksmithing/forge checker pewter fine warded lock", "crafts", ["earlymodern", "renaissance"], ["item:earlymodern_door_plh_good_warded_lock_150"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "forge checker pewter fine warded lock",
		Category = "Locksmithing",
		Blurb = "forge a checker pewter fine warded lock",
		Action = "forging a loose lock, key, latch, or fitting",
		ActiveCraftItemSdesc = "an in-progress checker pewter fine warded lock craft",
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
			new CatalogueItemRecipeProduct(ItemStableReference: "earlymodern_door_plh_good_warded_lock_150", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a checker pewter fine warded lock (@earlymodern_door_plh_good_warded_lock_150)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition CarpentryAndJoineryBuildWaterWaveBrassMetalGrille() => new()
	{
		Metadata = new("Carpentry and joinery/build water-wave brass metal grille", "crafts", ["earlymodern", "renaissance"], ["item:earlymodern_door_plh_grille_144"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "build water-wave brass metal grille",
		Category = "Carpentry and joinery",
		Blurb = "build a water-wave brass metal grille",
		Action = "building a door, gate, shutter, screen, grille, or barrier",
		ActiveCraftItemSdesc = "an in-progress water-wave brass metal grille craft",
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
			new CatalogueItemRecipeProduct(ItemStableReference: "earlymodern_door_plh_grille_144", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a water-wave brass metal grille (@earlymodern_door_plh_grille_144)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition LocksmithingForgeCloudWroughtIronHingePair() => new()
	{
		Metadata = new("Locksmithing/forge cloud wrought iron hinge pair", "crafts", ["earlymodern", "renaissance"], ["item:earlymodern_door_plh_hinge_pair_146"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "forge cloud wrought iron hinge pair",
		Category = "Locksmithing",
		Blurb = "forge a cloud wrought iron hinge pair",
		Action = "forging a loose lock, key, latch, or fitting",
		ActiveCraftItemSdesc = "an in-progress cloud wrought iron hinge pair craft",
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
			new CatalogueItemRecipeProduct(ItemStableReference: "earlymodern_door_plh_hinge_pair_146", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a cloud wrought iron hinge pair (@earlymodern_door_plh_hinge_pair_146)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition LocksmithingForgeShellBronzeKeyring() => new()
	{
		Metadata = new("Locksmithing/forge shell bronze keyring", "crafts", ["earlymodern", "renaissance"], ["item:earlymodern_door_plh_keyring_147"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "forge shell bronze keyring",
		Category = "Locksmithing",
		Blurb = "forge a shell bronze keyring",
		Action = "forging a loose lock, key, latch, or fitting",
		ActiveCraftItemSdesc = "an in-progress shell bronze keyring craft",
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
			new CatalogueItemRecipeProduct(ItemStableReference: "earlymodern_door_plh_keyring_147", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a shell bronze keyring (@earlymodern_door_plh_keyring_147)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition CarpentryAndJoineryBuildShellOakLatticeGate() => new()
	{
		Metadata = new("Carpentry and joinery/build shell oak lattice gate", "crafts", ["earlymodern", "renaissance"], ["item:earlymodern_door_plh_lattice_gate_140"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "build shell oak lattice gate",
		Category = "Carpentry and joinery",
		Blurb = "build a shell oak lattice gate",
		Action = "building a door, gate, shutter, screen, grille, or barrier",
		ActiveCraftItemSdesc = "an in-progress shell oak lattice gate craft",
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
			new CatalogueCommodityRecipeInput(Grams: 30624.0, Material: "oak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("30624 grams of oak", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Saw", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Saw tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "earlymodern_door_plh_lattice_gate_140", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a shell oak lattice gate (@earlymodern_door_plh_lattice_gate_140)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition CarpentryAndJoineryBuildWaterWavePineLockableDoor() => new()
	{
		Metadata = new("Carpentry and joinery/build water-wave pine lockable door", "crafts", ["earlymodern", "renaissance"], ["item:earlymodern_door_plh_lockable_door_136"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "build water-wave pine lockable door",
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
			new CatalogueCommodityRecipeInput(Grams: 24288.0, Material: "pine", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("24288 grams of pine", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Saw", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Saw tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "earlymodern_door_plh_lockable_door_136", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a water-wave pine lockable door (@earlymodern_door_plh_lockable_door_136)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition LocksmithingForgeBraidedWroughtIronMasterWardedLock() => new()
	{
		Metadata = new("Locksmithing/forge braided wrought iron master warded lock", "crafts", ["earlymodern", "renaissance"], ["item:earlymodern_door_plh_master_warded_lock_151"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "forge braided wrought iron master warded lock",
		Category = "Locksmithing",
		Blurb = "forge a braided wrought iron master warded lock",
		Action = "forging a loose lock, key, latch, or fitting",
		ActiveCraftItemSdesc = "an in-progress braided wrought iron master warded lock craft",
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
			new CatalogueItemRecipeProduct(ItemStableReference: "earlymodern_door_plh_master_warded_lock_151", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a braided wrought iron master warded lock (@earlymodern_door_plh_master_warded_lock_151)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition CarpentryAndJoineryBuildBraidedWoodDoorLeaf() => new()
	{
		Metadata = new("Carpentry and joinery/build braided wood door leaf", "crafts", ["earlymodern", "renaissance"], ["item:earlymodern_door_plh_panel_door_134"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "build braided wood door leaf",
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
			new CatalogueCommodityRecipeInput(Grams: 21120.0, Material: "wood", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("21120 grams of wood", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Saw", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Saw tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "earlymodern_door_plh_panel_door_134", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a braided wood door leaf (@earlymodern_door_plh_panel_door_134)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition CarpentryAndJoineryBuildGeometricCedarScreenDoor() => new()
	{
		Metadata = new("Carpentry and joinery/build geometric cedar screen door", "crafts", ["earlymodern", "renaissance"], ["item:earlymodern_door_plh_screen_139"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "build geometric cedar screen door",
		Category = "Carpentry and joinery",
		Blurb = "build a geometric cedar screen door",
		Action = "building a door, gate, shutter, screen, grille, or barrier",
		ActiveCraftItemSdesc = "an in-progress geometric cedar screen door craft",
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
			new CatalogueItemRecipeProduct(ItemStableReference: "earlymodern_door_plh_screen_139", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a geometric cedar screen door (@earlymodern_door_plh_screen_139)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition CarpentryAndJoineryBuildFlowerHeadCedarSecureDoor() => new()
	{
		Metadata = new("Carpentry and joinery/build flower-head cedar secure door", "crafts", ["earlymodern", "renaissance"], ["item:earlymodern_door_plh_secure_door_137"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "build flower-head cedar secure door",
		Category = "Carpentry and joinery",
		Blurb = "build a flower-head cedar secure door",
		Action = "building a door, gate, shutter, screen, grille, or barrier",
		ActiveCraftItemSdesc = "an in-progress flower-head cedar secure door craft",
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
			new CatalogueCommodityRecipeInput(Grams: 25872.0, Material: "cedar", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("25872 grams of cedar", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Saw", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Saw tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "earlymodern_door_plh_secure_door_137", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a flower-head cedar secure door (@earlymodern_door_plh_secure_door_137)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition CarpentryAndJoineryBuildBirdTeakShutteredOpening() => new()
	{
		Metadata = new("Carpentry and joinery/build bird teak shuttered opening", "crafts", ["earlymodern", "renaissance"], ["item:earlymodern_door_plh_shutter_138"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "build bird teak shuttered opening",
		Category = "Carpentry and joinery",
		Blurb = "build a bird teak shuttered opening",
		Action = "building a door, gate, shutter, screen, grille, or barrier",
		ActiveCraftItemSdesc = "an in-progress bird teak shuttered opening craft",
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
			new CatalogueCommodityRecipeInput(Grams: 27456.0, Material: "teak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("27456 grams of teak", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Saw", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Saw tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "earlymodern_door_plh_shutter_138", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a bird teak shuttered opening (@earlymodern_door_plh_shutter_138)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition LocksmithingForgeGeometricPewterStrikePlate() => new()
	{
		Metadata = new("Locksmithing/forge geometric pewter strike plate", "crafts", ["earlymodern", "renaissance"], ["item:earlymodern_door_plh_strike_plate_145"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "forge geometric pewter strike plate",
		Category = "Locksmithing",
		Blurb = "forge a geometric pewter strike plate",
		Action = "forging a loose lock, key, latch, or fitting",
		ActiveCraftItemSdesc = "an in-progress geometric pewter strike plate craft",
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
			new CatalogueItemRecipeProduct(ItemStableReference: "earlymodern_door_plh_strike_plate_145", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a geometric pewter strike plate (@earlymodern_door_plh_strike_plate_145)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition LocksmithingForgeRadiatingBronzeWardedKey() => new()
	{
		Metadata = new("Locksmithing/forge radiating bronze warded key", "crafts", ["earlymodern", "renaissance"], ["item:earlymodern_door_plh_warded_key_152"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "forge radiating bronze warded key",
		Category = "Locksmithing",
		Blurb = "forge a radiating bronze warded key",
		Action = "forging a loose lock, key, latch, or fitting",
		ActiveCraftItemSdesc = "an in-progress radiating bronze warded key craft",
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
			new CatalogueItemRecipeProduct(ItemStableReference: "earlymodern_door_plh_warded_key_152", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a radiating bronze warded key (@earlymodern_door_plh_warded_key_152)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition LocksmithingForgeRosetteCopperWardedLock() => new()
	{
		Metadata = new("Locksmithing/forge rosette copper warded lock", "crafts", ["earlymodern", "renaissance"], ["item:earlymodern_door_plh_warded_lock_149"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "forge rosette copper warded lock",
		Category = "Locksmithing",
		Blurb = "forge a rosette copper warded lock",
		Action = "forging a loose lock, key, latch, or fitting",
		ActiveCraftItemSdesc = "an in-progress rosette copper warded lock craft",
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
			new CatalogueItemRecipeProduct(ItemStableReference: "earlymodern_door_plh_warded_lock_149", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a rosette copper warded lock (@earlymodern_door_plh_warded_lock_149)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition CarpentryAndJoineryBuildBraidedTeakWarehouseGate() => new()
	{
		Metadata = new("Carpentry and joinery/build braided teak warehouse gate", "crafts", ["earlymodern", "renaissance"], ["item:earlymodern_door_plh_warehouse_gate_142"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "build braided teak warehouse gate",
		Category = "Carpentry and joinery",
		Blurb = "build a braided teak warehouse gate",
		Action = "building a door, gate, shutter, screen, grille, or barrier",
		ActiveCraftItemSdesc = "an in-progress braided teak warehouse gate craft",
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
			new CatalogueCommodityRecipeInput(Grams: 33792.0, Material: "teak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("33792 grams of teak", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Saw", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Saw tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "earlymodern_door_plh_warehouse_gate_142", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a braided teak warehouse gate (@earlymodern_door_plh_warehouse_gate_142)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition CarpentryAndJoineryBuildBirdPineBracedDoor() => new()
	{
		Metadata = new("Carpentry and joinery/build bird pine braced door", "crafts", ["earlymodern", "renaissance"], ["item:earlymodern_door_qing_braced_door_287"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "build bird pine braced door",
		Category = "Carpentry and joinery",
		Blurb = "build a bird pine braced door",
		Action = "building a door, gate, shutter, screen, grille, or barrier",
		ActiveCraftItemSdesc = "an in-progress bird pine braced door craft",
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
			new CatalogueItemRecipeProduct(ItemStableReference: "earlymodern_door_qing_braced_door_287", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a bird pine braced door (@earlymodern_door_qing_braced_door_287)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition CarpentryAndJoineryBuildWaterWaveTeakCourtyardGate() => new()
	{
		Metadata = new("Carpentry and joinery/build water-wave teak courtyard gate", "crafts", ["earlymodern", "renaissance"], ["item:earlymodern_door_qing_courtyard_gate_293"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "build water-wave teak courtyard gate",
		Category = "Carpentry and joinery",
		Blurb = "build a water-wave teak courtyard gate",
		Action = "building a door, gate, shutter, screen, grille, or barrier",
		ActiveCraftItemSdesc = "an in-progress water-wave teak courtyard gate craft",
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
			new CatalogueItemRecipeProduct(ItemStableReference: "earlymodern_door_qing_courtyard_gate_293", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a water-wave teak courtyard gate (@earlymodern_door_qing_courtyard_gate_293)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition LocksmithingForgeRadiatingPewterKeyholeEscutcheon() => new()
	{
		Metadata = new("Locksmithing/forge radiating pewter keyhole escutcheon", "crafts", ["earlymodern", "renaissance"], ["item:earlymodern_door_qing_escutcheon_300"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "forge radiating pewter keyhole escutcheon",
		Category = "Locksmithing",
		Blurb = "forge a radiating pewter keyhole escutcheon",
		Action = "forging a loose lock, key, latch, or fitting",
		ActiveCraftItemSdesc = "an in-progress radiating pewter keyhole escutcheon craft",
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
			new CatalogueCommodityRecipeInput(Grams: 828.0, Material: "pewter", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("828 grams of pewter", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Anvil", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Anvil tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "earlymodern_door_qing_escutcheon_300", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a radiating pewter keyhole escutcheon (@earlymodern_door_qing_escutcheon_300)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition CarpentryAndJoineryBuildBirdBronzeGardenGate() => new()
	{
		Metadata = new("Carpentry and joinery/build bird bronze garden gate", "crafts", ["earlymodern", "renaissance"], ["item:earlymodern_door_qing_garden_gate_295"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "build bird bronze garden gate",
		Category = "Carpentry and joinery",
		Blurb = "build a bird bronze garden gate",
		Action = "building a door, gate, shutter, screen, grille, or barrier",
		ActiveCraftItemSdesc = "an in-progress bird bronze garden gate craft",
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
			new CatalogueItemRecipeProduct(ItemStableReference: "earlymodern_door_qing_garden_gate_295", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a bird bronze garden gate (@earlymodern_door_qing_garden_gate_295)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition LocksmithingForgeSunburstBronzeFineWardedLock() => new()
	{
		Metadata = new("Locksmithing/forge sunburst bronze fine warded lock", "crafts", ["earlymodern", "renaissance"], ["item:earlymodern_door_qing_good_warded_lock_302"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "forge sunburst bronze fine warded lock",
		Category = "Locksmithing",
		Blurb = "forge a sunburst bronze fine warded lock",
		Action = "forging a loose lock, key, latch, or fitting",
		ActiveCraftItemSdesc = "an in-progress sunburst bronze fine warded lock craft",
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
			new CatalogueItemRecipeProduct(ItemStableReference: "earlymodern_door_qing_good_warded_lock_302", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a sunburst bronze fine warded lock (@earlymodern_door_qing_good_warded_lock_302)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition CarpentryAndJoineryBuildGeometricBrassMetalGrillePattern2() => new()
	{
		Metadata = new("Carpentry and joinery/build geometric brass metal grille pattern 2", "crafts", ["earlymodern", "renaissance"], ["item:earlymodern_door_qing_grille_296"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "build geometric brass metal grille pattern 2",
		Category = "Carpentry and joinery",
		Blurb = "build a geometric brass metal grille",
		Action = "building a door, gate, shutter, screen, grille, or barrier",
		ActiveCraftItemSdesc = "an in-progress geometric brass metal grille craft",
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
			new CatalogueItemRecipeProduct(ItemStableReference: "earlymodern_door_qing_grille_296", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a geometric brass metal grille (@earlymodern_door_qing_grille_296)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition LocksmithingForgeCheckerBrassHingePair() => new()
	{
		Metadata = new("Locksmithing/forge checker brass hinge pair", "crafts", ["earlymodern", "renaissance"], ["item:earlymodern_door_qing_hinge_pair_298"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "forge checker brass hinge pair",
		Category = "Locksmithing",
		Blurb = "forge a checker brass hinge pair",
		Action = "forging a loose lock, key, latch, or fitting",
		ActiveCraftItemSdesc = "an in-progress checker brass hinge pair craft",
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
			new CatalogueCommodityRecipeInput(Grams: 689.0, Material: "brass", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("689 grams of brass", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Anvil", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Anvil tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "earlymodern_door_qing_hinge_pair_298", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a checker brass hinge pair (@earlymodern_door_qing_hinge_pair_298)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition LocksmithingForgeBraidedCopperKeyring() => new()
	{
		Metadata = new("Locksmithing/forge braided copper keyring", "crafts", ["earlymodern", "renaissance"], ["item:earlymodern_door_qing_keyring_299"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "forge braided copper keyring",
		Category = "Locksmithing",
		Blurb = "forge a braided copper keyring",
		Action = "forging a loose lock, key, latch, or fitting",
		ActiveCraftItemSdesc = "an in-progress braided copper keyring craft",
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
			new CatalogueCommodityRecipeInput(Grams: 759.0, Material: "copper", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("759 grams of copper", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Anvil", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Anvil tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "earlymodern_door_qing_keyring_299", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a braided copper keyring (@earlymodern_door_qing_keyring_299)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition CarpentryAndJoineryBuildLeafScrollCedarLatticeGate() => new()
	{
		Metadata = new("Carpentry and joinery/build leaf-scroll cedar lattice gate", "crafts", ["earlymodern", "renaissance"], ["item:earlymodern_door_qing_lattice_gate_292"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "build leaf-scroll cedar lattice gate",
		Category = "Carpentry and joinery",
		Blurb = "build a leaf-scroll cedar lattice gate",
		Action = "building a door, gate, shutter, screen, grille, or barrier",
		ActiveCraftItemSdesc = "an in-progress leaf-scroll cedar lattice gate craft",
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
			new CatalogueItemRecipeProduct(ItemStableReference: "earlymodern_door_qing_lattice_gate_292", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a leaf-scroll cedar lattice gate (@earlymodern_door_qing_lattice_gate_292)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition CarpentryAndJoineryBuildGeometricCedarLockableDoor() => new()
	{
		Metadata = new("Carpentry and joinery/build geometric cedar lockable door", "crafts", ["earlymodern", "renaissance"], ["item:earlymodern_door_qing_lockable_door_288"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "build geometric cedar lockable door",
		Category = "Carpentry and joinery",
		Blurb = "build a geometric cedar lockable door",
		Action = "building a door, gate, shutter, screen, grille, or barrier",
		ActiveCraftItemSdesc = "an in-progress geometric cedar lockable door craft",
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
			new CatalogueItemRecipeProduct(ItemStableReference: "earlymodern_door_qing_lockable_door_288", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a geometric cedar lockable door (@earlymodern_door_qing_lockable_door_288)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition LocksmithingForgeWaterWaveBrassMasterWardedLock() => new()
	{
		Metadata = new("Locksmithing/forge water-wave brass master warded lock", "crafts", ["earlymodern", "renaissance"], ["item:earlymodern_door_qing_master_warded_lock_303"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "forge water-wave brass master warded lock",
		Category = "Locksmithing",
		Blurb = "forge a water-wave brass master warded lock",
		Action = "forging a loose lock, key, latch, or fitting",
		ActiveCraftItemSdesc = "an in-progress water-wave brass master warded lock craft",
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
			new CatalogueItemRecipeProduct(ItemStableReference: "earlymodern_door_qing_master_warded_lock_303", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a water-wave brass master warded lock (@earlymodern_door_qing_master_warded_lock_303)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition CarpentryAndJoineryBuildFlowerHeadOakDoorLeaf() => new()
	{
		Metadata = new("Carpentry and joinery/build flower-head oak door leaf", "crafts", ["earlymodern", "renaissance"], ["item:earlymodern_door_qing_panel_door_286"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "build flower-head oak door leaf",
		Category = "Carpentry and joinery",
		Blurb = "build a flower-head oak door leaf",
		Action = "building a door, gate, shutter, screen, grille, or barrier",
		ActiveCraftItemSdesc = "an in-progress flower-head oak door leaf craft",
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
			new CatalogueCommodityRecipeInput(Grams: 21120.0, Material: "oak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("21120 grams of oak", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Saw", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Saw tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "earlymodern_door_qing_panel_door_286", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a flower-head oak door leaf (@earlymodern_door_qing_panel_door_286)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition CarpentryAndJoineryBuildBraidedBambooScreenDoorPattern2() => new()
	{
		Metadata = new("Carpentry and joinery/build braided bamboo screen door pattern 2", "crafts", ["earlymodern", "renaissance"], ["item:earlymodern_door_qing_screen_291"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "build braided bamboo screen door pattern 2",
		Category = "Carpentry and joinery",
		Blurb = "build a braided bamboo screen door",
		Action = "building a door, gate, shutter, screen, grille, or barrier",
		ActiveCraftItemSdesc = "an in-progress braided bamboo screen door craft",
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
			new CatalogueCommodityRecipeInput(Grams: 29040.0, Material: "bamboo", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("29040 grams of bamboo", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Saw", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Saw tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "earlymodern_door_qing_screen_291", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a braided bamboo screen door (@earlymodern_door_qing_screen_291)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition CarpentryAndJoineryBuildShellTeakSecureDoor() => new()
	{
		Metadata = new("Carpentry and joinery/build shell teak secure door", "crafts", ["earlymodern", "renaissance"], ["item:earlymodern_door_qing_secure_door_289"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "build shell teak secure door",
		Category = "Carpentry and joinery",
		Blurb = "build a shell teak secure door",
		Action = "building a door, gate, shutter, screen, grille, or barrier",
		ActiveCraftItemSdesc = "an in-progress shell teak secure door craft",
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
			new CatalogueCommodityRecipeInput(Grams: 25872.0, Material: "teak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("25872 grams of teak", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Saw", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Saw tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "earlymodern_door_qing_secure_door_289", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a shell teak secure door (@earlymodern_door_qing_secure_door_289)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition CarpentryAndJoineryBuildRosetteWoodShutteredOpening() => new()
	{
		Metadata = new("Carpentry and joinery/build rosette wood shuttered opening", "crafts", ["earlymodern", "renaissance"], ["item:earlymodern_door_qing_shutter_290"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "build rosette wood shuttered opening",
		Category = "Carpentry and joinery",
		Blurb = "build a rosette wood shuttered opening",
		Action = "building a door, gate, shutter, screen, grille, or barrier",
		ActiveCraftItemSdesc = "an in-progress rosette wood shuttered opening craft",
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
			new CatalogueItemRecipeProduct(ItemStableReference: "earlymodern_door_qing_shutter_290", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a rosette wood shuttered opening (@earlymodern_door_qing_shutter_290)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition LocksmithingForgeRosetteBronzeStrikePlate() => new()
	{
		Metadata = new("Locksmithing/forge rosette bronze strike plate", "crafts", ["earlymodern", "renaissance"], ["item:earlymodern_door_qing_strike_plate_297"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "forge rosette bronze strike plate",
		Category = "Locksmithing",
		Blurb = "forge a rosette bronze strike plate",
		Action = "forging a loose lock, key, latch, or fitting",
		ActiveCraftItemSdesc = "an in-progress rosette bronze strike plate craft",
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
			new CatalogueItemRecipeProduct(ItemStableReference: "earlymodern_door_qing_strike_plate_297", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a rosette bronze strike plate (@earlymodern_door_qing_strike_plate_297)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition LocksmithingForgeKnottedCopperWardedKey() => new()
	{
		Metadata = new("Locksmithing/forge knotted copper warded key", "crafts", ["earlymodern", "renaissance"], ["item:earlymodern_door_qing_warded_key_304"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "forge knotted copper warded key",
		Category = "Locksmithing",
		Blurb = "forge a knotted copper warded key",
		Action = "forging a loose lock, key, latch, or fitting",
		ActiveCraftItemSdesc = "an in-progress knotted copper warded key craft",
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
			new CatalogueCommodityRecipeInput(Grams: 1107.0, Material: "copper", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1107 grams of copper", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Anvil", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Anvil tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "earlymodern_door_qing_warded_key_304", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a knotted copper warded key (@earlymodern_door_qing_warded_key_304)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition LocksmithingForgeLeafScrollWroughtIronWardedLock() => new()
	{
		Metadata = new("Locksmithing/forge leaf-scroll wrought iron warded lock", "crafts", ["earlymodern", "renaissance"], ["item:earlymodern_door_qing_warded_lock_301"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "forge leaf-scroll wrought iron warded lock",
		Category = "Locksmithing",
		Blurb = "forge a leaf-scroll wrought iron warded lock",
		Action = "forging a loose lock, key, latch, or fitting",
		ActiveCraftItemSdesc = "an in-progress leaf-scroll wrought iron warded lock craft",
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
			new CatalogueItemRecipeProduct(ItemStableReference: "earlymodern_door_qing_warded_lock_301", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a leaf-scroll wrought iron warded lock (@earlymodern_door_qing_warded_lock_301)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition CarpentryAndJoineryBuildFlowerHeadWroughtIronWarehouseGate() => new()
	{
		Metadata = new("Carpentry and joinery/build flower-head wrought iron warehouse gate", "crafts", ["earlymodern", "renaissance"], ["item:earlymodern_door_qing_warehouse_gate_294"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "build flower-head wrought iron warehouse gate",
		Category = "Carpentry and joinery",
		Blurb = "build a flower-head wrought iron warehouse gate",
		Action = "building a door, gate, shutter, screen, grille, or barrier",
		ActiveCraftItemSdesc = "an in-progress flower-head wrought iron warehouse gate craft",
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
			new CatalogueCommodityRecipeInput(Grams: 33792.0, Material: "wrought iron", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("33792 grams of wrought iron", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Saw", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Saw tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "earlymodern_door_qing_warehouse_gate_294", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a flower-head wrought iron warehouse gate (@earlymodern_door_qing_warehouse_gate_294)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition CarpentryAndJoineryBuildWaterWaveOakBracedDoor() => new()
	{
		Metadata = new("Carpentry and joinery/build water-wave oak braced door", "crafts", ["earlymodern", "renaissance"], ["item:earlymodern_door_rus_braced_door_154"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "build water-wave oak braced door",
		Category = "Carpentry and joinery",
		Blurb = "build a water-wave oak braced door",
		Action = "building a door, gate, shutter, screen, grille, or barrier",
		ActiveCraftItemSdesc = "an in-progress water-wave oak braced door craft",
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
			new CatalogueCommodityRecipeInput(Grams: 22704.0, Material: "oak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("22704 grams of oak", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Saw", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Saw tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "earlymodern_door_rus_braced_door_154", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a water-wave oak braced door (@earlymodern_door_rus_braced_door_154)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition CarpentryAndJoineryBuildBraidedCedarCourtyardGate() => new()
	{
		Metadata = new("Carpentry and joinery/build braided cedar courtyard gate", "crafts", ["earlymodern", "renaissance"], ["item:earlymodern_door_rus_courtyard_gate_160"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "build braided cedar courtyard gate",
		Category = "Carpentry and joinery",
		Blurb = "build a braided cedar courtyard gate",
		Action = "building a door, gate, shutter, screen, grille, or barrier",
		ActiveCraftItemSdesc = "an in-progress braided cedar courtyard gate craft",
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
			new CatalogueItemRecipeProduct(ItemStableReference: "earlymodern_door_rus_courtyard_gate_160", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a braided cedar courtyard gate (@earlymodern_door_rus_courtyard_gate_160)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition LocksmithingForgeBirdBronzeKeyholeEscutcheonPattern2() => new()
	{
		Metadata = new("Locksmithing/forge bird bronze keyhole escutcheon pattern 2", "crafts", ["earlymodern", "renaissance"], ["item:earlymodern_door_rus_escutcheon_167"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "forge bird bronze keyhole escutcheon pattern 2",
		Category = "Locksmithing",
		Blurb = "forge a bird bronze keyhole escutcheon",
		Action = "forging a loose lock, key, latch, or fitting",
		ActiveCraftItemSdesc = "an in-progress bird bronze keyhole escutcheon craft",
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
			new CatalogueItemRecipeProduct(ItemStableReference: "earlymodern_door_rus_escutcheon_167", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a bird bronze keyhole escutcheon (@earlymodern_door_rus_escutcheon_167)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition CarpentryAndJoineryBuildWaterWaveWroughtIronGardenGate() => new()
	{
		Metadata = new("Carpentry and joinery/build water-wave wrought iron garden gate", "crafts", ["earlymodern", "renaissance"], ["item:earlymodern_door_rus_garden_gate_162"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "build water-wave wrought iron garden gate",
		Category = "Carpentry and joinery",
		Blurb = "build a water-wave wrought iron garden gate",
		Action = "building a door, gate, shutter, screen, grille, or barrier",
		ActiveCraftItemSdesc = "an in-progress water-wave wrought iron garden gate craft",
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
			new CatalogueCommodityRecipeInput(Grams: 35376.0, Material: "wrought iron", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("35376 grams of wrought iron", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Saw", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Saw tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "earlymodern_door_rus_garden_gate_162", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a water-wave wrought iron garden gate (@earlymodern_door_rus_garden_gate_162)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition LocksmithingForgeGeometricCopperFineWardedLockPattern2() => new()
	{
		Metadata = new("Locksmithing/forge geometric copper fine warded lock pattern 2", "crafts", ["earlymodern", "renaissance"], ["item:earlymodern_door_rus_good_warded_lock_169"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "forge geometric copper fine warded lock pattern 2",
		Category = "Locksmithing",
		Blurb = "forge a geometric copper fine warded lock",
		Action = "forging a loose lock, key, latch, or fitting",
		ActiveCraftItemSdesc = "an in-progress geometric copper fine warded lock craft",
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
			new CatalogueCommodityRecipeInput(Grams: 1177.0, Material: "copper", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1177 grams of copper", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Anvil", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Anvil tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "earlymodern_door_rus_good_warded_lock_169", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a geometric copper fine warded lock (@earlymodern_door_rus_good_warded_lock_169)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition CarpentryAndJoineryBuildFlowerHeadBrassMetalGrillePattern2() => new()
	{
		Metadata = new("Carpentry and joinery/build flower-head brass metal grille pattern 2", "crafts", ["earlymodern", "renaissance"], ["item:earlymodern_door_rus_grille_163"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "build flower-head brass metal grille pattern 2",
		Category = "Carpentry and joinery",
		Blurb = "build a flower-head brass metal grille",
		Action = "building a door, gate, shutter, screen, grille, or barrier",
		ActiveCraftItemSdesc = "an in-progress flower-head brass metal grille craft",
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
			new CatalogueItemRecipeProduct(ItemStableReference: "earlymodern_door_rus_grille_163", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a flower-head brass metal grille (@earlymodern_door_rus_grille_163)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition LocksmithingForgeFlowerHeadPewterHingePairPattern2() => new()
	{
		Metadata = new("Locksmithing/forge flower-head pewter hinge pair pattern 2", "crafts", ["earlymodern", "renaissance"], ["item:earlymodern_door_rus_hinge_pair_165"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "forge flower-head pewter hinge pair pattern 2",
		Category = "Locksmithing",
		Blurb = "forge a flower-head pewter hinge pair",
		Action = "forging a loose lock, key, latch, or fitting",
		ActiveCraftItemSdesc = "an in-progress flower-head pewter hinge pair craft",
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
			new CatalogueItemRecipeProduct(ItemStableReference: "earlymodern_door_rus_hinge_pair_165", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a flower-head pewter hinge pair (@earlymodern_door_rus_hinge_pair_165)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition LocksmithingForgeStarWroughtIronKeyringPattern2() => new()
	{
		Metadata = new("Locksmithing/forge star wrought iron keyring pattern 2", "crafts", ["earlymodern", "renaissance"], ["item:earlymodern_door_rus_keyring_166"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.RenaissanceEarlyModernJewelleryDoors.cs:68", []),
		RequiredAnyEra = ["earlymodern"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "forge star wrought iron keyring pattern 2",
		Category = "Locksmithing",
		Blurb = "forge a star wrought iron keyring",
		Action = "forging a loose lock, key, latch, or fitting",
		ActiveCraftItemSdesc = "an in-progress star wrought iron keyring craft",
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
			new CatalogueCommodityRecipeInput(Grams: 968.0, Material: "wrought iron", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("968 grams of wrought iron", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Anvil", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Anvil tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "earlymodern_door_rus_keyring_166", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a star wrought iron keyring (@earlymodern_door_rus_keyring_166)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};
}
