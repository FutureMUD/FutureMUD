#nullable enable

using MudSharp.Body;
using MudSharp.GameItems.Inventory.Plans;
using MudSharp.RPG.Checks;

namespace DatabaseSeeder.Seeders;

public static partial class Recipes20Stock
{
	public static CatalogueRecipeDefinition FoodDrinkAndPackagedConsumablesMakeWeatherproofPaperSweetForDomesticProvisioningCraftIndustrialisedFoodDrinkPackaged0106WeatherproofSweet() => new()
	{
		Metadata = new("Food, drink and packaged consumables/make weatherproof paper sweet for domestic provisioning [craft_industrialised_food_drink_packaged_0106_weatherproof_sweet]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_food_drink_packaged_0106_weatherproof_sweet"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make weatherproof paper sweet for domestic provisioning [craft_industrialised_food_drink_packaged_0106_weatherproof_sweet]",
		Category = "Food, drink and packaged consumables",
		Blurb = "Make a weatherproof paper sweet for domestic provisioning from prepared paper stock.",
		Action = "making weatherproof paper sweet for domestic provisioning",
		ActiveCraftItemSdesc = "an in-progress weatherproof paper sweet for domestic provisioning craft",
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
			new CatalogueCommodityRecipeInput(Grams: 4925.0, Material: "paper", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("4925 grams of paper", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_food_drink_packaged_0106_weatherproof_sweet", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a weatherproof paper sweet for domestic provisioning (@industrialised_food_drink_packaged_0106_weatherproof_sweet)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodDrinkAndPackagedConsumablesMakeWeatherproofCottonCrackerForDomesticProvisioningCraftIndustrialisedFoodDrinkPackaged0107WeatherproofCracker() => new()
	{
		Metadata = new("Food, drink and packaged consumables/make weatherproof cotton cracker for domestic provisioning [craft_industrialised_food_drink_packaged_0107_weatherproof_cracker]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_food_drink_packaged_0107_weatherproof_cracker"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make weatherproof cotton cracker for domestic provisioning [craft_industrialised_food_drink_packaged_0107_weatherproof_cracker]",
		Category = "Food, drink and packaged consumables",
		Blurb = "Make a weatherproof cotton cracker for domestic provisioning from prepared cotton stock.",
		Action = "making weatherproof cotton cracker for domestic provisioning",
		ActiveCraftItemSdesc = "an in-progress weatherproof cotton cracker for domestic provisioning craft",
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
			new CatalogueCommodityRecipeInput(Grams: 4965.0, Material: "cotton", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("4965 grams of cotton", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_food_drink_packaged_0107_weatherproof_cracker", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a weatherproof cotton cracker for domestic provisioning (@industrialised_food_drink_packaged_0107_weatherproof_cracker)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodDrinkAndPackagedConsumablesMakeWeatherproofCeramicPacketForDomesticProvisioningCraftIndustrialisedFoodDrinkPackaged0108WeatherproofPacket() => new()
	{
		Metadata = new("Food, drink and packaged consumables/make weatherproof ceramic packet for domestic provisioning [craft_industrialised_food_drink_packaged_0108_weatherproof_packet]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_food_drink_packaged_0108_weatherproof_packet"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make weatherproof ceramic packet for domestic provisioning [craft_industrialised_food_drink_packaged_0108_weatherproof_packet]",
		Category = "Food, drink and packaged consumables",
		Blurb = "Make a weatherproof ceramic packet for domestic provisioning from prepared ceramic stock.",
		Action = "making weatherproof ceramic packet for domestic provisioning",
		ActiveCraftItemSdesc = "an in-progress weatherproof ceramic packet for domestic provisioning craft",
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
			new CatalogueCommodityRecipeInput(Grams: 5006.0, Material: "ceramic", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("5006 grams of ceramic", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_food_drink_packaged_0108_weatherproof_packet", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a weatherproof ceramic packet for domestic provisioning (@industrialised_food_drink_packaged_0108_weatherproof_packet)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodDrinkAndPackagedConsumablesMakeSomeServicePaperCrumbsForDomesticProvisioningCraftIndustrialisedFoodDrinkPackaged0109ServiceCrumbs() => new()
	{
		Metadata = new("Food, drink and packaged consumables/make some service paper crumbs for domestic provisioning [craft_industrialised_food_drink_packaged_0109_service_crumbs]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_food_drink_packaged_0109_service_crumbs"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make some service paper crumbs for domestic provisioning [craft_industrialised_food_drink_packaged_0109_service_crumbs]",
		Category = "Food, drink and packaged consumables",
		Blurb = "Make some service paper crumbs for domestic provisioning from prepared paper stock.",
		Action = "making some service paper crumbs for domestic provisioning",
		ActiveCraftItemSdesc = "an in-progress some service paper crumbs for domestic provisioning craft",
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
			new CatalogueCommodityRecipeInput(Grams: 5047.0, Material: "paper", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("5047 grams of paper", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_food_drink_packaged_0109_service_crumbs", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x some service paper crumbs for domestic provisioning (@industrialised_food_drink_packaged_0109_service_crumbs)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodDrinkAndPackagedConsumablesMakeServiceCottonBiscuitForDomesticProvisioningCraftIndustrialisedFoodDrinkPackaged0110ServiceBiscuit() => new()
	{
		Metadata = new("Food, drink and packaged consumables/make service cotton biscuit for domestic provisioning [craft_industrialised_food_drink_packaged_0110_service_biscuit]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_food_drink_packaged_0110_service_biscuit"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make service cotton biscuit for domestic provisioning [craft_industrialised_food_drink_packaged_0110_service_biscuit]",
		Category = "Food, drink and packaged consumables",
		Blurb = "Make a service cotton biscuit for domestic provisioning from prepared cotton stock.",
		Action = "making service cotton biscuit for domestic provisioning",
		ActiveCraftItemSdesc = "an in-progress service cotton biscuit for domestic provisioning craft",
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
			new CatalogueCommodityRecipeInput(Grams: 5088.0, Material: "cotton", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("5088 grams of cotton", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_food_drink_packaged_0110_service_biscuit", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a service cotton biscuit for domestic provisioning (@industrialised_food_drink_packaged_0110_service_biscuit)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodDrinkAndPackagedConsumablesMakeServiceCeramicRationForDomesticProvisioningCraftIndustrialisedFoodDrinkPackaged0111ServiceRation() => new()
	{
		Metadata = new("Food, drink and packaged consumables/make service ceramic ration for domestic provisioning [craft_industrialised_food_drink_packaged_0111_service_ration]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_food_drink_packaged_0111_service_ration"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make service ceramic ration for domestic provisioning [craft_industrialised_food_drink_packaged_0111_service_ration]",
		Category = "Food, drink and packaged consumables",
		Blurb = "Make a service ceramic ration for domestic provisioning from prepared ceramic stock.",
		Action = "making service ceramic ration for domestic provisioning",
		ActiveCraftItemSdesc = "an in-progress service ceramic ration for domestic provisioning craft",
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
			new CatalogueCommodityRecipeInput(Grams: 5128.0, Material: "ceramic", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("5128 grams of ceramic", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_food_drink_packaged_0111_service_ration", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a service ceramic ration for domestic provisioning (@industrialised_food_drink_packaged_0111_service_ration)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodDrinkAndPackagedConsumablesMakeServicePaperPreserveForDomesticProvisioningCraftIndustrialisedFoodDrinkPackaged0112ServicePreserve() => new()
	{
		Metadata = new("Food, drink and packaged consumables/make service paper preserve for domestic provisioning [craft_industrialised_food_drink_packaged_0112_service_preserve]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_food_drink_packaged_0112_service_preserve"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make service paper preserve for domestic provisioning [craft_industrialised_food_drink_packaged_0112_service_preserve]",
		Category = "Food, drink and packaged consumables",
		Blurb = "Make a service paper preserve for domestic provisioning from prepared paper stock.",
		Action = "making service paper preserve for domestic provisioning",
		ActiveCraftItemSdesc = "an in-progress service paper preserve for domestic provisioning craft",
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
			new CatalogueCommodityRecipeInput(Grams: 5169.0, Material: "paper", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("5169 grams of paper", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_food_drink_packaged_0112_service_preserve", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a service paper preserve for domestic provisioning (@industrialised_food_drink_packaged_0112_service_preserve)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodDrinkAndPackagedConsumablesMakeServiceCottonLoafForDomesticProvisioningCraftIndustrialisedFoodDrinkPackaged0113ServiceLoaf() => new()
	{
		Metadata = new("Food, drink and packaged consumables/make service cotton loaf for domestic provisioning [craft_industrialised_food_drink_packaged_0113_service_loaf]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_food_drink_packaged_0113_service_loaf"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make service cotton loaf for domestic provisioning [craft_industrialised_food_drink_packaged_0113_service_loaf]",
		Category = "Food, drink and packaged consumables",
		Blurb = "Make a service cotton loaf for domestic provisioning from prepared cotton stock.",
		Action = "making service cotton loaf for domestic provisioning",
		ActiveCraftItemSdesc = "an in-progress service cotton loaf for domestic provisioning craft",
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
			new CatalogueCommodityRecipeInput(Grams: 5210.0, Material: "cotton", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("5210 grams of cotton", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_food_drink_packaged_0113_service_loaf", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a service cotton loaf for domestic provisioning (@industrialised_food_drink_packaged_0113_service_loaf)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodDrinkAndPackagedConsumablesMakeServiceCeramicCakeForDomesticProvisioningCraftIndustrialisedFoodDrinkPackaged0114ServiceCake() => new()
	{
		Metadata = new("Food, drink and packaged consumables/make service ceramic cake for domestic provisioning [craft_industrialised_food_drink_packaged_0114_service_cake]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_food_drink_packaged_0114_service_cake"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make service ceramic cake for domestic provisioning [craft_industrialised_food_drink_packaged_0114_service_cake]",
		Category = "Food, drink and packaged consumables",
		Blurb = "Make a service ceramic cake for domestic provisioning from prepared ceramic stock.",
		Action = "making service ceramic cake for domestic provisioning",
		ActiveCraftItemSdesc = "an in-progress service ceramic cake for domestic provisioning craft",
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
			new CatalogueCommodityRecipeInput(Grams: 5250.0, Material: "ceramic", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("5250 grams of ceramic", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_food_drink_packaged_0114_service_cake", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a service ceramic cake for domestic provisioning (@industrialised_food_drink_packaged_0114_service_cake)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodDrinkAndPackagedConsumablesMakeServicePaperPuddingForDomesticProvisioningCraftIndustrialisedFoodDrinkPackaged0115ServicePudding() => new()
	{
		Metadata = new("Food, drink and packaged consumables/make service paper pudding for domestic provisioning [craft_industrialised_food_drink_packaged_0115_service_pudding]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_food_drink_packaged_0115_service_pudding"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make service paper pudding for domestic provisioning [craft_industrialised_food_drink_packaged_0115_service_pudding]",
		Category = "Food, drink and packaged consumables",
		Blurb = "Make a service paper pudding for domestic provisioning from prepared paper stock.",
		Action = "making service paper pudding for domestic provisioning",
		ActiveCraftItemSdesc = "an in-progress service paper pudding for domestic provisioning craft",
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
			new CatalogueCommodityRecipeInput(Grams: 5291.0, Material: "paper", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("5291 grams of paper", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_food_drink_packaged_0115_service_pudding", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a service paper pudding for domestic provisioning (@industrialised_food_drink_packaged_0115_service_pudding)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodDrinkAndPackagedConsumablesMakeServiceCottonSausageForDomesticProvisioningCraftIndustrialisedFoodDrinkPackaged0116ServiceSausage() => new()
	{
		Metadata = new("Food, drink and packaged consumables/make service cotton sausage for domestic provisioning [craft_industrialised_food_drink_packaged_0116_service_sausage]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_food_drink_packaged_0116_service_sausage"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make service cotton sausage for domestic provisioning [craft_industrialised_food_drink_packaged_0116_service_sausage]",
		Category = "Food, drink and packaged consumables",
		Blurb = "Make a service cotton sausage for domestic provisioning from prepared cotton stock.",
		Action = "making service cotton sausage for domestic provisioning",
		ActiveCraftItemSdesc = "an in-progress service cotton sausage for domestic provisioning craft",
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
			new CatalogueCommodityRecipeInput(Grams: 5332.0, Material: "cotton", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("5332 grams of cotton", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_food_drink_packaged_0116_service_sausage", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a service cotton sausage for domestic provisioning (@industrialised_food_drink_packaged_0116_service_sausage)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodDrinkAndPackagedConsumablesMakeServiceCeramicPickleForDomesticProvisioningCraftIndustrialisedFoodDrinkPackaged0117ServicePickle() => new()
	{
		Metadata = new("Food, drink and packaged consumables/make service ceramic pickle for domestic provisioning [craft_industrialised_food_drink_packaged_0117_service_pickle]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_food_drink_packaged_0117_service_pickle"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make service ceramic pickle for domestic provisioning [craft_industrialised_food_drink_packaged_0117_service_pickle]",
		Category = "Food, drink and packaged consumables",
		Blurb = "Make a service ceramic pickle for domestic provisioning from prepared ceramic stock.",
		Action = "making service ceramic pickle for domestic provisioning",
		ActiveCraftItemSdesc = "an in-progress service ceramic pickle for domestic provisioning craft",
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
			new CatalogueCommodityRecipeInput(Grams: 5372.0, Material: "ceramic", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("5372 grams of ceramic", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_food_drink_packaged_0117_service_pickle", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a service ceramic pickle for domestic provisioning (@industrialised_food_drink_packaged_0117_service_pickle)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodDrinkAndPackagedConsumablesMakeServicePaperSweetForDomesticProvisioningCraftIndustrialisedFoodDrinkPackaged0118ServiceSweet() => new()
	{
		Metadata = new("Food, drink and packaged consumables/make service paper sweet for domestic provisioning [craft_industrialised_food_drink_packaged_0118_service_sweet]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_food_drink_packaged_0118_service_sweet"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make service paper sweet for domestic provisioning [craft_industrialised_food_drink_packaged_0118_service_sweet]",
		Category = "Food, drink and packaged consumables",
		Blurb = "Make a service paper sweet for domestic provisioning from prepared paper stock.",
		Action = "making service paper sweet for domestic provisioning",
		ActiveCraftItemSdesc = "an in-progress service paper sweet for domestic provisioning craft",
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
			new CatalogueCommodityRecipeInput(Grams: 5413.0, Material: "paper", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("5413 grams of paper", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_food_drink_packaged_0118_service_sweet", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a service paper sweet for domestic provisioning (@industrialised_food_drink_packaged_0118_service_sweet)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodDrinkAndPackagedConsumablesMakeServiceCottonCrackerForDomesticProvisioningCraftIndustrialisedFoodDrinkPackaged0119ServiceCracker() => new()
	{
		Metadata = new("Food, drink and packaged consumables/make service cotton cracker for domestic provisioning [craft_industrialised_food_drink_packaged_0119_service_cracker]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_food_drink_packaged_0119_service_cracker"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make service cotton cracker for domestic provisioning [craft_industrialised_food_drink_packaged_0119_service_cracker]",
		Category = "Food, drink and packaged consumables",
		Blurb = "Make a service cotton cracker for domestic provisioning from prepared cotton stock.",
		Action = "making service cotton cracker for domestic provisioning",
		ActiveCraftItemSdesc = "an in-progress service cotton cracker for domestic provisioning craft",
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
			new CatalogueCommodityRecipeInput(Grams: 5454.0, Material: "cotton", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("5454 grams of cotton", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_food_drink_packaged_0119_service_cracker", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a service cotton cracker for domestic provisioning (@industrialised_food_drink_packaged_0119_service_cracker)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodDrinkAndPackagedConsumablesMakeServiceCeramicPacketForDomesticProvisioningCraftIndustrialisedFoodDrinkPackaged0120ServicePacket() => new()
	{
		Metadata = new("Food, drink and packaged consumables/make service ceramic packet for domestic provisioning [craft_industrialised_food_drink_packaged_0120_service_packet]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_food_drink_packaged_0120_service_packet"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make service ceramic packet for domestic provisioning [craft_industrialised_food_drink_packaged_0120_service_packet]",
		Category = "Food, drink and packaged consumables",
		Blurb = "Make a service ceramic packet for domestic provisioning from prepared ceramic stock.",
		Action = "making service ceramic packet for domestic provisioning",
		ActiveCraftItemSdesc = "an in-progress service ceramic packet for domestic provisioning craft",
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
			new CatalogueCommodityRecipeInput(Grams: 5494.0, Material: "ceramic", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("5494 grams of ceramic", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_food_drink_packaged_0120_service_packet", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a service ceramic packet for domestic provisioning (@industrialised_food_drink_packaged_0120_service_packet)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodDrinkAndPackagedConsumablesMakeSomePrecisionPaperCrumbsForDomesticProvisioningCraftIndustrialisedFoodDrinkPackaged0121PrecisionCrumbs() => new()
	{
		Metadata = new("Food, drink and packaged consumables/make some precision paper crumbs for domestic provisioning [craft_industrialised_food_drink_packaged_0121_precision_crumbs]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_food_drink_packaged_0121_precision_crumbs"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make some precision paper crumbs for domestic provisioning [craft_industrialised_food_drink_packaged_0121_precision_crumbs]",
		Category = "Food, drink and packaged consumables",
		Blurb = "Make some precision paper crumbs for domestic provisioning from prepared paper stock.",
		Action = "making some precision paper crumbs for domestic provisioning",
		ActiveCraftItemSdesc = "an in-progress some precision paper crumbs for domestic provisioning craft",
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
			new CatalogueCommodityRecipeInput(Grams: 90.0, Material: "paper", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("90 grams of paper", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_food_drink_packaged_0121_precision_crumbs", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x some precision paper crumbs for domestic provisioning (@industrialised_food_drink_packaged_0121_precision_crumbs)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodDrinkAndPackagedConsumablesMakePrecisionCottonBiscuitForDomesticProvisioningCraftIndustrialisedFoodDrinkPackaged0122PrecisionBiscuit() => new()
	{
		Metadata = new("Food, drink and packaged consumables/make precision cotton biscuit for domestic provisioning [craft_industrialised_food_drink_packaged_0122_precision_biscuit]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_food_drink_packaged_0122_precision_biscuit"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make precision cotton biscuit for domestic provisioning [craft_industrialised_food_drink_packaged_0122_precision_biscuit]",
		Category = "Food, drink and packaged consumables",
		Blurb = "Make a precision cotton biscuit for domestic provisioning from prepared cotton stock.",
		Action = "making precision cotton biscuit for domestic provisioning",
		ActiveCraftItemSdesc = "an in-progress precision cotton biscuit for domestic provisioning craft",
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
			new CatalogueCommodityRecipeInput(Grams: 131.0, Material: "cotton", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("131 grams of cotton", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_food_drink_packaged_0122_precision_biscuit", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a precision cotton biscuit for domestic provisioning (@industrialised_food_drink_packaged_0122_precision_biscuit)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodDrinkAndPackagedConsumablesMakePrecisionCeramicRationForDomesticProvisioningCraftIndustrialisedFoodDrinkPackaged0123PrecisionRation() => new()
	{
		Metadata = new("Food, drink and packaged consumables/make precision ceramic ration for domestic provisioning [craft_industrialised_food_drink_packaged_0123_precision_ration]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_food_drink_packaged_0123_precision_ration"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make precision ceramic ration for domestic provisioning [craft_industrialised_food_drink_packaged_0123_precision_ration]",
		Category = "Food, drink and packaged consumables",
		Blurb = "Make a precision ceramic ration for domestic provisioning from prepared ceramic stock.",
		Action = "making precision ceramic ration for domestic provisioning",
		ActiveCraftItemSdesc = "an in-progress precision ceramic ration for domestic provisioning craft",
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
			new CatalogueCommodityRecipeInput(Grams: 172.0, Material: "ceramic", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("172 grams of ceramic", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_food_drink_packaged_0123_precision_ration", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a precision ceramic ration for domestic provisioning (@industrialised_food_drink_packaged_0123_precision_ration)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodDrinkAndPackagedConsumablesMakePrecisionPaperPreserveForDomesticProvisioningCraftIndustrialisedFoodDrinkPackaged0124PrecisionPreserve() => new()
	{
		Metadata = new("Food, drink and packaged consumables/make precision paper preserve for domestic provisioning [craft_industrialised_food_drink_packaged_0124_precision_preserve]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_food_drink_packaged_0124_precision_preserve"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make precision paper preserve for domestic provisioning [craft_industrialised_food_drink_packaged_0124_precision_preserve]",
		Category = "Food, drink and packaged consumables",
		Blurb = "Make a precision paper preserve for domestic provisioning from prepared paper stock.",
		Action = "making precision paper preserve for domestic provisioning",
		ActiveCraftItemSdesc = "an in-progress precision paper preserve for domestic provisioning craft",
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
			new CatalogueCommodityRecipeInput(Grams: 212.0, Material: "paper", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("212 grams of paper", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_food_drink_packaged_0124_precision_preserve", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a precision paper preserve for domestic provisioning (@industrialised_food_drink_packaged_0124_precision_preserve)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodDrinkAndPackagedConsumablesMakePrecisionCottonLoafForDomesticProvisioningCraftIndustrialisedFoodDrinkPackaged0125PrecisionLoaf() => new()
	{
		Metadata = new("Food, drink and packaged consumables/make precision cotton loaf for domestic provisioning [craft_industrialised_food_drink_packaged_0125_precision_loaf]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_food_drink_packaged_0125_precision_loaf"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make precision cotton loaf for domestic provisioning [craft_industrialised_food_drink_packaged_0125_precision_loaf]",
		Category = "Food, drink and packaged consumables",
		Blurb = "Make a precision cotton loaf for domestic provisioning from prepared cotton stock.",
		Action = "making precision cotton loaf for domestic provisioning",
		ActiveCraftItemSdesc = "an in-progress precision cotton loaf for domestic provisioning craft",
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
			new CatalogueCommodityRecipeInput(Grams: 253.0, Material: "cotton", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("253 grams of cotton", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_food_drink_packaged_0125_precision_loaf", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a precision cotton loaf for domestic provisioning (@industrialised_food_drink_packaged_0125_precision_loaf)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodDrinkAndPackagedConsumablesMakePrecisionCeramicCakeForDomesticProvisioningCraftIndustrialisedFoodDrinkPackaged0126PrecisionCake() => new()
	{
		Metadata = new("Food, drink and packaged consumables/make precision ceramic cake for domestic provisioning [craft_industrialised_food_drink_packaged_0126_precision_cake]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_food_drink_packaged_0126_precision_cake"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make precision ceramic cake for domestic provisioning [craft_industrialised_food_drink_packaged_0126_precision_cake]",
		Category = "Food, drink and packaged consumables",
		Blurb = "Make a precision ceramic cake for domestic provisioning from prepared ceramic stock.",
		Action = "making precision ceramic cake for domestic provisioning",
		ActiveCraftItemSdesc = "an in-progress precision ceramic cake for domestic provisioning craft",
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
			new CatalogueCommodityRecipeInput(Grams: 294.0, Material: "ceramic", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("294 grams of ceramic", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_food_drink_packaged_0126_precision_cake", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a precision ceramic cake for domestic provisioning (@industrialised_food_drink_packaged_0126_precision_cake)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodDrinkAndPackagedConsumablesMakePrecisionPaperPuddingForDomesticProvisioningCraftIndustrialisedFoodDrinkPackaged0127PrecisionPudding() => new()
	{
		Metadata = new("Food, drink and packaged consumables/make precision paper pudding for domestic provisioning [craft_industrialised_food_drink_packaged_0127_precision_pudding]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_food_drink_packaged_0127_precision_pudding"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make precision paper pudding for domestic provisioning [craft_industrialised_food_drink_packaged_0127_precision_pudding]",
		Category = "Food, drink and packaged consumables",
		Blurb = "Make a precision paper pudding for domestic provisioning from prepared paper stock.",
		Action = "making precision paper pudding for domestic provisioning",
		ActiveCraftItemSdesc = "an in-progress precision paper pudding for domestic provisioning craft",
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
			new CatalogueCommodityRecipeInput(Grams: 334.0, Material: "paper", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("334 grams of paper", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_food_drink_packaged_0127_precision_pudding", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a precision paper pudding for domestic provisioning (@industrialised_food_drink_packaged_0127_precision_pudding)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodDrinkAndPackagedConsumablesMakePrecisionCottonSausageForDomesticProvisioningCraftIndustrialisedFoodDrinkPackaged0128PrecisionSausage() => new()
	{
		Metadata = new("Food, drink and packaged consumables/make precision cotton sausage for domestic provisioning [craft_industrialised_food_drink_packaged_0128_precision_sausage]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_food_drink_packaged_0128_precision_sausage"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make precision cotton sausage for domestic provisioning [craft_industrialised_food_drink_packaged_0128_precision_sausage]",
		Category = "Food, drink and packaged consumables",
		Blurb = "Make a precision cotton sausage for domestic provisioning from prepared cotton stock.",
		Action = "making precision cotton sausage for domestic provisioning",
		ActiveCraftItemSdesc = "an in-progress precision cotton sausage for domestic provisioning craft",
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
			new CatalogueCommodityRecipeInput(Grams: 375.0, Material: "cotton", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("375 grams of cotton", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_food_drink_packaged_0128_precision_sausage", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a precision cotton sausage for domestic provisioning (@industrialised_food_drink_packaged_0128_precision_sausage)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodDrinkAndPackagedConsumablesMakePrecisionCeramicPickleForDomesticProvisioningCraftIndustrialisedFoodDrinkPackaged0129PrecisionPickle() => new()
	{
		Metadata = new("Food, drink and packaged consumables/make precision ceramic pickle for domestic provisioning [craft_industrialised_food_drink_packaged_0129_precision_pickle]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_food_drink_packaged_0129_precision_pickle"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make precision ceramic pickle for domestic provisioning [craft_industrialised_food_drink_packaged_0129_precision_pickle]",
		Category = "Food, drink and packaged consumables",
		Blurb = "Make a precision ceramic pickle for domestic provisioning from prepared ceramic stock.",
		Action = "making precision ceramic pickle for domestic provisioning",
		ActiveCraftItemSdesc = "an in-progress precision ceramic pickle for domestic provisioning craft",
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
			new CatalogueCommodityRecipeInput(Grams: 416.0, Material: "ceramic", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("416 grams of ceramic", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_food_drink_packaged_0129_precision_pickle", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a precision ceramic pickle for domestic provisioning (@industrialised_food_drink_packaged_0129_precision_pickle)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodDrinkAndPackagedConsumablesMakePrecisionPaperSweetForDomesticProvisioningCraftIndustrialisedFoodDrinkPackaged0130PrecisionSweet() => new()
	{
		Metadata = new("Food, drink and packaged consumables/make precision paper sweet for domestic provisioning [craft_industrialised_food_drink_packaged_0130_precision_sweet]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_food_drink_packaged_0130_precision_sweet"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make precision paper sweet for domestic provisioning [craft_industrialised_food_drink_packaged_0130_precision_sweet]",
		Category = "Food, drink and packaged consumables",
		Blurb = "Make a precision paper sweet for domestic provisioning from prepared paper stock.",
		Action = "making precision paper sweet for domestic provisioning",
		ActiveCraftItemSdesc = "an in-progress precision paper sweet for domestic provisioning craft",
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
			new CatalogueCommodityRecipeInput(Grams: 457.0, Material: "paper", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("457 grams of paper", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_food_drink_packaged_0130_precision_sweet", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a precision paper sweet for domestic provisioning (@industrialised_food_drink_packaged_0130_precision_sweet)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodDrinkAndPackagedConsumablesMakePrecisionCottonCrackerForDomesticProvisioningCraftIndustrialisedFoodDrinkPackaged0131PrecisionCracker() => new()
	{
		Metadata = new("Food, drink and packaged consumables/make precision cotton cracker for domestic provisioning [craft_industrialised_food_drink_packaged_0131_precision_cracker]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_food_drink_packaged_0131_precision_cracker"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make precision cotton cracker for domestic provisioning [craft_industrialised_food_drink_packaged_0131_precision_cracker]",
		Category = "Food, drink and packaged consumables",
		Blurb = "Make a precision cotton cracker for domestic provisioning from prepared cotton stock.",
		Action = "making precision cotton cracker for domestic provisioning",
		ActiveCraftItemSdesc = "an in-progress precision cotton cracker for domestic provisioning craft",
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
			new CatalogueCommodityRecipeInput(Grams: 497.0, Material: "cotton", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("497 grams of cotton", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_food_drink_packaged_0131_precision_cracker", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a precision cotton cracker for domestic provisioning (@industrialised_food_drink_packaged_0131_precision_cracker)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodDrinkAndPackagedConsumablesMakePrecisionCeramicPacketForDomesticProvisioningCraftIndustrialisedFoodDrinkPackaged0132PrecisionPacket() => new()
	{
		Metadata = new("Food, drink and packaged consumables/make precision ceramic packet for domestic provisioning [craft_industrialised_food_drink_packaged_0132_precision_packet]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_food_drink_packaged_0132_precision_packet"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make precision ceramic packet for domestic provisioning [craft_industrialised_food_drink_packaged_0132_precision_packet]",
		Category = "Food, drink and packaged consumables",
		Blurb = "Make a precision ceramic packet for domestic provisioning from prepared ceramic stock.",
		Action = "making precision ceramic packet for domestic provisioning",
		ActiveCraftItemSdesc = "an in-progress precision ceramic packet for domestic provisioning craft",
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
			new CatalogueCommodityRecipeInput(Grams: 538.0, Material: "ceramic", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("538 grams of ceramic", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_food_drink_packaged_0132_precision_packet", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a precision ceramic packet for domestic provisioning (@industrialised_food_drink_packaged_0132_precision_packet)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodDrinkAndPackagedConsumablesMakeSomeEconomyPaperCrumbsForDomesticProvisioningCraftIndustrialisedFoodDrinkPackaged0133EconomyCrumbs() => new()
	{
		Metadata = new("Food, drink and packaged consumables/make some economy paper crumbs for domestic provisioning [craft_industrialised_food_drink_packaged_0133_economy_crumbs]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_food_drink_packaged_0133_economy_crumbs"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make some economy paper crumbs for domestic provisioning [craft_industrialised_food_drink_packaged_0133_economy_crumbs]",
		Category = "Food, drink and packaged consumables",
		Blurb = "Make some economy paper crumbs for domestic provisioning from prepared paper stock.",
		Action = "making some economy paper crumbs for domestic provisioning",
		ActiveCraftItemSdesc = "an in-progress some economy paper crumbs for domestic provisioning craft",
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
			new CatalogueCommodityRecipeInput(Grams: 579.0, Material: "paper", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("579 grams of paper", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_food_drink_packaged_0133_economy_crumbs", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x some economy paper crumbs for domestic provisioning (@industrialised_food_drink_packaged_0133_economy_crumbs)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodDrinkAndPackagedConsumablesMakeEconomyCottonBiscuitForDomesticProvisioningCraftIndustrialisedFoodDrinkPackaged0134EconomyBiscuit() => new()
	{
		Metadata = new("Food, drink and packaged consumables/make economy cotton biscuit for domestic provisioning [craft_industrialised_food_drink_packaged_0134_economy_biscuit]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_food_drink_packaged_0134_economy_biscuit"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make economy cotton biscuit for domestic provisioning [craft_industrialised_food_drink_packaged_0134_economy_biscuit]",
		Category = "Food, drink and packaged consumables",
		Blurb = "Make an economy cotton biscuit for domestic provisioning from prepared cotton stock.",
		Action = "making economy cotton biscuit for domestic provisioning",
		ActiveCraftItemSdesc = "an in-progress economy cotton biscuit for domestic provisioning craft",
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
			new CatalogueCommodityRecipeInput(Grams: 619.0, Material: "cotton", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("619 grams of cotton", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_food_drink_packaged_0134_economy_biscuit", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x an economy cotton biscuit for domestic provisioning (@industrialised_food_drink_packaged_0134_economy_biscuit)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodDrinkAndPackagedConsumablesMakeEconomyCeramicRationForDomesticProvisioningCraftIndustrialisedFoodDrinkPackaged0135EconomyRation() => new()
	{
		Metadata = new("Food, drink and packaged consumables/make economy ceramic ration for domestic provisioning [craft_industrialised_food_drink_packaged_0135_economy_ration]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_food_drink_packaged_0135_economy_ration"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make economy ceramic ration for domestic provisioning [craft_industrialised_food_drink_packaged_0135_economy_ration]",
		Category = "Food, drink and packaged consumables",
		Blurb = "Make an economy ceramic ration for domestic provisioning from prepared ceramic stock.",
		Action = "making economy ceramic ration for domestic provisioning",
		ActiveCraftItemSdesc = "an in-progress economy ceramic ration for domestic provisioning craft",
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
			new CatalogueCommodityRecipeInput(Grams: 660.0, Material: "ceramic", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("660 grams of ceramic", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_food_drink_packaged_0135_economy_ration", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x an economy ceramic ration for domestic provisioning (@industrialised_food_drink_packaged_0135_economy_ration)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodDrinkAndPackagedConsumablesMakeEconomyPaperPreserveForDomesticProvisioningCraftIndustrialisedFoodDrinkPackaged0136EconomyPreserve() => new()
	{
		Metadata = new("Food, drink and packaged consumables/make economy paper preserve for domestic provisioning [craft_industrialised_food_drink_packaged_0136_economy_preserve]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_food_drink_packaged_0136_economy_preserve"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make economy paper preserve for domestic provisioning [craft_industrialised_food_drink_packaged_0136_economy_preserve]",
		Category = "Food, drink and packaged consumables",
		Blurb = "Make an economy paper preserve for domestic provisioning from prepared paper stock.",
		Action = "making economy paper preserve for domestic provisioning",
		ActiveCraftItemSdesc = "an in-progress economy paper preserve for domestic provisioning craft",
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
			new CatalogueCommodityRecipeInput(Grams: 701.0, Material: "paper", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("701 grams of paper", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_food_drink_packaged_0136_economy_preserve", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x an economy paper preserve for domestic provisioning (@industrialised_food_drink_packaged_0136_economy_preserve)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodDrinkAndPackagedConsumablesMakeEconomyCottonLoafForDomesticProvisioningCraftIndustrialisedFoodDrinkPackaged0137EconomyLoaf() => new()
	{
		Metadata = new("Food, drink and packaged consumables/make economy cotton loaf for domestic provisioning [craft_industrialised_food_drink_packaged_0137_economy_loaf]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_food_drink_packaged_0137_economy_loaf"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make economy cotton loaf for domestic provisioning [craft_industrialised_food_drink_packaged_0137_economy_loaf]",
		Category = "Food, drink and packaged consumables",
		Blurb = "Make an economy cotton loaf for domestic provisioning from prepared cotton stock.",
		Action = "making economy cotton loaf for domestic provisioning",
		ActiveCraftItemSdesc = "an in-progress economy cotton loaf for domestic provisioning craft",
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
			new CatalogueCommodityRecipeInput(Grams: 741.0, Material: "cotton", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("741 grams of cotton", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_food_drink_packaged_0137_economy_loaf", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x an economy cotton loaf for domestic provisioning (@industrialised_food_drink_packaged_0137_economy_loaf)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodDrinkAndPackagedConsumablesMakeEconomyCeramicCakeForDomesticProvisioningCraftIndustrialisedFoodDrinkPackaged0138EconomyCake() => new()
	{
		Metadata = new("Food, drink and packaged consumables/make economy ceramic cake for domestic provisioning [craft_industrialised_food_drink_packaged_0138_economy_cake]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_food_drink_packaged_0138_economy_cake"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make economy ceramic cake for domestic provisioning [craft_industrialised_food_drink_packaged_0138_economy_cake]",
		Category = "Food, drink and packaged consumables",
		Blurb = "Make an economy ceramic cake for domestic provisioning from prepared ceramic stock.",
		Action = "making economy ceramic cake for domestic provisioning",
		ActiveCraftItemSdesc = "an in-progress economy ceramic cake for domestic provisioning craft",
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
			new CatalogueCommodityRecipeInput(Grams: 782.0, Material: "ceramic", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("782 grams of ceramic", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_food_drink_packaged_0138_economy_cake", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x an economy ceramic cake for domestic provisioning (@industrialised_food_drink_packaged_0138_economy_cake)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodDrinkAndPackagedConsumablesMakeEconomyPaperPuddingForDomesticProvisioningCraftIndustrialisedFoodDrinkPackaged0139EconomyPudding() => new()
	{
		Metadata = new("Food, drink and packaged consumables/make economy paper pudding for domestic provisioning [craft_industrialised_food_drink_packaged_0139_economy_pudding]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_food_drink_packaged_0139_economy_pudding"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make economy paper pudding for domestic provisioning [craft_industrialised_food_drink_packaged_0139_economy_pudding]",
		Category = "Food, drink and packaged consumables",
		Blurb = "Make an economy paper pudding for domestic provisioning from prepared paper stock.",
		Action = "making economy paper pudding for domestic provisioning",
		ActiveCraftItemSdesc = "an in-progress economy paper pudding for domestic provisioning craft",
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
			new CatalogueCommodityRecipeInput(Grams: 823.0, Material: "paper", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("823 grams of paper", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_food_drink_packaged_0139_economy_pudding", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x an economy paper pudding for domestic provisioning (@industrialised_food_drink_packaged_0139_economy_pudding)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodDrinkAndPackagedConsumablesMakeEconomyCottonSausageForDomesticProvisioningCraftIndustrialisedFoodDrinkPackaged0140EconomySausage() => new()
	{
		Metadata = new("Food, drink and packaged consumables/make economy cotton sausage for domestic provisioning [craft_industrialised_food_drink_packaged_0140_economy_sausage]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_food_drink_packaged_0140_economy_sausage"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make economy cotton sausage for domestic provisioning [craft_industrialised_food_drink_packaged_0140_economy_sausage]",
		Category = "Food, drink and packaged consumables",
		Blurb = "Make an economy cotton sausage for domestic provisioning from prepared cotton stock.",
		Action = "making economy cotton sausage for domestic provisioning",
		ActiveCraftItemSdesc = "an in-progress economy cotton sausage for domestic provisioning craft",
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
			new CatalogueCommodityRecipeInput(Grams: 864.0, Material: "cotton", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("864 grams of cotton", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_food_drink_packaged_0140_economy_sausage", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x an economy cotton sausage for domestic provisioning (@industrialised_food_drink_packaged_0140_economy_sausage)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodDrinkAndPackagedConsumablesMakeEconomyCeramicPickleForDomesticProvisioningCraftIndustrialisedFoodDrinkPackaged0141EconomyPickle() => new()
	{
		Metadata = new("Food, drink and packaged consumables/make economy ceramic pickle for domestic provisioning [craft_industrialised_food_drink_packaged_0141_economy_pickle]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_food_drink_packaged_0141_economy_pickle"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make economy ceramic pickle for domestic provisioning [craft_industrialised_food_drink_packaged_0141_economy_pickle]",
		Category = "Food, drink and packaged consumables",
		Blurb = "Make an economy ceramic pickle for domestic provisioning from prepared ceramic stock.",
		Action = "making economy ceramic pickle for domestic provisioning",
		ActiveCraftItemSdesc = "an in-progress economy ceramic pickle for domestic provisioning craft",
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
			new CatalogueCommodityRecipeInput(Grams: 904.0, Material: "ceramic", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("904 grams of ceramic", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_food_drink_packaged_0141_economy_pickle", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x an economy ceramic pickle for domestic provisioning (@industrialised_food_drink_packaged_0141_economy_pickle)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodDrinkAndPackagedConsumablesMakeEconomyPaperSweetForDomesticProvisioningCraftIndustrialisedFoodDrinkPackaged0142EconomySweet() => new()
	{
		Metadata = new("Food, drink and packaged consumables/make economy paper sweet for domestic provisioning [craft_industrialised_food_drink_packaged_0142_economy_sweet]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_food_drink_packaged_0142_economy_sweet"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make economy paper sweet for domestic provisioning [craft_industrialised_food_drink_packaged_0142_economy_sweet]",
		Category = "Food, drink and packaged consumables",
		Blurb = "Make an economy paper sweet for domestic provisioning from prepared paper stock.",
		Action = "making economy paper sweet for domestic provisioning",
		ActiveCraftItemSdesc = "an in-progress economy paper sweet for domestic provisioning craft",
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
			new CatalogueCommodityRecipeInput(Grams: 945.0, Material: "paper", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("945 grams of paper", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_food_drink_packaged_0142_economy_sweet", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x an economy paper sweet for domestic provisioning (@industrialised_food_drink_packaged_0142_economy_sweet)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodDrinkAndPackagedConsumablesMakeEconomyCottonCrackerForDomesticProvisioningCraftIndustrialisedFoodDrinkPackaged0143EconomyCracker() => new()
	{
		Metadata = new("Food, drink and packaged consumables/make economy cotton cracker for domestic provisioning [craft_industrialised_food_drink_packaged_0143_economy_cracker]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_food_drink_packaged_0143_economy_cracker"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make economy cotton cracker for domestic provisioning [craft_industrialised_food_drink_packaged_0143_economy_cracker]",
		Category = "Food, drink and packaged consumables",
		Blurb = "Make an economy cotton cracker for domestic provisioning from prepared cotton stock.",
		Action = "making economy cotton cracker for domestic provisioning",
		ActiveCraftItemSdesc = "an in-progress economy cotton cracker for domestic provisioning craft",
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
			new CatalogueCommodityRecipeInput(Grams: 986.0, Material: "cotton", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("986 grams of cotton", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_food_drink_packaged_0143_economy_cracker", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x an economy cotton cracker for domestic provisioning (@industrialised_food_drink_packaged_0143_economy_cracker)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodDrinkAndPackagedConsumablesMakeEconomyCeramicPacketForDomesticProvisioningCraftIndustrialisedFoodDrinkPackaged0144EconomyPacket() => new()
	{
		Metadata = new("Food, drink and packaged consumables/make economy ceramic packet for domestic provisioning [craft_industrialised_food_drink_packaged_0144_economy_packet]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_food_drink_packaged_0144_economy_packet"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make economy ceramic packet for domestic provisioning [craft_industrialised_food_drink_packaged_0144_economy_packet]",
		Category = "Food, drink and packaged consumables",
		Blurb = "Make an economy ceramic packet for domestic provisioning from prepared ceramic stock.",
		Action = "making economy ceramic packet for domestic provisioning",
		ActiveCraftItemSdesc = "an in-progress economy ceramic packet for domestic provisioning craft",
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
			new CatalogueCommodityRecipeInput(Grams: 1026.0, Material: "ceramic", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1026 grams of ceramic", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_food_drink_packaged_0144_economy_packet", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x an economy ceramic packet for domestic provisioning (@industrialised_food_drink_packaged_0144_economy_packet)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodDrinkAndPackagedConsumablesMakeSomeLinedPaperCrumbsForDomesticProvisioningCraftIndustrialisedFoodDrinkPackaged0145LinedCrumbs() => new()
	{
		Metadata = new("Food, drink and packaged consumables/make some lined paper crumbs for domestic provisioning [craft_industrialised_food_drink_packaged_0145_lined_crumbs]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_food_drink_packaged_0145_lined_crumbs"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make some lined paper crumbs for domestic provisioning [craft_industrialised_food_drink_packaged_0145_lined_crumbs]",
		Category = "Food, drink and packaged consumables",
		Blurb = "Make some lined paper crumbs for domestic provisioning from prepared paper stock.",
		Action = "making some lined paper crumbs for domestic provisioning",
		ActiveCraftItemSdesc = "an in-progress some lined paper crumbs for domestic provisioning craft",
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
			new CatalogueCommodityRecipeInput(Grams: 1067.0, Material: "paper", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1067 grams of paper", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_food_drink_packaged_0145_lined_crumbs", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x some lined paper crumbs for domestic provisioning (@industrialised_food_drink_packaged_0145_lined_crumbs)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodDrinkAndPackagedConsumablesMakeLinedCottonBiscuitForDomesticProvisioningCraftIndustrialisedFoodDrinkPackaged0146LinedBiscuit() => new()
	{
		Metadata = new("Food, drink and packaged consumables/make lined cotton biscuit for domestic provisioning [craft_industrialised_food_drink_packaged_0146_lined_biscuit]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_food_drink_packaged_0146_lined_biscuit"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make lined cotton biscuit for domestic provisioning [craft_industrialised_food_drink_packaged_0146_lined_biscuit]",
		Category = "Food, drink and packaged consumables",
		Blurb = "Make a lined cotton biscuit for domestic provisioning from prepared cotton stock.",
		Action = "making lined cotton biscuit for domestic provisioning",
		ActiveCraftItemSdesc = "an in-progress lined cotton biscuit for domestic provisioning craft",
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
			new CatalogueCommodityRecipeInput(Grams: 1108.0, Material: "cotton", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1108 grams of cotton", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_food_drink_packaged_0146_lined_biscuit", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a lined cotton biscuit for domestic provisioning (@industrialised_food_drink_packaged_0146_lined_biscuit)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodDrinkAndPackagedConsumablesMakeLinedCeramicRationForDomesticProvisioningCraftIndustrialisedFoodDrinkPackaged0147LinedRation() => new()
	{
		Metadata = new("Food, drink and packaged consumables/make lined ceramic ration for domestic provisioning [craft_industrialised_food_drink_packaged_0147_lined_ration]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_food_drink_packaged_0147_lined_ration"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make lined ceramic ration for domestic provisioning [craft_industrialised_food_drink_packaged_0147_lined_ration]",
		Category = "Food, drink and packaged consumables",
		Blurb = "Make a lined ceramic ration for domestic provisioning from prepared ceramic stock.",
		Action = "making lined ceramic ration for domestic provisioning",
		ActiveCraftItemSdesc = "an in-progress lined ceramic ration for domestic provisioning craft",
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
			new CatalogueCommodityRecipeInput(Grams: 1148.0, Material: "ceramic", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1148 grams of ceramic", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_food_drink_packaged_0147_lined_ration", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a lined ceramic ration for domestic provisioning (@industrialised_food_drink_packaged_0147_lined_ration)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodDrinkAndPackagedConsumablesMakeLinedPaperPreserveForDomesticProvisioningCraftIndustrialisedFoodDrinkPackaged0148LinedPreserve() => new()
	{
		Metadata = new("Food, drink and packaged consumables/make lined paper preserve for domestic provisioning [craft_industrialised_food_drink_packaged_0148_lined_preserve]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_food_drink_packaged_0148_lined_preserve"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make lined paper preserve for domestic provisioning [craft_industrialised_food_drink_packaged_0148_lined_preserve]",
		Category = "Food, drink and packaged consumables",
		Blurb = "Make a lined paper preserve for domestic provisioning from prepared paper stock.",
		Action = "making lined paper preserve for domestic provisioning",
		ActiveCraftItemSdesc = "an in-progress lined paper preserve for domestic provisioning craft",
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
			new CatalogueCommodityRecipeInput(Grams: 1189.0, Material: "paper", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1189 grams of paper", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_food_drink_packaged_0148_lined_preserve", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a lined paper preserve for domestic provisioning (@industrialised_food_drink_packaged_0148_lined_preserve)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodDrinkAndPackagedConsumablesMakeLinedCottonLoafForDomesticProvisioningCraftIndustrialisedFoodDrinkPackaged0149LinedLoaf() => new()
	{
		Metadata = new("Food, drink and packaged consumables/make lined cotton loaf for domestic provisioning [craft_industrialised_food_drink_packaged_0149_lined_loaf]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_food_drink_packaged_0149_lined_loaf"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make lined cotton loaf for domestic provisioning [craft_industrialised_food_drink_packaged_0149_lined_loaf]",
		Category = "Food, drink and packaged consumables",
		Blurb = "Make a lined cotton loaf for domestic provisioning from prepared cotton stock.",
		Action = "making lined cotton loaf for domestic provisioning",
		ActiveCraftItemSdesc = "an in-progress lined cotton loaf for domestic provisioning craft",
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
			new CatalogueCommodityRecipeInput(Grams: 1230.0, Material: "cotton", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1230 grams of cotton", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_food_drink_packaged_0149_lined_loaf", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a lined cotton loaf for domestic provisioning (@industrialised_food_drink_packaged_0149_lined_loaf)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodDrinkAndPackagedConsumablesMakeLinedCeramicCakeForDomesticProvisioningCraftIndustrialisedFoodDrinkPackaged0150LinedCake() => new()
	{
		Metadata = new("Food, drink and packaged consumables/make lined ceramic cake for domestic provisioning [craft_industrialised_food_drink_packaged_0150_lined_cake]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_food_drink_packaged_0150_lined_cake"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make lined ceramic cake for domestic provisioning [craft_industrialised_food_drink_packaged_0150_lined_cake]",
		Category = "Food, drink and packaged consumables",
		Blurb = "Make a lined ceramic cake for domestic provisioning from prepared ceramic stock.",
		Action = "making lined ceramic cake for domestic provisioning",
		ActiveCraftItemSdesc = "an in-progress lined ceramic cake for domestic provisioning craft",
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
			new CatalogueCommodityRecipeInput(Grams: 1270.0, Material: "ceramic", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1270 grams of ceramic", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_food_drink_packaged_0150_lined_cake", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a lined ceramic cake for domestic provisioning (@industrialised_food_drink_packaged_0150_lined_cake)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodDrinkAndPackagedConsumablesMakeLinedPaperPuddingForDomesticProvisioningCraftIndustrialisedFoodDrinkPackaged0151LinedPudding() => new()
	{
		Metadata = new("Food, drink and packaged consumables/make lined paper pudding for domestic provisioning [craft_industrialised_food_drink_packaged_0151_lined_pudding]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_food_drink_packaged_0151_lined_pudding"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make lined paper pudding for domestic provisioning [craft_industrialised_food_drink_packaged_0151_lined_pudding]",
		Category = "Food, drink and packaged consumables",
		Blurb = "Make a lined paper pudding for domestic provisioning from prepared paper stock.",
		Action = "making lined paper pudding for domestic provisioning",
		ActiveCraftItemSdesc = "an in-progress lined paper pudding for domestic provisioning craft",
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
			new CatalogueCommodityRecipeInput(Grams: 1311.0, Material: "paper", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1311 grams of paper", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_food_drink_packaged_0151_lined_pudding", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a lined paper pudding for domestic provisioning (@industrialised_food_drink_packaged_0151_lined_pudding)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodDrinkAndPackagedConsumablesMakeLinedCottonSausageForDomesticProvisioningCraftIndustrialisedFoodDrinkPackaged0152LinedSausage() => new()
	{
		Metadata = new("Food, drink and packaged consumables/make lined cotton sausage for domestic provisioning [craft_industrialised_food_drink_packaged_0152_lined_sausage]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_food_drink_packaged_0152_lined_sausage"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make lined cotton sausage for domestic provisioning [craft_industrialised_food_drink_packaged_0152_lined_sausage]",
		Category = "Food, drink and packaged consumables",
		Blurb = "Make a lined cotton sausage for domestic provisioning from prepared cotton stock.",
		Action = "making lined cotton sausage for domestic provisioning",
		ActiveCraftItemSdesc = "an in-progress lined cotton sausage for domestic provisioning craft",
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
			new CatalogueCommodityRecipeInput(Grams: 1352.0, Material: "cotton", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1352 grams of cotton", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_food_drink_packaged_0152_lined_sausage", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a lined cotton sausage for domestic provisioning (@industrialised_food_drink_packaged_0152_lined_sausage)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodDrinkAndPackagedConsumablesMakeLinedCeramicPickleForDomesticProvisioningCraftIndustrialisedFoodDrinkPackaged0153LinedPickle() => new()
	{
		Metadata = new("Food, drink and packaged consumables/make lined ceramic pickle for domestic provisioning [craft_industrialised_food_drink_packaged_0153_lined_pickle]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_food_drink_packaged_0153_lined_pickle"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make lined ceramic pickle for domestic provisioning [craft_industrialised_food_drink_packaged_0153_lined_pickle]",
		Category = "Food, drink and packaged consumables",
		Blurb = "Make a lined ceramic pickle for domestic provisioning from prepared ceramic stock.",
		Action = "making lined ceramic pickle for domestic provisioning",
		ActiveCraftItemSdesc = "an in-progress lined ceramic pickle for domestic provisioning craft",
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
			new CatalogueCommodityRecipeInput(Grams: 1393.0, Material: "ceramic", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1393 grams of ceramic", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_food_drink_packaged_0153_lined_pickle", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a lined ceramic pickle for domestic provisioning (@industrialised_food_drink_packaged_0153_lined_pickle)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodDrinkAndPackagedConsumablesMakeLinedPaperSweetForDomesticProvisioningCraftIndustrialisedFoodDrinkPackaged0154LinedSweet() => new()
	{
		Metadata = new("Food, drink and packaged consumables/make lined paper sweet for domestic provisioning [craft_industrialised_food_drink_packaged_0154_lined_sweet]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_food_drink_packaged_0154_lined_sweet"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make lined paper sweet for domestic provisioning [craft_industrialised_food_drink_packaged_0154_lined_sweet]",
		Category = "Food, drink and packaged consumables",
		Blurb = "Make a lined paper sweet for domestic provisioning from prepared paper stock.",
		Action = "making lined paper sweet for domestic provisioning",
		ActiveCraftItemSdesc = "an in-progress lined paper sweet for domestic provisioning craft",
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
			new CatalogueCommodityRecipeInput(Grams: 1433.0, Material: "paper", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1433 grams of paper", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_food_drink_packaged_0154_lined_sweet", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a lined paper sweet for domestic provisioning (@industrialised_food_drink_packaged_0154_lined_sweet)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodDrinkAndPackagedConsumablesMakeLinedCottonCrackerForDomesticProvisioningCraftIndustrialisedFoodDrinkPackaged0155LinedCracker() => new()
	{
		Metadata = new("Food, drink and packaged consumables/make lined cotton cracker for domestic provisioning [craft_industrialised_food_drink_packaged_0155_lined_cracker]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_food_drink_packaged_0155_lined_cracker"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make lined cotton cracker for domestic provisioning [craft_industrialised_food_drink_packaged_0155_lined_cracker]",
		Category = "Food, drink and packaged consumables",
		Blurb = "Make a lined cotton cracker for domestic provisioning from prepared cotton stock.",
		Action = "making lined cotton cracker for domestic provisioning",
		ActiveCraftItemSdesc = "an in-progress lined cotton cracker for domestic provisioning craft",
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
			new CatalogueCommodityRecipeInput(Grams: 1474.0, Material: "cotton", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1474 grams of cotton", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_food_drink_packaged_0155_lined_cracker", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a lined cotton cracker for domestic provisioning (@industrialised_food_drink_packaged_0155_lined_cracker)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodDrinkAndPackagedConsumablesMakeLinedCeramicPacketForDomesticProvisioningCraftIndustrialisedFoodDrinkPackaged0156LinedPacket() => new()
	{
		Metadata = new("Food, drink and packaged consumables/make lined ceramic packet for domestic provisioning [craft_industrialised_food_drink_packaged_0156_lined_packet]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_food_drink_packaged_0156_lined_packet"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make lined ceramic packet for domestic provisioning [craft_industrialised_food_drink_packaged_0156_lined_packet]",
		Category = "Food, drink and packaged consumables",
		Blurb = "Make a lined ceramic packet for domestic provisioning from prepared ceramic stock.",
		Action = "making lined ceramic packet for domestic provisioning",
		ActiveCraftItemSdesc = "an in-progress lined ceramic packet for domestic provisioning craft",
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
			new CatalogueCommodityRecipeInput(Grams: 1515.0, Material: "ceramic", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1515 grams of ceramic", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_food_drink_packaged_0156_lined_packet", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a lined ceramic packet for domestic provisioning (@industrialised_food_drink_packaged_0156_lined_packet)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodDrinkAndPackagedConsumablesMakeSomeVentilatedPaperCrumbsForDomesticProvisioningCraftIndustrialisedFoodDrinkPackaged0157VentilatedCrumbs() => new()
	{
		Metadata = new("Food, drink and packaged consumables/make some ventilated paper crumbs for domestic provisioning [craft_industrialised_food_drink_packaged_0157_ventilated_crumbs]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_food_drink_packaged_0157_ventilated_crumbs"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make some ventilated paper crumbs for domestic provisioning [craft_industrialised_food_drink_packaged_0157_ventilated_crumbs]",
		Category = "Food, drink and packaged consumables",
		Blurb = "Make some ventilated paper crumbs for domestic provisioning from prepared paper stock.",
		Action = "making some ventilated paper crumbs for domestic provisioning",
		ActiveCraftItemSdesc = "an in-progress some ventilated paper crumbs for domestic provisioning craft",
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
			new CatalogueCommodityRecipeInput(Grams: 1555.0, Material: "paper", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1555 grams of paper", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_food_drink_packaged_0157_ventilated_crumbs", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x some ventilated paper crumbs for domestic provisioning (@industrialised_food_drink_packaged_0157_ventilated_crumbs)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodDrinkAndPackagedConsumablesMakeVentilatedCottonBiscuitForDomesticProvisioningCraftIndustrialisedFoodDrinkPackaged0158VentilatedBiscuit() => new()
	{
		Metadata = new("Food, drink and packaged consumables/make ventilated cotton biscuit for domestic provisioning [craft_industrialised_food_drink_packaged_0158_ventilated_biscuit]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_food_drink_packaged_0158_ventilated_biscuit"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make ventilated cotton biscuit for domestic provisioning [craft_industrialised_food_drink_packaged_0158_ventilated_biscuit]",
		Category = "Food, drink and packaged consumables",
		Blurb = "Make a ventilated cotton biscuit for domestic provisioning from prepared cotton stock.",
		Action = "making ventilated cotton biscuit for domestic provisioning",
		ActiveCraftItemSdesc = "an in-progress ventilated cotton biscuit for domestic provisioning craft",
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
			new CatalogueCommodityRecipeInput(Grams: 1596.0, Material: "cotton", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1596 grams of cotton", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_food_drink_packaged_0158_ventilated_biscuit", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a ventilated cotton biscuit for domestic provisioning (@industrialised_food_drink_packaged_0158_ventilated_biscuit)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodDrinkAndPackagedConsumablesMakeVentilatedCeramicRationForDomesticProvisioningCraftIndustrialisedFoodDrinkPackaged0159VentilatedRation() => new()
	{
		Metadata = new("Food, drink and packaged consumables/make ventilated ceramic ration for domestic provisioning [craft_industrialised_food_drink_packaged_0159_ventilated_ration]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_food_drink_packaged_0159_ventilated_ration"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make ventilated ceramic ration for domestic provisioning [craft_industrialised_food_drink_packaged_0159_ventilated_ration]",
		Category = "Food, drink and packaged consumables",
		Blurb = "Make a ventilated ceramic ration for domestic provisioning from prepared ceramic stock.",
		Action = "making ventilated ceramic ration for domestic provisioning",
		ActiveCraftItemSdesc = "an in-progress ventilated ceramic ration for domestic provisioning craft",
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
			new CatalogueCommodityRecipeInput(Grams: 1637.0, Material: "ceramic", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1637 grams of ceramic", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_food_drink_packaged_0159_ventilated_ration", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a ventilated ceramic ration for domestic provisioning (@industrialised_food_drink_packaged_0159_ventilated_ration)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodDrinkAndPackagedConsumablesMakeVentilatedPaperPreserveForDomesticProvisioningCraftIndustrialisedFoodDrinkPackaged0160VentilatedPreserve() => new()
	{
		Metadata = new("Food, drink and packaged consumables/make ventilated paper preserve for domestic provisioning [craft_industrialised_food_drink_packaged_0160_ventilated_preserve]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_food_drink_packaged_0160_ventilated_preserve"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make ventilated paper preserve for domestic provisioning [craft_industrialised_food_drink_packaged_0160_ventilated_preserve]",
		Category = "Food, drink and packaged consumables",
		Blurb = "Make a ventilated paper preserve for domestic provisioning from prepared paper stock.",
		Action = "making ventilated paper preserve for domestic provisioning",
		ActiveCraftItemSdesc = "an in-progress ventilated paper preserve for domestic provisioning craft",
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
			new CatalogueCommodityRecipeInput(Grams: 1678.0, Material: "paper", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1678 grams of paper", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_food_drink_packaged_0160_ventilated_preserve", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a ventilated paper preserve for domestic provisioning (@industrialised_food_drink_packaged_0160_ventilated_preserve)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodDrinkAndPackagedConsumablesMakeVentilatedCottonLoafForDomesticProvisioningCraftIndustrialisedFoodDrinkPackaged0161VentilatedLoaf() => new()
	{
		Metadata = new("Food, drink and packaged consumables/make ventilated cotton loaf for domestic provisioning [craft_industrialised_food_drink_packaged_0161_ventilated_loaf]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_food_drink_packaged_0161_ventilated_loaf"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make ventilated cotton loaf for domestic provisioning [craft_industrialised_food_drink_packaged_0161_ventilated_loaf]",
		Category = "Food, drink and packaged consumables",
		Blurb = "Make a ventilated cotton loaf for domestic provisioning from prepared cotton stock.",
		Action = "making ventilated cotton loaf for domestic provisioning",
		ActiveCraftItemSdesc = "an in-progress ventilated cotton loaf for domestic provisioning craft",
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
			new CatalogueCommodityRecipeInput(Grams: 1718.0, Material: "cotton", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1718 grams of cotton", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_food_drink_packaged_0161_ventilated_loaf", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a ventilated cotton loaf for domestic provisioning (@industrialised_food_drink_packaged_0161_ventilated_loaf)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodDrinkAndPackagedConsumablesMakeVentilatedCeramicCakeForDomesticProvisioningCraftIndustrialisedFoodDrinkPackaged0162VentilatedCake() => new()
	{
		Metadata = new("Food, drink and packaged consumables/make ventilated ceramic cake for domestic provisioning [craft_industrialised_food_drink_packaged_0162_ventilated_cake]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_food_drink_packaged_0162_ventilated_cake"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make ventilated ceramic cake for domestic provisioning [craft_industrialised_food_drink_packaged_0162_ventilated_cake]",
		Category = "Food, drink and packaged consumables",
		Blurb = "Make a ventilated ceramic cake for domestic provisioning from prepared ceramic stock.",
		Action = "making ventilated ceramic cake for domestic provisioning",
		ActiveCraftItemSdesc = "an in-progress ventilated ceramic cake for domestic provisioning craft",
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
			new CatalogueCommodityRecipeInput(Grams: 1759.0, Material: "ceramic", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1759 grams of ceramic", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_food_drink_packaged_0162_ventilated_cake", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a ventilated ceramic cake for domestic provisioning (@industrialised_food_drink_packaged_0162_ventilated_cake)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodDrinkAndPackagedConsumablesMakeVentilatedPaperPuddingForDomesticProvisioningCraftIndustrialisedFoodDrinkPackaged0163VentilatedPudding() => new()
	{
		Metadata = new("Food, drink and packaged consumables/make ventilated paper pudding for domestic provisioning [craft_industrialised_food_drink_packaged_0163_ventilated_pudding]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_food_drink_packaged_0163_ventilated_pudding"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make ventilated paper pudding for domestic provisioning [craft_industrialised_food_drink_packaged_0163_ventilated_pudding]",
		Category = "Food, drink and packaged consumables",
		Blurb = "Make a ventilated paper pudding for domestic provisioning from prepared paper stock.",
		Action = "making ventilated paper pudding for domestic provisioning",
		ActiveCraftItemSdesc = "an in-progress ventilated paper pudding for domestic provisioning craft",
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
			new CatalogueCommodityRecipeInput(Grams: 1800.0, Material: "paper", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1800 grams of paper", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_food_drink_packaged_0163_ventilated_pudding", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a ventilated paper pudding for domestic provisioning (@industrialised_food_drink_packaged_0163_ventilated_pudding)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodDrinkAndPackagedConsumablesMakeVentilatedCottonSausageForDomesticProvisioningCraftIndustrialisedFoodDrinkPackaged0164VentilatedSausage() => new()
	{
		Metadata = new("Food, drink and packaged consumables/make ventilated cotton sausage for domestic provisioning [craft_industrialised_food_drink_packaged_0164_ventilated_sausage]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_food_drink_packaged_0164_ventilated_sausage"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make ventilated cotton sausage for domestic provisioning [craft_industrialised_food_drink_packaged_0164_ventilated_sausage]",
		Category = "Food, drink and packaged consumables",
		Blurb = "Make a ventilated cotton sausage for domestic provisioning from prepared cotton stock.",
		Action = "making ventilated cotton sausage for domestic provisioning",
		ActiveCraftItemSdesc = "an in-progress ventilated cotton sausage for domestic provisioning craft",
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
			new CatalogueCommodityRecipeInput(Grams: 1840.0, Material: "cotton", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1840 grams of cotton", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_food_drink_packaged_0164_ventilated_sausage", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a ventilated cotton sausage for domestic provisioning (@industrialised_food_drink_packaged_0164_ventilated_sausage)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodDrinkAndPackagedConsumablesMakeVentilatedCeramicPickleForDomesticProvisioningCraftIndustrialisedFoodDrinkPackaged0165VentilatedPickle() => new()
	{
		Metadata = new("Food, drink and packaged consumables/make ventilated ceramic pickle for domestic provisioning [craft_industrialised_food_drink_packaged_0165_ventilated_pickle]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_food_drink_packaged_0165_ventilated_pickle"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make ventilated ceramic pickle for domestic provisioning [craft_industrialised_food_drink_packaged_0165_ventilated_pickle]",
		Category = "Food, drink and packaged consumables",
		Blurb = "Make a ventilated ceramic pickle for domestic provisioning from prepared ceramic stock.",
		Action = "making ventilated ceramic pickle for domestic provisioning",
		ActiveCraftItemSdesc = "an in-progress ventilated ceramic pickle for domestic provisioning craft",
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
			new CatalogueCommodityRecipeInput(Grams: 1881.0, Material: "ceramic", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1881 grams of ceramic", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_food_drink_packaged_0165_ventilated_pickle", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a ventilated ceramic pickle for domestic provisioning (@industrialised_food_drink_packaged_0165_ventilated_pickle)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};
}
