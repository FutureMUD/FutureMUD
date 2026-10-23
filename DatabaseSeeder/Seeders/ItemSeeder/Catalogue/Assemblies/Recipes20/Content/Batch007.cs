#nullable enable

using MudSharp.Body;
using MudSharp.GameItems.Inventory.Plans;
using MudSharp.RPG.Checks;

namespace DatabaseSeeder.Seeders;

public static partial class Recipes20Stock
{
	public static CatalogueRecipeDefinition FoodDrinkAndPackagedConsumablesMakeHeavyDutyPaperSweetForDomesticProvisioningCraftIndustrialisedFoodDrinkPackaged0046HeavyDutySweet() => new()
	{
		Metadata = new("Food, drink and packaged consumables/make heavy-duty paper sweet for domestic provisioning [craft_industrialised_food_drink_packaged_0046_heavy_duty_sweet]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_food_drink_packaged_0046_heavy_duty_sweet"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make heavy-duty paper sweet for domestic provisioning [craft_industrialised_food_drink_packaged_0046_heavy_duty_sweet]",
		Category = "Food, drink and packaged consumables",
		Blurb = "Make a heavy-duty paper sweet for domestic provisioning from prepared paper stock.",
		Action = "making heavy-duty paper sweet for domestic provisioning",
		ActiveCraftItemSdesc = "an in-progress heavy-duty paper sweet for domestic provisioning craft",
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
			new CatalogueCommodityRecipeInput(Grams: 2483.0, Material: "paper", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2483 grams of paper", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_food_drink_packaged_0046_heavy_duty_sweet", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a heavy-duty paper sweet for domestic provisioning (@industrialised_food_drink_packaged_0046_heavy_duty_sweet)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodDrinkAndPackagedConsumablesMakeHeavyDutyCottonCrackerForDomesticProvisioningCraftIndustrialisedFoodDrinkPackaged0047HeavyDutyCracker() => new()
	{
		Metadata = new("Food, drink and packaged consumables/make heavy-duty cotton cracker for domestic provisioning [craft_industrialised_food_drink_packaged_0047_heavy_duty_cracker]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_food_drink_packaged_0047_heavy_duty_cracker"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make heavy-duty cotton cracker for domestic provisioning [craft_industrialised_food_drink_packaged_0047_heavy_duty_cracker]",
		Category = "Food, drink and packaged consumables",
		Blurb = "Make a heavy-duty cotton cracker for domestic provisioning from prepared cotton stock.",
		Action = "making heavy-duty cotton cracker for domestic provisioning",
		ActiveCraftItemSdesc = "an in-progress heavy-duty cotton cracker for domestic provisioning craft",
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
			new CatalogueCommodityRecipeInput(Grams: 2523.0, Material: "cotton", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2523 grams of cotton", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_food_drink_packaged_0047_heavy_duty_cracker", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a heavy-duty cotton cracker for domestic provisioning (@industrialised_food_drink_packaged_0047_heavy_duty_cracker)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodDrinkAndPackagedConsumablesMakeHeavyDutyCeramicPacketForDomesticProvisioningCraftIndustrialisedFoodDrinkPackaged0048HeavyDutyPacket() => new()
	{
		Metadata = new("Food, drink and packaged consumables/make heavy-duty ceramic packet for domestic provisioning [craft_industrialised_food_drink_packaged_0048_heavy_duty_packet]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_food_drink_packaged_0048_heavy_duty_packet"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make heavy-duty ceramic packet for domestic provisioning [craft_industrialised_food_drink_packaged_0048_heavy_duty_packet]",
		Category = "Food, drink and packaged consumables",
		Blurb = "Make a heavy-duty ceramic packet for domestic provisioning from prepared ceramic stock.",
		Action = "making heavy-duty ceramic packet for domestic provisioning",
		ActiveCraftItemSdesc = "an in-progress heavy-duty ceramic packet for domestic provisioning craft",
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
			new CatalogueCommodityRecipeInput(Grams: 2564.0, Material: "ceramic", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2564 grams of ceramic", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_food_drink_packaged_0048_heavy_duty_packet", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a heavy-duty ceramic packet for domestic provisioning (@industrialised_food_drink_packaged_0048_heavy_duty_packet)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodDrinkAndPackagedConsumablesMakeSomeFoldingPaperCrumbsForDomesticProvisioningCraftIndustrialisedFoodDrinkPackaged0049FoldingCrumbs() => new()
	{
		Metadata = new("Food, drink and packaged consumables/make some folding paper crumbs for domestic provisioning [craft_industrialised_food_drink_packaged_0049_folding_crumbs]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_food_drink_packaged_0049_folding_crumbs"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make some folding paper crumbs for domestic provisioning [craft_industrialised_food_drink_packaged_0049_folding_crumbs]",
		Category = "Food, drink and packaged consumables",
		Blurb = "Make some folding paper crumbs for domestic provisioning from prepared paper stock.",
		Action = "making some folding paper crumbs for domestic provisioning",
		ActiveCraftItemSdesc = "an in-progress some folding paper crumbs for domestic provisioning craft",
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
			new CatalogueCommodityRecipeInput(Grams: 2605.0, Material: "paper", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2605 grams of paper", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_food_drink_packaged_0049_folding_crumbs", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x some folding paper crumbs for domestic provisioning (@industrialised_food_drink_packaged_0049_folding_crumbs)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodDrinkAndPackagedConsumablesMakeFoldingCottonBiscuitForDomesticProvisioningCraftIndustrialisedFoodDrinkPackaged0050FoldingBiscuit() => new()
	{
		Metadata = new("Food, drink and packaged consumables/make folding cotton biscuit for domestic provisioning [craft_industrialised_food_drink_packaged_0050_folding_biscuit]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_food_drink_packaged_0050_folding_biscuit"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make folding cotton biscuit for domestic provisioning [craft_industrialised_food_drink_packaged_0050_folding_biscuit]",
		Category = "Food, drink and packaged consumables",
		Blurb = "Make a folding cotton biscuit for domestic provisioning from prepared cotton stock.",
		Action = "making folding cotton biscuit for domestic provisioning",
		ActiveCraftItemSdesc = "an in-progress folding cotton biscuit for domestic provisioning craft",
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
			new CatalogueCommodityRecipeInput(Grams: 2646.0, Material: "cotton", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2646 grams of cotton", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_food_drink_packaged_0050_folding_biscuit", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a folding cotton biscuit for domestic provisioning (@industrialised_food_drink_packaged_0050_folding_biscuit)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodDrinkAndPackagedConsumablesMakeFoldingCeramicRationForDomesticProvisioningCraftIndustrialisedFoodDrinkPackaged0051FoldingRation() => new()
	{
		Metadata = new("Food, drink and packaged consumables/make folding ceramic ration for domestic provisioning [craft_industrialised_food_drink_packaged_0051_folding_ration]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_food_drink_packaged_0051_folding_ration"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make folding ceramic ration for domestic provisioning [craft_industrialised_food_drink_packaged_0051_folding_ration]",
		Category = "Food, drink and packaged consumables",
		Blurb = "Make a folding ceramic ration for domestic provisioning from prepared ceramic stock.",
		Action = "making folding ceramic ration for domestic provisioning",
		ActiveCraftItemSdesc = "an in-progress folding ceramic ration for domestic provisioning craft",
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
			new CatalogueCommodityRecipeInput(Grams: 2686.0, Material: "ceramic", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2686 grams of ceramic", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_food_drink_packaged_0051_folding_ration", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a folding ceramic ration for domestic provisioning (@industrialised_food_drink_packaged_0051_folding_ration)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodDrinkAndPackagedConsumablesMakeFoldingPaperPreserveForDomesticProvisioningCraftIndustrialisedFoodDrinkPackaged0052FoldingPreserve() => new()
	{
		Metadata = new("Food, drink and packaged consumables/make folding paper preserve for domestic provisioning [craft_industrialised_food_drink_packaged_0052_folding_preserve]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_food_drink_packaged_0052_folding_preserve"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make folding paper preserve for domestic provisioning [craft_industrialised_food_drink_packaged_0052_folding_preserve]",
		Category = "Food, drink and packaged consumables",
		Blurb = "Make a folding paper preserve for domestic provisioning from prepared paper stock.",
		Action = "making folding paper preserve for domestic provisioning",
		ActiveCraftItemSdesc = "an in-progress folding paper preserve for domestic provisioning craft",
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
			new CatalogueCommodityRecipeInput(Grams: 2727.0, Material: "paper", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2727 grams of paper", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_food_drink_packaged_0052_folding_preserve", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a folding paper preserve for domestic provisioning (@industrialised_food_drink_packaged_0052_folding_preserve)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodDrinkAndPackagedConsumablesMakeFoldingCottonLoafForDomesticProvisioningCraftIndustrialisedFoodDrinkPackaged0053FoldingLoaf() => new()
	{
		Metadata = new("Food, drink and packaged consumables/make folding cotton loaf for domestic provisioning [craft_industrialised_food_drink_packaged_0053_folding_loaf]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_food_drink_packaged_0053_folding_loaf"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make folding cotton loaf for domestic provisioning [craft_industrialised_food_drink_packaged_0053_folding_loaf]",
		Category = "Food, drink and packaged consumables",
		Blurb = "Make a folding cotton loaf for domestic provisioning from prepared cotton stock.",
		Action = "making folding cotton loaf for domestic provisioning",
		ActiveCraftItemSdesc = "an in-progress folding cotton loaf for domestic provisioning craft",
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
			new CatalogueCommodityRecipeInput(Grams: 2768.0, Material: "cotton", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2768 grams of cotton", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_food_drink_packaged_0053_folding_loaf", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a folding cotton loaf for domestic provisioning (@industrialised_food_drink_packaged_0053_folding_loaf)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodDrinkAndPackagedConsumablesMakeFoldingCeramicCakeForDomesticProvisioningCraftIndustrialisedFoodDrinkPackaged0054FoldingCake() => new()
	{
		Metadata = new("Food, drink and packaged consumables/make folding ceramic cake for domestic provisioning [craft_industrialised_food_drink_packaged_0054_folding_cake]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_food_drink_packaged_0054_folding_cake"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make folding ceramic cake for domestic provisioning [craft_industrialised_food_drink_packaged_0054_folding_cake]",
		Category = "Food, drink and packaged consumables",
		Blurb = "Make a folding ceramic cake for domestic provisioning from prepared ceramic stock.",
		Action = "making folding ceramic cake for domestic provisioning",
		ActiveCraftItemSdesc = "an in-progress folding ceramic cake for domestic provisioning craft",
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
			new CatalogueCommodityRecipeInput(Grams: 2808.0, Material: "ceramic", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2808 grams of ceramic", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_food_drink_packaged_0054_folding_cake", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a folding ceramic cake for domestic provisioning (@industrialised_food_drink_packaged_0054_folding_cake)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodDrinkAndPackagedConsumablesMakeFoldingPaperPuddingForDomesticProvisioningCraftIndustrialisedFoodDrinkPackaged0055FoldingPudding() => new()
	{
		Metadata = new("Food, drink and packaged consumables/make folding paper pudding for domestic provisioning [craft_industrialised_food_drink_packaged_0055_folding_pudding]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_food_drink_packaged_0055_folding_pudding"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make folding paper pudding for domestic provisioning [craft_industrialised_food_drink_packaged_0055_folding_pudding]",
		Category = "Food, drink and packaged consumables",
		Blurb = "Make a folding paper pudding for domestic provisioning from prepared paper stock.",
		Action = "making folding paper pudding for domestic provisioning",
		ActiveCraftItemSdesc = "an in-progress folding paper pudding for domestic provisioning craft",
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
			new CatalogueCommodityRecipeInput(Grams: 2849.0, Material: "paper", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2849 grams of paper", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_food_drink_packaged_0055_folding_pudding", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a folding paper pudding for domestic provisioning (@industrialised_food_drink_packaged_0055_folding_pudding)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodDrinkAndPackagedConsumablesMakeFoldingCottonSausageForDomesticProvisioningCraftIndustrialisedFoodDrinkPackaged0056FoldingSausage() => new()
	{
		Metadata = new("Food, drink and packaged consumables/make folding cotton sausage for domestic provisioning [craft_industrialised_food_drink_packaged_0056_folding_sausage]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_food_drink_packaged_0056_folding_sausage"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make folding cotton sausage for domestic provisioning [craft_industrialised_food_drink_packaged_0056_folding_sausage]",
		Category = "Food, drink and packaged consumables",
		Blurb = "Make a folding cotton sausage for domestic provisioning from prepared cotton stock.",
		Action = "making folding cotton sausage for domestic provisioning",
		ActiveCraftItemSdesc = "an in-progress folding cotton sausage for domestic provisioning craft",
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
			new CatalogueCommodityRecipeInput(Grams: 2890.0, Material: "cotton", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2890 grams of cotton", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_food_drink_packaged_0056_folding_sausage", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a folding cotton sausage for domestic provisioning (@industrialised_food_drink_packaged_0056_folding_sausage)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodDrinkAndPackagedConsumablesMakeFoldingCeramicPickleForDomesticProvisioningCraftIndustrialisedFoodDrinkPackaged0057FoldingPickle() => new()
	{
		Metadata = new("Food, drink and packaged consumables/make folding ceramic pickle for domestic provisioning [craft_industrialised_food_drink_packaged_0057_folding_pickle]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_food_drink_packaged_0057_folding_pickle"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make folding ceramic pickle for domestic provisioning [craft_industrialised_food_drink_packaged_0057_folding_pickle]",
		Category = "Food, drink and packaged consumables",
		Blurb = "Make a folding ceramic pickle for domestic provisioning from prepared ceramic stock.",
		Action = "making folding ceramic pickle for domestic provisioning",
		ActiveCraftItemSdesc = "an in-progress folding ceramic pickle for domestic provisioning craft",
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
			new CatalogueCommodityRecipeInput(Grams: 2930.0, Material: "ceramic", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2930 grams of ceramic", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_food_drink_packaged_0057_folding_pickle", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a folding ceramic pickle for domestic provisioning (@industrialised_food_drink_packaged_0057_folding_pickle)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodDrinkAndPackagedConsumablesMakeFoldingPaperSweetForDomesticProvisioningCraftIndustrialisedFoodDrinkPackaged0058FoldingSweet() => new()
	{
		Metadata = new("Food, drink and packaged consumables/make folding paper sweet for domestic provisioning [craft_industrialised_food_drink_packaged_0058_folding_sweet]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_food_drink_packaged_0058_folding_sweet"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make folding paper sweet for domestic provisioning [craft_industrialised_food_drink_packaged_0058_folding_sweet]",
		Category = "Food, drink and packaged consumables",
		Blurb = "Make a folding paper sweet for domestic provisioning from prepared paper stock.",
		Action = "making folding paper sweet for domestic provisioning",
		ActiveCraftItemSdesc = "an in-progress folding paper sweet for domestic provisioning craft",
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
			new CatalogueCommodityRecipeInput(Grams: 2971.0, Material: "paper", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2971 grams of paper", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_food_drink_packaged_0058_folding_sweet", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a folding paper sweet for domestic provisioning (@industrialised_food_drink_packaged_0058_folding_sweet)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodDrinkAndPackagedConsumablesMakeFoldingCottonCrackerForDomesticProvisioningCraftIndustrialisedFoodDrinkPackaged0059FoldingCracker() => new()
	{
		Metadata = new("Food, drink and packaged consumables/make folding cotton cracker for domestic provisioning [craft_industrialised_food_drink_packaged_0059_folding_cracker]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_food_drink_packaged_0059_folding_cracker"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make folding cotton cracker for domestic provisioning [craft_industrialised_food_drink_packaged_0059_folding_cracker]",
		Category = "Food, drink and packaged consumables",
		Blurb = "Make a folding cotton cracker for domestic provisioning from prepared cotton stock.",
		Action = "making folding cotton cracker for domestic provisioning",
		ActiveCraftItemSdesc = "an in-progress folding cotton cracker for domestic provisioning craft",
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
			new CatalogueCommodityRecipeInput(Grams: 3012.0, Material: "cotton", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3012 grams of cotton", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_food_drink_packaged_0059_folding_cracker", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a folding cotton cracker for domestic provisioning (@industrialised_food_drink_packaged_0059_folding_cracker)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodDrinkAndPackagedConsumablesMakeFoldingCeramicPacketForDomesticProvisioningCraftIndustrialisedFoodDrinkPackaged0060FoldingPacket() => new()
	{
		Metadata = new("Food, drink and packaged consumables/make folding ceramic packet for domestic provisioning [craft_industrialised_food_drink_packaged_0060_folding_packet]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_food_drink_packaged_0060_folding_packet"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make folding ceramic packet for domestic provisioning [craft_industrialised_food_drink_packaged_0060_folding_packet]",
		Category = "Food, drink and packaged consumables",
		Blurb = "Make a folding ceramic packet for domestic provisioning from prepared ceramic stock.",
		Action = "making folding ceramic packet for domestic provisioning",
		ActiveCraftItemSdesc = "an in-progress folding ceramic packet for domestic provisioning craft",
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
			new CatalogueCommodityRecipeInput(Grams: 3053.0, Material: "ceramic", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3053 grams of ceramic", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_food_drink_packaged_0060_folding_packet", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a folding ceramic packet for domestic provisioning (@industrialised_food_drink_packaged_0060_folding_packet)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodDrinkAndPackagedConsumablesMakeSomePortablePaperCrumbsForDomesticProvisioningCraftIndustrialisedFoodDrinkPackaged0061PortableCrumbs() => new()
	{
		Metadata = new("Food, drink and packaged consumables/make some portable paper crumbs for domestic provisioning [craft_industrialised_food_drink_packaged_0061_portable_crumbs]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_food_drink_packaged_0061_portable_crumbs"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make some portable paper crumbs for domestic provisioning [craft_industrialised_food_drink_packaged_0061_portable_crumbs]",
		Category = "Food, drink and packaged consumables",
		Blurb = "Make some portable paper crumbs for domestic provisioning from prepared paper stock.",
		Action = "making some portable paper crumbs for domestic provisioning",
		ActiveCraftItemSdesc = "an in-progress some portable paper crumbs for domestic provisioning craft",
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
			new CatalogueCommodityRecipeInput(Grams: 3093.0, Material: "paper", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3093 grams of paper", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_food_drink_packaged_0061_portable_crumbs", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x some portable paper crumbs for domestic provisioning (@industrialised_food_drink_packaged_0061_portable_crumbs)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodDrinkAndPackagedConsumablesMakePortableCottonBiscuitForDomesticProvisioningCraftIndustrialisedFoodDrinkPackaged0062PortableBiscuit() => new()
	{
		Metadata = new("Food, drink and packaged consumables/make portable cotton biscuit for domestic provisioning [craft_industrialised_food_drink_packaged_0062_portable_biscuit]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_food_drink_packaged_0062_portable_biscuit"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make portable cotton biscuit for domestic provisioning [craft_industrialised_food_drink_packaged_0062_portable_biscuit]",
		Category = "Food, drink and packaged consumables",
		Blurb = "Make a portable cotton biscuit for domestic provisioning from prepared cotton stock.",
		Action = "making portable cotton biscuit for domestic provisioning",
		ActiveCraftItemSdesc = "an in-progress portable cotton biscuit for domestic provisioning craft",
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
			new CatalogueCommodityRecipeInput(Grams: 3134.0, Material: "cotton", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3134 grams of cotton", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_food_drink_packaged_0062_portable_biscuit", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a portable cotton biscuit for domestic provisioning (@industrialised_food_drink_packaged_0062_portable_biscuit)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodDrinkAndPackagedConsumablesMakePortableCeramicRationForDomesticProvisioningCraftIndustrialisedFoodDrinkPackaged0063PortableRation() => new()
	{
		Metadata = new("Food, drink and packaged consumables/make portable ceramic ration for domestic provisioning [craft_industrialised_food_drink_packaged_0063_portable_ration]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_food_drink_packaged_0063_portable_ration"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make portable ceramic ration for domestic provisioning [craft_industrialised_food_drink_packaged_0063_portable_ration]",
		Category = "Food, drink and packaged consumables",
		Blurb = "Make a portable ceramic ration for domestic provisioning from prepared ceramic stock.",
		Action = "making portable ceramic ration for domestic provisioning",
		ActiveCraftItemSdesc = "an in-progress portable ceramic ration for domestic provisioning craft",
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
			new CatalogueCommodityRecipeInput(Grams: 3175.0, Material: "ceramic", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3175 grams of ceramic", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_food_drink_packaged_0063_portable_ration", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a portable ceramic ration for domestic provisioning (@industrialised_food_drink_packaged_0063_portable_ration)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodDrinkAndPackagedConsumablesMakePortablePaperPreserveForDomesticProvisioningCraftIndustrialisedFoodDrinkPackaged0064PortablePreserve() => new()
	{
		Metadata = new("Food, drink and packaged consumables/make portable paper preserve for domestic provisioning [craft_industrialised_food_drink_packaged_0064_portable_preserve]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_food_drink_packaged_0064_portable_preserve"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make portable paper preserve for domestic provisioning [craft_industrialised_food_drink_packaged_0064_portable_preserve]",
		Category = "Food, drink and packaged consumables",
		Blurb = "Make a portable paper preserve for domestic provisioning from prepared paper stock.",
		Action = "making portable paper preserve for domestic provisioning",
		ActiveCraftItemSdesc = "an in-progress portable paper preserve for domestic provisioning craft",
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
			new CatalogueCommodityRecipeInput(Grams: 3215.0, Material: "paper", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3215 grams of paper", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_food_drink_packaged_0064_portable_preserve", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a portable paper preserve for domestic provisioning (@industrialised_food_drink_packaged_0064_portable_preserve)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodDrinkAndPackagedConsumablesMakePortableCottonLoafForDomesticProvisioningCraftIndustrialisedFoodDrinkPackaged0065PortableLoaf() => new()
	{
		Metadata = new("Food, drink and packaged consumables/make portable cotton loaf for domestic provisioning [craft_industrialised_food_drink_packaged_0065_portable_loaf]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_food_drink_packaged_0065_portable_loaf"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make portable cotton loaf for domestic provisioning [craft_industrialised_food_drink_packaged_0065_portable_loaf]",
		Category = "Food, drink and packaged consumables",
		Blurb = "Make a portable cotton loaf for domestic provisioning from prepared cotton stock.",
		Action = "making portable cotton loaf for domestic provisioning",
		ActiveCraftItemSdesc = "an in-progress portable cotton loaf for domestic provisioning craft",
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
			new CatalogueCommodityRecipeInput(Grams: 3256.0, Material: "cotton", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3256 grams of cotton", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_food_drink_packaged_0065_portable_loaf", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a portable cotton loaf for domestic provisioning (@industrialised_food_drink_packaged_0065_portable_loaf)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodDrinkAndPackagedConsumablesMakePortableCeramicCakeForDomesticProvisioningCraftIndustrialisedFoodDrinkPackaged0066PortableCake() => new()
	{
		Metadata = new("Food, drink and packaged consumables/make portable ceramic cake for domestic provisioning [craft_industrialised_food_drink_packaged_0066_portable_cake]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_food_drink_packaged_0066_portable_cake"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make portable ceramic cake for domestic provisioning [craft_industrialised_food_drink_packaged_0066_portable_cake]",
		Category = "Food, drink and packaged consumables",
		Blurb = "Make a portable ceramic cake for domestic provisioning from prepared ceramic stock.",
		Action = "making portable ceramic cake for domestic provisioning",
		ActiveCraftItemSdesc = "an in-progress portable ceramic cake for domestic provisioning craft",
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
			new CatalogueCommodityRecipeInput(Grams: 3297.0, Material: "ceramic", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3297 grams of ceramic", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_food_drink_packaged_0066_portable_cake", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a portable ceramic cake for domestic provisioning (@industrialised_food_drink_packaged_0066_portable_cake)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodDrinkAndPackagedConsumablesMakePortablePaperPuddingForDomesticProvisioningCraftIndustrialisedFoodDrinkPackaged0067PortablePudding() => new()
	{
		Metadata = new("Food, drink and packaged consumables/make portable paper pudding for domestic provisioning [craft_industrialised_food_drink_packaged_0067_portable_pudding]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_food_drink_packaged_0067_portable_pudding"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make portable paper pudding for domestic provisioning [craft_industrialised_food_drink_packaged_0067_portable_pudding]",
		Category = "Food, drink and packaged consumables",
		Blurb = "Make a portable paper pudding for domestic provisioning from prepared paper stock.",
		Action = "making portable paper pudding for domestic provisioning",
		ActiveCraftItemSdesc = "an in-progress portable paper pudding for domestic provisioning craft",
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
			new CatalogueCommodityRecipeInput(Grams: 3337.0, Material: "paper", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3337 grams of paper", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_food_drink_packaged_0067_portable_pudding", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a portable paper pudding for domestic provisioning (@industrialised_food_drink_packaged_0067_portable_pudding)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodDrinkAndPackagedConsumablesMakePortableCottonSausageForDomesticProvisioningCraftIndustrialisedFoodDrinkPackaged0068PortableSausage() => new()
	{
		Metadata = new("Food, drink and packaged consumables/make portable cotton sausage for domestic provisioning [craft_industrialised_food_drink_packaged_0068_portable_sausage]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_food_drink_packaged_0068_portable_sausage"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make portable cotton sausage for domestic provisioning [craft_industrialised_food_drink_packaged_0068_portable_sausage]",
		Category = "Food, drink and packaged consumables",
		Blurb = "Make a portable cotton sausage for domestic provisioning from prepared cotton stock.",
		Action = "making portable cotton sausage for domestic provisioning",
		ActiveCraftItemSdesc = "an in-progress portable cotton sausage for domestic provisioning craft",
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
			new CatalogueCommodityRecipeInput(Grams: 3378.0, Material: "cotton", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3378 grams of cotton", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_food_drink_packaged_0068_portable_sausage", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a portable cotton sausage for domestic provisioning (@industrialised_food_drink_packaged_0068_portable_sausage)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodDrinkAndPackagedConsumablesMakePortableCeramicPickleForDomesticProvisioningCraftIndustrialisedFoodDrinkPackaged0069PortablePickle() => new()
	{
		Metadata = new("Food, drink and packaged consumables/make portable ceramic pickle for domestic provisioning [craft_industrialised_food_drink_packaged_0069_portable_pickle]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_food_drink_packaged_0069_portable_pickle"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make portable ceramic pickle for domestic provisioning [craft_industrialised_food_drink_packaged_0069_portable_pickle]",
		Category = "Food, drink and packaged consumables",
		Blurb = "Make a portable ceramic pickle for domestic provisioning from prepared ceramic stock.",
		Action = "making portable ceramic pickle for domestic provisioning",
		ActiveCraftItemSdesc = "an in-progress portable ceramic pickle for domestic provisioning craft",
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
			new CatalogueCommodityRecipeInput(Grams: 3419.0, Material: "ceramic", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3419 grams of ceramic", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_food_drink_packaged_0069_portable_pickle", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a portable ceramic pickle for domestic provisioning (@industrialised_food_drink_packaged_0069_portable_pickle)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodDrinkAndPackagedConsumablesMakePortablePaperSweetForDomesticProvisioningCraftIndustrialisedFoodDrinkPackaged0070PortableSweet() => new()
	{
		Metadata = new("Food, drink and packaged consumables/make portable paper sweet for domestic provisioning [craft_industrialised_food_drink_packaged_0070_portable_sweet]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_food_drink_packaged_0070_portable_sweet"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make portable paper sweet for domestic provisioning [craft_industrialised_food_drink_packaged_0070_portable_sweet]",
		Category = "Food, drink and packaged consumables",
		Blurb = "Make a portable paper sweet for domestic provisioning from prepared paper stock.",
		Action = "making portable paper sweet for domestic provisioning",
		ActiveCraftItemSdesc = "an in-progress portable paper sweet for domestic provisioning craft",
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
			new CatalogueCommodityRecipeInput(Grams: 3460.0, Material: "paper", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3460 grams of paper", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_food_drink_packaged_0070_portable_sweet", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a portable paper sweet for domestic provisioning (@industrialised_food_drink_packaged_0070_portable_sweet)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodDrinkAndPackagedConsumablesMakePortableCottonCrackerForDomesticProvisioningCraftIndustrialisedFoodDrinkPackaged0071PortableCracker() => new()
	{
		Metadata = new("Food, drink and packaged consumables/make portable cotton cracker for domestic provisioning [craft_industrialised_food_drink_packaged_0071_portable_cracker]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_food_drink_packaged_0071_portable_cracker"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make portable cotton cracker for domestic provisioning [craft_industrialised_food_drink_packaged_0071_portable_cracker]",
		Category = "Food, drink and packaged consumables",
		Blurb = "Make a portable cotton cracker for domestic provisioning from prepared cotton stock.",
		Action = "making portable cotton cracker for domestic provisioning",
		ActiveCraftItemSdesc = "an in-progress portable cotton cracker for domestic provisioning craft",
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
			new CatalogueCommodityRecipeInput(Grams: 3500.0, Material: "cotton", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3500 grams of cotton", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_food_drink_packaged_0071_portable_cracker", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a portable cotton cracker for domestic provisioning (@industrialised_food_drink_packaged_0071_portable_cracker)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodDrinkAndPackagedConsumablesMakePortableCeramicPacketForDomesticProvisioningCraftIndustrialisedFoodDrinkPackaged0072PortablePacket() => new()
	{
		Metadata = new("Food, drink and packaged consumables/make portable ceramic packet for domestic provisioning [craft_industrialised_food_drink_packaged_0072_portable_packet]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_food_drink_packaged_0072_portable_packet"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make portable ceramic packet for domestic provisioning [craft_industrialised_food_drink_packaged_0072_portable_packet]",
		Category = "Food, drink and packaged consumables",
		Blurb = "Make a portable ceramic packet for domestic provisioning from prepared ceramic stock.",
		Action = "making portable ceramic packet for domestic provisioning",
		ActiveCraftItemSdesc = "an in-progress portable ceramic packet for domestic provisioning craft",
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
			new CatalogueCommodityRecipeInput(Grams: 3541.0, Material: "ceramic", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3541 grams of ceramic", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_food_drink_packaged_0072_portable_packet", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a portable ceramic packet for domestic provisioning (@industrialised_food_drink_packaged_0072_portable_packet)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodDrinkAndPackagedConsumablesMakeSomeWorkshopPaperCrumbsForDomesticProvisioningCraftIndustrialisedFoodDrinkPackaged0073WorkshopCrumbs() => new()
	{
		Metadata = new("Food, drink and packaged consumables/make some workshop paper crumbs for domestic provisioning [craft_industrialised_food_drink_packaged_0073_workshop_crumbs]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_food_drink_packaged_0073_workshop_crumbs"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make some workshop paper crumbs for domestic provisioning [craft_industrialised_food_drink_packaged_0073_workshop_crumbs]",
		Category = "Food, drink and packaged consumables",
		Blurb = "Make some workshop paper crumbs for domestic provisioning from prepared paper stock.",
		Action = "making some workshop paper crumbs for domestic provisioning",
		ActiveCraftItemSdesc = "an in-progress some workshop paper crumbs for domestic provisioning craft",
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
			new CatalogueCommodityRecipeInput(Grams: 3582.0, Material: "paper", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3582 grams of paper", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_food_drink_packaged_0073_workshop_crumbs", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x some workshop paper crumbs for domestic provisioning (@industrialised_food_drink_packaged_0073_workshop_crumbs)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodDrinkAndPackagedConsumablesMakeWorkshopCottonBiscuitForDomesticProvisioningCraftIndustrialisedFoodDrinkPackaged0074WorkshopBiscuit() => new()
	{
		Metadata = new("Food, drink and packaged consumables/make workshop cotton biscuit for domestic provisioning [craft_industrialised_food_drink_packaged_0074_workshop_biscuit]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_food_drink_packaged_0074_workshop_biscuit"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make workshop cotton biscuit for domestic provisioning [craft_industrialised_food_drink_packaged_0074_workshop_biscuit]",
		Category = "Food, drink and packaged consumables",
		Blurb = "Make a workshop cotton biscuit for domestic provisioning from prepared cotton stock.",
		Action = "making workshop cotton biscuit for domestic provisioning",
		ActiveCraftItemSdesc = "an in-progress workshop cotton biscuit for domestic provisioning craft",
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
			new CatalogueCommodityRecipeInput(Grams: 3622.0, Material: "cotton", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3622 grams of cotton", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_food_drink_packaged_0074_workshop_biscuit", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a workshop cotton biscuit for domestic provisioning (@industrialised_food_drink_packaged_0074_workshop_biscuit)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodDrinkAndPackagedConsumablesMakeWorkshopCeramicRationForDomesticProvisioningCraftIndustrialisedFoodDrinkPackaged0075WorkshopRation() => new()
	{
		Metadata = new("Food, drink and packaged consumables/make workshop ceramic ration for domestic provisioning [craft_industrialised_food_drink_packaged_0075_workshop_ration]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_food_drink_packaged_0075_workshop_ration"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make workshop ceramic ration for domestic provisioning [craft_industrialised_food_drink_packaged_0075_workshop_ration]",
		Category = "Food, drink and packaged consumables",
		Blurb = "Make a workshop ceramic ration for domestic provisioning from prepared ceramic stock.",
		Action = "making workshop ceramic ration for domestic provisioning",
		ActiveCraftItemSdesc = "an in-progress workshop ceramic ration for domestic provisioning craft",
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
			new CatalogueCommodityRecipeInput(Grams: 3663.0, Material: "ceramic", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3663 grams of ceramic", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_food_drink_packaged_0075_workshop_ration", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a workshop ceramic ration for domestic provisioning (@industrialised_food_drink_packaged_0075_workshop_ration)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodDrinkAndPackagedConsumablesMakeWorkshopPaperPreserveForDomesticProvisioningCraftIndustrialisedFoodDrinkPackaged0076WorkshopPreserve() => new()
	{
		Metadata = new("Food, drink and packaged consumables/make workshop paper preserve for domestic provisioning [craft_industrialised_food_drink_packaged_0076_workshop_preserve]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_food_drink_packaged_0076_workshop_preserve"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make workshop paper preserve for domestic provisioning [craft_industrialised_food_drink_packaged_0076_workshop_preserve]",
		Category = "Food, drink and packaged consumables",
		Blurb = "Make a workshop paper preserve for domestic provisioning from prepared paper stock.",
		Action = "making workshop paper preserve for domestic provisioning",
		ActiveCraftItemSdesc = "an in-progress workshop paper preserve for domestic provisioning craft",
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
			new CatalogueCommodityRecipeInput(Grams: 3704.0, Material: "paper", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3704 grams of paper", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_food_drink_packaged_0076_workshop_preserve", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a workshop paper preserve for domestic provisioning (@industrialised_food_drink_packaged_0076_workshop_preserve)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodDrinkAndPackagedConsumablesMakeWorkshopCottonLoafForDomesticProvisioningCraftIndustrialisedFoodDrinkPackaged0077WorkshopLoaf() => new()
	{
		Metadata = new("Food, drink and packaged consumables/make workshop cotton loaf for domestic provisioning [craft_industrialised_food_drink_packaged_0077_workshop_loaf]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_food_drink_packaged_0077_workshop_loaf"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make workshop cotton loaf for domestic provisioning [craft_industrialised_food_drink_packaged_0077_workshop_loaf]",
		Category = "Food, drink and packaged consumables",
		Blurb = "Make a workshop cotton loaf for domestic provisioning from prepared cotton stock.",
		Action = "making workshop cotton loaf for domestic provisioning",
		ActiveCraftItemSdesc = "an in-progress workshop cotton loaf for domestic provisioning craft",
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
			new CatalogueCommodityRecipeInput(Grams: 3744.0, Material: "cotton", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3744 grams of cotton", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_food_drink_packaged_0077_workshop_loaf", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a workshop cotton loaf for domestic provisioning (@industrialised_food_drink_packaged_0077_workshop_loaf)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodDrinkAndPackagedConsumablesMakeWorkshopCeramicCakeForDomesticProvisioningCraftIndustrialisedFoodDrinkPackaged0078WorkshopCake() => new()
	{
		Metadata = new("Food, drink and packaged consumables/make workshop ceramic cake for domestic provisioning [craft_industrialised_food_drink_packaged_0078_workshop_cake]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_food_drink_packaged_0078_workshop_cake"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make workshop ceramic cake for domestic provisioning [craft_industrialised_food_drink_packaged_0078_workshop_cake]",
		Category = "Food, drink and packaged consumables",
		Blurb = "Make a workshop ceramic cake for domestic provisioning from prepared ceramic stock.",
		Action = "making workshop ceramic cake for domestic provisioning",
		ActiveCraftItemSdesc = "an in-progress workshop ceramic cake for domestic provisioning craft",
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
			new CatalogueCommodityRecipeInput(Grams: 3785.0, Material: "ceramic", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3785 grams of ceramic", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_food_drink_packaged_0078_workshop_cake", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a workshop ceramic cake for domestic provisioning (@industrialised_food_drink_packaged_0078_workshop_cake)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodDrinkAndPackagedConsumablesMakeWorkshopPaperPuddingForDomesticProvisioningCraftIndustrialisedFoodDrinkPackaged0079WorkshopPudding() => new()
	{
		Metadata = new("Food, drink and packaged consumables/make workshop paper pudding for domestic provisioning [craft_industrialised_food_drink_packaged_0079_workshop_pudding]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_food_drink_packaged_0079_workshop_pudding"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make workshop paper pudding for domestic provisioning [craft_industrialised_food_drink_packaged_0079_workshop_pudding]",
		Category = "Food, drink and packaged consumables",
		Blurb = "Make a workshop paper pudding for domestic provisioning from prepared paper stock.",
		Action = "making workshop paper pudding for domestic provisioning",
		ActiveCraftItemSdesc = "an in-progress workshop paper pudding for domestic provisioning craft",
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
			new CatalogueCommodityRecipeInput(Grams: 3826.0, Material: "paper", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3826 grams of paper", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_food_drink_packaged_0079_workshop_pudding", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a workshop paper pudding for domestic provisioning (@industrialised_food_drink_packaged_0079_workshop_pudding)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodDrinkAndPackagedConsumablesMakeWorkshopCottonSausageForDomesticProvisioningCraftIndustrialisedFoodDrinkPackaged0080WorkshopSausage() => new()
	{
		Metadata = new("Food, drink and packaged consumables/make workshop cotton sausage for domestic provisioning [craft_industrialised_food_drink_packaged_0080_workshop_sausage]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_food_drink_packaged_0080_workshop_sausage"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make workshop cotton sausage for domestic provisioning [craft_industrialised_food_drink_packaged_0080_workshop_sausage]",
		Category = "Food, drink and packaged consumables",
		Blurb = "Make a workshop cotton sausage for domestic provisioning from prepared cotton stock.",
		Action = "making workshop cotton sausage for domestic provisioning",
		ActiveCraftItemSdesc = "an in-progress workshop cotton sausage for domestic provisioning craft",
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
			new CatalogueCommodityRecipeInput(Grams: 3867.0, Material: "cotton", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3867 grams of cotton", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_food_drink_packaged_0080_workshop_sausage", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a workshop cotton sausage for domestic provisioning (@industrialised_food_drink_packaged_0080_workshop_sausage)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodDrinkAndPackagedConsumablesMakeWorkshopCeramicPickleForDomesticProvisioningCraftIndustrialisedFoodDrinkPackaged0081WorkshopPickle() => new()
	{
		Metadata = new("Food, drink and packaged consumables/make workshop ceramic pickle for domestic provisioning [craft_industrialised_food_drink_packaged_0081_workshop_pickle]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_food_drink_packaged_0081_workshop_pickle"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make workshop ceramic pickle for domestic provisioning [craft_industrialised_food_drink_packaged_0081_workshop_pickle]",
		Category = "Food, drink and packaged consumables",
		Blurb = "Make a workshop ceramic pickle for domestic provisioning from prepared ceramic stock.",
		Action = "making workshop ceramic pickle for domestic provisioning",
		ActiveCraftItemSdesc = "an in-progress workshop ceramic pickle for domestic provisioning craft",
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
			new CatalogueCommodityRecipeInput(Grams: 3907.0, Material: "ceramic", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3907 grams of ceramic", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_food_drink_packaged_0081_workshop_pickle", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a workshop ceramic pickle for domestic provisioning (@industrialised_food_drink_packaged_0081_workshop_pickle)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodDrinkAndPackagedConsumablesMakeWorkshopPaperSweetForDomesticProvisioningCraftIndustrialisedFoodDrinkPackaged0082WorkshopSweet() => new()
	{
		Metadata = new("Food, drink and packaged consumables/make workshop paper sweet for domestic provisioning [craft_industrialised_food_drink_packaged_0082_workshop_sweet]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_food_drink_packaged_0082_workshop_sweet"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make workshop paper sweet for domestic provisioning [craft_industrialised_food_drink_packaged_0082_workshop_sweet]",
		Category = "Food, drink and packaged consumables",
		Blurb = "Make a workshop paper sweet for domestic provisioning from prepared paper stock.",
		Action = "making workshop paper sweet for domestic provisioning",
		ActiveCraftItemSdesc = "an in-progress workshop paper sweet for domestic provisioning craft",
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
			new CatalogueCommodityRecipeInput(Grams: 3948.0, Material: "paper", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3948 grams of paper", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_food_drink_packaged_0082_workshop_sweet", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a workshop paper sweet for domestic provisioning (@industrialised_food_drink_packaged_0082_workshop_sweet)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodDrinkAndPackagedConsumablesMakeWorkshopCottonCrackerForDomesticProvisioningCraftIndustrialisedFoodDrinkPackaged0083WorkshopCracker() => new()
	{
		Metadata = new("Food, drink and packaged consumables/make workshop cotton cracker for domestic provisioning [craft_industrialised_food_drink_packaged_0083_workshop_cracker]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_food_drink_packaged_0083_workshop_cracker"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make workshop cotton cracker for domestic provisioning [craft_industrialised_food_drink_packaged_0083_workshop_cracker]",
		Category = "Food, drink and packaged consumables",
		Blurb = "Make a workshop cotton cracker for domestic provisioning from prepared cotton stock.",
		Action = "making workshop cotton cracker for domestic provisioning",
		ActiveCraftItemSdesc = "an in-progress workshop cotton cracker for domestic provisioning craft",
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
			new CatalogueCommodityRecipeInput(Grams: 3989.0, Material: "cotton", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3989 grams of cotton", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_food_drink_packaged_0083_workshop_cracker", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a workshop cotton cracker for domestic provisioning (@industrialised_food_drink_packaged_0083_workshop_cracker)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodDrinkAndPackagedConsumablesMakeWorkshopCeramicPacketForDomesticProvisioningCraftIndustrialisedFoodDrinkPackaged0084WorkshopPacket() => new()
	{
		Metadata = new("Food, drink and packaged consumables/make workshop ceramic packet for domestic provisioning [craft_industrialised_food_drink_packaged_0084_workshop_packet]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_food_drink_packaged_0084_workshop_packet"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make workshop ceramic packet for domestic provisioning [craft_industrialised_food_drink_packaged_0084_workshop_packet]",
		Category = "Food, drink and packaged consumables",
		Blurb = "Make a workshop ceramic packet for domestic provisioning from prepared ceramic stock.",
		Action = "making workshop ceramic packet for domestic provisioning",
		ActiveCraftItemSdesc = "an in-progress workshop ceramic packet for domestic provisioning craft",
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
			new CatalogueCommodityRecipeInput(Grams: 4029.0, Material: "ceramic", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("4029 grams of ceramic", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_food_drink_packaged_0084_workshop_packet", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a workshop ceramic packet for domestic provisioning (@industrialised_food_drink_packaged_0084_workshop_packet)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodDrinkAndPackagedConsumablesMakeSomeInstitutionalPaperCrumbsForDomesticProvisioningCraftIndustrialisedFoodDrinkPackaged0085InstitutionalCrumbs() => new()
	{
		Metadata = new("Food, drink and packaged consumables/make some institutional paper crumbs for domestic provisioning [craft_industrialised_food_drink_packaged_0085_institutional_crumbs]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_food_drink_packaged_0085_institutional_crumbs"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make some institutional paper crumbs for domestic provisioning [craft_industrialised_food_drink_packaged_0085_institutional_crumbs]",
		Category = "Food, drink and packaged consumables",
		Blurb = "Make some institutional paper crumbs for domestic provisioning from prepared paper stock.",
		Action = "making some institutional paper crumbs for domestic provisioning",
		ActiveCraftItemSdesc = "an in-progress some institutional paper crumbs for domestic provisioning craft",
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
			new CatalogueCommodityRecipeInput(Grams: 4070.0, Material: "paper", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("4070 grams of paper", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_food_drink_packaged_0085_institutional_crumbs", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x some institutional paper crumbs for domestic provisioning (@industrialised_food_drink_packaged_0085_institutional_crumbs)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodDrinkAndPackagedConsumablesMakeInstitutionalCottonBiscuitForDomesticProvisioningCraftIndustrialisedFoodDrinkPackaged0086InstitutionalBiscuit() => new()
	{
		Metadata = new("Food, drink and packaged consumables/make institutional cotton biscuit for domestic provisioning [craft_industrialised_food_drink_packaged_0086_institutional_biscuit]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_food_drink_packaged_0086_institutional_biscuit"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make institutional cotton biscuit for domestic provisioning [craft_industrialised_food_drink_packaged_0086_institutional_biscuit]",
		Category = "Food, drink and packaged consumables",
		Blurb = "Make an institutional cotton biscuit for domestic provisioning from prepared cotton stock.",
		Action = "making institutional cotton biscuit for domestic provisioning",
		ActiveCraftItemSdesc = "an in-progress institutional cotton biscuit for domestic provisioning craft",
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
			new CatalogueCommodityRecipeInput(Grams: 4111.0, Material: "cotton", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("4111 grams of cotton", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_food_drink_packaged_0086_institutional_biscuit", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x an institutional cotton biscuit for domestic provisioning (@industrialised_food_drink_packaged_0086_institutional_biscuit)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodDrinkAndPackagedConsumablesMakeInstitutionalCeramicRationForDomesticProvisioningCraftIndustrialisedFoodDrinkPackaged0087InstitutionalRation() => new()
	{
		Metadata = new("Food, drink and packaged consumables/make institutional ceramic ration for domestic provisioning [craft_industrialised_food_drink_packaged_0087_institutional_ration]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_food_drink_packaged_0087_institutional_ration"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make institutional ceramic ration for domestic provisioning [craft_industrialised_food_drink_packaged_0087_institutional_ration]",
		Category = "Food, drink and packaged consumables",
		Blurb = "Make an institutional ceramic ration for domestic provisioning from prepared ceramic stock.",
		Action = "making institutional ceramic ration for domestic provisioning",
		ActiveCraftItemSdesc = "an in-progress institutional ceramic ration for domestic provisioning craft",
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
			new CatalogueCommodityRecipeInput(Grams: 4151.0, Material: "ceramic", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("4151 grams of ceramic", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_food_drink_packaged_0087_institutional_ration", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x an institutional ceramic ration for domestic provisioning (@industrialised_food_drink_packaged_0087_institutional_ration)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodDrinkAndPackagedConsumablesMakeInstitutionalPaperPreserveForDomesticProvisioningCraftIndustrialisedFoodDrinkPackaged0088InstitutionalPreserve() => new()
	{
		Metadata = new("Food, drink and packaged consumables/make institutional paper preserve for domestic provisioning [craft_industrialised_food_drink_packaged_0088_institutional_preserve]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_food_drink_packaged_0088_institutional_preserve"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make institutional paper preserve for domestic provisioning [craft_industrialised_food_drink_packaged_0088_institutional_preserve]",
		Category = "Food, drink and packaged consumables",
		Blurb = "Make an institutional paper preserve for domestic provisioning from prepared paper stock.",
		Action = "making institutional paper preserve for domestic provisioning",
		ActiveCraftItemSdesc = "an in-progress institutional paper preserve for domestic provisioning craft",
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
			new CatalogueCommodityRecipeInput(Grams: 4192.0, Material: "paper", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("4192 grams of paper", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_food_drink_packaged_0088_institutional_preserve", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x an institutional paper preserve for domestic provisioning (@industrialised_food_drink_packaged_0088_institutional_preserve)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodDrinkAndPackagedConsumablesMakeInstitutionalCottonLoafForDomesticProvisioningCraftIndustrialisedFoodDrinkPackaged0089InstitutionalLoaf() => new()
	{
		Metadata = new("Food, drink and packaged consumables/make institutional cotton loaf for domestic provisioning [craft_industrialised_food_drink_packaged_0089_institutional_loaf]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_food_drink_packaged_0089_institutional_loaf"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make institutional cotton loaf for domestic provisioning [craft_industrialised_food_drink_packaged_0089_institutional_loaf]",
		Category = "Food, drink and packaged consumables",
		Blurb = "Make an institutional cotton loaf for domestic provisioning from prepared cotton stock.",
		Action = "making institutional cotton loaf for domestic provisioning",
		ActiveCraftItemSdesc = "an in-progress institutional cotton loaf for domestic provisioning craft",
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
			new CatalogueCommodityRecipeInput(Grams: 4233.0, Material: "cotton", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("4233 grams of cotton", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_food_drink_packaged_0089_institutional_loaf", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x an institutional cotton loaf for domestic provisioning (@industrialised_food_drink_packaged_0089_institutional_loaf)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodDrinkAndPackagedConsumablesMakeInstitutionalCeramicCakeForDomesticProvisioningCraftIndustrialisedFoodDrinkPackaged0090InstitutionalCake() => new()
	{
		Metadata = new("Food, drink and packaged consumables/make institutional ceramic cake for domestic provisioning [craft_industrialised_food_drink_packaged_0090_institutional_cake]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_food_drink_packaged_0090_institutional_cake"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make institutional ceramic cake for domestic provisioning [craft_industrialised_food_drink_packaged_0090_institutional_cake]",
		Category = "Food, drink and packaged consumables",
		Blurb = "Make an institutional ceramic cake for domestic provisioning from prepared ceramic stock.",
		Action = "making institutional ceramic cake for domestic provisioning",
		ActiveCraftItemSdesc = "an in-progress institutional ceramic cake for domestic provisioning craft",
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
			new CatalogueCommodityRecipeInput(Grams: 4274.0, Material: "ceramic", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("4274 grams of ceramic", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_food_drink_packaged_0090_institutional_cake", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x an institutional ceramic cake for domestic provisioning (@industrialised_food_drink_packaged_0090_institutional_cake)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodDrinkAndPackagedConsumablesMakeInstitutionalPaperPuddingForDomesticProvisioningCraftIndustrialisedFoodDrinkPackaged0091InstitutionalPudding() => new()
	{
		Metadata = new("Food, drink and packaged consumables/make institutional paper pudding for domestic provisioning [craft_industrialised_food_drink_packaged_0091_institutional_pudding]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_food_drink_packaged_0091_institutional_pudding"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make institutional paper pudding for domestic provisioning [craft_industrialised_food_drink_packaged_0091_institutional_pudding]",
		Category = "Food, drink and packaged consumables",
		Blurb = "Make an institutional paper pudding for domestic provisioning from prepared paper stock.",
		Action = "making institutional paper pudding for domestic provisioning",
		ActiveCraftItemSdesc = "an in-progress institutional paper pudding for domestic provisioning craft",
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
			new CatalogueCommodityRecipeInput(Grams: 4314.0, Material: "paper", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("4314 grams of paper", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_food_drink_packaged_0091_institutional_pudding", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x an institutional paper pudding for domestic provisioning (@industrialised_food_drink_packaged_0091_institutional_pudding)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodDrinkAndPackagedConsumablesMakeInstitutionalCottonSausageForDomesticProvisioningCraftIndustrialisedFoodDrinkPackaged0092InstitutionalSausage() => new()
	{
		Metadata = new("Food, drink and packaged consumables/make institutional cotton sausage for domestic provisioning [craft_industrialised_food_drink_packaged_0092_institutional_sausage]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_food_drink_packaged_0092_institutional_sausage"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make institutional cotton sausage for domestic provisioning [craft_industrialised_food_drink_packaged_0092_institutional_sausage]",
		Category = "Food, drink and packaged consumables",
		Blurb = "Make an institutional cotton sausage for domestic provisioning from prepared cotton stock.",
		Action = "making institutional cotton sausage for domestic provisioning",
		ActiveCraftItemSdesc = "an in-progress institutional cotton sausage for domestic provisioning craft",
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
			new CatalogueCommodityRecipeInput(Grams: 4355.0, Material: "cotton", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("4355 grams of cotton", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_food_drink_packaged_0092_institutional_sausage", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x an institutional cotton sausage for domestic provisioning (@industrialised_food_drink_packaged_0092_institutional_sausage)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodDrinkAndPackagedConsumablesMakeInstitutionalCeramicPickleForDomesticProvisioningCraftIndustrialisedFoodDrinkPackaged0093InstitutionalPickle() => new()
	{
		Metadata = new("Food, drink and packaged consumables/make institutional ceramic pickle for domestic provisioning [craft_industrialised_food_drink_packaged_0093_institutional_pickle]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_food_drink_packaged_0093_institutional_pickle"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make institutional ceramic pickle for domestic provisioning [craft_industrialised_food_drink_packaged_0093_institutional_pickle]",
		Category = "Food, drink and packaged consumables",
		Blurb = "Make an institutional ceramic pickle for domestic provisioning from prepared ceramic stock.",
		Action = "making institutional ceramic pickle for domestic provisioning",
		ActiveCraftItemSdesc = "an in-progress institutional ceramic pickle for domestic provisioning craft",
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
			new CatalogueCommodityRecipeInput(Grams: 4396.0, Material: "ceramic", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("4396 grams of ceramic", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_food_drink_packaged_0093_institutional_pickle", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x an institutional ceramic pickle for domestic provisioning (@industrialised_food_drink_packaged_0093_institutional_pickle)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodDrinkAndPackagedConsumablesMakeInstitutionalPaperSweetForDomesticProvisioningCraftIndustrialisedFoodDrinkPackaged0094InstitutionalSweet() => new()
	{
		Metadata = new("Food, drink and packaged consumables/make institutional paper sweet for domestic provisioning [craft_industrialised_food_drink_packaged_0094_institutional_sweet]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_food_drink_packaged_0094_institutional_sweet"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make institutional paper sweet for domestic provisioning [craft_industrialised_food_drink_packaged_0094_institutional_sweet]",
		Category = "Food, drink and packaged consumables",
		Blurb = "Make an institutional paper sweet for domestic provisioning from prepared paper stock.",
		Action = "making institutional paper sweet for domestic provisioning",
		ActiveCraftItemSdesc = "an in-progress institutional paper sweet for domestic provisioning craft",
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
			new CatalogueCommodityRecipeInput(Grams: 4436.0, Material: "paper", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("4436 grams of paper", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_food_drink_packaged_0094_institutional_sweet", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x an institutional paper sweet for domestic provisioning (@industrialised_food_drink_packaged_0094_institutional_sweet)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodDrinkAndPackagedConsumablesMakeInstitutionalCottonCrackerForDomesticProvisioningCraftIndustrialisedFoodDrinkPackaged0095InstitutionalCracker() => new()
	{
		Metadata = new("Food, drink and packaged consumables/make institutional cotton cracker for domestic provisioning [craft_industrialised_food_drink_packaged_0095_institutional_cracker]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_food_drink_packaged_0095_institutional_cracker"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make institutional cotton cracker for domestic provisioning [craft_industrialised_food_drink_packaged_0095_institutional_cracker]",
		Category = "Food, drink and packaged consumables",
		Blurb = "Make an institutional cotton cracker for domestic provisioning from prepared cotton stock.",
		Action = "making institutional cotton cracker for domestic provisioning",
		ActiveCraftItemSdesc = "an in-progress institutional cotton cracker for domestic provisioning craft",
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
			new CatalogueCommodityRecipeInput(Grams: 4477.0, Material: "cotton", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("4477 grams of cotton", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_food_drink_packaged_0095_institutional_cracker", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x an institutional cotton cracker for domestic provisioning (@industrialised_food_drink_packaged_0095_institutional_cracker)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodDrinkAndPackagedConsumablesMakeInstitutionalCeramicPacketForDomesticProvisioningCraftIndustrialisedFoodDrinkPackaged0096InstitutionalPacket() => new()
	{
		Metadata = new("Food, drink and packaged consumables/make institutional ceramic packet for domestic provisioning [craft_industrialised_food_drink_packaged_0096_institutional_packet]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_food_drink_packaged_0096_institutional_packet"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make institutional ceramic packet for domestic provisioning [craft_industrialised_food_drink_packaged_0096_institutional_packet]",
		Category = "Food, drink and packaged consumables",
		Blurb = "Make an institutional ceramic packet for domestic provisioning from prepared ceramic stock.",
		Action = "making institutional ceramic packet for domestic provisioning",
		ActiveCraftItemSdesc = "an in-progress institutional ceramic packet for domestic provisioning craft",
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
			new CatalogueCommodityRecipeInput(Grams: 4518.0, Material: "ceramic", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("4518 grams of ceramic", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_food_drink_packaged_0096_institutional_packet", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x an institutional ceramic packet for domestic provisioning (@industrialised_food_drink_packaged_0096_institutional_packet)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodDrinkAndPackagedConsumablesMakeSomeWeatherproofPaperCrumbsForDomesticProvisioningCraftIndustrialisedFoodDrinkPackaged0097WeatherproofCrumbs() => new()
	{
		Metadata = new("Food, drink and packaged consumables/make some weatherproof paper crumbs for domestic provisioning [craft_industrialised_food_drink_packaged_0097_weatherproof_crumbs]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_food_drink_packaged_0097_weatherproof_crumbs"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make some weatherproof paper crumbs for domestic provisioning [craft_industrialised_food_drink_packaged_0097_weatherproof_crumbs]",
		Category = "Food, drink and packaged consumables",
		Blurb = "Make some weatherproof paper crumbs for domestic provisioning from prepared paper stock.",
		Action = "making some weatherproof paper crumbs for domestic provisioning",
		ActiveCraftItemSdesc = "an in-progress some weatherproof paper crumbs for domestic provisioning craft",
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
			new CatalogueCommodityRecipeInput(Grams: 4558.0, Material: "paper", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("4558 grams of paper", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_food_drink_packaged_0097_weatherproof_crumbs", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x some weatherproof paper crumbs for domestic provisioning (@industrialised_food_drink_packaged_0097_weatherproof_crumbs)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodDrinkAndPackagedConsumablesMakeWeatherproofCottonBiscuitForDomesticProvisioningCraftIndustrialisedFoodDrinkPackaged0098WeatherproofBiscuit() => new()
	{
		Metadata = new("Food, drink and packaged consumables/make weatherproof cotton biscuit for domestic provisioning [craft_industrialised_food_drink_packaged_0098_weatherproof_biscuit]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_food_drink_packaged_0098_weatherproof_biscuit"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make weatherproof cotton biscuit for domestic provisioning [craft_industrialised_food_drink_packaged_0098_weatherproof_biscuit]",
		Category = "Food, drink and packaged consumables",
		Blurb = "Make a weatherproof cotton biscuit for domestic provisioning from prepared cotton stock.",
		Action = "making weatherproof cotton biscuit for domestic provisioning",
		ActiveCraftItemSdesc = "an in-progress weatherproof cotton biscuit for domestic provisioning craft",
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
			new CatalogueCommodityRecipeInput(Grams: 4599.0, Material: "cotton", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("4599 grams of cotton", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_food_drink_packaged_0098_weatherproof_biscuit", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a weatherproof cotton biscuit for domestic provisioning (@industrialised_food_drink_packaged_0098_weatherproof_biscuit)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodDrinkAndPackagedConsumablesMakeWeatherproofCeramicRationForDomesticProvisioningCraftIndustrialisedFoodDrinkPackaged0099WeatherproofRation() => new()
	{
		Metadata = new("Food, drink and packaged consumables/make weatherproof ceramic ration for domestic provisioning [craft_industrialised_food_drink_packaged_0099_weatherproof_ration]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_food_drink_packaged_0099_weatherproof_ration"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make weatherproof ceramic ration for domestic provisioning [craft_industrialised_food_drink_packaged_0099_weatherproof_ration]",
		Category = "Food, drink and packaged consumables",
		Blurb = "Make a weatherproof ceramic ration for domestic provisioning from prepared ceramic stock.",
		Action = "making weatherproof ceramic ration for domestic provisioning",
		ActiveCraftItemSdesc = "an in-progress weatherproof ceramic ration for domestic provisioning craft",
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
			new CatalogueCommodityRecipeInput(Grams: 4640.0, Material: "ceramic", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("4640 grams of ceramic", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_food_drink_packaged_0099_weatherproof_ration", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a weatherproof ceramic ration for domestic provisioning (@industrialised_food_drink_packaged_0099_weatherproof_ration)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodDrinkAndPackagedConsumablesMakeWeatherproofPaperPreserveForDomesticProvisioningCraftIndustrialisedFoodDrinkPackaged0100WeatherproofPreserve() => new()
	{
		Metadata = new("Food, drink and packaged consumables/make weatherproof paper preserve for domestic provisioning [craft_industrialised_food_drink_packaged_0100_weatherproof_preserve]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_food_drink_packaged_0100_weatherproof_preserve"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make weatherproof paper preserve for domestic provisioning [craft_industrialised_food_drink_packaged_0100_weatherproof_preserve]",
		Category = "Food, drink and packaged consumables",
		Blurb = "Make a weatherproof paper preserve for domestic provisioning from prepared paper stock.",
		Action = "making weatherproof paper preserve for domestic provisioning",
		ActiveCraftItemSdesc = "an in-progress weatherproof paper preserve for domestic provisioning craft",
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
			new CatalogueCommodityRecipeInput(Grams: 4680.0, Material: "paper", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("4680 grams of paper", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_food_drink_packaged_0100_weatherproof_preserve", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a weatherproof paper preserve for domestic provisioning (@industrialised_food_drink_packaged_0100_weatherproof_preserve)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodDrinkAndPackagedConsumablesMakeWeatherproofCottonLoafForDomesticProvisioningCraftIndustrialisedFoodDrinkPackaged0101WeatherproofLoaf() => new()
	{
		Metadata = new("Food, drink and packaged consumables/make weatherproof cotton loaf for domestic provisioning [craft_industrialised_food_drink_packaged_0101_weatherproof_loaf]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_food_drink_packaged_0101_weatherproof_loaf"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make weatherproof cotton loaf for domestic provisioning [craft_industrialised_food_drink_packaged_0101_weatherproof_loaf]",
		Category = "Food, drink and packaged consumables",
		Blurb = "Make a weatherproof cotton loaf for domestic provisioning from prepared cotton stock.",
		Action = "making weatherproof cotton loaf for domestic provisioning",
		ActiveCraftItemSdesc = "an in-progress weatherproof cotton loaf for domestic provisioning craft",
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
			new CatalogueCommodityRecipeInput(Grams: 4721.0, Material: "cotton", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("4721 grams of cotton", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_food_drink_packaged_0101_weatherproof_loaf", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a weatherproof cotton loaf for domestic provisioning (@industrialised_food_drink_packaged_0101_weatherproof_loaf)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodDrinkAndPackagedConsumablesMakeWeatherproofCeramicCakeForDomesticProvisioningCraftIndustrialisedFoodDrinkPackaged0102WeatherproofCake() => new()
	{
		Metadata = new("Food, drink and packaged consumables/make weatherproof ceramic cake for domestic provisioning [craft_industrialised_food_drink_packaged_0102_weatherproof_cake]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_food_drink_packaged_0102_weatherproof_cake"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make weatherproof ceramic cake for domestic provisioning [craft_industrialised_food_drink_packaged_0102_weatherproof_cake]",
		Category = "Food, drink and packaged consumables",
		Blurb = "Make a weatherproof ceramic cake for domestic provisioning from prepared ceramic stock.",
		Action = "making weatherproof ceramic cake for domestic provisioning",
		ActiveCraftItemSdesc = "an in-progress weatherproof ceramic cake for domestic provisioning craft",
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
			new CatalogueCommodityRecipeInput(Grams: 4762.0, Material: "ceramic", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("4762 grams of ceramic", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_food_drink_packaged_0102_weatherproof_cake", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a weatherproof ceramic cake for domestic provisioning (@industrialised_food_drink_packaged_0102_weatherproof_cake)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodDrinkAndPackagedConsumablesMakeWeatherproofPaperPuddingForDomesticProvisioningCraftIndustrialisedFoodDrinkPackaged0103WeatherproofPudding() => new()
	{
		Metadata = new("Food, drink and packaged consumables/make weatherproof paper pudding for domestic provisioning [craft_industrialised_food_drink_packaged_0103_weatherproof_pudding]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_food_drink_packaged_0103_weatherproof_pudding"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make weatherproof paper pudding for domestic provisioning [craft_industrialised_food_drink_packaged_0103_weatherproof_pudding]",
		Category = "Food, drink and packaged consumables",
		Blurb = "Make a weatherproof paper pudding for domestic provisioning from prepared paper stock.",
		Action = "making weatherproof paper pudding for domestic provisioning",
		ActiveCraftItemSdesc = "an in-progress weatherproof paper pudding for domestic provisioning craft",
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
			new CatalogueCommodityRecipeInput(Grams: 4803.0, Material: "paper", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("4803 grams of paper", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_food_drink_packaged_0103_weatherproof_pudding", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a weatherproof paper pudding for domestic provisioning (@industrialised_food_drink_packaged_0103_weatherproof_pudding)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodDrinkAndPackagedConsumablesMakeWeatherproofCottonSausageForDomesticProvisioningCraftIndustrialisedFoodDrinkPackaged0104WeatherproofSausage() => new()
	{
		Metadata = new("Food, drink and packaged consumables/make weatherproof cotton sausage for domestic provisioning [craft_industrialised_food_drink_packaged_0104_weatherproof_sausage]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_food_drink_packaged_0104_weatherproof_sausage"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make weatherproof cotton sausage for domestic provisioning [craft_industrialised_food_drink_packaged_0104_weatherproof_sausage]",
		Category = "Food, drink and packaged consumables",
		Blurb = "Make a weatherproof cotton sausage for domestic provisioning from prepared cotton stock.",
		Action = "making weatherproof cotton sausage for domestic provisioning",
		ActiveCraftItemSdesc = "an in-progress weatherproof cotton sausage for domestic provisioning craft",
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
			new CatalogueCommodityRecipeInput(Grams: 4843.0, Material: "cotton", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("4843 grams of cotton", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_food_drink_packaged_0104_weatherproof_sausage", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a weatherproof cotton sausage for domestic provisioning (@industrialised_food_drink_packaged_0104_weatherproof_sausage)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodDrinkAndPackagedConsumablesMakeWeatherproofCeramicPickleForDomesticProvisioningCraftIndustrialisedFoodDrinkPackaged0105WeatherproofPickle() => new()
	{
		Metadata = new("Food, drink and packaged consumables/make weatherproof ceramic pickle for domestic provisioning [craft_industrialised_food_drink_packaged_0105_weatherproof_pickle]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_food_drink_packaged_0105_weatherproof_pickle"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make weatherproof ceramic pickle for domestic provisioning [craft_industrialised_food_drink_packaged_0105_weatherproof_pickle]",
		Category = "Food, drink and packaged consumables",
		Blurb = "Make a weatherproof ceramic pickle for domestic provisioning from prepared ceramic stock.",
		Action = "making weatherproof ceramic pickle for domestic provisioning",
		ActiveCraftItemSdesc = "an in-progress weatherproof ceramic pickle for domestic provisioning craft",
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
			new CatalogueCommodityRecipeInput(Grams: 4884.0, Material: "ceramic", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("4884 grams of ceramic", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_food_drink_packaged_0105_weatherproof_pickle", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a weatherproof ceramic pickle for domestic provisioning (@industrialised_food_drink_packaged_0105_weatherproof_pickle)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};
}
