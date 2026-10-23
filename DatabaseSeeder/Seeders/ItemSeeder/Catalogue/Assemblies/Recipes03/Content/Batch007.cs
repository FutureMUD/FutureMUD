#nullable enable

using MudSharp.Body;
using MudSharp.GameItems.Inventory.Plans;
using MudSharp.RPG.Checks;

namespace DatabaseSeeder.Seeders;

public static partial class Recipes03Stock
{
	public static CatalogueRecipeDefinition MilitaryEquipmentMakeSturdyArmourTree() => new()
	{
		Metadata = new("Military Equipment/make sturdy armour tree", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:medieval_military_sturdy_armour_tree"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Medieval.cs:893", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make sturdy armour tree",
		Category = "Military Equipment",
		Blurb = "make a sturdy armour tree",
		Action = "makeing a sturdy armour tree",
		ActiveCraftItemSdesc = "an in-progress sturdy armour tree craft",
		Gate = CatalogueRecipeGates.MedievalEquipmentCraftingWeaponcrafting10,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
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
			new CatalogueCommodityRecipeInput(Grams: 18000.0, Material: "oak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("18000 grams of oak", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "medieval_military_sturdy_armour_tree", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a sturdy armour tree (@medieval_military_sturdy_armour_tree)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MilitaryEquipmentMakeSwordRack() => new()
	{
		Metadata = new("Military Equipment/make sword rack", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:medieval_military_sword_rack"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Medieval.cs:893", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make sword rack",
		Category = "Military Equipment",
		Blurb = "make a sword rack",
		Action = "makeing a sword rack",
		ActiveCraftItemSdesc = "an in-progress sword rack craft",
		Gate = CatalogueRecipeGates.MedievalEquipmentCraftingWeaponcrafting10,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
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
			new CatalogueCommodityRecipeInput(Grams: 10500.0, Material: "oak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("10500 grams of oak", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "medieval_military_sword_rack", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a sword rack (@medieval_military_sword_rack)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MilitaryEquipmentMakeTallRawhideShield() => new()
	{
		Metadata = new("Military Equipment/make tall rawhide shield", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:medieval_military_tall_rawhide_shield"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Medieval.cs:893", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make tall rawhide shield",
		Category = "Military Equipment",
		Blurb = "make a tall rawhide shield",
		Action = "makeing a tall rawhide shield",
		ActiveCraftItemSdesc = "an in-progress tall rawhide shield craft",
		Gate = CatalogueRecipeGates.MedievalEquipmentCraftingWeaponcrafting10,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
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
			new CatalogueCommodityRecipeInput(Grams: 2700.0, Material: "rawhide", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2700 grams of rawhide", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "medieval_military_tall_rawhide_shield", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a tall rawhide shield (@medieval_military_tall_rawhide_shield)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MilitaryEquipmentMakeTargetArrow() => new()
	{
		Metadata = new("Military Equipment/make target arrow", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:medieval_military_target_arrow"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Medieval.cs:893", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make target arrow",
		Category = "Military Equipment",
		Blurb = "make a target arrow",
		Action = "makeing a target arrow",
		ActiveCraftItemSdesc = "an in-progress target arrow craft",
		Gate = CatalogueRecipeGates.MedievalEquipmentCraftingWeaponcrafting10,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
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
			new CatalogueCommodityRecipeInput(Grams: 23.0, Material: "poplar", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("23 grams of poplar", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "medieval_military_target_arrow", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a target arrow (@medieval_military_target_arrow)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MilitaryEquipmentMakeTargetBolt() => new()
	{
		Metadata = new("Military Equipment/make target bolt", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:medieval_military_target_bolt"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Medieval.cs:893", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make target bolt",
		Category = "Military Equipment",
		Blurb = "make a target bolt",
		Action = "makeing a target bolt",
		ActiveCraftItemSdesc = "an in-progress target bolt craft",
		Gate = CatalogueRecipeGates.MedievalEquipmentCraftingWeaponcrafting10,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
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
			new CatalogueCommodityRecipeInput(Grams: 33.0, Material: "poplar", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("33 grams of poplar", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "medieval_military_target_bolt", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a target bolt (@medieval_military_target_bolt)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MilitaryEquipmentMakeTemperedBodkinArrow() => new()
	{
		Metadata = new("Military Equipment/make tempered bodkin arrow", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:medieval_military_tempered_bodkin_arrow"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Medieval.cs:893", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make tempered bodkin arrow",
		Category = "Military Equipment",
		Blurb = "make a tempered bodkin arrow",
		Action = "makeing a tempered bodkin arrow",
		ActiveCraftItemSdesc = "an in-progress tempered bodkin arrow craft",
		Gate = CatalogueRecipeGates.MedievalEquipmentCraftingWeaponcrafting10,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
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
			new CatalogueCommodityRecipeInput(Grams: 29.0, Material: "birch", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("29 grams of birch", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "medieval_military_tempered_bodkin_arrow", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a tempered bodkin arrow (@medieval_military_tempered_bodkin_arrow)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MilitaryEquipmentMakeThickLeatherHideShield() => new()
	{
		Metadata = new("Military Equipment/make thick leather hide shield", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:medieval_military_thick_leather_hide_shield"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Medieval.cs:893", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make thick leather hide shield",
		Category = "Military Equipment",
		Blurb = "make a thick leather hide shield",
		Action = "makeing a thick leather hide shield",
		ActiveCraftItemSdesc = "an in-progress thick leather hide shield craft",
		Gate = CatalogueRecipeGates.MedievalEquipmentCraftingWeaponcrafting10,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
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
			new CatalogueCommodityRecipeInput(Grams: 2175.0, Material: "cow leather", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2175 grams of cow leather", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "medieval_military_thick_leather_hide_shield", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a thick leather hide shield (@medieval_military_thick_leather_hide_shield)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MilitaryEquipmentMakeColourTooledLeatherKnifeSheath() => new()
	{
		Metadata = new("Military Equipment/make $colour tooled leather knife sheath", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:medieval_military_tooled_leather_knife_sheath"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Medieval.cs:893", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make $colour tooled leather knife sheath",
		Category = "Military Equipment",
		Blurb = "make a $colour tooled leather knife sheath",
		Action = "makeing a $colour tooled leather knife sheath",
		ActiveCraftItemSdesc = "an in-progress $colour tooled leather knife sheath craft",
		Gate = CatalogueRecipeGates.MedievalEquipmentCraftingWeaponcrafting10,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
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
			new CatalogueCommodityRecipeInput(Grams: 83.0, Material: "leather", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("83 grams of leather", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "medieval_military_tooled_leather_knife_sheath", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a $colour tooled leather knife sheath (@medieval_military_tooled_leather_knife_sheath)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MilitaryEquipmentMakeWoodenPracticePolearm() => new()
	{
		Metadata = new("Military Equipment/make wooden practice polearm", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:medieval_military_training_axe_headed_polearm"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Medieval.cs:893", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make wooden practice polearm",
		Category = "Military Equipment",
		Blurb = "make a wooden practice polearm",
		Action = "makeing a wooden practice polearm",
		ActiveCraftItemSdesc = "an in-progress wooden practice polearm craft",
		Gate = CatalogueRecipeGates.MedievalEquipmentCraftingWeaponcrafting10,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
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
			new CatalogueCommodityRecipeInput(Grams: 1050.0, Material: "beech", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1050 grams of beech", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "medieval_military_training_axe_headed_polearm", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a wooden practice polearm (@medieval_military_training_axe_headed_polearm)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MilitaryEquipmentMakePaddedTrainingClub() => new()
	{
		Metadata = new("Military Equipment/make padded training club", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:medieval_military_training_club"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Medieval.cs:893", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make padded training club",
		Category = "Military Equipment",
		Blurb = "make a padded training club",
		Action = "makeing a padded training club",
		ActiveCraftItemSdesc = "an in-progress padded training club craft",
		Gate = CatalogueRecipeGates.MedievalEquipmentCraftingWeaponcrafting10,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
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
			new CatalogueCommodityRecipeInput(Grams: 488.0, Material: "beech", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("488 grams of beech", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "medieval_military_training_club", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a padded training club (@medieval_military_training_club)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MilitaryEquipmentMakePaddedTrainingFlail() => new()
	{
		Metadata = new("Military Equipment/make padded training flail", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:medieval_military_training_flail"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Medieval.cs:893", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make padded training flail",
		Category = "Military Equipment",
		Blurb = "make a padded training flail",
		Action = "makeing a padded training flail",
		ActiveCraftItemSdesc = "an in-progress padded training flail craft",
		Gate = CatalogueRecipeGates.MedievalEquipmentCraftingWeaponcrafting10,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
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
			new CatalogueCommodityRecipeInput(Grams: 750.0, Material: "ash", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("750 grams of ash", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "medieval_military_training_flail", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a padded training flail (@medieval_military_training_flail)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MilitaryEquipmentMakePaddedTrainingMace() => new()
	{
		Metadata = new("Military Equipment/make padded training mace", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:medieval_military_training_mace"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Medieval.cs:893", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make padded training mace",
		Category = "Military Equipment",
		Blurb = "make a padded training mace",
		Action = "makeing a padded training mace",
		ActiveCraftItemSdesc = "an in-progress padded training mace craft",
		Gate = CatalogueRecipeGates.MedievalEquipmentCraftingWeaponcrafting10,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
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
			new CatalogueCommodityRecipeInput(Grams: 570.0, Material: "beech", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("570 grams of beech", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "medieval_military_training_mace", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a padded training mace (@medieval_military_training_mace)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MilitaryEquipmentMakeBluntedTrainingMattock() => new()
	{
		Metadata = new("Military Equipment/make blunted training mattock", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:medieval_military_training_mattock"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Medieval.cs:893", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make blunted training mattock",
		Category = "Military Equipment",
		Blurb = "make a blunted training mattock",
		Action = "makeing a blunted training mattock",
		ActiveCraftItemSdesc = "an in-progress blunted training mattock craft",
		Gate = CatalogueRecipeGates.MedievalEquipmentCraftingWeaponcrafting10,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
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
			new CatalogueCommodityRecipeInput(Grams: 1125.0, Material: "beech", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1125 grams of beech", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "medieval_military_training_mattock", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a blunted training mattock (@medieval_military_training_mattock)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MilitaryEquipmentMakeBluntedTrainingPick() => new()
	{
		Metadata = new("Military Equipment/make blunted training pick", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:medieval_military_training_military_pick"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Medieval.cs:893", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make blunted training pick",
		Category = "Military Equipment",
		Blurb = "make a blunted training pick",
		Action = "makeing a blunted training pick",
		ActiveCraftItemSdesc = "an in-progress blunted training pick craft",
		Gate = CatalogueRecipeGates.MedievalEquipmentCraftingWeaponcrafting10,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
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
			new CatalogueCommodityRecipeInput(Grams: 525.0, Material: "beech", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("525 grams of beech", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "medieval_military_training_military_pick", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a blunted training pick (@medieval_military_training_military_pick)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MilitaryEquipmentMakeBluntedTrainingPike() => new()
	{
		Metadata = new("Military Equipment/make blunted training pike", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:medieval_military_training_pike"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Medieval.cs:893", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make blunted training pike",
		Category = "Military Equipment",
		Blurb = "make a blunted training pike",
		Action = "makeing a blunted training pike",
		ActiveCraftItemSdesc = "an in-progress blunted training pike craft",
		Gate = CatalogueRecipeGates.MedievalEquipmentCraftingWeaponcrafting10,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
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
			new CatalogueCommodityRecipeInput(Grams: 1650.0, Material: "ash", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1650 grams of ash", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "medieval_military_training_pike", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a blunted training pike (@medieval_military_training_pike)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MilitaryEquipmentMakeTrainingQuarterstaff() => new()
	{
		Metadata = new("Military Equipment/make training quarterstaff", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:medieval_military_training_quarterstaff"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Medieval.cs:893", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make training quarterstaff",
		Category = "Military Equipment",
		Blurb = "make a training quarterstaff",
		Action = "makeing a training quarterstaff",
		ActiveCraftItemSdesc = "an in-progress training quarterstaff craft",
		Gate = CatalogueRecipeGates.MedievalEquipmentCraftingWeaponcrafting10,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
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
			new CatalogueCommodityRecipeInput(Grams: 1013.0, Material: "ash", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1013 grams of ash", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "medieval_military_training_quarterstaff", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a training quarterstaff (@medieval_military_training_quarterstaff)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MilitaryEquipmentMakeBluntedTrainingSpear() => new()
	{
		Metadata = new("Military Equipment/make blunted training spear", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:medieval_military_training_spear"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Medieval.cs:893", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make blunted training spear",
		Category = "Military Equipment",
		Blurb = "make a blunted training spear",
		Action = "makeing a blunted training spear",
		ActiveCraftItemSdesc = "an in-progress blunted training spear craft",
		Gate = CatalogueRecipeGates.MedievalEquipmentCraftingWeaponcrafting10,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
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
			new CatalogueCommodityRecipeInput(Grams: 938.0, Material: "ash", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("938 grams of ash", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "medieval_military_training_spear", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a blunted training spear (@medieval_military_training_spear)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MilitaryEquipmentMakePaddedTrainingHammer() => new()
	{
		Metadata = new("Military Equipment/make padded training hammer", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:medieval_military_training_war_hammer"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Medieval.cs:893", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make padded training hammer",
		Category = "Military Equipment",
		Blurb = "make a padded training hammer",
		Action = "makeing a padded training hammer",
		ActiveCraftItemSdesc = "an in-progress padded training hammer craft",
		Gate = CatalogueRecipeGates.MedievalEquipmentCraftingWeaponcrafting10,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
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
			new CatalogueCommodityRecipeInput(Grams: 540.0, Material: "beech", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("540 grams of beech", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "medieval_military_training_war_hammer", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a padded training hammer (@medieval_military_training_war_hammer)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MilitaryEquipmentMakeTwoHandedWaster() => new()
	{
		Metadata = new("Military Equipment/make two-handed waster", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:medieval_military_two_handed_waster"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Medieval.cs:893", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make two-handed waster",
		Category = "Military Equipment",
		Blurb = "make a two-handed waster",
		Action = "makeing a two-handed waster",
		ActiveCraftItemSdesc = "an in-progress two-handed waster craft",
		Gate = CatalogueRecipeGates.MedievalEquipmentCraftingWeaponcrafting10,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
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
			new CatalogueCommodityRecipeInput(Grams: 1013.0, Material: "beech", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1013 grams of beech", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "medieval_military_two_handed_waster", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a two-handed waster (@medieval_military_two_handed_waster)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MilitaryEquipmentMakeWallMountedSpearRack() => new()
	{
		Metadata = new("Military Equipment/make wall-mounted spear rack", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:medieval_military_wall_spear_rack"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Medieval.cs:893", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make wall-mounted spear rack",
		Category = "Military Equipment",
		Blurb = "make a wall-mounted spear rack",
		Action = "makeing a wall-mounted spear rack",
		ActiveCraftItemSdesc = "an in-progress wall-mounted spear rack craft",
		Gate = CatalogueRecipeGates.MedievalEquipmentCraftingWeaponcrafting10,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
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
			new CatalogueCommodityRecipeInput(Grams: 13500.0, Material: "oak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("13500 grams of oak", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "medieval_military_wall_spear_rack", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a wall-mounted spear rack (@medieval_military_wall_spear_rack)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MilitaryEquipmentMakeWeatheredArmourTabard() => new()
	{
		Metadata = new("Military Equipment/make weathered armour tabard", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:medieval_military_weathered_armour_tabard"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Medieval.cs:893", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make weathered armour tabard",
		Category = "Military Equipment",
		Blurb = "make a weathered armour tabard",
		Action = "makeing a weathered armour tabard",
		ActiveCraftItemSdesc = "an in-progress weathered armour tabard craft",
		Gate = CatalogueRecipeGates.MedievalEquipmentCraftingWeaponcrafting10,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
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
			new CatalogueCommodityRecipeInput(Grams: 540.0, Material: "canvas", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("540 grams of canvas", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "medieval_military_weathered_armour_tabard", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a weathered armour tabard (@medieval_military_weathered_armour_tabard)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MilitaryEquipmentMakeWeightedWarClub() => new()
	{
		Metadata = new("Military Equipment/make weighted war club", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:medieval_military_weighted_war_club"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Medieval.cs:893", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make weighted war club",
		Category = "Military Equipment",
		Blurb = "make a weighted war club",
		Action = "makeing a weighted war club",
		ActiveCraftItemSdesc = "an in-progress weighted war club craft",
		Gate = CatalogueRecipeGates.MedievalEquipmentCraftingWeaponcrafting10,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
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
			new CatalogueCommodityRecipeInput(Grams: 938.0, Material: "oak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("938 grams of oak", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "medieval_military_weighted_war_club", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a weighted war club (@medieval_military_weighted_war_club)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MilitaryEquipmentMakeWickerArrowQuiver() => new()
	{
		Metadata = new("Military Equipment/make wicker arrow quiver", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:medieval_military_wicker_arrow_quiver"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Medieval.cs:893", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make wicker arrow quiver",
		Category = "Military Equipment",
		Blurb = "make a wicker arrow quiver",
		Action = "makeing a wicker arrow quiver",
		ActiveCraftItemSdesc = "an in-progress wicker arrow quiver craft",
		Gate = CatalogueRecipeGates.MedievalEquipmentCraftingWeaponcrafting10,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
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
			new CatalogueCommodityRecipeInput(Grams: 323.0, Material: "wicker", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("323 grams of wicker", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "medieval_military_wicker_arrow_quiver", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a wicker arrow quiver (@medieval_military_wicker_arrow_quiver)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MilitaryEquipmentMakeWickerFacedPavise() => new()
	{
		Metadata = new("Military Equipment/make wicker-faced pavise", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:medieval_military_wicker_faced_pavise"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Medieval.cs:893", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make wicker-faced pavise",
		Category = "Military Equipment",
		Blurb = "make a wicker-faced pavise",
		Action = "makeing a wicker-faced pavise",
		ActiveCraftItemSdesc = "an in-progress wicker-faced pavise craft",
		Gate = CatalogueRecipeGates.MedievalEquipmentCraftingWeaponcrafting10,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
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
			new CatalogueCommodityRecipeInput(Grams: 3900.0, Material: "wicker", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3900 grams of wicker", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "medieval_military_wicker_faced_pavise", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a wicker-faced pavise (@medieval_military_wicker_faced_pavise)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MilitaryEquipmentMakeWoodenCrossbowBoltBox() => new()
	{
		Metadata = new("Military Equipment/make wooden crossbow bolt box", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:medieval_military_wooden_crossbow_bolt_box"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Medieval.cs:893", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make wooden crossbow bolt box",
		Category = "Military Equipment",
		Blurb = "make a wooden crossbow bolt box",
		Action = "makeing a wooden crossbow bolt box",
		ActiveCraftItemSdesc = "an in-progress wooden crossbow bolt box craft",
		Gate = CatalogueRecipeGates.MedievalEquipmentCraftingWeaponcrafting10,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
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
			new CatalogueCommodityRecipeInput(Grams: 938.0, Material: "oak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("938 grams of oak", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "medieval_military_wooden_crossbow_bolt_box", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a wooden crossbow bolt box (@medieval_military_wooden_crossbow_bolt_box)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MilitaryEquipmentMakeWoodenPracticeDagger() => new()
	{
		Metadata = new("Military Equipment/make wooden practice dagger", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:medieval_military_wooden_practice_dagger"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Medieval.cs:893", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make wooden practice dagger",
		Category = "Military Equipment",
		Blurb = "make a wooden practice dagger",
		Action = "makeing a wooden practice dagger",
		ActiveCraftItemSdesc = "an in-progress wooden practice dagger craft",
		Gate = CatalogueRecipeGates.MedievalEquipmentCraftingWeaponcrafting10,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
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
			new CatalogueCommodityRecipeInput(Grams: 165.0, Material: "beech", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("165 grams of beech", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "medieval_military_wooden_practice_dagger", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a wooden practice dagger (@medieval_military_wooden_practice_dagger)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MilitaryEquipmentMakeWoodenPracticeKnife() => new()
	{
		Metadata = new("Military Equipment/make wooden practice knife", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:medieval_military_wooden_practice_knife"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Medieval.cs:893", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make wooden practice knife",
		Category = "Military Equipment",
		Blurb = "make a wooden practice knife",
		Action = "makeing a wooden practice knife",
		ActiveCraftItemSdesc = "an in-progress wooden practice knife craft",
		Gate = CatalogueRecipeGates.MedievalEquipmentCraftingWeaponcrafting10,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
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
			new CatalogueCommodityRecipeInput(Grams: 135.0, Material: "beech", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("135 grams of beech", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "medieval_military_wooden_practice_knife", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a wooden practice knife (@medieval_military_wooden_practice_knife)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MilitaryEquipmentMakeWoolBraidedSling() => new()
	{
		Metadata = new("Military Equipment/make wool-braided sling", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:medieval_military_wool_braided_sling"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Medieval.cs:893", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make wool-braided sling",
		Category = "Military Equipment",
		Blurb = "make a wool-braided sling",
		Action = "makeing a wool-braided sling",
		ActiveCraftItemSdesc = "an in-progress wool-braided sling craft",
		Gate = CatalogueRecipeGates.MedievalEquipmentCraftingWeaponcrafting10,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
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
			new CatalogueCommodityRecipeInput(Grams: 60.0, Material: "wool", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("60 grams of wool", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "medieval_military_wool_braided_sling", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a wool-braided sling (@medieval_military_wool_braided_sling)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MilitaryEquipmentMakeWateredWarSword() => new()
	{
		Metadata = new("Military Equipment/make watered war sword", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:medieval_military_wootz_war_sword"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Medieval.cs:893", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make watered war sword",
		Category = "Military Equipment",
		Blurb = "make a watered war sword",
		Action = "makeing a watered war sword",
		ActiveCraftItemSdesc = "an in-progress watered war sword craft",
		Gate = CatalogueRecipeGates.MedievalEquipmentCraftingWeaponcrafting10,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
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
			new CatalogueCommodityRecipeInput(Grams: 863.0, Material: "wootz steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("863 grams of wootz steel", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "medieval_military_wootz_war_sword", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a watered war sword (@medieval_military_wootz_war_sword)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MilitaryEquipmentMakeWornAshShortbow() => new()
	{
		Metadata = new("Military Equipment/make worn ash shortbow", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:medieval_military_worn_ash_shortbow"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Medieval.cs:893", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make worn ash shortbow",
		Category = "Military Equipment",
		Blurb = "make a worn ash shortbow",
		Action = "makeing a worn ash shortbow",
		ActiveCraftItemSdesc = "an in-progress worn ash shortbow craft",
		Gate = CatalogueRecipeGates.MedievalEquipmentCraftingWeaponcrafting10,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
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
			new CatalogueCommodityRecipeInput(Grams: 510.0, Material: "ash", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("510 grams of ash", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "medieval_military_worn_ash_shortbow", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a worn ash shortbow (@medieval_military_worn_ash_shortbow)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MilitaryEquipmentMakeWornBuckler() => new()
	{
		Metadata = new("Military Equipment/make worn buckler", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:medieval_military_worn_buckler"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Medieval.cs:893", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make worn buckler",
		Category = "Military Equipment",
		Blurb = "make a worn buckler",
		Action = "makeing a worn buckler",
		ActiveCraftItemSdesc = "an in-progress worn buckler craft",
		Gate = CatalogueRecipeGates.MedievalEquipmentCraftingWeaponcrafting10,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
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
			new CatalogueCommodityRecipeInput(Grams: 563.0, Material: "wood", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("563 grams of wood", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "medieval_military_worn_buckler", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a worn buckler (@medieval_military_worn_buckler)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MilitaryEquipmentMakeWornFightingKnife() => new()
	{
		Metadata = new("Military Equipment/make worn fighting knife", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:medieval_military_worn_fighting_knife"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Medieval.cs:893", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make worn fighting knife",
		Category = "Military Equipment",
		Blurb = "make a worn fighting knife",
		Action = "makeing a worn fighting knife",
		ActiveCraftItemSdesc = "an in-progress worn fighting knife craft",
		Gate = CatalogueRecipeGates.MedievalEquipmentCraftingWeaponcrafting10,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
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
			new CatalogueCommodityRecipeInput(Grams: 195.0, Material: "wrought iron", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("195 grams of wrought iron", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "medieval_military_worn_fighting_knife", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a worn fighting knife (@medieval_military_worn_fighting_knife)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MilitaryEquipmentMakeWornHandAxe() => new()
	{
		Metadata = new("Military Equipment/make worn hand axe", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:medieval_military_worn_hand_axe"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Medieval.cs:893", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make worn hand axe",
		Category = "Military Equipment",
		Blurb = "make a worn hand axe",
		Action = "makeing a worn hand axe",
		ActiveCraftItemSdesc = "an in-progress worn hand axe craft",
		Gate = CatalogueRecipeGates.MedievalEquipmentCraftingWeaponcrafting10,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
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
			new CatalogueCommodityRecipeInput(Grams: 540.0, Material: "wrought iron", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("540 grams of wrought iron", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "medieval_military_worn_hand_axe", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a worn hand axe (@medieval_military_worn_hand_axe)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MilitaryEquipmentMakeWornHeaterShield() => new()
	{
		Metadata = new("Military Equipment/make worn heater shield", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:medieval_military_worn_heater_shield"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Medieval.cs:893", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make worn heater shield",
		Category = "Military Equipment",
		Blurb = "make a worn heater shield",
		Action = "makeing a worn heater shield",
		ActiveCraftItemSdesc = "an in-progress worn heater shield craft",
		Gate = CatalogueRecipeGates.MedievalEquipmentCraftingWeaponcrafting10,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
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
			new CatalogueCommodityRecipeInput(Grams: 2250.0, Material: "poplar", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2250 grams of poplar", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "medieval_military_worn_heater_shield", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a worn heater shield (@medieval_military_worn_heater_shield)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MilitaryEquipmentMakeWornInfantryPike() => new()
	{
		Metadata = new("Military Equipment/make worn infantry pike", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:medieval_military_worn_infantry_pike"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Medieval.cs:893", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make worn infantry pike",
		Category = "Military Equipment",
		Blurb = "make a worn infantry pike",
		Action = "makeing a worn infantry pike",
		ActiveCraftItemSdesc = "an in-progress worn infantry pike craft",
		Gate = CatalogueRecipeGates.MedievalEquipmentCraftingWeaponcrafting10,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
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
			new CatalogueCommodityRecipeInput(Grams: 2250.0, Material: "wrought iron", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2250 grams of wrought iron", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "medieval_military_worn_infantry_pike", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a worn infantry pike (@medieval_military_worn_infantry_pike)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MilitaryEquipmentMakeWornIronDagger() => new()
	{
		Metadata = new("Military Equipment/make worn iron dagger", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:medieval_military_worn_iron_dagger"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Medieval.cs:893", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make worn iron dagger",
		Category = "Military Equipment",
		Blurb = "make a worn iron dagger",
		Action = "makeing a worn iron dagger",
		ActiveCraftItemSdesc = "an in-progress worn iron dagger craft",
		Gate = CatalogueRecipeGates.MedievalEquipmentCraftingWeaponcrafting10,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
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
			new CatalogueCommodityRecipeInput(Grams: 248.0, Material: "wrought iron", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("248 grams of wrought iron", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "medieval_military_worn_iron_dagger", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a worn iron dagger (@medieval_military_worn_iron_dagger)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MilitaryEquipmentMakeWornIronMace() => new()
	{
		Metadata = new("Military Equipment/make worn iron mace", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:medieval_military_worn_iron_mace"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Medieval.cs:893", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make worn iron mace",
		Category = "Military Equipment",
		Blurb = "make a worn iron mace",
		Action = "makeing a worn iron mace",
		ActiveCraftItemSdesc = "an in-progress worn iron mace craft",
		Gate = CatalogueRecipeGates.MedievalEquipmentCraftingWeaponcrafting10,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
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
			new CatalogueCommodityRecipeInput(Grams: 885.0, Material: "wrought iron", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("885 grams of wrought iron", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "medieval_military_worn_iron_mace", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a worn iron mace (@medieval_military_worn_iron_mace)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MilitaryEquipmentMakeWornKiteShield() => new()
	{
		Metadata = new("Military Equipment/make worn kite shield", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:medieval_military_worn_kite_shield"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Medieval.cs:893", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make worn kite shield",
		Category = "Military Equipment",
		Blurb = "make a worn kite shield",
		Action = "makeing a worn kite shield",
		ActiveCraftItemSdesc = "an in-progress worn kite shield craft",
		Gate = CatalogueRecipeGates.MedievalEquipmentCraftingWeaponcrafting10,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
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
			new CatalogueCommodityRecipeInput(Grams: 3600.0, Material: "poplar", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3600 grams of poplar", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "medieval_military_worn_kite_shield", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a worn kite shield (@medieval_military_worn_kite_shield)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MilitaryEquipmentMakeWornLevySpear() => new()
	{
		Metadata = new("Military Equipment/make worn levy spear", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:medieval_military_worn_levy_spear"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Medieval.cs:893", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make worn levy spear",
		Category = "Military Equipment",
		Blurb = "make a worn levy spear",
		Action = "makeing a worn levy spear",
		ActiveCraftItemSdesc = "an in-progress worn levy spear craft",
		Gate = CatalogueRecipeGates.MedievalEquipmentCraftingWeaponcrafting10,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
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
			new CatalogueCommodityRecipeInput(Grams: 1238.0, Material: "wrought iron", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1238 grams of wrought iron", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "medieval_military_worn_levy_spear", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a worn levy spear (@medieval_military_worn_levy_spear)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MilitaryEquipmentMakeWornMailByrnie() => new()
	{
		Metadata = new("Military Equipment/make worn mail byrnie", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:medieval_military_worn_mail_byrnie"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Medieval.cs:893", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make worn mail byrnie",
		Category = "Military Equipment",
		Blurb = "make a worn mail byrnie",
		Action = "makeing a worn mail byrnie",
		ActiveCraftItemSdesc = "an in-progress worn mail byrnie craft",
		Gate = CatalogueRecipeGates.MedievalEquipmentCraftingWeaponcrafting10,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
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
			new CatalogueCommodityRecipeInput(Grams: 4200.0, Material: "wrought iron", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("4200 grams of wrought iron", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "medieval_military_worn_mail_byrnie", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a worn mail byrnie (@medieval_military_worn_mail_byrnie)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MilitaryEquipmentMakeWornMilitaryPick() => new()
	{
		Metadata = new("Military Equipment/make worn military pick", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:medieval_military_worn_military_pick"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Medieval.cs:893", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make worn military pick",
		Category = "Military Equipment",
		Blurb = "make a worn military pick",
		Action = "makeing a worn military pick",
		ActiveCraftItemSdesc = "an in-progress worn military pick craft",
		Gate = CatalogueRecipeGates.MedievalEquipmentCraftingWeaponcrafting10,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
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
			new CatalogueCommodityRecipeInput(Grams: 690.0, Material: "wrought iron", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("690 grams of wrought iron", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "medieval_military_worn_military_pick", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a worn military pick (@medieval_military_worn_military_pick)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MilitaryEquipmentMakeWornQuarterstaff() => new()
	{
		Metadata = new("Military Equipment/make worn quarterstaff", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:medieval_military_worn_quarterstaff"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Medieval.cs:893", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make worn quarterstaff",
		Category = "Military Equipment",
		Blurb = "make a worn quarterstaff",
		Action = "makeing a worn quarterstaff",
		ActiveCraftItemSdesc = "an in-progress worn quarterstaff craft",
		Gate = CatalogueRecipeGates.MedievalEquipmentCraftingWeaponcrafting10,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
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
			new CatalogueCommodityRecipeInput(Grams: 1125.0, Material: "ash", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1125 grams of ash", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "medieval_military_worn_quarterstaff", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a worn quarterstaff (@medieval_military_worn_quarterstaff)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MilitaryEquipmentMakeWornRoundShield() => new()
	{
		Metadata = new("Military Equipment/make worn round shield", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:medieval_military_worn_round_shield"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Medieval.cs:893", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make worn round shield",
		Category = "Military Equipment",
		Blurb = "make a worn round shield",
		Action = "makeing a worn round shield",
		ActiveCraftItemSdesc = "an in-progress worn round shield craft",
		Gate = CatalogueRecipeGates.MedievalEquipmentCraftingWeaponcrafting10,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
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
			new CatalogueCommodityRecipeInput(Grams: 2400.0, Material: "linden", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2400 grams of linden", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "medieval_military_worn_round_shield", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a worn round shield (@medieval_military_worn_round_shield)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MilitaryEquipmentMakeWornShortSword() => new()
	{
		Metadata = new("Military Equipment/make worn short sword", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:medieval_military_worn_short_sword"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Medieval.cs:893", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make worn short sword",
		Category = "Military Equipment",
		Blurb = "make a worn short sword",
		Action = "makeing a worn short sword",
		ActiveCraftItemSdesc = "an in-progress worn short sword craft",
		Gate = CatalogueRecipeGates.MedievalEquipmentCraftingWeaponcrafting10,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
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
			new CatalogueCommodityRecipeInput(Grams: 675.0, Material: "wrought iron", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("675 grams of wrought iron", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "medieval_military_worn_short_sword", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a worn short sword (@medieval_military_worn_short_sword)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MilitaryEquipmentMakeWornSmallTarge() => new()
	{
		Metadata = new("Military Equipment/make worn small targe", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:medieval_military_worn_small_targe"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Medieval.cs:893", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make worn small targe",
		Category = "Military Equipment",
		Blurb = "make a worn small targe",
		Action = "makeing a worn small targe",
		ActiveCraftItemSdesc = "an in-progress worn small targe craft",
		Gate = CatalogueRecipeGates.MedievalEquipmentCraftingWeaponcrafting10,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
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
			new CatalogueCommodityRecipeInput(Grams: 1125.0, Material: "wood", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1125 grams of wood", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "medieval_military_worn_small_targe", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a worn small targe (@medieval_military_worn_small_targe)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MilitaryEquipmentMakeWornStirrupCrossbow() => new()
	{
		Metadata = new("Military Equipment/make worn stirrup crossbow", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:medieval_military_worn_stirrup_crossbow"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Medieval.cs:893", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make worn stirrup crossbow",
		Category = "Military Equipment",
		Blurb = "make a worn stirrup crossbow",
		Action = "makeing a worn stirrup crossbow",
		ActiveCraftItemSdesc = "an in-progress worn stirrup crossbow craft",
		Gate = CatalogueRecipeGates.MedievalEquipmentCraftingWeaponcrafting10,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
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
			new CatalogueCommodityRecipeInput(Grams: 2138.0, Material: "wood", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2138 grams of wood", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "medieval_military_worn_stirrup_crossbow", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a worn stirrup crossbow (@medieval_military_worn_stirrup_crossbow)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MilitaryEquipmentMakeWornSwordScabbard() => new()
	{
		Metadata = new("Military Equipment/make worn sword scabbard", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:medieval_military_worn_sword_scabbard"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Medieval.cs:893", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make worn sword scabbard",
		Category = "Military Equipment",
		Blurb = "make a worn sword scabbard",
		Action = "makeing a worn sword scabbard",
		ActiveCraftItemSdesc = "an in-progress worn sword scabbard craft",
		Gate = CatalogueRecipeGates.MedievalEquipmentCraftingWeaponcrafting10,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
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
			new CatalogueCommodityRecipeInput(Grams: 465.0, Material: "leather", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("465 grams of leather", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "medieval_military_worn_sword_scabbard", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a worn sword scabbard (@medieval_military_worn_sword_scabbard)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MilitaryEquipmentMakeWornThrowingKnife() => new()
	{
		Metadata = new("Military Equipment/make worn throwing knife", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:medieval_military_worn_throwing_knife"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Medieval.cs:893", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make worn throwing knife",
		Category = "Military Equipment",
		Blurb = "make a worn throwing knife",
		Action = "makeing a worn throwing knife",
		ActiveCraftItemSdesc = "an in-progress worn throwing knife craft",
		Gate = CatalogueRecipeGates.MedievalEquipmentCraftingWeaponcrafting10,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
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
			new CatalogueCommodityRecipeInput(Grams: 135.0, Material: "wrought iron", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("135 grams of wrought iron", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "medieval_military_worn_throwing_knife", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a worn throwing knife (@medieval_military_worn_throwing_knife)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MilitaryEquipmentMakeWornWarHammer() => new()
	{
		Metadata = new("Military Equipment/make worn war hammer", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:medieval_military_worn_war_hammer"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Medieval.cs:893", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make worn war hammer",
		Category = "Military Equipment",
		Blurb = "make a worn war hammer",
		Action = "makeing a worn war hammer",
		ActiveCraftItemSdesc = "an in-progress worn war hammer craft",
		Gate = CatalogueRecipeGates.MedievalEquipmentCraftingWeaponcrafting10,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
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
			new CatalogueCommodityRecipeInput(Grams: 713.0, Material: "wrought iron", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("713 grams of wrought iron", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "medieval_military_worn_war_hammer", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a worn war hammer (@medieval_military_worn_war_hammer)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MilitaryEquipmentMakeYewLongbow() => new()
	{
		Metadata = new("Military Equipment/make yew longbow", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:medieval_military_yew_longbow"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Medieval.cs:893", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make yew longbow",
		Category = "Military Equipment",
		Blurb = "make a yew longbow",
		Action = "makeing a yew longbow",
		ActiveCraftItemSdesc = "an in-progress yew longbow craft",
		Gate = CatalogueRecipeGates.MedievalEquipmentCraftingWeaponcrafting10,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
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
			new CatalogueCommodityRecipeInput(Grams: 675.0, Material: "yew", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("675 grams of yew", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "medieval_military_yew_longbow", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a yew longbow (@medieval_military_yew_longbow)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition WritingAndAdministrationMakeAccountParchmentRoll() => new()
	{
		Metadata = new("Writing and Administration/make account parchment roll", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:medieval_writing_account_roll"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Medieval.cs:893", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make account parchment roll",
		Category = "Writing and Administration",
		Blurb = "make an account parchment roll",
		Action = "makeing an account parchment roll",
		ActiveCraftItemSdesc = "an in-progress account parchment roll craft",
		Gate = CatalogueRecipeGates.MedievalWritingAndAdministrationBookbinding10,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
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
			new CatalogueCommodityRecipeInput(Grams: 98.0, Material: "parchment", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("98 grams of parchment", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Knife", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Knife tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "medieval_writing_account_roll", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x an account parchment roll (@medieval_writing_account_roll)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition WritingAndAdministrationMakeAccountTableBoard() => new()
	{
		Metadata = new("Writing and Administration/make account table board", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:medieval_writing_account_table_board"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Medieval.cs:893", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make account table board",
		Category = "Writing and Administration",
		Blurb = "make an account table board",
		Action = "makeing an account table board",
		ActiveCraftItemSdesc = "an in-progress account table board craft",
		Gate = CatalogueRecipeGates.MedievalWritingAndAdministrationBookbinding10,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
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
			new CatalogueCommodityRecipeInput(Grams: 1200.0, Material: "wood", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1200 grams of wood", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Knife", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Knife tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "medieval_writing_account_table_board", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x an account table board (@medieval_writing_account_table_board)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition WritingAndAdministrationMakeAccountWaxTablet() => new()
	{
		Metadata = new("Writing and Administration/make account wax tablet", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:medieval_writing_account_wax_tablet"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Medieval.cs:893", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make account wax tablet",
		Category = "Writing and Administration",
		Blurb = "make an account wax tablet",
		Action = "makeing an account wax tablet",
		ActiveCraftItemSdesc = "an in-progress account wax tablet craft",
		Gate = CatalogueRecipeGates.MedievalWritingAndAdministrationBookbinding10,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
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
			new CatalogueCommodityRecipeInput(Grams: 338.0, Material: "beeswax", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("338 grams of beeswax", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Knife", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Knife tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "medieval_writing_account_wax_tablet", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x an account wax tablet (@medieval_writing_account_wax_tablet)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition WritingAndAdministrationMakeAgateBurnisher() => new()
	{
		Metadata = new("Writing and Administration/make agate burnisher", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:medieval_writing_agate_burnisher"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Medieval.cs:893", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make agate burnisher",
		Category = "Writing and Administration",
		Blurb = "make an agate burnisher",
		Action = "makeing an agate burnisher",
		ActiveCraftItemSdesc = "an in-progress agate burnisher craft",
		Gate = CatalogueRecipeGates.MedievalWritingAndAdministrationBookbinding10,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
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
			new CatalogueCommodityRecipeInput(Grams: 64.0, Material: "agate", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("64 grams of agate", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Knife", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Knife tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "medieval_writing_agate_burnisher", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x an agate burnisher (@medieval_writing_agate_burnisher)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition WritingAndAdministrationMakeFineAgateInkstone() => new()
	{
		Metadata = new("Writing and Administration/make fine agate inkstone", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:medieval_writing_agate_inkstone"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Medieval.cs:893", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make fine agate inkstone",
		Category = "Writing and Administration",
		Blurb = "make a fine agate inkstone",
		Action = "makeing a fine agate inkstone",
		ActiveCraftItemSdesc = "an in-progress fine agate inkstone craft",
		Gate = CatalogueRecipeGates.MedievalWritingAndAdministrationBookbinding10,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
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
			new CatalogueCommodityRecipeInput(Grams: 390.0, Material: "agate", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("390 grams of agate", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Knife", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Knife tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "medieval_writing_agate_inkstone", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a fine agate inkstone (@medieval_writing_agate_inkstone)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition WritingAndAdministrationMakeBookbinderSBackingHammer() => new()
	{
		Metadata = new("Writing and Administration/make bookbinder's backing hammer", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:medieval_writing_backing_hammer"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Medieval.cs:893", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make bookbinder's backing hammer",
		Category = "Writing and Administration",
		Blurb = "make a bookbinder's backing hammer",
		Action = "makeing a bookbinder's backing hammer",
		ActiveCraftItemSdesc = "an in-progress bookbinder's backing hammer craft",
		Gate = CatalogueRecipeGates.MedievalWritingAndAdministrationBookbinding10,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
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
			new CatalogueCommodityRecipeInput(Grams: 465.0, Material: "wrought iron", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("465 grams of wrought iron", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Knife", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Knife tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "medieval_writing_backing_hammer", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a bookbinder's backing hammer (@medieval_writing_backing_hammer)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition WritingAndAdministrationMakeBambooDocumentTag() => new()
	{
		Metadata = new("Writing and Administration/make bamboo document tag", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:medieval_writing_bamboo_document_tag"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Medieval.cs:893", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make bamboo document tag",
		Category = "Writing and Administration",
		Blurb = "make a bamboo document tag",
		Action = "makeing a bamboo document tag",
		ActiveCraftItemSdesc = "an in-progress bamboo document tag craft",
		Gate = CatalogueRecipeGates.MedievalWritingAndAdministrationBookbinding10,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
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
			new CatalogueCommodityRecipeInput(Grams: 9.0, Material: "bamboo", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("9 grams of bamboo", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Knife", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Knife tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "medieval_writing_bamboo_document_tag", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a bamboo document tag (@medieval_writing_bamboo_document_tag)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition WritingAndAdministrationMakeBambooPenRack() => new()
	{
		Metadata = new("Writing and Administration/make bamboo pen rack", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:medieval_writing_bamboo_pen_rack"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Medieval.cs:893", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make bamboo pen rack",
		Category = "Writing and Administration",
		Blurb = "make a bamboo pen rack",
		Action = "makeing a bamboo pen rack",
		ActiveCraftItemSdesc = "an in-progress bamboo pen rack craft",
		Gate = CatalogueRecipeGates.MedievalWritingAndAdministrationBookbinding10,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
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
			new CatalogueCommodityRecipeInput(Grams: 135.0, Material: "bamboo", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("135 grams of bamboo", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Knife", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Knife tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "medieval_writing_bamboo_pen_rack", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a bamboo pen rack (@medieval_writing_bamboo_pen_rack)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition WritingAndAdministrationMakeBambooPracticePanel() => new()
	{
		Metadata = new("Writing and Administration/make bamboo practice panel", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:medieval_writing_bamboo_practice_panel"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Medieval.cs:893", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make bamboo practice panel",
		Category = "Writing and Administration",
		Blurb = "make a bamboo practice panel",
		Action = "makeing a bamboo practice panel",
		ActiveCraftItemSdesc = "an in-progress bamboo practice panel craft",
		Gate = CatalogueRecipeGates.MedievalWritingAndAdministrationBookbinding10,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
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
			new CatalogueCommodityRecipeInput(Grams: 120.0, Material: "bamboo", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("120 grams of bamboo", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Knife", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Knife tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "medieval_writing_bamboo_practice_panel", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a bamboo practice panel (@medieval_writing_bamboo_practice_panel)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition WritingAndAdministrationMakeBambooPrintingBoard() => new()
	{
		Metadata = new("Writing and Administration/make bamboo printing board", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:medieval_writing_bamboo_printing_board"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Medieval.cs:893", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make bamboo printing board",
		Category = "Writing and Administration",
		Blurb = "make a bamboo printing board",
		Action = "makeing a bamboo printing board",
		ActiveCraftItemSdesc = "an in-progress bamboo printing board craft",
		Gate = CatalogueRecipeGates.MedievalWritingAndAdministrationBookbinding10,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
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
			new CatalogueCommodityRecipeInput(Grams: 735.0, Material: "bamboo", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("735 grams of bamboo", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Knife", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Knife tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "medieval_writing_bamboo_printing_board", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a bamboo printing board (@medieval_writing_bamboo_printing_board)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};
}
