#nullable enable

using MudSharp.Body;
using MudSharp.GameItems.Inventory.Plans;
using MudSharp.RPG.Checks;

namespace DatabaseSeeder.Seeders;

public static partial class Recipes18Stock
{
	public static CatalogueRecipeDefinition MedicalAndRepairPrepareFineClockworkRepairKit() => new()
	{
		Metadata = new("Medical and repair/prepare fine clockwork repair kit", "crafts", ["renaissance"], ["item:renaissance_repair_fine_clockwork_repair_kit"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "prepare fine clockwork repair kit",
		Category = "Medical and repair",
		Blurb = "prepare a fine clockwork repair kit",
		Action = "preparing a medical or repair item",
		ActiveCraftItemSdesc = "an in-progress fine clockwork repair kit craft",
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
			new CatalogueCommodityRecipeInput(Grams: 665.0, Material: "leather", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("665 grams of leather", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Shears", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Shears tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Awl Punch", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Awl Punch tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_repair_fine_clockwork_repair_kit", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a fine clockwork repair kit (@renaissance_repair_fine_clockwork_repair_kit)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MedicalAndRepairPrepareFineFirearmRepairKit() => new()
	{
		Metadata = new("Medical and repair/prepare fine firearm repair kit", "crafts", ["renaissance"], ["item:renaissance_repair_fine_firearm_repair_kit"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "prepare fine firearm repair kit",
		Category = "Medical and repair",
		Blurb = "prepare a fine firearm repair kit",
		Action = "preparing a medical or repair item",
		ActiveCraftItemSdesc = "an in-progress fine firearm repair kit craft",
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
			new CatalogueCommodityRecipeInput(Grams: 665.0, Material: "leather", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("665 grams of leather", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Shears", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Shears tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Awl Punch", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Awl Punch tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_repair_fine_firearm_repair_kit", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a fine firearm repair kit (@renaissance_repair_fine_firearm_repair_kit)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MedicalAndRepairPrepareFineMedicalInstrumentRepairKit() => new()
	{
		Metadata = new("Medical and repair/prepare fine medical instrument repair kit", "crafts", ["renaissance"], ["item:renaissance_repair_fine_medical_instrument_repair_kit"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "prepare fine medical instrument repair kit",
		Category = "Medical and repair",
		Blurb = "prepare a fine medical instrument repair kit",
		Action = "preparing a medical or repair item",
		ActiveCraftItemSdesc = "an in-progress fine medical instrument repair kit craft",
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
			new CatalogueCommodityRecipeInput(Grams: 665.0, Material: "leather", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("665 grams of leather", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Shears", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Shears tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Awl Punch", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Awl Punch tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_repair_fine_medical_instrument_repair_kit", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a fine medical instrument repair kit (@renaissance_repair_fine_medical_instrument_repair_kit)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MedicalAndRepairPrepareFineOpticalInstrumentRepairKit() => new()
	{
		Metadata = new("Medical and repair/prepare fine optical instrument repair kit", "crafts", ["renaissance"], ["item:renaissance_repair_fine_optical_instrument_repair_kit"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "prepare fine optical instrument repair kit",
		Category = "Medical and repair",
		Blurb = "prepare a fine optical instrument repair kit",
		Action = "preparing a medical or repair item",
		ActiveCraftItemSdesc = "an in-progress fine optical instrument repair kit craft",
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
			new CatalogueCommodityRecipeInput(Grams: 665.0, Material: "leather", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("665 grams of leather", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Shears", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Shears tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Awl Punch", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Awl Punch tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_repair_fine_optical_instrument_repair_kit", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a fine optical instrument repair kit (@renaissance_repair_fine_optical_instrument_repair_kit)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MedicalAndRepairPrepareFinePrintingEquipmentRepairKit() => new()
	{
		Metadata = new("Medical and repair/prepare fine printing equipment repair kit", "crafts", ["renaissance"], ["item:renaissance_repair_fine_printing_equipment_repair_kit"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "prepare fine printing equipment repair kit",
		Category = "Medical and repair",
		Blurb = "prepare a fine printing equipment repair kit",
		Action = "preparing a medical or repair item",
		ActiveCraftItemSdesc = "an in-progress fine printing equipment repair kit craft",
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
			new CatalogueCommodityRecipeInput(Grams: 665.0, Material: "leather", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("665 grams of leather", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Shears", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Shears tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Awl Punch", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Awl Punch tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_repair_fine_printing_equipment_repair_kit", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a fine printing equipment repair kit (@renaissance_repair_fine_printing_equipment_repair_kit)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MedicalAndRepairPrepareFineScientificInstrumentRepairKit() => new()
	{
		Metadata = new("Medical and repair/prepare fine scientific instrument repair kit", "crafts", ["renaissance"], ["item:renaissance_repair_fine_scientific_instrument_repair_kit"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "prepare fine scientific instrument repair kit",
		Category = "Medical and repair",
		Blurb = "prepare a fine scientific instrument repair kit",
		Action = "preparing a medical or repair item",
		ActiveCraftItemSdesc = "an in-progress fine scientific instrument repair kit craft",
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
			new CatalogueCommodityRecipeInput(Grams: 665.0, Material: "leather", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("665 grams of leather", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Shears", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Shears tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Awl Punch", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Awl Punch tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_repair_fine_scientific_instrument_repair_kit", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a fine scientific instrument repair kit (@renaissance_repair_fine_scientific_instrument_repair_kit)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MedicalAndRepairPrepareFirearmRepairKit() => new()
	{
		Metadata = new("Medical and repair/prepare firearm repair kit", "crafts", ["renaissance"], ["item:renaissance_repair_firearm_repair_kit"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "prepare firearm repair kit",
		Category = "Medical and repair",
		Blurb = "prepare a firearm repair kit",
		Action = "preparing a medical or repair item",
		ActiveCraftItemSdesc = "an in-progress firearm repair kit craft",
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
			new CatalogueCommodityRecipeInput(Grams: 665.0, Material: "leather", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("665 grams of leather", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Shears", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Shears tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Awl Punch", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Awl Punch tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_repair_firearm_repair_kit", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a firearm repair kit (@renaissance_repair_firearm_repair_kit)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MedicalAndRepairPrepareMedicalInstrumentRepairKit() => new()
	{
		Metadata = new("Medical and repair/prepare medical instrument repair kit", "crafts", ["renaissance"], ["item:renaissance_repair_medical_instrument_repair_kit"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "prepare medical instrument repair kit",
		Category = "Medical and repair",
		Blurb = "prepare a medical instrument repair kit",
		Action = "preparing a medical or repair item",
		ActiveCraftItemSdesc = "an in-progress medical instrument repair kit craft",
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
			new CatalogueCommodityRecipeInput(Grams: 665.0, Material: "leather", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("665 grams of leather", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Shears", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Shears tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Awl Punch", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Awl Punch tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_repair_medical_instrument_repair_kit", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a medical instrument repair kit (@renaissance_repair_medical_instrument_repair_kit)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MedicalAndRepairPrepareOpticalInstrumentRepairKit() => new()
	{
		Metadata = new("Medical and repair/prepare optical instrument repair kit", "crafts", ["renaissance"], ["item:renaissance_repair_optical_instrument_repair_kit"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "prepare optical instrument repair kit",
		Category = "Medical and repair",
		Blurb = "prepare an optical instrument repair kit",
		Action = "preparing a medical or repair item",
		ActiveCraftItemSdesc = "an in-progress optical instrument repair kit craft",
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
			new CatalogueCommodityRecipeInput(Grams: 665.0, Material: "leather", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("665 grams of leather", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Shears", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Shears tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Awl Punch", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Awl Punch tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_repair_optical_instrument_repair_kit", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x an optical instrument repair kit (@renaissance_repair_optical_instrument_repair_kit)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MedicalAndRepairPreparePrintingEquipmentRepairKit() => new()
	{
		Metadata = new("Medical and repair/prepare printing equipment repair kit", "crafts", ["renaissance"], ["item:renaissance_repair_printing_equipment_repair_kit"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "prepare printing equipment repair kit",
		Category = "Medical and repair",
		Blurb = "prepare a printing equipment repair kit",
		Action = "preparing a medical or repair item",
		ActiveCraftItemSdesc = "an in-progress printing equipment repair kit craft",
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
			new CatalogueCommodityRecipeInput(Grams: 665.0, Material: "leather", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("665 grams of leather", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Shears", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Shears tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Awl Punch", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Awl Punch tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_repair_printing_equipment_repair_kit", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a printing equipment repair kit (@renaissance_repair_printing_equipment_repair_kit)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MedicalAndRepairPrepareRoughClockworkRepairKit() => new()
	{
		Metadata = new("Medical and repair/prepare rough clockwork repair kit", "crafts", ["renaissance"], ["item:renaissance_repair_rough_clockwork_repair_kit"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "prepare rough clockwork repair kit",
		Category = "Medical and repair",
		Blurb = "prepare a rough clockwork repair kit",
		Action = "preparing a medical or repair item",
		ActiveCraftItemSdesc = "an in-progress rough clockwork repair kit craft",
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
			new CatalogueCommodityRecipeInput(Grams: 665.0, Material: "leather", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("665 grams of leather", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Shears", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Shears tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Awl Punch", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Awl Punch tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_repair_rough_clockwork_repair_kit", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a rough clockwork repair kit (@renaissance_repair_rough_clockwork_repair_kit)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MedicalAndRepairPrepareRoughFirearmRepairKit() => new()
	{
		Metadata = new("Medical and repair/prepare rough firearm repair kit", "crafts", ["renaissance"], ["item:renaissance_repair_rough_firearm_repair_kit"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "prepare rough firearm repair kit",
		Category = "Medical and repair",
		Blurb = "prepare a rough firearm repair kit",
		Action = "preparing a medical or repair item",
		ActiveCraftItemSdesc = "an in-progress rough firearm repair kit craft",
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
			new CatalogueCommodityRecipeInput(Grams: 665.0, Material: "leather", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("665 grams of leather", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Shears", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Shears tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Awl Punch", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Awl Punch tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_repair_rough_firearm_repair_kit", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a rough firearm repair kit (@renaissance_repair_rough_firearm_repair_kit)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MedicalAndRepairPrepareRoughMedicalInstrumentRepairKit() => new()
	{
		Metadata = new("Medical and repair/prepare rough medical instrument repair kit", "crafts", ["renaissance"], ["item:renaissance_repair_rough_medical_instrument_repair_kit"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "prepare rough medical instrument repair kit",
		Category = "Medical and repair",
		Blurb = "prepare a rough medical instrument repair kit",
		Action = "preparing a medical or repair item",
		ActiveCraftItemSdesc = "an in-progress rough medical instrument repair kit craft",
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
			new CatalogueCommodityRecipeInput(Grams: 665.0, Material: "leather", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("665 grams of leather", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Shears", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Shears tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Awl Punch", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Awl Punch tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_repair_rough_medical_instrument_repair_kit", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a rough medical instrument repair kit (@renaissance_repair_rough_medical_instrument_repair_kit)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MedicalAndRepairPrepareRoughOpticalInstrumentRepairKit() => new()
	{
		Metadata = new("Medical and repair/prepare rough optical instrument repair kit", "crafts", ["renaissance"], ["item:renaissance_repair_rough_optical_instrument_repair_kit"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "prepare rough optical instrument repair kit",
		Category = "Medical and repair",
		Blurb = "prepare a rough optical instrument repair kit",
		Action = "preparing a medical or repair item",
		ActiveCraftItemSdesc = "an in-progress rough optical instrument repair kit craft",
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
			new CatalogueCommodityRecipeInput(Grams: 665.0, Material: "leather", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("665 grams of leather", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Shears", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Shears tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Awl Punch", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Awl Punch tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_repair_rough_optical_instrument_repair_kit", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a rough optical instrument repair kit (@renaissance_repair_rough_optical_instrument_repair_kit)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MedicalAndRepairPrepareRoughPrintingEquipmentRepairKit() => new()
	{
		Metadata = new("Medical and repair/prepare rough printing equipment repair kit", "crafts", ["renaissance"], ["item:renaissance_repair_rough_printing_equipment_repair_kit"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "prepare rough printing equipment repair kit",
		Category = "Medical and repair",
		Blurb = "prepare a rough printing equipment repair kit",
		Action = "preparing a medical or repair item",
		ActiveCraftItemSdesc = "an in-progress rough printing equipment repair kit craft",
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
			new CatalogueCommodityRecipeInput(Grams: 665.0, Material: "leather", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("665 grams of leather", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Shears", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Shears tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Awl Punch", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Awl Punch tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_repair_rough_printing_equipment_repair_kit", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a rough printing equipment repair kit (@renaissance_repair_rough_printing_equipment_repair_kit)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MedicalAndRepairPrepareRoughScientificInstrumentRepairKit() => new()
	{
		Metadata = new("Medical and repair/prepare rough scientific instrument repair kit", "crafts", ["renaissance"], ["item:renaissance_repair_rough_scientific_instrument_repair_kit"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "prepare rough scientific instrument repair kit",
		Category = "Medical and repair",
		Blurb = "prepare a rough scientific instrument repair kit",
		Action = "preparing a medical or repair item",
		ActiveCraftItemSdesc = "an in-progress rough scientific instrument repair kit craft",
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
			new CatalogueCommodityRecipeInput(Grams: 665.0, Material: "leather", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("665 grams of leather", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Shears", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Shears tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Awl Punch", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Awl Punch tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_repair_rough_scientific_instrument_repair_kit", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a rough scientific instrument repair kit (@renaissance_repair_rough_scientific_instrument_repair_kit)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MedicalAndRepairPrepareScientificInstrumentRepairKit() => new()
	{
		Metadata = new("Medical and repair/prepare scientific instrument repair kit", "crafts", ["renaissance"], ["item:renaissance_repair_scientific_instrument_repair_kit"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "prepare scientific instrument repair kit",
		Category = "Medical and repair",
		Blurb = "prepare a scientific instrument repair kit",
		Action = "preparing a medical or repair item",
		ActiveCraftItemSdesc = "an in-progress scientific instrument repair kit craft",
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
			new CatalogueCommodityRecipeInput(Grams: 665.0, Material: "leather", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("665 grams of leather", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Shears", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Shears tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Awl Punch", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Awl Punch tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_repair_scientific_instrument_repair_kit", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a scientific instrument repair kit (@renaissance_repair_scientific_instrument_repair_kit)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition TextileAndLeatherWorkTailorColourStructuredIslandCourtCap() => new()
	{
		Metadata = new("Textile and leather work/tailor $colour structured island court cap", "crafts", ["renaissance"], ["item:renaissance_ryukyu_formal_courtcap"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "tailor $colour structured island court cap",
		Category = "Textile and leather work",
		Blurb = "tailor a $colour structured island court cap",
		Action = "tailoring a textile or leather item",
		ActiveCraftItemSdesc = "an in-progress $colour structured island court cap craft",
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
			new(45, "$0 cut|cuts and join|joins the shaped material with $t1 and $t2. $0 measure|measures the developing form against the intended finish.", "$0 cut|cuts and join|joins the shaped material with $t1 and $t2. $0 measure|measures the developing form against the intended finish.", ExertionLevel.Stasis, 0),
			new(25, "$0 finish|finishes the seams and set|sets aside $p1.", "$0 find|finds the seams have failed and set|sets the material aside.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 143.0, Material: "silk", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("143 grams of silk", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Sewing Needle", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Sewing Needle tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Shears", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Shears tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_ryukyu_formal_courtcap", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a $colour structured island court cap (@renaissance_ryukyu_formal_courtcap)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition TextileAndLeatherWorkTailorPleatedColourCottonTropicalWrapSkirt() => new()
	{
		Metadata = new("Textile and leather work/tailor pleated $colour cotton tropical wrap skirt", "crafts", ["renaissance"], ["item:renaissance_ryukyu_pleated_wrapskirt"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "tailor pleated $colour cotton tropical wrap skirt",
		Category = "Textile and leather work",
		Blurb = "tailor a pleated $colour cotton tropical wrap skirt",
		Action = "tailoring a textile or leather item",
		ActiveCraftItemSdesc = "an in-progress pleated $colour cotton tropical wrap skirt craft",
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
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_ryukyu_pleated_wrapskirt", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a pleated $colour cotton tropical wrap skirt (@renaissance_ryukyu_pleated_wrapskirt)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition TextileAndLeatherWorkTailorShortColourCottonTropicalWrapJacket() => new()
	{
		Metadata = new("Textile and leather work/tailor short $colour cotton tropical wrap jacket", "crafts", ["renaissance"], ["item:renaissance_ryukyu_short_wrapjacket"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "tailor short $colour cotton tropical wrap jacket",
		Category = "Textile and leather work",
		Blurb = "tailor a short $colour cotton tropical wrap jacket",
		Action = "tailoring a textile or leather item",
		ActiveCraftItemSdesc = "an in-progress short $colour cotton tropical wrap jacket craft",
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
			new CatalogueCommodityRecipeInput(Grams: 435.0, Material: "cotton", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("435 grams of cotton", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Sewing Needle", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Sewing Needle tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Shears", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Shears tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_ryukyu_short_wrapjacket", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a short $colour cotton tropical wrap jacket (@renaissance_ryukyu_short_wrapjacket)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition TextileAndLeatherWorkTailorBroadSleevedColourRamieTropicalRobe() => new()
	{
		Metadata = new("Textile and leather work/tailor broad-sleeved $colour ramie tropical robe", "crafts", ["renaissance"], ["item:renaissance_ryukyu_widesleeve_summerrobe"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "tailor broad-sleeved $colour ramie tropical robe",
		Category = "Textile and leather work",
		Blurb = "tailor a broad-sleeved $colour ramie tropical robe",
		Action = "tailoring a textile or leather item",
		ActiveCraftItemSdesc = "an in-progress broad-sleeved $colour ramie tropical robe craft",
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
			new(45, "$0 cut|cuts and join|joins the shaped material with $t1 and $t2. $0 measure|measures the developing form against the intended finish.", "$0 cut|cuts and join|joins the shaped material with $t1 and $t2. $0 measure|measures the developing form against the intended finish.", ExertionLevel.Stasis, 0),
			new(25, "$0 finish|finishes the seams and set|sets aside $p1.", "$0 find|finds the seams have failed and set|sets the material aside.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 540.0, Material: "ramie cloth", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("540 grams of ramie cloth", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Sewing Needle", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Sewing Needle tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Shears", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Shears tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_ryukyu_widesleeve_summerrobe", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a broad-sleeved $colour ramie tropical robe (@renaissance_ryukyu_widesleeve_summerrobe)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition TextileAndLeatherWorkTailorBroadRectangularColourCottonOverRobe() => new()
	{
		Metadata = new("Textile and leather work/tailor broad rectangular $colour cotton over-robe", "crafts", ["renaissance"], ["item:renaissance_sahel_broad_rectangular_robe"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "tailor broad rectangular $colour cotton over-robe",
		Category = "Textile and leather work",
		Blurb = "tailor a broad rectangular $colour cotton over-robe",
		Action = "tailoring a textile or leather item",
		ActiveCraftItemSdesc = "an in-progress broad rectangular $colour cotton over-robe craft",
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
			new CatalogueCommodityRecipeInput(Grams: 608.0, Material: "cotton", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("608 grams of cotton", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Sewing Needle", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Sewing Needle tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Shears", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Shears tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_sahel_broad_rectangular_robe", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a broad rectangular $colour cotton over-robe (@renaissance_sahel_broad_rectangular_robe)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition TextileAndLeatherWorkTailorConicalLeatherRidingHat() => new()
	{
		Metadata = new("Textile and leather work/tailor conical leather riding hat", "crafts", ["renaissance"], ["item:renaissance_sahel_conical_leatherhat"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "tailor conical leather riding hat",
		Category = "Textile and leather work",
		Blurb = "tailor a conical leather riding hat",
		Action = "tailoring a textile or leather item",
		ActiveCraftItemSdesc = "an in-progress conical leather riding hat craft",
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
			new(40, "$0 cut|cuts the leather to shape with $t2 and work|works its edges true. $0 turn|turns the work over to judge its balance.", "$0 cut|cuts the leather to shape with $t2 and work|works its edges true. $0 turn|turns the work over to judge its balance.", ExertionLevel.Stasis, 0),
			new(25, "$0 burnish|burnishes the finished leatherwork and set|sets aside $p1.", "$0 split|splits the leatherwork and set|sets the material aside.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 158.0, Material: "leather", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("158 grams of leather", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Awl Punch", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Awl Punch tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Shears", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Shears tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_sahel_conical_leatherhat", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a conical leather riding hat (@renaissance_sahel_conical_leatherhat)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition TextileAndLeatherWorkTailorBroadNeckedColourCottonTunic() => new()
	{
		Metadata = new("Textile and leather work/tailor broad-necked $colour cotton tunic", "crafts", ["renaissance"], ["item:renaissance_sahel_embroidered_necktunic"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "tailor broad-necked $colour cotton tunic",
		Category = "Textile and leather work",
		Blurb = "tailor a broad-necked $colour cotton tunic",
		Action = "tailoring a textile or leather item",
		ActiveCraftItemSdesc = "an in-progress broad-necked $colour cotton tunic craft",
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
			new CatalogueCommodityRecipeInput(Grams: 405.0, Material: "cotton", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("405 grams of cotton", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Sewing Needle", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Sewing Needle tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Shears", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Shears tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_sahel_embroidered_necktunic", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a broad-necked $colour cotton tunic (@renaissance_sahel_embroidered_necktunic)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition TextileAndLeatherWorkTailorColourLongIndigoFaceVeil() => new()
	{
		Metadata = new("Textile and leather work/tailor $colour long indigo face veil", "crafts", ["renaissance"], ["item:renaissance_sahel_indigo_faceveil"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "tailor $colour long indigo face veil",
		Category = "Textile and leather work",
		Blurb = "tailor a $colour long indigo face veil",
		Action = "tailoring a textile or leather item",
		ActiveCraftItemSdesc = "an in-progress $colour long indigo face veil craft",
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
			new CatalogueCommodityRecipeInput(Grams: 135.0, Material: "cotton", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("135 grams of cotton", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Sewing Needle", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Sewing Needle tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Shears", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Shears tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_sahel_indigo_faceveil", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a $colour long indigo face veil (@renaissance_sahel_indigo_faceveil)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition TextileAndLeatherWorkTailorColourLongLeatherRidingCoat() => new()
	{
		Metadata = new("Textile and leather work/tailor $colour long leather riding coat", "crafts", ["renaissance"], ["item:renaissance_sahel_leather_ridingcoat"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "tailor $colour long leather riding coat",
		Category = "Textile and leather work",
		Blurb = "tailor a $colour long leather riding coat",
		Action = "tailoring a textile or leather item",
		ActiveCraftItemSdesc = "an in-progress $colour long leather riding coat craft",
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
			new(40, "$0 cut|cuts the leather to shape with $t2 and work|works its edges true. $0 turn|turns the work over to judge its balance.", "$0 cut|cuts the leather to shape with $t2 and work|works its edges true. $0 turn|turns the work over to judge its balance.", ExertionLevel.Stasis, 0),
			new(25, "$0 burnish|burnishes the finished leatherwork and set|sets aside $p1.", "$0 split|splits the leatherwork and set|sets the material aside.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 915.0, Material: "leather", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("915 grams of leather", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Awl Punch", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Awl Punch tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Shears", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Shears tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_sahel_leather_ridingcoat", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a $colour long leather riding coat (@renaissance_sahel_leather_ridingcoat)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition TextileAndLeatherWorkTailorNarrowLongColourCottonTunicPattern2() => new()
	{
		Metadata = new("Textile and leather work/tailor narrow long $colour cotton tunic pattern 2", "crafts", ["renaissance"], ["item:renaissance_sahel_narrow_longtunic"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "tailor narrow long $colour cotton tunic pattern 2",
		Category = "Textile and leather work",
		Blurb = "tailor a narrow long $colour cotton tunic",
		Action = "tailoring a textile or leather item",
		ActiveCraftItemSdesc = "an in-progress narrow long $colour cotton tunic craft",
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
			new(45, "$0 cut|cuts and join|joins the shaped material with $t1 and $t2. $0 measure|measures the developing form against the intended finish.", "$0 cut|cuts and join|joins the shaped material with $t1 and $t2. $0 measure|measures the developing form against the intended finish.", ExertionLevel.Stasis, 0),
			new(25, "$0 finish|finishes the seams and set|sets aside $p1.", "$0 find|finds the seams have failed and set|sets the material aside.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 405.0, Material: "cotton", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("405 grams of cotton", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Sewing Needle", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Sewing Needle tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Shears", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Shears tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_sahel_narrow_longtunic", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a narrow long $colour cotton tunic (@renaissance_sahel_narrow_longtunic)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition TextileAndLeatherWorkTailorColourQuiltedCottonRidingCap() => new()
	{
		Metadata = new("Textile and leather work/tailor $colour quilted cotton riding cap", "crafts", ["renaissance"], ["item:renaissance_sahel_quilted_ridingcap"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "tailor $colour quilted cotton riding cap",
		Category = "Textile and leather work",
		Blurb = "tailor a $colour quilted cotton riding cap",
		Action = "tailoring a textile or leather item",
		ActiveCraftItemSdesc = "an in-progress $colour quilted cotton riding cap craft",
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
			new(45, "$0 cut|cuts and join|joins the shaped material with $t1 and $t2. $0 clear|clears the work area before the final steps.", "$0 cut|cuts and join|joins the shaped material with $t1 and $t2. $0 clear|clears the work area before the final steps.", ExertionLevel.Stasis, 0),
			new(25, "$0 finish|finishes the seams and set|sets aside $p1.", "$0 find|finds the seams have failed and set|sets the material aside.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 143.0, Material: "cotton", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("143 grams of cotton", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Sewing Needle", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Sewing Needle tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Shears", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Shears tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_sahel_quilted_ridingcap", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a $colour quilted cotton riding cap (@renaissance_sahel_quilted_ridingcap)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition TextileAndLeatherWorkTailorLayeredColourCottonTurban() => new()
	{
		Metadata = new("Textile and leather work/tailor layered $colour cotton turban", "crafts", ["renaissance"], ["item:renaissance_sahel_scholar_turban"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "tailor layered $colour cotton turban",
		Category = "Textile and leather work",
		Blurb = "tailor a layered $colour cotton turban",
		Action = "tailoring a textile or leather item",
		ActiveCraftItemSdesc = "an in-progress layered $colour cotton turban craft",
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
			new(45, "$0 cut|cuts and join|joins the shaped material with $t1 and $t2. $0 measure|measures the developing form against the intended finish.", "$0 cut|cuts and join|joins the shaped material with $t1 and $t2. $0 measure|measures the developing form against the intended finish.", ExertionLevel.Stasis, 0),
			new(25, "$0 finish|finishes the seams and set|sets aside $p1.", "$0 find|finds the seams have failed and set|sets the material aside.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 165.0, Material: "cotton", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("165 grams of cotton", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Sewing Needle", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Sewing Needle tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Shears", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Shears tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_sahel_scholar_turban", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a layered $colour cotton turban (@renaissance_sahel_scholar_turban)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition TextileAndLeatherWorkTailorColourSoftLeatherRidingBoots() => new()
	{
		Metadata = new("Textile and leather work/tailor $colour soft leather riding boots", "crafts", ["renaissance"], ["item:renaissance_sahel_soft_ridingboots"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "tailor $colour soft leather riding boots",
		Category = "Textile and leather work",
		Blurb = "tailor a pair of $colour soft leather riding boots",
		Action = "tailoring a textile or leather item",
		ActiveCraftItemSdesc = "an in-progress $colour soft leather riding boots craft",
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
			new(40, "$0 cut|cuts the leather to shape with $t2 and work|works its edges true. $0 measure|measures the developing form against the intended finish.", "$0 cut|cuts the leather to shape with $t2 and work|works its edges true. $0 measure|measures the developing form against the intended finish.", ExertionLevel.Stasis, 0),
			new(25, "$0 burnish|burnishes the finished leatherwork and set|sets aside $p1.", "$0 split|splits the leatherwork and set|sets the material aside.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 788.0, Material: "leather", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("788 grams of leather", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Awl Punch", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Awl Punch tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Shears", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Shears tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_sahel_soft_ridingboots", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a pair of $colour soft leather riding boots (@renaissance_sahel_soft_ridingboots)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition TextileAndLeatherWorkTailorVeryFullColourCottonTrousersPattern2() => new()
	{
		Metadata = new("Textile and leather work/tailor very full $colour cotton trousers pattern 2", "crafts", ["renaissance"], ["item:renaissance_sahel_veryfull_trousers"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "tailor very full $colour cotton trousers pattern 2",
		Category = "Textile and leather work",
		Blurb = "tailor a pair of very full $colour cotton trousers",
		Action = "tailoring a textile or leather item",
		ActiveCraftItemSdesc = "an in-progress very full $colour cotton trousers craft",
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
			new(45, "$0 cut|cuts and join|joins the shaped material with $t1 and $t2. $0 measure|measures the developing form against the intended finish.", "$0 cut|cuts and join|joins the shaped material with $t1 and $t2. $0 measure|measures the developing form against the intended finish.", ExertionLevel.Stasis, 0),
			new(25, "$0 finish|finishes the seams and set|sets aside $p1.", "$0 find|finds the seams have failed and set|sets the material aside.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 375.0, Material: "cotton", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("375 grams of cotton", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Sewing Needle", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Sewing Needle tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Shears", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Shears tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_sahel_veryfull_trousers", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a pair of very full $colour cotton trousers (@renaissance_sahel_veryfull_trousers)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition GeneralCraftworkMakeSmallBrassArmillarySphere() => new()
	{
		Metadata = new("General craftwork/make small brass armillary sphere", "crafts", ["renaissance"], ["item:renaissance_science_armillary_sphere"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make small brass armillary sphere",
		Category = "General craftwork",
		Blurb = "make a small brass armillary sphere",
		Action = "making a finished item",
		ActiveCraftItemSdesc = "an in-progress small brass armillary sphere craft",
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
			new CatalogueCommodityRecipeInput(Grams: 1950.0, Material: "brass", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1950 grams of brass", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_science_armillary_sphere", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a small brass armillary sphere (@renaissance_science_armillary_sphere)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition GeneralCraftworkMakeGraduatedBrassQuadrant() => new()
	{
		Metadata = new("General craftwork/make graduated brass quadrant", "crafts", ["renaissance"], ["item:renaissance_science_brass_quadrant"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make graduated brass quadrant",
		Category = "General craftwork",
		Blurb = "make a graduated brass quadrant",
		Action = "making a finished item",
		ActiveCraftItemSdesc = "an in-progress graduated brass quadrant craft",
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
			new CatalogueCommodityRecipeInput(Grams: 540.0, Material: "brass", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("540 grams of brass", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_science_brass_quadrant", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a graduated brass quadrant (@renaissance_science_brass_quadrant)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};
}
