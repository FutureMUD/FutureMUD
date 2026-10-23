#nullable enable

using MudSharp.Body;
using MudSharp.GameItems.Inventory.Plans;
using MudSharp.RPG.Checks;

namespace DatabaseSeeder.Seeders;

public static partial class Recipes18Stock
{
	public static CatalogueRecipeDefinition TextileAndLeatherWorkTailorColourShortCloseFittingSailorJacket() => new()
	{
		Metadata = new("Textile and leather work/tailor $colour short close-fitting sailor jacket", "crafts", ["renaissance"], ["item:renaissance_iberian_short_sailor_jacket"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "tailor $colour short close-fitting sailor jacket",
		Category = "Textile and leather work",
		Blurb = "tailor a $colour short close-fitting sailor jacket",
		Action = "tailoring a textile or leather item",
		ActiveCraftItemSdesc = "an in-progress $colour short close-fitting sailor jacket craft",
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
			new CatalogueCommodityRecipeInput(Grams: 570.0, Material: "wool", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("570 grams of wool", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Sewing Needle", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Sewing Needle tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Shears", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Shears tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_iberian_short_sailor_jacket", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a $colour short close-fitting sailor jacket (@renaissance_iberian_short_sailor_jacket)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition TextileAndLeatherWorkTailorCloseColourEmbroideredRoundCap() => new()
	{
		Metadata = new("Textile and leather work/tailor close $colour embroidered round cap", "crafts", ["renaissance"], ["item:renaissance_indianocean_embroidered_roundcap"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "tailor close $colour embroidered round cap",
		Category = "Textile and leather work",
		Blurb = "tailor a close $colour embroidered round cap",
		Action = "tailoring a textile or leather item",
		ActiveCraftItemSdesc = "an in-progress close $colour embroidered round cap craft",
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
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_indianocean_embroidered_roundcap", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a close $colour embroidered round cap (@renaissance_indianocean_embroidered_roundcap)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition TextileAndLeatherWorkTailorLongCollarlessColourCottonCoastalRobe() => new()
	{
		Metadata = new("Textile and leather work/tailor long collarless $colour cotton coastal robe", "crafts", ["renaissance"], ["item:renaissance_indianocean_long_coastalrobe"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "tailor long collarless $colour cotton coastal robe",
		Category = "Textile and leather work",
		Blurb = "tailor a long collarless $colour cotton coastal robe",
		Action = "tailoring a textile or leather item",
		ActiveCraftItemSdesc = "an in-progress long collarless $colour cotton coastal robe craft",
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
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_indianocean_long_coastalrobe", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a long collarless $colour cotton coastal robe (@renaissance_indianocean_long_coastalrobe)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition TextileAndLeatherWorkTailorLightOpenFrontColourCottonMerchantCoat() => new()
	{
		Metadata = new("Textile and leather work/tailor light open-front $colour cotton merchant coat", "crafts", ["renaissance"], ["item:renaissance_indianocean_open_merchantcoat"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "tailor light open-front $colour cotton merchant coat",
		Category = "Textile and leather work",
		Blurb = "tailor a light open-front $colour cotton merchant coat",
		Action = "tailoring a textile or leather item",
		ActiveCraftItemSdesc = "an in-progress light open-front $colour cotton merchant coat craft",
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
			new CatalogueCommodityRecipeInput(Grams: 758.0, Material: "cotton", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("758 grams of cotton", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Sewing Needle", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Sewing Needle tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Shears", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Shears tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_indianocean_open_merchantcoat", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a light open-front $colour cotton merchant coat (@renaissance_indianocean_open_merchantcoat)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition TextileAndLeatherWorkTailorColourCompactCoastalPortTurban() => new()
	{
		Metadata = new("Textile and leather work/tailor $colour compact coastal port turban", "crafts", ["renaissance"], ["item:renaissance_indianocean_port_turban"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "tailor $colour compact coastal port turban",
		Category = "Textile and leather work",
		Blurb = "tailor a $colour compact coastal port turban",
		Action = "tailoring a textile or leather item",
		ActiveCraftItemSdesc = "an in-progress $colour compact coastal port turban craft",
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
			new CatalogueCommodityRecipeInput(Grams: 195.0, Material: "cotton", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("195 grams of cotton", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Sewing Needle", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Sewing Needle tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Shears", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Shears tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_indianocean_port_turban", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a $colour compact coastal port turban (@renaissance_indianocean_port_turban)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition TextileAndLeatherWorkTailorSewnColourCottonCoastalWrapSkirt() => new()
	{
		Metadata = new("Textile and leather work/tailor sewn $colour cotton coastal wrap skirt", "crafts", ["renaissance"], ["item:renaissance_indianocean_sewn_wrapskirt"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "tailor sewn $colour cotton coastal wrap skirt",
		Category = "Textile and leather work",
		Blurb = "tailor a sewn $colour cotton coastal wrap skirt",
		Action = "tailoring a textile or leather item",
		ActiveCraftItemSdesc = "an in-progress sewn $colour cotton coastal wrap skirt craft",
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
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_indianocean_sewn_wrapskirt", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a sewn $colour cotton coastal wrap skirt (@renaissance_indianocean_sewn_wrapskirt)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition TextileAndLeatherWorkTailorShortLooseColourCottonCoastalTunic() => new()
	{
		Metadata = new("Textile and leather work/tailor short loose $colour cotton coastal tunic", "crafts", ["renaissance"], ["item:renaissance_indianocean_short_coastaltunic"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "tailor short loose $colour cotton coastal tunic",
		Category = "Textile and leather work",
		Blurb = "tailor a short loose $colour cotton coastal tunic",
		Action = "tailoring a textile or leather item",
		ActiveCraftItemSdesc = "an in-progress short loose $colour cotton coastal tunic craft",
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
			new CatalogueCommodityRecipeInput(Grams: 353.0, Material: "cotton", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("353 grams of cotton", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Sewing Needle", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Sewing Needle tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Shears", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Shears tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_indianocean_short_coastaltunic", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a short loose $colour cotton coastal tunic (@renaissance_indianocean_short_coastaltunic)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition TextileAndLeatherWorkTailorColourStitchedCoastalSlippers() => new()
	{
		Metadata = new("Textile and leather work/tailor $colour stitched coastal slippers", "crafts", ["renaissance"], ["item:renaissance_indianocean_stitched_coastal_slippers"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "tailor $colour stitched coastal slippers",
		Category = "Textile and leather work",
		Blurb = "tailor a pair of $colour stitched coastal slippers",
		Action = "tailoring a textile or leather item",
		ActiveCraftItemSdesc = "an in-progress $colour stitched coastal slippers craft",
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
			new CatalogueCommodityRecipeInput(Grams: 518.0, Material: "leather", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("518 grams of leather", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Awl Punch", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Awl Punch tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Shears", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Shears tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_indianocean_stitched_coastal_slippers", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a pair of $colour stitched coastal slippers (@renaissance_indianocean_stitched_coastal_slippers)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition TextileAndLeatherWorkTailorToeLoopLeatherSandals() => new()
	{
		Metadata = new("Textile and leather work/tailor toe-loop leather sandals", "crafts", ["renaissance"], ["item:renaissance_indianocean_toeloop_sandals"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "tailor toe-loop leather sandals",
		Category = "Textile and leather work",
		Blurb = "tailor a pair of toe-loop leather sandals",
		Action = "tailoring a textile or leather item",
		ActiveCraftItemSdesc = "an in-progress toe-loop leather sandals craft",
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
			new CatalogueCommodityRecipeInput(Grams: 308.0, Material: "leather", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("308 grams of leather", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Awl Punch", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Awl Punch tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Shears", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Shears tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_indianocean_toeloop_sandals", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a pair of toe-loop leather sandals (@renaissance_indianocean_toeloop_sandals)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition TextileAndLeatherWorkTailorDiagonallyTiedColourLongCoat() => new()
	{
		Metadata = new("Textile and leather work/tailor diagonally tied $colour long coat", "crafts", ["renaissance"], ["item:renaissance_indopersian_diagonal_tie_jama"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "tailor diagonally tied $colour long coat",
		Category = "Textile and leather work",
		Blurb = "tailor a diagonally tied $colour long coat",
		Action = "tailoring a textile or leather item",
		ActiveCraftItemSdesc = "an in-progress diagonally tied $colour long coat craft",
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
			new CatalogueCommodityRecipeInput(Grams: 608.0, Material: "cotton", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("608 grams of cotton", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Sewing Needle", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Sewing Needle tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Shears", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Shears tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_indopersian_diagonal_tie_jama", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a diagonally tied $colour long coat (@renaissance_indopersian_diagonal_tie_jama)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition TextileAndLeatherWorkTailorColourFullSkirtedSideFastenedCoat() => new()
	{
		Metadata = new("Textile and leather work/tailor $colour full-skirted side-fastened coat", "crafts", ["renaissance"], ["item:renaissance_indopersian_fullskirt_jama"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "tailor $colour full-skirted side-fastened coat",
		Category = "Textile and leather work",
		Blurb = "tailor a $colour full-skirted side-fastened coat",
		Action = "tailoring a textile or leather item",
		ActiveCraftItemSdesc = "an in-progress $colour full-skirted side-fastened coat craft",
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
			new CatalogueCommodityRecipeInput(Grams: 735.0, Material: "silk", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("735 grams of silk", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Sewing Needle", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Sewing Needle tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Shears", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Shears tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_indopersian_fullskirt_jama", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a $colour full-skirted side-fastened coat (@renaissance_indopersian_fullskirt_jama)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition TextileAndLeatherWorkTailorSideFastenedColourFlaredLongCoat() => new()
	{
		Metadata = new("Textile and leather work/tailor side-fastened $colour flared long coat", "crafts", ["renaissance"], ["item:renaissance_indopersian_sidefastened_jama"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "tailor side-fastened $colour flared long coat",
		Category = "Textile and leather work",
		Blurb = "tailor a side-fastened $colour flared long coat",
		Action = "tailoring a textile or leather item",
		ActiveCraftItemSdesc = "an in-progress side-fastened $colour flared long coat craft",
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
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_indopersian_sidefastened_jama", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a side-fastened $colour flared long coat (@renaissance_indopersian_sidefastened_jama)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition GeneralCraftworkMakeLongColourAcademicRobe() => new()
	{
		Metadata = new("General craftwork/make long $colour academic robe", "crafts", ["renaissance"], ["item:renaissance_institution_academic_robe"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make long $colour academic robe",
		Category = "General craftwork",
		Blurb = "make a long $colour academic robe",
		Action = "making a finished item",
		ActiveCraftItemSdesc = "an in-progress long $colour academic robe craft",
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
			new CatalogueCommodityRecipeInput(Grams: 900.0, Material: "broadcloth", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("900 grams of broadcloth", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_institution_academic_robe", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a long $colour academic robe (@renaissance_institution_academic_robe)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition GeneralCraftworkMakeColourStructuredAcademicSquareCap() => new()
	{
		Metadata = new("General craftwork/make $colour structured academic square cap", "crafts", ["renaissance"], ["item:renaissance_institution_academic_squarecap"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make $colour structured academic square cap",
		Category = "General craftwork",
		Blurb = "make a $colour structured academic square cap",
		Action = "making a finished item",
		ActiveCraftItemSdesc = "an in-progress $colour structured academic square cap craft",
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
			new CatalogueCommodityRecipeInput(Grams: 143.0, Material: "broadcloth", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("143 grams of broadcloth", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_institution_academic_squarecap", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a $colour structured academic square cap (@renaissance_institution_academic_squarecap)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition TextileAndLeatherWorkTailorColourSleevelessLiturgicalOverVestment() => new()
	{
		Metadata = new("Textile and leather work/tailor $colour sleeveless liturgical over-vestment", "crafts", ["renaissance"], ["item:renaissance_institution_chasuble"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "tailor $colour sleeveless liturgical over-vestment",
		Category = "Textile and leather work",
		Blurb = "tailor a $colour sleeveless liturgical over-vestment",
		Action = "tailoring a textile or leather item",
		ActiveCraftItemSdesc = "an in-progress $colour sleeveless liturgical over-vestment craft",
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
			new CatalogueCommodityRecipeInput(Grams: 540.0, Material: "silk", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("540 grams of silk", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Sewing Needle", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Sewing Needle tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Shears", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Shears tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_institution_chasuble", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a $colour sleeveless liturgical over-vestment (@renaissance_institution_chasuble)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition TextileAndLeatherWorkTailorColourClosedMonasticSandals() => new()
	{
		Metadata = new("Textile and leather work/tailor $colour closed monastic sandals", "crafts", ["renaissance"], ["item:renaissance_institution_closed_monastic_sandals"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "tailor $colour closed monastic sandals",
		Category = "Textile and leather work",
		Blurb = "tailor a pair of $colour closed monastic sandals",
		Action = "tailoring a textile or leather item",
		ActiveCraftItemSdesc = "an in-progress $colour closed monastic sandals craft",
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
			new CatalogueCommodityRecipeInput(Grams: 315.0, Material: "leather", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("315 grams of leather", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Awl Punch", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Awl Punch tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Shears", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Shears tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_institution_closed_monastic_sandals", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a pair of $colour closed monastic sandals (@renaissance_institution_closed_monastic_sandals)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition TextileAndLeatherWorkTailorHoodedColourWoolCowl() => new()
	{
		Metadata = new("Textile and leather work/tailor hooded $colour wool cowl", "crafts", ["renaissance"], ["item:renaissance_institution_full_cowl"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "tailor hooded $colour wool cowl",
		Category = "Textile and leather work",
		Blurb = "tailor a hooded $colour wool cowl",
		Action = "tailoring a textile or leather item",
		ActiveCraftItemSdesc = "an in-progress hooded $colour wool cowl craft",
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
			new CatalogueCommodityRecipeInput(Grams: 675.0, Material: "wool", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("675 grams of wool", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Sewing Needle", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Sewing Needle tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Shears", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Shears tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_institution_full_cowl", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a hooded $colour wool cowl (@renaissance_institution_full_cowl)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition TextileAndLeatherWorkTailorLooseColourLinenSurplice() => new()
	{
		Metadata = new("Textile and leather work/tailor loose $colour linen surplice", "crafts", ["renaissance"], ["item:renaissance_institution_linen_surplus"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "tailor loose $colour linen surplice",
		Category = "Textile and leather work",
		Blurb = "tailor a loose $colour linen surplice",
		Action = "tailoring a textile or leather item",
		ActiveCraftItemSdesc = "an in-progress loose $colour linen surplice craft",
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
			new CatalogueCommodityRecipeInput(Grams: 488.0, Material: "linen", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("488 grams of linen", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Sewing Needle", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Sewing Needle tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Shears", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Shears tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_institution_linen_surplus", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a loose $colour linen surplice (@renaissance_institution_linen_surplus)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition TextileAndLeatherWorkTailorLongColourLinenAlb() => new()
	{
		Metadata = new("Textile and leather work/tailor long $colour linen alb", "crafts", ["renaissance"], ["item:renaissance_institution_liturgical_alb"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "tailor long $colour linen alb",
		Category = "Textile and leather work",
		Blurb = "tailor a long $colour linen alb",
		Action = "tailoring a textile or leather item",
		ActiveCraftItemSdesc = "an in-progress long $colour linen alb craft",
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
			new CatalogueCommodityRecipeInput(Grams: 735.0, Material: "linen", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("735 grams of linen", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Sewing Needle", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Sewing Needle tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Shears", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Shears tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_institution_liturgical_alb", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a long $colour linen alb (@renaissance_institution_liturgical_alb)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition TextileAndLeatherWorkTailorColourLongBeltedMonasticHabit() => new()
	{
		Metadata = new("Textile and leather work/tailor $colour long belted monastic habit", "crafts", ["renaissance"], ["item:renaissance_institution_monastic_habit"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "tailor $colour long belted monastic habit",
		Category = "Textile and leather work",
		Blurb = "tailor a $colour long belted monastic habit",
		Action = "tailoring a textile or leather item",
		ActiveCraftItemSdesc = "an in-progress $colour long belted monastic habit craft",
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
			new CatalogueCommodityRecipeInput(Grams: 735.0, Material: "wool", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("735 grams of wool", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Sewing Needle", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Sewing Needle tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Shears", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Shears tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_institution_monastic_habit", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a $colour long belted monastic habit (@renaissance_institution_monastic_habit)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition TextileAndLeatherWorkTailorLongColourWoolScapular() => new()
	{
		Metadata = new("Textile and leather work/tailor long $colour wool scapular", "crafts", ["renaissance"], ["item:renaissance_institution_monastic_scapular"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "tailor long $colour wool scapular",
		Category = "Textile and leather work",
		Blurb = "tailor a long $colour wool scapular",
		Action = "tailoring a textile or leather item",
		ActiveCraftItemSdesc = "an in-progress long $colour wool scapular craft",
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
			new CatalogueCommodityRecipeInput(Grams: 525.0, Material: "wool", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("525 grams of wool", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Sewing Needle", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Sewing Needle tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Shears", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Shears tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_institution_monastic_scapular", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a long $colour wool scapular (@renaissance_institution_monastic_scapular)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition GeneralCraftworkMakeColourFullRoundPhysicianCap() => new()
	{
		Metadata = new("General craftwork/make $colour full round physician cap", "crafts", ["renaissance"], ["item:renaissance_institution_physicians_roundcap"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make $colour full round physician cap",
		Category = "General craftwork",
		Blurb = "make a $colour full round physician cap",
		Action = "making a finished item",
		ActiveCraftItemSdesc = "an in-progress $colour full round physician cap craft",
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
			new CatalogueCommodityRecipeInput(Grams: 143.0, Material: "velvet", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("143 grams of velvet", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_institution_physicians_roundcap", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a $colour full round physician cap (@renaissance_institution_physicians_roundcap)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition GeneralCraftworkMakeLongColourCloseButtonedCassock() => new()
	{
		Metadata = new("General craftwork/make long $colour close-buttoned cassock", "crafts", ["renaissance"], ["item:renaissance_institution_plain_cassock"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make long $colour close-buttoned cassock",
		Category = "General craftwork",
		Blurb = "make a long $colour close-buttoned cassock",
		Action = "making a finished item",
		ActiveCraftItemSdesc = "an in-progress long $colour close-buttoned cassock craft",
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
			new CatalogueCommodityRecipeInput(Grams: 788.0, Material: "broadcloth", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("788 grams of broadcloth", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_institution_plain_cassock", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a long $colour close-buttoned cassock (@renaissance_institution_plain_cassock)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition GeneralCraftworkMakeFullSleevedPreachingGown() => new()
	{
		Metadata = new("General craftwork/make full-sleeved preaching gown", "crafts", ["renaissance"], ["item:renaissance_institution_preaching_gown"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make full-sleeved preaching gown",
		Category = "General craftwork",
		Blurb = "make a full-sleeved preaching gown",
		Action = "making a finished item",
		ActiveCraftItemSdesc = "an in-progress full-sleeved preaching gown craft",
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
			new CatalogueCommodityRecipeInput(Grams: 900.0, Material: "broadcloth", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("900 grams of broadcloth", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_institution_preaching_gown", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a full-sleeved preaching gown (@renaissance_institution_preaching_gown)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition TextileAndLeatherWorkTailorColourLongClaspedCeremonialCope() => new()
	{
		Metadata = new("Textile and leather work/tailor $colour long clasped ceremonial cope", "crafts", ["renaissance"], ["item:renaissance_institution_processional_cope"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "tailor $colour long clasped ceremonial cope",
		Category = "Textile and leather work",
		Blurb = "tailor a $colour long clasped ceremonial cope",
		Action = "tailoring a textile or leather item",
		ActiveCraftItemSdesc = "an in-progress $colour long clasped ceremonial cope craft",
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
			new CatalogueCommodityRecipeInput(Grams: 788.0, Material: "silk", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("788 grams of silk", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Sewing Needle", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Sewing Needle tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Shears", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Shears tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_institution_processional_cope", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a $colour long clasped ceremonial cope (@renaissance_institution_processional_cope)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition TextileAndLeatherWorkTailorColourBroadSleevedFormalRobe() => new()
	{
		Metadata = new("Textile and leather work/tailor $colour broad-sleeved formal robe", "crafts", ["renaissance"], ["item:renaissance_japanese_broadsleeve_formalrobe"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "tailor $colour broad-sleeved formal robe",
		Category = "Textile and leather work",
		Blurb = "tailor a $colour broad-sleeved formal robe",
		Action = "tailoring a textile or leather item",
		ActiveCraftItemSdesc = "an in-progress $colour broad-sleeved formal robe craft",
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
			new CatalogueCommodityRecipeInput(Grams: 735.0, Material: "silk", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("735 grams of silk", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Sewing Needle", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Sewing Needle tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Shears", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Shears tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_japanese_broadsleeve_formalrobe", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a $colour broad-sleeved formal robe (@renaissance_japanese_broadsleeve_formalrobe)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition GeneralCraftworkMakeColourBroadSedgeTravelHat() => new()
	{
		Metadata = new("General craftwork/make $colour broad sedge travel hat", "crafts", ["renaissance"], ["item:renaissance_japanese_broad_sedge_travelhat"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make $colour broad sedge travel hat",
		Category = "General craftwork",
		Blurb = "make a $colour broad sedge travel hat",
		Action = "making a finished item",
		ActiveCraftItemSdesc = "an in-progress $colour broad sedge travel hat craft",
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
			new CatalogueCommodityRecipeInput(Grams: 143.0, Material: "straw", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("143 grams of straw", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_japanese_broad_sedge_travelhat", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a $colour broad sedge travel hat (@renaissance_japanese_broad_sedge_travelhat)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition TextileAndLeatherWorkTailorColourDividedPleatedRidingTrousers() => new()
	{
		Metadata = new("Textile and leather work/tailor $colour divided pleated riding trousers", "crafts", ["renaissance"], ["item:renaissance_japanese_divided_riding_hakama"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "tailor $colour divided pleated riding trousers",
		Category = "Textile and leather work",
		Blurb = "tailor a $colour divided pleated riding trousers",
		Action = "tailoring a textile or leather item",
		ActiveCraftItemSdesc = "an in-progress $colour divided pleated riding trousers craft",
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
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_japanese_divided_riding_hakama", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a $colour divided pleated riding trousers (@renaissance_japanese_divided_riding_hakama)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition TextileAndLeatherWorkTailorCloseColourRamieFieldTrousers() => new()
	{
		Metadata = new("Textile and leather work/tailor close $colour ramie field trousers", "crafts", ["renaissance"], ["item:renaissance_japanese_field_trousers"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "tailor close $colour ramie field trousers",
		Category = "Textile and leather work",
		Blurb = "tailor a pair of close $colour ramie field trousers",
		Action = "tailoring a textile or leather item",
		ActiveCraftItemSdesc = "an in-progress close $colour ramie field trousers craft",
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
			new CatalogueCommodityRecipeInput(Grams: 338.0, Material: "ramie cloth", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("338 grams of ramie cloth", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Sewing Needle", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Sewing Needle tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Shears", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Shears tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_japanese_field_trousers", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a pair of close $colour ramie field trousers (@renaissance_japanese_field_trousers)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition TextileAndLeatherWorkTailorFullPleatedColourLowerGarment() => new()
	{
		Metadata = new("Textile and leather work/tailor full pleated $colour lower garment", "crafts", ["renaissance"], ["item:renaissance_japanese_full_pleated_hakama"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "tailor full pleated $colour lower garment",
		Category = "Textile and leather work",
		Blurb = "tailor a full pleated $colour lower garment",
		Action = "tailoring a textile or leather item",
		ActiveCraftItemSdesc = "an in-progress full pleated $colour lower garment craft",
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
			new CatalogueCommodityRecipeInput(Grams: 270.0, Material: "silk", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("270 grams of silk", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Sewing Needle", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Sewing Needle tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Shears", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Shears tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_japanese_full_pleated_hakama", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a full pleated $colour lower garment (@renaissance_japanese_full_pleated_hakama)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition TextileAndLeatherWorkTailorColourHeavyLinedOuterRobe() => new()
	{
		Metadata = new("Textile and leather work/tailor $colour heavy lined outer robe", "crafts", ["renaissance"], ["item:renaissance_japanese_heavy_outerrobe"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "tailor $colour heavy lined outer robe",
		Category = "Textile and leather work",
		Blurb = "tailor a $colour heavy lined outer robe",
		Action = "tailoring a textile or leather item",
		ActiveCraftItemSdesc = "an in-progress $colour heavy lined outer robe craft",
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
			new CatalogueCommodityRecipeInput(Grams: 915.0, Material: "silk", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("915 grams of silk", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Sewing Needle", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Sewing Needle tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Shears", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Shears tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_japanese_heavy_outerrobe", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a $colour heavy lined outer robe (@renaissance_japanese_heavy_outerrobe)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition CarpentryAndJoineryBuildLacqueredWoodenConicalHat() => new()
	{
		Metadata = new("Carpentry and joinery/build lacquered wooden conical hat", "crafts", ["renaissance"], ["item:renaissance_japanese_lacquered_conicalhat"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "build lacquered wooden conical hat",
		Category = "Carpentry and joinery",
		Blurb = "build a lacquered wooden conical hat",
		Action = "building a wooden item",
		ActiveCraftItemSdesc = "an in-progress lacquered wooden conical hat craft",
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
			new CatalogueCommodityRecipeInput(Grams: 184.0, Material: "wood", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("184 grams of wood", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Saw", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Saw tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_japanese_lacquered_conicalhat", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a lacquered wooden conical hat (@renaissance_japanese_lacquered_conicalhat)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition TextileAndLeatherWorkTailorColourQuiltedSleevelessVest() => new()
	{
		Metadata = new("Textile and leather work/tailor $colour quilted sleeveless vest", "crafts", ["renaissance"], ["item:renaissance_japanese_quilted_sleevelessvest"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "tailor $colour quilted sleeveless vest",
		Category = "Textile and leather work",
		Blurb = "tailor a $colour quilted sleeveless vest",
		Action = "tailoring a textile or leather item",
		ActiveCraftItemSdesc = "an in-progress $colour quilted sleeveless vest craft",
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
			new CatalogueCommodityRecipeInput(Grams: 420.0, Material: "cotton", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("420 grams of cotton", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Sewing Needle", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Sewing Needle tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Shears", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Shears tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_japanese_quilted_sleevelessvest", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a $colour quilted sleeveless vest (@renaissance_japanese_quilted_sleevelessvest)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition TextileAndLeatherWorkTailorShortOpenFrontColourJacket() => new()
	{
		Metadata = new("Textile and leather work/tailor short open-front $colour jacket", "crafts", ["renaissance"], ["item:renaissance_japanese_short_openjacket"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "tailor short open-front $colour jacket",
		Category = "Textile and leather work",
		Blurb = "tailor a short open-front $colour jacket",
		Action = "tailoring a textile or leather item",
		ActiveCraftItemSdesc = "an in-progress short open-front $colour jacket craft",
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
			new CatalogueCommodityRecipeInput(Grams: 315.0, Material: "silk", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("315 grams of silk", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Sewing Needle", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Sewing Needle tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Shears", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Shears tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_japanese_short_openjacket", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a short open-front $colour jacket (@renaissance_japanese_short_openjacket)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition GeneralCraftworkMakeColourStiffShoulderWingVest() => new()
	{
		Metadata = new("General craftwork/make $colour stiff shoulder-wing vest", "crafts", ["renaissance"], ["item:renaissance_japanese_shoulderwing_vest"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make $colour stiff shoulder-wing vest",
		Category = "General craftwork",
		Blurb = "make a $colour stiff shoulder-wing vest",
		Action = "making a finished item",
		ActiveCraftItemSdesc = "an in-progress $colour stiff shoulder-wing vest craft",
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
			new CatalogueCommodityRecipeInput(Grams: 420.0, Material: "broadcloth", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("420 grams of broadcloth", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_japanese_shoulderwing_vest", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a $colour stiff shoulder-wing vest (@renaissance_japanese_shoulderwing_vest)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition TextileAndLeatherWorkTailorSmallSleevedColourWrapRobe() => new()
	{
		Metadata = new("Textile and leather work/tailor small-sleeved $colour wrap robe", "crafts", ["renaissance"], ["item:renaissance_japanese_smallsleeve_wraprobe"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "tailor small-sleeved $colour wrap robe",
		Category = "Textile and leather work",
		Blurb = "tailor a small-sleeved $colour wrap robe",
		Action = "tailoring a textile or leather item",
		ActiveCraftItemSdesc = "an in-progress small-sleeved $colour wrap robe craft",
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
			new CatalogueCommodityRecipeInput(Grams: 435.0, Material: "silk", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("435 grams of silk", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Sewing Needle", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Sewing Needle tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Shears", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Shears tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_japanese_smallsleeve_wraprobe", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a small-sleeved $colour wrap robe (@renaissance_japanese_smallsleeve_wraprobe)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition TextileAndLeatherWorkTailorColourTallFoldedSoftCap() => new()
	{
		Metadata = new("Textile and leather work/tailor $colour tall folded soft cap", "crafts", ["renaissance"], ["item:renaissance_japanese_soft_folded_courtcap"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "tailor $colour tall folded soft cap",
		Category = "Textile and leather work",
		Blurb = "tailor a $colour tall folded soft cap",
		Action = "tailoring a textile or leather item",
		ActiveCraftItemSdesc = "an in-progress $colour tall folded soft cap craft",
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
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_japanese_soft_folded_courtcap", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a $colour tall folded soft cap (@renaissance_japanese_soft_folded_courtcap)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition TextileAndLeatherWorkTailorColourSplitToeSocks() => new()
	{
		Metadata = new("Textile and leather work/tailor $colour split-toe socks", "crafts", ["renaissance"], ["item:renaissance_japanese_splittoe_socks"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "tailor $colour split-toe socks",
		Category = "Textile and leather work",
		Blurb = "tailor a pair of $colour split-toe socks",
		Action = "tailoring a textile or leather item",
		ActiveCraftItemSdesc = "an in-progress $colour split-toe socks craft",
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
			new CatalogueCommodityRecipeInput(Grams: 75.0, Material: "cotton", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("75 grams of cotton", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Sewing Needle", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Sewing Needle tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Shears", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Shears tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_japanese_splittoe_socks", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a pair of $colour split-toe socks (@renaissance_japanese_splittoe_socks)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition TextileAndLeatherWorkTailorColourStiffLacqueredFoldedCap() => new()
	{
		Metadata = new("Textile and leather work/tailor $colour stiff lacquered folded cap", "crafts", ["renaissance"], ["item:renaissance_japanese_stiff_lacquered_eboshi"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "tailor $colour stiff lacquered folded cap",
		Category = "Textile and leather work",
		Blurb = "tailor a $colour stiff lacquered folded cap",
		Action = "tailoring a textile or leather item",
		ActiveCraftItemSdesc = "an in-progress $colour stiff lacquered folded cap craft",
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
			new CatalogueCommodityRecipeInput(Grams: 143.0, Material: "silk", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("143 grams of silk", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Sewing Needle", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Sewing Needle tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Shears", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Shears tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_japanese_stiff_lacquered_eboshi", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a $colour stiff lacquered folded cap (@renaissance_japanese_stiff_lacquered_eboshi)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition GeneralCraftworkMakeColourLayeredStrawRainCape() => new()
	{
		Metadata = new("General craftwork/make $colour layered straw rain cape", "crafts", ["renaissance"], ["item:renaissance_japanese_straw_raincape"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make $colour layered straw rain cape",
		Category = "General craftwork",
		Blurb = "make a $colour layered straw rain cape",
		Action = "making a finished item",
		ActiveCraftItemSdesc = "an in-progress $colour layered straw rain cape craft",
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
			new CatalogueCommodityRecipeInput(Grams: 788.0, Material: "straw", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("788 grams of straw", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_japanese_straw_raincape", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a $colour layered straw rain cape (@renaissance_japanese_straw_raincape)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition GeneralCraftworkMakeColourTightlyBraidedStrawRoadSandals() => new()
	{
		Metadata = new("General craftwork/make $colour tightly braided straw road sandals", "crafts", ["renaissance"], ["item:renaissance_japanese_tied_straw_travelsandals"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make $colour tightly braided straw road sandals",
		Category = "General craftwork",
		Blurb = "make a pair of $colour tightly braided straw road sandals",
		Action = "making a finished item",
		ActiveCraftItemSdesc = "an in-progress $colour tightly braided straw road sandals craft",
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
			new CatalogueCommodityRecipeInput(Grams: 315.0, Material: "straw", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("315 grams of straw", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_japanese_tied_straw_travelsandals", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a pair of $colour tightly braided straw road sandals (@renaissance_japanese_tied_straw_travelsandals)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition TextileAndLeatherWorkTailorUnlinedColourRamieSummerRobe() => new()
	{
		Metadata = new("Textile and leather work/tailor unlined $colour ramie summer robe", "crafts", ["renaissance"], ["item:renaissance_japanese_unlined_summerrobe"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "tailor unlined $colour ramie summer robe",
		Category = "Textile and leather work",
		Blurb = "tailor an unlined $colour ramie summer robe",
		Action = "tailoring a textile or leather item",
		ActiveCraftItemSdesc = "an in-progress unlined $colour ramie summer robe craft",
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
			new CatalogueCommodityRecipeInput(Grams: 540.0, Material: "ramie cloth", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("540 grams of ramie cloth", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Sewing Needle", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Sewing Needle tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Shears", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Shears tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_japanese_unlined_summerrobe", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x an unlined $colour ramie summer robe (@renaissance_japanese_unlined_summerrobe)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition CarpentryAndJoineryBuildRaisedColourWoodenClogs() => new()
	{
		Metadata = new("Carpentry and joinery/build raised $colour wooden clogs", "crafts", ["renaissance"], ["item:renaissance_japanese_wooden_clogs"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "build raised $colour wooden clogs",
		Category = "Carpentry and joinery",
		Blurb = "build a pair of raised $colour wooden clogs",
		Action = "building a wooden item",
		ActiveCraftItemSdesc = "an in-progress raised $colour wooden clogs craft",
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
			new CatalogueCommodityRecipeInput(Grams: 560.0, Material: "wood", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("560 grams of wood", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Saw", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Saw tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_japanese_wooden_clogs", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a pair of raised $colour wooden clogs (@renaissance_japanese_wooden_clogs)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition GeneralCraftworkMakeColourLongWovenStrawCoat() => new()
	{
		Metadata = new("General craftwork/make $colour long woven straw coat", "crafts", ["renaissance"], ["item:renaissance_japanese_woven_strawcoat"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make $colour long woven straw coat",
		Category = "General craftwork",
		Blurb = "make a $colour long woven straw coat",
		Action = "making a finished item",
		ActiveCraftItemSdesc = "an in-progress $colour long woven straw coat craft",
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
			new CatalogueCommodityRecipeInput(Grams: 915.0, Material: "straw", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("915 grams of straw", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_japanese_woven_strawcoat", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a $colour long woven straw coat (@renaissance_japanese_woven_strawcoat)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition TextileAndLeatherWorkTailorBroadSleevedColourScholarOverRobe() => new()
	{
		Metadata = new("Textile and leather work/tailor broad-sleeved $colour scholar over-robe", "crafts", ["renaissance"], ["item:renaissance_joseon_broadsleeve_scholaroverrobe"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "tailor broad-sleeved $colour scholar over-robe",
		Category = "Textile and leather work",
		Blurb = "tailor a broad-sleeved $colour scholar over-robe",
		Action = "tailoring a textile or leather item",
		ActiveCraftItemSdesc = "an in-progress broad-sleeved $colour scholar over-robe craft",
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
			new CatalogueCommodityRecipeInput(Grams: 548.0, Material: "silk", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("548 grams of silk", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Sewing Needle", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Sewing Needle tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Shears", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Shears tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_joseon_broadsleeve_scholaroverrobe", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a broad-sleeved $colour scholar over-robe (@renaissance_joseon_broadsleeve_scholaroverrobe)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition TextileAndLeatherWorkTailorFullGatheredColourWrapSkirt() => new()
	{
		Metadata = new("Textile and leather work/tailor full gathered $colour wrap skirt", "crafts", ["renaissance"], ["item:renaissance_joseon_full_gathered_wrapskirt"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "tailor full gathered $colour wrap skirt",
		Category = "Textile and leather work",
		Blurb = "tailor a full gathered $colour wrap skirt",
		Action = "tailoring a textile or leather item",
		ActiveCraftItemSdesc = "an in-progress full gathered $colour wrap skirt craft",
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
			new CatalogueCommodityRecipeInput(Grams: 345.0, Material: "silk", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("345 grams of silk", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Sewing Needle", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Sewing Needle tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Shears", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Shears tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_joseon_full_gathered_wrapskirt", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a full gathered $colour wrap skirt (@renaissance_joseon_full_gathered_wrapskirt)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition TextileAndLeatherWorkTailorLongCrossFrontColourSilkJacket() => new()
	{
		Metadata = new("Textile and leather work/tailor long cross-front $colour silk jacket", "crafts", ["renaissance"], ["item:renaissance_joseon_long_crossfront_jacket"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "tailor long cross-front $colour silk jacket",
		Category = "Textile and leather work",
		Blurb = "tailor a long cross-front $colour silk jacket",
		Action = "tailoring a textile or leather item",
		ActiveCraftItemSdesc = "an in-progress long cross-front $colour silk jacket craft",
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
			new CatalogueCommodityRecipeInput(Grams: 315.0, Material: "silk", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("315 grams of silk", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Sewing Needle", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Sewing Needle tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Shears", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Shears tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_joseon_long_crossfront_jacket", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a long cross-front $colour silk jacket (@renaissance_joseon_long_crossfront_jacket)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition TextileAndLeatherWorkTailorLongCrossFrontColourRamieUnderRobe() => new()
	{
		Metadata = new("Textile and leather work/tailor long cross-front $colour ramie under-robe", "crafts", ["renaissance"], ["item:renaissance_joseon_long_crossfront_underrobe"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "tailor long cross-front $colour ramie under-robe",
		Category = "Textile and leather work",
		Blurb = "tailor a long cross-front $colour ramie under-robe",
		Action = "tailoring a textile or leather item",
		ActiveCraftItemSdesc = "an in-progress long cross-front $colour ramie under-robe craft",
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
			new CatalogueCommodityRecipeInput(Grams: 585.0, Material: "ramie cloth", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("585 grams of ramie cloth", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Sewing Needle", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Sewing Needle tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Shears", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Shears tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_joseon_long_crossfront_underrobe", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a long cross-front $colour ramie under-robe (@renaissance_joseon_long_crossfront_underrobe)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition GeneralCraftworkMakeColourNarrowBrimHorsehairScholarHat() => new()
	{
		Metadata = new("General craftwork/make $colour narrow-brim horsehair scholar hat", "crafts", ["renaissance"], ["item:renaissance_joseon_narrowbrim_horsehairhat"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make $colour narrow-brim horsehair scholar hat",
		Category = "General craftwork",
		Blurb = "make a $colour narrow-brim horsehair scholar hat",
		Action = "making a finished item",
		ActiveCraftItemSdesc = "an in-progress $colour narrow-brim horsehair scholar hat craft",
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
			new CatalogueCommodityRecipeInput(Grams: 143.0, Material: "horsehair", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("143 grams of horsehair", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_joseon_narrowbrim_horsehairhat", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a $colour narrow-brim horsehair scholar hat (@renaissance_joseon_narrowbrim_horsehairhat)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition TextileAndLeatherWorkTailorColourPaddedWinterHeadHood() => new()
	{
		Metadata = new("Textile and leather work/tailor $colour padded winter head hood", "crafts", ["renaissance"], ["item:renaissance_joseon_padded_winterhood"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "tailor $colour padded winter head hood",
		Category = "Textile and leather work",
		Blurb = "tailor a $colour padded winter head hood",
		Action = "tailoring a textile or leather item",
		ActiveCraftItemSdesc = "an in-progress $colour padded winter head hood craft",
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
			new CatalogueCommodityRecipeInput(Grams: 225.0, Material: "cotton", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("225 grams of cotton", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Sewing Needle", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Sewing Needle tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Shears", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Shears tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_joseon_padded_winterhood", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a $colour padded winter head hood (@renaissance_joseon_padded_winterhood)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition TextileAndLeatherWorkTailorColourPleatedLongOfficialCoat() => new()
	{
		Metadata = new("Textile and leather work/tailor $colour pleated long official coat", "crafts", ["renaissance"], ["item:renaissance_joseon_pleated_officialovercoat"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "tailor $colour pleated long official coat",
		Category = "Textile and leather work",
		Blurb = "tailor a $colour pleated long official coat",
		Action = "tailoring a textile or leather item",
		ActiveCraftItemSdesc = "an in-progress $colour pleated long official coat craft",
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
			new CatalogueCommodityRecipeInput(Grams: 915.0, Material: "silk", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("915 grams of silk", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Sewing Needle", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Sewing Needle tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Shears", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Shears tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_joseon_pleated_officialovercoat", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a $colour pleated long official coat (@renaissance_joseon_pleated_officialovercoat)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition TextileAndLeatherWorkTailorColourRoundedFormalOfficialCap() => new()
	{
		Metadata = new("Textile and leather work/tailor $colour rounded formal official cap", "crafts", ["renaissance"], ["item:renaissance_joseon_rounded_officialcap"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "tailor $colour rounded formal official cap",
		Category = "Textile and leather work",
		Blurb = "tailor a $colour rounded formal official cap",
		Action = "tailoring a textile or leather item",
		ActiveCraftItemSdesc = "an in-progress $colour rounded formal official cap craft",
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
			new CatalogueCommodityRecipeInput(Grams: 143.0, Material: "silk", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("143 grams of silk", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Sewing Needle", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Sewing Needle tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Shears", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Shears tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_joseon_rounded_officialcap", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a $colour rounded formal official cap (@renaissance_joseon_rounded_officialcap)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition TextileAndLeatherWorkTailorShortCrossFrontColourRamieJacket() => new()
	{
		Metadata = new("Textile and leather work/tailor short cross-front $colour ramie jacket", "crafts", ["renaissance"], ["item:renaissance_joseon_short_crossfront_jacket"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "tailor short cross-front $colour ramie jacket",
		Category = "Textile and leather work",
		Blurb = "tailor a short cross-front $colour ramie jacket",
		Action = "tailoring a textile or leather item",
		ActiveCraftItemSdesc = "an in-progress short cross-front $colour ramie jacket craft",
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
			new CatalogueCommodityRecipeInput(Grams: 390.0, Material: "ramie cloth", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("390 grams of ramie cloth", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Sewing Needle", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Sewing Needle tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Shears", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Shears tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_joseon_short_crossfront_jacket", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a short cross-front $colour ramie jacket (@renaissance_joseon_short_crossfront_jacket)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition GeneralCraftworkMakeColourWovenStrawEverydayShoes() => new()
	{
		Metadata = new("General craftwork/make $colour woven straw everyday shoes", "crafts", ["renaissance"], ["item:renaissance_joseon_straw_everydayshoes"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make $colour woven straw everyday shoes",
		Category = "General craftwork",
		Blurb = "make a pair of $colour woven straw everyday shoes",
		Action = "making a finished item",
		ActiveCraftItemSdesc = "an in-progress $colour woven straw everyday shoes craft",
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
			new CatalogueCommodityRecipeInput(Grams: 518.0, Material: "straw", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("518 grams of straw", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_joseon_straw_everydayshoes", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a pair of $colour woven straw everyday shoes (@renaissance_joseon_straw_everydayshoes)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition GeneralCraftworkMakeTallTranslucentBrimmedHat() => new()
	{
		Metadata = new("General craftwork/make tall translucent brimmed hat", "crafts", ["renaissance"], ["item:renaissance_joseon_tall_horsehairhat"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make tall translucent brimmed hat",
		Category = "General craftwork",
		Blurb = "make a tall translucent brimmed hat",
		Action = "making a finished item",
		ActiveCraftItemSdesc = "an in-progress tall translucent brimmed hat craft",
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
			new CatalogueCommodityRecipeInput(Grams: 128.0, Material: "horsehair", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("128 grams of horsehair", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_joseon_tall_horsehairhat", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a tall translucent brimmed hat (@renaissance_joseon_tall_horsehairhat)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition TextileAndLeatherWorkTailorWhiteClothShoes() => new()
	{
		Metadata = new("Textile and leather work/tailor white cloth shoes", "crafts", ["renaissance"], ["item:renaissance_joseon_white_clothshoes"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "tailor white cloth shoes",
		Category = "Textile and leather work",
		Blurb = "tailor a pair of white cloth shoes",
		Action = "tailoring a textile or leather item",
		ActiveCraftItemSdesc = "an in-progress white cloth shoes craft",
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
			new CatalogueCommodityRecipeInput(Grams: 353.0, Material: "cotton", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("353 grams of cotton", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Sewing Needle", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Sewing Needle tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Shears", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Shears tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_joseon_white_clothshoes", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a pair of white cloth shoes (@renaissance_joseon_white_clothshoes)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition TextileAndLeatherWorkTailorColourWhiteLeatherCourtBoots() => new()
	{
		Metadata = new("Textile and leather work/tailor $colour white leather court boots", "crafts", ["renaissance"], ["item:renaissance_joseon_white_leather_courtboots"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "tailor $colour white leather court boots",
		Category = "Textile and leather work",
		Blurb = "tailor a pair of $colour white leather court boots",
		Action = "tailoring a textile or leather item",
		ActiveCraftItemSdesc = "an in-progress $colour white leather court boots craft",
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
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_joseon_white_leather_courtboots", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a pair of $colour white leather court boots (@renaissance_joseon_white_leather_courtboots)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition GeneralCraftworkMakeColourLongHoodedCanvasWatchCoat() => new()
	{
		Metadata = new("General craftwork/make $colour long hooded canvas watch coat", "crafts", ["renaissance"], ["item:renaissance_maritime_canvas_watchcoat"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make $colour long hooded canvas watch coat",
		Category = "General craftwork",
		Blurb = "make a $colour long hooded canvas watch coat",
		Action = "making a finished item",
		ActiveCraftItemSdesc = "an in-progress $colour long hooded canvas watch coat craft",
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
			new CatalogueCommodityRecipeInput(Grams: 915.0, Material: "canvas", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("915 grams of canvas", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_maritime_canvas_watchcoat", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a $colour long hooded canvas watch coat (@renaissance_maritime_canvas_watchcoat)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition TextileAndLeatherWorkTailorColourSleevelessKnittedWoolWaistcoat() => new()
	{
		Metadata = new("Textile and leather work/tailor $colour sleeveless knitted wool waistcoat", "crafts", ["renaissance"], ["item:renaissance_maritime_knitted_waistcoat"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "tailor $colour sleeveless knitted wool waistcoat",
		Category = "Textile and leather work",
		Blurb = "tailor a $colour sleeveless knitted wool waistcoat",
		Action = "tailoring a textile or leather item",
		ActiveCraftItemSdesc = "an in-progress $colour sleeveless knitted wool waistcoat craft",
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
			new CatalogueCommodityRecipeInput(Grams: 420.0, Material: "wool", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("420 grams of wool", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Sewing Needle", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Sewing Needle tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Shears", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Shears tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_maritime_knitted_waistcoat", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a $colour sleeveless knitted wool waistcoat (@renaissance_maritime_knitted_waistcoat)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition GeneralCraftworkMakeColourLooseAnkleLengthDeckTrousers() => new()
	{
		Metadata = new("General craftwork/make $colour loose ankle-length deck trousers", "crafts", ["renaissance"], ["item:renaissance_maritime_loose_decktrousers"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make $colour loose ankle-length deck trousers",
		Category = "General craftwork",
		Blurb = "make a $colour loose ankle-length deck trousers",
		Action = "making a finished item",
		ActiveCraftItemSdesc = "an in-progress $colour loose ankle-length deck trousers craft",
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
			new CatalogueCommodityRecipeInput(Grams: 465.0, Material: "canvas", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("465 grams of canvas", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_maritime_loose_decktrousers", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a $colour loose ankle-length deck trousers (@renaissance_maritime_loose_decktrousers)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};
}
