#nullable enable

using MudSharp.Body;
using MudSharp.GameItems.Inventory.Plans;
using MudSharp.RPG.Checks;

namespace DatabaseSeeder.Seeders;

public static partial class Recipes20Stock
{
	public static CatalogueRecipeDefinition FoodDrinkAndPackagedConsumablesMakeVentilatedPaperSweetForDomesticProvisioningCraftIndustrialisedFoodDrinkPackaged0166VentilatedSweet() => new()
	{
		Metadata = new("Food, drink and packaged consumables/make ventilated paper sweet for domestic provisioning [craft_industrialised_food_drink_packaged_0166_ventilated_sweet]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_food_drink_packaged_0166_ventilated_sweet"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make ventilated paper sweet for domestic provisioning [craft_industrialised_food_drink_packaged_0166_ventilated_sweet]",
		Category = "Food, drink and packaged consumables",
		Blurb = "Make a ventilated paper sweet for domestic provisioning from prepared paper stock.",
		Action = "making ventilated paper sweet for domestic provisioning",
		ActiveCraftItemSdesc = "an in-progress ventilated paper sweet for domestic provisioning craft",
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
			new CatalogueCommodityRecipeInput(Grams: 1922.0, Material: "paper", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1922 grams of paper", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_food_drink_packaged_0166_ventilated_sweet", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a ventilated paper sweet for domestic provisioning (@industrialised_food_drink_packaged_0166_ventilated_sweet)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodDrinkAndPackagedConsumablesMakeVentilatedCottonCrackerForDomesticProvisioningCraftIndustrialisedFoodDrinkPackaged0167VentilatedCracker() => new()
	{
		Metadata = new("Food, drink and packaged consumables/make ventilated cotton cracker for domestic provisioning [craft_industrialised_food_drink_packaged_0167_ventilated_cracker]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_food_drink_packaged_0167_ventilated_cracker"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make ventilated cotton cracker for domestic provisioning [craft_industrialised_food_drink_packaged_0167_ventilated_cracker]",
		Category = "Food, drink and packaged consumables",
		Blurb = "Make a ventilated cotton cracker for domestic provisioning from prepared cotton stock.",
		Action = "making ventilated cotton cracker for domestic provisioning",
		ActiveCraftItemSdesc = "an in-progress ventilated cotton cracker for domestic provisioning craft",
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
			new CatalogueCommodityRecipeInput(Grams: 1962.0, Material: "cotton", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1962 grams of cotton", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_food_drink_packaged_0167_ventilated_cracker", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a ventilated cotton cracker for domestic provisioning (@industrialised_food_drink_packaged_0167_ventilated_cracker)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodDrinkAndPackagedConsumablesMakeVentilatedCeramicPacketForDomesticProvisioningCraftIndustrialisedFoodDrinkPackaged0168VentilatedPacket() => new()
	{
		Metadata = new("Food, drink and packaged consumables/make ventilated ceramic packet for domestic provisioning [craft_industrialised_food_drink_packaged_0168_ventilated_packet]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_food_drink_packaged_0168_ventilated_packet"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make ventilated ceramic packet for domestic provisioning [craft_industrialised_food_drink_packaged_0168_ventilated_packet]",
		Category = "Food, drink and packaged consumables",
		Blurb = "Make a ventilated ceramic packet for domestic provisioning from prepared ceramic stock.",
		Action = "making ventilated ceramic packet for domestic provisioning",
		ActiveCraftItemSdesc = "an in-progress ventilated ceramic packet for domestic provisioning craft",
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
			new CatalogueCommodityRecipeInput(Grams: 2003.0, Material: "ceramic", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2003 grams of ceramic", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_food_drink_packaged_0168_ventilated_packet", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a ventilated ceramic packet for domestic provisioning (@industrialised_food_drink_packaged_0168_ventilated_packet)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodDrinkAndPackagedConsumablesMakeSomeStackablePaperCrumbsForDomesticProvisioningCraftIndustrialisedFoodDrinkPackaged0169StackableCrumbs() => new()
	{
		Metadata = new("Food, drink and packaged consumables/make some stackable paper crumbs for domestic provisioning [craft_industrialised_food_drink_packaged_0169_stackable_crumbs]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_food_drink_packaged_0169_stackable_crumbs"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make some stackable paper crumbs for domestic provisioning [craft_industrialised_food_drink_packaged_0169_stackable_crumbs]",
		Category = "Food, drink and packaged consumables",
		Blurb = "Make some stackable paper crumbs for domestic provisioning from prepared paper stock.",
		Action = "making some stackable paper crumbs for domestic provisioning",
		ActiveCraftItemSdesc = "an in-progress some stackable paper crumbs for domestic provisioning craft",
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
			new CatalogueCommodityRecipeInput(Grams: 2044.0, Material: "paper", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2044 grams of paper", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_food_drink_packaged_0169_stackable_crumbs", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x some stackable paper crumbs for domestic provisioning (@industrialised_food_drink_packaged_0169_stackable_crumbs)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodDrinkAndPackagedConsumablesMakeStackableCottonBiscuitForDomesticProvisioningCraftIndustrialisedFoodDrinkPackaged0170StackableBiscuit() => new()
	{
		Metadata = new("Food, drink and packaged consumables/make stackable cotton biscuit for domestic provisioning [craft_industrialised_food_drink_packaged_0170_stackable_biscuit]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_food_drink_packaged_0170_stackable_biscuit"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make stackable cotton biscuit for domestic provisioning [craft_industrialised_food_drink_packaged_0170_stackable_biscuit]",
		Category = "Food, drink and packaged consumables",
		Blurb = "Make a stackable cotton biscuit for domestic provisioning from prepared cotton stock.",
		Action = "making stackable cotton biscuit for domestic provisioning",
		ActiveCraftItemSdesc = "an in-progress stackable cotton biscuit for domestic provisioning craft",
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
			new CatalogueCommodityRecipeInput(Grams: 2084.0, Material: "cotton", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2084 grams of cotton", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_food_drink_packaged_0170_stackable_biscuit", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a stackable cotton biscuit for domestic provisioning (@industrialised_food_drink_packaged_0170_stackable_biscuit)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodDrinkAndPackagedConsumablesMakeStackableCeramicRationForDomesticProvisioningCraftIndustrialisedFoodDrinkPackaged0171StackableRation() => new()
	{
		Metadata = new("Food, drink and packaged consumables/make stackable ceramic ration for domestic provisioning [craft_industrialised_food_drink_packaged_0171_stackable_ration]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_food_drink_packaged_0171_stackable_ration"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make stackable ceramic ration for domestic provisioning [craft_industrialised_food_drink_packaged_0171_stackable_ration]",
		Category = "Food, drink and packaged consumables",
		Blurb = "Make a stackable ceramic ration for domestic provisioning from prepared ceramic stock.",
		Action = "making stackable ceramic ration for domestic provisioning",
		ActiveCraftItemSdesc = "an in-progress stackable ceramic ration for domestic provisioning craft",
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
			new CatalogueCommodityRecipeInput(Grams: 2125.0, Material: "ceramic", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2125 grams of ceramic", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_food_drink_packaged_0171_stackable_ration", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a stackable ceramic ration for domestic provisioning (@industrialised_food_drink_packaged_0171_stackable_ration)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodDrinkAndPackagedConsumablesMakeStackablePaperPreserveForDomesticProvisioningCraftIndustrialisedFoodDrinkPackaged0172StackablePreserve() => new()
	{
		Metadata = new("Food, drink and packaged consumables/make stackable paper preserve for domestic provisioning [craft_industrialised_food_drink_packaged_0172_stackable_preserve]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_food_drink_packaged_0172_stackable_preserve"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make stackable paper preserve for domestic provisioning [craft_industrialised_food_drink_packaged_0172_stackable_preserve]",
		Category = "Food, drink and packaged consumables",
		Blurb = "Make a stackable paper preserve for domestic provisioning from prepared paper stock.",
		Action = "making stackable paper preserve for domestic provisioning",
		ActiveCraftItemSdesc = "an in-progress stackable paper preserve for domestic provisioning craft",
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
			new CatalogueCommodityRecipeInput(Grams: 2166.0, Material: "paper", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2166 grams of paper", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_food_drink_packaged_0172_stackable_preserve", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a stackable paper preserve for domestic provisioning (@industrialised_food_drink_packaged_0172_stackable_preserve)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodDrinkAndPackagedConsumablesMakeStackableCottonLoafForDomesticProvisioningCraftIndustrialisedFoodDrinkPackaged0173StackableLoaf() => new()
	{
		Metadata = new("Food, drink and packaged consumables/make stackable cotton loaf for domestic provisioning [craft_industrialised_food_drink_packaged_0173_stackable_loaf]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_food_drink_packaged_0173_stackable_loaf"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make stackable cotton loaf for domestic provisioning [craft_industrialised_food_drink_packaged_0173_stackable_loaf]",
		Category = "Food, drink and packaged consumables",
		Blurb = "Make a stackable cotton loaf for domestic provisioning from prepared cotton stock.",
		Action = "making stackable cotton loaf for domestic provisioning",
		ActiveCraftItemSdesc = "an in-progress stackable cotton loaf for domestic provisioning craft",
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
			new CatalogueCommodityRecipeInput(Grams: 2207.0, Material: "cotton", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2207 grams of cotton", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_food_drink_packaged_0173_stackable_loaf", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a stackable cotton loaf for domestic provisioning (@industrialised_food_drink_packaged_0173_stackable_loaf)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodDrinkAndPackagedConsumablesMakeStackableCeramicCakeForDomesticProvisioningCraftIndustrialisedFoodDrinkPackaged0174StackableCake() => new()
	{
		Metadata = new("Food, drink and packaged consumables/make stackable ceramic cake for domestic provisioning [craft_industrialised_food_drink_packaged_0174_stackable_cake]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_food_drink_packaged_0174_stackable_cake"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make stackable ceramic cake for domestic provisioning [craft_industrialised_food_drink_packaged_0174_stackable_cake]",
		Category = "Food, drink and packaged consumables",
		Blurb = "Make a stackable ceramic cake for domestic provisioning from prepared ceramic stock.",
		Action = "making stackable ceramic cake for domestic provisioning",
		ActiveCraftItemSdesc = "an in-progress stackable ceramic cake for domestic provisioning craft",
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
			new CatalogueCommodityRecipeInput(Grams: 2247.0, Material: "ceramic", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2247 grams of ceramic", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_food_drink_packaged_0174_stackable_cake", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a stackable ceramic cake for domestic provisioning (@industrialised_food_drink_packaged_0174_stackable_cake)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodDrinkAndPackagedConsumablesMakeStackablePaperPuddingForDomesticProvisioningCraftIndustrialisedFoodDrinkPackaged0175StackablePudding() => new()
	{
		Metadata = new("Food, drink and packaged consumables/make stackable paper pudding for domestic provisioning [craft_industrialised_food_drink_packaged_0175_stackable_pudding]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_food_drink_packaged_0175_stackable_pudding"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make stackable paper pudding for domestic provisioning [craft_industrialised_food_drink_packaged_0175_stackable_pudding]",
		Category = "Food, drink and packaged consumables",
		Blurb = "Make a stackable paper pudding for domestic provisioning from prepared paper stock.",
		Action = "making stackable paper pudding for domestic provisioning",
		ActiveCraftItemSdesc = "an in-progress stackable paper pudding for domestic provisioning craft",
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
			new CatalogueCommodityRecipeInput(Grams: 2288.0, Material: "paper", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2288 grams of paper", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_food_drink_packaged_0175_stackable_pudding", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a stackable paper pudding for domestic provisioning (@industrialised_food_drink_packaged_0175_stackable_pudding)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodDrinkAndPackagedConsumablesMakeStackableCottonSausageForDomesticProvisioningCraftIndustrialisedFoodDrinkPackaged0176StackableSausage() => new()
	{
		Metadata = new("Food, drink and packaged consumables/make stackable cotton sausage for domestic provisioning [craft_industrialised_food_drink_packaged_0176_stackable_sausage]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_food_drink_packaged_0176_stackable_sausage"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make stackable cotton sausage for domestic provisioning [craft_industrialised_food_drink_packaged_0176_stackable_sausage]",
		Category = "Food, drink and packaged consumables",
		Blurb = "Make a stackable cotton sausage for domestic provisioning from prepared cotton stock.",
		Action = "making stackable cotton sausage for domestic provisioning",
		ActiveCraftItemSdesc = "an in-progress stackable cotton sausage for domestic provisioning craft",
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
			new CatalogueCommodityRecipeInput(Grams: 2329.0, Material: "cotton", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2329 grams of cotton", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_food_drink_packaged_0176_stackable_sausage", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a stackable cotton sausage for domestic provisioning (@industrialised_food_drink_packaged_0176_stackable_sausage)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodDrinkAndPackagedConsumablesMakeStackableCeramicPickleForDomesticProvisioningCraftIndustrialisedFoodDrinkPackaged0177StackablePickle() => new()
	{
		Metadata = new("Food, drink and packaged consumables/make stackable ceramic pickle for domestic provisioning [craft_industrialised_food_drink_packaged_0177_stackable_pickle]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_food_drink_packaged_0177_stackable_pickle"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make stackable ceramic pickle for domestic provisioning [craft_industrialised_food_drink_packaged_0177_stackable_pickle]",
		Category = "Food, drink and packaged consumables",
		Blurb = "Make a stackable ceramic pickle for domestic provisioning from prepared ceramic stock.",
		Action = "making stackable ceramic pickle for domestic provisioning",
		ActiveCraftItemSdesc = "an in-progress stackable ceramic pickle for domestic provisioning craft",
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
			new CatalogueCommodityRecipeInput(Grams: 2369.0, Material: "ceramic", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2369 grams of ceramic", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_food_drink_packaged_0177_stackable_pickle", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a stackable ceramic pickle for domestic provisioning (@industrialised_food_drink_packaged_0177_stackable_pickle)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodDrinkAndPackagedConsumablesMakeStackablePaperSweetForDomesticProvisioningCraftIndustrialisedFoodDrinkPackaged0178StackableSweet() => new()
	{
		Metadata = new("Food, drink and packaged consumables/make stackable paper sweet for domestic provisioning [craft_industrialised_food_drink_packaged_0178_stackable_sweet]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_food_drink_packaged_0178_stackable_sweet"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make stackable paper sweet for domestic provisioning [craft_industrialised_food_drink_packaged_0178_stackable_sweet]",
		Category = "Food, drink and packaged consumables",
		Blurb = "Make a stackable paper sweet for domestic provisioning from prepared paper stock.",
		Action = "making stackable paper sweet for domestic provisioning",
		ActiveCraftItemSdesc = "an in-progress stackable paper sweet for domestic provisioning craft",
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
			new CatalogueCommodityRecipeInput(Grams: 2410.0, Material: "paper", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2410 grams of paper", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_food_drink_packaged_0178_stackable_sweet", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a stackable paper sweet for domestic provisioning (@industrialised_food_drink_packaged_0178_stackable_sweet)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodDrinkAndPackagedConsumablesMakeStackableCottonCrackerForDomesticProvisioningCraftIndustrialisedFoodDrinkPackaged0179StackableCracker() => new()
	{
		Metadata = new("Food, drink and packaged consumables/make stackable cotton cracker for domestic provisioning [craft_industrialised_food_drink_packaged_0179_stackable_cracker]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_food_drink_packaged_0179_stackable_cracker"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make stackable cotton cracker for domestic provisioning [craft_industrialised_food_drink_packaged_0179_stackable_cracker]",
		Category = "Food, drink and packaged consumables",
		Blurb = "Make a stackable cotton cracker for domestic provisioning from prepared cotton stock.",
		Action = "making stackable cotton cracker for domestic provisioning",
		ActiveCraftItemSdesc = "an in-progress stackable cotton cracker for domestic provisioning craft",
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
			new CatalogueCommodityRecipeInput(Grams: 2451.0, Material: "cotton", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2451 grams of cotton", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_food_drink_packaged_0179_stackable_cracker", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a stackable cotton cracker for domestic provisioning (@industrialised_food_drink_packaged_0179_stackable_cracker)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodDrinkAndPackagedConsumablesMakeStackableCeramicPacketForDomesticProvisioningCraftIndustrialisedFoodDrinkPackaged0180StackablePacket() => new()
	{
		Metadata = new("Food, drink and packaged consumables/make stackable ceramic packet for domestic provisioning [craft_industrialised_food_drink_packaged_0180_stackable_packet]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_food_drink_packaged_0180_stackable_packet"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make stackable ceramic packet for domestic provisioning [craft_industrialised_food_drink_packaged_0180_stackable_packet]",
		Category = "Food, drink and packaged consumables",
		Blurb = "Make a stackable ceramic packet for domestic provisioning from prepared ceramic stock.",
		Action = "making stackable ceramic packet for domestic provisioning",
		ActiveCraftItemSdesc = "an in-progress stackable ceramic packet for domestic provisioning craft",
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
			new CatalogueCommodityRecipeInput(Grams: 2492.0, Material: "ceramic", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2492 grams of ceramic", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_food_drink_packaged_0180_stackable_packet", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a stackable ceramic packet for domestic provisioning (@industrialised_food_drink_packaged_0180_stackable_packet)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodDrinkAndPackagedConsumablesMakeSomeFieldPaperCrumbsForDomesticProvisioningCraftIndustrialisedFoodDrinkPackaged0181FieldCrumbs() => new()
	{
		Metadata = new("Food, drink and packaged consumables/make some field paper crumbs for domestic provisioning [craft_industrialised_food_drink_packaged_0181_field_crumbs]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_food_drink_packaged_0181_field_crumbs"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make some field paper crumbs for domestic provisioning [craft_industrialised_food_drink_packaged_0181_field_crumbs]",
		Category = "Food, drink and packaged consumables",
		Blurb = "Make some field paper crumbs for domestic provisioning from prepared paper stock.",
		Action = "making some field paper crumbs for domestic provisioning",
		ActiveCraftItemSdesc = "an in-progress some field paper crumbs for domestic provisioning craft",
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
			new CatalogueCommodityRecipeInput(Grams: 2532.0, Material: "paper", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2532 grams of paper", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_food_drink_packaged_0181_field_crumbs", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x some field paper crumbs for domestic provisioning (@industrialised_food_drink_packaged_0181_field_crumbs)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodDrinkAndPackagedConsumablesMakeFieldCottonBiscuitForDomesticProvisioningCraftIndustrialisedFoodDrinkPackaged0182FieldBiscuit() => new()
	{
		Metadata = new("Food, drink and packaged consumables/make field cotton biscuit for domestic provisioning [craft_industrialised_food_drink_packaged_0182_field_biscuit]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_food_drink_packaged_0182_field_biscuit"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make field cotton biscuit for domestic provisioning [craft_industrialised_food_drink_packaged_0182_field_biscuit]",
		Category = "Food, drink and packaged consumables",
		Blurb = "Make a field cotton biscuit for domestic provisioning from prepared cotton stock.",
		Action = "making field cotton biscuit for domestic provisioning",
		ActiveCraftItemSdesc = "an in-progress field cotton biscuit for domestic provisioning craft",
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
			new CatalogueCommodityRecipeInput(Grams: 2573.0, Material: "cotton", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2573 grams of cotton", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_food_drink_packaged_0182_field_biscuit", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a field cotton biscuit for domestic provisioning (@industrialised_food_drink_packaged_0182_field_biscuit)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodDrinkAndPackagedConsumablesMakeFieldCeramicRationForDomesticProvisioningCraftIndustrialisedFoodDrinkPackaged0183FieldRation() => new()
	{
		Metadata = new("Food, drink and packaged consumables/make field ceramic ration for domestic provisioning [craft_industrialised_food_drink_packaged_0183_field_ration]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_food_drink_packaged_0183_field_ration"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make field ceramic ration for domestic provisioning [craft_industrialised_food_drink_packaged_0183_field_ration]",
		Category = "Food, drink and packaged consumables",
		Blurb = "Make a field ceramic ration for domestic provisioning from prepared ceramic stock.",
		Action = "making field ceramic ration for domestic provisioning",
		ActiveCraftItemSdesc = "an in-progress field ceramic ration for domestic provisioning craft",
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
			new CatalogueCommodityRecipeInput(Grams: 2614.0, Material: "ceramic", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2614 grams of ceramic", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_food_drink_packaged_0183_field_ration", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a field ceramic ration for domestic provisioning (@industrialised_food_drink_packaged_0183_field_ration)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodDrinkAndPackagedConsumablesMakeFieldPaperPreserveForDomesticProvisioningCraftIndustrialisedFoodDrinkPackaged0184FieldPreserve() => new()
	{
		Metadata = new("Food, drink and packaged consumables/make field paper preserve for domestic provisioning [craft_industrialised_food_drink_packaged_0184_field_preserve]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_food_drink_packaged_0184_field_preserve"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make field paper preserve for domestic provisioning [craft_industrialised_food_drink_packaged_0184_field_preserve]",
		Category = "Food, drink and packaged consumables",
		Blurb = "Make a field paper preserve for domestic provisioning from prepared paper stock.",
		Action = "making field paper preserve for domestic provisioning",
		ActiveCraftItemSdesc = "an in-progress field paper preserve for domestic provisioning craft",
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
			new CatalogueCommodityRecipeInput(Grams: 2654.0, Material: "paper", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2654 grams of paper", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_food_drink_packaged_0184_field_preserve", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a field paper preserve for domestic provisioning (@industrialised_food_drink_packaged_0184_field_preserve)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodDrinkAndPackagedConsumablesMakeFieldCottonLoafForDomesticProvisioningCraftIndustrialisedFoodDrinkPackaged0185FieldLoaf() => new()
	{
		Metadata = new("Food, drink and packaged consumables/make field cotton loaf for domestic provisioning [craft_industrialised_food_drink_packaged_0185_field_loaf]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_food_drink_packaged_0185_field_loaf"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make field cotton loaf for domestic provisioning [craft_industrialised_food_drink_packaged_0185_field_loaf]",
		Category = "Food, drink and packaged consumables",
		Blurb = "Make a field cotton loaf for domestic provisioning from prepared cotton stock.",
		Action = "making field cotton loaf for domestic provisioning",
		ActiveCraftItemSdesc = "an in-progress field cotton loaf for domestic provisioning craft",
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
			new CatalogueCommodityRecipeInput(Grams: 2695.0, Material: "cotton", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2695 grams of cotton", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_food_drink_packaged_0185_field_loaf", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a field cotton loaf for domestic provisioning (@industrialised_food_drink_packaged_0185_field_loaf)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodDrinkAndPackagedConsumablesMakeFieldCeramicCakeForDomesticProvisioningCraftIndustrialisedFoodDrinkPackaged0186FieldCake() => new()
	{
		Metadata = new("Food, drink and packaged consumables/make field ceramic cake for domestic provisioning [craft_industrialised_food_drink_packaged_0186_field_cake]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_food_drink_packaged_0186_field_cake"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make field ceramic cake for domestic provisioning [craft_industrialised_food_drink_packaged_0186_field_cake]",
		Category = "Food, drink and packaged consumables",
		Blurb = "Make a field ceramic cake for domestic provisioning from prepared ceramic stock.",
		Action = "making field ceramic cake for domestic provisioning",
		ActiveCraftItemSdesc = "an in-progress field ceramic cake for domestic provisioning craft",
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
			new CatalogueCommodityRecipeInput(Grams: 2736.0, Material: "ceramic", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2736 grams of ceramic", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_food_drink_packaged_0186_field_cake", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a field ceramic cake for domestic provisioning (@industrialised_food_drink_packaged_0186_field_cake)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodDrinkAndPackagedConsumablesMakeFieldPaperPuddingForDomesticProvisioningCraftIndustrialisedFoodDrinkPackaged0187FieldPudding() => new()
	{
		Metadata = new("Food, drink and packaged consumables/make field paper pudding for domestic provisioning [craft_industrialised_food_drink_packaged_0187_field_pudding]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_food_drink_packaged_0187_field_pudding"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make field paper pudding for domestic provisioning [craft_industrialised_food_drink_packaged_0187_field_pudding]",
		Category = "Food, drink and packaged consumables",
		Blurb = "Make a field paper pudding for domestic provisioning from prepared paper stock.",
		Action = "making field paper pudding for domestic provisioning",
		ActiveCraftItemSdesc = "an in-progress field paper pudding for domestic provisioning craft",
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
			new CatalogueCommodityRecipeInput(Grams: 2776.0, Material: "paper", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2776 grams of paper", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_food_drink_packaged_0187_field_pudding", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a field paper pudding for domestic provisioning (@industrialised_food_drink_packaged_0187_field_pudding)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodDrinkAndPackagedConsumablesMakeFieldCottonSausageForDomesticProvisioningCraftIndustrialisedFoodDrinkPackaged0188FieldSausage() => new()
	{
		Metadata = new("Food, drink and packaged consumables/make field cotton sausage for domestic provisioning [craft_industrialised_food_drink_packaged_0188_field_sausage]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_food_drink_packaged_0188_field_sausage"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make field cotton sausage for domestic provisioning [craft_industrialised_food_drink_packaged_0188_field_sausage]",
		Category = "Food, drink and packaged consumables",
		Blurb = "Make a field cotton sausage for domestic provisioning from prepared cotton stock.",
		Action = "making field cotton sausage for domestic provisioning",
		ActiveCraftItemSdesc = "an in-progress field cotton sausage for domestic provisioning craft",
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
			new CatalogueCommodityRecipeInput(Grams: 2817.0, Material: "cotton", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2817 grams of cotton", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_food_drink_packaged_0188_field_sausage", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a field cotton sausage for domestic provisioning (@industrialised_food_drink_packaged_0188_field_sausage)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodDrinkAndPackagedConsumablesMakeFieldCeramicPickleForDomesticProvisioningCraftIndustrialisedFoodDrinkPackaged0189FieldPickle() => new()
	{
		Metadata = new("Food, drink and packaged consumables/make field ceramic pickle for domestic provisioning [craft_industrialised_food_drink_packaged_0189_field_pickle]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_food_drink_packaged_0189_field_pickle"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make field ceramic pickle for domestic provisioning [craft_industrialised_food_drink_packaged_0189_field_pickle]",
		Category = "Food, drink and packaged consumables",
		Blurb = "Make a field ceramic pickle for domestic provisioning from prepared ceramic stock.",
		Action = "making field ceramic pickle for domestic provisioning",
		ActiveCraftItemSdesc = "an in-progress field ceramic pickle for domestic provisioning craft",
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
			new CatalogueCommodityRecipeInput(Grams: 2858.0, Material: "ceramic", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2858 grams of ceramic", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_food_drink_packaged_0189_field_pickle", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a field ceramic pickle for domestic provisioning (@industrialised_food_drink_packaged_0189_field_pickle)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodDrinkAndPackagedConsumablesMakeFieldPaperSweetForDomesticProvisioningCraftIndustrialisedFoodDrinkPackaged0190FieldSweet() => new()
	{
		Metadata = new("Food, drink and packaged consumables/make field paper sweet for domestic provisioning [craft_industrialised_food_drink_packaged_0190_field_sweet]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_food_drink_packaged_0190_field_sweet"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make field paper sweet for domestic provisioning [craft_industrialised_food_drink_packaged_0190_field_sweet]",
		Category = "Food, drink and packaged consumables",
		Blurb = "Make a field paper sweet for domestic provisioning from prepared paper stock.",
		Action = "making field paper sweet for domestic provisioning",
		ActiveCraftItemSdesc = "an in-progress field paper sweet for domestic provisioning craft",
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
			new CatalogueCommodityRecipeInput(Grams: 2899.0, Material: "paper", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2899 grams of paper", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_food_drink_packaged_0190_field_sweet", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a field paper sweet for domestic provisioning (@industrialised_food_drink_packaged_0190_field_sweet)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodDrinkAndPackagedConsumablesMakeFieldCottonCrackerForDomesticProvisioningCraftIndustrialisedFoodDrinkPackaged0191FieldCracker() => new()
	{
		Metadata = new("Food, drink and packaged consumables/make field cotton cracker for domestic provisioning [craft_industrialised_food_drink_packaged_0191_field_cracker]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_food_drink_packaged_0191_field_cracker"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make field cotton cracker for domestic provisioning [craft_industrialised_food_drink_packaged_0191_field_cracker]",
		Category = "Food, drink and packaged consumables",
		Blurb = "Make a field cotton cracker for domestic provisioning from prepared cotton stock.",
		Action = "making field cotton cracker for domestic provisioning",
		ActiveCraftItemSdesc = "an in-progress field cotton cracker for domestic provisioning craft",
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
			new CatalogueCommodityRecipeInput(Grams: 2939.0, Material: "cotton", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2939 grams of cotton", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_food_drink_packaged_0191_field_cracker", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a field cotton cracker for domestic provisioning (@industrialised_food_drink_packaged_0191_field_cracker)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodDrinkAndPackagedConsumablesMakeFieldCeramicPacketForDomesticProvisioningCraftIndustrialisedFoodDrinkPackaged0192FieldPacket() => new()
	{
		Metadata = new("Food, drink and packaged consumables/make field ceramic packet for domestic provisioning [craft_industrialised_food_drink_packaged_0192_field_packet]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_food_drink_packaged_0192_field_packet"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make field ceramic packet for domestic provisioning [craft_industrialised_food_drink_packaged_0192_field_packet]",
		Category = "Food, drink and packaged consumables",
		Blurb = "Make a field ceramic packet for domestic provisioning from prepared ceramic stock.",
		Action = "making field ceramic packet for domestic provisioning",
		ActiveCraftItemSdesc = "an in-progress field ceramic packet for domestic provisioning craft",
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
			new CatalogueCommodityRecipeInput(Grams: 2980.0, Material: "ceramic", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2980 grams of ceramic", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_food_drink_packaged_0192_field_packet", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a field ceramic packet for domestic provisioning (@industrialised_food_drink_packaged_0192_field_packet)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodDrinkAndPackagedConsumablesMakeSomeCountertopPaperCrumbsForDomesticProvisioningCraftIndustrialisedFoodDrinkPackaged0193CountertopCrumbs() => new()
	{
		Metadata = new("Food, drink and packaged consumables/make some countertop paper crumbs for domestic provisioning [craft_industrialised_food_drink_packaged_0193_countertop_crumbs]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_food_drink_packaged_0193_countertop_crumbs"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make some countertop paper crumbs for domestic provisioning [craft_industrialised_food_drink_packaged_0193_countertop_crumbs]",
		Category = "Food, drink and packaged consumables",
		Blurb = "Make some countertop paper crumbs for domestic provisioning from prepared paper stock.",
		Action = "making some countertop paper crumbs for domestic provisioning",
		ActiveCraftItemSdesc = "an in-progress some countertop paper crumbs for domestic provisioning craft",
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
			new CatalogueCommodityRecipeInput(Grams: 3021.0, Material: "paper", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3021 grams of paper", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_food_drink_packaged_0193_countertop_crumbs", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x some countertop paper crumbs for domestic provisioning (@industrialised_food_drink_packaged_0193_countertop_crumbs)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodDrinkAndPackagedConsumablesMakeCountertopCottonBiscuitForDomesticProvisioningCraftIndustrialisedFoodDrinkPackaged0194CountertopBiscuit() => new()
	{
		Metadata = new("Food, drink and packaged consumables/make countertop cotton biscuit for domestic provisioning [craft_industrialised_food_drink_packaged_0194_countertop_biscuit]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_food_drink_packaged_0194_countertop_biscuit"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make countertop cotton biscuit for domestic provisioning [craft_industrialised_food_drink_packaged_0194_countertop_biscuit]",
		Category = "Food, drink and packaged consumables",
		Blurb = "Make a countertop cotton biscuit for domestic provisioning from prepared cotton stock.",
		Action = "making countertop cotton biscuit for domestic provisioning",
		ActiveCraftItemSdesc = "an in-progress countertop cotton biscuit for domestic provisioning craft",
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
			new CatalogueCommodityRecipeInput(Grams: 3061.0, Material: "cotton", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3061 grams of cotton", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_food_drink_packaged_0194_countertop_biscuit", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a countertop cotton biscuit for domestic provisioning (@industrialised_food_drink_packaged_0194_countertop_biscuit)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodDrinkAndPackagedConsumablesMakeCountertopCeramicRationForDomesticProvisioningCraftIndustrialisedFoodDrinkPackaged0195CountertopRation() => new()
	{
		Metadata = new("Food, drink and packaged consumables/make countertop ceramic ration for domestic provisioning [craft_industrialised_food_drink_packaged_0195_countertop_ration]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_food_drink_packaged_0195_countertop_ration"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make countertop ceramic ration for domestic provisioning [craft_industrialised_food_drink_packaged_0195_countertop_ration]",
		Category = "Food, drink and packaged consumables",
		Blurb = "Make a countertop ceramic ration for domestic provisioning from prepared ceramic stock.",
		Action = "making countertop ceramic ration for domestic provisioning",
		ActiveCraftItemSdesc = "an in-progress countertop ceramic ration for domestic provisioning craft",
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
			new CatalogueCommodityRecipeInput(Grams: 3102.0, Material: "ceramic", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3102 grams of ceramic", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_food_drink_packaged_0195_countertop_ration", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a countertop ceramic ration for domestic provisioning (@industrialised_food_drink_packaged_0195_countertop_ration)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodDrinkAndPackagedConsumablesMakeCountertopPaperPreserveForDomesticProvisioningCraftIndustrialisedFoodDrinkPackaged0196CountertopPreserve() => new()
	{
		Metadata = new("Food, drink and packaged consumables/make countertop paper preserve for domestic provisioning [craft_industrialised_food_drink_packaged_0196_countertop_preserve]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_food_drink_packaged_0196_countertop_preserve"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make countertop paper preserve for domestic provisioning [craft_industrialised_food_drink_packaged_0196_countertop_preserve]",
		Category = "Food, drink and packaged consumables",
		Blurb = "Make a countertop paper preserve for domestic provisioning from prepared paper stock.",
		Action = "making countertop paper preserve for domestic provisioning",
		ActiveCraftItemSdesc = "an in-progress countertop paper preserve for domestic provisioning craft",
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
			new CatalogueCommodityRecipeInput(Grams: 3143.0, Material: "paper", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3143 grams of paper", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_food_drink_packaged_0196_countertop_preserve", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a countertop paper preserve for domestic provisioning (@industrialised_food_drink_packaged_0196_countertop_preserve)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodDrinkAndPackagedConsumablesMakeCountertopCottonLoafForDomesticProvisioningCraftIndustrialisedFoodDrinkPackaged0197CountertopLoaf() => new()
	{
		Metadata = new("Food, drink and packaged consumables/make countertop cotton loaf for domestic provisioning [craft_industrialised_food_drink_packaged_0197_countertop_loaf]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_food_drink_packaged_0197_countertop_loaf"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make countertop cotton loaf for domestic provisioning [craft_industrialised_food_drink_packaged_0197_countertop_loaf]",
		Category = "Food, drink and packaged consumables",
		Blurb = "Make a countertop cotton loaf for domestic provisioning from prepared cotton stock.",
		Action = "making countertop cotton loaf for domestic provisioning",
		ActiveCraftItemSdesc = "an in-progress countertop cotton loaf for domestic provisioning craft",
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
			new CatalogueCommodityRecipeInput(Grams: 3183.0, Material: "cotton", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3183 grams of cotton", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_food_drink_packaged_0197_countertop_loaf", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a countertop cotton loaf for domestic provisioning (@industrialised_food_drink_packaged_0197_countertop_loaf)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodDrinkAndPackagedConsumablesMakeCountertopCeramicCakeForDomesticProvisioningCraftIndustrialisedFoodDrinkPackaged0198CountertopCake() => new()
	{
		Metadata = new("Food, drink and packaged consumables/make countertop ceramic cake for domestic provisioning [craft_industrialised_food_drink_packaged_0198_countertop_cake]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_food_drink_packaged_0198_countertop_cake"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make countertop ceramic cake for domestic provisioning [craft_industrialised_food_drink_packaged_0198_countertop_cake]",
		Category = "Food, drink and packaged consumables",
		Blurb = "Make a countertop ceramic cake for domestic provisioning from prepared ceramic stock.",
		Action = "making countertop ceramic cake for domestic provisioning",
		ActiveCraftItemSdesc = "an in-progress countertop ceramic cake for domestic provisioning craft",
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
			new CatalogueCommodityRecipeInput(Grams: 3224.0, Material: "ceramic", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3224 grams of ceramic", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_food_drink_packaged_0198_countertop_cake", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a countertop ceramic cake for domestic provisioning (@industrialised_food_drink_packaged_0198_countertop_cake)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodDrinkAndPackagedConsumablesMakeCountertopPaperPuddingForDomesticProvisioningCraftIndustrialisedFoodDrinkPackaged0199CountertopPudding() => new()
	{
		Metadata = new("Food, drink and packaged consumables/make countertop paper pudding for domestic provisioning [craft_industrialised_food_drink_packaged_0199_countertop_pudding]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_food_drink_packaged_0199_countertop_pudding"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make countertop paper pudding for domestic provisioning [craft_industrialised_food_drink_packaged_0199_countertop_pudding]",
		Category = "Food, drink and packaged consumables",
		Blurb = "Make a countertop paper pudding for domestic provisioning from prepared paper stock.",
		Action = "making countertop paper pudding for domestic provisioning",
		ActiveCraftItemSdesc = "an in-progress countertop paper pudding for domestic provisioning craft",
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
			new CatalogueCommodityRecipeInput(Grams: 3265.0, Material: "paper", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3265 grams of paper", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_food_drink_packaged_0199_countertop_pudding", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a countertop paper pudding for domestic provisioning (@industrialised_food_drink_packaged_0199_countertop_pudding)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodDrinkAndPackagedConsumablesMakeCountertopCottonSausageForDomesticProvisioningCraftIndustrialisedFoodDrinkPackaged0200CountertopSausage() => new()
	{
		Metadata = new("Food, drink and packaged consumables/make countertop cotton sausage for domestic provisioning [craft_industrialised_food_drink_packaged_0200_countertop_sausage]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_food_drink_packaged_0200_countertop_sausage"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make countertop cotton sausage for domestic provisioning [craft_industrialised_food_drink_packaged_0200_countertop_sausage]",
		Category = "Food, drink and packaged consumables",
		Blurb = "Make a countertop cotton sausage for domestic provisioning from prepared cotton stock.",
		Action = "making countertop cotton sausage for domestic provisioning",
		ActiveCraftItemSdesc = "an in-progress countertop cotton sausage for domestic provisioning craft",
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
			new CatalogueCommodityRecipeInput(Grams: 3306.0, Material: "cotton", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3306 grams of cotton", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_food_drink_packaged_0200_countertop_sausage", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a countertop cotton sausage for domestic provisioning (@industrialised_food_drink_packaged_0200_countertop_sausage)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodDrinkAndPackagedConsumablesMakeCountertopCeramicPickleForDomesticProvisioningCraftIndustrialisedFoodDrinkPackaged0201CountertopPickle() => new()
	{
		Metadata = new("Food, drink and packaged consumables/make countertop ceramic pickle for domestic provisioning [craft_industrialised_food_drink_packaged_0201_countertop_pickle]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_food_drink_packaged_0201_countertop_pickle"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make countertop ceramic pickle for domestic provisioning [craft_industrialised_food_drink_packaged_0201_countertop_pickle]",
		Category = "Food, drink and packaged consumables",
		Blurb = "Make a countertop ceramic pickle for domestic provisioning from prepared ceramic stock.",
		Action = "making countertop ceramic pickle for domestic provisioning",
		ActiveCraftItemSdesc = "an in-progress countertop ceramic pickle for domestic provisioning craft",
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
			new CatalogueCommodityRecipeInput(Grams: 3346.0, Material: "ceramic", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3346 grams of ceramic", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_food_drink_packaged_0201_countertop_pickle", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a countertop ceramic pickle for domestic provisioning (@industrialised_food_drink_packaged_0201_countertop_pickle)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodDrinkAndPackagedConsumablesMakeCountertopPaperSweetForDomesticProvisioningCraftIndustrialisedFoodDrinkPackaged0202CountertopSweet() => new()
	{
		Metadata = new("Food, drink and packaged consumables/make countertop paper sweet for domestic provisioning [craft_industrialised_food_drink_packaged_0202_countertop_sweet]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_food_drink_packaged_0202_countertop_sweet"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make countertop paper sweet for domestic provisioning [craft_industrialised_food_drink_packaged_0202_countertop_sweet]",
		Category = "Food, drink and packaged consumables",
		Blurb = "Make a countertop paper sweet for domestic provisioning from prepared paper stock.",
		Action = "making countertop paper sweet for domestic provisioning",
		ActiveCraftItemSdesc = "an in-progress countertop paper sweet for domestic provisioning craft",
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
			new CatalogueCommodityRecipeInput(Grams: 3387.0, Material: "paper", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3387 grams of paper", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_food_drink_packaged_0202_countertop_sweet", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a countertop paper sweet for domestic provisioning (@industrialised_food_drink_packaged_0202_countertop_sweet)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodDrinkAndPackagedConsumablesMakeCountertopCottonCrackerForDomesticProvisioningCraftIndustrialisedFoodDrinkPackaged0203CountertopCracker() => new()
	{
		Metadata = new("Food, drink and packaged consumables/make countertop cotton cracker for domestic provisioning [craft_industrialised_food_drink_packaged_0203_countertop_cracker]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_food_drink_packaged_0203_countertop_cracker"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make countertop cotton cracker for domestic provisioning [craft_industrialised_food_drink_packaged_0203_countertop_cracker]",
		Category = "Food, drink and packaged consumables",
		Blurb = "Make a countertop cotton cracker for domestic provisioning from prepared cotton stock.",
		Action = "making countertop cotton cracker for domestic provisioning",
		ActiveCraftItemSdesc = "an in-progress countertop cotton cracker for domestic provisioning craft",
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
			new CatalogueCommodityRecipeInput(Grams: 3428.0, Material: "cotton", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3428 grams of cotton", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_food_drink_packaged_0203_countertop_cracker", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a countertop cotton cracker for domestic provisioning (@industrialised_food_drink_packaged_0203_countertop_cracker)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodDrinkAndPackagedConsumablesMakeCountertopCeramicPacketForDomesticProvisioningCraftIndustrialisedFoodDrinkPackaged0204CountertopPacket() => new()
	{
		Metadata = new("Food, drink and packaged consumables/make countertop ceramic packet for domestic provisioning [craft_industrialised_food_drink_packaged_0204_countertop_packet]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_food_drink_packaged_0204_countertop_packet"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make countertop ceramic packet for domestic provisioning [craft_industrialised_food_drink_packaged_0204_countertop_packet]",
		Category = "Food, drink and packaged consumables",
		Blurb = "Make a countertop ceramic packet for domestic provisioning from prepared ceramic stock.",
		Action = "making countertop ceramic packet for domestic provisioning",
		ActiveCraftItemSdesc = "an in-progress countertop ceramic packet for domestic provisioning craft",
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
			new CatalogueCommodityRecipeInput(Grams: 3468.0, Material: "ceramic", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3468 grams of ceramic", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_food_drink_packaged_0204_countertop_packet", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a countertop ceramic packet for domestic provisioning (@industrialised_food_drink_packaged_0204_countertop_packet)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodDrinkAndPackagedConsumablesMakeSomeWallMountedPaperCrumbsForDomesticProvisioningCraftIndustrialisedFoodDrinkPackaged0205WallMountedCrumbs() => new()
	{
		Metadata = new("Food, drink and packaged consumables/make some wall-mounted paper crumbs for domestic provisioning [craft_industrialised_food_drink_packaged_0205_wall_mounted_crumbs]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_food_drink_packaged_0205_wall_mounted_crumbs"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make some wall-mounted paper crumbs for domestic provisioning [craft_industrialised_food_drink_packaged_0205_wall_mounted_crumbs]",
		Category = "Food, drink and packaged consumables",
		Blurb = "Make some wall-mounted paper crumbs for domestic provisioning from prepared paper stock.",
		Action = "making some wall-mounted paper crumbs for domestic provisioning",
		ActiveCraftItemSdesc = "an in-progress some wall-mounted paper crumbs for domestic provisioning craft",
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
			new CatalogueCommodityRecipeInput(Grams: 3509.0, Material: "paper", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3509 grams of paper", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_food_drink_packaged_0205_wall_mounted_crumbs", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x some wall-mounted paper crumbs for domestic provisioning (@industrialised_food_drink_packaged_0205_wall_mounted_crumbs)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodDrinkAndPackagedConsumablesMakeWallMountedCottonBiscuitForDomesticProvisioningCraftIndustrialisedFoodDrinkPackaged0206WallMountedBiscuit() => new()
	{
		Metadata = new("Food, drink and packaged consumables/make wall-mounted cotton biscuit for domestic provisioning [craft_industrialised_food_drink_packaged_0206_wall_mounted_biscuit]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_food_drink_packaged_0206_wall_mounted_biscuit"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make wall-mounted cotton biscuit for domestic provisioning [craft_industrialised_food_drink_packaged_0206_wall_mounted_biscuit]",
		Category = "Food, drink and packaged consumables",
		Blurb = "Make a wall-mounted cotton biscuit for domestic provisioning from prepared cotton stock.",
		Action = "making wall-mounted cotton biscuit for domestic provisioning",
		ActiveCraftItemSdesc = "an in-progress wall-mounted cotton biscuit for domestic provisioning craft",
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
			new CatalogueCommodityRecipeInput(Grams: 3550.0, Material: "cotton", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3550 grams of cotton", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_food_drink_packaged_0206_wall_mounted_biscuit", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a wall-mounted cotton biscuit for domestic provisioning (@industrialised_food_drink_packaged_0206_wall_mounted_biscuit)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodDrinkAndPackagedConsumablesMakeWallMountedCeramicRationForDomesticProvisioningCraftIndustrialisedFoodDrinkPackaged0207WallMountedRation() => new()
	{
		Metadata = new("Food, drink and packaged consumables/make wall-mounted ceramic ration for domestic provisioning [craft_industrialised_food_drink_packaged_0207_wall_mounted_ration]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_food_drink_packaged_0207_wall_mounted_ration"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make wall-mounted ceramic ration for domestic provisioning [craft_industrialised_food_drink_packaged_0207_wall_mounted_ration]",
		Category = "Food, drink and packaged consumables",
		Blurb = "Make a wall-mounted ceramic ration for domestic provisioning from prepared ceramic stock.",
		Action = "making wall-mounted ceramic ration for domestic provisioning",
		ActiveCraftItemSdesc = "an in-progress wall-mounted ceramic ration for domestic provisioning craft",
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
			new CatalogueCommodityRecipeInput(Grams: 3590.0, Material: "ceramic", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3590 grams of ceramic", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_food_drink_packaged_0207_wall_mounted_ration", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a wall-mounted ceramic ration for domestic provisioning (@industrialised_food_drink_packaged_0207_wall_mounted_ration)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodDrinkAndPackagedConsumablesMakeWallMountedPaperPreserveForDomesticProvisioningCraftIndustrialisedFoodDrinkPackaged0208WallMountedPreserve() => new()
	{
		Metadata = new("Food, drink and packaged consumables/make wall-mounted paper preserve for domestic provisioning [craft_industrialised_food_drink_packaged_0208_wall_mounted_preserve]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_food_drink_packaged_0208_wall_mounted_preserve"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make wall-mounted paper preserve for domestic provisioning [craft_industrialised_food_drink_packaged_0208_wall_mounted_preserve]",
		Category = "Food, drink and packaged consumables",
		Blurb = "Make a wall-mounted paper preserve for domestic provisioning from prepared paper stock.",
		Action = "making wall-mounted paper preserve for domestic provisioning",
		ActiveCraftItemSdesc = "an in-progress wall-mounted paper preserve for domestic provisioning craft",
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
			new CatalogueCommodityRecipeInput(Grams: 3631.0, Material: "paper", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3631 grams of paper", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_food_drink_packaged_0208_wall_mounted_preserve", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a wall-mounted paper preserve for domestic provisioning (@industrialised_food_drink_packaged_0208_wall_mounted_preserve)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodDrinkAndPackagedConsumablesMakeWallMountedCottonLoafForDomesticProvisioningCraftIndustrialisedFoodDrinkPackaged0209WallMountedLoaf() => new()
	{
		Metadata = new("Food, drink and packaged consumables/make wall-mounted cotton loaf for domestic provisioning [craft_industrialised_food_drink_packaged_0209_wall_mounted_loaf]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_food_drink_packaged_0209_wall_mounted_loaf"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make wall-mounted cotton loaf for domestic provisioning [craft_industrialised_food_drink_packaged_0209_wall_mounted_loaf]",
		Category = "Food, drink and packaged consumables",
		Blurb = "Make a wall-mounted cotton loaf for domestic provisioning from prepared cotton stock.",
		Action = "making wall-mounted cotton loaf for domestic provisioning",
		ActiveCraftItemSdesc = "an in-progress wall-mounted cotton loaf for domestic provisioning craft",
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
			new CatalogueCommodityRecipeInput(Grams: 3672.0, Material: "cotton", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3672 grams of cotton", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_food_drink_packaged_0209_wall_mounted_loaf", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a wall-mounted cotton loaf for domestic provisioning (@industrialised_food_drink_packaged_0209_wall_mounted_loaf)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodDrinkAndPackagedConsumablesMakeWallMountedCeramicCakeForDomesticProvisioningCraftIndustrialisedFoodDrinkPackaged0210WallMountedCake() => new()
	{
		Metadata = new("Food, drink and packaged consumables/make wall-mounted ceramic cake for domestic provisioning [craft_industrialised_food_drink_packaged_0210_wall_mounted_cake]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_food_drink_packaged_0210_wall_mounted_cake"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make wall-mounted ceramic cake for domestic provisioning [craft_industrialised_food_drink_packaged_0210_wall_mounted_cake]",
		Category = "Food, drink and packaged consumables",
		Blurb = "Make a wall-mounted ceramic cake for domestic provisioning from prepared ceramic stock.",
		Action = "making wall-mounted ceramic cake for domestic provisioning",
		ActiveCraftItemSdesc = "an in-progress wall-mounted ceramic cake for domestic provisioning craft",
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
			new CatalogueCommodityRecipeInput(Grams: 3713.0, Material: "ceramic", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3713 grams of ceramic", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_food_drink_packaged_0210_wall_mounted_cake", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a wall-mounted ceramic cake for domestic provisioning (@industrialised_food_drink_packaged_0210_wall_mounted_cake)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodDrinkAndPackagedConsumablesMakeWallMountedPaperPuddingForDomesticProvisioningCraftIndustrialisedFoodDrinkPackaged0211WallMountedPudding() => new()
	{
		Metadata = new("Food, drink and packaged consumables/make wall-mounted paper pudding for domestic provisioning [craft_industrialised_food_drink_packaged_0211_wall_mounted_pudding]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_food_drink_packaged_0211_wall_mounted_pudding"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make wall-mounted paper pudding for domestic provisioning [craft_industrialised_food_drink_packaged_0211_wall_mounted_pudding]",
		Category = "Food, drink and packaged consumables",
		Blurb = "Make a wall-mounted paper pudding for domestic provisioning from prepared paper stock.",
		Action = "making wall-mounted paper pudding for domestic provisioning",
		ActiveCraftItemSdesc = "an in-progress wall-mounted paper pudding for domestic provisioning craft",
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
			new CatalogueCommodityRecipeInput(Grams: 3753.0, Material: "paper", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3753 grams of paper", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_food_drink_packaged_0211_wall_mounted_pudding", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a wall-mounted paper pudding for domestic provisioning (@industrialised_food_drink_packaged_0211_wall_mounted_pudding)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodDrinkAndPackagedConsumablesMakeWallMountedCottonSausageForDomesticProvisioningCraftIndustrialisedFoodDrinkPackaged0212WallMountedSausage() => new()
	{
		Metadata = new("Food, drink and packaged consumables/make wall-mounted cotton sausage for domestic provisioning [craft_industrialised_food_drink_packaged_0212_wall_mounted_sausage]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_food_drink_packaged_0212_wall_mounted_sausage"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make wall-mounted cotton sausage for domestic provisioning [craft_industrialised_food_drink_packaged_0212_wall_mounted_sausage]",
		Category = "Food, drink and packaged consumables",
		Blurb = "Make a wall-mounted cotton sausage for domestic provisioning from prepared cotton stock.",
		Action = "making wall-mounted cotton sausage for domestic provisioning",
		ActiveCraftItemSdesc = "an in-progress wall-mounted cotton sausage for domestic provisioning craft",
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
			new CatalogueCommodityRecipeInput(Grams: 3794.0, Material: "cotton", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3794 grams of cotton", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_food_drink_packaged_0212_wall_mounted_sausage", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a wall-mounted cotton sausage for domestic provisioning (@industrialised_food_drink_packaged_0212_wall_mounted_sausage)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodDrinkAndPackagedConsumablesMakeWallMountedCeramicPickleForDomesticProvisioningCraftIndustrialisedFoodDrinkPackaged0213WallMountedPickle() => new()
	{
		Metadata = new("Food, drink and packaged consumables/make wall-mounted ceramic pickle for domestic provisioning [craft_industrialised_food_drink_packaged_0213_wall_mounted_pickle]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_food_drink_packaged_0213_wall_mounted_pickle"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make wall-mounted ceramic pickle for domestic provisioning [craft_industrialised_food_drink_packaged_0213_wall_mounted_pickle]",
		Category = "Food, drink and packaged consumables",
		Blurb = "Make a wall-mounted ceramic pickle for domestic provisioning from prepared ceramic stock.",
		Action = "making wall-mounted ceramic pickle for domestic provisioning",
		ActiveCraftItemSdesc = "an in-progress wall-mounted ceramic pickle for domestic provisioning craft",
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
			new CatalogueCommodityRecipeInput(Grams: 3835.0, Material: "ceramic", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3835 grams of ceramic", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_food_drink_packaged_0213_wall_mounted_pickle", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a wall-mounted ceramic pickle for domestic provisioning (@industrialised_food_drink_packaged_0213_wall_mounted_pickle)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodDrinkAndPackagedConsumablesMakeWallMountedPaperSweetForDomesticProvisioningCraftIndustrialisedFoodDrinkPackaged0214WallMountedSweet() => new()
	{
		Metadata = new("Food, drink and packaged consumables/make wall-mounted paper sweet for domestic provisioning [craft_industrialised_food_drink_packaged_0214_wall_mounted_sweet]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_food_drink_packaged_0214_wall_mounted_sweet"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make wall-mounted paper sweet for domestic provisioning [craft_industrialised_food_drink_packaged_0214_wall_mounted_sweet]",
		Category = "Food, drink and packaged consumables",
		Blurb = "Make a wall-mounted paper sweet for domestic provisioning from prepared paper stock.",
		Action = "making wall-mounted paper sweet for domestic provisioning",
		ActiveCraftItemSdesc = "an in-progress wall-mounted paper sweet for domestic provisioning craft",
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
			new CatalogueCommodityRecipeInput(Grams: 3875.0, Material: "paper", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3875 grams of paper", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_food_drink_packaged_0214_wall_mounted_sweet", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a wall-mounted paper sweet for domestic provisioning (@industrialised_food_drink_packaged_0214_wall_mounted_sweet)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodDrinkAndPackagedConsumablesMakeWallMountedCottonCrackerForDomesticProvisioningCraftIndustrialisedFoodDrinkPackaged0215WallMountedCracker() => new()
	{
		Metadata = new("Food, drink and packaged consumables/make wall-mounted cotton cracker for domestic provisioning [craft_industrialised_food_drink_packaged_0215_wall_mounted_cracker]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_food_drink_packaged_0215_wall_mounted_cracker"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make wall-mounted cotton cracker for domestic provisioning [craft_industrialised_food_drink_packaged_0215_wall_mounted_cracker]",
		Category = "Food, drink and packaged consumables",
		Blurb = "Make a wall-mounted cotton cracker for domestic provisioning from prepared cotton stock.",
		Action = "making wall-mounted cotton cracker for domestic provisioning",
		ActiveCraftItemSdesc = "an in-progress wall-mounted cotton cracker for domestic provisioning craft",
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
			new CatalogueCommodityRecipeInput(Grams: 3916.0, Material: "cotton", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3916 grams of cotton", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_food_drink_packaged_0215_wall_mounted_cracker", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a wall-mounted cotton cracker for domestic provisioning (@industrialised_food_drink_packaged_0215_wall_mounted_cracker)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodDrinkAndPackagedConsumablesMakeWallMountedCeramicPacketForDomesticProvisioningCraftIndustrialisedFoodDrinkPackaged0216WallMountedPacket() => new()
	{
		Metadata = new("Food, drink and packaged consumables/make wall-mounted ceramic packet for domestic provisioning [craft_industrialised_food_drink_packaged_0216_wall_mounted_packet]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_food_drink_packaged_0216_wall_mounted_packet"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make wall-mounted ceramic packet for domestic provisioning [craft_industrialised_food_drink_packaged_0216_wall_mounted_packet]",
		Category = "Food, drink and packaged consumables",
		Blurb = "Make a wall-mounted ceramic packet for domestic provisioning from prepared ceramic stock.",
		Action = "making wall-mounted ceramic packet for domestic provisioning",
		ActiveCraftItemSdesc = "an in-progress wall-mounted ceramic packet for domestic provisioning craft",
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
			new CatalogueCommodityRecipeInput(Grams: 3957.0, Material: "ceramic", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3957 grams of ceramic", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_food_drink_packaged_0216_wall_mounted_packet", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a wall-mounted ceramic packet for domestic provisioning (@industrialised_food_drink_packaged_0216_wall_mounted_packet)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodDrinkAndPackagedConsumablesMakeSomeTravelPaperCrumbsForDomesticProvisioningCraftIndustrialisedFoodDrinkPackaged0217TravelCrumbs() => new()
	{
		Metadata = new("Food, drink and packaged consumables/make some travel paper crumbs for domestic provisioning [craft_industrialised_food_drink_packaged_0217_travel_crumbs]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_food_drink_packaged_0217_travel_crumbs"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make some travel paper crumbs for domestic provisioning [craft_industrialised_food_drink_packaged_0217_travel_crumbs]",
		Category = "Food, drink and packaged consumables",
		Blurb = "Make some travel paper crumbs for domestic provisioning from prepared paper stock.",
		Action = "making some travel paper crumbs for domestic provisioning",
		ActiveCraftItemSdesc = "an in-progress some travel paper crumbs for domestic provisioning craft",
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
			new CatalogueCommodityRecipeInput(Grams: 3997.0, Material: "paper", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3997 grams of paper", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_food_drink_packaged_0217_travel_crumbs", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x some travel paper crumbs for domestic provisioning (@industrialised_food_drink_packaged_0217_travel_crumbs)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodDrinkAndPackagedConsumablesMakeTravelCottonBiscuitForDomesticProvisioningCraftIndustrialisedFoodDrinkPackaged0218TravelBiscuit() => new()
	{
		Metadata = new("Food, drink and packaged consumables/make travel cotton biscuit for domestic provisioning [craft_industrialised_food_drink_packaged_0218_travel_biscuit]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_food_drink_packaged_0218_travel_biscuit"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make travel cotton biscuit for domestic provisioning [craft_industrialised_food_drink_packaged_0218_travel_biscuit]",
		Category = "Food, drink and packaged consumables",
		Blurb = "Make a travel cotton biscuit for domestic provisioning from prepared cotton stock.",
		Action = "making travel cotton biscuit for domestic provisioning",
		ActiveCraftItemSdesc = "an in-progress travel cotton biscuit for domestic provisioning craft",
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
			new CatalogueCommodityRecipeInput(Grams: 4038.0, Material: "cotton", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("4038 grams of cotton", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_food_drink_packaged_0218_travel_biscuit", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a travel cotton biscuit for domestic provisioning (@industrialised_food_drink_packaged_0218_travel_biscuit)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodDrinkAndPackagedConsumablesMakeTravelCeramicRationForDomesticProvisioningCraftIndustrialisedFoodDrinkPackaged0219TravelRation() => new()
	{
		Metadata = new("Food, drink and packaged consumables/make travel ceramic ration for domestic provisioning [craft_industrialised_food_drink_packaged_0219_travel_ration]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_food_drink_packaged_0219_travel_ration"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make travel ceramic ration for domestic provisioning [craft_industrialised_food_drink_packaged_0219_travel_ration]",
		Category = "Food, drink and packaged consumables",
		Blurb = "Make a travel ceramic ration for domestic provisioning from prepared ceramic stock.",
		Action = "making travel ceramic ration for domestic provisioning",
		ActiveCraftItemSdesc = "an in-progress travel ceramic ration for domestic provisioning craft",
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
			new CatalogueCommodityRecipeInput(Grams: 4079.0, Material: "ceramic", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("4079 grams of ceramic", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_food_drink_packaged_0219_travel_ration", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a travel ceramic ration for domestic provisioning (@industrialised_food_drink_packaged_0219_travel_ration)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodDrinkAndPackagedConsumablesMakeTravelPaperPreserveForDomesticProvisioningCraftIndustrialisedFoodDrinkPackaged0220TravelPreserve() => new()
	{
		Metadata = new("Food, drink and packaged consumables/make travel paper preserve for domestic provisioning [craft_industrialised_food_drink_packaged_0220_travel_preserve]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_food_drink_packaged_0220_travel_preserve"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make travel paper preserve for domestic provisioning [craft_industrialised_food_drink_packaged_0220_travel_preserve]",
		Category = "Food, drink and packaged consumables",
		Blurb = "Make a travel paper preserve for domestic provisioning from prepared paper stock.",
		Action = "making travel paper preserve for domestic provisioning",
		ActiveCraftItemSdesc = "an in-progress travel paper preserve for domestic provisioning craft",
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
			new CatalogueCommodityRecipeInput(Grams: 4120.0, Material: "paper", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("4120 grams of paper", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_food_drink_packaged_0220_travel_preserve", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a travel paper preserve for domestic provisioning (@industrialised_food_drink_packaged_0220_travel_preserve)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodDrinkAndPackagedConsumablesMakeTravelCottonLoafForDomesticProvisioningCraftIndustrialisedFoodDrinkPackaged0221TravelLoaf() => new()
	{
		Metadata = new("Food, drink and packaged consumables/make travel cotton loaf for domestic provisioning [craft_industrialised_food_drink_packaged_0221_travel_loaf]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_food_drink_packaged_0221_travel_loaf"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make travel cotton loaf for domestic provisioning [craft_industrialised_food_drink_packaged_0221_travel_loaf]",
		Category = "Food, drink and packaged consumables",
		Blurb = "Make a travel cotton loaf for domestic provisioning from prepared cotton stock.",
		Action = "making travel cotton loaf for domestic provisioning",
		ActiveCraftItemSdesc = "an in-progress travel cotton loaf for domestic provisioning craft",
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
			new CatalogueCommodityRecipeInput(Grams: 4160.0, Material: "cotton", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("4160 grams of cotton", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_food_drink_packaged_0221_travel_loaf", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a travel cotton loaf for domestic provisioning (@industrialised_food_drink_packaged_0221_travel_loaf)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodDrinkAndPackagedConsumablesMakeTravelCeramicCakeForDomesticProvisioningCraftIndustrialisedFoodDrinkPackaged0222TravelCake() => new()
	{
		Metadata = new("Food, drink and packaged consumables/make travel ceramic cake for domestic provisioning [craft_industrialised_food_drink_packaged_0222_travel_cake]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_food_drink_packaged_0222_travel_cake"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make travel ceramic cake for domestic provisioning [craft_industrialised_food_drink_packaged_0222_travel_cake]",
		Category = "Food, drink and packaged consumables",
		Blurb = "Make a travel ceramic cake for domestic provisioning from prepared ceramic stock.",
		Action = "making travel ceramic cake for domestic provisioning",
		ActiveCraftItemSdesc = "an in-progress travel ceramic cake for domestic provisioning craft",
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
			new CatalogueCommodityRecipeInput(Grams: 4201.0, Material: "ceramic", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("4201 grams of ceramic", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_food_drink_packaged_0222_travel_cake", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a travel ceramic cake for domestic provisioning (@industrialised_food_drink_packaged_0222_travel_cake)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodDrinkAndPackagedConsumablesMakeTravelPaperPuddingForDomesticProvisioningCraftIndustrialisedFoodDrinkPackaged0223TravelPudding() => new()
	{
		Metadata = new("Food, drink and packaged consumables/make travel paper pudding for domestic provisioning [craft_industrialised_food_drink_packaged_0223_travel_pudding]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_food_drink_packaged_0223_travel_pudding"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make travel paper pudding for domestic provisioning [craft_industrialised_food_drink_packaged_0223_travel_pudding]",
		Category = "Food, drink and packaged consumables",
		Blurb = "Make a travel paper pudding for domestic provisioning from prepared paper stock.",
		Action = "making travel paper pudding for domestic provisioning",
		ActiveCraftItemSdesc = "an in-progress travel paper pudding for domestic provisioning craft",
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
			new CatalogueCommodityRecipeInput(Grams: 4242.0, Material: "paper", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("4242 grams of paper", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_food_drink_packaged_0223_travel_pudding", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a travel paper pudding for domestic provisioning (@industrialised_food_drink_packaged_0223_travel_pudding)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodDrinkAndPackagedConsumablesMakeTravelCottonSausageForDomesticProvisioningCraftIndustrialisedFoodDrinkPackaged0224TravelSausage() => new()
	{
		Metadata = new("Food, drink and packaged consumables/make travel cotton sausage for domestic provisioning [craft_industrialised_food_drink_packaged_0224_travel_sausage]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_food_drink_packaged_0224_travel_sausage"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make travel cotton sausage for domestic provisioning [craft_industrialised_food_drink_packaged_0224_travel_sausage]",
		Category = "Food, drink and packaged consumables",
		Blurb = "Make a travel cotton sausage for domestic provisioning from prepared cotton stock.",
		Action = "making travel cotton sausage for domestic provisioning",
		ActiveCraftItemSdesc = "an in-progress travel cotton sausage for domestic provisioning craft",
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
			new CatalogueCommodityRecipeInput(Grams: 4282.0, Material: "cotton", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("4282 grams of cotton", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_food_drink_packaged_0224_travel_sausage", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a travel cotton sausage for domestic provisioning (@industrialised_food_drink_packaged_0224_travel_sausage)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodDrinkAndPackagedConsumablesMakeTravelCeramicPickleForDomesticProvisioningCraftIndustrialisedFoodDrinkPackaged0225TravelPickle() => new()
	{
		Metadata = new("Food, drink and packaged consumables/make travel ceramic pickle for domestic provisioning [craft_industrialised_food_drink_packaged_0225_travel_pickle]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_food_drink_packaged_0225_travel_pickle"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make travel ceramic pickle for domestic provisioning [craft_industrialised_food_drink_packaged_0225_travel_pickle]",
		Category = "Food, drink and packaged consumables",
		Blurb = "Make a travel ceramic pickle for domestic provisioning from prepared ceramic stock.",
		Action = "making travel ceramic pickle for domestic provisioning",
		ActiveCraftItemSdesc = "an in-progress travel ceramic pickle for domestic provisioning craft",
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
			new CatalogueCommodityRecipeInput(Grams: 4323.0, Material: "ceramic", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("4323 grams of ceramic", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_food_drink_packaged_0225_travel_pickle", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a travel ceramic pickle for domestic provisioning (@industrialised_food_drink_packaged_0225_travel_pickle)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};
}
