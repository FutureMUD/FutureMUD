#nullable enable

using MudSharp.Body;
using MudSharp.GameItems.Inventory.Plans;
using MudSharp.RPG.Checks;

namespace DatabaseSeeder.Seeders;

public static partial class Recipes16Stock
{
	public static CatalogueRecipeDefinition MetalworkForgeBronzePalmWineLargeTransportJar() => new()
	{
		Metadata = new("Metalwork/forge bronze palm-wine large transport jar", "crafts", ["renaissance"], ["item:renaissance_container_msea_trade_large_transport_jar_03"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "forge bronze palm-wine large transport jar",
		Category = "Metalwork",
		Blurb = "forge a bronze palm-wine large transport jar",
		Action = "forging a metal item",
		ActiveCraftItemSdesc = "an in-progress bronze palm-wine large transport jar craft",
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
			new(45, "$0 strike|strikes the hot material against $t1 with $t2. $0 measure|measures the developing form against the intended finish.", "$0 strike|strikes the hot material against $t1 with $t2. $0 measure|measures the developing form against the intended finish.", ExertionLevel.Stasis, 0),
			new(30, "$0 cool|cools the finished work and set|sets aside $p1 for inspection.", "$0 find|finds the work twisted and set|sets the material aside.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 44321.0, Material: "bronze", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("44321 grams of bronze", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Anvil", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Anvil tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Forge Tongs", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Forge Tongs tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_container_msea_trade_large_transport_jar_03", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a bronze palm-wine large transport jar (@renaissance_container_msea_trade_large_transport_jar_03)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition CeramicAndGlassWorkFormGlassWaterSealedTransportJar() => new()
	{
		Metadata = new("Ceramic and glass work/form glass water sealed transport jar", "crafts", ["renaissance"], ["item:renaissance_container_msea_trade_sealed_transport_jar_01"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "form glass water sealed transport jar",
		Category = "Ceramic and glass work",
		Blurb = "form a glass water sealed transport jar",
		Action = "forming a ceramic or glass item",
		ActiveCraftItemSdesc = "an in-progress glass water sealed transport jar craft",
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
			new CatalogueCommodityRecipeInput(Grams: 14859.0, Material: "glass", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("14859 grams of glass", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hot Fire", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Hot Fire tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_container_msea_trade_sealed_transport_jar_01", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a glass water sealed transport jar (@renaissance_container_msea_trade_sealed_transport_jar_01)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MetalworkForgeIronBandedBronzeCeramicSampleTransitTrunk() => new()
	{
		Metadata = new("Metalwork/forge iron-banded bronze ceramic-sample transit trunk", "crafts", ["renaissance"], ["item:renaissance_container_msea_trade_transit_trunk_02"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "forge iron-banded bronze ceramic-sample transit trunk",
		Category = "Metalwork",
		Blurb = "forge an iron-banded bronze ceramic-sample transit trunk",
		Action = "forging a metal item",
		ActiveCraftItemSdesc = "an in-progress iron-banded bronze ceramic-sample transit trunk craft",
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
			new(45, "$0 strike|strikes the hot material against $t1 with $t2. $0 measure|measures the developing form against the intended finish.", "$0 strike|strikes the hot material against $t1 with $t2. $0 measure|measures the developing form against the intended finish.", ExertionLevel.Stasis, 0),
			new(30, "$0 cool|cools the finished work and set|sets aside $p1 for inspection.", "$0 find|finds the work twisted and set|sets the material aside.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 31135.0, Material: "bronze", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("31135 grams of bronze", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Anvil", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Anvil tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Forge Tongs", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Forge Tongs tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_container_msea_trade_transit_trunk_02", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x an iron-banded bronze ceramic-sample transit trunk (@renaissance_container_msea_trade_transit_trunk_02)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition CarpentryAndJoineryBuildCarvedWoodWaterHouseholdPot() => new()
	{
		Metadata = new("Carpentry and joinery/build carved wood water household pot", "crafts", ["renaissance"], ["item:renaissance_container_nac_liquid_household_pot_02"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "build carved wood water household pot",
		Category = "Carpentry and joinery",
		Blurb = "build a carved wood water household pot",
		Action = "building a wooden item",
		ActiveCraftItemSdesc = "an in-progress carved wood water household pot craft",
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
			new CatalogueCommodityRecipeInput(Grams: 7608.0, Material: "wood", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("7608 grams of wood", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Saw", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Saw tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_container_nac_liquid_household_pot_02", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a carved wood water household pot (@renaissance_container_nac_liquid_household_pot_02)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition CarpentryAndJoineryBuildPlainWoodWaterPouringEwer() => new()
	{
		Metadata = new("Carpentry and joinery/build plain wood water pouring ewer", "crafts", ["renaissance"], ["item:renaissance_container_nac_liquid_pouring_ewer_01"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "build plain wood water pouring ewer",
		Category = "Carpentry and joinery",
		Blurb = "build a plain wood water pouring ewer",
		Action = "building a wooden item",
		ActiveCraftItemSdesc = "an in-progress plain wood water pouring ewer craft",
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
			new CatalogueCommodityRecipeInput(Grams: 1768.0, Material: "wood", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1768 grams of wood", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Saw", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Saw tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_container_nac_liquid_pouring_ewer_01", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a plain wood water pouring ewer (@renaissance_container_nac_liquid_pouring_ewer_01)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition GeneralCraftworkMakeIncisedGourdShellOilWaterskinSling() => new()
	{
		Metadata = new("General craftwork/make incised gourd-shell oil waterskin sling", "crafts", ["renaissance"], ["item:renaissance_container_nac_personal_waterskin_sling_01"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make incised gourd-shell oil waterskin sling",
		Category = "General craftwork",
		Blurb = "make an incised gourd-shell oil waterskin sling",
		Action = "making a finished item",
		ActiveCraftItemSdesc = "an in-progress incised gourd-shell oil waterskin sling craft",
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
			new CatalogueCommodityRecipeInput(Grams: 248.0, Material: "gourd shell", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("248 grams of gourd shell", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_container_nac_personal_waterskin_sling_01", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x an incised gourd-shell oil waterskin sling (@renaissance_container_nac_personal_waterskin_sling_01)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition GeneralCraftworkMakeBirchTradeClothFittedSampleCase() => new()
	{
		Metadata = new("General craftwork/make birch trade-cloth fitted sample case", "crafts", ["renaissance"], ["item:renaissance_container_nac_trade_fitted_sample_case_01"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make birch trade-cloth fitted sample case",
		Category = "General craftwork",
		Blurb = "make a birch trade-cloth fitted sample case",
		Action = "making a finished item",
		ActiveCraftItemSdesc = "an in-progress birch trade-cloth fitted sample case craft",
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
			new CatalogueCommodityRecipeInput(Grams: 593.0, Material: "birch", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("593 grams of birch", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_container_nac_trade_fitted_sample_case_01", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a birch trade-cloth fitted sample case (@renaissance_container_nac_trade_fitted_sample_case_01)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition CarpentryAndJoineryBuildWoodShellBeadSegmentedPackingCrate() => new()
	{
		Metadata = new("Carpentry and joinery/build wood shell-bead segmented packing crate", "crafts", ["renaissance"], ["item:renaissance_container_nac_trade_segmented_packing_crate_02"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "build wood shell-bead segmented packing crate",
		Category = "Carpentry and joinery",
		Blurb = "build a wood shell-bead segmented packing crate",
		Action = "building a wooden item",
		ActiveCraftItemSdesc = "an in-progress wood shell-bead segmented packing crate craft",
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
			new CatalogueCommodityRecipeInput(Grams: 2768.0, Material: "wood", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2768 grams of wood", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Saw", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Saw tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_container_nac_trade_segmented_packing_crate_02", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a wood shell-bead segmented packing crate (@renaissance_container_nac_trade_segmented_packing_crate_02)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition GeneralCraftworkMakePlaitedWillowHallServiceBreadBasket() => new()
	{
		Metadata = new("General craftwork/make plaited willow hall-service bread basket", "crafts", ["renaissance"], ["item:renaissance_container_nba_domestic_bread_basket_01"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make plaited willow hall-service bread basket",
		Category = "General craftwork",
		Blurb = "make a plaited willow hall-service bread basket",
		Action = "making a finished item",
		ActiveCraftItemSdesc = "an in-progress plaited willow hall-service bread basket craft",
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
			new CatalogueCommodityRecipeInput(Grams: 833.0, Material: "willow", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("833 grams of willow", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_container_nba_domestic_bread_basket_01", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a plaited willow hall-service bread basket (@renaissance_container_nba_domestic_bread_basket_01)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition CeramicAndGlassWorkFormSaltGlazedEarthenwareWaterLargePitcher() => new()
	{
		Metadata = new("Ceramic and glass work/form salt-glazed earthenware water large pitcher", "crafts", ["renaissance"], ["item:renaissance_container_nba_liquid_large_pitcher_01"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "form salt-glazed earthenware water large pitcher",
		Category = "Ceramic and glass work",
		Blurb = "form a salt-glazed earthenware water large pitcher",
		Action = "forming a ceramic or glass item",
		ActiveCraftItemSdesc = "an in-progress salt-glazed earthenware water large pitcher craft",
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
			new CatalogueCommodityRecipeInput(Grams: 11365.0, Material: "earthenware", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("11365 grams of earthenware", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hot Fire", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Hot Fire tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_container_nba_liquid_large_pitcher_01", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a salt-glazed earthenware water large pitcher (@renaissance_container_nba_liquid_large_pitcher_01)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition TextileAndLeatherWorkTailorQuiltedLeatherWaxParcelShoulderSatchel() => new()
	{
		Metadata = new("Textile and leather work/tailor quilted leather wax-parcel shoulder satchel", "crafts", ["renaissance"], ["item:renaissance_container_nba_personal_shoulder_satchel_01"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "tailor quilted leather wax-parcel shoulder satchel",
		Category = "Textile and leather work",
		Blurb = "tailor a quilted leather wax-parcel shoulder satchel",
		Action = "tailoring a textile or leather item",
		ActiveCraftItemSdesc = "an in-progress quilted leather wax-parcel shoulder satchel craft",
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
			new CatalogueCommodityRecipeInput(Grams: 1403.0, Material: "leather", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1403 grams of leather", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Awl Punch", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Awl Punch tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Shears", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Shears tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_container_nba_personal_shoulder_satchel_01", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a quilted leather wax-parcel shoulder satchel (@renaissance_container_nba_personal_shoulder_satchel_01)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition CarpentryAndJoineryBuildOakShipPaperLockingTradeCoffer() => new()
	{
		Metadata = new("Carpentry and joinery/build oak ship-paper locking trade coffer", "crafts", ["renaissance"], ["item:renaissance_container_nba_trade_locking_trade_coffer_02"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "build oak ship-paper locking trade coffer",
		Category = "Carpentry and joinery",
		Blurb = "build an oak ship-paper locking trade coffer",
		Action = "building a wooden item",
		ActiveCraftItemSdesc = "an in-progress oak ship-paper locking trade coffer craft",
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
			new CatalogueCommodityRecipeInput(Grams: 9288.0, Material: "oak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("9288 grams of oak", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Saw", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Saw tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_container_nba_trade_locking_trade_coffer_02", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x an oak ship-paper locking trade coffer (@renaissance_container_nba_trade_locking_trade_coffer_02)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition GeneralCraftworkMakeCopperTarSampleStrongTradeChest() => new()
	{
		Metadata = new("General craftwork/make copper tar-sample strong trade chest", "crafts", ["renaissance"], ["item:renaissance_container_nba_trade_strong_trade_chest_01"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make copper tar-sample strong trade chest",
		Category = "General craftwork",
		Blurb = "make a copper tar-sample strong trade chest",
		Action = "making a finished item",
		ActiveCraftItemSdesc = "an in-progress copper tar-sample strong trade chest craft",
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
			new CatalogueCommodityRecipeInput(Grams: 22223.0, Material: "copper", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("22223 grams of copper", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_container_nba_trade_strong_trade_chest_01", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a copper tar-sample strong trade chest (@renaissance_container_nba_trade_strong_trade_chest_01)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition GeneralCraftworkMakePaintedBoxwoodCoffeeServiceServingTray() => new()
	{
		Metadata = new("General craftwork/make painted boxwood coffee-service serving tray", "crafts", ["renaissance"], ["item:renaissance_container_ott_domestic_serving_tray_01"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make painted boxwood coffee-service serving tray",
		Category = "General craftwork",
		Blurb = "make a painted boxwood coffee-service serving tray",
		Action = "making a finished item",
		ActiveCraftItemSdesc = "an in-progress painted boxwood coffee-service serving tray craft",
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
			new CatalogueCommodityRecipeInput(Grams: 510.0, Material: "boxwood", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("510 grams of boxwood", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_container_ott_domestic_serving_tray_01", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a painted boxwood coffee-service serving tray (@renaissance_container_ott_domestic_serving_tray_01)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition GeneralCraftworkMakeCopperRosewaterGreatStorageVat() => new()
	{
		Metadata = new("General craftwork/make copper rosewater great storage vat", "crafts", ["renaissance"], ["item:renaissance_container_ott_liquid_great_storage_vat_03"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make copper rosewater great storage vat",
		Category = "General craftwork",
		Blurb = "make a copper rosewater great storage vat",
		Action = "making a finished item",
		ActiveCraftItemSdesc = "an in-progress copper rosewater great storage vat craft",
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
			new CatalogueCommodityRecipeInput(Grams: 286125.0, Material: "copper", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("286125 grams of copper", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_container_ott_liquid_great_storage_vat_03", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a copper rosewater great storage vat (@renaissance_container_ott_liquid_great_storage_vat_03)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition CeramicAndGlassWorkFormMouldBlownGlassRosewaterHouseholdPot() => new()
	{
		Metadata = new("Ceramic and glass work/form mould-blown glass rosewater household pot", "crafts", ["renaissance"], ["item:renaissance_container_ott_liquid_household_pot_01"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "form mould-blown glass rosewater household pot",
		Category = "Ceramic and glass work",
		Blurb = "form a mould-blown glass rosewater household pot",
		Action = "forming a ceramic or glass item",
		ActiveCraftItemSdesc = "an in-progress mould-blown glass rosewater household pot craft",
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
			new(40, "$0 turn|turns the work carefully, tapping the form true with $t2. $0 clear|clears the work area before the final steps.", "$0 turn|turns the work carefully, tapping the form true with $t2. $0 clear|clears the work area before the final steps.", ExertionLevel.Stasis, 0),
			new(25, "$0 anneal|anneals the finished form and set|sets aside $p1.", "$0 find|finds the form has cracked and set|sets the material aside.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 10405.0, Material: "glass", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("10405 grams of glass", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hot Fire", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Hot Fire tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_container_ott_liquid_household_pot_01", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a mould-blown glass rosewater household pot (@renaissance_container_ott_liquid_household_pot_01)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition CeramicAndGlassWorkFormGlassSherbetMediumStorageJar() => new()
	{
		Metadata = new("Ceramic and glass work/form glass sherbet medium storage jar", "crafts", ["renaissance"], ["item:renaissance_container_ott_liquid_medium_storage_jar_02"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "form glass sherbet medium storage jar",
		Category = "Ceramic and glass work",
		Blurb = "form a glass sherbet medium storage jar",
		Action = "forming a ceramic or glass item",
		ActiveCraftItemSdesc = "an in-progress glass sherbet medium storage jar craft",
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
			new(40, "$0 turn|turns the work carefully, tapping the form true with $t2. $0 pause|pauses to check the proportions before carrying on.", "$0 turn|turns the work carefully, tapping the form true with $t2. $0 pause|pauses to check the proportions before carrying on.", ExertionLevel.Stasis, 0),
			new(25, "$0 anneal|anneals the finished form and set|sets aside $p1.", "$0 find|finds the form has cracked and set|sets the material aside.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 11848.0, Material: "glass", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("11848 grams of glass", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hot Fire", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Hot Fire tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_container_ott_liquid_medium_storage_jar_02", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a glass sherbet medium storage jar (@renaissance_container_ott_liquid_medium_storage_jar_02)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition WritingAndAdministrationBindEmbroideredLeatherPerfumeVialDocumentWallet() => new()
	{
		Metadata = new("Writing and administration/bind embroidered leather perfume-vial document wallet", "crafts", ["renaissance"], ["item:renaissance_container_ott_personal_document_wallet_01"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "bind embroidered leather perfume-vial document wallet",
		Category = "Writing and administration",
		Blurb = "bind an embroidered leather perfume-vial document wallet",
		Action = "binding a writing or administrative item",
		ActiveCraftItemSdesc = "an in-progress embroidered leather perfume-vial document wallet craft",
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
			new CatalogueCommodityRecipeInput(Grams: 312.0, Material: "leather", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("312 grams of leather", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Awl Punch", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Awl Punch tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Sewing Needle", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Sewing Needle tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_container_ott_personal_document_wallet_01", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x an embroidered leather perfume-vial document wallet (@renaissance_container_ott_personal_document_wallet_01)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition GeneralCraftworkMakeCypressChanceryPaperFittedBeltCase() => new()
	{
		Metadata = new("General craftwork/make cypress chancery-paper fitted belt case", "crafts", ["renaissance"], ["item:renaissance_container_ott_personal_fitted_belt_case_02"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make cypress chancery-paper fitted belt case",
		Category = "General craftwork",
		Blurb = "make a cypress chancery-paper fitted belt case",
		Action = "making a finished item",
		ActiveCraftItemSdesc = "an in-progress cypress chancery-paper fitted belt case craft",
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
			new CatalogueCommodityRecipeInput(Grams: 75.0, Material: "cypress", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("75 grams of cypress", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_container_ott_personal_fitted_belt_case_02", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a cypress chancery-paper fitted belt case (@renaissance_container_ott_personal_fitted_belt_case_02)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition GeneralCraftworkMakeWalnutCeramicSampleLargeTradeCask() => new()
	{
		Metadata = new("General craftwork/make walnut ceramic-sample large trade cask", "crafts", ["renaissance"], ["item:renaissance_container_ott_trade_large_trade_cask_01"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make walnut ceramic-sample large trade cask",
		Category = "General craftwork",
		Blurb = "make a walnut ceramic-sample large trade cask",
		Action = "making a finished item",
		ActiveCraftItemSdesc = "an in-progress walnut ceramic-sample large trade cask craft",
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
			new CatalogueCommodityRecipeInput(Grams: 12188.0, Material: "walnut", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("12188 grams of walnut", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_container_ott_trade_large_trade_cask_01", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a walnut ceramic-sample large trade cask (@renaissance_container_ott_trade_large_trade_cask_01)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition GeneralCraftworkMakeStonewareOilLargeTransportJar() => new()
	{
		Metadata = new("General craftwork/make stoneware oil large transport jar", "crafts", ["renaissance"], ["item:renaissance_container_ott_trade_large_transport_jar_04"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make stoneware oil large transport jar",
		Category = "General craftwork",
		Blurb = "make a stoneware oil large transport jar",
		Action = "making a finished item",
		ActiveCraftItemSdesc = "an in-progress stoneware oil large transport jar craft",
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
			new CatalogueCommodityRecipeInput(Grams: 32663.0, Material: "stoneware", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("32663 grams of stoneware", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_container_ott_trade_large_transport_jar_04", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a stoneware oil large transport jar (@renaissance_container_ott_trade_large_transport_jar_04)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition GeneralCraftworkMakeWickerChanceryPaperSegmentedPackingCrate() => new()
	{
		Metadata = new("General craftwork/make wicker chancery-paper segmented packing crate", "crafts", ["renaissance"], ["item:renaissance_container_ott_trade_segmented_packing_crate_02"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make wicker chancery-paper segmented packing crate",
		Category = "General craftwork",
		Blurb = "make a wicker chancery-paper segmented packing crate",
		Action = "making a finished item",
		ActiveCraftItemSdesc = "an in-progress wicker chancery-paper segmented packing crate craft",
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
			new CatalogueCommodityRecipeInput(Grams: 1448.0, Material: "wicker", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1448 grams of wicker", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_container_ott_trade_segmented_packing_crate_02", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a wicker chancery-paper segmented packing crate (@renaissance_container_ott_trade_segmented_packing_crate_02)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition GeneralCraftworkMakeReedPerfumeVialSegmentedPackingCrate() => new()
	{
		Metadata = new("General craftwork/make reed perfume-vial segmented packing crate", "crafts", ["renaissance"], ["item:renaissance_container_ott_trade_segmented_packing_crate_03"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make reed perfume-vial segmented packing crate",
		Category = "General craftwork",
		Blurb = "make a reed perfume-vial segmented packing crate",
		Action = "making a finished item",
		ActiveCraftItemSdesc = "an in-progress reed perfume-vial segmented packing crate craft",
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
			new CatalogueCommodityRecipeInput(Grams: 1313.0, Material: "reed", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1313 grams of reed", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_container_ott_trade_segmented_packing_crate_03", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a reed perfume-vial segmented packing crate (@renaissance_container_ott_trade_segmented_packing_crate_03)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition GeneralCraftworkMakeCopperHouseholdCompartmentedCondimentBox() => new()
	{
		Metadata = new("General craftwork/make copper household compartmented condiment box", "crafts", ["renaissance"], ["item:renaissance_container_pip_domestic_condiment_box_02"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make copper household compartmented condiment box",
		Category = "General craftwork",
		Blurb = "make a copper household compartmented condiment box",
		Action = "making a finished item",
		ActiveCraftItemSdesc = "an in-progress copper household compartmented condiment box craft",
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
			new CatalogueCommodityRecipeInput(Grams: 900.0, Material: "copper", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("900 grams of copper", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_container_pip_domestic_condiment_box_02", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a copper household compartmented condiment box (@renaissance_container_pip_domestic_condiment_box_02)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition CarpentryAndJoineryBuildCarvedCarvedCedarHouseholdServingTray() => new()
	{
		Metadata = new("Carpentry and joinery/build carved carved cedar household serving tray", "crafts", ["renaissance"], ["item:renaissance_container_pip_domestic_serving_tray_01"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "build carved carved cedar household serving tray",
		Category = "Carpentry and joinery",
		Blurb = "build a carved carved cedar household serving tray",
		Action = "building a wooden item",
		ActiveCraftItemSdesc = "an in-progress carved carved cedar household serving tray craft",
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
			new CatalogueCommodityRecipeInput(Grams: 1264.0, Material: "cedar", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1264 grams of cedar", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Saw", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Saw tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_container_pip_domestic_serving_tray_01", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a carved carved cedar household serving tray (@renaissance_container_pip_domestic_serving_tray_01)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition TextileAndLeatherWorkTailorWaxedLeatherSealMatrixBackPack() => new()
	{
		Metadata = new("Textile and leather work/tailor waxed leather seal-matrix back pack", "crafts", ["renaissance"], ["item:renaissance_container_pip_personal_back_pack_03"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "tailor waxed leather seal-matrix back pack",
		Category = "Textile and leather work",
		Blurb = "tailor a waxed leather seal-matrix back pack",
		Action = "tailoring a textile or leather item",
		ActiveCraftItemSdesc = "an in-progress waxed leather seal-matrix back pack craft",
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
			new CatalogueCommodityRecipeInput(Grams: 1485.0, Material: "leather", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1485 grams of leather", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Awl Punch", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Awl Punch tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Shears", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Shears tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_container_pip_personal_back_pack_03", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a waxed leather seal-matrix back pack (@renaissance_container_pip_personal_back_pack_03)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition TextileAndLeatherWorkTailorWaxedLeatherCarpetSampleShoulderSatchel() => new()
	{
		Metadata = new("Textile and leather work/tailor waxed leather carpet-sample shoulder satchel", "crafts", ["renaissance"], ["item:renaissance_container_pip_personal_shoulder_satchel_02"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "tailor waxed leather carpet-sample shoulder satchel",
		Category = "Textile and leather work",
		Blurb = "tailor a waxed leather carpet-sample shoulder satchel",
		Action = "tailoring a textile or leather item",
		ActiveCraftItemSdesc = "an in-progress waxed leather carpet-sample shoulder satchel craft",
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
			new CatalogueCommodityRecipeInput(Grams: 1658.0, Material: "leather", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1658 grams of leather", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Awl Punch", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Awl Punch tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Shears", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Shears tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_container_pip_personal_shoulder_satchel_02", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a waxed leather carpet-sample shoulder satchel (@renaissance_container_pip_personal_shoulder_satchel_02)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition TextileAndLeatherWorkTailorDoubledLeatherPigmentWaistPurse() => new()
	{
		Metadata = new("Textile and leather work/tailor doubled leather pigment waist purse", "crafts", ["renaissance"], ["item:renaissance_container_pip_personal_waist_purse_01"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "tailor doubled leather pigment waist purse",
		Category = "Textile and leather work",
		Blurb = "tailor a doubled leather pigment waist purse",
		Action = "tailoring a textile or leather item",
		ActiveCraftItemSdesc = "an in-progress doubled leather pigment waist purse craft",
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
			new CatalogueCommodityRecipeInput(Grams: 375.0, Material: "leather", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("375 grams of leather", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Awl Punch", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Awl Punch tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Shears", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Shears tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_container_pip_personal_waist_purse_01", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a doubled leather pigment waist purse (@renaissance_container_pip_personal_waist_purse_01)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition GeneralCraftworkMakeWickerScentVialDeepTradeHamper() => new()
	{
		Metadata = new("General craftwork/make wicker scent-vial deep trade hamper", "crafts", ["renaissance"], ["item:renaissance_container_pip_trade_deep_trade_hamper_03"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make wicker scent-vial deep trade hamper",
		Category = "General craftwork",
		Blurb = "make a wicker scent-vial deep trade hamper",
		Action = "making a finished item",
		ActiveCraftItemSdesc = "an in-progress wicker scent-vial deep trade hamper craft",
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
			new CatalogueCommodityRecipeInput(Grams: 5198.0, Material: "wicker", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("5198 grams of wicker", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_container_pip_trade_deep_trade_hamper_03", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a wicker scent-vial deep trade hamper (@renaissance_container_pip_trade_deep_trade_hamper_03)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition GeneralCraftworkMakeCopperCarpetSampleWaxSealableArchiveCoffer() => new()
	{
		Metadata = new("General craftwork/make copper carpet-sample wax-sealable archive coffer", "crafts", ["renaissance"], ["item:renaissance_container_pip_trade_sealable_archive_coffer_02"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make copper carpet-sample wax-sealable archive coffer",
		Category = "General craftwork",
		Blurb = "make a copper carpet-sample wax-sealable archive coffer",
		Action = "making a finished item",
		ActiveCraftItemSdesc = "an in-progress copper carpet-sample wax-sealable archive coffer craft",
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
			new CatalogueCommodityRecipeInput(Grams: 6135.0, Material: "copper", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("6135 grams of copper", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_container_pip_trade_sealable_archive_coffer_02", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a copper carpet-sample wax-sealable archive coffer (@renaissance_container_pip_trade_sealable_archive_coffer_02)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition CarpentryAndJoineryBuildLacqueredTeakPigmentTransitTrunk() => new()
	{
		Metadata = new("Carpentry and joinery/build lacquered teak pigment transit trunk", "crafts", ["renaissance"], ["item:renaissance_container_pip_trade_transit_trunk_01"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "build lacquered teak pigment transit trunk",
		Category = "Carpentry and joinery",
		Blurb = "build a lacquered teak pigment transit trunk",
		Action = "building a wooden item",
		ActiveCraftItemSdesc = "an in-progress lacquered teak pigment transit trunk craft",
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
			new CatalogueCommodityRecipeInput(Grams: 15320.0, Material: "teak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("15320 grams of teak", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Saw", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Saw tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_container_pip_trade_transit_trunk_01", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a lacquered teak pigment transit trunk (@renaissance_container_pip_trade_transit_trunk_01)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition GeneralCraftworkMakeBurnishedCopperWineStopperedFlask() => new()
	{
		Metadata = new("General craftwork/make burnished copper wine stoppered flask", "crafts", ["renaissance"], ["item:renaissance_container_rse_liquid_stoppered_flask_01"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make burnished copper wine stoppered flask",
		Category = "General craftwork",
		Blurb = "make a burnished copper wine stoppered flask",
		Action = "making a finished item",
		ActiveCraftItemSdesc = "an in-progress burnished copper wine stoppered flask craft",
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
			new CatalogueCommodityRecipeInput(Grams: 878.0, Material: "copper", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("878 grams of copper", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_container_rse_liquid_stoppered_flask_01", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a burnished copper wine stoppered flask (@renaissance_container_rse_liquid_stoppered_flask_01)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition GeneralCraftworkMakeBrassScentVialSampleRollCase() => new()
	{
		Metadata = new("General craftwork/make brass scent-vial sample roll case", "crafts", ["renaissance"], ["item:renaissance_container_rse_personal_sample_roll_case_01"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make brass scent-vial sample roll case",
		Category = "General craftwork",
		Blurb = "make a brass scent-vial sample roll case",
		Action = "making a finished item",
		ActiveCraftItemSdesc = "an in-progress brass scent-vial sample roll case craft",
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
			new CatalogueCommodityRecipeInput(Grams: 705.0, Material: "brass", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("705 grams of brass", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_container_rse_personal_sample_roll_case_01", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a brass scent-vial sample roll case (@renaissance_container_rse_personal_sample_roll_case_01)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition CarpentryAndJoineryBuildCedarSaltSampleLargeTradeCask() => new()
	{
		Metadata = new("Carpentry and joinery/build cedar salt-sample large trade cask", "crafts", ["renaissance"], ["item:renaissance_container_rse_trade_large_trade_cask_02"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "build cedar salt-sample large trade cask",
		Category = "Carpentry and joinery",
		Blurb = "build a cedar salt-sample large trade cask",
		Action = "building a wooden item",
		ActiveCraftItemSdesc = "an in-progress cedar salt-sample large trade cask craft",
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
			new CatalogueCommodityRecipeInput(Grams: 13056.0, Material: "cedar", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("13056 grams of cedar", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Saw", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Saw tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_container_rse_trade_large_trade_cask_02", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a cedar salt-sample large trade cask (@renaissance_container_rse_trade_large_trade_cask_02)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition GeneralCraftworkMakeHammeredCopperClothPatternSampleLockbox() => new()
	{
		Metadata = new("General craftwork/make hammered copper cloth-pattern sample lockbox", "crafts", ["renaissance"], ["item:renaissance_container_rse_trade_sample_lockbox_01"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make hammered copper cloth-pattern sample lockbox",
		Category = "General craftwork",
		Blurb = "make a hammered copper cloth-pattern sample lockbox",
		Action = "making a finished item",
		ActiveCraftItemSdesc = "an in-progress hammered copper cloth-pattern sample lockbox craft",
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
			new CatalogueCommodityRecipeInput(Grams: 1320.0, Material: "copper", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1320 grams of copper", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_container_rse_trade_sample_lockbox_01", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a hammered copper cloth-pattern sample lockbox (@renaissance_container_rse_trade_sample_lockbox_01)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition CarpentryAndJoineryBuildWoodIvoryParcelStrongTradeChest() => new()
	{
		Metadata = new("Carpentry and joinery/build wood ivory-parcel strong trade chest", "crafts", ["renaissance"], ["item:renaissance_container_rse_trade_strong_trade_chest_03"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "build wood ivory-parcel strong trade chest",
		Category = "Carpentry and joinery",
		Blurb = "build a wood ivory-parcel strong trade chest",
		Action = "building a wooden item",
		ActiveCraftItemSdesc = "an in-progress wood ivory-parcel strong trade chest craft",
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
			new CatalogueCommodityRecipeInput(Grams: 35720.0, Material: "wood", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("35720 grams of wood", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Saw", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Saw tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_container_rse_trade_strong_trade_chest_03", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a wood ivory-parcel strong trade chest (@renaissance_container_rse_trade_strong_trade_chest_03)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition CarpentryAndJoineryBuildCedarCaravanCompartmentedCondimentBox() => new()
	{
		Metadata = new("Carpentry and joinery/build cedar caravan compartmented condiment box", "crafts", ["renaissance"], ["item:renaissance_container_sai_domestic_condiment_box_02"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "build cedar caravan compartmented condiment box",
		Category = "Carpentry and joinery",
		Blurb = "build a cedar caravan compartmented condiment box",
		Action = "building a wooden item",
		ActiveCraftItemSdesc = "an in-progress cedar caravan compartmented condiment box craft",
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
			new CatalogueCommodityRecipeInput(Grams: 104.0, Material: "cedar", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("104 grams of cedar", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Saw", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Saw tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_container_sai_domestic_condiment_box_02", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a cedar caravan compartmented condiment box (@renaissance_container_sai_domestic_condiment_box_02)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MetalworkForgeWroughtIronCourtTableServiceBox() => new()
	{
		Metadata = new("Metalwork/forge wrought-iron court table service box", "crafts", ["renaissance"], ["item:renaissance_container_sai_domestic_table_service_box_01"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "forge wrought-iron court table service box",
		Category = "Metalwork",
		Blurb = "forge a wrought-iron court table service box",
		Action = "forging a metal item",
		ActiveCraftItemSdesc = "an in-progress wrought-iron court table service box craft",
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
			new(45, "$0 strike|strikes the hot material against $t1 with $t2. $0 measure|measures the developing form against the intended finish.", "$0 strike|strikes the hot material against $t1 with $t2. $0 measure|measures the developing form against the intended finish.", ExertionLevel.Stasis, 0),
			new(30, "$0 cool|cools the finished work and set|sets aside $p1 for inspection.", "$0 find|finds the work twisted and set|sets the material aside.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 894.0, Material: "wrought iron", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("894 grams of wrought iron", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Anvil", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Anvil tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Forge Tongs", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Forge Tongs tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_container_sai_domestic_table_service_box_01", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a wrought-iron court table service box (@renaissance_container_sai_domestic_table_service_box_01)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition CarpentryAndJoineryBuildWoodMedicineGreatStorageVat() => new()
	{
		Metadata = new("Carpentry and joinery/build wood medicine great storage vat", "crafts", ["renaissance"], ["item:renaissance_container_sai_liquid_great_storage_vat_01"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "build wood medicine great storage vat",
		Category = "Carpentry and joinery",
		Blurb = "build a wood medicine great storage vat",
		Action = "building a wooden item",
		ActiveCraftItemSdesc = "an in-progress wood medicine great storage vat craft",
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
			new CatalogueCommodityRecipeInput(Grams: 77720.0, Material: "wood", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("77720 grams of wood", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Saw", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Saw tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_container_sai_liquid_great_storage_vat_01", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a wood medicine great storage vat (@renaissance_container_sai_liquid_great_storage_vat_01)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition CarpentryAndJoineryBuildCedarSealMatrixSampleRollCase() => new()
	{
		Metadata = new("Carpentry and joinery/build cedar seal-matrix sample roll case", "crafts", ["renaissance"], ["item:renaissance_container_sai_personal_sample_roll_case_01"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "build cedar seal-matrix sample roll case",
		Category = "Carpentry and joinery",
		Blurb = "build a cedar seal-matrix sample roll case",
		Action = "building a wooden item",
		ActiveCraftItemSdesc = "an in-progress cedar seal-matrix sample roll case craft",
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
			new CatalogueCommodityRecipeInput(Grams: 336.0, Material: "cedar", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("336 grams of cedar", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Saw", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Saw tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_container_sai_personal_sample_roll_case_01", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a cedar seal-matrix sample roll case (@renaissance_container_sai_personal_sample_roll_case_01)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition CarpentryAndJoineryBuildCedarMilkLiquidTransportRundlet() => new()
	{
		Metadata = new("Carpentry and joinery/build cedar milk liquid transport rundlet", "crafts", ["renaissance"], ["item:renaissance_container_sai_trade_liquid_transport_rundlet_01"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "build cedar milk liquid transport rundlet",
		Category = "Carpentry and joinery",
		Blurb = "build a cedar milk liquid transport rundlet",
		Action = "building a wooden item",
		ActiveCraftItemSdesc = "an in-progress cedar milk liquid transport rundlet craft",
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
			new CatalogueCommodityRecipeInput(Grams: 8016.0, Material: "cedar", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("8016 grams of cedar", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Saw", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Saw tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_container_sai_trade_liquid_transport_rundlet_01", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a cedar milk liquid transport rundlet (@renaissance_container_sai_trade_liquid_transport_rundlet_01)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition CeramicAndGlassWorkFormIncisedEarthenwareHouseholdPlatter() => new()
	{
		Metadata = new("Ceramic and glass work/form incised earthenware household platter", "crafts", ["renaissance"], ["item:renaissance_container_sas_domestic_household_platter_01"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "form incised earthenware household platter",
		Category = "Ceramic and glass work",
		Blurb = "form an incised earthenware household platter",
		Action = "forming a ceramic or glass item",
		ActiveCraftItemSdesc = "an in-progress incised earthenware household platter craft",
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
			new(40, "$0 turn|turns the work carefully, tapping the form true with $t2. $0 measure|measures the developing form against the intended finish.", "$0 turn|turns the work carefully, tapping the form true with $t2. $0 measure|measures the developing form against the intended finish.", ExertionLevel.Stasis, 0),
			new(25, "$0 anneal|anneals the finished form and set|sets aside $p1.", "$0 find|finds the form has cracked and set|sets the material aside.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 772.0, Material: "earthenware", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("772 grams of earthenware", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hot Fire", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Hot Fire tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_container_sas_domestic_household_platter_01", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x an incised earthenware household platter (@renaissance_container_sas_domestic_household_platter_01)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition CarpentryAndJoineryBuildTeakOilGreatStorageVat() => new()
	{
		Metadata = new("Carpentry and joinery/build teak oil great storage vat", "crafts", ["renaissance"], ["item:renaissance_container_sas_liquid_great_storage_vat_04"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "build teak oil great storage vat",
		Category = "Carpentry and joinery",
		Blurb = "build a teak oil great storage vat",
		Action = "building a wooden item",
		ActiveCraftItemSdesc = "an in-progress teak oil great storage vat craft",
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
			new CatalogueCommodityRecipeInput(Grams: 86080.0, Material: "teak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("86080 grams of teak", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Saw", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Saw tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_container_sas_liquid_great_storage_vat_04", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a teak oil great storage vat (@renaissance_container_sas_liquid_great_storage_vat_04)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition CarpentryAndJoineryBuildLacqueredTeakWaterHouseholdRundlet() => new()
	{
		Metadata = new("Carpentry and joinery/build lacquered teak water household rundlet", "crafts", ["renaissance"], ["item:renaissance_container_sas_liquid_household_rundlet_02"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "build lacquered teak water household rundlet",
		Category = "Carpentry and joinery",
		Blurb = "build a lacquered teak water household rundlet",
		Action = "building a wooden item",
		ActiveCraftItemSdesc = "an in-progress lacquered teak water household rundlet craft",
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
			new CatalogueCommodityRecipeInput(Grams: 7312.0, Material: "teak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("7312 grams of teak", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Saw", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Saw tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_container_sas_liquid_household_rundlet_02", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a lacquered teak water household rundlet (@renaissance_container_sas_liquid_household_rundlet_02)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition GeneralCraftworkMakeBrassGheeSmallDrinkingCup() => new()
	{
		Metadata = new("General craftwork/make brass ghee small drinking cup", "crafts", ["renaissance"], ["item:renaissance_container_sas_liquid_small_drinking_cup_01"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make brass ghee small drinking cup",
		Category = "General craftwork",
		Blurb = "make a brass ghee small drinking cup",
		Action = "making a finished item",
		ActiveCraftItemSdesc = "an in-progress brass ghee small drinking cup craft",
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
			new CatalogueCommodityRecipeInput(Grams: 210.0, Material: "brass", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("210 grams of brass", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_container_sas_liquid_small_drinking_cup_01", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a brass ghee small drinking cup (@renaissance_container_sas_liquid_small_drinking_cup_01)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition CeramicAndGlassWorkFormGlassGheeSmallFootedCup() => new()
	{
		Metadata = new("Ceramic and glass work/form glass ghee small footed cup", "crafts", ["renaissance"], ["item:renaissance_container_sas_liquid_small_footed_cup_03"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "form glass ghee small footed cup",
		Category = "Ceramic and glass work",
		Blurb = "form a glass ghee small footed cup",
		Action = "forming a ceramic or glass item",
		ActiveCraftItemSdesc = "an in-progress glass ghee small footed cup craft",
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
			new(40, "$0 turn|turns the work carefully, tapping the form true with $t2. $0 pause|pauses to check the proportions before carrying on.", "$0 turn|turns the work carefully, tapping the form true with $t2. $0 pause|pauses to check the proportions before carrying on.", ExertionLevel.Stasis, 0),
			new(25, "$0 anneal|anneals the finished form and set|sets aside $p1.", "$0 find|finds the form has cracked and set|sets the material aside.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 62.0, Material: "glass", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("62 grams of glass", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hot Fire", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Hot Fire tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_container_sas_liquid_small_footed_cup_03", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a glass ghee small footed cup (@renaissance_container_sas_liquid_small_footed_cup_03)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition TextileAndLeatherWorkTailorCordedCottonSandalwoodShoulderBag() => new()
	{
		Metadata = new("Textile and leather work/tailor corded cotton sandalwood shoulder bag", "crafts", ["renaissance"], ["item:renaissance_container_sas_personal_shoulder_bag_01"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "tailor corded cotton sandalwood shoulder bag",
		Category = "Textile and leather work",
		Blurb = "tailor a corded cotton sandalwood shoulder bag",
		Action = "tailoring a textile or leather item",
		ActiveCraftItemSdesc = "an in-progress corded cotton sandalwood shoulder bag craft",
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
			new CatalogueCommodityRecipeInput(Grams: 720.0, Material: "cotton", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("720 grams of cotton", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Sewing Needle", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Sewing Needle tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Shears", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Shears tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_container_sas_personal_shoulder_bag_01", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a corded cotton sandalwood shoulder bag (@renaissance_container_sas_personal_shoulder_bag_01)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition CeramicAndGlassWorkFormGlassRiceWashLargeTransportJar() => new()
	{
		Metadata = new("Ceramic and glass work/form glass rice-wash large transport jar", "crafts", ["renaissance"], ["item:renaissance_container_sas_trade_large_transport_jar_02"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "form glass rice-wash large transport jar",
		Category = "Ceramic and glass work",
		Blurb = "form a glass rice-wash large transport jar",
		Action = "forming a ceramic or glass item",
		ActiveCraftItemSdesc = "an in-progress glass rice-wash large transport jar craft",
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
			new(40, "$0 turn|turns the work carefully, tapping the form true with $t2. $0 pause|pauses to check the proportions before carrying on.", "$0 turn|turns the work carefully, tapping the form true with $t2. $0 pause|pauses to check the proportions before carrying on.", ExertionLevel.Stasis, 0),
			new(25, "$0 anneal|anneals the finished form and set|sets aside $p1.", "$0 find|finds the form has cracked and set|sets the material aside.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 37362.0, Material: "glass", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("37362 grams of glass", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hot Fire", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Hot Fire tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_container_sas_trade_large_transport_jar_02", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a glass rice-wash large transport jar (@renaissance_container_sas_trade_large_transport_jar_02)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MetalworkForgeRivetedBronzeRiceMeasureTransitTrunk() => new()
	{
		Metadata = new("Metalwork/forge riveted bronze rice-measure transit trunk", "crafts", ["renaissance"], ["item:renaissance_container_sas_trade_transit_trunk_01"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "forge riveted bronze rice-measure transit trunk",
		Category = "Metalwork",
		Blurb = "forge a riveted bronze rice-measure transit trunk",
		Action = "forging a metal item",
		ActiveCraftItemSdesc = "an in-progress riveted bronze rice-measure transit trunk craft",
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
			new(45, "$0 strike|strikes the hot material against $t1 with $t2. $0 pause|pauses to check the proportions before carrying on.", "$0 strike|strikes the hot material against $t1 with $t2. $0 pause|pauses to check the proportions before carrying on.", ExertionLevel.Stasis, 0),
			new(30, "$0 cool|cools the finished work and set|sets aside $p1 for inspection.", "$0 find|finds the work twisted and set|sets the material aside.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 28511.0, Material: "bronze", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("28511 grams of bronze", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Anvil", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Anvil tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Forge Tongs", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Forge Tongs tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_container_sas_trade_transit_trunk_01", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a riveted bronze rice-measure transit trunk (@renaissance_container_sas_trade_transit_trunk_01)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition CarpentryAndJoineryBuildTeakHouseholdTableServiceBox() => new()
	{
		Metadata = new("Carpentry and joinery/build teak household table service box", "crafts", ["renaissance"], ["item:renaissance_container_sem_domestic_table_service_box_01"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "build teak household table service box",
		Category = "Carpentry and joinery",
		Blurb = "build a teak household table service box",
		Action = "building a wooden item",
		ActiveCraftItemSdesc = "an in-progress teak household table service box craft",
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
			new CatalogueCommodityRecipeInput(Grams: 1880.0, Material: "teak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1880 grams of teak", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Saw", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Saw tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_container_sem_domestic_table_service_box_01", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a teak household table service box (@renaissance_container_sem_domestic_table_service_box_01)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MetalworkForgeRivetedBronzeSyrupStopperedFlask() => new()
	{
		Metadata = new("Metalwork/forge riveted bronze syrup stoppered flask", "crafts", ["renaissance"], ["item:renaissance_container_sem_liquid_stoppered_flask_01"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "forge riveted bronze syrup stoppered flask",
		Category = "Metalwork",
		Blurb = "forge a riveted bronze syrup stoppered flask",
		Action = "forging a metal item",
		ActiveCraftItemSdesc = "an in-progress riveted bronze syrup stoppered flask craft",
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
			new(45, "$0 strike|strikes the hot material against $t1 with $t2. $0 measure|measures the developing form against the intended finish.", "$0 strike|strikes the hot material against $t1 with $t2. $0 measure|measures the developing form against the intended finish.", ExertionLevel.Stasis, 0),
			new(30, "$0 cool|cools the finished work and set|sets aside $p1 for inspection.", "$0 find|finds the work twisted and set|sets the material aside.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 968.0, Material: "bronze", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("968 grams of bronze", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Anvil", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Anvil tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Forge Tongs", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Forge Tongs tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_container_sem_liquid_stoppered_flask_01", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a riveted bronze syrup stoppered flask (@renaissance_container_sem_liquid_stoppered_flask_01)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition WritingAndAdministrationBindRattanSilkPatternPortableWritingBox() => new()
	{
		Metadata = new("Writing and administration/bind rattan silk-pattern portable writing box", "crafts", ["renaissance"], ["item:renaissance_container_sem_personal_portable_writing_box_01"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "bind rattan silk-pattern portable writing box",
		Category = "Writing and administration",
		Blurb = "bind a rattan silk-pattern portable writing box",
		Action = "binding a writing or administrative item",
		ActiveCraftItemSdesc = "an in-progress rattan silk-pattern portable writing box craft",
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
			new CatalogueCommodityRecipeInput(Grams: 702.0, Material: "rattan", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("702 grams of rattan", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Awl Punch", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Awl Punch tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Sewing Needle", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Sewing Needle tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_container_sem_personal_portable_writing_box_01", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a rattan silk-pattern portable writing box (@renaissance_container_sem_personal_portable_writing_box_01)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition CarpentryAndJoineryBuildBambooLacquerwareDeepTradeHamper() => new()
	{
		Metadata = new("Carpentry and joinery/build bamboo lacquerware deep trade hamper", "crafts", ["renaissance"], ["item:renaissance_container_sem_trade_deep_trade_hamper_01"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "build bamboo lacquerware deep trade hamper",
		Category = "Carpentry and joinery",
		Blurb = "build a bamboo lacquerware deep trade hamper",
		Action = "building a wooden item",
		ActiveCraftItemSdesc = "an in-progress bamboo lacquerware deep trade hamper craft",
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
			new CatalogueCommodityRecipeInput(Grams: 4896.0, Material: "bamboo", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("4896 grams of bamboo", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Saw", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Saw tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_container_sem_trade_deep_trade_hamper_01", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a bamboo lacquerware deep trade hamper (@renaissance_container_sem_trade_deep_trade_hamper_01)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition GeneralCraftworkMakeCopperWaterLargeTransportJar() => new()
	{
		Metadata = new("General craftwork/make copper water large transport jar", "crafts", ["renaissance"], ["item:renaissance_container_sem_trade_large_transport_jar_02"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make copper water large transport jar",
		Category = "General craftwork",
		Blurb = "make a copper water large transport jar",
		Action = "making a finished item",
		ActiveCraftItemSdesc = "an in-progress copper water large transport jar craft",
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
			new CatalogueCommodityRecipeInput(Grams: 37830.0, Material: "copper", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("37830 grams of copper", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_container_sem_trade_large_transport_jar_02", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a copper water large transport jar (@renaissance_container_sem_trade_large_transport_jar_02)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition GeneralCraftworkMakeBrassFermentedMilkLargeStorageJar() => new()
	{
		Metadata = new("General craftwork/make brass fermented milk large storage jar", "crafts", ["renaissance"], ["item:renaissance_container_stp_liquid_large_storage_jar_01"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make brass fermented milk large storage jar",
		Category = "General craftwork",
		Blurb = "make a brass fermented milk large storage jar",
		Action = "making a finished item",
		ActiveCraftItemSdesc = "an in-progress brass fermented milk large storage jar craft",
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
			new CatalogueCommodityRecipeInput(Grams: 29453.0, Material: "brass", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("29453 grams of brass", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_container_stp_liquid_large_storage_jar_01", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a brass fermented milk large storage jar (@renaissance_container_stp_liquid_large_storage_jar_01)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition GeneralCraftworkMakeCopperOilSmallDrinkingCup() => new()
	{
		Metadata = new("General craftwork/make copper oil small drinking cup", "crafts", ["renaissance"], ["item:renaissance_container_stp_liquid_small_drinking_cup_02"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make copper oil small drinking cup",
		Category = "General craftwork",
		Blurb = "make a copper oil small drinking cup",
		Action = "making a finished item",
		ActiveCraftItemSdesc = "an in-progress copper oil small drinking cup craft",
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
			new CatalogueCommodityRecipeInput(Grams: 210.0, Material: "copper", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("210 grams of copper", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_container_stp_liquid_small_drinking_cup_02", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a copper oil small drinking cup (@renaissance_container_stp_liquid_small_drinking_cup_02)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition GeneralCraftworkMakeReedBoundReedArrowFittingShoulderBasket() => new()
	{
		Metadata = new("General craftwork/make reed-bound reed arrow-fitting shoulder basket", "crafts", ["renaissance"], ["item:renaissance_container_stp_personal_shoulder_basket_02"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make reed-bound reed arrow-fitting shoulder basket",
		Category = "General craftwork",
		Blurb = "make a reed-bound reed arrow-fitting shoulder basket",
		Action = "making a finished item",
		ActiveCraftItemSdesc = "an in-progress reed-bound reed arrow-fitting shoulder basket craft",
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
			new CatalogueCommodityRecipeInput(Grams: 818.0, Material: "reed", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("818 grams of reed", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_container_stp_personal_shoulder_basket_02", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a reed-bound reed arrow-fitting shoulder basket (@renaissance_container_stp_personal_shoulder_basket_02)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition TextileAndLeatherWorkTailorHideReinforcedFeltMedicinePacketWaistPurse() => new()
	{
		Metadata = new("Textile and leather work/tailor hide-reinforced felt medicine-packet waist purse", "crafts", ["renaissance"], ["item:renaissance_container_stp_personal_waist_purse_01"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "tailor hide-reinforced felt medicine-packet waist purse",
		Category = "Textile and leather work",
		Blurb = "tailor a hide-reinforced felt medicine-packet waist purse",
		Action = "tailoring a textile or leather item",
		ActiveCraftItemSdesc = "an in-progress hide-reinforced felt medicine-packet waist purse craft",
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
			new CatalogueCommodityRecipeInput(Grams: 203.0, Material: "felt", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("203 grams of felt", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Sewing Needle", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Sewing Needle tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Shears", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Shears tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_container_stp_personal_waist_purse_01", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a hide-reinforced felt medicine-packet waist purse (@renaissance_container_stp_personal_waist_purse_01)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition GeneralCraftworkMakeBirchHorseGearLockingTradeCoffer() => new()
	{
		Metadata = new("General craftwork/make birch horse-gear locking trade coffer", "crafts", ["renaissance"], ["item:renaissance_container_stp_trade_locking_trade_coffer_01"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make birch horse-gear locking trade coffer",
		Category = "General craftwork",
		Blurb = "make a birch horse-gear locking trade coffer",
		Action = "making a finished item",
		ActiveCraftItemSdesc = "an in-progress birch horse-gear locking trade coffer craft",
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
			new CatalogueCommodityRecipeInput(Grams: 7313.0, Material: "birch", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("7313 grams of birch", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_container_stp_trade_locking_trade_coffer_01", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a birch horse-gear locking trade coffer (@renaissance_container_stp_trade_locking_trade_coffer_01)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodAndDrinkPrepareWillowBedchamberLiddedFoodBasket() => new()
	{
		Metadata = new("Food and drink/prepare willow bedchamber lidded food basket", "crafts", ["renaissance"], ["item:renaissance_container_wer_domestic_lidded_food_basket_01"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "prepare willow bedchamber lidded food basket",
		Category = "Food and drink",
		Blurb = "prepare a willow bedchamber lidded food basket",
		Action = "preparing a food or drink item",
		ActiveCraftItemSdesc = "an in-progress willow bedchamber lidded food basket craft",
		Gate = CatalogueRecipeGates.RenaissanceFinishedItemCraftingCooking10,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Easy,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 3,
		Interruptable = false,
		Phases = [
			new(25, "$0 sort|sorts and prepare|prepares $i1 beside $t1.", "$0 sort|sorts and prepare|prepares $i1 beside $t1.", ExertionLevel.Stasis, 0),
			new(40, "$0 tend|tends the heat and work|works the prepared material into its final form. $0 turn|turns the work over to judge its balance.", "$0 tend|tends the heat and work|works the prepared material into its final form. $0 turn|turns the work over to judge its balance.", ExertionLevel.Stasis, 0),
			new(25, "$0 portion|portions and set|sets aside $p1 for serving or storage.", "$0 find|finds the batch spoiled and set|sets the material aside.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 908.0, Material: "willow", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("908 grams of willow", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hot Fire", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Hot Fire tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_container_wer_domestic_lidded_food_basket_01", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a willow bedchamber lidded food basket (@renaissance_container_wer_domestic_lidded_food_basket_01)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};
}
