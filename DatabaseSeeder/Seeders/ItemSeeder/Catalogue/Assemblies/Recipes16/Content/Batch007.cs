#nullable enable

using MudSharp.Body;
using MudSharp.GameItems.Inventory.Plans;
using MudSharp.RPG.Checks;

namespace DatabaseSeeder.Seeders;

public static partial class Recipes16Stock
{
	public static CatalogueRecipeDefinition GeneralCraftworkMakeCompartmentedMerchantSSaltBox() => new()
	{
		Metadata = new("General craftwork/make compartmented merchant's salt box", "crafts", ["renaissance"], ["item:renaissance_domestic_western_counting_house_merchant_salt_box"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make compartmented merchant's salt box",
		Category = "General craftwork",
		Blurb = "make a compartmented merchant's salt box",
		Action = "making a finished item",
		ActiveCraftItemSdesc = "an in-progress compartmented merchant's salt box craft",
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
			new CatalogueCommodityRecipeInput(Grams: 413.0, Material: "boxwood", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("413 grams of boxwood", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_domestic_western_counting_house_merchant_salt_box", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a compartmented merchant's salt box (@renaissance_domestic_western_counting_house_merchant_salt_box)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition CeramicAndGlassWorkFormPaintedFaienceTableBowl() => new()
	{
		Metadata = new("Ceramic and glass work/form painted faience table bowl", "crafts", ["renaissance"], ["item:renaissance_domestic_western_counting_house_painted_bowl"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "form painted faience table bowl",
		Category = "Ceramic and glass work",
		Blurb = "form a painted faience table bowl",
		Action = "forming a ceramic or glass item",
		ActiveCraftItemSdesc = "an in-progress painted faience table bowl craft",
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
			new CatalogueCommodityRecipeInput(Grams: 468.0, Material: "faience", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("468 grams of faience", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hot Fire", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Hot Fire tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_domestic_western_counting_house_painted_bowl", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a painted faience table bowl (@renaissance_domestic_western_counting_house_painted_bowl)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition GeneralCraftworkMakeBroadPewterDiningPlate() => new()
	{
		Metadata = new("General craftwork/make broad pewter dining plate", "crafts", ["renaissance"], ["item:renaissance_domestic_western_counting_house_pewter_plate"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make broad pewter dining plate",
		Category = "General craftwork",
		Blurb = "make a broad pewter dining plate",
		Action = "making a finished item",
		ActiveCraftItemSdesc = "an in-progress broad pewter dining plate craft",
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
			new CatalogueCommodityRecipeInput(Grams: 735.0, Material: "pewter", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("735 grams of pewter", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_domestic_western_counting_house_pewter_plate", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a broad pewter dining plate (@renaissance_domestic_western_counting_house_pewter_plate)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition GeneralCraftworkMakeClearRibbedGlassBeaker() => new()
	{
		Metadata = new("General craftwork/make clear ribbed glass beaker", "crafts", ["renaissance"], ["item:renaissance_domestic_western_counting_house_venetian_beaker"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make clear ribbed glass beaker",
		Category = "General craftwork",
		Blurb = "make a clear ribbed glass beaker",
		Action = "making a finished item",
		ActiveCraftItemSdesc = "an in-progress clear ribbed glass beaker craft",
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
			new CatalogueCommodityRecipeInput(Grams: 233.0, Material: "soda-lime glass", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("233 grams of soda-lime glass", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_domestic_western_counting_house_venetian_beaker", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a clear ribbed glass beaker (@renaissance_domestic_western_counting_house_venetian_beaker)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition GeneralCraftworkMakeCastBrassCourtServingBowl() => new()
	{
		Metadata = new("General craftwork/make cast brass court serving bowl", "crafts", ["renaissance"], ["item:renaissance_domestic_west_african_court_brass_bowl"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make cast brass court serving bowl",
		Category = "General craftwork",
		Blurb = "make a cast brass court serving bowl",
		Action = "making a finished item",
		ActiveCraftItemSdesc = "an in-progress cast brass court serving bowl craft",
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
			new CatalogueCommodityRecipeInput(Grams: 600.0, Material: "brass", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("600 grams of brass", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_domestic_west_african_court_brass_bowl", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a cast brass court serving bowl (@renaissance_domestic_west_african_court_brass_bowl)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition GeneralCraftworkMakeHammeredAtlanticCopperEwer() => new()
	{
		Metadata = new("General craftwork/make hammered Atlantic copper ewer", "crafts", ["renaissance"], ["item:renaissance_domestic_west_african_court_copper_ewer"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make hammered Atlantic copper ewer",
		Category = "General craftwork",
		Blurb = "make a hammered Atlantic copper ewer",
		Action = "making a finished item",
		ActiveCraftItemSdesc = "an in-progress hammered Atlantic copper ewer craft",
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
			new CatalogueCommodityRecipeInput(Grams: 1725.0, Material: "copper", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1725 grams of copper", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_domestic_west_african_court_copper_ewer", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a hammered Atlantic copper ewer (@renaissance_domestic_west_african_court_copper_ewer)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition GeneralCraftworkMakePolishedCourtGourdBowl() => new()
	{
		Metadata = new("General craftwork/make polished court gourd bowl", "crafts", ["renaissance"], ["item:renaissance_domestic_west_african_court_gourd_bowl"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make polished court gourd bowl",
		Category = "General craftwork",
		Blurb = "make a polished court gourd bowl",
		Action = "making a finished item",
		ActiveCraftItemSdesc = "an in-progress polished court gourd bowl craft",
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
			new CatalogueCommodityRecipeInput(Grams: 143.0, Material: "gourd shell", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("143 grams of gourd shell", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_domestic_west_african_court_gourd_bowl", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a polished court gourd bowl (@renaissance_domestic_west_african_court_gourd_bowl)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition CarpentryAndJoineryBuildCarvedWestAfricanWoodenCup() => new()
	{
		Metadata = new("Carpentry and joinery/build carved West African wooden cup", "crafts", ["renaissance"], ["item:renaissance_domestic_west_african_court_wooden_cup"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "build carved West African wooden cup",
		Category = "Carpentry and joinery",
		Blurb = "build a carved West African wooden cup",
		Action = "building a wooden item",
		ActiveCraftItemSdesc = "an in-progress carved West African wooden cup craft",
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
			new CatalogueCommodityRecipeInput(Grams: 144.0, Material: "wood", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("144 grams of wood", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Saw", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Saw tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_domestic_west_african_court_wooden_cup", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a carved West African wooden cup (@renaissance_domestic_west_african_court_wooden_cup)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition CarpentryAndJoineryBuildBroadCarvedCourtPlatter() => new()
	{
		Metadata = new("Carpentry and joinery/build broad carved court platter", "crafts", ["renaissance"], ["item:renaissance_domestic_west_african_court_wooden_platter"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "build broad carved court platter",
		Category = "Carpentry and joinery",
		Blurb = "build a broad carved court platter",
		Action = "building a wooden item",
		ActiveCraftItemSdesc = "an in-progress broad carved court platter craft",
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
			new CatalogueCommodityRecipeInput(Grams: 1600.0, Material: "wood", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1600 grams of wood", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Saw", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Saw tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_domestic_west_african_court_wooden_platter", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a broad carved court platter (@renaissance_domestic_west_african_court_wooden_platter)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition GeneralCraftworkMakeCloseLiddedWovenFoodBasket() => new()
	{
		Metadata = new("General craftwork/make close-lidded woven food basket", "crafts", ["renaissance"], ["item:renaissance_domestic_west_african_court_woven_basket"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make close-lidded woven food basket",
		Category = "General craftwork",
		Blurb = "make a close-lidded woven food basket",
		Action = "making a finished item",
		ActiveCraftItemSdesc = "an in-progress close-lidded woven food basket craft",
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
			new CatalogueCommodityRecipeInput(Grams: 1275.0, Material: "wicker", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1275 grams of wicker", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_domestic_west_african_court_woven_basket", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a close-lidded woven food basket (@renaissance_domestic_west_african_court_woven_basket)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodAndDrinkPrepareAntimonyWineBottle() => new()
	{
		Metadata = new("Food and drink/prepare antimony wine bottle", "crafts", ["renaissance"], ["item:renaissance_drug_antimony_wine_bottle"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "prepare antimony wine bottle",
		Category = "Food and drink",
		Blurb = "prepare an antimony wine bottle",
		Action = "preparing a food or drink item",
		ActiveCraftItemSdesc = "an in-progress antimony wine bottle craft",
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
			new(40, "$0 tend|tends the heat and work|works the prepared material into its final form. $0 measure|measures the developing form against the intended finish.", "$0 tend|tends the heat and work|works the prepared material into its final form. $0 measure|measures the developing form against the intended finish.", ExertionLevel.Stasis, 0),
			new(25, "$0 portion|portions and set|sets aside $p1 for serving or storage.", "$0 find|finds the batch spoiled and set|sets the material aside.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 120.0, Material: "glass", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("120 grams of glass", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hot Fire", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Hot Fire tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_drug_antimony_wine_bottle", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x an antimony wine bottle (@renaissance_drug_antimony_wine_bottle)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MedicalAndRepairPrepareAquaVitaeWashBottle() => new()
	{
		Metadata = new("Medical and repair/prepare aqua vitae wash bottle", "crafts", ["renaissance"], ["item:renaissance_drug_aqua_vitae_wash_bottle"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "prepare aqua vitae wash bottle",
		Category = "Medical and repair",
		Blurb = "prepare an aqua vitae wash bottle",
		Action = "preparing a medical or repair item",
		ActiveCraftItemSdesc = "an in-progress aqua vitae wash bottle craft",
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
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_drug_aqua_vitae_wash_bottle", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x an aqua vitae wash bottle (@renaissance_drug_aqua_vitae_wash_bottle)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MedicalAndRepairPrepareBenzoinFumigantCone() => new()
	{
		Metadata = new("Medical and repair/prepare benzoin fumigant cone", "crafts", ["renaissance"], ["item:renaissance_drug_benzoin_fumigant_cone"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "prepare benzoin fumigant cone",
		Category = "Medical and repair",
		Blurb = "prepare a benzoin fumigant cone",
		Action = "preparing a medical or repair item",
		ActiveCraftItemSdesc = "an in-progress benzoin fumigant cone craft",
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
			new CatalogueCommodityRecipeInput(Grams: 32.0, Material: "benzoin resin", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("32 grams of benzoin resin", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Shears", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Shears tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Awl Punch", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Awl Punch tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_drug_benzoin_fumigant_cone", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a benzoin fumigant cone (@renaissance_drug_benzoin_fumigant_cone)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MedicalAndRepairPrepareCamphoratedBalmPot() => new()
	{
		Metadata = new("Medical and repair/prepare camphorated balm pot", "crafts", ["renaissance"], ["item:renaissance_drug_camphorated_balm_pot"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "prepare camphorated balm pot",
		Category = "Medical and repair",
		Blurb = "prepare a camphorated balm pot",
		Action = "preparing a medical or repair item",
		ActiveCraftItemSdesc = "an in-progress camphorated balm pot craft",
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
			new CatalogueCommodityRecipeInput(Grams: 67.0, Material: "beeswax", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("67 grams of beeswax", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Shears", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Shears tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Awl Punch", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Awl Punch tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_drug_camphorated_balm_pot", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a camphorated balm pot (@renaissance_drug_camphorated_balm_pot)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MedicalAndRepairPrepareGuaiacumDecoctionFlask() => new()
	{
		Metadata = new("Medical and repair/prepare guaiacum decoction flask", "crafts", ["renaissance"], ["item:renaissance_drug_guaiacum_decoction_flask"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "prepare guaiacum decoction flask",
		Category = "Medical and repair",
		Blurb = "prepare a guaiacum decoction flask",
		Action = "preparing a medical or repair item",
		ActiveCraftItemSdesc = "an in-progress guaiacum decoction flask craft",
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
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_drug_guaiacum_decoction_flask", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a guaiacum decoction flask (@renaissance_drug_guaiacum_decoction_flask)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MedicalAndRepairPrepareMercurialOintmentPot() => new()
	{
		Metadata = new("Medical and repair/prepare mercurial ointment pot", "crafts", ["renaissance"], ["item:renaissance_drug_mercurial_ointment_pot"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "prepare mercurial ointment pot",
		Category = "Medical and repair",
		Blurb = "prepare a mercurial ointment pot",
		Action = "preparing a medical or repair item",
		ActiveCraftItemSdesc = "an in-progress mercurial ointment pot craft",
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
			new CatalogueCommodityRecipeInput(Grams: 67.0, Material: "beeswax", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("67 grams of beeswax", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Shears", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Shears tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Awl Punch", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Awl Punch tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_drug_mercurial_ointment_pot", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a mercurial ointment pot (@renaissance_drug_mercurial_ointment_pot)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MedicalAndRepairPrepareMyrrhTinctureVial() => new()
	{
		Metadata = new("Medical and repair/prepare myrrh tincture vial", "crafts", ["renaissance"], ["item:renaissance_drug_myrrh_tincture_vial"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "prepare myrrh tincture vial",
		Category = "Medical and repair",
		Blurb = "prepare a myrrh tincture vial",
		Action = "preparing a medical or repair item",
		ActiveCraftItemSdesc = "an in-progress myrrh tincture vial craft",
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
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_drug_myrrh_tincture_vial", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a myrrh tincture vial (@renaissance_drug_myrrh_tincture_vial)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MedicalAndRepairPrepareOpiumTinctureBottle() => new()
	{
		Metadata = new("Medical and repair/prepare opium tincture bottle", "crafts", ["renaissance"], ["item:renaissance_drug_opium_tincture_bottle"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "prepare opium tincture bottle",
		Category = "Medical and repair",
		Blurb = "prepare an opium tincture bottle",
		Action = "preparing a medical or repair item",
		ActiveCraftItemSdesc = "an in-progress opium tincture bottle craft",
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
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_drug_opium_tincture_bottle", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x an opium tincture bottle (@renaissance_drug_opium_tincture_bottle)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MedicalAndRepairPrepareRosewaterCompress() => new()
	{
		Metadata = new("Medical and repair/prepare rosewater compress", "crafts", ["renaissance"], ["item:renaissance_drug_rosewater_compress"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "prepare rosewater compress",
		Category = "Medical and repair",
		Blurb = "prepare a rosewater compress",
		Action = "preparing a medical or repair item",
		ActiveCraftItemSdesc = "an in-progress rosewater compress craft",
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
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_drug_rosewater_compress", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a rosewater compress (@renaissance_drug_rosewater_compress)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MedicalAndRepairPrepareSarsaparillaSyrupBottle() => new()
	{
		Metadata = new("Medical and repair/prepare sarsaparilla syrup bottle", "crafts", ["renaissance"], ["item:renaissance_drug_sarsaparilla_syrup_bottle"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "prepare sarsaparilla syrup bottle",
		Category = "Medical and repair",
		Blurb = "prepare a sarsaparilla syrup bottle",
		Action = "preparing a medical or repair item",
		ActiveCraftItemSdesc = "an in-progress sarsaparilla syrup bottle craft",
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
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_drug_sarsaparilla_syrup_bottle", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a sarsaparilla syrup bottle (@renaissance_drug_sarsaparilla_syrup_bottle)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MedicalAndRepairPrepareSassafrasTonicBottle() => new()
	{
		Metadata = new("Medical and repair/prepare sassafras tonic bottle", "crafts", ["renaissance"], ["item:renaissance_drug_sassafras_tonic_bottle"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "prepare sassafras tonic bottle",
		Category = "Medical and repair",
		Blurb = "prepare a sassafras tonic bottle",
		Action = "preparing a medical or repair item",
		ActiveCraftItemSdesc = "an in-progress sassafras tonic bottle craft",
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
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_drug_sassafras_tonic_bottle", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a sassafras tonic bottle (@renaissance_drug_sassafras_tonic_bottle)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MedicalAndRepairPrepareTobaccoSmokeRoll() => new()
	{
		Metadata = new("Medical and repair/prepare tobacco smoke roll", "crafts", ["renaissance"], ["item:renaissance_drug_tobacco_smoke_roll"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "prepare tobacco smoke roll",
		Category = "Medical and repair",
		Blurb = "prepare a tobacco smoke roll",
		Action = "preparing a medical or repair item",
		ActiveCraftItemSdesc = "an in-progress tobacco smoke roll craft",
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
			new CatalogueCommodityRecipeInput(Grams: 1260.0, Material: "leather", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1260 grams of leather", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Shears", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Shears tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Awl Punch", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Awl Punch tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_drug_tobacco_smoke_roll", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a tobacco smoke roll (@renaissance_drug_tobacco_smoke_roll)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodAndDrinkPrepareIncaArmyCharquiRation() => new()
	{
		Metadata = new("Food and drink/prepare Inca army charqui ration", "crafts", ["renaissance"], ["item:renaissance_food_andean_army_charqui_ration"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "prepare Inca army charqui ration",
		Category = "Food and drink",
		Blurb = "prepare an Inca army charqui ration",
		Action = "preparing a food or drink item",
		ActiveCraftItemSdesc = "an in-progress Inca army charqui ration craft",
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
			new CatalogueCommodityRecipeInput(Grams: 338.0, Material: "meat", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("338 grams of meat", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hot Fire", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Hot Fire tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_food_andean_army_charqui_ration", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x an Inca army charqui ration (@renaissance_food_andean_army_charqui_ration)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodAndDrinkPrepareChichaGrainResidueBread() => new()
	{
		Metadata = new("Food and drink/prepare chicha-grain residue bread", "crafts", ["renaissance"], ["item:renaissance_food_andean_chicha_residue_bread"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "prepare chicha-grain residue bread",
		Category = "Food and drink",
		Blurb = "prepare a chicha-grain residue bread",
		Action = "preparing a food or drink item",
		ActiveCraftItemSdesc = "an in-progress chicha-grain residue bread craft",
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
			new CatalogueCommodityRecipeInput(Grams: 300.0, Material: "corn", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("300 grams of corn", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hot Fire", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Hot Fire tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_food_andean_chicha_residue_bread", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a chicha-grain residue bread (@renaissance_food_andean_chicha_residue_bread)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodAndDrinkPrepareAndeanChuOMeatStew() => new()
	{
		Metadata = new("Food and drink/prepare Andean chu\u00f1o meat stew", "crafts", ["renaissance"], ["item:renaissance_food_andean_chuno_meat_stew"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "prepare Andean chu\u00f1o meat stew",
		Category = "Food and drink",
		Blurb = "prepare an Andean chu\u00f1o meat stew",
		Action = "preparing a food or drink item",
		ActiveCraftItemSdesc = "an in-progress Andean chu\u00f1o meat stew craft",
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
			new CatalogueCommodityRecipeInput(Grams: 525.0, Material: "meat", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("525 grams of meat", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hot Fire", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Hot Fire tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_food_andean_chuno_meat_stew", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x an Andean chu\u00f1o meat stew (@renaissance_food_andean_chuno_meat_stew)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodAndDrinkPrepareAndeanFreezeDriedPotatoCake() => new()
	{
		Metadata = new("Food and drink/prepare Andean freeze-dried potato cake", "crafts", ["renaissance"], ["item:renaissance_food_andean_freeze_dried_potato_cake"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "prepare Andean freeze-dried potato cake",
		Category = "Food and drink",
		Blurb = "prepare an Andean freeze-dried potato cake",
		Action = "preparing a food or drink item",
		ActiveCraftItemSdesc = "an in-progress Andean freeze-dried potato cake craft",
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
			new(40, "$0 tend|tends the heat and work|works the prepared material into its final form. $0 clear|clears the work area before the final steps.", "$0 tend|tends the heat and work|works the prepared material into its final form. $0 clear|clears the work area before the final steps.", ExertionLevel.Stasis, 0),
			new(25, "$0 portion|portions and set|sets aside $p1 for serving or storage.", "$0 find|finds the batch spoiled and set|sets the material aside.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 263.0, Material: "vegetable", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("263 grams of vegetable", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hot Fire", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Hot Fire tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_food_andean_freeze_dried_potato_cake", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x an Andean freeze-dried potato cake (@renaissance_food_andean_freeze_dried_potato_cake)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodAndDrinkPrepareFreezeDriedTuberPudding() => new()
	{
		Metadata = new("Food and drink/prepare freeze-dried tuber pudding", "crafts", ["renaissance"], ["item:renaissance_food_andean_freeze_dried_tuber_pudding"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "prepare freeze-dried tuber pudding",
		Category = "Food and drink",
		Blurb = "prepare a freeze-dried tuber pudding",
		Action = "preparing a food or drink item",
		ActiveCraftItemSdesc = "an in-progress freeze-dried tuber pudding craft",
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
			new(40, "$0 tend|tends the heat and work|works the prepared material into its final form. $0 clear|clears the work area before the final steps.", "$0 tend|tends the heat and work|works the prepared material into its final form. $0 clear|clears the work area before the final steps.", ExertionLevel.Stasis, 0),
			new(25, "$0 portion|portions and set|sets aside $p1 for serving or storage.", "$0 find|finds the batch spoiled and set|sets the material aside.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 300.0, Material: "vegetable", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("300 grams of vegetable", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hot Fire", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Hot Fire tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_food_andean_freeze_dried_tuber_pudding", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a freeze-dried tuber pudding (@renaissance_food_andean_freeze_dried_tuber_pudding)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodAndDrinkPrepareAndeanFruitAndGrainBar() => new()
	{
		Metadata = new("Food and drink/prepare Andean fruit-and-grain bar", "crafts", ["renaissance"], ["item:renaissance_food_andean_fruit_quinoa_like_bar"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "prepare Andean fruit-and-grain bar",
		Category = "Food and drink",
		Blurb = "prepare an Andean fruit-and-grain bar",
		Action = "preparing a food or drink item",
		ActiveCraftItemSdesc = "an in-progress Andean fruit-and-grain bar craft",
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
			new(40, "$0 tend|tends the heat and work|works the prepared material into its final form. $0 pause|pauses to check the proportions before carrying on.", "$0 tend|tends the heat and work|works the prepared material into its final form. $0 pause|pauses to check the proportions before carrying on.", ExertionLevel.Stasis, 0),
			new(25, "$0 portion|portions and set|sets aside $p1 for serving or storage.", "$0 find|finds the batch spoiled and set|sets the material aside.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 210.0, Material: "food", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("210 grams of food", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hot Fire", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Hot Fire tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_food_andean_fruit_quinoa_like_bar", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x an Andean fruit-and-grain bar (@renaissance_food_andean_fruit_quinoa_like_bar)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodAndDrinkPrepareAndeanEarthOvenGuineaPig() => new()
	{
		Metadata = new("Food and drink/prepare Andean earth-oven guinea pig", "crafts", ["renaissance"], ["item:renaissance_food_andean_guinea_pig_pachamanca"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "prepare Andean earth-oven guinea pig",
		Category = "Food and drink",
		Blurb = "prepare an Andean earth-oven guinea pig",
		Action = "preparing a food or drink item",
		ActiveCraftItemSdesc = "an in-progress Andean earth-oven guinea pig craft",
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
			new(40, "$0 tend|tends the heat and work|works the prepared material into its final form. $0 measure|measures the developing form against the intended finish.", "$0 tend|tends the heat and work|works the prepared material into its final form. $0 measure|measures the developing form against the intended finish.", ExertionLevel.Stasis, 0),
			new(25, "$0 portion|portions and set|sets aside $p1 for serving or storage.", "$0 find|finds the batch spoiled and set|sets the material aside.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 525.0, Material: "meat", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("525 grams of meat", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hot Fire", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Hot Fire tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_food_andean_guinea_pig_pachamanca", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x an Andean earth-oven guinea pig (@renaissance_food_andean_guinea_pig_pachamanca)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodAndDrinkPrepareSweetAndeanMaizeOffering() => new()
	{
		Metadata = new("Food and drink/prepare sweet Andean maize offering", "crafts", ["renaissance"], ["item:renaissance_food_andean_honeyed_maize_offering"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "prepare sweet Andean maize offering",
		Category = "Food and drink",
		Blurb = "prepare a sweet Andean maize offering",
		Action = "preparing a food or drink item",
		ActiveCraftItemSdesc = "an in-progress sweet Andean maize offering craft",
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
			new(40, "$0 tend|tends the heat and work|works the prepared material into its final form. $0 measure|measures the developing form against the intended finish.", "$0 tend|tends the heat and work|works the prepared material into its final form. $0 measure|measures the developing form against the intended finish.", ExertionLevel.Stasis, 0),
			new(25, "$0 portion|portions and set|sets aside $p1 for serving or storage.", "$0 find|finds the batch spoiled and set|sets the material aside.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 263.0, Material: "corn", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("263 grams of corn", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hot Fire", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Hot Fire tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_food_andean_honeyed_maize_offering", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a sweet Andean maize offering (@renaissance_food_andean_honeyed_maize_offering)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodAndDrinkPrepareCharquiAndMaizeCake() => new()
	{
		Metadata = new("Food and drink/prepare charqui-and-maize cake", "crafts", ["renaissance"], ["item:renaissance_food_andean_llama_charqui_maize_pie"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "prepare charqui-and-maize cake",
		Category = "Food and drink",
		Blurb = "prepare a charqui-and-maize cake",
		Action = "preparing a food or drink item",
		ActiveCraftItemSdesc = "an in-progress charqui-and-maize cake craft",
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
			new(40, "$0 tend|tends the heat and work|works the prepared material into its final form. $0 clear|clears the work area before the final steps.", "$0 tend|tends the heat and work|works the prepared material into its final form. $0 clear|clears the work area before the final steps.", ExertionLevel.Stasis, 0),
			new(25, "$0 portion|portions and set|sets aside $p1 for serving or storage.", "$0 find|finds the batch spoiled and set|sets the material aside.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 375.0, Material: "meat", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("375 grams of meat", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hot Fire", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Hot Fire tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_food_andean_llama_charqui_maize_pie", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a charqui-and-maize cake (@renaissance_food_andean_llama_charqui_maize_pie)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodAndDrinkPrepareStuffedAndeanTuber() => new()
	{
		Metadata = new("Food and drink/prepare stuffed Andean tuber", "crafts", ["renaissance"], ["item:renaissance_food_andean_pepper_stuffed_tuber"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "prepare stuffed Andean tuber",
		Category = "Food and drink",
		Blurb = "prepare a stuffed Andean tuber",
		Action = "preparing a food or drink item",
		ActiveCraftItemSdesc = "an in-progress stuffed Andean tuber craft",
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
			new CatalogueCommodityRecipeInput(Grams: 338.0, Material: "vegetable", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("338 grams of vegetable", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hot Fire", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Hot Fire tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_food_andean_pepper_stuffed_tuber", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a stuffed Andean tuber (@renaissance_food_andean_pepper_stuffed_tuber)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodAndDrinkPrepareAndeanHighGrainCake() => new()
	{
		Metadata = new("Food and drink/prepare Andean high-grain cake", "crafts", ["renaissance"], ["item:renaissance_food_andean_quinoa_like_grain_cake"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "prepare Andean high-grain cake",
		Category = "Food and drink",
		Blurb = "prepare an Andean high-grain cake",
		Action = "preparing a food or drink item",
		ActiveCraftItemSdesc = "an in-progress Andean high-grain cake craft",
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
			new(40, "$0 tend|tends the heat and work|works the prepared material into its final form. $0 pause|pauses to check the proportions before carrying on.", "$0 tend|tends the heat and work|works the prepared material into its final form. $0 pause|pauses to check the proportions before carrying on.", ExertionLevel.Stasis, 0),
			new(25, "$0 portion|portions and set|sets aside $p1 for serving or storage.", "$0 find|finds the batch spoiled and set|sets the material aside.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 263.0, Material: "food", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("263 grams of food", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hot Fire", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Hot Fire tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_food_andean_quinoa_like_grain_cake", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x an Andean high-grain cake (@renaissance_food_andean_quinoa_like_grain_cake)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodAndDrinkPrepareRoyalStuffedSmallAnimalRoast() => new()
	{
		Metadata = new("Food and drink/prepare royal stuffed small-animal roast", "crafts", ["renaissance"], ["item:renaissance_food_andean_royal_cuy_stuffed_maize"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "prepare royal stuffed small-animal roast",
		Category = "Food and drink",
		Blurb = "prepare a royal stuffed small-animal roast",
		Action = "preparing a food or drink item",
		ActiveCraftItemSdesc = "an in-progress royal stuffed small-animal roast craft",
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
			new(40, "$0 tend|tends the heat and work|works the prepared material into its final form. $0 pause|pauses to check the proportions before carrying on.", "$0 tend|tends the heat and work|works the prepared material into its final form. $0 pause|pauses to check the proportions before carrying on.", ExertionLevel.Stasis, 0),
			new(25, "$0 portion|portions and set|sets aside $p1 for serving or storage.", "$0 find|finds the batch spoiled and set|sets the material aside.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 600.0, Material: "meat", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("600 grams of meat", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hot Fire", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Hot Fire tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_food_andean_royal_cuy_stuffed_maize", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a royal stuffed small-animal roast (@renaissance_food_andean_royal_cuy_stuffed_maize)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodAndDrinkPrepareAndeanRoyalMaizeTamale() => new()
	{
		Metadata = new("Food and drink/prepare Andean royal maize tamale", "crafts", ["renaissance"], ["item:renaissance_food_andean_royal_maize_tamale"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "prepare Andean royal maize tamale",
		Category = "Food and drink",
		Blurb = "prepare an Andean royal maize tamale",
		Action = "preparing a food or drink item",
		ActiveCraftItemSdesc = "an in-progress Andean royal maize tamale craft",
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
			new CatalogueCommodityRecipeInput(Grams: 375.0, Material: "corn", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("375 grams of corn", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hot Fire", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Hot Fire tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_food_andean_royal_maize_tamale", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x an Andean royal maize tamale (@renaissance_food_andean_royal_maize_tamale)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodAndDrinkPrepareAndeanSeaweedTuberPacket() => new()
	{
		Metadata = new("Food and drink/prepare Andean seaweed-tuber packet", "crafts", ["renaissance"], ["item:renaissance_food_andean_seaweed_tuber_packet"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "prepare Andean seaweed-tuber packet",
		Category = "Food and drink",
		Blurb = "prepare an Andean seaweed-tuber packet",
		Action = "preparing a food or drink item",
		ActiveCraftItemSdesc = "an in-progress Andean seaweed-tuber packet craft",
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
			new(40, "$0 tend|tends the heat and work|works the prepared material into its final form. $0 pause|pauses to check the proportions before carrying on.", "$0 tend|tends the heat and work|works the prepared material into its final form. $0 pause|pauses to check the proportions before carrying on.", ExertionLevel.Stasis, 0),
			new(25, "$0 portion|portions and set|sets aside $p1 for serving or storage.", "$0 find|finds the batch spoiled and set|sets the material aside.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 300.0, Material: "vegetable", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("300 grams of vegetable", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hot Fire", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Hot Fire tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_food_andean_seaweed_tuber_packet", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x an Andean seaweed-tuber packet (@renaissance_food_andean_seaweed_tuber_packet)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodAndDrinkPrepareAndeanFishMaizeCake() => new()
	{
		Metadata = new("Food and drink/prepare Andean fish-maize cake", "crafts", ["renaissance"], ["item:renaissance_food_andean_sun_dried_fish_maize_cake"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "prepare Andean fish-maize cake",
		Category = "Food and drink",
		Blurb = "prepare an Andean fish-maize cake",
		Action = "preparing a food or drink item",
		ActiveCraftItemSdesc = "an in-progress Andean fish-maize cake craft",
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
			new(40, "$0 tend|tends the heat and work|works the prepared material into its final form. $0 measure|measures the developing form against the intended finish.", "$0 tend|tends the heat and work|works the prepared material into its final form. $0 measure|measures the developing form against the intended finish.", ExertionLevel.Stasis, 0),
			new(25, "$0 portion|portions and set|sets aside $p1 for serving or storage.", "$0 find|finds the batch spoiled and set|sets the material aside.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 225.0, Material: "fish", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("225 grams of fish", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hot Fire", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Hot Fire tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_food_andean_sun_dried_fish_maize_cake", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x an Andean fish-maize cake (@renaissance_food_andean_sun_dried_fish_maize_cake)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodAndDrinkPrepareAtlanticTurkeyPastie() => new()
	{
		Metadata = new("Food and drink/prepare Atlantic turkey pastie", "crafts", ["renaissance"], ["item:renaissance_food_atlantic_ship_turkey_pastie"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "prepare Atlantic turkey pastie",
		Category = "Food and drink",
		Blurb = "prepare an Atlantic turkey pastie",
		Action = "preparing a food or drink item",
		ActiveCraftItemSdesc = "an in-progress Atlantic turkey pastie craft",
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
			new(40, "$0 tend|tends the heat and work|works the prepared material into its final form. $0 clear|clears the work area before the final steps.", "$0 tend|tends the heat and work|works the prepared material into its final form. $0 clear|clears the work area before the final steps.", ExertionLevel.Stasis, 0),
			new(25, "$0 portion|portions and set|sets aside $p1 for serving or storage.", "$0 find|finds the batch spoiled and set|sets the material aside.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 263.0, Material: "meat", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("263 grams of meat", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hot Fire", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Hot Fire tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_food_atlantic_ship_turkey_pastie", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x an Atlantic turkey pastie (@renaissance_food_atlantic_ship_turkey_pastie)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodAndDrinkPrepareAmaranthRitualFigure() => new()
	{
		Metadata = new("Food and drink/prepare amaranth ritual figure", "crafts", ["renaissance"], ["item:renaissance_food_aztec_amaranth_deity_figure"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "prepare amaranth ritual figure",
		Category = "Food and drink",
		Blurb = "prepare an amaranth ritual figure",
		Action = "preparing a food or drink item",
		ActiveCraftItemSdesc = "an in-progress amaranth ritual figure craft",
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
			new CatalogueCommodityRecipeInput(Grams: 338.0, Material: "food", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("338 grams of food", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hot Fire", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Hot Fire tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_food_aztec_amaranth_deity_figure", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x an amaranth ritual figure (@renaissance_food_aztec_amaranth_deity_figure)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodAndDrinkPrepareAvocadoMaizeRoll() => new()
	{
		Metadata = new("Food and drink/prepare avocado maize roll", "crafts", ["renaissance"], ["item:renaissance_food_aztec_avocado_maize_roll"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "prepare avocado maize roll",
		Category = "Food and drink",
		Blurb = "prepare an avocado maize roll",
		Action = "preparing a food or drink item",
		ActiveCraftItemSdesc = "an in-progress avocado maize roll craft",
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
			new(40, "$0 tend|tends the heat and work|works the prepared material into its final form. $0 measure|measures the developing form against the intended finish.", "$0 tend|tends the heat and work|works the prepared material into its final form. $0 measure|measures the developing form against the intended finish.", ExertionLevel.Stasis, 0),
			new(25, "$0 portion|portions and set|sets aside $p1 for serving or storage.", "$0 find|finds the batch spoiled and set|sets the material aside.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 210.0, Material: "corn", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("210 grams of corn", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hot Fire", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Hot Fire tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_food_aztec_avocado_maize_roll", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x an avocado maize roll (@renaissance_food_aztec_avocado_maize_roll)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodAndDrinkPrepareCacaoAndMaizeFeastCake() => new()
	{
		Metadata = new("Food and drink/prepare cacao-and-maize feast cake", "crafts", ["renaissance"], ["item:renaissance_food_aztec_cacao_maize_cake"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "prepare cacao-and-maize feast cake",
		Category = "Food and drink",
		Blurb = "prepare a cacao-and-maize feast cake",
		Action = "preparing a food or drink item",
		ActiveCraftItemSdesc = "an in-progress cacao-and-maize feast cake craft",
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
			new CatalogueCommodityRecipeInput(Grams: 225.0, Material: "food", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("225 grams of food", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hot Fire", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Hot Fire tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_food_aztec_cacao_maize_cake", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a cacao-and-maize feast cake (@renaissance_food_aztec_cacao_maize_cake)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodAndDrinkPrepareTurkeyFilledAztecTamales() => new()
	{
		Metadata = new("Food and drink/prepare turkey-filled Aztec tamales", "crafts", ["renaissance"], ["item:renaissance_food_aztec_filled_turkey_tamales"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "prepare turkey-filled Aztec tamales",
		Category = "Food and drink",
		Blurb = "prepare turkey-filled Aztec tamales",
		Action = "preparing a food or drink item",
		ActiveCraftItemSdesc = "an in-progress turkey-filled Aztec tamales craft",
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
			new(40, "$0 tend|tends the heat and work|works the prepared material into its final form. $0 clear|clears the work area before the final steps.", "$0 tend|tends the heat and work|works the prepared material into its final form. $0 clear|clears the work area before the final steps.", ExertionLevel.Stasis, 0),
			new(25, "$0 portion|portions and set|sets aside $p1 for serving or storage.", "$0 find|finds the batch spoiled and set|sets the material aside.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 413.0, Material: "meat", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("413 grams of meat", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hot Fire", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Hot Fire tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_food_aztec_filled_turkey_tamales", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x turkey-filled Aztec tamales (@renaissance_food_aztec_filled_turkey_tamales)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodAndDrinkPrepareFishRoeMaizeTamales() => new()
	{
		Metadata = new("Food and drink/prepare fish-roe maize tamales", "crafts", ["renaissance"], ["item:renaissance_food_aztec_fish_roe_tamales"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "prepare fish-roe maize tamales",
		Category = "Food and drink",
		Blurb = "prepare fish-roe maize tamales",
		Action = "preparing a food or drink item",
		ActiveCraftItemSdesc = "an in-progress fish-roe maize tamales craft",
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
			new CatalogueCommodityRecipeInput(Grams: 300.0, Material: "fish", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("300 grams of fish", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hot Fire", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Hot Fire tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_food_aztec_fish_roe_tamales", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x fish-roe maize tamales (@renaissance_food_aztec_fish_roe_tamales)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodAndDrinkPrepareFrogFilledLakeTamales() => new()
	{
		Metadata = new("Food and drink/prepare frog-filled lake tamales", "crafts", ["renaissance"], ["item:renaissance_food_aztec_frog_filled_tamales"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "prepare frog-filled lake tamales",
		Category = "Food and drink",
		Blurb = "prepare frog-filled lake tamales",
		Action = "preparing a food or drink item",
		ActiveCraftItemSdesc = "an in-progress frog-filled lake tamales craft",
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
			new CatalogueCommodityRecipeInput(Grams: 315.0, Material: "meat", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("315 grams of meat", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hot Fire", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Hot Fire tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_food_aztec_frog_filled_tamales", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x frog-filled lake tamales (@renaissance_food_aztec_frog_filled_tamales)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodAndDrinkPrepareAztecHominyFeastBowl() => new()
	{
		Metadata = new("Food and drink/prepare Aztec hominy feast bowl", "crafts", ["renaissance"], ["item:renaissance_food_aztec_hominy_pozole_feast"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "prepare Aztec hominy feast bowl",
		Category = "Food and drink",
		Blurb = "prepare an Aztec hominy feast bowl",
		Action = "preparing a food or drink item",
		ActiveCraftItemSdesc = "an in-progress Aztec hominy feast bowl craft",
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
			new(40, "$0 tend|tends the heat and work|works the prepared material into its final form. $0 pause|pauses to check the proportions before carrying on.", "$0 tend|tends the heat and work|works the prepared material into its final form. $0 pause|pauses to check the proportions before carrying on.", ExertionLevel.Stasis, 0),
			new(25, "$0 portion|portions and set|sets aside $p1 for serving or storage.", "$0 find|finds the batch spoiled and set|sets the material aside.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 563.0, Material: "corn", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("563 grams of corn", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hot Fire", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Hot Fire tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_food_aztec_hominy_pozole_feast", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x an Aztec hominy feast bowl (@renaissance_food_aztec_hominy_pozole_feast)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodAndDrinkPrepareAztecLakeAlgaeCake() => new()
	{
		Metadata = new("Food and drink/prepare Aztec lake-algae cake", "crafts", ["renaissance"], ["item:renaissance_food_aztec_lake_algae_cake"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "prepare Aztec lake-algae cake",
		Category = "Food and drink",
		Blurb = "prepare an Aztec lake-algae cake",
		Action = "preparing a food or drink item",
		ActiveCraftItemSdesc = "an in-progress Aztec lake-algae cake craft",
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
			new CatalogueCommodityRecipeInput(Grams: 90.0, Material: "vegetable", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("90 grams of vegetable", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hot Fire", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Hot Fire tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_food_aztec_lake_algae_cake", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x an Aztec lake-algae cake (@renaissance_food_aztec_lake_algae_cake)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodAndDrinkPrepareInsectAndSeedMarketCake() => new()
	{
		Metadata = new("Food and drink/prepare insect-and-seed market cake", "crafts", ["renaissance"], ["item:renaissance_food_aztec_market_insect_seed_cake"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "prepare insect-and-seed market cake",
		Category = "Food and drink",
		Blurb = "prepare an insect-and-seed market cake",
		Action = "preparing a food or drink item",
		ActiveCraftItemSdesc = "an in-progress insect-and-seed market cake craft",
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
			new(40, "$0 tend|tends the heat and work|works the prepared material into its final form. $0 pause|pauses to check the proportions before carrying on.", "$0 tend|tends the heat and work|works the prepared material into its final form. $0 pause|pauses to check the proportions before carrying on.", ExertionLevel.Stasis, 0),
			new(25, "$0 portion|portions and set|sets aside $p1 for serving or storage.", "$0 find|finds the batch spoiled and set|sets the material aside.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 135.0, Material: "food", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("135 grams of food", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hot Fire", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Hot Fire tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_food_aztec_market_insect_seed_cake", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x an insect-and-seed market cake (@renaissance_food_aztec_market_insect_seed_cake)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodAndDrinkPrepareAztecNixtamalTortillaStack() => new()
	{
		Metadata = new("Food and drink/prepare Aztec nixtamal tortilla stack", "crafts", ["renaissance"], ["item:renaissance_food_aztec_nixtamal_tortilla_stack"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "prepare Aztec nixtamal tortilla stack",
		Category = "Food and drink",
		Blurb = "prepare an Aztec nixtamal tortilla stack",
		Action = "preparing a food or drink item",
		ActiveCraftItemSdesc = "an in-progress Aztec nixtamal tortilla stack craft",
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
			new CatalogueCommodityRecipeInput(Grams: 300.0, Material: "corn", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("300 grams of corn", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hot Fire", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Hot Fire tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_food_aztec_nixtamal_tortilla_stack", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x an Aztec nixtamal tortilla stack (@renaissance_food_aztec_nixtamal_tortilla_stack)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodAndDrinkPrepareTurkeyInAnAztecSeedSauce() => new()
	{
		Metadata = new("Food and drink/prepare turkey in an Aztec seed sauce", "crafts", ["renaissance"], ["item:renaissance_food_aztec_royal_turkey_mole"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "prepare turkey in an Aztec seed sauce",
		Category = "Food and drink",
		Blurb = "prepare turkey in an Aztec seed sauce",
		Action = "preparing a food or drink item",
		ActiveCraftItemSdesc = "an in-progress turkey in an Aztec seed sauce craft",
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
			new(40, "$0 tend|tends the heat and work|works the prepared material into its final form. $0 measure|measures the developing form against the intended finish.", "$0 tend|tends the heat and work|works the prepared material into its final form. $0 measure|measures the developing form against the intended finish.", ExertionLevel.Stasis, 0),
			new(25, "$0 portion|portions and set|sets aside $p1 for serving or storage.", "$0 find|finds the batch spoiled and set|sets the material aside.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 638.0, Material: "meat", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("638 grams of meat", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hot Fire", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Hot Fire tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_food_aztec_royal_turkey_mole", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x turkey in an Aztec seed sauce (@renaissance_food_aztec_royal_turkey_mole)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodAndDrinkPrepareWhiteCeremonialTamales() => new()
	{
		Metadata = new("Food and drink/prepare white ceremonial tamales", "crafts", ["renaissance"], ["item:renaissance_food_aztec_white_ceremonial_tamales"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "prepare white ceremonial tamales",
		Category = "Food and drink",
		Blurb = "prepare white ceremonial tamales",
		Action = "preparing a food or drink item",
		ActiveCraftItemSdesc = "an in-progress white ceremonial tamales craft",
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
			new(40, "$0 tend|tends the heat and work|works the prepared material into its final form. $0 clear|clears the work area before the final steps.", "$0 tend|tends the heat and work|works the prepared material into its final form. $0 clear|clears the work area before the final steps.", ExertionLevel.Stasis, 0),
			new(25, "$0 portion|portions and set|sets aside $p1 for serving or storage.", "$0 find|finds the batch spoiled and set|sets the material aside.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 375.0, Material: "corn", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("375 grams of corn", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hot Fire", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Hot Fire tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_food_aztec_white_ceremonial_tamales", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x white ceremonial tamales (@renaissance_food_aztec_white_ceremonial_tamales)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodAndDrinkPrepareUnbakedLayeredBaklavaTray() => new()
	{
		Metadata = new("Food and drink/prepare unbaked layered baklava tray", "crafts", ["renaissance"], ["item:renaissance_food_baklava_unbaked_layered_tray"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "prepare unbaked layered baklava tray",
		Category = "Food and drink",
		Blurb = "prepare an unbaked layered baklava tray",
		Action = "preparing a food or drink item",
		ActiveCraftItemSdesc = "an in-progress unbaked layered baklava tray craft",
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
			new CatalogueCommodityRecipeInput(Grams: 675.0, Material: "dough", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("675 grams of dough", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hot Fire", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Hot Fire tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_food_baklava_unbaked_layered_tray", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x an unbaked layered baklava tray (@renaissance_food_baklava_unbaked_layered_tray)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodAndDrinkPrepareUnfinishedBanquetPieArmature() => new()
	{
		Metadata = new("Food and drink/prepare unfinished banquet pie armature", "crafts", ["renaissance"], ["item:renaissance_food_banquet_pie_armature"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "prepare unfinished banquet pie armature",
		Category = "Food and drink",
		Blurb = "prepare an unfinished banquet pie armature",
		Action = "preparing a food or drink item",
		ActiveCraftItemSdesc = "an in-progress unfinished banquet pie armature craft",
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
			new CatalogueCommodityRecipeInput(Grams: 675.0, Material: "dough", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("675 grams of dough", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hot Fire", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Hot Fire tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_food_banquet_pie_armature", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x an unfinished banquet pie armature (@renaissance_food_banquet_pie_armature)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodAndDrinkPrepareLeafSteamedBeanPudding() => new()
	{
		Metadata = new("Food and drink/prepare leaf-steamed bean pudding", "crafts", ["renaissance"], ["item:renaissance_food_benin_bean_leaf_pudding"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "prepare leaf-steamed bean pudding",
		Category = "Food and drink",
		Blurb = "prepare a leaf-steamed bean pudding",
		Action = "preparing a food or drink item",
		ActiveCraftItemSdesc = "an in-progress leaf-steamed bean pudding craft",
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
			new(40, "$0 tend|tends the heat and work|works the prepared material into its final form. $0 pause|pauses to check the proportions before carrying on.", "$0 tend|tends the heat and work|works the prepared material into its final form. $0 pause|pauses to check the proportions before carrying on.", ExertionLevel.Stasis, 0),
			new(25, "$0 portion|portions and set|sets aside $p1 for serving or storage.", "$0 find|finds the batch spoiled and set|sets the material aside.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 300.0, Material: "food", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("300 grams of food", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hot Fire", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Hot Fire tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_food_benin_bean_leaf_pudding", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a leaf-steamed bean pudding (@renaissance_food_benin_bean_leaf_pudding)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodAndDrinkPrepareBeninPalaceFeastFish() => new()
	{
		Metadata = new("Food and drink/prepare Benin palace feast fish", "crafts", ["renaissance"], ["item:renaissance_food_benin_coral_feast_fish"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "prepare Benin palace feast fish",
		Category = "Food and drink",
		Blurb = "prepare a Benin palace feast fish",
		Action = "preparing a food or drink item",
		ActiveCraftItemSdesc = "an in-progress Benin palace feast fish craft",
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
			new CatalogueCommodityRecipeInput(Grams: 825.0, Material: "fish", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("825 grams of fish", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hot Fire", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Hot Fire tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_food_benin_coral_feast_fish", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a Benin palace feast fish (@renaissance_food_benin_coral_feast_fish)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodAndDrinkPrepareBeninPalacePepperSoup() => new()
	{
		Metadata = new("Food and drink/prepare Benin palace pepper soup", "crafts", ["renaissance"], ["item:renaissance_food_benin_palace_pepper_soup"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "prepare Benin palace pepper soup",
		Category = "Food and drink",
		Blurb = "prepare a Benin palace pepper soup",
		Action = "preparing a food or drink item",
		ActiveCraftItemSdesc = "an in-progress Benin palace pepper soup craft",
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
			new(40, "$0 tend|tends the heat and work|works the prepared material into its final form. $0 pause|pauses to check the proportions before carrying on.", "$0 tend|tends the heat and work|works the prepared material into its final form. $0 pause|pauses to check the proportions before carrying on.", ExertionLevel.Stasis, 0),
			new(25, "$0 portion|portions and set|sets aside $p1 for serving or storage.", "$0 find|finds the batch spoiled and set|sets the material aside.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 450.0, Material: "meat", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("450 grams of meat", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hot Fire", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Hot Fire tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_food_benin_palace_pepper_soup", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a Benin palace pepper soup (@renaissance_food_benin_palace_pepper_soup)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodAndDrinkPrepareStackOfRawBRekSheets() => new()
	{
		Metadata = new("Food and drink/prepare stack of raw b\u00f6rek sheets", "crafts", ["renaissance"], ["item:renaissance_food_borek_phyllo_stack"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "prepare stack of raw b\u00f6rek sheets",
		Category = "Food and drink",
		Blurb = "prepare a stack of raw b\u00f6rek sheets",
		Action = "preparing a food or drink item",
		ActiveCraftItemSdesc = "an in-progress stack of raw b\u00f6rek sheets craft",
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
			new(40, "$0 tend|tends the heat and work|works the prepared material into its final form. $0 clear|clears the work area before the final steps.", "$0 tend|tends the heat and work|works the prepared material into its final form. $0 clear|clears the work area before the final steps.", ExertionLevel.Stasis, 0),
			new(25, "$0 portion|portions and set|sets aside $p1 for serving or storage.", "$0 find|finds the batch spoiled and set|sets the material aside.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 375.0, Material: "dough", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("375 grams of dough", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hot Fire", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Hot Fire tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_food_borek_phyllo_stack", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a stack of raw b\u00f6rek sheets (@renaissance_food_borek_phyllo_stack)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodAndDrinkPrepareBowlOfRawCaponForcemeat() => new()
	{
		Metadata = new("Food and drink/prepare bowl of raw capon forcemeat", "crafts", ["renaissance"], ["item:renaissance_food_capon_forcemeat_bowl"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "prepare bowl of raw capon forcemeat",
		Category = "Food and drink",
		Blurb = "prepare a bowl of raw capon forcemeat",
		Action = "preparing a food or drink item",
		ActiveCraftItemSdesc = "an in-progress bowl of raw capon forcemeat craft",
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
			new CatalogueCommodityRecipeInput(Grams: 450.0, Material: "meat", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("450 grams of meat", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hot Fire", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Hot Fire tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_food_capon_forcemeat_bowl", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a bowl of raw capon forcemeat (@renaissance_food_capon_forcemeat_bowl)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodAndDrinkPrepareFreshlySaltedCharquiStrips() => new()
	{
		Metadata = new("Food and drink/prepare freshly salted charqui strips", "crafts", ["renaissance"], ["item:renaissance_food_charqui_salted_drying_strips"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "prepare freshly salted charqui strips",
		Category = "Food and drink",
		Blurb = "prepare freshly salted charqui strips",
		Action = "preparing a food or drink item",
		ActiveCraftItemSdesc = "an in-progress freshly salted charqui strips craft",
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
			new CatalogueCommodityRecipeInput(Grams: 525.0, Material: "meat", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("525 grams of meat", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hot Fire", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Hot Fire tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_food_charqui_salted_drying_strips", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x freshly salted charqui strips (@renaissance_food_charqui_salted_drying_strips)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodAndDrinkPrepareThawingTuberPressBundle() => new()
	{
		Metadata = new("Food and drink/prepare thawing tuber press bundle", "crafts", ["renaissance"], ["item:renaissance_food_chuno_thawing_press_bundle"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "prepare thawing tuber press bundle",
		Category = "Food and drink",
		Blurb = "prepare a thawing tuber press bundle",
		Action = "preparing a food or drink item",
		ActiveCraftItemSdesc = "an in-progress thawing tuber press bundle craft",
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
			new(40, "$0 tend|tends the heat and work|works the prepared material into its final form. $0 clear|clears the work area before the final steps.", "$0 tend|tends the heat and work|works the prepared material into its final form. $0 clear|clears the work area before the final steps.", ExertionLevel.Stasis, 0),
			new(25, "$0 portion|portions and set|sets aside $p1 for serving or storage.", "$0 find|finds the batch spoiled and set|sets the material aside.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 675.0, Material: "vegetable", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("675 grams of vegetable", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hot Fire", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Hot Fire tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_food_chuno_thawing_press_bundle", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a thawing tuber press bundle (@renaissance_food_chuno_thawing_press_bundle)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodAndDrinkPreparePartCandiedCitrusPeel() => new()
	{
		Metadata = new("Food and drink/prepare part-candied citrus peel", "crafts", ["renaissance"], ["item:renaissance_food_citrus_peel_first_syrup_stage"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs:80", []),
		RequiredAnyEra = ["renaissance"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "prepare part-candied citrus peel",
		Category = "Food and drink",
		Blurb = "prepare part-candied citrus peel",
		Action = "preparing a food or drink item",
		ActiveCraftItemSdesc = "an in-progress part-candied citrus peel craft",
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
			new CatalogueCommodityRecipeInput(Grams: 300.0, Material: "food", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("300 grams of food", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hot Fire", State: DesiredItemState.InRoom) { Compatibility = new("InRoom - an item with the Hot Fire tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "renaissance_food_citrus_peel_first_syrup_stage", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x part-candied citrus peel (@renaissance_food_citrus_peel_first_syrup_stage)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};
}
