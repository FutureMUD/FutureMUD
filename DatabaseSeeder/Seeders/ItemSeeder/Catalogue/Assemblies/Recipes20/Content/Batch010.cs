#nullable enable

using MudSharp.Body;
using MudSharp.GameItems.Inventory.Plans;
using MudSharp.RPG.Checks;

namespace DatabaseSeeder.Seeders;

public static partial class Recipes20Stock
{
	public static CatalogueRecipeDefinition FoodDrinkAndPackagedConsumablesMakeTravelPaperSweetForDomesticProvisioningCraftIndustrialisedFoodDrinkPackaged0226TravelSweet() => new()
	{
		Metadata = new("Food, drink and packaged consumables/make travel paper sweet for domestic provisioning [craft_industrialised_food_drink_packaged_0226_travel_sweet]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_food_drink_packaged_0226_travel_sweet"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make travel paper sweet for domestic provisioning [craft_industrialised_food_drink_packaged_0226_travel_sweet]",
		Category = "Food, drink and packaged consumables",
		Blurb = "Make a travel paper sweet for domestic provisioning from prepared paper stock.",
		Action = "making travel paper sweet for domestic provisioning",
		ActiveCraftItemSdesc = "an in-progress travel paper sweet for domestic provisioning craft",
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
			new CatalogueCommodityRecipeInput(Grams: 4364.0, Material: "paper", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("4364 grams of paper", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_food_drink_packaged_0226_travel_sweet", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a travel paper sweet for domestic provisioning (@industrialised_food_drink_packaged_0226_travel_sweet)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodDrinkAndPackagedConsumablesMakeTravelCottonCrackerForDomesticProvisioningCraftIndustrialisedFoodDrinkPackaged0227TravelCracker() => new()
	{
		Metadata = new("Food, drink and packaged consumables/make travel cotton cracker for domestic provisioning [craft_industrialised_food_drink_packaged_0227_travel_cracker]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_food_drink_packaged_0227_travel_cracker"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make travel cotton cracker for domestic provisioning [craft_industrialised_food_drink_packaged_0227_travel_cracker]",
		Category = "Food, drink and packaged consumables",
		Blurb = "Make a travel cotton cracker for domestic provisioning from prepared cotton stock.",
		Action = "making travel cotton cracker for domestic provisioning",
		ActiveCraftItemSdesc = "an in-progress travel cotton cracker for domestic provisioning craft",
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
			new CatalogueCommodityRecipeInput(Grams: 4404.0, Material: "cotton", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("4404 grams of cotton", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_food_drink_packaged_0227_travel_cracker", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a travel cotton cracker for domestic provisioning (@industrialised_food_drink_packaged_0227_travel_cracker)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodDrinkAndPackagedConsumablesMakeTravelCeramicPacketForDomesticProvisioningCraftIndustrialisedFoodDrinkPackaged0228TravelPacket() => new()
	{
		Metadata = new("Food, drink and packaged consumables/make travel ceramic packet for domestic provisioning [craft_industrialised_food_drink_packaged_0228_travel_packet]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_food_drink_packaged_0228_travel_packet"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make travel ceramic packet for domestic provisioning [craft_industrialised_food_drink_packaged_0228_travel_packet]",
		Category = "Food, drink and packaged consumables",
		Blurb = "Make a travel ceramic packet for domestic provisioning from prepared ceramic stock.",
		Action = "making travel ceramic packet for domestic provisioning",
		ActiveCraftItemSdesc = "an in-progress travel ceramic packet for domestic provisioning craft",
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
			new CatalogueCommodityRecipeInput(Grams: 4445.0, Material: "ceramic", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("4445 grams of ceramic", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_food_drink_packaged_0228_travel_packet", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a travel ceramic packet for domestic provisioning (@industrialised_food_drink_packaged_0228_travel_packet)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodDrinkAndPackagedConsumablesMakeSomeUtilityPaperCrumbsForDomesticProvisioningCraftIndustrialisedFoodDrinkPackaged0229UtilityCrumbs() => new()
	{
		Metadata = new("Food, drink and packaged consumables/make some utility paper crumbs for domestic provisioning [craft_industrialised_food_drink_packaged_0229_utility_crumbs]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_food_drink_packaged_0229_utility_crumbs"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make some utility paper crumbs for domestic provisioning [craft_industrialised_food_drink_packaged_0229_utility_crumbs]",
		Category = "Food, drink and packaged consumables",
		Blurb = "Make some utility paper crumbs for domestic provisioning from prepared paper stock.",
		Action = "making some utility paper crumbs for domestic provisioning",
		ActiveCraftItemSdesc = "an in-progress some utility paper crumbs for domestic provisioning craft",
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
			new CatalogueCommodityRecipeInput(Grams: 4486.0, Material: "paper", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("4486 grams of paper", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_food_drink_packaged_0229_utility_crumbs", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x some utility paper crumbs for domestic provisioning (@industrialised_food_drink_packaged_0229_utility_crumbs)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodDrinkAndPackagedConsumablesMakeUtilityCottonBiscuitForDomesticProvisioningCraftIndustrialisedFoodDrinkPackaged0230UtilityBiscuit() => new()
	{
		Metadata = new("Food, drink and packaged consumables/make utility cotton biscuit for domestic provisioning [craft_industrialised_food_drink_packaged_0230_utility_biscuit]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_food_drink_packaged_0230_utility_biscuit"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make utility cotton biscuit for domestic provisioning [craft_industrialised_food_drink_packaged_0230_utility_biscuit]",
		Category = "Food, drink and packaged consumables",
		Blurb = "Make an utility cotton biscuit for domestic provisioning from prepared cotton stock.",
		Action = "making utility cotton biscuit for domestic provisioning",
		ActiveCraftItemSdesc = "an in-progress utility cotton biscuit for domestic provisioning craft",
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
			new CatalogueCommodityRecipeInput(Grams: 4526.0, Material: "cotton", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("4526 grams of cotton", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_food_drink_packaged_0230_utility_biscuit", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x an utility cotton biscuit for domestic provisioning (@industrialised_food_drink_packaged_0230_utility_biscuit)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodDrinkAndPackagedConsumablesMakeUtilityCeramicRationForDomesticProvisioningCraftIndustrialisedFoodDrinkPackaged0231UtilityRation() => new()
	{
		Metadata = new("Food, drink and packaged consumables/make utility ceramic ration for domestic provisioning [craft_industrialised_food_drink_packaged_0231_utility_ration]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_food_drink_packaged_0231_utility_ration"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make utility ceramic ration for domestic provisioning [craft_industrialised_food_drink_packaged_0231_utility_ration]",
		Category = "Food, drink and packaged consumables",
		Blurb = "Make an utility ceramic ration for domestic provisioning from prepared ceramic stock.",
		Action = "making utility ceramic ration for domestic provisioning",
		ActiveCraftItemSdesc = "an in-progress utility ceramic ration for domestic provisioning craft",
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
			new CatalogueCommodityRecipeInput(Grams: 4567.0, Material: "ceramic", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("4567 grams of ceramic", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_food_drink_packaged_0231_utility_ration", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x an utility ceramic ration for domestic provisioning (@industrialised_food_drink_packaged_0231_utility_ration)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodDrinkAndPackagedConsumablesMakeUtilityPaperPreserveForDomesticProvisioningCraftIndustrialisedFoodDrinkPackaged0232UtilityPreserve() => new()
	{
		Metadata = new("Food, drink and packaged consumables/make utility paper preserve for domestic provisioning [craft_industrialised_food_drink_packaged_0232_utility_preserve]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_food_drink_packaged_0232_utility_preserve"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make utility paper preserve for domestic provisioning [craft_industrialised_food_drink_packaged_0232_utility_preserve]",
		Category = "Food, drink and packaged consumables",
		Blurb = "Make an utility paper preserve for domestic provisioning from prepared paper stock.",
		Action = "making utility paper preserve for domestic provisioning",
		ActiveCraftItemSdesc = "an in-progress utility paper preserve for domestic provisioning craft",
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
			new CatalogueCommodityRecipeInput(Grams: 4608.0, Material: "paper", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("4608 grams of paper", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_food_drink_packaged_0232_utility_preserve", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x an utility paper preserve for domestic provisioning (@industrialised_food_drink_packaged_0232_utility_preserve)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodDrinkAndPackagedConsumablesMakeUtilityCottonLoafForDomesticProvisioningCraftIndustrialisedFoodDrinkPackaged0233UtilityLoaf() => new()
	{
		Metadata = new("Food, drink and packaged consumables/make utility cotton loaf for domestic provisioning [craft_industrialised_food_drink_packaged_0233_utility_loaf]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_food_drink_packaged_0233_utility_loaf"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make utility cotton loaf for domestic provisioning [craft_industrialised_food_drink_packaged_0233_utility_loaf]",
		Category = "Food, drink and packaged consumables",
		Blurb = "Make an utility cotton loaf for domestic provisioning from prepared cotton stock.",
		Action = "making utility cotton loaf for domestic provisioning",
		ActiveCraftItemSdesc = "an in-progress utility cotton loaf for domestic provisioning craft",
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
			new CatalogueCommodityRecipeInput(Grams: 4649.0, Material: "cotton", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("4649 grams of cotton", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_food_drink_packaged_0233_utility_loaf", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x an utility cotton loaf for domestic provisioning (@industrialised_food_drink_packaged_0233_utility_loaf)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodDrinkAndPackagedConsumablesMakeUtilityCeramicCakeForDomesticProvisioningCraftIndustrialisedFoodDrinkPackaged0234UtilityCake() => new()
	{
		Metadata = new("Food, drink and packaged consumables/make utility ceramic cake for domestic provisioning [craft_industrialised_food_drink_packaged_0234_utility_cake]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_food_drink_packaged_0234_utility_cake"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make utility ceramic cake for domestic provisioning [craft_industrialised_food_drink_packaged_0234_utility_cake]",
		Category = "Food, drink and packaged consumables",
		Blurb = "Make an utility ceramic cake for domestic provisioning from prepared ceramic stock.",
		Action = "making utility ceramic cake for domestic provisioning",
		ActiveCraftItemSdesc = "an in-progress utility ceramic cake for domestic provisioning craft",
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
			new CatalogueCommodityRecipeInput(Grams: 4689.0, Material: "ceramic", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("4689 grams of ceramic", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_food_drink_packaged_0234_utility_cake", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x an utility ceramic cake for domestic provisioning (@industrialised_food_drink_packaged_0234_utility_cake)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodDrinkAndPackagedConsumablesMakeStackableCeramicRationForFieldProvisioningCraftIndustrialFoodDrinkPackaged0001StackableRation() => new()
	{
		Metadata = new("Food, drink and packaged consumables/make stackable ceramic ration for field provisioning [craft_industrial_food_drink_packaged_0001_stackable_ration]", "crafts", ["industrial"], ["item:industrial_food_drink_packaged_0001_stackable_ration"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make stackable ceramic ration for field provisioning [craft_industrial_food_drink_packaged_0001_stackable_ration]",
		Category = "Food, drink and packaged consumables",
		Blurb = "Make a stackable ceramic ration for field provisioning from prepared ceramic stock.",
		Action = "making stackable ceramic ration for field provisioning",
		ActiveCraftItemSdesc = "an in-progress stackable ceramic ration for field provisioning craft",
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
			new CatalogueCommodityRecipeInput(Grams: 651.0, Material: "ceramic", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("651 grams of ceramic", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrial_food_drink_packaged_0001_stackable_ration", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a stackable ceramic ration for field provisioning (@industrial_food_drink_packaged_0001_stackable_ration)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodDrinkAndPackagedConsumablesMakeStackablePaperPreserveForFieldProvisioningCraftIndustrialFoodDrinkPackaged0002StackablePreserve() => new()
	{
		Metadata = new("Food, drink and packaged consumables/make stackable paper preserve for field provisioning [craft_industrial_food_drink_packaged_0002_stackable_preserve]", "crafts", ["industrial"], ["item:industrial_food_drink_packaged_0002_stackable_preserve"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make stackable paper preserve for field provisioning [craft_industrial_food_drink_packaged_0002_stackable_preserve]",
		Category = "Food, drink and packaged consumables",
		Blurb = "Make a stackable paper preserve for field provisioning from prepared paper stock.",
		Action = "making stackable paper preserve for field provisioning",
		ActiveCraftItemSdesc = "an in-progress stackable paper preserve for field provisioning craft",
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
			new CatalogueCommodityRecipeInput(Grams: 692.0, Material: "paper", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("692 grams of paper", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrial_food_drink_packaged_0002_stackable_preserve", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a stackable paper preserve for field provisioning (@industrial_food_drink_packaged_0002_stackable_preserve)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodDrinkAndPackagedConsumablesMakeStackableCottonLoafForFieldProvisioningCraftIndustrialFoodDrinkPackaged0003StackableLoaf() => new()
	{
		Metadata = new("Food, drink and packaged consumables/make stackable cotton loaf for field provisioning [craft_industrial_food_drink_packaged_0003_stackable_loaf]", "crafts", ["industrial"], ["item:industrial_food_drink_packaged_0003_stackable_loaf"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make stackable cotton loaf for field provisioning [craft_industrial_food_drink_packaged_0003_stackable_loaf]",
		Category = "Food, drink and packaged consumables",
		Blurb = "Make a stackable cotton loaf for field provisioning from prepared cotton stock.",
		Action = "making stackable cotton loaf for field provisioning",
		ActiveCraftItemSdesc = "an in-progress stackable cotton loaf for field provisioning craft",
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
			new CatalogueCommodityRecipeInput(Grams: 733.0, Material: "cotton", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("733 grams of cotton", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrial_food_drink_packaged_0003_stackable_loaf", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a stackable cotton loaf for field provisioning (@industrial_food_drink_packaged_0003_stackable_loaf)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodDrinkAndPackagedConsumablesMakeStackableCeramicCakeForFieldProvisioningCraftIndustrialFoodDrinkPackaged0004StackableCake() => new()
	{
		Metadata = new("Food, drink and packaged consumables/make stackable ceramic cake for field provisioning [craft_industrial_food_drink_packaged_0004_stackable_cake]", "crafts", ["industrial"], ["item:industrial_food_drink_packaged_0004_stackable_cake"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make stackable ceramic cake for field provisioning [craft_industrial_food_drink_packaged_0004_stackable_cake]",
		Category = "Food, drink and packaged consumables",
		Blurb = "Make a stackable ceramic cake for field provisioning from prepared ceramic stock.",
		Action = "making stackable ceramic cake for field provisioning",
		ActiveCraftItemSdesc = "an in-progress stackable ceramic cake for field provisioning craft",
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
			new CatalogueCommodityRecipeInput(Grams: 773.0, Material: "ceramic", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("773 grams of ceramic", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrial_food_drink_packaged_0004_stackable_cake", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a stackable ceramic cake for field provisioning (@industrial_food_drink_packaged_0004_stackable_cake)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodDrinkAndPackagedConsumablesMakeStackablePaperPuddingForFieldProvisioningCraftIndustrialFoodDrinkPackaged0005StackablePudding() => new()
	{
		Metadata = new("Food, drink and packaged consumables/make stackable paper pudding for field provisioning [craft_industrial_food_drink_packaged_0005_stackable_pudding]", "crafts", ["industrial"], ["item:industrial_food_drink_packaged_0005_stackable_pudding"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make stackable paper pudding for field provisioning [craft_industrial_food_drink_packaged_0005_stackable_pudding]",
		Category = "Food, drink and packaged consumables",
		Blurb = "Make a stackable paper pudding for field provisioning from prepared paper stock.",
		Action = "making stackable paper pudding for field provisioning",
		ActiveCraftItemSdesc = "an in-progress stackable paper pudding for field provisioning craft",
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
			new CatalogueCommodityRecipeInput(Grams: 814.0, Material: "paper", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("814 grams of paper", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrial_food_drink_packaged_0005_stackable_pudding", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a stackable paper pudding for field provisioning (@industrial_food_drink_packaged_0005_stackable_pudding)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodDrinkAndPackagedConsumablesMakeStackableCottonSausageForFieldProvisioningCraftIndustrialFoodDrinkPackaged0006StackableSausage() => new()
	{
		Metadata = new("Food, drink and packaged consumables/make stackable cotton sausage for field provisioning [craft_industrial_food_drink_packaged_0006_stackable_sausage]", "crafts", ["industrial"], ["item:industrial_food_drink_packaged_0006_stackable_sausage"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make stackable cotton sausage for field provisioning [craft_industrial_food_drink_packaged_0006_stackable_sausage]",
		Category = "Food, drink and packaged consumables",
		Blurb = "Make a stackable cotton sausage for field provisioning from prepared cotton stock.",
		Action = "making stackable cotton sausage for field provisioning",
		ActiveCraftItemSdesc = "an in-progress stackable cotton sausage for field provisioning craft",
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
			new CatalogueCommodityRecipeInput(Grams: 855.0, Material: "cotton", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("855 grams of cotton", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrial_food_drink_packaged_0006_stackable_sausage", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a stackable cotton sausage for field provisioning (@industrial_food_drink_packaged_0006_stackable_sausage)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodDrinkAndPackagedConsumablesMakeStackableCeramicPickleForFieldProvisioningCraftIndustrialFoodDrinkPackaged0007StackablePickle() => new()
	{
		Metadata = new("Food, drink and packaged consumables/make stackable ceramic pickle for field provisioning [craft_industrial_food_drink_packaged_0007_stackable_pickle]", "crafts", ["industrial"], ["item:industrial_food_drink_packaged_0007_stackable_pickle"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make stackable ceramic pickle for field provisioning [craft_industrial_food_drink_packaged_0007_stackable_pickle]",
		Category = "Food, drink and packaged consumables",
		Blurb = "Make a stackable ceramic pickle for field provisioning from prepared ceramic stock.",
		Action = "making stackable ceramic pickle for field provisioning",
		ActiveCraftItemSdesc = "an in-progress stackable ceramic pickle for field provisioning craft",
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
			new CatalogueCommodityRecipeInput(Grams: 895.0, Material: "ceramic", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("895 grams of ceramic", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrial_food_drink_packaged_0007_stackable_pickle", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a stackable ceramic pickle for field provisioning (@industrial_food_drink_packaged_0007_stackable_pickle)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodDrinkAndPackagedConsumablesMakeStackablePaperSweetForFieldProvisioningCraftIndustrialFoodDrinkPackaged0008StackableSweet() => new()
	{
		Metadata = new("Food, drink and packaged consumables/make stackable paper sweet for field provisioning [craft_industrial_food_drink_packaged_0008_stackable_sweet]", "crafts", ["industrial"], ["item:industrial_food_drink_packaged_0008_stackable_sweet"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make stackable paper sweet for field provisioning [craft_industrial_food_drink_packaged_0008_stackable_sweet]",
		Category = "Food, drink and packaged consumables",
		Blurb = "Make a stackable paper sweet for field provisioning from prepared paper stock.",
		Action = "making stackable paper sweet for field provisioning",
		ActiveCraftItemSdesc = "an in-progress stackable paper sweet for field provisioning craft",
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
			new CatalogueCommodityRecipeInput(Grams: 936.0, Material: "paper", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("936 grams of paper", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrial_food_drink_packaged_0008_stackable_sweet", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a stackable paper sweet for field provisioning (@industrial_food_drink_packaged_0008_stackable_sweet)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodDrinkAndPackagedConsumablesMakeStackableCottonCrackerForFieldProvisioningCraftIndustrialFoodDrinkPackaged0009StackableCracker() => new()
	{
		Metadata = new("Food, drink and packaged consumables/make stackable cotton cracker for field provisioning [craft_industrial_food_drink_packaged_0009_stackable_cracker]", "crafts", ["industrial"], ["item:industrial_food_drink_packaged_0009_stackable_cracker"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make stackable cotton cracker for field provisioning [craft_industrial_food_drink_packaged_0009_stackable_cracker]",
		Category = "Food, drink and packaged consumables",
		Blurb = "Make a stackable cotton cracker for field provisioning from prepared cotton stock.",
		Action = "making stackable cotton cracker for field provisioning",
		ActiveCraftItemSdesc = "an in-progress stackable cotton cracker for field provisioning craft",
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
			new CatalogueCommodityRecipeInput(Grams: 977.0, Material: "cotton", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("977 grams of cotton", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrial_food_drink_packaged_0009_stackable_cracker", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a stackable cotton cracker for field provisioning (@industrial_food_drink_packaged_0009_stackable_cracker)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodDrinkAndPackagedConsumablesMakeStackableCeramicPacketForFieldProvisioningCraftIndustrialFoodDrinkPackaged0010StackablePacket() => new()
	{
		Metadata = new("Food, drink and packaged consumables/make stackable ceramic packet for field provisioning [craft_industrial_food_drink_packaged_0010_stackable_packet]", "crafts", ["industrial"], ["item:industrial_food_drink_packaged_0010_stackable_packet"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make stackable ceramic packet for field provisioning [craft_industrial_food_drink_packaged_0010_stackable_packet]",
		Category = "Food, drink and packaged consumables",
		Blurb = "Make a stackable ceramic packet for field provisioning from prepared ceramic stock.",
		Action = "making stackable ceramic packet for field provisioning",
		ActiveCraftItemSdesc = "an in-progress stackable ceramic packet for field provisioning craft",
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
			new CatalogueCommodityRecipeInput(Grams: 1018.0, Material: "ceramic", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1018 grams of ceramic", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrial_food_drink_packaged_0010_stackable_packet", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a stackable ceramic packet for field provisioning (@industrial_food_drink_packaged_0010_stackable_packet)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodDrinkAndPackagedConsumablesMakeSomeFieldPaperCrumbsForFieldProvisioningCraftIndustrialFoodDrinkPackaged0011FieldCrumbs() => new()
	{
		Metadata = new("Food, drink and packaged consumables/make some field paper crumbs for field provisioning [craft_industrial_food_drink_packaged_0011_field_crumbs]", "crafts", ["industrial"], ["item:industrial_food_drink_packaged_0011_field_crumbs"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make some field paper crumbs for field provisioning [craft_industrial_food_drink_packaged_0011_field_crumbs]",
		Category = "Food, drink and packaged consumables",
		Blurb = "Make some field paper crumbs for field provisioning from prepared paper stock.",
		Action = "making some field paper crumbs for field provisioning",
		ActiveCraftItemSdesc = "an in-progress some field paper crumbs for field provisioning craft",
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
			new CatalogueCommodityRecipeInput(Grams: 1058.0, Material: "paper", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1058 grams of paper", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrial_food_drink_packaged_0011_field_crumbs", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x some field paper crumbs for field provisioning (@industrial_food_drink_packaged_0011_field_crumbs)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodDrinkAndPackagedConsumablesMakeFieldCottonBiscuitForFieldProvisioningCraftIndustrialFoodDrinkPackaged0012FieldBiscuit() => new()
	{
		Metadata = new("Food, drink and packaged consumables/make field cotton biscuit for field provisioning [craft_industrial_food_drink_packaged_0012_field_biscuit]", "crafts", ["industrial"], ["item:industrial_food_drink_packaged_0012_field_biscuit"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make field cotton biscuit for field provisioning [craft_industrial_food_drink_packaged_0012_field_biscuit]",
		Category = "Food, drink and packaged consumables",
		Blurb = "Make a field cotton biscuit for field provisioning from prepared cotton stock.",
		Action = "making field cotton biscuit for field provisioning",
		ActiveCraftItemSdesc = "an in-progress field cotton biscuit for field provisioning craft",
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
			new CatalogueCommodityRecipeInput(Grams: 1099.0, Material: "cotton", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1099 grams of cotton", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrial_food_drink_packaged_0012_field_biscuit", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a field cotton biscuit for field provisioning (@industrial_food_drink_packaged_0012_field_biscuit)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodDrinkAndPackagedConsumablesMakeFieldCeramicRationForFieldProvisioningCraftIndustrialFoodDrinkPackaged0013FieldRation() => new()
	{
		Metadata = new("Food, drink and packaged consumables/make field ceramic ration for field provisioning [craft_industrial_food_drink_packaged_0013_field_ration]", "crafts", ["industrial"], ["item:industrial_food_drink_packaged_0013_field_ration"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make field ceramic ration for field provisioning [craft_industrial_food_drink_packaged_0013_field_ration]",
		Category = "Food, drink and packaged consumables",
		Blurb = "Make a field ceramic ration for field provisioning from prepared ceramic stock.",
		Action = "making field ceramic ration for field provisioning",
		ActiveCraftItemSdesc = "an in-progress field ceramic ration for field provisioning craft",
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
			new CatalogueCommodityRecipeInput(Grams: 1140.0, Material: "ceramic", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1140 grams of ceramic", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrial_food_drink_packaged_0013_field_ration", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a field ceramic ration for field provisioning (@industrial_food_drink_packaged_0013_field_ration)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodDrinkAndPackagedConsumablesMakeFieldPaperPreserveForFieldProvisioningCraftIndustrialFoodDrinkPackaged0014FieldPreserve() => new()
	{
		Metadata = new("Food, drink and packaged consumables/make field paper preserve for field provisioning [craft_industrial_food_drink_packaged_0014_field_preserve]", "crafts", ["industrial"], ["item:industrial_food_drink_packaged_0014_field_preserve"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make field paper preserve for field provisioning [craft_industrial_food_drink_packaged_0014_field_preserve]",
		Category = "Food, drink and packaged consumables",
		Blurb = "Make a field paper preserve for field provisioning from prepared paper stock.",
		Action = "making field paper preserve for field provisioning",
		ActiveCraftItemSdesc = "an in-progress field paper preserve for field provisioning craft",
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
			new CatalogueCommodityRecipeInput(Grams: 1180.0, Material: "paper", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1180 grams of paper", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrial_food_drink_packaged_0014_field_preserve", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a field paper preserve for field provisioning (@industrial_food_drink_packaged_0014_field_preserve)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodDrinkAndPackagedConsumablesMakeFieldCottonLoafForFieldProvisioningCraftIndustrialFoodDrinkPackaged0015FieldLoaf() => new()
	{
		Metadata = new("Food, drink and packaged consumables/make field cotton loaf for field provisioning [craft_industrial_food_drink_packaged_0015_field_loaf]", "crafts", ["industrial"], ["item:industrial_food_drink_packaged_0015_field_loaf"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make field cotton loaf for field provisioning [craft_industrial_food_drink_packaged_0015_field_loaf]",
		Category = "Food, drink and packaged consumables",
		Blurb = "Make a field cotton loaf for field provisioning from prepared cotton stock.",
		Action = "making field cotton loaf for field provisioning",
		ActiveCraftItemSdesc = "an in-progress field cotton loaf for field provisioning craft",
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
			new CatalogueCommodityRecipeInput(Grams: 1221.0, Material: "cotton", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1221 grams of cotton", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrial_food_drink_packaged_0015_field_loaf", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a field cotton loaf for field provisioning (@industrial_food_drink_packaged_0015_field_loaf)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodDrinkAndPackagedConsumablesMakeFieldCeramicCakeForFieldProvisioningCraftIndustrialFoodDrinkPackaged0016FieldCake() => new()
	{
		Metadata = new("Food, drink and packaged consumables/make field ceramic cake for field provisioning [craft_industrial_food_drink_packaged_0016_field_cake]", "crafts", ["industrial"], ["item:industrial_food_drink_packaged_0016_field_cake"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make field ceramic cake for field provisioning [craft_industrial_food_drink_packaged_0016_field_cake]",
		Category = "Food, drink and packaged consumables",
		Blurb = "Make a field ceramic cake for field provisioning from prepared ceramic stock.",
		Action = "making field ceramic cake for field provisioning",
		ActiveCraftItemSdesc = "an in-progress field ceramic cake for field provisioning craft",
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
			new CatalogueCommodityRecipeInput(Grams: 1262.0, Material: "ceramic", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1262 grams of ceramic", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrial_food_drink_packaged_0016_field_cake", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a field ceramic cake for field provisioning (@industrial_food_drink_packaged_0016_field_cake)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodDrinkAndPackagedConsumablesMakeFieldPaperPuddingForFieldProvisioningCraftIndustrialFoodDrinkPackaged0017FieldPudding() => new()
	{
		Metadata = new("Food, drink and packaged consumables/make field paper pudding for field provisioning [craft_industrial_food_drink_packaged_0017_field_pudding]", "crafts", ["industrial"], ["item:industrial_food_drink_packaged_0017_field_pudding"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make field paper pudding for field provisioning [craft_industrial_food_drink_packaged_0017_field_pudding]",
		Category = "Food, drink and packaged consumables",
		Blurb = "Make a field paper pudding for field provisioning from prepared paper stock.",
		Action = "making field paper pudding for field provisioning",
		ActiveCraftItemSdesc = "an in-progress field paper pudding for field provisioning craft",
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
			new CatalogueCommodityRecipeInput(Grams: 1302.0, Material: "paper", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1302 grams of paper", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrial_food_drink_packaged_0017_field_pudding", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a field paper pudding for field provisioning (@industrial_food_drink_packaged_0017_field_pudding)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodDrinkAndPackagedConsumablesMakeFieldCottonSausageForFieldProvisioningCraftIndustrialFoodDrinkPackaged0018FieldSausage() => new()
	{
		Metadata = new("Food, drink and packaged consumables/make field cotton sausage for field provisioning [craft_industrial_food_drink_packaged_0018_field_sausage]", "crafts", ["industrial"], ["item:industrial_food_drink_packaged_0018_field_sausage"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make field cotton sausage for field provisioning [craft_industrial_food_drink_packaged_0018_field_sausage]",
		Category = "Food, drink and packaged consumables",
		Blurb = "Make a field cotton sausage for field provisioning from prepared cotton stock.",
		Action = "making field cotton sausage for field provisioning",
		ActiveCraftItemSdesc = "an in-progress field cotton sausage for field provisioning craft",
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
			new CatalogueCommodityRecipeInput(Grams: 1343.0, Material: "cotton", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1343 grams of cotton", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrial_food_drink_packaged_0018_field_sausage", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a field cotton sausage for field provisioning (@industrial_food_drink_packaged_0018_field_sausage)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition CleaningAndPersonalCareMakePlainCeramicWasteForDomesticHygieneCraftIndustrialisedCleaningPersonalCare0001PlainWaste() => new()
	{
		Metadata = new("Cleaning and personal care/make plain ceramic waste for domestic hygiene [craft_industrialised_cleaning_personal_care_0001_plain_waste]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_cleaning_personal_care_0001_plain_waste"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make plain ceramic waste for domestic hygiene [craft_industrialised_cleaning_personal_care_0001_plain_waste]",
		Category = "Cleaning and personal care",
		Blurb = "Make a plain ceramic waste for domestic hygiene from prepared ceramic stock.",
		Action = "making plain ceramic waste for domestic hygiene",
		ActiveCraftItemSdesc = "an in-progress plain ceramic waste for domestic hygiene craft",
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
			new CatalogueCommodityRecipeInput(Grams: 762.0, Material: "ceramic", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("762 grams of ceramic", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_cleaning_personal_care_0001_plain_waste", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a plain ceramic waste for domestic hygiene (@industrialised_cleaning_personal_care_0001_plain_waste)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition CleaningAndPersonalCareMakePlainOakBrushForDomesticHygieneCraftIndustrialisedCleaningPersonalCare0002PlainBrush() => new()
	{
		Metadata = new("Cleaning and personal care/make plain oak brush for domestic hygiene [craft_industrialised_cleaning_personal_care_0002_plain_brush]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_cleaning_personal_care_0002_plain_brush"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make plain oak brush for domestic hygiene [craft_industrialised_cleaning_personal_care_0002_plain_brush]",
		Category = "Cleaning and personal care",
		Blurb = "Make a plain oak brush for domestic hygiene from prepared oak stock.",
		Action = "making plain oak brush for domestic hygiene",
		ActiveCraftItemSdesc = "an in-progress plain oak brush for domestic hygiene craft",
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
			new CatalogueCommodityRecipeInput(Grams: 803.0, Material: "oak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("803 grams of oak", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_cleaning_personal_care_0002_plain_brush", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a plain oak brush for domestic hygiene (@industrialised_cleaning_personal_care_0002_plain_brush)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition CleaningAndPersonalCareMakePlainCottonCombForDomesticHygieneCraftIndustrialisedCleaningPersonalCare0003PlainComb() => new()
	{
		Metadata = new("Cleaning and personal care/make plain cotton comb for domestic hygiene [craft_industrialised_cleaning_personal_care_0003_plain_comb]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_cleaning_personal_care_0003_plain_comb"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make plain cotton comb for domestic hygiene [craft_industrialised_cleaning_personal_care_0003_plain_comb]",
		Category = "Cleaning and personal care",
		Blurb = "Make a plain cotton comb for domestic hygiene from prepared cotton stock.",
		Action = "making plain cotton comb for domestic hygiene",
		ActiveCraftItemSdesc = "an in-progress plain cotton comb for domestic hygiene craft",
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
			new CatalogueCommodityRecipeInput(Grams: 844.0, Material: "cotton", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("844 grams of cotton", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_cleaning_personal_care_0003_plain_comb", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a plain cotton comb for domestic hygiene (@industrialised_cleaning_personal_care_0003_plain_comb)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition CleaningAndPersonalCareMakePlainMildSteelRazorForDomesticHygieneCraftIndustrialisedCleaningPersonalCare0004PlainRazor() => new()
	{
		Metadata = new("Cleaning and personal care/make plain mild steel razor for domestic hygiene [craft_industrialised_cleaning_personal_care_0004_plain_razor]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_cleaning_personal_care_0004_plain_razor"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make plain mild steel razor for domestic hygiene [craft_industrialised_cleaning_personal_care_0004_plain_razor]",
		Category = "Cleaning and personal care",
		Blurb = "Make a plain mild steel razor for domestic hygiene from prepared mild steel stock.",
		Action = "making plain mild steel razor for domestic hygiene",
		ActiveCraftItemSdesc = "an in-progress plain mild steel razor for domestic hygiene craft",
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
			new CatalogueCommodityRecipeInput(Grams: 884.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("884 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_cleaning_personal_care_0004_plain_razor", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a plain mild steel razor for domestic hygiene (@industrialised_cleaning_personal_care_0004_plain_razor)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition CleaningAndPersonalCareMakePlainCeramicSpongeForDomesticHygieneCraftIndustrialisedCleaningPersonalCare0005PlainSponge() => new()
	{
		Metadata = new("Cleaning and personal care/make plain ceramic sponge for domestic hygiene [craft_industrialised_cleaning_personal_care_0005_plain_sponge]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_cleaning_personal_care_0005_plain_sponge"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make plain ceramic sponge for domestic hygiene [craft_industrialised_cleaning_personal_care_0005_plain_sponge]",
		Category = "Cleaning and personal care",
		Blurb = "Make a plain ceramic sponge for domestic hygiene from prepared ceramic stock.",
		Action = "making plain ceramic sponge for domestic hygiene",
		ActiveCraftItemSdesc = "an in-progress plain ceramic sponge for domestic hygiene craft",
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
			new CatalogueCommodityRecipeInput(Grams: 925.0, Material: "ceramic", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("925 grams of ceramic", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_cleaning_personal_care_0005_plain_sponge", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a plain ceramic sponge for domestic hygiene (@industrialised_cleaning_personal_care_0005_plain_sponge)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition CleaningAndPersonalCareMakePlainOakClothForDomesticHygieneCraftIndustrialisedCleaningPersonalCare0006PlainCloth() => new()
	{
		Metadata = new("Cleaning and personal care/make plain oak cloth for domestic hygiene [craft_industrialised_cleaning_personal_care_0006_plain_cloth]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_cleaning_personal_care_0006_plain_cloth"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make plain oak cloth for domestic hygiene [craft_industrialised_cleaning_personal_care_0006_plain_cloth]",
		Category = "Cleaning and personal care",
		Blurb = "Make a plain oak cloth for domestic hygiene from prepared oak stock.",
		Action = "making plain oak cloth for domestic hygiene",
		ActiveCraftItemSdesc = "an in-progress plain oak cloth for domestic hygiene craft",
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
			new CatalogueCommodityRecipeInput(Grams: 966.0, Material: "oak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("966 grams of oak", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_cleaning_personal_care_0006_plain_cloth", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a plain oak cloth for domestic hygiene (@industrialised_cleaning_personal_care_0006_plain_cloth)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition CleaningAndPersonalCareMakePlainCottonBroomForDomesticHygieneCraftIndustrialisedCleaningPersonalCare0007PlainBroom() => new()
	{
		Metadata = new("Cleaning and personal care/make plain cotton broom for domestic hygiene [craft_industrialised_cleaning_personal_care_0007_plain_broom]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_cleaning_personal_care_0007_plain_broom"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make plain cotton broom for domestic hygiene [craft_industrialised_cleaning_personal_care_0007_plain_broom]",
		Category = "Cleaning and personal care",
		Blurb = "Make a plain cotton broom for domestic hygiene from prepared cotton stock.",
		Action = "making plain cotton broom for domestic hygiene",
		ActiveCraftItemSdesc = "an in-progress plain cotton broom for domestic hygiene craft",
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
			new CatalogueCommodityRecipeInput(Grams: 1007.0, Material: "cotton", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1007 grams of cotton", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_cleaning_personal_care_0007_plain_broom", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a plain cotton broom for domestic hygiene (@industrialised_cleaning_personal_care_0007_plain_broom)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition CleaningAndPersonalCareMakePlainMildSteelMopForDomesticHygieneCraftIndustrialisedCleaningPersonalCare0008PlainMop() => new()
	{
		Metadata = new("Cleaning and personal care/make plain mild steel mop for domestic hygiene [craft_industrialised_cleaning_personal_care_0008_plain_mop]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_cleaning_personal_care_0008_plain_mop"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make plain mild steel mop for domestic hygiene [craft_industrialised_cleaning_personal_care_0008_plain_mop]",
		Category = "Cleaning and personal care",
		Blurb = "Make a plain mild steel mop for domestic hygiene from prepared mild steel stock.",
		Action = "making plain mild steel mop for domestic hygiene",
		ActiveCraftItemSdesc = "an in-progress plain mild steel mop for domestic hygiene craft",
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
			new CatalogueCommodityRecipeInput(Grams: 1047.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1047 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_cleaning_personal_care_0008_plain_mop", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a plain mild steel mop for domestic hygiene (@industrialised_cleaning_personal_care_0008_plain_mop)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition CleaningAndPersonalCareMakePlainCeramicBucketForDomesticHygieneCraftIndustrialisedCleaningPersonalCare0009PlainBucket() => new()
	{
		Metadata = new("Cleaning and personal care/make plain ceramic bucket for domestic hygiene [craft_industrialised_cleaning_personal_care_0009_plain_bucket]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_cleaning_personal_care_0009_plain_bucket"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make plain ceramic bucket for domestic hygiene [craft_industrialised_cleaning_personal_care_0009_plain_bucket]",
		Category = "Cleaning and personal care",
		Blurb = "Make a plain ceramic bucket for domestic hygiene from prepared ceramic stock.",
		Action = "making plain ceramic bucket for domestic hygiene",
		ActiveCraftItemSdesc = "an in-progress plain ceramic bucket for domestic hygiene craft",
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
			new CatalogueCommodityRecipeInput(Grams: 1088.0, Material: "ceramic", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1088 grams of ceramic", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_cleaning_personal_care_0009_plain_bucket", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a plain ceramic bucket for domestic hygiene (@industrialised_cleaning_personal_care_0009_plain_bucket)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition CleaningAndPersonalCareMakePlainOakSoapdishForDomesticHygieneCraftIndustrialisedCleaningPersonalCare0010PlainSoapdish() => new()
	{
		Metadata = new("Cleaning and personal care/make plain oak soapdish for domestic hygiene [craft_industrialised_cleaning_personal_care_0010_plain_soapdish]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_cleaning_personal_care_0010_plain_soapdish"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make plain oak soapdish for domestic hygiene [craft_industrialised_cleaning_personal_care_0010_plain_soapdish]",
		Category = "Cleaning and personal care",
		Blurb = "Make a plain oak soapdish for domestic hygiene from prepared oak stock.",
		Action = "making plain oak soapdish for domestic hygiene",
		ActiveCraftItemSdesc = "an in-progress plain oak soapdish for domestic hygiene craft",
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
			new CatalogueCommodityRecipeInput(Grams: 1129.0, Material: "oak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1129 grams of oak", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_cleaning_personal_care_0010_plain_soapdish", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a plain oak soapdish for domestic hygiene (@industrialised_cleaning_personal_care_0010_plain_soapdish)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition CleaningAndPersonalCareMakeReinforcedCottonWasteForDomesticHygieneCraftIndustrialisedCleaningPersonalCare0011ReinforcedWaste() => new()
	{
		Metadata = new("Cleaning and personal care/make reinforced cotton waste for domestic hygiene [craft_industrialised_cleaning_personal_care_0011_reinforced_waste]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_cleaning_personal_care_0011_reinforced_waste"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make reinforced cotton waste for domestic hygiene [craft_industrialised_cleaning_personal_care_0011_reinforced_waste]",
		Category = "Cleaning and personal care",
		Blurb = "Make a reinforced cotton waste for domestic hygiene from prepared cotton stock.",
		Action = "making reinforced cotton waste for domestic hygiene",
		ActiveCraftItemSdesc = "an in-progress reinforced cotton waste for domestic hygiene craft",
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
			new CatalogueCommodityRecipeInput(Grams: 1169.0, Material: "cotton", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1169 grams of cotton", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_cleaning_personal_care_0011_reinforced_waste", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a reinforced cotton waste for domestic hygiene (@industrialised_cleaning_personal_care_0011_reinforced_waste)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition CleaningAndPersonalCareMakeReinforcedMildSteelBrushForDomesticHygieneCraftIndustrialisedCleaningPersonalCare0012ReinforcedBrush() => new()
	{
		Metadata = new("Cleaning and personal care/make reinforced mild steel brush for domestic hygiene [craft_industrialised_cleaning_personal_care_0012_reinforced_brush]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_cleaning_personal_care_0012_reinforced_brush"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make reinforced mild steel brush for domestic hygiene [craft_industrialised_cleaning_personal_care_0012_reinforced_brush]",
		Category = "Cleaning and personal care",
		Blurb = "Make a reinforced mild steel brush for domestic hygiene from prepared mild steel stock.",
		Action = "making reinforced mild steel brush for domestic hygiene",
		ActiveCraftItemSdesc = "an in-progress reinforced mild steel brush for domestic hygiene craft",
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
			new CatalogueCommodityRecipeInput(Grams: 1210.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1210 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_cleaning_personal_care_0012_reinforced_brush", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a reinforced mild steel brush for domestic hygiene (@industrialised_cleaning_personal_care_0012_reinforced_brush)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition CleaningAndPersonalCareMakeReinforcedCeramicCombForDomesticHygieneCraftIndustrialisedCleaningPersonalCare0013ReinforcedComb() => new()
	{
		Metadata = new("Cleaning and personal care/make reinforced ceramic comb for domestic hygiene [craft_industrialised_cleaning_personal_care_0013_reinforced_comb]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_cleaning_personal_care_0013_reinforced_comb"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make reinforced ceramic comb for domestic hygiene [craft_industrialised_cleaning_personal_care_0013_reinforced_comb]",
		Category = "Cleaning and personal care",
		Blurb = "Make a reinforced ceramic comb for domestic hygiene from prepared ceramic stock.",
		Action = "making reinforced ceramic comb for domestic hygiene",
		ActiveCraftItemSdesc = "an in-progress reinforced ceramic comb for domestic hygiene craft",
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
			new CatalogueCommodityRecipeInput(Grams: 1251.0, Material: "ceramic", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1251 grams of ceramic", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_cleaning_personal_care_0013_reinforced_comb", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a reinforced ceramic comb for domestic hygiene (@industrialised_cleaning_personal_care_0013_reinforced_comb)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition CleaningAndPersonalCareMakeReinforcedOakRazorForDomesticHygieneCraftIndustrialisedCleaningPersonalCare0014ReinforcedRazor() => new()
	{
		Metadata = new("Cleaning and personal care/make reinforced oak razor for domestic hygiene [craft_industrialised_cleaning_personal_care_0014_reinforced_razor]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_cleaning_personal_care_0014_reinforced_razor"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make reinforced oak razor for domestic hygiene [craft_industrialised_cleaning_personal_care_0014_reinforced_razor]",
		Category = "Cleaning and personal care",
		Blurb = "Make a reinforced oak razor for domestic hygiene from prepared oak stock.",
		Action = "making reinforced oak razor for domestic hygiene",
		ActiveCraftItemSdesc = "an in-progress reinforced oak razor for domestic hygiene craft",
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
			new CatalogueCommodityRecipeInput(Grams: 1291.0, Material: "oak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1291 grams of oak", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_cleaning_personal_care_0014_reinforced_razor", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a reinforced oak razor for domestic hygiene (@industrialised_cleaning_personal_care_0014_reinforced_razor)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition CleaningAndPersonalCareMakeReinforcedCottonSpongeForDomesticHygieneCraftIndustrialisedCleaningPersonalCare0015ReinforcedSponge() => new()
	{
		Metadata = new("Cleaning and personal care/make reinforced cotton sponge for domestic hygiene [craft_industrialised_cleaning_personal_care_0015_reinforced_sponge]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_cleaning_personal_care_0015_reinforced_sponge"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make reinforced cotton sponge for domestic hygiene [craft_industrialised_cleaning_personal_care_0015_reinforced_sponge]",
		Category = "Cleaning and personal care",
		Blurb = "Make a reinforced cotton sponge for domestic hygiene from prepared cotton stock.",
		Action = "making reinforced cotton sponge for domestic hygiene",
		ActiveCraftItemSdesc = "an in-progress reinforced cotton sponge for domestic hygiene craft",
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
			new CatalogueCommodityRecipeInput(Grams: 1332.0, Material: "cotton", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1332 grams of cotton", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_cleaning_personal_care_0015_reinforced_sponge", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a reinforced cotton sponge for domestic hygiene (@industrialised_cleaning_personal_care_0015_reinforced_sponge)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition CleaningAndPersonalCareMakeReinforcedMildSteelClothForDomesticHygieneCraftIndustrialisedCleaningPersonalCare0016ReinforcedCloth() => new()
	{
		Metadata = new("Cleaning and personal care/make reinforced mild steel cloth for domestic hygiene [craft_industrialised_cleaning_personal_care_0016_reinforced_cloth]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_cleaning_personal_care_0016_reinforced_cloth"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make reinforced mild steel cloth for domestic hygiene [craft_industrialised_cleaning_personal_care_0016_reinforced_cloth]",
		Category = "Cleaning and personal care",
		Blurb = "Make a reinforced mild steel cloth for domestic hygiene from prepared mild steel stock.",
		Action = "making reinforced mild steel cloth for domestic hygiene",
		ActiveCraftItemSdesc = "an in-progress reinforced mild steel cloth for domestic hygiene craft",
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
			new CatalogueCommodityRecipeInput(Grams: 1373.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1373 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_cleaning_personal_care_0016_reinforced_cloth", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a reinforced mild steel cloth for domestic hygiene (@industrialised_cleaning_personal_care_0016_reinforced_cloth)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition CleaningAndPersonalCareMakeReinforcedCeramicBroomForDomesticHygieneCraftIndustrialisedCleaningPersonalCare0017ReinforcedBroom() => new()
	{
		Metadata = new("Cleaning and personal care/make reinforced ceramic broom for domestic hygiene [craft_industrialised_cleaning_personal_care_0017_reinforced_broom]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_cleaning_personal_care_0017_reinforced_broom"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make reinforced ceramic broom for domestic hygiene [craft_industrialised_cleaning_personal_care_0017_reinforced_broom]",
		Category = "Cleaning and personal care",
		Blurb = "Make a reinforced ceramic broom for domestic hygiene from prepared ceramic stock.",
		Action = "making reinforced ceramic broom for domestic hygiene",
		ActiveCraftItemSdesc = "an in-progress reinforced ceramic broom for domestic hygiene craft",
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
			new CatalogueCommodityRecipeInput(Grams: 1414.0, Material: "ceramic", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1414 grams of ceramic", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_cleaning_personal_care_0017_reinforced_broom", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a reinforced ceramic broom for domestic hygiene (@industrialised_cleaning_personal_care_0017_reinforced_broom)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition CleaningAndPersonalCareMakeReinforcedOakMopForDomesticHygieneCraftIndustrialisedCleaningPersonalCare0018ReinforcedMop() => new()
	{
		Metadata = new("Cleaning and personal care/make reinforced oak mop for domestic hygiene [craft_industrialised_cleaning_personal_care_0018_reinforced_mop]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_cleaning_personal_care_0018_reinforced_mop"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make reinforced oak mop for domestic hygiene [craft_industrialised_cleaning_personal_care_0018_reinforced_mop]",
		Category = "Cleaning and personal care",
		Blurb = "Make a reinforced oak mop for domestic hygiene from prepared oak stock.",
		Action = "making reinforced oak mop for domestic hygiene",
		ActiveCraftItemSdesc = "an in-progress reinforced oak mop for domestic hygiene craft",
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
			new CatalogueCommodityRecipeInput(Grams: 1454.0, Material: "oak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1454 grams of oak", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_cleaning_personal_care_0018_reinforced_mop", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a reinforced oak mop for domestic hygiene (@industrialised_cleaning_personal_care_0018_reinforced_mop)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition CleaningAndPersonalCareMakeReinforcedCottonBucketForDomesticHygieneCraftIndustrialisedCleaningPersonalCare0019ReinforcedBucket() => new()
	{
		Metadata = new("Cleaning and personal care/make reinforced cotton bucket for domestic hygiene [craft_industrialised_cleaning_personal_care_0019_reinforced_bucket]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_cleaning_personal_care_0019_reinforced_bucket"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make reinforced cotton bucket for domestic hygiene [craft_industrialised_cleaning_personal_care_0019_reinforced_bucket]",
		Category = "Cleaning and personal care",
		Blurb = "Make a reinforced cotton bucket for domestic hygiene from prepared cotton stock.",
		Action = "making reinforced cotton bucket for domestic hygiene",
		ActiveCraftItemSdesc = "an in-progress reinforced cotton bucket for domestic hygiene craft",
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
			new CatalogueCommodityRecipeInput(Grams: 1495.0, Material: "cotton", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1495 grams of cotton", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_cleaning_personal_care_0019_reinforced_bucket", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a reinforced cotton bucket for domestic hygiene (@industrialised_cleaning_personal_care_0019_reinforced_bucket)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition CleaningAndPersonalCareMakeReinforcedMildSteelSoapdishForDomesticHygieneCraftIndustrialisedCleaningPersonalCare0020ReinforcedSoapdish() => new()
	{
		Metadata = new("Cleaning and personal care/make reinforced mild steel soapdish for domestic hygiene [craft_industrialised_cleaning_personal_care_0020_reinforced_soapdish]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_cleaning_personal_care_0020_reinforced_soapdish"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make reinforced mild steel soapdish for domestic hygiene [craft_industrialised_cleaning_personal_care_0020_reinforced_soapdish]",
		Category = "Cleaning and personal care",
		Blurb = "Make a reinforced mild steel soapdish for domestic hygiene from prepared mild steel stock.",
		Action = "making reinforced mild steel soapdish for domestic hygiene",
		ActiveCraftItemSdesc = "an in-progress reinforced mild steel soapdish for domestic hygiene craft",
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
			new CatalogueCommodityRecipeInput(Grams: 1536.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1536 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_cleaning_personal_care_0020_reinforced_soapdish", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a reinforced mild steel soapdish for domestic hygiene (@industrialised_cleaning_personal_care_0020_reinforced_soapdish)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition CleaningAndPersonalCareMakeCompactCeramicWasteForDomesticHygieneCraftIndustrialisedCleaningPersonalCare0021CompactWaste() => new()
	{
		Metadata = new("Cleaning and personal care/make compact ceramic waste for domestic hygiene [craft_industrialised_cleaning_personal_care_0021_compact_waste]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_cleaning_personal_care_0021_compact_waste"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make compact ceramic waste for domestic hygiene [craft_industrialised_cleaning_personal_care_0021_compact_waste]",
		Category = "Cleaning and personal care",
		Blurb = "Make a compact ceramic waste for domestic hygiene from prepared ceramic stock.",
		Action = "making compact ceramic waste for domestic hygiene",
		ActiveCraftItemSdesc = "an in-progress compact ceramic waste for domestic hygiene craft",
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
			new CatalogueCommodityRecipeInput(Grams: 1576.0, Material: "ceramic", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1576 grams of ceramic", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_cleaning_personal_care_0021_compact_waste", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a compact ceramic waste for domestic hygiene (@industrialised_cleaning_personal_care_0021_compact_waste)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition CleaningAndPersonalCareMakeCompactOakBrushForDomesticHygieneCraftIndustrialisedCleaningPersonalCare0022CompactBrush() => new()
	{
		Metadata = new("Cleaning and personal care/make compact oak brush for domestic hygiene [craft_industrialised_cleaning_personal_care_0022_compact_brush]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_cleaning_personal_care_0022_compact_brush"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make compact oak brush for domestic hygiene [craft_industrialised_cleaning_personal_care_0022_compact_brush]",
		Category = "Cleaning and personal care",
		Blurb = "Make a compact oak brush for domestic hygiene from prepared oak stock.",
		Action = "making compact oak brush for domestic hygiene",
		ActiveCraftItemSdesc = "an in-progress compact oak brush for domestic hygiene craft",
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
			new CatalogueCommodityRecipeInput(Grams: 1617.0, Material: "oak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1617 grams of oak", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_cleaning_personal_care_0022_compact_brush", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a compact oak brush for domestic hygiene (@industrialised_cleaning_personal_care_0022_compact_brush)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition CleaningAndPersonalCareMakeCompactCottonCombForDomesticHygieneCraftIndustrialisedCleaningPersonalCare0023CompactComb() => new()
	{
		Metadata = new("Cleaning and personal care/make compact cotton comb for domestic hygiene [craft_industrialised_cleaning_personal_care_0023_compact_comb]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_cleaning_personal_care_0023_compact_comb"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make compact cotton comb for domestic hygiene [craft_industrialised_cleaning_personal_care_0023_compact_comb]",
		Category = "Cleaning and personal care",
		Blurb = "Make a compact cotton comb for domestic hygiene from prepared cotton stock.",
		Action = "making compact cotton comb for domestic hygiene",
		ActiveCraftItemSdesc = "an in-progress compact cotton comb for domestic hygiene craft",
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
			new CatalogueCommodityRecipeInput(Grams: 1658.0, Material: "cotton", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1658 grams of cotton", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_cleaning_personal_care_0023_compact_comb", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a compact cotton comb for domestic hygiene (@industrialised_cleaning_personal_care_0023_compact_comb)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition CleaningAndPersonalCareMakeCompactMildSteelRazorForDomesticHygieneCraftIndustrialisedCleaningPersonalCare0024CompactRazor() => new()
	{
		Metadata = new("Cleaning and personal care/make compact mild steel razor for domestic hygiene [craft_industrialised_cleaning_personal_care_0024_compact_razor]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_cleaning_personal_care_0024_compact_razor"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make compact mild steel razor for domestic hygiene [craft_industrialised_cleaning_personal_care_0024_compact_razor]",
		Category = "Cleaning and personal care",
		Blurb = "Make a compact mild steel razor for domestic hygiene from prepared mild steel stock.",
		Action = "making compact mild steel razor for domestic hygiene",
		ActiveCraftItemSdesc = "an in-progress compact mild steel razor for domestic hygiene craft",
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
			new CatalogueCommodityRecipeInput(Grams: 1698.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1698 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_cleaning_personal_care_0024_compact_razor", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a compact mild steel razor for domestic hygiene (@industrialised_cleaning_personal_care_0024_compact_razor)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition CleaningAndPersonalCareMakeCompactCeramicSpongeForDomesticHygieneCraftIndustrialisedCleaningPersonalCare0025CompactSponge() => new()
	{
		Metadata = new("Cleaning and personal care/make compact ceramic sponge for domestic hygiene [craft_industrialised_cleaning_personal_care_0025_compact_sponge]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_cleaning_personal_care_0025_compact_sponge"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make compact ceramic sponge for domestic hygiene [craft_industrialised_cleaning_personal_care_0025_compact_sponge]",
		Category = "Cleaning and personal care",
		Blurb = "Make a compact ceramic sponge for domestic hygiene from prepared ceramic stock.",
		Action = "making compact ceramic sponge for domestic hygiene",
		ActiveCraftItemSdesc = "an in-progress compact ceramic sponge for domestic hygiene craft",
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
			new CatalogueCommodityRecipeInput(Grams: 1739.0, Material: "ceramic", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1739 grams of ceramic", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_cleaning_personal_care_0025_compact_sponge", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a compact ceramic sponge for domestic hygiene (@industrialised_cleaning_personal_care_0025_compact_sponge)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition CleaningAndPersonalCareMakeCompactOakClothForDomesticHygieneCraftIndustrialisedCleaningPersonalCare0026CompactCloth() => new()
	{
		Metadata = new("Cleaning and personal care/make compact oak cloth for domestic hygiene [craft_industrialised_cleaning_personal_care_0026_compact_cloth]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_cleaning_personal_care_0026_compact_cloth"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make compact oak cloth for domestic hygiene [craft_industrialised_cleaning_personal_care_0026_compact_cloth]",
		Category = "Cleaning and personal care",
		Blurb = "Make a compact oak cloth for domestic hygiene from prepared oak stock.",
		Action = "making compact oak cloth for domestic hygiene",
		ActiveCraftItemSdesc = "an in-progress compact oak cloth for domestic hygiene craft",
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
			new CatalogueCommodityRecipeInput(Grams: 1780.0, Material: "oak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1780 grams of oak", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_cleaning_personal_care_0026_compact_cloth", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a compact oak cloth for domestic hygiene (@industrialised_cleaning_personal_care_0026_compact_cloth)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition CleaningAndPersonalCareMakeCompactCottonBroomForDomesticHygieneCraftIndustrialisedCleaningPersonalCare0027CompactBroom() => new()
	{
		Metadata = new("Cleaning and personal care/make compact cotton broom for domestic hygiene [craft_industrialised_cleaning_personal_care_0027_compact_broom]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_cleaning_personal_care_0027_compact_broom"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make compact cotton broom for domestic hygiene [craft_industrialised_cleaning_personal_care_0027_compact_broom]",
		Category = "Cleaning and personal care",
		Blurb = "Make a compact cotton broom for domestic hygiene from prepared cotton stock.",
		Action = "making compact cotton broom for domestic hygiene",
		ActiveCraftItemSdesc = "an in-progress compact cotton broom for domestic hygiene craft",
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
			new CatalogueCommodityRecipeInput(Grams: 1821.0, Material: "cotton", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1821 grams of cotton", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_cleaning_personal_care_0027_compact_broom", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a compact cotton broom for domestic hygiene (@industrialised_cleaning_personal_care_0027_compact_broom)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition CleaningAndPersonalCareMakeCompactMildSteelMopForDomesticHygieneCraftIndustrialisedCleaningPersonalCare0028CompactMop() => new()
	{
		Metadata = new("Cleaning and personal care/make compact mild steel mop for domestic hygiene [craft_industrialised_cleaning_personal_care_0028_compact_mop]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_cleaning_personal_care_0028_compact_mop"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make compact mild steel mop for domestic hygiene [craft_industrialised_cleaning_personal_care_0028_compact_mop]",
		Category = "Cleaning and personal care",
		Blurb = "Make a compact mild steel mop for domestic hygiene from prepared mild steel stock.",
		Action = "making compact mild steel mop for domestic hygiene",
		ActiveCraftItemSdesc = "an in-progress compact mild steel mop for domestic hygiene craft",
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
			new CatalogueCommodityRecipeInput(Grams: 1861.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1861 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_cleaning_personal_care_0028_compact_mop", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a compact mild steel mop for domestic hygiene (@industrialised_cleaning_personal_care_0028_compact_mop)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition CleaningAndPersonalCareMakeCompactCeramicBucketForDomesticHygieneCraftIndustrialisedCleaningPersonalCare0029CompactBucket() => new()
	{
		Metadata = new("Cleaning and personal care/make compact ceramic bucket for domestic hygiene [craft_industrialised_cleaning_personal_care_0029_compact_bucket]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_cleaning_personal_care_0029_compact_bucket"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make compact ceramic bucket for domestic hygiene [craft_industrialised_cleaning_personal_care_0029_compact_bucket]",
		Category = "Cleaning and personal care",
		Blurb = "Make a compact ceramic bucket for domestic hygiene from prepared ceramic stock.",
		Action = "making compact ceramic bucket for domestic hygiene",
		ActiveCraftItemSdesc = "an in-progress compact ceramic bucket for domestic hygiene craft",
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
			new CatalogueCommodityRecipeInput(Grams: 1902.0, Material: "ceramic", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1902 grams of ceramic", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_cleaning_personal_care_0029_compact_bucket", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a compact ceramic bucket for domestic hygiene (@industrialised_cleaning_personal_care_0029_compact_bucket)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition CleaningAndPersonalCareMakeCompactOakSoapdishForDomesticHygieneCraftIndustrialisedCleaningPersonalCare0030CompactSoapdish() => new()
	{
		Metadata = new("Cleaning and personal care/make compact oak soapdish for domestic hygiene [craft_industrialised_cleaning_personal_care_0030_compact_soapdish]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_cleaning_personal_care_0030_compact_soapdish"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make compact oak soapdish for domestic hygiene [craft_industrialised_cleaning_personal_care_0030_compact_soapdish]",
		Category = "Cleaning and personal care",
		Blurb = "Make a compact oak soapdish for domestic hygiene from prepared oak stock.",
		Action = "making compact oak soapdish for domestic hygiene",
		ActiveCraftItemSdesc = "an in-progress compact oak soapdish for domestic hygiene craft",
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
			new CatalogueCommodityRecipeInput(Grams: 1943.0, Material: "oak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1943 grams of oak", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_cleaning_personal_care_0030_compact_soapdish", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a compact oak soapdish for domestic hygiene (@industrialised_cleaning_personal_care_0030_compact_soapdish)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition CleaningAndPersonalCareMakeHeavyDutyCottonWasteForDomesticHygieneCraftIndustrialisedCleaningPersonalCare0031HeavyDutyWaste() => new()
	{
		Metadata = new("Cleaning and personal care/make heavy-duty cotton waste for domestic hygiene [craft_industrialised_cleaning_personal_care_0031_heavy_duty_waste]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_cleaning_personal_care_0031_heavy_duty_waste"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make heavy-duty cotton waste for domestic hygiene [craft_industrialised_cleaning_personal_care_0031_heavy_duty_waste]",
		Category = "Cleaning and personal care",
		Blurb = "Make a heavy-duty cotton waste for domestic hygiene from prepared cotton stock.",
		Action = "making heavy-duty cotton waste for domestic hygiene",
		ActiveCraftItemSdesc = "an in-progress heavy-duty cotton waste for domestic hygiene craft",
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
			new CatalogueCommodityRecipeInput(Grams: 1983.0, Material: "cotton", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1983 grams of cotton", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_cleaning_personal_care_0031_heavy_duty_waste", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a heavy-duty cotton waste for domestic hygiene (@industrialised_cleaning_personal_care_0031_heavy_duty_waste)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition CleaningAndPersonalCareMakeHeavyDutyMildSteelBrushForDomesticHygieneCraftIndustrialisedCleaningPersonalCare0032HeavyDutyBrush() => new()
	{
		Metadata = new("Cleaning and personal care/make heavy-duty mild steel brush for domestic hygiene [craft_industrialised_cleaning_personal_care_0032_heavy_duty_brush]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_cleaning_personal_care_0032_heavy_duty_brush"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make heavy-duty mild steel brush for domestic hygiene [craft_industrialised_cleaning_personal_care_0032_heavy_duty_brush]",
		Category = "Cleaning and personal care",
		Blurb = "Make a heavy-duty mild steel brush for domestic hygiene from prepared mild steel stock.",
		Action = "making heavy-duty mild steel brush for domestic hygiene",
		ActiveCraftItemSdesc = "an in-progress heavy-duty mild steel brush for domestic hygiene craft",
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
			new CatalogueCommodityRecipeInput(Grams: 2024.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2024 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_cleaning_personal_care_0032_heavy_duty_brush", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a heavy-duty mild steel brush for domestic hygiene (@industrialised_cleaning_personal_care_0032_heavy_duty_brush)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition CleaningAndPersonalCareMakeHeavyDutyCeramicCombForDomesticHygieneCraftIndustrialisedCleaningPersonalCare0033HeavyDutyComb() => new()
	{
		Metadata = new("Cleaning and personal care/make heavy-duty ceramic comb for domestic hygiene [craft_industrialised_cleaning_personal_care_0033_heavy_duty_comb]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_cleaning_personal_care_0033_heavy_duty_comb"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make heavy-duty ceramic comb for domestic hygiene [craft_industrialised_cleaning_personal_care_0033_heavy_duty_comb]",
		Category = "Cleaning and personal care",
		Blurb = "Make a heavy-duty ceramic comb for domestic hygiene from prepared ceramic stock.",
		Action = "making heavy-duty ceramic comb for domestic hygiene",
		ActiveCraftItemSdesc = "an in-progress heavy-duty ceramic comb for domestic hygiene craft",
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
			new CatalogueCommodityRecipeInput(Grams: 2065.0, Material: "ceramic", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2065 grams of ceramic", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_cleaning_personal_care_0033_heavy_duty_comb", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a heavy-duty ceramic comb for domestic hygiene (@industrialised_cleaning_personal_care_0033_heavy_duty_comb)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};
}
