#nullable enable

using MudSharp.Body;
using MudSharp.GameItems.Inventory.Plans;
using MudSharp.RPG.Checks;

namespace DatabaseSeeder.Seeders;

public static partial class Recipes17Stock
{
	public static CatalogueRecipeDefinition CarpentryAndJoineryBuildNarrowShellFacedTeakLockFittedCupboard() => new()
	{
		Metadata = new("Carpentry and joinery/build narrow shell-faced teak lock-fitted cupboard", "crafts", ["renaissance"], ["item:renaissance_furniture_mea_lock_fitted_cupboard_01"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "build narrow shell-faced teak lock-fitted cupboard",
		Category = "Carpentry and joinery",
		Blurb = "build a narrow shell-faced teak lock-fitted cupboard",
		Action = "building a wooden item",
		ActiveCraftItemSdesc = "an in-progress narrow shell-faced teak lock-fitted cupboard craft",
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
			new(45, "$0 fit|fits the joins and secure|secures them with $t1. $0 clear|clears the work area before the final steps.", "$0 fit|fits the joins and secure|secures them with $t1. $0 clear|clears the work area before the final steps.", ExertionLevel.Stasis, 0),
			new(30, "$0 smooth|smooths the finished piece and set|sets aside $p1.", "$0 find|finds the join has opened and set|sets the material aside.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 27440.0, Material: "teak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("27440 grams of teak", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Saw", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Saw tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_furniture_mea_lock_fitted_cupboard_01", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a narrow shell-faced teak lock-fitted cupboard (@renaissance_furniture_mea_lock_fitted_cupboard_01)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition CarpentryAndJoineryBuildLacqueredBambooLockFittedWardrobe() => new()
	{
		Metadata = new("Carpentry and joinery/build lacquered bamboo lock-fitted wardrobe", "crafts", ["renaissance"], ["item:renaissance_furniture_mea_lock_fitted_wardrobe_02"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "build lacquered bamboo lock-fitted wardrobe",
		Category = "Carpentry and joinery",
		Blurb = "build a lacquered bamboo lock-fitted wardrobe",
		Action = "building a wooden item",
		ActiveCraftItemSdesc = "an in-progress lacquered bamboo lock-fitted wardrobe craft",
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
			new CatalogueCommodityRecipeInput(Grams: 36800.0, Material: "bamboo", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("36800 grams of bamboo", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Saw", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Saw tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_furniture_mea_lock_fitted_wardrobe_02", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a lacquered bamboo lock-fitted wardrobe (@renaissance_furniture_mea_lock_fitted_wardrobe_02)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition CarpentryAndJoineryBuildLowBrassMountedBambooManyDrawerCabinet() => new()
	{
		Metadata = new("Carpentry and joinery/build low brass-mounted bamboo many-drawer cabinet", "crafts", ["renaissance"], ["item:renaissance_furniture_mea_many_drawer_cabinet_02"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "build low brass-mounted bamboo many-drawer cabinet",
		Category = "Carpentry and joinery",
		Blurb = "build a low brass-mounted bamboo many-drawer cabinet",
		Action = "building a wooden item",
		ActiveCraftItemSdesc = "an in-progress low brass-mounted bamboo many-drawer cabinet craft",
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
			new(45, "$0 fit|fits the joins and secure|secures them with $t1. $0 clear|clears the work area before the final steps.", "$0 fit|fits the joins and secure|secures them with $t1. $0 clear|clears the work area before the final steps.", ExertionLevel.Stasis, 0),
			new(30, "$0 smooth|smooths the finished piece and set|sets aside $p1.", "$0 find|finds the join has opened and set|sets the material aside.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 34440.0, Material: "bamboo", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("34440 grams of bamboo", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Saw", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Saw tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_furniture_mea_many_drawer_cabinet_02", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a low brass-mounted bamboo many-drawer cabinet (@renaissance_furniture_mea_many_drawer_cabinet_02)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition CarpentryAndJoineryBuildLowCedarScrollDisplayCabinet() => new()
	{
		Metadata = new("Carpentry and joinery/build low cedar scroll display cabinet", "crafts", ["renaissance"], ["item:renaissance_furniture_mea_scroll_display_cabinet_01"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "build low cedar scroll display cabinet",
		Category = "Carpentry and joinery",
		Blurb = "build a low cedar scroll display cabinet",
		Action = "building a wooden item",
		ActiveCraftItemSdesc = "an in-progress low cedar scroll display cabinet craft",
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
			new(45, "$0 fit|fits the joins and secure|secures them with $t1. $0 clear|clears the work area before the final steps.", "$0 fit|fits the joins and secure|secures them with $t1. $0 clear|clears the work area before the final steps.", ExertionLevel.Stasis, 0),
			new(30, "$0 smooth|smooths the finished piece and set|sets aside $p1.", "$0 find|finds the join has opened and set|sets the material aside.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 22880.0, Material: "cedar", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("22880 grams of cedar", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Saw", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Saw tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_furniture_mea_scroll_display_cabinet_01", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a low cedar scroll display cabinet (@renaissance_furniture_mea_scroll_display_cabinet_01)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition GeneralCraftworkMakeLongShellFacedLacquerScrollDisplayCabinet() => new()
	{
		Metadata = new("General craftwork/make long shell-faced lacquer scroll display cabinet", "crafts", ["renaissance"], ["item:renaissance_furniture_mea_scroll_display_cabinet_02"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make long shell-faced lacquer scroll display cabinet",
		Category = "General craftwork",
		Blurb = "make a long shell-faced lacquer scroll display cabinet",
		Action = "making a finished item",
		ActiveCraftItemSdesc = "an in-progress long shell-faced lacquer scroll display cabinet craft",
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
			new CatalogueCommodityRecipeInput(Grams: 62025.0, Material: "lacquer", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("62025 grams of lacquer", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_furniture_mea_scroll_display_cabinet_02", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a long shell-faced lacquer scroll display cabinet (@renaissance_furniture_mea_scroll_display_cabinet_02)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition WritingAndAdministrationBindSmallSlattedBambooWritingDesk() => new()
	{
		Metadata = new("Writing and administration/bind small slatted bamboo writing desk", "crafts", ["renaissance"], ["item:renaissance_furniture_mea_writing_desk_01"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "bind small slatted bamboo writing desk",
		Category = "Writing and administration",
		Blurb = "bind a small slatted bamboo writing desk",
		Action = "binding a writing or administrative item",
		ActiveCraftItemSdesc = "an in-progress small slatted bamboo writing desk craft",
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
			new CatalogueCommodityRecipeInput(Grams: 9555.0, Material: "bamboo", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("9555 grams of bamboo", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Awl Punch", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Awl Punch tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Sewing Needle", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Sewing Needle tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_furniture_mea_writing_desk_01", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a small slatted bamboo writing desk (@renaissance_furniture_mea_writing_desk_01)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition CarpentryAndJoineryBuildTallWovenBasketStorageRack() => new()
	{
		Metadata = new("Carpentry and joinery/build tall woven-basket storage rack", "crafts", ["renaissance"], ["item:renaissance_furniture_mesoamerican_market_basket_rack"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "build tall woven-basket storage rack",
		Category = "Carpentry and joinery",
		Blurb = "build a tall woven-basket storage rack",
		Action = "building a wooden item",
		ActiveCraftItemSdesc = "an in-progress tall woven-basket storage rack craft",
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
			new CatalogueCommodityRecipeInput(Grams: 44000.0, Material: "wood", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("44000 grams of wood", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Saw", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Saw tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_furniture_mesoamerican_market_basket_rack", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a tall woven-basket storage rack (@renaissance_furniture_mesoamerican_market_basket_rack)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition CarpentryAndJoineryBuildLowBarkPaperCodexChest() => new()
	{
		Metadata = new("Carpentry and joinery/build low bark-paper codex chest", "crafts", ["renaissance"], ["item:renaissance_furniture_mesoamerican_market_codex_shelf_chest"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "build low bark-paper codex chest",
		Category = "Carpentry and joinery",
		Blurb = "build a low bark-paper codex chest",
		Action = "building a wooden item",
		ActiveCraftItemSdesc = "an in-progress low bark-paper codex chest craft",
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
			new CatalogueCommodityRecipeInput(Grams: 62400.0, Material: "wood", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("62400 grams of wood", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Saw", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Saw tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_furniture_mesoamerican_market_codex_shelf_chest", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a low bark-paper codex chest (@renaissance_furniture_mesoamerican_market_codex_shelf_chest)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition CarpentryAndJoineryBuildRaisedPotteryJarPlatform() => new()
	{
		Metadata = new("Carpentry and joinery/build raised pottery-jar platform", "crafts", ["renaissance"], ["item:renaissance_furniture_mesoamerican_market_jar_storage_platform"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "build raised pottery-jar platform",
		Category = "Carpentry and joinery",
		Blurb = "build a raised pottery-jar platform",
		Action = "building a wooden item",
		ActiveCraftItemSdesc = "an in-progress raised pottery-jar platform craft",
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
			new CatalogueCommodityRecipeInput(Grams: 44000.0, Material: "wood", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("44000 grams of wood", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Saw", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Saw tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_furniture_mesoamerican_market_jar_storage_platform", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a raised pottery-jar platform (@renaissance_furniture_mesoamerican_market_jar_storage_platform)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition CarpentryAndJoineryBuildRaisedMarketDisplayPlatform() => new()
	{
		Metadata = new("Carpentry and joinery/build raised market display platform", "crafts", ["renaissance"], ["item:renaissance_furniture_mesoamerican_market_platform"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "build raised market display platform",
		Category = "Carpentry and joinery",
		Blurb = "build a raised market display platform",
		Action = "building a wooden item",
		ActiveCraftItemSdesc = "an in-progress raised market display platform craft",
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
			new CatalogueCommodityRecipeInput(Grams: 46400.0, Material: "wood", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("46400 grams of wood", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Saw", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Saw tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_furniture_mesoamerican_market_platform", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a raised market display platform (@renaissance_furniture_mesoamerican_market_platform)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition CarpentryAndJoineryBuildManyBayedTributeStoreBinWall() => new()
	{
		Metadata = new("Carpentry and joinery/build many-bayed tribute-store bin wall", "crafts", ["renaissance"], ["item:renaissance_furniture_mesoamerican_market_tribute_bin_wall"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "build many-bayed tribute-store bin wall",
		Category = "Carpentry and joinery",
		Blurb = "build a many-bayed tribute-store bin wall",
		Action = "building a wooden item",
		ActiveCraftItemSdesc = "an in-progress many-bayed tribute-store bin wall craft",
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
			new CatalogueCommodityRecipeInput(Grams: 44000.0, Material: "wood", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("44000 grams of wood", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Saw", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Saw tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_furniture_mesoamerican_market_tribute_bin_wall", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a many-bayed tribute-store bin wall (@renaissance_furniture_mesoamerican_market_tribute_bin_wall)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition CeramicAndGlassWorkFormRoundFrontedRaisedEarthenwareArmourPlinth() => new()
	{
		Metadata = new("Ceramic and glass work/form round-fronted raised earthenware armour plinth", "crafts", ["renaissance"], ["item:renaissance_furniture_mes_armour_plinth_04"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "form round-fronted raised earthenware armour plinth",
		Category = "Ceramic and glass work",
		Blurb = "form a round-fronted raised earthenware armour plinth",
		Action = "forming a ceramic or glass item",
		ActiveCraftItemSdesc = "an in-progress round-fronted raised earthenware armour plinth craft",
		Gate = CatalogueRecipeGates.RenaissanceFinishedItemCraftingGlassworking15,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 3,
		Interruptable = false,
		Phases = [
			new(25, "$0 shape|shapes $i1 and warm|warms it at $t1.", "$0 shape|shapes $i1 and warm|warms it at $t1.", ExertionLevel.Stasis, 0),
			new(40, "$0 turn|turns the work carefully, tapping the form true with $t2. $0 turn|turns the work over to judge its balance.", "$0 turn|turns the work carefully, tapping the form true with $t2. $0 turn|turns the work over to judge its balance.", ExertionLevel.Stasis, 0),
			new(25, "$0 anneal|anneals the finished form and set|sets aside $p1.", "$0 find|finds the form has cracked and set|sets the material aside.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 66534.0, Material: "earthenware", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("66534 grams of earthenware", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hot Fire", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Hot Fire tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_furniture_mes_armour_plinth_04", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a round-fronted raised earthenware armour plinth (@renaissance_furniture_mes_armour_plinth_04)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition CarpentryAndJoineryBuildLowOpenworkWoodArmourTree() => new()
	{
		Metadata = new("Carpentry and joinery/build low openwork wood armour tree", "crafts", ["renaissance"], ["item:renaissance_furniture_mes_armour_tree_01"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "build low openwork wood armour tree",
		Category = "Carpentry and joinery",
		Blurb = "build a low openwork wood armour tree",
		Action = "building a wooden item",
		ActiveCraftItemSdesc = "an in-progress low openwork wood armour tree craft",
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
			new CatalogueCommodityRecipeInput(Grams: 17320.0, Material: "wood", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("17320 grams of wood", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Saw", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Saw tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_furniture_mes_armour_tree_01", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a low openwork wood armour tree (@renaissance_furniture_mes_armour_tree_01)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition CarpentryAndJoineryBuildTieredDeepCarvedWoodBookcase() => new()
	{
		Metadata = new("Carpentry and joinery/build tiered deep-carved wood bookcase", "crafts", ["renaissance"], ["item:renaissance_furniture_mes_bookcase_01"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "build tiered deep-carved wood bookcase",
		Category = "Carpentry and joinery",
		Blurb = "build a tiered deep-carved wood bookcase",
		Action = "building a wooden item",
		ActiveCraftItemSdesc = "an in-progress tiered deep-carved wood bookcase craft",
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
			new CatalogueCommodityRecipeInput(Grams: 65160.0, Material: "wood", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("65160 grams of wood", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Saw", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Saw tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_furniture_mes_bookcase_01", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a tiered deep-carved wood bookcase (@renaissance_furniture_mes_bookcase_01)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition CarpentryAndJoineryBuildLowPaintedCedarCampaignWeaponRack() => new()
	{
		Metadata = new("Carpentry and joinery/build low painted cedar campaign weapon rack", "crafts", ["renaissance"], ["item:renaissance_furniture_mes_campaign_weapon_rack_01"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "build low painted cedar campaign weapon rack",
		Category = "Carpentry and joinery",
		Blurb = "build a low painted cedar campaign weapon rack",
		Action = "building a wooden item",
		ActiveCraftItemSdesc = "an in-progress low painted cedar campaign weapon rack craft",
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
			new(45, "$0 fit|fits the joins and secure|secures them with $t1. $0 clear|clears the work area before the final steps.", "$0 fit|fits the joins and secure|secures them with $t1. $0 clear|clears the work area before the final steps.", ExertionLevel.Stasis, 0),
			new(30, "$0 smooth|smooths the finished piece and set|sets aside $p1.", "$0 find|finds the join has opened and set|sets the material aside.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 26880.0, Material: "cedar", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("26880 grams of cedar", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Saw", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Saw tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_furniture_mes_campaign_weapon_rack_01", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a low painted cedar campaign weapon rack (@renaissance_furniture_mes_campaign_weapon_rack_01)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition GeneralCraftworkMakePlainFramedStoneFoldingDisplayStand() => new()
	{
		Metadata = new("General craftwork/make plain-framed stone folding display stand", "crafts", ["renaissance"], ["item:renaissance_furniture_mes_folding_display_stand_02"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make plain-framed stone folding display stand",
		Category = "General craftwork",
		Blurb = "make a plain-framed stone folding display stand",
		Action = "making a finished item",
		ActiveCraftItemSdesc = "an in-progress plain-framed stone folding display stand craft",
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
			new CatalogueCommodityRecipeInput(Grams: 5813.0, Material: "stone", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("5813 grams of stone", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_furniture_mes_folding_display_stand_02", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a plain-framed stone folding display stand (@renaissance_furniture_mes_folding_display_stand_02)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition GeneralCraftworkMakeDeepReedBoundWickerHelmetAndCuirassStand() => new()
	{
		Metadata = new("General craftwork/make deep reed-bound wicker helmet-and-cuirass stand", "crafts", ["renaissance"], ["item:renaissance_furniture_mes_helmet_cuirass_stand_02"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make deep reed-bound wicker helmet-and-cuirass stand",
		Category = "General craftwork",
		Blurb = "make a deep reed-bound wicker helmet-and-cuirass stand",
		Action = "making a finished item",
		ActiveCraftItemSdesc = "an in-progress deep reed-bound wicker helmet-and-cuirass stand craft",
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
			new CatalogueCommodityRecipeInput(Grams: 19013.0, Material: "wicker", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("19013 grams of wicker", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_furniture_mes_helmet_cuirass_stand_02", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a deep reed-bound wicker helmet-and-cuirass stand (@renaissance_furniture_mes_helmet_cuirass_stand_02)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition GeneralCraftworkMakeDeepReedBoundReedLampTable() => new()
	{
		Metadata = new("General craftwork/make deep reed-bound reed lamp table", "crafts", ["renaissance"], ["item:renaissance_furniture_mes_lamp_table_01"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make deep reed-bound reed lamp table",
		Category = "General craftwork",
		Blurb = "make a deep reed-bound reed lamp table",
		Action = "making a finished item",
		ActiveCraftItemSdesc = "an in-progress deep reed-bound reed lamp table craft",
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
			new CatalogueCommodityRecipeInput(Grams: 6263.0, Material: "reed", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("6263 grams of reed", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_furniture_mes_lamp_table_01", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a deep reed-bound reed lamp table (@renaissance_furniture_mes_lamp_table_01)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition GeneralCraftworkMakeWovenPanelWickerOfferingStand() => new()
	{
		Metadata = new("General craftwork/make woven-panel wicker offering stand", "crafts", ["renaissance"], ["item:renaissance_furniture_mes_offering_stand_01"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make woven-panel wicker offering stand",
		Category = "General craftwork",
		Blurb = "make a woven-panel wicker offering stand",
		Action = "making a finished item",
		ActiveCraftItemSdesc = "an in-progress woven-panel wicker offering stand craft",
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
			new CatalogueCommodityRecipeInput(Grams: 2588.0, Material: "wicker", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2588 grams of wicker", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_furniture_mes_offering_stand_01", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a woven-panel wicker offering stand (@renaissance_furniture_mes_offering_stand_01)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition GeneralCraftworkMakeSteppedStoneOfferingStand() => new()
	{
		Metadata = new("General craftwork/make stepped stone offering stand", "crafts", ["renaissance"], ["item:renaissance_furniture_mes_offering_stand_03"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make stepped stone offering stand",
		Category = "General craftwork",
		Blurb = "make a stepped stone offering stand",
		Action = "making a finished item",
		ActiveCraftItemSdesc = "an in-progress stepped stone offering stand craft",
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
			new CatalogueCommodityRecipeInput(Grams: 27825.0, Material: "stone", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("27825 grams of stone", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_furniture_mes_offering_stand_03", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a stepped stone offering stand (@renaissance_furniture_mes_offering_stand_03)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition CarpentryAndJoineryBuildPaintedCedarSwordRack() => new()
	{
		Metadata = new("Carpentry and joinery/build painted cedar sword rack", "crafts", ["renaissance"], ["item:renaissance_furniture_mes_sword_rack_03"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "build painted cedar sword rack",
		Category = "Carpentry and joinery",
		Blurb = "build a painted cedar sword rack",
		Action = "building a wooden item",
		ActiveCraftItemSdesc = "an in-progress painted cedar sword rack craft",
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
			new(45, "$0 fit|fits the joins and secure|secures them with $t1. $0 clear|clears the work area before the final steps.", "$0 fit|fits the joins and secure|secures them with $t1. $0 clear|clears the work area before the final steps.", ExertionLevel.Stasis, 0),
			new(30, "$0 smooth|smooths the finished piece and set|sets aside $p1.", "$0 find|finds the join has opened and set|sets the material aside.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 14680.0, Material: "cedar", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("14680 grams of cedar", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Saw", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Saw tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_furniture_mes_sword_rack_03", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a painted cedar sword rack (@renaissance_furniture_mes_sword_rack_03)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition GeneralCraftworkMakeArchedTopSteppedReedWallWeaponRack() => new()
	{
		Metadata = new("General craftwork/make arched-top stepped reed wall weapon rack", "crafts", ["renaissance"], ["item:renaissance_furniture_mes_wall_weapon_rack_02"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make arched-top stepped reed wall weapon rack",
		Category = "General craftwork",
		Blurb = "make an arched-top stepped reed wall weapon rack",
		Action = "making a finished item",
		ActiveCraftItemSdesc = "an in-progress arched-top stepped reed wall weapon rack craft",
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
			new CatalogueCommodityRecipeInput(Grams: 10313.0, Material: "reed", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("10313 grams of reed", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_furniture_mes_wall_weapon_rack_02", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x an arched-top stepped reed wall weapon rack (@renaissance_furniture_mes_wall_weapon_rack_02)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition WritingAndAdministrationBindPlainFramedWoodWritingTable() => new()
	{
		Metadata = new("Writing and administration/bind plain-framed wood writing table", "crafts", ["renaissance"], ["item:renaissance_furniture_mes_writing_table_01"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "bind plain-framed wood writing table",
		Category = "Writing and administration",
		Blurb = "bind a plain-framed wood writing table",
		Action = "binding a writing or administrative item",
		ActiveCraftItemSdesc = "an in-progress plain-framed wood writing table craft",
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
			new CatalogueCommodityRecipeInput(Grams: 19500.0, Material: "wood", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("19500 grams of wood", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Awl Punch", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Awl Punch tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Sewing Needle", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Sewing Needle tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_furniture_mes_writing_table_01", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a plain-framed wood writing table (@renaissance_furniture_mes_writing_table_01)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition WritingAndAdministrationBindOctagonalRaisedWoodWritingTable() => new()
	{
		Metadata = new("Writing and administration/bind octagonal raised wood writing table", "crafts", ["renaissance"], ["item:renaissance_furniture_mes_writing_table_02"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "bind octagonal raised wood writing table",
		Category = "Writing and administration",
		Blurb = "bind an octagonal raised wood writing table",
		Action = "binding a writing or administrative item",
		ActiveCraftItemSdesc = "an in-progress octagonal raised wood writing table craft",
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
			new CatalogueCommodityRecipeInput(Grams: 17550.0, Material: "wood", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("17550 grams of wood", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Awl Punch", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Awl Punch tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Sewing Needle", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Sewing Needle tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_furniture_mes_writing_table_02", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x an octagonal raised wood writing table (@renaissance_furniture_mes_writing_table_02)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition GeneralCraftworkMakePaintedBrassArmourTree() => new()
	{
		Metadata = new("General craftwork/make painted brass armour tree", "crafts", ["renaissance"], ["item:renaissance_furniture_msea_armour_tree_01"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make painted brass armour tree",
		Category = "General craftwork",
		Blurb = "make a painted brass armour tree",
		Action = "making a finished item",
		ActiveCraftItemSdesc = "an in-progress painted brass armour tree craft",
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
			new CatalogueCommodityRecipeInput(Grams: 57713.0, Material: "brass", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("57713 grams of brass", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_furniture_msea_armour_tree_01", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a painted brass armour tree (@renaissance_furniture_msea_armour_tree_01)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition GeneralCraftworkMakePaintedLacquerCloakCupboard() => new()
	{
		Metadata = new("General craftwork/make painted lacquer cloak cupboard", "crafts", ["renaissance"], ["item:renaissance_furniture_msea_cloak_cupboard_02"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make painted lacquer cloak cupboard",
		Category = "General craftwork",
		Blurb = "make a painted lacquer cloak cupboard",
		Action = "making a finished item",
		ActiveCraftItemSdesc = "an in-progress painted lacquer cloak cupboard craft",
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
			new CatalogueCommodityRecipeInput(Grams: 27075.0, Material: "lacquer", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("27075 grams of lacquer", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_furniture_msea_cloak_cupboard_02", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a painted lacquer cloak cupboard (@renaissance_furniture_msea_cloak_cupboard_02)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MetalworkForgeDeepLacqueredBronzeCornerTable() => new()
	{
		Metadata = new("Metalwork/forge deep lacquered bronze corner table", "crafts", ["renaissance"], ["item:renaissance_furniture_msea_corner_table_03"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "forge deep lacquered bronze corner table",
		Category = "Metalwork",
		Blurb = "forge a deep lacquered bronze corner table",
		Action = "forging a metal item",
		ActiveCraftItemSdesc = "an in-progress deep lacquered bronze corner table craft",
		Gate = CatalogueRecipeGates.RenaissanceFinishedItemCraftingBlacksmithing15,
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
			new(30, "$0 heat|heats $i1 at $t1 and lift|lifts it with $t3.", "$0 heat|heats $i1 at $t1 and lift|lifts it with $t3.", ExertionLevel.Stasis, 0),
			new(45, "$0 strike|strikes the hot material against $t1 with $t2. $0 clear|clears the work area before the final steps.", "$0 strike|strikes the hot material against $t1 with $t2. $0 clear|clears the work area before the final steps.", ExertionLevel.Stasis, 0),
			new(30, "$0 cool|cools the finished work and set|sets aside $p1 for inspection.", "$0 find|finds the work twisted and set|sets the material aside.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 24600.0, Material: "bronze", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("24600 grams of bronze", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Anvil", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Anvil tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Forge Tongs", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Forge Tongs tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_furniture_msea_corner_table_03", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a deep lacquered bronze corner table (@renaissance_furniture_msea_corner_table_03)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition GeneralCraftworkMakeLowRattanCountingDesk() => new()
	{
		Metadata = new("General craftwork/make low rattan counting desk", "crafts", ["renaissance"], ["item:renaissance_furniture_msea_counting_desk_01"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make low rattan counting desk",
		Category = "General craftwork",
		Blurb = "make a low rattan counting desk",
		Action = "making a finished item",
		ActiveCraftItemSdesc = "an in-progress low rattan counting desk craft",
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
			new CatalogueCommodityRecipeInput(Grams: 12938.0, Material: "rattan", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("12938 grams of rattan", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_furniture_msea_counting_desk_01", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a low rattan counting desk (@renaissance_furniture_msea_counting_desk_01)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition GeneralCraftworkMakeArchedTopLacqueredRattanDrawerNightstand() => new()
	{
		Metadata = new("General craftwork/make arched-top lacquered rattan drawer nightstand", "crafts", ["renaissance"], ["item:renaissance_furniture_msea_drawer_nightstand_01"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make arched-top lacquered rattan drawer nightstand",
		Category = "General craftwork",
		Blurb = "make an arched-top lacquered rattan drawer nightstand",
		Action = "making a finished item",
		ActiveCraftItemSdesc = "an in-progress arched-top lacquered rattan drawer nightstand craft",
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
			new CatalogueCommodityRecipeInput(Grams: 1913.0, Material: "rattan", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1913 grams of rattan", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_furniture_msea_drawer_nightstand_01", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x an arched-top lacquered rattan drawer nightstand (@renaissance_furniture_msea_drawer_nightstand_01)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition GeneralCraftworkMakeLockFittedBroadOpenworkBrassEndTable() => new()
	{
		Metadata = new("General craftwork/make lock-fitted broad openwork brass end table", "crafts", ["renaissance"], ["item:renaissance_furniture_msea_end_table_02_lockable"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make lock-fitted broad openwork brass end table",
		Category = "General craftwork",
		Blurb = "make a lock-fitted broad openwork brass end table",
		Action = "making a finished item",
		ActiveCraftItemSdesc = "an in-progress lock-fitted broad openwork brass end table craft",
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
			new CatalogueCommodityRecipeInput(Grams: 23025.0, Material: "brass", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("23025 grams of brass", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_furniture_msea_end_table_02_lockable", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a lock-fitted broad openwork brass end table (@renaissance_furniture_msea_end_table_02_lockable)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition GeneralCraftworkMakeDeepOpenworkRattanFoldingDisplayStand() => new()
	{
		Metadata = new("General craftwork/make deep openwork rattan folding display stand", "crafts", ["renaissance"], ["item:renaissance_furniture_msea_folding_display_stand_01"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make deep openwork rattan folding display stand",
		Category = "General craftwork",
		Blurb = "make a deep openwork rattan folding display stand",
		Action = "making a finished item",
		ActiveCraftItemSdesc = "an in-progress deep openwork rattan folding display stand craft",
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
			new CatalogueCommodityRecipeInput(Grams: 2063.0, Material: "rattan", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2063 grams of rattan", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_furniture_msea_folding_display_stand_01", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a deep openwork rattan folding display stand (@renaissance_furniture_msea_folding_display_stand_01)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition GeneralCraftworkMakeCompactLacqueredAlabasterFoldingDisplayStand() => new()
	{
		Metadata = new("General craftwork/make compact lacquered alabaster folding display stand", "crafts", ["renaissance"], ["item:renaissance_furniture_msea_folding_display_stand_02"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make compact lacquered alabaster folding display stand",
		Category = "General craftwork",
		Blurb = "make a compact lacquered alabaster folding display stand",
		Action = "making a finished item",
		ActiveCraftItemSdesc = "an in-progress compact lacquered alabaster folding display stand craft",
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
			new CatalogueCommodityRecipeInput(Grams: 18188.0, Material: "alabaster", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("18188 grams of alabaster", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_furniture_msea_folding_display_stand_02", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a compact lacquered alabaster folding display stand (@renaissance_furniture_msea_folding_display_stand_02)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition GeneralCraftworkMakeDeepLowRattanHangingCabinet() => new()
	{
		Metadata = new("General craftwork/make deep low rattan hanging cabinet", "crafts", ["renaissance"], ["item:renaissance_furniture_msea_hanging_cabinet_02"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make deep low rattan hanging cabinet",
		Category = "General craftwork",
		Blurb = "make a deep low rattan hanging cabinet",
		Action = "making a finished item",
		ActiveCraftItemSdesc = "an in-progress deep low rattan hanging cabinet craft",
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
			new CatalogueCommodityRecipeInput(Grams: 4238.0, Material: "rattan", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("4238 grams of rattan", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_furniture_msea_hanging_cabinet_02", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a deep low rattan hanging cabinet (@renaissance_furniture_msea_hanging_cabinet_02)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MetalworkForgeShellInlaidBronzeHarnessStand() => new()
	{
		Metadata = new("Metalwork/forge shell-inlaid bronze harness stand", "crafts", ["renaissance"], ["item:renaissance_furniture_msea_harness_stand_02"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "forge shell-inlaid bronze harness stand",
		Category = "Metalwork",
		Blurb = "forge a shell-inlaid bronze harness stand",
		Action = "forging a metal item",
		ActiveCraftItemSdesc = "an in-progress shell-inlaid bronze harness stand craft",
		Gate = CatalogueRecipeGates.RenaissanceFinishedItemCraftingBlacksmithing15,
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
			new(30, "$0 heat|heats $i1 at $t1 and lift|lifts it with $t3.", "$0 heat|heats $i1 at $t1 and lift|lifts it with $t3.", ExertionLevel.Stasis, 0),
			new(45, "$0 strike|strikes the hot material against $t1 with $t2. $0 turn|turns the work over to judge its balance.", "$0 strike|strikes the hot material against $t1 with $t2. $0 turn|turns the work over to judge its balance.", ExertionLevel.Stasis, 0),
			new(30, "$0 cool|cools the finished work and set|sets aside $p1 for inspection.", "$0 find|finds the work twisted and set|sets the material aside.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 138047.0, Material: "bronze", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("138047 grams of bronze", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Anvil", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Anvil tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Forge Tongs", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Forge Tongs tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_furniture_msea_harness_stand_02", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a shell-inlaid bronze harness stand (@renaissance_furniture_msea_harness_stand_02)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition GeneralCraftworkMakeLowRattanLargeStorageCabinet() => new()
	{
		Metadata = new("General craftwork/make low rattan large storage cabinet", "crafts", ["renaissance"], ["item:renaissance_furniture_msea_large_storage_cabinet_01"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make low rattan large storage cabinet",
		Category = "General craftwork",
		Blurb = "make a low rattan large storage cabinet",
		Action = "making a finished item",
		ActiveCraftItemSdesc = "an in-progress low rattan large storage cabinet craft",
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
			new CatalogueCommodityRecipeInput(Grams: 68325.0, Material: "rattan", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("68325 grams of rattan", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_furniture_msea_large_storage_cabinet_01", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a low rattan large storage cabinet (@renaissance_furniture_msea_large_storage_cabinet_01)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition CarpentryAndJoineryBuildBroadShellInlaidTeakLowDrawerChest() => new()
	{
		Metadata = new("Carpentry and joinery/build broad shell-inlaid teak low drawer chest", "crafts", ["renaissance"], ["item:renaissance_furniture_msea_low_drawer_chest_01"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "build broad shell-inlaid teak low drawer chest",
		Category = "Carpentry and joinery",
		Blurb = "build a broad shell-inlaid teak low drawer chest",
		Action = "building a wooden item",
		ActiveCraftItemSdesc = "an in-progress broad shell-inlaid teak low drawer chest craft",
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
			new(45, "$0 fit|fits the joins and secure|secures them with $t1. $0 clear|clears the work area before the final steps.", "$0 fit|fits the joins and secure|secures them with $t1. $0 clear|clears the work area before the final steps.", ExertionLevel.Stasis, 0),
			new(30, "$0 smooth|smooths the finished piece and set|sets aside $p1.", "$0 find|finds the join has opened and set|sets the material aside.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 27800.0, Material: "teak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("27800 grams of teak", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Saw", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Saw tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_furniture_msea_low_drawer_chest_01", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a broad shell-inlaid teak low drawer chest (@renaissance_furniture_msea_low_drawer_chest_01)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition CarpentryAndJoineryBuildTallLacqueredTeakSpearRack() => new()
	{
		Metadata = new("Carpentry and joinery/build tall lacquered teak spear rack", "crafts", ["renaissance"], ["item:renaissance_furniture_msea_spear_rack_01"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "build tall lacquered teak spear rack",
		Category = "Carpentry and joinery",
		Blurb = "build a tall lacquered teak spear rack",
		Action = "building a wooden item",
		ActiveCraftItemSdesc = "an in-progress tall lacquered teak spear rack craft",
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
			new CatalogueCommodityRecipeInput(Grams: 43120.0, Material: "teak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("43120 grams of teak", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Saw", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Saw tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_furniture_msea_spear_rack_01", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a tall lacquered teak spear rack (@renaissance_furniture_msea_spear_rack_01)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition CarpentryAndJoineryBuildRoundFrontedLowBambooSteppedDisplayShelves() => new()
	{
		Metadata = new("Carpentry and joinery/build round-fronted low bamboo stepped display shelves", "crafts", ["renaissance"], ["item:renaissance_furniture_msea_stepped_display_shelves_01"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "build round-fronted low bamboo stepped display shelves",
		Category = "Carpentry and joinery",
		Blurb = "build a round-fronted low bamboo stepped display shelves",
		Action = "building a wooden item",
		ActiveCraftItemSdesc = "an in-progress round-fronted low bamboo stepped display shelves craft",
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
			new CatalogueCommodityRecipeInput(Grams: 30200.0, Material: "bamboo", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("30200 grams of bamboo", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Saw", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Saw tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_furniture_msea_stepped_display_shelves_01", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a round-fronted low bamboo stepped display shelves (@renaissance_furniture_msea_stepped_display_shelves_01)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition GeneralCraftworkMakeSlenderRattanFramedLacquerTallWardrobe() => new()
	{
		Metadata = new("General craftwork/make slender rattan-framed lacquer tall wardrobe", "crafts", ["renaissance"], ["item:renaissance_furniture_msea_tall_wardrobe_03"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make slender rattan-framed lacquer tall wardrobe",
		Category = "General craftwork",
		Blurb = "make a slender rattan-framed lacquer tall wardrobe",
		Action = "making a finished item",
		ActiveCraftItemSdesc = "an in-progress slender rattan-framed lacquer tall wardrobe craft",
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
			new CatalogueCommodityRecipeInput(Grams: 68813.0, Material: "lacquer", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("68813 grams of lacquer", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_furniture_msea_tall_wardrobe_03", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a slender rattan-framed lacquer tall wardrobe (@renaissance_furniture_msea_tall_wardrobe_03)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition CarpentryAndJoineryBuildShellInlaidBambooTwoDoorWardrobe() => new()
	{
		Metadata = new("Carpentry and joinery/build shell-inlaid bamboo two-door wardrobe", "crafts", ["renaissance"], ["item:renaissance_furniture_msea_two_door_wardrobe_01"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "build shell-inlaid bamboo two-door wardrobe",
		Category = "Carpentry and joinery",
		Blurb = "build a shell-inlaid bamboo two-door wardrobe",
		Action = "building a wooden item",
		ActiveCraftItemSdesc = "an in-progress shell-inlaid bamboo two-door wardrobe craft",
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
			new CatalogueCommodityRecipeInput(Grams: 70360.0, Material: "bamboo", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("70360 grams of bamboo", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Saw", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Saw tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_furniture_msea_two_door_wardrobe_01", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a shell-inlaid bamboo two-door wardrobe (@renaissance_furniture_msea_two_door_wardrobe_01)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition CarpentryAndJoineryBuildLowWovenPanelWoodArmourPlinth() => new()
	{
		Metadata = new("Carpentry and joinery/build low woven-panel wood armour plinth", "crafts", ["renaissance"], ["item:renaissance_furniture_nac_armour_plinth_01"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "build low woven-panel wood armour plinth",
		Category = "Carpentry and joinery",
		Blurb = "build a low woven-panel wood armour plinth",
		Action = "building a wooden item",
		ActiveCraftItemSdesc = "an in-progress low woven-panel wood armour plinth craft",
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
			new CatalogueCommodityRecipeInput(Grams: 46920.0, Material: "wood", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("46920 grams of wood", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Saw", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Saw tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_furniture_nac_armour_plinth_01", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a low woven-panel wood armour plinth (@renaissance_furniture_nac_armour_plinth_01)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition GeneralCraftworkMakeWovenPanelReedBowRack() => new()
	{
		Metadata = new("General craftwork/make woven-panel reed bow rack", "crafts", ["renaissance"], ["item:renaissance_furniture_nac_bow_rack_01"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make woven-panel reed bow rack",
		Category = "General craftwork",
		Blurb = "make a woven-panel reed bow rack",
		Action = "making a finished item",
		ActiveCraftItemSdesc = "an in-progress woven-panel reed bow rack craft",
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
			new CatalogueCommodityRecipeInput(Grams: 21675.0, Material: "reed", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("21675 grams of reed", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_furniture_nac_bow_rack_01", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a woven-panel reed bow rack (@renaissance_furniture_nac_bow_rack_01)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition GeneralCraftworkMakeTallHideCoveredBirchHangingCabinet() => new()
	{
		Metadata = new("General craftwork/make tall hide-covered birch hanging cabinet", "crafts", ["renaissance"], ["item:renaissance_furniture_nac_hanging_cabinet_02"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make tall hide-covered birch hanging cabinet",
		Category = "General craftwork",
		Blurb = "make a tall hide-covered birch hanging cabinet",
		Action = "making a finished item",
		ActiveCraftItemSdesc = "an in-progress tall hide-covered birch hanging cabinet craft",
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
			new CatalogueCommodityRecipeInput(Grams: 21488.0, Material: "birch", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("21488 grams of birch", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_furniture_nac_hanging_cabinet_02", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a tall hide-covered birch hanging cabinet (@renaissance_furniture_nac_hanging_cabinet_02)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition GeneralCraftworkMakeHideCoveredWickerHelmetAndCuirassStand() => new()
	{
		Metadata = new("General craftwork/make hide-covered wicker helmet-and-cuirass stand", "crafts", ["renaissance"], ["item:renaissance_furniture_nac_helmet_cuirass_stand_01"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make hide-covered wicker helmet-and-cuirass stand",
		Category = "General craftwork",
		Blurb = "make a hide-covered wicker helmet-and-cuirass stand",
		Action = "making a finished item",
		ActiveCraftItemSdesc = "an in-progress hide-covered wicker helmet-and-cuirass stand craft",
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
			new CatalogueCommodityRecipeInput(Grams: 7913.0, Material: "wicker", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("7913 grams of wicker", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_furniture_nac_helmet_cuirass_stand_01", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a hide-covered wicker helmet-and-cuirass stand (@renaissance_furniture_nac_helmet_cuirass_stand_01)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition GeneralCraftworkMakeCompactLashedWickerLampTable() => new()
	{
		Metadata = new("General craftwork/make compact lashed wicker lamp table", "crafts", ["renaissance"], ["item:renaissance_furniture_nac_lamp_table_02"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make compact lashed wicker lamp table",
		Category = "General craftwork",
		Blurb = "make a compact lashed wicker lamp table",
		Action = "making a finished item",
		ActiveCraftItemSdesc = "an in-progress compact lashed wicker lamp table craft",
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
			new CatalogueCommodityRecipeInput(Grams: 1800.0, Material: "wicker", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1800 grams of wicker", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_furniture_nac_lamp_table_02", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a compact lashed wicker lamp table (@renaissance_furniture_nac_lamp_table_02)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition GeneralCraftworkMakeLashedWickerOpenDisplayCabinet() => new()
	{
		Metadata = new("General craftwork/make lashed wicker open display cabinet", "crafts", ["renaissance"], ["item:renaissance_furniture_nac_open_display_cabinet_01"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make lashed wicker open display cabinet",
		Category = "General craftwork",
		Blurb = "make a lashed wicker open display cabinet",
		Action = "making a finished item",
		ActiveCraftItemSdesc = "an in-progress lashed wicker open display cabinet craft",
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
			new CatalogueCommodityRecipeInput(Grams: 12375.0, Material: "wicker", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("12375 grams of wicker", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_furniture_nac_open_display_cabinet_01", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a lashed wicker open display cabinet (@renaissance_furniture_nac_open_display_cabinet_01)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition GeneralCraftworkMakeTallDeepCarvedBirchOpenDisplayShelves() => new()
	{
		Metadata = new("General craftwork/make tall deep-carved birch open display shelves", "crafts", ["renaissance"], ["item:renaissance_furniture_nac_open_display_shelves_02"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make tall deep-carved birch open display shelves",
		Category = "General craftwork",
		Blurb = "make a tall deep-carved birch open display shelves",
		Action = "making a finished item",
		ActiveCraftItemSdesc = "an in-progress tall deep-carved birch open display shelves craft",
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
			new CatalogueCommodityRecipeInput(Grams: 53288.0, Material: "birch", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("53288 grams of birch", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_furniture_nac_open_display_shelves_02", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a tall deep-carved birch open display shelves (@renaissance_furniture_nac_open_display_shelves_02)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition GeneralCraftworkMakeTallBentwoodCaneSideTable() => new()
	{
		Metadata = new("General craftwork/make tall bentwood cane side table", "crafts", ["renaissance"], ["item:renaissance_furniture_nac_side_table_01"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make tall bentwood cane side table",
		Category = "General craftwork",
		Blurb = "make a tall bentwood cane side table",
		Action = "making a finished item",
		ActiveCraftItemSdesc = "an in-progress tall bentwood cane side table craft",
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
			new CatalogueCommodityRecipeInput(Grams: 7088.0, Material: "cane", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("7088 grams of cane", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_furniture_nac_side_table_01", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a tall bentwood cane side table (@renaissance_furniture_nac_side_table_01)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition CarpentryAndJoineryBuildLashedWoodSwordRack() => new()
	{
		Metadata = new("Carpentry and joinery/build lashed wood sword rack", "crafts", ["renaissance"], ["item:renaissance_furniture_nac_sword_rack_02"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "build lashed wood sword rack",
		Category = "Carpentry and joinery",
		Blurb = "build a lashed wood sword rack",
		Action = "building a wooden item",
		ActiveCraftItemSdesc = "an in-progress lashed wood sword rack craft",
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
			new(45, "$0 fit|fits the joins and secure|secures them with $t1. $0 clear|clears the work area before the final steps.", "$0 fit|fits the joins and secure|secures them with $t1. $0 clear|clears the work area before the final steps.", ExertionLevel.Stasis, 0),
			new(30, "$0 smooth|smooths the finished piece and set|sets aside $p1.", "$0 find|finds the join has opened and set|sets the material aside.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 17320.0, Material: "wood", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("17320 grams of wood", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Saw", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Saw tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_furniture_nac_sword_rack_02", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a lashed wood sword rack (@renaissance_furniture_nac_sword_rack_02)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition GeneralCraftworkMakeBarkPanelledWickerWallCupboard() => new()
	{
		Metadata = new("General craftwork/make bark-panelled wicker wall cupboard", "crafts", ["renaissance"], ["item:renaissance_furniture_nac_wall_cupboard_01"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make bark-panelled wicker wall cupboard",
		Category = "General craftwork",
		Blurb = "make a bark-panelled wicker wall cupboard",
		Action = "making a finished item",
		ActiveCraftItemSdesc = "an in-progress bark-panelled wicker wall cupboard craft",
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
			new CatalogueCommodityRecipeInput(Grams: 14288.0, Material: "wicker", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("14288 grams of wicker", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_furniture_nac_wall_cupboard_01", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a bark-panelled wicker wall cupboard (@renaissance_furniture_nac_wall_cupboard_01)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition GeneralCraftworkMakeCarvedPineBookcase() => new()
	{
		Metadata = new("General craftwork/make carved pine bookcase", "crafts", ["renaissance"], ["item:renaissance_furniture_nba_bookcase_01"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make carved pine bookcase",
		Category = "General craftwork",
		Blurb = "make a carved pine bookcase",
		Action = "making a finished item",
		ActiveCraftItemSdesc = "an in-progress carved pine bookcase craft",
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
			new CatalogueCommodityRecipeInput(Grams: 93450.0, Material: "pine", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("93450 grams of pine", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_furniture_nba_bookcase_01", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a carved pine bookcase (@renaissance_furniture_nba_bookcase_01)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition GeneralCraftworkMakeArchedTopSlattedBirchBowRack() => new()
	{
		Metadata = new("General craftwork/make arched-top slatted birch bow rack", "crafts", ["renaissance"], ["item:renaissance_furniture_nba_bow_rack_04"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make arched-top slatted birch bow rack",
		Category = "General craftwork",
		Blurb = "make an arched-top slatted birch bow rack",
		Action = "making a finished item",
		ActiveCraftItemSdesc = "an in-progress arched-top slatted birch bow rack craft",
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
			new CatalogueCommodityRecipeInput(Grams: 31463.0, Material: "birch", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("31463 grams of birch", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_furniture_nba_bow_rack_04", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x an arched-top slatted birch bow rack (@renaissance_furniture_nba_bow_rack_04)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition GeneralCraftworkMakeLongHeavyFramedSpruceChestOfDrawers() => new()
	{
		Metadata = new("General craftwork/make long heavy-framed spruce chest of drawers", "crafts", ["renaissance"], ["item:renaissance_furniture_nba_chest_of_drawers_02"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make long heavy-framed spruce chest of drawers",
		Category = "General craftwork",
		Blurb = "make a long heavy-framed spruce chest of drawers",
		Action = "making a finished item",
		ActiveCraftItemSdesc = "an in-progress long heavy-framed spruce chest of drawers craft",
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
			new CatalogueCommodityRecipeInput(Grams: 49650.0, Material: "spruce", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("49650 grams of spruce", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_furniture_nba_chest_of_drawers_02", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a long heavy-framed spruce chest of drawers (@renaissance_furniture_nba_chest_of_drawers_02)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition CarpentryAndJoineryBuildDeepPaleBoardedOakCountingDesk() => new()
	{
		Metadata = new("Carpentry and joinery/build deep pale-boarded oak counting desk", "crafts", ["renaissance"], ["item:renaissance_furniture_nba_counting_desk_02"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "build deep pale-boarded oak counting desk",
		Category = "Carpentry and joinery",
		Blurb = "build a deep pale-boarded oak counting desk",
		Action = "building a wooden item",
		ActiveCraftItemSdesc = "an in-progress deep pale-boarded oak counting desk craft",
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
			new CatalogueCommodityRecipeInput(Grams: 24560.0, Material: "oak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("24560 grams of oak", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Saw", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Saw tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_furniture_nba_counting_desk_02", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a deep pale-boarded oak counting desk (@renaissance_furniture_nba_counting_desk_02)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition CarpentryAndJoineryBuildDeepPaleBoardedOakFreestandingWeaponRack() => new()
	{
		Metadata = new("Carpentry and joinery/build deep pale-boarded oak freestanding weapon rack", "crafts", ["renaissance"], ["item:renaissance_furniture_nba_freestanding_weapon_rack_01"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "build deep pale-boarded oak freestanding weapon rack",
		Category = "Carpentry and joinery",
		Blurb = "build a deep pale-boarded oak freestanding weapon rack",
		Action = "building a wooden item",
		ActiveCraftItemSdesc = "an in-progress deep pale-boarded oak freestanding weapon rack craft",
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
			new CatalogueCommodityRecipeInput(Grams: 37520.0, Material: "oak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("37520 grams of oak", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Saw", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Saw tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_furniture_nba_freestanding_weapon_rack_01", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a deep pale-boarded oak freestanding weapon rack (@renaissance_furniture_nba_freestanding_weapon_rack_01)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition CarpentryAndJoineryBuildPaleBoardedAshHarnessStand() => new()
	{
		Metadata = new("Carpentry and joinery/build pale-boarded ash harness stand", "crafts", ["renaissance"], ["item:renaissance_furniture_nba_harness_stand_01"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "build pale-boarded ash harness stand",
		Category = "Carpentry and joinery",
		Blurb = "build a pale-boarded ash harness stand",
		Action = "building a wooden item",
		ActiveCraftItemSdesc = "an in-progress pale-boarded ash harness stand craft",
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
			new CatalogueCommodityRecipeInput(Grams: 64760.0, Material: "ash", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("64760 grams of ash", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Saw", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Saw tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_furniture_nba_harness_stand_01", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a pale-boarded ash harness stand (@renaissance_furniture_nba_harness_stand_01)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition CarpentryAndJoineryBuildHighPaleBoardedOakHouseholdCupboard() => new()
	{
		Metadata = new("Carpentry and joinery/build high pale-boarded oak household cupboard", "crafts", ["renaissance"], ["item:renaissance_furniture_nba_household_cupboard_02"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "build high pale-boarded oak household cupboard",
		Category = "Carpentry and joinery",
		Blurb = "build a high pale-boarded oak household cupboard",
		Action = "building a wooden item",
		ActiveCraftItemSdesc = "an in-progress high pale-boarded oak household cupboard craft",
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
			new CatalogueCommodityRecipeInput(Grams: 63000.0, Material: "oak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("63000 grams of oak", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Saw", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Saw tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_furniture_nba_household_cupboard_02", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a high pale-boarded oak household cupboard (@renaissance_furniture_nba_household_cupboard_02)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition CarpentryAndJoineryBuildPaleBoardedAshLinenPress() => new()
	{
		Metadata = new("Carpentry and joinery/build pale-boarded ash linen press", "crafts", ["renaissance"], ["item:renaissance_furniture_nba_linen_press_01"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "build pale-boarded ash linen press",
		Category = "Carpentry and joinery",
		Blurb = "build a pale-boarded ash linen press",
		Action = "building a wooden item",
		ActiveCraftItemSdesc = "an in-progress pale-boarded ash linen press craft",
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
			new CatalogueCommodityRecipeInput(Grams: 28080.0, Material: "ash", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("28080 grams of ash", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Saw", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Saw tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_furniture_nba_linen_press_01", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a pale-boarded ash linen press (@renaissance_furniture_nba_linen_press_01)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition GeneralCraftworkMakeMassiveStaveFacedPineLockFittedNightstand() => new()
	{
		Metadata = new("General craftwork/make massive stave-faced pine lock-fitted nightstand", "crafts", ["renaissance"], ["item:renaissance_furniture_nba_lock_fitted_nightstand_01"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make massive stave-faced pine lock-fitted nightstand",
		Category = "General craftwork",
		Blurb = "make a massive stave-faced pine lock-fitted nightstand",
		Action = "making a finished item",
		ActiveCraftItemSdesc = "an in-progress massive stave-faced pine lock-fitted nightstand craft",
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
			new CatalogueCommodityRecipeInput(Grams: 2663.0, Material: "pine", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2663 grams of pine", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_furniture_nba_lock_fitted_nightstand_01", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a massive stave-faced pine lock-fitted nightstand (@renaissance_furniture_nba_lock_fitted_nightstand_01)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MetalworkForgeCompactPlainJoinedWroughtIronLockFittedWeaponCabinet() => new()
	{
		Metadata = new("Metalwork/forge compact plain-joined wrought-iron lock-fitted weapon cabinet", "crafts", ["renaissance"], ["item:renaissance_furniture_nba_lock_fitted_weapon_cabinet_02"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "forge compact plain-joined wrought-iron lock-fitted weapon cabinet",
		Category = "Metalwork",
		Blurb = "forge a compact plain-joined wrought-iron lock-fitted weapon cabinet",
		Action = "forging a metal item",
		ActiveCraftItemSdesc = "an in-progress compact plain-joined wrought-iron lock-fitted weapon cabinet craft",
		Gate = CatalogueRecipeGates.RenaissanceFinishedItemCraftingBlacksmithing15,
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
			new(30, "$0 heat|heats $i1 at $t1 and lift|lifts it with $t3.", "$0 heat|heats $i1 at $t1 and lift|lifts it with $t3.", ExertionLevel.Stasis, 0),
			new(45, "$0 strike|strikes the hot material against $t1 with $t2. $0 clear|clears the work area before the final steps.", "$0 strike|strikes the hot material against $t1 with $t2. $0 clear|clears the work area before the final steps.", ExertionLevel.Stasis, 0),
			new(30, "$0 cool|cools the finished work and set|sets aside $p1 for inspection.", "$0 find|finds the work twisted and set|sets the material aside.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 79048.0, Material: "wrought iron", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("79048 grams of wrought iron", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Anvil", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Anvil tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Forge Tongs", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Forge Tongs tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_furniture_nba_lock_fitted_weapon_cabinet_02", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a compact plain-joined wrought-iron lock-fitted weapon cabinet (@renaissance_furniture_nba_lock_fitted_weapon_cabinet_02)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};
}
