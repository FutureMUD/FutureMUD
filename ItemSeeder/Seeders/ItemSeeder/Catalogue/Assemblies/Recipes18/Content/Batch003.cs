#nullable enable

using MudSharp.Body;
using MudSharp.GameItems.Inventory.Plans;
using MudSharp.RPG.Checks;

namespace DatabaseSeeder.Seeders;

public static partial class Recipes18Stock
{
	public static CatalogueRecipeDefinition MedicalAndRepairPrepareLegBrace() => new()
	{
		Metadata = new("Medical and repair/prepare leg brace", "crafts", ["renaissance"], ["item:renaissance_medical_leg_brace"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "prepare leg brace",
		Category = "Medical and repair",
		Blurb = "prepare a leg brace",
		Action = "preparing a medical or repair item",
		ActiveCraftItemSdesc = "an in-progress leg brace craft",
		Gate = CatalogueRecipeGates.RenaissanceFinishedItemCraftingApothecary15,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 3,
		Interruptable = false,
		Phases = [
			new(25, "$0 inspect|inspects $i1 and trim|trims it with $t1.", "$0 inspect|inspects $i1 and trim|trims it with $t1.", ExertionLevel.Stasis, 0),
			new(35, "$0 measure|measures the prepared material and work|works it with $t2. $0 clear|clears the work area before the final steps.", "$0 measure|measures the prepared material and work|works it with $t2. $0 clear|clears the work area before the final steps.", ExertionLevel.Stasis, 0),
			new(25, "$0 pack|packs the finished preparation and set|sets aside $p1.", "$0 discard|discards the spoiled preparation and set|sets the material aside.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 875.0, Material: "oak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("875 grams of oak", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Shears", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Shears tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Awl Punch", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Awl Punch tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_medical_leg_brace", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a leg brace (@renaissance_medical_leg_brace)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MedicalAndRepairPrepareLivestockTruss() => new()
	{
		Metadata = new("Medical and repair/prepare livestock truss", "crafts", ["renaissance"], ["item:renaissance_medical_livestock_truss"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "prepare livestock truss",
		Category = "Medical and repair",
		Blurb = "prepare a livestock truss",
		Action = "preparing a medical or repair item",
		ActiveCraftItemSdesc = "an in-progress livestock truss craft",
		Gate = CatalogueRecipeGates.RenaissanceFinishedItemCraftingApothecary15,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 3,
		Interruptable = false,
		Phases = [
			new(25, "$0 inspect|inspects $i1 and trim|trims it with $t1.", "$0 inspect|inspects $i1 and trim|trims it with $t1.", ExertionLevel.Stasis, 0),
			new(35, "$0 measure|measures the prepared material and work|works it with $t2. $0 clear|clears the work area before the final steps.", "$0 measure|measures the prepared material and work|works it with $t2. $0 clear|clears the work area before the final steps.", ExertionLevel.Stasis, 0),
			new(25, "$0 pack|packs the finished preparation and set|sets aside $p1.", "$0 discard|discards the spoiled preparation and set|sets the material aside.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 175.0, Material: "linen", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("175 grams of linen", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Shears", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Shears tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Awl Punch", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Awl Punch tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_medical_livestock_truss", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a livestock truss (@renaissance_medical_livestock_truss)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MedicalAndRepairPrepareMidwifeHook() => new()
	{
		Metadata = new("Medical and repair/prepare midwife hook", "crafts", ["renaissance"], ["item:renaissance_medical_midwife_hook"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "prepare midwife hook",
		Category = "Medical and repair",
		Blurb = "prepare a midwife hook",
		Action = "preparing a medical or repair item",
		ActiveCraftItemSdesc = "an in-progress midwife hook craft",
		Gate = CatalogueRecipeGates.RenaissanceFinishedItemCraftingApothecary15,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 3,
		Interruptable = false,
		Phases = [
			new(25, "$0 inspect|inspects $i1 and trim|trims it with $t1.", "$0 inspect|inspects $i1 and trim|trims it with $t1.", ExertionLevel.Stasis, 0),
			new(35, "$0 measure|measures the prepared material and work|works it with $t2. $0 pause|pauses to check the proportions before carrying on.", "$0 measure|measures the prepared material and work|works it with $t2. $0 pause|pauses to check the proportions before carrying on.", ExertionLevel.Stasis, 0),
			new(25, "$0 pack|packs the finished preparation and set|sets aside $p1.", "$0 discard|discards the spoiled preparation and set|sets the material aside.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 175.0, Material: "linen", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("175 grams of linen", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Shears", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Shears tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Awl Punch", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Awl Punch tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_medical_midwife_hook", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a midwife hook (@renaissance_medical_midwife_hook)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MedicalAndRepairPrepareMummyPowderJar() => new()
	{
		Metadata = new("Medical and repair/prepare mummy powder jar", "crafts", ["renaissance"], ["item:renaissance_medical_mummy_powder_jar"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "prepare mummy powder jar",
		Category = "Medical and repair",
		Blurb = "prepare a mummy powder jar",
		Action = "preparing a medical or repair item",
		ActiveCraftItemSdesc = "an in-progress mummy powder jar craft",
		Gate = CatalogueRecipeGates.RenaissanceFinishedItemCraftingApothecary15,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 3,
		Interruptable = false,
		Phases = [
			new(25, "$0 inspect|inspects $i1 and trim|trims it with $t1.", "$0 inspect|inspects $i1 and trim|trims it with $t1.", ExertionLevel.Stasis, 0),
			new(35, "$0 measure|measures the prepared material and work|works it with $t2. $0 measure|measures the developing form against the intended finish.", "$0 measure|measures the prepared material and work|works it with $t2. $0 measure|measures the developing form against the intended finish.", ExertionLevel.Stasis, 0),
			new(25, "$0 pack|packs the finished preparation and set|sets aside $p1.", "$0 discard|discards the spoiled preparation and set|sets the material aside.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 25.0, Material: "bone", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("25 grams of bone", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Shears", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Shears tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Awl Punch", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Awl Punch tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_medical_mummy_powder_jar", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a mummy powder jar (@renaissance_medical_mummy_powder_jar)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MedicalAndRepairPrepareMyrrhResinStock() => new()
	{
		Metadata = new("Medical and repair/prepare myrrh resin stock", "crafts", ["renaissance"], ["item:renaissance_medical_myrrh_resin_stock"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "prepare myrrh resin stock",
		Category = "Medical and repair",
		Blurb = "prepare a myrrh resin stock",
		Action = "preparing a medical or repair item",
		ActiveCraftItemSdesc = "an in-progress myrrh resin stock craft",
		Gate = CatalogueRecipeGates.RenaissanceFinishedItemCraftingApothecary15,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 3,
		Interruptable = false,
		Phases = [
			new(25, "$0 inspect|inspects $i1 and trim|trims it with $t1.", "$0 inspect|inspects $i1 and trim|trims it with $t1.", ExertionLevel.Stasis, 0),
			new(35, "$0 measure|measures the prepared material and work|works it with $t2. $0 measure|measures the developing form against the intended finish.", "$0 measure|measures the prepared material and work|works it with $t2. $0 measure|measures the developing form against the intended finish.", ExertionLevel.Stasis, 0),
			new(25, "$0 pack|packs the finished preparation and set|sets aside $p1.", "$0 discard|discards the spoiled preparation and set|sets the material aside.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 63.0, Material: "myrrh resin", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("63 grams of myrrh resin", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Shears", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Shears tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Awl Punch", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Awl Punch tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_medical_myrrh_resin_stock", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a myrrh resin stock (@renaissance_medical_myrrh_resin_stock)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MedicalAndRepairPrepareNeckSupport() => new()
	{
		Metadata = new("Medical and repair/prepare neck support", "crafts", ["renaissance"], ["item:renaissance_medical_neck_support"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "prepare neck support",
		Category = "Medical and repair",
		Blurb = "prepare a neck support",
		Action = "preparing a medical or repair item",
		ActiveCraftItemSdesc = "an in-progress neck support craft",
		Gate = CatalogueRecipeGates.RenaissanceFinishedItemCraftingApothecary15,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 3,
		Interruptable = false,
		Phases = [
			new(25, "$0 inspect|inspects $i1 and trim|trims it with $t1.", "$0 inspect|inspects $i1 and trim|trims it with $t1.", ExertionLevel.Stasis, 0),
			new(35, "$0 measure|measures the prepared material and work|works it with $t2. $0 measure|measures the developing form against the intended finish.", "$0 measure|measures the prepared material and work|works it with $t2. $0 measure|measures the developing form against the intended finish.", ExertionLevel.Stasis, 0),
			new(25, "$0 pack|packs the finished preparation and set|sets aside $p1.", "$0 discard|discards the spoiled preparation and set|sets the material aside.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 175.0, Material: "linen", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("175 grams of linen", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Shears", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Shears tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Awl Punch", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Awl Punch tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_medical_neck_support", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a neck support (@renaissance_medical_neck_support)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MedicalAndRepairPrepareNestedWeightSet() => new()
	{
		Metadata = new("Medical and repair/prepare nested weight set", "crafts", ["renaissance"], ["item:renaissance_medical_nested_weight_set"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "prepare nested weight set",
		Category = "Medical and repair",
		Blurb = "prepare a nested weight set",
		Action = "preparing a medical or repair item",
		ActiveCraftItemSdesc = "an in-progress nested weight set craft",
		Gate = CatalogueRecipeGates.RenaissanceFinishedItemCraftingApothecary15,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 3,
		Interruptable = false,
		Phases = [
			new(25, "$0 inspect|inspects $i1 and trim|trims it with $t1.", "$0 inspect|inspects $i1 and trim|trims it with $t1.", ExertionLevel.Stasis, 0),
			new(35, "$0 measure|measures the prepared material and work|works it with $t2. $0 pause|pauses to check the proportions before carrying on.", "$0 measure|measures the prepared material and work|works it with $t2. $0 pause|pauses to check the proportions before carrying on.", ExertionLevel.Stasis, 0),
			new(25, "$0 pack|packs the finished preparation and set|sets aside $p1.", "$0 discard|discards the spoiled preparation and set|sets the material aside.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 294.0, Material: "brass", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("294 grams of brass", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Shears", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Shears tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Awl Punch", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Awl Punch tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_medical_nested_weight_set", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a nested weight set (@renaissance_medical_nested_weight_set)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MedicalAndRepairPrepareOintmentSpatula() => new()
	{
		Metadata = new("Medical and repair/prepare ointment spatula", "crafts", ["renaissance"], ["item:renaissance_medical_ointment_spatula"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "prepare ointment spatula",
		Category = "Medical and repair",
		Blurb = "prepare an ointment spatula",
		Action = "preparing a medical or repair item",
		ActiveCraftItemSdesc = "an in-progress ointment spatula craft",
		Gate = CatalogueRecipeGates.RenaissanceFinishedItemCraftingApothecary15,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 3,
		Interruptable = false,
		Phases = [
			new(25, "$0 inspect|inspects $i1 and trim|trims it with $t1.", "$0 inspect|inspects $i1 and trim|trims it with $t1.", ExertionLevel.Stasis, 0),
			new(35, "$0 measure|measures the prepared material and work|works it with $t2. $0 pause|pauses to check the proportions before carrying on.", "$0 measure|measures the prepared material and work|works it with $t2. $0 pause|pauses to check the proportions before carrying on.", ExertionLevel.Stasis, 0),
			new(25, "$0 pack|packs the finished preparation and set|sets aside $p1.", "$0 discard|discards the spoiled preparation and set|sets the material aside.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 67.0, Material: "beeswax", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("67 grams of beeswax", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Shears", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Shears tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Awl Punch", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Awl Punch tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_medical_ointment_spatula", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x an ointment spatula (@renaissance_medical_ointment_spatula)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MedicalAndRepairPrepareOpiumGumStock() => new()
	{
		Metadata = new("Medical and repair/prepare opium gum stock", "crafts", ["renaissance"], ["item:renaissance_medical_opium_gum_stock"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "prepare opium gum stock",
		Category = "Medical and repair",
		Blurb = "prepare an opium gum stock",
		Action = "preparing a medical or repair item",
		ActiveCraftItemSdesc = "an in-progress opium gum stock craft",
		Gate = CatalogueRecipeGates.RenaissanceFinishedItemCraftingApothecary15,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 3,
		Interruptable = false,
		Phases = [
			new(25, "$0 inspect|inspects $i1 and trim|trims it with $t1.", "$0 inspect|inspects $i1 and trim|trims it with $t1.", ExertionLevel.Stasis, 0),
			new(35, "$0 measure|measures the prepared material and work|works it with $t2. $0 pause|pauses to check the proportions before carrying on.", "$0 measure|measures the prepared material and work|works it with $t2. $0 pause|pauses to check the proportions before carrying on.", ExertionLevel.Stasis, 0),
			new(25, "$0 pack|packs the finished preparation and set|sets aside $p1.", "$0 discard|discards the spoiled preparation and set|sets the material aside.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 63.0, Material: "opium gum", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("63 grams of opium gum", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Shears", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Shears tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Awl Punch", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Awl Punch tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_medical_opium_gum_stock", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x an opium gum stock (@renaissance_medical_opium_gum_stock)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MedicalAndRepairPreparePaddedSplint() => new()
	{
		Metadata = new("Medical and repair/prepare padded splint", "crafts", ["renaissance"], ["item:renaissance_medical_padded_splint"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "prepare padded splint",
		Category = "Medical and repair",
		Blurb = "prepare a padded splint",
		Action = "preparing a medical or repair item",
		ActiveCraftItemSdesc = "an in-progress padded splint craft",
		Gate = CatalogueRecipeGates.RenaissanceFinishedItemCraftingApothecary15,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 3,
		Interruptable = false,
		Phases = [
			new(25, "$0 inspect|inspects $i1 and trim|trims it with $t1.", "$0 inspect|inspects $i1 and trim|trims it with $t1.", ExertionLevel.Stasis, 0),
			new(35, "$0 measure|measures the prepared material and work|works it with $t2. $0 pause|pauses to check the proportions before carrying on.", "$0 measure|measures the prepared material and work|works it with $t2. $0 pause|pauses to check the proportions before carrying on.", ExertionLevel.Stasis, 0),
			new(25, "$0 pack|packs the finished preparation and set|sets aside $p1.", "$0 discard|discards the spoiled preparation and set|sets the material aside.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 294.0, Material: "linen", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("294 grams of linen", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Shears", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Shears tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Awl Punch", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Awl Punch tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_medical_padded_splint", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a padded splint (@renaissance_medical_padded_splint)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MedicalAndRepairPrepareParchmentDosePacket() => new()
	{
		Metadata = new("Medical and repair/prepare parchment dose packet", "crafts", ["renaissance"], ["item:renaissance_medical_parchment_dose_packet"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "prepare parchment dose packet",
		Category = "Medical and repair",
		Blurb = "prepare a parchment dose packet",
		Action = "preparing a medical or repair item",
		ActiveCraftItemSdesc = "an in-progress parchment dose packet craft",
		Gate = CatalogueRecipeGates.RenaissanceFinishedItemCraftingApothecary15,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 3,
		Interruptable = false,
		Phases = [
			new(25, "$0 inspect|inspects $i1 and trim|trims it with $t1.", "$0 inspect|inspects $i1 and trim|trims it with $t1.", ExertionLevel.Stasis, 0),
			new(35, "$0 measure|measures the prepared material and work|works it with $t2. $0 pause|pauses to check the proportions before carrying on.", "$0 measure|measures the prepared material and work|works it with $t2. $0 pause|pauses to check the proportions before carrying on.", ExertionLevel.Stasis, 0),
			new(25, "$0 pack|packs the finished preparation and set|sets aside $p1.", "$0 discard|discards the spoiled preparation and set|sets the material aside.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 25.0, Material: "parchment", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("25 grams of parchment", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Shears", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Shears tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Awl Punch", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Awl Punch tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_medical_parchment_dose_packet", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a parchment dose packet (@renaissance_medical_parchment_dose_packet)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MedicalAndRepairPreparePegLeg() => new()
	{
		Metadata = new("Medical and repair/prepare peg leg", "crafts", ["renaissance"], ["item:renaissance_medical_peg_leg"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "prepare peg leg",
		Category = "Medical and repair",
		Blurb = "prepare a peg leg",
		Action = "preparing a medical or repair item",
		ActiveCraftItemSdesc = "an in-progress peg leg craft",
		Gate = CatalogueRecipeGates.RenaissanceFinishedItemCraftingApothecary15,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 3,
		Interruptable = false,
		Phases = [
			new(25, "$0 inspect|inspects $i1 and trim|trims it with $t1.", "$0 inspect|inspects $i1 and trim|trims it with $t1.", ExertionLevel.Stasis, 0),
			new(35, "$0 measure|measures the prepared material and work|works it with $t2. $0 measure|measures the developing form against the intended finish.", "$0 measure|measures the prepared material and work|works it with $t2. $0 measure|measures the developing form against the intended finish.", ExertionLevel.Stasis, 0),
			new(25, "$0 pack|packs the finished preparation and set|sets aside $p1.", "$0 discard|discards the spoiled preparation and set|sets the material aside.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 875.0, Material: "oak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("875 grams of oak", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Shears", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Shears tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Awl Punch", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Awl Punch tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_medical_peg_leg", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a peg leg (@renaissance_medical_peg_leg)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MedicalAndRepairPreparePeruvianBalsamStock() => new()
	{
		Metadata = new("Medical and repair/prepare Peruvian balsam stock", "crafts", ["renaissance"], ["item:renaissance_medical_peruvian_balsam_stock"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "prepare Peruvian balsam stock",
		Category = "Medical and repair",
		Blurb = "prepare a Peruvian balsam stock",
		Action = "preparing a medical or repair item",
		ActiveCraftItemSdesc = "an in-progress Peruvian balsam stock craft",
		Gate = CatalogueRecipeGates.RenaissanceFinishedItemCraftingApothecary15,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 3,
		Interruptable = false,
		Phases = [
			new(25, "$0 inspect|inspects $i1 and trim|trims it with $t1.", "$0 inspect|inspects $i1 and trim|trims it with $t1.", ExertionLevel.Stasis, 0),
			new(35, "$0 measure|measures the prepared material and work|works it with $t2. $0 turn|turns the work over to judge its balance.", "$0 measure|measures the prepared material and work|works it with $t2. $0 turn|turns the work over to judge its balance.", ExertionLevel.Stasis, 0),
			new(25, "$0 pack|packs the finished preparation and set|sets aside $p1.", "$0 discard|discards the spoiled preparation and set|sets the material aside.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 63.0, Material: "Peruvian balsam", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("63 grams of Peruvian balsam", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Shears", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Shears tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Awl Punch", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Awl Punch tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_medical_peruvian_balsam_stock", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a Peruvian balsam stock (@renaissance_medical_peruvian_balsam_stock)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MedicalAndRepairPreparePesthouseBedRoll() => new()
	{
		Metadata = new("Medical and repair/prepare pesthouse bed roll", "crafts", ["renaissance"], ["item:renaissance_medical_pesthouse_bed_roll"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "prepare pesthouse bed roll",
		Category = "Medical and repair",
		Blurb = "prepare a pesthouse bed roll",
		Action = "preparing a medical or repair item",
		ActiveCraftItemSdesc = "an in-progress pesthouse bed roll craft",
		Gate = CatalogueRecipeGates.RenaissanceFinishedItemCraftingApothecary15,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 3,
		Interruptable = false,
		Phases = [
			new(25, "$0 inspect|inspects $i1 and trim|trims it with $t1.", "$0 inspect|inspects $i1 and trim|trims it with $t1.", ExertionLevel.Stasis, 0),
			new(35, "$0 measure|measures the prepared material and work|works it with $t2. $0 clear|clears the work area before the final steps.", "$0 measure|measures the prepared material and work|works it with $t2. $0 clear|clears the work area before the final steps.", ExertionLevel.Stasis, 0),
			new(25, "$0 pack|packs the finished preparation and set|sets aside $p1.", "$0 discard|discards the spoiled preparation and set|sets the material aside.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1260.0, Material: "leather", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1260 grams of leather", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Shears", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Shears tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Awl Punch", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Awl Punch tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_medical_pesthouse_bed_roll", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a pesthouse bed roll (@renaissance_medical_pesthouse_bed_roll)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MedicalAndRepairPreparePillTile() => new()
	{
		Metadata = new("Medical and repair/prepare pill tile", "crafts", ["renaissance"], ["item:renaissance_medical_pill_tile"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "prepare pill tile",
		Category = "Medical and repair",
		Blurb = "prepare a pill tile",
		Action = "preparing a medical or repair item",
		ActiveCraftItemSdesc = "an in-progress pill tile craft",
		Gate = CatalogueRecipeGates.RenaissanceFinishedItemCraftingApothecary15,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 3,
		Interruptable = false,
		Phases = [
			new(25, "$0 inspect|inspects $i1 and trim|trims it with $t1.", "$0 inspect|inspects $i1 and trim|trims it with $t1.", ExertionLevel.Stasis, 0),
			new(35, "$0 measure|measures the prepared material and work|works it with $t2. $0 turn|turns the work over to judge its balance.", "$0 measure|measures the prepared material and work|works it with $t2. $0 turn|turns the work over to judge its balance.", ExertionLevel.Stasis, 0),
			new(25, "$0 pack|packs the finished preparation and set|sets aside $p1.", "$0 discard|discards the spoiled preparation and set|sets the material aside.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 168.0, Material: "ceramic", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("168 grams of ceramic", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Shears", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Shears tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Awl Punch", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Awl Punch tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_medical_pill_tile", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a pill tile (@renaissance_medical_pill_tile)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MedicalAndRepairPreparePlaguePomander() => new()
	{
		Metadata = new("Medical and repair/prepare plague pomander", "crafts", ["renaissance"], ["item:renaissance_medical_plague_pomander"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "prepare plague pomander",
		Category = "Medical and repair",
		Blurb = "prepare a plague pomander",
		Action = "preparing a medical or repair item",
		ActiveCraftItemSdesc = "an in-progress plague pomander craft",
		Gate = CatalogueRecipeGates.RenaissanceFinishedItemCraftingApothecary15,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 3,
		Interruptable = false,
		Phases = [
			new(25, "$0 inspect|inspects $i1 and trim|trims it with $t1.", "$0 inspect|inspects $i1 and trim|trims it with $t1.", ExertionLevel.Stasis, 0),
			new(35, "$0 measure|measures the prepared material and work|works it with $t2. $0 pause|pauses to check the proportions before carrying on.", "$0 measure|measures the prepared material and work|works it with $t2. $0 pause|pauses to check the proportions before carrying on.", ExertionLevel.Stasis, 0),
			new(25, "$0 pack|packs the finished preparation and set|sets aside $p1.", "$0 discard|discards the spoiled preparation and set|sets the material aside.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 63.0, Material: "silver", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("63 grams of silver", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Shears", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Shears tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Awl Punch", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Awl Punch tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_medical_plague_pomander", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a plague pomander (@renaissance_medical_plague_pomander)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MedicalAndRepairPreparePowderedPearlPacket() => new()
	{
		Metadata = new("Medical and repair/prepare powdered pearl packet", "crafts", ["renaissance"], ["item:renaissance_medical_powdered_pearl_packet"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "prepare powdered pearl packet",
		Category = "Medical and repair",
		Blurb = "prepare a powdered pearl packet",
		Action = "preparing a medical or repair item",
		ActiveCraftItemSdesc = "an in-progress powdered pearl packet craft",
		Gate = CatalogueRecipeGates.RenaissanceFinishedItemCraftingApothecary15,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 3,
		Interruptable = false,
		Phases = [
			new(25, "$0 inspect|inspects $i1 and trim|trims it with $t1.", "$0 inspect|inspects $i1 and trim|trims it with $t1.", ExertionLevel.Stasis, 0),
			new(35, "$0 measure|measures the prepared material and work|works it with $t2. $0 turn|turns the work over to judge its balance.", "$0 measure|measures the prepared material and work|works it with $t2. $0 turn|turns the work over to judge its balance.", ExertionLevel.Stasis, 0),
			new(25, "$0 pack|packs the finished preparation and set|sets aside $p1.", "$0 discard|discards the spoiled preparation and set|sets the material aside.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 15.0, Material: "pearl", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("15 grams of pearl", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Shears", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Shears tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Awl Punch", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Awl Punch tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_medical_powdered_pearl_packet", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a powdered pearl packet (@renaissance_medical_powdered_pearl_packet)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MedicalAndRepairPreparePowderPaper() => new()
	{
		Metadata = new("Medical and repair/prepare powder paper", "crafts", ["renaissance"], ["item:renaissance_medical_powder_paper"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "prepare powder paper",
		Category = "Medical and repair",
		Blurb = "prepare a powder paper",
		Action = "preparing a medical or repair item",
		ActiveCraftItemSdesc = "an in-progress powder paper craft",
		Gate = CatalogueRecipeGates.RenaissanceFinishedItemCraftingApothecary15,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 3,
		Interruptable = false,
		Phases = [
			new(25, "$0 inspect|inspects $i1 and trim|trims it with $t1.", "$0 inspect|inspects $i1 and trim|trims it with $t1.", ExertionLevel.Stasis, 0),
			new(35, "$0 measure|measures the prepared material and work|works it with $t2. $0 turn|turns the work over to judge its balance.", "$0 measure|measures the prepared material and work|works it with $t2. $0 turn|turns the work over to judge its balance.", ExertionLevel.Stasis, 0),
			new(25, "$0 pack|packs the finished preparation and set|sets aside $p1.", "$0 discard|discards the spoiled preparation and set|sets the material aside.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 25.0, Material: "paper", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("25 grams of paper", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Shears", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Shears tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Awl Punch", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Awl Punch tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_medical_powder_paper", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a powder paper (@renaissance_medical_powder_paper)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MedicalAndRepairPreparePublicPumpKey() => new()
	{
		Metadata = new("Medical and repair/prepare public pump key", "crafts", ["renaissance"], ["item:renaissance_medical_public_pump_key"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "prepare public pump key",
		Category = "Medical and repair",
		Blurb = "prepare a public pump key",
		Action = "preparing a medical or repair item",
		ActiveCraftItemSdesc = "an in-progress public pump key craft",
		Gate = CatalogueRecipeGates.RenaissanceFinishedItemCraftingApothecary15,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 3,
		Interruptable = false,
		Phases = [
			new(25, "$0 inspect|inspects $i1 and trim|trims it with $t1.", "$0 inspect|inspects $i1 and trim|trims it with $t1.", ExertionLevel.Stasis, 0),
			new(35, "$0 measure|measures the prepared material and work|works it with $t2. $0 clear|clears the work area before the final steps.", "$0 measure|measures the prepared material and work|works it with $t2. $0 clear|clears the work area before the final steps.", ExertionLevel.Stasis, 0),
			new(25, "$0 pack|packs the finished preparation and set|sets aside $p1.", "$0 discard|discards the spoiled preparation and set|sets the material aside.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 126.0, Material: "wrought iron", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("126 grams of wrought iron", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Shears", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Shears tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Awl Punch", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Awl Punch tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_medical_public_pump_key", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a public pump key (@renaissance_medical_public_pump_key)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MedicalAndRepairPrepareQuarantineFlag() => new()
	{
		Metadata = new("Medical and repair/prepare quarantine flag", "crafts", ["renaissance"], ["item:renaissance_medical_quarantine_flag"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "prepare quarantine flag",
		Category = "Medical and repair",
		Blurb = "prepare a quarantine flag",
		Action = "preparing a medical or repair item",
		ActiveCraftItemSdesc = "an in-progress quarantine flag craft",
		Gate = CatalogueRecipeGates.RenaissanceFinishedItemCraftingApothecary15,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 3,
		Interruptable = false,
		Phases = [
			new(25, "$0 inspect|inspects $i1 and trim|trims it with $t1.", "$0 inspect|inspects $i1 and trim|trims it with $t1.", ExertionLevel.Stasis, 0),
			new(35, "$0 measure|measures the prepared material and work|works it with $t2. $0 measure|measures the developing form against the intended finish.", "$0 measure|measures the prepared material and work|works it with $t2. $0 measure|measures the developing form against the intended finish.", ExertionLevel.Stasis, 0),
			new(25, "$0 pack|packs the finished preparation and set|sets aside $p1.", "$0 discard|discards the spoiled preparation and set|sets the material aside.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 175.0, Material: "linen", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("175 grams of linen", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Shears", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Shears tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Awl Punch", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Awl Punch tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_medical_quarantine_flag", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a quarantine flag (@renaissance_medical_quarantine_flag)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MedicalAndRepairPrepareRhubarbRootStock() => new()
	{
		Metadata = new("Medical and repair/prepare rhubarb root stock", "crafts", ["renaissance"], ["item:renaissance_medical_rhubarb_root_stock"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "prepare rhubarb root stock",
		Category = "Medical and repair",
		Blurb = "prepare a rhubarb root stock",
		Action = "preparing a medical or repair item",
		ActiveCraftItemSdesc = "an in-progress rhubarb root stock craft",
		Gate = CatalogueRecipeGates.RenaissanceFinishedItemCraftingApothecary15,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 3,
		Interruptable = false,
		Phases = [
			new(25, "$0 inspect|inspects $i1 and trim|trims it with $t1.", "$0 inspect|inspects $i1 and trim|trims it with $t1.", ExertionLevel.Stasis, 0),
			new(35, "$0 measure|measures the prepared material and work|works it with $t2. $0 turn|turns the work over to judge its balance.", "$0 measure|measures the prepared material and work|works it with $t2. $0 turn|turns the work over to judge its balance.", ExertionLevel.Stasis, 0),
			new(25, "$0 pack|packs the finished preparation and set|sets aside $p1.", "$0 discard|discards the spoiled preparation and set|sets the material aside.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 63.0, Material: "rhubarb root", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("63 grams of rhubarb root", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Shears", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Shears tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Awl Punch", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Awl Punch tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_medical_rhubarb_root_stock", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a rhubarb root stock (@renaissance_medical_rhubarb_root_stock)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MedicalAndRepairPrepareRosemaryStock() => new()
	{
		Metadata = new("Medical and repair/prepare rosemary stock", "crafts", ["renaissance"], ["item:renaissance_medical_rosemary_stock"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "prepare rosemary stock",
		Category = "Medical and repair",
		Blurb = "prepare a rosemary stock",
		Action = "preparing a medical or repair item",
		ActiveCraftItemSdesc = "an in-progress rosemary stock craft",
		Gate = CatalogueRecipeGates.RenaissanceFinishedItemCraftingApothecary15,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 3,
		Interruptable = false,
		Phases = [
			new(25, "$0 inspect|inspects $i1 and trim|trims it with $t1.", "$0 inspect|inspects $i1 and trim|trims it with $t1.", ExertionLevel.Stasis, 0),
			new(35, "$0 measure|measures the prepared material and work|works it with $t2. $0 pause|pauses to check the proportions before carrying on.", "$0 measure|measures the prepared material and work|works it with $t2. $0 pause|pauses to check the proportions before carrying on.", ExertionLevel.Stasis, 0),
			new(25, "$0 pack|packs the finished preparation and set|sets aside $p1.", "$0 discard|discards the spoiled preparation and set|sets the material aside.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 63.0, Material: "rosemary", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("63 grams of rosemary", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Shears", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Shears tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Awl Punch", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Awl Punch tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_medical_rosemary_stock", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a rosemary stock (@renaissance_medical_rosemary_stock)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MedicalAndRepairPrepareRoyalTouchToken() => new()
	{
		Metadata = new("Medical and repair/prepare royal-touch token", "crafts", ["renaissance"], ["item:renaissance_medical_royal_touch_token"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "prepare royal-touch token",
		Category = "Medical and repair",
		Blurb = "prepare a royal-touch token",
		Action = "preparing a medical or repair item",
		ActiveCraftItemSdesc = "an in-progress royal-touch token craft",
		Gate = CatalogueRecipeGates.RenaissanceFinishedItemCraftingApothecary15,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 3,
		Interruptable = false,
		Phases = [
			new(25, "$0 inspect|inspects $i1 and trim|trims it with $t1.", "$0 inspect|inspects $i1 and trim|trims it with $t1.", ExertionLevel.Stasis, 0),
			new(35, "$0 measure|measures the prepared material and work|works it with $t2. $0 measure|measures the developing form against the intended finish.", "$0 measure|measures the prepared material and work|works it with $t2. $0 measure|measures the developing form against the intended finish.", ExertionLevel.Stasis, 0),
			new(25, "$0 pack|packs the finished preparation and set|sets aside $p1.", "$0 discard|discards the spoiled preparation and set|sets the material aside.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 15.0, Material: "silver", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("15 grams of silver", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Shears", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Shears tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Awl Punch", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Awl Punch tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_medical_royal_touch_token", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a royal-touch token (@renaissance_medical_royal_touch_token)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MedicalAndRepairPrepareSarsaparillaRootStock() => new()
	{
		Metadata = new("Medical and repair/prepare sarsaparilla root stock", "crafts", ["renaissance"], ["item:renaissance_medical_sarsaparilla_root_stock"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "prepare sarsaparilla root stock",
		Category = "Medical and repair",
		Blurb = "prepare a sarsaparilla root stock",
		Action = "preparing a medical or repair item",
		ActiveCraftItemSdesc = "an in-progress sarsaparilla root stock craft",
		Gate = CatalogueRecipeGates.RenaissanceFinishedItemCraftingApothecary15,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 3,
		Interruptable = false,
		Phases = [
			new(25, "$0 inspect|inspects $i1 and trim|trims it with $t1.", "$0 inspect|inspects $i1 and trim|trims it with $t1.", ExertionLevel.Stasis, 0),
			new(35, "$0 measure|measures the prepared material and work|works it with $t2. $0 measure|measures the developing form against the intended finish.", "$0 measure|measures the prepared material and work|works it with $t2. $0 measure|measures the developing form against the intended finish.", ExertionLevel.Stasis, 0),
			new(25, "$0 pack|packs the finished preparation and set|sets aside $p1.", "$0 discard|discards the spoiled preparation and set|sets the material aside.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 63.0, Material: "sarsaparilla root", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("63 grams of sarsaparilla root", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Shears", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Shears tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Awl Punch", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Awl Punch tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_medical_sarsaparilla_root_stock", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a sarsaparilla root stock (@renaissance_medical_sarsaparilla_root_stock)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MedicalAndRepairPrepareSassafrasBarkStock() => new()
	{
		Metadata = new("Medical and repair/prepare sassafras bark stock", "crafts", ["renaissance"], ["item:renaissance_medical_sassafras_bark_stock"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "prepare sassafras bark stock",
		Category = "Medical and repair",
		Blurb = "prepare a sassafras bark stock",
		Action = "preparing a medical or repair item",
		ActiveCraftItemSdesc = "an in-progress sassafras bark stock craft",
		Gate = CatalogueRecipeGates.RenaissanceFinishedItemCraftingApothecary15,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 3,
		Interruptable = false,
		Phases = [
			new(25, "$0 inspect|inspects $i1 and trim|trims it with $t1.", "$0 inspect|inspects $i1 and trim|trims it with $t1.", ExertionLevel.Stasis, 0),
			new(35, "$0 measure|measures the prepared material and work|works it with $t2. $0 pause|pauses to check the proportions before carrying on.", "$0 measure|measures the prepared material and work|works it with $t2. $0 pause|pauses to check the proportions before carrying on.", ExertionLevel.Stasis, 0),
			new(25, "$0 pack|packs the finished preparation and set|sets aside $p1.", "$0 discard|discards the spoiled preparation and set|sets the material aside.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 63.0, Material: "sassafras bark", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("63 grams of sassafras bark", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Shears", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Shears tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Awl Punch", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Awl Punch tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_medical_sassafras_bark_stock", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a sassafras bark stock (@renaissance_medical_sassafras_bark_stock)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MedicalAndRepairPrepareSennaLeafStock() => new()
	{
		Metadata = new("Medical and repair/prepare senna leaf stock", "crafts", ["renaissance"], ["item:renaissance_medical_senna_leaf_stock"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "prepare senna leaf stock",
		Category = "Medical and repair",
		Blurb = "prepare a senna leaf stock",
		Action = "preparing a medical or repair item",
		ActiveCraftItemSdesc = "an in-progress senna leaf stock craft",
		Gate = CatalogueRecipeGates.RenaissanceFinishedItemCraftingApothecary15,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 3,
		Interruptable = false,
		Phases = [
			new(25, "$0 inspect|inspects $i1 and trim|trims it with $t1.", "$0 inspect|inspects $i1 and trim|trims it with $t1.", ExertionLevel.Stasis, 0),
			new(35, "$0 measure|measures the prepared material and work|works it with $t2. $0 pause|pauses to check the proportions before carrying on.", "$0 measure|measures the prepared material and work|works it with $t2. $0 pause|pauses to check the proportions before carrying on.", ExertionLevel.Stasis, 0),
			new(25, "$0 pack|packs the finished preparation and set|sets aside $p1.", "$0 discard|discards the spoiled preparation and set|sets the material aside.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 63.0, Material: "senna leaf", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("63 grams of senna leaf", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Shears", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Shears tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Awl Punch", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Awl Punch tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_medical_senna_leaf_stock", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a senna leaf stock (@renaissance_medical_senna_leaf_stock)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MedicalAndRepairPrepareSickroomChamberPot() => new()
	{
		Metadata = new("Medical and repair/prepare sickroom chamber pot", "crafts", ["renaissance"], ["item:renaissance_medical_sickroom_chamber_pot"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "prepare sickroom chamber pot",
		Category = "Medical and repair",
		Blurb = "prepare a sickroom chamber pot",
		Action = "preparing a medical or repair item",
		ActiveCraftItemSdesc = "an in-progress sickroom chamber pot craft",
		Gate = CatalogueRecipeGates.RenaissanceFinishedItemCraftingApothecary15,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 3,
		Interruptable = false,
		Phases = [
			new(25, "$0 inspect|inspects $i1 and trim|trims it with $t1.", "$0 inspect|inspects $i1 and trim|trims it with $t1.", ExertionLevel.Stasis, 0),
			new(35, "$0 measure|measures the prepared material and work|works it with $t2. $0 turn|turns the work over to judge its balance.", "$0 measure|measures the prepared material and work|works it with $t2. $0 turn|turns the work over to judge its balance.", ExertionLevel.Stasis, 0),
			new(25, "$0 pack|packs the finished preparation and set|sets aside $p1.", "$0 discard|discards the spoiled preparation and set|sets the material aside.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 770.0, Material: "ceramic", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("770 grams of ceramic", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Shears", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Shears tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Awl Punch", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Awl Punch tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_medical_sickroom_chamber_pot", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a sickroom chamber pot (@renaissance_medical_sickroom_chamber_pot)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MedicalAndRepairPrepareSpeculum() => new()
	{
		Metadata = new("Medical and repair/prepare speculum", "crafts", ["renaissance"], ["item:renaissance_medical_speculum"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "prepare speculum",
		Category = "Medical and repair",
		Blurb = "prepare a speculum",
		Action = "preparing a medical or repair item",
		ActiveCraftItemSdesc = "an in-progress speculum craft",
		Gate = CatalogueRecipeGates.RenaissanceFinishedItemCraftingApothecary15,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 3,
		Interruptable = false,
		Phases = [
			new(25, "$0 inspect|inspects $i1 and trim|trims it with $t1.", "$0 inspect|inspects $i1 and trim|trims it with $t1.", ExertionLevel.Stasis, 0),
			new(35, "$0 measure|measures the prepared material and work|works it with $t2. $0 turn|turns the work over to judge its balance.", "$0 measure|measures the prepared material and work|works it with $t2. $0 turn|turns the work over to judge its balance.", ExertionLevel.Stasis, 0),
			new(25, "$0 pack|packs the finished preparation and set|sets aside $p1.", "$0 discard|discards the spoiled preparation and set|sets the material aside.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 126.0, Material: "wrought iron", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("126 grams of wrought iron", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Shears", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Shears tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Awl Punch", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Awl Punch tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_medical_speculum", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a speculum (@renaissance_medical_speculum)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MedicalAndRepairPrepareStableEyeWash() => new()
	{
		Metadata = new("Medical and repair/prepare stable eye wash", "crafts", ["renaissance"], ["item:renaissance_medical_stable_eye_wash"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "prepare stable eye wash",
		Category = "Medical and repair",
		Blurb = "prepare a stable eye wash",
		Action = "preparing a medical or repair item",
		ActiveCraftItemSdesc = "an in-progress stable eye wash craft",
		Gate = CatalogueRecipeGates.RenaissanceFinishedItemCraftingApothecary15,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 3,
		Interruptable = false,
		Phases = [
			new(25, "$0 inspect|inspects $i1 and trim|trims it with $t1.", "$0 inspect|inspects $i1 and trim|trims it with $t1.", ExertionLevel.Stasis, 0),
			new(35, "$0 measure|measures the prepared material and work|works it with $t2. $0 measure|measures the developing form against the intended finish.", "$0 measure|measures the prepared material and work|works it with $t2. $0 measure|measures the developing form against the intended finish.", ExertionLevel.Stasis, 0),
			new(25, "$0 pack|packs the finished preparation and set|sets aside $p1.", "$0 discard|discards the spoiled preparation and set|sets the material aside.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 175.0, Material: "linen", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("175 grams of linen", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Shears", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Shears tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Awl Punch", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Awl Punch tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_medical_stable_eye_wash", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a stable eye wash (@renaissance_medical_stable_eye_wash)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MedicalAndRepairPrepareStopperedCordialBottle() => new()
	{
		Metadata = new("Medical and repair/prepare stoppered cordial bottle", "crafts", ["renaissance"], ["item:renaissance_medical_stoppered_cordial_bottle"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "prepare stoppered cordial bottle",
		Category = "Medical and repair",
		Blurb = "prepare a stoppered cordial bottle",
		Action = "preparing a medical or repair item",
		ActiveCraftItemSdesc = "an in-progress stoppered cordial bottle craft",
		Gate = CatalogueRecipeGates.RenaissanceFinishedItemCraftingApothecary15,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 3,
		Interruptable = false,
		Phases = [
			new(25, "$0 inspect|inspects $i1 and trim|trims it with $t1.", "$0 inspect|inspects $i1 and trim|trims it with $t1.", ExertionLevel.Stasis, 0),
			new(35, "$0 measure|measures the prepared material and work|works it with $t2. $0 clear|clears the work area before the final steps.", "$0 measure|measures the prepared material and work|works it with $t2. $0 clear|clears the work area before the final steps.", ExertionLevel.Stasis, 0),
			new(25, "$0 pack|packs the finished preparation and set|sets aside $p1.", "$0 discard|discards the spoiled preparation and set|sets the material aside.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 112.0, Material: "glass", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("112 grams of glass", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Shears", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Shears tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Awl Punch", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Awl Punch tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_medical_stoppered_cordial_bottle", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a stoppered cordial bottle (@renaissance_medical_stoppered_cordial_bottle)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MedicalAndRepairPrepareSurgeonFieldRoll() => new()
	{
		Metadata = new("Medical and repair/prepare surgeon field roll", "crafts", ["renaissance"], ["item:renaissance_medical_surgeon_field_roll"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "prepare surgeon field roll",
		Category = "Medical and repair",
		Blurb = "prepare a surgeon field roll",
		Action = "preparing a medical or repair item",
		ActiveCraftItemSdesc = "an in-progress surgeon field roll craft",
		Gate = CatalogueRecipeGates.RenaissanceFinishedItemCraftingApothecary15,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 3,
		Interruptable = false,
		Phases = [
			new(25, "$0 inspect|inspects $i1 and trim|trims it with $t1.", "$0 inspect|inspects $i1 and trim|trims it with $t1.", ExertionLevel.Stasis, 0),
			new(35, "$0 measure|measures the prepared material and work|works it with $t2. $0 clear|clears the work area before the final steps.", "$0 measure|measures the prepared material and work|works it with $t2. $0 clear|clears the work area before the final steps.", ExertionLevel.Stasis, 0),
			new(25, "$0 pack|packs the finished preparation and set|sets aside $p1.", "$0 discard|discards the spoiled preparation and set|sets the material aside.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1260.0, Material: "leather", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1260 grams of leather", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Shears", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Shears tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Awl Punch", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Awl Punch tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_medical_surgeon_field_roll", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a surgeon field roll (@renaissance_medical_surgeon_field_roll)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MedicalAndRepairPrepareSurgicalScissors() => new()
	{
		Metadata = new("Medical and repair/prepare surgical scissors", "crafts", ["renaissance"], ["item:renaissance_medical_surgical_scissors"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "prepare surgical scissors",
		Category = "Medical and repair",
		Blurb = "prepare a surgical scissors",
		Action = "preparing a medical or repair item",
		ActiveCraftItemSdesc = "an in-progress surgical scissors craft",
		Gate = CatalogueRecipeGates.RenaissanceFinishedItemCraftingApothecary15,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 3,
		Interruptable = false,
		Phases = [
			new(25, "$0 inspect|inspects $i1 and trim|trims it with $t1.", "$0 inspect|inspects $i1 and trim|trims it with $t1.", ExertionLevel.Stasis, 0),
			new(35, "$0 measure|measures the prepared material and work|works it with $t2. $0 clear|clears the work area before the final steps.", "$0 measure|measures the prepared material and work|works it with $t2. $0 clear|clears the work area before the final steps.", ExertionLevel.Stasis, 0),
			new(25, "$0 pack|packs the finished preparation and set|sets aside $p1.", "$0 discard|discards the spoiled preparation and set|sets the material aside.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 126.0, Material: "wrought iron", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("126 grams of wrought iron", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Shears", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Shears tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Awl Punch", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Awl Punch tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_medical_surgical_scissors", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a surgical scissors (@renaissance_medical_surgical_scissors)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MedicalAndRepairPrepareSutureNeedle() => new()
	{
		Metadata = new("Medical and repair/prepare suture needle", "crafts", ["renaissance"], ["item:renaissance_medical_suture_needle"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "prepare suture needle",
		Category = "Medical and repair",
		Blurb = "prepare a suture needle",
		Action = "preparing a medical or repair item",
		ActiveCraftItemSdesc = "an in-progress suture needle craft",
		Gate = CatalogueRecipeGates.RenaissanceFinishedItemCraftingApothecary15,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 3,
		Interruptable = false,
		Phases = [
			new(25, "$0 inspect|inspects $i1 and trim|trims it with $t1.", "$0 inspect|inspects $i1 and trim|trims it with $t1.", ExertionLevel.Stasis, 0),
			new(35, "$0 measure|measures the prepared material and work|works it with $t2. $0 turn|turns the work over to judge its balance.", "$0 measure|measures the prepared material and work|works it with $t2. $0 turn|turns the work over to judge its balance.", ExertionLevel.Stasis, 0),
			new(25, "$0 pack|packs the finished preparation and set|sets aside $p1.", "$0 discard|discards the spoiled preparation and set|sets the material aside.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 126.0, Material: "wrought iron", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("126 grams of wrought iron", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Shears", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Shears tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Awl Punch", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Awl Punch tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_medical_suture_needle", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a suture needle (@renaissance_medical_suture_needle)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MedicalAndRepairPrepareSympatheticPowderVial() => new()
	{
		Metadata = new("Medical and repair/prepare sympathetic powder vial", "crafts", ["renaissance"], ["item:renaissance_medical_sympathetic_powder_vial"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "prepare sympathetic powder vial",
		Category = "Medical and repair",
		Blurb = "prepare a sympathetic powder vial",
		Action = "preparing a medical or repair item",
		ActiveCraftItemSdesc = "an in-progress sympathetic powder vial craft",
		Gate = CatalogueRecipeGates.RenaissanceFinishedItemCraftingApothecary15,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 3,
		Interruptable = false,
		Phases = [
			new(25, "$0 inspect|inspects $i1 and trim|trims it with $t1.", "$0 inspect|inspects $i1 and trim|trims it with $t1.", ExertionLevel.Stasis, 0),
			new(35, "$0 measure|measures the prepared material and work|works it with $t2. $0 clear|clears the work area before the final steps.", "$0 measure|measures the prepared material and work|works it with $t2. $0 clear|clears the work area before the final steps.", ExertionLevel.Stasis, 0),
			new(25, "$0 pack|packs the finished preparation and set|sets aside $p1.", "$0 discard|discards the spoiled preparation and set|sets the material aside.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 21.0, Material: "green vitriol", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("21 grams of green vitriol", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Shears", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Shears tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Awl Punch", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Awl Punch tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_medical_sympathetic_powder_vial", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a sympathetic powder vial (@renaissance_medical_sympathetic_powder_vial)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MedicalAndRepairPrepareSyrupFunnel() => new()
	{
		Metadata = new("Medical and repair/prepare syrup funnel", "crafts", ["renaissance"], ["item:renaissance_medical_syrup_funnel"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "prepare syrup funnel",
		Category = "Medical and repair",
		Blurb = "prepare a syrup funnel",
		Action = "preparing a medical or repair item",
		ActiveCraftItemSdesc = "an in-progress syrup funnel craft",
		Gate = CatalogueRecipeGates.RenaissanceFinishedItemCraftingApothecary15,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 3,
		Interruptable = false,
		Phases = [
			new(25, "$0 inspect|inspects $i1 and trim|trims it with $t1.", "$0 inspect|inspects $i1 and trim|trims it with $t1.", ExertionLevel.Stasis, 0),
			new(35, "$0 measure|measures the prepared material and work|works it with $t2. $0 turn|turns the work over to judge its balance.", "$0 measure|measures the prepared material and work|works it with $t2. $0 turn|turns the work over to judge its balance.", ExertionLevel.Stasis, 0),
			new(25, "$0 pack|packs the finished preparation and set|sets aside $p1.", "$0 discard|discards the spoiled preparation and set|sets the material aside.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 112.0, Material: "glass", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("112 grams of glass", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Shears", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Shears tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Awl Punch", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Awl Punch tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_medical_syrup_funnel", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a syrup funnel (@renaissance_medical_syrup_funnel)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MedicalAndRepairPrepareTinctureFlask() => new()
	{
		Metadata = new("Medical and repair/prepare tincture flask", "crafts", ["renaissance"], ["item:renaissance_medical_tincture_flask"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "prepare tincture flask",
		Category = "Medical and repair",
		Blurb = "prepare a tincture flask",
		Action = "preparing a medical or repair item",
		ActiveCraftItemSdesc = "an in-progress tincture flask craft",
		Gate = CatalogueRecipeGates.RenaissanceFinishedItemCraftingApothecary15,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 3,
		Interruptable = false,
		Phases = [
			new(25, "$0 inspect|inspects $i1 and trim|trims it with $t1.", "$0 inspect|inspects $i1 and trim|trims it with $t1.", ExertionLevel.Stasis, 0),
			new(35, "$0 measure|measures the prepared material and work|works it with $t2. $0 turn|turns the work over to judge its balance.", "$0 measure|measures the prepared material and work|works it with $t2. $0 turn|turns the work over to judge its balance.", ExertionLevel.Stasis, 0),
			new(25, "$0 pack|packs the finished preparation and set|sets aside $p1.", "$0 discard|discards the spoiled preparation and set|sets the material aside.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 112.0, Material: "glass", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("112 grams of glass", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Shears", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Shears tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Awl Punch", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Awl Punch tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_medical_tincture_flask", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a tincture flask (@renaissance_medical_tincture_flask)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MedicalAndRepairPrepareTravellingMedicineChest() => new()
	{
		Metadata = new("Medical and repair/prepare travelling medicine chest", "crafts", ["renaissance"], ["item:renaissance_medical_travelling_medicine_chest"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "prepare travelling medicine chest",
		Category = "Medical and repair",
		Blurb = "prepare a travelling medicine chest",
		Action = "preparing a medical or repair item",
		ActiveCraftItemSdesc = "an in-progress travelling medicine chest craft",
		Gate = CatalogueRecipeGates.RenaissanceFinishedItemCraftingApothecary15,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 3,
		Interruptable = false,
		Phases = [
			new(25, "$0 inspect|inspects $i1 and trim|trims it with $t1.", "$0 inspect|inspects $i1 and trim|trims it with $t1.", ExertionLevel.Stasis, 0),
			new(35, "$0 measure|measures the prepared material and work|works it with $t2. $0 pause|pauses to check the proportions before carrying on.", "$0 measure|measures the prepared material and work|works it with $t2. $0 pause|pauses to check the proportions before carrying on.", ExertionLevel.Stasis, 0),
			new(25, "$0 pack|packs the finished preparation and set|sets aside $p1.", "$0 discard|discards the spoiled preparation and set|sets the material aside.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1260.0, Material: "oak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1260 grams of oak", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Shears", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Shears tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Awl Punch", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Awl Punch tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_medical_travelling_medicine_chest", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a travelling medicine chest (@renaissance_medical_travelling_medicine_chest)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MedicalAndRepairPrepareTrephine() => new()
	{
		Metadata = new("Medical and repair/prepare trephine", "crafts", ["renaissance"], ["item:renaissance_medical_trephine"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "prepare trephine",
		Category = "Medical and repair",
		Blurb = "prepare a trephine",
		Action = "preparing a medical or repair item",
		ActiveCraftItemSdesc = "an in-progress trephine craft",
		Gate = CatalogueRecipeGates.RenaissanceFinishedItemCraftingApothecary15,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 3,
		Interruptable = false,
		Phases = [
			new(25, "$0 inspect|inspects $i1 and trim|trims it with $t1.", "$0 inspect|inspects $i1 and trim|trims it with $t1.", ExertionLevel.Stasis, 0),
			new(35, "$0 measure|measures the prepared material and work|works it with $t2. $0 pause|pauses to check the proportions before carrying on.", "$0 measure|measures the prepared material and work|works it with $t2. $0 pause|pauses to check the proportions before carrying on.", ExertionLevel.Stasis, 0),
			new(25, "$0 pack|packs the finished preparation and set|sets aside $p1.", "$0 discard|discards the spoiled preparation and set|sets the material aside.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 126.0, Material: "wrought iron", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("126 grams of wrought iron", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Shears", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Shears tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Awl Punch", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Awl Punch tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_medical_trephine", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a trephine (@renaissance_medical_trephine)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MedicalAndRepairPrepareTrussBelt() => new()
	{
		Metadata = new("Medical and repair/prepare truss belt", "crafts", ["renaissance"], ["item:renaissance_medical_truss_belt"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "prepare truss belt",
		Category = "Medical and repair",
		Blurb = "prepare a truss belt",
		Action = "preparing a medical or repair item",
		ActiveCraftItemSdesc = "an in-progress truss belt craft",
		Gate = CatalogueRecipeGates.RenaissanceFinishedItemCraftingApothecary15,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 3,
		Interruptable = false,
		Phases = [
			new(25, "$0 inspect|inspects $i1 and trim|trims it with $t1.", "$0 inspect|inspects $i1 and trim|trims it with $t1.", ExertionLevel.Stasis, 0),
			new(35, "$0 measure|measures the prepared material and work|works it with $t2. $0 measure|measures the developing form against the intended finish.", "$0 measure|measures the prepared material and work|works it with $t2. $0 measure|measures the developing form against the intended finish.", ExertionLevel.Stasis, 0),
			new(25, "$0 pack|packs the finished preparation and set|sets aside $p1.", "$0 discard|discards the spoiled preparation and set|sets the material aside.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 175.0, Material: "linen", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("175 grams of linen", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Shears", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Shears tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Awl Punch", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Awl Punch tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_medical_truss_belt", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a truss belt (@renaissance_medical_truss_belt)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MedicalAndRepairPrepareTurpentineResinStock() => new()
	{
		Metadata = new("Medical and repair/prepare turpentine resin stock", "crafts", ["renaissance"], ["item:renaissance_medical_turpentine_resin_stock"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "prepare turpentine resin stock",
		Category = "Medical and repair",
		Blurb = "prepare a turpentine resin stock",
		Action = "preparing a medical or repair item",
		ActiveCraftItemSdesc = "an in-progress turpentine resin stock craft",
		Gate = CatalogueRecipeGates.RenaissanceFinishedItemCraftingApothecary15,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 3,
		Interruptable = false,
		Phases = [
			new(25, "$0 inspect|inspects $i1 and trim|trims it with $t1.", "$0 inspect|inspects $i1 and trim|trims it with $t1.", ExertionLevel.Stasis, 0),
			new(35, "$0 measure|measures the prepared material and work|works it with $t2. $0 pause|pauses to check the proportions before carrying on.", "$0 measure|measures the prepared material and work|works it with $t2. $0 pause|pauses to check the proportions before carrying on.", ExertionLevel.Stasis, 0),
			new(25, "$0 pack|packs the finished preparation and set|sets aside $p1.", "$0 discard|discards the spoiled preparation and set|sets the material aside.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 63.0, Material: "turpentine resin", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("63 grams of turpentine resin", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Shears", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Shears tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Awl Punch", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Awl Punch tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_medical_turpentine_resin_stock", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a turpentine resin stock (@renaissance_medical_turpentine_resin_stock)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MedicalAndRepairPrepareVeterinaryDrenchBottle() => new()
	{
		Metadata = new("Medical and repair/prepare veterinary drench bottle", "crafts", ["renaissance"], ["item:renaissance_medical_veterinary_drench_bottle"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "prepare veterinary drench bottle",
		Category = "Medical and repair",
		Blurb = "prepare a veterinary drench bottle",
		Action = "preparing a medical or repair item",
		ActiveCraftItemSdesc = "an in-progress veterinary drench bottle craft",
		Gate = CatalogueRecipeGates.RenaissanceFinishedItemCraftingApothecary15,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 3,
		Interruptable = false,
		Phases = [
			new(25, "$0 inspect|inspects $i1 and trim|trims it with $t1.", "$0 inspect|inspects $i1 and trim|trims it with $t1.", ExertionLevel.Stasis, 0),
			new(35, "$0 measure|measures the prepared material and work|works it with $t2. $0 measure|measures the developing form against the intended finish.", "$0 measure|measures the prepared material and work|works it with $t2. $0 measure|measures the developing form against the intended finish.", ExertionLevel.Stasis, 0),
			new(25, "$0 pack|packs the finished preparation and set|sets aside $p1.", "$0 discard|discards the spoiled preparation and set|sets the material aside.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 112.0, Material: "glass", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("112 grams of glass", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Shears", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Shears tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Awl Punch", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Awl Punch tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_medical_veterinary_drench_bottle", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a veterinary drench bottle (@renaissance_medical_veterinary_drench_bottle)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MedicalAndRepairPrepareWardWashstand() => new()
	{
		Metadata = new("Medical and repair/prepare ward washstand", "crafts", ["renaissance"], ["item:renaissance_medical_ward_washstand"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "prepare ward washstand",
		Category = "Medical and repair",
		Blurb = "prepare a ward washstand",
		Action = "preparing a medical or repair item",
		ActiveCraftItemSdesc = "an in-progress ward washstand craft",
		Gate = CatalogueRecipeGates.RenaissanceFinishedItemCraftingApothecary15,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 3,
		Interruptable = false,
		Phases = [
			new(25, "$0 inspect|inspects $i1 and trim|trims it with $t1.", "$0 inspect|inspects $i1 and trim|trims it with $t1.", ExertionLevel.Stasis, 0),
			new(35, "$0 measure|measures the prepared material and work|works it with $t2. $0 clear|clears the work area before the final steps.", "$0 measure|measures the prepared material and work|works it with $t2. $0 clear|clears the work area before the final steps.", ExertionLevel.Stasis, 0),
			new(25, "$0 pack|packs the finished preparation and set|sets aside $p1.", "$0 discard|discards the spoiled preparation and set|sets the material aside.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1260.0, Material: "oak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1260 grams of oak", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Shears", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Shears tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Awl Punch", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Awl Punch tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_medical_ward_washstand", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a ward washstand (@renaissance_medical_ward_washstand)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MedicalAndRepairPrepareWashableSickbedBlanket() => new()
	{
		Metadata = new("Medical and repair/prepare washable sickbed blanket", "crafts", ["renaissance"], ["item:renaissance_medical_washable_sickbed_blanket"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "prepare washable sickbed blanket",
		Category = "Medical and repair",
		Blurb = "prepare a washable sickbed blanket",
		Action = "preparing a medical or repair item",
		ActiveCraftItemSdesc = "an in-progress washable sickbed blanket craft",
		Gate = CatalogueRecipeGates.RenaissanceFinishedItemCraftingApothecary15,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 3,
		Interruptable = false,
		Phases = [
			new(25, "$0 inspect|inspects $i1 and trim|trims it with $t1.", "$0 inspect|inspects $i1 and trim|trims it with $t1.", ExertionLevel.Stasis, 0),
			new(35, "$0 measure|measures the prepared material and work|works it with $t2. $0 pause|pauses to check the proportions before carrying on.", "$0 measure|measures the prepared material and work|works it with $t2. $0 pause|pauses to check the proportions before carrying on.", ExertionLevel.Stasis, 0),
			new(25, "$0 pack|packs the finished preparation and set|sets aside $p1.", "$0 discard|discards the spoiled preparation and set|sets the material aside.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 294.0, Material: "linen", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("294 grams of linen", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Shears", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Shears tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Awl Punch", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Awl Punch tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_medical_washable_sickbed_blanket", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a washable sickbed blanket (@renaissance_medical_washable_sickbed_blanket)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MedicalAndRepairPrepareWaxedMedicineVial() => new()
	{
		Metadata = new("Medical and repair/prepare waxed medicine vial", "crafts", ["renaissance"], ["item:renaissance_medical_waxed_medicine_vial"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "prepare waxed medicine vial",
		Category = "Medical and repair",
		Blurb = "prepare a waxed medicine vial",
		Action = "preparing a medical or repair item",
		ActiveCraftItemSdesc = "an in-progress waxed medicine vial craft",
		Gate = CatalogueRecipeGates.RenaissanceFinishedItemCraftingApothecary15,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 3,
		Interruptable = false,
		Phases = [
			new(25, "$0 inspect|inspects $i1 and trim|trims it with $t1.", "$0 inspect|inspects $i1 and trim|trims it with $t1.", ExertionLevel.Stasis, 0),
			new(35, "$0 measure|measures the prepared material and work|works it with $t2. $0 measure|measures the developing form against the intended finish.", "$0 measure|measures the prepared material and work|works it with $t2. $0 measure|measures the developing form against the intended finish.", ExertionLevel.Stasis, 0),
			new(25, "$0 pack|packs the finished preparation and set|sets aside $p1.", "$0 discard|discards the spoiled preparation and set|sets the material aside.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 112.0, Material: "glass", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("112 grams of glass", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Shears", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Shears tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Awl Punch", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Awl Punch tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_medical_waxed_medicine_vial", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a waxed medicine vial (@renaissance_medical_waxed_medicine_vial)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MedicalAndRepairPrepareWeaponSalvePot() => new()
	{
		Metadata = new("Medical and repair/prepare weapon salve pot", "crafts", ["renaissance"], ["item:renaissance_medical_weapon_salve_pot"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "prepare weapon salve pot",
		Category = "Medical and repair",
		Blurb = "prepare a weapon salve pot",
		Action = "preparing a medical or repair item",
		ActiveCraftItemSdesc = "an in-progress weapon salve pot craft",
		Gate = CatalogueRecipeGates.RenaissanceFinishedItemCraftingApothecary15,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 3,
		Interruptable = false,
		Phases = [
			new(25, "$0 inspect|inspects $i1 and trim|trims it with $t1.", "$0 inspect|inspects $i1 and trim|trims it with $t1.", ExertionLevel.Stasis, 0),
			new(35, "$0 measure|measures the prepared material and work|works it with $t2. $0 clear|clears the work area before the final steps.", "$0 measure|measures the prepared material and work|works it with $t2. $0 clear|clears the work area before the final steps.", ExertionLevel.Stasis, 0),
			new(25, "$0 pack|packs the finished preparation and set|sets aside $p1.", "$0 discard|discards the spoiled preparation and set|sets the material aside.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 63.0, Material: "beeswax", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("63 grams of beeswax", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Shears", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Shears tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Awl Punch", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Awl Punch tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_medical_weapon_salve_pot", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a weapon salve pot (@renaissance_medical_weapon_salve_pot)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MedicalAndRepairPrepareWoodenHand() => new()
	{
		Metadata = new("Medical and repair/prepare wooden hand", "crafts", ["renaissance"], ["item:renaissance_medical_wooden_hand"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "prepare wooden hand",
		Category = "Medical and repair",
		Blurb = "prepare a wooden hand",
		Action = "preparing a medical or repair item",
		ActiveCraftItemSdesc = "an in-progress wooden hand craft",
		Gate = CatalogueRecipeGates.RenaissanceFinishedItemCraftingApothecary15,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 3,
		Interruptable = false,
		Phases = [
			new(25, "$0 inspect|inspects $i1 and trim|trims it with $t1.", "$0 inspect|inspects $i1 and trim|trims it with $t1.", ExertionLevel.Stasis, 0),
			new(35, "$0 measure|measures the prepared material and work|works it with $t2. $0 turn|turns the work over to judge its balance.", "$0 measure|measures the prepared material and work|works it with $t2. $0 turn|turns the work over to judge its balance.", ExertionLevel.Stasis, 0),
			new(25, "$0 pack|packs the finished preparation and set|sets aside $p1.", "$0 discard|discards the spoiled preparation and set|sets the material aside.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 875.0, Material: "oak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("875 grams of oak", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Shears", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Shears tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Awl Punch", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Awl Punch tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_medical_wooden_hand", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a wooden hand (@renaissance_medical_wooden_hand)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MedicalAndRepairPrepareWormingBall() => new()
	{
		Metadata = new("Medical and repair/prepare worming ball", "crafts", ["renaissance"], ["item:renaissance_medical_worming_ball"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "prepare worming ball",
		Category = "Medical and repair",
		Blurb = "prepare a worming ball",
		Action = "preparing a medical or repair item",
		ActiveCraftItemSdesc = "an in-progress worming ball craft",
		Gate = CatalogueRecipeGates.RenaissanceFinishedItemCraftingApothecary15,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 3,
		Interruptable = false,
		Phases = [
			new(25, "$0 inspect|inspects $i1 and trim|trims it with $t1.", "$0 inspect|inspects $i1 and trim|trims it with $t1.", ExertionLevel.Stasis, 0),
			new(35, "$0 measure|measures the prepared material and work|works it with $t2. $0 measure|measures the developing form against the intended finish.", "$0 measure|measures the prepared material and work|works it with $t2. $0 measure|measures the developing form against the intended finish.", ExertionLevel.Stasis, 0),
			new(25, "$0 pack|packs the finished preparation and set|sets aside $p1.", "$0 discard|discards the spoiled preparation and set|sets the material aside.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 25.0, Material: "herb", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("25 grams of herb", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Shears", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Shears tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Awl Punch", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Awl Punch tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_medical_worming_ball", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a worming ball (@renaissance_medical_worming_ball)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MedicalAndRepairPrepareWoundProbe() => new()
	{
		Metadata = new("Medical and repair/prepare wound probe", "crafts", ["renaissance"], ["item:renaissance_medical_wound_probe"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "prepare wound probe",
		Category = "Medical and repair",
		Blurb = "prepare a wound probe",
		Action = "preparing a medical or repair item",
		ActiveCraftItemSdesc = "an in-progress wound probe craft",
		Gate = CatalogueRecipeGates.RenaissanceFinishedItemCraftingApothecary15,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 3,
		Interruptable = false,
		Phases = [
			new(25, "$0 inspect|inspects $i1 and trim|trims it with $t1.", "$0 inspect|inspects $i1 and trim|trims it with $t1.", ExertionLevel.Stasis, 0),
			new(35, "$0 measure|measures the prepared material and work|works it with $t2. $0 turn|turns the work over to judge its balance.", "$0 measure|measures the prepared material and work|works it with $t2. $0 turn|turns the work over to judge its balance.", ExertionLevel.Stasis, 0),
			new(25, "$0 pack|packs the finished preparation and set|sets aside $p1.", "$0 discard|discards the spoiled preparation and set|sets the material aside.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 126.0, Material: "wrought iron", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("126 grams of wrought iron", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Shears", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Shears tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Awl Punch", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Awl Punch tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_medical_wound_probe", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a wound probe (@renaissance_medical_wound_probe)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition GeneralCraftworkMakeColourBroadFeatheredHeadbandPattern2() => new()
	{
		Metadata = new("General craftwork/make $colour broad feathered headband pattern 2", "crafts", ["renaissance"], ["item:renaissance_mesoamerican_broad_featherheadband"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make $colour broad feathered headband pattern 2",
		Category = "General craftwork",
		Blurb = "make a $colour broad feathered headband",
		Action = "making a finished item",
		ActiveCraftItemSdesc = "an in-progress $colour broad feathered headband craft",
		Gate = CatalogueRecipeGates.RenaissanceFinishedItemCraftingLabouring15,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 3,
		Interruptable = false,
		Phases = [
			new(25, "$0 examine|examines $i1 and set|sets up the work with $t1.", "$0 examine|examines $i1 and set|sets up the work with $t1.", ExertionLevel.Stasis, 0),
			new(40, "$0 shape|shapes the prepared material, checking the form as it develops. $0 turn|turns the work over to judge its balance.", "$0 shape|shapes the prepared material, checking the form as it develops. $0 turn|turns the work over to judge its balance.", ExertionLevel.Stasis, 0),
			new(25, "$0 finish|finishes the work and set|sets aside $p1.", "$0 find|finds the work unsound and set|sets the material aside.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 143.0, Material: "featherwork", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("143 grams of featherwork", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_mesoamerican_broad_featherheadband", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a $colour broad feathered headband (@renaissance_mesoamerican_broad_featherheadband)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition CarpentryAndJoineryBuildColourCarvedFullFaceMask() => new()
	{
		Metadata = new("Carpentry and joinery/build $colour carved full-face mask", "crafts", ["renaissance"], ["item:renaissance_mesoamerican_carved_fullfacemask"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "build $colour carved full-face mask",
		Category = "Carpentry and joinery",
		Blurb = "build a $colour carved full-face mask",
		Action = "building a wooden item",
		ActiveCraftItemSdesc = "an in-progress $colour carved full-face mask craft",
		Gate = CatalogueRecipeGates.RenaissanceFinishedItemCraftingCarpentry15,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 3,
		Interruptable = false,
		Phases = [
			new(30, "$0 mark|marks $i1 and cut|cuts the fitted pieces with $t2.", "$0 mark|marks $i1 and cut|cuts the fitted pieces with $t2.", ExertionLevel.Stasis, 0),
			new(45, "$0 fit|fits the joins and secure|secures them with $t1. $0 pause|pauses to check the proportions before carrying on.", "$0 fit|fits the joins and secure|secures them with $t1. $0 pause|pauses to check the proportions before carrying on.", ExertionLevel.Stasis, 0),
			new(30, "$0 smooth|smooths the finished piece and set|sets aside $p1.", "$0 find|finds the join has opened and set|sets the material aside.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 184.0, Material: "wood", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("184 grams of wood", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Saw", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Saw tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_mesoamerican_carved_fullfacemask", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a $colour carved full-face mask (@renaissance_mesoamerican_carved_fullfacemask)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition GeneralCraftworkMakeColourFeatherEdgedWovenCrown() => new()
	{
		Metadata = new("General craftwork/make $colour feather-edged woven crown", "crafts", ["renaissance"], ["item:renaissance_mesoamerican_featheredge_crown"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make $colour feather-edged woven crown",
		Category = "General craftwork",
		Blurb = "make a $colour feather-edged woven crown",
		Action = "making a finished item",
		ActiveCraftItemSdesc = "an in-progress $colour feather-edged woven crown craft",
		Gate = CatalogueRecipeGates.RenaissanceFinishedItemCraftingLabouring15,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 3,
		Interruptable = false,
		Phases = [
			new(25, "$0 examine|examines $i1 and set|sets up the work with $t1.", "$0 examine|examines $i1 and set|sets up the work with $t1.", ExertionLevel.Stasis, 0),
			new(40, "$0 shape|shapes the prepared material, checking the form as it develops. $0 measure|measures the developing form against the intended finish.", "$0 shape|shapes the prepared material, checking the form as it develops. $0 measure|measures the developing form against the intended finish.", ExertionLevel.Stasis, 0),
			new(25, "$0 finish|finishes the work and set|sets aside $p1.", "$0 find|finds the work unsound and set|sets the material aside.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 143.0, Material: "feather", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("143 grams of feather", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_mesoamerican_featheredge_crown", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a $colour feather-edged woven crown (@renaissance_mesoamerican_featheredge_crown)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition GeneralCraftworkMakeColourMosaicFeatherMantle() => new()
	{
		Metadata = new("General craftwork/make $colour mosaic feather mantle", "crafts", ["renaissance"], ["item:renaissance_mesoamerican_featherwork_mantle"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make $colour mosaic feather mantle",
		Category = "General craftwork",
		Blurb = "make a $colour mosaic feather mantle",
		Action = "making a finished item",
		ActiveCraftItemSdesc = "an in-progress $colour mosaic feather mantle craft",
		Gate = CatalogueRecipeGates.RenaissanceFinishedItemCraftingLabouring15,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 3,
		Interruptable = false,
		Phases = [
			new(25, "$0 examine|examines $i1 and set|sets up the work with $t1.", "$0 examine|examines $i1 and set|sets up the work with $t1.", ExertionLevel.Stasis, 0),
			new(40, "$0 shape|shapes the prepared material, checking the form as it develops. $0 pause|pauses to check the proportions before carrying on.", "$0 shape|shapes the prepared material, checking the form as it develops. $0 pause|pauses to check the proportions before carrying on.", ExertionLevel.Stasis, 0),
			new(25, "$0 finish|finishes the work and set|sets aside $p1.", "$0 find|finds the work unsound and set|sets the material aside.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 465.0, Material: "featherwork", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("465 grams of featherwork", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_mesoamerican_featherwork_mantle", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a $colour mosaic feather mantle (@renaissance_mesoamerican_featherwork_mantle)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition GeneralCraftworkMakeColourPanelledFeatherTunic() => new()
	{
		Metadata = new("General craftwork/make $colour panelled feather tunic", "crafts", ["renaissance"], ["item:renaissance_mesoamerican_featherwork_tunic"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make $colour panelled feather tunic",
		Category = "General craftwork",
		Blurb = "make a $colour panelled feather tunic",
		Action = "making a finished item",
		ActiveCraftItemSdesc = "an in-progress $colour panelled feather tunic craft",
		Gate = CatalogueRecipeGates.RenaissanceFinishedItemCraftingLabouring15,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 3,
		Interruptable = false,
		Phases = [
			new(25, "$0 examine|examines $i1 and set|sets up the work with $t1.", "$0 examine|examines $i1 and set|sets up the work with $t1.", ExertionLevel.Stasis, 0),
			new(40, "$0 shape|shapes the prepared material, checking the form as it develops. $0 measure|measures the developing form against the intended finish.", "$0 shape|shapes the prepared material, checking the form as it develops. $0 measure|measures the developing form against the intended finish.", ExertionLevel.Stasis, 0),
			new(25, "$0 finish|finishes the work and set|sets aside $p1.", "$0 find|finds the work unsound and set|sets the material aside.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 360.0, Material: "featherwork", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("360 grams of featherwork", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_mesoamerican_featherwork_tunic", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a $colour panelled feather tunic (@renaissance_mesoamerican_featherwork_tunic)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition TextileAndLeatherWorkTailorColourLongFullCeremonialTunic() => new()
	{
		Metadata = new("Textile and leather work/tailor $colour long full ceremonial tunic", "crafts", ["renaissance"], ["item:renaissance_mesoamerican_full_ceremonialtunic"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "tailor $colour long full ceremonial tunic",
		Category = "Textile and leather work",
		Blurb = "tailor a $colour long full ceremonial tunic",
		Action = "tailoring a textile or leather item",
		ActiveCraftItemSdesc = "an in-progress $colour long full ceremonial tunic craft",
		Gate = CatalogueRecipeGates.RenaissanceFinishedItemCraftingTailoring15,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 3,
		Interruptable = false,
		Phases = [
			new(25, "$0 lay|lays out $i1 and mark|marks the working lines with $t1.", "$0 lay|lays out $i1 and mark|marks the working lines with $t1.", ExertionLevel.Stasis, 0),
			new(45, "$0 cut|cuts and join|joins the shaped material with $t1 and $t2. $0 pause|pauses to check the proportions before carrying on.", "$0 cut|cuts and join|joins the shaped material with $t1 and $t2. $0 pause|pauses to check the proportions before carrying on.", ExertionLevel.Stasis, 0),
			new(25, "$0 finish|finishes the seams and set|sets aside $p1.", "$0 find|finds the seams have failed and set|sets the material aside.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 735.0, Material: "cotton", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("735 grams of cotton", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Sewing Needle", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Sewing Needle tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Shears", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Shears tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_mesoamerican_full_ceremonialtunic", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a $colour long full ceremonial tunic (@renaissance_mesoamerican_full_ceremonialtunic)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition TextileAndLeatherWorkTailorColourHeelStrappedWovenSandals() => new()
	{
		Metadata = new("Textile and leather work/tailor $colour heel-strapped woven sandals", "crafts", ["renaissance"], ["item:renaissance_mesoamerican_heelstrap_sandals"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "tailor $colour heel-strapped woven sandals",
		Category = "Textile and leather work",
		Blurb = "tailor a pair of $colour heel-strapped woven sandals",
		Action = "tailoring a textile or leather item",
		ActiveCraftItemSdesc = "an in-progress $colour heel-strapped woven sandals craft",
		Gate = CatalogueRecipeGates.RenaissanceFinishedItemCraftingLeathermaking15,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 3,
		Interruptable = false,
		Phases = [
			new(25, "$0 lay|lays out $i1 and open|opens the stitch points with $t1.", "$0 lay|lays out $i1 and open|opens the stitch points with $t1.", ExertionLevel.Stasis, 0),
			new(40, "$0 cut|cuts the leather to shape with $t2 and work|works its edges true. $0 clear|clears the work area before the final steps.", "$0 cut|cuts the leather to shape with $t2 and work|works its edges true. $0 clear|clears the work area before the final steps.", ExertionLevel.Stasis, 0),
			new(25, "$0 burnish|burnishes the finished leatherwork and set|sets aside $p1.", "$0 split|splits the leatherwork and set|sets the material aside.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 315.0, Material: "leather", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("315 grams of leather", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Awl Punch", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Awl Punch tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Shears", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Shears tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_mesoamerican_heelstrap_sandals", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a pair of $colour heel-strapped woven sandals (@renaissance_mesoamerican_heelstrap_sandals)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition TextileAndLeatherWorkTailorLongColourCottonWrapSkirt() => new()
	{
		Metadata = new("Textile and leather work/tailor long $colour cotton wrap skirt", "crafts", ["renaissance"], ["item:renaissance_mesoamerican_long_wrapskirt"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "tailor long $colour cotton wrap skirt",
		Category = "Textile and leather work",
		Blurb = "tailor a long $colour cotton wrap skirt",
		Action = "tailoring a textile or leather item",
		ActiveCraftItemSdesc = "an in-progress long $colour cotton wrap skirt craft",
		Gate = CatalogueRecipeGates.RenaissanceFinishedItemCraftingTailoring15,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 3,
		Interruptable = false,
		Phases = [
			new(25, "$0 lay|lays out $i1 and mark|marks the working lines with $t1.", "$0 lay|lays out $i1 and mark|marks the working lines with $t1.", ExertionLevel.Stasis, 0),
			new(45, "$0 cut|cuts and join|joins the shaped material with $t1 and $t2. $0 pause|pauses to check the proportions before carrying on.", "$0 cut|cuts and join|joins the shaped material with $t1 and $t2. $0 pause|pauses to check the proportions before carrying on.", ExertionLevel.Stasis, 0),
			new(25, "$0 finish|finishes the seams and set|sets aside $p1.", "$0 find|finds the seams have failed and set|sets the material aside.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 473.0, Material: "cotton", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("473 grams of cotton", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Sewing Needle", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Sewing Needle tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Shears", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Shears tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_mesoamerican_long_wrapskirt", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a long $colour cotton wrap skirt (@renaissance_mesoamerican_long_wrapskirt)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition TextileAndLeatherWorkTailorColourPaintedBarkclothShoulderCape() => new()
	{
		Metadata = new("Textile and leather work/tailor $colour painted barkcloth shoulder cape", "crafts", ["renaissance"], ["item:renaissance_mesoamerican_painted_barkclothcape"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "tailor $colour painted barkcloth shoulder cape",
		Category = "Textile and leather work",
		Blurb = "tailor a $colour painted barkcloth shoulder cape",
		Action = "tailoring a textile or leather item",
		ActiveCraftItemSdesc = "an in-progress $colour painted barkcloth shoulder cape craft",
		Gate = CatalogueRecipeGates.RenaissanceFinishedItemCraftingTailoring15,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 3,
		Interruptable = false,
		Phases = [
			new(25, "$0 lay|lays out $i1 and mark|marks the working lines with $t1.", "$0 lay|lays out $i1 and mark|marks the working lines with $t1.", ExertionLevel.Stasis, 0),
			new(45, "$0 cut|cuts and join|joins the shaped material with $t1 and $t2. $0 turn|turns the work over to judge its balance.", "$0 cut|cuts and join|joins the shaped material with $t1 and $t2. $0 turn|turns the work over to judge its balance.", ExertionLevel.Stasis, 0),
			new(25, "$0 finish|finishes the seams and set|sets aside $p1.", "$0 find|finds the seams have failed and set|sets the material aside.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 465.0, Material: "barkcloth", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("465 grams of barkcloth", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Sewing Needle", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Sewing Needle tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Shears", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Shears tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_mesoamerican_painted_barkclothcape", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a $colour painted barkcloth shoulder cape (@renaissance_mesoamerican_painted_barkclothcape)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition TextileAndLeatherWorkTailorLongPanelledColourCottonLoinGarment() => new()
	{
		Metadata = new("Textile and leather work/tailor long panelled $colour cotton loin garment", "crafts", ["renaissance"], ["item:renaissance_mesoamerican_panelled_loincloth"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "tailor long panelled $colour cotton loin garment",
		Category = "Textile and leather work",
		Blurb = "tailor a long panelled $colour cotton loin garment",
		Action = "tailoring a textile or leather item",
		ActiveCraftItemSdesc = "an in-progress long panelled $colour cotton loin garment craft",
		Gate = CatalogueRecipeGates.RenaissanceFinishedItemCraftingTailoring15,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 3,
		Interruptable = false,
		Phases = [
			new(25, "$0 lay|lays out $i1 and mark|marks the working lines with $t1.", "$0 lay|lays out $i1 and mark|marks the working lines with $t1.", ExertionLevel.Stasis, 0),
			new(45, "$0 cut|cuts and join|joins the shaped material with $t1 and $t2. $0 pause|pauses to check the proportions before carrying on.", "$0 cut|cuts and join|joins the shaped material with $t1 and $t2. $0 pause|pauses to check the proportions before carrying on.", ExertionLevel.Stasis, 0),
			new(25, "$0 finish|finishes the seams and set|sets aside $p1.", "$0 find|finds the seams have failed and set|sets the material aside.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 105.0, Material: "cotton", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("105 grams of cotton", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Sewing Needle", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Sewing Needle tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Shears", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Shears tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_mesoamerican_panelled_loincloth", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a long panelled $colour cotton loin garment (@renaissance_mesoamerican_panelled_loincloth)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition TextileAndLeatherWorkTailorRectangularSleevelessColourCottonBlouse() => new()
	{
		Metadata = new("Textile and leather work/tailor rectangular sleeveless $colour cotton blouse", "crafts", ["renaissance"], ["item:renaissance_mesoamerican_rectangular_blouse"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "tailor rectangular sleeveless $colour cotton blouse",
		Category = "Textile and leather work",
		Blurb = "tailor a rectangular sleeveless $colour cotton blouse",
		Action = "tailoring a textile or leather item",
		ActiveCraftItemSdesc = "an in-progress rectangular sleeveless $colour cotton blouse craft",
		Gate = CatalogueRecipeGates.RenaissanceFinishedItemCraftingTailoring15,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 3,
		Interruptable = false,
		Phases = [
			new(25, "$0 lay|lays out $i1 and mark|marks the working lines with $t1.", "$0 lay|lays out $i1 and mark|marks the working lines with $t1.", ExertionLevel.Stasis, 0),
			new(45, "$0 cut|cuts and join|joins the shaped material with $t1 and $t2. $0 turn|turns the work over to judge its balance.", "$0 cut|cuts and join|joins the shaped material with $t1 and $t2. $0 turn|turns the work over to judge its balance.", ExertionLevel.Stasis, 0),
			new(25, "$0 finish|finishes the seams and set|sets aside $p1.", "$0 find|finds the seams have failed and set|sets the material aside.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 285.0, Material: "cotton", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("285 grams of cotton", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Sewing Needle", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Sewing Needle tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Shears", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Shears tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_mesoamerican_rectangular_blouse", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a rectangular sleeveless $colour cotton blouse (@renaissance_mesoamerican_rectangular_blouse)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition GeneralCraftworkMakeColourShellAndBeadCeremonialApron() => new()
	{
		Metadata = new("General craftwork/make $colour shell-and-bead ceremonial apron", "crafts", ["renaissance"], ["item:renaissance_mesoamerican_shellbead_apron"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make $colour shell-and-bead ceremonial apron",
		Category = "General craftwork",
		Blurb = "make a $colour shell-and-bead ceremonial apron",
		Action = "making a finished item",
		ActiveCraftItemSdesc = "an in-progress $colour shell-and-bead ceremonial apron craft",
		Gate = CatalogueRecipeGates.RenaissanceFinishedItemCraftingLabouring15,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 3,
		Interruptable = false,
		Phases = [
			new(25, "$0 examine|examines $i1 and set|sets up the work with $t1.", "$0 examine|examines $i1 and set|sets up the work with $t1.", ExertionLevel.Stasis, 0),
			new(40, "$0 shape|shapes the prepared material, checking the form as it develops. $0 clear|clears the work area before the final steps.", "$0 shape|shapes the prepared material, checking the form as it develops. $0 clear|clears the work area before the final steps.", ExertionLevel.Stasis, 0),
			new(25, "$0 finish|finishes the work and set|sets aside $p1.", "$0 find|finds the work unsound and set|sets the material aside.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 510.0, Material: "beadwork", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("510 grams of beadwork", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_mesoamerican_shellbead_apron", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a $colour shell-and-bead ceremonial apron (@renaissance_mesoamerican_shellbead_apron)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};
}
