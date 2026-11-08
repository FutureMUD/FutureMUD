#nullable enable

using MudSharp.Body;
using MudSharp.GameItems.Inventory.Plans;
using MudSharp.RPG.Checks;

namespace DatabaseSeeder.Seeders;

public static partial class Recipes20Stock
{
	public static CatalogueRecipeDefinition KitchenAndAppliancesMakeServiceMildSteelPotForDomesticCookeryCraftIndustrialisedKitchenAppliances0112ServicePot() => new()
	{
		Metadata = new("Kitchen and appliances/make service mild steel pot for domestic cookery [craft_industrialised_kitchen_appliances_0112_service_pot]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_kitchen_appliances_0112_service_pot"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make service mild steel pot for domestic cookery [craft_industrialised_kitchen_appliances_0112_service_pot]",
		Category = "Kitchen and appliances",
		Blurb = "Make a service mild steel pot for domestic cookery from prepared mild steel stock.",
		Action = "making service mild steel pot for domestic cookery",
		ActiveCraftItemSdesc = "an in-progress service mild steel pot for domestic cookery craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 5058.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("5058 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_kitchen_appliances_0112_service_pot", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a service mild steel pot for domestic cookery (@industrialised_kitchen_appliances_0112_service_pot)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition KitchenAndAppliancesMakeServiceCastIronGraterForDomesticCookeryCraftIndustrialisedKitchenAppliances0113ServiceGrater() => new()
	{
		Metadata = new("Kitchen and appliances/make service cast iron grater for domestic cookery [craft_industrialised_kitchen_appliances_0113_service_grater]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_kitchen_appliances_0113_service_grater"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make service cast iron grater for domestic cookery [craft_industrialised_kitchen_appliances_0113_service_grater]",
		Category = "Kitchen and appliances",
		Blurb = "Make a service cast iron grater for domestic cookery from prepared cast iron stock.",
		Action = "making service cast iron grater for domestic cookery",
		ActiveCraftItemSdesc = "an in-progress service cast iron grater for domestic cookery craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 5098.0, Material: "cast iron", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("5098 grams of cast iron", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_kitchen_appliances_0113_service_grater", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a service cast iron grater for domestic cookery (@industrialised_kitchen_appliances_0113_service_grater)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition KitchenAndAppliancesMakeServiceCopperWhiskForDomesticCookeryCraftIndustrialisedKitchenAppliances0114ServiceWhisk() => new()
	{
		Metadata = new("Kitchen and appliances/make service copper whisk for domestic cookery [craft_industrialised_kitchen_appliances_0114_service_whisk]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_kitchen_appliances_0114_service_whisk"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make service copper whisk for domestic cookery [craft_industrialised_kitchen_appliances_0114_service_whisk]",
		Category = "Kitchen and appliances",
		Blurb = "Make a service copper whisk for domestic cookery from prepared copper stock.",
		Action = "making service copper whisk for domestic cookery",
		ActiveCraftItemSdesc = "an in-progress service copper whisk for domestic cookery craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 5139.0, Material: "copper", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("5139 grams of copper", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_kitchen_appliances_0114_service_whisk", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a service copper whisk for domestic cookery (@industrialised_kitchen_appliances_0114_service_whisk)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition KitchenAndAppliancesMakeServiceCeramicLadleForDomesticCookeryCraftIndustrialisedKitchenAppliances0115ServiceLadle() => new()
	{
		Metadata = new("Kitchen and appliances/make service ceramic ladle for domestic cookery [craft_industrialised_kitchen_appliances_0115_service_ladle]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_kitchen_appliances_0115_service_ladle"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make service ceramic ladle for domestic cookery [craft_industrialised_kitchen_appliances_0115_service_ladle]",
		Category = "Kitchen and appliances",
		Blurb = "Make a service ceramic ladle for domestic cookery from prepared ceramic stock.",
		Action = "making service ceramic ladle for domestic cookery",
		ActiveCraftItemSdesc = "an in-progress service ceramic ladle for domestic cookery craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 5180.0, Material: "ceramic", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("5180 grams of ceramic", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_kitchen_appliances_0115_service_ladle", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a service ceramic ladle for domestic cookery (@industrialised_kitchen_appliances_0115_service_ladle)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition KitchenAndAppliancesMakeServiceMildSteelStoveForDomesticCookeryCraftIndustrialisedKitchenAppliances0116ServiceStove() => new()
	{
		Metadata = new("Kitchen and appliances/make service mild steel stove for domestic cookery [craft_industrialised_kitchen_appliances_0116_service_stove]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_kitchen_appliances_0116_service_stove"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make service mild steel stove for domestic cookery [craft_industrialised_kitchen_appliances_0116_service_stove]",
		Category = "Kitchen and appliances",
		Blurb = "Make a service mild steel stove for domestic cookery from prepared mild steel stock.",
		Action = "making service mild steel stove for domestic cookery",
		ActiveCraftItemSdesc = "an in-progress service mild steel stove for domestic cookery craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 5221.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("5221 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_kitchen_appliances_0116_service_stove", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a service mild steel stove for domestic cookery (@industrialised_kitchen_appliances_0116_service_stove)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition KitchenAndAppliancesMakeServiceCastIronToasterForDomesticCookeryCraftIndustrialisedKitchenAppliances0117ServiceToaster() => new()
	{
		Metadata = new("Kitchen and appliances/make service cast iron toaster for domestic cookery [craft_industrialised_kitchen_appliances_0117_service_toaster]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_kitchen_appliances_0117_service_toaster"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make service cast iron toaster for domestic cookery [craft_industrialised_kitchen_appliances_0117_service_toaster]",
		Category = "Kitchen and appliances",
		Blurb = "Make a service cast iron toaster for domestic cookery from prepared cast iron stock.",
		Action = "making service cast iron toaster for domestic cookery",
		ActiveCraftItemSdesc = "an in-progress service cast iron toaster for domestic cookery craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 5261.0, Material: "cast iron", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("5261 grams of cast iron", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_kitchen_appliances_0117_service_toaster", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a service cast iron toaster for domestic cookery (@industrialised_kitchen_appliances_0117_service_toaster)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition KitchenAndAppliancesMakeInstitutionalCopperKettleForWorkshopCookeryCraftIndustrialKitchenAppliances0001InstitutionalKettle() => new()
	{
		Metadata = new("Kitchen and appliances/make institutional copper kettle for workshop cookery [craft_industrial_kitchen_appliances_0001_institutional_kettle]", "crafts", ["industrial"], ["item:industrial_kitchen_appliances_0001_institutional_kettle"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make institutional copper kettle for workshop cookery [craft_industrial_kitchen_appliances_0001_institutional_kettle]",
		Category = "Kitchen and appliances",
		Blurb = "Make an institutional copper kettle for workshop cookery from prepared copper stock.",
		Action = "making institutional copper kettle for workshop cookery",
		ActiveCraftItemSdesc = "an in-progress institutional copper kettle for workshop cookery craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 540.0, Material: "copper", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("540 grams of copper", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrial_kitchen_appliances_0001_institutional_kettle", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x an institutional copper kettle for workshop cookery (@industrial_kitchen_appliances_0001_institutional_kettle)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition KitchenAndAppliancesMakeInstitutionalCeramicPanForWorkshopCookeryCraftIndustrialKitchenAppliances0002InstitutionalPan() => new()
	{
		Metadata = new("Kitchen and appliances/make institutional ceramic pan for workshop cookery [craft_industrial_kitchen_appliances_0002_institutional_pan]", "crafts", ["industrial"], ["item:industrial_kitchen_appliances_0002_institutional_pan"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make institutional ceramic pan for workshop cookery [craft_industrial_kitchen_appliances_0002_institutional_pan]",
		Category = "Kitchen and appliances",
		Blurb = "Make an institutional ceramic pan for workshop cookery from prepared ceramic stock.",
		Action = "making institutional ceramic pan for workshop cookery",
		ActiveCraftItemSdesc = "an in-progress institutional ceramic pan for workshop cookery craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 581.0, Material: "ceramic", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("581 grams of ceramic", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrial_kitchen_appliances_0002_institutional_pan", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x an institutional ceramic pan for workshop cookery (@industrial_kitchen_appliances_0002_institutional_pan)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition KitchenAndAppliancesMakeInstitutionalMildSteelPotForWorkshopCookeryCraftIndustrialKitchenAppliances0003InstitutionalPot() => new()
	{
		Metadata = new("Kitchen and appliances/make institutional mild steel pot for workshop cookery [craft_industrial_kitchen_appliances_0003_institutional_pot]", "crafts", ["industrial"], ["item:industrial_kitchen_appliances_0003_institutional_pot"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make institutional mild steel pot for workshop cookery [craft_industrial_kitchen_appliances_0003_institutional_pot]",
		Category = "Kitchen and appliances",
		Blurb = "Make an institutional mild steel pot for workshop cookery from prepared mild steel stock.",
		Action = "making institutional mild steel pot for workshop cookery",
		ActiveCraftItemSdesc = "an in-progress institutional mild steel pot for workshop cookery craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 622.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("622 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrial_kitchen_appliances_0003_institutional_pot", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x an institutional mild steel pot for workshop cookery (@industrial_kitchen_appliances_0003_institutional_pot)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition KitchenAndAppliancesMakeInstitutionalCastIronGraterForWorkshopCookeryCraftIndustrialKitchenAppliances0004InstitutionalGrater() => new()
	{
		Metadata = new("Kitchen and appliances/make institutional cast iron grater for workshop cookery [craft_industrial_kitchen_appliances_0004_institutional_grater]", "crafts", ["industrial"], ["item:industrial_kitchen_appliances_0004_institutional_grater"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make institutional cast iron grater for workshop cookery [craft_industrial_kitchen_appliances_0004_institutional_grater]",
		Category = "Kitchen and appliances",
		Blurb = "Make an institutional cast iron grater for workshop cookery from prepared cast iron stock.",
		Action = "making institutional cast iron grater for workshop cookery",
		ActiveCraftItemSdesc = "an in-progress institutional cast iron grater for workshop cookery craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 662.0, Material: "cast iron", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("662 grams of cast iron", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrial_kitchen_appliances_0004_institutional_grater", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x an institutional cast iron grater for workshop cookery (@industrial_kitchen_appliances_0004_institutional_grater)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition KitchenAndAppliancesMakeInstitutionalCopperWhiskForWorkshopCookeryCraftIndustrialKitchenAppliances0005InstitutionalWhisk() => new()
	{
		Metadata = new("Kitchen and appliances/make institutional copper whisk for workshop cookery [craft_industrial_kitchen_appliances_0005_institutional_whisk]", "crafts", ["industrial"], ["item:industrial_kitchen_appliances_0005_institutional_whisk"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make institutional copper whisk for workshop cookery [craft_industrial_kitchen_appliances_0005_institutional_whisk]",
		Category = "Kitchen and appliances",
		Blurb = "Make an institutional copper whisk for workshop cookery from prepared copper stock.",
		Action = "making institutional copper whisk for workshop cookery",
		ActiveCraftItemSdesc = "an in-progress institutional copper whisk for workshop cookery craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 703.0, Material: "copper", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("703 grams of copper", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrial_kitchen_appliances_0005_institutional_whisk", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x an institutional copper whisk for workshop cookery (@industrial_kitchen_appliances_0005_institutional_whisk)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition KitchenAndAppliancesMakeInstitutionalCeramicLadleForWorkshopCookeryCraftIndustrialKitchenAppliances0006InstitutionalLadle() => new()
	{
		Metadata = new("Kitchen and appliances/make institutional ceramic ladle for workshop cookery [craft_industrial_kitchen_appliances_0006_institutional_ladle]", "crafts", ["industrial"], ["item:industrial_kitchen_appliances_0006_institutional_ladle"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make institutional ceramic ladle for workshop cookery [craft_industrial_kitchen_appliances_0006_institutional_ladle]",
		Category = "Kitchen and appliances",
		Blurb = "Make an institutional ceramic ladle for workshop cookery from prepared ceramic stock.",
		Action = "making institutional ceramic ladle for workshop cookery",
		ActiveCraftItemSdesc = "an in-progress institutional ceramic ladle for workshop cookery craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 744.0, Material: "ceramic", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("744 grams of ceramic", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrial_kitchen_appliances_0006_institutional_ladle", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x an institutional ceramic ladle for workshop cookery (@industrial_kitchen_appliances_0006_institutional_ladle)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition KitchenAndAppliancesMakeInstitutionalMildSteelStoveForWorkshopCookeryCraftIndustrialKitchenAppliances0007InstitutionalStove() => new()
	{
		Metadata = new("Kitchen and appliances/make institutional mild steel stove for workshop cookery [craft_industrial_kitchen_appliances_0007_institutional_stove]", "crafts", ["industrial"], ["item:industrial_kitchen_appliances_0007_institutional_stove"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make institutional mild steel stove for workshop cookery [craft_industrial_kitchen_appliances_0007_institutional_stove]",
		Category = "Kitchen and appliances",
		Blurb = "Make an institutional mild steel stove for workshop cookery from prepared mild steel stock.",
		Action = "making institutional mild steel stove for workshop cookery",
		ActiveCraftItemSdesc = "an in-progress institutional mild steel stove for workshop cookery craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 784.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("784 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrial_kitchen_appliances_0007_institutional_stove", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x an institutional mild steel stove for workshop cookery (@industrial_kitchen_appliances_0007_institutional_stove)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition KitchenAndAppliancesMakeInstitutionalCastIronToasterForWorkshopCookeryCraftIndustrialKitchenAppliances0008InstitutionalToaster() => new()
	{
		Metadata = new("Kitchen and appliances/make institutional cast iron toaster for workshop cookery [craft_industrial_kitchen_appliances_0008_institutional_toaster]", "crafts", ["industrial"], ["item:industrial_kitchen_appliances_0008_institutional_toaster"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make institutional cast iron toaster for workshop cookery [craft_industrial_kitchen_appliances_0008_institutional_toaster]",
		Category = "Kitchen and appliances",
		Blurb = "Make an institutional cast iron toaster for workshop cookery from prepared cast iron stock.",
		Action = "making institutional cast iron toaster for workshop cookery",
		ActiveCraftItemSdesc = "an in-progress institutional cast iron toaster for workshop cookery craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 825.0, Material: "cast iron", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("825 grams of cast iron", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrial_kitchen_appliances_0008_institutional_toaster", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x an institutional cast iron toaster for workshop cookery (@industrial_kitchen_appliances_0008_institutional_toaster)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition KitchenAndAppliancesMakeInstitutionalCopperMixerForWorkshopCookeryCraftIndustrialKitchenAppliances0009InstitutionalMixer() => new()
	{
		Metadata = new("Kitchen and appliances/make institutional copper mixer for workshop cookery [craft_industrial_kitchen_appliances_0009_institutional_mixer]", "crafts", ["industrial"], ["item:industrial_kitchen_appliances_0009_institutional_mixer"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make institutional copper mixer for workshop cookery [craft_industrial_kitchen_appliances_0009_institutional_mixer]",
		Category = "Kitchen and appliances",
		Blurb = "Make an institutional copper mixer for workshop cookery from prepared copper stock.",
		Action = "making institutional copper mixer for workshop cookery",
		ActiveCraftItemSdesc = "an in-progress institutional copper mixer for workshop cookery craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 866.0, Material: "copper", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("866 grams of copper", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrial_kitchen_appliances_0009_institutional_mixer", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x an institutional copper mixer for workshop cookery (@industrial_kitchen_appliances_0009_institutional_mixer)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodDrinkAndPackagedConsumablesMakeSomePlainPaperCrumbsForDomesticProvisioningCraftIndustrialisedFoodDrinkPackaged0001PlainCrumbs() => new()
	{
		Metadata = new("Food, drink and packaged consumables/make some plain paper crumbs for domestic provisioning [craft_industrialised_food_drink_packaged_0001_plain_crumbs]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_food_drink_packaged_0001_plain_crumbs"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make some plain paper crumbs for domestic provisioning [craft_industrialised_food_drink_packaged_0001_plain_crumbs]",
		Category = "Food, drink and packaged consumables",
		Blurb = "Make some plain paper crumbs for domestic provisioning from prepared paper stock.",
		Action = "making some plain paper crumbs for domestic provisioning",
		ActiveCraftItemSdesc = "an in-progress some plain paper crumbs for domestic provisioning craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 651.0, Material: "paper", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("651 grams of paper", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_food_drink_packaged_0001_plain_crumbs", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x some plain paper crumbs for domestic provisioning (@industrialised_food_drink_packaged_0001_plain_crumbs)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodDrinkAndPackagedConsumablesMakePlainCottonBiscuitForDomesticProvisioningCraftIndustrialisedFoodDrinkPackaged0002PlainBiscuit() => new()
	{
		Metadata = new("Food, drink and packaged consumables/make plain cotton biscuit for domestic provisioning [craft_industrialised_food_drink_packaged_0002_plain_biscuit]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_food_drink_packaged_0002_plain_biscuit"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make plain cotton biscuit for domestic provisioning [craft_industrialised_food_drink_packaged_0002_plain_biscuit]",
		Category = "Food, drink and packaged consumables",
		Blurb = "Make a plain cotton biscuit for domestic provisioning from prepared cotton stock.",
		Action = "making plain cotton biscuit for domestic provisioning",
		ActiveCraftItemSdesc = "an in-progress plain cotton biscuit for domestic provisioning craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 692.0, Material: "cotton", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("692 grams of cotton", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_food_drink_packaged_0002_plain_biscuit", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a plain cotton biscuit for domestic provisioning (@industrialised_food_drink_packaged_0002_plain_biscuit)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodDrinkAndPackagedConsumablesMakePlainCeramicRationForDomesticProvisioningCraftIndustrialisedFoodDrinkPackaged0003PlainRation() => new()
	{
		Metadata = new("Food, drink and packaged consumables/make plain ceramic ration for domestic provisioning [craft_industrialised_food_drink_packaged_0003_plain_ration]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_food_drink_packaged_0003_plain_ration"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make plain ceramic ration for domestic provisioning [craft_industrialised_food_drink_packaged_0003_plain_ration]",
		Category = "Food, drink and packaged consumables",
		Blurb = "Make a plain ceramic ration for domestic provisioning from prepared ceramic stock.",
		Action = "making plain ceramic ration for domestic provisioning",
		ActiveCraftItemSdesc = "an in-progress plain ceramic ration for domestic provisioning craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 733.0, Material: "ceramic", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("733 grams of ceramic", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_food_drink_packaged_0003_plain_ration", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a plain ceramic ration for domestic provisioning (@industrialised_food_drink_packaged_0003_plain_ration)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodDrinkAndPackagedConsumablesMakePlainPaperPreserveForDomesticProvisioningCraftIndustrialisedFoodDrinkPackaged0004PlainPreserve() => new()
	{
		Metadata = new("Food, drink and packaged consumables/make plain paper preserve for domestic provisioning [craft_industrialised_food_drink_packaged_0004_plain_preserve]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_food_drink_packaged_0004_plain_preserve"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make plain paper preserve for domestic provisioning [craft_industrialised_food_drink_packaged_0004_plain_preserve]",
		Category = "Food, drink and packaged consumables",
		Blurb = "Make a plain paper preserve for domestic provisioning from prepared paper stock.",
		Action = "making plain paper preserve for domestic provisioning",
		ActiveCraftItemSdesc = "an in-progress plain paper preserve for domestic provisioning craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 773.0, Material: "paper", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("773 grams of paper", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_food_drink_packaged_0004_plain_preserve", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a plain paper preserve for domestic provisioning (@industrialised_food_drink_packaged_0004_plain_preserve)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodDrinkAndPackagedConsumablesMakePlainCottonLoafForDomesticProvisioningCraftIndustrialisedFoodDrinkPackaged0005PlainLoaf() => new()
	{
		Metadata = new("Food, drink and packaged consumables/make plain cotton loaf for domestic provisioning [craft_industrialised_food_drink_packaged_0005_plain_loaf]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_food_drink_packaged_0005_plain_loaf"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make plain cotton loaf for domestic provisioning [craft_industrialised_food_drink_packaged_0005_plain_loaf]",
		Category = "Food, drink and packaged consumables",
		Blurb = "Make a plain cotton loaf for domestic provisioning from prepared cotton stock.",
		Action = "making plain cotton loaf for domestic provisioning",
		ActiveCraftItemSdesc = "an in-progress plain cotton loaf for domestic provisioning craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 814.0, Material: "cotton", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("814 grams of cotton", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_food_drink_packaged_0005_plain_loaf", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a plain cotton loaf for domestic provisioning (@industrialised_food_drink_packaged_0005_plain_loaf)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodDrinkAndPackagedConsumablesMakePlainCeramicCakeForDomesticProvisioningCraftIndustrialisedFoodDrinkPackaged0006PlainCake() => new()
	{
		Metadata = new("Food, drink and packaged consumables/make plain ceramic cake for domestic provisioning [craft_industrialised_food_drink_packaged_0006_plain_cake]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_food_drink_packaged_0006_plain_cake"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make plain ceramic cake for domestic provisioning [craft_industrialised_food_drink_packaged_0006_plain_cake]",
		Category = "Food, drink and packaged consumables",
		Blurb = "Make a plain ceramic cake for domestic provisioning from prepared ceramic stock.",
		Action = "making plain ceramic cake for domestic provisioning",
		ActiveCraftItemSdesc = "an in-progress plain ceramic cake for domestic provisioning craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 855.0, Material: "ceramic", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("855 grams of ceramic", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_food_drink_packaged_0006_plain_cake", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a plain ceramic cake for domestic provisioning (@industrialised_food_drink_packaged_0006_plain_cake)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodDrinkAndPackagedConsumablesMakePlainPaperPuddingForDomesticProvisioningCraftIndustrialisedFoodDrinkPackaged0007PlainPudding() => new()
	{
		Metadata = new("Food, drink and packaged consumables/make plain paper pudding for domestic provisioning [craft_industrialised_food_drink_packaged_0007_plain_pudding]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_food_drink_packaged_0007_plain_pudding"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make plain paper pudding for domestic provisioning [craft_industrialised_food_drink_packaged_0007_plain_pudding]",
		Category = "Food, drink and packaged consumables",
		Blurb = "Make a plain paper pudding for domestic provisioning from prepared paper stock.",
		Action = "making plain paper pudding for domestic provisioning",
		ActiveCraftItemSdesc = "an in-progress plain paper pudding for domestic provisioning craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 895.0, Material: "paper", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("895 grams of paper", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_food_drink_packaged_0007_plain_pudding", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a plain paper pudding for domestic provisioning (@industrialised_food_drink_packaged_0007_plain_pudding)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodDrinkAndPackagedConsumablesMakePlainCottonSausageForDomesticProvisioningCraftIndustrialisedFoodDrinkPackaged0008PlainSausage() => new()
	{
		Metadata = new("Food, drink and packaged consumables/make plain cotton sausage for domestic provisioning [craft_industrialised_food_drink_packaged_0008_plain_sausage]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_food_drink_packaged_0008_plain_sausage"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make plain cotton sausage for domestic provisioning [craft_industrialised_food_drink_packaged_0008_plain_sausage]",
		Category = "Food, drink and packaged consumables",
		Blurb = "Make a plain cotton sausage for domestic provisioning from prepared cotton stock.",
		Action = "making plain cotton sausage for domestic provisioning",
		ActiveCraftItemSdesc = "an in-progress plain cotton sausage for domestic provisioning craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 936.0, Material: "cotton", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("936 grams of cotton", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_food_drink_packaged_0008_plain_sausage", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a plain cotton sausage for domestic provisioning (@industrialised_food_drink_packaged_0008_plain_sausage)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodDrinkAndPackagedConsumablesMakePlainCeramicPickleForDomesticProvisioningCraftIndustrialisedFoodDrinkPackaged0009PlainPickle() => new()
	{
		Metadata = new("Food, drink and packaged consumables/make plain ceramic pickle for domestic provisioning [craft_industrialised_food_drink_packaged_0009_plain_pickle]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_food_drink_packaged_0009_plain_pickle"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make plain ceramic pickle for domestic provisioning [craft_industrialised_food_drink_packaged_0009_plain_pickle]",
		Category = "Food, drink and packaged consumables",
		Blurb = "Make a plain ceramic pickle for domestic provisioning from prepared ceramic stock.",
		Action = "making plain ceramic pickle for domestic provisioning",
		ActiveCraftItemSdesc = "an in-progress plain ceramic pickle for domestic provisioning craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 977.0, Material: "ceramic", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("977 grams of ceramic", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_food_drink_packaged_0009_plain_pickle", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a plain ceramic pickle for domestic provisioning (@industrialised_food_drink_packaged_0009_plain_pickle)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodDrinkAndPackagedConsumablesMakePlainPaperSweetForDomesticProvisioningCraftIndustrialisedFoodDrinkPackaged0010PlainSweet() => new()
	{
		Metadata = new("Food, drink and packaged consumables/make plain paper sweet for domestic provisioning [craft_industrialised_food_drink_packaged_0010_plain_sweet]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_food_drink_packaged_0010_plain_sweet"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make plain paper sweet for domestic provisioning [craft_industrialised_food_drink_packaged_0010_plain_sweet]",
		Category = "Food, drink and packaged consumables",
		Blurb = "Make a plain paper sweet for domestic provisioning from prepared paper stock.",
		Action = "making plain paper sweet for domestic provisioning",
		ActiveCraftItemSdesc = "an in-progress plain paper sweet for domestic provisioning craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1018.0, Material: "paper", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1018 grams of paper", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_food_drink_packaged_0010_plain_sweet", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a plain paper sweet for domestic provisioning (@industrialised_food_drink_packaged_0010_plain_sweet)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodDrinkAndPackagedConsumablesMakePlainCottonCrackerForDomesticProvisioningCraftIndustrialisedFoodDrinkPackaged0011PlainCracker() => new()
	{
		Metadata = new("Food, drink and packaged consumables/make plain cotton cracker for domestic provisioning [craft_industrialised_food_drink_packaged_0011_plain_cracker]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_food_drink_packaged_0011_plain_cracker"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make plain cotton cracker for domestic provisioning [craft_industrialised_food_drink_packaged_0011_plain_cracker]",
		Category = "Food, drink and packaged consumables",
		Blurb = "Make a plain cotton cracker for domestic provisioning from prepared cotton stock.",
		Action = "making plain cotton cracker for domestic provisioning",
		ActiveCraftItemSdesc = "an in-progress plain cotton cracker for domestic provisioning craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1058.0, Material: "cotton", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1058 grams of cotton", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_food_drink_packaged_0011_plain_cracker", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a plain cotton cracker for domestic provisioning (@industrialised_food_drink_packaged_0011_plain_cracker)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodDrinkAndPackagedConsumablesMakePlainCeramicPacketForDomesticProvisioningCraftIndustrialisedFoodDrinkPackaged0012PlainPacket() => new()
	{
		Metadata = new("Food, drink and packaged consumables/make plain ceramic packet for domestic provisioning [craft_industrialised_food_drink_packaged_0012_plain_packet]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_food_drink_packaged_0012_plain_packet"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make plain ceramic packet for domestic provisioning [craft_industrialised_food_drink_packaged_0012_plain_packet]",
		Category = "Food, drink and packaged consumables",
		Blurb = "Make a plain ceramic packet for domestic provisioning from prepared ceramic stock.",
		Action = "making plain ceramic packet for domestic provisioning",
		ActiveCraftItemSdesc = "an in-progress plain ceramic packet for domestic provisioning craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1099.0, Material: "ceramic", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1099 grams of ceramic", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_food_drink_packaged_0012_plain_packet", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a plain ceramic packet for domestic provisioning (@industrialised_food_drink_packaged_0012_plain_packet)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodDrinkAndPackagedConsumablesMakeSomeReinforcedPaperCrumbsForDomesticProvisioningCraftIndustrialisedFoodDrinkPackaged0013ReinforcedCrumbs() => new()
	{
		Metadata = new("Food, drink and packaged consumables/make some reinforced paper crumbs for domestic provisioning [craft_industrialised_food_drink_packaged_0013_reinforced_crumbs]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_food_drink_packaged_0013_reinforced_crumbs"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make some reinforced paper crumbs for domestic provisioning [craft_industrialised_food_drink_packaged_0013_reinforced_crumbs]",
		Category = "Food, drink and packaged consumables",
		Blurb = "Make some reinforced paper crumbs for domestic provisioning from prepared paper stock.",
		Action = "making some reinforced paper crumbs for domestic provisioning",
		ActiveCraftItemSdesc = "an in-progress some reinforced paper crumbs for domestic provisioning craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1140.0, Material: "paper", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1140 grams of paper", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_food_drink_packaged_0013_reinforced_crumbs", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x some reinforced paper crumbs for domestic provisioning (@industrialised_food_drink_packaged_0013_reinforced_crumbs)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodDrinkAndPackagedConsumablesMakeReinforcedCottonBiscuitForDomesticProvisioningCraftIndustrialisedFoodDrinkPackaged0014ReinforcedBiscuit() => new()
	{
		Metadata = new("Food, drink and packaged consumables/make reinforced cotton biscuit for domestic provisioning [craft_industrialised_food_drink_packaged_0014_reinforced_biscuit]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_food_drink_packaged_0014_reinforced_biscuit"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make reinforced cotton biscuit for domestic provisioning [craft_industrialised_food_drink_packaged_0014_reinforced_biscuit]",
		Category = "Food, drink and packaged consumables",
		Blurb = "Make a reinforced cotton biscuit for domestic provisioning from prepared cotton stock.",
		Action = "making reinforced cotton biscuit for domestic provisioning",
		ActiveCraftItemSdesc = "an in-progress reinforced cotton biscuit for domestic provisioning craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1180.0, Material: "cotton", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1180 grams of cotton", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_food_drink_packaged_0014_reinforced_biscuit", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a reinforced cotton biscuit for domestic provisioning (@industrialised_food_drink_packaged_0014_reinforced_biscuit)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodDrinkAndPackagedConsumablesMakeReinforcedCeramicRationForDomesticProvisioningCraftIndustrialisedFoodDrinkPackaged0015ReinforcedRation() => new()
	{
		Metadata = new("Food, drink and packaged consumables/make reinforced ceramic ration for domestic provisioning [craft_industrialised_food_drink_packaged_0015_reinforced_ration]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_food_drink_packaged_0015_reinforced_ration"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make reinforced ceramic ration for domestic provisioning [craft_industrialised_food_drink_packaged_0015_reinforced_ration]",
		Category = "Food, drink and packaged consumables",
		Blurb = "Make a reinforced ceramic ration for domestic provisioning from prepared ceramic stock.",
		Action = "making reinforced ceramic ration for domestic provisioning",
		ActiveCraftItemSdesc = "an in-progress reinforced ceramic ration for domestic provisioning craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1221.0, Material: "ceramic", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1221 grams of ceramic", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_food_drink_packaged_0015_reinforced_ration", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a reinforced ceramic ration for domestic provisioning (@industrialised_food_drink_packaged_0015_reinforced_ration)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodDrinkAndPackagedConsumablesMakeReinforcedPaperPreserveForDomesticProvisioningCraftIndustrialisedFoodDrinkPackaged0016ReinforcedPreserve() => new()
	{
		Metadata = new("Food, drink and packaged consumables/make reinforced paper preserve for domestic provisioning [craft_industrialised_food_drink_packaged_0016_reinforced_preserve]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_food_drink_packaged_0016_reinforced_preserve"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make reinforced paper preserve for domestic provisioning [craft_industrialised_food_drink_packaged_0016_reinforced_preserve]",
		Category = "Food, drink and packaged consumables",
		Blurb = "Make a reinforced paper preserve for domestic provisioning from prepared paper stock.",
		Action = "making reinforced paper preserve for domestic provisioning",
		ActiveCraftItemSdesc = "an in-progress reinforced paper preserve for domestic provisioning craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1262.0, Material: "paper", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1262 grams of paper", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_food_drink_packaged_0016_reinforced_preserve", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a reinforced paper preserve for domestic provisioning (@industrialised_food_drink_packaged_0016_reinforced_preserve)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodDrinkAndPackagedConsumablesMakeReinforcedCottonLoafForDomesticProvisioningCraftIndustrialisedFoodDrinkPackaged0017ReinforcedLoaf() => new()
	{
		Metadata = new("Food, drink and packaged consumables/make reinforced cotton loaf for domestic provisioning [craft_industrialised_food_drink_packaged_0017_reinforced_loaf]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_food_drink_packaged_0017_reinforced_loaf"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make reinforced cotton loaf for domestic provisioning [craft_industrialised_food_drink_packaged_0017_reinforced_loaf]",
		Category = "Food, drink and packaged consumables",
		Blurb = "Make a reinforced cotton loaf for domestic provisioning from prepared cotton stock.",
		Action = "making reinforced cotton loaf for domestic provisioning",
		ActiveCraftItemSdesc = "an in-progress reinforced cotton loaf for domestic provisioning craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1302.0, Material: "cotton", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1302 grams of cotton", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_food_drink_packaged_0017_reinforced_loaf", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a reinforced cotton loaf for domestic provisioning (@industrialised_food_drink_packaged_0017_reinforced_loaf)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodDrinkAndPackagedConsumablesMakeReinforcedCeramicCakeForDomesticProvisioningCraftIndustrialisedFoodDrinkPackaged0018ReinforcedCake() => new()
	{
		Metadata = new("Food, drink and packaged consumables/make reinforced ceramic cake for domestic provisioning [craft_industrialised_food_drink_packaged_0018_reinforced_cake]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_food_drink_packaged_0018_reinforced_cake"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make reinforced ceramic cake for domestic provisioning [craft_industrialised_food_drink_packaged_0018_reinforced_cake]",
		Category = "Food, drink and packaged consumables",
		Blurb = "Make a reinforced ceramic cake for domestic provisioning from prepared ceramic stock.",
		Action = "making reinforced ceramic cake for domestic provisioning",
		ActiveCraftItemSdesc = "an in-progress reinforced ceramic cake for domestic provisioning craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1343.0, Material: "ceramic", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1343 grams of ceramic", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_food_drink_packaged_0018_reinforced_cake", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a reinforced ceramic cake for domestic provisioning (@industrialised_food_drink_packaged_0018_reinforced_cake)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodDrinkAndPackagedConsumablesMakeReinforcedPaperPuddingForDomesticProvisioningCraftIndustrialisedFoodDrinkPackaged0019ReinforcedPudding() => new()
	{
		Metadata = new("Food, drink and packaged consumables/make reinforced paper pudding for domestic provisioning [craft_industrialised_food_drink_packaged_0019_reinforced_pudding]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_food_drink_packaged_0019_reinforced_pudding"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make reinforced paper pudding for domestic provisioning [craft_industrialised_food_drink_packaged_0019_reinforced_pudding]",
		Category = "Food, drink and packaged consumables",
		Blurb = "Make a reinforced paper pudding for domestic provisioning from prepared paper stock.",
		Action = "making reinforced paper pudding for domestic provisioning",
		ActiveCraftItemSdesc = "an in-progress reinforced paper pudding for domestic provisioning craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1384.0, Material: "paper", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1384 grams of paper", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_food_drink_packaged_0019_reinforced_pudding", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a reinforced paper pudding for domestic provisioning (@industrialised_food_drink_packaged_0019_reinforced_pudding)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodDrinkAndPackagedConsumablesMakeReinforcedCottonSausageForDomesticProvisioningCraftIndustrialisedFoodDrinkPackaged0020ReinforcedSausage() => new()
	{
		Metadata = new("Food, drink and packaged consumables/make reinforced cotton sausage for domestic provisioning [craft_industrialised_food_drink_packaged_0020_reinforced_sausage]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_food_drink_packaged_0020_reinforced_sausage"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make reinforced cotton sausage for domestic provisioning [craft_industrialised_food_drink_packaged_0020_reinforced_sausage]",
		Category = "Food, drink and packaged consumables",
		Blurb = "Make a reinforced cotton sausage for domestic provisioning from prepared cotton stock.",
		Action = "making reinforced cotton sausage for domestic provisioning",
		ActiveCraftItemSdesc = "an in-progress reinforced cotton sausage for domestic provisioning craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1425.0, Material: "cotton", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1425 grams of cotton", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_food_drink_packaged_0020_reinforced_sausage", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a reinforced cotton sausage for domestic provisioning (@industrialised_food_drink_packaged_0020_reinforced_sausage)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodDrinkAndPackagedConsumablesMakeReinforcedCeramicPickleForDomesticProvisioningCraftIndustrialisedFoodDrinkPackaged0021ReinforcedPickle() => new()
	{
		Metadata = new("Food, drink and packaged consumables/make reinforced ceramic pickle for domestic provisioning [craft_industrialised_food_drink_packaged_0021_reinforced_pickle]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_food_drink_packaged_0021_reinforced_pickle"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make reinforced ceramic pickle for domestic provisioning [craft_industrialised_food_drink_packaged_0021_reinforced_pickle]",
		Category = "Food, drink and packaged consumables",
		Blurb = "Make a reinforced ceramic pickle for domestic provisioning from prepared ceramic stock.",
		Action = "making reinforced ceramic pickle for domestic provisioning",
		ActiveCraftItemSdesc = "an in-progress reinforced ceramic pickle for domestic provisioning craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1465.0, Material: "ceramic", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1465 grams of ceramic", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_food_drink_packaged_0021_reinforced_pickle", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a reinforced ceramic pickle for domestic provisioning (@industrialised_food_drink_packaged_0021_reinforced_pickle)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodDrinkAndPackagedConsumablesMakeReinforcedPaperSweetForDomesticProvisioningCraftIndustrialisedFoodDrinkPackaged0022ReinforcedSweet() => new()
	{
		Metadata = new("Food, drink and packaged consumables/make reinforced paper sweet for domestic provisioning [craft_industrialised_food_drink_packaged_0022_reinforced_sweet]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_food_drink_packaged_0022_reinforced_sweet"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make reinforced paper sweet for domestic provisioning [craft_industrialised_food_drink_packaged_0022_reinforced_sweet]",
		Category = "Food, drink and packaged consumables",
		Blurb = "Make a reinforced paper sweet for domestic provisioning from prepared paper stock.",
		Action = "making reinforced paper sweet for domestic provisioning",
		ActiveCraftItemSdesc = "an in-progress reinforced paper sweet for domestic provisioning craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1506.0, Material: "paper", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1506 grams of paper", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_food_drink_packaged_0022_reinforced_sweet", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a reinforced paper sweet for domestic provisioning (@industrialised_food_drink_packaged_0022_reinforced_sweet)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodDrinkAndPackagedConsumablesMakeReinforcedCottonCrackerForDomesticProvisioningCraftIndustrialisedFoodDrinkPackaged0023ReinforcedCracker() => new()
	{
		Metadata = new("Food, drink and packaged consumables/make reinforced cotton cracker for domestic provisioning [craft_industrialised_food_drink_packaged_0023_reinforced_cracker]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_food_drink_packaged_0023_reinforced_cracker"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make reinforced cotton cracker for domestic provisioning [craft_industrialised_food_drink_packaged_0023_reinforced_cracker]",
		Category = "Food, drink and packaged consumables",
		Blurb = "Make a reinforced cotton cracker for domestic provisioning from prepared cotton stock.",
		Action = "making reinforced cotton cracker for domestic provisioning",
		ActiveCraftItemSdesc = "an in-progress reinforced cotton cracker for domestic provisioning craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1547.0, Material: "cotton", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1547 grams of cotton", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_food_drink_packaged_0023_reinforced_cracker", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a reinforced cotton cracker for domestic provisioning (@industrialised_food_drink_packaged_0023_reinforced_cracker)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodDrinkAndPackagedConsumablesMakeReinforcedCeramicPacketForDomesticProvisioningCraftIndustrialisedFoodDrinkPackaged0024ReinforcedPacket() => new()
	{
		Metadata = new("Food, drink and packaged consumables/make reinforced ceramic packet for domestic provisioning [craft_industrialised_food_drink_packaged_0024_reinforced_packet]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_food_drink_packaged_0024_reinforced_packet"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make reinforced ceramic packet for domestic provisioning [craft_industrialised_food_drink_packaged_0024_reinforced_packet]",
		Category = "Food, drink and packaged consumables",
		Blurb = "Make a reinforced ceramic packet for domestic provisioning from prepared ceramic stock.",
		Action = "making reinforced ceramic packet for domestic provisioning",
		ActiveCraftItemSdesc = "an in-progress reinforced ceramic packet for domestic provisioning craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1587.0, Material: "ceramic", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1587 grams of ceramic", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_food_drink_packaged_0024_reinforced_packet", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a reinforced ceramic packet for domestic provisioning (@industrialised_food_drink_packaged_0024_reinforced_packet)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodDrinkAndPackagedConsumablesMakeSomeCompactPaperCrumbsForDomesticProvisioningCraftIndustrialisedFoodDrinkPackaged0025CompactCrumbs() => new()
	{
		Metadata = new("Food, drink and packaged consumables/make some compact paper crumbs for domestic provisioning [craft_industrialised_food_drink_packaged_0025_compact_crumbs]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_food_drink_packaged_0025_compact_crumbs"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make some compact paper crumbs for domestic provisioning [craft_industrialised_food_drink_packaged_0025_compact_crumbs]",
		Category = "Food, drink and packaged consumables",
		Blurb = "Make some compact paper crumbs for domestic provisioning from prepared paper stock.",
		Action = "making some compact paper crumbs for domestic provisioning",
		ActiveCraftItemSdesc = "an in-progress some compact paper crumbs for domestic provisioning craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1628.0, Material: "paper", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1628 grams of paper", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_food_drink_packaged_0025_compact_crumbs", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x some compact paper crumbs for domestic provisioning (@industrialised_food_drink_packaged_0025_compact_crumbs)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodDrinkAndPackagedConsumablesMakeCompactCottonBiscuitForDomesticProvisioningCraftIndustrialisedFoodDrinkPackaged0026CompactBiscuit() => new()
	{
		Metadata = new("Food, drink and packaged consumables/make compact cotton biscuit for domestic provisioning [craft_industrialised_food_drink_packaged_0026_compact_biscuit]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_food_drink_packaged_0026_compact_biscuit"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make compact cotton biscuit for domestic provisioning [craft_industrialised_food_drink_packaged_0026_compact_biscuit]",
		Category = "Food, drink and packaged consumables",
		Blurb = "Make a compact cotton biscuit for domestic provisioning from prepared cotton stock.",
		Action = "making compact cotton biscuit for domestic provisioning",
		ActiveCraftItemSdesc = "an in-progress compact cotton biscuit for domestic provisioning craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1669.0, Material: "cotton", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1669 grams of cotton", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_food_drink_packaged_0026_compact_biscuit", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a compact cotton biscuit for domestic provisioning (@industrialised_food_drink_packaged_0026_compact_biscuit)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodDrinkAndPackagedConsumablesMakeCompactCeramicRationForDomesticProvisioningCraftIndustrialisedFoodDrinkPackaged0027CompactRation() => new()
	{
		Metadata = new("Food, drink and packaged consumables/make compact ceramic ration for domestic provisioning [craft_industrialised_food_drink_packaged_0027_compact_ration]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_food_drink_packaged_0027_compact_ration"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make compact ceramic ration for domestic provisioning [craft_industrialised_food_drink_packaged_0027_compact_ration]",
		Category = "Food, drink and packaged consumables",
		Blurb = "Make a compact ceramic ration for domestic provisioning from prepared ceramic stock.",
		Action = "making compact ceramic ration for domestic provisioning",
		ActiveCraftItemSdesc = "an in-progress compact ceramic ration for domestic provisioning craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1709.0, Material: "ceramic", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1709 grams of ceramic", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_food_drink_packaged_0027_compact_ration", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a compact ceramic ration for domestic provisioning (@industrialised_food_drink_packaged_0027_compact_ration)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodDrinkAndPackagedConsumablesMakeCompactPaperPreserveForDomesticProvisioningCraftIndustrialisedFoodDrinkPackaged0028CompactPreserve() => new()
	{
		Metadata = new("Food, drink and packaged consumables/make compact paper preserve for domestic provisioning [craft_industrialised_food_drink_packaged_0028_compact_preserve]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_food_drink_packaged_0028_compact_preserve"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make compact paper preserve for domestic provisioning [craft_industrialised_food_drink_packaged_0028_compact_preserve]",
		Category = "Food, drink and packaged consumables",
		Blurb = "Make a compact paper preserve for domestic provisioning from prepared paper stock.",
		Action = "making compact paper preserve for domestic provisioning",
		ActiveCraftItemSdesc = "an in-progress compact paper preserve for domestic provisioning craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1750.0, Material: "paper", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1750 grams of paper", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_food_drink_packaged_0028_compact_preserve", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a compact paper preserve for domestic provisioning (@industrialised_food_drink_packaged_0028_compact_preserve)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodDrinkAndPackagedConsumablesMakeCompactCottonLoafForDomesticProvisioningCraftIndustrialisedFoodDrinkPackaged0029CompactLoaf() => new()
	{
		Metadata = new("Food, drink and packaged consumables/make compact cotton loaf for domestic provisioning [craft_industrialised_food_drink_packaged_0029_compact_loaf]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_food_drink_packaged_0029_compact_loaf"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make compact cotton loaf for domestic provisioning [craft_industrialised_food_drink_packaged_0029_compact_loaf]",
		Category = "Food, drink and packaged consumables",
		Blurb = "Make a compact cotton loaf for domestic provisioning from prepared cotton stock.",
		Action = "making compact cotton loaf for domestic provisioning",
		ActiveCraftItemSdesc = "an in-progress compact cotton loaf for domestic provisioning craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1791.0, Material: "cotton", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1791 grams of cotton", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_food_drink_packaged_0029_compact_loaf", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a compact cotton loaf for domestic provisioning (@industrialised_food_drink_packaged_0029_compact_loaf)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodDrinkAndPackagedConsumablesMakeCompactCeramicCakeForDomesticProvisioningCraftIndustrialisedFoodDrinkPackaged0030CompactCake() => new()
	{
		Metadata = new("Food, drink and packaged consumables/make compact ceramic cake for domestic provisioning [craft_industrialised_food_drink_packaged_0030_compact_cake]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_food_drink_packaged_0030_compact_cake"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make compact ceramic cake for domestic provisioning [craft_industrialised_food_drink_packaged_0030_compact_cake]",
		Category = "Food, drink and packaged consumables",
		Blurb = "Make a compact ceramic cake for domestic provisioning from prepared ceramic stock.",
		Action = "making compact ceramic cake for domestic provisioning",
		ActiveCraftItemSdesc = "an in-progress compact ceramic cake for domestic provisioning craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1832.0, Material: "ceramic", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1832 grams of ceramic", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_food_drink_packaged_0030_compact_cake", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a compact ceramic cake for domestic provisioning (@industrialised_food_drink_packaged_0030_compact_cake)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodDrinkAndPackagedConsumablesMakeCompactPaperPuddingForDomesticProvisioningCraftIndustrialisedFoodDrinkPackaged0031CompactPudding() => new()
	{
		Metadata = new("Food, drink and packaged consumables/make compact paper pudding for domestic provisioning [craft_industrialised_food_drink_packaged_0031_compact_pudding]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_food_drink_packaged_0031_compact_pudding"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make compact paper pudding for domestic provisioning [craft_industrialised_food_drink_packaged_0031_compact_pudding]",
		Category = "Food, drink and packaged consumables",
		Blurb = "Make a compact paper pudding for domestic provisioning from prepared paper stock.",
		Action = "making compact paper pudding for domestic provisioning",
		ActiveCraftItemSdesc = "an in-progress compact paper pudding for domestic provisioning craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1872.0, Material: "paper", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1872 grams of paper", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_food_drink_packaged_0031_compact_pudding", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a compact paper pudding for domestic provisioning (@industrialised_food_drink_packaged_0031_compact_pudding)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodDrinkAndPackagedConsumablesMakeCompactCottonSausageForDomesticProvisioningCraftIndustrialisedFoodDrinkPackaged0032CompactSausage() => new()
	{
		Metadata = new("Food, drink and packaged consumables/make compact cotton sausage for domestic provisioning [craft_industrialised_food_drink_packaged_0032_compact_sausage]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_food_drink_packaged_0032_compact_sausage"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make compact cotton sausage for domestic provisioning [craft_industrialised_food_drink_packaged_0032_compact_sausage]",
		Category = "Food, drink and packaged consumables",
		Blurb = "Make a compact cotton sausage for domestic provisioning from prepared cotton stock.",
		Action = "making compact cotton sausage for domestic provisioning",
		ActiveCraftItemSdesc = "an in-progress compact cotton sausage for domestic provisioning craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1913.0, Material: "cotton", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1913 grams of cotton", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_food_drink_packaged_0032_compact_sausage", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a compact cotton sausage for domestic provisioning (@industrialised_food_drink_packaged_0032_compact_sausage)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodDrinkAndPackagedConsumablesMakeCompactCeramicPickleForDomesticProvisioningCraftIndustrialisedFoodDrinkPackaged0033CompactPickle() => new()
	{
		Metadata = new("Food, drink and packaged consumables/make compact ceramic pickle for domestic provisioning [craft_industrialised_food_drink_packaged_0033_compact_pickle]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_food_drink_packaged_0033_compact_pickle"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make compact ceramic pickle for domestic provisioning [craft_industrialised_food_drink_packaged_0033_compact_pickle]",
		Category = "Food, drink and packaged consumables",
		Blurb = "Make a compact ceramic pickle for domestic provisioning from prepared ceramic stock.",
		Action = "making compact ceramic pickle for domestic provisioning",
		ActiveCraftItemSdesc = "an in-progress compact ceramic pickle for domestic provisioning craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1954.0, Material: "ceramic", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1954 grams of ceramic", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_food_drink_packaged_0033_compact_pickle", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a compact ceramic pickle for domestic provisioning (@industrialised_food_drink_packaged_0033_compact_pickle)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodDrinkAndPackagedConsumablesMakeCompactPaperSweetForDomesticProvisioningCraftIndustrialisedFoodDrinkPackaged0034CompactSweet() => new()
	{
		Metadata = new("Food, drink and packaged consumables/make compact paper sweet for domestic provisioning [craft_industrialised_food_drink_packaged_0034_compact_sweet]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_food_drink_packaged_0034_compact_sweet"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make compact paper sweet for domestic provisioning [craft_industrialised_food_drink_packaged_0034_compact_sweet]",
		Category = "Food, drink and packaged consumables",
		Blurb = "Make a compact paper sweet for domestic provisioning from prepared paper stock.",
		Action = "making compact paper sweet for domestic provisioning",
		ActiveCraftItemSdesc = "an in-progress compact paper sweet for domestic provisioning craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1994.0, Material: "paper", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1994 grams of paper", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_food_drink_packaged_0034_compact_sweet", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a compact paper sweet for domestic provisioning (@industrialised_food_drink_packaged_0034_compact_sweet)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodDrinkAndPackagedConsumablesMakeCompactCottonCrackerForDomesticProvisioningCraftIndustrialisedFoodDrinkPackaged0035CompactCracker() => new()
	{
		Metadata = new("Food, drink and packaged consumables/make compact cotton cracker for domestic provisioning [craft_industrialised_food_drink_packaged_0035_compact_cracker]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_food_drink_packaged_0035_compact_cracker"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make compact cotton cracker for domestic provisioning [craft_industrialised_food_drink_packaged_0035_compact_cracker]",
		Category = "Food, drink and packaged consumables",
		Blurb = "Make a compact cotton cracker for domestic provisioning from prepared cotton stock.",
		Action = "making compact cotton cracker for domestic provisioning",
		ActiveCraftItemSdesc = "an in-progress compact cotton cracker for domestic provisioning craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2035.0, Material: "cotton", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2035 grams of cotton", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_food_drink_packaged_0035_compact_cracker", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a compact cotton cracker for domestic provisioning (@industrialised_food_drink_packaged_0035_compact_cracker)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodDrinkAndPackagedConsumablesMakeCompactCeramicPacketForDomesticProvisioningCraftIndustrialisedFoodDrinkPackaged0036CompactPacket() => new()
	{
		Metadata = new("Food, drink and packaged consumables/make compact ceramic packet for domestic provisioning [craft_industrialised_food_drink_packaged_0036_compact_packet]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_food_drink_packaged_0036_compact_packet"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make compact ceramic packet for domestic provisioning [craft_industrialised_food_drink_packaged_0036_compact_packet]",
		Category = "Food, drink and packaged consumables",
		Blurb = "Make a compact ceramic packet for domestic provisioning from prepared ceramic stock.",
		Action = "making compact ceramic packet for domestic provisioning",
		ActiveCraftItemSdesc = "an in-progress compact ceramic packet for domestic provisioning craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2076.0, Material: "ceramic", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2076 grams of ceramic", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_food_drink_packaged_0036_compact_packet", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a compact ceramic packet for domestic provisioning (@industrialised_food_drink_packaged_0036_compact_packet)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodDrinkAndPackagedConsumablesMakeSomeHeavyDutyPaperCrumbsForDomesticProvisioningCraftIndustrialisedFoodDrinkPackaged0037HeavyDutyCrumbs() => new()
	{
		Metadata = new("Food, drink and packaged consumables/make some heavy-duty paper crumbs for domestic provisioning [craft_industrialised_food_drink_packaged_0037_heavy_duty_crumbs]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_food_drink_packaged_0037_heavy_duty_crumbs"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make some heavy-duty paper crumbs for domestic provisioning [craft_industrialised_food_drink_packaged_0037_heavy_duty_crumbs]",
		Category = "Food, drink and packaged consumables",
		Blurb = "Make some heavy-duty paper crumbs for domestic provisioning from prepared paper stock.",
		Action = "making some heavy-duty paper crumbs for domestic provisioning",
		ActiveCraftItemSdesc = "an in-progress some heavy-duty paper crumbs for domestic provisioning craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2116.0, Material: "paper", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2116 grams of paper", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_food_drink_packaged_0037_heavy_duty_crumbs", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x some heavy-duty paper crumbs for domestic provisioning (@industrialised_food_drink_packaged_0037_heavy_duty_crumbs)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodDrinkAndPackagedConsumablesMakeHeavyDutyCottonBiscuitForDomesticProvisioningCraftIndustrialisedFoodDrinkPackaged0038HeavyDutyBiscuit() => new()
	{
		Metadata = new("Food, drink and packaged consumables/make heavy-duty cotton biscuit for domestic provisioning [craft_industrialised_food_drink_packaged_0038_heavy_duty_biscuit]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_food_drink_packaged_0038_heavy_duty_biscuit"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make heavy-duty cotton biscuit for domestic provisioning [craft_industrialised_food_drink_packaged_0038_heavy_duty_biscuit]",
		Category = "Food, drink and packaged consumables",
		Blurb = "Make a heavy-duty cotton biscuit for domestic provisioning from prepared cotton stock.",
		Action = "making heavy-duty cotton biscuit for domestic provisioning",
		ActiveCraftItemSdesc = "an in-progress heavy-duty cotton biscuit for domestic provisioning craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2157.0, Material: "cotton", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2157 grams of cotton", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_food_drink_packaged_0038_heavy_duty_biscuit", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a heavy-duty cotton biscuit for domestic provisioning (@industrialised_food_drink_packaged_0038_heavy_duty_biscuit)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodDrinkAndPackagedConsumablesMakeHeavyDutyCeramicRationForDomesticProvisioningCraftIndustrialisedFoodDrinkPackaged0039HeavyDutyRation() => new()
	{
		Metadata = new("Food, drink and packaged consumables/make heavy-duty ceramic ration for domestic provisioning [craft_industrialised_food_drink_packaged_0039_heavy_duty_ration]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_food_drink_packaged_0039_heavy_duty_ration"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make heavy-duty ceramic ration for domestic provisioning [craft_industrialised_food_drink_packaged_0039_heavy_duty_ration]",
		Category = "Food, drink and packaged consumables",
		Blurb = "Make a heavy-duty ceramic ration for domestic provisioning from prepared ceramic stock.",
		Action = "making heavy-duty ceramic ration for domestic provisioning",
		ActiveCraftItemSdesc = "an in-progress heavy-duty ceramic ration for domestic provisioning craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2198.0, Material: "ceramic", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2198 grams of ceramic", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_food_drink_packaged_0039_heavy_duty_ration", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a heavy-duty ceramic ration for domestic provisioning (@industrialised_food_drink_packaged_0039_heavy_duty_ration)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodDrinkAndPackagedConsumablesMakeHeavyDutyPaperPreserveForDomesticProvisioningCraftIndustrialisedFoodDrinkPackaged0040HeavyDutyPreserve() => new()
	{
		Metadata = new("Food, drink and packaged consumables/make heavy-duty paper preserve for domestic provisioning [craft_industrialised_food_drink_packaged_0040_heavy_duty_preserve]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_food_drink_packaged_0040_heavy_duty_preserve"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make heavy-duty paper preserve for domestic provisioning [craft_industrialised_food_drink_packaged_0040_heavy_duty_preserve]",
		Category = "Food, drink and packaged consumables",
		Blurb = "Make a heavy-duty paper preserve for domestic provisioning from prepared paper stock.",
		Action = "making heavy-duty paper preserve for domestic provisioning",
		ActiveCraftItemSdesc = "an in-progress heavy-duty paper preserve for domestic provisioning craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2238.0, Material: "paper", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2238 grams of paper", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_food_drink_packaged_0040_heavy_duty_preserve", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a heavy-duty paper preserve for domestic provisioning (@industrialised_food_drink_packaged_0040_heavy_duty_preserve)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodDrinkAndPackagedConsumablesMakeHeavyDutyCottonLoafForDomesticProvisioningCraftIndustrialisedFoodDrinkPackaged0041HeavyDutyLoaf() => new()
	{
		Metadata = new("Food, drink and packaged consumables/make heavy-duty cotton loaf for domestic provisioning [craft_industrialised_food_drink_packaged_0041_heavy_duty_loaf]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_food_drink_packaged_0041_heavy_duty_loaf"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make heavy-duty cotton loaf for domestic provisioning [craft_industrialised_food_drink_packaged_0041_heavy_duty_loaf]",
		Category = "Food, drink and packaged consumables",
		Blurb = "Make a heavy-duty cotton loaf for domestic provisioning from prepared cotton stock.",
		Action = "making heavy-duty cotton loaf for domestic provisioning",
		ActiveCraftItemSdesc = "an in-progress heavy-duty cotton loaf for domestic provisioning craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2279.0, Material: "cotton", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2279 grams of cotton", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_food_drink_packaged_0041_heavy_duty_loaf", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a heavy-duty cotton loaf for domestic provisioning (@industrialised_food_drink_packaged_0041_heavy_duty_loaf)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodDrinkAndPackagedConsumablesMakeHeavyDutyCeramicCakeForDomesticProvisioningCraftIndustrialisedFoodDrinkPackaged0042HeavyDutyCake() => new()
	{
		Metadata = new("Food, drink and packaged consumables/make heavy-duty ceramic cake for domestic provisioning [craft_industrialised_food_drink_packaged_0042_heavy_duty_cake]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_food_drink_packaged_0042_heavy_duty_cake"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make heavy-duty ceramic cake for domestic provisioning [craft_industrialised_food_drink_packaged_0042_heavy_duty_cake]",
		Category = "Food, drink and packaged consumables",
		Blurb = "Make a heavy-duty ceramic cake for domestic provisioning from prepared ceramic stock.",
		Action = "making heavy-duty ceramic cake for domestic provisioning",
		ActiveCraftItemSdesc = "an in-progress heavy-duty ceramic cake for domestic provisioning craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2320.0, Material: "ceramic", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2320 grams of ceramic", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_food_drink_packaged_0042_heavy_duty_cake", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a heavy-duty ceramic cake for domestic provisioning (@industrialised_food_drink_packaged_0042_heavy_duty_cake)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodDrinkAndPackagedConsumablesMakeHeavyDutyPaperPuddingForDomesticProvisioningCraftIndustrialisedFoodDrinkPackaged0043HeavyDutyPudding() => new()
	{
		Metadata = new("Food, drink and packaged consumables/make heavy-duty paper pudding for domestic provisioning [craft_industrialised_food_drink_packaged_0043_heavy_duty_pudding]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_food_drink_packaged_0043_heavy_duty_pudding"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make heavy-duty paper pudding for domestic provisioning [craft_industrialised_food_drink_packaged_0043_heavy_duty_pudding]",
		Category = "Food, drink and packaged consumables",
		Blurb = "Make a heavy-duty paper pudding for domestic provisioning from prepared paper stock.",
		Action = "making heavy-duty paper pudding for domestic provisioning",
		ActiveCraftItemSdesc = "an in-progress heavy-duty paper pudding for domestic provisioning craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2361.0, Material: "paper", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2361 grams of paper", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_food_drink_packaged_0043_heavy_duty_pudding", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a heavy-duty paper pudding for domestic provisioning (@industrialised_food_drink_packaged_0043_heavy_duty_pudding)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodDrinkAndPackagedConsumablesMakeHeavyDutyCottonSausageForDomesticProvisioningCraftIndustrialisedFoodDrinkPackaged0044HeavyDutySausage() => new()
	{
		Metadata = new("Food, drink and packaged consumables/make heavy-duty cotton sausage for domestic provisioning [craft_industrialised_food_drink_packaged_0044_heavy_duty_sausage]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_food_drink_packaged_0044_heavy_duty_sausage"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make heavy-duty cotton sausage for domestic provisioning [craft_industrialised_food_drink_packaged_0044_heavy_duty_sausage]",
		Category = "Food, drink and packaged consumables",
		Blurb = "Make a heavy-duty cotton sausage for domestic provisioning from prepared cotton stock.",
		Action = "making heavy-duty cotton sausage for domestic provisioning",
		ActiveCraftItemSdesc = "an in-progress heavy-duty cotton sausage for domestic provisioning craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2401.0, Material: "cotton", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2401 grams of cotton", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_food_drink_packaged_0044_heavy_duty_sausage", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a heavy-duty cotton sausage for domestic provisioning (@industrialised_food_drink_packaged_0044_heavy_duty_sausage)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition FoodDrinkAndPackagedConsumablesMakeHeavyDutyCeramicPickleForDomesticProvisioningCraftIndustrialisedFoodDrinkPackaged0045HeavyDutyPickle() => new()
	{
		Metadata = new("Food, drink and packaged consumables/make heavy-duty ceramic pickle for domestic provisioning [craft_industrialised_food_drink_packaged_0045_heavy_duty_pickle]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_food_drink_packaged_0045_heavy_duty_pickle"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make heavy-duty ceramic pickle for domestic provisioning [craft_industrialised_food_drink_packaged_0045_heavy_duty_pickle]",
		Category = "Food, drink and packaged consumables",
		Blurb = "Make a heavy-duty ceramic pickle for domestic provisioning from prepared ceramic stock.",
		Action = "making heavy-duty ceramic pickle for domestic provisioning",
		ActiveCraftItemSdesc = "an in-progress heavy-duty ceramic pickle for domestic provisioning craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2442.0, Material: "ceramic", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2442 grams of ceramic", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_food_drink_packaged_0045_heavy_duty_pickle", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a heavy-duty ceramic pickle for domestic provisioning (@industrialised_food_drink_packaged_0045_heavy_duty_pickle)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};
}
