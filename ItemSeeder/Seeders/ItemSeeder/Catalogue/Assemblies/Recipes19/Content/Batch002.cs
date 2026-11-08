#nullable enable

using MudSharp.Body;
using MudSharp.GameItems.Inventory.Plans;
using MudSharp.RPG.Checks;

namespace DatabaseSeeder.Seeders;

public static partial class Recipes19Stock
{
	public static CatalogueRecipeDefinition GeneralCraftworkMakeColourRopeSoledDeckShoes() => new()
	{
		Metadata = new("General craftwork/make $colour rope-soled deck shoes", "crafts", ["renaissance"], ["item:renaissance_shared_clothing_ropesole_deckshoes"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make $colour rope-soled deck shoes",
		Category = "General craftwork",
		Blurb = "make a pair of $colour rope-soled deck shoes",
		Action = "making a finished item",
		ActiveCraftItemSdesc = "an in-progress $colour rope-soled deck shoes craft",
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
			new CatalogueCommodityRecipeInput(Grams: 518.0, Material: "canvas", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("518 grams of canvas", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_shared_clothing_ropesole_deckshoes", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a pair of $colour rope-soled deck shoes (@renaissance_shared_clothing_ropesole_deckshoes)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition TextileAndLeatherWorkTailorColourSeparateClothHood() => new()
	{
		Metadata = new("Textile and leather work/tailor $colour separate cloth hood", "crafts", ["renaissance"], ["item:renaissance_shared_clothing_separate_hood"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "tailor $colour separate cloth hood",
		Category = "Textile and leather work",
		Blurb = "tailor a $colour separate cloth hood",
		Action = "tailoring a textile or leather item",
		ActiveCraftItemSdesc = "an in-progress $colour separate cloth hood craft",
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
			new CatalogueCommodityRecipeInput(Grams: 225.0, Material: "wool", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("225 grams of wool", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Sewing Needle", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Sewing Needle tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Shears", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Shears tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_shared_clothing_separate_hood", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a $colour separate cloth hood (@renaissance_shared_clothing_separate_hood)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition TextileAndLeatherWorkTailorShortColourHeadVeil() => new()
	{
		Metadata = new("Textile and leather work/tailor short $colour head veil", "crafts", ["renaissance"], ["item:renaissance_shared_clothing_short_head_veil"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "tailor short $colour head veil",
		Category = "Textile and leather work",
		Blurb = "tailor a short $colour head veil",
		Action = "tailoring a textile or leather item",
		ActiveCraftItemSdesc = "an in-progress short $colour head veil craft",
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
			new CatalogueCommodityRecipeInput(Grams: 90.0, Material: "linen", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("90 grams of linen", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Sewing Needle", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Sewing Needle tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Shears", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Shears tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_shared_clothing_short_head_veil", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a short $colour head veil (@renaissance_shared_clothing_short_head_veil)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition TextileAndLeatherWorkTailorColourShortUndershirt() => new()
	{
		Metadata = new("Textile and leather work/tailor $colour short undershirt", "crafts", ["renaissance"], ["item:renaissance_shared_clothing_short_undershirt"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "tailor $colour short undershirt",
		Category = "Textile and leather work",
		Blurb = "tailor a $colour short undershirt",
		Action = "tailoring a textile or leather item",
		ActiveCraftItemSdesc = "an in-progress $colour short undershirt craft",
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
			new CatalogueCommodityRecipeInput(Grams: 195.0, Material: "linen", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("195 grams of linen", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Sewing Needle", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Sewing Needle tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Shears", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Shears tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_shared_clothing_short_undershirt", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a $colour short undershirt (@renaissance_shared_clothing_short_undershirt)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition TextileAndLeatherWorkTailorColourLongShoulderScarf() => new()
	{
		Metadata = new("Textile and leather work/tailor $colour long shoulder scarf", "crafts", ["renaissance"], ["item:renaissance_shared_clothing_shoulder_scarf"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "tailor $colour long shoulder scarf",
		Category = "Textile and leather work",
		Blurb = "tailor a $colour long shoulder scarf",
		Action = "tailoring a textile or leather item",
		ActiveCraftItemSdesc = "an in-progress $colour long shoulder scarf craft",
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
			new CatalogueCommodityRecipeInput(Grams: 465.0, Material: "silk", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("465 grams of silk", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Sewing Needle", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Sewing Needle tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Shears", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Shears tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_shared_clothing_shoulder_scarf", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a $colour long shoulder scarf (@renaissance_shared_clothing_shoulder_scarf)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition TextileAndLeatherWorkTailorBroadColourWoolShoulderShawl() => new()
	{
		Metadata = new("Textile and leather work/tailor broad $colour wool shoulder shawl", "crafts", ["renaissance"], ["item:renaissance_shared_clothing_shoulder_shawl"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "tailor broad $colour wool shoulder shawl",
		Category = "Textile and leather work",
		Blurb = "tailor a broad $colour wool shoulder shawl",
		Action = "tailoring a textile or leather item",
		ActiveCraftItemSdesc = "an in-progress broad $colour wool shoulder shawl craft",
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
			new CatalogueCommodityRecipeInput(Grams: 510.0, Material: "wool", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("510 grams of wool", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Sewing Needle", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Sewing Needle tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Shears", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Shears tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_shared_clothing_shoulder_shawl", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a broad $colour wool shoulder shawl (@renaissance_shared_clothing_shoulder_shawl)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition TextileAndLeatherWorkTailorColourSideLacedTownShoes() => new()
	{
		Metadata = new("Textile and leather work/tailor $colour side-laced town shoes", "crafts", ["renaissance"], ["item:renaissance_shared_clothing_sidelaced_townshoes"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "tailor $colour side-laced town shoes",
		Category = "Textile and leather work",
		Blurb = "tailor a pair of $colour side-laced town shoes",
		Action = "tailoring a textile or leather item",
		ActiveCraftItemSdesc = "an in-progress $colour side-laced town shoes craft",
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
			new CatalogueCommodityRecipeInput(Grams: 518.0, Material: "leather", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("518 grams of leather", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Awl Punch", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Awl Punch tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Shears", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Shears tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_shared_clothing_sidelaced_townshoes", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a pair of $colour side-laced town shoes (@renaissance_shared_clothing_sidelaced_townshoes)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition TextileAndLeatherWorkTailorColourSideSlitTunic() => new()
	{
		Metadata = new("Textile and leather work/tailor $colour side-slit tunic", "crafts", ["renaissance"], ["item:renaissance_shared_clothing_side_slit_tunic"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "tailor $colour side-slit tunic",
		Category = "Textile and leather work",
		Blurb = "tailor a $colour side-slit tunic",
		Action = "tailoring a textile or leather item",
		ActiveCraftItemSdesc = "an in-progress $colour side-slit tunic craft",
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
			new CatalogueCommodityRecipeInput(Grams: 360.0, Material: "cotton", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("360 grams of cotton", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Sewing Needle", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Sewing Needle tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Shears", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Shears tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_shared_clothing_side_slit_tunic", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a $colour side-slit tunic (@renaissance_shared_clothing_side_slit_tunic)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition TextileAndLeatherWorkTailorColourSleevelessShortCape() => new()
	{
		Metadata = new("Textile and leather work/tailor $colour sleeveless short cape", "crafts", ["renaissance"], ["item:renaissance_shared_clothing_sleeveless_cape"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "tailor $colour sleeveless short cape",
		Category = "Textile and leather work",
		Blurb = "tailor a $colour sleeveless short cape",
		Action = "tailoring a textile or leather item",
		ActiveCraftItemSdesc = "an in-progress $colour sleeveless short cape craft",
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
			new CatalogueCommodityRecipeInput(Grams: 788.0, Material: "wool", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("788 grams of wool", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Sewing Needle", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Sewing Needle tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Shears", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Shears tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_shared_clothing_sleeveless_cape", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a $colour sleeveless short cape (@renaissance_shared_clothing_sleeveless_cape)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition TextileAndLeatherWorkTailorColourSleevelessOverVest() => new()
	{
		Metadata = new("Textile and leather work/tailor $colour sleeveless over-vest", "crafts", ["renaissance"], ["item:renaissance_shared_clothing_sleeveless_overvest"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "tailor $colour sleeveless over-vest",
		Category = "Textile and leather work",
		Blurb = "tailor a $colour sleeveless over-vest",
		Action = "tailoring a textile or leather item",
		ActiveCraftItemSdesc = "an in-progress $colour sleeveless over-vest craft",
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
			new CatalogueCommodityRecipeInput(Grams: 420.0, Material: "wool", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("420 grams of wool", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Sewing Needle", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Sewing Needle tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Shears", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Shears tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_shared_clothing_sleeveless_overvest", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a $colour sleeveless over-vest (@renaissance_shared_clothing_sleeveless_overvest)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition TextileAndLeatherWorkTailorColourSleevelessUndervest() => new()
	{
		Metadata = new("Textile and leather work/tailor $colour sleeveless undervest", "crafts", ["renaissance"], ["item:renaissance_shared_clothing_sleeveless_undervest"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "tailor $colour sleeveless undervest",
		Category = "Textile and leather work",
		Blurb = "tailor a $colour sleeveless undervest",
		Action = "tailoring a textile or leather item",
		ActiveCraftItemSdesc = "an in-progress $colour sleeveless undervest craft",
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
			new CatalogueCommodityRecipeInput(Grams: 420.0, Material: "linen", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("420 grams of linen", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Sewing Needle", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Sewing Needle tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Shears", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Shears tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_shared_clothing_sleeveless_undervest", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a $colour sleeveless undervest (@renaissance_shared_clothing_sleeveless_undervest)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition TextileAndLeatherWorkTailorSoftColourBrimlessCap() => new()
	{
		Metadata = new("Textile and leather work/tailor soft $colour brimless cap", "crafts", ["renaissance"], ["item:renaissance_shared_clothing_soft_brimless_cap"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "tailor soft $colour brimless cap",
		Category = "Textile and leather work",
		Blurb = "tailor a soft $colour brimless cap",
		Action = "tailoring a textile or leather item",
		ActiveCraftItemSdesc = "an in-progress soft $colour brimless cap craft",
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
			new CatalogueCommodityRecipeInput(Grams: 143.0, Material: "wool", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("143 grams of wool", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Sewing Needle", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Sewing Needle tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Shears", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Shears tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_shared_clothing_soft_brimless_cap", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a soft $colour brimless cap (@renaissance_shared_clothing_soft_brimless_cap)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition TextileAndLeatherWorkTailorSoftColourLeatherSlippers() => new()
	{
		Metadata = new("Textile and leather work/tailor soft $colour leather slippers", "crafts", ["renaissance"], ["item:renaissance_shared_clothing_soft_slippers"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "tailor soft $colour leather slippers",
		Category = "Textile and leather work",
		Blurb = "tailor a pair of soft $colour leather slippers",
		Action = "tailoring a textile or leather item",
		ActiveCraftItemSdesc = "an in-progress soft $colour leather slippers craft",
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
			new CatalogueCommodityRecipeInput(Grams: 488.0, Material: "leather", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("488 grams of leather", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Awl Punch", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Awl Punch tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Shears", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Shears tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_shared_clothing_soft_slippers", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a pair of soft $colour leather slippers (@renaissance_shared_clothing_soft_slippers)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition TextileAndLeatherWorkTailorColourSolelessClothStockings() => new()
	{
		Metadata = new("Textile and leather work/tailor $colour soleless cloth stockings", "crafts", ["renaissance"], ["item:renaissance_shared_clothing_soleless_stockings"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "tailor $colour soleless cloth stockings",
		Category = "Textile and leather work",
		Blurb = "tailor a $colour soleless cloth stockings",
		Action = "tailoring a textile or leather item",
		ActiveCraftItemSdesc = "an in-progress $colour soleless cloth stockings craft",
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
			new CatalogueCommodityRecipeInput(Grams: 210.0, Material: "wool", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("210 grams of wool", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Sewing Needle", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Sewing Needle tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Shears", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Shears tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_shared_clothing_soleless_stockings", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a $colour soleless cloth stockings (@renaissance_shared_clothing_soleless_stockings)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition TextileAndLeatherWorkTailorColourStraightWoolTrousers() => new()
	{
		Metadata = new("Textile and leather work/tailor $colour straight wool trousers", "crafts", ["renaissance"], ["item:renaissance_shared_clothing_straight_trousers"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "tailor $colour straight wool trousers",
		Category = "Textile and leather work",
		Blurb = "tailor a pair of $colour straight wool trousers",
		Action = "tailoring a textile or leather item",
		ActiveCraftItemSdesc = "an in-progress $colour straight wool trousers craft",
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
			new CatalogueCommodityRecipeInput(Grams: 465.0, Material: "wool", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("465 grams of wool", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Sewing Needle", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Sewing Needle tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Shears", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Shears tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_shared_clothing_straight_trousers", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a pair of $colour straight wool trousers (@renaissance_shared_clothing_straight_trousers)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition TextileAndLeatherWorkTailorColourStraightCutTunic() => new()
	{
		Metadata = new("Textile and leather work/tailor $colour straight-cut tunic", "crafts", ["renaissance"], ["item:renaissance_shared_clothing_straight_tunic"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "tailor $colour straight-cut tunic",
		Category = "Textile and leather work",
		Blurb = "tailor a $colour straight-cut tunic",
		Action = "tailoring a textile or leather item",
		ActiveCraftItemSdesc = "an in-progress $colour straight-cut tunic craft",
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
			new CatalogueCommodityRecipeInput(Grams: 360.0, Material: "linen", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("360 grams of linen", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Sewing Needle", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Sewing Needle tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Shears", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Shears tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_shared_clothing_straight_tunic", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a $colour straight-cut tunic (@renaissance_shared_clothing_straight_tunic)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition TextileAndLeatherWorkTailorColourStraightUnderRobe() => new()
	{
		Metadata = new("Textile and leather work/tailor $colour straight under-robe", "crafts", ["renaissance"], ["item:renaissance_shared_clothing_straight_underrobe"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "tailor $colour straight under-robe",
		Category = "Textile and leather work",
		Blurb = "tailor a $colour straight under-robe",
		Action = "tailoring a textile or leather item",
		ActiveCraftItemSdesc = "an in-progress $colour straight under-robe craft",
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
			new CatalogueCommodityRecipeInput(Grams: 585.0, Material: "linen", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("585 grams of linen", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Sewing Needle", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Sewing Needle tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Shears", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Shears tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_shared_clothing_straight_underrobe", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a $colour straight under-robe (@renaissance_shared_clothing_straight_underrobe)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition GeneralCraftworkMakeColourWovenStrawHat() => new()
	{
		Metadata = new("General craftwork/make $colour woven straw hat", "crafts", ["renaissance"], ["item:renaissance_shared_clothing_straw_brimmed_hat"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make $colour woven straw hat",
		Category = "General craftwork",
		Blurb = "make a $colour woven straw hat",
		Action = "making a finished item",
		ActiveCraftItemSdesc = "an in-progress $colour woven straw hat craft",
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
			new CatalogueCommodityRecipeInput(Grams: 143.0, Material: "straw", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("143 grams of straw", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_shared_clothing_straw_brimmed_hat", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a $colour woven straw hat (@renaissance_shared_clothing_straw_brimmed_hat)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition TextileAndLeatherWorkTailorColourTallCrownedFeltHat() => new()
	{
		Metadata = new("Textile and leather work/tailor $colour tall-crowned felt hat", "crafts", ["renaissance"], ["item:renaissance_shared_clothing_tallcrown_felthat"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "tailor $colour tall-crowned felt hat",
		Category = "Textile and leather work",
		Blurb = "tailor a $colour tall-crowned felt hat",
		Action = "tailoring a textile or leather item",
		ActiveCraftItemSdesc = "an in-progress $colour tall-crowned felt hat craft",
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
			new CatalogueCommodityRecipeInput(Grams: 143.0, Material: "felt", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("143 grams of felt", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Sewing Needle", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Sewing Needle tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Shears", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Shears tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_shared_clothing_tallcrown_felthat", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a $colour tall-crowned felt hat (@renaissance_shared_clothing_tallcrown_felthat)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition GeneralCraftworkMakeColourCloseTarredCanvasDeckCap() => new()
	{
		Metadata = new("General craftwork/make $colour close tarred canvas deck cap", "crafts", ["renaissance"], ["item:renaissance_shared_clothing_tarred_canvas_deckcap"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make $colour close tarred canvas deck cap",
		Category = "General craftwork",
		Blurb = "make a $colour close tarred canvas deck cap",
		Action = "making a finished item",
		ActiveCraftItemSdesc = "an in-progress $colour close tarred canvas deck cap craft",
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
			new CatalogueCommodityRecipeInput(Grams: 143.0, Material: "canvas", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("143 grams of canvas", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_shared_clothing_tarred_canvas_deckcap", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a $colour close tarred canvas deck cap (@renaissance_shared_clothing_tarred_canvas_deckcap)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition GeneralCraftworkMakeColourWovenSandals() => new()
	{
		Metadata = new("General craftwork/make $colour woven sandals", "crafts", ["renaissance"], ["item:renaissance_shared_clothing_textile_sandals"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make $colour woven sandals",
		Category = "General craftwork",
		Blurb = "make a pair of $colour woven sandals",
		Action = "making a finished item",
		ActiveCraftItemSdesc = "an in-progress $colour woven sandals craft",
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
			new CatalogueCommodityRecipeInput(Grams: 278.0, Material: "canvas", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("278 grams of canvas", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_shared_clothing_textile_sandals", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a pair of $colour woven sandals (@renaissance_shared_clothing_textile_sandals)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition TextileAndLeatherWorkTailorColourThickSoledWorkShoes() => new()
	{
		Metadata = new("Textile and leather work/tailor $colour thick-soled work shoes", "crafts", ["renaissance"], ["item:renaissance_shared_clothing_thicksoled_workshoes"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "tailor $colour thick-soled work shoes",
		Category = "Textile and leather work",
		Blurb = "tailor a pair of $colour thick-soled work shoes",
		Action = "tailoring a textile or leather item",
		ActiveCraftItemSdesc = "an in-progress $colour thick-soled work shoes craft",
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
			new(40, "$0 cut|cuts the leather to shape with $t2 and work|works its edges true. $0 pause|pauses to check the proportions before carrying on.", "$0 cut|cuts the leather to shape with $t2 and work|works its edges true. $0 pause|pauses to check the proportions before carrying on.", ExertionLevel.Stasis, 0),
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
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_shared_clothing_thicksoled_workshoes", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a pair of $colour thick-soled work shoes (@renaissance_shared_clothing_thicksoled_workshoes)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition GeneralCraftworkMakeLooseColourTravellingCoat() => new()
	{
		Metadata = new("General craftwork/make loose $colour travelling coat", "crafts", ["renaissance"], ["item:renaissance_shared_clothing_travelling_coat"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make loose $colour travelling coat",
		Category = "General craftwork",
		Blurb = "make a loose $colour travelling coat",
		Action = "making a finished item",
		ActiveCraftItemSdesc = "an in-progress loose $colour travelling coat craft",
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
			new CatalogueCommodityRecipeInput(Grams: 1005.0, Material: "broadcloth", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1005 grams of broadcloth", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_shared_clothing_travelling_coat", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a loose $colour travelling coat (@renaissance_shared_clothing_travelling_coat)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition CarpentryAndJoineryBuildColourWoodenSoledOvershoes() => new()
	{
		Metadata = new("Carpentry and joinery/build $colour wooden-soled overshoes", "crafts", ["renaissance"], ["item:renaissance_shared_clothing_wooden_soled_overshoes"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "build $colour wooden-soled overshoes",
		Category = "Carpentry and joinery",
		Blurb = "build a pair of $colour wooden-soled overshoes",
		Action = "building a wooden item",
		ActiveCraftItemSdesc = "an in-progress $colour wooden-soled overshoes craft",
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
			new(45, "$0 fit|fits the joins and secure|secures them with $t1. $0 turn|turns the work over to judge its balance.", "$0 fit|fits the joins and secure|secures them with $t1. $0 turn|turns the work over to judge its balance.", ExertionLevel.Stasis, 0),
			new(30, "$0 smooth|smooths the finished piece and set|sets aside $p1.", "$0 find|finds the join has opened and set|sets the material aside.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 304.0, Material: "wood", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("304 grams of wood", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Saw", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Saw tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_shared_clothing_wooden_soled_overshoes", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a pair of $colour wooden-soled overshoes (@renaissance_shared_clothing_wooden_soled_overshoes)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition TextileAndLeatherWorkTailorColourWrappedLoincloth() => new()
	{
		Metadata = new("Textile and leather work/tailor $colour wrapped loincloth", "crafts", ["renaissance"], ["item:renaissance_shared_clothing_wrapped_loincloth"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "tailor $colour wrapped loincloth",
		Category = "Textile and leather work",
		Blurb = "tailor a $colour wrapped loincloth",
		Action = "tailoring a textile or leather item",
		ActiveCraftItemSdesc = "an in-progress $colour wrapped loincloth craft",
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
			new CatalogueCommodityRecipeInput(Grams: 75.0, Material: "linen", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("75 grams of linen", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Sewing Needle", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Sewing Needle tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Shears", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Shears tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_shared_clothing_wrapped_loincloth", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a $colour wrapped loincloth (@renaissance_shared_clothing_wrapped_loincloth)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition TextileAndLeatherWorkTailorColourShortWrapJacket() => new()
	{
		Metadata = new("Textile and leather work/tailor $colour short wrap jacket", "crafts", ["renaissance"], ["item:renaissance_shared_clothing_wrap_jacket"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "tailor $colour short wrap jacket",
		Category = "Textile and leather work",
		Blurb = "tailor a $colour short wrap jacket",
		Action = "tailoring a textile or leather item",
		ActiveCraftItemSdesc = "an in-progress $colour short wrap jacket craft",
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
			new CatalogueCommodityRecipeInput(Grams: 390.0, Material: "linen", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("390 grams of linen", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Sewing Needle", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Sewing Needle tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Shears", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Shears tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_shared_clothing_wrap_jacket", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a $colour short wrap jacket (@renaissance_shared_clothing_wrap_jacket)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition TextileAndLeatherWorkTailorColourOverlappingWrapSkirt() => new()
	{
		Metadata = new("Textile and leather work/tailor $colour overlapping wrap skirt", "crafts", ["renaissance"], ["item:renaissance_shared_clothing_wrap_skirt"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "tailor $colour overlapping wrap skirt",
		Category = "Textile and leather work",
		Blurb = "tailor a $colour overlapping wrap skirt",
		Action = "tailoring a textile or leather item",
		ActiveCraftItemSdesc = "an in-progress $colour overlapping wrap skirt craft",
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
			new CatalogueCommodityRecipeInput(Grams: 510.0, Material: "cotton", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("510 grams of cotton", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Sewing Needle", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Sewing Needle tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Shears", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Shears tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_shared_clothing_wrap_skirt", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a $colour overlapping wrap skirt (@renaissance_shared_clothing_wrap_skirt)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition TextileAndLeatherWorkTailorBackTiedColourCottonBodice() => new()
	{
		Metadata = new("Textile and leather work/tailor back-tied $colour cotton bodice", "crafts", ["renaissance"], ["item:renaissance_southasian_backtied_bodice"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "tailor back-tied $colour cotton bodice",
		Category = "Textile and leather work",
		Blurb = "tailor a back-tied $colour cotton bodice",
		Action = "tailoring a textile or leather item",
		ActiveCraftItemSdesc = "an in-progress back-tied $colour cotton bodice craft",
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
			new CatalogueCommodityRecipeInput(Grams: 375.0, Material: "cotton", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("375 grams of cotton", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Sewing Needle", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Sewing Needle tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Shears", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Shears tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_southasian_backtied_bodice", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a back-tied $colour cotton bodice (@renaissance_southasian_backtied_bodice)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition TextileAndLeatherWorkTailorColourBeadedSilkCourtHairnet() => new()
	{
		Metadata = new("Textile and leather work/tailor $colour beaded silk court hairnet", "crafts", ["renaissance"], ["item:renaissance_southasian_beaded_silk_hairnet"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "tailor $colour beaded silk court hairnet",
		Category = "Textile and leather work",
		Blurb = "tailor a $colour beaded silk court hairnet",
		Action = "tailoring a textile or leather item",
		ActiveCraftItemSdesc = "an in-progress $colour beaded silk court hairnet craft",
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
			new CatalogueCommodityRecipeInput(Grams: 143.0, Material: "silk", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("143 grams of silk", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Sewing Needle", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Sewing Needle tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Shears", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Shears tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_southasian_beaded_silk_hairnet", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a $colour beaded silk court hairnet (@renaissance_southasian_beaded_silk_hairnet)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition TextileAndLeatherWorkTailorNarrowColourBunchedAnkleTrousers() => new()
	{
		Metadata = new("Textile and leather work/tailor narrow $colour bunched ankle trousers", "crafts", ["renaissance"], ["item:renaissance_southasian_bunched_ankletrousers"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "tailor narrow $colour bunched ankle trousers",
		Category = "Textile and leather work",
		Blurb = "tailor a pair of narrow $colour bunched ankle trousers",
		Action = "tailoring a textile or leather item",
		ActiveCraftItemSdesc = "an in-progress narrow $colour bunched ankle trousers craft",
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
			new CatalogueCommodityRecipeInput(Grams: 375.0, Material: "cotton", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("375 grams of cotton", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Sewing Needle", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Sewing Needle tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Shears", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Shears tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_southasian_bunched_ankletrousers", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a pair of narrow $colour bunched ankle trousers (@renaissance_southasian_bunched_ankletrousers)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition TextileAndLeatherWorkTailorColourChinTiedRidingTurban() => new()
	{
		Metadata = new("Textile and leather work/tailor $colour chin-tied riding turban", "crafts", ["renaissance"], ["item:renaissance_southasian_chintied_ridingturban"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "tailor $colour chin-tied riding turban",
		Category = "Textile and leather work",
		Blurb = "tailor a $colour chin-tied riding turban",
		Action = "tailoring a textile or leather item",
		ActiveCraftItemSdesc = "an in-progress $colour chin-tied riding turban craft",
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
			new CatalogueCommodityRecipeInput(Grams: 195.0, Material: "cotton", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("195 grams of cotton", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Sewing Needle", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Sewing Needle tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Shears", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Shears tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_southasian_chintied_ridingturban", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a $colour chin-tied riding turban (@renaissance_southasian_chintied_ridingturban)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition TextileAndLeatherWorkTailorCurvedToeLeatherShoes() => new()
	{
		Metadata = new("Textile and leather work/tailor curved-toe leather shoes", "crafts", ["renaissance"], ["item:renaissance_southasian_curvedtoe_shoes"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "tailor curved-toe leather shoes",
		Category = "Textile and leather work",
		Blurb = "tailor a pair of curved-toe leather shoes",
		Action = "tailoring a textile or leather item",
		ActiveCraftItemSdesc = "an in-progress curved-toe leather shoes craft",
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
			new CatalogueCommodityRecipeInput(Grams: 488.0, Material: "leather", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("488 grams of leather", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Awl Punch", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Awl Punch tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Shears", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Shears tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_southasian_curvedtoe_shoes", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a pair of curved-toe leather shoes (@renaissance_southasian_curvedtoe_shoes)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition TextileAndLeatherWorkTailorColourFanCrestedStructuredTurban() => new()
	{
		Metadata = new("Textile and leather work/tailor $colour fan-crested structured turban", "crafts", ["renaissance"], ["item:renaissance_southasian_fancrested_turban"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "tailor $colour fan-crested structured turban",
		Category = "Textile and leather work",
		Blurb = "tailor a $colour fan-crested structured turban",
		Action = "tailoring a textile or leather item",
		ActiveCraftItemSdesc = "an in-progress $colour fan-crested structured turban craft",
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
			new CatalogueCommodityRecipeInput(Grams: 195.0, Material: "silk", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("195 grams of silk", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Sewing Needle", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Sewing Needle tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Shears", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Shears tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_southasian_fancrested_turban", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a $colour fan-crested structured turban (@renaissance_southasian_fancrested_turban)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition TextileAndLeatherWorkTailorFittedColourCottonShortBodice() => new()
	{
		Metadata = new("Textile and leather work/tailor fitted $colour cotton short bodice", "crafts", ["renaissance"], ["item:renaissance_southasian_fitted_shortbodice"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "tailor fitted $colour cotton short bodice",
		Category = "Textile and leather work",
		Blurb = "tailor a fitted $colour cotton short bodice",
		Action = "tailoring a textile or leather item",
		ActiveCraftItemSdesc = "an in-progress fitted $colour cotton short bodice craft",
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
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_southasian_fitted_shortbodice", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a fitted $colour cotton short bodice (@renaissance_southasian_fitted_shortbodice)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition TextileAndLeatherWorkTailorColourFlaredSkirtLongTunic() => new()
	{
		Metadata = new("Textile and leather work/tailor $colour flared-skirt long tunic", "crafts", ["renaissance"], ["item:renaissance_southasian_flaredskirt_tunic"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "tailor $colour flared-skirt long tunic",
		Category = "Textile and leather work",
		Blurb = "tailor a $colour flared-skirt long tunic",
		Action = "tailoring a textile or leather item",
		ActiveCraftItemSdesc = "an in-progress $colour flared-skirt long tunic craft",
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
			new CatalogueCommodityRecipeInput(Grams: 735.0, Material: "cotton", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("735 grams of cotton", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Sewing Needle", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Sewing Needle tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Shears", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Shears tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_southasian_flaredskirt_tunic", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a $colour flared-skirt long tunic (@renaissance_southasian_flaredskirt_tunic)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition TextileAndLeatherWorkTailorBroadFlatWoundColourCottonTurban() => new()
	{
		Metadata = new("Textile and leather work/tailor broad flat-wound $colour cotton turban", "crafts", ["renaissance"], ["item:renaissance_southasian_flat_wound_turban"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "tailor broad flat-wound $colour cotton turban",
		Category = "Textile and leather work",
		Blurb = "tailor a broad flat-wound $colour cotton turban",
		Action = "tailoring a textile or leather item",
		ActiveCraftItemSdesc = "an in-progress broad flat-wound $colour cotton turban craft",
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
			new CatalogueCommodityRecipeInput(Grams: 165.0, Material: "cotton", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("165 grams of cotton", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Sewing Needle", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Sewing Needle tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Shears", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Shears tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_southasian_flat_wound_turban", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a broad flat-wound $colour cotton turban (@renaissance_southasian_flat_wound_turban)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition TextileAndLeatherWorkTailorFullGatheredColourCottonSkirt() => new()
	{
		Metadata = new("Textile and leather work/tailor full gathered $colour cotton skirt", "crafts", ["renaissance"], ["item:renaissance_southasian_gathered_longskirt"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "tailor full gathered $colour cotton skirt",
		Category = "Textile and leather work",
		Blurb = "tailor a full gathered $colour cotton skirt",
		Action = "tailoring a textile or leather item",
		ActiveCraftItemSdesc = "an in-progress full gathered $colour cotton skirt craft",
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
			new CatalogueCommodityRecipeInput(Grams: 473.0, Material: "cotton", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("473 grams of cotton", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Sewing Needle", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Sewing Needle tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Shears", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Shears tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_southasian_gathered_longskirt", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a full gathered $colour cotton skirt (@renaissance_southasian_gathered_longskirt)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition TextileAndLeatherWorkTailorColourHeadbandedLongCourtVeil() => new()
	{
		Metadata = new("Textile and leather work/tailor $colour headbanded long court veil", "crafts", ["renaissance"], ["item:renaissance_southasian_headbanded_courtveil"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "tailor $colour headbanded long court veil",
		Category = "Textile and leather work",
		Blurb = "tailor a $colour headbanded long court veil",
		Action = "tailoring a textile or leather item",
		ActiveCraftItemSdesc = "an in-progress $colour headbanded long court veil craft",
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
			new CatalogueCommodityRecipeInput(Grams: 135.0, Material: "silk", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("135 grams of silk", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Sewing Needle", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Sewing Needle tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Shears", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Shears tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_southasian_headbanded_courtveil", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a $colour headbanded long court veil (@renaissance_southasian_headbanded_courtveil)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition TextileAndLeatherWorkTailorColourHardWearingFieldSandals() => new()
	{
		Metadata = new("Textile and leather work/tailor $colour hard-wearing field sandals", "crafts", ["renaissance"], ["item:renaissance_southasian_leather_fieldsandals"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "tailor $colour hard-wearing field sandals",
		Category = "Textile and leather work",
		Blurb = "tailor a pair of $colour hard-wearing field sandals",
		Action = "tailoring a textile or leather item",
		ActiveCraftItemSdesc = "an in-progress $colour hard-wearing field sandals craft",
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
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_southasian_leather_fieldsandals", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a pair of $colour hard-wearing field sandals (@renaissance_southasian_leather_fieldsandals)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition TextileAndLeatherWorkTailorColourLongSleevedFittedBodice() => new()
	{
		Metadata = new("Textile and leather work/tailor $colour long-sleeved fitted bodice", "crafts", ["renaissance"], ["item:renaissance_southasian_longsleeve_bodice"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "tailor $colour long-sleeved fitted bodice",
		Category = "Textile and leather work",
		Blurb = "tailor a $colour long-sleeved fitted bodice",
		Action = "tailoring a textile or leather item",
		ActiveCraftItemSdesc = "an in-progress $colour long-sleeved fitted bodice craft",
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
			new CatalogueCommodityRecipeInput(Grams: 420.0, Material: "silk", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("420 grams of silk", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Sewing Needle", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Sewing Needle tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Shears", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Shears tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_southasian_longsleeve_bodice", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a $colour long-sleeved fitted bodice (@renaissance_southasian_longsleeve_bodice)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition TextileAndLeatherWorkTailorLongColourCottonCrossOverRobe() => new()
	{
		Metadata = new("Textile and leather work/tailor long $colour cotton cross-over robe", "crafts", ["renaissance"], ["item:renaissance_southasian_long_crossover_robe"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "tailor long $colour cotton cross-over robe",
		Category = "Textile and leather work",
		Blurb = "tailor a long $colour cotton cross-over robe",
		Action = "tailoring a textile or leather item",
		ActiveCraftItemSdesc = "an in-progress long $colour cotton cross-over robe craft",
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
			new CatalogueCommodityRecipeInput(Grams: 608.0, Material: "cotton", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("608 grams of cotton", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Sewing Needle", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Sewing Needle tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Shears", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Shears tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_southasian_long_crossover_robe", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a long $colour cotton cross-over robe (@renaissance_southasian_long_crossover_robe)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition TextileAndLeatherWorkTailorLongColourCottonSideSlitTunic() => new()
	{
		Metadata = new("Textile and leather work/tailor long $colour cotton side-slit tunic", "crafts", ["renaissance"], ["item:renaissance_southasian_long_sideslit_tunic"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "tailor long $colour cotton side-slit tunic",
		Category = "Textile and leather work",
		Blurb = "tailor a long $colour cotton side-slit tunic",
		Action = "tailoring a textile or leather item",
		ActiveCraftItemSdesc = "an in-progress long $colour cotton side-slit tunic craft",
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
			new CatalogueCommodityRecipeInput(Grams: 608.0, Material: "cotton", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("608 grams of cotton", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Sewing Needle", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Sewing Needle tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Shears", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Shears tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_southasian_long_sideslit_tunic", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a long $colour cotton side-slit tunic (@renaissance_southasian_long_sideslit_tunic)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition TextileAndLeatherWorkTailorPanelledFlaredColourSilkSkirt() => new()
	{
		Metadata = new("Textile and leather work/tailor panelled flared $colour silk skirt", "crafts", ["renaissance"], ["item:renaissance_southasian_panelled_longskirt"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "tailor panelled flared $colour silk skirt",
		Category = "Textile and leather work",
		Blurb = "tailor a panelled flared $colour silk skirt",
		Action = "tailoring a textile or leather item",
		ActiveCraftItemSdesc = "an in-progress panelled flared $colour silk skirt craft",
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
			new CatalogueCommodityRecipeInput(Grams: 345.0, Material: "silk", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("345 grams of silk", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Sewing Needle", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Sewing Needle tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Shears", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Shears tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_southasian_panelled_longskirt", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a panelled flared $colour silk skirt (@renaissance_southasian_panelled_longskirt)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition TextileAndLeatherWorkTailorLongPleatedColourCottonWaistcloth() => new()
	{
		Metadata = new("Textile and leather work/tailor long pleated $colour cotton waistcloth", "crafts", ["renaissance"], ["item:renaissance_southasian_pleated_waistcloth"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "tailor long pleated $colour cotton waistcloth",
		Category = "Textile and leather work",
		Blurb = "tailor a long pleated $colour cotton waistcloth",
		Action = "tailoring a textile or leather item",
		ActiveCraftItemSdesc = "an in-progress long pleated $colour cotton waistcloth craft",
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
			new CatalogueCommodityRecipeInput(Grams: 473.0, Material: "cotton", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("473 grams of cotton", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Sewing Needle", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Sewing Needle tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Shears", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Shears tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_southasian_pleated_waistcloth", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a long pleated $colour cotton waistcloth (@renaissance_southasian_pleated_waistcloth)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition TextileAndLeatherWorkTailorColourQuiltedCottonLongCoat() => new()
	{
		Metadata = new("Textile and leather work/tailor $colour quilted cotton long coat", "crafts", ["renaissance"], ["item:renaissance_southasian_quilted_cotton_longcoat"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "tailor $colour quilted cotton long coat",
		Category = "Textile and leather work",
		Blurb = "tailor a $colour quilted cotton long coat",
		Action = "tailoring a textile or leather item",
		ActiveCraftItemSdesc = "an in-progress $colour quilted cotton long coat craft",
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
			new CatalogueCommodityRecipeInput(Grams: 915.0, Material: "cotton", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("915 grams of cotton", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Sewing Needle", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Sewing Needle tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Shears", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Shears tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_southasian_quilted_cotton_longcoat", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a $colour quilted cotton long coat (@renaissance_southasian_quilted_cotton_longcoat)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition TextileAndLeatherWorkTailorShortColourCottonSideSlitTunic() => new()
	{
		Metadata = new("Textile and leather work/tailor short $colour cotton side-slit tunic", "crafts", ["renaissance"], ["item:renaissance_southasian_short_sideslit_tunic"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "tailor short $colour cotton side-slit tunic",
		Category = "Textile and leather work",
		Blurb = "tailor a short $colour cotton side-slit tunic",
		Action = "tailoring a textile or leather item",
		ActiveCraftItemSdesc = "an in-progress short $colour cotton side-slit tunic craft",
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
			new CatalogueCommodityRecipeInput(Grams: 225.0, Material: "cotton", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("225 grams of cotton", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Sewing Needle", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Sewing Needle tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Shears", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Shears tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_southasian_short_sideslit_tunic", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a short $colour cotton side-slit tunic (@renaissance_southasian_short_sideslit_tunic)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition TextileAndLeatherWorkTailorShortColourCottonCrossTiedJacket() => new()
	{
		Metadata = new("Textile and leather work/tailor short $colour cotton cross-tied jacket", "crafts", ["renaissance"], ["item:renaissance_southasian_short_wrapjacket"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "tailor short $colour cotton cross-tied jacket",
		Category = "Textile and leather work",
		Blurb = "tailor a short $colour cotton cross-tied jacket",
		Action = "tailoring a textile or leather item",
		ActiveCraftItemSdesc = "an in-progress short $colour cotton cross-tied jacket craft",
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
			new CatalogueCommodityRecipeInput(Grams: 435.0, Material: "cotton", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("435 grams of cotton", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Sewing Needle", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Sewing Needle tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Shears", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Shears tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_southasian_short_wrapjacket", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a short $colour cotton cross-tied jacket (@renaissance_southasian_short_wrapjacket)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition TextileAndLeatherWorkTailorFullColourCottonShoulderDrapedGarment() => new()
	{
		Metadata = new("Textile and leather work/tailor full $colour cotton shoulder-draped garment", "crafts", ["renaissance"], ["item:renaissance_southasian_shoulderdraped_garment"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "tailor full $colour cotton shoulder-draped garment",
		Category = "Textile and leather work",
		Blurb = "tailor a full $colour cotton shoulder-draped garment",
		Action = "tailoring a textile or leather item",
		ActiveCraftItemSdesc = "an in-progress full $colour cotton shoulder-draped garment craft",
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
			new CatalogueCommodityRecipeInput(Grams: 570.0, Material: "cotton", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("570 grams of cotton", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Sewing Needle", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Sewing Needle tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Shears", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Shears tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_southasian_shoulderdraped_garment", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a full $colour cotton shoulder-draped garment (@renaissance_southasian_shoulderdraped_garment)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition TextileAndLeatherWorkTailorColourSleevelessLongCourtOvercoat() => new()
	{
		Metadata = new("Textile and leather work/tailor $colour sleeveless long court overcoat", "crafts", ["renaissance"], ["item:renaissance_southasian_sleeveless_courtovercoat"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "tailor $colour sleeveless long court overcoat",
		Category = "Textile and leather work",
		Blurb = "tailor a $colour sleeveless long court overcoat",
		Action = "tailoring a textile or leather item",
		ActiveCraftItemSdesc = "an in-progress $colour sleeveless long court overcoat craft",
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
			new CatalogueCommodityRecipeInput(Grams: 915.0, Material: "silk", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("915 grams of silk", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Sewing Needle", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Sewing Needle tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Shears", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Shears tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_southasian_sleeveless_courtovercoat", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a $colour sleeveless long court overcoat (@renaissance_southasian_sleeveless_courtovercoat)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition TextileAndLeatherWorkTailorColourDividedRidingWaistcloth() => new()
	{
		Metadata = new("Textile and leather work/tailor $colour divided riding waistcloth", "crafts", ["renaissance"], ["item:renaissance_southasian_split_riding_waistcloth"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "tailor $colour divided riding waistcloth",
		Category = "Textile and leather work",
		Blurb = "tailor a $colour divided riding waistcloth",
		Action = "tailoring a textile or leather item",
		ActiveCraftItemSdesc = "an in-progress $colour divided riding waistcloth craft",
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
			new CatalogueCommodityRecipeInput(Grams: 465.0, Material: "cotton", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("465 grams of cotton", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Sewing Needle", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Sewing Needle tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Shears", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Shears tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_southasian_split_riding_waistcloth", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a $colour divided riding waistcloth (@renaissance_southasian_split_riding_waistcloth)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition TextileAndLeatherWorkTailorCloseColourCottonLoinDrawers() => new()
	{
		Metadata = new("Textile and leather work/tailor close $colour cotton loin drawers", "crafts", ["renaissance"], ["item:renaissance_southasian_stitched_loindrawers"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "tailor close $colour cotton loin drawers",
		Category = "Textile and leather work",
		Blurb = "tailor a pair of close $colour cotton loin drawers",
		Action = "tailoring a textile or leather item",
		ActiveCraftItemSdesc = "an in-progress close $colour cotton loin drawers craft",
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
			new CatalogueCommodityRecipeInput(Grams: 113.0, Material: "cotton", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("113 grams of cotton", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Sewing Needle", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Sewing Needle tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Shears", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Shears tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_southasian_stitched_loindrawers", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a pair of close $colour cotton loin drawers (@renaissance_southasian_stitched_loindrawers)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition CarpentryAndJoineryBuildColourToePostWoodenSandals() => new()
	{
		Metadata = new("Carpentry and joinery/build $colour toe-post wooden sandals", "crafts", ["renaissance"], ["item:renaissance_southasian_toepost_woodensandals"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "build $colour toe-post wooden sandals",
		Category = "Carpentry and joinery",
		Blurb = "build a pair of $colour toe-post wooden sandals",
		Action = "building a wooden item",
		ActiveCraftItemSdesc = "an in-progress $colour toe-post wooden sandals craft",
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
			new(45, "$0 fit|fits the joins and secure|secures them with $t1. $0 measure|measures the developing form against the intended finish.", "$0 fit|fits the joins and secure|secures them with $t1. $0 measure|measures the developing form against the intended finish.", ExertionLevel.Stasis, 0),
			new(30, "$0 smooth|smooths the finished piece and set|sets aside $p1.", "$0 find|finds the join has opened and set|sets the material aside.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 360.0, Material: "wood", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("360 grams of wood", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Saw", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Saw tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_southasian_toepost_woodensandals", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a pair of $colour toe-post wooden sandals (@renaissance_southasian_toepost_woodensandals)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition GeneralCraftworkMakeFurredEarFlapHat() => new()
	{
		Metadata = new("General craftwork/make furred ear-flap hat", "crafts", ["renaissance"], ["item:renaissance_steppe_fur_earflaphat"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make furred ear-flap hat",
		Category = "General craftwork",
		Blurb = "make a furred ear-flap hat",
		Action = "making a finished item",
		ActiveCraftItemSdesc = "an in-progress furred ear-flap hat craft",
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
			new CatalogueCommodityRecipeInput(Grams: 165.0, Material: "fur", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("165 grams of fur", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_steppe_fur_earflaphat", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a furred ear-flap hat (@renaissance_steppe_fur_earflaphat)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition TextileAndLeatherWorkTailorPointedColourFeltRidingCap() => new()
	{
		Metadata = new("Textile and leather work/tailor pointed $colour felt riding cap", "crafts", ["renaissance"], ["item:renaissance_steppe_pointed_feltcap"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "tailor pointed $colour felt riding cap",
		Category = "Textile and leather work",
		Blurb = "tailor a pointed $colour felt riding cap",
		Action = "tailoring a textile or leather item",
		ActiveCraftItemSdesc = "an in-progress pointed $colour felt riding cap craft",
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
			new CatalogueCommodityRecipeInput(Grams: 128.0, Material: "felt", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("128 grams of felt", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Sewing Needle", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Sewing Needle tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Shears", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Shears tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_steppe_pointed_feltcap", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a pointed $colour felt riding cap (@renaissance_steppe_pointed_feltcap)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition TextileAndLeatherWorkTailorQuiltedSideFastenedColourRidingRobe() => new()
	{
		Metadata = new("Textile and leather work/tailor quilted side-fastened $colour riding robe", "crafts", ["renaissance"], ["item:renaissance_steppe_quilted_ridingrobe"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "tailor quilted side-fastened $colour riding robe",
		Category = "Textile and leather work",
		Blurb = "tailor a quilted side-fastened $colour riding robe",
		Action = "tailoring a textile or leather item",
		ActiveCraftItemSdesc = "an in-progress quilted side-fastened $colour riding robe craft",
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
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_steppe_quilted_ridingrobe", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a quilted side-fastened $colour riding robe (@renaissance_steppe_quilted_ridingrobe)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition TextileAndLeatherWorkTailorShortSplitSkirtColourRidingCoat() => new()
	{
		Metadata = new("Textile and leather work/tailor short split-skirt $colour riding coat", "crafts", ["renaissance"], ["item:renaissance_steppe_short_split_ridingcoat"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "tailor short split-skirt $colour riding coat",
		Category = "Textile and leather work",
		Blurb = "tailor a short split-skirt $colour riding coat",
		Action = "tailoring a textile or leather item",
		ActiveCraftItemSdesc = "an in-progress short split-skirt $colour riding coat craft",
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
			new CatalogueCommodityRecipeInput(Grams: 540.0, Material: "wool", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("540 grams of wool", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Sewing Needle", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Sewing Needle tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Shears", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Shears tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_steppe_short_split_ridingcoat", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a short split-skirt $colour riding coat (@renaissance_steppe_short_split_ridingcoat)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition TextileAndLeatherWorkTailorSideFastenedColourFurLinedLongCoat() => new()
	{
		Metadata = new("Textile and leather work/tailor side-fastened $colour fur-lined long coat", "crafts", ["renaissance"], ["item:renaissance_steppe_sidefastened_furcoat"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "tailor side-fastened $colour fur-lined long coat",
		Category = "Textile and leather work",
		Blurb = "tailor a side-fastened $colour fur-lined long coat",
		Action = "tailoring a textile or leather item",
		ActiveCraftItemSdesc = "an in-progress side-fastened $colour fur-lined long coat craft",
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
			new CatalogueCommodityRecipeInput(Grams: 743.0, Material: "wool", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("743 grams of wool", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Sewing Needle", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Sewing Needle tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Shears", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Shears tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_steppe_sidefastened_furcoat", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a side-fastened $colour fur-lined long coat (@renaissance_steppe_sidefastened_furcoat)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition TextileAndLeatherWorkTailorSleevelessLeatherRidingVest() => new()
	{
		Metadata = new("Textile and leather work/tailor sleeveless leather riding vest", "crafts", ["renaissance"], ["item:renaissance_steppe_sleeveless_ridingvest"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "tailor sleeveless leather riding vest",
		Category = "Textile and leather work",
		Blurb = "tailor a sleeveless leather riding vest",
		Action = "tailoring a textile or leather item",
		ActiveCraftItemSdesc = "an in-progress sleeveless leather riding vest craft",
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
			new CatalogueCommodityRecipeInput(Grams: 338.0, Material: "leather", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("338 grams of leather", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Awl Punch", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Awl Punch tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Shears", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Shears tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_steppe_sleeveless_ridingvest", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a sleeveless leather riding vest (@renaissance_steppe_sleeveless_ridingvest)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition TextileAndLeatherWorkTailorSoftHighLeatherBootsPattern2() => new()
	{
		Metadata = new("Textile and leather work/tailor soft high leather boots pattern 2", "crafts", ["renaissance"], ["item:renaissance_steppe_soft_highboots"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "tailor soft high leather boots pattern 2",
		Category = "Textile and leather work",
		Blurb = "tailor a pair of soft high leather boots",
		Action = "tailoring a textile or leather item",
		ActiveCraftItemSdesc = "an in-progress soft high leather boots craft",
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
			new CatalogueCommodityRecipeInput(Grams: 915.0, Material: "leather", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("915 grams of leather", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Awl Punch", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Awl Punch tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Shears", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Shears tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_steppe_soft_highboots", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a pair of soft high leather boots (@renaissance_steppe_soft_highboots)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition TextileAndLeatherWorkTailorWideColourWoolRidingTrousers() => new()
	{
		Metadata = new("Textile and leather work/tailor wide $colour wool riding trousers", "crafts", ["renaissance"], ["item:renaissance_steppe_wide_ridingtrousers"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "tailor wide $colour wool riding trousers",
		Category = "Textile and leather work",
		Blurb = "tailor a pair of wide $colour wool riding trousers",
		Action = "tailoring a textile or leather item",
		ActiveCraftItemSdesc = "an in-progress wide $colour wool riding trousers craft",
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
			new CatalogueCommodityRecipeInput(Grams: 465.0, Material: "wool", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("465 grams of wool", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Sewing Needle", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Sewing Needle tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Shears", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Shears tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_steppe_wide_ridingtrousers", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a pair of wide $colour wool riding trousers (@renaissance_steppe_wide_ridingtrousers)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};
}
