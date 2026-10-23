#nullable enable

using MudSharp.Body;
using MudSharp.GameItems.Inventory.Plans;
using MudSharp.RPG.Checks;

namespace DatabaseSeeder.Seeders;

public static partial class Recipes19Stock
{
	public static CatalogueRecipeDefinition TextileAndLeatherWorkTailorColourWiredFrontLinenCoif() => new()
	{
		Metadata = new("Textile and leather work/tailor $colour wired-front linen coif", "crafts", ["renaissance"], ["item:renaissance_western_wired_linen_coif"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "tailor $colour wired-front linen coif",
		Category = "Textile and leather work",
		Blurb = "tailor a $colour wired-front linen coif",
		Action = "tailoring a textile or leather item",
		ActiveCraftItemSdesc = "an in-progress $colour wired-front linen coif craft",
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
			new CatalogueCommodityRecipeInput(Grams: 143.0, Material: "linen", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("143 grams of linen", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Sewing Needle", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Sewing Needle tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Shears", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Shears tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_western_wired_linen_coif", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a $colour wired-front linen coif (@renaissance_western_wired_linen_coif)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition TextileAndLeatherWorkTailorColourLooseWorkSmock() => new()
	{
		Metadata = new("Textile and leather work/tailor $colour loose work smock", "crafts", ["renaissance"], ["item:renaissance_western_work_smock"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "tailor $colour loose work smock",
		Category = "Textile and leather work",
		Blurb = "tailor a $colour loose work smock",
		Action = "tailoring a textile or leather item",
		ActiveCraftItemSdesc = "an in-progress $colour loose work smock craft",
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
			new CatalogueCommodityRecipeInput(Grams: 735.0, Material: "linen", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("735 grams of linen", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Sewing Needle", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Sewing Needle tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Shears", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Shears tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_western_work_smock", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a $colour loose work smock (@renaissance_western_work_smock)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition WritingAndAdministrationBindIronBoundArchiveChest() => new()
	{
		Metadata = new("Writing and administration/bind iron-bound archive chest", "crafts", ["renaissance"], ["item:renaissance_writing_archive_chest"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "bind iron-bound archive chest",
		Category = "Writing and administration",
		Blurb = "bind an iron-bound archive chest",
		Action = "binding a writing or administrative item",
		ActiveCraftItemSdesc = "an in-progress iron-bound archive chest craft",
		Gate = CatalogueRecipeGates.RenaissanceFinishedItemCraftingBookbinding15,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 3,
		Interruptable = false,
		Phases = [
			new(25, "$0 arrange|arranges $i1 and pierce|pierces the working points with $t1.", "$0 arrange|arranges $i1 and pierce|pierces the working points with $t1.", ExertionLevel.Stasis, 0),
			new(40, "$0 draw|draws $t2 through the prepared work, checking its alignment. $0 pause|pauses to check the proportions before carrying on.", "$0 draw|draws $t2 through the prepared work, checking its alignment. $0 pause|pauses to check the proportions before carrying on.", ExertionLevel.Stasis, 0),
			new(25, "$0 trim|trims the finished form and set|sets aside $p1.", "$0 find|finds the binding has slipped and set|sets the material aside.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 24700.0, Material: "oak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("24700 grams of oak", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Awl Punch", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Awl Punch tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Sewing Needle", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Sewing Needle tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_writing_archive_chest", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x an iron-bound archive chest (@renaissance_writing_archive_chest)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition WritingAndAdministrationBindFoldedCivicWarrant() => new()
	{
		Metadata = new("Writing and administration/bind folded civic warrant", "crafts", ["renaissance"], ["item:renaissance_writing_civic_warrant"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "bind folded civic warrant",
		Category = "Writing and administration",
		Blurb = "bind a folded civic warrant",
		Action = "binding a writing or administrative item",
		ActiveCraftItemSdesc = "an in-progress folded civic warrant craft",
		Gate = CatalogueRecipeGates.RenaissanceFinishedItemCraftingBookbinding15,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 3,
		Interruptable = false,
		Phases = [
			new(25, "$0 arrange|arranges $i1 and pierce|pierces the working points with $t1.", "$0 arrange|arranges $i1 and pierce|pierces the working points with $t1.", ExertionLevel.Stasis, 0),
			new(40, "$0 draw|draws $t2 through the prepared work, checking its alignment. $0 pause|pauses to check the proportions before carrying on.", "$0 draw|draws $t2 through the prepared work, checking its alignment. $0 pause|pauses to check the proportions before carrying on.", ExertionLevel.Stasis, 0),
			new(25, "$0 trim|trims the finished form and set|sets aside $p1.", "$0 find|finds the binding has slipped and set|sets the material aside.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 23.0, Material: "parchment", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("23 grams of parchment", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Awl Punch", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Awl Punch tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Sewing Needle", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Sewing Needle tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_writing_civic_warrant", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a folded civic warrant (@renaissance_writing_civic_warrant)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition WritingAndAdministrationBindLeatherBoundCommonplaceBook() => new()
	{
		Metadata = new("Writing and administration/bind leather-bound commonplace book", "crafts", ["renaissance"], ["item:renaissance_writing_commonplace_book"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "bind leather-bound commonplace book",
		Category = "Writing and administration",
		Blurb = "bind a leather-bound commonplace book",
		Action = "binding a writing or administrative item",
		ActiveCraftItemSdesc = "an in-progress leather-bound commonplace book craft",
		Gate = CatalogueRecipeGates.RenaissanceFinishedItemCraftingBookbinding15,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 3,
		Interruptable = false,
		Phases = [
			new(25, "$0 arrange|arranges $i1 and pierce|pierces the working points with $t1.", "$0 arrange|arranges $i1 and pierce|pierces the working points with $t1.", ExertionLevel.Stasis, 0),
			new(40, "$0 draw|draws $t2 through the prepared work, checking its alignment. $0 pause|pauses to check the proportions before carrying on.", "$0 draw|draws $t2 through the prepared work, checking its alignment. $0 pause|pauses to check the proportions before carrying on.", ExertionLevel.Stasis, 0),
			new(25, "$0 trim|trims the finished form and set|sets aside $p1.", "$0 find|finds the binding has slipped and set|sets the material aside.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 423.0, Material: "paper", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("423 grams of paper", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Awl Punch", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Awl Punch tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Sewing Needle", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Sewing Needle tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_writing_commonplace_book", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a leather-bound commonplace book (@renaissance_writing_commonplace_book)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition WritingAndAdministrationBindPolishedCopperEngravingPlate() => new()
	{
		Metadata = new("Writing and administration/bind polished copper engraving plate", "crafts", ["renaissance"], ["item:renaissance_writing_copper_engraving_plate"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "bind polished copper engraving plate",
		Category = "Writing and administration",
		Blurb = "bind a polished copper engraving plate",
		Action = "binding a writing or administrative item",
		ActiveCraftItemSdesc = "an in-progress polished copper engraving plate craft",
		Gate = CatalogueRecipeGates.RenaissanceFinishedItemCraftingBookbinding15,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 3,
		Interruptable = false,
		Phases = [
			new(25, "$0 arrange|arranges $i1 and pierce|pierces the working points with $t1.", "$0 arrange|arranges $i1 and pierce|pierces the working points with $t1.", ExertionLevel.Stasis, 0),
			new(40, "$0 draw|draws $t2 through the prepared work, checking its alignment. $0 measure|measures the developing form against the intended finish.", "$0 draw|draws $t2 through the prepared work, checking its alignment. $0 measure|measures the developing form against the intended finish.", ExertionLevel.Stasis, 0),
			new(25, "$0 trim|trims the finished form and set|sets aside $p1.", "$0 find|finds the binding has slipped and set|sets the material aside.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 611.0, Material: "copper", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("611 grams of copper", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Awl Punch", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Awl Punch tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Sewing Needle", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Sewing Needle tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_writing_copper_engraving_plate", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a polished copper engraving plate (@renaissance_writing_copper_engraving_plate)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition WritingAndAdministrationBindBuckledCourierSatchel() => new()
	{
		Metadata = new("Writing and administration/bind buckled courier satchel", "crafts", ["renaissance"], ["item:renaissance_writing_courier_satchel"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "bind buckled courier satchel",
		Category = "Writing and administration",
		Blurb = "bind a buckled courier satchel",
		Action = "binding a writing or administrative item",
		ActiveCraftItemSdesc = "an in-progress buckled courier satchel craft",
		Gate = CatalogueRecipeGates.RenaissanceFinishedItemCraftingBookbinding15,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 3,
		Interruptable = false,
		Phases = [
			new(25, "$0 arrange|arranges $i1 and pierce|pierces the working points with $t1.", "$0 arrange|arranges $i1 and pierce|pierces the working points with $t1.", ExertionLevel.Stasis, 0),
			new(40, "$0 draw|draws $t2 through the prepared work, checking its alignment. $0 pause|pauses to check the proportions before carrying on.", "$0 draw|draws $t2 through the prepared work, checking its alignment. $0 pause|pauses to check the proportions before carrying on.", ExertionLevel.Stasis, 0),
			new(25, "$0 trim|trims the finished form and set|sets aside $p1.", "$0 find|finds the binding has slipped and set|sets the material aside.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 598.0, Material: "leather", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("598 grams of leather", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Awl Punch", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Awl Punch tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Sewing Needle", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Sewing Needle tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_writing_courier_satchel", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a buckled courier satchel (@renaissance_writing_courier_satchel)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition WritingAndAdministrationBindGraverSSteelBurin() => new()
	{
		Metadata = new("Writing and administration/bind graver's steel burin", "crafts", ["renaissance"], ["item:renaissance_writing_engraving_burin"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "bind graver's steel burin",
		Category = "Writing and administration",
		Blurb = "bind a graver's steel burin",
		Action = "binding a writing or administrative item",
		ActiveCraftItemSdesc = "an in-progress graver's steel burin craft",
		Gate = CatalogueRecipeGates.RenaissanceFinishedItemCraftingBookbinding15,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 3,
		Interruptable = false,
		Phases = [
			new(25, "$0 arrange|arranges $i1 and pierce|pierces the working points with $t1.", "$0 arrange|arranges $i1 and pierce|pierces the working points with $t1.", ExertionLevel.Stasis, 0),
			new(40, "$0 draw|draws $t2 through the prepared work, checking its alignment. $0 measure|measures the developing form against the intended finish.", "$0 draw|draws $t2 through the prepared work, checking its alignment. $0 measure|measures the developing form against the intended finish.", ExertionLevel.Stasis, 0),
			new(25, "$0 trim|trims the finished form and set|sets aside $p1.", "$0 find|finds the binding has slipped and set|sets the material aside.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 117.0, Material: "wrought iron", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("117 grams of wrought iron", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Awl Punch", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Awl Punch tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Sewing Needle", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Sewing Needle tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_writing_engraving_burin", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a graver's steel burin (@renaissance_writing_engraving_burin)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition WritingAndAdministrationBindEtchingNeedle() => new()
	{
		Metadata = new("Writing and administration/bind etching needle", "crafts", ["renaissance"], ["item:renaissance_writing_etching_needle"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "bind etching needle",
		Category = "Writing and administration",
		Blurb = "bind an etching needle",
		Action = "binding a writing or administrative item",
		ActiveCraftItemSdesc = "an in-progress etching needle craft",
		Gate = CatalogueRecipeGates.RenaissanceFinishedItemCraftingBookbinding15,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 3,
		Interruptable = false,
		Phases = [
			new(25, "$0 arrange|arranges $i1 and pierce|pierces the working points with $t1.", "$0 arrange|arranges $i1 and pierce|pierces the working points with $t1.", ExertionLevel.Stasis, 0),
			new(40, "$0 draw|draws $t2 through the prepared work, checking its alignment. $0 pause|pauses to check the proportions before carrying on.", "$0 draw|draws $t2 through the prepared work, checking its alignment. $0 pause|pauses to check the proportions before carrying on.", ExertionLevel.Stasis, 0),
			new(25, "$0 trim|trims the finished form and set|sets aside $p1.", "$0 find|finds the binding has slipped and set|sets the material aside.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 39.0, Material: "wrought iron", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("39 grams of wrought iron", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Awl Punch", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Awl Punch tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Sewing Needle", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Sewing Needle tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_writing_etching_needle", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x an etching needle (@renaissance_writing_etching_needle)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition WritingAndAdministrationBindSealedNotarialInstrument() => new()
	{
		Metadata = new("Writing and administration/bind sealed notarial instrument", "crafts", ["renaissance"], ["item:renaissance_writing_notarial_instrument"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "bind sealed notarial instrument",
		Category = "Writing and administration",
		Blurb = "bind a sealed notarial instrument",
		Action = "binding a writing or administrative item",
		ActiveCraftItemSdesc = "an in-progress sealed notarial instrument craft",
		Gate = CatalogueRecipeGates.RenaissanceFinishedItemCraftingBookbinding15,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 3,
		Interruptable = false,
		Phases = [
			new(25, "$0 arrange|arranges $i1 and pierce|pierces the working points with $t1.", "$0 arrange|arranges $i1 and pierce|pierces the working points with $t1.", ExertionLevel.Stasis, 0),
			new(40, "$0 draw|draws $t2 through the prepared work, checking its alignment. $0 measure|measures the developing form against the intended finish.", "$0 draw|draws $t2 through the prepared work, checking its alignment. $0 measure|measures the developing form against the intended finish.", ExertionLevel.Stasis, 0),
			new(25, "$0 trim|trims the finished form and set|sets aside $p1.", "$0 find|finds the binding has slipped and set|sets the material aside.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 29.0, Material: "parchment", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("29 grams of parchment", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Awl Punch", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Awl Punch tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Sewing Needle", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Sewing Needle tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_writing_notarial_instrument", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a sealed notarial instrument (@renaissance_writing_notarial_instrument)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition WritingAndAdministrationBindBoundPilotBook() => new()
	{
		Metadata = new("Writing and administration/bind bound pilot book", "crafts", ["renaissance"], ["item:renaissance_writing_pilot_book"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "bind bound pilot book",
		Category = "Writing and administration",
		Blurb = "bind a bound pilot book",
		Action = "binding a writing or administrative item",
		ActiveCraftItemSdesc = "an in-progress bound pilot book craft",
		Gate = CatalogueRecipeGates.RenaissanceFinishedItemCraftingBookbinding15,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 3,
		Interruptable = false,
		Phases = [
			new(25, "$0 arrange|arranges $i1 and pierce|pierces the working points with $t1.", "$0 arrange|arranges $i1 and pierce|pierces the working points with $t1.", ExertionLevel.Stasis, 0),
			new(40, "$0 draw|draws $t2 through the prepared work, checking its alignment. $0 pause|pauses to check the proportions before carrying on.", "$0 draw|draws $t2 through the prepared work, checking its alignment. $0 pause|pauses to check the proportions before carrying on.", ExertionLevel.Stasis, 0),
			new(25, "$0 trim|trims the finished form and set|sets aside $p1.", "$0 find|finds the binding has slipped and set|sets the material aside.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 494.0, Material: "paper", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("494 grams of paper", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Awl Punch", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Awl Punch tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Sewing Needle", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Sewing Needle tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_writing_pilot_book", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a bound pilot book (@renaissance_writing_pilot_book)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition WritingAndAdministrationBindRolledPortolanChart() => new()
	{
		Metadata = new("Writing and administration/bind rolled portolan chart", "crafts", ["renaissance"], ["item:renaissance_writing_portolan_chart"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "bind rolled portolan chart",
		Category = "Writing and administration",
		Blurb = "bind a rolled portolan chart",
		Action = "binding a writing or administrative item",
		ActiveCraftItemSdesc = "an in-progress rolled portolan chart craft",
		Gate = CatalogueRecipeGates.RenaissanceFinishedItemCraftingBookbinding15,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 3,
		Interruptable = false,
		Phases = [
			new(25, "$0 arrange|arranges $i1 and pierce|pierces the working points with $t1.", "$0 arrange|arranges $i1 and pierce|pierces the working points with $t1.", ExertionLevel.Stasis, 0),
			new(40, "$0 draw|draws $t2 through the prepared work, checking its alignment. $0 measure|measures the developing form against the intended finish.", "$0 draw|draws $t2 through the prepared work, checking its alignment. $0 measure|measures the developing form against the intended finish.", ExertionLevel.Stasis, 0),
			new(25, "$0 trim|trims the finished form and set|sets aside $p1.", "$0 find|finds the binding has slipped and set|sets the material aside.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 62.0, Material: "parchment", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("62 grams of parchment", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Awl Punch", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Awl Punch tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Sewing Needle", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Sewing Needle tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_writing_portolan_chart", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a rolled portolan chart (@renaissance_writing_portolan_chart)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition WritingAndAdministrationBindBoundPortCustomsRegister() => new()
	{
		Metadata = new("Writing and administration/bind bound port customs register", "crafts", ["renaissance"], ["item:renaissance_writing_port_customs_register"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "bind bound port customs register",
		Category = "Writing and administration",
		Blurb = "bind a bound port customs register",
		Action = "binding a writing or administrative item",
		ActiveCraftItemSdesc = "an in-progress bound port customs register craft",
		Gate = CatalogueRecipeGates.RenaissanceFinishedItemCraftingBookbinding15,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 3,
		Interruptable = false,
		Phases = [
			new(25, "$0 arrange|arranges $i1 and pierce|pierces the working points with $t1.", "$0 arrange|arranges $i1 and pierce|pierces the working points with $t1.", ExertionLevel.Stasis, 0),
			new(40, "$0 draw|draws $t2 through the prepared work, checking its alignment. $0 measure|measures the developing form against the intended finish.", "$0 draw|draws $t2 through the prepared work, checking its alignment. $0 measure|measures the developing form against the intended finish.", ExertionLevel.Stasis, 0),
			new(25, "$0 trim|trims the finished form and set|sets aside $p1.", "$0 find|finds the binding has slipped and set|sets the material aside.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 553.0, Material: "paper", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("553 grams of paper", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Awl Punch", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Awl Punch tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Sewing Needle", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Sewing Needle tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_writing_port_customs_register", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a bound port customs register (@renaissance_writing_port_customs_register)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition WritingAndAdministrationBindStitchedPrintedMusicBook() => new()
	{
		Metadata = new("Writing and administration/bind stitched printed music book", "crafts", ["renaissance"], ["item:renaissance_writing_printed_music_book"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "bind stitched printed music book",
		Category = "Writing and administration",
		Blurb = "bind a stitched printed music book",
		Action = "binding a writing or administrative item",
		ActiveCraftItemSdesc = "an in-progress stitched printed music book craft",
		Gate = CatalogueRecipeGates.RenaissanceFinishedItemCraftingBookbinding15,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 3,
		Interruptable = false,
		Phases = [
			new(25, "$0 arrange|arranges $i1 and pierce|pierces the working points with $t1.", "$0 arrange|arranges $i1 and pierce|pierces the working points with $t1.", ExertionLevel.Stasis, 0),
			new(40, "$0 draw|draws $t2 through the prepared work, checking its alignment. $0 measure|measures the developing form against the intended finish.", "$0 draw|draws $t2 through the prepared work, checking its alignment. $0 measure|measures the developing form against the intended finish.", ExertionLevel.Stasis, 0),
			new(25, "$0 trim|trims the finished form and set|sets aside $p1.", "$0 find|finds the binding has slipped and set|sets the material aside.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 299.0, Material: "paper", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("299 grams of paper", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Awl Punch", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Awl Punch tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Sewing Needle", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Sewing Needle tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_writing_printed_music_book", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a stitched printed music book (@renaissance_writing_printed_music_book)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition WritingAndAdministrationBindSmallPrintedPrayerBook() => new()
	{
		Metadata = new("Writing and administration/bind small printed prayer book", "crafts", ["renaissance"], ["item:renaissance_writing_printed_prayer_book"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "bind small printed prayer book",
		Category = "Writing and administration",
		Blurb = "bind a small printed prayer book",
		Action = "binding a writing or administrative item",
		ActiveCraftItemSdesc = "an in-progress small printed prayer book craft",
		Gate = CatalogueRecipeGates.RenaissanceFinishedItemCraftingBookbinding15,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 3,
		Interruptable = false,
		Phases = [
			new(25, "$0 arrange|arranges $i1 and pierce|pierces the working points with $t1.", "$0 arrange|arranges $i1 and pierce|pierces the working points with $t1.", ExertionLevel.Stasis, 0),
			new(40, "$0 draw|draws $t2 through the prepared work, checking its alignment. $0 measure|measures the developing form against the intended finish.", "$0 draw|draws $t2 through the prepared work, checking its alignment. $0 measure|measures the developing form against the intended finish.", ExertionLevel.Stasis, 0),
			new(25, "$0 trim|trims the finished form and set|sets aside $p1.", "$0 find|finds the binding has slipped and set|sets the material aside.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 338.0, Material: "paper", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("338 grams of paper", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Awl Punch", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Awl Punch tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Sewing Needle", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Sewing Needle tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_writing_printed_prayer_book", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a small printed prayer book (@renaissance_writing_printed_prayer_book)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition WritingAndAdministrationBindRedSealingWaxStick() => new()
	{
		Metadata = new("Writing and administration/bind red sealing-wax stick", "crafts", ["renaissance"], ["item:renaissance_writing_sealing_wax_stick"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "bind red sealing-wax stick",
		Category = "Writing and administration",
		Blurb = "bind a red sealing-wax stick",
		Action = "binding a writing or administrative item",
		ActiveCraftItemSdesc = "an in-progress red sealing-wax stick craft",
		Gate = CatalogueRecipeGates.RenaissanceFinishedItemCraftingBookbinding15,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 3,
		Interruptable = false,
		Phases = [
			new(25, "$0 arrange|arranges $i1 and pierce|pierces the working points with $t1.", "$0 arrange|arranges $i1 and pierce|pierces the working points with $t1.", ExertionLevel.Stasis, 0),
			new(40, "$0 draw|draws $t2 through the prepared work, checking its alignment. $0 turn|turns the work over to judge its balance.", "$0 draw|draws $t2 through the prepared work, checking its alignment. $0 turn|turns the work over to judge its balance.", ExertionLevel.Stasis, 0),
			new(25, "$0 trim|trims the finished form and set|sets aside $p1.", "$0 find|finds the binding has slipped and set|sets the material aside.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 55.0, Material: "beeswax", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("55 grams of beeswax", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Awl Punch", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Awl Punch tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Sewing Needle", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Sewing Needle tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_writing_sealing_wax_stick", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a red sealing-wax stick (@renaissance_writing_sealing_wax_stick)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition WritingAndAdministrationBindBrassSealMatrix() => new()
	{
		Metadata = new("Writing and administration/bind brass seal matrix", "crafts", ["renaissance"], ["item:renaissance_writing_seal_matrix"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "bind brass seal matrix",
		Category = "Writing and administration",
		Blurb = "bind a brass seal matrix",
		Action = "binding a writing or administrative item",
		ActiveCraftItemSdesc = "an in-progress brass seal matrix craft",
		Gate = CatalogueRecipeGates.RenaissanceFinishedItemCraftingBookbinding15,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 3,
		Interruptable = false,
		Phases = [
			new(25, "$0 arrange|arranges $i1 and pierce|pierces the working points with $t1.", "$0 arrange|arranges $i1 and pierce|pierces the working points with $t1.", ExertionLevel.Stasis, 0),
			new(40, "$0 draw|draws $t2 through the prepared work, checking its alignment. $0 clear|clears the work area before the final steps.", "$0 draw|draws $t2 through the prepared work, checking its alignment. $0 clear|clears the work area before the final steps.", ExertionLevel.Stasis, 0),
			new(25, "$0 trim|trims the finished form and set|sets aside $p1.", "$0 find|finds the binding has slipped and set|sets the material aside.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 104.0, Material: "brass", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("104 grams of brass", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Awl Punch", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Awl Punch tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Sewing Needle", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Sewing Needle tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_writing_seal_matrix", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a brass seal matrix (@renaissance_writing_seal_matrix)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition WritingAndAdministrationBindStitchedArtistSSketchbook() => new()
	{
		Metadata = new("Writing and administration/bind stitched artist's sketchbook", "crafts", ["renaissance"], ["item:renaissance_writing_sketchbook"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "bind stitched artist's sketchbook",
		Category = "Writing and administration",
		Blurb = "bind a stitched artist's sketchbook",
		Action = "binding a writing or administrative item",
		ActiveCraftItemSdesc = "an in-progress stitched artist's sketchbook craft",
		Gate = CatalogueRecipeGates.RenaissanceFinishedItemCraftingBookbinding15,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 3,
		Interruptable = false,
		Phases = [
			new(25, "$0 arrange|arranges $i1 and pierce|pierces the working points with $t1.", "$0 arrange|arranges $i1 and pierce|pierces the working points with $t1.", ExertionLevel.Stasis, 0),
			new(40, "$0 draw|draws $t2 through the prepared work, checking its alignment. $0 turn|turns the work over to judge its balance.", "$0 draw|draws $t2 through the prepared work, checking its alignment. $0 turn|turns the work over to judge its balance.", ExertionLevel.Stasis, 0),
			new(25, "$0 trim|trims the finished form and set|sets aside $p1.", "$0 find|finds the binding has slipped and set|sets the material aside.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 273.0, Material: "paper", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("273 grams of paper", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Awl Punch", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Awl Punch tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Sewing Needle", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Sewing Needle tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_writing_sketchbook", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a stitched artist's sketchbook (@renaissance_writing_sketchbook)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition WritingAndAdministrationBindCarvedPrintingWoodblock() => new()
	{
		Metadata = new("Writing and administration/bind carved printing woodblock", "crafts", ["renaissance"], ["item:renaissance_writing_woodblock"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "bind carved printing woodblock",
		Category = "Writing and administration",
		Blurb = "bind a carved printing woodblock",
		Action = "binding a writing or administrative item",
		ActiveCraftItemSdesc = "an in-progress carved printing woodblock craft",
		Gate = CatalogueRecipeGates.RenaissanceFinishedItemCraftingBookbinding15,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 3,
		Interruptable = false,
		Phases = [
			new(25, "$0 arrange|arranges $i1 and pierce|pierces the working points with $t1.", "$0 arrange|arranges $i1 and pierce|pierces the working points with $t1.", ExertionLevel.Stasis, 0),
			new(40, "$0 draw|draws $t2 through the prepared work, checking its alignment. $0 pause|pauses to check the proportions before carrying on.", "$0 draw|draws $t2 through the prepared work, checking its alignment. $0 pause|pauses to check the proportions before carrying on.", ExertionLevel.Stasis, 0),
			new(25, "$0 trim|trims the finished form and set|sets aside $p1.", "$0 find|finds the binding has slipped and set|sets the material aside.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 488.0, Material: "oak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("488 grams of oak", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Awl Punch", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Awl Punch tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Sewing Needle", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Sewing Needle tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_writing_woodblock", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a carved printing woodblock (@renaissance_writing_woodblock)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ClothingFootwearAndUniformsMakePlainWoolRagForDomesticWearCraftIndustrialisedClothingFootwearUniforms0001PlainRag() => new()
	{
		Metadata = new("Clothing, footwear and uniforms/make plain wool rag for domestic wear [craft_industrialised_clothing_footwear_uniforms_0001_plain_rag]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_clothing_footwear_uniforms_0001_plain_rag"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make plain wool rag for domestic wear [craft_industrialised_clothing_footwear_uniforms_0001_plain_rag]",
		Category = "Clothing, footwear and uniforms",
		Blurb = "Make a plain wool rag for domestic wear from prepared wool stock.",
		Action = "making plain wool rag for domestic wear",
		ActiveCraftItemSdesc = "an in-progress plain wool rag for domestic wear craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 207.0, Material: "wool", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("207 grams of wool", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_clothing_footwear_uniforms_0001_plain_rag", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a plain wool rag for domestic wear (@industrialised_clothing_footwear_uniforms_0001_plain_rag)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ClothingFootwearAndUniformsMakePlainLeatherApronForDomesticWearCraftIndustrialisedClothingFootwearUniforms0002PlainApron() => new()
	{
		Metadata = new("Clothing, footwear and uniforms/make plain leather apron for domestic wear [craft_industrialised_clothing_footwear_uniforms_0002_plain_apron]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_clothing_footwear_uniforms_0002_plain_apron"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make plain leather apron for domestic wear [craft_industrialised_clothing_footwear_uniforms_0002_plain_apron]",
		Category = "Clothing, footwear and uniforms",
		Blurb = "Make a plain leather apron for domestic wear from prepared leather stock.",
		Action = "making plain leather apron for domestic wear",
		ActiveCraftItemSdesc = "an in-progress plain leather apron for domestic wear craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 248.0, Material: "leather", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("248 grams of leather", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_clothing_footwear_uniforms_0002_plain_apron", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a plain leather apron for domestic wear (@industrialised_clothing_footwear_uniforms_0002_plain_apron)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ClothingFootwearAndUniformsMakePlainCottonShirtForDomesticWearCraftIndustrialisedClothingFootwearUniforms0003PlainShirt() => new()
	{
		Metadata = new("Clothing, footwear and uniforms/make plain cotton shirt for domestic wear [craft_industrialised_clothing_footwear_uniforms_0003_plain_shirt]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_clothing_footwear_uniforms_0003_plain_shirt"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make plain cotton shirt for domestic wear [craft_industrialised_clothing_footwear_uniforms_0003_plain_shirt]",
		Category = "Clothing, footwear and uniforms",
		Blurb = "Make a plain cotton shirt for domestic wear from prepared cotton stock.",
		Action = "making plain cotton shirt for domestic wear",
		ActiveCraftItemSdesc = "an in-progress plain cotton shirt for domestic wear craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 288.0, Material: "cotton", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("288 grams of cotton", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_clothing_footwear_uniforms_0003_plain_shirt", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a plain cotton shirt for domestic wear (@industrialised_clothing_footwear_uniforms_0003_plain_shirt)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ClothingFootwearAndUniformsMakeSomePlainLinenTrousersForDomesticWearCraftIndustrialisedClothingFootwearUniforms0004PlainTrousers() => new()
	{
		Metadata = new("Clothing, footwear and uniforms/make some plain linen trousers for domestic wear [craft_industrialised_clothing_footwear_uniforms_0004_plain_trousers]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_clothing_footwear_uniforms_0004_plain_trousers"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make some plain linen trousers for domestic wear [craft_industrialised_clothing_footwear_uniforms_0004_plain_trousers]",
		Category = "Clothing, footwear and uniforms",
		Blurb = "Make some plain linen trousers for domestic wear from prepared linen stock.",
		Action = "making some plain linen trousers for domestic wear",
		ActiveCraftItemSdesc = "an in-progress some plain linen trousers for domestic wear craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 329.0, Material: "linen", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("329 grams of linen", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_clothing_footwear_uniforms_0004_plain_trousers", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x some plain linen trousers for domestic wear (@industrialised_clothing_footwear_uniforms_0004_plain_trousers)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ClothingFootwearAndUniformsMakeSomePlainWoolBootsForDomesticWearCraftIndustrialisedClothingFootwearUniforms0005PlainBoots() => new()
	{
		Metadata = new("Clothing, footwear and uniforms/make some plain wool boots for domestic wear [craft_industrialised_clothing_footwear_uniforms_0005_plain_boots]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_clothing_footwear_uniforms_0005_plain_boots"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make some plain wool boots for domestic wear [craft_industrialised_clothing_footwear_uniforms_0005_plain_boots]",
		Category = "Clothing, footwear and uniforms",
		Blurb = "Make some plain wool boots for domestic wear from prepared wool stock.",
		Action = "making some plain wool boots for domestic wear",
		ActiveCraftItemSdesc = "an in-progress some plain wool boots for domestic wear craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 370.0, Material: "wool", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("370 grams of wool", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_clothing_footwear_uniforms_0005_plain_boots", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x some plain wool boots for domestic wear (@industrialised_clothing_footwear_uniforms_0005_plain_boots)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ClothingFootwearAndUniformsMakePlainLeatherDressForDomesticWearCraftIndustrialisedClothingFootwearUniforms0006PlainDress() => new()
	{
		Metadata = new("Clothing, footwear and uniforms/make plain leather dress for domestic wear [craft_industrialised_clothing_footwear_uniforms_0006_plain_dress]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_clothing_footwear_uniforms_0006_plain_dress"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make plain leather dress for domestic wear [craft_industrialised_clothing_footwear_uniforms_0006_plain_dress]",
		Category = "Clothing, footwear and uniforms",
		Blurb = "Make a plain leather dress for domestic wear from prepared leather stock.",
		Action = "making plain leather dress for domestic wear",
		ActiveCraftItemSdesc = "an in-progress plain leather dress for domestic wear craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 410.0, Material: "leather", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("410 grams of leather", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_clothing_footwear_uniforms_0006_plain_dress", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a plain leather dress for domestic wear (@industrialised_clothing_footwear_uniforms_0006_plain_dress)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ClothingFootwearAndUniformsMakePlainCottonJacketForDomesticWearCraftIndustrialisedClothingFootwearUniforms0007PlainJacket() => new()
	{
		Metadata = new("Clothing, footwear and uniforms/make plain cotton jacket for domestic wear [craft_industrialised_clothing_footwear_uniforms_0007_plain_jacket]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_clothing_footwear_uniforms_0007_plain_jacket"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make plain cotton jacket for domestic wear [craft_industrialised_clothing_footwear_uniforms_0007_plain_jacket]",
		Category = "Clothing, footwear and uniforms",
		Blurb = "Make a plain cotton jacket for domestic wear from prepared cotton stock.",
		Action = "making plain cotton jacket for domestic wear",
		ActiveCraftItemSdesc = "an in-progress plain cotton jacket for domestic wear craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 451.0, Material: "cotton", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("451 grams of cotton", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_clothing_footwear_uniforms_0007_plain_jacket", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a plain cotton jacket for domestic wear (@industrialised_clothing_footwear_uniforms_0007_plain_jacket)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ClothingFootwearAndUniformsMakePlainLinenHatForDomesticWearCraftIndustrialisedClothingFootwearUniforms0008PlainHat() => new()
	{
		Metadata = new("Clothing, footwear and uniforms/make plain linen hat for domestic wear [craft_industrialised_clothing_footwear_uniforms_0008_plain_hat]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_clothing_footwear_uniforms_0008_plain_hat"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make plain linen hat for domestic wear [craft_industrialised_clothing_footwear_uniforms_0008_plain_hat]",
		Category = "Clothing, footwear and uniforms",
		Blurb = "Make a plain linen hat for domestic wear from prepared linen stock.",
		Action = "making plain linen hat for domestic wear",
		ActiveCraftItemSdesc = "an in-progress plain linen hat for domestic wear craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 492.0, Material: "linen", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("492 grams of linen", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_clothing_footwear_uniforms_0008_plain_hat", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a plain linen hat for domestic wear (@industrialised_clothing_footwear_uniforms_0008_plain_hat)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ClothingFootwearAndUniformsMakeSomePlainWoolGlovesForDomesticWearCraftIndustrialisedClothingFootwearUniforms0009PlainGloves() => new()
	{
		Metadata = new("Clothing, footwear and uniforms/make some plain wool gloves for domestic wear [craft_industrialised_clothing_footwear_uniforms_0009_plain_gloves]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_clothing_footwear_uniforms_0009_plain_gloves"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make some plain wool gloves for domestic wear [craft_industrialised_clothing_footwear_uniforms_0009_plain_gloves]",
		Category = "Clothing, footwear and uniforms",
		Blurb = "Make some plain wool gloves for domestic wear from prepared wool stock.",
		Action = "making some plain wool gloves for domestic wear",
		ActiveCraftItemSdesc = "an in-progress some plain wool gloves for domestic wear craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 532.0, Material: "wool", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("532 grams of wool", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_clothing_footwear_uniforms_0009_plain_gloves", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x some plain wool gloves for domestic wear (@industrialised_clothing_footwear_uniforms_0009_plain_gloves)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ClothingFootwearAndUniformsMakePlainLeatherScarfForDomesticWearCraftIndustrialisedClothingFootwearUniforms0010PlainScarf() => new()
	{
		Metadata = new("Clothing, footwear and uniforms/make plain leather scarf for domestic wear [craft_industrialised_clothing_footwear_uniforms_0010_plain_scarf]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_clothing_footwear_uniforms_0010_plain_scarf"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make plain leather scarf for domestic wear [craft_industrialised_clothing_footwear_uniforms_0010_plain_scarf]",
		Category = "Clothing, footwear and uniforms",
		Blurb = "Make a plain leather scarf for domestic wear from prepared leather stock.",
		Action = "making plain leather scarf for domestic wear",
		ActiveCraftItemSdesc = "an in-progress plain leather scarf for domestic wear craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 573.0, Material: "leather", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("573 grams of leather", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_clothing_footwear_uniforms_0010_plain_scarf", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a plain leather scarf for domestic wear (@industrialised_clothing_footwear_uniforms_0010_plain_scarf)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ClothingFootwearAndUniformsMakePlainCottonSkirtForDomesticWearCraftIndustrialisedClothingFootwearUniforms0011PlainSkirt() => new()
	{
		Metadata = new("Clothing, footwear and uniforms/make plain cotton skirt for domestic wear [craft_industrialised_clothing_footwear_uniforms_0011_plain_skirt]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_clothing_footwear_uniforms_0011_plain_skirt"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make plain cotton skirt for domestic wear [craft_industrialised_clothing_footwear_uniforms_0011_plain_skirt]",
		Category = "Clothing, footwear and uniforms",
		Blurb = "Make a plain cotton skirt for domestic wear from prepared cotton stock.",
		Action = "making plain cotton skirt for domestic wear",
		ActiveCraftItemSdesc = "an in-progress plain cotton skirt for domestic wear craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 614.0, Material: "cotton", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("614 grams of cotton", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_clothing_footwear_uniforms_0011_plain_skirt", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a plain cotton skirt for domestic wear (@industrialised_clothing_footwear_uniforms_0011_plain_skirt)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ClothingFootwearAndUniformsMakeSomePlainLinenSocksForDomesticWearCraftIndustrialisedClothingFootwearUniforms0012PlainSocks() => new()
	{
		Metadata = new("Clothing, footwear and uniforms/make some plain linen socks for domestic wear [craft_industrialised_clothing_footwear_uniforms_0012_plain_socks]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_clothing_footwear_uniforms_0012_plain_socks"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make some plain linen socks for domestic wear [craft_industrialised_clothing_footwear_uniforms_0012_plain_socks]",
		Category = "Clothing, footwear and uniforms",
		Blurb = "Make some plain linen socks for domestic wear from prepared linen stock.",
		Action = "making some plain linen socks for domestic wear",
		ActiveCraftItemSdesc = "an in-progress some plain linen socks for domestic wear craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 654.0, Material: "linen", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("654 grams of linen", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_clothing_footwear_uniforms_0012_plain_socks", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x some plain linen socks for domestic wear (@industrialised_clothing_footwear_uniforms_0012_plain_socks)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ClothingFootwearAndUniformsMakePlainWoolVestForDomesticWearCraftIndustrialisedClothingFootwearUniforms0013PlainVest() => new()
	{
		Metadata = new("Clothing, footwear and uniforms/make plain wool vest for domestic wear [craft_industrialised_clothing_footwear_uniforms_0013_plain_vest]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_clothing_footwear_uniforms_0013_plain_vest"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make plain wool vest for domestic wear [craft_industrialised_clothing_footwear_uniforms_0013_plain_vest]",
		Category = "Clothing, footwear and uniforms",
		Blurb = "Make a plain wool vest for domestic wear from prepared wool stock.",
		Action = "making plain wool vest for domestic wear",
		ActiveCraftItemSdesc = "an in-progress plain wool vest for domestic wear craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 695.0, Material: "wool", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("695 grams of wool", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_clothing_footwear_uniforms_0013_plain_vest", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a plain wool vest for domestic wear (@industrialised_clothing_footwear_uniforms_0013_plain_vest)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ClothingFootwearAndUniformsMakePlainLeatherCoatForDomesticWearCraftIndustrialisedClothingFootwearUniforms0014PlainCoat() => new()
	{
		Metadata = new("Clothing, footwear and uniforms/make plain leather coat for domestic wear [craft_industrialised_clothing_footwear_uniforms_0014_plain_coat]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_clothing_footwear_uniforms_0014_plain_coat"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make plain leather coat for domestic wear [craft_industrialised_clothing_footwear_uniforms_0014_plain_coat]",
		Category = "Clothing, footwear and uniforms",
		Blurb = "Make a plain leather coat for domestic wear from prepared leather stock.",
		Action = "making plain leather coat for domestic wear",
		ActiveCraftItemSdesc = "an in-progress plain leather coat for domestic wear craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 736.0, Material: "leather", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("736 grams of leather", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_clothing_footwear_uniforms_0014_plain_coat", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a plain leather coat for domestic wear (@industrialised_clothing_footwear_uniforms_0014_plain_coat)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ClothingFootwearAndUniformsMakePlainCottonSmockForDomesticWearCraftIndustrialisedClothingFootwearUniforms0015PlainSmock() => new()
	{
		Metadata = new("Clothing, footwear and uniforms/make plain cotton smock for domestic wear [craft_industrialised_clothing_footwear_uniforms_0015_plain_smock]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_clothing_footwear_uniforms_0015_plain_smock"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make plain cotton smock for domestic wear [craft_industrialised_clothing_footwear_uniforms_0015_plain_smock]",
		Category = "Clothing, footwear and uniforms",
		Blurb = "Make a plain cotton smock for domestic wear from prepared cotton stock.",
		Action = "making plain cotton smock for domestic wear",
		ActiveCraftItemSdesc = "an in-progress plain cotton smock for domestic wear craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 777.0, Material: "cotton", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("777 grams of cotton", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_clothing_footwear_uniforms_0015_plain_smock", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a plain cotton smock for domestic wear (@industrialised_clothing_footwear_uniforms_0015_plain_smock)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ClothingFootwearAndUniformsMakeReinforcedLinenRagForDomesticWearCraftIndustrialisedClothingFootwearUniforms0016ReinforcedRag() => new()
	{
		Metadata = new("Clothing, footwear and uniforms/make reinforced linen rag for domestic wear [craft_industrialised_clothing_footwear_uniforms_0016_reinforced_rag]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_clothing_footwear_uniforms_0016_reinforced_rag"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make reinforced linen rag for domestic wear [craft_industrialised_clothing_footwear_uniforms_0016_reinforced_rag]",
		Category = "Clothing, footwear and uniforms",
		Blurb = "Make a reinforced linen rag for domestic wear from prepared linen stock.",
		Action = "making reinforced linen rag for domestic wear",
		ActiveCraftItemSdesc = "an in-progress reinforced linen rag for domestic wear craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 817.0, Material: "linen", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("817 grams of linen", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_clothing_footwear_uniforms_0016_reinforced_rag", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a reinforced linen rag for domestic wear (@industrialised_clothing_footwear_uniforms_0016_reinforced_rag)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ClothingFootwearAndUniformsMakeReinforcedWoolApronForDomesticWearCraftIndustrialisedClothingFootwearUniforms0017ReinforcedApron() => new()
	{
		Metadata = new("Clothing, footwear and uniforms/make reinforced wool apron for domestic wear [craft_industrialised_clothing_footwear_uniforms_0017_reinforced_apron]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_clothing_footwear_uniforms_0017_reinforced_apron"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make reinforced wool apron for domestic wear [craft_industrialised_clothing_footwear_uniforms_0017_reinforced_apron]",
		Category = "Clothing, footwear and uniforms",
		Blurb = "Make a reinforced wool apron for domestic wear from prepared wool stock.",
		Action = "making reinforced wool apron for domestic wear",
		ActiveCraftItemSdesc = "an in-progress reinforced wool apron for domestic wear craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 858.0, Material: "wool", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("858 grams of wool", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_clothing_footwear_uniforms_0017_reinforced_apron", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a reinforced wool apron for domestic wear (@industrialised_clothing_footwear_uniforms_0017_reinforced_apron)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ClothingFootwearAndUniformsMakeReinforcedLeatherShirtForDomesticWearCraftIndustrialisedClothingFootwearUniforms0018ReinforcedShirt() => new()
	{
		Metadata = new("Clothing, footwear and uniforms/make reinforced leather shirt for domestic wear [craft_industrialised_clothing_footwear_uniforms_0018_reinforced_shirt]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_clothing_footwear_uniforms_0018_reinforced_shirt"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make reinforced leather shirt for domestic wear [craft_industrialised_clothing_footwear_uniforms_0018_reinforced_shirt]",
		Category = "Clothing, footwear and uniforms",
		Blurb = "Make a reinforced leather shirt for domestic wear from prepared leather stock.",
		Action = "making reinforced leather shirt for domestic wear",
		ActiveCraftItemSdesc = "an in-progress reinforced leather shirt for domestic wear craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 899.0, Material: "leather", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("899 grams of leather", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_clothing_footwear_uniforms_0018_reinforced_shirt", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a reinforced leather shirt for domestic wear (@industrialised_clothing_footwear_uniforms_0018_reinforced_shirt)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ClothingFootwearAndUniformsMakeSomeReinforcedCottonTrousersForDomesticWearCraftIndustrialisedClothingFootwearUniforms0019ReinforcedTrousers() => new()
	{
		Metadata = new("Clothing, footwear and uniforms/make some reinforced cotton trousers for domestic wear [craft_industrialised_clothing_footwear_uniforms_0019_reinforced_trousers]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_clothing_footwear_uniforms_0019_reinforced_trousers"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make some reinforced cotton trousers for domestic wear [craft_industrialised_clothing_footwear_uniforms_0019_reinforced_trousers]",
		Category = "Clothing, footwear and uniforms",
		Blurb = "Make some reinforced cotton trousers for domestic wear from prepared cotton stock.",
		Action = "making some reinforced cotton trousers for domestic wear",
		ActiveCraftItemSdesc = "an in-progress some reinforced cotton trousers for domestic wear craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 939.0, Material: "cotton", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("939 grams of cotton", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_clothing_footwear_uniforms_0019_reinforced_trousers", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x some reinforced cotton trousers for domestic wear (@industrialised_clothing_footwear_uniforms_0019_reinforced_trousers)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ClothingFootwearAndUniformsMakeSomeReinforcedLinenBootsForDomesticWearCraftIndustrialisedClothingFootwearUniforms0020ReinforcedBoots() => new()
	{
		Metadata = new("Clothing, footwear and uniforms/make some reinforced linen boots for domestic wear [craft_industrialised_clothing_footwear_uniforms_0020_reinforced_boots]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_clothing_footwear_uniforms_0020_reinforced_boots"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make some reinforced linen boots for domestic wear [craft_industrialised_clothing_footwear_uniforms_0020_reinforced_boots]",
		Category = "Clothing, footwear and uniforms",
		Blurb = "Make some reinforced linen boots for domestic wear from prepared linen stock.",
		Action = "making some reinforced linen boots for domestic wear",
		ActiveCraftItemSdesc = "an in-progress some reinforced linen boots for domestic wear craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 980.0, Material: "linen", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("980 grams of linen", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_clothing_footwear_uniforms_0020_reinforced_boots", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x some reinforced linen boots for domestic wear (@industrialised_clothing_footwear_uniforms_0020_reinforced_boots)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ClothingFootwearAndUniformsMakeReinforcedWoolDressForDomesticWearCraftIndustrialisedClothingFootwearUniforms0021ReinforcedDress() => new()
	{
		Metadata = new("Clothing, footwear and uniforms/make reinforced wool dress for domestic wear [craft_industrialised_clothing_footwear_uniforms_0021_reinforced_dress]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_clothing_footwear_uniforms_0021_reinforced_dress"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make reinforced wool dress for domestic wear [craft_industrialised_clothing_footwear_uniforms_0021_reinforced_dress]",
		Category = "Clothing, footwear and uniforms",
		Blurb = "Make a reinforced wool dress for domestic wear from prepared wool stock.",
		Action = "making reinforced wool dress for domestic wear",
		ActiveCraftItemSdesc = "an in-progress reinforced wool dress for domestic wear craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1021.0, Material: "wool", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1021 grams of wool", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_clothing_footwear_uniforms_0021_reinforced_dress", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a reinforced wool dress for domestic wear (@industrialised_clothing_footwear_uniforms_0021_reinforced_dress)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ClothingFootwearAndUniformsMakeReinforcedLeatherJacketForDomesticWearCraftIndustrialisedClothingFootwearUniforms0022ReinforcedJacket() => new()
	{
		Metadata = new("Clothing, footwear and uniforms/make reinforced leather jacket for domestic wear [craft_industrialised_clothing_footwear_uniforms_0022_reinforced_jacket]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_clothing_footwear_uniforms_0022_reinforced_jacket"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make reinforced leather jacket for domestic wear [craft_industrialised_clothing_footwear_uniforms_0022_reinforced_jacket]",
		Category = "Clothing, footwear and uniforms",
		Blurb = "Make a reinforced leather jacket for domestic wear from prepared leather stock.",
		Action = "making reinforced leather jacket for domestic wear",
		ActiveCraftItemSdesc = "an in-progress reinforced leather jacket for domestic wear craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1062.0, Material: "leather", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1062 grams of leather", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_clothing_footwear_uniforms_0022_reinforced_jacket", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a reinforced leather jacket for domestic wear (@industrialised_clothing_footwear_uniforms_0022_reinforced_jacket)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ClothingFootwearAndUniformsMakeReinforcedCottonHatForDomesticWearCraftIndustrialisedClothingFootwearUniforms0023ReinforcedHat() => new()
	{
		Metadata = new("Clothing, footwear and uniforms/make reinforced cotton hat for domestic wear [craft_industrialised_clothing_footwear_uniforms_0023_reinforced_hat]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_clothing_footwear_uniforms_0023_reinforced_hat"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make reinforced cotton hat for domestic wear [craft_industrialised_clothing_footwear_uniforms_0023_reinforced_hat]",
		Category = "Clothing, footwear and uniforms",
		Blurb = "Make a reinforced cotton hat for domestic wear from prepared cotton stock.",
		Action = "making reinforced cotton hat for domestic wear",
		ActiveCraftItemSdesc = "an in-progress reinforced cotton hat for domestic wear craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1102.0, Material: "cotton", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1102 grams of cotton", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_clothing_footwear_uniforms_0023_reinforced_hat", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a reinforced cotton hat for domestic wear (@industrialised_clothing_footwear_uniforms_0023_reinforced_hat)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ClothingFootwearAndUniformsMakeSomeReinforcedLinenGlovesForDomesticWearCraftIndustrialisedClothingFootwearUniforms0024ReinforcedGloves() => new()
	{
		Metadata = new("Clothing, footwear and uniforms/make some reinforced linen gloves for domestic wear [craft_industrialised_clothing_footwear_uniforms_0024_reinforced_gloves]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_clothing_footwear_uniforms_0024_reinforced_gloves"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make some reinforced linen gloves for domestic wear [craft_industrialised_clothing_footwear_uniforms_0024_reinforced_gloves]",
		Category = "Clothing, footwear and uniforms",
		Blurb = "Make some reinforced linen gloves for domestic wear from prepared linen stock.",
		Action = "making some reinforced linen gloves for domestic wear",
		ActiveCraftItemSdesc = "an in-progress some reinforced linen gloves for domestic wear craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1143.0, Material: "linen", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1143 grams of linen", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_clothing_footwear_uniforms_0024_reinforced_gloves", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x some reinforced linen gloves for domestic wear (@industrialised_clothing_footwear_uniforms_0024_reinforced_gloves)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ClothingFootwearAndUniformsMakeReinforcedWoolScarfForDomesticWearCraftIndustrialisedClothingFootwearUniforms0025ReinforcedScarf() => new()
	{
		Metadata = new("Clothing, footwear and uniforms/make reinforced wool scarf for domestic wear [craft_industrialised_clothing_footwear_uniforms_0025_reinforced_scarf]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_clothing_footwear_uniforms_0025_reinforced_scarf"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make reinforced wool scarf for domestic wear [craft_industrialised_clothing_footwear_uniforms_0025_reinforced_scarf]",
		Category = "Clothing, footwear and uniforms",
		Blurb = "Make a reinforced wool scarf for domestic wear from prepared wool stock.",
		Action = "making reinforced wool scarf for domestic wear",
		ActiveCraftItemSdesc = "an in-progress reinforced wool scarf for domestic wear craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1184.0, Material: "wool", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1184 grams of wool", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_clothing_footwear_uniforms_0025_reinforced_scarf", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a reinforced wool scarf for domestic wear (@industrialised_clothing_footwear_uniforms_0025_reinforced_scarf)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ClothingFootwearAndUniformsMakeReinforcedLeatherSkirtForDomesticWearCraftIndustrialisedClothingFootwearUniforms0026ReinforcedSkirt() => new()
	{
		Metadata = new("Clothing, footwear and uniforms/make reinforced leather skirt for domestic wear [craft_industrialised_clothing_footwear_uniforms_0026_reinforced_skirt]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_clothing_footwear_uniforms_0026_reinforced_skirt"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make reinforced leather skirt for domestic wear [craft_industrialised_clothing_footwear_uniforms_0026_reinforced_skirt]",
		Category = "Clothing, footwear and uniforms",
		Blurb = "Make a reinforced leather skirt for domestic wear from prepared leather stock.",
		Action = "making reinforced leather skirt for domestic wear",
		ActiveCraftItemSdesc = "an in-progress reinforced leather skirt for domestic wear craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1224.0, Material: "leather", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1224 grams of leather", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_clothing_footwear_uniforms_0026_reinforced_skirt", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a reinforced leather skirt for domestic wear (@industrialised_clothing_footwear_uniforms_0026_reinforced_skirt)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ClothingFootwearAndUniformsMakeSomeReinforcedCottonSocksForDomesticWearCraftIndustrialisedClothingFootwearUniforms0027ReinforcedSocks() => new()
	{
		Metadata = new("Clothing, footwear and uniforms/make some reinforced cotton socks for domestic wear [craft_industrialised_clothing_footwear_uniforms_0027_reinforced_socks]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_clothing_footwear_uniforms_0027_reinforced_socks"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make some reinforced cotton socks for domestic wear [craft_industrialised_clothing_footwear_uniforms_0027_reinforced_socks]",
		Category = "Clothing, footwear and uniforms",
		Blurb = "Make some reinforced cotton socks for domestic wear from prepared cotton stock.",
		Action = "making some reinforced cotton socks for domestic wear",
		ActiveCraftItemSdesc = "an in-progress some reinforced cotton socks for domestic wear craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1265.0, Material: "cotton", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1265 grams of cotton", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_clothing_footwear_uniforms_0027_reinforced_socks", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x some reinforced cotton socks for domestic wear (@industrialised_clothing_footwear_uniforms_0027_reinforced_socks)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ClothingFootwearAndUniformsMakeReinforcedLinenVestForDomesticWearCraftIndustrialisedClothingFootwearUniforms0028ReinforcedVest() => new()
	{
		Metadata = new("Clothing, footwear and uniforms/make reinforced linen vest for domestic wear [craft_industrialised_clothing_footwear_uniforms_0028_reinforced_vest]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_clothing_footwear_uniforms_0028_reinforced_vest"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make reinforced linen vest for domestic wear [craft_industrialised_clothing_footwear_uniforms_0028_reinforced_vest]",
		Category = "Clothing, footwear and uniforms",
		Blurb = "Make a reinforced linen vest for domestic wear from prepared linen stock.",
		Action = "making reinforced linen vest for domestic wear",
		ActiveCraftItemSdesc = "an in-progress reinforced linen vest for domestic wear craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1306.0, Material: "linen", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1306 grams of linen", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_clothing_footwear_uniforms_0028_reinforced_vest", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a reinforced linen vest for domestic wear (@industrialised_clothing_footwear_uniforms_0028_reinforced_vest)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ClothingFootwearAndUniformsMakeReinforcedWoolCoatForDomesticWearCraftIndustrialisedClothingFootwearUniforms0029ReinforcedCoat() => new()
	{
		Metadata = new("Clothing, footwear and uniforms/make reinforced wool coat for domestic wear [craft_industrialised_clothing_footwear_uniforms_0029_reinforced_coat]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_clothing_footwear_uniforms_0029_reinforced_coat"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make reinforced wool coat for domestic wear [craft_industrialised_clothing_footwear_uniforms_0029_reinforced_coat]",
		Category = "Clothing, footwear and uniforms",
		Blurb = "Make a reinforced wool coat for domestic wear from prepared wool stock.",
		Action = "making reinforced wool coat for domestic wear",
		ActiveCraftItemSdesc = "an in-progress reinforced wool coat for domestic wear craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1346.0, Material: "wool", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1346 grams of wool", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_clothing_footwear_uniforms_0029_reinforced_coat", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a reinforced wool coat for domestic wear (@industrialised_clothing_footwear_uniforms_0029_reinforced_coat)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ClothingFootwearAndUniformsMakeReinforcedLeatherSmockForDomesticWearCraftIndustrialisedClothingFootwearUniforms0030ReinforcedSmock() => new()
	{
		Metadata = new("Clothing, footwear and uniforms/make reinforced leather smock for domestic wear [craft_industrialised_clothing_footwear_uniforms_0030_reinforced_smock]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_clothing_footwear_uniforms_0030_reinforced_smock"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make reinforced leather smock for domestic wear [craft_industrialised_clothing_footwear_uniforms_0030_reinforced_smock]",
		Category = "Clothing, footwear and uniforms",
		Blurb = "Make a reinforced leather smock for domestic wear from prepared leather stock.",
		Action = "making reinforced leather smock for domestic wear",
		ActiveCraftItemSdesc = "an in-progress reinforced leather smock for domestic wear craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1387.0, Material: "leather", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1387 grams of leather", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_clothing_footwear_uniforms_0030_reinforced_smock", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a reinforced leather smock for domestic wear (@industrialised_clothing_footwear_uniforms_0030_reinforced_smock)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ClothingFootwearAndUniformsMakeCompactCottonRagForDomesticWearCraftIndustrialisedClothingFootwearUniforms0031CompactRag() => new()
	{
		Metadata = new("Clothing, footwear and uniforms/make compact cotton rag for domestic wear [craft_industrialised_clothing_footwear_uniforms_0031_compact_rag]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_clothing_footwear_uniforms_0031_compact_rag"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make compact cotton rag for domestic wear [craft_industrialised_clothing_footwear_uniforms_0031_compact_rag]",
		Category = "Clothing, footwear and uniforms",
		Blurb = "Make a compact cotton rag for domestic wear from prepared cotton stock.",
		Action = "making compact cotton rag for domestic wear",
		ActiveCraftItemSdesc = "an in-progress compact cotton rag for domestic wear craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1428.0, Material: "cotton", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1428 grams of cotton", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_clothing_footwear_uniforms_0031_compact_rag", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a compact cotton rag for domestic wear (@industrialised_clothing_footwear_uniforms_0031_compact_rag)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ClothingFootwearAndUniformsMakeCompactLinenApronForDomesticWearCraftIndustrialisedClothingFootwearUniforms0032CompactApron() => new()
	{
		Metadata = new("Clothing, footwear and uniforms/make compact linen apron for domestic wear [craft_industrialised_clothing_footwear_uniforms_0032_compact_apron]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_clothing_footwear_uniforms_0032_compact_apron"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make compact linen apron for domestic wear [craft_industrialised_clothing_footwear_uniforms_0032_compact_apron]",
		Category = "Clothing, footwear and uniforms",
		Blurb = "Make a compact linen apron for domestic wear from prepared linen stock.",
		Action = "making compact linen apron for domestic wear",
		ActiveCraftItemSdesc = "an in-progress compact linen apron for domestic wear craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1469.0, Material: "linen", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1469 grams of linen", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_clothing_footwear_uniforms_0032_compact_apron", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a compact linen apron for domestic wear (@industrialised_clothing_footwear_uniforms_0032_compact_apron)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ClothingFootwearAndUniformsMakeCompactWoolShirtForDomesticWearCraftIndustrialisedClothingFootwearUniforms0033CompactShirt() => new()
	{
		Metadata = new("Clothing, footwear and uniforms/make compact wool shirt for domestic wear [craft_industrialised_clothing_footwear_uniforms_0033_compact_shirt]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_clothing_footwear_uniforms_0033_compact_shirt"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make compact wool shirt for domestic wear [craft_industrialised_clothing_footwear_uniforms_0033_compact_shirt]",
		Category = "Clothing, footwear and uniforms",
		Blurb = "Make a compact wool shirt for domestic wear from prepared wool stock.",
		Action = "making compact wool shirt for domestic wear",
		ActiveCraftItemSdesc = "an in-progress compact wool shirt for domestic wear craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1509.0, Material: "wool", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1509 grams of wool", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_clothing_footwear_uniforms_0033_compact_shirt", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a compact wool shirt for domestic wear (@industrialised_clothing_footwear_uniforms_0033_compact_shirt)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ClothingFootwearAndUniformsMakeSomeCompactLeatherTrousersForDomesticWearCraftIndustrialisedClothingFootwearUniforms0034CompactTrousers() => new()
	{
		Metadata = new("Clothing, footwear and uniforms/make some compact leather trousers for domestic wear [craft_industrialised_clothing_footwear_uniforms_0034_compact_trousers]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_clothing_footwear_uniforms_0034_compact_trousers"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make some compact leather trousers for domestic wear [craft_industrialised_clothing_footwear_uniforms_0034_compact_trousers]",
		Category = "Clothing, footwear and uniforms",
		Blurb = "Make some compact leather trousers for domestic wear from prepared leather stock.",
		Action = "making some compact leather trousers for domestic wear",
		ActiveCraftItemSdesc = "an in-progress some compact leather trousers for domestic wear craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1550.0, Material: "leather", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1550 grams of leather", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_clothing_footwear_uniforms_0034_compact_trousers", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x some compact leather trousers for domestic wear (@industrialised_clothing_footwear_uniforms_0034_compact_trousers)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ClothingFootwearAndUniformsMakeSomeCompactCottonBootsForDomesticWearCraftIndustrialisedClothingFootwearUniforms0035CompactBoots() => new()
	{
		Metadata = new("Clothing, footwear and uniforms/make some compact cotton boots for domestic wear [craft_industrialised_clothing_footwear_uniforms_0035_compact_boots]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_clothing_footwear_uniforms_0035_compact_boots"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make some compact cotton boots for domestic wear [craft_industrialised_clothing_footwear_uniforms_0035_compact_boots]",
		Category = "Clothing, footwear and uniforms",
		Blurb = "Make some compact cotton boots for domestic wear from prepared cotton stock.",
		Action = "making some compact cotton boots for domestic wear",
		ActiveCraftItemSdesc = "an in-progress some compact cotton boots for domestic wear craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1591.0, Material: "cotton", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1591 grams of cotton", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_clothing_footwear_uniforms_0035_compact_boots", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x some compact cotton boots for domestic wear (@industrialised_clothing_footwear_uniforms_0035_compact_boots)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ClothingFootwearAndUniformsMakeCompactLinenDressForDomesticWearCraftIndustrialisedClothingFootwearUniforms0036CompactDress() => new()
	{
		Metadata = new("Clothing, footwear and uniforms/make compact linen dress for domestic wear [craft_industrialised_clothing_footwear_uniforms_0036_compact_dress]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_clothing_footwear_uniforms_0036_compact_dress"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make compact linen dress for domestic wear [craft_industrialised_clothing_footwear_uniforms_0036_compact_dress]",
		Category = "Clothing, footwear and uniforms",
		Blurb = "Make a compact linen dress for domestic wear from prepared linen stock.",
		Action = "making compact linen dress for domestic wear",
		ActiveCraftItemSdesc = "an in-progress compact linen dress for domestic wear craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1631.0, Material: "linen", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1631 grams of linen", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_clothing_footwear_uniforms_0036_compact_dress", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a compact linen dress for domestic wear (@industrialised_clothing_footwear_uniforms_0036_compact_dress)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ClothingFootwearAndUniformsMakeCompactWoolJacketForDomesticWearCraftIndustrialisedClothingFootwearUniforms0037CompactJacket() => new()
	{
		Metadata = new("Clothing, footwear and uniforms/make compact wool jacket for domestic wear [craft_industrialised_clothing_footwear_uniforms_0037_compact_jacket]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_clothing_footwear_uniforms_0037_compact_jacket"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make compact wool jacket for domestic wear [craft_industrialised_clothing_footwear_uniforms_0037_compact_jacket]",
		Category = "Clothing, footwear and uniforms",
		Blurb = "Make a compact wool jacket for domestic wear from prepared wool stock.",
		Action = "making compact wool jacket for domestic wear",
		ActiveCraftItemSdesc = "an in-progress compact wool jacket for domestic wear craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1672.0, Material: "wool", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1672 grams of wool", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_clothing_footwear_uniforms_0037_compact_jacket", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a compact wool jacket for domestic wear (@industrialised_clothing_footwear_uniforms_0037_compact_jacket)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ClothingFootwearAndUniformsMakeCompactLeatherHatForDomesticWearCraftIndustrialisedClothingFootwearUniforms0038CompactHat() => new()
	{
		Metadata = new("Clothing, footwear and uniforms/make compact leather hat for domestic wear [craft_industrialised_clothing_footwear_uniforms_0038_compact_hat]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_clothing_footwear_uniforms_0038_compact_hat"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make compact leather hat for domestic wear [craft_industrialised_clothing_footwear_uniforms_0038_compact_hat]",
		Category = "Clothing, footwear and uniforms",
		Blurb = "Make a compact leather hat for domestic wear from prepared leather stock.",
		Action = "making compact leather hat for domestic wear",
		ActiveCraftItemSdesc = "an in-progress compact leather hat for domestic wear craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1713.0, Material: "leather", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1713 grams of leather", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_clothing_footwear_uniforms_0038_compact_hat", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a compact leather hat for domestic wear (@industrialised_clothing_footwear_uniforms_0038_compact_hat)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ClothingFootwearAndUniformsMakeSomeCompactCottonGlovesForDomesticWearCraftIndustrialisedClothingFootwearUniforms0039CompactGloves() => new()
	{
		Metadata = new("Clothing, footwear and uniforms/make some compact cotton gloves for domestic wear [craft_industrialised_clothing_footwear_uniforms_0039_compact_gloves]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_clothing_footwear_uniforms_0039_compact_gloves"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make some compact cotton gloves for domestic wear [craft_industrialised_clothing_footwear_uniforms_0039_compact_gloves]",
		Category = "Clothing, footwear and uniforms",
		Blurb = "Make some compact cotton gloves for domestic wear from prepared cotton stock.",
		Action = "making some compact cotton gloves for domestic wear",
		ActiveCraftItemSdesc = "an in-progress some compact cotton gloves for domestic wear craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1753.0, Material: "cotton", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1753 grams of cotton", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_clothing_footwear_uniforms_0039_compact_gloves", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x some compact cotton gloves for domestic wear (@industrialised_clothing_footwear_uniforms_0039_compact_gloves)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ClothingFootwearAndUniformsMakeCompactLinenScarfForDomesticWearCraftIndustrialisedClothingFootwearUniforms0040CompactScarf() => new()
	{
		Metadata = new("Clothing, footwear and uniforms/make compact linen scarf for domestic wear [craft_industrialised_clothing_footwear_uniforms_0040_compact_scarf]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_clothing_footwear_uniforms_0040_compact_scarf"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make compact linen scarf for domestic wear [craft_industrialised_clothing_footwear_uniforms_0040_compact_scarf]",
		Category = "Clothing, footwear and uniforms",
		Blurb = "Make a compact linen scarf for domestic wear from prepared linen stock.",
		Action = "making compact linen scarf for domestic wear",
		ActiveCraftItemSdesc = "an in-progress compact linen scarf for domestic wear craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1794.0, Material: "linen", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1794 grams of linen", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_clothing_footwear_uniforms_0040_compact_scarf", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a compact linen scarf for domestic wear (@industrialised_clothing_footwear_uniforms_0040_compact_scarf)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ClothingFootwearAndUniformsMakeCompactWoolSkirtForDomesticWearCraftIndustrialisedClothingFootwearUniforms0041CompactSkirt() => new()
	{
		Metadata = new("Clothing, footwear and uniforms/make compact wool skirt for domestic wear [craft_industrialised_clothing_footwear_uniforms_0041_compact_skirt]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_clothing_footwear_uniforms_0041_compact_skirt"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make compact wool skirt for domestic wear [craft_industrialised_clothing_footwear_uniforms_0041_compact_skirt]",
		Category = "Clothing, footwear and uniforms",
		Blurb = "Make a compact wool skirt for domestic wear from prepared wool stock.",
		Action = "making compact wool skirt for domestic wear",
		ActiveCraftItemSdesc = "an in-progress compact wool skirt for domestic wear craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1835.0, Material: "wool", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1835 grams of wool", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_clothing_footwear_uniforms_0041_compact_skirt", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a compact wool skirt for domestic wear (@industrialised_clothing_footwear_uniforms_0041_compact_skirt)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};
}
