#nullable enable

using MudSharp.Body;
using MudSharp.GameItems.Inventory.Plans;
using MudSharp.RPG.Checks;

namespace DatabaseSeeder.Seeders;

public static partial class Recipes18Stock
{
	public static CatalogueRecipeDefinition GeneralCraftworkMakeColourSleevelessCanvasDeckSmock() => new()
	{
		Metadata = new("General craftwork/make $colour sleeveless canvas deck smock", "crafts", ["renaissance"], ["item:renaissance_maritime_sleeveless_decksmock"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make $colour sleeveless canvas deck smock",
		Category = "General craftwork",
		Blurb = "make a $colour sleeveless canvas deck smock",
		Action = "making a finished item",
		ActiveCraftItemSdesc = "an in-progress $colour sleeveless canvas deck smock craft",
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
			new CatalogueCommodityRecipeInput(Grams: 360.0, Material: "canvas", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("360 grams of canvas", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_maritime_sleeveless_decksmock", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a $colour sleeveless canvas deck smock (@renaissance_maritime_sleeveless_decksmock)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition GeneralCraftworkMakeColourTarredCanvasBrimmedSeamanHat() => new()
	{
		Metadata = new("General craftwork/make $colour tarred canvas brimmed seaman hat", "crafts", ["renaissance"], ["item:renaissance_maritime_tarred_brimhat"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make $colour tarred canvas brimmed seaman hat",
		Category = "General craftwork",
		Blurb = "make a $colour tarred canvas brimmed seaman hat",
		Action = "making a finished item",
		ActiveCraftItemSdesc = "an in-progress $colour tarred canvas brimmed seaman hat craft",
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
			new CatalogueCommodityRecipeInput(Grams: 143.0, Material: "canvas", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("143 grams of canvas", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_maritime_tarred_brimhat", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a $colour tarred canvas brimmed seaman hat (@renaissance_maritime_tarred_brimhat)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition TextileAndLeatherWorkTailorColourTarredHighDeckShoes() => new()
	{
		Metadata = new("Textile and leather work/tailor $colour tarred high deck shoes", "crafts", ["renaissance"], ["item:renaissance_maritime_tarred_highshoes"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "tailor $colour tarred high deck shoes",
		Category = "Textile and leather work",
		Blurb = "tailor a pair of $colour tarred high deck shoes",
		Action = "tailoring a textile or leather item",
		ActiveCraftItemSdesc = "an in-progress $colour tarred high deck shoes craft",
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
			new CatalogueCommodityRecipeInput(Grams: 518.0, Material: "leather", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("518 grams of leather", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Awl Punch", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Awl Punch tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Shears", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Shears tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_maritime_tarred_highshoes", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a pair of $colour tarred high deck shoes (@renaissance_maritime_tarred_highshoes)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition GeneralCraftworkMakeColourShortTreatedCanvasDeckCape() => new()
	{
		Metadata = new("General craftwork/make $colour short treated-canvas deck cape", "crafts", ["renaissance"], ["item:renaissance_maritime_treated_canvas_cape"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make $colour short treated-canvas deck cape",
		Category = "General craftwork",
		Blurb = "make a $colour short treated-canvas deck cape",
		Action = "making a finished item",
		ActiveCraftItemSdesc = "an in-progress $colour short treated-canvas deck cape craft",
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
			new CatalogueCommodityRecipeInput(Grams: 788.0, Material: "canvas", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("788 grams of canvas", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_maritime_treated_canvas_cape", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a $colour short treated-canvas deck cape (@renaissance_maritime_treated_canvas_cape)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MedicalAndRepairPrepareAdjustableCrutch() => new()
	{
		Metadata = new("Medical and repair/prepare adjustable crutch", "crafts", ["renaissance"], ["item:renaissance_medical_adjustable_crutch"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "prepare adjustable crutch",
		Category = "Medical and repair",
		Blurb = "prepare an adjustable crutch",
		Action = "preparing a medical or repair item",
		ActiveCraftItemSdesc = "an in-progress adjustable crutch craft",
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
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_medical_adjustable_crutch", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x an adjustable crutch (@renaissance_medical_adjustable_crutch)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MedicalAndRepairPrepareAlbarelloDrugJar() => new()
	{
		Metadata = new("Medical and repair/prepare albarello drug jar", "crafts", ["renaissance"], ["item:renaissance_medical_albarello_drug_jar"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "prepare albarello drug jar",
		Category = "Medical and repair",
		Blurb = "prepare an albarello drug jar",
		Action = "preparing a medical or repair item",
		ActiveCraftItemSdesc = "an in-progress albarello drug jar craft",
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
			new CatalogueCommodityRecipeInput(Grams: 112.0, Material: "glass", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("112 grams of glass", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Shears", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Shears tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Awl Punch", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Awl Punch tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_medical_albarello_drug_jar", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x an albarello drug jar (@renaissance_medical_albarello_drug_jar)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MedicalAndRepairPrepareAlumStock() => new()
	{
		Metadata = new("Medical and repair/prepare alum stock", "crafts", ["renaissance"], ["item:renaissance_medical_alum_stock"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "prepare alum stock",
		Category = "Medical and repair",
		Blurb = "prepare an alum stock",
		Action = "preparing a medical or repair item",
		ActiveCraftItemSdesc = "an in-progress alum stock craft",
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
			new CatalogueCommodityRecipeInput(Grams: 63.0, Material: "alum", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("63 grams of alum", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Shears", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Shears tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Awl Punch", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Awl Punch tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_medical_alum_stock", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x an alum stock (@renaissance_medical_alum_stock)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MedicalAndRepairPrepareAmputationKnife() => new()
	{
		Metadata = new("Medical and repair/prepare amputation knife", "crafts", ["renaissance"], ["item:renaissance_medical_amputation_knife"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "prepare amputation knife",
		Category = "Medical and repair",
		Blurb = "prepare an amputation knife",
		Action = "preparing a medical or repair item",
		ActiveCraftItemSdesc = "an in-progress amputation knife craft",
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
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_medical_amputation_knife", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x an amputation knife (@renaissance_medical_amputation_knife)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MedicalAndRepairPrepareAnimalBandage() => new()
	{
		Metadata = new("Medical and repair/prepare animal bandage", "crafts", ["renaissance"], ["item:renaissance_medical_animal_bandage"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "prepare animal bandage",
		Category = "Medical and repair",
		Blurb = "prepare an animal bandage",
		Action = "preparing a medical or repair item",
		ActiveCraftItemSdesc = "an in-progress animal bandage craft",
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
			new CatalogueCommodityRecipeInput(Grams: 294.0, Material: "linen", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("294 grams of linen", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Shears", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Shears tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Awl Punch", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Awl Punch tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_medical_animal_bandage", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x an animal bandage (@renaissance_medical_animal_bandage)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MedicalAndRepairPrepareAnimalDosingHorn() => new()
	{
		Metadata = new("Medical and repair/prepare animal dosing horn", "crafts", ["renaissance"], ["item:renaissance_medical_animal_dosing_horn"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "prepare animal dosing horn",
		Category = "Medical and repair",
		Blurb = "prepare an animal dosing horn",
		Action = "preparing a medical or repair item",
		ActiveCraftItemSdesc = "an in-progress animal dosing horn craft",
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
			new CatalogueCommodityRecipeInput(Grams: 126.0, Material: "horn", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("126 grams of horn", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Shears", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Shears tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Awl Punch", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Awl Punch tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_medical_animal_dosing_horn", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x an animal dosing horn (@renaissance_medical_animal_dosing_horn)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MedicalAndRepairPrepareAnimalSplint() => new()
	{
		Metadata = new("Medical and repair/prepare animal splint", "crafts", ["renaissance"], ["item:renaissance_medical_animal_splint"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "prepare animal splint",
		Category = "Medical and repair",
		Blurb = "prepare an animal splint",
		Action = "preparing a medical or repair item",
		ActiveCraftItemSdesc = "an in-progress animal splint craft",
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
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_medical_animal_splint", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x an animal splint (@renaissance_medical_animal_splint)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MedicalAndRepairPrepareApothecaryMortar() => new()
	{
		Metadata = new("Medical and repair/prepare apothecary mortar", "crafts", ["renaissance"], ["item:renaissance_medical_apothecary_mortar"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "prepare apothecary mortar",
		Category = "Medical and repair",
		Blurb = "prepare an apothecary mortar",
		Action = "preparing a medical or repair item",
		ActiveCraftItemSdesc = "an in-progress apothecary mortar craft",
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
			new CatalogueCommodityRecipeInput(Grams: 875.0, Material: "granite", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("875 grams of granite", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Shears", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Shears tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Awl Punch", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Awl Punch tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_medical_apothecary_mortar", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x an apothecary mortar (@renaissance_medical_apothecary_mortar)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MedicalAndRepairPrepareArmSling() => new()
	{
		Metadata = new("Medical and repair/prepare arm sling", "crafts", ["renaissance"], ["item:renaissance_medical_arm_sling"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "prepare arm sling",
		Category = "Medical and repair",
		Blurb = "prepare an arm sling",
		Action = "preparing a medical or repair item",
		ActiveCraftItemSdesc = "an in-progress arm sling craft",
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
			new CatalogueCommodityRecipeInput(Grams: 294.0, Material: "linen", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("294 grams of linen", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Shears", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Shears tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Awl Punch", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Awl Punch tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_medical_arm_sling", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x an arm sling (@renaissance_medical_arm_sling)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MedicalAndRepairPrepareAromaticVinegarCloth() => new()
	{
		Metadata = new("Medical and repair/prepare aromatic vinegar cloth", "crafts", ["renaissance"], ["item:renaissance_medical_aromatic_vinegar_cloth"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "prepare aromatic vinegar cloth",
		Category = "Medical and repair",
		Blurb = "prepare an aromatic vinegar cloth",
		Action = "preparing a medical or repair item",
		ActiveCraftItemSdesc = "an in-progress aromatic vinegar cloth craft",
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
			new CatalogueCommodityRecipeInput(Grams: 294.0, Material: "linen", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("294 grams of linen", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Shears", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Shears tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Awl Punch", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Awl Punch tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_medical_aromatic_vinegar_cloth", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x an aromatic vinegar cloth (@renaissance_medical_aromatic_vinegar_cloth)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MedicalAndRepairPrepareArteryForceps() => new()
	{
		Metadata = new("Medical and repair/prepare artery forceps", "crafts", ["renaissance"], ["item:renaissance_medical_artery_forceps"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "prepare artery forceps",
		Category = "Medical and repair",
		Blurb = "prepare an artery forceps",
		Action = "preparing a medical or repair item",
		ActiveCraftItemSdesc = "an in-progress artery forceps craft",
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
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_medical_artery_forceps", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x an artery forceps (@renaissance_medical_artery_forceps)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MedicalAndRepairPrepareAstrologicalDiagnosticGlass() => new()
	{
		Metadata = new("Medical and repair/prepare astrological diagnostic glass", "crafts", ["renaissance"], ["item:renaissance_medical_astrological_diagnostic_glass"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "prepare astrological diagnostic glass",
		Category = "Medical and repair",
		Blurb = "prepare an astrological diagnostic glass",
		Action = "preparing a medical or repair item",
		ActiveCraftItemSdesc = "an in-progress astrological diagnostic glass craft",
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
			new CatalogueCommodityRecipeInput(Grams: 147.0, Material: "glass", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("147 grams of glass", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Shears", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Shears tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Awl Punch", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Awl Punch tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_medical_astrological_diagnostic_glass", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x an astrological diagnostic glass (@renaissance_medical_astrological_diagnostic_glass)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MedicalAndRepairPrepareBarberSurgeonCase() => new()
	{
		Metadata = new("Medical and repair/prepare barber surgeon case", "crafts", ["renaissance"], ["item:renaissance_medical_barber_surgeon_case"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "prepare barber surgeon case",
		Category = "Medical and repair",
		Blurb = "prepare a barber surgeon case",
		Action = "preparing a medical or repair item",
		ActiveCraftItemSdesc = "an in-progress barber surgeon case craft",
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
			new CatalogueCommodityRecipeInput(Grams: 1260.0, Material: "leather", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1260 grams of leather", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Shears", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Shears tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Awl Punch", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Awl Punch tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_medical_barber_surgeon_case", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a barber surgeon case (@renaissance_medical_barber_surgeon_case)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MedicalAndRepairPrepareBenzoinResinStock() => new()
	{
		Metadata = new("Medical and repair/prepare benzoin resin stock", "crafts", ["renaissance"], ["item:renaissance_medical_benzoin_resin_stock"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "prepare benzoin resin stock",
		Category = "Medical and repair",
		Blurb = "prepare a benzoin resin stock",
		Action = "preparing a medical or repair item",
		ActiveCraftItemSdesc = "an in-progress benzoin resin stock craft",
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
			new CatalogueCommodityRecipeInput(Grams: 63.0, Material: "benzoin resin", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("63 grams of benzoin resin", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Shears", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Shears tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Awl Punch", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Awl Punch tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_medical_benzoin_resin_stock", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a benzoin resin stock (@renaissance_medical_benzoin_resin_stock)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MedicalAndRepairPrepareBirthingStool() => new()
	{
		Metadata = new("Medical and repair/prepare birthing stool", "crafts", ["renaissance"], ["item:renaissance_medical_birthing_stool"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "prepare birthing stool",
		Category = "Medical and repair",
		Blurb = "prepare a birthing stool",
		Action = "preparing a medical or repair item",
		ActiveCraftItemSdesc = "an in-progress birthing stool craft",
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
			new CatalogueCommodityRecipeInput(Grams: 1260.0, Material: "oak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1260 grams of oak", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Shears", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Shears tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Awl Punch", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Awl Punch tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_medical_birthing_stool", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a birthing stool (@renaissance_medical_birthing_stool)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MedicalAndRepairPrepareBoneSaw() => new()
	{
		Metadata = new("Medical and repair/prepare bone saw", "crafts", ["renaissance"], ["item:renaissance_medical_bone_saw"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "prepare bone saw",
		Category = "Medical and repair",
		Blurb = "prepare a bone saw",
		Action = "preparing a medical or repair item",
		ActiveCraftItemSdesc = "an in-progress bone saw craft",
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
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_medical_bone_saw", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a bone saw (@renaissance_medical_bone_saw)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MedicalAndRepairPrepareBrassBalance() => new()
	{
		Metadata = new("Medical and repair/prepare brass balance", "crafts", ["renaissance"], ["item:renaissance_medical_brass_balance"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "prepare brass balance",
		Category = "Medical and repair",
		Blurb = "prepare a brass balance",
		Action = "preparing a medical or repair item",
		ActiveCraftItemSdesc = "an in-progress brass balance craft",
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
			new CatalogueCommodityRecipeInput(Grams: 294.0, Material: "brass", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("294 grams of brass", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Shears", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Shears tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Awl Punch", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Awl Punch tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_medical_brass_balance", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a brass balance (@renaissance_medical_brass_balance)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MedicalAndRepairPrepareBulletExtractor() => new()
	{
		Metadata = new("Medical and repair/prepare bullet extractor", "crafts", ["renaissance"], ["item:renaissance_medical_bullet_extractor"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "prepare bullet extractor",
		Category = "Medical and repair",
		Blurb = "prepare a bullet extractor",
		Action = "preparing a medical or repair item",
		ActiveCraftItemSdesc = "an in-progress bullet extractor craft",
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
			new CatalogueCommodityRecipeInput(Grams: 126.0, Material: "wrought iron", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("126 grams of wrought iron", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Shears", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Shears tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Awl Punch", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Awl Punch tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_medical_bullet_extractor", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a bullet extractor (@renaissance_medical_bullet_extractor)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MedicalAndRepairPrepareCampaignMedicineChest() => new()
	{
		Metadata = new("Medical and repair/prepare campaign medicine chest", "crafts", ["renaissance"], ["item:renaissance_medical_campaign_medicine_chest"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "prepare campaign medicine chest",
		Category = "Medical and repair",
		Blurb = "prepare a campaign medicine chest",
		Action = "preparing a medical or repair item",
		ActiveCraftItemSdesc = "an in-progress campaign medicine chest craft",
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
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_medical_campaign_medicine_chest", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a campaign medicine chest (@renaissance_medical_campaign_medicine_chest)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MedicalAndRepairPrepareCamphorStock() => new()
	{
		Metadata = new("Medical and repair/prepare camphor stock", "crafts", ["renaissance"], ["item:renaissance_medical_camphor_stock"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "prepare camphor stock",
		Category = "Medical and repair",
		Blurb = "prepare a camphor stock",
		Action = "preparing a medical or repair item",
		ActiveCraftItemSdesc = "an in-progress camphor stock craft",
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
			new CatalogueCommodityRecipeInput(Grams: 63.0, Material: "camphor", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("63 grams of camphor", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Shears", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Shears tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Awl Punch", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Awl Punch tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_medical_camphor_stock", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a camphor stock (@renaissance_medical_camphor_stock)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MedicalAndRepairPrepareCasualtyLitter() => new()
	{
		Metadata = new("Medical and repair/prepare casualty litter", "crafts", ["renaissance"], ["item:renaissance_medical_casualty_litter"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "prepare casualty litter",
		Category = "Medical and repair",
		Blurb = "prepare a casualty litter",
		Action = "preparing a medical or repair item",
		ActiveCraftItemSdesc = "an in-progress casualty litter craft",
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
			new CatalogueCommodityRecipeInput(Grams: 1260.0, Material: "oak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1260 grams of oak", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Shears", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Shears tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Awl Punch", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Awl Punch tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_medical_casualty_litter", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a casualty litter (@renaissance_medical_casualty_litter)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MedicalAndRepairPrepareCatheter() => new()
	{
		Metadata = new("Medical and repair/prepare catheter", "crafts", ["renaissance"], ["item:renaissance_medical_catheter"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "prepare catheter",
		Category = "Medical and repair",
		Blurb = "prepare a catheter",
		Action = "preparing a medical or repair item",
		ActiveCraftItemSdesc = "an in-progress catheter craft",
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
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_medical_catheter", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a catheter (@renaissance_medical_catheter)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MedicalAndRepairPrepareCattleLancet() => new()
	{
		Metadata = new("Medical and repair/prepare cattle lancet", "crafts", ["renaissance"], ["item:renaissance_medical_cattle_lancet"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "prepare cattle lancet",
		Category = "Medical and repair",
		Blurb = "prepare a cattle lancet",
		Action = "preparing a medical or repair item",
		ActiveCraftItemSdesc = "an in-progress cattle lancet craft",
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
			new CatalogueCommodityRecipeInput(Grams: 126.0, Material: "wrought iron", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("126 grams of wrought iron", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Shears", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Shears tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Awl Punch", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Awl Punch tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_medical_cattle_lancet", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a cattle lancet (@renaissance_medical_cattle_lancet)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MedicalAndRepairPrepareCauteryIron() => new()
	{
		Metadata = new("Medical and repair/prepare cautery iron", "crafts", ["renaissance"], ["item:renaissance_medical_cautery_iron"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "prepare cautery iron",
		Category = "Medical and repair",
		Blurb = "prepare a cautery iron",
		Action = "preparing a medical or repair item",
		ActiveCraftItemSdesc = "an in-progress cautery iron craft",
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
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_medical_cautery_iron", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a cautery iron (@renaissance_medical_cautery_iron)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MedicalAndRepairPrepareChamomileStock() => new()
	{
		Metadata = new("Medical and repair/prepare chamomile stock", "crafts", ["renaissance"], ["item:renaissance_medical_chamomile_stock"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "prepare chamomile stock",
		Category = "Medical and repair",
		Blurb = "prepare a chamomile stock",
		Action = "preparing a medical or repair item",
		ActiveCraftItemSdesc = "an in-progress chamomile stock craft",
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
			new CatalogueCommodityRecipeInput(Grams: 63.0, Material: "chamomile", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("63 grams of chamomile", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Shears", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Shears tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Awl Punch", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Awl Punch tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_medical_chamomile_stock", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a chamomile stock (@renaissance_medical_chamomile_stock)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MedicalAndRepairPrepareCinnamonStock() => new()
	{
		Metadata = new("Medical and repair/prepare cinnamon stock", "crafts", ["renaissance"], ["item:renaissance_medical_cinnamon_stock"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "prepare cinnamon stock",
		Category = "Medical and repair",
		Blurb = "prepare a cinnamon stock",
		Action = "preparing a medical or repair item",
		ActiveCraftItemSdesc = "an in-progress cinnamon stock craft",
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
			new CatalogueCommodityRecipeInput(Grams: 63.0, Material: "cinnamon", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("63 grams of cinnamon", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Shears", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Shears tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Awl Punch", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Awl Punch tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_medical_cinnamon_stock", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a cinnamon stock (@renaissance_medical_cinnamon_stock)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MedicalAndRepairPrepareClovesStock() => new()
	{
		Metadata = new("Medical and repair/prepare cloves stock", "crafts", ["renaissance"], ["item:renaissance_medical_cloves_stock"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "prepare cloves stock",
		Category = "Medical and repair",
		Blurb = "prepare a cloves stock",
		Action = "preparing a medical or repair item",
		ActiveCraftItemSdesc = "an in-progress cloves stock craft",
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
			new CatalogueCommodityRecipeInput(Grams: 63.0, Material: "cloves", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("63 grams of cloves", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Shears", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Shears tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Awl Punch", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Awl Punch tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_medical_cloves_stock", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a cloves stock (@renaissance_medical_cloves_stock)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MedicalAndRepairPrepareCorpseShroud() => new()
	{
		Metadata = new("Medical and repair/prepare corpse shroud", "crafts", ["renaissance"], ["item:renaissance_medical_corpse_shroud"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "prepare corpse shroud",
		Category = "Medical and repair",
		Blurb = "prepare a corpse shroud",
		Action = "preparing a medical or repair item",
		ActiveCraftItemSdesc = "an in-progress corpse shroud craft",
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
			new CatalogueCommodityRecipeInput(Grams: 294.0, Material: "linen", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("294 grams of linen", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Shears", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Shears tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Awl Punch", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Awl Punch tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_medical_corpse_shroud", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a corpse shroud (@renaissance_medical_corpse_shroud)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MedicalAndRepairPrepareCuppingGlass() => new()
	{
		Metadata = new("Medical and repair/prepare cupping glass", "crafts", ["renaissance"], ["item:renaissance_medical_cupping_glass"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "prepare cupping glass",
		Category = "Medical and repair",
		Blurb = "prepare a cupping glass",
		Action = "preparing a medical or repair item",
		ActiveCraftItemSdesc = "an in-progress cupping glass craft",
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
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_medical_cupping_glass", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a cupping glass (@renaissance_medical_cupping_glass)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MedicalAndRepairPrepareDentalForceps() => new()
	{
		Metadata = new("Medical and repair/prepare dental forceps", "crafts", ["renaissance"], ["item:renaissance_medical_dental_forceps"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "prepare dental forceps",
		Category = "Medical and repair",
		Blurb = "prepare a dental forceps",
		Action = "preparing a medical or repair item",
		ActiveCraftItemSdesc = "an in-progress dental forceps craft",
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
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_medical_dental_forceps", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a dental forceps (@renaissance_medical_dental_forceps)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MedicalAndRepairPrepareDentalPelican() => new()
	{
		Metadata = new("Medical and repair/prepare dental pelican", "crafts", ["renaissance"], ["item:renaissance_medical_dental_pelican"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "prepare dental pelican",
		Category = "Medical and repair",
		Blurb = "prepare a dental pelican",
		Action = "preparing a medical or repair item",
		ActiveCraftItemSdesc = "an in-progress dental pelican craft",
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
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_medical_dental_pelican", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a dental pelican (@renaissance_medical_dental_pelican)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MedicalAndRepairPrepareDissectionSheet() => new()
	{
		Metadata = new("Medical and repair/prepare dissection sheet", "crafts", ["renaissance"], ["item:renaissance_medical_dissection_sheet"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "prepare dissection sheet",
		Category = "Medical and repair",
		Blurb = "prepare a dissection sheet",
		Action = "preparing a medical or repair item",
		ActiveCraftItemSdesc = "an in-progress dissection sheet craft",
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
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_medical_dissection_sheet", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a dissection sheet (@renaissance_medical_dissection_sheet)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MedicalAndRepairPrepareDistillationAlembic() => new()
	{
		Metadata = new("Medical and repair/prepare distillation alembic", "crafts", ["renaissance"], ["item:renaissance_medical_distillation_alembic"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "prepare distillation alembic",
		Category = "Medical and repair",
		Blurb = "prepare a distillation alembic",
		Action = "preparing a medical or repair item",
		ActiveCraftItemSdesc = "an in-progress distillation alembic craft",
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
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_medical_distillation_alembic", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a distillation alembic (@renaissance_medical_distillation_alembic)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MedicalAndRepairPrepareDragHarness() => new()
	{
		Metadata = new("Medical and repair/prepare drag harness", "crafts", ["renaissance"], ["item:renaissance_medical_drag_harness"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "prepare drag harness",
		Category = "Medical and repair",
		Blurb = "prepare a drag harness",
		Action = "preparing a medical or repair item",
		ActiveCraftItemSdesc = "an in-progress drag harness craft",
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
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_medical_drag_harness", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a drag harness (@renaissance_medical_drag_harness)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MedicalAndRepairPrepareDrugSieve() => new()
	{
		Metadata = new("Medical and repair/prepare drug sieve", "crafts", ["renaissance"], ["item:renaissance_medical_drug_sieve"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "prepare drug sieve",
		Category = "Medical and repair",
		Blurb = "prepare a drug sieve",
		Action = "preparing a medical or repair item",
		ActiveCraftItemSdesc = "an in-progress drug sieve craft",
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
			new CatalogueCommodityRecipeInput(Grams: 294.0, Material: "brass", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("294 grams of brass", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Shears", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Shears tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Awl Punch", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Awl Punch tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_medical_drug_sieve", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a drug sieve (@renaissance_medical_drug_sieve)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MedicalAndRepairPrepareEnemaSyringe() => new()
	{
		Metadata = new("Medical and repair/prepare enema syringe", "crafts", ["renaissance"], ["item:renaissance_medical_enema_syringe"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "prepare enema syringe",
		Category = "Medical and repair",
		Blurb = "prepare an enema syringe",
		Action = "preparing a medical or repair item",
		ActiveCraftItemSdesc = "an in-progress enema syringe craft",
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
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_medical_enema_syringe", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x an enema syringe (@renaissance_medical_enema_syringe)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MedicalAndRepairPrepareFarrierMedicineRoll() => new()
	{
		Metadata = new("Medical and repair/prepare farrier medicine roll", "crafts", ["renaissance"], ["item:renaissance_medical_farrier_medicine_roll"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "prepare farrier medicine roll",
		Category = "Medical and repair",
		Blurb = "prepare a farrier medicine roll",
		Action = "preparing a medical or repair item",
		ActiveCraftItemSdesc = "an in-progress farrier medicine roll craft",
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
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_medical_farrier_medicine_roll", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a farrier medicine roll (@renaissance_medical_farrier_medicine_roll)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MedicalAndRepairPrepareFeverCompress() => new()
	{
		Metadata = new("Medical and repair/prepare fever compress", "crafts", ["renaissance"], ["item:renaissance_medical_fever_compress"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "prepare fever compress",
		Category = "Medical and repair",
		Blurb = "prepare a fever compress",
		Action = "preparing a medical or repair item",
		ActiveCraftItemSdesc = "an in-progress fever compress craft",
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
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_medical_fever_compress", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a fever compress (@renaissance_medical_fever_compress)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MedicalAndRepairPrepareFieldStretcher() => new()
	{
		Metadata = new("Medical and repair/prepare field stretcher", "crafts", ["renaissance"], ["item:renaissance_medical_field_stretcher"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "prepare field stretcher",
		Category = "Medical and repair",
		Blurb = "prepare a field stretcher",
		Action = "preparing a medical or repair item",
		ActiveCraftItemSdesc = "an in-progress field stretcher craft",
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
			new CatalogueCommodityRecipeInput(Grams: 1260.0, Material: "oak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1260 grams of oak", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Shears", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Shears tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Awl Punch", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Awl Punch tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_medical_field_stretcher", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a field stretcher (@renaissance_medical_field_stretcher)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MedicalAndRepairPrepareFoldingLancet() => new()
	{
		Metadata = new("Medical and repair/prepare folding lancet", "crafts", ["renaissance"], ["item:renaissance_medical_folding_lancet"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "prepare folding lancet",
		Category = "Medical and repair",
		Blurb = "prepare a folding lancet",
		Action = "preparing a medical or repair item",
		ActiveCraftItemSdesc = "an in-progress folding lancet craft",
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
			new CatalogueCommodityRecipeInput(Grams: 126.0, Material: "wrought iron", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("126 grams of wrought iron", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Shears", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Shears tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Awl Punch", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Awl Punch tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_medical_folding_lancet", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a folding lancet (@renaissance_medical_folding_lancet)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MedicalAndRepairPrepareFractureFrame() => new()
	{
		Metadata = new("Medical and repair/prepare fracture frame", "crafts", ["renaissance"], ["item:renaissance_medical_fracture_frame"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "prepare fracture frame",
		Category = "Medical and repair",
		Blurb = "prepare a fracture frame",
		Action = "preparing a medical or repair item",
		ActiveCraftItemSdesc = "an in-progress fracture frame craft",
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
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_medical_fracture_frame", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a fracture frame (@renaissance_medical_fracture_frame)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MedicalAndRepairPrepareFumigationBrazier() => new()
	{
		Metadata = new("Medical and repair/prepare fumigation brazier", "crafts", ["renaissance"], ["item:renaissance_medical_fumigation_brazier"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "prepare fumigation brazier",
		Category = "Medical and repair",
		Blurb = "prepare a fumigation brazier",
		Action = "preparing a medical or repair item",
		ActiveCraftItemSdesc = "an in-progress fumigation brazier craft",
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
			new CatalogueCommodityRecipeInput(Grams: 1820.0, Material: "bronze", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1820 grams of bronze", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Shears", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Shears tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Awl Punch", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Awl Punch tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_medical_fumigation_brazier", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a fumigation brazier (@renaissance_medical_fumigation_brazier)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MedicalAndRepairPrepareGingerStock() => new()
	{
		Metadata = new("Medical and repair/prepare ginger stock", "crafts", ["renaissance"], ["item:renaissance_medical_ginger_stock"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "prepare ginger stock",
		Category = "Medical and repair",
		Blurb = "prepare a ginger stock",
		Action = "preparing a medical or repair item",
		ActiveCraftItemSdesc = "an in-progress ginger stock craft",
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
			new CatalogueCommodityRecipeInput(Grams: 63.0, Material: "ginger", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("63 grams of ginger", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Shears", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Shears tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Awl Punch", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Awl Punch tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_medical_ginger_stock", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a ginger stock (@renaissance_medical_ginger_stock)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MedicalAndRepairPrepareGlassEye() => new()
	{
		Metadata = new("Medical and repair/prepare glass eye", "crafts", ["renaissance"], ["item:renaissance_medical_glass_eye"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "prepare glass eye",
		Category = "Medical and repair",
		Blurb = "prepare a glass eye",
		Action = "preparing a medical or repair item",
		ActiveCraftItemSdesc = "an in-progress glass eye craft",
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
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_medical_glass_eye", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a glass eye (@renaissance_medical_glass_eye)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MedicalAndRepairPrepareGlassRetort() => new()
	{
		Metadata = new("Medical and repair/prepare glass retort", "crafts", ["renaissance"], ["item:renaissance_medical_glass_retort"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "prepare glass retort",
		Category = "Medical and repair",
		Blurb = "prepare a glass retort",
		Action = "preparing a medical or repair item",
		ActiveCraftItemSdesc = "an in-progress glass retort craft",
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
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_medical_glass_retort", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a glass retort (@renaissance_medical_glass_retort)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MedicalAndRepairPrepareGuaiacumWoodStock() => new()
	{
		Metadata = new("Medical and repair/prepare guaiacum wood stock", "crafts", ["renaissance"], ["item:renaissance_medical_guaiacum_wood_stock"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "prepare guaiacum wood stock",
		Category = "Medical and repair",
		Blurb = "prepare a guaiacum wood stock",
		Action = "preparing a medical or repair item",
		ActiveCraftItemSdesc = "an in-progress guaiacum wood stock craft",
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
			new CatalogueCommodityRecipeInput(Grams: 63.0, Material: "guaiacum wood", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("63 grams of guaiacum wood", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Shears", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Shears tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Awl Punch", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Awl Punch tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_medical_guaiacum_wood_stock", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a guaiacum wood stock (@renaissance_medical_guaiacum_wood_stock)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MedicalAndRepairPrepareHerbDryingRack() => new()
	{
		Metadata = new("Medical and repair/prepare herb drying rack", "crafts", ["renaissance"], ["item:renaissance_medical_herb_drying_rack"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "prepare herb drying rack",
		Category = "Medical and repair",
		Blurb = "prepare a herb drying rack",
		Action = "preparing a medical or repair item",
		ActiveCraftItemSdesc = "an in-progress herb drying rack craft",
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
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_medical_herb_drying_rack", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a herb drying rack (@renaissance_medical_herb_drying_rack)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MedicalAndRepairPrepareHoofPoultice() => new()
	{
		Metadata = new("Medical and repair/prepare hoof poultice", "crafts", ["renaissance"], ["item:renaissance_medical_hoof_poultice"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "prepare hoof poultice",
		Category = "Medical and repair",
		Blurb = "prepare a hoof poultice",
		Action = "preparing a medical or repair item",
		ActiveCraftItemSdesc = "an in-progress hoof poultice craft",
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
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_medical_hoof_poultice", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a hoof poultice (@renaissance_medical_hoof_poultice)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MedicalAndRepairPrepareHookHand() => new()
	{
		Metadata = new("Medical and repair/prepare hook hand", "crafts", ["renaissance"], ["item:renaissance_medical_hook_hand"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "prepare hook hand",
		Category = "Medical and repair",
		Blurb = "prepare a hook hand",
		Action = "preparing a medical or repair item",
		ActiveCraftItemSdesc = "an in-progress hook hand craft",
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
			new CatalogueCommodityRecipeInput(Grams: 875.0, Material: "oak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("875 grams of oak", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Shears", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Shears tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Awl Punch", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Awl Punch tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_medical_hook_hand", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a hook hand (@renaissance_medical_hook_hand)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MedicalAndRepairPrepareHorseFleam() => new()
	{
		Metadata = new("Medical and repair/prepare horse fleam", "crafts", ["renaissance"], ["item:renaissance_medical_horse_fleam"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "prepare horse fleam",
		Category = "Medical and repair",
		Blurb = "prepare a horse fleam",
		Action = "preparing a medical or repair item",
		ActiveCraftItemSdesc = "an in-progress horse fleam craft",
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
			new CatalogueCommodityRecipeInput(Grams: 126.0, Material: "wrought iron", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("126 grams of wrought iron", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Shears", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Shears tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Awl Punch", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Awl Punch tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_medical_horse_fleam", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a horse fleam (@renaissance_medical_horse_fleam)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MedicalAndRepairPrepareHorseSling() => new()
	{
		Metadata = new("Medical and repair/prepare horse sling", "crafts", ["renaissance"], ["item:renaissance_medical_horse_sling"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "prepare horse sling",
		Category = "Medical and repair",
		Blurb = "prepare a horse sling",
		Action = "preparing a medical or repair item",
		ActiveCraftItemSdesc = "an in-progress horse sling craft",
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
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_medical_horse_sling", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a horse sling (@renaissance_medical_horse_sling)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MedicalAndRepairPrepareHospitalScreen() => new()
	{
		Metadata = new("Medical and repair/prepare hospital screen", "crafts", ["renaissance"], ["item:renaissance_medical_hospital_screen"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "prepare hospital screen",
		Category = "Medical and repair",
		Blurb = "prepare a hospital screen",
		Action = "preparing a medical or repair item",
		ActiveCraftItemSdesc = "an in-progress hospital screen craft",
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
			new CatalogueCommodityRecipeInput(Grams: 1260.0, Material: "oak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1260 grams of oak", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Shears", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Shears tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Awl Punch", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Awl Punch tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_medical_hospital_screen", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a hospital screen (@renaissance_medical_hospital_screen)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MedicalAndRepairPrepareInvalidChair() => new()
	{
		Metadata = new("Medical and repair/prepare invalid chair", "crafts", ["renaissance"], ["item:renaissance_medical_invalid_chair"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "prepare invalid chair",
		Category = "Medical and repair",
		Blurb = "prepare an invalid chair",
		Action = "preparing a medical or repair item",
		ActiveCraftItemSdesc = "an in-progress invalid chair craft",
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
			new CatalogueCommodityRecipeInput(Grams: 1260.0, Material: "oak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1260 grams of oak", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Shears", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Shears tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Awl Punch", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Awl Punch tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_medical_invalid_chair", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x an invalid chair (@renaissance_medical_invalid_chair)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MedicalAndRepairPrepareIsolationPlacard() => new()
	{
		Metadata = new("Medical and repair/prepare isolation placard", "crafts", ["renaissance"], ["item:renaissance_medical_isolation_placard"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "prepare isolation placard",
		Category = "Medical and repair",
		Blurb = "prepare an isolation placard",
		Action = "preparing a medical or repair item",
		ActiveCraftItemSdesc = "an in-progress isolation placard craft",
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
			new CatalogueCommodityRecipeInput(Grams: 420.0, Material: "oak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("420 grams of oak", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Shears", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Shears tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Awl Punch", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Awl Punch tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_medical_isolation_placard", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x an isolation placard (@renaissance_medical_isolation_placard)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MedicalAndRepairPrepareLabelledGallipot() => new()
	{
		Metadata = new("Medical and repair/prepare labelled gallipot", "crafts", ["renaissance"], ["item:renaissance_medical_labelled_gallipot"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "prepare labelled gallipot",
		Category = "Medical and repair",
		Blurb = "prepare a labelled gallipot",
		Action = "preparing a medical or repair item",
		ActiveCraftItemSdesc = "an in-progress labelled gallipot craft",
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
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_medical_labelled_gallipot", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a labelled gallipot (@renaissance_medical_labelled_gallipot)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MedicalAndRepairPrepareLeechJar() => new()
	{
		Metadata = new("Medical and repair/prepare leech jar", "crafts", ["renaissance"], ["item:renaissance_medical_leech_jar"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "prepare leech jar",
		Category = "Medical and repair",
		Blurb = "prepare a leech jar",
		Action = "preparing a medical or repair item",
		ActiveCraftItemSdesc = "an in-progress leech jar craft",
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
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_medical_leech_jar", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a leech jar (@renaissance_medical_leech_jar)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};
}
